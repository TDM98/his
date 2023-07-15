using System;
using aEMR.DataAccessLayer.Providers;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ClinicDeptInOutStatisticsDetail_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ClinicDeptInOutStatisticsDetail_V2()
        {
            InitializeComponent();
        }
        
        private void XRpt_ClinicDeptInOutStatisticsDetail_V2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInOutStatistics_V21.EnforceConstraints = false;
            ReportSqlProvider.Instance.ReaderIntoSchema(
                dsInOutStatistics_V21.spRpt_ClinicDeptInOutStatistics_V2,
                spRpt_ClinicDeptInOutStatistics_V2TableAdapter1.Adapter.GetFillParameters(),
                new object[]
                {
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
