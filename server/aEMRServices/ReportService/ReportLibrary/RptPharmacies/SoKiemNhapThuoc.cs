using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class SoKiemNhapThuoc : DevExpress.XtraReports.UI.XtraReport
    {
        public SoKiemNhapThuoc()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            spRpt_PharmacyInwardDetailTableAdapter.Fill(dsSoKiemNhapThuoc1.spRpt_PharmacyInwardDetail, Convert.ToDateTime(this.FromDate.Value), Convert.ToDateTime(this.ToDate.Value), Convert.ToInt32(this.Quarter.Value), Convert.ToInt32(this.Month.Value), Convert.ToInt32(this.Year.Value), Convert.ToByte(this.Flag.Value), Convert.ToInt64(this.StoreID.Value));
        }
        private void SoKiemNhapThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

    }
}