using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ErrorLibrary;
using DataEntities;

namespace ConsultationsService.PatientInstruction
{
    [ServiceContract]
    public interface IPatientInstruction
    {
        [OperationContract]
        [FaultContract(typeof(AxException))]
        bool TestMethod1(long nInItemID, out string strOutMsg);

        [OperationContract]
        [FaultContract(typeof(AxException))]
        void AddPatientInstruction(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay, out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID);
    }
}
