namespace aEMR.ViewContracts
{
    public interface ISuppliersXapNhapPXHangKyGoi
    {
        long V_MedProductType { get; set; }
        bool IsChildWindow { get; set; }
        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mChinhSua { get; set; }
    }
}
