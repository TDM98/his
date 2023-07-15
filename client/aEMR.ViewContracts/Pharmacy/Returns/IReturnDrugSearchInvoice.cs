using aEMR.Common.Collections;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IReturnDrugSearchInvoice
    {
        SearchOutwardInfo SearchCriteriaReturn { get; set; }
        PagedSortableCollectionView<OutwardDrugInvoice> OutwardDrugInvoices { get; set; }
    }
}
