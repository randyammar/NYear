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
        public static object ColumnCompute()
        {
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
           
            return null;
        }

        [Demo(Demo = FuncType.Advance, MethodName = "Hook)", MethodDescript = "钩子")]
        public static object Hook()
        {
            return null;
        }
    }

}
