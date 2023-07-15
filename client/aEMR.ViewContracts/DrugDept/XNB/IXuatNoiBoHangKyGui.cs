using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IXuatNoiBoHangKyGui
    {
        long V_MedProductType { get; set; }
       // void InitializeInvoice();
        string strHienThi { get; set; }
        bool mTim { get; set; }
        bool mPhieuMoi { get; set; }
        bool mThucHien { get; set; }
        bool mThuTien { get; set; }
        bool mIn { get; set; }
        bool IsOutClinicDept { get; set; }
        bool mDeleteInvoice { get; set; }
        bool mPrintReceipt { get; set; }
        int ViewCase { get; set; }
        bool IsReturn { get; set; }
    }
}
