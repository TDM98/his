using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IBangKeChungTuThanhToanSearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<SupplierPharmacyPaymentReqs> SupplierPharmacyPaymentReqList { get; set; }
    }
}
