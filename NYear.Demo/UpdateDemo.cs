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
            var U = ctx.GetCmd<CmdSysUser>();
            U.Where(U.ColUserAccount == "User1", U.ColIsLocked == "N", U.ColStatus == "O", U.ColEmailAddr.IsNotNull)
             .Update(
                U.ColUserName == "新的名字", U.ColIsLocked == "Y"
                );
        }
        [Demo(Demo = FuncType.Update, MethodName = "UpdateModel", MethodDescript = "模型的数据到数据库")]
        public static void UpdateModel()
        {
            ///使用实体 Update 数据时，对于属性值为 null 的字段不作更新。
            ///这是由于在ORM组件的实际应用中，多数时候界面回传的是完整的实体对象，
            ///或者收接时使用完整的实体作为反序列化的容器，那些不需要更新的字段也在其中，而且为null.
            ODAContext ctx = new ODAContext(); 
            var U = ctx.GetCmd<CmdSysUser>();
            U.Where(U.ColUserAccount == "User1", U.ColIsLocked == "N", U.ColStatus == "O", U.ColEmailAddr.IsNotNull)
             .Update(new SYS_USER()
            {
                ADDRESS = "自由国度",
                CREATED_BY = "InsertModel",
                CREATED_DATE = DateTime.Now, 
                STATUS = "O",
                USER_ACCOUNT = "NYear1",
                USER_NAME = "多年1",
                USER_PASSWORD = "123",
                IS_LOCKED = "N",
            });

        }

        [Demo(Demo = FuncType.Update, MethodName = "UpdateCompute", MethodDescript = "更新运算")]
        public static void UpdateCompute()
        {
            ODAContext ctx = new ODAContext();
            //CmdOrgDepartment od = ctx.GetCmd<CmdOrgDepartment>();
            //od.Where(od.ColDeptName.Like("%小队"), od.ColBossName == "我的")
            //    .Update(od.ColAssistantId == od.ColAssistantName + "美女",////字符串连接，不能数据库通用
            //     od.ColDeptId == od.ColDeptName + od.ColDeptOrg,
            //     od.ColParentDept == "上级"
            //     );
        } 
    }
}