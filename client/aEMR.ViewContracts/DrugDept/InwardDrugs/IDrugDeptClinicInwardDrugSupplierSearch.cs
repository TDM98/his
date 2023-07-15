using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptClinicInwardDrugSupplierSearch
    {
        long? TypID { get; set; }
        InwardInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<InwardDrugClinicDeptInvoice> InwardInvoiceList { get; set; }
        long V_MedProductType { get; set; }
    }
}
