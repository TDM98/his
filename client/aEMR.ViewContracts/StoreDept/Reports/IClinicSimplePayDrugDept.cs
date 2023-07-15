namespace aEMR.ViewContracts
{
    public interface IClinicSimplePayDrugDept
    {
        decimal TotalMoney { get; set; }
        decimal TotalPayed { get; set; }
        int FormMode { get; set; }
    }
}
