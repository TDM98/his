using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies.XtraReports.Estimations
{
    public partial class BaoCaoTongHopDuTru_SoPhieu : DevExpress.XtraReports.UI.XtraReport
    {
        public BaoCaoTongHopDuTru_SoPhieu()
        {
            InitializeComponent();
        }

        private void FillData()
        {
           baoCao_TongHopDuTru_TheoSoPhieuTableAdapter.Fill((this.DataSource as DataSchema.Estimations.dsDuTruTheoSoPhieu).BaoCao_TongHopDuTru_TheoSoPhieu, Convert.ToDateTime(this.FromDate.Value),Convert.ToDateTime(this.ToDate.Value),Convert.ToInt32(this.Quarter.Value),Convert.ToInt32(this.Month.Value),Convert.ToInt32(this.Year.Value),Convert.ToByte(this.Flag.Value));
        }
        private void BaoCaoTongHopDuTru_SoPhieu_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}
