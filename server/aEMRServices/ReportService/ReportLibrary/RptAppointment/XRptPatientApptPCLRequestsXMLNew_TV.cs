using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptAppointment
{
    public partial class XRptPatientApptPCLRequestsXMLNew_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientApptPCLRequestsXMLNew_TV()
        {
            InitializeComponent();
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptPatientApptPCLRequests_TV)((XRSubreport)sender).ReportSource).parPatientPCLReqID.Value = Convert.ToInt32(GetCurrentColumnValue("RequestID"));
            ((XRptPatientApptPCLRequests_TV)((XRSubreport)sender).ReportSource).parLogoUrl.Value = parLogoUrl.Value.ToString();
            ((XRptPatientApptPCLRequests_TV)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
        }

        private void XRptPatientApptPCLRequestsXMLNew_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            spRptPatientApptPCLRequestsXmlTableAdapter.Fill(dsPatientApptPCLRequests1.spRptPatientApptPCLRequestsXml, parRequestXML.Value);
        }
    }
}
