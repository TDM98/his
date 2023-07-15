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
using System.Collections.ObjectModel;
using aEMR.Common.BaseModel;
/*
 * 20180920 #001 TBL:   Added IsPrescriptionChanged
 * 20191011 #002 TTM:   BM 0017421: Thêm thư ký y khoa cho ra toa
 */
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IePrescriptions)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ePrescriptionsViewModel : ViewModelBase, IePrescriptions
        , IHandle<ePrescriptionDoubleClickEvent>
        , IHandle<SelectListDrugDoubleClickEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        public void NotifyViewDataChanged()
        {
            if (UCePrescription != null)
            {
                UCePrescription.NotifyViewDataChanged();
            }
        }
        [ImportingConstructor]
        public ePrescriptionsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            System.Diagnostics.Debug.WriteLine("======> ePrescriptionsViewModel - Constructor");
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            Globals.EventAggregator.Subscribe(this);
            CreateSubVM();
            authorization();
        }

        ~ePrescriptionsViewModel()
        {
            System.Diagnostics.Debug.WriteLine("======> ePrescriptionsViewModel - Destructor");
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
                if (UCPatientProfileInfo != null)
                {
                    ((IPatientInfo)UCPatientProfileInfo).CS_DS = CS_DS;
                }
                ((IePrescriptionOldNew)UCePrescription).CS_DS = CS_DS;
            }
        }

        private void CreateSubVM()
        {
            UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo.isPreNoteTemp = true;
            UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            ucePrescriptions = Globals.GetViewModel<IePrescriptionList>();
            UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            UCePrescriptionListRoot = Globals.GetViewModel<IDrugListPatientUsed>();
            UCePrescriptionTemplateDoctor = Globals.GetViewModel<IePrescriptionTemplateDoctor>();
            UCePrescription = Globals.GetViewModel<IePrescriptionOldNew>();
            UCePrescription.hasTitle = false;
            UCePrescription.mToaThuocDaPhatHanh_ThongTin = mRaToa_tabToaThuocDaPhatHanh_ThongTinO;
            UCePrescription.mToaThuocDaPhatHanh_ChinhSua = mRaToa_tabToaThuocDaPhatHanh_ChinhSua;
            UCePrescription.mToaThuocDaPhatHanh_TaoToaMoi = mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi;
            UCePrescription.mToaThuocDaPhatHanh_PhatHanhLai = mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai;
            UCePrescription.mToaThuocDaPhatHanh_In = mRaToa_tabToaThuocDaPhatHanh_In;
            UCePrescription.mToaThuocDaPhatHanh_ChonChanDoan = mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan;
            if (Globals.ServerConfigSection.CommonItems.NumberOfCopiesPrescription <= 0)
            {
                UCePrescription.NumberOfTimesPrintVisibility = true;
            }
            else
            {
                UCePrescription.NumberOfTimesPrintVisibility = false;
            }
        }

        private void ActivateSubVM()
        {
            if (IsShowSummaryContent)
            {
                ActivateItem(UCHeaderInfoPMR);
                ActivateItem(UCPtRegDetailInfo);
                ActivateItem(UCPatientProfileInfo);
                ActivateItem(UCDoctorProfileInfo);
            }
            ActivateItem(ucePrescriptions);
            ActivateItem(UCePrescriptionListRoot);
            ActivateItem(UCePrescriptionTemplateDoctor);
            ActivateItem(UCePrescription);            
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (!IsShowSummaryContent)
            {
                UCHeaderInfoPMR = null;
                UCPtRegDetailInfo = null;
                UCPatientProfileInfo = null;
                UCDoctorProfileInfo = null;
            }
            ActivateSubVM();
        }


        public ILoginInfo UCDoctorProfileInfo
        {
            get;
            set;
        }

        public IPatientInfo UCPatientProfileInfo
        {
            get;
            set;
        }


        public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR
        {
            get;
            set;
        }

        public IePrescriptionList ucePrescriptions
        {
            get;
            set;
        }

        public IPtRegDetailInfo UCPtRegDetailInfo
        {
            get;
            set;
        }

        public IDrugListPatientUsed UCePrescriptionListRoot
        {
            get;
            set;
        }

        public IePrescriptionTemplateDoctor UCePrescriptionTemplateDoctor
        {
            get;
            set;
        }

        public IePrescriptionOldNew UCePrescription
        {
            get;
            set;
        }

        public TabControl tabCommon { get; set; }
        public void tabCommon_Loaded(object sender, RoutedEventArgs e)
        {
            tabCommon = (TabControl)sender;
            if (!mRaToa_TaoToaMoi)
                tabCommon.SelectedIndex = 1;
        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bUCePrescription = Globals.CheckOperation(Globals.listRefModule
            //    , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mToaThuocCuoi);
            //bucePrescriptionEditor = Globals.CheckOperation(Globals.listRefModule
            //    , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mToaThuocMoi);
            //bucePrescriptions = Globals.CheckOperation(Globals.listRefModule
            //    , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mDanhSachToaThuocCu);

            mRaToa_TaoToaMoi = Globals.CheckOperation(Globals.listRefModule
                , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mRaToa_TaoToaMoi);
            mRaToa_DSToaThuocPhatHanh_ThongTin = Globals.CheckOperation(Globals.listRefModule
                    , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mRaToa_DSToaThuocPhatHanh_ThongTin);
            mRaToa_tabToaThuocDaPhatHanh_ThongTin = Globals.CheckOperation(Globals.listRefModule
                    , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_ThongTin);
            mRaToa_TabDanhSachToaThuocGoc_Tim = Globals.CheckOperation(Globals.listRefModule
                    , (int)eModules.mConsultation, (int)eConsultation.mPtePrescriptionTab, (int)oConsultationEx.mRaToa_TabDanhSachToaThuocGoc_Tim);


            mRaToa_tabToaThuocDaPhatHanh_ThongTinO = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePrescriptionTab,
                                               (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_ThongTin, (int)ePermission.mView);
            mRaToa_tabToaThuocDaPhatHanh_ChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePrescriptionTab,
                                               (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_ChinhSua, (int)ePermission.mView);
            mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePrescriptionTab,
                                               (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi, (int)ePermission.mView);
            mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePrescriptionTab,
                                               (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai, (int)ePermission.mView);
            mRaToa_tabToaThuocDaPhatHanh_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePrescriptionTab,
                                               (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_In, (int)ePermission.mView);
            mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                               , (int)eConsultation.mPtePrescriptionTab,
                                               (int)oConsultationEx.mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan, (int)ePermission.mView);
        }

        #region account checking

        private bool _mRaToa_TaoToaMoi = true;
        private bool _mRaToa_DSToaThuocPhatHanh_ThongTin = true;
        private bool _mRaToa_tabToaThuocDaPhatHanh_ThongTin = true;
        private bool _mRaToa_TabDanhSachToaThuocGoc_Tim = true;

        private bool _mRaToa_tabToaThuocDaPhatHanh_ThongTinO = true;
        private bool _mRaToa_tabToaThuocDaPhatHanh_ChinhSua = true;
        private bool _mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi = true;
        private bool _mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai = true;
        private bool _mRaToa_tabToaThuocDaPhatHanh_In = true;
        private bool _mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan = true;


        public bool mRaToa_TaoToaMoi
        {
            get
            {
                return _mRaToa_TaoToaMoi;
            }
            set
            {
                if (_mRaToa_TaoToaMoi == value)
                    return;
                _mRaToa_TaoToaMoi = value;
            }
        }
        public bool mRaToa_DSToaThuocPhatHanh_ThongTin
        {
            get
            {
                return _mRaToa_DSToaThuocPhatHanh_ThongTin && !IsChildControl;
            }
            set
            {
                if (_mRaToa_DSToaThuocPhatHanh_ThongTin == value)
                    return;
                _mRaToa_DSToaThuocPhatHanh_ThongTin = value;
            }
        }
        public bool mRaToa_tabToaThuocDaPhatHanh_ThongTin
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_ThongTin;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_ThongTin == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_ThongTin = value;
            }
        }
        public bool mRaToa_TabDanhSachToaThuocGoc_Tim
        {
            get
            {
                return _mRaToa_TabDanhSachToaThuocGoc_Tim && !IsChildControl;
            }
            set
            {
                if (_mRaToa_TabDanhSachToaThuocGoc_Tim == value)
                    return;
                _mRaToa_TabDanhSachToaThuocGoc_Tim = value;
            }
        }

        public bool mRaToa_tabToaThuocDaPhatHanh_ThongTinO
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_ThongTinO;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_ThongTinO == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_ThongTinO = value;
                NotifyOfPropertyChange(() => mRaToa_tabToaThuocDaPhatHanh_ThongTinO);
            }
        }


        public bool mRaToa_tabToaThuocDaPhatHanh_ChinhSua
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_ChinhSua;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_ChinhSua == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_ChinhSua = value;
                NotifyOfPropertyChange(() => mRaToa_tabToaThuocDaPhatHanh_ChinhSua);
            }
        }


        public bool mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi = value;
                NotifyOfPropertyChange(() => mRaToa_tabToaThuocDaPhatHanh_TaoToaMoi);
            }
        }


        public bool mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai = value;
                NotifyOfPropertyChange(() => mRaToa_tabToaThuocDaPhatHanh_PhatHanhLai);
            }
        }


        public bool mRaToa_tabToaThuocDaPhatHanh_In
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_In;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_In == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_In = value;
                NotifyOfPropertyChange(() => mRaToa_tabToaThuocDaPhatHanh_In);
            }
        }


        public bool mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan
        {
            get
            {
                return _mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan;
            }
            set
            {
                if (_mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan == value)
                    return;
                _mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan = value;
                NotifyOfPropertyChange(() => mRaToa_tabToaThuocDaPhatHanh_ChonChanDoan);
            }
        }




        //private bool _bUCePrescription = true;
        //private bool _bucePrescriptionEditor = true;
        //private bool _bucePrescriptions = true;
        //public bool bUCePrescription
        //{
        //    get
        //    {
        //        return _bUCePrescription;
        //    }
        //    set
        //    {
        //        if (_bUCePrescription == value)
        //            return;
        //        _bUCePrescription = value;
        //    }
        //}
        //public bool bucePrescriptionEditor
        //{
        //    get
        //    {
        //        return _bucePrescriptionEditor;
        //    }
        //    set
        //    {
        //        if (_bucePrescriptionEditor == value)
        //            return;
        //        _bucePrescriptionEditor = value;
        //    }
        //}
        //public bool bucePrescriptions
        //{
        //    get
        //    {
        //        return _bucePrescriptions;
        //    }
        //    set
        //    {
        //        if (_bucePrescriptions == value)
        //            return;
        //        _bucePrescriptions = value;
        //    }
        //}



        #endregion

        public object ucePrescriptionEditor
        {
            get;
            set;
        }

        public long ServiceRecID
        {
            get;
            set;
        }
        //public long PtRegistrationID
        //{
        //    get;
        //    set;
        //}
        public string DiagnosisForDrug
        {
            get;
            set;
        }
        public bool IsChildWindow
        {
            get;
            set;
        }

        private int Index = 0;
        public void tabCommon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Index = ((TabControl)sender).SelectedIndex;
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

        #region IHandle<ePrescriptionDoubleClickEvent> Members

        public void Handle(ePrescriptionDoubleClickEvent message)
        {
            //if (message != null && this.IsActive) test01
            if (message != null)
            {
                ((TabItem)TabEdit).IsSelected = true;
            }
        }

        #endregion

        #region IHandle<SelectListDrugDoubleClickEvent> Members

        public void Handle(SelectListDrugDoubleClickEvent message)
        {
            //if (message != null && this.IsActive) test01
            if (message != null)
            {
                ((TabItem)TabEdit).IsSelected = true;
            }
        }

        #endregion
        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get
            {
                return _IsShowSummaryContent;
            }
            set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
                if (UCePrescription != null)
                {
                    UCePrescription.IsShowSummaryContent = IsShowSummaryContent;
                }
            }
        }
        public void InitPatientInfo()
        {
            UCePrescription.InitPatientInfo();
            ucePrescriptions.InitPatientInfo();
            UCePrescriptionListRoot.InitPatientInfo();
            UCePrescriptionTemplateDoctor.InitPatientInfo();
        }
        public void ChangeStatesAfterUpdated(bool IsUpdate = false)
        {
            UCePrescription.ChangeStatesAfterUpdated();
        }
        public Prescription ObjTaoThanhToaMoi
        {
            get { return UCePrescription.ObjTaoThanhToaMoi; }
            set { UCePrescription.ObjTaoThanhToaMoi = value; }
        }
        public ObservableCollection<PrescriptionIssueHistory> allPrescriptionIssueHistory
        {
            get { return UCePrescription.allPrescriptionIssueHistory; }
            set { UCePrescription.allPrescriptionIssueHistory = value; }
        }
        public void AddNewBlankDrugIntoPrescriptObjectNew()
        {
            UCePrescription.AddNewBlankDrugIntoPrescriptObjectNew();
        }
        public Prescription PrecriptionsBeforeUpdate
        {
            get { return UCePrescription.PrecriptionsBeforeUpdate; }
            set { UCePrescription.PrecriptionsBeforeUpdate = value; }
        }
        public void ChangeStatesBeforeUpdate()
        {
            UCePrescription.ChangeStatesBeforeUpdate();
        }
        public bool AllowUpdateThoughReturnDrugNotEnough
        {
            get { return UCePrescription.AllowUpdateThoughReturnDrugNotEnough; }
            set { UCePrescription.AllowUpdateThoughReturnDrugNotEnough = value; }
        }
        public bool CheckValidPrescription()
        {
            return UCePrescription.CheckValidPrescription() && UCePrescription.CheckValidPrescriptionWithDiagnosis();
        }
        public bool btnSaveAddNewIsEnabled
        {
            get
            {
                return UCePrescription.btnSaveAddNewIsEnabled;
            }
        }
        public bool btnUpdateIsEnabled
        {
            get
            {
                return UCePrescription.btnUpdateIsEnabled;
            }
        }
        /*▼====: #001*/
        public bool IsPrescriptionChanged
        {
            get
            {
                return UCePrescription.IsPrescriptionInfoChanged;
            }
        }
        /*▲====: #001*/
        public void AllowModifyPrescription()
        {
            UCePrescription.AllowModifyPrescription();
        }

        public void GetPhacDo()
        {
            UCePrescription.GetPhacDo();
        }

        public PhysicalExamination curPhysicalExamination
        {
            get
            {
                return UCePrescription.curPhysicalExamination;
            }
            set
            {
                UCePrescription.curPhysicalExamination = value;
            }
        }

        public void ResetPrescriptionInfoChanged()
        {
            UCePrescription.ResetPrescriptionInfoChanged();
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
                    UCePrescription.IsUpdateWithoutChangeDoctorIDAndDatetime = value;
                    NotifyOfPropertyChange(() => IsUpdateWithoutChangeDoctorIDAndDatetime);
                }
            }
        }
        public bool IsChildControl
        {
            get
            {
                return UCePrescription != null && UCePrescription.IsChildControl;
            }
            set
            {
                if (UCePrescription == null || UCePrescription.IsChildControl == value)
                {
                    return;
                }
                UCePrescription.IsChildControl = value;
                NotifyOfPropertyChange(() => IsChildControl);
                NotifyOfPropertyChange(() => mRaToa_DSToaThuocPhatHanh_ThongTin);
                NotifyOfPropertyChange(() => mRaToa_TabDanhSachToaThuocGoc_Tim);
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
                UCePrescription.Registration_DataStorage = Registration_DataStorage;
                UCePrescriptionListRoot.Registration_DataStorage = Registration_DataStorage;
                ucePrescriptions.Registration_DataStorage = Registration_DataStorage;
                UCePrescriptionTemplateDoctor.Registration_DataStorage = Registration_DataStorage;
                if (UCHeaderInfoPMR != null)
                {
                    UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
                }
                if (UCPtRegDetailInfo != null)
                {
                    UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
                }
            }
        }
        //▼===== #002: Truyền dữ liệu từ ConsultationSummary_V2ViewModel => ePrescriptionOldNewViewModel.
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
                    UCePrescription.Secretary = Secretary;
                    NotifyOfPropertyChange(() => Secretary);
                }
            }
        }
        //▲===== #002
        public long ServiceRecIDDiagTrmt
        {
            get
            {
                return UCePrescription.ServiceRecIDDiagTrmt;
            }
            set
            {
                UCePrescription.ServiceRecIDDiagTrmt = value;
            }
        }

        public long PtRegistrationID
        {
            get
            {
                return UCePrescription.PtRegistrationID;
            }
            set
            {
                UCePrescription.PtRegistrationID = value;
            }
        }
    }
}