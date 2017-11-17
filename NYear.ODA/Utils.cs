using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;


namespace NYear.ODA
{
    public class ODAException : Exception
    {
        public int ExceptionCode { get; set; }
        public ODAException(string Msg)
        {
            ExceptionCode = 1000001;
        }

        public ODAException(int Excode, string Msg)
            : base(string.Format("ODA-{0} {1}" , Excode , Msg))
        {
            ExceptionCode = Excode;
        }
    }

    public enum DBATranport
    {
        Http,
        Tcp,
        Ipc,
        Msmq,
    }
    [Serializable()]
    public enum DbAType
    {
        MsSQL,
        MySql,
        OdbcInformix,
        OledbAccess,
        Oracle,
        Sybase,
        DB2,
        SQLite,
    }
    [Serializable()]
    public enum CmdFuntion
    {
        NONE,
        MAX,
        MIN,
        COUNT,
        SUM,
        AVG,
        LENGTH,
        LTRIM,
        RTRIM,
        TRIM,
        ASCII,
        UPPER,
        LOWER
    }

    [Serializable()]
    public enum CmdConditionSymbol
    {
        NONE,
        EQUAL,
        NOTEQUAL,
        ISNULL,
        ISNOTNULL,
        BIGGER,
        NOTBIGGER,
        SMALLER,
        NOTSMALLER,
        IN,
        NOTIN,
        LIKE,
        ADD,
        REDUCE,
        TAKE,
        REMOVE,
        STAY,
    }

    //[Serializable()]
    //public enum DataBaseType
    //{
    //    DbAMsSQL,
    //    DbAMySql,
    //    DbAOdbcInformix,
    //    DbAOledbAccess,
    //    DbAOracle,
    //    DbASybase,
    //    DbADB2,
    //    DbASQLite,
    //    DbAReflection,
    //}

    [Serializable()]
    public enum ODAdbType
    {
        //Other = 0,
        OBinary = 1,
        OCursor = 2,
        ODatetime = 3,
        ODecimal = 4,
        OInt = 5,
        OChar = 6,
        OVarchar = 7,
        /// <summary>
        /// 主要针对存储过程数组参数
        /// </summary>
        OArrary = 8,
    }

    [Serializable()]
    [DataContract]
    public class ODAParameter
    {
        public static char ODAParamsMark { get { return '@'; } }

        [DataMember(IsRequired = true)]
        public string ParamsName { get; set; }
        [DataMember(IsRequired = true)]
        public ODAdbType DBDataType { get; set; }
        [DataMember]
        public ParameterDirection Direction { get; set; }
        [DataMember]
        public int Size { get; set; }
        [DataMember]
        public object ParamsValue { get; set; }
    }

    [Serializable()]
    [DataContract]
    public class ValuesCollection
    {
        [DataMember]
        public object ParamName { get; set; }
        [DataMember]
        public object ReturnValue { get; set; }
    }
    public class SqlJoinScript
    {
        public ODACmd JoinCmd { get; set; }
        public string JoinScript { get; set; }
    }

    public class SqlOrderbyScript
    {
        public ODAColumns OrderbyCol { get; set; }
        public string OrderbyScript { get; set; }
    }
    public class SqlColumnScript
    {
        public ODAColumns SqlColumn { get; set; }
        public string ConnScript { get; set; }
    }
    public class DatabaseColumnInfo
    {
        public string Name { get; set; }
        public decimal Length { get; set; }
        public string ColumnType { get; set; }
        public bool NoLength { get; set; }
        public bool NotNull { get; set; }
    }
    public enum SQLType
    {
        Other = 1,
        Insert = 2,
        Delete = 3,
        Select = 4,
        Update = 5,
    }

    /// <summary>
    /// 数据库集群（主－从）
    /// </summary>
    public class DataBaseGroup
    {
        public string GroupID { get; set; }
        public DbAType DBtype { get; set; }
        public string MasterDataBase { get; set; }
        public List<string> SlaveDataBase { get; set; }
        public List<string> Tables { get; set; }
    }

    /// <summary>
    /// 分表设置项
    /// </summary>
    public class TableGroup
    {
        /// <summary>
        /// 默认表
        /// </summary>
        public string MainObject { get; set; }
        /// <summary>
        /// 子表设置
        /// </summary>
        public List<SplitTable> SubTable { get; set; }
    }
    /// <summary>
    /// 子表
    /// </summary>
    public class SplitTable
    {
        public string SubTableName { get; set; }
        public List<SplitTableColumn> SplitCondition { get; set; }
    }

    /// <summary>
    /// 分表条件设定
    /// </summary>
    public class SplitTableColumn
    {
        public string ColumnName { get; set; }
        public SplitColumnType ColumnType { get; set; }
        public object MaxValue { get; set; }
        public object MinValue { get; set; }
    }

    public enum SplitColumnType
    {
        Varchar,
        Number,
        DateTime,
    }

    public delegate int CountEventHandler(ODACmd Cmd, ODAColumns Col); 
    public delegate DataTable SelectEventHandler(ODACmd Cmd, params ODAColumns[] Cols);
    public delegate DataTable SelectPagingEventHandler(ODACmd Cmd, int StartIndex, int MaxRecord, params ODAColumns[] Cols);
    public delegate object[] SelectFirstEventHandler(ODACmd Cmd, params ODAColumns[] Cols);
    public delegate DataTable SelectRecursionEventHandler(ODACmd Cmd, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, int MaxLevel, params ODAColumns[] Cols);
    public delegate bool UpdateEventHandler(ODACmd Cmd, params ODAColumns[] Cols);
    public delegate bool InsertEventHandler(ODACmd Cmd, params ODAColumns[] Cols);
    public delegate bool InsertScriptEventHandler(ODACmd InsertCmd, ODACmd SelectCmd, ODAColumns[] Cols);
    public delegate bool ImportEventHandler(ODACmd Cmd, ODAParameter[] Prms, DataTable Data);
    public delegate bool DeleteEventHandler(ODACmd Cmd);
    public delegate DataSet ExecuteProcedureEventHandler(ODACmd Cmd, params ODAColumns[] Cols);
    public delegate void ExecuteSqlEventHandler(object source, ExecuteEventArgs args);

    public class ExecuteEventArgs : EventArgs
    {
        public ODA.IDBAccess DBA { get; set; }
        public string SQL { get; set; }

        public ODAParameter[] SqlParams { get; set; }

        public string DebugSQL
        {
            get
            {
                if (this.DBA == null || string.IsNullOrWhiteSpace(this.SQL))
                    return string.Empty;
                return this.GetDebugSql(this.SQL, this.SqlParams);
            }
        }
        private string GetDebugSql(string Sql, params ODAParameter[] prms)
        {
            string debugSql = Sql;
            if (prms != null)
            {
                foreach (ODAParameter p in prms)
                {
                    string ParamsValue = p.ParamsValue.ToString();
                    string ParamsName = p.ParamsName;
                    switch (p.DBDataType)
                    {
                        case ODAdbType.OInt:
                        case ODAdbType.ODecimal:
                            ParamsValue = p.ParamsValue.ToString();
                            break;
                        case ODAdbType.OChar:
                        case ODAdbType.OVarchar:
                            ParamsValue = "'" + p.ParamsValue.ToString() + "'";
                            break;
                        case ODAdbType.ODatetime:
                            if (p.ParamsValue is DateTime)
                                ParamsValue = "'" + ((DateTime)p.ParamsValue).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            break;
                    }
                    debugSql = debugSql.Replace(ParamsName, ParamsValue);
                }
            }
            return debugSql;
        }
    }


    //public static class DataConvert
    //{
    //    public static List<T> ConvertToList<T>(DataTable dt) where T:new()
    //    {
    //        List<T> list = new List<T>();
    //        if (typeof(T).IsValueType || typeof(T) == typeof(string))
    //        {
    //            for (int i = 0; i < dt.Rows.Count; i++)
    //            {
    //                if (dt.Rows[i][0] is T)
    //                    list.Add((T)dt.Rows[i][0]);
    //                else
    //                    list.Add((T)System.Convert.ChangeType(dt.Rows[i][0], typeof(T), CultureInfo.InvariantCulture));
    //            }
    //        }
    //        else
    //        {
    //            T t = new T();
    //            PropertyInfo[] propertypes = t.GetType().GetProperties();
    //            for (int i = 0; i < dt.Rows.Count; i++)
    //            {
    //                foreach (PropertyInfo pro in propertypes)
    //                {
    //                    if (pro.CanWrite && dt.Columns.Contains(pro.Name))
    //                    {
    //                        object value = dt.Rows[i][pro.Name];
    //                        pro.SetValue(t, DataConvert(value, pro.PropertyType), null);
    //                    }
    //                }
    //                list.Add(t);
    //            }
    //        }
    //        return list;
    //    }

    //    public static T ConvertToModel<T>(DataTable dt)
    //    {
    //        T t = Activator.CreateInstance<T>();
    //        PropertyInfo[] propertypes = t.GetType().GetProperties();
    //        if (dt.Rows.Count > 0)
    //        {
    //            foreach (PropertyInfo pro in propertypes)
    //            {
    //                if (pro.CanWrite && dt.Columns.Contains(pro.Name))
    //                {
    //                    object value = dt.Rows[0][pro.Name];
    //                    pro.SetValue(t, DataConvert(value, pro.PropertyType), null);
    //                }
    //            }
    //            return t;
    //        }
    //        return default(T);
    //    }

    //    public static T ConvertToModeT<T>(object A) where T : class
    //    {
    //        T t = default(T);
    //        t = Activator.CreateInstance<T>();
    //        PropertyInfo[] PList = t.GetType().GetProperties();
    //        Type AT = A.GetType();
    //        foreach (PropertyInfo P in PList)
    //        {
    //            try
    //            {
    //                object obj = AT.GetProperty(P.Name).GetValue(A, null);
    //                P.SetValue(t, DataConvert(obj, P.PropertyType), null);
    //            }
    //            catch { }
    //        }
    //        return t;
    //    }

    //    public static object DataConvert(object val, Type targetType)
    //    {
    //        if (val == null || val == System.DBNull.Value)
    //        {
    //            if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
    //                return null;
    //            return Activator.CreateInstance(targetType);
    //        }
    //        if (targetType.IsInstanceOfType(val))
    //        {
    //            return val;
    //        }
    //        else
    //        {
    //            try
    //            {
    //                Type baseTargetType = targetType;
    //                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
    //                    baseTargetType = Nullable.GetUnderlyingType(targetType);
    //                return System.Convert.ChangeType(val, baseTargetType, CultureInfo.InvariantCulture);
    //            }
    //            catch
    //            {
    //                if (targetType.IsValueType)
    //                    return Activator.CreateInstance(targetType);
    //                return null;
    //            }
    //        }
    //    }
         
    //    public static T ChangeType<T>(int idx, object[] val) where T : IConvertible
    //    {
    //        if (val == null || val.Length <= idx || val[idx] == null || Convert.IsDBNull(val[idx]))
    //            return default(T);
    //        if (val[idx] is T)
    //            return (T)val[idx];
    //        try
    //        {
    //            return (T)Convert.ChangeType(val[idx], typeof(T));
    //        }
    //        catch
    //        {
    //            return default(T);
    //        }
    //    }
    //}
}
