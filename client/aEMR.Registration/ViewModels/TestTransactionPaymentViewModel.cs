using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading;
using System.Windows.Data;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using Caliburn.Micro;
using aEMR.ViewContracts;
using System.ComponentModel.Composition;
using DataEntities;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
using aEMR.Common.Collections;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ITestTransactionPayment))]
    public class TestTransactionPaymentViewModel : Conductor<object>, ITestTransactionPayment
                                                , IHandle<ItemSelected<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<PayForRegistrationCompleted>
        , IHandle<ResultFound<Patient>> 
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public TestTransactionPaymentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);

            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            var regDetailsVm = Globals.GetViewModel<IRegistrationSummaryV2>();
            RegistrationDetailsContent = regDetailsVm;
            ActivateItem(regDetailsVm);

            var oldPaymentVm = Globals.GetViewModel<IPatientPayment>();
            PaymentContent = oldPaymentVm;
            ActivateItem(oldPaymentVm);
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

        public IPatientPayment _paymentContent;

        public IPatientPayment PaymentContent
        {
            get { return _paymentContent; }
            set
            {
                _paymentContent = value;
                NotifyOfPropertyChange(() => PaymentContent);
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
                if (RegistrationDetailsContent.CurrentRegistration != null && RegistrationDetailsContent.CurrentRegistration.PatientTransaction != null)
                {
                    GetTransactionSum(RegistrationDetailsContent.CurrentRegistration.PatientTransaction.TransactionID);    
                }
                else
                {
                    if (curPatientTransactionDetail != null)
                    {
                        curPatientTransactionDetail.Clear();   
                    }
                }
                InitViewForPayments();
                PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
            }
        }

        private ObservableCollection<PatientTransactionDetail> _curPatientTransactionDetail;
        public ObservableCollection<PatientTransactionDetail> curPatientTransactionDetail
        {
            get
            {
                return _curPatientTransactionDetail;
            }
            set
            {
                if (_curPatientTransactionDetail == value)
                    return;
                _curPatientTransactionDetail = value;
                NotifyOfPropertyChange(()=>curPatientTransactionDetail);
            }
        }

        private PatientTransactionDetail _selectedTransactionDetail;
        public PatientTransactionDetail selectedTransactionDetail
        {
            get
            {
                return _selectedTransactionDetail;
            }
            set
            {
                if (_selectedTransactionDetail == value)
                    return;
                _selectedTransactionDetail = value;
                NotifyOfPropertyChange(() => selectedTransactionDetail);
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
                NotifyOfPropertyChange(()=>IsPaying);
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
            }
        }
        private void GetPatientByID(long patientID)
        {
            var t = new Thread(() =>
                                   {
                                       PatientLoading = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = eHCMSResources.Z0119_G1_DangLayTTinBN
                });

                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientByID(patientID, false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var patient = contract.EndGetPatientByID(asyncResult);
                                    CurrentPatient = patient;

                                    PatientLoading = false;
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
                    Globals.IsBusy = false;
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
                    CurRegistration = null;
                    Globals.ShowMessage(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN, eHCMSResources.T0432_G1_Error);
                }
            }
            else
            {
                CurRegistration = null;
                Globals.ShowMessage(eHCMSResources.A0633_G1_Msg_InfoKhongCoDKBN, eHCMSResources.T0432_G1_Error);
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
            }
        }
        /// <summary>
        /// Mở đăng ký đã có sẵn
        /// </summary>
        /// <param name="regID">ID của đăng ký</param>
        public void OpenRegistration(long regID)
        {
            var t = new Thread(() =>
            {
                RegistrationLoading = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0086_G1_DangLayTTinDK)
                });

                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRegistrationInfo(regID,FindPatient,false,false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndGetRegistrationInfo(asyncResult);
                                    RegistrationLoading = false;
                                    CurRegistration = regInfo;
                                    if (_currentPatient == null || CurRegistration.PatientID != CurrentPatient.PatientID)
                                    {
                                        _currentPatient = CurRegistration.Patient;
                                        NotifyOfPropertyChange(()=>CurrentPatient);
                                    }
                                    if (PatientSummaryInfoContent != null)
                                    {
                                        PatientSummaryInfoContent.CurrentPatient = _currentPatient;
                                    }
                                    ConfirmedHiItem = CurRegistration.HealthInsurance;
                                    ConfirmedPaperReferal = CurRegistration.PaperReferal;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    CurRegistration = null;
                                    error = new AxErrorEventArgs(fault);
                                }
                                catch (Exception ex)
                                {
                                    CurRegistration = null;
                                    error = new AxErrorEventArgs(ex);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    CurRegistration = null;
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void GetTransactionSum(long TransactionID)
        {
            var t = new Thread(() =>
            {
                RegistrationLoading = true;
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0188_G1_DangLayTTinTransaction)
                });

                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetTransactionSum(TransactionID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetTransactionSum(asyncResult);
                                    //
                                    if (curPatientTransactionDetail == null)
                                    {
                                        curPatientTransactionDetail = new ObservableCollection<PatientTransactionDetail>();
                                    }
                                    else
                                    {
                                        curPatientTransactionDetail.Clear();
                                    }
                                    foreach (var p in results)
                                    {
                                        curPatientTransactionDetail.Add(p);
                                    }

                                    NotifyOfPropertyChange(() => curPatientTransactionDetail);
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
                    CurRegistration = null;
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }
        public void OldRegistrationsCmd()
        {
            Action<IRegistrationList> onInitDlg = delegate (IRegistrationList vm)
            {
                vm.CurrentPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg);
        }

        private void InitViewForPayments()
        {
            List<PatientTransactionPayment> patientPayments = new List<PatientTransactionPayment>();

            if (CurRegistration != null && CurRegistration.PatientTransaction != null)
            {
                if (CurRegistration.PatientTransaction.PatientTransactionPayments != null)
                {
                    foreach (var item in CurRegistration.PatientTransaction.PatientTransactionPayments)
                    {
                        patientPayments.Add(item);
                    }
                }
            }

            PaymentContent.PatientPayments = patientPayments.ToObservableCollection();
        }
        public void Handle(PayForRegistrationCompleted message)
        {
            if (message != null)
            {
                //Load lai dang ky:
                var payment = message.Payment;
                if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                {
                    //Show Report:
                    Action<IPaymentReport> onInitDlg = delegate (IPaymentReport reportVm)
                    {
                        reportVm.PaymentID = payment.PtTranPaymtID;
                    };
                    GlobalsNAV.ShowDialog<IPaymentReport>(onInitDlg);

                    FindPatient = CurRegistration.FindPatient;
                    
                    OpenRegistration(message.Registration.PtRegistrationID);
                }
            }
        }

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
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
            if (message != null && message.Item != null)
            {
                FindPatient = message.Item.FindPatient;
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }
    }
}