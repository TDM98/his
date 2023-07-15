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
    public class PayForInPatientRegistrationTask:IResult
    {
        public Exception Error { get; private set; }

        private PatientTransaction _patientTransaction;

        /// <summary>
        /// Transaction kết quả sau khi thực hiện tính tiền.
        /// </summary>
        public  PatientTransaction PatientTransaction
        {
            get { return _patientTransaction; }
        }

        private PatientTransactionPayment _retPayment;
        /// <summary>
        /// Thông tin tính tiền do server trả về sau khi thực hiện xong.
        /// </summary>
        public PatientTransactionPayment PatientPayment
        {
            get { return _retPayment; }
        }

        private long _retCashAdvanceID;
        /// <summary>
        /// ID lần tạm ứng này.
        /// </summary>
        public long CashAdvanceID
        {
            get { return _retCashAdvanceID; }
        }

        public long StaffID
        {
            get { return _staffID; }
        }
        private readonly PatientRegistration _curRegistration;

        public PatientRegistration Registration
        {
            get
            {
                return _curRegistration;
            }
        }
        private PatientTransactionPayment _paymentDetails;
        private IList<InPatientBillingInvoice> _billingInvoiceList;
        private long _staffID;

        public PayForInPatientRegistrationTask(PatientRegistration regInfo, PatientTransactionPayment paymentDetails,
                                       IList<InPatientBillingInvoice> billingInvoiceList,long staffID)
        {
            _curRegistration = regInfo;

            _paymentDetails = paymentDetails;
            _billingInvoiceList = billingInvoiceList;
            _staffID = staffID;
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

                        contract.BeginPayForInPatientRegistration(StaffID,_curRegistration.PtRegistrationID, _paymentDetails, _billingInvoiceList,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndPayForInPatientRegistration(out _patientTransaction, out _retPayment, out _retCashAdvanceID, asyncResult);
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
