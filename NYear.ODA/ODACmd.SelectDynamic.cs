using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NYear.ODA
{
    public partial class ODACmd
    {
        public List<Record<T0>> SelectDynamic<T0>(params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 1) 
                throw new ODAException(100021, "Selecting Columns is not enough! ");
            Cols[0].As("Item0");
            return this.Select<Record<T0>>(Cols);
        }
        public List<Record<T0, T1>> SelectDynamic<T0, T1>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 2) 
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            return this.Select<Record<T0, T1>>(Cols);
        }
        public List<Record<T0, T1, T2>> SelectDynamic<T0, T1, T2>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 3)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            return this.Select<Record<T0, T1, T2>>(Cols);
        }
        public List<Record<T0, T1, T2, T3>> SelectDynamic<T0, T1, T2, T3>(params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 4 )
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            return this.Select<Record<T0, T1, T2, T3>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4>> SelectDynamic<T0, T1, T2, T3, T4>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 5 )
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            return this.Select<Record<T0, T1, T2, T3, T4>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5>> SelectDynamic<T0, T1, T2, T3, T4, T5>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 6  )
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            return this.Select<Record<T0, T1, T2, T3, T4, T5>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 7 )
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 8 )
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            Cols[7].As("Item7");
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 9 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 10 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length <11 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 12  )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 13 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 14  )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 15  )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 16  )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>( Cols);
        } 

        public List<Record<T0>> SelectDynamic<T0>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length !=1)
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");

            return this.Select<Record<T0>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1>> SelectDynamic<T0, T1>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 2)
                throw new ODAException(100021, "Selecting Columns is not enough!");
            Cols[0].As("Item0");
            Cols[1].As("Item1"); 
            return this.Select<Record<T0, T1>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2>> SelectDynamic<T0, T1, T2>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 3 )
                throw new ODAException(100021, "Selecting Columns is not enough!"); 
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            return this.Select<Record<T0, T1, T2>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3>> SelectDynamic<T0, T1, T2, T3>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 4  )
                throw new ODAException(100021, "Selecting Columns is not enough!"); 
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            return this.Select<Record<T0, T1, T2, T3>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4>> SelectDynamic<T0, T1, T2, T3, T4>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 5)
                throw new ODAException(100021, "Selecting Columns is not enough!"); 
            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4"); 
            return this.Select<Record<T0, T1, T2, T3, T4>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5>> SelectDynamic<T0, T1, T2, T3, T4, T5>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 6 )
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            return this.Select<Record<T0, T1, T2, T3, T4, T5>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 7 )
                throw new ODAException(100021, "Selecting Columns is not enough!");

            Cols[0].As("Item0");
            Cols[1].As("Item1");
            Cols[2].As("Item2");
            Cols[3].As("Item3");
            Cols[4].As("Item4");
            Cols[5].As("Item5");
            Cols[6].As("Item6");
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 9 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 9 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 10  )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 11 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null  || Cols.Length != 12 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 13 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 14 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>(StartIndex, MaxRecord, out Total, Cols);
        }
        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
        {
            if (Cols == null || Cols.Length != 16 )
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
            return this.Select<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>( StartIndex,  MaxRecord, out  Total, Cols);
        } 
    }
}
