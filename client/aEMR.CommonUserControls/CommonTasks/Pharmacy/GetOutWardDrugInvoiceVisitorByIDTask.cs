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
    public class GetOutWardDrugInvoiceVisitorByIDTask:IResult
    {
        public Exception Error { get; private set; }

        private long OutwardID { get; set; }
        public OutwardDrugInvoice OutInvoice { get; set; }

        public GetOutWardDrugInvoiceVisitorByIDTask(long _OutwardID)
        {
            OutwardID = _OutwardID;            
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
                        contract.BeginGetOutWardDrugInvoiceVisitorByID(OutwardID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    OutInvoice = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);                                    
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
                                    bool flag = false;
                                    if (OutInvoice==null)
                                    {
                                        flag = true;
                                    }
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = null,
                                        WasCancelled = flag
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
