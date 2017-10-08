﻿using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;

namespace NYear.ODA.Adapter
{
    public class DbASQLite : DBAccess
    {
        public DbASQLite(string ConnectionString)
            : base(ConnectionString)
        {

        }

        private SQLiteConnection _DBConn = null;
        protected override IDbConnection GetConnection()
        {
            if (_DBConn == null)
                _DBConn = new SQLiteConnection(ConnString);
            if (_DBConn.State == ConnectionState.Closed)
                _DBConn.Open();
            _DBConn.Disposed += _DBConn_Disposed;
            return _DBConn;
        }
        private void _DBConn_Disposed(object sender, EventArgs e)
        {
            _DBConn = null;
        }

        public static void CreateDataBase(string FileName)
        {
            SQLiteConnection.CreateFile(FileName);
        }
        protected override DbDataAdapter GetDataAdapter(IDbCommand SelectCmd)
        {
            return new SQLiteDataAdapter((SQLiteCommand)SelectCmd);
        }

        public override string[] GetUserTables()
        {
            DataTable dt_table = Select("SELECT Name AS TABLE_NAME  FROM SQLITE_MASTER WHERE TYPE='table' ", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["TABLE_NAME"].ToString().Trim().ToUpper();
            }
            return str;
        }
        public override string[] GetUserViews()
        {
            DataTable dt_table = Select("SELECT Name AS VIEW_NAME  FROM SQLITE_MASTER WHERE TYPE='view' ", null);
            string[] str = new string[dt_table.Rows.Count];
            for (int i = 0; i < str.Length; i++)
            {
                str[i] = dt_table.Rows[i]["VIEW_NAME"].ToString().Trim().ToUpper();
            }
            return str;
        }
        public override string[] GetPrimarykey(string TableName)
        {
            SQLiteConnection conn = (SQLiteConnection)this.GetConnection();
            List<string> list = new List<string>();
            using (SQLiteCommand sQLiteCommand2 = new SQLiteCommand(string.Format(CultureInfo.InvariantCulture, "PRAGMA [main].table_info([{0}])", TableName), conn))
            {
                using (SQLiteDataReader sQLiteDataReader2 = sQLiteCommand2.ExecuteReader())
                {
                    while (sQLiteDataReader2.Read())
                    {
                        if (sQLiteDataReader2.GetInt32(5) == 1)
                        {
                            list.Add(sQLiteDataReader2.GetString(1));

                        }
                    }
                }
            }
            return list.ToArray();
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

        public override DbAType DBAType { get { return DbAType.SQLite; } }

        public override DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        {
            string BlockStr = SQL + " limit " + MaxRecord.ToString() + " offset " + StartIndex.ToString();
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
            Cmd.CommandText = SQL;
            if (ParamList != null)
            {
                foreach (ODAParameter pr in ParamList)
                {
                    SQLiteParameter param = new SQLiteParameter();
                    param.ParameterName = pr.ParamsName;
                    if (pr.Size < 0)
                        param.Size = 1;
                    else
                        param.Size = pr.Size;
                    param.Direction = pr.Direction;
                    switch (pr.DBDataType)
                    {
                        case ODAdbType.ODatetime:
                            param.DbType = DbType.DateTime;
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
                            param.DbType = DbType.Decimal;
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
                            param.DbType = DbType.Binary;
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
                                    throw new ODAException(17001, "Params :" + pr.ParamsName + " Type must be byte[]");
                                }
                            }
                            break;
                        case ODAdbType.OInt:
                            param.DbType = DbType.Int32;
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
                            param.DbType = DbType.String;
                            param.Value = pr.ParamsValue;
                            break;
                    }
                    ((SQLiteParameterCollection)Cmd.Parameters).Add(param);
                }
            }
        }
    }
}

