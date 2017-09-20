using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using NYear.ODA;
using NYear.ODA.Model;

namespace NYear.ODA.Cmd
{
internal partial class CmdSequenceTable:ORMCmd<SEQUENCE_TABLE>
{
     public ODAColumns ColSequenceName{ get { return new ODAColumns(this, "SEQUENCE_NAME", ODAdbType.OVarchar, 32,false ); } }
     public ODAColumns ColCurrenceValue{ get { return new ODAColumns(this, "CURRENCE_VALUE", ODAdbType.ODecimal, 8,false ); } }
     public ODAColumns ColSetval{ get { return new ODAColumns(this, "SETVAL", ODAdbType.ODecimal, 8,false ); } }
     public override string CmdName { get { return "SEQUENCE_TABLE"; }}
      public override List<ODAColumns> GetColumnList() 
      { 
          return new List<ODAColumns>() { ColSequenceName,ColCurrenceValue,ColSetval};
         }
}
}
