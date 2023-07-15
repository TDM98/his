using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using aEMR.Common.BaseModel;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IPatientAccountTransaction)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientAccountTransactionViewModel: ViewModelBase, IPatientAccountTransaction
        , IHandle<ResultFound<Patient>>
        , IHandle<ItemSelected<Patient>>
    {
        [ImportingConstructor]
        public PatientAccountTransactionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_APPOINTMENT);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            searchPatientAndRegVm.mTimBN = true;
            searchPatientAndRegVm.mThemBN = true;

            searchPatientAndRegVm.PatientFindByVisibility = false;
            ((INotifyPropertyChangedEx)searchPatientAndRegVm).PropertyChanged += searchPatientAndRegVm_PropertyChanged;
            SearchRegistrationContent = searchPatientAndRegVm;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;

            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV3>();
            patientInfoVm.mInfo_CapNhatThongTinBN = true;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);


            var oldPaymentVm = Globals.GetViewModel<IPatientPayment>();
            OldPaymentContent = oldPaymentVm;
            ActivateItem(oldPaymentVm);

            AllPaymentReason = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PaymentReason).ToObservableCollection();

            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
            AllPaymentReason.Insert(0, firstItem);

            AllPaymentMode = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PAYMENT_MODE).ToObservableCollection();

            AllPaymentMode.Insert(0, firstItem);

            SetDefaultRem();
            ResetDefaultData();
            Globals.EventAggregator.Subscribe(this);
            authorization();

        }

        void searchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0113_G1_IsLoading || e.PropertyName == eHCMSResources.Z0114_G1_IsSearchingRegistration)
            {
                NotifyWhenBusy();
            }
        }

        #region Properties
        private ObservableCollection<Lookup> _AllPaymentReason;
        public ObservableCollection<Lookup> AllPaymentReason
        {
            get
            {
                return _AllPaymentReason;
            }
            set
            {
                if (_AllPaymentReason != value)
                {
                    _AllPaymentReason = value;
                    NotifyOfPropertyChange(() => AllPaymentReason);
                }
            }
        }

        private ObservableCollection<Lookup> _AllPaymentMode;
        public ObservableCollection<Lookup> AllPaymentMode
        {
            get
            {
                return _AllPaymentMode;
            }
            set
            {
                if (_AllPaymentMode != value)
                {
                    _AllPaymentMode = value;
                    NotifyOfPropertyChange(() => AllPaymentMode);
                }
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

        private IPatientSummaryInfoV3 _patientSummaryInfoContent;
        public IPatientSummaryInfoV3 PatientSummaryInfoContent
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
        private Patient _currentPatient;

        public Patient CurrentPatient
        {
            get { return _currentPatient; }
            set
            {
                _currentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
                NotifyOfPropertyChange(() => CanPayCmd);
            }
        }


        private PatientAccount _CurPatientAccount;
        public PatientAccount CurPatientAccount
        {
            get { return _CurPatientAccount; }
            set
            {
                _CurPatientAccount = value;
                NotifyOfPropertyChange(() => CurPatientAccount);
            }
        }


        private PatientAccountTransaction _SelectedPtAccountTransaction;
        public PatientAccountTransaction SelectedPtAccountTransaction
        {
            get { return _SelectedPtAccountTransaction; }
            set
            {
                _SelectedPtAccountTransaction = value;
                NotifyOfPropertyChange(() => SelectedPtAccountTransaction);
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

        private string _generalNote;
        public string GeneralNote
        {
            get { return _generalNote; }
            set
            {
                _generalNote = value;
                NotifyOfPropertyChange(() => GeneralNote);
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
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
                NotifyOfPropertyChange(() => TotalPatientPaid_Finalized);
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
                if (SearchRegistrationContent != null)
                {
                    SearchRegistrationContent.PatientFindBy = PatientFindBy;
                }
            }
        }

        private ObservableCollection<PatientAccountTransaction> _ObjPtAccTransaction;
        public ObservableCollection<PatientAccountTransaction> ObjPtAccTransaction
        {
            get
            {
                return _ObjPtAccTransaction;
            }
            set
            {
                if (_ObjPtAccTransaction != value)
                {
                    _ObjPtAccTransaction = value;
                    NotifyOfPropertyChange(() => ObjPtAccTransaction);
                }
            }
        }
        #endregion
        AxComboBox cbxPaymentReason { get; set; }
        public void cbxPaymentReason_Loaded(object sender, RoutedEventArgs e)
        {
            cbxPaymentReason = sender as AxComboBox;
            cbxPaymentReason.SelectedItem = AllPaymentReason.Where(x => x.LookupID == (long)AllLookupValues.V_PaymentReason.TAM_UNG_NOI_TRU).FirstOrDefault();
        }

        AxComboBox cbxPaymentMode { get; set; }
        public void cbxPaymentMode_Loaded(object sender, RoutedEventArgs e)
        {
            cbxPaymentMode = sender as AxComboBox;
            cbxPaymentMode.SelectedItem = AllPaymentMode.Where(x => x.LookupID == (long)AllLookupValues.PaymentMode.TIEN_MAT).FirstOrDefault();
        }

        private void PatientAccountTransaction_Insert(PatientAccountTransaction CashAdvance)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientAccountTransaction_Insert(CashAdvance,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    long ID = 0;
                                    var regItem = contract.EndPatientAccountTransaction_Insert(out ID, asyncResult);

                                    if (regItem)
                                    {
                                        MessageBox.Show(eHCMSResources.A0996_G1_Msg_InfoTUOK);
                                        ResetDefaultData();
                                        Coroutine.BeginExecute(ReloadData());
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MessageBox.Show(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.ToString());
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
                }

            });
            t.Start();
        }
        public void PayCmd()
        {
            if (!CanPayCmd)
            {
                return;
            }
            if (!PayAmount.HasValue || PayAmount.Value <= 0)
            {
                MessageBox.Show(eHCMSResources.A0604_G1_Msg_InfoNhapSoTien);
                return;
            }
            if (CurPatientAccount == null || CurPatientAccount.PatientAccountID == 0)
            {
                MessageBox.Show(eHCMSResources.Z2989_G1_ChonTKTamUng);
                return;
            }
            var payment = new PatientAccountTransaction
            {
                CreditAmount = PayAmount.Value,
                PatientAccountID = CurPatientAccount.PatientAccountID,
                Note = GeneralNote,
                TransactionDate = PaymentDate.GetValueOrDefault(Globals.GetCurServerDateTime()),
                Staff = Globals.LoggedUserAccount.Staff
            };


            if (cbxPaymentReason != null)
            {
                payment.V_PaymentReason = (Lookup)cbxPaymentReason.SelectedItem;
            }

            if (payment.V_PaymentReason == null || payment.V_PaymentReason.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0586_G1_Msg_InfoChonLoaiTU, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (cbxPaymentMode != null)
            {
                payment.V_PaymentMode = (Lookup)cbxPaymentMode.SelectedItem;
            }

            if (payment.V_PaymentMode.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0580_G1_Msg_InfoChonHTNop, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PatientAccountTransaction_Insert(payment);
        }

        public bool CanPayCmd
        {
            get
            {
                return CurrentPatient != null;
            }
        }


        public override bool IsProcessing
        {
            get
            {
                return _isLoadingPatient || _isLoadingRegistration;
            }
        }
        public override string StatusText
        {
            get
            {
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
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
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
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
                NotifyOfPropertyChange(() => TotalPatientPaid_Finalized);
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


        private void PatientAccountTransaction_GetAll(GenericCoRoutineTask genTask)
        {

            if (CurrentPatient == null || CurrentPatient.PatientID <= 0) 
            {
                MessageBox.Show(eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientAccountTransaction_GetAll(CurrentPatient.PatientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var regItem = contract.EndPatientAccountTransaction_GetAll(asyncResult);

                                if (regItem != null)
                                {
                                    ObjPtAccTransaction = regItem.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void SetDefaultRem()
        {
            if (ObjPatientAccount == null)
            {
                ObjPatientAccount = new ObservableCollection<PatientAccount>();
            }
            PatientAccount firstItem = new PatientAccount();
            firstItem.PatientAccountID = 0;
            firstItem.AccountNumber = string.Format(eHCMSResources.Z2990_G1_ChonTK);
            ObjPatientAccount.Insert(0, firstItem);
            if (ObjPatientAccount != null && ObjPatientAccount.Count > 0)
            {
                CurPatientAccount = ObjPatientAccount.FirstOrDefault();
            }
        }



        public void ResetDefaultData()
        {
            PaymentDate = Globals.GetCurServerDateTime();

            PayAmount = 0;
            if (AllPaymentReason != null && AllPaymentReason.Count > 0 && cbxPaymentReason != null)
            {
                cbxPaymentReason.SelectedItem = AllPaymentReason.Where(x => x.LookupID == (long)AllLookupValues.V_PaymentReason.TAM_UNG_NOI_TRU).FirstOrDefault();
            }

            if (AllPaymentMode != null && AllPaymentMode.Count > 0 && cbxPaymentMode != null)
            {
                cbxPaymentMode.SelectedItem = AllPaymentMode.Where(x => x.LookupID == (long)AllLookupValues.PaymentMode.TIEN_MAT).FirstOrDefault();
            }
        }

        private IEnumerator<IResult> ReloadData(bool LoadPatientAccount = false)
        {
            yield return GenericCoRoutineTask.StartTask(PatientAccount_GetAll);
            if (!LoadPatientAccount)
            {
                yield return GenericCoRoutineTask.StartTask(PatientAccountTransaction_GetAll);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
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

        #endregion

        public IEnumerator<IResult> PrintPatientCashAdvanceSilently(long PaymentID, int FindPatient)
        {
            yield return new PrintPatientCashAdvanceSilently(PaymentID, FindPatient);

        }

        private ObservableCollection<PatientAccount> _ObjPatientAccount;
        public ObservableCollection<PatientAccount> ObjPatientAccount
        {
            get { return _ObjPatientAccount; }
            set
            {
                _ObjPatientAccount = value;
                NotifyOfPropertyChange(() => ObjPatientAccount);
            }
        }

        public void cbxReminder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurPatientAccount == null)
            {
                return;
            }

            if (CurPatientAccount.PatientAccountID == 0)
            {
                if (AllPaymentReason != null)
                {
                    cbxPaymentReason.SelectedItem = AllPaymentReason.FirstOrDefault();
                }
                Coroutine.BeginExecute(ReloadData());
            }
        }


        public decimal BalanceCreditRemaining
        {
            get
            {
                //decimal calcBal = _sumOfAdvance - (_totalLiabilities + _TotalRefundMoney) + _TotalSupportFund;
                decimal calcBal = _TotalPatientPaid_NotFinalized + _TotalSupportFund_NotFinalized - _TotalPatientPayment_NotFinalized;
                if (tbTotBalCredit != null)
                {
                    if (calcBal >= 0)
                        tbTotBalCredit.Foreground = new SolidColorBrush(Colors.Black);
                    else
                        tbTotBalCredit.Foreground = new SolidColorBrush(Colors.Red);
                }
                return calcBal;
            }
        }


        private TextBlock tbTotBalCredit = null;
        public void TotalBalanceCredit_Loaded(object source)
        {
            if (source != null)
            {
                tbTotBalCredit = source as TextBlock;
            }
        }
        private decimal _TotalSupportFund;
        public decimal TotalSupportFund
        {
            get
            {
                return _TotalSupportFund;
            }
            set
            {
                _TotalSupportFund = value;
                NotifyOfPropertyChange(() => TotalSupportFund);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }

        private decimal _TotalPatientPayment_NotFinalized;
        public decimal TotalPatientPayment_NotFinalized
        {
            get
            {
                return _TotalPatientPayment_NotFinalized;
            }
            set
            {
                _TotalPatientPayment_NotFinalized = value;
                NotifyOfPropertyChange(() => TotalPatientPayment_NotFinalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }

        private decimal _TotalPatientPaid_NotFinalized;
        public decimal TotalPatientPaid_NotFinalized
        {
            get
            {
                return _TotalPatientPaid_NotFinalized;
            }
            set
            {
                _TotalPatientPaid_NotFinalized = value;
                NotifyOfPropertyChange(() => TotalPatientPaid_NotFinalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
                NotifyOfPropertyChange(() => TotalPatientPaid_Finalized);
            }
        }

        private decimal _TotalSupportFund_NotFinalized;
        public decimal TotalSupportFund_NotFinalized
        {
            get
            {
                return _TotalSupportFund_NotFinalized;
            }
            set
            {
                _TotalSupportFund_NotFinalized = value;
                NotifyOfPropertyChange(() => TotalSupportFund_NotFinalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }
        public decimal TotalPatientPaid_Finalized
        {
            get
            {
                return _sumOfAdvance - (_TotalPatientPaid_NotFinalized + _TotalRefundMoney);
            }
        }
        //public void btnRemCode_Click(PatientAccountTransaction aCashAdvance)
        //{
        //    if (aCashAdvance == null || aCashAdvance.RptPtCashAdvRemID.GetValueOrDefault(0) == 0)
        //    {
        //        return;
        //    }
        //    Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
        //    {
        //        proAlloc.ID = aCashAdvance.RptPtCashAdvRemID.Value;
        //        proAlloc.eItem = ReportName.PHIEUDENGHI_THANHTOAN;
        //    };
        //    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        //}

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    PatientSummaryInfoContent.CurrentPatient = CurrentPatient;
                    DoSetCurrentPatient(CurrentPatient);
                }
            }
        }
        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
                if (CurrentPatient != null)
                {
                    PatientSummaryInfoContent.CurrentPatient = CurrentPatient;
                    DoSetCurrentPatient(CurrentPatient);
                }
            }
        }

        public void DoSetCurrentPatient(Patient CurrentPatient)
        {
            
            Coroutine.BeginExecute(ReloadData());
        }

        private void PatientAccount_GetAll(GenericCoRoutineTask genTask)
        {

            if (CurrentPatient == null || CurrentPatient.PatientID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0478_G1_KhongTimThayBenhNhan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientAccount_GetAll(CurrentPatient.PatientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var regItem = contract.EndPatientAccount_GetAll(asyncResult);

                                if (regItem != null)
                                {
                                    ObjPatientAccount = regItem.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(bContinue);
                                }
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private string _AccountNumber;
        public string AccountNumber
        {
            get { return _AccountNumber; }
            set
            {
                _AccountNumber = value;
                NotifyOfPropertyChange(() => AccountNumber);
            }
        }
        public void RegistrationCmd()
        {
            if (CurrentPatient == null || CurrentPatient.PatientID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2987_G1_KhongCoTTKhongTheTaoTK);
                return;
            }
            if(string.IsNullOrEmpty(AccountNumber))
            {
                MessageBox.Show(eHCMSResources.Z2988_G1_NhapSoTKTamUng);
                return;
            }
            PatientAccount_Insert(CurrentPatient.PatientID, AccountNumber);
        }

        private void PatientAccount_Insert(long PatientID, string AccountNumber)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientAccount_Insert(PatientID, AccountNumber, 
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndPatientAccount_Insert(asyncResult);

                                    if (regItem)
                                    {
                                        MessageBox.Show(eHCMSResources.A0499_G1_Msg_InfoDKOK);
                                        ResetDefaultData();
                                        Coroutine.BeginExecute(ReloadData(regItem));
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MessageBox.Show(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.ToString());
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
                }

            });
            t.Start();
        }
    }
}


