using System;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadInPatientRegItemsIntoBillTask : IResult
    {
        private Exception _error;
        private readonly long _PtRegistrationID;
        public LoadInPatientRegItemsIntoBillTask(long _PtRegistrationID)
        {
            this._PtRegistrationID = _PtRegistrationID;
        }

        public InPatientBillingInvoice CurInPatientBillingInvoice { get; set; }
        public void Execute(ActionExecutionContext context)
        {
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new CommonServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;

            //            contract.BeginLoadInPatientRegItemsIntoBill(_PtRegistrationID,null,
            //                Globals.DispatchCallback((asyncResult) =>
            //                {
            //                    try
            //                    {
            //                        CurInPatientBillingInvoice = contract.EndLoadInPatientRegItemsIntoBill(asyncResult);
            //                    }
            //                    catch(Exception ex)
            //                    {
            //                        _error = ex;
            //                    }
            //                    finally
            //                    {
            //                        Completed(this, new ResultCompletionEventArgs
            //                        {
            //                            Error = _error,
            //                            WasCancelled = false
            //                        });
            //                    }
                               

            //                }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _error = ex;
            //        Completed(this, new ResultCompletionEventArgs
            //        {
            //            Error = _error,
            //            WasCancelled = false
            //        });
            //    }
            //});
            //t.Start();
        }

        //public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
