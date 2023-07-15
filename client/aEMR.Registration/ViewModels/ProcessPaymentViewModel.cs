using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IProcessPayment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ProcessPaymentViewModel : HandlePayCompletedViewModelBase, IProcessPayment
        , IHandle<ItemSelected<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<PayForRegistrationCompleted>
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<UpdateCompleted<PatientRegistration>>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public ProcessPaymentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            authorization();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            //searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.mTimBN = mTinhTien_Patient_TimBN;
            searchPatientAndRegVm.mThemBN = mTinhTien_Patient_ThemBN;
            searchPatientAndRegVm.mTimDangKy = mTinhTien_Patient_TimDangKy;
            (searchPatientAndRegVm as INotifyPropertyChangedEx).PropertyChanged += searchPatientAndRegVm_PropertyChanged;

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mTinhTien_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mTinhTien_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mTinhTien_Info_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = mTinhTien_Info_XemPhongKham;
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            var regDetailsVm = Globals.GetViewModel<IRegistrationSummaryV2>();
            RegistrationDetailsContent = regDetailsVm;
            RegistrationDetailsContent.CanStartAddingNewServicesAndPclEx = false;
            RegistrationDetailsContent.CanStartEditRegistrationEx = false;

            RegistrationDetailsContent.mDichVuDaTT_ChinhSua = mTinhTien_DichVuDaTT_ChinhSua;
            RegistrationDetailsContent.mDichVuDaTT_Luu = mTinhTien_DichVuDaTT_Luu;
            RegistrationDetailsContent.mDichVuDaTT_TraTien = mTinhTien_DichVuDaTT_TraTien;
            RegistrationDetailsContent.mDichVuDaTT_In = mTinhTien_DichVuDaTT_In;
            RegistrationDetailsContent.mDichVuDaTT_LuuTraTien = mTinhTien_DichVuDaTT_LuuTraTien;

            RegistrationDetailsContent.mDichVuMoi_ChinhSua = mTinhTien_DichVuMoi_ChinhSua;
            RegistrationDetailsContent.mDichVuMoi_Luu = mTinhTien_DichVuMoi_Luu;
            RegistrationDetailsContent.mDichVuMoi_TraTien = mTinhTien_DichVuMoi_TraTien;
            RegistrationDetailsContent.mDichVuMoi_In = mTinhTien_DichVuMoi_In;
            RegistrationDetailsContent.mDichVuMoi_LuuTraTien = mTinhTien_DichVuMoi_LuuTraTien;
            //RegistrationDetailsContent.IsEnableRoleUser = true;

            //RegistrationDetailsContent.ShowCheckBoxColumn = false;
            //RegistrationDetailsContent.ShowDeleteColumn = false;
            ActivateItem(regDetailsVm);
        }
        void searchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0113_G1_IsLoading || e.PropertyName == eHCMSResources.Z0114_G1_IsSearchingRegistration)
            {
                NotifyWhenBusy();
            }
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();
            Globals.EventAggregator.Subscribe(this);

            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = Globals.GetViewModel<IPaymentOutStandingTask>();
            homeVm.IsExpandOST = true;
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);

            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
        }

        private int _FindPatient;
        public int FindPatient
        {
            get { return _FindPatient; }
            set
            {
                if (_FindPatient != value)
                {
                    _FindPatient = value;
                    NotifyOfPropertyChange(() => FindPatient);
                }
            }
        }

        private ISearchPatientAndRegistration _searchRegistrationContent;

        public ISearchPatientAndRegistration SearchRegistrationContent
        {
            get { return _searchRegistrationContent; }
            set
            {
                _searchRegistrationContent = value;
                NotifyOfPropertyChange(() => SearchRegistrationContent);
            }
        }

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

        private IRegistrationSummaryV2 _registrationDetailsContent;
        public IRegistrationSummaryV2 RegistrationDetailsContent
        {
            get { return _registrationDetailsContent; }
            set
            {
                _registrationDetailsContent = value;
                NotifyOfPropertyChange(() => RegistrationDetailsContent);
            }
        }

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            private set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);

                if (PatientSummaryInfoContent != null)
                {
                    PatientSummaryInfoContent.CurrentPatient = _currentPatient;
                }
            }
        }

        private PatientRegistration _curRegistration;

        public PatientRegistration CurRegistration
        {
            get { return _curRegistration; }
            set
            {
                _curRegistration = value;
                NotifyOfPropertyChange(() => CurRegistration);
                NotifyOfPropertyChange(() => CanPayCmd);

                RegistrationDetailsContent.SetRegistration(CurRegistration);
                PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
            }
        }

        private HealthInsurance _confirmedHiItem;
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
        public HealthInsurance ConfirmedHiItem
        {
            get
            {
                return _confirmedHiItem;
            }
            private set
            {
                _confirmedHiItem = value;
                NotifyOfPropertyChange(() => ConfirmedHiItem);                
            }
        }

        private PaperReferal _confirmedPaperReferal;
        /// <summary>
        /// Thông tin giấy chuyển viện đã được confirm
        /// </summary>
        public PaperReferal ConfirmedPaperReferal
        {
            get
            {
                return _confirmedPaperReferal;
            }
            private set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);                
            }
        }

        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item as Patient;
                if (CurrentPatient != null)
                {
                    SetCurrentPatient(CurrentPatient);
                }
            }
        }


        private bool _isPaying;

        public bool IsPaying
        {
            get { return _isPaying; }
            set
            {
                _isPaying = value;
                NotifyOfPropertyChange(() => IsPaying);
            }
        }

        public bool CanPayCmd
        {
            get
            {
                return _curRegistration != null && _curRegistration.PtRegistrationID > 0;
            }
        }

        public void PayCmd()
        {
            Action<IPay> onInitDlg = delegate (IPay vm)
            {
                vm.Registration = CurRegistration;
                vm.SetRegistration(CurRegistration);
            };
            GlobalsNAV.ShowDialog<IPay>(onInitDlg);
        }

        public void SetCurrentPatient(object patient)
        {
            Patient p = patient as Patient;
            if (p == null || p.PatientID <= 0)
            {
                CurRegistration = null;
                return;
            }

            if (p.PatientID > 0)
            {
                GetPatientByID(p.PatientID);
            }
        }
        private bool _patientLoading = false;
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
                // Hpt 27/11/2015: Đã gán giá trị trong hàm khởi tạo rồi nhưng không có thời gian xem lại nên cứ để thêm một lần nữa ở đây, có thời gian sẽ xem lại và điều chỉnh 
                if (SearchRegistrationContent != null)
                {
                    SearchRegistrationContent.PatientFindBy = PatientFindBy;
                }
            }
        }


        private new void NotifyWhenBusy()
        {
            NotifyOfPropertyChange(() => IsProcessing);
            NotifyOfPropertyChange(() => StatusText);
        }

        public override bool IsProcessing
        {
            get
            {
                return RegistrationLoading || PatientLoading;

                //return _searchRegistrationContent.IsLoading || _searchRegistrationContent.IsSearchingRegistration
                //    || RegistrationLoading || PatientLoading;
            }
        }
        public override string StatusText
        {
            get
            {
                if (PatientLoading)
                {
                    return eHCMSResources.Z0119_G1_DangLayTTinBN;
                }
                if (RegistrationLoading)
                {
                    return eHCMSResources.Z0086_G1_DangLayTTinDK;
                }
                //if (_searchRegistrationContent.IsLoading)
                //{
                //    return "Đang tìm kiếm bệnh nhân";
                //}
                //if (_searchRegistrationContent.IsSearchingRegistration)
                //{
                //    return "Đang tìm đăng ký";
                //}
                return "";
            }
        }

        private void GetPatientByID(long patientID)
        {
            var t = new Thread(() =>
                                   {
                                       PatientLoading = true;

                                       AxErrorEventArgs error = null;
                                       try
                                       {
                                           using (var serviceFactory = new PatientRegistrationServiceClient())
                                           {
                                               var contract = serviceFactory.ServiceInstance;

                                               contract.BeginGetPatientByID(patientID, true, 
                                                   Globals.DispatchCallback((asyncResult) =>
                                                   {
                                                       try
                                                       {
                                                           var patient = contract.EndGetPatientByID(asyncResult);
                                                           CurrentPatient = patient;

                                                           LoadRegistrationInfo(patient);
                                                       }
                                                       catch (FaultException<AxException> fault)
                                                       {
                                                           error = new AxErrorEventArgs(fault);
                                                       }
                                                       catch (Exception ex)
                                                       {
                                                           error = new AxErrorEventArgs(ex);
                                                       }
                                                   }), null);
                                           }
                                       }
                                       catch (Exception ex)
                                       {
                                           error = new AxErrorEventArgs(ex);
                                       }
                                       finally
                                       {
                                           PatientLoading = false;
                                       }
                                       if (error != null)
                                       {
                                           Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                                       }
                                   });
            t.Start();
        }
        public void LoadRegistrationInfo(Patient p)
        {
            if (p == null)
                return;

            if (p.LatestRegistration == null) //Chưa có đăng ký lần nào
            {
                CurrentPatient = p;
                CurRegistration = null;
                //Thong bao khong tim thay dang ky de tinh tien.
                Globals.ShowMessage(eHCMSResources.Z0120_G1_KhongTimThayDKChoBNNay, eHCMSResources.T0432_G1_Error);
                return;
            }
            //Nếu có đăng ký trong ngày, hoặc còn trong khoảng thời gian có hiệu lực
            DateTime regDate = _currentPatient.LatestRegistration.ExamDate.Date;
            DateTime now;
            //Tam thoi lay ngay Client:

            if (RegistrationDate == DateTime.MinValue)
            {
                now = DateTime.Now.Date;
            }
            else
            {
                now = RegistrationDate;
            }

            if (regDate <= now && regDate.AddDays(ConfigValues.PatientRegistrationTimeout) >= now)
            {
                if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
                    //|| _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
                    )
                {
                    //Mở đăng ký còn đang sử dụng
                    //Tutu lam
                    OpenRegistration(_currentPatient.LatestRegistration.PtRegistrationID);
                }
                else
                {
                    //CurRegistration = null;
                    MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                }
            }
            else
            {
                CurRegistration = null;
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        private DateTime _RegDate;
        public DateTime RegistrationDate
        {
            get
            {
                return _RegDate;
            }
            set
            {
                _RegDate = value;
            }
        }
        private bool _registrationLoading = false;
        /// <summary>
        /// Dang trong qua trinh lay thong tin dang ky tu server.
        /// </summary>
        public bool RegistrationLoading
        {
            get
            {
                return _registrationLoading;
            }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);
                NotifyWhenBusy();
            }
        }
        /// <summary>
        /// Mở đăng ký đã có sẵn
        /// </summary>
        /// <param name="regID">ID của đăng ký</param>
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //Deployment.Current.Dispatcher.BeginInvoke(() => { RegistrationLoading = true; });
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);

            var loadRegTask = new LoadRegistrationInfoTask(regID, true);
            yield return loadRegTask;

            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK });
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = null, ID = regID });
            }
            else
            {
                CurRegistration = loadRegTask.Registration;
                if (_currentPatient == null || CurRegistration.PatientID != CurrentPatient.PatientID)
                {
                    _currentPatient = CurRegistration.Patient;
                    NotifyOfPropertyChange(() => CurrentPatient);
                }
                if (PatientSummaryInfoContent != null)
                {
                    PatientSummaryInfoContent.CurrentPatient = _currentPatient;
                    PatientSummaryInfoContent.CurrentPatientClassification = CurRegistration.PatientClassification;

                    PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);
                }
                ConfirmedHiItem = CurRegistration.HealthInsurance;
                ConfirmedPaperReferal = CurRegistration.PaperReferal;
                RegistrationDetailsContent.HiServiceBeingUsed = CurRegistration.HealthInsurance == null ? false : true;

                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = CurRegistration, ID = regID });

            }
            
            //Deployment.Current.Dispatcher.BeginInvoke(() => { RegistrationLoading = false; });
            this.HideBusyIndicator();

        }
        //public void OpenRegistration(long regID)
        //{
        //    var t = new Thread(() =>
        //    {
        //        RegistrationLoading = true;

        //        AxErrorEventArgs error = null;
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetRegistrationInfo(regID, FindPatient, false,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            var regInfo = contract.EndGetRegistrationInfo(asyncResult);
        //                            RegistrationLoading = false;
        //                            CurRegistration = regInfo;
        //                            if (_currentPatient == null || CurRegistration.PatientID != CurrentPatient.PatientID)
        //                            {
        //                                _currentPatient = CurRegistration.Patient;
        //                                NotifyOfPropertyChange(() => CurrentPatient);
        //                            }
        //                            if (PatientSummaryInfoContent != null)
        //                            {
        //                                PatientSummaryInfoContent.CurrentPatient = _currentPatient;
        //                            }
        //                            ConfirmedHiItem = CurRegistration.HealthInsurance;
        //                            ConfirmedPaperReferal = CurRegistration.PaperReferal;
        //                            if (PatientSummaryInfoContent != null)
        //                            {
        //                                PatientSummaryInfoContent.CurrentPatientClassification = CurRegistration.PatientClassification;
        //                            }

        //                            if (CurRegistration.PtInsuranceBenefit.HasValue)
        //                            {
        //                                PatientSummaryInfoContent.HiBenefit = CurRegistration.PtInsuranceBenefit;
        //                            }
        //                            else
        //                            {
        //                                PatientSummaryInfoContent.HiBenefit = null;
        //                            }
        //                            //ShowOldRegistration(CurRegistration);
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            CurRegistration = null;
        //                            error = new AxErrorEventArgs(fault);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            CurRegistration = null;
        //                            error = new AxErrorEventArgs(ex);
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            CurRegistration = null;
        //            error = new AxErrorEventArgs(ex);
        //        }
        //        finally
        //        {
        //            RegistrationLoading = false;
        //        }
        //        if (error != null)
        //        {
        //            Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
        //        }
        //    });
        //    t.Start();
        //}

        public void OldRegistrationsCmd()
        {
            Action<IRegistrationList> onInitDlg = delegate (IRegistrationList vm)
            {
                vm.CurrentPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg);
        }

        public void Handle(PayForRegistrationCompleted message)
        {
            if (this.GetView() != null && message != null)
            {
                //Load lai dang ky:
                var payment = message.Payment;
                if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                {
                    OpenRegistration(message.Registration.PtRegistrationID);
                    ProcessPayCompletedEvent(message);
                }
            }
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (this.GetView() != null && message != null)
            {
                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    SetCurrentPatient(CurrentPatient);
                }
            }
        }

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (this.GetView() != null && message != null && message.Item != null)
            {
                FindPatient = message.Item.FindPatient;
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (this.GetView() != null)
            {
                MessageBox.Show(message.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mTinhTien_DichVuDaTT_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuDaTT_ChinhSua, (int)ePermission.mView);
            mTinhTien_DichVuDaTT_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuDaTT_Luu, (int)ePermission.mView);
            mTinhTien_DichVuDaTT_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuDaTT_TraTien, (int)ePermission.mView);
            mTinhTien_DichVuDaTT_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuDaTT_In, (int)ePermission.mView);
            mTinhTien_DichVuDaTT_LuuTraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuDaTT_LuuTraTien, (int)ePermission.mView);

            mTinhTien_DichVuMoi_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuMoi_ChinhSua, (int)ePermission.mView);
            mTinhTien_DichVuMoi_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuMoi_Luu, (int)ePermission.mView);
            mTinhTien_DichVuMoi_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuMoi_TraTien, (int)ePermission.mView);
            mTinhTien_DichVuMoi_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuMoi_In, (int)ePermission.mView);
            mTinhTien_DichVuMoi_LuuTraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mProcessPayment,
                                               (int)oRegistrionEx.mTinhTien_DichVuMoi_LuuTraTien, (int)ePermission.mView);

            //phan nay nam trong module chung ne


            mTinhTien_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mProcessPayment,
                                             (int)oRegistrionEx.mTinhTien_Patient_TimBN, (int)ePermission.mView);
            mTinhTien_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mProcessPayment,
                                                 (int)oRegistrionEx.mTinhTien_Patient_ThemBN, (int)ePermission.mView);
            mTinhTien_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mProcessPayment,
                                                 (int)oRegistrionEx.mTinhTien_Patient_TimDangKy, (int)ePermission.mView);

            mTinhTien_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mProcessPayment,
                                                 (int)oRegistrionEx.mTinhTien_Info_CapNhatThongTinBN, (int)ePermission.mView);
            mTinhTien_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mProcessPayment,
                                                 (int)oRegistrionEx.mTinhTien_Info_XacNhan, (int)ePermission.mView);
            mTinhTien_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mProcessPayment,
                                                 (int)oRegistrionEx.mTinhTien_Info_XoaThe, (int)ePermission.mView);
            mTinhTien_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mProcessPayment,
                                                 (int)oRegistrionEx.mTinhTien_Info_XemPhongKham, (int)ePermission.mView);

        }
        #region checking account

        private bool _mTinhTien_DichVuDaTT_ChinhSua = true;
        private bool _mTinhTien_DichVuDaTT_Luu = true;
        private bool _mTinhTien_DichVuDaTT_TraTien = true;
        private bool _mTinhTien_DichVuDaTT_In = true;
        private bool _mTinhTien_DichVuDaTT_LuuTraTien = true;

        private bool _mTinhTien_DichVuMoi_ChinhSua = true;
        private bool _mTinhTien_DichVuMoi_Luu = true;
        private bool _mTinhTien_DichVuMoi_TraTien = true;
        private bool _mTinhTien_DichVuMoi_In = true;
        private bool _mTinhTien_DichVuMoi_LuuTraTien = true;

        public bool mTinhTien_DichVuDaTT_ChinhSua
        {
            get
            {
                return _mTinhTien_DichVuDaTT_ChinhSua;
            }
            set
            {
                if (_mTinhTien_DichVuDaTT_ChinhSua == value)
                    return;
                _mTinhTien_DichVuDaTT_ChinhSua = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuDaTT_ChinhSua);
            }
        }

        public bool mTinhTien_DichVuDaTT_Luu
        {
            get
            {
                return _mTinhTien_DichVuDaTT_Luu;
            }
            set
            {
                if (_mTinhTien_DichVuDaTT_Luu == value)
                    return;
                _mTinhTien_DichVuDaTT_Luu = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuDaTT_Luu);
            }
        }

        public bool mTinhTien_DichVuDaTT_TraTien
        {
            get
            {
                return _mTinhTien_DichVuDaTT_TraTien;
            }
            set
            {
                if (_mTinhTien_DichVuDaTT_TraTien == value)
                    return;
                _mTinhTien_DichVuDaTT_TraTien = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuDaTT_TraTien);
            }
        }

        public bool mTinhTien_DichVuDaTT_In
        {
            get
            {
                return _mTinhTien_DichVuDaTT_In;
            }
            set
            {
                if (_mTinhTien_DichVuDaTT_In == value)
                    return;
                _mTinhTien_DichVuDaTT_In = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuDaTT_In);
            }
        }

        public bool mTinhTien_DichVuDaTT_LuuTraTien
        {
            get
            {
                return _mTinhTien_DichVuDaTT_LuuTraTien;
            }
            set
            {
                if (_mTinhTien_DichVuDaTT_LuuTraTien == value)
                    return;
                _mTinhTien_DichVuDaTT_LuuTraTien = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuDaTT_LuuTraTien);
            }
        }


        public bool mTinhTien_DichVuMoi_ChinhSua
        {
            get
            {
                return _mTinhTien_DichVuMoi_ChinhSua;
            }
            set
            {
                if (_mTinhTien_DichVuMoi_ChinhSua == value)
                    return;
                _mTinhTien_DichVuMoi_ChinhSua = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuMoi_ChinhSua);
            }
        }

        public bool mTinhTien_DichVuMoi_Luu
        {
            get
            {
                return _mTinhTien_DichVuMoi_Luu;
            }
            set
            {
                if (_mTinhTien_DichVuMoi_Luu == value)
                    return;
                _mTinhTien_DichVuMoi_Luu = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuMoi_Luu);
            }
        }

        public bool mTinhTien_DichVuMoi_TraTien
        {
            get
            {
                return _mTinhTien_DichVuMoi_TraTien;
            }
            set
            {
                if (_mTinhTien_DichVuMoi_TraTien == value)
                    return;
                _mTinhTien_DichVuMoi_TraTien = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuMoi_TraTien);
            }
        }

        public bool mTinhTien_DichVuMoi_In
        {
            get
            {
                return _mTinhTien_DichVuMoi_In;
            }
            set
            {
                if (_mTinhTien_DichVuMoi_In == value)
                    return;
                _mTinhTien_DichVuMoi_In = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuMoi_In);
            }
        }

        public bool mTinhTien_DichVuMoi_LuuTraTien
        {
            get
            {
                return _mTinhTien_DichVuMoi_LuuTraTien;
            }
            set
            {
                if (_mTinhTien_DichVuMoi_LuuTraTien == value)
                    return;
                _mTinhTien_DichVuMoi_LuuTraTien = value;
                NotifyOfPropertyChange(() => mTinhTien_DichVuMoi_LuuTraTien);
            }
        }

        private bool _mTinhTien_Patient_TimBN = true;
        private bool _mTinhTien_Patient_ThemBN = true;
        private bool _mTinhTien_Patient_TimDangKy = true;

        private bool _mTinhTien_Info_CapNhatThongTinBN = true;
        private bool _mTinhTien_Info_XacNhan = true;
        private bool _mTinhTien_Info_XoaThe = true;
        private bool _mTinhTien_Info_XemPhongKham = true;

        public bool mTinhTien_Patient_TimBN
        {
            get
            {
                return _mTinhTien_Patient_TimBN;
            }
            set
            {
                if (_mTinhTien_Patient_TimBN == value)
                    return;
                _mTinhTien_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mTinhTien_Patient_TimBN);
            }
        }

        public bool mTinhTien_Patient_ThemBN
        {
            get
            {
                return _mTinhTien_Patient_ThemBN;
            }
            set
            {
                if (_mTinhTien_Patient_ThemBN == value)
                    return;
                _mTinhTien_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mTinhTien_Patient_ThemBN);
            }
        }

        public bool mTinhTien_Patient_TimDangKy
        {
            get
            {
                return _mTinhTien_Patient_TimDangKy;
            }
            set
            {
                if (_mTinhTien_Patient_TimDangKy == value)
                    return;
                _mTinhTien_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mTinhTien_Patient_TimDangKy);
            }
        }

        public bool mTinhTien_Info_CapNhatThongTinBN
        {
            get
            {
                return _mTinhTien_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mTinhTien_Info_CapNhatThongTinBN == value)
                    return;
                _mTinhTien_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mTinhTien_Info_CapNhatThongTinBN);
            }
        }

        public bool mTinhTien_Info_XacNhan
        {
            get
            {
                return _mTinhTien_Info_XacNhan;
            }
            set
            {
                if (_mTinhTien_Info_XacNhan == value)
                    return;
                _mTinhTien_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mTinhTien_Info_XacNhan);
            }
        }

        public bool mTinhTien_Info_XoaThe
        {
            get
            {
                return _mTinhTien_Info_XoaThe;
            }
            set
            {
                if (_mTinhTien_Info_XoaThe == value)
                    return;
                _mTinhTien_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mTinhTien_Info_XoaThe);
            }
        }

        public bool mTinhTien_Info_XemPhongKham
        {
            get
            {
                return _mTinhTien_Info_XemPhongKham;
            }
            set
            {
                if (_mTinhTien_Info_XemPhongKham == value)
                    return;
                _mTinhTien_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mTinhTien_Info_XemPhongKham);
            }
        }

        #endregion

        public void Handle(UpdateCompleted<PatientRegistration> message)
        {
            if (message.Item != null)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }
        public void OpenRegistration(long regID)
        {
            //RegistrationLoading = true;
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) => 
            { 
                //RegistrationLoading = false; 
                this.HideBusyIndicator();
            });
        }

        public bool IsFinalization
        {
            get { return RegistrationDetailsContent != null && RegistrationDetailsContent.IsFinalization; }
            set
            {
                if (RegistrationDetailsContent != null)
                {
                    RegistrationDetailsContent.IsFinalization = value;
                }
            }
        }
    }
}