namespace aEMR.ViewContracts
{
    public interface INutritionalRating
    {
    
        bool IsShowSummaryContent { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
      
    }
}