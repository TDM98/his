using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Registration.ViewModels;
using System.Windows.Controls;
/*
 * 20190419 #001 TTM:   BM 0006758: Ngăn không cho đăng ký vượt quá số lượng dịch vụ khám bệnh có BH do bệnh viện quy định cho 1 ca BH trong 1 ngày.
 * 20191001 #002 TTM:   BM 0017391: Fix lỗi Bạn đang thao tác bệnh nhân A, muốn chuyển sang B hay không
 * 20200101 #003 TNHX: Thêm DV giá dao động cho ngoại trú
 * 20211004 #004 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 */

namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientMedicalServiceRequest)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientMedicalServiceRequestViewModel : HandlePayCompletedViewModelBase, IPatientMedicalServiceRequest
                                                , IHandle<ItemSelected<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        //, IHandle<CreateNewPatientEvent>
        //, IHandle<HiCardConfirmedEvent>
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<AddCompleted<Patient>>
        , IHandle<PayForRegistrationCompleted>
        , IHandle<SaveAndPayForRegistrationCompleted>
        , IHandle<AddCompleted<PatientRegistration>>
        , IHandle<SimplePayViewCancelCloseEvent>
        , IHandle<UpdateCompleted<PatientRegistration>>
        , IHandle<UpdateCompleted<Patient>>
        , IHandle<ModifyPriceToInsert_Completed>
    {
        [ImportingConstructor]
        public PatientMedicalServiceRequestViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            //▼===== #002: Subscribe ở Onactive rồi không subscribe ở hàm khởi tạo.
            //eventArg.Subscribe(this);
            //▲===== #002
            Initialize();
            LoadDoctorStaffCollection();
            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV3>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mDangKyDV_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mDangKyDV_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mDangKyDV_Info_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = mDangKyDV_Info_XemPhongKham;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            var regDetailsVm = Globals.GetViewModel<IMedServiceReqSummary>();
            RegistrationDetailsContent = regDetailsVm;
            //phan quyen
            RegistrationDetailsContent.mDichVuDaTT_ChinhSua = mDangKyDV_DichVuDaTT_ChinhSua;
            RegistrationDetailsContent.mDichVuDaTT_Luu = mDangKyDV_DichVuDaTT_Luu;
            RegistrationDetailsContent.mDichVuDaTT_TraTien = mDangKyDV_DichVuDaTT_TraTien;
            RegistrationDetailsContent.mDichVuDaTT_In = mDangKyDV_DichVuDaTT_In;
            RegistrationDetailsContent.mDichVuDaTT_LuuTraTien = mDangKyDV_DichVuDaTT_LuuTraTien;

            RegistrationDetailsContent.mDichVuMoi_ChinhSua = mDangKyDV_DichVuMoi_ChinhSua;
            RegistrationDetailsContent.mDichVuMoi_Luu = mDangKyDV_DichVuMoi_Luu;
            RegistrationDetailsContent.mDichVuMoi_TraTien = mDangKyDV_DichVuMoi_TraTien;
            RegistrationDetailsContent.mDichVuMoi_In = mDangKyDV_DichVuMoi_In;
            RegistrationDetailsContent.mDichVuMoi_LuuTraTien = mDangKyDV_DichVuMoi_LuuTraTien;
            // 20181205 TNHX: [BM0005300] Enable Button View/Print PhieuChiDinh_DichVu
            RegistrationDetailsContent.ShowPhieuChiDinh_In = true;

            ActivateItem(regDetailsVm);
            regDetailsVm.ValidateRegistration = ValidateRegInfo;
            ((INotifyPropertyChangedEx)regDetailsVm).PropertyChanged += regDetailsVm_PropertyChanged;

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                GetServiceTypes();
            }
            var uc1 = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo = uc1;
            this.ActivateItem(UCDoctorProfileInfo);

            var uc2 = Globals.GetViewModel<IPatientInfo>();
            UCPatientProfileInfo = uc2;
            this.ActivateItem(uc2);
            var ucPRM = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            UCHeaderInfoPMR = ucPRM;
            ActivateItem(ucPRM);
            Authorization();
            GetConsultationRoomTargetTSegment();
            //20190401 TBL: Load theo ten dang nhap
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
            //20181214 TTM: Comment ra, vì màn hình này ko có hot key, mà để biến này vào sẽ làm cho HotKey.cs tưởng nhần màn hình này có hot key => Exception null => lỗi.
            //base.HasInputBindingCmd = true;
        }
        public object UCDoctorProfileInfo { get; set; }

        public IPatientInfo UCPatientProfileInfo { get; set; }
        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR { get; set; }

        void regDetailsVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSavingRegistration")
            {
                NotifyWhenBusy();
            }
        }
        
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPatientRegistrationNewView;
            Authorization();
        }

        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(CreateNewPatient)
            {
                GestureModifier = ModifierKeys.Control,
                GestureKey = Key.N
            };

            yield return new InputBindingCommand(SavePatientDetailsAndClose)
            {
                GestureModifier = ModifierKeys.Control,
                GestureKey = Key.S
            };

        }

        public void CreateNewPatient()
        {
            SearchRegistrationContent.CreateNewPatientCmd();
        }

        public void SavePatientDetailsAndClose()
        {
            //Globals.EventAggregator.Publish(new SavePatientDetailsAndClose());
        }


        #region PROPERTIES
        private bool _mChuaDangKy;
        public bool mChuaDangKy
        {
            get { return _mChuaDangKy; }
            set
            {
                _mChuaDangKy = value;
                NotifyOfPropertyChange(() => mChuaDangKy);
            }
        }

        private bool _mDangKyBH;
        public bool mDangKyBH
        {
            get { return _mDangKyBH; }
            set
            {
                _mDangKyBH = value;
                NotifyOfPropertyChange(() => mDangKyBH);
            }
        }

        private bool _mCuocHen_TraiHen;
        public bool mCuocHen_TraiHen
        {
            get { return _mCuocHen_TraiHen; }
            set
            {
                _mCuocHen_TraiHen = value;
                NotifyOfPropertyChange(() => mCuocHen_TraiHen);
            }
        }

        private bool _mCuocHen_DungHen;
        public bool mCuocHen_DungHen
        {
            get { return _mCuocHen_DungHen; }
            set
            {
                _mCuocHen_DungHen = value;
                NotifyOfPropertyChange(() => mCuocHen_DungHen);
            }
        }

        private bool _mDangKyChuaBH;
        public bool mDangKyChuaBH
        {
            get { return _mDangKyChuaBH; }
            set
            {
                _mDangKyChuaBH = value;
                NotifyOfPropertyChange(() => mDangKyChuaBH);
            }
        }

        private RegStatus _TitleStatus = RegStatus.None;
        public RegStatus TitleStatus
        {
            get { return _TitleStatus; }
            set
            {
                _TitleStatus = value;
                NotifyOfPropertyChange(() => TitleStatus);
                switch (TitleStatus)
                {
                    case RegStatus.None:
                        mChuaDangKy = false;
                        mDangKyBH = false;
                        mDangKyChuaBH = false;
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = false;
                        break;
                    case RegStatus.ChuaDangKy:
                        mChuaDangKy = true;
                        mDangKyBH = false;
                        mDangKyChuaBH = false;
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = false;
                        break;
                    case RegStatus.DangKyCoBH:
                        mChuaDangKy = false;
                        mDangKyBH = true;
                        mDangKyChuaBH = false;
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = false;
                        break;
                    case RegStatus.DangKyChuaBH:
                        mChuaDangKy = false;
                        mDangKyBH = false;
                        mDangKyChuaBH = true;
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = false;
                        break;
                    case RegStatus.CuocHen_DungHen:
                        mChuaDangKy = false;
                        mDangKyBH = false;
                        mDangKyChuaBH = false;
                        mCuocHen_DungHen = true;
                        mCuocHen_TraiHen = false;
                        break;
                    case RegStatus.CuocHen_TraiHen:
                        mChuaDangKy = false;
                        mDangKyBH = false;
                        mDangKyChuaBH = false;
                        mCuocHen_DungHen = false;
                        mCuocHen_TraiHen = true;
                        break;
                }
            }
        }


        private IPatientRegistrationNewView _currentView;

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            //var homeVm = Globals.GetViewModel<IHome>();
            //homeVm.OutstandingTaskContent = Globals.GetViewModel<IRegistrationOutStandingTask>();

            //DoSetCurrentPatientAndRegistration(Globals.PatientAllDetails.PatientInfo, Registration_DataStorage.CurrentPatientRegistration );
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                Coroutine.BeginExecute(DoOpenRegistration(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID));
            }
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);

            //var homeVm = Globals.GetViewModel<IHome>();
            //homeVm.OutstandingTaskContent = null;
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

        private IMedServiceReqSummary _registrationDetailsContent;
        public IMedServiceReqSummary RegistrationDetailsContent
        {
            get { return _registrationDetailsContent; }
            set
            {
                _registrationDetailsContent = value;
                NotifyOfPropertyChange(() => RegistrationDetailsContent);
            }
        }
        private IInPatientSelectPcl _selectPCLContent;
        public IInPatientSelectPcl SelectPCLContent
        {
            get { return _selectPCLContent; }
            set
            {
                _selectPCLContent = value;
                NotifyOfPropertyChange(() => SelectPCLContent);
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
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
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
        private void LoadDoctorStaffCollection()
        {
            //▼====: #004
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //▲====: #004
        }
        private DateTime? _gMedicalInstructionDate = Globals.GetCurServerDateTime();
        public DateTime? gMedicalInstructionDate
        {
            get
            {
                return _gMedicalInstructionDate;
            }
            set
            {
                if (_gMedicalInstructionDate == value) return;
                _gMedicalInstructionDate = value;
                NotifyOfPropertyChange(() => gMedicalInstructionDate);
            }
        }
        private IInPatientSelectPclLAB _selectPCLContentLAB;
        public IInPatientSelectPclLAB SelectPCLContentLAB
        {
            get { return _selectPCLContentLAB; }
            set
            {
                _selectPCLContentLAB = value;
                NotifyOfPropertyChange(() => SelectPCLContentLAB);
            }
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void InstructionDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0)
            {
                Int32 NumOfDays = (gMedicalInstructionDate.GetValueOrDefault().Date - Globals.GetCurServerDateTime().Date).Days;
                if (NumOfDays > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysInDischargeForm)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z2195_G1_NgYLenhKgVuotQuaNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    gMedicalInstructionDate = Globals.GetCurServerDateTime().Date;
                    return;
                }
            }
        }
        public string RegistrationTitle
        {
            get
            {
                if (_curRegistration == null)
                {
                    return eHCMSResources.Z0026_G1_TTinDKChuaCoDK;
                }
                if (_curRegistration.PtRegistrationID <= 0)
                {
                    return eHCMSResources.Z0025_G1_TTinDKMoi;
                }
                return string.Format(eHCMSResources.Z0364_G1_TTinDKNVienDK, _curRegistration.PtRegistrationCode
                    , _curRegistration.StaffName);
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

        private PatientRegistration _curRegistration;

        public PatientRegistration CurRegistration
        {
            get { return _curRegistration; }
            set
            {
                _curRegistration = value;
                NotifyOfPropertyChange(() => CurRegistration);
                NotifyOfPropertyChange(() => CanCancelRegistrationCmd);
                NotifyOfPropertyChange(() => CanReportRegistrationInfoInsuranceCmd);
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
        public PatientClassification CurClassification
        {
            get
            {
                return PatientSummaryInfoContent.CurrentPatientClassification;
            }
            set
            {
                PatientSummaryInfoContent.CurrentPatientClassification = value;
                NotifyOfPropertyChange(() => HiServiceBeingUsed);
                RegistrationDetailsContent.HiServiceBeingUsed = HiServiceBeingUsed;
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
        private bool _canPrint;
        public bool CanPrint
        {
            get { return _canPrint; }
            set
            {
                if (_canPrint != value)
                {
                    _canPrint = value;
                    NotifyOfPropertyChange(() => CanPrint);
                }
            }
        }

        private ObservableCollection<RefMedicalServiceType> _serviceTypes;
        public ObservableCollection<RefMedicalServiceType> ServiceTypes
        {
            get { return _serviceTypes; }
            set
            {
                _serviceTypes = value;
                NotifyOfPropertyChange(() => ServiceTypes);
            }
        }
        private RefMedicalServiceType _medServiceType;
        public RefMedicalServiceType MedServiceType
        {
            get
            {
                return _medServiceType;
            }
            set
            {
                if (_medServiceType != value)
                {
                    _medServiceType = value;
                    NotifyOfPropertyChange(() => MedServiceType);
                    NotifyOfPropertyChange(() => ShowLocationAndDoctor);
                    MedicalServiceItems = null;
                    ResetServiceToDefaultValue();
                    GetAllMedicalServiceItemsByType(_medServiceType);
                }
            }
        }

        private ObservableCollection<RefMedicalServiceItem> _medicalServiceItems;
        public ObservableCollection<RefMedicalServiceItem> MedicalServiceItems
        {
            get { return _medicalServiceItems; }
            set
            {
                _medicalServiceItems = value;
                NotifyOfPropertyChange(() => MedicalServiceItems);
            }
        }
        private RefMedicalServiceItem _medServiceItem;
        public RefMedicalServiceItem MedServiceItem
        {
            get
            {
                return _medServiceItem;
            }
            set
            {
                if (_medServiceItem != value)
                {
                    _medServiceItem = value;
                    NotifyOfPropertyChange(() => MedServiceItem);
                    DeptLocation = null;
                    ResetQuantityToDefaultValue();
                    //Load lai danh sach location tuong ung
                    long medServiceID = -1;
                    if (_medServiceItem != null)
                    {
                        medServiceID = _medServiceItem.MedServiceID;
                    }
                    ClearLocations();
                    if (medServiceID > 0 && _medServiceType != null)
                    {
                        if (_medServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
                        {
                            GetLocationsByServiceIDFromCatche(medServiceID);
                        }
                    }
                }
            }
        }

        public bool ShowLocationAndDoctor
        {
            get
            {
                if (_medServiceType != null && _medServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
                {
                    return false;
                }
                return true;
            }
        }

        private byte? _serviceQty;
        public byte? ServiceQty
        {
            get
            {
                return _serviceQty;
            }
            set
            {
                if (_serviceQty != value)
                {
                    _serviceQty = value;
                    NotifyOfPropertyChange(() => ServiceQty);
                }
            }
        }

        private ObservableCollection<DeptLocation> _deptLocations;
        public ObservableCollection<DeptLocation> DeptLocations
        {
            get
            {
                return _deptLocations;
            }
            set
            {
                if (_deptLocations != value)
                {
                    _deptLocations = value;
                    NotifyOfPropertyChange(() => DeptLocations);
                }
            }
        }

        private DeptLocation _deptLocation;
        public DeptLocation DeptLocation
        {
            get
            {
                return _deptLocation;
            }
            set
            {
                if (_deptLocation != value)
                {
                    _deptLocation = value;
                    NotifyOfPropertyChange(() => DeptLocation);
                }
            }
        }

        private bool _canAddService;
        public bool CanAddService
        {
            get
            {
                return _canAddService;
            }
            set
            {
                if (_canAddService != value)
                {
                    _canAddService = value;
                    NotifyOfPropertyChange(() => CanAddService);
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

        private bool _canChangePatientType;
        /// <summary>
        /// Chi duoc thay doi loai benh nhan khi chon dang ky moi.
        /// </summary>
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
        private DateTime _regDate;
        public DateTime RegistrationDate
        {
            get
            {
                return _regDate;
            }
            set
            {
                _regDate = value;
                NotifyOfPropertyChange(() => RegistrationDate);
                if (CurRegistration != null && CurRegistration.PtRegistrationID <= 0)
                {
                    CurRegistration.ExamDate = _regDate;
                }
            }
        }

        private string _Diagnosis;
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                if(_Diagnosis != value)
                {
                    _Diagnosis = value;
                    NotifyOfPropertyChange(() => Diagnosis);
                }
            }
        }
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
                UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
                PatientSummaryInfoContent.Registration_DataStorage = Registration_DataStorage;
                RegistrationDetailsContent.ServiceRecID = Registration_DataStorage != null && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count > 0 ? Registration_DataStorage.PatientServiceRecordCollection.First().ServiceRecID : 0;
            }
        }
        #endregion

        public void ConfirmHIItem(object hiItem)
        {
            ConfirmedHiItem = hiItem as HealthInsurance;
            if (CurRegistration != null)
            {
                CurRegistration.HealthInsurance = ConfirmedHiItem;
            }
        }
        public void ConfirmPaperReferal(object referal)
        {
            ConfirmedPaperReferal = referal as PaperReferal;
            if (CurRegistration != null)
            {
                CurRegistration.PaperReferal = ConfirmedPaperReferal;
            }
        }
        #region EVENT HANDLERS

        public void CheckAppointment(PatientAppointment Appointment)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0630_G1_DangLayTTinCHen);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetPatientByAppointment(Appointment,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                bool bOK;
                                PatientRegistration regInfo = null;
                                try
                                {
                                    regInfo = contract.EndGetPatientByAppointment(asyncResult);
                                    if (regInfo != null
                                        && regInfo.PCLRequests != null
                                        && regInfo.PCLRequests.Count > 0)
                                    {
                                        foreach (var item in regInfo.PCLRequests)
                                        {
                                            item.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
                                        }
                                    }
                                    bOK = true;
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    bOK = false;
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    bOK = false;
                                }
                                finally
                                {
                                    //IsLoadingAppointment = false;
                                }
                                if (bOK)
                                {
                                    CurrentPatient = regInfo.Patient;
                                    RegistrationDetailsContent.Reset();
                                    //Show du lieu len:
                                    ShowOldRegistration(regInfo, true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    //IsLoadingAppointment = false;
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
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

        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
                Coroutine.BeginExecute(DoSetCurrentPatient(message.Item));
            }
        }

        //---- DPT

        public void Handle(UpdateCompleted<Patient> message)
        {
            if (message != null && message.Item != null)
            {
                CurrentPatient = message.Item;
                Coroutine.BeginExecute(DoSetCurrentPatient(message.Item));
            }
        }


        //public void Handle(CreateNewPatientEvent message)
        //{
        //    if (message != null)
        //    {
        //        Action<IPatientDetailsAndHI> onInitDlg = delegate (IPatientDetailsAndHI vm)
        //        {
        //            vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
        //            vm.IsChildWindow = true;
        //            vm.CreateNewPatientAndHI();
        //        };
        //        GlobalsNAV.ShowDialog<IPatientDetailsAndHI>(onInitDlg);
        //    }
        //}

        public void Handle(NoChangeConfirmHiBenefit message)
        {
            if (message != null)
            {
                CurRegistration.HIComment = message.HIComment;
                if (ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
                {
                    return;
                }
            }
        }

        //20190930 TBL: Khi từ màn hình khám bệnh qua màn hình đăng ký rồi xác nhận BHYT thì nhận sự kiện này, mà CurRegistration = null nên báo lỗi => comment lại để khỏi nhận
        //public void Handle(HiCardConfirmedEvent message)
        //{
        //    if (message != null)
        //    {
        //        ConfirmHIItem(message.HiProfile);
        //        ConfirmPaperReferal(message.PaperReferal);
        //        if (message.HiProfile != null)
        //        {
        //            //Tinh lai quyen loi bao hiem.
        //            Coroutine.BeginExecute(
        //                DoCalcHiBenefit(CurRegistration.HealthInsurance, CurRegistration.PaperReferal), null
        //                , (o, e) =>
        //                {
        //                    var vm = message.Source as IPatientDetails;
        //                    if (vm != null)
        //                    {
        //                        vm.Close();
        //                    }
        //                });
        //        }
        //    }
        //}

        public void Handle(ResultFound<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Result;
                if (CurrentPatient != null)
                {
                    //KMx: Sau khi tìm hiểu thì thấy Event này tự bắn vào ViewModel này và RegistrationSummaryV2ViewModel (14/04/2014 17:51).
                    Globals.EventAggregator.Publish(new ItemSelected<Patient> { Item = CurrentPatient });
                }
            }
        }

        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0727_G1_Msg_ConfThemMoiBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    Action<IPatientDetails> onInitDlg = delegate (IPatientDetails vm)
                    {
                        var criteria = message.SearchCriteria as PatientSearchCriteria;
                        vm.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                        vm.CreateNewPatient();
                        vm.IsChildWindow = true;
                        if (criteria != null)
                        {
                            vm.CurrentPatient.FullName = criteria.FullName;
                        }
                    };
                    GlobalsNAV.ShowDialog<IPatientDetails>(onInitDlg);
                }
            }
        }
        public void Handle(AddCompleted<Patient> message)
        {
            if (message != null)
            {
                CurrentPatient = message.Item;
                if (CurrentPatient != null)
                {
                    //var regInfoVm = Globals.GetViewModel<IPatientSummaryInfoV2>();
                    if (PatientSummaryInfoContent != null)
                    {
                        PatientSummaryInfoContent.CurrentPatient = CurrentPatient;
                    }
                    Coroutine.BeginExecute(DoSetCurrentPatient(message.Item));
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
                    OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);

                    ProcessPayCompletedEvent(message);
                }
            }
        }
        public void Handle(SaveAndPayForRegistrationCompleted message)
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

                    if (message.RegistrationInfo != null)
                    {
                        ShowOldRegistration(message.RegistrationInfo);
                    }
                    else
                    {
                        OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
                    }
                }
            }
        }
        public void Handle(ItemSelected<PatientRegistration> message)
        {
            if (message != null && message.Item != null)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }
        public void Handle(AddCompleted<PatientRegistration> message)
        {
            if (message.Item != null)
            {

                // TxD 01/01/2014 Added the following to SAVE a trip back to the SERVER
                // BECAUSE we got PatientRegistration from return of the previous Call to the SERVER
                if (message.RefreshItemFromReturnedObj)
                {
                    RefreshRegistration(message.Item);
                }
                else
                {
                    OpenRegistration(message.Item.PtRegistrationID);
                }
            }
        }
        public void Handle(SimplePayViewCancelCloseEvent message)
        {
            if (message != null)
            {
                OpenRegistration(message.PtRegistrationID);
            }
        }

        public void Handle(UpdateCompleted<PatientRegistration> message)
        {
            if (message.Item != null)
            {
                OpenRegistration(message.Item.PtRegistrationID);
            }
        }
        #endregion

        public void GetServiceTypes()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0604_G1_DangLayDSLoaiDV)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var outTypes = new List<long>
                                           {
                                               (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NGOAITRU,
                                               (long) AllLookupValues.V_RefMedicalServiceInOutOthers.NOITRU_NGOAITRU,
                                           };
                        contract.BeginGetMedicalServiceTypesByInOutType(outTypes,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<RefMedicalServiceType> allItem = new ObservableCollection<RefMedicalServiceType>();
                                try
                                {
                                    allItem = contract.EndGetMedicalServiceTypesByInOutType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception)
                                {
                                }
                                ServiceTypes = new ObservableCollection<RefMedicalServiceType>(allItem);
                                ResetServiceTypeToDefaultValue();

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

        DateTime curDate = DateTime.Now.AddDays(-1);
        public void GetConsultationRoomTargetTSegment()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0604_G1_DangLayDSLoaiDV)
                });
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetConsultationRoomTargetTSegment(0, false,
                            Globals.DispatchCallback(asyncResult =>
                            {

                                try
                                {
                                    var res = contract.EndGetConsultationRoomTargetTSegment(out curDate, asyncResult);
                                    //KMx: Sau khi kiểm tra toàn bộ chương trình, thấy biến Globals.ConsultationRoomTarget này không còn sử dụng nữa.
                                    Globals.ConsultationRoomTarget = new ObservableCollection<ConsultationRoomTarget>(
                                        res.Where(item => //item.DeptLocationID == deptloc.DeptLocationID && 
                                        item.ConsultationTimeSegments.StartTime.TimeOfDay < curDate.TimeOfDay
                                        && curDate.TimeOfDay < item.ConsultationTimeSegments.EndTime.TimeOfDay
                                        && ConsultTimeSegCheckDayOfWeek(item)));
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception)
                                {
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
        private bool ConsultTimeSegCheckDayOfWeek(ConsultationRoomTarget crt)
        {
            switch (curDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    if (crt.MondayTargetNumberOfCases < 1) return false;
                    break;
                case DayOfWeek.Tuesday:
                    if (crt.TuesdayTargetNumberOfCases < 1) return false;
                    break;
                case DayOfWeek.Wednesday:
                    if (crt.WednesdayTargetNumberOfCases < 1) return false;
                    break;
                case DayOfWeek.Thursday:
                    if (crt.ThursdayTargetNumberOfCases < 1) return false;
                    break;
                case DayOfWeek.Friday:
                    if (crt.FridayTargetNumberOfCases < 1) return false;
                    break;
                case DayOfWeek.Saturday:
                    if (crt.SaturdayTargetNumberOfCases < 1) return false;
                    break;
                case DayOfWeek.Sunday:
                    if (crt.SundayTargetNumberOfCases < 1) return false;
                    break;
            }
            return true;
        }
        public void ResetServiceTypeToDefaultValue()
        {
            bool bFound = false;
            if (ServiceTypes != null)
            {
                foreach (RefMedicalServiceType type in ServiceTypes)
                {
                    if (type.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH)
                    {
                        MedServiceType = type;
                        bFound = true;
                        break;
                    }
                }
            }
            if (!bFound)
            {
                MedServiceType = null;
            }
        }
        public void GetAllMedicalServiceItemsByType(RefMedicalServiceType serviceType)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0604_G1_DangLayDSLoaiDV)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        long? serviceTypeID = null;
                        if (serviceType != null)
                        {
                            serviceTypeID = serviceType.MedicalServiceTypeID;
                        }

                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, null, null,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<RefMedicalServiceItem> allItem = new ObservableCollection<RefMedicalServiceItem>();
                                try
                                {
                                    allItem = contract.EndGetAllMedicalServiceItemsByType(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                if (allItem == null)
                                {
                                    MedicalServiceItems = null;
                                }
                                else
                                {
                                    var sType = (RefMedicalServiceType)asyncResult.AsyncState;
                                    var col = new ObservableCollection<RefMedicalServiceItem>();
                                    foreach (var item in allItem)
                                    {
                                        item.RefMedicalServiceType = sType;
                                        col.Add(item);
                                    }  
                                    MedicalServiceItems = col;
                                }

                                ResetServiceToDefaultValue();

                            }), serviceType);
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
        public void ResetServiceToDefaultValue()
        {
            if (MedicalServiceItems != null && MedicalServiceItems.Count > 0)
            {
                MedServiceItem = MedicalServiceItems[0];
            }
            else
            {
                MedServiceItem = null;
            }
        }
        public void ResetQuantityToDefaultValue()
        {
            if (_medServiceItem != null && _medServiceItem.MedicalServiceTypeID > 0)
            {
                ServiceQty = 1;
            }
            else
            {
                ServiceQty = null;
            }
        }
        public void ClearLocations()
        {
            if (DeptLocations != null)
            {
                DeptLocations.Clear();
            }
        }
        public void GetLocationsByServiceIDFromCatche(long medServiceID)
        {
            if (MedicalServiceItems.Any(o => o.MedServiceID == medServiceID) == false)
            {
                DeptLocations = null;
                return;
            }

            var theItem = (from medItem in MedicalServiceItems
                           where medItem.MedServiceID == medServiceID && medItem.allDeptLocation != null && medItem.allDeptLocation.Count > 0
                           select medItem).SingleOrDefault();

            if (theItem == null || theItem.allDeptLocation == null || theItem.allDeptLocation.Count == 0)
            {
                DeptLocations = null;
                return;
            }

            DeptLocations = new ObservableCollection<DeptLocation>(MedicalServiceItems.Where(o => o.MedServiceID == medServiceID && o.allDeptLocation != null && o.allDeptLocation.Count > 0).ToList()[0].allDeptLocation);//.Where(c => CheckDeptLocValid(c.DeptLocationID)));

            if (DeptLocations.Count > 1 && Globals.ServerConfigSection.OutRegisElements.AutoLocationAllocation)
            {
                DeptLocation defaultDeptLoc = new DeptLocation()
                {
                    Location = new Location()
                    {
                        LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0606_G1_TuDongPhBoPg)
                    },
                    DeptLocationID = -1
                };
                DeptLocations.Insert(0, defaultDeptLoc);
                DeptLocation = defaultDeptLoc;
            }
            else
            {
                DeptLocation = DeptLocations.FirstOrDefault();
            }
        }
        private PatientClassification CreateDefaultClassification()
        {
            if (ConfirmedHiItem != null)
            {
                return PatientClassification.CreatePatientClassification((long)PatientType.INSUARED_PATIENT, "");
            }
            return PatientClassification.CreatePatientClassification((long)PatientType.NORMAL_PATIENT, "");
        }

        public void AddRegItemCmd()
        {
            //if (UCPatientProfileInfo.CurrentPatient == null)
            //{
            //    Globals.ShowMessage(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN, eHCMSResources.G0442_G1_TBao);
            //    return;
            //}
            if (MedServiceItem == null || MedServiceItem.MedServiceID < 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0156_G1_Chon1DV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if(gSelectedDoctorStaff == null || gMedicalInstructionDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS);
                return;
            }
            //▼====== #001
            if (CurRegistration == null || RegistrationDetailsContent == null || RegistrationDetailsContent.CurrentRegistration == null || RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails == null)
            {
                return;
            }
            if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurRegistration, MedServiceItem, RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                return;
            }
            //▲====== #001
            //▼====: #003
            if (MedServiceItem.V_NewPriceType > 0 && (MedServiceItem.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Unknown_PriceType) || MedServiceItem.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Updatable_PriceType)))
            {
                PopupModifyPrice_Type = AllLookupValues.PopupModifyPrice_Type.INSERT_DICHVU;
                PatientRegistrationDetail item = new PatientRegistrationDetail();
                item.RefMedicalServiceItem = MedServiceItem.DeepCopy();
                void onInitDlg(IModifyBillingInvItem vm)
                {
                    vm.ModifyBillingInvItem.IsModItemOK = false;
                    vm.ModifyBillingInvItem = item;
                    vm.ModifyBillingInvItem.V_NewPriceType = item.RefMedicalServiceItem.V_NewPriceType;
                    vm.PopupType = 1;
                    vm.Init();
                }
                GlobalsNAV.ShowDialog<IModifyBillingInvItem>(onInitDlg);
            }
            else
            {
                RegistrationDetailsContent.AddNewService(ObjectCopier.DeepCopy(MedServiceItem), DeptLocation != null ? DeptLocation : null, MedServiceType, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
            }
            //▲====: #003
        }
        /// <summary>
        /// Gọi hàm này khi tạo mới một đăng ký, hoặc load xong một đăng ký đã có.
        /// Khởi tạo những giá trị cần thiết để đưa lên form
        /// </summary>
        private void InitRegistration()
        {
            //tai day chua co du thong tin cua patient
            //phai lay lai tat ca thong tin cua patient
            if (CurRegistration.PtRegistrationID <= 0)
            {
                CurRegistration.Patient = CurrentPatient;
            }
            else
            {
                _currentPatient = CurRegistration.Patient;

                if (_currentPatient != null && _currentPatient.PatientID > 0)
                {

                    CanAddService = true;
                }
                else
                {
                    CanAddService = true;

                }
            }
            NotifyOfPropertyChange(() => RegistrationTitle);
            InitFormData();
        }

        public IEnumerator<IResult> InitRegistrationFullInfo()
        {
            //tai day chua co du thong tin cua patient
            //phai lay lai tat ca thong tin cua patient
            if (CurRegistration.PtRegistrationID <= 0)
            {
                CurRegistration.Patient = CurrentPatient;
            }
            else
            {
                _currentPatient = CurRegistration.Patient;

                if (_currentPatient != null && _currentPatient.PatientID > 0)
                {
                    CanAddService = true;
                }
                else
                {
                    CanAddService = false;
                }
            }
            NotifyOfPropertyChange(() => RegistrationTitle);
            yield break;
        }

        private void InitFormData()
        {
            if (CurRegistration.PatientRegistrationDetails == null)
            {
                CurRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
            }
            if (CurRegistration.PCLRequests == null)
            {
                CurRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
            }

            if (_curRegistration != null && _curRegistration.PtRegistrationID > 0)
            {
                PatientSummaryInfoContent.CanConfirmHi = false;
            }
            else
            {
                PatientSummaryInfoContent.CanConfirmHi = true;
            }

            RegistrationDetailsContent.SetRegistration(CurRegistration);
            RegistrationDetailsContent.HiServiceBeingUsed = HiServiceBeingUsed;
            RegistrationDetailsContent.StartAddingNewServicesAndPclCmd();

            //PatientSummaryInfoContent.IsCrossRegion = CurRegistration.IsCrossRegion.GetValueOrDefault(true);
            PatientSummaryInfoContent.SetPatientHISumInfo(null);
        }

        private bool _registrationInfoHasChanged;
        /// <summary>
        /// Cho biet thong tin dang ky tren form da duoc thay doi chua.
        /// </summary>
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    NotifyOfPropertyChange(() => RegistrationInfoHasChanged);
                    NotifyOfPropertyChange(() => CanCancelRegistrationCmd);
                    CanSearchPatient = !_registrationInfoHasChanged;

                    NotifyButtonBehaviourChanges();
                }
            }
        }
        private bool _registrationCreating = false;
        /// <summary>
        /// Dang tao moi dang ky (ve server lay ngay thang.)
        /// </summary>
        public bool RegistrationCreating
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

        public override bool IsProcessing
        {
            get
            {
                return _registrationCreating || _registrationCancelling || _isLoadingRegistration;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_registrationCreating)
                {
                    return eHCMSResources.Z0607_G1_DangTaoDK;
                }
                if (_registrationCancelling)
                {
                    return eHCMSResources.Z0608_G1_DangHuyDK;
                }
                return "";
            }
        }
        private void NotifyButtonBehaviourChanges()
        {
            CanConfirm = !IsProcessing &&
                    (CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_CHANGED
                    || CurrentRegMode == RegistrationFormMode.NEW_REGISTRATION_CHANGED);

            CanCreateNewRegistration = !IsProcessing && (CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED);
            CanPrint = !IsProcessing && CurrentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED;
        }

        private ObservableCollection<Staff> _Staffs;
        public ObservableCollection<Staff> Staffs
        {
            get
            {
                return _Staffs;
            }
            set
            {
                if (_Staffs != value)
                {
                    _Staffs = value;
                    NotifyOfPropertyChange(() => Staffs);
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
        private bool _canPay;
        public bool CanPay
        {
            get
            {
                return _canPay;
            }
            set
            {
                if (_canPay != value)
                {
                    _canPay = value;
                    NotifyOfPropertyChange(() => CanPay);
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
        private bool _canSaveRegistration;
        public bool CanSaveRegistration
        {
            get
            {
                return _canSaveRegistration;
            }
            set
            {
                if (_canSaveRegistration != value)
                {
                    _canSaveRegistration = value;
                    NotifyOfPropertyChange(() => CanSaveRegistration);
                }
            }
        }

        private bool _canConfirm;
        public bool CanConfirm
        {
            get
            {
                return _canConfirm;
            }
            set
            {
                if (_canConfirm != value)
                {
                    _canConfirm = value;
                    NotifyOfPropertyChange(() => CanConfirm);
                }
            }
        }
        private bool _canCreateNewRegistration = true;
        public bool CanCreateNewRegistration
        {
            get
            {
                return _canCreateNewRegistration;
            }
            set
            {
                if (_canCreateNewRegistration != value)
                {
                    _canCreateNewRegistration = value;
                    NotifyOfPropertyChange(() => CanCreateNewRegistration);
                }
            }
        }

        private bool _bCreateNewRegistration = true;
        public bool bCreateNewRegistration
        {
            get
            {
                return _bCreateNewRegistration;
            }
            set
            {
                if (_bCreateNewRegistration != value)
                {
                    _bCreateNewRegistration = value;
                    NotifyOfPropertyChange(() => bCreateNewRegistration);
                }
            }
        }
        #region COROUTINES

        public IEnumerator<IResult> DoCalcHiBenefit(HealthInsurance hiItem, PaperReferal referal)
        {
            hiItem.isDoing = false;
            var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NGOAI_TRU);
            yield return calcHiTask;
            if (calcHiTask.Error == null)
            {
                CurRegistration.IsCrossRegion = calcHiTask.IsCrossRegion;

                Action<IConfirmHiBenefit> onInitDlg = delegate (IConfirmHiBenefit vm)
                {
                    vm.VisibilityCbxAllowCrossRegion = Globals.ServerConfigSection.HealthInsurances.AllowOutPtCrossRegion;
                    vm.OriginalHiBenefit = calcHiTask.HiBenefit;
                    vm.HiBenefit = calcHiTask.HiBenefit;
                    vm.HiId = calcHiTask.HiItem.HIID;
                    vm.PatientId = CurrentPatient.PatientID;
                    vm.OriginalIsCrossRegion = calcHiTask.IsCrossRegion;
                    vm.SetCrossRegion(calcHiTask.IsCrossRegion);
                };
                GlobalsNAV.ShowDialog<IConfirmHiBenefit>(onInitDlg);
            }
        }
        /// <summary>
        /// Kiểm tra hợp lệ cho form con sử dụng
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IResult> ValidateRegInfo(PatientRegistration regInfo, Common.Utilities.YieldValidationResult result)
        {
            if (regInfo == null)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}?", eHCMSResources.Z0609_G1_ChuaCoDK), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }
            if (_currentPatient == null)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z0148_G1_HayChon1BN, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            if (regInfo.ExamDate == DateTime.MinValue)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0154_G1_NgDKKhongHopLe), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            if (CurClassification == null)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0154_G1_HayChonLoaiBN), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }
            regInfo.PatientClassification = CurClassification;

            if (regInfo.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1000_G1_DayKgPhaiDKNgoaiTru, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            regInfo.StaffID = Globals.LoggedUserAccount.StaffID;
            if (ConfirmedPaperReferal != null)
            {
                regInfo.PaperReferal = ConfirmedPaperReferal;
            }

            if (HiServiceBeingUsed)
            {
                //Dang la benh nhan bao hiem.
                //Kiem tra neu chua confirm the bao hiem thi thong bao loi.
                if (ConfirmedHiItem == null)
                {
                    _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0157_G1_ChuaKTraTheBH), eHCMSResources.G0442_G1_TBao);
                    yield return _msgTask;
                    yield break;
                }
                regInfo.HealthInsurance = ConfirmedHiItem;

                long? hisID;
                if (ConfirmedHiItem != null
                    && ConfirmedHiItem.HealthInsuranceHistories != null
                    && ConfirmedHiItem.HealthInsuranceHistories.Count > 0)
                {
                    hisID = ConfirmedHiItem.HealthInsuranceHistories[0].HisID;
                    regInfo.HisID = hisID;
                }
                else
                {
                    hisID = regInfo.HisID;
                }
                foreach (PatientRegistrationDetail d in regInfo.PatientRegistrationDetails)
                {
                    d.HisID = hisID;
                }
            }

            regInfo.PatientID = _currentPatient.PatientID;
            regInfo.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NGOAI_TRU;
            regInfo.RefDepartment = Globals.ObjRefDepartment == null ? null : Globals.ObjRefDepartment;
            result.IsValid = true;

            yield break;

        }

        MessageBoxTask _msgTask;
        public void CallDoOpenRegistration(long regID)
        {
            Coroutine.BeginExecute(DoOpenRegistration(regID));
        }
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //Deployment.Current.Dispatcher.BeginInvoke(() => { IsLoadingRegistration = true; });
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);

            var loadRegTask = new LoadRegistrationInfoTask(regID, true);
            yield return loadRegTask;

            if (loadRegTask.Registration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK });
                Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = null, ID = regID });
                TitleStatus = RegStatus.ChuaDangKy;
            }
            else
            {
                TitleStatus = RegStatus.DangKyChuaBH;
                if (_currentPatient != null && loadRegTask.Registration.PatientID != CurrentPatient.PatientID)
                {
                    string newPatientName = loadRegTask.Registration != null &&
                                            loadRegTask.Registration.Patient != null
                                            ? loadRegTask.Registration.Patient.FullName : "";

                    string message = string.Format("{0} '{1}' \n{2} '{3}?'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, _currentPatient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, newPatientName);
                    _msgTask = new MessageBoxTask(message, eHCMSResources.G0442_G1_TBao, Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    if (_msgTask.Result == AxMessageBoxResult.Ok)
                    {
                        CurRegistration = loadRegTask.Registration;
                        ShowOldRegistration(CurRegistration);
                    }
                }
                else if (_curRegistration != loadRegTask.Registration)
                {
                    CurRegistration = loadRegTask.Registration;
                    ShowOldRegistration(CurRegistration);
                    Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = CurRegistration, ID = regID });
                }
            }
            this.HideBusyIndicator();
        }
        public void RefreshRegistration(PatientRegistration newRegInfo)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);

            TitleStatus = RegStatus.DangKyChuaBH;
            if (_currentPatient != null && newRegInfo.PatientID != CurrentPatient.PatientID)
            {
                string newPatientName = newRegInfo != null &&
                                        newRegInfo.Patient != null
                                        ? newRegInfo.Patient.FullName : "";

                string message = string.Format("{0}: {1} \n{2}: {3}", eHCMSResources.Z0175_G1_BanDangThaoTacBN, _currentPatient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, newPatientName);
                MessageBoxResult res = MessageBox.Show(message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);

                if (res == MessageBoxResult.OK)
                {
                    CurRegistration = newRegInfo;
                    ShowOldRegistration(CurRegistration);
                }
            }
            else if (_curRegistration != newRegInfo)
            {
                CurRegistration = newRegInfo;
                ShowOldRegistration(CurRegistration);
            }
            this.HideBusyIndicator();
        }

        public IEnumerator<IResult> DoSetCurrentPatient(Patient patient)
        {
            bCreateNewRegistration = true;
            var p = patient;
            if (p == null || p.PatientID <= 0)
            {
                CurrentPatient = null;
                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
                yield break;
            }
            //---------DPT trẻ em <72 tháng tuổi
            if (p.PatientID > 0)
            {

                DateTime loadCurrentDate = Globals.ServerDate.Value;
                int monthnew = 0;
                monthnew = (loadCurrentDate.Month + loadCurrentDate.Year * 12) - (Convert.ToDateTime(CurrentPatient.DOB).Month + Convert.ToDateTime(CurrentPatient.DOB).Year * 12);
                if (CurrentPatient.AgeOnly == true && ((loadCurrentDate.Year - Convert.ToDateTime(CurrentPatient.DOB).Year) <= 6))
                {
                    MessageBox.Show("Trẻ em dưới 6 tuổi phải nhập đầy đủ ngày tháng năm sinh");
                    CanAddService = false;
                    yield break;
                }
                else if (CurrentPatient.AgeOnly == false && monthnew <= 72)
                {
                    if (CurrentPatient.FContactFullName == null && (CurrentPatient.V_FamilyRelationship == null || CurrentPatient.V_FamilyRelationship == 0))
                    {
                        MessageBox.Show("Trẻ em dưới 6 tuổi phải nhập đầy đủ thông tin người liên hệ");
                        CanAddService = false;
                        yield break;
                    }
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { PatientLoading = true; });
            PatientLoaded = false;
            PatientLoading = true;
            var loadPatient = new LoadPatientTask(p.PatientID);
            yield return loadPatient;

            Globals.EventAggregator.Publish(new PatientReloadEvent { curPatient = loadPatient.CurrentPatient });

            PatientLoaded = true;
            PatientLoading = false;

            if (loadPatient.CurrentPatient != null)
            {
                CurrentPatient = loadPatient.CurrentPatient;

                if (_currentPatient == null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => { PatientLoading = false; });
                    yield break;
                }

                DateTime regDate = new DateTime();
                DateTime now = Globals.ServerDate.Value.Date;
                if (_currentPatient.LatestRegistration != null)
                {
                    regDate = _currentPatient.LatestRegistration.ExamDate.Date;
                }

                //Nếu có đăng ký cuối cùng và còn hiệu lực.
                if (_currentPatient.LatestRegistration != null && _currentPatient.LatestRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                    && _currentPatient.LatestRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED
                    && (regDate <= now && regDate.AddDays(ConfigValues.PatientRegistrationTimeout) >= now))
                {
                    //Nếu đăng ký thuộc 1 trong những Status PENDING, OPENED, REFUND
                    if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING
                      || _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
                      || _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND
                      //|| _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
                      )
                    {
                        if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
                        {
                            _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0121_G1_DKNayDaBiHuy), eHCMSResources.G0442_G1_TBao);
                            yield return _msgTask;
                        }
                        //Load dang ky len.
                        IEnumerator e = DoOpenRegistration(_currentPatient.LatestRegistration.PtRegistrationID);

                        while (e.MoveNext())
                            yield return e.Current as IResult;
                    }
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { PatientLoading = false; });
        }

        public void DoSetCurrentPatientAndRegistration(Patient curPatient, PatientRegistration curRegis)
        {
            bCreateNewRegistration = true;
            CurrentPatient = curPatient;
            CurRegistration = curRegis;
            ShowOldRegistration(CurRegistration);
        }


        public void Initialize()
        {
            Coroutine.BeginExecute(LoadStaffs());
        }

        private IEnumerator<IResult> LoadStaffs()
        {
            var paymentTypeTask = new LoadStaffListTask(false, true, Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI);
            yield return paymentTypeTask;
            Staffs = paymentTypeTask.StaffList;
            if (Staffs == null || Staffs.Count < 1)
            {
                RegistrationDetailsContent.DoctorStaffID = 0;
            }
            foreach (var item in Staffs)
            {
                if (item.StaffID == Globals.LoggedUserAccount.StaffID)
                {
                    NotifyOfPropertyChange(() => RegistrationDetailsContent.DoctorStaffID);
                }
            }
            yield break;
        }
        #endregion
        private new void NotifyWhenBusy()
        {
            base.NotifyWhenBusy();
            NotifyButtonBehaviourChanges();
        }
        private bool _isCalculatingPayment;
        public bool IsCalculatingPayment
        {
            get
            {
                return _isCalculatingPayment;
            }
            set
            {
                _isCalculatingPayment = value;
                NotifyOfPropertyChange(() => IsCalculatingPayment);

                NotifyWhenBusy();
            }
        }
        private void ShowOldRegistration(PatientRegistration regInfo, bool fromAppointment = false)
        {
            if (CurRegistration != null)
            {
                CurRegistration = regInfo;
                //Chuyen sang mode giong nhu mo lai dang ky cu
                CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_OPENED;
                _confirmedHiItem = CurRegistration.HealthInsurance;
                _confirmedPaperReferal = CurRegistration.PaperReferal;
                NotifyOfPropertyChange(() => ConfirmedHiItem);
                NotifyOfPropertyChange(() => ConfirmedPaperReferal);
                NotifyOfPropertyChange(() => RegistrationTitle);
                Coroutine.BeginExecute(InitRegistrationFullInfo());
                //InitRegistration();

                if (PatientSummaryInfoContent != null)
                {
                    if (CurRegistration.AppointmentID != null && CurRegistration.AppointmentID > 0 && CurRegistration.AppointmentDate != null)
                    {
                        //Đúng hẹn
                        if (CurRegistration.AppointmentDate.Value.Date == Globals.ServerDate.Value.Date)
                        {
                            TitleStatus = RegStatus.CuocHen_DungHen;
                        }
                        else
                        {
                            TitleStatus = RegStatus.CuocHen_TraiHen;
                        }
                    }
                    if (CurRegistration.PtInsuranceBenefit.HasValue)
                    {
                        TitleStatus = RegStatus.DangKyCoBH;
                        PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);
                        PatientSummaryInfoContent.ThongTuyen = CurRegistration.IsAllowCrossRegion;
                    }

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

                if (RegistrationDetailsContent != null)
                {
                    RegistrationDetailsContent.SetRegistration(CurRegistration);
                    RegistrationDetailsContent.HiServiceBeingUsed = HiServiceBeingUsed;
                    RegistrationDetailsContent.StartAddingNewServicesAndPclCmd(fromAppointment);
                }
                if (CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING)
                {
                    CanAddService = false;
                }
            }
        }
        private bool _registrationLoading;
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

        private bool _registrationCancelling;
        /// <summary>
        /// Dang trong qua trinh lay thong tin dang ky tu server.
        /// </summary>
        public bool RegistrationCancelling
        {
            get
            {
                return _registrationCancelling;
            }
            set
            {
                _registrationCancelling = value;
                NotifyOfPropertyChange(() => RegistrationCancelling);

                NotifyWhenBusy();
            }
        }
        /// <summary>
        /// Mở đăng ký đã có sẵn
        /// Lên server lấy đầy đủ thông tin của đăng ký
        /// </summary>
        /// <param name="regID">ID của đăng ký</param>
        public void OpenRegistration(long regID)
        {
            //IsLoadingRegistration = true;
            this.ShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) =>
            {
                //IsLoadingRegistration = false; 
                this.HideBusyIndicator();
            });
        }



        private bool _patientLoading;
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
        public bool CanCancelRegistrationCmd
        {
            get
            {
                return CurRegistration != null
                    && (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED || CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING)
                    && RegistrationInfoHasChanged == false;
            }
        }
        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mDangKyDV_DichVuDaTT_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mRegister,
                                             (int)oRegistrionEx.mDangKyDV_DichVuDaTT_ChinhSua, (int)ePermission.mView);
            mDangKyDV_DichVuDaTT_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuDaTT_Luu, (int)ePermission.mView);
            mDangKyDV_DichVuDaTT_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuDaTT_TraTien, (int)ePermission.mView);
            mDangKyDV_DichVuDaTT_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuDaTT_In, (int)ePermission.mView);
            mDangKyDV_DichVuDaTT_LuuTraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuDaTT_LuuTraTien, (int)ePermission.mView);

            mDangKyDV_DichVuMoi_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuMoi_ChinhSua, (int)ePermission.mView);
            mDangKyDV_DichVuMoi_Luu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuMoi_Luu, (int)ePermission.mView);
            mDangKyDV_DichVuMoi_TraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuMoi_TraTien, (int)ePermission.mView);
            mDangKyDV_DichVuMoi_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuMoi_In, (int)ePermission.mView);
            mDangKyDV_DichVuMoi_LuuTraTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVuMoi_LuuTraTien, (int)ePermission.mView);


            mDangKyDV_DichVu_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DichVu_Them, (int)ePermission.mView);
            mDangKyDV_DangKyMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_DangKyMoi, (int)ePermission.mView);
            mDangKyDV_ChuyenSangNoiTru = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_ChuyenSangNoiTru, (int)ePermission.mView);
            mDangKyDV_HenBenh = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_HenBenh, (int)ePermission.mView);
            mDangKyDV_HuyDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                               , (int)ePatient.mRegister,
                                               (int)oRegistrionEx.mDangKyDV_HuyDangKy, (int)ePermission.mView);

            //phan nay nam trong module chung ne
            mDangKyDV_Patient_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                             , (int)ePatient.mRegister,
                                             (int)oRegistrionEx.mDangKyDV_Patient_TimBN, (int)ePermission.mView);
            mDangKyDV_Patient_ThemBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mDangKyDV_Patient_ThemBN, (int)ePermission.mView);
            mDangKyDV_Patient_TimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mDangKyDV_Patient_TimDangKy, (int)ePermission.mView);

            mDangKyDV_Info_CapNhatThongTinBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mDangKyDV_Info_CapNhatThongTinBN, (int)ePermission.mView);
            mDangKyDV_Info_XacNhan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mDangKyDV_Info_XacNhan, (int)ePermission.mView);
            mDangKyDV_Info_XoaThe = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mDangKyDV_Info_XoaThe, (int)ePermission.mView);
            mDangKyDV_Info_XemPhongKham = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPatient
                                                 , (int)ePatient.mRegister,
                                                 (int)oRegistrionEx.mDangKyDV_Info_XemPhongKham, (int)ePermission.mView);

        }
        #region checking account



        private bool _mDangKyDV_DichVu_Them = true;
        private bool _mDangKyDV_DangKyMoi = true;
        private bool _mDangKyDV_ChuyenSangNoiTru = true;
        private bool _mDangKyDV_HenBenh = true;
        private bool _mDangKyDV_HuyDangKy = true;


        private bool _mDangKyDV_DichVuDaTT_ChinhSua = true;
        private bool _mDangKyDV_DichVuDaTT_Luu = true;
        private bool _mDangKyDV_DichVuDaTT_TraTien = true;
        private bool _mDangKyDV_DichVuDaTT_In = true;
        private bool _mDangKyDV_DichVuDaTT_LuuTraTien = true;

        private bool _mDangKyDV_DichVuMoi_ChinhSua = true;
        private bool _mDangKyDV_DichVuMoi_Luu = true;
        private bool _mDangKyDV_DichVuMoi_TraTien = true;
        private bool _mDangKyDV_DichVuMoi_In = true;
        private bool _mDangKyDV_DichVuMoi_LuuTraTien = true;





        public bool mDangKyDV_DichVu_Them
        {
            get
            {
                return _mDangKyDV_DichVu_Them;
            }
            set
            {
                if (_mDangKyDV_DichVu_Them == value)
                    return;
                _mDangKyDV_DichVu_Them = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVu_Them);
            }
        }

        public bool mDangKyDV_DangKyMoi
        {
            get
            {
                return _mDangKyDV_DangKyMoi;
            }
            set
            {
                if (_mDangKyDV_DangKyMoi == value)
                    return;
                _mDangKyDV_DangKyMoi = value;
                NotifyOfPropertyChange(() => mDangKyDV_DangKyMoi);
            }
        }

        public bool mDangKyDV_ChuyenSangNoiTru
        {
            get
            {
                return _mDangKyDV_ChuyenSangNoiTru;
            }
            set
            {
                if (_mDangKyDV_ChuyenSangNoiTru == value)
                    return;
                _mDangKyDV_ChuyenSangNoiTru = value;
                NotifyOfPropertyChange(() => mDangKyDV_ChuyenSangNoiTru);
            }
        }

        public bool mDangKyDV_HenBenh
        {
            get
            {
                return _mDangKyDV_HenBenh;
            }
            set
            {
                if (_mDangKyDV_HenBenh == value)
                    return;
                _mDangKyDV_HenBenh = value;
                NotifyOfPropertyChange(() => mDangKyDV_HenBenh);
            }
        }

        public bool mDangKyDV_HuyDangKy
        {
            get
            {
                return _mDangKyDV_HuyDangKy;
            }
            set
            {
                if (_mDangKyDV_HuyDangKy == value)
                    return;
                _mDangKyDV_HuyDangKy = value;
                NotifyOfPropertyChange(() => mDangKyDV_HuyDangKy);
            }
        }


        public bool mDangKyDV_DichVuDaTT_ChinhSua
        {
            get
            {
                return _mDangKyDV_DichVuDaTT_ChinhSua;
            }
            set
            {
                if (_mDangKyDV_DichVuDaTT_ChinhSua == value)
                    return;
                _mDangKyDV_DichVuDaTT_ChinhSua = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuDaTT_ChinhSua);
            }
        }

        public bool mDangKyDV_DichVuDaTT_Luu
        {
            get
            {
                return _mDangKyDV_DichVuDaTT_Luu;
            }
            set
            {
                if (_mDangKyDV_DichVuDaTT_Luu == value)
                    return;
                _mDangKyDV_DichVuDaTT_Luu = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuDaTT_Luu);
            }
        }

        public bool mDangKyDV_DichVuDaTT_TraTien
        {
            get
            {
                return _mDangKyDV_DichVuDaTT_TraTien;
            }
            set
            {
                if (_mDangKyDV_DichVuDaTT_TraTien == value)
                    return;
                _mDangKyDV_DichVuDaTT_TraTien = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuDaTT_TraTien);
            }
        }

        public bool mDangKyDV_DichVuDaTT_In
        {
            get
            {
                return _mDangKyDV_DichVuDaTT_In;
            }
            set
            {
                if (_mDangKyDV_DichVuDaTT_In == value)
                    return;
                _mDangKyDV_DichVuDaTT_In = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuDaTT_In);
            }
        }

        public bool mDangKyDV_DichVuDaTT_LuuTraTien
        {
            get
            {
                return _mDangKyDV_DichVuDaTT_LuuTraTien;
            }
            set
            {
                if (_mDangKyDV_DichVuDaTT_LuuTraTien == value)
                    return;
                _mDangKyDV_DichVuDaTT_LuuTraTien = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuDaTT_LuuTraTien);
            }
        }

        public bool mDangKyDV_DichVuMoi_ChinhSua
        {
            get
            {
                return _mDangKyDV_DichVuMoi_ChinhSua;
            }
            set
            {
                if (_mDangKyDV_DichVuMoi_ChinhSua == value)
                    return;
                _mDangKyDV_DichVuMoi_ChinhSua = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuMoi_ChinhSua);
            }
        }

        public bool mDangKyDV_DichVuMoi_Luu
        {
            get
            {
                return _mDangKyDV_DichVuMoi_Luu;
            }
            set
            {
                if (_mDangKyDV_DichVuMoi_Luu == value)
                    return;
                _mDangKyDV_DichVuMoi_Luu = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuMoi_Luu);
            }
        }

        public bool mDangKyDV_DichVuMoi_TraTien
        {
            get
            {
                return _mDangKyDV_DichVuMoi_TraTien;
            }
            set
            {
                if (_mDangKyDV_DichVuMoi_TraTien == value)
                    return;
                _mDangKyDV_DichVuMoi_TraTien = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuMoi_TraTien);
            }
        }

        public bool mDangKyDV_DichVuMoi_In
        {
            get
            {
                return _mDangKyDV_DichVuMoi_In;
            }
            set
            {
                if (_mDangKyDV_DichVuMoi_In == value)
                    return;
                _mDangKyDV_DichVuMoi_In = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuMoi_In);
            }
        }

        public bool mDangKyDV_DichVuMoi_LuuTraTien
        {
            get
            {
                return _mDangKyDV_DichVuMoi_LuuTraTien;
            }
            set
            {
                if (_mDangKyDV_DichVuMoi_LuuTraTien == value)
                    return;
                _mDangKyDV_DichVuMoi_LuuTraTien = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVuMoi_LuuTraTien);
            }
        }

        //phan nay nam trong module chung
        private bool _mDangKyDV_Patient_TimBN = true;
        private bool _mDangKyDV_Patient_ThemBN = true;
        private bool _mDangKyDV_Patient_TimDangKy = true;

        private bool _mDangKyDV_Info_CapNhatThongTinBN = true;
        private bool _mDangKyDV_Info_XacNhan = true;
        private bool _mDangKyDV_Info_XoaThe = true;
        private bool _mDangKyDV_Info_XemPhongKham = true;

        public bool mDangKyDV_Patient_TimBN
        {
            get
            {
                return _mDangKyDV_Patient_TimBN;
            }
            set
            {
                if (_mDangKyDV_Patient_TimBN == value)
                    return;
                _mDangKyDV_Patient_TimBN = value;
                NotifyOfPropertyChange(() => mDangKyDV_Patient_TimBN);
            }
        }

        public bool mDangKyDV_Patient_ThemBN
        {
            get
            {
                return _mDangKyDV_Patient_ThemBN;
            }
            set
            {
                if (_mDangKyDV_Patient_ThemBN == value)
                    return;
                _mDangKyDV_Patient_ThemBN = value;
                NotifyOfPropertyChange(() => mDangKyDV_Patient_ThemBN);
            }
        }

        public bool mDangKyDV_Patient_TimDangKy
        {
            get
            {
                return _mDangKyDV_Patient_TimDangKy;
            }
            set
            {
                if (_mDangKyDV_Patient_TimDangKy == value)
                    return;
                _mDangKyDV_Patient_TimDangKy = value;
                NotifyOfPropertyChange(() => mDangKyDV_Patient_TimDangKy);
            }
        }

        public bool mDangKyDV_Info_CapNhatThongTinBN
        {
            get
            {
                return _mDangKyDV_Info_CapNhatThongTinBN;
            }
            set
            {
                if (_mDangKyDV_Info_CapNhatThongTinBN == value)
                    return;
                _mDangKyDV_Info_CapNhatThongTinBN = value;
                NotifyOfPropertyChange(() => mDangKyDV_Info_CapNhatThongTinBN);
            }
        }

        public bool mDangKyDV_Info_XacNhan
        {
            get
            {
                return _mDangKyDV_Info_XacNhan;
            }
            set
            {
                if (_mDangKyDV_Info_XacNhan == value)
                    return;
                _mDangKyDV_Info_XacNhan = value;
                NotifyOfPropertyChange(() => mDangKyDV_Info_XacNhan);
            }
        }

        public bool mDangKyDV_Info_XoaThe
        {
            get
            {
                return _mDangKyDV_Info_XoaThe;
            }
            set
            {
                if (_mDangKyDV_Info_XoaThe == value)
                    return;
                _mDangKyDV_Info_XoaThe = value;
                NotifyOfPropertyChange(() => mDangKyDV_Info_XoaThe);
            }
        }

        public bool mDangKyDV_Info_XemPhongKham
        {
            get
            {
                return _mDangKyDV_Info_XemPhongKham;
            }
            set
            {
                if (_mDangKyDV_Info_XemPhongKham == value)
                    return;
                _mDangKyDV_Info_XemPhongKham = value;
                NotifyOfPropertyChange(() => mDangKyDV_Info_XemPhongKham);
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
        #endregion
        public bool CanReportRegistrationInfoInsuranceCmd
        {
            get { return CurRegistration != null && CurRegistration.PtRegistrationID > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0; }
        }
        public void SetEkip()
        {
            if (CurRegistration.PtInsuranceBenefit == null || CurRegistration.PtInsuranceBenefit == 0)
            {
                MessageBox.Show(eHCMSResources.Z2979_G1_ErrorEkip, eHCMSResources.G0442_G1_TBao);
                return;
            }
            ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetail = new ObservableCollection<PatientRegistrationDetail>();
            //20200208 TBL: BM 0022891: Thay đổi cách lấy dịch vụ để thêm ekip
            //ObsPatientRegistrationDetail = new ObservableCollection<PatientRegistrationDetail>(RegistrationDetailsContent.tempRegistration.PatientRegistrationDetails.Where(x => x.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
            //                                && x.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT
            //                                && x.RefMedicalServiceItem.HIAllowedPrice > 0 && x.IsCountHI));
            if (RegistrationDetailsContent.CurrentRegistration != null && RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails != null && RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails.Count > 0)
            {
                foreach (PatientRegistrationDetail item in RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails)
                {
                    if (((item.PtRegDetailID > 0 && item.PaidTime != null && item.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI) || (item.PtRegDetailID > 0 && (long)item.RecordState != 3) || item.PtRegDetailID <= 0)
                        && item.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.THUTHUAT
                        && item.RefMedicalServiceItem.HIAllowedPrice > 0 && item.IsCountHI)
                    {
                        ObsPatientRegistrationDetail.Add(item);
                    }
                }
            }
            if (ObsPatientRegistrationDetail.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2733_G1_KhongCoDVThietLapEkip, eHCMSResources.G0442_G1_TBao);
                return;
            }
            ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetailCopy = new ObservableCollection<PatientRegistrationDetail>(RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails.DeepCopy());

            ISetEkipForMedicalService aView = Globals.GetViewModel<ISetEkipForMedicalService>();
            aView.CurrentRegistration = new PatientRegistration();
            aView.CurrentRegistration.PtRegistrationID = RegistrationDetailsContent.tempRegistration.PtRegistrationID;
            aView.CurrentRegistration.PatientRegistrationDetails = ObsPatientRegistrationDetail;
            GlobalsNAV.ShowDialog_V3<ISetEkipForMedicalService>(aView, null, null, false, false);
            if (!aView.SaveOK) //20200509 TBL: Nếu không lưu thì trả về như trước lúc thiết lập
            {
                //foreach (PatientRegistrationDetail item in aView.CurrentRegistration.PatientRegistrationDetails)
                //{
                //    item.V_Ekip = null;
                //    item.V_EkipIndex = null;
                //}
                RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails = ObsPatientRegistrationDetailCopy;
                RegistrationDetailsContent.RefreshServicesView();
            }
            else //20200509 TBL: Nếu lưu những ekip đã thiết lập thì bắn sự kiện
            {
                Globals.EventAggregator.Publish(new SetEkipForServiceSuccess { RegistrationInfo = aView.CurrentRegistration });
            }
        }

        //▼====: #003
        private AllLookupValues.PopupModifyPrice_Type _PopupModifyPrice_Type;
        public AllLookupValues.PopupModifyPrice_Type PopupModifyPrice_Type
        {
            get
            {
                return _PopupModifyPrice_Type;
            }
            set
            {
                _PopupModifyPrice_Type = value;
                NotifyOfPropertyChange(() => PopupModifyPrice_Type);
            }
        }

        private string _ReasonChangePrice;
        public string ReasonChangePrice
        {
            get
            {
                return _ReasonChangePrice;
            }
            set
            {
                _ReasonChangePrice = value;
                NotifyOfPropertyChange(() => ReasonChangePrice);
            }
        }

        public void Handle(ModifyPriceToInsert_Completed message)
        {
            if (message == null || message.ModifyItem == null)
            {
                return;
            }
            if (!message.ModifyItem.IsModItemOK)
            {
                return;
            }
            ReasonChangePrice = message.ModifyItem.ReasonChangePrice;
            if (PopupModifyPrice_Type == AllLookupValues.PopupModifyPrice_Type.INSERT_DICHVU)
            {
                MedServiceItem.HIAllowedPrice = message.ModifyItem.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
                MedServiceItem.NormalPrice = message.ModifyItem.ChargeableItem.NormalPrice;
                MedServiceItem.HIPatientPrice = message.ModifyItem.ChargeableItem.NormalPrice;
                RegistrationDetailsContent.AddNewService(ObjectCopier.DeepCopy(MedServiceItem), DeptLocation != null ? DeptLocation : null, MedServiceType, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
            }
        }
        //▲====: #003
    }
}