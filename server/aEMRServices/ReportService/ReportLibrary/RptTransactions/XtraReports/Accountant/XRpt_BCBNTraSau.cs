using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCBNTraSau : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCBNTraSau()
        {
            InitializeComponent();
        }

        private void XRpt_BCBNTraSau_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCBNTraSau1.spRpt_BCBNTraSau, spRpt_BCBNTraSauTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
