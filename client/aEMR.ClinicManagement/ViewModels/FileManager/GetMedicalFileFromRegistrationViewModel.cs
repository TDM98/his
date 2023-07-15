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
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using aEMR.Common.Collections;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IGetMedicalFileFromRegistration)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GetMedicalFileFromRegistrationViewModel : Conductor<object>, IGetMedicalFileFromRegistration
    {
        #region Properties
        TextBox txtFileCode;
        private DateTime _StartDate = Globals.GetCurServerDateTime();
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                _StartDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }
        private DateTime _EndDate = Globals.GetCurServerDateTime();
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                _EndDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }
        private ObservableCollection<RefDepartment> _Departments;
        public ObservableCollection<RefDepartment> Departments
        {
            get { return _Departments; }
            set
            {
                _Departments = value;
                NotifyOfPropertyChange(() => Departments);
            }
        }
        private RefDepartment _SelectedDepartment;
        public RefDepartment SelectedDepartment
        {
            get { return _SelectedDepartment; }
            set
            {
                _SelectedDepartment = value;
                NotifyOfPropertyChange(() => SelectedDepartment);
                LoadLocations();
            }
        }
        private ObservableCollection<DeptLocation> _Locations;
        public ObservableCollection<DeptLocation> Locations
        {
            get
            {
                return _Locations;
            }
            set
            {
                _Locations = value;
                NotifyOfPropertyChange(() => Locations);
            }
        }
        private DeptLocation _SelectedLocation;
        public DeptLocation SelectedLocation
        {
            get
            {
                return _SelectedLocation;
            }
            set
            {

                _SelectedLocation = value;
                NotifyOfPropertyChange(() => SelectedLocation);
            }
        }
        private PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> _AllPatientMedicalFileStorageCheckOut = new PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut>();
        public PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut> AllPatientMedicalFileStorageCheckOut
        {
            get
            {
                return _AllPatientMedicalFileStorageCheckOut;
            }
            set
            {
                _AllPatientMedicalFileStorageCheckOut = value;
                NotifyOfPropertyChange(() => AllPatientMedicalFileStorageCheckOut);
            }
        }
        private ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> _ViewPatientMedicalFileStorageCheckOut = new PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut>();
        public ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> ViewPatientMedicalFileStorageCheckOut
        {
            get
            {
                return _ViewPatientMedicalFileStorageCheckOut;
            }
            set
            {
                _ViewPatientMedicalFileStorageCheckOut = value;
                NotifyOfPropertyChange(() => ViewPatientMedicalFileStorageCheckOut);
            }
        }
        private PatientMedicalFileStorageCheckInCheckOut _SelectedFileStorage;
        public PatientMedicalFileStorageCheckInCheckOut SelectedFileStorage
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
        private PatientMedicalFileStorageCheckInCheckOut _SelectedPatientMedicalFileStorageCheckOut;
        public PatientMedicalFileStorageCheckInCheckOut SelectedPatientMedicalFileStorageCheckOut
        {
            get
            {
                return _SelectedPatientMedicalFileStorageCheckOut;
            }
            set
            {
                _SelectedPatientMedicalFileStorageCheckOut = value;
                NotifyOfPropertyChange(() => SelectedPatientMedicalFileStorageCheckOut);
            }
        }
        private ObservableCollection<Staff> _AllStaff;
        public ObservableCollection<Staff> AllStaff
        {
            get { return _AllStaff; }
            set
            {
                if (_AllStaff == value)
                    return;
                _AllStaff = value;
                NotifyOfPropertyChange(() => AllStaff);
            }
        }
        private ObservableCollection<Staff> _AllStaffContext;
        public ObservableCollection<Staff> AllStaffContext
        {
            get { return _AllStaffContext; }
            set
            {
                _AllStaffContext = value;
                NotifyOfPropertyChange(() => AllStaffContext);
            }
        }
        private Staff _SelectedStaff;
        public Staff SelectedStaff
        {
            get { return _SelectedStaff; }
            set
            {
                if (_SelectedStaff != value)
                {
                    _SelectedStaff = value;
                    NotifyOfPropertyChange(() => SelectedStaff);
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
        private bool _IsBorrowed;
        public bool IsBorrowed
        {
            get { return _IsBorrowed; }
            set
            {
                _IsBorrowed = value;
                NotifyOfPropertyChange(() => IsBorrowed);
            }
        }
        private bool _IsStored = true;
        public bool IsStored
        {
            get { return _IsStored; }
            set
            {
                _IsStored = value;
                NotifyOfPropertyChange(() => IsStored);
            }
        }
        private bool _IsAllStaffs = false;
        public bool IsAllStaffs
        {
            get { return _IsAllStaffs; }
            set
            {
                _IsAllStaffs = value;
                NotifyOfPropertyChange(() => IsAllStaffs);
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
        public string ExportBy
        {
            get { return Globals.LoggedUserAccount.Staff.FullName; }
        }
        private int gPageSize = 20;
        private bool _IsAllChecked = false;
        public bool IsAllChecked
        {
            get { return _IsAllChecked; }
            set
            {
                _IsAllChecked = value;
                NotifyOfPropertyChange(() => IsAllChecked);
            }
        }
        private bool _VisibilityPaging = false;
        public bool VisibilityPaging
        {
            get
            {
                return _VisibilityPaging;
            }
            set
            {
                _VisibilityPaging = value;
                NotifyOfPropertyChange(() => VisibilityPaging);
            }
        }
        private DataGrid gvMedicalFileCheckOut;
        #endregion
        #region Events
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public GetMedicalFileFromRegistrationViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            LoadDepartments();
            GetAllStaffs();
            AllPatientMedicalFileStorageCheckOut.OnRefresh += AllPatientMedicalFileStorageCheckOut_OnRefresh;
            AllPatientMedicalFileStorageCheckOut.PageIndex = 0;
            AllPatientMedicalFileStorageCheckOut.PageSize = gPageSize;
        }
        public void gvMedicalFileCheckOut_Loaded(object sender, RoutedEventArgs e)
        {
            gvMedicalFileCheckOut = sender as DataGrid;
        }
        public void btnSearch()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetMedicalFileFromRegistration(SelectedDepartment.DeptID, SelectedLocation.Location.LID, StartDate.Date, EndDate.Date, IsBorrowed, IsStored, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var FileStorageCheckOut = contract.EndGetMedicalFileFromRegistration(asyncResult);
                                int xID = AllPatientMedicalFileStorageCheckOut.Count == 0 ? -1 : AllPatientMedicalFileStorageCheckOut.ToList().Min(x => Convert.ToInt32(x.PatientMedicalFileCheckoutID));
                                foreach (var item in FileStorageCheckOut)
                                {
                                    if (!AllPatientMedicalFileStorageCheckOut.ToList().Any(x => x.PtRegistrationID == item.PtRegistrationID))
                                    {
                                        if (item.PatientMedicalFileCheckoutID == 0)
                                        {
                                            item.PatientMedicalFileCheckoutID = xID;
                                            xID--;
                                        }
                                        AllPatientMedicalFileStorageCheckOut.Insert(0, item);
                                    }
                                }
                                ViewPatientMedicalFileStorageCheckOut = new ObservableCollection<PatientMedicalFileStorageCheckInCheckOut>(AllPatientMedicalFileStorageCheckOut.ToList().Take(gPageSize));
                                AllPatientMedicalFileStorageCheckOut.TotalItemCount = AllPatientMedicalFileStorageCheckOut.Count;
                                AllPatientMedicalFileStorageCheckOut.PageSize = gPageSize;
                                AllPatientMedicalFileStorageCheckOut.PageIndex = 0;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                FocusFileCode();
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
        public void txtFileCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.ShowBusyIndicator();
                FileCodeNumber = txtFileCode.Text;
                if (!string.IsNullOrEmpty(FileCodeNumber))
                {
                    SelectedPatientMedicalFileStorageCheckOut = AllPatientMedicalFileStorageCheckOut.Where(x => x.FileCodeNumber == FileCodeNumber || x.PatientCode == FileCodeNumber).FirstOrDefault();
                    if (SelectedPatientMedicalFileStorageCheckOut == null)
                    {
                        MessageBox.Show(eHCMSResources.Z1982_G1_KgTimThayHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                        FocusFileCode();
                        e.Handled = true;
                        return;
                    }
                    if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.OutStore)
                    {
                        MessageBox.Show(eHCMSResources.Z1983_G1_HSDaMuon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                        FocusFileCode();
                        e.Handled = true;
                        return;
                    }
                    //if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.UnSave)
                    //{
                    //    MessageBox.Show(eHCMSResources.Z1951_G1_KgTimThayKeChuaHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    //    this.HideBusyIndicator();
                    //    FocusFileCode();
                    //    e.Handled = true;
                    //    return;
                    //}
                    
                    int mIndex = AllPatientMedicalFileStorageCheckOut.IndexOf(SelectedPatientMedicalFileStorageCheckOut);
                    //TTM 30062018: (mIndex / gPageSize) ép kiểu về double. Vì Ceiling nó không biết nên xài parameter double hay decimal (Bên Cal xài double).
                    AllPatientMedicalFileStorageCheckOut.PageIndex = Convert.ToInt32(Math.Ceiling((double)(mIndex / gPageSize)));
                    AllPatientMedicalFileStorageCheckOut_OnRefresh(null, null);
                    if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore || SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.UnSave)
                        SelectedPatientMedicalFileStorageCheckOut.IsSelected = true;

                    EventHandler MyDataGrid_LayoutUpdated = null;
                    MyDataGrid_LayoutUpdated = (s1, e1) =>
                    {
                        gvMedicalFileCheckOut.ScrollIntoView(SelectedPatientMedicalFileStorageCheckOut, gvMedicalFileCheckOut.Columns[0]);
                        gvMedicalFileCheckOut.LayoutUpdated -= MyDataGrid_LayoutUpdated;
                        gvMedicalFileCheckOut.SelectedItem = SelectedPatientMedicalFileStorageCheckOut;
                    };
                    gvMedicalFileCheckOut.LayoutUpdated += MyDataGrid_LayoutUpdated;
                }
                this.HideBusyIndicator();
                FocusFileCode();
                e.Handled = true;
            }
        }
        public void txtFileCode_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtFileCode = sender as TextBox;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void btnUpdate()
        {
            this.ShowBusyIndicator();
            if (SelectedStaff == null)
            {
                MessageBox.Show(eHCMSResources.Z1952_G1_ChonTTinNgMuon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                this.HideBusyIndicator();
                FocusFileCode();
                return;
            }
            var CheckedFileStorages = AllPatientMedicalFileStorageCheckOut.Where(x => x.IsSelected == true).ToList();
            if (CheckedFileStorages.Count == 0)
            {
                MessageBox.Show(eHCMSResources.Z2043_G1_VuiLongXNHS, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                this.HideBusyIndicator();
                FocusFileCode();
                return;
            }
            var CheckoutDate = Globals.GetCurServerDateTime();
            CheckedFileStorages.ForEach(x => { x.CheckoutDate = CheckoutDate; x.CheckoutStaffID = Globals.LoggedUserAccount.Staff.StaffID; x.PatientMedicalFileCheckoutID = -1; x.StaffPersonID = SelectedStaff.StaffID; });
            List<PatientMedicalFileStorageCheckInCheckOut> CheckedFiles = new List<PatientMedicalFileStorageCheckInCheckOut>();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePatientMedicalFileStorageCheckOut(Globals.LoggedUserAccount.Staff.StaffID, CheckedFileStorages,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long MedicalFileStorageCheckID = 0;
                                if (!contract.EndUpdatePatientMedicalFileStorageCheckOut(out MedicalFileStorageCheckID, asyncResult))
                                    MessageBox.Show(eHCMSResources.Z1954_G1_CheckInCheckOutFail, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                else
                                {
                                    //MessageBox.Show(eHCMSResources.Z1955_G1_CheckInCheckOutSuccess, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    CheckedFiles = new List<PatientMedicalFileStorageCheckInCheckOut>(AllPatientMedicalFileStorageCheckOut.Where(x => x.IsSelected == true));
                                    while (AllPatientMedicalFileStorageCheckOut.Any(x => x.IsSelected == true))
                                        AllPatientMedicalFileStorageCheckOut.RemoveAt(AllPatientMedicalFileStorageCheckOut.IndexOf(AllPatientMedicalFileStorageCheckOut.Where(x => x.IsSelected == true).First()));
                                    PrintConfirm(CheckedFiles);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                FocusFileCode();
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
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedFileStorage != null)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z1953_G1_CoMuonXoaHS, SelectedFileStorage.FileCodeNumber), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        AllPatientMedicalFileStorageCheckOut.Remove(SelectedFileStorage);
                    FocusFileCode();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void btnClear()
        {
            AllPatientMedicalFileStorageCheckOut.Clear();
            AllPatientMedicalFileStorageCheckOut.PageIndex = 0;
            AllPatientMedicalFileStorageCheckOut.TotalItemCount = 0;
            ViewPatientMedicalFileStorageCheckOut.Clear();
        }
        public void btnPrint()
        {
            if (!ViewPatientMedicalFileStorageCheckOut.Any(x => !x.IsPrinted))
            {
                MessageBox.Show("Không có đăng ký để in!", eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
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
                        contract.BeginGetRegistrationXMLFromMedicalFileList(ViewPatientMedicalFileStorageCheckOut.Where(x => !x.IsPrinted).ToList(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var RegistrationXML = contract.EndGetRegistrationXMLFromMedicalFileList(asyncResult);

                                //var mPrintView = Globals.GetViewModel<ICommonPreviewView>();
                                //mPrintView.eItem = ReportName.MEDICALFILESLIST;
                                //mPrintView.FromDate = StartDate;
                                //mPrintView.ToDate = EndDate;
                                //mPrintView.FileCodeNumber = RegistrationXML;
                                //var mInstance = mPrintView as Conductor<object>;
                                //Globals.ShowDialog(mInstance, (o) => { });

                                Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                {
                                    mPrintView.eItem = ReportName.MEDICALFILESLIST;
                                    mPrintView.FromDate = StartDate;
                                    mPrintView.ToDate = EndDate;
                                    mPrintView.FileCodeNumber = RegistrationXML;
                                };
                                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

                                DateTime mToday = DateTime.Now;
                                foreach (var item in ViewPatientMedicalFileStorageCheckOut.Where(x => x.IsPrinted == false))
                                {
                                    AllPatientMedicalFileStorageCheckOut.Where(x => x.PtRegistrationID == item.PtRegistrationID).FirstOrDefault().PrintedDate = mToday;
                                }

                                AllPatientMedicalFileStorageCheckOut_OnRefresh(null, null);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                FocusFileCode();
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
        public void btnReview()
        {
            //var vm = Globals.GetViewModel<IMedicalFileCheckOutHistory>();
            //Globals.ShowDialog(vm as Conductor<object>);
            GlobalsNAV.ShowDialog<IMedicalFileCheckOutHistory>();
        }
        public void cboBorrowBy_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            AllStaffContext = new ObservableCollection<Staff>(AllStaff.Where(x => x.FullName.ToLower().Contains(cboContext.SearchText.ToLower())));
            cboContext.ItemsSource = AllStaffContext;
            cboContext.PopulateComplete();
        }
        public void cboBorrowBy_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void btnImportFile()
        {
            //var vm = Globals.GetViewModel<IRefShelfImportFile>();
            //vm.MaxWidth = 800;
            //Globals.ShowDialog(vm as Conductor<object>);
            Action<IRefShelfImportFile> onInitDlg = (vm) =>
            {
                vm.MaxWidth = 800;
            };
            GlobalsNAV.ShowDialog<IRefShelfImportFile>(onInitDlg);
        }
        public void btnCheckInFile()
        {
            //var vm = Globals.GetViewModel<IRefMedicalFileCheckIn>();
            //vm.IsCheckIn = true;
            //vm.MaxWidth = 800;
            //Globals.ShowDialog(vm as Conductor<object>);

            Action<IRefMedicalFileCheckIn> onInitDlg = (vm) =>
            {
                vm.IsCheckIn = true;
                vm.MaxWidth = 800;
            };
            GlobalsNAV.ShowDialog<IRefMedicalFileCheckIn>(onInitDlg);
        }
        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                gPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
                btnSearch();
            }
        }
        #endregion
        #region Method
        private void LoadDepartments()
        {
            try
            {
                if (Globals.AllRefDepartmentList == null || Globals.AllRefDepartmentList.Count == 0)
                {
                    return;
                }
                Departments = null;
                ObservableCollection<RefDepartment> loadingDepts = new ObservableCollection<RefDepartment>();
                loadingDepts = new ObservableCollection<RefDepartment>();
                foreach (var gDept in Globals.AllRefDepartmentList)
                {
                    loadingDepts.Add(gDept);
                }
                RefDepartment firstItem = new RefDepartment();
                firstItem.DeptID = 0;
                firstItem.DeptName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0493_G1_HayChonKhoa);
                loadingDepts.Insert(0, firstItem);
                Departments = loadingDepts;
                SetDefaultRefDept();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        private void SetDefaultRefDept()
        {
            SelectedDepartment = Departments.FirstOrDefault();
        }
        private void LoadLocations()
        {
            this.ShowBusyIndicator();
            if (SelectedDepartment.DeptID > 0)
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLocationsByDeptID(SelectedDepartment.DeptID, null, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var allItems = contract.EndGetAllLocationsByDeptID(asyncResult);
                                if (allItems != null)
                                {
                                    Locations = new ObservableCollection<DeptLocation>(allItems);
                                    var itemDefault = new DeptLocation();
                                    itemDefault.Location = new Location();
                                    itemDefault.Location.LID = -1;
                                    itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                                    Locations.Insert(0, itemDefault);
                                    SelectedLocation = itemDefault;
                                }
                                else
                                {
                                    Locations = new ObservableCollection<DeptLocation>();
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
                        }), null);
                    }
                });
                t.Start();
            }
            else
            {
                try
                {
                    Locations = new ObservableCollection<DeptLocation>();
                    var itemDefault = new DeptLocation();
                    itemDefault.Location = new Location();
                    itemDefault.Location.LID = -1;
                    itemDefault.Location.LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0116_G1_HayChonPg);
                    Locations.Insert(0, itemDefault);
                    SelectedLocation = itemDefault;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
                this.HideBusyIndicator();
            }
        }
        private void FocusFileCode()
        {
            txtFileCode.Text = "";
            txtFileCode.Focus();
        }
        private void GetAllStaffs()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllStaffs(Globals.DispatchCallback((asyncResult) =>
                        {
                            AllStaff = new ObservableCollection<Staff>(contract.EndGetAllStaffs(asyncResult));
                            this.HideBusyIndicator();
                            FocusFileCode();
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
        private void AllPatientMedicalFileStorageCheckOut_OnRefresh(object sender, RefreshEventArgs e)
        {
            ViewPatientMedicalFileStorageCheckOut = new ObservableCollection<PatientMedicalFileStorageCheckInCheckOut>(AllPatientMedicalFileStorageCheckOut.ToList().Skip(AllPatientMedicalFileStorageCheckOut.PageIndex * gPageSize).Take(gPageSize));
        }
        private void PrintConfirm(List<PatientMedicalFileStorageCheckInCheckOut> CheckedFiles)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistrationXMLFromMedicalFileList(CheckedFiles, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var RegistrationXML = contract.EndGetRegistrationXMLFromMedicalFileList(asyncResult);

                                //var mPrintView = Globals.GetViewModel<ICommonPreviewView>();
                                //mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                //mPrintView.FromDate = StartDate;
                                //mPrintView.ToDate = EndDate;
                                //mPrintView.FileCodeNumber = RegistrationXML;
                                //mPrintView.IssueName = Globals.LoggedUserAccount.Staff.FullName;
                                //mPrintView.ReceiveName = SelectedStaff.FullName;
                                //mPrintView.AdmissionDate = DateTime.Now.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
                                //var mInstance = mPrintView as Conductor<object>;
                                //Globals.ShowDialog(mInstance, (o) => { });

                                Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                {
                                    mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                    mPrintView.FromDate = StartDate;
                                    mPrintView.ToDate = EndDate;
                                    mPrintView.FileCodeNumber = RegistrationXML;
                                    mPrintView.IssueName = Globals.LoggedUserAccount.Staff.FullName;
                                    mPrintView.ReceiveName = SelectedStaff.FullName;
                                    mPrintView.AdmissionDate = DateTime.Now.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
                                };
                                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);

                                AllPatientMedicalFileStorageCheckOut_OnRefresh(null, null);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                FocusFileCode();
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
        #endregion
    }
}
