using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IHealthExaminationRecordServiceEdit
    {
        PatientRegistration CurrentRegistration { get; set; }
        void InitCurrentRegistration(PatientRegistration aRegistration);
        ObservableCollection<HosClientContractPatientGroup> PatientGroupCollection { set; }
        long CurrentPatientGroupID { get; }
        ObservableCollection<MedRegItemBase> MedRegItemBaseCollection { get; set; }
        bool IsChoossingCase { set; }
        bool IsConfirmed { get; }
        HospitalClientContract CurrentClientContract { get; set; }
    }
}