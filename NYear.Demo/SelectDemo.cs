using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NYear.ODA.Cmd;
using NYear.ODA.Model;
using NYear.ODA;

namespace NYear.Demo
{
    public class SelectDemo
    {
        

        [Demo(Demo = FuncType.Select, MethodName = "SelectM", MethodDescript = "SelectM")]
        public static void SelectM()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            List<PRM_ROLE> rlt = pr.Where(pr.ColRoleName == "Administrator").SelectM();
        }
        [Demo(Demo = FuncType.Select, MethodName = "Join", MethodDescript = "SelectJoin")]
        public static void Join()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            List<PRM_ROLE> rlt = pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
        }


        [Demo(Demo = FuncType.Select, MethodName = "ToModel", MethodDescript = "SelectToModel")]
        public static void ToModel()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
        }

        [Demo(Demo = FuncType.Select, MethodName = "Function", MethodDescript = " Function")]
        public static void Function()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            pr.Where(pr.ColRoleName == "Administrator")
                .Select(pr.ColDescript.Upper, pr.ColIsSupperAdmin.Ascii, pr.Function.CreateFunc("myFunction", ODAdbType.OVarchar, pr.ColRoleName, pr.ColDescript, "param0", 1));
        }

        [Demo(Demo = FuncType.Select, MethodName = "Where Exists", MethodDescript = "Where Exists")]
        public static void WhereExists()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            pr.Where(
                pr.ColRoleName == "Administrator",
                pr.Function.Exists(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                )
                .Select(pr.ColDescript, pr.ColIsSupperAdmin.Ascii);
        }
        [Demo(Demo = FuncType.Select, MethodName = "Where IN", MethodDescript = "Where IN")]
        public static void WhereIN()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            pr.Where(
                pr.ColRoleName == "Administrator",
                pr.ColRoleName.In(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                )
                .Select(pr.ColDescript, pr.ColIsSupperAdmin.Ascii);
        }
    }
}
