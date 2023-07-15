using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class ConsultationRoomDetailsReportModel: ReportModelBase
    {
        public ConsultationRoomDetailsReportModel()
            : base("eHCMS.ReportLib.RptConsultations.XRptBangKeChiTietKhamBenh")
        {
        }
    }

    public class BangKeChiTietKB : ReportModelBase
    {
        public BangKeChiTietKB()
            : base("eHCMS.ReportLib.RptConsultations.BangKeChiTietKB")
        {
        }
    }
}
