using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace NYear.ODA
{
    public interface IODACmd
    {
        string Alias { get; set; }
        GetDBAccessHandler GetDBAccess { get; set; }
        Func<string> GetAlias { get; set; }

        //ODAColumns AllColumn { get; }
        //string CmdName { get; }
        //Encoding DBCharSet { get; }
        //string DBObjectMap { get; set; }
        //ODACmd Distinct { get; }
        //IODAFunction Function { get; }

        //ODACmd And(params ODAColumns[] Cols);
        //int Count(ODAColumns Col = null);
        //bool Delete();
        //ODACmd Groupby(params ODAColumns[] ColumnNames);
        //ODACmd Having(params ODAColumns[] Params);
        //bool Import(DataTable Data, ODAParameter[] Prms);
        //ODACmd InnerJoin(ODACmd JoinCmd, params ODAColumns[] ONCols);
        //bool Insert(ODACmd SelectCmd, params ODAColumns[] Cols);
        //bool Insert(params ODAColumns[] Cols);
        //ODACmd LeftJoin(ODACmd JoinCmd, params ODAColumns[] ONCols);
        //ODACmd ListCmd(params ODACmd[] Cmds);
        //ODACmd Or(params ODAColumns[] Cols);
        //ODACmd OrderbyAsc(params ODAColumns[] ColumnNames);
        //ODACmd OrderbyDesc(params ODAColumns[] ColumnNames);
        //DataSet Procedure(params ODAColumns[] Cols);
        //ODACmd RightJoin(ODACmd JoinCmd, params ODAColumns[] ONCols);
        //DataTable Select(int StartIndex, int MaxRecord, out int TotalRecord, params ODAColumns[] Cols);
        //DataTable Select(params ODAColumns[] Cols);
        //List<T> Select<T>(int StartIndex, int MaxRecord, out int TotalRecord, params ODAColumns[] Cols) where T : class;
        //List<T> Select<T>(params ODAColumns[] Cols) where T : class;
        //List<dynamic> SelectDynamic(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols);
        //List<dynamic> SelectDynamic(params ODAColumns[] Cols);
        //dynamic SelectDynamicFirst(params ODAColumns[] Cols);
        //object[] SelectFirst(params ODAColumns[] Cols);
        //ODACmd StartWithConnectBy(string StartWithExpress, string ConnectByParent, string PriorChild, string ConnectColumn, string ConnectStr, int MaxLevel);
        //ODACmdView ToView(params ODAColumns[] Cols);
        //bool Update(params ODAColumns[] Cols);
        //ODACmd Where(params ODAColumns[] Cols);
    }
}