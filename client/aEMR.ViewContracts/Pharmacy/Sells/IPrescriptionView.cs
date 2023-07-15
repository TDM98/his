namespace aEMR.ViewContracts
{
    public interface IPrescriptionView
    {
        string GetValuePreID();
        string GetValuePreHICode();
        string GetValueHICode();
        string GetValueInvoiceID();
    }
}
