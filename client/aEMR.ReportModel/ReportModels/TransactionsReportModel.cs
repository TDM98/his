/*
 * 20170217 #001 CMN: Add TotalInPtRevenue Report
 * 20190622 #002 TNHX: [BM0011874] Create report RptTongHopDoanhThuTheoKhoa
*/
using aEMR.ReportModel.BaseModels;

namespace aEMR.ReportModel.ReportModels
{
    using DevExpress.Xpf.Printing;
    public class TransactionsReportModel : ReportModelBase
    {
        public TransactionsReportModel()
            : base("eHCMS.ReportLib.RptTransactions.XRptDoanhThuTongHop")
        {

        }
    }

    public class RptTongHopDoanhThuNoiTruReportModel : ReportModelBase
    {
        //==== #001
        /*
        public RptTongHopDoanhThuNoiTruReportModel()
            : base("eHCMS.ReportLib.RptTransactions.XRptDoanhThuTongHopNoiTru")
        {

        }
        */
        public RptTongHopDoanhThuNoiTruReportModel()
            : base("eHCMS.ReportLib.RptTransactions.XtraReports.XRptTotalInPtRevenue")
        {

        }
        //==== #001
    }

    public class RptHoatDongPhongDangKyReportModel : ReportModelBase
    {
        public RptHoatDongPhongDangKyReportModel()
            : base("eHCMS.ReportLib.RptTransactions.XRptHoatDongPhongDangKy")
        {

        }
    }
   
    //▼====: #002
    public class RptTongHopDoanhThuTheoKhoaReportModel : ReportModelBase
    {
        public RptTongHopDoanhThuTheoKhoaReportModel()
            : base("eHCMS.ReportLib.RptTransactions.XRpt_TongHopDoanhThuTheoKhoa")
        {
        }
    }
    //▲====: #002
}
