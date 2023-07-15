using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuNhanThuocSummary_InNhiet : XtraReport
    {
        public PhieuNhanThuocSummary_InNhiet()
        {
            InitializeComponent();
        }

        private void phieuNhanThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuNhanThuocInNhiet)((XRSubreport)sender).ReportSource).OutiID.Value = Convert.ToInt64(OutiID.Value);
            ((PhieuNhanThuocInNhiet)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
        private void phieuNhanThuocBH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuNhanThuocBHYTInNhiet)((XRSubreport)sender).ReportSource).OutiID.Value = Convert.ToInt64(OutiID.Value);
            ((PhieuNhanThuocBHYTInNhiet)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
    }
}
