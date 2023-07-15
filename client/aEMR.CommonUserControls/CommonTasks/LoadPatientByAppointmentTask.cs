using System;
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
    public class LoadPatientByAppointmentTask:IResult
    {
        public Exception Error { get; private set; }
        private PatientRegistration _registration;
        public PatientRegistration Registration
        {
            get { return _registration; }
            set { _registration = value; }
        }

        private PatientAppointment _patientAppointment;


        public LoadPatientByAppointmentTask(PatientAppointment patientAppointment)
        {
            _patientAppointment = patientAppointment;
            
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

                        contract.BeginGetPatientByAppointment(_patientAppointment,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Registration = contract.EndGetPatientByAppointment(asyncResult);
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
