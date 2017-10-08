using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;

namespace NYear.ODA
{
    public abstract class DBAccess : IDBAccess //: MarshalByRefObject,
    {
        #region 数据类型转换
        public static List<T> ConvertToList<T>(DataTable dt) where T : class
        {
            List<T> list = new List<T>();
            T t = Activator.CreateInstance<T>();
            PropertyInfo[] propertypes = t.GetType().GetProperties();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                t = Activator.CreateInstance<T>();
                foreach (PropertyInfo pro in propertypes)
                {
                    if (pro.CanWrite && dt.Columns.Contains(pro.Name))
                    {
                        object value = dt.Rows[i][pro.Name];
                        pro.SetValue(t, DataConvert(value, pro.PropertyType), null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        public static T ConvertToModel<T>(DataTable dt)
        {
            T t = Activator.CreateInstance<T>();
            PropertyInfo[] propertypes = t.GetType().GetProperties();
            if (dt.Rows.Count > 0)
            {
                foreach (PropertyInfo pro in propertypes)
                {
                    if (pro.CanWrite && dt.Columns.Contains(pro.Name))
                    {
                        object value = dt.Rows[0][pro.Name];
                        pro.SetValue(t, DataConvert(value, pro.PropertyType), null);
                    }
                }
                return t;
            }
            return default(T);
        }

        public static T ConvertToModeT<T>(object A) where T : class
        {
            T t = default(T);
            t = Activator.CreateInstance<T>();
            PropertyInfo[] PList = t.GetType().GetProperties();
            Type AT = A.GetType();
            foreach (PropertyInfo P in PList)
            {
                try
                {
                    object obj = AT.GetProperty(P.Name).GetValue(A, null);
                    P.SetValue(t, DataConvert(obj, P.PropertyType), null);
                }
                catch { }
            }
            return t;
        }

        public static object DataConvert(object val, Type targetType)
        {
            if (val == null || val == System.DBNull.Value)
            {
                if (targetType.IsValueType)
                    return Activator.CreateInstance(targetType);
                return null;
            }
            if (targetType.IsInstanceOfType(val))
            {
                return val;
            }
            else
            {
                try
                {
                    Type baseTargetType = targetType;
                    if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        baseTargetType = Nullable.GetUnderlyingType(targetType);
                    return System.Convert.ChangeType(val, baseTargetType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    if (targetType.IsValueType)
                        return Activator.CreateInstance(targetType);
                    return null;
                }
            }
        }
        #endregion

        private string _ConnStr = null;
        public string ConnString { get { return _ConnStr; } }
        public DBAccess(string ConnectionString)
        {
            _ConnStr = ConnectionString;
        }
        protected abstract DbDataAdapter GetDataAdapter(IDbCommand SelectCmd);
        protected abstract IDbConnection GetConnection();
        public virtual IDbTransaction Transaction { get; set; }
        public abstract string[] GetUserTables();
        public abstract string[] GetUserViews();
        public abstract DbAType DBAType { get; }

        public virtual char ParamsMark { get { return ODAParameter.ODAParamsMark; } }

        public virtual string[] GetUserProcedure()
        {
            throw new NotSupportedException("DBMS not support Procedure");
        }
        public virtual DataTable GetUserProcedureArguments(string ProcedureName)
        {
            throw new NotSupportedException("DBMS not support Procedure");
        }

        public abstract object GetExpressResult(string ExpressionString);

        public virtual DataTable GetTableColumns()
        {
            string[] user_tables = this.GetUserTables();
            return GetColumns(user_tables, "TABLE_COLUMN");
        }

        public abstract string[] GetPrimarykey(string TableName);

        public virtual DataTable GetUniqueIndex(string TableName)
        {
            return null;
        }

        public virtual DataTable GetViewColumns()
        {
            string[] UserView = this.GetUserViews();
            return GetColumns(UserView, "VIEW_COLUMN");
        }
        public virtual DatabaseColumnInfo ODAColumnToOrigin(string Name, string ColumnType, decimal Length)
        {
            DatabaseColumnInfo ColInof = new DatabaseColumnInfo();
            ColInof.Name = Name;
            ColInof.NoLength = false;
            ColInof.Length = Length;

            if (ColumnType.Trim() == ODAdbType.OBinary.ToString())
            {
                ColInof.ColumnType = "BINARY";
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
                return null;
            }
            return ColInof;
        }

        private DataTable GetColumns(string[] TableViewNames, string TableName)
        {
            IDbConnection Conn = (IDbConnection)GetConnection();
            if (Conn.State != ConnectionState.Open)
            {
                Conn.ConnectionString = ConnString;
                Conn.Open();
            }
            try
            {
                string[] UserView = TableViewNames;
                DataTable Dt = new DataTable(TableName);
                DataColumn dcTableName = new DataColumn("TABLE_NAME");
                Dt.Columns.Add(dcTableName);
                DataColumn dcColumnName = new DataColumn("COLUMN_NAME");
                Dt.Columns.Add(dcColumnName);
                DataColumn dcColSeq = new DataColumn("COL_SEQ");
                Dt.Columns.Add(dcColSeq);

                DataColumn dcOdaDatatype = new DataColumn("ODA_DATATYPE");
                Dt.Columns.Add(dcOdaDatatype);
                DataColumn dcLength = new DataColumn("LENGTH");
                Dt.Columns.Add(dcLength);
                DataColumn dcDirection = new DataColumn("DIRECTION");
                Dt.Columns.Add(dcDirection);
                DataColumn NotNull = new DataColumn("NOT_NULL");
                Dt.Columns.Add(NotNull);

                for (int i = 0; i < UserView.Length; i++)
                {
                    IDbCommand Cmd = Conn.CreateCommand();
                    Cmd.CommandText = "select * from  " + UserView[i] + " where 1=0 ";
                    Cmd.CommandType = CommandType.Text;

                    IDataReader idr = Cmd.ExecuteReader();


                    DataTable sch = idr.GetSchemaTable();
                    if (sch != null && sch.Rows.Count > 0)
                    {
                        for (int j = 0; j < sch.Rows.Count; j++)
                        {
                            DataRow dr_tmp = Dt.NewRow();
                            dr_tmp["TABLE_NAME"] = UserView[i];
                            dr_tmp["COLUMN_NAME"] = (string)sch.Rows[j]["ColumnName"];
                            int ln = (int)sch.Rows[j]["ColumnSize"];
                            ln = ln <= 0 ? 2000 : ln > 2000 ? 2000 : ln;
                            dr_tmp["LENGTH"] = ln;
                            dr_tmp["DIRECTION"] = "";
                            dr_tmp["NOT_NULL"] = ((bool)sch.Rows[j]["AllowDBNull"])?"N":"Y";
                            dr_tmp["COL_SEQ"] = j ;

                            string ColumnDataType = "ODA_DATATYPE";
                            Type Columntype =(Type) sch.Rows[j]["DataType"];
                            if (Columntype == typeof(string))
                            {
                                dr_tmp[ColumnDataType] = "OVarchar";
                            }
                            else if (Columntype == typeof(int))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.OInt;
                            }
                            else if (Columntype == typeof(long))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.ODecimal;
                            }
                            else if (Columntype == typeof(double))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.ODecimal;
                            }
                            else if (Columntype == typeof(float))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.ODecimal;
                            }
                            else if (Columntype == typeof(decimal))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.ODecimal;
                            }
                            else if (Columntype == typeof(System.DateTime))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.ODatetime;
                            }
                            else if (Columntype == typeof(byte[]))
                            {
                                dr_tmp[ColumnDataType] = ODAdbType.OBinary;
                            }
                            else
                            {
                                dr_tmp[ColumnDataType] = "OVarchar";
                            }

                            Dt.Rows.Add(dr_tmp);
                        }
                    }
                    idr.Close();
                }
                return Dt;
            }
            finally
            {
                Conn.Close();
            }
        }

        public virtual long GetSequenceNextVal(string SequenceName)
        {
            IDbCommand CmdU = OpenCommand();
            IDbCommand CmdS = OpenCommand();
            try
            {
                CmdU.CommandText = "UPDATE SEQUENCE_TABLE SET CURRENCE_VALUE = CURRENCE_VALUE + SETVAL WHERE SEQUENCE_NAME = '" + SequenceName + "'";
                CmdU.CommandType = CommandType.Text;
                CmdU.ExecuteNonQuery();
                CmdS.CommandText = "SELECT CURRENCE_VALUE FROM SEQUENCE_TABLE WHERE SEQUENCE_NAME = '" + SequenceName + "'";
                CmdS.CommandType = CommandType.Text;
                object obj = CmdS.ExecuteScalar();
                long currence_value = long.Parse(obj.ToString());
                return currence_value;
            }
            finally
            {
                CloseCommand(CmdU);
                CloseCommand(CmdS);
            }
        }
        public virtual DateTime GetDBDateTime() { return DateTime.Now; }
        public string Database { get { return GetConnection().Database; } }
        #region 事务管理
        /// 开始事务
        /// </summary>
        /// <param name="TimeOut">事务超时时长，小于或等于0时事务不会超时，单位:秒</param>
        /// <returns>返回事务的ID</returns>
        [System.ComponentModel.Description("事务超时时长，小于或等于0时事务不会超时,单位:秒")]
        public void BeginTransaction()
        {
            if (this.Transaction != null)
                return;
            this.Transaction = GetConnection().BeginTransaction();
        }

        public void Commit()
        {
            if (this.Transaction != null)
            {
                IDbConnection Conn = this.Transaction.Connection;
                this.Transaction.Commit();
                this.Transaction.Dispose();
                Conn.Close();
                Conn.Dispose();
                this.Transaction = null;
            }
            else
            {
                throw new ODAException(90000, "There isn't any Transaction to Commit");
            }

        }
        public void RollBack()
        {
            if (this.Transaction != null)
            {
                IDbConnection Conn = this.Transaction.Connection;
                this.Transaction.Rollback();
                this.Transaction.Dispose();
                Conn.Close();
                Conn.Dispose();
                this.Transaction = null;
            }
            else
            {
                throw new ODAException(90001, "There isn't any Transaction to RollBack");
            }
        }

        #endregion

        #region DML语句执行

        protected IDbCommand OpenCommand()
        {
            IDbCommand Cmd = null;
            if (this.Transaction != null)
            {
                Cmd = this.Transaction.Connection.CreateCommand();
                Cmd.Transaction = this.Transaction;
                return Cmd;
            }
            else
            {
                Cmd = this.GetConnection().CreateCommand();
            }
            return Cmd;
        }
        protected void CloseCommand(IDbCommand Cmd)
        {
            if (Cmd != null)
            {
                if (Cmd.Transaction == null)
                {
                    Cmd.Connection.Close();
                    Cmd.Connection.Dispose();
                    Cmd.Dispose();
                }
                else
                {
                    Cmd.Dispose();
                }
            }
        }

        protected abstract void SetCmdParameters(ref IDbCommand Cmd,string SQL, params ODAParameter[] ParamList);

        public List<T> Select<T>(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord, out int TotalRecord) where T : class
        {
            return ConvertToList<T>(Select(SQL, ParamList, StartIndex, MaxRecord, out TotalRecord));
        }
        public virtual DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord, out int TotalRecord)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandType = CommandType.Text;
                SetCmdParameters(ref Cmd, SQL, ParamList);
                IDataReader Dr = Cmd.ExecuteReader();
                TotalRecord = 0;
                DataTable dt = new DataTable("RECORDSET");
                if (Dr.FieldCount > 0)
                {
                    for (int num = 0; num < Dr.FieldCount; num++)
                    {
                        DataColumn column = new DataColumn();
                        if (dt.Columns.Contains(Dr.GetName(num)))
                            column.ColumnName = Dr.GetName(num) + num.ToString();
                        else
                            column.ColumnName = Dr.GetName(num);
                        column.DataType = Dr.GetFieldType(num);
                        dt.Columns.Add(column);
                    }
                    while (StartIndex > 0)
                    {
                        if (!Dr.Read())
                            return dt;
                        StartIndex--;
                        TotalRecord++;
                    }

                    int ReadRecord = MaxRecord;
                    while (ReadRecord > 0 || MaxRecord == -1)
                    {
                        if (Dr.Read())
                        {
                            DataRow row = dt.NewRow();
                            for (int num = 0; num < Dr.FieldCount; num++)
                            {
                                row[num] = Dr[num];
                            }
                            dt.Rows.Add(row);
                            ReadRecord--;
                            TotalRecord++;
                        }
                        else
                            break;
                    }
                    while (Dr.Read())
                    {
                        TotalRecord++;
                    }
                }
                Dr.Close();
                Dr.Dispose();
                return dt;
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        public virtual DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandType = CommandType.Text;
                SetCmdParameters(ref Cmd, SQL, ParamList);
                IDataReader Dr = Cmd.ExecuteReader();
                DataTable dt = new DataTable("RECORDSET");
                if (Dr.FieldCount > 0)
                {
                    for (int num = 0; num < Dr.FieldCount; num++)
                    {
                        DataColumn column = new DataColumn();
                        if (dt.Columns.Contains(Dr.GetName(num)))
                            column.ColumnName = Dr.GetName(num) + num.ToString();
                        else
                            column.ColumnName = Dr.GetName(num);
                        column.DataType = Dr.GetFieldType(num);
                        dt.Columns.Add(column);
                    }
                    while (StartIndex > 0)
                    {
                        if (!Dr.Read())
                            return dt;
                        StartIndex--;
                    }

                    int ReadRecord = MaxRecord;
                    while (ReadRecord > 0 || MaxRecord == -1)
                    {
                        if (Dr.Read())
                        {
                            DataRow row = dt.NewRow();
                            for (int num = 0; num < Dr.FieldCount; num++)
                            {
                                row[num] = Dr[num];
                            }
                            dt.Rows.Add(row);
                            ReadRecord--;
                        }
                        else
                            break;
                    }
                }
                Dr.Close();
                Dr.Dispose();
                return dt;
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }

        public List<T> Select<T>(string SQL, ODAParameter[] ParamList) where T : class
        {
            return ConvertToList<T>(Select(SQL, ParamList));
        }

        public List<T> Select<T>(string SQL, ODAParameter[] ParamList, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, int MaxLevel) where T : class
        {
            return ConvertToList<T>(Select(SQL, ParamList, StartWithExpress, ConnectBy, Prior, ConnectColumn, ConnectChar, MaxLevel));
        }

        public virtual DataTable Select(string SQL, ODAParameter[] ParamList)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandType = CommandType.Text;
                SetCmdParameters(ref Cmd, SQL, ParamList);
                DbDataAdapter Da = GetDataAdapter(Cmd);
                DataTable dt = new DataTable();
                Da.Fill(dt);
                Da.Dispose();
                return dt;
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }
        /// <summary>
        /// 递归取值，返回树状结构表，先把所有符合条件的数据读进内存然后递归筛选
        /// </summary>
        /// <param name="SQL">查询语句</param>
        /// <param name="ParamList">查询语句中的变量</param>
        /// <param name="StartWithExpress">递时入口条件</param>
        /// <param name="ConnectBy">连接的父字段</param>
        /// <param name="Prior">连接的子字段</param>
        /// <param name="ConnectColumn">连接的返回值字段</param>
        /// <param name="ConnectChar">父子之间的连接符</param>
        /// <param name="MaxLevel">递归深度</param>
        /// <returns></returns>       
        public DataTable Select(string SQL, ODAParameter[] ParamList, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, int MaxLevel)
        {
            DataTable Model = Select(SQL, ParamList);
            if (!String.IsNullOrEmpty(ConnectColumn))
            {
                if (Model.Columns.Contains(ConnectColumn))
                    Model.Columns[ConnectColumn].DataType = typeof(string);
                else
                    throw new ODAException(90002, "DataModel not contain Column:" + ConnectColumn);
            }

            if (!Model.Columns.Contains(ConnectBy) || !Model.Columns.Contains(Prior))
                throw new ODAException(90003, "DataModel not contain ConnectBy or Prior Column");

            DataTable dtRtl = this.Recursion(Model, StartWithExpress, ConnectBy, Prior, ConnectColumn, ConnectChar, "", 0, MaxLevel);
            return dtRtl;
        }

        /// <summary>
        /// 数据库树状结构递归，先把所有符合条件的数据读进内存然后递归筛选
        /// </summary>
        /// <param name="Model"></param>
        /// <param name="StartWithExpress"></param>
        /// <param name="ConnectBy"></param>
        /// <param name="Prior"></param>
        /// <param name="ConnectColumn"></param>
        /// <param name="ConnectChar"></param>
        /// <param name="PerentColumnString"></param>
        /// <param name="Deep"></param>
        /// <param name="MaxLevel"></param>
        /// <returns></returns>
        protected virtual DataTable Recursion(DataTable Model, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, string PerentColumnString, int Deep, int MaxLevel)
        {
            Deep++;
            DataTable Rtldt = Model.Clone();
            Rtldt.TableName = Model.TableName;
            if (Deep <= MaxLevel || MaxLevel == 0)
            {
                DataView dv = Model.DefaultView;
                dv.RowFilter = StartWithExpress;
                string PerentColumn = "";
                for (int i = 0; i < dv.Count; i++)
                {
                    Rtldt.Rows.Add(dv[i].Row.ItemArray);
                    if (!String.IsNullOrWhiteSpace(ConnectColumn))
                    {
                        Rtldt.Rows[Rtldt.Rows.Count - 1][ConnectColumn] = PerentColumnString + dv[i][ConnectColumn].ToString();
                        PerentColumn = PerentColumnString + dv[i][ConnectColumn].ToString() + ConnectChar;
                    }
                    DataTable ChildModel = Model.Copy();
                    DataTable dtChild = this.Recursion(ChildModel, ConnectBy + "='" + dv[i][Prior].ToString() + "'", ConnectBy, Prior, ConnectColumn, ConnectChar, PerentColumn, Deep, MaxLevel);
                    Rtldt.Merge(dtChild);
                }
            }
            return Rtldt;
        }

        /// <summary>
        /// 执行SQL,返回影响行数
        /// </summary>
        /// <param name="SQL">SQL语句</param>
        /// <param name="ParamList">SQL语句中的变量值</param>
        /// <returns></returns>
        public virtual int ExecuteSQL(string SQL, ODAParameter[] ParamList)
        {
            IDbCommand Cmd = OpenCommand();
            try
            {
                Cmd.CommandType = CommandType.Text;
                SetCmdParameters(ref Cmd, SQL, ParamList);
                return Cmd.ExecuteNonQuery();
            }
            finally
            {
                CloseCommand(Cmd);
            }
        }

        public virtual List<T> ExecuteProcedureGetList<T>(string SQL, ODAParameter[] ParamList, int RecordIndex) where T : class
        {
            DataSet Ds = ExecuteProcedure(SQL, ParamList);
            DataTable Dt = new DataTable();
            if (Ds.Tables.Count > RecordIndex)
                Dt = Ds.Tables[RecordIndex];
            return ConvertToList<T>(Dt);
        }

        public virtual List<ValuesCollection> ExecuteProcedureGetValues(string SQL, ODAParameter[] ParamList)
        {
            DataSet Ds = ExecuteProcedure(SQL, ParamList);
            return ConvertToList<ValuesCollection>(Ds.Tables["ValuesCollection"]);
        }

        public virtual DataSet ExecuteProcedure(string SQL, ODAParameter[] ParamList)
        {
            IDbCommand Cmd = OpenCommand();
            DataSet ds_rtl = new DataSet("ReturnValues");
            DataTable dt_values = new DataTable("ValuesCollection");
            dt_values.Columns.Add("ParamName");
            dt_values.Columns.Add("ReturnValue");
            IDataReader datareader = null;
            try
            {
                Cmd.CommandType = CommandType.StoredProcedure;
                SetCmdParameters(ref Cmd, SQL, ParamList);
                datareader = Cmd.ExecuteReader();

                int rtlcount = 0;
                do
                {
                    if (datareader.FieldCount > 0)
                    {
                        DataTable dt = new DataTable("RECORDSET" + rtlcount.ToString());
                        rtlcount++;
                        for (int num = 0; num < datareader.FieldCount; num++)
                        {
                            DataColumn column = new DataColumn();
                            if (dt.Columns.Contains(datareader.GetName(num)))
                                column.ColumnName = datareader.GetName(num) + num.ToString();
                            else
                                column.ColumnName = datareader.GetName(num);
                            column.DataType = datareader.GetFieldType(num);
                            dt.Columns.Add(column);
                        }
                        while (datareader.Read())
                        {
                            DataRow row = dt.NewRow();
                            for (int num = 0; num < datareader.FieldCount; num++)
                            {
                                row[num] = datareader[num];
                            }
                            dt.Rows.Add(row);
                        }
                        ds_rtl.Tables.Add(dt);
                    }
                }
                while (datareader.NextResult());
                datareader.Close();
                datareader.Dispose();

                foreach (DbParameter param in Cmd.Parameters)
                {
                    if (param.Direction == System.Data.ParameterDirection.InputOutput || param.Direction == System.Data.ParameterDirection.Output)
                    {
                        DataRow dr = dt_values.NewRow();
                        dr["ParamName"] = param.ParameterName;
                        dr["ReturnValue"] = param.Value;
                        dt_values.Rows.Add(dr);
                    }
                }
                ds_rtl.Tables.Add(dt_values);

                return ds_rtl;
            }
            finally
            {
                dt_values.Dispose();
                ds_rtl.Dispose();
                if (datareader != null)
                {
                    datareader.Close();
                    datareader.Dispose();
                }
                CloseCommand(Cmd);
            }
        }
        #endregion
    }
}
