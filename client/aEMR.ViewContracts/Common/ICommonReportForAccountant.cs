
using DataEntities;
/*
* 20190603 #001 TNHX: [BMBM0011782] Init new view for Report NXTheoMucDich
*/
namespace aEMR.ViewContracts
{
    public interface ICommonReportForAccountant
    {
        ReportName eItem { get; set; }

        ReportParameters RptParameters { get; set; }

        string strHienThi { get; set; }

        bool mXemIn { get; set; }

        bool mXemChiTiet { get; set; }

        bool mXemChiTietTheoThuoc { get; set; }

        bool IsEnabledToDatePicker { get; set; }

        void GetListStore(long? StoreType);

        bool ChonKho { get; set; }

        bool ViewBy { get; set; }

        bool IsPurpose { get; set; }
        bool CanSelectedRefGenDrugCatID_1 { get; set; }
        bool IsShowGroupReportType { get; set; }
    }
}
