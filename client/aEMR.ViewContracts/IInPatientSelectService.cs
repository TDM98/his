using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientSelectService
    {
        RefMedicalServiceType MedServiceType { get; set; }
        ObservableCollection<RefMedicalServiceType> ServiceTypes { get; set; }
        RefMedicalServiceItem MedServiceItem { get; set; }
        ObservableCollection<RefMedicalServiceItem> MedicalServiceItems { get; set; }
        long? DeptID { get; set; }
        InPatientAdmDisDetails CurrentInPatientAdmDisDetail { get; set; }
        void GetServiceTypes();
        void GetAllMedicalServiceItemsByType(RefMedicalServiceType serviceType);
    }
}