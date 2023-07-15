using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IXuatNoiBoSearch
    {
        MedDeptInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugMedDeptInvoice> OutwardMedDeptInvoiceList { get; set; }
        string strHienThi { get; set; }
    }
}
