using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;

namespace aEMR.ConsultantEPrescription.CommonRecs.ViewModels
{
    [Export(typeof(ICommonRecs)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommonRecsViewModel : ViewModelBase, ICommonRecs
        , IHandle<ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail>>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CommonRecsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            System.Diagnostics.Debug.WriteLine("=====> CommonRecsViewModel - Constructor");

            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);

            authorization();
            CreateSubVM();
        }

        ~CommonRecsViewModel()
        {
            System.Diagnostics.Debug.WriteLine("=====> CommonRecsViewModel - Destructor");
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
                ((IPatientInfo)UCPatientPInfo).CS_DS = CS_DS;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
                        
            ActivateSubVM();
        }

        public void CreateSubVM()
        {
            UCPatientPInfo = Globals.GetViewModel<IPatientInfo>();
                        
            //PhysicalExamination = Globals.GetViewModel<IPhysicalExamination>();
            
            VitalSigns = Globals.GetViewModel<IVitalSigns>();
            
            Immunizations = Globals.GetViewModel<IImmunizations>();
            
            MedicalConditions = Globals.GetViewModel<IMedicalConditions>();
            
            MedicalHistory = Globals.GetViewModel<IMedicalHistory>();
            
            HosHistory = Globals.GetViewModel<IHosHistory>();
            
            FamilyHistory = Globals.GetViewModel<IFamilyHistory>();            
        }

        public void ActivateSubVM()
        {
            ActivateItem(UCPatientPInfo);
            
            //ActivateItem(PhysicalExamination);
            
            ActivateItem(VitalSigns);
            
            ActivateItem(Immunizations);
            
            ActivateItem(MedicalConditions);
            
            ActivateItem(MedicalHistory);
            
            ActivateItem(HosHistory);
            
            ActivateItem(FamilyHistory);            
        }

        public void tabCommon_Loaded(object sender, RoutedEventArgs e)
        {
            tabCommon = sender as TabControl;
            if (((TabControl)sender).SelectedItem != null)
                ((TabItem)((TabControl)sender).SelectedItem).Focus();
        }
        public TabControl tabCommon
        {
            get;
            set;
        }

        public object UCPatientPInfo
        {
            get;
            set;
        }
        //public IPhysicalExamination PhysicalExamination
        //{
        //    get;
        //    set;
        //}
        public IVitalSigns VitalSigns
        {
            get;
            set;
        }

        public IImmunizations Immunizations { get; set; }
        public IMedicalConditions MedicalConditions { get; set; }
        public IMedicalHistory MedicalHistory { get; set; }
        public IHosHistory HosHistory { get; set; }
        public IFamilyHistory FamilyHistory { get; set; }
        
        private object _mainContent;
        public object mainContent
        {
            get
            {
                return _mainContent;
            }
            set
            {
                if (_mainContent == value)
                    return;
                _mainContent = value;
                NotifyOfPropertyChange(() => mainContent);
            }
        }
        
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            
            mTongQuat_XemThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_XemThongTin, (int)ePermission.mView);
            mTongQuat_ChinhSuaThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                                   , (int)eConsultation.mPtDashboardCommonRecs,
                                                   (int)oConsultationEx.mTongQuat_ChinhSuaThongTin, (int)ePermission.mView);
        }

        #region account checking

        private bool _mTongQuat_XemThongTin = true;
        private bool _mTongQuat_ChinhSuaThongTin = true && Globals.isConsultationStateEdit;

        public bool mTongQuat_XemThongTin
        {
            get
            {
                return _mTongQuat_XemThongTin;
            }
            set
            {
                if (_mTongQuat_XemThongTin == value)
                    return;
                _mTongQuat_XemThongTin = value;
            }
        }
        public bool mTongQuat_ChinhSuaThongTin
        {
            get
            {
                return _mTongQuat_ChinhSuaThongTin;
            }
            set
            {
                if (_mTongQuat_ChinhSuaThongTin == value)
                    return;
                _mTongQuat_ChinhSuaThongTin = value && Globals.isConsultationStateEdit;
            }
        }
        #endregion

        public void Handle(ShowPatientInfo_KHAMBENH_TONGQUAT<Patient, PatientRegistration, PatientRegistrationDetail> message)
        {
            //InitPatientInfo();
        }

        private bool _IsShowSummaryContent = true;
        public bool IsShowSummaryContent
        {
            get => _IsShowSummaryContent; set
            {
                _IsShowSummaryContent = value;
                NotifyOfPropertyChange(() => IsShowSummaryContent);
                //if (PhysicalExamination != null)
                //    PhysicalExamination.IsShowSummaryContent = IsShowSummaryContent;
            }
        }
        public void InitPatientInfo()
        {
            //PhysicalExamination.InitPatientInfo();
            Immunizations.InitPatientInfo();
            MedicalConditions.InitPatientInfo();
            MedicalHistory.InitPatientInfo();
            HosHistory.InitPatientInfo();
            FamilyHistory.InitPatientInfo();
        }
        public void GetRefImmunization(long MedServiceID)
        {
            Immunizations.GetRefImmunization(MedServiceID);
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
                FamilyHistory.Registration_DataStorage = Registration_DataStorage;
                VitalSigns.Registration_DataStorage = Registration_DataStorage;
                HosHistory.Registration_DataStorage = Registration_DataStorage;
                //PhysicalExamination.Registration_DataStorage = Registration_DataStorage;
                Immunizations.Registration_DataStorage = Registration_DataStorage;
                MedicalConditions.Registration_DataStorage = Registration_DataStorage;
                MedicalHistory.Registration_DataStorage = Registration_DataStorage;
            }
        }
    }
}