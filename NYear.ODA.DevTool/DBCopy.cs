using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace NYear.ODA.DevTool
{
    public partial class DBCopy : Form
    {
        public ODA.DBAccess TarDB = null;
        public DBCopy()
        {
            InitializeComponent();
        }

        private void cbx_selectall_CheckStateChanged(object sender, System.EventArgs e)
        {
            for (int i = 0; i < this.ckbxDatabaseobject.Items.Count; i++)
            {
                this.ckbxDatabaseobject.SetItemCheckState(i, this.cbx_selectall.CheckState);
            }
            this.ckbxDatabaseobject.Update();
            this.ckbxDatabaseobject.Refresh();
        }

        private void DBCopy_Load(object sender, EventArgs e)
        {
            this.ckbxDatabaseobject.Items.Clear();
            this.ckbxDatabaseobject.Items.AddRange(CurrentDatabase.DataSource.GetUserTables());

            if (this.MdiParent is MdiParentForm)
            {
                this.MdiParent.MdiChildActivate += MdiParent_MdiChildActivate;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            this.MdiParent.MdiChildActivate -= MdiParent_MdiChildActivate;
            ((MdiParentForm)this.MdiParent).DBConnectTest -= btn_connect_Click;
            ((MdiParentForm)this.MdiParent).DBCopy -= button1_Click;
        }
        private void MdiParent_MdiChildActivate(object sender, EventArgs e)
        {
            if (this.MdiParent.ActiveMdiChild == null || this.MdiParent.ActiveMdiChild != this)
            {
                ((MdiParentForm)this.MdiParent).DBConnectTest -= btn_connect_Click;
                ((MdiParentForm)this.MdiParent).DBCopy -= button1_Click;
            }
            else
            {
                ((MdiParentForm)this.MdiParent).DBConnectTest += btn_connect_Click;
                ((MdiParentForm)this.MdiParent).DBCopy += button1_Click;
            }
        }
        private void btn_connect_Click(object sender, EventArgs e)
        {
            string ConMsg = TarDBConnect();
            lblExecuteRlt.Text = ConMsg;
            if (TarDB != null)
                ckbxTarDB.Items.AddRange(TarDB.GetUserTables());
            this.tbc_ExecuteSqlResult.SelectedTab = this.tpgMsg;
        }

        private void cbbx_database_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (this.cbbx_database.Text == "DbAReflection")
            {
                this.lblExecuteRlt.Text = "使用反射建立DbAccess,dll 文件必需有一個類繼承ODA.DataSource.DbAccess, 而且程序集必需標注ODA.AssemblyOdaAttribute屬性";
            }
            else
            {
                switch (this.cbbx_database.Text)
                {
                    case "MsSQL":
                        tbx_connectstring.Text = "server=localhost;database=master;uid=sa;pwd=sa;";
                        this.lblExecuteRlt.Text = "server=localhost;database=master;uid=sa;pwd=sa;";
                        break;
                    case "MySql":
                        tbx_connectstring.Text = "Server=localhost;Database=; User=root;Password=;Use Procedure Bodies=false;Charset=utf8;Allow Zero Datetime=True; Pooling=false; Max Pool Size=50;Port=3306;";
                        this.lblExecuteRlt.Text = "Server=localhost;Database=; User=root;Password=;Use Procedure Bodies=false;Charset=utf8;Allow Zero Datetime=True; Pooling=false; Max Pool Size=50;Port=3306;";
                        break;
                    case "OdbcInformix":
                        tbx_connectstring.Text = "DSN=;User ID=;PWD=";
                        this.lblExecuteRlt.Text = "DSN=;User ID=;PWD=";
                        break;
                    case "OledbAccess":
                        tbx_connectstring.Text = " Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\\directory\\demo.mdb;User Id=admin;Password=; ";
                        this.lblExecuteRlt.Text = " Access建立时自动添加了系统表。但Access默认是不显示的，要想看到这些表，得手动设置一下：选择菜单“工具”－“选项”－“视图”，在“系统对象”前面打勾，就能看到如下七个表了： "
                            + " MSysAccessObjects、MSysAccessXML、MSysAces、MSysImexColumns、MSysObjects、MSysQueries、MSysRelationShips"
                            + " 看是看到了，但还不能读取表里的数据，还需要设置权限：选择菜单“工具”－“安全”－“用户与组的权限”，把这些表的读写权限都勾上，OK！一切尽在掌握了，想怎么用就怎么用。"
                            //+ " 遗憾的是，微软并没给出这些表的文档说明，具体功能也只好望文生义了。较常用的MSysObjects表，很显然储存的是一些对象，里面包含了两个字段Name和Type，可以依靠它们来判断数某个表或某个查询是否存在。"
                            //+ " 例：SELECT [Name] FROM [MSysObjects] WHERE (Left([Name],1)<>'~') AND ([Type]=1)  ORDER BY [Name] "
                            //+ " 其中已知的Type值和对应的类型分别是：1－表；5－查询；-32768－窗体；-32764－报表；-32761－模块；-32766－宏。"
                            + " Connect String: Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\\directory\\demo.mdb;User Id=admin;Password=;  ";
                        break;
                    case "Oracle":
                        tbx_connectstring.Text = "password=;user id=;data source=;";
                        this.lblExecuteRlt.Text = "password=;user id=;data source=;";
                        break;
                    case "Sybase":
                        tbx_connectstring.Text = "Data Source='myASEserver'; Port=5000; Database='myDBname'; UID='username'; PWD='password'; ";
                        this.lblExecuteRlt.Text = "Data Source='myASEserver'; Port=5000; Database='myDBname'; UID='username'; PWD='password'; ";
                        break;
                    case "DbAOledb":
                        this.lblExecuteRlt.Text = " Access:Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\\directory\\demo.mdb;User Id=admin;Password=; \r\n"
                        + " DB2:Provider=IBMDADB2;Database=demodeb;HOSTNAME=myservername;PROTOCOL=TCPIP;PORT=50000;uid=myusername;pwd=mypasswd;\r\n"
                        + " DBase:Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\directory;Extended Properties=dBASE IV;User ID=Admin;Password= \r\n"
                        + " Excel:Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\\directory;Extended Properties=dBASE IV;User ID=Admin;Password= \r\n"
                        + " Exchange:oConn.Provider = \"EXOLEDB.DataSource\" oConn.Open = \"http://myServerName/myVirtualRootName\" \r\n"
                        + " Firebird:User=SYSDBA;Password=mypasswd;Database=demo.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0 \r\n"
                        + " FoxPro:Provider=vfpoledb.1;Data Source=c:\\directory\\demo.dbc;Collating Sequence=machine \r\n"
                        + " Informix:Provider=Ifxoledbc.2;User ID=myusername;password=mypasswd;Data Source=demodb@demoservername;Persist Security Info=true \r\n"
                        + " MySQL:Provider=MySQLProv;Data Source=mydemodb;User Id=myusername;Password=mypasswd;  \r\n"
                        + " Oracle:Provider=msdaora;Data Source=mydemodb;User Id=myusername;Password=mypasswd; \r\n"
                        + " SQL Server:Provider=sqloledb;Data Source=myservername;Initial Catalog=mydemodb;User Id=myusername;Password=mypasswd; \r\n"
                        + " Sybase:Provider=Sybase.ASEOLEDBProvider;Server Name=myservername,5000;Initial Catalog=mydemodb;User Id=myusername;Password=mypassword  "
                        + " 以上信息來自:http://blog.csdn.net/yufei_yxd/archive/2010/04/21/5510593.aspx";
                        break;
                    case "SQLite":
                        tbx_connectstring.Text = "Data Source=./sqlite.db";
                        this.lblExecuteRlt.Text = "Data Source=./sqlite.db;Version=3;UseUTF16Encoding=True;Password=myPassword;Legacy Format=True;"
                        + " Pooling=False;Max Pool Size=100;Read Only=True;DateTimeFormat=Ticks;BinaryGUID=False;Cache Size=2000;Page Size=1024;\r\n"
                            + "在FrameWork4.0上运行须要在<configuration>节点下加入以下配置： <startup useLegacyV2RuntimeActivationPolicy=\"true\" > "
                            + " <supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.0\"/> "
                            + " <requiredRuntime Version=\"v4.0.20506\"/>"
                            + " </startup>";
                        break;
                    case "DB2":
                        break;
                }
            }
        }

        private string TarDBConnect()
        {
            try
            {
                switch (this.cbbx_database.Text)
                {
                    case "MsSQL":
                        TarDB = new ODA.Adapter.DbAMsSQL(this.tbx_connectstring.Text);
                        break;
                    case "MySql":
                        TarDB = new ODA.Adapter.DbAMySql(this.tbx_connectstring.Text);
                        break;
                    case "OdbcInformix":
                        TarDB = new ODA.Adapter.DbAOdbcInformix(this.tbx_connectstring.Text);
                        break;
                    case "OledbAccess":
                        TarDB = new ODA.Adapter.DbAOledbAccess(this.tbx_connectstring.Text);
                        break;
                    case "Oracle":
                        TarDB = new ODA.Adapter.DbAOracle(this.tbx_connectstring.Text);
                        break;
                    case "Sybase":
                        TarDB = new ODA.Adapter.DbASybase(this.tbx_connectstring.Text);
                        break;
                    case "SQLite":
                        TarDB = new ODA.Adapter.DbASQLite(this.tbx_connectstring.Text);
                        break;
                    case "DB2":
                        TarDB = new ODA.Adapter.DbADB2(this.tbx_connectstring.Text);
                        break;
                    default:
                        break;
                }
                return "Connect Success";
            }
            catch (Exception ex)
            {
                TarDB = null;
                return ex.Message;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string ConMsg = null;
            if (TarDB == null)
                TarDBConnect();
            if (TarDB != null)
            {
                TransferParams prm = new TransferParams();
                prm.SourceDB = CurrentDatabase.DataSource;
                prm.TargetDB = TarDB;
                prm.NeedTransData = cbxTransData.Checked;
                prm.NeedTransTable = cbxCreateTable.Checked;
                prm.SrcTables = CurrentDatabase.DataSource.GetTableColumns();
                prm.TranTable = new List<string>();

                for (int i = 0; i < this.ckbxDatabaseobject.CheckedItems.Count; i++)
                {
                    string TableName = this.ckbxDatabaseobject.CheckedItems[i].ToString();
                    prm.TranTable.Add(TableName);
                }

                BackgroundWorker bgw = new BackgroundWorker();
                bgw.WorkerReportsProgress = true;
                bgw.WorkerSupportsCancellation = true;
                bgw.ProgressChanged += bgw_ProgressChanged;
                bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
                bgw.DoWork += bgw_DoWork;
                bgw.RunWorkerAsync(prm);

            }
            else
            {
                lblExecuteRlt.Text = "无法连接目标数据库," + ConMsg;
                this.tbc_ExecuteSqlResult.SelectedTab = this.tpgMsg;
            }
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder sbrlt = new StringBuilder();
            TransferParams prm = e.Argument as TransferParams;
            BackgroundWorker bgw = sender as BackgroundWorker;
            for (int i = 0; i < prm.TranTable.Count; i++)
            {
                ReportStatus RS = new ReportStatus()
                {
                    Percent = i * 100 / prm.TranTable.Count,
                    TransObject = prm.TranTable[i],
                    TransType = "Table"
                };
                bgw.ReportProgress(RS.Percent, RS);
                if (prm.NeedTransTable)
                {
                    DataRow[] drs = prm.SrcTables.Select("TABLE_NAME ='" + prm.TranTable[i] + "'");
                    if (drs == null || drs.Length == 0)
                        continue;

                    DatabaseColumnInfo[] ColumnInfo = new DatabaseColumnInfo[drs.Length];
                    for (int j = 0; j < drs.Length; j++)
                    {
                        ColumnInfo[j] = prm.TargetDB.ODAColumnToOrigin(drs[j]["COLUMN_NAME"].ToString(), drs[j]["ODA_DATATYPE"].ToString().Trim(), decimal.Parse(drs[j]["LENGTH"].ToString().Trim()));
                        ColumnInfo[j].NotNull = drs[j]["NOT_NULL"].ToString() == "Y";
                    }

                    string[] Pkeys = prm.SourceDB.GetPrimarykey(prm.TranTable[i]);
                    DatabaseColumnInfo[] pInfo = null;
                    if (Pkeys != null && Pkeys.Length > 0)
                    {
                        pInfo = new DatabaseColumnInfo[Pkeys.Length];
                        for (int k = 0; k < Pkeys.Length; k++)
                        {
                            pInfo[k] = prm.TargetDB.ODAColumnToOrigin(Pkeys[k], ODAdbType.OChar.ToString(), 10);
                        }
                    }

                    string tlt = CreateTable(prm.TargetDB, prm.TranTable[i], ColumnInfo, pInfo);
                    if (!string.IsNullOrWhiteSpace(tlt))
                    {
                        sbrlt.AppendLine(tlt);
                        continue;
                    }
                }

                ReportStatus RST = new ReportStatus()
                {
                    Percent = (i + 1) * 100 / prm.TranTable.Count,
                    TransObject = "Table [" + prm.TranTable[i] + "] Created",
                    TransType = "Table"
                };
                bgw.ReportProgress(RS.Percent, RST);

                if (prm.NeedTransData)
                {
                    DataTable targetTablesInfo = prm.TargetDB.GetTableColumns();
                    DataRow[] drs = targetTablesInfo.Select("TABLE_NAME ='" + prm.TranTable[i] + "'");

                    string insertSQL = "INSERT INTO " + prm.TranTable[i];
                    string col = "";
                    string iprms = "";
                    string paramMark = prm.TargetDB.ParamsMark;
                    for (int c = 0; c < drs.Length; c++)
                    {
                        DatabaseColumnInfo ColumnInfo = prm.TargetDB.ODAColumnToOrigin(drs[c]["COLUMN_NAME"].ToString(), drs[c]["ODA_DATATYPE"].ToString().Trim(), decimal.Parse(drs[c]["LENGTH"].ToString().Trim()));

                        col += ColumnInfo.Name + ",";
                        iprms += paramMark + "P_" + this.MD5_16(drs[c]["COLUMN_NAME"].ToString()) + ",";
                    }

                    insertSQL += "(" + col.TrimEnd(',') + ") VALUES (" + iprms.TrimEnd(',') + ")";
                    int total = 1001;
                    int maxR = 10000;
                    int startIndx = 0;
                    do
                    {
                        ReportStatus RSData0 = new ReportStatus()
                        {
                            Percent = startIndx * 100 / total,
                            TransObject = "Preparing " + startIndx.ToString() + " ~ " + (startIndx + maxR).ToString() + " record ",
                            TransType = "Data"
                        };
                        bgw.ReportProgress(RS.Percent, RSData0);

                        DataTable DT_total = CurrentDatabase.DataSource.Select("SELECT COUNT(*) FROM " + prm.TranTable[i], null);
                        int.TryParse(DT_total.Rows[0][0].ToString(), out total);
                        DataTable Source = CurrentDatabase.DataSource.Select("SELECT * FROM " + prm.TranTable[i], null, startIndx, maxR);

                        TarDB.BeginTransaction();
                        try
                        {
                            for (int k = 0; k < Source.Rows.Count; k++)
                            {
                                ODAParameter[] Oprms = new ODAParameter[drs.Length];
                                for (int c = 0; c < drs.Length; c++)
                                {
                                    Oprms[c] = new ODAParameter();
                                    Oprms[c].DBDataType = (ODAdbType)Enum.Parse(typeof(ODAdbType), drs[c]["ODA_DATATYPE"].ToString());
                                    Oprms[c].Direction = ParameterDirection.Input;
                                    Oprms[c].ParamsName = paramMark + "P_" + this.MD5_16(drs[c]["COLUMN_NAME"].ToString());
                                    Oprms[c].Size = int.Parse(drs[c]["LENGTH"].ToString().Trim());
                                    if (Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()] == System.DBNull.Value)
                                    {
                                        Oprms[c].ParamsValue = System.DBNull.Value;
                                    }
                                    else
                                    {
                                        switch (Oprms[c].DBDataType)
                                        {
                                            case ODAdbType.OArrary:
                                            case ODAdbType.OBinary:
                                                Oprms[c].ParamsValue = NYear.ODA.DBAccess.DataConvert(Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()], typeof(byte[]));
                                                break;
                                            case ODAdbType.OChar:
                                            case ODAdbType.OVarchar:
                                                Oprms[c].ParamsValue = NYear.ODA.DBAccess.DataConvert(Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()], typeof(string));
                                                break;
                                            case ODAdbType.ODecimal:
                                                Oprms[c].ParamsValue = NYear.ODA.DBAccess.DataConvert(Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()], typeof(decimal));
                                                break;
                                            case ODAdbType.OInt:
                                                Oprms[c].ParamsValue = NYear.ODA.DBAccess.DataConvert(Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()], typeof(int));
                                                break;
                                            case ODAdbType.ODatetime:
                                                if (Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()].ToString() == DateTime.MinValue.ToString() || Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()].ToString() == DateTime.MaxValue.ToString())
                                                    Oprms[c].ParamsValue = null;
                                                else
                                                    Oprms[c].ParamsValue = NYear.ODA.DBAccess.DataConvert(Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()], typeof(DateTime));
                                                break;
                                            default:
                                                Oprms[c].ParamsValue = Source.Rows[k][drs[c]["COLUMN_NAME"].ToString()];
                                                break;
                                        }
                                    }
                                }
                                prm.TargetDB.ExecuteSQL(insertSQL, Oprms);
                                ReportStatus RSData1 = new ReportStatus()
                                {
                                    Percent = (startIndx + k) * 100 / total,
                                    TransObject = "Transfer Data : " + (startIndx + k).ToString() + "/" + total.ToString(),
                                    TransType = "Data"
                                };
                                bgw.ReportProgress(RS.Percent, RSData1);
                            }
                            prm.TargetDB.Commit();
                        }
                        catch (Exception ex)
                        {
                            prm.TargetDB.RollBack();
                            sbrlt.AppendLine("Tranfer data Error:" + ex.Message);
                        }
                        startIndx = startIndx + maxR;
                    }
                    while (startIndx + maxR < total);
                }
            }

            e.Result = sbrlt.ToString();

 
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.InvokeRequired)
            {
                this.pnlTranStatus.Visible = false;
                this.lblExecuteRlt.Text = e.Result.ToString();
            }
            else
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.pnlTranStatus.Visible = false;
                    this.lblExecuteRlt.Text = e.Result.ToString();
                }), null);

            }
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!this.InvokeRequired)
            {
                ReportStatus RS = e.UserState as ReportStatus;
                SetTranStatus(RS);
            }
            else
            {
                this.BeginInvoke(new Action<ReportStatus>(SetTranStatus), e.UserState);
            }
        }
        private void SetTranStatus(ReportStatus RS)
        {
            this.pnlTranStatus.Visible = true;
            tbc_ExecuteSqlResult.SelectedTab = tpgMsg;
            if (RS.TransType == "Table")
            {
                this.lblTransTable.Text = RS.TransObject;
                this.lblTransData.Text = "Preparing Data";
                pgbTable.Value = RS.Percent;
                pgbTable.Update();
            }
            else if (RS.TransType == "Data")
            {
                this.lblTransData.Text = RS.TransObject;
                this.pgbData.Value = RS.Percent;
                this.pgbData.Update();
            }
        }

        private string CreateTable(DBAccess TargetDB, string TableName, DatabaseColumnInfo[] ColumnInfo, DatabaseColumnInfo[]  Pkeys )
        {
            try
            {
                string dropSQL = "DROP TABLE " + TableName;
                TargetDB.ExecuteSQL( dropSQL, null);
            }
            catch { }

            StringBuilder creatSQL = new StringBuilder();
            try
            {
                creatSQL.AppendLine("CREATE TABLE " + TableName);
                creatSQL.AppendLine("(");

                for (int i = 0; i < ColumnInfo.Length; i++)
                {
                    if (ColumnInfo[i] != null)
                    {
                        if (ColumnInfo[i].NoLength)
                            creatSQL.Append(ColumnInfo[i].Name + " " + ColumnInfo[i].ColumnType);
                        else
                            creatSQL.Append(ColumnInfo[i].Name + " " + ColumnInfo[i].ColumnType + "(" + ColumnInfo[i].Length.ToString() + ")");

                        if (ColumnInfo[i].NotNull)
                            creatSQL.Append(" NOT NULL ");
                        if (i + 1 < ColumnInfo.Length)
                            creatSQL.AppendLine(",");
                    }
                }

                if (Pkeys != null && Pkeys.Length > 0)
                {
                    string p = "";
                    for (int j = 0; j < Pkeys.Length; j++)
                        p += Pkeys[j].Name + ",";
                    p = p.Remove(p.Length - ",".Length, ",".Length);
                    creatSQL.AppendLine(" , primary key (" + p + ") ");
                }

                creatSQL.AppendLine(")");
                TargetDB.ExecuteSQL( creatSQL.ToString(), null);
            }
            catch(Exception ex)
            {
                creatSQL.Insert(0, "Create Table Error: " + ex.Message);
                return creatSQL.ToString();
            }
            return "" ;
        }

        private string MD5_16(string Str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Str)), 4, 8);
            return  t2.Replace("-", "");
        }
    }
     

    public class TransferParams
    {
        public DBAccess SourceDB { get; set; }
        public DBAccess TargetDB { get; set; }
        public bool NeedTransData { get; set; }
        public bool NeedTransTable { get; set; }
        public DataTable SrcTables { get; set; }
        public List<string> TranTable { get; set; }
        public DataTable SrcData { get; set; }
    }

    public class ReportStatus
    {
        public string TransObject { get; set; }
        public string TransType { get; set; }
        public int Percent { get; set; }
    }

}
