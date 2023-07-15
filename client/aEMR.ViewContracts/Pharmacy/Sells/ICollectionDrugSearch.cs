using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface ICollectionDrugSearch
    {
        SearchOutwardInfo SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugInvoice> OutwardInfoList { get; set; }
        string pageTitle { get; set; }
        bool? bFlagStoreHI { get; set; }
        bool bFlagPaidTime { get; set; }
    }
}
