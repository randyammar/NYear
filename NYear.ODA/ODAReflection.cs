using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Creative.ODA
{
    internal class ODAReflection
    {
        private delegate object CreateObject();
        private static SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
        internal static object FastCreateInstance(Type objtype)
        {
            try
            {
                CreateObject c = null;
                if (_constrcache.TryGetValue(objtype, out c))
                {
                    return c();
                }
                else
                {
                    if (objtype.IsClass)
                    {
                        DynamicMethod dynMethod = new DynamicMethod("_", objtype, null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                        ilGen.Emit(OpCodes.Ret);
                        c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrcache.Add(objtype, c);
                    }
                    else // structs
                    {
                        DynamicMethod dynMethod = new DynamicMethod("_", typeof(object), null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        var lv = ilGen.DeclareLocal(objtype);
                        ilGen.Emit(OpCodes.Ldloca_S, lv);
                        ilGen.Emit(OpCodes.Initobj, objtype);
                        ilGen.Emit(OpCodes.Ldloc_0);
                        ilGen.Emit(OpCodes.Box, objtype);
                        ilGen.Emit(OpCodes.Ret);
                        c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrcache.Add(objtype, c);
                    }
                    return c();
                }
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'",
                    objtype.FullName, objtype.AssemblyQualifiedName), exc);
            }
        }
        internal static object ChangeType(object Val, Type TargetType)
        {
            if (Val == null)
            {
                if (TargetType.IsClass || TargetType.IsArray)
                    return null;
                if (TargetType.IsGenericType && TargetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return null;
            }
            if (TargetType.IsInstanceOfType(Val))
                return Val;
            Type baseType = null;
            if (TargetType.IsGenericType && TargetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                baseType = Nullable.GetUnderlyingType(TargetType);
            else
                baseType = TargetType;
            return Convert.ChangeType(Val, TargetType, CultureInfo.InvariantCulture);
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
