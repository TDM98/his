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
using aEMR.Common.ExportExcel;
/*
* #001 20180921 TNHX: Apply BusyIndicator, refactor code
* 20230509 #002 DatTB: IssueID: 3254 | Thêm nút xuất excel cho các danh mục/cấu hình
* 20230601 #003 DatTB: IssueID: 3254 | Chỉnh sửa/Gộp các function xuất excel danh mục/cấu hình (Bỏ Func cũ)
*/
namespace aEMR.Configuration.Locations.ViewModels
{
    [Export(typeof(ILocations_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Locations_ListFindViewModel : ViewModelBase, ILocations_ListFind
        , IHandle<Location_Event_Save>
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
        public Locations_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            ObjRoomType_GetAll = new ObservableCollection<DataEntities.RoomType>();
            SearchCriteria = new LocationSearchCriteria();
            SearchCriteria.RmTypeID = -1;
            SearchCriteria.LocationName = "";
            SearchCriteria.OrderBy = "";
            RoomType_GetAll();

            ObjLocations_ByRmTypeID_Paging = new PagedSortableCollectionView<Location>();
            ObjLocations_ByRmTypeID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjLocations_ByRmTypeID_Paging_OnRefresh);
        }

        void ObjLocations_ByRmTypeID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Locations_ByRmTypeID_Paging(ObjLocations_ByRmTypeID_Paging.PageIndex,
                            ObjLocations_ByRmTypeID_Paging.PageSize, false);
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

        private ObservableCollection<DataEntities.RoomType> _ObjRoomType_GetAll;
        public ObservableCollection<DataEntities.RoomType> ObjRoomType_GetAll
        {
            get
            {
                return _ObjRoomType_GetAll;
            }
            set
            {
                _ObjRoomType_GetAll = value;
                NotifyOfPropertyChange(() => ObjRoomType_GetAll);
            }
        }

        public void RoomType_GetAll()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            this.ShowBusyIndicator(eHCMSResources.K2993_G1_DSLgoaiPg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRoomType_GetAll(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndRoomType_GetAll(asyncResult);
                                if (items != null)
                                {
                                    ObjRoomType_GetAll = new ObservableCollection<DataEntities.RoomType>(items);
                                    //ItemDefault
                                    DataEntities.RoomType RoomTypeDefault = new DataEntities.RoomType();
                                    RoomTypeDefault.RmTypeID = -1;
                                    RoomTypeDefault.RmTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjRoomType_GetAll.Insert(0, RoomTypeDefault);
                                    //ItemDefault
                                }
                                else
                                {
                                    ObjRoomType_GetAll = null;
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

        private DataEntities.LocationSearchCriteria _SearchCriteria;
        public DataEntities.LocationSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<DataEntities.Location> _ObjLocations_ByRmTypeID_Paging;
        public PagedSortableCollectionView<DataEntities.Location> ObjLocations_ByRmTypeID_Paging
        {
            get { return _ObjLocations_ByRmTypeID_Paging; }
            set
            {
                _ObjLocations_ByRmTypeID_Paging = value;
                NotifyOfPropertyChange(() => ObjLocations_ByRmTypeID_Paging);
            }
        }

        public void cboRoomTypeSelectedItemChanged(object selectedItem)
        {
            SearchCriteria.RmTypeID = (selectedItem as DataEntities.RoomType).RmTypeID;
            ObjLocations_ByRmTypeID_Paging.PageIndex = 0;
            Locations_ByRmTypeID_Paging(0, ObjLocations_ByRmTypeID_Paging.PageSize, true);
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPhong,
                                               (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mEdit);
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucPhong,
                                               (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mDelete);
            bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                        , (int)eConfiguration_Management.mDanhMucPhong,
                                        (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                                , (int)eConfiguration_Management.mDanhMucPhong,
                                                (int)oConfigurationEx.mQuanLyDanhSachPhong, (int)ePermission.mAdd);
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
            ObjLocations_ByRmTypeID_Paging.PageIndex = 0;
            Locations_ByRmTypeID_Paging(0, ObjLocations_ByRmTypeID_Paging.PageSize, true);
        }

        public void Locations_MarkDeleted(Int64 LID)
        {
            string Result = "";
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.DlgShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginLocations_MarkDeleted(LID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndLocations_MarkDeleted(out Result, asyncResult);
                                if (Result == "InUse")
                                {
                                    Globals.ShowMessage(eHCMSResources.Z1318_G1_PhgDangSDungKgTheXoa, eHCMSResources.G2617_G1_Xoa);
                                }
                                if (Result == "0")
                                {
                                    Globals.ShowMessage(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa);
                                }
                                if (Result == "1")
                                {
                                    ObjLocations_ByRmTypeID_Paging.PageIndex = 0;
                                    Locations_ByRmTypeID_Paging(0, ObjLocations_ByRmTypeID_Paging.PageSize, true);
                                    Globals.ShowMessage(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });
            t.Start();
        }

        public void hplDelete_Click(object selectedItem)
        {
            Location p = (selectedItem as Location);
            if (p != null && p.LID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.LocationName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Locations_MarkDeleted(p.LID);
                }
            }
        }

        private object _Locations_Current;
        public object Locations_Current
        {
            get { return _Locations_Current; }
            set
            {
                _Locations_Current = value;
                NotifyOfPropertyChange(() => Locations_Current);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;
            Locations_Current = eventArgs.Value as Location;
            Globals.EventAggregator.Publish(new dgListDblClickSelectLocation_Event() { Location_Current = eventArgs.Value });
        }

        public void dtgListSelectionChanged(object args)
        {
            if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            {
                if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
                {
                    Locations_Current =
                        ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
                    var typeInfo = Globals.GetViewModel<IDeptLocation_ByDeptID>();
                    typeInfo.Locations_SelectForAdd = Locations_Current;

                    Globals.EventAggregator.Publish(new dgListClickSelectionChanged_Event()
                    {
                        Location_Current = ((object[]) (((System.Windows.Controls.SelectionChangedEventArgs) (args)).AddedItems))[0]
                    });
                }
            }
        }

        private void Locations_ByRmTypeID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator(eHCMSResources.K3054_G1_DSPg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginLocations_ByRmTypeID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.Location> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndLocations_ByRmTypeID_Paging(out Total, asyncResult);
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

                            ObjLocations_ByRmTypeID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjLocations_ByRmTypeID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjLocations_ByRmTypeID_Paging.Add(item);
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
            //Screen screen = new Screen();

            //var typeInfo = Globals.GetViewModel<ILocations_AddEdit>();
            //typeInfo.TitleForm = "Thêm Mới Phòng";
            //typeInfo.ObjRoomType_GetAll = ObjRoomType_GetAll;
            //typeInfo.InitializeNewItem(SearchCriteria.RmTypeID);

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});

            Action<ILocations_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Phòng";
                typeInfo.ObjRoomType_GetAll = ObjRoomType_GetAll;
                typeInfo.InitializeNewItem(SearchCriteria.RmTypeID);
            };
            GlobalsNAV.ShowDialog<ILocations_AddEdit>(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<ILocations_AddEdit>();
                //typeInfo.ObjLocation_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.Location));

                //typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.Location).LocationName.Trim() + ")";
                //typeInfo.ObjRoomType_GetAll = ObjRoomType_GetAll;

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //{
                //    //làm gì đó
                //});

                Action<ILocations_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjLocation_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.Location));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.Location).LocationName.Trim() + ")";
                    typeInfo.ObjRoomType_GetAll = ObjRoomType_GetAll;
                };
                GlobalsNAV.ShowDialog<ILocations_AddEdit>(onInitDlg);
            }
        }

        public void Handle(Location_Event_Save message)
        {
            ObjLocations_ByRmTypeID_Paging.PageIndex = 0;
            Locations_ByRmTypeID_Paging(0, ObjLocations_ByRmTypeID_Paging.PageSize, true);
        }

        //▼==== #003
        public void BtnExportExcel()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0669_G1_DangLayDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        ConfigurationReportParams Params = new ConfigurationReportParams()
                        {
                            ConfigurationName = ConfigurationName.Locations
                        };

                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginExportExcelConfigurationManager(Params, Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var results = contract.EndExportExcelConfigurationManager(asyncResult);
                                ExportToExcelFileAllData.Export(results, "Shee1");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲==== #003
    }
}
