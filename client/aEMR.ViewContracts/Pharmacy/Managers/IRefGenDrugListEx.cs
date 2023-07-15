using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRefGenDrugListEx
    {
        DrugSearchCriteria SearchCriteria { get; set; }
        bool IsPopUp { get; set; }
        string TitleForm { get; set; }
    }
}
