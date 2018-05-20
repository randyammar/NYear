using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text;

namespace NYear.ODA
{
    internal class ODAReflection
    {
        private static readonly MethodInfo isDBNull = typeof(DetaLoader).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo getValue = typeof(DetaLoader).GetMethod("GetValue", new Type[] { typeof(int) });

        public static SafeDictionary<Type, ODAProperty[]> TypeSetPropertyInfos = new SafeDictionary<Type, ODAProperty[]>();
        private static SafeDictionary<Type, object> Creators = new SafeDictionary<Type, object>();
         
        public static Func<DetaLoader, T> CreateInstance<T>()
        { 
            object func = null;
            if (Creators.TryGetValue(typeof(T), out func))
                return (Func<DetaLoader, T>)func;

            Type classType = typeof(T);
            ODAProperty[] ppIndex = null ;
            if (!TypeSetPropertyInfos.TryGetValue(typeof(T), out ppIndex))
            {
                var ppIdx  = new List<ODAProperty>();
                PropertyInfo[] prptys = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var pi in prptys)
                {
                    if (pi.CanWrite)
                    {
                        ppIdx.Add(new ODAProperty(pi));
                    }
                }
                ppIndex = new ODAProperty[ppIdx.Count]; 
                ppIdx.CopyTo(ppIndex);
                GC.KeepAlive(ppIndex);
                TypeSetPropertyInfos.Add(classType, ppIndex);
            }
            DynamicMethod method;
            if( classType.IsInterface)
                method = new DynamicMethod("Create" + classType.FullName, typeof(T), new Type[] { typeof(DetaLoader) }, classType.Module,true);
            else
                method = new DynamicMethod("Create" + classType.FullName, typeof(T), new Type[] { typeof(DetaLoader) }, classType, true);
            ILGenerator il = method.GetILGenerator(); 
            LocalBuilder result = il.DeclareLocal(classType);
            il.Emit(OpCodes.Newobj, classType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc, result); 

            for (int i = 0; i < ppIndex.Length; i++)
            {
                var setter = ppIndex[i].OriginProperty.GetSetMethod(true);
                if (setter != null)
                {
                    Label lb = il.DefineLabel();
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Callvirt, isDBNull);
                    il.Emit(OpCodes.Brtrue, lb);
                    il.Emit(OpCodes.Ldloc, result);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i); 
                    il.Emit(OpCodes.Callvirt, getValue); 

                    if (ppIndex[i].OriginType.IsValueType)
                    {
                        il.Emit(OpCodes.Unbox_Any, ppIndex[i].OriginType);
                    }
                    else
                    {
                        il.Emit(OpCodes.Castclass, ppIndex[i].OriginType);
                    } 
                    if (setter.IsFinal || !setter.IsVirtual)
                    {
                        il.Emit(OpCodes.Call, setter);
                    }
                    else
                    {
                        il.Emit(OpCodes.Callvirt, setter);
                    } 
                    il.MarkLabel(lb); 
                }
            }
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret);
             
            object reator = method.CreateDelegate(typeof(Func<DetaLoader, T>)); 
            Creators.Add(classType, reator);
            GC.KeepAlive(reator);
            return (Func<DetaLoader, T>)reator;
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

    internal class DetaLoader
    {
        IDataReader reader;
        ODAMappingInfo mapping;
        public DetaLoader(IDataReader Reader, ODAMappingInfo Mapping)
        {
            reader = Reader;
            mapping = Mapping; 
        } 
        public bool IsDBNull(int i)
        { 
            if (mapping.ReadIndex[i] >= 0)
                return reader.IsDBNull(mapping.ReadIndex[i]);
            return true;
        }
         
        public object GetValue(int i)
        {
            if (mapping.ReadIndex[i] >= 0)
            {
                var obj = reader.GetValue(mapping.ReadIndex[i]);
                switch (mapping.MapInfos[i])
                {
                    case MapResult.Match:  
                        return obj;
                    case MapResult.Convert: 
                        return Convert.ChangeType(reader.GetValue(mapping.ReadIndex[i]), mapping.Pptys[i].NonNullableUnderlyingType, CultureInfo.CurrentCulture);
                    case MapResult.EnumType:
                        if (obj is string v)
                        {
                            return Enum.Parse(mapping.Pptys[i].NonNullableUnderlyingType, v);
                        }
                        else
                        {
                            return Enum.ToObject(mapping.Pptys[i].NonNullableUnderlyingType, obj);
                        } 
                }
            } 
            /////实体中这个属性，但DataReader没有这个字段
            return DefaultValue(mapping.Pptys[i].NonNullableUnderlyingType);
        }

        public static object DefaultValue(Type TargetType)
        {
            if (TargetType == typeof(DateTime))
                return new DateTime(1900, 1, 1);
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
            return FormatterServices.GetUninitializedObject(TargetType);
        }
    }
  
    internal class ODAMappingInfo
    {
        public int[] ReadIndex;
        public ODAProperty[] Pptys;
        public MapResult[] MapInfos;
        public ODAMappingInfo(IDataReader Reader, ODAProperty[] Propertys,bool ByIndex = false)
        {
            ReadIndex = new int[Propertys.Length];
            Pptys = Propertys;
            MapInfos = new MapResult[Propertys.Length];
            if (ByIndex)
            {
                for (int i = 0; i < ReadIndex.Length && i < Reader.FieldCount; i++)
                {
                    ReadIndex[i] = i;
                    MapInfos[i] = MapResult.Convert; 
                    Type rType = Reader.GetFieldType(i);
                    if (Pptys[i].NonNullableUnderlyingType == rType)
                    {
                        MapInfos[i] = MapResult.Match;
                    }
                    else if (Pptys[i].OriginType.IsEnum)
                    {
                        MapInfos[i] = MapResult.EnumType;
                    }
                    break; 
                } 
            }
            else
            {
                for (int i = 0; i < ReadIndex.Length; i++)
                {
                    ReadIndex[i] = -1;
                    MapInfos[i] = MapResult.Convert;
                    for (int num = 0; num < Reader.FieldCount; num++)
                    {
                        if (Pptys[i].PropertyName == Reader.GetName(num))
                        {
                            ReadIndex[i] = Reader.GetOrdinal(Pptys[i].PropertyName);
                            Type rType = Reader.GetFieldType(num);
                            if (Pptys[i].NonNullableUnderlyingType == rType)
                            {
                                MapInfos[i] = MapResult.Match;
                            }
                            else if (Pptys[i].OriginType.IsEnum)
                            {
                                MapInfos[i] = MapResult.EnumType;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    internal enum MapResult
    {
        Match = 1,
        Convert = 2,
        EnumType = 3,
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
        public PropertyInfo OriginProperty { get; private set; }

        public bool IsNullableType { get; private set; }

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
            if (Property == null)
                throw new ArgumentNullException(nameof(Property));
            OriginProperty = Property;
            OriginType = Property.PropertyType;
            PropertyName = Property.Name;
            IsNullableType = ODAReflection.IsNullableType(Property.PropertyType);
        }
    }
    internal class SafeDictionary<TKey, TValue>
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
