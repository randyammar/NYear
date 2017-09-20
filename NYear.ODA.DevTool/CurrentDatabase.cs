using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NYear.ODA.DevTool
{
    internal class CurrentDatabase
    {
        private static ODA.DBAccess _DataSource = null;
        public static ODA.DBAccess DataSource
        {
            get { return _DataSource; }
            set 
            { 
                _DataSource = value;
                if (DBConnected != null)
                    DBConnected(_DataSource, EventArgs.Empty);
            
            }
        }
        public static string DBConnectString { get; set; }
        public static string[] UserTables { get; set; }
        public static string[] UserViews { get; set; }
        public static string[] UserProcedures { get; set; }
        public static event EventHandler DBConnected;
    }
}
