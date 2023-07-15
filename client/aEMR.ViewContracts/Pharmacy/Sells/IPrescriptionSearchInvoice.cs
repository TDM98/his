using aEMR.Common.Collections;
using DataEntities;
using System.Collections.Generic;
namespace aEMR.ViewContracts
{
    public interface IPrescriptionSearchInvoice
    {
        SearchOutwardInfo SearchInvoiceCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugInvoice> OutwardDrugInvoices { get; set; }
    }
}
