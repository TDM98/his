using System;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class BaoCaoNhapThuocHangThangInvoice : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoNhapThuocHangThangInvoice()
        {
            InitializeComponent();
            FillData();
        }

        private void FillData()
        {
            baoCao_NhapThuocHangThangInvoiceTableAdapter.Fill(baoCao_NhapThuocHangThangInvoice1._BaoCao_NhapThuocHangThangInvoice, Convert.ToInt64(this.StoreID.Value), Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value), Convert.ToInt32(this.InwardSource.Value));
        }
        private void BaoCaoNhapThuocHangThangInvoice_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
