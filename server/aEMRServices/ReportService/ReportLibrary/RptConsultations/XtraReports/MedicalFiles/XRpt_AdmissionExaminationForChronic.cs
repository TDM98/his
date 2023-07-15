using System;

namespace eHCMS.ReportLib.RptConsultations.XtraReports.MedicalFiles
{
    public partial class XRpt_AdmissionExaminationForChronic : DevExpress.XtraReports.UI.XtraReport
    {
        public XRpt_AdmissionExaminationForChronic()
        {
            InitializeComponent();
        }

        private void XRpt_AdmissionExaminationForChronic_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
        private void FillData()
        {
            dsXRpt_AdmissionExamination1.EnforceConstraints = false;
            spXRpt_AdmissionExaminationTableAdapter.Fill(dsXRpt_AdmissionExamination1.spXRpt_AdmissionExamination
                , Convert.ToInt64(PtRegistrationID.Value)
                , Convert.ToInt64(V_RegistrationType.Value));
        }
    }
}
