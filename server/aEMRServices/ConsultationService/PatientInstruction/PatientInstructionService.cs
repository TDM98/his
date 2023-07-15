using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel;
using System.Runtime.Serialization;
using ErrorLibrary;
using DataEntities;
using AxLogging;

namespace ConsultationsService.PatientInstruction
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    [KnownType(typeof(AxException))]
    public class PatientInstructionService : eHCMS.WCFServiceCustomHeader, IPatientInstruction
    {
        object TheLock = new object();
        int nInstID = 0;
        public bool TestMethod1(long nInItemID, out string strOutMsg)
        {
            int curThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            DateTime curTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("[{0}] ====> BEGIN PharmacyService.SaleAndOutward TestMethod1 Instance Number = {1} ON THREAD = {2}.", curTime.ToString("dd/MM/yyyy h:mm:ss.ff"), nInstID, curThreadID);
            strOutMsg = "====> PharmacyService.SaleAndOutward SERVICE TestMethod1 Instance Number = {" + nInstID.ToString() + "} ON SERVICE THREAD = {" + curThreadID.ToString() + "}";
            System.Threading.Thread.Sleep(200);
            curTime = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("[{0}] ====> END PharmacyService.SaleAndOutward TestMethod1 Instance Number = {1} ON THREAD = {2}.", curTime.ToString("dd/MM/yyyy h:mm:ss.ff"), nInstID, curThreadID);
            return true;
        }

        public void AddPatientInstruction(int? Apply15HIPercent, PatientRegistration registrationInfo, InPatientBillingInvoice billingInv, bool CalcPaymentToEndOfDay
            , out long PatientRegistrationID, out Dictionary<long, List<long>> DrugIDList_Error, out long NewBillingInvoiceID)
        {
            PatientRegistrationID = 0;
            DrugIDList_Error = null;
            NewBillingInvoiceID = 0;
            //try
            //{
            //    AxLogger.Instance.LogInfo("Start saving inpatient Registration.", CurrentUser);

            //    PatientRegistrationID = -1;
            //    var paymentProcessor = RegAndPaymentProcessorFactory.GetPaymentProcessor(registrationInfo);

            //    paymentProcessor.InitNewTxd(registrationInfo, false);

            //    paymentProcessor.AddInPatientBillingInvoice(Apply15HIPercent, billingInv, CalcPaymentToEndOfDay, out DrugIDList_Error, out NewBillingInvoiceID);
            //    PatientRegistrationID = registrationInfo.PtRegistrationID;

            //    AxLogger.Instance.LogInfo("End of saving inpatient Registration.", CurrentUser);
            //}
            //catch (Exception ex)
            //{
            //    AxLogger.Instance.LogInfo("End of saving inpatient Registration. Status: Failed.", CurrentUser);

            //    AxException axErr = AxException.CatchExceptionAndLogMessage(ex, ErrorNames.COMMON_SERVICE_CreateBillingInvoiceFromExistingItems, CurrentUser);

            //    throw new FaultException<AxException>(axErr, new FaultReason(ex.Message));
            //}
        }
    }
}
