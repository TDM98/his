using DataEntities;
using aEMR.Common.Collections;
namespace aEMR.ViewContracts
{
    public interface IRefGenDrugList
    {
        DrugSearchCriteria SearchCriteria { get; set; }
        bool IsPopUp { get; set; }
        PagedSortableCollectionView<RefGenericDrugDetail> DrugsResearch { get; set; }
        string TitleForm { get; set; }
    }

    public interface IRefGenDrugListNew
    {
        DrugSearchCriteria SearchCriteria { get; set; }
        bool IsPopUp { get; set; }
        PagedSortableCollectionView<RefGenericDrugDetail> DrugsResearch { get; set; }
        string TitleForm { get; set; }
    }

}
