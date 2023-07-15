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
/*
* 20200805 #002 TNHX: Chuyển từ màn hình quản lý BN nội trú sang để tránh khoa phòng thao tác
* 20220318 #003 QTD:  Thêm 1 danh sách BN tạm ứng bên OutstandingTask
* 20220416 #004 DatTB: Thêm cấu hình xác nhận hoãn tạm ứng
* 20220526 #005 DatTB: Lấy thêm các biến xác nhận tạm hoãn tạm ứng
* 20220526 #006 DatTB: Thêm chức năng Xác nhận bn hoãn/ miễn tạm ứng
* 20220615 #007 DatTB: Thay đổi thông báo sáp nhập cho đăng ký nội trú
* 20220622 #008 DatTB:
* + Ẩn nút “Xác nhận hoãn/ miễn tạm ứng” khi đã tạm ứng
* + Ẩn nút sát nhập khi đã sát nhập rồi.
* 20230210 #009 BLQ: Thêm hàm check trước khi sát nhập
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientCashAdvance)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientCashAdvanceViewModel : ViewModelBase, IInPatientCashAdvance
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<InPatientRegistrationSelectedForInPatientCashAdvance>
    {
        [ImportingConstructor]
        public InPatientCashAdvanceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //authorization();
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            searchPatientAndRegVm.mTimDangKy = mTamUng_TimDangKy;

            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

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

            //PaymentReasonContent = Globals.GetViewModel<IEnumListing>();
            //PaymentReasonContent.EnumType = typeof(AllLookupValues.V_PaymentReason);
            //PaymentReasonContent.AddSelectOneItem = true;
            //PaymentReasonContent.LoadData();

            AllPaymentReason = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PaymentReason).ToObservableCollection();

            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
            AllPaymentReason.Insert(0, firstItem);

            AllPaymentMode = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PAYMENT_MODE).ToObservableCollection();

            AllPaymentMode.Insert(0, firstItem);

            //▼====: #004
            if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
            {
                //▼====: #003
                UCInPatientOutstandingTask = Globals.GetViewModel<IInPatientOutstandingTask>();
                UCInPatientOutstandingTask.WhichVM = SetOutStandingTask.THUTIEN_TAMUNG;
                UCInPatientOutstandingTask.IsShowListOutPatientList = false;
                UCInPatientOutstandingTask.IsShowListInPatient = false;
                UCInPatientOutstandingTask.IsEnableDepartmentContent = false;
                UCInPatientOutstandingTask.IsShowListPatientCashAdvance = true;
                UCInPatientOutstandingTask.IsSearchForListPatientCashAdvance = true;
                //▲====: #003

                //▼====: #005
                if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
                {
                    mXemIn_TamUng = true;
                    mTamHoan_TamUng = true;
                }
                //▲====: #005

            }
            //▲====: #004

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

        //private IEnumListing _paymentReasonContent;

        //public IEnumListing PaymentReasonContent
        //{
        //    get { return _paymentReasonContent; }
        //    set
        //    {
        //        _paymentReasonContent = value;
        //        NotifyOfPropertyChange(() => PaymentReasonContent);
        //    }
        //}

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


        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get { return _curRegistration; }
            set
            {
                _curRegistration = value;
                NotifyOfPropertyChange(() => CurRegistration);
                NotifyOfPropertyChange(() => CanPayCmd);
                NotifyOfPropertyChange(() => ShowMergerCmd);
                //▼==== #008
                NotifyOfPropertyChange(() => CanMergerCmd);
                eMergerCmd = (CurRegistration.OutPtRegistrationID != 0) && !(_curRegistration.PatientCashAdvances.Where(x => x.RptPtCashAdvRemID == 0).Count() > 0);
                //▲==== #008

                //▼==== #006
                if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
                {
                    NotifyOfPropertyChange(() => CanPrintPostponementCmd);
                    NotifyOfPropertyChange(() => CanConfirmPostponementCmd);

                    eTamHoan_TamUng = !_curRegistration.AdmissionInfo.IsConfirmedPostponement && (_curRegistration.PatientCashAdvances.Where(x => x.RptPtCashAdvRemID != 0).Count() <= 0); // #008
                    eXemIn_TamUng = _curRegistration.AdmissionInfo.IsConfirmedPostponement;
                }
                //▲==== #006
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
                PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
            }
        }

        private PatientCashAdvance _SelectedPtCashAdvance;
        public PatientCashAdvance SelectedPtCashAdvance
        {
            get { return _SelectedPtCashAdvance; }
            set
            {
                _SelectedPtCashAdvance = value;
                NotifyOfPropertyChange(() => SelectedPtCashAdvance);
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

        public void DeletePtCashAdvance()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0348_G1_DangXoaPhieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientCashAdvance_Delete(SelectedPtCashAdvance,
                            PatientSummaryInfoContent.CurrentPatient.PatientID,
                            Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DispatchCallback(asyncResult =>
                                {
                                    try
                                    {
                                        bool result = contract.EndPatientCashAdvance_Delete(out string msg, asyncResult);

                                        if (result)
                                        {
                                            MessageBox.Show(eHCMSResources.K0535_G1_XoaTUOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        }
                                        else
                                        {
                                            MessageBox.Show(eHCMSResources.K0536_G1_XoaTUFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        }
                                        if (!string.IsNullOrEmpty(msg))
                                        {
                                            Globals.ShowMessage(msg, "[CẢNH BÁO]");
                                        }
                                        Coroutine.BeginExecute(ReloadData());
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void PatientCashAdvance_Add(PatientCashAdvance CashAdvance)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientCashAdvance_Insert(CashAdvance,
                            PatientSummaryInfoContent.CurrentPatient.PatientID,
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
                                        //Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(ID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
                                        //PatientCashAdvance_GetAll();
                                        //CurRptPatientCashAdvReminder = null;
                                        if (FirstCastAdvance)
                                        {
                                            eTamHoan_TamUng = false; // #008

                                            //▼==== #007
                                            if (CurRegistration.OutPtRegistrationID != 0)
                                            {
                                                MergerCmd();
                                            }
                                            else
                                            {
                                                ResetDefaultData();
                                                Coroutine.BeginExecute(ReloadData());
                                            }
                                            //▲==== #007
                                        }
                                        else
                                        {
                                            ResetDefaultData();
                                            Coroutine.BeginExecute(ReloadData());
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        Globals.ShowMessage(msg, "[CẢNH BÁO]");
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
                    this.HideBusyIndicator();
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
                MessageBox.Show(eHCMSResources.A0604_G1_Msg_InfoNhapSoTien);
                return;
            }

            //string GeneralNote = string.Empty;
            //if (PaymentReasonContent.SelectedItem != null)
            //{
            //    if (PaymentReasonContent.SelectedItem.EnumItem != null)
            //    {
            //        GeneralNote = PaymentReasonContent.SelectedItem.Description;
            //    }

            //}
            var payment = new PatientCashAdvance
            {
                PaymentAmount = PayAmount.Value,
                GeneralNote = GeneralNote,
                PtRegistrationID = CurRegistration.PtRegistrationID,
                V_RegistrationType = CurRegistration.V_RegistrationType,
                PaymentDate = PaymentDate.GetValueOrDefault(Globals.GetCurServerDateTime()),
                StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                RptPtCashAdvRemID = CurRptPatientCashAdvReminder != null ? CurRptPatientCashAdvReminder.RptPtCashAdvRemID : 0,
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

            if (payment.V_PaymentMode == null || payment.V_PaymentMode.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0580_G1_Msg_InfoChonHTNop, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PatientCashAdvance_Add(payment);
        }

        public bool CanPayCmd
        {
            get
            {
                return CurRegistration != null;
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

        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (GetView() != null)
            {
                TotalLiabilities = 0;
                SumOfAdvance = 0;
                TotalRefundMoney = 0;
                Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID), null, (o, e) =>
                {
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

        //KMx: Sau khi kiểm tra, thấy không còn sử dụng nữa (18/09/2014 11:21).
        //MessageBoxTask _msgTask;
        //public IEnumerator<IResult> DoSetCurrentPatient(Patient patient)
        //{
        //    var p = patient;
        //    if (p == null || p.PatientID <= 0)
        //    {
        //        yield break;
        //    }

        //    if (p.PatientID > 0)
        //    {
        //        IsLoadingPatient = true;
        //        var loadPatient = new LoadPatientTask(p.PatientID);
        //        yield return loadPatient;

        //        IsLoadingPatient = false;

        //        if (loadPatient.CurrentPatient != null)
        //        {
        //            CurrentPatient = loadPatient.CurrentPatient;

        //            if (_currentPatient == null)
        //            {
        //                yield break;
        //            }

        //            if (_currentPatient.LatestRegistration == null || _currentPatient.LatestRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU) //Chưa có đăng ký lần nào
        //            {
        //                //Thong bao khong co dang ky.
        //                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0120_G1_KhongTimThayDKChoBNNay), eHCMSResources.G0442_G1_TBao);
        //                yield return _msgTask;
        //                yield break;
        //            }

        //            if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING
        //                    || _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
        //                    || _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND
        //                    //|| _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
        //                )
        //            {
        //                if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
        //                {
        //                    _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0121_G1_DKNayDaBiHuy), eHCMSResources.G0442_G1_TBao);
        //                    yield return _msgTask;
        //                }
        //                LoadRegistrationByID(_currentPatient.LatestRegistration.PtRegistrationID);
        //                yield break;
        //            }
        //            _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0122_G1_KhongTUChoDKNay), eHCMSResources.G0442_G1_TBao);
        //            yield return _msgTask;
        //            yield break;
        //        }
        //        CurrentPatient = null;
        //        CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        //    }
        //}

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
                //PatientSummaryInfoContent.ConfirmedPaperReferal = null;
                //PatientSummaryInfoContent.ConfirmedHiItem = null;
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
                //PatientSummaryInfoContent.HiBenefit = null;

                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;

                PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);
                //PatientSummaryInfoContent.ConfirmedPaperReferal = CurRegistration.PaperReferal;
                //PatientSummaryInfoContent.ConfirmedHiItem = CurRegistration.HealthInsurance;
                //PatientSummaryInfoContent.IsCrossRegion = CurRegistration.IsCrossRegion.GetValueOrDefault(false);
                //if (CurRegistration.PtInsuranceBenefit.HasValue)
                //{
                //    PatientSummaryInfoContent.HiBenefit = CurRegistration.PtInsuranceBenefit;
                //}                
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

            if (CurRegistration != null && CurRegistration.PatientTransaction != null && CurRegistration.PatientTransaction.PatientTransactionPayments != null)
            {
                patientPayments = CurRegistration.PatientTransaction.PatientTransactionPayments.Where(x => x.PtPmtAccID != 3).ToObservableCollection();
            }
            if (patientPayments == null)
            {
                patientPayments = new ObservableCollection<PatientTransactionPayment>();
            }
            OldPaymentContent.PatientPayments = patientPayments;

        }

        //KMx: Load phiếu đã tạm ứng (18/09/2014 16:50).
        private void PatientCashAdvance_GetAll(GenericCoRoutineTask genTask)
        {

            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientCashAdvance_GetAll(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var regItem = contract.EndPatientCashAdvance_GetAll(asyncResult);

                                if (regItem != null)
                                {
                                    CurRegistration.PatientCashAdvances = regItem.ToObservableCollection();
                                    if (CurRegistration.PatientCashAdvances != null && CurRegistration.PatientCashAdvances.Count() == 0)
                                    {
                                        FirstCastAdvance = true;
                                    }
                                    else
                                    {
                                        FirstCastAdvance = false;
                                    }

                                    NotifyOfPropertyChange(() => CanMergerCmd); // #008
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
                                //KMx: A.Tuấn dặn check null.
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

                    //KMx: A.Tuấn dặn check null.
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }


        //KMx: Load những giấy đề nghị tạm ứng (18/09/2014 15:46).
        private void RptPatientCashAdvReminder_GetAll(GenericCoRoutineTask genTask)
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRptPatientCashAdvReminder_GetAll(CurRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var regItem = contract.EndRptPatientCashAdvReminder_GetAll(asyncResult);

                                if (regItem != null)
                                {
                                    CurRegistration.RptPatientCashAdvReminders = regItem.ToObservableCollection();
                                    GetRptPatientCashAdvReminders();
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
                                //KMx: A.Tuấn dặn check null.
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

                    //KMx: A.Tuấn dặn check null.
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        //Load thông tin thanh toán (18/09/2014 15:46).
        private void GetPaymentInfo(GenericCoRoutineTask genTask)
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            decimal totalLiabilities = 0;
            decimal sumOfAdvance = 0;
            decimal totalPatientPaymentPaidInvoice = 0;
            decimal totalRefundPatient = 0;
            decimal totalPtPayment_NotFinalized = 0;
            decimal totalPtPaid_NotFinalized = 0;
            decimal totalSupportFund_NotFinalized = 0;
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientRegistrationAndPaymentInfo(CurRegistration.PtRegistrationID, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                decimal totCashAdvBalanceAmount = 0;
                                decimal TotalCharityOrgPayment = 0;
                                var result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice
                                    , out totalRefundPatient, out totCashAdvBalanceAmount
                                    , out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized, out totalSupportFund_NotFinalized, asyncResult);
                                if (result)
                                {
                                    TotalLiabilities = totalLiabilities;
                                    SumOfAdvance = sumOfAdvance;
                                    TotalRefundMoney = totalRefundPatient;
                                    TotalSupportFund = TotalCharityOrgPayment;
                                    TotalPatientPayment_NotFinalized = totalPtPayment_NotFinalized;
                                    TotalPatientPaid_NotFinalized = totalPtPaid_NotFinalized;
                                    TotalSupportFund_NotFinalized = totalSupportFund_NotFinalized;
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
                                //KMx: A.Tuấn dặn check null.
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

                    //KMx: A.Tuấn dặn check null.
                    if (genTask != null)
                    {
                        genTask.ActionComplete(false);
                    }
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        //KMx: Sau khi kiểm tra, thấy không còn sử dụng nữa (18/09/2014 11:21).
        //private IEnumerator<IResult> LoadRegistrationByID(long registrationID)
        //{
        //    var TargetTask = new GetInPatientRegistrationAndPaymentInfoTask(registrationID);
        //    yield return TargetTask;
        //    if (TargetTask.CurRegistration != null)
        //    {
        //        CurRegistration = TargetTask.CurRegistration;
        //        TotalLiabilities = TargetTask.totalLiabilities;
        //        TotalRefundMoney = TargetTask.TotalRefundPatient;
        //        SumOfAdvance = TargetTask.sumOfAdvance;
        //        GetRptPatientCashAdvReminders();
        //    }
        //    yield break;

        //}

        public void SetDefaultRem()
        {
            if (RptPatientCashAdvReminders == null)
            {
                RptPatientCashAdvReminders = new ObservableCollection<RptPatientCashAdvReminder>();
            }
            if (RptPatientCashAdvReminders != null && RptPatientCashAdvReminders.Count > 0)
            {
                CurRptPatientCashAdvReminder = RptPatientCashAdvReminders.FirstOrDefault();
            }
            RptPatientCashAdvReminder firstItem = new RptPatientCashAdvReminder();
            firstItem.RptPtCashAdvRemID = -1;
            firstItem.RemCode = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.A0351_G1_Msg_InfoChonPh);
            RptPatientCashAdvReminders.Insert(0, firstItem);
            if (RptPatientCashAdvReminders != null && RptPatientCashAdvReminders.Count == 1)
            {
                CurRptPatientCashAdvReminder = RptPatientCashAdvReminders.FirstOrDefault();
            }
        }

        private void GetRptPatientCashAdvReminders()
        {
            if (CurRegistration == null || CurRegistration.RptPatientCashAdvReminders == null)
            {
                return;
            }
            //RptPatientCashAdvReminders = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG && CurRegistration.PatientCashAdvances != null && !CurRegistration.PatientCashAdvances.Any(t2 => t2.RptPtCashAdvRemID == x.RptPtCashAdvRemID)).ToObservableCollection();
            RptPatientCashAdvReminders = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG && x.Checked == false).ToObservableCollection();

            SetDefaultRem();
        }

        public void ResetDefaultData()
        {
            CurRptPatientCashAdvReminder = null;

            PaymentDate = Globals.GetCurServerDateTime();

            PayAmount = 0;

            //if (PaymentReasonContent != null && PaymentReasonContent.EnumItemList != null && PaymentReasonContent.EnumItemList.Count > 0)
            //{
            //    PaymentReasonContent.SelectedItem = PaymentReasonContent.EnumItemList[0];
            //}

            if (AllPaymentReason != null && AllPaymentReason.Count > 0 && cbxPaymentReason != null)
            {
                cbxPaymentReason.SelectedItem = AllPaymentReason.Where(x => x.LookupID == (long)AllLookupValues.V_PaymentReason.TAM_UNG_NOI_TRU).FirstOrDefault();
            }

            if (AllPaymentMode != null && AllPaymentMode.Count > 0 && cbxPaymentMode != null)
            {
                cbxPaymentMode.SelectedItem = AllPaymentMode.Where(x => x.LookupID == (long)AllLookupValues.PaymentMode.TIEN_MAT).FirstOrDefault();
            }
        }

        private IEnumerator<IResult> OpenRegistration(long regID)
        {
            ResetDefaultData();
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetCashAdvances = true;
            LoadRegisSwitch.IsGetPatientTransactions = true;

            var regInfo = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.PatientFindBy.NOITRU, LoadRegisSwitch);
            yield return regInfo;

            if (regInfo == null || regInfo.Registration == null)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            CurRegistration = regInfo.Registration;
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
            }

            if (CurRegistration.PatientCashAdvances != null && CurRegistration.PatientCashAdvances.Count() == 0)
            {
                FirstCastAdvance = true;
            }
            else
            {
                FirstCastAdvance = false;
            }

            yield return GenericCoRoutineTask.StartTask(RptPatientCashAdvReminder_GetAll);

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo);

            yield break;
        }


        private IEnumerator<IResult> ReloadData()
        {
            yield return GenericCoRoutineTask.StartTask(PatientCashAdvance_GetAll);

            yield return GenericCoRoutineTask.StartTask(RptPatientCashAdvReminder_GetAll);

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo);
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

        //▼==== #005
        private bool _mXemIn_TamUng = false;
        public bool mXemIn_TamUng
        {
            get
            {
                return _mXemIn_TamUng;
            }
            set
            {
                if (_mXemIn_TamUng == value)
                    return;
                _mXemIn_TamUng = value;
                NotifyOfPropertyChange(() => mXemIn_TamUng);
            }
        }

        private bool _eXemIn_TamUng = false;
        public bool eXemIn_TamUng
        {
            get
            {
                return _eXemIn_TamUng;
            }
            set
            {
                if (_eXemIn_TamUng == value)
                    return;
                _eXemIn_TamUng = value;
                NotifyOfPropertyChange(() => eXemIn_TamUng);
            }
        }

        private bool _mTamHoan_TamUng = false;
        public bool mTamHoan_TamUng
        {
            get
            {
                return _mTamHoan_TamUng;
            }
            set
            {
                if (_mTamHoan_TamUng == value)
                    return;
                _mTamHoan_TamUng = value;
                NotifyOfPropertyChange(() => mTamHoan_TamUng);
            }
        }

        private bool _eTamHoan_TamUng = false;
        public bool eTamHoan_TamUng
        {
            get
            {
                return _eTamHoan_TamUng;
            }
            set
            {
                if (_eTamHoan_TamUng == value)
                    return;
                _eTamHoan_TamUng = value;
                NotifyOfPropertyChange(() => eTamHoan_TamUng);
            }
        }

        public bool CanPrintPostponementCmd
        {
            get
            {
                return CurRegistration != null;
            }
        }

        public bool CanConfirmPostponementCmd
        {
            get
            {
                return CurRegistration != null && !CurRegistration.AdmissionInfo.IsConfirmedPostponement;
            }
        }
        //▲==== #005

        //▼==== #008
        private bool _eMergerCmd = false;
        public bool eMergerCmd
        {
            get
            {
                return _eMergerCmd;
            }
            set
            {
                if (_eMergerCmd == value)
                    return;
                _eMergerCmd = value;
                NotifyOfPropertyChange(() => eMergerCmd);
            }
        }

        public bool CanMergerCmd
        {
            get
            {
                return CurRegistration != null && !(CurRegistration.PatientCashAdvances.Where(x => x.RptPtCashAdvRemID == 0).Count() > 0);
            }
        }
        //▲==== #008

        //phan nay nam trong module chung

        #endregion

        public void lnkPrint_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                PatientCashAdvance PtCashAdvance = elem.DataContext as PatientCashAdvance;
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.PaymentID = PtCashAdvance.PtCashAdvanceID;
                    proAlloc.FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                    proAlloc.eItem = ReportName.PATIENTCASHADVANCE_REPORT;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                //  Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(test.PtCashAdvanceID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPtCashAdvance == null || SelectedPtCashAdvance.PtCashAdvanceID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0569_G1_Msg_InfoChonPhTU, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                return;
            }

            if (SelectedPtCashAdvance.PaymentAmount != SelectedPtCashAdvance.BalanceAmount)
            {
                MessageBox.Show(eHCMSResources.A0995_G1_Msg_InfoKhTheXoaTU, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                return;
            }

            if (MessageBox.Show(string.Format("{0} {1}?", eHCMSResources.A0172_G1_Msg_ConfXoaTU, SelectedPtCashAdvance.CashAdvReceiptNum), eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            DeletePtCashAdvance();
        }

        public IEnumerator<IResult> PrintPatientCashAdvanceSilently(long PaymentID, int FindPatient)
        {
            yield return new PrintPatientCashAdvanceSilently(PaymentID, FindPatient);

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

        public void cbxReminder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurRptPatientCashAdvReminder == null)
            {
                return;
            }

            if (CurRptPatientCashAdvReminder.RptPtCashAdvRemID > 0)
            {
                PayAmount = CurRptPatientCashAdvReminder.RemAmount;

                cbxPaymentReason.SelectedItem = CurRptPatientCashAdvReminder.V_PaymentReason;
            }
            else
            {
                PayAmount = 0;
                if (AllPaymentReason != null)
                {
                    cbxPaymentReason.SelectedItem = AllPaymentReason.FirstOrDefault();
                }
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

        public void btnRemCode_Click(PatientCashAdvance aCashAdvance)
        {
            if (aCashAdvance == null || aCashAdvance.RptPtCashAdvRemID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = aCashAdvance.RptPtCashAdvRemID.Value;
                proAlloc.eItem = ReportName.PHIEUDENGHI_THANHTOAN;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        private bool _FirstCastAdvance = false;
        public bool FirstCastAdvance
        {
            get
            {
                return _FirstCastAdvance;
            }
            set
            {
                _FirstCastAdvance = value;
                NotifyOfPropertyChange(() => FirstCastAdvance);
            }
        }

        //▼===== #002
        public bool ShowMergerCmd
        {
            get
            {
                return Globals.ServerConfigSection.InRegisElements.MergerPatientRegistration == 2;
            }
        }

        public void MergerCmd()
        {
            Coroutine.BeginExecute(CheckValid_New());
        }
        //▲===== #002

        //▼====: #003
        public void Handle(InPatientRegistrationSelectedForInPatientCashAdvance message)
        {
            if (message != null && message.Source != null)
            {
                if (GetView() != null)
                {
                    TotalLiabilities = 0;
                    SumOfAdvance = 0;
                    TotalRefundMoney = 0;
                    Coroutine.BeginExecute(OpenRegistration(message.Source.PtRegistrationID), null, (o, e) =>
                    {
                    });
                }
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = UCInPatientOutstandingTask;
            homeVm.IsExpandOST = true;
            ActivateItem(UCInPatientOutstandingTask);
        }

        protected override void OnDeactivate(bool close)
        {
            Globals.EventAggregator.Unsubscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
            base.OnDeactivate(close);
        }

        private IInPatientOutstandingTask UCInPatientOutstandingTask { get; set; }
        //▲====: #003

        //▼===== #006
        public void ConfirmPostponementCmd()
        {
            if (!CanConfirmPostponementCmd)
            {
                return;
            }
            if (CurRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z1793_G1_KgCoDK);
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginConfirmPatientPostponementAdvance(CurRegistration.AdmissionInfo, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                contract.EndConfirmPatientPostponementAdvance(asyncResult);
                                eTamHoan_TamUng = false;
                                eXemIn_TamUng = true;
                                //▼==== #007
                                if (CurRegistration.OutPtRegistrationID != 0)
                                {
                                    MergerCmd();
                                    MessageBox.Show(eHCMSResources.Z3249_G1_TamHoanTamUngTC);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K2823_G1_DaThien);
                                }
                                //▲==== #007
                                ResetDefaultData();
                                Coroutine.BeginExecute(ReloadData());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void PrintPostponementCmd()
        {
            if (!CanPrintPostponementCmd)
            {
                return;
            }
            if (CurRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z1793_G1_KgCoDK);
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.ID = CurRegistration.AdmissionInfo.InPatientAdmDisDetailID;

                if (CurRegistration.AdmissionInfo.V_ObjectType == (long)AllLookupValues.V_ObjectType.DoiTac)
                    proAlloc.eItem = ReportName.GiayMienTamUngNoiTru;
                else
                    proAlloc.eItem = ReportName.GiayHoanTamUngNoiTru;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        //▲===== #006
        public void RefundInPatientCostCmd()
        {
            if (CurRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z1793_G1_KgCoDK);
                return;
            }
            if (!CurRegistration.AdmissionInfo.ConfirmNotTreatedAsInPt)
            {
                MessageBox.Show("Bệnh nhân chưa xác nhận không điều trị nội trú không thể hoàn trả chi phí");
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefundInPatientCost(CurRegistration, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                bool result = contract.EndRefundInPatientCost(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show("Đã hoàn trả chi phí về nội trú về ngoại trú!", "Thông báo", MessageBoxButton.OK);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                this.HideBusyIndicator();
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▼==== #009
        private string _errorMessages;
        public string ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                _errorMessages = value;
                NotifyOfPropertyChange(() => ErrorMessages);
            }
        }

        private string _confirmMessages;
        public string ConfirmMessages
        {
            get { return _confirmMessages; }
            set
            {
                _confirmMessages = value;
                NotifyOfPropertyChange(() => ConfirmMessages);
            }
        }
        WarningWithConfirmMsgBoxTask confirmBeforeDischarge = null;
        WarningWithConfirmMsgBoxTask errorMessageBox = null;
        public IEnumerator<IResult> CheckValid_New()
        {
            //if (MessageBox.Show(eHCMSResources.Z3038_G1_CanhBaoSapNhap, eHCMSResources.K1576_G1_CBao, MessageBoxButton.YesNo, MessageBoxImage.None, MessageBoxResult.No) == MessageBoxResult.No)
            //{
            //    return;
            //}
            //▼==== #008
            if (!CanMergerCmd)
            {
                yield break;
            }
            //▲==== #008
            if (CurRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z1793_G1_KgCoDK);
                yield break;
            }

            yield return GenericCoRoutineTask.StartTask(CheckBeforeDischarge_Action);

            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                ErrorMessages = string.Format("{0}: ", eHCMSResources.Z1405_G1_LoiSapNhapKgThanhCong) + Environment.NewLine + ErrorMessages;

                errorMessageBox = new WarningWithConfirmMsgBoxTask(ErrorMessages, "", false);
                yield return errorMessageBox;

                errorMessageBox = null;
                yield break;
            }

            if (!string.IsNullOrEmpty(ConfirmMessages))
            {
                ConfirmMessages = "Xác nhận sáp nhập đăng ký: " + Environment.NewLine + ConfirmMessages + Environment.NewLine +
                    "Bạn có đồng ý sáp nhập cho bệnh nhân này không?";
                confirmBeforeDischarge = new WarningWithConfirmMsgBoxTask(ConfirmMessages, "Tiếp tục sáp nhập");
                yield return confirmBeforeDischarge;
                if (!confirmBeforeDischarge.IsAccept)
                {
                    confirmBeforeDischarge = null;
                    yield break;
                }
                confirmBeforeDischarge = null;
            }

            MergerRegistration();

        }
        public void MergerRegistration()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginMergerPatientRegistration(CurRegistration, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                contract.EndMergerPatientRegistration(asyncResult);
                                MessageBox.Show(eHCMSResources.K2823_G1_DaThien);
                                ResetDefaultData();
                                Coroutine.BeginExecute(ReloadData());

                                eMergerCmd = false; // #008
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        private void CheckBeforeDischarge_Action(GenericCoRoutineTask genTask)
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                genTask.ActionComplete(false);
            }

            ErrorMessages = "";
            ConfirmMessages = "";

            string errorMsg = "";
            string confirmMsg = "";

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckBeforeMergerPatientRegistration(CurRegistration.PtRegistrationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var result = contract.EndCheckBeforeMergerPatientRegistration(out errorMsg, out confirmMsg, asyncResult);

                                    if (result)
                                    {
                                        ErrorMessages = errorMsg;
                                        ConfirmMessages = confirmMsg;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z1405_G1_LoiSapNhapKgThanhCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        bContinue = false;
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
        //▲==== #009
    }
}
