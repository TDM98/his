using System;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_MedDeptInOutStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_MedDeptInOutStatistics()
        {
            InitializeComponent();
        }

        private void XRpt_MedDeptInOutStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics1.EnforceConstraints = false;
            spRpt_MedDeptInOutStatisticsTableAdapter.Fill(
                dsInOutStatistics1.spRpt_MedDeptInOutStatistics
                , Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt64(StoreID.Value), Convert.ToInt64(parStoreIn.Value), Convert.ToInt64(parStoreOut.Value)
                , Convert.ToByte(parPurposeIn.Value), Convert.ToByte(parPurposeOut.Value)
                , Convert.ToInt64(parV_MedProductType.Value)
            );
        }
    }
}
