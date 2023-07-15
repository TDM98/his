using eHCMSLanguage;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.BaseModel;

/*
 * #001 20180921 TNHX: Apply BusyIndicator, disable filter by RoomTypeID , refactor code
 */

namespace aEMR.Configuration.DeptLocation_ByDeptID.ViewModels
{
    [Export(typeof(IDeptLocation_ByDeptID)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DeptLocation_ByDeptIDViewModel : ViewModelBase, IDeptLocation_ByDeptID
        , IHandle<dgListDblClickSelectLocation_Event>
        , IHandle<dgListClickSelectionChanged_Event>
    {
        protected override void OnActivate()
        {
            Debug.WriteLine("OnActivate");
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            Debug.WriteLine("OnDeActivate");
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

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

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DeptLocation_ByDeptIDViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            //Load UC
            var UCLocations_ListFind = Globals.GetViewModel<ILocations_ListFind>();
            leftContent = UCLocations_ListFind;
            UCLocations_ListFind.hplAddNewVisible = Visibility.Collapsed;
            (this as ViewModelBase).ActivateItem(leftContent);
            //Load UC

            ObjDeptLocation_ByDeptID = new ObservableCollection<Location>();
        }

        private object _ObjKhoa_Current;
        public object ObjKhoa_Current
        {
            get { return _ObjKhoa_Current; }
            set
            {
                _ObjKhoa_Current = value;
                NotifyOfPropertyChange(() => ObjKhoa_Current);
            }
        }

        private ObservableCollection<DataEntities.RoomType> _ObjDeptLocation_GetRoomTypeByDeptID;
        public ObservableCollection<DataEntities.RoomType> ObjDeptLocation_GetRoomTypeByDeptID
        {
            get
            {
                return _ObjDeptLocation_GetRoomTypeByDeptID;
            }
            set
            {
                _ObjDeptLocation_GetRoomTypeByDeptID = value;
                NotifyOfPropertyChange(() => ObjDeptLocation_GetRoomTypeByDeptID);
            }
        }

        public void DeptLocation_GetRoomTypeByDeptID(Int64 DeptID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Loại Phòng..." });
            /*▼====: #001*/
            this.ShowBusyIndicator(eHCMSResources.K2993_G1_DSLgoaiPg);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeptLocation_GetRoomTypeByDeptID(DeptID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var items = contract.EndDeptLocation_GetRoomTypeByDeptID(asyncResult);
                                if (items != null)
                                {
                                    /*▼====: #001*/
                                    //ObjDeptLocation_GetRoomTypeByDeptID = new ObservableCollection<DataEntities.RoomType>(items);
                                    ObjDeptLocation_GetRoomTypeByDeptID = new ObservableCollection<DataEntities.RoomType>();
                                    /*▲====: #001*/
                                    //ItemDefault
                                    DataEntities.RoomType RoomTypeDefault = new DataEntities.RoomType();
                                    RoomTypeDefault.RmTypeID = -1;
                                    RoomTypeDefault.RmTypeName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.T0822_G1_TatCa);
                                    ObjDeptLocation_GetRoomTypeByDeptID.Insert(0, RoomTypeDefault);
                                    //ItemDefault
                                }
                                else
                                {
                                    ObjDeptLocation_GetRoomTypeByDeptID = null;
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
                /*▲====: #001*/
            });
            t.Start();
        }

        private string _TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }

        private ObservableCollection<DataEntities.RoomType> _ObjRoomType_GetAll_cbo;
        public ObservableCollection<DataEntities.RoomType> ObjRoomType_GetAll_cbo
        {
            get
            {
                return _ObjRoomType_GetAll_cbo;
            }
            set
            {
                _ObjRoomType_GetAll_cbo = value;
                NotifyOfPropertyChange(() => ObjRoomType_GetAll_cbo);
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
                                    ObjRoomType_GetAll_cbo = new ObservableCollection<DataEntities.RoomType>(items);
                                }
                                else
                                {
                                    ObjRoomType_GetAll_cbo = null;
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

        private object _Locations_SelectForAdd;
        public object Locations_SelectForAdd
        {
            get { return _Locations_SelectForAdd; }
            set
            {
                _Locations_SelectForAdd = value;
                NotifyOfPropertyChange(() => Locations_SelectForAdd);
            }
        }

        public void Handle(dgListDblClickSelectLocation_Event message)
        {
            var home = Globals.GetViewModel<IHome>();

            var activeItem = home.ActiveContent;

            if (message != null && ((aEMR.Configuration.ConfigurationModule.ViewModels.ConfigurationModuleViewModel)(activeItem)).MainContent is IRefDepartments)
            {
                Locations_SelectForAdd = message.Location_Current as Location;
                if (Locations_SelectForAdd != null)
                {
                    AddItem();
                }
            }
        }

        public void Handle(dgListClickSelectionChanged_Event message)
        {
            var home = Globals.GetViewModel<IHome>();

            var activeItem = home.ActiveContent;

            if (message != null && ((aEMR.Configuration.ConfigurationModule.ViewModels.ConfigurationModuleViewModel)(activeItem)).MainContent is IRefDepartments)
            {
                Locations_SelectForAdd = message.Location_Current as Location;
            }
        }

        private object _RoomType_SelectFind;
        public object RoomType_SelectFind
        {
            get { return _RoomType_SelectFind; }
            set
            {
                _RoomType_SelectFind = value;
                NotifyOfPropertyChange(() => RoomType_SelectFind);
            }
        }

        public void cboRoomTypeSelectedItemChanged(object selectedItem)
        {
            DeptLocation_ByDeptID((ObjKhoa_Current as DataEntities.RefDepartmentsTree).NodeID, (RoomType_SelectFind as DataEntities.RoomType).RmTypeID, LocationName);
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

        private string _LocationName;
        public string LocationName
        {
            get { return _LocationName; }
            set
            {
                _LocationName = value;
                NotifyOfPropertyChange(() => LocationName);
            }
        }

        public void DeptLocation_ByDeptID(Int64 DeptID, Int64 RmTypeID, string LocationName)
        {
            ObjDeptLocation_ByDeptID.Clear();

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Phòng..." });
            /*▼====: #001*/
            this.DlgShowBusyIndicator(eHCMSResources.K3054_G1_DSPg);
            var t = new Thread(() =>
            {
                try
                {
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
                /*▲====: #001*/
            });
            t.Start();
        }

        private bool b_btSearch1Click = false;
        public void btFind()
        {
            b_btSearch1Click = true;
            DeptLocation_ByDeptID((ObjKhoa_Current as DataEntities.RefDepartmentsTree).NodeID, (RoomType_SelectFind as DataEntities.RoomType).RmTypeID, LocationName);
        }

        public void btSaveItems()
        {
            if (ObjDeptLocation_ByDeptID != null && ObjDeptLocation_ByDeptID.Count > 0)
            {
                DeptLocation_XMLInsert((ObjKhoa_Current as DataEntities.RefDepartmentsTree).NodeID, ObjDeptLocation_ByDeptID);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg, eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
            }
        }

        public void DeptLocation_XMLInsert(Int64 DeptID, ObservableCollection<Location> objCollect)
        {
            bool Result = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            /*▼====: #001*/
            this.DlgShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeptLocation_XMLInsert(DeptID, objCollect, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Result = contract.EndDeptLocation_XMLInsert(asyncResult);
                                if (Result)
                                {
                                    Globals.EventAggregator.Publish(new DeptLocation_XMLInsert_Save_Event() { Result = true });
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
                /*▲====: #001*/
            });
            t.Start();
        }

        public void DeptLocation_MarkDeleted(Int64 DeptLocationID)
        {
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });
            /*▼====: #001*/
            this.DlgShowBusyIndicator(eHCMSResources.Z0343_G1_DangLuu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeptLocation_MarkDeleted(DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string Result = "";
                                contract.EndDeptLocation_MarkDeleted(out Result, asyncResult);
                                if (Result == "0")
                                {
                                    MessageBox.Show(eHCMSResources.K0484_G1_XoaFail, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
                                }
                                if (Result == "1")
                                {
                                    Globals.EventAggregator.Publish(new DeptLocation_MarkDeleted_Event() { Result = true });
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G2617_G1_Xoa, MessageBoxButton.OK);
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
                /*▲====: #001*/
            });
            t.Start();
        }

        public void btClose()
        {
            TryClose();
        }

        private bool b_Adding = false;
        public void btAddChoose()
        {
            if (Locations_SelectForAdd != null)
            {
                AddItem();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0355_G1_Msg_InfoChonPg, eHCMSResources.A0356_G1_Msg_ThemPg, MessageBoxButton.OK);
            }
        }

        private void AddItem()
        {
            b_Adding = true;

            if (b_btSearch1Click == false)
            {
                if (!ObjDeptLocation_ByDeptID.Contains(Locations_SelectForAdd as Location))
                {
                    ObjDeptLocation_ByDeptID.Add(Locations_SelectForAdd as Location);
                    Locations_SelectForAdd = null;
                    b_Adding = false;
                }
                else
                {
                    MessageBox.Show((Locations_SelectForAdd as Location).LocationName + string.Format(" {0}", eHCMSResources.A0071_G1_Msg_InfoItemIsSelected), eHCMSResources.G0156_G1_Them, MessageBoxButton.OK);
                }
            }
            else/*Có bấm nút Search ai biết list hiện tại là gì nên phải đọc lại list rồi add thêm vô*/
            {
                LocationName = "";
                DeptLocation_ByDeptID((ObjKhoa_Current as DataEntities.RefDepartmentsTree).NodeID, (RoomType_SelectFind as DataEntities.RoomType).RmTypeID, LocationName);
            }
        }

        public void hplDelete_Click(object selectedItem)
        {
            ObjDeptLocation_ByDeptID.Remove(selectedItem as Location);
        }
    }
}
