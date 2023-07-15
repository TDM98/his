using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptDrugDeptInOutStockByBid : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptInOutStockByBid()
        {
            InitializeComponent();
        }
        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptDrugDeptInOutStockByBidTableAdapter.Fill(dsDrugDeptInOutStockByBid1.spRptDrugDeptInOutStockByBid, StoreID.Value as long?, StartDate.Value as DateTime?, EndDate.Value as DateTime?, V_MedProductType.Value as long?, BidID.Value as long?);
        }
    }
}