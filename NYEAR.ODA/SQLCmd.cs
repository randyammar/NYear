using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace NYear.ODA
{
    public  class SQLCmd : ODACmd
    {

        #region 执行SQL语句
        private string DeleteSql = null;
        private string InsertSql = null;
        private string UpdateSql = null;
        private string SelectSql = null;
        private string OrderbySql = null; 
        private List<string> SqlTables = new List<string>();

        public void SetOperationTables(params string[] Tables)
        {
            if (Tables != null && Tables.Length > 0)
                this.SqlTables.AddRange(Tables);
        }
        protected override ODAScript GetCountSql(ODAColumns Col)
        {
            if (string.IsNullOrWhiteSpace(SelectSql))
                throw new ODAException(50001, "Not Select Sql to execute!");
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Select,
            };
            if (Object.ReferenceEquals(Col, null))
            {
                sql.SqlScript.Append("SELECT COUNT(*) FROM (")
                .Append(SelectSql.Replace(OrderbySql, OrderbySql))
                .Append(" ) CountSql");
            }
            else
            {
                string sqlCol = "";
                var prms = Col.GetSelectColumn(out sqlCol);
                sql.ValueList.AddRange(prms);
                sql.SqlScript.Append("SELECT COUNT(")
                    .Append(sqlCol)
                    .Append(") FROM (")
                    .Append(SelectSql.Replace(OrderbySql, OrderbySql))
                    .Append(" ) CountSql");
            }
            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                var asql = GetWhereSubSql(_WhereList, " AND ");
                if (asql.WhereList.Count > 0)
                    sql.WhereList.AddRange(asql.WhereList.ToArray()); 
                var osql = GetWhereSubSql(_OrList, " OR ");
                if (osql.WhereList.Count > 0)
                    sql.WhereList.AddRange(osql.WhereList.ToArray());
            }
            return sql;
        }
        protected override ODAScript GetDeleteSql()
        {
            if (string.IsNullOrWhiteSpace(DeleteSql))
                throw new ODAException(50002, "Not DeleteSql to execute!");

            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Delete,
            };
            if (SqlTables.Count > 0)
                sql.TableList.AddRange(this.SqlTables.ToArray());
            sql.SqlScript.Append(DeleteSql);
            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                var asql = GetWhereSubSql(_WhereList, " AND ");
                if (asql.WhereList.Count > 0)
                    sql.WhereList.AddRange(asql.WhereList.ToArray());
                var osql = GetWhereSubSql(_OrList, " OR ");
                if (osql.WhereList.Count > 0)
                    sql.WhereList.AddRange(osql.WhereList.ToArray());
            }
            return sql;
        }
        protected override ODAScript GetInsertSql(params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(InsertSql))
                throw new ODAException(50003, "Not InsertSql to execute!"); 
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Insert,
            };
            sql.SqlScript.Append(InsertSql);
          
            for (int i = 0; i < Cols.Length; i++)
            {
                string ColumnTmp = "";
                string ColumnParamsTmp = "";
                sql.ValueList.AddRange(Cols[i].GetInsertSubstring(out ColumnTmp, out ColumnParamsTmp)); 
            }
            if (SqlTables.Count > 0)
                sql.TableList.AddRange(this.SqlTables.ToArray());
            return sql;
        }
        protected override ODAScript GetUpdateSql(params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(UpdateSql))
                throw new ODAException(50003, "Not UpdateSql to execute!");
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Update,
            };
            sql.SqlScript.Append(UpdateSql);
            if (SqlTables.Count > 0)
                sql.TableList.AddRange(this.SqlTables.ToArray());
            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                var asql = GetWhereSubSql(_WhereList, " AND ");
                if (asql.WhereList.Count > 0)
                    sql.WhereList.AddRange(asql.WhereList.ToArray());
                var osql = GetWhereSubSql(_OrList, " OR ");
                if (osql.WhereList.Count > 0)
                    sql.WhereList.AddRange(osql.WhereList.ToArray());
            }
            for (int i = 0; i < Cols.Length; i++)
            {
                string ColumnTmp = "";
                ODAParameter[] P = Cols[i].GetUpdateSubstring(out ColumnTmp);
                if (P != null)
                    sql.ValueList.AddRange(P); 
            }
            return sql;
        }
        public override ODAScript GetSelectSql(params ODAColumns[] Cols)
        {
            if (string.IsNullOrWhiteSpace(SelectSql))
                throw new ODAException(50003, "Not SelectSql to execute!");
            ODAScript sql = new ODAScript()
            {
                ScriptType = SQLType.Select,
            };
            sql.SqlScript.Append(SelectSql);
            if (SqlTables.Count > 0)
                sql.TableList.AddRange(this.SqlTables.ToArray()); 
            if (_WhereList.Count > 0 || _OrList.Count > 0)
            {
                var asql = GetWhereSubSql(_WhereList, " AND ");
                if (asql.WhereList.Count > 0)
                    sql.WhereList.AddRange(asql.WhereList.ToArray());
                var osql = GetWhereSubSql(_OrList, " OR ");
                if (osql.WhereList.Count > 0)
                    sql.WhereList.AddRange(osql.WhereList.ToArray());
            }
            return sql;
        }
    
        /// <summary>
        /// 自定义SQL语名不支持指定查询字段的select
        /// </summary>
        /// <param name="Cols"></param>
        /// <returns></returns>
        public override DataTable Select(params ODAColumns[] Cols)
        {
            throw new ODAException(50004, "Not suport,Use Select(string SQL,params string[] Tables) instead!"); 
        }
        /// <summary>
        /// 在数据库中执行指定的SQL查询
        /// </summary>
        /// <param name="SQL">需要执行的SQL语句</param>
        /// <param name="Tables">查询语句中涉及的数据库表（可选参数）</param>
        /// <returns></returns>
        public DataTable Select(string SQL)
        {
            this.SelectSql = SQL; 
            return base.Select();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="StartIndex">读取查询到的数据起始记录数</param>
        /// <param name="MaxRecord">读取数据的记录数</param>
        /// <param name="TotalRecord">查询到的总记录数</param>
        /// <param name="SQL">需要执行的SQL语句</param>
        /// <param name="Tables">查询语句中涉及的数据库表（可选参数）</param>
        /// <returns></returns>
        public virtual DataTable Select(int StartIndex, int MaxRecord, out int TotalRecord, string SQL, string Orderby = null, params string[] Tables)
        {
            this.SelectSql = SQL;
            if (Tables != null && Tables.Length > 1)
                SqlTables.AddRange(Tables);
            this.OrderbySql = Orderby;
            TotalRecord = this.CountRecords();
            var sql = this.GetSelectSql(null);
            var db = this.GetDBAccess(sql);
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            return db.Select(sql.SqlScript.ToString(), prms, StartIndex, MaxRecord, sql.OrderBy); 
        }

        /// <summary>
        ///  查询数据并转换为对象列表
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="Cols">查询的字段，可为空，空则返回所有字段</param>
        /// <returns></returns>
        public virtual List<T> Select<T>(string SQL) where T : class
        {
            this.SelectSql = SQL; 
            return base.Select<T>();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="StartIndex">读取查询到的数据起始记录数</param>
        /// <param name="MaxRecord">读取数据的记录数</param>
        /// <param name="TotalRecord">查询到的总记录数</param>
        /// <param name="SQL">需要执行的SQL语句</param>
        /// <param name="Tables">查询语句中涉及的数据库表（可选参数）</param>
        /// <returns></returns>
        public virtual List<T> Select<T>(int StartIndex, int MaxRecord, out int TotalRecord,string SQL, string Orderby = null) where T : class
        {
            this.SelectSql = SQL;
            this.OrderbySql = Orderby;
            TotalRecord = this.CountRecords();
            var sql = this.GetSelectSql(null);
            var db = this.GetDBAccess(sql);
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            return db.Select<T>(sql.SqlScript.ToString(), prms, StartIndex, MaxRecord, sql.OrderBy);
        }
         
        
        /// <summary>
        /// 查询第一条记录
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public object[] SelectFirst(string SQL)
        {
            this.SelectSql = SQL;
            var sql = this.GetSelectSql(null);
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(50005, "ODACmd Select 没有执行程序");
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            return db.SelectFirst(sql.SqlScript.ToString(), prms);
        }
        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="Data">源数据</param>
        /// <param name="Prms">数据表对应的字段（Data.Roww[n][ColumnIndex]与Prms[ColumnIndex]对应）</param>
        /// <returns></returns>
        public override bool Import(DataTable Data, ODAParameter[] Prms)
        { 
            throw new ODAException(50006, "Not Import!");
        }
        /// <summary>
        /// 在数据库中执行Delete 语句
        /// </summary>
        /// <returns></returns>
        public bool Delete(string SQL)
        {
            this.DeleteSql = SQL;
            var sql = this.GetDeleteSql();
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(10013, "ODACmd Delete 没有执行程序");
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
        }
        /// <summary>
        /// 在数据库中执行update 语句
        /// </summary>
        /// <param name="Cols">需要更新的字段及其值</param>
        /// <returns></returns>
        public  bool Update(string SQL,params ODAColumns[] Cols)
        {
            this.UpdateSql = SQL;
            var sql = this.GetUpdateSql(Cols);
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(10014, "ODACmd Update 没有执行程序");
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
        }

        /// <summary>
        /// 在数据库中执行insert 语句
        /// </summary>
        /// <param name="Cols">插件的字段及其值</param>
        /// <returns></returns>
        public virtual bool Insert(string SQL,params ODAColumns[] Cols)
        {
            this.InsertSql = SQL;
            var sql = this.GetInsertSql(Cols);
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(10015, "ODACmd Update 没有执行程序");
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            return db.ExecuteSQL(sql.SqlScript.ToString(), prms) > 0;
        }
        /// <summary>
        /// insert () select
        /// </summary>
        /// <param name="SelectCmd"></param>
        /// <param name="Cols">select的字段</param>
        /// <returns></returns>
        public override bool Insert(ODACmd SelectCmd, params ODAColumns[] Cols)
        {
            throw new ODAException(50007, "Not Import!");
        }

        /// <summary>
        /// 在数据库中Procedure
        /// </summary>
        /// <param name="Cols">存储过程的参数及其值</param>
        /// <returns></returns>
        public override DataSet Procedure(params ODAColumns[] Cols)
        {
            string sqlScript = "";
            var prms = this.GetProcedureSql(out sqlScript, Cols);
            var sql = new ODAScript()
            {
                ScriptType = SQLType.Procedure,
            };
            sql.ValueList.AddRange(prms);
            sql.TableList.Add(sqlScript);
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(10017, "ODACmd Procedure 没有执行程序");
            return db.ExecuteProcedure(sqlScript, prms);
        }

        public ODAColumns CreateODAColumn(string Name, ODAdbType ODAType,int Size)
        {
            return new ODAColumns(this, Name, ODAType, Size);
        }

        #endregion
    }
}
