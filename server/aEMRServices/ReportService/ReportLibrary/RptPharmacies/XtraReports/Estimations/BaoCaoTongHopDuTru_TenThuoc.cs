using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Estimations
{
    public partial class BaoCaoTongHopDuTru_TenThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoTongHopDuTru_TenThuoc()
        {
            InitializeComponent();
        }

        private void FillData()
        {
           baoCao_TongHopDuTru_TheoThuocTableAdapter.Fill((this.DataSource as DataSchema.Estimations.dsDuTruTheoTenThuoc).BaoCao_TongHopDuTru_TheoThuoc, Convert.ToDateTime(this.FromDate.Value),Convert.ToDateTime(this.ToDate.Value),Convert.ToInt32(this.Quarter.Value),Convert.ToInt32(this.Month.Value),Convert.ToInt32(this.Year.Value),Convert.ToByte(this.Flag.Value));
        }
        private void BaoCaoTongHopDuTru_TenThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
