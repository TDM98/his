using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    using DevExpress.Xpf.Printing;
    public class PharmacyEstimationModel:ReportModelBase
    {
        public PharmacyEstimationModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptEstimatePharmacy")
        {

        }
    }
    public class PharmacyKiemKeVaDuTruReportModel : ReportModelBase
    {
        public PharmacyKiemKeVaDuTruReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_KiemKeVaDuTru")
        {

        }
    }
    public class PharmacyDuTruDuaTrenHeSoAnToanReportModel : ReportModelBase
    {
        public PharmacyDuTruDuaTrenHeSoAnToanReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_DuTruDuaTrenHeSoAnToan")
        {

        }
    }

    public class PharmacyTongHopDuTruTheoSoPhieuReportModel : ReportModelBase
    {
        public PharmacyTongHopDuTruTheoSoPhieuReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.Estimations.BaoCaoTongHopDuTru_SoPhieu")
        {

        }
    }
    public class PharmacyTongHopDuTruTheoTenThuocReportModel : ReportModelBase
    {
        public PharmacyTongHopDuTruTheoTenThuocReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.Estimations.BaoCaoTongHopDuTru_TenThuoc")
        {

        }
    }
    public class PharmacyHeSoAnToanBanReportModel : ReportModelBase
    {
        public PharmacyHeSoAnToanBanReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_HeSoAnToanBan")
        {

        }
    }
}
