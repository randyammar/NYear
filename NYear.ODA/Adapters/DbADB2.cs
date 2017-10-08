using IBM.Data.DB2;
using System;
using System.Data;
using System.Data.Common;

namespace NYear.ODA.Adapter
{
    public class DbADB2 : DBAccess
    {
        public DbADB2(string ConnectionString)
            : base(ConnectionString)
        {
        }
        public override DbAType DBAType { get { return DbAType.DB2; } }

        private DB2Connection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new DB2Connection(ConnString);
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
            return new DB2DataAdapter((DB2Command)SelectCmd);
        }
        public override DateTime GetDBDateTime()
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandText = "SELECT TO_CHAR(CURRENT  TIMESTAMP,'YYYY-MM-DD HH24:MI:SS')  DB_DATETIME  FROM SYSIBM.DUAL ";
                Cmd.CommandType = CommandType.Text;
                return Convert.ToDateTime(((DB2Command)Cmd).ExecuteScalar());
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        public override string[] GetUserTables()
        {
            string sql = "SELECT T.NAME TABLE_NAME FROM SYSIBM.SYSTABLES T WHERE T.TYPE='T' AND T.TBSPACE='USERSPACE1'";
            DataTable dt_table = Select(sql, null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            string sql = "SELECT T.NAME TABLE_NAME FROM SYSIBM.SYSVIEWS T WHERE T.DEFINERTYPE = 'U'";
            DataTable dt_table = Select(sql, null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override DataTable GetTableColumns()
        {
            string sql = @"SELECTT.NAME  TABLE_NAME,C.NAME AS COLUMN_NAME,C.COLNO COL_SEQ,
CASE C.COLTYPE WHEN 'CHAR' THEN 'OChar' WHEN 'VARCHAR' THEN 'OVarchar'
WHEN 'TIMESTMP' THEN 'ODatetime' WHEN 'DECIMAL' THEN 'ODecimal' WHEN 'INTEGER' THEN 'OInt'
WHEN 'BLOB' THEN 'OBinary' ELSE  C.COLTYPE END ODA_DATATYPE,
CASE c.nulls WHEN 'N' THEN 'Y' ELSE 'N' END NOT_NULL,C.LENGTH AS LENGTH, C.REMARKS DIRECTION 
FROM SYSIBM.SYSCOLUMNS C
INNER JOIN SYSIBM.SYSTABLES T ON C.TBNAME=T.NAME
WHERE T.TBSPACE ='USERSPACE1'
AND T.TYPE='T'
ORDER BY T.NAME ,C.COLNO";
            DataTable Dt = Select(sql, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            string sql = @"SELECT C.NAME AS COLUMN_NAME,T.NAME  TABLE_NAME,
CASE C.COLTYPE WHEN 'CHAR' THEN 'OChar' WHEN 'VARCHAR' THEN 'OVarchar'
WHEN 'TIMESTMP' THEN 'ODatetime' WHEN 'DECIMAL' THEN 'ODecimal' WHEN 'INTEGER' THEN 'OInt'
WHEN 'BLOB' THEN 'OBinary' ELSE C.COLTYPE END ODA_DATATYPE,
CASE c.nulls WHEN 'N' THEN 'Y' ELSE 'N' END NOT_NULL, C.LENGTH AS LENGTH, C.REMARKS DIRECTION
FROM SYSIBM.SYSCOLUMNS C
INNER JOIN SYSIBM.SYSVIEWS T ON C.TBNAME = T.NAME
WHERE T.DEFINERTYPE = 'U'
ORDER BY T.NAME ,C.COLNO";
            DataTable Dt = this.Select(sql, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override string[] GetPrimarykey(string TableName)
        {
            string sql = @"SELECT N.TBNAME TABLE_NAME, N.COLNAMES FROM  SYSIBM.SYSTABLES D  
INNER JOIN SYSIBM.SYSINDEXES N ON N.TBNAME = D.NAME
WHERE D.TBSPACE = 'USERSPACE1'
AND D.TYPE = 'T'
AND UNIQUERULE = 'P'";

            sql += " AND N.TBNAME = '" + TableName.ToUpper() + "'";
            DataTable Dt = this.Select(sql, null);
            if (Dt != null && Dt.Rows.Count > 0)
            {
               return  Dt.Rows[0]["COLNAMES"].ToString().Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries );
            }
            return null;
        }

// sysibm.syscolumns a
//INNER JOIN sysibm.systables d on a.tbname=d.name
//LEFT JOIN sysibm.sysindexes n on n.tbname= d.name and SUBSTR(colnames,2)=a.name
        public override DatabaseColumnInfo ODAColumnToOrigin(string Name, string ColumnType, decimal Length)
        {
            DatabaseColumnInfo ColInof = new DatabaseColumnInfo();
            ColInof.Name = "\"" +Name +"\"";
            ColInof.NoLength = false;
            ColInof.Length = Length > 2000 ? 2000 : Length < 0 ? 2000 : Length;

            if (ColumnType.Trim() == ODAdbType.OBinary.ToString())
            {
                ColInof.ColumnType = "BLOB";
                ColInof.NoLength = true; 
            }
            else if (ColumnType.Trim() == ODAdbType.ODatetime.ToString())
            {
                ColInof.ColumnType = "TIMESTAMP";
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
            string BlockStr = "select* from (select row_number() over(order by current time) as r_id_1,t_1.* from ( ";
            BlockStr += SQL;
            BlockStr += ") t_1 ) t_t_1 where t_t_1.r_id_1 > " + StartIndex.ToString() + " and t_t_1.r_id_1  <= " + (StartIndex + MaxRecord).ToString();
            DataTable dt = Select(BlockStr, ParamList);
            dt.Columns.Remove("r_id_1");
            return dt;
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
        protected override void SetCmdParameters(ref IDbCommand Cmd, string SQL, params ODAParameter[] ParamList)
        {
            Cmd.CommandText = SQL;
            if (ParamList != null)
            {
                foreach (ODAParameter pr in ParamList)
                {
                    DB2Parameter param = new DB2Parameter();
                    param.ParameterName = pr.ParamsName;
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            param.DB2Type = DB2Type.Date;
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
                            param.DB2Type = DB2Type.Decimal;

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
                            param.DB2Type = DB2Type.Blob;
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
                                else
                                {
                                    throw new ODAException(11001, "Params :" + pr.ParamsName + " Type must be byte[]");
                                }
                            }
                            break;
                        case ODAdbType.OInt:
                            param.DB2Type = DB2Type.Integer;
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
                            param.DB2Type = DB2Type.Char;
                            param.DbType = DbType.StringFixedLength;
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
                            param.DB2Type = DB2Type.VarChar;
                            param.DbType = DbType.String;
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
                            param.DB2Type = DB2Type.VarChar;
                            param.DbType = DbType.String;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((DB2ParameterCollection)Cmd.Parameters).Add(param);
                }
            }
        }
    }
}

