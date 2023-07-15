using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_DrugInOutStatisticsDetail_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_DrugInOutStatisticsDetail_V2()
        {
            InitializeComponent();
        }
        
        private void XRpt_DrugInOutStatisticsDetail_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics_V21.EnforceConstraints = false;
            ReportSqlProvider.Instance.ReaderIntoSchema(
                dsInOutStatistics_V21.spRpt_DrugInOutStatistics_V2,
                spRpt_DrugInOutStatistics_V2TableAdapter1.Adapter.GetFillParameters(),
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
