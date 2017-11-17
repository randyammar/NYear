using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NYear.ODA.Cmd;
using NYear.ODA.Model;
using NYear.ODA;
using System.Data;

namespace NYear.Demo
{
    public class InsertDemo
    {
        [Demo(Demo = FuncType.Insert, MethodName = "Insert", MethodDescript = "插入指定字段的数据")]
        public static void Insert()
        {
            ODAContext ctx = new ODAContext();
            CmdOrgDepartment od = ctx.GetCmd<CmdOrgDepartment>();
            od.Insert(od.ColAssistantId == "美女",
                od.ColAssistantName == "王昭君",
                 od.ColBossId == "我",
                 od.ColBossName == "我的名字",
                 od.ColDeptId == "小队00",
                 od.ColDeptName == "xxx小队",
                 od.ColDeptOrg == "org",
                 od.ColParentDept == "上级"
                 );
        }
        [Demo(Demo = FuncType.Insert, MethodName = "InsertModel", MethodDescript = "插入模型的数据")]
        public static void InsertModel()
        {
            ODAContext ctx = new ODAContext();
            CmdOrgDepartment od = ctx.GetCmd<CmdOrgDepartment>();
            ORG_DEPARTMENT md = new ORG_DEPARTMENT()
            {
                ASSISTANT_ID = "美女",
                ASSISTANT_NAME = "王昭君",
                BOSS_ID = "我",
                BOSS_NAME = "我的名字",
                DEPT_ID = "小队01",
                DEPT_NAME = "xxx小队",
                DEPT_ORG = "org",
                PARENT_DEPT = "上级"
            };
            od.Insert(md);
        }

        [Demo(Demo = FuncType.Insert, MethodName = "Import", MethodDescript = "大批量导入数据")]
        public static string Import()
        {
            ODAContext ctx = new ODAContext();
            var t = ctx.GetCmd<CmdTestBatchImport>();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(t.ColId.ColumnName, typeof(string)));
            dt.Columns.Add(new DataColumn(t.ColNum.ColumnName, typeof(int)));
            dt.Columns.Add(new DataColumn(t.ColTest.ColumnName, typeof(string)));

            ODAParameter[] prms = new ODAParameter[3];
            prms[0] = new ODAParameter() { ParamsName = t.ColId.ColumnName, DBDataType = ODAdbType.OVarchar };
            prms[1] = new ODAParameter() { ParamsName = t.ColNum.ColumnName, DBDataType = ODAdbType.ODecimal };
            prms[2] = new ODAParameter() { ParamsName = t.ColTest.ColumnName };

            for (int i = 0; i < 100000; i++)
                dt.Rows.Add(Guid.NewGuid().ToString("N").ToUpper(), i + 1, string.Format("this is {0} Rows", i + 1));

            ctx.BeginTransaction();
            DateTime be = DateTime.Now;
            t.Import(dt, prms);
            DateTime en = DateTime.Now;
            ctx.RollBack();

            return string.Format("Import data {0} records  from {1} to {2}", dt.Rows.Count.ToString(), be.ToString("yyyy-MM-dd HH:mm:ss.ffff"), en.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
        }
    }
}
