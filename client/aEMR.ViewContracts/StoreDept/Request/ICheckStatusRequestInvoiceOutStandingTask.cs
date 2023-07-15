using DataEntities;
using aEMR.Common.Collections;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ICheckStatusRequestInvoiceOutStandingTask
    {
        ObservableCollection<RequestDrugInwardClinicDept> RequestDruglist { get; set; }
        long V_MedProductType { get; set; }
        void LoadStore();
    }
}