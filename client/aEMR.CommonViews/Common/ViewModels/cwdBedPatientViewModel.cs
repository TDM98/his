using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using System.ComponentModel;
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using System.ServiceModel;
using aEMR.DataContracts;
using System.Collections.Generic;
using aEMR.Controls;
using System.Linq;
/*
 * 20211004 #001 TNHX: Lọc danh sách bsi theo cấu hình trách nhiệm + theo tích tạm khóa
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IcwdBedPatientCommon))]
    public class cwdBedPatientViewModel : ViewModelBase, IcwdBedPatientCommon
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public cwdBedPatientViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            _PatientInfo = new Patient();
            DepartmentContent = Globals.GetViewModel<IDepartmentListing>();
            DepartmentContent.AddSelectOneItem = true;
            bool designTime = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                DepartmentContent.LoadData();
            }
            CheckInDateTime = Globals.GetViewModel<IMinHourDateControl>();
            CheckInDateTime.DateTime = Globals.GetCurServerDateTime();
            CheckOutDateTime = Globals.GetViewModel<IMinHourDateControl>();
            CheckOutDateTime.DateTime = Globals.GetCurServerDateTime();
            IsShowCheckOutDateTime = Globals.ServerConfigSection.CommonItems.AllowReSelectRoomWhenLeaveDept && IsReSelectBed;
            _ObjGetDeptMedServiceItems = new ObservableCollection<MedServiceItemPrice>();
            LoadDoctorStaffCollection();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        public void imBedLoaded(object sender, RoutedEventArgs e)
        {
            imBed = sender as Image;
            initForm();
        }

        //KMx: BookBedAllocOnly = true: Khi bấm đặt giường là lưu xuống Database luôn, ngược lại thì khi form cha lưu thì mới lưu giường sau (05/09/2014 17:11). 
        private bool _bookBedAllocOnly;
        public bool BookBedAllocOnly
        {
            get { return _bookBedAllocOnly; }
            set { _bookBedAllocOnly = value; }
        }

        //KMx: NONE: Đặt giường là lưu xuống Database luôn. Khác: Bắn event về cho parent, khi nào parent lưu thì mới lưu giường (06/09/2014 16:56).
        private eFireBookingBedEvent _eFireBookingBedEventTo;
        public eFireBookingBedEvent eFireBookingBedEventTo
        {
            get { return _eFireBookingBedEventTo; }
            set
            {
                if (_eFireBookingBedEventTo != value)
                {
                    _eFireBookingBedEventTo = value;
                    NotifyOfPropertyChange(() => eFireBookingBedEventTo);
                }
            }
        }

        private IDepartmentListing _departmentContent;
        public IDepartmentListing DepartmentContent
        {
            get { return _departmentContent; }
            set
            {
                _departmentContent = value;
                NotifyOfPropertyChange(() => DepartmentContent);
            }
        }
        public RefDepartment ResponsibleDepartment
        {
            get { return _departmentContent != null ? _departmentContent.SelectedItem : null; }
            set
            {
                if (_departmentContent != null)
                {
                    _departmentContent.SelectedItem = value;
                }
            }
        }

        private bool _IsReSelectBed = false;
        public bool IsReSelectBed
        {
            get
            {
                return _IsReSelectBed;
            }
            set
            {
                if (_IsReSelectBed == value)
                    return;
                _IsReSelectBed = value;
                IsShowCheckOutDateTime = value;
            }
        }

        private ObservableCollection<MedServiceItemPrice> _ObjGetDeptMedServiceItems;
        public ObservableCollection<MedServiceItemPrice> ObjGetDeptMedServiceItems
        {
            get
            {
                return _ObjGetDeptMedServiceItems;
            }
            set
            {
                if (_ObjGetDeptMedServiceItems == value)
                    return;
                _ObjGetDeptMedServiceItems = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems);
            }
        }

        private MedServiceItemPrice _SelectedGetDeptMedServiceItems;
        public MedServiceItemPrice SelectedGetDeptMedServiceItems
        {
            get
            {
                return _SelectedGetDeptMedServiceItems;
            }
            set
            {
                if (_SelectedGetDeptMedServiceItems == value)
                    return;
                _SelectedGetDeptMedServiceItems = value;
                NotifyOfPropertyChange(() => SelectedGetDeptMedServiceItems);
            }
        }

        private bool _isDelete;
        public bool isDelete
        {
            get
            {
                return _isDelete;
            }
            set
            {
                if (_isDelete == value)
                    return;
                _isDelete = value;
            }
        }

        private bool _isDeleteAll;
        public bool isDeleteAll
        {
            get
            {
                return _isDeleteAll;
            }
            set
            {
                if (_isDeleteAll == value)
                    return;
                _isDeleteAll = value;
            }
        }

        private ObservableCollection<BedPatientAllocs> _allBedPatientAllocs;
        public ObservableCollection<BedPatientAllocs> allBedPatientAllocs
        {
            get
            {
                return _allBedPatientAllocs;
            }
            set
            {
                if (_allBedPatientAllocs == value)
                    return;
                _allBedPatientAllocs = value;
                NotifyOfPropertyChange(() => allBedPatientAllocs);
                InitIndex();
            }
        }

        private BedPatientAllocs _selectedBedPatientAllocs;
        public BedPatientAllocs selectedBedPatientAllocs
        {
            get
            {
                return _selectedBedPatientAllocs;
            }
            set
            {
                if (_selectedBedPatientAllocs == value)
                    return;
                _selectedBedPatientAllocs = value;
                if (CheckInDateTime != null && _selectedBedPatientAllocs.CheckInDate != null)
                {
                    CheckInDateTime.DateTime = _selectedBedPatientAllocs.CheckInDate;
                }
                NotifyOfPropertyChange(() => selectedBedPatientAllocs);
            }
        }

        private bool _IsSaveEnable;
        private bool _IsDeleteEnable;

        public bool IsSaveEnable
        {
            get
            {
                return _IsSaveEnable;
            }
            set
            {
                if (_IsSaveEnable == value)
                    return;
                _IsSaveEnable = value;
                NotifyOfPropertyChange(() => IsSaveEnable);
            }
        }
        public bool IsDeleteEnable
        {
            get
            {
                return _IsDeleteEnable;
            }
            set
            {
                if (_IsDeleteEnable == value)
                    return;
                _IsDeleteEnable = value;
                NotifyOfPropertyChange(() => IsDeleteEnable);
            }
        }

        private string _txtFullNameOld;
        private string _txtPatientCodeOld;
        private string _txtFullNameNew;
        private string _txtPatientCodeNew;

        public string txtFullNameOld
        {
            get
            {
                return _txtFullNameOld;
            }
            set
            {
                if (_txtFullNameOld == value)
                    return;
                _txtFullNameOld = value;
                NotifyOfPropertyChange(() => txtFullNameOld);
            }
        }
        public string txtPatientCodeOld
        {
            get
            {
                return _txtPatientCodeOld;
            }
            set
            {
                if (_txtPatientCodeOld == value)
                    return;
                _txtPatientCodeOld = value;
                NotifyOfPropertyChange(() => txtPatientCodeOld);
            }
        }
        public string txtFullNameNew
        {
            get
            {
                return _txtFullNameNew;
            }
            set
            {
                if (_txtFullNameNew == value)
                    return;
                _txtFullNameNew = value;
                NotifyOfPropertyChange(() => txtFullNameNew);
            }
        }
        public string txtPatientCodeNew
        {
            get
            {
                return _txtPatientCodeNew;
            }
            set
            {
                if (_txtPatientCodeNew == value)
                    return;
                _txtPatientCodeNew = value;
                NotifyOfPropertyChange(() => txtPatientCodeNew);
            }
        }

        private BedPatientAllocs _selectedTempBedPatientAllocs;
        public BedPatientAllocs selectedTempBedPatientAllocs
        {
            get
            {
                return _selectedTempBedPatientAllocs;
            }
            set
            {
                if (_selectedTempBedPatientAllocs == value)
                    return;
                _selectedTempBedPatientAllocs = value;
                NotifyOfPropertyChange(() => selectedTempBedPatientAllocs);
            }
        }

        private Patient _PatientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _PatientInfo;
            }
            set
            {
                if (_PatientInfo == value)
                    return;
                _PatientInfo = value;
                NotifyOfPropertyChange(() => PatientInfo);
            }
        }

        private InPatientDeptDetail _InPtDeptDetail;
        public InPatientDeptDetail InPtDeptDetail
        {
            get { return _InPtDeptDetail; }
            set
            {
                if (_InPtDeptDetail != value)
                {
                    _InPtDeptDetail = value;
                    NotifyOfPropertyChange(() => InPtDeptDetail);
                }
            }
        }

        private IMinHourDateControl _CheckInDateTime;
        public IMinHourDateControl CheckInDateTime
        {
            get { return _CheckInDateTime; }
            set
            {
                _CheckInDateTime = value;
                NotifyOfPropertyChange(() => CheckInDateTime);
            }
        }

        private IMinHourDateControl _CheckOutDateTime;
        public IMinHourDateControl CheckOutDateTime
        {
            get { return _CheckOutDateTime; }
            set
            {
                _CheckOutDateTime = value;
                NotifyOfPropertyChange(() => CheckOutDateTime);
            }
        }

        private bool _IsShowCheckOutDateTime = false;
        public bool IsShowCheckOutDateTime
        {
            get { return _IsShowCheckOutDateTime; }
            set
            {
                _IsShowCheckOutDateTime = value;
                NotifyOfPropertyChange(() => IsShowCheckOutDateTime);
            }
        }

        private int index = 0;

        public void butDelete()
        {
            if (Globals.ObjRefDepartment != null && selectedBedPatientAllocs != null
                && selectedBedPatientAllocs.VBedAllocation != null && selectedBedPatientAllocs.VBedAllocation.VDeptLocation != null
                && Globals.ObjRefDepartment.DeptID == selectedBedPatientAllocs.VBedAllocation.VDeptLocation.DeptID)
            {
                if (MessageBox.Show(eHCMSResources.K0219_G1_TraGiuongChoBN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DeleteBedPatientAllocs(selectedBedPatientAllocs.BedPatientID);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0105_G1_Msg_InfoKhTheTraGiuong);
            }
        }

        public void InitIndex()
        {
            for (; index < allBedPatientAllocs.Count; index++)
            {
                if (allBedPatientAllocs[index].IsActive)
                {
                    selectedBedPatientAllocs = allBedPatientAllocs[index];
                    break;
                }
            }
        }

        public void butSave()
        {
            if (InPtDeptDetail == null)
            {
                MessageBox.Show(eHCMSResources.A0632_G1_Msg_InfoKhCoCTietNpKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                selectedBedPatientAllocs.InPatientDeptDetailID = InPtDeptDetail.InPatientDeptDetailID;
            }

            selectedBedPatientAllocs.CheckInDate = CheckInDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);
            selectedBedPatientAllocs.CheckOutDate = CheckOutDateTime.DateTime.GetValueOrDefault(DateTime.MinValue);

            if (selectedBedPatientAllocs.CheckInDate == null)
            {
                MessageBox.Show(eHCMSResources.A0592_G1_Msg_InfoChonNgNhanGiuong);
                return;
            }

            if (selectedBedPatientAllocs.CheckInDate.GetValueOrDefault().Date < InPtDeptDetail.FromDate.Date)
            {
                MessageBox.Show(string.Format("{0}. {1} {2}", eHCMSResources.A0833_G1_Msg_InfoNgDatGiuong, eHCMSResources.A0834_G1_Msg_NgNpKhoa, InPtDeptDetail.FromDate.ToString("dd/MM/yyyy hh:mm:ss tt"))
                    , eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //KMx: Không biết để làm gì (26/05/2015 09:32).
            //DateTime dateTime = (DateTime)selectedBedPatientAllocs.CheckInDate;

            //DateTime dT = new System.DateTime(dateTime.Year
            //                            , dateTime.Month
            //                            , dateTime.Day
            //                            ).AddDays(selectedBedPatientAllocs.ExpectedStayingDays);

            if (selectedTempBedPatientAllocs == null || selectedTempBedPatientAllocs.VPtRegistration == null
                || selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID < 1)
            {
                MessageBox.Show(eHCMSResources.Z0615_G1_BNChuaDK);
                return;
            }
            else
            {
                selectedBedPatientAllocs.PtRegistrationID = selectedTempBedPatientAllocs.VPtRegistration.PtRegistrationID;
            }

            if (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.A0531_G1_Msg_InfoChuaChonBSCDinh);
                return;
            }
            
            if (SelectedGetDeptMedServiceItems == null || SelectedGetDeptMedServiceItems.MedServiceID == 0)
            {
                MessageBox.Show("Bắt buộc chọn dịch vụ theo giường!");
                return;
            }
            selectedBedPatientAllocs.DoctorStaffID = gSelectedDoctorStaff.StaffID;
            selectedBedPatientAllocs.BAMedServiceID = SelectedGetDeptMedServiceItems.MedServiceID;
            switch (eFireBookingBedEventTo)
            {
                case eFireBookingBedEvent.NONE:
                    {
                        if (Globals.ServerConfigSection.CommonItems.AllowReSelectRoomWhenLeaveDept && IsReSelectBed)
                        {
                            AddNewBedPatientAllocs(selectedBedPatientAllocs.BedAllocationID
                                    , selectedBedPatientAllocs.PtRegistrationID
                                    , selectedBedPatientAllocs.ResponsibleDepartment
                                    , selectedBedPatientAllocs.CheckInDate
                                    , selectedBedPatientAllocs.ExpectedStayingDays
                                    , selectedBedPatientAllocs.CheckOutDate
                                    , true
                                    , selectedBedPatientAllocs.InPatientDeptDetailID);
                        }
                        else
                        {
                            AddNewBedPatientAllocs(selectedBedPatientAllocs.BedAllocationID
                                    , selectedBedPatientAllocs.PtRegistrationID
                                    , selectedBedPatientAllocs.ResponsibleDepartment
                                    , selectedBedPatientAllocs.CheckInDate
                                    , selectedBedPatientAllocs.ExpectedStayingDays
                                    , null
                                    , true
                                    , selectedBedPatientAllocs.InPatientDeptDetailID);
                        }
                        break;
                    }
                case eFireBookingBedEvent.AcceptChangeDeptView:
                    {
                        //KMx: Đóng view cha (07/09/2014 09:36).
                        Globals.EventAggregator.Publish(new CloseBedPatientAllocViewEvent());
                        Globals.EventAggregator.Publish(new BookingBedForAcceptChangeDeptEvent() { SelectedBedPatientAlloc = selectedBedPatientAllocs });
                        break;
                    }
            }

            try
            {
                TryClose();
            }
            catch (Exception ex)
            {
                ClientLoggerHelper.LogInfo(ex.ToString());
            }
        }

        public void BtnEdit()
        {
            if (InPtDeptDetail == null)
            {
                MessageBox.Show(eHCMSResources.A0632_G1_Msg_InfoKhCoCTietNpKhoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            else
            {
                selectedBedPatientAllocs.InPatientDeptDetailID = InPtDeptDetail.InPatientDeptDetailID;
            }
            if (selectedBedPatientAllocs.CheckInDate == null)
            {
                MessageBox.Show(eHCMSResources.A0592_G1_Msg_InfoChonNgNhanGiuong);
                return;
            }
            if (gSelectedDoctorStaff == null || gSelectedDoctorStaff.StaffID == 0)
            {
                MessageBox.Show(eHCMSResources.A0531_G1_Msg_InfoChuaChonBSCDinh);
                return;
            }
            selectedBedPatientAllocs.DoctorStaffID = gSelectedDoctorStaff.StaffID;
            if (SelectedGetDeptMedServiceItems != null)
            {
                selectedBedPatientAllocs.BAMedServiceID = SelectedGetDeptMedServiceItems.MedServiceID;
            }

            switch (eFireBookingBedEventTo)
            {
                case eFireBookingBedEvent.NONE:
                    {
                        EditBedPatientAllocs(selectedBedPatientAllocs);
                        break;
                    }
                default:
                    break;
            }
        }

        public void initForm()
        {
            Image image = new Image();
            Uri uri = null;
            if (isDelete == true)
            {
                IsSaveEnable = false;
                IsDeleteEnable = true;
                uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed4.png", UriKind.Relative);
                GetPatientInfo(selectedBedPatientAllocs.PtRegistrationID);
                txtFullNameNew = "";
                txtPatientCodeNew = "";
            }
            else
            if (selectedBedPatientAllocs.IsActive == false)
            {
                IsSaveEnable = true;
                IsDeleteEnable = false;
                uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed6.jpg", UriKind.Relative);
                if (selectedTempBedPatientAllocs.VPtRegistration.Patient != null)
                {
                    txtFullNameNew = selectedTempBedPatientAllocs.VPtRegistration.Patient.FullName;
                    txtPatientCodeNew = selectedTempBedPatientAllocs.VPtRegistration.Patient.PatientCode;
                }
                //txtFullNameNew = selectedTempBedPatientAllocs.VPtRegistration.FullName;
                //txtPatientCodeNew = selectedTempBedPatientAllocs.VPtRegistration.PatientCode; Z
                txtFullNameOld = "";
                txtPatientCodeOld = "";

                selectedBedPatientAllocs.CheckInDate = Globals.ServerDate.Value;
            }
            else
            {
                IsSaveEnable = true && !IsEdit;
                IsDeleteEnable = true;
                if (selectedBedPatientAllocs.BedPatientID > 0)
                {
                    txtFullNameNew = selectedBedPatientAllocs.VPtRegistration.Patient.FullName;
                    txtPatientCodeNew = selectedBedPatientAllocs.VPtRegistration.Patient.PatientCode;
                    gSelectedDoctorStaff = DoctorStaffs != null ? DoctorStaffs.FirstOrDefault(x => x.StaffID == selectedBedPatientAllocs.DoctorStaffID) : null;
                    uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed4.png", UriKind.Relative);
                }
                else
                {
                    if (selectedTempBedPatientAllocs.VPtRegistration.Patient != null)
                    {
                        txtFullNameNew = selectedTempBedPatientAllocs.VPtRegistration.Patient.FullName;
                        txtPatientCodeNew = selectedTempBedPatientAllocs.VPtRegistration.Patient.PatientCode;
                    }
                    uri = new Uri("/aEMR.CommonViews;component/Assets/Images/Bed4.png", UriKind.Relative);
                    GetPatientInfo(selectedBedPatientAllocs.PtRegistrationID);
                    selectedBedPatientAllocs.CheckInDate = Globals.ServerDate.Value;
                }                
            }

            ImageSource img = new BitmapImage(uri);
            ((Image)imBed).SetValue(Image.SourceProperty, img);
        }

        private object _imBed;
        public object imBed
        {
            get
            {
                return _imBed;
            }
            set
            {
                if (_imBed == value)
                    return;
                _imBed = value;
            }
        }
        public void butExit()
        {
            TryClose();
        }

        private BedPatientAllocs CreateBedPatientAlloc(long BedAllocationID
            , long PtRegistrationID
            , RefDepartment ResponsibleDept
            , DateTime? AdmissionDate
            , int ExpectedStayingDays
            , DateTime? DischargeDate
            , bool IsActive
            , long? InPatientDeptDetailID = null
            , long BAMedServiceID = 0)
        {
            BedPatientAllocs alloc = new BedPatientAllocs();
            alloc.BedAllocationID = BedAllocationID;
            alloc.PtRegistrationID = PtRegistrationID;
            alloc.ResponsibleDepartment = ResponsibleDept;
            alloc.CheckInDate = AdmissionDate;
            alloc.ExpectedStayingDays = ExpectedStayingDays;
            alloc.CheckOutDate = DischargeDate;
            alloc.IsActive = IsActive;
            alloc.InPatientDeptDetailID = InPatientDeptDetailID;
            alloc.BAMedServiceID = BAMedServiceID;
            alloc.DoctorStaffID = gSelectedDoctorStaff.StaffID;
            return alloc;
        }

        private void AddNewBedPatientAllocs(long BedAllocationID
            , long PtRegistrationID
            , RefDepartment ResponsibleDept
            , DateTime? AdmissionDate
            , int ExpectedStayingDays
            , DateTime? DischargeDate
            , bool IsActive
            , long? InPatientDeptDetailID = null)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BedAllocationsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        long BAMedServiceID = 0;
                        if (SelectedGetDeptMedServiceItems != null)
                        {
                            BAMedServiceID = SelectedGetDeptMedServiceItems.MedServiceID;
                        }
                        var alloc = CreateBedPatientAlloc(BedAllocationID
                                                      , PtRegistrationID
                                                      , ResponsibleDept
                                                      , AdmissionDate
                                                      , ExpectedStayingDays
                                                      , DischargeDate
                                                      , IsActive
                                                      , InPatientDeptDetailID
                                                      , BAMedServiceID);
                        contract.BeginAddNewBedPatientAllocs(alloc, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndAddNewBedPatientAllocs(asyncResult);
                                if (results == 1)
                                {
                                    Globals.EventAggregator.Publish(new BedAllocEvent { });
                                    MessageBox.Show(eHCMSResources.A0507_G1_Msg_InfoDatGiuongOK);
                                    Globals.EventAggregator.Publish(new AddCompleted<BedPatientAllocs>() { Item = asyncResult.AsyncState as BedPatientAllocs });
                                }
                                else if (results > 1)
                                {
                                    MessageBox.Show(eHCMSResources.A0562_G1_Msg_InfoGiuongDuBN);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0506_G1_Msg_InfoDatGiuongFail);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), alloc);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void EditBedPatientAllocs(BedPatientAllocs UpdateBedPatientAllocs)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BedAllocationsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginEditBedPatientAllocs(UpdateBedPatientAllocs
                            , (long)Globals.LoggedUserAccount.StaffID
                            , Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndEditBedPatientAllocs(asyncResult);
                                if (results)
                                {
                                    MessageBox.Show(eHCMSResources.K2782_G1_DaCNhat);
                                    Globals.EventAggregator.Publish(new AddCompleted<BedPatientAllocs>() { Item = asyncResult.AsyncState as BedPatientAllocs });
                                    TryClose();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0272_G1_Msg_InfoCNhatFail);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        private void DeleteBedPatientAllocs(long BedPatientID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BedAllocationsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteBedPatientAllocs(BedPatientID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDeleteBedPatientAllocs(asyncResult);
                                if (results == true)
                                {
                                    Globals.EventAggregator.Publish(new BedAllocEvent { });
                                    MessageBox.Show(eHCMSResources.K0220_G1_TraGiuongOk);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                                if (!isDeleteAll)
                                {
                                    TryClose();
                                }
                                else
                                {
                                    index++;
                                    InitIndex();
                                    if (index >= allBedPatientAllocs.Count)
                                    {
                                        Globals.EventAggregator.Publish(new BedPatientDischarge { BedPatientDischargeItem = selectedBedPatientAllocs });
                                        TryClose();
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private int FindPatient = (int)AllLookupValues.V_FindPatientType.NOI_TRU;
        private void GetPatientInfo(long RegistrationID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetPatientInfoByPtRegistration(RegistrationID, null, FindPatient, Globals.DispatchCallback((asyncResult) =>
                          {
                              try
                              {
                                  PatientInfo = contract.EndGetPatientInfoByPtRegistration(asyncResult);
                                  if (PatientInfo != null)
                                  {
                                      txtFullNameOld = PatientInfo.FullName;
                                      txtPatientCodeOld = PatientInfo.PatientCode;
                                  }
                              }
                              catch (Exception ex)
                              {
                                  MessageBox.Show(ex.Message);
                              }
                              finally
                              {
                                  this.DlgHideBusyIndicator();
                              }
                          }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void GetBedAllocationAll_ByDeptID(DeptMedServiceItemsSearchCriteria SearchCriteria)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ConfigurationManagerServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginGetBedAllocationAll_ByDeptID(SearchCriteria, Globals.DispatchCallback((asyncResult) =>
                        {
                            IList<MedServiceItemPrice> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndGetBedAllocationAll_ByDeptID(asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                                MessageBox.Show(fault.Message);
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }

                            ObjGetDeptMedServiceItems.Clear();

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    if (allItems.Count == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.A0088_G1_Msg_InfoChuaC_HinhGiaGiuong);
                                        TryClose();
                                    }
                                    else
                                    {
                                        foreach (var item in allItems)
                                        {
                                            ObjGetDeptMedServiceItems.Add(item);
                                        }
                                        if (selectedBedPatientAllocs != null)
                                        {
                                            SelectedGetDeptMedServiceItems = ObjGetDeptMedServiceItems != null ? ObjGetDeptMedServiceItems.FirstOrDefault(x => x.MedServiceID == selectedBedPatientAllocs.BAMedServiceID) : null;
                                        }
                                    }
                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.DlgHideBusyIndicator();
                    MessageBox.Show(ex.Message);
                }
            });

            t.Start();
        }

        AxAutoComplete AcbDoctorStaff { get; set; }
        AutoCompleteBox cboContext { get; set; }
        private ObservableCollection<Staff> _DoctorStaffs;
        public ObservableCollection<Staff> DoctorStaffs
        {
            get
            {
                return _DoctorStaffs;
            }
            set
            {
                if (_DoctorStaffs != value)
                {
                    _DoctorStaffs = value;
                    NotifyOfPropertyChange(() => DoctorStaffs);
                }
            }
        }

        private Staff _gSelectedDoctorStaff;
        public Staff gSelectedDoctorStaff
        {
            get
            {
                return _gSelectedDoctorStaff;
            }
            set
            {
                _gSelectedDoctorStaff = value;
                NotifyOfPropertyChange(() => gSelectedDoctorStaff);
            }
        }

        private void LoadDoctorStaffCollection()
        {
            DoctorStaffs = new ObservableCollection<Staff>(Globals.AllStaffs.Where(x => x.RefStaffCategory != null && x.RefStaffCategory.V_StaffCatType == Globals.ServerConfigSection.CommonItems.StaffCatTypeBAC_SI 
                                                                                    && x.SCertificateNumber != null && x.SCertificateNumber.Length > 9
                                                                                    && (!x.IsStopUsing)).ToList());
        }

        public void DoctorStaff_Loaded(object sender, RoutedEventArgs e)
        {
            AcbDoctorStaff = (AxAutoComplete)sender;
        }

        public void DoctorStaff_Populating(object sender, PopulatingEventArgs e)
        {
            cboContext = sender as AutoCompleteBox;
            e.Cancel = true;
            var AllItemsContext = new ObservableCollection<Staff>(DoctorStaffs.Where(x => Globals.RemoveVietnameseString(x.FullName).ToLower().Contains(Globals.RemoveVietnameseString(cboContext.SearchText).ToLower())));
            cboContext.ItemsSource = AllItemsContext;
            cboContext.PopulateComplete();
        }

        public void DoctorStaff_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            gSelectedDoctorStaff = ((AutoCompleteBox)sender).SelectedItem as Staff;
        }

        private bool _IsEdit;
        public bool IsEdit
        {
            get
            {
                return _IsEdit;
            }
            set
            {
                _IsEdit = value;
                NotifyOfPropertyChange(() => IsEdit);
            }
        }
    }
}
