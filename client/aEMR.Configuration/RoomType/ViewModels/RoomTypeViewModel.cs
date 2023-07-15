using eHCMSLanguage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using System.Threading;
using DataEntities;
using System;
using aEMR.Common;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.Configuration.RoomType.ViewModels
{
    
    [Export(typeof(IRoomType)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoomTypeViewModel : Conductor<object>, IRoomType
        ,IHandle<RoomTypeEvent_Save>
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading!=value)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RoomTypeViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            authorization();

            V_RoomFunction_Selected = -1;
            Load_V_RoomFunction();

            SearchCriteria = new RoomTypeSearchCriteria();
            SearchCriteria.V_RoomFunction = V_RoomFunction_Selected;
            SearchCriteria.RmTypeName = "";
            SearchCriteria.OrderBy = "";
            ObjRoomType_GetList_Paging=new PagedSortableCollectionView<DataEntities.RoomType>();
            ObjRoomType_GetList_Paging.OnRefresh += new EventHandler<RefreshEventArgs>(ObjRoomType_GetList_Paging_OnRefresh);
           
        }

        void ObjRoomType_GetList_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            RoomType_GetList_Paging(ObjRoomType_GetList_Paging.PageIndex,ObjRoomType_GetList_Paging.PageSize, false);
        }

        private RoomTypeSearchCriteria _SearchCriteria;
        public RoomTypeSearchCriteria SearchCriteria
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

        private PagedSortableCollectionView<DataEntities.RoomType> _ObjRoomType_GetList_Paging;
        public PagedSortableCollectionView<DataEntities.RoomType> ObjRoomType_GetList_Paging
        {
            get { return _ObjRoomType_GetList_Paging; }
            set
            {
                _ObjRoomType_GetList_Paging = value;
                NotifyOfPropertyChange(() => ObjRoomType_GetList_Paging);
            }
        }


        private Int64 _V_RoomFunction_Selected;
        public Int64 V_RoomFunction_Selected
        {
            get { return _V_RoomFunction_Selected; }
            set
            {
                _V_RoomFunction_Selected = value;
                NotifyOfPropertyChange(()=>V_RoomFunction_Selected);
            }
        }


        public void cboV_RoomFunction_SelectedItemChanged(object selectedItem)
        {
            if (selectedItem != null)
            {
                V_RoomFunction_Selected = (selectedItem as DataEntities.Lookup).LookupID;
                SearchCriteria.V_RoomFunction = V_RoomFunction_Selected;
                ObjRoomType_GetList_Paging.PageIndex = 0;
                RoomType_GetList_Paging(0, ObjRoomType_GetList_Paging.PageSize, true);
            }
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bhplEdit = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucLoaiPhong
                                               ,(int)oConfigurationEx.mQuanLyDSLoaiPhong, (int)ePermission.mEdit);
            bhplDelete= Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                               , (int)eConfiguration_Management.mDanhMucLoaiPhong
                                               ,(int)oConfigurationEx.mQuanLyDSLoaiPhong, (int)ePermission.mDelete);
            bbtSearch= Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mDanhMucLoaiPhong
                                            ,(int)oConfigurationEx.mQuanLyDSLoaiPhong, (int)ePermission.mView);
            bhplAddNew = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mConfiguration_Management
                                            , (int)eConfiguration_Management.mDanhMucLoaiPhong
                                            , (int)oConfigurationEx.mQuanLyDSLoaiPhong, (int)ePermission.mAdd);

        }
        #region checking account

        private bool _bhplEdit = true;
        private bool _bhplDelete = true;
        private bool _bbtSearch= true;
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

        public Button hplEdit{ get; set; }
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
        public void hplAddNewRoomType_Click()
        {
            //Screen screen = new Screen();

            //var typeInfo = Globals.GetViewModel<IRoomTypeInfo>();
            //typeInfo.TitleForm = "Thêm Mới Loại Phòng";
            //typeInfo.ObjV_RoomFunction = ObjV_RoomFunction;

            //typeInfo.InitializeNewItem(V_RoomFunction_Selected);

            //var instance =  typeInfo as Conductor<object>;
            
            //Globals.ShowDialog(instance, (o)=>
            //                                 {
            //                                     //làm gì đó
            //                                 });

            Action<IRoomTypeInfo> onInitDlg = (typeInfo) =>
            {
                typeInfo.TitleForm = "Thêm Mới Loại Phòng";
                typeInfo.ObjV_RoomFunction = ObjV_RoomFunction;

                typeInfo.InitializeNewItem(V_RoomFunction_Selected);
            };
            GlobalsNAV.ShowDialog<IRoomTypeInfo>(onInitDlg);
        }

        public void hplEdit_Click(object selectedItem)
        {
            if (selectedItem != null)
            {
                //var typeInfo = Globals.GetViewModel<IRoomTypeInfo>();
                //typeInfo.ObjRoomType_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.RoomType));

                //typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.RoomType).RmTypeName.Trim() + ")";
                //typeInfo.ObjV_RoomFunction = ObjV_RoomFunction;

                //var instance = typeInfo as Conductor<object>;

                //Globals.ShowDialog(instance, (o) =>
                //                                 {
                //                                     //làm gì đó
                //                                 });

                Action<IRoomTypeInfo> onInitDlg = (typeInfo) =>
                {
                    typeInfo.ObjRoomType_Current = ObjectCopier.DeepCopy((selectedItem as DataEntities.RoomType));

                    typeInfo.TitleForm = "Hiệu Chỉnh (" + (selectedItem as DataEntities.RoomType).RmTypeName.Trim() + ")";
                    typeInfo.ObjV_RoomFunction = ObjV_RoomFunction;
                };
                GlobalsNAV.ShowDialog<IRoomTypeInfo>(onInitDlg);

            }
        }

        public void btFind()
        {
            ObjRoomType_GetList_Paging.PageIndex = 0;
            RoomType_GetList_Paging(0,ObjRoomType_GetList_Paging.PageSize, true);
        }

        private void RoomType_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            var t = new Thread(() =>
            {
                IsLoading = true; 

                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginRoomType_GetList_Paging(SearchCriteria, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.RoomType> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndRoomType_GetList_Paging(out Total, asyncResult);
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

                            ObjRoomType_GetList_Paging.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjRoomType_GetList_Paging.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjRoomType_GetList_Paging.Add(item);
                                    }

                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    //Globals.IsBusy = false;
                    IsLoading = false;
                }
            });
            t.Start();
        }


        private ObservableCollection<Lookup> _ObjV_RoomFunction;
        public ObservableCollection<Lookup> ObjV_RoomFunction
        {
            get { return _ObjV_RoomFunction; }
            set
            {
                _ObjV_RoomFunction = value;
                NotifyOfPropertyChange(() => ObjV_RoomFunction);
            }
        }

        public void Load_V_RoomFunction()
        {
            var t = new Thread(() =>
            {
                //Globals.EventAggregator.Publish(new BusyEvent{IsBusy = true,Message ="Danh Sách Loại Chức Năng..."});

                IsLoading = true;

                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RoomFunction,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RoomFunction = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjV_RoomFunction.Insert(0, firstItem);
                                }
                                catch (FaultException<AxException> fault)
                                {
                                    ClientLoggerHelper.LogInfo(fault.ToString());
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogInfo(ex.ToString());
                                }

                            }), null);
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
            });
            t.Start();
        }


        public void hplDelete_Click(object selectedItem)
        {
            DataEntities.RoomType p = (selectedItem as DataEntities.RoomType);

            if (p!=null && p.RmTypeID>0)
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, p.RmTypeName), eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    RoomType_MarkDelete(p.RmTypeID);
                }
            }

        }
        private void RoomType_MarkDelete(Int64 RmTypeID)
        {
            string Result = "";

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Xóa..." });

            var t = new Thread(() =>
            {
                IsLoading = true;

                using (var serviceFactory = new ConfigurationManagerServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginRoomType_MarkDelete(RmTypeID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            contract.EndRoomType_MarkDelete(out Result, asyncResult);
                            switch (Result)
                            {
                                case "Delete-0":
                                    {
                                        MessageBox.Show("Xóa Không Thành Công!", eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                        break; 
                                    }
                                case "Delete-1":
                                    {
                                        ObjRoomType_GetList_Paging.PageIndex = 0;
                                        RoomType_GetList_Paging(0, ObjRoomType_GetList_Paging.PageSize, true);
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
                            //Globals.IsBusy = false;
                            IsLoading = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }
        

        public void Handle(RoomTypeEvent_Save message)
        {
            if(message!=null)
            {
                btFind();
            }
        }
    }
}
