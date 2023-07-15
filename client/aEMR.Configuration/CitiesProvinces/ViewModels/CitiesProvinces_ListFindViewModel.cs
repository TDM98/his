using eHCMSLanguage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.ViewContracts;
using System.Windows.Controls;
using aEMR.Common.BaseModel;
using System.Linq;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
*/
namespace aEMR.Configuration.CitiesProvinces.ViewModels
{
    [Export(typeof(ICitiesProvinces_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CitiesProvinces_ListFindViewModel : ViewModelBase, ICitiesProvinces_ListFind
        , IHandle<CitiesProvinces_Event_Save>
    {
        protected override void OnActivate()
        {
            authorization();
            Debug.WriteLine("OnActivate");
            base.OnActivate();
        }
        protected override void OnDeactivate(bool close)
        {
            Debug.WriteLine("OnDeActivate");
            base.OnDeactivate(close);
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CitiesProvinces_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            LoadProvinces();
            LoadSuburbName();
            SearchCitiesProvinces = "";
       

            ObjCitiesProvince_Paging = new PagedSortableCollectionView<CitiesProvince>();
            ObjCitiesProvince_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjCitiesProvince_Paging_OnRefresh);
            ObjSuburbNames_Paging = new PagedSortableCollectionView<SuburbNames>();
            ObjSuburbNames_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjSuburbNames_Paging_OnRefresh);
            ObjWardNames_Paging = new PagedSortableCollectionView<WardNames>();
            ObjWardNames_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjWardNames_Paging_OnRefresh);
        }

        void ObjCitiesProvince_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            CitiesProvince_Paging(ObjCitiesProvince_Paging.PageIndex,
                            ObjCitiesProvince_Paging.PageSize, false);
        }
        void ObjSuburbNames_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            SuburbNames_Paging(ObjSuburbNames_Paging.PageIndex,
                            ObjSuburbNames_Paging.PageSize, false);
        }
        void ObjWardNames_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            WardNames_Paging(ObjWardNames_Paging.PageIndex,
                            ObjWardNames_Paging.PageSize, false);
        }

        private Visibility _hplAddNewVisible = Visibility.Visible;
        public Visibility hplAddNewVisible
        {
            get { return _hplAddNewVisible; }
            set
            {
                _hplAddNewVisible = value;
                NotifyOfPropertyChange(() => hplAddNewVisible);
            }
        }

        //private ObservableCollection<DataEntities.CitiesProvince> _ObjCitiesProvince_GetAll;
        //public ObservableCollection<DataEntities.CitiesProvince> ObjCitiesProvince_GetAll
        //{
        //    get
        //    {
        //        return _ObjCitiesProvince_GetAll;
        //    }
        //    set
        //    {
        //        _ObjCitiesProvince_GetAll = value;
        //        NotifyOfPropertyChange(() => ObjCitiesProvince_GetAll);
        //    }
        //}

        //public void CitiesProvince_GetAll()
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
        //    this.ShowBusyIndicator(eHCMSResources.K2993_G1_DSLgoaiPg);
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ConfigurationManagerServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                contract.BeginCitiesProvince_GetAll(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        var items = contract.EndCitiesProvince_GetAll(asyncResult);
        //                        if (items != null)
        //                        {
        //                            ObjCitiesProvince_GetAll = new ObservableCollection<DataEntities.CitiesProvince>(items);
        //                            //ItemDefault
        //                            DataEntities.CitiesProvince RoomTypeDefault = new DataEntities.CitiesProvince();
        //                            RoomTypeDefault.IDCode = -1;
        //                            RoomTypeDefault.DiseaseNameVN = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                            ObjCitiesProvince_GetAll.Insert(0, RoomTypeDefault);
        //                            //ItemDefault
        //                        }
        //                        else
        //                        {
        //                            ObjCitiesProvince_GetAll = null;
        //                        }
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                    }
        //                    finally
        //                    {
        //                        this.HideBusyIndicator();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //            this.HideBusyIndicator();
        //        }
        //    });
        //    t.Start();
        //}

        private string _SearchCitiesProvinces;
        public string SearchCitiesProvinces
        {
            get
            {
                return _SearchCitiesProvinces;
            }
            set
            {
                _SearchCitiesProvinces = value;
                NotifyOfPropertyChange(() => SearchCitiesProvinces);
            }
        }

        private string _SearchSuburbNames;
        public string SearchSuburbNames
        {
            get
            {
                return _SearchSuburbNames;
            }
            set
            {
                _SearchSuburbNames = value;
                NotifyOfPropertyChange(() => SearchSuburbNames);
            }
        }

        private string _SearchWardNames;
        public string SearchWardNames
        {
            get
            {
                return _SearchWardNames;
            }
            set
            {
                _SearchWardNames = value;
                NotifyOfPropertyChange(() => SearchWardNames);
            }
        }

        private PagedSortableCollectionView<CitiesProvince> _ObjCitiesProvince_Paging;
        public PagedSortableCollectionView<CitiesProvince> ObjCitiesProvince_Paging
        {
            get { return _ObjCitiesProvince_Paging; }
            set
            {
                _ObjCitiesProvince_Paging = value;
                NotifyOfPropertyChange(() => ObjCitiesProvince_Paging);
            }
        }

        private PagedSortableCollectionView<SuburbNames> _ObjSuburbNames_Paging;
        public PagedSortableCollectionView<SuburbNames> ObjSuburbNames_Paging
        {
            get { return _ObjSuburbNames_Paging; }
            set
            {
                _ObjSuburbNames_Paging = value;
                NotifyOfPropertyChange(() => ObjSuburbNames_Paging);
            }
        }

        private PagedSortableCollectionView<WardNames> _ObjWardNames_Paging;
        public PagedSortableCollectionView<WardNames> ObjWardNames_Paging
        {
            get { return _ObjWardNames_Paging; }
            set
            {
                _ObjWardNames_Paging = value;
                NotifyOfPropertyChange(() => ObjWardNames_Paging);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            //bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mEdit);
            //bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                   , (int)eConfiguration_Management.mDanhMucPhong,
            //                                   (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mDelete);
            //bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                            , (int)eConfiguration_Management.mDanhMucPhong,
            //                            (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mView);
            //bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
            //                                    , (int)eConfiguration_Management.mDanhMucPhong,
            //                                    (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bbtSearch = true;
        private bool _bhplAddNew = true;
        public bool bhplEdit
        {
            get
            {
                return _bhplEdit;
            }
            set
            {
                if (_bhplEdit == value)
                    return;
                _bhplEdit = value;
            }
        }

        public bool bhplDelete
        {
            get
            {
                return _bhplDelete;
            }
            set
            {
                if (_bhplDelete == value)
                    return;
                _bhplDelete = value;
            }
        }

        public bool bbtSearch
        {
            get
            {
                return _bbtSearch;
            }
            set
            {
                if (_bbtSearch == value)
                    return;
                _bbtSearch = value;
            }
        }

        public bool bhplAddNew
        {
            get
            {
                return _bhplAddNew;
            }
            set
            {
                if (_bhplAddNew == value)
                    return;
                _bhplAddNew = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEdit { get; set; }
        public Button hplDelete { get; set; }

        public void hplEdit_Loaded(object sender)
        {
            hplEdit = sender as Button;
            hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }
        #endregion

        public void btSearch()
        {
            ObjCitiesProvince_Paging.PageIndex = 0;
            CitiesProvince_Paging(0, ObjCitiesProvince_Paging.PageSize, true);
        }
        public void btSearchSuburbNames()
        {
            ObjSuburbNames_Paging.PageIndex = 0;
            SuburbNames_Paging(0, ObjSuburbNames_Paging.PageSize, true);
        }
        public void btSearchWardNames()
        {
            ObjWardNames_Paging.PageIndex = 0;
            WardNames_Paging(0, ObjWardNames_Paging.PageSize, true);
        }

        public void CitiesProvinces_MarkDeleted(Int64 IDCode)
        {
            //string Result = "";
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            //this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new ConfigurationManagerServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginCitiesProvince_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    contract.EndCitiesProvince_MarkDelete(out Result, asyncResult);
            //                    if (Result == "InUse")
            //                    {
            //                        Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
            //                    }
            //                    if (Result == "0")
            //                    {
            //                        Globals.ShowMessage("Thất bại", "Thông báo");
            //                    }
            //                    if (Result == "1")
            //                    {
            //                        ObjCitiesProvince_Paging.PageIndex = 0;
            //                        CitiesProvince_ByIDCode_Paging(0, ObjCitiesProvince_Paging.PageSize, true);
            //                        Globals.ShowMessage("Dừng sử dụng CitiesProvince", "Thông báo");
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //                }
            //                finally
            //                {
            //                    this.DlgHideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //        this.DlgHideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

        public void SuburbNames_MarkDeleted(Int64 IDCode)
        {
            //string Result = "";
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            //this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new ConfigurationManagerServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginCitiesProvince_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    contract.EndCitiesProvince_MarkDelete(out Result, asyncResult);
            //                    //if (Result == "InUse")
            //                    //{
            //                    //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
            //                    //}
            //                    if (Result == "0")
            //                    {
            //                        Globals.ShowMessage("Thất bại", "Thông báo");
            //                    }
            //                    if (Result == "1")
            //                    {
            //                        ObjCitiesProvince_Paging.PageIndex = 0;
            //                        CitiesProvince_ByIDCode_Paging(0, ObjCitiesProvince_Paging.PageSize, true);
            //                        Globals.ShowMessage("Dừng sử dụng CitiesProvince", "Thông báo");
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //                }
            //                finally
            //                {
            //                    this.DlgHideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //        this.DlgHideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

        public void WardNames_MarkDeleted(Int64 IDCode)
        {
            //string Result = "";
            ////Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            //this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new ConfigurationManagerServiceClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginCitiesProvince_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    contract.EndCitiesProvince_MarkDelete(out Result, asyncResult);
            //                    //if (Result == "InUse")
            //                    //{
            //                    //    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
            //                    //}
            //                    if (Result == "0")
            //                    {
            //                        Globals.ShowMessage("Thất bại", "Thông báo");
            //                    }
            //                    if (Result == "1")
            //                    {
            //                        ObjCitiesProvince_Paging.PageIndex = 0;
            //                        CitiesProvince_ByIDCode_Paging(0, ObjCitiesProvince_Paging.PageSize, true);
            //                        Globals.ShowMessage("Dừng sử dụng CitiesProvince", "Thông báo");
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //                }
            //                finally
            //                {
            //                    this.DlgHideBusyIndicator();
            //                }
            //            }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            //        this.DlgHideBusyIndicator();
            //    }
            //});
            //t.Start();
        }
        public void hplDelete_Click(object selectedItem)
        {

            //CitiesProvince p = (selectedItem as CitiesProvince);
            //if (p != null && p.IDCode > 0)
            //{
            //    if (MessageBox.Show(string.Format("Bạn có muốn dừng CitiesProvince này", p.CitiesProvince10Code), "Tạm dừng CitiesProvince", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        CitiesProvince_MarkDeleted(p.IDCode);
            //    }
            //}
        }

        private object _CitiesProvinces_Current;
        public object CitiesProvinces_Current
        {
            get { return _CitiesProvinces_Current; }
            set
            {
                _CitiesProvinces_Current = value;
                NotifyOfPropertyChange(() => CitiesProvinces_Current);
            }
        }

        private object _SuburbNames_Current;
        public object SuburbNames_Current
        {
            get { return _SuburbNames_Current; }
            set
            {
                _SuburbNames_Current = value;
                NotifyOfPropertyChange(() => SuburbNames_Current);
            }
        }

        private object _WardNames_Current;
        public object WardNames_Current
        {
            get { return _WardNames_Current; }
            set
            {
                _WardNames_Current = value;
                NotifyOfPropertyChange(() => WardNames_Current);
            }
        }

        private long _CitiesProvinces_ID = 0;
        public long CitiesProvinces_ID
        {
            get { return _CitiesProvinces_ID; }
            set
            {
                _CitiesProvinces_ID = value;
                NotifyOfPropertyChange(() => CitiesProvinces_ID);
            }
        }
        private long _SuburbNames_ID = 0;
        public long SuburbNames_ID
        {
            get { return _SuburbNames_ID; }
            set
            {
                _SuburbNames_ID = value;
                NotifyOfPropertyChange(() => SuburbNames_ID);
            }
        }

        private ObservableCollection<CitiesProvince> _provinces;
        public ObservableCollection<CitiesProvince> Provinces
        {
            get { return _provinces; }
            set
            {
                _provinces = value;
                NotifyOfPropertyChange(() => Provinces);
            }
        }
        private ObservableCollection<SuburbNames> _SuburbNames;
        public ObservableCollection<SuburbNames> SuburbNames
        {
            get { return _SuburbNames; }
            set
            {
                _SuburbNames = value;
                NotifyOfPropertyChange(() => SuburbNames);
            }
        }
        
        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            CitiesProvinces_Current = eventArgs.Value as CitiesProvince;
            //Globals.EventAggregator.Publish(new dgCitiesProvinceListClickSelectionChanged_Event() { CitiesProvince_Current = eventArgs.Value });
        }

        //public void dtgListSelectionChanged(object args)
        //{
        //    if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
        //    {
        //        if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
        //        {
        //            CitiesProvince_Current =
        //                ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
        //            var typeInfo = Globals.GetViewModel<ICitiesProvince_ListFind>();
        //            typeInfo.CitiesProvince_Current = (CitiesProvince)CitiesProvince_Current;

        //            Globals.EventAggregator.Publish(new dgCitiesProvinceListClickSelectionChanged_Event()
        //            {
        //                CitiesProvince_Current = ((object[]) (((System.Windows.Controls.SelectionChangedEventArgs) (args)).AddedItems))[0]
        //            });
        //        }
        //    }
        //}

        private void CitiesProvince_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách tỉnh");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginCitiesProvince_Paging(SearchCitiesProvinces, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<CitiesProvince> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndCitiesProvince_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjCitiesProvince_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjCitiesProvince_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjCitiesProvince_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        private void SuburbNames_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách quận/huyện");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSuburbNames_Paging(CitiesProvinces_ID, SearchSuburbNames, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<SuburbNames> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSuburbNames_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjSuburbNames_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjSuburbNames_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjSuburbNames_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        private void WardNames_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách phường/xã");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginWardNames_Paging(CitiesProvinces_ID, SuburbNames_ID,SearchWardNames, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<WardNames> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndWardNames_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjWardNames_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjWardNames_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjWardNames_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }
        public void hplAddNew_Click()
        {
            Action<ICitiesProvinces_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Tỉnh";
                typeInfo.FormType = 1;
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewSuburbNames_Click()
        {
            Action<ICitiesProvinces_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Quận/Huyện";
                typeInfo.FormType = 2;
                if (CitiesProvinces_ID == 0)
                {
                    typeInfo.InitializeNewItem();
                }
                else
                {
                    typeInfo.ObjSuburbNames_Current = new SuburbNames();
                    typeInfo.ObjSuburbNames_Current.CityProvinceID = CitiesProvinces_ID;
                }
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        public void hplAddNewWardNames_Click()
        {
            Action<ICitiesProvinces_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Phường/Xã";
                typeInfo.FormType = 3;
                if(CitiesProvinces_ID == 0)
                {
                    typeInfo.InitializeNewItem();
                }
                else if(SuburbNames_ID == 0)
                {
                    typeInfo.ObjSuburbNames_Current = new SuburbNames();
                    typeInfo.ObjSuburbNames_Current.CityProvinceID = CitiesProvinces_ID;
                }
                else
                {
                    typeInfo.ObjSuburbNames_Current = new SuburbNames();
                    typeInfo.ObjSuburbNames_Current.CityProvinceID = CitiesProvinces_ID;
                    typeInfo.ObjWardNames_Current = new WardNames();
                    typeInfo.ObjWardNames_Current.SuburbNameID = SuburbNames_ID;
                }
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        
        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ICitiesProvinces_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjCitiesProvinces_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.CitiesProvince));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.CitiesProvince).CityProvinceName.Trim() + ")";
                    typeInfo.FormType = 1;
                };
                GlobalsNAV.ShowDialog<ICitiesProvinces_AddEdit>(onInitDlg);
            }
        }
        public void hplEditSuburbNames_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ICitiesProvinces_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.Provinces = Provinces;
                    typeInfo.ObjSuburbNames_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.SuburbNames));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.SuburbNames).SuburbName.Trim() + ")";
                    typeInfo.FormType = 2;
                };
                GlobalsNAV.ShowDialog<ICitiesProvinces_AddEdit>(onInitDlg);
            }
        }
        public void hplEditWardNames_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ICitiesProvinces_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.Provinces = Provinces;
                    typeInfo.SuburbNames = SuburbNames;
                    typeInfo.ObjSuburbNames_Current = new SuburbNames();
                    typeInfo.ObjSuburbNames_Current.CityProvinceID = CitiesProvinces_ID;
                    typeInfo.ObjWardNames_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.WardNames));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.WardNames).WardName.Trim() + ")";
                    typeInfo.FormType = 3;
                };
                GlobalsNAV.ShowDialog<ICitiesProvinces_AddEdit>(onInitDlg);
            }
        }

        
        //public void Handle(CitiesProvince_Event_Save message)
        //{
        //    ObjCitiesProvince_Paging.PageIndex = 0;
        //    CitiesProvince_ByIDCode_Paging(0, ObjCitiesProvince_Paging.PageSize, true);
        //}
        public void LoadProvinces()
        {

            //if (Globals.allCitiesProvince != null && Globals.allCitiesProvince.Count > 0)
            //{
            //    Provinces = Globals.allCitiesProvince.ToObservableCollection();
            //    return;
            //}

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllProvinces(Globals.DispatchCallback(asyncResult =>
                        {
                            IList<CitiesProvince> allItems = null;
                            try
                            {
                                allItems = contract.EndGetAllProvinces(asyncResult);
                                //if (Globals.allCitiesProvince == null)
                                //{
                                //    Globals.allCitiesProvince = new List<CitiesProvince>(allItems);
                                //}
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(eHCMSResources.A0693_G1_Msg_InfoKhTheLayDSTinhThanh);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }

                            Provinces = allItems != null ? new ObservableCollection<CitiesProvince>(allItems) : null;
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
        private void LoadSuburbName()
        {
            //if (Globals.allSuburbNames != null && Globals.allSuburbNames.Count > 0)
            //{
            //    return;
            //}

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllSuburbNames(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllSuburbNames(asyncResult);
                                if (allItems != null)
                                {
                                    Globals.allSuburbNames = allItems.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {

                            }

                        }), null);

                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void cboCitiesProvinceSelectedItemChanged(object selectedItem)
        {
            CitiesProvinces_ID = (selectedItem as DataEntities.CitiesProvince).CityProvinceID;
            ObjSuburbNames_Paging.PageIndex = 0;
            SuburbNames_Paging(0, ObjSuburbNames_Paging.PageSize, true);
        }
        public void cboCitiesProvince2SelectedItemChanged(object selectedItem)
        {
            CitiesProvinces_ID = (selectedItem as DataEntities.CitiesProvince).CityProvinceID;
            SuburbNames = new ObservableCollection<SuburbNames>();
            SuburbNames firstItem = new SuburbNames();
            firstItem.SuburbNameID = 0;
            firstItem.SuburbName = "-- Tất cả --";
            SuburbNames.Add(firstItem);
            foreach (var item in Globals.allSuburbNames)
            {
                if (item.CityProvinceID == CitiesProvinces_ID)
                {
                    SuburbNames.Add(item);
                }
            }
        }
        public void cboSubrbsNamesSelectedItemChanged(object selectedItem)
        {
            if (selectedItem != null)
            {
                SuburbNames_ID = (selectedItem as DataEntities.SuburbNames).SuburbNameID;
                ObjWardNames_Paging.PageIndex = 0;
                WardNames_Paging(0, ObjWardNames_Paging.PageSize, true);
            }
            else
            {
                SuburbNames_ID = 0;
            }
        }
        public void Handle(CitiesProvinces_Event_Save message)
        {
            if (message.FormType == 1)
            {
                ObjCitiesProvince_Paging.PageIndex = 0;
                CitiesProvince_Paging(0, ObjCitiesProvince_Paging.PageSize,true);
            }
            else if (message.FormType == 2)
            {
                ObjSuburbNames_Paging.PageIndex = 0;
                SuburbNames_Paging(0, ObjSuburbNames_Paging.PageSize, true);
            }
            else if (message.FormType == 3)
            {
                ObjWardNames_Paging.PageIndex = 0;
                WardNames_Paging(0, ObjWardNames_Paging.PageSize, true);
            }
        }
    }
}
