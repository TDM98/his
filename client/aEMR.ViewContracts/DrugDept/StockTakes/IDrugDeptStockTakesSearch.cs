using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IDrugDeptStockTakesSearch
    {
        string strHienThi { get; set; }
        DrugDeptStockTakesSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<DrugDeptStockTakes> DrugDeptStockTakeList { get; set; }
        long V_MedProductType { get; set; }
    }
}
