using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class BaoCaoGiaoBanTheoKhoaLS_KHTH : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoGiaoBanTheoKhoaLS_KHTH()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            ReportSqlProvider.Instance.ReaderIntoSchema(dsBaoCaoGiaoBanTheoKhoaLS_KHTH.spBaoCaoGiaoBanTheoKhoaLS_KHTH, spBaoCaoGiaoBanTheoKhoaLS_KHTHTableAdapter.Adapter.GetFillParameters(), new object[] {
               Convert.ToInt32(DeptID.Value), Convert.ToDateTime(FromDate.Value.ToString()), Convert.ToDateTime(ToDate.Value.ToString())
            }, int.MaxValue);

            //spBaoCaoGiaoBanTheoKhoaLS_KHTHTableAdapter.Fill(dsBaoCaoGiaoBanTheoKhoaLS_KHTH.spBaoCaoGiaoBanTheoKhoaLS_KHTH,
            //    Convert.ToInt64(DeptID.Value),
            //    Convert.ToDateTime(FromDate.Value),
            //    Convert.ToDateTime(ToDate.Value));
        }

        private void BaoCaoGiaoBanTheoKhoaLS_KHTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
