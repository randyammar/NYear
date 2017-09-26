using NYear.ODA;
using NYear.ODA.Cmd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NYear.Demo
{
    public partial class Demo : Form
    {
        List<DemoMethodInfo> _DemoMethods = null;
        public Demo()
        {
            InitializeComponent();
            InitFuncType();
            _DemoMethods = GetDemoMethods();
            InitNYearODA();
        }

        private void ODAContext_ExecutingSql(object source, ODA.ExecuteEventArgs args)
        {
            rtbxSql.AppendText(args.DebugSQL);
            rtbxSql.AppendText("\r\n");
            rtbxSql.AppendText("\r\n");
        }
        private void InitNYearODA()
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
        private void InitFuncType()
        {
            string[] fts = Enum.GetNames(typeof(FuncType));
            foreach (var ft in fts)
            {
                CheckBox cbx = new CheckBox();
                cbx.Text = ft;
                cbx.CheckedChanged += Cbx_CheckedChanged;
                plFuncType.Controls.Add(cbx);
            }
        }
        private void Cbx_CheckedChanged(object sender, EventArgs e)
        {
            List<string> funcTypes = new List<string>();
            foreach (Control ctl in plFuncType.Controls)
            {
                if (ctl is CheckBox)
                {
                    if (((CheckBox)ctl).Checked)
                        funcTypes.Add(((CheckBox)ctl).Text);
                }
            }

            ShowDemoFunc(funcTypes);
        }

        private List<DemoMethodInfo> GetDemoMethods()
        {
            Type[] types = this.GetType().Assembly.GetTypes();
            List<DemoMethodInfo> DemoMethods = new List<DemoMethodInfo>();
            foreach (var tp in types)
            {
                MethodInfo[] mds = tp.GetMethods();
                foreach (var md in mds)
                {
                    if (md.IsDefined(typeof(DemoAttribute)))
                    {
                        var dmAttr = (DemoAttribute)md.GetCustomAttribute(typeof(DemoAttribute));
                        DemoMethodInfo mi = new DemoMethodInfo()
                        {
                            MethodName = dmAttr.MethodName,
                            DemoFunc = dmAttr.Demo,
                            MethodDescript = dmAttr.MethodDescript,
                            DemoMethod = md
                        };
                        DemoMethods.Add(mi);
                    }
                }
            }
            return DemoMethods;
        }

        private void ShowDemoFunc(List<string> funcTypes)
        {
            var mthds = (from mthd in _DemoMethods
                         where funcTypes.Contains(mthd.DemoFunc.ToString())
                         select mthd).ToArray();

            fplDemoFunc.Controls.Clear();
            foreach (var md in mthds)
            {
                Button btn = new Button();
                btn.Text = md.MethodName;
                btn.Click += Btn_Click;
                btn.Tag = md;
                ToolTip ti = new ToolTip();
                ti.AutoPopDelay = 5000;
                ti.InitialDelay = 1000;
                ti.ReshowDelay = 500;
                ti.ShowAlways = true;
                ti.SetToolTip(btn, md.MethodDescript);
                fplDemoFunc.Controls.Add(btn);
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            try
            {
                var md = (DemoMethodInfo)((Button)sender).Tag;
                object rtl = md.DemoMethod.Invoke(null, null);
                if (md.DemoFunc == FuncType.Select)
                    dgvData.DataSource = rtl;
            }
            catch (Exception ex)
            {
                rtbxSql.AppendText(GetInnerException(ex).Message);
                rtbxSql.AppendText("\r\n");
                rtbxSql.AppendText("\r\n");
            }
        }

        private Exception GetInnerException(Exception ex)
        {
            if (ex.InnerException == null)
                return ex;
            return GetInnerException(ex.InnerException);
        }
    }
}
