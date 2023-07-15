using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.CommonTasks;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Common.Utilities;
using Castle.Windsor;
using System.Data;
/*
* 20180829 #001 TTM:   Dữ liệu (quyền lợi, lần khám hiện tại) không được truyền cho ViewModel con nên tick Tính BH (xem chi tiết bill) không hoạt động.
*                      Truyền dữ liệu để tick hoạt động bình thường
* 20181225 #002 CMN:   Added condition to make only once TransactionFinalization for each Registration
* 20190815 #003 TTM:   BM 0013055: Fix lỗi vẫn huỷ quyết toán cho ca đã xác nhận xuất viện rồi
* 20201019 #004 TNHX: Thêm thông tin khi quyết toán (Người quyết toán + Nơi làm việc )
* 20220531 #005 DatTB: Thêm nút Hủy quyết toán bỏ qua HDDT và phân quyền
* 20220531 #006 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
* 20221102 #007 QTD:   Đẩy cổng XML tự động khi quyết toán TV
* 20230420 #008 TNHX: Bỏ chạy tự động trên HIS chuyển qua service
* 20230701 #009 DatTB: Thêm/Sửa module phân quyền cho 2 nút hủy quyết toán
*/
namespace aEMR.Registration.ViewModels
{
    [Export(typeof(IInPatientSettlement)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientSettlementViewModel : Conductor<object>, IInPatientSettlement
        , IHandle<ItemSelected<PatientRegistration>>

    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public InPatientSettlementViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();

            //searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_NEW_PATIENT_BTN);
            searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);

            // TxD 05/01/2014: Changed 'CanSearhRegAllDept' default value to false and it will be reset again when UsedByTaiVuOffice property is set
            searchPatientAndRegVm.CanSearhRegAllDept = false;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;

            SearchRegistrationContent = searchPatientAndRegVm;

            ActivateItem(searchPatientAndRegVm);

            //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 16:54).
            //var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            BillingInvoiceListingContent = billingVm;
            BillingInvoiceListingContent.ShowEditColumn = false;
            BillingInvoiceListingContent.ShowInfoColumn = true;
            BillingInvoiceListingContent.ShowRecalcHiColumn = false;
            BillingInvoiceListingContent.ShowRecalcHiWithPriceListColumn = false;
            BillingInvoiceListingContent.ShowCheckItemColumn = true;
            //BillingInvoiceListingContent.InvoiceDetailsContent.ShowDeleteColumn = false;

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
            patientInfoVm.mInfo_CapNhatThongTinBN = false;
            patientInfoVm.mInfo_XacNhan = false;
            patientInfoVm.mInfo_XoaThe = false;
            patientInfoVm.mInfo_XemPhongKham = false;
            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            EditingSupportFund = Globals.GetViewModel<ICharitySupportFund>();

            ReviewSettlementContent = Globals.GetViewModel<IReviewSettlement>();

            SearchBillingAllDepts = false;

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
                else
                {
                    SearchRegistrationContent.CanSearhRegAllDept = false;
                }

                InitRespDepts();
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


        private void InitRespDepts()
        {
            if (UsedByTaiVuOffice)
            {
                RespDepartments = Globals.AllRefDepartmentList.Where(item => item.IsDeleted == false).ToObservableCollection();
                SearchBillingAllDepts = true;
            }
            else
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Count() > 0)
                {

                    RespDepartments = Globals.AllRefDepartmentList.Where(item => Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(item.DeptID)).ToObservableCollection();
                    //20190401 TTM: Lý do sửa đã giải thích ở Globals Line 213.
                    if (Globals.AddDrugDeptToListRespDepts && Globals.DrugDeptID > 0 && Globals.AllRefDepartmentList.Where(item => item.DeptID == Globals.DrugDeptID).Count() > 0)
                    {
                        RespDepartments.Add(Globals.AllRefDepartmentList.Single(item => item.DeptID == Globals.DrugDeptID));
                        Globals.LoggedUserAccount.DeptIDResponsibilityList.Add(Globals.DrugDeptID);
                    }
                }
                else
                {
                    // TxD 07/12/2014: This must be Administrator User , if WRONG then we have to review this code AGIAN                   
                    RespDepartments = Globals.AllRefDepartmentList.Where(item => item.IsDeleted == false).ToObservableCollection();
                }
            }

            if (RespDepartments.Count() > 0)
            {
                SelRespDepartments = RespDepartments[0];

                if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0)
                {
                    foreach (var deptItem in RespDepartments)
                    {
                        if (deptItem.DeptID == Globals.ObjRefDepartment.DeptID)
                        {
                            SelRespDepartments = deptItem;
                            break;
                        }
                    }
                }
            }

            AllowSearchBillingAllDepts = RespDepartments.Count() > 1;
        }

        private IReviewSettlement _reviewSettlementContent;
        public IReviewSettlement ReviewSettlementContent
        {
            get { return _reviewSettlementContent; }
            set
            {
                _reviewSettlementContent = value;
                NotifyOfPropertyChange(() => ReviewSettlementContent);
            }
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

        private RefDepartment _SelRespDepartments;
        public RefDepartment SelRespDepartments
        {
            get
            {
                return _SelRespDepartments;
            }
            set
            {
                _SelRespDepartments = value;
                NotifyOfPropertyChange(() => SelRespDepartments);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadDepartments();
                LoadPatientClassifications();
            }
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
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

        //private IInPatientBillingInvoiceListing _billingInvoiceListingContent;
        //public IInPatientBillingInvoiceListing BillingInvoiceListingContent
        //{
        //    get { return _billingInvoiceListingContent; }
        //    set
        //    {
        //        _billingInvoiceListingContent = value;
        //        NotifyOfPropertyChange(() => BillingInvoiceListingContent);
        //    }
        //}

        private IInPatientBillingInvoiceListingNew _billingInvoiceListingContent;
        public IInPatientBillingInvoiceListingNew BillingInvoiceListingContent
        {
            get { return _billingInvoiceListingContent; }
            set
            {
                _billingInvoiceListingContent = value;
                NotifyOfPropertyChange(() => BillingInvoiceListingContent);
            }
        }

        private ObservableCollection<RefDepartment> _departments;

        public ObservableCollection<RefDepartment> Departments
        {
            get { return _departments; }
            set
            {
                _departments = value;
                NotifyOfPropertyChange(() => Departments);
            }
        }

        private ObservableCollection<PatientClassification> _patientClassifications;

        public ObservableCollection<PatientClassification> PatientClassifications
        {
            get { return _patientClassifications; }
            set
            {
                _patientClassifications = value;
                NotifyOfPropertyChange(() => PatientClassifications);
            }
        }
        private RefDepartment _department;
        public RefDepartment Department
        {
            get
            {
                return _department;
            }
            set
            {
                if (_department != value)
                {
                    _department = value;
                    NotifyOfPropertyChange(() => Department);
                }
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
        /// <summary>
        /// Lấy lại những giá trị mặc định để đưa lên form
        /// </summary>
        public void ResetToDefaultValues()
        {
            ResetDepartmentToDefaultValue();
        }
        public void ResetDepartmentToDefaultValue()
        {
            var loginVm = Globals.GetViewModel<ILogin>();
            if (loginVm.DeptLocation != null)
            {
                Department = loginVm.DeptLocation.RefDepartment;
            }
            else
            {
                Department = null;
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
        /// <summary>
        /// Thông tin thẻ bảo hiểm đã được confirm
        /// </summary>
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

        private PatientRegistration _curRegistration;
        public PatientRegistration CurRegistration
        {
            get
            {
                return _curRegistration;
            }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    NotifyOfPropertyChange(() => CurRegistration);
                    if (CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate != null)
                    {
                        isDischarged = true;
                        NotifyOfPropertyChange(() => isDischarged);
                    }
                    else
                    {
                        isDischarged = false;
                        NotifyOfPropertyChange(() => isDischarged);
                    }
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
                    if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
                    {
                        EditingSupportFund.PtRegistrationID = CurRegistration.PtRegistrationID;
                    }
                    PatientSummaryInfoContent.CurrentPatientRegistration = CurRegistration;
                }
            }
        }
        private Patient _curPatient;
        public Patient CurPatient
        {
            get
            {
                return _curPatient;
            }
            set
            {
                _curPatient = value;
                NotifyOfPropertyChange(() => CurPatient);
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
            set
            {
                _confirmedPaperReferal = value;
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
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

        private bool _SearchBillingAllDepts = false;
        public bool SearchBillingAllDepts
        {
            get { return _SearchBillingAllDepts; }
            set
            {
                _SearchBillingAllDepts = value;
                NotifyOfPropertyChange(() => SearchBillingAllDepts);
            }
        }

        private bool _AllowSearchBillingAllDepts = false;
        public bool AllowSearchBillingAllDepts
        {
            get { return _AllowSearchBillingAllDepts; }
            set
            {
                _AllowSearchBillingAllDepts = value;
                NotifyOfPropertyChange(() => AllowSearchBillingAllDepts);
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
        public void LoadDepartments()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0149_G1_DangLayDSCacKhoa)
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDepartments(false,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allDepts =
                                    contract.
                                        EndGetAllDepartments(
                                            asyncResult);

                                if (allDepts != null)
                                {
                                    Departments =
                                        new ObservableCollection
                                            <RefDepartment>(
                                            allDepts);
                                    ResetDepartmentToDefaultValue();
                                }
                                else
                                {
                                    Departments = null;
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
        public void LoadPatientClassifications()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0150_G1_DangLayDSLoaiBN)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllClassifications(
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                var allClassifications = contract.EndGetAllClassifications(asyncResult);

                                if (allClassifications != null)
                                {
                                    PatientClassifications = new ObservableCollection<PatientClassification>(allClassifications);
                                }
                                else
                                {
                                    PatientClassifications = null;
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
        private void InitFormData()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                PatientSummaryInfoContent.CanConfirmHi = true;
                return;
            }

            PatientSummaryInfoContent.CanConfirmHi = false;

            //BillingInvoiceListingContent.BillingInvoices = CurRegistration.InPatientBillingInvoices;


            if (CurRegistration.InPatientBillingInvoices != null)
            {
                //KMx: Không hiển thị bill của khoa A và B (07/03/2015 14:45).

                // Hpt 17/10/2015: Dang ky Vang Lai _ Tien Giai Phau co kha nang co bill cua khoa A-B truoc khi nhap vien 
                // Nhung bill nay phai thu tien, thanh toan, quyet toan duoc neu khong se dan den khong the tao tiep dang ky sau do vi ly do dang ky truoc con bill chua quyet toan
                // Vi vay phai cho dang ky Vang Lai - Tien Giai Phau nhin thay bill khoa A-B

                if (Globals.ServerConfigSection.InRegisElements.ExcludeDeptAAndB && CurRegistration.AdmissionInfo != null)
                {
                    BillingInvoiceListingContent.BillingInvoices = CurRegistration.InPatientBillingInvoices.Where(x => x.DeptID != 97 && x.DeptID != 98).ToObservableCollection();
                }
                else
                {
                    BillingInvoiceListingContent.BillingInvoices = CurRegistration.InPatientBillingInvoices;
                }

            }
            else
            {
                BillingInvoiceListingContent.BillingInvoices = null;
            }



            ReviewSettlementContent.GetPatientSettlement(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType);
        }


        private void ShowOldRegistration(PatientRegistration regInfo)
        {
            CurRegistration = regInfo;
            //Chuyen sang mode giong nhu mo lai dang ky cu
            CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
            _confirmedHiItem = CurRegistration.HealthInsurance;
            _confirmedPaperReferal = CurRegistration.PaperReferal;
            NotifyOfPropertyChange(() => ConfirmedHiItem);
            NotifyOfPropertyChange(() => ConfirmedPaperReferal);
            InitRegistration();

            CanRegister = false;

            //▼====== #001
            BillingInvoiceListingContent.HIBenefit = CurRegistration.PtInsuranceBenefit;
            BillingInvoiceListingContent.CurentRegistration = this.CurRegistration;
            //▲====== #001

            if (PatientSummaryInfoContent != null)
            {
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
            CanChangePatientType = false;
        }

        private void DisplayRegistrationInfo()
        {
            //InitViewForPayments();
            if (CurRegistration == null)
            {
                PatientSummaryInfoContent.CurrentPatientClassification = null;
                PatientSummaryInfoContent.SetPatientHISumInfo(null);
                return;
            }
            //PayAmount = 0;
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


        /// <summary>
        /// Gọi hàm này khi tạo mới một đăng ký, hoặc load xong một đăng ký đã có.
        /// Khởi tạo những giá trị cần thiết để đưa lên form
        /// </summary>
        private void InitRegistration()
        {
            if (CurRegistration.PtRegistrationID <= 0)
            {
                CurRegistration.Patient = CurPatient;
            }
            else
            {
                _curPatient = CurRegistration.Patient;

                NotifyOfPropertyChange(() => CurPatient);
                if (_curPatient != null && _curPatient.PatientID > 0)
                {
                    CanRegister = true;
                }
                else
                {
                    CanRegister = false;
                }
            }
            InitFormData();
        }

        public IEnumerator<IResult> OpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            //LoadRegisSwitch.IsGetBillingInvoices = true;

            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(8)" });
            }
            else
            {
                CurRegistration = loadRegTask.Registration;
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                ShowOldRegistration(CurRegistration);
            }

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo_Action);

            yield return GenericCoRoutineTask.StartTask(LoadBillingInvoices_Action);

            //EditingSupportFund.IsHighTechServiceBill = false;
            EditingSupportFund.PtRegistrationID = CurRegistration.PtRegistrationID;
            EditingSupportFund.GetCharitySupportFunds();
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


        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message == null || message.Item == null)
            {
                return;
            }
            //▼===== #003: Thay đổi cách thức lấy dữ liệu khi double click vào danh sách tìm kiếm. Chuyển sang xài thẳng mà không đưa ID để đi tìm lại. Cần xem lại chỗ này vì có thể ảnh hưởng.
            //Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID));
            //20191219 TBL: Phải sử dụng lại Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID)) vì nếu không thì màn hình Thông tin bệnh nhân sẽ không lấy được Mã thẻ BHYT
            //Coroutine.BeginExecute(OpenRegistration_WithoutLoadRegAgain(message.Item));
            Coroutine.BeginExecute(OpenRegistration(message.Item.PtRegistrationID));
            //▲===== #003

        }
        public IEnumerator<IResult> OpenRegistration_WithoutLoadRegAgain(PatientRegistration curRegistration)
        {
            if (curRegistration == null)
            {
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(8)" });
            }
            else
            {
                CurRegistration = curRegistration;
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                ShowOldRegistration(CurRegistration);
            }

            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo_Action);

            yield return GenericCoRoutineTask.StartTask(LoadBillingInvoices_Action);

            EditingSupportFund.PtRegistrationID = CurRegistration.PtRegistrationID;
            EditingSupportFund.GetCharitySupportFunds();
        }
        #region Finalize Payment for Billing Invoice methods

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

        private decimal _TotalPaymentAmount;
        public decimal TotalPaymentAmount
        {
            get
            {
                return _TotalPaymentAmount;
            }
            set
            {
                _TotalPaymentAmount = value;
                NotifyOfPropertyChange(() => TotalPaymentAmount);
            }
        }

        MessageBoxTask msgTask;
        private IEnumerator<IResult> DoValidatePaymentInfo(YieldValidationResult result, List<InPatientBillingInvoice> listBillingInvoiceToBeFinalized, List<InPatientBillingInvoice> listBillingInvoiceSelected)
        {
            TotalPaymentAmount = 0;
            CurrentPayment.PayAmount = 0;
            CurrentPayment.PayAdvance = 0;
            CurrentPayment.HIAmount = 0;
            CurrentPayment.TotalSupport = 0;
            TotalSupportRemain = 0;
            result.IsValid = false;

            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                msgTask = new MessageBoxTask(eHCMSResources.A0411_G1_Msg_InfoChuaCoTTinDK, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            if (listBillingInvoiceSelected == null || listBillingInvoiceSelected.Count() == 0)
            {
                msgTask = new MessageBoxTask(eHCMSResources.A0412_G1_Msg_InfoChuaCoTTinHDon, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }

            listBillingInvoiceToBeFinalized.AddRange(listBillingInvoiceSelected.Where(item => item.PaidTime == null));
            if (listBillingInvoiceToBeFinalized.Count() <= 0)
            {
                msgTask = new MessageBoxTask(eHCMSResources.Z0598_G1_KgTheQToanBillDaQToan, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                yield return msgTask;
                yield break;
            }
            else
            {
                // Number of Bills displayed already paid for NOT YET finalized
                int nNumBillAlreadyPaidForNotYetFinal = (BillingInvoiceListingContent.BillingInvoices.Where(item => (item.TotalPatientPayment - item.TotalPatientPaid < 0) && item.PaidTime == null)).Count();
                // Number of selected Bills NOT YET PAID for OR NOT FULLY PAID
                int nNumBillNotYetPaid = listBillingInvoiceToBeFinalized.Where(item => (item.TotalPatientPayment - item.TotalPatientPaid) > 0).Count();

                if (nNumBillAlreadyPaidForNotYetFinal > 0 && nNumBillNotYetPaid > 0)
                {
                    msgTask = new MessageBoxTask(eHCMSResources.Z0600_G1_KgTheQToanBillChuaDongTien, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                    yield return msgTask;
                    yield break;
                }
            }

            //decimal TotalBillValueToBeFinalized = listBillingInvoiceToBeFinalized.Sum(obj => obj.TotalPatientPayment);
            //20191019 TBL: TotalBillValueToBeFinalized cần trừ số tiền miễn giảm ra để khi quyết toán không hiện lên thông báo bắt đóng thêm số tiền đã được miễn giảm
            //20191215 TBL: Không làm tròn tổng bill
            //decimal TotalBillValueToBeFinalized = Globals.DecimalRound(listBillingInvoiceToBeFinalized.Sum(obj => obj.TotalInvoicePrice), 0) - listBillingInvoiceToBeFinalized.Sum(obj => obj.TotalHIPayment) - listBillingInvoiceToBeFinalized.Sum(obj => obj.DiscountAmt);
            decimal TotalBillValueToBeFinalized = listBillingInvoiceToBeFinalized.Sum(obj => obj.TotalInvoicePrice) - listBillingInvoiceToBeFinalized.Sum(obj => obj.TotalHIPayment) - listBillingInvoiceToBeFinalized.Sum(obj => obj.DiscountAmt) - listBillingInvoiceToBeFinalized.Sum(obj => obj.OtherAmt);
            decimal TotalHIAmount = listBillingInvoiceToBeFinalized.Sum(obj => obj.TotalHIPayment);
            decimal TotalSupport = EditingSupportFund.AllSupportFunds.Where(x => x.BillingInvID == 0).Sum(x => x.AmountValue);
            if (listBillingInvoiceSelected.Any(x => x.IsHighTechServiceBill == false))
            {
                decimal TotalSupported = listBillingInvoiceSelected.Where(x => x.IsHighTechServiceBill == false).Sum(x => x.TotalSupportFund);
                TotalSupportRemain += EditingSupportFund.AllSupportFunds.Where(x => x.BillingInvID == 0).Sum(x => x.AmountValue) - TotalSupported;
            }
            foreach (var item in listBillingInvoiceSelected.Where(x => x.IsHighTechServiceBill))
            {
                TotalSupportRemain += EditingSupportFund.AllSupportFunds.Where(y => y.BillingInvID == item.InPatientBillingInvID || y.IsHighTechServiceBill).Sum(z => z.AmountValue);
            }

            // TxD 20/01/2015 Added (TotalBillValueToBeFinalized - SumOfCashAdvBalanceAmount) > 2 to prevent rounding of floating value issue
            if (TotalBillValueToBeFinalized > SumOfCashAdvBalanceAmount + TotalSupportRemain && (TotalBillValueToBeFinalized - (SumOfCashAdvBalanceAmount + TotalSupportRemain)) > 2)
            {
                //msgTask = new MessageBoxTask(eHCMSResources.Z0599_G1_KgDuTienDeQToan, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.Ok);
                //yield return msgTask;
                //yield break;
                var dialog = new MessageWarningShowDialogTask(string.Format(eHCMSResources.Z2557_G1_BNDongYThanhToanThem, Math.Ceiling(TotalBillValueToBeFinalized - (SumOfCashAdvBalanceAmount + TotalSupportRemain)).ToString("#,#")), eHCMSResources.G2363_G1_XNhan);
                yield return dialog;
                if (!dialog.IsAccept)
                {
                    yield break;
                }
                TotalPaymentAmount = Math.Ceiling(TotalBillValueToBeFinalized - (SumOfCashAdvBalanceAmount + TotalSupportRemain));
            }

            CurrentPayment.PayAmount = TotalBillValueToBeFinalized;
            CurrentPayment.PayAdvance = TotalBillValueToBeFinalized;
            //20191119 TBL: Đem tổng tiền BHYT trả, Quỹ hổ trợ lưu xuống bảng TransactionFinalizations để tránh lệch do làm tròn
            CurrentPayment.HIAmount = TotalHIAmount;
            CurrentPayment.TotalSupport = TotalSupport;

            result.IsValid = true;

            yield break;

        }

        private void GetPayableBillingInvoiceList_Action(GenericCoRoutineTask theTask, object objInvList, object objDestList, object objTotalPayment)
        {
            IEnumerable<InPatientBillingInvoice> invList = (IEnumerable<InPatientBillingInvoice>)objInvList;
            List<InPatientBillingInvoice> destList = (List<InPatientBillingInvoice>)objDestList;
            decimal totalPayment = (decimal)objTotalPayment;
            decimal totalPaymentSelected = 0;
            if (destList != null)
            {
                foreach (var inv in destList)
                {
                    if (inv.PaidTime == null)//Chua tra tien.
                    {
                        totalPaymentSelected += inv.TotalPatientPayment;
                    }
                }
            }

            CurrentPayment.PayAdvance = totalPaymentSelected;

            if (AutoPay)
            {
                totalPayment = totalPayment + (SumOfAdvance - SumOfPaidInvoices) - totalPaymentSelected; //Trong ngoặc là tổng số tiền bn trả dư đối với những bill ĐÃ THANH TOÁN nhưng CHƯA QUYẾT TOÁN
            }
            else
            {
                totalPayment = totalPayment - totalPaymentSelected;
            }
            if (totalPayment != 0)
            {
                foreach (var inv in invList)
                {
                    if (inv.PaidTime != null)//Đã tra tien.
                    {
                        continue;
                    }
                    if (destList.Contains(inv))//Đã co trong danh sach.
                    {
                        continue;
                    }

                    if (inv.TotalPatientPayment <= totalPayment)
                    {
                        destList.Add(inv);
                        //Ny them
                        CurrentPayment.PayAdvance += inv.TotalPatientPayment;

                        totalPayment -= inv.TotalPatientPayment;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            theTask.ActionComplete(true);

        }

        private void InitPatientPayment()
        {
            CurrentPayment = new PatientTransactionPayment();
            CurrentPayment.PaymentMode = new Lookup() { LookupID = (long)AllLookupValues.PaymentMode.TIEN_MAT };
            CurrentPayment.PaymentType = new Lookup() { LookupID = (long)AllLookupValues.PaymentType.TAM_UNG };
            CurrentPayment.Currency = new Lookup() { LookupID = (long)AllLookupValues.Currency.VND };
            CurrentPayment.PtPmtAccID = 1;
            CurrentPayment.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
        }

        private IEnumerator<IResult> DoPayForRegistration(List<InPatientBillingInvoice> listBillingInvoiceSelected)
        {
            InitPatientPayment();

            YieldValidationResult result = new YieldValidationResult();
            List<InPatientBillingInvoice> listBillingInvoiceToBeFinalized = new List<InPatientBillingInvoice>();
            System.Collections.IEnumerator e = DoValidatePaymentInfo(result, listBillingInvoiceToBeFinalized, listBillingInvoiceSelected);
            int cnt = 0;
            while (e.MoveNext())
            {
                ++cnt;
                yield return e.Current as IResult;
            }
            //OK mới làm tiếp
            if (!result.IsValid)
            {
                yield break;
            }

            yield return GenericCoRoutineTask.StartTask(FinalizePaymentForBillingInvoice_Action, listBillingInvoiceToBeFinalized);

            yield return GenericCoRoutineTask.StartTask(LoadBillingInvoices_Action);

            yield return GenericCoRoutineTask.StartTask(GetTotalCashAdvBalanceAmountOnly_Action);

            EditingSupportFund.GetCharitySupportFunds();

            //KMx: Load những lần quyết toán (28/12/2014 15:05). 
            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
            {
                ReviewSettlementContent.GetPatientSettlement(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType);
            }
            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo_Action);
        }

        public void LoadBillingInvoices_Action(GenericCoRoutineTask genTask)
        {
            if (RespDepartments.Count() <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0442_G1_SomethingIsWrongHere);
                return;
            }
            List<long> listDeptIDs = new List<long>();
            if (SearchBillingAllDepts)
            {
                listDeptIDs = RespDepartments.Select(item => item.DeptID).ToList();
                SelRespDepartments = null;
            }
            else
            {
                if (SelRespDepartments == null)
                {
                    SelRespDepartments = RespDepartments[0];
                }
                listDeptIDs.Add(SelRespDepartments.DeptID);
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllInPatientBillingInvoices_FromListDeptID(CurRegistration.PtRegistrationID, listDeptIDs,
                        Globals.DispatchCallback((asyncresult) =>
                        {
                            try
                            {
                                var updListBillingInv = contract.EndGetAllInPatientBillingInvoices_FromListDeptID(asyncresult);
                                if (updListBillingInv != null)
                                {
                                    //BillingInvoiceListingContent.BillingInvoices = updListBillingInv.ToObservableCollection();

                                    //KMx: Không hiển thị bill của khoa A và B (07/03/2015 14:45). 

                                    // Hpt 17/10/2015: Dang ky Vang Lai _ Tien Giai Phau co kha nang co bill cua khoa A-B truoc khi nhap vien 
                                    // Nhung bill nay phai thu tien, thanh toan, quyet toan duoc neu khong se dan den khong the tao tiep dang ky sau do vi ly do dang ky truoc con bill chua quyet toan
                                    // Vi vay phai cho dang ky Vang Lai - Tien Giai Phau nhin thay bill khoa A-B

                                    if (Globals.ServerConfigSection.InRegisElements.ExcludeDeptAAndB && CurRegistration.AdmissionInfo != null)
                                    {
                                        BillingInvoiceListingContent.BillingInvoices = updListBillingInv.Where(x => x.DeptID != 97 && x.DeptID != 98).ToObservableCollection();
                                    }
                                    else
                                    {
                                        BillingInvoiceListingContent.BillingInvoices = updListBillingInv.ToObservableCollection();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                if (genTask != null)
                                {
                                    genTask.ActionComplete(true);
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    if (genTask != null)
                    {
                        genTask.ActionComplete(true);
                    }
                    MessageBox.Show(ex.ToString());
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void FinalizePaymentForBillingInvoice_Action(GenericCoRoutineTask genTask, object objListBillingInv)
        {
            IList<InPatientBillingInvoice> _billingInvoiceList = (IList<InPatientBillingInvoice>)objListBillingInv;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        PatientTransaction _patientTransaction;
                        contract.BeginFinalizeInPatientBillingInvoices(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0), CurRegistration.PtRegistrationID,
                            CurrentPayment, _billingInvoiceList, TotalPaymentAmount,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    contract.EndFinalizeInPatientBillingInvoices(out string msg, out _patientTransaction, asyncResult);
                                    MessageBox.Show(eHCMSResources.A0962_G1_Msg_InfoQToanOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //▼====: #008
                                    //▼====: #007
                                    //if (Globals.ServerConfigSection.CommonItems.IsApplyAutoCreateHIReportWhenSettlement 
                                    //    && CurRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU
                                    //    && UsedByTaiVuOffice)
                                    //{
                                    //    if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token))
                                    //        || (GlobalsNAV.gLoggedHIAPIUser != null && GlobalsNAV.gLoggedHIAPIUser.APIKey != null && GlobalsNAV.gLoggedHIAPIUser.APIKey.expires_in <= DateTime.Now))
                                    //    {
                                    //        GlobalsNAV.LoginHIAPI();
                                    //    }
                                    //    Coroutine.BeginExecute(CreateHIReportOutInPtXml_Routine());
                                    //}
                                    //▲====: #007
                                    //▲====: #008
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        Globals.ShowMessage(msg, "[CẢNH BÁO]");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    ClientLoggerHelper.LogError(ex.Message);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    genTask.ActionComplete(true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                    genTask.ActionComplete(true);
                }
            });
            t.Start();
        }


        public void StartCalculating()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetInPatientRegistrationNonFinalizedLiabilities(CurRegistration.PtRegistrationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                decimal liabilities;
                                decimal advance;
                                decimal totalPatientPayment_PaidInvoice;
                                decimal TotalRefundPatient;
                                try
                                {
                                    var bOK = contract.EndGetInPatientRegistrationNonFinalizedLiabilities(out liabilities, out advance, out totalPatientPayment_PaidInvoice, out TotalRefundPatient, asyncResult);

                                    if (bOK)
                                    {
                                        TotalLiabilities = liabilities;
                                        SumOfAdvance = advance;//tong so tien benh nhan ung
                                        SumOfPaidInvoices = totalPatientPayment_PaidInvoice;
                                        TotalRefundMoney = TotalRefundPatient;

                                        // TxD 06/01/2015 not used commented out 
                                        //CalcPatientPayment();

                                    }
                                    else
                                    {
                                    }
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
                }
            });
            t.Start();
        }

        private bool AutoPay { get; set; }

        private decimal _sumOfPaidInvoices = 0;
        /// <summary>
        /// Tổng tiền trả đối với những bill ĐÃ THANH TOÁN nhưng CHƯA QUYẾT TOÁN
        /// </summary>
        public decimal SumOfPaidInvoices
        {
            get { return _sumOfPaidInvoices; }
            set
            {
                _sumOfPaidInvoices = value;
                NotifyOfPropertyChange(() => SumOfPaidInvoices);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
            }
        }

        #endregion Finalize Payment for Billing Invoice methods

        public void RefreshBillingInvListCmd()
        {
            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
            {
                LoadBillingInvoices_Action(null);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0574_G1_Msg_InfoChonBNDeQToan);
            }
        }

        private decimal _TotalSupportRemain = 0;
        public decimal TotalSupportRemain
        {
            get
            {
                return _TotalSupportRemain;
            }
            set
            {
                _TotalSupportRemain = value;
                NotifyOfPropertyChange(() => TotalSupportRemain);
            }
        }

        public void PayCmd()
        {
            if (CurRegistration == null)
            {
                return;
            }
            if (!Globals.ServerConfigSection.CommonItems.IsApplyAutoCreateHIReportWhenSettlement && Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.Q0498_G1_QuyetToan.ToLower()))
            {
                Globals.ShowMessage("Ca điều trị đã được báo cáo BHYT. Để thực hiện các bước tiếp theo vui lòng liên hệ P. BHYT hủy báo cáo BHYT.", eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.DischargeDate == null)
            {
                Globals.ShowMessage("Bệnh nhân chưa xuất viện không thể quyết toán", eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▼====: #002
            //List<InPatientBillingInvoice> selBillingInv = BillingInvoiceListingContent.GetSelectedItems();
            if (ReviewSettlementContent.PatientSettlementList != null && ReviewSettlementContent.PatientSettlementList.Count > 0
                && (CurRegistration == null || CurRegistration.V_RegForPatientOfType != AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU))
            {
                MessageBox.Show(eHCMSResources.Z2371_G1_Msg, eHCMSResources.G0442_G1_TBao);
                return;
            }
            List<InPatientBillingInvoice> selBillingInv = BillingInvoiceListingContent.BillingInvoices == null ? new List<InPatientBillingInvoice>() : BillingInvoiceListingContent.BillingInvoices.ToList();
            //▲====: #002
            if (selBillingInv == null)
            {
                MessageBox.Show(eHCMSResources.A0627_G1_Msg_InfoKhCoBillDcChonDeQToan);
                return;
            }
            if (selBillingInv.Count() > 0)
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.isCheckBox = true;
                MessBox.SetMessage(eHCMSResources.Z2395_G1_Msg, eHCMSResources.K3847_G1_DongY);
                MessBox.FireOncloseEvent = true;
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (MessBox.IsAccept)
                {
                    Coroutine.BeginExecute(DoPayForRegistration(selBillingInv));
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0555_G1_YCChonBillDeQToan);
            }
            return;
        }

        public void XuatHD()
        {
            if (CurRegistration == null || CurRegistration.Patient == null || Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.Staff == null)
            {
                return;
            }
            if (ReviewSettlementContent.PatientSettlementList == null || ReviewSettlementContent.PatientSettlementList.Count == 0 || ReviewSettlementContent.PatientSettlementList.FirstOrDefault().TranFinalizationID < 0)
            {
                MessageBox.Show(eHCMSResources.Z2396_G1_DKChuaDuocQuyetToan);
                return;
            }
            //▼====: #004
            GlobalsNAV.ShowDialog((IEditOutPtTransactionFinalization aView) =>
            {
                aView.Registration = CurRegistration;
                aView.TransactionFinalizationObj = new OutPtTransactionFinalization
                {
                    TranFinalizationID = ReviewSettlementContent.PatientSettlementList.FirstOrDefault().TranFinalizationID,
                    TaxMemberName = CurRegistration.Patient.FullName,
                    TaxMemberAddress = CurRegistration.Patient.PatientFullStreetAddress,
                    StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                    CreatedStaff = Globals.LoggedUserAccount.Staff,
                    PtRegistrationID = CurRegistration.PtRegistrationID,
                    PatientFullName = CurRegistration.Patient == null ? null : CurRegistration.Patient.FullName,
                    V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                    V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU,
                    Buyer = CurRegistration.Patient.PatientEmployer
                };
            });
            //▲====: #004
        }


        //public void SettlementCmd()
        //{
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginInPatientSettlement(CurRegistration.PtRegistrationID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0),
        //                    Globals.DispatchCallback(asyncResult =>
        //                    {
        //                        try
        //                        {
        //                            contract.EndInPatientSettlement(asyncResult);
        //                            MessageBox.Show(eHCMSResources.A0962_G1_Msg_InfoQToanOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                            ClientLoggerHelper.LogInfo(ex.ToString());
        //                        }
        //                        finally
        //                        {
        //                            this.HideBusyIndicator();
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

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

        public decimal BalanceCreditRemaining
        {
            get
            {
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

        private decimal _SumOfCashAdvBalanceAmount;
        public decimal SumOfCashAdvBalanceAmount
        {
            get { return _SumOfCashAdvBalanceAmount; }
            set
            {
                _SumOfCashAdvBalanceAmount = value;
                NotifyOfPropertyChange(() => SumOfCashAdvBalanceAmount);
            }
        }

        public void LoadPaymentInfo()
        {
            Coroutine.BeginExecute(LoadPaymentInfoTask());
        }

        public IEnumerator<IResult> LoadPaymentInfoTask()
        {
            yield return GenericCoRoutineTask.StartTask(GetPaymentInfo_Action);
        }

        //Load thông tin thanh toán (18/09/2014 15:46).
        private void GetPaymentInfo_Action(GenericCoRoutineTask genTask)
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                genTask.ActionComplete(false);
            }
            decimal totalLiabilities = 0;
            decimal sumOfAdvance = 0;
            decimal totalPatientPaymentPaidInvoice = 0;
            decimal totalRefundPatient = 0;
            decimal totalCashAdvBalanceAmount = 0;
            decimal TotalCharityOrgPayment = 0;
            decimal totalPtPayment_NotFinalized = 0;
            decimal totalPtPaid_NotFinalized = 0;
            decimal totalSupportFund_NotFinalized = 0;
            this.ShowBusyIndicator();

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
                                var result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice, out totalRefundPatient, out totalCashAdvBalanceAmount,
                                                                                                out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized, out totalSupportFund_NotFinalized, asyncResult);

                                if (result)
                                {
                                    TotalLiabilities = totalLiabilities;
                                    SumOfAdvance = sumOfAdvance;
                                    TotalRefundMoney = totalRefundPatient;
                                    SumOfCashAdvBalanceAmount = totalCashAdvBalanceAmount;
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

        private void GetTotalCashAdvBalanceAmountOnly_Action(GenericCoRoutineTask genTask)
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID <= 0)
            {
                if (genTask != null)
                {
                    genTask.ActionComplete(false);
                }
            }

            decimal totalLiabilities = 0;
            decimal sumOfAdvance = 0;
            decimal totalPatientPaymentPaidInvoice = 0;
            decimal totalRefundPatient = 0;
            decimal totalCashAdvBalanceAmount = 0;
            decimal TotalCharityOrgPayment = 0;
            decimal totalPtPayment_NotFinalized = 0;
            decimal totalPtPaid_NotFinalized = 0;
            decimal totalSupportFund_NotFinalized = 0;

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientRegistrationAndPaymentInfo(CurRegistration.PtRegistrationID, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var result = contract.EndGetInPatientRegistrationAndPaymentInfo(out totalLiabilities, out sumOfAdvance, out totalPatientPaymentPaidInvoice, out totalRefundPatient, out totalCashAdvBalanceAmount,
                                                                                                out TotalCharityOrgPayment, out totalPtPayment_NotFinalized, out totalPtPaid_NotFinalized, out totalSupportFund_NotFinalized, asyncResult);

                                if (result)
                                {
                                    SumOfCashAdvBalanceAmount = totalCashAdvBalanceAmount;
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
        public void ResetView()
        {
            BillingInvoiceListingContent.BillingInvoices = null;
            //BillingInvoiceListingContent.BeingViewedItem = null;

            PatientSummaryInfoContent.SetPatientHISumInfo(null);
        }

        private TextBlock tbTotBalCredit = null;
        public void TotalBalanceCredit_Loaded(object source)
        {
            if (source != null)
            {
                tbTotBalCredit = source as TextBlock;
            }
        }

        private ICharitySupportFund _EditingSupportFund;
        public ICharitySupportFund EditingSupportFund
        {
            get { return _EditingSupportFund; }
            set
            {
                _EditingSupportFund = value;
                NotifyOfPropertyChange(() => EditingSupportFund);
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
                NotifyOfPropertyChange(() => TotalPatientPaid_Finalized);
                NotifyOfPropertyChange(() => BalanceCreditRemaining);
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

        private bool _IsUserAdmin = Globals.IsUserAdmin;
        public bool IsUserAdmin
        {
            get
            {
                return _IsUserAdmin;
            }
            set
            {
                if (_IsUserAdmin == value)
                {
                    return;
                }
                _IsUserAdmin = value;
                NotifyOfPropertyChange(() => IsUserAdmin);
            }
        }

        //▼==== #005
        //▼==== #009
        private bool _mDelTranFinal = Globals.IsUserAdmin || Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mTranFinal);
        public bool mDelTranFinal
        {
            get
            {
                return _mDelTranFinal;
            }
            set
            {
                if (_mDelTranFinal == value)
                {
                    return;
                }
                _mDelTranFinal = value;
                NotifyOfPropertyChange(() => mDelTranFinal);
            }
        }

        private bool _mDelTranFinalWithOutBill = Globals.IsUserAdmin || Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mTranFinalWithOutBill);
        public bool mDelTranFinalWithOutBill
        {
            get
            {
                return _mDelTranFinalWithOutBill;
            }
            set
            {
                if (_mDelTranFinalWithOutBill == value)
                {
                    return;
                }
                _mDelTranFinalWithOutBill = value;
                NotifyOfPropertyChange(() => mDelTranFinalWithOutBill);
            }
        }
        //▲==== #009

        public void btnDelTranFinalWithOutBill()
        {
            btnDelTranFinal(true);
        }
        //▲==== #005

        //▼==== #006
        public void btnDelTranFinal(bool IsWithOutBill = false)
        {
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (ReviewSettlementContent == null || ReviewSettlementContent.PatientSettlementList == null || ReviewSettlementContent.PatientSettlementList.Count != 1)
            {
                return;
            }
            if (CurRegistration == null || CurRegistration.PtRegistrationID == 0)
            {
                return;
            }
            if (CurRegistration.InPtAdmissionStatus == 4)
            {
                MessageBox.Show(eHCMSResources.Z2797_G1_KhongTheHuyQTNeuDaXNXuatVien);
                return;
            }
            var mThread = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteTransactionFinalization(ReviewSettlementContent.PatientSettlementList.First().FinalizedReceiptNum, Globals.LoggedUserAccount.StaffID.Value, (long)AllLookupValues.RegistrationType.NOI_TRU, null, IsWithOutBill, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string mOutMessage = contract.EndDeleteTransactionFinalization(asyncResult);
                            if (!string.IsNullOrEmpty(mOutMessage))
                            {
                                MessageBox.Show(mOutMessage, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            ReviewSettlementContent.GetPatientSettlement(CurRegistration.PtRegistrationID, (long)CurRegistration.V_RegistrationType);
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        //▲==== #006
        //▼==== #007
        private DateTime _FromDate = Globals.GetCurServerDateTime();
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate = Globals.GetCurServerDateTime();
        public DateTime ToDate
        {
            get => _ToDate;
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        private string _OutputErrorMessage;
        public string OutputErrorMessage
        {
            get => _OutputErrorMessage; set
            {
                _OutputErrorMessage = value;
                NotifyOfPropertyChange(() => OutputErrorMessage);
            }
        }
        private string gIAPISendHIReportAddress = Globals.ServerConfigSection.CommonItems.APISendHIReportAddress;
        private string gIAPISendHIReportAddressParams = "api/egw/guiHoSoGiamDinh4210?token={0}&id_token={1}&username={2}&password={3}&loaiHoSo=3&maTinh={4}&maCSKCB={5}";
        private string GetErrorMessageFromErrorCode(int aErrorCode)
        {
            switch (aErrorCode)
            {
                case 1001:
                    return "Kích thước file quá lớn (20MB trong khoảng thời gian từ 8 giờ đến 11 giờ và từ 14 giờ đến 19 giờ từ thứ 2 đến thứ 6,100MB với các thời gian khác)";
                case 205:
                    return "Lỗi sai định dạng tham số";
                case 401:
                    return "Sai tài khoản hoặc mật khẩu";
                case 123:
                    return "Sai định dạng file";
                case 124:
                    return "Lỗi khi lưu dữ liệu( file sẽ được tự động gửi lại)";
                case 701:
                    return "Lỗi thời gian gửi,thời gian quyết toán chỉ dc trong tháng hoặc đến mồng 5 tháng sau";
                case 500:
                    return "Lỗi trong quá trình gửi dữ liệu.Vui lòng liên hệ với nhân viên hỗ trợ để được hướng dẫn cụ thể";
                default:
                    return "";
            }
        }
        private void CreateHIReportOutInPtFileTask(GenericCoRoutineTask genTask, object aRegistrationIDList)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport { Title = string.Format("BC-{0}", aRegistrationIDList.ToString()), 
                            RegistrationIDList = aRegistrationIDList.ToString(), V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID }, 
                            V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Pending, FromDate = FromDate, ToDate = ToDate };
                        contract.BeginCreateHIReportOutInPt(mHealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    long mHIReportOutPt;
                                    var mResultVal = contract.EndCreateHIReportOutInPt(out mHIReportID, out mHIReportOutPt, asyncResult);
                                    if (mResultVal || mHIReportID > 0 || mHIReportOutPt > 0)
                                    {
                                        if (mHIReportID > 0)
                                        {
                                            mHealthInsuranceReport.HIReportID = mHIReportID;
                                        }
                                        if (mHIReportOutPt > 0)
                                        {
                                            mHealthInsuranceReport.HIReportOutPt = mHIReportOutPt;
                                        }
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        OutputErrorMessage += Environment.NewLine + eHCMSResources.Z2334_G1_KhongTaoDuocBaoCao;
                                        void onInitDlg(IErrorBold confDlg)
                                        {
                                            confDlg.ErrorTitle = "Lỗi khi đẩy cổng tự động";
                                            confDlg.isCheckBox = false;
                                            confDlg.SetMessage(OutputErrorMessage, "");
                                            confDlg.FireOncloseEvent = true;
                                        }
                                        GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                                        //ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Try => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    OutputErrorMessage = ex.Message;
                                    void onInitDlg(IErrorBold confDlg)
                                    {
                                        confDlg.ErrorTitle = "Lỗi khi đẩy cổng tự động";
                                        confDlg.isCheckBox = false;
                                        confDlg.SetMessage(OutputErrorMessage, "");
                                        confDlg.FireOncloseEvent = true;
                                    }
                                    GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                                    //ClientLoggerHelper.LogInfo(string.Format("CreateHIReportFileTask Catch => {0}", OutputErrorMessage));
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void CreateHIReportXmlTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport9324_AllTab123_InOneRpt((aHealthInsuranceReport as HealthInsuranceReport).HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var ReportStream = contract.EndGetHIXmlReport9324_AllTab123_InOneRpt(asyncResult);
                                    string mHIAPICheckHICardAddress = string.Format(gIAPISendHIReportAddressParams, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password, Globals.ServerConfigSection.Hospitals.HospitalCode.Length < 2 ? "" : Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0, 2), Globals.ServerConfigSection.Hospitals.HospitalCode);
                                    string mRestJson = GlobalsNAV.GetRESTServiceJSon(gIAPISendHIReportAddress, mHIAPICheckHICardAddress, ReportStream);
                                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = GlobalsNAV.ConvertJsonToObject<HIAPIUploadHIReportXmlResult>(mRestJson);
                                    if (mHIAPIUploadHIReportXmlResult.maKetQua == 200)
                                    {
                                        genTask.AddResultObj(mHIAPIUploadHIReportXmlResult);
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        var mErrorMessage = string.IsNullOrEmpty(mHIAPIUploadHIReportXmlResult.maGiaoDich) ? GetErrorMessageFromErrorCode(mHIAPIUploadHIReportXmlResult.maKetQua) : mHIAPIUploadHIReportXmlResult.maGiaoDich;
                                        if (!string.IsNullOrEmpty(mErrorMessage))
                                        {
                                            mErrorMessage = string.Format(" - {0}", mErrorMessage);
                                        }
                                        OutputErrorMessage += Environment.NewLine + string.Format("{0}: {1}{2}", eHCMSResources.T0074_G1_I, mHIAPIUploadHIReportXmlResult.maKetQua, mErrorMessage);
                                        void onInitDlg(IErrorBold confDlg)
                                        {
                                            confDlg.ErrorTitle = eHCMSResources.K1576_G1_CBao;
                                            confDlg.isCheckBox = false;
                                            confDlg.SetMessage(OutputErrorMessage, "");
                                            confDlg.FireOncloseEvent = true;
                                        }
                                        GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                                        //ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Else => {0}", OutputErrorMessage));
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Catch => {0}", ex.Message));
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void UpdateHIReportTask(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateHIReportStatus(aHealthInsuranceReport as HealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndUpdateHIReportStatus(asyncResult))
                                    {
                                        genTask.ActionComplete(true);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private IEnumerator<IResult> CreateHIReportOutInPtXml_Routine()
        {
            if (CurRegistration == null || CurRegistration.HIReportID != 0) yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            string mTitle = string.Format("{0}-{1}", (byte)AllLookupValues.RegTypeID.NOI_TRU, CurRegistration.PtRegistrationID);

            var mCreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReportOutInPtFileTask, mTitle);
            yield return mCreateHIReportFileTask;

            HealthInsuranceReport mHealthInsuranceReport = mCreateHIReportFileTask.GetResultObj(0) as HealthInsuranceReport;

            if (mHealthInsuranceReport != null)
            {
                if (mHealthInsuranceReport.HIReportID > 0)
                {
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    CurRegistration.HIReportID = mHealthInsuranceReport.HIReportID;
                    CurRegistration.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                    CurRegistration.V_ReportStatus = new Lookup();
                    CurRegistration.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                    CurRegistration.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                }
                if (mHealthInsuranceReport.HIReportOutPt > 0)
                {
                    mHealthInsuranceReport.HIReportID = mHealthInsuranceReport.HIReportOutPt;
                    var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXmlTask, mHealthInsuranceReport);
                    yield return mCreateHIReportXmlTask;

                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
                    mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
                    mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
                    mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

                    var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
                    yield return mUpdateHIReportTask;

                    CurRegistration.HIReportID = mHealthInsuranceReport.HIReportID;
                    CurRegistration.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
                    CurRegistration.V_ReportStatus = new Lookup();
                    CurRegistration.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
                    CurRegistration.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
                }
            }
            MessageBox.Show(eHCMSResources.K0461_G1_XNhanBHOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            this.HideBusyIndicator();
        }
        //▲==== #007
    }
}

/****

    public void Handle(ResultFound<Patient> message)
    {
        if (message != null)
        {
            CurPatient = message.Result;
            if (CurPatient != null)
            {
                Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurPatient });
            }
        }
    }

    public void Handle(ItemSelected<Patient> message)
    {
        if (message != null)
        {
            CurPatient = message.Item;
            if (CurPatient != null)
            {
                SetCurrentPatient(CurPatient);
            }
        }
    }

    public void Handle(CreateNewPatientEvent message)
    {
        if (message != null)
        {
            var vm = Globals.GetViewModel<IPatientDetails>();
            vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
            vm.CreateNewPatient();
            Globals.ShowDialog(vm as Conductor<object>);
        }
    }

    public void Handle(ResultNotFound<Patient> message)
    {
        if (message != null)
        {
            //Thông báo không tìm thấy bệnh nhân.
            MessageBoxResult result = MessageBox.Show("Không tìm thấy bệnh nhân. Bạn có muốn thêm bệnh nhân này?",
                                                        "eHCMS says", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                var vm = Globals.GetViewModel<IPatientDetails>();
                var criteria = message.SearchCriteria as PatientSearchCriteria;
                vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                vm.CreateNewPatient();
                if (criteria != null)
                {
                    vm.CurrentPatient.FullName = criteria.FullName;
                }
                Globals.ShowDialog(vm as Conductor<object>);
            }
        }
    }

    public void Handle(PayForRegistrationCompleted message)
    {
        if (message != null)
        {
            //Load lai dang ky:
            var payment = message.Payment;
            if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
            {
                if (payment.PtTranPaymtID > 0 && payment.PayAmount != 0 && message.ObjectState is long)
                {
                    //Show Report:
                    var reportVm = Globals.GetViewModel<IPaymentReport>();
                    reportVm.PaymentID = payment.PtTranPaymtID;
                    reportVm.FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
                    reportVm.CashAdvanceID = (long)message.ObjectState;
                    Globals.ShowDialog(reportVm as Conductor<object>);
                }

                OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
            }
        }
    }
 
    public void SetCurrentPatient(object patient)
    {
        Patient p = patient as Patient;
        if (p == null || p.PatientID <= 0)
        {
            return;
        }
        ConfirmedHiItem = null;
        ConfirmedPaperReferal = null;

        if (p.PatientID > 0)
        {
            GetPatientByID(p.PatientID);
        }
        else
        {
            CurPatient = null;
            CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        }
    }

    private void GetPatientByID(long patientID)
    {
        PatientLoading = true;
        var t = new Thread(() =>
        {
            Globals.EventAggregator.Publish(new BusyEvent
            {
                IsBusy = true,
                Message = eHCMSResources.Z0119_G1_DangLayTTinBN
            });
            try
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetPatientByID(patientID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var patient = contract.EndGetPatientByID(asyncResult);
                                CurPatient = patient;

                                PatientLoaded = true;
                                PatientLoading = false;
                                InitRegistrationForPatient();
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
 
    private bool _patientSearching = false;
    /// <summary>
    /// Dang trong qua trinh tim kiem benh nhan co hay khong.
    /// </summary>
    public bool PatientSearching
    {
        get
        {
            return _patientSearching;
        }
        set
        {
            _patientSearching = value;
            NotifyOfPropertyChange(() => PatientSearching);

            NotifyWhenBusy();
        }
    }

    private bool _isPaying;
    public bool IsPaying
    {
        get
        {
            return _isPaying;
        }
        set
        {
            _isPaying = value;
            NotifyOfPropertyChange(() => IsPaying);

            NotifyWhenBusy();
        }
    }

    private void NotifyWhenBusy()
    {
        NotifyOfPropertyChange(() => IsProcessing);
        NotifyButtonBehaviourChanges();
    }

    public void InitRegistrationForPatient()
    {
        if (_curPatient == null)
            return;

        if (_curPatient.LatestRegistration_InPt == null || (//_curPatient.LatestRegistration_InPt.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU_NOI_TRU &&
            _curPatient.LatestRegistration_InPt.V_RegistrationType != AllLookupValues.RegistrationType.NOI_TRU)) //Chưa có đăng ký lần nào hoac khong phai dang ky noi tru.
        {
            MessageBox.Show("Chưa có đăng ký Nội Trú");
            ResetView();
            return;
        }
        //Neu la dang ky noi tru. 

        if (_curPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED)
        // || _curPatient.LatestRegistration_InPt.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING)
        {
            //Mở đăng ký còn đang sử dụng
            OpenRegistration(_curPatient.LatestRegistration_InPt.PtRegistrationID);
        }
        else
        {
            MessageBox.Show("Không thể mở đăng ký này để tính tiền.");
        }
    }


*****/

/// <summary>
/// Tinh tong so tien BN phai tra (cho cac item duoc chon)
/// </summary>
//private void CalcPatientPayment()
//{
//if (BillingInvoices.Count == 0)
//{
//    MinimumToPay = 0;
//}
//else
//{
//    MinimumToPay = BillingInvoices.Where(item => item.PaidTime == null).Sum(obj => obj.TotalPatientPayment);
//}

////Truong hop so tien bn tra dang du ra:
//if (DebtRemaining < 0)
//{
//    //MinimumToPay = MinimumToPay + DebtRemaining;
//    //if (MinimumToPay < 0)
//    //{
//    //    MinimumToPay = 0;
//    //}
//    MinimumToPay = 0;
//    CurrentPayment.PayAmount = MinimumToPay;
//}
//else //Truong hop bn con no
//{
//    if (AutoPay)
//    {
//        //KMx: Trước đây Set liên tục 2 lần. Thấy vô lý nên bỏ set đầu tiên ra (10/10/2014 14:20). 
//        //CurrentPayment.PayAmount = MinimumToPay;
//        CurrentPayment.PayAmount = DebtRemaining;
//    }
//    else
//    {
//        CurrentPayment.PayAmount = MinimumToPay + DebtRemaining;

//        if (MinimumToPay > 0 && DebtRemaining > MinimumToPay)
//        {
//            CurrentPayment.PayAmount = MinimumToPay;
//        }
//        else
//        {
//            MinimumToPay = DebtRemaining;
//            CurrentPayment.PayAmount = DebtRemaining;
//        }
//    }
//}


//if (MinimumToPay > 0 && DebtRemaining > MinimumToPay)
//{
//    CurrentPayment.PayAmount = MinimumToPay;
//}
//else
//{
//    CurrentPayment.PayAmount = DebtRemaining;
//}
//}