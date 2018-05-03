//using System.Collections.Generic;
//using System.Data;
//using System.Text;

//namespace NYear.ODA
//{
//    public interface IDBScriptGenerator
//    {
//        string Alias { get; set; }
//        /// <summary>
//        /// 数据
//        /// </summary>
//        string SystemID { get; }
//        string CmdName { get; }

//        ODACmd BaseCmd { get; }
//        /// <summary>
//        /// 操作的表名
//        /// 用作分表
//        ///   CmdName：没有分表的情况下就是表名
//        ///   当对表[CmdName]纵向切割出N个分表时，DBObjectMap是根据路由条件临时给出表名 
//        /// </summary>
//        string DBObjectMap { get; set; }
//        List<SqlJoinScript> JoinCmd { get; }
//        List<ODACmd> ListJoinCmd { get; }
//        List<ODAColumns> WhereColumns { get; }
//        ODAParameter[] GetCountSql(out string CountSql, ODAColumns Col);
//        ODAParameter[] GetDeleteSql(out string Sql);
//        ODAParameter[] GetInsertSql(out string Sql, params ODAColumns[] Cols);
//        ODAParameter[] GetProcedureSql(out string Sql, params ODAColumns[] Cols);
//        ODAParameter[] GetSelectSql(out string SelectSql, params ODAColumns[] Cols);
//        ODAParameter[] GetUpdateSql(out string Sql, params ODAColumns[] Cols);

//    }
//}