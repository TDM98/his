using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_DrugInOutStatisticsDetail : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_DrugInOutStatisticsDetail()
        {
            InitializeComponent();
        }
        
        private void XRpt_DrugInOutStatisticsDetail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics1.EnforceConstraints = false;
            ReportSqlProvider.Instance.ReaderIntoSchema(
                dsInOutStatistics1.spRpt_DrugInOutStatistics,
                spRpt_DrugInOutStatisticsTableAdapter1.Adapter.GetFillParameters(),
                new object[]
                {
                    Convert.ToDateTime(parFromDate.Value),
                    Convert.ToDateTime(parToDate.Value),
                    Convert.ToInt64(StoreID.Value),
                    Convert.ToInt64(parStoreIn.Value),
                    Convert.ToInt64(parStoreOut.Value),
                    Convert.ToByte(parPurposeIn.Value),
                    Convert.ToByte(parPurposeOut.Value)
                }, int.MaxValue);
        }
    }
}
