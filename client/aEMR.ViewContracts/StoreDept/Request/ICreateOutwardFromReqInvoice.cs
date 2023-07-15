using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface ICreateOutwardFromReqInvoice
    {
        ObservableCollection<OutwardDrugClinicDept> OutwardDrugCollection { get; set; }
        long V_MedProductType { get; set; }
        long StoreID { get; set; }
    }
}