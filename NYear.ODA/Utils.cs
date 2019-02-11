using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.Text;

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
        /// <summary>
        /// 分表时用于识别
        /// </summary>
        [DataMember]
        public string ColumnName { get; set; }
        /// <summary>
        /// SQL查询参数
        /// </summary>
        [DataMember(IsRequired = true)]
        public string ParamsName { get; set; }
        /// <summary>
        /// SQL查询参数对应的数据库值类型
        /// </summary>
        [DataMember(IsRequired = true)]
        public ODAdbType DBDataType { get; set; }
        /// <summary>
        /// 参数的方向
        /// </summary>
        [DataMember]
        public ParameterDirection Direction { get; set; }
        /// <summary>
        /// 参数的最大长度
        /// </summary>
        [DataMember]
        public int Size { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        [DataMember]
        public object ParamsValue { get; set; }
    }
    [Serializable()]
    [DataContract]
    public class ODAScript
    {
        [DataMember]
        public string DataBaseId { get; set; } = null;
        [DataMember]
        public SQLType ScriptType { get; set; } = SQLType.Other;
        [DataMember]
        public StringBuilder SqlScript { get; set; } = new StringBuilder();
        [DataMember]
        public List<ODAParameter> ParamList { get; set; } = new List<ODAParameter>(); 
        [DataMember]
        public List<string> TableList { get; set; } = new List<string>(); 
        public StringBuilder OrderBy { get; set; } = new StringBuilder(); 
        public ODAScript Merge(ODAScript Child)
        {
            if (Child.SqlScript.Length > 0)
                SqlScript.Append(Child.SqlScript.ToString()); 
            if (Child.TableList.Count > 0)
                TableList.AddRange(Child.TableList.ToArray());
            if (Child.ParamList.Count > 0)
                ParamList.AddRange(Child.ParamList.ToArray()); 
            return this;
        }
    }
    [Serializable()]
    [DataContract]
    public class ValuesCollection
    {
        [DataMember]
        public string ParamName { get; set; }
        [DataMember]
        public object ReturnValue { get; set; }
    }
    public class SqlJoinScript
    {
        public ODACmd JoinCmd { get; set; }
        public string JoinScript { get; set; }
    }
    public class SqlUnionScript
    {
        public ODACmdView UnionCmd { get; set; }
        public string JoinScript { get; set; }
    }

    public class SqlOrderbyScript
    {
        public IODAColumns OrderbyCol { get; set; }
        public string OrderbyScript { get; set; }
    }
    public class SqlColumnScript
    {
        public IODAColumns SqlColumn { get; set; }
        public string ConnScript { get; set; }
    } 
    public class DatabaseColumnInfo
    {
        public string Name { get; set; }
        public int Length { get; set; }
        public int Scale { get; set; }
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
        BeginTransation = 6,
        Commit = 7,
        Rollback = 8,
        Import = 9,
        Procedure = 10,
    }

    public enum SplitColumnType
    {
        Varchar,
        Number,
        DateTime,
    }

    public delegate IDBAccess GetDBAccessHandler(ODAScript ODASql );

    public delegate void ExecuteSqlEventHandler(object source, ExecuteEventArgs args);

    public class ExecuteEventArgs : EventArgs
    {
        public IDBAccess DBA { get; set; } 
        public ODAScript SqlParams { get; set; }
        public string DebugSQL
        {
            get
            {
                return this.GetDebugSql(SqlParams.SqlScript.ToString(), SqlParams.ParamList.ToArray());
            }
        }
        private string GetDebugSql(string Sql, params ODAParameter[] prms)
        {
            string debugSql = Sql;
            if (prms != null)
            {
                foreach (ODAParameter p in prms)
                {
                    if (p.ParamsValue != null)
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
            }
            return debugSql;
        }
    }


    public class ODynamicModel : DynamicObject, IEnumerable<KeyValuePair<string, object>>
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (storage.ContainsKey(binder.Name))
            {
                result = storage[binder.Name];
                return true;
            }
            result = null;
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string key = binder.Name;
            if (storage.ContainsKey(key))
                storage[key] = value;
            else
                storage.Add(key, value);
            return true;
        }
        public override string ToString()
        {
            StringWriter message = new StringWriter();
            foreach (var item in storage)
                message.WriteLine("\"{0}\":\"{1}\"", item.Key, item.Value);
            return message.ToString();
        }
        public int Count
        {
            get
            {
                return storage.Count;
            }
        }

        public void Add(string key, object value)
        {
            storage.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return storage.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return storage.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return storage.TryGetValue(key, out value);
        }

        public void Clear()
        {
            storage.Clear();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)storage).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)storage).GetEnumerator();
        }
    }

    #region 数据连接设定
    public class ODAConfiguration
    {
        /// <summary>
        /// 数据库模式
        /// </summary>
        public ODAPattern Pattern { get; set; } = ODAPattern.Single;
        /// <summary>
        /// 单一数据库或主从模式的主库
        /// </summary>
        public ODAConnect ODADataBase { get; set; }
        /// <summary>
        /// 业务分库，主从模式的从库
        /// </summary>
        public ODAConnect[] DispersedDataBase { get; set; }

        /// <summary>
        /// 事务控制，事务资源访问顺序
        /// </summary>
        public string[] RegularObject { get; set; }
    }
    public enum ODAPattern
    {
        /// <summary>
        /// 单一数据库
        /// </summary>
        Single, 
        /// <summary>
        /// 主从数据库
        /// </summary>
        MasterSlave,
        /// <summary>
        /// 分库
        /// </summary>
        Dispersed,
    }

    public class ODAConnect
    {
        public string DataBaseId { get; set; }
        public DbAType DBtype { get; set; }
        public string ConnectionString { get; set; }
    }
    #endregion
}
