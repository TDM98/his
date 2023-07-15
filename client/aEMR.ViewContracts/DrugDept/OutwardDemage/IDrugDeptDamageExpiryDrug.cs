

namespace aEMR.ViewContracts
{
    public interface IDrugDeptDamageExpiryDrug
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mTim { get; set; }
        bool mThemMoi{ get; set; }
        bool mXuatExcel{ get; set; }
        bool mXemIn { get; set; }

    }
}
