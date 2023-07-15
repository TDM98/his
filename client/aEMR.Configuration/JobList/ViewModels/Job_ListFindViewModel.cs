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
namespace aEMR.Configuration.JobList.ViewModels
{
    [Export(typeof(IJob_ListFind)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Job_ListFindViewModel : ViewModelBase, IJob_ListFind
        , IHandle<Job_Event_Save>
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
        public Job_ListFindViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);

            SearchCriteria = "";



            ObjJob_Paging = new PagedSortableCollectionView<Lookup>();
            ObjJob_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjJob_Paging_OnRefresh);
        }

        void ObjJob_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            Job_ByIDCode_Paging(ObjJob_Paging.PageIndex,
                            ObjJob_Paging.PageSize, false);
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

        //public void Job_GetAll()
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
        //                contract.BeginJob_GetAll(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        var items = contract.EndJob_GetAll(asyncResult);
        //                        if (items != null)
        //                        {
        //                            ObjJob_GetAll = new ObservableCollection<DataEntities.Job>(items);
        //                            //ItemDefault
        //                            DataEntities.Job RoomTypeDefault = new DataEntities.Job();
        //                            RoomTypeDefault.IDCode = -1;
        //                            RoomTypeDefault.DiseaseNameVN = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                            ObjJob_GetAll.Insert(0, RoomTypeDefault);
        //                            //ItemDefault
        //                        }
        //                        else
        //                        {
        //                            ObjJob_GetAll = null;
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

        private PagedSortableCollectionView<Lookup> _ObjJob_Paging;
        public PagedSortableCollectionView<Lookup> ObjJob_Paging
        {
            get { return _ObjJob_Paging; }
            set
            {
                _ObjJob_Paging = value;
                NotifyOfPropertyChange(() => ObjJob_Paging);
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
            ObjJob_Paging.PageIndex = 0;
            Job_ByIDCode_Paging(0, ObjJob_Paging.PageSize, true);
        }

        public void Job_MarkDeleted(Int64 IDCode)
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
            //            contract.BeginJob_MarkDelete(IDCode, Globals.DispatchCallback((asyncResult) =>
            //            {
            //                try
            //                {
            //                    contract.EndJob_MarkDelete(out Result, asyncResult);
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
            //                        ObjJob_Paging.PageIndex = 0;
            //                        Job_ByIDCode_Paging(0, ObjJob_Paging.PageSize, true);
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
            //        Job_MarkDeleted(p.IDCode);
            //    }
            //}
        }

        private object _Job_Current;
        public object Job_Current
        {
            get { return _Job_Current; }
            set
            {
                _Job_Current = value;
                NotifyOfPropertyChange(() => Job_Current);
            }
        }

        public void DoubleClick(object args)
        {
            //EventArgs<object> eventArgs = args as EventArgs<object>;
            //Job_Current = eventArgs.Value as Lookup;
            //Globals.EventAggregator.Publish(new dgJobListClickSelectionChanged_Event() { Job_Current = eventArgs.Value });
        }

        public void dtgListSelectionChanged(object args)
        {
            //if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            //{
            //    if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
            //    {
            //        Job_Current =
            //            ((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0];
            //        var typeInfo = Globals.GetViewModel<IJob_ListFind>();
            //        typeInfo.Job_Current = (Job)Job_Current;

            //        Globals.EventAggregator.Publish(new dgJobListClickSelectionChanged_Event()
            //        {
            //            Job_Current = ((object[]) (((System.Windows.Controls.SelectionChangedEventArgs) (args)).AddedItems))[0]
            //        });
            //    }
            //}
        }

        private void Job_ByIDCode_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.DlgShowBusyIndicator("Danh sách nghề");
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginJob_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<Lookup> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndJob_Paging(out Total, asyncResult);
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

                            ObjJob_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjJob_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjJob_Paging.Add(item);
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
            Action<IJob_AddEdit> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Job";
                typeInfo.InitializeNewItem();
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                Action<IJob_AddEdit> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjJob_Current = ObjectCopier.DeepCopy((selectedItem as Lookup));
                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as Lookup).ObjectValue.Trim() + ")";
                };
                GlobalsNAV.ShowDialog<IJob_AddEdit>(onInitDlg);
            }
        }

        public void Handle(Job_Event_Save message)
        {
            ObjJob_Paging.PageIndex = 0;
            Job_ByIDCode_Paging(0, ObjJob_Paging.PageSize, true);
        }
    }
}
