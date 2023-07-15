using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IChooseBatchNumberClinicDept
    {
        OutwardDrugClinicDept SelectedOutwardDrug { get; set; }
        ObservableCollection<RefGenMedProductDetails> BatchNumberListShow { get; set; }
        ObservableCollection<OutwardDrugClinicDept> OutwardDrugListByDrugID { get; set; }
        int FormType { get; set; }
    }
}
