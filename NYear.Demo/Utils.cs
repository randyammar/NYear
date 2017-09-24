using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NYear.Demo
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DemoAttribute : Attribute
    {
        public FuncType Demo { get; set; }
        public string MethodName { get; set; }
        public string MethodDescript { get; set; }
    }
    public enum FuncType
    {
        Insert,
        Delete,
        Update,
        Select,
        Procedure,
        Customize,
        Advantage,
    }

    public class DemoMethodInfo
    {
        public FuncType DemoFunc { get; set; }
        public string MethodName { get; set; }
        public string MethodDescript { get; set; }
        public MethodInfo DemoMethod { get; set; }
    }

}
