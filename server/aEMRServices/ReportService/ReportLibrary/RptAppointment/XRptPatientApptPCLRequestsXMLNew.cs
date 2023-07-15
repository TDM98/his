using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptPCLRequestsXMLNew : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptPCLRequestsXMLNew()
        {
            InitializeComponent();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptPatientApptPCLRequests)((XRSubreport)sender).ReportSource).parPatientPCLReqID.Value = Convert.ToInt32(GetCurrentColumnValue("RequestID"));
            ((XRptPatientApptPCLRequests)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value.ToString();
            ((XRptPatientApptPCLRequests)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
        }

        private void XRptPatientApptPCLRequestsXMLNew_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptPatientApptPCLRequestsXmlTableAdapter.Fill(dsPatientApptPCLRequests1.spRptPatientApptPCLRequestsXml, parRequestXML.Value);
        }
    }
}
