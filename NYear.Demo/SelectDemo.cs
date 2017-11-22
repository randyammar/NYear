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
            var t = ctx.GetCmd<CmdTestBatchImport>();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(t.ColId.ColumnName, typeof(string)));
            dt.Columns.Add(new DataColumn(t.ColNum.ColumnName, typeof(int)));
            dt.Columns.Add(new DataColumn(t.ColTest.ColumnName, typeof(string)));

            ODAParameter[] prms = new ODAParameter[3];
            prms[0] = new ODAParameter() { ParamsName = t.ColId.ColumnName, DBDataType = ODAdbType.OVarchar };
            prms[1] = new ODAParameter() { ParamsName = t.ColNum.ColumnName, DBDataType = ODAdbType.ODecimal };
            prms[2] = new ODAParameter() { ParamsName = t.ColTest.ColumnName };

            for (int i = 0; i < 100; i++)
                dt.Rows.Add(Guid.NewGuid().ToString("N").ToUpper(), i + 1, string.Format("this is {0} Rows", i + 1));

            dt.Columns.Add("new", typeof(string), "COL_ID+COL_TEST");
            return dt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "GetDBDatetime", MethodDescript = "获取数据库时间")]
        public static object GetDBDatetime()
        {
            ODAContext ctx = new ODAContext();
            var rlt = ctx.DBDatetime;
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "SelectM", MethodDescript = "简单查询并返回数据模型")]
        public static object SelectM()
        {
            ODAContext ctx = new ODAContext();
            List<PRM_ROLE> rlt = ctx.GetCmd<CmdPrmRole>().SelectM();
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "SelectM_1", MethodDescript = "简单查询并返回数据模型")]
        public static object SelectM_1()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            List<PRM_ROLE> rlt = pr.Where(pr.ColRoleName == "Administrator")
                .SelectM(pr.ColRoleName,pr.ColIsSupperAdmin,pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Select", MethodDescript = "简单查询并返回DataTable")]
        public static object Select()
        {
            ODAContext ctx = new ODAContext();
            DataTable rlt = ctx.GetCmd<CmdPrmRole>().Select();
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Select<>", MethodDescript = "简单查询并返回泛型")]
        public static object Select_()
        {
            ODAContext ctx = new ODAContext();
            List<PRM_ROLE> rlt = ctx.GetCmd<CmdPrmRole>().Select<PRM_ROLE>();
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectFirst", MethodDescript = "获取第一条数据")]
        public static object SelectFirst()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            var rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pra.ColIsForbidden == "Y", pra.ColResourceName == "resource")
                .SelectFirst<string, string, string>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectDynamic", MethodDescript = "返回动态数据模型")]
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
        [Demo(Demo = FuncType.Select, MethodName = "ToModel", MethodDescript = "返回指定数据模型")]
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
        [Demo(Demo = FuncType.Select, MethodName = "ColumnAlias", MethodDescript = "字段别名")]
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
        [Demo(Demo = FuncType.Select, MethodName = "SelectPaging", MethodDescript = "查询分页")]
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

        [Demo(Demo = FuncType.Select, MethodName = "Join", MethodDescript = "连接查询")]
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

        [Demo(Demo = FuncType.Select, MethodName = "MultiJoin", MethodDescript = "多表连接查询")]
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
        [Demo(Demo = FuncType.Select, MethodName = "OtherInnerJoin", MethodDescript = "另一种形式的内连查询")]
        public static object OtherInnerJoin()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            CmdPrmUserRole ur = ctx.GetCmd<CmdPrmUserRole>();
            CmdPrmPermission p = ctx.GetCmd<CmdPrmPermission>();
            CmdVPrmUserAuthorize ua = ctx.GetCmd<CmdVPrmUserAuthorize>();
            List<PRM_ROLE> rlt = pr.ListCmd(pra, ur, ua, p)
                .Where(
                pr.ColRoleName == ur.ColRoleName,
                ur.ColUserId == ua.ColUserId,
                 ((p.ColResourceName == pra.ColResourceName).And(p.ColOperateName == pra.ColOperateName)).Or((ua.ColResourceName == p.ColResourceName).And(ua.ColOperateName == p.ColOperateName)),
                 pra.ColIsForbidden == "Y",
                 pra.ColResourceName == "resource"
                 )
                .Select<PRM_ROLE>(pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "OrderBy", MethodDescript = "查询排序")]
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
                .OrderbyDesc(ur.ColCreateDate, ur.ColUserId)
                .Select<PRM_ROLE>(pr.ColRoleName, ua.ColUserId, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "GroupByHaving", MethodDescript = "分组统计")]
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

        [Demo(Demo = FuncType.Select, MethodName = "Distinct", MethodDescript = "Distinct查询去重复")]
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
        [Demo(Demo = FuncType.Select, MethodName = "Function", MethodDescript = " 数据库函数")]
        public static object Function()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Where(pr.ColRoleName == "Administrator")
                .Select(pr.ColDescript.Upper, pr.ColIsSupperAdmin.Ascii, pr.Function.CreateFunc("myFunction", pr.ColRoleName, pr.ColDescript, "param0", 1).As("Funcresult"));
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Statistics ", MethodDescript = "统计运算")]
        public static object Statistics()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Where(pr.ColRoleName == "Administrator")
                .Select(pr.ColDescript.Min.As("MIN_DESCRIPT"), pr.ColRoleName.Sum.As("SUM_ROLE_NAME"));
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "VisualColumn", MethodDescript = "虚拟字段、临时字段")]
        public static object VisualColumn()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Where(pr.ColRoleName == "Administrator")
                .Select(pr.ColRoleName,pr.ColDescript,pr.ColIsSupperAdmin,pr.Function.VisualColumn("My Data").As("MyData"), pr.Function.VisualColumn("123", ODAdbType.ODecimal).As("MyCount"));
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "NullDefault", MethodDescript = " 空值转换 ")]
        public static object NullDefault()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Select(pr.Function.NullDefault(pr.ColDescript, pr.ColRoleName).As(pr.ColDescript.ColumnName),
                  pr.Function.NullDefault(pr.ColRoleName, "Administrator").As(pr.ColRoleName.ColumnName),
                  pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Decode", MethodDescript = "数据转换，与Oracle Decode函数的效果一样，ODA为了数据库通用，使用Case When 实现 ")]
        public static object Decode()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            var rlt = pr.Select(pr.Function.Decode(pr.ColDescript, pr.ColRoleName, pr.ColRoleName, pr.ColCreateBy, pr.ColIsSupperAdmin, "Administrator").As("col1"),
                  pr.Function.Decode(pr.ColDescript, "Default", "Is1", "then1", "is2", "then2", "is3", "then3").As("col2"),
                  pr.ColDescript);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "FuncCase", MethodDescript = "数据库Case [column] When语句")]
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
        [Demo(Demo = FuncType.Select, MethodName = "FuncCaseWhen", MethodDescript = "数据库Case When [条件] 语句")]
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
                  pr.Function.CaseWhen(whenThen1, "Administrator").As("col1"),
                  pr.Function.CaseWhen(whenThen2, pr.ColRoleName).As("col2"),
                  pr.ColDescript);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Where", MethodDescript = "数据库查询条件语法")]
        public static object Where()
        {
            //================================================================================
            ///语法1
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
            .Where(pr.ColRoleName == "Administrator")
            .And((pra.ColIsForbidden == "Y").Or(pra.ColIsForbidden == "N"))
            .Or(pr.ColCreateDate >= ctx.DBDatetime.AddDays(-300));

            var rlt = pr.Select(pr.ColRoleName, pr.ColDescript, pra.ColResourceName, pra.ColOperateName, pra.ColIsForbidden);

            //==================================================================================
            //语法2
            CmdPrmRole pr1 = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra1 = ctx.GetCmd<CmdPrmRoleAuthorize>();
            pr1.InnerJoin(pra1, pr1.ColRoleName == pra1.ColRoleName)
            .Where(pr1.ColRoleName == "Administrator", (pra1.ColIsForbidden == "Y").Or(pra1.ColIsForbidden == "N"))
            .Or(pr1.ColCreateDate >= ctx.DBDatetime.AddDays(-300));

            var rlt1 = pr1.Select(pr1.ColRoleName, pr1.ColDescript, pra1.ColResourceName, pra1.ColOperateName, pra1.ColIsForbidden);
            return rlt;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Condition", MethodDescript = "复杂 where 条件关系")]
        public static object Condition()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            ODAColumns operation = pra.ColOperateName == "Add";
            operation.Or(pra.ColOperateName == "Del");
            operation.Or((pra.ColOperateName == "Select").And(pra.ColOperateName == "View"));
            operation.Or(pra.ColOperateName == "update");

            List<PRM_ROLE> rlt = pr
                .InnerJoin(pra, pr.ColRoleName == pra.ColRoleName)
                .Where(pr.ColCreateDate >= System.DateTime.Now.AddDays(-30))
                .And(operation)
                .And(pr.ColRoleName.In("Administrator", "Admin", "System"))
                .Or(pra.ColIsForbidden != "N")
                .Select<PRM_ROLE>( pr.ColRoleName, pr.ColIsSupperAdmin, pr.ColDescript);
            return rlt;

        }
        [Demo(Demo = FuncType.Select, MethodName = "Like", MethodDescript = "Like 条件")]
        public static object Like()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize pra = ctx.GetCmd<CmdPrmRoleAuthorize>();

            pr.InnerJoin(pra, pr.ColRoleName == pra.ColRoleName);

            if (Guid.NewGuid().GetHashCode() % 1 == 0)
                pr.Where(pr.ColRoleName.Like("%Administrator%"));
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pr.Where(pr.ColRoleName.Like("%Admin"));
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pr.Where(pra.ColResourceName.Like("source1%"));

            if (Guid.NewGuid().GetHashCode() % 2 == 0)
                pra.Where(pra.ColOperateName == "Add");
            if (Guid.NewGuid().GetHashCode() % 2 == 1)
                pra.Where(pra.ColIsForbidden == "Y");

            var rlt = pr.Select(pr.ColRoleName, pr.ColDescript, pra.ColResourceName, pra.ColOperateName, pra.ColIsForbidden);
            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "WhereDynamic", MethodDescript = "跟据情况添加数据查询条件[ where 条件] ")]
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
 
        [Demo(Demo = FuncType.Select, MethodName = "Where Exists", MethodDescript = "Exists 子查询")]
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
        [Demo(Demo = FuncType.Select, MethodName = "Where IN", MethodDescript = "IN 子查询")]
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

        [Demo(Demo = FuncType.Select, MethodName = "SubQuery", MethodDescript = "嵌套子查询")]
        public static object SubQuery()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole rl = ctx.GetCmd<CmdPrmRole>();
            rl.Alias = "V0";
            CmdPrmRoleAuthorize rla = ctx.GetCmd<CmdPrmRoleAuthorize>();
            rla.Alias = "V1";
            CmdPrmUserRole ur = ctx.GetCmd<CmdPrmUserRole>();
            ur.Alias = "VV";

            var vwCmd = ur.InnerJoin(rl, rl.ColRoleName == rla.ColRoleName, rl.ColRoleName == "Admin")
                .InnerJoin(rla, rl.ColRoleName == ur.ColRoleName, rla.ColIsForbidden == "N")
                .Where(ur.ColUserName == "MyUserName")
                .ToView(rla.ColResourceName, rla.ColOperateName, rla.ColIsForbidden, rla.ColRoleName, ur.ColUserId, ur.ColUserName);

            CmdPrmPermission p = ctx.GetCmd<CmdPrmPermission>();

            var rlt = p.InnerJoin(vwCmd,
                  p.ColResourceName == vwCmd.CreateColumn(rla.ColResourceName.ColumnName),
                  p.ColOperateName == vwCmd.CreateColumn(rla.ColOperateName.ColumnName)
                  )
                  .Where(p.ColResourceName == "MyResource", p.ColCreateBy == "Admin", p.ColCreateDate > ctx.DBDatetime.AddDays(-300))
                  .Select(p.ColResourceName,
                  p.ColOperateName,
                  p.ColDescript,
                  vwCmd.ViewColumns[0],
                  vwCmd.ViewColumns[1],
                  vwCmd.ViewColumns[2],
                  vwCmd.ViewColumns[3]
                   );

            return rlt;
        }

        [Demo(Demo = FuncType.Select, MethodName = "RecursionSelect", MethodDescript = "递归查询,效果与oracle的StartWithConnectBy语句一致，ODA原理：先查询出来然后再递归筛选")]
        public static object RecursionSelect()
        {
            ODAContext ctx = new ODAContext();
            CmdOrgDepartment dpt = ctx.GetCmd<CmdOrgDepartment>();

            CmdOrgDepartment dpt1 = ctx.GetCmd<CmdOrgDepartment>();
            ///由于是在内存递归，所以 StartWithConnectBy使用到的所有字段必须包含在Seclect字段里

            ////由上而下递归
            var rlt = dpt.Where(dpt.ColStatue == "O")
                .StartWithConnectBy(dpt.ColDeptId.ColumnName + "='公司1'", dpt.ColParentDept.ColumnName, dpt.ColDeptId.ColumnName, "DEPT_FULL_NAME", "->", 10)
                .Select(dpt.ColDeptName.As("DEPT_FULL_NAME"),
                 dpt.ColDeptId,dpt.ColDeptName, dpt.ColParentDept,dpt.ColAssistantName,dpt.ColAssistantId,dpt.ColBossId,dpt.ColBossName);

            ////由下而上递归
            var rlt1 = dpt1.Where(dpt1.ColStatue == "O")
              .StartWithConnectBy(dpt1.ColDeptId.ColumnName + "='公司1'", dpt1.ColDeptId.ColumnName, dpt1.ColParentDept.ColumnName, "DEPT_FULL_NAME", "<-", 10)
              .Select(dpt1.ColDeptName.As("DEPT_FULL_NAME"),
               dpt1.ColDeptId, dpt1.ColDeptName, dpt1.ColParentDept, dpt1.ColAssistantName, dpt1.ColAssistantId, dpt1.ColBossId, dpt1.ColBossName);

            rlt1.Merge(rlt);
            return rlt1;
        }

        [Demo(Demo = FuncType.Select, MethodName = "ColumnCompute", MethodDescript = "字段之间的连接与运算")]
        public static object ColumnCompute()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("COL_ID", typeof(string)));
            dt.Columns.Add(new DataColumn("COL_NUM", typeof(int)));
            dt.Columns.Add(new DataColumn("COL_TEST", typeof(string)));
            dt.Columns.Add(new DataColumn("COL_NUM2", typeof(int)));

            for (int i = 0; i < 100; i++)
                dt.Rows.Add(Guid.NewGuid().ToString("N").ToUpper(), i + 1, string.Format("this is {0} Rows", i + 1),1000);

            dt.Columns.Add("CONNECT_COL", typeof(string), "COL_ID+'  +  '+COL_TEST");
            dt.Columns.Add("ADD_COL", typeof(decimal), "COL_NUM+COL_NUM2");
            return dt;
        }



        internal partial class CmdSfcBarcodeWipSerials : ORMCmd<object>
        {
            public ODAColumns ColBalanceQty { get { return new ODAColumns(this, "BALANCE_QTY", ODAdbType.ODecimal, 9); } }
            public ODAColumns ColBarcodeType { get { return new ODAColumns(this, "BARCODE_TYPE", ODAdbType.OVarchar, 80); } }
            public ODAColumns ColDatetimeCreated { get { return new ODAColumns(this, "DATETIME_CREATED", ODAdbType.ODatetime, 8); } }
            public ODAColumns ColDatetimeModified { get { return new ODAColumns(this, "DATETIME_MODIFIED", ODAdbType.ODatetime, 8); } }
            public ODAColumns ColEnterpriseId { get { return new ODAColumns(this, "ENTERPRISE_ID", ODAdbType.OVarchar, 36); } }
            public ODAColumns ColId { get { return new ODAColumns(this, "ID", ODAdbType.OVarchar, 36); } }
            public ODAColumns ColIsCompleted { get { return new ODAColumns(this, "IS_COMPLETED", ODAdbType.OVarchar, 1); } }
            public ODAColumns ColMoCode { get { return new ODAColumns(this, "MO_CODE", ODAdbType.OVarchar, 80); } }
            public ODAColumns ColOrgId { get { return new ODAColumns(this, "ORG_ID", ODAdbType.OVarchar, 36); } }
            public ODAColumns ColPrintoutCount { get { return new ODAColumns(this, "PRINTOUT_COUNT", ODAdbType.OInt, 4); } }
            public ODAColumns ColQty { get { return new ODAColumns(this, "QTY", ODAdbType.ODecimal, 9); } }
            public ODAColumns ColRunCard { get { return new ODAColumns(this, "RUN_CARD", ODAdbType.OVarchar, 480); } }
            public ODAColumns ColScheId { get { return new ODAColumns(this, "SCHE_ID", ODAdbType.OVarchar, 36); } }
            public ODAColumns ColSeq { get { return new ODAColumns(this, "SEQ", ODAdbType.OInt, 4); } }
            public ODAColumns ColSerialNumber { get { return new ODAColumns(this, "SERIAL_NUMBER", ODAdbType.OVarchar, 480); } }
            public ODAColumns ColState { get { return new ODAColumns(this, "STATE", ODAdbType.OVarchar, 1); } }
            public ODAColumns ColUserCreated { get { return new ODAColumns(this, "USER_CREATED", ODAdbType.OVarchar, 80); } }
            public ODAColumns ColUserModified { get { return new ODAColumns(this, "USER_MODIFIED", ODAdbType.OVarchar, 80); } }
            public override string CmdName { get { return "SFC_BARCODE_WIP_SERIALS"; } }
            public override DataSet Procedure(params ODAColumns[] Cols) { throw new ODAException("Not Suport Procedure CmdName " + CmdName); }
            public override List<ODAColumns> GetColumnList()
            {
                return new List<ODAColumns>() { ColBalanceQty, ColBarcodeType, ColDatetimeCreated, ColDatetimeModified, ColEnterpriseId, ColId, ColIsCompleted, ColMoCode, ColOrgId, ColPrintoutCount, ColQty, ColRunCard, ColScheId, ColSeq, ColSerialNumber, ColState, ColUserCreated, ColUserModified };
            }
        }
    }
}
