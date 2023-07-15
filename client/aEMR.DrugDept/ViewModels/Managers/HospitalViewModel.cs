using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using aEMR.Common;
using eHCMSLanguage;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptHospitals)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class HospitalViewModel : Conductor<object>, IDrugDeptHospitals
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HospitalViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            
            HospitalPaging = new PagedSortableCollectionView<Hospital>();
            HospitalPaging.OnRefresh += HospitalPaging_OnRefresh;
            HospitalPaging.PageSize = Globals.PageSize;
            HospitalPaging.PageIndex = 0;
            SearchsHospital(HospitalPaging.PageIndex, HospitalPaging.PageSize);

            //GetRefDepartments((long)AllLookupValues.V_DeptType.Khoa);
            //GetRefStorageWarehouseTypes();

            Content = ADD;
            Contenttitle = ADDTITLE;
            NewStorage = new Hospital();
           
        }

        void HospitalPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchsHospital(HospitalPaging.PageIndex, HospitalPaging.PageSize);
        }
        private void InitNewStorage()
        {
            NewStorage = null;
            NewStorage = new Hospital();
            if (RefStorageWarehouseTypes != null)
            {
                NewStorage.HosID = RefStorageWarehouseTypes.FirstOrDefault().StoreTypeID;
            }
        }

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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }
        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }
        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }
        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }
        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }

        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bView);
        }
        #endregion

        private string ADD = eHCMSResources.G0156_G1_Them;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        private string ADDTITLE = eHCMSResources.G0292_G1_ThemMoiBV;
        private string UPDATETITLE = eHCMSResources.Z0094_G1_CNhatBV;

        private Visibility _Visibility = Visibility.Collapsed;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    _Visibility = value;
                    NotifyOfPropertyChange(() => Visibility);
                }
                if (Visibility == Visibility.Visible)
                {
                    NotExpanded = false;
                }
                else
                {
                    NotExpanded = true;
                }
            }
        }

        private bool _NotExpanded = true;
        public bool NotExpanded
        {
            get
            {
                return _NotExpanded;
            }
            set
            {
                if (_NotExpanded != value)
                {
                    _NotExpanded = value;
                    NotifyOfPropertyChange(() => NotExpanded);
                }
            }
        }

        private string _Content;
        public string Content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    _Content = value;
                    NotifyOfPropertyChange(() => Content);
                }
            }
        }

        private string _Contenttitle;
        public string Contenttitle
        {
            get
            {
                return _Contenttitle;
            }
            set
            {
                if (_Contenttitle != value)
                {
                    _Contenttitle = value;
                    NotifyOfPropertyChange(() => Contenttitle);
                }
            }
        }

        private PagedSortableCollectionView<Hospital> _HospitalPaging;
        public PagedSortableCollectionView<Hospital> HospitalPaging
        {
            get
            {
                return _HospitalPaging;
            }
            set
            {
                if (_HospitalPaging != value)
                {
                    _HospitalPaging = value;
                    NotifyOfPropertyChange(() => HospitalPaging);
                }
            }
        }

        private Hospital _NewStorage;
        public Hospital NewStorage
        {
            get { return _NewStorage; }
            set
            {
                if (_NewStorage != value)
                {
                    _NewStorage = value;
                    NotifyOfPropertyChange(() => NewStorage);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseType> _RefStorageWarehouseTypes;
        public ObservableCollection<RefStorageWarehouseType> RefStorageWarehouseTypes
        {
            get { return _RefStorageWarehouseTypes; }
            set
            {
                if (_RefStorageWarehouseTypes != value)
                {
                    _RefStorageWarehouseTypes = value;
                    NotifyOfPropertyChange(() => RefStorageWarehouseTypes);
                }
            }
        }

        private ObservableCollection<RefDepartment> _RefDepartments;
        public ObservableCollection<RefDepartment> RefDepartments
        {
            get { return _RefDepartments; }
            set
            {
                if (_RefDepartments != value)
                {
                    _RefDepartments = value;
                    NotifyOfPropertyChange(() => RefDepartments);
                }
            }
        }

        private string _HosName;
        public string HosName
        {
            get { return _HosName; }
            set
            {
                if (_HosName != value)
                {
                    _HosName = value;
                    NotifyOfPropertyChange(() => HosName);
                }
            }
        }

        //public void AddStorage(Hospital NewStorage)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //                           {
        //                               using (var serviceFactory = new PharmacyHospitalServiceClient())
        //                               {
        //                                   var contract = serviceFactory.ServiceInstance;

        //                                   contract.BeginAddNewStorage(NewStorage, Globals.DispatchCallback((asyncResult) =>
        //                                   {

        //                                       try
        //                                       {
        //                                          int results= contract.EndAddNewStorage(asyncResult);
        //                                          if (results == 0)
        //                                          {
        //                                              Globals.ShowMessage(eHCMSResources.Z0718_G1_KhoMoiDaThem,eHCMSResources.G0442_G1_TBao);
        //                                              Visibility = Visibility.Collapsed;
        //                                              InitNewStorage();
        //                                              Search();
        //                                          }
        //                                          else
        //                                          {
        //                                              Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0719_G1_KhoDaTonTai), eHCMSResources.G0442_G1_TBao);
        //                                          }
        //                                       }
        //                                       catch (Exception ex)
        //                                       {
        //                                           Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                                       }
        //                                       finally
        //                                       {
        //                                           Globals.IsBusy = false;
        //                                       }

        //                                   }), null);

        //                               }

        //                           });

        //    t.Start();
        //}

        //public void UpdateStorage(Hospital NewStorage)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyHospitalServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginUpdateStorage(NewStorage, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                   int results=contract.EndUpdateStorage(asyncResult);
        //                   if (results == 0)
        //                   {
        //                       Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
        //                       Visibility = Visibility.Collapsed;
        //                       Content = ADD;
        //                       Contenttitle = ADDTITLE;
        //                       InitNewStorage();
        //                       Search();
        //                   }
        //                   else
        //                   {
        //                       Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0720_G1_TenKhoDaCo), eHCMSResources.G0442_G1_TBao);
        //                   }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        //public void DeleteStorage(long StoreID)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyHospitalServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginDeleteStorageByID(StoreID, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    int results=contract.EndDeleteStorageByID(asyncResult);
        //                    if (results == 0)
        //                    {
        //                        Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
        //                        Search();
        //                    }
        //                    else
        //                    {
        //                        Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0721_G1_KhoKgTonTai), eHCMSResources.G0442_G1_TBao);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        private void SearchsHospital(int PageIndex, int PageSize)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            int totalCount = 0;
            IsLoading = false;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchHospitals(HosName, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchHospitals(out totalCount, asyncResult);
                            HospitalPaging.Clear();
                            HospitalPaging.TotalItemCount = totalCount;
                            if (results != null && results.Count > 0)
                            {
                                foreach (Hospital p in results)
                                {
                                    HospitalPaging.Add(p);
                                }

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


        public void txt_search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HospitalPaging.PageIndex = 0;
                HosName = (sender as TextBox).Text;
                SearchsHospital(0, HospitalPaging.PageSize);
            }
        }

        public void Search()
        {
            HospitalPaging.PageIndex = 0;
            SearchsHospital(0, HospitalPaging.PageSize);
        }

        DataGrid GridHospital = null;
        public void GridHospital_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GridHospital = (sender as DataGrid);
        }

       
        public void GridHospital_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private bool CheckValid(object temp)
        {
            Hospital u = temp as Hospital;
            if (u == null)
            {
                return false;
            }
            return u.Validate();
        }

        public void btnCancel()
        {
            Content = ADD;
            Contenttitle = ADDTITLE;
            InitNewStorage();
            Visibility = Visibility.Collapsed;
        }
      
        public void btnAddOrUpdate()
        {
            //if (CheckValid(NewStorage))
            //{
            //    if (Content == ADD)
            //    {
            //        AddStorage(NewStorage);
            //    }
            //    else
            //    {
            //        UpdateStorage(NewStorage);
            //    }
            //}
        }
        public void hlbAdd()
        {
            Visibility = Visibility.Visible;
        }

        public void ViewClick(object sender, RoutedEventArgs e)
        {
            if (GridHospital != null && GridHospital.SelectedItem != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
                NewStorage = (GridHospital.SelectedItem as Hospital).DeepCopy();
            }
        }

        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show("Bạn có muốn xóa Kho này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    if (GridHospital.SelectedItem != null)
            //        DeleteStorage(((PharmaceuticalCompany)GridHospital.SelectedItem).PCOID);
            //}
        }

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
        public void GridHospital_DblClick(object sender, Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi
            if (IsChildWindow)
            {
                TryClose();
                Globals.EventAggregator.Publish(new DrugDeptCloseSearchHospitalEvent { SelectedHospital = e.Value });
            }
        }
    }
}
