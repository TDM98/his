
namespace aEMR.ViewContracts
{
    public interface ISimplePayReturnPharmacy
    {
        decimal HIReturn { get; set; }
        decimal PatientReturn { get; set; }
        decimal CongNo { get; set; }
        int FormMode { get; set; } //1: tu form tra hang goi qua,2:form ban hang goi tra tien cho huy
    }
}
