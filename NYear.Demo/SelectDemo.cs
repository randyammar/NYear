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
    public class SelectDemo
    {
        [Demo(Demo = FuncType.Select, MethodName = "SelectM", MethodDescript = "SelectM")]
        public static object SelectM()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };

            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            List<PRM_ROLE> rlt = pr.Where(pr.ColRoleName == "Administrator").SelectM();
            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }
        [Demo(Demo = FuncType.Select, MethodName = "Join", MethodDescript = "SelectJoin")]
        public static object Join()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            List<PRM_ROLE> rlt = pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);

            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }
        [Demo(Demo = FuncType.Select, MethodName = "MultiJoin", MethodDescript = "MultiJoin")]
        public static object MultiJoin()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            CmdPrmUserRole ur = ctx.GetCmd<CmdPrmUserRole>();
            CmdPrmPermission p = ctx.GetCmd<CmdPrmPermission>();
            CmdVPrmUserAuthorize ua = ctx.GetCmd<CmdVPrmUserAuthorize>();

            List<PRM_ROLE> rlt = pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .InnerJoin(ur, pr.ColRoleName == ur.ColRoleName)
                .InnerJoin(ua, ur.ColUserId == ua.ColUserId)
                .InnerJoin(p, ((p.ColResourceName == pra.ColResourceName).And(p.ColOperateName == pra.ColOperateName)).Or((ua.ColResourceName == p.ColResourceName).And(ua.ColOperateName == p.ColOperateName)))
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }

        [Demo(Demo = FuncType.Select, MethodName = "ToModel", MethodDescript = "SelectToModel")]
        public static object ToModel()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);

            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }

        [Demo(Demo = FuncType.Select, MethodName = "Function", MethodDescript = " Function")]
        public static object Function()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Where(pr.ColRoleName == "Administrator")
                .Select(pr.ColDescript.Upper, pr.ColIsSupperAdmin.Ascii, pr.Function.CreateFunc("myFunction", ODAdbType.OVarchar, pr.ColRoleName, pr.ColDescript, "param0", 1));

            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }

        [Demo(Demo = FuncType.Select, MethodName = "Where Exists", MethodDescript = "Where Exists")]
        public static object WhereExists()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            var rlt = pr.Where(
                  pr.ColRoleName == "Administrator",
                  pr.Function.Exists(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                  )
                  .Select(pr.ColDescript, pr.ColIsSupperAdmin.Ascii);

            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }
        [Demo(Demo = FuncType.Select, MethodName = "Where IN", MethodDescript = "Where IN")]
        public static object WhereIN( )
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            var rlt = pr.Where(
                   pr.ColRoleName == "Administrator",
                   pr.ColRoleName.In(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                   )
                   .Select(pr.ColDescript, pr.ColIsSupperAdmin.Ascii);
            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }
        [Demo(Demo = FuncType.Select, MethodName = "Distinct", MethodDescript = "Distinct")]
        public static object Distinct()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            int TotalRows = 0;
            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Distinct.Select<PRM_ROLE>(0, int.MaxValue, out TotalRows, pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);

            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectFirst", MethodDescript = "Distinct")]
        public static object SelectFirst()
        {
            ODAContext ctx = new ODAContext();
            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);
            };
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            var rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Distinct.SelectFirst<string,string,string>( pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            

            return new
            {
                sql = sb.ToString(),
                data = rlt
            };
        }

        [Demo(Demo = FuncType.Select, MethodName = "whereOr", MethodDescript = "whereOr")]
        public static object whereOr()
        {
            ODAContext ctx = new ODAContext();

            StringBuilder sb = new StringBuilder();
            ctx.CurrentExecutingSql += (src, args) =>
            {
                sb.AppendLine(args.DebugSQL);

            };


            CmdSfcExecutorTmpl et = ctx.GetCmd<CmdSfcExecutorTmpl>();
            CmdSfcExecutorTmplAssemblies eta = ctx.GetCmd<CmdSfcExecutorTmplAssemblies>();
            CmdSysAssemblies a = ctx.GetCmd<CmdSysAssemblies>();
            CmdSfcRoutingStation rs = ctx.GetCmd<CmdSfcRoutingStation>();

           var  result = rs.InnerJoin(et, et.ColId == rs.ColBizTemplateId, et.ColOrgId == rs.ColOrgId)
                    .InnerJoin(eta, eta.ColTmplId == rs.ColBizTemplateId, eta.ColOrgId == rs.ColOrgId)
                    .InnerJoin(a, eta.ColAssemblyFilename == a.ColAssemblyFilename)
                    .Where(rs.ColState == "A", et.ColState == "A", eta.ColState == "A", a.ColState == "A",
                            rs.ColOrgId == "111", rs.ColStationCode == "aaaa"
                            , rs.ColRoutingRevisionId == "bbbb", et.ColTmplClass == "R",
                            (a.ColOrgId == "*").Or(a.ColOrgId == "111"))
                    .Select(et.ColTmplCode, et.ColTmplClass, et.ColTmplGroup,
                    et.ColTmplDesc, et.ColId, eta.ColBusinessCategory, eta.ColExecSeq,
                    eta.ColAssemblyFilename, a.ColAssemblyDesc, a.ColAssemblyCategory);



            return result;
        }

        private static void Ctx_CurrentExecutingSql(object source, ExecuteEventArgs args)
        {
            throw new NotImplementedException();
        }
    }

    internal partial class CmdSfcExecutorTmpl : ORMCmd<object>
    {
        public ODAColumns ColDatetimeCreated { get { return new ODAColumns(this, "DATETIME_CREATED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColDatetimeModified { get { return new ODAColumns(this, "DATETIME_MODIFIED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColEnterpriseId { get { return new ODAColumns(this, "ENTERPRISE_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColId { get { return new ODAColumns(this, "ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColOrgId { get { return new ODAColumns(this, "ORG_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColState { get { return new ODAColumns(this, "STATE", ODAdbType.OChar, 1); } }
        public ODAColumns ColTmplClass { get { return new ODAColumns(this, "TMPL_CLASS", ODAdbType.OChar, 1); } }
        public ODAColumns ColTmplCode { get { return new ODAColumns(this, "TMPL_CODE", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColTmplDesc { get { return new ODAColumns(this, "TMPL_DESC", ODAdbType.OVarchar, 2000); } }
        public ODAColumns ColTmplGroup { get { return new ODAColumns(this, "TMPL_GROUP", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserCreated { get { return new ODAColumns(this, "USER_CREATED", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserModified { get { return new ODAColumns(this, "USER_MODIFIED", ODAdbType.OVarchar, 80); } }
        public override string CmdName { get { return "SFC_EXECUTOR_TMPL"; } }
        public override DataSet Procedure(params ODAColumns[] Cols) { throw new ODAException("Not Suport Procedure CmdName " + CmdName); }
        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColDatetimeCreated, ColDatetimeModified, ColEnterpriseId, ColId, ColOrgId, ColState, ColTmplClass, ColTmplCode, ColTmplDesc, ColTmplGroup, ColUserCreated, ColUserModified };
        }
    }

    internal partial class CmdSfcExecutorTmplAssemblies : ORMCmd<object>
    {
        public ODAColumns ColAssemblyFilename { get { return new ODAColumns(this, "ASSEMBLY_FILENAME", ODAdbType.OVarchar, 2000); } }
        public ODAColumns ColBusinessCategory { get { return new ODAColumns(this, "BUSINESS_CATEGORY", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColDatetimeCreated { get { return new ODAColumns(this, "DATETIME_CREATED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColDatetimeModified { get { return new ODAColumns(this, "DATETIME_MODIFIED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColEnterpriseId { get { return new ODAColumns(this, "ENTERPRISE_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColExecSeq { get { return new ODAColumns(this, "EXEC_SEQ", ODAdbType.OInt, 4); } }
        public ODAColumns ColId { get { return new ODAColumns(this, "ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColOrgId { get { return new ODAColumns(this, "ORG_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColState { get { return new ODAColumns(this, "STATE", ODAdbType.OChar, 1); } }
        public ODAColumns ColTmplId { get { return new ODAColumns(this, "TMPL_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColUserCreated { get { return new ODAColumns(this, "USER_CREATED", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserModified { get { return new ODAColumns(this, "USER_MODIFIED", ODAdbType.OVarchar, 80); } }
        public override string CmdName { get { return "SFC_EXECUTOR_TMPL_ASSEMBLIES"; } }
        public override DataSet Procedure(params ODAColumns[] Cols) { throw new ODAException("Not Suport Procedure CmdName " + CmdName); }
        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColAssemblyFilename, ColBusinessCategory, ColDatetimeCreated, ColDatetimeModified, ColEnterpriseId, ColExecSeq, ColId, ColOrgId, ColState, ColTmplId, ColUserCreated, ColUserModified };
        }
    }

    internal partial class CmdSysAssemblies : ORMCmd<object>
    {
        public ODAColumns ColAssemblyCategory { get { return new ODAColumns(this, "ASSEMBLY_CATEGORY", ODAdbType.OVarchar, 240); } }
        public ODAColumns ColAssemblyDesc { get { return new ODAColumns(this, "ASSEMBLY_DESC", ODAdbType.OVarchar, 2000); } }
        public ODAColumns ColAssemblyFilename { get { return new ODAColumns(this, "ASSEMBLY_FILENAME", ODAdbType.OVarchar, 4000); } }
        public ODAColumns ColDatetimeCreated { get { return new ODAColumns(this, "DATETIME_CREATED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColDatetimeModified { get { return new ODAColumns(this, "DATETIME_MODIFIED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColEnterpriseId { get { return new ODAColumns(this, "ENTERPRISE_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColId { get { return new ODAColumns(this, "ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColOrgId { get { return new ODAColumns(this, "ORG_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColState { get { return new ODAColumns(this, "STATE", ODAdbType.OChar, 1); } }
        public ODAColumns ColUserCreated { get { return new ODAColumns(this, "USER_CREATED", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserModified { get { return new ODAColumns(this, "USER_MODIFIED", ODAdbType.OVarchar, 80); } }
        public override string CmdName { get { return "SYS_ASSEMBLIES"; } }
        public override DataSet Procedure(params ODAColumns[] Cols) { throw new ODAException("Not Suport Procedure CmdName " + CmdName); }
        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColAssemblyCategory, ColAssemblyDesc, ColAssemblyFilename, ColDatetimeCreated, ColDatetimeModified, ColEnterpriseId, ColId, ColOrgId, ColState, ColUserCreated, ColUserModified };
        }
    }


    internal partial class CmdSfcRoutingStation : ORMCmd<object>
    {
        public ODAColumns ColBizTemplateId { get { return new ODAColumns(this, "BIZ_TEMPLATE_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColDatetimeCreated { get { return new ODAColumns(this, "DATETIME_CREATED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColDatetimeModified { get { return new ODAColumns(this, "DATETIME_MODIFIED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColEnterpriseId { get { return new ODAColumns(this, "ENTERPRISE_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColId { get { return new ODAColumns(this, "ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColOrgId { get { return new ODAColumns(this, "ORG_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColRoutingRevisionId { get { return new ODAColumns(this, "ROUTING_REVISION_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColState { get { return new ODAColumns(this, "STATE", ODAdbType.OChar, 1); } }
        public ODAColumns ColStationCode { get { return new ODAColumns(this, "STATION_CODE", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColStationType { get { return new ODAColumns(this, "STATION_TYPE", ODAdbType.OChar, 1); } }
        public ODAColumns ColUserCreated { get { return new ODAColumns(this, "USER_CREATED", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserModified { get { return new ODAColumns(this, "USER_MODIFIED", ODAdbType.OVarchar, 80); } }
        public override string CmdName { get { return "SFC_ROUTING_STATION"; } }
        public override DataSet Procedure(params ODAColumns[] Cols) { throw new ODAException("Not Suport Procedure CmdName " + CmdName); }
        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColBizTemplateId, ColDatetimeCreated, ColDatetimeModified, ColEnterpriseId, ColId, ColOrgId, ColRoutingRevisionId, ColState, ColStationCode, ColStationType, ColUserCreated, ColUserModified };
        }
    }
}
