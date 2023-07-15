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
    public class AddPCLRequestExtWithDetailsTask:IResult
    {
        public Exception Error { get; private set; }

        private PatientPCLRequest_Ext _ObjPatientPCLRequestNew;

        public PatientPCLRequest_Ext resPatientPCLRequestNew{get;set;}

        public long _PatientPCLReqExtID;
        
        public AddPCLRequestExtWithDetailsTask(PatientPCLRequest_Ext ObjPatientPCLRequestNew)
        {
            _ObjPatientPCLRequestNew = ObjPatientPCLRequestNew;
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
                        long PatientPCLReqExtID;
                        contract.BeginAddPCLRequestExtWithDetails(_ObjPatientPCLRequestNew,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndAddPCLRequestExtWithDetails(out PatientPCLReqExtID, asyncResult);
                                    _PatientPCLReqExtID = PatientPCLReqExtID;
                                    resPatientPCLRequestNew = regInfo;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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

    public class PCLRequestExtUpdateTask : IResult
    {
        public Exception Error { get; private set; }

        private PatientPCLRequest_Ext _ObjPatientPCLRequestNew;

        public long _PatientPCLReqExt;

        public PCLRequestExtUpdateTask(PatientPCLRequest_Ext ObjPatientPCLRequestNew)
        {
            _ObjPatientPCLRequestNew = ObjPatientPCLRequestNew;
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
                        contract.BeginPCLRequestExtUpdate(_ObjPatientPCLRequestNew,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndPCLRequestExtUpdate(asyncResult);
                                    
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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
