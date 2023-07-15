using System.Collections.ObjectModel;
using DataEntities;

namespace aEMR.ViewContracts
{

    public interface ITotalBillInvoice
    {
        decimal TotalPatientPayment { get; set; }
        decimal TotalHIPayment { get; set; }
        decimal TotalBillInvoice { get; set; }
        decimal TotalRealHIPayment { get; set; }
        decimal TotalCharitySupportFund { get; set; }

        bool ShowErrorMessage { get; set; }
    }
}
