using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using aEMR.CommonTasks;
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
using eHCMSLanguage;
using aEMR.Controls;
using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
/*
 * 20181119 #001 TTM:   BM 0005257: Tạo out standing task tìm kiếm bệnh nhân nằm tại khoa và sự kiện chụp lại khi chọn bệnh nhân từ Out standing task.
 * 20190729 #001 TTM:   BM 0013014: Mặc định lý do chi thanh toán là chi phí điều trị còn thừa.
 * 20191109 #003 TNHX:  BM 0013015: Mặc định số tiền thanh toán. Chặn thanh toán nếu chưa quyết toán (Theo cấu hình BlockPaymentWhenNotSettlement)
 * 20221226 #004 QTD:   Bỏ qua khoá đăng ký thanh toán khi bật cấu hình đẩy cồng tự động
 */
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientPayment)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientPaymentViewModel : ViewModelBase, IInPatientPayment
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<InPatientRegistrationSelectedForInPatientPayment>
    {
        [ImportingConstructor]
        public InPatientPaymentViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            searchPatientAndRegVm.PatientFindByVisibility = false;
            ((INotifyPropertyChangedEx)searchPatientAndRegVm).PropertyChanged += SearchPatientAndRegVm_PropertyChanged;
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

            var TTPaymentVm = Globals.GetViewModel<IPatientPayment_InPt>();
            TTPaymentContent = TTPaymentVm;
            ActivateItem(TTPaymentVm);

            Coroutine.BeginExecute(LoadPaymentModes());

            Globals.EventAggregator.Subscribe(this);

            //▼====== #002
            AllRefundPaymentReason = Globals.AllLookupValueList.Where(x => (x.ObjectTypeID == (long)LookupValues.V_RefundPaymentReasonInPt && x.LookupID != (long)AllLookupValues.V_RefundPaymentReasonInPt.THUA_CP_DIEU_TRI)).ToObservableCollection();

            Lookup firstItem = new Lookup
            {
                LookupID = (long)AllLookupValues.V_RefundPaymentReasonInPt.THUA_CP_DIEU_TRI,
                ObjectValue = eHCMSResources.Z2782_G1_ChiPhiDieuTriConThua
            };
            AllRefundPaymentReason.Insert(0, firstItem);
            //▲====== #002
            ResetPatientPayment();
        }

        //▼====== #001
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
            homeVm.OutstandingTaskContent = ostvm;
            homeVm.IsExpandOST = true;
            ostvm.WhichVM = SetOutStandingTask.THANHTOAN;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            var homeVm = Globals.GetViewModel<IHome>();
            homeVm.OutstandingTaskContent = null;
            homeVm.IsExpandOST = false;
        }

        //▲====== #001
        void SearchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        //KMx: Chuyển từ view IPatientPayment sang IPatientPayment_InPt (tách ngoại trú và nội trú ra riêng) (01/01/2015 17:31).
        private IPatientPayment_InPt _TTPaymentContent;
        public IPatientPayment_InPt TTPaymentContent
        {
            get { return _TTPaymentContent; }
            set
            {
                _TTPaymentContent = value;
                NotifyOfPropertyChange(() => TTPaymentContent);
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
                return (_totalLiabilities + _TotalRefundMoney) - (_sumOfAdvance + TotalSupportFund);
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

        private void ResetPatientPayment()
        {
            SgtRptPatientCashAdvReminder = null;

            CurrentPayment = new PatientTransactionPayment
            {
                StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                PaymentMode = new Lookup() { LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT },
                PaymentType = new Lookup() { LookupID = (long)AllLookupValues.PaymentType.HOAN_TIEN },
                Currency = new Lookup() { LookupID = (long)AllLookupValues.Currency.VND },
                PtPmtAccID = 3,//benh vien tra tien lai cho benh nhan
                PaymentDate = Globals.GetCurServerDateTime()
            };
            if (AllRefundPaymentReason != null && AllRefundPaymentReason.Count > 0 && cbxRefundPaymentReason != null)
            {
                cbxRefundPaymentReason.SelectedItem = AllRefundPaymentReason.FirstOrDefault();
            }
        }

        private IEnumerator<IResult> LoadPaymentModes()
        {
            var paymentModeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE);
            yield return paymentModeTask;
            PaymentModeList = paymentModeTask.LookupList;
            yield break;
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
                //IsLoadingRegistration = true;
                TotalLiabilities = 0;
                SumOfAdvance = 0;
                TotalRefundMoney = 0;
                Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID), null, (o, e) => { });
            }
        }

        //▼====== #001
        public void Handle(InPatientRegistrationSelectedForInPatientPayment message)
        {
            if (GetView() != null && message != null && message.Source != null)
            {
                TotalLiabilities = 0;
                SumOfAdvance = 0;
                TotalRefundMoney = 0;
                Coroutine.BeginExecute(OpenRegistration(message.Source.PtRegistrationID), null, (o, e) => { });
            }
        }

        //▲====== #001
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
                PatientSummaryInfoContent.SetPatientHISumInfo(null);
                PatientSummaryInfoContent.CurrentPatientClassification = null;

                return;
            }
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
            ObservableCollection<PatientTransactionPayment> TTpatientPayments = null;

            if (CurRegistration != null && CurRegistration.PatientTransaction != null && CurRegistration.PatientTransaction.PatientTransactionPayments != null)
            {
                TTpatientPayments = CurRegistration.PatientTransaction.PatientTransactionPayments.Where(x => x.PtPmtAccID == 3).ToObservableCollection();
            }

            if (TTpatientPayments == null)
            {
                TTpatientPayments = new ObservableCollection<PatientTransactionPayment>();
            }
            if (TTpatientPayments.Any(x => x.PatientTransaction != null))
            {
                foreach (var item in TTpatientPayments.Where(x => x.PatientTransaction != null))
                {
                    item.PatientTransaction.PatientRegistration = CurRegistration;
                }
            }
            TTPaymentContent.PatientPayments = new PagedCollectionView(TTpatientPayments);
            TTPaymentContent.PatientPayments.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PaymentType"));
        }

        //private decimal sumOfAdvance = 0;

        //private IEnumerator<IResult> LoadRegistrationByID(long registrationID)
        //{
        //    var TargetTask = new GetInPatientRegistrationAndPaymentInfoTask(registrationID);
        //    yield return TargetTask;
        //    if (TargetTask.CurRegistration != null)
        //    {
        //        //CurRegistration = TargetTask.CurRegistration;
        //        TotalLiabilities = TargetTask.totalLiabilities;
        //        TotalRefundMoney = TargetTask.TotalRefundPatient;
        //        SumOfAdvance = TargetTask.sumOfAdvance;
        //        GetRptPatientCashAdvReminders();
        //    }
        //    yield break;
        //}

        private void GetRptPatientCashAdvReminders()
        {
            if (CurRegistration == null || CurRegistration.RptPatientCashAdvReminders == null)
            {
                return;
            }
            //RptPatientCashAdvReminders = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG && CurRegistration.PatientCashAdvances != null && !CurRegistration.PatientCashAdvances.Any(t2 => t2.RptPtCashAdvRemID == x.RptPtCashAdvRemID)).ToObservableCollection();
            RptPatientCashAdvReminderSgts = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_THANH_TOAN && x.Checked == false).ToObservableCollection();
        }

        public void ResetDefaultData()
        {
            ResetPatientPayment();
        }

        private IEnumerator<IResult> OpenRegistration(long regID)
        {
            ResetPatientPayment();

            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch
            {
                IsGetAdmissionInfo = true,
                IsGetPatientTransactions = true
            };

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

            yield return GenericCoRoutineTask.StartTask(RptPatientCashAdvReminder_GetAll);

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo);

            yield break;
        }

        private IEnumerator<IResult> ReloadData()
        {
            yield return GenericCoRoutineTask.StartTask(PatientTransactionPayment_GetAll);

            yield return GenericCoRoutineTask.StartTask(RptPatientCashAdvReminder_GetAll);

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo);
        }

        //KMx: Load những phiếu thanh toán (18/09/2014 16:50).
        private void PatientTransactionPayment_GetAll(GenericCoRoutineTask genTask)
        {

            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllPaymentByRegistrationID_InPt(CurRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var regItem = contract.EndGetAllPaymentByRegistrationID_InPt(asyncResult);

                                if (regItem != null)
                                {
                                    CurRegistration.PatientTransaction = regItem;
                                    InitViewForPayments();
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
                                var result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities
                                    , out sumOfAdvance, out totalPatientPaymentPaidInvoice, out totalRefundPatient, out totCashAdvBalanceAmount
                                    , out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized
                                    , out totalSupportFund_NotFinalized, asyncResult);
                                if (result)
                                {
                                    TotalLiabilities = totalLiabilities;
                                    SumOfAdvance = sumOfAdvance;
                                    TotalRefundMoney = totalRefundPatient;
                                    TotalSupportFund = TotalCharityOrgPayment;
                                    TotalPatientPayment_NotFinalized = totalPtPayment_NotFinalized;
                                    TotalPatientPaid_NotFinalized = totalPtPaid_NotFinalized;
                                    TotalSupportFund_NotFinalized = totalSupportFund_NotFinalized;
                                    //▼====: #003
                                    if (CurrentPayment != null && Globals.ServerConfigSection.InRegisElements.BlockPaymentWhenNotSettlement)
                                    {
                                        decimal tempPayMount = _TotalPatientPaid_NotFinalized + _TotalSupportFund_NotFinalized - _TotalPatientPayment_NotFinalized;
                                        CurrentPayment.PayAmount = tempPayMount > 0 ? tempPayMount : 0;
                                    }
                                    //▲====: #003
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

        public void ThanhToanTienChoBenhNhan(PatientTransactionPayment payment, PatientTransactionDetail TrDetail,
            long PtRegistrationID, long patientId, long V_RegistrationType)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);
            var t = new Thread(() =>
            {
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
                                        MessageBox.Show(eHCMSResources.Z0366_G1_TToanThanhCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        //Coroutine.BeginExecute(LoadRegistrationByID(CurRegistration.PtRegistrationID));
                                        ResetPatientPayment();
                                        Coroutine.BeginExecute(ReloadData());
                                        //goi ham in tu dong
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A1011_G1_Msg_InfoTToanFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        Globals.ShowMessage(msg, "[CẢNH BÁO]");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(ex.Message);
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
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private PatientTransactionDetail _CurPatientTransactionDetail;

        public void btnRefundMoney()
        {
            //▼====: #003
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0411_G1_Msg_InfoChuaCoTTinDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurRegistration != null && Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.G0128_G1_TToan)
                //▼====: #004
                && !Globals.ServerConfigSection.CommonItems.IsApplyAutoCreateHIReportWhenSettlement)
                //▲====: #004
            {
                return;
            }
            if (CurrentPayment == null)
            {
                return;
            }

            if (CurRegistration != null && Globals.ServerConfigSection.InRegisElements.BlockPaymentWhenNotSettlement && TotalPatientPayment_NotFinalized > 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z2904_G1_ThanhToanSauKhiQuyetToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //▲====: #003

            if (DebtRemaining >= 0)
            {
                MessageBox.Show(eHCMSResources.Z0443_G1_KgTheTToanBNKgConTien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentPayment.PayAmount <= 0)
            {
                MessageBox.Show(eHCMSResources.A0988_G1_Msg_InfoTienTToanLonHon0, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurrentPayment.PayAmount > Math.Abs(DebtRemaining))
            {
                MessageBox.Show(eHCMSResources.Z0444_G1_KgTheTToan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (cbxRefundPaymentReason != null)
            {
                CurrentPayment.V_RefundPaymentReasonInPt = (Lookup)cbxRefundPaymentReason.SelectedItem;
            }

            if (CurrentPayment.V_RefundPaymentReasonInPt == null || CurrentPayment.V_RefundPaymentReasonInPt.LookupID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0445_G1_ChonLyDoTToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            _CurPatientTransactionDetail = new PatientTransactionDetail
            {
                StaffID = Globals.LoggedUserAccount.StaffID,
                Amount = CurrentPayment.PayAmount,
                TranRefID = SgtRptPatientCashAdvReminder != null ? SgtRptPatientCashAdvReminder.RptPtCashAdvRemID : 0,
                V_TranRefType = AllLookupValues.V_TranRefType.BILL_THANH_TOAN
            };

            ThanhToanTienChoBenhNhan(CurrentPayment, _CurPatientTransactionDetail, CurRegistration.PtRegistrationID,
                CurRegistration.Patient.PatientID, (long)CurRegistration.V_RegistrationType);
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

        public void cbxReminder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SgtRptPatientCashAdvReminder == null || CurrentPayment == null)
            {
                return;
            }
            if (SgtRptPatientCashAdvReminder.RptPtCashAdvRemID > 0)
            {
                CurrentPayment.PayAmount = SgtRptPatientCashAdvReminder.RemAmount;

                cbxRefundPaymentReason.SelectedItem = SgtRptPatientCashAdvReminder.V_RefundPaymentReasonInPt;
            }
            else
            {
                CurrentPayment.PayAmount = 0;
                if (AllRefundPaymentReason != null)
                {
                    cbxRefundPaymentReason.SelectedItem = AllRefundPaymentReason.FirstOrDefault();
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

        private ObservableCollection<Lookup> _AllRefundPaymentReason;
        public ObservableCollection<Lookup> AllRefundPaymentReason
        {
            get
            {
                return _AllRefundPaymentReason;
            }
            set
            {
                if (_AllRefundPaymentReason != value)
                {
                    _AllRefundPaymentReason = value;
                    NotifyOfPropertyChange(() => AllRefundPaymentReason);
                }
            }
        }

        AxComboBox cbxRefundPaymentReason { get; set; }
        public void cbxRefundPaymentReason_Loaded(object sender, RoutedEventArgs e)
        {
            cbxRefundPaymentReason = sender as AxComboBox;
        }
    }
}
