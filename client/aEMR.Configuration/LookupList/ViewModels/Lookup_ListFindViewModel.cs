using eHCMSLanguage;
using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using aEMR.CommonTasks;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
/*
* #001 20230531: Tạo mới ViewModel
*/
namespace aEMR.Configuration.LookupList.ViewModels
{
    [Export(typeof(ILookup_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Lookup_ListFindViewModel : ViewModelBase, ILookup_ListFind
        , IHandle<Lookup_Event_Save>
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
        public Lookup_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            eventAggregator.Subscribe(this);
            authorization();
            SearchCriteria = "";
            //Coroutine.BeginExecute(LoadLookupValues());
            TreeViewLookups = new ObservableCollection<LookupTree>();
            ObjLookup_Paging = new PagedSortableCollectionView<Lookup>();
            ObjLookup_Paging.OnRefresh += ObjLookup_Paging_OnRefresh;
            GetTreeView_LookupForMngt();
        }

        void ObjLookup_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Lookup_ByType_Paging(CurrentLookupTree != null && CurrentLookupTree.NodeID > 0 ? (LookupValues)CurrentLookupTree.NodeID : 0);
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

        private string _SearchCriteria;
        public string SearchCriteria
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

        private string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        private PagedSortableCollectionView<Lookup> _ObjLookup_Paging;
        public PagedSortableCollectionView<Lookup> ObjLookup_Paging
        {
            get { return _ObjLookup_Paging; }
            set
            {
                _ObjLookup_Paging = value;
                NotifyOfPropertyChange(() => ObjLookup_Paging);
            }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        private bool _IsDoubleClick;
        public bool IsDoubleClick
        {
            get { return _IsDoubleClick; }
            set
            {
                if (_IsDoubleClick != value)
                {
                    _IsDoubleClick = value;
                    NotifyOfPropertyChange(() => IsDoubleClick);
                }
            }
        }

        private LookupTree _CurrentLookupTree;
        public LookupTree CurrentLookupTree
        {
            get
            {
                return _CurrentLookupTree;
            }
            set
            {
                if (_CurrentLookupTree != value)
                {
                    _CurrentLookupTree = value;
                    if(_CurrentLookupTree != null && _CurrentLookupTree.NodeID > 0)
                    {
                        Lookup_ByType_Paging((LookupValues)_CurrentLookupTree.NodeID);
                        Title = string.Format("Danh mục {0}", _CurrentLookupTree.NodeText).ToUpper();
                    }
                    NotifyOfPropertyChange(() => CurrentLookupTree);
                }
            }
        }

        private ObservableCollection<LookupTree> _TreeViewLookups;
        public ObservableCollection<LookupTree> TreeViewLookups
        {
            get
            {
                return _TreeViewLookups;
            }
            set
            {
                if (_TreeViewLookups != value)
                {
                    _TreeViewLookups = value;
                    NotifyOfPropertyChange(() => TreeViewLookups);
                }
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
            //hplEdit.Visibility = Globals.convertVisibility(bhplEdit);
        }
        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            //hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }
        #endregion

        public void btSearch()
        {
            Lookup_ByType_Paging(CurrentLookupTree != null && CurrentLookupTree.NodeID > 0 ? (LookupValues)CurrentLookupTree.NodeID : 0);
        }

        public void Lookup_MarkDeleted(long LKID)
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
            //            contract.BeginLookup_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    contract.EndLookup_MarkDelete(out Result, asyncResult);
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
            //                        ObjLookup_Paging.PageIndex = 0;
            //                        Lookup_ByIDCode_Paging(0, ObjLookup_Paging.PageSize, true);
            //                        Globals.ShowMessage("Dừng sử dụng Job", "Thông báo");
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

            //Job p = (selectedItem as Job);
            //if (p != null && p.IDCode > 0)
            //{
            //    if (MessageBox.Show(string.Format("Bạn có muốn dừng Job này", p.Job10Code), "Tạm dừng Job", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        Lookup_MarkDeleted(p.IDCode);
            //    }
            //}
        }

        private Lookup _Lookup_Current;
        public Lookup Lookup_Current
        {
            get { return _Lookup_Current; }
            set
            {
                _Lookup_Current = value;
                NotifyOfPropertyChange(() => Lookup_Current);
            }
        }

        private void Lookup_ByType_Paging(LookupValues lookupType)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByTypeForMngt(lookupType, SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<Lookup> allItems = new ObservableCollection<Lookup>();
                            try
                            {
                                ObjLookup_Paging.Clear();
                                allItems = contract.EndGetAllLookupValuesByTypeForMngt(asyncResult);
                                if(allItems != null && allItems.Count > 0)
                                {
                                    foreach(var item in allItems)
                                    {
                                        ObjLookup_Paging.Add(item);
                                    }
                                    if(ObjLookup_Paging != null && ObjLookup_Paging.Count > 0)
                                    {
                                        Lookup_Current = ObjLookup_Paging.FirstOrDefault();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {}

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetTreeView_LookupForMngt()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetTreeView_LookupForMngt(Globals.DispatchCallback((asyncResult) =>
                        {
                            TreeViewLookups.Clear();
                            IList<LookupTree> allItems = new ObservableCollection<LookupTree>();
                            try
                            {
                                allItems = contract.EndGetTreeView_LookupForMngt(asyncResult);
                                if (allItems != null && allItems.Count > 0)
                                {
                                    TreeViewLookups = allItems.ToObservableCollection();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
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
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void hplAddNew_Click()
        {
            Action<ILookup_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới danh mục";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<ILookup_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjLookup_Current = ObjectCopier.DeepCopy((selectedItem as Lookup));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as Lookup).ObjectValue.Trim() + ")";
                };
                GlobalsNAV.ShowDialog(onInitDlg);
            }
        }

        public void Handle(Lookup_Event_Save message)
        {
            SearchCriteria = "";
            Lookup_ByType_Paging(CurrentLookupTree != null && CurrentLookupTree.NodeID > 0 ? (LookupValues)CurrentLookupTree.NodeID : 0);
        }

        public void gvRegistrations_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void treeViewLookup_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                CurrentLookupTree = (LookupTree)e.NewValue;
                SearchCriteria = "";
            }
        }

        public void treeViewLookup_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SearchCriteria = "";
            if (!(sender is TreeView) || (sender as TreeView).SelectedItem == null || !((sender as TreeView).SelectedItem is LookupTree))
            {
                return;
            }
            long parentID = ((sender as TreeView).SelectedItem as LookupTree).NodeID;
            LookupTree lookup = TreeViewLookups.Where(x => x.NodeID == parentID).FirstOrDefault();
            Title = string.Format("Danh mục {0}", lookup.NodeText).ToUpper();
            Globals.EventAggregator.Publish(new Lookup_Event_Choose() { ParentSelected = lookup, ChildrenSelected = (sender as TreeView).SelectedItem as LookupTree });
        }

        public void DoubleClick(object args)
        {
            if (Lookup_Current != null)
            {
                Globals.EventAggregator.Publish(new dgLookupListClickSelectionChanged_Event() { Lookup_Current = Lookup_Current });
            }
        }

        public void btnSave()
        {
            if(Lookup_Current == null || CurrentLookupTree == null || CurrentLookupTree.NodeID == 0)
            {
                return;
            }
            if (Lookup_Current.LookupID == 0)
            {

                Lookup_Current.ObjectTypeID = CurrentLookupTree.NodeID;
            }

            if (Lookup_Current.ObjectValue != "")
            {
                Lookup_InsertUpdate(Lookup_Current);
            }
            else
            {
                MessageBox.Show("Nhập đầy đủ thông tin mã Job và chẩn đoán!", eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public void btnNew()
        {
            Lookup_Current = new Lookup();
            Lookup_Current.DateActive = Globals.GetCurServerDateTime();
        }

        private void Lookup_InsertUpdate(Lookup Obj)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginLookup_Save(Obj, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndLookup_Save(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Duplex-Name":
                                        {
                                            MessageBox.Show(string.Format("{0} {1}!", "Tên danh mục đã tồn tại!", eHCMSResources.A1006_G1_Msg_DungTenKhac), eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            Lookup_ByType_Paging(CurrentLookupTree != null && CurrentLookupTree.NodeID > 0 ? (LookupValues)CurrentLookupTree.NodeID : 0);
                                            break;
                                        }
                                    case "Update-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A0608_G1_Msg_InfoHChinhFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Update-1":
                                        {
                                            Globals.EventAggregator.Publish(new Lookup_Event_Save() { Result = true });
                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            TryClose();
                                            break;
                                        }
                                    case "Insert-0":
                                        {
                                            MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Insert-1":
                                        {
                                            Globals.EventAggregator.Publish(new Lookup_Event_Save() { Result = true });
                                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
                                            TryClose();
                                            break;
                                        }
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
                    ClientLoggerHelper.LogError(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
    }
}
