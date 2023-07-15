namespace aEMR.ViewContracts
{
    public interface IDrugDeptStockTakes
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }

        bool mTim { get; set; }
        bool mThemMoi { get; set; }
        bool mXemIn { get; set; }
        bool mXuatExcel { get; set; }
    }
}
