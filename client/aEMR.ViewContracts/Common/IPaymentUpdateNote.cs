using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IPaymentUpdateNote
    {
        PatientTransactionPayment CurPaymentInfo { get; set; }
    }
}
