using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.ViewContracts
{
    public interface IDrugDeptBangKeChungTuThanhToanSearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<SupplierDrugDeptPaymentReqs> SupplierDrugDeptPaymentReqList { get; set; }
        long V_MedProductType { get; set; }
        string strHienThi { get; set; }
    }
}
