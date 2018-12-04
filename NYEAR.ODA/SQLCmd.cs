using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace NYear.ODA
{
    /// <summary>
    /// 执行用户自定义SQL语
    /// </summary>
    public class SQLCmd : IODACmd
    {
        public string Alias { get; set; }
        public GetDBAccessHandler GetDBAccess { get; set; }
        public DataTable Select(string Sql, params ODAParameter[] Parameters)
        {
            ODAScript oSql = new ODAScript()
            {
                ScriptType = SQLType.Select
            };
            oSql.SqlScript.Append(Sql);
            if (Parameters != null)
                oSql.ParamList.AddRange(Parameters);
            var db = GetDBAccess(oSql); 
            return db.Select(oSql.SqlScript.ToString(), oSql.ParamList.ToArray());
        } 
        public List<T> Select<T>(string Sql, params ODAParameter[] Parameters) where T : class
        {
            ODAScript oSql = new ODAScript()
            {
                ScriptType = SQLType.Select
            };
            oSql.SqlScript.Append(Sql);
            if (Parameters != null)
                oSql.ParamList.AddRange(Parameters);
            var db = GetDBAccess(oSql);
            return db.Select<T>(oSql.SqlScript.ToString(), oSql.ParamList.ToArray());
        }
        public dynamic SelectDynamicFirst(string Sql, params ODAParameter[] Parameters)
        {
            ODAScript oSql = new ODAScript()
            {
                ScriptType = SQLType.Select
            };
            oSql.SqlScript.Append(Sql);
            if (Parameters != null)
                oSql.ParamList.AddRange(Parameters);

            var db = GetDBAccess(oSql);
            var dt = db.Select(Sql, Parameters, 0, 1, "");
            OModel M = new OModel();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataColumn c in dt.Columns)
                {
                    M.Add(c.ColumnName, dt.Rows[0][c.ColumnName]);
                }
            }
            return M;
        }


        public bool Update(string Sql, params ODAParameter[] Parameters)
        {
            return Execute(Sql, SQLType.Update, Parameters);
        }

        public bool Insert(string Sql, params ODAParameter[] Parameters)
        {
            return Execute(Sql, SQLType.Insert, Parameters);
        }

        public bool Delete(string Sql, params ODAParameter[] Parameters)
        {
            return Execute(Sql, SQLType.Delete, Parameters);
        }

        public DataSet Procedure(string Sql, params ODAParameter[] Parameters)
        {
            ODAScript oSql = new ODAScript()
            {
                ScriptType =  SQLType.Procedure
            };
            oSql.SqlScript.Append(Sql);
            if (Parameters != null)
                oSql.ParamList.AddRange(Parameters);
            var db = GetDBAccess(oSql);
            return db.ExecuteProcedure(Sql, Parameters);
        }

        private bool Execute(string Sql, SQLType sqlType,  params ODAParameter[] Parameters)
        {
            ODAScript oSql = new ODAScript()
            {
                ScriptType = sqlType
            };
            oSql.SqlScript.Append(Sql);
            if (Parameters != null)
                oSql.ParamList.AddRange(Parameters);
            var db = GetDBAccess(oSql);
            return db.ExecuteSQL(oSql.SqlScript.ToString(), oSql.ParamList.ToArray()) > 0;
        }
    }
}
