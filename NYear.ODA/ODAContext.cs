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
            cmd.GetDBAccess = GetDBAccess;
            cmd.Alias = Alias;
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
        protected void CheckTransaction(ODAScript Cmd)
        {
            //this._Tran.TransactionId
            //return;
        }


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
            var Sql = new ODAScript()
            {
                ScriptType = SQLType.BeginTransation,
            };
            Sql.SqlScript.Append("begin tran");
            FireExecutingSqlEvent(new ExecuteEventArgs()
            {
                SqlParams = Sql,
            });
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (_Tran != null)
            {
                _Tran.Commit();
                var Sql = new ODAScript()
                {
                    ScriptType = SQLType.Commit,
                };
                Sql.SqlScript.Append("Commit");

                FireExecutingSqlEvent(new ExecuteEventArgs()
                {
                    SqlParams = Sql,
                }); 
            } 
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollBack()
        {
            if (_Tran != null)
            {
                _Tran.RollBack();
                var Sql = new ODAScript()
                {
                    ScriptType = SQLType.Rollback,
                };
                Sql.SqlScript.Append("Rollback");
                FireExecutingSqlEvent(new ExecuteEventArgs()
                {
                    SqlParams = Sql,
                });
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
        private DataBaseSetting RoutToDatabase(ODAScript ODASql)
        {
            if (ODAContext.DataBaseSetting.Count == 1)
            {
                return ODAContext.DataBaseSetting[0];
            }
            else
            {
                //分库路由算法
                ////
                ////
            }
            throw new ODAException(30001, "没有找到适用的数据库");
        }

        /// <summary>
        /// 分库路由器,如果操作存在事务,返回的DB连接已在事务中
        /// 数据库主从集群
        /// </summary>
        /// <param name="ODASql"></param> 
        /// <returns></returns>
        private IDBAccess DatabaseRouting(ODAScript ODASql)
        {
            IDBAccess DBA = null;
            if (ODAContext.SystemTableGroups == null && string.IsNullOrWhiteSpace(this.dbConn))
                throw new ODAException(30000, "没有找到可用的数据库连接设定");

            if (!string.IsNullOrWhiteSpace(this.dbConn))
            {
                if (_Tran != null)
                {
                    if (_Tran.IsTimeout)
                        throw new ODAException(30000, "事务已超时"); 
                    if (_Tran.TransDB.ContainsKey(this.dbConn))
                    {
                        return _Tran.TransDB[this.dbConn];
                    }
                    else
                    {
                        DBA = ODAContext.NewDBConnect(this.dbType, this.dbConn);
                        DBA.BeginTransaction();
                        _Tran.DoCommit += DBA.Commit;
                        _Tran.RollBacking += DBA.RollBack;
                        _Tran.TransDB.Add(this.dbConn, DBA);
                        return DBA;
                    } 
                }
                else
                {
                   return ODAContext.NewDBConnect(this.dbType, this.dbConn);
                }
            }

            DataBaseSetting DB = RoutToDatabase(ODASql);
            if (_Tran != null)
            {
                if (_Tran.IsTimeout)
                    throw new ODAException(30000, "事务已超时"); 
                this.CheckTransaction(ODASql); 
                if (_Tran.TransDB.ContainsKey(DB.ConnectionString))
                {
                    return _Tran.TransDB[DB.ConnectionString];
                }
                else
                {
                    DBA = DBA = ODAContext.NewDBConnect(DB.DBtype, DB.ConnectionString);
                    DBA.BeginTransaction();
                    _Tran.DoCommit += DBA.Commit;
                    _Tran.RollBacking += DBA.RollBack;
                    _Tran.TransDB.Add(DB.ConnectionString, DBA);
                    return DBA;
                }
            }
            else
            {
                int curDt = 0;
                if (ODASql.ScriptType == SQLType.Select && DB.SlaveConnectionStrings != null && DB.SlaveConnectionStrings.Capacity != 0)
                {
                    ///主写从读集群，简单负载均衡
                    int curDate = int.Parse(System.DateTime.Now.ToString("ssfffff"));
                    curDt = curDate % DB.SlaveConnectionStrings.Count;
                    DBA = ODAContext.NewDBConnect(DB.DBtype, DB.SlaveConnectionStrings[curDt]);
                }
                else
                {
                    DBA = ODAContext.NewDBConnect(DB.DBtype, DB.ConnectionString);
                }
            }
            return DBA;
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
        protected IDBAccess GetDBAccess(ODAScript ODASql)
        {
            
            IDBAccess DBA = DatabaseRouting(ODASql); 
            ExecuteEventArgs earg = new ExecuteEventArgs()
            {
                DBA = DBA,
                SqlParams = ODASql,
            };
            this.FireExecutingSqlEvent(earg);
            return earg.DBA;
        }
        /// <summary>
        /// 分表查询设定
        /// </summary>
        /// <param name="Cmd"></param>
        protected virtual void SetSelectSplitTable(ODACmd Cmd)
        {
            //if (Cmd.BaseCmd != null)
            //{
            //    SetSelectSplitTable(Cmd.BaseCmd);
            //}
            //else
            //{
            //    string[] mTbl = TableRouting(SQLType.Select, Cmd, Cmd.WhereColumns.ToArray());
            //    Cmd.DBObjectMap = mTbl[0];
            //}
            //foreach (IDBScriptGenerator c in Cmd.ListJoinCmd)
            //    SetSelectSplitTable(c);
            //foreach (SqlJoinScript c in Cmd.JoinCmd)
            //    SetSelectSplitTable(c.JoinCmd);
            //foreach (ODAColumns c in Cmd.WhereColumns)
            //    if (c.InCmd != null)
            //        SetSelectSplitTable(c.InCmd);
        }
        #endregion
    }
}