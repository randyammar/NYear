//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;

//namespace NYear.ODA
//{
//    public partial class ODACmd
//    {
//        public List<Record<T0>> SelectDynamic<T0>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0>> rlt = new List<Record<T0>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0>(this.ChangeType<T0>(0, rd.Rows[i].ItemArray)));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1>> SelectDynamic<T0, T1>(params ODAColumns[] Cols)
//           where T0 : IConvertible
//           where T1 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1>> rlt = new List<Record<T0, T1>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2>> SelectDynamic<T0, T1, T2>(params ODAColumns[] Cols)
//           where T0 : IConvertible
//           where T1 : IConvertible
//           where T2 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2>> rlt = new List<Record<T0, T1, T2>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(2, rd.Rows[i].ItemArray) 
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3>> SelectDynamic<T0, T1, T2, T3>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//            where T1 : IConvertible
//            where T2 : IConvertible
//            where T3 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3 >> rlt = new List<Record<T0, T1, T2, T3>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4>> SelectDynamic<T0, T1, T2, T3, T4>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//            where T1 : IConvertible
//            where T2 : IConvertible
//            where T3 : IConvertible
//            where T4 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4>> rlt = new List<Record<T0, T1, T2, T3, T4>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5>> SelectDynamic<T0, T1, T2, T3, T4, T5>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5>> rlt = new List<Record<T0, T1, T2, T3, T4, T5>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7>(params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        {
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray),
//                    ChangeType<T13>(13, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray),
//                    ChangeType<T13>(13, rd.Rows[i].ItemArray),
//                    ChangeType<T14>(14, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray),
//                    ChangeType<T13>(13, rd.Rows[i].ItemArray),
//                    ChangeType<T14>(14, rd.Rows[i].ItemArray),
//                    ChangeType<T15>(15, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }



//        public List<Record<T0>> SelectDynamic<T0>(int StartIndex,int MaxRecord, out int Total, params ODAColumns[] Cols)
//           where T0 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord,out Total,Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0>> rlt = new List<Record<T0>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0>(this.ChangeType<T0>(0, rd.Rows[i].ItemArray)));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1>> SelectDynamic<T0, T1>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//           where T0 : IConvertible
//           where T1 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1>> rlt = new List<Record<T0, T1>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2>> SelectDynamic<T0, T1, T2>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//           where T0 : IConvertible
//           where T1 : IConvertible
//           where T2 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2>> rlt = new List<Record<T0, T1, T2>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(2, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3>> SelectDynamic<T0, T1, T2, T3>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//            where T0 : IConvertible
//            where T1 : IConvertible
//            where T2 : IConvertible
//            where T3 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3>> rlt = new List<Record<T0, T1, T2, T3>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4>> SelectDynamic<T0, T1, T2, T3, T4>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//            where T0 : IConvertible
//            where T1 : IConvertible
//            where T2 : IConvertible
//            where T3 : IConvertible
//            where T4 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4>> rlt = new List<Record<T0, T1, T2, T3, T4>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5>> SelectDynamic<T0, T1, T2, T3, T4, T5>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5>> rlt = new List<Record<T0, T1, T2, T3, T4, T5>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
//            where T0 : IConvertible
//        where T1 : IConvertible
//        where T2 : IConvertible
//        where T3 : IConvertible
//        where T4 : IConvertible
//        where T5 : IConvertible
//        where T6 : IConvertible
//        where T7 : IConvertible
//        {
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray),
//                    ChangeType<T13>(13, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;
//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray),
//                    ChangeType<T13>(13, rd.Rows[i].ItemArray),
//                    ChangeType<T14>(14, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//        public List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> SelectDynamic<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(int StartIndex, int MaxRecord, out int Total, params ODAColumns[] Cols)
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
//            DataTable rd = this.Select(StartIndex, MaxRecord, out Total, Cols);
//            if (rd == null || rd.Rows.Count == 0)
//                return null;

//            List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> rlt = new List<Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>>();
//            for (int i = 0; i < rd.Rows.Count; i++)
//            {
//                rlt.Add(new Record<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
//                    ChangeType<T0>(0, rd.Rows[i].ItemArray),
//                    ChangeType<T1>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T2>(1, rd.Rows[i].ItemArray),
//                    ChangeType<T3>(3, rd.Rows[i].ItemArray),
//                    ChangeType<T4>(4, rd.Rows[i].ItemArray),
//                    ChangeType<T5>(5, rd.Rows[i].ItemArray),
//                    ChangeType<T6>(6, rd.Rows[i].ItemArray),
//                    ChangeType<T7>(7, rd.Rows[i].ItemArray),
//                    ChangeType<T8>(8, rd.Rows[i].ItemArray),
//                    ChangeType<T9>(9, rd.Rows[i].ItemArray),
//                    ChangeType<T10>(10, rd.Rows[i].ItemArray),
//                    ChangeType<T11>(11, rd.Rows[i].ItemArray),
//                    ChangeType<T12>(12, rd.Rows[i].ItemArray),
//                    ChangeType<T13>(13, rd.Rows[i].ItemArray),
//                    ChangeType<T14>(14, rd.Rows[i].ItemArray),
//                    ChangeType<T15>(15, rd.Rows[i].ItemArray)
//              ));
//            }
//            return rlt;
//        }
//    }
//}
