using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IRegistrationList
    {
        void OkCmd();
        void DoubleClick(object args);

        bool IsLoading { get; set; }
        Patient CurrentPatient { get; set; }
        ObservableCollection<PatientRegistration> Registrations { get;}
        bool IsInPtRegistration { get; set; }
    }
}
