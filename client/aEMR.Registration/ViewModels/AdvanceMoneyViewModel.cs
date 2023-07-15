using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IAdvanceMoney)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdvanceMoneyViewModel : ViewModelBase, IAdvanceMoney
                                       , IHandle<ItemSelected<PatientRegistration>>
    {
        [ImportingConstructor]
        public AdvanceMoneyViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.mTimDangKy = mTamUng_TimDangKy;

            searchPatientAndRegVm.PatientFindByVisibility = false;
            ((INotifyPropertyChangedEx)searchPatientAndRegVm).PropertyChanged += searchPatientAndRegVm_PropertyChanged;
            SearchRegistrationContent = searchPatientAndRegVm;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = false;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            var oldPaymentVm = Globals.GetViewModel<IPatientPayment>();
            OldPaymentContent = oldPaymentVm;
            ActivateItem(oldPaymentVm);

            var TTPaymentVm = Globals.GetViewModel<IPatientPayment>();
            TTPaymentContent = TTPaymentVm;
            ActivateItem(TTPaymentVm);

            PaymentReasonContent = Globals.GetViewModel<IEnumListing>();
            PaymentReasonContent.EnumType = typeof(AllLookupValues.V_PaymentReason);
            PaymentReasonContent.AddSelectOneItem = true;
            PaymentReasonContent.LoadData();

            Coroutine.BeginExecute(LoadPaymentModes());

            Globals.EventAggregator.Subscribe(this);
            authorization();

            ResetPatientPayment();
        }

        void searchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0113_G1_IsLoading || e.PropertyName == eHCMSResources.Z0114_G1_IsSearchingRegistration)
            {
                NotifyWhenBusy();
            }
        }

        private PatientTransactionPayment _currentPayment;
        public PatientTransactionPayment CurrentPayment
        {
            get
            {
                return _currentPayment;
            }
            set
            {
                _currentPayment = value;
                NotifyOfPropertyChange(() => CurrentPayment);
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

        private bool _isDischarged;
        public bool isDischarged
        {
            get { return _isDischarged; }
            set
            {
                _isDischarged = value;
                NotifyOfPropertyChange(() => isDischarged);
                isChangeDept = !isDischarged;
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
        private IPatientPayment _oldPaymentContent;
        public IPatientPayment OldPaymentContent
        {
            get { return _oldPaymentContent; }
            set
            {
                _oldPaymentContent = value;
                NotifyOfPropertyChange(() => OldPaymentContent);
            }
        }

        private IPatientPayment _TTPaymentContent;
        public IPatientPayment TTPaymentContent
        {
            get { return _TTPaymentContent; }
            set
            {
                _TTPaymentContent = value;
                NotifyOfPropertyChange(() => TTPaymentContent);
            }
        }

        private IEnumListing _paymentReasonContent;

        public IEnumListing PaymentReasonContent
        {
            get { return _paymentReasonContent; }
            set
            {
                _paymentReasonContent = value;
                NotifyOfPropertyChange(() => PaymentReasonContent);
            }
        }

        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }


        private ObservableCollection<Lookup> _paymentModeList;
        public ObservableCollection<Lookup> PaymentModeList
        {
            get { return _paymentModeList; }
            set
            {
                _paymentModeList = value;
                NotifyOfPropertyChange(() => PaymentModeList);
            }
        }

        private RptPatientCashAdvReminder _CurRptPatientCashAdvReminder;
        public RptPatientCashAdvReminder CurRptPatientCashAdvReminder
        {
            get { return _CurRptPatientCashAdvReminder; }
            set
            {
                _CurRptPatientCashAdvReminder = value;
                NotifyOfPropertyChange(() => CurRptPatientCashAdvReminder);
            }
        }


        private RptPatientCashAdvReminder _SgtRptPatientCashAdvReminder;
        public RptPatientCashAdvReminder SgtRptPatientCashAdvReminder
        {
            get { return _SgtRptPatientCashAdvReminder; }
            set
            {
                _SgtRptPatientCashAdvReminder = value;
                NotifyOfPropertyChange(() => SgtRptPatientCashAdvReminder);
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
                DisplayRegistrationInfo();
                //if (CurRegistration.AdmissionInfo!=null
                //    && CurRegistration.AdmissionInfo.DischargeDate != null)
                //{
                //    isDischarged = true;
                //    NotifyOfPropertyChange(() => isDischarged);
                //}
                //else
                //{
                //    isDischarged = false;
                //    NotifyOfPropertyChange(() => isDischarged);
                //}
            }
        }

        private decimal? _payAmount;
        public decimal? PayAmount
        {
            get { return _payAmount; }
            set
            {
                _payAmount = value;
                NotifyOfPropertyChange(() => PayAmount);
            }
        }

        private decimal _TotalRefundMoney;
        public decimal TotalRefundMoney
        {
            get { return _TotalRefundMoney; }
            set
            {
                _TotalRefundMoney = value;
                NotifyOfPropertyChange(() => TotalRefundMoney);
                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }

        /// <summary>
        /// Tổng công nợ (Tính tổng tiền của những bill chưa tính tiền).
        /// </summary>
        public decimal DebtRemaining
        {
            get
            {
                return (_totalLiabilities + _TotalRefundMoney) - _sumOfAdvance;
            }
        }


        private DateTime? _paymentDate = Globals.GetCurServerDateTime();
        public DateTime? PaymentDate
        {
            get { return _paymentDate; }
            set
            {
                _paymentDate = value;
                NotifyOfPropertyChange(() => PaymentDate);
            }
        }

        private bool _isLoadingPatient;

        public bool IsLoadingPatient
        {
            get { return _isLoadingPatient; }
            set
            {
                _isLoadingPatient = value;
                NotifyOfPropertyChange(() => IsLoadingPatient);
                NotifyWhenBusy();
            }
        }

        private bool _isLoadingRegistration;
        public bool IsLoadingRegistration
        {
            get { return _isLoadingRegistration; }
            set
            {
                _isLoadingRegistration = value;
                NotifyOfPropertyChange(() => IsLoadingRegistration);
                NotifyWhenBusy();
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
                NotifyOfPropertyChange(() => CanPayCmd);
            }
        }

        private void ResetPatientPayment()
        {
            CurrentPayment = new PatientTransactionPayment();
            CurrentPayment.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentPayment.PaymentMode = new Lookup() { LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT };
            CurrentPayment.PaymentType = new Lookup() { LookupID = (long)AllLookupValues.PaymentType.HOAN_TIEN };
            CurrentPayment.Currency = new Lookup() { LookupID = (long)AllLookupValues.Currency.VND };
            CurrentPayment.PtPmtAccID = 3;//benh vien tra tien lai cho benh nhan
            CurrentPayment.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
        }

        private IEnumerator<IResult> LoadPaymentModes()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE);
            yield return paymentModeTask;
            PaymentModeList = paymentModeTask.LookupList;
            yield break;
        }

        public void PayCmd()
        {
            if (!CanPayCmd)
            {
                return;
            }
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.T0774_G1_TU.ToLower()))
            {
                return;
            }
            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                MessageBox.Show(eHCMSResources.Z0275_G1_KgTheTUChoDKDaBiHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!PayAmount.HasValue || PayAmount.Value <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0276_G1_HayNhapSoTien));
                return;
            }
            string GeneralNote = string.Empty;
            if (PaymentReasonContent.SelectedItem != null)
            {
                if (PaymentReasonContent.SelectedItem.EnumItem != null)
                {
                    GeneralNote = PaymentReasonContent.SelectedItem.Description;
                }

            }
            var payment = new PatientCashAdvance
                              {
                                  PaymentAmount = PayAmount.Value,
                                  GeneralNote = GeneralNote,
                                  PtRegistrationID = CurRegistration.PtRegistrationID,
                                  V_RegistrationType = CurRegistration.V_RegistrationType,
                                  PaymentDate = PaymentDate.GetValueOrDefault(DateTime.Now),
                                  StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                                  RptPtCashAdvRemID = CurRptPatientCashAdvReminder != null ? CurRptPatientCashAdvReminder.RptPtCashAdvRemID : 0,
                              };

			PatientCashAdvance_Add(payment, PatientSummaryInfoContent.CurrentPatient.PatientID);
		}

        public bool CanPayCmd
        {
            get
            {
                return CurRegistration != null && !IsPaying;
            }
        }
        PatientTransactionPayment _tempPayment;
        private IEnumerator<IResult> DoPayForRegistration(PatientTransactionPayment patientPayment)
        {
            _tempPayment = null;

            var payTask = new PayForInPatientRegistrationTask(CurRegistration, patientPayment, null, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());

            yield return payTask;
            if (payTask.Error == null)
            {
                if (payTask.PatientPayment != null)
                {
                    payTask.PatientPayment.PatientTransaction = payTask.PatientTransaction;
                    _tempPayment = payTask.PatientPayment;
                }
                _msgTask = new MessageBoxTask(eHCMSResources.A0996_G1_Msg_InfoTUOK, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;

            }
            _msgTask = new MessageBoxTask(string.Format("{0}. {1}", eHCMSResources.Z0117_G1_CoLoiXayRa, payTask.Error), eHCMSResources.G0442_G1_TBao);
            yield return _msgTask;
            yield break;
        }

        public override bool IsProcessing
        {
            get
            {
                return _isPaying || _isLoadingPatient || _isLoadingRegistration;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isPaying)
                {
                    return eHCMSResources.Z0118_G1_DangTinhTien;
                }
                if (_isLoadingPatient)
                {
                    return eHCMSResources.Z0119_G1_DangLayTTinBN;
                }
                if (_isLoadingRegistration)
                {
                    return eHCMSResources.Z0086_G1_DangLayTTinDK;
                }
                return "";
            }
        }

        private decimal _totalLiabilities;
        public decimal TotalLiabilities
        {
            get { return _totalLiabilities; }
            set
            {
                _totalLiabilities = value;
                NotifyOfPropertyChange(() => TotalLiabilities);
                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }


        private decimal _sumOfAdvance;
        public decimal SumOfAdvance
        {
            get { return _sumOfAdvance; }
            set
            {
                _sumOfAdvance = value;
                NotifyOfPropertyChange(() => SumOfAdvance);
                NotifyOfPropertyChange(() => DebtRemaining);
            }
        }

        private decimal _totalPatientPaymentPaidInvoice;
        public decimal TotalPatientPaymentPaidInvoice
        {
            get { return _totalPatientPaymentPaidInvoice; }
            set
            {
                _totalPatientPaymentPaidInvoice = value;
                NotifyOfPropertyChange(() => TotalPatientPaymentPaidInvoice);
            }
        }

        public void SetRegistration(object registrationInfo)
        {

        }

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (GetView() != null)
            {
                IsLoadingRegistration = true;
                TotalLiabilities = 0;
                SumOfAdvance = 0;
                TotalRefundMoney = 0;
                TotalPatientPaymentPaidInvoice = 0;
                Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID), null, (o, e) =>
                {
                    IsLoadingRegistration = false;
                });
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
                }
            }
        }

        MessageBoxTask _msgTask;

        public PatientClassification CurClassification
        {
            get
            {
                return PatientSummaryInfoContent.CurrentPatientClassification;
            }
            set
            {
                PatientSummaryInfoContent.CurrentPatientClassification = value;
            }
        }
        private void DisplayRegistrationInfo()
        {
            InitViewForPayments();
            if (CurRegistration == null)
            {
                PatientSummaryInfoContent.SetPatientHISumInfo(null);
                PatientSummaryInfoContent.CurrentPatientClassification = null;
                return;
            }
            PayAmount = 0;
            //Chuyen sang mode giong nhu mo lai dang ky cu
            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            //InitRegistration();

            if (PatientSummaryInfoContent != null)
            {                
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;

                PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);                
            }
            if (CurRegistration.PatientClassification == null && CurRegistration.PatientClassID.HasValue)
            {
                CurClassification = PatientClassification.CreatePatientClassification(CurRegistration.PatientClassID.Value, "");
            }
            else
            {
                CurClassification = CurRegistration.PatientClassification;
            }
        }

        private void InitViewForPayments()
        {
            ObservableCollection<PatientTransactionPayment> patientPayments = null;
            ObservableCollection<PatientTransactionPayment> TTpatientPayments = null;

            if (CurRegistration != null && CurRegistration.PatientTransaction != null)
            {
                if (CurRegistration.PatientTransaction.PatientTransactionPayments != null)
                {
                    patientPayments = CurRegistration.PatientTransaction.PatientTransactionPayments.Where(x => x.PtPmtAccID != 3).ToObservableCollection();
                    TTpatientPayments = CurRegistration.PatientTransaction.PatientTransactionPayments.Where(x => x.PtPmtAccID == 3).ToObservableCollection();
                }
            }
            if (patientPayments == null)
            {
                patientPayments = new ObservableCollection<PatientTransactionPayment>();
            }
            OldPaymentContent.PatientPayments = patientPayments;

            if (TTpatientPayments == null)
            {
                TTpatientPayments = new ObservableCollection<PatientTransactionPayment>();
            }
            TTPaymentContent.PatientPayments = TTpatientPayments;
        }

        private void PatientCashAdvance_Add(PatientCashAdvance CashAdvance, long patientId)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientCashAdvance_Insert(CashAdvance, patientId,
							Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    long ID = 0;
                                    var regItem = contract.EndPatientCashAdvance_Insert(out ID, out string msg, asyncResult);
                                    if (regItem)
                                    {
                                        //goi report in ra 
                                        MessageBox.Show(eHCMSResources.A0996_G1_Msg_InfoTUOK);
                                        PayAmount = 0;

                                        Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(ID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
                                        PatientCashAdvance_GetAll();
                                        CurRptPatientCashAdvReminder = null;
                                    }
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        Globals.ShowMessage(msg, "[CẢNH BÁO]");
                                    }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        private void PatientCashAdvance_GetAll()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientCashAdvance_GetAll(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndPatientCashAdvance_GetAll(asyncResult);

                                    if (regItem != null)
                                    {
                                        CurRegistration.PatientCashAdvances = regItem.ToObservableCollection();
                                        if (CurRegistration.PatientCashAdvances != null)
                                        {
                                            decimal sumOfAdvance_TU = 0;
                                            foreach (var item in CurRegistration.PatientCashAdvances)
                                            {
                                                sumOfAdvance_TU += item.BalanceAmount;
                                            }
                                            SumOfAdvance = sumOfAdvance + sumOfAdvance_TU;
                                        }
                                        GetRptPatientCashAdvReminders();
                                    }

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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });
            t.Start();
        }

        private decimal sumOfAdvance = 0;

        private IEnumerator<IResult> LoadRegistrationByID(long registrationID)
        {
            var TargetTask = new GetInPatientRegistrationAndPaymentInfoTask(registrationID);
            yield return TargetTask;
            if (TargetTask.CurRegistration != null)
            {
                CurRegistration = TargetTask.CurRegistration;
                TotalLiabilities = TargetTask.totalLiabilities;
                TotalRefundMoney = TargetTask.TotalRefundPatient;
                TotalPatientPaymentPaidInvoice = TargetTask.totalPatientPaymentPaidInvoice;
                SumOfAdvance = TargetTask.sumOfAdvance;
                GetRptPatientCashAdvReminders();
            }
            yield break;

        }

        private void GetRptPatientCashAdvReminders()
        {
            //RptPatientCashAdvReminders = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG && CurRegistration.PatientCashAdvances != null && !CurRegistration.PatientCashAdvances.Any(t2 => t2.RptPtCashAdvRemID == x.RptPtCashAdvRemID)).ToObservableCollection();
            RptPatientCashAdvReminders = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG && x.Checked == false).ToObservableCollection();
            RptPatientCashAdvReminderSgts = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_THANH_TOAN && x.Checked == false).ToObservableCollection();
        }

  

        private IEnumerator<IResult> OpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetPatientTransactions = true;

            var regInfo = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.PatientFindBy.NOITRU, LoadRegisSwitch);
            yield return regInfo;
            CurrentPatient = regInfo.Registration.Patient;
            PatientSummaryInfoContent.CurrentPatient = _currentPatient;

            if (regInfo.Registration.AdmissionInfo != null)
            {
                if (regInfo.Registration.AdmissionInfo.DischargeDate != null)
                {
                    isDischarged = true;
                    NotifyOfPropertyChange(() => isDischarged);
                }
                else
                {
                    isDischarged = false;
                    NotifyOfPropertyChange(() => isDischarged);
                }

                var TargetTask = new GetInPatientRegistrationAndPaymentInfoTask(regInfo.Registration.PtRegistrationID);
                yield return TargetTask;
                if (TargetTask.CurRegistration != null)
                {
                    CurRegistration = TargetTask.CurRegistration;
                    TotalLiabilities = TargetTask.totalLiabilities;
                    TotalRefundMoney = TargetTask.TotalRefundPatient;
                    TotalPatientPaymentPaidInvoice = TargetTask.totalPatientPaymentPaidInvoice;
                    SumOfAdvance = TargetTask.sumOfAdvance;
                    GetRptPatientCashAdvReminders();
                }
            }
            yield break;
        }


        public void ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail,
            long PtRegistrationID, long patientId, long V_RegistrationType)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0086_G1_DangLayTTinDK)
                });
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginThanhToanTienChoBenhNhan(payment, TrDetail, PtRegistrationID, patientId, V_RegistrationType,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long PtTranPmtID = 0;
                                    var regInfo = contract.EndThanhToanTienChoBenhNhan(out PtTranPmtID, out string msg, asyncResult);
                                    if (PtTranPmtID > 0)
                                    {
                                        Globals.ShowMessage(eHCMSResources.Z0366_G1_TToanThanhCong, eHCMSResources.G0442_G1_TBao);
                                       Coroutine.BeginExecute(LoadRegistrationByID(CurRegistration.PtRegistrationID));
                                        //goi ham in tu dong
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0367_G1_SoTienHoanLaiLonHonSoTienConLai);
                                    }
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        Globals.ShowMessage(msg, "[CẢNH BÁO]");
                                    }
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

            mTamUng_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mAdvanceMoney,
                                               (int)oRegistrionEx.mTamUng_TimDangKy, (int)ePermission.mView);

            mTamUng_TamUng = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mAdvanceMoney,
                                               (int)oRegistrionEx.mTamUng_TamUng, (int)ePermission.mView);

        }
        #region checking account

        private bool _mTamUng_TimDangKy = true;
        private bool _mTamUng_TamUng = true;


        public bool mTamUng_TimDangKy
        {
            get
            {
                return _mTamUng_TimDangKy;
            }
            set
            {
                if (_mTamUng_TimDangKy == value)
                    return;
                _mTamUng_TimDangKy = value;
                NotifyOfPropertyChange(() => mTamUng_TimDangKy);
            }
        }


        public bool mTamUng_TamUng
        {
            get
            {
                return _mTamUng_TamUng;
            }
            set
            {
                if (_mTamUng_TamUng == value)
                    return;
                _mTamUng_TamUng = value;
                NotifyOfPropertyChange(() => mTamUng_TamUng);
            }
        }



        //phan nay nam trong module chung

        #endregion

        public void lnkPrint_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                PatientCashAdvance test = elem.DataContext as PatientCashAdvance;
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.PaymentID = test.PtCashAdvanceID;
                    proAlloc.FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                    proAlloc.eItem = ReportName.PATIENTCASHADVANCE_REPORT;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                PatientCashAdvance test = elem.DataContext as PatientCashAdvance;
                Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(test.PtCashAdvanceID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
            }
        }
        public IEnumerator<IResult> PrintPatientCashAdvanceSilently(long PaymentID, int FindPatient)
        {
            yield return new PrintPatientCashAdvanceSilently(PaymentID, FindPatient);

        }

        private PatientTransactionDetail _CurPatientTransactionDetail;

        public void btnRefundMoney()
        {
            if (CurrentPayment != null && CurrentPayment.PayAmount > 0)
            {
               
                _CurPatientTransactionDetail = new PatientTransactionDetail();
                _CurPatientTransactionDetail.StaffID = Globals.LoggedUserAccount.StaffID;
                _CurPatientTransactionDetail.Amount = CurrentPayment.PayAmount;
                _CurPatientTransactionDetail.TranRefID = SgtRptPatientCashAdvReminder != null ? SgtRptPatientCashAdvReminder.RptPtCashAdvRemID : 0;
                _CurPatientTransactionDetail.V_TranRefType = AllLookupValues.V_TranRefType.BILL_THANH_TOAN;

                ThanhToanTienChoBenhNhan(CurrentPayment, _CurPatientTransactionDetail, CurRegistration.PtRegistrationID,
                    CurRegistration.Patient.PatientID, (long)CurRegistration.V_RegistrationType);
            }
        }

        private ObservableCollection<RptPatientCashAdvReminder> _RptPatientCashAdvReminders;
        public ObservableCollection<RptPatientCashAdvReminder> RptPatientCashAdvReminders
        {
            get { return _RptPatientCashAdvReminders; }
            set
            {
                _RptPatientCashAdvReminders = value;
                NotifyOfPropertyChange(() => RptPatientCashAdvReminders);
            }
        }

        private ObservableCollection<RptPatientCashAdvReminder> _RptPatientCashAdvReminderSgts;
        public ObservableCollection<RptPatientCashAdvReminder> RptPatientCashAdvReminderSgts
        {
            get { return _RptPatientCashAdvReminderSgts; }
            set
            {
                _RptPatientCashAdvReminderSgts = value;
                NotifyOfPropertyChange(() => RptPatientCashAdvReminderSgts);
            }
        }
    }
}