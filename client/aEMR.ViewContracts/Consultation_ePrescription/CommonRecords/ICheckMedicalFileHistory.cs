using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ICheckMedicalFileHistory
    {
        void InitPatientInfo(PatientRegistration CurrentPatientRegistration = null);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        long V_RegistrationType { get; set; }
    }
}
