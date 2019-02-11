using NYear.ODA;
using NYear.ODA.Cmd;
using NYear.ODA.Model;
using System;
using System.Data;

namespace NYear.Demo
{
    public class InsertDemo
    {
        [Demo(Demo = FuncType.Insert, MethodName = "Insert", MethodDescript = "插入指定字段的数据")]
        public static void Insert()
        {
             
        }
        [Demo(Demo = FuncType.Insert, MethodName = "InsertModel", MethodDescript = "插入模型的数据")]
        public static void InsertModel()
        {
            
        }

        [Demo(Demo = FuncType.Insert, MethodName = "Import", MethodDescript = "大批量导入数据")]
        public static string Import()
        {
            return "";
        }
    }
}
