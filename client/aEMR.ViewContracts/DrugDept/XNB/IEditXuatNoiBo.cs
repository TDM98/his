using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IEditXuatNoiBo
    {
        long V_MedProductType { get; set; }
        long IDFirst { get; set; }

        OutwardDrugMedDeptInvoice SelectedOutInvoice { get; set; }
        ObservableCollection<OutwardDrugMedDept> OutwardDrugListCopy { get; set; }
        OutwardDrugMedDeptInvoice SelectedOutInvoiceCoppy { get; set; }
        ObservableCollection<OutwardDrugMedDept> ListOutwardDrugFirstCopy { get; set; }
        ObservableCollection<OutwardDrugMedDept> ListOutwardDrugFirst { get; set; }
        decimal SumTotalPriceNotVAT { get; set; }
        decimal SumTotalPrice { get; set; }

        bool IsOutClinicDept { get; set; }

        ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDeptsCopy { get; set; }
        long StoreID { get; set; }
    }
}
