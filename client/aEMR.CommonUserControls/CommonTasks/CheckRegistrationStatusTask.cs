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
    public class CheckRegistrationStatusTask:IResult
    {
        public Exception Error { get; private set; }

        private long _PtRegistrationID;

        public long V_RegistrationStatus
        {
            get;
            set;
        }
        public CheckRegistrationStatusTask(long PtRegistrationID)
        {
            _PtRegistrationID = PtRegistrationID;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCheckRegistrationStatus(_PtRegistrationID, 
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {                                   
                                    V_RegistrationStatus = contract.EndCheckRegistrationStatus(asyncResult);                                    
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
                        WasCancelled = true
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
