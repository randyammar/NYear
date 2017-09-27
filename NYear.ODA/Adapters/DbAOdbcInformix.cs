using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;

namespace NYear.ODA.Adapter
{
    public class DbAOdbcInformix : DBAccess
    {
        private static char DBParamsMark { get { return '?'; } }
        public DbAOdbcInformix(string ConnectionString)
            : base(ConnectionString)
        {

        }
        public override char ParamsMark
        {
            get { return DbAOdbcInformix.DBParamsMark; }
        }

        private OdbcConnection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new OdbcConnection(ConnString);
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
            return new OdbcDataAdapter((OdbcCommand)SelectCmd);
        }
        public override DateTime GetDBDateTime()
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                string sql = "SELECT TO_CHAR( CURRENT ,'%Y-%m-%d %H:%M:%S')  FROM  SYSTABLES WHERE TABID = 1";
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
            DataTable dt_table = Select("SELECT TABNAME AS TABLE_NAME,TABID FROM SYSTABLES WHERE TABID>99 AND TABTYPE='T' ORDER BY TABNAME ", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            DataTable dt_table = Select("SELECT TABNAME AS VIEW_NAME,TABID FROM SYSTABLES WHERE TABID>99 AND TABTYPE='V' ORDER BY TABNAME", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["VIEW_NAME"].ToString();
            }
            return str;
        }
        public override DbAType DBAType { get { return DbAType.OdbcInformix; } }

        public override DataTable GetTableColumns()
        {
            string sql_tabcol = "SELECT T1.TABNAME AS TABLE_NAME,C1.COLNAME AS COLUMN_NAME , "
            + " DECODE(C1.COLTYPE,0,'OChar',1,'OInt',2,'OInt',3,'ODecimal',4,'ODecimal',5,'ODecimal',6, "
            + " 'OInt',7,'ODatetime',8,'ODecimal',10,'ODatetime',11,'OInt',12,'OVarchar',13,'OVarchar',14, "
            + " 'ODecimal',15,'OChar',16,'OVarchar',17,'ODecimal',256,'OChar',257,'OInt',258,'OInt',259,"
            + " 'ODecimal',260,'ODecimal',261,'ODecimal',162,'OInt',263,'ODatetime',264,'ODecimal',266,'ODatetime',267,"
            + " 'OInt',268,'OVarchar',269,'OVarchar',270,'ODecimal',271,'OChar',272,'OVarchar',273,'ODecimal') AS ODA_DATATYPE,"
            + " C1.COLLENGTH AS LENGTH , 'INPUT' AS DIRECTION "
            + " FROM  SYSCOLUMNS  C1,SYSTABLES T1 "
            + " WHERE C1.TABID=T1.TABID  "
            + " AND T1.TABTYPE='T' "
            + " ORDER BY T1.TABNAME,C1.COLNO ";
            DataTable Dt = Select(sql_tabcol, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            string sql_view = "SELECT T1.TABNAME AS TABLE_NAME,C1.COLNAME AS COLUMN_NAME , "
            + " DECODE(C1.COLTYPE,0,'OChar',1,'OInt',2,'OInt',3,'ODecimal',4,'ODecimal',5,'ODecimal',6, "
            + " 'OInt',7,'ODatetime',8,'ODecimal',10,'ODatetime',11,'OInt',12,'OVarchar',13,'OVarchar',14, "
            + " 'ODecimal',15,'OChar',16,'OVarchar',17,'ODecimal',256,'OChar',257,'OInt',258,'OInt',259,"
            + " 'ODecimal',260,'ODecimal',261,'ODecimal',162,'OInt',263,'ODatetime',264,'ODecimal',266,'ODatetime',267,"
            + " 'OInt',268,'OVarchar',269,'OVarchar',270,'ODecimal',271,'OChar',272,'OVarchar',273,'ODecimal') AS ODA_DATATYPE,"
            + " C1.COLLENGTH AS LENGTH , 'INPUT' AS DIRECTION "
            + " FROM  SYSCOLUMNS  C1,SYSTABLES T1 "
            + " WHERE C1.TABID=T1.TABID  "
            + " AND T1.TABTYPE='V' "
            + " ORDER BY T1.TABNAME,C1.COLNO ";
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
                ColInof.ColumnType = "BLOB";
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
                ColInof.ColumnType = "VARCHAR";
            }
            else
            {
                ColInof.ColumnType = "VARCHAR";
            }
            return ColInof;
        }

        public override DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        {
            string BlockStr = "SELECT SKIP " + StartIndex.ToString() + " FIRST " + MaxRecord.ToString() + " "; ////取出MaxRecord记录
            BlockStr += SQL.Trim().Substring(6);
            return Select(BlockStr, ParamList);
        }

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
                    dbSql = dbSql.Replace(pr.ParamsName, pr.ParamsName.Replace(ODAParameter.ODAParamsMark, DbAOdbcInformix.DBParamsMark));

                    OdbcParameter param = new OdbcParameter();
                    param.ParameterName = pr.ParamsName;
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            param.OdbcType = OdbcType.DateTime;
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
                            param.OdbcType = OdbcType.Decimal;
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
                            param.OdbcType = OdbcType.Binary;
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
                            param.OdbcType = OdbcType.Int;
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
                            param.OdbcType = OdbcType.Char;
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
                            param.OdbcType = OdbcType.VarChar;
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
                            param.OdbcType = OdbcType.VarChar;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((OdbcParameterCollection)Cmd.Parameters).Add(param);
                }
            }

            Cmd.CommandText = dbSql;
        }
    }
}