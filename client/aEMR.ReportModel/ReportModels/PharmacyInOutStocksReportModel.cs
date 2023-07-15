using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    public class PharmacyInOutStock_NTReportModel : ReportModelBase
    {
        public PharmacyInOutStock_NTReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptInOutStockValue_NT")
        {

        }
    }
    public class PharmacyInOutStocksReportModel : ReportModelBase
    {
        public PharmacyInOutStocksReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptInOutStocks")
        {

        }
    }
    public class PharmacyInOutStockValueReportModel : ReportModelBase
    {
        public PharmacyInOutStockValueReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XRptInOutStockValue")
        {

        }
    }
    public class  NhapThuocHangThangReportModel : ReportModelBase
    {
        public NhapThuocHangThangReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.BaoCaoNhapThuocHangThang")
        {

        }
    }
    public class NhapHangThangTheoSoPhieuReportModel : ReportModelBase
    {
        public NhapHangThangTheoSoPhieuReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.BaoCaoNhapThuocHangThangInvoice")
        {

        }
    }
    public class BangKeChungTuThanhToanReportModel : ReportModelBase
    {
        public BangKeChungTuThanhToanReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.SupplierPharmacyPaymentReqs.XtraReports.SupplierPharmacyPaymentReqs")
        {

        }
    }
    public class PhieuDeNghiThanhToanReportModel : ReportModelBase
    {
        public PhieuDeNghiThanhToanReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.SupplierPharmacyPaymentReqs.XtraReports.PhieuDeNghiThanhToan")
        {

        }
    }
    public class PharmacyTheoDoiCongNoReportModel : ReportModelBase
    {
        public PharmacyTheoDoiCongNoReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.BaoCaoTheoDoiCongNo")
        {

        }
    }
    public class PharmacyTheoDoiSoLuongThuocReportModel : ReportModelBase
    {
        public PharmacyTheoDoiSoLuongThuocReportModel()
            : base("eHCMS.ReportLib.RptPharmacies.XtraReports.ReportGeneral.XRpt_TheoDoiSLThuocTheoDuocChinh")
        {

        }
    }
}
