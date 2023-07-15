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
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common.Collections;
using aEMR.Controls;

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefMedicalFileCheckOut)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalFileCheckOutViewModel : Conductor<object>, IRefMedicalFileCheckOut
    {
        #region Properties
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
                    LoadShelf();
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
                if (_SelectedRefShelfDetail != value)
                {
                    _SelectedRefShelfDetail = value;
                    NotifyOfPropertyChange(() => SelectedRefShelfDetail);
                }
            }
        }
        private int _BorrowingDay;
        public int BorrowingDay
        {
            get { return _BorrowingDay; }
            set
            {
                _BorrowingDay = value;
                NotifyOfPropertyChange(() => BorrowingDay);
            }
        }
        private string _Notes;
        public string Notes
        {
            get { return _Notes; }
            set
            {
                _Notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        }
        private string _CheckinStatus;
        public string CheckinStatus
        {
            get { return _CheckinStatus; }
            set
            {
                _CheckinStatus = value;
                NotifyOfPropertyChange(() => CheckinStatus);
            }
        }
        private ObservableCollection<Lookup> _V_ReasonType;
        public ObservableCollection<Lookup> V_ReasonType
        {
            get
            {
                return _V_ReasonType;
            }
            set
            {
                if (_V_ReasonType != value)
                {
                    _V_ReasonType = value;
                    NotifyOfPropertyChange(() => V_ReasonType);
                }
            }
        }
        private Lookup _SelectedReasonType;
        public Lookup SelectedReasonType
        {
            get
            {
                return _SelectedReasonType;
            }
            set
            {
                if (_SelectedReasonType != value)
                {
                    _SelectedReasonType = value;
                    NotifyOfPropertyChange(() => SelectedReasonType);
                }
            }
        }
        public string BusyMessage { get { return string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0995_G1_HThongDangXuLi); } }
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
        private bool _IsCheckIn;
        public bool IsCheckIn
        {
            get { return _IsCheckIn; }
            set
            {
                _IsCheckIn = value;
                NotifyOfPropertyChange(() => IsCheckIn);
                if (IsCheckIn)
                {
                    TitleForm = eHCMSResources.Z3288_G1_QLTraHS.ToUpper();
                    NotifyOfPropertyChange(() => TitleForm);
                }
                NotifyOfPropertyChange(() => SubTitleForm);
                NotifyOfPropertyChange(() => ContentButtonUpdate);
            }
        }
        private string _TitleForm = Globals.TitleForm;
        public string TitleForm
        {
            get { return _TitleForm; }
            set
            {
                _TitleForm = value;
                NotifyOfPropertyChange(() => TitleForm);
            }
        }
        public string SubTitleForm
        {
            get { return IsCheckIn ? eHCMSResources.Z3290_G1_TTTraHS : eHCMSResources.Z3289_G1_TTMuonHS; }
        }
        public string ContentButtonUpdate
        {
            get { return IsCheckIn ? "Nhập hồ sơ" : "Xuất hồ sơ"; }
        }
        TextBox txtFileCode;
        //private PatientMedicalFileStorageCheckInCheckOut _SelectedPatientMedicalFileStorageCheckOut;
        //public PatientMedicalFileStorageCheckInCheckOut SelectedPatientMedicalFileStorageCheckOut
        //{
        //    get
        //    {
        //        return _SelectedPatientMedicalFileStorageCheckOut;
        //    }
        //    set
        //    {
        //        _SelectedPatientMedicalFileStorageCheckOut = value;
        //        NotifyOfPropertyChange(() => SelectedPatientMedicalFileStorageCheckOut);
        //    }
        //}
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
        private bool _IsSetStaffPerson = false;
        public bool IsSetStaffPerson
        {
            get
            {
                return _IsSetStaffPerson;
            }
            set
            {
                _IsSetStaffPerson = value;
                NotifyOfPropertyChange(() => IsSetStaffPerson);
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
        private DateTime _CheckinDate = Globals.GetCurServerDateTime();
        public DateTime CheckinDate
        {
            get
            {
                return _CheckinDate;
            }
            set
            {
                _CheckinDate = value;
                NotifyOfPropertyChange(() => CheckinDate);
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
        private string _PatientCode;
        public string PatientCode
        {
            get
            {
                return _PatientCode;
            }
            set
            {
                if (_PatientCode != value)
                {
                    _PatientCode = value;
                    NotifyOfPropertyChange(() => PatientCode);
                }
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
        private ObservableCollection<Staff> _AllStaffByContext;
        public ObservableCollection<Staff> AllStaffByContext
        {
            get { return _AllStaffByContext; }
            set
            {
                _AllStaffByContext = value;
                NotifyOfPropertyChange(() => AllStaffByContext);
            }
        }
        AxAutoComplete cboContext { get; set; }
        AxAutoComplete cboContextBy { get; set; }
        private Staff _SelectedStaffOut;
        public Staff SelectedStaffOut
        {
            get { return _SelectedStaffOut; }
            set
            {
                if (_SelectedStaffOut != value)
                {
                    _SelectedStaffOut = value;
                    NotifyOfPropertyChange(() => SelectedStaffOut);
                }
            }
        }
        public void cboBorrow_Loaded(object sender, RoutedEventArgs e)
        {
            cboContext = (AxAutoComplete)sender;
        }

        public void cboBorrowBy_Loaded(object sender, RoutedEventArgs e)
        {
            cboContextBy = (AxAutoComplete)sender;
        }
        public void cboBorrowBy_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            e.Cancel = true;
            AllStaffByContext = new ObservableCollection<Staff>(AllStaff.Where(x => Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)));
            (sender as AxAutoComplete).ItemsSource = AllStaffByContext;
            (sender as AxAutoComplete).PopulateComplete();
        }
        public void cboBorrowBy_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedStaffOut = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public void cboBorrow_Populating(object sender, PopulatingEventArgs e)
        {
            var mSearchText = (sender as AxAutoComplete).SearchText;
            if (!string.IsNullOrEmpty(mSearchText))
            {
                mSearchText = mSearchText.ToLower();
                mSearchText = Globals.RemoveVietnameseString(mSearchText);
            }
            e.Cancel = true;
            AllStaffContext = new ObservableCollection<Staff>(AllStaff.Where(x => Globals.RemoveVietnameseString(x.FullName.ToLower()).Contains(mSearchText)));
            (sender as AxAutoComplete).ItemsSource = AllStaffContext;
            (sender as AxAutoComplete).PopulateComplete();
        }
        public void cboBorrow_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            SelectedStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }
        public string ExportByTitle
        {
            get { return IsCheckIn ? eHCMSResources.N0160_G1_NguoiNhan : eHCMSResources.Z2039_G1_NguoiXuat; }
        }
        public string ExportBy
        {
            get { return Globals.LoggedUserAccount.Staff.FullName; }
        }
        DataGrid gvMedicalFileCheckOut;
        #endregion
        #region Event
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefMedicalFileCheckOutViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        { 
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            LoadDepartments();
            LoadStores();
            V_ReasonType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_ReasonType).ToObservableCollection();
            SetDefaultV_ReasonType();
            LoadAllStaff();
            AllPatientMedicalFileStorageCheckOut.OnRefresh += AllPatientMedicalFileStorageCheckOut_OnRefresh;
        }
        //public void txtFileCode_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        IsBusy = true;
        //        FileCodeNumber = txtFileCode.Text;
        //        if (FileCodeNumber == "")
        //        {
        //            IsBusy = false;
        //            FocusFileCode();
        //            e.Handled = true;
        //            return;
        //        }
        //        if (AllPatientMedicalFileStorageCheckOut.Any(x => x.FileCodeNumber == FileCodeNumber))
        //        {
        //            IsBusy = false;
        //            MessageBox.Show(eHCMSResources.Z1984_G1_HSDaChon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //            FocusFileCode();
        //            e.Handled = true;
        //            return;
        //        }
        //        var t = new Thread(() =>
        //        {
        //            try
        //            {
        //                using (var serviceFactory = new ClinicManagementServiceClient())
        //                {
        //                    var contract = serviceFactory.ServiceInstance;
        //                    contract.BeginGetPatientMedicalFileStorageCheckInCheckOut(FileCodeNumber,
        //                    Globals.DispatchCallback((asyncResult) =>
        //                    {
        //                        SelectedPatientMedicalFileStorageCheckOut = AllPatientMedicalFileStorageCheckOut.Where(x => x.FileCodeNumber == FileCodeNumber).FirstOrDefault();
        //                        if (SelectedPatientMedicalFileStorageCheckOut == null)
        //                        {
        //                            var GettedItems = contract.EndGetPatientMedicalFileStorageCheckInCheckOut(asyncResult);
        //                            if (IsCheckIn)
        //                                SelectedPatientMedicalFileStorageCheckOut = GettedItems.Where(x => x.FileCodeNumber == FileCodeNumber && x.CheckinDate == null).FirstOrDefault();
        //                            if (SelectedPatientMedicalFileStorageCheckOut == null)
        //                                SelectedPatientMedicalFileStorageCheckOut = GettedItems.FirstOrDefault();
        //                            if (SelectedPatientMedicalFileStorageCheckOut == null)
        //                            {
        //                                IsBusy = false;
        //                                MessageBox.Show(eHCMSResources.Z1951_G1_KgTimThayKeChuaHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                                FocusFileCode();
        //                                return;
        //                            }
        //                            if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore)
        //                            {
        //                                if (AllPatientMedicalFileStorageCheckOut.Count == 0)
        //                                    SelectedPatientMedicalFileStorageCheckOut.PatientMedicalFileCheckoutID = -1;
        //                                else
        //                                    SelectedPatientMedicalFileStorageCheckOut.PatientMedicalFileCheckoutID = AllPatientMedicalFileStorageCheckOut.Min(x => x.PatientMedicalFileCheckoutID) > 0 ? -1 : AllPatientMedicalFileStorageCheckOut.Min(x => x.PatientMedicalFileCheckoutID) - 1;
        //                            }
        //                        }
        //                        if (SelectedPatientMedicalFileStorageCheckOut == null)
        //                        {
        //                            IsBusy = false;
        //                            MessageBox.Show(eHCMSResources.Z1952_G1_ChonTTinNgMuon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                            FocusFileCode();
        //                            return;
        //                        }
        //                        if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore && IsCheckIn)
        //                        {
        //                            MessageBox.Show(eHCMSResources.Z1991_G1_HSoDaNamTrongKe, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                            IsBusy = false;
        //                            FocusFileCode();
        //                            return;
        //                        }
        //                        if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.OutStore && !IsCheckIn)
        //                        {
        //                            MessageBox.Show(eHCMSResources.Z1983_G1_HSDaMuon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                            IsBusy = false;
        //                            FocusFileCode();
        //                            return;
        //                        }
        //                        if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore)
        //                        {
        //                            if (SelectedStaff == null && SelectedDepartment.DeptID == 0 && SelectedLocation.DeptLocationID == 0)
        //                            {
        //                                MessageBox.Show(eHCMSResources.Z1985_ChonDoiTuongMuonHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //                                IsBusy = false;
        //                                FocusFileCode();
        //                                return;
        //                            }
        //                            if (SelectedStaff != null)
        //                            {
        //                                SelectedPatientMedicalFileStorageCheckOut.StaffPersonID = SelectedStaff.StaffID;
        //                                SelectedPatientMedicalFileStorageCheckOut.FullName = SelectedStaff.FullName;
        //                            }
        //                            else
        //                            {
        //                                SelectedPatientMedicalFileStorageCheckOut.StaffPersonID = 0;
        //                                SelectedPatientMedicalFileStorageCheckOut.FullName = null;
        //                            }
        //                            if (SelectedDepartment.DeptID > 0)
        //                            {
        //                                SelectedPatientMedicalFileStorageCheckOut.DeptID = SelectedDepartment.DeptID;
        //                                SelectedPatientMedicalFileStorageCheckOut.DeptName = SelectedDepartment.DeptName;
        //                            }
        //                            else
        //                            {
        //                                SelectedPatientMedicalFileStorageCheckOut.DeptID = 0;
        //                                SelectedPatientMedicalFileStorageCheckOut.DeptName = null;
        //                            }
        //                            if (SelectedLocation.DeptLocationID > 0)
        //                            {
        //                                SelectedPatientMedicalFileStorageCheckOut.DeptLocID = SelectedLocation.DeptLocationID;
        //                                SelectedPatientMedicalFileStorageCheckOut.LocationName = SelectedLocation.Location.LocationName;
        //                            }
        //                            else
        //                            {
        //                                SelectedPatientMedicalFileStorageCheckOut.DeptLocID = 0;
        //                                SelectedPatientMedicalFileStorageCheckOut.LocationName = null;
        //                            }
        //                            if (!AllPatientMedicalFileStorageCheckOut.Any(x => x.FileCodeNumber == FileCodeNumber))
        //                                AllPatientMedicalFileStorageCheckOut.Add(SelectedPatientMedicalFileStorageCheckOut);
        //                        }
        //                        else if (!AllPatientMedicalFileStorageCheckOut.Any(x => x.FileCodeNumber == FileCodeNumber))
        //                            AllPatientMedicalFileStorageCheckOut.Add(SelectedPatientMedicalFileStorageCheckOut);
        //                        var AddedItem = AllPatientMedicalFileStorageCheckOut.LastOrDefault();
        //                        SelectedFileStorage = AddedItem;
        //                        gvMedicalFileCheckOut.ScrollIntoView(SelectedFileStorage, gvMedicalFileCheckOut.Columns[0]);
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
        //    try
        //    {
        //        txtFileCode = sender as TextBox;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
        //    }
        //}
        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedFileStorage != null)
                {
                    if (MessageBox.Show(string.Format(eHCMSResources.Z1953_G1_CoMuonXoaHS, SelectedFileStorage.FileCodeNumber), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        AllPatientMedicalFileStorageCheckOut.Remove(SelectedFileStorage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
            }
        }
        public void btnUpdate()
        {
            this.ShowBusyIndicator();
            if (!checkValidateUpdate())
            {
                this.HideBusyIndicator();
                return;
            }
            var CheckedFileStorages = AllPatientMedicalFileStorageCheckOut.Where(x => x.IsSelected == true
                && (IsCheckIn || (!IsCheckIn && x.FileStorageStatus != FileStorageStatus.OutStore))).ToList();
            if (CheckedFileStorages == null || CheckedFileStorages.Count == 0)
            {
                MessageBox.Show("Chọn hồ sơ cần thực hiện!", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                this.HideBusyIndicator();
                return;
            }
            var CheckoutDate = Globals.GetCurServerDateTime();
            CheckedFileStorages.ForEach(x => {
                x.PatientMedicalFileCheckoutID = IsCheckIn ? x.PatientMedicalFileCheckoutID : -1;
                x.DeptID = SelectedDepartment.DeptID;
                x.DeptLocID = SelectedLocation.DeptLocationID;
                x.CheckinDate = IsCheckIn ? CheckinDate : (DateTime?)null;
            });
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ClinicManagementServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdatePatientMedicalFileStorageCheckOut_V2(Globals.LoggedUserAccount.Staff.StaffID, SelectedStaffOut.StaffID, SelectedStaff.StaffID,
                            BorrowingDay, IsCheckIn ? CheckinStatus : Notes, SelectedReasonType.LookupID, IsCheckIn, CheckedFileStorages,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            long MedicalFileStorageCheckID = 0;
                            if (!contract.EndUpdatePatientMedicalFileStorageCheckOut_V2(out MedicalFileStorageCheckID, asyncResult))
                                MessageBox.Show(eHCMSResources.Z1954_G1_CheckInCheckOutFail, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            else
                            {
                                PrintConfirm(CheckedFileStorages.ToList(), MedicalFileStorageCheckID);
                                MessageBox.Show(eHCMSResources.Z1955_G1_CheckInCheckOutSuccess, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                AllPatientMedicalFileStorageCheckOut = new PagedSortableCollectionView<PatientMedicalFileStorageCheckInCheckOut>();
                                btnSearchMedicalFileCheckOut(0);
                                SetDefaultRefDept();
                                SetDefaultV_ReasonType();
                                SelectedStaff = null;
                                SelectedStaffOut = null;
                                cboContext.Text = "";
                                cboContextBy.Text = "";
                                Notes = string.Empty;
                                CheckinStatus = string.Empty;
                                CheckinDate = Globals.GetCurServerDateTime();
                                BorrowingDay = 0;
                            }
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
        public void btnLoadFrReg()
        { 
            GlobalsNAV.ShowDialog<IGetMedicalFileFromRegistration>();
        }
        private void PrintConfirm(List<PatientMedicalFileStorageCheckInCheckOut> CheckedFiles, long? MedicalFileStorageCheckID = null)
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

                                if (IsCheckIn)
                                {
                                    Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                    {
                                        mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                        mPrintView.FileCodeNumber = RegistrationXML;
                                        //mPrintView.ReceiveName = Globals.LoggedUserAccount.Staff.FullName;
                                        mPrintView.ReceiveName = SelectedStaffOut.FullName;
                                        mPrintView.MedicalFileStorageCheckID = MedicalFileStorageCheckID;
                                    };
                                    GlobalsNAV.ShowDialog(onInitDlg);
                                }
                                else
                                {
                                    Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                    {
                                        mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                        mPrintView.FileCodeNumber = RegistrationXML;
                                        //mPrintView.IssueName = Globals.LoggedUserAccount.Staff.FullName;
                                        mPrintView.IssueName = SelectedStaffOut.FullName;
                                        mPrintView.ReceiveName = SelectedStaff == null ? null : SelectedStaff.FullName;
                                        //mPrintView.AdmissionDate = DateTime.Now.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
                                        mPrintView.AdmissionDate = DateTime.Now.AddDays(BorrowingDay);
                                        mPrintView.MedicalFileStorageCheckID = MedicalFileStorageCheckID;
                                    };
                                    GlobalsNAV.ShowDialog(onInitDlg);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                                //FocusFileCode();
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
        public void gvMedicalFileCheckOut_Loaded(object sender, RoutedEventArgs e)
        {
            gvMedicalFileCheckOut = sender as DataGrid;
        }
        #endregion
        #region Method
        private void FocusFileCode()
        {
            txtFileCode.Text = "";
            txtFileCode.Focus();
        }
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
                    //if (gDept.V_DeptTypeOperation.Value == (long)V_DeptTypeOperation.KhoaNoi)
                    //{
                    //    loadingDepts.Add(gDept);
                    //}
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
        public void grd_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
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
                                    if (AllStores != null && AllStores.Count > 0)
                                    {
                                        SelectedStore = AllStores.FirstOrDefault();
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
            if (SelectedStore == null || SelectedStore.StoreID <= 0)
            {
                AllRefRows = new ObservableCollection<RefRows>();
                RefRows firstItem = new RefRows();
                firstItem.RefRowID = 0;
                firstItem.RefRowName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3283_G1_ChonDay);
                AllRefRows.Insert(0, firstItem);
                SelectedRow = AllRefRows.FirstOrDefault();
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
                                    RefRows firstItem = new RefRows();
                                    firstItem.RefRowID = 0;
                                    firstItem.RefRowName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3283_G1_ChonDay);
                                    AllRefRows.Insert(0, firstItem);
                                    SelectedRow = AllRefRows.FirstOrDefault();
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
                    MessageBox.Show(ex.Message);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void LoadShelf()
        {
            if (SelectedRow == null || SelectedRow.RefRowID <= 0)
            {
                AllRefShelfs = new ObservableCollection<RefShelves>();
                RefShelves firstItem = new RefShelves();
                firstItem.RefShelfID = 0;
                firstItem.RefShelfName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3284_G1_ChonKe);
                AllRefShelfs.Insert(0, firstItem);
                SelectedRefShelf = AllRefShelfs.FirstOrDefault();
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
                        contract.BeginGetRefShelves(0, SelectedRow.RefRowID, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelfs = contract.EndGetRefShelves(asyncResult);
                                RefShelves firstItem = new RefShelves();
                                firstItem.RefShelfID = 0;
                                firstItem.RefShelfName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3284_G1_ChonKe);
                                AllRefShelfs.Insert(0, firstItem);
                                SelectedRefShelf = AllRefShelfs.FirstOrDefault();
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
        private void LoadShelfDetails()
        {
            if (SelectedRefShelf == null || SelectedRefShelf.RefShelfID <= 0)
            {
                AllRefShelfDetails = new ObservableCollection<RefShelfDetails>();
                RefShelfDetails firstItem = new RefShelfDetails();
                firstItem.RefShelfDetailID = 0;
                firstItem.LocName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3285_G1_ChonNgan);
                AllRefShelfDetails.Insert(0, firstItem);
                SelectedRefShelfDetail = AllRefShelfDetails.FirstOrDefault();
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
                        contract.BeginGetRefShelfDetails(SelectedRefShelf.RefShelfID, null, 0, null,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                AllRefShelfDetails = contract.EndGetRefShelfDetails(asyncResult);
                                RefShelfDetails firstItem = new RefShelfDetails();
                                firstItem.RefShelfDetailID = 0;
                                firstItem.LocName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z3285_G1_ChonNgan);
                                AllRefShelfDetails.Insert(0, firstItem);
                                SelectedRefShelfDetail = AllRefShelfDetails.FirstOrDefault();
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        public void btnSearchMedicalFileCheckOut(int PageIndex)
        {
            this.ShowBusyIndicator();
            if (SelectedStore == null || SelectedStore.StoreID < 0)
            {
                MessageBox.Show(eHCMSResources.Z0941_G1_ChonKhoCanXem);
                this.HideBusyIndicator();
                return;
            }
            if (StartDate == null || EndDate == null)
            {
                MessageBox.Show("Chọn từ ngày đến ngày!");
                this.HideBusyIndicator();
                return;
            }
            if (EndDate < StartDate)
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
                        contract.BeginGetPatientMedicalFileStorageCheckInCheckOut_V2(SelectedStore.StoreID, SelectedRow.RefRowID, SelectedRefShelf.RefShelfID, 
                            SelectedRefShelfDetail.RefShelfDetailID, StartDate, EndDate, PatientCode, PatientName, FileCodeNumber, IsCheckIn, PageSize, PageIndex,
                        Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int TotalRecord = 0;
                                var GettedItems = contract.EndGetPatientMedicalFileStorageCheckInCheckOut_V2(out TotalRecord, asyncResult);
                                AllPatientMedicalFileStorageCheckOut.Clear();
                                foreach (var item in GettedItems)
                                {
                                    AllPatientMedicalFileStorageCheckOut.Add(item);
                                }
                                AllPatientMedicalFileStorageCheckOut.PageSize = PageSize;
                                AllPatientMedicalFileStorageCheckOut.PageIndex = PageIndex;
                                AllPatientMedicalFileStorageCheckOut.TotalItemCount = TotalRecord;
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
        private void SetDefaultV_ReasonType()
        {
            if (V_ReasonType != null)
            {
                SelectedReasonType = V_ReasonType.FirstOrDefault();
            }
        }

        private bool checkValidateUpdate()
        {
            if(SelectedDepartment == null || SelectedDepartment.DeptID <= 0)
            {
                MessageBox.Show(IsCheckIn ? "Chưa chọn khoa trả!" : "Chưa chọn khoa mượn!");
                return false;
            }
            if (SelectedLocation == null || SelectedLocation.DeptLocationID <= 0)
            {
                MessageBox.Show(IsCheckIn ? "Chưa chọn phòng trả!" : "Chưa chọn phòng mượn!");
                return false;
            }
            if (SelectedStaff == null || SelectedStaff.StaffID <= 0)
            {
                MessageBox.Show(IsCheckIn ? "Chưa chọn người trả!" : "Chưa chọn người mượn!");
                return false;
            }
            if (SelectedStaffOut == null || SelectedStaffOut.StaffID <= 0)
            {
                MessageBox.Show("Chưa chọn người cho mượn!");
                return false;
            }
            if (SelectedStaff.StaffID == SelectedStaffOut.StaffID)
            {
                MessageBox.Show(IsCheckIn ? "Người trả và người nhận không được trùng nhau" : "Người mượn và người cho mượn không được trùng nhau");
                return false;
            }
            if (BorrowingDay == 0 && !IsCheckIn)
            {
                MessageBox.Show("Nhập số ngày hoàn trả!");
                return false;
            }
            if (SelectedReasonType == null || SelectedReasonType.LookupID <= 0)
            {
                MessageBox.Show("Chưa chọn lý do mượn!");
                return false;
            }
            return true;
        }
        private void LoadAllStaff()
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
                            Staff firstItem = new Staff();
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
        public void btnRefresh()
        {
            LoadStores();
            SetDefaultV_ReasonType();
            SetDefaultRefDept();
            SelectedStaff = null;
            SelectedStaffOut = null;
            PatientName = string.Empty;
            PatientCode = string.Empty;
            FileCodeNumber = string.Empty;
            Notes = string.Empty;
            BorrowingDay = 0;
            StartDate = Globals.GetCurServerDateTime();
            EndDate = Globals.GetCurServerDateTime();
            CheckinDate = Globals.GetCurServerDateTime();
            cboContext.Text = "";
            cboContextBy.Text = "";
            btnSearchMedicalFileCheckOut(0);
        }
        public AxTextBoxFilter.TextBoxFilterType IntNumberFilter
        {
            get
            {
                return AxTextBoxFilter.TextBoxFilterType.Integer;
            }
        }
        private void AllPatientMedicalFileStorageCheckOut_OnRefresh(object sender, RefreshEventArgs e)
        {
            btnSearchMedicalFileCheckOut(AllPatientMedicalFileStorageCheckOut.PageIndex);
        }
        public int PageSize { get { return 20; } }
        #endregion
    }
}
