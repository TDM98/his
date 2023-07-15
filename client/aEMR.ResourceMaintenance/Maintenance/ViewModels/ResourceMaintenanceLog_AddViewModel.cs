using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.ResourcesManage.Maintenance;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using Caliburn.Micro;
using DataEntities;
using System.Collections.Generic;
using aEMR.Common;
using eHCMSLanguage;

namespace aEMR.ResourceMaintenance.Maintenance.ViewModels
{
    [Export(typeof(IResourceMaintenanceLog_Add)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceMaintenanceLog_AddViewModel : Conductor<object>, IResourceMaintenanceLog_Add
        , IHandle<InfoTransViewModelEvent_ChooseRscrForMaintenance>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResourceMaintenanceLog_AddViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            GetAllStaffByStaffCategory(6102);
            GetSupplierIsMaintenance_All();
            Load_V_RscrInitialStatus();
            InitializeNewItem();

        }
        protected override void OnActivate()
        {
            base.OnActivate();
            authorization();

            

        }
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bbtChooseResource = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                 , (int)eResources_Maintenance.mAddNewRequest
                                                 , (int)oResources_MaintenanceEx.mRequest
                                                 , (int)ePermission.mAdd);
            bbtSave = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mResources_Maintenance
                                                      , (int)eResources_Maintenance.mAddNewRequest
                                                      , (int)oResources_MaintenanceEx.mRequest
                                                      , (int)ePermission.mAdd);
        }

#region account checking

        private bool _bbtChooseResource = true;
        private bool _bbtSave = true;
        public bool bbtChooseResource
        {
            get
            {
                return _bbtChooseResource;
            }
            set
            {
                if (_bbtChooseResource == value)
                    return;
                _bbtChooseResource = value;
            }
        }
        public bool bbtSave
        {
            get
            {
                return _bbtSave;
            }
            set
            {
                if (_bbtSave == value)
                    return;
                _bbtSave = value;
            }
        }

        private bool _IsShowAsChildWindow=false;
        public bool IsShowAsChildWindow
        {
            get { return _IsShowAsChildWindow; }
            set
            {
                if(_IsShowAsChildWindow!=value)
                {
                    _IsShowAsChildWindow = value;
                    NotifyOfPropertyChange(()=>IsShowAsChildWindow);
                }
            }
        }

        private bool _btChooseResourceIsEnabled = true;
        public bool btChooseResourceIsEnabled
        {
            get { return _btChooseResourceIsEnabled; }
            set
            {
                if (_btChooseResourceIsEnabled != value)
                {
                    _btChooseResourceIsEnabled = value;
                    NotifyOfPropertyChange(() => btChooseResourceIsEnabled);
                }
            }
        }

        #endregion
        

        public void btChooseResource()
        {
            //var typeInfo = Globals.GetViewModel<ITranfHome>();
            //typeInfo.IsChildWindowForChonDiBaoTri = false;

            //var instance = typeInfo as Conductor<object>;

            //Globals.ShowDialog(instance, (o) =>
            //{
            //    //làm gì đó
            //});
            Action<ITranfHome> onInitDlg = (typeInfo) =>
            {
                typeInfo.IsChildWindowForChonDiBaoTri = false;
            };
            GlobalsNAV.ShowDialog<ITranfHome>(onInitDlg);

        }

        private DataEntities.ResourcePropLocations _ObjResource_Current;
        public DataEntities.ResourcePropLocations ObjResource_Current
        {
            get
            {
                return _ObjResource_Current;
            }
            set
            {
                if (_ObjResource_Current != value)
                {
                    _ObjResource_Current = value;
                    NotifyOfPropertyChange(() => ObjResource_Current);
                }
            }
        }

        private DataEntities.ResourceMaintenanceLog _ObjResourceMaintenanceLog_Current;
        public DataEntities.ResourceMaintenanceLog ObjResourceMaintenanceLog_Current
        {
            get
            {
                return _ObjResourceMaintenanceLog_Current;
            }
            set
            {
                _ObjResourceMaintenanceLog_Current = value;
                NotifyOfPropertyChange(() => ObjResourceMaintenanceLog_Current);
            }
        }

        public void InitializeNewItem()
        {
            ObjResourceMaintenanceLog_Current = new ResourceMaintenanceLog();
            ObjResourceMaintenanceLog_Current.LoggingDate = DateTime.Now;

            ObjResourceMaintenanceLog_Current.LoggerStaffID = Globals.LoggedUserAccount.Staff != null
                                                                             ? Globals.LoggedUserAccount.Staff.
                                                                                   StaffID
                                                                             : (Globals.LoggedUserAccount.StaffID.
                                                                                    HasValue
                                                                                    ? Globals.LoggedUserAccount.
                                                                                          StaffID.Value
                                                                                    : -1);



            ObjResourceMaintenanceLog_Current.AssignStaffID = -1;
            ObjResourceMaintenanceLog_Current.ExternalFixSupplierID = -1;
            ObjResourceMaintenanceLog_Current.V_RscrInitialStatus = -1;
            ObjResourceMaintenanceLog_Current.RscrPropertyID = 0;
            ObjResourceMaintenanceLog_Current.LoggingIssue = "";
            ObjResourceMaintenanceLog_Current.Comments = "";

            ObjResource_Current=new ResourcePropLocations();

            cboStaff_Visibility = Visibility.Visible;
            cboSupplier_Visibility = Visibility.Collapsed;
        }

        public void SetValueRscrPropertyID(ResourcePropLocations pResourcePropLocations)
        {
            ObjResourceMaintenanceLog_Current.RscrPropertyID = pResourcePropLocations.VRscrProperty.RscrPropertyID;
        }

        private ObservableCollection<Staff> _ObjGetAllStaffByStaffCategory;
        public ObservableCollection<Staff> ObjGetAllStaffByStaffCategory
        {
            get
            {
                return _ObjGetAllStaffByStaffCategory;
            }
            set
            {
                _ObjGetAllStaffByStaffCategory = value;
                NotifyOfPropertyChange(()=>ObjGetAllStaffByStaffCategory);
            }
        }
        private void GetAllStaffByStaffCategory(long StaffCategory)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Nhân Viên..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetAllStaffByStaffCategory(StaffCategory, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetAllStaffByStaffCategory(asyncResult);
                            

                            if (items != null)
                            {
                                ObjGetAllStaffByStaffCategory = new ObservableCollection<DataEntities.Staff>(items);

                                //ItemDefault
                                DataEntities.Staff ItemDefault = new DataEntities.Staff();
                                ItemDefault.StaffID = -1;
                                ItemDefault.FullName = "--Chọn Nhân Viên--";
                                ObjGetAllStaffByStaffCategory.Insert(0, ItemDefault);
                                //ItemDefault
                            }
                            else
                            {
                                ObjGetAllStaffByStaffCategory = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        private ObservableCollection<Supplier> _ObjGetSupplierIsMaintenance_All;
        public ObservableCollection<Supplier> ObjGetSupplierIsMaintenance_All
        {
            get
            {
                return _ObjGetSupplierIsMaintenance_All;
            }
            set
            {
                _ObjGetSupplierIsMaintenance_All = value;
                NotifyOfPropertyChange(()=>ObjGetSupplierIsMaintenance_All);
            }
        }
        private void GetSupplierIsMaintenance_All()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Danh Sách Nhà Cung Cấp..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetSupplierIsMaintenance_All( Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var items = contract.EndGetSupplierIsMaintenance_All(asyncResult);

                            if (items != null)
                            {
                                ObjGetSupplierIsMaintenance_All = new ObservableCollection<DataEntities.Supplier>(items);

                                //ItemDefault
                                DataEntities.Supplier ItemDefault = new DataEntities.Supplier();
                                ItemDefault.SupplierID = -1;
                                ItemDefault.SupplierName = "--Chọn Nhà Cung Cấp--";
                                ObjGetSupplierIsMaintenance_All.Insert(0, ItemDefault);
                                //ItemDefault
                            }
                            else
                            {
                                ObjGetSupplierIsMaintenance_All = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }


        private ObservableCollection<Lookup> _ObjV_RscrInitialStatus;
        public ObservableCollection<Lookup> ObjV_RscrInitialStatus
        {
            get { return _ObjV_RscrInitialStatus; }
            set
            {
                _ObjV_RscrInitialStatus = value;
                NotifyOfPropertyChange(() => ObjV_RscrInitialStatus);
            }
        }

        public void Load_V_RscrInitialStatus()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Tình Trạng Thiết Bị..."
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RscrInitialStatus,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RscrInitialStatus = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_RscrInitialStatus.Insert(0, firstItem);
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
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }


        private Visibility _cboStaff_Visibility;
        public Visibility cboStaff_Visibility
        {
            get { return _cboStaff_Visibility; }
            set
            {
                _cboStaff_Visibility = value;
                NotifyOfPropertyChange(() => cboStaff_Visibility);
            }
        }

        private Visibility _cboSupplier_Visibility;
        public Visibility cboSupplier_Visibility
        {
            get { return _cboSupplier_Visibility; }
            set
            {
                _cboSupplier_Visibility = value;
                NotifyOfPropertyChange(() => cboSupplier_Visibility);
            }
        }

        public void chkIsSupplier_Click(object args)
        {
            ObjResourceMaintenanceLog_Current.AssignStaffID = -1;
            ObjResourceMaintenanceLog_Current.ExternalFixSupplierID = -1;

            bool V = (((System.Windows.Controls.Primitives.ToggleButton)(((System.Windows.RoutedEventArgs)(args)).OriginalSource)).IsChecked.Value);
            if (V)
            {
                cboSupplier_Visibility = Visibility.Visible;
                cboStaff_Visibility = Visibility.Collapsed;
                ObjResourceMaintenanceLog_Current.AssignStaffID = null;
            }
            else
            {
                cboSupplier_Visibility = Visibility.Collapsed;
                cboStaff_Visibility = Visibility.Visible;
                ObjResourceMaintenanceLog_Current.ExternalFixSupplierID = null;
            }

        }
        
        public void btSave()
        {
            if (ObjResourceMaintenanceLog_Current.RscrPropertyID>0)
            {
                if(cboSupplier_Visibility == Visibility.Visible)
                {
                    if(ObjResourceMaintenanceLog_Current.ExternalFixSupplierID>0)
                    {
                        if (!string.IsNullOrEmpty(ObjResourceMaintenanceLog_Current.LoggingIssue))
                        {
                            ObjResourceMaintenanceLog_Current.AssignStaffID = null;
                            AddNewResourceMaintenanceLog(ObjResourceMaintenanceLog_Current);
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0886_G1_Msg_InfoNhapVanDe);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0342_G1_Msg_InfoChonNCC);
                    }
                }
                if( cboStaff_Visibility == Visibility.Visible)
                {
                    if(ObjResourceMaintenanceLog_Current.AssignStaffID>0)
                    {
                        if(!string.IsNullOrEmpty(ObjResourceMaintenanceLog_Current.LoggingIssue))
                        {
                            ObjResourceMaintenanceLog_Current.ExternalFixSupplierID = null;
                            AddNewResourceMaintenanceLog(ObjResourceMaintenanceLog_Current);
                        }
                        else
                        {
                            MessageBox.Show(eHCMSResources.A0886_G1_Msg_InfoNhapVanDe);
                        }
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0343_G1_Msg_InfoChonNhVien);
                    }
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0363_G1_Msg_InfoChonThietBiDiBTri);
            }
        }


        private  void AddNewResourceMaintenanceLog(ResourceMaintenanceLog obj)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Lưu..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddNewResourceMaintenanceLog(obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long RscrMaintLogID=0;
                            if(contract.EndAddNewResourceMaintenanceLog(out RscrMaintLogID, asyncResult))
                            {
                                InitializeNewItem();
                                MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu, "Tạo Yêu Cầu Bảo Trì", MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show("Lưu Không Thành Công!", "Tạo Yêu Cầu Bảo Trì", MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), null);
                }


            });
            t.Start();
        }

        public void Handle(InfoTransViewModelEvent_ChooseRscrForMaintenance message)
        {
            if(message!=null)
            {
                ResourcePropLocations ObjResourcePropLocations = (message.ObjResourcePropLocations as ResourcePropLocations);
                ObjResource_Current = ObjResourcePropLocations;
                SetValueRscrPropertyID(ObjResourcePropLocations);
            }
        }

        public void KhoiTaoYeuCauVaGanBien(ResourcePropLocations ObjResourcePropLocations)
        {
            btChooseResourceIsEnabled = false;   
            
            authorization();

            ObjResource_Current = ObjResourcePropLocations;
            SetValueRscrPropertyID(ObjResourcePropLocations);
        }
    }
}
