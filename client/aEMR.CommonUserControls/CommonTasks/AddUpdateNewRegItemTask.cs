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
/*
 * 20191025 #001 TBL:   BM 0018467: Thêm IsNotCheckInvalid để khỏi kiểm tra khoảng thời gian giữa 2 lần làm CLS được tính BHYT
 * 20230109 #002 QTD:   Thêm cờ đánh dấu lưu dịch vụ từ màn hình chỉ định dịch vụ của bác sĩ
 */

namespace aEMR.CommonTasks
{
    public class AddUpdateNewRegItemTask:IResult
    {
        public Exception Error { get; private set; }

        public string ErrorMesage { get; set; }

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get { return _curRegistration; }
        }
        private int? _Apply15HIPercent;
        private List<PatientRegistrationDetail> _newRegDetailList;
        private List<PatientPCLRequest> _newPclRequestList;

        private List<PatientRegistrationDetail> _deletedRegDetailList;
        private List<PatientPCLRequest> _deletedPclRequestList;
        private bool _IsNotCheckInvalid;
        private bool _IsProcess; //20200222 TBL Mod TMV1: Cờ liệu trình
        private bool _IsFromRequestDoctor;
        private long? _V_ReceiveMethod;

        public AddUpdateNewRegItemTask(PatientRegistration regInfo, List<PatientRegistrationDetail> newRegDetailList, List<PatientPCLRequest> newPclRequestList
            , List<PatientRegistrationDetail> deletedRegDetailList, List<PatientPCLRequest> deletedPclRequestList, int? Apply15HIPercent
            , bool IsNotCheckInvalid = false, bool IsProcess = false, bool IsFromRequestDoctor = false)
        {
            _curRegistration = regInfo;
            _newRegDetailList = newRegDetailList;
            _newPclRequestList = newPclRequestList;
            _deletedRegDetailList = deletedRegDetailList;
            _deletedPclRequestList = deletedPclRequestList;
            _Apply15HIPercent = Apply15HIPercent;
            //=====#001=====
            _IsNotCheckInvalid = IsNotCheckInvalid;
            //=====#001=====
            _IsProcess = IsProcess;
            //▼====#002
            _IsFromRequestDoctor = IsFromRequestDoctor;
            //▲====#002
            _V_ReceiveMethod = regInfo.V_ReceiveMethod;
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

                        contract.BeginAddServicesAndPCLRequests(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),Globals.DeptLocation.DeptLocationID, _Apply15HIPercent, 
                            _curRegistration, _newRegDetailList, _newPclRequestList, _deletedRegDetailList, _deletedPclRequestList, _IsNotCheckInvalid, false, default(DateTime),
                            _IsProcess, _IsFromRequestDoctor, _V_ReceiveMethod, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    IList<PatientRegistrationDetail> regDetails;
                                    IList<PatientPCLRequest> pclRequests;
                                    long regId;
                                    V_RegistrationError error = V_RegistrationError.mNone;
                                    _curRegistration = contract.EndAddServicesAndPCLRequests(out regId, out regDetails, out pclRequests, out error, asyncResult);

                                    if (error==V_RegistrationError.mRefresh)
                                    {
                                        ErrorMesage = eHCMSResources.Z1584_G1_DKDaThayDoiDKDcLoadLai2;
                                    }
                                    if (_curRegistration.PtRegistrationID <= 0)
                                    {
                                        _curRegistration.PtRegistrationID = regId;
                                    }
                                    else
                                    {
                                        //Apply quyen xoa tren danh sach registration details
                                        PermissionManager.ApplyPermissionToRegistration(_curRegistration);
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
