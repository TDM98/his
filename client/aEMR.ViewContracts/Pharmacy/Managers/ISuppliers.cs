using aEMR.Common.Collections;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface ISuppliers
    {
        bool IsChildWindow { get; set; }
        SupplierSearchCriteria SupplierCriteria { get; set; }
        PagedSortableCollectionView<Supplier> Suppliers { get; set; }
        string TitleForm { get; set; }
        eFirePharmacySupplierEvent ePharmacySupplierEvent { get; set; }
    }
}
