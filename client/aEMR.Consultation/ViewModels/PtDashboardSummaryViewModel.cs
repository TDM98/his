using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using DataEntities;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using aEMR.Common;
/*
 * 20181121 #001 TTM: BM 0005257: Chỉnh sửa Out standing task ngoại trú, tạo mới Out standing task nội trú. 
*/
namespace aEMR.ConsultantEPrescription.ViewModels
{
    [Export(typeof(IPtDashboardSummary)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PtDashboardSummaryViewModel : ViewModelBase, IPtDashboardSummary
    {
        #region Events
        [ImportingConstructor]
        public PtDashboardSummaryViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            authorization();
            //var uc1 = Globals.GetViewModel<IPatientInfo>();
            //UCPatientProfileInfo = uc1;
            //this.ActivateItem(uc1);

            var uc2 = Globals.GetViewModel<ISummary>();
            UCSummary = uc2;
            this.ActivateItem(uc2);

            var uc3 = Globals.GetViewModel<IPtRegDetailInfo>();
            UCPtRegDetailInfo = uc3;
            this.ActivateItem(uc3);

            //searchPatientAndRegVm = Globals.GetViewModel<ISearchPatientAndRegistration>();
            ////searchPatientAndRegVm.mTimBN = mTimBN;
            ////searchPatientAndRegVm.mTimDangKy = mTimDangKy;

            //if (Globals.PatientFindBy_ForConsultation == null)
            //{
            //    Globals.PatientFindBy_ForConsultation = AllLookupValues.PatientFindBy.NGOAITRU;
            //}
            //searchPatientAndRegVm.PatientFindBy = Globals.PatientFindBy_ForConsultation.Value;
            //searchPatientAndRegVm.CloseRegistrationFormWhenCompleteSelection = false;
            //if (mTimBN)
            //{
            //    searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_PATIENT_BTN);
            //    searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_PATIENT);
            //}
            //else
            //{
            //    searchPatientAndRegVm.InitButtonVisibility(SearchRegButtonsVisibility.SHOW_SEARCH_REG_BTN);
            //    searchPatientAndRegVm.SetDefaultButton(SearchRegistrationButtons.SEARCH_REGISTRATION);
            //}
            //searchPatientAndRegVm.IsSearchGoToKhamBenh = true;
            //searchPatientAndRegVm.PatientFindByVisibility = true;
            //searchPatientAndRegVm.CanSearhRegAllDept = true;
            //searchPatientAndRegVm.SearchAdmittedInPtRegOnly = true;
            ////20181018 TNHX: [BM0002186] turn on block find by name
            //if (!Globals.ServerConfigSection.ConsultationElements.IsAllowSearchingPtByName)
            //{
            //    searchPatientAndRegVm.IsAllowSearchingPtByName_Visible = true;
            //    searchPatientAndRegVm.IsSearchPtByNameChecked = false;
            //}
            //SearchRegistrationContent = searchPatientAndRegVm;
            //ActivateItem(searchPatientAndRegVm);
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            //▼====== #001
            if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                var homevm = Globals.GetViewModel<IHome>();
                homevm.OutstandingTaskContent = Globals.GetViewModel<IConsultationOutstandingTask>();
                homevm.IsExpandOST = true;
                //searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NGOAITRU;
            }
            else if (V_RegistrationType == (long)AllLookupValues.RegistrationType.NOI_TRU)
            {
                //var homevm = Globals.GetViewModel<IHome>();
                //IInPatientOutstandingTask ostvm = Globals.GetViewModel<IInPatientOutstandingTask>();
                //ostvm.WhichVM = SetOutStandingTask.KHAMBENH;
                //homevm.OutstandingTaskContent = ostvm;
                //searchPatientAndRegVm.PatientFindBy = AllLookupValues.PatientFindBy.NOITRU;
            }
            //▲====== #001
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
            //▼====== #001
            //var homevm = Globals.GetViewModel<IHome>();
            //homevm.OutstandingTaskContent = null;
            //▲====== #001
        }
        #endregion
        #region UserControl Properties
        //public object UCPatientProfileInfo { get; set; }
        public ISummary UCSummary
        { get; set; }
        //public object SearchRegistrationContent { get; set; }
        public IPtRegDetailInfo UCPtRegDetailInfo
        { get; set; }
        //public ISearchPatientAndRegistration searchPatientAndRegVm { get; set; }
        #endregion
        #region Properties
        private long _V_RegistrationType = (long)AllLookupValues.RegistrationType.NOI_TRU;
        public long V_RegistrationType
        {
            get
            {
                return _V_RegistrationType;
            }
            set
            {
                if (_V_RegistrationType == value)
                {
                    return;
                }
                _V_RegistrationType = value;
                NotifyOfPropertyChange(() => V_RegistrationType);
            }
        }
        #endregion
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mTimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                              , (int)eConsultation.mPtDashboardSummary,
                                              (int)oConsultationEx.mThongTinChung_TimBN, (int)ePermission.mView);
            mTimDangKy = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
                                              , (int)eConsultation.mPtDashboardSummary,
                                              (int)oConsultationEx.mThongTinChung_TimDK, (int)ePermission.mView);
        }
        #region checking account
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
        #endregion
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
                UCSummary.Registration_DataStorage = Registration_DataStorage;
                UCPtRegDetailInfo.Registration_DataStorage = Registration_DataStorage;
            }
        }
    }
}