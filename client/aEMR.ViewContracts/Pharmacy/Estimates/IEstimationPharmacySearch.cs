using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IEstimationPharmacySearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PharmacyEstimationForPO> PharmacyEstimationForPOList { get; set; }
        bool IsHIStorage { get; set; }
    }
}
