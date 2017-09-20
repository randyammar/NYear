using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NYear.ODA.DevTool
{
    public partial class SQLDevlop : Form
    {
        public SQLDevlop()
        {
            InitializeComponent();
        }

        private void SQLDevlop_Load(object sender, EventArgs e)
        {
            this.lbxTableView.Items.Clear();
            this.lbxTableView.Items.AddRange(CurrentDatabase.DataSource.GetUserTables());
            this.lbxTableView.Items.AddRange(CurrentDatabase.DataSource.GetUserViews());

            if (this.MdiParent is MdiParentForm)
            {
                this.MdiParent.MdiChildActivate += MdiParent_MdiChildActivate;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            this.MdiParent.MdiChildActivate -= MdiParent_MdiChildActivate;
            ((MdiParentForm)this.MdiParent).ExecuteSQL -= ExecuteSQL;
        }
 
        private void MdiParent_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.MdiParent.ActiveMdiChild == null ||  this.MdiParent.ActiveMdiChild != this)
            {
                ((MdiParentForm)this.MdiParent).ExecuteSQL -= ExecuteSQL;
            }
            else
            {
                ((MdiParentForm)this.MdiParent).ExecuteSQL += ExecuteSQL;
            }
        }
     
        private void ExecuteSQL(object sender, EventArgs e)
        {
            if (this.rtbxSql.SelectedText.Trim() == "")
                this.ExcuteSql(this.rtbxSql.Text);
            else
                this.ExcuteSql(this.rtbxSql.SelectedText);
        }


        private void lbxTableView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            rtbxSql.AppendText(lbxTableView.SelectedItem.ToString());
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.HWnd == this.rtbxSql.Handle)
            {
                if (keyData == (Keys.Control | Keys.Enter))
                {
                    ExecuteSQL(null, null);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ExcuteSql(string Sql)
        {
            string ExSql = "";
            try
            {
                if (Sql.Trim().ToUpper().StartsWith("SELECT "))
                {
                    DataTable Dt = CurrentDatabase.DataSource.Select( Sql, null,0,100);
                    this.dgvExecuteSql.DataSource = Dt;
                    this.dgvExecuteSql.Update();
                    this.dgvExecuteSql.Refresh();
                    ExSql = Sql;
                    this.tbc_ExecuteSqlResult.SelectedTab = this.tpgGrid;
                }
                else
                {
                    int i = CurrentDatabase.DataSource.ExecuteSQL( Sql, null);
                    ExSql = "執行SQL " + Sql + "\r\t 返回影響行數:  " + i.ToString();
                    this.tbc_ExecuteSqlResult.SelectedTab = this.tpgMsg;
                }
            }
            catch (Exception ex)
            {
                ExSql = ex.Message;
            }
            this.lblExecuteRlt.Text = ExSql;
        }

        private void dgvExecuteSql_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }
        //public void LanguageHighlight()
        //{
        //    rtbxSql.Text = "aa";
        //    rtbxSql.SelectionColor
        //}
    }


}
