# NYear.ODA
## 简介
C# .Net Database ORM for Oracle DB2 MySql SqlServer SQLite MariaDB;<br/>
NYear.ODA 是一个数据库访问的 ORM 组件，能够通用常用的数据库 DB2、Oracle、SqlServer、MySql(MariaDB)、SQLite;<br/>
对不常用的数据库Informix、Sybase、Access也能简单的使用；<br/>
就目前而言，NYear.ODA 是支持 T-SQL 语法最完整的 C# ORM 组件; <br/>
分页、别名、子查询、Union、Group by、having不在话下，递归查询、case when、Decode、NullDefault、虚拟字段、数据库function、update 字段运算、表达式等。<br/><br/>
允许用户注入SQL代码段，允许用户自己编写SQL代码等，同时也支持存储过程Procedure(或oracle的包）<br/>
但ODA的功能已经足够强大了，用之开发一个完整 MES 系统或 WorkFlow【工作流】系统都不需要自定义SQL;<br/>
**注： 已有实际项目应用；Oracle、Mysql、SqlServer 三个数据库上随意切换，**<br/>
**当然，建表时需要避免各种数据库的关键字及要注意不同数据库对数据记录的大小写敏感问题。**<br/>

## NYear.ODA 原理
ODA使用的是链式编程语法，编写方式与T-SQL神似；<br/>
开发时如有迷茫之处，基本可以用SQL语句类推出ODA的对应写法。<br/>
ODA为求通用各种数据库，转换出来的SQL都是标准通用的SQL语句；一些常用但数据不兼容的部分，在ODA内部实现（如递归树查询、分页等)。 <br/>

## 用法示例
### 简单查询
` ODAContext ctx = new ODAContext();
            var U = ctx.GetCmd<CmdSysUser>();
            object data = U.Where(U.ColUserAccount == "User1")
                  .And(U.ColIsLocked == "N")
                  .And(U.ColStatus == "O")
                  .And(U.ColEmailAddr.IsNotNull)  
                 .Select(U.ColUserAccount, U.ColUserPassword.As("PWD"), U.ColUserName, U.ColPhoneNo, U.ColEmailAddr); `
