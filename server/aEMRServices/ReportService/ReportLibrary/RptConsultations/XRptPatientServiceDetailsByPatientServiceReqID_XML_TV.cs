using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientServiceDetailsByPatientServiceReqID_XML_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientServiceDetailsByPatientServiceReqID_XML_TV()
        {
            InitializeComponent();
        }

        private void XRptPatientServiceDetailsByPatientServiceReqID_XML_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            dsPatientServiceRequestDetailsByPatientServiceReqID_XML1.EnforceConstraints = false;

            spRptPatientServiceRequestHeaderTableAdapter.ClearBeforeFill = true;
            spRptPatientServiceRequestHeaderTableAdapter.Fill(dsPatientServiceRequestDetailsByPatientServiceReqID_XML1.spRptPatientServiceRequestHeader, Convert.ToInt32(parPtRegistrationID.Value), Convert.ToInt32(paramV_RegistrationType.Value));

            spRptPatientServiceRequestDetailsByPatientServiceReqIDXmlTableAdapter.ClearBeforeFill = true;
            spRptPatientServiceRequestDetailsByPatientServiceReqIDXmlTableAdapter.Fill(dsPatientServiceRequestDetailsByPatientServiceReqID_XML1.spRptPatientServiceRequestDetailsByPatientServiceReqIDXml, parRequestXML.Value, Convert.ToInt32(paramV_RegistrationType.Value));
        }
    }
}
