namespace aEMR.ViewContracts
{
    public interface IPatientMedicalRecords_ByPatientID
    {
        void Init();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}