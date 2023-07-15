using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class PatientPaymentReportModel : ReportModelBase
    {
        public PatientPaymentReportModel()
            : base("eHCMS.ReportLib.RptPatientRegistration.XRptPatientPayment")
        {
        }
        ~PatientPaymentReportModel()
        {

        }
    }
}
