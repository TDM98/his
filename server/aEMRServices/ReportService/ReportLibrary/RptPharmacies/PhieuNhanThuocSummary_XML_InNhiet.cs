using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuNhanThuocSummary_XML_InNhiet : XtraReport
    {
        public PhieuNhanThuocSummary_XML_InNhiet()
        {
            InitializeComponent();
        }

        private void PhieuNhanThuocSummary_XML_InNhiet_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter.Fill(dsPhieuNhanThuoc1.sp_PhieuNhanThuocSummary_ByOutiIDXml, OutiID.Value);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuNhanThuocSummary_InNhiet)((XRSubreport)sender).ReportSource).OutiID.Value = Convert.ToInt64(GetCurrentColumnValue("OutiID"));
            ((PhieuNhanThuocSummary_InNhiet)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
    }
}
