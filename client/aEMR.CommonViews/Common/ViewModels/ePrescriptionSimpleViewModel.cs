using System.Linq;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using DataEntities;
using System.Threading;
using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using eHCMSLanguage;
using System.ComponentModel;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.Events;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.Common.Views;
using aEMR.CommonTasks;
using aEMR.ServiceClient;
using aEMR.ServiceClient.Consultation_PCLs;
using aEMR.Controls;
using aEMR.Common.Printing;
using aEMR.ViewContracts.Configuration;
using Service.Core.Common;
using System.Windows.Media;
using System.ServiceModel;
using aEMR.DataContracts;
/*
* 20171220 #001 CMN:    Fixed Print Empty Prescriptions
* 20180914 #002 TTM: 
* 20180919 #003 TTM: 
* 20180920 #004 TBL:    Chinh sua chuc nang bug mantis ID 0000061, thay doi thuoc tinh IsObjectBeingUsedByClient, IsDataChanged, IsPrescriptionChanged
* 20180921 #005 TBL:    Khong the xem in toa thuoc mau
* 20180922 #006 TBL:    BM 0000073. Them listICD10Codes vao BeginGetDrugsInTreatmentRegimen
* 20180924 #007 TBL:    BM 0000077. Tao toa thuoc moi dua tren cu khi luu thanh cong thi khong dc chinh sua
* 20180924 #008 TTM:
* 20180927 #009 TBL:    BM 0000095. In toa thuoc theo so lan. Neu co loi gi lien quan thi can phai coi lai
* 20180210 #010 TTM:    BM 0000112: Fix việc popup hẹn bệnh hiện lên khi chỉ lưu chẩn đoán chưa có đụng chạm gì đến toa thuốc. 
* 20181009 #011 TBL:    BM 0000115: Fix lại vấn đề khi muốn lưu CĐ thì lại báo chưa chọn thuốc để ra toa
* 20181016 #012 TBL:    BM 0002170: Tu lay thuoc theo phac do khi qua tab Ra toa
* 20181029 #013 TTM:    BM 0004199: Thêm cảnh báo nếu bác sĩ ra toa thuốc có thuốc ngoài danh mục cần phải có sự xác nhận của bệnh nhân
* 20181101 #014 TTM:    BM 0004220: Fix lỗi query liên tục mặc dù không có gì để in (Toa hướng thần) và bổ sung thêm toa TPCN/MP
* 20181112 #015 TBL:    BM 0005204: Lay phac do dieu tri theo list ICD10
* 20180114 #016 TTM:    BM 0006473: Thêm kiểm tra nếu toa đã được duyệt thì không được phép chỉnh sửa
* 20190613 #017 TNHX:   [BM0006826] Apply config AllowTimeUpdatePrescription for edit prescription
* 20190620 #018 TTM:    BM 0011857: Xây dựng màn hình thông tin thuốc.
* 20190708 #019 TBL:    BM 0011924: Hẹn tự động theo cấu hình AppointmentAuto, ParamAppointmentAuto
* 20190806 #020 TTM:    BM 0011843: Tạo mới dựa trên cũ những bệnh nhân đăng ký không bảo hiểm (trước đó có bảo hiểm) thì vẫn load thuốc trong danh mục bảo hiểm 
*                       => Xem in sai, ra toa sai.
* 20191005 #021 TBL:    BM 0017423: Quản lý hẹn bệnh ngày lễ
* 20191008 #022 TBL:    BM 0017428: Cảnh báo khi toa có thuốc trùng nhóm thuốc
* 20191011 #023 TTM:    BM 0017421: Thêm thư ký y khoa cho ra toa
* 20191011 #024 TBL:    BM 0017452: Trong toa thuốc có thuốc trùng nhau thì hiển thị tên thuốc lên thông báo
* 20191028 #025 TBL:    BM 0018501: Nếu chẩn đoán và toa thuốc không phải 1 cặp thì hiện đỏ xung quanh ngày khám của toa thuốc
* 20191115 #026 TBL:    BM 0019585: Hiển thị danh sách thuốc ngoài phác đồ, ngoài danh mục BHYT khi cảnh báo
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IePrescriptionSimple)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ePrescriptionSimpleViewModel : ViewModelBase, IePrescriptionSimple
        , IHandle<ReloadDataePrescriptionEvent>
        , IHandle<DiagnosisTreatmentSelectedEvent<DiagnosisTreatment>>
        , IHandle<PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int>>
        , IHandle<PatientChange>
    {
        [ImportingConstructor]
        public ePrescriptionSimpleViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            System.Diagnostics.Debug.WriteLine("========> ePrescriptionOldNewViewModel - Constructor");
            UCAllergiesWarningByPatientID = Globals.GetViewModel<IAllergiesWarning_ByPatientID>();
        }
        //==== 20161115 CMN Begin: Fix Choose reload handle
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            GetInitDataInfo();
        }
        //==== 20161115 End.
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        private void GetInitDataInfo()
        {
            authorization();
            DrugTypes = new ObservableCollection<Lookup>();
            ChooseDoses = new ObservableCollection<ChooseDose>();
            LatestePrecriptions = new Prescription();
            LieuDung = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
            DrugTypes = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_DrugType).ToObservableCollection();
            InitPatientInfo();
            PrescriptionNoteTemplates_GetAllIsActiveAdm();
            PrescriptionNoteTemplates_GetAllIsActiveItem();
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
        }

        ~ePrescriptionSimpleViewModel()
        {
            System.Diagnostics.Debug.WriteLine("========> ePrescriptionSimpleViewModel - Destructor");
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            if (this.GetView() != null)
            {
                ePrescriptionSimpleView thisView = (ePrescriptionSimpleView)view;
            }
        }

        public void AllowModifyPrescription()
        {
            CheckBeforePrescrip();
        }
        public void CheckBeforePrescrip(long PrescriptID = 0)
        {
            if (PrescriptID > 0)
            {
                allPrescriptionIssueHistory = Registration_DataStorage.PrescriptionIssueHistories;
                ObjDiagnosisTreatment_Current = Registration_DataStorage.PatientServiceRecordCollection[0].DiagnosisTreatments[0];
                GetPrescription(PrescriptID);
            }
        }
        public void LoadPrescriptionInfo(long PrescriptID = 0)
        {
            HisIDVisibility = RegistrationCoverByHI();
            LatestePrecriptions = new Prescription();
            NoiTru = IsBenhNhanNoiTru();
            BH = RegistrationCoverByHI();
            GetPrescriptionTypeList();
            loadPrescript = true;
            CheckBeforePrescrip(PrescriptID);
        }
        public void InitPatientInfo()
        {
            LoadPrescriptionInfo();
        }
        #region busy indicator
        public override bool IsProcessing
        {
            get
            {
                return false;
                /*
                return _isWaitingSaveDuocSiEdit
                    || _IsWaitingGetPrescriptionDetailsByPrescriptID
                    || _IsWaitingGetLatestPrescriptionByPtID
                    || _IsWaitingChooseDose
                    || _IsWaitingGetMedConditionByPtID
                    || _IsWaitingAddPrescriptIssueHistory
                    || _IsWaitingCapNhatToaThuoc
                    || _IsWaitingPrescriptions_UpdateDoctorAdvice
                    || _IsWaitingTaoThanhToaMoi
                    || _IsWaitingGetAllContrainIndicatorDrugs
                    || _IsWaitingGetDiagnosisTreatmentByPtID
                    || _IsWaitingPrescriptionNoteTemplates_GetAll;
                 * */
                //|| _loadPrescript;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_IsWaitingGetLatestPrescriptionByPtID)
                {
                    return eHCMSResources.Z1048_G1_TTinToaCuoi;
                }
                if (_IsWaitingGetPrescriptionTypes)
                {
                    return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.T2837_G1_LoaiToa);
                }
                if (_IsWaitingGetMedConditionByPtID)
                {
                    return eHCMSResources.K2871_G1_DangLoadDLieu;
                }
                if (_IsWaitingAddPrescriptIssueHistory)
                {
                    return eHCMSResources.Z1049_G1_PhatHanhLaiToa;
                }
                if (_IsWaitingCapNhatToaThuoc)
                {
                    return eHCMSResources.Z1050_G1_CNhatToa;
                }
                if (_IsWaitingPrescriptions_UpdateDoctorAdvice)
                {
                    return eHCMSResources.K1661_G1_CNhatLoiDan;
                }
                if (_IsWaitingTaoThanhToaMoi)
                {
                    return eHCMSResources.Z1051_G1_DangLuuToa;
                }
                if (_IsWaitingGetAllContrainIndicatorDrugs)
                {
                    return eHCMSResources.K2882_G1_DangTaiDLieu;
                }
                if (_IsWaitingPrescriptionNoteTemplates_GetAll)
                {
                    return eHCMSResources.Z1053_G1_DangLayDSLoiDan;
                }
                if (_IsWaitingMDAllergies_ByPatientID)
                {
                    return eHCMSResources.Z1054_G1_DangLayDSThuocDiUng;
                }
                return string.Empty;
            }
        }
        //▼====== #008: khai báo để sử dụng cho việc chuyển dữ liệu từ view con ra view cha (ConsultationSummary).
        private ICS_DataStorage _CS_DS = null;
        public ICS_DataStorage CS_DS
        {
            get
            {
                return _CS_DS;
            }
            set
            {
                _CS_DS = value;
            }
        }
        //▲====== #008
        private bool _loadPrescript = false;
        public bool loadPrescript
        {
            get { return _loadPrescript; }
            set
            {
                if (_loadPrescript != value)
                {
                    _loadPrescript = value;
                    NotifyWhenBusy();
                    NotifyOfPropertyChange(() => loadPrescript);
                    NotifyOfPropertyChange(() => IsProcessing);
                }
            }
        }

        private bool _IsWaitingMDAllergies_ByPatientID;
        public bool IsWaitingMDAllergies_ByPatientID
        {
            get { return _IsWaitingMDAllergies_ByPatientID; }
            set
            {
                if (_IsWaitingMDAllergies_ByPatientID != value)
                {
                    _IsWaitingMDAllergies_ByPatientID = value;
                    NotifyOfPropertyChange(() => IsWaitingMDAllergies_ByPatientID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingPrescriptionNoteTemplates_GetAll;
        public bool IsWaitingPrescriptionNoteTemplates_GetAll
        {
            get { return _IsWaitingPrescriptionNoteTemplates_GetAll; }
            set
            {
                if (_IsWaitingPrescriptionNoteTemplates_GetAll != value)
                {
                    _IsWaitingPrescriptionNoteTemplates_GetAll = value;
                    NotifyOfPropertyChange(() => IsWaitingPrescriptionNoteTemplates_GetAll);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetLatestPrescriptionByPtID;
        public bool IsWaitingGetLatestPrescriptionByPtID
        {
            get { return _IsWaitingGetLatestPrescriptionByPtID; }
            set
            {
                if (_IsWaitingGetLatestPrescriptionByPtID != value)
                {
                    _IsWaitingGetLatestPrescriptionByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetLatestPrescriptionByPtID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingChooseDose;
        public bool IsWaitingChooseDose
        {
            get { return _IsWaitingChooseDose; }
            set
            {
                if (_IsWaitingChooseDose != value)
                {
                    _IsWaitingChooseDose = value;
                    NotifyOfPropertyChange(() => IsWaitingChooseDose);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingGetPrescriptionTypes;
        public bool IsWaitingGetPrescriptionTypes
        {
            get { return _IsWaitingGetPrescriptionTypes; }
            set
            {
                if (_IsWaitingGetPrescriptionTypes != value)
                {
                    _IsWaitingGetPrescriptionTypes = value;
                    NotifyOfPropertyChange(() => IsWaitingGetPrescriptionTypes);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetMedConditionByPtID;
        public bool IsWaitingGetMedConditionByPtID
        {
            get { return _IsWaitingGetMedConditionByPtID; }
            set
            {
                if (_IsWaitingGetMedConditionByPtID != value)
                {
                    _IsWaitingGetMedConditionByPtID = value;
                    NotifyOfPropertyChange(() => IsWaitingGetMedConditionByPtID);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _IsWaitingAddPrescriptIssueHistory;
        public bool IsWaitingAddPrescriptIssueHistory
        {
            get { return _IsWaitingAddPrescriptIssueHistory; }
            set
            {
                if (_IsWaitingAddPrescriptIssueHistory != value)
                {
                    _IsWaitingAddPrescriptIssueHistory = value;
                    NotifyOfPropertyChange(() => IsWaitingAddPrescriptIssueHistory);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingCapNhatToaThuoc;
        public bool IsWaitingCapNhatToaThuoc
        {
            get { return _IsWaitingCapNhatToaThuoc; }
            set
            {
                if (_IsWaitingCapNhatToaThuoc != value)
                {
                    _IsWaitingCapNhatToaThuoc = value;
                    NotifyOfPropertyChange(() => IsWaitingCapNhatToaThuoc);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingPrescriptions_UpdateDoctorAdvice;
        public bool IsWaitingPrescriptions_UpdateDoctorAdvice
        {
            get { return _IsWaitingPrescriptions_UpdateDoctorAdvice; }
            set
            {
                if (_IsWaitingPrescriptions_UpdateDoctorAdvice != value)
                {
                    _IsWaitingPrescriptions_UpdateDoctorAdvice = value;
                    NotifyOfPropertyChange(() => IsWaitingPrescriptions_UpdateDoctorAdvice);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingTaoThanhToaMoi;
        public bool IsWaitingTaoThanhToaMoi
        {
            get { return _IsWaitingTaoThanhToaMoi; }
            set
            {
                if (_IsWaitingTaoThanhToaMoi != value)
                {
                    _IsWaitingTaoThanhToaMoi = value;
                    NotifyOfPropertyChange(() => IsWaitingTaoThanhToaMoi);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _IsWaitingGetAllContrainIndicatorDrugs;
        public bool IsWaitingGetAllContrainIndicatorDrugs
        {
            get { return _IsWaitingGetAllContrainIndicatorDrugs; }
            set
            {
                if (_IsWaitingGetAllContrainIndicatorDrugs != value)
                {
                    _IsWaitingGetAllContrainIndicatorDrugs = value;
                    NotifyOfPropertyChange(() => IsWaitingGetAllContrainIndicatorDrugs);
                    NotifyWhenBusy();
                }
            }
        }
        private ObservableCollection<Lookup> _DrugTypes;
        public ObservableCollection<Lookup> DrugTypes
        {
            get
            {
                return _DrugTypes;
            }
            set
            {
                if (_DrugTypes != value)
                {
                    _DrugTypes = value;
                    NotifyOfPropertyChange(() => DrugTypes);
                }
            }
        }

        private ObservableCollection<PrescriptionDetailSchedulesLieuDung> _LieuDung;
        public ObservableCollection<PrescriptionDetailSchedulesLieuDung> LieuDung
        {
            get
            {
                return _LieuDung;
            }
            set
            {
                if (_LieuDung != value)
                {
                    _LieuDung = value;
                    NotifyOfPropertyChange(() => _LieuDung);
                }
            }
        }

        private PhysicalExamination _curPhysicalExamination;
        public PhysicalExamination curPhysicalExamination
        {
            get { return _curPhysicalExamination; }
            set
            {
                if (_curPhysicalExamination != value)
                {
                    _curPhysicalExamination = value;
                    NotifyWhenBusy();
                    NotifyOfPropertyChange(() => curPhysicalExamination);
                }
            }
        }

        private bool _PreNoDrug;
        public bool PreNoDrug
        {
            get { return _PreNoDrug; }
            set
            {
                if (_PreNoDrug != value)
                {
                    /*▼====: #004*/
                    LatestePrecriptions.SetDataChanged();
                    /*▲====: #004*/
                    _PreNoDrug = value;
                    NotifyOfPropertyChange(() => PreNoDrug);
                }
            }
        }

        private bool _isHisID;
        public bool isHisID
        {
            get { return _isHisID; }
            set
            {
                if (_isHisID != value)
                {
                    _isHisID = value;
                    NotifyOfPropertyChange(() => isHisID);
                }
            }
        }

        private bool _HisIDVisibility = false;
        public bool HisIDVisibility
        {
            get { return _HisIDVisibility; }
            set
            {
                if (_HisIDVisibility != value)
                {
                    _HisIDVisibility = value;
                    NotifyOfPropertyChange(() => HisIDVisibility);
                }
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _DonViTinh;
        public ObservableCollection<PrescriptionNoteTemplates> DonViTinh
        {
            get { return _DonViTinh; }
            set
            {
                _DonViTinh = value;
                NotifyOfPropertyChange(() => DonViTinh);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _CachDung;
        public ObservableCollection<PrescriptionNoteTemplates> CachDung
        {
            get { return _CachDung; }
            set
            {
                _CachDung = value;
                NotifyOfPropertyChange(() => CachDung);
            }
        }

        private ObservableCollection<PrescriptionNoteTemplates> _GhiChu;
        public ObservableCollection<PrescriptionNoteTemplates> GhiChu
        {
            get { return _GhiChu; }
            set
            {
                _GhiChu = value;
                NotifyOfPropertyChange(() => GhiChu);
            }
        }
        #endregion

        private IePrescriptionCommentaryAutoComplete _ePreComAutoCompleteContent;

        public IePrescriptionCommentaryAutoComplete ePreComAutoCompleteContent
        {
            get { return _ePreComAutoCompleteContent; }
            set
            {
                if (_ePreComAutoCompleteContent != value)
                {
                    _ePreComAutoCompleteContent = value;
                    NotifyOfPropertyChange(() => ePreComAutoCompleteContent);
                }
            }
        }
        //▼===== 20190620 TTM: Do có bổ sung vào cột thông tin thuốc ở đầu Grid nên cần phải thay đổi lại
        //                     index của các cột. Bổ sung thêm cột INFORMATION. Vì nếu không thay đổi sẽ dẫn đến
        //                     Không cập nhật số lượng khi thay đổi liều và ngày dùng.
        public enum DataGridCol
        {
            INFORMATION = 0,
            DEL = INFORMATION + 1,
            //Down = DEL + 1,
            //Up = Down + 1,
            HI = DEL + 1,
            Schedule = HI + 1,
            //DrugNotInCat = Schedule+1,
            NotInCat = Schedule + 1,
            DRUG_NAME = NotInCat + 1,
            //STRENGHT =DRUG_NAME+ 1,
            UNITS = DRUG_NAME + 1,
            UNITUSE = UNITS + 1,
            DRUG_TYPE = UNITUSE + 1,

            //DOSAGE = DRUG_TYPE + 1,
            //CHOOSE = DOSAGE + 1,

            //▼===== #032
            //MDOSE = DRUG_TYPE + 1,
            //NDOSE = MDOSE + 1,
            //ADOSE = NDOSE + 1,
            //EDOSE = ADOSE + 1,

            MDOSE = DRUG_TYPE + 1,
            ADOSE = MDOSE + 1,
            EDOSE = ADOSE + 1,
            NDOSE = EDOSE + 1,
            //▲===== #032

            DayTotalCol = NDOSE + 1,
            //DaytExtended = Dayts+1,

            QTY = DayTotalCol + 1,
            //UNITUSE = QTY+1,
            USAGE = QTY + 1,
            INSTRUCTION = USAGE + 1
        }
        public List<long> lstMCTypeID;

        public IAllergiesWarning_ByPatientID UCAllergiesWarningByPatientID
        {
            get;
            set;
        }

        private int xNgayBHToiDa_NgoaiTru = 30;
        private int xNgayBHToiDa_NoiTru = 5;
        private int xNgayBHToiDa = 0;
        private long? StoreID = 2;//tam thoi mac dinh kho ban(nha thuoc benh vien)
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            if (paymentTypeTask != null && paymentTypeTask.LookupList != null)
            {
                StoreID = paymentTypeTask.LookupList.Where(x => x.IsMain == true).FirstOrDefault().StoreID;
                if (StoreID != null && StoreID > 0)
                {
                    yield break;
                }
                else
                {
                    StoreID = paymentTypeTask.LookupList.FirstOrDefault().StoreID;
                }
            }
            yield break;
        }

        public void Handle(PatientChange obj)
        {
            //Initialize();
        }
        #region Properties member
        private DiagnosisTreatment _ObjDiagnosisTreatment_Current;
        public DiagnosisTreatment ObjDiagnosisTreatment_Current
        {
            get
            {
                return _ObjDiagnosisTreatment_Current;
            }
            set
            {
                if (_ObjDiagnosisTreatment_Current != value)
                {
                    _ObjDiagnosisTreatment_Current = value;
                    NotifyOfPropertyChange(() => ObjDiagnosisTreatment_Current);
                }
            }
        }

        private ObservableCollection<ChooseDose> _ChooseDoses;
        public ObservableCollection<ChooseDose> ChooseDoses
        {
            get
            {
                return _ChooseDoses;
            }
            set
            {
                if (_ChooseDoses != value)
                {
                    _ChooseDoses = value;
                    NotifyOfPropertyChange(() => ChooseDoses);
                }
            }
        }


        private ObservableCollection<PrescriptionIssueHistory> _allPrescriptionIssueHistory;
        public ObservableCollection<PrescriptionIssueHistory> allPrescriptionIssueHistory
        {
            get
            {
                return _allPrescriptionIssueHistory;
            }
            set
            {
                if (_allPrescriptionIssueHistory != value)
                {
                    _allPrescriptionIssueHistory = value;
                    NotifyOfPropertyChange(() => allPrescriptionIssueHistory);
                }
            }
        }
        //private PrescriptionDetail SelectedPrescriptionDetailCopy;

        private PrescriptionDetail _SelectedPrescriptionDetail;
        public PrescriptionDetail SelectedPrescriptionDetail
        {
            get
            {
                return _SelectedPrescriptionDetail;
            }
            set
            {
                if (_SelectedPrescriptionDetail != value)
                {
                    _SelectedPrescriptionDetail = value;
                    NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
                }
            }
        }

        private PrescriptionDetail _PrescriptionDetailForForm;
        public PrescriptionDetail ObjPrescriptionDetailForForm
        {
            get
            {
                return _PrescriptionDetailForForm;
            }
            set
            {
                if (_PrescriptionDetailForForm != value)
                {
                    _PrescriptionDetailForForm = value;
                    NotifyOfPropertyChange(() => ObjPrescriptionDetailForForm);
                }
            }
        }

        private PagedSortableCollectionView<GetDrugForSellVisitor> _Drugs;
        public PagedSortableCollectionView<GetDrugForSellVisitor> DrugList
        {
            get
            {
                return _Drugs;
            }
            set
            {
                if (_Drugs != value)
                {
                    _Drugs = value;
                    NotifyOfPropertyChange(() => DrugList);
                }
            }
        }

        private ObservableCollection<Staff> _StaffCatgs;
        public ObservableCollection<Staff> StaffCatgs
        {
            get
            {
                return _StaffCatgs;
            }
            set
            {
                if (_StaffCatgs != value)
                {
                    _StaffCatgs = value;
                    NotifyOfPropertyChange(() => StaffCatgs);
                }
            }
        }

        private Prescription _LatestePrecriptions;
        public Prescription LatestePrecriptions
        {
            get
            {
                return _LatestePrecriptions;
            }
            set
            {
                if (_LatestePrecriptions != value)
                {
                    _LatestePrecriptions = value;
                    /*▼====: #004*/
                    if (_LatestePrecriptions.PtRegistrationID > 0)
                    {
                        LatestePrecriptions.IsObjectBeingUsedByClient = true;
                    }
                    /*▲====: #004*/
                    NotifyOfPropertyChange(() => LatestePrecriptions);
                    if (_LatestePrecriptions != null)
                    {
                        if (_LatestePrecriptions.PrescriptionIssueHistory == null)
                        {
                            _LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
                        }

                        if (HisIDVisibility)
                        {
                            if (Registration_DataStorage.PatientServiceRecordCollection != null
                                && Registration_DataStorage.PatientServiceRecordCollection.Count > 0
                                && Registration_DataStorage.PatientServiceRecordCollection[0] != null
                                && Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories.Count > 0
                                && Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0] != null)
                            {
                                isHisID = Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].HisID > 0 ? true : false;
                                //KMx: Nếu để dòng bên dưới. Sau khi Cập nhật toa bảo hiểm sẽ bị mất dấu check trong checkbox "Toa bảo hiểm".
                                //LatestePrecriptions.PrescriptionIssueHistory.HisID = 0;
                            }
                            else
                            {
                                isHisID = true;
                                LatestePrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration == null ? 0 : Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
                            }
                        }
                        else
                        {
                            isHisID = false;
                            LatestePrecriptions.PrescriptionIssueHistory.HisID = 0;
                        }
                    }

                }
            }
        }

        private Prescription _PrecriptionsForPrint;
        public Prescription PrecriptionsForPrint
        {
            get
            {
                return _PrecriptionsForPrint;
            }
            set
            {
                if (_PrecriptionsForPrint != value)
                {
                    _PrecriptionsForPrint = value;
                    NotifyOfPropertyChange(() => PrecriptionsForPrint);
                }
            }
        }

        private int _NumOfDays = 7;
        public int NumOfDays
        {
            get
            {
                return _NumOfDays;
            }
            set
            {
                if (_NumOfDays != value)
                {
                    _NumOfDays = value;
                    NotifyOfPropertyChange(() => NumOfDays);
                }
            }
        }

        private ObservableCollection<Lookup> _PrescriptionType;
        public ObservableCollection<Lookup> PrescriptionTypeList
        {
            get
            {
                return _PrescriptionType;
            }
            set
            {
                if (_PrescriptionType != value)
                {
                    _PrescriptionType = value;
                    NotifyOfPropertyChange(() => PrescriptionTypeList);
                }
            }
        }

        private Lookup _CurrentPrescriptionType;
        public Lookup CurrentPrescriptionType
        {
            get
            {
                return _CurrentPrescriptionType;
            }
            set
            {
                if (_CurrentPrescriptionType != value)
                {
                    _CurrentPrescriptionType = value;
                    NotifyOfPropertyChange(() => CurrentPrescriptionType);
                }
            }
        }

        private bool _BH;
        public bool BH
        {
            get
            {
                return _BH;
            }
            set
            {
                _BH = value;
                NotifyOfPropertyChange(() => BH);
            }
        }

        private bool _NoiTru;
        public bool NoiTru
        {
            get
            {
                return _NoiTru;
            }
            set
            {
                _NoiTru = value;
                NotifyOfPropertyChange(() => NoiTru);
            }
        }


        private bool _IsEnabledPrint = true;
        public bool IsEnabledPrint
        {
            get
            {
                return _IsEnabledPrint;
            }
            set
            {
                if (_IsEnabledPrint != value)
                {
                    _IsEnabledPrint = value;
                    NotifyOfPropertyChange(() => IsEnabledPrint);
                }
            }
        }


        private bool _CanPrint;
        public bool CanPrint
        {
            get
            {
                return _CanPrint;
            }
            set
            {
                if (_CanPrint != value)
                {
                    _CanPrint = value;
                    //CanUndo = !CanPrint;
                    NotifyOfPropertyChange(() => CanPrint);
                }
            }
        }

        private int _ClassificationPatient;
        public int ClassificationPatient
        {
            get
            {
                return _ClassificationPatient;
            }
            set
            {
                _ClassificationPatient = value;
                NotifyOfPropertyChange(() => ClassificationPatient);
            }
        }

        private ObservableCollection<MedicalConditionRecord> _PtMedCond;
        public ObservableCollection<MedicalConditionRecord> PtMedCond
        {
            get
            {
                return _PtMedCond;
            }
            set
            {
                if (_PtMedCond == value)
                    return;
                _PtMedCond = value;
                NotifyOfPropertyChange(() => PtMedCond);
            }
        }

        private List<Prescription> _LstPrescription_TrongNgay;
        public List<Prescription> LstPrescription_TrongNgay
        {
            get
            {
                return _LstPrescription_TrongNgay;
            }
            set
            {
                if (_LstPrescription_TrongNgay != value)
                {
                    _LstPrescription_TrongNgay = value;
                    NotifyOfPropertyChange(() => LstPrescription_TrongNgay);
                }
            }
        }

        private Prescription _Prescriptions_TrongNgay;
        public Prescription Prescriptions_TrongNgay
        {
            get
            {
                return _Prescriptions_TrongNgay;
            }
            set
            {
                if (_Prescriptions_TrongNgay != value)
                {
                    _Prescriptions_TrongNgay = value;
                    NotifyOfPropertyChange(() => Prescriptions_TrongNgay);
                }
            }
        }

        private List<IList<PrescriptionDetail>> _ListPrescriptionDetailTrongNgay;
        public List<IList<PrescriptionDetail>> ListPrescriptionDetailTrongNgay
        {
            get
            {
                return _ListPrescriptionDetailTrongNgay;
            }
            set
            {
                if (_ListPrescriptionDetailTrongNgay != value)
                {
                    _ListPrescriptionDetailTrongNgay = value;
                    NotifyOfPropertyChange(() => ListPrescriptionDetailTrongNgay);
                }
            }
        }
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region account checking

        private bool _mToaThuocDaPhatHanh_ThongTin = true;
        private bool _mToaThuocDaPhatHanh_ChinhSua = true;
        private bool _mToaThuocDaPhatHanh_TaoToaMoi = true;
        private bool _mToaThuocDaPhatHanh_PhatHanhLai = true;
        private bool _mToaThuocDaPhatHanh_In = true;
        private bool _mToaThuocDaPhatHanh_ChonChanDoan = true;
        private bool _hasTitle = true;

        public bool mToaThuocDaPhatHanh_ThongTin
        {
            get
            {
                return _mToaThuocDaPhatHanh_ThongTin;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_ThongTin == value)
                    return;
                _mToaThuocDaPhatHanh_ThongTin = value;
            }
        }
        public bool mToaThuocDaPhatHanh_ChinhSua
        {
            get
            {
                return _mToaThuocDaPhatHanh_ChinhSua;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_ChinhSua == value)
                    return;
                _mToaThuocDaPhatHanh_ChinhSua = value;
            }
        }
        public bool mToaThuocDaPhatHanh_TaoToaMoi
        {
            get
            {
                return _mToaThuocDaPhatHanh_TaoToaMoi;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_TaoToaMoi == value)
                    return;
                _mToaThuocDaPhatHanh_TaoToaMoi = value;
            }
        }
        public bool mToaThuocDaPhatHanh_PhatHanhLai
        {
            get
            {
                return _mToaThuocDaPhatHanh_PhatHanhLai;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_PhatHanhLai == value)
                    return;
                _mToaThuocDaPhatHanh_PhatHanhLai = value;
            }
        }
        public bool mToaThuocDaPhatHanh_In
        {
            get
            {
                return _mToaThuocDaPhatHanh_In;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_In == value)
                    return;
                _mToaThuocDaPhatHanh_In = value;
            }
        }
        public bool mToaThuocDaPhatHanh_ChonChanDoan
        {
            get
            {
                return _mToaThuocDaPhatHanh_ChonChanDoan;
            }
            set
            {
                if (_mToaThuocDaPhatHanh_ChonChanDoan == value)
                    return;
                _mToaThuocDaPhatHanh_ChonChanDoan = value;
            }
        }
        public bool hasTitle
        {
            get
            {
                return _hasTitle;
            }
            set
            {
                if (_hasTitle == value)
                    return;
                _hasTitle = value;
            }
        }
        #endregion


        private bool _ContentKhungTaiKhamIsEnabled = true;
        public bool ContentKhungTaiKhamIsEnabled
        {
            get
            {
                return _ContentKhungTaiKhamIsEnabled;
            }
            set
            {
                if (_ContentKhungTaiKhamIsEnabled != value)
                {
                    _ContentKhungTaiKhamIsEnabled = value;
                    NotifyOfPropertyChange(() => ContentKhungTaiKhamIsEnabled);
                }
            }
        }

        #region service function
        public void GetPrescriptionDetailsByPrescriptID(long prescriptID, bool GetRemaining = false)
        {
            //IsWaitingGetPrescriptionDetailsByPrescriptID = true;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, GetRemaining, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
                            LatestePrecriptions.PrescriptionDetails = Results.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            //IsWaitingGetPrescriptionDetailsByPrescriptID = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetMedConditionByPtID(long patientID, int mcTypeID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });
            this.ShowBusyIndicator();
            IsWaitingGetMedConditionByPtID = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ComRecordsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetMedConditionByPtID(patientID, mcTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {

                            var items = contract.EndGetMedConditionByPtID(asyncResult);
                            if (items != null)
                            {
                                PtMedCond = new ObservableCollection<MedicalConditionRecord>(items);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            IsWaitingGetMedConditionByPtID = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion

        #region minifunction

        public object GetChooseDose(object value)
        {
            PrescriptionDetail p = value as PrescriptionDetail;
            if (p != null)
            {
                return p.ChooseDose;
            }
            else
            {
                return null;
            }
        }
        private bool BlankDrugLineAlreadyExist()
        {
            if (LatestePrecriptions.PrescriptionDetails == null)
                return true;

            int nCount = LatestePrecriptions.PrescriptionDetails.Count;
            if (nCount == 0)
                return false;

            if (nCount > 0)
            {
                // Txd 12/10/2013 : The current Last line has been selected with DrugID or it is a Drug outside of Catalog
                PrescriptionDetail LastPrescriptDetail = LatestePrecriptions.PrescriptionDetails[nCount - 1];
                if (LastPrescriptDetail.DrugID > 0 || (LastPrescriptDetail.IsDrugNotInCat && !string.IsNullOrEmpty(LastPrescriptDetail.SelectedDrugForPrescription.BrandName)))
                {
                    return false;
                }
            }
            return true;
        }

        public void AddNewBlankDrugIntoPrescriptObjectNew()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (LatestePrecriptions == null)
                {
                    LatestePrecriptions = new Prescription();
                }

                if (LatestePrecriptions.PrescriptionDetails == null)
                {
                    LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
                }

                if (BlankDrugLineAlreadyExist())
                {
                    return;
                }

                PrescriptionDetail prescriptDObj = NewReInitPrescriptionDetail(false, null);

                LatestePrecriptions.PrescriptionDetails.Add(prescriptDObj);
                NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);

                //20180918 TBL: Tao dong rong khong can phai gan gia tri
                //ObjPrescriptionDetailForForm = NewReInitPrescriptionDetail(true, null);
                if (LatestePrecriptions.PrescriptionDetails.Count > 1)
                {
                    LatestePrecriptions.PreNoDrug = false;
                    NotifyOfPropertyChange(() => LatestePrecriptions.PreNoDrug);
                }
            });
        }


        private PrescriptionDetail NewReInitPrescriptionDetail(bool bForm, PrescriptionDetail existingPrescriptObj, bool bOnlyInitObj = false)
        {
            PrescriptionDetail prescriptDObj = existingPrescriptObj;
            if (!bOnlyInitObj)
            {
                if (prescriptDObj == null)
                {
                    prescriptDObj = new PrescriptionDetail();
                    /*▼====: #004*/
                    prescriptDObj.IsObjectBeingUsedByClient = true;
                    /*▲====: #004*/
                }
                prescriptDObj.isForm = bForm;
                prescriptDObj.SelectedDrugForPrescription = new GetDrugForSellVisitor();
                prescriptDObj.DrugID = 0;
                if (BH)
                {
                    prescriptDObj.BeOfHIMedicineList = true;
                    prescriptDObj.InsuranceCover = true;
                }
                else
                {
                    prescriptDObj.BeOfHIMedicineList = false;
                    prescriptDObj.InsuranceCover = false;
                }
                prescriptDObj.Index = LatestePrecriptions.PrescriptionDetails.Count;
            }

            prescriptDObj.IsInsurance = null;
            prescriptDObj.Strength = "";
            prescriptDObj.Qty = 0;
            prescriptDObj.MDoseStr = "0";
            prescriptDObj.ADoseStr = "0";
            prescriptDObj.EDoseStr = "0";
            prescriptDObj.NDoseStr = "0";
            prescriptDObj.DrugType = new Lookup
            {
                LookupID = (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG,
                ObjectValue = eHCMSResources.T0748_G1_T.ToUpper()
            };
            prescriptDObj.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG;

            int nDayVal = LatestePrecriptions.NDay == null ? 0 : LatestePrecriptions.NDay.Value;
            SetDefaultDay(prescriptDObj, nDayVal);
            //GetDayRptNormal(prescriptDObj, xNgayBHToiDa, nDayVal);

            prescriptDObj.DrugInstructionNotes = "";
            prescriptDObj.Administration = "";
            if (LatestePrecriptions.PrescriptionDetails != null && LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                prescriptDObj.PrescriptDetailID = LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].PrescriptDetailID + 1;
            }
            else
            {
                prescriptDObj.PrescriptDetailID = 0;
            }
            //20180918 TBL: Anh Tuan keu khog dc tu DeepCopy chinh no, chung nao chuong trinh co van de thi moi quay lai xem xet
            //prescriptDObj = ObjectCopier.DeepCopy(prescriptDObj);

            return prescriptDObj;
        }

        /// <summary>
        /// Cap nhat lai ngay va so luong trong toa thuoc hien hanh
        /// </summary>

        private void SetDefaultDay(PrescriptionDetail item, int nDayTotal)
        {
            item.DayRpts = nDayTotal;
            item.DayExtended = 0;
            item.RealDay = nDayTotal;
        }

        private float CalQtyForDay(PrescriptionDetail drugItem)
        {
            if (drugItem == null || drugItem.SelectedDrugForPrescription == null)
            {
                return 0;
            }

            float QtyAllDose = 0;
            float Result = 0;

            QtyAllDose = drugItem.MDose + drugItem.ADose.GetValueOrDefault() + drugItem.NDose.GetValueOrDefault() + drugItem.EDose.GetValueOrDefault();

            Result = QtyAllDose / ((float)drugItem.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : (float)drugItem.SelectedDrugForPrescription.DispenseVolume);

            return Result;
        }

        private float CalQtyForNormalDrug(PrescriptionDetail drugItem, int nNumDayPrescribed)
        {
            if (drugItem == null || drugItem.SelectedDrugForPrescription == null)
            {
                return 0;
            }

            float QtyAllDose = 0;
            float Result = 0;

            QtyAllDose = drugItem.MDose + drugItem.ADose.GetValueOrDefault() + drugItem.NDose.GetValueOrDefault() + drugItem.EDose.GetValueOrDefault();

            //KMx: Phải nhân trước rồi chia sau để hạn chế kết quả có số lẻ (06/11/2014 11:11).
            Result = (QtyAllDose * nNumDayPrescribed) / ((float)drugItem.SelectedDrugForPrescription.DispenseVolume == 0 ? 1 : (float)drugItem.SelectedDrugForPrescription.DispenseVolume);

            //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
            return (float)Math.Ceiling(Math.Round(Result, 2));
        }

        private void CalcTotalQtyForDrugItem(PrescriptionDetail drugItem)
        {
            if (drugItem.HasSchedules)
            {
                // Only calculate for item without Weekly Taking Schedule (Lich Tuan)
                return;
            }

            if (drugItem == null || drugItem.SelectedDrugForPrescription == null)
            {
                return;
            }

            drugItem.Qty = CalQtyForNormalDrug(drugItem, drugItem.RealDay);

            //Hàm này chỉ dùng cho thuốc thường, cần và phải nằm trong DMBH, nếu như có thuốc ngoài DM lọt vào hàm này thì chỉ tính Qty rồi return (13/06/2014 15:14).
            if (drugItem.IsDrugNotInCat)
            {
                return;
            }

            //Thuốc cần.
            //KMx: Nếu là thuốc cần thì QtyMaxAllowed = Qty (A.Tuấn quyết định) (05/06/2014 16:61).
            if (drugItem.isNeedToUse)
            {
                drugItem.QtyMaxAllowed = drugItem.Qty;
                return;
            }

            //Thuốc thường.
            drugItem.QtyForDay = CalQtyForDay(drugItem);

            //KMx: Tính số lượng thuốc (thuốc thường) mà BH đồng ý chi trả (05/06/2014 14:00).
            if (drugItem.RealDay <= xNgayBHToiDa)
            {
                drugItem.QtyMaxAllowed = drugItem.Qty;
            }
            else
            {
                drugItem.QtyMaxAllowed = CalQtyForNormalDrug(drugItem, xNgayBHToiDa);
            }
        }


        #endregion
        private bool _chkHasAppointmentValue;
        public bool chkHasAppointmentValue
        {
            get
            {
                return _chkHasAppointmentValue;
            }
            set
            {
                if (_chkHasAppointmentValue != value)
                {
                    _chkHasAppointmentValue = value;

                    NotifyOfPropertyChange(() => chkHasAppointmentValue);
                    //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);
                }
            }
        }

        private bool CheckValidationEditor1Row(PrescriptionDetail item)
        {
            if (item != null
                && item.SelectedDrugForPrescription != null
                && item.SelectedDrugForPrescription.BrandName != ""
                )
            {
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
        }
        // TxD 11/04/2014: Added the following method to replace methods: CheckRegHasHI & IsPatientInsurance
        //                  and uniform the checking for HI Cover of Patient's Registration
        private bool RegistrationCoverByHI()
        {
            if (Registration_DataStorage.CurrentPatientRegistrationDetail == null)
            {
                return false;
            }
            //20190330 TBL: Tick toa bao hiem se dua tren HisID cua dich vu chu khong dua vao cua dang ky nua
            //if (Registration_DataStorage.CurrentPatientRegistration.HisID.HasValue && Registration_DataStorage.CurrentPatientRegistration.HisID.Value > 0)
            //{
            //    return true;
            //}
            if (Registration_DataStorage.CurrentPatientRegistrationDetail.HisID.HasValue && Registration_DataStorage.CurrentPatientRegistrationDetail.HisID.Value > 0)
            {
                return true;
            }
            return false;
        }
        #region button member

        //Khóa mở S,Tr,C,T TextBox theo Combo chọn
        public void cbxChooseDose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
            if (Ctr != null)
            {
                if (Ctr.SelectedItemEx != null)
                {
                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

                    if (Objtmp != null && Objtmp.HasSchedules == true)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                        return;
                    }

                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;
                    SetEnableDisalbeInputDose(ObjChooseDose, Objtmp);

                    if (ObjChooseDose.ID > 0)
                    {
                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
                        if (Objtmp != null && Objtmp.DayRpts > 0)
                        {
                            CalcTotalQtyForDrugItem(Objtmp);
                        }
                    }
                }
            }
        }

        public void cbxChooseDoseForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
            if (Ctr != null)
            {
                if (Ctr.SelectedItemEx != null)
                {
                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

                    if (Objtmp != null && Objtmp.HasSchedules == true)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
                        return;
                    }

                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;
                    SetEnableDisalbeInputDose(ObjChooseDose, Objtmp);

                    if (ObjChooseDose.ID > 0)
                    {
                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
                        if (Objtmp != null && Objtmp.DayRpts > 0)
                        {
                            CalcTotalQtyForDrugItem(Objtmp);
                        }
                    }
                }
            }
        }

        private bool IsBenhNhanNoiTru()
        {
            //cho nay can coi lai vi ben nha thuoc sua toa thuoc se khong co RegistrationInfo
            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatient.PatientID > 0)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType ==
                    AllLookupValues.RegistrationType.NOI_TRU)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        private void SetEnableDisalbeInputDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
        {
            if (grdPrescription != null)
            {
                int indexRow = grdPrescription.SelectedIndex;
                if (indexRow < 0)
                {
                    return;
                }

                AxTextBox Sang = grdPrescription.Columns[(int)DataGridCol.MDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
                AxTextBox Trua = grdPrescription.Columns[(int)DataGridCol.NDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
                AxTextBox Chieu = grdPrescription.Columns[(int)DataGridCol.ADOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
                AxTextBox Toi = grdPrescription.Columns[(int)DataGridCol.EDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;

                if (indexRow >= 0)
                {
                    if (Objtmp != null)
                    {
                        if (Objtmp.ChooseDose != null)
                        {
                            if (Objtmp.dosage <= 0)
                            {
                                Objtmp.dosage = 0;
                            }
                            switch (ObjChooseDose.ID)
                            {
                                case 1:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 2:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 3:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 4:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 5:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 6:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = 0;
                                    break;

                                case 7:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 8:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 9:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 10:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 11:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;

                                case 12:
                                    Objtmp.MDose = Objtmp.dosage;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = 0;
                                    break;

                                case 13:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = Objtmp.dosage;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = 0;
                                    break;

                                case 14:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = Objtmp.dosage;
                                    Objtmp.NDose = 0;
                                    break;

                                case 15:
                                    Objtmp.MDose = 0;
                                    Objtmp.ADose = 0;
                                    Objtmp.EDose = 0;
                                    Objtmp.NDose = Objtmp.dosage;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void SetValueFollowComboDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
        {
            if (Objtmp != null)
            {
                if (ObjChooseDose != null)
                {
                    Objtmp.MDoseStr = "0";
                    Objtmp.NDoseStr = "0";
                    Objtmp.ADoseStr = "0";
                    Objtmp.EDoseStr = "0";
                    switch (ObjChooseDose.ID)
                    {
                        case 1://S

                            Objtmp.MDoseStr = Objtmp.dosageStr;
                            break;
                        case 2://Tr 
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 3://C
                            {
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 4://T
                            {
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 5://S Tr
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 6://S C
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 7://S T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }

                        case 8://Tr C
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 9://Tr T
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }

                        case 10://C T
                            {
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 11://S Tr C
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                break;
                            }

                        case 12://S Tr T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 13://S C T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 14://Tr C T
                            {
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                        case 15://S Tr C T
                            {
                                Objtmp.MDoseStr = Objtmp.dosageStr;
                                Objtmp.NDoseStr = Objtmp.dosageStr;
                                Objtmp.ADoseStr = Objtmp.dosageStr;
                                Objtmp.EDoseStr = Objtmp.dosageStr;
                                break;
                            }
                    }
                }
            }
        }


        #endregion

        #region Tạo Toa Mới

        //Chọn 1 chẩn đoán để ra toa
        public void Handle(DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> message)
        {
            if (message != null)
            {
                ObjDiagnosisTreatment_Current = message.DiagnosisTreatment.DeepCopy();

                string cd = ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
                if (string.IsNullOrEmpty(cd))
                {
                    cd = ObjDiagnosisTreatment_Current.Diagnosis.Trim();
                }

                if (LatestePrecriptions == null) LatestePrecriptions = new Prescription();

                LatestePrecriptions.Diagnosis = cd;
                LatestePrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
                if (LatestePrecriptions.PrescriptionIssueHistory == null)
                {
                    LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
                }

                // TxD 26/12/2014: The Bug is here ie. Registration_DataStorage.CurrentPatientRegistrationDetail is null
                //                  Check out why it is not set yet or is it ever set .... then fix it
                HisIDVisibility = Registration_DataStorage.CurrentPatientRegistrationDetail.HisID != null
                && Registration_DataStorage.CurrentPatientRegistrationDetail.HisID.Value > 0 ? true : false;
                isHisID = HisIDVisibility;

                LatestePrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
                //Cụ thể  DV nào
                if (Registration_DataStorage.CurrentPatientRegistration != null)
                {
                    if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                        {
                            LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = ObjDiagnosisTreatment_Current.PtRegDetailID;
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                            return;
                        }
                    }
                    else/*Khám VIP, Khám Cho Nội Trú*/
                    {
                        LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
                    return;
                }
                //Cụ thể  DV nào
            }
        }
        private Prescription _ObjTaoThanhToaMoi;
        public Prescription ObjTaoThanhToaMoi
        {
            get
            {
                return _ObjTaoThanhToaMoi;
            }
            set
            {
                if (_ObjTaoThanhToaMoi != value)
                {
                    _ObjTaoThanhToaMoi = value;
                    NotifyOfPropertyChange(() => ObjTaoThanhToaMoi);
                }
            }
        }

        #endregion


        #region IHandle<ePrescriptionSelectedEvent> Members
        public Staff ObjStaff
        {
            get
            {
                return Globals.LoggedUserAccount.Staff;
            }
        }
        #endregion

        private void CallAppointmentDialog(bool aIsPCLBookingView = false)
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.T1455_G1_HBenh.ToLower()))
            {
                return;
            }
            //KMx: Không được dựa vào Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories. Vì nếu user chọn toa cũ thì khi hẹn bệnh sẽ bị lỗi hẹn cho toa của DK hiện tại (28/03/2016 15:34).
            //if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count < 1
            //    || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories == null || Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories.Count < 1)
            //{
            //    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            //if (Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].AppointmentID.GetValueOrDefault() <= 0)
            //{
            //    MessageBox.Show("Bạn chưa chọn hẹn tái khám nên không thể hẹn bệnh!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            if (LatestePrecriptions == null || LatestePrecriptions.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator();
            long? appointmentID = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAppointmentID(LatestePrecriptions.IssueID, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndGetAppointmentID(out appointmentID, asyncResult);
                            //KMx: Khi đổi ngày cuộc hẹn thì ID cuộc hẹn cũ bị xóa, ID cuộc hẹn mới được update vào toa thuốc nhưng toa thuốc không load lại. Dẫn tới dữ liệu không đồng bộ nên mỗi lần hẹn bệnh phải load lại AppointmentID (15/05/2016 17:52). 
                            LatestePrecriptions.AppointmentID = appointmentID;
                            if (LatestePrecriptions.AppointmentID.GetValueOrDefault() <= 0)
                            {
                                MessageBox.Show(eHCMSResources.K0214_G1_ToaThuocKhongHenTK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                return;
                            }
                            PatientAppointment curAppt = new PatientAppointment();
                            curAppt.Patient = Registration_DataStorage.CurrentPatient;
                            //curAppt.AppointmentID = Registration_DataStorage.PatientServiceRecordCollection[0].PrescriptionIssueHistories[0].AppointmentID.GetValueOrDefault();
                            curAppt.AppointmentID = LatestePrecriptions.AppointmentID.GetValueOrDefault();
                            if (aIsPCLBookingView && (curAppt == null || curAppt.AppointmentID == 0))
                            {
                                return;
                            }
                            Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
                            {
                                apptVm.Registration_DataStorage = Registration_DataStorage;
                                apptVm.SetCurrentAppointment(curAppt);
                                apptVm.IsPCLBookingView = aIsPCLBookingView;
                            };
                            GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
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
        //CMN: Mở Popup gõ chi tiết cuộc hẹn thay vì hẹn tự động từ toa thuốc
        private void CallLoadAppoinmentDialog()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.T1455_G1_HBenh.ToLower()))
            {
                return;
            }
            if (LatestePrecriptions == null || LatestePrecriptions.IssueID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0485_G1_Msg_InfoDKChuaCoToa_KhTheHen, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            this.ShowBusyIndicator();
            var CurrentThread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //CMN: Lấy cuộc hẹn tái khám bằng ID toa thuốc
                        contract.BeginGetAppointmentByID(0, LatestePrecriptions.IssueID, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var appt = contract.EndGetAppointmentByID(asyncResult);
                                PatientAppointment CurrentAppointment = new PatientAppointment();
                                if (appt != null)
                                {
                                    CurrentAppointment = appt;
                                }
                                else
                                {
                                    CurrentAppointment.DoctorStaffID = Globals.LoggedUserAccount.StaffID;
                                    CurrentAppointment.DoctorStaff = Globals.LoggedUserAccount.Staff;
                                }
                                CurrentAppointment.ApptDate = appt != null && appt.ApptDate.HasValue && appt.ApptDate != null ? appt.ApptDate.Value : LatestePrecriptions.IssuedDateTime.GetValueOrDefault(Globals.GetCurServerDateTime()).AddDays((int)LatestePrecriptions.NDay);
                                CurrentAppointment.NDay = LatestePrecriptions.NDay;
                                CurrentAppointment.Patient = Registration_DataStorage.CurrentPatient;
                                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null)
                                {
                                    CurrentAppointment.ServiceRecID = Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID;
                                }
                                if (CurrentAppointment.ServiceRecID.GetValueOrDefault(0) == 0 &&
                                    Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.PatientServiceRecordCollection != null &&
                                    Registration_DataStorage.PatientServiceRecordCollection.Any(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID))
                                {
                                    CurrentAppointment.ServiceRecID = Registration_DataStorage.PatientServiceRecordCollection.First(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID).ServiceRecID;
                                }
                                Action<IAddEditAppointment> onInitDlg = delegate (IAddEditAppointment apptVm)
                                {
                                    apptVm.Registration_DataStorage = Registration_DataStorage;
                                    apptVm.IsCreateApptFromConsultation = true;
                                    apptVm.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                                    if (CurrentAppointment.AppointmentID > 0)
                                    {
                                        apptVm.SetCurrentAppointment(CurrentAppointment);
                                    }
                                    else
                                    {
                                        apptVm.SetCurrentAppointment(CurrentAppointment, LatestePrecriptions.IssueID);
                                    }
                                };
                                GlobalsNAV.ShowDialog<IAddEditAppointment>(onInitDlg);
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                    this.HideBusyIndicator();
                }
            });
            CurrentThread.Start();
        }
        //public void btnCLSAppointment()
        //{
        //    CallAppointmentDialog(true);
        //}
        //CMN: Thêm nút load danh sách cuộc hẹn để tạo CLS sổ
        private void CallPCLBookingView()
        {
            IPatientAppointments DialogView = Globals.GetViewModel<IPatientAppointments>();
            DialogView.IsPCLBookingView = true;
            DialogView.Registration_DataStorage = Registration_DataStorage;
            if (LatestePrecriptions.AppointmentID.HasValue && LatestePrecriptions.AppointmentID > 0)
            {
                DialogView.CurrentPtRegDetailAppointmentID = LatestePrecriptions.AppointmentID.Value;
            }
            GlobalsNAV.ShowDialog_V3(DialogView, (aView) =>
            {
                if (LatestePrecriptions.AppointmentID.HasValue && LatestePrecriptions.AppointmentID > 0)
                {
                    aView.SetCurrentPatient(Registration_DataStorage.CurrentPatient, LatestePrecriptions);
                }
                else
                {
                    aView.SetCurrentPatient(Registration_DataStorage.CurrentPatient, LatestePrecriptions);
                }
            }, null, false, true, Globals.GetDefaultDialogViewSize());
        }
        private bool _btChonChanDoanIsEnabled;
        public bool btChonChanDoanIsEnabled
        {
            get
            {
                return _btChonChanDoanIsEnabled;
            }
            set
            {
                if (_btChonChanDoanIsEnabled != value)
                {
                    _btChonChanDoanIsEnabled = value;
                    NotifyOfPropertyChange(() => btChonChanDoanIsEnabled);
                }
            }
        }


        public void btChonChanDoan(object sender, RoutedEventArgs e)
        {
            Globals.ConsultationIsChildWindow = true;
            Action<IConsultations> onInitDlg = delegate (IConsultations proAlloc)
            {
                proAlloc.IsPopUp = true;
            };
            GlobalsNAV.ShowDialog<IConsultations>(onInitDlg);
        }


        //KMx: Sau khi kiểm tra, thấy Handle này không còn sử dụng nữa (16/05/2014 17:07)
        #region IHandle<ReloadDataePrescriptionEvent> Members

        public void Handle(ReloadDataePrescriptionEvent message)
        {
            MessageBox.Show(eHCMSResources.A0563_G1_Msg_HandleKhongSuDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //if (message != null)
            //{
            //    if (Registration_DataStorage.CurrentPatient != null)
            //    {
            //        GetLatestPrescriptionByPtID(Registration_DataStorage.CurrentPatient.PatientID);
            //    }
            //}
        }

        #endregion

        #region Schedule
        private int _IndexRow;
        public int IndexRow
        {
            get { return _IndexRow; }
            set
            {
                if (_IndexRow != value)
                {
                    _IndexRow = value;
                    NotifyOfPropertyChange(() => IndexRow);
                }
            }
        }

        public bool SelectedPrescriptionDetailIsValid()
        {
            if (grdPrescription.SelectedIndex < 0 || grdPrescription.SelectedIndex > (LatestePrecriptions.PrescriptionDetails.Count - 1))
            {
                return false;
            }
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];
            if (selPrescriptionDetail == null)
            {
                return false;
            }
            if ((selPrescriptionDetail.DrugID > 0) || (selPrescriptionDetail.IsDrugNotInCat == true && selPrescriptionDetail.BrandName.Length > 1))
            {
                return true;
            }
            return false;
        }

        public void hplEditSchedules_Click(Object pSelectedItem)
        {
            if (pSelectedItem == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0534_G1_Msg_InfoDongThuocKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];

            if (selPrescriptionDetail.DrugType.LookupID != (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
            {
                MessageBox.Show(eHCMSResources.A0533_G1_Msg_InfoThuocKhUongTheoLichTuan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            hplEditSchedules(selPrescriptionDetail);

        }

        public void hplEditSchedules(PrescriptionDetail pSelectedItem)
        {
            if (!CheckValidationEditor1Row(pSelectedItem))
            {
                return;
            }

            IndexRow = grdPrescription.SelectedIndex;

            Action<IPrescriptionDetailSchedulesNew> onInitDlg = delegate (IPrescriptionDetailSchedulesNew typeInfo)
            {
                typeInfo.ParentVM = this;
                typeInfo.ObjPrescriptionDetail = pSelectedItem;

                typeInfo.ModeForm = LatestePrecriptions.IssueID > 0 ? 1 : 0;/*Update*//*AddNew*/

                typeInfo.IsMaxDay = typeInfo.ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts == 0
                    || typeInfo.ObjPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed > 0 ? true : false;


                int nScheduleDays = Convert.ToInt32(typeInfo.ObjPrescriptionDetail.DayRpts + typeInfo.ObjPrescriptionDetail.DayExtended);

                if (nScheduleDays <= 0 && LatestePrecriptions.NDay.HasValue)
                {
                    nScheduleDays = LatestePrecriptions.NDay.Value;
                }

                typeInfo.NDay = nScheduleDays;

                typeInfo.ObjPrescriptionDetailSchedules_ByPrescriptDetailID = pSelectedItem.ObjPrescriptionDetailSchedules.DeepCopy();

                typeInfo.Initialize();
            };
            GlobalsNAV.ShowDialog<IPrescriptionDetailSchedulesNew>(onInitDlg);
        }

        public void HandleDrugSchedule(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note)
        {
            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];

            if (selPrescriptionDetail == null || selPrescriptionDetail.SelectedDrugForPrescription == null)
            {
                return;
            }

            //Chọn Lịch thì gán bên ngoài = 0
            selPrescriptionDetail.MDose = 0;
            selPrescriptionDetail.ADose = 0;
            selPrescriptionDetail.NDose = 0;
            selPrescriptionDetail.EDose = 0;
            selPrescriptionDetail.dosage = 0;

            selPrescriptionDetail.MDoseStr = "0";
            selPrescriptionDetail.ADoseStr = "0";
            selPrescriptionDetail.NDoseStr = "0";
            selPrescriptionDetail.EDoseStr = "0";
            selPrescriptionDetail.dosageStr = "0";
            if (RegistrationCoverByHI())
            {
                if (IsBenhNhanNoiTru() == false)
                {
                    if (numOfDay > xNgayBHToiDa_NgoaiTru)
                    {
                        selPrescriptionDetail.DayRpts = xNgayBHToiDa_NgoaiTru;
                        selPrescriptionDetail.DayExtended = numOfDay - xNgayBHToiDa_NgoaiTru;
                    }
                    else
                    {
                        selPrescriptionDetail.DayRpts = numOfDay;
                        selPrescriptionDetail.DayExtended = 0;
                    }
                }
                else
                {
                    if (numOfDay > xNgayBHToiDa_NoiTru)
                    {
                        selPrescriptionDetail.DayRpts = xNgayBHToiDa_NoiTru;
                        selPrescriptionDetail.DayExtended = numOfDay - xNgayBHToiDa_NoiTru;
                    }
                    else
                    {
                        selPrescriptionDetail.DayRpts = numOfDay;
                        selPrescriptionDetail.DayExtended = 0;
                    }
                }
            }
            else
            {
                selPrescriptionDetail.DayRpts = numOfDay;
                selPrescriptionDetail.DayExtended = 0;
            }

            //selPrescriptionDetail.Qty = message.TongThuoc;
            selPrescriptionDetail.DrugInstructionNotes = note;
            //Chọn Lịch thì gán bên ngoài = 0

            // // Txd Commented out
            //SelectedPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
            //SelectedPrescriptionDetail.HasSchedules = message.HasSchedule;

            selPrescriptionDetail.ObjPrescriptionDetailSchedules = drugSchedule;
            selPrescriptionDetail.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;

            CalQtyAndQtyMaxForSchedule(selPrescriptionDetail);
        }

        public void HandleDrugScheduleForm(ObservableCollection<PrescriptionDetailSchedules> drugSchedule, double numOfDay, string note)
        {
            if (ObjPrescriptionDetailForForm == null)
            {
                ObjPrescriptionDetailForForm = new PrescriptionDetail();
                ObjPrescriptionDetailForForm.isForm = true;
            }

            //Chọn Lịch thì gán bên ngoài = 0
            ObjPrescriptionDetailForForm.MDose = 0;
            ObjPrescriptionDetailForForm.ADose = 0;
            ObjPrescriptionDetailForForm.NDose = 0;
            ObjPrescriptionDetailForForm.EDose = 0;
            ObjPrescriptionDetailForForm.dosage = 0;

            ObjPrescriptionDetailForForm.MDoseStr = "0";
            ObjPrescriptionDetailForForm.ADoseStr = "0";
            ObjPrescriptionDetailForForm.NDoseStr = "0";
            ObjPrescriptionDetailForForm.EDoseStr = "0";
            ObjPrescriptionDetailForForm.dosageStr = "0";
            if (RegistrationCoverByHI())
            {
                if (IsBenhNhanNoiTru() == false)
                {
                    if (numOfDay > xNgayBHToiDa_NgoaiTru)
                    {
                        ObjPrescriptionDetailForForm.DayRpts = xNgayBHToiDa_NgoaiTru;
                        ObjPrescriptionDetailForForm.DayExtended = numOfDay - xNgayBHToiDa_NgoaiTru;
                    }
                    else
                    {
                        ObjPrescriptionDetailForForm.DayRpts = numOfDay;
                        ObjPrescriptionDetailForForm.DayExtended = 0;
                    }
                }
                else
                {
                    if (numOfDay > xNgayBHToiDa_NoiTru)
                    {
                        ObjPrescriptionDetailForForm.DayRpts = xNgayBHToiDa_NoiTru;
                        ObjPrescriptionDetailForForm.DayExtended = numOfDay - xNgayBHToiDa_NoiTru;
                    }
                    else
                    {
                        ObjPrescriptionDetailForForm.DayRpts = numOfDay;
                        ObjPrescriptionDetailForForm.DayExtended = 0;
                    }
                }
            }
            else
            {
                ObjPrescriptionDetailForForm.DayRpts = numOfDay;
                ObjPrescriptionDetailForForm.DayExtended = 0;
            }

            //ObjPrescriptionDetailForForm.Qty = message.TongThuoc;
            ObjPrescriptionDetailForForm.DrugInstructionNotes = note;
            //Chọn Lịch thì gán bên ngoài = 0

            ObjPrescriptionDetailForForm.ObjPrescriptionDetailSchedules = drugSchedule;
            ObjPrescriptionDetailForForm.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;

            CalQtyAndQtyMaxForSchedule(ObjPrescriptionDetailForForm);
        }

        //▼===== 20190620 TTM: Do có bổ sung vào cột thông tin thuốc ở đầu Grid nên cần phải thay đổi lại
        //                     index của bắt đầu và Index kết thúc của Grid để xuống dòng.
        private enum PrescriptGridEditCellIdx { EditCellIdxBegin = 5, EditCellIdxEnd = 16 };
        EmrPrescriptionGrid grdPrescription;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as EmrPrescriptionGrid;
            grdPrescription.nFirstEditIdx = (int)PrescriptGridEditCellIdx.EditCellIdxBegin;
            grdPrescription.nLastEditIdx = (int)PrescriptGridEditCellIdx.EditCellIdxEnd;
        }

        #endregion

        #region "Ra Toa Mới"

        public void InitDataGridNew()
        {
            if (NewPrecriptions == null)
            {
                NewPrecriptions = new Prescription();
            }
            if (NewPrecriptions.PrescriptionDetails == null || NewPrecriptions.PrescriptionDetails.Count == 0)
            {
                AddNewBlankDrugIntoPrescriptObjectNew();
            }
        }

        private bool _HasDiagnosis = false;
        public bool HasDiagnosis
        {
            get
            {
                return _HasDiagnosis;
            }
            set
            {
                if (_HasDiagnosis != value)
                {
                    _HasDiagnosis = value;
                    NotifyOfPropertyChange(() => HasDiagnosis);
                }
            }
        }

        private void DefaultValueForNewPrecription()
        {
            NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID = new Staff();
            NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

            if (HasDiagnosis)
            {
                if (ObjDiagnosisTreatment_Current != null)
                {
                    NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord == null && !IsShowSummaryContent ? Globals.GetCurServerDateTime() : ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;
                }
            }


            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;

            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
            NewPrecriptions.PrescriptionIssueHistory.DeptLocID = Globals.DeptLocation.DeptLocationID;
            if (HisIDVisibility)//neu dang ky co bao hiem
            {
                NewPrecriptions.PrescriptionIssueHistory.HisID = Registration_DataStorage.CurrentPatientRegistration.HisID.Value;
            }

            //Cụ thể  DV nào
            if (Registration_DataStorage.CurrentPatientRegistration != null)
            {
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
                    {
                        NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
                        return;
                    }
                }
                else/*Khám VIP, Khám Cho Nội Trú*/
                {
                    NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
                return;
            }
            //Cụ thể  DV nào

        }

        private Prescription _NewPrecriptions;
        public Prescription NewPrecriptions
        {
            get
            {
                return _NewPrecriptions;
            }
            set
            {
                if (_NewPrecriptions != value)
                {
                    _NewPrecriptions = value;
                    NotifyOfPropertyChange(() => NewPrecriptions);
                }
            }
        }

        #endregion


        public void Handle(PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int> message)
        {
            if (message != null && this.GetView() != null)
            {
                UpdateLatestePrecriptionsDrugNotInCat(message.PrescriptionDrugNotInCat, message.Index);
            }
        }

        public void UpdateLatestePrecriptionsDrugNotInCat(PrescriptionDetail ObjPrescriptionDetail, int Index)
        {
            LatestePrecriptions.PrescriptionDetails[Index] = ObjPrescriptionDetail.DeepCopy();

            if (
                (LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].DrugID == null
                ||
                LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].DrugID == 0)
                &&
                LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].IsDrugNotInCat == true
                )
            {
                AddNewBlankDrugIntoPrescriptObjectNew();
            }
        }

        #region "Check Thuoc Di Ung"
        private ObservableCollection<MDAllergy> _ptAllergyList;
        public ObservableCollection<MDAllergy> PtAllergyList
        {
            get
            {
                return _ptAllergyList;
            }
            set
            {
                if (_ptAllergyList != value)
                {
                    _ptAllergyList = value;
                    NotifyOfPropertyChange(() => PtAllergyList);
                }
            }
        }

        #endregion
        private void GetPrescriptionTypeList()
        {
            ObservableCollection<Lookup> PrescriptLookupList = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.PRESCRIPTION_TYPE).ToObservableCollection();

            if (PrescriptLookupList == null || PrescriptLookupList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0742_G1_Msg_InfoKhTimThayLoaiToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PrescriptionTypeList = new ObservableCollection<Lookup>(PrescriptLookupList);

            if (PrescriptionTypeList.Count > 0)
            {
                CurrentPrescriptionType = PrescriptionTypeList[0];
            }
        }
        private byte GetSchedBeginDOW()
        {
            //KMx: Nếu "tạo mới dựa trên toa cũ" thì OrigIssuedDateTime = NULL. Xuống dưới set lại là ngày hiện tại (06/06/2014 15:30).
            DateTime OrigIssuedDateTime = LatestePrecriptions.OrigIssuedDateTime.GetValueOrDefault();

            if (OrigIssuedDateTime == DateTime.MinValue)
            {
                OrigIssuedDateTime = Globals.ServerDate.Value;
            }

            return Globals.GetDayOfWeek(OrigIssuedDateTime);
        }

        public float[] CalDrugForDayFromSchedule(PrescriptionDetail prescriptDetail)
        {
            float[] WeeklySchedule = new float[7];

            if (prescriptDetail == null || prescriptDetail.ObjPrescriptionDetailSchedules == null || prescriptDetail.ObjPrescriptionDetailSchedules.Count <= 0)
            {
                return WeeklySchedule;
            }

            foreach (PrescriptionDetailSchedules item in prescriptDetail.ObjPrescriptionDetailSchedules)
            {
                WeeklySchedule[0] += item.Monday.GetValueOrDefault(0);
                WeeklySchedule[1] += item.Tuesday.GetValueOrDefault(0);
                WeeklySchedule[2] += item.Wednesday.GetValueOrDefault(0);
                WeeklySchedule[3] += item.Thursday.GetValueOrDefault(0);
                WeeklySchedule[4] += item.Friday.GetValueOrDefault(0);
                WeeklySchedule[5] += item.Saturday.GetValueOrDefault(0);
                WeeklySchedule[6] += item.Sunday.GetValueOrDefault(0);
            }

            prescriptDetail.QtySchedMon = WeeklySchedule[0];
            prescriptDetail.QtySchedTue = WeeklySchedule[1];
            prescriptDetail.QtySchedWed = WeeklySchedule[2];
            prescriptDetail.QtySchedThu = WeeklySchedule[3];
            prescriptDetail.QtySchedFri = WeeklySchedule[4];
            prescriptDetail.QtySchedSat = WeeklySchedule[5];
            prescriptDetail.QtySchedSun = WeeklySchedule[6];

            return WeeklySchedule;
        }

        private double CalQtyForScheduleDrug(PrescriptionDetail prescriptDetail, int nNumDayPrescribed)
        {
            prescriptDetail.SchedBeginDOW = GetSchedBeginDOW();

            float[] WeeklySchedule = CalDrugForDayFromSchedule(prescriptDetail);

            return Globals.CalcWeeklySchedulePrescription(prescriptDetail.SchedBeginDOW, nNumDayPrescribed, WeeklySchedule, (float)prescriptDetail.SelectedDrugForPrescription.DispenseVolume);
        }

        //KMx: Tính số lượng thuốc lịch (04/06/2014 15:39).
        private void CalQtyAndQtyMaxForSchedule(PrescriptionDetail prescriptDetail)
        {
            prescriptDetail.Qty = CalQtyForScheduleDrug(prescriptDetail, prescriptDetail.RealDay);

            if (prescriptDetail.IsDrugNotInCat)
            {
                return;
            }

            //KMx: Tính số lượng thuốc tối đa (thuốc lịch) mà BH đồng ý chi trả (05/06/2014 14:00).
            if (prescriptDetail.RealDay <= xNgayBHToiDa)
            {
                prescriptDetail.QtyMaxAllowed = prescriptDetail.Qty;
            }
            else
            {
                prescriptDetail.QtyMaxAllowed = CalQtyForScheduleDrug(prescriptDetail, xNgayBHToiDa);
            }
        }

        //==== 20161004 CMN Begin: Call services check drug interactions
        public void btnIntraction()
        {
            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
            {
                string[] mGenericName = LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID != 0).Select(x => x.SelectedDrugForPrescription.GenericName).ToArray();
                string[] mBrandName = LatestePrecriptions.PrescriptionDetails.Where(x => x.DrugID != 0).Select(x => x.SelectedDrugForPrescription.BrandName).ToArray();
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        this.ShowBusyIndicator();
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckDrugInteraction(mGenericName, mBrandName, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string DrugIntractions = contract.EndCheckDrugInteraction(mGenericName, mBrandName, asyncResult);
                                this.HideBusyIndicator();
                                if (!String.IsNullOrEmpty(DrugIntractions))
                                    MessageBox.Show(DrugIntractions);
                                else
                                    MessageBox.Show(eHCMSResources.A0669_G1_Msg_InfoKhCoTuongTacThuoc);
                                //System.Windows.Threading.Dispatcher dispatcher = Deployment.Current.Dispatcher;
                                //dispatcher.BeginInvoke(() =>
                                //{
                                //    var msgVm = Globals.GetViewModel<IDrugIntractionMessageBox>();
                                //    msgVm.IntractionText = DrugIntractions;
                                //    Globals.ShowDialog(msgVm as Conductor<object>);
                                //});
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            finally
                            {
                            }

                        }), null);
                    }
                });
                t.Start();
            }
        }
        //20161004 CMN End
        private ObservableCollection<GetDrugForSellVisitor> _DrugsInTreatmentRegimen;
        public ObservableCollection<GetDrugForSellVisitor> DrugsInTreatmentRegimen
        {
            get => _DrugsInTreatmentRegimen;
            set
            {
                _DrugsInTreatmentRegimen = value;
                NotifyOfPropertyChange(() => DrugsInTreatmentRegimen);
            }
        }

        private bool _IsSearchByTreatmentRegimenVisibility = Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen;
        public bool IsSearchByTreatmentRegimenVisibility
        {
            get => _IsSearchByTreatmentRegimenVisibility; set
            {
                _IsSearchByTreatmentRegimenVisibility = value;
                NotifyOfPropertyChange(() => IsSearchByTreatmentRegimenVisibility);
            }
        }
        private bool _IsSearchByTreatmentRegimen = Globals.ServerConfigSection.ConsultationElements.CheckedTreatmentRegimen;
        public bool IsSearchByTreatmentRegimen
        {
            get => _IsSearchByTreatmentRegimen; set
            {
                _IsSearchByTreatmentRegimen = value;
                NotifyOfPropertyChange(() => IsSearchByTreatmentRegimen);
            }
        }

        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
            }
        }
        /*▼====: #004*/

        public void ResetPrescriptionInfoChanged()
        {
            LatestePrecriptions.ResetDataChanged();
        }

        //private bool _IsPrescriptionChanged = false;
        public bool IsPrescriptionInfoChanged
        {
            get
            {
                return LatestePrecriptions.IsPrescriptionDataChanged;
            }
        }
        /*▲====: #004*/

        //▼====== #013
        public bool IsOutCatConfirmed = false;
        //▲====== #013

        public void btnEditDiagnosis()
        {
            GlobalsNAV.ShowDialog<IConsultationOld>((ConsultationVM) =>
            {
                //▼===== 20191020 TTM:  Trước đây không cần đưa giá trị vào màn hình do xài Globals. Nhưng đã chuyển từ Globals sang biến nội bộ là Registration_DataStorage
                //                      nên cần set giá trị lại để không bị System Null Exception.
                ConsultationVM.Registration_DataStorage = Registration_DataStorage;

                ConsultationVM.IsUpdateFromPresciption = true;
            }, null, false, true, new Size(900, 600));
        }
        public bool KiemTraDaDuyetToaChua()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.ConfirmHIStaffID > 0)
            {
                MessageBox.Show(eHCMSResources.Z2446_G1_KhongChinhSuaToaKhiDaDuyet);
                return false;
            }
            return true;
        }
        public bool KiemTraDaXuatChua()
        {
            //Globals.ServerConfigSection.
            //if (true)
            //{
            //    MessageBox.Show(eHCMSResources.K3971_G1_XacNhanChinhSua);
            //    return false;
            //}
            return true;
        }

        private bool _IsUpdateWithoutChangeDoctorIDAndDatetime = false;
        public bool IsUpdateWithoutChangeDoctorIDAndDatetime
        {
            get
            {
                return _IsUpdateWithoutChangeDoctorIDAndDatetime;
            }
            set
            {
                if (_IsUpdateWithoutChangeDoctorIDAndDatetime != value)
                {
                    _IsUpdateWithoutChangeDoctorIDAndDatetime = value;
                    NotifyOfPropertyChange(() => IsUpdateWithoutChangeDoctorIDAndDatetime);
                }
            }
        }
        //▼===== #018
        public void hplCheckDrugInfor_Click(Object pSelectedItem)
        {
            if (pSelectedItem == null)
            {
                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (!SelectedPrescriptionDetailIsValid())
            {
                MessageBox.Show(eHCMSResources.A0534_G1_Msg_InfoDongThuocKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PrescriptionDetail selPrescriptionDetail = LatestePrecriptions.PrescriptionDetails[grdPrescription.SelectedIndex];

            GetDrugInformation(selPrescriptionDetail);

        }

        public void GetDrugInformation(PrescriptionDetail selPrescriptionDetail)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetDrugInformation(selPrescriptionDetail.DrugID, Globals.DispatchCallback((asyncResult) =>
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
        //▲===== #018
        //▼====== #019
        public void PatientAppointments_Save(bool PassCheckFullTarget = true)
        {
            PatientAppointment CurrentAppointment = new PatientAppointment();
            CurrentAppointment.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentAppointment.DoctorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentAppointment.Patient = Registration_DataStorage.CurrentPatient;
            CurrentAppointment.AppointmentID = LatestePrecriptions.AppointmentID.GetValueOrDefault();
            CurrentAppointment.V_ApptStatus = AllLookupValues.ApptStatus.BOOKED;
            CurrentAppointment.NDay = LatestePrecriptions.NDay;
            CurrentAppointment.ApptDate = Globals.GetCurServerDateTime().AddDays(CurrentAppointment.NDay.Value);
            if (!Globals.ServerConfigSection.ConsultationElements.AllowWorkingOnSunday && CurrentAppointment.ApptDate.Value.DayOfWeek == DayOfWeek.Sunday)
            {
                CurrentAppointment.ApptDate = CurrentAppointment.ApptDate.Value.AddDays(1);
            }
            CurrentAppointment.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            string[] ListStr = Globals.ServerConfigSection.ConsultationElements.ParamAppointmentAuto.Split(new char[] { ';' });
            PatientApptServiceDetails item = new PatientApptServiceDetails
            {
                V_AppointmentType = Convert.ToInt64(ListStr[0]),
                MedServiceID = Convert.ToInt64(ListStr[1]),
                DeptLocationID = Convert.ToInt64(ListStr[2]),
                ApptTimeSegmentID = Convert.ToInt16(ListStr[3]),
                EntityState = EntityState.DETACHED
            };
            CurrentAppointment.ObjApptServiceDetailsList_Add = new ObservableCollection<PatientApptServiceDetails>();
            CurrentAppointment.ObjApptServiceDetailsList_Add.Add(item);
            this.DlgShowBusyIndicator(eHCMSResources.Z1016_G1_DangLuuCuocHen);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new AppointmentServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientAppointments_Save(CurrentAppointment, PassCheckFullTarget, null, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long AppointmentID = 0;
                                string ErrorDetail = "";

                                string ListNotConfig = "";
                                string ListTargetFull = "";
                                string ListMax = "";
                                string ListRequestID = "";

                                var b = contract.EndPatientAppointments_Save(out AppointmentID, out ErrorDetail, out ListNotConfig, out ListTargetFull, out ListMax, out ListRequestID, asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲====== #019
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
                UCAllergiesWarningByPatientID.Registration_DataStorage = Registration_DataStorage;
            }
        }
        private bool _IsEnableHIStore = Globals.ServerConfigSection.CommonItems.EnableHIStore;
        public bool IsEnableHIStore
        {
            get
            {
                return _IsEnableHIStore;
            }
            set
            {
                if (_IsEnableHIStore == value)
                {
                    return;
                }
                _IsEnableHIStore = value;
                NotifyOfPropertyChange(() => IsEnableHIStore);
            }
        }

        //▼===== #023
        private Staff _Secretary;
        public Staff Secretary
        {
            get
            {
                return _Secretary;
            }
            set
            {
                if (_Secretary != value)
                {
                    _Secretary = value;
                    if (LatestePrecriptions != null)
                    {
                        LatestePrecriptions.SecretaryStaff = _Secretary;
                    }
                    NotifyOfPropertyChange(() => Secretary);
                }
            }
        }
        //▲===== #023
        private bool _IsCheckApmtOnPrescription = Globals.ServerConfigSection.ConsultationElements.IsCheckApmtOnPrescription;
        public bool IsCheckApmtOnPrescription
        {
            get
            {
                return _IsCheckApmtOnPrescription;
            }
            set
            {
                if (_IsCheckApmtOnPrescription == value)
                {
                    return;
                }
                _IsCheckApmtOnPrescription = value;
                NotifyOfPropertyChange(() => IsCheckApmtOnPrescription);
            }
        }

        private void GetPrescription(long PrescriptID = 0)
        {
            LatestePrecriptions.Diagnosis = ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
            LatestePrecriptions.ExamDate = Registration_DataStorage.CurrentPatientRegistration.ExamDate;
            GetPrescriptionDetailsByPrescriptID(PrescriptID);
            //InitialNewPrescription();
            //SetToaBaoHiem_KhongBaoHiem();
            //if (IsRegDetailHasPrescript)
            //{
            //    GetToaThuocDaCo(PrescriptID);
            //}
            //else if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            //{
            //    GetLatestPrescriptionByPtID_New(Registration_DataStorage.CurrentPatient.PatientID);
            //}
        }
        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            for (int idx = 1; idx < grdPrescription.Columns.Count; idx++)
            {
                //if (idx == (int)DataGridCol.STRENGHT || idx == (int)DataGridCol.UNITS || idx == (int)DataGridCol.UNITUSE || idx == (int)DataGridCol.USAGE || idx == (int)DataGridCol.QTY)
                //{
                //    TextBox obj = grdPrescription.Columns[idx].GetCellContent(e.Row) as TextBox;

                //    if (obj != null)
                //    {
                //        obj.IsReadOnly = true;
                //        //if ((e.Row.GetIndex() % 2) == 1)
                //        obj.Background = new SolidColorBrush(Color.FromArgb(245, 228, 228, 231));
                //        //else
                //        //obj.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
                //    }
                //}
            }

            PrescriptionDetail objRows = e.Row.DataContext as PrescriptionDetail;

            if (objRows.HasSchedules)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120));
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
            }

            switch (objRows.V_DrugType)
            {
                case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:
                    e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120)); break;
            }

            if (objRows.SelectedDrugForPrescription != null
                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed != null
                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed > 0)
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));//pink
                objRows.BackGroundColor = "#E79DEA";
                NotifyOfPropertyChange(() => e.Row.Background);
            }

            if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && DrugsInTreatmentRegimen != null && DrugsInTreatmentRegimen.Count > 0 && objRows != null && objRows.DrugID.GetValueOrDefault(0) > 0 && !DrugsInTreatmentRegimen.Any(x => x.DrugID == objRows.DrugID))
            {
                e.Row.Background = new SolidColorBrush(Color.FromArgb(46, 204, 113, 248));
            }

            if (objRows.RefGenericDrugDetail != null && objRows.RefGenericDrugDetail.V_CatDrugType == (long)AllLookupValues.V_CatDrugType.DrugDept)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(236, 219, 77)); //Yellow
            }
            //▼====== #009
            if (objRows.IsContraIndicator && Globals.ServerConfigSection.ConsultationElements.AllowBlockContraIndicator)
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 69, 0)); //Red
            }
            //▲====== #009
            if (Globals.ServerConfigSection.ConsultationElements.LevelWarningWhenCreateNewAndCopy > 0 && objRows.InsuranceCover == true
                && Registration_DataStorage.CurrentPatientRegistration != null &&
                (Registration_DataStorage.CurrentPatientRegistration.HisID == null || Registration_DataStorage.CurrentPatientRegistration.HisID == 0))
            {
                e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 205, 220)); //Orange
            }
        }
        public void cbxDrugType_Loaded(object sender, RoutedEventArgs e)
        {
            var kbEnabledComboBox = sender as KeyEnabledComboBox;
            if (kbEnabledComboBox != null)
            {
                kbEnabledComboBox.ItemsSource = DrugTypes;
            }
        }

        AutoCompleteBox AutoGenMedProduct;
        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            AutoGenMedProduct = sender as AutoCompleteBox;
        }

        public void DrugUnitUse_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = DonViTinh;
        }

        public void DrugAdministration_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = CachDung;
        }

        public void PrescriptionNoteTemplates_GetAllIsActiveAdm()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionAdministration;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                CachDung = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
                                NotifyOfPropertyChange(() => CachDung);
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
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }

        public void DrugInstructionNotes_Loaded(object sender, PopulatingEventArgs e)
        {
            ((AxAutoComplete)sender).ItemsSource = GhiChu;
        }

        public void PrescriptionNoteTemplates_GetAllIsActiveItem()
        {
            var t = new Thread(() =>
            {
                IsWaitingPrescriptionNoteTemplates_GetAll = true;

                try
                {
                    using (var serviceFactory = new ePrescriptionsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        var pnt = new PrescriptionNoteTemplates();
                        pnt.V_PrescriptionNoteTempType = AllLookupValues.V_PrescriptionNoteTempType.PrescriptionNoteItem;
                        contract.BeginPrescriptionNoteTemplates_GetAllIsActive(pnt, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<PrescriptionNoteTemplates> allItems = new ObservableCollection<PrescriptionNoteTemplates>();
                            try
                            {
                                allItems = contract.EndPrescriptionNoteTemplates_GetAllIsActive(asyncResult);
                                GhiChu = new ObservableCollection<PrescriptionNoteTemplates>(allItems.OrderBy(x => x.NoteDetails));
                                NotifyOfPropertyChange(() => GhiChu);
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
                    IsWaitingPrescriptionNoteTemplates_GetAll = false;
                }
            });
            t.Start();
        }
    }
}