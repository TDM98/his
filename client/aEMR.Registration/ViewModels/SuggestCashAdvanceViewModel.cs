using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
/*
* 20220322 #001 QTD:  Thêm 1 danh sách BN tạm ứng bên OutstandingTask
* 20220416 #002 DatTB: Thêm cấu hình xác nhận hoãn tạm ứng
*/

namespace aEMR.Registration.ViewModels
{
    [Export(typeof(ISuggestCashAdvance)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SuggestCashAdvanceViewModel : ViewModelBase, ISuggestCashAdvance
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<InPatientRegistrationSelectedForInPatientCashAdvance>
    {
        private decimal MinPatientCashAdv = 0;
        [ImportingConstructor]
        public SuggestCashAdvanceViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            AllPaymentReason = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_PaymentReason).ToObservableCollection();

            Lookup firstItem = new Lookup();
            firstItem.LookupID = -1;
            firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0111_G1_HayChon1GTri);
            AllPaymentReason.Insert(0, firstItem);

            //CurRptPatientCashAdvReminder = new RptPatientCashAdvReminder();

            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            searchPatientAndRegVm.PatientFindByVisibility = false;
            ((INotifyPropertyChangedEx)searchPatientAndRegVm).PropertyChanged += searchPatientAndRegVm_PropertyChanged;
            SearchRegistrationContent = searchPatientAndRegVm;
            searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;

            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            ActivateItem(searchPatientAndRegVm);

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = false;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            //PaymentReasonContent = Globals.GetViewModel<IEnumListing>();
            //PaymentReasonContent.EnumType = typeof(AllLookupValues.V_PaymentReason);
            //PaymentReasonContent.AddSelectOneItem = true;
            //PaymentReasonContent.LoadData();

            //KMx: Thêm Khoa (06/01/2015 09:33).
            GetRespDepartments();

            //▼====: #002
            if (Globals.ServerConfigSection.CommonItems.EnablePostponementAdvancePayment)
            {
                //▼====: #001
                UCInPatientOutstandingTask = Globals.GetViewModel<IInPatientOutstandingTask>();
                UCInPatientOutstandingTask.WhichVM = SetOutStandingTask.DENGHI_TAMUNG;
                UCInPatientOutstandingTask.IsShowListOutPatientList = false;
                UCInPatientOutstandingTask.IsShowListInPatient = false;
                UCInPatientOutstandingTask.IsEnableDepartmentContent = true;
                UCInPatientOutstandingTask.IsShowListPatientCashAdvance = true;
                UCInPatientOutstandingTask.IsSearchForListPatientCashAdvance = true;
                //▲====: #001
            }
            //▲====: #002

            //lay gia tri o day ne!!
            //xem hien tai benh nhan dang nam o khoa nao?=>lay so tien buoc tam ung o khoa do len

            //MinPatientCashAdv = Convert.ToDecimal(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.MinPatientCashAdvance].ToString());

            // Txd 25/05/2014 Replaced ConfigList
            MinPatientCashAdv = (decimal)Globals.ServerConfigSection.Hospitals.MinPatientCashAdvance;

            ResetDefaultData();

            Globals.EventAggregator.Subscribe(this);
        }

        private bool _UsedByTaiVuOffice = false;
        public bool UsedByTaiVuOffice 
        {
            get { return _UsedByTaiVuOffice; }
            set
            {
                _UsedByTaiVuOffice = value;
                if (_UsedByTaiVuOffice)
                {
                    SearchRegistrationContent.CanSearhRegAllDept = true;
                }
                GetRespDepartments();
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

        void searchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0113_G1_IsLoading || e.PropertyName == eHCMSResources.Z0114_G1_IsSearchingRegistration)
            {
                NotifyWhenBusy();
            }
        }


        private void SetDefaultDepartment()
        {
            if (CurRptPatientCashAdvReminder == null || RespDepartments == null || RespDepartments.Count() <= 0)
            {
                return;
            }

            CurRptPatientCashAdvReminder.DepartmentSuggest = RespDepartments[0];

            if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0)
            {
                foreach (var deptItem in RespDepartments)
                {
                    if (deptItem.DeptID == Globals.ObjRefDepartment.DeptID)
                    {
                        CurRptPatientCashAdvReminder.DepartmentSuggest = deptItem;
                        break;
                    }
                }
            }

        }

        private void GetRespDepartments()
        {
            RespDepartments = new ObservableCollection<RefDepartment>();

            if (!Globals.isAccountCheck || UsedByTaiVuOffice)
            {
                RespDepartments = Globals.AllRefDepartmentList.Where(item => !item.IsDeleted).ToObservableCollection();
            }
            else
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList != null && Globals.LoggedUserAccount.DeptIDResponsibilityList.Count() > 0)
                {
                    RespDepartments = Globals.AllRefDepartmentList.Where(item => !item.IsDeleted && Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(item.DeptID)).ToObservableCollection();
                }
            }

            RefDepartment firstItem = new RefDepartment();
            firstItem.DeptID = 0;
            firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1997_G1_ChonKhoa);
            RespDepartments.Insert(0, firstItem);
            
        }


        private ObservableCollection<RefDepartment> _RespDepartments;
        public ObservableCollection<RefDepartment> RespDepartments
        {
            get
            {
                return _RespDepartments;
            }
            set
            {
                _RespDepartments = value;
                NotifyOfPropertyChange(() => RespDepartments);
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


        private void RptPatientCashAdvReminder_Add(RptPatientCashAdvReminder CashAdvance)
        {
            this.ShowBusyIndicator();
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
                                        ResetDefaultData();
                                        RptPatientCashAdvReminder_GetAll(null);

                                        //RptPatientCashAdvReminder_GetAll();
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
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void RptPatientCashAdvReminder_Update(RptPatientCashAdvReminder CashAdvance)
        {
            this.ShowBusyIndicator();
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
                                        ResetDefaultData();
                                        RptPatientCashAdvReminder_GetAll(null);
                                        //RptPatientCashAdvReminder_GetAll();
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
                finally
                {
                    this.HideBusyIndicator();
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

                                        RptPatientCashAdvReminder_GetAll(null);
                                        //RptPatientCashAdvReminder_GetAll();
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
        private void RptPatientCashAdvReminder_ByType()
        {
            if (CurRegistration == null || CurRegistration.RptPatientCashAdvReminders == null)
            {
                return;
            }

            TongTienChuaTamUng = 0;

            CurRptPatientCashAdvReminderLst = new ObservableCollection<RptPatientCashAdvReminder>();

            if (CurRegistration.RptPatientCashAdvReminders != null)
            {
                foreach(RptPatientCashAdvReminder item in CurRegistration.RptPatientCashAdvReminders)
                {
                    if (item.DepartmentSuggest == null)
                    {
                        continue;
                    }
                    if (item.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG
                        && RespDepartments.Any(x => x.DeptID == item.DepartmentSuggest.DeptID))
                    {
                        CurRptPatientCashAdvReminderLst.Add(item);
                    }
                }

                //KMx: Trước đây phiếu đề nghị không có lọc theo khoa. Bây giờ không có thời gian nên lọc theo khoa trên giao diện, đúng ra phải lọc dưới DB (06/01/2015 14:33).
                //CurRptPatientCashAdvReminderLst = CurRegistration.RptPatientCashAdvReminders.Where(x => x.V_CashAdvanceType == (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG).ToObservableCollection();
                
                TongTienChuaTamUng = CurRptPatientCashAdvReminderLst.Where(x => x.Checked == false).Sum(x => x.RemAmount);
            }
            CurRptPatientCashAdvReminder.RptPtCashAdvRemID = 0;
            CurRptPatientCashAdvReminder.RemNote = "";

            if (NoMinimumCashAdvAmtRequired)
            {
                CurRptPatientCashAdvReminder.RemAmount = (DebtRemaining - TongTienChuaTamUng > 0 ? DebtRemaining - TongTienChuaTamUng : 0);
            }
            else
            {
                CurRptPatientCashAdvReminder.RemAmount = MinPatientCashAdv + DebtRemaining - TongTienChuaTamUng > 0 ? MinPatientCashAdv + DebtRemaining - TongTienChuaTamUng : 0;
            }

            RptPatientCashAdvReminderCopy = CurRptPatientCashAdvReminder.DeepCopy();

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
                                decimal totalPtPayment_NotFinalized = 0;
                                decimal totalPtPaid_NotFinalized = 0;
                                decimal totalSupportFund_NotFinalized = 0;
                                var result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice, out totalRefundPatient, out totCashAdvBalanceAmount, 
                                                                                                out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized, out totalSupportFund_NotFinalized, asyncResult);

                                if (result)
                                {
                                    TotalLiabilities = totalLiabilities;
                                    SumOfAdvance = sumOfAdvance;
                                    TotalPatientPaymentPaidInvoice = totalPatientPaymentPaidInvoice;
                                    TotalRefundMoney = totalRefundPatient;
                                    TotalSupportFund = TotalCharityOrgPayment;
                                    TotalPatientPaid_NotFinalized = totalPtPaid_NotFinalized;
                                    TotalPatientPayment_NotFinalized = totalPtPayment_NotFinalized;
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
                                    RptPatientCashAdvReminder_ByType();
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


        public void ResetDefaultData()
        {
            CurRptPatientCashAdvReminder = new RptPatientCashAdvReminder();
            CurRptPatientCashAdvReminder.RemDate = Globals.GetCurServerDateTime();

            //if (PaymentReasonContent != null && PaymentReasonContent.EnumItemList != null && PaymentReasonContent.EnumItemList.Count > 0)
            //{
            //    PaymentReasonContent.SelectedItem = PaymentReasonContent.EnumItemList[0];
            //}

            if (AllPaymentReason != null && AllPaymentReason.Count > 0)
            {
                CurRptPatientCashAdvReminder.V_PaymentReason = AllPaymentReason.FirstOrDefault();
            }
            
            SetDefaultDepartment();
        }

        private IEnumerator<IResult> OpenRegistration(long regID)
        {
            ResetDefaultData();
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetCashAdvances = true;

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

            if (CurRegistration.AdmissionInfo != null)
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

                //KMx: Load số tiền tối thiểu phải tạm ứng của từng khoa (Không phải Kiên viết) (18/09/2014 15:24).
                var remininfo = new RefDepartmentReqCashAdv_DeptIDTask(regInfo.Registration.AdmissionInfo != null ? regInfo.Registration.AdmissionInfo.DeptID : -1);
                yield return remininfo;
                if (remininfo.RefDepartmentReqCashAdvList != null && remininfo.RefDepartmentReqCashAdvList.Count > 0)
                {
                    MinPatientCashAdv = remininfo.RefDepartmentReqCashAdvList.FirstOrDefault().CashAdvAmtReq;
                }
            }

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo);

            yield return GenericCoRoutineTask.StartTask(RptPatientCashAdvReminder_GetAll);

            yield break;
        }


        public void GetAllInPatientBillingInvoices()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllInPatientBillingInvoices(CurRegistration.PtRegistrationID, null, (long)AllLookupValues.RegistrationType.NOI_TRU, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var inv = contract.EndGetAllInPatientBillingInvoices(asyncResult);
                            
                            Action<IInPatientBillingInvoiceListingNew> onInitDlg = delegate (IInPatientBillingInvoiceListingNew vm)
                            {
                                vm.ShowEditColumn = false;
                                vm.ShowInfoColumn = true;
                                vm.ShowRecalcHiColumn = false;
                                vm.ShowRecalcHiWithPriceListColumn = false;
                                if (CurRegistration.HisID.GetValueOrDefault() > 0 && CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
                                {
                                    vm.ShowHIAppliedColumn = true;
                                }
                                else
                                {
                                    vm.ShowHIAppliedColumn = false;
                                }

                                vm.BillingInvoices = inv.ToObservableCollection();
                            };
                            GlobalsNAV.ShowDialog<IInPatientBillingInvoiceListingNew>(onInitDlg);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();

        }


        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.Z1004_G1_XoaDNghiTU))
            {
                return;
            }
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

        public void ViewBillCmd()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0300_G1_ChonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            GetAllInPatientBillingInvoices();

            //var vm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();

            //vm.ShowEditColumn = false;
            //vm.ShowInfoColumn = true;
            //vm.ShowRecalcHiColumn = false;
            //if (CurRegistration.HisID.GetValueOrDefault() > 0 && CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0)
            //{
            //    vm.ShowHIAppliedColumn = true;
            //}
            //else
            //{
            //    vm.ShowHIAppliedColumn = false;
            //}

            //vm.BillingInvoices = CurRegistration.InPatientBillingInvoices;

            //Globals.ShowDialog(vm as Conductor<object>);
        }

        public void CancelCmd()
        {
            CurRptPatientCashAdvReminder = RptPatientCashAdvReminderCopy.DeepCopy();
        }

        private bool _noMinimumCashAdvAmtRequired = false;
        public bool NoMinimumCashAdvAmtRequired
        {
            get { return _noMinimumCashAdvAmtRequired; }
            set
            {
                _noMinimumCashAdvAmtRequired = value;
                NotifyOfPropertyChange(() => NoMinimumCashAdvAmtRequired);
            }
        }

        public void ReCalcCashAdvRequestCmd()
        {
            if (NoMinimumCashAdvAmtRequired)
            {
                CurRptPatientCashAdvReminder.RemAmount = (DebtRemaining - TongTienChuaTamUng > 0 ? DebtRemaining - TongTienChuaTamUng : 0);
            }
            else
            {
                CurRptPatientCashAdvReminder.RemAmount = MinPatientCashAdv + DebtRemaining - TongTienChuaTamUng > 0 ? MinPatientCashAdv + DebtRemaining - TongTienChuaTamUng : 0;
            }
        }

        public void PayCmd()
        {
            if (CurRptPatientCashAdvReminder == null)
            {
                return;
            }

            if (!CanPayCmd)
            {
                return;
            }
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.K3167_G1_DNghiTU.ToLower()))
            {
                return;
            }
            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                MessageBox.Show(eHCMSResources.Z0275_G1_KgTheTUChoDKDaBiHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.DepartmentSuggest == null || CurRptPatientCashAdvReminder.DepartmentSuggest.DeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0094_G1_Msg_InfoChuaChonKhoaDeNghi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.RemAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien));
                return;
            }

            if (CurRptPatientCashAdvReminder.V_PaymentReason == null || CurRptPatientCashAdvReminder.V_PaymentReason.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0586_G1_Msg_InfoChonLoaiTU, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            CurRptPatientCashAdvReminder.V_CashAdvanceType = (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG;

            CurRptPatientCashAdvReminder.PtRegistrationID = CurRegistration.PtRegistrationID;
            CurRptPatientCashAdvReminder.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0);
            RptPatientCashAdvReminder_Add(CurRptPatientCashAdvReminder);

        }

        public void UpdateCmd()
        {
            if (CurRptPatientCashAdvReminder == null)
            {
                return;
            }

            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.Z0234_G1_CNhatPhDNTU.ToLower()))
            {
                return;
            }

            if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                MessageBox.Show(eHCMSResources.Z0275_G1_KgTheTUChoDKDaBiHuy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.DepartmentSuggest == null || CurRptPatientCashAdvReminder.DepartmentSuggest.DeptID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0094_G1_Msg_InfoChuaChonKhoaDeNghi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (CurRptPatientCashAdvReminder.RemAmount <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0276_G1_HayNhapSoTien));
                return;
            }

            if (CurRptPatientCashAdvReminder.V_PaymentReason == null || CurRptPatientCashAdvReminder.V_PaymentReason.LookupID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0586_G1_Msg_InfoChonLoaiTU, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            CurRptPatientCashAdvReminder.V_CashAdvanceType = (long)AllLookupValues.V_CashAdvanceType.DE_NGHI_TAM_UNG;
            CurRptPatientCashAdvReminder.PtRegistrationID = CurRegistration.PtRegistrationID;
            CurRptPatientCashAdvReminder.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0);

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
                if (test == null)
                {
                    return;
                }
                if (test.Checked)
                {
                    MessageBox.Show(eHCMSResources.A0674_G1_Msg_InfoKhDcSuaPhDaTU);
                    return;
                }

                //goi ham cap nhat o day ne phieu de nghi tam ung o day
                CurRptPatientCashAdvReminder = test.DeepCopy();

                //KMx: Nếu khoa đề nghị phiếu không nằm trong danh sách cấu hình trách nhiệm của nhân viên thì set khoa đề nghị = null (Trường hợp NV khoa A thấy phiếu đề nghị của NV khoa B)(06/01/2015 13:50).
                if (CurRptPatientCashAdvReminder != null && CurRptPatientCashAdvReminder.DepartmentSuggest != null
                    && !RespDepartments.Any(x => x.DeptID == CurRptPatientCashAdvReminder.DepartmentSuggest.DeptID))
                {
                    CurRptPatientCashAdvReminder.DepartmentSuggest = null;
                }
            }
        }

        public void lnkPreviewAdv_Click(object sender, RoutedEventArgs e)
        {
            var elem = sender as FrameworkElement;
            if (elem != null && elem.DataContext != null)
            {
                RptPatientCashAdvReminder item = elem.DataContext as RptPatientCashAdvReminder;
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.ID = item.RptPtCashAdvRemID;
                    // 20181017 TNHX: [BM0002176] Change PHIEUDENGHITAMUNG -> PHIEUDENGHITAMUNG_TV
                    proAlloc.eItem = ReportName.PHIEUDENGHITAMUNG_TV;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        #endregion

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
        //▼====: #001
        public void Handle(InPatientRegistrationSelectedForInPatientCashAdvance message)
        {
            if (message != null && message.Source != null)
            {
                if (GetView() != null)
                {
                    IsLoadingRegistration = true;
                    TotalLiabilities = 0;
                    SumOfAdvance = 0;
                    TotalRefundMoney = 0;
                    Coroutine.BeginExecute(OpenRegistration(message.Source.PtRegistrationID), null, (o, e) =>
                    {
                        IsLoadingRegistration = false;
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
        //▲====: #001
    }
}

//Load những giấy đề nghị tạm ứng.
//private void RptPatientCashAdvReminder_GetAll()
//{
//    var t = new Thread(() =>
//    {
//        try
//        {
//            using (var serviceFactory = new CommonServiceClient())
//            {
//                var contract = serviceFactory.ServiceInstance;
//                contract.BeginRptPatientCashAdvReminder_GetAll(CurRegistration.PtRegistrationID,
//                    Globals.DispatchCallback(asyncResult =>
//                    {
//                        try
//                        {
//                            var regItem = contract.EndRptPatientCashAdvReminder_GetAll(asyncResult);

//                            if (regItem != null)
//                            {
//                                CurRegistration.RptPatientCashAdvReminders = regItem.ToObservableCollection();
//                                RptPatientCashAdvReminder_ByType();
//                            }
//                        }
//                        catch (FaultException<AxException> fault)
//                        {
//                            ClientLoggerHelper.LogInfo(fault.ToString());
//                        }
//                        catch (Exception ex)
//                        {
//                            ClientLoggerHelper.LogInfo(ex.ToString());
//                        }
//                    }), null);
//            }
//        }
//        catch (Exception ex)
//        {
//            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//        }
//    });
//    t.Start();
//}

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
//    }
//    yield break;

//}