using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptCardStorageDrugDept_InCost : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptCardStorageDrugDept_InCost()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsCardStorageDrugDept_InCost1.EnforceConstraints = false;
            spRpt_DrugDept_CardStorage_InCostTableAdapter.Fill(dsCardStorageDrugDept_InCost1.spRpt_DrugDept_CardStorage_InCost,
                Convert.ToInt64(StoreID.Value), Convert.ToInt64(GenMedProductID.Value), Convert.ToDateTime(FromDate.Value)
                , Convert.ToDateTime(ToDate.Value), Convert.ToInt64(V_MedProductType.Value)
            );
        }

        private void XRptCardStorage_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
            if (dsCardStorageDrugDept_InCost1.spRpt_DrugDept_CardStorage_InCost != null && dsCardStorageDrugDept_InCost1.spRpt_DrugDept_CardStorage_InCost.Rows.Count > 0)
            {
                Parameters["StockFinal"].Value = Convert.ToDecimal(dsCardStorageDrugDept_InCost1.spRpt_DrugDept_CardStorage_InCost.Rows[dsCardStorageDrugDept_InCost1.spRpt_DrugDept_CardStorage_InCost.Rows.Count - 1]["QtyStocks"]);
            }
        }
    }
}
