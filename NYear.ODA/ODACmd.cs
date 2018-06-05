using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace NYear.ODA
{
    public partial class ODACmd //: IDBScriptGenerator
    {
        #region ODA指令寄存器
        internal int SubCmdCout = 0;
        private string _Alias = "";
        private string _StartWithExpress = null;
        private string _ConnectByParent = null;
        private string _PriorChild = null;
        private string _ConnectStr = "";
        private string _ConnectColumn = null;
        private int _MaxLevel = 32;

        protected bool _Distinct = false;
        protected List<ODAColumns> _WhereList = new List<ODAColumns>();
        protected List<ODAColumns> _OrList = new List<ODAColumns>();
        protected List<SqlOrderbyScript> _Orderby = new List<SqlOrderbyScript>();
        protected List<ODAColumns> _Groupby = new List<ODAColumns>();
        protected List<ODAColumns> _Having = new List<ODAColumns>();
        protected List<ODACmd> _ListCmd = new List<ODACmd>();
        protected List<SqlJoinScript> _JoinCmd = new List<SqlJoinScript>();

        protected List<ODAColumns> WhereColumns { get { return _WhereList; } }
        protected List<ODAColumns> OrColumns { get { return _OrList; } }
        protected List<ODACmd> ListJoinCmd { get { return _ListCmd; } }
        protected List<SqlJoinScript> JoinCmd { get { return _JoinCmd; } }
        protected virtual ODACmd BaseCmd { get { return null; } }
        protected virtual string DataBaseId { get { return null; } }
        #endregion

        #region 基础信息

        /// <summary>
        /// 命令名称
        /// </summary>
        public virtual string CmdName
        {
            get;
        }
        private string _DBObjectMap = string.Empty;
        /// <summary>
        /// 操作的表名
        /// 用作分表
        ///   CmdName：没有分表的情况下就是表名
        ///   当对表[CmdName]纵向切割出N个分表时，DBObjectMap是根据路由条件临时给出表名 
        /// </summary>
        public virtual string DBObjectMap
        {
            get
            {
                return string.IsNullOrWhiteSpace(_DBObjectMap) ? this.CmdName : _DBObjectMap;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _DBObjectMap = value;
            }
        }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias
        {
            get { return _Alias; }
            set { _Alias = value; }
        }
        /// <summary>
        /// 确定输入项长度
        /// </summary>
        public virtual System.Text.Encoding DBCharSet
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }

        #endregion

        #region ODA应用语法定义

        /// <summary>
        /// 查询去除重复
        /// </summary>
        public virtual ODACmd Distinct
        {
            get
            {
                _Distinct = true;
                return this;
            }
        }

        /// <summary>
        /// 新建查询函数
        /// </summary>
        public IODAFunction Function
        {
            get
            {
                return new ODAFunction(this);
            }
        }

        /// <summary>
        /// 这个表的所有字段,即 "*"
        /// </summary>
        public ODAColumns AllColumn
        {
            get
            {
                return new ODAColumns(this, "*");
            }
        }

        protected void SetSubViewAlias(ref int Id)
        {
            if (string.IsNullOrWhiteSpace(this.Alias) || this.Alias.StartsWith("LT") || this.Alias.StartsWith("JT"))
            {
                this.Alias = "VS" + Id.ToString();
            }

            for (int i = 0; i < _ListCmd.Count; i++)
            {
                Id++;
                _ListCmd[i].SetSubViewAlias(ref Id);
            }
            for (int i = 0; i < _JoinCmd.Count; i++)
            {
                Id++;
                _JoinCmd[i].JoinCmd.SetSubViewAlias(ref Id);
            }
        }
        /// <summary>
        /// 转换成子查询
        /// </summary>
        /// <param name="Cols">子查询的字段</param>
        /// <returns></returns>
        public virtual ODACmdView ToView(params ODAColumns[] Cols)
        {
            int Id = 0;
            this.SetSubViewAlias(ref Id);
            return new ODACmdView(this, Cols)
            {
                GetDBAccess = this.GetDBAccess,
            };
        }
        /// <summary>
        /// 内连接查询,形如：select * from table1 t1,table2 t2
        /// </summary>
        /// <param name="Cmds">要连接的表</param>
        /// <returns></returns>
        public virtual ODACmd ListCmd(params ODACmd[] Cmds)
        {
            int Tcount = _ListCmd.Count + _JoinCmd.Count;
            for (int i = 0; i < Cmds.Length; i++)
            {
                if (Cmds[i] == this)
                    throw new ODAException(10000, "ListCmd 对象不能是本身");
                if (string.IsNullOrWhiteSpace(Cmds[i].Alias))
                    Cmds[i].Alias = "LT" + (Tcount + i).ToString();
                _ListCmd.Add(Cmds[i]);
            }
            return this;
        }
        /// <summary>
        ///  左连接查询 
        /// </summary>
        /// <param name="JoinCmd">要连接的表</param>
        /// <param name="ONCols">连接条件</param>
        /// <returns></returns>
        public virtual ODACmd LeftJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            return Join(JoinCmd, " LEFT JOIN ", ONCols);
        }
        /// <summary>
        ///  右连接查询 
        /// </summary>
        /// <param name="JoinCmd">要连接的表</param>
        /// <param name="ONCols">连接条件</param>
        /// <returns></returns>
        public virtual ODACmd RightJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            return Join(JoinCmd, " RIGHT JOIN ", ONCols);
        }
        /// <summary>
        ///  内连接查询 
        /// </summary>
        /// <param name="JoinCmd">要连接的表</param>
        /// <param name="ONCols">连接条件</param>
        /// <returns></returns>
        public virtual ODACmd InnerJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            return Join(JoinCmd, " INNER JOIN ", ONCols);
        }
        /// <summary>
        /// 连接查询
        /// </summary>
        /// <param name="JoinCmd"></param>
        /// <param name="Join"></param>
        /// <param name="ONCols"></param>
        /// <returns></returns>
        protected virtual ODACmd Join(ODACmd JoinCmd, string Join, params ODAColumns[] ONCols)
        {
            if (JoinCmd == this)
                throw new ODAException(10002, "Inner Join Instance Can't be itselft");

            int Tcount = _ListCmd.Count + _JoinCmd.Count;
            if (string.IsNullOrWhiteSpace(JoinCmd.Alias))
                JoinCmd.Alias = "JT" + Tcount.ToString();
            JoinCmd.Where(ONCols);
            _JoinCmd.Add(new SqlJoinScript() { JoinCmd = JoinCmd, JoinScript = Join });
            return this;
        }

        /// <summary>
        /// 递归查询
        /// </summary>
        /// <param name="StartWithExpress">递归入口条件表达式,如 ColumnName = 0</param>
        /// <param name="ConnectByParent">递归父字段名称,如：ColumnParent </param>
        /// <param name="PriorChild">递归子字段名称,如ColumnChild</param>
        /// <param name="ConnectColumn">递归路径字段名称，用来返加父子层级关系</param>
        /// <param name="ConnectStr">父子层级之间的连接字符</param>
        /// <param name="MaxLevel">最大递归深度,最大递归深度不超过32层</param>
        /// <returns></returns>
        public virtual ODACmd StartWithConnectBy(string StartWithExpress, string ConnectByParent, string PriorChild, string ConnectColumn, string ConnectStr, int MaxLevel)
        {
            if (MaxLevel > 32)
                throw new ODAException(10003, "MaxLevel should be smaller than  32");
            if (string.IsNullOrWhiteSpace(ConnectByParent) || string.IsNullOrWhiteSpace(PriorChild) || string.IsNullOrWhiteSpace(StartWithExpress))
                throw new ODAException(10004, "StartWithExpress and ConnectByParent and PriorChild Can't be Empty");
            _StartWithExpress = StartWithExpress;
            _ConnectColumn = ConnectColumn;
            _ConnectByParent = ConnectByParent;
            _PriorChild = PriorChild;
            _ConnectStr = ConnectStr;
            _MaxLevel = MaxLevel;
            return this;
        }
        public virtual ODACmd OrderbyAsc(params ODAColumns[] ColumnNames)
        {
            if (ColumnNames != null)
                for (int i = 0; i < ColumnNames.Length; i++)
                    _Orderby.Add(new SqlOrderbyScript() { OrderbyCol = ColumnNames[i], OrderbyScript = " ASC " });
            return this;
        }
        public virtual ODACmd OrderbyDesc(params ODAColumns[] ColumnNames)
        {
            if (ColumnNames != null)
                for (int i = 0; i < ColumnNames.Length; i++)
                    _Orderby.Add(new SqlOrderbyScript() { OrderbyCol = ColumnNames[i], OrderbyScript = " DESC " });
            return this;
        }
        public virtual ODACmd Groupby(params ODAColumns[] ColumnNames)
        {
            _Groupby.AddRange(ColumnNames);
            return this;
        }
        public virtual ODACmd Having(params ODAColumns[] Params)
        {
            _Having.AddRange(Params);
            return this;
        }
        public virtual ODACmd Where(params ODAColumns[] Cols)
        {
            _WhereList.AddRange(Cols);
            return this;
        }
        public virtual ODACmd And(params ODAColumns[] Cols)
        {
            _WhereList.AddRange(Cols);
            return this;
        }
        public virtual ODACmd Or(params ODAColumns[] Cols)
        {
            if (_WhereList.Count == 0)
                throw new ODAException(10005, "Where Condition is null,Add Where first");
            _OrList.AddRange(Cols);
            return this;
        }

        //public virtual ODACmd Union(ODACmd Union,params ODAColumns[] Cols)
        //{
        //    return this;
        //}
        //public virtual ODACmd UnionAll(ODACmd Union,params ODAColumns[] Cols)
        //{
        //    return this;
        //}
        #endregion

        #region SQL语句生成

        /// <summary>
        /// Cmd命令本身是一个查询语句
        /// </summary>
        /// <param name="DBObject"></param>
        /// <returns></returns>
        protected virtual ODAScript GetCmdSql()
        {
            var sql = new ODAScript();
            sql.SqlScript.Append(this.DBObjectMap);
            sql.TableList.Add(this.DBObjectMap);
            return sql;
        }
        /// <summary>
        /// 获取查询语主中 form 字符串及变量
        /// </summary>
        /// <param name="SubSql"></param>
        /// <returns></returns>
        protected virtual ODAScript GetFromSubString()
        {
            var sql = new ODAScript();
            sql.SqlScript.Append(" FROM ");
            var cmdsql = GetCmdSql();
            sql.Merge(cmdsql);
            string AliasSql = String.IsNullOrWhiteSpace(Alias) ? "" : " " + Alias;
            sql.SqlScript.Append(AliasSql);

            for (int i = 0; i < _JoinCmd.Count; i++)
            {
                sql.SqlScript.Append(_JoinCmd[i].JoinScript);
                var join = _JoinCmd[i].JoinCmd.GetCmdSql();
                sql.Merge(join);
                sql.SqlScript.Append(" ").Append(_JoinCmd[i].JoinCmd.Alias);
                var wsql = GetWhereSubSql(_JoinCmd[i].JoinCmd._WhereList, " AND ");
                if (wsql.SqlScript.Length > 0)
                {
                    sql.SqlScript.Append(" ON ");
                    sql.Merge(wsql);
                }
            }
            for (int i = 0; i < _ListCmd.Count; i++)
            {
                var lsql = _ListCmd[i].GetCmdSql();
                sql.SqlScript.Append(",");
                sql.Merge(lsql).SqlScript.Append(" ").Append(_ListCmd[i].Alias);
            }
            return sql;
        }
        /// <summary>
        /// 获取查询语主中 where 字符串有变量
        /// </summary>
        /// <param name="WhereList"></param>
        /// <param name="RelationStr"></param>
        /// <param name="SubSql"></param>
        /// <returns></returns>
        protected virtual ODAScript GetWhereSubSql(List<ODAColumns> WhereList, string RelationStr)
        {
            var sql = new ODAScript();
            if (WhereList == null || WhereList.Count == 0)
                return sql;
            List<ODAParameter> ParamsList = new List<ODAParameter>();

            int A = 0;
            foreach (ODAColumns W in WhereList)
            {
                var sub = W.GetWhereSubstring(A.ToString());
                sql.Merge(sub);
                sql.SqlScript.Append(RelationStr);
                A++;
            }
            if (sql.SqlScript.Length > 0)
                sql.SqlScript.Remove(sql.SqlScript.Length - RelationStr.Length, RelationStr.Length);
            return sql;
        }

        /// <summary>
        /// 获取查询语主中 select 字符串有变量
        /// </summary>
        /// <param name="ConnectStr"></param>
        /// <param name="SubSql"></param>
        /// <param name="ColList"></param>
        /// <returns></returns>
        protected virtual ODAParameter[] GetSelectColumns(string ConnectStr, out string SubSql, params ODAColumns[] ColList)
        {
            string Sql = "";
            List<ODAParameter> ParamList = new List<ODAParameter>();
            foreach (ODAColumns Col in ColList)
            {
                string SubSelectSql = "";
                ODAParameter[] prms = Col.GetSelectColumn(out SubSelectSql);
                ParamList.AddRange(prms);
                Sql += SubSelectSql + ConnectStr;
            }
            SubSql = string.IsNullOrEmpty(Sql) ? "" : Sql.Substring(0, Sql.Length - ConnectStr.Length);
            return ParamList.ToArray();
        }

        /// <summary>
        ///  获取查询语主中 GroupBy 字符串有变量
        /// </summary>
        /// <param name="ColList"></param>
        /// <returns></returns>
        protected virtual string GetGroupByColumns(params ODAColumns[] ColList)
        {
            string Sql = "";
            foreach (ODAColumns Col in ColList)
            {
                Sql += Col.GetColumnName() + ",";
            }
            return string.IsNullOrEmpty(Sql) ? "" : Sql.Substring(0, Sql.Length - 1);
        }

        /// <summary>
        ///  获取查询语主中 Orderby 字符串有变量
        /// </summary>
        /// <param name="OrderbyList"></param>
        /// <returns></returns>
        protected virtual string GetOrderbyColumns(params SqlOrderbyScript[] OrderbyList)
        {
            string Sql = "";
            if (OrderbyList != null)
                for (int i = 0; i < OrderbyList.Length; i++)
                    Sql += OrderbyList[i].OrderbyCol.GetColumnName() + OrderbyList[i].OrderbyScript + ",";
            return string.IsNullOrEmpty(Sql) ? "" : Sql.Substring(0, Sql.Length - 1);
        }

        /// <summary>
        /// 生成统计数据行数的SQL
        /// </summary>
        /// <param name="CountSql"></param>
        /// <param name="Col"></param>
        /// <returns></returns>
        protected virtual ODAScript GetCountSql(ODAColumns Col)
        {
            if (_Groupby.Count > 0 || _Having.Count > 0)
                throw new ODAException(10006, "Do not count the [Group by] cmd,You should probably use [ ToView(Columns).Count()] instead.");
            if (string.IsNullOrWhiteSpace(Alias))
                Alias = "T";

            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Select,
                DataBaseId = this.DataBaseId,
            };
            if (_Distinct)
                sql.SqlScript.Append("SELECT COUNT(DISTINCT ");
            else
                sql.SqlScript.Append("SELECT COUNT(");
            if (System.Object.ReferenceEquals(Col, null))
            {
                sql.SqlScript.Append("*");
            }
            else
            {
                string SubSelectSql = "";
                ODAParameter[] SubSelectPrms = GetSelectColumns(",", out SubSelectSql, Col);
                sql.SqlScript.Append(SubSelectSql);

                if (SubSelectPrms != null && SubSelectPrms.Length > 0)
                    sql.ValueList.AddRange(SubSelectPrms);
            }
            sql.SqlScript.Append(") AS TOTAL_RECORD");
            var fSql = GetFromSubString();
            sql.Merge(fSql);

            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                sql.SqlScript.Append(" WHERE ");
                var asql = GetWhereSubSql(_WhereList, " AND ");
                sql.Merge(asql);
                if (_OrList.Count > 0)
                {
                    if (_WhereList.Count > 0)
                        sql.SqlScript.Append(" OR ");
                    var osql = GetWhereSubSql(_OrList, " OR ");
                    sql.Merge(osql);
                }
            }
            return sql;
        }

        /// <summary>
        /// 生成查询语句
        /// </summary>
        /// <param name="SelectSql">sql脚本</param>
        /// <param name="Cols">变量列表及变操作符</param>
        /// <returns>变量列表</returns>
        public virtual ODAScript GetSelectSql(params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(Alias))
                Alias = "T";
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Select,
                DataBaseId = this.DataBaseId
            };
            if (_Distinct)
                sql.SqlScript.Append("SELECT DISTINCT ");
            else
                sql.SqlScript.Append("SELECT ");

            if (Cols == null || Cols.Length == 0)
            {
                sql.SqlScript.Append(" * ");
            }
            else
            {
                string SubSelectSql = "";
                ODAParameter[] SubSelectPrms = GetSelectColumns(",", out SubSelectSql, Cols);
                sql.SqlScript.Append(SubSelectSql);
                if (SubSelectPrms != null && SubSelectPrms.Length > 0)
                    sql.ValueList.AddRange(SubSelectPrms);
            }
            var fSql = this.GetFromSubString();
            sql.Merge(fSql);

            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                sql.SqlScript.Append(" WHERE ");
                var asql = GetWhereSubSql(_WhereList, " AND ");
                sql.Merge(asql);
                if (_OrList.Count > 0)
                {
                    if (_WhereList.Count > 0)
                        sql.SqlScript.Append(" OR ");
                    var osql = GetWhereSubSql(_OrList, " OR ");
                    sql.Merge(osql);
                }
            }
            if (_Groupby.Count > 0)
            {
                string gy = GetGroupByColumns(_Groupby.ToArray());
                sql.SqlScript.Append(" GROUP BY ").Append(gy);
            }
            if (_Having.Count > 0)
            {
                sql.SqlScript.Append(" HAVING ");
                var hsql = GetWhereSubSql(_Having, " AND ");
                sql.Merge(hsql);
            }
            if (_Orderby.Count > 0)
            {
                string Orderby = GetOrderbyColumns(_Orderby.ToArray());
                sql.OrderBy = " ORDER BY " + Orderby;
                sql.SqlScript.Append(sql.OrderBy);
            }
            return sql;
        }
        /// <summary>
        /// 生成删除语
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        protected virtual ODAScript GetDeleteSql()
        {
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Delete,
                DataBaseId = this.DataBaseId
            };
            sql.TableList.Add(this.DBObjectMap);
            sql.SqlScript.Append("DELETE FROM ").Append(this.DBObjectMap);

            if (!string.IsNullOrWhiteSpace(Alias))
                sql.SqlScript.Append(" ").Append(Alias);

            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                sql.SqlScript.Append(" WHERE ");
                var asql = GetWhereSubSql(_WhereList, " AND ");
                sql.Merge(asql);
                if (_OrList.Count > 0)
                {
                    if (_WhereList.Count > 0)
                        sql.SqlScript.Append(" OR ");
                    var osql = GetWhereSubSql(_OrList, " OR ");
                    sql.Merge(osql);
                }
            }
            return sql;
        }

        /// <summary>
        /// 生成插入语句
        /// </summary>
        /// <param name="Sql">脚本</param>
        /// <param name="Cols">变量列表及变操作符</param>
        /// <returns>变量列表</returns>
        protected virtual ODAScript GetInsertSql(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length == 0)
                throw new ODAException(10018, "NO Columns for Insert!");
            this.Alias = "";
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Insert,
                DataBaseId = this.DataBaseId
            };
            sql.TableList.Add(this.DBObjectMap);
            sql.SqlScript.Append("INSERT INTO ").Append(this.DBObjectMap).Append("(");

            List<ODAParameter> ParamList = new List<ODAParameter>();
            string Column = "";
            string ColumnParams = "";
            for (int i = 0; i < Cols.Length; i++)
            {
                string ColumnTmp = "";
                string ColumnParamsTmp = "";
                sql.ValueList.AddRange(Cols[i].GetInsertSubstring(out ColumnTmp, out ColumnParamsTmp));
                Column += ColumnTmp + ",";
                ColumnParams += ColumnParamsTmp + ",";
            }
            sql.SqlScript.Append(Column.Remove(Column.Length - 1, 1)).Append(") VALUES (").Append(ColumnParams.Remove(ColumnParams.Length - 1, 1)).Append(")");
            return sql;
        }

        /// <summary>
        /// 生成update语句
        /// </summary>
        /// <param name="Sql">脚本</param>
        /// <param name="Cols">变量列表及变操作符</param>
        /// <returns>变量列表</returns>
        protected virtual ODAScript GetUpdateSql(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length == 0)
                throw new ODAException(10019, "NO Columns for update!");
            this.Alias = "";
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Update,
                DataBaseId = this.DataBaseId
            };
            sql.TableList.Add(this.DBObjectMap);
            sql.SqlScript.Append("UPDATE ").Append(this.DBObjectMap).Append(" SET ");
            string Column = "";
            for (int i = 0; i < Cols.Length; i++)
            {
                string ColumnTmp = "";
                ODAParameter[] P = Cols[i].GetUpdateSubstring(out ColumnTmp);
                if (P != null)
                    sql.ValueList.AddRange(P);
                Column += ColumnTmp + ",";
            }
            sql.SqlScript.Append(Column.Remove(Column.Length - 1, 1));
            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                sql.SqlScript.Append(" WHERE ");
                var asql = GetWhereSubSql(_WhereList, " AND ");
                sql.Merge(asql);
                if (_OrList.Count > 0)
                {
                    if (_WhereList.Count > 0)
                        sql.SqlScript.Append(" OR ");
                    var osql = GetWhereSubSql(_OrList, " OR ");
                    sql.Merge(osql);
                }
            }
            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Sql"></param>
        /// <param name="Cols"></param>
        /// <returns></returns>
        protected virtual ODAParameter[] GetProcedureSql(out string Sql, params ODAColumns[] Cols)
        {
            List<ODAParameter> ParamList = new List<ODAParameter>();
            Sql = this.DBObjectMap;
            for (int i = 0; i < Cols.Length; i++)
            {
                ParamList.Add(Cols[i].GetProcedureParams());
            }
            return ParamList.ToArray();
        }

        #endregion

        #region 执行SQL语句
        public GetDBAccessHandler GetDBAccess = null;
        /// <summary>
        /// 在数据库中执行select count 语句,返统计结果
        /// </summary>
        /// <param name="Col"></param>
        /// <returns></returns>
        public virtual int Count(ODAColumns Col = null)
        {
            try
            {
                return CountRecords(Col);
            }
            finally
            {
                this.Clear(); 
            } 
        }
        /// <summary>
        /// 在数据库中执行select count 语句,返统计结果
        /// </summary>
        /// <param name="Col"></param>
        /// <returns></returns>
        protected virtual int CountRecords(ODAColumns Col = null)
        {
            var sql = this.GetCountSql(Col);
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(10007, "ODACmd Count 没有执行程序");
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            object[] vl = db.SelectFirst(sql.SqlScript.ToString(), prms);
            return int.Parse(vl[0].ToString());
        }

        /// <summary>
        /// 在数据库中执行select语句,并返回结果集
        /// </summary>
        /// <param name="Cols">要select的字段</param>
        /// <returns></returns>
        public virtual DataTable Select(params ODAColumns[] Cols)
        {
            try
            {
                var sql = this.GetSelectSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10008, "ODACmd Select 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                if (string.IsNullOrEmpty(_StartWithExpress) || string.IsNullOrEmpty(_ConnectByParent) || string.IsNullOrEmpty(_PriorChild))
                {
                    return db.Select(sql.SqlScript.ToString(), prms);
                }
                else
                {
                    return db.Select(sql.SqlScript.ToString(), prms, _StartWithExpress, _ConnectByParent, _PriorChild, _ConnectColumn, _ConnectStr, _MaxLevel);
                }
            }
            finally
            {
                this.Clear();
            }
        }

        /// <summary>
        /// 查询数据（取前多少条记录）
        /// </summary>
        /// <param name="StartIndex">起始记录的位置</param>
        /// <param name="MaxRecord">返回最大的记录数</param>
        /// <param name="TotalRecord">查询得到的总记录数</param>
        /// <param name="Cols">查询的字段，可为空，空则返回所有字段</param>
        /// <returns></returns>
        public virtual DataTable Select(int StartIndex, int MaxRecord, out int TotalRecord, params ODAColumns[] Cols)
        {
            try
            {
                var sql = this.GetSelectSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10008, "ODACmd Select 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                if (string.IsNullOrEmpty(_StartWithExpress) || string.IsNullOrEmpty(_ConnectByParent) || string.IsNullOrEmpty(_PriorChild))
                {
                    if ((_Groupby.Count > 0 || _Having.Count > 0) || (Cols.Length > 0 && _Distinct))
                    {
                        if (_Orderby.Count > 0)
                        {
                            SqlOrderbyScript[] orderbys = this._Orderby.ToArray();
                            this._Orderby.Clear();
                            TotalRecord = this.ToView(Cols).CountRecords();
                            this._Orderby.AddRange(orderbys);
                        }
                        else
                        {
                            TotalRecord = this.ToView(Cols).CountRecords();
                        }
                    }
                    else
                    {
                        if (_Orderby.Count > 0)
                        {
                            SqlOrderbyScript[] orderbys = this._Orderby.ToArray();
                            this._Orderby.Clear();
                            TotalRecord = this.CountRecords();
                            this._Orderby.AddRange(orderbys);
                        }
                        else
                        {
                            TotalRecord = this.CountRecords();
                        }
                    }
                    return db.Select(sql.SqlScript.ToString(), prms, StartIndex, MaxRecord, sql.OrderBy);
                }
                else
                {
                    DataTable dt = db.Select(sql.SqlScript.ToString(), prms, _StartWithExpress, _ConnectByParent, _PriorChild, _ConnectColumn, _ConnectStr, _MaxLevel);
                    TotalRecord = dt.Rows.Count;
                    DataTable dtrtl = dt.Clone();
                    for (int i = StartIndex; i < StartIndex + MaxRecord; i++)
                    {
                        if (dt.Rows.Count > i)
                            dtrtl.Rows.Add(dt.Rows[i].ItemArray);
                        else
                            break;
                    }
                    return dtrtl;
                }
            }
            finally
            {
                this.Clear();
            }
        }

        /// <summary>
        ///  查询数据并转换为对象列表
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="Cols">查询的字段，可为空，空则返回所有字段</param>
        /// <returns></returns>
        public virtual List<T> Select<T>(params ODAColumns[] Cols) where T : class
        {
            try
            {
                var sql = this.GetSelectSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10009, "ODACmd Select 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                if (string.IsNullOrEmpty(_StartWithExpress) || string.IsNullOrEmpty(_ConnectByParent) || string.IsNullOrEmpty(_PriorChild))
                {
                    return db.Select<T>(sql.SqlScript.ToString(), prms);
                }
                else
                {
                    return db.Select<T>(sql.SqlScript.ToString(), prms, _StartWithExpress, _ConnectByParent, _PriorChild, _ConnectColumn, _ConnectStr, _MaxLevel);
                }
            }
            finally
            {
                this.Clear();
            }
        }
        /// <summary>
        /// 查询数据并转换为对象列表（取前多少条记录）
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="StartIndex">起始记录的位置</param>
        /// <param name="MaxRecord">返回最大的记录数</param>
        /// <param name="TotalRecord">查询得到的总记录数</param>
        /// <param name="Cols">查询的字段，可为空，空则返回所有字段</param>
        /// <returns></returns>
        public virtual List<T> Select<T>(int StartIndex, int MaxRecord, out int TotalRecord, params ODAColumns[] Cols) where T : class
        {
            try
            {
                var sql = this.GetSelectSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10010, "ODACmd Select 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);

                if (string.IsNullOrEmpty(_StartWithExpress) || string.IsNullOrEmpty(_ConnectByParent) || string.IsNullOrEmpty(_PriorChild))
                {
                    if ((_Groupby.Count > 0 || _Having.Count > 0) || (Cols.Length > 0 && _Distinct))
                    {
                        if (_Orderby.Count > 0)
                        {
                            SqlOrderbyScript[] orderbys = this._Orderby.ToArray();
                            this._Orderby.Clear();
                            TotalRecord = this.ToView(Cols).CountRecords();
                            this._Orderby.AddRange(orderbys);
                        }
                        else
                        {
                            TotalRecord = this.ToView(Cols).CountRecords();
                        }
                    }
                    else
                    {
                        if (_Orderby.Count > 0)
                        {
                            SqlOrderbyScript[] orderbys = this._Orderby.ToArray();
                            this._Orderby.Clear();
                            TotalRecord = this.CountRecords();
                            this._Orderby.AddRange(orderbys);
                        }
                        else
                        {
                            TotalRecord = this.CountRecords();
                        }
                    }
                    return db.Select<T>(sql.SqlScript.ToString(), prms, StartIndex, MaxRecord, sql.OrderBy);
                }
                else
                {
                    DataTable dt = db.Select(sql.SqlScript.ToString(), prms, _StartWithExpress, _ConnectByParent, _PriorChild, _ConnectColumn, _ConnectStr, _MaxLevel);
                    TotalRecord = dt.Rows.Count;
                    DataTable dtrtl = dt.Clone();
                    for (int i = StartIndex; i < StartIndex + MaxRecord; i++)
                    {
                        if (dt.Rows.Count > i)
                            dtrtl.Rows.Add(dt.Rows[i].ItemArray);
                        else
                            break;
                    }
                    return DBAccess.ConvertToList<T>(dtrtl);
                }
            }
            finally
            {
                this.Clear();
            }
        }
        /// <summary>
        /// 查询数据，接参数参顺序返回第一行的数据
        /// </summary>
        /// <param name="Cols">要查询的字段</param>
        /// <returns>第一行的数据据</returns>
        public object[] SelectFirst(params ODAColumns[] Cols)
        {
            try
            {
                var sql = this.GetSelectSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10011, "ODACmd Select 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                return db.SelectFirst(sql.SqlScript.ToString(), prms);
            }
            finally
            {
                this.Clear();
            }
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="Data">源数据</param>
        /// <param name="Prms">数据表对应的字段（Data.Row[n][ColumnIndex]与Prms[ColumnIndex]对应）</param>
        /// <returns></returns>
        public virtual bool Import(DataTable Data, ODAParameter[] Prms)
        {
            try
            {
                ODAScript sql = new ODAScript()
                {
                    ScriptType = SQLType.Import,
                };
                sql.TableList.Add(this.CmdName);
                sql.ValueList.AddRange(Prms);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10012, "ODACmd Import 没有执行程序");
                return db.Import(this.CmdName, Prms, Data);
            }
            finally
            {
                this.Clear();
            }
        }
        /// <summary>
        /// 在数据库中执行Delete 语句
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete()
        {
            try
            {
                var sql = this.GetDeleteSql();
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10013, "ODACmd Delete 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
            }
            finally
            {
                this.Clear();
            }
        }
        /// <summary>
        /// 在数据库中执行update 语句
        /// </summary>
        /// <param name="Cols">需要更新的字段及其值</param>
        /// <returns></returns>
        public virtual bool Update(params ODAColumns[] Cols)
        {
            try
            {
                var sql = this.GetUpdateSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10014, "ODACmd Update 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
            }
            finally
            {
                this.Clear();
            }
        }

        /// <summary>
        /// 在数据库中执行insert 语句
        /// </summary>
        /// <param name="Cols">插件的字段及其值</param>
        /// <returns></returns>
        public virtual bool Insert(params ODAColumns[] Cols)
        {
            try
            {
                var sql = this.GetInsertSql(Cols);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10015, "ODACmd Update 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
            }
            finally
            {
                this.Clear();
            }
        }
        /// <summary>
        /// insert () select
        /// </summary>
        /// <param name="SelectCmd"></param>
        /// <param name="Cols">select的字段</param>
        /// <returns></returns>
        public virtual bool Insert(ODACmd SelectCmd, params ODAColumns[] Cols)
        {
            try
            {
                var sql = new ODAScript()
                {
                    ScriptType = SQLType.Insert,
                };
                sql.SqlScript.Append("INSERT INTO ").Append(this.CmdName).Append("(");
                string Column = "";
                for (int i = 0; i < Cols.Length; i++)
                    Column += Cols[i].ColumnName + ",";
                sql.SqlScript.Append(Column.Remove(Column.Length - 1, 1)).Append(") ");
                var sSql = SelectCmd.GetSelectSql(Cols);
                sql.Merge(sSql);

                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10016, "ODACmd Insert 没有执行程序");
                var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
                sql.ValueList.CopyTo(prms, 0);
                sql.WhereList.CopyTo(prms, sql.ValueList.Count);
                return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
            }
            finally
            {
                this.Clear();
            }
        }

        /// <summary>
        /// 在数据库中Procedure
        /// </summary>
        /// <param name="Cols">存储过程的参数及其值</param>
        /// <returns></returns>
        public virtual DataSet Procedure(params ODAColumns[] Cols)
        {
            try
            {
                string sqlScript = "";
                var prms = this.GetProcedureSql(out sqlScript, Cols);
                var sql = new ODAScript()
                {
                    ScriptType = SQLType.Procedure,
                };
                sql.ValueList.AddRange(prms);
                sql.TableList.Add(sqlScript);
                var db = this.GetDBAccess(sql);
                if (db == null)
                    throw new ODAException(10017, "ODACmd Procedure 没有执行程序");
                return db.ExecuteProcedure(sqlScript, prms);
            }
            finally
            {
                this.Clear();
            }
        }

        /// <summary>
        /// 验证更新值是否合法
        /// </summary>
        /// <param name="Cols"></param>
        protected virtual void ValidColumn(params ODAColumns[] Cols)
        {
            StringBuilder sbr = new StringBuilder();
            foreach (ODAColumns c in Cols)
            {
                if (c.IsRequired)
                {
                    if (c.CompareValue == null || c.CompareValue == System.DBNull.Value || (c.DBDataType == ODAdbType.OVarchar && String.IsNullOrWhiteSpace(c.CompareValue.ToString())))
                    {
                        sbr.AppendLine((String.IsNullOrWhiteSpace(c.ColumnComment) ? c.ColumnName : c.ColumnComment) + "不能为空;");
                    }
                }
                if (c.DBDataType == ODAdbType.OVarchar && (c.CompareValue.ToString().Length > c.Size))
                    sbr.AppendLine((String.IsNullOrWhiteSpace(c.ColumnComment) ? c.ColumnName : c.ColumnComment) + "超出长度限制");
            }
            if (!string.IsNullOrWhiteSpace(sbr.ToString()))
            {
                sbr.Insert(0, "修改数据错误，");
                throw new ODAException(10018, sbr.ToString());
            }
        }
        protected virtual void Clear()
        {
            SubCmdCout = 0;
            _Alias = "";
            _StartWithExpress = null;
            _ConnectByParent = null;
            _PriorChild = null;
            _ConnectStr = "";
            _ConnectColumn = null;
            _MaxLevel = 32; 
            _Distinct = false;
            _WhereList.Clear();
            _OrList.Clear();
            _Orderby.Clear();
            _Groupby.Clear();
            _Having.Clear();
            _ListCmd.Clear();
            _JoinCmd.Clear();
        }

        #endregion
    }
}