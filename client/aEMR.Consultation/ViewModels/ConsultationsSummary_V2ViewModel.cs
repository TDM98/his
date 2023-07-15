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
using aEMR.Common.Converters;
using System.Text;
using OrderDTO = DataEntities.OrderDTO;
using aEMR.ViewContracts.Consultation_ePrescription;

/*
* 20180920 #001 TBL:    Chinh sua chuc nang bug mantis ID 0000061
* 20180923 #002 TTM:    Chuyển thông tin Patient xuống cho màn hình con, vì trước đây nó là màn hình độc lập => Khi nó được gọi thì nó chạy khởi tạo (code lấy dữ liệu nằm ở hàm khởi tạo)
*                       Bây giờ là màn hình con nên không thể chạy khởi tạo để lấy dữ liệu (khởi tạo cùng lúc với màn hình cha ConsultationSummary). 
*                       Nên cần 1 hàm để chuyển dữ liệu xuống để hoạt động.
* 20180923 #003 TBL:    BM 0000066: Added out long DTItemID and set OutServiceRecID, DTItemID
* 20180926 #004 TBL:    BM 0000077. 
* 20181016 #005 TBL:    BM 0002170: Lay phac do tu dong khi chuyen qua tab Ra toa
* 20181020 #006 TBL:    BM 0003196: Hien thi ten dich vu o Expander
* 20181023 #007 TBL:    BM 0003206: Khi luu can phai biet la them moi hay cap nhat
* 20181029 #008 TBL:    BM 0004208: Chinh sua toa thuoc khi vua luu thi toa thuoc khong duoc tinh BH
* 20181115 #009 TBL:    BM 0005264: Khi qua tab co nut luu thi an nut luu tong di
* 20181121 #010 TTM:    Tạo mới out standing task ở màn hình sử dụng, không còn tạo từ Module nữa
* 20190531 #011 TTM:    BM 0006684: Xây dựng màn hình cập nhật chẩn đoán ra toa - báo cáo BHYT
* 20190727 #012 TBL:    BM 0012974: Tạo mới dựa trên cũ cho thủ thuật
* 20190822 #013 TTM:    BM 0013186: Fix lỗi khoá trắng màn hình chỉ định nội ngoại trú khi chuyển từ thông tin chung sang màn hình khám bệnh.
* 20191006 #014 TTM:    BM 0017421: [Ra toa] Thêm tên thư ký y khoa thực hiện toa 
* 20191028 #015 TBL:    BM 0018501: Nếu chẩn đoán và toa thuốc không phải 1 cặp thì hiện đỏ xung quanh ngày khám của toa thuốc
* 20200212 #016 TBL:    BM 0023910: Thủ thuật nếu được chỉ định từ dịch vụ khám bệnh thì không bắt buộc phải tạo chẩn đoán
* 20200309 #017 TTM:    BM 0023978: Bổ sung điều kiện kiểm tra ngày kết thúc không được vượt quá ngày bắt đầu => Số giờ quy định đặt trong cấu hình
* 20200902 #018 TTM:	BM 0038177: Về vấn đề thuốc đã bán và tạo toa mới. 
* 20200926 #019 TNHX:	BM: Filter prescription has MaxHIPay was requestes by Dr Vu
* 20210228 #020 TNHX: 219 Add config AllowFirstHIExaminationWithoutPay
* 20210423 #021 TNHX: Lấy danh sách ICD phụ để hiển thị cho bsi chọn
* 20210611 #022 TNHX: 346 Lấy danh sách rule ICD10
* 20211225 #023 BLQ: 857 Bỏ các tab kết quả xét nghiệm; kết quả hình ảnh; thông tin tổng quát; cls hình ảnh ngoại viện.
* 20220530 #024 BLQ: Kiểm tra thời gian thao tác của bác sĩ
* 20220928 #025 BLQ: Thêm loại toa thuốc lưu xuống database
* 20221120 #026 QTD: Kiểm tra nhắc CLS trước khi lưu toa thuốc và y lệnh
* 20230307 #027 BLQ: Thêm định mức bàn khám 
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IConsultationsSummary_V2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ConsultationsSummary_V2ViewModel : ViewModelBase, IConsultationsSummary_V2
        , IHandle<ShowPatientInfo_KHAMBENH_THONGTINCHUNG<Patient, PatientRegistration, PatientRegistrationDetail>>
        , IHandle<HaveTransferForm>
        , IHandle<CheckValidProcedure>
        , IHandle<AllPCLRequestImageClose>
        , IHandle<AllPCLRequestClose>
    {
        [ImportingConstructor]
        public ConsultationsSummary_V2ViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            Authorization();
            var homevm = Globals.GetViewModel<IHome>();
            DiagnosisTreatmentHistoriesTreeContent = Globals.GetViewModel<IDiagnosisTreatmentHistoriesTree>();
            homevm.LeftMenu = DiagnosisTreatmentHistoriesTreeContent;
            ActivateItem(DiagnosisTreatmentHistoriesTreeContent);
            CreateSubVM();
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
            //▼====== #023
            //UCCommonRecs.CS_DS = null;
            //▲====== #023
            UCPatientTreeForm.CS_DS = null;
            ((ISearchPatientAndRegistration)SearchRegistrationContent).CS_DS = null;
            UCDoctorProfileInfo.CS_DS = null;
            UCPatientPCLRequest.CS_DS = null;
            UCPatientPCLRequestImage.CS_DS = null;
            //▼====== #023
            //UCPatientPCLLaboratoryResult.CS_DS = null;
            //UCPatientPCLImagingResult.CS_DS = null;
            //UCPatientPCLDeptImagingExtHome.CS_DS = null;
            //▲====== #023
            CS_DS = null;

            UCPatientProfileInfo = null;
            UCSummary = null;
            UCPtRegDetailInfo = null;
            //▼====== #023
            //UCCommonRecs = null;
            //▲====== #023
            UCPatientTreeForm = null;
            SearchRegistrationContent = null;
            UCDoctorProfileInfo = null;
            UCPatientPCLRequest = null;
            UCPatientPCLRequestImage = null;
            //▼====== #023
            //UCPatientPCLLaboratoryResult = null;
            //UCPatientPCLImagingResult = null;
            //UCPatientPCLDeptImagingExtHome = null;
            //▲====== #023
        }

        #region Properties
        //▼====== #019
        public List<FilterPrescriptionsHasHIPay> ListFilterPrescriptionsHasHIPay { get; set; }
        //▲====== #019
        //▼====== #021
        public List<RequiredSubDiseasesReferences> ListRequiredSubDiseasesReferences { get; set; }
        //▲====== #021
        //▼====== #022
        public List<RuleDiseasesReferences> ListRuleDiseasesReferences { get; set; }
        //▲====== #022
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
        public ISummary_V3 UCSummary { get; set; }
        public IePrescriptions UCePrescriptions
        {
            get
            {
                return UCSummary != null ? UCSummary.UCePrescriptions : null;
            }
            set
            {
                if (UCSummary == null)
                {
                    return;
                }
                UCSummary.UCePrescriptions = value;
            }
        }
        //▼====: #023
        //public ICommonRecs UCCommonRecs { get; set; }
        //▲====: #023
        public IPatientTreeForm UCPatientTreeForm { get; set; }
        public ILoginInfo UCDoctorProfileInfo { get; set; }
        public IPatientPCLRequest UCPatientPCLRequest { get; set; }
        public IPatientPCLRequestImage UCPatientPCLRequestImage { get; set; }
        //▼====: #023
        //public IPatientPCLLaboratoryResult UCPatientPCLLaboratoryResult { get; set; }
        //public IPatientPCLImagingResult UCPatientPCLImagingResult { get; set; }
        //public IPatientPCLDeptImagingExtHome UCPatientPCLDeptImagingExtHome { get; set; }
        //▲====: #023
        public bool IsOutPtTreatmentProgram
        {
            get
            {
                return UCSummary != null && UCSummary.IsOutPtTreatmentProgram;
            }
            set
            {
                if (UCSummary != null)
                {
                    UCSummary.IsOutPtTreatmentProgram = value;
                }
                if (DiagnosisTreatmentHistoriesTreeContent != null)
                {
                    DiagnosisTreatmentHistoriesTreeContent.IsOutPtTreatmentProgram = value;
                }
            }
        }
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
                //▼====== #019
                if (ListFilterPrescriptionsHasHIPay != null && _Registration_DataStorage.ListFilterPrescriptionsHasHIPay == null)
                {
                    _Registration_DataStorage.ListFilterPrescriptionsHasHIPay = ListFilterPrescriptionsHasHIPay;
                }
                //▲====== #019
                //▼====== #021
                if (ListRequiredSubDiseasesReferences != null && _Registration_DataStorage.ListRequiredSubDiseasesReferences == null)
                {
                    _Registration_DataStorage.ListRequiredSubDiseasesReferences = ListRequiredSubDiseasesReferences;
                }
                //▲====== #021
                //▼====== #022
                if (ListRuleDiseasesReferences != null && _Registration_DataStorage.ListRuleDiseasesReferences == null)
                {
                    _Registration_DataStorage.ListRuleDiseasesReferences = ListRuleDiseasesReferences;
                }
                //▲====== #022
                UCPatientPCLRequest.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLRequestImage.Registration_DataStorage = Registration_DataStorage;
                //▼====== #023
                //UCPatientPCLLaboratoryResult.Registration_DataStorage = Registration_DataStorage;
                //UCPatientPCLImagingResult.Registration_DataStorage = Registration_DataStorage;
                //▲====== #023
                UCSummary.Registration_DataStorage = Registration_DataStorage;
                UCePrescriptions.Registration_DataStorage = Registration_DataStorage;
                UCPatientTreeForm.Registration_DataStorage = Registration_DataStorage;
                //▼====== #023
                //UCCommonRecs.Registration_DataStorage = Registration_DataStorage;
                //UCPatientPCLDeptImagingExtHome.Registration_DataStorage = Registration_DataStorage;
                //▲====== #023
                UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
                DiagnosisTreatmentHistoriesTreeContent.Registration_DataStorage = Registration_DataStorage;
            }
        }
        private string _GeneralInfoString;
        public string GeneralInfoString
        {
            get { return _GeneralInfoString; }
            set
            {
                if (_GeneralInfoString != value)
                {
                    _GeneralInfoString = value;
                    NotifyOfPropertyChange(() => GeneralInfoString);
                }
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
        private bool _IsSearchOnlyProcedure;
        public bool IsSearchOnlyProcedure
        {
            get { return _IsSearchOnlyProcedure; }
            set
            {
                if (_IsSearchOnlyProcedure != value)
                {
                    _IsSearchOnlyProcedure = value;
                    NotifyOfPropertyChange("IsSearchOnlyProcedure");
                    ((ISearchPatientAndRegistration)SearchRegistrationContent).IsSearchOnlyProcedure = IsSearchOnlyProcedure;
                }
            }
        }
        private bool _IsHaveTransferForm = false;
        public bool IsHaveTransferForm
        {
            get
            {
                return _IsHaveTransferForm;
            }
            set
            {
                if (_IsHaveTransferForm != value)
                {
                    _IsHaveTransferForm = value;
                    NotifyOfPropertyChange(() => IsHaveTransferForm);
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
                    //20191023 TBL: Task #1115: Do khi để nghị dịch vụ thì Patient được set lại nên GeneralInfoString sẽ thay đổi làm mất tên dịch vụ và chẩn đoán sơ bộ
                    //UCPatientProfileInfo.CS_DS.CurrentPatient.GeneralInfoString += string.Format("                                              [{0}]       CĐ sơ bộ: [{1}]", message.PtRegDetail.MedServiceName, message.PtReg.BasicDiagTreatment);
                    GeneralInfoString = UCPatientProfileInfo.CS_DS.CurrentPatient.GeneralInfoString;
                    if (message.PtReg != null && message.PtReg.PtInsuranceBenefit.GetValueOrDefault() > 0)
                    {
                        var converter = new PercentageConverter();
                        string str = (string)converter.Convert(message.PtReg.PtInsuranceBenefit.Value, typeof(string), null, null);
                        GeneralInfoString += string.Format(" - {0}: ", eHCMSResources.Q0421_G1_QL) + str;
                    }
                    GeneralInfoString += string.Format("                                              [{0}]       CĐ sơ bộ: [{1}]", message.PtRegDetail.MedServiceName, message.PtReg.BasicDiagTreatment);
                    //▼===== #015
                    UCePrescriptions.PtRegistrationID = message.PtRegDetail.PtRegistrationID.GetValueOrDefault();
                    //▲===== #015
                }
                /*▲====: #006*/
                UCSummary.InitConsultationInfo(message.Pt, message.PtRegDetail);
                //UCPatientTreeForm.InitPatientInfo();
                //▼===== #023
                //UCCommonRecs.InitPatientInfo();
                //▲===== #023
                UCPatientPCLRequest.InitPatientInfo();
                UCPatientPCLRequestImage.InitPatientInfo();
                //▼===== #023
                //▼====== #002
                //UCPatientPCLLaboratoryResult.InitPatientInfo(message.Pt);
                //UCPatientPCLImagingResult.InitPatientInfo(message.Pt);
                //▲====== #002
                //▲===== #023
                //20182411 TBL: Khi tim dang ky se set MedSerID de load cac loai Vaccine theo MedSerID
                MedSerID = message.PtRegDetail.MedServiceID.GetValueOrDefault();
                //▼===== #016
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
                //▲===== #016
                GetRefTreatmentRegimensAndDetail(message.PtRegDetail);
                if (DiagnosisTreatmentHistoriesTreeContent != null && Registration_DataStorage.CurrentPatient != null)
                {
                    //20200323 TBL: Mặc định thời gian hiển thị lịch sử khám bệnh 6 tháng
                    DiagnosisTreatmentHistoriesTreeContent.ToDate = Globals.GetCurServerDateTime().AddMonths(-6);
                    DiagnosisTreatmentHistoriesTreeContent.FromDate = Globals.GetCurServerDateTime();
                    DiagnosisTreatmentHistoriesTreeContent.GetPatientServicesTreeView(Registration_DataStorage.CurrentPatient.PatientID);
                }
                //▼===== #015
                UCePrescriptions.ServiceRecIDDiagTrmt = UCSummary.DiagTrmtItem.ServiceRecID.GetValueOrDefault();
                //▲===== #015
                // QMS Service
                if (GlobalsNAV.IsQMSEnable())
                {
                    Registration_DataStorage.CurrentPatient.ServiceStartedAt = Globals.GetCurServerDateTime();
                    //OrderDTO orderDTO = GlobalsNAV.UpdateOrder(Registration_DataStorage.CurrentPatient, OrderDTO.WAITING_STATUS);
                    OrderDTO orderDTO = GlobalsNAV.UpdateOrder(Registration_DataStorage.CurrentPatient, OrderDTO.CALLING_STATUS);
                }
            }
        }
        public void Handle(HaveTransferForm message)
        {
            if (message != null)
            {
                IsHaveTransferForm = message.IsHaveTransferForm;
            }

        }
        public void Handle(CheckValidProcedure message)
        {
            if (message != null)
            {
                IsHaveOneProcedure = message.IsHaveOneProcedure;
            }

        }
        public void Handle(AllPCLRequestImageClose message)
        {
            if (message != null)
            {
                IsAllPCLRequestImageClose = message.AllPCLClose;
            }
            if (IsAllPCLRequestImageClose && IsAllPCLRequestClose)
            {
                UpdateDateStarted2(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID, Globals.GetCurServerDateTime());
            }
        }
        public void Handle(AllPCLRequestClose message)
        {
            if (message != null)
            {
                IsAllPCLRequestClose = message.AllPCLClose;
            }
            if (IsAllPCLRequestImageClose && IsAllPCLRequestClose)
            {
                UpdateDateStarted2(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID, Globals.GetCurServerDateTime());
            }
        }
        #endregion
        private bool _IsHaveOneProcedure = true;
        public bool IsHaveOneProcedure
        {
            get
            {
                return _IsHaveOneProcedure;
            }
            set
            {
                if (_IsHaveOneProcedure == value)
                {
                    return;
                }
                _IsHaveOneProcedure = value;
                NotifyOfPropertyChange(() => IsHaveOneProcedure);
            }
        }
        private bool _IsAllPCLRequestImageClose = false;
        public bool IsAllPCLRequestImageClose
        {
            get
            {
                return _IsAllPCLRequestImageClose;
            }
            set
            {
                if (_IsAllPCLRequestImageClose == value)
                {
                    return;
                }
                _IsAllPCLRequestImageClose = value;
                NotifyOfPropertyChange(() => IsAllPCLRequestImageClose);
            }
        }
        private bool _IsAllPCLRequestClose = false;
        public bool IsAllPCLRequestClose
        {
            get
            {
                return _IsAllPCLRequestClose;
            }
            set
            {
                if (_IsAllPCLRequestClose == value)
                {
                    return;
                }
                _IsAllPCLRequestClose = value;
                NotifyOfPropertyChange(() => IsAllPCLRequestClose);
            }
        }
        public void ClearDataCS_DS()
        {
            CS_DS.DiagTreatment = new DiagnosisTreatment();
            CS_DS.refIDC10List = new ObservableCollection<DiagnosisIcd10Items>();
            CS_DS.TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
        }
        private void CreateSubVM()
        {
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCSummary = Globals.GetViewModel<ISummary_V3>();
            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            //▼====: #023
            //UCCommonRecs = Globals.GetViewModel<ICommonRecs>();
            //▲====: #023
            UCPatientTreeForm = Globals.GetViewModel<IPatientTreeForm>();
            //▼====: #023
            //UCCommonRecs.IsShowSummaryContent = false;v
            //▲====: #023
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
            searchPatientAndRegVm.IsSearchOnlyProcedure = IsSearchOnlyProcedure;
            searchPatientAndRegVm.IsShowBtnChooseUserOfficial = Globals.ServerConfigSection.CommonItems.AllowToBorrowDoctorAccount;
            SearchRegistrationContent = searchPatientAndRegVm;
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo.isPreNoteTemp = true;
            UCPatientPCLRequest = Globals.GetViewModel<IPatientPCLRequest>();
            UCPatientPCLRequestImage = Globals.GetViewModel<IPatientPCLRequestImage>();
            //▼====: #023
            //UCPatientPCLLaboratoryResult = Globals.GetViewModel<IPatientPCLLaboratoryResult>();
            //UCPatientPCLImagingResult = Globals.GetViewModel<IPatientPCLImagingResult>();
            //UCPatientPCLDeptImagingExtHome = Globals.GetViewModel<IPatientPCLDeptImagingExtHome>();
            //▲====: #023
            UCPatientPCLRequest.IsShowSummaryContent = false;
            UCPatientPCLRequestImage.IsShowSummaryContent = false;
            //▼====: #023
            //UCPatientPCLLaboratoryResult.IsShowSummaryContent = false;
            //UCPatientPCLImagingResult.IsShowSummaryContent = false;
            //UCPatientPCLDeptImagingExtHome.IsShowSummaryContent = false;
            //▲====: #023
            UCPatientProfileInfo.CS_DS = CS_DS;
            UCSummary.CS_DS = CS_DS;
            ((IPtRegDetailInfo)UCPtRegDetailInfo).CS_DS = CS_DS;
            UCePrescriptions.CS_DS = CS_DS;
            //▼====: #023
            //UCCommonRecs.CS_DS = CS_DS;
            //▲====: #023
            UCPatientTreeForm.CS_DS = CS_DS;
            ((ISearchPatientAndRegistration)SearchRegistrationContent).CS_DS = CS_DS;
            UCDoctorProfileInfo.CS_DS = CS_DS;
            UCPatientPCLRequest.CS_DS = CS_DS;
            UCPatientPCLRequestImage.CS_DS = CS_DS;
            //▼====: #023
            //UCPatientPCLLaboratoryResult.CS_DS = CS_DS;
            //UCPatientPCLImagingResult.CS_DS = CS_DS;
            //UCPatientPCLDeptImagingExtHome.CS_DS = CS_DS;
            //▲====: #023
            Registration_DataStorage = new Registration_DataStorage();
        }
        private void ActivateSubVM()
        {
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCSummary);
            ActivateItem(UCPtRegDetailInfo);
            //▼====: #023
            //ActivateItem(UCCommonRecs);
            //▲====: #023
            ActivateItem(UCPatientTreeForm);
            ActivateItem(SearchRegistrationContent);
            ActivateItem(UCDoctorProfileInfo);
            ActivateItem(UCPatientPCLRequest);
            ActivateItem(UCPatientPCLRequestImage);
            //▼====: #023
            //ActivateItem(UCPatientPCLLaboratoryResult);
            //ActivateItem(UCPatientPCLImagingResult);
            //ActivateItem(UCPatientPCLDeptImagingExtHome);
            //▲====: #023
        }
        private void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCPatientProfileInfo, close);
            DeactivateItem(UCSummary, close);
            DeactivateItem(UCPtRegDetailInfo, close);
            //▼====: #023
            //DeactivateItem(UCCommonRecs, close);
            //▲====: #023
            DeactivateItem(UCPatientTreeForm, close);
            DeactivateItem(SearchRegistrationContent, close);
            DeactivateItem(UCDoctorProfileInfo, close);
            DeactivateItem(UCPatientPCLRequest, close);
            DeactivateItem(UCPatientPCLRequestImage, close);
            //▼====: #023
            //DeactivateItem(UCPatientPCLLaboratoryResult, close);
            //DeactivateItem(UCPatientPCLImagingResult, close);
            //DeactivateItem(UCPatientPCLDeptImagingExtHome, close);
            //▲====: #023
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
            //▼====== #019
            LoadListFilterPrescriptionsHasHIPay();
            //▲====== #019
            //▼====== #021
            LoadGetListRequiredSubDiseasesReferences();
            //▲====== #021
            //▼====== #022
            LoadGetListRuleDiseasesReferences();
            //▲====== #022
        }
        protected override void OnDeactivate(bool close)
        {
            System.Diagnostics.Debug.WriteLine(" ====>  ConsultationSummaryViewModel - OnDeactivate - BEGIN");
            DeActivateSubVM(close);
            CleanUpDataStorage();
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
            //20200506 TBL: Không cần phải kiểm tra khi chưa lưu chỉ định CLS
            //if (UCPatientPCLRequest.btSaveIsEnabled || UCPatientPCLRequestImage.btSaveIsEnabled)
            //{
            //    MessageBox.Show(eHCMSResources.Z2297_G1_VuiLongHoanTatCDCLS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            //    return;
            //}
            //20181219 TBL: Khong cho cap nhat khi dang ky da dc bao cao BHYT

            // VuTTM - QMS Service
            if (Registration_DataStorage != null
                && Registration_DataStorage.CurrentPatient != null
                && GlobalsNAV.IsQMSEnable())
            {
                OrderDTO orderDTO = GlobalsNAV.UpdateOrder(Registration_DataStorage.CurrentPatient, OrderDTO.DONE_STATUS);
            }

            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistration == null
                || Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
            {
                return;
            }
            //Coroutine.BeginExecute(CoroutineCheckTransferForm());
            if (UCSummary.CS_DS.DiagTreatment.V_TreatmentType == (long)AllLookupValues.V_TreatmentType.Transfer && !IsHaveTransferForm)
            {
                Globals.ShowMessage("Chưa có giấy chuyển tuyến vui lòng nhập giấy chuyển tuyến", "Thông báo");
                return;
            }
            if (!IsHaveOneProcedure && UCePrescriptions.IsPrescriptionChanged == true)
            {
                Globals.ShowMessage("Cách điều trị không cần ra toa thuốc", "Thông báo");
                return;
            }
            //▼==== #026
            //if (UCSummary.CheckValidDiagnosis() && UCePrescriptions.CheckValidPrescription())
            //{
            //    Coroutine.BeginExecute(CoroutineUpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0));
            //}

            //if (!UCSummary.btUpdateIsEnabled)
            //{
            //    Coroutine.BeginExecute(CoroutineLoadListPCLExamAccordingICD(UCSummary.CS_DS.CurrentPatient.PatientID, UCSummary.DiagTrmtItem.ICD10List));
            //}
           
            if ((UCSummary.DiagTrmtItem != null && UCSummary.DiagTrmtItem.DTItemID == 0) ||
                (UCePrescriptions.IsPrescriptionChanged && UCePrescriptions.btnSaveAddNewIsEnabled))
            {
                //Coroutine.BeginExecute(CoroutineLoadListPCLExamAccordingICD(UCSummary.CS_DS.CurrentPatient.PatientID, UCSummary.DiagTrmtItem.ICD10List));
                //var ICD10List = string.Join(",", UCSummary.refIDC10List.Where(x => !string.IsNullOrEmpty(x.ICD10Code) && x.DiseasesReference != null)
                //.Select(x => x.ICD10Code));
                Coroutine.BeginExecute(CoroutineLoadListPCLExamAccordingICD(UCSummary.CS_DS.CurrentPatient.PatientID,
                    UCSummary.Registration_DataStorage.CurrentPatientRegistrationDetail.RefMedicalServiceItem.V_SpecialistType,
                    Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID));
            }
            else
            {
                if (UCSummary.CheckValidDiagnosis() && UCePrescriptions.CheckValidPrescription())
                {
                    Coroutine.BeginExecute(CoroutineUpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0));
                }
            }
            //▲==== #026

            if (Globals.ServerConfigSection.CommonItems.AutoSavePhysicalExamination && UCSummary.PtPhyExamItem != null && Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null)
            {
                if (UCSummary.PtPhyExamItem.PtRegistrationID != Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID)
                {
                    UCSummary.PtPhyExamItem.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                    UCSummary.PtPhyExamItem.PhyExamID = 0;
                    UCSummary.PtPhyExamItem.CommonMedRecID = 0;
                    var t = new Thread(() =>
                    {
                        try
                        {
                            using (var serviceFactory = new SummaryServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                contract.BeginAddNewPhysicalExamination_V2(UCSummary.PtPhyExamItem, Globals.LoggedUserAccount.StaffID
                                    , Globals.DispatchCallback((asyncResult) =>
                                    {
                                        try
                                        {
                                            var results = contract.EndAddNewPhysicalExamination_V2(asyncResult);
                                            Globals.EventAggregator.Publish(new CommonClosedPhysicalForSummaryEvent());
                                        }
                                        catch (Exception ex)
                                        {
                                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
            }
        }

        public void btnCancel()
        {
            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistration == null
                || Globals.IsLockRegistration(Registration_DataStorage.CurrentPatientRegistration.RegLockFlag, eHCMSResources.Z0407_G1_CNhatCDoan))
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


        //private bool _IsHaveTransferForm;
        //public bool IsHaveTransferForm
        //{
        //    get
        //    {
        //        return _IsHaveTransferForm;
        //    }
        //    set
        //    {
        //        if (_IsHaveTransferForm != value)
        //        {
        //            _IsHaveTransferForm = value;
        //            NotifyOfPropertyChange(() => IsHaveTransferForm);
        //        }
        //    }
        //}
        //private void CheckTransferForm()
        //{
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ePMRsServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginGetTransferFormByPtRegistrationID(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID,  /*TMA*/
        //                Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {

        //                        IList<TransferForm> Result = contract.EndGetTransferFormByPtRegistrationID(asyncResult);
        //                        if (Result == null || Result.Count <= 0)
        //                        {
        //                            IsHaveTransferForm = false;
        //                        }
        //                        else
        //                        {
        //                            IsHaveTransferForm = true;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                        ClientLoggerHelper.LogError(ex.Message);

        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //            ClientLoggerHelper.LogError(ex.Message);
        //            this.HideBusyIndicator();
        //            IsHaveTransferForm = false;
        //        }
        //    });
        //    t.Start();
        //}
        //private IEnumerator<IResult> CoroutineCheckTransferForm()
        //{
        //    CheckTransferForm();
        //    yield break;
        //}

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
                UpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0);
                UCePrescriptions.ResetPrescriptionInfoChanged();
            }
            //20200111 TBL: Nếu không đồng ý thì load lại cả chẩn đoán và toa thuốc
            else
            {
                PatientServiceRecordsGetForKhamBenh_Ext(false, true);
            }
            yield break;
        }
        private IEnumerator<IResult> CoroutineUpdateDiagnosisTreatmentAndPrescription(bool IsUpdate = false, bool IsPrescriptionUpdate = false, bool IsHaveEvent = false)
        {
            //▼==== #027
            yield return GenericCoRoutineTask.StartTask(CheckBeforeUpdateDiagnosisTreatment_Action);

            if (!string.IsNullOrEmpty(ErrorMessages))
            {
                //ErrorMessages = string.Format("{0}: ", eHCMSResources.Z1405_G1_LoiSapNhapKgThanhCong) + Environment.NewLine + ErrorMessages;

                errorMessageBox = new WarningWithConfirmMsgBoxTask(ErrorMessages, "", false);
                yield return errorMessageBox;

                errorMessageBox = null;
                yield break;
            }

            if (!string.IsNullOrEmpty(ConfirmMessages))
            {
                //ConfirmMessages = "Xác nhận sáp nhập đăng ký: " + Environment.NewLine + ConfirmMessages + Environment.NewLine +
                //    "Bạn có đồng ý sáp nhập cho bệnh nhân này không?";
                //confirmBeforeDischarge = new WarningWithConfirmMsgBoxTask(ConfirmMessages, "Tiếp tục sáp nhập",false);
                //yield return confirmBeforeDischarge;
                //if (!confirmBeforeDischarge.IsAccept)
                //{
                //    confirmBeforeDischarge = null;
                //    yield break;
                //}
                //confirmBeforeDischarge = null;
                MessageBox.Show(ConfirmMessages);
            }
            //▲==== #027
            UpdateDiagnosisTreatmentAndPrescription(IsUpdate, IsPrescriptionUpdate, IsHaveEvent);
            yield break;
        }
        private IEnumerator<IResult> CoroutineLoadListPCLExamAccordingICD(long PatientID, long V_SpecialistType, long PtRegistrationID)
        {
            var Task = new GenericCoRoutineTask(LoadListPCLExamAccordingICD, PatientID, V_SpecialistType, PtRegistrationID);
            yield return Task;
            //▼==== #026
            List<PCLExamAccordingICD> temp = Task.GetResultObj(0) as List<PCLExamAccordingICD>;
            if (temp != null && temp.Count > 0)
            {
                Action<IPCLExamAccordingICD> onInitDlg = delegate (IPCLExamAccordingICD _pclExamAccordingICD)
                {
                    _pclExamAccordingICD.ListPCLExamAccordingICD = temp;
                };
                GlobalsNAV.ShowDialog<IPCLExamAccordingICD>(onInitDlg, (o, s) =>
                {
                    if (UCSummary.CheckValidDiagnosis() && UCePrescriptions.CheckValidPrescription())
                    {
                        Coroutine.BeginExecute(CoroutineUpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0, false, true));
                        if (Globals.ServerConfigSection.CommonItems.ApplyReport130 && UCSummary.DiagTrmtItem.DTItemID == 0)
                        {
                            Coroutine.BeginExecute(CreateCheckIn_130_OutInPtXml_Routine());
                        }
                    }
                });
            }
            else
            {
                if (UCSummary.CheckValidDiagnosis() && UCePrescriptions.CheckValidPrescription())
                {
                    Coroutine.BeginExecute(CoroutineUpdateDiagnosisTreatmentAndPrescription(UCSummary.DiagTrmtItem.DTItemID > 0));
                    if (Globals.ServerConfigSection.CommonItems.ApplyReport130 && UCSummary.DiagTrmtItem.DTItemID == 0)
                    {
                        Coroutine.BeginExecute(CreateCheckIn_130_OutInPtXml_Routine());
                    }
                }
            }
            yield break;
            //▲==== #026
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
            UCSummary.DiagTrmtItem.DoctorAccountBorrowedID = Globals.DoctorAccountBorrowed.StaffID ?? 0;
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
                //20200411 TBL: Khi nào lưu thành công thì mới set lại
                //UCSummary.IsDiagTrmentChanged = false;
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
                //▼====: #025
                //GetV_PrescriptionIssuedTypeForIssue(objPre);
                //▲====: #025
            }
            if (objDiag == null && objPre == null && UCSummary.UpdatedSmallProcedure == null)
            {
                MessageBox.Show(eHCMSResources.Z2299_G1_KhongCoGiDeLuu);
                return false;
            }
            //if (UCSummary.DiagTrmtItem == null && UCSummary.UpdatedSmallProcedure != null)
            //20190306 TBL: Rang buoc thu thuat phai nhap chan doan
            if ((UCSummary.UpdatedSmallProcedure == null && objDiag == null && Registration_DataStorage.PatientServiceRecordCollection != null
                && Registration_DataStorage.PatientServiceRecordCollection.Count == 0)
                || (UCSummary.UpdatedSmallProcedure != null && UCSummary.UpdatedSmallProcedure.ServiceRecID == 0 && objDiag == null && Registration_DataStorage.PatientServiceRecordCollection != null
                && Registration_DataStorage.PatientServiceRecordCollection.Count == 0))
            {
                MessageBox.Show(eHCMSResources.A0405_G1_Msg_InfoChuaCoCD);
                return false;
            }

            return true;
        }

        /// <summary>
        /// QMS Service
        /// </summary>
        /// <param name="requestOrderDTO"></param>
        /// <param name="PtRegDetailID"></param>
        /// <param name="PCLReqItemID"></param>
        public void GetOrder(OrderDTO requestOrderDTO, long PtRegDetailID = 0, long PCLReqItemID = 0)
        {
            //string mJsonData = GlobalsNAV.ConvertObjectToJson(requestOrderDTO);
            //string mJsonData = requestOrderDTO.PostString();
            //try
            //{
            //    //Task<string> Request = GlobalsNAV.PostRESTServiceJSon_NewAsync(Globals.ServerConfigSection.CommonItems.QMS_API_Url, eHCMSResources.Z3115_G1_PublishOrder, mJsonData);
            //    //if (Request.Status == TaskStatus.Created)
            //    //    Request.Start();
            //    //var result = Request.Result;
            //    var result = GlobalsNAV.PostRESTServiceJSon_New(Globals.ServerConfigSection.CommonItems.QMS_API_Url, eHCMSResources.Z3115_G1_PublishOrder, mJsonData);
            //    if (result != null)
            //    {
            //        dynamic Obj = GlobalsNAV.ConvertJsonToObject_New(result);
            //        //UpdateOrderNumber(PtRegDetailID, PCLReqItemID, (long)Obj["orderNumber"]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        /// <summary>
        /// QMS Service
        /// </summary>
        /// <param name="requestOrderDTO"></param>
        public void UpdateOrder(OrderDTO requestOrderDTO)
        {
            //string UrlData = requestOrderDTO.PutString();
            //try
            //{

            //    var result = GlobalsNAV.PutRESTServiceJSon_New(Globals.ServerConfigSection.CommonItems.QMS_API_Url, eHCMSResources.Z3115_G1_PublishOrder, UrlData, requestOrderDTO);
            //    if (result != null)
            //    {
            //        dynamic Obj = GlobalsNAV.ConvertJsonToObject_New(result);
            //        //UpdateOrderNumber(PtRegDetailID, PCLReqItemID, (long)Obj["orderNumber"]);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        private void ProcessReturnResultAfterSaving(int SaveRecResult, Int16 SaveDiagPrescriptFlag, long OutServiceRecID, long DTItemID, bool IsUpdate, Prescription objPre, string OutError, bool IsMessageBox, int VersionNumberOut, long SmallProcedureID = 0)
        {
            if (SaveRecResult == SaveDiagPrescriptFlag)    // Saving all OK                        
            {

                bool bAllowModifyPrescription = false;
                UCSummary.IsDiagTrmentChanged = false;
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
                else if (SmallProcedureID > 0)
                {
                    PatientServiceRecordsGetForKhamBenh_Ext(false, false, true);
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

                // VuTTM QMS Service
                //if (GlobalsNAV.IsQMSEnable() && !IsUpdate && objPre != null)
                //{
                //    OrderDTO orderDTO = GlobalsNAV.UpdateOrder(Registration_DataStorage.CurrentPatient, OrderDTO.DONE_STATUS);
                //UpdateOrder(new RequestOrderDTO
                //{
                //    orderStatus = "DONE",
                //    patientId = UCSummary.CS_DS.CurrentPatient.PatientID,
                //    refLocationId = 1,//(long)UCSummary.Registration_DataStorage.CurrentPatientRegistrationDetail.DeptLocID,
                //    refDeptId = 1//(long)UCSummary.Registration_DataStorage.CurrentPatientRegistration.DeptID
                //});
                //if (objPre.PrescriptionDetails.Where(x => x.BeOfHIMedicineList == true) == null)// không có thuốc bh
                //{
                //    GetOrder(new RequestOrderDTO
                //    {
                //        patientId = UCSummary.CS_DS.CurrentPatient.PatientID,
                //        patientName = UCSummary.CS_DS.CurrentPatient.FullName,
                //        patientDOB = (DateTime)UCSummary.CS_DS.CurrentPatient.DOB,
                //        description = "",
                //        hightPriority = true,
                //        refDeptId = 2,//temp.DeptLocation.DeptID,
                //        refLocationId = 6//temp.DeptLocation.LID
                //    });
                //}
                //else // có thuốc bh
                //{
                //    GetOrder(new RequestOrderDTO
                //    {
                //        patientId = UCSummary.CS_DS.CurrentPatient.PatientID,
                //        patientName = UCSummary.CS_DS.CurrentPatient.FullName,
                //        patientDOB = (DateTime)UCSummary.CS_DS.CurrentPatient.DOB,
                //        description = "",
                //        hightPriority = true,
                //        refDeptId = 2,//temp.DeptLocation.DeptID,
                //        refLocationId = 6//temp.DeptLocation.LID
                //    });
                //}
                //}
            }
            else
            {
                if (SaveRecResult > 0)  // Luu ca 2 chan doan va toa thuoc nhung chi chan doan luu thanh cong
                {
                    UCSummary.IsDiagTrmentChanged = false;
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
                    //20200111 TBL: Không được load lại ở đây. Chỗ này không cần thiết phải load lại.
                    //PatientServiceRecordsGetForKhamBenh_Ext(false, true);
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
                                    //▼===== #018
                                    //UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    //IsMessageBox = false;
                                    //Coroutine.BeginExecute(WarningReturnDrugNotEnough());
                                    //break;

                                    UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    MessageBox.Show(eHCMSResources.Z3068_G1_ToaDaXuatKhongTheCapNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //Coroutine.BeginExecute(WarningReturnDrugNotEnough());
                                    break;
                                    //▲===== #018
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
            //▼===== #015
            UCePrescriptions.ServiceRecIDDiagTrmt = UCSummary.DiagTrmtItem.ServiceRecID.GetValueOrDefault();
            UCePrescriptions.PtRegistrationID = UCSummary.DiagTrmtItem.PatientServiceRecord.PtRegistrationID.GetValueOrDefault();
            //▲===== #015
            //20200111 TBL: Luôn luôn load lại phác đồ khi lưu 
            if (Registration_DataStorage.CurrentPatientRegistrationDetail != null && Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID > 0)
            {
                GetRefTreatmentRegimensAndDetail(Registration_DataStorage.CurrentPatientRegistrationDetail);
            }
        }
        private bool CheckValidSmallProcedure(SmallProcedure aSmallProcedure)
        {

            StringBuilder sb = new StringBuilder();
            if (aSmallProcedure == null)
            {
                return true;
            }
            if (aSmallProcedure.CompletedDateTime < aSmallProcedure.ProcedureDateTime)
            {
                sb.AppendLine(eHCMSResources.Z3019_G1_MsgNgayBatDauNgayKetThuc);
            }
            if (aSmallProcedure.ProcedureDateTime == DateTime.MinValue)
            {
                sb.AppendLine(eHCMSResources.Z2408_G1_VuiLongNhapTGPTTT);
            }
            //▼===== 20191212: Tường minh thông tin báo lỗi khi người dùng không có chẩn đoán trước thủ thuật phẫu thuật.
            if (aSmallProcedure.BeforeICD10 == null || string.IsNullOrEmpty(aSmallProcedure.BeforeICD10.DiseaseNameVN))
            {
                sb.AppendLine(eHCMSResources.Z2940_G1_ChanDoanTruocTTPT);
            }
            //▲===== 
            if (aSmallProcedure.AfterICD10 == null || string.IsNullOrEmpty(aSmallProcedure.AfterICD10.DiseaseNameVN))
            {
                sb.AppendLine(eHCMSResources.Z2915_G1_VLNhapCDSauPT);
            }
            if (string.IsNullOrEmpty(aSmallProcedure.ProcedureMethod))
            {
                sb.AppendLine(eHCMSResources.Z2408_G1_VLNhapPhuongPhapTTPT);
            }
            if (aSmallProcedure.CompletedDateTime == DateTime.MinValue)
            {
                sb.AppendLine(eHCMSResources.Z2916_G1_VLNhapNgayKetThuc);
            }
            //20200217 TBL: Bắt buộc phải nhập 1 trong 8 trường để lấy CCHN báo cáo BHYT
            if (aSmallProcedure.ProcedureDoctorStaffID == null && aSmallProcedure.ProcedureDoctorStaffID2 == null && aSmallProcedure.NurseStaffID == null && aSmallProcedure.NurseStaffID2 == null
                && aSmallProcedure.NurseStaffID3 == null && aSmallProcedure.NarcoticDoctorStaffID == null && aSmallProcedure.NarcoticDoctorStaffID2 == null && aSmallProcedure.CheckRecordDoctorStaffID == null)
            {
                sb.AppendLine(eHCMSResources.Z2981_G1_ChuaChonNguoiThucHien);
            }

            //▼===== #017
            TimeSpan CompareTime = new TimeSpan();
            CompareTime = aSmallProcedure.CompletedDateTime.Subtract(aSmallProcedure.ProcedureDateTime);
            if (CompareTime.TotalHours > Globals.ServerConfigSection.CommonItems.MaxTimeForSmallProcedure)
            {
                sb.AppendLine(string.Format(eHCMSResources.Z2994_G1_KetThucKhongVuotQuaBatDau, Globals.ServerConfigSection.CommonItems.MaxTimeForSmallProcedure));
            }
            //▲===== #017
            if (string.IsNullOrWhiteSpace(aSmallProcedure.TrinhTu))
            {
                sb.AppendLine("Chưa nhập trình tự thủ thuật!");
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            else
            {
                return true;
            }
        }
        private void UpdateDiagnosisTreatmentAndPrescription(bool IsUpdate = false, bool IsPrescriptionUpdate = false, bool IsHaveEvent = false)
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
                    //▼===== #014
                    if (Globals.ConfirmSecretaryLogin != null)
                    {
                        if (Globals.ConfirmSecretaryLogin.Staff != null)
                        {
                            if (mPrecriptionsBeforeUpdate != null && objPre != null)
                            {
                                objPre.MedSecretaryID = Globals.ConfirmSecretaryLogin.Staff.StaffID;
                            }
                            if (objDiag != null)
                            {
                                objDiag.MedSecretaryID = Globals.ConfirmSecretaryLogin.Staff.StaffID;
                            }
                        }
                    }
                    //▲===== #014
                    //▼==== #028
                    if (objPre != null && objPre.PrescriptionIssueHistory != null)
                    {
                        objPre.PrescriptionIssueHistory.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID ?? 0;
                    }
                    //▲==== #028
                    contract.BeginAddDiagnosisTreatmentAndPrescription(IsUpdateWithoutChangeDoctorIDAndDatetime, objDiag, UCSummary.Compare2Object(), UCSummary.refIDC10List
                            , UCSummary.Compare2ICD9List(), UCSummary.refICD9List
                            , (short)Globals.ServerConfigSection.CommonItems.NumberTypePrescriptions_Rule, objPre
                            , mPrecriptionsBeforeUpdate, UCePrescriptions.AllowUpdateThoughReturnDrugNotEnough
                            , mSmallProcedure, Globals.LoggedUserAccount != null && Globals.LoggedUserAccount.StaffID.HasValue ? Globals.LoggedUserAccount.StaffID.Value : 0
                            , UCSummary.SelectedResourceList
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
                                        UCSummary.FormEditorIsEnabled = true;
                                    }

                                    //20220331 QTD Thêm cấu hình bật/tắt QMS cho Duyệt toa
                                    if (GlobalsNAV.IsQMSEnable() && Globals.ServerConfigSection.CommonItems.IsEnableQMSForPrescription)
                                    {
                                        if (objPre != null && objPre.PrescriptionDetails != null)
                                        {
                                            GlobalsNAV.CreatePrescriptionOrders(Registration_DataStorage.CurrentPatient, OutPrescriptionID, objPre.PrescriptionDetails);
                                        }
                                    }

                                    ProcessReturnResultAfterSaving(SaveRecResult, SaveDiagPrescriptFlag, OutServiceRecID, DTItemID, IsUpdate, objPre, OutError, IsMessageBox, VersionNumberOut, SmallProcedureID);
                                    /*▲====: #007*/
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    if (ex.Message.Contains("[ERROR-TBL]"))
                                    {
                                        ClientLoggerHelper.LogError("ConsultationsSummary_V2ViewModel UpdateDiagnosisTreatmentAndPrescription - " + Globals.GetCurServerDateTime() + " -  [" + Globals.LoggedUserAccount.StaffID + " - "
                                        + Globals.LoggedUserAccount.Staff.FullName + "] - " + "PatientID: " + (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null ? Registration_DataStorage.CurrentPatient.PatientID : 0).ToString()
                                        + " PtRegistrationID: " + (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null ? Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID : 0).ToString()
                                        + " PtRegDetailID: " + (mSmallProcedure != null ? mSmallProcedure.PtRegDetailID : 0).ToString()
                                        + " SmallProcedureID: " + (mSmallProcedure != null ? mSmallProcedure.SmallProcedureID : 0).ToString() + ex.Message);
                                    }
                                }
                                finally
                                {
                                    if (EditPrescription)
                                    {
                                        UCePrescriptions.AddNewBlankDrugIntoPrescriptObjectNew();
                                    }
                                    if (IsHaveEvent)
                                    {
                                        Globals.EventAggregator.Publish(new UpdateDiagnosisTreatmentAndPrescription_Event { Result = true });
                                    }
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
                //20191128 TBL: Hiện tại do Toa thuốc đã không còn tab nên không cần
                //TBL: Khi qua tab Ra toa thi tu dong lay phac do
                //if (destTabIndex == 1)
                //{
                //    UCePrescriptions.GetPhacDo();
                //    //TTM 24102018:     Khi bệnh nhân vừa đc thêm tình trạng thể chất => toa thuốc đã đc load trước khi thêm => không có tình trạng thể chất bên toa thuốc
                //    //                  => khi vừa thêm/ cập nhật tình trạng thể chất bên chẩn đoán => bên ra toa cũng đc thêm/ cập nhật lại.
                //    if (Globals.curPhysicalExamination != null)
                //    {
                //        UCePrescriptions.curPhysicalExamination = Globals.curPhysicalExamination;
                //    }
                //    _currentTabIndex = destTabIndex;
                //}
                //==== #009 ==== //TBL: Co 4 tab can an nut luu
                if (destTabIndex != 0)
                {
                    bEnableSave = false;
                }
                if (destTabIndex == 0)
                {
                    bEnableSave = true;
                }
                //▼====: #023
                //==== #009 ====
                //20181124 TBL: Khi chon tab Thong tin tong quat se di load Vaccine theo MedServiceID
                //if (destTabIndex == 5)
                //{
                //    UCCommonRecs.GetRefImmunization(MedSerID);
                //    _currentTabIndex = destTabIndex;
                //}
                //else
                //{
                _currentTabIndex = destTabIndex;
                //}
                //▲====: #023
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
                //▼====: #020
                if (p.RefMedicalServiceItem != null && p.RefMedicalServiceItem.IsAllowToPayAfter == 0 && p.PaidTime == null
                    && ObjPR.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.PayAfter
                    && ObjPR.PatientClassID.GetValueOrDefault(0) != (long)ePatientClassification.CompanyHealthRecord
                    && !Globals.ServerConfigSection.CommonItems.AllowFirstHIExaminationWithoutPay)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1470_G1_DV0ChuaTraTienKgTheKB, p.RefMedicalServiceItem.MedServiceName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                //▲====: #020
            }
            if (p.V_ExamRegStatus == (long)V_ExamRegStatus.mDangKyKham || p.V_ExamRegStatus == (long)V_ExamRegStatus.mChoKham || p.V_ExamRegStatus == (long)V_ExamRegStatus.mHoanTat || p.V_ExamRegStatus == (long)V_ExamRegStatus.mBatDauThucHien
                || (p.V_ExamRegStatus == (long)V_ExamRegStatus.mNgungTraTienLai && ObjPR.IsAdmission))
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
            if (Globals.IsLockRegistration(ObjPR.RegLockFlag, "Khám bệnh, ra toa và chỉ định cận lâm sàng"))
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
            //▼====: #024
            Globals.StartDatetimeExam = Globals.GetCurServerDateTime();
            //▲====: #024
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
                                UCSummary.NotifyViewDataChanged();
                                DiagnosisTreatmentHistoriesTreeContent.NotifyViewDataChanged();
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
            , bool IsReloadPrescription = true, bool IsUpdateSmallProcedureOnly = false)
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null)
            {
                MessageBox.Show(eHCMSResources.A0733_G1_Msg_InfoKhTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            PatientServiceRecordsGetForKhamBenh(Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID, Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID, Registration_DataStorage.CurrentPatientRegistration.V_RegistrationType, bAllowModifyPrescription
                , IsReloadPrescription, IsUpdateSmallProcedureOnly);
        }
        public void PatientServiceRecordsGetForKhamBenh(long PtRegistrationID, long PtRegDetailID, AllLookupValues.RegistrationType V_RegistrationType, bool bAllowModifyPrescription
            , bool IsReloadPrescription, bool IsUpdateSmallProcedureOnly)
        {
            this.ShowBusyIndicator();
            PatientServiceRecord psrSearch = new PatientServiceRecord();
            psrSearch.PtRegistrationID = PtRegistrationID;
            psrSearch.PtRegDetailID = PtRegDetailID;
            psrSearch.V_RegistrationType = V_RegistrationType;
            psrSearch.IsUpdateSmallProcedureOnly = IsUpdateSmallProcedureOnly;
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
                                UCSummary.FormEditorIsEnabled = !Registration_DataStorage.CurrentPatientRegistration.IsAdmission;
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
                                    mSmallProcedure.CompletedDateTime = mSmallProcedure.ProcedureDateTime.AddMinutes(mSmallProcedure.ServiceMainTime);//Globals.GetCurServerDateTime();
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
                            mSmallProcedure.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
                            GetSmallProcedureTime(mSmallProcedure, MedServiceID);
                            //UCSummary.ApplySmallProcedure(mSmallProcedure);
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
        public void GetSmallProcedureTime(SmallProcedure mSmallProcedure, long MedServiceID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonUtilsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSmallProcedureTime(MedServiceID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            int SmallProcedureTime = contract.EndGetSmallProcedureTime(asyncResult);
                            if (mSmallProcedure.ProcedureDateTime == DateTime.MinValue)
                            {
                                mSmallProcedure.ProcedureDateTime = Globals.GetCurServerDateTime();
                            }
                            if (SmallProcedureTime > 0)
                            {
                                mSmallProcedure.CompletedDateTime = mSmallProcedure.ProcedureDateTime.AddMinutes(SmallProcedureTime);
                            }
                            UCSummary.ApplySmallProcedure(mSmallProcedure);
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
        //▼===== #014
        public void btnAddMedSecretary()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID == 0)
            {
                return;
            }
            Action<ILogin> onInitDlg = delegate (ILogin proAlloc)
            {
                proAlloc.IsConfirmForSecretary = true;
            };
            GlobalsNAV.ShowDialog<ILogin>(onInitDlg);
            if (Globals.ConfirmSecretaryLogin != null)
            {
                UCePrescriptions.Secretary = Globals.ConfirmSecretaryLogin.Staff;
            }
        }
        //▲===== #014
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
        //▼===== #016
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
        //▲===== #016
        //▼===== #019
        private void LoadListFilterPrescriptionsHasHIPay()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetFilterPrescriptionsHasHIPay(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListFilterPrescriptionsHasHIPay = contract.EndGetFilterPrescriptionsHasHIPay(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲===== #019
        //▼===== #021
        private void LoadGetListRequiredSubDiseasesReferences()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListRequiredSubDiseasesReferences("*", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListRequiredSubDiseasesReferences = contract.EndGetListRequiredSubDiseasesReferences(asyncResult).ToList();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲===== #021
        //▼===== #022
        private void LoadGetListRuleDiseasesReferences()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListRuleDiseasesReferences("*", Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                ListRuleDiseasesReferences = contract.EndGetListRuleDiseasesReferences(asyncResult).ToList();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        //▲===== #022
        private void LoadListPCLExamAccordingICD(GenericCoRoutineTask genTask, object PatientID, object V_SpecialistType, object PtRegistrationID)
        {
            long nPatientID = Convert.ToInt32(PatientID);
            long nV_SpecialistType = Convert.ToInt32(V_SpecialistType);
            long nPtRegistrationID = Convert.ToInt32(PtRegistrationID);
            List<PCLExamAccordingICD> Result = new List<PCLExamAccordingICD>();
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetListPCLExamAccordingICD(nPatientID, nV_SpecialistType, nPtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Result = contract.EndGetListPCLExamAccordingICD(asyncResult).ToList();
                            }
                            catch (Exception ex)
                            {

                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                genTask.AddResultObj(Result);
                                genTask.ActionComplete(true);
                                this.HideBusyIndicator();
                            }
                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void UpdateDateStarted2(long PtRegDetailID, DateTime DateStarted2)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PCLsClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginUpdateDateStarted2(PtRegDetailID, DateStarted2, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                client.EndUpdateDateStarted2(asyncResult);
                            }
                            catch (Exception innerEx)
                            {
                                MessageBox.Show(innerEx.Message);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }
        //▼==== #025
        private void GetV_PrescriptionIssuedTypeForIssue(Prescription prescription)
        {
            foreach (var PsychotropicDrugs in prescription.PrescriptionDetails.Where(x => x.DrugID > 0))
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 2)
                {
                    prescription.PrescriptionIssueHistory.V_PrescriptionIssuedType = (long)AllLookupValues.V_PrescriptionIssuedType.Gay_Nghien;
                    break;
                }
            }

            foreach (var PsychotropicDrugs in prescription.PrescriptionDetails.Where(x => x.DrugID > 0))
            {
                if (PsychotropicDrugs.RefGenericDrugDetail.RefPharmacyDrugCatID == 4)
                {
                    prescription.PrescriptionIssueHistory.V_PrescriptionIssuedType = (long)AllLookupValues.V_PrescriptionIssuedType.Huong_Than;
                    break;
                }
            }
            foreach (var detail in prescription.PrescriptionDetails.Where(x => x.DrugID > 0))
            {
                if (detail.SelectedDrugForPrescription.DrugClassID == 122)
                {
                    prescription.PrescriptionIssueHistory.V_PrescriptionIssuedType = (long)AllLookupValues.V_PrescriptionIssuedType.Dong_Y;
                    break;
                }
            }

        }
        //▲==== #025
        //▼==== #027
        private string _errorMessages;
        public string ErrorMessages
        {
            get { return _errorMessages; }
            set
            {
                _errorMessages = value;
                NotifyOfPropertyChange(() => ErrorMessages);
            }
        }

        private string _confirmMessages;
        public string ConfirmMessages
        {
            get { return _confirmMessages; }
            set
            {
                _confirmMessages = value;
                NotifyOfPropertyChange(() => ConfirmMessages);
            }
        }
        WarningWithConfirmMsgBoxTask confirmBeforeDischarge = null;
        WarningWithConfirmMsgBoxTask errorMessageBox = null;
        private void CheckBeforeUpdateDiagnosisTreatment_Action(GenericCoRoutineTask genTask)
        {
            if (Registration_DataStorage == null
                || Registration_DataStorage.CurrentPatientRegistrationDetail == null
                || Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z0083_G1_KhongTimThayDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                genTask.ActionComplete(false);
            }

            ErrorMessages = "";
            ConfirmMessages = "";

            string errorMsg = "";
            string confirmMsg = "";

            this.ShowBusyIndicator();

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ePMRsServiceClient())
                    {
                        bool bContinue = true;

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCheckBeforeUpdateDiagnosisTreatment(Registration_DataStorage.CurrentPatientRegistrationDetail.PtRegDetailID,
                            Globals.LoggedUserAccount.StaffID.Value,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var result = contract.EndCheckBeforeUpdateDiagnosisTreatment(out errorMsg, out confirmMsg, asyncResult);

                                    if (result)
                                    {
                                        ErrorMessages = errorMsg;
                                        ConfirmMessages = confirmMsg;
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z1405_G1_LoiSapNhapKgThanhCong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                        bContinue = false;
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
        //▲==== #027
        private IEnumerator<IResult> CreateCheckIn_130_OutInPtXml_Routine()
        {
            if (Registration_DataStorage.CurrentPatientRegistration == null 
                || Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID <= 0
                || Registration_DataStorage.CurrentPatientRegistration.HisID <= 0
                )
                yield break;

            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            var mCreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReport_130_XmlTask, Registration_DataStorage.CurrentPatientRegistration);
            yield return mCreateHIReportXmlTask;

            HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = mCreateHIReportXmlTask.GetResultObj(0) as HIAPIUploadHIReportXmlResult;
            //mHealthInsuranceReport.V_ReportStatus = (long)AllLookupValues.V_ReportStatus.Completed;
            //mHealthInsuranceReport.ReportAppliedCode = mHIAPIUploadHIReportXmlResult.maGiaoDich;
            //mHealthInsuranceReport.ReportAppliedResultCode = mHIAPIUploadHIReportXmlResult.maKetQua;

            //var mUpdateHIReportTask = new GenericCoRoutineTask(UpdateHIReportTask, mHealthInsuranceReport);
            //yield return mUpdateHIReportTask;

            //foreach (var item in PatientRegistrationCollection.Where(x => x.IsSelected && x.HIReportID == 0 && x.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU))
            //{
            //    item.HIReportID = mHealthInsuranceReport.HIReportID;
            //    item.ReportAppliedCode = mHealthInsuranceReport.ReportAppliedCode;
            //    item.V_ReportStatus.LookupID = mHealthInsuranceReport.V_ReportStatus;
            //    item.V_ReportStatus.ObjectValue = eHCMSResources.Z2333_G1_XacNhanBCHoanTat;
            //}


            this.HideBusyIndicator();
        }
        private void CreateHIReport_130_XmlTask(GenericCoRoutineTask genTask, object aPatientRegistration)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport_130_CheckIn_OutPt((aPatientRegistration as PatientRegistration).PtRegistrationID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var ReportStream = contract.EndGetHIXmlReport_130_CheckIn_OutPt(asyncResult);
                                    //string mHIAPICheckHICardAddress = string.Format(gIAPISendHIReportAddressParams, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password, Globals.ServerConfigSection.Hospitals.HospitalCode.Length < 2 ? "" : Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0, 2), Globals.ServerConfigSection.Hospitals.HospitalCode);
                                    //string mRestJson = GlobalsNAV.GetRESTServiceJSon(gIAPISendHIReportAddress, mHIAPICheckHICardAddress, ReportStream);
                                    //HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = GlobalsNAV.ConvertJsonToObject<HIAPIUploadHIReportXmlResult>(mRestJson);
                                    //if (mHIAPIUploadHIReportXmlResult.maKetQua == 200)
                                    //{
                                    //    genTask.AddResultObj(mHIAPIUploadHIReportXmlResult);
                                    //    genTask.ActionComplete(true);
                                    //}
                                    //else
                                    //{
                                    //    MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    //    var mErrorMessage = string.IsNullOrEmpty(mHIAPIUploadHIReportXmlResult.maGiaoDich) ? GetErrorMessageFromErrorCode(mHIAPIUploadHIReportXmlResult.maKetQua) : mHIAPIUploadHIReportXmlResult.maGiaoDich;
                                    //    if (!string.IsNullOrEmpty(mErrorMessage))
                                    //    {
                                    //        mErrorMessage = string.Format(" - {0}", mErrorMessage);
                                    //    }
                                    //    OutputErrorMessage += Environment.NewLine + string.Format("{0}: {1}{2}", eHCMSResources.T0074_G1_I, mHIAPIUploadHIReportXmlResult.maKetQua, mErrorMessage);
                                    //    ClientLoggerHelper.LogInfo(string.Format("CreateHIReportXmlTask Else => {0}", OutputErrorMessage));
                                    //    genTask.ActionComplete(false);
                                    //    this.HideBusyIndicator();
                                    //}
                                    this.HideBusyIndicator();
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
    }
}