using System;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadRegistrationSimpleTask:IResult
    {
        public Exception Error { get; private set; }

        private long _registrationID;

        private PatientRegistration _registration;
        public PatientRegistration Registration
        {
            get { return _registration; }
        }

        private int _PatientFindBy;

        public LoadRegistrationSimpleTask(long regID, int PatientFindBy)
        {
            _registrationID = regID;
            _PatientFindBy = PatientFindBy;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRegistrationSimple(_registrationID, _PatientFindBy,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                _registration = contract.EndGetRegistrationSimple(asyncResult);
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
