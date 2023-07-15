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

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefRow)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefRowViewModel : Conductor<object>, IRefRow, IHandle<RefRowMedicalFileManagerEdit_Event>
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
        private ObservableCollection<RefRows> _AllRefRows = new ObservableCollection<RefRows>();
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
        private RefRows _SelectedRefRow;
        public RefRows SelectedRefRow
        {
            get
            {
                return _SelectedRefRow;
            }
            set
            {
                if (_SelectedRefRow != value)
                {
                    _SelectedRefRow = value;
                    NotifyOfPropertyChange(() => SelectedRefRow);
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
                    if(_SelectedStore != null)
                    {
                        LoadRefRows();
                    }
                    else
                    {
                        AllRefRows = new ObservableCollection<RefRows>();
                    }
                }
            }
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefRowViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Globals.EventAggregator.Subscribe(this);
            LoadStores();
            CreateSubVM();
        }
        public void hplEdit_Click(object selectItem)
        {
            try
            {
                if (selectItem != null)
                {
                    RefRows mRefRow = selectItem as RefRows;
                    if (mRefRow.RefRowID > 0)
                    {
                        Action<IRefRowShelfEditViewModel> onInitDlg = (mShellDetail) =>
                        {
                            mShellDetail.StoreID = SelectedStore.StoreID;
                            mShellDetail.RefRowID = mRefRow.RefRowID;
                            mShellDetail.IsRefRowManager = true;
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
        public void hplDelete_Click(object selectItem)
        {
            try
            {
                RefRows mRefRow = selectItem as RefRows;
                if (mRefRow != null)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa, eHCMSResources.Z3275_G1_Day + " " + mRefRow.RefRowName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DeleteRefRows(mRefRow);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void gvRefRows_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Row.GetIndex() == (AllRefRows.Count - 1) && e.EditAction == DataGridEditAction.Commit && AllRefRows.Last().Validate() && !string.IsNullOrEmpty(AllRefRows.Last().RefRowName))
            {
                AddBlank();
            }
        }
        public void cboStore_LostFocus(object sender, RoutedEventArgs e)
        {
            //SelectedRefRowDetail.CaseCode = (sender as eHCMS.ControlsLibrary.AxAutoComplete).SearchText;
        }
        public void cboStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as RefStorageWarehouseLocation;
                SelectedRefRow.StoreID = item.StoreID;
                SelectedRefRow.swhlName = item.swhlName;
            }
        }
        public void btnSearch()
        {
            IsAdding = false;
            IsUpdating = false;
            if (SelectedStore == null)
            {
                MessageBox.Show(eHCMSResources.K0310_G1_ChonKhoCanXem, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                this.HideBusyIndicator();
                return;
            }
            LoadRefRows();
        }
        #region Method
        private void AddBlank()
        {
            if (IsAdding)
            {
                try
                {
                    RefRows mRefRow = new RefRows { CreatedDate = Globals.GetCurServerDateTime() };
                    if (AllRefRows.Count == 0)
                        mRefRow.RefRowID = -1;
                    else
                        mRefRow.RefRowID = AllRefRows.Min(x => x.RefRowID) > 0 ? -1 : AllRefRows.Min(x => x.RefRowID) - 1;
                    //mRefRow.RefRowCode = AllRefRows.Count == 0 ? "D1" : "D" + (Convert.ToInt32(AllRefRows.Max(x => x.RefRowCode).Replace("D", "")) + 1).ToString();
                    mRefRow.AllStores = this.AllStores.ToList();
                    mRefRow.StoreID = SavedStore.StoreID;
                    mRefRow.swhlName = SavedStore.swhlName;
                    AllRefRows.Add(mRefRow);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
            }
        }
        private void LoadRefRows()
        {
            if(SelectedStore == null)
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
                        contract.BeginGetRefRows(SelectedStore.StoreID, RefRowName, 0,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefRows = contract.EndGetRefRows(asyncResult);
                                foreach (var item in AllRefRows)
                                    item.AllStores = this.AllStores.ToList();
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
        public IRefShelf UCRefShelf { get; set; }
        public IRefShelfDetail UCRefShelfDetail { get; set; }

        private void CreateSubVM()
        {
            UCRefShelf = Globals.GetViewModel<IRefShelf>();
            UCRefShelfDetail = Globals.GetViewModel<IRefShelfDetail>();
        }

        protected override void OnDeactivate(bool close)
        {
            DeactivateItem(UCRefShelf, close);
            UCRefShelf = null;
            DeactivateItem(UCRefShelfDetail, close);
            UCRefShelfDetail = null;
            base.OnDeactivate(close);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ActivateItem(UCRefShelf);
            ActivateItem(UCRefShelfDetail);
        }
        //public void cboRefRows_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    SelectedRefRow = ((AutoCompleteBox)sender).SelectedItem as RefRows;
        //}
        private void SetDefaultSelectedStore()
        {
            if(AllStores != null)
            {
                SelectedStore = AllStores.FirstOrDefault();
            }
        }
        public void btnAdd()
        {
            Action<IRefRowShelfEditViewModel> onInitDlg = (mShellDetail) =>
            {
                mShellDetail.StoreID = SelectedStore != null ? SelectedStore.StoreID : 0;
                mShellDetail.IsRefRowManager = true;
            };
            GlobalsNAV.ShowDialog(onInitDlg);
        }
        private string _RefRowName;
        public string RefRowName
        {
            get
            {
                return _RefRowName;
            }
            set
            {
                if (_RefRowName != value)
                {
                    _RefRowName = value;
                    NotifyOfPropertyChange(() => RefRowName);
                }
            }
        }
        public bool IsDisableEdit
        {
            get
            {
                return !IsAdding && !IsUpdating;
            }
        }
        private bool _IsAdding;
        public bool IsAdding
        {
            get
            {
                return _IsAdding;
            }
            set
            {
                if (_IsAdding != value)
                {
                    _IsAdding = value;
                    NotifyOfPropertyChange(() => IsAdding);
                    NotifyOfPropertyChange(() => IsDisableEdit);
                }
            }
        }
        private bool _IsUpdating;
        public bool IsUpdating
        {
            get
            {
                return _IsUpdating;
            }
            set
            {
                if (_IsUpdating != value)
                {
                    _IsUpdating = value;
                    NotifyOfPropertyChange(() => IsUpdating);
                    NotifyOfPropertyChange(() => IsDisableEdit);
                }
            }
        }
        public void DeleteRefRows(RefRows RefRowDeleted)
        {
            this.ShowBusyIndicator();
            var ListRefRowDeleted = new ObservableCollection<RefRows>();
            ListRefRowDeleted.Add(RefRowDeleted);
            if (ListRefRowDeleted == null || ListRefRowDeleted.Count == 0)
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
                        contract.BeginUpdateRefRows(Globals.LoggedUserAccount.Staff.StaffID, null, ListRefRowDeleted.ToList(), SelectedStore.StoreID,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndUpdateRefRows(asyncResult);
                                MessageBox.Show(eHCMSResources.A0478_G1_Msg_InfoXoaOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                Globals.EventAggregator.Publish(new RefRowMedicalFileManagerEdit_Event() { Result = true });
                                this.HideBusyIndicator();
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

        public void Handle(RefRowMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadStores();
            }
        }
    }
}
