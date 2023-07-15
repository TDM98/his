using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuNhanThuocSummary : XtraReport
    {
        public PhieuNhanThuocSummary()
        {
            InitializeComponent();
        }

        private void phieuNhanThuoc_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuNhanThuoc)((XRSubreport)sender).ReportSource).OutiID.Value = Convert.ToInt64(OutiID.Value);
            ((PhieuNhanThuoc)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
        private void phieuNhanThuocBH_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuNhanThuocBH)((XRSubreport)sender).ReportSource).OutiID.Value = Convert.ToInt64(OutiID.Value);
            ((PhieuNhanThuocBH)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
    }
}
