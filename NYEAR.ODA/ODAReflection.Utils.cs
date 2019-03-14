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
    public static class MyDataReader
    { 
        public static object GetEnum(this IDataRecord dr, int i,Type EnumType )
        { 
            return Enum.ToObject(EnumType, dr.GetValue(i));
        } 
        public static byte[] GetBytes(this IDataRecord dr, int i)
        {
           return dr.GetValue(i) as byte[];
        }
        public static char[] GetChars(this IDataRecord dr, int i)
        {
            return dr.GetValue(i) as char[];
        } 
        public static sbyte GetSbyte(this IDataRecord dr, int i)
        {
            sbyte v = 0x00;
            sbyte.TryParse(dr.GetValue(i).ToString(), out v);
            return v;
        }   
        public static uint GetUInt32(this IDataRecord dr, int i)
        {
            uint v = 0x00;
            uint.TryParse(dr.GetValue(i).ToString(), out v);
            return v; 
        } 
        public static ulong GetUInt64(this IDataRecord dr,  int i)
        {
            ulong v = 0x00;
            ulong.TryParse(dr.GetValue(i).ToString(), out v);
            return v;
        } 
        public static ushort GetUInt16(this IDataRecord dr, int i)
        {
            ushort v = 0x0;
            ushort.TryParse(dr.GetValue(i).ToString(), out v);
            return v; 
        } 
         
        public static DateTimeOffset GetDateTimeOffset(this IDataRecord dr, int i)
        {
            return (DateTimeOffset)dr.GetValue(i);
        }
        public static DateTimeOffset GetDateTimeOffsetDateTime(this IDataRecord dr, int i)
        {
            return new DateTimeOffset(dr.GetDateTime(i));
        }

        public static Single GetSingleInt(this IDataRecord dr, int i)
        {
            return dr.GetInt32(i);
        }
        public static Single GetSingleFloat(this IDataRecord dr, int i)
        {
            return dr.GetFloat(i);
        }
        public static Single GetSingleLong(this IDataRecord dr, int i)
        {
            return dr.GetInt64(i);
        } 
        public static TimeSpan GetTimeSpanInt(this IDataRecord dr, int i)
        {
            return new TimeSpan(dr.GetInt32(i));
        }
        public static TimeSpan GetTimeSpanLong(this IDataRecord dr, int i)
        {
            return new TimeSpan(dr.GetInt64(i));
        } 
        public static string GetStringValue(this IDataRecord dr, int i)
        {
            return dr.GetValue(i).ToString();
        }
        public static object GetValueConvert(this IDataRecord dr, int i, Type TargetType)
        {
            return Convert.ChangeType(dr.GetValue(i), TargetType, CultureInfo.CurrentCulture);
        }
    }

    public class EntityPropertyInfo
    {
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
        public bool IsNullableTypeProperty
        {
            get;
            private set;
        }
        public bool IsNullableProperty
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
            IsNullableProperty = IsNullable(OriginType);
            IsNullableTypeProperty = IsNullableType(OriginType.UnderlyingSystemType); 
            UnderlyingType = IsNullableProperty && IsNullableTypeProperty ? Nullable.GetUnderlyingType(OriginType) : OriginType;
        }
    }

    public class SafeDictionary<TKey, TValue>
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
