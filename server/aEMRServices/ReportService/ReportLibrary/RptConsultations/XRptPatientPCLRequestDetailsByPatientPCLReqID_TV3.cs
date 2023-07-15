using System;
using DevExpress.XtraReports.UI;
using eHCMS.ReportLib.RptConsultations;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLRequestDetailsByPatientPCLReqID_TV3()
        {
            InitializeComponent();
        }

        private void XtraReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsPatientPCLRequestDetails1.EnforceConstraints = false;
            sp_RptPatientPCLRequestXMLTableAdapter.Fill(dsPatientPCLRequestDetails1.sp_RptPatientPCLRequestXML, parPatientPCLReqXML.Value);
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (Convert.ToInt32(GetCurrentColumnValue("HasDeptGroup")) == 1) // danh sách phiếu cùng phòng
            {
                xrSubreport1.ReportSource = new XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV();
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV)((XRSubreport)sender).ReportSource).paramV_RegistrationType.Value = Convert.ToInt32(paramV_RegistrationType.Value);
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV)((XRSubreport)sender).ReportSource).parPtRegistrationID.Value = Convert.ToInt32(parPtRegistrationID.Value);
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV)((XRSubreport)sender).ReportSource).parPCLReqID_XML.Value = Convert.ToString(GetCurrentColumnValue("PatientPCLReqIDStr"));
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value;
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_TV)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value;
            }
            else
            {
                xrSubreport1.ReportSource = new XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML();
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML)((XRSubreport)sender).ReportSource).paramV_RegistrationType.Value = Convert.ToInt32(paramV_RegistrationType.Value);
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML)((XRSubreport)sender).ReportSource).parPatientPCLReqID.Value = Convert.ToString(GetCurrentColumnValue("PatientPCLReqIDStr"));
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
                ((XRptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value.ToString();
            }
        }
    }
}
