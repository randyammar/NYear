using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
        [DataMember]
        public string ColumnName { get; set; }
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
    public class ODAScript
    {
        [DataMember]
        public string DataBaseId { get; set; } = null;
        [DataMember]
        public SQLType ScriptType { get; set; } = SQLType.Other;
        [DataMember]
        public StringBuilder SqlScript { get; set; } = new StringBuilder();
        [DataMember]
        public List<ODAParameter> WhereList { get; set; } = new List<ODAParameter>();
        [DataMember]
        public List<ODAParameter> ValueList { get; set; } = new List<ODAParameter>();
        [DataMember]
        public List<string> TableList { get; set; } = new List<string>();
        [DataMember]
        public string OrderBy  { get; set; }  

        public ODAScript Merge(ODAScript Child)
        {
            if (Child.SqlScript.Length > 0)
                SqlScript.Append(Child.SqlScript.ToString()); 
            if (Child.TableList.Count > 0)
                TableList.AddRange(Child.TableList.ToArray());
            if (Child.ValueList.Count > 0)
                ValueList.AddRange(Child.ValueList.ToArray());
            if (Child.WhereList.Count > 0)
                WhereList.AddRange(Child.WhereList.ToArray()); 
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
                var prms = new ODAParameter[0];
                if (SqlParams != null)
                {
                    prms = new ODAParameter[SqlParams.ValueList.Count + SqlParams.WhereList.Count];
                    SqlParams.ValueList.CopyTo(prms, 0);
                    SqlParams.WhereList.CopyTo(prms, SqlParams.ValueList.Count);
                } 
                return this.GetDebugSql(SqlParams.SqlScript.ToString(), prms);
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
        /// 有序的数据库对象
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
