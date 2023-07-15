using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPrescriptionAndConfirmHI
    {
        void LoadAllTabDetails(PatientRegistration aPatientRegistration);
        bool IsConfirmPrescriptionOnly { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        bool IsOutPtTreatmentPrescription { get; set; }
    }
}