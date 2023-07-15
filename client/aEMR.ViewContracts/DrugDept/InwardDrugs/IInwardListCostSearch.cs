using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IInwardListCostSearch
    {
        InwardInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<CostTableMedDept> InwardInvoiceList { get; set; }
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
    }
}
