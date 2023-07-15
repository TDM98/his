using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using aEMR.Common.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using aEMR.ViewContracts;
using Castle.Windsor;
/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.Configuration.RefMedicalServiceItems.ViewModels
{
    [Export(typeof(IDeptRefMedicalServiceItems)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptRefMedicalServiceItemsViewModel : Conductor<object>, IDeptRefMedicalServiceItems
        , IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
        , IHandle<RefMedicalServiceItems_NotPCL_Add_Event>
        , IHandle<MedServiceItemPrice_AddEditViewModel_Save_Event>
        , IHandle<SaveEvent<bool>>
    {
        private object _leftContent;
        public object leftContent
        {
            get
            {
                return _leftContent;
            }
            set
            {
                if (_leftContent == value)
                    return;
                _leftContent = value;
                NotifyOfPropertyChange(() => leftContent);
            }
        }

        [ImportingConstructor]
        public DeptRefMedicalServiceItemsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            Globals.EventAggregator.Subscribe(this);

            ObjRefMedicalServiceTypeSelected = new RefMedicalServiceType();
            ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID = -1;

            ObjTreeNodeRefDepartments_Current = new RefDepartmentsTree();

            SearchCriteria = new DeptMedServiceItemsSearchCriteria();
            SearchCriteria.MedicalServiceTypeID = -1;

            //Load UC
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = false;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            leftContent = UCRefDepartments_BystrV_DeptTypeViewModel;
            (this as Conductor<object>).ActivateItem(leftContent);
            //Load UC
            RefMedSerItemsSearchCriteria = new RefMedicalServiceItemsSearchCriteria();
            ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<RefMedicalServiceType>();

            curRefMedicalServiceItem = new RefMedicalServiceItem();
            curDeptMedServiceItems = new DataEntities.DeptMedServiceItems();

            ObjGetDeptMedServiceItems_Paging = new PagedSortableCollectionView<DataEntities.DeptMedServiceItems>();
            ObjGetDeptMedServiceItems_Paging.PageSize = 1000;
            ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
            ObjGetDeptMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjGetDeptMedServiceItems_Paging_OnRefresh);
            GetAllMedicalServiceTypes_SubtractPCL();
            ObjMedServiceItems_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjMedServiceItems_Paging_OnRefresh);

            allDeptMedServiceItems = new ObjectEdit<DataEntities.DeptMedServiceItems>("MedServiceID", "DeptID", "");
        }

        void ObjMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetMedServiceItems_Paging(ObjMedServiceItems_Paging.PageIndex, ObjMedServiceItems_Paging.PageSize, true);
        }

        void ObjGetDeptMedServiceItems_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetDeptMedServiceItems_Paging(ObjGetDeptMedServiceItems_Paging.PageIndex, ObjGetDeptMedServiceItems_Paging.PageSize, false);
        }

        private ObservableCollection<RefMedicalServiceType> _ObjRefMedicalServiceTypes_GetAll;
        public ObservableCollection<RefMedicalServiceType> ObjRefMedicalServiceTypes_GetAll
        {
            get { return _ObjRefMedicalServiceTypes_GetAll; }
            set
            {
                _ObjRefMedicalServiceTypes_GetAll = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceTypes_GetAll);
            }
        }

        private PagedSortableCollectionView<RefMedicalServiceItem> _ObjMedServiceItems_Paging;
        public PagedSortableCollectionView<RefMedicalServiceItem> ObjMedServiceItems_Paging
        {
            get { return _ObjMedServiceItems_Paging; }
            set
            {
                _ObjMedServiceItems_Paging = value;
                NotifyOfPropertyChange(() => ObjMedServiceItems_Paging);
            }
        }

        private RefMedicalServiceItem _curRefMedicalServiceItem;
        public RefMedicalServiceItem curRefMedicalServiceItem
        {
            get { return _curRefMedicalServiceItem; }
            set
            {
                _curRefMedicalServiceItem = value;
                NotifyOfPropertyChange(() => curRefMedicalServiceItem);
            }
        }

        private ObjectEdit<DataEntities.DeptMedServiceItems> _allDeptMedServiceItems;
        public ObjectEdit<DataEntities.DeptMedServiceItems> allDeptMedServiceItems
        {
            get { return _allDeptMedServiceItems; }
            set
            {
                _allDeptMedServiceItems = value;
                NotifyOfPropertyChange(() => allDeptMedServiceItems);
            }
        }

        private DataEntities.DeptMedServiceItems _curDeptMedServiceItems;
        public DataEntities.DeptMedServiceItems curDeptMedServiceItems
        {
            get { return _curDeptMedServiceItems; }
            set
            {
                _curDeptMedServiceItems = value;
                NotifyOfPropertyChange(() => curDeptMedServiceItems);
            }
        }

        public void RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
        {
            ObjRefMedicalServiceTypes_GetAll.Clear();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z0604_G1_DangLayDSLoaiDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(DeptID, V, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndRefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(asyncResult);
                                if (items != null)
                                {
                                    ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                    //Item Default
                                    ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);

                                    if (ObjRefMedicalServiceTypes_GetAll.Count > 1)
                                    {
                                        //Tất Cả Để Tìm Cho Tiện
                                        RefMedicalServiceType ItemAll = new RefMedicalServiceType();
                                        ItemAll.MedicalServiceTypeID = 0;
                                        ItemAll.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                        ObjRefMedicalServiceTypes_GetAll.Insert(1, ItemAll);
                                        //Tất Cả Để Tìm Cho Tiện
                                    }
                                }
                                else
                                {
                                    ObjRefMedicalServiceTypes_GetAll = null;
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

        public void GetAllMedicalServiceTypes_SubtractPCL()
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z0604_G1_DangLayDSLoaiDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllMedicalServiceTypes_SubtractPCL(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllMedicalServiceTypes_SubtractPCL(asyncResult);
                                if (items != null)
                                {
                                    ObjRefMedicalServiceTypes_GetAll = new ObservableCollection<DataEntities.RefMedicalServiceType>(items);

                                    //Item Default
                                    RefMedicalServiceType ItemDefault = new RefMedicalServiceType();
                                    ItemDefault.MedicalServiceTypeID = -1;
                                    ItemDefault.MedicalServiceTypeName = "--Chọn Loại Dịch Vụ--";
                                    //Item Default

                                    ObjRefMedicalServiceTypes_GetAll.Insert(0, ItemDefault);
                                }
                                else
                                {
                                    ObjRefMedicalServiceTypes_GetAll = null;
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

        private RefDepartmentsTree _ObjTreeNodeRefDepartments_Current = new RefDepartmentsTree();
        public RefDepartmentsTree ObjTreeNodeRefDepartments_Current
        {
            get { return _ObjTreeNodeRefDepartments_Current; }
            set
            {
                _ObjTreeNodeRefDepartments_Current = value;
                NotifyOfPropertyChange(() => ObjTreeNodeRefDepartments_Current);
            }
        }

        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null
                && ((RefDepartmentsTree)message.ObjRefDepartments_Current).NodeID > 0)
            {
                ObjTreeNodeRefDepartments_Current = message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;

                SearchCriteria.DeptID = ObjTreeNodeRefDepartments_Current.NodeID;
                ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID = -1;

                ////Xóa Lưới
                //ObjGetDeptMedServiceItems_Paging.Clear();
                ////Xóa Lưới

                //RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(SearchCriteria.DeptID, 1);//Subtract loại PCL
                SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                ObjGetDeptMedServiceItems_Paging.PageIndex = 0;

                GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
            }
        }

        private DeptMedServiceItemsSearchCriteria _SearchCriteria;
        public DeptMedServiceItemsSearchCriteria SearchCriteria
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

        private RefMedicalServiceItemsSearchCriteria _RefMedSerItemsSearchCriteria;
        public RefMedicalServiceItemsSearchCriteria RefMedSerItemsSearchCriteria
        {
            get
            {
                return _RefMedSerItemsSearchCriteria;
            }
            set
            {
                _RefMedSerItemsSearchCriteria = value;
                NotifyOfPropertyChange(() => RefMedSerItemsSearchCriteria);
            }
        }

        private PagedSortableCollectionView<DataEntities.DeptMedServiceItems> _ObjGetDeptMedServiceItems_Paging;
        public PagedSortableCollectionView<DataEntities.DeptMedServiceItems> ObjGetDeptMedServiceItems_Paging
        {
            get { return _ObjGetDeptMedServiceItems_Paging; }
            set
            {
                _ObjGetDeptMedServiceItems_Paging = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems_Paging);
            }
        }

        public void btAddChoose()
        {
            DataEntities.DeptMedServiceItems DeptMedSer = new DataEntities.DeptMedServiceItems();
            DeptMedSer.ObjRefMedicalServiceItem = curRefMedicalServiceItem;
            DeptMedSer.DeptID = ObjTreeNodeRefDepartments_Current.NodeID;
            DeptMedSer.MedServiceID = curRefMedicalServiceItem.MedServiceID;
            if (!allDeptMedServiceItems.Add(DeptMedSer))
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
            }
        }

        public void DoubleClick()
        {
            DataEntities.DeptMedServiceItems DeptMedSer = new DataEntities.DeptMedServiceItems();
            DeptMedSer.ObjRefMedicalServiceItem = curRefMedicalServiceItem;
            DeptMedSer.DeptID = ObjTreeNodeRefDepartments_Current.NodeID;
            DeptMedSer.MedServiceID = curRefMedicalServiceItem.MedServiceID;
            if (!allDeptMedServiceItems.Add(DeptMedSer))
            {
                MessageBox.Show(eHCMSResources.A0453_G1_Msg_InfoDaCoDV);
            }
        }

        public void btSaveItems()
        {
            if (allDeptMedServiceItems.DeleteObject != null
                && allDeptMedServiceItems.DeleteObject.Count > 0)
            {
                DeptMedServiceItems_DeleteXML(allDeptMedServiceItems.DeleteObject);
            }
            if (allDeptMedServiceItems.NewObject != null
                && allDeptMedServiceItems.NewObject.Count > 0)
            {
                DeptMedServiceItems_InsertXML(allDeptMedServiceItems.NewObject);
            }
        }

        private void GetDeptMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetDeptMedServiceItems_DeptIDPaging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.DeptMedServiceItems> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetDeptMedServiceItems_DeptIDPaging(out Total, asyncResult);
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
                                this.HideBusyIndicator();
                            }

                            ObjGetDeptMedServiceItems_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjGetDeptMedServiceItems_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjGetDeptMedServiceItems_Paging.Add(item);
                                    }
                                }
                                allDeptMedServiceItems = new ObjectEdit<DataEntities.DeptMedServiceItems>(ObjGetDeptMedServiceItems_Paging
                                    , "MedServiceID", "DeptID", "");
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private void GetMedServiceItems_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ..." });
            this.ShowBusyIndicator(eHCMSResources.Z1007_G1_LoadDSDV);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetMedServiceItems_Paging(RefMedSerItemsSearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<RefMedicalServiceItem> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetMedServiceItems_Paging(out Total, asyncResult);
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
                                this.HideBusyIndicator();
                            }

                            ObjMedServiceItems_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjMedServiceItems_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjMedServiceItems_Paging.Add(item);
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void DeptMedServiceItems_InsertXML(ObservableCollection<DataEntities.DeptMedServiceItems> lstDeptMedServiceItems)
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
                        contract.BeginDeptMedServiceItems_InsertXML(lstDeptMedServiceItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeptMedServiceItems_InsertXML(asyncResult))
                                {
                                    ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
                                    GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A1026_G1_Msg_InfoThemFail, "Thêm Dịch Vụ Cho Khoa", MessageBoxButton.OK);
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

        public void DeptMedServiceItems_DeleteXML(ObservableCollection<DataEntities.DeptMedServiceItems> lstDeptMedServiceItems)
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
                        contract.BeginDeptMedServiceItems_DeleteXML(lstDeptMedServiceItems, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeptMedServiceItems_DeleteXML(asyncResult))
                                {
                                    ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
                                    GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, "Xóa Dịch Vụ Cho Khoa", MessageBoxButton.OK);
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

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEditService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mEdit);
            bhplDeleteService = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mDelete);
            bhplListPrice = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mView);
            bhplAddNewRefMedicalServiceItemsView = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mQLDichVu_DonGiaCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDichVuDonGia, (int)ePermission.mAdd);
        }

        #region checking account
        private bool _bhplEditService = true;
        private bool _bhplDeleteService = true;
        private bool _bhplListPrice = true;
        private bool _bhplAddNewRefMedicalServiceItemsView = true;
        public bool bhplEditService
        {
            get
            {
                return _bhplEditService;
            }
            set
            {
                if (_bhplEditService == value)
                    return;
                _bhplEditService = value;
            }
        }

        public bool bhplDeleteService
        {
            get
            {
                return _bhplDeleteService;
            }
            set
            {
                if (_bhplDeleteService == value)
                    return;
                _bhplDeleteService = value;
            }
        }

        public bool bhplListPrice
        {
            get
            {
                return _bhplListPrice;
            }
            set
            {
                if (_bhplListPrice == value)
                    return;
                _bhplListPrice = value;
            }
        }

        public bool bhplAddNewRefMedicalServiceItemsView
        {
            get
            {
                return _bhplAddNewRefMedicalServiceItemsView;
            }
            set
            {
                if (_bhplAddNewRefMedicalServiceItemsView == value)
                    return;
                _bhplAddNewRefMedicalServiceItemsView = value;
            }
        }
        #endregion

        #region binding visibilty
        public Button hplEditService { get; set; }
        public Button hplDeleteService { get; set; }
        public Button hplListPrice { get; set; }

        public void hplEditService_Loaded(object sender)
        {
            hplEditService = sender as Button;
            hplEditService.Visibility = Globals.convertVisibility(bhplEditService);
        }

        public void hplDeleteService_Loaded(object sender)
        {
            hplDeleteService = sender as Button;
            hplDeleteService.Visibility = Globals.convertVisibility(bhplDeleteService);
        }

        public void hplListPrice_Loaded(object sender)
        {
            hplListPrice = sender as Button;
            hplListPrice.Visibility = Globals.convertVisibility(bhplListPrice);
        }
        #endregion

        public void btSearch()
        {
            //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
            {
                if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                {
                    //SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    //ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
                    //GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);

                    RefMedSerItemsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    ObjMedServiceItems_Paging.PageIndex = 0;
                    GetMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
                }
            }
        }

        //public void hplAddNewRefMedicalServiceItemsView()
        //{
        //    if (ObjTreeNodeRefDepartments_Current != null)
        //    {
        //        if (ObjTreeNodeRefDepartments_Current.ParentID > 0)
        //        {
        //            var typeInfo = Globals.GetViewModel<IDeptMedServiceItems_AddEdit>();
        //            DataEntities.DeptMedServiceItems ObjInit = new DataEntities.DeptMedServiceItems();

        //            ObjInit.ObjRefMedicalServiceItem = new RefMedicalServiceItem();
        //            ObjInit.ObjRefMedicalServiceItem.MedicalServiceTypeID = SearchCriteria.MedicalServiceTypeID;
        //            ObjInit.ObjRefMedicalServiceItem.ExpiryDate = DateTime.Now.AddYears(20);//Mặc định cứng 20  năm hết hạn dịch vụ này đã xử lý dưới SQL rồi

        //            ObjInit.ObjRefMedicalServiceItem.ObjMedicalServiceTypeID = new RefMedicalServiceType();
        //            ObjInit.ObjRefMedicalServiceItem.ObjMedicalServiceTypeID.V_RefMedicalServiceTypes = ObjRefMedicalServiceTypeSelected.V_RefMedicalServiceTypes;


        //            ObjInit.ObjMedServiceItemPrice = new DataEntities.MedServiceItemPrice();
        //            ObjInit.DeptID = ObjTreeNodeRefDepartments_Current.NodeID;

        //            typeInfo.ObjDeptMedServiceItems_Save = ObjInit;

        //            typeInfo.TitleForm = "Thêm Dịch Vụ Cho: " + ObjTreeNodeRefDepartments_Current.NodeText;

        //            typeInfo.ObjRefMedicalServiceTypeSelected = ObjRefMedicalServiceTypeSelected;

        //            typeInfo.Init(ObjInit);

        //            var instance = typeInfo as Conductor<object>;

        //            Globals.ShowDialog(instance, (o) =>
        //            {
        //                //làm gì đó
        //            });
        //        }
        //        else
        //        {
        //            MessageBox.Show("Chọn Khoa Để Thêm Dịch Vụ(Khác Nút Gốc)!", "Thêm Dịch Vụ", MessageBoxButton.OK);
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show(eHCMSResources.A0315_G1_Msg_InfoChonKhoaDeThemDV, "Thêm Dịch Vụ", MessageBoxButton.OK);
        //    }
        //}

        public void btAddNew()
        {
            Action<IDeptMedServiceItems_EditInfo> onInitDlg = delegate (IDeptMedServiceItems_EditInfo typeInfo)
            {
                typeInfo.TitleForm = eHCMSResources.G0298_G1_ThemMoiDV;
                typeInfo.isUpdate = false;

                typeInfo.ObjRefMedicalServiceTypeSelected = ObjRefMedicalServiceTypeSelected;

                typeInfo.ObjDeptMedServiceItems_Save = new DataEntities.DeptMedServiceItems();
            };
            GlobalsNAV.ShowDialog<IDeptMedServiceItems_EditInfo>(onInitDlg);
        }

        #region Nút trong dataGrid
        public void hplEditService_Click(object datacontext)
        {
            DataEntities.DeptMedServiceItems p = datacontext as DataEntities.DeptMedServiceItems;

            //Chỉ cho phép sửa Tên thôi
            Action<IDeptMedServiceItems_EditInfo> onInitDlg = delegate (IDeptMedServiceItems_EditInfo typeInfo)
            {
                DataEntities.DeptMedServiceItems ObjDeptMedServiceItemsEdit = new DataEntities.DeptMedServiceItems();
                //ObjDeptMedServiceItemsEdit = p.ObjDeptMedServiceItems;
                typeInfo.TitleForm = "Hiệu Chỉnh Thông Tin Dịch Vụ";
                typeInfo.isUpdate = true;
                typeInfo.ObjRefMedicalServiceTypeSelected = ObjRefMedicalServiceTypeSelected;

                typeInfo.ObjDeptMedServiceItems_Save = ObjectCopier.DeepCopy(ObjDeptMedServiceItemsEdit);
            };
            GlobalsNAV.ShowDialog<IDeptMedServiceItems_EditInfo>(onInitDlg);
        }

        public void hplDeleteService_Click(object datacontext)
        {
            DataEntities.DeptMedServiceItems p = datacontext as DataEntities.DeptMedServiceItems;

            //if (p.PriceType == "PriceFuture-Active-1")
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.ObjRefMedicalServiceItem.MedServiceName), eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    allDeptMedServiceItems.Remove(curDeptMedServiceItems);
                    //DeptMedServiceItems_TrueDelete(p.DeptMedServItemID, p.ObjRefMedicalServiceItem.MedServItemPriceID, p.MedServiceID);
                }
            }
            //else if (p.PriceType == "PriceCurrent")
            //{
            //    if (MessageBox.Show(string.Format("{0} ", eHCMSResources.A0156_G1_Msg_ConfXoaDV) + p.ObjMedServiceID.MedServiceName + ", Này Không?", "Xóa Dịch Vụ", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //    {
            //        RefMedicalServiceItems_MarkDeleted(p.MedServiceID);
            //    }
            //}
        }

        public void hplListPrice_Click(object datacontext)
        {
            DataEntities.MedServiceItemPrice p = datacontext as DataEntities.MedServiceItemPrice;

            if (ObjTreeNodeRefDepartments_Current != null)
            {
                if (ObjTreeNodeRefDepartments_Current.ParentID > 0)
                {
                    Action<IMedServiceItemPrice> onInitDlg = delegate (IMedServiceItemPrice typeInfo)
                    {

                        DataEntities.DeptMedServiceItems ItemDefault = new DataEntities.DeptMedServiceItems();
                        ItemDefault = p.ObjDeptMedServiceItems;

                        DataEntities.MedServiceItemPrice ObjPrice = SetValueObjPrice(p);
                        ItemDefault.ObjMedServiceItemPrice = ObjPrice;

                        typeInfo.IDeptMedServiceItems_Current = ItemDefault;

                        //typeInfo.TitleForm = " Thêm Dịch Vụ Cho:" + ObjTreeNodeRefDepartments_Current.NodeText;

                        MedServiceItemPriceSearchCriteria SearchCriteria = new MedServiceItemPriceSearchCriteria();
                        SearchCriteria.DeptMedServItemID = p.ObjDeptMedServiceItems.DeptMedServItemID;
                        SearchCriteria.V_TypePrice = 1;//Đang áp dụng
                        SearchCriteria.FromDate = null;
                        SearchCriteria.ToDate = null;

                        typeInfo.IStatusCheckFindDate = false;
                        typeInfo.ISearchCriteria = SearchCriteria;

                        typeInfo.IStatusFromDate = false;
                        typeInfo.IStatusToDate = false;

                        typeInfo.LoadForm();
                    };
                    GlobalsNAV.ShowDialog<IMedServiceItemPrice>(onInitDlg);
                }
                else
                {
                    MessageBox.Show("Chọn Khoa Để Thêm Dịch Vụ(Khác Nút Gốc)!", eHCMSResources.G0252_G1_ThemDV, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0315_G1_Msg_InfoChonKhoaDeThemDV, eHCMSResources.G0252_G1_ThemDV, MessageBoxButton.OK);
            }
        }

        public void DeptMedServiceItems_TrueDelete(Int64 DeptMedServItemID, Int64 MedServItemPriceID, Int64 MedServiceID)
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
                        contract.BeginDeptMedServiceItems_TrueDelete(DeptMedServItemID, MedServItemPriceID, MedServiceID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                if (contract.EndDeptMedServiceItems_TrueDelete(asyncResult))
                                {
                                    ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
                                    GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OK);
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

        public void RefMedicalServiceItems_MarkDeleted(Int64 MedServiceID)
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
                        contract.BeginRefMedicalServiceItems_MarkDeleted(MedServiceID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndRefMedicalServiceItems_MarkDeleted(out Result, asyncResult);
                                switch (Result)
                                {
                                    case "Delete-0":
                                        {
                                            MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                            break;
                                        }
                                    case "Delete-1":
                                        {
                                            ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
                                            GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                                            MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        private DataEntities.MedServiceItemPrice SetValueObjPrice(DataEntities.MedServiceItemPrice p)
        {
            DataEntities.MedServiceItemPrice ObjPrice = new DataEntities.MedServiceItemPrice();
            ObjPrice.MedServItemPriceID = p.MedServItemPriceID;
            ObjPrice.MedServiceID = p.MedServiceID;
            ObjPrice.StaffID = p.StaffID;
            ObjPrice.ApprovedStaffID = p.ApprovedStaffID;
            ObjPrice.VATRate = p.VATRate;
            ObjPrice.NormalPrice = p.NormalPrice;
            ObjPrice.PriceForHIPatient = p.PriceForHIPatient;
            ObjPrice.PriceDifference = p.PriceDifference;
            ObjPrice.HIAllowedPrice = p.HIAllowedPrice;
            ObjPrice.EffectiveDate = p.EffectiveDate;
            ObjPrice.IsActive = p.IsActive;
            ObjPrice.IsDeleted = p.IsDeleted;
            ObjPrice.Note = p.Note;
            return ObjPrice;
        }
        #endregion

        public void Handle(RefMedicalServiceItems_NotPCL_Add_Event message)
        {
            if (message != null)
            {
                //RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(SearchCriteria.DeptID, 1);//Subtract loại PCL
                ReLoadAfterSave();
            }
        }

        public void Handle(MedServiceItemPrice_AddEditViewModel_Save_Event message)
        {
            if (message != null)
            {
                ReLoadAfterSave();
            }
        }

        //public void cboMedicalServiceTypesSubTractPCL_SelectionChanged(object selectItem)
        //{
        //    if(selectItem!=null)
        //    {
        //        if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
        //        {
        //            if (SearchCriteria.MedicalServiceTypeID >= 0)
        //            {
        //                ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
        //                GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
        //            }
        //            else//-1 Text Chọn Loại Dịch Vụ
        //            {
        //                //Xóa Lưới
        //                ObjGetDeptMedServiceItems_Paging.Clear();
        //                //Xóa Lưới
        //            }
        //        }
        //    }
        //}

        public void Handle(SaveEvent<bool> message)
        {
            if (message != null)
            {
                if (message.Result)
                {
                    ReLoadAfterSave();
                }
            }
        }

        private void ReLoadAfterSave()
        {
            if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
            {
                if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                {
                    SearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                    ObjGetDeptMedServiceItems_Paging.PageIndex = 0;
                    GetDeptMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                }
            }
        }

        private RefMedicalServiceType _ObjRefMedicalServiceTypeSelected = new RefMedicalServiceType();
        public RefMedicalServiceType ObjRefMedicalServiceTypeSelected
        {
            get
            {
                return _ObjRefMedicalServiceTypeSelected;
            }
            set
            {
                if (_ObjRefMedicalServiceTypeSelected != value)
                {
                    _ObjRefMedicalServiceTypeSelected = value;
                    NotifyOfPropertyChange(() => ObjRefMedicalServiceTypeSelected);

                    //if (ObjTreeNodeRefDepartments_Current.NodeID > 0)
                    {
                        if (ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID > 0)
                        {
                            RefMedSerItemsSearchCriteria.MedicalServiceTypeID = ObjRefMedicalServiceTypeSelected.MedicalServiceTypeID;
                            ObjMedServiceItems_Paging.PageIndex = 0;
                            GetMedServiceItems_Paging(0, ObjGetDeptMedServiceItems_Paging.PageSize, true);
                        }
                        else//-1 Text Chọn Loại Dịch Vụ
                        {
                            //Xóa Lưới
                            if (ObjMedServiceItems_Paging == null)
                            {
                                ObjMedServiceItems_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
                            }
                            ObjMedServiceItems_Paging.Clear();
                            //Xóa Lưới
                        }
                    }
                }
            }
        }
    }
}
