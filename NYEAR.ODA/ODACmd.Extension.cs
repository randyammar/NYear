using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NYear.ODA
{
    //public static class JoinCmd
    //{
    //    public static ODACmd LeftJoin<T>(Func<ODACmd, ODACmd, ODAColumns> On) where T : ODACmd,new()
    //    {
    //        T c = new T();
    //        this.JoinCmd[1].JoinCmd
    //        this.LeftJoin(c, On(this, c)); 
    //        return this;
    //    }
    //}


    public static class ODAExtension
    {
        //public static ODACmd<T> GetJoinCmd<T>(this ODAContext context)
        //    where T : ODACmd, new()
        //{
        //    var jc = new JoinCmd<T>() { Context = context };
        //    jc.T0 = context.GetCmd<T>();
        //    return jc;
        //}

        public static IODAColumns[] AllColumns<T>(this T cmd)
            where T : ORMCmd<T>
        {
            var list = cmd.GetColumnList();
            return list.ToArray();
        }
        public static IODAColumns[] AllColumnsWith<T>(this T cmd, params ODAColumns[] withCols)
            where T : ORMCmd<T>
        {
            var list = cmd.GetColumnList();
            list.AddRange(withCols);
            return list.ToArray();
        }
        //public static ODAColumns[] AllColumnsExcept<T>(this T cmd, params ODAColumns[] exceptCols)
        //    where T : ORMCmd<T>
        //{
        //    var list = cmd.GetColumnList();
        //    var arr = list.Except(exceptCols, ODAColumnsEqualityComparer.Default).ToArray();
        //    return arr;
        //}

        public static T Where<T>(this T cmd, Func<ODAColumns> condition)
            where T : ODACmd
        {
            cmd.Where(condition());
            return cmd;
        }
        public static T OrderbyAsc<T>(this T cmd, Func<T, ODAColumns> col)
            where T : ODACmd
        {
            cmd.OrderbyAsc(col(cmd));
            return cmd;
        }
        public static T OrderbyDesc<T>(this T cmd, Func<T, ODAColumns> col)
            where T : ODACmd
        {
            cmd.OrderbyDesc(col(cmd));
            return cmd;
        }

        public static DataTable Select<T>(this T cmd, Func<T, ODAColumns> col)
            where T : ODACmd
        {
            return cmd.Select(col(cmd));
        }

        public static DataTable Select<T>(this T cmd, Func<T, ODAColumns[]> cols)
            where T : ODACmd
        {
            return cmd.Select(cols(cmd));
        }

        public static DataTable SelectFirstOrDefault<T>(this T cmd, Func<T, ODAColumns[]> cols, out int total)
            where T : ODACmd
        {
            return cmd.Select(0, 1, out total, cols(cmd));
        }

        public static DataTable SelectFirstOrDefault<T>(this T cmd, Func<T, ODAColumns[]> cols)
            where T : ODACmd
        {
            int total;
            return cmd.Select(0, 1, out total, cols(cmd));
        }

        public static DataTable SelectFirstOrDefault<T>(this T cmd, Func<T, ODAColumns> col, out int total)
            where T : ODACmd
        {
            return cmd.Select(0, 1, out total, col(cmd));
        }

        public static DataTable SelectFirstOrDefault<T>(this T cmd, Func<T, ODAColumns> col)
            where T : ODACmd
        {
            int total;
            return cmd.Select(0, 1, out total, col(cmd));
        }

        public static bool Update<T>(this T cmd, Func<T, ODAColumns> col)
            where T : ODACmd
        {
            return cmd.Update(col(cmd));
        }
        public static bool Update<T>(this T cmd, Func<T, IEnumerable<ODAColumns>> cols)
            where T : ODACmd
        {
            return cmd.Update(cols(cmd).ToArray());
        }
       

        /// <summary>
        /// like '%str%'
        /// </summary>
        /// <param name="col"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ODAColumns Contains(this ODAColumns col, string str)
        {
            return col.Like(string.Format("%{0}%", str));
        }
        /// <summary>
        /// like 'str%'
        /// </summary>
        /// <param name="col"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ODAColumns ContainsLeft(this ODAColumns col, string str)
        {
            return col.Like(string.Format("{0}%", str));
        }
        /// <summary>
        /// like '%str'
        /// </summary>
        /// <param name="col"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ODAColumns ContainsRight(this ODAColumns col, string str)
        {
            return col.Like(string.Format("%{0}", str));
        }

        //public static bool Import<T, TModel>(this T cmd, IEnumerable<TModel> data)
        //    where T : ODACmd
        //    where TModel : class
        //{
        //    var dt = data.ToDataTable();
        //    var columns = cmd.GetColumnList();
        //    List<ODAParameter> ps = new List<ODAParameter>(columns.Count);
        //    foreach (DataColumn col in dt.Columns)
        //    {
        //        var p = columns.Where(x => string.Equals(col.ColumnName, x.ColumnName, StringComparison.OrdinalIgnoreCase))
        //            .Select(x => new ODAParameter()
        //            {
        //                ColumnName = x.ColumnName,
        //                DBDataType = x.DBDataType,
        //                Direction = ParameterDirection.Input,
        //                ParamsName = x.ColumnName,
        //                Size = x.Size
        //            }).FirstOrDefault();
        //        if (p != null)
        //        {
        //            ps.Add(p);
        //        }
        //    }
        //    return cmd.Import(dt, ps.ToArray());
        //}
    }

}
