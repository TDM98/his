namespace aEMR.ViewContracts
{
    public interface IChildListing
    {
    
        bool IsShowSummaryContent { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
      
    }
}