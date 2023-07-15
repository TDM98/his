using System;
namespace eHCMS.ReportLib.RptDrugDepts.XtraReports
{
    public partial class XRptDrugDeptLocalOutput : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptDrugDeptLocalOutput()
        {
            InitializeComponent();
        }
        private void XRptDrugDeptLocalOutput_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spDrugDeptLocalOutputStatisticsTableAdapter.Fill(dsDrugDeptLocalOutput1.spDrugDeptLocalOutputStatistics, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt64(StoreID.Value), Convert.ToInt64(OutputStoreID.Value));
        }
    }
}