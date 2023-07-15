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
using eHCMSLanguage;

namespace aEMR.CommonTasks
{
    /// <summary>
    /// Task này dùng để xóa những dịch vụ đăng ký đã tính tiền rồi.
    /// </summary>
    public class RemoveOldRegItemTask:IResult
    {
        public Exception Error { get; private set; }

        public PatientRegistration Registration
        {
            get
            {
                return  _curRegistration;
            }
        }
        public string ErrorMesage { get; set; }
        private PatientRegistration _curRegistration;
        private List<PatientRegistrationDetail> _oldRegDetailList;
        private List<PatientPCLRequest> _oldPclRequestList;
        private int? _Apply15HIPercent;

        public RemoveOldRegItemTask(PatientRegistration regInfo, List<PatientRegistrationDetail> oldRegDetailList, List<PatientPCLRequest> oldPclRequestList,int? Apply15HIPercent)
        {
            _curRegistration = regInfo;
            _oldRegDetailList = oldRegDetailList;
            _oldPclRequestList = oldPclRequestList;
            _Apply15HIPercent = Apply15HIPercent;
        }

        public void Execute(ActionExecutionContext context)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginRemovePaidRegItems(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DeptLocation.DeptLocationID, _Apply15HIPercent, _curRegistration.PtRegistrationID
                            ,_curRegistration.FindPatient, _oldRegDetailList, _oldPclRequestList, null, null, null,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    V_RegistrationError error = V_RegistrationError.mRefresh;
                                    contract.EndRemovePaidRegItems(out error,asyncResult);
                                    if (error == V_RegistrationError.mRefresh)
                                    {
                                        ErrorMesage = eHCMSResources.Z1584_G1_DKDaThayDoiDKDcLoadLai2;
                                    }
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
