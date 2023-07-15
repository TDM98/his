namespace aEMR.ViewContracts
{
    public interface IHosHistory
    {
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}