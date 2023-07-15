using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionInpt_V2_SubReport_TV : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptEPrescriptionInpt_V2_SubReport_TV()
        {
            InitializeComponent();
        }

        private void XRptEPrescriptionInpt_V2_SubReport_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

        private void xrSubreport1_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = parIssueID.Value.ToString() + "1".ToString();
            ((XRptEPrescriptionInpt_TV)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionInpt_TV)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1]!=2"; // toa thuốc gây nghiện
            ((XRptEPrescriptionInpt_TV)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value.ToString();
        }
    }
}
