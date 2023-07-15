using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    using DevExpress.Xpf.Printing;
    public class PharmacyStockByEveryStorage : ReportModelBase
    {
        public PharmacyStockByEveryStorage()
            : base("eHCMS.ReportLib.RptPharmacies.XRptGroupStorages")
        {

        }
    }
    public class BangKeChiTietPhatThuoc : ReportModelBase
    {
        public BangKeChiTietPhatThuoc()
            : base("eHCMS.ReportLib.RptPharmacies.XRpt_BangKeChiTietPhatThuoc")
        {

        }
    }
    public class BaoCaoNopTienHangNgay : ReportModelBase
    {
        public BaoCaoNopTienHangNgay()
            : base("eHCMS.ReportLib.RptPharmacies.XRpt_BaoCaoNopTienHangNgay")
        {

        }
    }

    public class BaoCaoNopTienHangNgayChiTietModel : ReportModelBase
    {
        public BaoCaoNopTienHangNgayChiTietModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRpt_BaoCaoNopTienChiTiet")
        {

        }
    }
    //XRpt_PhieuKiemKe
    public class InPhieuKiemKeModel : ReportModelBase
    {
        public InPhieuKiemKeModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRpt_PhieuKiemKe")
        {

        }
    }

    public class TestSubreport : ReportModelBase
    {
        public TestSubreport()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReport1")
        {

        }
    }

    //XRpt_PhieuKiemKe
    public class TongHopDoanhThuReportModel : ReportModelBase
    {
        public TongHopDoanhThuReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_TongHopDoanhThu")
        {

        }
    }

}
