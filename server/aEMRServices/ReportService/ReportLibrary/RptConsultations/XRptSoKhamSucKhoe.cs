using DevExpress.XtraReports.UI;
using System;

namespace eHCMS.ReportLib.RptConsultations.XtraReports
{
    public partial class XRptSoKhamSucKhoe : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptSoKhamSucKhoe()
        {
            InitializeComponent();
        }

        private void XRptSoKhamSucKhoe_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (IsShowExamAllResultDetails.Value as bool? == true)
            {
                MedicalExaminationExamAllResults.Visible = true;
                //((XRptMedicalExaminationExamAllResults)((XRSubreport)MedicalExaminationExamAllResults).ReportSource).Parameters["PtRegistrationID"].Value = parPtRegistrationID.Value as long?;
            }
            else
            {
                MedicalExaminationExamAllResults.Visible = false;
            }
        }

        private void xrSubreport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //((XRptPatientDeltail)((XRSubreport)sender).ReportSource).Parameters["parPtRegDetailID"].Value = parPtRegDetailID.Value;
            ((XRptPatientDetail_New)((XRSubreport)sender).ReportSource).Parameters["parPtRegistrationID"].Value = parPtRegistrationID.Value;
        }

        private void xrSubreport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //((XRptMedicalExaminationResults1)((XRSubreport)sender).ReportSource).Parameters["parPtRegDetailID"].Value = parPtRegDetailID.Value;
            ((XRptMedicalExaminationResults1_New)((XRSubreport)sender).ReportSource).Parameters["parPtRegistrationID"].Value = parPtRegistrationID.Value;
        }

        private void xrSubreport3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //((XRptMedicalExaminationResults2)((XRSubreport)sender).ReportSource).Parameters["parPtRegDetailID"].Value = parPtRegDetailID.Value;
            ((XRptMedicalExaminationResults2_New)((XRSubreport)sender).ReportSource).Parameters["parPtRegistrationID"].Value = parPtRegistrationID.Value;
        }
        private void xrSubreport4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            //((XRptMedicalExaminationResults2)((XRSubreport)sender).ReportSource).Parameters["parPtRegDetailID"].Value = parPtRegDetailID.Value;
            ((XRptMedicalExaminationResults3_New)((XRSubreport)sender).ReportSource).Parameters["parPtRegistrationID"].Value = parPtRegistrationID.Value;
        }

        private void XRptPatientFinal_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            ((XRptPatientFinal)((XRSubreport)sender).ReportSource).Parameters["parPtRegistrationID"].Value = parPtRegistrationID.Value;
        }

        private void MedicalExaminationExamAllResults_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            switch (ExaminationResultVersion.Value)
            {
                case 1:
                    MedicalExaminationExamAllResults.ReportSource = new XRptMedicalExaminationExamAllResults();
                    ((XRptMedicalExaminationExamAllResults)((XRSubreport)sender).ReportSource).Parameters["PtRegistrationID"].Value = parPtRegistrationID.Value;
                    break;
                case 2:
                    MedicalExaminationExamAllResults.ReportSource = new XRptMedicalExaminationExamAllResults_V2();
                    ((XRptMedicalExaminationExamAllResults_V2)((XRSubreport)sender).ReportSource).Parameters["PtRegistrationID"].Value = parPtRegistrationID.Value;
                    break;
            }
        }
    }
}