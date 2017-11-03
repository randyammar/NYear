using System;
using System.Collections.Generic;
using System.Data;

namespace NYear.ODA
{

    /// <summary>
    /// ODA字段信息
    /// </summary>
    public class ODAColumns
    {
        private CmdFuntion _Fun = CmdFuntion.NONE;
        private List<SqlColumnScript> _SqlColumnList = new List<SqlColumnScript>();
        private object _CompareValue = null;
        private int _Size = 0;
        private string _ColumnComment = "";
        private bool _IsRequired = false;
        private string _AliasColumnName = null;
        private ParameterDirection _PDirection = ParameterDirection.Input;
        private volatile static int AliasCount = 1;
        private ODACmd _InCmd = null;
        private ODAColumns _InColumn = null;
        protected CmdConditionSymbol _Symbol = CmdConditionSymbol.NONE;
        protected string _ColumnName = "*";
        protected ODACmd _Cmd = null;
        protected ODAdbType _DBDataType = ODAdbType.OChar;

        internal virtual string ODAColumnName
        {
            get
            {
                return string.IsNullOrEmpty(_Cmd.Alias) ? _ColumnName : _Cmd.Alias + "." + _ColumnName;
            }
        }
        public string ColumnName
        {
            get
            {
                return _ColumnName;
            }
        }
        /// <summary>
        /// In 子查询命令
        /// </summary>
        public ODACmd InCmd
        {
            get
            {
                return _InCmd;
            }
        }
        public ODACmd ParentCmd
        {
            get
            {
                return _Cmd;
            }
        }
        public object CompareValue
        {
            get
            {
                return _CompareValue;
            }
        }
        public string AliasName
        {
            get
            {
                return _AliasColumnName;
            }
        }
        internal ODAdbType DBDataType { get { return _DBDataType; } }
        internal int Size { get { return _Size; } }
        internal string ColumnComment { get { return _ColumnComment; } }
        internal bool IsRequired { get { return _IsRequired; } }
        /// <summary>
        /// 创建ODA字段，默认 OVarchar（2000）,
        /// </summary>
        /// <param name="Cmd"></param>
        /// <param name="ColumnName"></param>
        public ODAColumns(ODACmd Cmd, string ColumnName)
        {
            this.ODAColumnsInit(Cmd, ColumnName, ODAdbType.OVarchar, 2000,null,false, ParameterDirection.Input);
        }
        public ODAColumns(ODACmd Cmd, string ColumnName, ODAdbType DBDataType)
        {
            this.ODAColumnsInit(Cmd, ColumnName, DBDataType, 2000,null,false, ParameterDirection.Input);
        }
        public ODAColumns(ODACmd Cmd, string ColumnName, ODAdbType DBDataType, int Size)
        {
            this.ODAColumnsInit(Cmd, ColumnName, DBDataType, Size,null,false,ParameterDirection.Input);
        }
        public ODAColumns(ODACmd Cmd, string ColumnName, ODAdbType DBDataType, int Size, bool IsRequired)
        {
            this.ODAColumnsInit(Cmd, ColumnName, DBDataType, Size,  null, IsRequired, ParameterDirection.Input);
        }

        public ODAColumns(ODACmd Cmd, string ColumnName, ODAdbType DBDataType, int Size, string ColumnComment, bool IsRequired,ParameterDirection Direction)
        {
            this.ODAColumnsInit(Cmd, ColumnName, DBDataType, Size, ColumnComment, IsRequired, Direction);
        }

        private void ODAColumnsInit(ODACmd Cmd, string ColumnName, ODAdbType DBDataType, int Size, string ColumnComment, bool IsRequired, ParameterDirection Direction)
        {
            if (Cmd == null)
                throw new ODAException(20003, string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment + "Cmd can't be null");
            _Cmd = Cmd;
            if (!String.IsNullOrEmpty(ColumnName))
                _ColumnName = ColumnName;
            _DBDataType = DBDataType;
            _Size = Size;
            _ColumnComment = ColumnComment;
            _PDirection = Direction;
            _IsRequired = IsRequired;
        }
        public  string GetColumnName()
        {
            string SColumn = "";
            switch (_Fun)
            {
                case CmdFuntion.AVG:
                    SColumn = "AVG(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.COUNT:
                    SColumn = "COUNT(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.MAX:
                    SColumn = "MAX(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.MIN:
                    SColumn = "MIN(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.SUM:
                    SColumn = "SUM(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.ASCII:
                    SColumn = "ASCII(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.LENGTH:
                    SColumn = "LENGTH(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.LTRIM:
                    SColumn = "LTRIM(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.RTRIM:
                    SColumn = "RTRIM(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.TRIM:
                    SColumn = "TRIM(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.UPPER:
                    SColumn = "UPPER(" + this.ODAColumnName + ")";
                    break;
                case CmdFuntion.LOWER:
                    SColumn = "LOWER(" + this.ODAColumnName + ")";
                    break;
                default:
                    SColumn = this.ODAColumnName;
                    break;
            }
            return SColumn;
        }
        #region Function
        public ODAColumns Max
        {
            get
            {
                _Fun = CmdFuntion.MAX;
                return this;
            }
        }
        public ODAColumns Min
        {
            get
            {
                _Fun = CmdFuntion.MIN;
                return this;
            }
        }
        public ODAColumns Avg
        {
            get
            {
                _Fun = CmdFuntion.AVG;
                this._DBDataType = ODAdbType.ODecimal;
                return this;
            }
        }
        public virtual ODAColumns Count
        {
            get
            {
                _Fun = CmdFuntion.COUNT;
                this._DBDataType = ODAdbType.ODecimal;
                return this;
            }
        }
        public ODAColumns Sum
        {
            get
            {
                _Fun = CmdFuntion.SUM;
                this._DBDataType = ODAdbType.ODecimal;
                return this;
            }
        }
        public ODAColumns Length
        {
            get
            {
                _Fun = CmdFuntion.LENGTH;
                return this;
            }
        }
        public ODAColumns Ltrim
        {
            get
            {
                _Fun = CmdFuntion.LTRIM;
                return this;
            }
        }
        public ODAColumns Rtrim
        {
            get
            {
                _Fun = CmdFuntion.RTRIM;
                return this;
            }
        }
        public ODAColumns Upper
        {
            get
            {
                _Fun = CmdFuntion.UPPER;
                return this;
            }
        }
        public ODAColumns Lower
        {
            get
            {
                _Fun = CmdFuntion.LOWER;
                return this;
            }
        }
        public ODAColumns Trim
        {
            get
            {
                _Fun = CmdFuntion.TRIM;
                return this;
            }
        }
        public ODAColumns Ascii
        {
            get
            {
                _Fun = CmdFuntion.ASCII;
                return this;
            }
        }
        #endregion
        public virtual ODAParameter[] GetSelectColumn(out string SubSql)
        {
            string SColumn = GetColumnName();
            if (!string.IsNullOrEmpty(_AliasColumnName))
                SubSql = SColumn + " AS " + _AliasColumnName;
            else
                SubSql = SColumn;
            return new ODAParameter[0];
        }
        public virtual ODAParameter[] GetInsertSubstring(out string SubSql, out string SubSqlParams)
        {
            if (_CompareValue is ODAFunction)
            {
                ODAParameter[] ps = ((ODAFunction)_CompareValue).GetInsertSubstring(out SubSql, out SubSqlParams);
                SubSql = _ColumnName;
                return ps;
            }
            else
            {
                ODAParameter P = new ODAParameter();
                string PName = ODAParameter.ODAParamsMark + _ColumnName + "_V" + _Cmd.Alias;
                if (PName.Length > 27)///变量太长
                {
                    AliasCount++;
                    PName = ODAParameter.ODAParamsMark + "P" + AliasCount.ToString() + _Cmd.Alias;
                }
                P.ParamsName = PName;
                P.ParamsValue = _CompareValue == null ? System.DBNull.Value : _CompareValue;
                P.DBDataType = _DBDataType;
                P.Direction = ParameterDirection.Input;
                P.Size = _Size;
                SubSql = _ColumnName;
                SubSqlParams = P.ParamsName;
                return new ODAParameter[] { P };
            }
        }
        public virtual ODAParameter[] GetUpdateSubstring(out string SubSql)
        {
            SubSql = "";
            List<ODAParameter> PList = new List<ODAParameter>();
            switch (_Symbol)
            {
                case CmdConditionSymbol.EQUAL:
                    SubSql = _ColumnName + "=";
                    break;
                case CmdConditionSymbol.ADD:
                    SubSql = _ColumnName + " + ";
                    break;
                case CmdConditionSymbol.REDUCE:
                    SubSql = _ColumnName + " - ";
                    break;
                case CmdConditionSymbol.TAKE:
                    SubSql = _ColumnName + " * ";
                    break;
                case CmdConditionSymbol.REMOVE:
                    SubSql = _ColumnName + " / ";
                    break;
                case CmdConditionSymbol.STAY:
                    SubSql = _ColumnName + " % ";
                    break;
                default:
                    throw new ODAException(20004,(string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment) + "CmdConditionSymbol Errror");
            }
            if (_CompareValue is ODAColumns)
            {
                string SubSqltmp = "";
                if (((ODAColumns)_CompareValue)._Symbol == CmdConditionSymbol.NONE)
                {
                    SubSql += ((ODAColumns)_CompareValue).ODAColumnName;
                }
                else
                {
                    ODAParameter[] p = ((ODAColumns)_CompareValue).GetUpdateSubstring(out SubSqltmp);
                    PList.AddRange(p);
                    SubSql += SubSqltmp;
                }
            }
            else
            {
                ODAParameter P = new ODAParameter();
                string PName = ODAParameter.ODAParamsMark + _ColumnName + "_V" + _Cmd.Alias;
                if (PName.Length > 27)///变量太长
                {
                    AliasCount++;
                    PName = ODAParameter.ODAParamsMark + "P" + AliasCount.ToString() + "_V" + _Cmd.Alias;
                }
                P.ParamsName = PName;
                P.ParamsValue = _CompareValue == null ? System.DBNull.Value : _CompareValue;
                P.DBDataType = _DBDataType;
                P.Direction = _PDirection;
                P.Size = _Size;

                SubSql += P.ParamsName;
                PList.Add(P);
            }
            return PList.ToArray();
        }
        public virtual ODAParameter[] GetWhereSubstring(string ConIndex, out string SubSql)
        {
            string SColumn = GetColumnName();
            List<ODAParameter> ParamsList = new List<ODAParameter>();
            if (_CompareValue is ODAColumns)
            {
                switch (_Symbol)
                {
                    case CmdConditionSymbol.BIGGER:
                        SColumn += " > ";
                        break;
                    case CmdConditionSymbol.EQUAL:
                        SColumn += " = ";
                        break;
                    case CmdConditionSymbol.LIKE:
                        SColumn += " LIKE ";
                        break;
                    case CmdConditionSymbol.NOTBIGGER:
                        SColumn += " <= ";
                        break;
                    case CmdConditionSymbol.NOTEQUAL:
                        SColumn += " <> ";
                        break;
                    case CmdConditionSymbol.NOTSMALLER:
                        SColumn += " >= ";
                        break;
                    case CmdConditionSymbol.SMALLER:
                        SColumn += " < ";
                        break;
                    case CmdConditionSymbol.ADD:
                        SColumn += " + ";
                        break;
                    case CmdConditionSymbol.REDUCE:
                        SColumn += " - ";
                        break;
                    case CmdConditionSymbol.TAKE:
                        SColumn += " * ";
                        break;
                    case CmdConditionSymbol.REMOVE:
                        SColumn += " / ";
                        break;
                    case CmdConditionSymbol.STAY:
                        SColumn += " % ";
                        break;
                    default:
                        throw new ODAException(20005,string.IsNullOrEmpty(_ColumnComment) ? _ColumnName + " not assign" : _ColumnComment + "CmdConditionSymbol Errror");
                }
                string SubSqltmp = "";
                if (((ODAColumns)_CompareValue)._Symbol == CmdConditionSymbol.NONE)
                    SColumn += ((ODAColumns)_CompareValue).ODAColumnName;
                else
                    ParamsList.AddRange(((ODAColumns)_CompareValue).GetWhereSubstring(ConIndex +"C", out SubSqltmp));
                SColumn += SubSqltmp;
            }
            else
            {
                ODAParameter param = new ODAParameter();
                string PName = ODAParameter.ODAParamsMark + _ColumnName + "_" + ConIndex + _Cmd.Alias;
                if (PName.Length > 27)///变量太长
                {
                    AliasCount++;
                    PName = ODAParameter.ODAParamsMark + "P" + AliasCount.ToString() + ConIndex + _Cmd.Alias;
                }

                param.ParamsName = PName;
                param.ParamsValue = _CompareValue == null ? System.DBNull.Value : _CompareValue;
                param.DBDataType = _DBDataType;
                param.Direction = _PDirection;
                param.Size = _Size;
                switch (_Symbol)
                {
                    case CmdConditionSymbol.BIGGER:
                        SColumn += " > " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.EQUAL:
                        SColumn += " = " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.IN:
                        if (_CompareValue != System.DBNull.Value)
                        {
                            object[] ValueList = (object[])_CompareValue;
                            SColumn += " IN (";
                            for (int k = 0; k < ValueList.Length; k++)
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ParamsName = ODAParameter.ODAParamsMark + _ColumnName + "_" + ConIndex + "_W" + k.ToString() + _Cmd.Alias;
                                paramSub.ParamsValue = ValueList[k];
                                paramSub.DBDataType = _DBDataType;
                                paramSub.Direction = _PDirection;
                                paramSub.Size = _Size;
                                ParamsList.Add(paramSub);
                                SColumn += paramSub.ParamsName + ",";
                            }
                            SColumn = SColumn.Remove(SColumn.Length - 1, 1) + ")";
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(_InCmd.Alias))
                            {
                                _Cmd.SubCmdCout++;
                                _InCmd.Alias = "ST_" + _Cmd.SubCmdCout.ToString();
                            }
                            string InSubsql = "";
                            ODAParameter[] subPrms = ((IDBScriptGenerator)_InCmd).GetSelectSql(out InSubsql, _InColumn);
                            SColumn += " IN ( " + InSubsql + ")";
                            ParamsList.AddRange(subPrms);
                        }
                        break;
                    case CmdConditionSymbol.NOTIN:
                        if (_CompareValue != System.DBNull.Value)
                        {
                            object[] ValueList = (object[])_CompareValue;
                            SColumn += " NOT IN (";
                            for (int k = 0; k < ValueList.Length; k++)
                            {
                                ODAParameter paramSub = new ODAParameter();
                                paramSub.ParamsName = ODAParameter.ODAParamsMark + _ColumnName + "_" + ConIndex + "_W" + k.ToString() + _Cmd.Alias;
                                paramSub.ParamsValue = ValueList[k];
                                paramSub.DBDataType = _DBDataType;
                                paramSub.Direction = _PDirection;
                                paramSub.Size = _Size;

                                ParamsList.Add(paramSub);
                                SColumn += paramSub.ParamsName + ",";
                            }
                            SColumn = SColumn.Remove(SColumn.Length - 1, 1) + ")";
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(_InCmd.Alias))
                            {
                                _Cmd.SubCmdCout++;
                                _InCmd.Alias = "ST_" + _Cmd.SubCmdCout.ToString();
                            }
                            string InSubsql = "";
                            ODAParameter[] subPrms = ((IDBScriptGenerator)_InCmd).GetSelectSql(out InSubsql, _InColumn);
                            SColumn += " NOT IN ( " + InSubsql + ")";
                            ParamsList.AddRange(subPrms);
                        }
                        break;
                    case CmdConditionSymbol.LIKE:
                        SColumn += " LIKE " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.NOTBIGGER:
                        SColumn += " <= " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.NOTEQUAL:
                        SColumn += " <> " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.NOTSMALLER:
                        SColumn += " >= " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.SMALLER:
                        SColumn += " < " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.ISNOTNULL:
                        SColumn += " IS NOT NULL ";
                        break;
                    case CmdConditionSymbol.ISNULL:
                        SColumn += " IS NULL ";
                        break;
                    case CmdConditionSymbol.ADD:
                        SColumn += " + " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.REDUCE:
                        SColumn += " - " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.TAKE:
                        SColumn += " * " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.REMOVE:
                        SColumn += " / " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    case CmdConditionSymbol.STAY:
                        SColumn += " % " + param.ParamsName;
                        ParamsList.Add(param);
                        break;
                    default:
                        throw new ODAException(20006,string.IsNullOrWhiteSpace(_ColumnComment) ? _ColumnName + " not assign" : _ColumnComment + "CmdConditionSymbol Errror");
                }
            }
            string cSql = "";
            for (int i = 0; i < _SqlColumnList.Count; i++)
            {
                string cSqlSub = "";
                ODAParameter[] AcParams = _SqlColumnList[i].SqlColumn.GetWhereSubstring(ConIndex + i.ToString(), out cSqlSub);
                cSql += _SqlColumnList[i].ConnScript + cSqlSub;
                ParamsList.AddRange(AcParams);
            }

            SubSql = String.IsNullOrEmpty(cSql) ? SColumn : "(" + SColumn + cSql + ")";
            return ParamsList.ToArray();
        }
        public virtual ODAParameter GetProcedureParams()
        {
            ODAParameter P = new ODAParameter();
            P.ParamsName = _ColumnName;
            P.ParamsValue = _CompareValue == null ? System.DBNull.Value : _CompareValue;
            P.DBDataType = _DBDataType;
            P.Direction = _PDirection;
            P.Size = _Size;
            return P;
        }
        public ODAColumns As(string AliasColumnName)
        {
            _AliasColumnName = AliasColumnName;
            return this;
        }
       
        public ODAColumns SetParamDataType(ODAdbType ParamType)
        {
            this._DBDataType = ParamType;
            return this;
        }
        public ODAColumns SetCondition(CmdConditionSymbol Symbol, object CompareValue)
        {
            if (Symbol == CmdConditionSymbol.NONE)
                throw new ODAException(20007,string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment + "CmdConditionSymbol Should not be NONE");
            if (_Symbol != CmdConditionSymbol.NONE)
                throw new ODAException(20008,string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment + "CmdConditionSymbol was setted");
            _Symbol = Symbol;
            _CompareValue = CompareValue == null ? System.DBNull.Value : CompareValue;
            return this;
        }
        public ODAColumns IsNull
        {
            get
            {
                return SetCondition(CmdConditionSymbol.ISNULL, System.DBNull.Value);
            }
        }
        public ODAColumns IsNotNull
        {
            get
            {
                return SetCondition(CmdConditionSymbol.ISNOTNULL, System.DBNull.Value);
            }
        }
        public ODAColumns Like(object CompareValue)
        {
            return SetCondition(CmdConditionSymbol.LIKE, CompareValue);
        }
        public ODAColumns In(params object[] CompareValue)
        {
            if (CompareValue == null)
                throw new ODAException(20009,(string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment) + " In statement CompareValue Can't be null");
            if (CompareValue.Length == 0)
                throw new ODAException(20010,(string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment) + " In statement  CompareValue Can't be empty");
            return SetCondition(CmdConditionSymbol.IN, CompareValue);
        }
        public ODAColumns NotIn(params object[] CompareValue)
        {
            if (CompareValue == null)
                throw new ODAException(20011,(string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment) + "In statement  CompareValue Can't be null");
            if (CompareValue.Length == 0)
                throw new ODAException(20012,(string.IsNullOrEmpty(_ColumnComment) ? _ColumnName : _ColumnComment) + "In statement CompareValue Can't be empty");
            return SetCondition(CmdConditionSymbol.NOTIN, CompareValue);
        }
        public ODAColumns In(ODACmd Cmd, ODAColumns Col)
        {
            if (Cmd == null || System.Object.ReferenceEquals(Col, null))
                throw new ODAException(20013,"Cmd and Col Args Can't be null");
            _InColumn = Col;
            _InCmd = Cmd;
            return SetCondition(CmdConditionSymbol.IN, null);
        }
        public ODAColumns NotIn(ODACmd Cmd, ODAColumns Col)
        {
            if (Cmd == null || System.Object.ReferenceEquals(Col, null))
                throw new ODAException(20014,"Cmd and Col Args Can't be null");
            _InColumn = Col;
            _InCmd = Cmd;
            return SetCondition(CmdConditionSymbol.NOTIN, null);
        }
        public ODAColumns And(params ODAColumns[] Columns)
        {
            if (Columns != null)
            {
                foreach (ODAColumns cl in Columns)
                    _SqlColumnList.Add(new SqlColumnScript() { ConnScript = " AND ", SqlColumn = cl });
            }
            return this;
        }
        public ODAColumns Or(params ODAColumns[] Columns)
        {
            if(Columns != null)
            {
                foreach (ODAColumns cl in Columns)
                    _SqlColumnList.Add(new SqlColumnScript() { ConnScript = " OR ", SqlColumn = cl });
            }
            return this;
        }

        #region 符号重载
        public static ODAColumns operator ==(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.EQUAL, CValue);
        }
        public static ODAColumns operator !=(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.NOTEQUAL, CValue);
        }
        public static ODAColumns operator >(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.BIGGER, CValue);
        }
        public static ODAColumns operator <(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.SMALLER, CValue);
        }
        public static ODAColumns operator <=(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.NOTBIGGER, CValue);
        }
        public static ODAColumns operator >=(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.NOTSMALLER, CValue);
        }
        public static ODAColumns operator +(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.ADD, CValue);
        }
        public static ODAColumns operator -(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.REDUCE, CValue);
        }
        public static ODAColumns operator *(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.TAKE, CValue);
        }
        public static ODAColumns operator /(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.REMOVE, CValue);
        }
        public static ODAColumns operator %(ODAColumns left, object CValue)
        {
            return left.SetCondition(CmdConditionSymbol.STAY, CValue);
        }
        #endregion
        public override int GetHashCode()
        {
            return _ColumnName.GetHashCode()
                + _CompareValue.GetHashCode()
                + _SqlColumnList.GetHashCode(); 
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this.GetHashCode() == obj.GetHashCode();
        }
    }
}