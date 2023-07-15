//using eHCMSLanguage;
//using System.Linq;
//using Caliburn.Micro;
//using System.Windows;
//using System.Windows.Controls;
//using System.Collections.ObjectModel;
//using DataEntities;
//using System.Threading;
//using System;
//using System.Windows.Media;
//using System.Windows.Input;
//using System.Text;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using aEMR.Common.BaseModel;
//using aEMR.Infrastructure.Events;
//using Castle.Windsor;
//using aEMR.Infrastructure.CachingUtils;
//using aEMR.ViewContracts;
//using System.ComponentModel.Composition;
//using aEMR.Infrastructure;
//using aEMR.Common.Collections;
//using aEMR.ServiceClient;
//using aEMR.Controls;
//using aEMR.ServiceClient.Consultation_PCLs;
//using aEMR.CommonTasks;
///*
// * 20180922 #001 TTM:   Chuyển lấy Lookup từ gọi về Service sang lấy từ cache trên client. Vì đã có lấy tất cả Lookup lúc đăng nhập rồi không cần phải
// *                      gọi về Service tốn thời gian.
// */
//namespace aEMR.Common.ViewModels
//{
//    //[Export(typeof(IePrescriptionOldNew)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public partial class ePrescriptionOldNewBackupViewModel : ViewModelBase//, IePrescriptionOldNew
//    , IHandle<ePrescriptionDoubleClickEvent>
//    , IHandle<ReloadDataePrescriptionEvent>
//    , IHandle<SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int>>
//    , IHandle<SendPrescriptionDetailSchedulesFormEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int>>
//    , IHandle<DiagnosisTreatmentSelectedEvent<DiagnosisTreatment>>
//    , IHandle<DuocSi_EditingToaThuocEvent>
//    , IHandle<SelectListDrugDoubleClickEvent>
//    , IHandle<PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int>>
//        , IHandle<PrescriptionNoteTempType_Change>
//        , IHandle<PatientChange>
//        , IHandle<ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>>
//        , IHandle<GlobalCurPatientServiceRecordLoadComplete_EPrescript>
//    {
//        [ImportingConstructor]
//        public ePrescriptionOldNewBackupViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
//        {
//            eventArg.Subscribe(this);
            
//            authorization();
//            DrugTypes = new ObservableCollection<Lookup>();
//            ChooseDoses = new ObservableCollection<ChooseDose>();

//            GetAllContrainIndicatorDrugs();
//            ObjPrescriptionNoteTemplates_GetAll = new ObservableCollection<PrescriptionNoteTemplates>();
//            LieuDung = Globals.allPrescriptionDetailSchedulesLieuDung.ToObservableCollection();
//            RefreshLookup();
//            GetAllLookupForDrugTypes();

//            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

//            DrugList = new PagedSortableCollectionView<GetDrugForSellVisitor>();
//            DrugList.OnRefresh += new EventHandler<aEMR.Common.Collections.RefreshEventArgs>(DrugList_OnRefresh);

//            initPatientInfo();
            
//        }
//        public void Handle(ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail> message) 
//        {
//            //initPatientInfo();
//        }
//        public void Handle(GlobalCurPatientServiceRecordLoadComplete_EPrescript message)
//        {
//            initPatientInfo();
//        }
//        public void CheckBeforePrescrip() 
//        {
//            if (Globals.PatientFindBy_ForConsultation.Value == AllLookupValues.PatientFindBy.NGOAITRU)
//            {
//                if (Globals.curLstPatientServiceRecord == null
//                || Globals.curLstPatientServiceRecord.Count < 1)
//                {
//                    IsEnabledForm = false;
//                    HasDiagnosis = false;
//                    ObjDiagnosisTreatment_Current = new DiagnosisTreatment();

//                    btChonChanDoanIsEnabled = false;
//                    btnSaveAddNewIsEnabled = false;
//                    IsEnabledPrint = false;

//                    MessageBox.Show(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return;
//                }
//                else
//                {
//                    ObjDiagnosisTreatment_Current = Globals.curLstPatientServiceRecord[0].DiagnosisTreatments[0];

//                    Globals.ObjGetDiagnosisTreatmentByPtID = ObjectCopier.DeepCopy(ObjDiagnosisTreatment_Current);

//                    HasDiagnosis = true;


//                    if (Globals.curLstPatientServiceRecord[0].PrescriptionIssueHistories == null
//                        || Globals.curLstPatientServiceRecord[0].PrescriptionIssueHistories.Count < 1)
//                    {
//                        PrescripState = PrescriptionState.NewPrescriptionState;
//                        //Đọc Toa Thuốc Cuối lên nếu có
//                        GetPrescriptionTypes_New();
//                    }
//                    else
//                    {
//                        //IsEnabledForm = false;
//                        btnUndoIsEnabled = false;
//                        btnUpdateIsEnabled = false;

//                        //btnEditIsEnabled = true;
//                        //btnCopyToIsEnabled = true;
//                        GetPrescriptionTypes_DaCo();

//                        if (Globals.curLstPatientServiceRecord[0].PrescriptionIssueHistories[0].IssuerStaffID
//                            != Globals.LoggedUserAccount.StaffID)
//                        {
//                            //Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!
//                            PrescripState = PrescriptionState.PublishNewPrescriptionState;
//                        }
//                        else
//                        {
//                            PrescripState = PrescriptionState.EditPrescriptionState;
//                        }

//                    }
//                }
//            }
//            else 
//            {
//                PrescripState = PrescriptionState.NewPrescriptionState;
//                IsEnabledForm = true;
//                loadPrescript = true;
//            }
//        }

//        public void initPatientInfo() 
//        {
//            //Kiem tra trang thai toa thuoc
            
//            #region cu
//            if (Globals.PatientAllDetails.PatientInfo != null)
//            {
//                IsEnabledForm = true;
//                loadPrescript = true;
//                LatestePrecriptions = new Prescription();

//                PrescriptionTypeList = new ObservableCollection<Lookup>();

//                //Danh sach Di Ung
//                MDAllergies_ByPatientID(Globals.PatientAllDetails.PatientInfo.PatientID, 1);
//                //Danh sach Di Ung

//                InitChooseDose();

//                GetMedConditionByPtID(Globals.PatientAllDetails.PatientInfo.PatientID, -1);

//                ObjPrescriptionNoteTemplates_Selected = new PrescriptionNoteTemplates();
//                ObjPrescriptionNoteTemplates_Selected.PrescriptNoteTemplateID = -1;

//                if (Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID > 0)
//                {
//                    //GetDiagnosisTreatmentByPtID(Globals.PatientAllDetails.PatientInfo.PatientID, Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID, "", 1, true);
//                    CheckBeforePrescrip();
//                }
                

//                //if (Globals.ConfigList != null)
//                //{
//                //    xNgayBHToiDa_NgoaiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NgoaiTru]);
//                //    xNgayBHToiDa_NoiTru = Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyMaxDaysHIRebate_NoiTru]);
//                //}
                
//                // Txd 25/05/2014 Replaced ConfigList
//                xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;
//                xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

//                loadPrescript = true;
//            }
//            else
//            {
//                IsEnabledForm = false;
//            }
//            #endregion
//        }

//        #region busy indicator
//        public override bool IsProcessing
//        {
//            get
//            {
//                return _isWaitingSaveDuocSiEdit
//                    || _IsWaitingGetPrescriptionDetailsByPrescriptID
//                    || _IsWaitingGetLatestPrescriptionByPtID
//                    || _IsWaitingChooseDose
//                    || _IsWaitingGetMedConditionByPtID
//                    || _IsWaitingAddPrescriptIssueHistory
//                    || _IsWaitingCapNhatToaThuoc
//                    || _IsWaitingPrescriptions_UpdateDoctorAdvice
//                    || _IsWaitingTaoThanhToaMoi
//                    || _IsWaitingGetAllContrainIndicatorDrugs
//                    || _IsWaitingGetDiagnosisTreatmentByPtID
//                    || _IsWaitingPrescriptionNoteTemplates_GetAll
//                    || _loadPrescript;
//            }
//        }
//        public override string StatusText
//        {
//            get
//            {
//                if (_isWaitingSaveDuocSiEdit)
//                {
//                    return eHCMSResources.Z0343_G1_DangLuu;
//                }
//                if (_IsWaitingGetPrescriptionDetailsByPrescriptID)
//                {
//                    return eHCMSResources.Z1047_G1_TTinCTietToaCuoi;
//                }
//                if (_IsWaitingGetLatestPrescriptionByPtID)
//                {
//                    return eHCMSResources.Z1048_G1_TTinToaCuoi;
//                }
//                if (_IsWaitingGetPrescriptionTypes)
//                {
//                    return eHCMSResources.T2837_G1_LoaiToa;
//                }
//                if (_IsWaitingGetMedConditionByPtID)
//                {
//                    return eHCMSResources.K2871_G1_DangLoadDLieu;
//                }
//                if (_IsWaitingAddPrescriptIssueHistory)
//                {
//                    return eHCMSResources.Z1049_G1_PhatHanhLaiToa;
//                }
//                if (_IsWaitingCapNhatToaThuoc)
//                {
//                    return eHCMSResources.Z1050_G1_CNhatToa;
//                }
//                if (_IsWaitingPrescriptions_UpdateDoctorAdvice)
//                {
//                    return eHCMSResources.K1661_G1_CNhatLoiDan;
//                }
//                if (_IsWaitingTaoThanhToaMoi)
//                {
//                    return eHCMSResources.Z1051_G1_DangLuuToa;
//                }
//                if (_IsWaitingGetAllContrainIndicatorDrugs)
//                {
//                    return eHCMSResources.K2882_G1_DangTaiDLieu;
//                }
//                if (_IsWaitingGetDiagnosisTreatmentByPtID)
//                {
//                    return eHCMSResources.Z1052_G1_DangLayTTinCDoan;
//                }
//                if (_IsWaitingPrescriptionNoteTemplates_GetAll)
//                {
//                    return eHCMSResources.Z1053_G1_DangLayDSLoiDan;
//                }
//                if (_IsWaitingMDAllergies_ByPatientID)
//                {
//                    return eHCMSResources.Z1054_G1_DangLayDSThuocDiUng;
//                }
//                return string.Empty;
//            }
//        }

//        private bool _loadPrescript=false;
//        public bool loadPrescript
//        {
//            get { return _loadPrescript; }
//            set
//            {
//                if (_loadPrescript != value)
//                {
//                    _loadPrescript = value;
//                    NotifyOfPropertyChange(() => loadPrescript);
//                    NotifyOfPropertyChange(() => IsProcessing);
//                }
//            }
//        }

//        private bool _IsWaitingMDAllergies_ByPatientID;
//        public bool IsWaitingMDAllergies_ByPatientID
//        {
//            get { return _IsWaitingMDAllergies_ByPatientID; }
//            set
//            {
//                if (_IsWaitingMDAllergies_ByPatientID != value)
//                {
//                    _IsWaitingMDAllergies_ByPatientID = value;
//                    NotifyOfPropertyChange(() => IsWaitingMDAllergies_ByPatientID);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _IsWaitingPrescriptionNoteTemplates_GetAll;
//        public bool IsWaitingPrescriptionNoteTemplates_GetAll
//        {
//            get { return _IsWaitingPrescriptionNoteTemplates_GetAll; }
//            set
//            {
//                if (_IsWaitingPrescriptionNoteTemplates_GetAll != value)
//                {
//                    _IsWaitingPrescriptionNoteTemplates_GetAll = value;
//                    NotifyOfPropertyChange(() => IsWaitingPrescriptionNoteTemplates_GetAll);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _isWaitingSaveDuocSiEdit;
//        public bool IsWaitingSaveDuocSiEdit
//        {
//            get { return _isWaitingSaveDuocSiEdit; }
//            set
//            {
//                if (_isWaitingSaveDuocSiEdit != value)
//                {
//                    _isWaitingSaveDuocSiEdit = value;
//                    NotifyOfPropertyChange(() => IsWaitingSaveDuocSiEdit);
//                    NotifyWhenBusy();

//                    NotifyOfPropertyChange(() => CanbtDuocSiEdit);
//                }
//            }
//        }

//        private bool _IsWaitingGetPrescriptionDetailsByPrescriptID;
//        public bool IsWaitingGetPrescriptionDetailsByPrescriptID
//        {
//            get { return _IsWaitingGetPrescriptionDetailsByPrescriptID; }
//            set
//            {
//                if (_IsWaitingGetPrescriptionDetailsByPrescriptID != value)
//                {
//                    _IsWaitingGetPrescriptionDetailsByPrescriptID = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetPrescriptionDetailsByPrescriptID);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _IsWaitingGetLatestPrescriptionByPtID;
//        public bool IsWaitingGetLatestPrescriptionByPtID
//        {
//            get { return _IsWaitingGetLatestPrescriptionByPtID; }
//            set
//            {
//                if (_IsWaitingGetLatestPrescriptionByPtID != value)
//                {
//                    _IsWaitingGetLatestPrescriptionByPtID = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetLatestPrescriptionByPtID);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _IsWaitingChooseDose;
//        public bool IsWaitingChooseDose
//        {
//            get { return _IsWaitingChooseDose; }
//            set
//            {
//                if (_IsWaitingChooseDose != value)
//                {
//                    _IsWaitingChooseDose = value;
//                    NotifyOfPropertyChange(() => IsWaitingChooseDose);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _IsWaitingGetPrescriptionTypes;
//        public bool IsWaitingGetPrescriptionTypes
//        {
//            get { return _IsWaitingGetPrescriptionTypes; }
//            set
//            {
//                if (_IsWaitingGetPrescriptionTypes != value)
//                {
//                    _IsWaitingGetPrescriptionTypes = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetPrescriptionTypes);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingGetMedConditionByPtID;
//        public bool IsWaitingGetMedConditionByPtID
//        {
//            get { return _IsWaitingGetMedConditionByPtID; }
//            set
//            {
//                if (_IsWaitingGetMedConditionByPtID != value)
//                {
//                    _IsWaitingGetMedConditionByPtID = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetMedConditionByPtID);
//                    NotifyWhenBusy();
//                }
//            }
//        }


//        private bool _IsWaitingAddPrescriptIssueHistory;
//        public bool IsWaitingAddPrescriptIssueHistory
//        {
//            get { return _IsWaitingAddPrescriptIssueHistory; }
//            set
//            {
//                if (_IsWaitingAddPrescriptIssueHistory != value)
//                {
//                    _IsWaitingAddPrescriptIssueHistory = value;
//                    NotifyOfPropertyChange(() => IsWaitingAddPrescriptIssueHistory);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingCapNhatToaThuoc;
//        public bool IsWaitingCapNhatToaThuoc
//        {
//            get { return _IsWaitingCapNhatToaThuoc; }
//            set
//            {
//                if (_IsWaitingCapNhatToaThuoc != value)
//                {
//                    _IsWaitingCapNhatToaThuoc = value;
//                    NotifyOfPropertyChange(() => IsWaitingCapNhatToaThuoc);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingPrescriptions_UpdateDoctorAdvice;
//        public bool IsWaitingPrescriptions_UpdateDoctorAdvice
//        {
//            get { return _IsWaitingPrescriptions_UpdateDoctorAdvice; }
//            set
//            {
//                if (_IsWaitingPrescriptions_UpdateDoctorAdvice != value)
//                {
//                    _IsWaitingPrescriptions_UpdateDoctorAdvice = value;
//                    NotifyOfPropertyChange(() => IsWaitingPrescriptions_UpdateDoctorAdvice);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingTaoThanhToaMoi;
//        public bool IsWaitingTaoThanhToaMoi
//        {
//            get { return _IsWaitingTaoThanhToaMoi; }
//            set
//            {
//                if (_IsWaitingTaoThanhToaMoi != value)
//                {
//                    _IsWaitingTaoThanhToaMoi = value;
//                    NotifyOfPropertyChange(() => IsWaitingTaoThanhToaMoi);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        private bool _IsWaitingGetAllContrainIndicatorDrugs;
//        public bool IsWaitingGetAllContrainIndicatorDrugs
//        {
//            get { return _IsWaitingGetAllContrainIndicatorDrugs; }
//            set
//            {
//                if (_IsWaitingGetAllContrainIndicatorDrugs != value)
//                {
//                    _IsWaitingGetAllContrainIndicatorDrugs = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetAllContrainIndicatorDrugs);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        #endregion


//        private IePrescriptionCommentaryAutoComplete _ePreComAutoCompleteContent;

//        public IePrescriptionCommentaryAutoComplete ePreComAutoCompleteContent
//        {
//            get { return _ePreComAutoCompleteContent; }
//            set
//            {
//                if (_ePreComAutoCompleteContent != value)
//                {
//                    _ePreComAutoCompleteContent = value;
//                    NotifyOfPropertyChange(() => ePreComAutoCompleteContent);
//                }
//            }
//        }

//        public enum DataGridCol
//        {
//            DEL = 0,
//            //Down = DEL + 1,
//            //Up = Down + 1,
//            HI = DEL + 1,
//            Schedule = HI + 1,
//            //DrugNotInCat = Schedule+1,
//            NotInCat = Schedule + 1,
//            DRUG_NAME = NotInCat + 1,
//            //STRENGHT =DRUG_NAME+ 1,
//            UNITS = DRUG_NAME + 1,
//            UNITUSE = UNITS + 1,
//            DRUG_TYPE = UNITUSE + 1,
            
//            //DOSAGE = DRUG_TYPE + 1,
//            //CHOOSE = DOSAGE + 1,

//            MDOSE = DRUG_TYPE + 1,
//            ADOSE = MDOSE+1,
//            EDOSE =ADOSE+ 1,
//            NDOSE =EDOSE+ 1,
//            Dayts =NDOSE+ 1,
//            DaytExtended = Dayts+1,
//            QTY = DaytExtended+1,
//            //UNITUSE = QTY+1,
//            USAGE = QTY + 1,
//            INSTRUCTION = USAGE+1
//        }

//        public List<long> lstMCTypeID;

//        public object UCAllergiesWarningByPatientID
//        {
//            get;
//            set;
//        }

//        private bool _DuocSi_IsEditingToaThuoc = false;

//        public bool DuocSi_IsEditingToaThuoc
//        {
//            get { return _DuocSi_IsEditingToaThuoc; }
//            set
//            {
//                if (_DuocSi_IsEditingToaThuoc != value)
//                {
//                    _DuocSi_IsEditingToaThuoc = value;
//                    NotifyOfPropertyChange(() => DuocSi_IsEditingToaThuoc);
//                }
//            }
//        }

//        private int xNgayBHToiDa_NgoaiTru = 30;
//        private int xNgayBHToiDa_NoiTru = 5;
//        private long? StoreID = 2;//tam thoi mac dinh kho ban(nha thuoc benh vien)
//        private IEnumerator<IResult> DoGetStore_EXTERNAL()
//        {
//            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
//            yield return paymentTypeTask;
//            if (paymentTypeTask != null && paymentTypeTask.LookupList != null)
//            {
//                StoreID = paymentTypeTask.LookupList.Where(x => x.IsMain == true).FirstOrDefault().StoreID;
//                if (StoreID != null && StoreID > 0)
//                {
//                    yield break;
//                }
//                else
//                {
//                    StoreID = paymentTypeTask.LookupList.FirstOrDefault().StoreID;
//                }
//            }
//            yield break;
//        }


//        public void Handle(PatientChange obj)
//        {
//            //Initialize();
//        }

//        void DrugList_OnRefresh(object sender, aEMR.Common.Collections.RefreshEventArgs e)
//        {
//            SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, false);
//        }


//        public void Init()
//        {
//            //Load UC Dị Ứng/Cảnh Báo
//            var uc1 = Globals.GetViewModel<IAllergiesWarning_ByPatientID>();
//            UCAllergiesWarningByPatientID = uc1;
//            this.ActivateItem(uc1);
//            //Load UC Dị Ứng/Cảnh Báo
//        }


//        #region Properties member

//        private bool _btDuocSiEditVisibility = false;
//        public bool btDuocSiEditVisibility
//        {
//            get
//            {
//                return _btDuocSiEditVisibility;
//            }
//            set
//            {
//                if (_btDuocSiEditVisibility != value)
//                {
//                    _btDuocSiEditVisibility = value;
//                    NotifyOfPropertyChange(() => btDuocSiEditVisibility);
//                }
//            }
//        }


//        private bool _IsEnabledForm = false;
//        public bool IsEnabledForm
//        {
//            get
//            {
//                return _IsEnabledForm;
//            }
//            set
//            {
//                if (_IsEnabledForm != value)
//                {
//                    _IsEnabledForm = value;
//                    NotifyOfPropertyChange(() => IsEnabledForm);
//                }
//            }
//        }


//        private DiagnosisTreatment _ObjDiagnosisTreatment_Current;
//        public DiagnosisTreatment ObjDiagnosisTreatment_Current
//        {
//            get
//            {
//                return _ObjDiagnosisTreatment_Current;
//            }
//            set
//            {
//                if (_ObjDiagnosisTreatment_Current != value)
//                {
//                    _ObjDiagnosisTreatment_Current = value;
//                    NotifyOfPropertyChange(() => ObjDiagnosisTreatment_Current);
//                }
//            }
//        }

//        private ObservableCollection<ChooseDose> _ChooseDoses;
//        public ObservableCollection<ChooseDose> ChooseDoses
//        {
//            get
//            {
//                return _ChooseDoses;
//            }
//            set
//            {
//                if (_ChooseDoses != value)
//                {
//                    _ChooseDoses = value;
//                    NotifyOfPropertyChange(() => ChooseDoses);
//                }
//            }
//        }

//        private PrescriptionDetail _SelectedPrescriptionDetail;
//        public PrescriptionDetail SelectedPrescriptionDetail
//        {
//            get
//            {
//                return _SelectedPrescriptionDetail;
//            }
//            set
//            {
//                if (_SelectedPrescriptionDetail != value)
//                {
//                    _SelectedPrescriptionDetail = value;                    
//                    NotifyOfPropertyChange(() => SelectedPrescriptionDetail);
//                }
//            }
//        }

//        private PrescriptionDetail _tempPrescriptionDetail;
//        public PrescriptionDetail tempPrescriptionDetail
//        {
//            get
//            {
//                return _tempPrescriptionDetail;
//            }
//            set
//            {
//                if (_tempPrescriptionDetail != value)
//                {
//                    _tempPrescriptionDetail = value;
//                    NotifyOfPropertyChange(() => tempPrescriptionDetail);
//                }
//            }
//        }

//        private PagedSortableCollectionView<GetDrugForSellVisitor> _Drugs;
//        public PagedSortableCollectionView<GetDrugForSellVisitor> DrugList
//        {
//            get
//            {
//                return _Drugs;
//            }
//            set
//            {
//                if (_Drugs != value)
//                {
//                    _Drugs = value;
//                    NotifyOfPropertyChange(() => DrugList);
//                }
//            }
//        }

//        private ObservableCollection<Staff> _StaffCatgs;
//        public ObservableCollection<Staff> StaffCatgs
//        {
//            get
//            {
//                return _StaffCatgs;
//            }
//            set
//            {
//                if (_StaffCatgs != value)
//                {
//                    _StaffCatgs = value;
//                    NotifyOfPropertyChange(() => StaffCatgs);
//                }
//            }
//        }

//        private bool _IsEnabledAutoComplete;
//        public bool IsEnabledAutoComplete
//        {
//            get
//            {
//                return _IsEnabledAutoComplete;
//            }
//            set
//            {
//                if (_IsEnabledAutoComplete != value)
//                {
//                    _IsEnabledAutoComplete = value;
//                    NotifyOfPropertyChange(() => IsEnabledAutoComplete);
//                }
//            }
//        }

//        private Prescription _LatestePrecriptions;
//        public Prescription LatestePrecriptions
//        {
//            get
//            {
//                return _LatestePrecriptions;
//            }
//            set
//            {
//                if (_LatestePrecriptions != value)
//                {
//                    _LatestePrecriptions = value;
//                    NotifyOfPropertyChange(() => LatestePrecriptions);
//                }
//            }
//        }

//        private Prescription _PrecriptionsForPrint;
//        public Prescription PrecriptionsForPrint
//        {
//            get
//            {
//                return _PrecriptionsForPrint;
//            }
//            set
//            {
//                if (_PrecriptionsForPrint != value)
//                {
//                    _PrecriptionsForPrint = value;
//                    NotifyOfPropertyChange(() => PrecriptionsForPrint);
//                }
//            }
//        }

//        private Prescription _RetoreLatestePrecriptions;
//        public Prescription RetoreLatestePrecriptions
//        {
//            get
//            {
//                return _RetoreLatestePrecriptions;
//            }
//            set
//            {
//                if (_RetoreLatestePrecriptions != value)
//                {
//                    _RetoreLatestePrecriptions = value;
//                    NotifyOfPropertyChange(() => RetoreLatestePrecriptions);
//                }
//            }
//        }

//        private int _NumOfDays = 7;
//        public int NumOfDays
//        {
//            get
//            {
//                return _NumOfDays;
//            }
//            set
//            {
//                if (_NumOfDays != value)
//                {
//                    _NumOfDays = value;
//                    NotifyOfPropertyChange(() => NumOfDays);
//                }
//            }
//        }

//        private ObservableCollection<Lookup> _PrescriptionType;
//        public ObservableCollection<Lookup> PrescriptionTypeList
//        {
//            get
//            {
//                return _PrescriptionType;
//            }
//            set
//            {
//                if (_PrescriptionType != value)
//                {
//                    _PrescriptionType = value;
//                    NotifyOfPropertyChange(() => PrescriptionTypeList);
//                }
//            }
//        }

//        private Lookup _CurrentPrescriptionType;
//        public Lookup CurrentPrescriptionType
//        {
//            get
//            {
//                return _CurrentPrescriptionType;
//            }
//            set
//            {
//                if (_CurrentPrescriptionType != value)
//                {
//                    _CurrentPrescriptionType = value;
//                    NotifyOfPropertyChange(() => CurrentPrescriptionType);
//                }
//            }
//        }

//        //private Visibility _IsVisibility = Visibility.Collapsed;
//        //public Visibility IsVisibility
//        //{
//        //    get
//        //    {
//        //        return _IsVisibility;
//        //    }
//        //    set
//        //    {
//        //        if (_IsVisibility != value)
//        //        {
//        //            _IsVisibility = value;
//        //            NotifyOfPropertyChange(() => IsVisibility);
//        //        }
//        //    }
//        //}

//        //private Visibility _HideButton = Visibility.Visible;
//        //public Visibility HideButton
//        //{
//        //    get { return _HideButton; }
//        //    set
//        //    {
//        //        if (_HideButton != value)
//        //        {
//        //            _HideButton = value;
//        //            NotifyOfPropertyChange(() => HideButton);
//        //        }
//        //    }
//        //}

//        private bool _IsEnabled;
//        public bool IsEnabled
//        {
//            get
//            {
//                return _IsEnabled;
//            }
//            set
//            {
//                _IsEnabled = value;
//                NotifyOfPropertyChange(() => IsEnabled);
//                //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);
//            }
//        }

//        //private bool _IsEnabledSave = true;
//        //public bool IsEnabledSave
//        //{
//        //    get
//        //    {
//        //        return _IsEnabledSave;
//        //    }
//        //    set
//        //    {
//        //        if (_IsEnabledSave != value)
//        //        {
//        //            _IsEnabledSave = value;
//        //            //IsVisibility = IsEnabledSave ? Visibility.Collapsed : Visibility.Visible;
//        //            NotifyOfPropertyChange(() => IsEnabledSave);
//        //        }
//        //    }
//        //}


//        private bool _IsEnabledPrint = true;
//        public bool IsEnabledPrint
//        {
//            get
//            {
//                return _IsEnabledPrint;
//            }
//            set
//            {
//                if (_IsEnabledPrint != value)
//                {
//                    _IsEnabledPrint = value;
//                    NotifyOfPropertyChange(() => IsEnabledPrint);
//                }
//            }
//        }


//        private bool _CanPrint;
//        public bool CanPrint
//        {
//            get
//            {
//                return _CanPrint;
//            }
//            set
//            {
//                if (_CanPrint != value)
//                {
//                    _CanPrint = value;
//                    //CanUndo = !CanPrint;
//                    NotifyOfPropertyChange(() => CanPrint);
//                }
//            }
//        }

//        private int _ClassificationPatient;
//        public int ClassificationPatient
//        {
//            get
//            {
//                return _ClassificationPatient;
//            }
//            set
//            {
//                _ClassificationPatient = value;
//                NotifyOfPropertyChange(() => ClassificationPatient);
//            }
//        }

//        private ObservableCollection<MedicalConditionRecord> _PtMedCond;
//        public ObservableCollection<MedicalConditionRecord> PtMedCond
//        {
//            get
//            {
//                return _PtMedCond;
//            }
//            set
//            {
//                if (_PtMedCond == value)
//                    return;
//                _PtMedCond = value;
//                NotifyOfPropertyChange(() => PtMedCond);
//            }
//        }

//        #endregion

//        public void authorization()
//        {
//            if (!Globals.isAccountCheck)
//            {
//                return;
//            }
//        }
//        #region account checking

//        private bool _mToaThuocDaPhatHanh_ThongTin = true;
//        private bool _mToaThuocDaPhatHanh_ChinhSua = true;
//        private bool _mToaThuocDaPhatHanh_TaoToaMoi = true;
//        private bool _mToaThuocDaPhatHanh_PhatHanhLai = true;
//        private bool _mToaThuocDaPhatHanh_In = true;
//        private bool _mToaThuocDaPhatHanh_ChonChanDoan = true;
//        private bool _hasTitle = true;

//        public bool mToaThuocDaPhatHanh_ThongTin
//        {
//            get
//            {
//                return _mToaThuocDaPhatHanh_ThongTin;
//            }
//            set
//            {
//                if (_mToaThuocDaPhatHanh_ThongTin == value)
//                    return;
//                _mToaThuocDaPhatHanh_ThongTin = value;
//            }
//        }
//        public bool mToaThuocDaPhatHanh_ChinhSua
//        {
//            get
//            {
//                return _mToaThuocDaPhatHanh_ChinhSua;
//            }
//            set
//            {
//                if (_mToaThuocDaPhatHanh_ChinhSua == value)
//                    return;
//                _mToaThuocDaPhatHanh_ChinhSua = value;
//            }
//        }
//        public bool mToaThuocDaPhatHanh_TaoToaMoi
//        {
//            get
//            {
//                return _mToaThuocDaPhatHanh_TaoToaMoi;
//            }
//            set
//            {
//                if (_mToaThuocDaPhatHanh_TaoToaMoi == value)
//                    return;
//                _mToaThuocDaPhatHanh_TaoToaMoi = value;
//            }
//        }
//        public bool mToaThuocDaPhatHanh_PhatHanhLai
//        {
//            get
//            {
//                return _mToaThuocDaPhatHanh_PhatHanhLai;
//            }
//            set
//            {
//                if (_mToaThuocDaPhatHanh_PhatHanhLai == value)
//                    return;
//                _mToaThuocDaPhatHanh_PhatHanhLai = value;
//            }
//        }
//        public bool mToaThuocDaPhatHanh_In
//        {
//            get
//            {
//                return _mToaThuocDaPhatHanh_In;
//            }
//            set
//            {
//                if (_mToaThuocDaPhatHanh_In == value)
//                    return;
//                _mToaThuocDaPhatHanh_In = value;
//            }
//        }
//        public bool mToaThuocDaPhatHanh_ChonChanDoan
//        {
//            get
//            {
//                return _mToaThuocDaPhatHanh_ChonChanDoan;
//            }
//            set
//            {
//                if (_mToaThuocDaPhatHanh_ChonChanDoan == value)
//                    return;
//                _mToaThuocDaPhatHanh_ChonChanDoan = value;
//            }
//        }
//        public bool hasTitle
//        {
//            get
//            {
//                return _hasTitle;
//            }
//            set
//            {
//                if (_hasTitle == value)
//                    return;
//                _hasTitle = value;
//            }
//        }
//        #endregion

//        public void GetPrescriptionDetailsByPrescriptID(long prescriptID)
//        {
//            IsWaitingGetPrescriptionDetailsByPrescriptID = true;
            
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetPrescriptionDetailsByPrescriptID(prescriptID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var Results = contract.EndGetPrescriptionDetailsByPrescriptID(asyncResult);
//                            LatestePrecriptions.PrescriptionDetails = Results.ToObservableCollection();
//                            BackupCurPrescriptionItem();
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            //Globals.IsBusy = false;
//                            IsWaitingGetPrescriptionDetailsByPrescriptID = false;
                            
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private bool _ContentKhungTaiKhamIsEnabled = true;
//        public bool ContentKhungTaiKhamIsEnabled
//        {
//            get
//            {
//                return _ContentKhungTaiKhamIsEnabled;
//            }
//            set
//            {
//                if (_ContentKhungTaiKhamIsEnabled != value)
//                {
//                    _ContentKhungTaiKhamIsEnabled = value;
//                    NotifyOfPropertyChange(() => ContentKhungTaiKhamIsEnabled);
//                }
//            }
//        }


//        private void GetLatestPrescriptionByPtID(long PatientID)
//        {

//            IsWaitingGetLatestPrescriptionByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetLatestPrescriptionByPtID(PatientID, Globals.DispatchCallback((asyncResult) =>
//                    {

//                        try
//                        {
//                            var ToaThuoc_Cuoi = contract.EndGetLatestPrescriptionByPtID(asyncResult);

//                            if (ToaThuoc_Cuoi != null && ToaThuoc_Cuoi.IssueID > 0)
//                            {
//                                btnCreateNewIsEnabled = true;
//                                btnSaveAddNewIsEnabled = false;
//                                if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc, ToaThuoc_Cuoi) == false)
//                                {
//                                    btnEditIsEnabled = false;
//                                }
//                                else
//                                {
//                                    btnEditIsEnabled = true;
//                                }
//                                btnCreateAndCopyIsEnabled = true;
//                                btnCopyToIsEnabled = true;
//                                IsEnabledPrint = true;

//                                LatestePrecriptions = ToaThuoc_Cuoi;
//                                PrecriptionsForPrint = LatestePrecriptions;

//                                ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

//                                SoNgayDungThuoc_Root = LatestePrecriptions.NDay;

//                                if (LatestePrecriptions.NDay > 0)
//                                {
//                                    chkHasAppointmentValue = true;
//                                }
//                                else
//                                {
//                                    chkHasAppointmentValue = false;
//                                }

//                                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
//                                ////Toa thuoc nay co roi
//                                ////check xem neu la cung mot bac si thi chinh sua
//                                ////khac bac si thi la phat hanh lai toa thuoc
//                                //PrescripState = PrescriptionState.EditPrescriptionState;
//                            }
//                            else/*Toa Mới*/
//                            {
//                                LatestePrecriptions = NewPrecriptions.DeepCopy();

//                                AddNewBlankDrugIntoPrescriptObjectNew();

//                                IsEnabled = true;

//                                btChonChanDoanIsEnabled = true;

//                                btnCreateNewIsEnabled = false;
//                                btnSaveAddNewIsEnabled = true;
//                                btnUndoIsEnabled = true;
//                                btnEditIsEnabled = false;
//                                btnCopyToIsEnabled = false;
//                                IsEnabledPrint = false;
//                                //dinh chuyen sanh trang thai tao toa moi
//                                PrescripState = PrescriptionState.NewPrescriptionState;
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsWaitingGetLatestPrescriptionByPtID = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private void GetLatestPrescriptionByPtID_New(long PatientID)
//        {
//            //Danh cho truong hop la toa moi
//            IsWaitingGetLatestPrescriptionByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginGetLatestPrescriptionByPtID(PatientID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var ToaThuoc_Cuoi = contract.EndGetLatestPrescriptionByPtID(asyncResult);

//                            if (ToaThuoc_Cuoi != null && ToaThuoc_Cuoi.IssueID > 0)
//                            {
//                                btnCreateNewIsEnabled = true;
//                                btnSaveAddNewIsEnabled = false;
//                                if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc, ToaThuoc_Cuoi) == false)
//                                {
//                                    btnEditIsEnabled = false;
//                                }
//                                else
//                                {
//                                    btnEditIsEnabled = true;
//                                }
//                                btnCreateAndCopyIsEnabled = true;
//                                btnCopyToIsEnabled = true;
//                                IsEnabledPrint = true;

//                                LatestePrecriptions = ToaThuoc_Cuoi;
                                
//                                LatestePrecriptions.ObjDiagnosisTreatment=Globals.curLstPatientServiceRecord[0].DiagnosisTreatments[0];

//                                PrecriptionsForPrint = LatestePrecriptions;

//                                ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

//                                SoNgayDungThuoc_Root = LatestePrecriptions.NDay;

//                                if (LatestePrecriptions.NDay > 0)
//                                {
//                                    chkHasAppointmentValue = true;
//                                }
//                                else
//                                {
//                                    chkHasAppointmentValue = false;
//                                }

//                                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
//                                ////Toa thuoc nay co roi
//                                ////check xem neu la cung mot bac si thi chinh sua
//                                ////khac bac si thi la phat hanh lai toa thuoc
//                                //PrescripState = PrescriptionState.EditPrescriptionState;
//                            }
//                            else/*Toa Mới*/
//                            {
//                                LatestePrecriptions = NewPrecriptions.DeepCopy();
//                                BackupCurPrescriptionItem();
//                                AddNewBlankDrugIntoPrescriptObjectNew();

//                                IsEnabled = true;

//                                btChonChanDoanIsEnabled = true;

//                                btnCreateNewIsEnabled = false;
//                                btnSaveAddNewIsEnabled = true;
//                                btnUndoIsEnabled = true;
//                                btnEditIsEnabled = false;
//                                btnCopyToIsEnabled = false;
//                                IsEnabledPrint = false;
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsWaitingGetLatestPrescriptionByPtID = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private void InitChooseDose()
//        {
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            IsWaitingChooseDose = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginInitChooseDoses(Globals.DispatchCallback((asyncResult) =>
//                    {

//                        try
//                        {
//                            var results = contract.EndInitChooseDoses(asyncResult);
//                            if (results != null)
//                            {
//                                if (ChooseDoses == null)
//                                {
//                                    ChooseDoses = new ObservableCollection<ChooseDose>();
//                                }
//                                else
//                                {
//                                    ChooseDoses.Clear();
//                                }
//                                foreach (ChooseDose p in results)
//                                {
//                                    ChooseDoses.Add(p);
//                                }
//                                NotifyOfPropertyChange(() => ChooseDoses);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsWaitingChooseDose = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        public object GetChooseDose(object value)
//        {
//            PrescriptionDetail p = value as PrescriptionDetail;
//            if (p != null)
//            {
//                return p.ChooseDose;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public void cbxChooseDose_Loaded(object sender, RoutedEventArgs e)
//        {
//            var comboBox = sender as ComboBox;
//            if (comboBox != null)
//            {
//                comboBox.ItemsSource = ChooseDoses;
//                if (SelectedPrescriptionDetail != null)
//                {
//                    comboBox.SelectedItem = GetChooseDose(SelectedPrescriptionDetail);
//                }
//            }
//        }

//        public void cbxChooseDoseForm_Loaded(object sender, RoutedEventArgs e)
//        {
//            var comboBox = sender as ComboBox;
//            if (comboBox != null)
//            {
//                comboBox.ItemsSource = ChooseDoses;
//                if (tempPrescriptionDetail != null)
//                {
//                    comboBox.SelectedItem = GetChooseDose(tempPrescriptionDetail);
//                }
//            }
//        }

//        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
//        {

//            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
//            for (int idx = 1; idx < grdPrescription.Columns.Count; idx++)
//            {
//                //if (idx == (int)DataGridCol.STRENGHT || idx == (int)DataGridCol.UNITS || idx == (int)DataGridCol.UNITUSE || idx == (int)DataGridCol.USAGE || idx == (int)DataGridCol.QTY)
//                //{
//                //    TextBox obj = grdPrescription.Columns[idx].GetCellContent(e.Row) as TextBox;

//                //    if (obj != null)
//                //    {
//                //        obj.IsReadOnly = true;
//                //        //if ((e.Row.GetIndex() % 2) == 1)
//                //        obj.Background = new SolidColorBrush(Color.FromArgb(245, 228, 228, 231));
//                //        //else
//                //        //obj.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
//                //    }
//                //}

                
//            }

//            PrescriptionDetail objRows = e.Row.DataContext as PrescriptionDetail;
            
//            if (objRows.HasSchedules)
//            {
//                e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120));
//            }
//            else
//            {
//                e.Row.Background = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248));
//            }

//            switch (objRows.V_DrugType)
//            {
//                //case (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG:
//                //    e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120)); break;
//                //case (long)AllLookupValues.V_DrugType.THUOC_NGOAIDANH_MUC:
//                //    e.Row.Background = new SolidColorBrush(Colors.Green); break;
//                case (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN:
//                    e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120)); break;
//                //default: e.Row.Background = new SolidColorBrush(Color.FromArgb(205, 180, 200, 120)); break;
//            }

//            if (objRows.SelectedDrugForPrescription != null
//                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed != null
//                            && objRows.SelectedDrugForPrescription.MaxDayPrescribed > 0)
//            {
//                e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));//pink
//                objRows.BackGroundColor = "#E79DEA";
//                NotifyOfPropertyChange(() => e.Row.Background);
//            }
//        }

//        private string BrandName;
//        private int IsInsurance;

//        private void SearchDrugForPrescription_Paging(long? StoreID, int PageIndex, int PageSize, bool CountTotal)
//        {
//            DrugList.Clear();

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var client = serviceFactory.ServiceInstance;
//                    client.BeginSearchDrugForPrescription_Paging(BrandName, false, IsInsurance, StoreID, PageIndex, PageSize, CountTotal, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        int Total = 0;
//                        IList<GetDrugForSellVisitor> allItems = null;
//                        bool bOK = false;
//                        try
//                        {
//                            allItems = client.EndSearchDrugForPrescription_Paging(out Total, asyncResult);
//                            bOK = true;
//                        }
//                        catch (Exception innerEx)
//                        {
//                            MessageBox.Show(innerEx.ToString());
//                        }

//                        if (bOK)
//                        {
//                            if (CountTotal)
//                            {
//                                DrugList.TotalItemCount = Total;
//                            }
//                            if (allItems != null)
//                            {
//                                foreach (var item in allItems)
//                                {
//                                    DrugList.Add(item);
//                                }
//                                if(AutoGenMedProduct!=null)
//                                {
//                                    AutoGenMedProduct.ItemsSource = ObjectCopier.DeepCopy( DrugList);                                
//                                    AutoGenMedProduct.PopulateComplete();
//                                }

//                                if (AutoThuoc!=null)
//                                {
//                                    AutoThuoc.ItemsSource = ObjectCopier.DeepCopy(DrugList);
//                                    AutoThuoc.PopulateComplete();
//                                }
//                            }
//                        }
//                    }), null);

//                }

//            });

//            t.Start();
//        }


//        private PrescriptionDetail _PrescriptionDetailPreparing;
//        public PrescriptionDetail PrescriptionDetailPreparing
//        {
//            get
//            {
//                return _PrescriptionDetailPreparing;
//            }
//            set
//            {
//                if (_PrescriptionDetailPreparing != value)
//                {
//                    _PrescriptionDetailPreparing = value;
//                    NotifyOfPropertyChange(() => PrescriptionDetailPreparing);
//                }
//            }
//        }

//        public void grdPrescription_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
//        {
//            PrescriptionDetailPreparing = (e.Row.DataContext as PrescriptionDetail).DeepCopy();
            
            
//            //if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.DRUG_NAME].GetCellContent(PrescriptionDetailPreparing) as AxAutoComplete).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            //(grdPrescription.Columns[(int)DataGridCol.DRUG_NAME].GetCellContent(PrescriptionDetailPreparing) as AxAutoComplete).IsEnabled = true;
//            //            BrandName = PrescriptionDetailPreparing.SelectedDrugForPrescription.BrandName;
//            //        }
//            //    }
//            //    else
//            //    {
//            //        BrandName = "";
//            //    }
//            //}
//            ////if (e.Column.DisplayIndex == (int)DataGridCol.DOSAGE)
//            ////{
//            ////    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            ////    {
//            ////        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            ////        //{
//            ////        //    (grdPrescription.Columns[(int)DataGridCol.DOSAGE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            ////        //}
//            ////        //else
//            ////        {
//            ////            (grdPrescription.Columns[(int)DataGridCol.DOSAGE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            ////        }
//            ////    }
//            ////}

//            ////if (e.Column.DisplayIndex == (int)DataGridCol.CHOOSE)
//            ////{
//            ////    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            ////    {
//            ////        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            ////        //{
//            ////        //    (grdPrescription.Columns[(int)DataGridCol.CHOOSE].GetCellContent(PrescriptionDetailPreparing) as KeyEnabledComboBox).IsEnabled = false;
//            ////        //}
//            ////        //else
//            ////        {
//            ////            (grdPrescription.Columns[(int)DataGridCol.CHOOSE].GetCellContent(PrescriptionDetailPreparing) as KeyEnabledComboBox).IsEnabled = true;
//            ////        }
//            ////    }
//            ////}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.MDOSE)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.MDOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.MDOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.ADOSE)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.ADOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.ADOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.EDOSE)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.EDOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.EDOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.NDOSE)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.NDOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.NDOSE].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.Dayts)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.Dayts].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            if (checkDrugType(PrescriptionDetailPreparing))
//            //            {
//            //                (grdPrescription.Columns[(int)DataGridCol.Dayts].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //            }
//            //            else 
//            //            {
//            //                (grdPrescription.Columns[(int)DataGridCol.Dayts].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //            }
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.DaytExtended)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.DaytExtended].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.DaytExtended].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.DaytExtended)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.DaytExtended].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.DaytExtended].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//            //if (e.Column.DisplayIndex == (int)DataGridCol.QTY)
//            //{
//            //    if (PrescriptionDetailPreparing != null && PrescriptionDetailPreparing.SelectedDrugForPrescription != null)
//            //    {
//            //        //if (PrescriptionDetailPreparing.IsDrugNotInCat)
//            //        //{
//            //        //    (grdPrescription.Columns[(int)DataGridCol.QTY].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = false;
//            //        //}
//            //        //else
//            //        {
//            //            (grdPrescription.Columns[(int)DataGridCol.QTY].GetCellContent(PrescriptionDetailPreparing) as AxTextBox).IsEnabled = true;
//            //        }
//            //    }
//            //}

//        }

//        public void acbDrug_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if (grdPrescription == null ) 
//            {
//                return;
//            }
//            if (SelectedPrescriptionDetail == null)
//            {
//                return;
//            }
//            if(SelectedPrescriptionDetail.IsDrugNotInCat)
//            {
//                SelectedPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
//                SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
//                ClearDataRow(SelectedPrescriptionDetail);
//            }
//            else
//            if (grdPrescription.SelectedItem != null && AutoGenMedProduct != null && AutoGenMedProduct.SelectedItem != null)
//            {
//                SelectedPrescriptionDetail.SelectedDrugForPrescription = ObjectCopier.DeepCopy(AutoGenMedProduct.SelectedItem as GetDrugForSellVisitor);
//                ClearDataRow(SelectedPrescriptionDetail);
//                //SelectedPrescriptionDetail = RefreshPrescriptObject(SelectedPrescriptionDetail); 
//            }
//        }

//        #region Form Cho Thuoc
//        AutoCompleteBox AutoThuoc;

//        public void AutoThuoc_Loaded(object sender, RoutedEventArgs e)
//        {
//            AutoThuoc = sender as AutoCompleteBox;
//        }
//        public void TenThuoc_GotFocus(object sender, RoutedEventArgs e)
//        {
//            if (tempPrescriptionDetail!=null
//                && tempPrescriptionDetail.SelectedDrugForPrescription == null)
//            {
//                tempPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
//                tempPrescriptionDetail.SelectedDrugForPrescription.DispenseVolume = 1;
//                tempPrescriptionDetail.SelectedDrugForPrescription.UnitVolume = 1;
//                tempPrescriptionDetail.SelectedDrugForPrescription.DayRpts =
//                    tempPrescriptionDetail.DayRpts;
//                ClearDataRow(tempPrescriptionDetail);
//            }
//        }
//        public void TenThuoc_LostFocus(object sender, RoutedEventArgs e)
//        {
//            if (tempPrescriptionDetail != null
//                && tempPrescriptionDetail.SelectedDrugForPrescription != null
//                && tempPrescriptionDetail.SelectedDrugForPrescription.BrandName != null
//                && !tempPrescriptionDetail.SelectedDrugForPrescription.BrandName.Contains(" *"))
//            {
//                tempPrescriptionDetail.BrandName = tempPrescriptionDetail.SelectedDrugForPrescription.BrandName + " *";
//            }
//        }
//        public void AutoThuoc_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if (tempPrescriptionDetail == null)
//                return;
//            if (tempPrescriptionDetail.IsDrugNotInCat)
//            {
//                tempPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
//                tempPrescriptionDetail.SelectedDrugForPrescription.BrandName = AutoThuoc.SearchText;
//                ClearDataRow(tempPrescriptionDetail);
//            }
//            else
//                if (AutoThuoc != null && AutoThuoc.SelectedItem != null
//                    && AutoThuoc.Text!="" && AutoThuoc.SearchText!="")
//                {
//                    tempPrescriptionDetail.SelectedDrugForPrescription = AutoThuoc.SelectedItem as GetDrugForSellVisitor;
//                    //ClearDataRow(tempPrescriptionDetail);
//                    tempPrescriptionDetail=ObjectCopier.DeepCopy( RefreshPrescriptObject(tempPrescriptionDetail)); 
//                }
//        }

//        public void AutoThuoc_Populating(object sender, PopulatingEventArgs e)
//        {
//            //IsEditor = true;
//            //if (BrandName != e.Parameter)
//            {
//                DrugList.PageIndex = 0;
//                BrandName = e.Parameter;
//                if (tempPrescriptionDetail != null)
//                {
//                    if (tempPrescriptionDetail.IsDrugNotInCat)
//                    {
//                        //return;
//                    }

//                    if (tempPrescriptionDetail.BeOfHIMedicineList)
//                    {
//                        IsInsurance = 1;
//                        SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
//                    }
//                    else
//                    {
//                        IsInsurance = 0;
//                        SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
//                    }
//                }
//            }
//        }

//        public void btAddNew() 
//        {
//            if (!CheckThuocHopLe(tempPrescriptionDetail)) 
//            {
//                MessageBox.Show(eHCMSResources.K0020_G1_ThuocChonKhongHopLe);
//                return;
//            }
//            RemoveLastRowPrecriptionDetail();
            
//            LatestePrecriptions.PrescriptionDetails.Add(ObjectCopier.DeepCopy(tempPrescriptionDetail));
//            AddNewBlankDrugIntoPrescriptObjectNew();
//            AutoThuoc.Focus();
//            btClear();
//        }



//        public void btClear()
//        {
//            tempPrescriptionDetail = ObjectCopier.DeepCopy( CreateNewPrescriptObject());
//            NotifyOfPropertyChange(() => tempPrescriptionDetail);
//        }
//        AutoCompleteBox DrugAdministrationForm { get; set; }
//        public void DrugAdministrationForm_Loaded(object sender, RoutedEventArgs e) 
//        {
//            DrugAdministrationForm = (AutoCompleteBox)sender;
//        }
//        #endregion
//        AutoCompleteBox AutoGenMedProduct;

//        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
//        {
//            AutoGenMedProduct = sender as AutoCompleteBox;
//        }


//        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
//        {
//            if (BrandName != e.Parameter)
//            {
//                DrugList.PageIndex = 0;
//                BrandName = e.Parameter;
//                if (SelectedPrescriptionDetail != null)
//                {
//                    if (SelectedPrescriptionDetail.IsDrugNotInCat)
//                    {
//                        return;
//                    }
//                    CheckBox check = grdPrescription.Columns[(int)DataGridCol.HI].GetCellContent(grdPrescription.SelectedItem) as CheckBox;
//                    AutoCompleteBox completebox = grdPrescription.Columns[(int)DataGridCol.DRUG_NAME].GetCellContent(grdPrescription.SelectedItem) as AutoCompleteBox;

//                    if (check.IsChecked.Value)
//                    {
//                        IsInsurance = 1;
//                        SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
//                    }
//                    else
//                    {
//                        IsInsurance = 0;
//                        SearchDrugForPrescription_Paging(StoreID, DrugList.PageIndex, DrugList.PageSize, true);
//                    }
//                }
//            }
//        }

//        public void acbDrug_LostFocus(object sender, RoutedEventArgs e)
//        {
            
//        }

//        #region autocomplete box for DVT, Cach Dung, Ghi Chu

//        public void DrugUnitUse_Loaded(object sender, PopulatingEventArgs e)
//        {
//            ((AxAutoComplete)sender).ItemsSource = DonViTinh;
//        }
//        public void DrugUnitUse_Populating(object sender, PopulatingEventArgs e)
//        {
//            if (SelectedPrescriptionDetail != null)
//            {
//                SelectedPrescriptionDetail.UnitUse = e.Parameter;
//            }
//        }
//        public void DrugUnitUse_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
//            {
//                if (((AxAutoComplete)sender).SelectedItem != null)
//                {
//                    SelectedPrescriptionDetail.UnitUse = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
//                }
//                else
//                {
//                    SelectedPrescriptionDetail.UnitUse = ((AxAutoComplete)sender).Text;
//                }
//            }
//        }

//        public void DrugInstructionNotes_Loaded(object sender, PopulatingEventArgs e)
//        {
//            ((AxAutoComplete)sender).ItemsSource = GhiChu;
//        }
//        public void DrugInstructionNotes_Populating(object sender, PopulatingEventArgs e)
//        {
//            //if (SelectedPrescriptionDetail != null)
//            //{
//            //    SelectedPrescriptionDetail.DrugInstructionNotes = e.Parameter;
//            //}
//        }
//        public void DrugInstructionNotes_LostFocus(object sender, RoutedEventArgs e)
//        {
//            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
//            //{
//            //    if (((AxAutoComplete)sender).SelectedItem != null)
//            //    {
//            //        SelectedPrescriptionDetail.DrugInstructionNotes = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
//            //    }
//            //    else
//            //    {
//            //        SelectedPrescriptionDetail.DrugInstructionNotes = ((AxAutoComplete)sender).Text;
//            //    }
//            //}
//        }
//        public void DrugInstructionNotes_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e) 
//        {
//            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
//            //{
//            //    if (((AxAutoComplete)sender).SelectedItem != null)
//            //    {
//            //        PrescriptionDetailPreparing.DrugInstructionNotes = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
//            //    }
//            //    else
//            //    {
//            //        PrescriptionDetailPreparing.DrugInstructionNotes = ((AxAutoComplete)sender).Text;
//            //    }
//            //}
//        }

//        public void DrugInstructionNotesForm_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            //if (((AxAutoComplete)sender) != null)
//            //{
//            //    if (((AxAutoComplete)sender).SelectedItem != null)
//            //    {
//            //        tempPrescriptionDetail.DrugInstructionNotes = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
//            //    }
//            //    else
//            //    {
//            //        tempPrescriptionDetail.DrugInstructionNotes = ((AxAutoComplete)sender).Text;
//            //    }
//            //}
//        }

//        public void DrugAdministration_Loaded(object sender, PopulatingEventArgs e)
//        {
//            ((AxAutoComplete)sender).ItemsSource = CachDung;
//        }
//        public void DrugAdministration_Populating(object sender, PopulatingEventArgs e)
//        {
//            //if (SelectedPrescriptionDetail != null)
//            //{
//            //    SelectedPrescriptionDetail.Administration = e.Parameter;
//            //}
//        }
//        public void DrugAdministration_LostFocus(object sender, RoutedEventArgs e)
//        {
//            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
//            //{
//            //    if (((AxAutoComplete)sender).SelectedItem != null)
//            //    {
//            //        SelectedPrescriptionDetail.Administration = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
//            //    }
//            //    else
//            //    {
//            //        SelectedPrescriptionDetail.Administration = ((AxAutoComplete)sender).Text;
//            //    }
//            //}
//        }

//        public void DrugAdministration_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            //if (grdPrescription != null && grdPrescription.SelectedItem != null && ((AxAutoComplete)sender) != null)
//            //{
//            //    SelectedPrescriptionDetail.Administration = SelectedPrescriptionDetail.SelectedDrugForPrescription.dru;                
//            //}
//        }

//        public void DrugAdministrationForm_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
//        {
//            if ((AxAutoComplete)sender != null)
//            {
//                if (((AxAutoComplete)sender).SelectedItem != null)
//                {
//                    tempPrescriptionDetail.Administration = ((PrescriptionNoteTemplates)((AxAutoComplete)sender).SelectedItem).NoteDetails;
//                }
//                else
//                {
//                    tempPrescriptionDetail.Administration = ((AxAutoComplete)sender).Text;
//                }
//            }
//        }
//        #endregion

//        private void ClearDataRow(PrescriptionDetail ObjRow)
//        {
//            if (ObjRow != null)
//            {
//                if (ObjRow.DrugID > 0)
//                {
//                    ObjRow.dosageStr = "0";
//                    ObjRow.dosage = 0;
//                    ObjRow.MDoseStr = "0";
//                    ObjRow.MDose = 0;
//                    ObjRow.ADoseStr = "0";
//                    ObjRow.ADose = 0;
//                    ObjRow.EDoseStr = "0";
//                    ObjRow.EDose = 0;
//                    ObjRow.NDoseStr = "0";
//                    ObjRow.NDose = 0;
//                    //ObjRow.DayRpts = 0;//cai này vô là sai vi cứ reset ND=0 miết
//                    //ObjRow.DayExtended =0;//cai này vô là sai vi cứ reset ND=0 miết
//                    ObjRow.Qty = 0;
//                    ObjRow.ChooseDose = new ChooseDose();
//                    ObjRow.DrugInstructionNotes = "";
//                    ObjRow.Administration = "";
//                    ObjRow.ObjPrescriptionDetailSchedules = new ObservableCollection<PrescriptionDetailSchedules>();
//                }
//            }
//        }

//        #region autocomplete doctor
//        private void SearchStaffCatgs(string SearchKeys)
//        {
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new CommonUtilsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginSearchStaffFullName(SearchKeys, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
//                    {

//                        try
//                        {
//                            var results = contract.EndSearchStaffFullName(asyncResult);
//                            if (results != null)
//                            {
//                                StaffCatgs = new ObservableCollection<Staff>();
//                                foreach (Staff p in results)
//                                {
//                                    StaffCatgs.Add(p);
//                                }
//                                NotifyOfPropertyChange(() => StaffCatgs);
//                            }
//                            aucHoldConsultDoctor.ItemsSource = StaffCatgs;
//                            aucHoldConsultDoctor.PopulateComplete();
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            Globals.IsBusy = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

        
//        AutoCompleteBox aucHoldConsultDoctor;

//        public void aucHoldConsultDoctor_Populating(object sender, PopulatingEventArgs e)
//        {
//            aucHoldConsultDoctor = sender as AutoCompleteBox;
//            this.SearchStaffCatgs(e.Parameter);
//        }

//        public void chkNeedToHold_Check(object sender, RoutedEventArgs e)
//        {
//            IsEnabledAutoComplete = true;
//        }

//        public void chkNeedToHold_UnCheck(object sender, RoutedEventArgs e)
//        {
//            IsEnabledAutoComplete = false;
//            LatestePrecriptions.ConsultantID = null;
//            if (LatestePrecriptions.ConsultantDoctor != null)
//            {
//                LatestePrecriptions.ConsultantDoctor.FullName = "";
//            }
//        }

//        public void aucHoldConsultDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            aucHoldConsultDoctor = sender as AutoCompleteBox;

//            if (aucHoldConsultDoctor.SelectedItem != null)
//            {
//                LatestePrecriptions.ConsultantID = (aucHoldConsultDoctor.SelectedItem as Staff).StaffID;
//                if (LatestePrecriptions.ConsultantDoctor != null)
//                {
//                    LatestePrecriptions.ConsultantDoctor.FullName = (aucHoldConsultDoctor.SelectedItem as Staff).FullName;
//                }
//            }
//            else
//            {
//                if (LatestePrecriptions != null)
//                {
//                    if (LatestePrecriptions.ConsultantDoctor != null)
//                    {
//                        LatestePrecriptions.ConsultantDoctor.FullName = "";
//                    }
//                }
//            }
//        }
//        #endregion

//        public void lnkDelete_Click(object sender, RoutedEventArgs e)
//        {
//            PrescriptionDetail p = grdPrescription.SelectedItem as PrescriptionDetail;

//            if (p != null && p.DrugID > 0)
//            {
//                if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                {
//                    LatestePrecriptions.PrescriptionDetails.Remove(p);
//                }
//            }
//            else
//            {
//                if (p.IsDrugNotInCat)
//                {
//                    if (MessageBox.Show(eHCMSResources.Z0554_G1_CoChacMuonXoaDongNayKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                    {
//                        LatestePrecriptions.PrescriptionDetails.Remove(p);
//                    }
//                }
//            }
//        }

//        private void GetPrescriptionTypes()
//        {
//            //▼====== #001
//            PrescriptionTypeList = new ObservableCollection<Lookup>();
//            foreach (var tmpLookup in Globals.AllLookupValueList)
//            {
//                if (tmpLookup.ObjectTypeID == (long)(LookupValues.PRESCRIPTION_TYPE))
//                {
//                    PrescriptionTypeList.Add(tmpLookup);
//                }
//            }
//            if (PrescriptionTypeList.Count > 0)
//            {
//                CurrentPrescriptionType = PrescriptionTypeList[0];

//                InitialNewPrescription();

//                SetToaBaoHiem_KhongBaoHiem();

//                //Đọc Toa Thuốc Cuối lên nếu có
//                GetLatestPrescriptionByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);

//            }
//            //▲====== #001
//            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            //IsWaitingGetPrescriptionTypes = true;

//            //var t = new Thread(() =>
//            //{
//            //    using (var serviceFactory = new ePrescriptionsServiceClient())
//            //    {
//            //        var contract = serviceFactory.ServiceInstance;
//            //        contract.BeginGetLookupPrescriptionType(Globals.DispatchCallback((asyncResult) =>
//            //        {

//            //            try
//            //            {
//            //                var results = contract.EndGetLookupPrescriptionType(asyncResult);
//            //                if (results != null)
//            //                {
//            //                    PrescriptionTypeList.Clear();

//            //                    PrescriptionTypeList = new ObservableCollection<Lookup>(results);

//            //                    if (PrescriptionTypeList.Count > 0)
//            //                    {
//            //                        CurrentPrescriptionType = PrescriptionTypeList[0];

//            //                        InitialNewPrescription();

//            //                        SetToaBaoHiem_KhongBaoHiem();

//            //                        //Đọc Toa Thuốc Cuối lên nếu có
//            //                        GetLatestPrescriptionByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);

//            //                    }
//            //                }
//            //            }
//            //            catch (Exception ex)
//            //            {
//            //                MessageBox.Show(ex.Message);
//            //            }
//            //            finally
//            //            {
//            //                IsWaitingGetPrescriptionTypes = false;
//            //            }

//            //        }), null);

//            //    }

//            //});

//            //t.Start();
//        }

//        private void GetMedConditionByPtID(long patientID, int mcTypeID)
//        {
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "" });

//            IsWaitingGetMedConditionByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ComRecordsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetMedConditionByPtID(patientID, mcTypeID, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {

//                            var items = contract.EndGetMedConditionByPtID(asyncResult);
//                            if (items != null)
//                            {
//                                PtMedCond = new ObservableCollection<MedicalConditionRecord>(items);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsWaitingGetMedConditionByPtID = false;
//                        }
//                    }), null);
//                }


//            });
//            t.Start();
//        }

//        public void grdPrescription_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
//        {
//            if (grdPrescription != null)
//            {
//                if (grdPrescription.SelectedItem != null)
//                {
//                    grdPrescription.BeginEdit();
//                }
//            }
//        }


//        private bool _chkHasAppointmentValue;
//        public bool chkHasAppointmentValue
//        {
//            get
//            {
//                return _chkHasAppointmentValue;
//            }
//            set
//            {
//                if (_chkHasAppointmentValue != value)
//                {
//                    _chkHasAppointmentValue = value;

//                    NotifyOfPropertyChange(() => chkHasAppointmentValue);
//                    //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);
//                }
//            }
//        }

//        private bool CheckValidationEditor1Row(PrescriptionDetail item)
//        {
//            if (item != null 
//                && item.SelectedDrugForPrescription!=null
//                && item.SelectedDrugForPrescription.BrandName!=""
//                )
//            {
//                return true;
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return false;
//            }
//        }

//        private bool ischanged(object item)
//        {
//            PrescriptionDetail p = item as PrescriptionDetail;
//            if (p != null)
//            {
//                if ((p.DrugID != null && p.DrugID != 0)
//                    ||(p.IsDrugNotInCat// && p.SelectedDrugForPrescription!=null
//                    && p.BrandName != ""))
//                {
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                return false;
//            }
//        }

//        private bool IsPatientInsurance()
//        {
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.HisID != null)
//                    return true;
//                return false;
//            }
//            return false;

//        }

//        private void AddNewBlankDrugIntoPrescriptObjectNew()
//        {
//            NotifyOfPropertyChange(() => tempPrescriptionDetail);
//            if (LatestePrecriptions == null)
//                LatestePrecriptions = new Prescription();

//            if (LatestePrecriptions.PrescriptionDetails == null)
//                LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();

//            int i = LatestePrecriptions.PrescriptionDetails.Count + 1;
//            PrescriptionDetail prescriptDObj = ObjectCopier.DeepCopy( CreateNewPrescriptObject());
            
//            LatestePrecriptions.PrescriptionDetails.Add(ObjectCopier.DeepCopy( prescriptDObj));            
//            NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//            tempPrescriptionDetail = ObjectCopier.DeepCopy(CreateNewPrescriptObject());
//        }

//        private PrescriptionDetail CreateNewPrescriptObject()
//        {
//            PrescriptionDetail prescriptDObj = new PrescriptionDetail();
//            prescriptDObj.SelectedDrugForPrescription = new GetDrugForSellVisitor();
//            prescriptDObj.DrugID = 0;
//            if (IsPatientInsurance())
//            {
//                prescriptDObj.BeOfHIMedicineList = true;
//                prescriptDObj.InsuranceCover = true;
//            }
//            else
//            {
//                prescriptDObj.BeOfHIMedicineList = false;
//                prescriptDObj.InsuranceCover = false;
//            }
//            prescriptDObj = ObjectCopier.DeepCopy(CreateNewDrugForSellVisitor(prescriptDObj));
//            prescriptDObj.Index = LatestePrecriptions.PrescriptionDetails.Count;
            
//            return prescriptDObj;
//        }
//        private PrescriptionDetail CreateNewDrugForSellVisitor(PrescriptionDetail prescriptDObj)
//        {
//            prescriptDObj.SelectedDrugForPrescription = new GetDrugForSellVisitor();
//            prescriptDObj.DrugID = 0;
//            prescriptDObj = ObjectCopier.DeepCopy(RefreshPrescriptObject(prescriptDObj));
            
//            return prescriptDObj;
//        }
//        //refresh lai Thuoc giu nguyen trang thai thuoc ngoai danh muc hay ko

//        private PrescriptionDetail RefreshPrescriptObject(PrescriptionDetail prescriptDObj)
//        {
//            prescriptDObj.IsInsurance = null;
//            prescriptDObj.Strength = "";
//            prescriptDObj.Qty = 0;
//            prescriptDObj.MDoseStr = "0";
//            prescriptDObj.ADoseStr = "0";
//            prescriptDObj.EDoseStr = "0";
//            prescriptDObj.NDoseStr = "0";
//            prescriptDObj.DrugType = new Lookup{LookupID = (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG
//                                                ,ObjectValue="T"};
//            prescriptDObj.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG;

//            prescriptDObj =ObjectCopier.DeepCopy( RefreshDayRptsPrescriptObject(prescriptDObj));

//            prescriptDObj.DrugInstructionNotes = "";
//            if (LatestePrecriptions.PrescriptionDetails != null
//                && LatestePrecriptions.PrescriptionDetails.Count > 0)
//            {
//                prescriptDObj.PrescriptDetailID = LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].PrescriptDetailID + 1;                
//            }
//            else
//            {
//                prescriptDObj.PrescriptDetailID = 0;
//            }

//            //prescriptDObj.IsDrugNotInCat = false;
//            prescriptDObj.DrugInstructionNotes = "";
//            prescriptDObj.Administration = "";
//            return prescriptDObj;
//        }

//        private PrescriptionDetail RefreshDayRptsPrescriptObject(PrescriptionDetail prescriptDObj)
//        {
//            if (LatestePrecriptions.NDay > 0)
//            {
//                if (IsBenhNhanNoiTru() && IsPatientInsurance())
//                {
//                    if (prescriptDObj.InsuranceCover && LatestePrecriptions.NDay > xNgayBHToiDa_NoiTru)
//                    {
//                        prescriptDObj.DayRpts = xNgayBHToiDa_NoiTru;
//                        prescriptDObj.DayExtended = LatestePrecriptions.NDay.GetValueOrDefault() - xNgayBHToiDa_NoiTru;
//                    }
//                    else
//                    {
//                        prescriptDObj.DayRpts = LatestePrecriptions.NDay.GetValueOrDefault();
//                        prescriptDObj.DayExtended = 0;
//                    }
//                }
//                else if (!IsBenhNhanNoiTru() && IsPatientInsurance())
//                {
//                    if (prescriptDObj.InsuranceCover && LatestePrecriptions.NDay > xNgayBHToiDa_NgoaiTru)
//                    {
//                        prescriptDObj.DayRpts = xNgayBHToiDa_NgoaiTru;
//                        prescriptDObj.DayExtended = LatestePrecriptions.NDay.GetValueOrDefault() - xNgayBHToiDa_NgoaiTru;
//                    }
//                    else
//                    {
//                        prescriptDObj.DayRpts = LatestePrecriptions.NDay.GetValueOrDefault();
//                        prescriptDObj.DayExtended = 0;
//                    }
//                }
//                else
//                {
//                    prescriptDObj.DayRpts = LatestePrecriptions.NDay.GetValueOrDefault();
//                    prescriptDObj.DayExtended = 0;
//                }
//            }
//            return prescriptDObj;
//        }

//        public void ClearForm()
//        {
//            AutoThuoc.Text = "";
//            DrugAdministrationForm.Text = "";
            
//        }

//        public void grdPrescription_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
//        {
//            PrescriptionDetailPreparing = (e.Row.DataContext as PrescriptionDetail);
//            if (LatestePrecriptions.PrescriptionDetails == null)
//                return;
//            if (PrescriptionDetailPreparing.SelectedDrugForPrescription != null
//                            && PrescriptionDetailPreparing.SelectedDrugForPrescription.MaxDayPrescribed != null
//                            && PrescriptionDetailPreparing.SelectedDrugForPrescription.MaxDayPrescribed > 0)
//            {
//                e.Row.Background = new SolidColorBrush(Color.FromArgb(200, 224, 130, 228));
//                PrescriptionDetailPreparing.BackGroundColor = "#E79DEA";
//                SelectedPrescriptionDetail.BackGroundColor = "#E79DEA";
//                NotifyOfPropertyChange(() => SelectedPrescriptionDetail.BackGroundColor);
//                NotifyOfPropertyChange(() => PrescriptionDetailPreparing.BackGroundColor);
//            }
//            if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
//            {
//                if (PrescriptionDetailPreparing.IsDrugNotInCat
//                    && AutoGenMedProduct.SearchText != "")
//                {
//                    PrescriptionDetailPreparing.SelectedDrugForPrescription.BrandName = AutoGenMedProduct.SearchText;
//                    ClearDataRow(PrescriptionDetailPreparing);                    
//                }
//            }
//            if (ischanged(grdPrescription.SelectedItem))
//            {
//                if (e.Row.GetIndex() == (LatestePrecriptions.PrescriptionDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit)
//                {
//                    //if (((PrescriptionDetail)this.grdPrescription.SelectedItem).V_DrugType == 
//                    //    (long)AllLookupValues.V_DrugType.THUOC_NGOAIDANH_MUC)
//                    //{
//                    //    ((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat = true;
//                    //}
//                    AddNewBlankDrugIntoPrescriptObjectNew();
//                }
//                if (e.Column.DisplayIndex == (int)DataGridCol.DRUG_NAME)
//                {
//                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat == true)
//                    {
//                        if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName))
//                        {
//                            MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, ((PrescriptionDetail)this.grdPrescription.SelectedItem).BrandName.Trim()));
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        if (Check1ThuocBiDiUng(((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName))
//                        {
//                            MessageBox.Show(string.Format(eHCMSResources.Z0990_G1_Thuoc0DiUngBN, ((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.BrandName.Trim()));
//                            return;
//                        }
//                    }


//                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID != null && (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID > 0)
//                    {
//                        Globals.CheckContrain(PtMedCond, (long)((PrescriptionDetail)this.grdPrescription.SelectedItem).DrugID);
//                    }
//                }
//                else if (e.Column.DisplayIndex == (int)DataGridCol.QTY)
//                {
//                    if (((PrescriptionDetail)this.grdPrescription.SelectedItem).IsDrugNotInCat == false)
//                    {
//                        if (((PrescriptionDetail)this.grdPrescription.SelectedItem).SelectedDrugForPrescription.Remaining < ((PrescriptionDetail)this.grdPrescription.SelectedItem).Qty)
//                        {
//                            MessageBox.Show(eHCMSResources.A0977_G1_Msg_InfoSLgKhDuBan);
//                        }
//                    }
//                }

//                else if (e.Column.DisplayIndex == (int)DataGridCol.MDOSE)
//                {
//                    ChangeMDose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.MDoseStr : "0", this.grdPrescription.SelectedItem);
//                }

//                else if (e.Column.DisplayIndex == (int)DataGridCol.EDOSE)
//                {
//                    ChangeEDose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.EDoseStr : "0", this.grdPrescription.SelectedItem);
//                }
//                else if (e.Column.DisplayIndex == (int)DataGridCol.ADOSE)
//                {
//                    ChangeADose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.ADoseStr : "0", this.grdPrescription.SelectedItem);
//                }

//                else if (e.Column.DisplayIndex == (int)DataGridCol.NDOSE)
//                {
//                    ChangeNDose(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.NDoseStr : "0", this.grdPrescription.SelectedItem);
//                }

//                //else if (e.Column.DisplayIndex == (int)DataGridCol.DOSAGE)
//                //{
//                //    ChangeDosage(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.dosageStr : "0", this.grdPrescription.SelectedItem);
//                //}

//                else if (e.Column.DisplayIndex == (int)DataGridCol.Dayts)
//                {
//                    ChangeNgayDung(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.DayRpts : 0, this.grdPrescription.SelectedItem);
//                }
//                else if (e.Column.DisplayIndex == (int)DataGridCol.DaytExtended)
//                {
//                    ChangeNgayDungExtend(PrescriptionDetailPreparing != null ? PrescriptionDetailPreparing.DayExtended : 0, this.grdPrescription.SelectedItem);
//                }
//                else if (e.Column.DisplayIndex == (int)DataGridCol.USAGE)
//                {
//                    if (PrescriptionDetailPreparing.SelectedDrugForPrescription!=null
//                        && PrescriptionDetailPreparing.SelectedDrugForPrescription.Administration!=null)
//                    {
//                        PrescriptionDetailPreparing.Administration = PrescriptionDetailPreparing.SelectedDrugForPrescription.Administration;
//                    }
//                }
//                else if (e.Column.DisplayIndex == (int)DataGridCol.NotInCat)
//                {

//                }
//            }
//        }

//        private bool checkDrugType(PrescriptionDetail PrescriptDetail)
//        {
//            switch (PrescriptDetail.DrugType.LookupID)
//            {
//                case (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN:
//                    MessageBox.Show(eHCMSResources.Z0967_G1_ThuocUongTheoLichTuan);
//                    return false;
//            }
//            return true;
//        }

//        private void BackupCurPrescriptionItem()
//        {
//            RetoreLatestePrecriptions = LatestePrecriptions.DeepCopy();
//            PrecriptionsBeforeUpdate = LatestePrecriptions.DeepCopy();
//        }

//        private void RestoreCurPrescriptionItem()
//        {
//            LatestePrecriptions = RetoreLatestePrecriptions;
//            if (LatestePrecriptions != null)
//            {
//                SoNgayDungThuoc_Root = LatestePrecriptions.NDay;
//            }
//        }

//        private void RestoreToaChonDeSua()
//        {
//            if (DuocSi_IsEditingToaThuoc)
//            {
//                LatestePrecriptions = ToaChonDeSua;
//                SoNgayDungThuoc_Root = LatestePrecriptions.NDay;
//                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
//            }
//        }

//        #region button member

//        public void btnSavePhatHanhLai()
//        {
//            if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
//            {
//                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0815_G1_ChiDinhCDoan), eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }


//            if (LatestePrecriptions.V_PrescriptionType == null)
//            {
//                MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }


//            if (grdPrescription == null)
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckAllThuocBiDiUng())
//            {
//                return;
//            }

//            if (ErrCheckChongChiDinh())
//            {
//                return;
//            }

//            if (grdPrescription.IsValid == false)
//            {
//                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckThuocHopLe() == false)
//            {
//                return;
//            }

//            if (CheckHoiChan() == false)
//            {
//                return;
//            }

//            if (CheckThuocBiTrungTrongToa())
//            {
//                return;
//            }

//            //if (CheckHasCheckHasAppointment() == false)
//            //{
//            //    return;
//            //}

//            //Bỏ dòng cuối cùng rỗng của Toa
//            RemoveLastRowPrecriptionDetail();

//            CapNhatToaThuoc = true;

//            if (ConfirmSoLuongNotEnoughBeforeSave())
//            {
//                SetValueTaoThanhToaMoi();
//                //Prescriptions_Update();
//                //Dinh sua o day
//                Coroutine.BeginExecute(CoroutinePrescriptions_Update());
//            }
//            else
//            {
//                //Cộng lại dòng trống
//                AddNewBlankDrugIntoPrescriptObjectNew();
//                //Cộng lại dòng trống
//            }
//        }

//        public void btnUpdate()
//        {
//            if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
//            {
//                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0815_G1_ChiDinhCDoan), eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }


//            if (LatestePrecriptions.V_PrescriptionType == null)
//            {
//                MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }


//            if (grdPrescription == null)
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckAllThuocBiDiUng())
//            {
//                return;
//            }

//            if (ErrCheckChongChiDinh())
//            {
//                return;
//            }

//            if (grdPrescription.IsValid == false)
//            {
//                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckThuocHopLe() == false)
//            {
//                return;
//            }

//            if (CheckHoiChan() == false)
//            {
//                return;
//            }

//            if (CheckThuocBiTrungTrongToa())
//            {
//                return;
//            }

//            //if (CheckHasCheckHasAppointment() == false)
//            //{
//            //    return;
//            //}

//            //Bỏ dòng cuối cùng rỗng của Toa
//            RemoveLastRowPrecriptionDetail();

//            CapNhatToaThuoc = true;

//            if (ConfirmSoLuongNotEnoughBeforeSave())
//            {
//                SetValueTaoThanhToaMoi();
//                //Prescriptions_Update();
//                //Dinh sua o day
//                Coroutine.BeginExecute(CoroutinePrescriptions_Update());
//            }
//            else
//            {
//                //Cộng lại dòng trống
//                AddNewBlankDrugIntoPrescriptObjectNew();
//                //Cộng lại dòng trống
//            }
//        }

//        private void RemoveLastRowPrecriptionDetail()
//        {
//            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
//            {
//                var listtmp = LatestePrecriptions.PrescriptionDetails.DeepCopy();

//                var listPCLAdd = (from c in listtmp
//                                  where (c.IsDrugNotInCat) || (c.SelectedDrugForPrescription != null && c.SelectedDrugForPrescription.DrugID > 0)
//                                  select c);

//                LatestePrecriptions.PrescriptionDetails.Clear();

//                foreach (var item in listPCLAdd)
//                {
//                    LatestePrecriptions.PrescriptionDetails.Add(ObjectCopier.DeepCopy( item));
//                }
//            }            
//        }

//        private void CheckpreApptCheck()
//        {
//            //tam thoi de o day
//            //xem benh nhan co hen benh chua
//            if (LatestePrecriptions.HasAppointment)
//            {
//                return;
//            }
//            //neu chua co hen benh thi bat popup chon hen benh len
//            GlobalsNAV.ShowDialog<IPrescriptionApptCheck>();
//        }

//        private bool CheckHoiChan()
//        {
//            if (IsEnabledAutoComplete)
//            {
//                if (LatestePrecriptions.ConsultantID == null)
//                {
//                    MessageBox.Show(eHCMSResources.A0341_G1_Msg_InfoChonNguoiHoiChan);
//                    return false;
//                }
//            }
//            return true;
//        }

//        public void btnUndo(object sender, RoutedEventArgs e)
//        {
//            PrescriptionNoteTemplates tmp = new PrescriptionNoteTemplates();
//            tmp.PrescriptNoteTemplateID = -1;
//            tmp.NoteDetails = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K0616_G1_ChonMau);
//            ObjPrescriptionNoteTemplates_Selected = tmp;


//            IsEnabled = false;

//            btnUndoIsEnabled = false;

//            btChonChanDoanIsEnabled = false;

//            if (DuocSi_IsEditingToaThuoc)
//            {
//                RestoreToaChonDeSua();

//                btnEditIsEnabled = true;
//                btDuocSiEditIsEnabled = false;
//                IsEnabledPrint = true;
//            }
//            else
//            {
//                btnCreateNewIsEnabled = true;
//                btnSaveAddNewIsEnabled = false;

//                btnUpdateIsEnabled = false;
//                //bntSaveAsIsEnabled = false;

//                RestoreCurPrescriptionItem();
//                if (LatestePrecriptions != null && LatestePrecriptions.PrescriptID > 0)//Sửa
//                {
//                    btnCopyToIsEnabled = true;
//                    if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc) == false)
//                    {
//                        btnEditIsEnabled = false;
//                    }
//                    else
//                    {
//                        btnEditIsEnabled = true;
//                    }
//                    btnCreateAndCopyIsEnabled = true;
//                    IsEnabledPrint = true;
//                }
//                else
//                {
//                    LatestePrecriptions.DoctorAdvice = "";
//                    btnCopyToIsEnabled = false;
//                    IsEnabledPrint = false;
//                    btnEditIsEnabled = false;
//                    btnCreateAndCopyIsEnabled = false;
//                }
//            }

//        }

//        bool TaoThanhToaMoi = false;
//        bool CapNhatToaThuoc = false;


//        //public void bntSaveAs(object sender, RoutedEventArgs e)
//        //{
//        //    if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
//        //    {
//        //        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0815_G1_ChiDinhCDoan), eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//        //        return;
//        //    }

//        //    if (LatestePrecriptions.V_PrescriptionType == null)
//        //    {
//        //        MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//        //        return;
//        //    }

//        //    if (grdPrescription == null)
//        //    {
//        //        MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//        //        return;
//        //    }

//        //    if (CheckAllThuocBiDiUng())
//        //    {
//        //        return;
//        //    }

//        //    if (ErrCheckChongChiDinh())
//        //    {
//        //        return;
//        //    }

//        //    if (grdPrescription.IsValid == false)
//        //    {
//        //        MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//        //        return;
//        //    }

//        //    if (CheckThuocHopLe() == false)
//        //    {
//        //        return;
//        //    }

//        //    if (CheckHoiChan() == false)
//        //    {
//        //        return;
//        //    }


//        //    if (CheckThuocBiTrungTrongToa())
//        //    {
//        //        return;
//        //    }

//        //    //if (CheckHasCheckHasAppointment() == false)
//        //    //{
//        //    //    return;
//        //    //}

//        //    //Bỏ dòng cuối cùng rỗng của Toa
//        //    RemoveLastRowPrecriptionDetail();

//        //    TaoThanhToaMoi = true;

//        //    if (ConfirmSoLuongNotEnoughBeforeSave())
//        //    {
//        //        SetValueTaoThanhToaMoi();
//        //        Prescriptions_Add();
//        //    }
//        //    else
//        //    {
//        //        //Cộng lại dòng trống
//        //        AddNewBlankDrugIntoPrescriptObjectNew();
//        //        //Cộng lại dòng trống
//        //    }
//        //}

//        private void AddPrescriptIssueHistory()
//        {
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            IsWaitingAddPrescriptIssueHistory = true;

//            IsEnabledForm = false;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    //contract.BeginAddPrescriptIssueHistory(Globals.NumberTypePrescriptions_Rule, PhatHanhLai, Globals.DispatchCallback((asyncResult) =>
//                    // Txd 25/05/2014 Replaced ConfigList
//                    contract.BeginAddPrescriptIssueHistory((short)Globals.ServerConfigSection.CommonItems.NumberTypePrescriptions_Rule, PhatHanhLai, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            string OutError = "";

//                            if (contract.EndAddPrescriptIssueHistory(out OutError, asyncResult))
//                            {
//                                //phát sự kiện load lại danh sách toa thuốc
//                                Globals.EventAggregator.Publish(new ReloadDataePrescriptionEvent { });
//                                //Dinh sua
//                                Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });
//                                //phát sự kiện load lại danh sách toa thuốc

//                                //đọc lại toa thuốc cuối
//                                //GetLatestPrescriptionByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);
//                                //đọc lại toa thuốc cuối

//                                MessageBox.Show(eHCMSResources.A0472_G1_Msg_InfoDaPHanhToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            else
//                            {
//                                if (OutError.Contains("Duplex-Prescriptions_PrescriptionsInDay"))
//                                {
//                                    MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
//                                        + Environment.NewLine + OutError.Replace("Duplex-Prescriptions_PrescriptionsInDay@", "")
//                                        + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                }
//                                else
//                                {
//                                    MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsEnabledForm = true;

//                            IsWaitingAddPrescriptIssueHistory = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private Prescription _PhatHanhLai;
//        public Prescription PhatHanhLai
//        {
//            get
//            {
//                return _PhatHanhLai;
//            }
//            set
//            {
//                if (_PhatHanhLai != value)
//                {
//                    _PhatHanhLai = value;
//                    NotifyOfPropertyChange(() => PhatHanhLai);
//                }
//            }
//        }

//        private void SetValueForPhatHanhLaiToaThuoc()
//        {
//            if (PhatHanhLai == null)
//                PhatHanhLai = new Prescription();

//            PhatHanhLai = LatestePrecriptions.DeepCopy();
//            //PhatHanhLai.NDay = CheckAppDate();


//            if (PhatHanhLai.PrescriptionIssueHistory == null)
//                PhatHanhLai.PrescriptionIssueHistory = new PrescriptionIssueHistory();

//            if (Globals.ObjGetDiagnosisTreatmentByPtID == null)
//            {
//                MessageBox.Show(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK);
//                return;
//            }

//            PhatHanhLai.ServiceRecID = Globals.ObjGetDiagnosisTreatmentByPtID.ServiceRecID;

//            //Cụ thể  DV nào
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        PhatHanhLai.PrescriptionIssueHistory.PtRegDetailID = Globals.ObjGetDiagnosisTreatmentByPtID.PtRegDetailID;
//                    }
//                    else
//                    {
//                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                        return;
//                    }
//                }
//                else/*Khám VIP, Khám Cho Nội Trú*/
//                {
//                    PhatHanhLai.PrescriptionIssueHistory.PtRegDetailID = 0;
//                }
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                return;
//            }
//            //Cụ thể  DV nào

//            PhatHanhLai.PrescriptionIssueHistory.IssueIDOld = LatestePrecriptions.IssueID;
//            PhatHanhLai.ObjReIssuerStaffID = Globals.LoggedUserAccount.Staff;
//            PhatHanhLai.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;
//            PhatHanhLai.ObjDoctorStaffID = Globals.LoggedUserAccount.Staff;
//            PhatHanhLai.NDay = LatestePrecriptions.NDay;
//            PhatHanhLai.PrescriptionIssueHistory.StoreServiceSeqNumType = LatestePrecriptions.PrescriptionIssueHistory.StoreServiceSeqNumType;
//            PhatHanhLai.PrescriptionIssueHistory.StoreServiceSeqNum = LatestePrecriptions.PrescriptionIssueHistory.StoreServiceSeqNum;
//        }

//        private bool _btnCopyToIsEnabled;
//        public bool btnCopyToIsEnabled
//        {
//            get { return _btnCopyToIsEnabled; }
//            set
//            {
//                if (_btnCopyToIsEnabled != value)
//                {
//                    _btnCopyToIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnCopyToIsEnabled);
//                }
//            }

//        }

//        private bool IsPhatHanhLai = false;
//        public void btnCopyTo(object sender, RoutedEventArgs e)
//        {
//            IsPhatHanhLai = true;
//            if (LatestePrecriptions.CanEdit)
//            {
//                if (CheckIsVuotNgayQuiDinhHI())
//                {
//                    if (MessageBox.Show(eHCMSResources.A0838_G1_Msg_ConfTuDChinhNgDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                    {
//                        //Tự động điều chỉnh
//                        AutoAdjust();

//                        //Trạng thái đợi sửa
//                        TrangThaiWaitingCreateAndCopy(IsPhatHanhLai);
//                    }
//                    else
//                    {
//                        //Trạng thái đợi sửa 
//                        TrangThaiWaitingCreateAndCopy(IsPhatHanhLai);
//                    }
//                }
//                else
//                {
//                    //Trạng thái đợi sửa 
//                    TrangThaiWaitingCreateAndCopy(IsPhatHanhLai);
//                }
//            }
//            else
//            {
//                switch (LatestePrecriptions.ReasonCanEdit)
//                {
//                    case "PhaiThucHien-TraPhieuTruoc":
//                        {
//                            MessageBox.Show(eHCMSResources.Z0999_G1_ToaDaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            break;
//                        }
//                    case "ToaNay-DaTungPhatHanh-VaSuDung":
//                        {
//                            MessageBox.Show(eHCMSResources.K0199_G1_KhongTheCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            break;
//                        }
//                }

//            }
//            //if (MessageBox.Show("Bạn Muốn Phát Hành Lại Toa Thuốc Này Không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//            //{
//            //    SetValueForPhatHanhLaiToaThuoc();
//            //    AddPrescriptIssueHistory();
//            //}
//        }


//        //private bool _btnEditIsEnabledSave;
//        //public bool btnEditIsEnabledSave
//        //{
//        //    get { return (IsEnabledSave); }
//        //    set
//        //    {
//        //        if(_btnEditIsEnabledSave!=value)
//        //        {
//        //            _btnEditIsEnabledSave = value;
//        //            NotifyOfPropertyChange(() => btnEditIsEnabledSave);
//        //        }
//        //    }

//        //}

//        private bool _btDuocSiEditIsEnabled;
//        public bool btDuocSiEditIsEnabled
//        {
//            get { return _btDuocSiEditIsEnabled; }
//            set
//            {
//                if (_btDuocSiEditIsEnabled != value)
//                {
//                    _btDuocSiEditIsEnabled = value;
//                    NotifyOfPropertyChange(() => btDuocSiEditIsEnabled);
//                }
//            }
//        }


//        //private bool _bntSaveAsIsEnabled;
//        //public bool bntSaveAsIsEnabled
//        //{
//        //    get { return _bntSaveAsIsEnabled; }
//        //    set
//        //    {
//        //        if (_bntSaveAsIsEnabled != value)
//        //        {
//        //            _bntSaveAsIsEnabled = value;
//        //            NotifyOfPropertyChange(() => bntSaveAsIsEnabled);
//        //        }
//        //    }
//        //}


//        private bool _btnUpdateIsEnabled;
//        public bool btnUpdateIsEnabled
//        {
//            get { return _btnUpdateIsEnabled; }
//            set
//            {
//                if (_btnUpdateIsEnabled != value)
//                {
//                    _btnUpdateIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnUpdateIsEnabled);
//                }
//            }
//        }


//        private bool _btnEditIsEnabled;
//        public bool btnEditIsEnabled
//        {
//            get { return _btnEditIsEnabled; }
//            set
//            {
//                if (_btnEditIsEnabled != value)
//                {
//                    _btnEditIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnEditIsEnabled);
//                }
//            }
//        }
//        private bool _btnCreateAndCopyIsEnabled;
//        public bool btnCreateAndCopyIsEnabled
//        {
//            get { return _btnCreateAndCopyIsEnabled; }
//            set
//            {
//                if (_btnCreateAndCopyIsEnabled != value)
//                {
//                    _btnCreateAndCopyIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnCreateAndCopyIsEnabled);
//                }
//            }
//        }

//        private bool CheckToaThuocDuocPhepCapNhat(bool DuocSiDangSua)
//        {
//            if (DuocSiDangSua)
//            {
//                return true;
//            }

//            if (LatestePrecriptions.CreatorStaffID != Globals.LoggedUserAccount.StaffID)
//            {
//                //MessageBox.Show("Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);/*khoi can thong bao*/
//                return false;
//            }
//            //PrescripState = PrescriptionState.EditPrescriptionState;
//            return true;
//        }
//        private bool CheckToaThuocDuocPhepCapNhat(bool DuocSiDangSua, Prescription ToaThuoc)
//        {
//            if (DuocSiDangSua)
//            {
//                return true;
//            }

//            if (ToaThuoc.CreatorStaffID != Globals.LoggedUserAccount.StaffID)
//            {
//                //MessageBox.Show("Chỉ Có Người Tạo Ra Toa Thuốc này mới được phép Chỉnh Sửa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);/*khoi can thong bao*/
//                return false;
//            }
//            //PrescripState = PrescriptionState.EditPrescriptionState;
//            return true;
//        }

//        public void btnEdit()
//        {
//            if (LatestePrecriptions.CanEdit)
//            {
//                if (CheckIsVuotNgayQuiDinhHI())
//                {
//                    if (MessageBox.Show(eHCMSResources.A0838_G1_Msg_ConfTuDChinhNgDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                    {
//                        //Tự động điều chỉnh
//                        AutoAdjust();

//                        //Trạng thái đợi sửa
//                        TrangThaiWaitingSua();
//                    }
//                    else
//                    {
//                        //Trạng thái đợi sửa 
//                        TrangThaiWaitingSua();
//                    }
//                }
//                else
//                {
//                    //Trạng thái đợi sửa 
//                    TrangThaiWaitingSua();
//                }
//            }
//            else
//            {
//                switch (LatestePrecriptions.ReasonCanEdit)
//                {
//                    case "PhaiThucHien-TraPhieuTruoc":
//                        {
//                            MessageBox.Show(eHCMSResources.Z0999_G1_ToaDaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            break;
//                        }
//                    case "ToaNay-DaTungPhatHanh-VaSuDung":
//                        {
//                            MessageBox.Show(eHCMSResources.K0199_G1_KhongTheCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            break;
//                        }
//                }
//            }
//        }

//        private void TrangThaiWaitingSua()
//        {
//            IsEnabled = true;
//            btnUndoIsEnabled = true;
//            btnEditIsEnabled = false;
//            btnCreateAndCopyIsEnabled = false;

//            //Xét Phải Dược Sĩ Sửa Không
//            if (DuocSi_IsEditingToaThuoc)
//            {
//                btDuocSiEditIsEnabled = true;
//            }
//            else
//            {
//                if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc) == false)
//                {
//                    btnUpdateIsEnabled = false;
//                }
//                else
//                {
//                    btnUpdateIsEnabled = true;
//                }

//                btChonChanDoanIsEnabled = true;

//                btnCreateNewIsEnabled = false;
//                btnSaveAddNewIsEnabled = false;

//                //bntSaveAsIsEnabled = false;

//                btnCopyToIsEnabled = false;
//                IsEnabledPrint = false;
//            }

//            IsWaitingSaveDuocSiEdit = false;

//            BackupCurPrescriptionItem();
//            AddNewBlankDrugIntoPrescriptObjectNew();
//        }

//        private void TrangThaiWaitingCreateAndCopy(bool bPhatHanhLai)
//        {
//            IsEnabled = true;
//            btnUndoIsEnabled = true;
//            btnEditIsEnabled = false;
//            btnCreateAndCopyIsEnabled = false;


//            //Xét Phải Dược Sĩ Sửa Không
//            if (DuocSi_IsEditingToaThuoc)
//            {
//                btDuocSiEditIsEnabled = true;
//            }
//            else
//            {
//                btnUpdateIsEnabled = false;

//                btChonChanDoanIsEnabled = true;

//                btnCreateNewIsEnabled = false;
//                btnSaveAddNewIsEnabled = true;

//                //bntSaveAsIsEnabled = true;

//                btnCopyToIsEnabled = false;
//                IsEnabledPrint = false;
//            }

//            IsWaitingSaveDuocSiEdit = false;

//            BackupCurPrescriptionItem();
//            //if (!bPhatHanhLai)
//            //{
//            //    LatestePrecriptions.Diagnosis = "";
//            //}
//            LatestePrecriptions.ExamDate = Globals.PatientAllDetails.PtRegistrationInfo.ExamDate;
//            AddNewBlankDrugIntoPrescriptObjectNew();
//        }

//        //Bấm Nút sửa thì sẽ duyệt qua Toa, coi có vị phạm ràng buộc Số Ngày BH hay không. Nếu vi phạm (thẻ BH mang đến update sau) thì cảnh báo.
//        //Yes thì tự động sửa. No thì không
//        private bool CheckIsVuotNgayQuiDinhHIEx(ObservableCollection<PrescriptionDetail> PrescriptionDetails 
//                                                                    ,int xNgayBHToiDa)
//        {
//            if (PrescriptionDetails == null || PrescriptionDetails.Count<1)
//            {
//                return false;
//            }
//            bool ViPhamBH = false;
//            foreach (var item in PrescriptionDetails)
//            {
//                if (item.IsDrugNotInCat)
//                {
//                    continue;
//                }
//                if (item.DayRpts > xNgayBHToiDa)
//                {
//                    ViPhamBH = true;
//                    break;
//                }
//            }
//            return ViPhamBH;
//        }
//        private bool CheckIsVuotNgayQuiDinhHI()
//        {
//            bool ViPhamBH = false;

//            if (IsPatientInsurance())
//            {
//                if (IsBenhNhanNoiTru())
//                {
//                    ViPhamBH= CheckIsVuotNgayQuiDinhHIEx(LatestePrecriptions.PrescriptionDetails,xNgayBHToiDa_NoiTru);
//                }
//                else
//                {
//                    ViPhamBH= CheckIsVuotNgayQuiDinhHIEx(LatestePrecriptions.PrescriptionDetails, xNgayBHToiDa_NgoaiTru);
//                }
//            }
//            return ViPhamBH;
//        }


//        private void AutoAdjust()
//        {
//            if (IsBenhNhanNoiTru() == false)
//            {
//                foreach (var item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (item.InsuranceCover && item.DayRpts > xNgayBHToiDa_NgoaiTru)
//                    {
//                        double tmp = item.DayRpts;
//                        item.DayRpts = xNgayBHToiDa_NgoaiTru;
//                        item.DayExtended = tmp - xNgayBHToiDa_NgoaiTru;
//                    }
//                }
//            }
//            else
//            {
//                foreach (var item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (item.InsuranceCover && item.DayRpts > xNgayBHToiDa_NoiTru)
//                    {
//                        double tmp = item.DayRpts;
//                        item.DayRpts = xNgayBHToiDa_NoiTru;
//                        item.DayExtended = tmp - xNgayBHToiDa_NoiTru;
//                    }
//                }
//            }
//        }

//        private void GetDayRpt(PrescriptionDetail item, int xNgayBHToiDa) 
//        {
//            if (item.isNeedToUse )
//            {
//                return;
//            }

//            if (LatestePrecriptions.NDay==null
//                || LatestePrecriptions.NDay==0) 
//            {
//                return;
//            }
//            int tmp = LatestePrecriptions.NDay.Value;
//            if (item.HasSchedules)//== (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN
//            {
//                double thuocCu = item.Qty;
//                double ngayCu = item.DayRpts +item.DayExtended;

//                tmp = tmp % 7 == 0 ? tmp : (tmp / 7 + 1) * 7;
//                item.Qty = Math.Ceiling((tmp * thuocCu) / ngayCu);

//                if (item.InsuranceCover)
//                {
//                    if (xNgayBHToiDa > tmp)
//                    {
//                        item.DayRpts = tmp;
//                        item.DayExtended = 0;
//                    }
//                    else
//                    {
//                        item.DayRpts = xNgayBHToiDa;
//                        item.DayExtended = tmp - xNgayBHToiDa;
//                    }
//                }
//                else
//                {
//                    item.DayRpts = tmp;
//                    item.DayExtended = 0;
//                }
                
//                return;
//            }
            
//            if (item.IsDrugNotInCat)
//            {
//                item.DayRpts = tmp;
//                item.DayExtended= 0;
//                SetValueFollowNgayDung(item);
//                return;
//            }
//            if (item.SelectedDrugForPrescription != null
//                            && item.SelectedDrugForPrescription.MaxDayPrescribed > 0
//                            && LatestePrecriptions.NDay.Value > item.SelectedDrugForPrescription.MaxDayPrescribed)
//            {
//                item.DayRpts = item.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//            }
//            else
//                if (item != null && item.SelectedDrugForPrescription != null )
//                    //&& item.SelectedDrugForPrescription.DrugID > 0)
//                {
//                    if (item.SelectedDrugForPrescription.MaxDayPrescribed != null && item.SelectedDrugForPrescription.MaxDayPrescribed != 0)
//                    {
//                        if (item.InsuranceCover)
//                        {
//                            if (xNgayBHToiDa > item.SelectedDrugForPrescription.MaxDayPrescribed)
//                            {
//                                item.DayRpts = tmp;
//                                item.DayExtended = 0;
//                            }
//                            else
//                            {
//                                item.DayRpts = xNgayBHToiDa;
//                                item.DayExtended = tmp - xNgayBHToiDa;
//                            }
//                        }
//                        else 
//                        {
//                            item.DayRpts = tmp;
//                            item.DayExtended = 0;
//                        }
//                    }
//                    else
//                    {
//                        if (item.InsuranceCover && tmp > xNgayBHToiDa)
//                        {
//                            item.DayRpts = xNgayBHToiDa;
//                            item.DayExtended = tmp - xNgayBHToiDa;
//                        }
//                        else
//                        {
//                            item.DayRpts = tmp;
//                            item.DayExtended = 0;
//                        }

//                    }
//                }
//                else/*Dòng trống không*/
//                {
//                    item.DayRpts = tmp;
//                    item.DayExtended = 0;
//                }
//            SetValueFollowNgayDung(item);
//        }

//        private void AutoAdjustCancelDrugShortDays()
//        {
//            if (IsPatientInsurance())
//            {
//                if (IsBenhNhanNoiTru() == false)
//                {
//                    foreach (var item in LatestePrecriptions.PrescriptionDetails)
//                    {
//                        GetDayRpt( item, xNgayBHToiDa_NgoaiTru) ;
//                    }
//                }
//                else
//                {
//                    foreach (var item in LatestePrecriptions.PrescriptionDetails)
//                    {
//                        GetDayRpt(item, xNgayBHToiDa_NoiTru);
//                    }
//                }
//            }
//            else/*Không Có BH*/
//            {
//                foreach (var item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    int tmp = LatestePrecriptions.NDay.Value;
//                    if (item == null) 
//                    {
//                        continue;
//                    }
//                    if (item.V_DrugType==(long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN )
//                    {
//                        //tính quy tắc tam xuất ở đây
//                        double thuocCu = item.Qty;
//                        double ngayCu = item.DayRpts;
//                        item.DayRpts = tmp % 7 == 0 ? tmp : (tmp / 7 + 1)*7;
//                        item.Qty = Math.Ceiling((item.DayRpts * thuocCu) / ngayCu);
//                        continue;
//                    }

//                    if (item != null && item.SelectedDrugForPrescription != null && item.SelectedDrugForPrescription.DrugID > 0)
//                    {
//                        if (item.SelectedDrugForPrescription.MaxDayPrescribed != null && item.SelectedDrugForPrescription.MaxDayPrescribed != 0)//---check lai cho nay
//                        {
//                            if (item.SelectedDrugForPrescription.MaxDayPrescribed > tmp)
//                            {
//                                item.DayRpts = tmp;
//                                item.DayExtended = 0;
//                            }
//                            else
//                            {
//                                item.DayRpts = item.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//                                item.DayExtended = 0;
//                            }
//                        }
//                        else
//                        {
//                            item.DayRpts = tmp;
//                            item.DayExtended = 0;
//                        }
//                    }
//                    else/*Dòng trống không*/
//                    {
//                        item.DayRpts = tmp;
//                        item.DayExtended = 0;
//                    }

//                    SetValueFollowNgayDung(item);
//                }
//            }

//        }

//        private void AutoAdjustCancelDrugShortDays(PrescriptionDetail detail)
//        {
//            if (IsPatientInsurance())
//            {
//                if (IsBenhNhanNoiTru() == false)
//                {
//                    GetDayRpt(detail, xNgayBHToiDa_NgoaiTru);                    
//                }
//                else
//                {
//                    GetDayRpt(detail, xNgayBHToiDa_NoiTru);                    
//                }
//            }
//            else/*Không Có BH*/
//            {
//                int tmp = LatestePrecriptions.NDay.Value;
//                if (detail == null)
//                {
//                    return;
//                }
//                if (detail.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
//                {
//                    //tính quy tắc tam xuất ở đây
//                    double thuocCu = detail.Qty;
//                    double ngayCu = detail.DayRpts;
//                    detail.DayRpts = tmp % 7 == 0 ? tmp : (tmp / 7 + 1) * 7;
//                    detail.Qty = Math.Ceiling((detail.DayRpts * thuocCu) / ngayCu);
//                    return;
//                }

//                if (detail != null && detail.SelectedDrugForPrescription != null && detail.SelectedDrugForPrescription.DrugID > 0)
//                {
//                    if (detail.SelectedDrugForPrescription.MaxDayPrescribed != null && detail.SelectedDrugForPrescription.MaxDayPrescribed != 0)//---check lai cho nay
//                    {
//                        if (detail.SelectedDrugForPrescription.MaxDayPrescribed > tmp)
//                        {
//                            detail.DayRpts = tmp;
//                            detail.DayExtended = 0;
//                        }
//                        else
//                        {
//                            detail.DayRpts = detail.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//                            detail.DayExtended = 0;
//                        }
//                    }
//                    else
//                    {
//                        detail.DayRpts = tmp;
//                        detail.DayExtended = 0;
//                    }
//                }
//                else/*Dòng trống không*/
//                {
//                    detail.DayRpts = tmp;
//                    detail.DayExtended = 0;
//                }

//                SetValueFollowNgayDung(detail);
               
//            }

//        }

//        private void Prescriptions_Update()
//        {
//            IsEnabledForm = false;

//            IsWaitingCapNhatToaThuoc = true;
//            IList<PrescriptionIssueHistory> lstPrescriptionIssueHistory = new List<PrescriptionIssueHistory>();
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginPrescriptions_Update(ObjTaoThanhToaMoi, PrecriptionsBeforeUpdate, false, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {

//                            string Result = "";

//                            long NewPrescriptionID = 0;
//                            long IssueID = 0;

//                            contract.EndPrescriptions_Update(out Result, out NewPrescriptionID, out IssueID,out lstPrescriptionIssueHistory, asyncResult);
//                            string druglist =Result.IndexOf("@")>0?
//                                Result.Substring(Result.IndexOf("@"), Result.Length - Result.IndexOf("@")):"None";
//                            Result = Result.Replace(druglist,"");
//                            switch (Result)
//                            {
//                                case "OK":
//                                    {
//                                        ObjTaoThanhToaMoi.PrescriptID = NewPrescriptionID;
//                                        ObjTaoThanhToaMoi.IssueID = IssueID;

//                                        PrecriptionsForPrint = ObjTaoThanhToaMoi;
//                                        IsEnabledAutoComplete = false;

//                                        //bntSaveAsVisibility = Visibility.Collapsed;
//                                        //btnUpdateVisibility = true;


//                                        IsEnabled = false;
//                                        btnUpdateIsEnabled = false;
//                                        btnSaveAddNewIsEnabled = false;
//                                        btnUndoIsEnabled = false;
//                                        btnCreateNewIsEnabled = true;
//                                        btnEditIsEnabled = true;
//                                        btnCreateAndCopyIsEnabled = true;
//                                        IsEnabledPrint = true;

//                                        btChonChanDoanIsEnabled = false;

//                                        //phát sự kiện load lại danh sách toa thuốc
//                                        Globals.EventAggregator.Publish(new ReloadDataePrescriptionEvent { });
//                                        ////phát sự kiện load lại danh sách toa thuốc
//                                        ////đọc lại toa thuốc cuối
//                                        //GetLatestPrescriptionByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);
//                                        ////đọc lại toa thuốc cuối

//                                        //Dinh sua
//                                        Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });

//                                        MessageBox.Show(eHCMSResources.Z0998_G1_DaCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

//                                        break;
//                                    }
//                                case "Error":
//                                    {
//                                        //Cộng lại dòng trống
//                                        AddNewBlankDrugIntoPrescriptObjectNew();
//                                        //Cộng lại dòng trống

//                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                        break;
//                                    }
//                                case "PhaiThucHien-TraPhieuTruoc":
//                                    {
//                                        //Cộng lại dòng trống
//                                        AddNewBlankDrugIntoPrescriptObjectNew();
//                                        //Cộng lại dòng trống

//                                        MessageBox.Show(eHCMSResources.Z0999_G1_ToaDaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                        break;
//                                    }
//                                case "ToaNay-DaTungPhatHanh-VaSuDung":
//                                    {
//                                        //Cộng lại dòng trống
//                                        AddNewBlankDrugIntoPrescriptObjectNew();
//                                        //Cộng lại dòng trống

//                                        MessageBox.Show(eHCMSResources.K0199_G1_KhongTheCNhatToaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                        break;
//                                    }
//                                case "Duplex-Prescriptions_PrescriptionsInDay":
//                                    {
//                                        //Cộng lại dòng trống
//                                        AddNewBlankDrugIntoPrescriptObjectNew();
//                                        //Cộng lại dòng trống

//                                        MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
//                                            + Environment.NewLine + druglist.Replace("@","")
//                                            + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                        break;
//                                    }
//                                case "Error-Exception":
//                                    {
//                                        //Cộng lại dòng trống
//                                        AddNewBlankDrugIntoPrescriptObjectNew();
//                                        //Cộng lại dòng trống

//                                        MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                        break;
//                                    }
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.ToString());
//                        }
//                        finally
//                        {
//                            IsEnabledForm = true;

//                            IsWaitingCapNhatToaThuoc = false;
//                        }

//                    }), null);
//                }
//            });

//            t.Start();
//        }

//        public void btUpdateDoctorAdvice()
//        {
//            Prescriptions_UpdateDoctorAdvice();
//        }

//        private void Prescriptions_UpdateDoctorAdvice()
//        {
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            IsEnabledForm = false;

//            IsWaitingPrescriptions_UpdateDoctorAdvice = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginPrescriptions_UpdateDoctorAdvice(LatestePrecriptions, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            string Result = "";
//                            contract.EndPrescriptions_UpdateDoctorAdvice(out Result, asyncResult);
//                            if (Result == "1")
//                            {
//                                IsEnabledAutoComplete = false;

//                                //bntSaveAsVisibility = Visibility.Collapsed;
//                                //btnUpdateVisibility = Visibility.Collapsed;


//                                IsEnabled = false;
//                                btnEditIsEnabled = true;
//                                btnCreateAndCopyIsEnabled = true;
//                                IsEnabledPrint = true;

//                                //chỉ cập nhật lời dặn --> ko cần đọc lại toa
//                                ////////phát sự kiện load lại danh sách toa thuốc
//                                //////Globals.EventAggregator.Publish(new ReloadDataePrescriptionEvent { });
//                                ////////phát sự kiện load lại danh sách toa thuốc

//                                ////////đọc lại toa thuốc cuối
//                                //////GetLatestPrescriptionByPtID(Globals.PatientInfo.PatientID);
//                                ////////đọc lại toa thuốc cuối

//                                MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                            else
//                            {
//                                MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsEnabledForm = true;

//                            IsWaitingPrescriptions_UpdateDoctorAdvice = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }


//        //Khóa mở S,Tr,C,T TextBox theo Combo chọn
//        public void cbxChooseDose_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
//            if (Ctr != null)
//            {
//                if (Ctr.SelectedItemEx != null)
//                {
//                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

//                    if (Objtmp != null && Objtmp.HasSchedules == true)
//                    {
//                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                        return;
//                    }

//                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;
//                    SetEnableDisalbeInputDose(ObjChooseDose, Objtmp);

//                    if (ObjChooseDose.ID > 0)
//                    {
//                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
//                        if (Objtmp != null && Objtmp.DayRpts > 0)
//                        {
//                            SetValueFollowNgayDung(Objtmp);
//                        }
//                    }
//                }
//            }
//        }

//        public void cbxChooseDoseForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            KeyEnabledComboBox Ctr = (sender as KeyEnabledComboBox);
//            if (Ctr != null)
//            {
//                if (Ctr.SelectedItemEx != null)
//                {
//                    PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

//                    if (Objtmp != null && Objtmp.HasSchedules == true)
//                    {
//                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                        return;
//                    }

//                    ChooseDose ObjChooseDose = Ctr.SelectedItemEx as ChooseDose;
//                    SetEnableDisalbeInputDose(ObjChooseDose, Objtmp);

//                    if (ObjChooseDose.ID > 0)
//                    {
//                        SetValueFollowComboDose(ObjChooseDose, Objtmp);
//                        if (Objtmp != null && Objtmp.DayRpts > 0)
//                        {
//                            SetValueFollowNgayDung(Objtmp);
//                        }
//                    }
//                }
//            }
//        }
//        //Khóa mở S,Tr,C,T TextBox theo Combo chọn


//        //#region "S,Tr,C,T LosFocus"
//        //public void tbMDose_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            Objtmp.MDose = ChangeDoseStringToFloat(Objtmp.MDoseStr);
//        //            if (Objtmp.HasSchedules == false)
//        //            {
//        //                SetValueFollowNgayDung(Objtmp);
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbADose_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            Objtmp.ADose = ChangeDoseStringToFloat(Objtmp.ADoseStr);
//        //            if (Objtmp.HasSchedules == false)
//        //            {
//        //                SetValueFollowNgayDung(Objtmp);
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbEDose_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            Objtmp.EDose = ChangeDoseStringToFloat(Objtmp.EDoseStr);
//        //            if (Objtmp.HasSchedules == false)
//        //            {
//        //                SetValueFollowNgayDung(Objtmp);
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbNDose_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            Objtmp.NDose = ChangeDoseStringToFloat(Objtmp.NDoseStr);
//        //            if (Objtmp.HasSchedules == false)
//        //            {
//        //                SetValueFollowNgayDung(Objtmp);
//        //            }
//        //        }
//        //    }
//        //}

//        //public void tbdosage_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);
//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;

//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.Validate())
//        //            {
//        //                if (Objtmp.HasSchedules == false)
//        //                {
//        //                    Objtmp.dosage = ChangeDoseStringToFloat(Objtmp.dosageStr);
//        //                    if (Objtmp.dosage == 0)
//        //                    {
//        //                        Objtmp.dosageStr = "0";
//        //                    }
//        //                    SetValueFollowComboDose(Objtmp.ChooseDose, Objtmp);
//        //                    if (Objtmp.dosage > 0)
//        //                    {
//        //                        SetValueFollowNgayDung(Objtmp);
//        //                    }
//        //                }
//        //            }
//        //        }
//        //    }
//        //}

//        //public void tbNgayDung_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);
//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = grdPrescription.SelectedItem as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (IsPatientInsurance())
//        //            {
//        //                if (IsBenhNhanNoiTru() == false)
//        //                {
//        //                    if (Objtmp.DayRpts > xNgayBHToiDa_NgoaiTru)
//        //                    {
//        //                        Objtmp.DayRpts = xNgayBHToiDa_NgoaiTru;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                    else if (Objtmp.DayRpts < xNgayBHToiDa_NgoaiTru && Objtmp.DayExtended > 0)
//        //                    {
//        //                        Objtmp.DayExtended = 0;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                }
//        //                else
//        //                {
//        //                    if (Objtmp.DayRpts > xNgayBHToiDa_NoiTru)
//        //                    {
//        //                        Objtmp.DayRpts = xNgayBHToiDa_NoiTru;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                    else if (Objtmp.DayRpts < xNgayBHToiDa_NoiTru && Objtmp.DayExtended > 0)
//        //                    {
//        //                        Objtmp.DayExtended = 0;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                }
//        //            }
//        //            if (Objtmp.HasSchedules == false)
//        //            {
//        //                SetValueFollowNgayDung(Objtmp);
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbNgayDungExtended_LostFocus(object sender, RoutedEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);
//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = grdPrescription.SelectedItem as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (IsPatientInsurance())
//        //            {
//        //                if (IsBenhNhanNoiTru() == false)
//        //                {
//        //                    if (Objtmp.DayRpts > xNgayBHToiDa_NgoaiTru)
//        //                    {
//        //                        Objtmp.DayRpts = xNgayBHToiDa_NgoaiTru;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                    else if (Objtmp.DayRpts < xNgayBHToiDa_NgoaiTru && Objtmp.DayExtended > 0)
//        //                    {
//        //                        Objtmp.DayExtended = 0;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                }
//        //                else
//        //                {
//        //                    if (Objtmp.DayRpts > xNgayBHToiDa_NoiTru)
//        //                    {
//        //                        Objtmp.DayRpts = xNgayBHToiDa_NoiTru;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                    else if (Objtmp.DayRpts < xNgayBHToiDa_NoiTru && Objtmp.DayExtended > 0)
//        //                    {
//        //                        Objtmp.DayExtended = 0;
//        //                        Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//        //                    }
//        //                }
//        //            }

//        //            if (Objtmp.HasSchedules == false)
//        //            {
//        //                SetValueFollowNgayDung(Objtmp);
//        //            }
//        //        }
//        //    }
//        //}


//        //#endregion

//        private float ChangeDoseStringToFloat(string value)
//        {
//            float result = 0;
//            if (!string.IsNullOrEmpty(value))
//            {
//                if (value.Contains("/"))
//                {
//                    string pattern = @"\b[\d]+/[\d]+\b";
//                    if (!Regex.IsMatch(value, pattern))
//                    {
//                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
//                        return 0;
//                    }
//                    else
//                    {
//                        string[] items = null;
//                        items = value.Split('/');
//                        if (items.Count() > 2 || items.Count() == 0)
//                        {
//                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
//                            return 0;
//                        }
//                        else if (float.Parse(items[1]) == 0)
//                        {
//                            Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
//                            return 0;
//                        }
//                        result = (float)Math.Round((float.Parse(items[0]) / float.Parse(items[1])), 3);
//                        if (result < 0)
//                        {
//                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
//                            return 0;
//                        }
//                    }
//                }
//                else
//                {
//                    try
//                    {
//                        result = float.Parse(value);
//                        if (result < 0)
//                        {
//                            Globals.ShowMessage(eHCMSResources.Z1071_G1_LieuDungKgNhoHon0, eHCMSResources.G0442_G1_TBao);
//                            return 0;
//                        }
//                    }
//                    catch
//                    {
//                        Globals.ShowMessage(eHCMSResources.Z1069_G1_LieuDungKgHopLe, eHCMSResources.G0442_G1_TBao);
//                        return 0;
//                    }
//                }
//            }
//            return result;
//        }

//        private bool IsBenhNhanNoiTru()
//        {
//            //cho nay can coi lai vi ben nha thuoc sua toa thuoc se khong co RegistrationInfo
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.PatientAllDetails.PatientInfo != null && Globals.PatientAllDetails.PatientInfo.PatientID > 0)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType ==
//                    AllLookupValues.RegistrationType.NOI_TRU)
//                {
//                    return true;
//                }
//                return false;
//            }
//            return false;
//        }

//        private void ChangeDosage(string value, object Obj)
//        {
//            PrescriptionDetail Objtmp = Obj as PrescriptionDetail;
//            if (Objtmp != null && (value == null || Objtmp.dosageStr.ToLower() != value.ToLower()))
//            {
//                if (Objtmp.Validate())
//                {
//                    if (Objtmp.HasSchedules == false)
//                    {
//                        Objtmp.dosage = ChangeDoseStringToFloat(Objtmp.dosageStr);
//                        if (Objtmp.dosage == 0)
//                        {
//                            Objtmp.dosageStr = "0";
//                        }
//                        SetValueFollowComboDose(Objtmp.ChooseDose, Objtmp);
//                        SetValueFollowNgayDung(Objtmp);
//                    }
//                    else
//                    {
//                        Objtmp.dosageStr = "0";
//                        Objtmp.dosage = 0;
//                        MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                    }
//                }
//            }
//        }

//        private void ChangeMDose(string value, object Obj)
//        {
//            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
//            if (Objtmp != null && (value == null || Objtmp.MDoseStr.ToLower() != value.ToLower()))
//            {
//                Objtmp.MDose = ChangeDoseStringToFloat(Objtmp.MDoseStr);
//                if (Objtmp.HasSchedules == false)
//                {
//                    SetValueFollowNgayDung(Objtmp);
//                }
//                else
//                {
//                    Objtmp.MDoseStr = "0";
//                    Objtmp.MDose = 0;
//                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                }
//            }
//        }

//        private void ChangeNDose(string value, object Obj)
//        {
//            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
//            if (Objtmp != null && (value == null || Objtmp.NDoseStr.ToLower() != value.ToLower()))
//            {
//                Objtmp.NDose = ChangeDoseStringToFloat(Objtmp.NDoseStr);
//                if (Objtmp.HasSchedules == false)
//                {
//                    SetValueFollowNgayDung(Objtmp);
//                }
//                else
//                {
//                    Objtmp.NDoseStr = "0";
//                    Objtmp.NDose = 0;
//                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                }
//            }
//        }

//        private void ChangeEDose(string value, object Obj)
//        {
//            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
//            if (Objtmp != null && (value == null || Objtmp.EDoseStr.ToLower() != value.ToLower()))
//            {
//                Objtmp.EDose = ChangeDoseStringToFloat(Objtmp.EDoseStr);
//                if (Objtmp.HasSchedules == false)
//                {
//                    SetValueFollowNgayDung(Objtmp);
//                }
//                else
//                {
//                    Objtmp.EDoseStr = "0";
//                    Objtmp.EDose = 0;
//                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                }
//            }
//        }

//        private void ChangeADose(string value, object Obj)
//        {
//            PrescriptionDetail Objtmp = (PrescriptionDetail)this.grdPrescription.SelectedItem;
//            if (Objtmp != null && (value == null || Objtmp.ADoseStr.ToLower() != value.ToLower()))
//            {
//                Objtmp.ADose = ChangeDoseStringToFloat(Objtmp.ADoseStr);
//                if (Objtmp.HasSchedules == false)
//                {
//                    SetValueFollowNgayDung(Objtmp);
//                }
//                else
//                {
//                    Objtmp.ADoseStr = "0";
//                    Objtmp.ADose = 0;
//                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                }
//            }
//        }

//        private void ChangeNgayDung(double value, object Obj)
//        {
//            PrescriptionDetail Objtmp = Obj as PrescriptionDetail;
//            if (Objtmp != null)
//            {
//                if (Objtmp.V_DrugType==(long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
//                {
//                    return;
//                }
//                if (Objtmp.SelectedDrugForPrescription.MaxDayPrescribed>0
//                    && Objtmp.DayRpts> Objtmp.SelectedDrugForPrescription.MaxDayPrescribed)
//                {
//                    MessageBox.Show(eHCMSResources.A0360_G1_Msg_WarnSoNgDungThuoc1);
//                    Objtmp.DayRpts = Objtmp.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//                }
//                if (IsPatientInsurance())
//                {
//                    if (Objtmp.IsDrugNotInCat) return;
//                    if (IsBenhNhanNoiTru() == false)
//                    {
//                        if (Objtmp.InsuranceCover && Objtmp.DayRpts > xNgayBHToiDa_NgoaiTru)
//                        {
//                            double NgayInput = Objtmp.DayRpts.DeepCopy();

//                            Objtmp.DayRpts = xNgayBHToiDa_NgoaiTru;

//                            Objtmp.DayExtended = NgayInput - xNgayBHToiDa_NgoaiTru;

//                            Globals.ShowMessage(string.Format(eHCMSResources.Z1085_G1_NgBHKgLonHon0Ng, xNgayBHToiDa_NgoaiTru.ToString(), Objtmp.DayExtended.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                        else if (Objtmp.DayRpts <= xNgayBHToiDa_NgoaiTru && Objtmp.DayExtended > 0)
//                        {
//                            //Objtmp.DayExtended = 0;
//                            //Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                    }
//                    else
//                    {
//                        if (Objtmp.InsuranceCover && Objtmp.DayRpts > xNgayBHToiDa_NoiTru)
//                        {
//                            double NgayInput = Objtmp.DayRpts.DeepCopy();

//                            Objtmp.DayRpts = xNgayBHToiDa_NoiTru;

//                            Objtmp.DayExtended = NgayInput - xNgayBHToiDa_NoiTru;

//                            Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NoiTru.ToString()) + string.Format(" {0}", eHCMSResources.N0045_G1_Ng.ToLower()) + Environment.NewLine + Objtmp.DayExtended.ToString() + " ngày còn lại sẽ đưa vào ngày dùng thêm", eHCMSResources.G0442_G1_TBao);
//                        }
//                        else if (Objtmp.DayRpts <= xNgayBHToiDa_NoiTru && Objtmp.DayExtended > 0)
//                        {
//                            Objtmp.DayExtended = 0;
//                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                    }
//                }

//                if (Objtmp.HasSchedules == false)
//                {
//                    SetValueFollowNgayDung(Objtmp);
//                }
//            }
//        }

//        private void ChangeNgayDungExtend(double value, object Obj)
//        {
//            PrescriptionDetail Objtmp = Obj as PrescriptionDetail;
//            if (Objtmp != null && Objtmp.DayExtended != value)
//            {
//                if (Objtmp.V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
//                {
//                    return;
//                }
//                if (Objtmp.SelectedDrugForPrescription.MaxDayPrescribed > 0
//                    && Objtmp.DayRpts > Objtmp.SelectedDrugForPrescription.MaxDayPrescribed)
//                {
//                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0962_G1_SoNgDungThuoc));
//                    Objtmp.DayRpts = Objtmp.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//                }
//                if (IsPatientInsurance())
//                {
//                    if (IsBenhNhanNoiTru() == false)
//                    {
//                        if (Objtmp.InsuranceCover && Objtmp.DayRpts > xNgayBHToiDa_NgoaiTru)
//                        {
//                            Objtmp.DayRpts = xNgayBHToiDa_NgoaiTru;
//                            Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                        else if (Objtmp.DayRpts < xNgayBHToiDa_NgoaiTru && Objtmp.DayExtended > 0)
//                        {
//                            Objtmp.DayExtended = 0;
//                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NgoaiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                    }
//                    else
//                    {
//                        if (Objtmp.DayRpts > xNgayBHToiDa_NoiTru)
//                        {
//                            Objtmp.DayRpts = xNgayBHToiDa_NoiTru;
//                            Globals.ShowMessage(string.Format(eHCMSResources.Z0963_G1_NgBHKgLonHon0, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                        else if (Objtmp.DayRpts < xNgayBHToiDa_NoiTru && Objtmp.DayExtended > 0)
//                        {
//                            Objtmp.DayExtended = 0;
//                            Globals.ShowMessage(string.Format(eHCMSResources.Z1087_G1_NgBHNgDungThem2, xNgayBHToiDa_NoiTru.ToString()), eHCMSResources.G0442_G1_TBao);
//                        }
//                    }
//                }

//                if (Objtmp.HasSchedules == false)
//                {
//                    SetValueFollowNgayDung(Objtmp);
//                }
//                else
//                {
//                    Objtmp.DayExtended = value;
//                    MessageBox.Show(string.Format(eHCMSResources.Z0989_G1_ThuocDaCoCDinhUongTheoTuan, Objtmp.SelectedDrugForPrescription.BrandName.Trim()));
//                }
//            }
//        }

//        private void SetValueFollowNgayDung(PrescriptionDetail Objtmp)
//        {
//            //if (Objtmp != null && Objtmp.IsDrugNotInCat == false)
//            {
//                if (Objtmp.HasSchedules == false)
//                {
//                    Nullable<float> TongThuoc = 0;
//                    float Tong = 0;

//                    if (Objtmp != null && Objtmp.SelectedDrugForPrescription != null)
//                    {
//                        TongThuoc = Objtmp.MDose + Objtmp.ADose.GetValueOrDefault() + Objtmp.NDose.GetValueOrDefault() + Objtmp.EDose.GetValueOrDefault();
//                        Tong = (float)(TongThuoc.Value * (Objtmp.DayRpts + Objtmp.DayExtended)
//                            * (Objtmp.SelectedDrugForPrescription.UnitVolume == 0 ? 1 : Objtmp.SelectedDrugForPrescription.UnitVolume)) 
//                            / ((float)Objtmp.SelectedDrugForPrescription.DispenseVolume==0? 1:(float)Objtmp.SelectedDrugForPrescription.DispenseVolume);
//                        Objtmp.Qty = Math.Ceiling(Tong);
//                    }
//                }
//            }
//        }

//        private void SetValueFollowComboDoseOld(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
//        {
//            if (Objtmp != null)
//            {
//                if (ObjChooseDose != null)
//                {
//                    switch (ObjChooseDose.ID)
//                    {
//                        case 1:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 2:
//                            {
//                                Objtmp.MDose = Objtmp.dosage;
//                                Objtmp.ADose = Objtmp.dosage;
//                                Objtmp.EDose = Objtmp.dosage;
//                                Objtmp.NDose = 0;

//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                Objtmp.NDoseStr = "0";
//                                break;
//                            }
//                        case 3:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 4:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 5:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 6:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = 0;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = "0";
//                            break;
//                        case 7:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = 0;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = "0";
//                            break;
//                        case 8:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 9:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = 0;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = "0";
//                            break;
//                        case 10:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 11:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 12:
//                            Objtmp.MDose = Objtmp.dosage;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = 0;

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = "0";
//                            break;
//                        case 13:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = Objtmp.dosage;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = 0;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = Objtmp.dosageStr;
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = "0";
//                            break;
//                        case 14:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = Objtmp.dosage;
//                            Objtmp.NDose = 0;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = Objtmp.dosageStr;
//                            Objtmp.NDoseStr = "0";
//                            break;
//                        case 15:
//                            Objtmp.MDose = 0;
//                            Objtmp.ADose = 0;
//                            Objtmp.EDose = 0;
//                            Objtmp.NDose = Objtmp.dosage;

//                            Objtmp.MDoseStr = "0";
//                            Objtmp.ADoseStr = "0";
//                            Objtmp.EDoseStr = "0";
//                            Objtmp.NDoseStr = Objtmp.dosageStr;
//                            break;
//                    }

//                }
//            }
//        }

//        private void SetEnableDisalbeInputDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
//        {
//            if (grdPrescription != null)
//            {
//                int indexRow = grdPrescription.SelectedIndex;
//                if (indexRow<0)
//                {
//                    return;
//                }

//                AxTextBox Sang = grdPrescription.Columns[(int)DataGridCol.MDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
//                AxTextBox Trua = grdPrescription.Columns[(int)DataGridCol.NDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
//                AxTextBox Chieu = grdPrescription.Columns[(int)DataGridCol.ADOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;
//                AxTextBox Toi = grdPrescription.Columns[(int)DataGridCol.EDOSE].GetCellContent(grdPrescription.SelectedItem) as AxTextBox;

//                if (indexRow >= 0)
//                {
//                    if (Objtmp != null)
//                    {
//                        if (Objtmp.ChooseDose != null)
//                        {
//                            if (Objtmp.dosage <= 0)
//                            {
//                                Objtmp.dosage = 0;
//                            }
//                            switch (ObjChooseDose.ID)
//                            {
//                                case 1:

//                                    //Sang.IsReadOnly = false;
//                                    //Trua.IsReadOnly = false;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = false;

//                                    Objtmp.MDose = Objtmp.dosage;
//                                    Objtmp.ADose = Objtmp.dosage;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = Objtmp.dosage;
//                                    break;
//                                case 2:
//                                    {
//                                        //Sang.IsReadOnly = false;
//                                        //Trua.IsReadOnly = false;
//                                        //Chieu.IsReadOnly = false;
//                                        //Toi.IsReadOnly = true;

//                                        Objtmp.MDose = Objtmp.dosage;
//                                        Objtmp.ADose = Objtmp.dosage;
//                                        Objtmp.EDose = Objtmp.dosage;
//                                        Objtmp.NDose = 0;
//                                        break;
//                                    }
//                                case 3:
//                                    {
//                                        //Sang.IsReadOnly = false;
//                                        //Trua.IsReadOnly = false;
//                                        //Chieu.IsReadOnly = true;
//                                        //Toi.IsReadOnly = false;

//                                        Objtmp.MDose = Objtmp.dosage;
//                                        Objtmp.ADose = Objtmp.dosage;
//                                        Objtmp.EDose = 0;
//                                        Objtmp.NDose = Objtmp.dosage;
//                                        break;
//                                    }
//                                case 4:

//                                    //Sang.IsReadOnly = false;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = false;

//                                    Objtmp.MDose = Objtmp.dosage;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = Objtmp.dosage;
//                                    break;
//                                case 5:
//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = false;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = false;

//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = Objtmp.dosage;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = Objtmp.dosage;
//                                    break;
//                                case 6:

//                                    //Sang.IsReadOnly = false;
//                                    //Trua.IsReadOnly = false;
//                                    //Chieu.IsReadOnly = true;
//                                    //Toi.IsReadOnly = true;

//                                    Objtmp.MDose = Objtmp.dosage;
//                                    Objtmp.ADose = Objtmp.dosage;
//                                    Objtmp.EDose = 0;
//                                    Objtmp.NDose = 0;
//                                    break;
//                                case 7:

//                                    //Sang.IsReadOnly = false;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = true;


//                                    Objtmp.MDose = Objtmp.dosage;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = 0;
//                                    break;
//                                case 8:
//                                    //Sang.IsReadOnly = false;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = true;
//                                    //Toi.IsReadOnly = false;


//                                    Objtmp.MDose = Objtmp.dosage;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = 0;
//                                    Objtmp.NDose = Objtmp.dosage;

//                                    break;
//                                case 9:
//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = false;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = true;

//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = Objtmp.dosage;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = 0;
//                                    break;
//                                case 10:
//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = false;
//                                    //Chieu.IsReadOnly = true;
//                                    //Toi.IsReadOnly = false;


//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = Objtmp.dosage;
//                                    Objtmp.EDose = 0;
//                                    Objtmp.NDose = Objtmp.dosage;
//                                    break;
//                                case 11:

//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = false;

//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = Objtmp.dosage;
//                                    break;
//                                case 12:

//                                    //Sang.IsReadOnly = false;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = true;
//                                    //Toi.IsReadOnly = true;

//                                    Objtmp.MDose = Objtmp.dosage;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = 0;
//                                    Objtmp.NDose = 0;
//                                    break;
//                                case 13:

//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = false;
//                                    //Chieu.IsReadOnly = true;
//                                    //Toi.IsReadOnly = true;


//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = Objtmp.dosage;
//                                    Objtmp.EDose = 0;
//                                    Objtmp.NDose = 0;
//                                    break;
//                                case 14:

//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = false;
//                                    //Toi.IsReadOnly = true;

//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = Objtmp.dosage;
//                                    Objtmp.NDose = 0;
//                                    break;
//                                case 15:

//                                    //Sang.IsReadOnly = true;
//                                    //Trua.IsReadOnly = true;
//                                    //Chieu.IsReadOnly = true;
//                                    //Toi.IsReadOnly = false;

//                                    Objtmp.MDose = 0;
//                                    Objtmp.ADose = 0;
//                                    Objtmp.EDose = 0;
//                                    Objtmp.NDose = Objtmp.dosage;
//                                    break;
//                            }

//                        }
//                    }

//                }
//            }
//        }

//        private void SetValueFollowComboDose(ChooseDose ObjChooseDose, PrescriptionDetail Objtmp)
//        {
//            if (Objtmp != null)
//            {
//                if (ObjChooseDose != null)
//                {
//                    Objtmp.MDoseStr = "0";
//                    Objtmp.NDoseStr = "0";
//                    Objtmp.ADoseStr = "0";
//                    Objtmp.EDoseStr = "0";
//                    switch (ObjChooseDose.ID)
//                    {
//                        case 1://S

//                            Objtmp.MDoseStr = Objtmp.dosageStr;
//                            break;
//                        case 2://Tr 
//                            {
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 3://C
//                            {
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 4://T
//                            {
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 5://S Tr
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 6://S C
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 7://S T
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }

//                        case 8://Tr C
//                            {
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 9://Tr T
//                            {
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }

//                        case 10://C T
//                            {
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 11://S Tr C
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                break;
//                            }

//                        case 12://S Tr T
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 13://S C T
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 14://Tr C T
//                            {
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                        case 15://S Tr C T
//                            {
//                                Objtmp.MDoseStr = Objtmp.dosageStr;
//                                Objtmp.NDoseStr = Objtmp.dosageStr;
//                                Objtmp.ADoseStr = Objtmp.dosageStr;
//                                Objtmp.EDoseStr = Objtmp.dosageStr;
//                                break;
//                            }
//                    }
//                }
//            }
//        }


//        #endregion


//        #region Tạo Toa Mới

//        //Chọn 1 chẩn đoán để ra toa
//        public void Handle(DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> message)
//        {
//            if (message != null)
//            {
//                ObjDiagnosisTreatment_Current = message.DiagnosisTreatment.DeepCopy();

//                string cd = ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
//                if (string.IsNullOrEmpty(cd))
//                {
//                    cd = ObjDiagnosisTreatment_Current.Diagnosis.Trim();
//                }
//                LatestePrecriptions.Diagnosis = cd;
//                LatestePrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
//                if (LatestePrecriptions.PrescriptionIssueHistory == null)
//                {
//                    LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
//                }

//                //Cụ thể  DV nào
//                if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                    {
//                        if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                        {
//                            LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = ObjDiagnosisTreatment_Current.PtRegDetailID;
//                        }
//                        else
//                        {
//                            MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                            return;
//                        }
//                    }
//                    else/*Khám VIP, Khám Cho Nội Trú*/
//                    {
//                        LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
//                    }
//                }
//                else
//                {
//                    MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                    return;
//                }
//                //Cụ thể  DV nào

//            }
//        }

//        //Dinh them phan hen ngay tai kham
//        private int CheckAppDate()
//        {
//            if (LatestePrecriptions.NDay.GetValueOrDefault() > 0)
//            {
//                return LatestePrecriptions.NDay.GetValueOrDefault();
//            }
//            else 
//            {
//                int maxDay=0;
//                foreach (var item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (maxDay<item.DayRpts)
//                    {
//                        maxDay = Convert.ToInt32( item.DayRpts);
//                    }
//                }
//                return maxDay;
//            }
//        }
//        private void SetValueTaoThanhToaMoi()
//        {
//            ObjTaoThanhToaMoi = LatestePrecriptions.DeepCopy();
//            //ObjTaoThanhToaMoi.NDay = CheckAppDate();

//            if (CapNhatToaThuoc)
//            {
//                ObjTaoThanhToaMoi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.EDITTUTOAKHAC;
//            }
//            if (TaoThanhToaMoi)
//            {
//                ObjTaoThanhToaMoi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.EDITTUTOAKHAC;
//            }
//            ObjTaoThanhToaMoi.OriginalPrescriptID = LatestePrecriptions.OriginalPrescriptID;/*Gốc Của Phát Hành Này*/

//            ObjTaoThanhToaMoi.PrescriptionIssueHistory = new PrescriptionIssueHistory();

//            //Gán lại ServiceRecID, PtRegDetailID
//            ObjTaoThanhToaMoi.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
//            ObjTaoThanhToaMoi.CreatorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();

//            //Cụ thể  DV nào
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        ObjTaoThanhToaMoi.PrescriptionIssueHistory.PtRegDetailID = ObjDiagnosisTreatment_Current.PtRegDetailID;
//                    }
//                    else
//                    {
//                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                        return;
//                    }
//                }
//                else/*Khám VIP, Khám Cho Nội Trú*/
//                {
//                    ObjTaoThanhToaMoi.PrescriptionIssueHistory.PtRegDetailID = 0;
//                }
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                return;
//            }
//            //Cụ thể  DV nào

//            //Gán lại ServiceRecID, PtRegDetailID
//        }

//        private void SetValueTaoThanhToaMoi_CreateNew()
//        {
//            ObjTaoThanhToaMoi = LatestePrecriptions.DeepCopy();
//            //ObjTaoThanhToaMoi.NDay = CheckAppDate();
//            ObjTaoThanhToaMoi.CreatorStaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0);
//            if (CapNhatToaThuoc)
//            {
//                ObjTaoThanhToaMoi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
//            }
//            if (TaoThanhToaMoi)
//            {
//                ObjTaoThanhToaMoi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.EDITTUTOAKHAC;
//            }
//            ObjTaoThanhToaMoi.OriginalPrescriptID = LatestePrecriptions.OriginalPrescriptID;/*Gốc Của Phát Hành Này*/

//            ObjTaoThanhToaMoi.PrescriptionIssueHistory = new PrescriptionIssueHistory();

//            //Gán lại ServiceRecID, PtRegDetailID
//            ObjTaoThanhToaMoi.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;

//            //Cụ thể  DV nào
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {

//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        ObjTaoThanhToaMoi.PrescriptionIssueHistory.PtRegDetailID = Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID;
//                    }
//                    else
//                    {
//                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                        return;
//                    }
//                }
//                else/*Khám VIP, Khám Cho Nội Trú*/
//                {
//                    ObjTaoThanhToaMoi.PrescriptionIssueHistory.PtRegDetailID = 0;
//                }
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                return;
//            }
//            //Cụ thể  DV nào

//            //Gán lại ServiceRecID, PtRegDetailID
//        }


//        private void SetValueToaOfDuocSi()
//        {
//            ObjToaOfDuocSi = LatestePrecriptions.DeepCopy();
//            //ObjToaOfDuocSi.NDay = CheckAppDate();
//            ObjToaOfDuocSi.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
//            ObjToaOfDuocSi.OriginalPrescriptID = LatestePrecriptions.OriginalPrescriptID;/*Gốc Của Phát Hành Này*/

//            ObjToaOfDuocSi.PrescriptionIssueHistory = new PrescriptionIssueHistory();
//            //Gán lại ServiceRecID, PtRegDetailID
//            ObjToaOfDuocSi.ServiceRecID = LatestePrecriptions.ObjDiagnosisTreatment.ServiceRecID;
//            ObjToaOfDuocSi.PrescriptionIssueHistory.PtRegDetailID = LatestePrecriptions.ObjDiagnosisTreatment.PtRegDetailID;

//            //Cụ thể  DV nào  Notes Ny: Cho nay can xem lai ti,hinh nhu ko can cai nay do dsi chi sua toa thuoc
//            //nen chi can lay lai PtRegDetailID cu la dc
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        ObjToaOfDuocSi.PrescriptionIssueHistory.PtRegDetailID = LatestePrecriptions.ObjDiagnosisTreatment.PtRegDetailID;
//                    }
//                    else
//                    {
//                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                        return;
//                    }
//                }
//                else/*Khám VIP, Khám Cho Nội Trú*/
//                {
//                    ObjToaOfDuocSi.PrescriptionIssueHistory.PtRegDetailID = 0;
//                }
//            }
//            //else
//            //{
//            //    MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//            //    return;
//            //}
//            //Cụ thể  DV nào

//            ObjToaOfDuocSi.ModifierStaffID = Globals.LoggedUserAccount.Staff.StaffID;
//            //Gán lại ServiceRecID, PtRegDetailID
//        }

//        private Prescription _ObjTaoThanhToaMoi;
//        public Prescription ObjTaoThanhToaMoi
//        {
//            get
//            {
//                return _ObjTaoThanhToaMoi;
//            }
//            set
//            {
//                if (_ObjTaoThanhToaMoi != value)
//                {
//                    _ObjTaoThanhToaMoi = value;
//                    NotifyOfPropertyChange(() => ObjTaoThanhToaMoi);
//                }
//            }
//        }

//        private Prescription _ObjToaOfDuocSi;
//        public Prescription ObjToaOfDuocSi
//        {
//            get
//            {
//                return _ObjToaOfDuocSi;
//            }
//            set
//            {
//                if (_ObjToaOfDuocSi != value)
//                {
//                    _ObjToaOfDuocSi = value;
//                    NotifyOfPropertyChange(() => ObjToaOfDuocSi);
//                }
//            }
//        }

//        private void Prescriptions_Add()
//        {
//            TaoThanhToaMoi = false;

//            IsEnabledForm = false;

//            IsWaitingTaoThanhToaMoi = true;

//            long IssueID = 0;
//            long PrescriptionID = 0;
//            string OutError = "";

//            IList<PrescriptionIssueHistory> lstPrescriptionIssueHistory = new List<PrescriptionIssueHistory>();
//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    //contract.BeginPrescriptions_Add(Globals.NumberTypePrescriptions_Rule, ObjTaoThanhToaMoi, Globals.DispatchCallback((asyncResult) =>
//                    // Txd 25/05/2014 Replaced ConfigList
//                    contract.BeginPrescriptions_Add((short)Globals.ServerConfigSection.CommonItems.NumberTypePrescriptions_Rule, ObjTaoThanhToaMoi, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            if (contract.EndPrescriptions_Add(out PrescriptionID, out IssueID, out OutError,out lstPrescriptionIssueHistory, asyncResult))
//                            {
//                                ObjTaoThanhToaMoi.PrescriptID = PrescriptionID;
//                                ObjTaoThanhToaMoi.IssueID = IssueID;

//                                PrecriptionsForPrint = ObjTaoThanhToaMoi;
//                                IsEnabledAutoComplete = false;

//                                //bntSaveAsVisibility = Visibility.Collapsed;
//                                //btnUpdateVisibility = Visibility.Collapsed;

//                                IsEnabled = false;
//                                btnSaveAddNewIsEnabled = false;
//                                btnUndoIsEnabled = false;
//                                //bntSaveAsIsEnabled = false;
//                                btnCreateNewIsEnabled = true;
//                                btnEditIsEnabled = true;
//                                btnCreateAndCopyIsEnabled = true;
//                                IsEnabledPrint = true;

//                                btChonChanDoanIsEnabled = false;

//                                MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);

//                                ////phat su kien load lai toa thuoc cuoi
//                                Globals.EventAggregator.Publish(new ReloadDataePrescriptionEvent { });
//                                ////PrescripState = PrescriptionState.EditPrescriptionState;
//                                //Dinh sua
//                                Globals.EventAggregator.Publish(new ReloadDataConsultationEvent { });
//                            }
//                            else
//                            {
//                                //Cộng lại dòng trống
//                                AddNewBlankDrugIntoPrescriptObjectNew();
//                                //Cộng lại dòng trống
//                                if (OutError.Contains("Duplex-Prescriptions_PrescriptionsInDay"))
//                                {
//                                    MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
//                                        + Environment.NewLine + OutError.Replace("Duplex-Prescriptions_PrescriptionsInDay@", "")
//                                        + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                }
//                                else 
//                                {
//                                    MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
//                                }
//                                //switch (OutError)
//                                //{
//                                //    case "Duplex-Prescriptions_PrescriptionsInDay":
//                                //        {
//                                //            MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                //            break;
//                                //        }
//                                //    default:
//                                //        {
//                                //            MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
//                                //            break;
//                                //        }
//                                //}
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsEnabledForm = true;

//                            IsWaitingTaoThanhToaMoi = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }
//        #endregion


//        #region IHandle<ePrescriptionSelectedEvent> Members

//        private Prescription _PrecriptionsBeforeUpdate;
//        public Prescription PrecriptionsBeforeUpdate
//        {
//            get
//            {
//                return _PrecriptionsBeforeUpdate;
//            }
//            set
//            {
//                if (_PrecriptionsBeforeUpdate != value)
//                {
//                    _PrecriptionsBeforeUpdate = value;
//                    NotifyOfPropertyChange(() => PrecriptionsBeforeUpdate);
//                }
//            }
//        }

//        public Staff ObjStaff
//        {
//            get
//            {
//                return Globals.LoggedUserAccount.Staff;
//            }
//        }

//        /*Đọc chi tiết 1 Toa khi Double Click chọn từ danh sách Toa Thuốc*/
//        public void Handle(ePrescriptionDoubleClickEvent message)
//        {
//            if (message != null)
//            {
//                IsEnabled = false;

//                btnSaveAddNewIsEnabled = false;
//                btnUndoIsEnabled = false;
//                btnCreateNewIsEnabled = true;
//                //bntSaveAsIsEnabled = false;
//                IsEnabledPrint = true;


//                LatestePrecriptions = message.SelectedPrescription.DeepCopy();
//                NotifyOfPropertyChange(()=>LatestePrecriptions.PrescriptionDetails);
//                if (CheckToaThuocDuocPhepCapNhat(DuocSi_IsEditingToaThuoc) == false)
//                {
//                    btnEditIsEnabled = false;
//                }
//                else
//                {
//                    btnEditIsEnabled = true;
//                }
//                btnCreateAndCopyIsEnabled = true;

//                if (LatestePrecriptions.NDay > 0)
//                {
//                    chkHasAppointmentValue = true;
//                }
//                else
//                {
//                    chkHasAppointmentValue = false;
//                }

//                //NotifyOfPropertyChange(() => txtDaysAfterIsEnabled);

//                PrecriptionsBeforeUpdate = message.SelectedPrescription.DeepCopy();

//                //HideButton = Visibility.Visible;
//                //GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
//                BackupCurPrescriptionItem();

//                PrecriptionsForPrint = message.SelectedPrescription.DeepCopy();

//                ContentKhungTaiKhamIsEnabled = LatestePrecriptions.IsAllowEditNDay;

//                SoNgayDungThuoc_Root = LatestePrecriptions.NDay;

//            }
//        }

//        public void Handle(SelectListDrugDoubleClickEvent message)
//        {
//            if (message != null)
//            {
//                IsEnabled = true;

//                btnCreateNewIsEnabled = false;

//                btnSaveAddNewIsEnabled = true;
//                btnUndoIsEnabled = true;

//                btnEditIsEnabled = false;
//                btnCreateAndCopyIsEnabled = false;
//                btnUpdateIsEnabled = false;
//                //bntSaveAsIsEnabled = false;
//                btDuocSiEditIsEnabled = false;
//                btnCopyToIsEnabled = false;

//                btChonChanDoanIsEnabled = true;

//                IsEnabledPrint = false;
//                //

//                //App
//                chkHasAppointmentValue = false;
//                //App

//                NewPrecriptions = new Prescription();
//                NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
//                NewPrecriptions.PtRegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;
//                NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
//                NewPrecriptions.NDay = 0;

//                NewPrecriptions.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;

//                NewPrecriptions.ConsultantDoctor = new Staff();

//                NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
//                NewPrecriptions.ObjCreatorStaffID = new Staff();
//                NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
//                NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

//                NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;

//                NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
//                NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;

//                NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();

//                //Cụ thể  DV nào
//                if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                    {
//                        if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                        {
//                            NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID;
//                        }
//                        else
//                        {
//                            MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                            return;
//                        }
//                    }
//                    else/*Khám VIP, Khám Cho Nội Trú*/
//                    {
//                        NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
//                    }
//                }
//                else
//                {
//                    MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                    return;
//                }
//                //Cụ thể  DV nào

//                LatestePrecriptions = NewPrecriptions.DeepCopy();

//                for (int i = 0; i < message.GetDrugForSellVisitorList.Count; i++)
//                {
//                    AddNewBlankDrugIntoPrescriptObjectNew(i, message.GetDrugForSellVisitorList[i]);
//                }

//                AddNewBlankDrugIntoPrescriptObjectNew();
//            }
//        }

//        private void AddNewBlankDrugIntoPrescriptObjectNew(int index, GetDrugForSellVisitor item)
//        {
//            if (LatestePrecriptions == null)
//                LatestePrecriptions = new Prescription();

//            if (LatestePrecriptions.PrescriptionDetails == null)
//                LatestePrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();

//            int i = LatestePrecriptions.PrescriptionDetails.Count + 1;
//            PrescriptionDetail prescriptDObj = new PrescriptionDetail();
//            prescriptDObj.DrugID = 0;
//            prescriptDObj.IsInsurance = null;
//            prescriptDObj.Strength = "";
//            prescriptDObj.Qty = 0;
//            prescriptDObj.MDose = 0;
//            prescriptDObj.ADose = 0;
//            prescriptDObj.EDose = 0;
//            prescriptDObj.NDose = 0;
//            if (IsPatientInsurance())
//            {
//                prescriptDObj.BeOfHIMedicineList = true;
//                prescriptDObj.InsuranceCover = true;
//            }
//            else
//            {
//                prescriptDObj.BeOfHIMedicineList = false;
//                prescriptDObj.InsuranceCover = false;
//            }

//            prescriptDObj.DrugInstructionNotes = "";
//            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
//            {
//                prescriptDObj.PrescriptDetailID = LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].PrescriptDetailID + 1;
//            }
//            else
//            {
//                prescriptDObj.PrescriptDetailID = 0;
//            }
//            prescriptDObj.SelectedDrugForPrescription = item;
//            LatestePrecriptions.PrescriptionDetails.Insert(index, prescriptDObj);
//            NotifyOfPropertyChange(() => LatestePrecriptions.PrescriptionDetails);
//        }
//        #endregion

//        #region printing member

//        public void btnPreview()
//        {
//            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
//            {
//                proAlloc.IssueID = (int)PrecriptionsForPrint.IssueID;
//                proAlloc.eItem = ReportName.CONSULTATION_TOATHUOC;
//            };
//            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
//        }

//        public void btnPrint()
//        {
//            MessageBox.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.A0073_G1_CNangDangHThien));
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            //var t = new Thread(() =>
//            //{
//            //    using (var serviceFactory = new ReportServiceClient())
//            //    {
//            //        var contract = serviceFactory.ServiceInstance;

//            //        contract.BeginGetXRptEPrescriptionInPdfFormat(LatestePrecriptions.PatientID.GetValueOrDefault(),LatestePrecriptions.PrescriptID, Globals.DispatchCallback((asyncResult) =>
//            //        {
//            //            try
//            //            {
//            //                var results = contract.EndGetXRptEPrescriptionInPdfFormat(asyncResult);
//            //                ClientPrintHelper.PrintPdfData(results);
//            //            }
//            //            catch (Exception ex)
//            //            {
//            //                MessageBox.Show(ex.Message);
//            //            }
//            //            finally
//            //            {
//            //                Globals.IsBusy = false;
//            //            }

//            //        }), null);

//            //    }

//            //});
//            //t.Start();
//        }

//        #endregion


//        private bool _btChonChanDoanIsEnabled;
//        public bool btChonChanDoanIsEnabled
//        {
//            get
//            {
//                return _btChonChanDoanIsEnabled;
//            }
//            set
//            {
//                if (_btChonChanDoanIsEnabled != value)
//                {
//                    _btChonChanDoanIsEnabled = value;
//                    NotifyOfPropertyChange(() => btChonChanDoanIsEnabled);
//                }
//            }
//        }


//        public void btChonChanDoan(object sender, RoutedEventArgs e)
//        {
//            Globals.ConsultationIsChildWindow = true;
//            Action<IConsultations> onInitDlg = delegate (IConsultations proAlloc)
//            {
//                proAlloc.IsPopUp = true;
//            };
//            GlobalsNAV.ShowDialog<IConsultations>(onInitDlg);
//        }

//        #region IHandle<RaiseEPrescriptoldEvent> Members

//        //public void Handle(RaiseEPrescriptoldEvent message)
//        //{
//        //    if (message != null)/*chụp 1 chẩn đoán đã chọn từ Poppop chọn chẩn đoán*/
//        //    {
//        //        LatestePrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
//        //        LatestePrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();

//        //        if (LatestePrecriptions.PrescriptionIssueHistory == null)
//        //        {
//        //            LatestePrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();
//        //        }

//        //        LatestePrecriptions.PrescriptionIssueHistory.PtRegDetailID = ObjDiagnosisTreatment_Current.PtRegDetailID;

//        //    }
//        //}

//        #endregion

//        #region IHandle<ReloadDataePrescriptionEvent> Members

//        public void Handle(ReloadDataePrescriptionEvent message)
//        {
//            if (message != null)
//            {
//                if (Globals.PatientAllDetails.PatientInfo != null)
//                {
//                    GetLatestPrescriptionByPtID(Globals.PatientAllDetails.PatientInfo.PatientID);
//                }
//            }
//        }

//        #endregion

//        #region Schedule
//        private int _IndexRow;
//        public int IndexRow
//        {
//            get { return _IndexRow; }
//            set
//            {
//                if (_IndexRow != value)
//                {
//                    _IndexRow = value;
//                    NotifyOfPropertyChange(() => IndexRow);
//                }
//            }
//        }

//        public void hplEditSchedules_Click(Object pSelectedItem)
//        {
//            SelectedPrescriptionDetail = pSelectedItem as PrescriptionDetail;
//            //if (!SelectedPrescriptionDetail.isComboDrugType)
//            //{
//            //    MessageBox.Show("Thuốc này ngoài danh mục, không được phép chỉnh sửa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            //    return;
//            //}
//            if (!SelectedPrescriptionDetail.HasSchedules)
//            {
//                MessageBox.Show(eHCMSResources.A0086_G1_Msg_InfoUongThuocTheoLich, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            hplEditSchedules(SelectedPrescriptionDetail);
//        }

//        public void hplEditSchedules(PrescriptionDetail pSelectedItem)
//        {
//            if (pSelectedItem == null)
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            //if (SelectedPrescriptionDetail.IsDrugNotInCat == false)
//            {
//                if (CheckValidationEditor1Row(pSelectedItem))
//                {
//                    IndexRow = grdPrescription.SelectedIndex;

//                    Action<IPrescriptionDetailSchedulesNew> onInitDlg = delegate (IPrescriptionDetailSchedulesNew typeInfo)
//                    {
//                        typeInfo.ObjPrescriptionDetail = pSelectedItem;

//                        typeInfo.ModeForm = LatestePrecriptions.IssueID > 0 ? 1 : 0;/*Update*//*AddNew*/

//                        typeInfo.IsMaxDay = typeInfo.ObjPrescriptionDetail.SelectedDrugForPrescription.DayRpts == 0
//                            || typeInfo.ObjPrescriptionDetail.SelectedDrugForPrescription.MaxDayPrescribed > 0 ? true : false;


//                        //typeInfo.NDay = typeInfo.IsMaxDay ? Convert.ToInt32(typeInfo.ObjPrescriptionDetail.DayRpts)
//                        //    : LatestePrecriptions.NDay > 0 ? LatestePrecriptions.NDay.Value : Convert.ToInt32(typeInfo.ObjPrescriptionDetail.DayRpts);
//                        typeInfo.NDay = Convert.ToInt32(typeInfo.ObjPrescriptionDetail.DayRpts + typeInfo.ObjPrescriptionDetail.DayExtended);

//                        typeInfo.ObjPrescriptionDetailSchedules_ByPrescriptDetailID = pSelectedItem.ObjPrescriptionDetailSchedules;

//                        typeInfo.Initialize();
//                    };
//                    GlobalsNAV.ShowDialog<IPrescriptionDetailSchedulesNew>(onInitDlg);
//                }
//            }
//        }

//        public void Handle(SendPrescriptionDetailSchedulesEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int> message)
//        {
//            if (message != null)
//            {
//                //if (message.ModeForm == 1)/*Update*/
//                {
//                    if (SelectedPrescriptionDetail == null)
//                    {
//                        SelectedPrescriptionDetail = new PrescriptionDetail();
//                    }

//                    //Chọn Lịch thì gán bên ngoài = 0
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].MDose = 0;
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].ADose = 0;
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].NDose = 0;
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].EDose = 0;
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].dosage = 0;

//                    LatestePrecriptions.PrescriptionDetails[IndexRow].MDoseStr = "0";
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].ADoseStr = "0";
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].NDoseStr = "0";
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].EDoseStr = "0";
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].dosageStr = "0";
//                    if (IsPatientInsurance())
//                    {
//                        if (IsBenhNhanNoiTru() == false)
//                        {
//                            if (message.SoNgayDung > xNgayBHToiDa_NgoaiTru)
//                            {
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayRpts = xNgayBHToiDa_NgoaiTru;
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayExtended = message.SoNgayDung - xNgayBHToiDa_NgoaiTru;
//                            }
//                            else
//                            {
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayRpts = message.SoNgayDung;
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayExtended = 0;
//                            }
//                        }
//                        else
//                        {
//                            if (message.SoNgayDung > xNgayBHToiDa_NoiTru)
//                            {
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayRpts = xNgayBHToiDa_NoiTru;
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayExtended = message.SoNgayDung - xNgayBHToiDa_NoiTru;
//                            }
//                            else
//                            {
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayRpts = message.SoNgayDung;
//                                LatestePrecriptions.PrescriptionDetails[IndexRow].DayExtended = 0;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        LatestePrecriptions.PrescriptionDetails[IndexRow].DayRpts = message.SoNgayDung;
//                        LatestePrecriptions.PrescriptionDetails[IndexRow].DayExtended = 0;
//                    }

//                    LatestePrecriptions.PrescriptionDetails[IndexRow].Qty = message.TongThuoc;
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].DrugInstructionNotes = message.GhiChu;
//                    //Chọn Lịch thì gán bên ngoài = 0

//                    //SelectedPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
//                    //SelectedPrescriptionDetail.HasSchedules = message.HasSchedule;

//                    LatestePrecriptions.PrescriptionDetails[IndexRow].ObjPrescriptionDetailSchedules = message.Data;
//                    LatestePrecriptions.PrescriptionDetails[IndexRow].V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;

//                }

//            }
//        }

//        public void Handle(SendPrescriptionDetailSchedulesFormEvent<ObservableCollection<PrescriptionDetailSchedules>, bool, double, double, string, int> message)
//        {
//            if (message != null)
//            {
//                //if (message.ModeForm == 1)/*Update*/
//                {
//                    if (tempPrescriptionDetail == null)
//                    {
//                        tempPrescriptionDetail = new PrescriptionDetail();
//                    }

//                    //Chọn Lịch thì gán bên ngoài = 0
//                    tempPrescriptionDetail.MDose = 0;
//                    tempPrescriptionDetail.ADose = 0;
//                    tempPrescriptionDetail.NDose = 0;
//                    tempPrescriptionDetail.EDose = 0;
//                    tempPrescriptionDetail.dosage = 0;

//                    tempPrescriptionDetail.MDoseStr = "0";
//                    tempPrescriptionDetail.ADoseStr = "0";
//                    tempPrescriptionDetail.NDoseStr = "0";
//                    tempPrescriptionDetail.EDoseStr = "0";
//                    tempPrescriptionDetail.dosageStr = "0";
//                    if (IsPatientInsurance())
//                    {
//                        if (IsBenhNhanNoiTru() == false)
//                        {
//                            if (message.SoNgayDung > xNgayBHToiDa_NgoaiTru)
//                            {
//                                tempPrescriptionDetail.DayRpts = xNgayBHToiDa_NgoaiTru;
//                                tempPrescriptionDetail.DayExtended = message.SoNgayDung - xNgayBHToiDa_NgoaiTru;
//                            }
//                            else
//                            {
//                                tempPrescriptionDetail.DayRpts = message.SoNgayDung;
//                                tempPrescriptionDetail.DayExtended = 0;
//                            }
//                        }
//                        else
//                        {
//                            if (message.SoNgayDung > xNgayBHToiDa_NoiTru)
//                            {
//                                tempPrescriptionDetail.DayRpts = xNgayBHToiDa_NoiTru;
//                                tempPrescriptionDetail.DayExtended = message.SoNgayDung - xNgayBHToiDa_NoiTru;
//                            }
//                            else
//                            {
//                                tempPrescriptionDetail.DayRpts = message.SoNgayDung;
//                                tempPrescriptionDetail.DayExtended = 0;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        tempPrescriptionDetail.DayRpts = message.SoNgayDung;
//                        tempPrescriptionDetail.DayExtended = 0;
//                    }

//                    tempPrescriptionDetail.Qty = message.TongThuoc;
//                    tempPrescriptionDetail.DrugInstructionNotes = message.GhiChu;
//                    //Chọn Lịch thì gán bên ngoài = 0

//                    //tempPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
//                    //tempPrescriptionDetail.HasSchedules = message.HasSchedule;

//                    tempPrescriptionDetail.ObjPrescriptionDetailSchedules = message.Data;
//                    tempPrescriptionDetail.V_DrugType = (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN;
//                }
//            }
//        }

//        AxDataGridNy grdPrescription;
//        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
//        {
//            grdPrescription = sender as AxDataGridNy;
//        }

//        #endregion


//        private bool CheckThuocHopLe()
//        {
//            StringBuilder sb = new StringBuilder();

//            bool Result = true;

//            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
//            {
//                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (item == LatestePrecriptions.PrescriptionDetails.LastOrDefault())
//                    {
//                        continue;
//                    }
//                    if (item.SelectedDrugForPrescription ==null
//                        || item.SelectedDrugForPrescription == null
//                        || item.SelectedDrugForPrescription.BrandName == null
//                        || item.SelectedDrugForPrescription.BrandName == "")
//                    {
//                        sb.AppendLine(string.Format(eHCMSResources.Z0908_G1_ThuocDong0KgHopLe, (item.Index + 1).ToString()));
//                        Result = false;
//                        continue;
//                    }
//                    if (!item.IsDrugNotInCat 
//                        && item.SelectedDrugForPrescription.DrugID<1)
//                    {
//                        sb.AppendLine(string.Format(eHCMSResources.Z0908_G1_ThuocDong0KgHopLe, (item.Index + 1).ToString()));
//                        Result = false;
//                        continue;
//                    }
//                    if (item.SelectedDrugForPrescription != null && item.SelectedDrugForPrescription.BrandName != null)
//                    {
//                        if (item.HasSchedules)//Có Lịch mà bên ngoài sáng chưa chiều tối còn có thì báo lỗi
//                        {
//                            if (item.MDose > 0 ||
//                                item.ADose > 0 ||
//                                item.EDose > 0 ||
//                                item.NDose > 0)
//                            {
//                                sb.AppendLine(string.Format(eHCMSResources.Z0909_G1_Thuoc0DaCoChiDinhLich, item.SelectedDrugForPrescription.BrandName.Trim()));
//                                item.dosage = 0;
//                                item.MDose = 0;
//                                item.ADose = 0;
//                                item.EDose = 0;
//                                item.NDose = 0;
//                                Result = false;
//                            }
//                        }
//                        else//Không Lịch
//                        {
//                            if (item.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
//                            {
//                                if (item.MDose == 0 &&
//                                item.ADose == 0 &&
//                                item.EDose == 0 &&
//                                item.NDose == 0)
//                                {
//                                    sb.AppendLine(string.Format(eHCMSResources.Z0910_G1_Thuoc0SangTruaChieuToi, item.SelectedDrugForPrescription.BrandName.Trim()));
//                                    Result = false;
//                                }

//                                if (item.DayRpts <= 0)
//                                {
//                                    sb.AppendLine(string.Format(eHCMSResources.Z0911_G1_Thuoc0NgDungLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
//                                    Result = false;
//                                }

//                                //if (item.IsDrugNotInCat == false)
//                                {
//                                    if (item.SelectedDrugForPrescription.MaxDayPrescribed != null 
//                                        && item.SelectedDrugForPrescription.MaxDayPrescribed > 0 
//                                        && item.SelectedDrugForPrescription.MaxDayPrescribed < (item.DayExtended + item.DayRpts))
//                                    {
//                                        sb.AppendLine(string.Format(eHCMSResources.Z0912_G1_Thuoc0NgDungVuotSoNgToiDa, item.SelectedDrugForPrescription.BrandName.Trim(), item.SelectedDrugForPrescription.MaxDayPrescribed));
//                                        item.DayExtended = (double)item.SelectedDrugForPrescription.MaxDayPrescribed - item.DayRpts;
//                                        Result = false;
//                                    }
//                                }

//                                if (CheckQtyLessThanQtyAutoCalc(item))
//                                {
//                                    sb.AppendLine(string.Format(eHCMSResources.Z0914_G1_Thuoc0CanKTraLai, item.SelectedDrugForPrescription.BrandName.Trim()));
//                                    Result = false;
//                                }
                                
//                            }
//                            else 
//                            {
                            
//                            }

//                            //if (item.IsDrugNotInCat == false)
//                            {
//                                if (item.Qty <= 0)
//                                {
//                                    sb.AppendLine(string.Format(eHCMSResources.Z1057_G1_ThuocSLgLonHon0, item.SelectedDrugForPrescription.BrandName.Trim()));
//                                    Result = false;
//                                }
//                            }

//                        }
//                    }
//                }
//                if (Result == false)
//                {
//                    MessageBox.Show(sb.ToString() + Environment.NewLine + eHCMSResources.I0945_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return false;
//                }
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return false;
//            }
//            return true;
//        }

//        private bool CheckQtyLessThanQtyAutoCalc(PrescriptionDetail Objtmp)
//        {
//            if (Objtmp != null && Objtmp.IsDrugNotInCat == false)
//            {
//                if (Objtmp.HasSchedules == false)
//                {
//                    Nullable<float> TongThuoc = 0;
//                    float Tong = 0;

//                    if (Objtmp != null)
//                    {
//                        TongThuoc = Objtmp.MDose + Objtmp.ADose.GetValueOrDefault() + Objtmp.NDose.GetValueOrDefault() + Objtmp.EDose.GetValueOrDefault();
//                        Tong = (float)(TongThuoc.Value * (Objtmp.DayRpts + Objtmp.DayExtended) * Objtmp.SelectedDrugForPrescription.UnitVolume) / (float)Objtmp.SelectedDrugForPrescription.DispenseVolume;

//                        if (Objtmp.Qty < Math.Ceiling(Tong))
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            return false;
//        }


//        private bool ErrCheckChongChiDinh()
//        {
//            StringBuilder sb = new StringBuilder();

//            bool Result = false;

//            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
//            {
//                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (item.SelectedDrugForPrescription != null)
//                    {
//                        string err = "";

//                        if (CheckChongChiDinh1Drug(item.DrugID.Value, out err))
//                        {
//                            sb.AppendLine(err);
//                            Result = true;
//                        }
//                    }
//                }
//            }
//            if (Result)
//            {
//                MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            }
//            return Result;
//        }

//        private bool CheckChongChiDinh1Drug(long DrugID, out string msg)
//        {
//            msg = "";

//            if (Globals.allContraIndicatorDrugsRelToMedCond == null)
//                return false;

//            foreach (var MedCond in PtMedCond)
//            {
//                long mcType = (long)MedCond.RefMedicalCondition.MedContraTypeID;
//                foreach (var cDR in Globals.allContraIndicatorDrugsRelToMedCond)
//                {
//                    if (cDR.RefMedicalConditionType.MedContraTypeID == mcType && cDR.DrugID == DrugID)
//                    {
//                        msg = string.Format(eHCMSResources.Z1498_G1_Thuoc0CCDVoiDKienBenh1, cDR.RefGenericDrugDetail.BrandName.Trim(), cDR.RefMedicalConditionType.MedContraIndicationType.Trim());
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }


//        #region CheckThuocBiTrungTrongToa
//        private bool CheckThuocBiTrungTrongToa()
//        {
//            foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
//            {
//                if (CountDrug(prescriptionDetail.DrugID) >= 2)
//                {
//                    MessageBox.Show(string.Format("{0} {1}", eHCMSResources.K0072_G1_TrungThuocTrongToa, eHCMSResources.Z0889_G1_KTraLai), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return true;
//                }
//            }
//            return false;
//        }
//        private int CountDrug(Nullable<Int64> DrugID)
//        {
//            int d = 0;
//            if (DrugID != null && DrugID > 0)
//            {
//                foreach (var prescriptionDetail in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (prescriptionDetail.DrugID == DrugID)
//                    {
//                        d++;
//                    }
//                }
//            }
//            return d;
//        }
//        #endregion


//        #region "Dược Sĩ Sửa Lại Toa Của Bác Sĩ"

//        public void Handle(DuocSi_EditingToaThuocEvent message)
//        {
//            if (message != null)
//            {
//                GetPrescriptionTypes_LoadList();

//                IsEnabledForm = true;

//                //Các nút DS không cần nhìn thấy
//                btnCreateNewVisibility = false;
//                btnSaveAddNewVisibility = false;
//                btnUpdateVisibility = false;
//                bntSaveAsVisibility = false;
//                btnCopyToVisible = false;
//                btChonChanDoanVisibility = false;
//                //Các nút DS không cần nhìn thấy


//                IsEnabled = false;
//                btnEditIsEnabled = true;
//                btnCreateAndCopyIsEnabled = true;
//                IsEnabledPrint = true;


//                ToaChonDeSua = message.SelectedPrescription.DeepCopy();

//                PrecriptionsForPrint = ToaChonDeSua;

//                InitForDuocSi_EditingToaThuoc();

//                LatestePrecriptions = message.SelectedPrescription;

//                //HideButton = Visibility.Visible;
//                GetPrescriptionDetailsByPrescriptID(LatestePrecriptions.PrescriptID);
//            }
//        }

//        private void InitForDuocSi_EditingToaThuoc()
//        {
//            //Load UC Dị Ứng/Cảnh Báo
//            var uc1 = Globals.GetViewModel<IAllergiesWarning_ByPatientID>();
//            UCAllergiesWarningByPatientID = uc1;
//            this.ActivateItem(uc1);
//            //Load UC Dị Ứng/Cảnh Báo
//        }

//        private Prescription _ToaChonDeSua;
//        public Prescription ToaChonDeSua
//        {
//            get { return _ToaChonDeSua; }
//            set
//            {
//                if (_ToaChonDeSua != value)
//                {
//                    _ToaChonDeSua = value;
//                    NotifyOfPropertyChange("ToaChonDeSua");
//                }
//            }
//        }


//        public bool CanbtDuocSiEdit
//        {
//            get
//            {
//                return (!IsWaitingSaveDuocSiEdit);
//            }
//        }
//        public void btDuocSiEdit()
//        {
//            if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
//            {
//                MessageBox.Show(eHCMSResources.Z0815_G1_ChiDinhCDoan, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }

//            if (LatestePrecriptions.V_PrescriptionType == null)
//            {
//                MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }

//            if (grdPrescription == null)
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckAllThuocBiDiUng())
//            {
//                return;
//            }

//            if (ErrCheckChongChiDinh())
//            {
//                return;
//            }

//            if (grdPrescription.IsValid == false)
//            {
//                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckThuocHopLe() == false)
//            {
//                return;
//            }

//            if (CheckHoiChan() == false)
//            {
//                return;
//            }

//            if (CheckThuocBiTrungTrongToa())
//            {
//                return;
//            }

//            //if (CheckHasCheckHasAppointment() == false)
//            //{
//            //    return;
//            //}

//            //Bỏ dòng cuối cùng rỗng của Toa
//            RemoveLastRowPrecriptionDetail();

//            DuocSi_IsEditingToaThuoc = true;


//            if (ConfirmSoLuongNotEnoughBeforeSave())
//            {
//                if (ToaChonDeSua.ModifierStaffID == null) /*DS Đang Sửa Toa BS*/
//                {
//                    SetValueToaOfDuocSi();
//                    //Prescriptions_DuocSiEdit();
//                    //Dinh sua o day
//                    Coroutine.BeginExecute(CoroutinePrescriptions_DuocSiEdit());
//                }
//                else /*DS Sửa Toa DS*/
//                {
//                    SetValueToaOfDuocSi();
//                    //Prescriptions_DuocSiEditDuocSi();
//                    Coroutine.BeginExecute(CoroutinePrescriptions_DuocSiEditDuocSi());
//                }
//            }
//            else
//            {
//                //Cộng lại dòng trống
//                AddNewBlankDrugIntoPrescriptObjectNew();
//                //Cộng lại dòng trống
//            }
//        }

//        private void Prescriptions_DuocSiEdit()
//        {
//            IsEnabledForm = false;

//            IsWaitingSaveDuocSiEdit = true;

//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            long PrescriptionID = 0;
//            long IssueID = 0;
//            string OutError = "";



//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginPrescriptions_DuocSiEdit(ObjToaOfDuocSi, ToaChonDeSua, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            if (contract.EndPrescriptions_DuocSiEdit(out PrescriptionID, out IssueID, out OutError, asyncResult))
//                            {
//                                ObjToaOfDuocSi.IssueID = IssueID;
//                                ObjToaOfDuocSi.PrescriptID = PrescriptionID;

//                                PrecriptionsForPrint = ObjToaOfDuocSi;
//                                ToaChonDeSua = ObjToaOfDuocSi;
//                                LatestePrecriptions = ObjToaOfDuocSi;

//                                IsEnabledAutoComplete = false;

//                                IsEnabled = false;

//                                btDuocSiEditIsEnabled = false;
//                                btnUndoIsEnabled = false;
//                                btnEditIsEnabled = true;
//                                btnCreateAndCopyIsEnabled = true;
//                                IsEnabledPrint = true;

//                                MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);

//                                //phat su kien load lai toa thuoc cuoi
//                                if (DuocSi_IsEditingToaThuoc)
//                                {
//                                    Globals.EventAggregator.Publish(new ReLoadToaOfDuocSiEditEvent<Prescription> { Prescription = ObjToaOfDuocSi });
//                                    TryClose();
//                                }

//                            }
//                            else
//                            {
//                                //Cộng lại dòng trống
//                                AddNewBlankDrugIntoPrescriptObjectNew();
//                                //Cộng lại dòng trống
//                                if (OutError.Contains("Duplex-Prescriptions_PrescriptionsInDay"))
//                                {
//                                    MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
//                                        + Environment.NewLine + OutError.Replace("Duplex-Prescriptions_PrescriptionsInDay@", "")
//                                        + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                }
//                                else
//                                {
//                                    MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
//                                }
                                
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {

//                            IsEnabledForm = true;

//                            IsWaitingSaveDuocSiEdit = false;


//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private void Prescriptions_DuocSiEditDuocSi()
//        {

//            IsEnabledForm = false;

//            IsWaitingSaveDuocSiEdit = true;


//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            long PrescriptionID = 0;
//            long IssueID = 0;
//            string OutError = "";


//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginPrescriptions_DuocSiEditDuocSi(ObjToaOfDuocSi, ToaChonDeSua, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            if (contract.EndPrescriptions_DuocSiEditDuocSi(out PrescriptionID, out IssueID, out OutError, asyncResult))
//                            {
//                                ObjToaOfDuocSi.IssueID = IssueID;
//                                ObjToaOfDuocSi.PrescriptID = PrescriptionID;

//                                PrecriptionsForPrint = ObjToaOfDuocSi;
//                                LatestePrecriptions = ObjToaOfDuocSi;
//                                ToaChonDeSua = ObjToaOfDuocSi;

//                                IsEnabledAutoComplete = false;

//                                IsEnabled = false;
//                                btDuocSiEditIsEnabled = false;
//                                btnUndoIsEnabled = false;
//                                btnEditIsEnabled = true;
//                                btnCreateAndCopyIsEnabled = true;
//                                IsEnabledPrint = true;

//                                MessageBox.Show(eHCMSResources.K0175_G1_ToaThuocDaLuuOk);

//                                //phat su kien load lai toa thuoc cuoi
//                                if (DuocSi_IsEditingToaThuoc)
//                                {
//                                    Globals.EventAggregator.Publish(new ReLoadToaOfDuocSiEditEvent<Prescription> { Prescription = ObjToaOfDuocSi });
//                                    TryClose();
//                                }
//                            }
//                            else
//                            {

//                                //Cộng lại dòng trống
//                                AddNewBlankDrugIntoPrescriptObjectNew();
//                                //Cộng lại dòng trống

//                                if (OutError.Contains("Duplex-Prescriptions_PrescriptionsInDay"))
//                                {
//                                    MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
//                                        + Environment.NewLine + OutError.Replace("Duplex-Prescriptions_PrescriptionsInDay@", "")
//                                        + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                                }
//                                else
//                                {
//                                    MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsEnabledForm = true;

//                            IsWaitingSaveDuocSiEdit = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        #endregion

//        public void GetAllContrainIndicatorDrugs()
//        {
//            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            IsWaitingGetAllContrainIndicatorDrugs = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new PharmacyDrugServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginGetAllContrainIndicatorDrugs(Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndGetAllContrainIndicatorDrugs(asyncResult);
//                            if (results != null)
//                            {
//                                if (Globals.allContraIndicatorDrugsRelToMedCond == null)
//                                {
//                                    Globals.allContraIndicatorDrugsRelToMedCond = new ObservableCollection<ContraIndicatorDrugsRelToMedCond>();
//                                }
//                                else
//                                {
//                                    Globals.allContraIndicatorDrugsRelToMedCond.Clear();
//                                }
//                                foreach (var p in results)
//                                {
//                                    Globals.allContraIndicatorDrugsRelToMedCond.Add(p);
//                                }
//                                NotifyOfPropertyChange(() => Globals.allContraIndicatorDrugsRelToMedCond);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            IsWaitingGetAllContrainIndicatorDrugs = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        private bool CheckSoLuongThuocDeBan(out string msg)
//        {
//            StringBuilder sb = new StringBuilder();

//            msg = "";

//            bool Result = true;

//            if (LatestePrecriptions.PrescriptionDetails.Count > 0)
//            {
//                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if (item.SelectedDrugForPrescription != null)
//                    {
//                        if (item.IsDrugNotInCat == false)
//                        {
//                            if (item.SelectedDrugForPrescription.Remaining < item.Qty)
//                            {
//                                sb.AppendLine(string.Format(eHCMSResources.Z1073_G1_ThuocSLgTrongKhoKgDuBan, item.SelectedDrugForPrescription.BrandName.Trim()));
//                                Result = false;
//                            }
//                        }
//                    }
//                }
//            }

//            msg = sb.ToString();

//            return Result;
//        }


//        private bool ConfirmSoLuongNotEnoughBeforeSave()
//        {
//            string msg = "";
//            if (CheckSoLuongThuocDeBan(out msg) == false)
//            {
//                if (MessageBox.Show(msg + Environment.NewLine + eHCMSResources.I0943_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                {
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                return true;
//            }
//        }


//        //#region KeyUp khi ra toa các giá trị
//        //public void tbdosage_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.dosageStr = "0";
//        //                Objtmp.dosage = 0;

//        //                ((System.Windows.Controls.TextBox)(sender)).Text = "0";//show ra giao diện

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbMDose_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.MDoseStr = "0";
//        //                Objtmp.MDose = 0;

//        //                ((System.Windows.Controls.TextBox)(sender)).Text = "0";//show ra giao diện

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbADose_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.ADoseStr = "0";
//        //                Objtmp.ADose = 0;

//        //                ((System.Windows.Controls.TextBox)(sender)).Text = "0";//show ra giao diện

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbEDose_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.EDoseStr = "0";
//        //                Objtmp.EDose = 0;

//        //                ((System.Windows.Controls.TextBox)(sender)).Text = "0";//show ra giao diện

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}
//        //public void tbNDose_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.EDoseStr = "0";
//        //                Objtmp.NDose = 0;

//        //                ((System.Windows.Controls.TextBox)(sender)).Text = "0";//show ra giao diện

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}

//        //public void tbNgayDung_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        PrescriptionDetail ObjtmpCopy = ObjectCopier.DeepCopy(Objtmp);
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.DayRpts = ObjtmpCopy.DayRpts;

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}

//        //public void tbNgayDungExtended_KeyUp(object sender, KeyEventArgs e)
//        //{
//        //    AxTextBox Ctr = (sender as AxTextBox);

//        //    if (Ctr != null)
//        //    {
//        //        PrescriptionDetail Objtmp = (Ctr.DataContext) as PrescriptionDetail;
//        //        PrescriptionDetail ObjtmpCopy = ObjectCopier.DeepCopy(Objtmp);
//        //        if (Objtmp != null)
//        //        {
//        //            if (Objtmp.HasSchedules == true)
//        //            {
//        //                Objtmp.DayExtended = ObjtmpCopy.DayExtended;

//        //                ((System.Windows.Controls.TextBox)(sender)).Text = "0";//show ra giao diện

//        //                MessageBox.Show(string.Format("'{0}' {1}!", Objtmp.SelectedDrugForPrescription.BrandName.Trim(), eHCMSResources.K0082_G1_DaCoCDinhUongTheoTuan));
//        //            }
//        //        }
//        //    }
//        //}
//        //#endregion


//        #region "Ra Toa Mới"

//        private bool _btnUndoIsEnabled;
//        public bool btnUndoIsEnabled
//        {
//            get
//            {
//                return _btnUndoIsEnabled;
//            }
//            set
//            {
//                if (_btnUndoIsEnabled != value)
//                {
//                    _btnUndoIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnUndoIsEnabled);
//                }
//            }
//        }

//        private bool _btnCreateNewIsEnabled;
//        public bool btnCreateNewIsEnabled
//        {
//            get
//            {
//                return _btnCreateNewIsEnabled;
//            }
//            set
//            {
//                if (_btnCreateNewIsEnabled != value)
//                {
//                    _btnCreateNewIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnCreateNewIsEnabled);
//                }
//            }
//        }

//        private bool _btnSaveAddNewIsEnabled = true;
//        public bool btnSaveAddNewIsEnabled
//        {
//            get
//            {
//                return _btnSaveAddNewIsEnabled;
//            }
//            set
//            {
//                if (_btnSaveAddNewIsEnabled != value)
//                {
//                    _btnSaveAddNewIsEnabled = value;
//                    NotifyOfPropertyChange(() => btnSaveAddNewIsEnabled);
//                }
//            }
//        }

//        private void SetToaBaoHiem_KhongBaoHiem()
//        {
//            if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PatientInfo != null)
//            {
//                if (Globals.PatientAllDetails.PatientInfo.CurrentHealthInsurance != null)
//                {
//                    NewPrecriptions.HICardNo = Globals.PatientAllDetails.PatientInfo.CurrentHealthInsurance.HICardNo;
//                }
//                //else
//                //{
//                //    NewPrecriptions.HICardNo = "";
//                //}
//            }
//        }

//        public void InitDataGridNew()
//        {
//            if (NewPrecriptions == null)
//            {
//                NewPrecriptions = new Prescription();
//            }
//            if (NewPrecriptions.PrescriptionDetails == null || NewPrecriptions.PrescriptionDetails.Count == 0)
//            {
//                AddNewBlankDrugIntoPrescriptObjectNew();
//            }
//        }

//        private bool _HasDiagnosis = false;
//        public bool HasDiagnosis
//        {
//            get
//            {
//                return _HasDiagnosis;
//            }
//            set
//            {
//                if (_HasDiagnosis != value)
//                {
//                    _HasDiagnosis = value;
//                    NotifyOfPropertyChange(() => HasDiagnosis);
//                }
//            }
//        }

//        private void DefaultValueForNewPrecription()
//        {
//            NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
//            NewPrecriptions.ObjCreatorStaffID = new Staff();
//            NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
//            NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

//            if (HasDiagnosis)
//            {
//                if (ObjDiagnosisTreatment_Current != null)
//                {
//                    NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;
//                }
//            }


//            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;

//            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();

//            //Cụ thể  DV nào
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID;
//                    }
//                    else
//                    {
//                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                        return;
//                    }
//                }
//                else/*Khám VIP, Khám Cho Nội Trú*/
//                {
//                    NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
//                }
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                return;
//            }
//            //Cụ thể  DV nào

//        }

//        private Prescription _NewPrecriptions;
//        public Prescription NewPrecriptions
//        {
//            get
//            {
//                return _NewPrecriptions;
//            }
//            set
//            {
//                if (_NewPrecriptions != value)
//                {
//                    _NewPrecriptions = value;
//                    NotifyOfPropertyChange(() => NewPrecriptions);
//                }
//            }
//        }

//        private void InitialNewPrescription()
//        {
//            try
//            {
//                NewPrecriptions = new Prescription();

//                if (NewPrecriptions.PrescriptionDetails == null)
//                {
//                    NewPrecriptions.PrescriptionDetails = new ObservableCollection<PrescriptionDetail>();
//                }

//                NewPrecriptions.PrescriptionDetails.Clear();

//                NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
//                NewPrecriptions.PtRegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;
//                NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
//                NewPrecriptions.NDay = 0;

//                NewPrecriptions.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;

//                NewPrecriptions.ConsultantDoctor = new Staff();

//                NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;

//                DefaultValueForNewPrecription();

//                InitDataGridNew();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.ToString());
//            }
//        }

//        private bool _IsWaitingGetDiagnosisTreatmentByPtID;
//        public bool IsWaitingGetDiagnosisTreatmentByPtID
//        {
//            get { return _IsWaitingGetDiagnosisTreatmentByPtID; }
//            set
//            {
//                if (_IsWaitingGetDiagnosisTreatmentByPtID != value)
//                {
//                    _IsWaitingGetDiagnosisTreatmentByPtID = value;
//                    NotifyOfPropertyChange(() => IsWaitingGetDiagnosisTreatmentByPtID);
//                    NotifyWhenBusy();
//                }
//            }
//        }

//        //private DiagnosisTreatment _ObjGetDiagnosisTreatmentByPtID;
//        //public DiagnosisTreatment ObjGetDiagnosisTreatmentByPtID
//        //{
//        //    get
//        //    {
//        //        return _ObjGetDiagnosisTreatmentByPtID;
//        //    }
//        //    set
//        //    {
//        //        if (_ObjGetDiagnosisTreatmentByPtID != value)
//        //        {
//        //            _ObjGetDiagnosisTreatmentByPtID = value;
//        //            NotifyOfPropertyChange(() => ObjGetDiagnosisTreatmentByPtID);
//        //        }
//        //    }
//        //}

//        /*opt:-- 0: Query by PatientID, 1: Query by PtRegistrationID, 2: Query By NationalMedicalCode  */
//        private void GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType)
//        {
//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0536_G1_CDoanCuoi) });

//            IsWaitingGetDiagnosisTreatmentByPtID = true;

//            var t = new Thread(() =>
//            {
//                using (var serviceFactory = new ePMRsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    //HPT 03/10/2016: thêm tham số ServiceRecID để lấy chẩn đoán cho phiếu chỉ định đúng với dịch vụ khám bệnh (đa khoa) mà hàm này dùng chung nên ở những chỗ ngoài màn hình phiếu chỉ định cận lâm sàng, truyền tham số ServiceRecID = null
//                    contract.BeginGetDiagnosisTreatmentByPtID(patientID, PtRegistrationID, null, opt, true, V_RegistrationType, null,Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndGetDiagnosisTreatmentByPtID(asyncResult);

//                            if (results != null && results.Count > 0)
//                            {
//                                ObjDiagnosisTreatment_Current = results.ToObservableCollection()[0];

//                                Globals.ObjGetDiagnosisTreatmentByPtID = ObjectCopier.DeepCopy(ObjDiagnosisTreatment_Current);

//                                HasDiagnosis = true;

//                                GetPrescriptionTypes();
//                            }
//                            else
//                            {
//                                IsEnabledForm = false;

//                                HasDiagnosis = false;
//                                ObjDiagnosisTreatment_Current = new DiagnosisTreatment();

//                                btChonChanDoanIsEnabled = false;
//                                btnSaveAddNewIsEnabled = false;
//                                IsEnabledPrint = false;

//                                MessageBox.Show(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                            }

//                        }
//                        catch (Exception ex)
//                        {
//                            MessageBox.Show(ex.Message);
//                        }
//                        finally
//                        {
//                            IsWaitingGetDiagnosisTreatmentByPtID = false;
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        public void btnSaveAddNew()
//        {
//            if (IsPhatHanhLai)
//            {
//                btnUpdate();
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(LatestePrecriptions.Diagnosis))
//                {
//                    MessageBox.Show(eHCMSResources.A0289_G1_Msg_InfoCDinhCDDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                    return;
//                }

//                if (LatestePrecriptions.V_PrescriptionType == null)
//                {
//                    MessageBox.Show(eHCMSResources.A0333_G1_Msg_InfoChonLoaiToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                    return;
//                }


//                if (grdPrescription == null)
//                {
//                    MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.A0290_G1_Msg_LuuToaThuoc, MessageBoxButton.OK);
//                    return;
//                }

//                if (CheckAllThuocBiDiUng())
//                {
//                    return;
//                }

//                if (ErrCheckChongChiDinh())
//                    return;

//                if (grdPrescription.IsValid == false)
//                {
//                    MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                    return;
//                }

//                if (CheckThuocHopLe() == false)
//                    return;

//                if (CheckHoiChan() == false)
//                    return;

//                if (CheckThuocBiTrungTrongToa())
//                    return;

//                //Bỏ dòng cuối cùng rỗng của Toa
//                RemoveLastRowPrecriptionDetail();

//                UpdateDefaultValueForNewPrecription();

//                if (ConfirmSoLuongNotEnoughBeforeSave())
//                {
//                    SetValueTaoThanhToaMoi_CreateNew();
//                    //Prescriptions_Add();
//                    //Dinh sua o day
//                    Coroutine.BeginExecute(CoroutinePrescriptions_Add());
//                }
//                else
//                {
//                    //Cộng lại dòng trống
//                    AddNewBlankDrugIntoPrescriptObjectNew();
//                    //Cộng lại dòng trống
//                }
//            }
//        }

//        public IEnumerator<IResult> CoroutinePrescriptions_Add() 
//        {
//            if (!LatestePrecriptions.HasAppointment)
//            {
//                var dialog = new PresApptShowDialogTask();
//                yield return dialog;
//                ObjTaoThanhToaMoi.HasAppointment = dialog.HasAppointment;
//            }
//            Prescriptions_Add();            
//            yield break;
//        }

//        public IEnumerator<IResult> CoroutinePrescriptions_Update()
//        {
//            if (!LatestePrecriptions.HasAppointment)
//            {
//                var dialog = new PresApptShowDialogTask();
//                yield return dialog;
//                ObjTaoThanhToaMoi.HasAppointment = dialog.HasAppointment;
//            }
//            Prescriptions_Update();
//            yield break;
//        }

//        public IEnumerator<IResult> CoroutinePrescriptions_DuocSiEdit()
//        {
//            if (!LatestePrecriptions.HasAppointment)
//            {
//                var dialog = new PresApptShowDialogTask();
//                yield return dialog;
//                ObjToaOfDuocSi.HasAppointment = dialog.HasAppointment;
//            }
//            Prescriptions_DuocSiEdit();
            
//            yield break;
//        }

//        public IEnumerator<IResult> CoroutinePrescriptions_DuocSiEditDuocSi()
//        {
//            if (!LatestePrecriptions.HasAppointment)
//            {
//                var dialog = new PresApptShowDialogTask();
//                yield return dialog;
//                ObjToaOfDuocSi.HasAppointment = dialog.HasAppointment;
//            }
//            Prescriptions_DuocSiEditDuocSi();
//            yield break;
//        }

//        public void UpdateDefaultValueForNewPrecription()
//        {
//            NewPrecriptions.TimesNumberIsPrinted = 0;
//            NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;
//        }

//        private void ButtonCreateNew()
//        {
//            IsPhatHanhLai = false;

//            ContentKhungTaiKhamIsEnabled = true;
//            IsEnabled = true;

//            btnCreateNewIsEnabled = false;

//            btnSaveAddNewIsEnabled = true;
//            btnUndoIsEnabled = true;

//            btnEditIsEnabled = false;
//            btnCreateAndCopyIsEnabled = false;

//            btnUpdateIsEnabled = false;
//            //bntSaveAsIsEnabled = false;
//            btDuocSiEditIsEnabled = false;
//            btnCopyToIsEnabled = false;

//            btChonChanDoanIsEnabled = true;

//            IsEnabledPrint = false;
//            //
//            //App
//            chkHasAppointmentValue = false;
//            //App
//        }


//        public void btnCreateNew()
//        {
//            ButtonCreateNew();

//            NewPrecriptions = new Prescription();
//            NewPrecriptions.ServiceRecID = ObjDiagnosisTreatment_Current.ServiceRecID;
//            NewPrecriptions.PtRegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;
//            NewPrecriptions.Diagnosis = string.IsNullOrEmpty(ObjDiagnosisTreatment_Current.DiagnosisFinal) ? ObjDiagnosisTreatment_Current.Diagnosis.Trim() : ObjDiagnosisTreatment_Current.DiagnosisFinal.Trim();
//            NewPrecriptions.NDay = 0;

//            NewPrecriptions.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;

//            NewPrecriptions.ConsultantDoctor = new Staff();

//            NewPrecriptions.CreatorStaffID = Globals.LoggedUserAccount.Staff.StaffID;
//            NewPrecriptions.ObjCreatorStaffID = new Staff();
//            NewPrecriptions.ObjCreatorStaffID.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
//            NewPrecriptions.ObjCreatorStaffID.FullName = Globals.LoggedUserAccount.Staff.FullName;

//            NewPrecriptions.ExamDate = ObjDiagnosisTreatment_Current.PatientServiceRecord.ExamDate;

//            NewPrecriptions.V_PrescriptionNotes = (long)AllLookupValues.V_PrescriptionNotes.TOAGOC;
//            NewPrecriptions.V_PrescriptionType = CurrentPrescriptionType.LookupID;

//            NewPrecriptions.PrescriptionIssueHistory = new PrescriptionIssueHistory();

//            //Cụ thể  DV nào
//            if (Globals.PatientAllDetails.PtRegistrationInfo != null)
//            {
//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
//                {
//                    if (Globals.PatientAllDetails.PtRegistrationDetailInfo != null && Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID > 0)
//                    {
//                        NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = Globals.PatientAllDetails.PtRegistrationDetailInfo.PtRegDetailID;
//                    }
//                    else
//                    {
//                        MessageBox.Show(eHCMSResources.A0705_G1_Msg_KhTheRaToaNgTru);
//                        return;
//                    }
//                }
//                else/*Khám VIP, Khám Cho Nội Trú*/
//                {
//                    NewPrecriptions.PrescriptionIssueHistory.PtRegDetailID = 0;
//                }
//            }
//            else
//            {
//                MessageBox.Show(eHCMSResources.A0704_G1_Msg_InfoKhTheRaToa);
//                return;
//            }
//            //Cụ thể  DV nào
//            BackupCurPrescriptionItem();
//            LatestePrecriptions = NewPrecriptions.DeepCopy();

//            AddNewBlankDrugIntoPrescriptObjectNew();

//        }

//        public void btnCreateNewAndCopy()
//        {
//            IsPhatHanhLai = false;
//            //if (LatestePrecriptions.CanEdit)
//            {
//                if (CheckIsVuotNgayQuiDinhHI())
//                {
//                    if (MessageBox.Show(eHCMSResources.A0838_G1_Msg_ConfTuDChinhNgDung, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                    {
//                        //Tự động điều chỉnh
//                        AutoAdjust();

//                        //Trạng thái đợi sửa
//                        TrangThaiWaitingCreateAndCopy(IsPhatHanhLai);
//                    }
//                    else
//                    {
//                        //Trạng thái đợi sửa 
//                        TrangThaiWaitingCreateAndCopy(IsPhatHanhLai);
//                    }
//                }
//                else
//                {
//                    //Trạng thái đợi sửa 
//                    TrangThaiWaitingCreateAndCopy(IsPhatHanhLai);
//                }
//            }

//        }

//        #region "Dãy Nút Dược Sĩ Không Cần Phải Thấy"
//        private bool _btnCreateNewVisibility = true;
//        public bool btnCreateNewVisibility
//        {
//            get
//            {
//                return _btnCreateNewVisibility;
//            }
//            set
//            {
//                if (_btnCreateNewVisibility != value)
//                {
//                    _btnCreateNewVisibility = value;
//                    NotifyOfPropertyChange(() => btnCreateNewVisibility);
//                }
//            }
//        }

//        private bool _btnSaveAddNewVisibility = true;
//        public bool btnSaveAddNewVisibility
//        {
//            get
//            {
//                return _btnSaveAddNewVisibility;
//            }
//            set
//            {
//                if (_btnSaveAddNewVisibility != value)
//                {
//                    _btnSaveAddNewVisibility = value;
//                    NotifyOfPropertyChange(() => btnSaveAddNewVisibility);
//                }
//            }
//        }


//        private bool _bntSaveAsVisibility = true;
//        public bool bntSaveAsVisibility
//        {
//            get
//            {
//                return _bntSaveAsVisibility;
//            }
//            set
//            {
//                if (_bntSaveAsVisibility != value)
//                {
//                    _bntSaveAsVisibility = value;
//                    NotifyOfPropertyChange(() => bntSaveAsVisibility);
//                }
//            }
//        }


//        private bool _btnUpdateVisibility = true;
//        public bool btnUpdateVisibility
//        {
//            get
//            {
//                return _btnUpdateVisibility;
//            }
//            set
//            {
//                if (_btnUpdateVisibility != value)
//                {
//                    _btnUpdateVisibility = value;
//                    NotifyOfPropertyChange(() => btnUpdateVisibility);
//                }
//            }
//        }

//        private bool _btnCopyToVisible = true;
//        public bool btnCopyToVisible
//        {
//            get
//            {
//                return _btnCopyToVisible;
//            }
//            set
//            {
//                if (_btnCopyToVisible != value)
//                {
//                    _btnCopyToVisible = value;
//                    NotifyOfPropertyChange(() => btnCopyToVisible);
//                }
//            }
//        }

//        private bool _btChonChanDoanVisibility = true;
//        public bool btChonChanDoanVisibility
//        {
//            get
//            {
//                return _btChonChanDoanVisibility;
//            }
//            set
//            {
//                if (_btChonChanDoanVisibility != value)
//                {
//                    _btChonChanDoanVisibility = value;
//                    NotifyOfPropertyChange(() => btChonChanDoanVisibility);
//                }
//            }
//        }

//        private void GetPrescriptionTypes_LoadList()
//        {
//            //▼====== #001
//            PrescriptionTypeList = new ObservableCollection<Lookup>();
//            foreach (var tmpLookup in Globals.AllLookupValueList)
//            {
//                if (tmpLookup.ObjectTypeID == (long)(LookupValues.PRESCRIPTION_TYPE))
//                {
//                    PrescriptionTypeList.Add(tmpLookup);
//                }
//            }
//            if (PrescriptionTypeList.Count > 0)
//            {
//                CurrentPrescriptionType = PrescriptionTypeList[0];
//            }
//            //▲====== #001
//            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

//            //IsWaitingGetPrescriptionTypes = true;


//            //var t = new Thread(() =>
//            //{
//            //    using (var serviceFactory = new ePrescriptionsServiceClient())
//            //    {
//            //        var contract = serviceFactory.ServiceInstance;
//            //        contract.BeginGetLookupPrescriptionType(Globals.DispatchCallback((asyncResult) =>
//            //        {

//            //            try
//            //            {
//            //                var results = contract.EndGetLookupPrescriptionType(asyncResult);
//            //                if (results != null)
//            //                {
//            //                    PrescriptionTypeList.Clear();

//            //                    PrescriptionTypeList = new ObservableCollection<Lookup>(results);

//            //                    if (PrescriptionTypeList.Count > 0)
//            //                    {
//            //                        CurrentPrescriptionType = PrescriptionTypeList[0];
//            //                    }
//            //                }
//            //            }
//            //            catch (Exception ex)
//            //            {
//            //                MessageBox.Show(ex.Message);
//            //            }
//            //            finally
//            //            {
//            //                IsWaitingGetPrescriptionTypes = false;
//            //            }

//            //        }), null);

//            //    }

//            //});

//            //t.Start();
//        }

//        #endregion


//        #endregion

//        private int SoNgayTaiKham;
//        public void txtDaysAfter_KeyUp(object sender, KeyEventArgs e)
//        {
//            int value = 0;
//            int.TryParse((sender as TextBox).Text, out value);
//            txtDaysAfter_KeyUp(tempPrescriptionDetail,value);
//            if (LatestePrecriptions != null)
//            {
//                if (LatestePrecriptions.IssueID <= 0)/*Thì mới tự động lấy trên xài xuống cho Toa Thuốc, còn Sửa thì phải đợi người ta Bấm Update Ngày Ngày*/
//                {
//                    if (SoNgayTaiKham != value && value > 0)
//                    {
//                        SoNgayTaiKham = value;
//                        LatestePrecriptions.NDay = value;
                        
//                        if (LatestePrecriptions.PrescriptionDetails != null)
//                        {
//                            foreach (PrescriptionDetail prescriptDObj in LatestePrecriptions.PrescriptionDetails)
//                            {
//                                if (prescriptDObj.HasSchedules || prescriptDObj.isNeedToUse) 
//                                {
//                                    continue;
//                                }
//                                int NDay = LatestePrecriptions.NDay.GetValueOrDefault();
//                                if (prescriptDObj.SelectedDrugForPrescription != null
//                                        && prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed > 0
//                                        && LatestePrecriptions.NDay > prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed)
//                                {
//                                    NDay = prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed.GetValueOrDefault();
//                                    //prescriptDObj.DayRpts = prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//                                }
//                                //else                                
//                                {
//                                    prescriptDObj.DayRpts = NDay;
//                                    prescriptDObj.DayExtended = 0;
//                                    if (IsBenhNhanNoiTru() && IsPatientInsurance())
//                                    {
//                                        if (NDay > xNgayBHToiDa_NoiTru)
//                                        {
//                                            prescriptDObj.DayRpts = xNgayBHToiDa_NoiTru;
//                                            prescriptDObj.DayExtended = NDay - xNgayBHToiDa_NoiTru;
//                                        }
//                                        else
//                                        {
//                                            prescriptDObj.DayRpts = NDay;
//                                            prescriptDObj.DayExtended = 0;
//                                        }
//                                    }
//                                    else if (!IsBenhNhanNoiTru() && IsPatientInsurance())
//                                    {
//                                        if (!prescriptDObj.IsDrugNotInCat)
//                                        {
//                                            if (NDay > xNgayBHToiDa_NgoaiTru)
//                                            {

//                                                prescriptDObj.DayRpts = xNgayBHToiDa_NgoaiTru;
//                                                prescriptDObj.DayExtended = NDay - xNgayBHToiDa_NgoaiTru;
//                                            }
//                                            else
//                                            {
//                                                prescriptDObj.DayRpts = NDay;
//                                                prescriptDObj.DayExtended = 0;
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        prescriptDObj.DayRpts = NDay;
//                                        prescriptDObj.DayExtended = 0;
//                                    }
//                                    SetValueFollowNgayDung(prescriptDObj);
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            if (CtrcboDonVi.SelectedIndex == 0)
//            {
//                if (value <= 0)
//                {
//                    CtrtbSoTuan.Text = value.ToString();
//                    return;
//                }

//                if (value > 7)
//                {
//                    CtrtbSoTuan.Text = (value / 7).ToString();
//                }
//                else
//                {
//                    CtrtbSoTuan.Text = "1";
//                }
//            }
//            else
//            {
//                CtrtbSoTuan.Text = value.ToString();
//            }
//        }

//        public void txtDaysAfter_KeyUp(PrescriptionDetail prescriptDObj, int value)
//        {
            
//            if (SoNgayTaiKham != value && value > 0)
//            {
//                LatestePrecriptions.NDay = value;
//                if (LatestePrecriptions.PrescriptionDetails != null)
//                {
//                    if (prescriptDObj.HasSchedules || prescriptDObj.isNeedToUse)
//                    {
//                        return;
//                    }
//                    int NDay = LatestePrecriptions.NDay.GetValueOrDefault();
//                    if (prescriptDObj.SelectedDrugForPrescription != null
//                            && prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed > 0
//                            && LatestePrecriptions.NDay > prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed)
//                    {
//                        NDay = prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed.GetValueOrDefault();
//                        //prescriptDObj.DayRpts = prescriptDObj.SelectedDrugForPrescription.MaxDayPrescribed.Value;
//                    }
//                    //else                                
//                    {
//                        prescriptDObj.DayRpts = NDay;
//                        prescriptDObj.DayExtended = 0;
//                        if (IsBenhNhanNoiTru() && IsPatientInsurance())
//                        {
//                            if (prescriptDObj.InsuranceCover && NDay > xNgayBHToiDa_NoiTru)
//                            {
//                                prescriptDObj.DayRpts = xNgayBHToiDa_NoiTru;
//                                prescriptDObj.DayExtended = NDay - xNgayBHToiDa_NoiTru;
//                            }
//                            else
//                            {
//                                prescriptDObj.DayRpts = NDay;
//                                prescriptDObj.DayExtended = 0;
//                            }
//                        }
//                        else if (!IsBenhNhanNoiTru() && IsPatientInsurance())
//                        {
//                            if (!prescriptDObj.IsDrugNotInCat
//                                && prescriptDObj.InsuranceCover)
//                            {
//                                if (NDay > xNgayBHToiDa_NgoaiTru)
//                                {

//                                    prescriptDObj.DayRpts = xNgayBHToiDa_NgoaiTru;
//                                    prescriptDObj.DayExtended = NDay - xNgayBHToiDa_NgoaiTru;
//                                }
//                                else
//                                {
//                                    prescriptDObj.DayRpts = NDay;
//                                    prescriptDObj.DayExtended = 0;
//                                }
//                            }
//                        }
//                        else
//                        {
//                            prescriptDObj.DayRpts = NDay;
//                            prescriptDObj.DayExtended = 0;
//                        }
//                        SetValueFollowNgayDung(prescriptDObj);
//                    }
//                }
//            }
//        }

//        #region Đơn Vị nhập Tuần, Ngày
//        private AxComboBox CtrcboDonVi;
//        public void cboDonVi_Loaded(object sender, RoutedEventArgs e)
//        {
//            CtrcboDonVi = sender as AxComboBox;
//        }

//        private AxTextBox CtrtbSoTuan;
//        public void tbSoTuan_Loaded(object sender, RoutedEventArgs e)
//        {
//            CtrtbSoTuan = sender as AxTextBox;
//        }

//        public void tbSoTuan_KeyUp(object sender, KeyEventArgs e)
//        {
//            string v = (sender as TextBox).Text;
//            if (!string.IsNullOrEmpty(v))
//            {
//                int num = 0;
//                int.TryParse(v, out num);

//                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện

//                if (CtrcboDonVi.SelectedIndex == 0)
//                {
//                    LatestePrecriptions.NDay = num * 7;
//                }
//                else
//                {
//                    LatestePrecriptions.NDay = num;
//                }
//            }

//        }

//        public void cboDonVi_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            AxComboBox Ctr = sender as AxComboBox;

//            string v = CtrtbSoTuan.Text;
//            if (!string.IsNullOrEmpty(v))
//            {
//                int num = 0;
//                int.TryParse(v, out num);

//                if (Ctr.SelectedIndex == 0)
//                {
//                    LatestePrecriptions.NDay = num * 7;
//                }
//                else
//                {
//                    LatestePrecriptions.NDay = num;
//                }
//            }

//            if (LatestePrecriptions.NDay > 0)
//            {
//                if (
//                    MessageBox.Show(eHCMSResources.A0982_G1_Msg_ConfCNhatSoNgDungThuoc2                       
//                        , eHCMSResources.G0442_G1_TBao,MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                {
//                    AutoAdjustCancelDrugShortDays();
//                    AutoAdjustCancelDrugShortDays(tempPrescriptionDetail);
//                    SoNgayDungThuoc_Root = LatestePrecriptions.NDay;
//                }
//            }
//        }

//        public void tbSoTuan_LostFocus(object sender, RoutedEventArgs e)
//        {
//            SoNgayDungThuocThayDoi(sender, e);
//        }

//        private void SoNgayDungThuocThayDoi(object sender, RoutedEventArgs e)
//        {
//            Nullable<int> v = LatestePrecriptions.NDay;

//            if (LatestePrecriptions != null && LatestePrecriptions.IssueID > 0)
//            {
//                if (SoNgayDungThuoc_Root != v)
//                {
//                    txtDaysAfter_LostFocus(sender, e);
//                }
//            }
//        }

//        #endregion


//        #region Tính Tiền Thuốc
//        public void btCalc()
//        {

//            if (grdPrescription == null)
//            {
//                MessageBox.Show(eHCMSResources.A0365_G1_Msg_InfoChonThuocDeRaToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckAllThuocBiDiUng())
//            {
//                return;
//            }

//            if (ErrCheckChongChiDinh())
//                return;

//            if (grdPrescription.IsValid == false)
//            {
//                MessageBox.Show(string.Format("{0} {1}", eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.A0541_G1_Msg_InfoDataToaThuocKhHopLe), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//                return;
//            }

//            if (CheckThuocHopLe() == false)
//                return;

//            if (CheckThuocBiTrungTrongToa())
//                return;


//            short LoaiBenhNhan = 0;//--1;bh,0;binh thuong
//            if (Globals.IsPatientInsurance())
//            {
//                LoaiBenhNhan = 1;
//            }
//            Action<IePrescriptionCalcNotSave> onInitDlg = delegate (IePrescriptionCalcNotSave typeInfo)
//            {
//                typeInfo.ObjPrescription = LatestePrecriptions;
//                typeInfo.StoreID = StoreID.Value;
//                typeInfo.RegistrationType = LoaiBenhNhan;
//                typeInfo.Calc();
//            };
//            GlobalsNAV.ShowDialog<IePrescriptionCalcNotSave>(onInitDlg);
//        }



//        #endregion

//        public void btUpdateNgayDung()
//        {
//            if (LatestePrecriptions == null)
//                return;
//            if (LatestePrecriptions.NDay == null)
//                return;

//            if (
//                MessageBox.Show(eHCMSResources.A0980_G1_Msg_ConfCNhatSoNgDungThuoc, eHCMSResources.G0442_G1_TBao,
//                                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//            {
//                AutoAdjustCancelDrugShortDays();
//                AutoAdjustCancelDrugShortDays(tempPrescriptionDetail);
//            }
//        }


//        Nullable<int> SoNgayDungThuoc_Root;

//        public void txtDaysAfter_LostFocus(object sender, RoutedEventArgs e)
//        {
//            string v = (sender as TextBox).Text;

//            if (!string.IsNullOrEmpty(v))
//            {
//                int num = 0;
//                int.TryParse(v, out num);

//                ((System.Windows.Controls.TextBox)(sender)).Text = num.ToString();//show ra giao diện

//                if (LatestePrecriptions != null && LatestePrecriptions.IssueID > 0)
//                {
//                    if (SoNgayDungThuoc_Root != num)
//                    {
//                        if (MessageBox.Show(eHCMSResources.A0982_G1_Msg_ConfCNhatSoNgDungThuoc2,eHCMSResources.G0442_G1_TBao, 
//                            MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//                        {
//                            AutoAdjustCancelDrugShortDays();
//                            AutoAdjustCancelDrugShortDays(tempPrescriptionDetail);
//                            SoNgayDungThuoc_Root = LatestePrecriptions.NDay;
//                        }
//                        else
//                        {
//                            LatestePrecriptions.NDay = SoNgayDungThuoc_Root;
//                        }
//                    }
//                }
//            }
//        }


//        public void hplDrugNotInCat_Click(Object pSelectedItem)
//        {
//            SelectedPrescriptionDetail = pSelectedItem as PrescriptionDetail;
//            //if (!SelectedPrescriptionDetail.isComboDrugType)
//            //{
//            //    MessageBox.Show("Thuốc này ngoài danh mục, không được phép chỉnh sửa!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            //    return;
//            //}
//            if (!SelectedPrescriptionDetail.IsDrugNotInCat)
//            {
//                MessageBox.Show(eHCMSResources.A0085_G1_Msg_InfoChonThuocNgoaiDMTruoc);
//                return;
//            }

//            if (SelectedPrescriptionDetail.SelectedDrugForPrescription == null)
//                SelectedPrescriptionDetail.SelectedDrugForPrescription = new GetDrugForSellVisitor();
            
//            //if (SelectedPrescriptionDetail.HasSchedules == true)
//            //{
//            //    MessageBox.Show("Thuốc: '" + SelectedPrescriptionDetail.SelectedDrugForPrescription.BrandName.Trim() + "' Đã Có Chỉ Định Uống Theo Tuần!");
//            //    return;
//            //}

//            IndexRow = grdPrescription.SelectedIndex;

//            Action<IPrescriptionDrugNotInCat> onInitDlg = delegate (IPrescriptionDrugNotInCat typeInfo)
//            {
//                typeInfo.IndexRow = IndexRow;
//                typeInfo.ObjPrescriptionDetail = SelectedPrescriptionDetail.DeepCopy();
//                typeInfo.ChooseDoses = ChooseDoses;
//            };
//            GlobalsNAV.ShowDialog<IPrescriptionDrugNotInCat>(onInitDlg);
//        }


//        public void Handle(PrescriptionDrugNotInCatSelectedEvent<PrescriptionDetail, int> message)
//        {
//            if (message != null && this.GetView() != null)
//            {
//                UpdateLatestePrecriptionsDrugNotInCat(message.PrescriptionDrugNotInCat, message.Index);
//            }
//        }

//        public void UpdateLatestePrecriptionsDrugNotInCat(PrescriptionDetail ObjPrescriptionDetail, int Index)
//        {
//            LatestePrecriptions.PrescriptionDetails[Index] = ObjPrescriptionDetail.DeepCopy();

//            if (
//                (LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].DrugID == null
//                ||
//                LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].DrugID == 0)
//                &&
//                LatestePrecriptions.PrescriptionDetails[LatestePrecriptions.PrescriptionDetails.Count - 1].IsDrugNotInCat == true
//                )
//            {
//                AddNewBlankDrugIntoPrescriptObjectNew();
//            }
//        }

       

//        #region "Check Thuoc Di Ung"
//        private ObservableCollection<MDAllergy> _ptAllergyList;
//        public ObservableCollection<MDAllergy> PtAllergyList
//        {
//            get
//            {
//                return _ptAllergyList;
//            }
//            set
//            {
//                if (_ptAllergyList != value)
//                {
//                    _ptAllergyList = value;
//                    NotifyOfPropertyChange(() => PtAllergyList);
//                }
//            }
//        }
//        private void MDAllergies_ByPatientID(Int64 PatientID, int flag)
//        {
//            var t = new Thread(() =>
//            {
//                IsWaitingMDAllergies_ByPatientID = true;

//                using (var serviceFactory = new SummaryServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;
//                    contract.BeginMDAllergies_ByPatientID(PatientID, flag, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndMDAllergies_ByPatientID(asyncResult);

//                            string str = "";

//                            if (results != null)
//                            {
//                                if (PtAllergyList == null)
//                                {
//                                    PtAllergyList = new ObservableCollection<MDAllergy>();
//                                }
//                                else
//                                {
//                                    PtAllergyList.Clear();
//                                }
//                                foreach (MDAllergy p in results)
//                                {
//                                    PtAllergyList.Add(p);
//                                    str += p.AllergiesItems.Trim() + ";";
//                                }
//                            }
//                            if (!string.IsNullOrEmpty(str))
//                            {
//                                str = str.Substring(0, str.Length - 1);
//                            }
//                            Globals.Allergies = str;

//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            IsWaitingMDAllergies_ByPatientID = false;
//                        }

//                    }), null);

//                }

//            });
//            t.Start();
//        }

//        private bool Check1ThuocBiDiUng(string DrugName)
//        {
//            bool Res = false;

//            foreach (var item in PtAllergyList)
//            {
//                if (DrugName.ToLower().Trim() == item.AllergiesItems.ToLower().Trim())
//                {
//                    Res = true;
//                }
//            }
//            return Res;
//        }

//        private bool CheckAllThuocBiDiUng()
//        {
//            StringBuilder sb = new StringBuilder();

//            bool Result = false;

//            if (LatestePrecriptions.PrescriptionDetails.Count > 1)
//            {
//                foreach (PrescriptionDetail item in LatestePrecriptions.PrescriptionDetails)
//                {
//                    if(!CheckThuocHopLe(item))
//                        continue;
//                    if (item.SelectedDrugForPrescription != null && !string.IsNullOrEmpty(item.BrandName))
//                    {
//                        if (item.IsDrugNotInCat == true)
//                        {
//                            if (Check1ThuocBiDiUng(item.BrandName.Trim()))
//                            {
//                                sb.AppendLine("-" + item.BrandName);
//                                Result = true;
//                            }
//                        }
//                        else
//                        {
//                            if (Check1ThuocBiDiUng(item.SelectedDrugForPrescription.BrandName.Trim()))
//                            {
//                                sb.AppendLine("-" + item.SelectedDrugForPrescription.BrandName.Trim());
//                                Result = true;
//                            }
//                        }
//                    }
//                }
//            }
//            if (Result)
//            {
//                MessageBox.Show(string.Format("{0}: \n{1}", eHCMSResources.A0504_G1_Msg_InfoDSThuocDiUng, sb.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
//            }
//            return Result;
//        }

//        /// <summary>
//        /// Kiem tra thuoc co hop le hay ko
//        /// neu trong danh muc phai co drugID, ngoai dm phai co brandName
//        /// /// </summary>
//        /// <returns></returns>
//        private bool CheckThuocHopLe(PrescriptionDetail item) 
//        {
//            if (item.SelectedDrugForPrescription==null)
//            {
//                return false;
//            }
//            if (!item.IsDrugNotInCat && item.SelectedDrugForPrescription.DrugID < 1)
//                return false;
//            if (item.IsDrugNotInCat &&
//                (item.SelectedDrugForPrescription.BrandName == null
//                || item.SelectedDrugForPrescription.BrandName == ""))
//            {
//                return false;
//            }
//            return true;
//        }

//        #endregion

//        public void Handle(PrescriptionNoteTempType_Change obj)
//        {
//            RefreshLookup();
//        }
        
//        public bool PrintInfoBH
//        {
//            get { return Globals.PatientAllDetails.PtRegistrationInfo != null && Globals.PatientAllDetails.PtRegistrationInfo.HisID > 0; }
//        }

//        public void ShowHiConfirmationReport()
//        {
//            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
//            {
//                reportVm.RegistrationID = Globals.PatientAllDetails.PtRegistrationInfo.PtRegistrationID;

//                if (Globals.PatientAllDetails.PtRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
//                {
//                    reportVm.eItem = ReportName.REGISTRATION_IN_PATIENT_HI_CONFIRMATION;
//                }
//                else
//                {
//                    reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_HI_CONFIRMATION;
//                    reportVm.ServiceRecID = LatestePrecriptions != null ? LatestePrecriptions.ServiceRecID : 0;
//                }
//            };
//            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

//            //var reportVm = Globals.GetViewModel<ICommonPreviewView>();
//            //reportVm.RegistrationID = Globals.PatientAllDetails.RegistrationInfo.PtRegistrationID;
//            //reportVm.ServiceRecID = LatestePrecriptions.ServiceRecID;
//            //reportVm.eItem = ReportName.REGISTRATION_OUT_PATIENT_HI_CONFIRMATION;
//            //Globals.ShowDialog(reportVm as Conductor<object>);
//        }
//    }
//}

