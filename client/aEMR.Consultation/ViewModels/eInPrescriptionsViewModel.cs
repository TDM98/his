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
using aEMR.Common;
using aEMR.Common.BaseModel;
using System.Collections.Generic;
/*
* 20181022 #001 TTM: BM0003214: Fix lỗi không hiển thị thông tin trên PatientInfo cho khám bệnh nội trú
* 20181121 #002 TTM: BM 0005257 Tạo mới Out standing task nội trú và sự kiện để load lại thông tin bệnh nhân từ out standing task.
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IeInPrescriptions)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class eInPrescriptionsViewModel : ViewModelBase, IeInPrescriptions
        , IHandle<ePrescriptionDoubleClickEvent_InPt_1>
        , IHandle<SelectListDrugDoubleClickEvent>
        , IHandle<SetInPatientInfoAndRegistrationForePresciption_InPt>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public eInPrescriptionsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            //▼====== #001
            CreateSubVM();
            //UCDoctorProfileInfo.isPreNoteTemp = true;
            ucePrescriptions.IsInPatient = true;
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
            //▲====== #001
        }
        //▼====== #001
        public void CreateSubVM()
        {
            //UCDoctorProfileInfo = Globals.GetViewModel<ILoginInfo>();
            //UCPatientProfileInfo = Globals.GetViewModel<IPatientInfo>();
            //UCHeaderInfoPMR = Globals.GetViewModel<IPatientMedicalRecords_ByPatientID>();
            ucePrescriptions = Globals.GetViewModel<IePrescriptionList>();
            //UCPtRegDetailInfo = Globals.GetViewModel<IPtRegDetailInfo>();
            UCePrescriptionListRoot = Globals.GetViewModel<IDrugListPatientUsed>();
            UCePrescription = Globals.GetViewModel<IeInPrescriptionOldNew>();
        }
        public void ActivateSubVM()
        {
            //ActivateItem(UCDoctorProfileInfo);
            //ActivateItem(UCPatientProfileInfo);
            //ActivateItem(UCHeaderInfoPMR);
            ActivateItem(ucePrescriptions);
            //ActivateItem(UCPtRegDetailInfo);
            ActivateItem(UCePrescriptionListRoot);
            ActivateItem(UCePrescription);
        }
        protected override void OnActivate()
        {
            ActivateSubVM();
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);

            //▼====== #002
            //var homevm = Globals.GetViewModel<IHome>();
            //IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
            //ostvm.WhichVM = SetOutStandingTask.RATOA;
            //homevm.OutstandingTaskContent = ostvm;
            //▲====== #002
        }
        private void DeActivateSubVM(bool close)
        {
            //DeactivateItem(UCDoctorProfileInfo, close);
            //DeactivateItem(UCPatientProfileInfo, close);
            //DeactivateItem(UCHeaderInfoPMR, close);
            DeactivateItem(ucePrescriptions, close);
            //DeactivateItem(UCPtRegDetailInfo, close);
            DeactivateItem(UCePrescriptionListRoot, close);
            DeactivateItem(UCePrescription, close);
        }
        protected override void OnDeactivate(bool close)
        {
            DeActivateSubVM(close);
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //▼====== #002
            //var homevm = Globals.GetViewModel<IHome>();
            //homevm.OutstandingTaskContent = null;
            //▲====== #002
        }
        //▲====== #001
        //public ILoginInfo UCDoctorProfileInfo { get; set; }
        //public IPatientInfo UCPatientProfileInfo { get; set; }
        //public IPatientMedicalRecords_ByPatientID UCHeaderInfoPMR { get; set; }

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
                return _mRaToa_DSToaThuocPhatHanh_ThongTin;
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
                return _mRaToa_TabDanhSachToaThuocGoc_Tim;
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

        public IeInPrescriptionOldNew UCePrescription
        {
            get;
            set;
        }
        public object ucePrescriptionEditor
        {
            get;
            set;
        }
        public IePrescriptionList ucePrescriptions
        {
            get;
            set;
        }
        public IDrugListPatientUsed UCePrescriptionListRoot
        {
            get;
            set;
        }

        public object UCePrescriptionTemplateDoctor
        {
            get;
            set;
        }

        public long ServiceRecID
        {
            get;
            set;
        }
        public long PtRegistrationID
        {
            get;
            set;
        }
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
        private bool _IsUpdateDiagConfirmInPT;
        public bool IsUpdateDiagConfirmInPT
        {
            get { return _IsUpdateDiagConfirmInPT; }
            set
            {
                if (_IsUpdateDiagConfirmInPT != value)
                {
                    _IsUpdateDiagConfirmInPT = value;
                    UCePrescription.IsUpdateDiagConfirmInPT = IsUpdateDiagConfirmInPT;
                    NotifyOfPropertyChange(() => IsUpdateDiagConfirmInPT);
                }
            }
        }

        //public IPtRegDetailInfo UCPtRegDetailInfo { get; set; }


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

        #region IHandle<ePrescriptionDoubleClickEvent_InPt_1> Members

        public void Handle(ePrescriptionDoubleClickEvent_InPt_1 message)
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

        //▼====== #002
        public void Handle(SetInPatientInfoAndRegistrationForePresciption_InPt message)
        {
            if (message != null)
            {
                UCePrescription.GetInitDataInfo();
                //UCPatientProfileInfo.InitData();
            }
        }
        //▲====== #002
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
                //UCPatientProfileInfo.Registration_DataStorage = Registration_DataStorage;
                UCePrescription.Registration_DataStorage = Registration_DataStorage;
                UCePrescriptionListRoot.Registration_DataStorage = Registration_DataStorage;
                //UCHeaderInfoPMR.Registration_DataStorage = Registration_DataStorage;
                ucePrescriptions.Registration_DataStorage = Registration_DataStorage;
                //UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
            }
        }
        public void ApplyDiagnosisTreatmentCollection(IList<DiagnosisTreatment> aDiagnosisTreatmentCollection)
        {
            UCePrescription.ApplyDiagnosisTreatmentCollection(aDiagnosisTreatmentCollection);
        }
        public void SetLastDiagnosisForConfirm()
        {
            UCePrescription.SetLastDiagnosisForConfirm();
        }
    }
}