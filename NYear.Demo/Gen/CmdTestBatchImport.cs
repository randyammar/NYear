using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
    internal partial class CmdTestBatchImport : ORMCmd<object>
    {
        public ODAColumns ColId { get { return new ODAColumns(this, "COL_ID", ODAdbType.OChar, 32); } }
        public ODAColumns ColNum { get { return new ODAColumns(this, "COL_NUM", ODAdbType.OInt, 64); } }

        public ODAColumns ColTest { get { return new ODAColumns(this, "COL_TEST", ODAdbType.OVarchar, 2000); } }

        public override string CmdName { get { return "TEST_BATCH_IMPORT"; } }

        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColId, ColNum, ColTest };
        }
    }
}
