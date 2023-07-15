using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IPurchaseOrderSearchEstimate
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PharmacyEstimationForPO> PharmacyEstimationForPOList { get; set; }
    }
}
