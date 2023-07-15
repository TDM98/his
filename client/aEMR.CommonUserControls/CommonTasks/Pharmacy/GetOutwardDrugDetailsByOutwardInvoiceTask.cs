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
using aEMR.Common.Collections;

namespace aEMR.CommonTasks
{
    public class GetOutwardDrugDetailsByOutwardInvoiceTask:IResult
    {
        public Exception Error { get; private set; }

        private long OutiID { get; set; }
        public ObservableCollection<OutwardDrug> OutwardDrugs { get; set; }

        public GetOutwardDrugDetailsByOutwardInvoiceTask(long _OutiID)
        {
            OutiID = _OutiID;            
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
                        contract.BeginGetOutwardDrugDetailsByOutwardInvoice(OutiID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    OutwardDrugs = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult).ToObservableCollection();                                    
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
                                    if (OutwardDrugs == null
                                        || OutwardDrugs.Count < 1)
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
