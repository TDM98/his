using System;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ClinicDeptInOutStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ClinicDeptInOutStatistics()
        {
            InitializeComponent();
        }

        private void XRpt_ClinicDeptInOutStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics1.EnforceConstraints = false;
            spRpt_ClinicDeptInOutStatisticsTableAdapter.Fill(
                dsInOutStatistics1.spRpt_ClinicDeptInOutStatistics
                , Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt64(StoreID.Value), Convert.ToInt64(parStoreIn.Value), Convert.ToInt64(parStoreOut.Value)
                , Convert.ToByte(parPurposeIn.Value), Convert.ToByte(parPurposeOut.Value)
                , Convert.ToInt64(parV_MedProductType.Value)
            );
        }
    }
}
