using NYear.ODA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NYear.Demo
{
    public class FunctionDemo
    {

        [Demo(Demo = FuncType.Function, MethodName = "Function", MethodDescript = " 数据库函数")]
        public static object Function()
        {
            ODAContext ctx = new ODAContext();

            return null;
        }
        [Demo(Demo = FuncType.Function, MethodName = "Express ", MethodDescript = "表达式")]
        public static object Express()
        {
            ODAContext ctx = new ODAContext();

            return null;
        }

        [Demo(Demo = FuncType.Function, MethodName = "UserDefined ", MethodDescript = "用户自定义的函数")]
        public static object UserDefined()
        {
            ODAContext ctx = new ODAContext();
            return null;
        }

        [Demo(Demo = FuncType.Function, MethodName = "VisualColumn", MethodDescript = "虚拟字段、临时字段")]
        public static object VisualColumn()
        {
            ODAContext ctx = new ODAContext();

            return null;
        }

        [Demo(Demo = FuncType.Function, MethodName = "NullDefault", MethodDescript = "空值转换")]
        public static object NullDefault()
        {
            ODAContext ctx = new ODAContext();
            return null;
        }
        [Demo(Demo = FuncType.Function, MethodName = "Decode", MethodDescript = "数据转内容转换")]
        public static object Decode()
        {
            ODAContext ctx = new ODAContext();
           
            return null;
        }

        [Demo(Demo = FuncType.Function, MethodName = "CaseWhen", MethodDescript = "数据转内容转换")]
        public static object CaseWhen()
        {
            ODAContext ctx = new ODAContext();
           
            return null;
        }
    }
}
