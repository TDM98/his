namespace aEMR.ViewContracts
{
    public interface IePrescriptionList
    {
        bool IsInPatient { get; set; }
        void InitPatientInfo();
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}