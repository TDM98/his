using DataEntities;

namespace aEMR.ViewContracts
{
    public interface ITransmissionMonitor
    {
        void LoadRegistrationInfo(PatientRegistration aRegistration, bool LoadInstructionAlso = true);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
      
    }
}