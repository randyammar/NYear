using NYear.ODA;
using System.Collections.Generic;
using System.Data;

namespace NYear.Demo
{
    internal partial class CmdIqcResult : ORMCmd<ModelIqcResult>
    {
        public ODAColumns ColDatetimeCreated { get { return new ODAColumns(this, "DATETIME_CREATED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColDatetimeModified { get { return new ODAColumns(this, "DATETIME_MODIFIED", ODAdbType.ODatetime, 8); } }
        public ODAColumns ColDeliveryBillNo { get { return new ODAColumns(this, "DELIVERY_BILL_NO", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColDeliveryQty { get { return new ODAColumns(this, "DELIVERY_QTY", ODAdbType.OInt, 9); } }
        public ODAColumns ColEnterpriseId { get { return new ODAColumns(this, "ENTERPRISE_ID", ODAdbType.OVarchar, 36); } }
        public ODAColumns ColId { get { return new ODAColumns(this, "ID", ODAdbType.OVarchar, 32); } }
        public ODAColumns ColInspectResult { get { return new ODAColumns(this, "INSPECT_RESULT", ODAdbType.OVarchar, 32); } }
        public ODAColumns ColLotNo { get { return new ODAColumns(this, "LOT_NO", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColMitemCode { get { return new ODAColumns(this, "MITEM_CODE", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColNgReason { get { return new ODAColumns(this, "NG_REASON", ODAdbType.OVarchar, 2000); } }
        public ODAColumns ColOrgId { get { return new ODAColumns(this, "ORG_ID", ODAdbType.OVarchar, 32); } }
        public ODAColumns ColState { get { return new ODAColumns(this, "STATE", ODAdbType.OVarchar, 1); } }
        public ODAColumns ColSupplierCode { get { return new ODAColumns(this, "SUPPLIER_CODE", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserCreated { get { return new ODAColumns(this, "USER_CREATED", ODAdbType.OVarchar, 80); } }
        public ODAColumns ColUserModified { get { return new ODAColumns(this, "USER_MODIFIED", ODAdbType.OVarchar, 80); } }
        public override string CmdName { get { return "IQC_RESULT"; } }
        public override DataSet Procedure(params ODAColumns[] Cols) { throw new ODAException("Not Suport Procedure CmdName " + CmdName); }
        public override List<ODAColumns> GetColumnList()
        {
            return new List<ODAColumns>() { ColDatetimeCreated, ColDatetimeModified, ColDeliveryBillNo, ColDeliveryQty, ColEnterpriseId, ColId, ColInspectResult, ColLotNo, ColMitemCode, ColNgReason, ColOrgId, ColState, ColSupplierCode, ColUserCreated, ColUserModified };
        }
    } 
}
