namespace aEMR.ViewContracts
{
    public interface IExamInformationPatients
    {
    
        bool IsShowSummaryContent { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
      
    }
}