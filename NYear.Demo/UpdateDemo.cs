using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NYear.ODA.Cmd;
using NYear.ODA.Model;
using NYear.ODA;
using System.Data;
using System;

namespace NYear.Demo
{
    public class UpdateDemo
    {
        [Demo(Demo = FuncType.Update, MethodName = "Update", MethodDescript = "更新数据")]
        public static void Update()
        {
            ODAContext ctx = new ODAContext();
            CmdOrgDepartment od = ctx.GetCmd<CmdOrgDepartment>();
            od.Where(od.ColDeptName.Like( "%小队"),od.ColBossName == "我的")
                .Update(od.ColAssistantId == "美女",
                od.ColAssistantName == "王昭君",
                 od.ColBossId == "我",
                 od.ColBossName == "我的名字",
                 od.ColDeptId == "小队00",
                 od.ColDeptName == "xxx小队",
                 od.ColDeptOrg == "org",
                 od.ColParentDept == "上级"
                 );
        }
        [Demo(Demo = FuncType.Update, MethodName = "UpdateModel", MethodDescript = "模型的数据到数据库")]
        public static void UpdateModel()
        {
            ODAContext ctx = new ODAContext();
            CmdOrgDepartment od = ctx.GetCmd<CmdOrgDepartment>();
            od.Where(od.ColDeptName.Like("%小队"), od.ColBossName == "我的")
                .Update(
             new ORG_DEPARTMENT()
             {
                 ASSISTANT_ID = "美女",
                 ASSISTANT_NAME = "王昭君",
                 BOSS_ID = "我",
                 BOSS_NAME = "我的名字",
                 DEPT_ID = "小队01",
                 DEPT_NAME = "xxx小队",
                 DEPT_ORG = "org",
                 PARENT_DEPT = "上级"
             });
        }

        [Demo(Demo = FuncType.Update, MethodName = "UpdateCompute", MethodDescript = "更新运算")]
        public static void UpdateCompute()
        {
            ODAContext ctx = new ODAContext();
            CmdOrgDepartment od = ctx.GetCmd<CmdOrgDepartment>();
            od.Where(od.ColDeptName.Like("%小队"), od.ColBossName == "我的")
                .Update(od.ColAssistantId == od.ColAssistantName + "美女",////字符串连接，不能数据库通用
                 od.ColDeptId == od.ColDeptName + od.ColDeptOrg,
                 od.ColParentDept == "上级"
                 );
        } 
    }
}