using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IReturnDrugSearch
    {
        SearchOutwardInfo SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugInvoice> OutwardInfoList { get; set; }
    }
}
