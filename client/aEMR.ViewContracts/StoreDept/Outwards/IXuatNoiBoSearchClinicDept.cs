using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IXuatNoiBoSearchClinicDept
    {
        MedDeptInvoiceSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<OutwardDrugClinicDeptInvoice> OutwardClinicDeptInvoiceList { get; set; }
        string strHienThi { get; set; }
    }
}
