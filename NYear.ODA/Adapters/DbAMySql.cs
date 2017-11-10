using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;

namespace NYear.ODA.Adapter
{
    public class DbAMySql : DBAccess
    {
        public DbAMySql(string ConnectionString)
            : base(ConnectionString)
        {
        }
        private MySqlConnection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new MySqlConnection(ConnString);
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
            return new MySqlDataAdapter((MySqlCommand)SelectCmd);
        }
        public override DateTime GetDBDateTime()
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                string sql = "SELECT sysdate() AS DB_DATETIME ";
                Cmd.CommandText = sql;
                Cmd.CommandType = CommandType.Text;
                return Convert.ToDateTime(Cmd.ExecuteScalar());
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        public override DbAType DBAType { get { return DbAType.MySql; } }
        public override string[] GetUserTables()
        {
            DataTable dt_table = Select("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES "
                + " WHERE TABLE_NAME NOT IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE  TABLE_SCHEMA =DATABASE()) "
                + " AND TABLE_SCHEMA =DATABASE() ", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            DataTable dt_table = Select("SELECT TABLE_NAME VIEW_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA =DATABASE()", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["VIEW_NAME"].ToString();
            }
            return str;
        }

        public override DataTable GetTableColumns()
        {
            string sql_tabcol = "SELECT C.TABLE_NAME,C.COLUMN_NAME, C.ORDINAL_POSITION COL_SEQ, "
            + " CASE C.DATA_TYPE WHEN 'char' THEN 'OChar' WHEN 'varchar' THEN 'OVarchar' WHEN 'tinytext' THEN 'OVarchar' "
            + " WHEN 'text' THEN 'OVarchar' WHEN 'mediumtext' THEN 'OVarchar' WHEN 'longtext' THEN 'OVarchar' WHEN 'enum' THEN 'OChar' "
            + " WHEN 'set' THEN 'OVarchar' WHEN 'geometry' THEN 'OVarchar' WHEN 'point' THEN 'OVarchar' WHEN 'linestring' THEN 'OVarchar' "
            + " WHEN 'polygon' THEN 'OVarchar' WHEN 'multipoint' THEN 'OVarchar' WHEN 'multilinestring' THEN 'OVarchar'  "
            + " WHEN 'multipolygon' THEN 'OVarchar' WHEN 'geometrycollection' THEN 'OVarchar' "
            + " WHEN 'int' THEN 'OInt' WHEN 'tynyint' THEN 'OInt' WHEN 'smallint' THEN 'OInt' WHEN 'mediumint' THEN 'OInt'"
            + " WHEN 'bigint' THEN 'ODecimal' WHEN 'real' THEN 'ODecimal'  WHEN 'double' THEN 'ODecimal' WHEN 'float' THEN 'ODecimal' "
            + " WHEN 'numeric' THEN 'ODecimal' WHEN 'decimal' THEN 'ODecimal' "
            + " WHEN 'binary' THEN 'OBinary' WHEN 'varbinary' THEN 'OBinary' WHEN 'blob' THEN 'OBinary' WHEN 'mediumblob' THEN 'OBinary' WHEN 'longblob' THEN 'OBinary' "
            + " WHEN 'date' THEN 'ODatetime' WHEN 'year' THEN 'ODatetime' WHEN 'time' THEN 'ODatetime' WHEN 'timestamp' THEN 'ODatetime' "
            + " WHEN 'datetime' THEN 'ODatetime' "
            + " END AS ODA_DATATYPE ,CASE C.IS_NULLABLE WHEN 'NO' THEN 'Y' ELSE 'N' END AS NOT_NULL, "
            + " CASE WHEN C.CHARACTER_MAXIMUM_LENGTH IS NULL THEN 9 ELSE  CASE WHEN  C.CHARACTER_MAXIMUM_LENGTH > 65534 THEN 0 ELSE  C.CHARACTER_MAXIMUM_LENGTH END END LENGTH,'INPUT' DIRECTION  "
            + " FROM INFORMATION_SCHEMA.COLUMNS C "
            + " WHERE C.TABLE_NAME NOT IN (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS V WHERE V.TABLE_SCHEMA  = DATABASE() AND  C.TABLE_SCHEMA =DATABASE()) "
            + " AND C.TABLE_SCHEMA =DATABASE() "
            + " ORDER BY C.TABLE_NAME, C.ORDINAL_POSITION ";
            DataTable Dt = Select(sql_tabcol, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            string sql_view = "SELECT C.TABLE_NAME,C.COLUMN_NAME, CASE C.DATA_TYPE "
            + " WHEN 'char' THEN 'OChar' WHEN 'varchar' THEN 'OVarchar' WHEN 'tinytext' THEN 'OVarchar' "
            + " WHEN 'text' THEN 'OVarchar' WHEN 'mediumtext' THEN 'OVarchar' WHEN 'longtext' THEN 'OVarchar' WHEN 'enum' THEN 'OChar' "
            + " WHEN 'set' THEN 'OVarchar' WHEN 'geometry' THEN 'OVarchar' WHEN 'point' THEN 'OVarchar' WHEN 'linestring' THEN 'OVarchar' "
            + " WHEN 'polygon' THEN 'OVarchar' WHEN 'multipoint' THEN 'OVarchar' WHEN 'multilinestring' THEN 'OVarchar'  "
            + " WHEN 'multipolygon' THEN 'OVarchar' WHEN 'geometrycollection' THEN 'OVarchar' "
            + " WHEN 'int' THEN 'OInt' WHEN 'tynyint' THEN 'OInt' WHEN 'smallint' THEN 'OInt' WHEN 'mediumint' THEN 'OInt' "
            + " WHEN 'bigint' THEN 'ODecimal' WHEN 'real' THEN 'ODecimal'  WHEN 'double' THEN 'ODecimal' WHEN 'float' THEN 'ODecimal' "
            + " WHEN 'numeric' THEN 'ODecimal' WHEN 'decimal' THEN 'ODecimal' "
            + " WHEN 'binary' THEN 'OBinary' WHEN 'varbinary' THEN 'OBinary' WHEN 'blob' THEN 'OBinary' WHEN 'mediumblob' THEN 'OBinary' WHEN 'longblob' THEN 'OBinary' "
            + " WHEN 'date' THEN 'ODatetime' WHEN 'year' THEN 'ODatetime' WHEN 'time' THEN 'ODatetime' WHEN 'timestamp' THEN 'ODatetime' "
            + " WHEN 'datetime' THEN 'ODatetime' "
            + " END AS ODA_DATATYPE ,CASE C.IS_NULLABLE WHEN 'NO' THEN 'Y' ELSE 'N' END AS NOT_NULL, "
            + " CASE WHEN C.CHARACTER_MAXIMUM_LENGTH IS NULL THEN 9 ELSE  CASE WHEN  C.CHARACTER_MAXIMUM_LENGTH > 65534 THEN 0 ELSE  C.CHARACTER_MAXIMUM_LENGTH END  END LENGTH,'INPUT' DIRECTION  "
            + " FROM INFORMATION_SCHEMA.COLUMNS C, INFORMATION_SCHEMA.VIEWS V "
            + " WHERE C.TABLE_NAME = V.TABLE_NAME "
            + " AND C.TABLE_SCHEMA =DATABASE() "
            + " AND V.TABLE_SCHEMA  = DATABASE() "
            + " ORDER BY C.TABLE_NAME,C.COLUMN_NAME ";
            DataTable Dt = Select(sql_view, null);
            Dt.TableName = "VIEW_COLUMN";
            return Dt;
        }

        public override string[] GetPrimarykey(string TableName)
        {
            string PrimaryCols = string.Format("SELECT  CU.COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU,INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC "
            + " WHERE  CU.TABLE_NAME = TC.TABLE_NAME AND  TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND CU.TABLE_NAME ='{0}'", TableName);
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

        public override DatabaseColumnInfo ODAColumnToOrigin(string Name, string ColumnType, decimal Length)
        {
            DatabaseColumnInfo ColInof = new DatabaseColumnInfo();
            ColInof.Name = Name;
            ColInof.NoLength = false;
            ColInof.Length = Length > 2000 ? 2000 : Length < 0 ? 2000 : Length;

            if (ColumnType.Trim() == ODAdbType.OBinary.ToString())
            {
                ColInof.ColumnType = "LongBlob";
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
                ColInof.ColumnType = "VARCHAR2";
            }
            return ColInof;
        }

        public override DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        {
            string BlockStr = SQL + " limit " + StartIndex.ToString() + "," + MaxRecord.ToString(); ///取出MaxRecord条记录
            return Select(BlockStr, ParamList);
        }
        public override bool Import(string DbTable, ODAParameter[] prms, DataTable FormTable)
        {
            int ImportCount = 0;
            MySqlConnection conn = null;
            string tmpPath = Path.GetTempFileName();
            try
            {
                MySqlBulkLoader bulk = null;
                string csv = DataTableToCsv(FormTable, prms.Length);
                File.WriteAllText(tmpPath, csv);
                if (this.Transaction == null)
                {
                    bulk = new MySqlBulkLoader((MySqlConnection)this.Transaction.Connection);
                }
                else
                {
                    conn = (MySqlConnection)this.GetConnection();
                    bulk = new MySqlBulkLoader((MySqlConnection)conn);
                }
                bulk.FieldTerminator = ",";
                bulk.FieldQuotationCharacter = '"';
                bulk.EscapeCharacter = '"';
                bulk.LineTerminator = "\r\n";
                bulk.FileName = tmpPath;
                bulk.NumberOfLinesToSkip = 0;
                bulk.TableName = DbTable;
                for (int i = 0; i < prms.Length; i++)
                    bulk.Columns.Add(prms[i].ParamsName);
                ImportCount = bulk.Load();
                return ImportCount > 0;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
                if (File.Exists(tmpPath))
                    File.Delete(tmpPath);
            }
        }
        /// <summary>
        ///将DataTable转换为标准的CSV
        /// </summary>
        /// <param name="table">数据表</param>
        /// <returns>返回标准的CSV</returns>
        private static string DataTableToCsv(DataTable table, int ColIdx)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i <= ColIdx; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
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
                    MySqlParameter param = new MySqlParameter();
                    param.ParameterName = pr.ParamsName;
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            param.MySqlDbType = MySqlDbType.DateTime;
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
                            param.MySqlDbType = MySqlDbType.Decimal;
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
                            param.MySqlDbType = MySqlDbType.Blob;
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
                            param.MySqlDbType = MySqlDbType.Int32;
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
                            param.MySqlDbType = MySqlDbType.VarChar;
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
                            param.MySqlDbType = MySqlDbType.VarChar;
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
                            param.MySqlDbType = MySqlDbType.VarChar;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((MySqlParameterCollection)Cmd.Parameters).Add(param);
                }
            }
        }
    }
}
