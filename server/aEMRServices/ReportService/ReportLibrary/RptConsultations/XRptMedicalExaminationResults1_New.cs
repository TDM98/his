using System;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptMedicalExaminationResults1_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptMedicalExaminationResults1_New()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsMedicalExaminationResult1.EnforceConstraints = false;
            spGetMedicalExaminationResultByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetMedicalExaminationResultByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
            spGetPatientDetail_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
            spGetPhysicalExamination_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetPhysicalExamination_ByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
        }

        private void XRptMedicalExaminationResults1_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
