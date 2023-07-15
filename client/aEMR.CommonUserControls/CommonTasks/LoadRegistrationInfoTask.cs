using System;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadRegistrationInfoTask:IResult
    {
        public Exception Error { get; private set; }
        private long _registrationID;
        private bool _loadFromAppointment = false;
        private PatientRegistration _registration;
        private bool _IsProcess; //20200222 TBL Mod TMV1: Cờ liệu trình
        public PatientRegistration Registration
        {
            get { return _registration; }
        }

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
        }

        public LoadRegistrationInfoTask(long regID,int  findPatient)
        {
            _registrationID = regID;
            _FindPatient = findPatient;
        }

        public LoadRegistrationInfoTask(long regID)
        {
            _registrationID = regID;
        }

        public LoadRegistrationInfoTask(long regID, bool loadFromAppointment, bool IsProcess = false)
        {
            _registrationID = regID;
            _loadFromAppointment = loadFromAppointment;
            _FindPatient = (int)AllLookupValues.PatientFindBy.NGOAITRU;
            _IsProcess = IsProcess;
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

                        contract.BeginGetRegistrationInfo(_registrationID, _FindPatient, _loadFromAppointment, _IsProcess,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                _registration = contract.EndGetRegistrationInfo(asyncResult);

                                //Apply quyen xoa tren danh sach registration details
                                PermissionManager.ApplyPermissionToRegistration(_registration);
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
