using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IChooseBatchNumber
    {
        OutwardDrugMedDept SelectedOutwardDrug { get; set; }
        ObservableCollection<RefGenMedProductDetails> BatchNumberListShow { get; set; }
        ObservableCollection<OutwardDrugMedDept> OutwardDrugListByDrugID { get; set; }
        int FormType { get; set; }
    }
}
