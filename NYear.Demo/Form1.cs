using NYear.ODA.Cmd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NYear.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


        }

        private void button1_Click(object sender, EventArgs e)
        {

            DemoCode dc = new DemoCode();
            dc.Select1();

            NYear.ODA.ODAContext ctx = new ODA.ODAContext();
            

            var r =  ctx.GetCmd<CmdPrmRole>();
            int total = 0;
            var m = r.Where(r.ColRoleName == "aa", r.ColDescript.Like("aaa%"))
                .Groupby(r.ColRoleName)
                .Having(r.Function.Count > 0)
                .SelectM(0, 20, out total,  r.ColRoleName,r.Function.Count.As("AAA"));

            dataGridView1.DataSource = m;
            ///0,20,out total,
        }

        private void Ctx_ExecutingSql(object source, ODA.ExecuteEventArgs args)
        {
            
        }
    }
}
