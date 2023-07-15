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
    public class Registrations_UpdateStatusTask:IResult
    {
        public Exception Error { get; private set; }

        private PatientRegistration _ObjPatientRegistration;
        public PatientRegistration ObjPatientRegistration
        {
            get
            {
                return _ObjPatientRegistration;
            }
        }
        
        private long _V_RegistrationStatus;
        public long V_RegistrationStatus
        {
            get
            {
                return _V_RegistrationStatus;
            }
        }

        public bool Result
        {
            get;
            set;
        }

        public Registrations_UpdateStatusTask(PatientRegistration pObjPatientRegistration, long pV_RegistrationStatus)
        {
            _ObjPatientRegistration = pObjPatientRegistration;
            _V_RegistrationStatus = pV_RegistrationStatus;
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

                        Result = false;

                        contract.BeginRegistrations_UpdateStatus(_ObjPatientRegistration , _V_RegistrationStatus,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool KQ = contract.EndRegistrations_UpdateStatus(asyncResult);
                                    Result = KQ;
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
