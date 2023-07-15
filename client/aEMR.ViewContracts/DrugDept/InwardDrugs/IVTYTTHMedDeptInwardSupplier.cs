namespace aEMR.ViewContracts
{
    public interface IVTYTTHMedDeptInwardSupplier
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mIn { get; set; }
        bool mCapNhat { get; set; }
    }
}
