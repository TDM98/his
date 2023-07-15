using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using System.ComponentModel;
using System.Linq;
using eHCMSLanguage;
using static aEMR.Infrastructure.Events.TransferFormEvent;
using System.Threading;
using aEMR.ServiceClient;
using System.Windows.Controls;
using aEMR.Controls;
using System.Windows.Input;
/*
* 20170927 #001 CMN: Added DeadReason
* 
* 11/01/2021 DatTB --001: Thay đoạn này để gán cố định với khoa phòng User đăng nhập đã chọn làm việc. Yêu cầu của anh ThangNHX
* 20220909 #002 DatTB: Thêm event load "Lời dặn" qua tab Xuất viện
* 20230411 #003 QTD: Thêm dữ liệu bảng 7
* 20230517 #004 DatTB: Khóa/mở nút giấy chuyển tuyến theo loại khám
* 20230519 #005 QTD: Thêm thông tin liên hệ phụ vào lời dặn
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IDischargeInfo)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DischargeInfoViewModel : Conductor<object>, IDischargeInfo
        , IHandle<ItemSelected<DiseasesReference>>
        , IHandle<DischargeConditionChange>
        , IHandle<OnChangedPaperReferal>
        , IHandle<OnChangedUpdatePrescription> // #002
        , IHandle<SelectedTreatmentDischarge_Event> // #003
    {

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public DischargeInfoViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            var vm = Globals.GetViewModel<IEnumListing>();
            vm.EnumType = typeof(AllLookupValues.DischargeCondition);
            vm.AddSelectOneItem = true;
            vm.LoadData();
            DischargeConditionContent = vm;

            /*▼====: #001*/
            var vm3 = Globals.GetViewModel<IEnumListing>();
            vm3.EnumType = typeof(AllLookupValues.DeadReason);
            vm3.AddSelectOneItem = true;
            vm3.LoadData();
            DeadReasonContent = vm3;
            /*▲====: #001*/

            var vm2 = Globals.GetViewModel<IEnumListing>();
            vm2.EnumType = typeof(AllLookupValues.CategoryOfDecease);
            vm2.AddSelectOneItem = true;
            vm2.LoadData();
            ReasonOfDeceasedContent = vm2;
            (ReasonOfDeceasedContent as INotifyPropertyChangedEx).PropertyChanged += ReasonOfDeceasedContent_PropertyChanged;

            DischargeTypeContent = Globals.GetViewModel<IEnumListing>();
            DischargeTypeContent.EnumType = typeof(AllLookupValues.V_DischargeType);
            DischargeTypeContent.AddSelectOneItem = true;
            DischargeTypeContent.LoadData();

            (DischargeTypeContent as PropertyChangedBase).PropertyChanged += new PropertyChangedEventHandler(DischargeTypeContent_PropertyChanged);


            //refIDC10 = new PagedSortableCollectionView<DiseasesReference>();
            //refIDC10.OnRefresh += new EventHandler<RefreshEventArgs>(refIDC10_OnRefresh);
            //refIDC10.PageSize = Globals.PageSize;

            MainReasonOfDeceasedInfoContent = Globals.GetViewModel<Iicd10Listing>();
            PostMorternInfoContent = Globals.GetViewModel<Iicd10Listing>();

            DateOfDeceaseContent = Globals.GetViewModel<IMinHourDateControl>();
            if (DeceasedInfo != null)
            {
                DateOfDeceaseContent.DateTime = DeceasedInfo.DeceasedDateTime;
            }
            else
            {
                DateOfDeceaseContent.DateTime = null;
            }
            (DateOfDeceaseContent as INotifyPropertyChangedEx).PropertyChanged += DateOfDeceaseContent_PropertyChanged;

            DischargeDateContent = Globals.GetViewModel<IMinHourDateControl>();

            //KMx: Mặc định để trống, để user tự nhập vào (03/06/2015 15:43).
            DischargeDateContent.DateTime = null;

            PatientAllocListingContent = Globals.GetViewModel<IInPatientBedPatientAllocListing>();
            //PatientAllocListingContent.ShowDeleteColumn = false;
            InPatientDeptListingContent = Globals.GetViewModel<IInPatientDeptListing>();

            //KMx: Chuyển từ View IInPatientBillingInvoiceListing -> IInPatientBillingInvoiceListingNew (13/09/2014 17:11).
            //var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListing>();
            var billingVm = Globals.GetViewModel<IInPatientBillingInvoiceListingNew>();
            BillingInvoiceListingContent = billingVm;
            BillingInvoiceListingContent.ShowEditColumn = false;
            BillingInvoiceListingContent.ShowInfoColumn = false;
            BillingInvoiceListingContent.ShowRecalcHiColumn = false;
            BillingInvoiceListingContent.ShowRecalcHiWithPriceListColumn = false;
            BillingInvoiceListingContent.mExpanderDetail = false;
            BillingInvoiceListingContent.mDangKyNoiTru_XemChiTiet = false;
            BillingInvoiceListingContent.mDangKyNoiTru_SuaDV = false;
            BillingInvoiceListingContent.mTickSelect = false;

            //BillingInvoiceListingContent.InvoiceDetailsContent.ShowDeleteColumn = false;

            //▼====: #003
            DateOfPregnancyTermination = Globals.GetViewModel<IMinHourDateControl>();
            if (Registration != null && Registration.AdmissionInfo != null
                && Registration.AdmissionInfo.DischargePapersInfo != null && Registration.AdmissionInfo.DischargePapersInfo.PregnancyTerminationDateTime != null)
            {
                DateOfPregnancyTermination.DateTime = Registration.AdmissionInfo.DischargePapersInfo.PregnancyTerminationDateTime;
            }
            else
            {
                DateOfPregnancyTermination.DateTime = null;
            }
            (DateOfPregnancyTermination as INotifyPropertyChangedEx).PropertyChanged += DateOfPregnancyTermination_PropertyChanged;
            LoadDoctorStaffCollection();
            //▲====: #003
            GetRespDepartments();

        }

        private bool _CanEdit;
        public bool CanEdit
        {
            get
            {
                return _CanEdit;
            }
            set
            {
                if (_CanEdit == value)
                {
                    return;
                }
                _CanEdit = value;
                NotifyOfPropertyChange(() => CanEdit);
            }
        }
        private bool _btGCTIsEnabled = false;
        public bool btGCTIsEnabled
        {
            get
            {
                return _btGCTIsEnabled;
            }
            set
            {
                _btGCTIsEnabled = value;
                NotifyOfPropertyChange(() => btGCTIsEnabled);
            }
        }
        private bool _IsConsultation;
        public bool IsConsultation
        {
            get
            {
                return _IsConsultation;
            }
            set
            {
                if (_IsConsultation == value)
                {
                    return;
                }
                _IsConsultation = value;
                NotifyOfPropertyChange(() => IsConsultation);
            }
        }

        private void SetCanEdit()
        {
            if (IsConsultation)
            {
                CanEdit = true;
            }
            else
            {
                if (Registration == null || Registration.AdmissionInfo == null)
                {
                    CanEdit = false;
                    return;
                }

                if (Registration.AdmissionInfo.IsDoctorCreatedDischargePaper)
                {
                    CanEdit = false;
                    //KMx: Mỗi lần load BN lên để xuất viện mà cứ hiện thông báo hoài (12/06/2015 15:24).
                    //if (Registration.AdmissionInfo.DischargeDate == null)
                    //{
                    //    MessageBox.Show("Bác sĩ đã làm giấy xuất viện cho bệnh nhân, bạn chỉ được xác nhận xuất viện mà không được chỉnh sửa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    //}
                }
                else
                {
                    CanEdit = true;
                }
            }
        }

        private bool _ConfirmNotTreatedAsInPt_Enable;
        public bool ConfirmNotTreatedAsInPt_Enable
        {
            get
            {
                return _ConfirmNotTreatedAsInPt_Enable;
            }
            set
            {
                _ConfirmNotTreatedAsInPt_Enable = value;
                NotifyOfPropertyChange(() => ConfirmNotTreatedAsInPt_Enable);
            }
        }

        private void DischargeTypeContent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedItem")
            {
                return;
            }
            if (DischargeTypeContent.SelectedItem == null)
            {
                return;
            }
            if (DischargeTypeContent.SelectedItem.EnumValue == AllLookupValues.V_DischargeType.RA_VIEN.ToString())
            {
                ConfirmNotTreatedAsInPt_Enable = true;
            }
            else
            {
                if (Registration != null && Registration.AdmissionInfo != null)
                {
                    Registration.AdmissionInfo.ConfirmNotTreatedAsInPt = false;
                }
                ConfirmNotTreatedAsInPt_Enable = false;
            }
            btGCTIsEnabled = DischargeTypeContent.SelectedItem.EnumValue == AllLookupValues.V_DischargeType.CHUYEN_TUYEN_CHUYEN_MON.ToString()
                || DischargeTypeContent.SelectedItem.EnumValue == AllLookupValues.V_DischargeType.CHUYEN_VIEN_NGUOI_BENH.ToString();
        }

        public void SetDischargeDeptSelection(List<RefDepartment> curInPtDepts, bool enableAllRespDepts)
        {
            if (curInPtDepts == null)
            {
                RespDepartments = null;
                return;
            }
            if (enableAllRespDepts)
            {
                RespDepartments = curInPtDepts.ToObservableCollection();
            }
            else
            {
                RespDepartments = curInPtDepts.Where(item => item.DeptID == Globals.ObjRefDepartment.DeptID).ToObservableCollection();
                //--▼--001--11/01/2021 DatTB 
                //RespDepartments = curInPtDepts.Where(item => Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(item.DeptID)).ToObservableCollection();
                //--▲--001--11/01/2021 DatTB 
            }
        }

        private void GetRespDepartments()
        {
            if (Globals.LoggedUserAccount.DeptIDResponsibilityList.Count() > 0)
            {
                RespDepartments = Globals.AllRefDepartmentList.Where(item => Globals.LoggedUserAccount.DeptIDResponsibilityList.Contains(item.DeptID)).ToObservableCollection();
            }
            else
            {
                // TxD 07/12/2014: This must be Administrator User , if WRONG then we have to review this code AGIAN                   
                RespDepartments = Globals.AllRefDepartmentList.Where(item => item.IsDeleted == false).ToObservableCollection();
            }

            RefDepartment firstItem = new RefDepartment();
            firstItem.DeptID = 0;
            firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0493_G1_HayChonKhoa);
            RespDepartments.Insert(0, firstItem);

            if (RespDepartments.Count() > 0)
            {
                SelectedDischargeDepartment = RespDepartments[0];

                if (Globals.ObjRefDepartment != null && Globals.ObjRefDepartment.DeptID > 0)
                {
                    foreach (var deptItem in RespDepartments)
                    {
                        if (deptItem.DeptID == Globals.ObjRefDepartment.DeptID)
                        {
                            SelectedDischargeDepartment = deptItem;
                            break;
                        }
                    }
                }
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

        private RefDepartment _SelectedDischargeDepartment;
        public RefDepartment SelectedDischargeDepartment
        {
            get
            {
                return _SelectedDischargeDepartment;
            }
            set
            {
                _SelectedDischargeDepartment = value;
                NotifyOfPropertyChange(() => SelectedDischargeDepartment);
            }
        }



        //void refIDC10_OnRefresh(object sender, RefreshEventArgs e)
        //{
        //    LoadRefDiseases(Name, Type, refIDC10.PageIndex, refIDC10.PageSize);
        //}

        void ReasonOfDeceasedContent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.DeceasedInfo != null)
                {
                    //Registration.AdmissionInfo.DeceasedInfo.V_CategoryOfDecease = (AllLookupValues.CategoryOfDecease)ReasonOfDeceasedContent.SelectedItem.EnumItem;
                    Registration.AdmissionInfo.DeceasedInfo.V_CategoryOfDecease = ReasonOfDeceasedContent.SelectedItem.EnumItem != null ? (AllLookupValues.CategoryOfDecease)ReasonOfDeceasedContent.SelectedItem.EnumItem : 0;
                }
            }
        }

        void DateOfDeceaseContent_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DateTime")
            {
                if (DeceasedInfo != null)
                {
                    DeceasedInfo.DeceasedDateTime = DateOfDeceaseContent.DateTime.GetValueOrDefault(DateTime.MinValue);
                }
            }
        }


        private bool _isNotPayment;
        public bool isNotPayment
        {
            get { return _isNotPayment; }
            set
            {
                _isNotPayment = value;
                NotifyOfPropertyChange(() => isNotPayment);
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

        private IEnumListing _dischargeConditionContent;
        public IEnumListing DischargeConditionContent
        {
            get { return _dischargeConditionContent; }
            set
            {
                _dischargeConditionContent = value;
                NotifyOfPropertyChange(() => DischargeConditionContent);
            }
        }

        private Iicd10Listing _postMorternInfoContent;
        public Iicd10Listing PostMorternInfoContent
        {
            get { return _postMorternInfoContent; }
            set
            {
                _postMorternInfoContent = value;
                NotifyOfPropertyChange(() => PostMorternInfoContent);
            }
        }

        private Iicd10Listing _mainReasonOfDeceasedInfoContent;
        public Iicd10Listing MainReasonOfDeceasedInfoContent
        {
            get { return _mainReasonOfDeceasedInfoContent; }
            set
            {
                _mainReasonOfDeceasedInfoContent = value;
                NotifyOfPropertyChange(() => MainReasonOfDeceasedInfoContent);
            }
        }

        private IEnumListing _reasonOfDeceasedContent;
        public IEnumListing ReasonOfDeceasedContent
        {
            get { return _reasonOfDeceasedContent; }
            set
            {
                _reasonOfDeceasedContent = value;
                NotifyOfPropertyChange(() => ReasonOfDeceasedContent);
            }
        }

        private IMinHourDateControl _dateOfDeceaseContent;

        public DateTime? DateOfDecease
        {
            get
            {
                if (DeceasedInfo != null)
                {
                    return DeceasedInfo.DeceasedDateTime;
                }
                return null;
            }
            set
            {
                if (DateOfDeceaseContent != null)
                {
                    DateOfDeceaseContent.DateTime = value;
                }
            }
        }

        public IMinHourDateControl DateOfDeceaseContent
        {
            get { return _dateOfDeceaseContent; }
            set
            {
                _dateOfDeceaseContent = value;
                NotifyOfPropertyChange(() => DateOfDeceaseContent);
            }
        }

        private ObservableCollection<DiagnosisIcd10Items> _allExtraDiagnosisIcd10Items;

        public ObservableCollection<DiagnosisIcd10Items> AllExtraDiagnosisIcd10Items
        {
            get { return _allExtraDiagnosisIcd10Items; }
            set
            {
                _allExtraDiagnosisIcd10Items = value;
                NotifyOfPropertyChange(() => AllExtraDiagnosisIcd10Items);
            }
        }

        private PatientRegistration _registration;
        public PatientRegistration Registration
        {
            get { return _registration; }
            set
            {
                if (_registration != value)
                {
                    _registration = value;
                    //▼==== #005
                    if (_registration != null && _registration.Patient != null
                        && _registration.Patient.Age < 16
                        && _registration.AdmissionInfo != null
                        && string.IsNullOrEmpty(_registration.AdmissionInfo.Comment))
                    {
                        _registration.AdmissionInfo.Comment = GetFContact(_registration.Patient);
                    }
                    //▲==== #005
                    NotifyOfPropertyChange(() => Registration);

                    if (_registration == null)
                    {
                        PatientAllocListingContent.Registration = null;
                        InPatientDeptListingContent.AdmissionInfo = null;
                        BillingInvoiceListingContent.BillingInvoices = null;
                        Reset();
                        return;
                    }

                    if (CurrentTransferForm.CurPatientRegistration != null && _registration.PatientID != CurrentTransferForm.CurPatientRegistration.PatientID)
                    {
                        CurrentTransferForm = new TransferForm();
                    }
                    PatientAllocListingContent.Registration = _registration;
                    InPatientDeptListingContent.AdmissionInfo = _registration.AdmissionInfo;
                    BillingInvoiceListingContent.BillingInvoices = _registration.InPatientBillingInvoices;

                    if (_registration.AdmissionInfo != null)
                    {
                        if (_registration.AdmissionInfo.DischargeDepartment != null)
                        {
                            SelectedDischargeDepartment = _registration.AdmissionInfo.DischargeDepartment;
                        }
                        else
                        {
                            SelectedDischargeDepartment = RespDepartments[0];
                        }

                        DeceasedInfo = _registration.AdmissionInfo.DeceasedInfo;
                        if (DeceasedInfo != null)
                        {
                            ReasonOfDeceasedContent.SetSelectedID(DeceasedInfo.V_CategoryOfDecease.ToString());
                            HasDeceaseInfo = true;
                            MainReasonOfDeceasedInfoContent.SetText(DeceasedInfo.MainCauseOfDeceaseCode);
                            PostMorternInfoContent.SetText(DeceasedInfo.PostMortemExamCode);
                            IsMortem = DeceasedInfo.IsPostMorternExam.GetValueOrDefault(false);
                        }
                        else
                        {
                            HasDeceaseInfo = false;
                            MainReasonOfDeceasedInfoContent.SetText(string.Empty);
                            PostMorternInfoContent.SetText(string.Empty);
                            IsMortem = false;
                        }

                        DateOfDeceaseContent.DateTime = DateOfDecease;

                        //▼==== #003
                        if(_registration.AdmissionInfo.DischargePapersInfo != null)
                        {
                            DateOfPregnancyTermination.DateTime = _registration.AdmissionInfo.DischargePapersInfo.PregnancyTerminationDateTime != null
                                ? _registration.AdmissionInfo.DischargePapersInfo.PregnancyTerminationDateTime : null;
                            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == _registration.AdmissionInfo.DischargePapersInfo.HeadOfDepartmentDoctorStaffID) : null;
                            gSelectedUnitLeaderDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == _registration.AdmissionInfo.DischargePapersInfo.UnitLeaderDoctorStaffID) : null;
                            IsPregnancyTermination = _registration.AdmissionInfo.DischargePapersInfo.IsPregnancyTermination;
                            FromDate = _registration.AdmissionInfo.DischargePapersInfo.FromDateLeaveForTreatment;
                            ToDate = _registration.AdmissionInfo.DischargePapersInfo.ToDateLeaveForTreatment;
                            if(_registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment > 0)
                            {
                                IsLeaveForTreatment = true;
                            }
                        }
                        else
                        {
                            _registration.AdmissionInfo.DischargePapersInfo = new DischargePapersInfo();
                            _registration.AdmissionInfo.DischargePapersInfo.DoctorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                            FromDate = null;
                            ToDate = null;
                            IsLeaveForTreatment = false;
                            IsPregnancyTermination = false;
                            gSelectedDoctorStaff = null;
                            gSelectedUnitLeaderDoctorStaff = null;
                            DateOfPregnancyTermination.DateTime = null;
                        }
                        //▲==== #003
                    }
                    else
                    {
                        Reset();
                    }

                    SetCanEdit();
                    NotifyOfPropertyChange(() => HasPregnancyTermination);
                }
            }
        }

        private IEnumListing _dischargeTypeContent;

        public IEnumListing DischargeTypeContent
        {
            get { return _dischargeTypeContent; }
            set
            {
                _dischargeTypeContent = value;
                NotifyOfPropertyChange(() => DischargeTypeContent);
            }
        }

        private IMinHourDateControl _dischargeDateContent;

        public IMinHourDateControl DischargeDateContent
        {
            get { return _dischargeDateContent; }
            set
            {
                _dischargeDateContent = value;
                NotifyOfPropertyChange(() => DischargeDateContent);
            }
        }

        private DeceasedInfo _deceasedInfo;
        public DeceasedInfo DeceasedInfo
        {
            get { return _deceasedInfo; }
            set
            {
                _deceasedInfo = value;
                NotifyOfPropertyChange(() => DeceasedInfo);
            }
        }

        private bool _isMortem;
        public bool IsMortem
        {
            get { return _isMortem; }
            set
            {
                _isMortem = value;
                NotifyOfPropertyChange(() => IsMortem);
            }
        }

        public void PayCmd()
        {
            //SetHyperlinkSelectedStyle(source as HyperlinkButton);
            var registrationLeftMenu = Globals.GetViewModel<IRegistrationLeftMenu>();
            registrationLeftMenu.InPatientProcessPaymentCmd(this);
            Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = Registration });

        }

        #region Thông tin nhập viện
        private IInPatientBedPatientAllocListing _patientAllocListingContent;
        public IInPatientBedPatientAllocListing PatientAllocListingContent
        {
            get { return _patientAllocListingContent; }
            set
            {
                _patientAllocListingContent = value;
                NotifyOfPropertyChange(() => PatientAllocListingContent);
            }
        }

        private IInPatientDeptListing _inPatientDeptListingContent;
        public IInPatientDeptListing InPatientDeptListingContent
        {
            get { return _inPatientDeptListingContent; }
            set
            {
                _inPatientDeptListingContent = value;
                NotifyOfPropertyChange(() => InPatientDeptListingContent);
            }
        }

        #endregion
        public void Handle(ItemSelected<DiseasesReference> message)
        {
            if (GetView() != null)
            {
                if (message.Source == MainReasonOfDeceasedInfoContent)
                {
                    if (DeceasedInfo != null)
                    {
                        if (message.Item != null)
                        {
                            DeceasedInfo.MainReasonOfDecease = message.Item.DiseaseNameVN;
                            DeceasedInfo.MainCauseOfDeceaseCode = message.Item.ICD10Code;
                        }
                    }
                }
                else if (message.Source == PostMorternInfoContent)
                {
                    if (DeceasedInfo != null)
                    {
                        if (message.Item != null)
                        {
                            DeceasedInfo.PostMortemExamDiagnosis = message.Item.DiseaseNameVN;
                            DeceasedInfo.PostMortemExamCode = message.Item.ICD10Code;
                        }
                    }
                }
            }
        }

        private bool _hasDeceaseInfo;

        public bool HasDeceaseInfo
        {
            get { return _hasDeceaseInfo; }
            set
            {
                _hasDeceaseInfo = value;
                NotifyOfPropertyChange(() => HasDeceaseInfo);

                if (_hasDeceaseInfo)
                {
                    if (Registration.AdmissionInfo != null && Registration.AdmissionInfo.DeceasedInfo == null)
                    {
                        Registration.AdmissionInfo.DeceasedInfo = new DeceasedInfo();
                        DeceasedInfo = Registration.AdmissionInfo.DeceasedInfo;
                    }
                }
                else
                {
                    if (Registration != null && Registration.AdmissionInfo != null)
                    {
                        Registration.AdmissionInfo.DeceasedInfo = null;
                    }
                }
            }
        }

        public void CreateNew()
        {
            Reset();
            DeceasedInfo = new DeceasedInfo();
        }
        public void Reset()
        {
            DeceasedInfo = null;
            ReasonOfDeceasedContent.SetSelectedID(string.Empty);
        }


        /*▼====: #001*/
        private IEnumListing _DeadReasonContent;
        public IEnumListing DeadReasonContent
        {
            get { return _DeadReasonContent; }
            set
            {
                _DeadReasonContent = value;
                NotifyOfPropertyChange(() => DeadReasonContent);
            }
        }
        private Visibility _IsDead = Visibility.Collapsed;
        public Visibility IsDead
        {
            get { return _IsDead; }
            set
            {
                _IsDead = value;
                NotifyOfPropertyChange(() => IsDead);
            }
        }
        public void Handle(DischargeConditionChange DischargeCondition)
        {
            if (Registration !=null && DischargeCondition != null && DischargeCondition.Item == AllLookupValues.DischargeCondition.TU_VONG)
            {
                IsDead = Visibility.Visible;
                HasDeceaseInfo = true;
            }
            else
            {
                IsDead = Visibility.Collapsed;
                HasDeceaseInfo = false;
            }

        }
        /*▲====: #001*/

        #region Properties
        private bool _gIsReported = false;
        public bool gIsReported
        {
            get
            {
                return _gIsReported;
            }
            set
            {
                if (_gIsReported == value) return;
                _gIsReported = value;
                NotifyOfPropertyChange(() => gIsReported);
            }
        }

        public TransferForm CurrentTransferForm { get; set; } = new TransferForm();
        #endregion

        //private bool IsCode=true;
        //private string Name = "";
        //private byte Type = 0;

        //private PagedSortableCollectionView<DiseasesReference> _refIDC10;
        //public PagedSortableCollectionView<DiseasesReference> refIDC10
        //{
        //    get
        //    {
        //        return _refIDC10;
        //    }
        //    set
        //    {
        //        if (_refIDC10 != value)
        //        {
        //            _refIDC10 = value;
        //        }
        //        NotifyOfPropertyChange(() => refIDC10);
        //    }
        //}

        //public void LoadRefDiseases(string name, byte type, int PageIndex, int PageSize)
        //{
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new CommonUtilsServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginSearchRefDiseases(name, PageIndex, PageSize, type, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    int Total = 10;
        //                    var results = contract.EndSearchRefDiseases(out Total, asyncResult);
        //                    refIDC10.Clear();
        //                    refIDC10.TotalItemCount = Total;
        //                    if (results != null)
        //                    {
        //                        foreach (DiseasesReference p in results)
        //                        {
        //                            refIDC10.Add(p);
        //                        }
        //                    }
        //                    if (_PhanLoai == 0)
        //                    {
        //                        AutoDischargeCode.ItemsSource = refIDC10;
        //                        AutoDischargeCode.PopulateComplete();
        //                    }
        //                    else if (_PhanLoai == 1)
        //                    {
        //                        AutoDischargeCodeName.ItemsSource = refIDC10;
        //                        AutoDischargeCodeName.PopulateComplete();
        //                    }
        //                    else if (_PhanLoai == 2)
        //                    {
        //                        AutoDischargeCode2.ItemsSource = refIDC10;
        //                        AutoDischargeCode2.PopulateComplete();
        //                    }
        //                    else
        //                    {
        //                        AutoDischargeCodeName2.ItemsSource = refIDC10;
        //                        AutoDischargeCodeName2.PopulateComplete();
        //                    }

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {

        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        //AutoCompleteBox AutoDischargeCode = null;
        //AutoCompleteBox AutoDischargeCodeName = null;

        //AutoCompleteBox AutoDischargeCode2 = null;
        //AutoCompleteBox AutoDischargeCodeName2 = null;

        //public void DischargeCode_Populating(object sender, PopulatingEventArgs e)
        //{
        //    if (IsCode)
        //    {
        //        AutoDischargeCode = (AutoCompleteBox)sender;
        //        Name = e.Parameter;
        //        Type = 0;
        //        _PhanLoai = 0;
        //        refIDC10.PageIndex = 0;
        //        LoadRefDiseases(e.Parameter, Type, 0, refIDC10.PageSize);
        //    }
        //}

        //public void DischargeCode_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (AutoDischargeCode == null)
        //    {
        //        AutoDischargeCode = (AutoCompleteBox)sender;
        //    }
        //    if (AutoDischargeCode != null && AutoDischargeCode.SelectedItem != null)
        //    {
        //        Registration.AdmissionInfo.DischargeNote = ((DiseasesReference)AutoDischargeCode.SelectedItem).DiseaseNameVN;
        //    }
        //    else
        //    {
        //        if (Registration.AdmissionInfo != null)
        //        {
        //            Registration.AdmissionInfo.DischargeNote = "";
        //        }
        //    }
        //    GetDiagnosis();
        //}

        //private void GetDiagnosis()
        //{
        //    if (Registration.AdmissionInfo != null)
        //    {
        //        if (string.IsNullOrEmpty(Registration.AdmissionInfo.DischargeNote))
        //        {
        //            Registration.AdmissionInfo.Diagnosis = Registration.AdmissionInfo.DischargeNote2;
        //        }
        //        else if (string.IsNullOrEmpty(Registration.AdmissionInfo.DischargeNote2))
        //        {
        //            Registration.AdmissionInfo.Diagnosis = Registration.AdmissionInfo.DischargeNote;
        //        }
        //        else
        //        {
        //            Registration.AdmissionInfo.Diagnosis = Registration.AdmissionInfo.DischargeNote + "," + Registration.AdmissionInfo.DischargeNote2;
        //        }
        //    }
        //}

        //public void DischargeNote_Populating(object sender, PopulatingEventArgs e)
        //{
        //    if (IsCode)
        //    {
        //        AutoDischargeCodeName = (AutoCompleteBox)sender;
        //        Name = e.Parameter;
        //        Type = 1;
        //        _PhanLoai = 1;
        //        refIDC10.PageIndex = 0;
        //        LoadRefDiseases(e.Parameter, Type, 0, refIDC10.PageSize);
        //    }
        //}

        //public void DischargeNote_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (AutoDischargeCodeName == null)
        //    {
        //        AutoDischargeCodeName = (AutoCompleteBox)sender;
        //    }
        //    if (AutoDischargeCodeName != null && AutoDischargeCodeName.SelectedItem != null)
        //    {
        //        Registration.AdmissionInfo.DischargeCode = ((DiseasesReference)AutoDischargeCodeName.SelectedItem).ICD10Code;
        //    }
        //    else
        //    {
        //        if (Registration.AdmissionInfo != null)
        //        {
        //            Registration.AdmissionInfo.DischargeCode = "";
        //        }
        //    }
        //    GetDiagnosis();
        //}

        //private int _PhanLoai = 0;
        //public void DischargeCode2_Populating(object sender, PopulatingEventArgs e)
        //{
        //    if (IsCode)
        //    {
        //        AutoDischargeCode2 = (AutoCompleteBox)sender;
        //        Name = e.Parameter;
        //        _PhanLoai = 2;
        //        Type = 0;
        //        refIDC10.PageIndex = 0;
        //        LoadRefDiseases(e.Parameter, Type, 0, refIDC10.PageSize);
        //    }

        //}

        //public void DischargeCode2_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (AutoDischargeCode2 == null)
        //    {
        //        AutoDischargeCode2 = (AutoCompleteBox)sender;
        //    }
        //    if (AutoDischargeCode2 != null && AutoDischargeCode2.SelectedItem != null)
        //    {
        //        Registration.AdmissionInfo.DischargeNote2 = ((DiseasesReference)AutoDischargeCode2.SelectedItem).DiseaseNameVN;
        //    }
        //    else
        //    {
        //        if (Registration.AdmissionInfo != null)
        //        {
        //            Registration.AdmissionInfo.DischargeNote2 = "";
        //        }
        //    }
        //    GetDiagnosis();
        //}

        //public void DischargeNote2_Populating(object sender, PopulatingEventArgs e)
        //{
        //    if (IsCode)
        //    {
        //        AutoDischargeCodeName2 = (AutoCompleteBox)sender;
        //        Name = e.Parameter;
        //        _PhanLoai = 3;
        //        Type = 1;
        //        refIDC10.PageIndex = 0;
        //        LoadRefDiseases(e.Parameter, Type, 0, refIDC10.PageSize);
        //    }
        //}

        //public void DischargeNote2_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (AutoDischargeCodeName2 == null)
        //    {
        //        AutoDischargeCodeName2 = (AutoCompleteBox)sender;
        //    }
        //    if (AutoDischargeCodeName2 != null && AutoDischargeCodeName2.SelectedItem != null)
        //    {
        //        Registration.AdmissionInfo.DischargeCode2 = ((DiseasesReference)AutoDischargeCodeName2.SelectedItem).ICD10Code;
        //    }
        //    else
        //    {
        //        if (Registration.AdmissionInfo != null)
        //        {
        //            Registration.AdmissionInfo.DischargeCode2 = "";
        //        }
        //    }
        //    GetDiagnosis();
        //}
        public void btGCT()
        {
            if (Registration.AdmissionInfo.TransferFormID > 0 && CurrentTransferForm.TransferFormID == 0)
            {
                GetTransferFormByID(Registration.AdmissionInfo.TransferFormID);
            }
            else
            {
                GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di, (int)AllLookupValues.PatientFindBy.NOITRU);
            }
        }

        private void GetTransferFormByID(long transferFormID)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTransferFormByID(transferFormID, (int)AllLookupValues.PatientFindBy.NOITRU, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CurrentTransferForm = contract.EndGetTransferFormByID(asyncResult);
                            GetPaPerReferalFul((int)AllLookupValues.V_TransferFormType.CHUYEN_Di, (int)AllLookupValues.PatientFindBy.NOITRU);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {

                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void GetPaPerReferalFul(int V_TransferFormType, int PatientFindBy)
        {
            PatientRegistration CurRegistration = Registration;
            var mEvent = new TransferFormEvent();

            if (CurrentTransferForm != null && CurrentTransferForm.TransferFormID > 0)
            {
                mEvent.Item = CurrentTransferForm;
            }
            else
            {
                mEvent.Item = new TransferForm();

                mEvent.Item.CurPatientRegistration = new PatientRegistration();

                mEvent.Item.TransferFormID = (long)0;
                mEvent.Item.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
                if (CurRegistration.HisID_2 != null)
                    mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
                else if (CurRegistration.HisID != null)
                    mEvent.Item.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
                if (CurRegistration != null)
                {
                    if (CurRegistration.HealthInsurance_2 != null)
                        mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance_2;
                    else if (CurRegistration.HealthInsurance != null)
                        mEvent.Item.CurPatientRegistration.HealthInsurance = CurRegistration.HealthInsurance;
                    if (CurRegistration.Patient != null)
                    {
                        mEvent.Item.CurPatientRegistration.Patient = CurRegistration.Patient;
                    }
                    if (Registration.AdmissionInfo.DiagnosisTreatmentInfo != null)
                    {
                        var DiagTrmtItem = Registration.AdmissionInfo.DiagnosisTreatmentInfo;
                        if (DiagTrmtItem.Diagnosis != null)
                            mEvent.Item.ClinicalSign = DiagTrmtItem.Diagnosis;
                        if (DiagTrmtItem.Treatment != null)
                            mEvent.Item.UsedServicesAndItems = DiagTrmtItem.Treatment;
                        if (DiagTrmtItem.DiagnosisFinal != null && DiagTrmtItem.ICD10List != null)
                        {
                            mEvent.Item.ICD10Final = DiagTrmtItem.ICD10List;
                            mEvent.Item.ICD10 = DiagTrmtItem.ICD10List;
                            mEvent.Item.DiagnosisTreatment_Final = DiagTrmtItem.DiagnosisFinal;
                        }
                        if (DiagTrmtItem.PatientServiceRecord != null && DiagTrmtItem.PatientServiceRecord.ExamDate != null)
                            mEvent.Item.FromDate = DiagTrmtItem.PatientServiceRecord.ExamDate;
                    }

                    mEvent.Item.ToDate = DateTime.Now;
                    mEvent.Item.TransferDate = DateTime.Now;
                    mEvent.Item.TransferFromHos = new Hospital();
                    mEvent.Item.TransferToHos = new Hospital();
                    mEvent.Item.V_TransferTypeID = 62604;           // defalut : chuyễn giữa các cơ sở cùng tuyế 
                    mEvent.Item.V_PatientStatusID = 63002;          //defalut : không cấp cứu
                    mEvent.Item.V_TransferReasonID = 62902;         //default : yêu cầu chuyên môn
                    mEvent.Item.V_TreatmentResultID = 62702;        //defalut : ko thuyên giảm. Nặng lên - V_TreatmentResult_V2 trong bảng lookup
                    mEvent.Item.V_CMKTID = 62801;                   // default : chuyển đúng tuyến, đúng chuyên môn kỹ thuật
                }
                mEvent.Item.PatientFindBy = PatientFindBy;
                mEvent.Item.V_TransferFormType = V_TransferFormType;

            }

            Action<IPaperReferalFull> onInitDlg = delegate (IPaperReferalFull TransferFromVm)
            {
                TransferFromVm.IsThisViewDialog = true;
                TransferFromVm.V_TransferFormType = V_TransferFormType;
                TransferFromVm.V_GetPaperReferalFullFromOtherView = true;
                //▼==== #004
                if (CurRegistration.AdmissionInfo.DischargeDate != null && CurRegistration.AdmissionInfo.DischargeDetailRecCreatedDate != null)
                {
                    TransferFromVm.IsHiReportOrDischarge = true;
                }
                //▲==== #004
                if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
                {
                    TransferFromVm.CurrentTransferForm.CurPatientRegistration = new PatientRegistration();
                    TransferFromVm.CurrentTransferForm.CurPatientRegistration.PtRegistrationID = (long)CurRegistration.PtRegistrationID;
                    if (CurRegistration.HisID_2 != null)
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID_2.Value;
                    else if (CurRegistration.HisID != null)
                        TransferFromVm.CurrentTransferForm.CurPatientRegistration.HisID = (long)CurRegistration.HisID.Value;
                }
                this.ActivateItem(TransferFromVm);
                TransferFromVm.SetCurrentInformation(mEvent);
            };
            GlobalsNAV.ShowDialog<IPaperReferalFull>(onInitDlg);
        }

        public void Handle(OnChangedPaperReferal message)
        {
            CurrentTransferForm = message.TransferForm;
            if (Registration != null && Registration.AdmissionInfo != null)
            {
                Registration.AdmissionInfo.TransferFormID = CurrentTransferForm.TransferFormID;
            }
        }

        //▼==== #002
        public void Handle(OnChangedUpdatePrescription message)
        {
            if (message != null && message.AdmissionInfoComment != null)
            {
                if (Registration != null && Registration.AdmissionInfo != null)
                {
                    Registration.AdmissionInfo.Comment = string.IsNullOrEmpty(Registration.AdmissionInfo.Comment) ? message.AdmissionInfoComment
                        : string.Format("{0}; {1}", Registration.AdmissionInfo.Comment, message.AdmissionInfoComment);
                }
            }
        }
        //▲==== #002
        private bool _ApplyReport130 = Globals.ServerConfigSection.CommonItems.ApplyReport130;
        public bool ApplyReport130
        {
            get
            {
                return _ApplyReport130;
            }
            set
            {
                if (_ApplyReport130 == value)
                {
                    return;
                }
                _ApplyReport130 = value;
                NotifyOfPropertyChange(() => ApplyReport130);
            }
        }

        //▼==== #003
        private bool _IsPregnancyTermination;

        public bool IsPregnancyTermination
        {
            get { return _IsPregnancyTermination; }
            set
            {
                if(_IsPregnancyTermination != value)
                {
                    //if (value)
                    //{
                    //    if (DateOfDeceaseContent.DateTime == null)
                    //    {
                    //        MessageBox.Show("Vui lòng chọn ngày giờ đình chỉ thai nghén!");
                    //        return;
                    //    }
                    //    if (Registration.AdmissionInfo != null)
                    //    {
                    //        Registration.AdmissionInfo.TreatmentDischarge = string.Format("{0} vào {1}", eHCMSResources.Z3311_G1_DinhChiThai, DateOfPregnancyTermination.DateTimeToString);
                    //    }
                    //}
                    _IsPregnancyTermination = value;
                    if (Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null)
                    {
                        Registration.AdmissionInfo.DischargePapersInfo.IsPregnancyTermination = _IsPregnancyTermination;
                    }
                    NotifyOfPropertyChange(() => IsPregnancyTermination);
                }
            }
        }

        public void btnTreatmentDischarge()
        {
            GlobalsNAV.ShowDialog<ITreatmentDischargeList>((proAlloc) => {}, null, false, true, new Size(350, 250));
        }

        public void Handle(SelectedTreatmentDischarge_Event message)
        {
            if (message != null && message.SelectedTreatmentDischarge != null)
            {
                if (Registration != null && Registration.AdmissionInfo != null)
                {
                    Registration.AdmissionInfo.TreatmentDischarge += message.SelectedTreatmentDischarge;
                }
            }
        }

        private IMinHourDateControl _DateOfPregnancyTermination;
        public IMinHourDateControl DateOfPregnancyTermination
        {
            get { return _DateOfPregnancyTermination; }
            set
            {
                _DateOfPregnancyTermination = value;
                NotifyOfPropertyChange(() => DateOfPregnancyTermination);
            }
        }

        void DateOfPregnancyTermination_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DateTime")
            {
                if (Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null)
                {
                    Registration.AdmissionInfo.DischargePapersInfo.PregnancyTerminationDateTime
                        = DateOfPregnancyTermination.DateTime.GetValueOrDefault(DateTime.MinValue);
                    Registration.AdmissionInfo.TreatmentDischarge += (string.IsNullOrEmpty(Registration.AdmissionInfo.TreatmentDischarge) ? "" : "; ") +
                        string.Format("{0} vào {1}", eHCMSResources.Z3311_G1_DinhChiThai, DateOfPregnancyTermination.DateTimeToString);
                }
            }
        }

        public bool HasPregnancyTermination
        {
            get 
            {
                return Registration != null && Registration.Patient.Gender == "F";
            }           
        }

        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }

        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                _gSelectedDoctorStaff = value;
                if(Registration != null && Registration.AdmissionInfo != null 
                    && Registration.AdmissionInfo.DischargePapersInfo != null && _gSelectedDoctorStaff != null)
                {
                    Registration.AdmissionInfo.DischargePapersInfo.HeadOfDepartmentDoctorStaffID = _gSelectedDoctorStaff.StaffID;
                }
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }

        private Staff _gSelectedUnitLeaderDoctorStaff;
        public Staff gSelectedUnitLeaderDoctorStaff
        {
            get
            {
                return _gSelectedUnitLeaderDoctorStaff;
            }
            set
            {
                _gSelectedUnitLeaderDoctorStaff = value;
                if (Registration != null && Registration.AdmissionInfo != null 
                    && Registration.AdmissionInfo.DischargePapersInfo != null && _gSelectedUnitLeaderDoctorStaff != null)
                {
                    Registration.AdmissionInfo.DischargePapersInfo.UnitLeaderDoctorStaffID = _gSelectedUnitLeaderDoctorStaff.StaffID;
                }
                NotifyOfPropertyChange(() => gSelectedUnitLeaderDoctorStaff);
            }
        }

        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //&& x.PrintTitle != null && x.PrintTitle.Trim().ToLower() == "bs.".ToList());
            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|")
                && x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
            if (DoctorStaffs.Count() > 0 && DoctorStaffs.Any(x => Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|")
                && x.StaffID == Globals.LoggedUserAccount.Staff.StaffID))
            {
                gSelectedUnitLeaderDoctorStaff = DoctorStaffs.Where(x => x.StaffID == Globals.LoggedUserAccount.Staff.StaffID).FirstOrDefault();
            }
        }

        AxAutoComplete AcbDoctorStaff { get; set; }

        public void DoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbDoctorStaff = (AxAutoComplete)sender;
        }

        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.ServerConfigSection.CommonItems.BacSiTruongPhoKhoa.Contains("|" + x.V_JobPosition.ToString() + "|") &&
                Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }

        AxAutoComplete AcbUnitLeaderDoctorStaff { get; set; }
        public void UnitLeaderDoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbUnitLeaderDoctorStaff = (AxAutoComplete)sender;
        }

        public void UnitLeaderDoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.ServerConfigSection.CommonItems.ThuTruongDonVi.Contains("|" + x.V_JobPosition.ToString() + "|") &&
                Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
                                                                                        //&& x.IsUnitLeader));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void UnitLeaderDoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (sender != null)
            {
                gSelectedUnitLeaderDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
            }
        }

        private bool _IsLeaveForTreatment;
        public bool IsLeaveForTreatment
        {
            get { return _IsLeaveForTreatment; }
            set
            {
                if (_IsLeaveForTreatment == value)
                {
                    return;
                }
                if (value == false)
                {
                    FromDate = null;
                    ToDate = null;
                    if(Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null
                        && Registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment != null)
                    {
                        Registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment = null;
                    }
                }
                _IsLeaveForTreatment = value;
                NotifyOfPropertyChange(() => IsLeaveForTreatment);
            }
        }

        private DateTime? _FromDate;
        public DateTime? FromDate
        {
            get { return _FromDate; }
            set
            {
                if (_FromDate != value)
                {
                    _FromDate = value;
                    if(Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null)
                    {
                        Registration.AdmissionInfo.DischargePapersInfo.FromDateLeaveForTreatment = _FromDate;
                    }
                    NotifyOfPropertyChange(() => FromDate);
                    SetNumberDayOfLeaveForTreatment();
                }
            }
        }

        private DateTime? _ToDate;
        public DateTime? ToDate
        {
            get { return _ToDate; }
            set
            {
                if (_ToDate != value)
                {
                    _ToDate = value;
                    if (Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null)
                    {
                        Registration.AdmissionInfo.DischargePapersInfo.ToDateLeaveForTreatment = _ToDate;
                    }
                    NotifyOfPropertyChange(() => ToDate);
                    SetNumberDayOfLeaveForTreatment();
                }
            }
        }

        private void SetNumberDayOfLeaveForTreatment()
        {
            if(FromDate != null && ToDate != null && ToDate.HasValue && FromDate.HasValue && FromDate < ToDate)
            {
                int numberDayOfLeaveForTreatment = ToDate.Value.Date.Subtract(FromDate.Value.Date).Days + 01;
                if (Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null)
                {
                    Registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment = numberDayOfLeaveForTreatment;
                }
                if (IsLeaveForTreatment)
                {
                    string Comment = string.Format("Đề nghị: Nghỉ ốm {0} ngày (Kể từ ngày {1} đến hết ngày {2}", numberDayOfLeaveForTreatment
                        , FromDate.GetValueOrDefault().ToString("dd/MM/yyyy"), ToDate.GetValueOrDefault().ToString("dd/MM/yyyy"));
                    Registration.AdmissionInfo.Comment = string.IsNullOrEmpty(Registration.AdmissionInfo.Comment) ? Comment : string.Format("{0}; {1}", Registration.AdmissionInfo.Comment, Comment);
                }
            }
        }

        public bool CheckValidForDischaregePaper()
        {
            if(Registration != null && Registration.AdmissionInfo != null && Registration.AdmissionInfo.DischargePapersInfo != null)
            {
                if(Registration.AdmissionInfo.DischargePapersInfo.HeadOfDepartmentDoctorStaffID == null
                    || Registration.AdmissionInfo.DischargePapersInfo.UnitLeaderDoctorStaffID == null)
                {
                    MessageBox.Show("Bắt buộc chọn bác sĩ ký giấy ra viện!");
                    return false;
                }
                if (IsPregnancyTermination)
                {
                    if (Registration.AdmissionInfo.DischargePapersInfo.FetalAge == null)
                    {
                        MessageBox.Show("Bắt buộc nhập tuổi thai khi chọn đình chỉ thai!");
                        return false;
                    }
                    if (Registration.AdmissionInfo.DischargePapersInfo.FetalAge != null &&
                    (Registration.AdmissionInfo.DischargePapersInfo.FetalAge > 42 || Registration.AdmissionInfo.DischargePapersInfo.FetalAge < 1))
                    {
                        MessageBox.Show("Tuổi thai phải lớn hơn 0 và bé hơn 42!");
                        return false;
                    }
                    if (Registration.AdmissionInfo.DischargePapersInfo.PregnancyTerminationDateTime == null)
                    {
                        MessageBox.Show("Bắt buộc nhập ngày giờ đình chỉ thai!");
                        return false;
                    }
                    if (string.IsNullOrEmpty(Registration.AdmissionInfo.DischargePapersInfo.ReasonOfPregnancyTermination))
                    {
                        MessageBox.Show("Bắt buộc nhập nguyên nhân đình chỉ thai!");
                        return false;
                    }
                }

                if (IsLeaveForTreatment)
                {
                    if (Registration.AdmissionInfo.DischargePapersInfo.FromDateLeaveForTreatment == null || Registration.AdmissionInfo.DischargePapersInfo.ToDateLeaveForTreatment == null)

                    {
                        MessageBox.Show("Vui lòng nhập đầy đủ từ ngày đến ngày Nghỉ để điều trị ngoại trú!");
                        return false;
                    }

                    if (Registration.AdmissionInfo.DischargePapersInfo.FromDateLeaveForTreatment > Registration.AdmissionInfo.DischargePapersInfo.ToDateLeaveForTreatment)
                    {
                        MessageBox.Show(eHCMSResources.K0229_G1_TuNgKhongLonHonDenNg);
                        return false;
                    }

                    if ((IsPregnancyTermination && ((Registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment > 50 && Registration.AdmissionInfo.DischargePapersInfo.FetalAge >= 13)
                            || (Registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment > 30 && Registration.AdmissionInfo.DischargePapersInfo.FetalAge < 13)))
                        || (!IsPregnancyTermination && Registration.AdmissionInfo.DischargePapersInfo.NumberDayOfLeaveForTreatment > 30))
                    {
                        MessageBox.Show("Số ngày nghỉ để điều trị ngoại trú vượt quá số ngày quy định. Vui lòng kiểm tra và chọn lại số ngày phù hợp!");
                        return false;
                    }
                }
            }
            return true;
        }

        public AxTextBoxFilter.TextBoxFilterType IntNumberFilter
        {
            get
            {
                return AxTextBoxFilter.TextBoxFilterType.Integer;
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;
                if (!string.IsNullOrEmpty(txt) && int.Parse(txt) >= 22 && IsPregnancyTermination)
                {
                    Registration.AdmissionInfo.Comment = string.IsNullOrEmpty(Registration.AdmissionInfo.Comment) ? "Đẻ non, con chết" : string.Format("{0}; Đẻ non, con chết", Registration.AdmissionInfo.Comment);
                }
            }
        }
        //▲==== #003

        //▼==== #005
        private string GetFContact(Patient patient)
        {
            string value = "";
            if(patient.V_FamilyRelationship != null)
            {
                Lookup familyRelationship = Globals.AllLookupValueList.FirstOrDefault(x => x.LookupID == patient.V_FamilyRelationship);
                if(familyRelationship != null)
                {
                    value = string.Format("{0}: {1}", familyRelationship.ObjectValue, patient.FContactFullName);
                }
            }
            return value;
        }
        //▲==== #005
    }
}
