using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;

/*
 Trang nay rac khong Bang da sua ok
 * 20181001 #001 TBL: BM 0000106. Fix PatientInfo không hiển thị thông tin
 */
namespace aEMR.PCLDepartment.ViewModels
{
    [Export(typeof(ILaboratoryResultHome)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class LaboratoryResultHomeViewModel : ViewModelBase, ILaboratoryResultHome
        , IHandle<DbClickPatientPCLRequest<PatientPCLRequest, String>>
        , IHandle<DbClickSelectedObjectEvent<PatientPCLRequest>>

    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public LaboratoryResultHomeViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            /*▼====: #001*/
            CreateSubVM();
            //FormLoad();
            /*▲====: #001*/
        }

        private void FormLoad()
        {
            vmSearchPCLRequest = Globals.GetViewModel<IPCLDepartmentSearchPCLRequest>();
            vmSearchPCLRequest.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            //UCPCLDepartmentSearchPCLRequest = ucSearchPCLRequest;
            //ActivateItem(ucSearchPCLRequest);

            var uc1 = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo = uc1;
            //ActivateItem(uc1);

            var uc2 = Globals.GetViewModel<IPatientInfo>();
            UCPatientProfileInfo = uc2;
            //ActivateItem(uc2);

            var uc3 = Globals.GetViewModel<ILaboratoryResultAddNew>();
            UCLaboratoryResultAddNew = uc3;
            //ActivateItem(uc3);
        }
        /*▼====: #001*/
        public void CreateSubVM()
        {
            vmSearchPCLRequest = Globals.GetViewModel<IPCLDepartmentSearchPCLRequest>();
            vmSearchPCLRequest.V_PCLMainCategory = (long)AllLookupValues.V_PCLMainCategory.Laboratory;
            var uc1 = Globals.GetViewModel<ILoginInfo>();
            UCDoctorProfileInfo = uc1;
            var uc2 = Globals.GetViewModel<IPatientInfo>();
            uc2.IsShowPCL_V2 = Visibility.Visible;
            UCPatientProfileInfo = uc2;
            var uc3 = Globals.GetViewModel<ILaboratoryResultAddNew>();
            UCLaboratoryResultAddNew = uc3;
        }
        public void ActivateSubVM()
        {
            ActivateItem(UCPatientProfileInfo);
            ActivateItem(UCLaboratoryResultAddNew);
        }
        public void DeActivateSubVM(bool close)
        {
            DeactivateItem(UCPatientProfileInfo, close);
        }
        protected override void OnActivate()
        {
            ActivateSubVM();
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }
        protected override void OnDeactivate(bool close)
        {
            DeActivateSubVM(close);
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }
        /*▲====: #001*/
        private IPCLDepartmentSearchPCLRequest _vmSearchPCLRequest;
        public IPCLDepartmentSearchPCLRequest vmSearchPCLRequest
        {
            get
            {
                return _vmSearchPCLRequest;
            }
            set
            {
                _vmSearchPCLRequest = value;
                NotifyOfPropertyChange(() => vmSearchPCLRequest);
            }
        }

        public object UCDoctorProfileInfo { get; set; }
        public object UCPatientProfileInfo { get; set; }

        public object UCLaboratoryResultAddNew { get; set; }

        public object UCLaboratoryResultList { get; set; }

        public TabItem TabLaboratoryResultAddNew{get;set;}
        public void TabLaboratoryResultAddNew_Loaded(object sender,RoutedEventArgs e)
        {
            TabLaboratoryResultAddNew = sender as TabItem;
        }
        
        #region properties
        private PatientPCLRequest _ObjPatientPCLRequest_SearchPaging_Selected;
        private ObservableCollection<PatientPCLLaboratoryResultDetail> _allPatientPCLLaboratoryResultDetail;

        private PatientPCLLaboratoryResultDetail _curPatientPCLLaboratoryResultDetail;

        public PatientPCLRequest ObjPatientPCLRequest_SearchPaging_Selected
        {
            get { return _ObjPatientPCLRequest_SearchPaging_Selected; }
            set
            {
                _ObjPatientPCLRequest_SearchPaging_Selected = value;
                NotifyOfPropertyChange(() => ObjPatientPCLRequest_SearchPaging_Selected);
            }
        }

        public PatientPCLLaboratoryResultDetail curPatientPCLLaboratoryResultDetail
        {
            get { return _curPatientPCLLaboratoryResultDetail; }
            set
            {
                if (_curPatientPCLLaboratoryResultDetail == value)
                    return;
                _curPatientPCLLaboratoryResultDetail = value;
                NotifyOfPropertyChange(() => curPatientPCLLaboratoryResultDetail);
            }
        }

        public ObservableCollection<PatientPCLLaboratoryResultDetail> allPatientPCLLaboratoryResultDetail
        {
            get { return _allPatientPCLLaboratoryResultDetail; }
            set
            {
                if (_allPatientPCLLaboratoryResultDetail == value)
                    return;
                _allPatientPCLLaboratoryResultDetail = value;
                NotifyOfPropertyChange(() => allPatientPCLLaboratoryResultDetail);
            }
        }

        #endregion

        //public override void DeactivateItem(object item, bool close)
        //{
        //    base.DeactivateItem(item, close);
        //    //Globals.EventAggregator.Unsubscribe(this);
        //}

        public void Handle(DbClickPatientPCLRequest<PatientPCLRequest, string> message)
        {
            //if (message != null)
            //{
            //    if (message.ObjB == eHCMSResources.Z0055_G1_Edit)
            //    {
            //        TabLaboratoryResultAddNew.IsSelected = true;
            //    }
            //}
        }


        public void Handle(DbClickSelectedObjectEvent<PatientPCLRequest> message)
        {
            //if (message != null)
            //{
            //    TabLaboratoryResultAddNew.IsSelected = true;
            //}
        }
    }
}