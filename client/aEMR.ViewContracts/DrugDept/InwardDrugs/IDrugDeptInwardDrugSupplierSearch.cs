using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptInwardDrugSupplierSearch
    {
        long? TypID { get; set; }
        InwardInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<InwardDrugMedDeptInvoice> InwardInvoiceList { get; set; }
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
        bool IsConsignment { get; set; }
    }
}
