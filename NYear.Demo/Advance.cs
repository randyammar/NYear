using NYear.ODA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NYear.Demo
{
    public class Advance
    {
        [Demo(Demo = FuncType.Advance, MethodName = "ColumnJoin", MethodDescript = "字段连接")]
        public static object ColumnJoin()
        {
            ////字符串的连接，不同数据库的处理差异太大，ODA没有提供字符串连接的方法.
            ////但可以用户DataTable方法或通能过实体属性实现
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("COL_ID", typeof(string)));
            dt.Columns.Add(new DataColumn("COL_NUM", typeof(int)));
            dt.Columns.Add(new DataColumn("COL_TEST", typeof(string)));
            dt.Columns.Add(new DataColumn("COL_NUM2", typeof(int)));

            for (int i = 0; i < 100; i++)
                dt.Rows.Add(Guid.NewGuid().ToString("N").ToUpper(), i + 1, string.Format("this is {0} Rows", i + 1), 1000);

            dt.Columns.Add("CONNECT_COL", typeof(string), "COL_ID+'  +  '+COL_TEST");
            dt.Columns.Add("ADD_COL", typeof(decimal), "COL_NUM+COL_NUM2");
            return dt;
        }

        [Demo(Demo = FuncType.Advance, MethodName = "ConvertModel", MethodDescript = "转List")]
        public static object ConvertModel()
        {
            return null;
        }

        [Demo(Demo = FuncType.Advance, MethodName = "UserSQL", MethodDescript = "自定义SQL")]
        public static object UserSQL()
        {
            ODAContext ctx = new ODAContext();
            var sql = ctx.GetCmd<SQLCmd>();
            var data = sql.Select("SELECT * FROM SYS_USER WHERE USER_ACCOUNT = @T1", ODAParameter.CreateParam("@T1","User1"));
            return data;
        }

        [Demo(Demo = FuncType.Advance, MethodName = "Hook)", MethodDescript = "ODA钩子")] 
        public static object Hook()
        {
            return null;
        }
    }

}
