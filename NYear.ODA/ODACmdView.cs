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
        protected override ODACmd BaseCmd
        {
            get
            {
                return _CmdView;
            }
        }

        protected override string DBObjectMap
        {
            get
            {
                return ((IDBScriptGenerator)_CmdView).DBObjectMap;
            }
            set
            {
                ((IDBScriptGenerator)_CmdView).DBObjectMap = value;
            }
        }

        protected override ODAParameter[] GetCmdSql(out string DBObject)
        {
            string View = null;
            ODAParameter[] prms = ((IDBScriptGenerator)_CmdView).GetSelectSql(out View, SelectCols);
            DBObject = "(" + View + ")";
            return prms;
        }

        public ODAColumns CreateColumn(string ColName, ODAdbType ColType = ODAdbType.OVarchar, int size = 2000)
        {
            return new ODAColumns(this, ColName, ColType, size);
        }

        internal ODACmdView(ODACmd Cmd, params ODAColumns[] Cols)
        {
            _CmdView = Cmd;
            Alias = Cmd.Alias + "V";
            SelectCols = Cols; 
        }
      
    }
}