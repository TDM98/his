using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IPharmacyPurchaseOrderSearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PharmacyPurchaseOrder> PharmacyPurchaseOrderList { get; set; }
    }
}
