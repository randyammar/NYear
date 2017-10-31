using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NYear.ODA
{
    public class Record<T0>
       where T0 : IConvertible
    {
        public Record() { }
        public Record(T0 item0)
        {
            this.Item0 = item0;

        }
        public T0 Item0 { get; set; }

    }

    public class Record<T0, T1>
        where T0 : IConvertible
 where T1 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1)
        {
            this.Item0 = item0;
            this.Item1 = item1;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
    }
    public class Record<T0, T1, T2>
       where T0 : IConvertible
       where T1 : IConvertible
       where T2 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }
    public class Record<T0, T1, T2, T3>
       where T0 : IConvertible
       where T1 : IConvertible
       where T2 : IConvertible
       where T3 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4>
      where T0 : IConvertible
      where T1 : IConvertible
      where T2 : IConvertible
      where T3 : IConvertible
      where T4 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5>
        where T0 : IConvertible
        where T1 : IConvertible
        where T2 : IConvertible
        where T3 : IConvertible
        where T4 : IConvertible
        where T5 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6>
       where T0 : IConvertible
       where T1 : IConvertible
       where T2 : IConvertible
       where T3 : IConvertible
       where T4 : IConvertible
       where T5 : IConvertible
       where T6 : IConvertible

    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7>
       where T0 : IConvertible
       where T1 : IConvertible
       where T2 : IConvertible
       where T3 : IConvertible
       where T4 : IConvertible
       where T5 : IConvertible
       where T6 : IConvertible
       where T7 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>
        where T0 : IConvertible
        where T1 : IConvertible
        where T2 : IConvertible
        where T3 : IConvertible
        where T4 : IConvertible
        where T5 : IConvertible
        where T6 : IConvertible
        where T7 : IConvertible
        where T8 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
       where T0 : IConvertible
       where T1 : IConvertible
       where T2 : IConvertible
       where T3 : IConvertible
       where T4 : IConvertible
       where T5 : IConvertible
       where T6 : IConvertible
       where T7 : IConvertible
       where T8 : IConvertible
       where T9 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    where T0 : IConvertible
    where T1 : IConvertible
    where T2 : IConvertible
    where T3 : IConvertible
    where T4 : IConvertible
    where T5 : IConvertible
    where T6 : IConvertible
    where T7 : IConvertible
    where T8 : IConvertible
    where T9 : IConvertible
    where T10 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
            this.Item10 = item10;

        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
        public T10 Item10 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
        where T0 : IConvertible
        where T1 : IConvertible
        where T2 : IConvertible
        where T3 : IConvertible
        where T4 : IConvertible
        where T5 : IConvertible
        where T6 : IConvertible
        where T7 : IConvertible
        where T8 : IConvertible
        where T9 : IConvertible
        where T10 : IConvertible
        where T11 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
            this.Item10 = item10;
            this.Item11 = item11;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
        public T10 Item10 { get; set; }
        public T11 Item11 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
         where T0 : IConvertible
    where T1 : IConvertible
    where T2 : IConvertible
    where T3 : IConvertible
    where T4 : IConvertible
    where T5 : IConvertible
    where T6 : IConvertible
    where T7 : IConvertible
    where T8 : IConvertible
    where T9 : IConvertible
    where T10 : IConvertible
    where T11 : IConvertible
    where T12 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
            this.Item10 = item10;
            this.Item11 = item11;
            this.Item12 = item12;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
        public T10 Item10 { get; set; }
        public T11 Item11 { get; set; }
        public T12 Item12 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
         where T0 : IConvertible
     where T1 : IConvertible
     where T2 : IConvertible
     where T3 : IConvertible
     where T4 : IConvertible
     where T5 : IConvertible
     where T6 : IConvertible
     where T7 : IConvertible
     where T8 : IConvertible
     where T9 : IConvertible
     where T10 : IConvertible
     where T11 : IConvertible
     where T12 : IConvertible
     where T13 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
            this.Item10 = item10;
            this.Item11 = item11;
            this.Item12 = item12;
            this.Item13 = item13;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
        public T10 Item10 { get; set; }
        public T11 Item11 { get; set; }
        public T12 Item12 { get; set; }
        public T13 Item13 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
         where T0 : IConvertible
      where T1 : IConvertible
      where T2 : IConvertible
      where T3 : IConvertible
      where T4 : IConvertible
      where T5 : IConvertible
      where T6 : IConvertible
      where T7 : IConvertible
      where T8 : IConvertible
      where T9 : IConvertible
      where T10 : IConvertible
      where T11 : IConvertible
      where T12 : IConvertible
      where T13 : IConvertible
      where T14 : IConvertible
    {
        public Record() { }
        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13, T14 item14)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
            this.Item10 = item10;
            this.Item11 = item11;
            this.Item12 = item12;
            this.Item13 = item13;
            this.Item14 = item14;
        }
        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
        public T10 Item10 { get; set; }
        public T11 Item11 { get; set; }
        public T12 Item12 { get; set; }
        public T13 Item13 { get; set; }
        public T14 Item14 { get; set; }
    }
    public class Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
        where T0 : IConvertible
        where T1 : IConvertible
        where T2 : IConvertible
        where T3 : IConvertible
        where T4 : IConvertible
        where T5 : IConvertible
        where T6 : IConvertible
        where T7 : IConvertible
        where T8 : IConvertible
        where T9 : IConvertible
        where T10 : IConvertible
        where T11 : IConvertible
        where T12 : IConvertible
        where T13 : IConvertible
        where T14 : IConvertible
        where T15 : IConvertible
    {
        public Record() { }

        public Record(T0 item0, T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8, T9 item9, T10 item10, T11 item11, T12 item12, T13 item13, T14 item14, T15 item15)
        {
            this.Item0 = item0;
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
            this.Item4 = item4;
            this.Item5 = item5;
            this.Item6 = item6;
            this.Item7 = item7;
            this.Item8 = item8;
            this.Item9 = item9;
            this.Item10 = item10;
            this.Item11 = item11;
            this.Item12 = item12;
            this.Item13 = item13;
            this.Item14 = item14;
            this.Item15 = item15;
        }

        public T0 Item0 { get; set; }
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }
        public T4 Item4 { get; set; }
        public T5 Item5 { get; set; }
        public T6 Item6 { get; set; }
        public T7 Item7 { get; set; }
        public T8 Item8 { get; set; }
        public T9 Item9 { get; set; }
        public T10 Item10 { get; set; }
        public T11 Item11 { get; set; }
        public T12 Item12 { get; set; }
        public T13 Item13 { get; set; }
        public T14 Item14 { get; set; }
        public T15 Item15 { get; set; }
    }
}
