using NYear.ODA.Adapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;

namespace NYear.ODA
{
    /// <summary>
    /// 数据库访问上下文
    /// </summary>
    public class ODAContext
    {
        public ODAContext()
        {
            if (ODAContext.GolbalDataBaseGroup == null)
                throw new ODAException(30000, "ODAContext.GolbalDataBaseGroup is not setted!");
        }

        /// <summary>
        /// 数据库当前的时间
        /// </summary>
        public DateTime DBDatetime
        {
            get
            {   /////第一次取数据库时间,并保存与本地的时间差异,下一次取本地时间
                if (_DBTimeDiff == null)
                {
                    if (GolbalDataBaseGroup != null)
                    {
                        if (!string.IsNullOrWhiteSpace(GolbalDataBaseGroup.MasterDataBase))
                        {
                            _DBTimeDiff = (NewDBConnect(GolbalDataBaseGroup.DBtype, GolbalDataBaseGroup.MasterDataBase).GetDBDateTime() - DateTime.Now).TotalSeconds;
                        }
                    }
                    else
                    {
                        _DBTimeDiff = 0d;
                    }
                    return DateTime.Now.AddSeconds(_DBTimeDiff.Value);
                }
                else
                    return DateTime.Now.AddSeconds(_DBTimeDiff.Value);
            }
        }

        /// <summary>
        /// 获取当前数据库访问上下文的访问命令实列
        /// </summary>
        /// <typeparam name="U">命令类型</typeparam>
        /// <param name="Alias">别名</param>
        /// <returns></returns>
        public virtual U GetCmd<U>(string Alias = null) where U : ODACmd
        {
            U cmd = Activator.CreateInstance<U>();
            cmd.ParamsMark = ODAContext.DBParamsMark;
            cmd.Counting = Count;
            cmd.Selecting = Select;
            cmd.SelectPaging = Select;
            cmd.SelectRecursion = Select;
            cmd.Updating = Update;
            cmd.Inserting = Insert;
            cmd.InsertScript = Insert;
            cmd.Deleting = Delete;
            cmd.ExecutingProcedure = ExecuteProcedure;
            return cmd;
        }

        #region 数据库连接管理
        private static string _DBParamsMark = null;

        /// <summary>
        /// 当前数据库类型的变量标志
        /// 　待解决：分库时如果不是同一数据库时，DBParamsMark是不同的
        /// </summary>
        public static string DBParamsMark
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_DBParamsMark))
                {
                    IDBAccess DBA = NewDBConnect(GolbalDataBaseGroup.DBtype, GolbalDataBaseGroup.MasterDataBase);
                    _DBParamsMark = DBA.ParamsMark;
                }
                return _DBParamsMark;
            }
        }

        /// <summary>
        /// 公共数据库集群
        /// </summary>
        public static DataBaseGroup GolbalDataBaseGroup
        {
            get;
            set;
        }
        /// <summary>
        /// 数据库集群
        /// </summary>
        public static List<DataBaseGroup> SystemDataBaseGroups
        {
            get;
            set;
        }

        private static List<TableGroup> _SystemTableGroups = new List<TableGroup>();
        /// <summary>
        /// 分表设定,不设定有错误
        /// </summary>
        public static List<TableGroup> SystemTableGroups
        {
            get
            {
                return _SystemTableGroups;
            }
            set
            {
                _SystemTableGroups = value;
            }
        }

        private static double? _DBTimeDiff = null;
        private static IDBAccess NewDBConnect(DbAType dbtype, string Connecting)
        {
            IDBAccess DBA = null;
            switch (dbtype)
            {
                case DbAType.DB2:
                    DBA = new DbADB2(Connecting);
                    break;
                case DbAType.MsSQL:
                    DBA = new DbAMsSQL(Connecting);
                    break;
                case DbAType.MySql:
                    DBA = new DbAMySql(Connecting);
                    break;
                case DbAType.OdbcInformix:
                    DBA = new DbAOdbcInformix(Connecting);
                    break;
                case DbAType.OledbAccess:
                    DBA = new DbAOledbAccess(Connecting);
                    break;
                case DbAType.Oracle:
                    DBA = new DbAOracle(Connecting);
                    break;
                case DbAType.SQLite:
                    DBA = new DbASQLite(Connecting);
                    break;
                case DbAType.Sybase:
                    DBA = new DbASybase(Connecting);
                    break;
            }
            return DBA;
        }

        #endregion

        #region 事务管理
        private ODATransaction _Tran = null;
        /// <summary>
        /// 指示是否已启动了事务
        /// </summary>
        public bool IsTransactionBegined { get { return _Tran != null; } }
        /// <summary>
        /// 事务对象的顺序，用以避免死锁
        /// </summary>
        protected virtual string[] TransactionSequence
        {
            get
            {
                return null;
            }
        }
        protected void CheckTransaction(string DBObjectName)
        {
            return;
        }

        /// <summary>
        /// 使用了事务的数据库连接
        /// </summary>
        protected Dictionary<string, IDBAccess> TransationDataAccess = new Dictionary<string, IDBAccess>();

        /// <summary>
        /// 开启事务，默认30秒超时
        /// </summary>
        public virtual void BeginTransaction()
        {
            BeginTransaction(30);
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="TimeOut">事务超时时长，小于或等于0时事务不会超时,单位:秒</param>
        /// <returns></returns>
        public virtual void BeginTransaction(int TimeOut)
        {
            _Tran = new ODATransaction(TimeOut);
            _Tran.RollBacking += _Tran_RollBacking;///超时自动RollBack
        }

        private void _Tran_RollBacking()
        {
            if (TransationDataAccess != null)
                TransationDataAccess.Clear();
            if (_Tran != null)
                _Tran = null;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (TransationDataAccess != null)
                TransationDataAccess.Clear();
            ODATransaction Tran = _Tran;
            _Tran = null;
            if (Tran != null)
                Tran.Commit();
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            if (TransationDataAccess != null)
                TransationDataAccess.Clear();
            ODATransaction Tran = _Tran;
            _Tran = null;
            if (Tran != null)
                Tran.RollBack();
        }
        #endregion

        #region 分库分表算法

        /// <summary>
        /// 获取命令中涉及的所有数据库对象
        /// </summary>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        private List<string> GetCmdDBObjects(IDBScriptGenerator Cmd)
        {
            List<string> CmdTablesList = new List<string>();
            if (Cmd.BaseCmd != null)
                CmdTablesList.AddRange(GetCmdDBObjects(Cmd.BaseCmd).ToArray());
            else
                CmdTablesList.Add(Cmd.DBObjectMap);

            foreach (IDBScriptGenerator c in Cmd.ListJoinCmd)
            {
                List<string> cTableList = GetCmdDBObjects(c);
                CmdTablesList.AddRange(cTableList.ToArray());
            }
            foreach (SqlJoinScript c in Cmd.JoinCmd)
            {
                List<string> cTableList = GetCmdDBObjects(c.JoinCmd);
                CmdTablesList.AddRange(cTableList.ToArray());
            }
            foreach (ODAColumns c in Cmd.WhereColumns)
            {
                if (c.InCmd != null)
                {
                    List<string> cTableList = GetCmdDBObjects(c.InCmd);
                    CmdTablesList.AddRange(cTableList.ToArray());
                }
            }
            return CmdTablesList;
        }

        /// <summary>
        /// 分库路由算法
        /// </summary>
        /// <param name="SqlType"></param>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        private DataBaseGroup RoutToDatabase(SQLType SqlType, IDBScriptGenerator Cmd)
        {
            if (GolbalDataBaseGroup == null)
                throw new ODAException(30001, "没有找到公共数据库服务器");

            DataBaseGroup RoutedDB = GolbalDataBaseGroup;

            ////没有分库设定，直接返回公共数据库群 GolbalDataBaseGroup
            if (SystemDataBaseGroups == null || SystemDataBaseGroups.Count == 0)
                return RoutedDB;

            ///查看命令中是否所有数据库对象都分布在公共数据库中
            List<string> DBObjects = GetCmdDBObjects(Cmd);
            foreach (string o in DBObjects)
            {
                if (GolbalDataBaseGroup.Tables == null || !GolbalDataBaseGroup.Tables.Contains(o))
                    goto NotGblDB;
            }
            return GolbalDataBaseGroup;

            NotGblDB:
            ///更新公共数据库，但不是所有目标对象都在公共数据库上
            if (SqlType != SQLType.Select && (GolbalDataBaseGroup.Tables == null || GolbalDataBaseGroup.Tables.Contains(Cmd.DBObjectMap)))
            {
                string subObj = String.Join(",", DBObjects.ToArray());
                throw new ODAException(30002, String.Format("更新操作违犯分库设定，对[{0}]执行[{1}]操作其间需要访问数据库对象[{2}]", Cmd.DBObjectMap, Enum.GetName(typeof(SQLType), SqlType), subObj));
            }
           
            foreach (DataBaseGroup dbg in SystemDataBaseGroups)
            {
                foreach (string o in DBObjects)
                {
                    if (dbg.Tables != null && dbg.Tables.Contains(o))
                    {
                       return dbg;
                    }
                }
            }
            return RoutedDB;
        }

        /// <summary>
        /// 分库路由器,如果操作存在事务,返回的DB连接已在事务中
        /// </summary>
        /// <param name="SqlType"></param>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        protected virtual IDBAccess DatabaseRouting(SQLType SqlType, IDBScriptGenerator Cmd)
        {
            IDBAccess DBA = null;
            DataBaseGroup DBGroup = RoutToDatabase(SqlType, Cmd);
            int curDate = int.Parse(System.DateTime.Now.ToString("HHmmssfff"));

            if (_Tran != null)
            {
                if (TransationDataAccess.ContainsKey(DBGroup.GroupID))
                    return TransationDataAccess[DBGroup.GroupID];

                DBA = NewDBConnect(DBGroup.DBtype, DBGroup.MasterDataBase);

                DBA.BeginTransaction();
                _Tran.Committing += DBA.Commit;
                _Tran.RollBacking += DBA.RollBack;
                TransationDataAccess.Add(DBGroup.GroupID, DBA);
            }
            else
            {
                int curDt = 0;
                string DBConn = "";
                if (SqlType == SQLType.Select)
                {
                    if (DBGroup.SlaveDataBase == null || DBGroup.SlaveDataBase.Count == 0)
                    {
                        DBConn = DBGroup.MasterDataBase;
                    }
                    else
                    {
                        curDt = curDate % DBGroup.SlaveDataBase.Count;
                        DBConn = DBGroup.SlaveDataBase[curDt];
                    }
                }
                else
                {
                    DBConn = DBGroup.MasterDataBase;
                }
                DBA = NewDBConnect(DBGroup.DBtype, DBConn);
            }
            Cmd.ParamsMark = DBA.ParamsMark;
            return DBA;
        }

        /// <summary>
        /// 分表路由器
        /// </summary>
        /// <param name="SqlType"></param>
        /// <param name="CmdName"></param>
        /// <param name="ColumnValues"></param>
        /// <returns></returns>
        protected virtual string[] TableRouting(SQLType SqlType, string CmdName, params ODAColumns[] ColumnValues)
        {
            TableGroup tbl = null;
            foreach (TableGroup ctbl in SystemTableGroups)
            {
                if (ctbl.MainObject == CmdName)
                {
                    tbl = ctbl;
                    break;
                }
            }

            ///没有分表设定
            if (tbl == null || tbl.SubTable == null || tbl.SubTable.Count == 0)
                return new string[] { CmdName };

            List<string> SubTableList = new List<string>();

            #region 分表路由算法
            foreach (SplitTable st in tbl.SubTable)
                SubTableList.Add(st.SubTableName);

            foreach (SplitTable sptbl in tbl.SubTable)
            {
                for (int i = 0; i < ColumnValues.Length; i++)
                {
                    foreach (SplitTableColumn spc in sptbl.SplitCondition)
                    {
                        if (ColumnValues[i].ColumnName == spc.ColumnName)
                        {
                            if (!(ColumnValues[i].CompareValue is ODAColumns))
                            {
                                if (SubTableList.Contains(sptbl.SubTableName))
                                    SubTableList.Remove(sptbl.SubTableName);
                            }
                            else
                            {
                                string CompareValue = "";
                                switch (spc.ColumnType)
                                {
                                    case SplitColumnType.Varchar:
                                        CompareValue = "";
                                        if (ColumnValues[i].CompareValue != System.DBNull.Value && ColumnValues[i].CompareValue != null)
                                            CompareValue = ColumnValues[i].CompareValue.ToString();

                                        MD5CryptoServiceProvider MD5CSP = new MD5CryptoServiceProvider();
                                        byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(CompareValue);
                                        byte[] bytHash = MD5CSP.ComputeHash(bytValue);

                                        int intVal = 0;
                                        for (int j = 0; j < bytHash.Length; j++)
                                            intVal += bytHash[j];

                                        intVal = intVal % tbl.SubTable.Count;
                                        int maxInt = tbl.SubTable.Count - 1;
                                        int minInt = 0;
                                        if (!int.TryParse(spc.MaxValue.ToString(), out maxInt)
                                             || !int.TryParse(spc.MinValue.ToString(), out minInt)
                                             || minInt > intVal
                                             || intVal > maxInt
                                            )
                                        {
                                            if (SubTableList.Contains(sptbl.SubTableName))
                                                SubTableList.Remove(sptbl.SubTableName);
                                        }

                                        break;
                                    case SplitColumnType.DateTime:
                                        DateTime valDateTime = DateTime.Now;
                                        if (ColumnValues[i].CompareValue == System.DBNull.Value || ColumnValues[i].CompareValue == null || !DateTime.TryParse(ColumnValues[i].CompareValue.ToString(), out valDateTime))
                                        {
                                            if (SubTableList.Contains(sptbl.SubTableName))
                                                SubTableList.Remove(sptbl.SubTableName);
                                            break;
                                        }
                                        DateTime maxDateTime = DateTime.MaxValue;
                                        DateTime minDateTime = DateTime.MinValue;

                                        if (!DateTime.TryParse(spc.MaxValue.ToString(), out maxDateTime)
                                              || !DateTime.TryParse(spc.MinValue.ToString(), out minDateTime)
                                              || minDateTime > valDateTime
                                              || valDateTime > maxDateTime
                                            )
                                        {
                                            if (SubTableList.Contains(sptbl.SubTableName))
                                                SubTableList.Remove(sptbl.SubTableName);
                                        }
                                        break;
                                    case SplitColumnType.Number:
                                        decimal valDecimal = 0;
                                        if (ColumnValues[i].CompareValue == System.DBNull.Value || ColumnValues[i].CompareValue == null || !decimal.TryParse(ColumnValues[i].CompareValue.ToString(), out valDecimal))
                                        {
                                            if (SubTableList.Contains(sptbl.SubTableName))
                                                SubTableList.Remove(sptbl.SubTableName);
                                            break;
                                        }
                                        decimal maxDecimal = decimal.MaxValue;
                                        decimal minDecimal = decimal.MinValue;
                                        if (!decimal.TryParse(spc.MaxValue.ToString(), out maxDecimal)
                                              || !decimal.TryParse(spc.MinValue.ToString(), out minDecimal)
                                            || minDecimal > valDecimal
                                            || valDecimal > maxDecimal
                                            )
                                        {
                                            if (SubTableList.Contains(sptbl.SubTableName))
                                                SubTableList.Remove(sptbl.SubTableName);
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (SubTableList.Count == 0)///所有表都没有对应的字段的值
            {
                if (SqlType == SQLType.Select)
                    return new string[] { tbl.MainObject };
                else
                    throw new ODAException(30004, "分表路由器找不到需要更改数据的子表");
            }
            else if (SubTableList.Count > 1)
            {
                if (SqlType == SQLType.Select)
                {
                    return new string[] { tbl.MainObject };////如果是分表则，需要建立一个主视图供全表查询
                }
                else if (SqlType == SQLType.Insert)
                {
                    throw new ODAException(30005, "分表路由器定位不到需要插入数据数的子表，请检查插入的数据是否包含分表字段或分表字段的值与分表规则匹配情况");
                }
                else if (SqlType == SQLType.Other)
                {
                    return new string[] { tbl.MainObject };
                }
            }
            return SubTableList.ToArray();
        }
        #endregion

        #region SQL语句执行。（待扩展：使用消息队列实现多数据实时同步）

        /// <summary>
        /// 
        /// </summary>

        public static event ExecuteSqlEventHandler ExecutingSql;

        private void FireExecutingSqlEvent(ExecuteEventArgs args)
        {
            if (ExecutingSql != null)
                ExecutingSql(this, args);
        }

        protected virtual void SetSelectSplitTable(IDBScriptGenerator Cmd)
        {
            if (Cmd.BaseCmd != null)
            {
                SetSelectSplitTable(Cmd.BaseCmd);
            }
            else
            {
                string[] mTbl = TableRouting(SQLType.Select, Cmd.CmdName, Cmd.WhereColumns.ToArray());
                Cmd.DBObjectMap = mTbl[0];
            }
            foreach (IDBScriptGenerator c in Cmd.ListJoinCmd)
                SetSelectSplitTable(c);
            foreach (SqlJoinScript c in Cmd.JoinCmd)
                SetSelectSplitTable(c.JoinCmd);
            foreach (ODAColumns c in Cmd.WhereColumns)
                if (c.InCmd != null)
                    SetSelectSplitTable(c.InCmd);
        }

        /// <summary>
        /// 执行 select count 查询并返回值
        /// </summary>
        /// <param name="Cmd"></param>
        /// <param name="Col"></param>
        /// <returns></returns>
        protected virtual int Count(IDBScriptGenerator Cmd, ODAColumns Col)
        {
            SetSelectSplitTable(Cmd);
            IDBAccess DBA = DatabaseRouting(SQLType.Select, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
            string sql;
            EA.SqlParams = Cmd.GetCountSql(out sql, Col);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            DataTable d = EA.DBA.Select(EA.SQL, EA.SqlParams);
            return int.Parse(d.Rows[0]["TOTAL_RECORD"].ToString());
        }
        
        /// <summary>
        /// 执行查询
        /// </summary>
        /// <param name="Cmd">查询命令</param>
        /// <param name="Cols">查询的字段</param>
        /// <returns></returns>
        protected virtual DataTable Select(IDBScriptGenerator Cmd, params ODAColumns[] Cols)
        {
            try
            {
                SetSelectSplitTable(Cmd);
                IDBAccess DBA = DatabaseRouting(SQLType.Select, Cmd);
                ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
                string sql;
                EA.SqlParams = Cmd.GetSelectSql(out sql, Cols);
                EA.SQL = sql;
                this.FireExecutingSqlEvent(EA);
                DataTable d = EA.DBA.Select(EA.SQL, EA.SqlParams);
                return d;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 查询分页
        /// </summary>
        /// <param name="Cmd">查询脚本</param>
        /// <param name="StartIndex">起始行数从0开始</param>
        /// <param name="MaxRecord">读取行数</param>
        /// <param name="Cols">要求返回的列</param>
        /// <returns></returns>
        protected virtual DataTable Select(IDBScriptGenerator Cmd, int StartIndex, int MaxRecord, params ODAColumns[] Cols)
        {
            SetSelectSplitTable(Cmd);
            IDBAccess DBA = DatabaseRouting(SQLType.Select, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
            string sql;
            EA.SqlParams = Cmd.GetSelectSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            DataTable d = EA.DBA.Select(EA.SQL, EA.SqlParams, StartIndex, MaxRecord);
            return d;
        }
        protected virtual DataTable Select(IDBScriptGenerator Cmd, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, int MaxLevel, params ODAColumns[] Cols)
        {
            SetSelectSplitTable(Cmd);
            IDBAccess DBA = DatabaseRouting(SQLType.Select, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
            string sql;
            EA.SqlParams = Cmd.GetSelectSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            DataTable d = EA.DBA.Select(EA.SQL, EA.SqlParams, StartWithExpress, ConnectBy, Prior, ConnectColumn, ConnectChar, MaxLevel);
            return d;
        }
        protected virtual bool Insert(IDBScriptGenerator Cmd, params ODAColumns[] Cols)
        {
            string[] mTbl = TableRouting(SQLType.Insert, Cmd.CmdName, Cmd.WhereColumns.ToArray());
            if (mTbl.Length != 1)
                throw new ODAException(30006, "插入数据分表设定错误");
            Cmd.DBObjectMap = mTbl[0];
            Cmd.Alias = "";
            IDBAccess DBA = DatabaseRouting(SQLType.Insert, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
            string sql;
            EA.SqlParams = Cmd.GetInsertSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            return DBA.ExecuteSQL(EA.SQL, EA.SqlParams) > 0;
        }
        protected virtual bool Insert(IDBScriptGenerator InsertCmd, IDBScriptGenerator SelectCmdparams, ODAColumns[] Cols)
        {
            IDBAccess DBA = DatabaseRouting(SQLType.Insert, InsertCmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
            string slct = "";
            EA.SqlParams = SelectCmdparams.GetSelectSql(out slct, Cols);
            string Column = "";
            for (int i = 0; i < Cols.Length; i++)
                Column += Cols[i].ColumnName + ",";
            EA.SQL = "INSERT INTO " + InsertCmd.CmdName + "(" + Column.Remove(Column.Length - 1, 1) + ") " + slct;
            this.FireExecutingSqlEvent(EA);
            return DBA.ExecuteSQL(EA.SQL, EA.SqlParams) > 0;
        }
        protected virtual bool Update(IDBScriptGenerator Cmd, params ODAColumns[] Cols)
        {
            string[] mTbl = TableRouting(SQLType.Update, Cmd.CmdName, Cmd.WhereColumns.ToArray());
            foreach (ODAColumns c in Cmd.WhereColumns)
                if (c.InCmd != null)
                    SetSelectSplitTable(c.InCmd);

            ////如果有分表,而且命令涉及到两个表以上,及单表操作,则启动事务
            bool LocalTranstion = false;
            if (_Tran == null && mTbl.Length > 1)
            {
                this.BeginTransaction(30);
                LocalTranstion = true;
            }
            foreach (string subTable in mTbl)
            {
                Cmd.DBObjectMap = subTable;
                Cmd.Alias = "";
                IDBAccess DBA = DatabaseRouting(SQLType.Update, Cmd);
                ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
                string sql;
                EA.SqlParams = Cmd.GetUpdateSql(out sql, Cols);
                EA.SQL = sql;
                this.FireExecutingSqlEvent(EA);
                EA.DBA.ExecuteSQL(EA.SQL, EA.SqlParams);
            }
            if (LocalTranstion)
                _Tran.Commit();
            return true;
        }
        protected virtual bool Delete(IDBScriptGenerator Cmd)
        {
            string[] mTbl = TableRouting(SQLType.Insert, Cmd.CmdName, Cmd.WhereColumns.ToArray());
            foreach (ODAColumns c in Cmd.WhereColumns)
                if (c.InCmd != null)
                    SetSelectSplitTable(c.InCmd);

            ////如果有分表,而且命令涉及到两个表以上,及单表操作,则启动事务
            bool LocalTranstion = false;
            if (_Tran == null && mTbl.Length > 1)
            {
                this.BeginTransaction(30);
                LocalTranstion = true;
            }
            foreach (string subTable in mTbl)
            {
                Cmd.DBObjectMap = subTable;
                Cmd.Alias = "";
                IDBAccess DBA = DatabaseRouting(SQLType.Delete, Cmd);
                ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
                string sql;
                EA.SqlParams = Cmd.GetDeleteSql(out sql);
                EA.SQL = sql;
                this.FireExecutingSqlEvent(EA);
                EA.DBA.ExecuteSQL(EA.SQL, EA.SqlParams);
            }
            if (LocalTranstion)
                _Tran.Commit();
            return true;
        }
        protected virtual DataSet ExecuteProcedure(IDBScriptGenerator Cmd, params ODAColumns[] Cols)
        {
            string[] mTbl = TableRouting(SQLType.Other, Cmd.CmdName, Cols);
            if (mTbl.Length != 1)
                throw new ODAException(30007, "存储过程分表设定错误");
            Cmd.DBObjectMap = mTbl[0];

            Cmd.Alias = "";
            IDBAccess DBA = DatabaseRouting(SQLType.Other, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA };
            string Sql = "";
            EA.SqlParams = Cmd.GetProcedureSql(out Sql, Cols);
            EA.SQL = Sql;
            this.FireExecutingSqlEvent(EA);
            return EA.DBA.ExecuteProcedure(EA.SQL, EA.SqlParams);
        }
        #endregion
    }
}