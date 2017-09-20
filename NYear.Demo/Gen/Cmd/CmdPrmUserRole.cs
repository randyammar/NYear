using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
internal partial class CmdPrmUserRole:ORMCmd<PRM_USER_ROLE>
{
     public ODAColumns ColUserId{ get { return new ODAColumns(this, "USER_ID", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColUserName{ get { return new ODAColumns(this, "USER_NAME", ODAdbType.OVarchar, 64,false ); } }
     public ODAColumns ColRoleName{ get { return new ODAColumns(this, "ROLE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColCreateDate{ get { return new ODAColumns(this, "CREATE_DATE", ODAdbType.ODatetime, 8,false ); } }
     public ODAColumns ColCreateBy{ get { return new ODAColumns(this, "CREATE_BY", ODAdbType.OVarchar, 100,false ); } }
     public override string CmdName { get { return "PRM_USER_ROLE"; }}
      public override List<ODAColumns> GetColumnList() 
      { 
          return new List<ODAColumns>() { ColUserId,ColUserName,ColRoleName,ColCreateDate,ColCreateBy};
         }
}
}
