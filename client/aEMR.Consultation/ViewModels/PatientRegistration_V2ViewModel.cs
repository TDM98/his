using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Registration.ViewModels;
using System.Windows.Controls;
/*
* 20181005 #001 TNHX:  [BM0000034] Add Report PhieuChiDinh
* 20181129 #002 TNHX:  [BM0005312] Add Report PhieuMienGiam
* 20181208 #003 TTM:   BM 0005255
* 20181212 #004 TTM:   BM 0005413: Thêm trường Chẩn đoán ban đầu và lưu lại (BasicDiagTreatment).
* 20181225 #005 TNHX:  [BM0005462] Re-make report PhieuChiDinh
* 20190409 #006 TTM:   BM 0006662:
* 20190419 #007 TTM:   BM 0006758: Ngăn không cho đăng ký vượt quá số lượng dịch vụ khám bệnh có BH do bệnh viện quy định cho 1 ca BH trong 1 ngày.
* 20190502 #008 TTM:   BM 0006818: Lấy giá cho 1 ca load từ cuộc hẹn.
* 20190707 #009 TTM:   BM 0011892: Fix lỗi không kiểm tra số tiền còn dư khi huỷ đăng ký => Do chuyển từ PatientPayment sang OutPatientCashAdvance dẫn đến By pass điều kiện kiểm tra.
* 20190803 #010 TTM:   BM 0013069: Fix lỗi không cập nhật thông tin thẻ và mức hưởng ở PatientSummaryInfoV3 khi load 1 đăng ký không bảo hiểm sau 1 đăng ký có bảo hiểm 
*                                  => Quyền lợi và thông tin thẻ của đăng ký trước vẫn còn thể hiện. 
* 20191001 #011 TTM:   BM 0017390: [Khám bệnh mới] OST bị đổi khi popup chỉ định dịch vụ hiển thị
* 20191105 #012 TTM:   BM 0018535: Lỗi cảnh báo ngoài phác đồ điều trị trên màn hình đăng ký dịch vụ ngoại trú cho bệnh nhân mới sau khi load bệnh nhân cũ.
* 20191127 #013 TNHX:  BM 0013220: Thêm DV giá dao động cho ngoại trú
* 20200725 #014 TTM:   BM 0022848: Bổ sung kiểm tra phác đồ cho chỉ định khám bệnh ngoại trú.
* 20200810 #015 TTM:    BM 0039422: Bổ sung code kiểm tra phác đồ nếu bệnh nhân có nhiều ICD cùng 1 phác đồ thì chỉ insert dữ liệu dịch vụ 1 lần thôi.
* 20210701 #016 TNHX: 260 Thêm user bsi mượn
* 20211004 #017 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
* 20221215 #018 QTD:  Nhắc CLS
* 20221228 #019 QTD:  Thêm cấu hình hiển thị gọi dịch vụ màn hình chỉ định dịch vụ của bác sĩ
* 20230109 #020 QTD:  Thêm điều kiện đánh dấu lưu chỉ định từ màn hình chỉ định dịch vụ bác sĩ
* 20230516 #021 QTD:  Thêm ICD10
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientRegistration_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientRegistration_V2ViewModel : HandlePayCompletedViewModelBase, IPatientRegistration_V2
        , IHandle<ItemSelected<Patient>>
        , IHandle<ItemSelected<PatientRegistration>>
        , IHandle<CreateNewPatientEvent>
        , IHandle<HiCardConfirmedEvent>
        , IHandle<ResultFound<Patient>>
        , IHandle<ResultNotFound<Patient>>
        , IHandle<AddCompleted<Patient>>
        , IHandle<PayForRegistrationCompleted>
        , IHandle<SaveAndPayForRegistrationCompleted>
        , IHandle<AddCompleted<PatientRegistration>>
        , IHandle<SimplePayViewCancelCloseEvent>
        , IHandle<UpdateCompleted<PatientRegistration>>
        /*▼====: #001*/
        , IHandle<PhieuChiDinhForRegistrationCompleted>
        /*▲====: #001*/
        //▼====: #002
        , IHandle<PhieuMienGiamForRegistrationCompleted>
        //▲====: #002
        , IHandle<ItemSelecting<object, PatientAppointment>>
        , IHandle<ConfirmHiBenefitEvent>
        , IHandle<DoubleClick>
        , IHandle<DoubleClickAddReqLAB>
        // DPT 
        , IHandle<UpdateCompleted<Patient>>
        , IHandle<SetBasicDiagTreatmentForRegistrationSummaryV2>
        , IHandle<SetCurRegistrationForPatientSummaryInfoV3>
        , IHandle<ModifyPriceToInsert_Completed>
    {
        IEventAggregator _eventArg;
        [ImportingConstructor]
        public PatientRegistration_V2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            _eventArg = eventArg;
            Initialize();

            var patientInfoVm = Globals.GetViewModel<IPatientSummaryInfoV3>();
            patientInfoVm.mInfo_CapNhatThongTinBN = mDangKyDV_Info_CapNhatThongTinBN;
            patientInfoVm.mInfo_XacNhan = mDangKyDV_Info_XacNhan;
            patientInfoVm.mInfo_XoaThe = mDangKyDV_Info_XoaThe;
            patientInfoVm.mInfo_XemPhongKham = mDangKyDV_Info_XemPhongKham;

            PatientSummaryInfoContent = patientInfoVm;
            ActivateItem(patientInfoVm);

            var regDetailsVm = Globals.GetViewModel<IRegistrationSummaryV2>();
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
            RegistrationDetailsContent.TreatmentRegimenCollection = TreatmentRegimenCollection;
            RegistrationDetailsContent.gCallCloseDialog += () =>
            {
                TryClose();
            };
            ActivateItem(regDetailsVm);
            regDetailsVm.ValidateRegistration = ValidateRegInfo;
            ((INotifyPropertyChangedEx)regDetailsVm).PropertyChanged += regDetailsVm_PropertyChanged;

            var selectPclVm = Globals.GetViewModel<IInPatientSelectPcl>();
            SelectPCLContent = selectPclVm;
            SelectPCLContent.ShowUsedField = false;
            SelectPCLContent.ShowLocationSelection = true;
            ActivateItem(selectPclVm);

            //tab LAB
            var selectPclVmLAB = Globals.GetViewModel<IInPatientSelectPclLAB>();
            SelectPCLContentLAB = selectPclVmLAB;
            SelectPCLContentLAB.ShowUsedField = false;
            SelectPCLContentLAB.ShowLocationSelection = true;
            ActivateItem(selectPclVmLAB);
            //tab LAB
           
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                GetServiceTypes();
            }
            Authorization();
            GetConsultationRoomTargetTSegment();
            LoadDoctorStaffCollection();
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
            //▼===== #014
            IsRegimenChecked = Globals.ServerConfigSection.ConsultationElements.CheckedTreatmentRegimen;
            //▲===== #014
        }

        void regDetailsVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSavingRegistration")
            {
                NotifyWhenBusy();
            }
        }

        void searchPatientAndRegVm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == eHCMSResources.Z0113_G1_IsLoading || e.PropertyName == eHCMSResources.Z0114_G1_IsSearchingRegistration)
            {
                NotifyWhenBusy();
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPatientRegistrationView;
            Authorization();
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

        private bool _mCuocHen_KSK = false;
        public bool mCuocHen_KSK
        {
            get
            {
                return _mCuocHen_KSK;
            }
            set
            {
                _mCuocHen_KSK = value;
                NotifyOfPropertyChange(() => mCuocHen_KSK);
                NotifyOfPropertyChange(() => mDangKyChuaBH);
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
            get
            {
                return _mDangKyChuaBH && !mCuocHen_KSK;
            }
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

        private IPatientRegistrationView _currentView;

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            if (ViewCase == RegistrationViewCase.MedicalServiceGroupView)
            {
                ViewTitle = eHCMSResources.T1280_G1_GoiDV;
            }
            //▼===== #011: Không load OST của PatientRegistrationViewModel mà chỉ sử dụng OST của màn hình khám bệnh.
            if (ViewCase != RegistrationViewCase.RegistrationRequestView)
            {
                var homeVm = Globals.GetViewModel<IHome>();
                homeVm.OutstandingTaskContent = Globals.GetViewModel<IRegistrationOutStandingTask>();
                homeVm.IsExpandOST = true;
            }
            //▲===== 
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼===== #011: Không load OST của PatientRegistrationViewModel mà chỉ sử dụng OST của màn hình khám bệnh.
            if (ViewCase != RegistrationViewCase.RegistrationRequestView)
            {
                var homeVm = Globals.GetViewModel<IHome>();
                homeVm.OutstandingTaskContent = null;
                homeVm.IsExpandOST = false;
            }
            //▲===== 
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

        private IRegistrationSummaryV2 _registrationDetailsContent;
        public IRegistrationSummaryV2 RegistrationDetailsContent
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
                //InitRegistrationForPatient();
            }
        }

        private PatientRegistration _curRegistration;

        public PatientRegistration CurRegistration
        {
            get { return _curRegistration; }
            set
            {
                if (_curRegistration != value)
                {
                    _curRegistration = value;
                    //▼====== #003: Set CurrentPatient bằng thông tin bệnh nhân từ đăng ký
                    //              Lý do set: Vì visible của nút các lần đăng ký trước dựa vào CurrentPatient mà tìm kiếm đăng ký thì không set => không hiển thị. Set để hiển thị
                    if (_curRegistration.Patient != null)
                    {
                        CurrentPatient = _curRegistration.Patient;
                    }
                    //▲====== #003
                    //▼====== #004
                    if (_curRegistration != null)
                    {
                        PatientSummaryInfoContent.curRegistration = _curRegistration;
                    }
                    //▲====== #004
                }
                NotifyOfPropertyChange(() => CurRegistration);
                NotifyOfPropertyChange(() => CanCancelRegistrationCmd);
                // if (_curRegistration != null)
                //{
                //    IsEnableRoleUser = PermissionManager.IsEnableRoleUserStatic(_curRegistration.StaffID);
                //    if (RegistrationDetailsContent != null)
                //    {
                //        RegistrationDetailsContent.IsEnableRoleUser = IsEnableRoleUser;
                //    }
                //}
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

        //private PatientClassification _curClassification;
        public PatientClassification CurClassification
        {
            get
            {
                return PatientSummaryInfoContent.CurrentPatientClassification;
            }
            set
            {
                PatientSummaryInfoContent.ApplySpecialPatientClass(value);
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
                    //Load lại danh sách service tương ứng
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
                        //else
                        //{
                        //    DefaultPclExamTypeContent.MedServiceID = medServiceID;
                        //}
                    }
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
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                if (gSelectedDoctorStaff != null && gSelectedDoctorStaff.StaffID > 0)
                {
                    RegistrationDetailsContent.RequestDoctorStaffID = gSelectedDoctorStaff.StaffID;
                }
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

        private string _Diagnosis;
        public string Diagnosis
        {
            get
            {
                return _Diagnosis;
            }
            set
            {
                if (_Diagnosis != value)
                {
                    _Diagnosis = value;
                    NotifyOfPropertyChange(() => Diagnosis);
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

        //20200222 TBL Mod TMV1: Cờ liệu trình
        private bool _IsProcess;
        public bool IsProcess
        {
            get { return _IsProcess; }
            set
            {
                if (_IsProcess != value)
                {
                    _IsProcess = value;
                    NotifyOfPropertyChange(() => IsProcess);
                    RegistrationDetailsContent.IsProcess = IsProcess;
                }
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
            //PatientSummaryInfoContent.ConfirmedHiItem = ConfirmedHiItem;
        }

        public void ConfirmPaperReferal(object referal)
        {
            ConfirmedPaperReferal = referal as PaperReferal;
            if (CurRegistration != null)
            {
                CurRegistration.PaperReferal = ConfirmedPaperReferal;
            }
            //PatientSummaryInfoContent.ConfirmedPaperReferal = ConfirmedPaperReferal;
        }

        #region EVENT HANDLERS
        public void Handle(ItemSelecting<object, PatientAppointment> message)
        {
            if (message != null)
            {
                // TxD 16/04/2018 Begin: Added the following to fix bug described in Bitrix24 Task#217
                //  Health Insurance Details from previously opened Registration NOT CLEAR
                if (PatientSummaryInfoContent != null)
                {
                    PatientSummaryInfoContent.SetPatientHISumInfo(null);
                }
                // TxD 16/04/2018 End
                Coroutine.BeginExecute(DoCheckAppointment(message));
            }
        }
        private void CallShowBusyIndicator(string BusyContent = null)
        {
            if (ViewCase == RegistrationViewCase.RegistrationRequestView)
            {
                this.DlgShowBusyIndicator(BusyContent);
            }
            else
            {
                this.ShowBusyIndicator(BusyContent);
            }
        }
        private void CallHideBusyIndicator()
        {
            if (ViewCase == RegistrationViewCase.RegistrationRequestView)
            {
                this.DlgHideBusyIndicator();
            }
            else
            {
                this.HideBusyIndicator();
            }
        }
        public void CheckAppointment(PatientAppointment Appointment)
        {
            CallShowBusyIndicator(eHCMSResources.Z0630_G1_DangLayTTinCHen);
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
                                    CallHideBusyIndicator();
                                }
                                if (bOK)
                                {
                                    CurrentPatient = regInfo.Patient;
                                    RegistrationDetailsContent.Reset();
                                    //Show du lieu len:
                                    ShowOldRegistration(regInfo, Appointment, true);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    //IsLoadingAppointment = false;
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    CallHideBusyIndicator();
                }
            });
            t.Start();
        }

        public IEnumerator<IResult> DoCheckAppointment(ItemSelecting<object, PatientAppointment> message)
        {

            if (_currentPatient != null)
            {
                if (message.Item.Patient.PatientID != _currentPatient.PatientID)
                {
                    var str = string.Format("{0} '{1}'.\n{2} '{3}'", eHCMSResources.Z0175_G1_BanDangThaoTacBN, _currentPatient.FullName, eHCMSResources.Z0191_G1_BanCoMuonChSangDKBN, message.Item.Patient.FullName);

                    if (MessageBox.Show(str, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        message.Cancel = true;
                        yield break;
                    }
                }
            }
            if (message.Item.HosClientContractID > 0 && message.Item.V_ApptStatus == AllLookupValues.ApptStatus.UNKNOWN)
            {
                CheckAppointment(message.Item);
                if (message.Sender is IFindAppointment)
                {
                    ((ViewModelBase)message.Sender).TryClose();
                }
                if (message.Sender is IFindAppointmentKSK)
                {
                    ((ViewModelBase)message.Sender).TryClose();
                }
                yield break;
            }
            else if (message.Item.V_ApptStatus != AllLookupValues.ApptStatus.BOOKED)
            {
                switch (message.Item.V_ApptStatus)
                {
                    case (AllLookupValues.ApptStatus.ACTIONED):
                        var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z0192_G1_CuocHenNayDaDuocXNhan, eHCMSResources.Z0195_G1_VanMoCuocHen);
                        yield return dialog;
                        if (dialog.IsAccept)
                        {
                            CheckAppointment(message.Item);
                            if (message.Sender is IFindAppointment)
                            {
                                ((ViewModelBase)message.Sender).TryClose();
                            }
                        }
                        break;
                    case (AllLookupValues.ApptStatus.PENDING):
                        var dialog1 = new MessageWarningShowDialogTask(eHCMSResources.Z0193_G1_CuocHenNayDangChoXNhan, eHCMSResources.K1576_G1_CBao, false);
                        yield return dialog1;
                        break;
                    case (AllLookupValues.ApptStatus.WAITING):
                        var dialog2 = new MessageWarningShowDialogTask(eHCMSResources.Z0194_G1_CuocHenNayDangChoDuyet, eHCMSResources.K1576_G1_CBao, false);
                        yield return dialog2;
                        break;
                }

                yield break;
            }

            CheckAppointment(message.Item);
            if (message.Sender is IFindAppointment)
            {
                ((ViewModelBase)message.Sender).TryClose();
            }
            if (message.Sender is IFindAppointmentKSK)
            {
                ((ViewModelBase)message.Sender).TryClose();
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
                if (message.bUpdate_CurrentPatient_Info_Only == false)
                {
                    Coroutine.BeginExecute(DoSetCurrentPatient(message.Item));
                }
            }
        }


        public void Handle(CreateNewPatientEvent message)
        {
            if (message != null)
            {
                PatientDetailsContent = null;
                PatientDetailsContent = Globals.GetViewModel<IPatientDetails>();
                PatientDetailsContent.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                PatientDetailsContent.IsChildWindow = true;
                PatientDetailsContent.InitLoadControlData_FromExt(null);
                PatientDetailsContent.CreateNewPatient();
                PatientDetailsContent.Enable_ReConfirmHI_InPatientOnly = true;
                GlobalsNAV.ShowDialog_V3<IPatientDetails>(PatientDetailsContent);
            }
        }

        public void Handle(HiCardConfirmedEvent evtMsg)
        {
            if (evtMsg == null || evtMsg.HiProfile == null)
                return;

            //if (evtMsg.HiProfile.HealthInsuranceHistories == null || evtMsg.HiProfile.HealthInsuranceHistories.Count == 0)
            //{
            //    MessageBox.Show("Khong the xac nhan The BHYT khong co HisID. Loi Ky thuat can xem lai.");
            //    return;
            //}

            //Tinh quyen loi bao hiem.
            Coroutine.BeginExecute
            (
                CalcHIBenefit_ForRegistration(evtMsg.HiProfile, evtMsg.PaperReferal, evtMsg.IsEmergInPtReExamination, evtMsg.IsHICard_FiveYearsCont
                    , evtMsg.IsChildUnder6YearsOld, evtMsg.IsAllowCrossRegion, evtMsg.IsHICard_FiveYearsCont_NoPaid
                    , evtMsg.FiveYearsAppliedDate, evtMsg.FiveYearsARowDate, evtMsg.V_ReceiveMethod),
                null,
                (o, e) =>
                {
                    var vm = evtMsg.Source as IPatientDetails;
                    if (vm != null)
                    {
                        vm.Close();
                    }
                }
            );

        }

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
        IPatientDetails PatientDetailsContent { get; set; }
        public void Handle(ResultNotFound<Patient> message)
        {
            if (message != null)
            {
                //Thông báo không tìm thấy bệnh nhân.
                MessageBoxResult result = MessageBox.Show(eHCMSResources.A0727_G1_Msg_ConfThemMoiBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    PatientDetailsContent = null;
                    PatientDetailsContent = Globals.GetViewModel<IPatientDetails>();
                    var criteria = message.SearchCriteria as PatientSearchCriteria;
                    PatientDetailsContent.CurrentAction = eHCMSResources.Z0037_G1_ThemBN;
                    PatientDetailsContent.Enable_ReConfirmHI_InPatientOnly = true;
                    PatientDetailsContent.CreateNewPatient();
                    PatientDetailsContent.IsChildWindow = true;
                    if (criteria != null)
                    {
                        PatientDetailsContent.CurrentPatient.FullName = criteria.FullName;
                        PatientDetailsContent.QRCode = criteria.QRCode;
                    }
                    GlobalsNAV.ShowDialog_V3<IPatientDetails>(PatientDetailsContent);
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
                        //if (CurrentPatient.QRCode != null && PatientDetailsContent != null)
                        //{
                        //    PatientDetailsContent.LoadPatientDetailsAndHI_V2(CurrentPatient, false);
                        //}
                    }
                    if (PatientDetailsContent != null)
                    {
                        PatientDetailsContent.LoadPatientDetailsAndHI_V2(CurrentPatient, false);
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
                    if (message.RefreshItemFromReturnedObj)
                    {
                        // RefreshRegistration(message.Registration);
                    }
                    else
                    {
                        OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);
                    }

                    ProcessPayCompletedEvent(message);
                }
            }
        }

        //▼====: #005
        //▼====: #001
        public void Handle(PhieuChiDinhForRegistrationCompleted message)
        {
            if (message != null)
            {
                ProcessPrintPhieuChiDinhEvent(message);
            }
        }
        //▲====: #001
        //▲====: #005
        //▼====: #002
        public void Handle(PhieuMienGiamForRegistrationCompleted message)
        {
            if (message != null)
            {
                //Load lai dang ky:
                var payment = message.Payment;
                if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                {
                    // TBR - TxD 28/04/2019: WHY ?!? OpenRegistration is CALLED HERE -- COMMENTed OUT FOR NOW -- TO BE REVIEWED UNTIL OK 
                    //OpenRegistration(payment.PatientTransaction.PtRegistrationID.Value);

                    ProcessPrintPhieuMienGiamEvent(message);
                }
            }
        }
        //▲====: #002
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
                        ShowOldRegistration(message.RegistrationInfo, null);
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
                                               //(long) AllLookupValues.V_RefMedicalServiceInOutOthers.HANHCHANH_NGOAITRU
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
                                //foreach (var item in ServiceTypes)
                                //{
                                //    GetAllMedicalServiceItemsByType(item);                                    
                                //}
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

        //KMx: Bỏ hàm CheckDeptLocValid(). Vì trường hợp: Quầy đăng ký không gán phòng được khi giờ đăng ký (giờ hiện tại) ngoài giờ "Chỉ tiêu phòng khám".
        // Hàm này không còn sử dụng nữa.
        //bool CheckDeptLocValid(long DeptLocationID)
        //{
        //    foreach (var crt in Globals.ConsultationRoomTarget)
        //    {
        //        if (crt.DeptLocationID == DeptLocationID)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

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
                                        //▼===== #014
                                        if (IsRegimenChecked)
                                        {
                                            if (item.MedicalServiceTypeID == (long)AllLookupValues.MedicalServiceTypeID.KHAMBENH)
                                            {
                                                item.RefMedicalServiceType = sType;
                                                col.Add(item);
                                            }
                                            foreach (var MedicalService in ListRegiment)
                                            {
                                                foreach (var detail in MedicalService.RefTreatmentRegimenServiceDetails)
                                                {
                                                    //▼===== #015
                                                    //if (detail.MedServiceID == item.MedServiceID)
                                                    if (detail.MedServiceID == item.MedServiceID && !col.Any(x => x.MedServiceID == detail.MedServiceID))
                                                    //▲===== #015
                                                    {
                                                        item.RefMedicalServiceType = sType;
                                                        col.Add(item);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            item.RefMedicalServiceType = sType;
                                            col.Add(item);
                                        }

                                    }
                                    //▲===== #014
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

        //KMx: Sau khi kiểm tra toàn bộ chương trình, thấy hàm này không còn sử dụng nữa.
        //Nếu có sử dụng lại hàm này thì chú ý hàm CheckDeptLocValid() ở bên trong.
        //Vì nếu gọi hàm CheckDeptLocValid() thì Quầy đăng ký không gán phòng được khi giờ đăng ký (giờ hiện tại) ngoài giờ "Chỉ tiêu phòng khám".
        public void GetLocationsByServiceID(long medServiceID)
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0605_G1_DangLayDSPK)
                });
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetLocationsByServiceID(medServiceID,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                IList<DeptLocation> allItem = null;
                                try
                                {
                                    allItem = contract.EndGetLocationsByServiceID(asyncResult);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }
                                if (allItem != null && allItem.Count > 0)
                                {
                                    //KMx: Nếu gọi hàm CheckDeptLocValid() thì Quầy đăng ký không gán phòng được khi giờ đăng ký (giờ hiện tại) ngoài giờ "Chỉ tiêu phòng khám".
                                    //DeptLocations = new ObservableCollection<DeptLocation>(allItem.Where(c => CheckDeptLocValid(c.DeptLocationID)));
                                }
                                else
                                {
                                    DeptLocations = new ObservableCollection<DeptLocation>();
                                }
                                if (allItem.Count > 1)
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

        private void GetLocationsByServiceIDFromCatche(long medServiceID)
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

            //20191207 TBL: BM 0019707: Thêm cấu hình Tự động phân bổ phòng
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

        /// <summary>
        /// Lấy lại những giá trị mặc định để đưa lên form
        /// </summary>
        //public void ResetToDefaultValues()
        //{
        //    ResetDepartmentToDefaultValue();
        //    ResetDocumentTypeToDefaultValue();
        //}
        //public void ResetDepartmentToDefaultValue()
        //{
        //    var loginVm = Globals.GetViewModel<ILogin>();
        //    if (loginVm.DeptLocation != null)
        //    {
        //        Department = loginVm.DeptLocation.RefDepartment;
        //    }
        //    else
        //    {
        //        Department = null;
        //    }
        //}
        public void ResetPatientClassificationToDefaultValue()
        {
            CurClassification = CreateDefaultClassification();
        }

        public void ResetDocumentTypeToDefaultValue()
        {
            //NewRegistration.V_DocumentTypeOnHold = -1;
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
            CallAddRegItemCmd();
        }
        private void CallAddRegItemCmd(long? ConsultationRoomStaffAllocID = null, DateTime? ApptStartDate = null, DateTime? ApptEndDate = null)
        {
            if (CurrentPatient == null)
            {
                Globals.ShowMessage(eHCMSResources.A0378_G1_Msg_InfoChuaChonBN, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (MedServiceItem == null || MedServiceItem.MedServiceID < 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0156_G1_Chon1DV, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (gSelectedDoctorStaff == null || gMedicalInstructionDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS);
                return;
            }
            //▼====== #007
            if (ViewCase != RegistrationViewCase.MedicalServiceGroupView)
            {
                if (CurRegistration == null || RegistrationDetailsContent == null || RegistrationDetailsContent.CurrentRegistration == null || RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails == null)
                {
                    return;
                }
                if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurRegistration, MedServiceItem, RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails))
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                    return;
                }
            }
            //▲====== #007
            //▼====: #013
            if (MedServiceItem.V_NewPriceType > 0 && (MedServiceItem.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Unknown_PriceType) || MedServiceItem.V_NewPriceType == Convert.ToInt32(AllLookupValues.V_NewPriceType.Updatable_PriceType)))
            {
                PopupModifyPrice_Type = AllLookupValues.PopupModifyPrice_Type.INSERT_DICHVU;
                PatientRegistrationDetail item = new PatientRegistrationDetail();
                //▼====: #016
                item.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
                //▲====: #016
                item.RefMedicalServiceItem = MedServiceItem.DeepCopy();
                // Hpt 21/11/2015: Nếu dịch vụ thuộc loại giá thay đổi hoặc không giá, sẽ cập nhật giá khi thêm dịch vụ vào bill nên phải lấy giá sau khi cập nhật chứ không thể lấy giá từ InPatientSelectServiceContent
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
                RegistrationDetailsContent.AddNewService(ObjectCopier.DeepCopy(MedServiceItem), DeptLocation != null ? DeptLocation : null, MedServiceType, ConsultationRoomStaffAllocID, ApptStartDate, ApptEndDate, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
            }
            //▲====: #013
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
                //20190409 TTM: cả if và else đều set CanAddService = true thì cần gì để if else.
                CanAddService = true;
                //if (_currentPatient != null && _currentPatient.PatientID > 0)
                //{

                //    CanAddService = true;
                //}
                //else
                //{
                //    CanAddService = true;

                //}
            }
            setPatientSummaryInfo(CurRegistration);//set lai thong tin cho benh nhan

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
            //if (CurRegistration.PatientID != null && CurRegistration.PatientID.Value > 0)
            //{
            //    var loadPatient = new LoadPatientTask(CurRegistration.PatientID.Value);
            //    yield return loadPatient;
            //    CurRegistration.Patient = loadPatient.CurrentPatient;
            //}
            setPatientSummaryInfo(CurRegistration);//set lai thong tin cho benh nhan

            NotifyOfPropertyChange(() => RegistrationTitle);

            // TxD Move the following outside of this method
            //InitFormData();
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

        //private bool _isLoadingAppointment;
        //public bool IsLoadingAppointment
        //{
        //    get { return _isLoadingAppointment; }
        //    set
        //    {
        //        _isLoadingAppointment = value;
        //        NotifyOfPropertyChange(() => IsLoadingAppointment);
        //    }
        //}

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
                //return _isRegistering || _isLoadingAppointment || _searchRegistrationContent.IsLoading || _searchRegistrationContent.IsSearchingRegistration
                //    || RegistrationDetailsContent.IsSavingRegistration || _registrationCreating || _registrationCancelling || _isLoadingRegistration;

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
                //if (_isRegistering)
                //{
                //    return "Đang lưu đăng ký";
                //}
                if (_registrationCancelling)
                {
                    return eHCMSResources.Z0608_G1_DangHuyDK;
                }
                //if (_isLoadingAppointment)
                //{
                //    return "Đang lấy thông tin cuộc hẹn";
                //}
                //if (_searchRegistrationContent.IsLoading)
                //{
                //    return "Đang tìm kiếm bệnh nhân";
                //}
                //if (_searchRegistrationContent.IsSearchingRegistration)
                //{
                //    return "Đang tìm đăng ký";
                //}
                //if (RegistrationDetailsContent.IsSavingRegistration)
                //{
                //    return "Đang lưu đăng ký";
                //}
                //if (_isLoadingRegistration)
                //{
                //    return eHCMSResources.Z0086_G1_DangLayTTinDK;
                //}
                return "";
            }
        }

        public void ResetToInitialState()
        {
            CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
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

        string CalcHiBenefitAndRegister_ErrorMsg = "";
        MessageWarningShowDialogTask warnDlgTask = null;

        public IEnumerator<IResult> CalcHIBenefit_ForRegistration(HealthInsurance hiItem, PaperReferal referal, bool IsEmergInPtReExamination
            , bool IsHICard_FiveYearsCont, bool IsChildUnder6YearsOld, bool IsAllowCrossRegion, bool IsHICard_FiveYearsCont_NoPaid
            , DateTime? FiveYearsAppliedDate, DateTime? FiveYearsARowDate, long V_ReceiveMethod)
        {
            // =========================================================================================================================

            if ((IsHICard_FiveYearsCont && IsChildUnder6YearsOld && IsEmergInPtReExamination) ||
                 (IsHICard_FiveYearsCont && IsChildUnder6YearsOld && !IsEmergInPtReExamination) ||
                 (IsHICard_FiveYearsCont && !IsChildUnder6YearsOld && IsEmergInPtReExamination) ||
                 (!IsHICard_FiveYearsCont && IsChildUnder6YearsOld && IsEmergInPtReExamination))
            {
                MessageBox.Show(eHCMSResources.A0213_G1_Msg_InfoBNHuong1in3QLBHYT);
                yield break;
            }

            if (IsHICard_FiveYearsCont || IsHICard_FiveYearsCont_NoPaid)
            {
                CalcHiBenefitAndRegister_ErrorMsg = eHCMSResources.Z0221_G1_DKHuongBHYT5Nam;
                warnDlgTask = new MessageWarningShowDialogTask(CalcHiBenefitAndRegister_ErrorMsg, eHCMSResources.Z0222_G1_TiepTucLuuDK);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    IsHICard_FiveYearsCont = false;
                }
            }
            else if (IsChildUnder6YearsOld)
            {
                string ErrorMsg = "";
                string warningMsg = "";
                string strDefProvinceID = "79";

                if (!Globals.CheckFor_CityChild_Under6YearsOld(out ErrorMsg, CurrentPatient.YOB.GetValueOrDefault(), ConfirmedHiItem, strDefProvinceID))
                {
                    warningMsg = string.Format("{0}: ", eHCMSResources.Z0215_G1_BNChuaThoaManYC) + ErrorMsg;
                }
                else
                {
                    warningMsg = eHCMSResources.Z0216_G1_XNhanBNTreEmDuoi6Tuoi;
                }

                warnDlgTask = new MessageWarningShowDialogTask(warningMsg, eHCMSResources.Z0222_G1_TiepTucLuuDK);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    IsChildUnder6YearsOld = false;
                }
            }
            else if (IsEmergInPtReExamination)
            {
                string warningMsg = "";
                string strInvalid = "";
                warningMsg = eHCMSResources.Z0223_G1_XNhanLuuDKCapCuuTK;
                if (CurrentPatient == null || CurrentPatient.LatestRegistration_InPt == null)
                {
                    MessageBox.Show(eHCMSResources.A0746_G1_Msg_InfoKhTheXNhanBNCapCuuTK3);
                    IsEmergInPtReExamination = false;
                    CanCreateNewRegistration = true;
                    yield break;
                }

                if ((CurrentPatient.LatestRegistration_InPt.HisID.HasValue == false || CurrentPatient.LatestRegistration_InPt.HisID.Value <= 0)
                        || (CurrentPatient.LatestRegistration_InPt.PtInsuranceBenefit.HasValue == false || CurrentPatient.LatestRegistration_InPt.PtInsuranceBenefit <= 0))
                {
                    MessageBox.Show(eHCMSResources.A0498_G1_Msg_InfoKhTheXNhanBNCapCuuTK2);
                    IsEmergInPtReExamination = false;
                    CanCreateNewRegistration = true;
                    yield break;
                }
                if (CurrentPatient.LatestRegistration_InPt.DischargeDate == null)
                {
                    MessageBox.Show(eHCMSResources.A0497_G1_Msg_InfoKhTheXNhanBNCapCuuTK);
                    IsEmergInPtReExamination = false;
                    CanCreateNewRegistration = true;
                    yield break;
                }
                if (CurrentPatient.LatestRegistration_InPt.EmergRecID.GetValueOrDefault() > 0 || CurrentPatient.LatestRegistration_InPt.AdmDeptID == Globals.ServerConfigSection.InRegisElements.EmerDeptID)
                {
                    if (CurrentPatient.AppointmentList != null && CurrentPatient.AppointmentList.Count > 0 && !(CurrentPatient.AppointmentList[0].IsEmergInPtReExamApp))
                    {
                        strInvalid = eHCMSResources.Z0225_G1_DKTaoTuCuocHenKgPhaiCCTK;
                    }
                    else
                    {
                        strInvalid = eHCMSResources.Z0224_G1_XNhanTaoDKBNCapCuuTK;
                    }
                }
                else
                {
                    strInvalid = string.Format("{0}: \n- {1} \n- {2}", eHCMSResources.Z0515_G1_DKNoiTruSauCungCuaBN, eHCMSResources.Z0226_G1_NVVaoKhoaCapCuu, eHCMSResources.Z0227_G1_KgDuocXNhanBNCC);
                }

                warnDlgTask = new MessageWarningShowDialogTask(strInvalid, warningMsg);
                yield return warnDlgTask;
                if (!warnDlgTask.IsAccept)
                {
                    IsEmergInPtReExamination = false;
                }
            }


            // =========================================================================================================================

            hiItem.isDoing = false;

            var calcHiTask = new CalcHiBenefitTask(hiItem, referal, (long)AllLookupValues.RegistrationType.NGOAI_TRU, false, IsEmergInPtReExamination, IsHICard_FiveYearsCont, IsChildUnder6YearsOld, IsAllowCrossRegion, IsHICard_FiveYearsCont_NoPaid);
            yield return calcHiTask;

            if (calcHiTask.Error != null)
            {
                yield break;
            }

            hiItem.HIPatientBenefit = calcHiTask.HiBenefit;
            ConfirmHIItem(hiItem);
            ConfirmPaperReferal(referal);

            if (PatientSummaryInfoContent != null)
            {
                PatientHI_SummaryInfo sumHI_Info = new PatientHI_SummaryInfo { HiBenefit = ConfirmedHiItem.HIPatientBenefit, ConfirmedHiItem = ConfirmedHiItem, ConfirmedPaperReferal = ConfirmedPaperReferal };
                PatientSummaryInfoContent.SetPatientHISumInfo(sumHI_Info);
            }

            PatientSummaryInfoContent.ThongTuyen = IsAllowCrossRegion;

            CurRegistration.PaperReferal = ConfirmedPaperReferal;
            CurRegistration.HealthInsurance = ConfirmedHiItem;
            CurRegistration.IsForeigner = false;
            CurRegistration.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            CurRegistration.IsChildUnder6YearsOld = IsChildUnder6YearsOld;
            CurRegistration.PaperReferal = ConfirmedPaperReferal;
            CurRegistration.HealthInsurance = ConfirmedHiItem;
            CurRegistration.IsForeigner = false;
            CurRegistration.IsHICard_FiveYearsCont = IsHICard_FiveYearsCont;
            CurRegistration.EmergInPtReExamination = IsEmergInPtReExamination;
            CurRegistration.IsAllowCrossRegion = IsAllowCrossRegion;
            CurRegistration.IsHICard_FiveYearsCont_NoPaid = IsHICard_FiveYearsCont_NoPaid;
            CurRegistration.FiveYearsAppliedDate = FiveYearsAppliedDate;
            CurRegistration.FiveYearsARowDate = FiveYearsARowDate;
            CurRegistration.V_ReceiveMethod = V_ReceiveMethod;

            CurRegistration.RegTypeID = (byte)PatientRegistrationType.DK_KHAM_BENH_NGOAI_TRU;

            CurRegistration.PatientClassification = new PatientClassification { PatientClassID = 2 };
            CurRegistration.HIApprovedStaffID = Globals.LoggedUserAccount.StaffID;
            CurRegistration.HisID = Globals.DefaultHisID_ForNewRegis_BeforeSaving;
            CurRegistration.PtInsuranceBenefit = calcHiTask.HiBenefit;
            //▼===== #008
            GetPriceForAppointment(CurRegistration);
            //▲===== #008
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
                foreach (PatientRegistrationDetail d in regInfo.PatientRegistrationDetails.Where(x => x.IsCountHI))
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
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //Deployment.Current.Dispatcher.BeginInvoke(() => { IsLoadingRegistration = true; });
            CallShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);

            var loadRegTask = new LoadRegistrationInfoTask(regID, true, IsProcess);
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
                        ShowOldRegistration(CurRegistration, null);
                        setPatientSummaryInfo(CurRegistration);//set lai thong tin cho benh nhan
                    }
                }
                else if (_curRegistration != loadRegTask.Registration)
                {
                    CurRegistration = loadRegTask.Registration;
                    ShowOldRegistration(CurRegistration, null);
                    setPatientSummaryInfo(CurRegistration);//set thong tin cho benh nhan
                    Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = CurRegistration, ID = regID });
                }
            }
            //Deployment.Current.Dispatcher.BeginInvoke(() => { IsLoadingRegistration = false; });
            if (pCLExamTypes.Count > 0 && IsFromPCLExamAccording)
            {
                HandlePCLExamAccordingICD_Event(pCLExamTypes);
            }
            CallHideBusyIndicator();
        }

        public void RefreshRegistration(PatientRegistration newRegInfo)
        {
            CallShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);

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
                    ShowOldRegistration(CurRegistration, null);
                    setPatientSummaryInfo(CurRegistration);//set lai thong tin cho benh nhan
                }
            }
            else if (_curRegistration != newRegInfo)
            {
                CurRegistration = newRegInfo;
                ShowOldRegistration(CurRegistration, null);
                setPatientSummaryInfo(CurRegistration);//set thong tin cho benh nhan
                //Globals.EventAggregator.Publish(new ItemLoaded<PatientRegistration, long> { Item = CurRegistration, ID = CurRegistration.PtRegistrationID });
            }
            CallHideBusyIndicator();
        }

        public void setPatientSummaryInfo(PatientRegistration curRegistration)
        {
            PatientSummaryInfoContent.CurrentPatient = curRegistration.Patient;
            PatientSummaryInfoContent.HiComment = curRegistration.HIComment;
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
                //▼====== #006: By pass yield break vẫn để load thông tin bệnh nhân < 6 tuổi. Nếu không load đc thông tin sẽ => tên bệnh nhân A, đăng ký bệnh nhân B.
                //              => Sẽ kiểm tra 1 lần nữa khi lưu nếu không bổ sung đầy đủ thông tin thì sẽ không cho lưu.
                DateTime loadCurrentDate = Globals.ServerDate.Value;
                int monthnew = 0;
                monthnew = (loadCurrentDate.Month + loadCurrentDate.Year * 12) - (Convert.ToDateTime(CurrentPatient.DOB).Month + Convert.ToDateTime(CurrentPatient.DOB).Year * 12);
                if (CurrentPatient.AgeOnly == true && ((loadCurrentDate.Year - Convert.ToDateTime(CurrentPatient.DOB).Year) <= 6))
                {
                    MessageBox.Show(eHCMSResources.Z2643_G1_KhongDuThongTinTreDuoi6Tuoi);
                    CanAddService = false;
                    //yield break;
                }
                else if (CurrentPatient.AgeOnly == false && monthnew <= 72)
                {
                    if (CurrentPatient.FContactFullName == null && (CurrentPatient.V_FamilyRelationship == null || CurrentPatient.V_FamilyRelationship == 0))
                    {
                        MessageBox.Show(eHCMSResources.Z2644_G1_KhongDuThongTinNguoiThanTreDuoi6Tuoi);
                        CanAddService = false;
                        //yield break;
                    }
                }
                //▲====== #006
            }

            ConfirmedHiItem = null;
            ConfirmedPaperReferal = null;


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

                //if (_currentPatient.LatestRegistration == null || _currentPatient.LatestRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU
                //    || _currentPatient.LatestRegistration.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.COMPLETED
                //    )//Chưa có đăng ký lần nào
                //{
                //    TitleStatus = RegStatus.ChuaDangKy;
                //    DoCreateNewRegistration();
                //}

                DateTime regDate = new DateTime();
                DateTime now = Globals.ServerDate.Value.Date;
                if (_currentPatient.LatestRegistration != null)
                {
                    regDate = _currentPatient.LatestRegistration.ExamDate.Date;
                }

                //Nếu có đăng ký cuối cùng và còn hiệu lực.
                //if (_currentPatient.LatestRegistration != null && _currentPatient.LatestRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                //    && _currentPatient.LatestRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED
                //    && (regDate <= now && regDate.AddDays(ConfigValues.PatientRegistrationTimeout) >= now))
                //{
                //    //Nếu đăng ký thuộc 1 trong những Status PENDING, OPENED, REFUND
                //    if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING
                //      || _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
                //      || _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND
                //      //|| _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
                //      )
                //    {
                //        if (_currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
                //        {
                //            _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0121_G1_DKNayDaBiHuy), eHCMSResources.G0442_G1_TBao);
                //            yield return _msgTask;
                //        }
                //        //Load dang ky len.
                //        IEnumerator e = DoOpenRegistration(_currentPatient.LatestRegistration.PtRegistrationID);

                //        while (e.MoveNext())
                //            yield return e.Current as IResult;
                //    }
                //    else
                //    {
                //        TitleStatus = RegStatus.ChuaDangKy;
                //        DoCreateNewRegistration();
                //    }
                //}
                if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU
                  && Registration_DataStorage.CurrentPatientRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED
                  && (regDate <= now && regDate.AddDays(ConfigValues.PatientRegistrationTimeout) >= now))
                {
                    //Nếu đăng ký thuộc 1 trong những Status PENDING, OPENED, REFUND
                    if (Registration_DataStorage.CurrentPatientRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING
                      || Registration_DataStorage.CurrentPatientRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
                      || Registration_DataStorage.CurrentPatientRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND
                      //|| _currentPatient.LatestRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
                      )
                    {
                        if (Registration_DataStorage.CurrentPatientRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
                        {
                            _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0121_G1_DKNayDaBiHuy), eHCMSResources.G0442_G1_TBao);
                            yield return _msgTask;
                        }
                        //Load dang ky len.
                        IEnumerator e = DoOpenRegistration(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);

                        while (e.MoveNext())
                            yield return e.Current as IResult;
                    }
                    else
                    {
                        TitleStatus = RegStatus.ChuaDangKy;
                        DoCreateNewRegistration();
                    }
                }
                else
                {
                    //Nếu không có cuộc hẹn nào hết
                    if (_currentPatient.AppointmentList == null || _currentPatient.AppointmentList.Count <= 0)
                    {
                        //Create New Registration
                        TitleStatus = RegStatus.ChuaDangKy;
                        DoCreateNewRegistration();
                    }

                    //Nếu có cuộc hẹn đã xác nhận
                    else
                    {
                        DoCreateNewRegistration();

                        //Nếu có 1 cuộc hẹn đã xác nhận và đúng hẹn.
                        if (_currentPatient.AppointmentList.Count == 1 && _currentPatient.AppointmentList[0].ApptDate.Value.Date == now)
                        {
                            //Open cuộc hẹn
                            PatientAppointment Appointment = new PatientAppointment();
                            Appointment = _currentPatient.AppointmentList[0].DeepCopy();
                            //KMx: Phải có Appointment.Patient.PatientID thì mới load chi tiết cuộc hẹn lên được (14/04/2014 15:27).
                            Appointment.Patient = new Patient();
                            Appointment.Patient.PatientID = _currentPatient.PatientID;
                            CheckAppointment(Appointment);
                        }
                        //Nếu có 1 cuộc hẹn không đúng hẹn hoặc nhiều hơn 1 cuộc hẹn đã xác nhận
                        else
                        {
                            //Open pop-up cho user chọn cuộc hẹn
                            Action<IFindAppointment> onInitDlg = delegate (IFindAppointment vm)
                            {
                                if (!string.IsNullOrEmpty(_currentPatient.PatientCode))
                                {
                                    vm.SearchCriteria.PatientCode = _currentPatient.PatientCode;
                                    vm.SearchCriteria.V_ApptStatus = (long)AllLookupValues.ApptStatus.BOOKED;
                                    vm.SearchCriteria.IsSearchPatient = true;
                                    vm.SearchCmd();
                                }
                            };
                            GlobalsNAV.ShowDialog<IFindAppointment>(onInitDlg);
                        }
                    }
                }
            }
            System.Windows.Application.Current.Dispatcher.Invoke(() => { PatientLoading = false; });
        }

        public void DoCreateNewRegistration(bool forNewPatient = true, bool confirm = false)
        {
            //Lấy ngày hiện tại trên server làm ngày đăng ký.
            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCreating = true; });

            DateTime loadCurrentDate = Globals.ServerDate.Value;
            if (loadCurrentDate == null || loadCurrentDate == DateTime.MinValue)
            {
                //Thong bao khong lay ngay thang tren server duoc roi ve luon. Khong lam tiep
                MessageBox.Show(eHCMSResources.A0696_G1_Msg_InfoKhTheLayNgSVr, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCreating = false; });
                return;
            }
            //▼===== #012:  TreatmentRegimenCollection chỉ được set giá trị khi load bệnh nhân đã có đăng ký.
            //              => Nếu load bệnh nhân A đã có chẩn đoán ở màn hình đăng ký dịch vụ rồi load bệnh nhân B lên thao tác. Sẽ dẫn việc TreatmentRegimenCollection của bệnh nhân A
            //              Sử dụng tiếp cho bệnh nhân B
            //              => Sửa lại nếu load bệnh nhân mới chưa có đăng ký gì thì phải clear TreatmentRegimenCollection đi. Còn nếu đã có đăng ký thì đã được clear ở hàm GetRefTreatmentRegimensAndDetail
            if (TreatmentRegimenCollection == null)
            {
                TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
            }
            else
            {
                TreatmentRegimenCollection.Clear();
            }
            //▲===== #012
            if (CurRegistration != null && confirm)
            {
                //Nếu có đăng ký trong ngày, hoặc còn trong khoảng thời gian có hiệu lực
                DateTime regDate = CurRegistration.ExamDate.Date;
                DateTime now = loadCurrentDate.Date;

                if (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED
                    || CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING
                    //|| CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PROCESSING
                    )
                {
                    if (regDate <= now && regDate.AddDays(ConfigValues.PatientRegistrationTimeout) >= now)
                    {
                        if (MessageBox.Show(eHCMSResources.A0495_G1_Msg_ConfTaoDKMoi, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCreating = false; });
                            return;
                        }
                    }
                }
            }

            _currentView.ResetView();
            TitleStatus = RegStatus.None;
            CurRegistration = new PatientRegistration
            {
                //KMx: Không chỉ lấy ngày mà phải lấy giờ luôn (28/05/2014 18:06)
                //ExamDate = loadCurrentDate.Date,
                ExamDate = loadCurrentDate,
                V_RegistrationType = AllLookupValues.RegistrationType.NGOAI_TRU

            };

            if (forNewPatient)
            {
                if (ConfirmedHiItem == null || CurrentPatient == null || CurrentPatient.PatientID != ConfirmedHiItem.PatientID)
                {
                    ConfirmedHiItem = null;
                }

                if (ConfirmedPaperReferal == null || ConfirmedHiItem == null || ConfirmedHiItem.HIID != ConfirmedPaperReferal.HiId)
                {
                    ConfirmedPaperReferal = null;
                }

                CurRegistration.HealthInsurance = ConfirmedHiItem;
                CurRegistration.PaperReferal = ConfirmedPaperReferal;
            }
            else
            {
                ConfirmedHiItem = null;
                ConfirmedPaperReferal = null;
            }
            if (PatientSummaryInfoContent != null)
            {
                //PatientSummaryInfoContent.ConfirmedHiItem = _confirmedHiItem;
                //PatientSummaryInfoContent.ConfirmedPaperReferal = _confirmedPaperReferal;
                //PatientSummaryInfoContent.HiBenefit = null;
            }
            InitRegistration();
            //ResetToDefaultValues();
            if (CurrentPatient == null)
            {

                CurrentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
                CanAddService = false;
            }
            else
            {
                ////---------DPT trẻ em <72 tháng tuổi
                //int monthnew = 0;
                //monthnew = (loadCurrentDate.Month + loadCurrentDate.Year * 12) - (Convert.ToDateTime(CurrentPatient.DOB).Month + Convert.ToDateTime(CurrentPatient.DOB).Year * 12);
                //if (CurrentPatient.AgeOnly == true && ((loadCurrentDate.Year - Convert.ToDateTime(CurrentPatient.DOB).Year) <= 6))
                //{
                //    MessageBox.Show("Trẻ em dưới 6 tuổi phải nhập đầy đủ ngày tháng năm sinh");
                //    CanAddService = false;
                //}
                //else if (CurrentPatient.AgeOnly == false && monthnew <= 72)  
                //{
                //    if (CurrentPatient.FContactFullName == null && CurrentPatient.V_FamilyRelationship == null)
                //    {
                //        MessageBox.Show("Trẻ em dưới 6 tuổi phải nhập đầy đủ thông tin người liên hệ");
                //        CanAddService = false;
                //    }
                //    else
                //    {
                //        CurrentRegMode = RegistrationFormMode.NEW_REGISTRATION_OPENED;
                //        CanAddService = true;
                //    }

                //}
                //else
                //{
                CurrentRegMode = RegistrationFormMode.NEW_REGISTRATION_OPENED;
                CanAddService = true;
                //}

            }
            CanChangePatientType = true;
            //BeginEdit();

            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCreating = false; });
        }

        public IEnumerator<IResult> DoCancelRegistration()
        {
            if (CurRegistration == null)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}?", eHCMSResources.Z0609_G1_ChuaCoDK), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }
            if (Globals.IsLockRegistration(CurRegistration.RegLockFlag, eHCMSResources.T1743_G1_HuyDK.ToLower()))
            {
                yield break;
            }
            if (RegistrationInfoHasChanged)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0196_G1_TTinThayDoiPhaiLuuTruoc), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            if (CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.OPENED &&
                CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0197_G1_KgTheHuyDKNay), eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }


            if (CurRegistration.DrugInvoices != null && CurRegistration.DrugInvoices.Count > 0)
            {
                throw new Exception(eHCMSResources.Z0198_G1_DaLayThuocKgTheHuyDK);
            }

            if (CurRegistration.PatientRegistrationDetails != null)
            {
                foreach (var regDetail in CurRegistration.PatientRegistrationDetails)
                {
                    if (regDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                    {
                        _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0199_G1_DaKBKgTheHuyDK), eHCMSResources.G0442_G1_TBao);
                        yield return _msgTask;
                        yield break;
                    }
                }
            }
            if (CurRegistration.PCLRequests != null)
            {
                foreach (var request in CurRegistration.PCLRequests)
                {
                    if (request.PatientPCLRequestIndicators != null)
                    {
                        request.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        foreach (var requestDetails in request.PatientPCLRequestIndicators)
                        {
                            if (requestDetails.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
                            {
                                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0200_G1_DaLamCLSKgTheHuyDK), eHCMSResources.G0442_G1_TBao);
                                yield return _msgTask;
                                yield break;
                            }
                        }
                    }
                }
            }
            //Kiểm tra các dịch vụ có được hủy và trả tiền lại hết chưa

            if (CurRegistration.PatientTransaction != null)
            {
                //▼===== #009
                //if (CurRegistration.PatientTransaction.PatientTransactionPayments != null
                //    && CurRegistration.PatientTransaction.PatientTransactionPayments.Count > 0)
                //    foreach (var item in CurRegistration.PatientTransaction.PatientTransactionPayments)
                //    {
                //        if (item.IsDeleted == null
                //            || item.IsDeleted == false)
                //        {
                //            var dialog = new MessageWarningShowDialogTask(string.Format("{0} \n{1}", eHCMSResources.Z0201_G1_DKConDVChuaHuyHoacChuaHoanTien, eHCMSResources.Z0202_G1_HuyVaHoanTienDVTruocKhiHuyDK)
                //                , eHCMSResources.G0442_G1_TBao, false);
                //            yield return dialog;
                //            yield break;
                //        }
                //    }
                if (!CheckTransactionPaymentAndOutPatientCashAdvance(CurRegistration.PatientTransaction))
                {
                    var dialog = new MessageWarningShowDialogTask(string.Format("{0} \n{1}", eHCMSResources.Z0201_G1_DKConDVChuaHuyHoacChuaHoanTien, eHCMSResources.Z0202_G1_HuyVaHoanTienDVTruocKhiHuyDK)
                        , eHCMSResources.G0442_G1_TBao, false);
                    yield return dialog;
                    yield break;
                }
                //▲===== #009
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCancelling = true; });
            CurRegistration.RegCancelStaffID = Globals.LoggedUserAccount.StaffID.Value;
            var cancelRegTask = new CancelRegistrationTask(CurRegistration);
            yield return cancelRegTask;

            if (cancelRegTask.Error == null && cancelRegTask.RegistrationInfo != null)
            {
                OpenRegistration(cancelRegTask.RegistrationInfo.PtRegistrationID);
                //System.Windows.Application.Current.Dispatcher.Invoke(() => MessageBox.Show(eHCMSResources.A0613_G1_Msg_InfoHuyOK));
                System.Windows.Application.Current.Dispatcher.Invoke(() => GlobalsNAV.ShowMessagePopup(eHCMSResources.A0613_G1_Msg_InfoHuyOK));
            }

            System.Windows.Application.Current.Dispatcher.Invoke(() => { RegistrationCancelling = false; });
        }
        //▼====== #009
        private bool CheckTransactionPaymentAndOutPatientCashAdvance(PatientTransaction tempPatientTransation)
        {
            decimal varTongTien = 0;
            if (tempPatientTransation.PatientTransactionPayments != null
                    && tempPatientTransation.PatientTransactionPayments.Count > 0)
            {
                foreach (var item in tempPatientTransation.PatientTransactionPayments)
                {
                    if (item.IsDeleted == null
                        || item.IsDeleted == false)
                    {
                        return false;
                    }
                }
            }
            if (tempPatientTransation.PatientCashAdvances != null
                    && tempPatientTransation.PatientCashAdvances.Count > 0)
            {
                foreach (var tmpPatientCashAdvance in tempPatientTransation.PatientCashAdvances)
                {
                    if (tmpPatientCashAdvance.PaymentAmount != 0)
                    {
                        varTongTien += tmpPatientCashAdvance.PaymentAmount;
                    }
                }
                if (varTongTien != 0)
                {
                    return false;
                }
            }
            return true;
        }
        //▲===== #009
        //Load medical service len mot lan
        //public IEnumerator<IResult> GetMedServiceAll()
        //{
        //    var RefMedServiceTypesTask = new LoadRefMedicalServiceTypeTask();
        //    yield return RefMedServiceTypesTask;
        //    if (RefMedServiceTypesTask.RefMedicalServiceTypes != null && RefMedServiceTypesTask.RefMedicalServiceTypes.Count > 0)
        //    {
        //        foreach (var item in RefMedServiceTypesTask.RefMedicalServiceTypes)
        //        {
        //            var RefMedServiceItemTask = new LoadRefMedicalServiceItemTask(item);
        //            yield return RefMedServiceItemTask;
        //            item.RefMedicalServiceItems = RefMedServiceItemTask.RefMedicalServiceItems;
        //            if (item.RefMedicalServiceItems != null && item.RefMedicalServiceItems.Count > 0)
        //            {
        //                foreach (var serItem in RefMedServiceItemTask.RefMedicalServiceItems)
        //                {
        //                    var LocationsTask = new LoadGetLocationsByServiceIDTask(serItem.MedServiceID);
        //                    yield return LocationsTask;
        //                }
        //            }
        //        }
        //    }
        //}

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
                    // TxD 16/03/2017 Commented out the following line because it looked very WRONG to set DoctorStaffID here
                    // If something stops working then REVIEW and FIX afterward
                    //RegistrationDetailsContent.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;

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

        private void ShowOldRegistration(PatientRegistration regInfo, PatientAppointment aAppointment, bool fromAppointment = false)
        {
            mCuocHen_KSK = false;
            if (regInfo != null)
            {
                if (aAppointment != null)
                {
                    regInfo.Appointment = aAppointment;
                }
                else
                {
                    aAppointment = regInfo.Appointment;
                }
                if (aAppointment != null && aAppointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE)
                {
                    mCuocHen_KSK = true;
                }
            }
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
                else
                {
                    //▼===== #010:
                    TitleStatus = RegStatus.DangKyChuaBH;
                    PatientSummaryInfoContent.SetPatientHISumInfo(null);
                    PatientSummaryInfoContent.ThongTuyen = CurRegistration.IsAllowCrossRegion;
                    //▲===== #010
                }

            }
            if (aAppointment != null && aAppointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE)
            {
                CurRegistration.PatientClassification = null;
                CurRegistration.PatientClassID = (long)ePatientClassification.CompanyHealthRecord;
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

            //if (PatientSummaryInfoContent != null)
            //{
            //    PatientSummaryInfoContent.IsCrossRegion = CurRegistration.IsCrossRegion.GetValueOrDefault(true);
            //}

            if (CurRegistration.RegistrationStatus != AllLookupValues.RegistrationStatus.PENDING)
            {
                CanAddService = false;
            }
            //▼===== #012: Nếu là màn hình đăng ký thì không cần phải load danh sách phác đồ của bệnh nhân
            //             Lưu ý: Nếu người sử dụng yêu cầu thì sẽ viết lại code để kiểm tra.
            if (!RegistrationView && Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen)
            {
                GetRefTreatmentRegimensAndDetail(CurRegistration.PtRegistrationID);
            }
            //▲===== 
        }

        public void CreateNewRegistrationForThisPatientCmd()
        {
            DoCreateNewRegistration(false, true);
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
            CallShowBusyIndicator(eHCMSResources.Z0086_G1_DangLayTTinDK);
            Coroutine.BeginExecute(DoOpenRegistration(regID), null, (o, e) =>
            {
                //IsLoadingRegistration = false; 
                CallHideBusyIndicator();
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

        public void OldRegistrationsCmd()
        {
            Action<IRegistrationList> onInitDlg = delegate (IRegistrationList vm)
            {
                vm.CurrentPatient = CurrentPatient;
            };
            GlobalsNAV.ShowDialog<IRegistrationList>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void AddPclExamTypeCmd_LAB()
        {
            AddPclExamTypeCmdLAB();
        }

        public void AddAllPclExamTypeCmd_LAB()
        {
            AddAllPclExamTypeCmdLAB();
        }

        #region COMMANDS
        public void CheckAppointmentCmd()
        {
            GlobalsNAV.ShowDialog<IAppointmentSearch>();
        }
        public void PayCmd()
        {
            if (_curRegistration == null)
            {
                MessageBox.Show(eHCMSResources.Z0609_G1_ChuaCoDK);
                return;
            }
            if (_curRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0610_G1_DKChuaLuu);
                return;
            }
            if (IsCalculatingPayment)
            {
                return;
            }
            Action<IPay> onInitDlg = delegate (IPay vm)
            {
                vm.LoadRegistration(_curRegistration.PtRegistrationID);
                vm.FormMode = PaymentFormMode.REGISTER_AND_PAY;
            };
            GlobalsNAV.ShowDialog<IPay>(onInitDlg);
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
        public void CancelRegistrationCmd()
        {
            Coroutine.BeginExecute(DoCancelRegistration());
        }

        public bool CanCancelRegistrationAndPayCmd
        {
            get
            {
                return CurRegistration != null
                    && (CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.OPENED || CurRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.PENDING)
                    && RegistrationInfoHasChanged == false;
            }
        }
        public void CancelRegistrationAndPayCmd()
        {

        }
        public void AddPclExamTypeCmd()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItem(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent { ValidationResults = validationResults });
                return;
            }
            if (gSelectedDoctorStaff == null || gMedicalInstructionDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS);
                return;
            }
            DeptLocation deptLocation = null;
            if (SelectPCLContent.SelectedPclExamTypeLocation != null)
            {
                deptLocation = SelectPCLContent.SelectedPclExamTypeLocation.DeptLocation;
            }
            RegistrationDetailsContent.AddNewPclRequestDetailFromPclExamType(SelectPCLContent.SelectedPCLExamType, deptLocation, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
        }
        private bool ValidatePclItem(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (SelectPCLContent.SelectedPCLExamType == null || SelectPCLContent.SelectedPCLExamType.PCLExamTypeID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult(eHCMSResources.Z0164_G1_HayChonDVCLS, new[] { "SelectedPclExamType" });
                result.Add(item);
            }
            //if (SelectPCLContent.SelectedPclExamTypeLocation == null || SelectPCLContent.SelectedPclExamTypeLocation.DeptLocation == null
            //    || SelectPCLContent.SelectedPclExamTypeLocation.DeptLocation.DeptLocationID <= 0)
            //{
            //    var item = new ValidationResult("Hãy chọn phòng", new[] { "DeptLocation" });
            //    result.Add(item);
            //}
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }

        #endregion

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
                return _mDangKyDV_DichVu_Them || ViewCase == RegistrationViewCase.RegistrationRequestView;
            }
            set
            {
                if (_mDangKyDV_DichVu_Them == value)
                    return;
                _mDangKyDV_DichVu_Them = value;
                NotifyOfPropertyChange(() => mDangKyDV_DichVu_Them);
                NotifyOfPropertyChange(() => IsAddRegPackVisible);
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

        #endregion

        public void Handle(ConfirmHiBenefit message)
        {
            if (CurRegistration == null || ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
            {
                return;
            }

            CurRegistration.IsCrossRegion = message.IsCrossRegion;

            //PatientSummaryInfoContent.HiBenefit = message.HiBenefit;
            //PatientSummaryInfoContent.IsCrossRegion = message.IsCrossRegion.GetValueOrDefault(true);

            CurRegistration.HIComment = message.HIComment;
        }

        public void Handle(RemoveConfirmedHiCard message)
        {
            if (GetView() == null || message.HiId <= 0)
            {
                return;
            }

            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
            {
                return;
            }
            if (ConfirmedHiItem != null && ConfirmedHiItem.HIID == message.HiId)
            {
                ConfirmedHiItem = null;
                ConfirmedPaperReferal = null;
                PatientSummaryInfoContent.SetPatientHISumInfo(null);

                if (CurRegistration != null)
                {
                    CurRegistration.HealthInsurance = null;
                    CurRegistration.PaperReferal = null;
                }
            }
        }

        public void Handle(DoubleClick message)
        {
            if (message.Source != SelectPCLContent)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContent.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmd();
        }

        public void Handle(DoubleClickAddReqLAB message)
        {
            if (message.Source != SelectPCLContentLAB)
            {
                return;
            }
            if (message.EventArgs.Value != SelectPCLContentLAB.SelectedPCLExamType)
            {
                return;
            }
            AddPclExamTypeCmdLAB();
        }

        //Dinh them tai day
        public void ConfirmHiBenefit(ConfirmHiBenefitEvent message)
        {
            if (CurRegistration == null || ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
            {
                return;
            }
            ConfirmedHiItem.isDoing = true;
            ConfirmedHiItem.EditLocked = false;
            CurRegistration.IsCrossRegion = message.IsCrossRegion;
            ConfirmedHiItem.HIPatientBenefit = Double.Parse(message.HiBenefit.GetValueOrDefault(0).ToString());


            CurRegistration.HIComment = message.HIComment;
        }

        public void RemoveConfirmedHiCard(ConfirmHiBenefitEvent message)
        {
            if (GetView() == null || message.HiId <= 0)
            {
                return;
            }

            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0)
            {
                return;
            }
            if (ConfirmedHiItem != null && ConfirmedHiItem.HIID == message.HiId)
            {
                ConfirmedHiItem = null;
                ConfirmedPaperReferal = null;
                PatientSummaryInfoContent.SetPatientHISumInfo(null);

                if (CurRegistration != null)
                {
                    CurRegistration.HealthInsurance = null;
                    CurRegistration.PaperReferal = null;
                }
            }
        }

        public void NoChangeConfirmHiBenefit(ConfirmHiBenefitEvent message)
        {
            if (message != null)
            {
                CurRegistration.HIComment = message.HIComment;
                if (ConfirmedHiItem == null || ConfirmedHiItem.HIID != message.HiId)
                {
                    return;
                }
                ConfirmedHiItem.isDoing = true;
                ConfirmedHiItem.EditLocked = true;
            }
        }

        public void Handle(ConfirmHiBenefitEvent message)
        {
            if (message != null)
            {
                PatientSummaryInfoContent.HiComment = message.HIComment;
                switch (message.confirmHiBenefitEnum)
                {
                    case ConfirmHiBenefitEnum.ConfirmHiBenefit:
                        ConfirmHiBenefit(message);
                        break;
                    case ConfirmHiBenefitEnum.NoChangeConfirmHiBenefit:
                        NoChangeConfirmHiBenefit(message);
                        break;
                    case ConfirmHiBenefitEnum.RemoveConfirmedHiCard:
                        RemoveConfirmedHiCard(message);
                        break;
                }
            }
        }

        #region Add LAB
        public void AddPclExamTypeCmdLAB()
        {
            ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults;
            if (!ValidatePclItemLAB(out validationResults))
            {
                Globals.EventAggregator.Publish(new ValidateFailedEvent { ValidationResults = validationResults });
                return;
            }
            if (gSelectedDoctorStaff == null || gMedicalInstructionDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS);
                return;
            }
            DeptLocation deptLocation = null;
            if (SelectPCLContentLAB.SelectedPclExamTypeLocation != null)
            {
                deptLocation = SelectPCLContentLAB.SelectedPclExamTypeLocation.DeptLocation;
            }
            RegistrationDetailsContent.AddNewPclRequestDetailFromPclExamType(SelectPCLContentLAB.SelectedPCLExamType, deptLocation, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
        }

        public void AddAllPclExamTypeCmdLAB()
        {
            if (SelectPCLContentLAB.PclExamTypes == null || SelectPCLContentLAB.PclExamTypes.Count < 1)
            {
                return;
            }
            if (gSelectedDoctorStaff == null || gMedicalInstructionDate == null)
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS);
                return;
            }
            RegistrationDetailsContent.AddNewAllPclRequestDetailFromPclExamType(SelectPCLContentLAB.PclExamTypes, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
        }
        private bool ValidatePclItemLAB(out ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult> result)
        {
            result = new ObservableCollection<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (SelectPCLContentLAB.SelectedPCLExamType == null || SelectPCLContentLAB.SelectedPCLExamType.PCLExamTypeID < 1)
            {
                var item = new System.ComponentModel.DataAnnotations.ValidationResult("Hãy chọn dịch vụ Xét Nghiệm", new[] { "SelectedPclExamType" });
                result.Add(item);
            }
            if (result.Count > 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        public bool CanReportRegistrationInfoInsuranceCmd
        {
            get { return CurRegistration != null && CurRegistration.PtRegistrationID > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0; }
        }

        public void ReportRegistrationInfoInsuranceCmd()
        {
            if (CurRegistration != null && CurRegistration.PtRegistrationID > 0 && CurRegistration.HisID.GetValueOrDefault(0) > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
                {
                    reportVm.RegistrationID = CurRegistration.PtRegistrationID;
                    reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_HI_CONFIRMATION;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        public string ViewTitle
        {
            get => _ViewTitle; set
            {
                _ViewTitle = value;
                NotifyOfPropertyChange(() => ViewTitle);
            }
        }

        private RegistrationViewCase ViewCase
        {
            get
            {
                return RegistrationDetailsContent == null ? RegistrationViewCase.RegistrationView : RegistrationDetailsContent.ViewCase;
            }
        }

        private string _ViewTitle = eHCMSResources.K2863_G1_DKDV;
        public RefMedicalServiceGroups RefMedicalServiceGroupObj
        {
            get
            {
                return RegistrationDetailsContent == null ? null : RegistrationDetailsContent.RefMedicalServiceGroupObj;
            }
        }

        public bool IsRegistrationView
        {
            get
            {
                return ViewCase == RegistrationViewCase.RegistrationView;
            }
        }

        public bool IsMedicalServiceGroupView
        {
            get
            {
                return ViewCase == RegistrationViewCase.MedicalServiceGroupView;
            }
        }

        public void ApplyViewCase(RegistrationViewCase aViewCase, DiagnosisTreatment CurrentDiagnosisTreatment)
        {
            if (RegistrationDetailsContent != null)
            {
                RegistrationDetailsContent.ViewCase = aViewCase;
                RegistrationDetailsContent.CurrentDiagnosisTreatment = CurrentDiagnosisTreatment;
            }
            if (PatientSummaryInfoContent != null)
            {
                PatientSummaryInfoContent.ViewCase = aViewCase;
            }
            NotifyOfPropertyChange(() => ViewCase);
            NotifyOfPropertyChange(() => IsRegistrationView);
            NotifyOfPropertyChange(() => IsAddRegPackVisible);
            NotifyOfPropertyChange(() => IsMedicalServiceGroupView);
            NotifyOfPropertyChange(() => mDangKyDV_DichVu_Them);
            if (ViewCase == RegistrationViewCase.MedicalServiceGroupView)
            {
                CurrentPatient = new Patient();
                if (RegistrationDetailsContent == null)
                {
                    return;
                }
                RegistrationDetailsContent.CurrentRegistration = new PatientRegistration { Patient = new Patient() };
            }
            else if (ViewCase == RegistrationViewCase.RegistrationRequestView)
            {
                if (RegistrationDetailsContent != null && CurRegistration != null)
                {
                    OpenRegistration(CurRegistration.PtRegistrationID);
                }
            }
        }

        private string _SearchMedicalServiceGroupCode;
        public string SearchMedicalServiceGroupCode
        {
            get => _SearchMedicalServiceGroupCode; set
            {
                _SearchMedicalServiceGroupCode = value;
                NotifyOfPropertyChange(() => SearchMedicalServiceGroupCode);
            }
        }

        private List<RefMedicalServiceGroups> MedicalServiceGroupCollection
        {
            get
            {
                return RegistrationDetailsContent == null ? null : RegistrationDetailsContent.MedicalServiceGroupCollection;
            }
        }

        public void btnSearchMedServiceGroups()
        {
            if (MedicalServiceGroupCollection == null || MedicalServiceGroupCollection.Count() == 0)
            {
                return;
            }
            if (SearchMedicalServiceGroupCode == null)
            {
                SearchMedicalServiceGroupCode = "";
            }
            var SearchCode = Globals.RemoveVietnameseString(SearchMedicalServiceGroupCode.ToLower());
            var SearchMedicalServiceGroups = MedicalServiceGroupCollection.Where(x => (!string.IsNullOrEmpty(x.MedicalServiceGroupCode) && Globals.RemoveVietnameseString(x.MedicalServiceGroupCode.ToLower()).Contains(SearchCode)) || (!string.IsNullOrWhiteSpace(x.MedicalServiceGroupName) && Globals.RemoveVietnameseString(x.MedicalServiceGroupName.ToLower()).Contains(SearchCode))).ToList();
            if (SearchMedicalServiceGroups != null && SearchMedicalServiceGroups.Count() == 1)
            {
                RegistrationDetailsContent.ApplyRefMedicalServiceGroup(SearchMedicalServiceGroups.First(), DeptLocation != null ? DeptLocation : null);
                NotifyOfPropertyChange(() => RefMedicalServiceGroupObj);
            }
            else if (SearchMedicalServiceGroups != null && SearchMedicalServiceGroups.Count() > 1)
            {
                ISearchMedicalServiceGroups SearchView = Globals.GetViewModel<ISearchMedicalServiceGroups>();
                SearchView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                SearchView.ApplySearchContent(MedicalServiceGroupCollection, SearchMedicalServiceGroupCode, SearchMedicalServiceGroups);
                GlobalsNAV.ShowDialog_V3(SearchView, null, null, false, true, Globals.GetTwoFourthWidthDefaultDialogViewSize());
                if (SearchView.SelectedRefMedicalServiceGroup != null)
                {
                    RegistrationDetailsContent.ApplyRefMedicalServiceGroup(SearchView.SelectedRefMedicalServiceGroup, DeptLocation != null ? DeptLocation : null);
                    NotifyOfPropertyChange(() => RefMedicalServiceGroupObj);
                }
            }
        }

        public void AddRegPackCmd()
        {
            //▼====: #019
            if (Globals.ServerConfigSection.CommonItems.IsEnableAddRegPackByDoctor && gSelectedDoctorStaff == null)
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS);
                return;
            }
            //▲====: #019
            ISearchMedicalServiceGroups SearchView = Globals.GetViewModel<ISearchMedicalServiceGroups>();
            SearchView.ApplySearchContent(MedicalServiceGroupCollection, "", null);
            GlobalsNAV.ShowDialog_V3(SearchView, null, null, false, true, Globals.GetTwoFourthWidthDefaultDialogViewSize());
            if (SearchView.SelectedRefMedicalServiceGroup != null)
            {
                RegistrationDetailsContent.ApplyRefMedicalServiceGroup(SearchView.SelectedRefMedicalServiceGroup, DeptLocation != null ? DeptLocation : null, true, gSelectedDoctorStaff);
            }
        }

        public bool IsAddRegPackVisible
        {
            get
            {
                //20190106 TTM: Ẩn gói đi vì đang có bug fix xong sẽ mở ra xài lại.
                //20190626 TTM: Mở gói lên test lại nếu ok sẽ release và bỏ hết comment này ra.
                return mDangKyDV_DichVu_Them 
                    && ((IsRegistrationView && !Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA) 
                        || Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA
                        //▼==== #019
                        || Globals.ServerConfigSection.CommonItems.IsEnableAddRegPackByDoctor);
                        //▲==== #019
            }
        }

        //▼====== #004
        //Handle đc kích hoạt khi BasicDiagTreatment bên màn hình PatientSummaryV3 thay đổi dữ liệu. Sau đó đc set tự động cho PatientRegistration để đi lưu
        public void Handle(SetBasicDiagTreatmentForRegistrationSummaryV2 message)
        {
            if (!string.IsNullOrEmpty(PatientSummaryInfoContent.BasicDiagTreatment))
            {
                RegistrationDetailsContent.BasicDiagTreatment = PatientSummaryInfoContent.BasicDiagTreatment;
                if (PatientSummaryInfoContent.gSelectedDoctorStaff != null)
                {
                    RegistrationDetailsContent.DoctorStaffID = PatientSummaryInfoContent.gSelectedDoctorStaff.StaffID;
                }
            }
        }

        //Handle đc kích hoạt khi PatientRegistration lưu/ cập nhật đăng ký => PatientSummaryV3 để biết cập nhật BasicDiagTreatment cho đăng ký nào của bệnh nhân
        public void Handle(SetCurRegistrationForPatientSummaryInfoV3 message)
        {
            if (CurRegistration != null)
            {
                PatientSummaryInfoContent.curRegistration = CurRegistration;
            }
        }
        //▲======= #004
        //▼===== #008
        public void GetPriceForAppointment(PatientRegistration CurRegistration)
        {
            foreach (var item in CurRegistration.GetSaveInvoiceItem())
            {
                item.GetItemPrice(CurRegistration, Globals.GetCurServerDateTime());
                item.GetItemTotalPrice();
            }
            CommonGlobals.CorrectRegistrationDetails(CurRegistration);
            RegistrationDetailsContent.TinhTongGiaTien();
        }

        //▲===== #008
        private List<RefTreatmentRegimen> _TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
        public List<RefTreatmentRegimen> TreatmentRegimenCollection
        {
            get
            {
                return _TreatmentRegimenCollection;
            }
            set
            {
                _TreatmentRegimenCollection = value;
            }
        }

        private void GetRefTreatmentRegimensAndDetail(long aPtRegistrationID)
        {
            if (TreatmentRegimenCollection == null)
            {
                TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
            }
            else
            {
                TreatmentRegimenCollection.Clear();
            }
            if (aPtRegistrationID == 0)
            {
                return;
            }
            CallShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefTreatmentRegimensAndDetailByPtRegistrationID(aPtRegistrationID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var mTreatmentRegimenCollection = contract.EndGetRefTreatmentRegimensAndDetailByPtRegistrationID(asyncResult);
                                if (mTreatmentRegimenCollection != null)
                                {
                                    foreach (var aItem in mTreatmentRegimenCollection)
                                    {
                                        TreatmentRegimenCollection.Add(aItem);
                                    }
                                }
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }
                            finally
                            {
                                CallHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    CallHideBusyIndicator();
                }
            });

            t.Start();
        }
        public void SetEkip()
        {
            if (RegistrationDetailsContent.CurrentRegistration == null || RegistrationDetailsContent.CurrentRegistration.PatientRegistrationDetails == null)
            {
                return;
            }
            if (CurRegistration.PtInsuranceBenefit == null || CurRegistration.PtInsuranceBenefit == 0)
            {
                MessageBox.Show(eHCMSResources.Z2979_G1_ErrorEkip, eHCMSResources.G0442_G1_TBao);
                return;
            }
            ObservableCollection<PatientRegistrationDetail> ObsPatientRegistrationDetail = new ObservableCollection<PatientRegistrationDetail>();

            //20200208 TBL: BM 0022891: Thay đổi cách lấy dịch vụ để thêm Ekip
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
            aView.CurrentRegistration.V_RegistrationType = RegistrationDetailsContent.CurrentRegistration.V_RegistrationType;
            aView.CurrentRegistration.PtRegistrationID = RegistrationDetailsContent.CurrentRegistration.PtRegistrationID;
            aView.CurrentRegistration.PatientRegistrationDetails = ObsPatientRegistrationDetail;
            GlobalsNAV.ShowDialog_V3<ISetEkipForMedicalService>(aView, null, null, false, false);
            if (!aView.SaveOK) //20200509 TBL: Nếu không lưu thì xóa hết những ekip đã thiết lập
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
                PatientSummaryInfoContent.Registration_DataStorage = Registration_DataStorage;
                RegistrationDetailsContent.ServiceRecID = Registration_DataStorage != null && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count > 0 ? Registration_DataStorage.PatientServiceRecordCollection.First().ServiceRecID : 0;
            }
        }

        public void HandlePatientFromConsultation(Patient patient)
        {
            if (patient != null)
            {
                CurrentPatient = patient;
                Coroutine.BeginExecute(DoSetCurrentPatient(patient));
            }
        }

        //▼===== #012
        private bool _RegistrationView = false;
        public bool RegistrationView
        {
            get { return _RegistrationView; }
            set
            {
                _RegistrationView = value;
                if (_RegistrationView)
                {
                    RegistrationDetailsContent.RegistrationView = _RegistrationView;
                }
                NotifyOfPropertyChange(() => RegistrationView);
            }
        }
        //▲===== #012

        //▼===== #013
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
                RegistrationDetailsContent.AddNewService(ObjectCopier.DeepCopy(MedServiceItem), DeptLocation != null ? DeptLocation : null, MedServiceType);
            }
        }
        //▲===== #013

        public void ChooseDoctorStaff()
        {
            if (MedServiceItem == null || MedServiceItem.MedServiceID == 0)
            {
                return;
            }
            ICalendarDay DayView = Globals.GetViewModel<ICalendarDay>();
            DayView.CurrentLocation = DeptLocation == null ? Globals.DeptLocation : DeptLocation;
            DayView.CurrentDate = Globals.GetCurServerDateTime().Date;
            DayView.CurrentPatient = CurrentPatient;
            DayView.CurrentMedServiceID = MedServiceItem == null ? 0 : MedServiceItem.MedServiceID;
            DayView.CurrentViewCase = 1;
            DateTime? StartDate = null;
            DateTime? EndDate = null;
            DayView.OnDateChangedCallback = new OnDateChanged((s, e) =>
            {
                StartDate = s;
                EndDate = e;
            });
            GlobalsNAV.ShowDialog_V3(DayView);
            if (DayView.IsConfirmed && RegistrationDetailsContent != null && RegistrationDetailsContent.CanAddService && DayView.CurrentConsultationRoomStaffAllocID.GetValueOrDefault(0) > 0)
            {
                CallAddRegItemCmd(DayView.CurrentConsultationRoomStaffAllocID.Value, StartDate, EndDate);
            }
        }
        private void LoadDoctorStaffCollection()
        {
            //▼====: #017
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //▲====: #017
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (Globals.ServerConfigSection.CommonItems.IsApplyTimeSegments)
            {
                DoctorStaffs = new ObservableCollection<Staff>(DoctorStaffs.Where(x =>
                            x.ConsultationTimeSegmentsList != null &&
                            (x.ConsultationTimeSegmentsList.Where(y =>
                                    y.StartTime.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                    && y.EndTime.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0
                            || x.ConsultationTimeSegmentsList.Where(y =>
                                    y.EndTime2 != null
                                    && y.StartTime2.Value.TimeOfDay < Globals.ServerDate.Value.TimeOfDay
                                    && y.EndTime2.Value.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)));
            }
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        //▼===== #014
        private bool _IsRegimenChecked;
        public bool IsRegimenChecked
        {
            get
            {
                return _IsRegimenChecked;
            }
            set
            {
                _IsRegimenChecked = value;
                if (SelectPCLContentLAB != null)
                {
                    SelectPCLContentLAB.IsRegimenChecked = _IsRegimenChecked;
                }
                if (SelectPCLContent != null)
                {
                    SelectPCLContent.IsRegimenChecked = _IsRegimenChecked;
                }
                if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                {
                    GetRefTreatmentRegimensAndDetail(Registration_DataStorage.CurrentPatientRegistrationDetail);
                }
                NotifyOfPropertyChange(() => IsRegimenChecked);
            }
        }
        private List<RefTreatmentRegimen> _ListRegiment = new List<RefTreatmentRegimen>();
        public List<RefTreatmentRegimen> ListRegiment
        {
            get { return _ListRegiment; }
            set
            {
                if (_ListRegiment != value)
                {
                    _ListRegiment = value;
                    NotifyOfPropertyChange(() => ListRegiment);
                }
            }
        }
        public void GetRefTreatmentRegimensAndDetail(PatientRegistrationDetail aRegistrationDetail)
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefTreatmentRegimensAndDetail(aRegistrationDetail.PtRegDetailID, null, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var listRegiment = contract.EndGetRefTreatmentRegimensAndDetail(asyncResult);
                                ListRegiment = listRegiment.ToList();
                                if (SelectPCLContentLAB != null)
                                {
                                    SelectPCLContentLAB.ListRegiment = listRegiment.ToList();
                                }
                                if (SelectPCLContent != null)
                                {
                                    SelectPCLContent.ListRegiment = listRegiment.ToList();
                                }
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #014
        //▼===== #018
        private bool _IsFromPCLExamAccording;
        public bool IsFromPCLExamAccording
        {
            get
            {
                return _IsFromPCLExamAccording;
            }
            set
            {
                _IsFromPCLExamAccording = value;
                NotifyOfPropertyChange(() => IsFromPCLExamAccording);
            }
        }
        private ObservableCollection<PCLExamType> _pCLExamTypes = new ObservableCollection<PCLExamType>();
        public ObservableCollection<PCLExamType> pCLExamTypes
        {
            get { return _pCLExamTypes; }
            set
            {
                if (_pCLExamTypes != value)
                {
                    _pCLExamTypes = value;
                    NotifyOfPropertyChange(() => pCLExamTypes);
                }
            }
        }
        private void HandlePCLExamAccordingICD_Event(ObservableCollection<PCLExamType> pCLExamTypes)
        {
            if (pCLExamTypes != null && pCLExamTypes.Count > 0)
            {
                Staff gSelectedDoctorStaff = Globals.LoggedUserAccount.Staff;
                var pCLExamTypesLOB = new ObservableCollection<PCLExamType>();
                foreach (var item in pCLExamTypes.Where(x => (bool)x.IsActive && x.IsUsed).ToList())
                {
                    if(item.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                    {
                        pCLExamTypesLOB.Add(item);
                    }
                    else
                    {
                        RegistrationDetailsContent.AddNewPclRequestDetailFromPclExamType(item, item.PCLExamTypeLocations != null ? item.PCLExamTypeLocations.FirstOrDefault().DeptLocation : new DeptLocation(), gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
                    }
                }
                if(pCLExamTypesLOB.Count > 0)
                {
                    RegistrationDetailsContent.AddNewAllPclRequestDetailFromPclExamType(pCLExamTypesLOB, gSelectedDoctorStaff, gMedicalInstructionDate, Diagnosis);
                }
            }
        }

        //public void Handle(UpdateDiagnosisTreatmentAndPrescription_Event message)
        //{
        //    if (message != null && message.Result == true)
        //    {
        //        if(Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null 
        //            && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistrationDetail != null)
        //        {
        //            IsNew = true;
        //            GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID,
        //            Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, "", 1, true, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
        //        }

        //    }
        //}
        //▲===== #018
        //▼===== #020
        private bool _IsFromRequestDoctor;
        public bool IsFromRequestDoctor
        {
            get
            {
                return _IsFromRequestDoctor;
            }
            set
            {
                _IsFromRequestDoctor = value;
                NotifyOfPropertyChange(() => IsFromRequestDoctor);
                //▼====: #019
                RegistrationDetailsContent.IsFromRequestDoctor = IsFromRequestDoctor;
                //▲====: #019
            }
        }
        //▲===== #020

        //▼===== #021
        private string _ICD10List;
        public string ICD10List
        {
            get
            {
                return _ICD10List;
            }
            set
            {
                _ICD10List = value;
                NotifyOfPropertyChange(() => ICD10List);
            }
        }
        //▲===== #021
    }

    public enum RegStatus
    {
        None = 1,
        ChuaDangKy = 2,
        DangKyCoBH = 3,
        DangKyChuaBH = 4,
        CuocHen_DungHen = 5,
        CuocHen_TraiHen = 6
    }
}
