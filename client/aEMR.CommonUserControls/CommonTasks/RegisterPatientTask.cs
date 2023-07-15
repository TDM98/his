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


    public class RegisterPatientTask : IResult
    {
        private Exception _error;
        public Exception Error
        {
            get { return _error; }
        }
        private PatientRegistration _registration;
        public PatientRegistration RegistrationInfo { get { return _registration; } }
        public RegisterPatientTask(PatientRegistration registration)
        {
            _error = null;
            this._registration = registration;
        }
        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    /*
                    using (var serviceFactory = new CommonServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                    }
                    */
                }
                catch (Exception ex)
                {
                    _error = ex;
                }
                finally
                {
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = _error,
                        WasCancelled = false
                    });
                }
            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
