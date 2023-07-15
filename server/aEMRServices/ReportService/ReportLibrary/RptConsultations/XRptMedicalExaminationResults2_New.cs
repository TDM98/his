using System;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptMedicalExaminationResults2_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptMedicalExaminationResults2_New()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsMedicalExaminationResult1.EnforceConstraints = false;
            spGetMedicalExaminationResultByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetMedicalExaminationResultByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
            spGetPatientDetail_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));

            if (dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID != null && dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID.Rows.Count > 0)
            {
                if (Convert.ToString(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID.FirstOrDefault()["Gender"]) == "Nữ")
                {
                    xrPictureBox7.Visible = true;
                    xrTableCell85.Visible = true;
                    xrTableCell86.Visible = true;
                    xrTableCell87.Visible = true;
                    xrTableCell88.Visible = true;
                    xrTableCell89.Visible = true;
                }
            }
        }

        private void XRptMedicalExaminationResults2_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
