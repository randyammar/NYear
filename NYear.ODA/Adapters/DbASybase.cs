using Sybase.Data.AseClient;
using System;
using System.Data;
using System.Data.Common;

namespace NYear.ODA.Adapter
{
    public class DbASybase : DBAccess
    {
        private static char ParamsMark { get { return ':'; } }

        public DbASybase(string ConnectionString)
            : base(ConnectionString)
        {
        }

        private AseConnection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new AseConnection(ConnString);
            if (_DBConn.State == ConnectionState.Closed)
                _DBConn.Open();
            _DBConn.Disposed += _DBConn_Disposed;
            return _DBConn;
        }
        private void _DBConn_Disposed(object sender, EventArgs e)
        {
            _DBConn = null;
        }

        protected override DbDataAdapter GetDataAdapter(IDbCommand SelectCmd)
        {
            return new AseDataAdapter((AseCommand)SelectCmd);
        }
        public override DateTime GetDBDateTime()
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                string sql = "SELECT getdate() as  DB_DATETIME ";
                Cmd.CommandText = sql;
                Cmd.CommandType = CommandType.Text;
                return Convert.ToDateTime(Cmd.ExecuteScalar());
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        public override string[] GetUserTables()
        {
            DataTable dt_table = this.Select("SELECT T.name TABLE_NAME FROM sysobjects T WHERE  T.type='U' ORDER BY TABLE_NAME", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            DataTable dt_table = this.Select("SELECT V.name VIEW_NAME FROM sysobjects  V WHERE V.type ='V' AND V.name <>'sysquerymetrics' ORDER BY  VIEW_NAME", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["VIEW_NAME"].ToString();
            }
            return str;
        }
        public override DbAType DBAType { get { return DbAType.Sybase; } }

        public override DataTable GetTableColumns()
        {
            string sql_tabcol = " SELECT SOBJ.name AS TABLE_NAME, SCOL.name AS COLUMN_NAME ,"
            + " CASE STYPE.name  WHEN 'sysname'  THEN 'OVarchar' WHEN 'sql_varint'  THEN 'OVarchar'    WHEN 'varchar' THEN 'OVarchar' WHEN 'char'  THEN 'OChar' "
            + " WHEN 'nchar'  THEN 'OChar' WHEN 'ntext'  THEN 'OVarchar'  WHEN 'nvarchar'  THEN 'OVarchar'  WHEN 'text'  THEN 'OVarchar' WHEN 'varchar'  THEN 'OVarchar' "
            + " WHEN 'bigint'  THEN 'ODecimal'  WHEN 'decimal'  THEN 'ODecimal'   WHEN 'float'  THEN 'ODecimal'   WHEN 'money'  THEN 'ODecimal' "
            + " WHEN 'numeric'  THEN 'ODecimal'    WHEN 'real'  THEN 'ODecimal'   WHEN 'smallmoney'  THEN 'ODecimal' "
            + " WHEN 'int'  THEN 'OInt'  WHEN 'smallint'  THEN 'OInt'    WHEN 'bit'  THEN 'OInt'  "
            + " WHEN 'datetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  "
            + " WHEN 'binary'  THEN 'OBinary'  WHEN 'image'  THEN 'OBinary'  WHEN 'timestamp'  THEN 'OBinary'    WHEN 'varbinary'  THEN 'OBinary' "
            + " ELSE  STYPE.name  END AS ODA_DATATYPE ,"
            + " SCOL.length AS LENGTH, 'INPUT' AS DIRECTION "
            + " FROM sysobjects SOBJ,syscolumns SCOL,systypes STYPE "
            + " WHERE  SOBJ.type  = 'U' "
            + " AND SOBJ.id = SCOL.id"
            + " AND STYPE.usertype = SCOL.usertype"
            + " ORDER BY  TABLE_NAME , COLUMN_NAME ";
            DataTable Dt = Select(sql_tabcol, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            string sql_view = "SELECT SOBJ.name AS TABLE_NAME, SCOL.name AS COLUMN_NAME ,"
            + " CASE STYPE.name  WHEN 'sysname'  THEN 'OVarchar' WHEN 'sql_varint'  THEN 'OVarchar'    WHEN 'varchar' THEN 'OVarchar' WHEN 'char'  THEN 'OChar' "
            + " WHEN 'nchar'  THEN 'OChar' WHEN 'ntext'  THEN 'OVarchar'  WHEN 'nvarchar'  THEN 'OVarchar'  WHEN 'text'  THEN 'OVarchar' WHEN 'varchar'  THEN 'OVarchar' "
            + " WHEN 'bigint'  THEN 'ODecimal'  WHEN 'decimal'  THEN 'ODecimal'   WHEN 'float'  THEN 'ODecimal'   WHEN 'money'  THEN 'ODecimal' "
            + " WHEN 'numeric'  THEN 'ODecimal'    WHEN 'real'  THEN 'ODecimal'   WHEN 'smallmoney'  THEN 'ODecimal' "
            + " WHEN 'int'  THEN 'OInt'  WHEN 'smallint'  THEN 'OInt'    WHEN 'bit'  THEN 'OInt'  "
            + " WHEN 'datetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  "
            + " WHEN 'binary'  THEN 'OBinary'  WHEN 'image'  THEN 'OBinary'  WHEN 'timestamp'  THEN 'OBinary'    WHEN 'varbinary'  THEN 'OBinary' "
            + " ELSE  STYPE.name  END AS ODA_DATATYPE ,"
            + " SCOL.length AS LENGTH, 'INPUT' AS DIRECTION "
            + " FROM sysobjects SOBJ,syscolumns SCOL,systypes STYPE "
            + " WHERE  SOBJ.type  = 'V' "
            + " AND SOBJ.name <>'sysquerymetrics'"
            + " AND SOBJ.id = SCOL.id"
            + " AND STYPE.usertype = SCOL.usertype"
            + " ORDER BY  TABLE_NAME , COLUMN_NAME ";
            DataTable Dt = Select(sql_view, null);
            Dt.TableName = "VIEW_COLUMN";
            return Dt;
        }

        public override string[] GetPrimarykey(string TableName)
        {
            return null;
        }
        public override DatabaseColumnInfo ODAColumnToOrigin(string Name, string ColumnType, decimal Length)
        {
            DatabaseColumnInfo ColInof = new DatabaseColumnInfo();
            ColInof.Name = Name;
            ColInof.NoLength = false;
            ColInof.Length = Length;

            if (ColumnType.Trim() == ODAdbType.OBinary.ToString())
            {
                ColInof.ColumnType = "IMAGE";
                ColInof.NoLength = true;
            }
            else if (ColumnType.Trim() == ODAdbType.ODatetime.ToString())
            {
                ColInof.ColumnType = "DATETIME";
                ColInof.NoLength = true;
            }
            else if (ColumnType.Trim() == ODAdbType.ODecimal.ToString())
            {
                ColInof.ColumnType = "DECIMAL";
                ColInof.NoLength = true;
            }
            else if (ColumnType.Trim() == ODAdbType.OInt.ToString())
            {
                ColInof.ColumnType = "INT";
                ColInof.NoLength = true;
            }
            else if (ColumnType.Trim() == ODAdbType.OChar.ToString())
            {
                ColInof.ColumnType = "CHAR";
            }
            else if (ColumnType.Trim() == ODAdbType.OVarchar.ToString())
            {
                ColInof.ColumnType = "VARCHAR2";
            }
            else
            {
                ColInof.ColumnType = "VARCHAR2";
            }
            return ColInof;
        }

        //public override DataTable Select(string TransactionID, string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        //{
        //    string BlockStr = SQL + " limit " + StartIndex.ToString() + "," + MaxRecord.ToString();
        //    return Select(TransactionID, BlockStr, ParamList);
        //}

        public override object GetExpressResult(string ExpressionString)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                string sql = " SELECT" + ExpressionString + " AS VALUE  FROM SEQUENCE_TABLE WHERE SEQUENCE_NAME = 'DUAL'";
                Cmd.CommandText = sql;
                Cmd.CommandType = CommandType.Text;
                return Cmd.ExecuteScalar();
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }

        protected override void SetCmdParameters(ref IDbCommand Cmd,string SQL, params ODAParameter[] ParamList)
        {
            string dbSql = SQL;
            if (ParamList != null)
            {
                foreach (ODAParameter pr in ParamList)
                {
                    dbSql = dbSql.Replace(pr.ParamsName, pr.ParamsName.Replace(ODAParameter.ODAParamsMark, DbASybase.ParamsMark));
                    AseParameter param = new AseParameter();
                    param.ParameterName = pr.ParamsName;
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            param.AseDbType = AseDbType.DateTime;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue.ToString().Trim() == "")
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = pr.ParamsValue;
                                }
                            }
                            break;
                        case ODAdbType.ODecimal:
                            param.AseDbType = AseDbType.Decimal;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue.ToString().Trim() == "")
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = pr.ParamsValue;
                                }
                            }
                            break;
                        case ODAdbType.OBinary:
                            param.AseDbType = AseDbType.Image;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                param.Value = pr.ParamsValue;
                                if (typeof(byte[]) == pr.ParamsValue.GetType())
                                {
                                    param.Size = ((byte[])pr.ParamsValue).Length;
                                }
                            }
                            break;
                        case ODAdbType.OInt:
                            param.AseDbType = AseDbType.Integer;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue.ToString().Trim() == "")
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = pr.ParamsValue;
                                }
                            }
                            break;
                        case ODAdbType.OChar:
                            param.AseDbType = AseDbType.UniChar;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue.ToString().Trim() == "")
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = pr.ParamsValue.ToString().Trim();
                                }
                            }
                            break;
                        case ODAdbType.OVarchar:
                            param.AseDbType = AseDbType.UniVarChar;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue.ToString().Trim() == "")
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = pr.ParamsValue.ToString().Trim();
                                }
                            }
                            break;
                        default:
                            param.AseDbType = AseDbType.VarChar;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((AseParameterCollection)Cmd.Parameters).Add(param);
                }
            }
            Cmd.CommandText = dbSql;
        }
    }
}
