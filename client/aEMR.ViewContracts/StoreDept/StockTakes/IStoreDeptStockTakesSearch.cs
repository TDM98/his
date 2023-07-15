using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IStoreDeptStockTakesSearch
    {
        ClinicDeptStockTakesSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<ClinicDeptStockTakes> ClinicDeptStockTakeList { get; set; }
    }
}
