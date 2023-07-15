namespace aEMR.ViewContracts
{
    public interface IConfirmDecimal
    {
        int ConfirmValue { get; set; }
        bool IsConfirmed { get; set; }
    }
}