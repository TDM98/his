namespace aEMR.ViewContracts
{
    public interface ISimplePayPharmacy
    {
        decimal TotalPayForSelectedItem { get; set; }
        decimal TotalPaySuggested { get; set; }
        void StartCalculating();
        long V_TradingPlaces { get; set; }
    }
}
