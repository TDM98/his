using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XRpt_ThongKeDsachKBNgTruTheoBacSi : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_ThongKeDsachKBNgTruTheoBacSi()
        {
            InitializeComponent();
        }

        private void XRpt_ThongKeDsachKBNgTruTheoBacSi_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsThongKeDsachKBTheoBacSi1.spRpt_ThongKeDsachBacSiKhamBenhNgTru, spRpt_ThongKeDsachBacSiKhamBenhNgTruTableAdapter1.Adapter.GetFillParameters(), new object[] {
                Convert.ToDateTime(parFromDate.Value), Convert.ToDateTime(parToDate.Value), 0
            }, int.MaxValue);
        }
    }
}
 