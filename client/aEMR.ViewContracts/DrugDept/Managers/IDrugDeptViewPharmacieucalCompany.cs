using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptViewPharmacieucalCompany
    {
        DrugDeptPharmaceuticalCompany SelectedDrugDeptPharmaceuticalCompany { get; set; }
        void RefGenMedProductDetails_ByPCOID(int PageIndex, int PageSize);
    }
}
