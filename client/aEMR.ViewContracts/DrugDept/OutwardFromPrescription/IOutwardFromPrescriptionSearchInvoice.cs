using aEMR.Common.Collections;
using DataEntities;
using System.Collections.Generic;
namespace aEMR.ViewContracts
{
    public interface IOutwardFromPrescriptionSearchInvoice
    {
        SearchOutwardInfo SearchInvoiceCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugMedDeptInvoice> OutwardDrugMedDeptInvoices { get; set; }
    }
}
