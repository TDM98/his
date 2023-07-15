using System.Collections.Generic;
using DataEntities;

namespace aEMR.ViewContracts
{
    public interface IInPatientPayBill
    {

        void SetValues(PatientRegistration regInfo, IList<InPatientBillingInvoice> billingInvoiceList);

        void StartCalculating();
    }
}
