﻿using System.Collections.Generic;

namespace NYear.ODA
{
    public interface IODAFunction
    {
        ODAColumns Count { get; }
        ODAColumns Case(ODAColumns CaseColumn, Dictionary<object, object> WhenThen, object ElseVal);
        ODAColumns CaseWhen(Dictionary<ODAColumns, object> WhenThen, object ElseVal);
        ODAColumns CreateFunc(string Function, ODAdbType ColumnType = ODAdbType.OVarchar, params object[] ParamsList);
        ODAColumns Exists(ODACmd Cmd, params ODAColumns[] Cols);
        ODAColumns NotExists(ODACmd Cmd, params ODAColumns[] Cols);

        ODAColumns VisualColumn(string Val, ODAdbType ColumnType = ODAdbType.OVarchar);

        //ODAColumns IsNullDefault(ODAColumns Col, object DefVal);
        //ODAColumns Decode(ODAColumns Col, object DefVal,params object[] KeyValue );

    }
}