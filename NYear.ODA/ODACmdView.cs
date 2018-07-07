using System.Collections.Generic;

namespace NYear.ODA
{
    public class ODACmdView : ODACmd
    {
        private ODACmd _CmdView = null;
        private ODAColumns[] SelectCols = null;
        public override string CmdName
        {
            get
            {
                return _CmdView.CmdName;
            }
        }
        public ODAColumns[] ViewColumns
        {
            get
            {
                int vcl = SelectCols == null ? 0 : SelectCols.Length;
                ODAColumns[] vc = new ODAColumns[vcl];
                for (int i = 0; i < vcl && SelectCols != null; i++)
                    vc[i] = new ODAColumns(this, SelectCols[i].ColumnName, SelectCols[i].DBDataType, SelectCols[i].Size);
                return vc;
            }
        }
        protected override ODACmd BaseCmd
        {
            get
            {
                return _CmdView;
            }
        }

        public override string DBObjectMap
        {
            get
            {
                return ((ODACmd)_CmdView).DBObjectMap;
            }
            set
            {
                ((ODACmd)_CmdView).DBObjectMap = value;
            }
        }

        protected override ODAScript GetCmdSql()
        {
            var view =  ((ODACmd)_CmdView).GetSelectSql(SelectCols);
            view.SqlScript.Insert(0, "(").Append(")");
            return view; 
        } 
        public ODAColumns CreateColumn(string ColName, ODAdbType ColType = ODAdbType.OVarchar, int size = 2000)
        {
            return new ODAColumns(this, ColName, ColType, size);
        }

        internal ODACmdView(ODACmd Cmd, params ODAColumns[] Cols)
        { 
            _CmdView = Cmd;
            Alias = Cmd.GetAlias();
            SelectCols = Cols; 
        }
    }
}