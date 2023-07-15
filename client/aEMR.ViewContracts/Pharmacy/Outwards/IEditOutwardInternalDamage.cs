using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IEditOutwardInternalDamage
    {
        string TitleForm { get; set; }

        OutwardDrugInvoice SelectedOutInvoice { get; set; }
        ObservableCollection<OutwardDrug> OutwardDrugListCopy { get; set; }
        OutwardDrugInvoice SelectedOutInvoiceCoppy { get; set; }
        ObservableCollection<OutwardDrug> ListOutwardDrugFirstCopy { get; set; }
        ObservableCollection<OutwardDrug> ListOutwardDrugFirst { get; set; }
        decimal SumTotalPrice { get; set; }
        long IDFirst { get; set; }
    }
}
