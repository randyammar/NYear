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
        [Demo(Demo = FuncType.Select, MethodName = "Select", MethodDescript = "简单查询")]
        public static object Select()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            object data = U.Where(U.ColUserAccount == "User1")
                  .And(U.ColIsLocked == "N")
                  .And(U.ColStatus == "O")
                  .And(U.ColEmailAddr.IsNotNull)  
                 .Select(U.ColUserAccount, U.ColUserPassword.As("PWD"), U.ColUserName, U.ColPhoneNo, U.ColEmailAddr); 
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "SelectM", MethodDescript = "查询默认实体")]
        public static object SelectM()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var data = U.Where(U.ColUserAccount == "User1", U.ColIsLocked == "N", U.ColStatus == "O", U.ColEmailAddr.IsNotNull)
                .SelectM(U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr);
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Select<>", MethodDescript = "查询并返回泛型")]
        public static object SelectDefine()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            object data = U.Where(U.ColUserAccount == "User1")
                  .And(U.ColIsLocked == "N")
                  .And(U.ColStatus == "O")
                  .And(U.ColEmailAddr.IsNotNull)
                 .Select<SYS_USER>(U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr);
            return data;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SelectPaging", MethodDescript = "查询分页")]
        public static object SelectPaging()
        {
            ODAContext ctx = new ODAContext(); 
            int total = 0; 
            var U = ctx.GetCmd<CmdSysUser>();
            var data = U.Where(U.ColUserAccount == "User1", U.ColIsLocked == "N", U.ColEmailAddr.IsNotNull)
            .SelectM(0,20,out total, U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr); 
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "SelectFirst", MethodDescript = "查询第一行")]
        public static object SelectFirst()
        {
            ODAContext ctx = new ODAContext(); 
            var U = ctx.GetCmd<CmdSysUser>();
            var data = U.Where(U.ColUserAccount == "User1", U.ColIsLocked == "N", U.ColEmailAddr.IsNotNull)
            .SelectDynamicFirst(U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr);

            string UserName = data.USER_NAME; ///属性 USER_NAME 与 ColUserName 的ColumnName一致，如果没有数据则返回null
            return data; 
        }

         
        [Demo(Demo = FuncType.Select, MethodName = "SelectDynamic", MethodDescript = "返回动态数据模型")]
        public static object SelectDynamic()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var data = U.Where(U.ColUserAccount == "User1", U.ColIsLocked == "N", U.ColEmailAddr.IsNotNull)
            .SelectDynamic(U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr);

            string UserName = "";
            if (data.Count > 0)
                UserName =  data[0].USER_NAME; ///与 ColUserName  的 ColumnName一致.
            return data;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Distinct", MethodDescript = "去重复")]
        public static object Distinct()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var data = U.Where( U.ColIsLocked == "N", U.ColEmailAddr.IsNotNull)
            .Distinct.Select(U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr);
            return data;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Join", MethodDescript = "连接查询")]
        public static object Join()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var R = ctx.GetCmd<CmdSysRole>();
            var UR = ctx.GetCmd<CmdSysUserRole>();

            var data = U.InnerJoin(UR, U.ColUserAccount == UR.ColUserAccount, UR.ColStatus == "O")
                .InnerJoin(R, UR.ColRoleCode == R.ColRoleCode, R.ColStatus == "O")
                .Where(U.ColStatus == "O",R.ColRoleCode == "Administrator")
                 .Select<UserDefineModel>(U.ColUserAccount.As("UserAccount"), U.ColUserName.As("UserName"),R.ColRoleCode.As("Role"), R.ColRoleName.As("RoleName"));
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "List", MethodDescript = "简单内连接")]
        public static object List()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var R = ctx.GetCmd<CmdSysRole>();
            var UR = ctx.GetCmd<CmdSysUserRole>();
            var data =  U.ListCmd(UR,R)
                .Where(U.ColUserAccount == UR.ColUserAccount, 
                 UR.ColStatus == "O",
                 UR.ColRoleCode == R.ColRoleCode,
                 R.ColStatus == "O",
                 U.ColStatus == "O",
                 R.ColRoleCode == "Administrator")
                 .Select< UserDefineModel>(U.ColUserAccount.As("UserAccount"), U.ColUserName.As("UserName"),U.ColEmailAddr.As("Email"), R.ColRoleCode.As("Role"), R.ColRoleName.As("RoleName"));

            return data;
        }

        [Demo(Demo = FuncType.Select, MethodName = "SubQuery", MethodDescript = "嵌套子查询")]
        public static object SubQuery()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var R = ctx.GetCmd<CmdSysRole>();
            var UR = ctx.GetCmd<CmdSysUserRole>();

            var UA = ctx.GetCmd<CmdSysUserAuthorization>();
            var RA = ctx.GetCmd<CmdSysRoleAuthorization>();

            var Admin = U.InnerJoin(UR, U.ColUserAccount == UR.ColUserAccount, UR.ColStatus == "O")
                .InnerJoin(R, UR.ColRoleCode == R.ColRoleCode, R.ColStatus == "O")
                .Where(U.ColStatus == "O")
                .ToView(U.ColUserAccount.As("SYS_USER"), U.ColUserName, R.ColRoleCode.As("SYS_ROLE"), R.ColRoleName); ////子查询

           var data =  Admin.InnerJoin(UA, UA.ColUserAccount == Admin.ViewColumns[1],UA.ColIsForbidden == "N")
                .InnerJoin(RA,RA.ColRoleCode == Admin.ViewColumns[2],RA.ColIsForbidden =="N") 
                .Where(Admin.ViewColumns[1] == "张三",
                Admin.ViewColumns[2] == "Administrator")
                .Select(); 
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Union", MethodDescript = "Union")]
        public static object Union()
        {
            ODAContext ctx = new ODAContext(); 
            var U = ctx.GetCmd<CmdSysUser>(); 
            var UR = ctx.GetCmd<CmdSysUserRole>();  
            var RA = ctx.GetCmd<CmdSysRoleAuthorization>();
            var RS = ctx.GetCmd<CmdSysResource>();


            var U1 = ctx.GetCmd<CmdSysUser>();
            var UA = ctx.GetCmd<CmdSysUserAuthorization>();
            var RS1 = ctx.GetCmd<CmdSysResource>();

            U.InnerJoin(UR, U.ColUserAccount == UR.ColUserAccount, UR.ColStatus == "O")
                .InnerJoin(RA, RA.ColRoleCode == UR.ColRoleCode, RA.ColStatus == "O",RA.ColIsForbidden =="N")
                .InnerJoin(RS, RS.ColId == RA.ColResourceId, RS.ColStatus == "O")
                .Where(U.ColUserAccount == "User1");

            U1.InnerJoin(UA, U1.ColUserAccount == UA.ColUserAccount, UA.ColStatus == "O")
                .InnerJoin(RS1, RS1.ColId == UA.ColResourceId, UA.ColIsForbidden == "N")
                .Where(U1.ColUserAccount == "User1");

           var data = U.Union(U1.ToView(U1.ColUserAccount, U1.ColUserName, UA.ColIsForbidden,
                RS1.ColId, RS1.ColResourceType, RS1.ColResourceScope, RS1.ColResourceLocation
                ))
                .Select(U.ColUserAccount, U.ColUserName, RA.ColIsForbidden,
                RS.ColId, RS.ColResourceType, RS.ColResourceScope, RS.ColResourceLocation
                ); 
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "OrderBy", MethodDescript = "查询排序")]
        public static object OrderBy()
        {
            ODAContext ctx = new ODAContext();

            var RS = ctx.GetCmd<CmdSysResource>();
           var datra = RS.Where(RS.ColResourceType == "WEB", RS.ColStatus == "O")
                .OrderbyAsc(RS.ColResourceIndex)
                .SelectM();
            return datra;
        }
        [Demo(Demo = FuncType.Select, MethodName = "GroupByHaving", MethodDescript = "分组统计")]
        public static object GroupByHaving()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            var UR = ctx.GetCmd<CmdSysUserRole>();
           var data =  U.InnerJoin(UR, U.ColUserAccount == UR.ColUserAccount, UR.ColStatus == "O")
              .Where(U.ColStatus == "O", UR.ColRoleCode.In("Administrator","Admin","PowerUser","User","Guest"))
              .Groupby(UR.ColRoleCode)
              .Having(U.ColUserAccount.Count > 2)
              .OrderbyAsc(U.ColUserAccount.Count)
              .Select(U.ColUserAccount.Count.As("USER_COUNT"), UR.ColRoleCode); 
            return data;
        }

        [Demo(Demo = FuncType.Select, MethodName = "Where", MethodDescript = "数据库查询条件语法")]
        public static object Where()
        {
            ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            U.Where(U.ColUserAccount == "User1");
            U.Where(U.ColIsLocked == "N");
            U.Where(U.ColEmailAddr.IsNotNull | U.ColEmailAddr == "riwfnsse@163.com" );
            U.Or(U.ColUserAccount == "User2");
             
            var data = U .SelectM( U.ColUserAccount, U.ColUserName, U.ColPhoneNo, U.ColEmailAddr);

            return data; ;
        }
        [Demo(Demo = FuncType.Select, MethodName = "Like", MethodDescript = "Like 条件")]
        public static object Like()
        {
            ODAContext ctx = new ODAContext(); 
            var R = ctx.GetCmd<CmdSysRole>(); 
            var data = R.Where(R.ColStatus == "O", R.ColRoleCode.Like("Admin%"))
                .SelectM(); 
            return data;
        }
        [Demo(Demo = FuncType.Select, MethodName = "IN/NOT IN", MethodDescript = "IN 条件")]
        public static object In()
        {
            ODAContext ctx = new ODAContext(); 
            var RA = ctx.GetCmd<CmdSysRoleAuthorization>();
            var RS = ctx.GetCmd<CmdSysResource>(); 
            ///IN 数组
            RA.Where(RA.ColIsForbidden == "N", RA.ColStatus == "O", RA.ColRoleCode.In("Administrator", "Admin", "PowerUser"));

            ///IN 子查询
            var data = RS.Where(RS.ColStatus == "O", RS.ColId.In(RA, RA.ColResourceId)) 
                .SelectM(); 
            return data;  
        }

        [Demo(Demo = FuncType.Select, MethodName = "Exists/NOT Exists", MethodDescript = "Exists 子查询")]
        public static object Exists()
        {
            ODAContext ctx = new ODAContext();
            var RA = ctx.GetCmd<CmdSysRoleAuthorization>();
            var RS = ctx.GetCmd<CmdSysResource>();

            //Exists 子查询的条件
            RA.Where(RA.ColIsForbidden == "N", RA.ColStatus == "O", RA.ColResourceId == RS.ColId); 
            var data = RS.Where(RS.ColStatus == "O", RS.Function.Exists(RA, RA.AllColumn)) 
                .SelectM();
            return data;
        }

        [Demo(Demo = FuncType.Select, MethodName = "RecursionSelect", MethodDescript = "递归查询,效果与oracle的StartWithConnectBy语句一致，ODA原理：先查询出来然后再递归筛选")]
        public static object RecursionSelect()
        {
            ODAContext ctx = new ODAContext();
            CmdSysResource RS = ctx.GetCmd<CmdSysResource>();

            CmdSysResource RS1 = ctx.GetCmd<CmdSysResource>();
            ///由于是在内存递归，所以 StartWithConnectBy使用到的所有字段必须包含在Seclect字段里

            ////由上而下递归
            var rlt = RS.Where(RS.ColStatus == "O", RS.ColResourceType == "MENU")
                .StartWithConnectBy(RS.ColParentId.ColumnName + "=''", RS.ColParentId.ColumnName, RS.ColId.ColumnName, "MENU_PATH", "->", 10)
                .Select(RS.ColResourceName.As("MENU_PATH"), RS.ColId, RS.ColParentId, RS.ColResourceName, RS.ColResourceType, RS.ColResourceScope, RS.ColResourceLocation, RS.ColResourceIndex);

            ////由下而上递归 
            var rlt1 = RS.Where(RS.ColStatus == "O", RS.ColResourceType == "MENU")
                .StartWithConnectBy(RS.ColResourceName.ColumnName + "='叶子1'", RS.ColParentId.ColumnName, RS.ColId.ColumnName, "MENU_PATH", "<-", 10)
                .Select(RS.ColResourceName.As("MENU_PATH"), RS.ColId, RS.ColParentId, RS.ColResourceName, RS.ColResourceType, RS.ColResourceScope, RS.ColResourceLocation, RS.ColResourceIndex);
            rlt1.Merge(rlt);
            return null;
        }
 

        [Demo(Demo = FuncType.Select, MethodName = "ComplexQuery", MethodDescript = "复杂查询")]
        public static object ComplexQuery()
        {
            ODAContext ctx = new ODAContext();


            return null;
        }


        [Demo(Demo = FuncType.Select, MethodName = "Lambda", MethodDescript = "Lambda语法支持")]
        public static object Lambda()
        {
            ///Lambda语法支持最多九个表连接查询
            int total = 0;
            var data = new ODAContext().GetJoinCmd<CmdSysUser>()
                .InnerJoin<CmdSysUserRole>((u, ur) => u.ColUserAccount == ur.ColUserAccount & ur.ColStatus == "O")
                 .InnerJoin<CmdSysRole>((u, ur, r) => ur.ColRoleCode == r.ColRoleCode & r.ColStatus == "O" )
                 .Where((u, ur, r) => u.ColStatus == "O" & (r.ColRoleCode == "Administrator" | r.ColRoleCode =="Admin") & u.ColIsLocked =="N" )
                 .Select<UserDefineModel>(0, 20, out total, (u, ur, r) => new IODAColumns[] { u.ColUserAccount.As("UserAccount"), u.ColUserName.As("UserName"), r.ColRoleCode.As("Role"), r.ColRoleName.As("RoleName") });
             
            return data;
        }

    }



    public class UserDefineModel
    {
        public string UserAccount{get;set;}
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
    }
}
