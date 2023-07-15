using aEMR.DataAccessLayer.Providers;
using System;

namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptPatientFinal : DevExpress.XtraReports.UI.XtraReport
    {
        public XRptPatientFinal()
        {
            InitializeComponent();
        }

        private void FillData()
        {
            dsMedicalExaminationResult1.EnforceConstraints = false;
            spGetPatientDetail_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationResult1.spGetPatientDetail_ByPtRegistrationID, Convert.ToInt64(parPtRegistrationID.Value));
            ReportSqlProvider.Instance.ReaderIntoSchema(dsMedicalExaminationResult1.spGetFinalMedicalExaminationResult_ByPtRegistrationID
                , spGetFinalMedicalExaminationResult_ByPtRegistrationIDTableAdapter1.Adapter.GetFillParameters(), new object[] {
                Convert.ToInt64(parPtRegistrationID.Value)
            }, int.MaxValue);
        }

        private void XRptPatientFinal_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            FillData();
        }
    }
}
