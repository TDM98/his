using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IManagePatientDetails)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ManagePatientDetailsViewModel : Conductor<object>, IManagePatientDetails
        , IHandle<ItemSelected<Patient>>
        , IHandle<CreateNewPatientEvent>                                
        , IHandle<ResultNotFound<Patient>>
                                        
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ManagePatientDetailsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            // 02/12/13 TxD: ExamDate NOW get from Globals
            ExamDate = Globals.GetCurServerDateTime();
            _examDateServerInitVal = ExamDate;
            //getdate();

            authorization();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            searchPatientAndRegVm.mTimBN = mPatient_TimBN;
            searchPatientAndRegVm.mThemBN = mPatient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mPatient_TimDangKy;
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mInfo_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mInfo_XacNhan;
            patientInfoVm.mInfo_XoaThe = mInfo_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = mInfo_XemPhongKham;

            //var patientInfoVm = Globals.GetViewModel<IPatientInfo>();
            PatientSummaryInfoContent = patientInfoVm;
            //PatientSummaryInfoContent.DisplayButtons = false;
            ActivateItem(patientInfoVm);

            var patientDetailsVm = Globals.GetViewModel<IPatientDetails>();
            PatientDetailsContent = patientDetailsVm;

            patientDetailsVm.ActiveTab = PatientInfoTabs.GENERAL_INFO;
            patientDetailsVm.ActivationMode = ActivationMode.PATIENT_GENERAL_HI_VIEW; 

            //patientDetailsVm.ShowCloseFormButton = false;

            //patientDetailsVm.mNhanBenh_ThongTin_Sua = mNhanBenh_ThongTin_Sua;
            //patientDetailsVm.mNhanBenh_TheBH_ThemMoi = mNhanBenh_TheBH_ThemMoi;
            //patientDetailsVm.mNhanBenh_TheBH_XacNhan = mNhanBenh_TheBH_XacNhan;
            //patientDetailsVm.mNhanBenh_DangKy = mNhanBenh_DangKy;
            //patientDetailsVm.mNhanBenh_TheBH_Sua = mNhanBenh_TheBH_Sua;

            ActivateItem(patientDetailsVm);

            var loginVm = Globals.GetViewModel<ILogin>();
            if (loginVm.DeptLocation != null)
            {
                _department = loginVm.DeptLocation.RefDepartment;
            }
            else
            {
                _department = null;
            }
        }

        private void ResetView()
        {
            //PatientSummaryInfoContent.ConfirmedPaperReferal = null;
            //PatientSummaryInfoContent.ConfirmedHiItem = null;
            //PatientSummaryInfoContent.HiBenefit = null;
        }

        // 02/12/13 TxD: ONLY SET ExamDate at the very last just before Saving
        //public void getdate()
        //{
        //    Coroutine.BeginExecute(DoGetTime());
        //}

        //public IEnumerator<IResult> DoGetTime()
        //{
        //    var loadCurrentDate = new LoadCurrentDateTask();
        //    yield return loadCurrentDate;
        //    DateTime today;
        //    if (loadCurrentDate.CurrentDate == DateTime.MinValue)
        //    {
        //        today = DateTime.Now.Date;
        //        ExamDate = DateTime.Now.Date;
        //        _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0144_G1_KhongTheLayNgThTuServer), eHCMSResources.G0442_G1_TBao);
        //        yield return _msgTask;
        //    }
        //    else
        //    {
        //        today = loadCurrentDate.CurrentDate;
        //        ExamDate = loadCurrentDate.CurrentDate;
        //    }
        //}

        protected override void OnActivate()
        {
            base.OnActivate();

            // 02/12/13 TxD: ExamDate is NOW get from Globals
            ExamDate = Globals.GetCurServerDateTime();
            _examDateServerInitVal = ExamDate;
            //getdate();

            authorization();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        private object _searchRegistrationContent;

        public object SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                _pageTitle = value;
                NotifyOfPropertyChange(() => PageTitle);
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

        public string Temp38
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    return string.Format(eHCMSResources.Z0490_G1_In, eHCMSResources.T3710_G1_Mau01BvNgTru);
                }
                else
                {

                    return string.Format(eHCMSResources.Z0490_G1_In, eHCMSResources.Z0325_G1_Mau02BVNoiTru);
                }
            }

        }
        //Temp38
        private IPatientSummaryInfoV2 _patientSummaryInfoContent;

        public IPatientSummaryInfoV2 PatientSummaryInfoContent
        {
            get { return _patientSummaryInfoContent; }
            set
            {
                _patientSummaryInfoContent = value;
                NotifyOfPropertyChange(() => PatientSummaryInfoContent);
            }
        }

        private IPatientDetails _patientDetailsContent;
        public IPatientDetails PatientDetailsContent
        {
            get { return _patientDetailsContent; }
            set
            {
                _patientDetailsContent = value;
                NotifyOfPropertyChange(() => PatientDetailsContent);
            }
        }

        private bool _mCuocHen_TraiHen;
        public bool mCuocHen_TraiHen
        {
            get { return _mCuocHen_TraiHen; }
            set
            {
                _mCuocHen_TraiHen = value;
                NotifyOfPropertyChange(() => mCuocHen_TraiHen);
            }
        }

        private bool _mCuocHen_DungHen;
        public bool mCuocHen_DungHen
        {
            get { return _mCuocHen_DungHen; }
            set
            {
                _mCuocHen_DungHen = value;
                NotifyOfPropertyChange(() => mCuocHen_DungHen);
            }
        }

        private Status _TitleStatus = Status.None;
        public Status TitleStatus
        {
            get { return _TitleStatus; }
            set
            {
                _TitleStatus = value;
                NotifyOfPropertyChange(() => TitleStatus);
                switch (TitleStatus)
                {
                    case Status.None:
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = false;
                        break;
                    case Status.CuocHen_DungHen:
                        mCuocHen_DungHen = true;
                        mCuocHen_TraiHen = false;
                        break;
                    case Status.CuocHen_TraiHen:
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Loại đăng ký (Nhận bệnh cho đăng ký nội trú hay ngoại trú)
        /// </summary>
        private AllLookupValues.RegistrationType _registrationType = AllLookupValues.RegistrationType.Unknown;
        public AllLookupValues.RegistrationType RegistrationType 
        {
            get
            {
                return _registrationType;
            }
            set
            {
                _registrationType = value;
                if (PatientDetailsContent != null)
                {
                    PatientDetailsContent.RegistrationType = value;
                    // TxD: Nhan Benh Noi Tru or Cap Cuu khong BHYT initially
                    if (value == AllLookupValues.RegistrationType.CAP_CUU || value == AllLookupValues.RegistrationType.NOI_TRU)
                    {
                        //PatientDetailsContent.ActivationMode = ActivationMode.EDIT_PATIENT_GENERAL_INFO;
                        PatientDetailsContent.HITabVisible = false;
                    }
                }
            }
        }

        public bool IsEmergency { get; set; }

        private PatientAppointment _currentAppointment;

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                //if (_currentPatient == value)
                //{
                //    return;
                //}
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
                NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                PatientSummaryInfoContent.CurrentPatient = _currentPatient;
                //ExamDate = DateTime.Now;
                NotifyOfPropertyChange(() => CanReportRegistrationInfoInsuranceCmd);
                
            }
        }


        private RegistrationFormMode _currentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _currentRegMode;
            }
            set
            {
                if (_currentRegMode != value)
                {
                    _currentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                    NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                }
            }
        }


        public void Handle(ItemSelected<Patient> message)
        {            
            if (this.IsActive && message != null)
            {
                CurrentPatient = message.Item;                
                Coroutine.BeginExecute(SetCurrentPatient_CoRoutine(message.Item));
            }
        }


        // TxD 14/07/2014: The following method has been added to get rid of the call to StartEditingPatientLazyLoad
        //                  in method SetCurrentPatient when it is called with parameter patient == null
        //                  because StartEditingPatientLazyLoad has now been removed from PatientDetailsViewModel due to RACING condition that may occur
        public void SetCurrentPatientToNull()
        {
            
            CurrentPatient = null;
            CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;

        }


        private IEnumerator<IResult> SetCurrentPatient_CoRoutine(Patient patient)
        {
            
            var p = patient;
            if (p == null || p.PatientID <= 0)
            {
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
            }
            else
            {
                CurrentPatient = patient;
                CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
            }

            yield return new GenericCoRoutineTask(PatientDetailsContent.LoadPatientDetailsAndHI_GenAction, patient, false);

            yield break;
        }

        public void Handle(CreateNewPatientEvent message)
        {
            if (GetView() != null && message != null)
            {
                // TxD 03/12/13
                ExamDate = Globals.GetCurServerDateTime();
                _examDateServerInitVal = ExamDate;

                if (string.IsNullOrEmpty(GlobalsNAV.HIRegistrationForm))
                {
                    // TxD 14/07/2014: replaced the following with SetPatientToNull
                    //SetCurrentPatient(null);
                    SetCurrentPatientToNull();

                    PatientDetailsContent.CreateNewPatient(true);
                    ResetView();
                }
                else
                {
                    Coroutine.BeginExecute(GlobalsNAV.DoMessageBoxHIRegis(), null, (o, e) =>
                    {
                        if (GlobalsNAV.msgb.Result == AxMessageBoxResult.Ok)
                        {
                            // TxD 14/07/2014: replaced the following with SetPatientToNull
                            //SetCurrentPatient(null);
                            SetCurrentPatientToNull();

                            PatientDetailsContent.CreateNewPatient();
                            ResetView();
                            GlobalsNAV.msgb = null;
                            GlobalsNAV.HIRegistrationForm = "";
                        }
                    });
                }
                //if (PatientDetailsContent.GeneralInfoChanged || PatientDetailsContent.HealthInsuranceContent.InfoHasChanged
                //    || PatientDetailsContent.HealthInsuranceContent.PaperReferalContent.InfoHasChanged)
                //{
                //    if (MessageBox.Show("Thông tin đã thay đổi. Bạn có muốn bỏ qua?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                //    {
                //        return;
                //    }
                //}

            }
        }



        private bool _registrationInfoHasChanged;
        /// <summary>
        /// Cho biet thong tin dang ky tren form da duoc thay doi chua.
        /// </summary>
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    NotifyOfPropertyChange(() => RegistrationInfoHasChanged);

                    //NotifyButtonBehaviourChanges();
                }
            }
        }

        public bool IsProcessing
        {
            get
            {
                return _patientLoading;
            }
        }

        private bool _patientLoading;
        /// <summary>
        /// Dang trong qua trinh lay thong tin benh nhan tu server.
        /// </summary>
        public bool PatientLoading
        {
            get
            {
                return _patientLoading;
            }
            set
            {
                _patientLoading = value;
                NotifyOfPropertyChange(() => PatientLoading);

                NotifyWhenBusy();
            }
        }
        private void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
        }
        private bool _patientLoaded;
        public bool PatientLoaded
        {
            get
            {
                return _patientLoaded;
            }
            set
            {
                _patientLoaded = value;
                NotifyOfPropertyChange(() => PatientLoaded);
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0727_G1_Msg_ConfThemMoiBN,
                                                          eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    // TxD 03/12/13
                    ExamDate = Globals.GetCurServerDateTime();
                    _examDateServerInitVal = ExamDate;

                    // TxD 14/07/2014: replaced the following with SetPatientToNull
                    //SetCurrentPatient(null);
                    SetCurrentPatientToNull();

                    var criteria = message.SearchCriteria as PatientSearchCriteria;
                    PatientDetailsContent.CreateNewPatient();
                    if (criteria != null)
                    {
                        PatientDetailsContent.CurrentPatient.FullName = criteria.FullName;
                    }
                }
            }
        }

        public void OldRegistrationsCmd()
        {
            //var vm = Globals.GetViewModel<IRegistrationList>();
            //vm.CurrentPatient = CurrentPatient;
            //Globals.ShowDialog(vm as Conductor<object>);

            Action<IRegistrationList> onInitDlg = (vm) =>
            {
                vm.CurrentPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg);
        }

        private bool _isRegistering;
        public bool IsRegistering
        {
            get
            {
                return _isRegistering;
            }
            set
            {
                _isRegistering = value;
                NotifyOfPropertyChange(() => IsRegistering);

                NotifyWhenBusy();
            }
        }


        private bool _isRegisterPatient = false;

        private long? RegistrationID = 0;
        public void CreateRegistrationCmd()
        {
            CanCreateNewRegistration = false;
            
        }



        private bool _canCreateNewRegistration;

        public bool CanCreateNewRegistration
        {
            get { return _canCreateNewRegistration; }
            set
            {
                _canCreateNewRegistration = value;
                NotifyOfPropertyChange(() => CanCreateRegistrationCmd);
                NotifyOfPropertyChange(() => CanReportRegistrationInfoInsuranceCmd);                
            }
        }


        private bool _CanCancelRegistrationInfoCmd = false;

        public bool CanCancelRegistrationInfoCmd
        {
            get { return _CanCancelRegistrationInfoCmd; }
            set
            {
                _CanCancelRegistrationInfoCmd = value;
                NotifyOfPropertyChange(() => CanCancelRegistrationInfoCmd);
            }
        }

        public bool CanCreateRegistrationCmd
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.Unknown)
                {
                    return false;
                }
                return _currentPatient != null && CurrentRegMode == RegistrationFormMode.PATIENT_SELECTED && CanCreateNewRegistration ;
            }
        }

        private DateTime _examDateServerInitVal;
        private DateTime _examDate;
        public DateTime ExamDate
        {
            get
            {
                return _examDate;
            }
            set
            {
                if (_examDate != value)
                {
                    _examDate = value;
                    NotifyOfPropertyChange(() => ExamDate);
                }
            }
        }

        private readonly RefDepartment _department;


        
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mNhanBenh_ThongTin_Sua = true;
        private bool _mNhanBenh_TheBH_ThemMoi = false;
        private bool _mNhanBenh_TheBH_XacNhan = false;
        private bool _mNhanBenh_DangKy = false;
        private bool _mNhanBenh_TheBH_Sua = false;

        public bool mNhanBenh_ThongTin_Sua
        {
            get
            {
                return _mNhanBenh_ThongTin_Sua;
            }
            set
            {
                if (_mNhanBenh_ThongTin_Sua == value)
                    return;
                _mNhanBenh_ThongTin_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenh_ThongTin_Sua);
            }
        }


        public bool mNhanBenh_TheBH_ThemMoi
        {
            get
            {
                return _mNhanBenh_TheBH_ThemMoi;
            }
            set
            {
                if (_mNhanBenh_TheBH_ThemMoi == value)
                    return;
                _mNhanBenh_TheBH_ThemMoi = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_ThemMoi);
            }
        }


        public bool mNhanBenh_TheBH_XacNhan
        {
            get
            {
                return _mNhanBenh_TheBH_XacNhan;
            }
            set
            {
                if (_mNhanBenh_TheBH_XacNhan == value)
                    return;
                _mNhanBenh_TheBH_XacNhan = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_XacNhan);
            }
        }

        public bool mNhanBenh_TheBH_Sua
        {
            get
            {
                return _mNhanBenh_TheBH_Sua;
            }
            set
            {
                if (_mNhanBenh_TheBH_Sua == value)
                    return;
                _mNhanBenh_TheBH_Sua = value;
                NotifyOfPropertyChange(() => mNhanBenh_TheBH_Sua);
            }
        }

        public bool mNhanBenh_DangKy
        {
            get
            {
                return _mNhanBenh_DangKy;
            }
            set
            {
                if (_mNhanBenh_DangKy == value)
                    return;
                _mNhanBenh_DangKy = value;
                NotifyOfPropertyChange(() => mNhanBenh_DangKy);
            }
        }


        //phan nay nam trong module chung
        private bool _mPatient_TimBN = true;
        private bool _mPatient_ThemBN = true;
        private bool _mPatient_TimDangKy = false;

        private bool _mInfo_CapNhatThongTinBN = true;
        private bool _mInfo_XacNhan = false;
        private bool _mInfo_XoaThe = false;
        private bool _mInfo_XemPhongKham = false;

        public bool mPatient_TimBN
        {
            get
            {
                return _mPatient_TimBN;
            }
            set
            {
                if (_mPatient_TimBN == value)
                    return;
                _mPatient_TimBN = value;
                NotifyOfPropertyChange(() => mPatient_TimBN);
            }
        }

        public bool mPatient_ThemBN
        {
            get
            {
                return _mPatient_ThemBN;
            }
            set
            {
                if (_mPatient_ThemBN == value)
                    return;
                _mPatient_ThemBN = value;
                NotifyOfPropertyChange(() => mPatient_ThemBN);
            }
        }

        public bool mPatient_TimDangKy
        {
            get
            {
                return _mPatient_TimDangKy;
            }
            set
            {
                if (_mPatient_TimDangKy == value)
                    return;
                _mPatient_TimDangKy = value;
                NotifyOfPropertyChange(() => mPatient_TimDangKy);
            }
        }

        public bool mInfo_CapNhatThongTinBN
        {
            get
            {
                return _mInfo_CapNhatThongTinBN;
            }
            set
            {
                if (_mInfo_CapNhatThongTinBN == value)
                    return;
                _mInfo_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mInfo_CapNhatThongTinBN);
            }
        }

        public bool mInfo_XacNhan
        {
            get
            {
                return _mInfo_XacNhan;
            }
            set
            {
                if (_mInfo_XacNhan == value)
                    return;
                _mInfo_XacNhan = value;
                NotifyOfPropertyChange(() => mInfo_XacNhan);
            }
        }

        public bool mInfo_XoaThe
        {
            get
            {
                return _mInfo_XoaThe;
            }
            set
            {
                if (_mInfo_XoaThe == value)
                    return;
                _mInfo_XoaThe = value;
                NotifyOfPropertyChange(() => mInfo_XoaThe);
            }
        }

        public bool mInfo_XemPhongKham
        {
            get
            {
                return _mInfo_XemPhongKham;
            }
            set
            {
                if (_mInfo_XemPhongKham == value)
                    return;
                _mInfo_XemPhongKham = value;
                NotifyOfPropertyChange(() => mInfo_XemPhongKham);
            }
        }


        #endregion


        string HIComment = "";



        public bool CanReportRegistrationInfoInsuranceCmd
        {
            get
            {
                if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    return !CanCreateNewRegistration && CurrentPatient != null && CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.HisID.GetValueOrDefault(0) > 0;
                }
                else
                {
                    return !CanCreateNewRegistration && CurrentPatient != null && CurrentPatient.LatestRegistration_InPt != null && CurrentPatient.LatestRegistration_InPt.HisID.GetValueOrDefault(0) > 0;
                }
            }
        }


        private bool RegistrationCancelling { get; set; }


        public void CancelRegistration(PatientRegistration CurRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginCancelRegistration(CurRegistration,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                PatientRegistration registration = null;
                                try
                                {
                                    bool bOK = contract.EndCancelRegistration(out registration, asyncResult);
                                    CurRegistration = registration;
                                    MessageBox.Show(eHCMSResources.A0613_G1_Msg_InfoHuyOK);
                                    CanCreateNewRegistration = true;
                                    CanCancelRegistrationInfoCmd = false;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }
    }

    public enum Status
    {
        None = 1,
        CuocHen_DungHen = 2,
        CuocHen_TraiHen = 3
    }

}

//public bool CanCreateRegistrationCmd
//{
//    get
//    {
//        if (RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//        {
//            return _currentPatient != null //&& _confirmedHiItem != null
//                && CurrentRegMode == RegistrationFormMode.PATIENT_SELECTED && CanCreateNewRegistration;
//        }
//        if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//        {
//            return _currentPatient != null
//                   && CurrentRegMode == RegistrationFormMode.PATIENT_SELECTED && CanCreateNewRegistration;
//        }
//        return false;
//    }
//}


//private IEnumerator<IResult> DoSetCurrentPatient(Patient p, bool bFromCancelRegistration = false)
//{
//    CanCreateNewRegistration = true;
//    CanCancelRegistrationInfoCmd = false;

//    // 02/12/13 TxD: ExamDate NOW get from Globals
//    ExamDate = Globals.ServerDate.Value;
//    _examDateServerInitVal = ExamDate;

//    ConfirmedHiItem = null;
//    ConfirmedPaperReferal = null;
//    if (p == null || p.PatientID <= 0)
//    {
//        _currentAppointment = null;
//        CurrentPatient = null;
//        CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
//        yield break;
//    }
//    else
//    {
//        //KMx: Nếu không phải từ cuộc hẹn gọi thì set _currentAppointment = null. Tránh trường hợp khi tìm bệnh nhân, người sau lấy cuộc hẹn của người trước đi lưu (16/04/2014 17:06).
//        if (!p.FromAppointment)
//        {
//            _currentAppointment = null;
//        }
//    }

//    //yield return Loader.Show("Đang lấy thông tin bệnh nhân........");

//    PatientLoaded = false;
//    PatientLoading = true;
//    var loadPatient = new LoadPatientTask(p.PatientID);
//    yield return loadPatient;
//    Globals.EventAggregator.Publish(new PatientReloadEvent { curPatient = loadPatient.CurrentPatient });

//    // 03/12/2013 TxD : The following Code lines have been commented out except today
//    // because Examdate NOW get from Globals.

//    DateTime today = Globals.ServerDate.Value.Date;

//    if (loadPatient.CurrentPatient != null)
//    {
//        bool bAlreadyRegistered = false;

//        if (loadPatient.CurrentPatient.latestHIRegistration != null && loadPatient.CurrentPatient.latestHIRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.REFUND)
//        {
//            var regDate = loadPatient.CurrentPatient.latestHIRegistration.ExamDate;

//            //Kiểm tra xem có được phép tạo mới ĐKBH hay không?
//            if (AxHelper.CompareDate(regDate, today) != 1 && AxHelper.CompareDate(regDate.AddDays(ConfigValues.PatientRegistrationTimeout), today) != 2)
//            {
//                bAlreadyRegistered = true;

//                string sLoaiDK = Globals.GetTextV_RegistrationType((long)loadPatient.CurrentPatient.latestHIRegistration.V_RegistrationType);
//                if (!string.IsNullOrEmpty(sLoaiDK))
//                {
//                    sLoaiDK = eHCMSResources.Z0028_G1_DauNgoacTrai.ToUpper() + sLoaiDK + ")";
//                }

//                //chi thong bao thoi
//                _msgTask = new MessageBoxTask("Bệnh nhân này đã đăng ký " + sLoaiDK + "!", eHCMSResources.G0442_G1_TBao, MessageBoxOptions.Ok);
//                yield return _msgTask;

//                CanCreateNewRegistration = false; //dinh them nho bo ra
//                CanCancelRegistrationInfoCmd = true;

//            }

//            var loadRegTask = new LoadRegistrationInfoTask(loadPatient.CurrentPatient.latestHIRegistration.PtRegistrationID, true);
//            yield return loadRegTask;
//            loadPatient.CurrentPatient.latestHIRegistration = loadRegTask.Registration;

//            if (loadRegTask.Registration != null && loadRegTask.Registration.HealthInsurance != null)
//            {
//                PatientSummaryInfoContent.HiBenefit = loadRegTask.Registration.HealthInsurance.HIPatientBenefit;
//                PatientSummaryInfoContent.HiComment = loadRegTask.Registration.HIComment;
//            }
//        }

//        //KMx: (16/04/2014 12:03)
//        //FromAppointment: Có phải là pop-up Danh sách cuộc hẹn gọi hay không? Nếu là pop-up gọi thì không load cuộc hẹn nữa.
//        //bAlreadyRegistered: Trong thời gian cho phép, đã có ĐKBH chưa? Nếu đã có ĐKBH rồi thì không load cuộc hẹn.
//        //bFromCancelRegistration: Sau khi hủy đăng ký thì không load cuộc hẹn nữa.
//        if (!p.FromAppointment && !bAlreadyRegistered && !bFromCancelRegistration && loadPatient.CurrentPatient.AppointmentList != null && loadPatient.CurrentPatient.AppointmentList.Count > 0)
//        {
//            //Nếu có 1 cuộc hẹn đã xác nhận và đúng hẹn.
//            if (loadPatient.CurrentPatient.AppointmentList.Count == 1 && loadPatient.CurrentPatient.AppointmentList[0].ApptDate.Value.Date == today)
//            {
//                var result = MessageBox.Show("Đăng kí bảo hiểm cho cuộc hẹn hay đăng kí mới? \n Chọn 'OK' để đăng kí bảo hiểm cho cuộc hẹn. \n Chọn 'Cancel' để đăng kí mới.", "Thông báo quan trọng", MessageBoxButton.OKCancel);
//                if (result == MessageBoxResult.OK)
//                {
//                    _currentAppointment = loadPatient.CurrentPatient.AppointmentList[0].DeepCopy();
//                    _currentAppointment.Patient = new Patient();
//                    _currentAppointment.Patient.PatientID = loadPatient.CurrentPatient.PatientID;
//                }
//            }
//            //Nếu có 1 cuộc hẹn không đúng hẹn hoặc nhiều hơn 1 cuộc hẹn đã xác nhận
//            else
//            {
//                //Open pop-up cho user chọn cuộc hẹn
//                var vm = Globals.GetViewModel<IFindAppointment>();
//                if (!string.IsNullOrEmpty(_currentPatient.PatientCode))
//                {
//                    vm.SearchCriteria.PatientCode = _currentPatient.PatientCode;
//                    vm.SearchCriteria.V_ApptStatus = (long)AllLookupValues.ApptStatus.BOOKED;
//                    vm.SearchCmd();
//                }
//                Globals.ShowDialog(vm as Conductor<object>);
//            }
//        }


//        if (RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//        {
//            if (loadPatient.CurrentPatient.LatestRegistration_InPt != null)
//            {
//                if (loadPatient.CurrentPatient.LatestRegistration_InPt.DischargeDate == null)
//                {
//                    MessageBox.Show("BN này đã được đăng ký nội trú!");
//                    CanCreateNewRegistration = false;
//                }
//                else
//                {
//                    CanCreateNewRegistration = true;
//                }
//            }
//        }

//        CurrentPatient = ObjectCopier.DeepCopy(loadPatient.CurrentPatient);
//        CurrentRegMode = RegistrationFormMode.PATIENT_SELECTED;
//    }
//    else
//    {
//        CurrentPatient = null;
//        CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;

//        _msgTask = new MessageBoxTask(eHCMSResources.Z0146_G1_KhongTheLayTTinBN, eHCMSResources.G0442_G1_TBao);
//        yield return _msgTask;
//    }


//    if (_currentAppointment != null && _currentAppointment.AppointmentID > 0 && _currentAppointment.ApptDate != null)
//    {
//        if (_currentAppointment.ApptDate.Value.Date == today)
//        {
//            TitleStatus = Status.CuocHen_DungHen;
//        }
//        else
//        {
//            TitleStatus = Status.CuocHen_TraiHen;
//        }
//    }
//    else
//    {
//        TitleStatus = Status.None;
//    }

//    PatientDetailsContent.StartEditingPatientLazyLoad(loadPatient.CurrentPatient);
//    ConfirmedHiItem = null;
//    ConfirmedPaperReferal = null;
            
//    //yield return Loader.Hide();

//    //if (CanCreateNewRegistration)
//    //{
//    //    //Update trạng thái của đăng ký cuối này của Ngoại Trú sang PendingClose
//    //    //khong xai cai nay nua
//    //    if (loadPatient.CurrentPatient.latestHIRegistration!=null && loadPatient.CurrentPatient.latestHIRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//    //    {
//    //        var ResultUpdateNgoaiTru = new Registrations_UpdateStatusTask(loadPatient.CurrentPatient.latestHIRegistration, (long)AllLookupValues.RegistrationStatus.PENDINGCLOSE);
//    //        yield return ResultUpdateNgoaiTru;

//    //        if (ResultUpdateNgoaiTru.Result == false)
//    //        {
//    //            _msgTask = new MessageBoxTask(eHCMSResources.Z0232_G1_KgTheCNhatTThaiPendingClose, eHCMSResources.G0442_G1_TBao);
//    //            yield return _msgTask;
//    //        }
//    //    }
//    //}

//    if (CurrentPatient != null)
//    {
//        if (CurrentPatient.latestHIRegistration != null && CurrentPatient.latestHIRegistration.PtRegistrationID > 0 && CanCreateNewRegistration == false)
//        {
//            Globals.HIRegistrationForm = "";
//        }
//        else
//        {
//            Globals.HIRegistrationForm = string.Format("{0} ", eHCMSResources.Z0208_G1_ChưaDKChoBN) + CurrentPatient.FullName + " !Bạn có muốn bỏ qua không?";
//        }
//    }
//}

