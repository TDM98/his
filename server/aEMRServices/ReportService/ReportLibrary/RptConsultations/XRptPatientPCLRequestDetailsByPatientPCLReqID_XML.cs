using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientPCLRequestDetailsByPatientPCLReqID_XML : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientPCLRequestDetailsByPatientPCLReqID_XML()
        {
            InitializeComponent();
        }

        private void XRptPatientPCLRequestDetailsByPatientPCLReqID_XML_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }

        private void FillData()
        {
            this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.EnforceConstraints = false;
            //this.spRptPatientPCLRequestDetailsByPatientPCLReqIDXmlTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml, this.parPCLReqID_XML.Value.ToString(), Convert.ToInt32(this.paramV_RegistrationType.Value));
            //this.spRptPatientPCLRequestsHeaderTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader, Convert.ToInt32(this.parPtRegistrationID.Value), this.parPCLReqID_XML.Value.ToString(), Convert.ToInt32(this.paramV_RegistrationType.Value));
            this.spRptPatientPCLRequestDetailsByPatientPCLReqIDXmlTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestDetailsByPatientPCLReqIDXml, this.parPCLReqID_XML.Value.ToString(), Convert.ToInt32(this.paramV_RegistrationType.Value));
            this.spRptPatientPCLRequestsHeaderTableAdapter.Fill(this.dsPatientPCLRequestDetailsByPatientPCLReqID_XML1.spRptPatientPCLRequestsHeader, Convert.ToInt32(this.parPtRegistrationID.Value), this.parPCLReqID_XML.Value.ToString(), Convert.ToInt32(this.paramV_RegistrationType.Value));
        }
    }
}