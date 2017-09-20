using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
internal partial class CmdPrmRoleAuthorize:ORMCmd<PRM_ROLE_AUTHORIZE>
{
     public ODAColumns ColRoleName{ get { return new ODAColumns(this, "ROLE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColResourceName{ get { return new ODAColumns(this, "RESOURCE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColOperateName{ get { return new ODAColumns(this, "OPERATE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColIsForbidden{ get { return new ODAColumns(this, "IS_FORBIDDEN", ODAdbType.OVarchar, 1,false ); } }
     public ODAColumns ColCreateDate{ get { return new ODAColumns(this, "CREATE_DATE", ODAdbType.ODatetime, 8,false ); } }
     public ODAColumns ColCreateBy{ get { return new ODAColumns(this, "CREATE_BY", ODAdbType.OVarchar, 100,false ); } }
     public override string CmdName { get { return "PRM_ROLE_AUTHORIZE"; }}
      public override List<ODAColumns> GetColumnList() 
      { 
          return new List<ODAColumns>() { ColRoleName,ColResourceName,ColOperateName,ColIsForbidden,ColCreateDate,ColCreateBy};
         }
}
}
