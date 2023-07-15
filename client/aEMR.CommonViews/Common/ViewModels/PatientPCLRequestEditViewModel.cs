using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using Caliburn.Micro;
using System.Windows;
using DataEntities;
using Service.Core.Common;
using System.Collections.Generic;
using System.Windows.Input;
using eHCMS.CommonUserControls.CommonTasks;
using eHCMSLanguage;
using aEMR.ViewContracts;
using aEMR.Common.BaseModel;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Common.PagedCollectionView;
using aEMR.DataContracts;
using aEMR.CommonTasks;

/*
 * 20181121 #001 TBL:   BM 0004201: Hen benh cls khong can phai nhap ngay y lenh
 * 20181129 #002 TTM:   BM 0005340: Bắt buộc nhập chẩn đoán khi chỉ định xét nghiệm. (Đã comment kiểm tra vì không cần thiết)
 * 20181228 #003 TTM:   Nếu là phiếu mới thì bác sĩ chỉ định là người đăng nhập
 * 20190314 #004 TTM:   BM 0006646: 
 * 20190822 #005 TTM:   BM 0013207: Sửa lỗi lấy chẩn đoán của phiếu cũ nếu như tạo mới từ 1 đợt điều trị mới đã có chẩn đoán mới.
 * 20190822 #006 TTM:   BM 0013217: Cho phép sử dụng Grid phiếu chỉ định trong đợt đăng ký để thực hiện hiệu chỉnh.
 * 20191025 #007 TBL:   BM 0018467: Thêm IsNotCheckInvalid để khỏi kiểm tra khoảng thời gian giữa 2 lần làm CLS được tính BHYT
 * 20200530 #008 TTM:   BM 0038202: Chỉnh sửa trường ngày y lệnh từ loại control chỉ có ngày sang có cả ngày và giờ.
 * 20200716 #009 TTM:   BM 0038205: Cập nhật phiếu yêu cầu CLS nếu cùng phòng, cùng bác sĩ chỉ định, cùng ngày y lệnh thì cho phép gộp các dịch vụ thành 1 phiếu ngược lại thì tách phiếu.
 * 20200723 #010 TTM:   BM 0039393: Fix lỗi chết chương trình khi tạo phiếu yêu cầu CLS khi chưa có Bác sĩ chỉ định.
 * 20210701 #011 TNHX:  260 Thêm user bsi mượn
 * 20211004 #012 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
 * 20220530 #013 BLQ: Kiểm tra thời gian thao tác của bác sĩ
 * 20220801 #014 DatTB: Chặn không cho lưu y lệnh khi có đề nghị chuyển khoa.
 * 20230516 #015 QTD:   Chạn không cho lưu chỉ định khi chưa có ICD
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientPCLRequestEdit)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PatientPCLRequestEditViewModel : ViewModelBase, IPatientPCLRequestEdit
        , IHandle<DbClickSelectedObjectEventWithKey<PatientPCLRequest, String>>
        , IHandle<TinhTongTienDuTruEvent>
        , IHandle<SelectedObjectEventWithKey<PCLExamType, String>>
        , IHandle<SelectedObjectEventWithKeyAppt<PCLExamType, String>>
        , IHandle<PCLRequestDetailRemoveEvent<PatientPCLRequestDetail>>
        //, IHandle<ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM>
        , IHandle<DbClickSelectedObjectEventWithKeyToShowDetails<PatientPCLRequest, String>>
        //, IHandle<PCLExamAccordingICD_Event>
        //, IHandle<UpdateDiagnosisTreatmentAndPrescription_Event>
    {
        /// <summary>
        /// sua lai form nay su dung chung cho 2 chuc nang
        /// - tao phieu can lam sang
        /// - hen can lam sang (them moi va chinh sua)
        /// </summary>
        private long IDLABByName_Bo = 28889;

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Publish(new ItemEdited<PatientApptPCLRequests> { Item = null, Source = null });
        }
        #region property gen su dung cho busy indicator
        public override bool IsProcessing
        {
            get
            {
                return _isLoadingMainCategory
                    || _isLoadingPCLForms
                    || _isWaitingLoadPCLExamType
                    || _isWaitingSave
                    || _isWaitingLoadPCLInfo
                    || _isWaitingLoadKhoa
                    || _isWaitingLoadPhong
                    || _isWaitingLoadDetailPCLRequest
                    || _isWaitingDeletePCLRequest
                    || _isWaitingLoadChanDoan
                    || _isWaitingLoadRegistration;
            }
        }
        public override string StatusText
        {
            get
            {
                if (_isLoadingMainCategory)
                {
                    return eHCMSResources.Z0340_G1_LoadDSLoaiPCL;
                }
                if (_isLoadingPCLForms)
                {
                    return eHCMSResources.Z0341_G1_LoadDSPCLForm;
                }
                if (_isWaitingLoadPCLExamType)
                {
                    return eHCMSResources.Z0483_G1_LoadDSXetNghiem;
                }
                if (_isWaitingSave)
                {
                    return eHCMSResources.Z0343_G1_DangLuu;
                }
                if (_isWaitingLoadPCLInfo)
                {
                    return eHCMSResources.Z0344_G1_LoadTTinPh;
                }
                if (_isWaitingLoadKhoa)
                {
                    return eHCMSResources.Z0345_G1_LoadKhoa;
                }
                if (_isWaitingLoadPhong)
                {
                    return eHCMSResources.Z0346_G1_LoadPhg;
                }
                if (_isWaitingLoadDetailPCLRequest)
                {
                    return eHCMSResources.Z0347_G1_LoadCTietPhieu;
                }
                if (_isWaitingDeletePCLRequest)
                {
                    return eHCMSResources.Z0348_G1_DangXoaPhieu;
                }
                if (_isWaitingLoadChanDoan)
                {
                    return eHCMSResources.Z0484_G1_LoadDSChanDoan;
                }
                if (_isWaitingLoadRegistration)
                {
                    return eHCMSResources.Z0349_G1_LayTTinDKCuaPh;
                }
                return string.Empty;
            }
        }
        private bool _isLoadingMainCategory;
        public bool IsLoadingMainCategory
        {
            get { return _isLoadingMainCategory; }
            set
            {
                if (_isLoadingMainCategory != value)
                {
                    _isLoadingMainCategory = value;
                    NotifyOfPropertyChange(() => IsLoadingMainCategory);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _isLoadingPCLForms;
        public bool IsLoadingPCLForms
        {
            get { return _isLoadingPCLForms; }
            set
            {
                if (_isLoadingPCLForms != value)
                {
                    _isLoadingPCLForms = value;
                    NotifyOfPropertyChange(() => IsLoadingPCLForms);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadPCLExamType;
        public bool IsWaitingLoadPCLExamType
        {
            get { return _isWaitingLoadPCLExamType; }
            set
            {
                if (_isWaitingLoadPCLExamType != value)
                {
                    _isWaitingLoadPCLExamType = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadPCLExamType);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingSave;
        public bool IsWaitingSave
        {
            get { return _isWaitingSave; }
            set
            {
                if (_isWaitingSave != value)
                {
                    _isWaitingSave = value;
                    NotifyOfPropertyChange(() => IsWaitingSave);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadPCLInfo;
        public bool IsWaitingLoadPCLInfo
        {
            get { return _isWaitingLoadPCLInfo; }
            set
            {
                if (_isWaitingLoadPCLInfo != value)
                {
                    _isWaitingLoadPCLInfo = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadPCLInfo);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadKhoa;
        public bool IsWaitingLoadKhoa
        {
            get { return _isWaitingLoadKhoa; }
            set
            {
                if (_isWaitingLoadKhoa != value)
                {
                    _isWaitingLoadKhoa = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadKhoa);
                    NotifyWhenBusy();
                }
            }
        }

        private bool _isWaitingLoadPhong;
        public bool IsWaitingLoadPhong
        {
            get { return _isWaitingLoadPhong; }
            set
            {
                if (_isWaitingLoadPhong != value)
                {
                    _isWaitingLoadPhong = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadPhong);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadDetailPCLRequest;
        public bool IsWaitingLoadDetailPCLRequest
        {
            get { return _isWaitingLoadDetailPCLRequest; }
            set
            {
                if (_isWaitingLoadDetailPCLRequest != value)
                {
                    _isWaitingLoadDetailPCLRequest = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadDetailPCLRequest);
                    NotifyWhenBusy();
                }
            }
        }



        private bool _isWaitingDeletePCLRequest;
        public bool IsWaitingDeletePCLRequest
        {
            get { return _isWaitingDeletePCLRequest; }
            set
            {
                if (_isWaitingDeletePCLRequest != value)
                {
                    _isWaitingDeletePCLRequest = value;
                    NotifyOfPropertyChange(() => IsWaitingDeletePCLRequest);
                    NotifyWhenBusy();
                }
            }
        }


        private bool _isWaitingLoadRegistration;
        public bool IsWaitingLoadRegistration
        {
            get { return _isWaitingLoadRegistration; }
            set
            {
                if (_isWaitingLoadRegistration != value)
                {
                    _isWaitingLoadRegistration = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadRegistration);
                    NotifyWhenBusy();
                }
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

        private bool _IsNotCheckInvalid;
        public bool IsNotCheckInvalid
        {
            get { return _IsNotCheckInvalid; }
            set
            {
                _IsNotCheckInvalid = value;
                NotifyOfPropertyChange(() => IsNotCheckInvalid);
            }
        }

        #endregion
        /// <summary> bien xac dinh la hen benh hay can lam sang
        /// =true la hen benh
        /// </summary>
        private bool _IsAppointment;
        public bool IsAppointment
        {
            get { return _IsAppointment; }
            set
            {
                if (_IsAppointment != value)
                {
                    _IsAppointment = value;
                    if (_IsAppointment)
                    {
                        AddEditApptPCLInit();

                    }
                    NotifyOfPropertyChange(() => IsAppointment);
                }
            }
        }
        //==== #001 =====
        public bool IsEnableMedicalInstructionDate
        {
            get
            {
                return !IsAppointment && V_RegistrationType != (long)AllLookupValues.RegistrationType.NGOAI_TRU && (CurrentPclRequest == null || CurrentPclRequest.IntPtDiagDrInstructionID.GetValueOrDefault(0) == 0);
            }
        }
        //==== #001 =====
        private bool _HasDiag = true;
        public bool HasDiag
        {
            get { return _HasDiag; }
            set
            {
                if (_HasDiag != value)
                {
                    _HasDiag = value;
                    NotifyOfPropertyChange(() => HasDiag);
                }
            }
        }

        private bool _OneClick = true;
        public bool OneClick
        {
            get { return _OneClick; }
            set
            {
                if (_OneClick != value)
                {
                    _OneClick = value;
                    NotifyOfPropertyChange(() => OneClick);
                    //NotifyOfPropertyChange(() => CanbtAddAll);
                }
            }
        }

        private string _DoctorComments;
        public string DoctorComments
        {
            get { return _DoctorComments; }
            set
            {
                if (_DoctorComments != value)
                {
                    _DoctorComments = value;
                    if (CurrentPclRequest != null)
                    {
                        CurrentPclRequest.DoctorComments = _DoctorComments;
                    }

                    if (EditingApptPCLRequest != null)
                    {
                        EditingApptPCLRequest.DoctorComments = _DoctorComments;
                    }
                    NotifyOfPropertyChange(() => DoctorComments);
                }
            }
        }

        private bool _IsEdit;
        public bool IsEdit
        {
            get { return _IsEdit; }
            set
            {
                if (_IsEdit != value)
                {
                    _IsEdit = value;
                    if (_IsEdit)
                    {

                    }
                    NotifyOfPropertyChange(() => IsEdit);
                }
            }
        }

        private bool _hasItem = false;
        public bool hasItem
        {
            get
            {
                return _hasItem;
            }
            set
            {
                if (_hasItem != value)
                {
                    _hasItem = value;
                    NotifyOfPropertyChange(() => hasItem);
                }
            }
        }

        /// <summary> bien dung de break ra
        /// = true la co break ra theo phieu
        /// </summary>
        private bool _IsBreakStatus;
        public bool IsBreakStatus
        {
            get { return _IsBreakStatus; }
            set
            {
                if (_IsBreakStatus != value)
                {
                    _IsBreakStatus = value;
                    NotifyOfPropertyChange(() => IsBreakStatus);
                }
            }
        }
        [ImportingConstructor]
        public PatientPCLRequestEditViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            authorization();
            FindPatient = Globals.PatientFindBy_ForConsultation == null ?
                0 : (int)Globals.PatientFindBy_ForConsultation.Value;
            PCLRequestDetailsContent = Globals.GetViewModel<IPCLRequestDetails>();
            //InitPatientInfo();            

            dicPCLExamType = new Dictionary<long, PCLExamType>();
            foreach (var item in Globals.ListPclExamTypesAllPCLForms)
            {
                if (!dicPCLExamType.ContainsKey(item.PCLExamTypeID))
                {
                    dicPCLExamType.Add(item.PCLExamTypeID, item);
                }
            }
            foreach (var item in Globals.ListPclExamTypesAllCombos)
            {
                if (!dicPCLExamType.ContainsKey(item.PCLExamTypeID))
                {
                    dicPCLExamType.Add(item.PCLExamTypeID, item.PCLExamType);
                }
            }
            //▼===== #008
            MedicalInstructionDateContent = Globals.GetViewModel<IMinHourDateControl>();
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            //▲===== #008
            //LoadDoctorStaff();
            LoadDoctorStaffCollection();
            //▼===== 20191017 TTM:  Hàm này không có gì thay đổi nên không cần phải để ở InitLoadData. Vì để đó mỗi lần chuyển bệnh nhân sẽ lại đi
            //                      query => không cần thiết
            CurrentPclRequest = new PatientPCLRequest();
            ObjTestingAgencyList = new ObservableCollection<TestingAgency>();
            GetTestingAgency_All();
            //▲===== 20191017
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

        //▼====: #012
        private void LoadDoctorStaffCollection()
        {
            long CurrentDeptID = Globals.DeptLocation.DeptID;
            if (Globals.ServerConfigSection.ConsultationElements.FilterDoctorByDeptResponsibilitiesInPt && !Globals.IsUserAdmin)
            {
                DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && x.ListDeptResponsibilities != null && ((x.ListDeptResponsibilities.Contains(CurrentDeptID.ToString()) || CurrentDeptID == 0))
                                                                                    && (!x.IsStopUsing)).ToList());
            }
            else
            {
                DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null
                                                                                    && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI
                                                                                    && (!x.IsStopUsing)).ToList());
            }
        }
        //▲====: #012

        //private IEnumerator<IResult> LoadStaffs()
        //{
        //    var paymentTypeTask = new LoadStaffListTask(false, true, Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI);
        //    yield return paymentTypeTask;
        //    DoctorStaffs = paymentTypeTask.StaffList;
        //    if (DoctorStaffs != null && DoctorStaffs.Any(x => x.StaffID == Globals.LoggedUserAccount.StaffID))
        //        gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID);
        //    yield break;
        //}
        //public void LoadDoctorStaff()
        //{
        //    if (Globals.DoctorStaffs == null || Globals.DoctorStaffs.Count() <= 0)
        //    {
        //        Coroutine.BeginExecute(LoadStaffs());
        //    }
        //}
        public void InitPatientInfo(Patient CurrentPatient)
        {
            HasDiag = true;
            DoctorComments = "";
            if (CurrentPatient != null)/*Làm CLS chỉ cần kiểm tra BN !=null*/
            {
                InitControlsForExt();

                FormIsEnabled = true;

                InitPCLRequest();

                InitLoadData();

                if (IsAppointment)
                {
                    return;
                }

                if (!Globals.isConsultationStateEdit)
                {
                    MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0391_G1_BNDuocChonTuLSBA));
                    FormIsEnabled = false;
                    return;
                }

                ObjGetDiagnosisTreatmentByPtID = new DiagnosisTreatment();


                if (Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
                {
                    PatientPCLRequestDetail_ByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                    //đối với ngoại trú, nếu chưa có chẩn đoán thì không được chỉ định cận lâm sàng vậy với nội trú thì sao? Nếu tính chẩn đoán thì kiềm tra chẩn đoán nào?
                    if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, "", 1, true, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                        //PatientPCLRequestDetail_ByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                    }
                    else
                    {
                        PatientPCLRequest_RequestLastest();
                        if (Registration_DataStorage.CurrentPatientRegistration != null
                            && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null
                            && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.MedServiceItemPriceListID > 0)
                        {
                            LoadAllPclExamTypesAction(Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo);
                            LoadAllPCLExamTypesByComboIDAction(Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo);
                        }
                        else
                        {
                            CurrentListPclExamTypesAllPCLForms = Globals.ListPclExamTypesAllPCLForms;
                            CurrentListPclExamTypesAllCombos = Globals.ListPclExamTypesAllCombos;
                            if (SPTheoAutoComplete != null)
                            {
                                SPTheoAutoComplete.SetDataForAutoComplete(Globals.ListPclExamTypesAllPCLForms, Globals.ListPclExamTypesAllCombos
                                    , Globals.ListPclExamTypesAllPCLFormImages);
                            }
                        }
                    }
                    //KMx: Load PCLRequestByRegistrationID, sau đó truyền qua cho PCLRequestDetailsView hiển thị (24/03/2014 15:54)
                    GetPatientPCLRequestList_ByRegistrationID();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0747_G1_Msg_InfoKhTimThayMaDK + " (PtRegistrationID): " + Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
                if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan, false))
                {
                    FormIsEnabled = false;
                }
            }
            else
            {
                FormIsEnabled = false;
            }
        }

        private void InitPCLRequest()
        {
            CurrentPclRequest = NewPCLReq();
            CurrentPclRequest.AgencyID = -1;
            ObjPatientPCLRequestNew_BackUp = new PatientPCLRequest();
        }

        private void InitLoadData(bool IsLoaded = false)
        {
            if (!IsLoaded)
            {
                PCLPCLExamTypeCombo_Search();
            }
            ObjV_PCLMainCategory = new ObservableCollection<Lookup>();
            ObjV_PCLMainCategory_Selected = new Lookup();
            ObjV_PCLMainCategory_Selected.LookupID = -1;
            LoadV_PCLMainCategory();

            //▼===== 20191017 TTM: Chuyển TestingAgency về hàm khởi tạo của view.
            //ObjTestingAgencyList = new ObservableCollection<TestingAgency>();
            //GetTestingAgency_All();
            //▲===== 20191017

            ObjPCLForms_GetList = new ObservableCollection<PCLForm>();
            ObjPCLForms_GetList_Selected = new PCLForm();


            ObjPCLItems_ByPCLFormID_Selected = new ObservableCollection<PCLExamType>();
            if (!IsLoaded)
            {
                SearchCriteriaDetail = new PatientPCLRequestDetailSearchCriteria();
                SearchCriteria = new PCLFormsSearchCriteria();
                SearchCriteria.V_PCLMainCategory = -1;
                SearchCriteria.OrderBy = "";

                SearchCriteriaExamTypeForChoose = new PCLExamTypeSearchCriteria();
            }
            HasGroup = true;

            SelectedItemForChoose = new PCLExamType();

        }

        private ObservableCollection<PrescriptionNoteTemplates> _allPrescriptionNoteTemplates;
        public ObservableCollection<PrescriptionNoteTemplates> allPrescriptionNoteTemplates
        {
            get { return _allPrescriptionNoteTemplates; }
            set
            {
                _allPrescriptionNoteTemplates = value;
                NotifyOfPropertyChange(() => allPrescriptionNoteTemplates);
            }
        }



        /// <summary> content control cho phieu yeu cau
        /// content list PCLExamtype
        /// </summary>
        private IPCLRequestDetails _pclRequestDetailsContent;
        public IPCLRequestDetails PCLRequestDetailsContent
        {
            get { return _pclRequestDetailsContent; }
            set
            {
                _pclRequestDetailsContent = value;
                NotifyOfPropertyChange(() => PCLRequestDetailsContent);
            }
        }

        /// <summary>Content control cho phieu hen can lam sang
        /// content PCLExamtype
        /// </summary>
        private IEditApptPclRequestDetailList _pclApptRequestDetailsContent;
        public IEditApptPclRequestDetailList pclApptRequestDetailsContent
        {
            get { return _pclApptRequestDetailsContent; }
            set
            {
                if (_pclApptRequestDetailsContent != value)
                {
                    _pclApptRequestDetailsContent = value;
                    _pclApptRequestDetailsContent.detailListChanged += new EventHandler(_pclApptRequestDetailsContent_detailListChanged);
                    NotifyOfPropertyChange(() => pclApptRequestDetailsContent);
                }
            }
        }

        void _pclApptRequestDetailsContent_detailListChanged(object sender, EventArgs e)
        {
            hasItem = (bool)sender;
        }

        private PatientPCLRequest _currentPclRequest;
        public PatientPCLRequest CurrentPclRequest
        {
            get
            {
                return _currentPclRequest;
            }
            set
            {
                if (_currentPclRequest != value)
                {
                    _currentPclRequest = value;
                    NotifyOfPropertyChange(() => CurrentPclRequest);
                    PCLRequestDetailsContent.PCLRequest = CurrentPclRequest;
                    DoctorComments = CurrentPclRequest.DoctorComments;
                    if (CurrentPclRequest != null)
                    {
                        //▼===== #008
                        //gMedicalInstructionDate = CurrentPclRequest.MedicalInstructionDate;
                        MedicalInstructionDateContent.DateTime = CurrentPclRequest.MedicalInstructionDate;
                        //▲===== #008
                    }
                    if (CurrentPclRequest != null && DoctorStaffs != null)
                    {
                        gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == CurrentPclRequest.DoctorStaffID);
                    }
                    //▼====== #003
                    if (CurrentPclRequest != null && CurrentPclRequest.PatientPCLReqID == 0)
                    {
                        gSelectedDoctorStaff = DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID);
                    }
                    //▲====== #003
                    //20190117 TTM: Chỉ có phiếu đã hoàn thành thì nút cập nhật sau chẩn đoán mới enable cho sử dụng
                    if (CurrentPclRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CLOSE)
                    {
                        mPCL_SuaSauHoanTat = true;
                    }
                    else
                    {
                        mPCL_SuaSauHoanTat = false;
                    }
                    NotifyOfPropertyChange(() => IsEnableMedicalInstructionDate);
                }
            }
        }

        private ObservableCollection<PatientPCLRequestDetail> _CurrentPclRequestDetail;
        public ObservableCollection<PatientPCLRequestDetail> CurrentPclRequestDetail
        {
            get
            {
                return _CurrentPclRequestDetail;
            }
            set
            {
                if (_CurrentPclRequestDetail != value)
                {
                    _CurrentPclRequestDetail = value;
                    NotifyOfPropertyChange(() => CurrentPclRequestDetail);
                }
            }
        }

        private bool _FormIsEnabled;
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

        private bool _FormInputIsEnabled;
        public bool FormInputIsEnabled
        {
            get { return _FormInputIsEnabled; }
            set
            {
                if (_FormInputIsEnabled != value)
                {
                    _FormInputIsEnabled = value;
                    NotifyOfPropertyChange(() => FormInputIsEnabled);
                }
            }
        }


        private bool _btEditIsEnabled;
        public bool btEditIsEnabled
        {
            get { return _btEditIsEnabled; }
            set
            {
                if (_btEditIsEnabled != value)
                {
                    _btEditIsEnabled = value;
                    NotifyOfPropertyChange(() => btEditIsEnabled);
                }
            }
        }

        private bool _btCancelIsEnabled;
        public bool btCancelIsEnabled
        {
            get { return _btCancelIsEnabled; }
            set
            {
                if (_btCancelIsEnabled != value)
                {
                    _btCancelIsEnabled = value;
                    NotifyOfPropertyChange(() => btCancelIsEnabled);
                }
            }
        }


        private bool _btSaveIsEnabled;
        public bool btSaveIsEnabled
        {
            get { return _btSaveIsEnabled; }
            set
            {
                if (_btSaveIsEnabled != value)
                {
                    _btSaveIsEnabled = value;
                    NotifyOfPropertyChange(() => btSaveIsEnabled);
                    NotifyOfPropertyChange(() => IsAllowToUpdateDiagnosis);
                }
            }
        }

        private bool _IsEnabledtxbDiagnosis;
        public bool IsEnabledtxbDiagnosis
        {
            get
            {
                return _IsEnabledtxbDiagnosis;
            }
            set
            {
                _IsEnabledtxbDiagnosis = value;
                NotifyOfPropertyChange(() => IsEnabledtxbDiagnosis);
            }
        }

        public void EnableTxbDiagnosis()
        {
            IsEnabledtxbDiagnosis = true;
        }


        public enum DataGridCol
        {
            SEL = 0,
            EXAM_CODE = 1,
            EXAM_TYPE_NAME = 2,
            QTY = 3,
            NormalPrice = 4,
            PriceForHIPatient = 5,
            PriceDifference = 4,
        }


        private decimal _Sum_NormalPrice;
        public decimal Sum_NormalPrice
        {
            get
            {
                return _Sum_NormalPrice;
            }
            set
            {
                if (_Sum_NormalPrice != value)
                {
                    _Sum_NormalPrice = value;
                    NotifyOfPropertyChange(() => Sum_NormalPrice);
                }
            }
        }

        private decimal _Sum_HIAllowedPrice;
        public decimal Sum_HIAllowedPrice
        {
            get
            {
                return _Sum_HIAllowedPrice;
            }
            set
            {
                if (_Sum_HIAllowedPrice != value)
                {
                    _Sum_HIAllowedPrice = value;
                    NotifyOfPropertyChange(() => Sum_HIAllowedPrice);
                }
            }
        }

        private decimal _Sum_PriceDifference;
        public decimal Sum_PriceDifference
        {
            get
            {
                return _Sum_PriceDifference;
            }
            set
            {
                if (_Sum_PriceDifference != value)
                {
                    _Sum_PriceDifference = value;
                    NotifyOfPropertyChange(() => Sum_PriceDifference);
                }
            }
        }

        private Staff _ObjStaff;
        public Staff ObjStaff
        {
            get { return _ObjStaff; }
            set
            {
                if (_ObjStaff != value)
                {
                    _ObjStaff = value;
                }
            }
        }

        //KMx: Sau khi kiểm tra, thấy biến này không được sử dụng nữa (25/05/2014 14:48).
        //public object UCHeaderInfoPMR
        //{
        //    get;
        //    set;
        //}
        public void LoadDiagnosisTreatmentAndFillDiagnosis(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var mFactory = new ePMRsServiceClient())
                {
                    var mContract = mFactory.ServiceInstance;
                    mContract.BeginGetDiagnosisTreatment_InPt_ByPtRegID(PtRegistrationID, null, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mReturnCollection = mContract.EndGetDiagnosisTreatment_InPt_ByPtRegID(asyncResult);
                            if (mReturnCollection != null && mReturnCollection.Count > 0)
                            {
                                ObjGetDiagnosisTreatmentByPtID = mReturnCollection.OrderBy(x => x.DTItemID).LastOrDefault(x => Globals.DeptLocation != null && Globals.DeptLocation.DeptID > 0 && x.Department != null && x.Department.DeptID == Globals.DeptLocation.DeptID);
                                if (ObjGetDiagnosisTreatmentByPtID == null)
                                {
                                    ObjGetDiagnosisTreatmentByPtID = mReturnCollection.OrderBy(x => x.DTItemID).LastOrDefault();
                                }
                                if (CurrentPclRequest != null && CurrentPclRequest.PatientPCLReqID == 0)
                                {
                                    CurrentPclRequest.Diagnosis = ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal;
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
        //Phiếu cuối
        private void PatientPCLRequest_RequestLastest()
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0534_G1_PhGanNhat) });
            var t = new Thread(() =>
            {
                IsWaitingLoadPCLInfo = true;
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        CurrentPclRequest = NewPCLReq();
                        SearchCriteriaDetail = new PatientPCLRequestDetailSearchCriteria();
                        client.BeginPatientPCLRequest_RequestLastest(Registration_DataStorage.CurrentPatient.PatientID, V_RegistrationType, (long)AllLookupValues.V_PCLMainCategory.Laboratory, Globals.DispatchCallback((asyncResult) =>
                        {
                            var PCLReqLast = client.EndPatientPCLRequest_RequestLastest(asyncResult);

                            if (PCLReqLast != null && PCLReqLast.PatientPCLReqID > 0)
                            {
                                CurrentPclRequest = PCLReqLast;

                                SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                                PatientPCLRequestDetail_ByPatientPCLReqID();
                            }

                            ObjPatientPCLRequestNew_BackUp = ObjectCopier.DeepCopy(CurrentPclRequest);

                            FormInputIsEnabled = false;
                            PCLRequestDetailsContent.IsEnableListPCL = false;
                            if (Registration_DataStorage.CurrentPatientRegistration != null &&
                                Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0 &&
                                Globals.ServerConfigSection.ConsultationElements.UseOnlyDailyDiagnosis &&
                                Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType != AllLookupValues.RegistrationType.NGOAI_TRU)
                            {
                                LoadDiagnosisTreatmentAndFillDiagnosis(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                            }
                            if (CurrentPclRequest != null && CurrentPclRequest.PatientPCLReqID > 0)
                            {
                                StatusForm(AllLookupValues.StatusForm.HIEUCHINH);
                            }
                            else
                            {
                                FormInputIsEnabled = false;
                                PCLRequestDetailsContent.IsEnableListPCL = false;
                                //▼===== #008
                                //gMedicalInstructionDate = Globals.GetCurServerDateTime().Date;
                                MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
                                //▲===== #008
                                StatusForm(AllLookupValues.StatusForm.XEM);
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsWaitingLoadPCLInfo = false;
                }
            });
            t.Start();
        }

        private ObservableCollection<DeptLocation> _ObjGetAllLocationsByDeptID;
        public ObservableCollection<DeptLocation> ObjGetAllLocationsByDeptID
        {
            get
            {
                return _ObjGetAllLocationsByDeptID;
            }
            set
            {
                _ObjGetAllLocationsByDeptID = value;
                NotifyOfPropertyChange(() => ObjGetAllLocationsByDeptID);
            }
        }

        private IEnumerator<IResult> DoLoadLocations(long deptId)
        {
            var deptLoc = new LoadDeptLoctionByIDTask(deptId);
            yield return deptLoc;
            if (deptLoc.DeptLocations != null)
            {
                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>(deptLoc.DeptLocations);
            }
            else
            {
                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
            }

            var itemDefault = new DeptLocation();
            itemDefault.Location = new Location();
            itemDefault.Location.LID = -1;
            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
            ObjGetAllLocationsByDeptID.Insert(0, itemDefault);

            yield break;
        }

        public void GetAllLocationsByDeptID(long? deptId)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3054_G1_DSPg) });

            var t = new Thread(() =>
            {
                IsWaitingLoadPhong = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllLocationsByDeptIDOld(deptId, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var allItems = contract.EndGetAllLocationsByDeptIDOld(asyncResult);

                            if (allItems != null)
                            {
                                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>(allItems);
                            }
                            else
                            {
                                ObjGetAllLocationsByDeptID = new ObservableCollection<DeptLocation>();
                            }

                            var itemDefault = new DeptLocation();
                            itemDefault.Location = new Location();
                            itemDefault.Location.LID = -1;
                            itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                            ObjGetAllLocationsByDeptID.Insert(0, itemDefault);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingLoadPhong = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private PCLFormsSearchCriteria _SearchCriteria = new PCLFormsSearchCriteria();
        public PCLFormsSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);

            }
        }

        private DataEntities.PCLForm _ObjPCLForms_GetList_Selected;
        public DataEntities.PCLForm ObjPCLForms_GetList_Selected
        {
            get { return _ObjPCLForms_GetList_Selected; }
            set
            {
                _ObjPCLForms_GetList_Selected = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList_Selected);
            }
        }

        private ObservableCollection<DataEntities.PCLForm> _ObjPCLForms_GetList;
        public ObservableCollection<DataEntities.PCLForm> ObjPCLForms_GetList
        {
            get { return _ObjPCLForms_GetList; }
            set
            {
                _ObjPCLForms_GetList = value;
                NotifyOfPropertyChange(() => ObjPCLForms_GetList);
            }
        }

        private Visibility _VisibilityTemplate = Visibility.Collapsed;
        public Visibility VisibilityTemplate
        {
            get { return _VisibilityTemplate; }
            set
            {
                _VisibilityTemplate = value;
                NotifyOfPropertyChange(() => VisibilityTemplate);
            }
        }
        private Visibility _VisibilityFrom = Visibility.Visible;
        public Visibility VisibilityFrom
        {
            get { return _VisibilityFrom; }
            set
            {
                _VisibilityFrom = value;
                NotifyOfPropertyChange(() => VisibilityFrom);
            }
        }
        private Visibility _VisibilityTestingAgency = Visibility.Collapsed;
        public Visibility VisibilityTestingAgency
        {
            get { return _VisibilityTestingAgency; }
            set
            {
                _VisibilityTestingAgency = value;
                NotifyOfPropertyChange(() => VisibilityTestingAgency);
            }
        }
        private void SetVisibility()
        {
            if (ObjV_PCLMainCategory_Selected.LookupID == IDLABByName_Bo)
            {
                VisibilityFrom = Visibility.Collapsed;
                VisibilityTemplate = Visibility.Visible;
                VisibilityTestingAgency = Visibility.Collapsed;
            }
            else if (ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory_External)
            {
                VisibilityFrom = Visibility.Collapsed;
                VisibilityTemplate = Visibility.Collapsed;
                VisibilityTestingAgency = Visibility.Visible;
            }
            else
            {
                VisibilityFrom = Visibility.Visible;
                VisibilityTemplate = Visibility.Collapsed;
                VisibilityTestingAgency = Visibility.Collapsed;
            }
        }

        private void PCLForm_GetList(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPCLForms_GetList.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3022_G1_DSPCLForms) });

            var t = new Thread(() =>
            {
                IsLoadingPCLForms = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLForms_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;

                            var allItems = client.EndPCLForms_GetList_Paging(out Total, asyncResult);
                            if (allItems != null)
                            {
                                ObjPCLForms_GetList = new ObservableCollection<DataEntities.PCLForm>(allItems);

                                //ItemDefault
                                DataEntities.PCLForm ItemDefault = new DataEntities.PCLForm();
                                ItemDefault.PCLFormID = -1;
                                if (ObjV_PCLMainCategory_Selected != null)
                                {
                                    //if (ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Imaging)
                                    //{
                                    //    ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0350_G1_ChonCDoanHATDCN);
                                    //}
                                    //else 
                                    if (ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                                    {
                                        ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z1117_G1_ChonNhomXN);
                                    }
                                    else if (ObjV_PCLMainCategory_Selected.LookupID == IDLABByName_Bo)
                                    {
                                        ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0186_G1_ChonComboXN);
                                    }
                                    else
                                    {
                                        ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                    }
                                }
                                else
                                {
                                    ItemDefault.PCLFormName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                }
                                ObjPCLForms_GetList.Insert(0, ItemDefault);
                                //ItemDefault

                                ObjPCLForms_GetList_Selected = ItemDefault;
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
                    //Globals.IsBusy = false;
                    IsLoadingPCLForms = false;
                }
            });
            t.Start();
        }


        //Main
        private Lookup _ObjV_PCLMainCategory_Selected;
        public Lookup ObjV_PCLMainCategory_Selected
        {
            get { return _ObjV_PCLMainCategory_Selected; }
            set
            {
                if (_ObjV_PCLMainCategory_Selected == value)
                {
                    return;
                }
                _ObjV_PCLMainCategory_Selected = value;
                if (ObjPCLForms_GetList != null && ObjPCLForms_GetList.Count > 0)
                {
                    selectedObjPCLForms = 0;
                }
                if (PCLExamTypeCombos != null && PCLExamTypeCombos.Count > 0)
                {
                    selectedComboIndex = 0;
                }

                if (ObjTestingAgencyList != null && ObjTestingAgencyList.Count > 0)
                {
                    selectedTestingAgency = 0;
                }

                NotifyOfPropertyChange(() => ObjV_PCLMainCategory_Selected);


                if (_ObjV_PCLMainCategory_Selected != null && _ObjV_PCLMainCategory_Selected.LookupID > 0)
                {
                    SearchCriteria.V_PCLMainCategory = _ObjV_PCLMainCategory_Selected.LookupID;

                    ObjPCLItems_ByPCLFormID = null;
                    SelectedItemForChoose = new PCLExamType();

                    PCLForm_GetList(0, 99999, true);//ko phân trang

                    SetVisibility();
                    if (SPTheoAutoComplete != null)
                    {
                        if (_ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory_External)
                        {

                            SPTheoAutoComplete.SearchCriteria.IsExternalExam = true;
                        }
                        else
                        {
                            SPTheoAutoComplete.SearchCriteria.IsExternalExam = false;
                        }
                    }
                }
                else
                {
                    if (SPTheoAutoComplete != null)
                    {
                        SPTheoAutoComplete.SearchCriteria.IsExternalExam = null;
                    }
                }

                NotifyOfPropertyChange(() => cboPCLFormIsEnabled);

                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);

                EffectBoNut();
            }
        }

        private ObservableCollection<Lookup> _ObjV_PCLMainCategory;
        public ObservableCollection<Lookup> ObjV_PCLMainCategory
        {
            get { return _ObjV_PCLMainCategory; }
            set
            {
                _ObjV_PCLMainCategory = value;
                NotifyOfPropertyChange(() => ObjV_PCLMainCategory);
            }
        }

        private ObservableCollection<PCLExamTypeCombo> _PCLExamTypeCombos;
        public ObservableCollection<PCLExamTypeCombo> PCLExamTypeCombos
        {
            get { return _PCLExamTypeCombos; }
            set
            {
                _PCLExamTypeCombos = value;
                NotifyOfPropertyChange(() => PCLExamTypeCombos);
            }
        }

        private int _selectedComboIndex = -1;
        public int selectedComboIndex
        {
            get { return _selectedComboIndex; }
            set
            {
                _selectedComboIndex = value;
                NotifyOfPropertyChange(() => selectedComboIndex);
            }
        }

        private int _selectedTestingAgency = -1;
        public int selectedTestingAgency
        {
            get { return _selectedTestingAgency; }
            set
            {
                _selectedTestingAgency = value;
                NotifyOfPropertyChange(() => selectedTestingAgency);
            }
        }

        private int _selectedObjPCLForms = -1;
        public int selectedObjPCLForms
        {
            get { return _selectedObjPCLForms; }
            set
            {
                _selectedObjPCLForms = value;
                NotifyOfPropertyChange(() => selectedObjPCLForms);
            }
        }

        private void PCLPCLExamTypeCombo_Search()
        {
            var t = new Thread(() =>
            {
                //IsLoading = true;

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeCombo_Search(new GeneralSearchCriteria(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndPCLExamTypeCombo_Search(asyncResult);
                                PCLExamTypeCombos = allItems.ToObservableCollection();
                                DataEntities.PCLExamTypeCombo ItemDefault = new DataEntities.PCLExamTypeCombo();
                                ItemDefault.PCLExamTypeComboID = -1;

                                ItemDefault.ComboName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0186_G1_ChonComboXN);
                                PCLExamTypeCombos.Insert(0, ItemDefault);
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                            }

                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        /// <summary>
        /// QMS Service
        /// Update order number
        /// Author: BaoLQ, VuTTM
        /// </summary>
        /// <param name="PtRegDetailID"></param>
        /// <param name="PCLReqItemID"></param>
        /// <param name="orderDTO"></param>
        //private void UpdateOrderNumber(long PtRegDetailID, long PCLReqItemID, OrderDTO orderDTO,
        //    long PrescriptID = 0)
        //{
        //    if ((0 == PtRegDetailID && 0 == PCLReqItemID) || null == orderDTO || 0 == orderDTO.orderNumber)
        //    {
        //        MessageBox.Show("Không thể cấp số thứ tự!", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //        return;
        //    }

        //    var t = new Thread(() =>
        //    {
        //        using (var mFactory = new CommonService_V2Client())
        //        {
        //            try
        //            {
        //                var mContract = mFactory.ServiceInstance;
        //                mContract.BeginUpdateOrderNumberRegistration(PtRegDetailID, PCLReqItemID, PrescriptID, orderDTO.orderNumber.Value, orderDTO.roomId.Value,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            bool IsOK = mContract.EndUpdateOrderNumberRegistration(asyncResult);
        //                            if (IsOK)
        //                            {
        //                                MessageBox.Show(String.Format("Số thứ tự của bệnh nhân là {0}.", orderDTO.orderNumber),
        //                                    eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                        }
        //                    }), null);
        //            }
        //            catch (Exception ex)
        //            {
        //                Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
        //            }
        //        }
        //    });
        //    t.Start();
        //}

        public void LoadV_PCLMainCategory()
        {
            List<Lookup> ObjList = new List<Lookup>();

            Lookup Item0 = new Lookup();
            Item0.LookupID = -1;/**/
            Item0.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2034_G1_ChonLoai2);
            ObjList.Add(Item0);

            //Lookup Item1 = new Lookup();
            //Item1.LookupID = (long)AllLookupValues.V_PCLMainCategory.Imaging;
            //Item1.ObjectValue = eHCMSResources.Z0351_G1_CDoanHAVaTDCN;// "Imaging";
            //ObjList.Add(Item1);

            Lookup Item5 = new Lookup();
            Item5.LookupID = IDLABByName_Bo;
            Item5.ObjectValue = eHCMSResources.Z0182_G1_XNTheoCombo;// "Laboratory(theo Form)";
            ObjList.Add(Item5);


            Lookup Item2 = new Lookup();
            Item2.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            Item2.ObjectValue = eHCMSResources.Z0184_G1_XNTheoNhom;// "Laboratory(theo Form)";
            ObjList.Add(Item2);

            Lookup Item4 = new Lookup();
            Item4.LookupID = (long)AllLookupValues.V_PCLMainCategory.Laboratory_External;
            Item4.ObjectValue = eHCMSResources.Z1165_G1_XNNgoaiVien;
            ObjList.Add(Item4);

            ObjV_PCLMainCategory = new ObservableCollection<Lookup>(ObjList);
        }
        //Main

        public void cboV_PCLMainCategory_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                Lookup Objtmp = (selectItem as Lookup);

                if (Objtmp != null)
                {
                    SearchCriteria.V_PCLMainCategory = Objtmp.LookupID;

                    ObjPCLItems_ByPCLFormID = null;
                    SelectedItemForChoose = new PCLExamType();

                    PCLForm_GetList(0, 99999, true);//ko phân trang
                }
            }
        }

        public void cboPCLForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                PCLForm Objtmp = ((sender as ComboBox).SelectedItem as PCLForm);
                if (Objtmp.PCLFormID > 0)
                {
                    //PCLItems_ByPCLFormID_HasGroup(HasGroup);
                    PCLItemsByPCLFormID_Lingq(Objtmp.PCLFormID, HasGroup);
                }
            }
        }

        public void PCLExamTypeCombos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                PCLExamTypeCombo Objtmp = ((sender as ComboBox).SelectedItem as PCLExamTypeCombo);
                if (Objtmp != null && Objtmp.PCLExamTypeComboID > 0)
                {
                    //lay danh sach Examtype add vao
                    //PCLExamType_ByComboID(Objtmp.PCLExamTypeComboID, HasGroup);
                    PCLExamTypeByComboIDLingq(Objtmp.PCLExamTypeComboID, HasGroup);
                }
            }
        }


        private PCLExamTypeSearchCriteria _SearchCriteriaExamTypeForChoose;
        public PCLExamTypeSearchCriteria SearchCriteriaExamTypeForChoose
        {
            get
            {
                return _SearchCriteriaExamTypeForChoose;
            }
            set
            {
                _SearchCriteriaExamTypeForChoose = value;
                NotifyOfPropertyChange(() => SearchCriteriaExamTypeForChoose);

            }
        }

        private PagedCollectionView _ObjPCLItems_ByPCLFormID;
        public PagedCollectionView ObjPCLItems_ByPCLFormID
        {
            get { return _ObjPCLItems_ByPCLFormID; }
            set
            {
                _ObjPCLItems_ByPCLFormID = value;
                NotifyOfPropertyChange(() => ObjPCLItems_ByPCLFormID);
            }
        }

        private bool _HasGroup;
        public bool HasGroup
        {
            get { return _HasGroup; }
            set
            {
                if (_HasGroup != value)
                {
                    _HasGroup = value;
                    NotifyOfPropertyChange(() => HasGroup);
                }
            }
        }

        public void PCLExamTypeByComboIDLingq(long ID, bool isGroup)
        {
            //KMx: Lưu ý quan trọng, phải để DeepCopy().
            //Nếu không để DeepCopy(), khi thay đổi giá trị trong var items thì Globals.ListPclExamTypesAllCombos sẽ bị thay đổi theo (tham chiếu).
            //Trường hợp Khám cho BN không BH, chỉ định CLS, sau đó khám cho BN có BH chỉ định CLS thì BN BH ko được hưởng BH.
            var allItems = CurrentListPclExamTypesAllCombos.Where(o => o.PCLExamTypeComboID == ID).Select(o => o.PCLExamType).Where(o => !o.IsRegimenChecking || !IsRegimenChecked || CS_DS == null || CS_DS.TreatmentRegimenCollection == null
                || CS_DS.TreatmentRegimenCollection.Count == 0
                || !CS_DS.TreatmentRegimenCollection.Any(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0)
                || !CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).Any(x => x.RefTreatmentRegimenPCLDetails.Count > 0)
                || CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).SelectMany(x => x.RefTreatmentRegimenPCLDetails).Any(x => x.PCLExamTypeID == o.PCLExamTypeID)).ToList().DeepCopy();

            if (allItems != null)
            {
                //TinhGiaChoBenhNhanBaoHiem(allItems);

                ObjPCLItems_ByPCLFormID = new PagedCollectionView(allItems);

                if (isGroup)
                {
                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                }
            }
            else
            {
                ObjPCLItems_ByPCLFormID = null;
            }

            EffectBoNut();

        }

        //KMx: Sau khi kiểm tra, thấy hàm này không được sử dụng nữa (26/04/2014 17:00)
        public void PCLExamType_ByComboID(long ID, bool isGroup)
        {
            var t = new Thread(() =>
            {
                IsLoadingPCLForms = true;
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPCLExamType_byComboID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndGetPCLExamType_byComboID(asyncResult);
                                if (allItems != null)
                                {
                                    TinhGiaChoBenhNhanBaoHiem(allItems);

                                    ObjPCLItems_ByPCLFormID = new PagedCollectionView(allItems);

                                    if (isGroup)
                                    {
                                        ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                                    }
                                }
                                else
                                {
                                    ObjPCLItems_ByPCLFormID = null;
                                }

                                EffectBoNut();

                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoadingPCLForms = false;
                }
            });
            t.Start();
        }

        public void PCLExamType_ByHosID(long ID, bool isGroup)
        {
            //KMx: Lưu ý quan trọng, phải để DeepCopy().
            //Nếu không để DeepCopy(), khi thay đổi giá trị trong var items thì Globals.ListPclExamTypesAllPCLForms sẽ bị thay đổi theo (tham chiếu).
            //Trường hợp Khám cho BN không BH, chỉ định CLS, sau đó khám cho BN có BH chỉ định CLS thì BN BH ko được hưởng BH.
            var allItems = CurrentListPclExamTypesAllPCLForms.Where(o => o.IsExternalExam.Value && o.HosIDofExternalExam != null && o.HosIDofExternalExam.Value == ID).ToList().DeepCopy();
            if (allItems != null)
            {
                //TinhGiaChoBenhNhanBaoHiem(allItems);
                ObjPCLItems_ByPCLFormID = new PagedCollectionView(allItems);
                if (isGroup)
                {
                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                }
            }
            else
            {
                ObjPCLItems_ByPCLFormID = null;
            }
            EffectBoNut();
        }

        //public void PCLExamType_ByHosID(long ID, bool isGroup)
        //{
        //    var t = new Thread(() =>
        //    {
        //        IsLoadingPCLForms = true;

        //        try
        //        {
        //            using (var serviceFactory = new ConfigurationManagerServiceClient())
        //            {
        //                var client = serviceFactory.ServiceInstance;
        //                client.BeginGetPCLExamType_byHosID(ID, Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        var allItems = client.EndGetPCLExamType_byHosID(asyncResult);
        //                        if (allItems != null)
        //                        {
        //                            TinhGiaChoBenhNhanBaoHiem(allItems);

        //                            ObjPCLItems_ByPCLFormID = new PagedCollectionView(allItems);

        //                            if (isGroup)
        //                            {
        //                                ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new PropertyGroupDescription("PCLSectionName"));
        //                            }
        //                        }
        //                        else
        //                        {
        //                            ObjPCLItems_ByPCLFormID = null;
        //                        }

        //                        EffectBoNut();
        //                        //if (allItems != null && allItems.Count > 0)
        //                        //{
        //                        //    if (IsAppointment)
        //                        //    {
        //                        //        foreach (PCLExamType p in allItems)
        //                        //        {
        //                        //            AddOnePCLAppt(p);
        //                        //        }
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        foreach (PCLExamType p in allItems)
        //                        //        {
        //                        //            AddOnePCLRequest(p);
        //                        //        }
        //                        //    }
        //                        //}

        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {

        //                    }
        //                    catch (Exception ex)
        //                    { }
        //                }), null)
        //                    ;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //        }
        //        finally
        //        {
        //            //Globals.IsBusy = false;
        //            IsLoadingPCLForms = false;
        //        }
        //    });
        //    t.Start();
        //}

        private void PCLItemsByPCLFormID_Lingq(long FormID, bool isGroup)
        {
            //KMx: Lưu ý quan trọng, phải để DeepCopy().
            //Nếu không để DeepCopy(), khi thay đổi giá trị trong var items thì Globals.ListPclExamTypesAllPCLForms sẽ bị thay đổi theo (tham chiếu).
            //Trường hợp Khám cho BN không BH, chỉ định CLS, sau đó khám cho BN có BH chỉ định CLS thì BN BH ko được hưởng BH.
            var items = CurrentListPclExamTypesAllPCLForms.Where(o => o.PCLFormID == FormID && (!o.IsRegimenChecking || !IsRegimenChecked || CS_DS == null || CS_DS.TreatmentRegimenCollection == null
                    || CS_DS.TreatmentRegimenCollection.Count == 0
                    || !CS_DS.TreatmentRegimenCollection.Any(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0)
                    || !CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).Any(x => x.RefTreatmentRegimenPCLDetails.Count > 0)
                    || CS_DS.TreatmentRegimenCollection.Where(x => x.RefTreatmentRegimenPCLDetails != null && x.RefTreatmentRegimenPCLDetails.Count > 0).SelectMany(x => x.RefTreatmentRegimenPCLDetails).Any(x => x.PCLExamTypeID == o.PCLExamTypeID))).ToList().DeepCopy();
            if (items != null)
            {
                //TinhGiaChoBenhNhanBaoHiem(items);

                ObjPCLItems_ByPCLFormID = new PagedCollectionView(items);

                if (isGroup)
                {
                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                }
            }
            else
            {
                ObjPCLItems_ByPCLFormID = null;
            }

            EffectBoNut();

        }

        private void PCLItems_ByPCLFormID_HasGroup(bool isGroup)
        {

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.K3014_G1_DSPCLExamType) });

            var t = new Thread(() =>
            {
                IsWaitingLoadPCLExamType = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLItems_ByPCLFormID(SearchCriteriaExamTypeForChoose, ObjPCLForms_GetList_Selected.PCLFormID, null, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLItems_ByPCLFormID(asyncResult);

                            if (items != null)
                            {
                                TinhGiaChoBenhNhanBaoHiem(items);

                                ObjPCLItems_ByPCLFormID = new PagedCollectionView(items);

                                if (isGroup)
                                {
                                    ObjPCLItems_ByPCLFormID.GroupDescriptions.Add(new Common.PagedCollectionView.PropertyGroupDescription("PCLSectionName"));
                                }
                            }
                            else
                            {
                                ObjPCLItems_ByPCLFormID = null;
                            }

                            EffectBoNut();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsWaitingLoadPCLExamType = false;
                        }
                    }), null);
                }


            });
            t.Start();

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGrid dtgList = (sender as DataGrid);
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        #region "CalcPatientHI"
        private void TinhGiaChoBenhNhanBaoHiem(List<PCLExamType> ObjList)
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatientRegistration.HisID.GetValueOrDefault(0) > 0)
            {
                foreach (var pclExamType in ObjList)
                {
                    pclExamType.NormalPrice = pclExamType.HIPatientPrice;
                }
            }
            //else
            //{
            //    foreach (var pclExamType in ObjList)
            //    {
            //        pclExamType.HIAllowedPrice = 0;
            //    }
            //}
        }
        #endregion


        //Khởi tạo đối tượng

        private PatientPCLRequest _ObjPatientPCLRequestNew_BackUp;
        public PatientPCLRequest ObjPatientPCLRequestNew_BackUp
        {
            get
            {
                return _ObjPatientPCLRequestNew_BackUp;
            }
            set
            {
                if (_ObjPatientPCLRequestNew_BackUp != value)
                {
                    _ObjPatientPCLRequestNew_BackUp = value;
                    NotifyOfPropertyChange(() => ObjPatientPCLRequestNew_BackUp);
                }
            }
        }

        //private PatientPCLRequest _ObjPatientPCLRequestNew;
        //public PatientPCLRequest ObjPatientPCLRequestNew
        //{
        //    get
        //    {
        //        return _ObjPatientPCLRequestNew;
        //    }
        //    set
        //    {
        //        if (_ObjPatientPCLRequestNew != value)
        //        {
        //            _ObjPatientPCLRequestNew = value;
        //            NotifyOfPropertyChange(() => ObjPatientPCLRequestNew);
        //            if (_ObjPatientPCLRequestNew != null)
        //            {
        //                DoctorComments = _ObjPatientPCLRequestNew.DoctorComments;
        //            }
        //        }
        //    }
        //}


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }


            mPCL_XemSuaPhieuYeuCau_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_ChinhSua, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_Huy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_Huy, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_XemIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_XemIn, (int)ePermission.mView);
            mPCL_XemSuaPhieuYeuCau_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtPCLRequest,
                                               (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_In, (int)ePermission.mView);
        }
        #region checking account

        private bool _mPCL_XemSuaPhieuYeuCau_ChinhSua = true;
        private bool _mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi = true;
        private bool _mPCL_XemSuaPhieuYeuCau_Huy = true;
        private bool _mPCL_XemSuaPhieuYeuCau_XemIn = true;
        private bool _mPCL_XemSuaPhieuYeuCau_In = true;

        public bool mPCL_XemSuaPhieuYeuCau_ChinhSua
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_ChinhSua;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_ChinhSua == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_ChinhSua = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_ChinhSua);
                if (!mPCL_XemSuaPhieuYeuCau_ChinhSua)
                {
                    btCancelIsEnabled = false;
                }
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_TaoPhieuMoi);
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_Huy
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_Huy;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_Huy == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_Huy = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_Huy);
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_XemIn
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_XemIn;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_XemIn == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_XemIn = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_XemIn);
            }
        }


        public bool mPCL_XemSuaPhieuYeuCau_In
        {
            get
            {
                return _mPCL_XemSuaPhieuYeuCau_In;
            }
            set
            {
                if (_mPCL_XemSuaPhieuYeuCau_In == value)
                    return;
                _mPCL_XemSuaPhieuYeuCau_In = value;
                NotifyOfPropertyChange(() => mPCL_XemSuaPhieuYeuCau_In);
            }
        }





        #endregion

        #region 3 Nút Add

        private PCLExamType _SelectedItemForChoose;
        public PCLExamType SelectedItemForChoose
        {
            get { return _SelectedItemForChoose; }
            set
            {
                _SelectedItemForChoose = value;
                NotifyOfPropertyChange(() => SelectedItemForChoose);
            }
        }

        private ObservableCollection<PCLExamType> _ObjPCLItems_ByPCLFormID_Selected;
        public ObservableCollection<PCLExamType> ObjPCLItems_ByPCLFormID_Selected
        {
            get { return _ObjPCLItems_ByPCLFormID_Selected; }
            set
            {
                _ObjPCLItems_ByPCLFormID_Selected = value;
                NotifyOfPropertyChange(() => ObjPCLItems_ByPCLFormID_Selected);
            }
        }

        private PatientPCLRequestDetailSearchCriteria _SearchCriteriaDetail;
        public PatientPCLRequestDetailSearchCriteria SearchCriteriaDetail
        {
            get
            {
                return _SearchCriteriaDetail;
            }
            set
            {
                if (_SearchCriteriaDetail != value)
                {
                    _SearchCriteriaDetail = value;
                    NotifyOfPropertyChange(() => SearchCriteriaDetail);
                }

            }
        }

        public void PatientPCLRequestDetail_ByPatientPCLReqID()
        {
            ObjPCLItems_ByPCLFormID_Selected.Clear();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0531_G1_DSLoaiXN) });

            var t = new Thread(() =>
            {
                IsWaitingLoadDetailPCLRequest = true;

                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPatientPCLRequestDetail_ByPatientPCLReqID(SearchCriteriaDetail, V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPatientPCLRequestDetail_ByPatientPCLReqID(asyncResult);

                            if (items != null)
                            {
                                CurrentPclRequest.PatientPCLRequestIndicators = items.ToObservableCollection();

                                PCLRequestDetailsContent.PCLRequest = CurrentPclRequest;
                                TinhTongTienDuTru();
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingLoadDetailPCLRequest = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void PatientPCLRequestDetail_ByPtRegistrationID(long PtRegistrationID)
        {
            CurrentPclRequestDetail = new ObservableCollection<PatientPCLRequestDetail>();
            ObjPCLItems_ByPCLFormID_Selected.Clear();
            var t = new Thread(() =>
            {
                IsWaitingLoadDetailPCLRequest = true;
                using (var serviceFactory = new PCLsClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPatientPCLRequestDetail_ByPtRegistrationID(PtRegistrationID, V_RegistrationType, (long)AllLookupValues.V_PCLMainCategory.Laboratory, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPatientPCLRequestDetail_ByPtRegistrationID(asyncResult);
                            if (items != null)
                            {
                                CurrentPclRequestDetail = items.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                            IsWaitingLoadDetailPCLRequest = false;
                        }
                    }), null);
                }
            });
            t.Start();
        }

        /// <summary>
        /// Tra ve true neu examType nay da ton tai.
        /// </summary>
        /// <param name="examType"></param>
        /// <returns></returns>
        private bool AddToCurrentPclRequestIfNotExists(PCLExamType examType)
        {
            bool bExists = false;
            if (CurrentPclRequest.PatientPCLRequestIndicators != null
                && CurrentPclRequest.PatientPCLRequestIndicators.Count > 0)
            {
                foreach (var requestDetail in CurrentPclRequest.PatientPCLRequestIndicators)
                {
                    if (requestDetail.PCLExamType.PCLExamTypeID == examType.PCLExamTypeID)
                    {
                        bExists = true;
                        break;
                    }
                }
            }
            else
            {
                CurrentPclRequest.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
            }

            //20190311 TBL: Do con kiem tra CLS nay co trung trong dang ky hay khong nen chua them lien
            //if (!bExists)
            //{
            //    //Lấy ds phòng PCLExamType lên
            //    AddPCLDetail(examType);
            //}
            return bExists;
        }

        private bool AddToCurrentPclRequestIfNotExistReq(PCLExamType examType)
        {
            bool bExists = false;
            if (CurrentPclRequestDetail == null)
            {
                return bExists;
            }
            if (CurrentPclRequestDetail != null && CurrentPclRequestDetail.Count > 0)
            {
                foreach (var requestDetail in CurrentPclRequestDetail)
                {
                    if (requestDetail.PCLExamType.PCLExamTypeID == examType.PCLExamTypeID)
                    {
                        bExists = true;
                        break;
                    }
                }
            }
            return bExists;
        }

        private bool AddToCurrentPclApptIfNotExists(PCLExamType examType)
        {
            if (EditingApptPCLRequest == null)
            {
                return false;
            }
            if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList == null)
            {
                EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList = new ObservableCollection<PatientApptPCLRequestDetails>();
            }
            var existingItem = EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.FirstOrDefault(item => item.ObjPCLExamTypes.PCLExamTypeID == examType.PCLExamTypeID && item.EntityState != EntityState.DELETED_MODIFIED);

            if (existingItem != null)
            {
                //TODO:Tu tu lam.    
            }
            else
            {
                ListDeptLocationByPCLExamTypeID(examType);
            }

            return existingItem != null;

        }

        private void AddPCLDetail(PCLExamType examType)
        {
            var requestDetail = new PatientPCLRequestDetail();
            requestDetail.PCLExamType = examType;
            requestDetail.NumberOfTest = 1;
            requestDetail.InvoicePrice = examType.NormalPrice;
            requestDetail.V_ExamRegStatus = (long)AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            requestDetail.IsCountPatient = true;
            requestDetail.IsCountHI = ((Registration_DataStorage.CurrentPatientRegistration.HisID > 0) && (bool)examType.HIApproved) ? true : false;
            requestDetail.EntityState = EntityState.DETACHED;
            requestDetail.RecordState = RecordState.DETACHED;
            //20181212 TNHX [BM0005404] Request was create by Doctor
            requestDetail.RequestedByDoctor = 1;
            requestDetail.HIAllowedPrice = examType.HIAllowedPrice > 0 ? examType.HIAllowedPrice : 0;
            //requestDetail.DeptLocation = new DeptLocation();
            //requestDetail.DeptLocation.DeptLocationID = 0;

            //▼===== #009,#010
            if (gSelectedDoctorStaff != null)
            {
                requestDetail.DoctorStaff = gSelectedDoctorStaff;
                requestDetail.DoctorStaff.StaffID = (long)gSelectedDoctorStaff.StaffID;
            }
            requestDetail.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
            //▲===== #009,#010
            //▼====: #011
            requestDetail.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            //▲====: #011

            examType.ObjDeptLocationList = CurrentListPclExamTypesAllPCLForms.Where
                    (o => o.PCLExamTypeID == examType.PCLExamTypeID).FirstOrDefault().PCLExamTypeLocations.Select(i => i.DeptLocation).ToObservableCollection();
            if (examType.ObjDeptLocationList != null && examType.ObjDeptLocationList.Count > 0)
            {
                requestDetail.DeptLocation = examType.ObjDeptLocationList.Count > 1 ?
                    new DeptLocation
                    {
                        DeptLocationID = 0,
                        Location = new Location { LID = 0, LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg) }
                    } : examType.ObjDeptLocationList.FirstOrDefault();
            }
            else
            {
                requestDetail.DeptLocation = new DeptLocation();
            }

            requestDetail.PatientPCLRequest = CurrentPclRequest;

            CurrentPclRequest.PatientPCLRequestIndicators.Add(requestDetail);
            PCLRequestDetailsContent.AddItemToView(requestDetail);
        }


        private void EffectBoNut()
        {
            NotifyOfPropertyChange(() => CanbtAdd);
            //NotifyOfPropertyChange(() => CanbtAddAll);
            NotifyOfPropertyChange(() => OneClick);
            NotifyOfPropertyChange(() => CanbtSubtractAll);
        }

        //public bool CanbtAddAll
        //{
        //    get
        //    {
        //        if (ObjPCLItems_ByPCLFormID == null || ObjPCLItems_ByPCLFormID.Count <= 0)
        //            return false;
        //        return true && OneClick;
        //    }
        //}

        public bool CanbtAdd
        {
            get
            {
                if (ObjPCLItems_ByPCLFormID == null || ObjPCLItems_ByPCLFormID.Count <= 0)
                    return false;
                return true;
            }
        }

        public bool CanbtSubtractAll
        {
            get
            {
                if (CurrentPclRequest == null || CurrentPclRequest.PatientPCLRequestIndicators == null || CurrentPclRequest.PatientPCLRequestIndicators.Count < 0)
                    return false;
                return true;
            }
        }

        public void btAdd()
        {
            if (ObjPCLItems_ByPCLFormID.Count <= 0)
            {
                return;
            }
            AddOne();
        }

        private void AddOnePCLRequest(PCLExamType ItemForChoose)
        {
            if (ItemForChoose == null || string.IsNullOrEmpty(ItemForChoose.PCLExamTypeName))
            {
                return;
            }
            bool bExists = AddToCurrentPclRequestIfNotExists(ItemForChoose);
            bool bExistReq = AddToCurrentPclRequestIfNotExistReq(ItemForChoose);
            if (bExists)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, SelectedItemForChoose.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else if (bExistReq)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z2593_G1_DVDaDK, ItemForChoose.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
                ItemForChoose.HIAllowedPrice = 0;
            }
            AddPCLDetail(ItemForChoose);
            TinhTongTienDuTru();
        }
        private void AddOnePCLAppt(PCLExamType ItemForChoose)
        {
            var bExists = AddToCurrentPclApptIfNotExists(ItemForChoose);

            if (bExists)
            {
                MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, SelectedItemForChoose.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            pclApptRequestDetailsContent.RefreshView();
        }
        private void AddOne(PCLExamType ItemForChoose)
        {
            PCLExamType item = new PCLExamType();
            item = ItemForChoose.DeepCopy(); //TBL: DeepCopy de khi gia BH thay doi thi gia BH cua danh muc khong doi
            if (item != null)
            {
                if (IsAppointment)
                {
                    AddOnePCLAppt(item);
                }
                else
                {
                    AddOnePCLRequest(item);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private void AddOne()
        {
            AddOne(SelectedItemForChoose);
        }


        public void btAddAll()
        {
            OneClick = false;
            if (ObjPCLItems_ByPCLFormID == null
                || ObjPCLItems_ByPCLFormID.Count < 1)
            {
                OneClick = true;
                return;
            }

            StringBuilder sb = new StringBuilder();
            if (!IsAppointment)
            {
                foreach (PCLExamType itemForChoose in ObjPCLItems_ByPCLFormID)
                {
                    PCLExamType item = new PCLExamType();
                    item = itemForChoose.DeepCopy(); //TBL: DeepCopy de khi gia BH thay doi thi gia BH cua danh muc khong doi
                    bool bExists = AddToCurrentPclRequestIfNotExists(item);
                    bool bExistReq = AddToCurrentPclRequestIfNotExistReq(item);

                    if (bExists)
                    {
                        sb.AppendLine("'" + item.PCLExamTypeName.Trim() + "'");
                    }
                    else if (bExistReq)
                    {
                        if (MessageBox.Show(string.Format(eHCMSResources.Z2593_G1_DVDaDK, item.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                        {
                            continue;
                        }
                        item.HIAllowedPrice = 0;
                    }
                    AddPCLDetail(item);
                }
            }
            else
            {
                foreach (PCLExamType itemForChoose in ObjPCLItems_ByPCLFormID)
                {
                    bool bExists = AddToCurrentPclApptIfNotExists(itemForChoose);

                    if (bExists)
                    {
                        sb.AppendLine("'" + itemForChoose.PCLExamTypeName.Trim() + "'");
                    }
                }
                pclApptRequestDetailsContent.RefreshView();
            }

            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                MessageBox.Show(sb.ToString() + Environment.NewLine + eHCMSResources.K0652_G1_DaDuocChon);
            }
            if (!IsAppointment)
            {
                TinhTongTienDuTru();
            }
            OneClick = true;
        }
        public void btSubtractAll()
        {

            if (!IsAppointment)
            {
                SubtractRequestAll();
            }
            else
            {
                SubtractApptAll();
            }

        }
        public void SubtractRequestAll()
        {
            if (CurrentPclRequest.PatientPCLRequestIndicators.Count <= 0)
                return;

            if (!CurrentPclRequest.CheckNewPCL(CurrentPclRequest))
            {
                MessageBox.Show(eHCMSResources.A0637_G1_Msg_InfoKhCoDVMoi);
                return;
            }

            if (MessageBox.Show(eHCMSResources.K0478_G1_XoaHetPCLExamTypeMoiThemTrongDS + "! " + eHCMSResources.K0478_G1_BanCoChacKhong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //CurrentPclRequest.PatientPCLRequestIndicators.Clear();
                if (CurrentPclRequest.PatientPCLRequestIndicators.Count > 0)
                {
                    foreach (var item in CurrentPclRequest.PatientPCLRequestIndicators)
                    {
                        //if (item.PCLReqItemID > 0)
                        //{
                        //    if (item.RecordState == RecordState.DETACHED)
                        //    {
                        //        //CurrentPclRequest.PatientPCLRequestIndicators.Remove(item);
                        //    }
                        //    else
                        //    {
                        //        item.RecordState = RecordState.DELETED;
                        //        item.MarkedAsDeleted = true;
                        //    }
                        //}
                        if (item.PCLReqItemID < 1)
                        {
                            PCLRequestDetailsContent.RemoveItemFromView(item);
                        }
                    }
                    CurrentPclRequest.PatientPCLRequestIndicators.Clear();
                }
                //PCLRequestDetailsContent.ResetCollection();
                TinhTongTienDuTru();
            }

        }

        public void SubtractApptAll()
        {
            if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList == null || EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count <= 0)
                return;
            if (!EditingApptPCLRequest.CheckNewPCL(EditingApptPCLRequest))
            {
                MessageBox.Show(eHCMSResources.A0637_G1_Msg_InfoKhCoDVMoi);
                return;
            }
            if (MessageBox.Show(eHCMSResources.K0482_G1_XoaHetPCLExamTypeTrongDS + " " + eHCMSResources.K0478_G1_BanCoChacKhong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Count > 0)
                {
                    EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList =
                        EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList.Where(item => item.PCLReqItemID > 0).ToObservableCollection();
                }
            }
            NotifyOfPropertyChange(() => CanbtSubtractAll);
            NotifyOfPropertyChange(() => EditingApptPCLRequest.ObjPatientApptPCLRequestDetailsList);
            pclApptRequestDetailsContent.RefreshView();
        }

        private void TinhTongTienDuTru()
        {
            Sum_NormalPrice = 0;
            Sum_HIAllowedPrice = 0;
            Sum_PriceDifference = 0;

            foreach (var requestDetail in CurrentPclRequest.PatientPCLRequestIndicators)
            {
                if (requestDetail.RecordState != RecordState.DELETED)
                {
                    int SL = Convert.ToByte(requestDetail.NumberOfTest);
                    if (SL == 0)
                    {
                        SL = 1;
                    }
                    Sum_NormalPrice += requestDetail.InvoicePrice * SL;
                    Sum_HIAllowedPrice += (requestDetail.PCLExamType.HIAllowedPrice == null ? 0 : requestDetail.PCLExamType.HIAllowedPrice.Value) * SL;
                }
            }
            Sum_PriceDifference = Sum_NormalPrice - Sum_HIAllowedPrice;

            EffectBoNut();
        }

        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.PCLExamType p = (selectedItem as DataEntities.PCLExamType);

            if (p != null && p.PCLExamTypeID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    ObjPCLItems_ByPCLFormID_Selected.Remove(p);
                    TinhTongTienDuTru();
                }
            }
        }

        #endregion

        #region Các Nút Lưu

        public void btEdit()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "hiệu chỉnh phiếu chỉ định"))
            {
                return;
            }
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU && CurrentPclRequest.InPatientBillingInvID != null && CurrentPclRequest.InPatientBillingInvID > 0)
            {
                MessageBox.Show(eHCMSResources.A0905_G1_Msg_InfoKhTheHChinhPh);
                return;
            }
            if (CurrentPclRequest.PaidTime != null)
            {
                MessageBox.Show(eHCMSResources.Z0356_G1_PhDaTToanKgDuocHChinh);
                return;
            }
            FormInputIsEnabled = true;
            PCLRequestDetailsContent.IsEnableListPCL = true;
            ButtonClicked(AllLookupValues.ButtonClicked.HIEUCHINH);
        }

        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
                NotifyOfPropertyChange(() => IsAllowToUpdateDiagnosis);
            }
        }

        public void btCancel()
        {
            if (CurrentPclRequest != null
                //&& ObjPatientPCLRequestNew.PatientPCLReqID > 0
                )
            {
                CurrentPclRequest = Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                PatientPCLRequestDetail_ByPatientPCLReqID();
            }

            ButtonClicked(AllLookupValues.ButtonClicked.BOQUA);

        }

        int FindPatient = 0;/*0: Ngoai Tru;1: Noi Tru*/

        private bool CheckExistIndicator(ObservableCollection<PatientPCLRequestDetail> PatientPCLRequestIndicators)
        {
            foreach (var item in PatientPCLRequestIndicators)
            {
                if (!item.MarkedAsDeleted)
                {
                    return true;
                }
            }
            return false;
        }

        private IEnumerator<IResult> btSaveCoroutine()
        {
            //▼===== #008
            //if (gMedicalInstructionDate == null) gMedicalInstructionDate = Globals.GetCurServerDateTime();
            //gMedicalInstructionDate = Globals.ApplyValidMedicalInstructionDate(gMedicalInstructionDate.Value, Registration_DataStorage.CurrentPatientRegistration);
            //CurrentPclRequest.MedicalInstructionDate = gMedicalInstructionDate;

            if (MedicalInstructionDateContent.DateTime == null)
            {
                MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            }
            MedicalInstructionDateContent.DateTime = Globals.ApplyValidMedicalInstructionDate(MedicalInstructionDateContent.DateTime.Value, Registration_DataStorage.CurrentPatientRegistration);
            CurrentPclRequest.MedicalInstructionDate = MedicalInstructionDateContent.DateTime;
            //▲===== #008
            CurrentPclRequest.DoctorStaffID = gSelectedDoctorStaff != null ? (long?)gSelectedDoctorStaff.StaffID : null;
            CurrentPclRequest.DoctorStaff = gSelectedDoctorStaff != null ? gSelectedDoctorStaff : null;

            yield return GenericCoRoutineTask.StartTask(UpdatePCLRequestInfo);

            if (Registration_DataStorage.CurrentPatient == null)
            {
                MessageBox.Show(eHCMSResources.K0286_G1_ChonBNTruocKhiLapPhYCPCL, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            if (CheckAllowEdit() == false)
                yield break;

            long PatientID = Registration_DataStorage.CurrentPatient.PatientID;

            if (PatientID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0286_G1_ChonBNTruocKhiLapPhYCPCL, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            if (RequireDiagnosisForPCLReq && string.IsNullOrEmpty(CurrentPclRequest.Diagnosis))
            {
                MessageBox.Show(eHCMSResources.K0420_G1_NhapCDoan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }

            if (cboIsExternalExamIsEnabled)
            {
                if (CurrentPclRequest.AgencyID == null)
                {
                    MessageBox.Show(eHCMSResources.A0302_G1_Msg_InfoChonBVBenNgoai, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    yield break;
                }
            }

            if (CurrentPclRequest.PatientPCLRequestIndicators.Count <= 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.K2033_G1_ChonLoaiXN2), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                yield break;
            }
            CurrentPclRequest.ServiceRecID = curServiceRecID;
            //▼===== 20191101 TTM: Task #1244   Thêm PtRegDetailID để phân biệt phiếu CLS này được tạo ra từ dịch vụ nào.
            //                                  Tránh trường hợp bệnh nhân 2 dịch vụ khám, chỉ định cho dịch vụ khám 2 không có gì phân biệt 
            //                                  => Chỉ định dịch vụ khám 2 lại là chẩn đoán dịch vụ khám 1
            CurrentPclRequest.PtRegDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
            //▲===== 
            //kiem tra truong hop xoa dich vu trong phieu da ton tai
            //neu xoa het thi phai huy phieu
            if (!CheckExistIndicator(CurrentPclRequest.PatientPCLRequestIndicators))
            {
                var res = new MessageWarningShowDialogTask(eHCMSResources.Z0359_G1_DVTrongPhYCCLSDaBiXoa
                    + "\n" + eHCMSResources.Z0906_G1_CoMuonHuyPh, eHCMSResources.T1747_G1_HuyPh);
                yield return res;
                if (res.IsAccept)
                {
                    if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        DeletePCLRequestWithDetails();
                    }
                    else
                    {
                        DeleteInPtPCLRequestWithDetails();
                    }
                }
                yield break;
            }

            Registration_DataStorage.CurrentPatientRegistration.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;

            //ObjPatientPCLRequestNew.PatientPCLRequestIndicators = CurrentPclRequest.PatientPCLRequestIndicators;
            List<PatientPCLRequest> ListSave = new List<PatientPCLRequest>();
            ListSave.Add(CurrentPclRequest);

            //list request goi xuong la tat ca cac request chua tra tien, tra roi khong ban lam gi nua
            var listPCLRequest = (from c in ListSave
                                  where c.PaidTime == null
                                  select c);
            if (listPCLRequest != null && listPCLRequest.Count() > 0)
            {
                var listitems = listPCLRequest.SelectMany(x => x.PatientPCLRequestIndicators);
                foreach (var item in listitems)
                {
                    //neu benh nhan duoc hen thi khong can kiem tra
                    if (item.PCLExamType.ObjPCLExamTypeServiceTarget != null)
                    {
                        //se kiem tra o day.......!!can lam sang so
                        var TargetTask = new PCLExamTypeServiceTarget_CheckedTask(item.PCLExamType.PCLExamTypeID, Globals.ServerDate.GetValueOrDefault());
                        yield return TargetTask;

                        if (TargetTask.Error != null)
                        {
                            Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}!", eHCMSResources.Z1041_G1_LoiXayRaCLSSo) });
                            yield break;
                        }
                        else
                        {
                            if (!TargetTask.Result)
                            {
                                if (MessageBox.Show(string.Format(eHCMSResources.Z1393_G1_VuotChiTieu, item.PCLExamType.PCLExamTypeName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                                {
                                    yield break;
                                }
                            }
                        }
                    }
                }
            }
            //▼====== #004
            //20190314 TTM: Gán cho CreateDate bằng giờ server.
            //              Lý do: CreatedDate đang được tạo mặc định tại DataEntities PatientPCLRequest = DateTime.Now => Nếu PatientPCLRequest đc new tại Service thì sẽ lấy giờ Server
            //                  Còn nếu new tại Client sẽ lấy giờ của máy. Do là DataEntities đc sử dụng chung cho cả Services và Client nên không set đc giờ mặc định là giờ Server.
            //              => Nếu máy người sử dụng đang để giờ sai => CreatedDate tạo cho phiếu yêu cầu này sẽ lấy theo giờ sai => Khi load lên tìm phiếu để thực hiện CLS sẽ không tìm được, do 
            //                  Điều kiện để lấy phiếu lên thực hiện dựa vào CreatedDate => sẽ không tìm thấy phiếu để thực hiện.           
            CurrentPclRequest.CreatedDate = Globals.GetCurServerDateTime();
            //▲====== #004

            if (CurrentPclRequest.PatientPCLReqID > 0)
            {
                //UpdatePCLRequest(CurrentPclRequest);/*bo di Bang da check ok ham nay vut*/
                List<PatientPCLRequest> lstPCLReqDelete = new List<PatientPCLRequest>();
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    FindPatient = 0;
                    GetListPCLReqDelete(out lstPCLReqDelete);
                }

                PatientPCLRequest PCLReqDelete = new PatientPCLRequest();
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
                {
                    FindPatient = 1;
                    GetPCLReqDelete(out PCLReqDelete);
                }

                //Vi sua la khong biet sua cho noi tru hay ngoai tru nen phai goi cai dangky cua phieu xuong thi moi dung.
                /* HPT 09/09/2016: Trước đây hàm này đi load thông tin đăng ký xong sẽ gọi luôn hàm Update nhưng thấy hơi khó hiểu do tên hàm là đi load đăng ký mà lại bỏ tham số là danh sách cận lâm sàng tạo mới và xóa
                 * Nên sửa lại cho hàm này sau khi load xong sẽ gán giá trị cho biến CurrentRegistrationInfo rồi sẽ kiểm tra và gọi hàm Update ngay đây chứ không gọi trong hàm GetRegistrationForPCLRequest() nữa
                 */
                yield return GenericCoRoutineTask.StartTask(GetRegistrationForPCLRequest);
                yield return GenericCoRoutineTask.StartTask(GetPatientPCLRequestList_ByRegistrationIDTask);
                if (lstPCLReqDelete != null && lstPCLReqDelete.Count > 0)
                {
                    foreach (var item in lstPCLReqDelete)
                    {
                        foreach (var details in CurrentRegistrationInfo.PCLRequests)
                        {
                            if (details.PatientPCLReqID == item.PatientPCLReqID)
                            {
                                details.RecordState = RecordState.MODIFIED;
                                details.PatientPCLRequestIndicators = item.PatientPCLRequestIndicators;
                                break;
                            }
                        }
                    }
                }

                if (CurrentRegistrationInfo != null)
                {
                    if (CurrentRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
                    {
                        AddServicesAndPCLRequests_Update(CurrentRegistrationInfo, null, listPCLRequest.ToList(), null, lstPCLReqDelete);
                    }
                    else if (CurrentRegistrationInfo.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                    {
                        UpdatePCLRequestsForInPt(CurrentRegistrationInfo, CurrentPclRequest, PCLReqDelete);
                    }
                }
            }
            else
            {
                FindPatient = (int)Globals.PatientFindBy_ForConsultation.Value;
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    AddServicesAndPCLRequests(Registration_DataStorage.CurrentPatientRegistration, null, listPCLRequest.ToList(), null, null);
                }
                else
                {
                    AddPCLRequestsForInPt(Registration_DataStorage.CurrentPatientRegistration, CurrentPclRequest, null);
                }
            }
        }

        private void SetPCLRequestOrderNumber(List<PatientPCLRequest> patientPCLRequests, List<PatientPCLRequest> deletedPtPCLRequests)
        {

        }

        public void btSave()
        {
            ////▼====== #002
            //if (CurrentPclRequest == null)
            //{
            //    MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
            //    return;
            //}
            //if (string.IsNullOrEmpty(CurrentPclRequest.Diagnosis))
            //{
            //    MessageBox.Show(eHCMSResources.K0420_G1_NhapCDoan);
            //    return;
            //}
            ////▲====== #002

            //▼==== #014
            if (Registration_DataStorage == null || Registration_DataStorage.CurrentPatientRegistration == null)
            {
                return;
            }
            if (Registration_DataStorage.CurrentPatientRegistration.InPatientTransferDeptReqID > 0)
            {
                MessageBox.Show(eHCMSResources.Z3262_G1_BNDaDeNghiChKhoa, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //▲==== #014

            //▼===== #008:  Do không còn sử dụng biến gMedicalInstructionDate nên không còn sự kiện lostfocus để kiểm tra nữa
            //              nên đặt sang nút lưu.
            if (Globals.ServerConfigSection.InRegisElements.CheckMedicalInstructDate && Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate >= 0)
            {
                Int32 NumOfDays = (MedicalInstructionDateContent.DateTime.GetValueOrDefault().Date - Globals.GetCurServerDateTime().Date).Days;
                if (NumOfDays > Globals.ServerConfigSection.InRegisElements.NumOfOverDaysInDischargeForm)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z2195_G1_NgYLenhKgVuotQuaNgHTai, Globals.ServerConfigSection.InRegisElements.NumOfOverDaysForMedicalInstructDate), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
                    return;
                }
            }
            //▲===== #008


            if (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.IsHIUnder15Percent.GetValueOrDefault(false))
            {
                MessageBox.Show(eHCMSResources.Z2202_G1_KhongTheSuaDKMCCT, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "lưu thông tin phiếu chỉ định"))
            {
                return;
            }
            //▼===== #008
            //if (RequireDoctorAndDate && (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID <= 0 || gMedicalInstructionDate == null))
            if (RequireDoctorAndDate && (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID <= 0 || MedicalInstructionDateContent.DateTime == null))
            //▲===== #008
            {
                MessageBox.Show(eHCMSResources.Z2184_G1_NhapDayDuNgayYLVaBS, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return;
            }
            if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && CS_DS != null && !CommonGlobals.CheckValidRequestRegimen(CurrentPclRequest, CS_DS.TreatmentRegimenCollection))
            {
                if (MessageBox.Show(eHCMSResources.Z2694_G1_PhieuYeuCauDVNgoaiPhacDo, eHCMSResources.K1576_G1_CBao, System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            IsEnabledtxbDiagnosis = false;
            IsLoadingPCLForms = true;
            //KMx: Khi bấm nút lưu thì cập nhật "Chẩn đoán" và "Yêu cầu" bằng method btUpdateInfo() trước, sau đó cập nhật PCL sau.
            //btUpdateInfo();
            Coroutine.BeginExecute(btSaveCoroutine(), null, (o, e) =>
            {
                IsLoadingPCLForms = false;
            });
        }
        private void GetPatientPCLRequestList_ByRegistrationIDTask(GenericCoRoutineTask genTask)
        {
            if (CurrentPclRequest == null || CurrentPclRequest.PatientPCLReqID < 1)
            {
                genTask.ActionComplete(true);
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientPCLRequestList_ByRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, V_RegistrationType, V_PCLMainCategory, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndGetPatientPCLRequestList_ByRegistrationID(asyncResult);
                                if (CurrentRegistrationInfo.PCLRequests == null)
                                {
                                    CurrentRegistrationInfo.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                                }
                                if (allItems != null && allItems.Count > 0)
                                {
                                    foreach (var item in allItems)
                                    {
                                        CurrentRegistrationInfo.PCLRequests.Add(item);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                    genTask.ActionComplete(false);
                }
                finally
                {
                }
            });
            t.Start();
        }
        private void UpdatePCLRequestInfo(GenericCoRoutineTask genTask)
        {
            if (CurrentPclRequest == null || CurrentPclRequest.PatientPCLReqID < 1)
            {
                genTask.ActionComplete(true);
                return;
            }
            //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        this.ShowBusyIndicator();
                        bool LoadCompleted = false;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePclRequestInfo(CurrentPclRequest,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    LoadCompleted = contract.EndUpdatePclRequestInfo(asyncResult);
                                    if (!LoadCompleted)
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0270_G1_Msg_CNhatCDoan_YCFail));
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                    MessageBox.Show(fault.ToString(), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                    genTask.ActionComplete(LoadCompleted);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                    genTask.ActionComplete(false);
                }
            });
            t.Start();
        }
        //Cập nhật "Chẩn đoán" và "Yêu cầu"
        //public void btUpdateInfo()
        //{
        //    if (CurrentPclRequest == null || CurrentPclRequest.PatientPCLReqID < 1)
        //    {
        //        return;
        //    }
        //    //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
        //    var t = new Thread(() =>
        //    {
        //        IsWaitingLoadRegistration = true;

        //        try
        //        {
        //            using (var serviceFactory = new PatientRegistrationServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginUpdatePclRequestInfo(CurrentPclRequest,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            var res = contract.EndUpdatePclRequestInfo(asyncResult);
        //                            if (!res)
        //                            {
        //                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0270_G1_Msg_CNhatCDoan_YCFail));
        //                            }
        //                        }
        //                        catch (FaultException<AxException> fault)
        //                        {
        //                            ClientLoggerHelper.LogInfo(fault.ToString());
        //                            MessageBox.Show(fault.ToString(), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            ClientLoggerHelper.LogInfo(ex.ToString());
        //                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
        //                        }
        //                    }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //        finally
        //        {
        //            IsWaitingLoadRegistration = false;
        //        }
        //    });

        //    t.Start();
        //}

        private PatientRegistration _CurrentRegistrationInfo;
        public PatientRegistration CurrentRegistrationInfo
        {
            get
            {
                return _CurrentRegistrationInfo;
            }
            set
            {
                _CurrentRegistrationInfo = value;
                NotifyOfPropertyChange(() => CurrentRegistrationInfo);
            }
        }
        private void GetRegistrationForPCLRequest(GenericCoRoutineTask genTask)
        {
            if (CurrentPclRequest == null || CurrentPclRequest.PatientPCLReqID < 1)
            {
                MessageBox.Show(eHCMSResources.A0409_G1_Msg_InfoChuaCoPhCLS);
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool LoadCompleted = false;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistration(CurrentPclRequest.PtRegistrationID, FindPatient,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var regInfo = contract.EndGetRegistration(asyncResult);

                                    if (regInfo != null)
                                    {
                                        CurrentRegistrationInfo = regInfo;
                                        LoadCompleted = true;
                                    }
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.Message.ToString());
                                }
                                finally
                                {
                                    genTask.ActionComplete(LoadCompleted);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        // HPT 09/09/2016: Hàm này trước đây tên là GetPCLReqDelete() dùng để lấy danh sách những phiếu chỉ định bị xóa ?!? (Không hiểu là trong trường hợp nào sẽ xóa được nhiều phiếu chỉ định cùng lúc!)
        // Đầu tiên, nhận thấy là trong màn hình này, các thao tác thêm xóa sửa trong một thời điểm chỉ có thể thao tác trên một phiếu thôi nên sửa param out của nó từ List<PatientPCLRequest> thành PatientPCLRequest
        // Tuy nhiên hàm này dùng cho ngoại trú, nếu sửa vậy sợ đụng nên đổi tên hàm cũ từ GetPCLReqDelete() --> GetListPCLReqDelete() và viết lại hàm GetPCLReqDelete với param out là một PatientPCLRequest
        // Chú thích: Có thể giữ nguyên hàm cũ, viết hàm mới với tên khác nhưng mà tên hàm cũ GetPCLReqDelete nghe có vẻ như trả về một phiếu chỉ định hơn nên giờ sửa tên hàm cũ cho dễ hình dung.
        // tóm lại, hàm GetListReqDelete sẽ dùng cho Ngoại trú, còn hàm GetReqPCLDelete đã được viết lại và dùng cho nội trú
        // Tuy nhiên, về cơ bản, đúng ra nội hay ngoại trú cũng đều như nhau. Sẽ review và kiểm tra lại cần thiết sẽ sửa cho ngoại trú luôn, giờ không có thời gian
        private void GetListPCLReqDelete(out List<PatientPCLRequest> lstPCLReqDelete)
        {
            lstPCLReqDelete = new List<PatientPCLRequest>();

            PatientPCLRequest RqDelete = new PatientPCLRequest();
            RqDelete.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
            RqDelete.ServiceRecID = CurrentPclRequest.ServiceRecID;
            RqDelete.ReqFromDeptLocID = CurrentPclRequest.ReqFromDeptLocID;
            RqDelete.PCLRequestNumID = CurrentPclRequest.PCLRequestNumID;
            RqDelete.Diagnosis = CurrentPclRequest.Diagnosis;
            RqDelete.DoctorComments = CurrentPclRequest.DoctorComments;
            RqDelete.IsExternalExam = CurrentPclRequest.IsExternalExam;
            RqDelete.IsImported = CurrentPclRequest.IsImported;
            RqDelete.IsCaseOfEmergency = CurrentPclRequest.IsCaseOfEmergency;
            RqDelete.StaffID = CurrentPclRequest.StaffID;
            RqDelete.MarkedAsDeleted = CurrentPclRequest.MarkedAsDeleted;
            RqDelete.V_PCLRequestType = CurrentPclRequest.V_PCLRequestType;
            RqDelete.V_PCLRequestStatus = CurrentPclRequest.V_PCLRequestStatus;
            RqDelete.PaidTime = CurrentPclRequest.PaidTime;
            RqDelete.RefundTime = CurrentPclRequest.RefundTime;
            RqDelete.CreatedDate = CurrentPclRequest.CreatedDate;
            RqDelete.AgencyID = CurrentPclRequest.AgencyID;
            RqDelete.InPatientBillingInvID = CurrentPclRequest.InPatientBillingInvID;

            RqDelete.RecordState = RecordState.MODIFIED;

            RqDelete.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();

            foreach (var detail in CurrentPclRequest.PatientPCLRequestIndicators)
            {
                if (detail.RecordState == RecordState.DELETED)
                {
                    RqDelete.PatientPCLRequestIndicators.Add(detail);
                }
            }

            if (RqDelete.PatientPCLRequestIndicators.Count <= 0)
            {
                lstPCLReqDelete = null;
            }
            else
            {
                lstPCLReqDelete.Add(RqDelete);
            }

        }


        // HPT: 09/09/2016: trước đây hàm này trả về một danh sách những phiếu chỉ định cận lâm sàng bị xóa !?! (Không hiểu tại sao?)
        // Xét thấy trong màn hình phiếu chỉ định, dù là thêm, xóa hay sửa thì tại một thời điểm chỉ thao tác trên một phiếu thôi nên đã sửa hàm này cho trả về một phiếu chỉ định bị xóa thôi. Phiếu này chứa những dòng item bị MarkedAsDelete của phiếu hiện hành
        private void GetPCLReqDelete(out PatientPCLRequest PCLReqDelete)
        {
            if (!CurrentPclRequest.PatientPCLRequestIndicators.Any(x => x.RecordState == RecordState.DELETED))
            {
                PCLReqDelete = null;
                return;
            }
            PCLReqDelete = new PatientPCLRequest();
            PCLReqDelete.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
            PCLReqDelete.ServiceRecID = CurrentPclRequest.ServiceRecID;
            PCLReqDelete.ReqFromDeptLocID = CurrentPclRequest.ReqFromDeptLocID;
            PCLReqDelete.PCLRequestNumID = CurrentPclRequest.PCLRequestNumID;
            PCLReqDelete.Diagnosis = CurrentPclRequest.Diagnosis;
            PCLReqDelete.DoctorComments = CurrentPclRequest.DoctorComments;
            PCLReqDelete.IsExternalExam = CurrentPclRequest.IsExternalExam;
            PCLReqDelete.IsImported = CurrentPclRequest.IsImported;
            PCLReqDelete.IsCaseOfEmergency = CurrentPclRequest.IsCaseOfEmergency;
            PCLReqDelete.StaffID = CurrentPclRequest.StaffID;
            PCLReqDelete.MarkedAsDeleted = CurrentPclRequest.MarkedAsDeleted;
            PCLReqDelete.V_PCLRequestType = CurrentPclRequest.V_PCLRequestType;
            PCLReqDelete.V_PCLRequestStatus = CurrentPclRequest.V_PCLRequestStatus;
            PCLReqDelete.PaidTime = CurrentPclRequest.PaidTime;
            PCLReqDelete.RefundTime = CurrentPclRequest.RefundTime;
            PCLReqDelete.CreatedDate = CurrentPclRequest.CreatedDate;
            PCLReqDelete.AgencyID = CurrentPclRequest.AgencyID;
            PCLReqDelete.InPatientBillingInvID = CurrentPclRequest.InPatientBillingInvID;

            PCLReqDelete.RecordState = RecordState.MODIFIED;

            PCLReqDelete.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();
            PCLReqDelete.PatientPCLRequestIndicators = CurrentPclRequest.PatientPCLRequestIndicators.Where(x => x.RecordState == RecordState.DELETED).ToObservableCollection();
            //foreach (var detail in CurrentPclRequest.PatientPCLRequestIndicators)
            //{
            //    if (detail.RecordState == RecordState.DELETED)
            //    {
            //        PCLReqDelete.PatientPCLRequestIndicators.Add(detail);
            //    }
            //}
        }
        //private bool CheckDeptLocID()
        //{
        //    for (int i = 0; i < CurrentPclRequest.PatientPCLRequestIndicators.Count; i++)
        //    {
        //        if (CurrentPclRequest.PatientPCLRequestIndicators[i].DeptLocation == null || CurrentPclRequest.PatientPCLRequestIndicators[i].DeptLocation.DeptLocationID <= 0)
        //        {
        //            MessageBox.Show("Lỗi dòng " + (i + 1).ToString() + ": Chưa chọn Phòng!");
        //            return false;
        //        }
        //    }
        //    return true;
        //}


        private bool CheckAllowEdit()
        {
            if (CurrentPclRequest == null)
            {
                MessageBox.Show(eHCMSResources.A0350_G1_Msg_InfoChonPhDeHChinh);
                return false;
            }
            if (CurrentPclRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN)
            {
                return true;
            }

            switch (CurrentPclRequest.V_PCLRequestStatus)
            {
                case AllLookupValues.V_PCLRequestStatus.CANCEL:
                    {
                        MessageBox.Show(string.Format("{0}! ", eHCMSResources.Z1030_G1_PhNayBNTraLaiKgLamXN));
                        break;
                    }
                case AllLookupValues.V_PCLRequestStatus.CLOSE:
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0920_G1_Msg_InfoKhDcSuaPhDaXong));
                        break;
                    }
                case AllLookupValues.V_PCLRequestStatus.PROCESSING:
                    {
                        MessageBox.Show(string.Format("{0}!", eHCMSResources.A0920_G1_Msg_InfoKhDcSuaPhDaXong));
                        break;
                    }
            }
            return false;

        }

        public void UpdatePCLRequest(PatientPCLRequest request)
        {
            /*20170818 CMN: Checked that not working*/
            /*
            IsWaitingSave = true;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdatePCLRequest(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), request, default(DateTime), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                List<PatientPCLRequest> listPclSave = new List<PatientPCLRequest>();

                                contract.EndUpdatePCLRequest(out listPclSave, asyncResult);

                                //Khóa Màn Hình
                                FormInputIsEnabled = false;
                                StatusForm(AllLookupValues.StatusForm.HIEUCHINH);

                                //Phát sự kiện load lại danh sách yêu cầu PCL
                                Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });


                                //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu
                                ObjPatientPCLRequestNew_BackUp = ObjectCopier.DeepCopy(CurrentPclRequest);

                                CurrentPclRequest = ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                                SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                                PatientPCLRequestDetail_ByPatientPCLReqID();
                                //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu


                                if (listPclSave.Count == 0)
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1064_G1_DaCNhatPh), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}! ", eHCMSResources.Z1064_G1_DaCNhatPh) + Environment.NewLine + listPclSave.Count.ToString() + string.Format(" {0}", eHCMSResources.A0442_G1_Msg_InfoTaoMoiViKhacPg), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                            }
                            catch (Exception innerEx)
                            {
                                ClientLoggerHelper.LogInfo(innerEx.ToString());
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    IsWaitingSave = false;
                }
            });
            t.Start();
            */
        }

        public void btPreviewPrint()
        {
            if (CurrentPclRequest.PatientPCLReqID > 0)
            {
                Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                {
                    proAlloc.PatientPCLReqID = (int)CurrentPclRequest.PatientPCLReqID;
                    proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                    proAlloc.V_RegistrationType = V_RegistrationType;
                };
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0351_G1_Msg_InfoChonPh);
                return;
            }
        }

        public void btPrint()
        {
            MessageBox.Show(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.A0073_G1_CNangDangHThien));
        }

        public bool cboIsExternalExamIsEnabled
        {
            get
            {
                if (CurrentPclRequest == null)
                    return false;
                return (CurrentPclRequest.IsExternalExam == null ? false : CurrentPclRequest.IsExternalExam.Value);
            }
        }

        public void chkExternalExam_Click(object sender, RoutedEventArgs e)
        {
            NotifyOfPropertyChange(() => cboIsExternalExamIsEnabled);

            CheckBox Ctr = (sender as CheckBox);
            if (Ctr.IsChecked.Value)
            {
                CurrentPclRequest.IsExternalExam = true;
                CurrentPclRequest.IsImported = false;
            }
            else
            {
                CurrentPclRequest.AgencyID = -1;
                CurrentPclRequest.IsExternalExam = false;
                CurrentPclRequest.IsImported = false;
            }

        }

        #endregion

        #region Thông tin bệnh viện ngoài
        private ObservableCollection<TestingAgency> _ObjTestingAgencyList;
        public ObservableCollection<TestingAgency> ObjTestingAgencyList
        {
            get
            {
                return _ObjTestingAgencyList;
            }
            set
            {
                if (_ObjTestingAgencyList != value)
                {
                    _ObjTestingAgencyList = value;
                    NotifyOfPropertyChange(() => ObjTestingAgencyList);
                }
            }
        }
        private void GetTestingAgency_All()
        {
            ObjTestingAgencyList.Clear();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0535_G1_DSBVNgoai) });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetTestingAgency_All(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetTestingAgency_All(asyncResult);
                            if (items != null)
                            {
                                ObjTestingAgencyList = new ObservableCollection<TestingAgency>(items);

                                //Item Default
                                DataEntities.TestingAgency ItemDefault = new DataEntities.TestingAgency();
                                ItemDefault.AgencyID = -1;
                                ItemDefault.AgencyName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K1916_G1_Chon);
                                ObjTestingAgencyList.Insert(0, ItemDefault);
                                //Item Default

                                if (CurrentPclRequest.AgencyID == null || CurrentPclRequest.AgencyID <= 0)
                                {
                                    CurrentPclRequest.AgencyID = -1;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });
            t.Start();
        }
        #endregion

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            PCLExamType Objtmp = eventArgs.Value as PCLExamType;
            AddOne(Objtmp);
        }

        public void Handle(TinhTongTienDuTruEvent message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    TinhTongTienDuTru();
                }
            }
        }

        public void Handle(DbClickSelectedObjectEventWithKey<PatientPCLRequest, string> message)
        {
            DoctorComments = "";
            if (this.GetView() != null)
            {
                //Gán lại đối tượng hiện hành là cái thằng chụp được này
                if (message != null && message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    ObjPatientPCLRequestNew_BackUp = Common.ObjectCopier.DeepCopy(message.ObjA);

                    CurrentPclRequest = Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                    SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                    PatientPCLRequestDetail_ByPatientPCLReqID();

                    //Khóa Màn Hình

                    if (!Globals.isConsultationStateEdit
                        || (Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails != null
                        && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0 && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation != null
                        && Globals.DeptLocation != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation.DeptID != Globals.DeptLocation.DeptID))
                    {
                        FormIsEnabled = false;
                        return;
                    }
                    else
                    {
                        FormIsEnabled = true;
                        FormInputIsEnabled = false;
                        PCLRequestDetailsContent.IsEnableListPCL = false;
                        StatusForm(AllLookupValues.StatusForm.HIEUCHINH);
                    }

                }
            }
        }

        public void btDelete()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "hủy phiếu chỉ định"))
            {
                return;
            }
            if (CurrentPclRequest.PaidTime != null)
            {
                MessageBox.Show(eHCMSResources.Z0393_G1_PhDaTToanKgDuocXoa);
                return;
            }
            if (MessageBox.Show(string.Format(eHCMSResources.Z1065_G1_CoChacHuyPhKg, CurrentPclRequest.PCLRequestNumID.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
                {
                    DeletePCLRequestWithDetails();
                }
                else
                {
                    DeleteInPtPCLRequestWithDetails();
                }
            }
        }

        public void DeletePCLRequestWithDetails()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeletePCLRequestWithDetails(CurrentPclRequest.PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndDeletePCLRequestWithDetails(out Result, asyncResult);

                                switch (Result)
                                {
                                    case "HasPaidTime":
                                        {
                                            MessageBox.Show(eHCMSResources.Z0362_G1_PhDaTraTien);
                                            break;
                                        }
                                    case "NotExists":
                                        {
                                            MessageBox.Show(string.Format("{0}!", eHCMSResources.A0745_G1_Msg_InfoKhTimThayPh));
                                            break;
                                        }
                                    case "0":
                                        {
                                            MessageBox.Show(string.Format(eHCMSResources.Z1068_G1_HuyPhThatBai, CurrentPclRequest.PCLRequestNumID.Trim()));
                                            break;
                                        }
                                    case "1":
                                        {
                                            //Phát sự kiện load lại danh sách yêu cầu PCL

                                            PatientPCLRequest_RequestLastest();

                                            //KMx: Sau khi xóa chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 16:22).
                                            GetPatientPCLRequestList_ByRegistrationID();

                                            PCLRequestDetailsContent.ResetCollection();

                                            Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });

                                            //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1070_G1_HuyPhYCLoadPhKeCan));
                                            GlobalsNAV.ShowMessagePopup(string.Format("{0}!", eHCMSResources.Z1070_G1_HuyPhYCLoadPhKeCan));

                                            PatientPCLRequestDetail_ByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);

                                            TinhTongTienDuTru();

                                            FormInputIsEnabled = false;
                                            PCLRequestDetailsContent.IsEnableListPCL = false;
                                            StatusForm(AllLookupValues.StatusForm.TAOMOI);

                                            UpdateOrderStatusForDeletedPCLRequestList();

                                            break;
                                        }
                                }
                            }
                            catch (Exception innerEx)
                            {
                                Globals.ShowMessage(innerEx.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                IsWaitingDeletePCLRequest = false;
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

        public void UpdateOrderStatusForDeletedPCLRequestList()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientDeletedPCLRequestListByRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = client.EndGetPatientDeletedPCLRequestListByRegistrationID(asyncResult);
                                List<PatientPCLRequest> deletedPatientPCLRequestList = null;
                                if (null != allItems)
                                {
                                    deletedPatientPCLRequestList = allItems.ToList<PatientPCLRequest>();
                                }

                                if (null == deletedPatientPCLRequestList || deletedPatientPCLRequestList.Count < 1)
                                {
                                    return;
                                }
                                //20220331: QTD Cấu hình bật/tắt QMS cho CLS
                                if (Globals.ServerConfigSection.CommonItems.IsEnableQMSForPCL)
                                {
                                    GlobalsNAV.UpdateDoneStatus(Registration_DataStorage.CurrentPatient, deletedPatientPCLRequestList);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
            t.Start();
        }

        public void DeleteInPtPCLRequestWithDetails()
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeleteInPtPCLRequestWithDetails(CurrentPclRequest.PatientPCLReqID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDeleteInPtPCLRequestWithDetails(asyncResult);

                                //Phát sự kiện load lại danh sách yêu cầu PCL

                                PatientPCLRequest_RequestLastest();

                                //KMx: Sau khi xóa chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 10:32).
                                GetPatientPCLRequestList_ByRegistrationID();

                                PCLRequestDetailsContent.ResetCollection();

                                Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });

                                //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1070_G1_HuyPhYCLoadPhKeCan));
                                GlobalsNAV.ShowMessagePopup(string.Format("{0}!", eHCMSResources.Z1070_G1_HuyPhYCLoadPhKeCan));

                                TinhTongTienDuTru();

                                FormInputIsEnabled = false;
                                PCLRequestDetailsContent.IsEnableListPCL = false;
                                //StatusForm(AllLookupValues.StatusForm.TAOMOI);
                            }
                            catch (Exception innerEx)
                            {
                                Globals.ShowMessage(innerEx.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                IsWaitingDeletePCLRequest = false;
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

        #region "Ext chọn theo tên gõ vào autocomplete"

        private IPCLItems_SearchAutoComplete _SPTheoAutoComplete;
        public IPCLItems_SearchAutoComplete SPTheoAutoComplete
        {
            get { return _SPTheoAutoComplete; }
            set
            {
                if (_SPTheoAutoComplete != value)
                {
                    _SPTheoAutoComplete = value;
                    NotifyOfPropertyChange(() => SPTheoAutoComplete);
                }
            }
        }

        private void InitControlsForExt()
        {
            //SPTheoFormVisibility = true;
            //SPTheoAutoCompleteVisibility = false;

            NotifyOfPropertyChange(() => SPTheoFormVisibility);
            NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);

            var ucDynamic = Globals.GetViewModel<IPCLItems_SearchAutoComplete>();
            //20181210 TTM: để AutoComplete biết đang sử dụng cho yêu cầu hình ảnh hay xét nghiệm mà lấy dữ liệu
            //              cho chính xác.
            ucDynamic.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            ucDynamic.KeyAction = eHCMSResources.Z0055_G1_Edit;
            SPTheoAutoComplete = ucDynamic;
            SPTheoAutoComplete.CS_DS = CS_DS;
            if (IsAppointment)
            {
                SPTheoAutoComplete.IsAppointment = true;
            }
            this.ActivateItem(ucDynamic);
            NotifyOfPropertyChange(() => IsRegimenChecked);

            //cboPCLFormIsEnabled = true;
            //rdoLABByFormIsEnabled = false;
            //rdoLABByNameIsEnabled = false;

        }

        RadioButton CtrrdoLABByForm;
        RadioButton CtrrdoLABByName;
        public void rdoLABByForm_Loaded(object sender, RoutedEventArgs e)
        {
            CtrrdoLABByForm = sender as RadioButton;
            NotifyOfPropertyChange(() => SPTheoFormVisibility);
        }
        public void rdoLABByName_Loaded(object sender, RoutedEventArgs e)
        {
            CtrrdoLABByName = sender as RadioButton;
            NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
        }

        public bool cboPCLFormIsEnabled
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID <= 0)
                    return false;
                return true;
            }
        }


        public bool rdoLABByFormIsEnabled
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    return true;
                }
                return false;
            }
        }

        public bool rdoLABByNameIsEnabled
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                {
                    return true;
                }
                return false;
            }
        }


        //private bool _SPTheoAutoCompleteVisibility;
        public bool SPTheoAutoCompleteVisibility
        {
            get
            {
                if (ObjV_PCLMainCategory_Selected != null && ObjV_PCLMainCategory_Selected.LookupID <= 0)
                    return true;
                return false;
            }
        }

        //private bool _SPTheoFormVisibility;
        public bool SPTheoFormVisibility
        {
            get
            {
                return !(SPTheoAutoCompleteVisibility);
            }
        }

        public void rdoLABByForm_Click(object sender, RoutedEventArgs e)
        {
            RadioButton Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                //SPTheoFormVisibility = true;
                //SPTheoAutoCompleteVisibility = false;

                NotifyOfPropertyChange(() => cboPCLFormIsEnabled);

                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
            }
        }

        public void rdoLABByName_Click(object sender, RoutedEventArgs e)
        {
            RadioButton Ctr = (sender as RadioButton);
            if (Ctr.IsChecked.Value)
            {
                //SPTheoFormVisibility = false;
                //SPTheoAutoCompleteVisibility = true;
                NotifyOfPropertyChange(() => cboPCLFormIsEnabled);

                NotifyOfPropertyChange(() => SPTheoFormVisibility);
                NotifyOfPropertyChange(() => SPTheoAutoCompleteVisibility);
            }
        }

        public void Handle(SelectedObjectEventWithKey<PCLExamType, String> message)
        {
            if (message != null)
            {
                if (this.GetView() != null)
                {
                    if (message.Key == eHCMSResources.Z0055_G1_Edit)
                    {
                        SelectedItemForChoose = message.Result;
                        AddOne();
                    }
                }
            }
        }

        #endregion


        private bool _btXoaIsEnabled;
        public bool btXoaIsEnabled
        {
            get { return _btXoaIsEnabled; }
            set
            {
                if (_btXoaIsEnabled != value)
                {
                    _btXoaIsEnabled = value;
                    NotifyOfPropertyChange(() => btXoaIsEnabled);
                }
            }
        }

        private bool _btInIsEnabled;
        public bool btInIsEnabled
        {
            get { return _btInIsEnabled; }
            set
            {
                if (_btInIsEnabled != value)
                {
                    _btInIsEnabled = value;
                    NotifyOfPropertyChange(() => btInIsEnabled);
                }
            }
        }


        #region "Tạo mới PCLRequest"

        private bool _isWaitingLoadChanDoan;
        public bool IsWaitingLoadChanDoan
        {
            get { return _isWaitingLoadChanDoan; }
            set
            {
                if (_isWaitingLoadChanDoan != value)
                {
                    _isWaitingLoadChanDoan = value;
                    NotifyOfPropertyChange(() => IsWaitingLoadChanDoan);
                    NotifyWhenBusy();
                }
            }
        }


        private DiagnosisTreatment _ObjGetDiagnosisTreatmentByPtID;
        public DiagnosisTreatment ObjGetDiagnosisTreatmentByPtID
        {
            get
            {
                return _ObjGetDiagnosisTreatmentByPtID;
            }
            set
            {
                if (_ObjGetDiagnosisTreatmentByPtID != value)
                {
                    _ObjGetDiagnosisTreatmentByPtID = value;
                    NotifyOfPropertyChange(() => ObjGetDiagnosisTreatmentByPtID);
                }
            }
        }

        public long curServiceRecID
        {
            get
            {
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0
                    && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count() > 0)
                {
                    return Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault(x => x.PtRegDetailID == Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID).ServiceRecID;
                }
                return 0;
            }
        }


        /*opt:-- 0: Query by PatientID, 1: Query by PtRegistrationID, 2: Query By NationalMedicalCode  */
        private void GetDiagnosisTreatmentByPtID(long? patientID, long? PtRegistrationID, string nationalMedCode, int opt, bool latest, long? V_RegistrationType, long? PtRegDetailID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0536_G1_CDoanCuoi) });
            IsWaitingLoadChanDoan = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisTreatmentByPtID_V2(patientID, null, null, opt, true, V_RegistrationType, curServiceRecID, PtRegDetailID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetDiagnosisTreatmentByPtID_V2(asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    ObjGetDiagnosisTreatmentByPtID = results.ToObservableCollection()[0];
                                    CurrentPclRequest.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID;
                                    CurrentPclRequest.PatientServiceRecord.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID.Value;
                                    CurrentPclRequest.Diagnosis = string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal) ? ObjGetDiagnosisTreatmentByPtID.Diagnosis.Trim() : ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal.Trim()
                                    + (string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisOther) ? "" : "; " + ObjGetDiagnosisTreatmentByPtID.DiagnosisOther);
                                    HasDiag = true;
                                }
                                else
                                {
                                    HasDiag = false;
                                    if (!IsAppointment && RequireDiagnosisForPCLReq)
                                    {
                                        FormIsEnabled = false;
                                        MessageBox.Show(eHCMSResources.A0403_G1_Msg_InfoChuaCoCDChoDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                //Đọc Phiếu YC cuối lên
                                if (!IsNew)
                                {
                                    PatientPCLRequest_RequestLastest();
                                }
                                IsNew = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                //Globals.IsBusy = false;
                                IsWaitingLoadChanDoan = false;
                            }

                        }), null);

                }

            });

            t.Start();
        }

        private bool _btNewIsEnabled;
        public bool btNewIsEnabled
        {
            get { return _btNewIsEnabled; }
            set
            {
                if (_btNewIsEnabled != value)
                {
                    _btNewIsEnabled = value;
                    NotifyOfPropertyChange(() => btNewIsEnabled);
                }
            }
        }
        private bool IsNew = false;
        public void btNew()
        {
            if (Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, "tạo mới phiếu chỉ định"))
            {
                return;
            }
            //▼====: #013
            if (Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                if (!Globals.CheckDoctorContactPatientTime())
                {
                    return;
                }
                else
                {
                    GlobalsNAV.AddUpdateDoctorContactPatientTimeAction(Registration_DataStorage.CurrentPatient.PatientID
                        , Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID
                        , Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID
                        , Globals.StartDatetimeExam.Value
                        , "XN"
                        , Globals.LoggedUserAccount.Staff.StaffID);
                }
            }
            //▲====: #013
            FormInputIsEnabled = true;
            PCLRequestDetailsContent.IsEnableListPCL = true;
            CreateNewPCLReq();

            ButtonClicked(AllLookupValues.ButtonClicked.TAOMOI);

        }
        private long V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
        private PatientPCLRequest NewPCLReq()
        {
            ObjStaff = Globals.LoggedUserAccount.Staff;
            Sum_NormalPrice = 0;
            Sum_HIAllowedPrice = 0;
            Sum_PriceDifference = 0;

            //Reset ben list chua detail luon
            PCLRequestDetailsContent.ResetCollection();

            var ObjNew = new PatientPCLRequest();
            ObjNew.DeptID = Globals.ObjRefDepartment.DeptID;

            if (!IsAppointment)
            {
                if (Registration_DataStorage.CurrentPatientRegistration != null
                && Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID > 0)
                {
                    ObjNew.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0380_G1_Msg_InfoChuaChonDK);
                }
            }

            ObjNew.V_PCLMainCategory = V_PCLMainCategory;
            ObjNew.StaffIDName = ObjStaff.FullName;
            ObjNew.StaffID = ObjStaff.StaffID;
            ObjNew.DoctorStaffID = gSelectedDoctorStaff != null ? (long?)gSelectedDoctorStaff.StaffID : null;

            ObjNew.PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>();

            ObjNew.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
            ObjNew.ReqFromDeptID = Globals.DeptLocation.DeptID;
            ObjNew.DoctorComments = "";
            ObjNew.IsCaseOfEmergency = false;

            ObjNew.IsExternalExam = false;
            ObjNew.IsImported = false;

            ObjNew.PatientServiceRecord = new PatientServiceRecord();
            if (ObjGetDiagnosisTreatmentByPtID != null
                && ObjGetDiagnosisTreatmentByPtID.ServiceRecID > 0)
            {
                ObjNew.PatientServiceRecord.ServiceRecID = ObjGetDiagnosisTreatmentByPtID.ServiceRecID.Value;
                ObjNew.Diagnosis = string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal) ? ObjGetDiagnosisTreatmentByPtID.Diagnosis : ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal;
                ObjNew.ICD10List = ObjGetDiagnosisTreatmentByPtID.ICD10List == null ? null : ObjGetDiagnosisTreatmentByPtID.ICD10List.Trim().Replace(",", "; ");
            }

            ObjNew.PatientServiceRecord.StaffID = Globals.LoggedUserAccount.StaffID;
            ObjNew.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN;

            return ObjNew;
        }

        private void CreateNewPCLReq()
        {
            CurrentPclRequest = NewPCLReq();
            if ((long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
            {
                if (ObjGetDiagnosisTreatmentByPtID != null)
                {
                    CurrentPclRequest.Diagnosis = (string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal) ? ObjGetDiagnosisTreatmentByPtID.Diagnosis : ObjGetDiagnosisTreatmentByPtID.DiagnosisFinal)
                        + (string.IsNullOrEmpty(ObjGetDiagnosisTreatmentByPtID.DiagnosisOther) ? "" : "; " + ObjGetDiagnosisTreatmentByPtID.DiagnosisOther);
                }
            }
            else
            {
                //▼===== #005: Khi tạo mới 1 chỉ định CLS sẽ lấy chẩn đoán cuối cùng trong đợt đăng ký của bệnh nhân để đưa vào chỉ định.
                IsNew = true;
                GetDiagnosisTreatmentByPtID(Registration_DataStorage.CurrentPatient.PatientID, Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, "", 1, true, (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                //▲===== #005
            }
            HasGroup = true;

            ObjPCLItems_ByPCLFormID_Selected = new ObservableCollection<PCLExamType>();
            SelectedItemForChoose = new PCLExamType();

            NotifyOfPropertyChange(() => cboIsExternalExamIsEnabled);

            //▼===== 20200515 TTM: Sửa lại ngày y lệnh khi bấm tạo mới lấy luôn giờ phút giây vì ngày GIỜ y lệnh thì phải có GIỜ
            //gMedicalInstructionDate = Globals.GetCurServerDateTime().Date;
            gMedicalInstructionDate = Globals.GetCurServerDateTime();
            //▲=====
            //▼===== #008
            MedicalInstructionDateContent.DateTime = new DateTime(Globals.GetCurServerDateTime().Year, Globals.GetCurServerDateTime().Month, Globals.GetCurServerDateTime().Day, Globals.GetCurServerDateTime().Hour, Globals.GetCurServerDateTime().Minute, 0);
            //▲===== #008
            gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == Globals.LoggedUserAccount.StaffID) : null;
        }


        public void AddPCLRequestsForInPt(PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddPCLRequestsForInPt(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DeptLocation.DeptLocationID, Globals.DeptLocation.DeptID, Registration_DataStorage.CurrentPatientRegistration, pclRequest, deletedPclRequest, IsNotCheckInvalid, default(DateTime), Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    IList<PatientPCLRequest> SavedPclRequestList;
                                    contract.EndAddPCLRequestsForInPt(out SavedPclRequestList, asyncResult);

                                    if (SavedPclRequestList != null && SavedPclRequestList.Count > 0)
                                    {
                                        //Phát sự kiện load lại danh sách yêu cầu PCL
                                        Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });

                                        FormInputIsEnabled = false;
                                        PCLRequestDetailsContent.IsEnableListPCL = false;
                                        StatusForm(AllLookupValues.StatusForm.HIEUCHINH);

                                        PatientPCLRequest_RequestLastest();

                                        //KMx: Sau khi thêm mới chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 16:21).
                                        GetPatientPCLRequestList_ByRegistrationID();
                                        IsNotCheckInvalid = false;
                                        if (SavedPclRequestList.Count == 1)
                                        {
                                            //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1045_G1_DaLuuPhYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1045_G1_DaLuuPhYC);
                                            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                                            {
                                                proAlloc.PatientPCLReqID = SavedPclRequestList.FirstOrDefault().PatientPCLReqID;
                                                proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                                                proAlloc.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
                                            };
                                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                                        }
                                        else
                                        {
                                            //MessageBox.Show(string.Format(eHCMSResources.Z1061_G1_LuuPhYCTachPh, SavedPclRequestList.Count.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            GlobalsNAV.ShowMessagePopup(string.Format(eHCMSResources.Z1061_G1_LuuPhYCTachPh, SavedPclRequestList.Count.ToString()));
                                            //KMx: Nếu có 2 phiếu trở lên thì in gộp (24/03/2014 16:31).
                                            strPCLRequestIDList = GetPCLRequestIDList(SavedPclRequestList.ToList());
                                            StartViewPrintAllLabPCL();
                                            strPCLRequestIDList = null;

                                        }

                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0477_G1_LuuKhongThanhCong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //▼===== #007
                                    //TBL: 19090601 là ID để xác định thông báo
                                    if (ex.Message.Contains("19090601") && MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                                    {
                                        //20191025 TBL: Nếu đồng ý lưu thì lưu lại và bỏ qua kiểm tra dưới store
                                        IsNotCheckInvalid = true;
                                        btSave();
                                    }
                                    else if (!ex.Message.Contains("19090601"))
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                                    }
                                    //▲===== #007
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
                }
            });
            t.Start();
        }
        public void UpdatePCLRequestsForInPt(PatientRegistration regInfo, PatientPCLRequest pclRequest, PatientPCLRequest deletedPclRequest)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddPCLRequestsForInPt(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                            Globals.DeptLocation.DeptLocationID, Globals.DeptLocation.DeptID, regInfo, pclRequest, deletedPclRequest, IsNotCheckInvalid, default(DateTime), Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    IList<PatientPCLRequest> SavedPclRequestList;
                                    contract.EndAddPCLRequestsForInPt(out SavedPclRequestList, asyncResult);

                                    //Phát sự kiện load lại danh sách yêu cầu PCL
                                    Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });

                                    FormInputIsEnabled = false;
                                    PCLRequestDetailsContent.IsEnableListPCL = false;
                                    StatusForm(AllLookupValues.StatusForm.HIEUCHINH);

                                    //KMx: Sau khi cập nhật chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 16:22).
                                    GetPatientPCLRequestList_ByRegistrationID();

                                    //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu
                                    if (SavedPclRequestList != null && SavedPclRequestList.Count > 0)
                                    {
                                        CurrentPclRequest = SavedPclRequestList[0];
                                        ObjPatientPCLRequestNew_BackUp = Common.ObjectCopier.DeepCopy(CurrentPclRequest);
                                        CurrentPclRequest = Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                                    }

                                    SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                                    PatientPCLRequestDetail_ByPatientPCLReqID();
                                    //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu

                                    if (SavedPclRequestList != null && SavedPclRequestList.Count > 0)
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z1062_G1_LuuPhYC, SavedPclRequestList.Count.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                                        if (SavedPclRequestList.Count == 1)
                                        {
                                            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                                            {
                                                proAlloc.PatientPCLReqID = SavedPclRequestList.FirstOrDefault().PatientPCLReqID;
                                                proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                                                proAlloc.V_RegistrationType = V_RegistrationType;
                                            };
                                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                                        }
                                        else
                                        {
                                            //KMx: Nếu có 2 phiếu trở lên thì in gộp (24/03/2014 16:31).
                                            strPCLRequestIDList = GetPCLRequestIDList(SavedPclRequestList.ToList());
                                            StartViewPrintAllLabPCL();
                                            strPCLRequestIDList = null;
                                        }
                                    }
                                    else
                                    {
                                        //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1045_G1_DaLuuPhYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        GlobalsNAV.ShowMessagePopup(string.Format("{0}!", eHCMSResources.Z1045_G1_DaLuuPhYC));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                }
            });
            t.Start();
        }


        public void AddServicesAndPCLRequests(PatientRegistration regInfo, IList<PatientRegistrationDetail> regDetailList, IList<PatientPCLRequest> pclRequestList, IList<PatientRegistrationDetail> deletedRegDetailList, IList<PatientPCLRequest> deletedPclRequestList)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddServicesAndPCLRequests(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID, null,
                            Registration_DataStorage.CurrentPatientRegistration, regDetailList, pclRequestList, deletedRegDetailList, deletedPclRequestList, IsNotCheckInvalid,
                            false, default(DateTime), false, false, null, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long NewRegistrationID = 0;
                                    IList<PatientRegistrationDetail> SavedRegistrationDetailList;
                                    IList<PatientPCLRequest> SavedPclRequestList;
                                    V_RegistrationError error = V_RegistrationError.mNone;
                                    contract.EndAddServicesAndPCLRequests(out NewRegistrationID, out SavedRegistrationDetailList, out SavedPclRequestList, out error, asyncResult);

                                    if (SavedPclRequestList != null && SavedPclRequestList.Count > 0)
                                    {
                                        //Phát sự kiện load lại danh sách yêu cầu PCL
                                        Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });

                                        FormInputIsEnabled = false;
                                        PCLRequestDetailsContent.IsEnableListPCL = false;
                                        StatusForm(AllLookupValues.StatusForm.HIEUCHINH);

                                        //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu
                                        //CurrentPclRequest = SavedPclRequestList[0];
                                        //ObjPatientPCLRequestNew_BackUp = ObjectCopier.DeepCopy(CurrentPclRequest);

                                        //CurrentPclRequest = ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);

                                        //SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                                        //PatientPCLRequestDetail_ByPatientPCLReqID();
                                        //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu

                                        PatientPCLRequest_RequestLastest();

                                        //KMx: Sau khi thêm mới chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 16:21).
                                        GetPatientPCLRequestList_ByRegistrationID();
                                        IsNotCheckInvalid = false;

                                        // VuTTM - QMS Service
                                        // Creating the registration / PCL request orders
                                        // QTD - Thêm cấu hình bật/tắt QMS
                                        if(Globals.ServerConfigSection.CommonItems.IsEnableQMSForPCL)
                                        {
                                            bool isPaid = false;
                                            bool hasCashierOrder = true;
                                            bool canUpdateSeqNumber = true;
                                            GlobalsNAV.CreateOrders(isPaid, hasCashierOrder, regInfo.Patient, ref SavedRegistrationDetailList, ref SavedPclRequestList,
                                                canUpdateSeqNumber);
                                        }
                                        if (SavedPclRequestList.Count == 1)
                                        {
                                            //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1045_G1_DaLuuPhYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1045_G1_DaLuuPhYC);
                                            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                                            {
                                                proAlloc.PatientPCLReqID = SavedPclRequestList.FirstOrDefault().PatientPCLReqID;
                                                proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                                                proAlloc.V_RegistrationType = V_RegistrationType;
                                            };
                                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                                        }
                                        else
                                        {
                                            //MessageBox.Show(string.Format(eHCMSResources.Z1061_G1_LuuPhYCTachPh, SavedPclRequestList.Count.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                            GlobalsNAV.ShowMessagePopup(string.Format(eHCMSResources.Z1061_G1_LuuPhYCTachPh, SavedPclRequestList.Count.ToString()));
                                            //KMx: Nếu có 2 phiếu trở lên thì in gộp (24/03/2014 16:31).
                                            strPCLRequestIDList = GetPCLRequestIDList(SavedPclRequestList.ToList());
                                            StartViewPrintAllLabPCL();
                                            strPCLRequestIDList = null;

                                        }
                                        var homeVm = Globals.GetViewModel<IHome>();
                                        if (homeVm.OutstandingTaskContent != null && homeVm.OutstandingTaskContent is IConsultationOutstandingTask)
                                        {
                                            ((IConsultationOutstandingTask)homeVm.OutstandingTaskContent).SearchRegistrationListForOST();
                                        }
                                        PatientPCLRequestDetail_ByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0477_G1_LuuKhongThanhCong), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //▼===== #007
                                    //TBL: 19090601 là ID để xác định thông báo
                                    if (ex.Message.Contains("19090601") && MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                                    {
                                        //20191025 TBL: Nếu đồng ý lưu thì lưu lại và bỏ qua kiểm tra dưới store
                                        IsNotCheckInvalid = true;
                                        btSave();
                                    }
                                    else if (!ex.Message.Contains("19090601"))
                                    {
                                        Globals.ShowMessage(ex.Message, eHCMSResources.G0442_G1_TBao);
                                    }
                                    //▲===== #007
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
                }
            });
            t.Start();
        }

        public void AddServicesAndPCLRequests_Update(PatientRegistration regInfo, IList<PatientRegistrationDetail> regDetailList, IList<PatientPCLRequest> pclRequestList, IList<PatientRegistrationDetail> deletedRegDetailList, IList<PatientPCLRequest> deletedPclRequestList)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddServicesAndPCLRequests(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID, null, regInfo,
                            regDetailList, pclRequestList, deletedRegDetailList, deletedPclRequestList, IsNotCheckInvalid, true, default(DateTime), false, false, null, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long NewRegistrationID = 0;
                                    IList<PatientRegistrationDetail> SavedRegistrationDetailList;
                                    IList<PatientPCLRequest> SavedPclRequestList;
                                    V_RegistrationError error = V_RegistrationError.mNone;
                                    contract.EndAddServicesAndPCLRequests(out NewRegistrationID, out SavedRegistrationDetailList, out SavedPclRequestList, out error, asyncResult);

                                    //Phát sự kiện load lại danh sách yêu cầu PCL
                                    Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });

                                    FormInputIsEnabled = false;
                                    PCLRequestDetailsContent.IsEnableListPCL = false;
                                    StatusForm(AllLookupValues.StatusForm.HIEUCHINH);

                                    //KMx: Sau khi cập nhật chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 16:22).
                                    GetPatientPCLRequestList_ByRegistrationID();

                                    //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu
                                    if (SavedPclRequestList != null && SavedPclRequestList.Count > 0)
                                    {
                                        CurrentPclRequest = SavedPclRequestList[0];
                                        ObjPatientPCLRequestNew_BackUp = Common.ObjectCopier.DeepCopy(CurrentPclRequest);
                                        CurrentPclRequest = Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                                    }

                                    SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                                    PatientPCLRequestDetail_ByPatientPCLReqID();
                                    //Load lại chi tiết phiếu vì khi bị tách phiếu in sẽ thấy khó hiểu

                                    if (SavedPclRequestList != null && SavedPclRequestList.Count > 0)
                                    {
                                        //MessageBox.Show(string.Format(eHCMSResources.Z1062_G1_LuuPhYC, SavedPclRequestList.Count.ToString()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        GlobalsNAV.ShowMessagePopup(string.Format(eHCMSResources.Z1062_G1_LuuPhYC, SavedPclRequestList.Count.ToString()));
                                        if (SavedPclRequestList.Count == 1)
                                        {
                                            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
                                            {
                                                proAlloc.PatientPCLReqID = SavedPclRequestList.FirstOrDefault().PatientPCLReqID;
                                                proAlloc.eItem = ReportName.RptPatientPCLRequestDetailsByPatientPCLReqID;
                                                proAlloc.V_RegistrationType = V_RegistrationType;
                                            };
                                            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                                        }
                                        else
                                        {
                                            //KMx: Nếu có 2 phiếu trở lên thì in gộp (24/03/2014 16:31).
                                            strPCLRequestIDList = GetPCLRequestIDList(SavedPclRequestList.ToList());
                                            StartViewPrintAllLabPCL();
                                            strPCLRequestIDList = null;
                                        }
                                    }
                                    else
                                    {
                                        //MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1045_G1_DaLuuPhYC), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        GlobalsNAV.ShowMessagePopup(string.Format("{0}!", eHCMSResources.Z1045_G1_DaLuuPhYC));
                                    }
                                    PatientPCLRequestDetail_ByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                }
            });
            t.Start();
        }

        #endregion

        #region "Trạng Thái Form"
        private void StatusForm(AllLookupValues.StatusForm status)
        {
            switch (status)
            {
                case AllLookupValues.StatusForm.TAOMOI:
                    {
                        btNewIsEnabled = false;
                        btEditIsEnabled = false;
                        btXoaIsEnabled = false;
                        btCancelIsEnabled = true;
                        btSaveIsEnabled = true;
                        btInIsEnabled = false;
                        break;
                    }
                case AllLookupValues.StatusForm.HIEUCHINH:
                    {
                        btNewIsEnabled = true;
                        btEditIsEnabled = true && (CurrentPclRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN);
                        NotifyOfPropertyChange(() => btEditIsEnabled);
                        btXoaIsEnabled = true && (CurrentPclRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN);
                        NotifyOfPropertyChange(() => btXoaIsEnabled);
                        btCancelIsEnabled = false;
                        btSaveIsEnabled = false;
                        btInIsEnabled = true;
                        break;
                    }
                //▼=====: Bổ sung thêm chế độ xem: chỉ có nút tạo mới.
                case AllLookupValues.StatusForm.XEM:
                    {
                        btNewIsEnabled = true;
                        btEditIsEnabled = false;
                        btXoaIsEnabled = false;
                        btCancelIsEnabled = false;
                        btSaveIsEnabled = false;
                        btInIsEnabled = false;
                        break;
                    }
            }
        }

        private void ButtonClicked(AllLookupValues.ButtonClicked status)
        {
            switch (status)
            {
                case AllLookupValues.ButtonClicked.TAOMOI:
                    {
                        btNewIsEnabled = false;
                        btEditIsEnabled = false;
                        btXoaIsEnabled = false;
                        btCancelIsEnabled = true;
                        btSaveIsEnabled = true;
                        btInIsEnabled = false;
                        break;
                    }
                case AllLookupValues.ButtonClicked.HIEUCHINH:
                    {
                        btNewIsEnabled = false;
                        btEditIsEnabled = false;
                        btXoaIsEnabled = false;
                        btCancelIsEnabled = true;
                        btSaveIsEnabled = true;
                        btInIsEnabled = false;
                        break;
                    }
                case AllLookupValues.ButtonClicked.BOQUA:
                    {
                        FormInputIsEnabled = false;
                        PCLRequestDetailsContent.IsEnableListPCL = false;
                        btNewIsEnabled = true;

                        if (CurrentPclRequest.PatientPCLReqID > 0)
                        {
                            btEditIsEnabled = true && (CurrentPclRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN);
                            NotifyOfPropertyChange(() => btEditIsEnabled);
                            btXoaIsEnabled = true && (CurrentPclRequest.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN);
                            NotifyOfPropertyChange(() => btXoaIsEnabled);

                            btInIsEnabled = true;
                        }
                        else
                        {
                            btEditIsEnabled = false;
                            btXoaIsEnabled = false;
                            btInIsEnabled = false;
                        }

                        btCancelIsEnabled = false;
                        btSaveIsEnabled = false;
                        IsEnabledtxbDiagnosis = false;
                        break;
                    }
            }
        }
        #endregion


        public void cbx_TestingAgency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedItem != null)
            {
                TestingAgency Objtmp = ((sender as ComboBox).SelectedItem as TestingAgency);
                if (Objtmp != null && Objtmp.HosID > 0)
                {
                    //lay danh sach Examtype add vao
                    PCLExamType_ByHosID(Objtmp.HosID.GetValueOrDefault(), HasGroup);
                }
            }

        }



        public void Handle(PCLRequestDetailRemoveEvent<PatientPCLRequestDetail> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    PatientPCLRequestDetail tmp = message.PCLRequestDetail;

                    if (tmp.EntityState == EntityState.DETACHED || tmp.RecordState == RecordState.DETACHED)
                    {
                        CurrentPclRequest.PatientPCLRequestIndicators.Remove(tmp);
                    }
                }
            }
        }

        public void Handle(ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM message)
        {
            InitPatientInfo(message.Patient);
        }

        //public void Handle(ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU<Patient, PatientRegistration, PatientRegistrationDetail> message)
        //{
        //    InitPatientInfo(message.Pt);
        //}

        public Dictionary<long, PCLExamType> _dicPCLExamType;
        public Dictionary<long, PCLExamType> dicPCLExamType
        {
            get
            {
                return _dicPCLExamType;
            }
            set
            {
                _dicPCLExamType = value;
                NotifyOfPropertyChange(() => dicPCLExamType);

            }
        }


        public void PCLExamTypeName_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox ctr = (sender as TextBox);
            string str = ctr.Text.Trim();
            LoadPCLExamTypesLinq(str);

        }
        private void LoadPCLExamTypesLinq(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                var resComBo = (from c in dicPCLExamType
                                where
                                (Common.Utilities.VNConvertString.ConvertString(c.Value.PCLExamTypeName)
                                .ToLower().Contains(Common.Utilities.VNConvertString.ConvertString(str).ToLower()))
                                select c);

                List<PCLExamType> allPclType = new List<PCLExamType>();

                foreach (var item in resComBo)
                {
                    allPclType.Add(item.Value);
                }

                ObjPCLItems_ByPCLFormID = new PagedCollectionView(allPclType);
            }
            else
            {
                ObjPCLItems_ByPCLFormID = null;
            }
        }

        //KMx: Biến này dùng để in tổng hợp (CLS Hình Ảnh) và truyền sang PCLRequestDetailsViewModel để hiển thị lên giao diện (những chỉ định của 1 ĐK) (21/03/2014 15:47).
        private ObservableCollection<PatientPCLRequest> _PatientPCLRequest_ByRegistrationID;
        public ObservableCollection<PatientPCLRequest> PatientPCLRequest_ByRegistrationID
        {
            get { return _PatientPCLRequest_ByRegistrationID; }
            set
            {
                _PatientPCLRequest_ByRegistrationID = value;
                NotifyOfPropertyChange(() => PatientPCLRequest_ByRegistrationID);
            }
        }

        //KMx: Lấy toàn bộ chỉ định CLS của đăng ký (24/03/2014 15:56).
        private void GetPatientPCLRequestList_ByRegistrationID()
        {
            PatientPCLRequest_ByRegistrationID = new ObservableCollection<PatientPCLRequest>();
            if (IsShowSummaryContent)
                this.ShowBusyIndicator();
            else
                this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetPatientPCLRequestList_ByRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, V_RegistrationType, V_PCLMainCategory, Globals.DispatchCallback((asyncResult) =>
                        {
                            var allItems = client.EndGetPatientPCLRequestList_ByRegistrationID(asyncResult);
                            if (allItems != null && allItems.Count > 0)
                            {
                                foreach (var item in allItems)
                                {
                                    PatientPCLRequest_ByRegistrationID.Add(item);
                                }
                            }
                            //KMx: Truyền PCL Request List vào PCLRequestDetailsView để hiển thị (24/03/2014 16:48)
                            PCLRequestDetailsContent.ObjPatientPCLRequest_ByRegistrationID = PatientPCLRequest_ByRegistrationID;
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if (IsShowSummaryContent)
                        this.HideBusyIndicator();
                    else
                        this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        private void StartViewPrintAllLabPCL()
        {
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.Result = strPCLRequestIDList;
                proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                proAlloc.eItem = ReportName.PatientPCLRequestDetailsByPatientPCLReqID_XML;
                proAlloc.V_RegistrationType = V_RegistrationType;
                proAlloc.TieuDeRpt = eHCMSResources.P0383_G1_PhYeuCauXetNghiem;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }


        private string strPCLRequestIDList = string.Empty;

        private string GetPCLRequestIDList(List<PatientPCLRequest> ListItems)
        {
            StringBuilder sb = new StringBuilder();

            if (ListItems != null && ListItems.Count > 0)
            {
                sb.Append("<Root>");
                foreach (var PCLRequest in ListItems)
                {
                    sb.Append("<PCLReqIDList>");
                    sb.AppendFormat("<PCLReqID>{0}</PCLReqID>", PCLRequest.PatientPCLReqID);
                    sb.Append("</PCLReqIDList>");
                }
                sb.Append("</Root>");
            }
            else
            {
                return string.Empty;
            }

            return sb.ToString();
        }

        public bool IsAllowToUpdateDiagnosis
        {
            get
            {
                return !btSaveIsEnabled && V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU
                    && (CallByPCLRequestViewModel == LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM) && Globals.ServerConfigSection.ConsultationElements.AllowToUpdateDiagnosisIntoPCLReq;
            }
        }

        private LeftModuleActive _CallByPCLRequestViewModel = LeftModuleActive.NONE;
        public LeftModuleActive CallByPCLRequestViewModel
        {
            get
            {
                return _CallByPCLRequestViewModel;
            }
            set
            {
                _CallByPCLRequestViewModel = value;
                NotifyOfPropertyChange(() => CallByPCLRequestViewModel);
                NotifyOfPropertyChange(() => IsAllowToUpdateDiagnosis);
            }
        }

        //HPT : cập nhật tên bác sĩ chỉ định và chẩn đoán
        public void UpdateDrAndDiagnosisForPCLRequest()
        {
            if (Registration_DataStorage.PatientServiceRecordCollection == null || Registration_DataStorage.PatientServiceRecordCollection.Count() <= 0)
            {
                return;
            }
            if (CurrentPclRequest == null || CurrentPclRequest.PatientPCLReqID <= 0)
            {
                return;
            }
            if (Globals.DoctorStaffs == null || Globals.DoctorStaffs.Count() <= 0)
            {
                return;
            }
            if (!Globals.DoctorStaffs.Any(x => x.StaffID == Globals.LoggedUserAccount.StaffID))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z1058_G1_BSiMoiCNhatTTinPhChiDinh));
                return;
            }
            IsWaitingSave = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateDrAndDiagTrmtForPCLReq(Registration_DataStorage.PatientServiceRecordCollection.FirstOrDefault().ServiceRecID, CurrentPclRequest.PatientPCLReqID, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentPclRequest = contract.EndUpdateDrAndDiagTrmtForPCLReq(asyncResult);

                                Globals.EventAggregator.Publish(new ReLoadListPCLRequest { });
                                FormInputIsEnabled = false;
                                PCLRequestDetailsContent.IsEnableListPCL = false;
                                StatusForm(AllLookupValues.StatusForm.HIEUCHINH);
                                //KMx: Sau khi cập nhật chỉ định CLS thành công thì load lại danh sách phiếu (24/03/2014 10:32).
                                GetPatientPCLRequestList_ByRegistrationID();

                                SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                                PatientPCLRequestDetail_ByPatientPCLReqID();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            finally
                            {
                                IsWaitingSave = false;
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

        public bool RequireDiagnosisForPCLReq
        {
            get
            {
                if (Globals.ServerConfigSection.Pcls.RequireDiagnosisForPCLReq == 0)
                {
                    return false;
                }
                if (Globals.ServerConfigSection.Pcls.RequireDiagnosisForPCLReq == (int)AllLookupValues.RequireDiagnosisForPCLReq.DIAG_FOR_PCLREQ)
                {
                    return false;
                }
                return (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU && Globals.ServerConfigSection.Pcls.RequireDiagnosisForPCLReq == (int)AllLookupValues.RequireDiagnosisForPCLReq.DIAG_FOR_PCLREQ_OUTPATIENT)
                    || (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU && Globals.ServerConfigSection.Pcls.RequireDiagnosisForPCLReq == (int)AllLookupValues.RequireDiagnosisForPCLReq.DIAG_FOR_PCLREQ_INPATIENT);
            }
        }
        private DateTime? _gMedicalInstructionDate = Globals.GetCurServerDateTime().Date;
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
        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
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
        private bool _RequireDoctorAndDate = Globals.ServerConfigSection.ClinicDeptElements.RequireDoctorAndDateForMed;
        public bool RequireDoctorAndDate
        {
            get
            {
                return _RequireDoctorAndDate;
            }
            set
            {
                if (_RequireDoctorAndDate != value) return;
                _RequireDoctorAndDate = value;
                NotifyOfPropertyChange(() => RequireDoctorAndDate);
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
        private bool _mPCL_SuaSauHoanTat = false;
        public bool mPCL_SuaSauHoanTat
        {
            get
            {
                return _mPCL_SuaSauHoanTat;
            }
            set
            {
                if (_mPCL_SuaSauHoanTat == value)
                {
                    return;
                }
                _mPCL_SuaSauHoanTat = value;
                NotifyOfPropertyChange(() => mPCL_SuaSauHoanTat);
            }
        }
        public void UpdateConsultationAfterCompleteCmd()
        {
            Action<IDiagnosisTextBox> onInitDlg = delegate (IDiagnosisTextBox proAlloc)
            {
                proAlloc.CurrentPclRequest = CurrentPclRequest;
            };
            GlobalsNAV.ShowDialog<IDiagnosisTextBox>(onInitDlg);
        }
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
                if (SPTheoAutoComplete == null)
                {
                    return;
                }
                SPTheoAutoComplete.CS_DS = CS_DS;
            }
        }
        private long _PCLComboID;
        public long PCLComboID
        {
            get
            {
                return _PCLComboID;
            }
            set
            {
                _PCLComboID = value;
                NotifyOfPropertyChange(() => PCLComboID);
            }
        }
        public bool IsRegimenChecked
        {
            get
            {
                return SPTheoAutoComplete != null ? SPTheoAutoComplete.IsRegimenChecked : false;
            }
            set
            {
                if (SPTheoAutoComplete == null || SPTheoAutoComplete.IsRegimenChecked == value)
                {
                    return;
                }
                SPTheoAutoComplete.IsRegimenChecked = value;
                NotifyOfPropertyChange(() => IsRegimenChecked);
                if (ObjV_PCLMainCategory_Selected == null)
                {
                    return;
                }
                if (ObjV_PCLMainCategory_Selected.LookupID == IDLABByName_Bo)
                {
                    PCLExamTypeByComboIDLingq(PCLComboID, HasGroup);
                }
                else if (ObjPCLForms_GetList_Selected != null)
                {
                    PCLItemsByPCLFormID_Lingq(ObjPCLForms_GetList_Selected.PCLFormID, HasGroup);
                }
            }
        }
        private void LoadAllPclExamTypesAction(InPatientAdmDisDetails CurrentInPatientAdmDisDetail)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        var criteria = new PCLExamTypeSearchCriteria { V_PCLMainCategory = 0 };
                        client.BeginPCLItems_ByPCLFormID(criteria, 0, CurrentInPatientAdmDisDetail.PCLExamTypePriceListID, Globals.DispatchCallback(delegate (IAsyncResult asyncResult)
                        {
                            try
                            {
                                List<PCLExamType> ListAllPclExamTypes = new List<PCLExamType>();
                                ListAllPclExamTypes = client.EndPCLItems_ByPCLFormID(asyncResult).ToList();
                                List<PCLExamType> listpcl = new List<PCLExamType>();
                                foreach (PCLExamType pclItem in ListAllPclExamTypes)
                                {
                                    if (pclItem.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory)
                                    {
                                        listpcl.Add(pclItem);
                                    }
                                }
                                CurrentListPclExamTypesAllPCLForms = listpcl;
                                if (SPTheoAutoComplete != null)
                                {
                                    SPTheoAutoComplete.SetDataForAutoComplete(CurrentListPclExamTypesAllPCLForms, CurrentListPclExamTypesAllCombos
                                        , Globals.ListPclExamTypesAllPCLFormImages);
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadAllPCLExamTypesByComboIDAction(InPatientAdmDisDetails CurrentInPatientAdmDisDetail)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPCLExamTypeComboItems_All(CurrentInPatientAdmDisDetail.PCLExamTypePriceListID, Globals.DispatchCallback(delegate (IAsyncResult asyncResult)
                        {
                            try
                            {
                                IList<PCLExamTypeComboItem> allItems = client.EndPCLExamTypeComboItems_All(asyncResult);
                                if (allItems == null)
                                {
                                    allItems = new List<PCLExamTypeComboItem>();
                                }
                                CurrentListPclExamTypesAllCombos = allItems.ToList();
                                if (SPTheoAutoComplete != null)
                                {
                                    SPTheoAutoComplete.SetDataForAutoComplete(CurrentListPclExamTypesAllPCLForms, CurrentListPclExamTypesAllCombos
                                        , Globals.ListPclExamTypesAllPCLFormImages);
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
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
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private IList<PCLExamType> CurrentListPclExamTypesAllPCLForms = Globals.ListPclExamTypesAllPCLForms;
        private IList<PCLExamTypeComboItem> CurrentListPclExamTypesAllCombos = Globals.ListPclExamTypesAllCombos;

        //▼===== #006
        public void Handle(DbClickSelectedObjectEventWithKeyToShowDetails<PatientPCLRequest, string> message)
        {
            DoctorComments = "";
            if (this.GetView() != null)
            {
                if (message != null && message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    ObjPatientPCLRequestNew_BackUp = Common.ObjectCopier.DeepCopy(message.ObjA);

                    CurrentPclRequest = Common.ObjectCopier.DeepCopy(ObjPatientPCLRequestNew_BackUp);
                    SearchCriteriaDetail.PatientPCLReqID = CurrentPclRequest.PatientPCLReqID;
                    PatientPCLRequestDetail_ByPatientPCLReqID();

                    InitLoadData(true);
                    if (!Globals.isConsultationStateEdit)
                    {
                        FormIsEnabled = false;
                        return;
                    }
                    else
                    {
                        FormIsEnabled = true;
                        FormInputIsEnabled = false;
                        PCLRequestDetailsContent.IsEnableListPCL = false;
                        StatusForm(AllLookupValues.StatusForm.HIEUCHINH);
                    }

                }
            }
        }
        //▲===== #006
        private bool _IsShowCheckBoxPayAfter = true;
        public bool IsShowCheckBoxPayAfter
        {
            get { return _IsShowCheckBoxPayAfter; }
            set
            {
                if (_IsShowCheckBoxPayAfter != value)
                {
                    _IsShowCheckBoxPayAfter = value;
                    NotifyOfPropertyChange(() => IsShowCheckBoxPayAfter);
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
                if (SPTheoAutoComplete != null)
                {
                    SPTheoAutoComplete.Registration_DataStorage = Registration_DataStorage;
                }
                PCLRequestDetailsContent.Registration_DataStorage = Registration_DataStorage;
            }
        }
        //▼===== 20190927 TTM: Đưa sự kiện bắn dịch vụ sang Grid thành 1 sự kiện khác 
        //                Dành riêng cho màn hình hẹn bệnh
        //                Lý do: Vì màn hình PatientPCLRequestEditViewModel vừa là màn hình cha
        //                ở các tab vừa là popup ở hẹn bệnh nếu dùng cùng 1 sự kiện sẽ thực hiện 2 lần.
        public void Handle(SelectedObjectEventWithKeyAppt<PCLExamType, String> message)
        {
            if (!IsAppointment)
            {
                return;
            }
            if (message != null)
            {
                if (this.GetView() != null)
                {
                    if (message.Key == eHCMSResources.Z0055_G1_Edit)
                    {
                        SelectedItemForChoose = message.Result;
                        AddOne();
                    }
                }
            }
        }
        //▲=====

        #region Ngày y lệnh
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
        //public void Handle(PCLExamAccordingICD_Event message)
        //{
        //    if (message != null && Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null 
        //            && Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatient != null)
        //    {
        //        btNew();
        //        foreach (var item in message.ListPCLExamAccordingICD.Where(x => x.V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.Laboratory))
        //        {
        //            AddOne(item);
        //        }
        //    }
        //}

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
    }
}