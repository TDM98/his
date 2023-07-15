using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using System.Windows.Controls;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientTransferDeptReq)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientTransferDeptReqViewModel : Conductor<object>, IInPatientTransferDeptReq
        , IHandle<AcceptChangeDeptViewModelEvent>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientTransferDeptReqViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            authorization();

            InPatientTransferDeptReqsearch = new InPatientTransferDeptReq { ReqDate = Globals.GetCurServerDateTime()};
            Coroutine.BeginExecute(LoadDepartments());  
        }

        private PatientRegistration _currentRegistration;
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _currentRegistration;
            }
            set
            {
                _currentRegistration = value;
                NotifyOfPropertyChange(() => CurrentRegistration);
            }
        }
        private string _DeptLocTitle;
        public string DeptLocTitle
        {
            get
            {
                return _DeptLocTitle;
            }
            set
            {
                _DeptLocTitle = value;
                NotifyOfPropertyChange(() => DeptLocTitle);
            }
        }



        private InPatientTransferDeptReq _InPatientTransferDeptReqsearch;
        public InPatientTransferDeptReq InPatientTransferDeptReqsearch
        {
            get { return _InPatientTransferDeptReqsearch; }
            set
            {
                _InPatientTransferDeptReqsearch = value;
                NotifyOfPropertyChange(() => InPatientTransferDeptReqsearch);
            }
        }

        private ObservableCollection<InPatientTransferDeptReq> _allInPatientTransferDeptReq;
        public ObservableCollection<InPatientTransferDeptReq> allInPatientTransferDeptReq
        {
            get { return _allInPatientTransferDeptReq; }
            set
            {
                _allInPatientTransferDeptReq = value;
                NotifyOfPropertyChange(() => allInPatientTransferDeptReq);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private bool _isChangeDept = true;
        public bool isChangeDept
        {
            get { return _isChangeDept; }
            set
            {
                _isChangeDept = value;
                NotifyOfPropertyChange(() => isChangeDept);
            }
        }

        public bool IsProcessing
        {
            get
            {
                return _patientLoading;
            }
        }

        private bool _patientLoading = false;



        private ObservableCollection<RefDepartment> _departments;

        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
            set
            {
                _departments = value;
                NotifyOfPropertyChange(() => Departments);
            }
        }
        private ObservableCollection<RefDepartment> _toDepartments;
        public ObservableCollection<RefDepartment> ToDepartments
        {
            get { return _toDepartments; }
            set
            {
                _toDepartments = value;
                NotifyOfPropertyChange(() => ToDepartments);
            }
        }


        //private ObservableCollection<RefDepartment> _endDepartments;

        //public ObservableCollection<RefDepartment> EndDepartments
        //{
        //    get { return _endDepartments; }
        //    set
        //    {
        //        _endDepartments = value;
        //        NotifyOfPropertyChange(() => EndDepartments);
        //    }
        //}


        private AllLookupValues.PatientFindBy _PatientFindBy;
        public AllLookupValues.PatientFindBy PatientFindBy
        {
            get
            {
                return _PatientFindBy;
            }
            set
            {
                _PatientFindBy = value;
                NotifyOfPropertyChange(() => PatientFindBy);
            }
        }


        private IEnumerator<IResult> LoadDepartments()
        {
            var DepartmentTask = new LoadDepartmentsTask(false, true);
            yield return DepartmentTask;
            Departments = DepartmentTask.Departments;

            InPatientTransferDeptReqsearch.ReqDeptID = Globals.ObjRefDepartment != null ? Globals.ObjRefDepartment.DeptID : 0;
            if (ToDepartments == null)
            {
                ToDepartments = new ObservableCollection<RefDepartment>();
            }
            if (!Globals.isAccountCheck)
            {
                ToDepartments = Departments;
            }
            else
            {
                if (Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.DeptIDResponsibilityList != null && Globals.LoggedUserAccount.DeptIDResponsibilityList.Count() > 0)
                {
                    foreach (var Dept in Departments)
                    {
                        if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Any(x => x == Dept.DeptID))
                        {
                            ToDepartments.Add(Dept);
                        }
                    }
                }
            }
            //var EndDepartmentTask = new LoadDepartmentsTask(LstRefDepartment, true);
            //yield return EndDepartmentTask;
            //EndDepartments = EndDepartmentTask.Departments;

            yield break;

        }
        public void SearchCmd()
        {
            GetInPatientTransferDeptReq(InPatientTransferDeptReqsearch);
        }

        public void ResetHplnk()
        {
            InPatientTransferDeptReqsearch = new InPatientTransferDeptReq { ReqDate = Globals.GetCurServerDateTime()};
        }

        public void AcceptDeptTranferLoaded(object sender)
        {
            ((Button)sender).Visibility = Globals.convertVisibility(mChuyenKhoa_ChapNhanChuyenKhoa);
        }

        MessageBoxTask msgTask;
        public IEnumerator<IResult> DoOpenRegistration(long regID, InPatientTransferDeptReq cur)
        {
            yield return Loader.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK);

            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;

            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent() { Message = eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK });
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long>() { Item = null, ID = regID });
            }
            else
            {
                if (loadRegTask.Registration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)
                {
                    string message = eHCMSResources.Z0085_G1_DayKhongPhaiDKNoiTru + Environment.NewLine + eHCMSResources.Z0084_H1_KhongTheLoadDK;
                    msgTask = new MessageBoxTask(message, eHCMSResources.G0442_G1_TBao);
                    yield return msgTask;
                    yield return Loader.Hide();
                    yield break;
                }

                CurrentRegistration = loadRegTask.Registration;
                if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.Z0082_G1_ChapNhanDNghiChKhoaCuaDKNay))
                {
                    yield break;
                }
                Action<IAcceptChangeDept> onInitDlg = delegate (IAcceptChangeDept vm)
                {
                    vm.CurrentRegistration = CurrentRegistration;
                    vm.CurInPatientTransferDeptReq = cur;
                    vm.LoadLocations(cur.ReqDeptID);
                };
                GlobalsNAV.ShowDialog<IAcceptChangeDept>(onInitDlg);
            }

            yield return Loader.Hide();
        }



        public void AcceptDeptTranferClick(object sender)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                InPatientTransferDeptReq cur = elem.DataContext as InPatientTransferDeptReq;

                if (cur == null || cur.PtRegistrationID <= 0)
                {
                    return;
                }

                if (Globals.isAccountCheck && !Globals.LoggedUserAccount.DeptIDResponsibilityList.Any(x => x == cur.ReqDeptID))
                {
                    MessageBox.Show(eHCMSResources.A0186_G1_Msg_InfoKhTheChapNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }


                Coroutine.BeginExecute(DoOpenRegistration(cur.PtRegistrationID, cur));

            }
        }

        //DeletedTranferClick
        public void DeletedTranferClick(object sender)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                InPatientTransferDeptReq cur = elem.DataContext as InPatientTransferDeptReq;
                if (cur != null)
                {
                    DeleteInPatientTransferDeptReq(cur);
                }
            }
        }

        public void InPatientDeptDetailsTranfer(InPatientDeptDetail p, long InPatientTransferDeptReqID)
        {
            var t = new Thread(() =>
            {
                long patientDeptDetailId;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInPatientDeptDetailsTranfer(p, InPatientTransferDeptReqID, false, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                bool bOK = contract.EndInPatientDeptDetailsTranfer(out patientDeptDetailId, asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0431_G1_Msg_InfoChKhoaOK);
                                    SearchCmd();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0432_G1_Msg_InfoChKhoaFail);
                                }
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                MessageBox.Show(eHCMSResources.A0432_G1_Msg_InfoChKhoaFail);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        public void GetInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            /*▼====: #001*/
            if (p.ReqDate == null || p.ReqDate == DateTime.MinValue)
            {
                MessageBox.Show(eHCMSResources.Z1672_G1_NhapNgCanXem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            /*▲====: #001*/
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetInPatientTransferDeptReq(p,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                allInPatientTransferDeptReq = new ObservableCollection<InPatientTransferDeptReq>();
                                try
                                {
                                    var res = contract.EndGetInPatientTransferDeptReq(asyncResult);
                                    if (res != null)
                                    {
                                        foreach (var inPatientTransferDeptReq in res)
                                        {
                                            allInPatientTransferDeptReq.Add(inPatientTransferDeptReq);
                                        }
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void DeleteInPatientTransferDeptReq(InPatientTransferDeptReq p)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeleteInPatientTransferDeptReq(p.InPatientTransferDeptReqID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                allInPatientTransferDeptReq = new ObservableCollection<InPatientTransferDeptReq>();
                                try
                                {
                                    var res = contract.EndDeleteInPatientTransferDeptReq(asyncResult);
                                    if (res)
                                    {
                                        MessageBox.Show(eHCMSResources.K0537_G1_XoaOk);
                                        SearchCmd();
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                //InPatientAdmissionInfoContent = Globals.GetViewModel<IInPatientAdmissionInfo>();
                return;
            }

            mChuyenKhoa_TimDeNghiChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mChuyenKhoa,
                                               (int)oRegistrionEx.mChuyenKhoa_TimDeNghiChuyenKhoa, (int)ePermission.mView);

            mChuyenKhoa_ChapNhanChuyenKhoa = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mChuyenKhoa,
                                               (int)oRegistrionEx.mChuyenKhoa_ChapNhanChuyenKhoa, (int)ePermission.mView);

        }

        #region checking account

        private bool _mChuyenKhoa_TimDeNghiChuyenKhoa = true;
        private bool _mChuyenKhoa_ChapNhanChuyenKhoa = true;

        public bool mChuyenKhoa_TimDeNghiChuyenKhoa
        {
            get
            {
                return _mChuyenKhoa_TimDeNghiChuyenKhoa;
            }
            set
            {
                if (_mChuyenKhoa_TimDeNghiChuyenKhoa == value)
                    return;
                _mChuyenKhoa_TimDeNghiChuyenKhoa = value;
                NotifyOfPropertyChange(() => mChuyenKhoa_TimDeNghiChuyenKhoa);
            }
        }


        public bool mChuyenKhoa_ChapNhanChuyenKhoa
        {
            get
            {
                return _mChuyenKhoa_ChapNhanChuyenKhoa;
            }
            set
            {
                if (_mChuyenKhoa_ChapNhanChuyenKhoa == value)
                    return;
                _mChuyenKhoa_ChapNhanChuyenKhoa = value;
                NotifyOfPropertyChange(() => mChuyenKhoa_ChapNhanChuyenKhoa);
            }
        }

        //phan nay nam trong module chung

        #endregion

        public void Handle(AcceptChangeDeptViewModelEvent message)
        {
            if (this.IsActive && message != null)
            {
                SearchCmd();
            }
        }

        //private ObservableCollection<long> _LstRefDepartment;
        //public ObservableCollection<long> LstRefDepartment
        //{
        //    get { return _LstRefDepartment; }
        //    set
        //    {
        //        _LstRefDepartment = value;
        //        NotifyOfPropertyChange(() => LstRefDepartment);
        //    }
        //}

        //public void CheckResponsibility()
        //{
        //    List<StaffDeptResponsibilities> results = Globals.LoggedUserAccount.AllStaffDeptResponsibilities;

        //    LstRefDepartment = new ObservableCollection<long>();
        //    if (results != null && results.Count > 0)
        //    {
        //        foreach (var item in results)
        //        {
        //            if (item.ResNhapVien)
        //            {
        //                LstRefDepartment.Add(item.DeptID);
        //            }
        //        }
        //    }

        //    if (LstRefDepartment.Count < 1)
        //    {
        //        MessageBox.Show("Bạn chưa được phân công trách nhiệm với khoa phòng nào. " +
        //                            "\nLiên hệ với người quản trị để được phân bổ khoa phòng chịu trách nhiệm.");
        //    }

        //}



    }
}
