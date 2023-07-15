using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions.XtraReports.QualityManagement
{
    public partial class XRptBCThoiGianBNChoTaiBV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBCThoiGianBNChoTaiBV()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCThoiGianBNChoTaiBV1.spRpt_BCThoiGianBNChoTaiBV, spRpt_BCThoiGianBNChoTaiBVTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value)
            }, int.MaxValue);
        }

        private void XRptBCThoiGianBNChoTaiBV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
