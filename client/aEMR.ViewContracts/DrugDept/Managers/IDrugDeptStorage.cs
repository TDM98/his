namespace aEMR.ViewContracts
{
    public interface IDrugDeptStorage
    {
        bool IsChildWindow { get; set; }
        bool IsSubStorage { get; set; }
        long V_MedProductType { get; set; }
        long V_GroupTypes { get; set; }//-- 28/12/2020 DatTB
        bool bAdd { get; set; }
        //▼====: 20210922
        bool IsMainStorage { get; set; }
        long StoreTypeID { get; set; }
    }
}