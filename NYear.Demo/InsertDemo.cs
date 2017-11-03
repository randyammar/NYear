using NYear.ODA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYear.Demo
{
    public class InsertDemo
    {
        [Demo(Demo = FuncType.Insert, MethodName = "Import", MethodDescript = "Import")]
        public static void Import()
        {
            ODAContext ctx = new ODAContext();
           var t =  ctx.GetCmd<CmdTestBatchImport>();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(t.ColId.ColumnName, typeof(string)));
            dt.Columns.Add(new DataColumn(t.ColNum.ColumnName, typeof(int)));
            dt.Columns.Add(new DataColumn(t.ColTest.ColumnName, typeof(string)));

            ODAParameter[] prms = new ODAParameter[3];
            prms[0] = new ODAParameter()
            {
                ParamsName = t.ColId.ColumnName
            };
            prms[1] = new ODAParameter()
            {
                ParamsName = t.ColNum.ColumnName,
            };
            prms[2] = new ODAParameter()
            {
                ParamsName = t.ColTest.ColumnName,
            };

            for (int i = 0; i < 100000; i++)
                dt.Rows.Add(Guid.NewGuid().ToString("N").ToUpper(), i + 1, string.Format("this is {0} Rows", i + 1));


            DateTime be = DateTime.Now;
            t.Import(dt, prms);
            DateTime en = DateTime.Now;
        }
    }

    internal partial class CmdTestBatchImport : ORMCmd<object>
    {
        public ODAColumns ColId { get { return new ODAColumns(this, "COL_ID", ODAdbType.OChar, 32); } }
        public ODAColumns ColNum { get { return new ODAColumns(this, "COL_NUM", ODAdbType.OInt, 64); } }
 
        public ODAColumns ColTest{ get { return new ODAColumns(this, "COL_TEST", ODAdbType.OVarchar, 2000); } }

        public override string CmdName { get { return "TEST_BATCH_IMPORT"; } }

        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColId, ColNum, ColTest };
        }
    }

}
