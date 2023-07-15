using System.ComponentModel.Composition;
using Caliburn.Micro;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using DataEntities;
using System.Threading;
using aEMR.ServiceClient;
using System;
using System.Windows;
using eHCMSLanguage;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using aEMR.Infrastructure.Events;
/*
* 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
* 20220815 #002 QTD: Dời kệ vào dãy
*/
namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefRowShelfEditViewModel)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefRowShelfEditViewModel : Conductor<object>, IRefRowShelfEditViewModel
    {
        private int _MaxWidth;
        public int MaxWidth
        {
            get { return _MaxWidth; }
            set
            {
                _MaxWidth = value;
                NotifyOfPropertyChange(() => MaxWidth);
            }
        }
        private ObservableCollection<RefShelves> _AllRefShelves = new ObservableCollection<RefShelves>();
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
                    if(RefShelfDetailID > 0)
                    {
                        LoadRefShelfDetails();
                    }
                }
            }
        }
        private RefShelves _RefShelf = new RefShelves();
        public RefShelves RefShelf
        {
            get
            {
                return _RefShelf;
            }
            set
            {
                if (_RefShelf != value)
                {
                    _RefShelf = value;
                    NotifyOfPropertyChange(() => RefShelf);
                }
            }
        }
        private RefShelfDetails _RefShelfDetails = new RefShelfDetails();
        public RefShelfDetails RefShelfDetails
        {
            get
            {
                return _RefShelfDetails;
            }
            set
            {
                if (_RefShelfDetails != value)
                {
                    _RefShelfDetails = value;
                    NotifyOfPropertyChange(() => RefShelfDetails);
                }
            }
        }
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
                    if (IsRefRowManager)
                    {
                        if(RefRowID > 0)
                        {
                            LoadRefRows(RefRowID);
                            return;
                        }
                        return;
                    }
                    LoadRefRows(0);
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefRowShelfEditViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            LoadData();
        }
        public void btnUpdateOrInsert()
        {
            if(IsRefShelfManager)
            {
                btnUpdateOrInsertRefShelve();
            }
            else if(IsRefShelfDetailManager)
            {
                btnUpdateOrInsertRefSheltDetails();
            }
            else
            {
                btnUpdateOrInsertRefRow();
            }
        }
        public void btnUpdateOrInsertRefRow()
        {
            this.ShowBusyIndicator();
            var UpdateRefRows = new ObservableCollection<RefRows>();
            UpdateRefRows.Add(RefRow);
            if(UpdateRefRows != null && UpdateRefRows.Count > 0)
            {
                if (SelectedStore == null)
                {
                    MessageBox.Show(eHCMSResources.K1973_G1_ChonKho, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (UpdateRefRows.Any(x => string.IsNullOrEmpty(x.RefRowCode)))
                {
                    MessageBox.Show("Vui lòng nhập mã dãy!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (UpdateRefRows.Any(x => string.IsNullOrEmpty(x.RefRowName)))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ tên dãy!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
            }
            else
            {
                this.HideBusyIndicator();
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateRefRows(Globals.LoggedUserAccount.Staff.StaffID, UpdateRefRows.ToList(), null, SelectedStore.StoreID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefRows = contract.EndUpdateRefRows(asyncResult);
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                                Globals.EventAggregator.Publish(new RefRowMedicalFileManagerEdit_Event() { Result = true });
                                btnCancel();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                            }
                            
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnUpdateOrInsertRefShelve()
        {
            this.ShowBusyIndicator();
            var UpdateRefShelfs = new ObservableCollection<RefShelves>();
            UpdateRefShelfs.Add(RefShelf);
            if (UpdateRefShelfs != null && UpdateRefShelfs.Count > 0)
            {
                if (SelectedRow == null)
                {
                    MessageBox.Show(eHCMSResources.Z3283_G1_ChonDay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (UpdateRefShelfs.Any(x => string.IsNullOrEmpty(x.RefShelfCode)))
                {
                    MessageBox.Show("Vui lòng nhập mã kệ!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (UpdateRefShelfs.Any(x => string.IsNullOrEmpty(x.RefShelfName)))
                {
                    MessageBox.Show(eHCMSResources.Z1993_G1_VuiLongNHapTenKe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
            }
            else
            {
                this.HideBusyIndicator();
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateRefShelves(Globals.LoggedUserAccount.Staff.StaffID, UpdateRefShelfs.ToList(), null, SelectedRow.RefRowID, true,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelves = contract.EndUpdateRefShelves(asyncResult);
                                MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                                Globals.EventAggregator.Publish(new RefShelfMedicalFileManagerEdit_Event() { Result = true });
                                btnCancel();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                            }
                            
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnUpdateOrInsertRefSheltDetails()
        {
            this.ShowBusyIndicator();
            var UpdateRefShelfDetails = new ObservableCollection<RefShelfDetails>();
            UpdateRefShelfDetails.Add(RefShelfDetails);
            if(UpdateRefShelfDetails != null && UpdateRefShelfDetails.Count > 0)
            {
                if (SelectedRow == null)
                {
                    MessageBox.Show(eHCMSResources.Z3283_G1_ChonDay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (SelectedRefShelf == null)
                {
                    MessageBox.Show(eHCMSResources.Z3284_G1_ChonKe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (UpdateRefShelfDetails.Any(x => string.IsNullOrEmpty(x.LocCode)))
                {
                    MessageBox.Show("Vui lòng nhập mã ngăn!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
                if (UpdateRefShelfDetails.Any(x => string.IsNullOrEmpty(x.LocName)))
                {
                    MessageBox.Show(eHCMSResources.Z1992_G1_VuiLongNhapTenNgan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                    return;
                }
            }
            else
            {
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateRefShelfDetails(SelectedRefShelf.RefShelfID, Globals.LoggedUserAccount.Staff.StaffID, UpdateRefShelfDetails.ToList(), null, true,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var result = contract.EndUpdateRefShelfDetails(asyncResult);
                                if (result)
                                {
                                    MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    Globals.EventAggregator.Publish(new RefShelfDetailsMedicalFileManagerEdit_Event() { Result = true });
                                    btnCancel();
                                }
                                this.HideBusyIndicator();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                            }
                            
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //public void cboStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((sender as ComboBox).SelectedItem == null)
        //    {
        //        return;
        //    }
        //    SelectedStore = (sender as ComboBox).SelectedItem as RefStorageWarehouseLocation;
        //    if(SelectedStore != null)
        //    {
        //        LoadRefRows();
        //    }
        //}
        #region Method

        private void LoadRefShelves(long? refShelfID)
        {
            if (IsRefRowManager)
            {
                return;
            }
            if(SelectedRow == null)
            {
                AllRefShelves = new ObservableCollection<RefShelves>();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelves(refShelfID.Value, SelectedRow.RefRowID, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelves = contract.EndGetRefShelves(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
                                this.HideBusyIndicator();
                            }
                            finally
                            {
                                if (IsRefShelfDetailManager)
                                {
                                    SetDefaultSelectedRefShelf();
                                }
                                if (RefShelfID > 0 && IsRefShelfManager)
                                {
                                    if (AllRefShelves != null && AllRefShelves.Count > 0)
                                    {
                                        RefShelf = AllRefShelves.First();
                                    }
                                }
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
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
        #endregion
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
        private void LoadRefRows(long? refRowID)
        {
            if(SelectedStore == null)
            {
                AllRefRows = new ObservableCollection<RefRows>();
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefRows(SelectedStore.StoreID, null, refRowID.Value,
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
                                    if(IsRefRowManager && RefRowID > 0)
                                    {
                                        if(AllRefRows != null && AllRefRows.Count > 0)
                                        {
                                            RefRow = AllRefRows.First();
                                        }
                                    }
                                    if (!IsRefRowManager)
                                    {
                                        SetDefaultSelectedRefRow();
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
                    if (IsRefShelfManager)
                    {
                        if(RefShelfID > 0)
                        {
                            LoadRefShelves(RefShelfID);
                            return;
                        }
                        return;
                    }
                    LoadRefShelves(0);
                }
            }
        }
        private RefRows _RefRow = new RefRows();
        public RefRows RefRow
        {
            get
            {
                return _RefRow;
            }
            set
            {
                if (_RefRow != value)
                {
                    _RefRow = value;
                    NotifyOfPropertyChange(() => RefRow);
                }
            }
        }
        private void SetDefaultSelectedStore()
        {
            if (AllStores != null && AllStores.Count > 0)
            {
                if(StoreID > 0)
                {
                    SelectedStore = AllStores.Where(x => x.StoreID == StoreID).FirstOrDefault();
                    return;
                }
                SelectedStore = AllStores.FirstOrDefault();
            }
        }
        private void SetDefaultSelectedRefRow()
        {
            if (AllRefRows != null && AllRefRows.Count > 0)
            {
                if(RefRowID > 0)
                {
                    SelectedRow = AllRefRows.Where(x => x.RefRowID == RefRowID).FirstOrDefault();
                    if(SelectedRow == null)
                    {
                        SelectedRow = AllRefRows.FirstOrDefault();
                    }
                    return;
                }
                SelectedRow = AllRefRows.FirstOrDefault();
            }
        }
        private void SetDefaultSelectedRefShelf()
        {
            if (AllRefShelves != null && AllRefShelves.Count > 0)
            {
                if(RefShelfID > 0)
                {
                    SelectedRefShelf = AllRefShelves.Where(x => x.RefShelfID == RefShelfID).FirstOrDefault();
                    if(SelectedRefShelf == null)
                    {
                        SelectedRefShelf = AllRefShelves.FirstOrDefault();
                    }
                    return;
                }
                SelectedRefShelf = AllRefShelves.FirstOrDefault();
            }
        }
        private long _StoreID;
        public long StoreID
        {
            get
            {
                return _StoreID;
            }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }
        private long _RefRowID;
        public long RefRowID
        {
            get
            {
                return _RefRowID;
            }
            set
            {
                if (_RefRowID != value)
                {
                    _RefRowID = value;
                    NotifyOfPropertyChange(() => RefRowID);
                }
            }
        }
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
        private long _RefShelfDetailID;
        public long RefShelfDetailID
        {
            get
            {
                return _RefShelfDetailID;
            }
            set
            {
                if (_RefShelfDetailID != value)
                {
                    _RefShelfDetailID = value;
                    NotifyOfPropertyChange(() => RefShelfDetailID);
                }
            }
        }
        private bool _IsUpdate;
        public bool IsUpdate
        {
            get { return _IsUpdate; }
            set
            {
                if(_IsUpdate != value)
                {
                    _IsUpdate = value;
                    NotifyOfPropertyChange(() => IsUpdate);
                }
            }
        }
        private bool _IsRefRowManager;
        public bool IsRefRowManager
        {
            get { return _IsRefRowManager; }
            set
            {
                if (_IsRefRowManager != value)
                {
                    _IsRefRowManager = value;
                    NotifyOfPropertyChange(() => IsRefRowManager);
                    NotifyOfPropertyChange(() => title);
                }
            }
        }
        private bool _IsRefShelfManager;
        public bool IsRefShelfManager
        {
            get { return _IsRefShelfManager; }
            set
            {
                if (_IsRefShelfManager != value)
                {
                    _IsRefShelfManager = value;
                    NotifyOfPropertyChange(() => IsRefShelfManager);
                    NotifyOfPropertyChange(() => title);
                }
            }
        }
        private bool _IsRefShelfDetailManager;
        public bool IsRefShelfDetailManager
        {
            get { return _IsRefShelfDetailManager; }
            set
            {
                if (_IsRefShelfDetailManager != value)
                {
                    _IsRefShelfDetailManager = value;
                    NotifyOfPropertyChange(() => IsRefShelfDetailManager);
                    NotifyOfPropertyChange(() => title);
                }
            }
        }
        public string title
        {
            get
            {
                return IsRefRowManager ? eHCMSResources.Z3281_G1_ThongTinDay : (IsRefShelfManager ? eHCMSResources.Z3282_G1_ThongTinKe : eHCMSResources.Z3282_G1_ThongTinNgan);
            }
        }
        private void LoadData()
        {
            LoadStores();
        }
        public void btnCancel()
        {
            initData();
            TryClose();
            Globals.EventAggregator.Unsubscribe(this);
        }
        private void initData()
        {
            SelectedRefShelf = new RefShelves();
            SelectedRow = new RefRows();
            SelectedStore = new RefStorageWarehouseLocation();
            RefShelfDetailID = 0;
            RefShelfID = 0;
            RefRowID = 0;
        }
        private void LoadRefShelfDetails()
        {
            if (IsRefRowManager)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelfDetails(0, null, RefShelfDetailID, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var result = contract.EndGetRefShelfDetails(asyncResult);
                                if(result != null && result.Count > 0)
                                {
                                    RefShelfDetails = result.First();
                                }    
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                this.HideBusyIndicator();
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
    }
}
