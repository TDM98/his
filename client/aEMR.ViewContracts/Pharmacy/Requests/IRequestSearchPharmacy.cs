using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IRequestSearchPharmacy
    {
        RequestSearchCriteria SearchCriteria { get; set; }
        PagedSortableCollectionView<RequestDrugInward> RequestDruglist { get; set; }
        void SearchRequestDrugInward(int PageIndex,int PageSize);
    }
}
