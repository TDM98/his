using eHCMSLanguage;
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


    public class CancelRegistrationTask : IResult
    {
        private Exception _error;
        public Exception Error
        {
            get { return _error; }
        }
        private PatientRegistration _registration;
        public PatientRegistration RegistrationInfo { get { return _registration; } }
        public CancelRegistrationTask(PatientRegistration registration)
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
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        bool bCancel = false;
                        contract.BeginCancelRegistration(_registration,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                PatientRegistration registration = null;
                                try
                                {
                                    bool bOK = contract.EndCancelRegistration(out registration, asyncResult);
                                    this._registration = registration;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    _error = fault;
                                    bCancel = true;
                                }
                                catch (Exception ex)
                                {
                                    _error = ex;
                                    bCancel = true;
                                    MessageBox.Show(ex.Message.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    Completed(this, new ResultCompletionEventArgs
                                    {
                                        Error = _error,
                                        WasCancelled = bCancel
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
                        WasCancelled = true
                    });
                }

            });
            t.Start();
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}
