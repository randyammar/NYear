using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace NYear.ODA.Adapter
{
    public class DbAOracle : DBAccess
    {
        private static char DBParamsMark { get { return ':'; } }
        public DbAOracle(string ConnectionString)
            : base(ConnectionString)
        {

        }
        public override char ParamsMark
        {
            get { return DbAOracle.DBParamsMark; }
        }
        private OracleConnection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new OracleConnection(ConnString);
            if (_DBConn.State == ConnectionState.Closed)
                _DBConn.Open();
            _DBConn.Disposed += _DBConn_Disposed;
            return _DBConn;
        }
        private void _DBConn_Disposed(object sender, EventArgs e)
        {
            _DBConn = null;
        }

        public override DataSet ExecuteProcedure(string SQL, ODAParameter[] ParamList)
        {
            IDbCommand Cmd = OpenCommand();
            DataSet ds_rtl = new DataSet("ReturnValues");
            DataTable dt_values = new DataTable("ValuesCollection");
            dt_values.Columns.Add("ParamName");
            dt_values.Columns.Add("ReturnValue");
            try
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                SetCmdParameters(ref Cmd, SQL, ParamList);
                Cmd.ExecuteNonQuery();
                foreach (OracleParameter opc in Cmd.Parameters)
                {
                    if (opc.Direction == System.Data.ParameterDirection.InputOutput || opc.Direction == System.Data.ParameterDirection.Output)
                    {
                        // if (opc.OracleType == System.Data.OracleClient.OracleType.Cursor)
                        if (opc.OracleDbType == OracleDbType.RefCursor)
                        {
                            DataTable dt = new DataTable(opc.ParameterName);
                            //OracleDataReader odr = (System.Data.OracleClient.OracleDataReader)opc.Value;
                            OracleDataReader odr = (OracleDataReader)opc.Value;
                            for (int num = 0; num < odr.FieldCount; num++)
                            {
                                DataColumn column = new DataColumn();
                                column.DataType = odr.GetFieldType(num);
                                column.ColumnName = odr.GetName(num);
                                dt.Columns.Add(column);
                            }
                            while (odr.Read())
                            {
                                DataRow row = dt.NewRow();
                                for (int num = 0; num < odr.FieldCount; num++)
                                {
                                    row[num] = odr[num];
                                }
                                dt.Rows.Add(row);
                            }
                            ds_rtl.Tables.Add(dt);
                            odr.Close();
                            odr.Dispose();
                        }
                        else
                        {
                            DataRow dr = dt_values.NewRow();
                            dr["ParamName"] = opc.ParameterName;
                            dr["ReturnValue"] = opc.Value;
                            dt_values.Rows.Add(dr);
                        }
                    }
                }
                ds_rtl.Tables.Add(dt_values);
                return ds_rtl;
            }
            finally
            {
                dt_values.Dispose();
                ds_rtl.Dispose();
                CloseCommand(Cmd);
            }
        }
        protected override DbDataAdapter GetDataAdapter(IDbCommand SelectCmd)
        {
            ((OracleCommand)SelectCmd).BindByName = true;
            return new OracleDataAdapter((OracleCommand)SelectCmd);
        }
        public override DateTime GetDBDateTime()
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandText = "SELECT TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') DB_DATETIME FROM DUAL ";
                Cmd.CommandType = CommandType.Text;
                // return Convert.ToDateTime(((OracleCommand)Cmd).ExecuteOracleScalar());
                return Convert.ToDateTime(((OracleCommand)Cmd).ExecuteScalar());
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        public override string[] GetUserTables()
        {
            DataTable dt_table = Select("SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            DataTable dt_table = Select("SELECT VIEW_NAME FROM USER_VIEWS ORDER BY VIEW_NAME", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["VIEW_NAME"].ToString();
            }
            return str;
        }
        /*
         *
         *
         * 存储过程 SELECT DBMS_METADATA.GET_DDL(U.OBJECT_TYPE, u.object_name)
FROM USER_OBJECTS u
where U.OBJECT_TYPE IN ('PROCEDURE'，'PACKAGE');
         
            视图
             select dbms_metadata.get_ddl('VIEW','VM_EXCEPTION_INFO') from dual;
             SELECT DBMS_METADATA.GET_DDL('VIEW',u.VIEW_name) FROM USER_VIEWS u;
             */
        public override string[] GetPrimarykey(string TableName)
        {
            string PrimaryCols = string.Format("SELECT A.COLUMN_NAME  FROM USER_CONS_COLUMNS A, USER_CONSTRAINTS B  WHERE A.CONSTRAINT_NAME = B.CONSTRAINT_NAME  AND B.CONSTRAINT_TYPE = 'P' AND A.TABLE_NAME ='{0}'", TableName);
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
        public override DbAType DBAType { get { return DbAType.Oracle; } }

        public override DatabaseColumnInfo ODAColumnToOrigin(string Name, string ColumnType, decimal Length)
        {
            DatabaseColumnInfo ColInof = new DatabaseColumnInfo();
            ColInof.Name = "\"" + Name +"\"";
            ColInof.NoLength = false;
            ColInof.Length = Length > 2000 ? 2000 : Length < 0 ? 2000 : Length;

            if (ColumnType.Trim() == ODAdbType.OBinary.ToString())
            {
                ColInof.ColumnType = "BLOB";
                ColInof.NoLength = true;
            }
            else if (ColumnType.Trim() == ODAdbType.ODatetime.ToString())
            {
                ColInof.ColumnType = "DATE";
                ColInof.NoLength = true;
            }
            else if (ColumnType.Trim() == ODAdbType.ODecimal.ToString())
            {
                ColInof.ColumnType = "NUMBER";
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
        public override string[] GetUserProcedure()
        {
            DataTable dt_table = Select("SELECT ARG.OBJECT_NAME PROCEDURE_NAME FROM USER_OBJECTS O,USER_ARGUMENTS ARG WHERE  O.OBJECT_TYPE='PROCEDURE' AND O.OBJECT_NAME = ARG.OBJECT_NAME AND ARG.PACKAGE_NAME IS NULL "
            + " UNION SELECT ARG.PACKAGE_NAME||'.'|| ARG.OBJECT_NAME PROCEDURE_NAME FROM USER_OBJECTS O,USER_ARGUMENTS ARG WHERE  O.OBJECT_TYPE='PACKAGE' AND O.OBJECT_NAME = ARG.PACKAGE_NAME ", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["PROCEDURE_NAME"].ToString();
            }
            return str;
        }

        public override DataTable GetTableColumns()
        {
            string sql_tabcol = " SELECT TC.TABLE_NAME,TC.COLUMN_NAME,CASE TC.NULLABLE WHEN 'N' THEN 'Y' ELSE 'N' END   NOTNULL,TC.COLUMN_ID COL_SEQ,"
            + " DECODE(TC.DATA_TYPE,'CHAR','OChar','VARCHAR','OVarchar','VARCHAR2','OVarchar','NVARCHAR2','OVarchar','MLSLABEL','OVarchar',"
            + " 'UROWID','OVarchar','URITYPE','OVarchar','CHARACTER','OVarchar','CLOB','OVarchar','INTEGER','OInt','INT','OInt',"
            + " 'SMALLINT','OInt','DATE','ODatetime','LONG','ODecimal','DECIMAL','ODecimal','NUMERIC','ODecimal','REAL','ODecimal',"
            + " 'NUMBER','ODecimal','BLOB','OBinary','BFILE','OBinary','OVarchar') ODA_DATATYPE, "
            + " DECODE(TC.DATA_TYPE,'BLOB',2000000000,'CLOB',2000000000, TC.DATA_LENGTH)  LENGTH,"  
            + " TCC.COMMENTS DIRECTION "
            + " FROM USER_TABLES  TB,USER_TAB_COLUMNS TC ,USER_COL_COMMENTS  TCC"
            + " WHERE TB.TABLE_NAME = TC.TABLE_NAME "
            + " AND TC.TABLE_NAME = TCC.table_name(+) "
            + " AND TC.COLUMN_NAME = TCC.column_name(+) "
            + " ORDER BY TC.TABLE_NAME,TC.COLUMN_ID ";
            DataTable Dt = Select(sql_tabcol, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            string sql_view = "SELECT TC.TABLE_NAME,TC.COLUMN_NAME,CASE TC.NULLABLE WHEN 'N' THEN 'Y' ELSE 'N' END NOTNULL,"
                + " DECODE(TC.DATA_TYPE,'CHAR','OChar','VARCHAR','OVarchar','VARCHAR2','OVarchar','NVARCHAR2','OVarchar','MLSLABEL','OVarchar','UROWID','OVarchar','URITYPE','OVarchar','CHARACTER','OVarchar','CLOB','OVarchar', "
                + " 'INTEGER','OInt','INT','OInt','SMALLINT','OInt','DATE','ODatetime','LONG','ODecimal','DECIMAL','ODecimal','NUMERIC','ODecimal','REAL','ODecimal','NUMBER','ODecimal','BLOB','OBinary','BFILE','OBinary','OVarchar') ODA_DATATYPE, "
                + " DECODE(TC.DATA_TYPE,'BLOB',-1,'CLOB',-1, TC.DATA_LENGTH)  LENGTH,TCC.COMMENTS DIRECTION "
                + " FROM USER_VIEWS TV,USER_TAB_COLUMNS TC ,'N' NOT_NULL,USER_COL_COMMENTS  TCC"
                + " WHERE TV.VIEW_NAME = TC.TABLE_NAME "
                + " AND TC.TABLE_NAME = TCC.table_name(+) "
                + " AND TC.COLUMN_NAME = TCC.column_name(+) "
                + " ORDER BY  TC.TABLE_NAME,TC.COLUMN_NAME ";
            DataTable Dt = Select(sql_view, null);
            Dt.TableName = "VIEW_COLUMN";
            return Dt;
        }
        public override DataTable GetUserProcedureArguments(string ProcedureName)
        {
            string SqlArg = " select arg.object_name PROCEDURE_NAME, arg.ARGUMENT_NAME,arg.DATA_TYPE,"
            + " DECODE(arg.DATA_TYPE,'CHAR','OChar','VARCHAR','OVarchar','VARCHAR2','OVarchar','NVARCHAR2','OVarchar','MLSLABEL','OVarchar','UROWID','OVarchar','URITYPE','OVarchar','CHARACTER','OVarchar','CLOB','OVarchar', "
            + " 'INTEGER','OInt','INT','OInt','SMALLINT','OInt','DATE','ODatetime','LONG','ODecimal','DECIMAL','ODecimal','NUMERIC','ODecimal','REAL','ODecimal','NUMBER','ODecimal','BLOB','OBinary','BFILE','OBinary','PL/SQL TABLE','OArrary','REF CURSOR','OTable','OVarchar') ODA_DATATYPE, "
            + " arg.POSITION,arg.IN_OUT DIRECTION, "
            + " NVL(DECODE(arg.DATA_TYPE,'BLOB',-1,'CLOB',-1,arg.DATA_LENGTH),-1)  LENGTH"
            + " from user_objects o,user_arguments arg"
            + " where  o.object_type='PROCEDURE' "
            + " and o.OBJECT_NAME = arg.OBJECT_NAME "
            + " and arg.PACKAGE_NAME is null"
            + " and o.OBJECT_ID = arg.OBJECT_ID"
            + " and o.OBJECT_NAME = @ProcedureName"
            + " union "
            + " select arg.package_name||'.'|| arg.object_name PROCEDURE_NAME, arg.ARGUMENT_NAME, arg.DATA_TYPE,"
            + " DECODE(arg.DATA_TYPE,'CHAR','OChar','VARCHAR','OVarchar','VARCHAR2','OVarchar','NVARCHAR2','OVarchar','MLSLABEL','OVarchar','UROWID','OVarchar','URITYPE','OVarchar','CHARACTER','OVarchar','CLOB','OVarchar', "
            + " 'INTEGER','OInt','INT','OInt','SMALLINT','OInt','DATE','ODatetime','LONG','ODecimal','DECIMAL','ODecimal','NUMERIC','ODecimal','REAL','ODecimal','NUMBER','ODecimal','BLOB','OBinary','BFILE','OBinary','PL/SQL TABLE','OArrary','REF CURSOR','OTable','OVarchar') ODA_DATATYPE, "
            + " arg.POSITION,arg.IN_OUT DIRECTION,"
            + " NVL(DECODE(arg.DATA_TYPE,'BLOB',-1,'CLOB',-1,arg.DATA_LENGTH),-1)  LENGTH"
            + " from user_objects o,user_arguments arg"
            + " where  o.object_type='PACKAGE' "
            + " and o.OBJECT_NAME = arg.package_name "
            + " and o.OBJECT_ID = arg.OBJECT_ID"
            + " and arg.package_name||'.'|| arg.object_name =@ProcedureName"
            + " ORDER BY PROCEDURE_NAME ,POSITION ";

            ODAParameter p = new ODAParameter() { DBDataType = ODAdbType.OVarchar, Direction = ParameterDirection.Input, ParamsName = "@ProcedureName", ParamsValue = ProcedureName, Size = 200 };

            DataTable Dttmp = Select(SqlArg, new ODAParameter[] { p });
            Dttmp.TableName = "PROCEDURE_ARGUMENTS";
            return Dttmp;
        }
        public override DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        {
            string BlockStr = "SELECT * FROM (SELECT ROWNUM AS R_ID_1 ,T_T_1.* FROM ( ";
            BlockStr += SQL;
            BlockStr += ") T_T_1 ) WHERE R_ID_1 > " + StartIndex.ToString() + " AND R_ID_1 <= " + (StartIndex + MaxRecord).ToString();  ///取出MaxRecord条记录
            DataTable dt = Select(BlockStr, ParamList);
            dt.Columns.Remove("R_ID_1");
            return dt;
        }

        public override object GetExpressResult(string ExpressionString)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandText = "SELECT " + ExpressionString + " FROM DUAL ";
                Cmd.CommandType = CommandType.Text;
                // return ((OracleCommand)Cmd).ExecuteOracleScalar();
                return ((OracleCommand)Cmd).ExecuteScalar();
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        public override long GetSequenceNextVal(string SequenceName)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                string sql = " SELECT " + SequenceName + ".NEXTVAL FROM DUAL";
                Cmd.CommandText = sql;
                Cmd.CommandType = CommandType.Text;
                //return long.Parse(((OracleCommand)Cmd).ExecuteOracleScalar().ToString());
                return long.Parse(((OracleCommand)Cmd).ExecuteScalar().ToString());
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
                    dbSql = dbSql.Replace(pr.ParamsName, pr.ParamsName.Replace(ODAParameter.ODAParamsMark, DbAOracle.DBParamsMark));
                    OracleParameter param = new OracleParameter();
                    param.ParameterName = pr.ParamsName.Replace(ODAParameter.ODAParamsMark, DbAOracle.DBParamsMark);
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            // param.OracleType = OracleType.DateTime;
                            param.OracleDbType = OracleDbType.Date;
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
                            //param.OracleType = OracleType.Number;
                            param.OracleDbType = OracleDbType.Decimal;
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
                            //param.OracleType = OracleType.Blob;
                            param.OracleDbType = OracleDbType.Blob;
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
                                    throw new ODAException(16001, "Params :" + pr.ParamsName + " Type must be byte[]");
                                }
                            }
                            break;
                        case ODAdbType.OInt:
                            //  param.OracleType = OracleType.Int32;
                            param.OracleDbType = OracleDbType.Int32;
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
                            // param.OracleType = OracleType.Char;
                            param.OracleDbType = OracleDbType.Char;
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
                            // param.OracleType = OracleType.VarChar;
                            param.OracleDbType = OracleDbType.Varchar2;
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
                        case ODAdbType.OArrary:
                            // param.OracleType = OracleType.VarChar;
                            param.CollectionType = OracleCollectionType.PLSQLAssociativeArray;
                            param.OracleDbType = OracleDbType.Varchar2;
                            if (pr.ParamsValue == null || pr.ParamsValue is System.DBNull)
                            {
                                param.Value = System.DBNull.Value;
                            }
                            else
                            {
                                param.Value = pr.ParamsValue;
                                if (param.Value is Array)
                                    param.Size = ((Array)param.Value).Length;
                            }
                            break;
                        default:
                            // param.OracleType = OracleType.VarChar;
                            param.OracleDbType = OracleDbType.Varchar2;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((OracleParameterCollection)Cmd.Parameters).Add(param);
                }
            }
            Cmd.CommandText = dbSql;
        }
    }
}
