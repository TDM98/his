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
    public class AddTracsactionForDrugTask:IResult
    {
        public Exception Error { get; private set; }

        private long _PaymentID;
        public long PaymentID
        {
            get { return _PaymentID; }
        }

        private readonly long _OutiID;
        public long OutiID
        {
            get
            {
                return _OutiID;
            }
        }

        private readonly long _V_TranRefType;
        public long V_TranRefType
        {
            get
            {
                return _V_TranRefType;
            }
        }

        private readonly PatientTransactionPayment _payment;
        public PatientTransactionPayment payment
        {
            get
            {
                return _payment;
            }
        }
        public AddTracsactionForDrugTask(PatientTransactionPayment Payment, long outid, long v_TranRefType)
        {
            _payment = Payment;
            _OutiID = outid;
            _V_TranRefType = v_TranRefType;
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

                        contract.BeginAddTransactionForDrug(_payment,_OutiID,_V_TranRefType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndAddTransactionForDrug(out _PaymentID, asyncResult);
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
