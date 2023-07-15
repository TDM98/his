using DataEntities;
using System.Collections.Generic;

namespace aEMR.ViewContracts
{
    public interface IOutPatientBillingManage
    {
        void UpdateItemList(IList<InPatientBillingInvoice> aItemCollection);
    }
}