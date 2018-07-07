using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NYear.ODA
{
    public partial class ODACmd
    {
        public T SelectFirst<T>(params ODAColumns[] Cols) where T : class
        {
            var sql = this.GetSelectSql(Cols);
            var db = this.GetDBAccess(sql);
            if (db == null)
                throw new ODAException(10010, "ODACmd Select 没有执行程序");
            var prms = new ODAParameter[sql.ValueList.Count + sql.WhereList.Count];
            sql.ValueList.CopyTo(prms, 0);
            sql.WhereList.CopyTo(prms, sql.ValueList.Count);
            var Tlist = db.Select<T>(sql.SqlScript.ToString(), prms, 0, 1, sql.OrderBy);
            if (Tlist != null && Tlist.Count > 0)
                return Tlist[0];
            return null;
        }
        public Record<T0, T1> SelectFirst<T0, T1>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length == 2)
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            return this.SelectFirst<Record<T0, T1>>(Cols);
        }
        public Record<T0, T1, T2> SelectFirst<T0, T1, T2>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 3)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            return this.SelectFirst<Record<T0, T1, T2>>(Cols);
        }
        public Record<T0, T1, T2, T3> SelectFirst<T0, T1, T2, T3>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 4)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            return this.SelectFirst<Record<T0, T1, T2, T3>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4> SelectFirst<T0, T1, T2, T3, T4>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 5)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5> SelectFirst<T0, T1, T2, T3, T4, T5>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 6)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6> SelectFirst<T0, T1, T2, T3, T4, T5, T6>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 7)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 8)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 9)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 10)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 11)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            Cols[10].As("Item10");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 12)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            Cols[10].As("Item10");
            Cols[11].As("Item11");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 13)
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            Cols[10].As("Item10");
            Cols[11].As("Item11");
            Cols[12].As("Item12");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 14)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            Cols[10].As("Item10");
            Cols[11].As("Item11");
            Cols[12].As("Item12");
            Cols[13].As("Item13");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 15)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            Cols[10].As("Item10");
            Cols[11].As("Item11");
            Cols[12].As("Item12");
            Cols[13].As("Item13");
            Cols[14].As("Item14");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(Cols);
        }
        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 16)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            Cols[8].As("Item8");
            Cols[9].As("Item9");
            Cols[10].As("Item10");
            Cols[11].As("Item11");
            Cols[12].As("Item12");
            Cols[13].As("Item13");
            Cols[14].As("Item14");
            Cols[15].As("Item15");
            return this.SelectFirst<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>(Cols);
        }
    }
}
