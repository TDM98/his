using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class ConsultationPCLFormReportModel: ReportModelBase
    {
        public ConsultationPCLFormReportModel()
            : base("eHCMS.ReportLib.RptConsultations.XRptPCLForm")
        {
        }
        ~ConsultationPCLFormReportModel()
        {}
    }
}
