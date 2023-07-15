using System;
namespace eHCMS.ReportLib.RptDrugDepts.XtraReports
{
    public partial class XRptDrugDeptInOutStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptInOutStatistics()
        {
            InitializeComponent();
        }
        private void XRptDrugDeptInOutStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spDrugDeptInOutStatisticsTableAdapter.Fill(dsDrugDeptInOutStatistics1.spDrugDeptInOutStatistics, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(StoreID.Value));
        }
    }
}