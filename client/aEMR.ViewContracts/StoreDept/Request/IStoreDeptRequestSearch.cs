using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IStoreDeptRequestSearch
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<RequestDrugInwardClinicDept> RequestDruglist { get; set; }
        void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize);
        long V_MedProductType { get; set; }
        bool IsCreateNewListFromSelectExisting { get; set; }
        long FilterByPtRegistrationID { get; set; }

        PagedSortableCollectionView<RequestDrugInwardForHiStore> RequestDruglistHIStore { get; set; }
        bool IsRequestFromHIStore { get; set; }
        void SetList();
        PagedSortableCollectionView<RequestFoodClinicDept> RequestFoodlist { get; set; }
    }
}
