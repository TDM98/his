namespace aEMR.ViewContracts
{
    public interface IDrugDeptInwardDrugSupplier
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mIn { get; set; }
        bool mCapNhat { get; set; }
        bool IsRetundView { get; set; }
    }
}
