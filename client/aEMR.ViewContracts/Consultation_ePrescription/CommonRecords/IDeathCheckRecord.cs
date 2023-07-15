using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDeathCheckRecord
    {
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        DeathCheckRecord CurDeathCheckRecord { get; set; }
        void InitPatientInfo(PatientRegistration CurrentPatientRegistration);
    }
}
