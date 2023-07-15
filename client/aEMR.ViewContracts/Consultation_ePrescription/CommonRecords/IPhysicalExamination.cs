namespace aEMR.ViewContracts
{
    public interface IPhysicalExamination
    {
        void InitPatientInfo();
        bool IsShowSummaryContent { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
        long V_RegistrationType { get; set; }
    }
}