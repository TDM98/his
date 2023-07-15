namespace aEMR.ViewContracts
{
    public interface IImmunizations
    {
        void InitPatientInfo();
        void GetRefImmunization(long MedServiceID);
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}