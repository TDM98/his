using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IDocumentAccordingMOH
    {
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        void InitPatientInfo(PatientRegistration CurrentPatientRegistration);
    }
}
