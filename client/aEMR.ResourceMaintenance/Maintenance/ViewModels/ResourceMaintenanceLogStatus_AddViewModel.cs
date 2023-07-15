using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using aEMR.DataContracts;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.ResourcesManage.Maintenance;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure;
using aEMR.ServiceClient;
using System.ServiceModel;
using aEMR.Common;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.ResourceMaintenance.Maintenance.ViewModels
{
    [Export(typeof(IResourceMaintenanceLogStatus_Add)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ResourceMaintenanceLogStatus_AddViewModel : Conductor<object>, IResourceMaintenanceLogStatus_Add
    {
        [ImportingConstructor]
        public ResourceMaintenanceLogStatus_AddViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            ObjGetResourceMaintenanceLogStatusByID=new PagedSortableCollectionView<ResourceMaintenanceLogStatus>();
            ObjGetResourceMaintenanceLogStatusByID.OnRefresh += new EventHandler<RefreshEventArgs>(ObjGetResourceMaintenanceLogStatusByID_OnRefresh);
            
            ObjV_CurrentStatus=new ObservableCollection<Lookup>();
            Load_V_CurrentStatus();

            ObjGetAllStaffByStaffCategory=new ObservableCollection<Staff>();
            GetAllStaffByStaffCategory(6102);

            ObjGetSupplierIsMaintenance_All=new ObservableCollection<Supplier>();
            GetSupplierIsMaintenance_All();

            NgayHoanTatBaoTri = DateTime.Now;

        }

        void ObjGetResourceMaintenanceLogStatusByID_OnRefresh(object sender, RefreshEventArgs e)
        {
            if(ObjResourceMaintenanceLog_Current!=null)
            {
                if(ObjResourceMaintenanceLog_Current.RscrMaintLogID>0)
                {
                    GetResourceMaintenanceLogStatusByID(ObjResourceMaintenanceLog_Current.RscrMaintLogID,ObjGetResourceMaintenanceLogStatusByID.PageIndex, ObjGetResourceMaintenanceLogStatusByID.PageSize, false);
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

        private PagedSortableCollectionView<DataEntities.ResourceMaintenanceLogStatus> _ObjGetResourceMaintenanceLogStatusByID;
        public PagedSortableCollectionView<DataEntities.ResourceMaintenanceLogStatus> ObjGetResourceMaintenanceLogStatusByID
        {
            get { return _ObjGetResourceMaintenanceLogStatusByID; }
            set
            {
                _ObjGetResourceMaintenanceLogStatusByID = value;
                NotifyOfPropertyChange(() => ObjGetResourceMaintenanceLogStatusByID);
            }
        }

        private void GetResourceMaintenanceLogStatusByID(long RscrMaintLogID, int PageIndex, int PageSize, bool CountTotal)
        {
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = "Danh Sách Tình Trạng..." });

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ResourcesManagementServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetResourceMaintenanceLogStatusByID(RscrMaintLogID, PageSize, PageIndex,"", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DataEntities.ResourceMaintenanceLogStatus> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetResourceMaintenanceLogStatusByID(out Total, asyncResult);
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

                            ObjGetResourceMaintenanceLogStatusByID.Clear();

                            if (bOK)
                            {
                                if (CountTotal)
                                {
                                    ObjGetResourceMaintenanceLogStatusByID.TotalItemCount = Total;
                                }
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjGetResourceMaintenanceLogStatusByID.Add(item);
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
                    Globals.IsBusy = false;
                }
            });
            t.Start();
        }

        private ObservableCollection<Lookup> _ObjV_CurrentStatus;
        public ObservableCollection<Lookup> ObjV_CurrentStatus
        {
            get { return _ObjV_CurrentStatus; }
            set
            {
                _ObjV_CurrentStatus = value;
                NotifyOfPropertyChange(() => ObjV_CurrentStatus);
            }
        }

        public void Load_V_CurrentStatus()
        {
            var t = new Thread(() =>
            {
                Globals.EventAggregator.Publish(new BusyEvent
                {
                    IsBusy = true,
                    Message =
                        "Danh Sách Tình Trạng..."
                });
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetAllLookupValuesByType(LookupValues.V_CurrentStatus,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                IList<Lookup> allItems = new ObservableCollection<Lookup>();
                                try
                                {
                                    allItems = contract.EndGetAllLookupValuesByType(asyncResult);

                                    ObjV_CurrentStatus = new ObservableCollection<Lookup>(allItems);
                                    Lookup firstItem = new Lookup();
                                    firstItem.LookupID = -1;
                                    firstItem.ObjectValue = eHCMSResources.A0015_G1_Chon;
                                    ObjV_CurrentStatus.Insert(0, firstItem);

                                    //bỏ Open ra(giá trị là 9000)
                                    foreach (Lookup item in ObjV_CurrentStatus)
                                    {
                                        if(item.LookupID==9000)
                                        {
                                            ObjV_CurrentStatus.Remove(item);
                                        }
                                    }
                                    
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

        public void LoadListHistoryStatus()
        {
            GetResourceMaintenanceLogStatusByID(ObjResourceMaintenanceLog_Current.RscrMaintLogID, 0, ObjGetResourceMaintenanceLogStatusByID.PageSize, true);


            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9998 || ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã Sửa Xong
            {
                //ex1DoneAndWaiting_IsExpanded = true;
                btUpdateStatus_IsEnabled = false;
                btSaveWaitingVerified_IsEnabled = false;
                btCancel_IsEnabled = false;
                chkIsSupplier_IsEnabled = false;
                XetAiFix();
            }
            else
            {
                btUpdateStatus_IsEnabled = true;
                ex1DoneAndWaiting_IsExpanded = false;
                btSaveWaitingVerified_IsEnabled = false;
                btCancel_IsEnabled = false;
                chkIsSupplier_IsEnabled = true;
                XetAiFix();
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

        private void ShowHideCBO(bool pIsSupplier)
        {
            if(pIsSupplier)
            {
                cboSupplier_Visibility = Visibility.Visible;
                cboStaff_Visibility = Visibility.Collapsed;
            }
            else
            {
                cboSupplier_Visibility = Visibility.Collapsed;
                cboStaff_Visibility =Visibility.Visible ;
            }
        }


        private Int64 _V_CurrentStatus_Seleted;
        public  Int64 V_CurrentStatus_Seleted
        {
            get { return _V_CurrentStatus_Seleted; }
            set { _V_CurrentStatus_Seleted = value;
                NotifyOfPropertyChange(()=>V_CurrentStatus_Seleted);
            }

        }


        private bool _btUpdateStatus_IsEnabled;
        public bool btUpdateStatus_IsEnabled
        {
            get { return _btUpdateStatus_IsEnabled; }
            set
            {
                _btUpdateStatus_IsEnabled = value;
                NotifyOfPropertyChange(() => btUpdateStatus_IsEnabled);
            }
        }

        private bool _ex1DoneAndWaiting_IsExpanded;
        public bool ex1DoneAndWaiting_IsExpanded
        {
            get { return _ex1DoneAndWaiting_IsExpanded; }
            set
            {
                _ex1DoneAndWaiting_IsExpanded = value;
                NotifyOfPropertyChange(() => ex1DoneAndWaiting_IsExpanded);
            }
        }

        private bool _btSaveWaitingVerified_IsEnabled;
        public bool btSaveWaitingVerified_IsEnabled
        {
            get { return _btSaveWaitingVerified_IsEnabled; }
            set
            {
                _btSaveWaitingVerified_IsEnabled = value;
                NotifyOfPropertyChange(() => btSaveWaitingVerified_IsEnabled);
            }
        }

        private bool _btCancel_IsEnabled;
        public bool btCancel_IsEnabled
        {
            get { return _btCancel_IsEnabled; }
            set
            {
                _btCancel_IsEnabled = value;
                NotifyOfPropertyChange(() => btCancel_IsEnabled);
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
        

        private bool _chkIsSupplier_IsEnabled;
        public bool chkIsSupplier_IsEnabled
        {
            get { return _chkIsSupplier_IsEnabled; }
            set
            {
                _chkIsSupplier_IsEnabled = value;
                NotifyOfPropertyChange(() => chkIsSupplier_IsEnabled);
            }
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
            ObjResourceMaintenanceLog_Current.FixStaffID = -1;
            ObjResourceMaintenanceLog_Current.FixSupplierID = -1;
            
            bool V = (((System.Windows.Controls.Primitives.ToggleButton)(((System.Windows.RoutedEventArgs)(args)).OriginalSource)).IsChecked.Value);

            IsSupplierFix = V;

            if (V)
            {
                cboSupplier_Visibility = Visibility.Visible;
                cboStaff_Visibility = Visibility.Collapsed;
                ObjResourceMaintenanceLog_Current.FixStaffID = null;
            }
            else
            {
                cboSupplier_Visibility = Visibility.Collapsed;
                cboStaff_Visibility = Visibility.Visible;
                ObjResourceMaintenanceLog_Current.FixSupplierID = null;
            }

        }

        public void ex1DoneAndWaiting_Expanded()
        {
            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9998 || ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã Sửa Xong
            {
                KhoaNut();
            }
            else
            {
                btUpdateStatus_IsEnabled = false;
                btSaveWaitingVerified_IsEnabled = true;
                btCancel_IsEnabled = true;
            }
        }

        public void ex1DoneAndWaiting_Collapsed()
        {
            V_CurrentStatus_Seleted = -1;
            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9998 || ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã Sửa Xong
            {
                KhoaNut();
            }
            else
            {
                btCancel();
            }
        }

        public void btCancel()
        {
            ex1DoneAndWaiting_IsExpanded = false;
            btUpdateStatus_IsEnabled = true;
            btSaveWaitingVerified_IsEnabled = false;
            btCancel_IsEnabled = false;
        }

        public void btUpdateStatus()
        {
            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9998 || ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã Sửa Xong
            {
                MessageBox.Show("Bảo Trì Này Đã Hoàn Tất! Không Thể Sửa Đổi Ghi Nhận!");
                return; 
            }

            if(V_CurrentStatus_Seleted>0)
            {
                if(ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID==V_CurrentStatus_Seleted)
                {
                    MessageBox.Show("Chọn Trạng Thái Khác Với Trạng Thái Hiện Tại Để Cập Nhật!");
                }
                else
                {
                    if(ObjResourceMaintenanceLogStatus_New!=null)
                    {
                        if(ObjResourceMaintenanceLogStatus_New.V_CurrentStatus==V_CurrentStatus_Seleted)
                        {
                            MessageBox.Show("Chọn Trạng Thái Khác Với Trạng Thái Hiện Tại Để Cập Nhật!");
                        }
                        else
                        {
                            InitializeNewItem(V_CurrentStatus_Seleted);
                            AddResourceMaintenanceLogStatus_New(ObjResourceMaintenanceLogStatus_New);                                  
                        }
                    }
                    else
                    {
                        InitializeNewItem(V_CurrentStatus_Seleted);
                        AddResourceMaintenanceLogStatus_New(ObjResourceMaintenanceLogStatus_New);                                  
                    }
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0370_G1_Msg_InfoChonTThai);
            }
        }


        private DataEntities.ResourceMaintenanceLogStatus _ObjResourceMaintenanceLogStatus_New;
        public DataEntities.ResourceMaintenanceLogStatus ObjResourceMaintenanceLogStatus_New
        {
            get
            {
                return _ObjResourceMaintenanceLogStatus_New;
            }
            set
            {
                _ObjResourceMaintenanceLogStatus_New = value;
                NotifyOfPropertyChange(() => ObjResourceMaintenanceLogStatus_New);
            }
        }

        public void InitializeNewItem(Int64 pV_CurrentStatus)
        {
            ObjResourceMaintenanceLogStatus_New = new ResourceMaintenanceLogStatus();
            ObjResourceMaintenanceLogStatus_New.RecDateCreated = DateTime.Now;
            ObjResourceMaintenanceLogStatus_New.StatusChangeDate = DateTime.Now;
            ObjResourceMaintenanceLogStatus_New.UpdateStatusStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            ObjResourceMaintenanceLogStatus_New.V_CurrentStatus = pV_CurrentStatus;
            ObjResourceMaintenanceLogStatus_New.RscrMaintLogID = ObjResourceMaintenanceLog_Current.RscrMaintLogID;
        }

        private void AddResourceMaintenanceLogStatus_New(ResourceMaintenanceLogStatus obj)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi Nhận..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginAddResourceMaintenanceLogStatus_New(obj, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndAddResourceMaintenanceLogStatus_New(asyncResult))
                            {
                                GetResourceMaintenanceLogStatusByID(ObjResourceMaintenanceLog_Current.RscrMaintLogID, 0, ObjGetResourceMaintenanceLogStatusByID.PageSize, true);   
                                MessageBox.Show(eHCMSResources.A0457_G1_Msg_InfoDaGhi, "Cập Nhật Trạng Thái", MessageBoxButton.OK);

                            }
                            else
                            {
                                MessageBox.Show("Ghi Không Thành Công!", "Cập Nhật Trạng Thái", MessageBoxButton.OK);
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

        private Nullable<DateTime> _NgayHoanTatBaoTri;
        public Nullable<DateTime> NgayHoanTatBaoTri
        {
            get { return _NgayHoanTatBaoTri; }
            set
            {
                _NgayHoanTatBaoTri = value;
                NotifyOfPropertyChange(()=>NgayHoanTatBaoTri);
            }
        }


        public void cboCurrentStatus_SelectedChanged(object selectItem)
        {
            V_CurrentStatus_Seleted = (selectItem as Lookup).LookupID;

            if (V_CurrentStatus_Seleted == 9998)//Done and waiting verified
            {
                btUpdateStatus_IsEnabled = false;
                ex1DoneAndWaiting_IsExpanded = true;
                btSaveWaitingVerified_IsEnabled = true;
                btCancel_IsEnabled = true;
            }
            else
            {
                btUpdateStatus_IsEnabled = true;
                ex1DoneAndWaiting_IsExpanded = false;
                btSaveWaitingVerified_IsEnabled = false;
                btCancel_IsEnabled = false;
            }

            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9998 || ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã Sửa Xong
            {
                ex1DoneAndWaiting_IsExpanded = true;
                KhoaNut();
            }
        }
        
        public void btSaveWaitingVerified()
        {
            if (ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9998 || ObjResourceMaintenanceLog_Current.ObjV_CurrentStatus.LookupID == 9999)//Đã Sửa Xong
            {
                MessageBox.Show("Bảo Trì Này Đã Hoàn Tất! Không Thể Sửa Đổi Ghi Nhận!");
                return;
            }
            else
            {
                if (V_CurrentStatus_Seleted == 9998)
                {
                    if (CheckValidSaveWaitingVerified())
                    {
                        if (IsSupplierFix)
                        {
                            ObjResourceMaintenanceLog_Current.FixStaffID = null;
                        }
                        else
                        {
                            ObjResourceMaintenanceLog_Current.FixSupplierID = null;
                        }

                        Int64 VerifiedStaffID = Globals.LoggedUserAccount.Staff.StaffID;

                        SaveResourceMaintenanceLogOfFix(ObjResourceMaintenanceLog_Current.RscrMaintLogID,
                                                        ObjResourceMaintenanceLog_Current.FixStaffID,
                                                        ObjResourceMaintenanceLog_Current.FixSupplierID,
                                                        ObjResourceMaintenanceLog_Current.RecDateCreated,
                                                        ObjResourceMaintenanceLog_Current.FixSolutions,
                                                        ObjResourceMaintenanceLog_Current.FixComments, VerifiedStaffID,
                                                        V_CurrentStatus_Seleted);
                    }
                }
                else
                {
                    MessageBox.Show("Chọn Done and Waiting Verified! Để Hoàn Tất Và Đợi Xác Nhận", "Lưu", MessageBoxButton.OK);
                }
            }
        }

        private void SaveResourceMaintenanceLogOfFix(
            Int64 RscrMaintLogID,
            Nullable<Int64> FixStaffID,
            Nullable<Int64> FixSupplierID,
            DateTime FixDate,
            string FixSolutions,
            string FixComments,
            Int64 VerifiedStaffID,
            Int64 V_CurrentStatus)
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Đang Ghi Nhận..." });

            var t = new Thread(() =>
            {
                using (var serviceFactory = new ResourcesManagementServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginSaveResourceMaintenanceLogOfFix(RscrMaintLogID,FixStaffID,FixSupplierID,FixDate,FixSolutions,FixComments,VerifiedStaffID,V_CurrentStatus, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            if (contract.EndSaveResourceMaintenanceLogOfFix(asyncResult))
                            {
                                GetResourceMaintenanceLogStatusByID(ObjResourceMaintenanceLog_Current.RscrMaintLogID, 0, ObjGetResourceMaintenanceLogStatusByID.PageSize, true);

                                btUpdateStatus_IsEnabled = false;
                                btSaveWaitingVerified_IsEnabled = false;
                                btCancel_IsEnabled = false;
                                chkIsSupplier_IsEnabled = false;

                                Globals.EventAggregator.Publish(new ResourceMaintenanceLogStatus_AddViewModel_HoanTatBaoTri_Event() { Result = true });

                                MessageBox.Show(eHCMSResources.A0457_G1_Msg_InfoDaGhi, "Hoàn Tất Bảo Trì", MessageBoxButton.OK);

                            }
                            else
                            {
                                MessageBox.Show("Ghi Không Thành Công!", "Hoàn Tất Bảo Trì", MessageBoxButton.OK);
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

        private  bool CheckValidSaveWaitingVerified()
        {
            DateTime DateFix;

            if (NgayHoanTatBaoTri==null)
            {
                MessageBox.Show(eHCMSResources.A0875_G1_Msg_InfoNhapNgHTatBTri);
                return false;
            }
            try
            {
                DateFix=DateTime.Parse(ObjResourceMaintenanceLog_Current.RecDateCreated.ToString());

                if (DateTime.Now.Subtract(DateFix).Days < 0)
                {
                    MessageBox.Show("Ngày Hoàn Tất Bảo Trì Phải >= Ngày Hiện Hành!", "Lưu", MessageBoxButton.OK);
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(eHCMSResources.A0875_G1_Msg_InfoNhapNgHTatBTri);
                return  false;
            }
            
            if (IsSupplierFix)
            {
                if (ObjResourceMaintenanceLog_Current.FixSupplierID <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0342_G1_Msg_InfoChonNCC);
                    return  false;  
                }
            }
            else
            {
                if (ObjResourceMaintenanceLog_Current.FixStaffID <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0343_G1_Msg_InfoChonNhVien);
                    return false;
                }
            }

            if(string.IsNullOrEmpty(ObjResourceMaintenanceLog_Current.FixSolutions))
            {
                MessageBox.Show(eHCMSResources.A0869_G1_Msg_InfoNhapHuongGiaiQuyet);
                return false;
            }
            return true;
        }

        public void LostFocus_dtDateFix(object textDate)
        {
            if (string.IsNullOrEmpty(textDate.ToString()))
            {
                NgayHoanTatBaoTri = null;
            }
            else
            {
                try
                {
                    NgayHoanTatBaoTri = DateTime.Parse(textDate.ToString());
                }
                catch
                {
                    NgayHoanTatBaoTri=null;
                }
            }
        }
       
        private  void KhoaNut()
        {
            btUpdateStatus_IsEnabled = false;
            btSaveWaitingVerified_IsEnabled = false;
            btCancel_IsEnabled = false;
        }
    }
}
