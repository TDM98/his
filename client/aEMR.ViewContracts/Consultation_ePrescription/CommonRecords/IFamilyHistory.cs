namespace aEMR.ViewContracts
{
    public interface IFamilyHistory
    {
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}