using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCChiTietDV_CLS_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCChiTietDV_CLS_New()
        {
            InitializeComponent();
        }

        private void XRpt_BCChiTietDV_CLS_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCChiTietDV_CLS_New1.spRpt_BCChiTietDV_CLS_New, spRpt_BCChiTietDV_CLS_NewTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value)
            }, int.MaxValue);
        }
    }
}
