using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IPharmacyDamageExpiryDrugSearch
    {
        SearchOutwardInfo SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugInvoice> OutwardDrugInvoiceList { get; set; }
        string pageTitle { get; set; }
    }
}
