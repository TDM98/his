using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptPurchaseOrderSearchEstimate
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DrugDeptEstimationForPO> DrugDeptEstimationForPOList { get; set; }

        long V_MedProductType { get; set; }
    }
}
