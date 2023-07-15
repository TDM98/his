using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IStockTakesSearch
    {
        PharmacyStockTakesSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<PharmacyStockTakes> PharmacyStockTakeList { get; set; }
    }
}
