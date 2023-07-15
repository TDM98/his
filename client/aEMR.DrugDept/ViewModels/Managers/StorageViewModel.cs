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
using System.Collections.Generic;
using aEMR.Common.BaseModel;
/*
 * 20210922 #001 QTD: Filter Storage
 * 20221110 #002 QTD: Lọc bỏ kho ký gửi khi xuất nội bộ khoa Dược
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptStorage)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StorageViewModel : ViewModelBase, IDrugDeptStorage
    {
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

            GetRefDepartments((long)AllLookupValues.V_DeptType.Khoa);
            GetRefStorageWarehouseTypes();
            LoadV_MedProductTypeList();
            Content = ADD;
            Contenttitle = ADDTITLE;
            NewStorage = new RefStorageWarehouseLocation();

        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
            SearchsStorages(IsChildWindow);
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
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

        void StorageWareHousesPaging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchsStorages(IsChildWindow);
        }
        private void InitNewStorage()
        {
            NewStorage = null;
            NewStorage = new RefStorageWarehouseLocation();
            if (RefStorageWarehouseTypes != null)
            {
                NewStorage.StoreTypeID = RefStorageWarehouseTypes.FirstOrDefault().StoreTypeID;
            }
            LoadV_MedProductTypeList();
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

        private ObservableCollection<Lookup> _V_MedProductTypeList;
        public ObservableCollection<Lookup> V_MedProductTypeList
        {
            get { return _V_MedProductTypeList; }
            set
            {
                if (_V_MedProductTypeList != value)
                {
                    _V_MedProductTypeList = value;
                    NotifyOfPropertyChange(() => V_MedProductTypeList);
                }
            }
        }

        public void LoadV_MedProductTypeList()
        {
            this.DlgShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        try
                        {
                            contract.BeginGetAllLookupValuesByType(LookupValues.V_MedProductType,
                                    Globals.DispatchCallback(asyncResult =>
                                    {
                                        try
                                        {
                                            var allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                            //V_MedProductTypeList = allItems != null ? new ObservableCollection<Lookup>(allItems) : null;
                                            V_MedProductTypeList = allItems.ToObservableCollection();
                                        }
                                        catch (Exception ex1)
                                        {
                                            ClientLoggerHelper.LogInfo(ex1.ToString());
                                        }
                                        finally
                                        {
                                            this.HideBusyIndicator();
                                        }
                                    }), null);
                        }
                        catch (Exception exc)
                        {
                            Globals.ShowMessage(exc.Message, eHCMSResources.T0432_G1_Error);
                            this.DlgHideBusyIndicator();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
                finally
                {
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void AddStorage(RefStorageWarehouseLocation NewStorage)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
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
                                                       Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0719_G1_KhoDaTonTai), eHCMSResources.G0442_G1_TBao);
                                                   }
                                               }
                                               catch (Exception ex)
                                               {
                                                   Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                               }
                                               finally
                                               {
                                                   this.DlgHideBusyIndicator();
                                               }

                                           }), null);

                                       }

                                   });

            t.Start();
        }

        public void UpdateStorage(RefStorageWarehouseLocation NewStorage)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
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
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0720_G1_TenKhoDaCo), eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void DeleteStorage(long StoreID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
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
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0721_G1_KhoKgTonTai), eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SearchsStorages(bool IsChildWindow)
        {
            if (!IsChildWindow)
            {
                V_MedProductType = 0;
            }
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyStoragesServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchStorage(StorageName, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndSearchStorage(out totalCount, asyncResult);
                            StorageWareHousesPaging.Clear();
                            if (results != null && results.Count > 0)
                            {
                                foreach (RefStorageWarehouseLocation p in results)
                                {
                                    if(!Globals.ServerConfigSection.MedDeptElements.IsEnableFilterStorage)
                                    {
                                        if (IsSubStorage
                                            && ((V_MedProductType == (long)AllLookupValues.MedProductType.THUOC && !p.IsMedicineStore)
                                            || (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU && !p.IsUtilStore) || !p.IsSubStorage)
                                            || (V_GroupTypes != 0 && (V_GroupTypes != p.V_GroupTypes || p.V_GroupTypes == 0))) //--28/12/2020 DatTB Thêm điều kiện lọc loại kho GTGT
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {   //▼====: #001
                                        if (IsSubStorage
                                        && ((V_MedProductType == (long)AllLookupValues.MedProductType.THUOC && !p.IsMedicineStore)
                                            || (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU && !p.IsUtilStore)) //|| !p.IsSubStorage)
                                        || (V_GroupTypes != 0 && (V_GroupTypes != p.V_GroupTypes || p.V_GroupTypes == 0))
                                        || ((IsSubStorage && p.IsSubStorage == IsSubStorage || IsMainStorage && (p.IsMain == IsMainStorage || p.IsSubStorage == IsSubStorage))
                                            && (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC || V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU))
                                        || StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_DRUGDEPT && p.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL
                                        || StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_EXTERNAL && p.StoreTypeID != (long)AllLookupValues.StoreType.STORAGE_EXTERNAL
                                        || (IsMainStorage && p.IsMain == IsMainStorage && (V_MedProductType != (long)AllLookupValues.MedProductType.THUOC || V_MedProductType != (long)AllLookupValues.MedProductType.Y_CU))
                                        //▼====: #002
                                        || (IsSubStorage && p.IsConsignment))
                                         //▲====: #002
                                        //▲====: #001
                                        {
                                            continue;
                                        }
                                    }
                                    StorageWareHousesPaging.Add(p);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRefDepartments(long DeptType)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetRefStorageWarehouseTypes()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                            this.DlgHideBusyIndicator();
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
                StorageWareHousesPaging.PageIndex = 0;
                SearchsStorages(IsChildWindow);
            }
        }

        public void Search()
        {
            StorageWareHousesPaging.PageIndex = 0;
            SearchsStorages(IsChildWindow);
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
            NewStorage.ListV_MedProductType = String.Join(";", from item in V_MedProductTypeList where item.IsChecked select item.LookupID);
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
            if (chkKT != null)
            {
                chkKT.IsEnabled = true;
            }

            if (chkStoFast != null)
            {
                chkStoFast.IsEnabled = true;
            }

            IsCheckV_GroupType = false;
        }
        string[] arrayV_MedProductType;
        public void ViewClick(object sender, RoutedEventArgs e)
        {
            arrayV_MedProductType = null;
            if (GridStorages != null && GridStorages.SelectedItem != null)
            {
                Content = UPDATE;
                Contenttitle = UPDATETITLE;

                Visibility = Visibility.Visible;
                NewStorage = (GridStorages.SelectedItem as RefStorageWarehouseLocation).DeepCopy();
                if (NewStorage.ListV_MedProductType != null)
                {
                    arrayV_MedProductType = NewStorage.ListV_MedProductType.Split(';');
                }
                if (V_MedProductTypeList != null)
                {
                    foreach (var item in V_MedProductTypeList)
                    {
                        item.IsChecked = false;
                        if (arrayV_MedProductType != null)
                        {
                            foreach (var array in arrayV_MedProductType)
                            {
                                if ((item.LookupID).ToString() == array)
                                {
                                    item.IsChecked = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (chkKT != null)
                {
                    chkKT.IsEnabled = false;
                }

                IsCheckV_GroupType = NewStorage.V_GroupTypes == (long)AllLookupValues.V_GroupTypes.TINH_GTGT;
                //if (chkStoFast != null)
                //{
                //    chkStoFast.IsEnabled = false;
                //}
            }
        }

        public void DeletedClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.T2144_G1_Kho), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (GridStorages.SelectedItem != null)
                    DeleteStorage(((RefStorageWarehouseLocation)GridStorages.SelectedItem).StoreID);
            }
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

        public void GridStorages_DblClick(object sender, Common.EventArgs<object> e)
        {
            //neu la childwindow thi phat ra su kien ben duoi
            if (IsChildWindow)
            {
                TryClose();
                Globals.EventAggregator.Publish(new DrugDeptCloseSearchStorageEvent { SelectedStorage = e.Value });
            }
        }

        private bool _IsSubStorage = false;
        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        public bool IsSubStorage
        {
            get => _IsSubStorage; set
            {
                _IsSubStorage = value;
                NotifyOfPropertyChange(() => IsSubStorage);
            }
        }
        public long V_MedProductType
        {
            get => _V_MedProductType; set
            {
                _V_MedProductType = value;
                NotifyOfPropertyChange(() => V_MedProductType);
            }
        }

        CheckBox chkKT = null;

        public void chkKT_Loaded(object sender, RoutedEventArgs e)
        {
            chkKT = sender as CheckBox;
        }

        //--▼-- 28/12/2020 DatTB Thêm biến loại kho GTGT
        private long _V_GroupTypes;
        public long V_GroupTypes
        {
            get => _V_GroupTypes; set
            {
                _V_GroupTypes = value;
                NotifyOfPropertyChange(() => V_GroupTypes);
            }
        }
        //--▲-- 28/12/2020 DatTB
      
        private bool _IsMainStorage = false;
        public bool IsMainStorage
        {
            get => _IsMainStorage; set
            {
                _IsMainStorage = value;
                NotifyOfPropertyChange(() => IsMainStorage);
            }
        }

        private long _StoreTypeID;
        public long StoreTypeID
        {
            get => _StoreTypeID; set
            {
                _StoreTypeID = value;
                NotifyOfPropertyChange(() => StoreTypeID);
            }
        }

        CheckBox chkStoFast = null;

        public void chkStoFast_Loaded(object sender, RoutedEventArgs e)
        {
            chkStoFast = sender as CheckBox;
        }

        CheckBox chkV_GroupType = null;

        public void chkV_GroupType_Loaded(object sender, RoutedEventArgs e)
        {
            chkV_GroupType = sender as CheckBox;
        }

        private bool _IsCheckV_GroupType = false;
        public bool IsCheckV_GroupType
        {
            get
            {
                return _IsCheckV_GroupType;
            }
            set
            {
                if (_IsCheckV_GroupType != value)
                {
                    _IsCheckV_GroupType = value;
                    NotifyOfPropertyChange(() => IsCheckV_GroupType);
                    if (_IsCheckV_GroupType)
                    {
                        NewStorage.V_GroupTypes = (long)AllLookupValues.V_GroupTypes.TINH_GTGT;
                    }
                    else NewStorage.V_GroupTypes = (long)AllLookupValues.V_GroupTypes.KHONG_TINH_GTGT;
                }                
            }
        }
    }
}