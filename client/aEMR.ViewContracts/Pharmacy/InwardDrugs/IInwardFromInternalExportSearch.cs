using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IInwardFromInternalExportSearch
    {
        long? TypID { get; set; }
        InwardInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<InwardDrugInvoice> InwardInvoiceList { get; set; }
    }
}
