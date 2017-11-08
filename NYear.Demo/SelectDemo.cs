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
        [Demo(Demo = FuncType.Select, MethodName = "DataTypeTest", MethodDescript = "DataTypeTest")]
        public static object DataTypeTest()
        {
            ODAContext ctx = new ODAContext();
            CmdTestBatchImport t = ctx.GetCmd<CmdTestBatchImport>();
            var rlt = t.SelectFirst<string,decimal,string>(t.ColId, t.ColNum, t.ColTest);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectM", MethodDescript = "SelectM")]
        public static object SelectM()
        {
            ODAContext ctx = new ODAContext();
            List<PRM_ROLE> rlt = ctx.GetCmd<CmdPrmRole>().SelectM();
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "SelectM_1", MethodDescript = "SelectM_1")]
        public static object SelectM_1()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            List<PRM_ROLE> rlt = pr.Where(pr.ColRoleName == "Administrator").SelectM();
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Select", MethodDescript = "Select")]
        public static object Select()
        {
            ODAContext ctx = new ODAContext();
            DataTable rlt = ctx.GetCmd<CmdPrmRole>().Select();
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Select<>", MethodDescript = "Select<>")]
        public static object Select_()
        {
            ODAContext ctx = new ODAContext();
            List<PRM_ROLE> rlt = ctx.GetCmd<CmdPrmRole>().Select<PRM_ROLE>();
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectFirst", MethodDescript = "Distinct")]
        public static object SelectFirst()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            var rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Distinct.SelectFirst<string, string, string>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectDynamic", MethodDescript = "SelectDynamic")]
        public static object SelectDynamic()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            var rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Distinct.SelectDynamic<string, string, string>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "ToModel", MethodDescript = "SelectToModel")]
        public static object ToModel()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "ColumnAlias", MethodDescript = "ColumnAlias")]
        public static object ColumnAlias()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
           var rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select(pra.ColRoleName.As("ColumnAlias"), pr.ColIsSupperAdmin.As("AliasName_1"), pr.ColDescript.As("AliasName_2"));
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "SelectPaging", MethodDescript = "SelectPaging")]
        public static object SelectPaging()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            int PageIndex = 0;
            int PageSize = 30;
            int PageCount = 0;

            int total = 0;
            var rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Distinct.SelectDynamic<string, string, string>(PageIndex * PageSize, PageSize, out total, pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            PageCount = total / PageSize + 1;
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Join", MethodDescript = "SelectJoin")]
        public static object Join()
        {
            ODAContext ctx = new ODAContext(); 
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
  
            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt; 
        }
        [Demo(Demo = FuncType.Select, MethodName = "MultiJoin", MethodDescript = "MultiJoin")]
        public static object MultiJoin()
        {
            ODAContext ctx = new ODAContext();
            
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
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "OrderBy", MethodDescript = "OrderBy")]
        public static object OrderBy()
        {
            ODAContext ctx = new ODAContext();
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
                .OrderbyAsc(pr.ColRoleName, ua.ColUserId)
                .OrderbyDesc(ur.ColCreateDate,ur.ColUserId)
                .Select<PRM_ROLE>(pr.ColRoleName, ua.ColUserId,pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "GroupByHaving", MethodDescript = "GroupByHaving")]
        public static object GroupByHaving()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            CmdPrmUserRole ur = ctx.GetCmd<CmdPrmUserRole>();
            CmdPrmPermission p = ctx.GetCmd<CmdPrmPermission>();
            CmdVPrmUserAuthorize ua = ctx.GetCmd<CmdVPrmUserAuthorize>();
            var rlt = pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .InnerJoin(ur, pr.ColRoleName == ur.ColRoleName)
                .InnerJoin(ua, ur.ColUserId == ua.ColUserId)
                .InnerJoin(p, ((p.ColResourceName == pra.ColResourceName).And(p.ColOperateName == pra.ColOperateName)).Or((ua.ColResourceName == p.ColResourceName).And(ua.ColOperateName == p.ColOperateName)))
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Groupby(pr.ColRoleName, pra.ColResourceName, pra.ColOperateName)
                .Having(pr.Function.Count > 2)
                .Select(pr.Function.Count.As("CountData"), pr.ColRoleName, pra.ColResourceName, pra.ColOperateName);
            return rlt;
        }


        [Demo(Demo = FuncType.Select, MethodName = "Distinct", MethodDescript = "Distinct")]
        public static object Distinct()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            int TotalRows = 0;
            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .Distinct.Select<PRM_ROLE>(0, int.MaxValue, out TotalRows, pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Function", MethodDescript = " Function")]
        public static object Function()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Where(pr.ColRoleName == "Administrator")
                .Select(pr.ColDescript.Upper, pr.ColIsSupperAdmin.Ascii, pr.Function.CreateFunc("myFunction", pr.ColRoleName, pr.ColDescript, "param0", 1).As("Funcresult"));
            return rlt;
        }
        
        [Demo(Demo = FuncType.Select, MethodName = "NullDefault", MethodDescript = "NullDefault")]
        public static object NullDefault()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
          var rlt =  pr.Select(pr.Function.NullDefault(pr.ColDescript, pr.ColRoleName).As(pr.ColDescript.ColumnName),
                pr.Function.NullDefault(pr.ColRoleName, "Administrator").As(pr.ColRoleName.ColumnName),
                pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Decode", MethodDescript = "Decode")]
        public static object Decode()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Select(pr.Function.Decode(pr.ColDescript, pr.ColRoleName, pr.ColRoleName,pr.ColCreateBy,pr.ColIsSupperAdmin,"Administrator").As("col1"),
                  pr.Function.Decode(pr.ColDescript,"Default", "Is1", "then1", "is2", "then2", "is3", "then3").As("col2"),
                  pr.ColDescript);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "FuncCase", MethodDescript = "FuncCase")]
        public static object FuncCase()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
           var whenThen1 = new Dictionary<object, object>();
            whenThen1.Add(pr.ColRoleName, pr.ColRoleName);
            whenThen1.Add("when11", "then11");
            var whenThen2 = new Dictionary<object, object>();
            whenThen2.Add("when21", "then21");
            whenThen2.Add("when22", "then22");
            var rlt = pr.Select(
                  pr.Function.Case(pr.ColDescript, whenThen1, "Administrator").As("col1"),
                  pr.Function.Case(pr.ColDescript, whenThen2, pr.ColRoleName).As("col2"),
                  pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "FuncCaseWhen", MethodDescript = "FuncCaseWhen")]
        public static object FuncCaseWhen()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var whenThen1 = new Dictionary<ODAColumns, object>();
            whenThen1.Add(pr.ColRoleName.IsNull, pr.ColRoleName);
            whenThen1.Add(pr.ColRoleName == "value1", "then11");
            var whenThen2 = new Dictionary<ODAColumns, object>();
            whenThen2.Add(pr.ColRoleName == "abc", "then21");
            whenThen2.Add(pr.ColRoleName == "abcd", "then22");
            var rlt = pr.Select(
                  pr.Function.CaseWhen( whenThen1, "Administrator").As("col1"),
                  pr.Function.CaseWhen( whenThen2, pr.ColRoleName).As("col2"),
                  pr.ColDescript);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "WhereDynamic", MethodDescript = "WhereDynamic")]
        public static object WhereDynamic()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName);

            if (Guid.NewGuid().GetHashCode() % 1 == 0)
                pr.Where(pr.ColRoleName == "Administrator");
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pr.Where(pr.ColRoleName == "Admin");
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pr.Where(pra.ColResourceName == "source1");

            if (Guid.NewGuid().GetHashCode() % 2 == 0)
                pra.Where(pra.ColOperateName == "Add");
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pra.Where(pra.ColIsForbidden == "Y");

            var rlt = pr.Select(pr.ColRoleName, pr.ColDescript, pra.ColResourceName, pra.ColOperateName, pra.ColIsForbidden);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Condition", MethodDescript = "Condition")]
        public static object Condition()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            int TotalRows = 0;
            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pr.ColCreateDate >= System.DateTime.Now.AddDays(-30))
                .And(pra.ColOperateName =="Add")
                .Or(pra.ColIsForbidden != "N")
                .Select<PRM_ROLE>(0, int.MaxValue, out TotalRows, pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Like", MethodDescript = "Like")]
        public static object Like()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName);

            if (Guid.NewGuid().GetHashCode() % 1 == 0)
                pr.Where(pr.ColRoleName.Like("%Administrator%"));
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pr.Where(pr.ColRoleName.Like("%AAdmin"));
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pr.Where(pra.ColResourceName.Like("source1%"));

            if (Guid.NewGuid().GetHashCode() % 2 == 0)
                pra.Where(pra.ColOperateName == "Add");
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pra.Where(pra.ColIsForbidden == "Y");

            var rlt = pr.Select(pr.ColRoleName, pr.ColDescript, pra.ColResourceName, pra.ColOperateName, pra.ColIsForbidden);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "WhereOr", MethodDescript = "WhereOr")]
        public static object WhereOr()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
            .Where(
                (pr.ColRoleName == "Administrator").Or(pr.ColRoleName == "Admin"),
                (pra.ColIsForbidden == "Y").And(pra.ColOperateName == "Add")
                );

            var rlt = pr.Select(pr.ColRoleName, pr.ColDescript, pra.ColResourceName, pra.ColOperateName, pra.ColIsForbidden);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Where Exists", MethodDescript = "Where Exists")]
        public static object WhereExists()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            var rlt = pr.Where(
                  pr.ColRoleName == "Administrator",
                  pr.Function.Exists(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                  )
                  .Select(pr.ColDescript, pr.ColIsSupperAdmin.Ascii);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Where IN", MethodDescript = "Where IN")]
        public static object WhereIN()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            var rlt = pr.Where(
                   pr.ColRoleName == "Administrator",
                   pr.ColRoleName.In(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                   )
                   .Select(pr.ColDescript, pr.ColIsSupperAdmin.Ascii);
            return rlt;
        }
    }
}
