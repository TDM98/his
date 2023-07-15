using System;
using System.Collections.Generic;
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
using aEMR.ViewContracts.ResourcesManage.Maintenance;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ViewContracts;

namespace aEMR.ResourceMaintenance.Maintenance.ViewModels
{
    [Export(typeof(IResourceMaintenanceLog_Confirm)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceMaintenanceLog_ConfirmViewModel : Conductor<object>, IResourceMaintenanceLog_Confirm
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ResourceMaintenanceLog_ConfirmViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Load_V_RscrInitialStatus();
            Load_V_RscrFinalStatus();


            ObjGetAllStaffByStaffCategory = new ObservableCollection<Staff>();
            GetAllStaffByStaffCategory(6102);

            ObjGetSupplierIsMaintenance_All = new ObservableCollection<Supplier>();
            GetSupplierIsMaintenance_All();
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
                NotifyOfPropertyChange(() => ObjGetAllStaffByStaffCategory);
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
                NotifyOfPropertyChange(() => ObjGetSupplierIsMaintenance_All);
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

                    contract.BeginGetSupplierIsMaintenance_All(Globals.DispatchCallback((asyncResult) =>
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


        public void LoadForm(DataEntities.ResourceMaintenanceLog Obj)
        {
            //Load UC
            var UC = Globals.GetViewModel<IResourceMaintenanceLogStatus_Add>();

            UC.ObjResourceMaintenanceLog_Current = Obj;

            UC.V_CurrentStatus_Seleted = -1;

            UC.LoadListHistoryStatus();

            Tab2 = UC;

            (this as Conductor<object>).ActivateItem(Tab2);
            //Load UC

            XetTinhTrangSauBaoTri();

            XetAiFix();

            //Xét Confirm chưa
            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã confirm
            {
                btConfirmSave_IsEnabled = false;
            }
            else
            {
                btConfirmSave_IsEnabled = true;
            }
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
                        "Tình Trạng Ban Đầu Của Thiết Bị ..."
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

        private object _Tab2;
        public object Tab2
        {
            get
            {
                return _Tab2;
            }
            set
            {
                if (_Tab2 == value)
                    return;
                _Tab2 = value;
                NotifyOfPropertyChange(() => Tab2);
            }
        }

        private bool _IsSupplierFix;
        public bool IsSupplierFix
        {
            get { return _IsSupplierFix; }
            set
            {
                _IsSupplierFix = value;
                NotifyOfPropertyChange(() => IsSupplierFix);
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

        private void XetAiFix()//là người đã sửa xong, hoặc là người đề xuất sửa
        {
            if (ObjResourceMaintenanceLog_Current.FixStaffID != null)
            {
                IsSupplierFix = false;
            }
            else if (ObjResourceMaintenanceLog_Current.FixSupplierID != null)
            {
                IsSupplierFix = true;
            }
            else
            {
                if (object.Equals(ObjResourceMaintenanceLog_Current.AssignStaffID, null) == false)
                {
                    ObjResourceMaintenanceLog_Current.FixStaffID = ObjResourceMaintenanceLog_Current.AssignStaffID;
                    IsSupplierFix = false;
                }
                else
                {
                    ObjResourceMaintenanceLog_Current.FixSupplierID = ObjResourceMaintenanceLog_Current.ExternalFixSupplierID;
                    IsSupplierFix = true;
                }
            }

            ShowHideCBO(IsSupplierFix);
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


        private void ShowHideCBO(bool pIsSupplier)
        {
            if (pIsSupplier)
            {
                cboSupplier_Visibility = Visibility.Visible;
                cboStaff_Visibility = Visibility.Collapsed;
            }
            else
            {
                cboSupplier_Visibility = Visibility.Collapsed;
                cboStaff_Visibility = Visibility.Visible;
            }
        }

        private ObservableCollection<Lookup> _ObjV_RscrFinalStatus;
        public ObservableCollection<Lookup> ObjV_RscrFinalStatus
        {
            get { return _ObjV_RscrFinalStatus; }
            set
            {
                _ObjV_RscrFinalStatus = value;
                NotifyOfPropertyChange(() => ObjV_RscrFinalStatus);
            }
        }

        public void Load_V_RscrFinalStatus()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Nhận Xét Thiết Bị..."
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_RscrFinalStatus,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_RscrFinalStatus = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_RscrFinalStatus.Insert(0, firstItem);
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

        private void XetTinhTrangSauBaoTri()
        {
            if (ObjResourceMaintenanceLog_Current.ObjV_RscrFinalStatus != null)
            {
                if (ObjResourceMaintenanceLog_Current.ObjV_RscrFinalStatus.LookupID <= 0)
                    ObjResourceMaintenanceLog_Current.ObjV_RscrFinalStatus.LookupID = -1;
            }
        }

        private bool _btConfirmSave_IsEnabled;
        public bool btConfirmSave_IsEnabled
        {
            get { return _btConfirmSave_IsEnabled; }
            set
            {
                _btConfirmSave_IsEnabled = value;
                NotifyOfPropertyChange(() => btConfirmSave_IsEnabled);
            }
        }

        public void btConfirmSave()
        {
            GetCheckForVerified(ObjResourceMaintenanceLog_Current.RscrMaintLogID);
        }

        private void SaveConfirmBaoTri()
        {
            if (ObjResourceMaintenanceLog_Current.ObjV_RscrFinalStatus.LookupID > 0)
            {
                Int64 VerifiedStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                UpdateResourceMaintenanceLog_FinalStatus(ObjResourceMaintenanceLog_Current.RscrMaintLogID,
                                                         VerifiedStaffID,
                                                         ObjResourceMaintenanceLog_Current.ObjV_RscrFinalStatus.
                                                             LookupID);
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0368_G1_Msg_InfoChonTinhTrangSauBTri, "Xác Nhận Bảo Trì", MessageBoxButton.OK);
            }
        }

        private void GetCheckForVerified(Int64 RscrMaintLogID)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Kiểm Tra..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetCheckForVerified(RscrMaintLogID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndGetCheckForVerified(asyncResult))
                            {
                                SaveConfirmBaoTri();
                            }
                            else
                            {
                                MessageBox.Show("Vấn Đề Này Đang Bảo Trì! Chưa Bảo Trì Xong, Không Thể Xác Nhận!", "Xác Nhận Bảo Trì", MessageBoxButton.OK);
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

        private void UpdateResourceMaintenanceLog_FinalStatus(
             Int64 RscrMaintLogID,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi Nhận..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginUpdateResourceMaintenanceLog_FinalStatus(RscrMaintLogID, VerifiedStaffID, V_CurrentStatus, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndUpdateResourceMaintenanceLog_FinalStatus(asyncResult))
                            {
                                btConfirmSave_IsEnabled = false;

                                Globals.EventAggregator.Publish(new ResourceMaintenanceLog_ConfirmViewModel_Confirm_Event() { Result = true });

                                MessageBox.Show(eHCMSResources.A0457_G1_Msg_InfoDaGhi, "Xác Nhận Bảo Trì", MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show("Ghi Nhận Không Thành Công!", "Xác Nhận Bảo Trì", MessageBoxButton.OK);
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
    }
}
