using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IEditPrescription
    {
        OutwardDrugInvoice SelectedOutInvoice { get; set; }
        Patient PatientInfo { get; set; }
        Prescription SelectedPrescription { get; set; }
        OutwardDrugInvoice SelectedOutInvoiceCopy { get; set; }
        ObservableCollection<OutwardDrug> OutwardDrugsCopy { get; set; }

        ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListByPrescriptID { get; set; }

        void GetClassicationPatientInvoice();

        decimal TotalInvoicePrice { get; set; }
        decimal TotalHIPayment { get; set; }
        decimal TotalPatientPayment { get; set; }
        void SetDefaultForStore();
    }
}
