using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
internal partial class CmdPrmRole:ORMCmd<PRM_ROLE>
{
     public ODAColumns ColRoleName{ get { return new ODAColumns(this, "ROLE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColIsSupperAdmin{ get { return new ODAColumns(this, "IS_SUPPER_ADMIN", ODAdbType.OVarchar, 1,false ); } }
     public ODAColumns ColDescript{ get { return new ODAColumns(this, "DESCRIPT", ODAdbType.OVarchar, 200,false ); } }
     public ODAColumns ColCreateDate{ get { return new ODAColumns(this, "CREATE_DATE", ODAdbType.ODatetime, 8,false ); } }
     public ODAColumns ColCreateBy{ get { return new ODAColumns(this, "CREATE_BY", ODAdbType.OVarchar, 100,false ); } }
     public override string CmdName { get { return "PRM_ROLE"; }}
      public override List<ODAColumns> GetColumnList() 
      { 
          return new List<ODAColumns>() { ColRoleName,ColIsSupperAdmin,ColDescript,ColCreateDate,ColCreateBy};
         }
}
}
