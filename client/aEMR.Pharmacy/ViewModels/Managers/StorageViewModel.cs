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
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IStorage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StorageViewModel : ViewModelBase, IStorage
    {
        public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StorageViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();
            StorageWareHousesPaging = new PagedSortableCollectionView<RefStorageWarehouseLocation>();
            StorageWareHousesPaging.OnRefresh += StorageWareHousesPaging_OnRefresh;
            StorageWareHousesPaging.PageSize = Globals.PageSize;
            StorageWareHousesPaging.PageIndex = 0;
            SearchsStorages();

            GetRefDepartments((long)AllLookupValues.V_DeptType.Khoa);
            GetRefStorageWarehouseTypes();

            Content = ADD;
            Contenttitle = ADDTITLE;
            NewStorage = new RefStorageWarehouseLocation();
           
        }

        void StorageWareHousesPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchsStorages();
        }
        private void InitNewStorage()
        {
            NewStorage = null;
            NewStorage = new RefStorageWarehouseLocation();
            if (RefStorageWarehouseTypes != null)
            {
                NewStorage.StoreTypeID = RefStorageWarehouseTypes.FirstOrDefault().StoreTypeID;
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
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyKho,
                                               (int)oPharmacyEx.mQuanLyKho_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyKho,
                                               (int)oPharmacyEx.mQuanLyKho_Them, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mQuanLyKho,
                                               (int)oPharmacyEx.mQuanLyKho_ChinhSua, (int)ePermission.mView);
            

        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
            }
        }
        
        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }
        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bChinhSua);
        }
        #endregion

        private string ADD = eHCMSResources.G0156_G1_Them;
        private string UPDATE = eHCMSResources.K1599_G1_CNhat;

        private string ADDTITLE = string.Format("{0} {1}", eHCMSResources.G0276_G1_ThemMoi, eHCMSResources.T2144_G1_Kho);
        private string UPDATETITLE = string.Format("{0} {1}", eHCMSResources.K1599_G1_CNhat, eHCMSResources.T2144_G1_Kho);

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

        private PagedSortableCollectionView<RefStorageWarehouseLocation> _StorageWareHousesPaging;
        public PagedSortableCollectionView<RefStorageWarehouseLocation> StorageWareHousesPaging
        {
            get
            {
                return _StorageWareHousesPaging;
            }
            set
            {
                if (_StorageWareHousesPaging != value)
                {
                    _StorageWareHousesPaging = value;
                    NotifyOfPropertyChange(() => StorageWareHousesPaging);
                }
            }
        }

        private RefStorageWarehouseLocation _NewStorage;
        public RefStorageWarehouseLocation NewStorage
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

        private string _StorageName;
        public string StorageName
        {
            get { return _StorageName; }
            set
            {
                if (_StorageName != value)
                {
                    _StorageName = value;
                    NotifyOfPropertyChange(() => StorageName);
                }
            }
        }

        public void AddStorage(RefStorageWarehouseLocation NewStorage)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginAddNewStorage(NewStorage, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int results = contract.EndAddNewStorage(asyncResult);
                                if (results == 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0718_G1_KhoMoiDaThem, eHCMSResources.G0442_G1_TBao);
                                    Visibility = Visibility.Collapsed;
                                    InitNewStorage();
                                    Search();
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0719_G1_KhoDaTonTai, eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                IsLoading = false;
                                //Globals.IsBusy = false;
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                }
            });

            t.Start();
        }

        public void UpdateStorage(RefStorageWarehouseLocation NewStorage)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginUpdateStorage(NewStorage, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndUpdateStorage(asyncResult);
                                if (results == 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
                                    Visibility = Visibility.Collapsed;
                                    Content = ADD;
                                    Contenttitle = ADDTITLE;
                                    InitNewStorage();
                                    Search();
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0720_G1_TenKhoDaCo, eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.DlgHideBusyIndicator();
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                IsLoading = false;
                                //Globals.IsBusy = false;
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                    this.DlgHideBusyIndicator();
                }

            });

            t.Start();
        }

        public void DeleteStorage(long StoreID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeleteStorageByID(StoreID, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int results = contract.EndDeleteStorageByID(asyncResult);
                                if (results == 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.K0537_G1_XoaOk, eHCMSResources.G0442_G1_TBao);
                                    Search();
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0721_G1_KhoKgTonTai, eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                IsLoading = false;
                                //Globals.IsBusy = false;
                                this.DlgHideBusyIndicator();
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                    this.DlgHideBusyIndicator();
                }

            });

            t.Start();
        }

        private void SearchsStorages()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            int totalCount = 0;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchStorage(StorageName, 0, Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                var results = contract.EndSearchStorage(out totalCount, asyncResult);
                                StorageWareHousesPaging.Clear();
                                StorageWareHousesPaging.TotalItemCount = totalCount;
                                if (results != null && results.Count > 0)
                                {
                                    foreach (RefStorageWarehouseLocation p in results)
                                    {
                                        StorageWareHousesPaging.Add(p);
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Info(ex.Message);
                            }
                            finally
                            {
                                IsLoading = false;
                                //Globals.IsBusy = false;
                                this.DlgHideBusyIndicator();
                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Info(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetRefDepartments (long DeptType)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyStoragesServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefDepartment_ByDeptType(DeptType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefDepartment_ByDeptType(asyncResult);
                            RefDepartments = results.ToObservableCollection();
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

        private void GetRefStorageWarehouseTypes()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyStoragesServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetRefStorageWarehouseType_All(Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetRefStorageWarehouseType_All(asyncResult);
                            RefStorageWarehouseTypes = results.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                           // Globals.IsBusy = false;
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
                StorageName = (sender as TextBox).Text;
                Search();
            }
        }

        public void Search()
        {
            StorageWareHousesPaging.PageIndex = 0;
            SearchsStorages();
        }

        DataGrid GridStorages = null;
        public void GridStorages_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GridStorages = (sender as DataGrid);
        }

       
        public void GridStorages_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private bool CheckValid(object temp)
        {
            RefStorageWarehouseLocation u = temp as RefStorageWarehouseLocation;
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
            if (CheckValid(NewStorage))
            {
                if (Content == ADD)
                {
                    AddStorage(NewStorage);
                }
                else
                {
                    UpdateStorage(NewStorage);
                }
            }
        }
        public void hlbAdd()
        {
            Visibility = Visibility.Visible;
        }

        public void ViewClick(object sender, RoutedEventArgs e)
        {
            if (GridStorages != null && GridStorages.SelectedItem != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
                NewStorage = (GridStorages.SelectedItem as RefStorageWarehouseLocation).DeepCopy();
            }
        }

        public void DoubleClick(object sender, Common.EventArgs<object> e)
        {
            NewStorage = (e.Value as RefStorageWarehouseLocation).DeepCopy();
            if (NewStorage != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
               
            }
        }
        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0162_G1_Msg_ConfXoaKho, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridStorages.SelectedItem != null)
                    DeleteStorage(((PharmaceuticalCompany)GridStorages.SelectedItem).PCOID);
            }
        }

    }
}
