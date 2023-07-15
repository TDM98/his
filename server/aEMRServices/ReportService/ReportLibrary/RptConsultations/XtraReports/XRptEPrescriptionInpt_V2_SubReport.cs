using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionInpt_V2_SubReport : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionInpt_V2_SubReport()
        {
            InitializeComponent();
        }

        private void XRptEPrescriptionInpt_V2_SubReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

        private void xrSubreport1_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = this.parIssueID.Value.ToString() + "1".ToString();
            ((XRptEPrescriptionNew_InPt_V2)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionNew_InPt_V2)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1]!=2"; // toa thuốc gây nghiện
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = this.parIssueID.Value.ToString() + "2".ToString();
            ((XRptEPrescriptionNew_InPt_V2)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionNew_InPt_V2)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1] = 2"; // toa thuốc hướng thần
        }
    }
}
