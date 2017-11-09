using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
    internal partial class CmdOrgDepartment : ORMCmd<ORG_DEPARTMENT>
    {
        public ODAColumns ColDeptId { get { return new ODAColumns(this, "DEPT_ID", ODAdbType.OVarchar, 2000, true); } }
        public ODAColumns ColParentDept { get { return new ODAColumns(this, "PARENT_DEPT", ODAdbType.OVarchar, 2000, false); } }
        public ODAColumns ColDeptOrg { get { return new ODAColumns(this, "DEPT_ORG", ODAdbType.OVarchar, 2000, false); } }
        public ODAColumns ColDeptName { get { return new ODAColumns(this, "DEPT_NAME", ODAdbType.OVarchar, 2000, false); } }
        public ODAColumns ColBossId { get { return new ODAColumns(this, "BOSS_ID", ODAdbType.OVarchar, 2000, false); } }
        public ODAColumns ColBossName { get { return new ODAColumns(this, "BOSS_NAME", ODAdbType.OVarchar, 2000, false); } }
        public ODAColumns ColAssistantId { get { return new ODAColumns(this, "ASSISTANT_ID", ODAdbType.OVarchar, 2000, false); } }
        public ODAColumns ColAssistantName { get { return new ODAColumns(this, "ASSISTANT_NAME", ODAdbType.OVarchar, 2000, false); } }

        public ODAColumns ColStatue { get { return new ODAColumns(this, "STATUS", ODAdbType.OChar, 1, false); } }
        

        public override string CmdName { get { return "ORG_DEPARTMENT"; } }
        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColDeptId, ColParentDept, ColDeptOrg, ColDeptName, ColBossId, ColBossName, ColAssistantId, ColAssistantName, ColStatue };
        }
    }

}
