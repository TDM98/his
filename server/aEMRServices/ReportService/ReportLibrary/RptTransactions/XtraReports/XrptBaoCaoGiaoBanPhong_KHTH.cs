using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptTransactions
{
    public partial class XrptBaoCaoGiaoBanPhong_KHTH : DevExpress.XtraReports.UI.XtraReport
    {
        public XrptBaoCaoGiaoBanPhong_KHTH()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsRpt_BaoCaoGiaoBanPhong_KHTH1.EnforceConstraints = false;
            spRpt_BaoCaoGiaoBanPhong_KHTHTableAdapter.Fill(dsRpt_BaoCaoGiaoBanPhong_KHTH1.spRpt_BaoCaoGiaoBanPhong_KHTH, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
        }

        private void BaoCaoGiaoBanTheoKhoaLS_KHTH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
