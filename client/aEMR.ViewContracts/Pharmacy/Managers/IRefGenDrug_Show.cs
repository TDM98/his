using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IRefGenDrug_Show
    {
        RefGenericDrugDetail SelectedDrug {get;set;}
    }

    public interface IRefGenDrug_ShowNew
    {
        RefGenericDrugDetail SelectedDrug { get; set; }
    }
}
