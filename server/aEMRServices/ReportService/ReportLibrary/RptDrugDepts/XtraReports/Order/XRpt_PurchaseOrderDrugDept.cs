using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Order
{
    public partial class XRpt_PurchaseOrderDrugDept : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_PurchaseOrderDrugDept()
        {
            InitializeComponent();
        }
     
        public void FillData()
        {
            spDrugDeptPurchaseOrderDetails_ByIDParentTableAdapter.Fill(dsOrderDrugDept1.spDrugDeptPurchaseOrderDetails_ByIDParent, Convert.ToInt64(ID.Value), 0);
            spDrugDeptPurchaseOrders_ByIDTableAdapter.Fill(dsOrderDrugDept1.spDrugDeptPurchaseOrders_ByID, Convert.ToInt64(ID.Value));

            decimal totalNotVAT = 0;
            decimal VAT = 0;
            decimal VATValue = 0;
            decimal total = 0;
            if (dsOrderDrugDept1.spDrugDeptPurchaseOrders_ByID != null && dsOrderDrugDept1.spDrugDeptPurchaseOrders_ByID.Rows.Count > 0)
            {
                VAT = Convert.ToDecimal(dsOrderDrugDept1.spDrugDeptPurchaseOrders_ByID.Rows[0]["VAT"]);
            }
            if (dsOrderDrugDept1.spDrugDeptPurchaseOrderDetails_ByIDParent != null && dsOrderDrugDept1.spDrugDeptPurchaseOrderDetails_ByIDParent.Rows.Count > 0)
            {
                for (int i = 0; i < dsOrderDrugDept1.spDrugDeptPurchaseOrderDetails_ByIDParent.Rows.Count; i++)
                {
                    totalNotVAT += Convert.ToDecimal(dsOrderDrugDept1.spDrugDeptPurchaseOrderDetails_ByIDParent.Rows[i]["UnitPrice"]) * Convert.ToDecimal(dsOrderDrugDept1.spDrugDeptPurchaseOrderDetails_ByIDParent.Rows[i]["PoUnitQty"]);
                }
                if (VAT > 0)
                {
                    VATValue = (VAT - 1) * totalNotVAT;
                    Parameters["VATValue"].Value = VATValue;
                }
                total = totalNotVAT + VATValue;
                Parameters["TotalValue"].Value = total;

            //    NumberToLetterConverter converter = new NumberToLetterConverter();
            //    decimal temp = 0;
            //    string prefix = "";
            //    if (total < 0)
            //    {
            //        temp = 0 - total;
            //        prefix = string.Format(" {0} ",  eHCMSResources.Z0873_G1_Am.ToLower());
            //    }
            //    else
            //    {
            //        temp = total;
            //        prefix = "";
            //    }
            //    this.Parameters["ReadMoney"].Value = string.Format("{0} : ",  eHCMSResources.G2256_G1_VietBangChu) + prefix + converter.Convert(Math.Round(temp, 0).ToString(), '.', eHCMSResources.Z0871_G1_Le.ToLower()) + string.Format(" {0}",  eHCMSResources.Z0872_G1_Dong.ToLower());
            }
        }

        private void XRpt_PurchaseOrderPharmacy_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
