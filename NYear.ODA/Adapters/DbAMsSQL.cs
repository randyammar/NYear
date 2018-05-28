using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace NYear.ODA.Adapter
{
    public class DbAMsSQL : DBAccess
    {
        public DbAMsSQL(string ConnectionString)
            : base(ConnectionString)
        {
        }
        private SqlConnection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new SqlConnection(ConnString);
            if (_DBConn.State == ConnectionState.Closed)
                _DBConn.Open();
            return _DBConn;
        }
        protected override DbDataAdapter GetDataAdapter(IDbCommand SelectCmd)
        {
            return new SqlDataAdapter((SqlCommand)SelectCmd);
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
        public override DbAType DBAType { get { return DbAType.MsSQL; } }
        public override string[] GetUserTables()
        {
            DataTable dt_table = this.Select("SELECT T.NAME TABLE_NAME FROM SYSOBJECTS T WHERE  TYPE='U' ORDER BY TABLE_NAME  ", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            DataTable dt_table = this.Select("SELECT V.NAME VIEW_NAME FROM SYSOBJECTS  V WHERE V.TYPE ='V' ORDER BY  VIEW_NAME", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["VIEW_NAME"].ToString();
            }
            return str;
        }
        public override DataTable GetTableColumns()
        {
            StringBuilder sql_tabcol = new StringBuilder().Append( "SELECT DISTINCT SOBJ.NAME AS TABLE_NAME, SCOL.NAME AS COLUMN_NAME , SCOL.COLID AS COL_SEQ,")
            .Append(" CASE SYT.NAME  WHEN 'sysname'  THEN 'OVarchar' WHEN 'sql_varint'  THEN 'OVarchar'    WHEN 'varchar' THEN 'OVarchar' WHEN 'char'  THEN 'OChar' ")
            .Append(" WHEN 'nchar'  THEN 'OChar' WHEN 'ntext'  THEN 'OVarchar'  WHEN 'nvarchar'  THEN 'OVarchar'  WHEN 'text'  THEN 'OVarchar' WHEN 'varchar'  THEN 'OVarchar' ")
            .Append(" WHEN 'bigint'  THEN 'ODecimal'  WHEN 'decimal'  THEN 'ODecimal'   WHEN 'float'  THEN 'ODecimal'   WHEN 'money'  THEN 'ODecimal' ")
            .Append(" WHEN 'numeric'  THEN 'ODecimal'    WHEN 'real'  THEN 'ODecimal'   WHEN 'smallmoney'  THEN 'ODecimal' ")
            .Append(" WHEN 'int'  THEN 'OInt'  WHEN 'smallint'  THEN 'OInt'  WHEN 'bit'  THEN 'OInt'  ")
            .Append(" WHEN 'datetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'date'  THEN 'ODatetime' ")
            .Append(" WHEN 'binary'  THEN 'OBinary'  WHEN 'image'  THEN 'OBinary'  WHEN 'timestamp'  THEN 'OBinary'    WHEN 'varbinary'  THEN 'OBinary' ")
            .Append(" ELSE  SYT.NAME  END AS ODA_DATATYPE ,")
            .Append(" CASE SCOL.ISNULLABLE WHEN 0 THEN 'Y' ELSE 'N' END AS NOT_NULL,")
            .Append(" SCOL.LENGTH AS LENGTH, ext.value AS DIRECTION  ")
            .Append(" FROM SYSOBJECTS SOBJ ")
            .Append(" inner join SYSCOLUMNS SCOL ")
            .Append(" on SOBJ.ID = SCOL.ID  ")
            .Append(" inner join SYS.TYPES SYT ")
            .Append(" on SYT.IS_USER_DEFINED = 0 ")
            .Append(" AND SYT.SYSTEM_TYPE_ID = SCOL.XTYPE ")
            .Append(" left join sys.extended_properties ext ")
            .Append(" on ext.name= 'MS_Description' ")
            .Append(" and ext.major_id =OBJECT_ID (UPPER(SOBJ.NAME)) ")
            .Append(" and ext.minor_id = SCOL.colid ")
            .Append(" WHERE  SOBJ.XTYPE  = 'U '")
            .Append(" ORDER BY  TABLE_NAME , SCOL.COLID  ");
            DataTable Dt = this.Select(sql_tabcol.ToString(), null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            StringBuilder sql_view = new StringBuilder().Append("SELECT DISTINCT SOBJ.NAME AS TABLE_NAME, SCOL.NAME AS COLUMN_NAME , ")
            .Append(" case SCOL.isnullable when 0 then 'False' else 'True' end as REQUIRE,")
            .Append(" CASE SYT.NAME  WHEN 'sysname'  THEN 'OVarchar' WHEN 'sql_varint'  THEN 'OVarchar'    WHEN 'varchar' THEN 'OVarchar' WHEN 'char'  THEN 'OChar' ")
            .Append(" WHEN 'nchar'  THEN 'OChar' WHEN 'ntext'  THEN 'OVarchar'  WHEN 'nvarchar'  THEN 'OVarchar'  WHEN 'text'  THEN 'OVarchar' WHEN 'varchar'  THEN 'OVarchar' ")
            .Append(" WHEN 'bigint'  THEN 'ODecimal'  WHEN 'decimal'  THEN 'ODecimal'   WHEN 'float'  THEN 'ODecimal'   WHEN 'money'  THEN 'ODecimal' ")
            .Append(" WHEN 'numeric'  THEN 'ODecimal'    WHEN 'real'  THEN 'ODecimal'   WHEN 'smallmoney'  THEN 'ODecimal' ")
            .Append(" WHEN 'int'  THEN 'OInt'  WHEN 'smallint'  THEN 'OInt'  WHEN 'bit'  THEN 'OInt'  ")
            .Append(" WHEN 'datetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'date'  THEN 'ODatetime' ")
            .Append(" WHEN 'binary'  THEN 'OBinary'  WHEN 'image'  THEN 'OBinary'  WHEN 'timestamp'  THEN 'OBinary'    WHEN 'varbinary'  THEN 'OBinary' ")
            .Append(" ELSE  SYT.NAME  END AS ODA_DATATYPE ,")
            .Append(" SCOL.LENGTH AS LENGTH, ext.value AS DIRECTION  ")
            .Append(" FROM SYSOBJECTS SOBJ ")
            .Append(" inner join SYSCOLUMNS SCOL ")
            .Append(" on SOBJ.ID = SCOL.ID  ")
            .Append(" inner join SYS.TYPES SYT ")
            .Append(" on SYT.IS_USER_DEFINED = 0 ")
            .Append(" AND SYT.SYSTEM_TYPE_ID = SCOL.XTYPE ")
            .Append(" left join sys.extended_properties ext ")
            .Append(" on ext.name= 'MS_Description' ")
            .Append(" and ext.major_id = OBJECT_ID (UPPER(SOBJ.NAME)) ")
            .Append(" and ext.minor_id = SCOL.colid ")
            .Append(" WHERE  SOBJ.XTYPE  = 'V '")
            .Append(" ORDER BY  TABLE_NAME , COLUMN_NAME  ");
            DataTable Dt = this.Select(sql_view.ToString(), null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }


        public override string[] GetPrimarykey(string TableName)
        {
            string PrimaryCols = new StringBuilder().Append("SELECT B.COLUMN_NAME ")
            .Append(" FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS A ")
            .Append(" INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE B ")
            .Append(" ON A.CONSTRAINT_NAME = B.CONSTRAINT_NAME ")
            .Append(" WHERE A.CONSTRAINT_TYPE = 'PRIMARY KEY'")
            .Append(" AND A.TABLE_NAME ='").Append(TableName).Append("'").ToString();
            DataTable Dt = this.Select(PrimaryCols, null);
            if (Dt != null && Dt.Rows.Count > 0)
            {
                List<string> cols = new List<string>();
                for (int i = 0; i < Dt.Rows.Count; i++)
                    cols.Add(Dt.Rows[i]["COLUMN_NAME"].ToString());
                return cols.ToArray();
            }
            return null;
        }

        public override string[] GetUserProcedure()
        {
            DataTable dt_table = Select("SELECT name as PROCEDURE_NAME FROM sys.objects o WHERE   o.type = 'P'", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["PROCEDURE_NAME"].ToString();
            }
            return str;
        }

        public override DatabaseColumnInfo ODAColumnToOrigin(string Name, string ColumnType, decimal Length)
        {
            DatabaseColumnInfo ColInof = new DatabaseColumnInfo();
            ColInof.Name = "[" + Name + "]";
            ColInof.NoLength = false;
            ColInof.Length = Length > 8000 ? 8000 : Length < 0 ? 8000 : Length;

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
                ColInof.ColumnType = "NVARCHAR";
            }
            else
            {
                ColInof.ColumnType = "VARCHAR2";
            }
            return ColInof;
        }

        public override DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord, string Orderby)
        { 
            int sidx = SQL.IndexOf("SELECT ", 0, StringComparison.InvariantCultureIgnoreCase);
            int distinct = SQL.IndexOf(" DISTINCT ", 0, StringComparison.InvariantCultureIgnoreCase); 
            SQL = SQL.Remove(sidx, "SELECT ".Length); 

            if (string.IsNullOrWhiteSpace(Orderby))
            {
                if ((distinct < 0 || distinct > "SELECT * FROM ".Length))
                {
                    SQL = SQL.Insert(sidx, "SELECT row_number() over(order by GETDATE()) AS R_ID_1, ");
                }
                else
                {
                    SQL = new StringBuilder().Append("SELECT row_number() over(order by GETDATE()) AS R_ID_1,VT_1_1.* FROM ( SELECT ")
                        .Append(SQL)
                        .Append(") VT_1_1").ToString();
                }               
            }
            else
            {
                SQL = SQL.Replace(Orderby, "");
                if ((distinct < 0 || distinct > "SELECT * FROM ".Length))
                {
                    SQL = SQL.Insert(sidx, "SELECT ROW_NUMBER() OVER(" + Orderby + ") AS R_ID_1, ");
                }
                else
                {
                    SQL = new StringBuilder().Append("SELECT row_number() over(")
                     .Append(Orderby)
                     .Append(") AS R_ID_1,VT_1_1.* FROM ( SELECT ")
                     .Append(SQL)
                     .Append(") VT_1_1").ToString();
                }
            }
            DataTable dt = Select("SELECT A_B_1.* FROM ( " + SQL + " ) AS A_B_1 WHERE A_B_1.R_ID_1 > " + StartIndex.ToString() + " AND A_B_1.R_ID_1 <= " + (StartIndex + MaxRecord).ToString(), ParamList); 
            dt.Columns.Remove("R_ID_1");
            return dt; 
        }
        public override List<T> Select<T>(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord, string Orderby)
        {

            int sidx = SQL.IndexOf("SELECT ", 0, StringComparison.InvariantCultureIgnoreCase);
            int distinct = SQL.IndexOf(" DISTINCT ", 0, StringComparison.InvariantCultureIgnoreCase);
            SQL = SQL.Remove(sidx, "SELECT ".Length);

            if (string.IsNullOrWhiteSpace(Orderby))
            {
                if ((distinct < 0 || distinct > "SELECT * FROM ".Length))
                {
                    SQL = SQL.Insert(sidx, "SELECT row_number() over(order by GETDATE()) AS R_ID_1, ");
                }
                else
                {
                    SQL = new StringBuilder().Append("SELECT row_number() over(order by GETDATE()) AS R_ID_1,VT_1_1.* FROM ( SELECT ")
                        .Append(SQL)
                        .Append(") VT_1_1").ToString();
                }
            }
            else
            {
                SQL = SQL.Replace(Orderby, "");
                if ((distinct < 0 || distinct > "SELECT * FROM ".Length))
                {
                    SQL = SQL.Insert(sidx, "SELECT ROW_NUMBER() OVER(" + Orderby + ") AS R_ID_1, ");
                }
                else
                {
                    SQL = new StringBuilder().Append("SELECT row_number() over(")
                     .Append(Orderby)
                     .Append(") AS R_ID_1,VT_1_1.* FROM ( SELECT ")
                     .Append(SQL)
                     .Append(") VT_1_1").ToString();
                }
            }
            return Select<T>("SELECT A_B_1.* FROM ( " + SQL + " ) AS A_B_1 WHERE A_B_1.R_ID_1 > " + StartIndex.ToString() + " AND A_B_1.R_ID_1 <= " + (StartIndex + MaxRecord).ToString(), ParamList);
        }

        public override bool Import(string DbTable, ODAParameter[] prms, DataTable FormTable)
        {
            SqlBulkCopy sqlbulkcopy = null;
            IDbConnection conn = null;
            try
            {
                if (this.Transaction == null)
                {
                    conn = this.GetConnection();
                    sqlbulkcopy = new SqlBulkCopy((SqlConnection)conn);
                }
                else
                {
                    sqlbulkcopy = new SqlBulkCopy((SqlConnection)this.Transaction.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)this.Transaction);
                }
                for (int i = 0; i < prms.Length; i++)
                {
                    if (FormTable.Columns.Contains(prms[i].ParamsName))
                    {
                        SqlBulkCopyColumnMapping colMap = new SqlBulkCopyColumnMapping(FormTable.Columns[i].ColumnName, prms[i].ParamsName);
                        sqlbulkcopy.ColumnMappings.Add(colMap);
                    }
                }
                //需要操作的数据库表名  
                sqlbulkcopy.DestinationTableName = DbTable;
                //将内存表表写入  
                sqlbulkcopy.WriteToServer(FormTable);
                return true;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
                if (sqlbulkcopy != null)
                {
                    sqlbulkcopy.Close();
                    sqlbulkcopy = null;
                }
            }
        }
        public override object GetExpressResult(string ExpressionString)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                string sql = " SELECT" + ExpressionString + " AS VALUE FROM SEQUENCE_TABLE WHERE SEQUENCE_NAME = 'DUAL'";
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
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = pr.ParamsName;
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            param.SqlDbType = SqlDbType.DateTime;
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
                                    if (pr.ParamsValue is DateTime || pr.ParamsValue is DateTime?)
                                    {
                                        param.Value = pr.ParamsValue;
                                    }
                                    else if (string.IsNullOrWhiteSpace(pr.ParamsValue.ToString().Trim()))
                                    {
                                        param.Value = System.DBNull.Value;
                                    }
                                    else
                                    {
                                        param.Value = Convert.ToDateTime(pr.ParamsValue);
                                    }
                                }
                            }
                            break;
                        case ODAdbType.ODecimal:
                            param.SqlDbType = SqlDbType.Decimal;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue is decimal || pr.ParamsValue is decimal?)
                                {
                                    param.Value = pr.ParamsValue;
                                }
                                else if (string.IsNullOrWhiteSpace(pr.ParamsValue.ToString().Trim()))
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = Convert.ToDecimal(pr.ParamsValue);
                                }
                            }
                            break;
                        case ODAdbType.OBinary:
                            param.SqlDbType = SqlDbType.Image;
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
                                    throw new ODAException(201, "Params :" + pr.ParamsName + " Type must be byte[]");
                                }
                            }
                            break;
                        case ODAdbType.OInt:
                            param.SqlDbType = SqlDbType.Int;
                            if (pr.ParamsValue == null || pr.ParamsValue == System.DBNull.Value)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                if (pr.ParamsValue is int || pr.ParamsValue is int?)
                                {
                                    param.Value = pr.ParamsValue;
                                }
                                else if (string.IsNullOrWhiteSpace(pr.ParamsValue.ToString().Trim()))
                                {
                                    param.Value = System.DBNull.Value;
                                }
                                else
                                {
                                    param.Value = Convert.ToInt32(pr.ParamsValue);
                                }
                            }
                            break;
                        case ODAdbType.OChar:
                            param.SqlDbType = SqlDbType.Char;
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
                            param.SqlDbType = SqlDbType.VarChar;
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
                            param.SqlDbType = SqlDbType.VarChar;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((SqlParameterCollection)Cmd.Parameters).Add(param);
                }
            }
        }
    }
}
