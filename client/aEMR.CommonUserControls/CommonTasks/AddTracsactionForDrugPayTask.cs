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
    public class AddTracsactionForDrugPayTask:IResult
    {
        public Exception Error { get; private set; }

        private long _PaymentID;
        public long PaymentID
        {
            get { return _PaymentID; }
        }

        private readonly PatientTransactionPayment _payment;
        public PatientTransactionPayment payment
        {
            get
            {
                return _payment;
            }
        }

        private readonly long _StaffID;
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
        }

        private readonly long? _collectorDeptLocID;
        public long? CollectorDeptLocID
        {
            get { return _collectorDeptLocID; }
        } 


        private readonly OutwardDrugInvoice _CurOutwardDrugInvoice;
        public OutwardDrugInvoice CurOutwardDrugInvoice
        {
            get
            {
                return _CurOutwardDrugInvoice;
            }
        }
        public AddTracsactionForDrugPayTask(PatientTransactionPayment Payment, OutwardDrugInvoice CurOutwardDrugInvoice, long StaffID, long? CollectorDeptLocID)
        {
            _payment = Payment;
            _CurOutwardDrugInvoice = CurOutwardDrugInvoice;
            _StaffID = StaffID;
            _collectorDeptLocID = CollectorDeptLocID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddTransactionVisitor(_payment, _CurOutwardDrugInvoice, _StaffID, _collectorDeptLocID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool value = contract.EndAddTransactionVisitor(out _PaymentID, asyncResult);
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

    public class AddTracsactionMedDeptForDrugPayTask : IResult
    {
        public Exception Error { get; private set; }

        private long _PaymentID;
        public long PaymentID
        {
            get { return _PaymentID; }
        }

        private readonly PatientTransactionPayment _payment;
        public PatientTransactionPayment payment
        {
            get
            {
                return _payment;
            }
        }

        private readonly long _StaffID;
        public long StaffID
        {
            get
            {
                return _StaffID;
            }
        }

        private readonly OutwardDrugMedDeptInvoice _CurOutwardDrugInvoice;
        public OutwardDrugMedDeptInvoice CurOutwardDrugInvoice
        {
            get
            {
                return _CurOutwardDrugInvoice;
            }
        }
        public AddTracsactionMedDeptForDrugPayTask(PatientTransactionPayment Payment, OutwardDrugMedDeptInvoice CurOutwardDrugInvoice, long StaffID)
        {
            _payment = Payment;
            _CurOutwardDrugInvoice = CurOutwardDrugInvoice;
            _StaffID = StaffID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddTransactionMedDept(_payment, _CurOutwardDrugInvoice, _StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool value = contract.EndAddTransactionMedDept(out _PaymentID, asyncResult);
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
