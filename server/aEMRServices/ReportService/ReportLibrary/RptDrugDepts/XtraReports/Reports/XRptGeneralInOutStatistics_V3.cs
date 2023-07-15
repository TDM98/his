using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptDrugDepts.XtraReports.Reports
{
    public partial class XRptGeneralInOutStatistics_V3 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptGeneralInOutStatistics_V3()
        {
            InitializeComponent();           
        }

        public void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsGeneralInOutStatistics_V31.spGeneralInOutStatistics_V3, spGeneralInOutStatistics_V3TableAdapter1.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(FromDate.Value.ToString())
                , Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);
        }

        private void XRptGeneralInOutStatistics_V3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
