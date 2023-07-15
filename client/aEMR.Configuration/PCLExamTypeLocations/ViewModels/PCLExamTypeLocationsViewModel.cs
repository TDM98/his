using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.Configuration.PCLExamTypeLocations.ViewModels
{
    [Export(typeof(IPCLExamTypeLocations)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypeLocationsViewModel : Conductor<object>, IPCLExamTypeLocations
        , IHandle<RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event>
        , IHandle<DbClickSelectedObjectEvent<PCLExamType>>
        , IHandle<SelectedObjectEvent<PCLExamType>>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private object _ContentPCLExamTypes;
        public object ContentPCLExamTypes
        {
            get
            {
                return _ContentPCLExamTypes;
            }
            set
            {
                if (_ContentPCLExamTypes != value)
                {
                    _ContentPCLExamTypes = value;
                    NotifyOfPropertyChange(() => ContentPCLExamTypes);
                }
            }
        }

        private object _ContentRefDepartments;
        public object ContentRefDepartments
        {
            get
            {
                return _ContentRefDepartments;
            }
            set
            {
                if (_ContentRefDepartments != value)
                {
                    _ContentRefDepartments = value;
                    NotifyOfPropertyChange(() => ContentRefDepartments);
                }
            }
        }
        [ImportingConstructor]
        public PCLExamTypeLocationsViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);

            //Load UC
            var UCPCLExamTypes = Globals.GetViewModel<IPCLExamTypes_List_Paging>();
            ContentPCLExamTypes = UCPCLExamTypes;
            (this as Conductor<object>).ActivateItem(ContentPCLExamTypes);
            UCPCLExamTypes.SearchCriteria = new PCLExamTypeSearchCriteria();
            UCPCLExamTypes.SearchCriteria.IsNotInPCLExamTypeLocations = true;    
            UCPCLExamTypes.SearchCriteria.IsNotInPCLItems = true;
            UCPCLExamTypes.IsNotInPCLItemsVisibility = Visibility.Collapsed;
            UCPCLExamTypes.FormLoad();
            
            var UCRefDepartments_BystrV_DeptTypeViewModel = Globals.GetViewModel<IRefDepartments_BystrV_DeptType>();
            UCRefDepartments_BystrV_DeptTypeViewModel.strV_DeptType = "7000";
            UCRefDepartments_BystrV_DeptTypeViewModel.ShowDeptLocation = false;
            UCRefDepartments_BystrV_DeptTypeViewModel.Parent = this;
            UCRefDepartments_BystrV_DeptTypeViewModel.RefDepartments_Tree();
            ContentRefDepartments = UCRefDepartments_BystrV_DeptTypeViewModel;
            (this as Conductor<object>).ActivateItem(ContentRefDepartments);
            //Load UC
            
            //ObjDeptLocation_GetRoomTypeByDeptID = new ObservableCollection<DataEntities.RoomType>();
            //ObjDeptLocation_GetRoomTypeByDeptID_Selected = new DataEntities.RoomType();

            ObjDeptLocation_ByDeptID=new ObservableCollection<Location>();
            ObjDeptLocation_ByDeptID_Selected=new Location();

            ObjPCLExamTypeLocations_ByDeptLocationID=new ObservableCollection<PCLExamType>();
        }

        private DataEntities.RefDepartmentsTree _ObjKhoa_Current;
        public DataEntities.RefDepartmentsTree ObjKhoa_Current
        {
            get { return _ObjKhoa_Current; }
            set
            {
                _ObjKhoa_Current = value;
                NotifyOfPropertyChange(() => ObjKhoa_Current);
            }
        }

        //#region Loại Phòng
        //private DataEntities.RoomType _ObjDeptLocation_GetRoomTypeByDeptID_Selected;
        //public DataEntities.RoomType ObjDeptLocation_GetRoomTypeByDeptID_Selected
        //{
        //    get
        //    {
        //        return _ObjDeptLocation_GetRoomTypeByDeptID_Selected;
        //    }
        //    set
        //    {
        //        _ObjDeptLocation_GetRoomTypeByDeptID_Selected = value;
        //        NotifyOfPropertyChange(() => ObjDeptLocation_GetRoomTypeByDeptID_Selected);
        //    }
        //}
        //private ObservableCollection<DataEntities.RoomType> _ObjDeptLocation_GetRoomTypeByDeptID;
        //public ObservableCollection<DataEntities.RoomType> ObjDeptLocation_GetRoomTypeByDeptID
        //{
        //    get
        //    {
        //        return _ObjDeptLocation_GetRoomTypeByDeptID;
        //    }
        //    set
        //    {
        //        _ObjDeptLocation_GetRoomTypeByDeptID = value;
        //        NotifyOfPropertyChange(() => ObjDeptLocation_GetRoomTypeByDeptID);
        //    }
        //}
        //public void DeptLocation_GetRoomTypeByDeptID(Int64 DeptID)
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });

        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ConfigurationManagerServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginDeptLocation_GetRoomTypeByDeptID(DeptID, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var items = contract.EndDeptLocation_GetRoomTypeByDeptID(asyncResult);


        //                    if (items != null)
        //                    {
        //                        ObjDeptLocation_GetRoomTypeByDeptID = new ObservableCollection<DataEntities.RoomType>(items);

        //                        //ItemDefault
        //                        DataEntities.RoomType ItemDefault = new DataEntities.RoomType();
        //                        ItemDefault.RmTypeID = -1;
        //                        ItemDefault.RmTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
        //                        ObjDeptLocation_GetRoomTypeByDeptID.Insert(0, ItemDefault);
        //                        //ItemDefault

        //                        ObjDeptLocation_GetRoomTypeByDeptID_Selected = ItemDefault;
        //                    }
        //                    else
        //                    {
        //                        ObjDeptLocation_GetRoomTypeByDeptID = null;
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
        //#endregion

        //public void cboRoomTypeSelectedItemChanged(object selectedItem)
        //{
        //    DeptLocation_ByDeptID(ObjKhoa_Current.NodeID, ObjDeptLocation_GetRoomTypeByDeptID_Selected.RmTypeID, "");
        //}

        #region Phòng
        private Location _ObjDeptLocation_ByDeptID_Selected;
        public Location ObjDeptLocation_ByDeptID_Selected
        {
            get { return _ObjDeptLocation_ByDeptID_Selected; }
            set
            {
                _ObjDeptLocation_ByDeptID_Selected = value;
                NotifyOfPropertyChange(() => ObjDeptLocation_ByDeptID_Selected);

            }
        }

        private ObservableCollection<Location> _ObjDeptLocation_ByDeptID;
        public ObservableCollection<Location> ObjDeptLocation_ByDeptID
        {
            get { return _ObjDeptLocation_ByDeptID; }
            set
            {
                _ObjDeptLocation_ByDeptID = value;
                NotifyOfPropertyChange(() => ObjDeptLocation_ByDeptID);

            }
        }
        public void DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Phòng..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginDeptLocation_ByDeptID(DeptID, RmTypeID, LocationName, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndDeptLocation_ByDeptID(asyncResult);

                            if (items != null)
                            {
                                ObjDeptLocation_ByDeptID = new ObservableCollection<DataEntities.Location>(items);
                                
                                //ItemDefault
                                DataEntities.Location ItemDefault = new DataEntities.Location();
                                ItemDefault.LID = -1;
                                ItemDefault.DeptLocationID=-1;
                                ItemDefault.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.K2094_G1_ChonPg);
                                ObjDeptLocation_ByDeptID.Insert(0, ItemDefault);
                                //ItemDefault

                                ObjDeptLocation_ByDeptID_Selected = ItemDefault;
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        #endregion

        public void Handle(RefDepartments_BystrV_DeptTypeEvent.TreeSelectionChanged_Event message)
        {
            if (message != null)
            {
                DataEntities.RefDepartmentsTree NodeTree =message.ObjRefDepartments_Current as DataEntities.RefDepartmentsTree;
                if (NodeTree.ParentID != null)
                {
                    ObjKhoa_Current = ObjectCopier.DeepCopy(NodeTree);
                    //DeptLocation_GetRoomTypeByDeptID(ObjKhoa_Current.NodeID);
                    DeptLocation_ByDeptID(ObjKhoa_Current.NodeID, -1,"");
                    ObjPCLExamTypeLocations_ByDeptLocationID.Clear();
                }
            }
        }
        
        //Loại Phòng Change
        public void cboDeptLocationID_SelectedItemChanged(object selectedItem)
        {
            DataEntities.DeptLocation Item = (selectedItem as DataEntities.DeptLocation);
            if (Item != null)
            {
                if (Item.DeptLocationID >= 0)
                {
                    DeptLocation_ByDeptID(ObjKhoa_Current.NodeID, Item.Location.RmTypeID.Value,"");
                }
            }
        }
        //Loại Phòng Change


        //Phòng Change
        public void cboObjDeptLocation_ByDeptID_SelectionChanged(object selectedItem)
        {
            DataEntities.Location Item = (selectedItem as DataEntities.Location);
            if (Item != null)
            {
                if (Item.DeptLocationID >= 0)
                {
                    PCLExamTypeLocations_ByDeptLocationID();
                }
            }
        }
        //Phòng Change


        private PCLExamType _ObjPCLExamType_SelectForAdd;
        public PCLExamType ObjPCLExamType_SelectForAdd
        {
            get { return _ObjPCLExamType_SelectForAdd; }
            set
            {
                _ObjPCLExamType_SelectForAdd = value;
                NotifyOfPropertyChange(() => ObjPCLExamType_SelectForAdd);
            }
        }


        private string _PCLExamTypeName;
        public string PCLExamTypeName
        {
            get { return _PCLExamTypeName; }
            set
            {
                if(_PCLExamTypeName!=value)
                {
                    _PCLExamTypeName = value;
                    NotifyOfPropertyChange(()=>PCLExamTypeName);
                }
            }
        }

        private ObservableCollection<PCLExamType> _ObjPCLExamTypeLocations_ByDeptLocationID;
        public ObservableCollection<PCLExamType> ObjPCLExamTypeLocations_ByDeptLocationID
        {
            get { return _ObjPCLExamTypeLocations_ByDeptLocationID; }
            set
            {
                _ObjPCLExamTypeLocations_ByDeptLocationID = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypeLocations_ByDeptLocationID);

            }
        }
        public void PCLExamTypeLocations_ByDeptLocationID()
        {
            ObjPCLExamTypeLocations_ByDeptLocationID.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách PCLExamType..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeLocations_ByDeptLocationID(PCLExamTypeName,ObjDeptLocation_ByDeptID_Selected.DeptLocationID,  Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndPCLExamTypeLocations_ByDeptLocationID(asyncResult);

                            if (items != null)
                            {
                                ObjPCLExamTypeLocations_ByDeptLocationID = new ObservableCollection<DataEntities.PCLExamType>(items);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void btAddChoose()
        {
            if (ObjPCLExamType_SelectForAdd != null)
            {
                AddItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public void btSaveItems()
        {
            if (ObjDeptLocation_ByDeptID_Selected == null)
                return;
            if (ObjDeptLocation_ByDeptID_Selected.DeptLocationID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (ObjPCLExamTypeLocations_ByDeptLocationID != null && ObjPCLExamTypeLocations_ByDeptLocationID.Count > 0)
            {
                PCLExamTypeLocations_XMLInsert(ObjDeptLocation_ByDeptID_Selected.DeptLocationID, ObjPCLExamTypeLocations_ByDeptLocationID);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0347_G1_Msg_InfoChonPCLExamType, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void PCLExamTypeLocations_XMLInsert(Int64 DeptLocationID, IEnumerable<PCLExamType> ObjList)
        {
            bool Result = false;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeLocations_XMLInsert(DeptLocationID, ObjPCLExamTypeLocations_ByDeptLocationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            Result = contract.EndPCLExamTypeLocations_XMLInsert(asyncResult);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void hplDelete_Click(object selectedItem)
        {
            if(ObjDeptLocation_ByDeptID_Selected==null || ObjDeptLocation_ByDeptID_Selected.DeptLocationID<=0)
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg);
                return;
            }

            PCLExamType p = selectedItem as PCLExamType;
            if (p != null && p.PCLExamTypeID > 0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.PCLExamTypeName.Trim()), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    PCLExamTypeLocations_MarkDeleted(p.PCLExamTypeID, ObjDeptLocation_ByDeptID_Selected.DeptLocationID);
                }
            }

            //ObjPCLExamTypeLocations_ByDeptLocationID.Remove(selectedItem as PCLExamType);
        }

        private void PCLExamTypeLocations_MarkDeleted(Int64 PCLExamTypeID, Int64 DeptLocationID)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginPCLExamTypeLocations_MarkDeleted(PCLExamTypeID,DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndPCLExamTypeLocations_MarkDeleted(out Result, asyncResult);
                            switch (Result)
                            {
                                case "0":
                                    {
                                        MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break;
                                    }
                                case "1":
                                    {
                                        //ObjRoomType_GetList_Paging.PageIndex = 0;
                                        //RoomType_GetList_Paging(0, ObjRoomType_GetList_Paging.PageSize, true);
                                        //MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        btSearch1();
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        public void Handle(SelectedObjectEvent<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    ObjPCLExamType_SelectForAdd = message.Result;
                }
            }
        }

        
        public void Handle(DbClickSelectedObjectEvent<PCLExamType> message)
        {
            if (this.GetView() != null)
            {
                if (message != null)
                {
                    ObjPCLExamType_SelectForAdd = message.Result;
                    AddItem();
                }
            }
        }

        private bool b_Adding = false;
        private void AddItem()
        {
            if (this.GetView() != null)
            {
                b_Adding = true;

                if (ObjDeptLocation_ByDeptID_Selected == null)
                    return;
                if (ObjDeptLocation_ByDeptID_Selected.DeptLocationID <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                
                if (ObjPCLExamType_SelectForAdd != null)
                {
                    if (b_btSearch1Click == false)
                    {
                        if (!ObjPCLExamTypeLocations_ByDeptLocationID.Contains(ObjPCLExamType_SelectForAdd))
                        {
                            ObjPCLExamTypeLocations_ByDeptLocationID.Add(ObjPCLExamType_SelectForAdd);
                            b_Adding = false;
                        }
                        else
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z0357_G1_DVDaChonRoi, ObjPCLExamType_SelectForAdd.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                    }
                    else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
                    {
                        PCLExamTypeName = "";
                        PCLExamTypeLocations_ByDeptLocationID();
                    }
                }
            }
        }

        private bool b_btSearch1Click = false;
        public void btSearch1()
        {
            b_btSearch1Click = true;
            if (ObjDeptLocation_ByDeptID_Selected.DeptLocationID >0)
            {
                PCLExamTypeLocations_ByDeptLocationID();
            }
            else
            {
                MessageBox.Show(eHCMSResources.K2094_G1_ChonPg);
            }
        }
    }
}
