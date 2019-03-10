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
        private static readonly MethodInfo IsDBNull = typeof(DataLoader).GetMethod("IsDBNull", new Type[] { typeof(int) });
        private static readonly MethodInfo GetValue = typeof(DataLoader).GetMethod("GetValue", new Type[] { typeof(int) });

        private static SafeDictionary<Type, EntityPropertyInfo[]> TypeSetPropertyInfos = new SafeDictionary<Type, EntityPropertyInfo[]>();
        private static SafeDictionary<Type, object> Creators = new SafeDictionary<Type, object>();


        public static Func<DataLoader, T> GetCreator<T>()
        {
            object func = null;
            if (Creators.TryGetValue(typeof(T), out func))
                return (Func<DataLoader, T>)func;

            Type classType = typeof(T);
            EntityPropertyInfo[] ppIndex = null;
            if (!TypeSetPropertyInfos.TryGetValue(typeof(T), out ppIndex))
            {
                var ppIdx = new List<EntityPropertyInfo>();
                PropertyInfo[] prptys = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                for (int i = 0; i < prptys.Length; i++)
                {
                    if (prptys[i].CanWrite)
                    {
                        ppIdx.Add(new EntityPropertyInfo(prptys[i]));
                    }
                }
                ppIndex = new EntityPropertyInfo[ppIdx.Count];
                ppIdx.CopyTo(ppIndex);
                GC.KeepAlive(ppIndex);
                TypeSetPropertyInfos.Add(classType, ppIndex);
            }
            DynamicMethod method;
            if (classType.IsInterface)
                method = new DynamicMethod("Create" + classType.FullName, typeof(T), new Type[] { typeof(DataLoader) }, classType.Module, true);
            else
                method = new DynamicMethod("Create" + classType.FullName, typeof(T), new Type[] { typeof(DataLoader) }, classType, true);
            ILGenerator il = method.GetILGenerator();
            LocalBuilder result = il.DeclareLocal(classType);
            il.Emit(OpCodes.Nop);
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
                    il.Emit(OpCodes.Callvirt, IsDBNull);
                    il.Emit(OpCodes.Brtrue, lb);
                    il.Emit(OpCodes.Ldloc, result);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldc_I4, i);
                    il.Emit(OpCodes.Callvirt, GetValue);
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
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Nop);
                    il.MarkLabel(lb);
                }
            }
            il.Emit(OpCodes.Ldloc, result);
            il.Emit(OpCodes.Ret); 

            object reator = method.CreateDelegate(typeof(Func<DataLoader, T>));
            Creators.Add(classType, reator);
            GC.KeepAlive(reator);
            return (Func<DataLoader, T>)reator;
        }

        public static DataLoader GetDataLoader<T>(IDataReader Reader)
        {
            Type classType = typeof(T);
            EntityPropertyInfo[] ppIndex = null;
            if (!TypeSetPropertyInfos.TryGetValue(typeof(T), out ppIndex))
            {
                var ppIdx = new List<EntityPropertyInfo>();
                PropertyInfo[] prptys = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                for (int i = 0; i < prptys.Length; i++)
                {
                    if (prptys[i].CanWrite)
                    {
                        ppIdx.Add(new EntityPropertyInfo(prptys[i]));
                    }
                }
                ppIndex = new EntityPropertyInfo[ppIdx.Count];
                ppIdx.CopyTo(ppIndex);
                GC.KeepAlive(ppIndex);
                TypeSetPropertyInfos.Add(classType, ppIndex);
            }
            DataLoader dloader = new DataLoader(Reader, ppIndex);
            return dloader;
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
    internal class DataLoader
    {
        IDataReader Reader;
        EntityPropertyInfo[] MapInfo = null;
        int[] MapColIdx = new int[0];
        MapType[] MapTypeIdx = new MapType[0];


        public DataLoader(IDataReader reader, EntityPropertyInfo[] ColumnMapping)
        {
            Reader = reader;
            if (ColumnMapping == null || ColumnMapping.Length == 0)
                throw new ODAException(40001, "ColumnMapping Error!");
            MapInfo = new EntityPropertyInfo[ColumnMapping.Length];
            MapColIdx = new int[ColumnMapping.Length];
            MapTypeIdx = new MapType[ColumnMapping.Length];

            for (int m = 0; m < ColumnMapping.Length; m++)
            {
                EntityPropertyInfo mp = new EntityPropertyInfo(ColumnMapping[m].OriginProperty);
                MapColIdx[m]  = -1;
                MapTypeIdx[m] = MapType.ChangeType;
                for (int n = 0; n < Reader.FieldCount; n++)
                {
                    if (Reader.GetName(n) == ColumnMapping[m].PropertyName)
                    {
                        MapColIdx[m] = n; 
                        if (Reader.GetFieldType(n) == ColumnMapping[m].UnderlyingType)
                            MapTypeIdx[m] = MapType.Match;
                        else if (ColumnMapping[m].OriginType.IsEnum)
                            MapTypeIdx[m] = MapType.EnumType;
                        break;
                    }
                }
                MapInfo[m] = mp;
            }
        }

        public bool IsDBNull(int i)
        {
            if (MapColIdx[i] > -1)
                return Reader.IsDBNull(MapColIdx[i]);
            return true;
        } 
        public object GetValue(int i)
        {
            var obj = Reader.GetValue(MapColIdx[i]);
            try
            {
                switch (MapTypeIdx[i])
                {
                    case MapType.Match:
                        return obj; 
                    case MapType.EnumType: 
                        if (MapInfo[i].OriginType.IsEnumDefined(obj))
                        { 
                            return Enum.ToObject(MapInfo[i].UnderlyingType, obj);
                        }
                        return Enum.Parse(MapInfo[i].OriginType, Enum.GetNames(MapInfo[i].OriginType)[0]);
                    
                    case MapType.ChangeType:
                    default:
                        return Convert.ChangeType(obj, MapInfo[i].UnderlyingType, CultureInfo.CurrentCulture);
                } 
            }
            catch
            {
                throw new ODAException(40000, string.Format("Reading value from column : [{0}] , Set [{1}] to [{2}] ", MapInfo[i].PropertyName, obj, MapInfo[i].OriginType));
            }
        }
    }

    internal class EntityPropertyInfo
    {
        #region 实体信息
        public PropertyInfo OriginProperty { get; private set; }
        public string PropertyName
        {
            get;
            private set;
        }
        public Type OriginType
        {
            get;
            private set;
        }
        public Type UnderlyingType
        {
            get;
            private set;
        }
        #endregion 

        public EntityPropertyInfo(PropertyInfo Property)
        {
            if (Property == null)
                throw new ArgumentNullException(nameof(Property));
            OriginProperty = Property;
            OriginType = Property.PropertyType;
            PropertyName = Property.Name;
            UnderlyingType = (ODAReflection.IsNullable(OriginType.UnderlyingSystemType) && ODAReflection.IsNullableType(OriginType.UnderlyingSystemType)) ? Nullable.GetUnderlyingType(OriginType.UnderlyingSystemType) : OriginType.UnderlyingSystemType;
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
    internal enum MapType
    {
        Match = 1,
        ChangeType = 2,
        EnumType = 3,
    }
 
}
