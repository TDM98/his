using eHCMSLanguage;
using System;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows;
using System.Windows.Controls;
using DataEntities;
using aEMR.Common.BaseModel;
using aEMR.Common;
/*
 * 20181121 #001 TTM:   Chỉnh sửa Out Standing Task ngoại trú, tạo mới out standing task nội trú (thêm sự kiện load lại thông tin bệnh nhân nếu chọn từ OST nội trú)
 * 20190822 #002 TTM:   BM 0013217: Cho phép sử dụng Grid phiếu chỉ định trong đợt đăng ký để thực hiện hiệu chỉnh.
 * 20190907 #003 TBL:   BM 0013255: Nếu đăng nhập khoa khác với khoa bệnh nhân đang nằm thì không cho chỉ định
 * 20190919 #004 TTM:   BM 0014353: Gộp menu điều trị ngoại trú và chỉ định hình ảnh. Vì sử dụng cùng 1 view nên chỉ cần phân biệt đâu là bệnh nhân điều trị ngoại trú và nội trú bình thường 
 *                      Để hiển thị cho chính xác là được.
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPatientPCLRequestImage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PatientPCLRequestImageViewModel : ViewModelBase, IPatientPCLRequestImage
        , IHandle<DbClickSelectedObjectEventWithKeyForImage<PatientPCLRequest, String>>
        , IHandle<ReLoadListPCLRequest>
        , IHandle<SetInPatientInfoAndRegistrationForPatientPCLRequestImage>
        , IHandle<DbClickSelectedObjectEventWithKeyToShowDetailsForImage<PatientPCLRequest, String>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public PatientPCLRequestImageViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAgr, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAgr.Subscribe(this);

            authorization();

            CreateSubVM();
        }

        // TxD 22/09/2018 Added the following to ALLOW child VM of each Tab to get ACCESS to the CENTRAL Data stored in the MainKB (Main KHAM BENH) Screen
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
                if (UCPatientPCLRequestEdit == null)
                {
                    return;
                }
                UCPatientPCLRequestEdit.CS_DS = CS_DS;
            }
        }

        private void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();

            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            ((IPatientInfo)UCPatientProfileInfo).CS_DS = CS_DS;

            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();

            var uc4 = Globals.GetViewModel<IListPCLRequest_Common>();
            uc4.SearchCriteria = new PatientPCLRequestSearchCriteria();
            UCPatientPCLRequestList = uc4;

            var uc5 = Globals.GetViewModel<IPatientPCLRequestEditImage>();
            uc5.CallByPCLRequestViewModel = Common.LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH;
            UCPatientPCLRequestEdit = uc5;
        }

        private void ActivateSubVM()
        {
            ActivateItem(UCDoctorProfileInfo);
            
            ActivateItem(UCPatientProfileInfo);

            ActivateItem(UCHeaderInfoPMR);

            ActivateItem(UCPatientPCLRequestList);
            
            ActivateItem(UCPatientPCLRequestEdit);
        }
        private void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCDoctorProfileInfo, close);

            DeactivateItem(UCPatientProfileInfo, close);

            DeactivateItem(UCHeaderInfoPMR, close);

            DeactivateItem(UCPatientPCLRequestList, close);

            DeactivateItem(UCPatientPCLRequestEdit, close);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            UCPatientPCLRequestEdit.V_RegistrationType = V_RegistrationType;
            ActivateSubVM();
            //▼===== #004
            //if (V_PCLMainCategory == (long)AllLookupValues.V_PCLMainCategory.GeneralSugery)
            //{
            //    ViewTitle = eHCMSResources.Z2779_G1_DichVuDTNgoaiTru.ToUpper();
            //    if (UCPatientPCLRequestEdit != null)
            //    {
            //        UCPatientPCLRequestEdit.V_PCLMainCategory = V_PCLMainCategory;
            //    }
            //}
            //▲===== #004
            if (IsShowSummaryContent && Registration_DataStorage.CurrentPatient != null)
            {
                InitPatientInfo();
            }
            //▼====== #001
            if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH_NT)
            {
                var homevm = Globals.GetViewModel<IHome>();
                IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                ostvm.WhichVM = SetOutStandingTask.PHIEU_YEU_CAU_CLS;
                homevm.OutstandingTaskContent = ostvm;
                homevm.IsExpandOST = true;
            }
            if (Globals.LeftModuleActive == LeftModuleActive.KHAMBENH_CLS_PHIEUYEUCAUHINHANH)
            {
                var homevm = Globals.GetViewModel<IHome>();
                homevm.OutstandingTaskContent = Globals.GetViewModel<IConsultationOutstandingTask>();
                homevm.IsExpandOST = true;
            }
            //▲====== #001
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            DeActivateSubVM(close);
            //▼====== #001
            var homevm = Globals.GetViewModel<IHome>();
            homevm.OutstandingTaskContent = null;
            homevm.IsExpandOST = false;
            //▲====== #001
        }
        public void InitPatientInfo()
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatient != null)
            {
                //▼===== #004
                if (UCPatientPCLRequestEdit == null)
                {
                    return;
                }
                if (Registration_DataStorage.CurrentPatientRegistration.V_RegForPatientOfType == AllLookupValues.V_RegForPatientOfType.DKBN_DT_NGOAI_TRU)
                {
                    UCPatientPCLRequestList.SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.GeneralSugery;
                    V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.GeneralSugery;
                    ViewTitle = eHCMSResources.Z2779_G1_DichVuDTNgoaiTru.ToUpper();
                }
                else
                {
                    UCPatientPCLRequestList.SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
                    V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
                    ViewTitle = eHCMSResources.P0382_G1_PhYeuCauHA.ToUpper();
                }
                UCPatientPCLRequestEdit.V_PCLMainCategory = V_PCLMainCategory;
                //▲===== #004
                UCPatientPCLRequestEdit.InitPatientInfo(Registration_DataStorage.CurrentPatient);
                UCPatientPCLRequestList.SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
                UCPatientPCLRequestList.SearchCriteria.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
                UCPatientPCLRequestList.SearchCriteria.LoaiDanhSach = AllLookupValues.PatientPCLRequestListType.DANHSACHPHIEUYEUCAU;

                UCPatientPCLRequestList.dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
                UCPatientPCLRequestList.dtgCellTemplateNgung_Visible = Visibility.Collapsed;
                UCPatientPCLRequestList.dtgCellTemplateKetQua_Visible = Visibility.Collapsed;
                UCPatientPCLRequestList.Init();
            }
        }

        public object UCPtRegDetailInfo
        {
            get;
            set;
        }

        public object UCDoctorProfileInfo
        {
            get;
            set;
        }

        public object UCPatientProfileInfo
        {
            get;
            set;
        }

        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR
        {
            get;
            set;
        }

        public object UCPatientPCLRequestNew
        {
            get;
            set;
        }
        public IListPCLRequest_Common UCPatientPCLRequestList
        {
            get;
            set;
        }
        public IPatientPCLRequestEditImage UCPatientPCLRequestEdit
        {
            get;
            set;
        }

        private int Index = 0;
        public void tabCommon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Index = ((TabControl)sender).SelectedIndex;
        }

        public TabControl tabCommon { get; set; }
        public void tabCommon_Loaded(object sender, RoutedEventArgs e)
        {
            tabCommon = (TabControl)sender;
            if (!mUCPatientPCLRequestNew)
                tabCommon.SelectedIndex = 1;
        }

        public object TabEdit
        {
            get;
            set;
        }
        public void TabEdit_Loaded(object sender, RoutedEventArgs e)
        {
            TabEdit = sender;
        }

        //Bắt sự kiện và load lại Danh sách phiếu yêu cầu cls
        public void Handle(ReLoadListPCLRequest message)
        {
            if (message != null)
            {
                SetValueListPCLRequest_Common();
            }
        }

        private void SetValueListPCLRequest_Common()
        {
            UCPatientPCLRequestList.SearchCriteria.PatientID = Registration_DataStorage.CurrentPatient.PatientID;
            UCPatientPCLRequestList.SearchCriteria.PtRegistrationID = Registration_DataStorage.CurrentPatientRegistration.PtRegistrationID;
            UCPatientPCLRequestList.SearchCriteria.LoaiDanhSach = AllLookupValues.PatientPCLRequestListType.DANHSACHPHIEUYEUCAU;
            UCPatientPCLRequestList.SearchCriteria.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;

            UCPatientPCLRequestList.dtgCellTemplateThucHien_Visible = Visibility.Collapsed;
            UCPatientPCLRequestList.dtgCellTemplateNgung_Visible = Visibility.Collapsed;
            UCPatientPCLRequestList.dtgCellTemplateKetQua_Visible = Visibility.Visible;

            UCPatientPCLRequestList.Init();
        }
        //Bắt sự kiện và load lại Danh sách phiếu yêu cầu cls

        public void Handle(DbClickSelectedObjectEventWithKeyForImage<PatientPCLRequest, string> message)
        {
            if (this.GetView() != null)
            {
                if (message != null && message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    ((TabItem)TabEdit).IsSelected = true;
                }
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mUCPatientPCLRequestNew = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtPCLRequest
                , (int)oConsultationEx.mPCL_TaoPhieuMoi_Them);
            mUCPatientPCLRequestList = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtPCLRequest
                , (int)oConsultationEx.mPCL_DSPhieuYeuCau_ThongTin);
            mUCPatientPCLRequestEdit = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtPCLRequest
                , (int)oConsultationEx.mPCL_XemSuaPhieuYeuCau_ThongTin);
        }

        #region account checking
        private bool _mUCPatientPCLRequestNew = true;
        private bool _mUCPatientPCLRequestList = true;
        private bool _mUCPatientPCLRequestEdit = true;

        public bool mUCPatientPCLRequestNew
        {
            get
            {
                return _mUCPatientPCLRequestNew;
            }
            set
            {
                if (_mUCPatientPCLRequestNew == value)
                    return;
                _mUCPatientPCLRequestNew = value;
                NotifyOfPropertyChange(() => mUCPatientPCLRequestNew);
            }
        }

        public bool mUCPatientPCLRequestList
        {
            get
            {
                return _mUCPatientPCLRequestList;
            }
            set
            {
                if (_mUCPatientPCLRequestList == value)
                    return;
                _mUCPatientPCLRequestList = value;
                NotifyOfPropertyChange(() => mUCPatientPCLRequestList);
            }
        }

        public bool mUCPatientPCLRequestEdit
        {
            get
            {
                return _mUCPatientPCLRequestEdit;
            }
            set
            {
                if (_mUCPatientPCLRequestEdit == value)
                    return;
                _mUCPatientPCLRequestEdit = value;
                NotifyOfPropertyChange(() => mUCPatientPCLRequestEdit);
            }
        }

        #endregion
        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
        public long V_RegistrationType
        {
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
            get
            {
                return _V_RegistrationType;
            }
        }


        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
                if (UCPatientPCLRequestEdit != null)
                    UCPatientPCLRequestEdit.IsShowSummaryContent = this.IsShowSummaryContent;
                if (UCPatientPCLRequestList!=null)
                    UCPatientPCLRequestList.IsShowSummaryContent = this.IsShowSummaryContent;
            }
        }

        public bool btSaveIsEnabled
        {
            get { return UCPatientPCLRequestEdit.btSaveIsEnabled && UCPatientPCLRequestEdit.CurrentPclRequest != null && UCPatientPCLRequestEdit.CurrentPclRequest.PatientPCLRequestIndicators != null && UCPatientPCLRequestEdit.CurrentPclRequest.PatientPCLRequestIndicators.Count > 0; }
        }

        //▼====== #001
        public void Handle (SetInPatientInfoAndRegistrationForPatientPCLRequestImage message)
        {
            if (message != null)
            {
                InitPatientInfo();
            }
        }
        //▲====== #001

        private long _V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Imaging;
        private string _ViewTitle = eHCMSResources.P0381_G1_PhYeuCauCLS;
        public long V_PCLMainCategory
        {
            get
            {
                return _V_PCLMainCategory;
            }
            set
            {
                if (_V_PCLMainCategory == value)
                {
                    return;
                }
                _V_PCLMainCategory = value;
                NotifyOfPropertyChange(() => V_PCLMainCategory);
            }
        }
        public string ViewTitle
        {
            get
            {
                return _ViewTitle;
            }
            set
            {
                if (_ViewTitle == value)
                {
                    return;
                }
                _ViewTitle = value;
                NotifyOfPropertyChange(() => ViewTitle);
            }
        }
        //▼===== #002
        public void Handle(DbClickSelectedObjectEventWithKeyToShowDetailsForImage<PatientPCLRequest, string> message)
        {
            if (this.GetView() != null)
            {
                if (message != null && message.ObjB == eHCMSResources.Z0055_G1_Edit)
                {
                    ((TabItem)TabEdit).IsSelected = true;
                }
            }
        }
        //▲===== #002
        private bool _IsShowCheckBoxPayAfter = true;
        public bool IsShowCheckBoxPayAfter
        {
            get { return _IsShowCheckBoxPayAfter; }
            set
            {
                if (_IsShowCheckBoxPayAfter != value)
                {
                    _IsShowCheckBoxPayAfter = value;
                    UCPatientPCLRequestEdit.IsShowCheckBoxPayAfter = _IsShowCheckBoxPayAfter;
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
                UCPatientPCLRequestEdit.Registration_DataStorage = Registration_DataStorage;
                UCPatientPCLRequestList.Registration_DataStorage = Registration_DataStorage;
                UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
            }
        }
        //▼===== #003
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
        public void CheckDeptLocation()
        {
            if (Registration_DataStorage != null && Registration_DataStorage.CurrentPatientRegistration != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails != null
                && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails.Count > 0 && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation != null
                && Globals.DeptLocation != null && Registration_DataStorage.CurrentPatientRegistration.AdmissionInfo.InPatientDeptDetails[0].DeptLocation.DeptID != Globals.DeptLocation.DeptID)
            {
                UCPatientPCLRequestEdit.FormIsEnabled = false;
                bShowContent = true;
            }
            else
            {
                UCPatientPCLRequestEdit.FormIsEnabled = true;
                bShowContent = false;
            }
        }
        //▲===== #003
    }
}