using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class GetInPatientRegistrationAndPaymentInfoTask : IResult
    {
        public Exception Error { get; private set; }

        public decimal totalLiabilities { get; set; }
        public decimal totalPatientPaymentPaidInvoice { get; set; }
        public decimal TotalRefundPatient { get; set; }
        public decimal sumOfAdvance { get; set; }

        public PatientRegistration CurRegistration { get; set; }

        private long _RegistrationID;
        public GetInPatientRegistrationAndPaymentInfoTask(long RegistrationID)
        {
            _RegistrationID = RegistrationID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientRegistrationAndPaymentInfo(_RegistrationID, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                decimal totalLiabilities = 0;
                                decimal totalPatientPaymentPaidInvoice = 0;
                                decimal TotalRefundPatient = 0;
                                decimal sumOfAdvance = 0;
                                decimal sumCashAdvBalanceAmount = 0;
                                decimal TotalCharityOrgPayment = 0;
                                decimal totalPtPayment_NotFinalized = 0;
                                decimal totalPtPaid_NotFinalized = 0;
                                decimal totalSupportFund_NotFinalized = 0;
                                //CurRegistration = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice, out TotalRefundPatient, asyncResult);
                                bool result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice, out TotalRefundPatient, out sumCashAdvBalanceAmount,
                                                                                                out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized, out totalSupportFund_NotFinalized, asyncResult);

                                if (result)
                                {
                                    this.totalLiabilities = totalLiabilities;
                                    this.totalPatientPaymentPaidInvoice = totalPatientPaymentPaidInvoice;
                                    this.TotalRefundPatient = TotalRefundPatient;
                                    this.sumOfAdvance = sumOfAdvance;
                                }
                            }
                            catch (Exception ex)
                            {
                                Error = ex;
                            }
                            finally
                            {
                                Completed(this, new ResultCompletionEventArgs
                                {
                                    Error = null,
                                    WasCancelled = false
                                });
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Error = ex;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = false
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

    }

}
