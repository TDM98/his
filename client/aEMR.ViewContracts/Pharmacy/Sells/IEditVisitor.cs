using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IEditVisitor
    {
        OutwardDrugInvoice SelectedOutwardInfo { get; set; }
        ObservableCollection<OutwardDrug> OutwardDrugListCopy { get; set; }
        OutwardDrugInvoice SelectedOutwardInfoCoppy { get; set; }
        ObservableCollection<OutwardDrug> ListOutwardDrugFirstCopy { get; set; }
        ObservableCollection<OutwardDrug> ListOutwardDrugFirst { get; set; }
        decimal SumTotalPrice { get; set; }
        long IDFirst { get; set; }
        void SetDefaultForStore();
    }
}
