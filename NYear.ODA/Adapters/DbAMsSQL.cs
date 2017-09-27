using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

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
            _DBConn.Disposed += _DBConn_Disposed;
            return _DBConn;
        }
        private void _DBConn_Disposed(object sender, EventArgs e)
        {
            _DBConn = null;
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
            string sql_tabcol = "SELECT DISTINCT SOBJ.NAME AS TABLE_NAME, SCOL.NAME AS COLUMN_NAME , SCOL.COLID AS COL_SEQ,"
            + " CASE SYT.NAME  WHEN 'sysname'  THEN 'OVarchar' WHEN 'sql_varint'  THEN 'OVarchar'    WHEN 'varchar' THEN 'OVarchar' WHEN 'char'  THEN 'OChar' "
            + " WHEN 'nchar'  THEN 'OChar' WHEN 'ntext'  THEN 'OVarchar'  WHEN 'nvarchar'  THEN 'OVarchar'  WHEN 'text'  THEN 'OVarchar' WHEN 'varchar'  THEN 'OVarchar' "
            + " WHEN 'bigint'  THEN 'ODecimal'  WHEN 'decimal'  THEN 'ODecimal'   WHEN 'float'  THEN 'ODecimal'   WHEN 'money'  THEN 'ODecimal' "
            + " WHEN 'numeric'  THEN 'ODecimal'    WHEN 'real'  THEN 'ODecimal'   WHEN 'smallmoney'  THEN 'ODecimal' "
            + " WHEN 'int'  THEN 'OInt'  WHEN 'smallint'  THEN 'OInt'  WHEN 'bit'  THEN 'OInt'  "
            + " WHEN 'datetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'date'  THEN 'ODatetime' "
            + " WHEN 'binary'  THEN 'OBinary'  WHEN 'image'  THEN 'OBinary'  WHEN 'timestamp'  THEN 'OBinary'    WHEN 'varbinary'  THEN 'OBinary' "
            + " ELSE  SYT.NAME  END AS ODA_DATATYPE ,"
            + " CASE SCOL.ISNULLABLE WHEN 0 THEN 'Y' ELSE 'N' END AS NOT_NULL,"
            + " SCOL.LENGTH AS LENGTH, ext.value AS DIRECTION  "
            + " FROM SYSOBJECTS SOBJ "
            + " inner join SYSCOLUMNS SCOL "
            + " on SOBJ.ID = SCOL.ID  "
            + " inner join SYS.TYPES SYT "
            + " on SYT.IS_USER_DEFINED = 0 "
            + " AND SYT.SYSTEM_TYPE_ID = SCOL.XTYPE "
            + " left join sys.extended_properties ext "
            + " on ext.name= 'MS_Description' "
            + " and ext.major_id =OBJECT_ID (UPPER(SOBJ.NAME)) "
            + " and ext.minor_id = SCOL.colid "
            + " WHERE  SOBJ.XTYPE  = 'U '"
            + " ORDER BY  TABLE_NAME , SCOL.COLID  ";
            DataTable Dt = this.Select(sql_tabcol, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }
        public override DataTable GetViewColumns()
        {
            string sql_view = "SELECT DISTINCT SOBJ.NAME AS TABLE_NAME, SCOL.NAME AS COLUMN_NAME , "
            + " case SCOL.isnullable when 0 then 'False' else 'True' end as REQUIRE,"
            + " CASE SYT.NAME  WHEN 'sysname'  THEN 'OVarchar' WHEN 'sql_varint'  THEN 'OVarchar'    WHEN 'varchar' THEN 'OVarchar' WHEN 'char'  THEN 'OChar' "
            + " WHEN 'nchar'  THEN 'OChar' WHEN 'ntext'  THEN 'OVarchar'  WHEN 'nvarchar'  THEN 'OVarchar'  WHEN 'text'  THEN 'OVarchar' WHEN 'varchar'  THEN 'OVarchar' "
            + " WHEN 'bigint'  THEN 'ODecimal'  WHEN 'decimal'  THEN 'ODecimal'   WHEN 'float'  THEN 'ODecimal'   WHEN 'money'  THEN 'ODecimal' "
            + " WHEN 'numeric'  THEN 'ODecimal'    WHEN 'real'  THEN 'ODecimal'   WHEN 'smallmoney'  THEN 'ODecimal' "
            + " WHEN 'int'  THEN 'OInt'  WHEN 'smallint'  THEN 'OInt'  WHEN 'bit'  THEN 'OInt'  "
            + " WHEN 'datetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'smalldatetime'  THEN 'ODatetime'  WHEN 'date'  THEN 'ODatetime' "
            + " WHEN 'binary'  THEN 'OBinary'  WHEN 'image'  THEN 'OBinary'  WHEN 'timestamp'  THEN 'OBinary'    WHEN 'varbinary'  THEN 'OBinary' "
            + " ELSE  SYT.NAME  END AS ODA_DATATYPE ,"
            + " SCOL.LENGTH AS LENGTH, ext.value AS DIRECTION  "
            + " FROM SYSOBJECTS SOBJ "
            + " inner join SYSCOLUMNS SCOL "
            + " on SOBJ.ID = SCOL.ID  "
            + " inner join SYS.TYPES SYT "
            + " on SYT.IS_USER_DEFINED = 0 "
            + " AND SYT.SYSTEM_TYPE_ID = SCOL.XTYPE "
            + " left join sys.extended_properties ext "
            + " on ext.name= 'MS_Description' "
            + " and ext.major_id = OBJECT_ID (UPPER(SOBJ.NAME)) "
            + " and ext.minor_id = SCOL.colid "
            + " WHERE  SOBJ.XTYPE  = 'V '"
            + " ORDER BY  TABLE_NAME , COLUMN_NAME  ";
            DataTable Dt = this.Select(sql_view, null);
            Dt.TableName = "TABLE_COLUMN";
            return Dt;
        }



        /* 
         * 
         * 索引的脚本
          WITH    idxcol
        AS ( SELECT
              i.object_id ,
              i.index_id ,
              OBJECT_NAME(i.object_id) AS objname ,
              i.name AS idxname ,
              ocol.name AS colname ,
              i.type AS idxtype ,
              i.type_desc AS idxtypedesc ,
              i.is_unique ,
              i.is_primary_key ,
              i.is_unique_constraint ,
              i.fill_factor ,
              icol.key_ordinal AS idxcoloder ,
              icol.is_descending_key ,
              icol.is_included_column ,
              pt.row_count ,
              pt.used_page_count * 8 *1024.0 / POWER(1024, 2) AS [usedrowpage_mb] ,
              pt.reserved_page_count * 8 *1024.0 / POWER(1024, 2) AS [allrowpage_MB]
             FROM
              sys.indexes i ,
              sys.index_columns icol ,
              sys.columns ocol ,
              sys.dm_db_partition_stats pt
             WHERE
              i.object_id = icol.object_id
              AND i.index_id = icol.index_id
              AND icol.object_id = ocol.object_id
              AND icol.column_id = ocol.column_id
              AND i.object_id = pt.object_id
              AND i.index_id = pt.index_id
              AND EXISTS ( SELECT
                              1
                           FROM
                              sys.objects o
                           WHERE
                              o.object_id = i.object_id
                           AND o.type = 'U' ))

SELECT
  * ,
  N'CREATE ' + t.idxtypedesc COLLATE Latin1_General_CI_AS_KS_WS + 
N' INDEX ' + t.idxname COLLATE Latin1_General_CI_AS_KS_WS + 
N' ON ' + t.objname COLLATE Latin1_General_CI_AS_KS_WS +
N'(' + CASE WHEN t.colsinc IS NULL THEN 
t.cols COLLATE Latin1_General_CI_AS_KS_WS 
ELSE 
SUBSTRING(cols,LEN(colsinc)+2,LEN(cols)-LEN(colsinc)) 
END 
+ N')'+CASE WHEN t.colsinc IS NOT NULL THEN ' INCLUDE('+t.colsinc+')' ELSE ' ' END 
FROM
  ( SELECT 
DISTINCT
      object_id ,
      index_id ,
      objname ,
      idxname ,
      idxtypedesc ,
      CASE WHEN is_primary_key = 1 THEN 'prmiary key'
           ELSE CASE WHEN is_unique_constraint = 1 THEN 'unique constraint'
                     ELSE CASE WHEN is_unique = 1 THEN 'Unique '
                               ELSE ''
                          END + idxtypedesc
                END
      END AS typedesc ,
      STUFF(( SELECT
                  ',' + colname + CASE WHEN is_descending_key = 1 THEN ' desc'
                                       ELSE ''
                                  END
              FROM
                  idxcol
              WHERE
                  object_id = c.object_id
                  AND index_id = c.index_id
              ORDER BY
                  idxcoloder
            FOR
              XML PATH('') ), 1, 1, '') AS cols ,
      STUFF(( SELECT
                  ',' + colname
              FROM
                  idxcol
              WHERE
                  object_id = c.object_id
                  AND index_id = c.index_id
                  AND is_included_column = 1
              ORDER BY
                  idxcoloder
            FOR
              XML PATH('') ), 1, 1, '') AS colsinc ,
      row_count ,
      [allrowpage_MB] ,
      [usedrowpage_mb] ,
      [allrowpage_MB] - [usedrowpage_mb] AS unusedrowpage_mb
    FROM
      idxcol c ) AS t
      */


        /*
         * 视图的脚本
                         SELECT o.NAME AS  VIEW_NAME , c.text AS VIEW_SCRIPT
             FROM sys.objects o,sys.syscomments c 
             WHERE   o.type = 'V'
             and c.ID = OBJECT_ID(o.name)
         */

        public override string[] GetPrimarykey(string TableName)
        {
            string PrimaryCols = string.Format("SELECT B.COLUMN_NAME "
            + " FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS A "
            + " INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE B "
            + " ON A.CONSTRAINT_NAME = B.CONSTRAINT_NAME "
            + " WHERE A.CONSTRAINT_TYPE = 'PRIMARY KEY'"
            + " AND A.TABLE_NAME ='{0}'", TableName);
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

        public override DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        {
            int sidx = SQL.IndexOf("SELECT ", 0, StringComparison.InvariantCultureIgnoreCase);
            SQL = SQL.Remove(sidx, "SELECT ".Length);
            int distinct = SQL.IndexOf(" DISTINCT ", 0, StringComparison.InvariantCultureIgnoreCase);

            if (distinct < 0 || distinct > "SELECT * FROM ".Length)
            {
                SQL = SQL.Insert(sidx, "SELECT  TOP " + (StartIndex + MaxRecord).ToString() + " row_number() over(order by getdate()) AS R_ID_1, ");
            }
            else
            {
                SQL = "SELECT TOP " + (StartIndex + MaxRecord).ToString() + " ROW_NUMBER() OVER(ORDER BY GETDATE()) AS R_ID_1,DISTINCT_TMP.* FROM ( SELECT  " + SQL + ") DISTINCT_TMP";
            }
            DataTable dt = Select("SELECT a_b_1.* FROM ( " + SQL + " ) as a_b_1 WHERE a_b_1.R_ID_1 > " + StartIndex.ToString(), ParamList);
            dt.Columns.Remove("R_ID_1");
            return dt;

            //string BlockStr = "select* from (select row_number() over(order by getdate()) as r_id_1,t_1.* from ( ";
            //BlockStr += SQL;
            //BlockStr += ") t_1 ) t_t_1 where t_t_1.r_id_1 > " + StartIndex.ToString() + " and t_t_1.r_id_1  <= " + (StartIndex + MaxRecord).ToString();
            //DataTable dt = Select(BlockStr, ParamList);
            //dt.Columns.Remove("r_id_1");
            //return dt;
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

        protected override void SetCmdParameters(ref  IDbCommand Cmd,string SQL, params ODAParameter[] ParamList)
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
                                    param.Value = pr.ParamsValue;
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
