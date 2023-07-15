using System;
using System.Threading;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using Caliburn.Micro;
using DataEntities;

namespace aEMR.CommonTasks
{
    public class LoadPatientTask : IResult
    {
        private Exception _error;
        private readonly long _patientID;
        public LoadPatientTask(long patientID, bool bGetInPtRegInfo = false, bool bGetOutPtRegInfo = false, bool bToRegisOutPt = false)
        {
            this._patientID = patientID;
            m_bGetInPtRegInfo = bGetInPtRegInfo;
            m_bGetOutPtRegInfo = bGetOutPtRegInfo;
            ToRegisOutPt = bToRegisOutPt;
        }

        private bool m_bGetOutPtRegInfo = false;
        private bool m_bGetInPtRegInfo = false;
        private bool ToRegisOutPt = false;

        public Patient CurrentPatient { get; set; }
        public void Execute(ActionExecutionContext context)
        {
            this.ShowBusyIndicator();
            if (!m_bGetInPtRegInfo)
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PatientRegistrationServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;

                            contract.BeginGetPatientByID(_patientID, ToRegisOutPt,
                                Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var patient = contract.EndGetPatientByID(asyncResult);
                                        CurrentPatient = patient;
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
                                        this.HideBusyIndicator();
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
                        this.HideBusyIndicator();
                    }
                });
                t.Start();

            }
            else
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PatientRegistrationServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;

                            contract.BeginGetPatientByID_InPt(_patientID, m_bGetOutPtRegInfo, ToRegisOutPt,
                                Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var patient = contract.EndGetPatientByID_InPt(asyncResult);
                                        CurrentPatient = patient;
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

                                        this.HideBusyIndicator();
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
                        this.HideBusyIndicator();
                    }
                });
                t.Start();
            }

        }

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }

 
}
