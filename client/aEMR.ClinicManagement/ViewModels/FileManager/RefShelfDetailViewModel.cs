using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using System.Collections.Generic;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using eHCMSLanguage;
using aEMR.Common;
using System.Windows.Controls;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using System.Linq;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
/*
* 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
* 20220817 #002 QTD: Chỉnh sửa giao diện ngăn
*/
namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefShelfDetail)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefShelfDetailViewModel : Conductor<object>, IRefShelfDetail
        , IHandle<RefShelfDetailsMedicalFileManagerEdit_Event>
        , IHandle<RefRowMedicalFileManagerEdit_Event>
        , IHandle<RefShelfMedicalFileManagerEdit_Event>
    {
        private long _RefShelfID;
        public long RefShelfID
        {
            get
            {
                return _RefShelfID;
            }
            set
            {
                if (_RefShelfID != value)
                {
                    _RefShelfID = value;
                    NotifyOfPropertyChange(() => RefShelfID);
                }
            }
        }
        private ObservableCollection<RefShelfDetails> _AllRefShelfDetails = new ObservableCollection<RefShelfDetails>();
        public ObservableCollection<RefShelfDetails> AllRefShelfDetails
        {
            get
            {
                return _AllRefShelfDetails;
            }
            set
            {
                _AllRefShelfDetails = value;
                NotifyOfPropertyChange(() => AllRefShelfDetails);
            }
        }
        private RefShelfDetails _SelectedRefShelfDetail;
        public RefShelfDetails SelectedRefShelfDetail
        {
            get
            {
                return _SelectedRefShelfDetail;
            }
            set
            {
                _SelectedRefShelfDetail = value;
                NotifyOfPropertyChange(() => SelectedRefShelfDetail);
            }
        }
        DataGrid gvRefShelfDetail;
        private bool _IsBusy;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }
        private string _BusyMessage;
        public string BusyMessage
        {
            get { return _BusyMessage; }
            set
            {
                _BusyMessage = value;
                NotifyOfPropertyChange(() => BusyMessage);
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefShelfDetailViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            //LoadData();
            LoadStores();
        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedRefShelfDetail != null && AllRefShelfDetails.IndexOf(SelectedRefShelfDetail) < AllRefShelfDetails.Count - 1)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.Z1997_G1_Ngan + " " + SelectedRefShelfDetail.LocName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        AllRefShelfDetails.Remove(SelectedRefShelfDetail);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        private void AddBlank()
        {
            try
            {
                if (AllRefShelfDetails.Count > 0 && string.IsNullOrEmpty(AllRefShelfDetails.Last().LocName))
                    return;
                long mRefShelfDetailID = 0;
                if (AllRefShelfDetails.Count > 0)
                    mRefShelfDetailID = AllRefShelfDetails.Max(x => x.RefShelfDetailID) + 1;
                var mLocCode = AllRefShelfDetails.Count == 0 ? "H1" : "H" + (Convert.ToInt32(AllRefShelfDetails.Max(x => x.LocCode).Replace("H", "")) + 1).ToString();
                AllRefShelfDetails.Add(new RefShelfDetails { RefShelfID = 0, RefShelfDetailID = mRefShelfDetailID, LocCode = mLocCode, CreatedDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        private void LoadRefShelfDetails()
        {
            if(SelectedRefShelf == null || SelectedRefShelf.RefShelfID == 0)
            {
                return;
            }
            ShowBusy();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelfDetails(SelectedRefShelf.RefShelfID, null, 0, LocName,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelfDetails = contract.EndGetRefShelfDetails(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                HideBusy();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    HideBusy();
                }
            });
            t.Start();
        }
        public void gvRefShelfDetail_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
        }
        //▼====== #001
        //public void gvRefShelfDetail_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    if (e.Row.GetIndex() == (AllRefShelfDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit && !string.IsNullOrEmpty(AllRefShelfDetails.Last().LocCode))
        //    {
        //        AddBlank();
        //    }
        //}
        public void gvRefShelfDetail_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.GetIndex() == (AllRefShelfDetails.Count - 1) && e.EditAction == DataGridEditAction.Commit && !string.IsNullOrEmpty(AllRefShelfDetails.Last().LocCode))
            {
                AddBlank();
            }
        }
        //▲====== #001
        public void gvRefShelfDetail_Loaded(object sender, RoutedEventArgs e)
        {
            gvRefShelfDetail = sender as DataGrid;
        }
        public void cboLocCode_LostFocus(object sender, RoutedEventArgs e)
        {
            //SelectedRefShelfDetail.CaseCode = (sender as eHCMS.ControlsLibrary.AxAutoComplete).SearchText;
        }
        private void ShowBusy()
        {
            BusyMessage = eHCMSResources.Z0125_G1_DangXuLi;
            IsBusy = true;
        }
        private void HideBusy()
        {
            IsBusy = false;
        }
        //public void btnUpdate()
        //{
        //    ShowBusy();
        //    if (AllRefShelfDetails.Take(AllRefShelfDetails.Count - 1).Any(x => string.IsNullOrEmpty(x.LocName)))
        //    {
        //        MessageBox.Show(eHCMSResources.Z1992_G1_VuiLongNhapTenNgan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        HideBusy();
        //        return;
        //    }
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new ClinicManagementServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;
        //                var ListRefShelfDetail = AllRefShelfDetails.Where(x => !string.IsNullOrEmpty(x.LocCode)).ToList();
        //                ListRefShelfDetail.Where(x => x.RefShelfID == 0).ToList().ForEach(x => { x.RefShelfID = RefShelfID; x.RefShelfDetailID = -1; });
        //                contract.BeginUpdateRefShelfDetails(RefShelfID, ListRefShelfDetail,
        //                Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        bool mResult = contract.EndUpdateRefShelfDetails(asyncResult);
        //                        if (mResult)
        //                        {
        //                            MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                            LoadData();
        //                        }
        //                        else
        //                            MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                        HideBusy();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                        HideBusy();
        //                    }
        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //            HideBusy();
        //        }
        //    });
        //    t.Start();
        //}
        //▼====: #002
        private ObservableCollection<RefStorageWarehouseLocation> _AllStores;
        public ObservableCollection<RefStorageWarehouseLocation> AllStores
        {
            get
            {
                return _AllStores;
            }
            set
            {
                if (_AllStores != value)
                {
                    _AllStores = value;
                    NotifyOfPropertyChange(() => AllStores);
                }
            }
        }
        private RefStorageWarehouseLocation _SelectedStore;
        public RefStorageWarehouseLocation SelectedStore
        {
            get
            {
                return _SelectedStore;
            }
            set
            {
                if (_SelectedStore != value)
                {
                    _SelectedStore = value;
                    NotifyOfPropertyChange(() => SelectedStore);
                }
            }
        }
        private ObservableCollection<RefRows> _AllRefRows;
        public ObservableCollection<RefRows> AllRefRows
        {
            get
            {
                return _AllRefRows;
            }
            set
            {
                if (_AllRefRows != value)
                {
                    _AllRefRows = value;
                    NotifyOfPropertyChange(() => AllRefRows);
                }
            }
        }
        private RefRows _SelectedRow;
        public RefRows SelectedRow
        {
            get
            {
                return _SelectedRow;
            }
            set
            {
                if (_SelectedRow != value)
                {
                    _SelectedRow = value;
                    NotifyOfPropertyChange(() => SelectedRow);
                }
            }
        }
        private ObservableCollection<RefShelves> _AllRefShelves;
        public ObservableCollection<RefShelves> AllRefShelves
        {
            get
            {
                return _AllRefShelves;
            }
            set
            {
                if (_AllRefShelves != value)
                {
                    _AllRefShelves = value;
                    NotifyOfPropertyChange(() => AllRefShelves);
                }
            }
        }
        private RefShelves _SelectedRefShelf;
        public RefShelves SelectedRefShelf
        {
            get
            {
                return _SelectedRefShelf;
            }
            set
            {
                if (_SelectedRefShelf != value)
                {
                    _SelectedRefShelf = value;
                    NotifyOfPropertyChange(() => SelectedRefShelf);
                    if(_SelectedRefShelf != null)
                    {
                        LoadRefShelfDetails();
                    }
                    else
                    {
                        AllRefShelfDetails = new ObservableCollection<RefShelfDetails>();
                    }
                }
            }
        }

        private string _LocName;
        public string LocName
        {
            get
            {
                return _LocName;
            }
            set
            {
                if (_LocName != value)
                {
                    _LocName = value;
                    NotifyOfPropertyChange(() => LocName);
                }
            }
        }
        private void SetDefaultSelectedStore()
        {
            if (AllStores != null && AllStores.Count > 0)
            {
                SelectedStore = AllStores.FirstOrDefault();
            }
        }
        private void SetDefaultSelectedRefRow()
        {
            if (AllRefRows != null && AllRefRows.Count > 0)
            {
                SelectedRow = AllRefRows.FirstOrDefault();
            }
        }
        private void SetDefaultSelectedRefShelf()
        {
            if (AllRefShelves != null && AllRefShelves.Count > 0)
            {
                SelectedRefShelf = AllRefShelves.FirstOrDefault();
            }
        }
        private void LoadStores()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyStoragesServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStoragesNotPaging((long)AllLookupValues.StoreType.STORAGE_FILES, false, null, null,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    AllStores = new ObservableCollection<RefStorageWarehouseLocation>(contract.EndGetAllStoragesNotPaging(asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    SetDefaultSelectedStore();
                                    if (SelectedStore != null)
                                    {
                                        LoadRefRows();
                                    }
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadRefRows()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefRows(SelectedStore.StoreID, null, 0,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefRows = contract.EndGetRefRows(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
                            }
                            finally
                            {
                                AddFirstItemRefRow();
                                if (SelectedStore != null && SelectedRow != null)
                                {
                                    LoadRefShelves();
                                }
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadRefShelves()
        {
            if (SelectedRow == null || SelectedRow.RefRowID <= 0)
            {
                AllRefShelves = new ObservableCollection<RefShelves>();
                AddFirstItemRefShelf();
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelves(0, SelectedRow.RefRowID, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelves = contract.EndGetRefShelves(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
                            }
                            finally
                            {
                                AddFirstItemRefShelf();
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnSearch()
        {
            if (SelectedStore == null)
            {
                MessageBox.Show("Chọn kho!", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                HideBusy();
                return;
            }
            if (SelectedRow == null)
            {
                MessageBox.Show("Chọn dãy!", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                HideBusy();
                return;
            }
            if (SelectedRefShelf == null)
            {
                MessageBox.Show("Chọn kệ!", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                HideBusy();
                return;
            }
            LoadRefShelfDetails();
        }

        public void cboStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null)
            {
                return;
            }
            SelectedStore = (sender as ComboBox).SelectedItem as RefStorageWarehouseLocation;
            if (SelectedStore != null)
            {
                LoadRefRows();
            }
        }
        public void cboRefRow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem == null)
            {
                return;
            }
            SelectedRow = (sender as ComboBox).SelectedItem as RefRows;
            if (SelectedRow != null)
            {
                LoadRefShelves();
            }
        }
        public void DeleteRefShelfDetails(RefShelfDetails RefShelfDetailDeleted)
        {
            ShowBusy();
            var ListRefShelfDetailDeleted = new ObservableCollection<RefShelfDetails>();
            ListRefShelfDetailDeleted.Add(RefShelfDetailDeleted);
            if (ListRefShelfDetailDeleted == null || ListRefShelfDetailDeleted.Count == 0)
            {
                HideBusy();
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateRefShelfDetails(RefShelfDetailDeleted.RefShelfID, Globals.LoggedUserAccount.Staff.StaffID, null, ListRefShelfDetailDeleted.ToList(), true,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool mResult = contract.EndUpdateRefShelfDetails(asyncResult);
                                if (mResult)
                                {
                                    MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    Globals.EventAggregator.Publish(new RefShelfDetailsMedicalFileManagerEdit_Event() { Result = true });
                                }
                                else
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                HideBusy();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                HideBusy();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    HideBusy();
                }
            });
            t.Start();
        }
        public void hplDelete_Click(object selectItem)
        {
            try
            {
                RefShelfDetails mRefShelfDetails = selectItem as RefShelfDetails;
                if (mRefShelfDetails != null)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.Z1997_G1_Ngan + " " + mRefShelfDetails.LocName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DeleteRefShelfDetails(mRefShelfDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void hplEdit_Click(object selectItem)
        {
            try
            {
                if (selectItem != null)
                {
                    RefShelfDetails mRefShelfDetails = selectItem as RefShelfDetails;
                    if (mRefShelfDetails.RefShelfDetailID > 0)
                    {
                        Action<IRefRowShelfEditViewModel> onInitDlg = (mShellDetail) =>
                        {
                            mShellDetail.StoreID = SelectedStore.StoreID;
                            mShellDetail.RefRowID = SelectedRow.RefRowID;
                            mShellDetail.RefShelfID = SelectedRefShelf.RefShelfID;
                            mShellDetail.RefShelfDetailID = mRefShelfDetails.RefShelfDetailID;
                            mShellDetail.IsRefShelfDetailManager = true;
                        };
                        GlobalsNAV.ShowDialog(onInitDlg);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        
        public void btnAdd()
        {
            Action<IRefRowShelfEditViewModel> onInitDlg = (mShellDetail) =>
            {
                mShellDetail.StoreID = SelectedStore != null ? SelectedStore.StoreID : 0;
                mShellDetail.RefRowID = SelectedRow!= null ? SelectedRow.RefRowID : 0;
                mShellDetail.RefShelfID = SelectedRefShelf != null ? SelectedRefShelf.RefShelfID : 0;
                mShellDetail.IsRefShelfDetailManager = true;
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }

        public void Handle(RefShelfDetailsMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadRefShelfDetails();
            }
        }
        public void Handle(RefShelfMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadRefShelves();
            }
        }
        public void Handle(RefRowMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadRefRows();
            }
        }
        private void AddFirstItemRefRow()
        {
            RefRows firstItem = new RefRows();
            firstItem.RefRowID = 0;
            firstItem.RefRowName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3283_G1_ChonDay);
            AllRefRows.Insert(0, firstItem);
            SetDefaultSelectedRefRow();
        }
        private void AddFirstItemRefShelf()
        {
            RefShelves firstItem = new RefShelves();
            firstItem.RefShelfID = 0;
            firstItem.RefShelfName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3284_G1_ChonKe);
            AllRefShelves.Insert(0, firstItem);
            SetDefaultSelectedRefShelf();
        }
        //▲====: #002
    }
}
