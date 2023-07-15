using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

/*
 * #001 20180922 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.UserAccountManagement.ViewModels
{
    [Export(typeof(IStaffGrid)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffGridViewModel : Conductor<object>, IStaffGrid,IHandle<allStaffChangeEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StaffGridViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            curStaff =new Staff();
            GetAllRefStaffCategories();
            GetAllProvinces();
            GetAllCountries();
            GetAllEthnics();
            GetAllDepartments();
            GetAllReligion();
            GetAllMaritalStatus();
            _allStaff = new PagedSortableCollectionView<Staff>();
            _allStaff.PageIndex = 0;
            _allStaff.OnRefresh += new EventHandler<RefreshEventArgs>(_allStaff_OnRefresh);
            GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
        }

        void _allStaff_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            //==== 20161206 CMN Begin: Disable method called in onloaded
            //GetAllRefStaffCategories();
            //GetAllProvinces();
            //GetAllCountries();
            //GetAllEthnics();
            //GetAllDepartments();
            //GetAllReligion();
            //GetAllMaritalStatus();
            //_allStaff = new PagedSortableCollectionView<Staff>();
            //_allStaff.OnRefresh+=new EventHandler<RefreshEventArgs>(_allStaff_OnRefresh);
            //GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
            //==== 20161206 CMN End.
            Globals.EventAggregator.Subscribe(this);
        }
        
        #region properties
        public object PerImage { get; set; }
        private ObservableCollection<RefStaffCategory> _allRefStaffCategory;
        public ObservableCollection<RefStaffCategory> allRefStaffCategory
        {
            get
            {
                return _allRefStaffCategory;
            }
            set
            {
                if (_allRefStaffCategory == value)
                    return;
                _allRefStaffCategory = value;
                NotifyOfPropertyChange(() => allRefStaffCategory);
            }
        }

        private RefStaffCategory _SelectedRefStaffCategory;
        public RefStaffCategory SelectedRefStaffCategory
        {
            get
            {
                return _SelectedRefStaffCategory;
            }
            set
            {
                if (_SelectedRefStaffCategory == value)
                    return;
                _SelectedRefStaffCategory = value;
                NotifyOfPropertyChange(() => SelectedRefStaffCategory);
                if (SelectedRefStaffCategory!=null)
                {
                    curStaff.StaffCatgID = SelectedRefStaffCategory.StaffCatgID;
                }
            }
        }

        private PagedSortableCollectionView<Staff> _allStaff;
        public PagedSortableCollectionView<Staff> allStaff
        {
            get
            {
                return _allStaff;
            }
            set
            {
                if (_allStaff == value)
                    return;
                _allStaff = value;
                NotifyOfPropertyChange(() => allStaff);
            }
        }

        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get
            {
                return _SelectedStaff;
            }
            set
            {
                if (_SelectedStaff == value)
                    return;
                _SelectedStaff = value;
            }
        }

        private Staff _curStaff;
        public Staff curStaff
        {
            get
            {
                return _curStaff;
            }
            set
            {
                if (_curStaff == value)
                    return;
                _curStaff = value;
                NotifyOfPropertyChange(()=>curStaff);
            }
        }

        private ObservableCollection<CitiesProvince> _allCitiesProvince;
        public ObservableCollection<CitiesProvince> allCitiesProvince
        {
            get
            {
                return _allCitiesProvince;
            }
            set
            {
                if (_allCitiesProvince == value)
                    return;
                _allCitiesProvince = value;
                NotifyOfPropertyChange(() => allCitiesProvince);
            }
        }

        private CitiesProvince _SelectedCitiesProvince;
        public CitiesProvince SelectedCitiesProvince
        {
            get
            {
                return _SelectedCitiesProvince;
            }
            set
            {
                if (_SelectedCitiesProvince == value)
                    return;
                _SelectedCitiesProvince = value;
                NotifyOfPropertyChange(() => SelectedCitiesProvince);
                if (SelectedCitiesProvince != null)
                {
                    curStaff.CityProvinceID = SelectedCitiesProvince.CityProvinceID;
                }
            }
        }

        private ObservableCollection<RefCountry> _allRefCountry;
        public ObservableCollection<RefCountry> allRefCountry
        {
          get
          {
            return _allRefCountry;
          }
          set
          {
            if (_allRefCountry == value)
              return;
            _allRefCountry = value;
              NotifyOfPropertyChange(()=>allRefCountry);
          }
        }

        private RefCountry _SelectedRefCountry;
        public RefCountry SelectedRefCountry
        {
            get
            {
                return _SelectedRefCountry;
            }
            set
            {
                if (_SelectedRefCountry == value)
                    return;
                _SelectedRefCountry = value;
                NotifyOfPropertyChange(()=>SelectedRefCountry);
                if (SelectedRefCountry != null)
                {
                    curStaff.CountryID = SelectedRefCountry.CountryID;
                }
            }
        }

        private ObservableCollection<Lookup> _allEthnics;
        public ObservableCollection<Lookup> allEthnics
        {
            get
            {
                return _allEthnics;
            }
            set
            {
                if (_allEthnics == value)
                    return;
                _allEthnics = value;
                NotifyOfPropertyChange(() => allEthnics);
            }
        }

        private Lookup _SelectedEthnics;
        public Lookup SelectedEthnics
        {
            get
            {
                return _SelectedEthnics;
            }
            set
            {
                if (_SelectedEthnics == value)
                    return;
                _SelectedEthnics = value;
                NotifyOfPropertyChange(() => SelectedEthnics);
                if (SelectedEthnics != null)
                {
                    curStaff.V_Ethnic = SelectedEthnics.LookupID;
                }
            }
        }

        private ObservableCollection<RefDepartment> _allRefDepartment;
        public ObservableCollection<RefDepartment> allRefDepartment
        {
            get
            {
                return _allRefDepartment;
            }
            set
            {
                if (_allRefDepartment == value)
                    return;
                _allRefDepartment = value;
                NotifyOfPropertyChange(() => allRefDepartment);
            }
        }

        private RefDepartment _SelectedDepartment;
        public RefDepartment SelectedDepartment
        {
            get
            {
                return _SelectedDepartment;
            }
            set
            {
                if (_SelectedDepartment == value)
                    return;
                _SelectedDepartment = value;
                NotifyOfPropertyChange(() => SelectedDepartment);
                if (SelectedDepartment != null)
                {
                    curStaff.DeptID = SelectedDepartment.DeptID;
                }
            }
        }

        private ObservableCollection<Lookup> _allReligion;
        public ObservableCollection<Lookup> allReligion
        {
            get
            {
                return _allReligion;
            }
            set
            {
                if (_allReligion == value)
                    return;
                _allReligion = value;
                NotifyOfPropertyChange(() => allReligion);
            }
        }

        private Lookup _SelectedReligion;
        public Lookup SelectedReligion
        {
            get
            {
                return _SelectedReligion;
            }
            set
            {
                if (_SelectedReligion == value)
                    return;
                _SelectedReligion = value;
                NotifyOfPropertyChange(() => SelectedReligion);
                if (SelectedReligion != null)
                {
                    curStaff.V_Religion = SelectedReligion.LookupID;
                }
            }
        }

        private ObservableCollection<Lookup> _allMaritalStatus;
        public ObservableCollection<Lookup> allMaritalStatus
        {
            get
            {
                return _allMaritalStatus;
            }
            set
            {
                if (_allMaritalStatus == value)
                    return;
                _allMaritalStatus = value;
                NotifyOfPropertyChange(() => allMaritalStatus);
            }
        }

        private Lookup _SelectedMaritalStatus;
        public Lookup SelectedMaritalStatus
        {
            get
            {
                return _SelectedMaritalStatus;
            }
            set
            {
                if (_SelectedMaritalStatus == value)
                    return;
                _SelectedMaritalStatus = value;
                NotifyOfPropertyChange(() => SelectedMaritalStatus);
                if (SelectedMaritalStatus != null)
                {
                    curStaff.V_MaritalStatus = SelectedMaritalStatus    .LookupID;
                }
            }
        }

        public void butClear()
        {
            curStaff=new Staff();
            SelectedCitiesProvince = null;
            SelectedDepartment = null;
            SelectedEthnics = null;
            SelectedMaritalStatus = null;
            SelectedRefCountry = null;
            SelectedRefStaffCategory = null;
            SelectedReligion = null;
            NotifyOfPropertyChange(() => SelectedCitiesProvince);
            NotifyOfPropertyChange(() => SelectedDepartment);
            NotifyOfPropertyChange(() => SelectedEthnics);
            NotifyOfPropertyChange(() => SelectedMaritalStatus);
            NotifyOfPropertyChange(() => SelectedRefCountry);
            NotifyOfPropertyChange(() => SelectedRefStaffCategory);
            NotifyOfPropertyChange(() => SelectedReligion);
        }
        public void butSearch()
        {
            GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
        }
        /*▼====: #001*/
        public void butNew( )
        {
            void onInitDlg(IStaff StaffVM)
            {
                StaffVM.IsAddNew = true;
                StaffVM.ModifiedPasswordDT = true;
                ActivateItem(StaffVM);
            }
            GlobalsNAV.ShowDialog<IStaff>(onInitDlg);
        }

        public void lnkUpdateClick(object sender,RoutedEventArgs e)
        {
            Action<IStaff> onInitDlg = (StaffVM) =>
            {
                StaffVM.IsAddNew = false;
                //StaffVM.TitleForm = eHCMSResources.K1671_G1_CNhatNhVien;
                StaffVM.curStaff = ObjectCopier.DeepCopy(SelectedStaff);
                this.ActivateItem(StaffVM);
            };
            GlobalsNAV.ShowDialog<IStaff>(onInitDlg);
        }
        /*▲====: #001*/

        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn xoá không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteStaff((int)SelectedStaff.StaffID);
            }
        }
        #endregion

        public void Handle(allStaffChangeEvent obj)
        {
            if(obj!=null)
            {
                allStaff.PageIndex = 0;
                GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
            }
        }

        #region method
        /*▼====: #001*/
        private void GetAllRefStaffCategories()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllRefStaffCategories(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllRefStaffCategories(asyncResult);
                                if (results != null)
                                {
                                    allRefStaffCategory = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefStaffCategory);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllProvinces()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllProvinces(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllProvinces(asyncResult);
                                if (results != null)
                                {
                                    allCitiesProvince = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allCitiesProvince);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        
        private void GetAllCountries()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllCountries(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllCountries(asyncResult);
                                if (results != null)
                                {
                                    allRefCountry = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefCountry);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllEthnics()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllEthnics(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllEthnics(asyncResult);
                                if (results != null)
                                {
                                    allEthnics = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allEthnics);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllDepartments()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllDepartments(false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllDepartments(asyncResult);
                                if (results != null)
                                {
                                    allRefDepartment = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allRefDepartment);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        
        private void GetAllReligion()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllReligion(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllReligion(asyncResult);
                                if (results != null)
                                {
                                    allReligion = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allReligion);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllMaritalStatus()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMaritalStatus(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllMaritalStatus(asyncResult);
                                if (results != null)
                                {
                                    allMaritalStatus = results.ToObservableCollection();
                                    NotifyOfPropertyChange(() => allMaritalStatus);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy, bool CountTotal)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffAllPaging(searchStaff, PageSize, PageIndex, OrderBy,
                                                             CountTotal, Globals.DispatchCallback((asyncResult) =>
                                 {
                                     try
                                     {
                                         int Total = 0;
                                         var results = contract.EndGetAllStaffAllPaging(out Total, asyncResult);
                                         if (results != null)
                                         {
                                             if (allStaff == null)
                                             {
                                                 allStaff = new PagedSortableCollectionView<Staff>();
                                             }
                                             else
                                             {
                                                 allStaff.Clear();
                                             }
                                             foreach (var p in results)
                                             {
                                                 allStaff.Add(p);
                                             }
                                             allStaff.TotalItemCount = Total;
                                             NotifyOfPropertyChange(() => allStaff);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void DeleteStaff(int StaffID)
        {            
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new UserAccountsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteStaff(StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteStaff(asyncResult);
                                Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.T0432_G1_Error);
                                GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        /*▲====: #001*/
        #endregion
    }
}
