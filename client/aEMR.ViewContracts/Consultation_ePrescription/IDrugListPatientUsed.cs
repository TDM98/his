namespace aEMR.ViewContracts
{
    public interface IDrugListPatientUsed
    {
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}
