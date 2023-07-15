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
    public class LoadRegistrationInfo_InPtTask:IResult
    {
        public Exception Error { get; private set; }
        private long _registrationID;
        private bool _loadFromAppointment = false;
        private PatientRegistration _registration;
        public PatientRegistration Registration
        {
            get { return _registration; }
        }

        //KMx: Khi load Registration thì dùng Switch này để lấy những thông tin cần thiết thôi, không lấy hết như trước đây (17/09/2014 15:32).
        public LoadRegistrationSwitch _loadRegisSwitch;

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
        }
        
        public LoadRegistrationInfo_InPtTask(long regID, int findPatient, LoadRegistrationSwitch LoadRegisSwitch)
        {
            _registrationID = regID;
            _FindPatient = findPatient;
            _loadRegisSwitch = LoadRegisSwitch;
        }

        public LoadRegistrationInfo_InPtTask(long regID)
        {
            _registrationID = regID;
        }
        public LoadRegistrationInfo_InPtTask(long regID, bool loadFromAppointment)
        {
            _registrationID = regID;
            _loadFromAppointment = loadFromAppointment;
            _FindPatient = (int)AllLookupValues.PatientFindBy.NOITRU;
        }

        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator(eHCMSLanguage.eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRegistrationInfo_InPt(_registrationID, _FindPatient, _loadRegisSwitch, _loadFromAppointment,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                _registration = contract.EndGetRegistrationInfo_InPt(asyncResult);

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
