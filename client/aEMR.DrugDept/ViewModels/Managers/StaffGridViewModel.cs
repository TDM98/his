using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Image = System.Windows.Controls.Image;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptStaffs)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaffGridViewModel : Conductor<object>, IDrugDeptStaffs
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StaffGridViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

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

        }

        void _allStaff_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            GetAllRefStaffCategories();
            GetAllProvinces();
            GetAllCountries();
            GetAllEthnics();
            GetAllDepartments();
            GetAllReligion();
            GetAllMaritalStatus();
            _allStaff = new PagedSortableCollectionView<Staff>();
            _allStaff.OnRefresh+=new EventHandler<RefreshEventArgs>(_allStaff_OnRefresh);
            GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
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
        public void butNew( )
        {
            //var StaffVM = Globals.GetViewModel<IStaff>();
            //var instance = StaffVM as Conductor<object>;
            //this.ActivateItem(StaffVM);
            //Globals.ShowDialog(instance,(o)=>{});

            Action<IStaff> onInitDlg = (StaffVM) =>
            {
                this.ActivateItem(StaffVM);
            };
            GlobalsNAV.ShowDialog<IStaff>(onInitDlg);

        }
        public void lnkUpdateClick(object sender,RoutedEventArgs e)
        {
            //var StaffVM = Globals.GetViewModel<IStaffEdit>();
            //StaffVM.curStaff = ObjectCopier.DeepCopy(SelectedStaff);
            //var instance = StaffVM as Conductor<object>;

            //this.ActivateItem(StaffVM);
            //Globals.ShowDialog(instance, (o) => { });

            Action<IStaffEdit> onInitDlg = (StaffVM) =>
            {
                StaffVM.curStaff = ObjectCopier.DeepCopy(SelectedStaff);
                this.ActivateItem(StaffVM);
            };
            GlobalsNAV.ShowDialog<IStaffEdit>(onInitDlg);
        }
        public void lnkDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0164_G1_Msg_ConfXoaSth, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteStaff((int)SelectedStaff.StaffID);
            }
        }
       #endregion

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        public void Handle(allStaffChangeEvent obj)
        {
            if(obj!=null)
            {
                allStaff.PageIndex = 0;
                GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
            }

        }

        #region method
        private string ALL = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0913_G1_None);
        private void GetAllRefStaffCategories()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllRefStaffCategories(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllRefStaffCategories(asyncResult);
                            allRefStaffCategory = results.ToObservableCollection();
                            RefStaffCategory item = new RefStaffCategory();
                            item.StaffCatgID = -1;
                            item.StaffCatgDescription = ALL;
                            allRefStaffCategory.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        private void GetAllProvinces()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllProvinces(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllProvinces(asyncResult);
                            allCitiesProvince = results.ToObservableCollection();
                            CitiesProvince item = new CitiesProvince();
                            item.CityProvinceID = -1;
                            item.CityProvinceName = ALL;
                            allCitiesProvince.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        
        private void GetAllCountries()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllCountries(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllCountries(asyncResult);
                            allRefCountry = results.ToObservableCollection();
                            RefCountry item = new RefCountry();
                            item.CountryID = -1;
                            item.CountryName = ALL;
                            allRefCountry.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllEthnics()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllEthnics(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllEthnics(asyncResult);
                            allEthnics = results.ToObservableCollection();
                            Lookup item = new Lookup();
                            item.LookupID = -1;
                            item.ObjectValue = ALL;
                            allEthnics.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllDepartments()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllDepartments(false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllDepartments(asyncResult);
                            allRefDepartment = results.ToObservableCollection();
                            RefDepartment item = new RefDepartment();
                            item.DeptID = -1;
                            item.DeptName = ALL;
                            allRefDepartment.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        
        private void GetAllReligion()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllReligion(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllReligion(asyncResult);
                            allReligion = results.ToObservableCollection();
                            Lookup item = new Lookup();
                            item.LookupID = -1;
                            item.ObjectValue = ALL;
                            allReligion.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllMaritalStatus()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllMaritalStatus(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllMaritalStatus(asyncResult);
                            allMaritalStatus = results.ToObservableCollection();
                            Lookup item = new Lookup();
                            item.LookupID = -1;
                            item.ObjectValue = ALL;
                            allMaritalStatus.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllStaffAllPaging(Staff searchStaff, int PageSize, int PageIndex, string OrderBy,
                                                         bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
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
                                     IsLoading = false;
                                     Globals.IsBusy = false;
                                 }

                             }), null);

                }

            });

            t.Start();
        }
        private void DeleteStaff(int StaffID)
        {
            
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new UserAccountsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteStaff(StaffID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndDeleteStaff(asyncResult);
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0537_G1_XoaOk), eHCMSResources.T0432_G1_Error);
                                GetAllStaffAllPaging(curStaff, allStaff.PageSize, allStaff.PageIndex, "", true);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

     #endregion

        private bool _IsChildWindow = false;
        public bool IsChildWindow
        {
            get
            {
                return _IsChildWindow;
            }
            set
            {
                if (_IsChildWindow != value)
                {
                    _IsChildWindow = value;
                    NotifyOfPropertyChange(() => IsChildWindow);
                }
            }
        }

        public void GridStaffs_DblClick(object sender, Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi
            if (IsChildWindow)
            {
                TryClose();
                Globals.EventAggregator.Publish(new DrugDeptCloseSearchStaffEvent { SelectedStaff = e.Value });
            }
        }
    }
}
