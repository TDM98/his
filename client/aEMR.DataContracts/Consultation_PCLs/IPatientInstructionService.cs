

using System.ServiceModel;
using aEMR.DataContracts;
using System;
using DataEntities;
using System.Collections.Generic;
namespace ePrescriptionService
{
    [ServiceContract]
    public interface IPatientInstructionService
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginTestMethod1(long supplierID, AsyncCallback callback, object state);
        bool EndTestMethod1(out string strOutMsg, IAsyncResult asyncResult);

        [OperationContract(AsyncPattern = true)]
        [FaultContractAttribute(typeof(AxException))]
        IAsyncResult BeginAddPatientInstruction(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, AsyncCallback callback, object state);
        void EndAddPatientInstruction(out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID, IAsyncResult asyncResult);
    }
}


