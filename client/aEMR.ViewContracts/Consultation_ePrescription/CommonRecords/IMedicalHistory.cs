namespace aEMR.ViewContracts
{
    public interface IMedicalHistory
    {
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}