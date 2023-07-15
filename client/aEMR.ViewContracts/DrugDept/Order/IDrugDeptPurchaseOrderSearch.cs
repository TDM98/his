
using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptPurchaseOrderSearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DrugDeptPurchaseOrder> DrugDeptPurchaseOrderList { get; set; }
        long V_MedProductType { get; set; }
    }
}
