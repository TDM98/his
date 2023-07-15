namespace aEMR.ViewContracts
{
    public interface IPtDashboardSummary
    {
        long V_RegistrationType { get; set; }
        IRegistration_DataStorage Registration_DataStorage { get; set; }
    }
}