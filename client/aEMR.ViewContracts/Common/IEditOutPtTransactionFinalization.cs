using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IEditOutPtTransactionFinalization
    {
        OutPtTransactionFinalization TransactionFinalizationObj { get; set; }
        PatientRegistration Registration { get; set; }
        byte ViewCase { get; set; }
        bool IsSaveCompleted { get; set; }
        byte InvoiceType { get; set; }
    }
}