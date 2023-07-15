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
/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.Configuration.BedCategory.ViewModels
{
    [Export(typeof(IBedCategory_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BedCategory_ListFindViewModel : ViewModelBase, IBedCategory_ListFind
        , IHandle<BedCategory_Event_Save>
        , IHandle<DeptLocBedSelectedEvent>
    {
        protected override void OnActivate()
        {
            authorization();
            Debug.WriteLine("OnActivate");
            base.OnActivate();
            var DepartmentTreeVM = Globals.GetViewModel<IDeptTree>();
            UCDepartmentTree = DepartmentTreeVM;
            this.ActivateItem(DepartmentTreeVM);

            //var UCBedCategoryGridVM = Globals.GetViewModel<IUCBedCategoryGrid>();
            //UCBedCategoryGrid = UCBedCategoryGridVM;
            //this.ActivateItem(UCBedCategoryGridVM);

            //var UCRoomEditVM = Globals.GetViewModel<IUCRoomEdit>();
            //UCRoomEdit = UCRoomEditVM;
            //this.ActivateItem(UCRoomEditVM);

            //var UCRoomInfoVM = Globals.GetViewModel<IUCRoomInfo>();
            //UCRoomInfo = UCRoomInfoVM;
            //this.ActivateItem(UCRoomInfoVM);
            GetAllBedType();
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
        public BedCategory_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            SearchCriteria = new BedCategorySearchCriteria();
            SearchCriteria.V_BedType = 0;
            SearchCriteria.HosBedCode = "";
            SearchCriteria.HosBedName = "";


            ObjBedCategory_Paging = new PagedSortableCollectionView<DataEntities.BedCategory>();
            ObjBedCategory_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjBedCategory_Paging_OnRefresh);
            ObjBedCategory_Paging.PageSize = 100;
            ObjBedCategory_ByDeptLocID_Paging = new PagedSortableCollectionView<DataEntities.BedCategory>();
            ObjBedCategory_ByDeptLocID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjBedCategory_ByDeptLocID_Paging_OnRefresh);
            ObjBedCategory_ByDeptLocID_Paging.PageSize = 250;
            BedTypeSelected = new Lookup();
            allBedCategory = new ObjectEdit<DataEntities.BedCategory>("BedCategoryID", "DeptLocID", "");
        }

        void ObjBedCategory_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            BedCategory_Paging(ObjBedCategory_Paging.PageIndex,
                            ObjBedCategory_Paging.PageSize, false);
        }
        void ObjBedCategory_ByDeptLocID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            BedCategory_ByDeptLocID_Paging(ObjBedCategory_ByDeptLocID_Paging.PageIndex,
                             ObjBedCategory_ByDeptLocID_Paging.PageSize, false);
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

        private object _UCDepartmentTree;

        public object UCDepartmentTree
        {
            get { return _UCDepartmentTree; }
            set
            {
                _UCDepartmentTree = value;
                NotifyOfPropertyChange(() => UCDepartmentTree);
            }
        }

        private object _UCBedCategoryGrid;
        public object UCBedCategoryGrid
        {
            get
            {
                return _UCBedCategoryGrid;
            }
            set
            {
                if (_UCBedCategoryGrid == value)
                    return;
                _UCBedCategoryGrid = value;
                NotifyOfPropertyChange(() => UCBedCategoryGrid);
            }
        }


        private object _UCRoomInfo;
        public object UCRoomInfo
        {
            get
            {
                return _UCRoomInfo;
            }
            set
            {
                if (_UCRoomInfo == value)
                    return;
                _UCRoomInfo = value;
                NotifyOfPropertyChange(() => UCRoomInfo);
            }
        }

        private BedCategorySearchCriteria _SearchCriteria;
        public BedCategorySearchCriteria SearchCriteria
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
        private ObservableCollection<Lookup> _BedTypeList;

        public ObservableCollection<Lookup> BedTypeList
        {
            get { return _BedTypeList; }
            set
            {
                _BedTypeList = value;
                NotifyOfPropertyChange(() => BedTypeList);
            }
        }

        private Lookup _BedTypeSelected;

        public Lookup BedTypeSelected
        {
            get { return _BedTypeSelected; }
            set
            {
                //_BedTypeSelected = value;
                //NotifyOfPropertyChange(() => BedTypeSelected);
                if (_BedTypeSelected != value)
                {
                    _BedTypeSelected = value;
                    NotifyOfPropertyChange(() => BedTypeSelected);

                    //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
                    {
                        if (BedTypeSelected.LookupID > 0)
                        {
                            SearchCriteria.V_BedType = BedTypeSelected.LookupID;
                            ObjBedCategory_Paging.PageIndex = 0;
                            BedCategory_Paging(0, ObjBedCategory_Paging.PageSize, true);
                        }
                        else//-1 Text Chọn Loại Dịch Vụ
                        {
                            //Xóa Lưới
                            if (ObjBedCategory_Paging == null)
                            {
                                ObjBedCategory_Paging = new PagedSortableCollectionView<DataEntities.BedCategory>();
                            }
                            ObjBedCategory_Paging.Clear();
                            //Xóa Lưới
                        }
                    }
                }
            }
        }
        private RefDepartmentsTree _SeletedRefDepartmentsTree;
        public RefDepartmentsTree SeletedRefDepartmentsTree
        {
            get
            {
                return _SeletedRefDepartmentsTree;
            }
            set
            {
                if (_SeletedRefDepartmentsTree == value)
                    return;
                _SeletedRefDepartmentsTree = value;
                NotifyOfPropertyChange(() => SeletedRefDepartmentsTree);
            }
        }

        private PagedSortableCollectionView<DataEntities.BedCategory> _ObjBedCategory_Paging;
        public PagedSortableCollectionView<DataEntities.BedCategory> ObjBedCategory_Paging
        {
            get { return _ObjBedCategory_Paging; }
            set
            {
                _ObjBedCategory_Paging = value;
                NotifyOfPropertyChange(() => ObjBedCategory_Paging);
            }
        }
        private PagedSortableCollectionView<DataEntities.BedCategory> _ObjBedCategory_ByDeptLocID_Paging;
        public PagedSortableCollectionView<DataEntities.BedCategory> ObjBedCategory_ByDeptLocID_Paging
        {
            get { return _ObjBedCategory_ByDeptLocID_Paging; }
            set
            {
                _ObjBedCategory_ByDeptLocID_Paging = value;
                NotifyOfPropertyChange(() => ObjBedCategory_ByDeptLocID_Paging);
            }
        }
        private DataEntities.BedCategory _curBedCategory;
        public DataEntities.BedCategory curBedCategory
        {
            get { return _curBedCategory; }
            set
            {
                _curBedCategory = value;
                NotifyOfPropertyChange(() => curBedCategory);
            }
        }

        private bool _IsValidBedCategory = true;
        public bool IsValidBedCategory
        {
            get { return _IsValidBedCategory; }
            set
            {
                _IsValidBedCategory = value;
                NotifyOfPropertyChange(() => IsValidBedCategory);
            }
        }

        private ObjectEdit<DataEntities.BedCategory> _allBedCategory;
        public ObjectEdit<DataEntities.BedCategory> allBedCategory
        {
            get { return _allBedCategory; }
            set
            {
                _allBedCategory = value;
                NotifyOfPropertyChange(() => allBedCategory);
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
        public void GetAllBedType()
        {

            BedTypeList = new ObservableCollection<Lookup>();
            foreach (var tmpLookup in Globals.AllLookupValueList)
            {
                if (tmpLookup.ObjectTypeID == (long)(LookupValues.V_BedType))
                {
                    BedTypeList.Add(tmpLookup);
                }
            }
        }
        public void btSearch()
        {
            ObjBedCategory_Paging.PageIndex = 0;
            BedCategory_Paging(0, ObjBedCategory_Paging.PageSize, true);
        }

        public void btSearch2()
        {
            ObjBedCategory_Paging.PageIndex = 0;
            SearchCriteria.IsBookBed = true;
            BedCategory_Paging(0, ObjBedCategory_Paging.PageSize, true);
        }

        public void BedCategory_MarkDeleted(Int64 IDCode)
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
            //            contract.BeginBedCategory_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    contract.EndBedCategory_MarkDelete(out Result, asyncResult);
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
            //                        ObjBedCategory_Paging.PageIndex = 0;
            //                        BedCategory_Paging(0, ObjBedCategory_Paging.PageSize, true);
            //                        Globals.ShowMessage("Dừng sử dụng BedCategory", "Thông báo");
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

            //BedCategory p = (selectedItem as BedCategory);
            //if (p != null && p.IDCode > 0)
            //{
            //    if (MessageBox.Show(string.Format("Bạn có muốn dừng BedCategory này", p.BedCategory10Code), "Tạm dừng BedCategory", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        BedCategory_MarkDeleted(p.IDCode);
            //    }
            //}
        }

        private object _BedCategory_Current;
        public object BedCategory_Current
        {
            get { return _BedCategory_Current; }
            set
            {
                _BedCategory_Current = value;
                NotifyOfPropertyChange(() => BedCategory_Current);
            }
        }

        public void DoubleClick(object args)
        {
            //EventArgs<object> eventArgs = args as EventArgs<object>;
            //BedCategory_Current = eventArgs.Value as  DataEntities.BedCategory;
            //Globals.EventAggregator.Publish(new dgBedCategoryListClickSelectionChanged_Event() { BedCategory_Current = eventArgs.Value });
        }

        public void dtgListSelectionChanged(object args)
        {
            //if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            //{
            //    if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
            //    {
            //        BedCategory_Current =
            //            ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
            //        var typeInfo = Globals.GetViewModel<IBedCategory_ListFind>();
            //        typeInfo.BedCategory_Current = (BedCategory)BedCategory_Current;

            //        Globals.EventAggregator.Publish(new dgBedCategoryListClickSelectionChanged_Event()
            //        {
            //            BedCategory_Current = ((object[]) (((System.Windows.Controls.SelectionChangedEventArgs) (args)).AddedItems))[0]
            //        });
            //    }
            //}
        }

        private void BedCategory_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách giường");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginBedCategory_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.BedCategory> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndBedCategory_Paging(out Total, asyncResult);
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

                            ObjBedCategory_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjBedCategory_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjBedCategory_Paging.Add(item);
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
        private void BedCategory_ByDeptLocID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách giường");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginBedCategory_ByDeptLocID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.BedCategory> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndBedCategory_ByDeptLocID_Paging(out Total, asyncResult);
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

                            ObjBedCategory_ByDeptLocID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjBedCategory_ByDeptLocID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjBedCategory_ByDeptLocID_Paging.Add(item);
                                    }
                                }
                                allBedCategory = new ObjectEdit<DataEntities.BedCategory>(ObjBedCategory_ByDeptLocID_Paging
                                  , "BedCategoryID", "DeptLocID", "");
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
        private void CheckValidBedCategory(long BedCategoryID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginCheckValidBedCategory(BedCategoryID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                IsValidBedCategory = client.EndCheckValidBedCategory(asyncResult);
                                AddBedToRoom();
                            }
                            catch (Exception ex)
                            {
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
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
            });
            t.Start();
        }

        public void hplAddNew_Click()
        {
            Action<IBedCategory_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Giướng Bệnh";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IBedCategory_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.GetAllBedType();
                    typeInfo.ObjBedCategory_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.BedCategory));

                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.BedCategory).HosBedName.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IBedCategory_AddEdit>(onInitDlg);
            }
        }
        public void btAddChoose()
        {
            CheckValidBedCategory(curBedCategory.BedCategoryID);
        }
        public void BedCategrory_DoubleClick()
        {
            CheckValidBedCategory(curBedCategory.BedCategoryID);
           
        }
        private void AddBedToRoom()
        {
            if (IsValidBedCategory)
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
                return;
            }
            DataEntities.BedCategory bedCategory = new DataEntities.BedCategory();
            bedCategory = curBedCategory;
            bedCategory.BedCategoryID = curBedCategory.BedCategoryID;
            bedCategory.DeptLocID = SeletedRefDepartmentsTree.NodeID;
            if (!allBedCategory.Add(bedCategory))
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
            }
        }
        public void btSaveItems()
        {
            if (allBedCategory.DeleteObject != null && allBedCategory.DeleteObject.Count > 0)
            {
                BedCategory_DeleteXML(allBedCategory.DeleteObject);
            }
            if (allBedCategory.NewObject != null && allBedCategory.NewObject.Count > 0)
            {
                BedCategory_InsertXML(allBedCategory.NewObject);
            }
        }
        public void hplDeleteBedCategory_Click(object datacontext)
        {
            DataEntities.BedCategory p = datacontext as DataEntities.BedCategory;
            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.HosBedName), eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                allBedCategory.Remove(p);
            }
        }
        public void BedCategory_InsertXML(ObservableCollection<DataEntities.BedCategory> lstBedCategory)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.ShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginBedCategory_InsertXML(lstBedCategory, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndBedCategory_InsertXML(asyncResult))
                                {
                                    ObjBedCategory_ByDeptLocID_Paging.PageIndex = 0;
                                    BedCategory_ByDeptLocID_Paging(0, ObjBedCategory_ByDeptLocID_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, "Lưu giường thành công", MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, "Thêm giường cho phòng", MessageBoxButton.OK);
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
        public void BedCategory_DeleteXML(ObservableCollection<DataEntities.BedCategory> lstBedCategory)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });
            this.ShowBusyIndicator(eHCMSResources.Z0492_G1_DangXoa);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginBedCategory_DeleteXML(lstBedCategory, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndBedCategory_DeleteXML(asyncResult))
                                {
                                    ObjBedCategory_ByDeptLocID_Paging.PageIndex = 0;
                                    BedCategory_ByDeptLocID_Paging(0, ObjBedCategory_ByDeptLocID_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, "Xóa giường cho phòng", MessageBoxButton.OK);
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
        public void Handle(BedCategory_Event_Save message)
        {
            ObjBedCategory_Paging.PageIndex = 0;
            BedCategory_Paging(0, ObjBedCategory_Paging.PageSize, true);
        }
        public void Handle(DeptLocBedSelectedEvent obj)
        {
            if (obj != null && ((RefDepartmentsTree)obj.curDeptLoc).NodeID > 0)
            {
                SeletedRefDepartmentsTree = (RefDepartmentsTree)obj.curDeptLoc;


                SearchCriteria.DeptLocID = SeletedRefDepartmentsTree.NodeID;
                //BedTypeSelected.LookupID = -1;

                ////Xóa Lưới
                //ObjGetDeptMedServiceItems_Paging.Clear();
                ////Xóa Lưới

                //RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(SearchCriteria.DeptID, 1);//Subtract loại PCL
                //SearchCriteria.DeptLocID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                ObjBedCategory_ByDeptLocID_Paging.PageIndex = 0;

                BedCategory_ByDeptLocID_Paging(0, ObjBedCategory_ByDeptLocID_Paging.PageSize, true);
            }
        }
    }
}
