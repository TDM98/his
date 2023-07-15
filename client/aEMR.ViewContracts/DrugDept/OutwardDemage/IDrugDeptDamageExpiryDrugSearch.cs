
using aEMR.Common.Collections;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptDamageExpiryDrugSearch
    {
        MedDeptInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoiceList { get; set; }
    }
}
