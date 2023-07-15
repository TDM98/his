using System;
using DevExpress.XtraReports.UI;
/*
 * 20200106 #001 TNHX: Init
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptInPatientPhieuChiDinhXML : XtraReport
    {
        public XRptInPatientPhieuChiDinhXML()
        {
            InitializeComponent();
        }

        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsInPatientPhieuChiDinh1.EnforceConstraints = false;
            sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter.Fill(dsInPatientPhieuChiDinh1.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXml, param_ListID.Value);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptInPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).param_DeptLocID.Value = Convert.ToInt32(GetCurrentColumnValue("DeptLocID"));
            ((XRptInPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).param_ItemType.Value = GetCurrentColumnValue("ItemType");
            ((XRptInPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).param_ListID.Value = GetCurrentColumnValue("IDList");
            ((XRptInPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
            ((XRptInPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
            ((XRptInPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value.ToString();
        }
    }
}
