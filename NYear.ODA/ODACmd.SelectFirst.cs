//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace NYear.ODA
//{
//    public partial class ODACmd
//    {
//        public Record<T0> SelectFirst<T0>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0> rlt = new Record<T0>(
//                ChangeType<T0>(0, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1> SelectFirst<T0, T1>(params ODAColumns[] Cols)
//           where T0 : IConvertible
//           where T1 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1> rlt = new Record<T0, T1>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2> SelectFirst<T0, T1, T2>(params ODAColumns[] Cols)
//           where T0 : IConvertible
//           where T1 : IConvertible
//           where T2 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2> rlt = new Record<T0, T1, T2>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                 ChangeType<T2>(2, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3> SelectFirst<T0, T1, T2, T3>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//            where T1 : IConvertible
//            where T2 : IConvertible
//            where T3 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3> rlt = new Record<T0, T1, T2, T3>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                 ChangeType<T2>(2, items),
//                  ChangeType<T3>(3, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4> SelectFirst<T0, T1, T2, T3, T4>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//            where T1 : IConvertible
//            where T2 : IConvertible
//            where T3 : IConvertible
//            where T4 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4> rlt = new Record<T0, T1, T2, T3, T4>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5> SelectFirst<T0, T1, T2, T3, T4, T5>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5> rlt = new Record<T0, T1, T2, T3, T4, T5>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6> SelectFirst<T0, T1, T2, T3, T4, T5, T6>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6> rlt = new Record<T0, T1, T2, T3, T4, T5, T6>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        where T10 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items),
//                ChangeType<T10>(10, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        where T10 : IConvertible
//        where T11 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items),
//                ChangeType<T10>(10, items),
//                ChangeType<T11>(11, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        where T10 : IConvertible
//        where T11 : IConvertible
//        where T12 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items),
//                ChangeType<T10>(10, items),
//                ChangeType<T11>(11, items),
//                ChangeType<T12>(12, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        where T10 : IConvertible
//        where T11 : IConvertible
//        where T12 : IConvertible
//        where T13 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items),
//                ChangeType<T10>(10, items),
//                ChangeType<T11>(11, items),
//                ChangeType<T12>(12, items),
//                ChangeType<T13>(13, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        where T10 : IConvertible
//        where T11 : IConvertible
//        where T12 : IConvertible
//        where T13 : IConvertible
//        where T14 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items),
//                ChangeType<T10>(10, items),
//                ChangeType<T11>(11, items),
//                ChangeType<T12>(12, items),
//                ChangeType<T13>(13, items),
//                ChangeType<T14>(14, items)
//                );
//            return rlt;
//        }
//        public Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> SelectFirst<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        where T8 : IConvertible
//        where T9 : IConvertible
//        where T10 : IConvertible
//        where T11 : IConvertible
//        where T12 : IConvertible
//        where T13 : IConvertible
//        where T14 : IConvertible
//        where T15 : IConvertible
//        {
//            object[] items = this.SelectFirst(Cols);
//            if (items == null)
//                return null;
//            Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> rlt = new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
//                ChangeType<T0>(0, items),
//                ChangeType<T1>(1, items),
//                ChangeType<T2>(2, items),
//                ChangeType<T3>(3, items),
//                ChangeType<T4>(4, items),
//                ChangeType<T5>(5, items),
//                ChangeType<T6>(6, items),
//                ChangeType<T7>(7, items),
//                ChangeType<T8>(8, items),
//                ChangeType<T9>(9, items),
//                ChangeType<T10>(10, items),
//                ChangeType<T11>(11, items),
//                ChangeType<T12>(12, items),
//                ChangeType<T13>(13, items),
//                ChangeType<T14>(14, items),
//                ChangeType<T15>(15, items)
//                );
//            return rlt;
//        }
//    }
//}
