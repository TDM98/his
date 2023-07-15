using System.Collections.ObjectModel;
using DataEntities;
namespace aEMR.ViewContracts
{
    public interface IChooseBatchNumberForPrescription
    {
        OutwardDrugMedDept SelectedOutwardDrug{get;set;}
        ObservableCollection<GetGenMedProductForSell> BatchNumberListShow { get; set; }
        ObservableCollection<OutwardDrugMedDept> OutwardDrugListByGenMedProductID { get; set; }
        int FormType { get; set; }
    }
}
