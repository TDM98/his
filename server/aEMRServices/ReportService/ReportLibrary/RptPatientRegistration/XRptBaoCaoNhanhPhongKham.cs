using System;

namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptBaoCaoNhanhPhongKham : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptBaoCaoNhanhPhongKham()
        {
            InitializeComponent();
        }

        private void XRptBaoCaoNhanhPhongKham_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsBaoCaoNhanhPhongKham1.EnforceConstraints = false;
            baoCao_NHANHKHUKHAMBENHTableAdapter.Fill(dsBaoCaoNhanhPhongKham1.BaoCao_NHANHKHUKHAMBENH, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
            baoCao_ChiTietTaiPKTableAdapter.Fill(dsBaoCaoNhanhPhongKham1.BaoCao_ChiTietTaiPK, Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value));
        }
    }
}
