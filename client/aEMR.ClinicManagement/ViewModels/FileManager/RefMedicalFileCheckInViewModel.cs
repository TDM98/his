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

namespace aEMR.ClinicManagement.ViewModels
{
    [Export(typeof(IRefMedicalFileCheckIn)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RefMedicalFileCheckInViewModel : Conductor<object>, IRefMedicalFileCheckIn
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
                    TitleForm = eHCMSResources.Z1994_G1_NhapHSo.ToUpper();
                    NotifyOfPropertyChange(() => TitleForm);
                }
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
        TextBox txtFileCode;
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
        private ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> _AllPatientMedicalFileStorageCheckOut = new ObservableCollection<PatientMedicalFileStorageCheckInCheckOut>();
        public ObservableCollection<PatientMedicalFileStorageCheckInCheckOut> AllPatientMedicalFileStorageCheckOut
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
        public string ExportByTitle
        {
            get { return IsCheckIn ? eHCMSResources.N0160_G1_NguoiNhan : eHCMSResources.Z2039_G1_NguoiXuat; }
        }
        public string ExportBy
        {
            get { return Globals.LoggedUserAccount.Staff.FullName; }
        }
        DataGrid gvMedicalFileCheckIn;
        #endregion
        #region Event
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public RefMedicalFileCheckInViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        { 
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            LoadDepartments();

            IsBusy = true;
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
        public void txtFileCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsBusy = true;
                FileCodeNumber = txtFileCode.Text;
                if (FileCodeNumber == "")
                {
                    IsBusy = false;
                    FocusFileCode();
                    e.Handled = true;
                    return;
                }
                if (AllPatientMedicalFileStorageCheckOut.Any(x => x.FileCodeNumber == FileCodeNumber))
                {
                    IsBusy = false;
                    MessageBox.Show(eHCMSResources.Z1984_G1_HSDaChon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                    FocusFileCode();
                    e.Handled = true;
                    return;
                }
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ClinicManagementServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginGetPatientMedicalFileStorageCheckInCheckOut(FileCodeNumber,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                SelectedPatientMedicalFileStorageCheckOut = AllPatientMedicalFileStorageCheckOut.Where(x => x.FileCodeNumber == FileCodeNumber).FirstOrDefault();
                                if (SelectedPatientMedicalFileStorageCheckOut == null)
                                {
                                    var GettedItems = contract.EndGetPatientMedicalFileStorageCheckInCheckOut(asyncResult);
                                    if (IsCheckIn)
                                        SelectedPatientMedicalFileStorageCheckOut = GettedItems.Where(x => x.FileCodeNumber == FileCodeNumber && x.CheckinDate == null).FirstOrDefault();
                                    if (SelectedPatientMedicalFileStorageCheckOut == null)
                                        SelectedPatientMedicalFileStorageCheckOut = GettedItems.FirstOrDefault();
                                    if (SelectedPatientMedicalFileStorageCheckOut == null)
                                    {
                                        IsBusy = false;
                                        MessageBox.Show(eHCMSResources.Z1951_G1_KgTimThayKeChuaHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                        FocusFileCode();
                                        return;
                                    }
                                    if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore)
                                    {
                                        if (AllPatientMedicalFileStorageCheckOut.Count == 0)
                                            SelectedPatientMedicalFileStorageCheckOut.PatientMedicalFileCheckoutID = -1;
                                        else
                                            SelectedPatientMedicalFileStorageCheckOut.PatientMedicalFileCheckoutID = AllPatientMedicalFileStorageCheckOut.Min(x => x.PatientMedicalFileCheckoutID) > 0 ? -1 : AllPatientMedicalFileStorageCheckOut.Min(x => x.PatientMedicalFileCheckoutID) - 1;
                                    }
                                }
                                if (SelectedPatientMedicalFileStorageCheckOut == null)
                                {
                                    IsBusy = false;
                                    MessageBox.Show(eHCMSResources.Z1952_G1_ChonTTinNgMuon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    FocusFileCode();
                                    return;
                                }
                                if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore && IsCheckIn)
                                {
                                    MessageBox.Show(eHCMSResources.Z1991_G1_HSoDaNamTrongKe, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    IsBusy = false;
                                    FocusFileCode();
                                    return;
                                }
                                if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.OutStore && !IsCheckIn)
                                {
                                    MessageBox.Show(eHCMSResources.Z1983_G1_HSDaMuon, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                    IsBusy = false;
                                    FocusFileCode();
                                    return;
                                }
                                if (SelectedPatientMedicalFileStorageCheckOut.FileStorageStatus == FileStorageStatus.InStore)
                                {
                                    if (SelectedStaff == null && SelectedDepartment.DeptID == 0 && SelectedLocation.DeptLocationID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.Z1985_ChonDoiTuongMuonHS, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                        IsBusy = false;
                                        FocusFileCode();
                                        return;
                                    }
                                    if (SelectedStaff != null)
                                    {
                                        SelectedPatientMedicalFileStorageCheckOut.StaffPersonID = SelectedStaff.StaffID;
                                        SelectedPatientMedicalFileStorageCheckOut.FullName = SelectedStaff.FullName;
                                    }
                                    else
                                    {
                                        SelectedPatientMedicalFileStorageCheckOut.StaffPersonID = 0;
                                        SelectedPatientMedicalFileStorageCheckOut.FullName = null;
                                    }
                                    if (SelectedDepartment.DeptID > 0)
                                    {
                                        SelectedPatientMedicalFileStorageCheckOut.DeptID = SelectedDepartment.DeptID;
                                        SelectedPatientMedicalFileStorageCheckOut.DeptName = SelectedDepartment.DeptName;
                                    }
                                    else
                                    {
                                        SelectedPatientMedicalFileStorageCheckOut.DeptID = 0;
                                        SelectedPatientMedicalFileStorageCheckOut.DeptName = null;
                                    }
                                    if (SelectedLocation.DeptLocationID > 0)
                                    {
                                        SelectedPatientMedicalFileStorageCheckOut.DeptLocID = SelectedLocation.DeptLocationID;
                                        SelectedPatientMedicalFileStorageCheckOut.LocationName = SelectedLocation.Location.LocationName;
                                    }
                                    else
                                    {
                                        SelectedPatientMedicalFileStorageCheckOut.DeptLocID = 0;
                                        SelectedPatientMedicalFileStorageCheckOut.LocationName = null;
                                    }
                                    if (!AllPatientMedicalFileStorageCheckOut.Any(x => x.FileCodeNumber == FileCodeNumber))
                                        AllPatientMedicalFileStorageCheckOut.Add(SelectedPatientMedicalFileStorageCheckOut);
                                }
                                else if (!AllPatientMedicalFileStorageCheckOut.Any(x => x.FileCodeNumber == FileCodeNumber))
                                    AllPatientMedicalFileStorageCheckOut.Add(SelectedPatientMedicalFileStorageCheckOut);
                                var AddedItem = AllPatientMedicalFileStorageCheckOut.LastOrDefault();
                                SelectedFileStorage = AddedItem;
                                gvMedicalFileCheckIn.ScrollIntoView(SelectedFileStorage, gvMedicalFileCheckIn.Columns[0]);
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
            IsBusy = true;
            if (AllPatientMedicalFileStorageCheckOut == null || AllPatientMedicalFileStorageCheckOut.Count == 0)
            {
                MessageBox.Show(eHCMSResources.Z1986_ThemHSCanThucHien, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                IsBusy = false;
                FocusFileCode();
            }
            else
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new ClinicManagementServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginUpdatePatientMedicalFileStorageCheckOut(Globals.LoggedUserAccount.Staff.StaffID, AllPatientMedicalFileStorageCheckOut.ToList(),
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                long MedicalFileStorageCheckID = 0;
                                if (!contract.EndUpdatePatientMedicalFileStorageCheckOut(out MedicalFileStorageCheckID, asyncResult))
                                    MessageBox.Show(eHCMSResources.Z1954_G1_CheckInCheckOutFail, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                                else
                                {
                                    PrintConfirm(AllPatientMedicalFileStorageCheckOut.ToList(), MedicalFileStorageCheckID);
                                    MessageBox.Show(eHCMSResources.Z1955_G1_CheckInCheckOutSuccess, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    AllPatientMedicalFileStorageCheckOut = new ObservableCollection<PatientMedicalFileStorageCheckInCheckOut>();
                                }
                                SelectedPatientMedicalFileStorageCheckOut = null;

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
        }
        public void btnLoadFrReg()
        { 
        //    var vm = Globals.GetViewModel<IGetMedicalFileFromRegistration>();
        //    Globals.ShowDialog(vm as Conductor<object>);

            GlobalsNAV.ShowDialog<IGetMedicalFileFromRegistration>();
        }
        private void PrintConfirm(List<PatientMedicalFileStorageCheckInCheckOut> CheckedFiles, long? MedicalFileStorageCheckID = null)
        {
            IsBusy = true;
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
                                    //var mPrintView = Globals.GetViewModel<ICommonPreviewView>();
                                    //mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                    //mPrintView.FileCodeNumber = RegistrationXML;
                                    //mPrintView.ReceiveName = Globals.LoggedUserAccount.Staff.FullName;
                                    //var mInstance = mPrintView as Conductor<object>;
                                    //Globals.ShowDialog(mInstance, (o) => { });

                                    Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                    {
                                        mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                        mPrintView.FileCodeNumber = RegistrationXML;
                                        mPrintView.ReceiveName = Globals.LoggedUserAccount.Staff.FullName;
                                    };
                                    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                                }
                                else
                                {
                                    //var mPrintView = Globals.GetViewModel<ICommonPreviewView>();
                                    //mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                    //mPrintView.FileCodeNumber = RegistrationXML;
                                    //mPrintView.IssueName = Globals.LoggedUserAccount.Staff.FullName;
                                    //mPrintView.ReceiveName = SelectedStaff == null ? null : SelectedStaff.FullName;
                                    //mPrintView.AdmissionDate = DateTime.Now.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
                                    //mPrintView.MedicalFileStorageCheckID = MedicalFileStorageCheckID;
                                    //var mInstance = mPrintView as Conductor<object>;
                                    //Globals.ShowDialog(mInstance, (o) => { });


                                    Action<ICommonPreviewView> onInitDlg = (mPrintView) =>
                                    {
                                        mPrintView.eItem = ReportName.MEDICALFILECHECKOUTCONFIRM;
                                        mPrintView.FileCodeNumber = RegistrationXML;
                                        mPrintView.IssueName = Globals.LoggedUserAccount.Staff.FullName;
                                        mPrintView.ReceiveName = SelectedStaff == null ? null : SelectedStaff.FullName;
                                        mPrintView.AdmissionDate = DateTime.Now.AddHours(Globals.ServerConfigSection.CommonItems.BorrowTimeLimit);
                                        mPrintView.MedicalFileStorageCheckID = MedicalFileStorageCheckID;
                                    };
                                    GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                IsBusy = false;
                                FocusFileCode();
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
        public void gvMedicalFileCheckIn_Loaded(object sender, RoutedEventArgs e)
        {
            gvMedicalFileCheckIn = sender as DataGrid;
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
            IsBusy = true;
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
                                IsBusy = false;
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
                IsBusy = false;
            }
        }
        #endregion
    }
}
