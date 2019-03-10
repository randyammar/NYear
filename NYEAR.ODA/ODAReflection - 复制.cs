//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Runtime.Serialization;
//using System.Text;

//namespace NYear.ODA
//{
//    internal class ODAReflection
//    {
//        private static readonly MethodInfo IsDBNull = typeof(DataLoader).GetMethod("IsDBNull", new Type[] { typeof(int) });
//        private static readonly MethodInfo IsMatch = typeof(DataLoader).GetMethod("IsMatch", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetEnum = typeof(DataLoader).GetMethod("GetEnum", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetValue = typeof(DataLoader).GetMethod("GetValue", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetBoolean = typeof(DataLoader).GetMethod("GetBoolean", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetByte = typeof(DataLoader).GetMethod("GetByte", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetBytes = typeof(DataLoader).GetMethod("GetBytes", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetChar = typeof(DataLoader).GetMethod("GetChar", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetDateTime = typeof(DataLoader).GetMethod("GetDateTime", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetDecimal = typeof(DataLoader).GetMethod("GetDecimal", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetDouble = typeof(DataLoader).GetMethod("GetDouble", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetFloat = typeof(DataLoader).GetMethod("GetFloat", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetGuid = typeof(DataLoader).GetMethod("GetGuid", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetInt16 = typeof(DataLoader).GetMethod("GetInt16", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetInt32 = typeof(DataLoader).GetMethod("GetInt32", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetInt64 = typeof(DataLoader).GetMethod("GetInt64", new Type[] { typeof(int) });
//        private static readonly MethodInfo GetString = typeof(DataLoader).GetMethod("GetString", new Type[] { typeof(int) });



//        private static SafeDictionary<Type, MappingInfo[]> TypeSetPropertyInfos = new SafeDictionary<Type, MappingInfo[]>();
//        private static SafeDictionary<Type, object> Creators = new SafeDictionary<Type, object>();

//        public static Func<DataLoader, T> GetCreator<T>()
//        {
//            object func = null;
//            if (Creators.TryGetValue(typeof(T), out func))
//                return (Func<DataLoader, T>)func;

//            Type classType = typeof(T);
//            MappingInfo[] ppIndex = null;
//            if (!TypeSetPropertyInfos.TryGetValue(typeof(T), out ppIndex))
//            {
//                var ppIdx = new List<MappingInfo>();
//                PropertyInfo[] prptys = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

//                for (int i = 0; i < prptys.Length; i++)
//                {
//                    if (prptys[i].CanWrite)
//                    {
//                        ppIdx.Add(new MappingInfo(prptys[i]));
//                    }
//                }
//                ppIndex = new MappingInfo[ppIdx.Count];
//                ppIdx.CopyTo(ppIndex);
//                GC.KeepAlive(ppIndex);
//                TypeSetPropertyInfos.Add(classType, ppIndex);
//            }
//            DynamicMethod method;
//            if (classType.IsInterface)
//                method = new DynamicMethod("Create" + classType.FullName, typeof(T), new Type[] { typeof(DataLoader) }, classType.Module, true);
//            else
//                method = new DynamicMethod("Create" + classType.FullName, typeof(T), new Type[] { typeof(DataLoader) }, classType, true);
//            ILGenerator il = method.GetILGenerator();
//            LocalBuilder result = il.DeclareLocal(classType);
//            il.Emit(OpCodes.Nop);
//            il.Emit(OpCodes.Newobj, classType.GetConstructor(Type.EmptyTypes));
//            il.Emit(OpCodes.Stloc, result);

//            for (int i = 0; i < ppIndex.Length; i++)
//            {
//                var setter = ppIndex[i].OriginProperty.GetSetMethod(true);
//                if (setter != null)
//                {
//                    Label lb = il.DefineLabel();
//                    Label lbMatchTrue = il.DefineLabel();
//                    Label lbMatchFalse = il.DefineLabel();

//                    il.Emit(OpCodes.Ldarg_0);
//                    il.Emit(OpCodes.Ldc_I4, i);
//                    il.Emit(OpCodes.Callvirt, IsDBNull);
//                    il.Emit(OpCodes.Brtrue, lb);

//                    il.Emit(OpCodes.Ldarg_0);
//                    il.Emit(OpCodes.Ldc_I4, i);
//                    il.Emit(OpCodes.Callvirt, IsMatch);
//                    il.Emit(OpCodes.Brfalse, lbMatchFalse);
//                    il.Emit(OpCodes.Ldloc, result);
//                    il.Emit(OpCodes.Ldarg_0);
//                    il.Emit(OpCodes.Ldc_I4, i);
//                    if (ppIndex[i].UnderlyingType == typeof(bool))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetBoolean);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(bool?).GetConstructor(new Type[] { typeof(bool) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(byte))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetByte);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(byte?).GetConstructor(new Type[] { typeof(byte) }));

//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(byte[]))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetBytes);
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(char))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetChar);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(char?).GetConstructor(new Type[] { typeof(char) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(DateTime))
//                    {
//                        il.Emit(OpCodes.Call, GetDateTime);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));

//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(decimal))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetDecimal);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(double))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetDouble);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(double?).GetConstructor(new Type[] { typeof(double) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(float))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetFloat);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(float?).GetConstructor(new Type[] { typeof(float) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(Guid))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetGuid);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(Guid?).GetConstructor(new Type[] { typeof(Guid) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(short))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetInt16);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(short?).GetConstructor(new Type[] { typeof(short) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(int))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetInt32);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(int?).GetConstructor(new Type[] { typeof(int) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(long))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetInt64);
//                        if (ppIndex[i].OriginType != ppIndex[i].UnderlyingType)
//                            il.Emit(OpCodes.Newobj, typeof(long?).GetConstructor(new Type[] { typeof(long) }));
//                    }
//                    else if (ppIndex[i].UnderlyingType == typeof(string))
//                    {
//                        il.Emit(OpCodes.Callvirt, GetString);
//                    }
//                    else
//                    {
//                        if (ppIndex[i].IsEnum)
//                        {
//                            il.Emit(OpCodes.Callvirt, GetEnum);
//                        }
//                        else
//                        {
//                            il.Emit(OpCodes.Callvirt, GetValue);
//                        }
//                        if (ppIndex[i].OriginType.IsValueType)
//                        {
//                            il.Emit(OpCodes.Unbox_Any, ppIndex[i].OriginType);
//                        }
//                        else
//                        {
//                            il.Emit(OpCodes.Castclass, ppIndex[i].OriginType);
//                        }
//                    }

//                    il.Emit(OpCodes.Br, lbMatchTrue);
//                    il.MarkLabel(lbMatchFalse);
//                    il.Emit(OpCodes.Ldloc, result);
//                    il.Emit(OpCodes.Ldarg_0);
//                    il.Emit(OpCodes.Ldc_I4, i);
//                    il.Emit(OpCodes.Callvirt, GetValue);
//                    if (ppIndex[i].OriginType.IsValueType)
//                    {
//                        il.Emit(OpCodes.Unbox_Any, ppIndex[i].OriginType);
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Castclass, ppIndex[i].OriginType);
//                    }
//                    il.MarkLabel(lbMatchTrue);
//                    if (setter.IsFinal || !setter.IsVirtual)
//                    {
//                        il.Emit(OpCodes.Call, setter);
//                    }
//                    else
//                    {
//                        il.Emit(OpCodes.Callvirt, setter);
//                    }
//                    il.Emit(OpCodes.Nop);
//                    il.Emit(OpCodes.Nop);
//                    il.MarkLabel(lb);
//                }
//            }
//            il.Emit(OpCodes.Ldloc, result);
//            il.Emit(OpCodes.Ret);

//            object reator = method.CreateDelegate(typeof(Func<DataLoader, T>));
//            Creators.Add(classType, reator);
//            GC.KeepAlive(reator);
//            return (Func<DataLoader, T>)reator;
//        }

//        public static DataLoader GetDataLoader<T>(IDataReader Reader)
//        {
//            Type classType = typeof(T);
//            MappingInfo[] ppIndex = null;
//            if (!TypeSetPropertyInfos.TryGetValue(typeof(T), out ppIndex))
//            {
//                var ppIdx = new List<MappingInfo>();
//                PropertyInfo[] prptys = classType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
//                for (int i = 0; i < prptys.Length; i++)
//                {
//                    if (prptys[i].CanWrite)
//                    {
//                        ppIdx.Add(new MappingInfo(prptys[i]));
//                    }
//                }
//                ppIndex = new MappingInfo[ppIdx.Count];
//                ppIdx.CopyTo(ppIndex);
//                GC.KeepAlive(ppIndex);
//                TypeSetPropertyInfos.Add(classType, ppIndex);
//            }
//            DataLoader dloader = new DataLoader(Reader, ppIndex);
//            return dloader;
//        }

//        public static bool IsNullable(Type t)
//        {
//            if (t.IsValueType)
//            {
//                return IsNullableType(t);
//            }
//            return true;
//        }

//        public static bool IsNullableType(Type t)
//        {
//            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
//        }
//    }
//    internal class DataLoader
//    {
//        IDataReader Reader;
//        MappingInfo[] Mapping = null;

         


//        public EntityMapp CreateEntity(IDataReader Reader, int[] Mapped, bool[] Match)
//        {
//            EntityMapp en = new EntityMapp();
//            if (Mapped[0] > -1 && Reader.IsDBNull(Mapped[0]))
//            {
//                if(Match[0])
//                {
//                    en.pp1 = Reader.GetInt32(Mapped[0]);
//                }
//                else
//                {
//                    en.pp1 = (int)Convert.ChangeType(Reader.GetValue(Mapped[0]), typeof(int), CultureInfo.CurrentCulture);
//                }
//            }
//            if (Mapped[1] > -1 && Reader.IsDBNull(Mapped[1]))
//            {
//                if (Match[1])
//                {
//                    en.pp1 = Reader.GetInt32(Mapped[1]);
//                }
//                else
//                {
//                    en.pp1 = (int)Convert.ChangeType(Reader.GetValue(Mapped[1]), typeof(int), CultureInfo.CurrentCulture);
//                }
//            }

//            if (Mapped[1] > -1 && Reader.IsDBNull(Mapped[1]))
//            {
//                if (Match[1])
//                {
//                    en.pp7 = Reader.GetString(Mapped[1]);
//                }
//                else
//                {
//                    en.pp7 = Reader.GetValue(Mapped[1]).ToString();
//                }
//            }

//            return en;

//        }


//        public DataLoader(IDataReader reader, MappingInfo[] ColumnMapping)
//        {
//            Reader = reader; 
//            if (ColumnMapping == null || ColumnMapping.Length == 0)
//                throw new ODAException(40001, "ColumnMapping Error!");
//            Mapping = new MappingInfo[ColumnMapping.Length];
//            for (int m = 0; m < ColumnMapping.Length; m++)
//            {
//                MappingInfo mp = new MappingInfo(ColumnMapping[m].OriginProperty);
//                mp.ReadIndex = -1;
//                mp.IsMath = false;
//                for (int n = 0; n < Reader.FieldCount; n++)
//                {
//                    if (Reader.GetName(n) == ColumnMapping[m].PropertyName)
//                    {
//                        mp.ReadIndex = n;
//                        mp.IsMath = Reader.GetFieldType(n) == ColumnMapping[m].UnderlyingType;
//                        break;
//                    }
//                }
//                Mapping[m] = mp;
//            }
//        }

//        public bool IsDBNull(int i)
//        {
//            if (Mapping[i].ReadIndex > -1)
//                return Reader.IsDBNull(Mapping[i].ReadIndex);
//            return true;
//        }

//        public bool IsMatch(int i)
//        {
//            return Mapping[i].IsMath;
//        }
//        public object GetValue(int i)
//        {
//            try
//            {
//                return Convert.ChangeType(Reader.GetValue(Mapping[i].ReadIndex), Mapping[i].UnderlyingType, CultureInfo.CurrentCulture);
//            }
//            catch
//            {
//                throw new ODAException(40002, string.Format("Reading value from column : [{0}] , Set [{1}] to [{2}] ", Mapping[i].PropertyName, Reader.GetValue(Mapping[i].ReadIndex).ToString(), Mapping[i].UnderlyingType.ToString()));
//            }
//        }
//        public object GetEnum(int i)
//        {
//            try
//            {
//                return Enum.Parse(Mapping[i].UnderlyingType, Reader.GetValue(Mapping[i].ReadIndex).ToString(), true);
//            }
//            catch
//            {
//                throw new ODAException(40003, string.Format("Reading value from column : [{0}] , Set [{1}] to [{2}] ", Mapping[i].PropertyName, Reader.GetValue(Mapping[i].ReadIndex).ToString(), Mapping[i].UnderlyingType.ToString()));
//            }
//        }
//        public bool GetBoolean(int i)
//        {
//            return Reader.GetBoolean(Mapping[i].ReadIndex);
//        }
//        public byte GetByte(int i)
//        {
//            return Reader.GetByte(Mapping[i].ReadIndex);
//        }
//        public byte[] GetBytes(int i)
//        {
//            return Reader.GetValue(Mapping[i].ReadIndex) as byte[];
//        }
//        public char GetChar(int i)
//        {
//            return Reader.GetChar(Mapping[i].ReadIndex);
//        }
//        public DateTime GetDateTime(int i)
//        {
//            return Reader.GetDateTime(Mapping[i].ReadIndex);
//        }
//        public decimal GetDecimal(int i)
//        {
//            return Reader.GetDecimal(Mapping[i].ReadIndex);
//        }
//        public double GetDouble(int i)
//        {
//            return Reader.GetDouble(Mapping[i].ReadIndex);
//        }
//        public float GetFloat(int i)
//        {
//            return Reader.GetFloat(Mapping[i].ReadIndex);
//        }
//        public Guid GetGuid(int i)
//        {
//            return Reader.GetGuid(Mapping[i].ReadIndex);
//        }
//        public short GetInt16(int i)
//        {
//            return Reader.GetInt16(Mapping[i].ReadIndex);
//        }
//        public int GetInt32(int i)
//        {
//            return Reader.GetInt32(Mapping[i].ReadIndex);
//        }
//        public long GetInt64(int i)
//        {
//            return Reader.GetInt64(Mapping[i].ReadIndex);
//        }
//        public string GetString(int i)
//        {
//            return Reader.GetString(Mapping[i].ReadIndex);
//        }
//    }
//    internal class MappingInfo
//    {
//        #region 映射信息
//        public int ReadIndex { get; set; }
//        public bool IsMath { get; set; }
//        #endregion

//        #region 实体信息
//        public PropertyInfo OriginProperty { get; private set; }
//        public string PropertyName
//        {
//            get;
//            private set;
//        }
//        public Type OriginType
//        {
//            get;
//            private set;
//        }
//        public Type UnderlyingType
//        {
//            get;
//            private set;
//        }
//        public bool IsEnum { get; private set; }
//        #endregion 

//        public MappingInfo(PropertyInfo Property)
//        {
//            if (Property == null)
//                throw new ArgumentNullException(nameof(Property));
//            OriginProperty = Property;
//            OriginType = Property.PropertyType;
//            PropertyName = Property.Name;
//            IsEnum = Property.PropertyType.IsEnum;
//            UnderlyingType = (ODAReflection.IsNullable(OriginType.UnderlyingSystemType) && ODAReflection.IsNullableType(OriginType.UnderlyingSystemType)) ? Nullable.GetUnderlyingType(OriginType.UnderlyingSystemType) : OriginType.UnderlyingSystemType;
//        }
//    }
//    internal class SafeDictionary<TKey, TValue>
//    {
//        private readonly object _Padlock = new object();
//        private readonly Dictionary<TKey, TValue> _Dictionary;

//        public SafeDictionary(int capacity)
//        {
//            _Dictionary = new Dictionary<TKey, TValue>(capacity);
//        }
//        public SafeDictionary()
//        {
//            _Dictionary = new Dictionary<TKey, TValue>();
//        }

//        public bool TryGetValue(TKey key, out TValue value)
//        {
//            lock (_Padlock)
//                return _Dictionary.TryGetValue(key, out value);
//        }

//        public int Count { get { lock (_Padlock) return _Dictionary.Count; } }

//        public TValue this[TKey key]
//        {
//            get
//            {
//                lock (_Padlock)
//                    return _Dictionary[key];
//            }
//            set
//            {
//                lock (_Padlock)
//                    _Dictionary[key] = value;
//            }
//        }

//        public void Add(TKey key, TValue value)
//        {
//            lock (_Padlock)
//            {
//                if (_Dictionary.ContainsKey(key) == false)
//                    _Dictionary.Add(key, value);
//            }
//        }
//    }


//    public class EntityMapp
//    {
//        public int pp1 { get; set; }
//        public int? pp2 { get; set; }
//        public decimal pp3 { get; set; }
//        public decimal? pp4 { get; set; }
//        public DateTime pp5 { get; set; }
//        public DateTime? pp6 { get; set; }
//        public string pp7 { get; set; }
//    }
    
//}
