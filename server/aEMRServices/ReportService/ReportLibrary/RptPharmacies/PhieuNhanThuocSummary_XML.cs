using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptPharmacies
{
    public partial class PhieuNhanThuocSummary_XML : XtraReport
    {
        public PhieuNhanThuocSummary_XML()
        {
            InitializeComponent();
        }

        private void PhieuNhanThuocSummary_XML_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            sp_PhieuNhanThuocSummary_ByOutiIDXmlTableAdapter.Fill(dsPhieuNhanThuoc1.sp_PhieuNhanThuocSummary_ByOutiIDXml, OutiID.Value);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((PhieuNhanThuocSummary)((XRSubreport)sender).ReportSource).OutiID.Value = Convert.ToInt64(GetCurrentColumnValue("OutiID"));
            ((PhieuNhanThuocSummary)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
        }
    }
}
