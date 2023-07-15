using System;
using DevExpress.XtraReports.UI;
using eHCMS.ReportLib.RptConsultations;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML()
        {
            InitializeComponent();
        }

        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPatientPCLRequestDetails1.EnforceConstraints = false;
            sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXmlTableAdapter.Fill(dsPatientPCLRequestDetails1.sp_Rpt_spPatientPCLRequestDetails_ByPatientPCLReqIDXml
                , parPatientPCLReqID.Value);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV)((XRSubreport)sender).ReportSource).paramV_RegistrationType.Value = paramV_RegistrationType.Value;
            ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV)((XRSubreport)sender).ReportSource).parPatientPCLReqID.Value = Convert.ToInt64(GetCurrentColumnValue("PatientPCLReqID"));
            ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
            ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value;
        }
    }
}
