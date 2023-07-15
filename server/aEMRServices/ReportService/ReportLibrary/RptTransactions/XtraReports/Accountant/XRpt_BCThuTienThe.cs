using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_BCThuTienThe : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_BCThuTienThe()
        {
            InitializeComponent();
        }

        private void XRpt_BCThuTienThe_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBaoCaoThuTienThe1.sp_BCThanhToanThe, sp_BCThanhToanTheTableAdapter.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value)
            }, int.MaxValue);
        }
    }
}
