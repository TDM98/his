using System;
using DevExpress.XtraReports.UI;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptEPrescriptionInpt_V2_SubReport_TV3 : XtraReport
    {
        public XRptEPrescriptionInpt_V2_SubReport_TV3()
        {
            InitializeComponent();
        }

        private void XRptEPrescriptionInpt_V2_SubReport_TV_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
        }

        private void xrSubreport1_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string parIssueIDstr = parIssueID.Value.ToString() + "1".ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).Parameters["parIssueID"].Value = Convert.ToInt32(parIssueIDstr); // erro cast
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).FilterString = "[RefGenDrugCatID_1]!=2"; // toa thuốc gây nghiện
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parHospitalCode.Value = parHospitalCode.Value.ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parPrescriptionMainRightHeader.Value = parPrescriptionMainRightHeader.Value.ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parPrescriptionSubRightHeader.Value = parPrescriptionSubRightHeader.Value.ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parHospitalAddress.Value = parHospitalAddress.Value.ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parDepartmentOfHealth.Value = parDepartmentOfHealth.Value.ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parHospitalName.Value = parHospitalName.Value.ToString();
            ((XRptEPrescriptionInpt_TV3)((XRSubreport)sender).ReportSource).parKBYTLink.Value = parKBYTLink.Value.ToString();
            
        }
    }
}
