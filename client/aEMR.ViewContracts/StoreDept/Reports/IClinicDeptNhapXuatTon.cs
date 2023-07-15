namespace aEMR.ViewContracts
{
    public interface IClinicDeptNhapXuatTon
    {
        long V_MedProductType { get; set; }
        bool CanSelectedRefGenDrugCatID_1 { get; set; }
        bool mBaoCaoXuatNhapTon_XemIn { get; set; }
        bool mBaoCaoXuatNhapTon_KetChuyen { get; set; }

        void LoadRefGenericDrugCategory_1();

        bool IsGetValue { get; set; }
    }
}
