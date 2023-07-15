using System;
using System.Linq;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptMedicalExaminationResults3_New : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptMedicalExaminationResults3_New()
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
                    xrTableCell29.Text = "6. Răng - Hàm - Mặt:";
                    xrTableCell43.Text = "7. Da liễu:";
                }
            }
        }

        private void XRptMedicalExaminationResults3_New_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
