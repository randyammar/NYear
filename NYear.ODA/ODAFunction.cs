using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NYear.ODA
{
    /// <summary>
    /// 应用数据库函数字段
    /// </summary>
    public class ODAFunction : ODAColumns, IODAFunction
    {
        private enum Func
        {
            Normal = 1,
            Self = 2,
            Case = 3,
            CaseWhen = 4,
            Exists = 5,
        }
        private string _FuncName = "";
        private List<ODAColumns> _FunColumnList = new List<ODAColumns>();
        private List<object> _FunArgs = new List<object>();
        private ODACmd _SubCmd = null;

        /// <summary>
        /// 0 非case语句;1 case column when 语句; 2 case when 语句
        /// </summary>
        private Func _FuncType = Func.Normal;
        private ODAColumns _CaseCol;
        private Dictionary<object, object> _CaseThen;
        private Dictionary<ODAColumns, object> _WhenThen;
        private object _CaseElseVal;
        private static volatile int AliasCount = 1;

        private string _ODAColumnName = "User_Function";
        internal override string ODAColumnName
        {
            get { return _ODAColumnName; }
        }
        /// <summary>
        /// 数据库函数
        /// </summary>
        private string FunctionName
        {
            get
            {
                return _FuncName;
            }
        }
        /// <summary>
        /// 数据库函数
        /// </summary>
        /// <param name="Cmd"></param>
        internal ODAFunction(ODACmd Cmd)
            : base(Cmd, "User_Function", ODAdbType.OVarchar, 4000)
        {
        }

        private ODAParameter[] GetFunctionArgs(out string SubSql)
        {
            List<ODAParameter> PrmsList = new List<ODAParameter>();
            string sql = "";
            AliasCount++;

            switch (_FuncType)
            {
                case Func.Normal:
                    #region Normal Function
                    {
                        for (int k = 0; k < _FunArgs.Count; k++)
                        {
                            if (_FunArgs[k] is ODAColumns)
                            {
                                string colSql;
                                ODAParameter[] prmSub = ((ODAColumns)_FunArgs[k]).GetSelectColumn(out colSql);
                                sql += colSql + ",";
                                if (prmSub != null && prmSub.Length > 0)
                                    PrmsList.AddRange(prmSub);
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ParamsName = _Cmd.ParamsMark + "FA" + "_" + AliasCount.ToString() + k.ToString() + _Cmd.Alias;
                                paramSub.ParamsValue = _FunArgs[k];
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (_FunArgs[k] is DateTime ? ODAdbType.ODatetime : _FunArgs[k].GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                                sql += paramSub.ParamsName + ",";
                                PrmsList.Add(paramSub);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(sql))
                            SubSql = "(" + sql.Remove(sql.Length - 1) + ")";
                        else
                            SubSql = sql;
                        return PrmsList.ToArray();
                    }
                #endregion
                case Func.Case:
                    #region CASE COLUMN WHEN
                    {
                        sql = "( CASE " + _CaseCol.ODAColumnName;
                        int k = 0;

                        foreach (KeyValuePair<object, object> wt in _CaseThen)
                        {
                            k++;
                            if (wt.Key is ODAColumns)
                            {
                                sql += " WHEN " + ((ODAColumns)wt.Key).GetColumnName();
                            }
                            else if (wt.Key is System.DBNull)
                            {
                                sql += " WHEN NULL";
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ParamsName = _Cmd.ParamsMark + "CSW" + "_" + AliasCount.ToString() + k.ToString();
                                paramSub.ParamsValue = wt.Key;
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (wt.Key is DateTime ? ODAdbType.ODatetime : wt.Value.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);

                                sql += " WHEN " + paramSub.ParamsName;
                                PrmsList.Add(paramSub);
                            }

                            if (wt.Value is ODAColumns)
                            {
                                sql += " THEN " + ((ODAColumns)wt.Value).ODAColumnName;
                            }
                            else if (wt.Value is System.DBNull)
                            {
                                sql += " THEN NULL ";
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ParamsName = _Cmd.ParamsMark + "CST" + "_" + AliasCount.ToString() + k.ToString();
                                paramSub.ParamsValue = wt.Value;
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (wt.Value is DateTime ? ODAdbType.ODatetime : wt.Value.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);

                                sql += " THEN " + paramSub.ParamsName;
                                PrmsList.Add(paramSub);
                            }
                        }

                        if (_CaseElseVal is ODAColumns)
                        {
                            sql += " ELSE " + ((ODAColumns)_CaseElseVal).GetColumnName() + " END )";
                        }
                        else if (_CaseElseVal is System.DBNull)
                        {
                            sql += " ELSE NULL END ) ";
                        }
                        else
                        {
                            ODAParameter paramSub = new ODAParameter();
                            paramSub.ParamsName = _Cmd.ParamsMark + "CSV" + "_" + AliasCount.ToString() + k.ToString();
                            paramSub.ParamsValue = _CaseElseVal;
                            paramSub.Direction = System.Data.ParameterDirection.Input;
                            paramSub.Size = 2000;
                            paramSub.DBDataType = (_CaseElseVal is DateTime ? ODAdbType.ODatetime : _CaseElseVal.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                            sql += " ELSE " + paramSub.ParamsName + " END ) ";
                            PrmsList.Add(paramSub);
                        }
                        this._FuncName = "";
                        SubSql = sql;
                        return PrmsList.ToArray();
                    }
                #endregion
                case Func.CaseWhen:
                    #region CASE WHEN COLUMN
                    {
                        sql = "( CASE ";
                        int k = 0;

                        foreach (KeyValuePair<ODAColumns, object> wt in _WhenThen)
                        {
                            k++;
                            string sqltmp = "";
                            ODAParameter[] prmstmp = wt.Key.GetWhereSubstring("CSW_" + AliasCount.ToString() + k.ToString(), out sqltmp);
                            sql += " WHEN " + sqltmp;
                            PrmsList.AddRange(prmstmp);
                            if (wt.Value is ODAColumns)
                            {
                                sql += " THEN " + ((ODAColumns)wt.Value).ODAColumnName;
                            }
                            else if (wt.Value is System.DBNull)
                            {
                                sql += " THEN NULL ";
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ParamsName = _Cmd.ParamsMark + "CST" + "_" + AliasCount.ToString() + k.ToString();
                                paramSub.ParamsValue = wt.Value;
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (wt.Value is DateTime ? ODAdbType.ODatetime : wt.Value.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                                sql += " THEN " + paramSub.ParamsName;
                                PrmsList.Add(paramSub);
                            }
                        }

                        if (_CaseElseVal is ODAColumns)
                        {
                            string slt;
                            ODAParameter[] sltPrm = ((ODAColumns)_CaseElseVal).GetSelectColumn(out slt);
                            if (sltPrm != null && sltPrm.Length > 0)
                                PrmsList.AddRange(sltPrm);
                            sql += " ELSE " + slt + " END )";
                        }
                        else if (_CaseElseVal is System.DBNull)
                        {
                            sql += " ELSE NULL END ) ";
                        }
                        else
                        {
                            ODAParameter paramSub = new ODAParameter();
                            paramSub.ParamsName = _Cmd.ParamsMark + "CSE" + "_" + AliasCount.ToString() + k.ToString();
                            paramSub.ParamsValue = _CaseElseVal;
                            paramSub.Direction = System.Data.ParameterDirection.Input;
                            paramSub.Size = 2000;
                            paramSub.DBDataType = (_CaseElseVal is DateTime ? ODAdbType.ODatetime : _CaseElseVal.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                            sql += " ELSE " + paramSub.ParamsName + " END ) ";
                            PrmsList.Add(paramSub);
                        }
                        this._FuncName = "";
                        SubSql = sql;
                        return PrmsList.ToArray();
                    }
                    #endregion
            }
            SubSql = "";
            return new ODAParameter[] { };
        }

        #region Oda Function
        /// <summary>
        /// 自定义函数
        /// </summary>
        /// <param name="Function">函数的名字</param>
        /// <param name="ParamsList">函数的参数</param>
        /// <returns></returns>
        public ODAColumns CreateFunc(string Function, ODAdbType ColumnType = ODAdbType.OVarchar, params object[] ParamsList)
        {
            _FuncType = Func.Normal;
            _FunArgs.AddRange(ParamsList);
            _FuncName = Function;
            _DBDataType = ColumnType;
            return this;
        }
        /// <summary>
        /// COUNT(*)函数
        /// </summary>
        public override ODAColumns Count
        {
            get
            {
                _FuncType = Func.Self;
                _FuncName = "COUNT(*)";
                _DBDataType = ODAdbType.ODecimal;
                return this;
            }
        }
        /// <summary>
        /// “存在”子查询函数
        /// </summary>
        /// <param name="Cmd"></param>
        /// <param name="Cols"></param>
        /// <returns></returns>
        public ODAColumns Exists(ODACmd Cmd, params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(Cmd.Alias))
            {
                _Cmd.SubCmdCout++;
                Cmd.Alias = "ST_" + _Cmd.SubCmdCout.ToString();
            }
            _SubCmd = Cmd;
            _FuncName = " EXISTS ";
            _FunColumnList.AddRange(Cols);
            _FuncType = Func.Exists;
            return this;
        }
        /// <summary>
        /// “不存在”子查询函数
        /// </summary>
        /// <param name="Cmd"></param>
        /// <param name="Cols"></param>
        /// <returns></returns>
        public ODAColumns NotExists(ODACmd Cmd, params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(Cmd.Alias))
            {
                _Cmd.SubCmdCout++;
                Cmd.Alias = "ST_" + _Cmd.SubCmdCout.ToString();
            }
            _SubCmd = Cmd;
            _FunColumnList.AddRange(Cols);
            _FuncName = " NOT EXISTS ";
            _FuncType = Func.Exists;
            return this;
        }
        /// <summary>
        /// 查询语句 case 函数
        /// </summary>
        /// <param name="CaseColumn">用作比对的字段</param>
        /// <param name="WhenThen">当case字段为key值时，则使用value值</param>
        /// <param name="ElseVal">所有WhenThen的key值都不满足时，使用ElseVal值</param>
        /// <returns></returns>
        public ODAColumns Case(ODAColumns CaseColumn, Dictionary<object, object> WhenThen, object ElseVal)
        {
            if (WhenThen == null)
                throw new ODAException(40001, "Param WhenThen can not be NULL");
            _CaseCol = CaseColumn;
            _CaseThen = WhenThen;
            _CaseElseVal = ElseVal;
            _FuncType = Func.Case;
            _FuncName = "";

            return this;
        }
        /// <summary>
        /// 查询语句 case 函数
        /// </summary>
        /// <param name="WhenThen">当 key 条件成立时，则使用 value 值</param>
        /// <param name="ElseVal">当所有 WhenThen 的 key 条件不成立时，则使用ElseVal值</param>
        /// <returns></returns>
        public ODAColumns CaseWhen(Dictionary<ODAColumns, object> WhenThen, object ElseVal)
        {
            if (WhenThen == null)
                throw new ODAException(40001, "Param WhenThen can not be NULL");
            _WhenThen = WhenThen;
            _CaseElseVal = ElseVal;
            _FuncType = Func.CaseWhen;
            _FuncName = "";
            return this;
        }
        public ODAColumns VisualColumn(string Val, ODAdbType ColumnType = ODAdbType.OVarchar)
        {
            _DBDataType = ColumnType;
            switch (ColumnType)
            {
                case ODAdbType.OVarchar:
                case ODAdbType.OChar:
                    _FuncName = "'" + Val + "'";
                    break;
                case ODAdbType.ODecimal:
                case ODAdbType.OInt:
                    _FuncName =   Val ;
                    break;
            }
            _FuncType = Func.Self;
            return this;
        }

        //public ODAColumns IsNullDefault(ODAColumns Col, object DefVal)
        //{
        //    return this;
        //}

        //public ODAColumns Decode(ODAColumns Col, Dictionary<object, object> TrueVale, object DefVal)
        //{
        //}

        #endregion
        /// <summary>
        /// 获取在 select 语句中使用的函表达式及其参数
        /// </summary>
        /// <param name="SubSql">函数表达式</param>
        /// <returns>函表达式的参数</returns>
        public override ODAParameter[] GetSelectColumn(out string SubSql)
        {

            if (_FuncType == Func.Exists)
                throw new ODAException(40001, string.Format("{0} Function not for Select", this.FunctionName));

            string colSql;
            List<ODAParameter> prms = new List<ODAParameter>();
            ODAParameter[] prms0 = base.GetSelectColumn(out colSql);
            string colName = this.GetColumnName();
            string sql;
            ODAParameter[] prms1 = GetFunctionArgs(out sql);
            SubSql = colSql.Replace(colName, _FuncName + sql);

            if (prms1 != null && prms1.Length > 0)
                prms.AddRange(prms1);
            if (prms0 != null && prms0.Length > 0)
                prms.AddRange(prms0);
            if (prms.Count > 0)
                return prms.ToArray();
            return prms0;

        }
        /// <summary>
        /// 获取在 insert 语句中使用的函表达式及其参数
        /// </summary>
        /// <param name="SubSql">insert 的字段名字</param>
        /// <param name="SubSqlParams">函数表达式</param>
        /// <returns>函数表达式的参数</returns>
        public override ODAParameter[] GetInsertSubstring(out string SubSql, out string SubSqlParams)
        {
            throw new ODAException(40001, string.Format("{0} Function not for Insert", this.FunctionName));
        }
        /// <summary>
        /// 获取在 update 语句中使用的函表达式及其参数
        /// </summary>
        /// <param name="SubSql">函数表达式</param>
        /// <returns>函数表达式的参数</returns>
        public override ODAParameter[] GetUpdateSubstring(out string SubSql)
        {
            throw new ODAException(40001, string.Format("{0} Function not for Update", this.FunctionName));
        }
        /// <summary>
        ///  获取在 where 语句中使用的函表达式及其参数
        /// </summary>
        /// <param name="ConIndex">此参数无效</param>
        /// <param name="SubSql">函数表达式</param>
        /// <returns>函数表达式的参数</returns>
        public override ODAParameter[] GetWhereSubstring(string ConIndex, out string SubSql)
        {
            string colSql = this.GetColumnName();
            ODAParameter[] prms0 = null;
            if (_Symbol != CmdConditionSymbol.NONE)
                prms0 = base.GetWhereSubstring(ConIndex, out colSql);

            string colName = this.GetColumnName();
            if (_FuncType == Func.Exists)
            {
                List<ODAParameter> prms = new List<ODAParameter>();
                string Colsql = "";
                _SubCmd.ParamsMark = _Cmd.ParamsMark;
               ODAParameter[] prms1 = ((IDBScriptGenerator)_SubCmd).GetSelectSql(out Colsql, _FunColumnList.ToArray());
                SubSql = colSql.Replace(colName, this._FuncName + " ( " + Colsql + ")");
                if (prms1 != null && prms1.Length > 0)
                    prms.AddRange(prms1);
                if (prms0 != null && prms0.Length > 0)
                    prms.AddRange(prms0);
                if (prms.Count > 0)
                    return prms.ToArray();
                return prms0;
            }
            else if (_FuncType == Func.Normal)
            {
                List<ODAParameter> prms = new List<ODAParameter>();
                string sql;
                ODAParameter[] prms1 = GetFunctionArgs(out sql);
                SubSql = colSql.Replace(colName, _FuncName + sql);

                if (prms1 != null && prms1.Length > 0)
                    prms.AddRange(prms1);
                if (prms0 != null && prms0.Length > 0)
                    prms.AddRange(prms0);
                if (prms.Count > 0)
                    return prms.ToArray();
                return prms0;
            }
            else
            {
                throw new ODAException(40006, string.Format("{0} Function not for query", this.FunctionName));
            }
        }
    }
}
