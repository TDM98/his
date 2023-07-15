using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IAddSupplierForDrug
    {
        SupplierGenericDrug SupplierDrug { get; set; }
        void SupplierGenericDrug_LoadDrugID(int PageIndex, int PageSize);
    }
}
