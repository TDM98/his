using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCBNHoanTra : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCBNHoanTra()
        {
            InitializeComponent();
        }

        private void XRpt_BCBNHoanTra_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBCHoanTra1.sp_BCHoanTra, sp_BCHoanTraTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), Convert.ToInt32(parFindPatient.Value), Convert.ToInt64(parStaffID.Value)
            }, int.MaxValue);
        }
    }
}
