using DataEntities;

namespace Ax.ViewContracts
{
    public interface IQuotation
    {
        short CurrentViewCase { get; set; }
    }
    public interface IQuotationCollection
    {
        short CurrentViewCase { get; set; }
        bool IsHasSelected { get; set; }
        InPatientBillingInvoice SelectedQuotation { get; set; }
    }
}