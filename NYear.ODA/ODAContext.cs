/*======================================================================================================
 * 分库：
 * 分库是为了减少单个数据库压力而分开多个数据库存储和处理多个系统或模块数据的方法，是业务逻辑的分割。
 * 分库的技术限制：
 * 1.不能跨库强关连。
 *    分库是为了提高性能，但跨库强关连性能十分低下（例:oracle的dblink)
 *    技术实现难度太大，只能从各个库中作数查询足够完整的数据然后在内存中连接筛选，耗费资源太多，得不偿失。
 * 2.分布式事务很难做，应用服务器（ODA）做不到，所以在应用服务上不能有跨库事务，跨库事务一般都是数据库层面考虑。
 *    目前的分布式事务解决方案是数据库的二阶段提交（PreCommit、doCommit）或三阶段提交（CanCommit、PreCommit、doCommit）。
 *    除此之外还可以业务结合技术考虑,如：
 *    同一个事务内的表各自增加一个事务表，把事务数据（原数据及新数据）暂存到事务表，以备回滚和提交（CanCommit、PreCommit），
 *    提交或回滚时(DoCommit)再从事务表更新数据到业务表。
 *
 * 数据库集群（读写分离或者说是主从数据库）：
 * 提高系统响应速度的纯技术方案，以空间换速度。对业务透明，付带效果是容灾备份但增加维护复杂度及成本。
 *   从数据库的数量不是越多越好，也是有约束的，约束如下（只是理论值，不考虑网络且数据库硬用性能都一样) 
 *   如测定一个数据库一秒内的最大吞吐量：reads + 2×writes = 1200,90%读10%写,writes = 2* reads(写耗时是读的两倍)
 *   则从服务器最大数量：reads/9 = writes / (N + 1) （N 从服务器最大数量)
 *   
 * 分表：
 * 分表是为了提高单个功能的响应速度，而进行的数据存储结构优化方案。
 *  1.水平分表(按数据内容分表)，与分区表原理相同；
 *     ODA分表方式：一个主视图（一般为物化视图)以及多个子物理表。
 *  
 *  2.垂直分表（按字段分表)，一般不作此类分表，因为这样分表对常规的查询有反作用（增加复杂性、降低查询速度)；
 *     但有一些大字段如 Blob、Clob字段或冷热不均的数据（标题、作者、分类、文章内容与浏览量、回复数等统计信息)
 *     为了提高表的处理速度和减少对大字段操作或为了做缓存而垂直分割。
 ========================================================================================================*/

using NYear.ODA.Adapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Timers;

namespace NYear.ODA
{
    /// <summary>
    /// 数据库访问上下文
    /// </summary>
    public class ODAContext
    {
        #region 数据库连接管理
        private static List<DataBaseSetting> _DataBaseSetting;
        /// <summary>
        /// 数据库连接设定
        /// </summary>
        public static List<DataBaseSetting> DataBaseSetting
        {
            get { return _DataBaseSetting; }
            set { _DataBaseSetting = value; }
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

        //static ODAContext()
        //{
        //    Timer tmr = new Timer();
        //    tmr.Interval = 1000;
        //    tmr.Elapsed += Tmr_Elapsed;
        //}
        ///// <summary>
        ///// 从数据库可用性维护
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private static void Tmr_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    ((Timer)sender).Stop();
        //    if (_DataBaseSetting != null && _DataBaseSetting.Count > 0)
        //    {
        //        Dictionary<string[], DbAType> SlaveDb = new Dictionary<string[], DbAType>();
        //        lock (_DataBaseSetting)
        //        {
        //            foreach (var db in _DataBaseSetting)
        //            {
        //                if (db.SlaveConnectionStrings != null && db.SlaveConnectionStrings.Count > 0)
        //                {
        //                    SlaveDb.Add(db.SlaveConnectionStrings.ToArray(), db.DBtype);
        //                }
        //            }
        //        }

        //        if (SlaveDb.Count > 0)
        //        {
        //            ///
        //        }
        //    }
        //    ((Timer)sender).Start();
        //}
        #endregion

        private DbAType dbType = DbAType.MsSQL;
        private string dbConn = null;
        public ODAContext()
        {
        }
        public ODAContext(DbAType DbType, string ConectionString)
        {
            dbType = DbType;
            dbConn = ConectionString;
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
                    if (string.IsNullOrWhiteSpace(dbConn))
                    {
                        if (ODAContext.DataBaseSetting != null || ODAContext.DataBaseSetting.Count > 0)
                        {
                            if (!string.IsNullOrWhiteSpace(ODAContext.DataBaseSetting[0].ConnectionString))
                            {
                                _DBTimeDiff = (ODAContext.NewDBConnect(ODAContext.DataBaseSetting[0].DBtype, ODAContext.DataBaseSetting[0].ConnectionString).GetDBDateTime() - DateTime.Now).TotalSeconds;
                            }
                        }
                    }
                    else
                    {
                        _DBTimeDiff = (ODAContext.NewDBConnect(this.dbType,this.dbConn).GetDBDateTime() - DateTime.Now).TotalSeconds;
                    }
                    if(_DBTimeDiff== null)
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
        public virtual U GetCmd<U>(string Alias = "") where U : ODACmd,new()
        {
            U cmd = new U();
            cmd.Alias = Alias;
            cmd.Counting = Count;
            cmd.Selecting = Select;
            cmd.SelectPaging = Select;
            cmd.SelectRecursion = Select;
            cmd.SelectingFirst = SelectFirst;
            cmd.Updating = Update;
            cmd.Inserting = Insert;
            cmd.InsertScript = Insert;
            cmd.Deleting = Delete;
            cmd.ExecutingProcedure = ExecuteProcedure;
            cmd.Importing = Import;
            return cmd;
        }

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
        /// <summary>
        /// 检查事务对象的顺序
        /// </summary>
        /// <param name="Cmd"></param>
        protected void CheckTransaction(IDBScriptGenerator Cmd)
        {
            //this._Tran.TransactionId
            //return;
        }

        /// <summary>
        /// 启动了事务的数据库
        /// </summary>
        private Dictionary<string, IDBAccess> _TransDataBase = new Dictionary<string, IDBAccess>();
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
            _Tran.TransactionTimeOut = this.RollBack;
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            try
            {
                if (_Tran != null)
                {
                    _Tran.Commit();
                    FireExecutingSqlEvent(new ExecuteEventArgs()
                    {
                        Operation = SQLType.Other,
                        SQL = "Commit"
                    });

                }
            }
            finally
            {
                if (_TransDataBase != null)
                    _TransDataBase.Clear();
            }
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            try
            {
                if (_Tran != null)
                {
                    _Tran.RollBack();
                    FireExecutingSqlEvent(new ExecuteEventArgs()
                    {
                        Operation = SQLType.Other,
                        SQL = "RollBack"
                    });
                }
            }
            finally
            {
                if (_TransDataBase != null)
                    _TransDataBase.Clear();
            }      
        }
        #endregion

        #region 分库分表算法
        /// <summary>
        /// 分库路由算法
        /// </summary>
        /// <param name="SqlType"></param>
        /// <param name="Cmd"></param>
        /// <returns></returns>
        private DataBaseSetting RoutToDatabase(SQLType SqlType, IDBScriptGenerator Cmd)
        {
            if (ODAContext.DataBaseSetting.Count == 1)
            {
                return ODAContext.DataBaseSetting[0];
            }
            else
            {
                foreach (var db in ODAContext.DataBaseSetting)
                {
                    if ((string.IsNullOrWhiteSpace(db.SystemID) && string.IsNullOrWhiteSpace(Cmd.SystemID))
                        || db.SystemID == Cmd.SystemID
                        )
                        return db;
                }
            }
            throw new ODAException(30001, "没有找到适用的数据库");
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
            if (ODAContext.SystemTableGroups == null && string.IsNullOrWhiteSpace(this.dbConn))
                throw new ODAException(30000, "没有找到可用的数据库连接设定");

            if (!string.IsNullOrWhiteSpace(this.dbConn))
            {
                if (_Tran != null)
                {
                    if(_Tran.IsTimeout)
                        throw new ODAException(30000, "事务已超时");
                    this.CheckTransaction(Cmd);
                    if (_TransDataBase.ContainsKey(this.dbConn))
                    {
                        return _TransDataBase[this.dbConn];
                    }
                    else
                    {
                        DBA = ODAContext.NewDBConnect(this.dbType, this.dbConn);
                        DBA.BeginTransaction();
                        _Tran.DoCommit += DBA.Commit;
                        _Tran.RollBacking += DBA.RollBack;
                        _TransDataBase.Add(this.dbConn, DBA);
                    }
                }
                else
                {
                    DBA = ODAContext.NewDBConnect(this.dbType, this.dbConn);
                }
            }

            DataBaseSetting DB = RoutToDatabase(SqlType, Cmd);
            if (_Tran != null)
            {
                if (_Tran.IsTimeout)
                    throw new ODAException(30000, "事务已超时");

                this.CheckTransaction(Cmd);
                if (_TransDataBase.ContainsKey(DB.ConnectionString))
                {
                    return _TransDataBase[DB.ConnectionString];
                }
                else if (_TransDataBase.Count > 0)
                {
                    throw new ODAException(30008,"不支持跨数据库事务（分布式事务)");
                }

                DBA = ODAContext.NewDBConnect(DB.DBtype, DB.ConnectionString);
                DBA.BeginTransaction();
                _Tran.DoCommit += DBA.Commit;
                _Tran.RollBacking += DBA.RollBack; 
                _TransDataBase.Add(DB.ConnectionString,DBA);
            }
            else
            {
                int curDt = 0;
                if (SqlType == SQLType.Select && DB.SlaveConnectionStrings != null && DB.SlaveConnectionStrings.Capacity != 0)
                {
                    int curDate = int.Parse(System.DateTime.Now.ToString("ssfffff"));
                    curDt = curDate % DB.SlaveConnectionStrings.Count;
                    DBA = ODAContext.NewDBConnect(DB.DBtype, DB.SlaveConnectionStrings[curDt]);
                }
                else
                {
                    DB = RoutToDatabase(SqlType, Cmd); 
                    DBA = ODAContext.NewDBConnect(DB.DBtype, DB.ConnectionString);
                }
            }
            return DBA;
        }

        /// <summary>
        /// 分表路由器
        /// </summary>
        /// <param name="SqlType"></param>
        /// <param name="CmdName"></param>
        /// <param name="ColumnValues"></param>
        /// <returns></returns>
        protected virtual string[] TableRouting(SQLType SqlType, IDBScriptGenerator Cmd, params ODAColumns[] ColumnValues)
        {
            TableGroup tbl = null;
            foreach (TableGroup ctbl in SystemTableGroups)
            {
                if (ctbl.MainObject == Cmd.CmdName)
                {
                    if (ctbl.SystemID == Cmd.SystemID || (string.IsNullOrWhiteSpace(ctbl.SystemID) && string.IsNullOrWhiteSpace(Cmd.SystemID)))
                    {
                        tbl = ctbl;
                        break;
                    }
                }
            }
            ///没有分表设定
            if (tbl == null || tbl.SubTable == null || tbl.SubTable.Count == 0)
                return new string[] { Cmd.CmdName };

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
        public static event ExecuteSqlEventHandler ExecutingSql;
        public event ExecuteSqlEventHandler CurrentExecutingSql;
        private void FireExecutingSqlEvent(ExecuteEventArgs args)
        {
            ExecutingSql?.Invoke(this, args);
            CurrentExecutingSql?.Invoke(this, args);
        }

        /// <summary>
        /// 分表查询设定
        /// </summary>
        /// <param name="Cmd"></param>
        protected virtual void SetSelectSplitTable(IDBScriptGenerator Cmd)
        {
            if (Cmd.BaseCmd != null)
            {
                SetSelectSplitTable(Cmd.BaseCmd);
            }
            else
            {
                string[] mTbl = TableRouting(SQLType.Select, Cmd, Cmd.WhereColumns.ToArray());
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
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Select };
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
            SetSelectSplitTable(Cmd);
            IDBAccess DBA = DatabaseRouting(SQLType.Select, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Select };
            string sql;
            EA.SqlParams = Cmd.GetSelectSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            DataTable d = EA.DBA.Select(EA.SQL, EA.SqlParams);
            return d;
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
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Select };
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
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Select };
            string sql;
            EA.SqlParams = Cmd.GetSelectSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            DataTable d = EA.DBA.Select(EA.SQL, EA.SqlParams, StartWithExpress, ConnectBy, Prior, ConnectColumn, ConnectChar, MaxLevel);
            return d;
        }

        protected virtual object[] SelectFirst(IDBScriptGenerator Cmd, params ODAColumns[] Cols)
        {
            SetSelectSplitTable(Cmd);
            IDBAccess DBA = DatabaseRouting(SQLType.Select, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Select };
           
            string sql;
            EA.SqlParams = Cmd.GetSelectSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            object[] o = EA.DBA.SelectFirst(EA.SQL, EA.SqlParams);
            return o;
        }
        protected virtual bool Insert(IDBScriptGenerator Cmd, params ODAColumns[] Cols)
        {
            string[] mTbl = TableRouting(SQLType.Insert, Cmd, Cmd.WhereColumns.ToArray());
            if (mTbl.Length != 1)
                throw new ODAException(30006, "插入数据分表设定错误");
            Cmd.DBObjectMap = mTbl[0];
            Cmd.Alias = "";
            IDBAccess DBA = DatabaseRouting(SQLType.Insert, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Insert };
            string sql;
            EA.SqlParams = Cmd.GetInsertSql(out sql, Cols);
            EA.SQL = sql;
            this.FireExecutingSqlEvent(EA);
            return DBA.ExecuteSQL(EA.SQL, EA.SqlParams) > 0;
        }
        protected virtual bool Insert(IDBScriptGenerator InsertCmd, IDBScriptGenerator SelectCmdparams, ODAColumns[] Cols)
        {
            IDBAccess DBA = DatabaseRouting(SQLType.Insert, InsertCmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Insert };
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
            string[] mTbl = TableRouting(SQLType.Update, Cmd, Cmd.WhereColumns.ToArray());
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
                ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Update };
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
            string[] mTbl = TableRouting(SQLType.Insert, Cmd, Cmd.WhereColumns.ToArray());
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
                ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Delete };
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
            string[] mTbl = TableRouting(SQLType.Other, Cmd, Cols);
            if (mTbl.Length != 1)
                throw new ODAException(30007, "存储过程分表设定错误");
            Cmd.DBObjectMap = mTbl[0];

            Cmd.Alias = "";
            IDBAccess DBA = DatabaseRouting(SQLType.Other, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Other };
            string Sql = "";
            EA.SqlParams = Cmd.GetProcedureSql(out Sql, Cols);
            EA.SQL = Sql;
            this.FireExecutingSqlEvent(EA);
            return EA.DBA.ExecuteProcedure(EA.SQL, EA.SqlParams);
        }
        protected virtual bool Import(IDBScriptGenerator Cmd, ODAParameter[] Prms, DataTable Data)
        {
            string[] mTbl = TableRouting(SQLType.Other, Cmd);
            if (mTbl.Length != 1)
                throw new ODAException(30008, "存储过程分表设定错误");
            Cmd.DBObjectMap = mTbl[0];

            Cmd.Alias = "";
            IDBAccess DBA = DatabaseRouting(SQLType.Other, Cmd);
            ExecuteEventArgs EA = new ExecuteEventArgs() { DBA = DBA, Operation = SQLType.Insert };
            EA.SqlParams = new ODAParameter[] { new ODAParameter() { ParamsName = "value", ParamsValue = Data } };
            EA.SqlParams = Prms;
            this.FireExecutingSqlEvent(EA);
            return EA.DBA.Import(Cmd.CmdName, Prms, Data);
        }
        #endregion
    }
}