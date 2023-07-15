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
    public class LoadPatientInfoByRegistrationTask : IResult
    {
        private Exception _error;
        private readonly long? _patientID;
        private readonly long? _registrationID;
        private readonly int _FindPatient;
        
        public LoadPatientInfoByRegistrationTask(long? RegistrationID,long? patientID,int findPatient)
        {
            this._patientID = patientID;
            this._registrationID = RegistrationID;
            this._FindPatient= findPatient;
        }

        public Patient CurrentPatient { get; set; }
        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientInfoByPtRegistration(_registrationID, _patientID,_FindPatient,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    CurrentPatient = contract.EndGetPatientInfoByPtRegistration(asyncResult);
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


                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    _error = ex;
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
