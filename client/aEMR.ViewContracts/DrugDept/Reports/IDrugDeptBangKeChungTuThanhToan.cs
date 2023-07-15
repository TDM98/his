
namespace aEMR.ViewContracts
{
    public interface IDrugDeptBangKeChungTuThanhToan
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mChinhSua { get; set; }
        bool mXemInBK { get; set; }
        bool mXemInPDNTT { get; set; }
        
    }
}

