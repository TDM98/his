namespace aEMR.ViewContracts
{
    public interface IMedicalConditions
    {
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}