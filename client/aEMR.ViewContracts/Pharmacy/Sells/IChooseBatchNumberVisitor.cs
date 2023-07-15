using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IChooseBatchNumberVisitor
    {
        OutwardDrug SelectedOutwardDrug{get;set;}
        ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow { get; set; }
        ObservableCollection<OutwardDrug> OutwardDrugListByDrugID { get; set; }
        int FormType { get; set; }
    }
}
