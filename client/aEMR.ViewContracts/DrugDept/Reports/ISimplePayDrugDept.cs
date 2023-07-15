namespace aEMR.ViewContracts
{
    public interface ISimplePayDrugDept
    {
        decimal TotalMoney { get; set; }
        decimal TotalPayed { get; set; }
        int FormMode { get; set; }
    }
}
