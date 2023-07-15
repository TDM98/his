using DataEntities;
using DevExpress.Xpf.Printing;
/*
* 20190610 #001 TNHX: [BM0010768] Create XRpt_BCXuatDuocNoiBo_NT
*/
namespace aEMR.ViewContracts
{
    public interface IReportByDDMMYYYY
    {
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        string pageTitle { get; set; }
        bool bXem { get; set; }
        bool bIn { get; set; }
        bool bXuatExcel { get; set; }

        //▼====: 20210321 QTD
        bool IsShowPaymentMode { get; set; }
        //▲====: 20210321 QTD
        bool BXemChiTiet { get; set; }

        //▼====: #001
        void ViewByDate();
        bool ShowTongKho { get; set; }
        //▲====: #001
    }
}
