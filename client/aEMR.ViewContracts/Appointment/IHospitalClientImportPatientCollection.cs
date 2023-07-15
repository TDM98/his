using DataEntities;
using System.Collections.ObjectModel;

namespace aEMR.ViewContracts
{
    public interface IHospitalClientImportPatientCollection
    {
        ObservableCollection<HosClientContractPatientGroup> PatientGroupCollection { set; }
        long CurrentPatientGroupID { get; }
        string CurrentFileName { get; }
        bool IsConfirmed { get; }
        long HosClientContractID { set; }
        ObservableCollection<MedRegItemBase> MedRegItemBaseCollection { get; }
        HospitalClientContract CurrentClientContract { get; set; }
        PatientRegistration CurrentRegistration { get; set; }
        HosClientContractPatientGroup AddedNewHosClientContractPatientGroup { get; set; }
    }
}