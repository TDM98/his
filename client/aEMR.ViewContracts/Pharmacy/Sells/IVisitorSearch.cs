using aEMR.Common.Collections;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IVisitorSearch
    {
        SearchOutwardInfo SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugInvoice> OutwardInfoList { get; set; }
    }
}
