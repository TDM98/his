using System;
using System.Windows;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using aEMR.CommonTasks;
using System.Threading;
using aEMR.ServiceClient;
using eHCMSLanguage;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Common.Collections;
using System.Windows.Controls;
using Castle.Windsor;
using aEMR.Common.BaseModel;
using System.Text;
using aEMR.Common;
using System.Windows.Data;
using aEMR.Controls;
using System.Windows.Input;
using aEMR.Common.HotKeyManagement;
using System.ComponentModel;
using eHCMS.Services.Core.Base;
using System.Windows.Media;
using System.Text.RegularExpressions;
using Service.Core.Common;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.ViewContracts.Configuration;
using System.Xml;
/*
* 20190815 #001 TTM:    BM 0013178: Fix lỗi không lấy được phòng đang nằm của bệnh nhân.
* 20191004 #002 TBL:    BM 0017400: Kiểm tra các trường bắt buộc phải nhập khi lưu y lệnh (có cấu hình)
* 20191005 #003 TBL:    BM 0017404: Tạo mới dựa trên cũ y lệnh (popup) 
* 20191016 #004 TTM:    BM 0018459: [Dịch truyền] Sửa lỗi chạy đua khi lưu và đọc dữ liệu cho y lệnh. (Chuyển tất cả dữ liệu sang InPatientInstructionViewModel để thực hiện
*                                   lưu và đọc dữ liệu.
* 20191030 #005 TBL:    BM 0018511: Thêm trường Bác sĩ chỉ định
* 20191130 #006 TBL:    BM 0019674: Fix lỗi xem lại y lệnh cũ thì combobox của phòng và giường không hiển thị
* 20200120 #007 TBL:    BM 0022851: Khi lưu y lệnh kiểm tra phải có ít nhất 1 DV hoặc 1 Thuốc hoặc 1 Dịch truyền hoặc 1 Y cụ thì mới cho lưu mới và cập nhật
* 20200131 #008 TTM:    BM ?      : Khi tạo mới y lệnh sẽ xoá toàn bộ thông tin các dịch vụ, cls, thuốc, y cụ trước đó. 
* 20200208 #009 TTM:    BM 0023907: Fix lỗi dữ liệu dịch vụ từ y lệnh bị sai cột PriceDifference
* 20200330 #010 TTM:    BM 0029060: Gỡ kiểm tra dị ứng khi tìm thuốc trong y lệnh vì cái kiểm tra này sai (do kiểm tra chuỗi với chuỗi). Anh Tuấn yêu cầu gỡ để lên task mới mà hoàn thiện chức năng.
* 20201029 #011 TNHX:   BM: Tăng ngày y lệnh được phép nhập dựa trên cấu hình NumOfOverDaysForMedicalInstructDate
* 20201108 #012 TNHX:   BM: Thêm mẫu cung cấp máu đối với DV có V_RefMedicalServiceTypes = 56008
* 20201211 #013 TNHX:   BM: Lọc danh sách bsi theo khoa phòng cấu hình (FilterDoctorByDeptResponsibilitiesInPt) + Chỉ chặn tương tác thuốc có mức độ từ cấu hình BlockInteractionSeverityLevelInPt trở lên
* 20201214 #014 TNHX:   BM: Thêm phiếu sao thuốc vào y lệnh + Chỉnh Phiếu cung cấp máu
* 20210109 #015 TNHX:   BM: Thêm nút xem thông tin thuốc nội trú
* 20210422 #016 TNHX:   Chặn ICD không làm bệnh chính
* 20210610 #017 TNHX:  331: Dựa vào mạch, huyết áp của "y lệnh theo dõi sinh hiệu" của y lệnh gần nhất để biết có cần nhập lại DHST không
* 20210614 #018 TNHX:  329: Thêm lọc giường theo Khoa
* 20210805 #019 TNHX:  428: Thêm điều kiện kiểm tra thuốc đi kèm với DVKT sử dụng gây tê 
* 20210915 #020 TNHX:  592: thêm kiêm tra DHST
* 20210923 #021 TNHX:  571: Thêm nút "Phiếu chăm sóc"
* 20211004 #022 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
* 20220105 #023 TNHX: 887 Lọc danh sách thuốc/ y cụ theo danh mục COVID 
* + lấy lại thông tin của y lệnh cũ + kiểm tra DHST so với thời gian y lệnh theo dõi sinh hiệu trước đó
* 20220225 #024 QTD: Cho phép chọn ngày y lệnh lớn hơn ngày hiện tại + ngày cấu hình của khoa
* 20220708 #025 QTD: Loại Kho chẵn khi đánh y lệnh thuốc
* 20220725 #026 DatTB:
* + Thêm thông tin nhịp thở RespiratoryRate vào DiagnosisTreatment_InPt.
* + Xóa lựa chọn trường "Đường huyết", "ECG" khi nhấn tạo mới
* 20220801 #027 DatTB: Chặn không cho lưu y lệnh khi có đề nghị chuyển khoa.
* 20220804 #028 DatTB: Fix Xóa lựa chọn trường "Đường huyết", "ECG" khi nhấn tạo mới: Khi là “Ngưng theo dõi” mới xóa lựa chọn
* 20220910 #029 DatTB: Thêm phiếu công khai thuốc KK
* 20221026 #030 BLQ: Bỏ check Chống chỉ định trong ngày. Chỉ check chống chỉ định với y lệnh hiện tại
* 20230104 #031 BLQ: Thêm thời hạn được tính bảo hiểm theo cấu hình NumDayHIAgreeToPayAfterHIExpiresInPt
* 20230503 #032 TNHX: 3132 - Lấy DispenseVolum + Group nhóm thuốc theo giá lớn nhất + chỉnh thông tin hiển thị khi đánh thuốc
* 20230504 #032 QTD: Thêm điều kiện kiểm tra sơ kết khi tạo y lệnh
* 20230509 #033 TNHX: 3132 - Chuyển chặn số lần/ tốc độ truyền thành cảnh báo. Làm tròn số lượng
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IInPatientInstruction)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InPatientInstructionViewModel : ViewModelBase, IInPatientInstruction
        , IHandle<RegistrationSelectedForInPtInstruction_1>
        , IHandle<RegistrationSelectedForInPtInstruction_2>
        , IHandle<ReloadLoadMedicalInstructionEvent>
        , IHandle<CreateNewFromOld_ForInPatientInstruction>
        , IHandle<DiseaseProgressionInstruction_Event>
        , IHandle<LoadInstructionFromWebsite>
        , IHandle<TreatmentProcessEvent.OnChangedTreatmentProcess>
    {
        [ImportingConstructor]
        public InPatientInstructionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            this.HasInputBindingCmd = true;
            /*
            SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            SearchRegistrationContent.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            SearchRegistrationContent.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            SearchRegistrationContent.PatientFindByVisibility = false;
            SearchRegistrationContent.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            //SearchRegistrationContent.mTimBN = mDangKyNoiTru_Patient_TimBN;
            //SearchRegistrationContent.mThemBN = mDangKyNoiTru_Patient_ThemBN;
            //SearchRegistrationContent.mTimDangKy = mDangKyNoiTru_Patient_TimDangKy;
            SearchRegistrationContent.mTimBN = true;
            SearchRegistrationContent.mThemBN = true;
            SearchRegistrationContent.mTimDangKy = true;
            SearchRegistrationContent.SearchAdmittedInPtRegOnly = true;
            PatientSummaryInfoContent = Globals.GetViewModel<IPatientInfo>();
            */
            InPatientDailyDiagnosisContent = Globals.GetViewModel<IInPatientDailyDiagnosis>();
            MedicalInstructionContent = Globals.GetViewModel<IMedicalInstruction>();
            MonitoringVitalSignsContent = Globals.GetViewModel<IMonitoringVitalSigns>();
            IntravenousContent = Globals.GetViewModel<IIntravenous>();

            this.ActivateItem(MedicalInstructionContent);

            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            //20190928 TBL: Set ngày y lệnh = ngày giờ hiện tại (không lấy giây)
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            (MedicalInstructionDateContent as Conductor<object>).PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Equals("DateTime"))
                {
                    IntravenousContent.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
                    InPatientDailyDiagnosisContent.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
                }
            };
            LoadDoctorStaffCollection();
            AllRouteOfAdministration = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_RouteOfAdministration).ToObservableCollection();
            ListV_ReconmendTimeUsageDistance = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ReconmendTimeUsageDistance).ToObservableCollection();
            ListV_TransferRateUnit= Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_TransferRateUnit).ToObservableCollection();
        }

        #region ViewModel
        //private ISearchPatientAndRegistration _searchRegistrationContent;

        //public ISearchPatientAndRegistration SearchRegistrationContent
        //{
        //    get { return _searchRegistrationContent; }
        //    set
        //    {
        //        _searchRegistrationContent = value;
        //        NotifyOfPropertyChange(() => SearchRegistrationContent);
        //    }
        //}

        //private IPatientInfo _patientSummaryInfoContent;

        //public IPatientInfo PatientSummaryInfoContent
        //{
        //    get { return _patientSummaryInfoContent; }
        //    set
        //    {
        //        _patientSummaryInfoContent = value;
        //        NotifyOfPropertyChange(() => PatientSummaryInfoContent);
        //    }
        //}

        private IInPatientDailyDiagnosis _inPatientDailyDiagnosisContent;

        public IInPatientDailyDiagnosis InPatientDailyDiagnosisContent
        {
            get
            {
                return _inPatientDailyDiagnosisContent;
            }
            set
            {
                _inPatientDailyDiagnosisContent = value;
                NotifyOfPropertyChange(() => InPatientDailyDiagnosisContent);
            }
        }

        private IMedicalInstruction _medicalInstructionContent;
        public IMedicalInstruction MedicalInstructionContent
        {
            get
            {
                return _medicalInstructionContent;
            }
            set
            {
                _medicalInstructionContent = value;
                NotifyOfPropertyChange(() => MedicalInstructionContent);
            }
        }

        private IMonitoringVitalSigns _monitoringVitalSignsContent;
        public IMonitoringVitalSigns MonitoringVitalSignsContent
        {
            get
            {
                return _monitoringVitalSignsContent;
            }
            set
            {
                _monitoringVitalSignsContent = value;
                NotifyOfPropertyChange(() => MonitoringVitalSignsContent);
            }
        }

        private IIntravenous _intravenousContent;
        public IIntravenous IntravenousContent
        {
            get
            {
                return _intravenousContent;
            }
            set
            {
                _intravenousContent = value;
                NotifyOfPropertyChange(() => IntravenousContent);
            }
        }
        private IMinHourDateControl _MedicalInstructionDateContent;
        public IMinHourDateControl MedicalInstructionDateContent
        {
            get { return _MedicalInstructionDateContent; }
            set
            {
                _MedicalInstructionDateContent = value;
                NotifyOfPropertyChange(() => MedicalInstructionDateContent);
            }
        }
        #endregion
        #region Properties
        //▼====: #023
        private bool _IsCOVID = false;
        public bool IsCOVID
        {
            get
            {
                return _IsCOVID;
            }
            set
            {
                if (_IsCOVID != value)
                {
                    _IsCOVID = value;
                    NotifyOfPropertyChange(() => IsCOVID);
                }
            }
        }
        //▲====: #023
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
                    MedicalInstructionContent.CurRegistration = _curRegistration;
                    _curRegistration.InPatientInstruction = new InPatientInstruction();
                    //IntravenousContent.InPtInstruction = _curRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
                    NotifyOfPropertyChange(() => CurRegistration);
                    GetAntibioticTreatmentsByPtRegID();
                    NotifyOfPropertyChange(() => IsCanUpdateCurrentInstruction);
                }
            }
        }
        private ObservableCollection<InPatientDeptDetail> _InPatientDeptDetails;
        public ObservableCollection<InPatientDeptDetail> InPatientDeptDetails
        {
            get
            {
                return _InPatientDeptDetails;
            }
            set
            {
                _InPatientDeptDetails = value;
                NotifyOfPropertyChange(() => InPatientDeptDetails);
            }
        }
        private InPatientDeptDetail _SelectedInPatientDeptDetail;
        public InPatientDeptDetail SelectedInPatientDeptDetail
        {
            get
            {
                return _SelectedInPatientDeptDetail;
            }
            set
            {
                _SelectedInPatientDeptDetail = value;
                NotifyOfPropertyChange(() => SelectedInPatientDeptDetail);
            }
        }
        private ObservableCollection<DeptLocation> _gAttachLocationCollection;
        public ObservableCollection<DeptLocation> gAttachLocationCollection
        {
            get
            {
                return _gAttachLocationCollection;
            }
            set
            {
                _gAttachLocationCollection = value;
                NotifyOfPropertyChange(() => gAttachLocationCollection);
            }
        }
        private ObservableCollection<BedPatientAllocs> _BedAllocations;
        public ObservableCollection<BedPatientAllocs> BedAllocations
        {
            get
            {
                return _BedAllocations;
            }
            set
            {
                _BedAllocations = value;
                NotifyOfPropertyChange(() => BedAllocations);
            }
        }
        private BedPatientAllocs _SelectedBedAllocation;
        public BedPatientAllocs SelectedBedAllocation
        {
            get
            {
                return _SelectedBedAllocation;
            }

            set
            {
                _SelectedBedAllocation = value;
                NotifyOfPropertyChange(() => SelectedBedAllocation);
            }
        }
        private bool _gIsUpdate = false;
        public bool gIsUpdate
        {
            get
            {
                return _gIsUpdate;
            }
            set
            {
                _gIsUpdate = value;
                NotifyOfPropertyChange(() => gIsUpdate);
                NotifyOfPropertyChange(() => IsCanUpdateCurrentInstruction);
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
                InPatientDailyDiagnosisContent.Registration_DataStorage = Registration_DataStorage;
            }
        }
        private bool _IsDialogViewObject;
        public bool IsDialogViewObject
        {
            get
            {
                return _IsDialogViewObject;
            }
            set
            {
                if (_IsDialogViewObject == value)
                {
                    return;
                }
                _IsDialogViewObject = value;
                NotifyOfPropertyChange(() => IsDialogViewObject);
                NotifyOfPropertyChange(() => IsCanUpdateCurrentInstruction);
            }
        }
        private bool _IsApplyUpdateInstruction = Globals.ServerConfigSection.CommonItems.IsApplyUpdateInstruction;
        public bool IsApplyUpdateInstruction
        {
            get
            {
                return _IsApplyUpdateInstruction;
            }
            set
            {
                if (_IsApplyUpdateInstruction == value)
                {
                    return;
                }
                _IsApplyUpdateInstruction = value;
                NotifyOfPropertyChange(() => IsApplyUpdateInstruction);
            }
        }
        private bool _CantUpdateInstruction;
        public bool CantUpdateInstruction
        {
            get
            {
                return _CantUpdateInstruction;
            }
            set
            {
                if (_CantUpdateInstruction == value)
                {
                    return;
                }
                _CantUpdateInstruction = value;
                NotifyOfPropertyChange(() => CantUpdateInstruction);
            }
        }
        private ICollectionView _IntravenousView;
        public ICollectionView IntravenousView
        {
            get
            {
                return _IntravenousView;
            }
            set
            {
                if (_IntravenousView == value)
                {
                    return;
                }
                _IntravenousView = value;
                NotifyOfPropertyChange(() => IntravenousView);
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _IntravenousContentCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> IntravenousContentCollection
        {
            get
            {
                return _IntravenousContentCollection;
            }
            set
            {
                if (_IntravenousContentCollection == value)
                {
                    return;
                }
                _IntravenousContentCollection = value;
                NotifyOfPropertyChange(() => IntravenousContentCollection);
                IntravenousView = CollectionViewSource.GetDefaultView(IntravenousContentCollection == null ? new ObservableCollection<ClientReqOutwardDrugClinicDeptPatient>() : IntravenousContentCollection.Where(x => x.GenMedProductID.GetValueOrDefault(0) != 0).Select(x => new ClientReqOutwardDrugClinicDeptPatient(x, IntravenousContentCollection)));
                IntravenousView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentIntravenousGroupingObject"));
            }
        }
        private List<ReqOutwardDrugClinicDeptPatient> DeletedIntravenousContentCollection { get; set; }
        private ReqOutwardDrugClinicDeptPatient _SelectedIntravenousContent;
        public ReqOutwardDrugClinicDeptPatient SelectedIntravenousContent
        {
            get
            {
                return _SelectedIntravenousContent;
            }
            set
            {
                if (_SelectedIntravenousContent != value)
                {
                    _SelectedIntravenousContent = value;
                    NotifyOfPropertyChange(() => SelectedIntravenousContent);
                }
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        private ObservableCollection<RefStorageWarehouseLocation> _InNutritionStorageCollection;
        public ObservableCollection<RefStorageWarehouseLocation> InNutritionStorageCollection
        {
            get
            {
                return _InNutritionStorageCollection;
            }
            set
            {
                if (_InNutritionStorageCollection != value)
                {
                    _InNutritionStorageCollection = value;
                    NotifyOfPropertyChange(() => InNutritionStorageCollection);
                }
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        private RefStorageWarehouseLocation _SelectedInNutritionStorage;
        public RefStorageWarehouseLocation SelectedInNutritionStorage
        {
            get
            {
                return _SelectedInNutritionStorage;
            }
            set
            {
                if (_SelectedInNutritionStorage != value)
                {
                    _SelectedInNutritionStorage = value;
                    NotifyOfPropertyChange(() => SelectedInNutritionStorage);
                }
            }
        }

        //==== #002
        private ObservableCollection<RefStorageWarehouseLocation> _IntravenousStorageCollection;
        public ObservableCollection<RefStorageWarehouseLocation> IntravenousStorageCollection
        {
            get
            {
                return _IntravenousStorageCollection;
            }
            set
            {
                if (_IntravenousStorageCollection != value)
                {
                    _IntravenousStorageCollection = value;
                    NotifyOfPropertyChange(() => IntravenousStorageCollection);
                }
            }
        }
        private RefStorageWarehouseLocation _SelectedIntravenousStorage;
        public RefStorageWarehouseLocation SelectedIntravenousStorage
        {
            get
            {
                return _SelectedIntravenousStorage;
            }
            set
            {
                if (_SelectedIntravenousStorage != value)
                {
                    _SelectedIntravenousStorage = value;
                    NotifyOfPropertyChange(() => SelectedIntravenousStorage);
                }
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
                if (_gSelectedDoctorStaff == value) return;
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }

        private bool _IsUpdateDoctorStaff;
        public bool IsUpdateDoctorStaff
        {
            get { return _IsUpdateDoctorStaff; }
            set
            {
                if (_IsUpdateDoctorStaff != value)
                {
                    _IsUpdateDoctorStaff = value;
                    NotifyOfPropertyChange(() => IsUpdateDoctorStaff);
                }
            }
        }
        private bool _IntravenousEditView = true;
        public bool IntravenousEditView
        {
            get
            {
                return _IntravenousEditView;
            }
            set
            {
                if (_IntravenousEditView == value)
                {
                    return;
                }
                _IntravenousEditView = value;
                NotifyOfPropertyChange(() => IntravenousEditView);
                NotifyOfPropertyChange(() => IntravenousEditButtonContent);
            }
        }
        public string IntravenousEditButtonContent
        {
            get
            {
                return IntravenousEditView ? eHCMSResources.G2386_G1_Xem : eHCMSResources.K1872_G1_ChSua;
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _AntibioticTreatmentUsageHistories;
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> AntibioticTreatmentUsageHistories
        {
            get
            {
                return _AntibioticTreatmentUsageHistories;
            }
            set
            {
                if (_AntibioticTreatmentUsageHistories == value)
                {
                    return;
                }
                _AntibioticTreatmentUsageHistories = value;
                NotifyOfPropertyChange(() => AntibioticTreatmentUsageHistories);
                IntravenousContent.AntibioticTreatmentUsageHistories = AntibioticTreatmentUsageHistories;
                if (AntibioticTreatmentUsageHistories != null && AntibioticTreatmentUsageHistories.Any(x => x.CurrentAntibioticTreatment != null))
                {
                    var GroupingObject = AntibioticTreatmentUsageHistories.Where(x => x.CurrentAntibioticTreatment != null).Select(x => new AntibioticTreatment { AntibioticTreatmentID = x.CurrentAntibioticTreatment.AntibioticTreatmentID, AntibioticTreatmentTitle = x.CurrentAntibioticTreatment.AntibioticTreatmentTitle }).ToList().Distinct();
                    foreach (var aItem in AntibioticTreatmentUsageHistories.Where(x => x.CurrentAntibioticTreatment != null))
                    {
                        aItem.CurrentAntibioticTreatment = GroupingObject.FirstOrDefault(x => x.AntibioticTreatmentID == aItem.AntibioticTreatmentID.Value);
                    }
                }
                AntibioticTreatmentUsageHistoriesView = CollectionViewSource.GetDefaultView(AntibioticTreatmentUsageHistories);
                AntibioticTreatmentUsageHistoriesView.GroupDescriptions.Add(new PropertyGroupDescription("CurrentAntibioticTreatment"));
                AntibioticTreatmentUsageHistoriesView.SortDescriptions.Add(new SortDescription("CurrentAntibioticTreatment.AntibioticTreatmentID", ListSortDirection.Ascending));
                AntibioticTreatmentUsageHistoriesView.SortDescriptions.Add(new SortDescription("RefGenericDrugDetail.BrandName", ListSortDirection.Ascending));
                AntibioticTreatmentUsageHistoriesView.SortDescriptions.Add(new SortDescription("MedicalInstructionDate", ListSortDirection.Ascending));
            }
        }
        private ICollectionView _AntibioticTreatmentUsageHistoriesView;
        public ICollectionView AntibioticTreatmentUsageHistoriesView
        {
            get
            {
                return _AntibioticTreatmentUsageHistoriesView;
            }
            set
            {
                if (_AntibioticTreatmentUsageHistoriesView == value)
                {
                    return;
                }
                _AntibioticTreatmentUsageHistoriesView = value;
                NotifyOfPropertyChange(() => AntibioticTreatmentUsageHistoriesView);
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> DeletedOnCurrentNutritionCollection;
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _CurrentNutritionCollection;
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> CurrentNutritionCollection
        {
            get
            {
                return _CurrentNutritionCollection;
            }
            set
            {
                if (_CurrentNutritionCollection == value)
                {
                    return;
                }
                _CurrentNutritionCollection = value;
                NotifyOfPropertyChange(() => CurrentNutritionCollection);
                DeletedOnCurrentNutritionCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
        }

        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> _CurrentDrugCollection;
        public ObservableCollection<ReqOutwardDrugClinicDeptPatient> CurrentDrugCollection
        {
            get
            {
                return _CurrentDrugCollection;
            }
            set
            {
                if (_CurrentDrugCollection == value)
                {
                    return;
                }
                _CurrentDrugCollection = value;
                NotifyOfPropertyChange(() => CurrentDrugCollection);
                DeletedOnCurrentDrugCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
        }
        private ObservableCollection<ReqOutwardDrugClinicDeptPatient> DeletedOnCurrentDrugCollection = null;
        private ObservableCollection<MDAllergy> _PatientMDAllergyCollection;
        public ObservableCollection<MDAllergy> PatientMDAllergyCollection
        {
            get
            {
                return _PatientMDAllergyCollection;
            }
            set
            {
                if (_PatientMDAllergyCollection != value)
                {
                    _PatientMDAllergyCollection = value;
                    NotifyOfPropertyChange(() => PatientMDAllergyCollection);
                }
            }
        }
        private ObservableCollection<MedicalConditionRecord> _PatientMedicalConditionCollection;
        public ObservableCollection<MedicalConditionRecord> PatientMedicalConditionCollection
        {
            get
            {
                return _PatientMedicalConditionCollection;
            }
            set
            {
                if (_PatientMedicalConditionCollection == value)
                {
                    return;
                }
                _PatientMedicalConditionCollection = value;
                NotifyOfPropertyChange(() => PatientMedicalConditionCollection);
            }
        }
        //Danh sách thuốc nằm trong phác đồ đang điều trị
        private ObservableCollection<GetDrugForSellVisitor> _DrugsInTreatmentRegimen;
        public ObservableCollection<GetDrugForSellVisitor> DrugsInTreatmentRegimen
        {
            get => _DrugsInTreatmentRegimen;
            set
            {
                if (_DrugsInTreatmentRegimen == value)
                {
                    return;
                }
                _DrugsInTreatmentRegimen = value;
                NotifyOfPropertyChange(() => DrugsInTreatmentRegimen);
            }
        }
        private ObservableCollection<DrugAndConTra> _allMedProductContraIndicatorRelToMedCond;
        public ObservableCollection<DrugAndConTra> allMedProductContraIndicatorRelToMedCond
        {
            get
            {
                return _allMedProductContraIndicatorRelToMedCond;
            }
            set
            {
                if (_allMedProductContraIndicatorRelToMedCond != value)
                {
                    _allMedProductContraIndicatorRelToMedCond = value;
                    NotifyOfPropertyChange(() => allMedProductContraIndicatorRelToMedCond);
                }
            }
        }
        private bool _IsSearchByGenericName;
        public bool IsSearchByGenericName
        {
            get
            {
                return _IsSearchByGenericName;
            }
            set
            {
                if (_IsSearchByGenericName != value)
                {
                    _IsSearchByGenericName = value;
                    NotifyOfPropertyChange(() => IsSearchByGenericName);
                }
            }
        }
        public bool IsCanUpdateCurrentInstruction
        {
            get
            {
                if (CurRegistration == null || CurRegistration.InPatientInstruction == null ||
                     CurRegistration.InPatientInstruction.DoctorStaff == null)
                {
                    return false;
                }
                return IsUpdateDiagConfirmInPT ||
                    //(!IsDialogViewObject || 
                    (!CantUpdateInstruction && 
                    (CurRegistration.InPatientInstruction.DoctorStaff.StaffID == Globals.LoggedUserAccount.StaffID ||
                    (CurRegistration.InPatientInstruction.Staff != null &&
                    CurRegistration.InPatientInstruction.Staff.StaffID == Globals.LoggedUserAccount.StaffID)));
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        private ReqOutwardDrugClinicDeptPatient _CurrentSelectedNutrition;
        public ReqOutwardDrugClinicDeptPatient CurrentSelectedNutrition
        {
            get
            {
                return _CurrentSelectedNutrition;
            }
            set
            {
                if (_CurrentSelectedNutrition != value)
                {
                    _CurrentSelectedNutrition = value;
                    NotifyOfPropertyChange(() => CurrentSelectedNutrition);
                }
            }
        }

        private ReqOutwardDrugClinicDeptPatient _CurrentSelectedDrug;
        public ReqOutwardDrugClinicDeptPatient CurrentSelectedDrug
        {
            get
            {
                return _CurrentSelectedDrug;
            }
            set
            {
                if (_CurrentSelectedDrug != value)
                {
                    _CurrentSelectedDrug = value;
                    NotifyOfPropertyChange(() => CurrentSelectedDrug);
                }
            }
        }
        private bool _IsUpdateDiagConfirmInPT;
        public bool IsUpdateDiagConfirmInPT
        {
            get { return _IsUpdateDiagConfirmInPT; }
            set
            {
                if (_IsUpdateDiagConfirmInPT = value)
                {
                    _IsUpdateDiagConfirmInPT = value;
                    NotifyOfPropertyChange(() => IsUpdateDiagConfirmInPT);
                }
            }
        }
        #endregion
        #region Handle
        public void Handle(RegistrationSelectedForInPtInstruction_1 message)
        {
            if (this.GetView() != null && message != null && message.PtRegistration != null)
            {
                OpenRegistration(message.PtRegistration.PtRegistrationID);
            }
        }
        public void Handle(RegistrationSelectedForInPtInstruction_2 message)
        {
            if (this.GetView() != null && message != null && message.PtRegistration != null)
            {
                OpenRegistration(message.PtRegistration.PtRegistrationID);
            }
        }
        public void Handle(ReloadLoadMedicalInstructionEvent message)
        {
            if (this.GetView() != null && message != null && message.gInPatientInstruction != null)
            {
                ReloadLoadInPatientInstruction(message.gInPatientInstruction.IntPtDiagDrInstructionID);
            }
        }
        //▼===== #003
        //TBL: Phải tự bắn và tự bắt vì khi mở popup lên để tạo mới dựa trên cũ thì viewmodel vẫn đang được sử dụng trên tab. Nếu có vấn đề thì cần review lại
        public void Handle(CreateNewFromOld_ForInPatientInstruction message)
        {
            if (!FormIsEnabled)
            {
                return;
            }
            if (this.GetView() != null && message != null )//&& message.DiagnosisTreatmentContent != null)
            {
                InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.DoctorStaffID = message.DoctorStaffID;
                RenewCmd();
                if (CreateNewFromOldFlag)
                {
                    CurrentDrugCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                    if (message.DrugList != null && message.DrugList.Count > 0)
                    {
                        foreach (var item in message.DrugList)
                        {
                            item.OutClinicDeptReqID = 0;
                            item.ReqQty = 0;
                            item.DateTimeSelection = Globals.GetCurServerDateTime();
                            CurrentDrugCollection.Add(item);
                        }
                    }
                    if (message.IntravenousDetails != null && message.IntravenousDetails.Count > 0)
                    {

                        Intravenous tempIntravenousList = new Intravenous();
                        tempIntravenousList.IntravenousDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                        foreach (var item in message.IntravenousDetails)
                        {
                            item.OutClinicDeptReqID = 0;
                            item.DateTimeSelection = Globals.GetCurServerDateTime();
                            tempIntravenousList.IntravenousDetails.Add(item);
                        }
                        IntravenousContent.IntravenousList.Add(tempIntravenousList);
                        IntravenousContent.ReloadIntravenousListForCreateNewFromOld();
                    }
                }
                //InPatientDailyDiagnosisContent.CreateNewFromOld(message.DiagnosisTreatmentContent, message.ICD10List);
                //MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
                //gIsUpdate = false;
                //MonitoringVitalSignsContent.gInPatientInstruction = new InPatientInstruction();
                ////20191122 TBL: BM 0019573: Fix lỗi tạo mới dựa trên cũ thì trường Chăm sóc cấp không clear
                //MonitoringVitalSignsContent.gInPatientInstruction.V_LevelCare = new Lookup();
                //MedicalInstructionContent.AllRegistrationItems = new ObservableCollection<MedRegItemBase>();
                ////▼====: CMN: Remove code behind cause of bad perform
                ////IntravenousContent.IntravenousDetailsList = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                ////IntravenousContent.CVReqDetails = CollectionViewSource.GetDefaultView(new ObservableCollection<ReqOutwardDrugClinicDeptPatient>());
                //IntravenousContent.IntravenousList = new ObservableCollection<Intravenous>();
                //IntravenousContent.ReloadIntravenousList();
                //CurrentDrugCollection.Add(InitNullDrugItem());
                ////▲====:
                //IntravenousContentCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                //AddIntravenousBlankRow();

                //// VuTTM - Nutrition
                //// Create a new nutrition collection, and then add empty nutrition into the one 
                //CurrentNutritionCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>()
                //{
                //    InitNullDrugItem()
                //};

                ////▼===== #005
                //gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
                ////▲===== #005
            }
        }
        //▲===== #003
        #endregion
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //var homeVm = Globals.GetViewModel<IHome>();
            //if (homeVm.OutstandingTaskContent != null && homeVm.OutstandingTaskContent is IConsultationOutstandingTask)
            //    ((IConsultationOutstandingTask)homeVm.OutstandingTaskContent).IsMedicalInstruction = true;
            //homeVm.OutstandingTaskContent = Globals.GetViewModel<IInPatientInstructionOutstandingTask>();
            IsDialogViewObject = this.IsDialogView;
            CantUpdateInstruction = IsApplyUpdateInstruction ? false : IsDialogViewObject;
            IntravenousContent.IsDialogViewObject = IsApplyUpdateInstruction ? CantUpdateInstruction : IsDialogViewObject;
            if (!IsDialogViewObject || !CantUpdateInstruction) //20200414 TBL: BM 0030111: Nếu là xem lại y lệnh thì không cần chạy
            {
                Coroutine.BeginExecute(DoGetIntravenousStorageCollection());
                GetAllDrugsContrainIndicator();
                Globals.EventAggregator.Publish(new LoadMedicalInstructionEvent { gRegistration = new PatientRegistration() });
            }
            //▼===== 20191005: Không tạo ra blank row khi vừa vào màn hình mà chưa có tìm bệnh nhân gì lên khám/
            //AddIntravenousBlankRow();
            //▲=====
            CheckDeptCanUseInstruction();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //var homeVm = Globals.GetViewModel<IHome>();
            //homeVm.OutstandingTaskContent = null;
            //if (homeVm.OutstandingTaskContent != null && homeVm.OutstandingTaskContent is IConsultationOutstandingTask)
            //    ((IConsultationOutstandingTask)homeVm.OutstandingTaskContent).IsMedicalInstruction = false;
        }
        public void OpenRegistration(long regID)
        {
            Coroutine.BeginExecute(DoOpenRegistration(regID));
        }
        public void cboDepartments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedInPatientDeptDetail == null || CurRegistration == null) return;
            gAttachLocationCollection = new ObservableCollection<DeptLocation> { SelectedInPatientDeptDetail.DeptLocation };
            BedAllocations = CurRegistration.BedAllocations.Where(x => x.ResponsibleDeptID == SelectedInPatientDeptDetail.DeptLocation.DeptID).ToObservableCollection();
            //20191130 TBL: Nếu 1 bệnh nhân chuyển giường nhiều lần thì lấy mặc định giường bệnh nhân đang nằm để tạo y lệnh mới
            //SelectedBedAllocation = BedAllocations != null ? BedAllocations.FirstOrDefault() : null;
            SelectedBedAllocation = BedAllocations != null ? BedAllocations.Where(x => x.IsActive).FirstOrDefault() : null;
            if (CurRegistration.InPatientInstruction != null)
            {
                CurRegistration.InPatientInstruction.LocationInDept = gAttachLocationCollection.FirstOrDefault();
            }
        }
        public IEnumerator<IResult> DoOpenRegistration(long regID)
        {
            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetAdmissionInfo = true;
            LoadRegisSwitch.IsGetBedAllocations = true;
            var loadRegTask = new LoadRegistrationInfo_InPtTask(regID, (int)AllLookupValues.V_FindPatientType.NOI_TRU, LoadRegisSwitch);
            yield return loadRegTask;
            LoadRegistrationInfo(loadRegTask.Registration);
        }
        public void LoadRegistrationInfo(PatientRegistration aRegistration, bool LoadInstructionAlso = true)
        {
            if (aRegistration == null)
            {
                //Thong bao khong load duoc dang ky
                Globals.EventAggregator.Publish(new ShowMessageEvent { Message = "Error!(7)" });
            }
            else
            {
                //KMx: Ở đây set CurRegistration rồi, trong hàm ShowOldRegistration lại set nữa là sao? (16/09/2014 11:33).
                CurRegistration = aRegistration;
                /*
                PatientSummaryInfoContent.CurrentPatient = CurRegistration.Patient;
                PatientSummaryInfoContent.CurrentRegistration = CurRegistration;
                PatientSummaryInfoContent.HiBenefit = CurRegistration.PtInsuranceBenefit;
                //PatientSummaryInfoContent.SetPatientHISumInfo(CurRegistration.PtHISumInfo);
                */
                //==== #001
                this.InPatientDeptDetails = CurRegistration != null & CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.InPatientDeptDetails != null ? CurRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG).ToObservableCollection() : null;
                SelectedInPatientDeptDetail = this.InPatientDeptDetails.FirstOrDefault();
                //▼====: #023
                if (SelectedInPatientDeptDetail != null)
                {
                    IsCOVID = SelectedInPatientDeptDetail.IsTreatmentCOVID;
                    if (IntravenousContent != null)
                    {
                        IntravenousContent.IsCOVID = SelectedInPatientDeptDetail.IsTreatmentCOVID;
                    }
                }
                else
                {
                    if (IntravenousContent != null)
                    {
                        IntravenousContent.IsCOVID = false;
                    }
                    IsCOVID = false;
                }
                //▲====: #023
                if (LoadInstructionAlso)
                {
                    LoadInPatientInstruction();
                }
                //CMN: Chỉ được gọi từ bên ngoài và 100% tạo mới view bằng Popup nên không cần gọi clear
                //else
                //{
                //    UploadViewContent(CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID > 0);
                //}
                if (!this.IsDialogView)
                {
                    Globals.EventAggregator.Publish(new LoadMedicalInstructionEvent { gRegistration = CurRegistration });
                }
                //==== #001
                //ShowOldRegistration(CurRegistration);
                if (CurRegistration.PatientID.GetValueOrDefault(0) == 0)
                {
                    PatientMDAllergyCollection = new ObservableCollection<MDAllergy>();
                    PatientMedicalConditionCollection = new ObservableCollection<MedicalConditionRecord>();
                }
                else
                {
                    GetPatientMDAllergyCollection(CurRegistration.PatientID.Value, 1);
                    GetPatientMedicalCondition(CurRegistration.PatientID.Value, -1);
                }
                CheckDeptCanUseInstruction();
              
            }
        }
        public void CheckInPatientDailyDiagnosisTreatment(DiagnosisTreatment curDiagnosisTreatment, out string Error, out string WarningMessage)
        {
            Error = "";
            WarningMessage = "";

            //▼====: #024
            if(Globals.ServerConfigSection.CommonItems.IsApplyTimeForAllowUpdateMedicalInstruction)
            {
                int TimeForAllowUpdateMedicalInstruction = Globals.ServerConfigSection.CommonItems.TimeForAllowUpdateMedicalInstruction;
                int NumOfDayAllowMedicalInstruction = SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null && SelectedInPatientDeptDetail.DeptLocation.RefDepartment != null
                    ? SelectedInPatientDeptDetail.DeptLocation.RefDepartment.NumOfDayAllowMedicalInstruction : 0;
                if (Globals.GetCurServerDateTime().Hour >= TimeForAllowUpdateMedicalInstruction)
                {
                    Error += string.Format("Đã quá thời gian cho phép tạo y lệnh, Thời gian cho phép đến {0}h mỗi ngày!", TimeForAllowUpdateMedicalInstruction);
                    return;
                }
                if (MedicalInstructionDateContent != null && MedicalInstructionDateContent.DateTime.GetValueOrDefault().Date.Subtract(Globals.GetCurServerDateTime().Date).Days > NumOfDayAllowMedicalInstruction)
                {
                    Error = string.Format("Ngày tạo y lệnh vượt quá số ngày cho phép so với ngày hiện tại của Khoa! \n - Khoa: {0} , \n - Số ngày cho phép nhập trước y lệnh: {1} ngày", 
                        SelectedInPatientDeptDetail.DeptLocation.RefDepartment.DeptName, 
                        NumOfDayAllowMedicalInstruction);
                    return;
                }
            }
            //▲====: #024

            if (curDiagnosisTreatment == null)
            {
                Error = eHCMSResources.A0405_G1_Msg_InfoChuaCoCD;
                return;
            }

            //▼====: #020
            // 20210628 TNHX: Lấy thêm tuổi theo ngày của trẻ sơ sinh thì không cần nhập huyết áp (Khoa sản)
            int DOB = 0;
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatientRegistration != null)
            {
                DOB = (Registration_DataStorage.CurrentPatientRegistration.ExamDate.Date - Registration_DataStorage.CurrentPatient.DOB.GetValueOrDefault().Date).Days;
            }
            // thêm kiểm tra giá trị bắt buộc của mạch/ nhiệt độ/ huyết áp
            if (curDiagnosisTreatment.Pulse > (decimal)GlobalsNAV.BLOCK_PULSE_TOP || curDiagnosisTreatment.Pulse < (decimal)GlobalsNAV.BLOCK_PULSE_BOTTOM)
            {
                Error += string.Format(" - Giá trị \"Mạch\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PULSE_BOTTOM, GlobalsNAV.BLOCK_PULSE_TOP);
            }
            //▼==== #026
            if (curDiagnosisTreatment.RespiratoryRate > (decimal)GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP || curDiagnosisTreatment.RespiratoryRate < (decimal)GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM)
            {
                Error += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.BLOCK_RESPIRATORY_RATE_TOP);
            }
            //▲==== #026
            if (curDiagnosisTreatment.Temperature > (decimal)GlobalsNAV.BLOCK_TEMPERATURE_TOP
                || curDiagnosisTreatment.Temperature < (decimal)GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM)
            {
                Error += string.Format("\n - Giá trị \"Nhiệt độ\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM, GlobalsNAV.BLOCK_TEMPERATURE_TOP);
            }
            if (curDiagnosisTreatment.BloodPressure > (decimal)GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP || curDiagnosisTreatment.BloodPressure < (decimal)GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
            {
                Error += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP);
            }
            if (curDiagnosisTreatment.LowerBloodPressure > (decimal)GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP || curDiagnosisTreatment.LowerBloodPressure < (decimal)GlobalsNAV.BLOCK_PRESSURE_BOTTOM)
            {
                Error += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_PRESSURE_BOTTOM, GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP);
            }
            if (curDiagnosisTreatment.SpO2 > (decimal)GlobalsNAV.BLOCK_SPO2_TOP || curDiagnosisTreatment.SpO2 < (decimal)GlobalsNAV.BLOCK_SPO2_BOTTOM)
            {
                Error += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng cho phép {0} - {1}!", GlobalsNAV.BLOCK_SPO2_BOTTOM, GlobalsNAV.BLOCK_SPO2_TOP);
            }
            if (MedicalInstructionDateContent != null && Registration_DataStorage.CurrentPatient != null
                && Registration_DataStorage.CurrentPatientRegistration != null)
            {

                PhysicalExamination current = null;
                if (Registration_DataStorage.PtPhyExamList != null)
                {
                    //▼====: #024
                    if (!Globals.ServerConfigSection.CommonItems.IsApplyTimeForAllowUpdateMedicalInstruction)
                    {
                        current = Registration_DataStorage.PtPhyExamList.Where(x => x.RecordDate.GetValueOrDefault().Subtract(MedicalInstructionDateContent.DateTime.GetValueOrDefault()).Days == 0).ToObservableCollection().FirstOrDefault();
                        foreach (var item in Registration_DataStorage.PtPhyExamList)
                        {
                            if (item.RecordDate.GetValueOrDefault().Subtract(MedicalInstructionDateContent.DateTime.GetValueOrDefault()).Days == 0)
                            {
                                current = item;
                            }
                        }
                    }
                    else
                    {
                        current = Registration_DataStorage.PtPhyExamList.Where(x => x.RecordDate.GetValueOrDefault().Subtract(Globals.GetCurServerDateTime()).Days == 0).ToObservableCollection().FirstOrDefault();
                        foreach (var item in Registration_DataStorage.PtPhyExamList)
                        {
                            if (item.RecordDate.GetValueOrDefault().Subtract(Globals.GetCurServerDateTime()).Days == 0)
                            {
                                current = item;
                            }
                        }
                    }
                    //▲====: #024
                }

                if (current == null)
                {
                    Error += "\n - Chưa nhập dấu hiệu sinh tồn trong ngày. Vui lòng vào tab \"Theo dõi sinh hiệu\" để thêm mới!";
                }
                else
                {
                    if (DOB < Globals.ServerConfigSection.CommonItems.AgeMustHasDHST)
                    {
                        if (current.Weight == null || current.Weight == 0
                            || current.SpO2 == null //|| current.SpO2 == 0
                            || current.Temperature == null || current.Temperature == 0)
                        {
                            Error += string.Format("\n - Trẻ em dưới {0} tuổi phải nhập cân nặng/ SpO2/ nhiệt độ!", Globals.ServerConfigSection.CommonItems.AgeMustHasDHST / 365);
                        }
                    }
                    else
                    {
                        if (current.Pulse == null //|| current.Pulse == 0
                            || current.SystolicPressure == null //|| current.SystolicPressure == 0
                            || current.DiastolicPressure == null) //|| current.DiastolicPressure == 0)
                        {
                            Error += string.Format("\n - Người bệnh trên {0} tuổi phải nhập huyết áp/ mạch!", Globals.ServerConfigSection.CommonItems.AgeMustHasDHST / 365);
                        }
                    }
                }
            }
            // kiểm tra cảnh báo
            //▼==== #026
            if (curDiagnosisTreatment.RespiratoryRate > (decimal)GlobalsNAV.WARNING_RESPIRATORY_RATE_TOP || curDiagnosisTreatment.RespiratoryRate < (decimal)GlobalsNAV.WARNING_RESPIRATORY_RATE_BOTTOM)
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhịp thở\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_RESPIRATORY_RATE_BOTTOM, GlobalsNAV.WARNING_RESPIRATORY_RATE_TOP);
            }
            //▲==== #026
            if ((curDiagnosisTreatment.Temperature <= (decimal)GlobalsNAV.BLOCK_TEMPERATURE_TOP && curDiagnosisTreatment.Temperature > (decimal)GlobalsNAV.WARNING_TEMPERATURE_TOP)
                || (curDiagnosisTreatment.Temperature < (decimal)GlobalsNAV.WARNING_TEMPERATURE_BOTTOM && curDiagnosisTreatment.Temperature >= (decimal)GlobalsNAV.BLOCK_TEMPERATURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhiệt độ\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_TEMPERATURE_BOTTOM, GlobalsNAV.WARNING_TEMPERATURE_TOP);
            }
            if ((curDiagnosisTreatment.Pulse < (decimal)GlobalsNAV.BLOCK_PULSE_TOP && curDiagnosisTreatment.Pulse > (decimal)GlobalsNAV.WARNING_PULSE_TOP)
                || (curDiagnosisTreatment.Pulse < (decimal)GlobalsNAV.WARNING_PULSE_BOTTOM && curDiagnosisTreatment.Pulse > (decimal)GlobalsNAV.BLOCK_PULSE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Nhịp tim\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_PULSE_BOTTOM, GlobalsNAV.WARNING_PULSE_TOP);
            }
            if ((curDiagnosisTreatment.SpO2 < (decimal)GlobalsNAV.BLOCK_SPO2_TOP && curDiagnosisTreatment.SpO2 > (decimal)GlobalsNAV.WARNING_SPO2_TOP)
                || (curDiagnosisTreatment.SpO2 < (decimal)GlobalsNAV.WARNING_SPO2_BOTTOM && curDiagnosisTreatment.SpO2 > (decimal)GlobalsNAV.BLOCK_SPO2_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"SpO2\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_SPO2_BOTTOM, GlobalsNAV.WARNING_SPO2_TOP);
            }
            if ((curDiagnosisTreatment.BloodPressure < (decimal)GlobalsNAV.BLOCK_UPPER_PRESSURE_TOP && curDiagnosisTreatment.BloodPressure > (decimal)GlobalsNAV.WARNING_UPPER_PRESSURE_TOP)
                || (curDiagnosisTreatment.BloodPressure < (decimal)GlobalsNAV.WARNING_UPPER_PRESSURE_BOTTOM && curDiagnosisTreatment.BloodPressure > (decimal)GlobalsNAV.BLOCK_PRESSURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Huyết áp trên\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_UPPER_PRESSURE_BOTTOM, GlobalsNAV.WARNING_UPPER_PRESSURE_TOP);
            }
            if ((curDiagnosisTreatment.LowerBloodPressure < (decimal)GlobalsNAV.BLOCK_LOWER_PRESSURE_TOP && curDiagnosisTreatment.LowerBloodPressure > (decimal)GlobalsNAV.WARNING_LOWER_PRESSURE_TOP)
                || (curDiagnosisTreatment.LowerBloodPressure < (decimal)GlobalsNAV.WARNING_LOWER_PRESSURE_BOTTOM && curDiagnosisTreatment.LowerBloodPressure > (decimal)GlobalsNAV.BLOCK_PRESSURE_BOTTOM))
            {
                WarningMessage += string.Format("\n - Giá trị \"Huyết áp dưới\" ngoài khoảng {0} - {1}!", GlobalsNAV.WARNING_LOWER_PRESSURE_BOTTOM, GlobalsNAV.WARNING_LOWER_PRESSURE_TOP);
            }
            //▲====: #020
            //▼====: #016
            if (InPatientDailyDiagnosisContent.ICD10List != null && InPatientDailyDiagnosisContent.ICD10List.Count > 0)
            {
                bool HasICDCannotBeMain = false;
                string NameOfICDCannotBeMain = "Danh sách ICD không được làm mã bệnh chính:";
                ObservableCollection<DiagnosisIcd10Items> temp = InPatientDailyDiagnosisContent.ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                if (temp != null && temp.Count > 0)
                {
                    int bcount = 0;
                    for (int i = 0; i < temp.Count; i++)
                    {
                        if (temp[i].IsMain)
                        {
                            bcount++;
                            if (temp[i].DiseasesReference != null && temp[i].DiseasesReference.NotBeMain)
                            {
                                HasICDCannotBeMain = true;
                                NameOfICDCannotBeMain += " \n\t" + temp[i].DiseasesReference.ICD10Code + " - " + temp[i].DiseasesReference.DiseaseNameVN;
                            }
                        }
                    }
                    if (HasICDCannotBeMain)
                    {
                        Error = Error + NameOfICDCannotBeMain;
                    }
                }
            }
            //▲====: #016
        }
        private void EditInPatientInstruction(bool IsUpdate = false)
        {
            PatientRegistration ProcessRegistration = CurRegistration.DeepCopy();
            if (!InPatientDailyDiagnosisContent.CheckValidNewDiagnosis(SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null ? SelectedInPatientDeptDetail.DeptLocation.DeptID : 0)) return;
            if (ProcessRegistration == null)
            {
                MessageBox.Show(eHCMSResources.K0290_G1_ChonBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            string Error = "";
            //▼====: #020
            string WarningMessage = "";
            CheckInPatientDailyDiagnosisTreatment(InPatientDailyDiagnosisContent.DiagnosisTreatmentContent, out Error, out WarningMessage);
            if (!string.IsNullOrEmpty(Error))
            {
                void onInitDlg(IErrorBold confDlg)
                {
                    confDlg.ErrorTitle = eHCMSResources.K1576_G1_CBao;
                    confDlg.isCheckBox = false;
                    confDlg.SetMessage(Error, "");
                    confDlg.FireOncloseEvent = true;
                }
                GlobalsNAV.ShowDialog<IErrorBold>(onInitDlg);
                return;
            }
            else if (!string.IsNullOrEmpty(WarningMessage))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.isCheckBox = true;
                MessBox.ErrorTitle = "Cảnh báo bệnh lý";
                MessBox.SetMessage(WarningMessage, eHCMSResources.Z0627_G1_TiepTucLuu);
                MessBox.FireOncloseEvent = true;
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return;
                }
            }
            //▲====: #020
            if (ProcessRegistration.InPatientInstruction.LocationInDept == null || ProcessRegistration.InPatientInstruction.LocationInDept.DeptLocationID == 0)
            {
                MessageBox.Show(eHCMSResources.K0384_G1_ChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.A0571_G1_Msg_InfoChonBSCDinh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //==== #002
            string error = "";
            CheckMonitoringVitalSigns(MonitoringVitalSignsContent.gInPatientInstruction, out error);
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //==== #002
            ProcessRegistration.InPatientInstruction.Staff = Globals.LoggedUserAccount.Staff;
            //▼===== #005
            //ProcessRegistration.InPatientInstruction.DoctorStaff = Globals.LoggedUserAccount.Staff;
            ProcessRegistration.InPatientInstruction.DoctorStaff = gSelectedDoctorStaff;
            //▲===== #005
            ProcessRegistration.InPatientInstruction.InstructionDate = MedicalInstructionDateContent.DateTime.GetValueOrDefault(new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0));
            //▼====: #011
            if (ProcessRegistration.InPatientInstruction.InstructionDate > Globals.GetCurServerDateTime().AddDays(Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate))
            //▲====: #011
            {
                MessageBox.Show(eHCMSResources.Z2217_G1_NgYLenhKgVuotQuaNgHTai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20191008 TBL: Ngày nhập khoa sẽ có lúc vẫn còn second và millisecond nên set về = 0
            DateTime NgayNhapKhoa = SelectedInPatientDeptDetail.FromDate.AddSeconds(-(SelectedInPatientDeptDetail.FromDate.Second));
            NgayNhapKhoa = NgayNhapKhoa.AddMilliseconds(-(NgayNhapKhoa.Millisecond));
            if (SelectedInPatientDeptDetail != null && ProcessRegistration.InPatientInstruction.InstructionDate < NgayNhapKhoa)
            {
                MessageBox.Show(eHCMSResources.Z2218_G1_NgYLenhKgNhoHonNgNKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (SelectedInPatientDeptDetail.ToDate != null)
            {
                //20191008 TBL: Ngày xuất khoa sẽ có lúc vẫn còn second và millisecond nên set về = 0
                DateTime NgayXuatKhoa = SelectedInPatientDeptDetail.ToDate.Value.AddSeconds(-(SelectedInPatientDeptDetail.ToDate.Value.Second));
                NgayXuatKhoa = NgayXuatKhoa.AddMilliseconds(-(NgayXuatKhoa.Millisecond));
                if (SelectedInPatientDeptDetail != null && ProcessRegistration.InPatientInstruction.InstructionDate > NgayXuatKhoa)
                {
                    MessageBox.Show(eHCMSResources.Z2219_G1_NgYLenhKgVuotQuaNgXKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            ProcessRegistration.InPatientInstruction.InPatientDeptDetailID = SelectedInPatientDeptDetail != null ? SelectedInPatientDeptDetail.InPatientDeptDetailID : 0;
            ProcessRegistration.InPatientInstruction.BedPatientID = SelectedBedAllocation != null ? SelectedBedAllocation.BedPatientID : (long?)null;
            if (ProcessRegistration.InPatientInstruction.RegistrationDetails != null)
            {
                //▼====: #019
                long RefGenDrugCatID_2ForDrug = Globals.ServerConfigSection.InRegisElements.RefGenDrugCatID_2ForDrug;
                if (ProcessRegistration.InPatientInstruction.RegistrationDetails.Where(x => x.RefMedicalServiceItem.UseAnalgesic).ToList().Count() > 0)
                {
                    if (CurrentDrugCollection == null || CurrentDrugCollection != null
                        && CurrentDrugCollection.Where(x => x.RefGenericDrugDetail.RefGenDrugCatID_2 == RefGenDrugCatID_2ForDrug).ToList().Count() == 0)
                    {
                        if (MessageBox.Show("DVKT có sử dụng thuốc gây tê nhưng chưa có chỉ định thuốc gây tê. " + eHCMSResources.I0941_G1_I, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                        {
                            return;
                        }
                    }
                }
                //▲====: #019
                foreach (var item in ProcessRegistration.InPatientInstruction.RegistrationDetails)
                {
                    item.DoctorStaff = ProcessRegistration.InPatientInstruction.DoctorStaff;
                    item.MedicalInstructionDate = ProcessRegistration.InPatientInstruction.InstructionDate;
                    item.ResultDate = ProcessRegistration.InPatientInstruction.InstructionDate; //20190928 TBL: Set ngày kết quả = ngày y lệnh để khi load bill và lưu bill sẽ không bị cảnh báo ngày y lệnh lớn hơn ngày kết quả
                    item.DeptLocation = ProcessRegistration.InPatientInstruction.LocationInDept;
                    //▼===== 20200109 TTM: Bổ sung thêm code để đưa HisID và TotalCoPayment cho các dịch vụ hoặc CLS khi chỉ định
                    CalcInvoiceItem(item);
                    //▲===== 
                }
            }
            if (ProcessRegistration.InPatientInstruction.PclRequests != null)
            {
                foreach (var item in ProcessRegistration.InPatientInstruction.PclRequests)
                {
                    item.DoctorStaff = ProcessRegistration.InPatientInstruction.DoctorStaff;
                    item.MedicalInstructionDate = ProcessRegistration.InPatientInstruction.InstructionDate;
                    item.ReqFromDeptID = ProcessRegistration.InPatientInstruction.LocationInDept == null ? 0 : ProcessRegistration.InPatientInstruction.LocationInDept.DeptID;
                    item.ReqFromDeptLocID = ProcessRegistration.InPatientInstruction.LocationInDept == null ? 0 : ProcessRegistration.InPatientInstruction.LocationInDept.DeptLocationID;
                    //20191126 TBL: BM 0019654: Khi chỉ định CLS từ y lệnh thì lấy chẩn đoán của y lệnh cho phiếu chỉ định CLS 
                    item.Diagnosis = InPatientDailyDiagnosisContent.DiagnosisTreatmentContent != null ? InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.DiagnosisFinal : "";
                    foreach (var detail in item.PatientPCLRequestIndicators)
                    {
                        detail.ResultDate = ProcessRegistration.InPatientInstruction.InstructionDate; //20190928 TBL: Set ngày kết quả = ngày y lệnh cho từng CLS để khi load bill và lưu bill sẽ không bị cảnh báo ngày y lệnh lớn hơn ngày kết quả
                        //▼===== 20190801 TTM: Bổ sung thêm DoctorStaff và MedicalInstructionDate cho chi tiết phiếu chỉ định CLS.
                        detail.DoctorStaff = ProcessRegistration.InPatientInstruction.DoctorStaff;
                        detail.MedicalInstructionDate = ProcessRegistration.InPatientInstruction.InstructionDate;
                        //▲===== 20190801
                        //▼===== 20200109 TTM: Bổ sung thêm code để đưa HisID và TotalCoPayment cho các dịch vụ hoặc CLS khi chỉ định
                        CalcInvoiceItem(detail);
                        //▲=====
                    }
                }
            }
            //Thuốc và Y cụ
            //ProcessRegistration.InPatientInstruction.IntravenousPlan = IntravenousContent.IntravenousList.Where(x => x.IntravenousID != 0).ToList();
            if (CurrentDrugCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && x.MDose + x.ADose + x.EDose + x.NDose == 0))
            {
                Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentDrugCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && !CheckRequiredTruyenTinhMach(x.Notes) 
                    && x.V_RouteOfAdministration == 61319 && (x.TransferRate < 1 || x.V_TransferRateUnit==0)))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                MessBox.IsShowReason = true;
                MessBox.SetMessage(eHCMSResources.Z2878_G1_SoLanTocDoTruyenKhongTrong, eHCMSResources.Z0627_G1_TiepTucLuu);
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return;
                }

                //Globals.ShowMessage(eHCMSResources.Z2878_G1_SoLanTocDoTruyenKhongTrong, eHCMSResources.G0442_G1_TBao);
                //return;
            }
            if (CurrentDrugCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && string.IsNullOrEmpty(x.UsageDistance)))
            {
                Globals.ShowMessage("Khoảng cách dùng không được để trống", eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (IntravenousContent.IntravenousList.Any(x => x.IntravenousID == 0))
            {
                ProcessRegistration.InPatientInstruction.ReqOutwardDetails = IntravenousContent.IntravenousList.Where(x => x.IntravenousID == 0).FirstOrDefault().IntravenousDetails.Where(x => x.OutClinicDeptReqID == 0 || x.RecordState == RecordState.MODIFIED).ToObservableCollection();
            }
            if (ProcessRegistration.InPatientInstruction.ReqOutwardDetails == null)
            {
                ProcessRegistration.InPatientInstruction.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            }
            if (CurrentDrugCollection.Any(x => x.OutClinicDeptReqID == 0 && x.GenMedProductID.GetValueOrDefault(0) > 0))
            {
                foreach (var Item in CurrentDrugCollection.Where(x => x.OutClinicDeptReqID == 0 && x.GenMedProductID.GetValueOrDefault(0) > 0))
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(Item);
                }
            }
            if (CurrentDrugCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && x.IsHashChanged))
            {
                foreach (var Item in CurrentDrugCollection.Where(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && x.IsHashChanged))
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(Item);
                }
            }
            if (DeletedOnCurrentDrugCollection != null && DeletedOnCurrentDrugCollection.Count > 0)
            {
                if (ProcessRegistration.InPatientInstruction.ReqOutwardDetails == null)
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                foreach (var Item in DeletedOnCurrentDrugCollection)
                {
                    Item.IsDeleted = true;
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(Item);
                }
            }
            if (IntravenousContent.DeletedIntravenousDetails != null && IntravenousContent.DeletedIntravenousDetails.Count > 0)
            {
                if (ProcessRegistration.InPatientInstruction.ReqOutwardDetails == null)
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                foreach (var item in IntravenousContent.DeletedIntravenousDetails)
                {
                    item.IsDeleted = true;
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(item);
                }
            }

            // VuTTM
            // For nutrition
            if (CurrentNutritionCollection.Any(x => x.OutClinicDeptReqID == 0 && x.GenMedProductID.GetValueOrDefault(0) > 0))
            {
                foreach (var Item in CurrentNutritionCollection.Where(x => x.OutClinicDeptReqID == 0 && x.GenMedProductID.GetValueOrDefault(0) > 0))
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(Item);
                }
            }
            if (CurrentNutritionCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && x.IsHashChanged))
            {
                foreach (var Item in CurrentNutritionCollection.Where(x => x.GenMedProductID.GetValueOrDefault(0) > 0 && x.IsHashChanged))
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(Item);
                }
            }
            if (DeletedOnCurrentNutritionCollection != null && DeletedOnCurrentNutritionCollection.Count > 0)
            {
                if (ProcessRegistration.InPatientInstruction.ReqOutwardDetails == null)
                {
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                foreach (var Item in DeletedOnCurrentNutritionCollection)
                {
                    Item.IsDeleted = true;
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(Item);
                }
            }

            //Dịch truyền
            if (IntravenousContentCollection != null && IntravenousContentCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) > 0))
            {
                if (IntravenousContentCollection.Where(x => x.IntravenousPlan_InPtID.GetValueOrDefault(0) == 0 && x.GenMedProductID.GetValueOrDefault(0) > 0).GroupBy(x => x.GroupOrdinal).Any(x => !x.Any(i => i.StartDateTime != null)))
                {
                    Globals.ShowMessage(eHCMSResources.Z2859_G2_DichTruyenChuaDNNBD, eHCMSResources.T0074_G1_I);
                    return;
                }
                if (IntravenousContentCollection.Any(x => x.IntravenousPlan_InPtID.GetValueOrDefault(0) > 0))
                {
                    if (IntravenousContentCollection.Where(x => x.IntravenousPlan_InPtID.GetValueOrDefault(0) > 0).GroupBy(x => x.IntravenousPlan_InPtID).Any(x => x.ToList().Select(i => i.GroupOrdinal).Distinct().Count() > 1))
                    {
                        Globals.ShowMessage(eHCMSResources.Z2873_G1_DichTruyenCuKhongCNNBD, eHCMSResources.T0074_G1_I);
                        return;
                    }
                    else if (IntravenousContentCollection.Where(x => x.IntravenousPlan_InPtID.GetValueOrDefault(0) > 0).GroupBy(x => x.IntravenousPlan_InPtID).Any(x => !x.ToList().Any(i => i.StartDateTime != null)))
                    {
                        Globals.ShowMessage(eHCMSResources.Z2859_G2_DichTruyenChuaDNNBD, eHCMSResources.T0074_G1_I);
                        return;
                    }
                }
                int AddingOrdinal = 0;
                if (IntravenousContentCollection.Any(x => x.GroupOrdinal == 0))
                {
                    AddingOrdinal = 1;
                }
                foreach (var aItem in IntravenousContentCollection.Where(x => x.IntravenousPlan_InPtID.GetValueOrDefault(0) <= 0))
                {
                    aItem.IntravenousPlan_InPtID = (aItem.GroupOrdinal + AddingOrdinal) * -1;
                }
                ProcessRegistration.InPatientInstruction.IntravenousPlan = IntravenousContentCollection.Where(x => x.StartDateTime != null).Select(x => new Intravenous
                {
                    IntravenousID = x.IntravenousPlan_InPtID.GetValueOrDefault(0),
                    FlowRate = x.FlowRate,
                    NumOfTimes = x.NumOfTimes,
                    StartDateTime = x.StartDateTime,
                    IntravenousDetails = IntravenousContentCollection.Where(i => i.GroupOrdinal == x.GroupOrdinal && i.GenMedProductID.GetValueOrDefault(0) > 0).ToObservableCollection(),
                    V_InfusionType = new Lookup { LookupID = (long)AllLookupValues.V_InfusionType.Infusion },
                    V_InfusionProcessType = new Lookup { LookupID = (long)AllLookupValues.V_InfusionProcessType.Continuous },
                    V_TimeIntervalUnit = new Lookup { LookupID = (long)AllLookupValues.V_TimeIntervalUnit.Time },
                }).ToList();
                if (ProcessRegistration.InPatientInstruction.IntravenousPlan.Any(x => string.IsNullOrEmpty(x.FlowRate) || string.IsNullOrEmpty(x.NumOfTimes) || x.FlowRate == "0" || x.NumOfTimes == "0"))
                {
                    Globals.ShowMessage(eHCMSResources.Z2878_G1_SoLanTocDoTruyenKhongTrong, eHCMSResources.T0432_G1_Error);
                    return;
                }
                if (ProcessRegistration.InPatientInstruction.IntravenousPlan.Any(x => x.IntravenousDetails.Select(i => i.GenMedProductID).GroupBy(i => i).Any(i => i.ToList().Count > 1)))
                {
                    Globals.ShowMessage(eHCMSResources.Z2884_G1_ThuocDichTruyenBiTrung, eHCMSResources.T0432_G1_Error);
                    return;
                }
            }
            if (DeletedIntravenousContentCollection != null && DeletedIntravenousContentCollection.Count > 0)
            {
                if (ProcessRegistration.InPatientInstruction.ReqOutwardDetails == null) ProcessRegistration.InPatientInstruction.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                foreach (var item in DeletedIntravenousContentCollection)
                {
                    item.IsDeleted = true;
                    ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Add(item);
                }
            }
            if (ProcessRegistration.InPatientInstruction.ReqOutwardDetails != null && ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Any(x => x.RefGenericDrugDetail != null && x.RefGenericDrugDetail.V_InstructionOrdinalType == (long)AllLookupValues.V_InstructionOrdinalType.KhangSinh && x.AntibioticTreatmentID.GetValueOrDefault(0) == 0))
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z2930_G1_ThuocPhaiThuocDotKS, string.Join(",", ProcessRegistration.InPatientInstruction.ReqOutwardDetails.Where(x => x.RefGenericDrugDetail != null && x.RefGenericDrugDetail.V_InstructionOrdinalType == (long)AllLookupValues.V_InstructionOrdinalType.KhangSinh && x.AntibioticTreatmentID.GetValueOrDefault(0) == 0).Select(x => x.RefGenericDrugDetail.BrandName).ToList())), eHCMSResources.G0442_G1_TBao);
                return;
            }
            //20191005 TBL: Đem những giá trị bên MonitoringVitalSignsContent set cho ProcessRegistration để lưu
            if (MonitoringVitalSignsContent.gInPatientInstruction != null)
            {
                ProcessRegistration.InPatientInstruction.PulseAndBloodPressure = MonitoringVitalSignsContent.gInPatientInstruction.PulseAndBloodPressure;
                ProcessRegistration.InPatientInstruction.SpO2 = MonitoringVitalSignsContent.gInPatientInstruction.SpO2;
                ProcessRegistration.InPatientInstruction.Temperature = MonitoringVitalSignsContent.gInPatientInstruction.Temperature;
                ProcessRegistration.InPatientInstruction.Sense = MonitoringVitalSignsContent.gInPatientInstruction.Sense;
                ProcessRegistration.InPatientInstruction.BloodSugar = MonitoringVitalSignsContent.gInPatientInstruction.BloodSugar;
                ProcessRegistration.InPatientInstruction.Urine = MonitoringVitalSignsContent.gInPatientInstruction.Urine;
                ProcessRegistration.InPatientInstruction.ECG = MonitoringVitalSignsContent.gInPatientInstruction.ECG;
                ProcessRegistration.InPatientInstruction.PhysicalExamOther = MonitoringVitalSignsContent.gInPatientInstruction.PhysicalExamOther;
                ProcessRegistration.InPatientInstruction.Diet = MonitoringVitalSignsContent.gInPatientInstruction.Diet;
                ProcessRegistration.InPatientInstruction.V_LevelCare = MonitoringVitalSignsContent.gInPatientInstruction.V_LevelCare;
                //▼==== #026
                ProcessRegistration.InPatientInstruction.RespiratoryRate = MonitoringVitalSignsContent.gInPatientInstruction.RespiratoryRate;
                //▲==== #026
            }
            if (ProcessRegistration.InPatientInstruction.V_LevelCare == null)
            {
                ProcessRegistration.InPatientInstruction.LevelCare = 1;
            }
            else
            {
                ProcessRegistration.InPatientInstruction.LevelCare = ProcessRegistration.InPatientInstruction.V_LevelCare.LookupID;
            }
            //▼===== #007
            if ((CurrentDrugCollection == null || CurrentDrugCollection.Count <= 1)
                && (CurrentNutritionCollection == null || CurrentNutritionCollection.Count <= 1) // VuTTM
                && (IntravenousContent.IntravenousDetailsList == null
                    || IntravenousContent.IntravenousDetailsList
                        .Where(x => x.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                        .ToObservableCollection().Count <= 0)
                && (ProcessRegistration.InPatientInstruction.RegistrationDetails == null
                    || ProcessRegistration.InPatientInstruction.RegistrationDetails
                        .Where(x => !x.MarkedAsDeleted).ToObservableCollection().Count <= 0)
                && (ProcessRegistration.InPatientInstruction.PclRequests == null
                    || ProcessRegistration.InPatientInstruction.PclRequests
                        .Where(x => (x.PatientPCLReqID == 0 && !x.MarkedAsDeleted
                            && x.PatientPCLRequestIndicators.Count > 0)
                            || (x.PatientPCLReqID > 0
                            && (!x.MarkedAsDeleted || x.PatientPCLRequestIndicators.Any(y => !y.MarkedAsDeleted))))
                        .ToObservableCollection().Count <= 0)
                && (ProcessRegistration.InPatientInstruction.IntravenousPlan == null
                    || ProcessRegistration.InPatientInstruction.IntravenousPlan.Count <= 0)
                && ((string.IsNullOrEmpty(ProcessRegistration.InPatientInstruction.InstructionOther) && Globals.ServerConfigSection.CommonItems.IsSaveMedicalInstructionWithoutPrescription)
                    || !Globals.ServerConfigSection.CommonItems.IsSaveMedicalInstructionWithoutPrescription))
            {
                MessageBox.Show(eHCMSResources.Z2974_G1_MsgYLenh, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲===== #007
            CallShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        ProcessRegistration.InPatientInstruction.RegistrationDetails = ProcessRegistration.InPatientInstruction.RegistrationDetails == null ? null : ProcessRegistration.InPatientInstruction.RegistrationDetails.Where(x => (x.PtRegDetailID == 0 && !x.MarkedAsDeleted) || (x.PtRegDetailID > 0 && x.MarkedAsDeleted)).ToObservableCollection();
                        ProcessRegistration.InPatientInstruction.PclRequests = ProcessRegistration.InPatientInstruction.PclRequests == null ? null : ProcessRegistration.InPatientInstruction.PclRequests.Where(x => (x.PatientPCLReqID == 0 && !x.MarkedAsDeleted) || (x.PatientPCLReqID > 0 && (x.MarkedAsDeleted || x.PatientPCLRequestIndicators.Any(y => y.MarkedAsDeleted)))).ToObservableCollection();
                        //contract.BeginAddInPatientInstruction(ProcessRegistration, IsUpdateDiagConfirmInPT, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginAddInPatientInstruction(ProcessRegistration, IsUpdate, WebIntPtDiagDrInstructionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            bool isNewRegistration = ProcessRegistration.PtRegistrationID <= 0;
                            long mIntPtDiagDrInstructionID;
                            try
                            {
                                contract.EndAddInPatientInstruction(out mIntPtDiagDrInstructionID, asyncResult);
                                CurRegistration = ProcessRegistration;
                                WebIntPtDiagDrInstructionID = 0;
                                //▼===== #004
                                InPatientDailyDiagnosisContent.SetValueWhenSaveAndUpdateDiag(mIntPtDiagDrInstructionID
                                    , SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null ? SelectedInPatientDeptDetail.DeptLocation.DeptID : 0);
                                Coroutine.BeginExecute(SaveNewDiagnosis(mIntPtDiagDrInstructionID, IsUpdate));
                                //Coroutine.BeginExecute(InPatientDailyDiagnosisContent.SaveNewDiagnosis(mIntPtDiagDrInstructionID, SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null ? SelectedInPatientDeptDetail.DeptLocation.DeptID : 0, IsUpdate), null, new EventHandler<ResultCompletionEventArgs>((o, e) =>
                                //{
                                //    ReloadLoadInPatientInstruction(mIntPtDiagDrInstructionID);
                                //    Globals.EventAggregator.Publish(new LoadMedicalInstructionEvent { gRegistration = ProcessRegistration });
                                //    CallHideBusyIndicator();
                                //}
                                //▲===== #004
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                //if (ProcessRegistration != null && ProcessRegistration.InPatientInstruction != null && ProcessRegistration.InPatientInstruction.IntPtDiagDrInstructionID > 0)
                                //    ReloadLoadInPatientInstruction(ProcessRegistration.InPatientInstruction.IntPtDiagDrInstructionID);
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
                    MessageBox.Show(ex.Message);
                    CallHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void SaveInPatientInstructionCmd()
        {
            if (CurRegistration == null || CurRegistration.InPatientInstruction == null)
            {
                return;
            }
            //▼==== #027
            if (CurRegistration.InPatientTransferDeptReqID > 0)
            {
                MessageBox.Show(eHCMSResources.Z3262_G1_BNDaDeNghiChKhoa, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲==== #027
            if (CurRegistration != null && CurRegistration.InPatientInstruction != null)
            {
                CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID = 0;
            }
            //EditInPatientInstruction();
            Coroutine.BeginExecute(EditCurrentInstruction_Routine(false));
        }
        public void RenewCmd(bool bNotClickButton = false)
        {
            CreateNewFromOldFlag = false;
            if (CurRegistration != null && CurRegistration.InPatientInstruction != null)
            {
                //▼====: #032
                if(CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.IsNeedTreatmentSummary)
                {
                    MessageBox.Show("Bệnh nhân đã điều trị trên 15 ngày. Đề nghị khoa phòng nhập Sơ kết 15 ngày điều trị theo QĐ 1895/1997 của BYT. Liên hệ Kế hoạch tổng hợp để biết thêm chi tiết!"
                        , eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▲====: #032
                //▼====: #023
                if (CurRegistration.InPatientInstruction != null)
                {
                    //20190928 TBL: Set ngày y lệnh = ngày giờ hiện tại (không lấy giây)
                    DateTime CurMedicalInstructionDateContent = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
                    MedicalInstructionDateContent.DateTime = CurMedicalInstructionDateContent;
                    // thêm điều kiện kiểm tra y lệnh sinh hiệu gần nhất vs DHST gần nhất. nếu quá thời gian của y lệnh sinh hiệu thì bắt buộc nhập mới DHST
                    Lookup PulseAndBloodPressure = Globals.AllLookupValueList.Where(x => x.ObjectValue == CurRegistration.InPatientInstruction.PulseAndBloodPressure).ToObservableCollection().FirstOrDefault();
                    bool ReUsedInPatientInstruction = false;
                    PhysicalExamination LastPhysicalExamination = new PhysicalExamination();
                    if (Registration_DataStorage != null && Registration_DataStorage.PtPhyExamList != null)
                    {
                        LastPhysicalExamination = Registration_DataStorage.PtPhyExamList.FirstOrDefault();
                    }
                    if (LastPhysicalExamination.Pulse == null)
                    {
                        ReUsedInPatientInstruction = false;
                    }
                    else if (PulseAndBloodPressure != null)
                    {
                        switch (PulseAndBloodPressure.LookupID)
                        {
                            case (long)V_ReconmendTime.FIVE_MINUTES:
                                if (CurMedicalInstructionDateContent.AddMinutes(-5) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.TEN_MINUTES:
                                if (CurMedicalInstructionDateContent.AddMinutes(-10) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.FIFTY_MINUTES:
                                if (CurMedicalInstructionDateContent.AddMinutes(-15) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.THIRTY_MINUTES:
                                if (CurMedicalInstructionDateContent.AddMinutes(-30) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.TWENTY_MINUTES:
                                if (CurMedicalInstructionDateContent.AddMinutes(-20) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.ONE_HOUR:
                                if (CurMedicalInstructionDateContent.AddHours(-1) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.TWO_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-2) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.FOUR_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-4) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.SIX_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-6) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.TWELVE_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-12) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.TWENTYFOUR_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-24) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.THREE_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-3) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.FIVE_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-5) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            case (long)V_ReconmendTime.EIGHT_HOURS:
                                if (CurMedicalInstructionDateContent.AddHours(-8) < LastPhysicalExamination.RecordDate)
                                {
                                    ReUsedInPatientInstruction = true;
                                }
                                break;
                            default:
                                ReUsedInPatientInstruction = true;
                                break;
                        }
                    }
                    else
                    {
                        ReUsedInPatientInstruction = true;
                    }
                    if (!ReUsedInPatientInstruction)
                    {
                        MessageBox.Show("Dấu hiệu sinh tồn hiện tại đã quá thời gian cần theo dõi theo yêu cầu của bác sĩ. Điều dưỡng cần lấy lại thông tin và nhập mới dấu hiệu sinh tồn."
                            , eHCMSResources.G0442_G1_TBao);
                        return;
                    }

                    DiagnosisTreatment DiagnosisTreatmentContent = new DiagnosisTreatment();
                    DiagnosisTreatmentContent = ObjectCopier.DeepCopy(InPatientDailyDiagnosisContent.DiagnosisTreatmentContent);
                    ObservableCollection<DiagnosisIcd10Items> ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                    ICD10List = ObjectCopier.DeepCopy(InPatientDailyDiagnosisContent.ICD10List);
                    //Globals.EventAggregator.Publish(new CreateNewFromOld_ForInPatientInstruction() { DiagnosisTreatmentContent = DiagnosisTreatmentContent, ICD10List = ICD10List });

                    if (CurRegistration.InPatientInstruction.PulseAndBloodPressure == null)
                    {
                        MonitoringVitalSignsContent.gInPatientInstruction = new InPatientInstruction();
                        //20191122 TBL: BM 0019573: Fix lỗi tạo mới thì trường Chăm sóc cấp không clear
                        MonitoringVitalSignsContent.gInPatientInstruction.V_LevelCare = new Lookup();
                        CurRegistration.InPatientInstruction.PulseAndBloodPressure = null;
                        CurRegistration.InPatientInstruction.SpO2 = null;
                        CurRegistration.InPatientInstruction.Temperature = null;
                        CurRegistration.InPatientInstruction.Sense = null;
                        CurRegistration.InPatientInstruction.BloodSugar = null;
                        CurRegistration.InPatientInstruction.Urine = null;
                        CurRegistration.InPatientInstruction.ECG = null;
                        CurRegistration.InPatientInstruction.PhysicalExamOther = null;
                        CurRegistration.InPatientInstruction.Diet = null;
                        CurRegistration.InPatientInstruction.V_LevelCare = null;
                        //▼==== #026
                        CurRegistration.InPatientInstruction.RespiratoryRate = null;
                        //▲==== #026
                    }
                    else
                    {
                        MonitoringVitalSignsContent.gInPatientInstruction = new InPatientInstruction();
                        MonitoringVitalSignsContent.gInPatientInstruction.V_LevelCare = new Lookup();
                        MonitoringVitalSignsContent.gInPatientInstruction = CurRegistration.InPatientInstruction;
                        //▼==== #026, #028
                        if (MonitoringVitalSignsContent.gInPatientInstruction.BloodSugar != null
                            && MonitoringVitalSignsContent.gInPatientInstruction.BloodSugar.Equals("Ngưng theo dõi"))
                        {
                            MonitoringVitalSignsContent.gInPatientInstruction.BloodSugar = null;
                        }

                        if (MonitoringVitalSignsContent.gInPatientInstruction.ECG != null
                            && MonitoringVitalSignsContent.gInPatientInstruction.ECG.Equals("Ngưng theo dõi"))
                        {
                            MonitoringVitalSignsContent.gInPatientInstruction.ECG = null;
                        }
                        //▲==== #026, #028
                    }
                }
                //▲====: #023

                IntravenousContentCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                AddIntravenousBlankRow();
                IntravenousContent.IntravenousList = new ObservableCollection<Intravenous>();
                IntravenousContent.ReloadIntravenousList();
                CurrentDrugCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                CurrentDrugCollection.Add(InitNullDrugItem());

                // VuTTM
                // Renew for nutrition
                CurrentNutritionCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>()
                {
                    InitNullDrugItem()
                };

                MedicalInstructionContent.LoadAllRegistrationItemsByID(0);
                cboDepartments_SelectionChanged(null, null);
                
                if (!bNotClickButton) //20191012 TBL: Nếu click vào Tạo mới thì mới thực hiện
                {
                    InPatientDailyDiagnosisContent.setDefaultValueWhenReNew();
                }
                //▼===== #001: Khi Renew thì lấy khoa đang nằm của bệnh nhân, bỏ code cũ ở UploadViewContent, nếu đặt ở đó khi tìm lại y lệnh cũ sẽ không đúng khoa của y lệnh.
                InPatientDeptDetails = CurRegistration != null & CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.InPatientDeptDetails != null ? CurRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.V_InPatientDeptStatus == AllLookupValues.InPatientDeptStatus.NHAP_KHOA_PHONG).ToObservableCollection() : null;
                SelectedInPatientDeptDetail = InPatientDeptDetails.FirstOrDefault();
                //▲===== #001
                //▼===== #018
                MedicalInstructionContent.SelectBedContent.DeptID = InPatientDeptDetails.FirstOrDefault().DeptLocation != null ? InPatientDeptDetails.FirstOrDefault().DeptLocation.DeptID : 0;
                MedicalInstructionContent.SelectBedContent.GetAllMedicalServiceItemsByType();
                //▲===== #018
                //▼====: #023
                //MonitoringVitalSignsContent.gInPatientInstruction = new InPatientInstruction();
                //20191122 TBL: BM 0019573: Fix lỗi tạo mới thì trường Chăm sóc cấp không clear
                //MonitoringVitalSignsContent.gInPatientInstruction.V_LevelCare = new Lookup();
                //CurRegistration.InPatientInstruction.PulseAndBloodPressure = null;
                //CurRegistration.InPatientInstruction.SpO2 = null;
                //CurRegistration.InPatientInstruction.Temperature = null;
                //CurRegistration.InPatientInstruction.Sense = null;
                //CurRegistration.InPatientInstruction.BloodSugar = null;
                //CurRegistration.InPatientInstruction.Urine = null;
                //CurRegistration.InPatientInstruction.ECG = null;
                //CurRegistration.InPatientInstruction.PhysicalExamOther = null;
                //CurRegistration.InPatientInstruction.Diet = null;
                //CurRegistration.InPatientInstruction.V_LevelCare = null;
                //▲====: #023
                gIsUpdate = false;
                MonitoringVitalSignsContent.EnableSaveCmd = false;
                //▼===== #005
                gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
                IsUpdateDoctorStaff = true;
                //▲===== #005
                //▼===== #006
                CurRegistration.InPatientInstruction.PclRequests = new ObservableCollection<PatientPCLRequest>();
                CurRegistration.InPatientInstruction.RegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                CurRegistration.InPatientInstruction.ReqOutwardDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                CurRegistration.InPatientInstruction.IntravenousPlan = new List<Intravenous>();
                //▲===== #006
                //▼===== 20200723 TTM: Khi bấm tạo mới sẽ refresh dịch truyền và mặc định cho gõ.
                IntravenousView.Refresh();
                IntravenousEditView = true;
                //▲===== 
                CreateNewFromOldFlag = true;
                if (LoadFromWebsiteFlag)
                {
                    LoadDataFromWeb(WebIntPtDiagDrInstructionID);
                }
            }
        }
        //▼===== #003
        public void CreateNewFromOld()
        {
            if (CurRegistration.InPatientInstruction != null)
            {
                if(CurrentDrugCollection.Count<=1 && IntravenousContent.IntravenousList.Count == 0)
                {
                    MessageBox.Show("Y lệnh không có thuốc và y cụ. Không thể tạo mới dựa trên cũ");
                    return;
                }
                //DiagnosisTreatment DiagnosisTreatmentContent = new DiagnosisTreatment();
                //DiagnosisTreatmentContent = ObjectCopier.DeepCopy(InPatientDailyDiagnosisContent.DiagnosisTreatmentContent);
                //ObservableCollection<DiagnosisIcd10Items> ICD10List = new ObservableCollection<DiagnosisIcd10Items>();
                //ICD10List = ObjectCopier.DeepCopy(InPatientDailyDiagnosisContent.ICD10List);
                ObservableCollection<ReqOutwardDrugClinicDeptPatient> DrugList = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                DrugList = ObjectCopier.DeepCopy(CurrentDrugCollection);
                ObservableCollection<ReqOutwardDrugClinicDeptPatient> IntravenousDetails = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                if (IntravenousContent.IntravenousList.Count > 0)
                {
                    foreach (var item in IntravenousContent.IntravenousList.FirstOrDefault().IntravenousDetails)
                    {
                        if (item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                        {
                            IntravenousDetails.Add(item);
                        }
                    }
                }
                long DoctorStaffID = InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.DoctorStaffID;
                TryClose();
                Globals.EventAggregator.Publish(new CreateNewFromOld_ForInPatientInstruction() {
                    DoctorStaffID = DoctorStaffID,
                    DrugList = DrugList,
                    IntravenousDetails = IntravenousDetails
                });
            }
        }
        //▲===== #003
        public void UpdateInPatientInstructionCmd()
        {
            if (CurRegistration == null && CurRegistration.InPatientInstruction == null) return;
            //▼==== #027
            if (CurRegistration.InPatientTransferDeptReqID > 0)
            {
                MessageBox.Show(eHCMSResources.Z3262_G1_BNDaDeNghiChKhoa, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲==== #027
            //EditInPatientInstruction(true);
            Coroutine.BeginExecute(EditCurrentInstruction_Routine(true));
        }
        public void PrintBillingInvoiceCmd()
        {
            if (CurRegistration == null && CurRegistration.InPatientInstruction == null) return;
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.PATIENTINSTRUCTION_TH;
                proAlloc.IntPtDiagDrInstructionID = CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        public void PrintBillingInvoiceCmd_TH()
        {
            if (CurRegistration == null && CurRegistration.InPatientInstruction == null) return;

            IReportCriteria ReportCriteria = Globals.GetViewModel<IReportCriteria>();
            GlobalsNAV.ShowDialog_V3<IReportCriteria>(ReportCriteria, (aView) =>
            {
                aView.FromDate = Globals.GetCurServerDateTime();
                aView.ToDate = Globals.GetCurServerDateTime();
            });
            if (ReportCriteria.IsCompleted)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.eItem = ReportName.PATIENTINSTRUCTION_TH;
                    proAlloc.RegistrationID = CurRegistration.PtRegistrationID;
                    proAlloc.FromDate = ReportCriteria.FromDate;
                    proAlloc.ToDate = ReportCriteria.ToDate;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }
        public void PrintPCLReq()
        {
            if (MedicalInstructionContent != null && MedicalInstructionContent.CurRegistration != null
                && MedicalInstructionContent.CurRegistration.InPatientInstruction != null
                && MedicalInstructionContent.CurRegistration.InPatientInstruction.PclRequests != null)
            {
                ObservableCollection<PatientPCLRequest> temp = MedicalInstructionContent.CurRegistration.InPatientInstruction.PclRequests;
                if (temp.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Root>");
                    foreach (var item in temp)
                    {
                        sb.Append("<PatientPCLReqIDList>");
                        sb.AppendFormat("<PatientPCLReqID>{0}</PatientPCLReqID>", item.PatientPCLReqID);
                        sb.Append("</PatientPCLReqIDList>");
                    }
                    sb.Append("</Root>");
                    void onInitDlg(ICommonPreviewView proAlloc)
                    {
                        proAlloc.Result = sb.ToString();
                        proAlloc.V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
                        proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID_TV_XML;
                    }
                    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn, eHCMSResources.G0442_G1_TBao);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn, eHCMSResources.G0442_G1_TBao);
            }
        }

        //▼====: #014
        private bool _ShowPhieuSaoThuoc = Globals.ServerConfigSection.CommonItems.WhichHospitalUseThisApp == 2 ? true : false;
        public bool ShowPhieuSaoThuoc
        {
            get { return _ShowPhieuSaoThuoc; }
            set
            {
                if (_ShowPhieuSaoThuoc != value)
                {
                    _ShowPhieuSaoThuoc = value;
                    NotifyOfPropertyChange(() => ShowPhieuSaoThuoc);
                }
            }
        }

        public void PrintDrugReq()
        {
            if (MedicalInstructionContent != null && MedicalInstructionContent.CurRegistration != null
                && IntravenousContentCollection != null && IntravenousContentCollection.Count > 0)
            {
                void onInitDlgPhgieuSaoThuoc(IClinicDeptReportDocumentPreview proAlloc)
                {
                    proAlloc.IntPtDiagDrInstructionID = MedicalInstructionContent.CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
                    //▼==== #029
                    proAlloc.eItem = ReportName.KK_PhieuCongKhaiThuoc;
                    //proAlloc.eItem = ReportName.XRptPhieuSaoThuoc;
                    //▲==== #029
                    proAlloc.V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
                }
                GlobalsNAV.ShowDialog<IClinicDeptReportDocumentPreview>(onInitDlgPhgieuSaoThuoc);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn, eHCMSResources.G0442_G1_TBao);
            }
        }
        //▲====: #014

        //▼====: #012
        public void PrintDVReq()
        {
            if (MedicalInstructionContent != null && MedicalInstructionContent.CurRegistration != null
                && MedicalInstructionContent.CurRegistration.InPatientInstruction != null
                && MedicalInstructionContent.CurRegistration.InPatientInstruction.RegistrationDetails != null)
            {
                ObservableCollection<PatientRegistrationDetail> temp = MedicalInstructionContent.CurRegistration.InPatientInstruction.RegistrationDetails;
                if (temp.Count > 0)
                {
                    bool HaveBloodService = false;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Root>");
                    sb.Append("<IDList>");
                    if (temp.Count > 0)
                    {
                        sb.Append("<ServiceIDList>");
                        List<long?> listDeptLocID = temp.Select(x => x.DeptLocID).Distinct().ToList();
                        foreach (long? deptLocID in listDeptLocID)
                        {
                            sb.Append("<DeptData>");
                            sb.AppendFormat("<DeptLocID>{0}</DeptLocID>", deptLocID);
                            string listID = "";
                            foreach (PatientRegistrationDetail itemRegDetail in temp)
                            {
                                if (itemRegDetail.DeptLocID == deptLocID)
                                {
                                    if (listID == "")
                                    {
                                        listID += itemRegDetail.ID;
                                    }
                                    else listID = listID + "," + itemRegDetail.ID;
                                }
                                if (itemRegDetail.RefMedicalServiceItem.RefMedicalServiceType != null
                                    && itemRegDetail.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes
                                    == (long)AllLookupValues.V_RefMedicalServiceTypes.MAU)
                                {
                                    HaveBloodService = true;
                                }
                            }
                            sb.AppendFormat("<IDList>{0}</IDList>", listID);
                            sb.Append("</DeptData>");
                            if (listID == "")
                            {
                                sb.Replace("<IDList></IDList>", "");
                                sb.Replace("<DeptData><DeptLocID>" + deptLocID + "</DeptLocID></DeptData>", "");
                            }
                        }
                        sb.Append("</ServiceIDList>");
                        sb.Replace("<ServiceIDList></ServiceIDList>", "");
                    }
                    sb.Append("</IDList>");
                    sb.Append("</Root>");
                    if (HaveBloodService)
                    {
                        void onInitDlgMau(ICommonPreviewView reportVm)
                        {
                            reportVm.PatientCode = MedicalInstructionContent.CurRegistration.Patient.PatientCode;
                            reportVm.ID = MedicalInstructionContent.CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
                            reportVm.eItem = ReportName.XRptPhieuCungCapMau;
                        }
                        GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlgMau);
                    }
                    if (temp.Count > 1 || (temp.Count == 1 && !HaveBloodService))
                    {
                        void onInitDlg(ICommonPreviewView reportVm)
                        {
                            reportVm.Result = sb.ToString();
                            reportVm.eItem = ReportName.REGISTRATION_INPT_SPECIFY_VOTES_XML;
                        }
                        GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn, eHCMSResources.G0442_G1_TBao);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn, eHCMSResources.G0442_G1_TBao);
            }
        }
        //▲====: #012

        public void CancelChangesCmd()
        {
            if (MessageBox.Show(eHCMSResources.A0138_G1_Msg_ConfBoQua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CurRegistration.InPatientInstruction = new InPatientInstruction();
                Coroutine.BeginExecute(DoOpenRegistration(CurRegistration.PtRegistrationID));
            }
        }
        //Xóa dòng chi tiết y lệnh
        public void DeleteIntravenousContent_Click(object sender, RoutedEventArgs e)
        {
            if (((sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient).GenMedProductID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (DeletedIntravenousContentCollection == null)
            {
                DeletedIntravenousContentCollection = new List<ReqOutwardDrugClinicDeptPatient>();
            }
            if (((sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient).OutClinicDeptReqID > 0)
            {
                DeletedIntravenousContentCollection.Add((sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient);
            }
            IntravenousContentCollection.Remove((sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient);
        }
        public void EditIntravenousContent_Click(object sender, RoutedEventArgs e)
        {
        }
        public void cboDrugName_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboDrugName = sender as AutoCompleteBox;
            if (cboDrugName.DataContext != null &&
                cboDrugName.DataContext is ReqOutwardDrugClinicDeptPatient &&
                (cboDrugName.DataContext as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail != null && (cboDrugName.DataContext as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail.BrandName == e.Parameter)
            {
                return;
            }
            GetRefGenMedProductDetails_Auto(e.Parameter, false, 0, 1000, cboDrugName);
        }
        public void cboDrugName_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (SelectedIntravenousContent != null)
            {
                RefGenMedProductDetails obj = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductDetails;
                SelectedIntravenousContent.RefGenericDrugDetail = obj.DeepCopy();
                AutoGetOrdinal(SelectedIntravenousContent);
                AddIntravenousBlankRow();
            }
        }
        public void txtDrugCode_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text) || (((sender as TextBox).DataContext as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail != null && (sender as TextBox).Text == ((sender as TextBox).DataContext as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail.Code))
            {
                return;
            }
            GetRefGenMedProductDetails_Auto((sender as TextBox).Text, true, 0, 1, null, (sender as TextBox).DataContext as ReqOutwardDrugClinicDeptPatient);
        }
        public void gvIntravenous_CurrentCellChanged(object sender, System.EventArgs e)
        {
            if (IntravenousContentCollection.Count(x => x.StartDateTime != null) > 1
                && IntravenousContentCollection.Any(x => x.GroupOrdinal == 0))
            {
                RecalIntravenousGroupID();
            }
            else if (IntravenousContentCollection.Any(x => x.GenMedProductID > 0) && (IntravenousContentCollection.Count(x => x.StartDateTime != null) == 1 ? 0 : IntravenousContentCollection.Count(x => x.StartDateTime != null)) != IntravenousContentCollection.Where(x => x.GenMedProductID > 0).Max(x => x.GroupOrdinal))
            {
                RecalIntravenousGroupID();
            }
            else if (IntravenousContentCollection.Any(x => x.GenMedProductID > 0) && IntravenousContentCollection.Last(x => x.GenMedProductID > 0).StartDateTime == null)
            {
                RecalIntravenousGroupID();
            }
        }
        public void gvIntravenous_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (!(e.Row.DataContext as ReqOutwardDrugClinicDeptPatient).IsHasStartDateTime && (e.Column == (sender as DataGrid).GetColumnByName("clFlowRate") || e.Column == (sender as DataGrid).GetColumnByName("clNumOfTimes")))
            {
                e.Cancel = true;
                return;
            }
            if (e.Column == (sender as DataGrid).GetColumnByName("clAntibioticOrdinal") &&
                ((e.Row.DataContext as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail == null ||
                    (e.Row.DataContext as ReqOutwardDrugClinicDeptPatient).RefGenericDrugDetail.V_InstructionOrdinalType == (long)AllLookupValues.V_InstructionOrdinalType.Thuong))
            {
                e.Cancel = true;
                return;
            }
        }
        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }
        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(() => CallAddExistsIntravenous())
            {
                HotKey_Registered_Name = "CallAddExistsIntravenous",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.N
            };
        }
        public void SwitchViewCmd()
        {
            if (CurRegistration == null || CurRegistration.InPatientInstruction == null || CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID == 0)
            {
                return;
            }
            IntravenousView.Refresh();
            IntravenousEditView = !IntravenousEditView;
        }
        public void EditAntibioticTreatmentCmd()
        {
            if (CurRegistration == null || CurRegistration.InPatientInstruction == null ||
                SelectedInPatientDeptDetail == null ||
                SelectedInPatientDeptDetail.DeptLocation == null ||
                SelectedInPatientDeptDetail.DeptLocation.DeptID == 0)
            {
                return;
            }
            IAntibioticTreatmentEdit mDialogView = Globals.GetViewModel<IAntibioticTreatmentEdit>();
            mDialogView.CurrentAntibioticTreatment = new AntibioticTreatment { PtRegistrationID = CurRegistration.PtRegistrationID, StartDate = Globals.GetCurServerDateTime() };
            mDialogView.DeptID = SelectedInPatientDeptDetail.DeptLocation.DeptID;
            mDialogView.PtRegistrationID = CurRegistration.PtRegistrationID;
            GlobalsNAV.ShowDialog_V3(mDialogView);
            if (mDialogView.IsUpdatedCompleted)
            {
                EditAntibioticTreatment(mDialogView.CurrentAntibioticTreatment);
            }
        }
        public void DrugView_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            ReqOutwardDrugClinicDeptPatient CurrentContext = e.Row.DataContext as ReqOutwardDrugClinicDeptPatient;
            if (CurrentContext == null)
            {
                return;
            }
            e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
            //Ngoài phác đồ
            //if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && DrugsInTreatmentRegimen != null && DrugsInTreatmentRegimen.Count > 0 && objRows != null && objRows.DrugID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(x => x.DrugID == objRows.DrugID))
            //{
            //    e.Row.Background = new SolidColorBrush(Color.FromArgb(46, 204, 113, 248));
            //}
            if (CurrentContext.RefGenericDrugDetail != null && CurrentContext.RefGenericDrugDetail.V_CatDrugType == (long)AllLookupValues.V_CatDrugType.DrugDept)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(236, 219, 77)); //Yellow
            }
            //if (CurrentContext.IsContraIndicator && Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicator)
            //{
            //    e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 69, 0)); //Red
            //}
            if (Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy > 0 && CurrentContext.RefGenericDrugDetail.InsuranceCover == true
                && Registration_DataStorage.CurrentPatientRegistration != null &&
                (Registration_DataStorage.CurrentPatientRegistration.HisID == null || Registration_DataStorage.CurrentPatientRegistration.HisID == 0))
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 205, 220)); //Orange
            }
        }
        public void DrugView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender as AxDataGridNy != null)
            {
                if ((sender as AxDataGridNy).SelectedItem != null)
                {
                    (sender as AxDataGridNy).BeginEdit();
                }
            }
        }
        //20191016 TBL: Khi click vào grid toa thuốc thì đi lấy phác đồ
        public void DrugView_GotFocus(object sender, RoutedEventArgs e)
        {
            ////20191127 TBL: Nếu thấy ServiceRecID của chẩn đoán cũ trước đó thì không cho tạo mới dựa trên cũ toa thuốc để tránh bị lỗi
            //if (CS_DS.DiagTreatment.ServiceRecID != ObjDiagnosisTreatment_Current.ServiceRecID)
            //{
            //    MessageBox.Show(eHCMSResources.Z2925_G1_VuiLongNhapCD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            ////20191127 
            //GetPhacDo();
        }
        public void DrugView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            AxDataGridNy CurrentDataGrid = sender as AxDataGridNy;
            if (CurrentDrugCollection == null)
            {
                return;
            }
            // Txd 28/092013 
            // For some reason the Grid call this function when focus was outside of the bottom row, so we have to check this to ensure that
            // Removelastrow does not stuff it up.
            if (CurrentDrugCollection.Count <= 0)
            {
                return;
            }
            //KMx: Click any cell of the last row, then click on space of the gird (click out of grid, error not appear).
            //When you click the save button, the function RemoveLastRowPrecriptionDetail() is called and it remove the last row of CurrentDrugCollection.
            //After remove the last row, caliburn will call grdPrescription_CellEditEnded() and exception occurs at line CurrentDrugCollection[grdPrescription.SelectedIndex] because index was out of range.
            //So we need to check index is valid (04/10/2016 11:43).
            if (CurrentDataGrid.SelectedIndex > CurrentDrugCollection.Count - 1)
            {
                return;
            }
            //DataGrid axNyGrid = (DataGrid)sender;
            //BindingExpression bindNy = axNyGrid.GetBindingExpression(DataGrid.ItemsSourceProperty);
            //System.Diagnostics.Debug.WriteLine(" =*=*=*====+++>>>> AxDataGridNy Binding Path: " + bindNy.ParentBinding.Path.Path);
            ReqOutwardDrugClinicDeptPatient CurrentContext = CurrentDrugCollection[CurrentDataGrid.SelectedIndex];
            if (CurrentContext == null)
            {
                return;
            }
            if (e.Column.Equals(CurrentDataGrid.GetColumnByName("DrugNameColumn")))
            {
                if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen)
                {
                    if (DrugsInTreatmentRegimen != null && DrugsInTreatmentRegimen.Count > 0 && CurrentContext != null && CurrentContext.GenMedProductID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(x => x.DrugID == CurrentContext.GenMedProductID))
                    {
                        e.Row.Background = new SolidColorBrush(Color.FromArgb(46, 204, 113, 248));
                    }
                    else
                    {
                        e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    }
                }
            }
            if (e.Row.GetIndex() == (CurrentDrugCollection.Count - 1) && e.EditAction == DataGridEditAction.Commit)
            {
                AddBlankRowIntoCurrentDrugCollection();
            }
            //▼===== #010
            //if (e.Column.Equals(CurrentDataGrid.GetColumnByName("DrugNameColumn")))
            //{
            //    if (CurrentContext.RefGenericDrugDetail != null && Check1ThuocBiDiUng(CurrentContext.RefGenericDrugDetail.BrandName))
            //    {
            //        MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, CurrentContext.RefGenericDrugDetail.BrandName.Trim()));
            //        return;
            //    }
            //    if (CurrentContext.GenMedProductID.GetValueOrDefault(0) > 0)
            //    {
            //        Globals.CheckContrain(PatientMedicalConditionCollection, CurrentContext.GenMedProductID.Value);
            //    }
            //}
            //else 
            //▲===== #010
            if (e.Column.Equals(CurrentDataGrid.GetColumnByName("QuantityColumn")))
            {
                if (CurrentContext.OutClinicDeptReqID == 0 && CurrentContext.RefGenericDrugDetail != null && CurrentContext.RefGenericDrugDetail.Remaining < CurrentContext.PrescribedQty)
                {
                    MessageBox.Show(eHCMSResources.A0977_G1_Msg_InfoSLgKhDuBan);
                }
            }
            else if (e.Column.Equals(CurrentDataGrid.GetColumnByName("MDoseColumn")))
            {
                CurrentContext.MDoseStr = AddMoreZeroAndChangeCommaToDot(CurrentContext.MDoseStr);
                ChangeAnyDoseQty(1, CurrentContext.MDoseStr, CurrentContext);
            }
            else if (e.Column.Equals(CurrentDataGrid.GetColumnByName("NDoseColumn")))
            {
                CurrentContext.NDoseStr = AddMoreZeroAndChangeCommaToDot(CurrentContext.NDoseStr);
                ChangeAnyDoseQty(2, CurrentContext.NDoseStr, CurrentContext);
            }
            else if (e.Column.Equals(CurrentDataGrid.GetColumnByName("ADoseColumn")))
            {
                CurrentContext.ADoseStr = AddMoreZeroAndChangeCommaToDot(CurrentContext.ADoseStr);
                ChangeAnyDoseQty(3, CurrentContext.ADoseStr, CurrentContext);
            }
            else if (e.Column.Equals(CurrentDataGrid.GetColumnByName("EDoseColumn")))
            {
                CurrentContext.EDoseStr = AddMoreZeroAndChangeCommaToDot(CurrentContext.EDoseStr);
                ChangeAnyDoseQty(4, CurrentContext.EDoseStr, CurrentContext);
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NutritionNameComboBox_Populating(object sender, PopulatingEventArgs e)
        {
            AxAutoComplete CurrentComboBox = sender as AxAutoComplete;
            long OutFromStoreObject = 1;
            if (SelectedIntravenousStorage != null)
            {
                OutFromStoreObject = SelectedInNutritionStorage.StoreID;
            }
            var SearchText = CurrentComboBox.SearchText;
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new PharmacyMedDeptServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName,
                        null,
                        SearchText,
                        OutFromStoreObject,
                        (long)AllLookupValues.MedProductType.NUTRITION, null, null, false, null, null, null, true
                        //▼====: #023
                        , IsCOVID
                        //▲====: #023
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = CurrentContract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                CurrentComboBox.ItemsSource = GroupRemaining(results).ToObservableCollection();
                            }
                            else
                            {
                                CurrentComboBox.ItemsSource = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            CurrentComboBox.ItemsSource = null;
                        }
                        finally
                        {
                            CurrentComboBox.PopulateComplete();
                        }
                    }), CurrentComboBox);
                }
            });
            CurrentThread.Start();
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NutritionNameComboBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AxAutoComplete CurrentComboBox = sender as AxAutoComplete;
            var CurrentContext = CurrentSelectedNutrition;
            RefGenMedProductDetails CurrentItem = CurrentComboBox.SelectedItem as RefGenMedProductDetails;
            if (CurrentContext != null && CurrentItem != null)
            {
                CurrentContext.RefGenericDrugDetail = CurrentItem.DeepCopy();
                ClearDrugDataRowContent(CurrentContext);
                if (CurrentContext.RefGenericDrugDetail.V_InstructionOrdinalType != (long)AllLookupValues.V_InstructionOrdinalType.Thuong)
                {
                    var temp = IntravenousContent.AntibioticTreatmentCollection.DeepCopy();
                    IntravenousContent.AntibioticTreatmentCollection = temp.OrderByDescending(z => z.AntibioticTreatmentID)
                        .Where(x => x.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.Instruction)
                        .ToObservableCollection();
                    CurrentContext.CurrentAntibioticTreatment = temp.OrderByDescending(z => z.AntibioticTreatmentID)
                        .Where(x => x.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.Instruction)
                        .FirstOrDefault();
                }
                Globals.AutoSetAntibioticIndex(AntibioticTreatmentUsageHistories, CurrentContext, MedicalInstructionDateContent.DateTime);
                AddBlankRowIntoCurrentNutritionCollection();
            }
        }

        public void DrugNameComboBox_Populating(object sender, PopulatingEventArgs e)
        {
            AxAutoComplete CurrentComboBox = sender as AxAutoComplete;
            long OutFromStoreObject = 1;
            if (SelectedIntravenousStorage != null)
            {
                OutFromStoreObject = SelectedIntravenousStorage.StoreID;
            }
            var SearchText = CurrentComboBox.SearchText;
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new PharmacyMedDeptServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(IsSearchByGenericName, null, SearchText, OutFromStoreObject
                        , (long)AllLookupValues.MedProductType.THUOC, null, null, false, null, null, null, true
                        //▼====: #023
                        , IsCOVID
                        //▲====: #023
                        , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = CurrentContract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                CurrentComboBox.ItemsSource = GroupRemaining(results).ToObservableCollection();
                            }
                            else
                            {
                                CurrentComboBox.ItemsSource = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            CurrentComboBox.ItemsSource = null;
                        }
                        finally
                        {
                            CurrentComboBox.PopulateComplete();
                        }
                    }), CurrentComboBox);
                }
            });
            CurrentThread.Start();
        }
        public void DrugNameComboBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AxAutoComplete CurrentComboBox = sender as AxAutoComplete;
            var CurrentContext = CurrentSelectedDrug;
            RefGenMedProductDetails CurrentItem = CurrentComboBox.SelectedItem as RefGenMedProductDetails;
            if (CurrentContext != null && CurrentItem != null)
            {
                CurrentContext.RefGenericDrugDetail = CurrentItem.DeepCopy();
                ClearDrugDataRowContent(CurrentContext);
                if (CurrentContext.RefGenericDrugDetail.V_InstructionOrdinalType != (long)AllLookupValues.V_InstructionOrdinalType.Thuong)
                {
                    var temp = IntravenousContent.AntibioticTreatmentCollection.DeepCopy();
                    IntravenousContent.AntibioticTreatmentCollection = temp.OrderByDescending(z => z.AntibioticTreatmentID).Where(x => x.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.Instruction).ToObservableCollection();
                    CurrentContext.CurrentAntibioticTreatment = temp.OrderByDescending(z => z.AntibioticTreatmentID).Where(x => x.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.Instruction).FirstOrDefault();
                }
                Globals.AutoSetAntibioticIndex(AntibioticTreatmentUsageHistories, CurrentContext, MedicalInstructionDateContent.DateTime);
                AddBlankRowIntoCurrentDrugCollection();
                CurrentSelectedDrug.Notes = string.IsNullOrEmpty(CurrentItem.Notes) ? "" : CurrentItem.Notes;
                if (CurrentItem.ListRouteOfAdministration != null && CurrentItem.ListRouteOfAdministration.Count > 0)
                {
                    CurrentSelectedDrug.V_RouteOfAdministration = CurrentItem.ListRouteOfAdministration.FirstOrDefault().LookupID;
                    CurrentSelectedDrug.IsTruyenTinhMach = CurrentSelectedDrug.V_RouteOfAdministration == 61319;
                    CurrentSelectedDrug.TransferRate = 0;
                    CurrentSelectedDrug.V_TransferRateUnit = 0;
                }
            }
        }
        public void RemoveCurrentDrugItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient == null)
            {
                return;
            }
            var CurrentItem = (sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient;
            if (CurrentItem.RefGenericDrugDetail == null || CurrentItem.GenMedProductID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (CurrentItem.OutClinicDeptReqID > 0)
            {
                DeletedOnCurrentDrugCollection.Add(CurrentItem.DeepCopy());
            }
            CurrentDrugCollection.Remove(CurrentItem);
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveCurrentNutritionItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient == null)
            {
                return;
            }
            var CurrentItem = (sender as Button).DataContext as ReqOutwardDrugClinicDeptPatient;
            if (CurrentItem.RefGenericDrugDetail == null || CurrentItem.GenMedProductID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            if (CurrentItem.OutClinicDeptReqID > 0)
            {
                DeletedOnCurrentNutritionCollection.Add(CurrentItem.DeepCopy());
            }
            CurrentNutritionCollection.Remove(CurrentItem);
        }

        #endregion
        #region Methods
        private void CallAddExistsIntravenous()
        {
            if (SelectedIntravenousContent == null || SelectedIntravenousContent.IntravenousPlan_InPtID.GetValueOrDefault(0) <= 0 || IntravenousContentCollection == null)
            {
                return;
            }
            if (IntravenousContentCollection.Any(x => x.IntravenousPlan_InPtID == SelectedIntravenousContent.IntravenousPlan_InPtID && x.GenMedProductID.GetValueOrDefault(0) == 0))
            {
                return;
            }
            var mNewItem = SelectedIntravenousContent.DeepCopy();
            mNewItem.RefGenericDrugDetail = null;
            mNewItem.GenMedProductID = null;
            mNewItem.OutClinicDeptReqID = 0;
            mNewItem.StartDateTime = null;
            mNewItem.FlowRate = "";
            mNewItem.NumOfTimes = "";
            mNewItem.Notes = null;
            mNewItem.PrescribedQty = 0;
            IntravenousContentCollection.Insert(IntravenousContentCollection.IndexOf(IntravenousContentCollection.FirstOrDefault(x => x.IntravenousPlan_InPtID == SelectedIntravenousContent.IntravenousPlan_InPtID && x.IsHasStartDateTime)), mNewItem);
        }
        private void RecalIntravenousGroupID()
        {
            int GroupPosition = 0;
            if (IntravenousContentCollection.Count(x => x.StartDateTime != null) > 1)
            {
                GroupPosition = 1;
            }
            foreach (var item in IntravenousContentCollection)
            {
                item.GroupOrdinal = GroupPosition;
                if (item.StartDateTime != null)
                {
                    GroupPosition++;
                }
            }
        }
        private bool CheckIntravenousBlankRow()
        {
            if (IntravenousContentCollection != null && IntravenousContentCollection.Count > 0)
            {
                for (int i = 0; i < IntravenousContentCollection.Count; i++)
                {
                    if (IntravenousContentCollection[i].GenMedProductID == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void AddIntravenousBlankRow()
        {
            if (CheckIntravenousBlankRow())
            {
                if (IntravenousContentCollection == null)
                {
                    IntravenousContentCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                }
                ReqOutwardDrugClinicDeptPatient p = new ReqOutwardDrugClinicDeptPatient { GenMedProductID = 0, PrescribedQty = 1, ReqQty = 1, ReqDrugInClinicDeptID = 1 };
                IntravenousContentCollection.Add(p);
            }
            RecalIntravenousGroupID();
        }
        private IList<RefGenMedProductDetails> GroupRemaining(IList<RefGenMedProductDetails> results)
        {
            if (results == null || results.Count == 0)
            {
                return null;
            }
            var ListRefGMP = from RefGMP in results
                             group RefGMP by new
                             {
                                 RefGMP.GenMedProductID,
                                 RefGMP.BrandName,
                                 RefGMP.SelectedUnit.UnitName,
                                 RefGMP.RequestQty,
                                 RefGMP.Code,
                                 RefGMP.ProductCodeRefNum,
                                 RefGMP.RefGenMedDrugDetails.Content,
                                 RefGMP.GenericID,
                                 RefGMP.GenericName,
                                 RefGMP.V_MedProductType,
                                 SelectedUnitUse = RefGMP.SelectedUnitUse == null ? null : RefGMP.SelectedUnitUse.UnitName,
                                 RefGMP.V_InstructionOrdinalType,
                                 RefGMP.MinDayOrdinalContinueIsAllowable,
                                 RefGMP.RefGenDrugCatID_1,
                                 RefGMP.RefGenDrugCatID_2
                                 , RefGMP.Administration
                                 , RefGMP.Notes
                                 //▼====: #032
                                 , RefGMP.DispenseVolume
                                 //▲====: #032
                             }
                             into RefGMPGroup
                             select new
                             {
                                 Remaining = RefGMPGroup.Sum(groupItem => groupItem.Remaining),
                                 RefGMPGroup.Key.GenMedProductID,
                                 RefGMPGroup.Key.UnitName,
                                 RefGMPGroup.Key.BrandName,
                                 RefGMPGroup.Key.GenericID,
                                 RefGMPGroup.Key.GenericName,
                                 RefGMPGroup.Key.Content,
                                 RefGMPGroup.Key.Code,
                                 Qty = RefGMPGroup.Key.RequestQty,
                                 RefGMPGroup.Key.ProductCodeRefNum,
                                 RefGMPGroup.Key.V_MedProductType,
                                 RefGMPGroup.Key.SelectedUnitUse,
                                 RefGMPGroup.Key.V_InstructionOrdinalType,
                                 RefGMPGroup.Key.MinDayOrdinalContinueIsAllowable,
                                 //▼====: #019
                                 RefGMPGroup.Key.RefGenDrugCatID_1,
                                 RefGMPGroup.Key.RefGenDrugCatID_2
                                 //▲====: #019
                                 , RefGMPGroup.Key.Administration
                                 , RefGMPGroup.Key.Notes
                                 //▼====: #032
                                 , RefGMPGroup.Key.DispenseVolume
                                 , HIPatientPrice = RefGMPGroup.Max(groupItem => groupItem.HIPatientPrice),
                                 //▲====: #032
                             };
            var RefGenMedProductDetailsListSum = new ObservableCollection<RefGenMedProductDetails>();
            foreach (var Details in ListRefGMP)
            {
                if (Details.Remaining == 0)
                {
                    continue;
                }
                RefGenMedProductDetails item = new RefGenMedProductDetails();
                item.GenMedProductID = Details.GenMedProductID;
                item.BrandName = Details.BrandName;
                item.SelectedUnit = new RefUnit();
                item.SelectedUnit.UnitName = Details.UnitName;
                item.Code = Details.Code;
                item.Remaining = Details.Remaining;
                item.RequestQty = Details.Qty;
                item.ProductCodeRefNum = Details.ProductCodeRefNum;
                item.V_MedProductType = Details.V_MedProductType;
                item.SelectedUnitUse = new RefUnit();
                item.SelectedUnitUse.UnitName = Details.SelectedUnitUse;
                item.V_InstructionOrdinalType = Details.V_InstructionOrdinalType;
                item.MinDayOrdinalContinueIsAllowable = Details.MinDayOrdinalContinueIsAllowable;
                item.GenericID = Details.GenericID;
                item.GenericName = Details.GenericName;
                //▼====: #019
                item.RefGenDrugCatID_1 = Details.RefGenDrugCatID_1;
                item.RefGenDrugCatID_2 = Details.RefGenDrugCatID_2;
                //▲====: #019
                item.Administration = Details.Administration;
                item.Notes = Details.Notes;
                //▼====: #032
                item.HIPatientPrice = Details.HIPatientPrice;
                item.DispenseVolume = Details.DispenseVolume;
                //▲====: #032
                GetRouteOfAdministrationList(item,0, item.GenMedProductID);
                RefGenMedProductDetailsListSum.Add(item);
            }
            return RefGenMedProductDetailsListSum;
        }
        private void AutoGetOrdinal(ReqOutwardDrugClinicDeptPatient item)
        {
            //Tự động đánh số thứ tự cho các thuốc bắt buộc nhập STT
            if (AntibioticTreatmentUsageHistories != null && item.RefGenericDrugDetail != null && item.RefGenericDrugDetail.V_InstructionOrdinalType != (long)AllLookupValues.V_InstructionOrdinalType.Thuong)
            {
                if (!AntibioticTreatmentUsageHistories.Any(x => x.GenMedProductID == item.GenMedProductID.GetValueOrDefault(0) && x.MedicalInstructionDate.HasValue))
                {
                    item.AntibioticOrdinal = 1;
                }
                else
                {
                    var LastAntibioticTreatment = AntibioticTreatmentUsageHistories.OrderByDescending(x => x.MedicalInstructionDate).OrderByDescending(x => x.OutClinicDeptReqID).FirstOrDefault(x => x.GenMedProductID == item.GenMedProductID.Value && x.MedicalInstructionDate.HasValue);
                    var CurrentMedicalInstructionDate = item.MedicalInstructionDate.GetValueOrDefault(Globals.GetCurServerDateTime()).Date;
                    if (LastAntibioticTreatment.MedicalInstructionDate.Value.Date == CurrentMedicalInstructionDate)
                    {
                        item.AntibioticOrdinal = LastAntibioticTreatment.AntibioticOrdinal;
                    }
                    else if ((CurrentMedicalInstructionDate - LastAntibioticTreatment.MedicalInstructionDate.Value.Date).TotalDays == 1
                        || (CurrentMedicalInstructionDate - LastAntibioticTreatment.MedicalInstructionDate.Value.Date).TotalDays <= item.RefGenericDrugDetail.MinDayOrdinalContinueIsAllowable)
                    {
                        item.AntibioticOrdinal = LastAntibioticTreatment.AntibioticOrdinal + 1;
                    }
                    else
                    {
                        item.AntibioticOrdinal = 1;
                    }
                }
            }
            else
            {
                item.AntibioticOrdinal = null;
            }
        }
        private void GetRefGenMedProductDetails_Auto(string BrandName, bool IsCode, int PageIndex, int PageSize
            , AutoCompleteBox cboDrugName = null, ReqOutwardDrugClinicDeptPatient aUpdateContent = null)
        {
            if (IsCode == false && BrandName.Length < 1)
            {
                return;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(false, IsCode, BrandName
                        , SelectedIntravenousStorage == null ? 0 : SelectedIntravenousStorage.StoreID
                        , (long)AllLookupValues.MedProductType.THUOC, null, null, IsCode, null
                        , Globals.ServerConfigSection.MedDeptElements.IntravenousCatID, null, true
                        //▼====: #023
                        , IsCOVID
                        //▲====: #023
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetDrugForSellVisitorAutoComplete_ForRequestDrugDept_V2(asyncResult);
                                var GettedCollection = GroupRemaining(results);
                                if (IsCode && aUpdateContent != null)
                                {
                                    if (GettedCollection != null && GettedCollection.Count > 0)
                                    {
                                        aUpdateContent.RefGenericDrugDetail = GettedCollection.FirstOrDefault();
                                        AutoGetOrdinal(aUpdateContent);
                                        AddIntravenousBlankRow();
                                    }
                                }
                                else if (!IsCode && cboDrugName != null)
                                {
                                    cboDrugName.ItemsSource = GettedCollection;
                                    cboDrugName.PopulateComplete();
                                }
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
        public void ReloadLoadInPatientInstruction(long aIntPtDiagDrInstructionID)
        {
            CallShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstructionByInstructionID(aIntPtDiagDrInstructionID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var mPatientInstruction = contract.EndGetInPatientInstructionByInstructionID(asyncResult);
                                    if (mPatientInstruction != null)
                                    {
                                        CurRegistration.InPatientInstruction = mPatientInstruction;
                                        //▼===== #005
                                        if (CurRegistration.InPatientInstruction.DoctorStaff != null)
                                        {
                                            gSelectedDoctorStaff = CurRegistration.InPatientInstruction.DoctorStaff;
                                            //20191030 TBL: Theo ý anh Tuấn nếu người tạo y lệnh và bác sĩ chỉ định là 1 người thì không cho cập nhật lại bác sĩ chỉ định
                                            if (CurRegistration.InPatientInstruction.Staff != null && CurRegistration.InPatientInstruction.Staff.StaffID != CurRegistration.InPatientInstruction.DoctorStaff.StaffID)
                                            {
                                                IsUpdateDoctorStaff = true;
                                            }
                                            else
                                            {
                                                IsUpdateDoctorStaff = false;
                                            }
                                        }
                                        else
                                        {
                                            gSelectedDoctorStaff = new Staff();
                                        }
                                        //▲===== #005
                                        UploadViewContent(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    CallHideBusyIndicator();
                }
            });
            t.Start();
        }
        private void UploadViewContent(bool aIsUpdate = false)
        {
            if (CurRegistration.InPatientInstruction != null)
            {
                InPatientDailyDiagnosisContent.IntPtDiagDrInstructionID = CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
                InPatientDailyDiagnosisContent.PatientRegistrationContent = CurRegistration;
                //20200413 TBL: BM 0030110: Gọi hàm khác để load chẩn đoán và danh sách ICD10 cùng lúc thay vì load chẩn đoán xong rồi mới load danh sách ICD10
                //InPatientDailyDiagnosisContent.GetLatesDiagTrmtByPtID_InPt(CurRegistration.PatientID.Value, CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY, !aIsUpdate);
                InPatientDailyDiagnosisContent.GetLatestDiagnosisTreatment_InPtByInstructionID_V2(CurRegistration.PatientID.Value, CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID, (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_DAILY, !aIsUpdate);
                MedicalInstructionContent.IntPtDiagDrInstructionID = CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
                LoadIntravenous(CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID, !aIsUpdate);
                if (aIsUpdate)
                {
                    MedicalInstructionContent.LoadAllRegistrationItemsByID(MedicalInstructionContent.IntPtDiagDrInstructionID);

                    this.InPatientDeptDetails = CurRegistration != null & CurRegistration.AdmissionInfo != null && CurRegistration.AdmissionInfo.InPatientDeptDetails != null ? CurRegistration.AdmissionInfo.InPatientDeptDetails.Where(x => x.InPatientDeptDetailID == CurRegistration.InPatientInstruction.InPatientDeptDetailID).ToObservableCollection() : null;
                    SelectedInPatientDeptDetail = this.InPatientDeptDetails.FirstOrDefault();
                    //▼===== #006
                    gAttachLocationCollection = new ObservableCollection<DeptLocation> { SelectedInPatientDeptDetail.DeptLocation };
                    if (CurRegistration.InPatientInstruction != null)
                    {
                        CurRegistration.InPatientInstruction.LocationInDept = gAttachLocationCollection.FirstOrDefault();
                    }
                    //SelectedBedAllocation = CurRegistration != null && CurRegistration.InPatientInstruction != null && BedAllocations != null && BedAllocations.Any(x => x.BedPatientID == CurRegistration.InPatientInstruction.BedPatientID.GetValueOrDefault(0)) ? BedAllocations.Where(x => x.BedPatientID == CurRegistration.InPatientInstruction.BedPatientID.GetValueOrDefault(0)).FirstOrDefault() : null;
                    SelectedBedAllocation = CurRegistration != null && CurRegistration.InPatientInstruction != null && CurRegistration.BedAllocations != null && CurRegistration.BedAllocations.Any(x => x.BedPatientID == CurRegistration.InPatientInstruction.BedPatientID.GetValueOrDefault(0)) ? CurRegistration.BedAllocations.Where(x => x.BedPatientID == CurRegistration.InPatientInstruction.BedPatientID.GetValueOrDefault(0)).FirstOrDefault() : null;
                    //▲===== #006
                    MedicalInstructionDateContent.DateTime = CurRegistration.InPatientInstruction.InstructionDate;
                    gIsUpdate = aIsUpdate;
                }
                else
                {
                    RenewCmd(true);
                }
                MonitoringVitalSignsContent.gInPatientInstruction = CurRegistration.InPatientInstruction;
                //▼====: #017
                InPatientDailyDiagnosisContent.gInPatientInstruction = CurRegistration.InPatientInstruction;
                //▲====: #017
                gIsUpdate = aIsUpdate;
            }
            else
            {
                IntravenousEditView = true;
            }
            GetAntibioticTreatmentUsageHistory();
        }
        private void LoadInPatientInstruction()
        {
            CallShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstruction(CurRegistration, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                InPatientInstruction mPatientInstruction = contract.EndGetInPatientInstruction(asyncResult);
                                CurRegistration.InPatientInstruction = mPatientInstruction == null ? new InPatientInstruction() : mPatientInstruction;
                                //▼===== #005
                                if (CurRegistration.InPatientInstruction.DoctorStaff != null)
                                {
                                    gSelectedDoctorStaff = CurRegistration.InPatientInstruction.DoctorStaff;
                                    //20191030 TBL: Theo ý anh Tuấn nếu người tạo y lệnh và bác sĩ chỉ định là 1 người thì không cho cập nhật lại bác sĩ chỉ định
                                    if (CurRegistration.InPatientInstruction.Staff != null && CurRegistration.InPatientInstruction.Staff.StaffID != CurRegistration.InPatientInstruction.DoctorStaff.StaffID)
                                    {
                                        IsUpdateDoctorStaff = true;
                                    }
                                    else
                                    {
                                        IsUpdateDoctorStaff = false;
                                    }
                                }
                                else
                                {
                                    gSelectedDoctorStaff = new Staff();
                                }
                                //▲===== #005
                                UploadViewContent(CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID > 0);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    CallHideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadIntravenous(long IntPtDiagDrInstructionID, bool IsLoadNew = false)
        {
            CallShowBusyIndicator();
            DeletedIntravenousContentCollection = new List<ReqOutwardDrugClinicDeptPatient>();
            IntravenousContentCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetIntravenousPlan_InPt(IntPtDiagDrInstructionID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    List<Intravenous> mPatientInstruction = contract.EndGetIntravenousPlan_InPt(asyncResult);
                                    if (mPatientInstruction != null)
                                    {
                                        int mIntravenousID = 0;
                                        foreach (Intravenous PatientInstructionItem in mPatientInstruction)
                                        {
                                            if (PatientInstructionItem.IntravenousID > 0)
                                            {
                                                if (IsLoadNew)
                                                {
                                                    PatientInstructionItem.IntravenousID = --mIntravenousID;
                                                }
                                                foreach (var item in PatientInstructionItem.IntravenousDetails)
                                                {
                                                    item.IntravenousPlan_InPtID = PatientInstructionItem.IntravenousID;
                                                    item.IDAndInfusionProcessType = PatientInstructionItem.IntravenousID + string.Format(" : {0} {1}", (PatientInstructionItem.V_InfusionType as Lookup).ObjectValue, PatientInstructionItem.V_InfusionProcessType != null ? (PatientInstructionItem.V_InfusionProcessType as Lookup).ObjectValue : "");
                                                    if (IsLoadNew)
                                                    {
                                                        item.OutClinicDeptReqID = 0;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (var item in PatientInstructionItem.IntravenousDetails)
                                                {
                                                    item.IntravenousPlan_InPtID = 0;
                                                    item.IDAndInfusionProcessType = item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC ? eHCMSResources.G0787_G1_Thuoc : (item.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU ? eHCMSResources.G2907_G1_YCu : eHCMSResources.T1616_G1_HC);
                                                    if (IsLoadNew)
                                                    {
                                                        item.OutClinicDeptReqID = 0;
                                                    }
                                                }
                                            }
                                        }
                                        //▼===== 20191005:  Sửa điều kiện lại để blank row được tạo ra khi bệnh nhân chưa có thông tin gì về thuốc/ dịch truyền.
                                        //                  Vì phải luôn luôn hiển thị để tạo dịch truyền, do chưa có status cho y lệnh nên để alway visible. Sau này
                                        //                  Khi đã có status cho y lệnh thì dựa vào status mà set blank row lên hay không.
                                        if (mPatientInstruction != null)
                                        {
                                            IntravenousContentCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                                            if (mPatientInstruction.Any(x => x.IntravenousID > 0))
                                            {
                                                foreach (var aItem in mPatientInstruction.Where(x => x.IntravenousID > 0).OrderBy(x => x.IntravenousID))
                                                {
                                                    foreach (var aChildItem in aItem.IntravenousDetails)
                                                    {
                                                        if (aChildItem == aItem.IntravenousDetails.Last())
                                                        {
                                                            aChildItem.FlowRate = aItem.FlowRate;
                                                            aChildItem.StartDateTime = aItem.StartDateTime;
                                                            aChildItem.NumOfTimes = aItem.NumOfTimes;
                                                        }
                                                        IntravenousContentCollection.Add(aChildItem);
                                                    }
                                                }
                                                RecalIntravenousGroupID();
                                                IntravenousView.Refresh();
                                                IntravenousEditView = false;
                                            }
                                            else
                                            {
                                                IntravenousEditView = true;
                                            }
                                            AddIntravenousBlankRow();
                                        }
                                        else
                                        {
                                            IntravenousEditView = true;
                                        }
                                        //▲=====
                                        IntravenousContent.IntravenousList = new ObservableCollection<Intravenous>(mPatientInstruction.Any(x => x.IntravenousID == 0) ? mPatientInstruction.Where(x => x.IntravenousID == 0) : new ObservableCollection<Intravenous>());
                                        IntravenousContent.ReloadIntravenousList();
                                        CurrentDrugCollection = mPatientInstruction.Any(x => x.IntravenousID == 0) ? mPatientInstruction.Where(x => x.IntravenousID == 0).SelectMany(x => x.IntravenousDetails).Where(x => x.RefGenericDrugDetail != null && x.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC).ToObservableCollection() : new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                                        foreach (var Item in CurrentDrugCollection)
                                        {
                                            Item.BackupCurrentHash();
                                            GetRouteOfAdministrationList(Item.RefGenericDrugDetail, 0, Item.RefGenericDrugDetail.GenMedProductID);
                                        }
                                        CurrentDrugCollection.Add(InitNullDrugItem());

                                        // VuTTM
                                        // For nutrition
                                        CurrentNutritionCollection = mPatientInstruction
                                            .Any(x => x.IntravenousID == 0) ? mPatientInstruction
                                            .Where(x => x.IntravenousID == 0)
                                            .SelectMany(x => x.IntravenousDetails)
                                            .Where(x => x.RefGenericDrugDetail != null
                                                && x.RefGenericDrugDetail.V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                                            .ToObservableCollection() : new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                                        foreach (var item in CurrentNutritionCollection)
                                        {
                                            item.BackupCurrentHash();
                                        }
                                        CurrentNutritionCollection.Add(InitNullDrugItem());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
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
                    MessageBox.Show(ex.Message);
                    CallHideBusyIndicator();
                }
            });
            t.Start();
        }
        //==== #002
        private const Int16 PulseAndBloodPressure = 1;
        private const Int16 Temperature = 2;
        private const Int16 BloodSugar = 4;
        private const Int16 ECG = 8;
        private const Int16 Diet = 16;
        private const Int16 SpO2 = 32;
        private const Int16 Sense = 64;
        private const Int16 Urine = 128;
        private const Int16 PhysicalExamOther = 256;
        private const Int16 LevelCare = 512;
        //▼==== #026
        private const Int16 RespiratoryRate = 1;
        //▲==== #026
        private void CheckMonitoringVitalSigns(InPatientInstruction inPatientInstruction, out string error)
        {
            error = "";
            //TBL: Tuy theo cau hinh se khong kiem tra truong nao
            //TBL: Khong kiem tra Mach, huyet ap khi cau hinh = 1
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & PulseAndBloodPressure) == 0) && string.IsNullOrEmpty(inPatientInstruction.PulseAndBloodPressure))
            {
                error += eHCMSResources.Z2851_G1_ChuaNhapMachHA + Environment.NewLine;
            }
            //▼==== #026
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & RespiratoryRate) == 0) && string.IsNullOrEmpty(inPatientInstruction.RespiratoryRate))
            {
                error += string.Format(eHCMSResources.Z3252_G1_ChuaNhap, eHCMSResources.N0237_G1_NhipTho) + Environment.NewLine;
            }
            //▲==== #026
            //TBL: Khong kiem tra Nhiet do khi cau hinh = 2
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & Temperature) < 2) && string.IsNullOrEmpty(inPatientInstruction.Temperature))
            {
                error += eHCMSResources.Z2865_G1_ChuaNhapNhietDo + Environment.NewLine;
            }
            //TBL: Khong kiem tra Duong huyet khi cau hinh = 4
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & BloodSugar) < 4) && string.IsNullOrEmpty(inPatientInstruction.BloodSugar))
            {
                error += eHCMSResources.Z2866_G1_ChuaNhapDuongHuyet + Environment.NewLine;
            }
            //TBL: Khong kiem tra ECG cau hinh = 8
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & ECG) < 8) && string.IsNullOrEmpty(inPatientInstruction.ECG))
            {
                error += eHCMSResources.Z2867_G1_ChuaNhapECG + Environment.NewLine;
            }
            //TBL: Khong kiem tra Che do an khi cau hinh = 16
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & Diet) < 16) && string.IsNullOrEmpty(inPatientInstruction.Diet))
            {
                error += eHCMSResources.Z2852_G1_ChuaNhapCheDoAn + Environment.NewLine;
            }
            //TBL: Khong kiem tra SpO2 cau hinh = 32
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & SpO2) < 32) && string.IsNullOrEmpty(inPatientInstruction.SpO2))
            {
                error += eHCMSResources.Z2868_G1_ChuaNhapSpO2 + Environment.NewLine;
            }
            //TBL: Khong kiem tra Tri giac khi cau hinh = 64
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & Sense) < 64) && string.IsNullOrEmpty(inPatientInstruction.Sense))
            {
                error += eHCMSResources.Z2869_G1_ChuaNhapTriGiac + Environment.NewLine;
            }
            //TBL: Khong kiem tra Nuoc tieu khi cau hinh = 128
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & Urine) < 128) && string.IsNullOrEmpty(inPatientInstruction.Urine))
            {
                error += eHCMSResources.Z2870_G1_ChuaNhapNuocTieu + Environment.NewLine;
            }
            //TBL: Khong kiem tra Khac khi cau hinh = 256
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & PhysicalExamOther) < 256) && string.IsNullOrEmpty(inPatientInstruction.PhysicalExamOther))
            {
                error += eHCMSResources.Z2871_G1_ChuaNhapKhac + Environment.NewLine;
            }
            //TBL: Khong kiem tra Che do cham soc khi cau hinh = 512
            if (((Globals.ServerConfigSection.ConsultationElements.CheckMonitoringVitalSigns & LevelCare) < 512) && (inPatientInstruction.V_LevelCare == null || inPatientInstruction.V_LevelCare.LookupID <= 0))
            {
                error += eHCMSResources.Z2853_G1_ChuaNhapCheDoChamSoc + Environment.NewLine;
            }
        }
        private IEnumerator<IResult> DoGetIntravenousStorageCollection()
        {
            var LoadStorageTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, true, false);
            yield return LoadStorageTask;
            IntravenousStorageCollection = LoadStorageTask
                .LookupList
                .Where(x => x.ListV_MedProductType != null
                        && x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.THUOC).ToString())
                        && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT
                        //▼====: #025
                        && (((!x.IsMain || (x.IsMain && x.IsSubStorage)) && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
                        || !Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage))
                        //▲====: #025
                .ToObservableCollection();
            SelectedIntravenousStorage = IntravenousStorageCollection
                .OrderByDescending(x => x.IsSubStorage)
                .FirstOrDefault();

            // VuTTM
            // Get the nutrition storage list
            InNutritionStorageCollection = LoadStorageTask
                .LookupList
                .Where(x => x.ListV_MedProductType != null
                        && x.ListV_MedProductType.Contains(((long)AllLookupValues.MedProductType.NUTRITION).ToString())
                        && x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT
                        //▼====: #025
                        && (((!x.IsMain || (x.IsMain && x.IsSubStorage)) && Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage)
                        || !Globals.ServerConfigSection.MedDeptElements.IsEnableMedSubStorage))
                        //▲====: #025
                .ToObservableCollection();
            SelectedInNutritionStorage = InNutritionStorageCollection
                .OrderByDescending(x => x.IsSubStorage)
                .FirstOrDefault();
            yield break;
        }
        private void CallShowBusyIndicator()
        {
            if (this.IsDialogView)
            {
                this.DlgShowBusyIndicator();
            }
            else
            {
                this.ShowBusyIndicator();
            }
        }
        private void CallHideBusyIndicator()
        {
            if (this.IsDialogView)
            {
                this.DlgHideBusyIndicator();
            }
            else
            {
                this.HideBusyIndicator();
            }
        }
        private void EditAntibioticTreatment(AntibioticTreatment CurrentAntibioticTreatment)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditAntibioticTreatment(CurrentAntibioticTreatment, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndEditAntibioticTreatment(asyncResult);
                                GetAntibioticTreatmentsByPtRegID();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
            CurrentThread.Start();
        }
        private void GetAntibioticTreatmentsByPtRegID()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID == 0)
            {
                var AntibioticTreatmentCollection = new ObservableCollection<AntibioticTreatment>();
                AntibioticTreatmentCollection.Insert(0, new AntibioticTreatment { AntibioticTreatmentID = -1, AntibioticTreatmentTitle = string.Empty });
                IntravenousContent.AntibioticTreatmentCollection = AntibioticTreatmentCollection;
                return;
            }
            CallShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAntibioticTreatmentsByPtRegID(CurRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var GettedCollection = contract.EndGetAntibioticTreatmentsByPtRegID(asyncResult);
                                var AntibioticTreatmentCollection = new ObservableCollection<AntibioticTreatment>();
                                if (GettedCollection != null && GettedCollection.Count > 0)
                                {
                                    AntibioticTreatmentCollection = GettedCollection.OrderByDescending(x => x.AntibioticTreatmentID).ToObservableCollection();
                                }
                                AntibioticTreatmentCollection.Insert(0, new AntibioticTreatment { AntibioticTreatmentID = -1, AntibioticTreatmentTitle = string.Empty, V_AntibioticTreatmentType = (long)AllLookupValues.V_AntibioticTreatmentType.Instruction });
                                IntravenousContent.AntibioticTreatmentCollection = AntibioticTreatmentCollection.Where(x => x.V_AntibioticTreatmentType == (long)AllLookupValues.V_AntibioticTreatmentType.Instruction).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    CallHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        private void GetAntibioticTreatmentUsageHistory()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID == 0)
            {
                AntibioticTreatmentUsageHistories = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                return;
            }
            CallShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new CommonService_V2Client())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetAntibioticTreatmentUsageHistory(CurRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ItemCollection = CurrentContract.EndGetAntibioticTreatmentUsageHistory(asyncResult);
                                if (ItemCollection != null && ItemCollection.Count > 0)
                                {
                                    AntibioticTreatmentUsageHistories = ItemCollection.ToObservableCollection();
                                }
                                else
                                {
                                    AntibioticTreatmentUsageHistories = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                                }
                            }
                            catch (Exception ex)
                            {
                                AntibioticTreatmentUsageHistories = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                                MessageBox.Show(ex.Message);
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
                    AntibioticTreatmentUsageHistories = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                    MessageBox.Show(ex.Message);
                    CallHideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        private void AddBlankRowIntoCurrentNutritionCollection()
        {
            if (CurrentNutritionCollection == null
                || CurrentNutritionCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) == 0))
            {
                return;
            }
            CurrentNutritionCollection.Add(InitNullDrugItem());
        }

        private void AddBlankRowIntoCurrentDrugCollection()
        {
            if (CurrentDrugCollection == null || CurrentDrugCollection.Any(x => x.GenMedProductID.GetValueOrDefault(0) == 0))
            {
                return;
            }
            CurrentDrugCollection.Add(InitNullDrugItem());
        }
        private void ClearDrugDataRowContent(ReqOutwardDrugClinicDeptPatient CurrentContext)
        {
            if (CurrentContext != null && CurrentContext.GenMedProductID.GetValueOrDefault(0) > 0)
            {
                CurrentContext.MDoseStr = "0";
                CurrentContext.MDose = 0;
                CurrentContext.ADoseStr = "0";
                CurrentContext.ADose = 0;
                CurrentContext.EDoseStr = "0";
                CurrentContext.EDose = 0;
                CurrentContext.NDoseStr = "0";
                CurrentContext.NDose = 0;
                CurrentContext.PrescribedQty = 0;
                //CurrentContext.RefGenericDrugDetail.Administration = "";
                //CurrentContext.real
                CurrentContext.V_RouteOfAdministration = AllRouteOfAdministration == null? 0: AllRouteOfAdministration.FirstOrDefault().LookupID;
            }
        }
        private bool Check1ThuocBiDiUng(string DrugName)
        {
            if (PatientMDAllergyCollection == null)
            {
                return false;
            }
            foreach (var Item in PatientMDAllergyCollection)
            {
                if (DrugName.ToLower().Trim() == Item.AllergiesItems.ToLower().Trim())
                {
                    return true;
                }
            }
            return false;
        }
        private float ChangeDoseStringToFloat(string value)
        {
            float result = 0;
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("/"))
                    {
                        string pattern = @"\b[\d]+/[\d]+\b";
                        if (!Regex.IsMatch(value, pattern))
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                        else
                        {
                            string[] items = null;
                            items = value.Split('/');
                            if (items.Count() > 2 || items.Count() == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                                return 0;
                            }
                            else if (float.Parse(items[1]) == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                                return 0;
                            }

                            //KMx: Không được Round số lượng. Nếu không sẽ bị sai trong trường hợp thuốc 1/7 viên * 35 ngày.
                            //Kết quả không Round là 5, kết quả sau khi Round là 6.
                            //result = (float)Math.Round((float.Parse(items[0]) / float.Parse(items[1])), 3);

                            result = (float.Parse(items[0]) / float.Parse(items[1]));

                            if (result < 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                                return 0;
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            result = float.Parse(value);
                            if (result < 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                                return 0;
                            }
                        }
                        catch
                        {
                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                            return 0;
                        }
                    }
                }
            }
            catch
            {
                Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
                return 0;
            }
            return result;
        }
        private decimal CalQtyForNormalDrug(ReqOutwardDrugClinicDeptPatient drugItem, int nNumDayPrescribed)
        {
            if (drugItem == null || drugItem.RefGenericDrugDetail == null)
            {
                return 0;
            }
            float QtyAllDose = 0;
            float Result = 0;
            QtyAllDose = drugItem.MDose + drugItem.ADose + drugItem.NDose + drugItem.EDose;
            //KMx: Phải nhân trước rồi chia sau để hạn chế kết quả có số lẻ (06/11/2014 11:11).
            Result = (QtyAllDose * nNumDayPrescribed) / ((float)drugItem.RefGenericDrugDetail.DispenseVolume == 0 ? 1 : (float)drugItem.RefGenericDrugDetail.DispenseVolume);
            //▼====: #03
            //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
            return (decimal)Math.Ceiling(Math.Round(Result, 2));
            //▲====: #033
        }
        private void ChangeAnyDoseQty(int nDoseType, string strDoseQty, ReqOutwardDrugClinicDeptPatient presDetailObj)
        {
            if (presDetailObj == null || presDetailObj.RefGenericDrugDetail == null)
            {
                MessageBox.Show(eHCMSResources.A0545_G1_Msg_InfoEnglish3);
                return;
            }
            float fDoseQty = 0;
            if (strDoseQty != null && strDoseQty.Length > 0)
            {
                fDoseQty = ChangeDoseStringToFloat(strDoseQty);
            }
            switch (nDoseType)
            {
                case 1:
                    presDetailObj.MDose = fDoseQty;
                    break;
                case 2:
                    presDetailObj.NDose = fDoseQty;
                    break;
                case 3:
                    presDetailObj.ADose = fDoseQty;
                    break;
                case 4:
                    presDetailObj.EDose = fDoseQty;
                    break;

            }
            presDetailObj.PrescribedQty = CalQtyForNormalDrug(presDetailObj, 1);
        }
        public string AddMoreZeroAndChangeCommaToDot(string DoseQtyStr)
        {
            if (string.IsNullOrEmpty(DoseQtyStr))
            {
                return "0";
            }
            if (DoseQtyStr.Contains(","))
            {
                DoseQtyStr = DoseQtyStr.Replace(",", ".");
            }
            if (DoseQtyStr.EndsWith("."))
            {
                DoseQtyStr = DoseQtyStr.TrimEnd('.');
                if (string.IsNullOrEmpty(DoseQtyStr))
                {
                    return "0";
                }
            }
            if (DoseQtyStr.StartsWith("."))
            {
                DoseQtyStr = "0" + DoseQtyStr;
            }
            return DoseQtyStr;
        }
        private ReqOutwardDrugClinicDeptPatient InitNullDrugItem()
        {
            ReqOutwardDrugClinicDeptPatient NewDrugItem = new ReqOutwardDrugClinicDeptPatient();
            NewDrugItem.DateTimeSelection = Globals.GetCurServerDateTime();
            NewDrugItem.EntityState = EntityState.NEW;
            NewDrugItem.PrescribedQty = NewDrugItem.ReqQty;
            NewDrugItem.RefGenericDrugDetail = new RefGenMedProductDetails();
            NewDrugItem.MDoseStr = "0";
            NewDrugItem.ADoseStr = "0";
            NewDrugItem.EDoseStr = "0";
            NewDrugItem.NDoseStr = "0";
            NewDrugItem.MDose = 0;
            NewDrugItem.ADose = 0;
            NewDrugItem.EDose = 0;
            NewDrugItem.NDose = 0;
            NewDrugItem.ReqQty = 0;
            return NewDrugItem;
        }
        private void GetPatientMDAllergyCollection(long PatientID, int flag)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new SummaryServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginMDAllergies_ByPatientID(PatientID, flag, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PatientMDAllergyCollection = CurrentContract.EndMDAllergies_ByPatientID(asyncResult).ToObservableCollection();
                            //string str = "";
                            //if (results != null)
                            //{
                            //    foreach (MDAllergy p in results)
                            //    {
                            //        str += p.AllergiesItems.Trim() + ";";
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(str))
                            //{
                            //    str = str.Substring(0, str.Length - 1);
                            //}
                            //Globals.Allergies = str;
                            //curPhysicalExamination = Globals.curPhysicalExamination;
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            CurrentThread.Start();
        }
        private void GetPatientMedicalCondition(long PatientID, int TypeID)
        {
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                using (var CurrentFactory = new ComRecordsServiceClient())
                {
                    var CurrentContract = CurrentFactory.ServiceInstance;
                    CurrentContract.BeginGetMedConditionByPtID(PatientID, TypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            PatientMedicalConditionCollection = CurrentContract.EndGetMedConditionByPtID(asyncResult).ToObservableCollection();
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
            CurrentThread.Start();
        }
        private void GetDrugsInTreatmentRegimen(GenericCoRoutineTask aGenTask, object aICD10Collection)
        {
            ObservableCollection<DiagnosisIcd10Items> ICD10Collection = aICD10Collection as ObservableCollection<DiagnosisIcd10Items>;
            if (ICD10Collection == null || ICD10Collection.Count == 0 ||
                !ICD10Collection.Any(x => !string.IsNullOrEmpty(x.ICD10Code)))
            {
                DrugsInTreatmentRegimen = null;
                aGenTask.ActionComplete(true);
                return;
            }
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new ePrescriptionsServiceClient())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetDrugsInTreatmentRegimen(0, ICD10Collection.Where(x => !string.IsNullOrEmpty(x.ICD10Code)).Select(x => x.ICD10Code).ToList(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DrugsInTreatmentRegimen = CurrentContract.EndGetDrugsInTreatmentRegimen(asyncResult).ToObservableCollection();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            finally
                            {
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    aGenTask.ActionComplete(true);
                }
            });
            CurrentThread.Start();
        }
        private bool CheckQtyLessThanQtyAutoCalc(ReqOutwardDrugClinicDeptPatient Objtmp, int NDay)
        {
            if (Objtmp != null)
            {
                float TongThuoc = 0;
                decimal Tong = 0;
                if (Objtmp != null)
                {
                    TongThuoc = Objtmp.MDose + Objtmp.ADose + Objtmp.NDose + Objtmp.EDose;
                    Tong = (decimal)(TongThuoc * NDay * Objtmp.RefGenericDrugDetail.Volume.GetValueOrDefault(1)) / (decimal)Objtmp.RefGenericDrugDetail.DispenseVolume;
                    if (Tong - Objtmp.PrescribedQty > 0.0001m)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool CheckThuocHopLe(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection,
            bool checkDose = true)
        {
            StringBuilder CurrentStringBuilder = new StringBuilder();
            bool Result = true;
            if (ItemCollection.Count > 1)
            {
                foreach (ReqOutwardDrugClinicDeptPatient item in ItemCollection)
                {
                    if (item == ItemCollection.Last())
                    {
                        continue;
                    }
                    if (item.RefGenericDrugDetail == null
                        || item.RefGenericDrugDetail == null
                        || item.RefGenericDrugDetail.BrandName == null
                        || item.RefGenericDrugDetail.BrandName == "")
                    {
                        CurrentStringBuilder.AppendLine(string.Format(eHCMSResources.Z0908_G1_ThuocDong0KgHopLe, (ItemCollection.IndexOf(item) + 1).ToString()));
                        Result = false;
                        continue;
                    }
                    if (item.RefGenericDrugDetail == null || item.RefGenericDrugDetail.BrandName == null)
                    {
                        continue;
                    }
                    if (item.PrescribedQty <= 0)
                    {
                        CurrentStringBuilder.AppendLine(string.Format(eHCMSResources.Z1057_G1_ThuocSLgLonHon0, item.RefGenericDrugDetail.BrandName.Trim()));
                        Result = false;
                    }
                    //Thuốc thường.
                    if (item.MDose == 0 &&
                        item.ADose == 0 &&
                        item.EDose == 0 &&
                        item.NDose == 0 &&
                        checkDose)
                    {
                        CurrentStringBuilder.AppendLine(string.Format(eHCMSResources.Z0910_G1_Thuoc0SangTruaChieuToi, item.RefGenericDrugDetail.BrandName.Trim()));
                        Result = false;
                    }
                    if (CheckQtyLessThanQtyAutoCalc(item, 1))
                    {
                        CurrentStringBuilder.AppendLine(string.Format(eHCMSResources.Z0914_G1_Thuoc0CanKTraLai, item.RefGenericDrugDetail.BrandName.Trim()));
                        Result = false;
                    }
                }
                if (Result == false)
                {
                    MessageBox.Show(CurrentStringBuilder.ToString() + Environment.NewLine + eHCMSResources.I0945_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }
            }
            return true;
        }
        private string CheckAllThuocBiDiUng(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock)
        {
            StringBuilder CurrentStringBuilder = new StringBuilder();
            bool IsError = false;
            if (ItemCollection.Count > 1)
            {
                foreach (ReqOutwardDrugClinicDeptPatient Item in ItemCollection)
                {
                    if (Item.GenMedProductID.GetValueOrDefault(0) == 0)
                    {
                        continue;
                    }
                    if (Item.RefGenericDrugDetail != null && !string.IsNullOrEmpty(Item.RefGenericDrugDetail.BrandName))
                    {
                        if (Check1ThuocBiDiUng(Item.RefGenericDrugDetail.BrandName.Trim()))
                        {
                            CurrentStringBuilder.AppendLine("-" + Item.RefGenericDrugDetail.BrandName.Trim());
                            IsError = true;
                        }
                    }
                }
            }
            if (IsError)
            {
                bBlock = true;
                return (("{0}:", eHCMSResources.A0504_G1_Msg_InfoDSThuocDiUng) + Environment.NewLine + CurrentStringBuilder.ToString());
            }
            return "";
        }
        private int CountDrug(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, long? GenMedProductID, out string BrandName)
        {
            BrandName = "";
            int CountedQty = 0;
            if (GenMedProductID != null && GenMedProductID > 0)
            {
                foreach (var Item in ItemCollection)
                {
                    if (Item.GenMedProductID == GenMedProductID)
                    {
                        CountedQty++;
                    }
                    if (CountedQty == 2) //TBL: Nếu có đếm được 2 lần thuốc trùng nhau thì out BrandName ra để hiển thị
                    {
                        BrandName = " - " + Item.RefGenericDrugDetail.BrandName;
                        break;
                    }
                }
            }
            return CountedQty;
        }
        private string CheckThuocBiTrungTrongToa(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock)
        {
            StringBuilder CurrentStringBuilder = new StringBuilder();
            foreach (var Item in ItemCollection)
            {
                string DrugName = "";
                if (CountDrug(ItemCollection, Item.GenMedProductID, out DrugName) >= 2)
                {
                    if (!CurrentStringBuilder.ToString().Contains(DrugName)) //TBL: Kiểm tra tên thuốc bị trùng đã có trong thông báo chưa, nếu có rồi thì bỏ qua để tránh bị trùng lắp khi thông báo
                    {
                        CurrentStringBuilder.AppendLine(DrugName);
                    }
                }
            }
            if (!string.IsNullOrEmpty(CurrentStringBuilder.ToString()))
            {
                bBlock = true;
                return eHCMSResources.K0072_G1_TrungThuocTrongToa + ":" + Environment.NewLine + CurrentStringBuilder.ToString();
            }
            else
            {
                return "";
            }
        }
        public void GetAllDrugsContrainIndicator()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllDrugsContrainIndicator(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllDrugsContrainIndicator(asyncResult);
                            if (results != null)
                            {
                                if (allMedProductContraIndicatorRelToMedCond == null)
                                {
                                    allMedProductContraIndicatorRelToMedCond = new ObservableCollection<DrugAndConTra>();
                                }
                                else
                                {
                                    allMedProductContraIndicatorRelToMedCond.Clear();
                                }
                                foreach (var p in results)
                                {
                                    allMedProductContraIndicatorRelToMedCond.Add(p);
                                }
                                NotifyOfPropertyChange(() => allMedProductContraIndicatorRelToMedCond);
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

                    }), null);

                }

            });

            t.Start();
        }
        private bool CheckChongChiDinh1Drug(long DrugID, ObservableCollection<DiagnosisIcd10Items> LstICD10Item, out string msg, out bool IsWarning)
        {
            double Age = Registration_DataStorage.CurrentPatientRegistration.Patient.Age.GetValueOrDefault();
            //20190923 TBL: Trường hợp bệnh nhân chỉ có tháng không có tuổi thì lấy tháng / 12 để ra được số tuổi
            if (Age == 0 && Registration_DataStorage.CurrentPatientRegistration.Patient.MonthsOld > 0)
            {
                Age = Convert.ToDouble(Registration_DataStorage.CurrentPatientRegistration.Patient.MonthsOld) / 12;
            }
            //double AgeMonth = Convert.ToDouble((Globals.GetCurServerDateTime().Year - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Year) * 12 + (Globals.GetCurServerDateTime().Month - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Month));
            //double AgeDay = Convert.ToDouble((Globals.GetCurServerDateTime() - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB).Value.TotalDays);
            msg = "";
            string msgV_AgeUnit = "";
            IsWarning = true;
            if (allMedProductContraIndicatorRelToMedCond == null)
            {
                return false;
            }
            foreach (var DCR in allMedProductContraIndicatorRelToMedCond)
            {
                if (DrugID == DCR.DrugID)
                {
                    foreach (var LCT in DCR.ListConTraAndLstICDs)
                    {
                        switch (LCT.V_AgeUnit)
                        {
                            case (long)AllLookupValues.V_AgeUnit.Thang:
                                Age = Convert.ToDouble((Globals.GetCurServerDateTime().Year - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Year) * 12
                                    + (Globals.GetCurServerDateTime().Month - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB.Value.Month));
                                msgV_AgeUnit = " tháng ";
                                break;
                            case (long)AllLookupValues.V_AgeUnit.Ngay:
                                Age = Convert.ToDouble((Globals.GetCurServerDateTime() - Registration_DataStorage.CurrentPatientRegistration.Patient.DOB).Value.TotalDays);
                                msgV_AgeUnit = " ngày ";
                                break;
                        }
                        //CCD theo tuoi
                        if (LCT.ListICD10Code == null || LCT.ListICD10Code.Count == 0)
                        {
                            //Chong chi dinh theo do tuoi tu ... den...
                            if (LCT.AgeFrom <= Age && LCT.AgeTo >= Age && LCT.AgeFrom != 0 && LCT.AgeTo != 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2629_G1_Thuoc0CCDVoiTuoiTu1Den2, DCR.BrandName.Trim(), LCT.AgeFrom.ToString() + msgV_AgeUnit, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo do tuoi tren ...
                            else if (LCT.AgeFrom < Age && LCT.AgeFrom != 0 && LCT.AgeTo == 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2632_G1_Thuoc0CCDVoiTuoiTren1, DCR.BrandName.Trim(), LCT.AgeFrom.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo do tuoi duoi ...
                            else if (LCT.AgeTo > Age && LCT.AgeTo != 0 && LCT.AgeFrom == 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2631_G1_Thuoc0CCDVoiTuoiDuoi1, DCR.BrandName.Trim(), LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                        }
                        //CCD theo ICD + tuoi
                        else
                        {
                            long indexICD = 0;
                            string ICDs = "";
                            List<string> ListICD = new List<string>();
                            foreach (string LstICD in LCT.ListICD10Code)
                            {
                                foreach (DiagnosisIcd10Items ICD10 in LstICD10Item)
                                {
                                    if (LstICD == ICD10.ICD10Code)
                                    {
                                        ListICD.Add(LstICD);
                                    }
                                }
                            }
                            foreach (string item in ListICD)
                            {
                                if (indexICD < ListICD.Count - 1)
                                {
                                    ICDs = ICDs + item + ", ";
                                }
                                else
                                {
                                    ICDs += item;
                                }
                                indexICD++;
                            }
                            //Chong chi dinh theo ICD10
                            if (ListICD.Count > 0 && (LCT.AgeFrom == 0 && LCT.AgeTo == 0))
                            {
                                msg += string.Format("- " + eHCMSResources.Z1498_G1_Thuoc0CCDVoiDKienBenh1, DCR.BrandName.Trim(), ICDs, LCT.AgeFrom.ToString() + msgV_AgeUnit, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo ICD10 va do tuoi tu ... den ...
                            else if (ListICD.Count > 0 && LCT.AgeFrom <= Age && LCT.AgeTo >= Age && LCT.AgeFrom != 0 && LCT.AgeTo != 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2630_G1_Thuoc0CCDVoiDKienBenh1VaDoTuoiTu2Den3, DCR.BrandName.Trim(), ICDs, LCT.AgeFrom.ToString() + msgV_AgeUnit, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo ICD10 va do tuoi tren ...
                            else if (ListICD.Count > 0 && LCT.AgeFrom < Age && LCT.AgeTo == 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2660_G1_Thuoc0CCDVoiDKienBenh1VaDoTuoiTren2, DCR.BrandName.Trim(), ICDs, LCT.AgeFrom.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                            //Chong chi dinh theo ICD10 va do tuoi duoi ...
                            else if (ListICD.Count > 0 && LCT.AgeTo > Age && LCT.AgeFrom == 0)
                            {
                                msg += string.Format("- " + eHCMSResources.Z2661_G1_Thuoc0CCDVoiDKienBenh1VaDoTuoiDuoi2, DCR.BrandName.Trim(), ICDs, LCT.AgeTo.ToString() + msgV_AgeUnit);
                                IsWarning = LCT.IsWarning;
                                if (!IsWarning)
                                {
                                    msg += " (*)" + Environment.NewLine;
                                }
                                else
                                {
                                    msg += Environment.NewLine;
                                }
                            }
                        }



                    }
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                return true;
            }
            return false;
        }
        private string ErrCheckChongChiDinh(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock, ObservableCollection<DiagnosisIcd10Items> ICD10List)
        {
            //TBL: Neu bat cau hinh thi moi kiem tra chong chi dinh
            if (!Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicator)
            {
                return "";
            }
            StringBuilder CurrentStringBuilder = new StringBuilder();
            if (ItemCollection.Count > 1)
            {
                foreach (var Item in ItemCollection)
                {
                    if (Item.RefGenericDrugDetail != null)
                    {
                        string err = "";
                        bool IsWarning = true;
                        if (CheckChongChiDinh1Drug(Item.GenMedProductID.Value, ICD10List, out err, out IsWarning))
                        {
                            CurrentStringBuilder.Append(err);
                            //Item.IsContraIndicator = true;
                        }
                        else
                        {
                            //Item.IsContraIndicator = false;
                        }
                        if (!IsWarning)
                        {
                            bBlock = true;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(CurrentStringBuilder.ToString()))
            {
                return eHCMSResources.Z2927_G1_ToaCoCCD + ":" + Environment.NewLine + CurrentStringBuilder.ToString();
            }
            return "";
        }
        private string ErrCheckChongChiDinh_TrongNgay(IList<ReqOutwardDrugClinicDeptPatient> AllOrtherItemCollection, ref bool bBlock, ObservableCollection<DiagnosisIcd10Items> ICD10List)
        {
            //TBL: Neu bat cau hinh thi moi kiem tra chong chi dinh
            if (!Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicatorInDay)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            if (AllOrtherItemCollection.Count > 1)
            {
                foreach (var item in AllOrtherItemCollection)
                {
                    if (item.RefGenericDrugDetail != null)
                    {
                        string err = "";
                        bool IsWarning = true;
                        if (CheckChongChiDinh1Drug(item.GenMedProductID.Value, ICD10List.ToObservableCollection(), out err, out IsWarning))
                        {
                            sb.Append(err);
                            //item.IsContraIndicator = true;
                        }
                        else
                        {
                            //item.IsContraIndicator = false;
                        }
                        if (!IsWarning)
                        {
                            bBlock = true;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2928_G1_ToaCoCCDTrongNgay + ":" + Environment.NewLine + sb.ToString();
            }
            return "";
        }
        private string CheckPhacDo(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock)
        {
            string warning = "";
            //LatestePrecriptions.IsOutsideRegimen = false;
            //TBL: Hien tai dang test phac do nen them dieu kien kiem tra DrugsInTreatmentRegimen
            if (DrugsInTreatmentRegimen != null)
            {
                //20190603 TBL: Luu lan dau toa thuoc co thuoc ngoai phac do nhung khong thay thong bao
                if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && !bBlock && DrugsInTreatmentRegimen.Count > 0 && ItemCollection.Any(pd => pd.GenMedProductID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(tr => tr.DrugID == pd.GenMedProductID)))
                {
                    //LatestePrecriptions.IsOutsideRegimen = true;
                    warning += eHCMSResources.Z2263_G1_ToaChuaThuocNgoaiPD + ":" + Environment.NewLine;
                }
                //▼===== #026
                if (!string.IsNullOrEmpty(warning))
                {
                    foreach (var item in ItemCollection)
                    {
                        if (item.GenMedProductID.GetValueOrDefault() > 0 && !DrugsInTreatmentRegimen.Any(tr => tr.DrugID == item.GenMedProductID))
                        {
                            warning += "- " + item.RefGenericDrugDetail.BrandName + Environment.NewLine;
                        }
                    }
                }
                //▲===== #026
            }
            if (!string.IsNullOrEmpty(warning))
            {
                return warning;
            }
            return "";
        }
        private List<RefGenericRelation> GetRefGenericRelation(long GenericID)
        {
            List<RefGenericRelation> ListRefGenericRelation = new List<RefGenericRelation>();
            if (Globals.MAPRefGenericRelation != null && Globals.MAPRefGenericRelation.Count > 0 && Globals.MAPRefGenericRelation.ContainsKey(GenericID))
            {
                ListRefGenericRelation = Globals.MAPRefGenericRelation[GenericID];
            }
            return ListRefGenericRelation;
        }
        private string KiemTraTuongTacHoatChatTrongNgay(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock
            , IList<ReqOutwardDrugClinicDeptPatient> AllOrtherItemCollection)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.KiemTraQuanHeHoatChat || bBlock)
            {
                return "";
            }
            //Tạo diction các hoạt chất với các giá trị và true or false cho toa thuốc đang chỉnh sửa và tên hoạt chất
            Dictionary<long, object[]> GenericCollection = new Dictionary<long, object[]>();
            //Thêm các hoạt chất từ toa thuốc đang chỉnh sửa vào diction
            foreach (var item in ItemCollection.Select(x => x.RefGenericDrugDetail).Where(x => x != null && x.GenMedProductID > 0))
            {
                if (GenericCollection.ContainsKey(item.GenericID))
                {
                    continue;
                }
                GenericCollection.Add(item.GenericID, new object[] { true, item.GenericName });
            }
            //Thêm các hoạt chất từ những toa thuốc khác vào diction
            foreach (var item in AllOrtherItemCollection.Select(x => x.RefGenericDrugDetail).Where(x => x != null && x.GenMedProductID > 0))
            {
                if (GenericCollection.ContainsKey(item.GenericID))
                {
                    continue;
                }
                GenericCollection.Add(item.GenericID, new object[] { false, item.GenericName });
            }
            bool IsHasBlockInteraction = false;
            bool IsHasInteraction = false;
            List<long> ListAddedGenericID = new List<long>();
            StringBuilder mStringBuilder = new StringBuilder();
            //Lặp tất cả các hoạt chất của toa hiện tại
            foreach (var item in GenericCollection.Where(x => Convert.ToBoolean(x.Value[0])))
            {
                //Lấy danh sách các hoạt chất tương tác với hoạt chất đang kiểm tra
                List<RefGenericRelation> ListInteractionGeneric = GetRefGenericRelation(item.Key);
                //Kiểm tra có hoạt chất tương tác tồn tại trong diction (khác hoạt chất đang kiểm tra)
                if (ListInteractionGeneric.Any(x => x.IsInteraction && x.V_InteractionWarningLevel.LookupID != (long)AllLookupValues.V_WarningLevel.Normal && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)))
                {
                    IsHasInteraction = true;
                    ListAddedGenericID.Add(item.Key);
                    //Lặp danh sách hoạt chất tương tác để tạo chuỗi thông báo lỗi
                    foreach (var seconditem in ListInteractionGeneric.Where(x => x.IsInteraction && x.V_InteractionWarningLevel.LookupID != (long)AllLookupValues.V_WarningLevel.Normal && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)).ToList())
                    {
                        // 20201211 TNHX: chỉ chặn V_InteractionSeverityLevel >= 82904
                        if (!IsHasBlockInteraction && seconditem.V_InteractionWarningLevel.LookupID == (long)AllLookupValues.V_WarningLevel.Block
                            && seconditem.V_InteractionSeverityLevel.LookupID >= Globals.ServerConfigSection.ConsultationElements.BlockInteractionSeverityLevelInPt)
                        {
                            IsHasBlockInteraction = true;
                        }
                        //Bỏ qua hoạt chất đã được kiểm tra trước đó
                        if (ListAddedGenericID.Contains(seconditem.SecondGeneric.GenericID))
                        {
                            continue;
                        }
                        //Tạo chuỗi thông báo lỗi
                        mStringBuilder.AppendLine(string.Format("- " + item.Value[1].ToString().Trim() + " tương tác với " + seconditem.SecondGeneric.GenericName + " [" + seconditem.V_InteractionSeverityLevel.ObjectValue + "] " + (seconditem.V_InteractionWarningLevel.LookupID == (long)AllLookupValues.V_WarningLevel.Block ? " (*)" : "") + (!Convert.ToBoolean(GenericCollection[seconditem.SecondGeneric.GenericID][0]) ? " (Ngoài toa)" : "")));
                    }
                }
            }
            //LatestePrecriptions.IsWarningInteraction = IsHasInteraction;
            if (!IsHasInteraction)
            {
                return "";
            }
            if (!IsHasBlockInteraction)
            {
                return string.Format(eHCMSResources.Z2674_G1_ToaThuocCoHoatChatTuongTac + Environment.NewLine + mStringBuilder.ToString());
            }
            else if (IsHasBlockInteraction)
            {
                bBlock = true;
                return string.Format(eHCMSResources.Z2674_G1_ToaThuocCoHoatChatTuongTac + Environment.NewLine + mStringBuilder.ToString());
            }
            return "";
        }
        private bool CheckDrug_ForGeneric(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, long? GenericID, long? DrugID, out string DrugName)
        {
            DrugName = "";
            if (GenericID != null && GenericID > 0)
            {
                foreach (var prescriptionDetail in ItemCollection)
                {
                    if (prescriptionDetail.RefGenericDrugDetail != null && prescriptionDetail.RefGenericDrugDetail.GenericID == GenericID && prescriptionDetail.RefGenericDrugDetail.GenMedProductID != DrugID)
                    {
                        DrugName = " - " + prescriptionDetail.RefGenericDrugDetail.BrandName;
                        return true;
                    }
                }
            }
            return false;
        }
        private string CheckThuocBiTrungHoatChatTrongToa(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungTheoHoatChat || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var prescriptionDetail in ItemCollection)
            {
                string DrugName = "";
                if (prescriptionDetail.RefGenericDrugDetail != null
                    && CheckDrug_ForGeneric(ItemCollection, prescriptionDetail.RefGenericDrugDetail.GenericID, prescriptionDetail.RefGenericDrugDetail.GenMedProductID, out DrugName)
                    && !sb.ToString().Contains(prescriptionDetail.RefGenericDrugDetail.BrandName)) //TBL: Tránh lặp lại tên thuốc A - B và B - A
                {
                    DrugName += " và " + prescriptionDetail.RefGenericDrugDetail.BrandName;
                    sb.AppendLine(DrugName);
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2861_G1_TrungHoatChatTrongToa + ":" + Environment.NewLine + sb.ToString();
            }
            else
            {
                return "";
            }
        }
        private bool CheckThuocBiTrungTrongNgay_TheoHoatChat(IList<ReqOutwardDrugClinicDeptPatient> AllOrtherItemCollection, ReqOutwardDrugClinicDeptPatient pNeedSave)
        {
            if (AllOrtherItemCollection != null && AllOrtherItemCollection.Count > 0)
            {
                foreach (ReqOutwardDrugClinicDeptPatient listdetail_i in AllOrtherItemCollection)
                {
                    if (listdetail_i.GenMedProductID != 0 && listdetail_i.GenMedProductID != null && listdetail_i.RefGenericDrugDetail != null &&
                        pNeedSave.RefGenericDrugDetail != null && listdetail_i.RefGenericDrugDetail.GenericID == pNeedSave.RefGenericDrugDetail.GenericID)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool CheckToaThuocBiTrungTrongNgay_TheoHoatChat(IList<ReqOutwardDrugClinicDeptPatient> AllItemCollection, IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, out string msg)
        {
            StringBuilder sb = new StringBuilder();
            msg = "";
            if (ItemCollection != null && ItemCollection.Count > 0)
            {
                foreach (var prescriptDetaiil in ItemCollection)
                {
                    if (!CheckThuocBiTrungTrongNgay_TheoHoatChat(AllItemCollection, prescriptDetaiil) && prescriptDetaiil.RefGenericDrugDetail != null && prescriptDetaiil.RefGenericDrugDetail.GenericName != null)
                    {
                        sb.AppendLine(string.Format("- " + prescriptDetaiil.RefGenericDrugDetail.GenericName.Trim()));
                    }
                }
                msg = sb.ToString();
            }
            if (!string.IsNullOrEmpty(msg))
            {
                return true;
            }
            return false;
        }
        private string CheckThuocBiTrungHoatChatTrongNgay(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, IList<ReqOutwardDrugClinicDeptPatient> AllOrtherItemCollection, ref bool bBlock)
        {
            //TBL: Neu bat cau hinh thi moi kiem tra hoat chat
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungTheoHoatChat || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            string err = "";
            if (AllOrtherItemCollection.Count > 1)
            {
                if (CheckToaThuocBiTrungTrongNgay_TheoHoatChat(AllOrtherItemCollection, ItemCollection, out err))
                {
                    return string.Format(eHCMSResources.Z2650_G1_ToaThuocTrungHoatChatTrongNgay + Environment.NewLine + err);
                }
            }
            return "";
        }
        private string KiemTraTuongTuHoatChatTrongNgay(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock, IList<ReqOutwardDrugClinicDeptPatient> AllOrtherItemCollection)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.KiemTraQuanHeHoatChat || bBlock)
            {
                return "";
            }
            //Tạo diction các hoạt chất với các giá trị và true or false cho toa thuốc đang chỉnh sửa và tên hoạt chất
            Dictionary<long, object[]> GenericCollection = new Dictionary<long, object[]>();
            //Thêm các hoạt chất từ toa thuốc đang chỉnh sửa vào diction
            foreach (var item in ItemCollection.Select(x => x.RefGenericDrugDetail).Where(x => x != null && x.GenMedProductID > 0))
            {
                if (GenericCollection.ContainsKey(item.GenericID))
                {
                    continue;
                }
                GenericCollection.Add(item.GenericID, new object[] { true, item.GenericName });
            }
            //Thêm các hoạt chất từ những toa thuốc khác vào diction
            foreach (var item in AllOrtherItemCollection.Select(x => x.RefGenericDrugDetail).Where(x => x != null && x.GenMedProductID > 0))
            {
                if (GenericCollection.ContainsKey(item.GenericID))
                {
                    continue;
                }
                GenericCollection.Add(item.GenericID, new object[] { false, item.GenericName });
            }
            bool IsHasSimilar = false;
            List<long> ListAddedGenericID = new List<long>();
            StringBuilder mStringBuilder = new StringBuilder();
            //Lặp tất cả các hoạt chất của toa hiện tại
            foreach (var item in GenericCollection.Where(x => Convert.ToBoolean(x.Value[0])))
            {
                //Lấy danh sách các hoạt chất tương tự với hoạt chất đang kiểm tra
                List<RefGenericRelation> ListInteractionGeneric = GetRefGenericRelation(item.Key);
                //Kiểm tra có hoạt chất tương tự tồn tại trong diction (khác hoạt chất đang kiểm tra)
                if (ListInteractionGeneric.Any(x => x.IsSimilar && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)))
                {
                    IsHasSimilar = true;
                    ListAddedGenericID.Add(item.Key);
                    //Lặp danh sách hoạt chất tương tự để tạo chuỗi thông báo lỗi
                    foreach (var seconditem in ListInteractionGeneric.Where(x => x.IsSimilar && x.SecondGeneric.GenericID != item.Key && GenericCollection.ContainsKey(x.SecondGeneric.GenericID)).ToList())
                    {
                        //Bỏ qua hoạt chất đã được kiểm tra trước đó
                        if (ListAddedGenericID.Contains(seconditem.SecondGeneric.GenericID))
                        {
                            continue;
                        }
                        //Tạo chuỗi thông báo lỗi
                        mStringBuilder.AppendLine(string.Format("- " + item.Value[1].ToString().Trim() + " tương tự với " + seconditem.SecondGeneric.GenericName + (!Convert.ToBoolean(GenericCollection[seconditem.SecondGeneric.GenericID][0]) ? " (Ngoài toa)" : "")));
                    }
                }
            }
            //LatestePrecriptions.IsWarningSimilar = IsHasSimilar;
            if (!IsHasSimilar)
            {
                return "";
            }
            else
            {
                return string.Format(eHCMSResources.Z2675_G1_ToaThuocCoHoatChatTuongTu + Environment.NewLine + mStringBuilder.ToString());
            }
        }
        private bool CheckDrug_ForDrugClass(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, long? DrugClassID, long? DrugID, out string DrugName)
        {
            DrugName = "";
            if (DrugClassID != null && DrugClassID > 0)
            {
                foreach (var prescriptionDetail in ItemCollection)
                {
                    if (prescriptionDetail.RefGenericDrugDetail != null && prescriptionDetail.RefGenericDrugDetail.DrugClassID == DrugClassID && prescriptionDetail.RefGenericDrugDetail.GenMedProductID != DrugID)
                    {
                        DrugName = " - " + prescriptionDetail.RefGenericDrugDetail.BrandName;
                        return true;
                    }
                }
            }
            return false;
        }
        private string CheckThuocBiTrungNhomThuocTrongToa(IList<ReqOutwardDrugClinicDeptPatient> ItemCollection, ref bool bBlock)
        {
            if (!Globals.ServerConfigSection.ConsultationElements.CheckToaThuocBiTrungNhomThuoc || bBlock)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            foreach (var prescriptionDetail in ItemCollection)
            {
                string DrugName = "";
                if (prescriptionDetail.RefGenericDrugDetail != null
                    && CheckDrug_ForDrugClass(ItemCollection, prescriptionDetail.RefGenericDrugDetail.DrugClassID, prescriptionDetail.RefGenericDrugDetail.GenMedProductID, out DrugName)
                    && !sb.ToString().Contains(prescriptionDetail.RefGenericDrugDetail.BrandName)) //TBL: Tránh lặp lại tên thuốc A - B và B - A
                {
                    DrugName += " và " + prescriptionDetail.RefGenericDrugDetail.BrandName + ": " + (prescriptionDetail.RefGenericDrugDetail.SelectedDrugClass == null ? "" : prescriptionDetail.RefGenericDrugDetail.SelectedDrugClass.FaName);
                    sb.AppendLine(DrugName);
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return eHCMSResources.Z2862_G1_TrungNhomThuocTrongToa + ":" + Environment.NewLine + sb.ToString();
            }
            else
            {
                return "";
            }
        }
        private bool CheckValidDrugInUseBeforeSave(IList<ReqOutwardDrugClinicDeptPatient> AllOrtherItemCollection)
        {
            if (CurRegistration != null && CurRegistration.InPatientInstruction != null &&
                CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID > 0)
            {
                AllOrtherItemCollection = AllOrtherItemCollection.Where(x => x.IntPtDiagDrInstructionID != CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID).ToList();
            }
            var IsBlockProcess = false;
            StringBuilder CurrentStringBuilder = new StringBuilder();
            string ErrorMessage = "";
            //▼===== #022
            //TBL: Thay đổi cách hiển thị thông báo. Lúc trước hiển thị từng popup, bây giờ hiển thị 1 lúc
            ErrorMessage += CheckAllThuocBiDiUng(CurrentDrugCollection, ref IsBlockProcess);
            ErrorMessage += CheckThuocBiTrungTrongToa(CurrentDrugCollection, ref IsBlockProcess);
            ErrorMessage += ErrCheckChongChiDinh(CurrentDrugCollection, ref IsBlockProcess, InPatientDailyDiagnosisContent.ICD10List);

            // VuTTM
            // Check for nutrition
            ErrorMessage += CheckAllThuocBiDiUng(CurrentNutritionCollection, ref IsBlockProcess);
            ErrorMessage += CheckThuocBiTrungTrongToa(CurrentNutritionCollection, ref IsBlockProcess);
            ErrorMessage += ErrCheckChongChiDinh(CurrentNutritionCollection, ref IsBlockProcess, InPatientDailyDiagnosisContent.ICD10List);
            //▼===== #030
            //if (AllOrtherItemCollection != null && AllOrtherItemCollection.Count > 0)
            //{
            //    ErrorMessage += ErrCheckChongChiDinh_TrongNgay(AllOrtherItemCollection, ref IsBlockProcess, InPatientDailyDiagnosisContent.ICD10List);
            //}
            //▲===== #030
            //ErrorMessage += CheckPhacDo(CurrentDrugCollection, ref IsBlockProcess);
            if (AllOrtherItemCollection != null && AllOrtherItemCollection.Count > 0)
            {
                ErrorMessage += KiemTraTuongTacHoatChatTrongNgay(CurrentDrugCollection, ref IsBlockProcess, AllOrtherItemCollection);

                // VuTTM
                // Check for nutrition
                ErrorMessage += KiemTraTuongTacHoatChatTrongNgay(CurrentNutritionCollection, ref IsBlockProcess, AllOrtherItemCollection);
            }
            ErrorMessage += CheckThuocBiTrungHoatChatTrongToa(CurrentDrugCollection, ref IsBlockProcess);
            if (AllOrtherItemCollection != null && AllOrtherItemCollection.Count > 0)
            {
                ErrorMessage += CheckThuocBiTrungHoatChatTrongNgay(CurrentDrugCollection, AllOrtherItemCollection, ref IsBlockProcess);
                ErrorMessage += KiemTraTuongTuHoatChatTrongNgay(CurrentDrugCollection, ref IsBlockProcess, AllOrtherItemCollection);

                // VuTTM
                // Check for nutrition
                ErrorMessage += CheckThuocBiTrungHoatChatTrongNgay(CurrentNutritionCollection, AllOrtherItemCollection, ref IsBlockProcess);
                ErrorMessage += KiemTraTuongTuHoatChatTrongNgay(CurrentNutritionCollection, ref IsBlockProcess, AllOrtherItemCollection);
            }
            ErrorMessage += CheckThuocBiTrungNhomThuocTrongToa(CurrentDrugCollection, ref IsBlockProcess);

            // VuTTM
            // Check for nutrition
            ErrorMessage += CheckThuocBiTrungNhomThuocTrongToa(CurrentNutritionCollection, ref IsBlockProcess);
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                IErrorBold MessBox = Globals.GetViewModel<IErrorBold>();
                MessBox.FireOncloseEvent = true;
                if (IsBlockProcess)
                {
                    MessBox.isCheckBox = false;
                    MessBox.SetMessage(ErrorMessage + Environment.NewLine + eHCMSResources.Z2676_G1_KhongTheRaToa, "");
                }
                else
                {
                    MessBox.IsShowReason = true;
                    MessBox.SetMessage(ErrorMessage, eHCMSResources.Z0627_G1_TiepTucLuu);
                    //20191028 TBL: BM 0018509: Toa thuốc có Reason thì khi cập nhật đem Reason lên để trên popup cho người dùng hiệu chỉnh
                    //MessBox.Reason = LatestePrecriptions.Reason;
                }
                GlobalsNAV.ShowDialog_V3(MessBox);
                if (!MessBox.IsAccept)
                {
                    return false;
                }
                //LatestePrecriptions.Reason = MessBox.Reason;
            }
            else //20191029 TBL: Trường hợp lúc đầu có cảnh báo và đã nhập lý do nhưng sau đó số lượng thuốc không đủ thì chỉnh sửa lại số lượng thuốc và không còn cảnh báo nữa thì set Reason = ""
            {
                //LatestePrecriptions.Reason = "";
            }
            //▲===== #022
            if (!CheckThuocHopLe(CurrentDrugCollection)
                || !CheckThuocHopLe(CurrentNutritionCollection, false)) // VuTTM - Check for nutrition
            {
                return false;
            }
            return true;
        }
        public void GetAllDrugFromDoctorInstruction(GenericCoRoutineTask aGenTask, object aPtRegistrationID, object aCurrentDate)
        {
            long PtRegistrationID = Convert.ToInt64(aPtRegistrationID);
            DateTime CurrentDate = Convert.ToDateTime(aCurrentDate);
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var CurrentFactory = new CommonService_V2Client())
                    {
                        var CurrentContract = CurrentFactory.ServiceInstance;
                        CurrentContract.BeginGetAllDrugFromDoctorInstruction(CurRegistration.PtRegistrationID, CurrentDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                aGenTask.AddResultObj(CurrentContract.EndGetAllDrugFromDoctorInstruction(asyncResult));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                aGenTask.ActionComplete(true);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                    aGenTask.ActionComplete(true);
                }
            });
            CurrentThread.Start();
        }
        private IEnumerator<IResult> EditCurrentInstruction_Routine(bool IsUpdate = false)
        {
            DrugsInTreatmentRegimen = new ObservableCollection<GetDrugForSellVisitor>();

            var GetDrugsInTreatmentRegimenTask = new GenericCoRoutineTask(GetDrugsInTreatmentRegimen, InPatientDailyDiagnosisContent.ICD10List);
            yield return GetDrugsInTreatmentRegimenTask;

            var GetAllDrugFromDoctorInstructionTask = new GenericCoRoutineTask(GetAllDrugFromDoctorInstruction,
                    CurRegistration.PtRegistrationID,
                    MedicalInstructionDateContent.DateTime.GetValueOrDefault(
                        new DateTime(Globals.GetCurServerDateTime().Year,
                            Globals.GetCurServerDateTime().Month,
                            Globals.GetCurServerDateTime().Day,
                            Globals.GetCurServerDateTime().Hour,
                            Globals.GetCurServerDateTime().Minute, 0)));
            yield return GetAllDrugFromDoctorInstructionTask;
            var ItemCollection = GetAllDrugFromDoctorInstructionTask.GetResultObj(0) as IList<ReqOutwardDrugClinicDeptPatient>;

            if (!CheckValidDrugInUseBeforeSave(ItemCollection))
            {
                yield break;
            }

            EditInPatientInstruction(IsUpdate);
        }
        #endregion
        //▼===== #004
        private void ReloadLoadInPatientInstructionNew(GenericCoRoutineTask genTask, object aIntPtDiagDrInstructionID)
        {
            CallShowBusyIndicator();
            var t = new Thread(() =>
            {
                bool bContinue = true;
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInPatientInstructionByInstructionID((long)aIntPtDiagDrInstructionID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var mPatientInstruction = contract.EndGetInPatientInstructionByInstructionID(asyncResult);
                                    if (mPatientInstruction != null)
                                    {
                                        CurRegistration.InPatientInstruction = mPatientInstruction;
                                        UploadViewContent(true);
                                        //▼===== #005
                                        if (CurRegistration.InPatientInstruction.DoctorStaff != null)
                                        {
                                            gSelectedDoctorStaff = CurRegistration.InPatientInstruction.DoctorStaff;
                                            //20191030 TBL: Theo ý anh Tuấn nếu người tạo y lệnh và bác sĩ chỉ định là 1 người thì không cho cập nhật lại bác sĩ chỉ định
                                            if (CurRegistration.InPatientInstruction.Staff != null && CurRegistration.InPatientInstruction.Staff.StaffID != CurRegistration.InPatientInstruction.DoctorStaff.StaffID)
                                            {
                                                IsUpdateDoctorStaff = true;
                                            }
                                            else
                                            {
                                                IsUpdateDoctorStaff = false;
                                            }
                                        }
                                        else
                                        {
                                            gSelectedDoctorStaff = new Staff();
                                        }
                                        //▲===== #005
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    bContinue = false;
                                }
                                finally
                                {
                                    genTask.ActionComplete(bContinue);
                                    CallHideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    CallHideBusyIndicator();
                }
            });
            t.Start();
        }
        private void AddNewDiagTrmt(GenericCoRoutineTask genTask)
        {
            CallShowBusyIndicator();
            //20191129 TBL: Lấy StaffID của bác sĩ để set DoctorStaffID
            Registration_DataStorage.CurrentDiagnosisTreatment.PatientServiceRecord.StaffID = gSelectedDoctorStaff.StaffID;
            Registration_DataStorage.CurrentDiagnosisTreatment.IsSevereIllness = CurRegistration.AdmissionInfo.IsSevereIllness;
            long ID = Compare2Object();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddDiagnosisTreatment_InPt(Registration_DataStorage.CurrentDiagnosisTreatment, ID, ICD10List, 0, new List<DiagnosisICD9Items>(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
                                if (contract.EndAddDiagnosisTreatment_InPt(out ReloadInPatientDeptDetails, asyncResult))
                                {
                                    if (ICD10List != null)
                                    {
                                        InPatientDailyDiagnosisContent.ICD10List = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                                    }
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                }
                                else
                                {
                                    if (Registration_DataStorage.CurrentDiagnosisTreatment.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0409_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else if (Registration_DataStorage.CurrentDiagnosisTreatment.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0410_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0802_G1_Msg_InfoLuuCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                CallHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    CallHideBusyIndicator();
                }

            });
            t.Start();
        }
        private void UpdateDiagTrmt(GenericCoRoutineTask genTask)
        {
            CallShowBusyIndicator();
            //20191129 TBL: Lấy StaffID của bác sĩ để set DoctorStaffID
            Registration_DataStorage.CurrentDiagnosisTreatment.DoctorStaffID = gSelectedDoctorStaff.StaffID;
            Registration_DataStorage.CurrentDiagnosisTreatment.IsSevereIllness = CurRegistration.AdmissionInfo.IsSevereIllness;
            long ID = Compare2Object();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateDiagnosisTreatment_InPt(Registration_DataStorage.CurrentDiagnosisTreatment, ID, ICD10List, 0, new List<DiagnosisICD9Items>(), IsUpdateDiagConfirmInPT, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<InPatientDeptDetail> ReloadInPatientDeptDetails = new List<InPatientDeptDetail>();
                                int VersionNumberOut = 0;
                                if (contract.EndUpdateDiagnosisTreatment_InPt(out ReloadInPatientDeptDetails, out VersionNumberOut, asyncResult))
                                {
                                    if (ICD10List != null)
                                    {
                                        InPatientDailyDiagnosisContent.ICD10List = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                                    }
                                    Registration_DataStorage.CurrentDiagnosisTreatment.VersionNumber = VersionNumberOut;
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.K2782_G1_DaCNhat);
                                }
                                else
                                {
                                    if (Registration_DataStorage.CurrentDiagnosisTreatment.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_IN)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0403_G1_DaCoCDoanNV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else if (Registration_DataStorage.CurrentDiagnosisTreatment.V_DiagnosisType == (long)AllLookupValues.V_DiagnosisType.DIAGNOSIS_OUTHOS)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0404_G1_DaCoCDoanXV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                CallHideBusyIndicator();
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    CallHideBusyIndicator();
                }


            });
            t.Start();
        }
        private bool Equal(DiagnosisIcd10Items item1, DiagnosisIcd10Items item2)
        {
            return item1.DiagIcd10ItemID == item2.DiagIcd10ItemID
                && item1.DiagnosisIcd10ListID == item2.DiagnosisIcd10ListID
                && item1.ICD10Code == item2.ICD10Code
                && item1.IsMain == item2.IsMain
                && item1.IsCongenital == item2.IsCongenital
                && (item1.LookupStatus != null && item2.LookupStatus != null
                    && item1.LookupStatus.LookupID == item2.LookupStatus.LookupID);
        }
        private long Compare2Object()
        {
            long ListID = 0;
            ObservableCollection<DiagnosisIcd10Items> temp = ICD10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
            if (refIDC10ListCopy != null && refIDC10ListCopy.Count > 0 && refIDC10ListCopy.Count == temp.Count)
            {
                int icount = 0;
                for (int i = 0; i < refIDC10ListCopy.Count; i++)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (Equal(refIDC10ListCopy[i], ICD10List[j]))
                        {
                            icount++;
                        }
                    }
                }
                if (icount == refIDC10ListCopy.Count)
                {
                    ListID = refIDC10ListCopy.FirstOrDefault().DiagnosisIcd10ListID;
                    return ListID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        public ObservableCollection<DiagnosisIcd10Items> ICD10List
        {
            get
            {
                return InPatientDailyDiagnosisContent.ICD10List;
            }
        }
        public ObservableCollection<DiagnosisIcd10Items> refIDC10ListCopy
        {
            get
            {
                return InPatientDailyDiagnosisContent.refIDC10ListCopy;
            }
        }
        private IEnumerator<IResult> SaveNewDiagnosis(long aIntPtDiagDrInstructionID, bool IsUpdate = false)
        {
            if (!IsUpdate)
            {
                yield return GenericCoRoutineTask.StartTask(AddNewDiagTrmt);
            }
            else
            {
                yield return GenericCoRoutineTask.StartTask(UpdateDiagTrmt);
            }
            yield return GenericCoRoutineTask.StartTask(ReloadLoadInPatientInstructionNew, aIntPtDiagDrInstructionID);

            //▼===== 20200708 TTM: Sau khi load y lệnh lại thì load luôn cây y lệnh
            Globals.EventAggregator.Publish(new ReloadDiagnosisTreatmentTree { gRegistration = CurRegistration });
            yield break;
        }
        //▲===== #004
        //▼===== #005
        private void LoadDoctorStaffCollection()
        {
            //▼====: #022
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null 
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            //▲====: #022
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
        }
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt)
            {
                string tempCurDeptID = SelectedInPatientDeptDetail != null && SelectedInPatientDeptDetail.DeptLocation != null ? SelectedInPatientDeptDetail.DeptLocation.DeptID.ToString() : "";
                DoctorStaffs = DoctorStaffs.Where(x => x.ListDeptResponsibilities != null && ((x.ListDeptResponsibilities.Contains(tempCurDeptID) || tempCurDeptID == ""))).ToObservableCollection();
            }
            if (Globals.ServerConfigSection.CommonItems.IsApplyTimeSegments)
            {
                DoctorStaffs = DoctorStaffs.Where(x => 
                            x.ConsultationTimeSegmentsList != null && 
                            (x.ConsultationTimeSegmentsList.Where(y => 
                                    y.StartTime.TimeOfDay < Globals.ServerDate.Value.TimeOfDay 
                                    && y.EndTime.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0 
                            || x.ConsultationTimeSegmentsList.Where(y => 
                                    y.EndTime2 != null 
                                    && y.StartTime2.Value.TimeOfDay < Globals.ServerDate.Value.TimeOfDay 
                                    && y.EndTime2.Value.TimeOfDay > Globals.ServerDate.Value.TimeOfDay).Count() > 0)).ToObservableCollection();
            }
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }
        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        //▲===== #005
        private bool _FromInPatientAdmView = false;
        public bool FromInPatientAdmView
        {
            get { return _FromInPatientAdmView; }
            set
            {
                if (_FromInPatientAdmView != value)
                {
                    _FromInPatientAdmView = value;
                    NotifyOfPropertyChange(() => FromInPatientAdmView);
                }
            }
        }

        //▼===== 20200109 TTM: Bổ sung thêm code để đưa HisID và TotalCoPayment cho các dịch vụ hoặc CLS khi chỉ định
        private void CalcInvoiceItem(MedRegItemBase item)
        {
            item.HIAllowedPrice = item.ChargeableItem.HIAllowedPrice.GetValueOrDefault();
            item.PriceDifference = item.InvoicePrice - item.HIAllowedPrice.GetValueOrDefault(0);

            bool bItemInstDateIsValidWithHICard_1 = false;
            bool bItemInstDateIsValidWithHICard_2 = false;
            bool bItemInstDateIsValidWithHICard_3 = false;
            if (CurRegistration.HealthInsurance != null 
                && CurRegistration.PtInsuranceBenefit > 0 
                && (item.MedicalInstructionDate >= CurRegistration.HealthInsurance.ValidDateFrom
                //▼====: #031
                && item.MedicalInstructionDate <= CurRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt)))
                //▲====: #031
            {
                bItemInstDateIsValidWithHICard_1 = true;
            }
            else
            {
                if (CurRegistration.HealthInsurance_3 != null 
                    && item.MedicalInstructionDate >= CurRegistration.HealthInsurance_3.ValidDateFrom
                    //▼====: #031
                    && item.MedicalInstructionDate <= CurRegistration.HealthInsurance_3.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt))
                    //▲====: #031
                {
                    bItemInstDateIsValidWithHICard_3 = true;
                }
                else if (CurRegistration.HealthInsurance_2 != null 
                    && item.MedicalInstructionDate >= CurRegistration.HealthInsurance_2.ValidDateFrom
                    //▼====: #031
                    && item.MedicalInstructionDate <= CurRegistration.HealthInsurance_2.ValidDateTo.GetValueOrDefault().AddDays(Globals.ServerConfigSection.InRegisElements.NumDayHIAgreeToPayAfterHIExpiresInPt))
                    //▲====: #031

                {
                    bItemInstDateIsValidWithHICard_2 = true;
                }
            }

            if (item.IsCountHI && bItemInstDateIsValidWithHICard_1)
            {
                item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0.0) * item.Qty;
                item.TotalCoPayment = item.HIAllowedPrice.GetValueOrDefault(0) - item.TotalHIPayment;
                item.HIBenefit = CurRegistration.PtInsuranceBenefit.GetValueOrDefault(0);
                //▼===== #009
                item.TotalPriceDifference = (item.InvoicePrice - (decimal)item.HIAllowedPrice) * item.Qty;
                //▲===== #009
                item.HisID = CurRegistration.HisID;

            }
            else if (item.IsCountHI && bItemInstDateIsValidWithHICard_3)
            {
                item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)CurRegistration.PtInsuranceBenefit_3.GetValueOrDefault(0.0) * item.Qty;
                item.TotalCoPayment = item.HIAllowedPrice.GetValueOrDefault(0) - item.TotalHIPayment;
                item.HIBenefit = CurRegistration.PtInsuranceBenefit_3.GetValueOrDefault(0.0);
                //▼===== #009
                item.TotalPriceDifference = (item.InvoicePrice - (decimal)item.HIAllowedPrice) * item.Qty;
                //▲===== #009
                item.HisID = CurRegistration.HisID_3;
            }
            else if (item.IsCountHI && bItemInstDateIsValidWithHICard_2)
            {
                item.TotalHIPayment = item.HIAllowedPrice.GetValueOrDefault(0) * (decimal)CurRegistration.PtInsuranceBenefit_2.GetValueOrDefault(0.0) * item.Qty;
                item.TotalCoPayment = item.HIAllowedPrice.GetValueOrDefault(0) - item.TotalHIPayment;
                item.HIBenefit = CurRegistration.PtInsuranceBenefit_2.GetValueOrDefault(0.0);
                //▼===== #009
                item.TotalPriceDifference = (item.InvoicePrice - (decimal)item.HIAllowedPrice) * item.Qty;
                //▲===== #009
                item.HisID = CurRegistration.HisID_2;
            }
            else
            {
                item.HisID = null;
                item.HIBenefit = null;
                item.IsCountHI = false;
                item.TotalHIPayment = 0;
            }
        }

        //▼===== #015
        public void hplCheckDrugInfor_Click(Object pSelectedItem)
        {
            if (pSelectedItem == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            GetDrugInformation((long)(pSelectedItem as ReqOutwardDrugClinicDeptPatient).GenMedProductID);
        }

        public void GetDrugInformation(long DrugID)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugInformation(DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugInformation(asyncResult);
                            if (results == null || results.RefGenericDrugDetail == null || string.IsNullOrEmpty(results.RefGenericDrugDetail.SdlDescription))
                            {
                                MessageBox.Show(eHCMSResources.Z2738_G1_ThongTinThuocKhongHopLe);
                            }
                            else
                            {
                                Action<IDrugInformation> onInitDlg = delegate (IDrugInformation proAlloc)
                                {
                                    proAlloc.SelectedDrugInformation = results;
                                };
                                GlobalsNAV.ShowDialog<IDrugInformation>(onInitDlg);
                            }
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
        //▲===== #015
        //▼===== #023
        public void EditTicketCareCmd()
        {
            if (CurRegistration == null || CurRegistration.InPatientInstruction == null ||
                CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID == 0 ||
                SelectedInPatientDeptDetail == null ||
                SelectedInPatientDeptDetail.DeptLocation == null ||
                SelectedInPatientDeptDetail.DeptLocation.DeptID == 0)
            {
                return;
            }
            ITicketCare mDialogView = Globals.GetViewModel<ITicketCare>();
            mDialogView.PtRegistrationID = CurRegistration.PtRegistrationID;
            mDialogView.IntPtDiagDrInstructionID = CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID;
            mDialogView.V_LevelCare = CurRegistration.InPatientInstruction.V_LevelCare.LookupID;
            mDialogView.GetTicketCare(CurRegistration.InPatientInstruction.IntPtDiagDrInstructionID);
            if (InPatientDailyDiagnosisContent != null && InPatientDailyDiagnosisContent.DiagnosisTreatmentContent != null)
            {
                mDialogView.DoctorOrientedTreatment = InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.OrientedTreatment;
            }
            GlobalsNAV.ShowDialog_V3(mDialogView);
        }
        //▲===== #023
        public void Handle(DiseaseProgressionInstruction_Event message)
        {
            if (message != null)
            {
                if (String.IsNullOrEmpty(InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.OrientedTreatment))
                {
                    InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.OrientedTreatment += "Bệnh tỉnh, tiếp xúc tốt, da niêm hồng, tim đều, phổi trong, bụng mềm" + message.SelectedDiseaseProgression;
                }
                else
                {
                    InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.OrientedTreatment += message.SelectedDiseaseProgression;
                }
                //if (String.IsNullOrEmpty(DiagnosisTreatmentContent.Diagnosis))
                //{
                //    DiagnosisTreatmentContent.Diagnosis += message.SelectedDiseaseProgression.Substring(2, message.SelectedDiseaseProgression.Length - 2);
                //}
                //else
                //{
                //    DiagnosisTreatmentContent.Diagnosis += message.SelectedDiseaseProgression;
                //}
            }
        }
        
        private ObservableCollection<Lookup> _ListV_TransferRateUnit;
        public ObservableCollection<Lookup> ListV_TransferRateUnit
        {
            get { return _ListV_TransferRateUnit; }
            set
            {
                if (_ListV_TransferRateUnit != value)
                    _ListV_TransferRateUnit = value;
                NotifyOfPropertyChange(() => ListV_TransferRateUnit);
            }
        }
        private ObservableCollection<Lookup> _AllRouteOfAdministration;
        public ObservableCollection<Lookup> AllRouteOfAdministration
        {
            get { return _AllRouteOfAdministration; }
            set
            {
                if (_AllRouteOfAdministration != value)
                    _AllRouteOfAdministration = value;
                NotifyOfPropertyChange(() => AllRouteOfAdministration);
            }
        }
        private ObservableCollection<Lookup> _ListV_ReconmendTimeUsageDistance;
        public ObservableCollection<Lookup> ListV_ReconmendTimeUsageDistance
        {
            get { return _ListV_ReconmendTimeUsageDistance; }
            set
            {
                _ListV_ReconmendTimeUsageDistance = value;
                NotifyOfPropertyChange(() => ListV_ReconmendTimeUsageDistance);
            }
        }
        public void GetRouteOfAdministrationList(RefGenMedProductDetails refGen, long DrugROAID, long DrugID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRouteOfAdministrationList(DrugROAID, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRouteOfAdministrationList(asyncResult);
                            if (results != null)
                            {
                                refGen.ListRouteOfAdministration = new ObservableCollection<Lookup>();
                                var obsRMCT = new ObservableCollection<Lookup>();
                                foreach (var p in results)
                                {
                                    if (p.RouteOfAdministration == null)
                                    {
                                        p.RouteOfAdministration = new Lookup();
                                    }
                                    //p.RefMedicalConditionType.IsWarning = p.IsWarning;
                                    refGen.ListRouteOfAdministration.Add(p.RouteOfAdministration);
                                }
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

                    }), null);

                }

            });

            t.Start();
        }
        //private bool _IsTruyenTinhMach;
        //public bool IsTruyenTinhMach
        //{
        //    get { return _IsTruyenTinhMach; }
        //    set
        //    {
        //        _IsTruyenTinhMach = value;
        //        NotifyOfPropertyChange(() => IsTruyenTinhMach);
        //    }
        //}
        public void RouteOfAdministrationComboBox_Close(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            ComboBox comboBoxROA = (ComboBox)sender;
            if(comboBoxROA.Items.Count==0 || comboBoxROA.SelectedItem == null)
            {
                return;
            }
            if ((comboBoxROA.SelectedItem as Lookup).LookupID != 61319)
            {
                CurrentSelectedDrug.IsTruyenTinhMach = false;
                CurrentSelectedDrug.TransferRate = 0;
                CurrentSelectedDrug.V_TransferRateUnit = 0;
            }
            else
            {
                CurrentSelectedDrug.IsTruyenTinhMach = true;
                CurrentSelectedDrug.TransferRate = 0;
                CurrentSelectedDrug.V_TransferRateUnit = 0;
            }
        }
        //Cờ xác định y lênh đã được tạo mới thì mới thì mới lấy thông tin từ y lệnh cũ truyền vào
        private bool _CreateNewFromOldFlag ;
        public bool CreateNewFromOldFlag
        {
            get { return _CreateNewFromOldFlag; }
            set
            {
                _CreateNewFromOldFlag = value;
                NotifyOfPropertyChange(() => CreateNewFromOldFlag);
            }
        }
        private bool _LoadFromWebsiteFlag;
        public bool LoadFromWebsiteFlag
        {
            get { return _LoadFromWebsiteFlag; }
            set
            {
                _LoadFromWebsiteFlag = value;
                NotifyOfPropertyChange(() => LoadFromWebsiteFlag);
            }
        }
        private long _WebIntPtDiagDrInstructionID;
        public long WebIntPtDiagDrInstructionID
        {
            get { return _WebIntPtDiagDrInstructionID; }
            set
            {
                _WebIntPtDiagDrInstructionID = value;
                NotifyOfPropertyChange(() => WebIntPtDiagDrInstructionID);
            }
        }
        public void LoadFromWebCmd()
        {
            if (CurRegistration == null || CurRegistration.PtRegistrationID == 0)
            {
                return;
            }
            IInstructionFromWeb mDialogView = Globals.GetViewModel<IInstructionFromWeb>();
            mDialogView.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            mDialogView.GetInstructionList();
            GlobalsNAV.ShowDialog_V3<IInstructionFromWeb>(mDialogView, null, null, false, true, new Size(420, 600));
        }
        public void Handle(LoadInstructionFromWebsite message)
        {
            if (message != null)
            {
                LoadFromWebsiteFlag = true;
                WebIntPtDiagDrInstructionID = message.IntPtDiagDrInstructionID;
                RenewCmd();
                LoadFromWebsiteFlag = false;
            }
        }
        public void LoadDataFromWeb(long IntPtDiagDrInstructionID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(IntPtDiagDrInstructionID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            mContract.EndGetDataFromWebForInstruction_ByIntPtDiagDrInstructionID(out string Disease_Progression, out string Diet, out long V_LevelCare
                                , out string PCLExamTypeList, out IList<RefMedicalServiceGroupItem> MedServiceList, out string DrugList, asyncResult);
                            if (!String.IsNullOrEmpty(Disease_Progression))
                            {
                                InPatientDailyDiagnosisContent.DiagnosisTreatmentContent.OrientedTreatment = Disease_Progression;
                            }
                            if (!String.IsNullOrEmpty(Diet))
                            {
                                MonitoringVitalSignsContent.gInPatientInstruction.Diet = Diet;
                                InPatientDailyDiagnosisContent.gInPatientInstruction.Diet = Diet;
                            }
                            if (V_LevelCare > 0)
                            {
                                MonitoringVitalSignsContent.gInPatientInstruction.V_LevelCare = Globals.AllLookupValueList.Where(x => x.LookupID == V_LevelCare).FirstOrDefault();
                                InPatientDailyDiagnosisContent.gInPatientInstruction.V_LevelCare = Globals.AllLookupValueList.Where(x=>x.LookupID == V_LevelCare).FirstOrDefault();
                            }
                            if (!String.IsNullOrEmpty(PCLExamTypeList))
                            {
                                MedicalInstructionContent.LoadPCLFromWeb(PCLExamTypeList);
                            }
                            if (MedServiceList != null)
                            {
                                MedicalInstructionContent.LoadServiceFromWeb(MedServiceList);
                            }
                            if (!String.IsNullOrEmpty(DrugList))
                            {
                                //CurrentDrugCollection = new ObservableCollection<ReqOutwardDrugClinicDeptPatient>();
                                string xmlString = "<root>" + DrugList +
                                //"<drug><gen_med_product_id>0</gen_med_product_id><m_dose>0</m_dose><a_dose>0</a_dose><e_dose>0</e_dose><n_dose>0</n_dose></drug>" +
                                "</root>";
                                XmlDocument xml = new XmlDocument();
                                xml.LoadXml(xmlString);
                                XmlNodeList nodeList =(xml.SelectNodes("root/drug"));
                                foreach (XmlNode elem in nodeList)
                                {
                                    GetGenMedProduct(Convert.ToInt64(elem.ChildNodes[0].InnerText),
                                         (float)Convert.ToDouble(elem.ChildNodes[1].InnerText),
                                         (float)Convert.ToDouble(elem.ChildNodes[2].InnerText),
                                         (float)Convert.ToDouble(elem.ChildNodes[3].InnerText),
                                         (float)Convert.ToDouble(elem.ChildNodes[4].InnerText));
                                }
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
                    }), null);
                }
            });
            t.Start();
        }
        private void GetGenMedProduct(long GenMedProductID, float MDose, float ADose, float EDose, float NDose)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new PharmacyMedDeptServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetRefGenMedProductDetails(GenMedProductID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            RefGenMedProductDetails refGenMed =  mContract.EndGetRefGenMedProductDetails(asyncResult);
                            if(refGenMed != null)
                            {
                                GetRouteOfAdministrationListForWebsite(refGenMed, 0, GenMedProductID, MDose, ADose, EDose, NDose);
                            }
                            else
                            {
                                //GetRouteOfAdministrationListForWebsite(refGenMed, 0, GenMedProductID, MDose, ADose, EDose, NDose);
                                //AddBlankRowIntoCurrentDrugCollection();
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
                    }), null);
                }
            });
            t.Start();
        }
        public void GetRouteOfAdministrationListForWebsite(RefGenMedProductDetails refGen, long DrugROAID, long DrugID, float MDose, float ADose, float EDose, float NDose)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRouteOfAdministrationList(DrugROAID, DrugID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetRouteOfAdministrationList(asyncResult);
                            if (results != null && results.Count>0)
                            {
                                refGen.ListRouteOfAdministration = new ObservableCollection<Lookup>();
                                var obsRMCT = new ObservableCollection<Lookup>();
                                foreach (var p in results)
                                {
                                    if (p.RouteOfAdministration == null)
                                    {
                                        p.RouteOfAdministration = new Lookup();
                                    }
                                    //p.RefMedicalConditionType.IsWarning = p.IsWarning;
                                    refGen.ListRouteOfAdministration.Add(p.RouteOfAdministration);
                                }
                                CurrentDrugCollection.Add(new ReqOutwardDrugClinicDeptPatient
                                {
                                    GenMedProductID = DrugID,
                                    RefGenericDrugDetail = refGen,
                                    MDose = MDose,
                                    MDoseStr = MDose.ToString(),
                                    ADose = ADose,
                                    ADoseStr = ADose.ToString(),
                                    EDose = EDose,
                                    EDoseStr = EDose.ToString(),
                                    NDose = NDose,
                                    NDoseStr = NDose.ToString(),
                                    PrescribedQty = (decimal)(MDose + ADose + EDose + NDose),
                                    DateTimeSelection = Globals.GetCurServerDateTime(),
                                    EntityState = EntityState.NEW,
                                    V_RouteOfAdministration = refGen.ListRouteOfAdministration.FirstOrDefault().LookupID,
                                    IsTruyenTinhMach = refGen.ListRouteOfAdministration.FirstOrDefault().LookupID == 61319,
                                    TransferRate = 0,
                                    V_TransferRateUnit = 0
                                });
                                CurrentDrugCollection = CurrentDrugCollection.OrderByDescending(x => x.GenMedProductID).ToObservableCollection();
                            }
                            else
                            {
                                CurrentDrugCollection.Add(new ReqOutwardDrugClinicDeptPatient
                                {
                                    GenMedProductID = DrugID,
                                    RefGenericDrugDetail = refGen,
                                    MDose = MDose,
                                    MDoseStr = MDose.ToString(),
                                    ADose = ADose,
                                    ADoseStr = ADose.ToString(),
                                    EDose = EDose,
                                    EDoseStr = EDose.ToString(),
                                    NDose = NDose,
                                    NDoseStr = NDose.ToString(),
                                    PrescribedQty = (decimal)(MDose + ADose + EDose + NDose),
                                    DateTimeSelection = Globals.GetCurServerDateTime(),
                                    EntityState = EntityState.NEW
                                });
                                CurrentDrugCollection = CurrentDrugCollection.OrderByDescending(x => x.GenMedProductID).ToObservableCollection();
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

                    }), null);

                }

            });
            t.Start();
        }
        private bool _FormIsEnabled = false;
        public bool FormIsEnabled
        {
            get { return _FormIsEnabled; }
            set
            {
                if (_FormIsEnabled != value)
                {
                    _FormIsEnabled = value;
                    NotifyOfPropertyChange(() => FormIsEnabled);
                }
            }
        }
        private bool _bShowContent;
        public bool bShowContent
        {
            get { return _bShowContent; }
            set
            {
                _bShowContent = value;
                NotifyOfPropertyChange(() => bShowContent);
            }
        }
        private void CheckDeptCanUseInstruction()
        {
            if(Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null)
            {
                return;
            }
            if (!IsDialogViewObject 
                && (Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null 
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails != null
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0 
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation != null
                && Globals.DeptLocation != null 
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation.DeptID != Globals.DeptLocation.DeptID))
            {
                FormIsEnabled = false;
                bShowContent = true;
            }
            else
            {
                FormIsEnabled = true;
                bShowContent = false;
            }
        }  
        private bool CheckRequiredTruyenTinhMach(string CurrentNotes)
        {
            string NoteWithNotRequired = Globals.ServerConfigSection.CommonItems.NotesKhongCheckTocDoTruyen; 
            foreach (var itemNotes in NoteWithNotRequired.Split(','))
            {
                if (CurrentNotes.Contains(itemNotes))
                {
                    return true;
                }
            }
            return false;
        }

        public void Handle(TreatmentProcessEvent.OnChangedTreatmentProcess message)
        {
            if(message != null && message.TreatmentProcess != null && message.TreatmentProcess.CurPatientRegistration != null)
            {
                OpenRegistration(message.TreatmentProcess.CurPatientRegistration.PtRegistrationID);
            }
        }
    }//▲===== 
    //CMN: Tạo lớp mới để kế thừa hiển thị Y lệnh theo Grouping format
    public class ClientReqOutwardDrugClinicDeptPatient : NotifyChangedBase
    {
        public ClientReqOutwardDrugClinicDeptPatient(ReqOutwardDrugClinicDeptPatient aReqOutwardDrugClinicDeptPatient, IList<ReqOutwardDrugClinicDeptPatient> AllReqOutwardDrug)
        {
            CurrentReqOutwardDrugClinicDeptPatient = aReqOutwardDrugClinicDeptPatient;
            CurrentIntravenousGroupingObject = new IntravenousGroupingObject();
            CurrentIntravenousGroupingObject.AllMedProductNameString = !AllReqOutwardDrug.Any(x => x.GroupOrdinal == aReqOutwardDrugClinicDeptPatient.GroupOrdinal && x.GenMedProductID > 0) ? "" : string.Join(",", AllReqOutwardDrug.Where(x => x.GroupOrdinal == aReqOutwardDrugClinicDeptPatient.GroupOrdinal && x.GenMedProductID > 0).Select(x => string.Format("{0} ({1:#,0.###})", x.RefGenericDrugDetail.BrandName, x.PrescribedQty)));
            var GroupContent = AllReqOutwardDrug.FirstOrDefault(x => x.GroupOrdinal == aReqOutwardDrugClinicDeptPatient.GroupOrdinal && x.StartDateTime.HasValue && x.StartDateTime.Value != null && x.StartDateTime.Value > new DateTime(2010, 1, 1));
            if (GroupContent != null)
            {
                CurrentIntravenousGroupingObject.GroupOrdinal = GroupContent.GroupOrdinal;
                CurrentIntravenousGroupingObject.NumOfTimes = GroupContent.NumOfTimes;
                CurrentIntravenousGroupingObject.FlowRate = GroupContent.FlowRate;
                CurrentIntravenousGroupingObject.StartDateTime = GroupContent.StartDateTime;
            }
            RaisePropertyChanged("CurrentIntravenousGroupingObject");
        }
        private ReqOutwardDrugClinicDeptPatient _CurrentReqOutwardDrugClinicDeptPatient;
        private IntravenousGroupingObject _CurrentIntravenousGroupingObject;
        public ReqOutwardDrugClinicDeptPatient CurrentReqOutwardDrugClinicDeptPatient
        {
            get
            {
                return _CurrentReqOutwardDrugClinicDeptPatient;
            }
            set
            {
                if (_CurrentReqOutwardDrugClinicDeptPatient == value)
                {
                    return;
                }
                _CurrentReqOutwardDrugClinicDeptPatient = value;
                RaisePropertyChanged("CurrentReqOutwardDrugClinicDeptPatient");
            }
        }
        public IntravenousGroupingObject CurrentIntravenousGroupingObject
        {
            get
            {
                return _CurrentIntravenousGroupingObject;
            }
            set
            {
                if (_CurrentIntravenousGroupingObject == value)
                {
                    return;
                }
                _CurrentIntravenousGroupingObject = value;
                RaisePropertyChanged("CurrentIntravenousGroupingObject");
            }
        }
    }
    //CMN: Tạo lớp chứa các thông tin để hiển thị Grouping Text
    public class IntravenousGroupingObject : NotifyChangedBase
    {
        private int _GroupOrdinal = 0;
        private string _NumOfTimes = "";
        private string _FlowRate;
        private DateTime? _StartDateTime;
        private string _AllMedProductNameString;
        public int GroupOrdinal
        {
            get
            {
                return _GroupOrdinal;
            }
            set
            {
                if (_GroupOrdinal == value)
                {
                    return;
                }
                _GroupOrdinal = value;
                RaisePropertyChanged("GroupOrdinal");
            }
        }
        public string NumOfTimes
        {
            get
            {
                return _NumOfTimes;
            }
            set
            {
                if (_NumOfTimes == value)
                {
                    return;
                }
                _NumOfTimes = value;
                RaisePropertyChanged("NumOfTimes");
            }
        }
        public string FlowRate
        {
            get
            {
                return _FlowRate;
            }
            set
            {
                if (_FlowRate == value)
                {
                    return;
                }
                _FlowRate = value;
                RaisePropertyChanged("FlowRate");
            }
        }
        public DateTime? StartDateTime
        {
            get
            {
                return _StartDateTime;
            }
            set
            {
                if (_StartDateTime == value)
                {
                    return;
                }
                _StartDateTime = value;
                RaisePropertyChanged("StartDateTime");
            }
        }
        public string AllMedProductNameString
        {
            get
            {
                return _AllMedProductNameString;
            }
            set
            {
                if (_AllMedProductNameString == value)
                {
                    return;
                }
                _AllMedProductNameString = value;
                RaisePropertyChanged("AllMedProductNameString");
            }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return !(obj is IntravenousGroupingObject) ? false : GroupOrdinal == (obj as IntravenousGroupingObject).GroupOrdinal;
        }
        public override string ToString()
        {
            return string.Format("{0:HH:mm dd/MM/yyyy}: {1} (x{2}): {3}", StartDateTime, FlowRate, NumOfTimes, AllMedProductNameString);
        }
    }
}
