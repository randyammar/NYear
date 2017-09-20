using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace NYear.ODA.DevTool
{
    public partial class ORMCode : Form
    {
        public ORMCode()
        {
            InitializeComponent();
        }
        private void ORMCode_Load(object sender, EventArgs e)
        {
            this.ckbx_databaseobject.Items.Clear();
            this.ckbx_databaseobject.Items.AddRange(CurrentDatabase.DataSource.GetUserTables());
            this.ckbx_databaseobject.Items.AddRange(CurrentDatabase.DataSource.GetUserViews());
            
            if (this.MdiParent is MdiParentForm)
            {
                this.MdiParent.MdiChildActivate += MdiParent_MdiChildActivate;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            this.MdiParent.MdiChildActivate -= MdiParent_MdiChildActivate;
            ((MdiParentForm)this.MdiParent).ORMCodeCreate -= btn_gena_Click;
            ((MdiParentForm)this.MdiParent).ORMCodeSave -= SaveORMCode;
        }
        private void MdiParent_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.MdiParent.ActiveMdiChild == null || this.MdiParent.ActiveMdiChild != this)
            {
                ((MdiParentForm)this.MdiParent).ORMCodeCreate -= btn_gena_Click;
                ((MdiParentForm)this.MdiParent).ORMCodeSave -= SaveORMCode;
            }
            else
            {
                ((MdiParentForm)this.MdiParent).ORMCodeCreate += btn_gena_Click;
                ((MdiParentForm)this.MdiParent).ORMCodeSave += SaveORMCode;
            }
        }

        private void btn_gena_Click(object sender, System.EventArgs e)
        {
            string[] creat = new string[this.ckbx_databaseobject.CheckedItems.Count];
            string tranSeq = "";
            for (int i = 0; i < creat.Length; i++)
            {
                creat[i] = this.ckbx_databaseobject.CheckedItems[i].ToString();
                tranSeq += ",\"" + creat[i] + "\"";
            }
            CodeGenerator codeGen = new CodeGenerator(CurrentDatabase.DataSource);
            string code = codeGen.GenerateORMBase(CurrentDatabase.DBConnectString);
            rtbx_ORMCmd.Text = code;
            string[] Code = codeGen.Generate_Code(creat);
            this.rtbx_code.Text = Code[0];
            this.rtbxModel.Text = Code[1];
            this.tbc_databaseinfo.SelectedTab = this.tpgCommand;
        }

        private void SaveORMCode(object sender, System.EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (folderDlg.ShowDialog(this) == DialogResult.OK)
            {
                if (MessageBox.Show("Create file for each Class ?", "Create file for each Class", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    string[] creat = new string[this.ckbx_databaseobject.CheckedItems.Count];
                    for (int i = 0; i < creat.Length; i++)
                        creat[i] = this.ckbx_databaseobject.CheckedItems[i].ToString();

                    CodeGenerator codeGen = new CodeGenerator(CurrentDatabase.DataSource);
                    for (int i = 0; i < creat.Length; i++)
                    {
                        string[] Code = codeGen.Generate_Code(creat[i]);
                        string CmdPascal = codeGen.Pascal(creat[i]);

                        File.WriteAllText(Path.Combine(folderDlg.SelectedPath + "\\Cmd" + CmdPascal + ".cs"), Code[0], Encoding.UTF8);
                        File.WriteAllText(Path.Combine(folderDlg.SelectedPath + "\\" + creat[i] + ".cs"), Code[1], Encoding.UTF8);
                    }
                }
                else
                {
                    if (File.Exists(Path.Combine(folderDlg.SelectedPath + "\\ORMBase.gen.cs"))
                        || File.Exists(Path.Combine(folderDlg.SelectedPath + "\\Command.gen.cs"))
                         || File.Exists(Path.Combine(folderDlg.SelectedPath + "\\Model.gen.cs"))
                        )
                    {
                        if (MessageBox.Show("File Exists,Replace it?", "File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }
                    }
                    File.WriteAllText(Path.Combine(folderDlg.SelectedPath + "\\ORMBase.gen.cs"), rtbx_ORMCmd.Text);
                    File.WriteAllText(Path.Combine(folderDlg.SelectedPath + "\\Command.gen.cs"), rtbx_code.Text);
                    File.WriteAllText(Path.Combine(folderDlg.SelectedPath + "\\Model.gen.cs"), rtbxModel.Text);
                }
            }
        }

        private void cbx_table_CheckStateChanged(object sender, System.EventArgs e)
        {
            if (sender.Equals(this.cbx_table))
            {
                if (this.cbx_table.Checked)
                {
                    this.ckbx_databaseobject.Items.AddRange(CurrentDatabase.DataSource.GetUserTables());
                }
                else
                {
                    string[] tables = CurrentDatabase.DataSource.GetUserTables();
                    for (int i = 0; i < tables.Length; i++)
                    {
                        this.ckbx_databaseobject.Items.Remove(tables[i]);
                    }
                }
            }

            if (sender.Equals(this.cbx_veiw))
            {
                if (this.cbx_veiw.Checked)
                {
                    this.ckbx_databaseobject.Items.AddRange(CurrentDatabase.DataSource.GetUserViews());
                }
                else
                {
                    string[] tables = CurrentDatabase.DataSource.GetUserViews();
                    for (int i = 0; i < tables.Length; i++)
                    {
                        this.ckbx_databaseobject.Items.Remove(tables[i]);
                    }
                }
            }
            this.ckbx_databaseobject.Refresh();
        }

        private void cbx_selectall_CheckStateChanged(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.ckbx_databaseobject.Items.Count; i++)
            {
                this.ckbx_databaseobject.SetItemCheckState(i, this.cbx_selectall.CheckState);
            }
            this.ckbx_databaseobject.Update();
            this.ckbx_databaseobject.Refresh();
        }
    }
}
