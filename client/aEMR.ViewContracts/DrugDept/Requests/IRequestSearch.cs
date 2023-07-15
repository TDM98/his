using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IRequestSearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<RequestDrugInwardClinicDept> RequestDruglist { get; set; }
        void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize);
        long V_MedProductType { get; set; }
         PagedSortableCollectionView<RequestDrugInwardForHiStore> RequestDruglistHIStore { get; set; }
        void SetList();
        bool IsRequestFromHIStore { get; set; }
        void SearchRequestDrugInwardHIStore(int PageIndex, int PageSize);
        bool IsFromApprovePage { get; set; }
    }
}
