using System;
using System.Collections.Generic;
using System.Reflection;

namespace NYear.ODA
{
    public abstract class ORMCmd<T> : ODACmd where T : class
    {
        /// <summary>
        /// 子类重写,命令可用的变量
        /// </summary>
        /// <returns></returns>
        public abstract List<ODAColumns> GetColumnList();
        protected virtual List<ODAColumns> BindColumnValues(T Model)
        {
            PropertyInfo[] Pis = Model.GetType().GetProperties();
            List<ODAColumns> Cs = GetColumnList();
            List<ODAColumns> CList = new List<ODAColumns>();
            foreach (PropertyInfo Pi in Pis)
            {
                object V = Pi.GetValue(Model, null);
                if (V != DBNull.Value && V != null)
                    foreach (ODAColumns C in Cs)
                        if (C.ColumnName == Pi.Name)
                        {
                            C.SetCondition(CmdConditionSymbol.EQUAL, V);
                            CList.Add(C);
                        }
            }
            return CList;
        }
        public List<T> SelectM(params ODAColumns[] Cols)
        {
            return this.Select<T>(Cols); 
        }
        public List<T> SelectM(int StartIndex, int MaxRecord, out int TotalRecord, params ODAColumns[] Cols)
        {
           return this.Select<T>(StartIndex, MaxRecord, out TotalRecord, Cols);
        }
        public bool Insert(T Model)
        {
            return Insert(BindColumnValues(Model).ToArray());
        }
        public bool Update(T Model)
        {
            return Update(BindColumnValues(Model).ToArray());
        }
        public new ORMCmd<T> Distinct
        {
            get { _Distinct = true; return this; }
        }
        public new ORMCmd<T> ListCmd(params ODACmd[] Cmds)
        {
            return (ORMCmd<T>)base.ListCmd(Cmds);
        }
        public new ORMCmd<T> LeftJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            return (ORMCmd<T>)base.LeftJoin(JoinCmd, ONCols);
        }
        public new ORMCmd<T> InnerJoin(ODACmd JoinCmd, params ODAColumns[] ONCols)
        {
            return (ORMCmd<T>)base.InnerJoin(JoinCmd, ONCols);
        }
        public new ORMCmd<T> StartWithConnectBy(string StartWithExpress, string ConnectByParent, string PriorChild, string ConnectColumn, string ConnectStr, int MaxLevel)
        {
            return (ORMCmd<T>)base.StartWithConnectBy(StartWithExpress, ConnectByParent, PriorChild, ConnectColumn, ConnectStr, MaxLevel);
        }
        public new ORMCmd<T> OrderbyAsc(params ODAColumns[] ColumnNames)
        {
            return (ORMCmd<T>)base.OrderbyAsc(ColumnNames);
        }
        public new ORMCmd<T> OrderbyDesc(params ODAColumns[] ColumnNames)
        {
            return (ORMCmd<T>)base.OrderbyDesc(ColumnNames);
        }
        public new ORMCmd<T> Groupby(params ODAColumns[] ColumnNames)
        {
            return (ORMCmd<T>)base.Groupby(ColumnNames);
        }
        public new ORMCmd<T> Having(params ODAColumns[] Params)
        {
            return (ORMCmd<T>)base.Having(Params);
        }
        public new ORMCmd<T> Where(params ODAColumns[] Cols)
        {
            return (ORMCmd<T>)base.Where(Cols);
        }
        public new ORMCmd<T> Or(params ODAColumns[] Cols)
        {
            return (ORMCmd<T>)base.Or(Cols);
        }
    }
}
