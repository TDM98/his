using System;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;


namespace aEMR.CommonTasks
{
    public class CalcMoneyPaidedForDrugInvoiceTask:IResult
    {
        public Exception Error { get; private set; }

        private long _outiID;

        public long outiID
        {
            get
            {
                return _outiID;
            }
        }

        public decimal Amount
        {
            get;
            set;
        }
      
        public CalcMoneyPaidedForDrugInvoiceTask(long ID)
        {
            _outiID = ID;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCountMoneyForVisitorPharmacy(_outiID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    decimal _amount = 0;
                                    contract.EndCountMoneyForVisitorPharmacy(out _amount, asyncResult);
                                    Amount = _amount;
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
                                    this.HideBusyIndicator();
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
