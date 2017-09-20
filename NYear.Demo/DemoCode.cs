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
    public class DemoCode
    {
        static DemoCode()
        {
            ODAContext.GolbalDataBaseGroup = new DataBaseGroup()
            {
                MasterDataBase = @"Data Source=./sqlite.db",
                DBtype = ODA.DbAType.SQLite,
                SlaveDataBase = null,
                GroupID = "SQLite",
                Tables = null,
            };
            ODAContext.ExecutingSql += ODAContext_ExecutingSql;
        }

        private static void ODAContext_ExecutingSql(object source, ExecuteEventArgs args)
        {
            
        }

        public void Select1()
        {
            ODAContext ctx = new ODAContext();
            CmdPrmRole pr = ctx.GetCmd<CmdPrmRole>();
            CmdPrmRoleAuthorize ra = ctx.GetCmd<CmdPrmRoleAuthorize>();
            ra.Where(ra.ColIsForbidden == "Y", pr.ColRoleName == ra.ColRoleName);

            pr.Where(
                pr.ColRoleName == "Administrator",
                pr.Function.Exists(ra, ra.Function.VisualColumn("1", ODAdbType.OInt))
                )
                .Select(pr.ColDescript.Upper, pr.ColIsSupperAdmin.Ascii);
        }
    }
}
