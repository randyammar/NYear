using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
internal partial class CmdVPrmUserAuthorize:ORMCmd<V_PRM_USER_AUTHORIZE>
{
     public override bool Insert(params ODA.ODAColumns[] Cols) { throw new ODAException("Not suport Insert CmdName " + CmdName);}
     public override bool Update(params ODAColumns[] Cols) {  throw new ODAException("Not Suport Update CmdName " + CmdName);}
     public override bool Delete() {  throw new ODAException("Not Suport Delete CmdName " + CmdName);}
     public ODAColumns ColUserId{ get { return new ODAColumns(this, "USER_ID", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColUserName{ get { return new ODAColumns(this, "USER_NAME", ODAdbType.OVarchar, 64,false ); } }
     public ODAColumns ColResourceName{ get { return new ODAColumns(this, "RESOURCE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColOperateName{ get { return new ODAColumns(this, "OPERATE_NAME", ODAdbType.OVarchar, 64,true ); } }
     public ODAColumns ColIsForbidden{ get { return new ODAColumns(this, "IS_FORBIDDEN", ODAdbType.OVarchar, 1,false ); } }
     public ODAColumns ColDescript{ get { return new ODAColumns(this, "DESCRIPT", ODAdbType.OVarchar, 200,false ); } }
     public override string CmdName { get { return "V_PRM_USER_AUTHORIZE"; }}
      public override List<ODAColumns> GetColumnList() 
      { 
          return new List<ODAColumns>() { ColUserId,ColUserName,ColResourceName,ColOperateName,ColIsForbidden,ColDescript};
         }
}
}
