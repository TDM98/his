using Caliburn.Micro;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using aEMR.Infrastructure.Events;
using System.Collections.Generic;
using aEMR.CommonTasks;
using eHCMSLanguage;
using System.Linq;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Common.BaseModel;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using aEMR.Common.HotKeyManagement;
using aEMR.Common.ViewModels;
using aEMR.ServiceClient.Consultation_PCLs;
using System.ServiceModel;
using aEMR.DataContracts;

/*
 * 20180920 #001 TBL:   Chinh sua chuc nang bug mantis ID 0000061
 * 20180923 #002 TTM:   Chuyển thông tin Patient xuống cho màn hình con, vì trước đây nó là màn hình độc lập => Khi nó được gọi thì nó chạy khởi tạo (code lấy dữ liệu nằm ở hàm khởi tạo)
 *                      Bây giờ là màn hình con nên không thể chạy khởi tạo để lấy dữ liệu (khởi tạo cùng lúc với màn hình cha ConsultationSummary). 
 *                      Nên cần 1 hàm để chuyển dữ liệu xuống để hoạt động.
 * 20180923 #003 TBL: BM 0000066: Added out long DTItemID and set OutServiceRecID, DTItemID
 * 20180926 #004 TBL: BM 0000077. 
 * 20181016 #005 TBL: BM 0002170: Lay phac do tu dong khi chuyen qua tab Ra toa
 * 20181020 #006 TBL: BM 0003196: Hien thi ten dich vu o Expander
 * 20181023 #007 TBL: BM 0003206: Khi luu can phai biet la them moi hay cap nhat
 * 20181029 #008 TBL: BM 0004208: Chinh sua toa thuoc khi vua luu thi toa thuoc khong duoc tinh BH
 * 20181115 #009 TBL: BM 0005264: Khi qua tab co nut luu thi an nut luu tong di
 * 20181121 #010 TTM: Tạo mới out standing task ở màn hình sử dụng, không còn tạo từ Module nữa
 * 20190531 #011 TTM: BM 0006684: Xây dựng màn hình cập nhật chẩn đoán ra toa - báo cáo BHYT
 * 20190727 #012 TBL: BM 0012974: Tạo mới dựa trên cũ cho thủ thuật
 * 20190822 #013 TTM: BM 0013186: Fix lỗi khoá trắng màn hình chỉ định nội ngoại trú khi chuyển từ thông tin chung sang màn hình khám bệnh.
 * 20190909 #014 TNHX: BM0013263: Update func print SmallProcedure
 * 20210228 #015 TNHX: 219 Add config AllowFirstHIExaminationWithoutPay
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    public class CS_DataStorage : ICS_DataStorage
    {
        private PatientMedicalRecord _PatientMedicalRecordInfo = null;
        public PatientMedicalRecord PatientMedicalRecordInfo
        {
            get
            {
                return _PatientMedicalRecordInfo;
            }
            set
            {
                if (_PatientMedicalRecordInfo != value)
                {
                    _PatientMedicalRecordInfo = value;
                }
            }
        }

        private bool _Getting_PatientMedicalRecordInfo = false;
        public bool Getting_PatientMedicalRecordInfo
        {
            get
            {
                return _Getting_PatientMedicalRecordInfo;
            }
            set
            {
                _Getting_PatientMedicalRecordInfo = value;
            }
        }

        private Patient _CurrentPatient = null;
        public Patient CurrentPatient
        {
            get
            {
                return _CurrentPatient;
            }
            set
            {
                _CurrentPatient = value;
            }
        }
        private PatientRegistration _CurrentRegistration = null;
        public PatientRegistration CurrentRegistration
        {
            get
            {
                return _CurrentRegistration;
            }
            set
            {
                _CurrentRegistration = value;
            }
        }
        private DiagnosisTreatment _DiagTreatment = null;
        public DiagnosisTreatment DiagTreatment
        {
            get
            {
                return _DiagTreatment;
            }
            set
            {
                _DiagTreatment = value;
            }
        }

        private ObservableCollection<DiagnosisIcd10Items> _refIDC10List = null;
        public ObservableCollection<DiagnosisIcd10Items> refIDC10List
        {
            get
            {
                return _refIDC10List;
            }
            set
            {
                _refIDC10List = value;
            }
        }
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
    }
    [Export(typeof(IConsultationsSummary)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationsSummaryViewModel : ViewModelBase, IConsultationsSummary
        , IHandle<ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        [ImportingConstructor]
        public ConsultationsSummaryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            System.Diagnostics.Debug.WriteLine("===> ConsultationsSummaryViewModel - Constructor");
            base.HasInputBindingCmd = true;
            Authorization();
            var homevm = Globals.GetViewModel<IHome>();
            DiagnosisTreatmentHistoriesTreeContent = Globals.GetViewModel<IDiagnosisTreatmentHistoriesTree>();
            homevm.LeftMenu = DiagnosisTreatmentHistoriesTreeContent;
            ActivateItem(DiagnosisTreatmentHistoriesTreeContent);
            CreateSubVM();
        }

        ~ConsultationsSummaryViewModel()
        {
            System.Diagnostics.Debug.WriteLine("===> ConsultationsSummaryViewModel - Destructor");
        }

        private bool VMObjAlreadyCleaned = false;
        private void CleanUpDataStorage()
        {
            if (VMObjAlreadyCleaned)
                return;
            VMObjAlreadyCleaned = true;

            UCPatientProfileInfo.CS_DS = null;
            UCSummary.CS_DS = null;
            ((IPtRegDetailInfo)UCPtRegDetailInfo).CS_DS = null;
            UCePrescriptions.CS_DS = null;
            UCCommonRecs.CS_DS = null;
            UCPatientTreeForm.CS_DS = null;
            ((ISearchPatientAndRegistration)SearchRegistrationContent).CS_DS = null;
            UCDoctorProfileInfo.CS_DS = null;
            UCPatientPCLRequest.CS_DS = null;
            UCPatientPCLRequestImage.CS_DS = null;
            UCPatientPCLLaboratoryResult.CS_DS = null;
            UCPatientPCLImagingResult.CS_DS = null;
            UCPatientPCLDeptImagingExtHome.CS_DS = null;
            CS_DS = null;

            UCPatientProfileInfo = null;
            UCSummary = null;
            UCPtRegDetailInfo = null;
            UCePrescriptions = null;
            UCCommonRecs = null;
            UCPatientTreeForm = null;
            SearchRegistrationContent = null;
            UCDoctorProfileInfo = null;
            UCPatientPCLRequest = null;
            UCPatientPCLRequestImage = null;
            UCPatientPCLLaboratoryResult = null;
            UCPatientPCLImagingResult = null;
            UCPatientPCLDeptImagingExtHome = null;

        }

        #region Properties
        private IDiagnosisTreatmentHistoriesTree _DiagnosisTreatmentHistoriesTreeContent;
        public IDiagnosisTreatmentHistoriesTree DiagnosisTreatmentHistoriesTreeContent
        {
            get
            {
                return _DiagnosisTreatmentHistoriesTreeContent;
            }
            set
            {
                if (_DiagnosisTreatmentHistoriesTreeContent == value)
                {
                    return;
                }
                _DiagnosisTreatmentHistoriesTreeContent = value;
                NotifyOfPropertyChange(() => DiagnosisTreatmentHistoriesTreeContent);
            }
        }
        public IPatientInfo UCPatientProfileInfo { get; set; }
        public object SearchRegistrationContent { get; set; }
        public IPtRegDetailInfo UCPtRegDetailInfo { get; set; }
        public ISummary_V2 UCSummary { get; set; }
        public IePrescriptions UCePrescriptions { get; set; }
        public ICommonRecs UCCommonRecs { get; set; }
        public IPatientTreeForm UCPatientTreeForm { get; set; }
        public ILoginInfo UCDoctorProfileInfo { get; set; }

        public IPatientPCLRequest UCPatientPCLRequest { get; set; }
        public IPatientPCLRequestImage UCPatientPCLRequestImage { get; set; }
        public IPatientPCLLaboratoryResult UCPatientPCLLaboratoryResult { get; set; }
        public IPatientPCLImagingResult UCPatientPCLImagingResult { get; set; }
        public IPatientPCLDeptImagingExtHome UCPatientPCLDeptImagingExtHome { get; set; }
        //public IPatientMedicalServiceRequest UCPatientMedicalServiceRequest { get; set; }

        private bool _mTimBN;
        private bool _mThemBN = true;
        private bool _mTimDangKy = true;

        public bool mTimBN
        {
            get
            {
                return _mTimBN;
            }
            set
            {
                if (_mTimBN == value)
                    return;
                _mTimBN = value;
            }
        }
        public bool mThemBN
        {
            get
            {
                return _mThemBN;
            }
            set
            {
                if (_mThemBN == value)
                    return;
                _mThemBN = value;
            }
        }
        public bool mTimDangKy
        {
            get
            {
                return _mTimDangKy;
            }
            set
            {
                if (_mTimDangKy == value)
                    return;
                _mTimDangKy = value;
            }
        }

        public bool IsShowEditTinhTrangTheChat
        {
            get
            {
                return UCSummary.IsShowEditTinhTrangTheChat;
            }
            set
            {
                UCSummary.IsShowEditTinhTrangTheChat = value;
            }
        }
        TabControl tcMainCommon { get; set; }
        private string _ReasonChangeStatus;
        public string ReasonChangeStatus
        {
            get { return _ReasonChangeStatus; }
            set
            {
                _ReasonChangeStatus = value;
                NotifyOfPropertyChange(() => ReasonChangeStatus);
            }
        }
        private Registration_DataStorage _Registration_DataStorage;
        public Registration_DataStorage Registration_DataStorage
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
                UCPatientPCLRequest.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLRequestImage.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLLaboratoryResult.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLImagingResult.Registration_DataStorage = Registration_DataStorage;
                UCSummary.Registration_DataStorage = Registration_DataStorage;
                UCePrescriptions.Registration_DataStorage = Registration_DataStorage;
                UCPatientTreeForm.Registration_DataStorage = Registration_DataStorage;
                //UCPatientMedicalServiceRequest.Registration_DataStorage = Registration_DataStorage;
                UCCommonRecs.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLDeptImagingExtHome.Registration_DataStorage = Registration_DataStorage;
                UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
                DiagnosisTreatmentHistoriesTreeContent.Registration_DataStorage = Registration_DataStorage;
            }
        }
        private ObservableCollection<DiagnosisIcd10Items> _refICD10List;
        public ObservableCollection<DiagnosisIcd10Items> refICD10List
        {
            get
            {
                return _refICD10List;
            }
            set
            {
                if (_refICD10List != value)
                {
                    _refICD10List = value;
                    NotifyOfPropertyChange(() => refICD10List);
                }
            }
        }
        #endregion
        #region Handles
        public void Handle(ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            //Clear CS_DS cho benh nhan moi
            if (CS_DS != null)
            {
                ClearDataCS_DS();
                ReasonChangeStatus = null;
            }
            if (message != null)
            {
                /*▼====: #006*/
                if (message.PtRegDetail != null)
                {
                    UCPatientProfileInfo.CS_DS.CurrentPatient.GeneralInfoString += string.Format("                                              [{0}]       CĐ sơ bộ: [{1}]", message.PtRegDetail.MedServiceName, message.PtReg.BasicDiagTreatment);
                }
                /*▲====: #006*/
                UCSummary.InitConsultationInfo(message.Pt);
                UCePrescriptions.InitPatientInfo();
                //UCPatientTreeForm.InitPatientInfo();
                UCCommonRecs.InitPatientInfo();
                UCPatientPCLRequest.InitPatientInfo();
                UCPatientPCLRequestImage.InitPatientInfo();
                //▼====== #002
                UCPatientPCLLaboratoryResult.InitPatientInfo(message.Pt);
                UCPatientPCLImagingResult.InitPatientInfo(message.Pt);
                //▲====== #002
                //20182411 TBL: Khi tim dang ky se set MedSerID de load cac loai Vaccine theo MedSerID
                MedSerID = message.PtRegDetail.MedServiceID.GetValueOrDefault();
                UCSummary.ApplySmallProcedure(new SmallProcedure());
                UCSummary.FormEditorIsEnabled = false;
                if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.MedServiceType != null && Registration_DataStorage.CurrentPatientRegistrationDetail.MedServiceType.MedicalServiceTypeID != 1)
                {
                    if (Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID > 0)
                    {
                        DiagnosisIcd10Items_Load(Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID, null, false);
                    }
                    else
                    {
                        GetSmallProcedure(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
                    }
                }
                GetRefTreatmentRegimensAndDetail(message.PtRegDetail);
                if (DiagnosisTreatmentHistoriesTreeContent != null && Registration_DataStorage.CurrentPatient != null)
                {
                    //20200323 TBL: Mặc định thời gian hiển thị lịch sử khám bệnh 6 tháng
                    DiagnosisTreatmentHistoriesTreeContent.ToDate = Globals.GetCurServerDateTime().AddMonths(-6);
                    DiagnosisTreatmentHistoriesTreeContent.FromDate = Globals.GetCurServerDateTime();
                    DiagnosisTreatmentHistoriesTreeContent.GetPatientServicesTreeView(Registration_DataStorage.CurrentPatient.PatientID);
                }
            }
        }
        #endregion
        public void ClearDataCS_DS()
        {
            CS_DS.DiagTreatment = new DiagnosisTreatment();
            CS_DS.refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
            CS_DS.TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
        }
        private void CreateSubVM()
        {
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCSummary = Globals.GetViewModel<ISummary_V2>();
            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            UCePrescriptions = Globals.GetViewModel<IePrescriptions>();
            UCePrescriptions.IsShowSummaryContent = false;
            UCCommonRecs = Globals.GetViewModel<ICommonRecs>();
            UCPatientTreeForm = Globals.GetViewModel<IPatientTreeForm>();
            UCCommonRecs.IsShowSummaryContent = false;
            UCPatientTreeForm.IsShowSummaryContent = false;
            var searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            //searchPatientAndRegVm.mTimBN = mTimBN;
            //searchPatientAndRegVm.mTimDangKy = mTimDangKy;
            if (Globals.PatientFindBy_ForConsultation == null)
            {
                Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            searchPatientAndRegVm.PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            searchPatientAndRegVm.CloseRegistrationFormWhenCompleteSelection = false;
            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 1)
            {
                searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN | SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                //20190718 TBL: Ben BS Huan khi bam enter se tim BN chu khong tim dang ky
                //searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
                searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            }
            else if (mTimBN)
            {
                searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
                searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            }
            else
            {
                searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
                searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            }
            searchPatientAndRegVm.IsSearchGoToKhamBenh = true;
            searchPatientAndRegVm.PatientFindByVisibility = true;
            searchPatientAndRegVm.CanSearhRegAllDept = true;
            searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            //20181018 TNHX: [BM0002186] turn on block find by name
            if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName)
            {
                searchPatientAndRegVm.IsAllowSearchingPtByName_Visible = true;
                searchPatientAndRegVm.IsSearchPtByNameChecked = false;
            }
            SearchRegistrationContent = searchPatientAndRegVm;
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo.isPreNoteTemp = true;
            UCPatientPCLRequest = Globals.GetViewModel<IPatientPCLRequest>();
            UCPatientPCLRequestImage = Globals.GetViewModel<IPatientPCLRequestImage>();
            UCPatientPCLLaboratoryResult = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
            UCPatientPCLImagingResult = Globals.GetViewModel<IPatientPCLImagingResult>();
            UCPatientPCLDeptImagingExtHome = Globals.GetViewModel<IPatientPCLDeptImagingExtHome>();
            UCPatientPCLRequest.IsShowSummaryContent = false;
            UCPatientPCLRequestImage.IsShowSummaryContent = false;
            UCPatientPCLLaboratoryResult.IsShowSummaryContent = false;
            UCPatientPCLImagingResult.IsShowSummaryContent = false;
            UCPatientPCLDeptImagingExtHome.IsShowSummaryContent = false;
            UCPatientProfileInfo.CS_DS = CS_DS;
            UCSummary.CS_DS = CS_DS;
            ((IPtRegDetailInfo)UCPtRegDetailInfo).CS_DS = CS_DS;
            UCePrescriptions.CS_DS = CS_DS;
            UCCommonRecs.CS_DS = CS_DS;
            UCPatientTreeForm.CS_DS = CS_DS;
            ((ISearchPatientAndRegistration)SearchRegistrationContent).CS_DS = CS_DS;
            UCDoctorProfileInfo.CS_DS = CS_DS;
            UCPatientPCLRequest.CS_DS = CS_DS;
            UCPatientPCLRequestImage.CS_DS = CS_DS;
            UCPatientPCLLaboratoryResult.CS_DS = CS_DS;
            UCPatientPCLImagingResult.CS_DS = CS_DS;
            UCPatientPCLDeptImagingExtHome.CS_DS = CS_DS;
            //UCPatientMedicalServiceRequest = Globals.GetViewModel<IPatientMedicalServiceRequest>();
            Registration_DataStorage = new Registration_DataStorage();
        }
        private void ActivateSubVM()
        {
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCSummary);
            ActivateItem(UCPtRegDetailInfo);
            ActivateItem(UCePrescriptions);
            ActivateItem(UCCommonRecs);
            ActivateItem(UCPatientTreeForm);
            ActivateItem(SearchRegistrationContent);
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCPatientPCLRequest);
            ActivateItem(UCPatientPCLRequestImage);
            ActivateItem(UCPatientPCLLaboratoryResult);
            ActivateItem(UCPatientPCLImagingResult);
            ActivateItem(UCPatientPCLDeptImagingExtHome);
        }
        private void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(UCSummary, close);
            DeactivateItem(UCPtRegDetailInfo, close);
            DeactivateItem(UCePrescriptions, close);
            DeactivateItem(UCCommonRecs, close);
            DeactivateItem(UCPatientTreeForm, close);
            DeactivateItem(SearchRegistrationContent, close);
            DeactivateItem(UCDoctorProfileInfo, close);
            DeactivateItem(UCPatientPCLRequest, close);
            DeactivateItem(UCPatientPCLRequestImage, close);
            DeactivateItem(UCPatientPCLLaboratoryResult, close);
            DeactivateItem(UCPatientPCLImagingResult, close);
            DeactivateItem(UCPatientPCLDeptImagingExtHome, close);
        }
        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateSubVM();
            Globals.EventAggregator.Subscribe(this);

            //▼====== #010
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = Globals.GetViewModel<IConsultationOutstandingTask>();
            homevm.IsExpandOST = true;
            if (SearchRegistrationContent == null)
            {
                SearchRegistrationContent = Globals.GetViewModel<ISearchPatientAndRegistration>();
            }
            ((ISearchPatientAndRegistration)SearchRegistrationContent).PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            //▲====== #010
            if (Globals.CrossRegionHospital == null || Globals.CrossRegionHospital.Count == 0)
            {
                LoadCrossRegionHospitals();
            }
            //▼===== #013
            setDataPatientForRequestPCL();
            //▲===== #013
            homevm.IsEnableLeftMenu = true;
            homevm.IsExpandLeftMenu = true;
        }
        protected override void OnDeactivate(bool close)
        {
            System.Diagnostics.Debug.WriteLine(" ====>  ConsultationSummaryViewModel - OnDeactivate - BEGIN");
            DeActivateSubVM(close);
            CleanUpDataStorage();
            //this.DeactivateItem(UCePrescriptions, close);
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼====== #010
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsEnableLeftMenu = false;
            homevm.IsExpandLeftMenu = false;
            homevm.IsExpandOST = false;
            //▲====== #010
            System.Diagnostics.Debug.WriteLine(" ====>  ConsultationSummaryViewModel - OnDeactivate - END");
        }
        public void btnSave()
        {
            if (UCPatientPCLRequest.btSaveIsEnabled || UCPatientPCLRequestImage.btSaveIsEnabled)
            {
                MessageBox.Show(eHCMSResources.Z2297_G1_VuiLongHoanTatCDCLS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //20181219 TBL: Khong cho cap nhat khi dang ky da dc bao cao BHYT
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                return;
            }
            if (UCSummary.CheckValidDiagnosis() && UCePrescriptions.CheckValidPrescription())
            {
                Coroutine.BeginExecute(CoroutineUpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0));
            }
        }
        public void btnCancel()
        {
            if (Registration_DataStorage.CurrentPatientRegistration != null && Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                return;
            }
            if (!string.IsNullOrWhiteSpace(ReasonChangeStatus))
            {
                if (MessageBox.Show(eHCMSResources.Z2682_G1_XacNhanXoaPhieuKham, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ChangeStatus(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID, Globals.LoggedUserAccount.StaffID.Value, ReasonChangeStatus);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2680_G1_ChuaNhapLyDoHuy, eHCMSResources.G0442_G1_TBao);
            }
        }
        private void ChangeStatus(long PtRegDetailID, long StaffChangeStatus, string ReasonChangeStatus)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginChangeStatus(PtRegDetailID, StaffChangeStatus, ReasonChangeStatus, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool bOK = contract.EndChangeStatus(asyncResult);
                                if (bOK)
                                {
                                    MessageBox.Show("Hủy thành công");
                                }
                                else
                                {
                                    MessageBox.Show("Hủy thất bại");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            yield return new InputBindingCommand(() => NextTab(1))
            {
                HotKey_Registered_Name = "ghkNextTab1",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.N
            };
            yield return new InputBindingCommand(() => NextTab(-1))
            {
                HotKey_Registered_Name = "ghkPrevTab1",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.P
            };
        }
        public void tcMainCommon_Loaded(object sender)
        {
            tcMainCommon = sender as TabControl;
        }
        public void PrintHealthRecord()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
            {
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.eItem = ReportName.MedicalRecord;
                proAlloc.RegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                proAlloc.V_RegistrationType = (long)Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        #endregion
        #region Methods
        public void Authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mTimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation, (int)eConsultation.mPtDashboardSummary, (int)oConsultationEx.mThongTinChung_TimBN, (int)ePermission.mView);
            mTimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation, (int)eConsultation.mPtDashboardSummary, (int)oConsultationEx.mThongTinChung_TimDK, (int)ePermission.mView);
        }
        public IEnumerator<IResult> WarningReturnDrugNotEnough()
        {
            //var dialog = new MessageWarningShowDialogTask(eHCMSResources.Z1066_G1_ToaDaBanDongYCNhatKg, "Đồng ý");
            //yield return dialog;

            var dialog = new WarningWithConfirmMsgBoxTask(eHCMSResources.Z1066_G1_ToaDaBanDongYCNhatKg, eHCMSResources.K3847_G1_DongY, true);
            yield return dialog;

            if (dialog.IsAccept)
            {
                UCePrescriptions.AllowUpdateThoughReturnDrugNotEnough = true;
                //UCePrescriptions.IsPrescriptionChanged = true;
                UpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0);
                UCePrescriptions.ResetPrescriptionInfoChanged();
            }
            yield break;
        }
        private IEnumerator<IResult> CoroutineUpdateDiagnosisTreatmentAndPrescription(bool IsUpdate = false, bool IsPrescriptionUpdate = false)
        {
            UpdateDiagnosisTreatmentAndPrescription(IsUpdate, IsPrescriptionUpdate);
            yield break;
        }

        const Int16 cNewDiagnosisTreatment = 0b1000;
        const Int16 cUpdateDiagnosisTreatment = 0b0100;
        const Int16 cNewPrescription = 0b0010;
        const Int16 cUpdatePrescription = 0b0001;

        private bool PrepareDataAndCheckIfRequireToSaveDiagAndPrescript(out DiagnosisTreatment objDiag, out Prescription objPre, bool EditPrescription, out Int16 SaveDiagPrescriptFlag)
        {
            objDiag = null;
            objPre = null;
            SaveDiagPrescriptFlag = 0;
            UCSummary.DiagTrmtItem.DoctorStaffID = Globals.LoggedUserAccount.StaffID.Value;
            UCSummary.DiagTrmtItem.DeptLocationID = Globals.DeptLocation.DeptLocationID;

            if (UCSummary.IsDiagTrmentChanged)
            {
                objDiag = UCSummary.DiagTrmtItem;
                if (!CheckValidSmallProcedure(UCSummary.UpdatedSmallProcedure))
                {
                    return false;
                }
                if (objDiag.DTItemID > 0)
                {
                    SaveDiagPrescriptFlag += cUpdateDiagnosisTreatment;
                }
                else
                {
                    SaveDiagPrescriptFlag += cNewDiagnosisTreatment;
                }

                UCSummary.IsDiagTrmentChanged = false;
            }
            if (UCePrescriptions.IsPrescriptionChanged && (UCePrescriptions.btnSaveAddNewIsEnabled || UCePrescriptions.btnUpdateIsEnabled))
            {
                if (EditPrescription)
                {
                    UCePrescriptions.ChangeStatesBeforeUpdate();
                }
                objPre = UCePrescriptions.ObjTaoThanhToaMoi;
                /*▼====: #007*/
                if (objPre.PrescriptID > 0)
                {
                    SaveDiagPrescriptFlag += cUpdatePrescription;
                }
                else
                {
                    SaveDiagPrescriptFlag += cNewPrescription;
                }
                /*▲====: #007*/
            }
            if (objDiag == null && objPre == null && UCSummary.UpdatedSmallProcedure == null)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return false;
            }
            //if (UCSummary.DiagTrmtItem == null && UCSummary.UpdatedSmallProcedure != null)
            //20190306 TBL: Rang buoc thu thuat phai nhap chan doan
            if (UCSummary.UpdatedSmallProcedure != null && objDiag == null && Registration_DataStorage.PatientServiceRecordCollection != null && Registration_DataStorage.PatientServiceRecordCollection.Count == 0)
            {
                MessageBox.Show(eHCMSResources.A0405_G1_Msg_InfoChuaCoCD);
                return false;
            }

            return true;
        }

        private void ProcessReturnResultAfterSaving(int SaveRecResult, Int16 SaveDiagPrescriptFlag, long OutServiceRecID, long DTItemID, bool IsUpdate, Prescription objPre, string OutError, bool IsMessageBox, int VersionNumberOut)
        {
            if (SaveRecResult == SaveDiagPrescriptFlag)    // Saving all OK                        
            {
                bool bAllowModifyPrescription = false;

                if (OutServiceRecID > 0)
                {
                    UCSummary.DiagTrmtItem.ServiceRecID = OutServiceRecID;
                }
                if (DTItemID > 0)
                {
                    UCSummary.DiagTrmtItem.DTItemID = DTItemID;
                }
                if (VersionNumberOut > 0)
                {
                    UCSummary.DiagTrmtItem.VersionNumber = VersionNumberOut;
                }
                UCSummary.ChangeStatesAfterUpdated(IsUpdate);
                if (UCSummary.refIDC10List != null)
                {
                    UCSummary.refIDC10List = UCSummary.refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                }

                if (objPre != null && (UCePrescriptions.btnSaveAddNewIsEnabled || UCePrescriptions.btnUpdateIsEnabled))
                {
                    // Comment ra de tham khao sau do delete
                    //UCePrescriptions.allPrescriptionIssueHistory = OutPrescriptionIssueHistory.ToObservableCollection();
                    //UCePrescriptions.ObjTaoThanhToaMoi.PrescriptID = OutPrescriptionID;
                    //UCePrescriptions.ObjTaoThanhToaMoi.IssueID = OutIssueID;

                    if (!UCePrescriptions.btnUpdateIsEnabled)
                    {
                        if (Registration_DataStorage.CurrentPatient != null)
                        {
                            Globals.EventAggregator.Publish(new ClearPrescriptionListAfterAddNewEvent());
                            Globals.EventAggregator.Publish(new ClearDrugUsedAfterAddNewEvent());
                        }
                    }
                    else
                    {
                        UCePrescriptions.AllowUpdateThoughReturnDrugNotEnough = false;
                        if (Registration_DataStorage.CurrentPatient != null)
                        {
                            Globals.EventAggregator.Publish(new ClearPrescriptionListAfterUpdateEvent());
                            Globals.EventAggregator.Publish(new ClearDrugUsedAfterUpdateEvent());
                        }
                    }

                    UCePrescriptions.ResetPrescriptionInfoChanged();

                    //UCePrescriptions.AllowModifyPrescription();

                    bAllowModifyPrescription = true;

                }
                bool IsReloadPrescription = true;
                if (UCePrescriptions.ObjTaoThanhToaMoi == null || (UCePrescriptions.ObjTaoThanhToaMoi != null && UCePrescriptions.ObjTaoThanhToaMoi.PrescriptID == 0 && !UCePrescriptions.ObjTaoThanhToaMoi.PrescriptionDetails.Any(x => x.DrugID > 0)))
                {
                    IsReloadPrescription = false;
                }
                if (((SaveRecResult & cNewDiagnosisTreatment) > 0) || ((SaveRecResult & cNewPrescription) > 0) || ((SaveRecResult & cUpdatePrescription) > 0))
                {
                    PatientServiceRecordsGetForKhamBenh_Ext(bAllowModifyPrescription, IsReloadPrescription);
                }

                if (!IsUpdate)
                {
                    var HomeView = Globals.GetViewModel<IHome>();
                    HomeView.IsExpandOST = true;
                    if (HomeView.OutstandingTaskContent != null && HomeView.OutstandingTaskContent is IConsultationOutstandingTask)
                    {
                        ((IConsultationOutstandingTask)HomeView.OutstandingTaskContent).SearchRegistrationListForOST();
                    }
                }

                if (!IsUpdate)
                    Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterAddNewEvent());
                else
                    Globals.EventAggregator.Publish(new ClearAllDiagnosisListAfterUpdateEvent());

                if (Globals.ConsultationIsChildWindow)
                {
                    Globals.EventAggregator.Publish(new DiagnosisTreatmentSelectedEvent<DiagnosisTreatment> { DiagnosisTreatment = UCSummary.DiagTrmtItem.DeepCopy() });
                }

                if (IsMessageBox)
                {
                    if (!IsUpdate)
                    {
                        GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                    }
                    else
                    {
                        GlobalsNAV.ShowMessagePopup(eHCMSResources.K2782_G1_DaCNhat);
                    }
                }
            }
            else
            {
                if (SaveRecResult > 0)  // Luu ca 2 chan doan va toa thuoc nhung chi chan doan luu thanh cong
                {
                    if (OutServiceRecID > 0)
                    {
                        UCSummary.DiagTrmtItem.ServiceRecID = OutServiceRecID;
                    }
                    if (DTItemID > 0)
                    {
                        UCSummary.DiagTrmtItem.DTItemID = DTItemID;
                    }
                    if (VersionNumberOut > 0)
                    {
                        UCSummary.DiagTrmtItem.VersionNumber = VersionNumberOut;
                    }
                    UCSummary.ChangeStatesAfterUpdated(IsUpdate);
                    if (UCSummary.refIDC10List != null)
                    {
                        UCSummary.refIDC10List = UCSummary.refIDC10List.Where(x => x.DiseasesReference != null).ToObservableCollection();
                    }
                    PatientServiceRecordsGetForKhamBenh_Ext(false, true);
                }
                else
                {
                    if (((SaveDiagPrescriptFlag & cNewDiagnosisTreatment) > 0) || ((SaveDiagPrescriptFlag & cUpdateDiagnosisTreatment) > 0))
                    {
                        if (!IsUpdate)
                        {
                            MessageBox.Show(eHCMSResources.Z0411_G1_LuuCDoanKgThCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0269_G1_Msg_InfoCNhatCDFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                    }
                }

                if (((SaveDiagPrescriptFlag & cNewPrescription) > 0) || ((SaveDiagPrescriptFlag & cUpdatePrescription) > 0))
                {
                    if (!string.IsNullOrEmpty(OutError))
                    {
                        string DrugList = OutError.IndexOf("@") > 0 ? OutError.Substring(OutError.IndexOf("@"), OutError.Length - OutError.IndexOf("@")) : "None";
                        OutError = OutError.Replace(DrugList, "");
                        switch (OutError)
                        {
                            case "Error":
                                {
                                    UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    break;
                                }
                            case "PhaiThucHien-TraPhieuTruoc":
                                {
                                    UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    IsMessageBox = false;
                                    Coroutine.BeginExecute(WarningReturnDrugNotEnough());
                                    break;
                                }
                            case "ToaNay-DaTungPhatHanh-VaSuDung":
                                {
                                    UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0688_G1_ToaDaPhHanhRoi) + Environment.NewLine + string.Format("{0}!", eHCMSResources.Z0870_G1_KgTheCNhatToa), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    break;
                                }
                            case "Duplex-Prescriptions_PrescriptionsInDay":
                                {
                                    UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
                                    + Environment.NewLine + DrugList.Replace("@", "")
                                    + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    break;
                                }
                            case "Error-Exception":
                                {
                                    UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0272_G1_Msg_InfoCNhatFail), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    break;
                                }
                            default:
                                {
                                    MessageBox.Show(string.Format("{0}", OutError), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    break;
                                }
                        }
                    }
                }
            }
            if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
            {
                GetRefTreatmentRegimensAndDetail(Registration_DataStorage.CurrentPatientRegistrationDetail);
            }
            //TBL: Toa thuoc bi loi

            //TBL: Chi luu duoc chan doan, con toa thuoc thi bi loi
            //else
            //{


            //    if (OutError.Contains("Duplex-Prescriptions_PrescriptionsInDay"))
            //    {
            //        MessageBox.Show(eHCMSResources.K0152_G1_ToaCoThuocBiTrungTrongNg
            //        + Environment.NewLine + OutError.Replace("Duplex-Prescriptions_PrescriptionsInDay@", "")
            //        + Environment.NewLine + eHCMSResources.K0151_G1_KTraThuocTrongToa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    }
            //    else
            //    {
            //        MessageBox.Show(OutError + Environment.NewLine + eHCMSResources.I0944_G1_I);
            //    }
            //}


        }
        private bool CheckValidSmallProcedure(SmallProcedure aSmallProcedure)
        {
            if (aSmallProcedure == null)
            {
                return true;
            }
            if (aSmallProcedure.ProcedureDateTime == DateTime.MinValue)
            {
                MessageBox.Show(eHCMSResources.Z2408_G1_VuiLongNhapTGPTTT);
                return false;
            }
            if (aSmallProcedure.CompletedDateTime == DateTime.MinValue)
            {
                MessageBox.Show(eHCMSResources.Z2408_G1_VuiLongNhapTGPTTT);
                return false;
            }
            if (string.IsNullOrEmpty(aSmallProcedure.Diagnosis))
            {
                MessageBox.Show(eHCMSResources.Z2408_G1_VLNhapCDSauPT);
                return false;
            }
            if (string.IsNullOrEmpty(aSmallProcedure.AfterICD10.DiseaseNameVN) || string.IsNullOrEmpty(aSmallProcedure.AfterICD10.ICD10Code))
            {
                MessageBox.Show(eHCMSResources.Z2408_G1_VLNhapCDSauPT);
                return false;
            }
            if (string.IsNullOrEmpty(aSmallProcedure.ProcedureMethod))
            {
                MessageBox.Show(eHCMSResources.Z2408_G1_VLNhapPhuongPhapTTPT);
                return false;
            }
            return true;
        }
        private void UpdateDiagnosisTreatmentAndPrescription(bool IsUpdate = false, bool IsPrescriptionUpdate = false)
        {
            Prescription objPre = null;
            DiagnosisTreatment objDiag = null;
            bool IsMessageBox = true;
            long OutServiceRecID = 0;
            long OutPrescriptionID = 0;
            long OutIssueID = 0;
            string OutError = "";
            IList<PrescriptionIssueHistory> OutPrescriptionIssueHistory = new List<PrescriptionIssueHistory>();
            long DTItemID = 0;
            int VersionNumberOut = 0;

            Int16 SaveDiagPrescriptFlag = 0;

            bool EditPrescription = UCePrescriptions.btnSaveAddNewIsEnabled || UCePrescriptions.btnUpdateIsEnabled;

            if (PrepareDataAndCheckIfRequireToSaveDiagAndPrescript(out objDiag, out objPre, EditPrescription, out SaveDiagPrescriptFlag) == false)
            {
                return;
            }
            var mSmallProcedure = UCSummary.UpdatedSmallProcedure;
            if (!CheckValidSmallProcedure(mSmallProcedure))
            {
                return;
            }
            if (mSmallProcedure != null && UCSummary != null)
            {
                mSmallProcedure.ProcedureDescription = UCSummary.ProcedureDescription;
                mSmallProcedure.ProcedureDescriptionContent = UCSummary.ProcedureDescriptionContent;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    /*▲====: #001*/
                    var mPrecriptionsBeforeUpdate = UCePrescriptions.PrecriptionsBeforeUpdate;
                    if (mPrecriptionsBeforeUpdate != null && objPre != null && objDiag != null)
                    {
                        objPre.Diagnosis = objDiag.DiagnosisFinal;
                    }
                    contract.BeginAddDiagnosisTreatmentAndPrescription(IsUpdateWithoutChangeDoctorIDAndDatetime, objDiag, UCSummary.Compare2Object(), UCSummary.refIDC10List
                            , 0, new List<DiagnosisICD9Items>()
                            , (short)Globals.ServerConfigSection.CommonItems.NumberTypePrescriptions_Rule, objPre
                            , mPrecriptionsBeforeUpdate, UCePrescriptions.AllowUpdateThoughReturnDrugNotEnough
                            , mSmallProcedure, Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : 0
                            , new List<Resources>()
                            , Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long SmallProcedureID;
                            int SaveRecResult = contract.EndAddDiagnosisTreatmentAndPrescription(out OutServiceRecID, out OutPrescriptionID, out OutIssueID, out OutError, out OutPrescriptionIssueHistory, out DTItemID, out SmallProcedureID, out VersionNumberOut, asyncResult);
                            if (mSmallProcedure != null)
                            {
                                mSmallProcedure.SmallProcedureID = SmallProcedureID;
                                UCSummary.ApplySmallProcedure(mSmallProcedure);
                            }
                            if (SmallProcedureID > 0)
                            {
                                UCSummary.IsVisibilitySkip = false;
                            }
                            ProcessReturnResultAfterSaving(SaveRecResult, SaveDiagPrescriptFlag, OutServiceRecID, DTItemID, IsUpdate, objPre, OutError, IsMessageBox, VersionNumberOut);
                            /*▲====: #007*/
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            if (EditPrescription)
                                UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }
        public void NextTab(int aIndex = 1)
        {
            if (aIndex > 0 && tcMainCommon.SelectedIndex + 1 < tcMainCommon.Items.Count)
            {
                tcMainCommon.SelectedItem = tcMainCommon.Items[tcMainCommon.SelectedIndex + 1];
            }
            else if (aIndex < 0 && tcMainCommon.SelectedIndex > 0)
            {
                tcMainCommon.SelectedItem = tcMainCommon.Items[tcMainCommon.SelectedIndex - 1];
            }
            tcMainCommon.Focus();
        }
        //==== #009 ====
        private bool _bEnableSave = true;
        public bool bEnableSave
        {
            get
            {
                return _bEnableSave;
            }
            set
            {
                if (_bEnableSave != value)
                {
                    _bEnableSave = value;
                    NotifyOfPropertyChange(() => bEnableSave);
                }
            }
        }
        //==== #009 ====
        long MedSerID;
        /*▼====: #005*/
        private int _currentTabIndex = 0;
        public void tcMainCommon_Changed(object source, object eventArgs)
        {
            var tabCtrl = source as TabControl;
            int destTabIndex = tabCtrl.SelectedIndex;
            if (destTabIndex != _currentTabIndex)
            {
                //TBL: Khi qua tab Ra toa thi tu dong lay phac do
                if (destTabIndex == 1)
                {
                    UCePrescriptions.GetPhacDo();
                    //TTM 24102018:     Khi bệnh nhân vừa đc thêm tình trạng thể chất => toa thuốc đã đc load trước khi thêm => không có tình trạng thể chất bên toa thuốc
                    //                  => khi vừa thêm/ cập nhật tình trạng thể chất bên chẩn đoán => bên ra toa cũng đc thêm/ cập nhật lại.
                    if (Globals.curPhysicalExamination != null)
                    {
                        UCePrescriptions.curPhysicalExamination = Globals.curPhysicalExamination;
                    }
                    _currentTabIndex = destTabIndex;
                }
                //==== #009 ==== //TBL: Co 4 tab can an nut luu
                if (destTabIndex == 2 || destTabIndex == 3 || destTabIndex == 6 || destTabIndex == 8)
                {
                    bEnableSave = false;
                }
                if (destTabIndex != 2 && destTabIndex != 3 && destTabIndex != 6 && destTabIndex != 8)
                {
                    bEnableSave = true;
                }
                //==== #009 ====
                //20181124 TBL: Khi chon tab Thong tin tong quat se di load Vaccine theo MedServiceID
                if (destTabIndex == 6)
                {
                    UCCommonRecs.GetRefImmunization(MedSerID);
                    _currentTabIndex = destTabIndex;
                }
                else
                {
                    _currentTabIndex = destTabIndex;
                }
            }
        }
        /*▲====: #005*/

        public void btnPrint()
        {
            if (UCSummary.UpdatedSmallProcedure != null)
            {
                MessageBox.Show(eHCMSResources.A1037_G1_Msg_InfoTTinBiDoi);
                return;
            }
            if (UCSummary.SmallProcedureObj == null)
            {
                MessageBox.Show(eHCMSResources.Z2410_G1_KhongCoTTThuThuat);
                return;
            }
            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView proAlloc)
            {
                proAlloc.RegistrationDetailID = Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID;
                //20190723 TBL: BM 0012972. Xem in thu thuat khong thay noi dung
                proAlloc.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                //20190723
                proAlloc.eItem = ReportName.The_Thu_Thuat;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }
        #endregion

        #region CENTRAL STORAGE for ALL DATA belonging to ALL TABs
        public ICS_DataStorage CS_DS = new CS_DataStorage();
        #endregion
        private void GetSmallProcedure(long PtRegDetailID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSmallProcedure(PtRegDetailID, null, (long)AllLookupValues.RegistrationType.NGOAI_TRU
                        , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mSmallProcedure = contract.EndGetSmallProcedure(asyncResult);
                                UCSummary.IsVisibility = false;
                                UCSummary.IsVisibilitySkip = false;
                                UCSummary.FormEditorIsEnabled = true;
                                if (mSmallProcedure == null || mSmallProcedure.SmallProcedureID == 0)
                                {
                                    mSmallProcedure = new SmallProcedure();
                                    //▼====== #012 Neu dang ky thu thuat nay chua co ket qua thi moi lay ket qua cu len
                                    if (Registration_DataStorage.CurrentPatient != null)
                                    {
                                        GetLatesSmallProcedure(Registration_DataStorage.CurrentPatient.PatientID, MedSerID, PtRegDetailID);
                                    }
                                    //▲====== #012
                                    if (mSmallProcedure.PtRegDetailID == 0)
                                    {
                                        mSmallProcedure.PtRegDetailID = PtRegDetailID;
                                    }
                                    UCSummary.ApplySmallProcedure(mSmallProcedure);
                                }
                                else if (mSmallProcedure.SmallProcedureID > 0 && mSmallProcedure.Diagnosis == "Temp Diagnosis")
                                {
                                    if (mSmallProcedure.PtRegDetailID == 0)
                                    {
                                        mSmallProcedure.PtRegDetailID = PtRegDetailID;
                                    }
                                    mSmallProcedure.ProcedureMethod = "";
                                    mSmallProcedure.Diagnosis = "";
                                    mSmallProcedure.AfterICD10 = null;
                                    mSmallProcedure.ProcedureDateTime = Globals.GetCurServerDateTime();
                                    mSmallProcedure.CompletedDateTime = Globals.GetCurServerDateTime();
                                    UCSummary.ApplySmallProcedure(mSmallProcedure);
                                    //▼===== #016
                                    //TBL: Nếu thủ thuật được chỉ định từ dịch vụ khám bệnh thì mới đem chẩn đoán của dịch vụ khám bệnh cho thủ thuật
                                    if (Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID > 0)
                                    {
                                        UCSummary.ICD10Changed(refICD10List);
                                    }
                                    //▲===== #016
                                }
                                else
                                {
                                    if (mSmallProcedure.PtRegDetailID == 0)
                                    {
                                        mSmallProcedure.PtRegDetailID = PtRegDetailID;
                                    }
                                    UCSummary.ApplySmallProcedure(mSmallProcedure);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
        //▼====== #012
        public void GetLatesSmallProcedure(long PatientID, long MedServiceID, long PtRegDetailID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetLatesSmallProcedure(PatientID, MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var mSmallProcedure = contract.EndGetLatesSmallProcedure(asyncResult);
                            UCSummary.IsVisibility = false;
                            UCSummary.IsVisibilitySkip = false;
                            if (mSmallProcedure == null)
                            {
                                mSmallProcedure = new SmallProcedure();
                            }
                            else if (mSmallProcedure.SmallProcedureID > 0) //20191116 TBL: Nếu có thủ thuật cũ thì mới set IsVisibility = true
                            {
                                UCSummary.IsVisibility = true;
                                UCSummary.FormEditorIsEnabled = false;
                            }
                            mSmallProcedure.PtRegDetailID = PtRegDetailID;
                            UCSummary.ApplySmallProcedure(mSmallProcedure);
                            //▼===== #016
                            //TBL: Nếu thủ thuật được chỉ định từ dịch vụ khám bệnh thì mới đem chẩn đoán của dịch vụ khám bệnh cho thủ thuật
                            if (Registration_DataStorage.CurrentPatientRegistrationDetail.ServiceRecID > 0)
                            {
                                UCSummary.ICD10Changed(refICD10List);
                            }
                            //▲===== #016
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
        //▲====== #012
        private void LoadCrossRegionHospitals()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginLoadCrossRegionHospitals(
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    Globals.CrossRegionHospital = contract.EndLoadCrossRegionHospitals(asyncResult).ToObservableCollection();
                                }
                                catch (Exception innerEx)
                                {
                                    ClientLoggerHelper.LogInfo(innerEx.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▼====== #006
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
        private bool _IsVisibleUpdate = true;
        public bool IsVisibleUpdate
        {
            get
            {
                return _IsVisibleUpdate;
            }
            set
            {
                if (_IsVisibleUpdate != value)
                {
                    _IsVisibleUpdate = value;
                    NotifyOfPropertyChange(() => IsVisibleUpdate);
                }
            }
        }
        public void CheckVisibleForTabControl()
        {
            if (IsUpdateWithoutChangeDoctorIDAndDatetime)
            {
                IsVisibleUpdate = false;
                UCePrescriptions.IsUpdateWithoutChangeDoctorIDAndDatetime = IsUpdateWithoutChangeDoctorIDAndDatetime;
            }
        }
        //▲====== #011
        private void GetRefTreatmentRegimensAndDetail(PatientRegistrationDetail aRegistrationDetail)
        {
            if (CS_DS.TreatmentRegimenCollection == null)
            {
                CS_DS.TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
            }
            else
            {
                CS_DS.TreatmentRegimenCollection.Clear();
            }
            this.ShowBusyIndicator();
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
                                 var mTreatmentRegimenCollection = contract.EndGetRefTreatmentRegimensAndDetail(asyncResult);
                                 if (mTreatmentRegimenCollection != null)
                                 {
                                     foreach (var aItem in mTreatmentRegimenCollection)
                                     {
                                         CS_DS.TreatmentRegimenCollection.Add(aItem);
                                     }
                                 }
                             }
                             catch (Exception innerEx)
                             {
                                 ClientLoggerHelper.LogInfo(innerEx.ToString());
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▼===== #013
        public void setDataPatientForRequestPCL()
        {
            if (Registration_DataStorage.CurrentPatient != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistrationDetail != null)
            {
                UCPatientPCLRequest.InitPatientInfo();
                UCPatientPCLRequestImage.InitPatientInfo();
            }
        }
        //▲===== #013
        public void btnPrintProcedure()
        {
            if (UCSummary == null || UCSummary.SmallProcedureObj == null || UCSummary.SmallProcedureObj.SmallProcedureID == 0)
            {
                return;
            }
            CommonGlobals.PrintProcedureProcess(this, UCSummary.SmallProcedureObj, Registration_DataStorage.CurrentPatientRegistration);
        }
        public void btnEditProcedureDesc()
        {
            if (UCSummary == null || UCSummary.SmallProcedureObj == null || UCSummary.SmallProcedureObj.SmallProcedureID == 0)
            {
                return;
            }
            CommonGlobals.PrintProcedureProcess_V2(this, UCSummary.SmallProcedureObj, Registration_DataStorage.CurrentPatientRegistration);
        }
        public void GetAllRegistrationDetails_ForGoToKhamBenh_Ext(PatientRegistrationDetail PtRegDetail)
        {
            Registration_DataStorage = new Registration_DataStorage();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRegistrationDetails_ForGoToKhamBenh(PtRegDetail.PatientRegistration.PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllRegistrationDetails_ForGoToKhamBenh(asyncResult);
                                if (items != null)
                                {
                                    Registration_DataStorage.PrescriptionIssueHistories = new ObservableCollection<PrescriptionIssueHistory>();
                                    foreach (var item in items)
                                    {
                                        if (item.prescriptionIssueHistory != null)
                                        {
                                            Registration_DataStorage.PrescriptionIssueHistories.Add(item.prescriptionIssueHistory);
                                        }
                                    }
                                }
                                CheckForDiKhamBenh(PtRegDetail.PatientRegistration, PtRegDetail);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.Message);
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
                    ClientLoggerHelper.LogError(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void CheckForDiKhamBenh(PatientRegistration ObjPR, PatientRegistrationDetail p)
        {
            if (ObjPR.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                //▼====: #015
                if (p.RefMedicalServiceItem != null && p.RefMedicalServiceItem.IsAllowToPayAfter == 0 && p.PaidTime == null 
                    && ObjPR.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.PayAfter 
                    && ObjPR.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.CompanyHealthRecord
                    && !Globals.ServerConfigSection.CommonItems.AllowFirstHIExaminationWithoutPay)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1470_G1_DV0ChuaTraTienKgTheKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                //▲====: #015
            }
            if (p.V_ExamRegStatus == (long)V_ExamRegStatus.mDangKyKham || p.V_ExamRegStatus == (long)V_ExamRegStatus.mChoKham || p.V_ExamRegStatus == (long)V_ExamRegStatus.mHoanTat || p.V_ExamRegStatus == (long)V_ExamRegStatus.mBatDauThucHien)
            {
                Coroutine.BeginExecute(SetPatientInfoForConsultation(ObjPR, p));
            }
            else
            {
                switch (p.V_ExamRegStatus)
                {
                    case (Int64)AllLookupValues.ExamRegStatus.KHONG_XAC_DINH:
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1394_G1_KgTheTienHanhKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                    case (Int64)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI:
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1395_G1_DVDaNgungVaTraLaiTien, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            break;
                        }
                }
            }
            if (Globals.IsLockRegistration(ObjPR.RegLockFlag, "Khám bệnh và chỉ định cận lâm sàng"))
            {
                return;
            }
        }
        private IEnumerator<IResult> SetPatientInfoForConsultation(PatientRegistration PtRegistration, PatientRegistrationDetail PtRegDetail)
        {
            if (PtRegistration == null || PtRegDetail == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0400_G1_KgNhanDuocDLieuLamViec), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                yield break;
            }
            Globals.isConsultationStateEdit = true;
            Globals.EventAggregator.Publish(new isConsultationStateEditEvent { isConsultationStateEdit = true });
            yield return GenericCoRoutineTask.StartTask(InitPhyExamAction, PtRegistration);
            yield return GenericCoRoutineTask.StartTask(GetPtServiceRecordForKhamBenhAction, PtRegistration, PtRegDetail);
            //Lấy thông tin đăng ký đầy đủ để lưu lại trong module Khám Bệnh
            yield return GenericCoRoutineTask.StartTask(GetRegistrationAction, PtRegistration, PtRegDetail);
            PublishEventGlobalsPSRLoad(false);
        }
        private void InitPhyExamAction(GenericCoRoutineTask genTask, object ObjPtRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new SummaryServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPhyExam_ByPtRegID(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (long)((PatientRegistration)ObjPtRegistration).V_RegistrationType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Globals.curPhysicalExamination = contract.EndGetPhyExam_ByPtRegID(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetPtServiceRecordForKhamBenhAction(GenericCoRoutineTask genTask, object ObjPtRegistration, object ObjPtRegDetail)
        {
            this.ShowBusyIndicator();

            PatientServiceRecord psrSearch = new PatientServiceRecord();
            psrSearch.PtRegistrationID = ((PatientRegistration)ObjPtRegistration).PtRegistrationID;
            psrSearch.PtRegDetailID = ((PatientRegistrationDetail)ObjPtRegDetail).PtRegDetailID;
            psrSearch.V_RegistrationType = ((PatientRegistration)ObjPtRegistration).V_RegistrationType;

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientServiceRecordsGetForKhamBenh(psrSearch, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void GetRegistrationAction(GenericCoRoutineTask genTask, object ObjPtRegistration, object ObjPtRegDetail)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        bool bContinue = true;
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistration(((PatientRegistration)ObjPtRegistration).PtRegistrationID, (int)Globals.PatientFindBy_ForConsultation.Value,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                PatientRegistration regInfo = contract.EndGetRegistration(asyncResult);
                                regInfo.Patient.CurrentHealthInsurance = regInfo.HealthInsurance;
                                Registration_DataStorage.CurrentPatientRegistration = regInfo;
                                Registration_DataStorage.CurrentPatientRegistrationDetail = (PatientRegistrationDetail)ObjPtRegDetail;
                                PublishEventShowPatientInfo(regInfo.Patient, regInfo, (PatientRegistrationDetail)ObjPtRegDetail);
                                //if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null)
                                //{
                                //    UCPatientMedicalServiceRequest.CallDoOpenRegistration(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID);
                                //}
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                ClientLoggerHelper.LogError(ex.Message);
                                bContinue = false;
                            }
                            finally
                            {
                                genTask.ActionComplete(bContinue);
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    ClientLoggerHelper.LogError(ex.Message);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void PublishEventGlobalsPSRLoad(bool IsReloadPrescript, bool bAllowModifyPrescription = false)
        {
            switch (Globals.LeftModuleActive)
            {
                case LeftModuleActive.KHAMBENH_CHANDOAN:
                    Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult());
                    break;
                case LeftModuleActive.KHAMBENH_RATOA:
                    if (IsReloadPrescript)
                    {
                        Globals.EventAggregator.Publish(new LoadPrescriptionAfterSaved());
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_EPrescript());
                    }
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN_RATOA:
                    Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult());
                    if (IsReloadPrescript && bAllowModifyPrescription)
                    {
                        Globals.EventAggregator.Publish(new LoadPrescriptionAfterSaved());
                    }
                    else
                    {
                        Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_EPrescript { bJustCallAllowModifyPrescription = true });
                    }
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN_NOITRU:
                    Globals.EventAggregator.Publish(new GlobalCurPatientServiceRecordLoadComplete_Consult_InPt());
                    break;
                case LeftModuleActive.KHAMBENH_RATOA_XUATVIEN:
                    Globals.EventAggregator.Publish(new LoadPrescriptionInPtAfterSaved());
                    break;
            }
        }
        public void PublishEventShowPatientInfo(Patient patient, PatientRegistration regInfo, PatientRegistrationDetail Item)
        {
            if (Globals.LeftModuleActive == LeftModuleActive.NONE)
            {
                return;
            }
            //publish cai nay cho patient info
            //Globals.EventAggregator.Publish(new ShowPatientInfo<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
            //kiem tra thang nao dang active roi publish cho no

            //KMx: Show Patient Info (25/05/2014 12:07).
            Globals.EventAggregator.Publish(new ShowPatientInfoForConsultation() { Patient = patient, PtRegistration = regInfo });
            switch (Globals.LeftModuleActive)
            {
                case LeftModuleActive.KHAMBENH_THONGTINCHUNG:
                case LeftModuleActive.KHAMBENH_CHANDOAN_RATOA:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_TONGQUAT:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_CHANDOAN:
                    Globals.EventAggregator.Publish(new ShowPtRegDetailForDiagnosis() { PtRegistration = regInfo, PtRegDetail = Item });
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CHANDOAN<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_RATOA:
                    Globals.EventAggregator.Publish(new ShowPtRegDetailForPrescription() { PtRegistration = regInfo, PtRegDetail = Item });
                    Globals.EventAggregator.Publish(new ClearPrescriptionListAfterSelectPatientEvent());
                    Globals.EventAggregator.Publish(new ClearDrugUsedAfterSelectPatientEvent());
                    Globals.EventAggregator.Publish(new ClearPrescriptTemplateAfterSelectPatientEvent());
                    break;
                case LeftModuleActive.KHAMBENH_LSBENHAN:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_LSBENHAN<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;

                case LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUXETNGHIEM:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM() { Patient = patient });
                    Globals.EventAggregator.Publish(new ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_XETNGHIEM() { Patient = patient, PtRegistration = regInfo });
                    break;
                case LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH() { Patient = patient });
                    Globals.EventAggregator.Publish(new ShowListPCLRequest_KHAMBENH_CLS_PHIEUYEUCAU_HINHANH() { Patient = patient, PtRegistration = regInfo });
                    break;
                case LeftModuleActive.KHAMBENH_HENCLS_HENCLS:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_HENCLS_HENCLS<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_CLS_KETQUAHINHANH:
                    Globals.EventAggregator.Publish(new InitDataForPtPCLImagingResult());
                    break;
                case LeftModuleActive.KHAMBENH_CLS_KETQUAXETNGHIEM:
                    Globals.EventAggregator.Publish(new InitDataForPtPCLLaboratoryResult());
                    break;
                case LeftModuleActive.KHAMBENH_CLS_NGOAIVIEN_HINHANH:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_HINHANH<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
                case LeftModuleActive.KHAMBENH_CLS_NGOAIVIEN_XETNGHIEM:
                    Globals.EventAggregator.Publish(new ShowPatientInfo_KHAMBENH_CLS_NGOAIVIEN_XETNGHIEM<Patient, PatientRegistration, PatientRegistrationDetail>() { Pt = patient, PtReg = regInfo, PtRegDetail = Item });
                    break;
            }
        }
        public void PatientServiceRecordsGetForKhamBenh_Ext(bool bAllowModifyPrescription = false
            , bool IsReloadPrescription = true)
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PatientServiceRecordsGetForKhamBenh(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID, Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType, bAllowModifyPrescription
                , IsReloadPrescription);
        }
        public void PatientServiceRecordsGetForKhamBenh(long PtRegistrationID, long PtRegDetailID, AllLookupValues.RegistrationType V_RegistrationType, bool bAllowModifyPrescription
            , bool IsReloadPrescription)
        {
            this.ShowBusyIndicator();
            PatientServiceRecord psrSearch = new PatientServiceRecord();
            psrSearch.PtRegistrationID = PtRegistrationID;
            psrSearch.PtRegDetailID = PtRegDetailID;
            psrSearch.V_RegistrationType = V_RegistrationType;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientServiceRecordsGetForKhamBenh(psrSearch,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var psr = contract.EndPatientServiceRecordsGetForKhamBenh(asyncResult);
                                Registration_DataStorage.PatientServiceRecordCollection = new ObservableCollection<PatientServiceRecord>(psr);
                                PublishEventGlobalsPSRLoad(IsReloadPrescription, bAllowModifyPrescription);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void DiagnosisIcd10Items_Load(long? ServiceRecID, long? PatientID, bool Last)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePMRsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDiagnosisIcd10Items_Load(ServiceRecID, PatientID, Last, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDiagnosisIcd10Items_Load(asyncResult);
                            refICD10List = results.ToObservableCollection();
                            GetSmallProcedure(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID);
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
    }
}