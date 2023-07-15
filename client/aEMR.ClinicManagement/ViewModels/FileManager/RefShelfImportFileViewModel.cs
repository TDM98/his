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
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Infrastructure.Events;
/*
* 20220811 #001 QTD: Chỉnh sửa chức năng đặt hồ sơ vào kệ
*/
namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefShelfImportFile)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefShelfImportFileViewModel : Conductor<object>, IRefShelfImportFile
        , IHandle<RefRowMedicalFileManagerEdit_Event>
        , IHandle<RefShelfMedicalFileManagerEdit_Event>
        , IHandle<RefShelfDetailsMedicalFileManagerEdit_Event>
    {
        #region Properties
        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                _IsBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }
        public string BusyMessage { get { return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0995_G1_HThongDangXuLi); } }
        private string _ShelfCode;
        public string ShelfCode
        {
            get
            {
                return _ShelfCode;
            }
            set
            {
                if (_ShelfCode != value)
                {
                    _ShelfCode = value;
                    NotifyOfPropertyChange(() => ShelfCode);
                }
            }
        }
        private string _ShelfName;
        public string ShelfName
        {
            get
            {
                return _ShelfName;
            }
            set
            {
                if (_ShelfName != value)
                {
                    _ShelfName = value;
                    NotifyOfPropertyChange(() => ShelfName);
                }
            }
        }
        private string _FileCodeNumber;
        public string FileCodeNumber
        {
            get
            {
                return _FileCodeNumber;
            }
            set
            {
                if (_FileCodeNumber != value)
                {
                    _FileCodeNumber = value;
                    NotifyOfPropertyChange(() => FileCodeNumber);
                }
            }
        }
        private string _LocCode;
        public string LocCode
        {
            get
            {
                return _LocCode;
            }
            set
            {
                if (_LocCode != value)
                {
                    _LocCode = value;
                    NotifyOfPropertyChange(() => LocCode);
                }
            }
        }
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                if (_ErrorMessage != value)
                {
                    _ErrorMessage = value;
                    NotifyOfPropertyChange(() => ErrorMessage);
                }
            }
        }
        private long _PatientMedicalFileID;
        public long PatientMedicalFileID
        {
            get
            {
                return _PatientMedicalFileID;
            }
            set
            {
                if (_PatientMedicalFileID != value)
                {
                    _PatientMedicalFileID = value;
                    NotifyOfPropertyChange(() => PatientMedicalFileID);
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
        private ObservableCollection<RefShelfDetails> _AllRefShelfDetails;
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
                if (value != _SelectedRefShelfDetail)
                {
                    _SelectedRefShelfDetail = value;
                    NotifyOfPropertyChange(() => SelectedRefShelfDetail);
                    //RefShelfDetailID = SelectedRefShelfDetail.RefShelfDetailID;
                    //LoadShelf(SelectedRefShelfDetail.RefShelfID);
                }
            }
        }
        private ObservableCollection<RefShelves> _AllRefShelfs;
        public ObservableCollection<RefShelves> AllRefShelfs
        {
            get
            {
                return _AllRefShelfs;
            }
            set
            {
                if (_AllRefShelfs != value)
                {
                    _AllRefShelfs = value;
                    NotifyOfPropertyChange(() => AllRefShelfs);
                }
            }
        }
        private ObservableCollection<PatientMedicalFileStorage> _AllPatientMedicalFileStorage = new ObservableCollection<PatientMedicalFileStorage>();
        public ObservableCollection<PatientMedicalFileStorage> AllPatientMedicalFileStorage
        {
            get
            {
                return _AllPatientMedicalFileStorage;
            }
            set
            {
                if (_AllPatientMedicalFileStorage != value)
                {
                    _AllPatientMedicalFileStorage = value;
                    NotifyOfPropertyChange(() => AllPatientMedicalFileStorage);
                }
            }
        }
        private PatientMedicalFileStorage _SelectedFileStorage;
        public PatientMedicalFileStorage SelectedFileStorage
        {
            get
            {
                return _SelectedFileStorage;
            }
            set
            {
                _SelectedFileStorage = value;
                NotifyOfPropertyChange(() => SelectedFileStorage);
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
                    LoadRefRows();
                }
            }
        }
        TextBox txtFileCode;
        DataGrid gvShelfImport;
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
        #endregion
        #region Event
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefShelfImportFileViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg, ISalePosCaching salePosCaching)
        {
            eventArg.Subscribe(this);
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
           
            LoadStores();
            SelectedPatientMedicalFileStorage = new ObjectEdit<PatientMedicalFileStorage>("FileCreatedDate", "ProgDateFrom", "");
            V_MedicalFileType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_MedicalFileType).ToObservableCollection();
            V_ExpiryTime = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ExpiryTime).ToObservableCollection();
            SetDefaultV_MedicalFileType();
        }
        private void LoadShelf()
        {
            if(SelectedRow == null || SelectedRow.RefRowID <= 0)
            {
                AllRefShelfs = new ObservableCollection<RefShelves>();
                AddFirstItemRefShelf();
                return;
            }    
            IsBusy = true;
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
                                AllRefShelfs = contract.EndGetRefShelves(asyncResult);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                AddFirstItemRefShelf();
                                IsBusy = false;
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
                    IsBusy = false;
                    //LoadFileStorage(RefShelfDetailID);
                }
            });
            t.Start();
        }
        private void LoadFileStorage(long RefShelfDetailID)
        {
            IsBusy = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientMedicalFileStorage(RefShelfDetailID, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            long PatientMedicalFileID;
                            AllPatientMedicalFileStorage = contract.EndGetPatientMedicalFileStorage(out PatientMedicalFileID, asyncResult);
                            IsBusy = false;
                            FocusFileCode();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    IsBusy = false;
                }
            });
            t.Start();
        }
        private void LoadShelfDetails()
        {
            if(SelectedRefShelf == null || SelectedRefShelf.RefShelfID <= 0)
            {
                AllRefShelfDetails = new ObservableCollection<RefShelfDetails>();
                AddFirstItemRefShelfDetails();
                return;
            }
            IsBusy = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelfDetails(SelectedRefShelf.RefShelfID, null, 0, null,
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

                                AddFirstItemRefShelfDetails();
                                IsBusy = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    IsBusy = false;
                }
            });
            t.Start();
        }
        //public void txtShelfCode_KeyUp(object sender, KeyEventArgs e)
        //{
        //    ErrorMessage = null;
        //    if (e.Key == Key.Enter)
        //    {
        //        LocCode = (sender as TextBox).Text;
        //        this.ShowBusyIndicator();
        //        var t = new Thread(() =>
        //        {
        //            try
        //            {
        //                using (var serviceFactory = new ClinicManagementServiceClient())
        //                {
        //                    var contract = serviceFactory.ServiceInstance;
        //                    contract.BeginGetRefShelfDetails(0, LocCode,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        try
        //                        {
        //                            AllRefShelfDetails = contract.EndGetRefShelfDetails(asyncResult);
        //                            if (AllRefShelfDetails.Count == 1)
        //                            {
        //                                RefShelfDetailID = AllRefShelfDetails.First().RefShelfDetailID;
        //                                LoadShelf(AllRefShelfDetails.First().RefShelfID);
        //                            }
        //                            else
        //                            {
        //                                RefShelfDetailID = 0;
        //                                //MessageBox.Show(eHCMSResources.Z1956_G1_KgTimThayKeChuaNganNay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //                                ShelfCode = null;
        //                                if (string.IsNullOrEmpty(LocCode))
        //                                    LoadFileStorage(0);
        //                                else
        //                                    AllPatientMedicalFileStorage = new ObservableCollection<PatientMedicalFileStorage>();
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                        }
        //                        finally
        //                        {
        //                            IsBusy = false;
        //                        }
        //                    }), null);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                IsBusy = false;
        //            }
        //        });
        //        t.Start();
        //    }
        //}
        //public void txtFileCode_KeyUp(object sender, KeyEventArgs e)
        //{
        //    ErrorMessage = null;
        //    if (SelectedRefShelfDetail == null)
        //    {
        //        MessageBox.Show(eHCMSResources.Z2000_G1_ChonNganDatHSo, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        FocusFileCode();
        //        e.Handled = true;
        //        return;
        //    }
        //    if (e.Key == Key.Enter)
        //    {
        //        IsBusy = true;
        //        FileCodeNumber = txtFileCode.Text;
        //        var t = new Thread(() =>
        //        {
        //            try
        //            {
        //                using (var serviceFactory = new ClinicManagementServiceClient())
        //                {
        //                    var contract = serviceFactory.ServiceInstance;
        //                    contract.BeginGetPatientMedicalFileStorage(0, FileCodeNumber,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        if (string.IsNullOrEmpty(FileCodeNumber))
        //                        {
        //                            IsBusy = false;
        //                            FocusFileCode();
        //                            return;
        //                        }
        //                        if (AllPatientMedicalFileStorage.Any(x => x.FileCodeNumber == FileCodeNumber))
        //                        {
        //                            ErrorMessage = eHCMSResources.Z1957_G1_HSDaTonTai;
        //                            SelectedFileStorage = AllPatientMedicalFileStorage.Where(x => x.FileCodeNumber == FileCodeNumber).First();
        //                            gvShelfImport.ScrollIntoView(SelectedFileStorage, gvShelfImport.Columns[0]);
        //                            IsBusy = false;
        //                            FocusFileCode();
        //                            return;
        //                        }
        //                        long PatientMedicalFileID;
        //                        var GettedPatientMedicalFileStorage = contract.EndGetPatientMedicalFileStorage(out PatientMedicalFileID, asyncResult);
        //                        this.PatientMedicalFileID = PatientMedicalFileID;
        //                        if (PatientMedicalFileID == 0)
        //                        {
        //                            ErrorMessage = eHCMSResources.Z1958_G1_SoHSKgHopLe;
        //                            IsBusy = false;
        //                            FocusFileCode();
        //                            return;
        //                        }
        //                        if (GettedPatientMedicalFileStorage.Count == 0)
        //                        {
        //                            AllPatientMedicalFileStorage.Add(new PatientMedicalFileStorage { FileCodeNumber = FileCodeNumber, LocCode = null, CreatedDate = Globals.GetCurServerDateTime(), RefShelfDetailID = SelectedRefShelfDetail.RefShelfDetailID, PatientMedicalFileID = PatientMedicalFileID });
        //                            var AddedItem = AllPatientMedicalFileStorage.LastOrDefault();
        //                            SelectedFileStorage = AddedItem;
        //                            gvShelfImport.ScrollIntoView(SelectedFileStorage, gvShelfImport.Columns[0]);
        //                        }
        //                        else
        //                            ErrorMessage = eHCMSResources.Z1959_G1_HSDaTonTaiTrongNganKhac;
        //                        IsBusy = false;
        //                        FocusFileCode();
        //                    }), null);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                IsBusy = false;
        //            }
        //        });
        //        t.Start();
        //    }
        //}
        //public void txtFileCode_Loaded(object sender, RoutedEventArgs e)
        //{
        //    txtFileCode = sender as TextBox;
        //}
        public void gvShelfImport_Loaded(object sender, RoutedEventArgs e)
        {
            gvShelfImport = sender as DataGrid;
        }
        public void btnUpdate()
        {
            IsBusy = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePatientMedicalFileStorage(SelectedRefShelfDetail.RefShelfDetailID, SelectedPatientMedicalFileStorage.TempObject.ToList()
                            , Globals.LoggedUserAccount.StaffID.Value,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            contract.EndUpdatePatientMedicalFileStorage(asyncResult).ToObservableCollection();
                            IsBusy = false;
                            SelectedPatientMedicalFileStorage.Clear();
                            MessageBox.Show(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            //FocusFileCode();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    IsBusy = false;
                }
                finally
                {
                    btnSearch();
                }
            });
            t.Start();
        }
        public void btnShelfMan()
        {
            //var vm = Globals.GetViewModel<IRefShelf>();
            //vm.MaxWidth = 800;
            //Globals.ShowDialog(vm as Conductor<object>);

            //Action<IRefShelf> onInitDlg = (vm) =>
            //{
            //    vm.MaxWidth = 1300;
            //};
            //GlobalsNAV.ShowDialog<IRefShelf>(onInitDlg);

            Action<IRefRow> onInitDlg = (vm) =>
            {
                vm.MaxWidth = 1250;
            };
            GlobalsNAV.ShowDialog(onInitDlg);

        }
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedFileStorage != null)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z1953_G1_CoMuonXoaHS, SelectedFileStorage.FileCodeNumber), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        AllPatientMedicalFileStorage.Remove(SelectedFileStorage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void cboRefShelfDetail_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedRefShelfDetail = ((AutoCompleteBox)sender).SelectedItem as RefShelfDetails;
        }
        #endregion
        #region Method
        private void FocusFileCode()
        {
            txtFileCode.Text = "";
            txtFileCode.Focus();
        }
        private void LoadStores()
        {
            IsBusy = true;
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
                                    if(AllStores != null && AllStores.Count > 0)
                                    {
                                        SelectedStore = AllStores.FirstOrDefault();
                                    }    
                                    IsBusy = false;
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    IsBusy = false;
                }
            });
            t.Start();
        }
        #endregion

        //▼====: #001
        private DateTime _FromDate = Globals.GetCurServerDateTime();
        public DateTime FromDate
        {
            get => _FromDate;
            set
            {
                _FromDate = value;
                NotifyOfPropertyChange(() => FromDate);
            }
        }
        private DateTime _ToDate = Globals.GetCurServerDateTime();
        public DateTime ToDate
        {
            get => _ToDate;
            set
            {
                _ToDate = value;
                NotifyOfPropertyChange(() => ToDate);
            }
        }
        private string _PatientName;
        public string PatientName
        {
            get
            {
                return _PatientName;
            }
            set
            {
                if (_PatientName != value)
                {
                    _PatientName = value;
                    NotifyOfPropertyChange(() => PatientName);
                }
            }
        }
        private ObjectEdit<PatientMedicalFileStorage> _SelectedPatientMedicalFileStorage;
        public ObjectEdit<PatientMedicalFileStorage> SelectedPatientMedicalFileStorage
        {
            get
            {
                return _SelectedPatientMedicalFileStorage;
            }
            set
            {
                if (_SelectedPatientMedicalFileStorage != value)
                {
                    _SelectedPatientMedicalFileStorage = value;
                    NotifyOfPropertyChange(() => SelectedPatientMedicalFileStorage);
                }
            }
        }
        private ObservableCollection<Lookup> _V_MedicalFileType;
        public ObservableCollection<Lookup> V_MedicalFileType
        {
            get
            {
                return _V_MedicalFileType;
            }
            set
            {
                if (_V_MedicalFileType != value)
                {
                    _V_MedicalFileType = value;
                    NotifyOfPropertyChange(() => V_MedicalFileType);
                }
            }
        }
        private ObservableCollection<Lookup> _V_ExpiryTime;
        public ObservableCollection<Lookup> V_ExpiryTime
        {
            get
            {
                return _V_ExpiryTime;
            }
            set
            {
                if (_V_ExpiryTime != value)
                {
                    _V_ExpiryTime = value;
                    NotifyOfPropertyChange(() => V_ExpiryTime);
                }
            }
        }
        private Lookup _SelectedMedicalFileType;
        public Lookup SelectedMedicalFileType
        {
            get
            {
                return _SelectedMedicalFileType;
            }
            set
            {
                if (_SelectedMedicalFileType != value)
                {
                    _SelectedMedicalFileType = value;
                    NotifyOfPropertyChange(() => SelectedMedicalFileType);
                }
            }
        }
        private void SetDefaultV_MedicalFileType()
        {
            if (V_MedicalFileType != null)
            {
                SelectedMedicalFileType = V_MedicalFileType.FirstOrDefault();
            }
        }
        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ((ComboBox)sender).ItemsSource = V_ExpiryTime;
            if (SelectedFileStorage != null && V_ExpiryTime != null)
            {
                if (SelectedFileStorage.ExpiryTime == null)
                {
                    ((ComboBox)sender).SelectedIndex = 0;
                }
                else
                {
                    ((ComboBox)sender).SelectedItem = SelectedFileStorage.ExpiryTime;
                }
            }
        }
        public void btnSearch()
        {
            this.ShowBusyIndicator();
            if(FromDate == null || ToDate == null)
            {
                MessageBox.Show("Chọn từ ngày đến ngày!");
                this.HideBusyIndicator();
                return;
            }
            if (ToDate < FromDate)
            {
                MessageBox.Show(eHCMSResources.A0857_G1_Msg_InfoNgThangKhHopLe2);
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
                        contract.BeginSearchPatientMedicalFileManager(SelectedMedicalFileType.LookupID, FileCodeNumber, PatientName, FromDate, ToDate, 
                        Globals.DispatchCallback(asyncResult =>
                        {
                            try
                            {
                                var result = contract.EndSearchPatientMedicalFileManager(asyncResult);
                                AllPatientMedicalFileStorage.Clear();
                                SelectedPatientMedicalFileStorage.Clear();
                                if (result != null && result.Count > 0)
                                {
                                    AllPatientMedicalFileStorage = result;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void DoubleClick()
        {
            PatientMedicalFileStorage PtMedicalFile = new PatientMedicalFileStorage();
            PtMedicalFile = SelectedFileStorage;
            if(PtMedicalFile == null)
            {
                MessageBox.Show("Chưa chọn hồ sơ!");
                return;
            }
            if (string.IsNullOrEmpty(PtMedicalFile.ExpiryTime))
            {
                MessageBox.Show("Chưa chọn hạn lưu trữ!");
                return;
            }
            if (!SelectedPatientMedicalFileStorage.Add(PtMedicalFile))
            {
                MessageBox.Show("Hồ sơ này đã có rồi!");
                return;
            }
            else
            {
                AllPatientMedicalFileStorage.Remove(PtMedicalFile);
            }
        }
        public void btAddChoose()
        {
            DoubleClick();
        }
        public void btSaveItems()
        {
            if (SelectedStore == null || SelectedStore.StoreID <= 0)
            {
                MessageBox.Show("Chưa chọn Kho!");
                return;
            }
            if (SelectedRow == null || SelectedRow.RefRowID <= 0)
            {
                MessageBox.Show("Chưa chọn Dãy!");
                return;
            }
            if (SelectedRefShelf == null || SelectedRefShelf.RefShelfID <= 0)
            {
                MessageBox.Show("Chưa chọn Kệ!");
                return;
            }
            if (SelectedRefShelfDetail == null || SelectedRefShelfDetail.RefShelfDetailID <= 0)
            {
                MessageBox.Show("Chưa chọn Ngăn!");
                return;
            }
            if(SelectedPatientMedicalFileStorage.TempObject != null && SelectedPatientMedicalFileStorage.TempObject.Count > 0)
            {
                btnSave();
            }
        }
        public void hplDeleteService_Loaded(object sender)
        {
            hplDeleteService = sender as Button;
            //hplDeleteService.Visibility = Globals.convertVisibility(bhplDeleteService);
        }
        public Button hplDeleteService { get; set; }
        public void hplDeleteService_Click(object datacontext)
        {
            PatientMedicalFileStorage p = datacontext as PatientMedicalFileStorage;
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z0476_G1_BanMuonXoa,"hồ sơ bệnh nhân " + p.FullName), eHCMSResources.G2773_G1_XoaDV, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SelectedPatientMedicalFileStorage.Remove(p);
                    p.ExpiryTime = null;
                    AllPatientMedicalFileStorage.Add(p);
                }
            }
        }
        public void btnSave()
        {
            IsBusy = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPatientMedicalFileStorage_InsertXML(SelectedRefShelfDetail.RefShelfDetailID, SelectedPatientMedicalFileStorage.TempObject.ToList()
                            , Globals.LoggedUserAccount.StaffID.Value,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            contract.EndPatientMedicalFileStorage_InsertXML(asyncResult).ToObservableCollection();
                            IsBusy = false;
                            SelectedPatientMedicalFileStorage.Clear();
                            MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            //FocusFileCode();
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    IsBusy = false;
                }
                finally
                {
                    btnSearch();
                }
            });
            t.Start();
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
                    LoadShelfDetails();
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
                    LoadShelf();
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
        private void LoadRefRows()
        {
            if (SelectedStore == null || SelectedStore.StoreID <= 0)
            {
                AllRefRows = new ObservableCollection<RefRows>();
                AddFirstItemRefRow();
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
                        contract.BeginGetRefRows(SelectedStore.StoreID, null, 0,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    AllRefRows = contract.EndGetRefRows(asyncResult);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
        public void Handle(RefRowMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadRefRows();
            }
        }
        public void Handle(RefShelfMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadShelf();
            }
        }
        public void Handle(RefShelfDetailsMedicalFileManagerEdit_Event message)
        {
            if (message != null)
            {
                LoadShelfDetails();
            }
        }
        private void AddFirstItemRefRow()
        {
            RefRows firstItem = new RefRows();
            firstItem.RefRowID = 0;
            firstItem.RefRowName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3283_G1_ChonDay);
            AllRefRows.Insert(0, firstItem);
            SelectedRow = AllRefRows.FirstOrDefault();
        }
        private void AddFirstItemRefShelf()
        {
            RefShelves firstItem = new RefShelves();
            firstItem.RefShelfID = 0;
            firstItem.RefShelfName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3284_G1_ChonKe);
            AllRefShelfs.Insert(0, firstItem);
            SelectedRefShelf = AllRefShelfs.FirstOrDefault();
        }
        private void AddFirstItemRefShelfDetails()
        {
            RefShelfDetails firstItem = new RefShelfDetails();
            firstItem.RefShelfDetailID = 0;
            firstItem.LocName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3285_G1_ChonNgan);
            AllRefShelfDetails.Insert(0, firstItem);
            SelectedRefShelfDetail = AllRefShelfDetails.FirstOrDefault();
        }
        //▲====: #001
    }
}
