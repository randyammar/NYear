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
            Express = 6,
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

        private ODAScript GetFunctionArgs()
        {
            ODAScript sql = new ODAScript();
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
                                sql.SqlScript.Append(colSql).Append(",");
                                if (prmSub != null && prmSub.Length > 0)
                                    sql.ValueList.AddRange(prmSub);
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ColumnName = "@FunctionParameter";
                                paramSub.ParamsName = ODAParameter.ODAParamsMark + _Cmd.GetAlias();
                                paramSub.ParamsValue = _FunArgs[k];
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (_FunArgs[k] is DateTime ? ODAdbType.ODatetime : _FunArgs[k].GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                                sql.SqlScript.Append(paramSub.ParamsName).Append(",");
                                sql.ValueList.Add(paramSub);
                            }
                        }
                        if (sql.SqlScript.Length > 0)
                        {
                            sql.SqlScript.Remove(sql.SqlScript.Length - 1, 1);
                            sql.SqlScript.Insert(0, "(").Append(")");
                        }
                        return sql;
                    }
                #endregion
                case Func.Case:
                    #region CASE COLUMN WHEN
                    {
                        sql.SqlScript.Append("( CASE ").Append(_CaseCol.ODAColumnName);
                        foreach (KeyValuePair<object, object> wt in _CaseThen)
                        {
                            if (wt.Key is ODAColumns)
                            {
                                sql.SqlScript.Append(" WHEN ").Append(((ODAColumns)wt.Key).GetColumnName());
                            }
                            else if (wt.Key is System.DBNull)
                            {
                                sql.SqlScript.Append(" WHEN NULL ");
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ColumnName = _CaseCol.ODAColumnName;
                                paramSub.ParamsName = ODAParameter.ODAParamsMark + _Cmd.GetAlias();
                                paramSub.ParamsValue = wt.Key;
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (wt.Key is DateTime ? ODAdbType.ODatetime : wt.Value.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);

                                sql.SqlScript.Append(" WHEN ").Append(paramSub.ParamsName);
                                sql.ValueList.Add(paramSub);
                            }

                            if (wt.Value is ODAColumns)
                            {
                                sql.SqlScript.Append(" THEN ").Append(((ODAColumns)wt.Value).ODAColumnName);
                            }
                            else if (wt.Value is System.DBNull)
                            {
                                sql.SqlScript.Append(" THEN NULL ");
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ColumnName = _CaseCol.ODAColumnName;
                                paramSub.ParamsName = ODAParameter.ODAParamsMark + _Cmd.GetAlias(); ;
                                paramSub.ParamsValue = wt.Value;
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (wt.Value is DateTime ? ODAdbType.ODatetime : wt.Value.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);

                                sql.SqlScript.Append(" THEN ").Append(paramSub.ParamsName);
                                sql.ValueList.Add(paramSub);
                            }
                        }

                        if (_CaseElseVal is ODAColumns)
                        {
                            sql.SqlScript.Append(" ELSE ").Append(((ODAColumns)_CaseElseVal).GetColumnName()).Append(" END )");
                        }
                        else if (_CaseElseVal is System.DBNull)
                        {
                            sql.SqlScript.Append(" ELSE NULL END ) ");
                        }
                        else
                        {
                            ODAParameter paramSub = new ODAParameter();
                            paramSub.ColumnName = _CaseCol.ODAColumnName;
                            paramSub.ParamsName = ODAParameter.ODAParamsMark + _Cmd.GetAlias();
                            paramSub.ParamsValue = _CaseElseVal;
                            paramSub.Direction = System.Data.ParameterDirection.Input;
                            paramSub.Size = 2000;
                            paramSub.DBDataType = (_CaseElseVal is DateTime ? ODAdbType.ODatetime : _CaseElseVal.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                            sql.SqlScript.Append(" ELSE ").Append(paramSub.ParamsName).Append(" END ) ");
                            sql.ValueList.Add(paramSub);
                        }
                        this._FuncName = "";
                        return sql;
                    }
                #endregion
                case Func.CaseWhen:
                    #region CASE WHEN COLUMN
                    {
                        sql.SqlScript.Append("( CASE "); 
                        foreach (KeyValuePair<ODAColumns, object> wt in _WhenThen)
                        { 
                            sql.SqlScript.Append(" WHEN ");
                            var wSql = wt.Key.GetWhereSubstring();
                            sql.Merge(wSql);
                            if (wt.Value is ODAColumns)
                            {
                                sql.SqlScript.Append(" THEN ").Append(((ODAColumns)wt.Value).ODAColumnName);
                            }
                            else if (wt.Value is System.DBNull)
                            {
                                sql.SqlScript.Append(" THEN NULL ");
                            }
                            else
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ColumnName = wt.Key.ODAColumnName;
                                paramSub.ParamsName = ODAParameter.ODAParamsMark + _Cmd.GetAlias();
                                paramSub.ParamsValue = wt.Value;
                                paramSub.Direction = System.Data.ParameterDirection.Input;
                                paramSub.Size = 2000;
                                paramSub.DBDataType = (wt.Value is DateTime ? ODAdbType.ODatetime : wt.Value.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                                sql.SqlScript.Append(" THEN ").Append(paramSub.ParamsName);
                                sql.ValueList.Add(paramSub);
                            }
                        }

                        if (_CaseElseVal is ODAColumns)
                        {
                            string slt;
                            ODAParameter[] sltPrm = ((ODAColumns)_CaseElseVal).GetSelectColumn(out slt);
                            if (sltPrm != null && sltPrm.Length > 0)
                                sql.ValueList.AddRange(sltPrm);
                            sql.SqlScript.Append(" ELSE ").Append(slt).Append(" END )");
                        }
                        else if (_CaseElseVal is System.DBNull)
                        {
                            sql.SqlScript.Append(" ELSE NULL END ) ");
                        }
                        else
                        {
                            ODAParameter paramSub = new ODAParameter();
                            paramSub.ColumnName = "";
                            paramSub.ParamsName = ODAParameter.ODAParamsMark + _Cmd.GetAlias();
                            paramSub.ParamsValue = _CaseElseVal;
                            paramSub.Direction = System.Data.ParameterDirection.Input;
                            paramSub.Size = 2000;
                            paramSub.DBDataType = (_CaseElseVal is DateTime ? ODAdbType.ODatetime : _CaseElseVal.GetType().IsPrimitive ? ODAdbType.ODecimal : ODAdbType.OVarchar);
                            sql.SqlScript.Append(" ELSE ").Append(paramSub.ParamsName).Append(" END ) ");
                            sql.ValueList.Add(paramSub);
                        }
                        this._FuncName = "";
                        return sql;
                    }
                #endregion
            }
            return sql;
        }

        #region Oda Function
        /// <summary>
        /// 自定义函数
        /// </summary>
        /// <param name="Function">函数的名字</param>
        /// <param name="ParamsList">函数的参数</param>
        /// <returns></returns>
        public ODAColumns Express(string Expression)
        {
            _FuncType = Func.Express;
            _FuncName = Expression; 
            return this;
        }
        /// <summary>
        /// 使用常量创建一个虚拟字段
        /// </summary>
        /// <param name="Val"></param>
        /// <returns></returns>
        public ODAColumns VisualColumn(string Val)
        {
            return Express(Val); 
        }
        /// <summary>
        /// 自定义函数
        /// </summary>
        /// <param name="Function">函数的名字</param>
        /// <param name="ParamsList">函数的参数</param>
        /// <returns></returns>
        public ODAColumns CreateFunc(string Function, params object[] ParamsList)
        {
            _FuncType = Func.Normal;
            _FunArgs.AddRange(ParamsList);
            _FuncName = Function; 
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
                Cmd.Alias = _Cmd.GetAlias();
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
                Cmd.Alias = _Cmd.GetAlias();
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


        public ODAColumns NullDefault(ODAColumns Col, object DefVal)
        {
            Dictionary<ODAColumns, object> WhenThen = new Dictionary<ODAColumns, object>();
            WhenThen.Add(Col.IsNull, DefVal);
            return this.CaseWhen(WhenThen, Col);
        }
        public ODAColumns Decode(ODAColumns Col, object DefVal, params object[] KeyValue)
        {
            if (KeyValue == null || KeyValue.Length % 2 != 0)
                throw new ODAException(40007, "Decode Method ,params [KeyValue] duable case ");
            Dictionary<object, object> TrueVale = new Dictionary<object, object>();
            for (int i = 0; i < KeyValue.Length; i += 2)
                TrueVale.Add(KeyValue[i], KeyValue[i + 1]);
            return this.Case(Col, TrueVale, DefVal);
        }

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

            ODAScript cSql = new ODAScript();
            string colSql;
            ODAParameter[] prms0 = base.GetSelectColumn(out colSql);
            string colName = this.GetColumnName();
            cSql.SqlScript.Append(colSql);
            if (prms0 != null && prms0.Length > 0)
                cSql.ValueList.AddRange(prms0);

            var fSql = GetFunctionArgs();
            SubSql = colSql.Replace(colName, _FuncName + fSql.SqlScript.ToString());
            if (prms0 != null && prms0.Length > 0)
            {
                var fprms = new ODAParameter[fSql.ValueList.Count + fSql.WhereList.Count + prms0.Length];
                fSql.ValueList.CopyTo(fprms, 0);
                fSql.WhereList.CopyTo(fprms, fSql.ValueList.Count);
                prms0.CopyTo(fprms, fSql.ValueList.Count + fSql.WhereList.Count);
                return fprms;
            }
            else
            {
                var fprms = new ODAParameter[fSql.ValueList.Count + fSql.WhereList.Count];
                fSql.ValueList.CopyTo(fprms, 0);
                fSql.WhereList.CopyTo(fprms, fSql.ValueList.Count);
                return fprms;
            }
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
        public override ODAScript GetWhereSubstring()
        {
            ODAScript sql = new ODAScript();
            if (_Symbol != CmdConditionSymbol.NONE)
            {
                var wSql = base.GetWhereSubstring();
                sql.Merge(wSql);
            }
            else
            {
                sql.SqlScript.Append(this.GetColumnName());
            }

            string colName = this.GetColumnName();
            if (_FuncType == Func.Exists)
            {
                var sSql = ((ODACmd)_SubCmd).GetSelectSql(_FunColumnList.ToArray());
                string SubSql = sql.SqlScript.Replace(colName, this._FuncName + " ( " + sSql.SqlScript + ")").ToString(); ;

                sql.Merge(sSql);
                sql.SqlScript.Clear();
                sql.SqlScript.Append(SubSql);
                return sql;
            }
            else if (_FuncType == Func.Normal)
            {
                var fSql = GetFunctionArgs();
                string SubSql = sql.SqlScript.Replace(colName, _FuncName + fSql.SqlScript.ToString()).ToString();

                sql.Merge(fSql);
                sql.SqlScript.Clear();
                sql.SqlScript.Append(SubSql);
                return sql;
            }
            else if (_FuncType == Func.Self)
            {
                string tmpCol = _Cmd.GetAlias();
                _ODAColumnName = tmpCol;
                var wSql = base.GetWhereSubstring();
                wSql.SqlScript.Replace(tmpCol, this.FunctionName); ///形如：count(*) > 0
                return wSql;
            }
            else if (_FuncType == Func.Express)
            {
                ODAScript wSql = new ODAScript();
                wSql.SqlScript.Append(this.FunctionName);
                return wSql;
            }
            return sql;
        }
    }
}
