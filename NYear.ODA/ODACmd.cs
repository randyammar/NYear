using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace NYear.ODA
{
    public class ODACmd : IDBScriptGenerator
    {
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
        protected virtual string DBObjectMap
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
        /// 数据库变量标识
        /// </summary>
        public virtual string ParamsMark
        {
            get;
            set;
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
        /// 转换成子查询
        /// </summary>
        /// <param name="Cols">子查询的字段</param>
        /// <returns></returns>
        public virtual ODACmdView ToView(params ODAColumns[] Cols)
        {
            return new ODACmdView(this, Cols)
            {
                Counting = this.Counting,
                Selecting = this.Selecting,
                SelectPaging = this.SelectPaging,
                SelectRecursion = this.SelectRecursion,
                Updating = this.Updating,
                Inserting = this.Inserting,
                Deleting = this.Deleting,
                ExecutingProcedure = this.ExecutingProcedure
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
                    throw new ODAException(10021, "ListCmd 对象不能是本身");
                if (string.IsNullOrWhiteSpace(Cmds[i].Alias))
                    Cmds[i].Alias = "LT" + (Tcount + i).ToString();
                _ListCmd.Add(Cmds[i]);
            }
            return this;
        }
        /// <summary>
        ///  内连接查询 
        /// </summary>
        /// <param name="JoinCmd">要连接的表</param>
        /// <param name="ONCols">连接条件</param>
        /// <returns></returns>
        public virtual ODACmd LeftJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            if (JoinCmd == this)
                throw new ODAException(10021, "LeftJoin 对象不能是本身");

            int Tcount = _ListCmd.Count + _JoinCmd.Count;
            if (string.IsNullOrWhiteSpace(JoinCmd.Alias))
                JoinCmd.Alias = "JT" + Tcount.ToString();
            JoinCmd.Where(ONCols);
            _JoinCmd.Add(new SqlJoinScript() { JoinCmd = JoinCmd, JoinScript = " LEFT JOIN " });
            return this;
        }
        /// <summary>
        ///  内连接查询 
        /// </summary>
        /// <param name="JoinCmd">要连接的表</param>
        /// <param name="ONCols">连接条件</param>
        /// <returns></returns>
        public virtual ODACmd InnerJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            if (JoinCmd == this)
                throw new ODAException(10021, "LeftJoin 对象不能是本身");

            int Tcount = _ListCmd.Count + _JoinCmd.Count;
            if (string.IsNullOrWhiteSpace(JoinCmd.Alias))
                JoinCmd.Alias = "JT" + Tcount.ToString();
            JoinCmd.Where(ONCols);
            _JoinCmd.Add(new SqlJoinScript() { JoinCmd = JoinCmd, JoinScript = " INNER JOIN " });
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
                throw new ODAException(10001, "MaxLevel should be smaller than  32");
            if (string.IsNullOrEmpty(ConnectByParent) || string.IsNullOrEmpty(PriorChild) || string.IsNullOrEmpty(StartWithExpress))
                throw new ODAException(10002, "StartWithExpress and ConnectByParent and PriorChild Can't be Empty");
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
                throw new ODAException(10003, "Where Condition is null,Add Where first");
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

        #region SQL命语句生成

        /// <summary>
        /// Cmd命令本身是一个查询语句
        /// </summary>
        /// <param name="DBObject"></param>
        /// <returns></returns>
        protected virtual ODAParameter[] GetCmdSql(out string DBObject)
        {
            DBObject = this.DBObjectMap;
            return new ODAParameter[0];
        }
        /// <summary>
        /// 获取查询语主中 form 字符串及变量
        /// </summary>
        /// <param name="SubSql"></param>
        /// <returns></returns>
        protected virtual ODAParameter[] GetFromSubString(out string SubSql)
        {
            List<ODAParameter> ParamList = new List<ODAParameter>();
            string AliasSql = String.IsNullOrWhiteSpace(_Alias) ? "" : " " + _Alias;
            string SelSql = null;
            ParamList.AddRange(GetCmdSql(out SelSql));
            SelSql = " FROM " + SelSql + AliasSql;

            for (int i = 0; i < _JoinCmd.Count; i++)
            {
                string CmdSql = "";
                ParamList.AddRange(_JoinCmd[i].JoinCmd.GetCmdSql(out CmdSql));
                string JoinSql = "";
                ParamList.AddRange(GetWhereSubSql(_JoinCmd[i].JoinCmd._WhereList, " AND ", out JoinSql));
                JoinSql = String.IsNullOrEmpty(JoinSql) ? "" : " ON " + JoinSql;
                SelSql += String.Format("{0}{1} {2}{3}", _JoinCmd[i].JoinScript, CmdSql, _JoinCmd[i].JoinCmd.Alias, JoinSql);
            }

            for (int i = 0; i < _ListCmd.Count; i++)
            {
                string CmdSql = "";
                ParamList.AddRange(_ListCmd[i].GetCmdSql(out CmdSql));
                SelSql += "," + CmdSql + " " + _ListCmd[i].Alias;
            }
            SubSql = SelSql;
            return ParamList.ToArray();
        }
        /// <summary>
        /// 获取查询语主中 where 字符串有变量
        /// </summary>
        /// <param name="WhereList"></param>
        /// <param name="RelationStr"></param>
        /// <param name="SubSql"></param>
        /// <returns></returns>
        protected virtual ODAParameter[] GetWhereSubSql(List<ODAColumns> WhereList, string RelationStr, out string SubSql)
        {
            List<ODAParameter> ParamsList = new List<ODAParameter>();
            string WSql = "";
            int A = 0;
            foreach (ODAColumns W in WhereList)
            {
                string WSqlSub = "";
                ODAParameter[] WParams = W.GetWhereSubstring(A.ToString(), out WSqlSub);
                WSql += WSqlSub + RelationStr;
                ParamsList.AddRange(WParams);
                A++;
            }

            SubSql = String.IsNullOrEmpty(WSql) ? "" : WSql.Substring(0, WSql.Length - RelationStr.Length);
            return ParamsList.ToArray();
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
        protected virtual ODAParameter[] GetCountSql(out string CountSql, ODAColumns Col)
        {
            if (string.IsNullOrWhiteSpace(_Alias))
                _Alias = "T";

            List<ODAParameter> ParamList = new List<ODAParameter>();
            string SelSql = "";
            if (_Groupby.Count > 0 || _Having.Count > 0) ///有group by 语句的使用子查询
            {
                throw new ODAException(10004, "NOT suport including GROUPBY or HAVING subsql, using GroupbyCount METHOD instead");
            }
            else ////没有group by 语句 直接count
            {
                SelSql = _Distinct ? "SELECT COUNT(DISTINCT " : "SELECT COUNT(";
                if (System.Object.ReferenceEquals(Col, null))
                {
                    SelSql += "*";
                }
                else
                {
                    string SubSelectSql = "";
                    ODAParameter[] SubSelectPrms = GetSelectColumns(",", out SubSelectSql, Col);
                    SelSql += SubSelectSql;
                    if (SubSelectPrms != null && SubSelectPrms.Length > 0)
                        ParamList.AddRange(SubSelectPrms);
                }
                SelSql += ") AS TOTAL_RECORD";
                string FromSubString = "";
                ParamList.AddRange(GetFromSubString(out FromSubString));
                SelSql += FromSubString;

                string WhereSql = "";
                ParamList.AddRange(GetWhereSubSql(_WhereList, " AND ", out WhereSql));
                string OrSql = "";
                ParamList.AddRange(GetWhereSubSql(_OrList, " OR ", out OrSql));
                SelSql += String.IsNullOrEmpty(WhereSql) ? String.IsNullOrEmpty(OrSql) ? "" : " WHERE " + OrSql : String.IsNullOrEmpty(OrSql) ? " WHERE " + WhereSql : " WHERE " + WhereSql + " OR " + OrSql;
                CountSql = SelSql;
                return ParamList.ToArray();
            }
        }

        /// <summary>
        /// 生成查询语句
        /// </summary>
        /// <param name="SelectSql">sql脚本</param>
        /// <param name="Cols">变量列表及变操作符</param>
        /// <returns>变量列表</returns>
        protected virtual ODAParameter[] GetSelectSql(out string SelectSql, params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(_Alias))
                _Alias = "T";



            List<ODAParameter> ParamList = new List<ODAParameter>();
            string SelSql = _Distinct ? "SELECT DISTINCT " : "SELECT ";

            if (Cols == null || Cols.Length == 0)
            {
                SelSql += "*";
            }
            else
            {
                string SubSelectSql = "";
                ODAParameter[] SubSelectPrms = GetSelectColumns(",", out SubSelectSql, Cols);
                SelSql += SubSelectSql;
                if (SubSelectPrms != null && SubSelectPrms.Length > 0)
                    ParamList.AddRange(SubSelectPrms);
            }

            string FromSubString = "";
            ParamList.AddRange(GetFromSubString(out FromSubString));
            SelSql += FromSubString;

            string WhereSql = "";
            ParamList.AddRange(GetWhereSubSql(_WhereList, " AND ", out WhereSql));
            string OrSql = "";
            ParamList.AddRange(GetWhereSubSql(_OrList, " OR ", out OrSql));
            SelSql += String.IsNullOrEmpty(WhereSql) ? String.IsNullOrEmpty(OrSql) ? "" : " WHERE " + OrSql : String.IsNullOrEmpty(OrSql) ? " WHERE " + WhereSql : " WHERE " + WhereSql + " OR " + OrSql;

            string GroupbySql = GetGroupByColumns(_Groupby.ToArray());
            SelSql += String.IsNullOrEmpty(GroupbySql) ? "" : " GROUP BY " + GroupbySql;

            string HavingSql = "";
            ParamList.AddRange(GetWhereSubSql(_Having, " AND ", out HavingSql));
            SelSql += String.IsNullOrEmpty(HavingSql) ? "" : " HAVING " + HavingSql;

            string Orderby = GetOrderbyColumns(_Orderby.ToArray());

            SelSql += String.IsNullOrEmpty(Orderby) ? "" : " ORDER BY " + Orderby;
            SelectSql = SelSql;
            return ParamList.ToArray();
        }
        /// <summary>
        /// 生成删除语
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        protected virtual ODAParameter[] GetDeleteSql(out string Sql)
        {
            string SubSql = "";
            List<ODAParameter> ParamList = new List<ODAParameter>();
            string WhereSql = "";
            ParamList.AddRange(GetWhereSubSql(_WhereList, " AND ", out WhereSql));
            string OrSql = "";
            ParamList.AddRange(GetWhereSubSql(_OrList, " OR ", out OrSql));
            SubSql = String.IsNullOrEmpty(WhereSql) ? String.IsNullOrEmpty(OrSql) ? "" : " WHERE " + OrSql : String.IsNullOrEmpty(OrSql) ? " WHERE " + WhereSql : " WHERE " + WhereSql + " OR " + OrSql;

            string AliasSql = String.IsNullOrEmpty(_Alias) ? "" : " " + _Alias;
            Sql = "DELETE FROM " + this.DBObjectMap + AliasSql + SubSql;
            return ParamList.ToArray();
        }

        /// <summary>
        /// 生成插入语句
        /// </summary>
        /// <param name="Sql">脚本</param>
        /// <param name="Cols">变量列表及变操作符</param>
        /// <returns>变量列表</returns>
        protected virtual ODAParameter[] GetInsertSql(out string Sql, params ODAColumns[] Cols)
        {
            List<ODAParameter> ParamList = new List<ODAParameter>();
            string Column = "";
            string ColumnParams = "";
            for (int i = 0; i < Cols.Length; i++)
            {
                string ColumnTmp = "";
                string ColumnParamsTmp = "";
                ParamList.AddRange(Cols[i].GetInsertSubstring(out ColumnTmp, out ColumnParamsTmp));
                Column += ColumnTmp + ",";
                ColumnParams += ColumnParamsTmp + ",";
            }
            Sql = "INSERT INTO " + this.DBObjectMap + "(" + Column.Remove(Column.Length - 1, 1) + ") VALUES (" + ColumnParams.Remove(ColumnParams.Length - 1, 1) + ")";
            return ParamList.ToArray();
        }

        /// <summary>
        /// 生成update语句
        /// </summary>
        /// <param name="Sql">脚本</param>
        /// <param name="Cols">变量列表及变操作符</param>
        /// <returns>变量列表</returns>
        protected virtual ODAParameter[] GetUpdateSql(out string Sql, params ODAColumns[] Cols)
        {
            List<ODAParameter> ParamList = new List<ODAParameter>();
            string Column = "";
            for (int i = 0; i < Cols.Length; i++)
            {
                string ColumnTmp = "";
                ODAParameter[] P = Cols[i].GetUpdateSubstring(out ColumnTmp);
                if (P != null)
                    ParamList.AddRange(P);
                Column += ColumnTmp + ",";
            }
            string SubSql = "";
            string WhereSql = "";
            ParamList.AddRange(GetWhereSubSql(_WhereList, " AND ", out WhereSql));
            string OrSql = "";
            ParamList.AddRange(GetWhereSubSql(_OrList, " OR ", out OrSql));
            SubSql = String.IsNullOrEmpty(WhereSql) ? String.IsNullOrEmpty(OrSql) ? "" : " WHERE " + OrSql : String.IsNullOrEmpty(OrSql) ? " WHERE " + WhereSql : " WHERE " + WhereSql + " OR " + OrSql;


            Sql = "UPDATE " + this.DBObjectMap + " SET " + Column.Remove(Column.Length - 1, 1) + SubSql;
            return ParamList.ToArray();
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
        internal CountEventHandler Counting = null;
        internal SelectEventHandler Selecting = null;
        internal SelectPagingEventHandler SelectPaging = null;
        internal SelectRecursionEventHandler SelectRecursion = null;
        internal UpdateEventHandler Updating = null;
        internal InsertEventHandler Inserting = null;
        internal InsertScriptEventHandler InsertScript = null;
        internal DeleteEventHandler Deleting = null;
        internal ExecuteProcedureEventHandler ExecutingProcedure = null;

        /// <summary>
        /// 在数据库中执行select count 语句,返统计结果
        /// </summary>
        /// <param name="Col"></param>
        /// <returns></returns>
        public virtual int Count(ODAColumns Col = null)
        {
            if (Counting == null)
                throw new ODAException(10006, "ODACmd Count 没有执行程序");
            return Counting(this, Col);

        }
        /// <summary>
        /// 在数据库中执行select语句,并返回结果集
        /// </summary>
        /// <param name="Cols">要select的字段</param>
        /// <returns></returns>
        public virtual DataTable Select(params ODAColumns[] Cols)
        {
            if (string.IsNullOrEmpty(_StartWithExpress) || string.IsNullOrEmpty(_ConnectByParent) || string.IsNullOrEmpty(_PriorChild))
            {
                if (Selecting == null)
                    throw new ODAException(10008, "ODACmd Select 没有执行程序 ");
                return Selecting(this, Cols);
            }
            else
            {
                if (SelectRecursion == null)
                    throw new ODAException(10009, "ODACmd SelectRecursion 没有执行程序");
                return SelectRecursion(this, _StartWithExpress, _ConnectByParent, _PriorChild, _ConnectColumn, _ConnectStr, _MaxLevel, Cols);
            }
        }

        /// <summary>
        /// 查询数据并转换为对象列表（取前多少条记录）
        /// </summary>
        /// <param name="StartIndex">起始记录的位置</param>
        /// <param name="MaxRecord">返回最大的记录数</param>
        /// <param name="TotalRecord">查询得到的总记录数</param>
        /// <param name="Cols">查询的字段，可为空，空则返回所有字段</param>
        /// <returns></returns>
        public virtual DataTable Select(int StartIndex, int MaxRecord, out int TotalRecord, params ODAColumns[] Cols)
        {
            if (_Groupby.Count > 0 && Cols.Length > 0 || _Distinct)
            {
                TotalRecord = this.ToView(Cols).Count();
            }
            else
            {
                TotalRecord = this.Count();
            }

            if (string.IsNullOrEmpty(_StartWithExpress) || string.IsNullOrEmpty(_ConnectByParent) || string.IsNullOrEmpty(_PriorChild))
            {
                if (SelectPaging == null)
                    throw new ODAException(10010, "ODACmd Select Paging 没有执行程序");
                return SelectPaging(this, StartIndex, MaxRecord, Cols);
            }
            else
            {
                if (SelectRecursion == null)
                    throw new ODAException(10011, "ODACmd SelectRecursion Paging  没有执行程序 ");
                DataTable dt = SelectRecursion(this, _StartWithExpress, _ConnectByParent, _PriorChild, _ConnectColumn, _ConnectStr, _MaxLevel, Cols);
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

        /// <summary>
        ///  查询数据并转换为对象列表
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="Cols">查询的字段，可为空，空则返回所有字段</param>
        /// <returns></returns>
        public virtual List<T> Select<T>(params ODAColumns[] Cols) where T : class
        {
            DataTable dt = Select(Cols);
            return DBAccess.ConvertToList<T>(dt);
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
            DataTable dt = Select(StartIndex, MaxRecord, out TotalRecord, Cols);
            return DBAccess.ConvertToList<T>(dt);
        }
        /// <summary>
        /// 在数据库中执行Delete 语句
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete()
        {
            if (Deleting == null)
                throw new ODAException(10012, "ODACmd Delete 没有执行程序 ");
            return Deleting(this);
        }
        /// <summary>
        /// 在数据库中执行update 语句
        /// </summary>
        /// <param name="Cols">需要更新的字段及其值</param>
        /// <returns></returns>
        public virtual bool Update(params ODAColumns[] Cols)
        {
            if (Updating == null)
                throw new ODAException(10013, "ODACmd Update 没有执行程序");
            this.ValidColumn(Cols);
            return Updating(this, Cols);
        }

        /// <summary>
        /// 在数据库中执行insert 语句
        /// </summary>
        /// <param name="Cols">插件的字段及其值</param>
        /// <returns></returns>
        public virtual bool Insert(params ODAColumns[] Cols)
        {
            if (Inserting == null)
                throw new ODAException(10014, "ODACmd Insert 没有执行程序");
            this.ValidColumn(Cols);
            return Inserting(this, Cols);
        }
        /// <summary>
        /// insert () select
        /// </summary>
        /// <param name="SelectCmd"></param>
        /// <param name="Cols">select的字段</param>
        /// <returns></returns>
        public virtual bool Insert(ODACmd SelectCmd, params ODAColumns[] Cols)
        {
            if (InsertScript == null)
                throw new ODAException(10014, "ODACmd Insert 没有执行程序");
            if (Cols == null || Cols.Length == 0)
                throw new ODAException(10014, "ScriptInsert 没有执行程序");
            return InsertScript(this, SelectCmd, Cols);
        }

        /// <summary>
        /// 在数据库中Procedure
        /// </summary>
        /// <param name="Cols">存储过程的参数及其值</param>
        /// <returns></returns>
        public virtual DataSet Procedure(params ODAColumns[] Cols)
        {
            if (ExecutingProcedure == null)
                throw new ODAException(10015, "ODACmd Procedure 没有执行程序");
            return ExecutingProcedure(this, Cols);
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
                throw new ODAException(10016, sbr.ToString());
            }
        }
        #endregion

        #region  Interface IDBScriptGenerator

        //string IDBScriptGenerator.Alias
        //{
        //    get { return this.Alias; }
        //    set { this.Alias = value; }
        //}
        //string IDBScriptGenerator.CmdName { get { return this.CmdName; } }

        ODACmd IDBScriptGenerator.BaseCmd { get { return this.BaseCmd; } }
        /// <summary>
        /// 操作的表名
        /// 用作分表
        ///   CmdName：没有分表的情况下就是表名
        ///   当对表[CmdName]纵向切割出N个分表时，DBObjectMap是根据路由条件临时给出表名 
        /// </summary>
        string IDBScriptGenerator.DBObjectMap
        {
            get { return this.DBObjectMap; }
            set { this.DBObjectMap = value; }
        }
        List<SqlJoinScript> IDBScriptGenerator.JoinCmd { get { return this.JoinCmd; } }
        List<ODACmd> IDBScriptGenerator.ListJoinCmd { get { return this.ListJoinCmd; } }
        List<ODAColumns> IDBScriptGenerator.WhereColumns { get { return this.WhereColumns; } }
        string IDBScriptGenerator.ParamsMark
        {
            get { return this.ParamsMark; }
            set { this.ParamsMark = value; }
        }
        ODAParameter[] IDBScriptGenerator.GetCountSql(out string CountSql, ODAColumns Col)
        {
            return this.GetCountSql(out CountSql, Col);
        }
        ODAParameter[] IDBScriptGenerator.GetDeleteSql(out string Sql)
        {
            return this.GetDeleteSql(out Sql);
        }
        ODAParameter[] IDBScriptGenerator.GetInsertSql(out string Sql, params ODAColumns[] Cols)
        {
            return this.GetInsertSql(out Sql, Cols);
        }
        ODAParameter[] IDBScriptGenerator.GetProcedureSql(out string Sql, params ODAColumns[] Cols)
        {
            return this.GetProcedureSql(out Sql, Cols);
        }
        ODAParameter[] IDBScriptGenerator.GetSelectSql(out string SelectSql, params ODAColumns[] Cols)
        {
            return this.GetSelectSql(out SelectSql, Cols);
        }
        ODAParameter[] IDBScriptGenerator.GetUpdateSql(out string Sql, params ODAColumns[] Cols)
        {
            return this.GetUpdateSql(out Sql, Cols);
        }
        #endregion
    }
}