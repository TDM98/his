using System;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class BaoCaoNhapThuocHangThang : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoNhapThuocHangThang()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            baoCao_NhapThuocHangThangTableAdapter.Fill(dsBaoCao_NhapThuocHangThang1.BaoCao_NhapThuocHangThang, Convert.ToInt64(StoreID.Value), Convert.ToDateTime(FromDate.Value), Convert.ToDateTime(ToDate.Value), Convert.ToInt32(Quarter.Value), Convert.ToInt32(Month.Value), Convert.ToInt32(Year.Value), Convert.ToByte(Flag.Value), Convert.ToInt32(InwardSource.Value));
        }
        private void BaoCaoNhapThuocHangThang_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
