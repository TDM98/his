namespace aEMR.ViewContracts
{
    public interface IDrugDeptTheKho
    {
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool mXem { get; set; }
        bool mIn { get; set; }
    }
}
