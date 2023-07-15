using DataEntities;
using System.Collections.ObjectModel;
namespace aEMR.ViewContracts
{
    public interface IEditOutwardFromPrescription
    {
        OutwardDrugMedDeptInvoice SelectedOutInvoice { get; set; }
        Patient PatientInfo { get; set; }
        PatientRegistration CurRegistration { get; set; }
        Prescription SelectedPrescription { get; set; }
        OutwardDrugMedDeptInvoice SelectedOutInvoiceCopy { get; set; }
        ObservableCollection<OutwardDrugMedDept> OutwardDrugsCopy { get; set; }

        ObservableCollection<GetGenMedProductForSell> GetGenMedProductForSellListByPrescriptID { get; set; }

        void GetClassicationPatientInvoice();

        decimal TotalInvoicePrice { get; set; }
        decimal TotalHIPayment { get; set; }
        decimal TotalPatientPayment { get; set; }
    }
}
