using System;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptMedicalExaminationResults1 : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptMedicalExaminationResults1()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsMedicalExaminationResult1.EnforceConstraints = false;
            spGetMedicalExaminationResultByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetMedicalExaminationResultByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
            spGetPatientDetail_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
            spGetPhysicalExamination_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetPhysicalExamination_ByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));

            if (dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID != null && dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID.Rows.Count > 0)
            {
                if (Convert.ToString(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID.FirstOrDefault()["Gender"]) == "Nữ")
                {
                    xrPictureBox3.Visible = true;
                    xrTableCell47.Visible = true;
                    xrTableCell48.Visible = true;
                    xrTableCell49.Visible = true;
                    xrTableCell41.Visible = true;
                    xrTableCell42.Visible = true;
                }
            }
        }

        private void XRptMedicalExaminationResults1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
