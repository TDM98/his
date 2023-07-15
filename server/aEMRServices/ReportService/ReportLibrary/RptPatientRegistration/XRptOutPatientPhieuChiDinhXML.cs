using System;
using DevExpress.XtraReports.UI;
/*
 * 20181030 #001 TNHX: [BM0002176] Add params HospitalName, DepartmentOfHealth. Update report base on new flow (XacNhanQLBH)
 */
namespace eHCMS.ReportLib.RptPatientRegistration
{
    public partial class XRptOutPatientPhieuChiDinhXML : XtraReport
    {
        public XRptOutPatientPhieuChiDinhXML()
        {
            InitializeComponent();
        }

        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            outPatientPhieuChiDinh.EnforceConstraints = false;
            sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXmlTableAdapter.Fill(outPatientPhieuChiDinh.sp_Rpt_spReportOutPatientPhieuChiDinh_ByPaymentIDXml, param_ListID.Value);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptOutPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).param_DeptLocID.Value = Convert.ToInt32(GetCurrentColumnValue("DeptLocID"));
            ((XRptOutPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).param_ItemType.Value = GetCurrentColumnValue("ItemType");
            ((XRptOutPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).param_ListID.Value = GetCurrentColumnValue("IDList");
            ((XRptOutPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
            ((XRptOutPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
            ((XRptOutPatientPhieuChiDinh)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value.ToString();
        }
    }
}
