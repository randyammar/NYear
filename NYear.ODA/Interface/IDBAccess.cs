using System;
using System.Collections.Generic;
using System.Data;
#if FW
using System.ServiceModel;
#endif
namespace NYear.ODA
{
#if FW
    [ServiceKnownType(typeof(DBNull))]
    [ServiceContract]
#endif
    public interface IDBAccess
    {
        char ParamsMark { get; }
        DbAType DBAType { get; }
        string ConnString { get; }
        string[] ObjectFlag { get; }
        IDbTransaction Transaction { get; set; }
        Action<IDbCommand> ExecutingCommand { get; set; }
#if FW
        [OperationContract]
#endif
        void BeginTransaction();
#if FW
        [OperationContract]
#endif
        void Commit();
#if FW
        [OperationContract]
#endif
        void RollBack();
#if FW
        [OperationContract]
#endif
        DataSet ExecuteProcedure(string SQL, ODAParameter[] ParamList);
#if FW
        [OperationContract]
#endif
        int ExecuteSQL(string SQL, ODAParameter[] ParamList);
#if FW
        [OperationContract]
#endif
        DateTime GetDBDateTime();
#if FW
        [OperationContract]
#endif
        object GetExpressResult(string ExpressionString);
#if FW
        [OperationContract]
#endif
        long GetSequenceNextVal(string SequenceName);
#if FW
        [OperationContract]
#endif
        DataTable GetTableColumns();
#if FW
        [OperationContract]
#endif
        string[] GetUserTables();
#if FW
        [OperationContract]
#endif
        string[] GetUserViews();
#if FW
        [OperationContract]
#endif
        string[] GetUserProcedure();
#if FW
        [OperationContract]
#endif
        string[] GetPrimarykey(string TableName);

#if FW
        [OperationContract]
#endif
        Dictionary<string, string[]> GetPrimarykey();
#if FW
        [OperationContract]
#endif
        DataTable GetUniqueIndex(string TableName);
#if FW
        [OperationContract]
#endif
        DataTable GetViewColumns(); 
#if FW
        [OperationContract]
#endif
        List<T> Select<T>(string SQL, ODAParameter[] ParamList) where T : class;
#if FW
        [OperationContract]
#endif
        List<T> Select<T>(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord,string Orderby) where T : class;
#if FW
        [OperationContract]
#endif
        List<T> Select<T>(string SQL, ODAParameter[] ParamList, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, int MaxLevel) where T : class;
#if FW
        [OperationContract(Name = "Select")]
#endif
        DataTable Select(string SQL, ODAParameter[] ParamList);
#if FW
        [OperationContract(Name = "SelectBlock")]
#endif
        DataTable Select(string SQL, ODAParameter[] ParamList, int StartIndex, int MaxRecord,string Orderby);
#if FW
        [OperationContract(Name = "SelectFirst")]
#endif
        object[] SelectFirst(string SQL, ODAParameter[] ParamList); 
#if FW
        [OperationContract(Name = "SelectRecursion")]
#endif
        DataTable Select(string SQL, ODAParameter[] ParamList, string StartWithExpress, string ConnectBy, string Prior, string ConnectColumn, string ConnectChar, int MaxLevel);
#if FW
        [OperationContract]
#endif
        DataTable GetUserProcedureArguments(string ProcedureName);
#if FW
        [OperationContract]
#endif
        bool Import(DataTable Data, ODAParameter[] Prms); 
    }
}
