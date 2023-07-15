//using eHCMSLanguage;
//using System;
//using System.ComponentModel.Composition;
//using System.Threading;
//using aEMR.Infrastructure;
//using aEMR.Infrastructure.Events;
//using aEMR.ServiceClient;
//using aEMR.ViewContracts;
//using aEMR.Infrastructure.CachingUtils;
//using Castle.Windsor;
//using Castle.Core.Logging;
//using Caliburn.Micro;
//using DataEntities;
//using aEMR.Common.Collections;

//namespace aEMR.ConsultantEPrescription.ViewModels
//{
//    [Export(typeof(IePrescriptionListRoot)), PartCreationPolicy(CreationPolicy.NonShared)]
//    public class ePrescriptionListRootViewModel : Conductor<object>, IePrescriptionListRoot
//        , IHandle<ReloadDataePrescriptionEvent>
//        , IHandle<ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail>>
//    {
//        private readonly INavigationService _navigationService;
//        private readonly ISalePosCaching _salePosCaching;
//        private readonly ILogger _logger;

//        private bool _isLoading;
//        public bool IsLoading
//        {
//            get { return _isLoading; }
//            set
//            {
//                if (_isLoading != value)
//                {
//                    _isLoading = value;
//                    NotifyOfPropertyChange(() => IsLoading);
//                }
//            }
//        }

//        [ImportingConstructor]
//        public ePrescriptionListRootViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
//        {
//            _navigationService = navigationService;
//            _logger = container.Resolve<ILogger>();
//            _salePosCaching = salePosCaching;

//            Globals.EventAggregator.Subscribe(this);

//            authorization();
//            initPatientInfo();
//        }

//        public void Handle(ShowPatientInfo_KHAMBENH_RATOA<Patient, PatientRegistration, PatientRegistrationDetail> message)
//        {
//            initPatientInfo();
//        }

//        public void initPatientInfo()
//        {
//            if (Globals.PatientAllDetails.PatientInfo != null)
//            {
//                Globals.EventAggregator.Subscribe(this);

//                SearchCriteria = new PrescriptionSearchCriteria();
//                SearchCriteria.PatientID = Globals.PatientAllDetails.PatientInfo.PatientID;
//                SearchCriteria.OrderBy = "";

//                ObjPrescriptions_ListRootByPatientID_Paging =
//                    new PagedSortableCollectionView<DataEntities.Prescription>();
//                ObjPrescriptions_ListRootByPatientID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjPrescriptions_ListRootByPatientID_Paging_OnRefresh);

//                Prescriptions_ListRootByPatientID_Paging(0, ObjPrescriptions_ListRootByPatientID_Paging.PageSize, true);
//            }
//        }

//        void ObjPrescriptions_ListRootByPatientID_Paging_OnRefresh(object sender, RefreshEventArgs e)
//        {
//            Prescriptions_ListRootByPatientID_Paging(ObjPrescriptions_ListRootByPatientID_Paging.PageIndex, ObjPrescriptions_ListRootByPatientID_Paging.PageSize, false);
//        }

//        #region Properties Member
//        private PrescriptionSearchCriteria _SearchCriteria;
//        public PrescriptionSearchCriteria SearchCriteria
//        {
//            get
//            {
//                return _SearchCriteria;
//            }
//            set
//            {
//                _SearchCriteria = value;
//                NotifyOfPropertyChange(() => SearchCriteria);

//            }
//        }

//        private PagedSortableCollectionView<Prescription> _ObjPrescriptions_ListRootByPatientID_Paging;
//        public PagedSortableCollectionView<Prescription> ObjPrescriptions_ListRootByPatientID_Paging
//        {
//            get
//            {
//                return _ObjPrescriptions_ListRootByPatientID_Paging;
//            }
//            set
//            {
//                if (_ObjPrescriptions_ListRootByPatientID_Paging != value)
//                {
//                    _ObjPrescriptions_ListRootByPatientID_Paging = value;
//                    NotifyOfPropertyChange(() => ObjPrescriptions_ListRootByPatientID_Paging);
//                }
//            }
//        }
       
//        #endregion

//        private void Prescriptions_ListRootByPatientID_Paging(int PageIndex, int PageSize, bool CountTotal)
//        {
//            ObjPrescriptions_ListRootByPatientID_Paging.Clear();

//            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
//            int TotalCount = 0;
//            var t = new Thread(() =>
//            {
//                IsLoading=true;

//                using (var serviceFactory = new ePrescriptionsServiceClient())
//                {
//                    var contract = serviceFactory.ServiceInstance;

//                    contract.BeginPrescriptions_ListRootByPatientID_Paging(SearchCriteria,PageIndex, PageSize,"",CountTotal, Globals.DispatchCallback((asyncResult) =>
//                    {
//                        try
//                        {
//                            var results = contract.EndPrescriptions_ListRootByPatientID_Paging(out TotalCount, asyncResult);
                            
//                            ObjPrescriptions_ListRootByPatientID_Paging.TotalItemCount = TotalCount;

//                            if (results != null)
//                            {
//                                foreach (Prescription p in results)
//                                {
//                                    ObjPrescriptions_ListRootByPatientID_Paging.Add(p);
//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
//                        }
//                        finally
//                        {
//                            //Globals.IsBusy = false;
//                            IsLoading=false;    
//                        }

//                    }), null);

//                }

//            });

//            t.Start();
//        }

//        public void btFind()
//        {
//            ObjPrescriptions_ListRootByPatientID_Paging.PageIndex = 0;
//            Prescriptions_ListRootByPatientID_Paging(0, ObjPrescriptions_ListRootByPatientID_Paging.PageSize, true);
//        }

//        public void Handle(ReloadDataePrescriptionEvent message)
//        {
//            if(message!=null)
//            {
//                ObjPrescriptions_ListRootByPatientID_Paging.PageIndex = 0;
//                Prescriptions_ListRootByPatientID_Paging(0, ObjPrescriptions_ListRootByPatientID_Paging.PageSize, true);
//            }
//        }

//        public void authorization()
//        {
//            if (!Globals.isAccountCheck)
//            {
//                return;
//            }
            
//            mRaToa_TabDanhSachToaThuocGoc_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConsultation
//                                              , (int)eConsultation.mPtePrescriptionTab,
//                                              (int)oConsultationEx.mRaToa_TabDanhSachToaThuocGoc_Tim, (int)ePermission.mView);
           
//        }
//        #region account checking

//        private bool _mRaToa_TabDanhSachToaThuocGoc_Tim  = true;
//        public bool mRaToa_TabDanhSachToaThuocGoc_Tim
//        {
//            get
//            {
//                return _mRaToa_TabDanhSachToaThuocGoc_Tim;
//            }
//            set
//            {
//                if (_mRaToa_TabDanhSachToaThuocGoc_Tim == value)
//                    return;
//                _mRaToa_TabDanhSachToaThuocGoc_Tim = value;
//            }
//        }
//        #endregion
//    }
//}