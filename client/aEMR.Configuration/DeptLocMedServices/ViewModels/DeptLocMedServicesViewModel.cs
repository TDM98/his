using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
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
using eHCMSLanguage;
using System.Windows.Controls;
using Castle.Windsor;
using aEMR.ViewContracts;
/*
 * #001 20180921 TNHX: Apply BusyIndicator, refactor code
 */
namespace aEMR.Configuration.DeptLocMedServices.ViewModels
{
    [Export(typeof(IDeptLocMedServices)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptLocMedServicesViewModel : Conductor<object>, IDeptLocMedServices
        , IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
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
        public DeptLocMedServicesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            authorization();
            SearchCriteria0 = new RefMedicalServiceItemsSearchCriteria();
            SearchCriteria0.MedicalServiceTypeID = -1;

            ObjDeptLocation_ByDeptID = new ObservableCollection<DeptLocation>();

            ObjDeptLocation_ByDeptID = new ObservableCollection<DeptLocation>();
            SearchCriteria1 = new RefMedicalServiceItemsSearchCriteria();
            SearchCriteria1.DeptLocationID = -1;

            ObjRefMedicalServiceItems_In_DeptLocMedServices = new ObservableCollection<RefMedicalServiceItem>();
            //Load UC
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            // 20181019 TNHX: [BM0003193] Update DepartmentTree so set it true to show list Phong
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = true;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            leftContent = UCRefDepartments_BystrV_DeptTypeViewModel;
            (this as Conductor<object>).ActivateItem(leftContent);
            //Load UC

            ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging = new PagedSortableCollectionView<RefMedicalServiceItem>();
            ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging_OnRefresh);
        }

        void ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.PageIndex,
                            ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.PageSize, false);
        }

        private PagedSortableCollectionView<DataEntities.RefMedicalServiceItem> _ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging;
        public PagedSortableCollectionView<DataEntities.RefMedicalServiceItem> ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging
        {
            get { return _ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging; }
            set
            {
                _ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging);
            }
        }

        private DataEntities.RefMedicalServiceItemsSearchCriteria _SearchCriteria0;
        public DataEntities.RefMedicalServiceItemsSearchCriteria SearchCriteria0
        {
            get
            {
                return _SearchCriteria0;
            }
            set
            {
                _SearchCriteria0 = value;
                NotifyOfPropertyChange(() => SearchCriteria0);
            }
        }

        //Col 1
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

        public void RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(Int64 DeptID, int V)
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

                                    //if (ObjRefMedicalServiceTypes_GetAll.Count > 1)
                                    //{
                                    //    //Tất Cả Để Tìm Cho Tiện
                                    //    RefMedicalServiceType ItemAll = new RefMedicalServiceType();
                                    //    ItemAll.MedicalServiceTypeID = 0;
                                    //    ItemAll.MedicalServiceTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    //    ObjRefMedicalServiceTypes_GetAll.Insert(1, ItemAll);
                                    //    //Tất Cả Để Tìm Cho Tiện
                                    //}
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
        //Col 1

        private void RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            if (CheckClickHeaderNotValid() == false)
                return;

            RefMedicalServiceItemsSearchCriteria SearchCriteria = SearchCriteria0;

            //Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Dịch Vụ Khám Bệnh..." });
            this.ShowBusyIndicator(eHCMSResources.K2974_G1_DSDVKB);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.RefMedicalServiceItem> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(out Total, asyncResult);
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

                            ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.Add(item);
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

        private bool CheckClickHeaderNotValid()
        {
            if (SearchCriteria0.DeptID > 0 && SearchCriteria0.MedicalServiceTypeID >= 0)
                return true;
            return false;
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplDelete = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mPhanBoTatCaDichVu_PhongCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDVPhong, (int)ePermission.mDelete);
            bbtAdd = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mPhanBoTatCaDichVu_PhongCuaKhoa,
                                               (int)oConfigurationEx.mQuanLyDVPhong, (int)ePermission.mAdd);
            bbtSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                           , (int)eConfiguration_Management.mPhanBoTatCaDichVu_PhongCuaKhoa,
                                           (int)oConfigurationEx.mQuanLyDVPhong, (int)ePermission.mView);

        }

        #region checking account
        private bool _bhplDelete = true;
        private bool _bbtAdd = true;
        private bool _bbtSearch = true;
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

        public bool bbtAdd
        {
            get
            {
                return _bbtAdd;
            }
            set
            {
                if (_bbtAdd == value)
                    return;
                _bbtAdd = value;
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
        #endregion

        #region binding visibilty
        public Button hplDelete { get; set; }

        public void hplDelete_Loaded(object sender)
        {
            hplDelete = sender as Button;
            hplDelete.Visibility = Globals.convertVisibility(bhplDelete);
        }        
        #endregion

        public void btSearch0()
        {
            if (SearchCriteria0.DeptID > 0)
            {
                if (SearchCriteria0.MedicalServiceTypeID > 0)
                {
                    //Ds Dịch Vụ
                    ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.PageIndex = 0;
                    RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(0, ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.PageSize, true);
                    //Ds Dịch Vụ
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0327_G1_Msg_InfoChonLoaiDV, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0320_G1_Msg_InfoChonKhoaKhacNutGoc, eHCMSResources.G1174_G1_TimKiem, MessageBoxButton.OK);
            }
        }

        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null)
            {
                DataEntities.RefDepartmentsTree NodeTree = message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;
                if (NodeTree.ParentID != null)
                {
                    SearchCriteria0.DeptID = NodeTree.NodeID;

                    SearchCriteria0.MedicalServiceTypeID = -1;
                    //Ds Loại DV
                    RefMedicalServiceItems_GroupMedicalServiceTypeIDByDeptID(SearchCriteria0.DeptID, 1);//1 Subtract DV Loại CLS
                    //Ds Loại DV

                    //Ds Phòng
                    GetAllDeptLocationByDeptID(NodeTree.NodeID);
                    //Ds Phòng

                    //Ds Phòng Text đợi sẵn Chọn Phòng
                    SearchCriteria1.DeptLocationID = -1;
                    //Ds Phòng Text đợi sẵn Chọn Phòng

                    //DV
                    SearchCriteria1.DeptID = NodeTree.NodeID;
                    //DV

                    //DS Dịch Vụ Trong Phòng Rỗng
                    ObjRefMedicalServiceItems_In_DeptLocMedServices.Clear();
                    //DS Dịch Vụ Trong Phòng Rỗng
                }
            }
        }

        private ObservableCollection<DeptLocation> _ObjDeptLocation_ByDeptID;
        public ObservableCollection<DeptLocation> ObjDeptLocation_ByDeptID
        {
            get { return _ObjDeptLocation_ByDeptID; }
            set
            {
                _ObjDeptLocation_ByDeptID = value;
                NotifyOfPropertyChange(() => ObjDeptLocation_ByDeptID);
            }
        }

        private void GetAllDeptLocationByDeptID(Int64 DeptID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.ShowBusyIndicator(eHCMSResources.Z1006_G1_LoadDSPhg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllDeptLocationByDeptID(DeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndGetAllDeptLocationByDeptID(asyncResult);

                                if (items != null)
                                {
                                    ObjDeptLocation_ByDeptID = new ObservableCollection<DataEntities.DeptLocation>(items);

                                //ItemDefault
                                DataEntities.DeptLocation ItemDefault = new DataEntities.DeptLocation();
                                    ItemDefault.DeptLocationID = -1;
                                    ItemDefault.Location = new Location();
                                    ItemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
                                    ObjDeptLocation_ByDeptID.Insert(0, ItemDefault);
                                //ItemDefault

                                //Tất Cả
                                DataEntities.DeptLocation ItemAll = new DataEntities.DeptLocation();
                                    ItemAll.DeptLocationID = 0;
                                    ItemAll.Location = new Location();
                                    ItemAll.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjDeptLocation_ByDeptID.Insert(1, ItemAll);
                                //Tất Cả
                            }
                                else
                                {
                                    ObjDeptLocation_ByDeptID = null;
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

        private DataEntities.RefMedicalServiceItemsSearchCriteria _SearchCriteria1;
        public DataEntities.RefMedicalServiceItemsSearchCriteria SearchCriteria1
        {
            get
            {
                return _SearchCriteria1;
            }
            set
            {
                _SearchCriteria1 = value;
                NotifyOfPropertyChange(() => SearchCriteria1);
            }
        }

        private ObservableCollection<DataEntities.RefMedicalServiceItem> _ObjRefMedicalServiceItems_In_DeptLocMedServices;
        public ObservableCollection<DataEntities.RefMedicalServiceItem> ObjRefMedicalServiceItems_In_DeptLocMedServices
        {
            get { return _ObjRefMedicalServiceItems_In_DeptLocMedServices; }
            set
            {
                _ObjRefMedicalServiceItems_In_DeptLocMedServices = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceItems_In_DeptLocMedServices);
            }
        }

        private void RefMedicalServiceItems_In_DeptLocMedServices(RefMedicalServiceItemsSearchCriteria SearchCriteria)
        {
            ObjRefMedicalServiceItems_In_DeptLocMedServices.Clear();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Phòng..." });
            this.ShowBusyIndicator(eHCMSResources.Z1006_G1_LoadDSPhg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndRefMedicalServiceItems_In_DeptLocMedServices(asyncResult);

                                if (items != null)
                                {
                                    ObjRefMedicalServiceItems_In_DeptLocMedServices = new ObservableCollection<DataEntities.RefMedicalServiceItem>(items);
                                    if (b_Adding)
                                    {
                                        if (b_btSearch1Click)
                                        {
                                            b_btSearch1Click = false;
                                            AddItem();
                                        }
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

        private bool b_btSearch1Click = false;
        public void btSearch1()
        {
            b_btSearch1Click = true;
            RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria1);
        }

        public void cboDeptLocationID_SelectedItemChanged(object selectedItem)
        {
            DataEntities.DeptLocation Item = (selectedItem as DataEntities.DeptLocation);
            if (Item != null)
            {
                if (Item.DeptLocationID >= 0)
                {
                    RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria1);
                }
                else
                {
                    ObjRefMedicalServiceItems_In_DeptLocMedServices.Clear();
                }
            }
        }

        public void hplDelete_Click(object selectedItem)
        {
            ObjRefMedicalServiceItems_In_DeptLocMedServices.Remove(selectedItem as RefMedicalServiceItem);
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            DataEntities.RefMedicalServiceItem ItemSelect = (eventArgs.Value as DataEntities.RefMedicalServiceItem);

            if (SearchCriteria1.DeptLocationID > 0)
            {
                AddItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        private DataEntities.RefMedicalServiceItem _ObjRefMedicalServiceItem_SelectedForAdd;
        public DataEntities.RefMedicalServiceItem ObjRefMedicalServiceItem_SelectedForAdd
        {
            get
            {
                return _ObjRefMedicalServiceItem_SelectedForAdd;
            }
            set
            {
                _ObjRefMedicalServiceItem_SelectedForAdd = value;
                NotifyOfPropertyChange(() => ObjRefMedicalServiceItem_SelectedForAdd);
            }
        }

        /*db click chọn*/
        public void dtgListSelectionChanged(object args)
        {
            if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems)).Length > 0)
            {
                if (((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0] != null)
                {
                    ObjRefMedicalServiceItem_SelectedForAdd = ((DataEntities.RefMedicalServiceItem)(((object[])(((System.Windows.Controls.SelectionChangedEventArgs)(args)).AddedItems))[0]));
                }
            }
        }
        /*db click chọn*/

        public void btAddChoose()
        {
            if (SearchCriteria1.DeptLocationID > 0)
            {
                AddItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.K2094_G1_ChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        private bool b_Adding = false;
        private void AddItem()
        {
            b_Adding = true;

            if (ObjRefMedicalServiceItem_SelectedForAdd != null)
            {
                if (b_btSearch1Click == false)
                {
                    if (!ObjRefMedicalServiceItems_In_DeptLocMedServices.Contains(ObjRefMedicalServiceItem_SelectedForAdd))
                    {
                        ObjRefMedicalServiceItems_In_DeptLocMedServices.Add(ObjRefMedicalServiceItem_SelectedForAdd);
                        b_Adding = false;
                    }
                    else
                    {
                        MessageBox.Show(ObjRefMedicalServiceItem_SelectedForAdd.MedServiceName + string.Format(eHCMSResources.A0071_G1_Msg_InfoItemIsSelected), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                }
                else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                {
                    SearchCriteria1.MedServiceCode = "";
                    SearchCriteria1.MedServiceName = "";
                    RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria1);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.K1939_G1_ChonDV, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void btAddAll()
        {
            if (SearchCriteria1.DeptLocationID > 0)
            {
                AddAllItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.K2094_G1_ChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }
        private void AddAllItem()
        {
            string warning = "";
            if (ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging != null && ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.Count > 0)
            {
                foreach (var item in ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging)
                if (b_btSearch1Click == false)
                {
                    if (!ObjRefMedicalServiceItems_In_DeptLocMedServices.Contains(item))
                    {
                        ObjRefMedicalServiceItems_In_DeptLocMedServices.Add(item);
                    }
                    else
                    {
                        warning += item.MedServiceName + string.Format(eHCMSResources.A0071_G1_Msg_InfoItemIsSelected) + Environment.NewLine;
                    }
                }
                else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                {
                    SearchCriteria1.MedServiceCode = "";
                    SearchCriteria1.MedServiceName = "";
                    RefMedicalServiceItems_In_DeptLocMedServices(SearchCriteria1);
                }
                if (!string.IsNullOrEmpty(warning))
                {
                    MessageBox.Show(warning, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z2934_G1_KhongCoDVDeThem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void btSaveItems()
        {
            if (ObjRefMedicalServiceItems_In_DeptLocMedServices != null && ObjRefMedicalServiceItems_In_DeptLocMedServices.Count >= 0)
            {
                DeptLocMedServices_XMLInsert(SearchCriteria1.DeptLocationID, ObjRefMedicalServiceItems_In_DeptLocMedServices);
            }
            else
            {
                //Globals.ShowMessage("Chọn Phòng", "Lưu");
                MessageBox.Show(eHCMSResources.K2094_G1_ChonPg, eHCMSResources.T2937_G1_Luu, MessageBoxButton.OK);
            }
        }

        private void DeptLocMedServices_XMLInsert(Int64 DeptLocationID, ObservableCollection<RefMedicalServiceItem> objCollect)
        {
            bool Result = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            this.ShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginDeptLocMedServices_XMLInsert(DeptLocationID, objCollect, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Result = contract.EndDeptLocMedServices_XMLInsert(asyncResult);
                                if (Result)
                                {
                                    MessageBox.Show(eHCMSResources.Z0655_G1_DaGhi, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0654_G1_GhiKgThCong, eHCMSResources.A0464_G1_Msg_Ghi, MessageBoxButton.OK);
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

        public void btClose()
        {
            TryClose();
        }

        public void cboMedicalServiceTypeAll_SelectionChanged(object selectItem)
        {
            if (selectItem != null)
            {
                if (SearchCriteria0.DeptID > 0)
                {
                    if (SearchCriteria0.MedicalServiceTypeID >= 0)
                    {
                        //Ds Dịch Vụ
                        ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.PageIndex = 0;
                        RefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging(0, ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.PageSize, true);
                        //Ds Dịch Vụ
                    }
                    else
                    {
                        //Ds Rỗng
                        ObjRefMedicalServiceItems_ByDeptIDMedicalServiceTypeID_Paging.Clear();
                        //Ds Rỗng
                    }
                }
            }
        }
    }
}
