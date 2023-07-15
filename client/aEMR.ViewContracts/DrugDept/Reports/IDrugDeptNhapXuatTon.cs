using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDrugDeptNhapXuatTon
    {
        long V_MedProductType { get; set; }
        bool CanSelectedRefGenDrugCatID_1 { get; set; }
        string strHienThi { get; set; }
        bool mKetChuyen { get; set; }
        bool mXemIn { get; set; }
        void LoadRefGenericDrugCategory_1();
        bool IsGetValue { get; set; }
        bool IsShowClinic { get; set; }
        void LoadDrugDeptProductGroupReportTypes();
        bool CanSelectedWareHouse { get; set; }
        bool CanPrint { get; set; }
        bool IsCheck { get; set; }
        ReportName eItem { get; set; }
        ReportParameters RptParameters { get; set; }
        bool ShowBid { get; set; }
        bool ShowRangeOfHospital { get; set; }
        bool IsShowBHYT { get; set; }
    }
}
