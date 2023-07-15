using System;
using System.ComponentModel.Composition;
using aEMR.Infrastructure;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using Castle.Windsor;
using System.Collections.ObjectModel;
using System.Threading;
using aEMR.Infrastructure.Events;
using eHCMSLanguage;
using aEMR.ServiceClient;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Windows;
using aEMR.Common;
using System.Windows.Controls;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common.Utilities;
using System.ServiceModel;
using aEMR.DataContracts;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IOutPatientSettlement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class OutPatientSettlementViewModel: Conductor<object>, IOutPatientSettlement
        , IHandle<ResultFound<Patient>>
        , IHandle<ItemSelected<Patient>>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public OutPatientSettlementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_APPOINTMENT);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            searchPatientAndRegVm.mTimBN = true;
            searchPatientAndRegVm.mThemBN = true;

            searchPatientAndRegVm.PatientFindByVisibility = false;
            SearchRegistrationContent = searchPatientAndRegVm;
            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV3>();
            patientInfoVm.mInfo_CapNhatThongTinBN = true;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);
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

        private bool _isDischarged;
        public bool isDischarged
        {
            get { return _isDischarged; }
            set
            {
                _isDischarged = value;
                NotifyOfPropertyChange(() => isDischarged);
            }
        }


        public bool IsProcessing
        {
            get
            {
                return true;
            }
        }

        private bool _CanPay;
        public bool CanPay
        {
            get
            {
                return _CanPay;
            }
            set
            {
                if (_CanPay != value)
                {
                    _CanPay = value;
                    NotifyOfPropertyChange(() => CanPay);
                }
            }
        }
        private bool _canChangePatientType = false;
        public bool CanChangePatientType
        {
            get
            {
                return _canChangePatientType;
            }
            set
            {
                if (_canChangePatientType != value)
                {
                    _canChangePatientType = value;
                    NotifyOfPropertyChange(() => CanChangePatientType);
                }
            }
        }
        private bool _canSaveRegistrationAndPay;
        public bool CanSaveRegistrationAndPay
        {
            get
            {
                return _canSaveRegistrationAndPay;
            }
            set
            {
                if (_canSaveRegistrationAndPay != value)
                {
                    _canSaveRegistrationAndPay = value;
                    NotifyOfPropertyChange(() => CanSaveRegistrationAndPay);
                }
            }
        }

        private bool _canSearchPatient = true;
        public bool CanSearchPatient
        {
            get
            {
                return _canSearchPatient;
            }
            set
            {
                if (_canSearchPatient != value)
                {
                    _canSearchPatient = value;
                    NotifyOfPropertyChange(() => CanSearchPatient);
                }
            }
        }

        private void NotifyButtonBehaviourChanges()
        {
            CanPay = !IsProcessing && CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED;

            CanSaveRegistrationAndPay = !IsProcessing;
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
                NotifyOfPropertyChange(() => CurClassification);
                NotifyOfPropertyChange(() => HiServiceBeingUsed);
            }
        }
        public bool HiServiceBeingUsed
        {
            get
            {
                if (CurClassification == null)
                {
                    return false;
                }
                return CurClassification.PatientType == PatientType.INSUARED_PATIENT;
            }
        }
        private HealthInsurance _confirmedHiItem;

        public HealthInsurance ConfirmedHiItem
        {
            get
            {
                return _confirmedHiItem;
            }
            set
            {
                _confirmedHiItem = value;
                NotifyOfPropertyChange(() => ConfirmedHiItem);

                CurClassification = CreateDefaultClassification();
            }
        }

        private Patient _CurrentPatient;
        public Patient CurrentPatient
        {
            get
            {
                return _CurrentPatient;
            }
            set
            {
                _CurrentPatient = value;
                NotifyOfPropertyChange(() => CurrentPatient);
            }
        }
        private bool _canRegister;
        public bool CanRegister
        {
            get
            {
                return _canRegister;
            }
            set
            {
                if (_canRegister != value)
                {
                    _canRegister = value;
                    NotifyOfPropertyChange(() => CanRegister);
                }
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
                    NotifyButtonBehaviourChanges();
                }
            }
        }

        private bool _SearchIncludeFinalizedBills = false;
        public bool SearchIncludeFinalizedBills
        {
            get { return _SearchIncludeFinalizedBills; }
            set
            {
                _SearchIncludeFinalizedBills = value;
                NotifyOfPropertyChange(() => SearchIncludeFinalizedBills);
            }
        }


        public void ResetPatientClassificationToDefaultValue()
        {
            CurClassification = CreateDefaultClassification();
        }
        private PatientClassification CreateDefaultClassification()
        {
            if (ConfirmedHiItem != null)
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.INSUARED_PATIENT, "");
            }
            else
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.NORMAL_PATIENT, "");
            }
        }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                NotifyOfPropertyChange(() => SelectedDate);
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
                NotifyOfPropertyChange(() => RegistrationDate);
            }
        }

        private ObservableCollection<PatientRegistration> _ListPatientRegistration;
        public ObservableCollection<PatientRegistration> ListPatientRegistration
        {
            get { return _ListPatientRegistration; }
            set
            {
                if (_ListPatientRegistration != value)
                {
                    _ListPatientRegistration = value;
                    NotifyOfPropertyChange(() => ListPatientRegistration);
                }
            }
        }

        private PatientRegistration _SelectedPatientRegistration;
        public PatientRegistration SelectedPatientRegistration
        {
            get { return _SelectedPatientRegistration; }
            set
            {
                if (_SelectedPatientRegistration != value)
                {
                    _SelectedPatientRegistration = value;
                    NotifyOfPropertyChange(() => SelectedPatientRegistration);
                }
            }
        }

        private ObservableCollection<PatientRegistration> _ListPatientRegistrationSettlement;
        public ObservableCollection<PatientRegistration> ListPatientRegistrationSettlement
        {
            get { return _ListPatientRegistrationSettlement; }
            set
            {
                if (_ListPatientRegistrationSettlement != value)
                {
                    _ListPatientRegistrationSettlement = value;
                    NotifyOfPropertyChange(() => ListPatientRegistrationSettlement);
                }
            }
        }
        private ObservableCollection<PatientRegistration> _SelectedPatientRegistrationSettlement;
        public ObservableCollection<PatientRegistration> SelectedPatientRegistrationSettlement
        {
            get { return _SelectedPatientRegistrationSettlement; }
            set
            {
                if (_SelectedPatientRegistrationSettlement != value)
                {
                    _SelectedPatientRegistrationSettlement = value;
                    NotifyOfPropertyChange(() => SelectedPatientRegistrationSettlement);
                }
            }
        }
        private ObservableCollection<TransactionFinalization> _patientSettlementList;
        public ObservableCollection<TransactionFinalization> PatientSettlementList
        {
            get { return _patientSettlementList; }
            set
            {
                if (_patientSettlementList != value)
                {
                    _patientSettlementList = value;
                    NotifyOfPropertyChange(() => PatientSettlementList);
                }
            }
        }
        private ObservableCollection<PatientAccountTransaction> _ListPtAccountTran;
        public ObservableCollection<PatientAccountTransaction> ListPtAccountTran
        {
            get { return _ListPtAccountTran; }
            set
            {
                if (_ListPtAccountTran != value)
                {
                    _ListPtAccountTran = value;
                    NotifyOfPropertyChange(() => ListPtAccountTran);
                }
            }
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
        private decimal _TotalAdvPayment;
        public decimal TotalAdvPayment
        {
            get { return _TotalAdvPayment; }
            set
            {
                if (_TotalAdvPayment != value)
                {
                    _TotalAdvPayment = value;
                    NotifyOfPropertyChange(() => TotalAdvPayment);
                }
            }
        }
        private decimal _TotalPayment;
        public decimal TotalPayment
        {
            get { return _TotalPayment; }
            set
            {
                if (_TotalPayment != value)
                {
                    _TotalPayment = value;
                    NotifyOfPropertyChange(() => TotalPayment);
                }
            }
        }

        #region Method
        private void GetAllRegistrationForSettlement(GenericCoRoutineTask genTask)
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
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRegistrationForSettlement(CurrentPatient.PatientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allPatientRegistration = contract.EndGetAllRegistrationForSettlement(asyncResult);
                                ListPatientRegistration = new ObservableCollection<PatientRegistration>();
                                ListPatientRegistrationSettlement = new ObservableCollection<PatientRegistration>();
                                if (allPatientRegistration != null)
                                {
                                    //ListPatientRegistration = new ObservableCollection<PatientRegistration>(allPatientRegistration);
                                    foreach(var items in allPatientRegistration)
                                    {
                                        if (!items.IsSettlement && items.TotalPatientPaymentForSettlement > 0)
                                        {
                                            ListPatientRegistration.Add(items);
                                        }
                                        else
                                        {
                                            ListPatientRegistrationSettlement.Add(items);
                                        }
                                    }
                                    if(ListPatientRegistration.Count > 0)
                                    {
                                        TotalPayment = ListPatientRegistration.Where(x => x.PtRegistrationID > 0 && !x.IsSettlement && x.TotalPatientPaymentForSettlement > 0).Sum(y => y.TotalPatientPaymentForSettlement);
                                    }
                                    else
                                    {
                                        TotalPayment = 0;
                                    }
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
                                    ListPtAccountTran = regItem.ToObservableCollection();
                                    if (ListPtAccountTran.Count > 0)
                                    {
                                        TotalAdvPayment = ListPtAccountTran.Where(x => x.PtAccountTranID > 0).Sum(y => y.CreditAmount);
                                    }
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
        public void DoSetCurrentPatient(Patient CurrentPatient)
        {
            Coroutine.BeginExecute(ReloadData());
        }

        private IEnumerator<IResult> ReloadData(bool LoadPatientAccount = false)
        {
            yield return GenericCoRoutineTask.StartTask(GetAllRegistrationForSettlement);
            yield return GenericCoRoutineTask.StartTask(PatientAccount_GetAll);
            yield return GenericCoRoutineTask.StartTask(PatientAccountTransaction_GetAll);
        }
        private IEnumerator<IResult> PaySetllement(bool LoadPatientAccount = false)
        {
            if ((TotalAdvPayment - TotalPayment) < 0)
            {
                var dialog = new MessageWarningShowDialogTask(string.Format(eHCMSResources.Z2557_G1_BNDongYThanhToanThem, Math.Ceiling(Math.Abs(TotalAdvPayment - TotalPayment)).ToString("#,#")), eHCMSResources.G2363_G1_XNhan);
                yield return dialog;
                if (!dialog.IsAccept)
                {
                    yield break;
                }
                yield return GenericCoRoutineTask.StartTask(PatientAccountTransaction_Insert);
            }
            yield return GenericCoRoutineTask.StartTask(btnSettlement, ListPatientRegistration);
        }
        public void ckbIsChecked_Click(object source, object sender)
        {
            CheckBox ckbIsChecked = source as CheckBox;
            bool? copier = ckbIsChecked.IsChecked;
            if (!(ckbIsChecked.DataContext is PatientRegistration))
            {
                ckbIsChecked.IsChecked = !copier;
                return;
            }
            PatientRegistration SelectedPtRegistration = ckbIsChecked.DataContext as PatientRegistration;
            if ((bool)ckbIsChecked.IsChecked)
            {
                SelectedPtRegistration.IsChecked = (bool)ckbIsChecked.IsChecked;
            }
        }
        public void PayCmd()
        {
            ObservableCollection<PatientRegistration> ListPatientRegistrationForSettlement = new ObservableCollection<PatientRegistration>();
            if(ListPatientRegistration == null || ListPatientRegistration.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2984_G1_KhongCoDKThoaQuyetToan);
                return;
            }
            if (ObjPatientAccount == null || ObjPatientAccount.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2985_G1_KhongTKTamUng);
                return;
            }
            foreach(var item in ListPatientRegistration)
            {
                if (item.IsChecked)
                {
                    ListPatientRegistrationForSettlement.Add(item);
                }
            }
            if (ListPatientRegistrationForSettlement.Count > 0)
            {

            }
            Coroutine.BeginExecute(PaySetllement());
        }

        private void PatientAccountTransaction_Insert(GenericCoRoutineTask genTask)
        {
            PatientAccountTransaction CashAdvance = new PatientAccountTransaction
            {
                CreditAmount = Math.Ceiling(Math.Abs(TotalPayment - TotalAdvPayment)),
                PatientAccountID = ObjPatientAccount.FirstOrDefault().PatientAccountID,
                Note = "",
                TransactionDate = Globals.GetCurServerDateTime(),
                Staff = Globals.LoggedUserAccount.Staff,
            };
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
                                bool bContinue = true;
                                try
                                {
                                    long ID = 0;
                                    var regItem = contract.EndPatientAccountTransaction_Insert(out ID, asyncResult);

                                    if (regItem)
                                    {
                                        Coroutine.BeginExecute(ReloadData());
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MessageBox.Show(fault.ToString());
                                    bContinue = false;
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.ToString());
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
        public void btnSettlement(GenericCoRoutineTask genTask, object ListPtRegistration)
        {
            List<PatientRegistration> ListPtRegistrationForSettlement = new List<PatientRegistration>(ListPtRegistration as ObservableCollection<PatientRegistration>);
            var mThread = new Thread(() =>
            {
                try
                {
                    this.ShowBusyIndicator();
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDoSettlementForListOutPatient(ListPtRegistrationForSettlement, CurrentPatient.PatientID, (long)Globals.LoggedUserAccount.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool IsOK = contract.EndDoSettlementForListOutPatient(asyncResult);
                                if (IsOK)
                                {
                                    MessageBox.Show(eHCMSResources.A0962_G1_Msg_InfoQToanOK);
                                }
                                Coroutine.BeginExecute(ReloadData());
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
            mThread.Start();
        }
        #endregion

        #region Handle
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
        #endregion

        #region Other
        private TextBlock tbTotBalCredit = null;
        public void TotalBalanceCredit_Loaded(object source)
        {
            if (source != null)
            {
                tbTotBalCredit = source as TextBlock;
            }
        }
        #endregion
    }
}
