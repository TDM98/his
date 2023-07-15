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
    [Export(typeof(IRefShelf)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefShelfViewModel : Conductor<object>, IRefShelf
        , IHandle<RefShelfMedicalFileManagerEdit_Event>
        , IHandle<RefRowMedicalFileManagerEdit_Event>
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
                }
            }
        }
        private RefStorageWarehouseLocation SavedStore;
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
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefShelfViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            LoadStores();
        }
        public void hplEdit_Click(object selectItem)
        {
            try
            {
                if (selectItem != null)
                {
                    RefShelves mRefShelf = selectItem as RefShelves;
                    if (mRefShelf.RefShelfID > 0)
                    {
                        //var mShellDetail = Globals.GetViewModel<IRefShelfDetail>();
                        //mShellDetail.RefShelfID = mRefShelf.RefShelfID;
                        //var instance = mShellDetail as Conductor<object>;
                        //Globals.ShowDialog(instance, (o) => { });

                        //Action<IRefShelfDetail> onInitDlg = (mShellDetail) =>
                        //{
                        //    mShellDetail.RefShelfID = mRefShelf.RefShelfID;
                        //};
                        //GlobalsNAV.ShowDialog<IRefShelfDetail>(onInitDlg);
                        Action<IRefRowShelfEditViewModel> onInitDlg = (mShellDetail) =>
                        {
                            mShellDetail.StoreID = SelectedStore.StoreID;
                            mShellDetail.RefRowID = SelectedRow.RefRowID;
                            mShellDetail.RefShelfID = mRefShelf.RefShelfID;
                            mShellDetail.IsRefShelfManager = true;
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
        //public void hplDelete_Click(object selectItem)
        //{
        //    try
        //    {
        //        if (selectItem != null && AllRefShelves.IndexOf(selectItem as RefShelves) < AllRefShelves.Count - 1)
        //        {
        //            RefShelves mRefShelf = (selectItem as RefShelves);
        //            if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.T2013_G1_Ke + " " + mRefShelf.RefShelfName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //            {
        //                this.ShowBusyIndicator();
        //                AllRefShelves.Remove(mRefShelf);
        //                this.HideBusyIndicator();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //    }
        //}

        //▼====== #001
        //public void gvRefShelves_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    if (e.Row.GetIndex() == (AllRefShelves.Count - 1) && e.EditAction == DataGridEditAction.Commit && AllRefShelves.Last().Validate())
        //    {
        //        AddBlank();
        //    }
        //}
        public void gvRefShelves_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.GetIndex() == (AllRefShelves.Count - 1) && e.EditAction == DataGridEditAction.Commit && AllRefShelves.Last().Validate() && !string.IsNullOrEmpty(AllRefShelves.Last().RefShelfName))
            {
                AddBlank();
            }
        }
        //▲====== #001
        public void btnUpdate()
        {
            this.ShowBusyIndicator();
            var UpdateRefShelfs = AllRefShelves.Take(AllRefShelves.Count - 1).ToList();
            if (UpdateRefShelfs.Count == 0 || SelectedRow == null)
            {
                this.HideBusyIndicator();
                return;
            }
            if (UpdateRefShelfs.Any(x => string.IsNullOrEmpty(x.RefShelfName)))
            {
                MessageBox.Show(eHCMSResources.Z1993_G1_VuiLongNHapTenKe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                        contract.BeginUpdateRefShelves(Globals.LoggedUserAccount.Staff.StaffID, UpdateRefShelfs, null, SelectedRow.RefRowID, false,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            AllRefShelves = contract.EndUpdateRefShelves(asyncResult);
                            MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            foreach (var item in AllRefShelves)
                                item.AllStores = this.AllStores.ToList();
                            //AddBlank();
                            this.HideBusyIndicator();
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
        public void cboStore_LostFocus(object sender, RoutedEventArgs e)
        {
            //SelectedRefShelfDetail.CaseCode = (sender as eHCMS.ControlsLibrary.AxAutoComplete).SearchText;
        }
        public void cboStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //▼====: #002
            //if (e.AddedItems.Count > 0)
            //{
            //    var item = (e.AddedItems[0] as RefStorageWarehouseLocation);
            //    SelectedRefShelf.StoreID = item.StoreID;
            //    SelectedRefShelf.swhlName = item.swhlName;
            //}
            if ((sender as ComboBox).SelectedItem == null)
            {
                return;
            }
            SelectedStore = (sender as ComboBox).SelectedItem as RefStorageWarehouseLocation;
            if(SelectedStore != null)
            {
                LoadRefRows();
            }
            //▲====: #002
        }
        public void btnSearch()
        {
            if (SelectedStore == null)
            {
                MessageBox.Show("Chọn kho", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                this.HideBusyIndicator();
                return;
            }
            if (SelectedRow == null)
            {
                MessageBox.Show("Chọn dãy", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                this.HideBusyIndicator();
                return;
            }
            LoadRefShelves();
        }
        #region Method
        private void AddBlank()
        {
            try
            {
                RefShelves mRefShelf = new RefShelves { CreatedDate = Globals.GetCurServerDateTime() };
                if (AllRefShelves.Count == 0)
                    mRefShelf.RefShelfID = -1;
                else
                    mRefShelf.RefShelfID = AllRefShelves.Min(x => x.RefShelfID) > 0 ? -1 : AllRefShelves.Min(x => x.RefShelfID) - 1;
                mRefShelf.RefShelfCode = AllRefShelves.Count == 0 ? "K1" : "K" + (Convert.ToInt32(AllRefShelves.Max(x => x.RefShelfCode).Replace("K", "")) + 1).ToString();
                mRefShelf.AllStores = this.AllStores.ToList();
                mRefShelf.StoreID = SavedStore.StoreID;
                mRefShelf.swhlName = SavedStore.swhlName;
                AllRefShelves.Add(mRefShelf);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        private void LoadRefShelves()
        {
            if(SelectedRow == null || SelectedRow.RefRowID == 0)
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
                        contract.BeginGetRefShelves(0, SelectedRow.RefRowID, RefShelfName,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelves = contract.EndGetRefShelves(asyncResult);
                                foreach (var item in AllRefShelves)
                                    item.AllStores = this.AllStores.ToList();
                                //AddBlank();
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.Message);
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
                                    if(SelectedStore != null)
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
                    if(_SelectedRow != null)
                    {
                        LoadRefShelves();
                    }
                    else
                    {
                        AllRefShelves = new ObservableCollection<RefShelves>();
                    }
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
        private string _RefShelfName;
        public string RefShelfName
        {
            get
            {
                return _RefShelfName;
            }
            set
            {
                if (_RefShelfName != value)
                {
                    _RefShelfName = value;
                    NotifyOfPropertyChange(() => RefShelfName);
                }
            }
        }
        public void btnAdd()
        {
            Action<IRefRowShelfEditViewModel> onInitDlg = (mShellDetail) =>
            {
                mShellDetail.StoreID = SelectedStore != null ? SelectedStore.StoreID : 0;
                mShellDetail.RefRowID = SelectedRow!= null ? SelectedRow.RefRowID : 0;
                mShellDetail.IsRefShelfManager = true;
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        private void DeleteRefShelf(RefShelves DeleteRefShelfs)
        {
            this.ShowBusyIndicator();
            var ListDeleteRefShelfs = new ObservableCollection<RefShelves>();
            ListDeleteRefShelfs.Add(DeleteRefShelfs);
            if (ListDeleteRefShelfs.Count == 0 || ListDeleteRefShelfs == null)
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
                        contract.BeginUpdateRefShelves(Globals.LoggedUserAccount.Staff.StaffID, null, ListDeleteRefShelfs.ToList(), 0, true,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndUpdateRefShelves(asyncResult);
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                this.HideBusyIndicator();
                                Globals.EventAggregator.Publish(new RefShelfMedicalFileManagerEdit_Event() { Result = true });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
        public void hplDelete_Click(object selectItem)
        {
            try
            {
                RefShelves mRefShelf = selectItem as RefShelves;
                if (mRefShelf != null)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.T2013_G1_Ke + " " + mRefShelf.RefShelfName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DeleteRefShelf(mRefShelf);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
        //▲====: #002
    }
}
