using System;
namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_DrugInOutStatistics : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_DrugInOutStatistics()
        {
            InitializeComponent();
        }

        private void XRpt_DrugInOutStatistics_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics1.EnforceConstraints = false;
            spRpt_DrugInOutStatisticsTableAdapter1.Fill(
                dsInOutStatistics1.spRpt_DrugInOutStatistics
                , Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
                , Convert.ToInt64(StoreID.Value), Convert.ToInt64(parStoreIn.Value), Convert.ToInt64(parStoreOut.Value)
                , Convert.ToByte(parPurposeIn.Value), Convert.ToByte(parPurposeOut.Value)
            );
        }
    }
}
