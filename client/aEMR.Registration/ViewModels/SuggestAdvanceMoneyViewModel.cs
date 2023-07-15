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
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ISuggestAdvanceMoney)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SuggestAdvanceMoneyViewModel : ViewModelBase, ISuggestAdvanceMoney

        , IHandle<ItemSelected<PatientRegistration>>
    {
        private decimal MinPatientCashAdv = 0;
        [ImportingConstructor]
        public SuggestAdvanceMoneyViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            CurRptPatientCashAdvReminder = new RptPatientCashAdvReminder();
            SgtRptPatientCashAdvReminder = new RptPatientCashAdvReminder();

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

            PaymentReasonContent = Globals.GetViewModel<IEnumListing>();
            PaymentReasonContent.EnumType = typeof(AllLookupValues.V_PaymentReason);
            PaymentReasonContent.AddSelectOneItem = true;
            PaymentReasonContent.LoadData();

            var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            BillingInvoiceListingContent = billingVm;
            BillingInvoiceListingContent.ShowEditColumn = false;
            BillingInvoiceListingContent.ShowInfoColumn = true;
            BillingInvoiceListingContent.ShowRecalcHiColumn = false;
            BillingInvoiceListingContent.ShowRecalcHiWithPriceListColumn = false;
            BillingInvoiceListingContent.InvoiceDetailsContent.ShowDeleteColumn = false;

            Globals.EventAggregator.Subscribe(this);
            authorization();

            //lay gia tri o day ne!!
            //xem hien tai benh nhan dang nam o khoa nao?=>lay so tien buoc tam ung o khoa do len
            
            //MinPatientCashAdv = Convert.ToDecimal(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.MinPatientCashAdvance].ToString());

            // Txd 25/05/2014 Replaced ConfigList
            MinPatientCashAdv = (decimal)Globals.ServerConfigSection.Hospitals.MinPatientCashAdvance;
        }

        void searchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0113_G1_IsLoading || e.PropertyName == eHCMSResources.Z0114_G1_IsSearchingRegistration)
            {
                NotifyWhenBusy();
            }
        }

        private IInPatientBillingInvoiceListing _billingInvoiceListingContent;
        public IInPatientBillingInvoiceListing BillingInvoiceListingContent
        {
            get { return _billingInvoiceListingContent; }
            set
            {
                _billingInvoiceListingContent = value;
                NotifyOfPropertyChange(() => BillingInvoiceListingContent);
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


        private ObservableCollection<RptPatientCashAdvReminder> _CurRptPatientCashAdvReminderLst;
        public ObservableCollection<RptPatientCashAdvReminder> CurRptPatientCashAdvReminderLst
        {
            get { return _CurRptPatientCashAdvReminderLst; }
            set
            {
                _CurRptPatientCashAdvReminderLst = value;
                NotifyOfPropertyChange(() => CurRptPatientCashAdvReminderLst);
            }
        }

        private ObservableCollection<RptPatientCashAdvReminder> _SgtRptPatientCashAdvReminderLst;
        public ObservableCollection<RptPatientCashAdvReminder> SgtRptPatientCashAdvReminderLst
        {
            get { return _SgtRptPatientCashAdvReminderLst; }
            set
            {
                _SgtRptPatientCashAdvReminderLst = value;
                NotifyOfPropertyChange(() => SgtRptPatientCashAdvReminderLst);
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
                if (BillingInvoiceListingContent != null)
                {
                    if (CurRegistration.HisID.GetValueOrDefault() > 0 && CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                    {
                        BillingInvoiceListingContent.ShowHIAppliedColumn = true;
                    }
                    else
                    {
                        BillingInvoiceListingContent.ShowHIAppliedColumn = false;
                    }
                }
                PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
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

        public bool CanPayCmd
        {
            get
            {
                return CurRegistration != null && !IsPaying;
            }
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

        private decimal TongTienChuaTamUng = 0;
        private decimal TongTienChuaThanhToan = 0;


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
        //                Coroutine.BeginExecute(LoadRegistrationByID(_currentPatient.LatestRegistration.PtRegistrationID));
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
            if (CurRegistration == null)
            {
                PatientSummaryInfoContent.CurrentPatientClassification = null;
                PatientSummaryInfoContent.SetPatientHISumInfo(null);
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


        private void RptPatientCashAdvReminder_Add(RptPatientCashAdvReminder CashAdvance)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRptPatientCashAdvReminder_Insert(CashAdvance,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    long ID = 0;
                                    var regItem = contract.EndRptPatientCashAdvReminder_Insert(out ID, asyncResult);

                                    if (regItem)
                                    {
                                        //goi report in ra 
                                        MessageBox.Show(eHCMSResources.A0761_G1_Msg_InfoLapPhOK);

                                        //Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(ID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
                                        RptPatientCashAdvReminder_GetAll();
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

        private void RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder CashAdvance)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRptPatientCashAdvReminder_Update(CashAdvance,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndRptPatientCashAdvReminder_Update(asyncResult);
                                    if (regItem)
                                    {
                                        //goi report in ra 
                                        MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK);

                                        //Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(ID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
                                        RptPatientCashAdvReminder_GetAll();
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

        private void RptPatientCashAdvReminder_Delete(long ID)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRptPatientCashAdvReminder_Delete(ID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndRptPatientCashAdvReminder_Delete(asyncResult);
                                    if (regItem)
                                    {
                                        //goi report in ra 
                                        MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK);

                                        //Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(ID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
                                        RptPatientCashAdvReminder_GetAll();
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

        private RptPatientCashAdvReminder RptPatientCashAdvReminderCopy;
        private RptPatientCashAdvReminder SgtRptPatientCashAdvReminderCopy;
        private void RptPatientCashAdvReminder_ByType()
        {
            TongTienChuaTamUng = 0;
            TongTienChuaThanhToan = 0;
            if (CurRegistration.RptPatientCashAdvReminders != null)
            {
                CurRptPatientCashAdvReminderLst = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG).ToObservableCollection();
                TongTienChuaTamUng = CurRptPatientCashAdvReminderLst.Where(x => x.Checked == false).Sum(x => x.RemAmount);
                SgtRptPatientCashAdvReminderLst = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_THANH_TOAN).ToObservableCollection();
                TongTienChuaThanhToan = SgtRptPatientCashAdvReminderLst.Where(x => x.Checked == false).Sum(x => x.RemAmount);
            }
            CurRptPatientCashAdvReminder.RptPtCashAdvRemID = 0;
            CurRptPatientCashAdvReminder.RemNote = "";
            CurRptPatientCashAdvReminder.RemAmount = MinPatientCashAdv + DebtRemaining - TongTienChuaTamUng > 0 ? MinPatientCashAdv + DebtRemaining - TongTienChuaTamUng : 0;
            RptPatientCashAdvReminderCopy = CurRptPatientCashAdvReminder.DeepCopy();
            SgtRptPatientCashAdvReminder.RptPtCashAdvRemID = 0;
            SgtRptPatientCashAdvReminder.RemNote = "";
            SgtRptPatientCashAdvReminder.RemAmount = DebtRemaining + TongTienChuaThanhToan <= 0 ? Math.Abs(DebtRemaining) - TongTienChuaThanhToan : 0;
            SgtRptPatientCashAdvReminderCopy = SgtRptPatientCashAdvReminder.DeepCopy();
        }

        private void RptPatientCashAdvReminder_GetAll()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRptPatientCashAdvReminder_GetAll(CurRegistration.PtRegistrationID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    var regItem = contract.EndRptPatientCashAdvReminder_GetAll(asyncResult);

                                    if (regItem != null)
                                    {
                                        CurRegistration.RptPatientCashAdvReminders = regItem.ToObservableCollection();
                                        RptPatientCashAdvReminder_ByType();
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
        //        TotalPatientPaymentPaidInvoice = TargetTask.totalPatientPaymentPaidInvoice;
        //        SumOfAdvance = TargetTask.sumOfAdvance;
        //        RptPatientCashAdvReminder_ByType();

        //        if (CurRegistration != null)
        //        {
        //            BillingInvoiceListingContent.BillingInvoices = CurRegistration.InPatientBillingInvoices;
        //        }
        //    }
        //    yield break;

        //}
     
        private IEnumerator<IResult> OpenRegistration(long regID)
        {
            //var regInfo = new LoadRegistrationInfo_InPtTask(regID,(int)AllLookupValues.PatientFindBy.NOITRU);
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBillingInvoices = true;

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
                var remininfo = new RefDepartmentReqCashAdv_DeptIDTask(regInfo.Registration.AdmissionInfo != null ? regInfo.Registration.AdmissionInfo.DeptID : -1);
                yield return remininfo;
                if (remininfo.RefDepartmentReqCashAdvList != null && remininfo.RefDepartmentReqCashAdvList.Count > 0)
                {
                    MinPatientCashAdv = remininfo.RefDepartmentReqCashAdvList.FirstOrDefault().CashAdvAmtReq;
                }

                var TargetTask = new GetInPatientRegistrationAndPaymentInfoTask(regID);
                yield return TargetTask;
                if (TargetTask.CurRegistration != null)
                {
                    CurRegistration = TargetTask.CurRegistration;
                    TotalLiabilities = TargetTask.totalLiabilities;
                    TotalRefundMoney = TargetTask.TotalRefundPatient;
                    TotalPatientPaymentPaidInvoice = TargetTask.totalPatientPaymentPaidInvoice;
                    SumOfAdvance = TargetTask.sumOfAdvance;
                    RptPatientCashAdvReminder_ByType();

                    if (CurRegistration != null)
                    {
                        BillingInvoiceListingContent.BillingInvoices = CurRegistration.InPatientBillingInvoices;
                    }
                }
            }
            yield break;
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

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
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
                // Coroutine.BeginExecute(PrintPatientCashAdvanceSilently(test.PtCashAdvanceID, (int)AllLookupValues.V_FindPatientType.NOI_TRU));
            }
        }

        public IEnumerator<IResult> PrintPatientCashAdvanceSilently(long PaymentID, int FindPatient)
        {
            yield return new PrintPatientCashAdvanceSilently(PaymentID, FindPatient);
        }

        #region Them/Xoa/Sua/In cho de nghi tam ung

        public void CancelCmd()
        {
            CurRptPatientCashAdvReminder = RptPatientCashAdvReminderCopy.DeepCopy();
        }

        public void PayCmd()
        {
            if (!CanPayCmd)
            {
                return;
            }
            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                MessageBox.Show(eHCMSResources.Z0275_G1_KgTheTUChoDKDaBiHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.RemAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien));
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
            CurRptPatientCashAdvReminder.V_CashAdvanceType = (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG;
            CurRptPatientCashAdvReminder.PtRegistrationID = CurRegistration.PtRegistrationID;
            RptPatientCashAdvReminder_Add(CurRptPatientCashAdvReminder);

        }

        public void UpdateCmd()
        {
            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                MessageBox.Show(eHCMSResources.Z0275_G1_KgTheTUChoDKDaBiHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.RemAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien));
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
            CurRptPatientCashAdvReminder.V_CashAdvanceType = (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG;
            CurRptPatientCashAdvReminder.PtRegistrationID = CurRegistration.PtRegistrationID;
            RptPatientCashAdvReminder_Update(CurRptPatientCashAdvReminder);
        }


        public void lnkDeleteAdv_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder test = elem.DataContext as RptPatientCashAdvReminder;
                if (test != null)
                {
                    if (test.Checked)
                    {
                        MessageBox.Show(eHCMSResources.Z0512_G1_PhDaTUKgDuocXoa);
                    }
                    else
                    {
                        if (MessageBox.Show(eHCMSResources.A0170_G1_Msg_ConfXoaPhDNTU, eHCMSResources.T0432_G1_Error, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            RptPatientCashAdvReminder_Delete(test.RptPtCashAdvRemID);
                        }
                    }

                }
            }
        }

        public void lnkEditAdv_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder test = elem.DataContext as RptPatientCashAdvReminder;
                if (test != null)
                {
                    if (test.Checked)
                    {
                        MessageBox.Show(eHCMSResources.Z0512_G1_PhDaTUKgDuocXoa);
                    }
                    else
                    {
                        //goi ham cap nhat o day ne phieu de nghi tam ung o day
                        CurRptPatientCashAdvReminder = test.DeepCopy();
                    }
                }
            }
        }
        public void lnkPreviewAdv_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder test = elem.DataContext as RptPatientCashAdvReminder;
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.ID = test.RptPtCashAdvRemID;
                    // 20181017 TNHX: [BM0002176] Change PHIEUDENGHITAMUNG -> PHIEUDENGHITAMUNG_TV
                    proAlloc.eItem = ReportName.PHIEUDENGHITAMUNG_TV;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        #endregion

        #region Them/Xoa/Sua/In cho de nghi thanh toan

        public void btnRefundCancelCmd()
        {
            SgtRptPatientCashAdvReminder = SgtRptPatientCashAdvReminderCopy.DeepCopy();
        }

        public void btnRefundMoney()
        {
            if (SgtRptPatientCashAdvReminder.RemAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien));
                return;
            }
            if (SgtRptPatientCashAdvReminder.RemAmount > Math.Abs(DebtRemaining))
            {
                MessageBox.Show(string.Format("{0}: {1}", eHCMSResources.A0986_G1_Msg_InfoTienDNTToan, Math.Abs(DebtRemaining).ToString()));
                return;
            }
            SgtRptPatientCashAdvReminder.V_CashAdvanceType = (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_THANH_TOAN;
            SgtRptPatientCashAdvReminder.PtRegistrationID = CurRegistration.PtRegistrationID;
            RptPatientCashAdvReminder_Add(SgtRptPatientCashAdvReminder);
        }

        public void btnRefundUpdateCmd()
        {
            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                MessageBox.Show(eHCMSResources.Z0275_G1_KgTheTUChoDKDaBiHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.RemAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien));
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
            SgtRptPatientCashAdvReminder.V_CashAdvanceType = (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG;
            SgtRptPatientCashAdvReminder.PtRegistrationID = CurRegistration.PtRegistrationID;
            RptPatientCashAdvReminder_Update(SgtRptPatientCashAdvReminder);
        }


        public void lnkDeleteSuggest_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder test = elem.DataContext as RptPatientCashAdvReminder;
                if (test != null)
                {
                    if (test.Checked)
                    {
                        MessageBox.Show(eHCMSResources.Z0512_G1_PhDaTUKgDuocXoa);
                    }
                    else
                    {
                        if (MessageBox.Show(eHCMSResources.Z1003_G1_XoaPhDNghiTToan, eHCMSResources.T0432_G1_Error, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            RptPatientCashAdvReminder_Delete(test.RptPtCashAdvRemID);
                        }
                    }

                }
            }
        }

        public void lnkEditSuggest_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder test = elem.DataContext as RptPatientCashAdvReminder;
                if (test != null)
                {
                    if (test.Checked)
                    {
                        MessageBox.Show(eHCMSResources.A0674_G1_Msg_InfoKhDcSuaPhDaTU);
                    }
                    else
                    {
                        //goi ham cap nhat o day ne phieu de nghi tam ung o day
                        SgtRptPatientCashAdvReminder = test.DeepCopy();
                    }
                }
            }
        }
        public void lnkPreviewSuggest_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder test = elem.DataContext as RptPatientCashAdvReminder;
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.ID = test.RptPtCashAdvRemID;
                    // 20181017 TNHX: [BM0002176] Change PHIEUDENGHITAMUNG -> PHIEUDENGHITAMUNG_TV
                    proAlloc.eItem = ReportName.PHIEUDENGHITAMUNG_TV;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        #endregion
    }
}