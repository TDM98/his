using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ClinicDeptInOutStatisticsDetail : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ClinicDeptInOutStatisticsDetail()
        {
            InitializeComponent();
        }
        
        private void XRpt_ClinicDeptInOutStatisticsDetail_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics1.EnforceConstraints = false;
            ReportSqlProvider.Instance.ReaderIntoSchema(
                dsInOutStatistics1.spRpt_ClinicDeptInOutStatistics,
                spRpt_ClinicDeptInOutStatisticsTableAdapter.Adapter.GetFillParameters(),
                new object[] {
                    Convert.ToDateTime(parFromDate.Value),
                    Convert.ToDateTime(parToDate.Value),
                    Convert.ToInt64(StoreID.Value),
                    Convert.ToInt64(parStoreIn.Value),
                    Convert.ToInt64(parStoreOut.Value),
                    Convert.ToByte(parPurposeIn.Value),
                    Convert.ToByte(parPurposeOut.Value),
                    Convert.ToInt64(parV_MedProductType.Value)
                }, int.MaxValue);
        }
    }
}
