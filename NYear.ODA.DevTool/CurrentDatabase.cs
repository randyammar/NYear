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


        private static Dictionary<string, object> _ODATypeMap = null;
        public static Dictionary<string, object> ODATypeMap
        {
            get
            {
                if (_ODATypeMap == null)
                {
                    System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();
                    if (System.IO.File.Exists("DbType.json"))
                    {
                        string dbType = System.IO.File.ReadAllText("DbType.json", Encoding.UTF8);
                        _ODATypeMap = json.Deserialize<Dictionary<string, object>>(dbType); 
                    }
                }
                return _ODATypeMap;
            }
        }
        public static string GetTargetsType(string Original,string From,string Target)
        {
            if (ODATypeMap != null && ODATypeMap.ContainsKey(From))
            {
                var FromDict = ODATypeMap[From] as Dictionary<string, object>;

                if (FromDict != null && FromDict.ContainsKey(Target))
                {
                    var TargetDict = FromDict[Target] as Dictionary<string, object>;
                    if (TargetDict != null && TargetDict.ContainsKey(Original))
                    {
                        return TargetDict[Original].ToString();
                    }
                }
            }
            return Original;
        }
    }
}
