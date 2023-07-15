namespace eHCMS.ReportLib.RptConsultations
{
    public partial class XRptMedicalExaminationExamAllResults_V2 : DevExpress.XtraReports.UI.XtraReport
    {
        private bool IsPrinted = false;
        public XRptMedicalExaminationExamAllResults_V2()
        {
            InitializeComponent();
        }

        private void Report_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            dsMedicalExaminationExamAllResults1.EnforceConstraints = false;
            spRptMedicalExaminationExamAllResultsTableAdapter.Fill(dsMedicalExaminationExamAllResults1.spRptMedicalExaminationExamAllResults, PtRegistrationID.Value as long?);
            spGetPatientDetail_ByPtRegistrationIDTableAdapter.Fill(dsMedicalExaminationExamAllResults1.spGetPatientDetail_ByPtRegistrationID, PtRegistrationID.Value as long?);
            //DataSource = null;
        }
    }
}
