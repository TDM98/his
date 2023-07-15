using DataEntities;
using System;
namespace aEMR.ViewContracts
{
    public interface IInPatientChoosePriceList
    {
        InPatientBillingInvoice RecalBillingInvoice { get; set; }
    }
}
