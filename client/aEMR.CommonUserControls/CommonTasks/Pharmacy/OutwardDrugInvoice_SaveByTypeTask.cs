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
using aEMR.Common;

namespace aEMR.CommonTasks
{
    public class OutwardDrugInvoice_SaveByTypeTask:IResult
    {
        public Exception Error { get; private set; }

        private OutwardDrugInvoice OutwardInvoice { get; set; }

        public long OutID { get; set; }
        public string StrError { get; set; }
        
        public OutwardDrugInvoice_SaveByTypeTask(OutwardDrugInvoice _OutwardInvoice)
        {
            OutwardInvoice = OutwardInvoice;
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
                        contract.BeginOutwardDrugInvoice_SaveByType(OutwardInvoice,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long _OutID=0;
                                    string _StrError="";
                                    var res = contract.EndOutwardDrugInvoice_SaveByType(out _OutID, out _StrError, asyncResult);                                    
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = true
                                    });
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
                        WasCancelled = true
                    });
                }
            });

            t.Start();
        }


        public event EventHandler<ResultCompletionEventArgs> Completed;
        
    }
}
