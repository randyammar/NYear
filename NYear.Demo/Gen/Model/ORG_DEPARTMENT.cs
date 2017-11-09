using System;

namespace NYear.ODA.Model
{
    public partial class ORG_DEPARTMENT
    {
        public string DEPT_ID { get; set; }
        public string PARENT_DEPT { get; set; }
        public string DEPT_ORG { get; set; }
        public string DEPT_NAME { get; set; }
        public string BOSS_ID { get; set; }
        public string BOSS_NAME { get; set; }
        public string ASSISTANT_ID { get; set; }
        public string ASSISTANT_NAME { get; set; }
        public string STATUS { get; set; }
    }
}
