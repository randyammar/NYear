using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;

namespace NYear.ODA
{
    internal class ODAReflectionObject<T>
    {
        private static SafeDictionary<Type, object> _constrcache = new SafeDictionary<Type, object>();
        private ODAReflectionObject()
        {
        }
        public static ODAReflectionObject<A> GetConstructor<A>()
        { 
            object RflOject;
            Type dType = typeof(A);
            if (!_constrcache.TryGetValue(dType, out RflOject))
            {
                Func<A> creator = ODAReflection.CreateDefaultConstructor<A>();
                RflOject = new ODAReflectionObject<A>();
                ((ODAReflectionObject<A>)RflOject)._creator = creator;
                PropertyInfo[] prptys = dType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var pi in prptys)
                {
                    if (pi.CanWrite)
                    {
                        var settor = ODAReflection.CreateSet<A>(pi);
                        ((ODAReflectionObject<A>)RflOject)._setters.Add(pi.Name, settor);
                        ((ODAReflectionObject<A>)RflOject).GetPropertys.Add(pi.Name, new ODADataType(pi.PropertyType));
                    }
                    if (pi.CanRead)
                    {
                        var gettor = ODAReflection.CreateGet<A>(pi);
                        ((ODAReflectionObject<A>)RflOject)._getters.Add(pi.Name, gettor);
                        ((ODAReflectionObject<A>)RflOject).SetPropertys.Add(pi.Name, new ODADataType(pi.PropertyType));
                    }
                }
            }
            return (ODAReflectionObject<A>)RflOject;
        }

        private Func<T> _creator { get; set; } 
        private Dictionary<string, Action<T, object>> _setters = new Dictionary<string, Action<T, object>>();
        private Dictionary<string, Func<T, object>> _getters = new Dictionary<string, Func<T, object>>(); 
        private Dictionary<string, ODADataType> GetPropertys { get; } = new Dictionary<string, ODADataType>(); 
        private Dictionary<string, ODADataType> SetPropertys { get; } = new Dictionary<string, ODADataType>();

        public T CreateInstance()
        {
            return _creator();
        }

        public object GetValue(T target, PropertyInfo property)
        {
            return GetValue(target, property.Name);
        }

        public object GetValue(T target, string property)
        {
            if (_getters.ContainsKey(property)) 
                return _getters[property](target); 
            throw new Exception();
        }

        public void SetValue(T target, PropertyInfo property, object value)
        {
            SetValue(target, property.Name, value);
        }

        public void SetValue(T target, string property, object value)
        {
            if (_setters.ContainsKey(property))
            {
                object safeVal = ODAReflection.ChangeType(value, SetPropertys[property]);
                _setters[property](target, safeVal);
            }
        }
    }

    internal class ODADataType
    {
        public Type OriginType
        {
            get;
            private set;
        }
        Type _NonNullableUnderlyingType = null;
        public Type NonNullableUnderlyingType
        {
            get
            {
                if (_NonNullableUnderlyingType != null)
                    return _NonNullableUnderlyingType;
                _NonNullableUnderlyingType = OriginType.UnderlyingSystemType;
                _NonNullableUnderlyingType = (ODAReflection.IsNullable(_NonNullableUnderlyingType) && ODAReflection.IsNullableType(_NonNullableUnderlyingType)) ? Nullable.GetUnderlyingType(_NonNullableUnderlyingType) : _NonNullableUnderlyingType;

                return _NonNullableUnderlyingType;
            }
        }
         
        public ODADataType(Type type)
        {
            OriginType = type ?? throw new ArgumentNullException(nameof(type));
        }
    }

    internal class ODAReflection
    {
        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            DynamicMethod dynamicMethod = !owner.IsInterface
                ? new DynamicMethod(name, returnType, parameterTypes, owner, true)
                : new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);
            return dynamicMethod;
        }

        public static Func<T> CreateDefaultConstructor<T>()
        {
            Type type = typeof(T);
            DynamicMethod dynamicMethod = CreateDynamicMethod("Create" + type.FullName, typeof(T), Type.EmptyTypes, type);
            dynamicMethod.InitLocals = true;
            ILGenerator generator = dynamicMethod.GetILGenerator();

            if (type.IsValueType)
            {
                generator.DeclareLocal(type);
                generator.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null); 
                if (constructorInfo == null)
                {
                    throw new ArgumentException(string.Format("Could not get constructor for {0}.", type));
                }
                generator.Emit(OpCodes.Newobj, constructorInfo);
            } 
            generator.Emit(OpCodes.Ret); 
            return (Func<T>)dynamicMethod.CreateDelegate(typeof(Func<T>));
        }

        public static Func<T, object> CreateGet<T>(PropertyInfo propertyInfo)
        {
            DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + propertyInfo.Name, typeof(object), new[] { typeof(T) }, propertyInfo.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            MethodInfo getMethod = propertyInfo.GetGetMethod(true);
            if (getMethod == null)
            {
                throw new ArgumentException(string.Format("Property '{0}' does not have a getter.", propertyInfo.Name));
            }

            if (!getMethod.IsStatic)
            {
                var dtype = propertyInfo.DeclaringType;
                generator.Emit(OpCodes.Ldarg_0);
                if (dtype.IsValueType)
                {
                    generator.Emit(OpCodes.Unbox, dtype);
                }
                else
                {
                    generator.Emit(OpCodes.Castclass, dtype);
                }
            }
 
            if (getMethod.IsFinal || !getMethod.IsVirtual)
            {
                generator.Emit(OpCodes.Call, getMethod);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, getMethod);
            }
            var pType = propertyInfo.PropertyType;
            if (pType.IsValueType)
            {
                generator.Emit(OpCodes.Box, pType);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, pType);
            }
            generator.Emit(OpCodes.Ret);

            return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
        }

        public static Action<T, object> CreateSet<T>(PropertyInfo propertyInfo)
        {
            DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + propertyInfo.Name, null, new[] { typeof(T), typeof(object) }, propertyInfo.DeclaringType);
            ILGenerator generator = dynamicMethod.GetILGenerator();
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (!setMethod.IsStatic)
            {
                var dtype = propertyInfo.DeclaringType;
                generator.Emit(OpCodes.Ldarg_0);
                if (dtype.IsValueType)
                {
                    generator.Emit(OpCodes.Unbox, dtype);
                }
                else
                {
                    generator.Emit(OpCodes.Castclass, dtype);
                }
            }
            generator.Emit(OpCodes.Ldarg_1); 
            var pType = propertyInfo.PropertyType;
            if (pType.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, pType);
            }
            else
            {
                generator.Emit(OpCodes.Castclass, pType);
            } 
            if (setMethod.IsFinal || !setMethod.IsVirtual)
            {
                generator.Emit(OpCodes.Call, setMethod);
            }
            else
            {
                generator.Emit(OpCodes.Callvirt, setMethod);
            }
            generator.Emit(OpCodes.Ret);

            return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Val"></param>
        /// <param name="TargetType">Non Nullable Underlying Type</param>
        /// <returns></returns>
        public static object ChangeType(object Val, ODADataType TargetType)
        {
            if (TargetType == null || TargetType.OriginType.IsInstanceOfType(Val))
                return Val;
            if (Val == null || Val == DBNull.Value)
            {
                if (IsNullable(TargetType.OriginType))
                    return null;
                return DefaultValue(TargetType.OriginType);
            }
            if (TargetType.OriginType.IsEnum)
            {
                if (Val is string)
                    return Enum.Parse(TargetType.OriginType, Val as string);
                else
                    return Enum.ToObject(TargetType.OriginType, Val);
            }
 
            if (Val is BigInteger integer)
            {
                return FromBigInteger(integer, TargetType.NonNullableUnderlyingType);
            }
            return Convert.ChangeType(Val, TargetType.NonNullableUnderlyingType, CultureInfo.CurrentCulture);
        }

        public static object FromBigInteger(BigInteger i, Type targetType)
        {
            if (targetType == typeof(decimal))
            {
                return (decimal)i;
            }
            if (targetType == typeof(double))
            {
                return (double)i;
            }
            if (targetType == typeof(float))
            {
                return (float)i;
            }
            if (targetType == typeof(ulong))
            {
                return (ulong)i;
            }
            if (targetType == typeof(bool))
            {
                return i != 0;
            }

            try
            {
                return System.Convert.ChangeType((long)i, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Can not convert from BigInteger to {0}.", targetType), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TargetType"></param>
        /// <returns></returns>
        public static object DefaultValue(Type TargetType)
        {
            if (TargetType == typeof(DateTime))
                return new DateTime(1900, 1, 1, 0, 0, 1);
            if (TargetType == typeof(bool))
                return false;
            if (TargetType == typeof(byte))
                return 0;
            if (TargetType == typeof(char))
                return '\0';
            if (TargetType == typeof(decimal))
                return 0m;
            if (TargetType == typeof(double))
                return 0d;
            if (TargetType == typeof(float))
                return 0f;
            if (TargetType == typeof(int))
                return 0;
            if (TargetType == typeof(long))
                return 0L;
            if (TargetType == typeof(sbyte))
                return 0;
            if (TargetType == typeof(short))
                return (short)0;
            if (TargetType == typeof(uint))
                return 0u;
            if (TargetType == typeof(ulong))
                return 0UL;
            if (TargetType == typeof(ushort))
                return (ushort)0;
            if (TargetType.IsEnum)
                return Enum.ToObject(TargetType, 1);
           return  FormatterServices.GetUninitializedObject(TargetType);
        }
        public static bool IsNullable(Type t)
        {
            if (t.IsValueType)
            {
                return IsNullableType(t);
            }
            return true;
        }

        public static bool IsNullableType(Type t)
        {
            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
    
    internal sealed class SafeDictionary<TKey, TValue>
    {
        private readonly object _Padlock = new object();
        private readonly Dictionary<TKey, TValue> _Dictionary;

        public SafeDictionary(int capacity)
        {
            _Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public SafeDictionary()
        {
            _Dictionary = new Dictionary<TKey, TValue>();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_Padlock)
                return _Dictionary.TryGetValue(key, out value);
        }

        public int Count { get { lock (_Padlock) return _Dictionary.Count; } }

        public TValue this[TKey key]
        {
            get
            {
                lock (_Padlock)
                    return _Dictionary[key];
            }
            set
            {
                lock (_Padlock)
                    _Dictionary[key] = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            lock (_Padlock)
            {
                if (_Dictionary.ContainsKey(key) == false)
                    _Dictionary.Add(key, value);
            }
        }
    }
}
