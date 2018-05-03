using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace NYear.ODA
{
    internal sealed class ODAReflectionFactory : ODAReflectionObject<object>
    {
        private static SafeDictionary<Type, object> _constrcache = new SafeDictionary<Type, object>();
        public static ODAReflectionObject<T> GetConstructor<T>()
        {
            object RflOject;
            Type dType = typeof(T);
            if (!_constrcache.TryGetValue(dType, out RflOject))
            {
                Func<T> creator = ODAReflection.CreateDefaultConstructor<T>();
                RflOject = new ODAReflectionObject<T>();
                ((ODAReflectionObject<T>)RflOject).Creator = creator;
                PropertyInfo[] prptys = dType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var pi in prptys)
                {
                    if (pi.CanWrite)
                    {
                        var settor = ODAReflection.CreateSet<T>(pi);
                        ((ODAReflectionObject<T>)RflOject).Setters.Add(pi.Name, settor);
                        ((ODAReflectionObject<T>)RflOject).GetPropertys.Add(new ODAProperty(pi));
                    }
                    if (pi.CanRead)
                    {
                        var gettor = ODAReflection.CreateGet<T>(pi);
                        ((ODAReflectionObject<T>)RflOject).Getters.Add(pi.Name, gettor);
                        ((ODAReflectionObject<T>)RflOject).SetPropertys.Add(new ODAProperty(pi));
                    }
                }
            }
            return (ODAReflectionObject<T>)RflOject;
        }
    }
    internal class ODAReflectionObject<T>
    {
        public Func<T> Creator { get; set; }
        public Dictionary<string, Action<T, object>> Setters { get; } = new Dictionary<string, Action<T, object>>();
        public Dictionary<string, Func<T, object>> Getters { get; } = new Dictionary<string, Func<T, object>>(); 
        public List<ODAProperty> GetPropertys { get; } = new List<ODAProperty>(); 
        public List<ODAProperty> SetPropertys { get; } = new List<ODAProperty>();

        public T CreateInstance()
        {
            return Creator();
        }

        public object GetValue(T target, PropertyInfo property)
        {
            return GetValue(target, property.Name);
        }

        public object GetValue(T target, string property)
        {
            if (Getters.ContainsKey(property)) 
                return Getters[property](target); 
            throw new Exception();
        }

        public void SetValue(T target, PropertyInfo property, object value)
        {
            SetValue(target, property.Name, value);
        }

        public void SetValue(T target, string property, object value)
        {
            if (Setters.ContainsKey(property))
                Setters[property](target, value);
        }
    }

    internal class ODAProperty
    {
        public Type OriginType
        {
            get;
            private set;
        }
        public string PropertyName
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
                _NonNullableUnderlyingType = (ODAReflection.IsNullable(OriginType.UnderlyingSystemType) && ODAReflection.IsNullableType(OriginType.UnderlyingSystemType)) ? Nullable.GetUnderlyingType(OriginType.UnderlyingSystemType) : OriginType.UnderlyingSystemType;
                return _NonNullableUnderlyingType;
            }
        }
         
        public ODAProperty(PropertyInfo Property)
        {
            if(Property == null)
                throw new ArgumentNullException(nameof(Property));
            OriginType = Property.PropertyType;
            PropertyName = Property.Name;
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
        public static object ChangeType(object Val, ODAProperty TargetType)
        {
            if (TargetType.OriginType.IsInstanceOfType(Val))
                return Val;

            if (Val == null || Val == DBNull.Value)
            {
                if (IsNullable(TargetType.OriginType))
                {
                    return Val;
                }
                else
                {
                    return DefaultValue(TargetType.NonNullableUnderlyingType);
                }
            }


            //decimal.Parse(

          //  TargetType.NonNullableUnderlyingType.GetMethod("Parse", BindingFlags.Public| BindingFlags.Static,




            if (TargetType.OriginType.IsEnum)
            {
                if (Val is string)
                    return Enum.Parse(TargetType.OriginType, Val as string);
                else
                    return Enum.ToObject(TargetType.OriginType, Val);
            }
            return Convert.ChangeType(Val, TargetType.NonNullableUnderlyingType, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TargetType"></param>
        /// <returns></returns>
        public static object DefaultValue(Type TargetType)
        { 
            if (TargetType == typeof(DateTime))
                return new DateTime(1900, 1, 1, 0, 0, 0);
            if (TargetType == typeof(Guid))
                return new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
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
