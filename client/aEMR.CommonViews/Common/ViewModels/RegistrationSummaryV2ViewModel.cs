using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using aEMR.CommonTasks;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.CachingUtils;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.GlobalFuncs;
using aEMR.ViewContracts;
using Caliburn.Micro;
using Castle.Core.Logging;
using Castle.Windsor;
using DataEntities;
using Service.Core.Common;
using eHCMSLanguage;
using System.Threading;
using aEMR.ServiceClient;
//using aEMR.Common.PagedCollectionView;
using aEMR.Common.Utilities;
using aEMR.Common.Collections;
using aEMR.Common.HotKeyManagement;
using aEMR.Common.BaseModel;
using System.Windows.Input;
using System.Windows.Data;
using System.ServiceModel;
using System.Text;
/*
* 20181105 #001 TTM:    Bổ sung cho fix lỗi quyết toán 2 lần làm die chương trình
* 20181116 #002 TBL:    BM 0005210: Thêm hiển thị tổng tiền, tiền bảo hiểm trả, tổng bệnh nhân trả
* 20181212 #003 TTM:    BM 0005413: Lưu trường chẩn đoán ban đầu (BasicDiagTreatment)
* 20190222 #004 TTM:    Sửa lỗi chuyển tab (bấm bỏ qua) sẽ làm cho đăng ký đang xác nhận BHYT thay đổi quyền lợi về 30%
* 20190222 #005 TTM:    Bổ sung hot key chuyển tab giữa Grid Dịch vụ mới và dịch vụ đã thanh toán
* 20190409 #006 TTM:    BM 0006662: Kiểm tra trẻ em 6 tuổi khi lưu. 
*                       Lý do kiểm tra: Vì khi tìm kiếm bệnh nhân lên đăng ký đã bỏ qua việc yield break khi tìm thấy trẻ em dưới 6 tuổi
*                       Nên kiểm tra thêm 1 lần nữa khi lưu để ngăn chặn lại (Trẻ em dưới 6 tuổi phải đầy đủ thông tin ngày tháng năm sinh để thanh toán BHYT).
* 20190617 #007 CMN:    Recalculate service item price when it was be set in ekip
* 20191012 #008 TBL:    Task #478: Fix lỗi hủy bỏ popup trả tiền khiến tick BH không lấy lại giá BH
* 20191025 #009 TBL:    BM 0018467: Thêm IsNotCheckInvalid để khỏi kiểm tra khoảng thời gian giữa 2 lần làm CLS được tính BHYT
* 20191105 #010 TTM:    BM 0018535: Lỗi cảnh báo ngoài phác đồ điều trị trên màn hình đăng ký dịch vụ ngoại trú
* 20191121 #011 TTM:    BM 0019640: Bổ sung thêm kiểm tra số lần cho phép đăng ký dịch vụ bảo hiểm (Khám bệnh).
* 20200113 #012 TTM:    BM 0021787: Fix lỗi không lưu thông tin miễn giảm và bảo hiểm nếu lưu và trả tiền cho dịch vụ đã được lưu và thay đổi thông tin miễn giảm và bảo hiểm.
* 20200208 #013 TBL:    BM 0022891: Thay đổi cách lưu Ekip
* 20200807 #014 TTM:    BM 0041446: Fix lỗi SimplePay tính toán sai khi xóa bớt 1 số DV XN cùng trong 1 phiếu
* 20200811 #015 TTM:    BM 0040429: Fix ý số 2 của task, khi hủy ekip đầu tiên ra thì các ekip còn lại trong cùng 1 nhóm sẽ tự động clear.
* 20200821 #016 TTM:    BM 0041462: Ngăn không cho bác sĩ có thể trả tiền nếu dùng phím tắt.
* 20200822 #017 TTM:    BM 0041463: Bổ sung nút xem in Yêu cầu dịch vụ khám chữa bệnh.
* 20200906 #018 TNHX:   BM: Chỉnh cấu hình BlockRegNoTicket
* 20201019 #019 TNHX:   BM: Thêm dữ liệu khi quyết toán (Người tạo + Nơi làm việc)
* 20210701 #020 TNHX:   260 Thêm user bsi mượn
* 20211117 #021 TNHX:   chỉnh lại số lượng khi bỏ từ bên phải qua cho gói dịch vụ
* 20220531 #022 DatTB: Thêm biến IsWithOutBill Hủy quyết toán bỏ qua HDDT và phân quyền
* 20220531 #023 DatTB: Thêm nút Hủy quyết toán bỏ qua HDDT và phân quyền
* 20221027 #024 QTD:   Đánh dấu ưu tiên cho CLS
* 20221228 #025 QTD:   Chỉ định gói CLS màn hình Chỉ định dịch vụ bác sĩ
* 20230701 #026 DatTB: Thêm/Sửa module phân quyền cho 2 nút hủy quyết toán
*/
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IRegistrationSummaryV2)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RegistrationSummaryV2ViewModel : ViewModelBase, IRegistrationSummaryV2
        , IHandle<RemoveItem<PatientRegistrationDetail>>
        , IHandle<RemoveItem<PatientPCLRequestDetail>>
        , IHandle<RemoveItem<PatientPCLRequest>>
        , IHandle<RemoveItem<InPatientBillingInvoice>>
        , IHandle<StateChanged<PatientPCLRequest>>
        , IHandle<StateChanged<PatientRegistrationDetail>>
        , IHandle<ItemSelected<Patient>>
        , IHandle<SaveRegisFromSimplePayCompleted>
        , IHandle<ChangeHIStatus<PatientRegistrationDetail>>
        , IHandle<ChangePCLHIStatus<PatientPCLRequestDetail>>
        , IHandle<SetEkipForServiceSuccess>
        , IHandle<RegDetailSelectedForProcess>
        , IHandle<ChangeDiscountStatus<PatientRegistrationDetail>>
        , IHandle<ChangeDiscountStatus<PatientPCLRequestDetail>>
        , IHandle<CreateNewMedicalServiceGroup>
        , IHandle<BasicDiagTreatmentChanged>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public RegistrationSummaryV2ViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            base.HasInputBindingCmd = true;
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            _eventAggregator = eventAggregator;

            authorization();
            var newServiceVm = Globals.GetViewModel<IOutPatientServiceManage>();
            NewServiceContent = newServiceVm;
            NewServiceContent.IsOldList = false;
            ActivateItem(newServiceVm);

            var oldServiceVm = Globals.GetViewModel<IOutPatientServiceManage>();
            OldServiceContent = oldServiceVm;
            OldServiceContent.IsOldList = true;
            ActivateItem(oldServiceVm);

            var newPclVm = Globals.GetViewModel<IOutPatientPclRequestManage>();
            NewPclContent = newPclVm;
            NewPclContent.IsOldList = false;
            ActivateItem(newPclVm);

            var oldPclVm = Globals.GetViewModel<IOutPatientPclRequestManage>();
            OldPclContent = oldPclVm;
            OldPclContent.IsOldList = true;
            ActivateItem(oldPclVm);

            var oldDrugVm = Globals.GetViewModel<IOutPatientDrugManage>();
            OldDrugContent = oldDrugVm;
            ActivateItem(oldDrugVm);

            var oldPaymentVm = Globals.GetViewModel<IPatientPayment>();
            OldPaymentContent = oldPaymentVm;
            OldPaymentContent.ShowPrintColumn = true;
            ActivateItem(oldPaymentVm);

            _eventAggregator.Subscribe(this);

            ShowCheckBoxColumn = true;

            //ShowAddRegisButton = Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ShowAddRegisButton]);
            //AllowDuplicateMedicalServiceItems = Convert.ToInt16(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowDuplicateMedicalServiceItems]);

            // Txd 25/05/2014 Replaced ConfigList
            ShowAddRegisButton = Globals.ServerConfigSection.CommonItems.ShowAddRegisButton;
            AllowDuplicateMedicalServiceItems = Globals.ServerConfigSection.CommonItems.AllowDuplicateMedicalServiceItems;
            LoadMedicalServiceGroupCollection();
            GetAllMedicalServiceItemsByType(null);

            NewBillingContent = Globals.GetViewModel<IOutPatientBillingManage>();
            ActivateItem(NewBillingContent);
            OldBillingContent = Globals.GetViewModel<IOutPatientBillingManage>();
            ActivateItem(OldBillingContent);
        }

        private int AllowDuplicateMedicalServiceItems { get; set; }

        private bool ShowAddRegisButton { get; set; }
        public bool IsCrossRegion { get; set; }
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IRegistrationSummaryV2View;
            if (_currentView != null)
            {
                _currentView.ShowClickButton(_showClickButton);
            }
        }
        #region PROPERTIES

        private int? Apply15HIPercent;

        //private bool _IsEnableRoleUser;
        //public bool IsEnableRoleUser
        //{
        //    get { return _IsEnableRoleUser; }
        //    set
        //    {
        //        _IsEnableRoleUser = value;
        //        NotifyOfPropertyChange(() => IsEnableRoleUser);
        //    }
        //}
        private bool _showClickButton = true;
        public bool ShowClickButton
        {
            get { return _showClickButton; }
            set
            {
                if (_showClickButton != value)
                {
                    _showClickButton = value;
                    NotifyOfPropertyChange(() => ShowClickButton);
                    var view = this.GetView() as IRegistrationSummaryV2View;
                    if (view != null)
                    {
                        view.ShowClickButton(_showClickButton);
                    }
                }
            }
        }

        private IRegistrationSummaryV2View _currentView;

        private IOutPatientDrugManage _oldDrugContent;
        public IOutPatientDrugManage OldDrugContent
        {
            get { return _oldDrugContent; }
            set { _oldDrugContent = value; }
        }

        //private IOutPatientDrugManage _newDrugContent;
        //public IOutPatientDrugManage NewDrugContent
        //{
        //    get { return _newDrugContent; }
        //    set { _newDrugContent = value; }
        //}

        private ObservableCollection<Staff> _allStaffs;
        public ObservableCollection<Staff> allStaffs
        {
            get
            {
                return _allStaffs;
            }
            set
            {
                if (_allStaffs != value)
                {
                    _allStaffs = value;
                    NotifyOfPropertyChange(() => allStaffs);
                }
            }
        }

        private IOutPatientPclRequestManage _oldPclContent;
        public IOutPatientPclRequestManage OldPclContent
        {
            get { return _oldPclContent; }
            set { _oldPclContent = value; }
        }

        private IOutPatientPclRequestManage _newPclContent;
        public IOutPatientPclRequestManage NewPclContent
        {
            get { return _newPclContent; }
            set { _newPclContent = value; }
        }

        private IOutPatientServiceManage _oldServiceContent;
        public IOutPatientServiceManage OldServiceContent
        {
            get { return _oldServiceContent; }
            set { _oldServiceContent = value; }
        }

        private IOutPatientServiceManage _newServiceContent;
        public IOutPatientServiceManage NewServiceContent
        {
            get { return _newServiceContent; }
            set { _newServiceContent = value; }
        }

        private IPatientPayment _oldPaymentContent;
        public IPatientPayment OldPaymentContent
        {
            get { return _oldPaymentContent; }
            set { _oldPaymentContent = value; }
        }

        private PatientRegistration _currentRegistration;

        public PatientRegistration CurrentRegistration
        {
            get { return _currentRegistration; }
            set
            {
                if (_currentRegistration != value)
                {
                    //20190113 TTM: Khi thay đổi đăng ký sẽ clear chẩn đoán sơ bộ, tránh trường hợp không clear
                    //dẫn đến 1 cdsb cho nhiều bệnh nhân
                    BasicDiagTreatment = "";
                    _currentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                    NotifyOfPropertyChange(() => CanNewCount15HIPercentCmd);
                    NotifyOfPropertyChange(() => CanCreateHIReport);
                    InitRegistration();
                }
            }
        }

        public bool CanAddService
        {
            get { return _currentTabIndex == 0 && IsInEditMode; }
        }

        public bool CanPaidService
        {
            get { return _currentTabIndex == 1 && IsInEditMode; } //TBL: Khi chuyen qua tab Cac dich vu da thanh toan thi true
        }

        private bool _showButtonList = true;
        public bool ShowButtonList
        {
            get { return _showButtonList; }
            set
            {
                _showButtonList = value;
                NotifyOfPropertyChange(() => ShowButtonList);
            }
        }

        private bool _showCheckBoxColumn;
        public bool ShowCheckBoxColumn
        {
            get { return _showCheckBoxColumn; }
            set
            {
                _showCheckBoxColumn = value;
                NotifyOfPropertyChange(() => ShowCheckBoxColumn);

                OldPclContent.ShowCheckBoxColumn = _showCheckBoxColumn;
                NewPclContent.ShowCheckBoxColumn = _showCheckBoxColumn;
                OldServiceContent.ShowCheckBoxColumn = _showCheckBoxColumn;
                NewServiceContent.ShowCheckBoxColumn = _showCheckBoxColumn;
            }
        }

        private bool _registrationLoading;

        public bool RegistrationLoading
        {
            get { return _registrationLoading; }
            set
            {
                _registrationLoading = value;
                NotifyOfPropertyChange(() => RegistrationLoading);
            }
        }

        private bool _registrationInfoHasChanged;
        public bool RegistrationInfoHasChanged
        {
            get
            {
                return _registrationInfoHasChanged;
            }
            set
            {
                if (_registrationInfoHasChanged != value)
                {
                    _registrationInfoHasChanged = value;
                    if (_registrationInfoHasChanged)
                    {
                        Globals.HIRegistrationForm = string.Format(eHCMSResources.Z1126_G1_ChuaLuuDKBN, CurrentRegistration.Patient.FullName);
                    }
                    else
                    {
                        Globals.HIRegistrationForm = "";
                    }
                    NotifyOfPropertyChange(() => RegistrationInfoHasChanged);
                    NotifyOfPropertyChange(() => CanSaveNewServicesAndPclCmd);
                    NotifyOfPropertyChange(() => CanSaveAndPayForNewServiceCmd);
                    NotifyOfPropertyChange(() => CanUpdateRegistrationCmd);
                    NotifyOfPropertyChange(() => CanPayForNewServiceCmd);
                    NotifyOfPropertyChange(() => CanNewCount15HIPercentCmd);
                    NotifyOfPropertyChange(() => CanPayForOldServiceCmd);
                    NotifyOfPropertyChange(() => CanCancelAddingServiceAndPCLCmd);
                    NotifyOfPropertyChange(() => CanCancelChangesOnRegistrationCmd);
                    NotifyOfPropertyChange(() => CanSaveAndPayForOldServiceCmd);
                    NotifyOfPropertyChange(() => CanbtnCancelService);
                }
            }
        }

        private bool _hiServiceBeingUsed;
        public bool HiServiceBeingUsed
        {
            get { return _hiServiceBeingUsed; }
            set
            {
                if (_hiServiceBeingUsed != value)
                {
                    _hiServiceBeingUsed = value;
                    NotifyOfPropertyChange(() => HiServiceBeingUsed);

                    NewServiceContent.HiServiceBeingUsed = value;
                    OldServiceContent.HiServiceBeingUsed = value;

                    //NewDrugContent.HiServiceBeingUsed = value;
                    OldDrugContent.HiServiceBeingUsed = value;

                    NewPclContent.HiServiceBeingUsed = value;
                    OldPclContent.HiServiceBeingUsed = value;
                }
            }
        }

        private RegistrationFormMode _currentRegMode = RegistrationFormMode.PATIENT_NOT_SELECTED;
        public RegistrationFormMode CurrentRegMode
        {
            get
            {
                return _currentRegMode;
            }
            set
            {
                if (_currentRegMode != value)
                {
                    _currentRegMode = value;
                    NotifyOfPropertyChange(() => CurrentRegMode);
                }
            }
        }

        private long _DoctorStaffID;
        public long DoctorStaffID
        {
            get
            {
                return _DoctorStaffID;
            }
            set
            {
                if (_DoctorStaffID != value)
                {
                    _DoctorStaffID = value;
                    NotifyOfPropertyChange(() => DoctorStaffID);
                }
            }
        }

        private bool _IsNotCheckInvalid;
        public bool IsNotCheckInvalid
        {
            get { return _IsNotCheckInvalid; }
            set
            {
                _IsNotCheckInvalid = value;
                NotifyOfPropertyChange(() => IsNotCheckInvalid);
            }
        }

        private long _ServiceRecID;
        public long ServiceRecID
        {
            get
            {
                return _ServiceRecID;
            }
            set
            {
                if (_ServiceRecID != value)
                {
                    _ServiceRecID = value;
                    NotifyOfPropertyChange(() => ServiceRecID);
                }
            }
        }

        private bool _IsProcess;
        public bool IsProcess
        {
            get { return _IsProcess; }
            set
            {
                if (_IsProcess != value)
                {
                    _IsProcess = value;
                    NotifyOfPropertyChange(() => IsProcess);
                }
            }
        }

        private SeachPtRegistrationCriteria _searchRegCriteria;
        public SeachPtRegistrationCriteria SearchRegCriteria
        {
            get { return _searchRegCriteria; }
            set
            {
                _searchRegCriteria = value;
                NotifyOfPropertyChange(() => SearchRegCriteria);
            }
        }

        #region Tính tiền dự đoán
        private string _TongGiaTien_Text;
        public string TongGiaTien_Text
        {
            get { return _TongGiaTien_Text; }
            set
            {
                if (_TongGiaTien_Text != value)
                {
                    _TongGiaTien_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTien_Text);
                }
            }
        }

        private string _TongGiaTienBH_Text;
        public string TongGiaTienBH_Text
        {
            get { return _TongGiaTienBH_Text; }
            set
            {
                if (_TongGiaTienBH_Text != value)
                {
                    _TongGiaTienBH_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTienBH_Text);
                }
            }
        }

        private string _TongGiaTienBN_Text;
        public string TongGiaTienBN_Text
        {
            get { return _TongGiaTienBN_Text; }
            set
            {
                if (_TongGiaTienBN_Text != value)
                {
                    _TongGiaTienBN_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTienBN_Text);
                }
            }
        }

        private string _TongGiaTienDaTra_Text;
        public string TongGiaTienDaTra_Text
        {
            get { return _TongGiaTienDaTra_Text; }
            set
            {
                if (_TongGiaTienDaTra_Text != value)
                {
                    _TongGiaTienDaTra_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTienDaTra_Text);
                }
            }
        }

        private string _TongGiaTienBHDaTra_Text;
        public string TongGiaTienBHDaTra_Text
        {
            get { return _TongGiaTienBHDaTra_Text; }
            set
            {
                if (_TongGiaTienBHDaTra_Text != value)
                {
                    _TongGiaTienBHDaTra_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTienBHDaTra_Text);
                }
            }
        }

        private string _TongGiaTienBNDaTra_Text;
        public string TongGiaTienBNDaTra_Text
        {
            get { return _TongGiaTienBNDaTra_Text; }
            set
            {
                if (_TongGiaTienBNDaTra_Text != value)
                {
                    _TongGiaTienBNDaTra_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTienBNDaTra_Text);
                }
            }
        }

        private string _TongGiaTienDaHuy_Text;
        public string TongGiaTienDaHuy_Text
        {
            get { return _TongGiaTienDaHuy_Text; }
            set
            {
                if (_TongGiaTienDaHuy_Text != value)
                {
                    _TongGiaTienDaHuy_Text = value;
                    NotifyOfPropertyChange(() => TongGiaTienDaHuy_Text);
                }
            }
        }

        private string _TongGiaMG_Text;
        public string TongGiaMG_Text
        {
            get { return _TongGiaMG_Text; }
            set
            {
                if (_TongGiaMG_Text != value)
                {
                    _TongGiaMG_Text = value;
                    NotifyOfPropertyChange(() => TongGiaMG_Text);
                }
            }
        }

        private string _TongGiaDaMG_Text;
        public string TongGiaDaMG_Text
        {
            get { return _TongGiaDaMG_Text; }
            set
            {
                if (_TongGiaDaMG_Text != value)
                {
                    _TongGiaDaMG_Text = value;
                    NotifyOfPropertyChange(() => TongGiaDaMG_Text);
                }
            }
        }

        public void TinhTongGiaTien()
        {
            if (_currentRegistration == null)
            {
                return;
            }
            decimal TongGia = 0;
            decimal TongGiaBH = 0;
            decimal TongGiaBN = 0;
            decimal TongGiaMG = 0;
            decimal TongGiaDaTra = 0;
            decimal TongGiaBHDaTra = 0;
            decimal TongGiaBNDaTra = 0;
            decimal TongGiaDaHuy = 0;
            decimal TongGiaDaMG = 0;
            if (_currentRegistration.PatientRegistrationDetails != null && _currentRegistration.PatientRegistrationDetails.Count > 0)
            {
                foreach (var serItem in _currentRegistration.PatientRegistrationDetails)
                {
                    if (serItem.RecordState != RecordState.DELETED && serItem.PaidTime == null)
                    {
                        TongGia += serItem.TotalInvoicePrice;
                        TongGiaBH += serItem.TotalHIPayment;
                        TongGiaBN += serItem.TotalPatientPayment;
                        TongGiaMG += serItem.DiscountAmt;
                    }
                }
            }
            if (_currentRegistration.PCLRequests != null && _currentRegistration.PCLRequests.Count > 0)
            {
                foreach (var req in _currentRegistration.PCLRequests)
                {
                    foreach (var pcl in req.PatientPCLRequestIndicators)
                    {
                        if (pcl.RecordState != RecordState.DELETED && pcl.PaidTime == null)
                        {
                            TongGia += pcl.TotalInvoicePrice;
                            TongGiaBH += pcl.TotalHIPayment;
                            TongGiaBN += pcl.TotalPatientPayment;
                            TongGiaMG += pcl.DiscountAmt;
                        }
                    }
                }
            }
            TongGiaTien_Text = string.Format("{0}: ", eHCMSResources.G1472_G1_TCong) + TongGia.ToString("#,##0.##");
            TongGiaTienBH_Text = string.Format("{0}: ", eHCMSResources.G0909_G1_TienBHTra) + TongGiaBH.ToString("#,##0.##");
            TongGiaTienBN_Text = string.Format("{0}: ", eHCMSResources.G1466_G1_TgBNTra) + TongGiaBN.ToString("#,##0.##");
            //TongGiaMG_Text = string.Format("{0}: ", eHCMSResources.Z2390_G1_TongTienMG) + TongGiaMG.ToString("#,##0.##");

            //==== #002 ====
            if (_currentRegistration.PatientRegistrationDetails != null && _currentRegistration.PatientRegistrationDetails.Count > 0)
            {
                foreach (var serItem in _currentRegistration.PatientRegistrationDetails)
                {
                    if (serItem.RecordState != RecordState.DELETED && serItem.PaidTime != null)
                    {
                        TongGiaDaTra += serItem.TotalInvoicePrice;
                        if (serItem.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                        {
                            TongGiaBHDaTra += serItem.TotalHIPayment;
                            TongGiaBNDaTra += serItem.TotalPatientPayment;
                            TongGiaDaMG += serItem.DiscountAmt;
                        }
                        if (serItem.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                        {
                            TongGiaDaHuy += serItem.TotalPatientPayment + serItem.TotalHIPayment + serItem.DiscountAmt;
                        }
                    }
                }
            }
            if (_currentRegistration.PCLRequests != null && _currentRegistration.PCLRequests.Count > 0)
            {
                foreach (var req in _currentRegistration.PCLRequests)
                {
                    foreach (var pcl in req.PatientPCLRequestIndicators)
                    {
                        if (pcl.RecordState != RecordState.DELETED && pcl.PaidTime != null)
                        {
                            TongGiaDaTra += pcl.TotalInvoicePrice;
                            if (pcl.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                            {
                                TongGiaBHDaTra += pcl.TotalHIPayment;
                                TongGiaBNDaTra += pcl.TotalPatientPayment;
                                TongGiaDaMG += pcl.DiscountAmt;
                            }
                            if (pcl.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                            {
                                TongGiaDaHuy += pcl.TotalPatientPayment + pcl.TotalHIPayment + pcl.DiscountAmt;
                            }
                        }
                    }
                }
            }
            if (_currentRegistration.DrugInvoices != null && _currentRegistration.DrugInvoices.Count > 0)
            {
                foreach (var drug in _currentRegistration.DrugInvoices)
                {
                    foreach (var item in drug.OutwardDrugs)
                    {
                        if (item.RecordState != RecordState.DELETED && drug.PaidTime != null)
                        {
                            TongGiaDaTra += item.InvoicePrice * item.Qty;
                            TongGiaBHDaTra += item.TotalHIPayment;
                            TongGiaBNDaTra += item.TotalPatientPayment;
                        }
                        //TBL: Tra thuoc dang co van de nen chua tinh dc
                        //if (item.RecordState == RecordState.DELETED && item.PaidTime != null)
                        //{
                        //    tonggiadahuy += item.TotalPatientPayment;
                        //}
                    }
                }
            }
            TongGiaTienDaTra_Text = string.Format("{0}: ", eHCMSResources.G1472_G1_TCong) + TongGiaDaTra.ToString("#,##0.##");
            TongGiaTienBHDaTra_Text = string.Format("{0}: ", eHCMSResources.G0909_G1_TienBHTra) + TongGiaBHDaTra.ToString("#,##0.##");
            TongGiaTienBNDaTra_Text = string.Format("{0}: ", eHCMSResources.G1466_G1_TgBNTra) + TongGiaBNDaTra.ToString("#,##0.##");
            TongGiaTienDaHuy_Text = string.Format("{0}: ", eHCMSResources.Z2341_G1_TongHuy) + TongGiaDaHuy.ToString("#,##0.##");
            //TongGiaDaMG_Text = string.Format("{0}: ", eHCMSResources.Z2390_G1_TongTienMG) + TongGiaDaMG.ToString("#,##0.##");
            //==== #002 ====
        }

        #endregion

        #endregion
        private bool _IsShowCount15HIPercentCmd = false;
        public bool IsShowCount15HIPercentCmd
        {
            get { return _IsShowCount15HIPercentCmd; }
            set
            {
                _IsShowCount15HIPercentCmd = value;
                NotifyOfPropertyChange(() => IsShowCount15HIPercentCmd);
                NotifyOfPropertyChange(() => CanNewCount15HIPercentCmd);
            }
        }

        //▼==== #026
        private bool _mDelTranFinal = Globals.IsUserAdmin || Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mTranFinal);
        public bool mDelTranFinal
        {
            get
            {
                return _mDelTranFinal;
            }
            set
            {
                if (_mDelTranFinal == value)
                {
                    return;
                }
                _mDelTranFinal = value;
                NotifyOfPropertyChange(() => mDelTranFinal);
            }
        }

        private bool _mDelTranFinalWithOutBill = Globals.IsUserAdmin || Globals.CheckFunction(Globals.listRefModule, (int)eModules.mTransaction_Management
                                               , (int)eTransaction_Management.mTranFinalWithOutBill);
        public bool mDelTranFinalWithOutBill
        {
            get
            {
                return _mDelTranFinalWithOutBill;
            }
            set
            {
                if (_mDelTranFinalWithOutBill == value)
                {
                    return;
                }
                _mDelTranFinalWithOutBill = value;
                NotifyOfPropertyChange(() => mDelTranFinalWithOutBill);
            }
        }
        //▲==== #026
        private void InitRegistration()
        {
            if (CurrentRegistration != null)
            {
                CurrentRegistration.HIApprovedStaffID = Globals.LoggedUserAccount.StaffID;
                InitViewForServiceItems();
                InitViewForPCLRequests();
                InitViewForDrugItems();
                InitViewForPayments();
                gIsHIUnder15Percent = CurrentRegistration.IsHIUnder15Percent;
                if (CurrentRegistration.InPatientBillingInvoices != null)
                {
                    CurrentRegistration.InPatientBillingInvoices = CurrentRegistration.InPatientBillingInvoices.Where(x => (x.OutwardDrugClinicDeptInvoices != null && x.OutwardDrugClinicDeptInvoices.Count > 0) || x.TotalInvoicePrice > 0).ToObservableCollection();
                }
                NewBillingContent.UpdateItemList(CurrentRegistration.InPatientBillingInvoices == null || !CurrentRegistration.InPatientBillingInvoices.Any(x => x.PaidTime == null) ? null : CurrentRegistration.InPatientBillingInvoices.Where(x => x.PaidTime == null).ToList());
                OldBillingContent.UpdateItemList(CurrentRegistration.InPatientBillingInvoices == null || !CurrentRegistration.InPatientBillingInvoices.Any(x => x.PaidTime != null && x.RecordState != RecordState.DELETED) ? null : CurrentRegistration.InPatientBillingInvoices.Where(x => x.PaidTime != null && x.RecordState != RecordState.DELETED).ToList());
            }
        }

        /// <summary>
        /// Gọi hàm này khi thông tin về danh sách Cận Lâm Sàng bị thay đổi
        /// </summary>
        public void RefreshPCLRequestDetailsView()
        {
            InitViewForPCLRequests();
        }

        /// <summary>
        /// Gọi hàm này khi thông tin về danh sách Dịch vụ bị thay đổi
        /// </summary>
        public void RefreshServicesView()
        {
            InitViewForServiceItems();
        }


        private bool CheckHIExpired()
        {
            DateTime Now = Globals.GetCurServerDateTime().Date;

            if (_currentRegistration.HisID.GetValueOrDefault(0) > 0 && _currentRegistration.HealthInsurance != null
                && (Now < _currentRegistration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date || Now > _currentRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date))
            {
                return true;
            }
            return false;
        }

        private void InitViewForServiceItems()
        {
            //Tách danh sách dịch vụ ra 2 bên.
            //1 bên là danh sách đã trả tiền, một bên là chưa trả tiền.
            var oldServiceList = new List<PatientRegistrationDetail>();
            var newServiceList = new List<PatientRegistrationDetail>();

            if (CurrentRegistration != null && CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (var item in CurrentRegistration.PatientRegistrationDetails)
                {
                    if (item.RecordState != RecordState.DELETED)
                    {
                        if (item.PaidTime.HasValue && item.PaidTimeTmp == null) //Da tra tien
                        {
                            oldServiceList.Add(item);
                        }
                        else
                        {
                            item.IsChecked = true;
                            newServiceList.Add(item);
                        }
                    }
                }
            }


            OldServiceContent.UpdateServiceItemList(oldServiceList);
            NewServiceContent.UpdateServiceItemList(newServiceList);
            //OldServiceContent.RegistrationDetails = new aEMR.Common.PagedCollectionView.PagedCollectionView(oldServiceList);
            //OldServiceContent.RegistrationDetails.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("RefMedicalServiceItem.RefMedicalServiceType"));

            //NewServiceContent.RegistrationDetails = new aEMR.Common.PagedCollectionView.PagedCollectionView(newServiceList);
            //NewServiceContent.RegistrationDetails.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("RefMedicalServiceItem.RefMedicalServiceType"));

            TinhTongGiaTien();
        }

        private void InitViewForPayments()
        {
            OldPaymentContent.InitViewForPayments(CurrentRegistration);
        }

        private void InitViewForPCLRequests()
        {
            var oldList = new ObservableCollection<PatientPCLRequest>();
            var newList = new ObservableCollection<PatientPCLRequest>();

            if (CurrentRegistration != null && CurrentRegistration.PCLRequests != null)
            {
                foreach (var request in CurrentRegistration.PCLRequests)
                {
                    if (request.RecordState == RecordState.DELETED || request.PatientPCLRequestIndicators == null)
                        continue;
                    if (request.PaidTime.HasValue) //Da tra tien
                    {
                        oldList.Add(request);
                    }
                    else
                    {
                        request.IsChecked = true;
                        newList.Add(request);
                    }
                }
            }
            OldPclContent.PCLRequests = oldList;
            NewPclContent.PCLRequests = newList;

            TinhTongGiaTien();

            ////Tách danh sách dịch vụ ra 2 bên.
            ////1 bên là danh sách đã trả tiền, một bên là chưa trả tiền.
            //List<PatientPCLRequestDetail> oldServiceList = new List<PatientPCLRequestDetail>();
            //List<PatientPCLRequestDetail> newServiceList = new List<PatientPCLRequestDetail>();

            //if (CurrentRegistration != null && CurrentRegistration.PCLRequests != null)
            //{
            //    var requestDetails = new List<PatientPCLRequestDetail>();
            //    foreach (PatientPCLRequest request in CurrentRegistration.PCLRequests)
            //    {
            //        if (request.RecordState != RecordState.DELETED && request.PatientPCLRequestIndicators != null)
            //        {
            //            if (request.PaidTime.HasValue) //Da tra tien
            //            {
            //                foreach (PatientPCLRequestDetail item in request.PatientPCLRequestIndicators)
            //                {
            //                    if (item.RecordState != RecordState.DELETED)
            //                    {
            //                        oldServiceList.Add(item);
            //                    }

            //                    item.PatientPCLRequest = request;
            //                }
            //            }
            //            else
            //            {
            //                request.IsChecked = true;
            //                foreach (PatientPCLRequestDetail item in request.PatientPCLRequestIndicators)
            //                {
            //                    if (item.RecordState != RecordState.DELETED)
            //                    {
            //                        newServiceList.Add(item);
            //                    }

            //                    item.PatientPCLRequest = request;
            //                }
            //            }
            //        }
            //    }
            //}

            //OldPclContent.PclServiceDetails = new PagedCollectionView(oldServiceList);
            //OldPclContent.PclServiceDetails.GroupDescriptions.Add(new PropertyGroupDescription("PatientPCLRequest"));

            //NewPclContent.PclServiceDetails = new PagedCollectionView(newServiceList);
            //NewPclContent.PclServiceDetails.GroupDescriptions.Add(new PropertyGroupDescription("PatientPCLRequest"));
        }

        private void InitViewForDrugItems()
        {
            var newDrugList = new List<OutwardDrug>();
            var oldDrugList = new List<OutwardDrug>();

            if (CurrentRegistration.DrugInvoices != null)
            {
                foreach (var inv in CurrentRegistration.DrugInvoices)
                {
                    if (inv.RecordState != RecordState.DELETED && inv.OutwardDrugs != null)
                    {
                        if (inv.PaidTime == null)
                        {
                            newDrugList.AddRange(inv.OutwardDrugs);
                        }
                        else
                        {
                            oldDrugList.AddRange(inv.OutwardDrugs);
                            if (inv.ReturnedInvoices != null)
                            {
                                foreach (var innerInv in inv.ReturnedInvoices)
                                {
                                    if (innerInv.OutwardDrugs != null)
                                    {
                                        oldDrugList.AddRange(innerInv.OutwardDrugs);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            OldDrugContent.DrugItems = new aEMR.Common.PagedCollectionView.PagedCollectionView(oldDrugList);
            OldDrugContent.DrugItems.GroupDescriptions.Add(new aEMR.Common.PagedCollectionView.PropertyGroupDescription("OutwardDrugInvoice"));

            //NewDrugContent.DrugItems = new PagedCollectionView(newDrugList);
            //NewDrugContent.DrugItems.GroupDescriptions.Add(new PropertyGroupDescription("OutwardDrugInvoice"));
        }

        private bool _IsEnableSaveButton = true;
        public bool IsEnableSaveButton
        {
            get
            {
                return _IsEnableSaveButton;
            }
            set
            {
                if (_IsEnableSaveButton == value)
                {
                    return;
                }
                _IsEnableSaveButton = value;
                NotifyOfPropertyChange(() => IsEnableSaveButton);
            }
        }
        public void SetRegistration(PatientRegistration registrationInfo)
        {
            if (registrationInfo != null && registrationInfo.PtRegistrationID == 0 &&
                registrationInfo.Appointment != null &&
                registrationInfo.Appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE &&
                registrationInfo.PatientRegistrationDetails != null &&
                registrationInfo.PatientRegistrationDetails.Any(x => x.RefMedicalServiceItem != null && x.RefMedicalServiceItem.MedServiceID > 0 && x.DeptLocation == null))
            {
                foreach (var Item in registrationInfo.PatientRegistrationDetails.Where(x => x.RefMedicalServiceItem != null && x.RefMedicalServiceItem.MedServiceID > 0 && x.DeptLocation == null))
                {
                    Item.RefMedicalServiceItem.allDeptLocation = GetDefaultLocationsByServiceIDFromCatche(Item.RefMedicalServiceItem.MedServiceID);
                    Item.DeptLocation = Item.RefMedicalServiceItem.allDeptLocation.FirstOrDefault();
                }
            }
            //▼====: #007
            if (registrationInfo != null && registrationInfo.PtRegistrationID > 0
                && registrationInfo.PatientRegistrationDetails != null
                && registrationInfo.PatientRegistrationDetails.Any(x => x.PtRegDetailID > 0 && x.PaidTime == null && x.V_EkipIndex != null && x.V_EkipIndex.LookupID > 0))
            {
                foreach (var aRegDetailItem in registrationInfo.PatientRegistrationDetails.Where(x => x.PtRegDetailID > 0 && x.PaidTime == null && x.V_EkipIndex != null && x.V_EkipIndex.LookupID > 0).ToList())
                {
                    aRegDetailItem.GetItemPrice(registrationInfo, null);
                    aRegDetailItem.GetItemTotalPrice();
                }
            }
            //▲====: #007
            CurrentRegistration = registrationInfo;
            //CorrectRegistrationDetailsForAppt();
            RegistrationInfoHasChanged = false;
            IsInEditMode = false;
            IsShowCount15HIPercentCmd = CurrentRegistration.IsCrossRegion.GetValueOrDefault(true) ? false : true;
            NewServiceContent.RegistrationObj = CurrentRegistration;
            OldServiceContent.RegistrationObj = CurrentRegistration;
            NewPclContent.RegistrationObj = CurrentRegistration;
            OldPclContent.RegistrationObj = CurrentRegistration;
            //20190105 TTM: Nếu cấu hình cho phép thay đổi thông tin bảo hiểm sau khi đã lưu và trả tiền thì phải set giá trị cho RegistrationObj để tính toán lại thông tin bảo hiểm.
            if (Globals.ServerConfigSection.CommonItems.ChangeHIAfterSaveAndPayRule)
            {
                OldServiceContent.RegistrationObj = CurrentRegistration;
                OldPclContent.RegistrationObj = CurrentRegistration;
            }
            if (CurrentRegistration != null && CurrentRegistration.PtRegistrationID == 0 && CurrentRegistration.Appointment != null && CurrentRegistration.Appointment.V_AppointmentType == (long)AllLookupValues.AppointmentType.HEN_KHAM_SUC_KHOE)
            {
                IsEnableSaveButton = false;
            }
            else
            {
                IsEnableSaveButton = true;
            }
        }
        #region COMMANDS

        #region NHỮNG DỊCH VỤ CHƯA TÍNH TIỀN
        /// <summary>
        /// Chuyển sang trạng thái bắt đầu thêm mới dịch vụ.
        /// </summary>
        public void StartAddingNewServicesAndPclCmd(bool fromAppointment = false)
        {
            BeginEdit();
            if (fromAppointment)
            {
                RegistrationInfoHasChanged = true;
            }
        }
        private bool _canStartAddingNewServicesAndPclEx = true;
        public bool CanStartAddingNewServicesAndPclEx
        {
            get
            {
                return _canStartAddingNewServicesAndPclEx;
            }
            set
            {
                _canStartAddingNewServicesAndPclEx = value;
                NotifyOfPropertyChange(() => CanStartAddingNewServicesAndPclCmd);
            }
        }
        public bool CanStartAddingNewServicesAndPclCmd
        {
            get
            {
                return !IsInEditMode && CanStartAddingNewServicesAndPclEx;
            }
        }

        /// <summary>
        /// Lưu danh sách dịch vụ mới thêm vào hoặc những dịch vụ chưa trả tiền bị xóa.
        /// </summary>
        public void SaveNewServicesAndPclCmd()
        {
            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, eHCMSResources.T0783_G1_TaoBillMoi.ToLower()))
            {
                if (!Globals.ServerConfigSection.InRegisElements.ShowMessageBoxForLockReportedRegistration)
                {
                    MessageBox.Show(eHCMSResources.Z2197_G1_CaDieuTriDaDuocBCBHYT, eHCMSResources.G0442_G1_TBao);
                }
                return;
            }
            //▼===== #010: Bổ sung thêm RegistrationView để phân biệt view này từ đâu gọi chỉ định hay đăng ký.
            //             Nếu từ đăng ký thì by pass kiểm tra.
            if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && !RegistrationView && CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Any(x => x.PatientPCLReqID == 0) && !CommonGlobals.CheckValidRequestRegimen(CurrentRegistration.PCLRequests.First(x => x.PatientPCLReqID == 0), TreatmentRegimenCollection))
            {
                if (MessageBox.Show(eHCMSResources.Z2694_G1_PhieuYeuCauDVNgoaiPhacDo, eHCMSResources.K1576_G1_CBao, System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            //▲===== #010
            //▼===== #011
            if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurrentRegistration, null, CurrentRegistration.PatientRegistrationDetails))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                return;
            }
            //▲===== #011
            if (!Globals.CheckServiceForHIPatient(CurrentRegistration, CurrentRegistration.PatientRegistrationDetails)) //&& !RegistrationView)
            {
                string HuongXuLy = RegistrationView ? "Bạn cần tạo đăng ký mới" : "Liên hệ kế toán thu để đăng ký";
                MessageBox.Show("Người bệnh là đối tượng BHYT không được phép đăng ký phiếu khám dịch vụ"+ Environment.NewLine +"Hướng xử lý: " + HuongXuLy);
                return;
            }
            if(CurrentRegistration.PtInsuranceBenefit > 0 && CurrentRegistration.V_MedicalExaminationType == null)
            {
                MessageBox.Show("Chưa chọn Loại KCB");
                return;
            }
            if(CurrentRegistration.PtInsuranceBenefit > 0 && CurrentRegistration.V_ReasonHospitalStay == null)
            {
                MessageBox.Show("Chưa chọn Lý do vào viện");
                return;
            }
            this.DlgShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            _currentRegistration.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
            Coroutine.BeginExecute(DoSaveAndPayNewServices(false, IsFromRequestDoctor), null, (o, e) =>
            {
                this.DlgHideBusyIndicator();
            });

        }
        public bool CanSaveNewServicesAndPclCmd
        {
            get
            {
                return RegistrationInfoHasChanged;
            }
        }

        /// <summary>
        /// Lưu và tính tiền luôn cho những dịch vụ mới thêm vào và những dịch vụ chưa trả tiền bị xóa.
        /// </summary>
        public void SaveAndPayForNewServiceCmd(bool IsHotKey = false)
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "lưu và trả tiền cho bill"))
            {
                if (!Globals.ServerConfigSection.InRegisElements.ShowMessageBoxForLockReportedRegistration)
                {
                    MessageBox.Show(eHCMSResources.Z2197_G1_CaDieuTriDaDuocBCBHYT, eHCMSResources.G0442_G1_TBao);
                }
                return;
            }
            //▼===== #010: Bổ sung thêm RegistrationView để phân biệt view này từ đâu gọi chỉ định hay đăng ký.
            //             Nếu từ đăng ký thì by pass kiểm tra.
            if (Globals.ServerConfigSection.ConsultationElements.EnableTreatmentRegimen && !RegistrationView && CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Any(x => x.PatientPCLReqID == 0) && !CommonGlobals.CheckValidRequestRegimen(CurrentRegistration.PCLRequests.First(x => x.PatientPCLReqID == 0), TreatmentRegimenCollection))
            {
                if (MessageBox.Show(eHCMSResources.Z2694_G1_PhieuYeuCauDVNgoaiPhacDo, eHCMSResources.K1576_G1_CBao, System.Windows.MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            //▲=====
            //▼===== #011
            if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurrentRegistration, null, CurrentRegistration.PatientRegistrationDetails))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                return;
            }
            //▲===== #011
           
            if (CurrentRegistration.PtInsuranceBenefit > 0 && CurrentRegistration.V_ReasonHospitalStay == null)
            {
                MessageBox.Show("Chưa chọn Lý do vào viện");
                return;
            }
            if (CurrentRegistration.PtInsuranceBenefit > 0 && CurrentRegistration.V_MedicalExaminationType == null)
            {
                MessageBox.Show("Chưa chọn Loại KCB");
                return;
            }
            //if (CurrentRegistration.AdmissionICD10 == null)
            //{
            //    MessageBox.Show("Chưa chọn chẩn đoán ban đầu");
            //    return;
            //}
            _currentRegistration.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
            bool IsSaveByHotKey = IsHotKey;
            if (Globals.ServerConfigSection.InRegisElements.Use_SaveRegisThenPay)
            {
                Coroutine.BeginExecute(SaveAndPayForNewServicesAndPclReqs(IsSaveByHotKey), null, (o, e) =>
                {                    
                });
            }
            else
            {
                this.ShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
                Coroutine.BeginExecute(DoSaveAndPayNewServices(true), null, (o, e) =>
                {                    
                    this.HideBusyIndicator();
                });

            }
        }

        public bool CanSaveAndPayForNewServiceCmd
        {
            get { return RegistrationInfoHasChanged; }
        }


        /// <summary>
        /// Trả tiền cho những dịch vụ chưa được tính tiền (và được chọn)
        /// </summary>
        /// 
        public void PayForNewServiceCmd(bool bRegisAndPay)
        {
            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "trả tiền bill"))
            {
                return;
            }
            //▼===== #011
            if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurrentRegistration, null, CurrentRegistration.PatientRegistrationDetails))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                return;
            }
            //▲===== #011
            Coroutine.BeginExecute(PayForNewService(bRegisAndPay));
        }


        private bool CheckHIServiceAndPCLNotPaidYet()
        {
            if (CurrentRegistration == null)
            {
                return false;
            }

            if (CurrentRegistration.PatientRegistrationDetails != null && CurrentRegistration.PatientRegistrationDetails.Any(item => item.PaidTime == null && item.HIBenefit > 0))
            {
                return true;
            }

            if (CurrentRegistration.PCLRequests != null)
            {
                foreach (var request in CurrentRegistration.PCLRequests)
                {
                    if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Any(item => item.PaidTime == null && item.HIBenefit > 0))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void RecalServiceAndPCLWithoutHI()
        {
            if (CurrentRegistration == null)
            {
                return;
            }

            if (CurrentRegistration.PatientRegistrationDetails != null)
            {
                foreach (var regDetail in CurrentRegistration.PatientRegistrationDetails)
                {
                    if (regDetail.PaidTime == null && regDetail.HIBenefit > 0)
                    {
                        regDetail.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime());
                        regDetail.GetItemTotalPrice();
                    }
                }
            }

            if (CurrentRegistration.PCLRequests != null)
            {
                foreach (var request in CurrentRegistration.PCLRequests)
                {
                    if (request.RecordState == RecordState.DELETED || request.PatientPCLRequestIndicators == null)
                    {
                        continue;
                    }

                    foreach (PatientPCLRequestDetail pclDetail in request.PatientPCLRequestIndicators)
                    {
                        if (pclDetail.PaidTime == null && pclDetail.HIBenefit > 0)
                        {
                            pclDetail.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime());
                            pclDetail.GetItemTotalPrice();
                        }
                    }
                }
            }

            TinhTongGiaTien();

            //Refresh để tính lại tổng tiền trên từng phiếu (12/11/2014 14:38).

            if (NewServiceContent != null && NewServiceContent.RegistrationDetails != null)
            {
                NewServiceContent.CV_RegDetailItems.Refresh();
            }

            if (NewPclContent != null && NewPclContent.PtPclReqDetailItems != null)
            {
                //NewPclContent.PtPclReqDetailItems.Refresh();
            }


        }


        WarningWithConfirmMsgBoxTask warnOfHIExpired = null;

        public IEnumerator<IResult> PayForNewService(bool RegisAndPay)
        {
            if (CurrentRegistration == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0380_G1_Msg_InfoChuaChonDK));
                yield break;
            }
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "trả tiền bill"))
            {
                yield break;
            }

            if (CheckHIExpired())
            {
                if (CheckHIServiceAndPCLNotPaidYet())
                {
                    string msg = string.Format(eHCMSResources.Z1127_G1_TheBHHetHan, CurrentRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault().ToShortDateString());
                    warnOfHIExpired = new WarningWithConfirmMsgBoxTask(msg, eHCMSResources.Z1167_G1_KgTinhBH);
                    yield return warnOfHIExpired;
                    if (warnOfHIExpired.IsAccept)
                    {
                        RecalServiceAndPCLWithoutHI();

                        yield break;
                    }
                }
            }


            //Phải kiểm tra có dịch vụ nào đã trả tiền rồi và sau đó bị xóa mà chưa hoàn tiền cho bệnh nhân
            //thì yêu cầu phải hoàn tiền cho bệnh nhân trước rồi mới tính.
            bool bMustRefund = false;
            if (_currentRegistration.PatientRegistrationDetails != null)
            {
                if (_currentRegistration.PatientRegistrationDetails.Any(item => item.PaidTime != null && item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                && item.RefundTime == null))
                {
                    bMustRefund = true;
                }
            }
            if (!bMustRefund)
            {
                if (_currentRegistration.PCLRequests != null)
                {
                    foreach (var request in _currentRegistration.PCLRequests)
                    {
                        if (request.PaidTime != null && request.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL //&& request.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                            && request.RefundTime == null)
                        {
                            bMustRefund = true;
                            break;
                        }
                        if (request.PatientPCLRequestIndicators != null)
                        {
                            if (request.PatientPCLRequestIndicators.Any(item => item.PaidTime != null && item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                && item.RefundTime == null))
                            {
                                bMustRefund = true;
                            }
                        }
                        if (bMustRefund)
                        {
                            break;
                        }
                    }
                }
            }
            if (bMustRefund && !Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance)
            {
                MessageBox.Show(eHCMSResources.A0456_G1_Msg_InfoHTienTruocTToan);
                yield break;
            }
            /////////////////////////////////////////////////////////////
            List<PatientRegistrationDetail> lsRegDetails = null;
            List<PatientPCLRequest> lsPclRequests = null;
            List<InPatientBillingInvoice> BillingInvoiceCollection = null;
            if (_currentRegistration.PatientRegistrationDetails != null)
            {
                //20200222 TBL Mod TMV1: Thêm điều kiện item.PaidTimeTmp != null để đem những dịch vụ được lấy từ gói đã trả tiền đi đăng ký
                //lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.IsChecked && item.PaidTime == null &&
                //                                                            (item.RecordState == RecordState.UNCHANGED
                //                                                            || item.RecordState == RecordState.ADDED
                //                                                            || item.RecordState == RecordState.MODIFIED)).ToList();
                lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.IsChecked && (item.PaidTime == null || item.PaidTimeTmp != null) &&
                                                                            (item.RecordState == RecordState.UNCHANGED
                                                                            || item.RecordState == RecordState.ADDED
                                                                            || item.RecordState == RecordState.MODIFIED)).ToList();
            }
            if (_currentRegistration.PCLRequests != null)
            {
                lsPclRequests = _currentRegistration.PCLRequests.Where(item => item.IsChecked && item.PaidTime == null
                && (item.RecordState == RecordState.UNCHANGED
                || item.RecordState == RecordState.MODIFIED)).ToList();
            }
            if (CurrentRegistration.InPatientBillingInvoices != null)
            {
                BillingInvoiceCollection = CurrentRegistration.InPatientBillingInvoices.Where(x => x.PaidTime == null && x.V_InPatientBillingInvStatus != AllLookupValues.V_InPatientBillingInvStatus.NGUNG_TRA_TIEN_LAI).ToList();
            }
            if ((lsRegDetails == null || lsRegDetails.Count == 0)
                && (lsPclRequests == null || lsPclRequests.Count == 0)
                && (BillingInvoiceCollection == null || BillingInvoiceCollection.Count == 0))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0900_G1_Msg_InfoChonDVDeTinhTien));
                yield break;
            }
            string mErrorMessage;
            bool bValidRegisToPay = Globals.CheckValidRegistrationForPay(_currentRegistration, out mErrorMessage);
            if (bValidRegisToPay)
            {
                Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                {
                    vm.Registration = _currentRegistration;
                    vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY;
                    vm.FormMode = PaymentFormMode.PAY;
                    vm.PayNewService = true;
                    vm.IsSaveRegisDetailsThenPay = false;
                    vm.IsProcess = IsProcess;
                    vm.RegistrationDetails = lsRegDetails;
                    vm.PclRequests = lsPclRequests;

                    vm.StartCalculating();
                };
                GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
            }
            else
            {
                if (mErrorMessage != null)
                {
                    MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            }
        }


        public bool CanPayForNewServiceCmd
        {
            get { return !RegistrationInfoHasChanged; }
        }

        /// <summary>
        /// Bỏ qua những thay đổi bên những dịch vụ chưa tính tiền.
        /// </summary>
        public void CancelAddingServiceAndPCLCmd()
        {
            CancelEdit();
        }
        public bool CanCancelAddingServiceAndPCLCmd
        {
            get { return RegistrationInfoHasChanged; }
        }

        public bool CanNewCount15HIPercentCmd
        {
            get { return !RegistrationInfoHasChanged && CurrentRegistration != null && CurrentRegistration.PtRegistrationID > 0 && IsShowCount15HIPercentCmd; }
        }

        public void PrintNewServiceCmd()
        {
            Coroutine.BeginExecute(DoPrintNewService(CurrentRegistration));
        }
        public bool CanPrintNewServiceCmd
        {
            get { return true; }
        }
        #endregion

        #region NHỮNG DỊCH VỤ ĐÃ TÍNH TIỀN
        /// <summary>
        /// Chuyển sang trạng thái sửa đăng ký (xóa dịch vụ)
        /// </summary>
        public void StartEditRegistrationCmd()
        {
            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "chỉnh sửa bill"))
            {
                if (!Globals.ServerConfigSection.InRegisElements.ShowMessageBoxForLockReportedRegistration)
                {
                    MessageBox.Show(eHCMSResources.Z2197_G1_CaDieuTriDaDuocBCBHYT, eHCMSResources.G0442_G1_TBao);
                }
                return;
            }
            BeginEdit();
        }
        private bool _canStartEditRegistrationEx = true;
        public bool CanStartEditRegistrationEx
        {
            get
            {
                return _canStartEditRegistrationEx;
            }
            set
            {
                _canStartEditRegistrationEx = value;
                NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
            }
        }
        public bool CanStartEditRegistrationCmd
        {
            get
            {
                return !IsInEditMode && CanStartEditRegistrationEx;
            }
        }

        /// <summary>
        /// Cập nhật đăng ký cũ (sau khi xóa dịch vụ đã trả tiền)
        /// </summary>
        public void UpdateRegistrationCmd()
        {
            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "lưu cập nhật bill"))
            {
                if (!Globals.ServerConfigSection.InRegisElements.ShowMessageBoxForLockReportedRegistration)
                {
                    MessageBox.Show(eHCMSResources.Z2197_G1_CaDieuTriDaDuocBCBHYT, eHCMSResources.G0442_G1_TBao);
                }
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            Coroutine.BeginExecute(DoSaveOldServices(), null, (o, e) =>
            {

                this.HideBusyIndicator();
            });
        }

        public bool CanUpdateRegistrationCmd
        {
            get { return RegistrationInfoHasChanged; }
        }

        /// <summary>
        /// Lưu và tính tiền cho đăng ký cũ (sau khi xóa dịch vụ đã trả tiền)
        /// </summary>
        public void SaveAndPayForOldServiceCmd()
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "lưu và trả tiền bill cũ"))
            {
                if (!Globals.ServerConfigSection.InRegisElements.ShowMessageBoxForLockReportedRegistration)
                {
                    MessageBox.Show(eHCMSResources.Z2197_G1_CaDieuTriDaDuocBCBHYT, eHCMSResources.G0442_G1_TBao);
                }
                return;
            }

            this.ShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            Coroutine.BeginExecute(DoSaveAndPayOldServices(), null, (o, e) =>
            {

                this.HideBusyIndicator();
            });
        }
        public bool CanSaveAndPayForOldServiceCmd
        {
            get { return RegistrationInfoHasChanged; }
        }
        public bool CanbtnCancelService
        {
            get { return RegistrationInfoHasChanged; }
        }

        /// <summary>
        /// Trả tiền cho những dịch vụ cũ (trả tiền chưa hết, hoàn tiền).
        /// Hiển thị tất cả những chi tiết đăng ký (dịch vụ, CLS, thuốc, y dụng cụ, hóa chất)
        /// </summary>
        public void PayForOldServiceCmd()
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "trả tiền bill cũ"))
            {
                if (!Globals.ServerConfigSection.InRegisElements.ShowMessageBoxForLockReportedRegistration)
                {
                    MessageBox.Show(eHCMSResources.Z2197_G1_CaDieuTriDaDuocBCBHYT, eHCMSResources.G0442_G1_TBao);
                }
                return;
            }
            //Neu co dich vu nao o trang thai hoan tien ma chua hoan tien thi hoan tien luon.
            string mErrorMessage;
            if (Globals.CheckValidRegistrationForPay(_currentRegistration, out mErrorMessage))
            {
                Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                {
                    vm.Registration = CurrentRegistration.DeepCopy();
                    vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY;
                    vm.FormMode = PaymentFormMode.PAY;
                    vm.PayNewService = false;
                    vm.Refundable = true;
                    if (_currentRegistration == null)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0380_G1_Msg_InfoChuaChonDK));
                        return;
                    }
                    if (_currentRegistration.PtRegistrationID == 0)
                    {
                        MessageBox.Show(string.Format("{0}.", eHCMSResources.A0713_G1_Msg_InfoKhTheTinhTienDKNay));
                        return;
                    }
                    List<PatientRegistrationDetail> lsRegDetails = null;
                    var lsPclRequests = new List<PatientPCLRequest>();
                    if (_currentRegistration.PatientRegistrationDetails != null)
                    {

                        lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.PaidTime != null && item.RefundTime == null
                            && item.RecordState == RecordState.UNCHANGED).ToList();
                    }
                    if (_currentRegistration.PCLRequests != null)
                    {
                        foreach (var request in _currentRegistration.PCLRequests)
                        {
                            //Neu co 1 request detail bi delete va chua hoan tien thi add nguyen cai request nay luon.
                            if (request.PatientPCLRequestIndicators != null)
                            {
                                if (request.PaidTime != null && request.RefundTime == null &&
                                    request.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL
                                        //request.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                        && request.RecordState == RecordState.UNCHANGED)
                                {
                                    lsPclRequests.Add(request);
                                    continue;
                                }

                                if (request.PatientPCLRequestIndicators.Any(requestDetail => requestDetail.PaidTime != null && requestDetail.RefundTime == null &&
                                                                                             requestDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                             && requestDetail.RecordState == RecordState.UNCHANGED))
                                {
                                    lsPclRequests.Add(request);
                                }
                            }
                        }
                    }

                    vm.RegistrationDetails = lsRegDetails.DeepCopy();
                    vm.PclRequests = lsPclRequests;

                    vm.StartCalculating();
                };
                GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
            }
            else
            {
                //▼====== #001
                if (mErrorMessage != null)
                {
                    MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                //▲====== #001
            }
        }
        public bool CanPayForOldServiceCmd
        {
            get { return !RegistrationInfoHasChanged; }
        }

        /// <summary>
        /// Bỏ qua những thay đổi sau khi xóa dịch vụ.
        /// </summary>
        public void CancelChangesOnRegistrationCmd()
        {
            CancelEdit();
        }
        public bool CanCancelChangesOnRegistrationCmd
        {
            get { return RegistrationInfoHasChanged; }
        }
        public void PrintOldServiceCmd()
        {
            Coroutine.BeginExecute(DoPrintOldService(CurrentRegistration));
        }
        public bool CanPrintOldServiceCmd
        {
            get { return true; }
        }
        public void Reset()
        {
            if (_isInEditMode)
            {
                CancelEdit();
            }
            if (_currentView != null)
            {
                _currentView.ResetView();
            }
        }
        private int _currentTabIndex = 0;
        public void RegInfoTabsChanged(object source, object eventArgs)
        {
            var tabCtrl = source as TabControl;
            int destTabIndex = tabCtrl.SelectedIndex;
            if (_currentView != null && RegistrationInfoHasChanged)
            {
                bool wannaChange = false;
                if (destTabIndex != _currentTabIndex)
                {
                    //Hoi co muon chuyen tab khong. Muon chuyen thi cancel edit roi chuyen.
                    MessageBoxResult result = MessageBox.Show(eHCMSResources.Z0446_G1_TTinDaThayDoi + ". " + eHCMSResources.A0138_G1_Msg_ConfBoQua, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel);
                    if (result == MessageBoxResult.OK)
                    {
                        wannaChange = true;
                    }
                }

                if (wannaChange)
                {
                    CancelEdit();
                    _currentTabIndex = destTabIndex;
                    NotifyOfPropertyChange(() => CanAddService);
                    NotifyOfPropertyChange(() => CanPaidService);
                }
                else
                {
                    tabCtrl.SelectedIndex = _currentTabIndex;
                }
            }
            else
            {
                _currentTabIndex = destTabIndex;
                NotifyOfPropertyChange(() => CanAddService);
                NotifyOfPropertyChange(() => CanPaidService);
            }
        }
        TabControl TCRegistrationInfo { get; set; }
        public void TCRegistrationInfo_Loaded(object sender, RoutedEventArgs e)
        {
            TCRegistrationInfo = sender as TabControl;
            if (IsFinalization)
            {
                //TCRegistrationInfo = sender as TabControl;
                TabItem TIOldItems = TCRegistrationInfo.Items.Cast<TabItem>().Where(x => x.Name == "tabitemOldItems").FirstOrDefault();
                if (TIOldItems != null)
                {
                    TCRegistrationInfo.SelectedItem = TIOldItems;
                }
            }
        }

        //▼====== #005
        //20190222 TTM: Do sử dụng các tab items để hiển thị số tiền cần biết của bệnh nhân, nên phải hard code chỗ này để ko phải nhảy sang các tab rỗng đó khi sử dụng Hot key chuyển tab.
        private int HardCodeTabItems = 2;
        public void NextTab(int aIndex = 1)
        {
            if (aIndex > 0 && TCRegistrationInfo.SelectedIndex + 1 < HardCodeTabItems)
            {
                TCRegistrationInfo.SelectedItem = TCRegistrationInfo.Items[TCRegistrationInfo.SelectedIndex + 1];
            }
            else if (aIndex < 0 && TCRegistrationInfo.SelectedIndex > 0)
            {
                TCRegistrationInfo.SelectedItem = TCRegistrationInfo.Items[TCRegistrationInfo.SelectedIndex - 1];
            }
            TCRegistrationInfo.Focus();
        }
        public override void HandleHotKey_Action_New(object sender, LocalHotKeyEventArgs e)
        {
            foreach (var inputBindingCommand in ListInputBindingCmds)
            {
                if (inputBindingCommand.HotKey_Registered_Name == e.HotKey.Name)
                {
                    inputBindingCommand._executeDelegate.Invoke(this);
                    break;
                }
            }
        }
        protected override IEnumerable<InputBindingCommand> GetInputBindingCommands()
        {
            yield return new InputBindingCommand(() => NextTab(1))
            {
                HotKey_Registered_Name = "ghkNextTab1",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.N
            };
            yield return new InputBindingCommand(() => NextTab(-1))
            {
                HotKey_Registered_Name = "ghkPrevTab1",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.P
            };
            yield return new InputBindingCommand(() => SaveAndPay())
            {
                HotKey_Registered_Name = "ghkSaveAndPayServiceNewAndOld",
                GestureModifier = ModifierKeys.Control,
                GestureKey = (Key)Keys.S
            };
        }
        private void SaveAndPay()
        {
            if (CurrentRegistration != null)
            {
                if (TCRegistrationInfo.SelectedIndex == 0)
                {
                    SaveAndPayForNewServiceCmd(true);
                }
                else if (TCRegistrationInfo.SelectedIndex == 1)
                {
                    SaveAndPayForOldServiceCmd();
                }
            }
        }
        //▲====== #005
        #endregion

        #endregion

        #region COROUTINES

        private IEnumerator<IResult> DoRemovePatientRegistrationDetails(PatientRegistrationDetail details)
        {
            if (!IsInEditMode)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1181_G1_ChuyenSangCheDoChSua, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
            if (details.RecordState == RecordState.ADDED || details.RecordState == RecordState.DETACHED)
            {
                CurrentRegistration.PatientRegistrationDetails.Remove(details);
                //▼===== #015
                ResetEkip(details);
                //▲===== #015
                RefreshRegistrationDetails();
                //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
                CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);
                OnDetailsChanged();
                InitViewForServiceItems();
                //--Đỉnh-- kiểm tra nếu xóa dịch vụ bảo hiểm chi trả, phải tính lại
                //if (details.TotalHIPayment > 0 && details.PtRegDetailID < 1)
                //{
                //    RefreshRegistrationDetails();
                //    CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
                //}
                yield break;
            }
            //Kiem tra neu item DV chua tra tien thi hoi co muon xoa hay khong.
            //Neu dich vu da duoc tra tien va da su dung roi => hoi lai.

            if (details.PaidTime == null)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1183_G1_CoMuonXoaDVNayKg, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                if (_msgTask.Result == AxMessageBoxResult.Ok)
                {
                    //▼===== #015
                    ResetEkip(details);
                    //▲===== #015
                    details.RecordState = RecordState.DELETED;
                    CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);
                    OnDetailsChanged();
                    InitViewForServiceItems();
                }
                yield break;
            }
            //Da tra tien roi. && dang thuc hien.
            if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN || details.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1185_G1_DVDaSDKgTheXoa, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
            _msgTask = new MessageBoxTask(eHCMSResources.Z1186_G1_DVDaTinhTien, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return _msgTask;
            if (_msgTask.Result == AxMessageBoxResult.Ok)
            {
                details.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                details.CanDelete = false;
                details.RecordState = RecordState.MODIFIED;
                //▼===== #015
                ResetEkip(details);
                //▲===== #015
                CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);
                OnDetailsChanged();
                InitViewForServiceItems();
                //--Đỉnh-- kiểm tra nếu xóa dịch vụ bảo hiểm chi trả, phải tính lại
                //if (details.TotalHIPayment > 0 && details.PtRegDetailID < 1)
                //{
                //    RefreshRegistrationDetails();
                //    CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
                //}
            }

            yield break;
        }

        private void RemoveEmptyPclRequest(PatientPCLRequest request)
        {
            if (request.PatientPCLRequestIndicators == null
                    || request.PatientPCLRequestIndicators.Count(item => item.RecordState != RecordState.DELETED) == 0)
            {
                request.RecordState = RecordState.DELETED;
            }
        }

        private bool CheckEmptyPclRequest(PatientPCLRequest request)
        {
            if (request.PatientPCLRequestIndicators == null
                    || request.PatientPCLRequestIndicators.Count(item => item.RecordState != RecordState.DELETED
                    && item.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI) == 0)
            {
                return true;
            }
            return false;
        }

        private IEnumerator<IResult> DoRemovePatientPCLRequestDetails(PatientPCLRequestDetail details)
        {
            if (!IsInEditMode)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1181_G1_ChuyenSangCheDoChSua, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
            if (details.RecordState == RecordState.ADDED || details.RecordState == RecordState.DETACHED)
            {
                details.PatientPCLRequest.PatientPCLRequestIndicators.Remove(details);
                if (details.PatientPCLRequest.RecordState == RecordState.DETACHED)
                {
                    if (details.PatientPCLRequest.PatientPCLRequestIndicators.Count == 0)
                    {
                        CurrentRegistration.PCLRequests.Remove(details.PatientPCLRequest);
                    }
                }
                else
                {
                    RemoveEmptyPclRequest(details.PatientPCLRequest);
                }

                //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
                CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);

                OnDetailsChanged();
                InitViewForPCLRequests();
                yield break;
            }
            //Kiem tra neu item DV chua tra tien thi hoi co muon xoa hay khong.
            //Neu dich vu da duoc tra tien va da su dung roi => hoi lai.

            if (details.PaidTime == null)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1187_G1_CoMuonXoaDVCLSNay, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                if (_msgTask.Result == AxMessageBoxResult.Ok)
                {
                    details.PatientPCLRequest.RecordState = RecordState.MODIFIED;
                    details.RecordState = RecordState.DELETED;
                    //▼===== #014
                    details.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                    //▲===== #014
                    RemoveEmptyPclRequest(details.PatientPCLRequest);
                    OnDetailsChanged();
                    InitViewForPCLRequests();
                }
                yield break;
            }
            //Da tra tien roi. && dang thuc hien.
            if (details.ExamRegStatus == AllLookupValues.ExamRegStatus.BAT_DAU_THUC_HIEN || details.ExamRegStatus == AllLookupValues.ExamRegStatus.HOAN_TAT)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1189_G1_DVCLSDaSD, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
            _msgTask = new MessageBoxTask(eHCMSResources.Z1190_G1_DVCLSDaTinhTien, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return _msgTask;
            if (_msgTask.Result == AxMessageBoxResult.Ok)
            {
                details.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                details.CanDelete = false;
                details.RecordState = RecordState.MODIFIED;
                details.PatientPCLRequest.RecordState = RecordState.MODIFIED;
                RemoveEmptyPclRequest(details.PatientPCLRequest);
                if (CheckEmptyPclRequest(details.PatientPCLRequest))
                {
                    var _msgAnnounce = new MessageBoxTask(string.Format("{0}!", eHCMSResources.Z1191_G1_PhYCCLSKgConDVNao), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return _msgAnnounce;
                    Coroutine.BeginExecute(DoRemoveEmptyPatientPCLRequest(details.PatientPCLRequest), null, (o, e) =>
                    {

                    });
                }
                OnDetailsChanged();
                InitViewForPCLRequests();
            }
            yield break;
        }

        private IEnumerator<IResult> DoRemovePatientPCLRequest(PatientPCLRequest request)
        {
            if (!IsInEditMode)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1181_G1_ChuyenSangCheDoChSua, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
            if (request.RecordState == RecordState.ADDED || request.RecordState == RecordState.DETACHED)
            {
                CurrentRegistration.PCLRequests.Remove(request);
                OnDetailsChanged();
                InitViewForPCLRequests();
                yield break;
            }

            if (request.PaidTime != null)
            {
                if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Count > 0)
                {
                    _msgTask = new MessageBoxTask(eHCMSResources.Z1193_G1_YCCLSDaTToan, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    if (_msgTask.Result == AxMessageBoxResult.Ok)
                    {
                        //request.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        request.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CANCEL;
                        request.RecordState = RecordState.MODIFIED;
                        foreach (var regDetails in request.PatientPCLRequestIndicators)
                        {
                            //Neu item nao da tra tien roi thi thoi tien lai. Neu item nao chua tra tien thi mark delete.
                            if (regDetails.PaidTime.HasValue)
                            {
                                regDetails.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                                regDetails.CanDelete = false;
                                regDetails.RecordState = RecordState.MODIFIED;
                                OnDetailsChanged();
                                InitViewForPCLRequests();
                            }
                            else
                            {
                                regDetails.RecordState = RecordState.DELETED;
                                OnDetailsChanged();
                                InitViewForPCLRequests();
                            }
                        }
                        request.CanDelete = false;
                    }
                }
                else
                {
                    request.RecordState = RecordState.DELETED;
                    OnDetailsChanged();
                    InitViewForPCLRequests();
                }
            }
            else
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1194_G1_XoaPhYCCLS, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                if (_msgTask.Result == AxMessageBoxResult.Ok)
                {
                    request.RecordState = RecordState.DELETED;
                    foreach (var regDetails in request.PatientPCLRequestIndicators)
                    {
                        regDetails.RecordState = RecordState.DELETED;
                        OnDetailsChanged();
                        InitViewForPCLRequests();
                        regDetails.CanDelete = false;
                    }
                    request.CanDelete = false;
                }
            }

            yield break;
        }
        private IEnumerator<IResult> DoRemoveEmptyPatientPCLRequest(PatientPCLRequest request)
        {
            if (request.PatientPCLRequestIndicators != null && request.PatientPCLRequestIndicators.Count > 0)
            {
                request.V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.CANCEL;
                request.RecordState = RecordState.MODIFIED;
                foreach (var regDetails in request.PatientPCLRequestIndicators)
                {
                    //Neu item nao da tra tien roi thi thoi tien lai. Neu item nao chua tra tien thi mark delete.
                    if (regDetails.PaidTime.HasValue)
                    {
                        regDetails.ExamRegStatus = AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI;
                        regDetails.CanDelete = false;
                        regDetails.RecordState = RecordState.MODIFIED;
                        OnDetailsChanged();
                        InitViewForPCLRequests();
                    }
                    else
                    {
                        regDetails.RecordState = RecordState.DELETED;
                        OnDetailsChanged();
                        InitViewForPCLRequests();
                    }
                }
                request.CanDelete = false;
            }
            yield break;
        }
        MessageBoxTask _msgTask;
        private IEnumerator<IResult> DoAddRegItem(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, long? ConsultationRoomStaffAllocID = null,
                                                    DateTime? ApptStartDate = null, DateTime? ApptEndDate = null, Staff staff = null
            , DateTime? dt = null, string diagnosis = null, bool isPriority = false
            , decimal Qty = 1)
        {
            if (_currentRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1195_G1_DKDaBiHuy, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }
            if (serviceItem.RefMedicalServiceType.V_RefMedicalServiceTypes != (long)AllLookupValues.V_RefMedicalServiceTypes.CANLAMSANG)
            {
                PatientRegistrationDetail existingDetails = _currentRegistration.PatientRegistrationDetails.FirstOrDefault(d => d.RecordState != RecordState.DELETED && d.RefMedicalServiceItem.Equals(serviceItem));//&& d.PaidTime == null:co hay chua tra tien deu bao
                if (existingDetails != null)
                {
                    if (AllowDuplicateMedicalServiceItems == 1)
                    {
                        _msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1197_G1_DV0DaCo, existingDetails.RefMedicalServiceItem.MedServiceName), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                        yield return _msgTask;
                        if (_msgTask.Result != AxMessageBoxResult.Ok)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1091_G1_DV0DaCoChonDVKhac, existingDetails.RefMedicalServiceItem.MedServiceName));
                        yield break;
                    }
                }

                AddNewRegistrationDetails(serviceItem, deptLoc, serviceType, ConsultationRoomStaffAllocID, ApptStartDate
                    , ApptEndDate, staff, dt, diagnosis, isPriority
                    , Qty);

            }
            else
            {
                bool exists = _currentRegistration.PCLRequests.Any(item => item.RecordState != RecordState.DELETED && item.PtRegDetailID.HasValue && item.PtRegDetailID.Value == serviceItem.MedServiceID);//&& item.PaidTime == null
                if (exists)
                {
                    if (AllowDuplicateMedicalServiceItems == 1)
                    {
                        _msgTask = new MessageBoxTask(eHCMSResources.Z1198_G1_DVDaCo, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                        yield return _msgTask;
                        if (_msgTask.Result != AxMessageBoxResult.Ok)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1091_G1_DV0DaCoChonDVKhac, ""));
                        yield break;
                    }
                }

                //Kiem tra nhu vay moi dung????da thuc hien roi co thong bao hay ko ta?
                var listchuathuchien = (from c in _currentRegistration.PCLRequests
                                        where c.RecordState != RecordState.DELETED //c.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN &&
                                        select c);

                string Results = "";

                long? deptLocID = null;
                if (Globals.DeptLocation != null)
                {
                    deptLocID = Globals.DeptLocation.DeptLocationID;
                }

                var defaultPclTask = new CreateDefaultPCLRequestTask(serviceItem.MedServiceID, null, deptLocID);
                yield return defaultPclTask;

                if (defaultPclTask.Error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorNotification { Message = eHCMSResources.Z1128_G1_LoiXayRaKhiTimDVCLS });
                    yield break;
                }

                if (defaultPclTask.PatientPCLRequest != null && defaultPclTask.PatientPCLRequest.PatientPCLRequestIndicators != null
                    && defaultPclTask.PatientPCLRequest.PatientPCLRequestIndicators.Count > 0)
                {
                    //kiem tra can lam sang so

                    PermissionManager.ApplyPermissionToPclRequest(defaultPclTask.PatientPCLRequest);

                    if (defaultPclTask.PatientPCLRequest.PatientPCLRequestIndicators != null)
                    {
                        foreach (var item in defaultPclTask.PatientPCLRequest.PatientPCLRequestIndicators)
                        {

                            foreach (var rq in listchuathuchien.SelectMany(detail => detail.PatientPCLRequestIndicators))
                            {
                                if (rq.PCLExamType.PCLExamTypeID == item.PCLExamType.PCLExamTypeID && rq.RecordState != RecordState.DELETED && rq.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    if (string.IsNullOrEmpty(Results))
                                    {
                                        Results = Results + " " + rq.PCLExamType.PCLExamTypeName;
                                    }
                                    else
                                    {
                                        Results = Results + ", " + rq.PCLExamType.PCLExamTypeName;
                                    }
                                    break;
                                }
                            }
                        }
                        //Kiem tra nhu vay moi dung
                        if (!string.IsNullOrEmpty(Results))
                        {
                            if (AllowDuplicateMedicalServiceItems == 1)
                            {
                                _msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1197_G1_DV0DaCo, Results.Trim()), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                                yield return _msgTask;
                                if (_msgTask.Result != AxMessageBoxResult.Ok)
                                {
                                    yield break;
                                }
                            }
                            else
                            {
                                MessageBox.Show(string.Format(eHCMSResources.Z1091_G1_DV0DaCoChonDVKhac, Results.Trim()));
                                yield break;
                            }
                        }

                        //Gan lai gia tien de hien thi
                        foreach (var item in defaultPclTask.PatientPCLRequest.PatientPCLRequestIndicators)
                        {
                            item.HIAllowedPrice = item.PCLExamType.HIAllowedPrice;
                            if (CurrentRegistration.HealthInsurance != null)
                            {
                                item.InvoicePrice = item.PCLExamType.HIPatientPrice;
                            }
                            else
                            {
                                item.InvoicePrice = item.PCLExamType.NormalPrice;
                            }
                            item.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime());
                            item.GetItemTotalPrice();
                            //▼====: #016
                            item.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
                            //▲====: #016
                        }
                    }
                    _currentRegistration.PCLRequests.Add(defaultPclTask.PatientPCLRequest);
                    InitViewForPCLRequests();
                    OnDetailsChanged();
                }
                else
                {
                    _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1199_G1_KgTimThayDVMacDinh), eHCMSResources.G0442_G1_TBao);
                    yield return _msgTask;
                }

                if (defaultPclTask.ExternalPclRequest != null)
                {
                    PermissionManager.ApplyPermissionToPclRequest(defaultPclTask.ExternalPclRequest);
                    if (defaultPclTask.ExternalPclRequest.PatientPCLRequestIndicators != null)
                    {
                        foreach (var item in defaultPclTask.ExternalPclRequest.PatientPCLRequestIndicators)
                        {
                            //▼====: #016
                            item.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
                            //▲====: #016
                            foreach (var rq in listchuathuchien.SelectMany(detail => detail.PatientPCLRequestIndicators))
                            {
                                if (rq.PCLExamType.PCLExamTypeID == item.PCLExamType.PCLExamTypeID && rq.RecordState != RecordState.DELETED && rq.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                                {
                                    if (string.IsNullOrEmpty(Results))
                                    {
                                        Results = Results + " " + rq.PCLExamType.PCLExamTypeName;
                                    }
                                    else
                                    {
                                        Results = Results + ", " + rq.PCLExamType.PCLExamTypeName;
                                    }
                                    break;
                                }
                            }
                        }
                        //Kiem tra nhu vay moi dung
                        if (!string.IsNullOrEmpty(Results))
                        {
                            if (AllowDuplicateMedicalServiceItems == 1)
                            {
                                _msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1197_G1_DV0DaCo, Results.Trim()), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                                yield return _msgTask;
                                if (_msgTask.Result != AxMessageBoxResult.Ok)
                                {
                                    yield break;
                                }
                            }
                            else
                            {
                                MessageBox.Show(string.Format(eHCMSResources.Z1091_G1_DV0DaCoChonDVKhac, Results.Trim()));
                                yield break;
                            }
                        }

                    }

                    _currentRegistration.PCLRequests.Add(defaultPclTask.ExternalPclRequest);
                    InitViewForPCLRequests();
                    OnDetailsChanged();
                }
                yield break;
            }
        }
        private PatientPCLRequestDetail CreatePatientPCLRequestDetail(PCLExamType examType, DeptLocation deptLoc, bool Priority = false)
        {
            var item = new PatientPCLRequestDetail
            {
                StaffID = Globals.LoggedUserAccount.StaffID,
                MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG,
                PCLExamType = examType,
                Qty = 1,
                DeptLocation = deptLoc == null ? null : deptLoc,
                HIAllowedPrice = examType.HIAllowedPrice,
                InvoicePrice = CurrentRegistration.HealthInsurance != null ? examType.HIPatientPrice : examType.NormalPrice,
                CreatedDate = Globals.GetCurServerDateTime(),
                //▼====: #016
                UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID,
                //▲====: #016
                //▼====: #024
                IsPriority = Priority,
                //▲====: #024
            };
            item.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime(), false, Globals.ServerConfigSection.HealthInsurances.FullHIBenefitForConfirm, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary);
            if (CurrentRegistration.Appointment != null &&
                CurrentRegistration.Appointment.ClientContract != null &&
                CurrentRegistration.Appointment.ClientContract.DiscountPercent > 0)
            {
                item.DiscountAmt = item.PatientPayment * CurrentRegistration.Appointment.ClientContract.DiscountPercent / 100;
                item.PatientPayment = item.PatientPayment - item.DiscountAmt;
            }
            item.GetItemTotalPrice();
            return item;
        }
        private PatientPCLRequest CreateTempPatientPCLRequest(Staff staff = null, DateTime? dt = null, string diagnosis = null)
        {
            //Tim xem co request moi nao chua. Neu chua co thi tao moi. Neu co roi thi thoi.
            PatientPCLRequest tempRequest = _currentRegistration.PCLRequests.Where(p => p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NGOAI_TRU
                                                                                                && p.RecordState == RecordState.DETACHED).FirstOrDefault();
            if (tempRequest == null)
            {
                tempRequest = new PatientPCLRequest
                {
                    PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(),
                    Diagnosis = diagnosis != null ? diagnosis : eHCMSResources.Z1116_G1_ChuaXacDinh,
                    StaffID = Globals.LoggedUserAccount.StaffID,
                    DoctorStaffID = staff != null ? staff.StaffID : 0,
                    MedicalInstructionDate = dt,
                    V_PCLRequestType = AllLookupValues.V_PCLRequestType.NGOAI_TRU,
                    V_PCLRequestStatus = AllLookupValues.V_PCLRequestStatus.OPEN,
                    RecordState = RecordState.DETACHED,
                    EntityState = EntityState.DETACHED
            };
                tempRequest.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;
                _currentRegistration.PCLRequests.Add(tempRequest);
            }
            return tempRequest;
        }

        private IEnumerator<IResult> DoAddPclExamType(PCLExamType examType, DeptLocation deptLoc, Staff staff = null, DateTime? dt = null, string diagnosis = null, bool isPriority = false)
        {
            if (_currentRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1200_G1_DKDaBiHuyKgTheThemCLS, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            var item = CreatePatientPCLRequestDetail(examType, deptLoc, isPriority);

            if (CurrentRegistration.PCLRequests == null)
            {
                CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
            }

            //Kiem tra nhu vay moi dung????da thuc hien roi co thong bao hay ko ta?
            var listchuathuchien = (from c in _currentRegistration.PCLRequests
                                    where c.RecordState != RecordState.DELETED //c.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN &&
                                    select c);

            if (checkExistService(listchuathuchien.ToList(), item))
            {
                if (AllowDuplicateMedicalServiceItems == 1)
                {
                    _msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1197_G1_DV0DaCo, item.PCLExamType.PCLExamTypeName.Trim()), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    if (_msgTask.Result != AxMessageBoxResult.Ok)
                    {
                        yield break;
                    }
                }
                else
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1091_G1_DV0DaCoChonDVKhac, item.PCLExamType.PCLExamTypeName.Trim()));
                    yield break;
                }
            }

            var tempRequest = CreateTempPatientPCLRequest(staff, dt, diagnosis);
            tempRequest.PatientPCLRequestIndicators.Add(item);

            //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
            CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);

            PermissionManager.ApplyPermissionToPclRequest(tempRequest);

            InitViewForPCLRequests();
            OnDetailsChanged();

            yield break;
        }

        private IEnumerator<IResult> DoAddAllPclExamType(ObservableCollection<PCLExamType> AllExamType, Staff staff = null, DateTime? dt = null, string diagnosis = null)
        {
            if (_currentRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1200_G1_DKDaBiHuyKgTheThemCLS, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            if (CurrentRegistration.PCLRequests == null)
            {
                CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
            }

            //Kiem tra nhu vay moi dung????da thuc hien roi co thong bao hay ko ta?
            var listchuathuchien = (from c in _currentRegistration.PCLRequests
                                    where c.RecordState != RecordState.DELETED //c.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN &&
                                    select c);
            string duplicateExamtype = "";
            var tempRequest = CreateTempPatientPCLRequest(staff, dt, diagnosis);
            List<PatientPCLRequestDetail> existPCLDetails = new List<PatientPCLRequestDetail>();
            foreach (var ExamType in AllExamType)
            {
                var item = CreatePatientPCLRequestDetail(ExamType, ExamType.ObjDeptLocationList.FirstOrDefault());

                if (checkExistService(listchuathuchien.ToList(), item))
                {
                    if (AllowDuplicateMedicalServiceItems == 1)
                    {
                        duplicateExamtype += "\n  - " + item.PCLExamType.PCLExamTypeName.Trim();
                        existPCLDetails.Add(item);
                    }
                }
                else
                {
                    tempRequest.PatientPCLRequestIndicators.Add(item);
                    ///PermissionManager.ApplyPermissionToPclRequest(tempRequest);
                }
            }
            //Kiem tra doi voi nhung thang da ton tai
            if (existPCLDetails.Count > 0)
            {
                if (AllowDuplicateMedicalServiceItems == 1)
                {
                    _msgTask = new MessageBoxTask(string.Format(eHCMSResources.Z1197_G1_DV0DaCo, duplicateExamtype), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    if (_msgTask.Result == AxMessageBoxResult.Ok)
                    {
                        foreach (var PCLDetail in existPCLDetails)
                        {
                            tempRequest.PatientPCLRequestIndicators.Add(PCLDetail);

                        }
                    }
                }
                else
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1091_G1_DV0DaCoChonDVKhac, duplicateExamtype));
                }
            }

            //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
            CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);

            PermissionManager.ApplyPermissionToPclRequest(tempRequest);
            InitViewForPCLRequests();
            OnDetailsChanged();
            yield break;
        }

        public bool checkExistService(List<PatientPCLRequest> listchuathuchien, PatientPCLRequestDetail item)
        {
            foreach (var rq in listchuathuchien.SelectMany(detail => detail.PatientPCLRequestIndicators))
            {
                if (rq.PCLExamType.PCLExamTypeID == item.PCLExamType.PCLExamTypeID && rq.RecordState != RecordState.DELETED && rq.V_ExamRegStatus != (long)AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI)
                {
                    //if (Convert.ToInt16(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowDuplicateMedicalServiceItems]) == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Kiểm tra hợp lệ khi cập nhật những service chưa tính tiền.
        /// </summary>
        /// <returns></returns>
        private IEnumerator<IResult> DoValidateNewServices(YieldValidationResult result)
        {
            result.IsValid = false;

            //kiem tra dang ky nay con ton tai hok hay da bi huy roi
            var res = new CheckRegistrationStatusTask(_currentRegistration.PtRegistrationID);
            yield return res;
            if (res.V_RegistrationStatus == (long)AllLookupValues.RegistrationStatus.REFUND)
            {
                var dialog = new MessageWarningShowDialogTask(string.Format("{0}!", eHCMSResources.Z1195_G1_DKDaBiHuy), eHCMSResources.G0442_G1_TBao, false);
                yield return dialog;
                yield break;
            }


            if (ValidateRegistration != null)
            {
                IEnumerator e = ValidateRegistration(_currentRegistration, result);

                while (e.MoveNext())
                    yield return e.Current as IResult;

                if (!result.IsValid)
                {
                    yield break;
                }
            }
            result.IsValid = false;
            //IList<PatientRegistrationDetail> RegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => (item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED || item.RecordState == RecordState.DELETED)).ToList();
            //if (RegDetails == null
            //    || RegDetails.Count < 1)
            //KMx: Nếu có 1 DV hoặc 1 PCL đã lưu rồi. Mà người dùng vẫn bấm lưu thì vẫn cho lưu. Trường hợp đã lưu DV A, sau đó thêm DV B và xóa DV B thì bấm lưu không được. Phải bấm nút "Bỏ qua" và chọn "Tính tiền". (03/04/2014 17:46)
            IList<PatientRegistrationDetail> RegDetails = _currentRegistration.PatientRegistrationDetails;
            if (RegDetails == null
                || RegDetails.Count == 0)
            {
                //Kiem tra co CLS khong. Neu cung khong co luon thi bao chua duoc
                //if (_currentRegistration.PCLRequests == null || _currentRegistration.PCLRequests.Count(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED || item.RecordState == RecordState.DELETED
                //                                                                                                 || item.RecordState == RecordState.MODIFIED) == 0)
                if (_currentRegistration.PCLRequests == null || _currentRegistration.PCLRequests.Count == 0)
                {
                    _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z0156_G1_Chon1DV), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    yield break;
                }
            }
            else
            {
                if (RegDetails.Where(o => (o.RefMedicalServiceItem.ServiceMainTime == (long)AllLookupValues.V_ServicePrice.Changeable
                    && o.InvoicePrice < 1000)).ToList().Count > 0)
                {
                    _msgTask = new MessageBoxTask(string.Format("{0}!", eHCMSResources.Z1201_G1_KgLuuDVCoGiaNhoHon1000), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.Ok);
                    yield return _msgTask;
                    yield break;
                }
            }

            if (CurrentRegistration.IsHIUnder15Percent.GetValueOrDefault(false) &&
                ((CurrentRegistration.PatientRegistrationDetails != null && CurrentRegistration.PatientRegistrationDetails.Any(x => x.RecordState == RecordState.ADDED))
                || (CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Any(x => x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Any(y => y.RecordState == RecordState.DETACHED)))))
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z2202_G1_KhongTheSuaDKMCCT, eHCMSResources.T0074_G1_I, aEMR.Infrastructure.MessageBoxOptions.Ok);
                yield return _msgTask;
                yield break;
            }

            result.IsValid = true;
        }
        private DiagnosisTreatment _CurrentDiagnosisTreatment;
        public DiagnosisTreatment CurrentDiagnosisTreatment
        {
            get
            {
                return _CurrentDiagnosisTreatment;
            }
            set
            {
                if (_CurrentDiagnosisTreatment == value)
                {
                    return;
                }
                _CurrentDiagnosisTreatment = value;
                NotifyOfPropertyChange(() => CurrentDiagnosisTreatment);
            }
        }
        public CallCloseDialog gCallCloseDialog { get; set; }
        private IEnumerator<IResult> DoSaveAndPayNewServices(bool IsPayAlso, bool IsFromRequestDoctor = false)
        {
            if (IsPayAlso && CurrentRegistration.HisID.GetValueOrDefault() == Globals.DefaultHisID_ForNewRegis_BeforeSaving)
            {
                CurrentRegistration.HisID = null;
                CurrentRegistration.PtInsuranceBenefit = null;
            }
            if (!Globals.CheckChildrenUnder6YearOlds(CurrentPatient))
            {
                yield break;
            }

            //kiem tra dang ky nay da dc dang ky bao hiem chua?neu roi thi ko cho dang ky bao hiem nua
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateNewServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;

            if (!result.IsValid)
            {
                yield break;
            }

            //▼===== 20200610 TTM: Kiểm tra QMS
            if (Globals.LoggedUserAccount.Staff.AllowRegWithoutTicket == false)
            {
                if (Globals.ServerConfigSection.CommonItems.UseQMSSystem
                && (TicketIssueObj == null || TicketIssueObj.TicketNumberSeq <= 0)
                && CurrentRegistration.PtRegistrationID == 0)
                {
                    //▼====: #018
                    if (Globals.ServerConfigSection.CommonItems.BlockRegNoTicket == 2)
                    {
                        string warning = string.Format("{0} ", "Bệnh nhân đang đăng ký không có thông tin số thứ tự");
                        warnConfDlg = new WarningWithConfirmMsgBoxTask(warning, "Xác Nhận");
                        yield return warnConfDlg;
                        if (!warnConfDlg.IsAccept)
                        {
                            yield break;
                        }
                    }
                    else if (Globals.ServerConfigSection.CommonItems.BlockRegNoTicket == 1)
                    {
                        MessageBox.Show("Bệnh nhân đang đăng ký không có thông tin số thứ tự, không thể đăng ký.");
                        yield break;
                    }
                    //▲====: #018
                }
            }

            string ErrStr = "";
            int WarningType = 0;
            if (Globals.ServerConfigSection.CommonItems.UseQMSSystem
                && Globals.ServerConfigSection.CommonItems.CheckPatientInfoQMSSystem
                && !Globals.CheckValidForTicketQMS(TicketIssueObj, CurrentRegistration, out ErrStr, out WarningType))
            {
                if (WarningType == 2)
                {
                    if (MessageBox.Show(string.Format(ErrStr), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        yield break;
                    }
                }
                else
                {
                    if (MessageBox.Show(string.Format(ErrStr), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        yield break;
                    }
                    IsDiffBetweenRegistrationAndTicket = true;
                }
            }
            //▲===== 


            //Se dang ky ngay o cho nay.
            var newServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED || (item.RecordState == RecordState.MODIFIED && item.PaidStaffID == 0)).ToList();
            if (_currentRegistration.PCLRequests == null)
            {
                _currentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
            }
            if (ViewCase == RegistrationViewCase.RegistrationRequestView && CurrentDiagnosisTreatment != null)
            {
                foreach (var item in CurrentRegistration.PCLRequests)
                {
                    if (item.PatientPCLReqID > 0)
                    {
                        continue;
                    }
                    item.Diagnosis = string.IsNullOrEmpty(item.Diagnosis) ? CurrentDiagnosisTreatment.DiagnosisFinal : item.Diagnosis;
                    item.PtRegDetailID = CurrentDiagnosisTreatment.PtRegDetailID;
                    item.MedicalInstructionDate = item.MedicalInstructionDate == null ? Globals.GetCurServerDateTime() : item.MedicalInstructionDate;
                }
            }
            var newPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();
            var deletedServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList();
            var deletedPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED).ToList();

            string mSaveError = string.Format("{0}.", eHCMSResources.Z1202_G1_KgCoThayDoi);
            if (IsPayAlso)
            {
                mSaveError = eHCMSResources.Z1203_G1_KgTheLuuVaTraTien;
            }
            if (newServiceList.Count == 0 && newPclRequestList.Count == 0 && deletedServiceList.Count == 0 && deletedPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(mSaveError, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }

            //kiem tra can lam sang so
            var listitems = newPclRequestList.SelectMany(x => x.PatientPCLRequestIndicators);
            foreach (var item in listitems)
            {
                //neu benh nhan duoc hen thi khong can kiem tra
                if (item.PCLExamType.ObjPCLExamTypeServiceTarget != null)
                {
                    //se kiem tra o day.......!!can lam sang so
                    var TargetTask = new PCLExamTypeServiceTarget_CheckedTask(item.PCLExamType.PCLExamTypeID, _currentRegistration.ExamDate);
                    yield return TargetTask;

                    if (TargetTask.Error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}!", eHCMSResources.Z1041_G1_LoiXayRaCLSSo) });
                        yield break;
                    }
                    else
                    {
                        if (!TargetTask.Result)
                        {
                            if (MessageBox.Show(string.Format(eHCMSResources.Z1393_G1_VuotChiTieu, item.PCLExamType.PCLExamTypeName), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }

            foreach (var request in newPclRequestList)
            {
                //kiem tra lai cho nay ne
                request.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                //kiem tra user co phai la bac si khong
                if (request.DoctorStaffID == null || request.DoctorStaffID < 1)
                {
                    request.DoctorStaffID = DoctorStaffID;
                }
            }

            //list request goi xuong la tat ca cac request chua tra tien, tra roi khong tinh toi khong lam gi nua
            var listPCLRequest = (from c in _currentRegistration.PCLRequests
                                  where c.PaidTime == null
                                  select c);

            if (IsPayAlso)
            {
                // ================================ TxD 29/10/2018 BEGIN=========================
                // Reset HisID and HIBenefit back to NULL if they were SET previously by PatientRegistrationViewModel 
                // upon receiving Confirmed HIItem from the NEWLY way of Confirming HIBenefit NOT going via ReceivePatientViewModel 

                long defHisIDBefSaveRegis = Globals.DefaultHisID_ForNewRegis_BeforeSaving;
                newServiceList.ForEach(servItem =>
                {
                    if (servItem.HisID == defHisIDBefSaveRegis)
                    {
                        servItem.HisID = null;
                    }
                });

                foreach (var pclReq in newPclRequestList)
                {
                    foreach (var pclItem in pclReq.PatientPCLRequestIndicators)
                    {
                        if (pclItem.HisID == defHisIDBefSaveRegis)
                        {
                            pclItem.HisID = null;
                        }
                    }
                }

                if (CurrentRegistration.HisID == Globals.DefaultHisID_ForNewRegis_BeforeSaving)
                {
                    CurrentRegistration.HisID = null;
                    CurrentRegistration.PtInsuranceBenefit = null;
                }
                // ================================ TxD 29/10/2018 END ==========================
            }

            //Lưu xuống ở đây
            CurrentRegistration.BasicDiagTreatment = BasicDiagTreatment;
            //20190117 Gán giá trị cho Phiếu yêu cầu CLS từ quầy đăng ký dựa vào Chẩn đoán ban đầu.
            foreach (var x in listPCLRequest)
            {
                if (x.PatientPCLReqID > 0)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(BasicDiagTreatment) && BasicDiagTreatment.Length > 2)
                {
                    x.Diagnosis = BasicDiagTreatment;
                    x.DoctorStaffID = DoctorStaffID;
                }
            }

            foreach (PatientRegistrationDetail detail in newServiceList)
            {
                detail.ServiceRecID = ServiceRecID;
            }

            bool IsUpdateStatus = false;
            //20200430 TTM: set giá trị cho hàng đợi tại đây trước khi đi lưu.
            if (TicketIssueObj != null && TicketIssueObj.V_TicketStatus != (int)V_TicketStatus_Enum.TKT_ALREADY_REGIS
                && _currentRegistration.PtRegistrationID == 0)
            {
                _currentRegistration.TicketIssue = TicketIssueObj;
                IsUpdateStatus = true;
            }

            var regTask = new AddUpdateNewRegItemTask(_currentRegistration, newServiceList, listPCLRequest.ToList(), deletedServiceList, deletedPclRequestList, null
                , IsNotCheckInvalid, IsProcess, IsFromRequestDoctor);
            yield return regTask;
            IsNotCheckInvalid = false;
            if (!string.IsNullOrEmpty(regTask.ErrorMesage))
            {
                //20200604 TBL: BM 0038236: Khi lưu dịch vụ mà có lỗi thì tắt busy trước khi hiện thông báo lỗi vì nếu không tắt trước thì khi người dùng bấm dấu X chứ không bấm OK thì busy sẽ không được tắt
                if (!IsPayAlso)
                {
                    this.DlgHideBusyIndicator();
                }
                else
                {
                    this.HideBusyIndicator();
                }
                var message = new MessageWarningShowDialogTask(regTask.ErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = _currentRegistration });
                yield break;
            }
            if (regTask.Error != null)
            {
                //▼===== #009
                //Thong bao loi.
                //Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(regTask.Error) });
                //TBL: 19090601 là ID để xác định thông báo
                if (regTask.Error.Message.Contains("19090601") && MessageBox.Show(regTask.Error.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    //20191025 TBL: Nếu đồng ý lưu thì lưu lại và bỏ qua kiểm tra dưới store
                    IsNotCheckInvalid = true;
                    SaveNewServicesAndPclCmd();
                }
                else if (!regTask.Error.Message.Contains("19090601"))
                {
                    Globals.ShowMessage(regTask.Error.Message, eHCMSResources.G0442_G1_TBao);
                }
                //▲===== #009
                yield break;
            }
            //if (ViewCase == RegistrationViewCase.RegistrationRequestView)
            //{
            //    if (gCallCloseDialog != null)
            //    {
            //        gCallCloseDialog();
            //    }
            //    yield break;
            //}
            CurrentRegistration = regTask.CurRegistration;

            //20200430 TTM: Sau khi đã lưu giá trị hàng đợi xong sẽ cập nhật số TT đó sang tình trạng đã đăng ký => Chỉ làm khi cấu hình bật.
            if (Globals.ServerConfigSection.CommonItems.UseQMSSystem
                && IsUpdateStatus && CurrentRegistration.TicketIssue != null
                && Globals.ServerConfigSection.CommonItems.UpdateTicketStatusAfterRegister)
            {
                UpdateTicketStatusAfterRegister(CurrentRegistration.TicketIssue.TicketNumberText, CurrentRegistration.TicketIssue.TicketGetTime);
            }

            //Thanh cong roi.
            Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = regTask.CurRegistration, RefreshItemFromReturnedObj = true });
            if (Globals.ServerConfigSection.CommonItems.AllowFirstHIExaminationWithoutPay)
            {
                if (newServiceList != null && newServiceList.Count > 0 && regTask.CurRegistration.AllSaveRegistrationDetails != null)
                {
                    List<PatientRegistrationDetail> temp = new List<PatientRegistrationDetail>();
                    foreach (var tempDetail in regTask.CurRegistration.AllSaveRegistrationDetails)
                    {
                        foreach (var tempNewDetail in newServiceList)
                        {
                            if (tempDetail.MedServiceName == tempNewDetail.MedServiceName)
                            {
                                temp.Add(tempDetail.DeepCopy());
                            }
                        }
                    }
                    if (temp.Count > 0)
                    {
                        Globals.EventAggregator.Publish(new PhieuChiDinhForRegistrationCompleted() { Registration = regTask.CurRegistration, RegDetailsList = temp, PCLRequestList = new List<PatientPCLRequest>() });
                    }
                }
            }

            // VuTTM
            // QMS Service
            // Creating the registration and PCL request orders
            // QTD - Thêm cấu hình bật/tắt QMS
            if (Globals.ServerConfigSection.CommonItems.IsEnableCreateOrderFromAccountant)
            {
                bool isPaid = false;
                bool hasCashierOrder = true;
                bool canUpdateSeqNumber = true;
                IList<PatientRegistrationDetail> SavedRegistrationDetailList = _currentRegistration.PatientRegistrationDetails;
                IList<PatientPCLRequest> SavedPclRequestList = _currentRegistration.PCLRequests;
                GlobalsNAV.CreateOrders(isPaid, hasCashierOrder, CurrentPatient, ref SavedRegistrationDetailList,
                    ref SavedPclRequestList, canUpdateSeqNumber, deletedServiceList, deletedPclRequestList);
            }
            //▼===== 20200217 TTM: Không tính lại giá dịch vụ sau khi đã lưu vì trước khi lưu đã tính hết rồi, 
            //                     Lưu xong đọc lên không cần tính lại.
            //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
            //▲===== 
            if (IsPayAlso)
            {
                PayForNewServiceCmd(true);
            }
            else
            {
                BeginEdit();
            }
            //20181212 TTM: Bắn sự kiện để PatientRegistration gán CurRegistration cho PatientSummaryInfoV3.
            Globals.EventAggregator.Publish(new SetCurRegistrationForPatientSummaryInfoV3 { });
        }

        private IEnumerator<IResult> SaveAndPayForNewServicesAndPclReqs(bool IsHotKey = false)
        {
            if (CurrentRegistration == null)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0380_G1_Msg_InfoChuaChonDK));
                yield break;
            }
            //▼====: #008
            //TBL: Làm theo cách mới thì không cần phải set = null vì có thể hủy bỏ khi tính tiền
            //if (CurrentRegistration.HisID.GetValueOrDefault() == Globals.DefaultHisID_ForNewRegis_BeforeSaving && !IsHotKey)
            //{
            //    CurrentRegistration.HisID = null;
            //    CurrentRegistration.PtInsuranceBenefit = null;
            //}
            //▲====: #008
            if (!Globals.CheckChildrenUnder6YearOlds(CurrentPatient))
            {
                yield break;
            }

            //kiem tra dang ky nay da dc dang ky bao hiem chua?neu roi thi ko cho dang ky bao hiem nua
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateNewServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;

            if (!result.IsValid)
            {
                yield break;
            }

            //▼===== 20200610 TTM: Kiểm tra QMS
            if (Globals.LoggedUserAccount.Staff.AllowRegWithoutTicket == false)
            {
                if (Globals.ServerConfigSection.CommonItems.UseQMSSystem
                && (TicketIssueObj == null || TicketIssueObj.TicketNumberSeq <= 0)
                && CurrentRegistration.PtRegistrationID == 0)
                {
                    //▼====: #018
                    if (Globals.ServerConfigSection.CommonItems.BlockRegNoTicket == 2)
                    {
                        string warning = string.Format("{0} ", "Bệnh nhân đang đăng ký không có thông tin số thứ tự");
                        warnConfDlg = new WarningWithConfirmMsgBoxTask(warning, "Xác Nhận");
                        yield return warnConfDlg;
                        if (!warnConfDlg.IsAccept)
                        {
                            yield break;
                        }
                    }
                    else if (Globals.ServerConfigSection.CommonItems.BlockRegNoTicket == 1)
                    {
                        MessageBox.Show("Bệnh nhân đang đăng ký không có thông tin số thứ tự, không thể đăng ký.");
                        yield break;
                    }
                    //▲====: #018
                }
            }
            string ErrStr = "";
            int WarningType = 0;
            if (Globals.ServerConfigSection.CommonItems.UseQMSSystem
                && Globals.ServerConfigSection.CommonItems.CheckPatientInfoQMSSystem
                && !Globals.CheckValidForTicketQMS(TicketIssueObj, CurrentRegistration, out ErrStr, out WarningType))
            {
                if (WarningType == 2)
                {
                    if (MessageBox.Show(string.Format(ErrStr), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        yield break;
                    }
                }
                else
                {
                    if (MessageBox.Show(string.Format(ErrStr), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        yield break;
                    }
                    IsDiffBetweenRegistrationAndTicket = true;
                }
            }
            //▲===== 20200610 TTM: Kiểm tra QMS


            //20200530 TBL: Thêm trường hợp khi dịch vụ chỉ được lưu sau đó thay đổi giá thì dịch vụ đó đang có RecordState = MODIFIED
            //var newServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();
            var newServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED || (item.RecordState == RecordState.MODIFIED && item.PaidStaffID == 0)).ToList();

            var newPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var deletedServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList();

            var deletedPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED).ToList();

            string mSaveError = string.Format("{0}.", eHCMSResources.Z1203_G1_KgTheLuuVaTraTien);

            if (newServiceList.Count == 0 && newPclRequestList.Count == 0 && deletedServiceList.Count == 0 && deletedPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(mSaveError, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }

            //kiem tra can lam sang so
            var listitems = newPclRequestList.SelectMany(x => x.PatientPCLRequestIndicators);
            foreach (var item in listitems)
            {
                //neu benh nhan duoc hen thi khong can kiem tra
                if (item.PCLExamType.ObjPCLExamTypeServiceTarget != null)
                {
                    //se kiem tra o day.......!!can lam sang so
                    var TargetTask = new PCLExamTypeServiceTarget_CheckedTask(item.PCLExamType.PCLExamTypeID, _currentRegistration.ExamDate);
                    yield return TargetTask;

                    if (TargetTask.Error != null)
                    {
                        Globals.EventAggregator.Publish(new ErrorNotification { Message = string.Format("{0}!", eHCMSResources.Z1041_G1_LoiXayRaCLSSo) });
                        yield break;
                    }
                    else
                    {
                        if (!TargetTask.Result)
                        {
                            if (MessageBox.Show(string.Format(eHCMSResources.Z1393_G1_VuotChiTieu, item.PCLExamType.PCLExamTypeName), eHCMSResources.K1576_G1_CBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                            {
                                yield break;
                            }
                        }
                    }
                }
            }

            foreach (var request in newPclRequestList)
            {
                //kiem tra lai cho nay ne
                request.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                //kiem tra user co phai la bac si khong
                if (request.DoctorStaffID == null || request.DoctorStaffID < 1)
                {
                    request.DoctorStaffID = DoctorStaffID;
                }
            }

            //list request goi xuong la tat ca cac request chua tra tien, tra roi khong tinh toi khong lam gi nua
            var listPCLRequest = (from c in _currentRegistration.PCLRequests
                                  where c.PaidTime == null
                                  select c);

            // ================================ TxD 29/10/2018 BEGIN=========================
            // Reset HisID and HIBenefit back to NULL if they were SET previously by PatientRegistrationViewModel 
            // upon receiving Confirmed HIItem from the NEWLY way of Confirming HIBenefit NOT going via ReceivePatientViewModel 

            long defHisIDBefSaveRegis = Globals.DefaultHisID_ForNewRegis_BeforeSaving;
            newServiceList.ForEach(servItem =>
            {
                if (servItem.HisID == defHisIDBefSaveRegis)
                {
                    servItem.HisID = null;
                }
            });

            foreach (var pclReq in newPclRequestList)
            {
                foreach (var pclItem in pclReq.PatientPCLRequestIndicators)
                {
                    if (pclItem.HisID == defHisIDBefSaveRegis)
                    {
                        pclItem.HisID = null;
                    }
                }
            }
            //▼====: #008
            //TBL: Làm theo cách mới thì không cần phải set = null vì có thể hủy bỏ khi tính tiền
            //if (CurrentRegistration.HisID == Globals.DefaultHisID_ForNewRegis_BeforeSaving)
            //{
            //    CurrentRegistration.HisID = null;
            //    CurrentRegistration.PtInsuranceBenefit = null;
            //}
            //▲====: #008
            // ================================ TxD 29/10/2018 END ==========================



            CurrentRegistration.BasicDiagTreatment = BasicDiagTreatment;
            //20190117 Gán giá trị cho Phiếu yêu cầu CLS từ quầy đăng ký dựa vào Chẩn đoán ban đầu.
            foreach (var x in listPCLRequest)
            {
                if (x.PatientPCLReqID > 0)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(BasicDiagTreatment) && BasicDiagTreatment.Length > 2)
                {
                    x.Diagnosis = BasicDiagTreatment;
                    x.DoctorStaffID = DoctorStaffID;
                }
            }


            if (CheckHIExpired() && CheckHIServiceAndPCLNotPaidYet())
            {
                string msg = string.Format(eHCMSResources.Z1127_G1_TheBHHetHan, CurrentRegistration.HealthInsurance.ValidDateTo.GetValueOrDefault().ToShortDateString());
                warnOfHIExpired = new WarningWithConfirmMsgBoxTask(msg, eHCMSResources.Z1167_G1_KgTinhBH);
                yield return warnOfHIExpired;
                if (warnOfHIExpired.IsAccept)
                {
                    RecalServiceAndPCLWithoutHI();

                    yield break;
                }
            }


            //Phải kiểm tra có dịch vụ nào đã trả tiền rồi và sau đó bị xóa mà chưa hoàn tiền cho bệnh nhân
            //thì yêu cầu phải hoàn tiền cho bệnh nhân trước rồi mới tính.
            bool bMustRefund = false;
            if (_currentRegistration.PatientRegistrationDetails != null)
            {
                if (_currentRegistration.PatientRegistrationDetails.Any(item => item.PaidTime != null && item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                && item.RefundTime == null))
                {
                    bMustRefund = true;
                }
            }
            if (!bMustRefund)
            {
                if (_currentRegistration.PCLRequests != null)
                {
                    foreach (var request in _currentRegistration.PCLRequests)
                    {
                        if (request.PaidTime != null && request.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL //&& request.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                            && request.RefundTime == null)
                        {
                            bMustRefund = true;
                            break;
                        }
                        if (request.PatientPCLRequestIndicators != null)
                        {
                            if (request.PatientPCLRequestIndicators.Any(item => item.PaidTime != null && item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                && item.RefundTime == null))
                            {
                                bMustRefund = true;
                            }
                        }
                        if (bMustRefund)
                        {
                            break;
                        }
                    }
                }
            }
            if (bMustRefund && !Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance)
            {
                MessageBox.Show(eHCMSResources.A0456_G1_Msg_InfoHTienTruocTToan);
                yield break;
            }
            /////////////////////////////////////////////////////////////
            List<PatientRegistrationDetail> lsRegDetails = null;
            List<PatientPCLRequest> lsPclRequests = null;
            //if (_currentRegistration.PatientRegistrationDetails != null)
            //{
            //    lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.IsChecked && item.PaidTime == null &&
            //                                                                (item.RecordState == RecordState.UNCHANGED || item.RecordState == RecordState.ADDED)).ToList();
            //}
            //if (_currentRegistration.PCLRequests != null)
            //{
            //    lsPclRequests = _currentRegistration.PCLRequests.Where(item => item.IsChecked && item.PaidTime == null && item.RecordState == RecordState.UNCHANGED).ToList();
            //}

            //▼===== #012: Do các dịch vụ lưu trước khi apply thông tin miễn giảm hoặc bảo hiểm được set RecordState là Modified nên cần 
            //             loại là Modified để tính toán cho chính xác 
            if (_currentRegistration.PatientRegistrationDetails != null)
            {
                //20200222 TBL Mod TMV1: Thêm điều kiện item.PaidTimeTmp != null để đem những dịch vụ được lấy từ gói đã trả tiền đi đăng ký
                //lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.PaidTime == null &&
                //                                                            (item.RecordState == RecordState.UNCHANGED
                //                                                            || item.RecordState == RecordState.ADDED
                //                                                            || item.RecordState == RecordState.MODIFIED)).ToList();
                //▼===== 20200729 TTM: Bổ sung thêm trường hợp nếu dịch vụ là đã trả tiền bị thay đổi thông tin 
                //                     thì đem vào simplepay để tính toán lại cho chính xác số tiền cần phải trả.
                lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => (item.IsChecked && (item.PaidTime == null || item.PaidTimeTmp != null) &&
                                                                            (item.RecordState == RecordState.UNCHANGED
                                                                            || item.RecordState == RecordState.ADDED
                                                                            || item.RecordState == RecordState.MODIFIED))
                                                                            || (!item.IsChecked && item.PaidTime != null && item.RecordState == RecordState.MODIFIED)).ToList();
                //▲=====
            }
            if (_currentRegistration.PCLRequests != null)
            {
                lsPclRequests = _currentRegistration.PCLRequests.Where(item => item.PaidTime == null &&
                                                    (item.RecordState == RecordState.UNCHANGED || item.RecordState == RecordState.DETACHED || item.RecordState == RecordState.MODIFIED)).ToList();
            }
            //▲===== #012

            bool IsUpdateStatus = false;
            //20200430 TTM: set giá trị cho hàng đợi tại đây trước khi đi lưu.
            if (TicketIssueObj != null && TicketIssueObj.V_TicketStatus != (int)V_TicketStatus_Enum.TKT_ALREADY_REGIS
                && _currentRegistration.PtRegistrationID == 0)
            {
                _currentRegistration.TicketIssue = TicketIssueObj;
                IsUpdateStatus = true;
            }

            if ((lsRegDetails == null || lsRegDetails.Count == 0)
                && (lsPclRequests == null || lsPclRequests.Count == 0))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0900_G1_Msg_InfoChonDVDeTinhTien));
                yield break;
            }
            string mErrorMessage;
            bool bValidRegisForPay = Globals.CheckValidRegistrationForPay(_currentRegistration, out mErrorMessage);
            if (bValidRegisForPay)
            {

                Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                {
                    vm.Registration = _currentRegistration;
                    vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY;
                    vm.FormMode = PaymentFormMode.PAY;
                    vm.PayNewService = true;
                    vm.IsSaveRegisDetailsThenPay = true;
                    vm.IsProcess = IsProcess;
                    vm.RegistrationDetails = lsRegDetails;
                    vm.PclRequests = lsPclRequests;
                    vm.IsUpdateStatus = IsUpdateStatus;
                    if (TicketIssueObj != null && !string.IsNullOrEmpty(TicketIssueObj.TicketNumberText))
                    {
                        vm.TicketIssueObj = TicketIssueObj;
                    }
                    vm.StartCalculating();
                };
                GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);

            }
            else
            {
                if (mErrorMessage != null)
                {
                    MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            }

        }


        WaitForSaveRegistrationCompletedTask _waitForSaveCompletedTask;

        public IEnumerator<IResult> DoPrintNewService(PatientRegistration registrationInfo)
        {
            _msgTask = new MessageBoxTask(eHCMSResources.Z0368_G1_BanCoMuonInKg, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return _msgTask;
            if (_msgTask.Result == AxMessageBoxResult.Ok)
            {
                yield return Loader.Show("Đang in");

                //In Dich vu kham chua benh
                if (registrationInfo != null && registrationInfo.PtRegistrationID > 0 &&
                    registrationInfo.PatientRegistrationDetails != null)
                {
                    foreach (var regDetails in registrationInfo.PatientRegistrationDetails)
                    {
                        if (regDetails.PtRegDetailID > 0 && regDetails.IsChecked
                            && regDetails.PaidTime == null)
                        {
                            yield return new PrintRegisteredServiceSilently(registrationInfo, regDetails);
                        }
                    }
                }

                //In dich vu CLS
                //PrintPclItemsSilently
                if (registrationInfo != null && registrationInfo.PCLRequests != null)
                {
                    foreach (var request in registrationInfo.PCLRequests)
                    {
                        if (request.PatientPCLReqID > 0 && request.IsChecked
                            && request.PaidTime == null)
                        {
                            var ids = (from requestDetails in request.PatientPCLRequestIndicators where requestDetails.PCLReqItemID > 0 && requestDetails.EntityState == EntityState.PERSITED select requestDetails.PCLReqItemID).ToList();
                            if (ids.Count > 0)
                            {
                                yield return new PrintPclItemsSilently(registrationInfo, request, ids);
                            }
                        }
                    }
                }

                yield return Loader.Hide();
            }
        }

        /// <summary>
        /// Kiểm tra hợp lệ khi cập nhật những service chưa tính tiền.
        /// </summary>
        /// <returns></returns>
        private IEnumerator<IResult> DoValidateOldServices(YieldValidationResult result)
        {
            if (!RegistrationInfoHasChanged)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1204_G1_ChuaThayDoiTTin), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
            if (ValidateRegistration != null)
            {
                IEnumerator e = ValidateRegistration(_currentRegistration, result);

                while (e.MoveNext())
                    yield return e.Current as IResult;
                if (!result.IsValid)
                {
                    yield break;
                }
            }
            if (_currentRegistration.PatientRegistrationDetails != null
                && _currentRegistration.PatientRegistrationDetails.Any(x => x.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && x.RefundTime == null)
                && _currentRegistration.PatientRegistrationDetails.Where(x => x.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && x.RefundTime == null).GroupBy(x => x.HosClientContractID).Count() > 1)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z2945_G1_Msg, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                result.IsValid = false;
                yield break;
            }
            if (_currentRegistration.PCLRequests != null
                && _currentRegistration.PCLRequests.SelectMany(x => x.PatientPCLRequestIndicators).Any(x => x.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && x.RefundTime == null)
                && _currentRegistration.PCLRequests.SelectMany(x => x.PatientPCLRequestIndicators).Where(x => x.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI && x.RefundTime == null).GroupBy(x => x.HosClientContractID).Count() > 1)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z2945_G1_Msg, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                result.IsValid = false;
                yield break;
            }
            if (_currentRegistration.PatientRegistrationDetails == null || _currentRegistration.PatientRegistrationDetails.Count(item => item.RecordState == RecordState.MODIFIED) == 0)
            {
                //Kiem tra co CLS khong. Neu cung khong co luon thi bao chua duoc
                if (_currentRegistration.PCLRequests == null || _currentRegistration.PCLRequests.Count(item => item.RecordState == RecordState.MODIFIED) == 0)
                {
                    _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1204_G1_ChuaThayDoiTTin), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                    yield return _msgTask;
                    yield break;
                }
            }
            result.IsValid = true;
        }
        /// <summary>
        /// Lưu những chi tiết đăng ký cũ. Thực ra chỉ là xóa những đăng ký đã tính tiền
        /// </summary>
        /// <returns></returns>
        private IEnumerator<IResult> DoSaveOldServices()
        {
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateOldServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;
            if (!result.IsValid)
            {
                yield break;
            }
            CurrentRegistration.BasicDiagTreatment = BasicDiagTreatment;
            //Se dang ky ngay o cho nay.
            var oldServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            var oldPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            if (oldServiceList.Count == 0 && oldPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1202_G1_KgCoThayDoi), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;

            }
            //20190117 Gán giá trị cho Phiếu yêu cầu CLS từ quầy đăng ký dựa vào Chẩn đoán ban đầu.
            foreach (var x in oldPclRequestList)
            {
                if (x.PatientPCLReqID > 0)
                {
                    break;
                }
                if (!string.IsNullOrEmpty(BasicDiagTreatment) && BasicDiagTreatment.Length > 2)
                {
                    x.Diagnosis = BasicDiagTreatment;
                    x.DoctorStaffID = DoctorStaffID;
                }
            }
            var removeOldServiceTask = new RemoveOldRegItemTask(_currentRegistration, oldServiceList, oldPclRequestList, null);
            yield return removeOldServiceTask;
            if (!string.IsNullOrEmpty(removeOldServiceTask.ErrorMesage))
            {
                this.HideBusyIndicator();
                var message = new MessageWarningShowDialogTask(removeOldServiceTask.ErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = _currentRegistration });
                yield break;
            }
            if (removeOldServiceTask.Error != null)
            {
                //Thong bao loi.
                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(removeOldServiceTask.Error) });
                yield break;
            }
            //Thanh cong roi.
            Globals.EventAggregator.Publish(new UpdateCompleted<PatientRegistration> { Item = removeOldServiceTask.Registration });
            //20181212 TTM: Bắn sự kiện để PatientRegistration gán CurRegistration cho PatientSummaryInfoV3.
            Globals.EventAggregator.Publish(new SetCurRegistrationForPatientSummaryInfoV3 { });
        }

        private IEnumerator<IResult> DoSaveAndPayOldServices(bool IsCheckReported = true)
        {
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateOldServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;
            if (!result.IsValid)
            {
                yield break;
            }
            CurrentRegistration.BasicDiagTreatment = BasicDiagTreatment;
            //Se dang ky ngay o cho nay.
            var oldServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            var oldPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            if (oldServiceList.Count == 0 && oldPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1202_G1_KgCoThayDoi), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;

            }
            //20190117 Gán giá trị cho Phiếu yêu cầu CLS từ quầy đăng ký dựa vào Chẩn đoán ban đầu.
            foreach (var x in oldPclRequestList)
            {
                if (x.PatientPCLReqID > 0)
                {
                    break;
                }
                if (!string.IsNullOrEmpty(BasicDiagTreatment) && BasicDiagTreatment.Length > 2)
                {
                    x.Diagnosis = BasicDiagTreatment;
                    x.DoctorStaffID = DoctorStaffID;
                }
            }
            var removeOldServiceTask = new RemoveOldRegItemTask(_currentRegistration, oldServiceList, oldPclRequestList, Apply15HIPercent);
            yield return removeOldServiceTask;
            if (!string.IsNullOrEmpty(removeOldServiceTask.ErrorMesage))
            {
                this.HideBusyIndicator();
                var message = new MessageWarningShowDialogTask(removeOldServiceTask.ErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = _currentRegistration });
                yield break;
            }
            if (removeOldServiceTask.Error != null)
            {
                //Thong bao loi.
                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(removeOldServiceTask.Error) });
                yield break;
            }
            //Thanh cong roi.
            Globals.EventAggregator.Publish(new UpdateCompleted<PatientRegistration> { Item = removeOldServiceTask.Registration });
            _waitForSaveCompletedTask = new WaitForSaveRegistrationCompletedTask(removeOldServiceTask.Registration.PtRegistrationID, _eventAggregator);
            yield return _waitForSaveCompletedTask;
            if (_waitForSaveCompletedTask.RegistrationInfo != null)
            {
                if (IsCheckReported)
                {
                    PayForOldServiceCmd();
                }
                else
                {
                    ConfirmHIPercentCmd(true);
                }
            }
            //20181212 TTM: Bắn sự kiện để PatientRegistration gán CurRegistration cho PatientSummaryInfoV3.
            Globals.EventAggregator.Publish(new SetCurRegistrationForPatientSummaryInfoV3 { });
        }

        private IEnumerator<IResult> CountAgainDoSaveAndPayOldServices(bool bThuTien)
        {
            if ((_currentRegistration.PatientRegistrationDetails != null && _currentRegistration.PatientRegistrationDetails.Count > 0) || (_currentRegistration.PCLRequests != null && _currentRegistration.PCLRequests.Count > 0))
            {

                //Se dang ky ngay o cho nay.
                var oldServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

                var oldPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

                var removeOldServiceTask = new RemoveOldRegItemTask(_currentRegistration, oldServiceList, oldPclRequestList, Apply15HIPercent);
                yield return removeOldServiceTask;
                if (!string.IsNullOrEmpty(removeOldServiceTask.ErrorMesage))
                {
                    var message = new MessageWarningShowDialogTask(removeOldServiceTask.ErrorMesage, "", false);
                    yield return message;
                    Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = _currentRegistration });
                    yield break;
                }
                if (removeOldServiceTask.Error != null)
                {
                    //Thong bao loi.
                    Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(removeOldServiceTask.Error) });
                    yield break;
                }
                //Thanh cong roi.
                Globals.EventAggregator.Publish(new UpdateCompleted<PatientRegistration> { Item = removeOldServiceTask.Registration });
                _waitForSaveCompletedTask = new WaitForSaveRegistrationCompletedTask(removeOldServiceTask.Registration.PtRegistrationID, _eventAggregator);
                yield return _waitForSaveCompletedTask;
                if (bThuTien)
                {
                    if (_waitForSaveCompletedTask.RegistrationInfo != null)
                    {
                        PayForOldServiceCmd();
                    }
                }
            }
            else
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1205_G1_KgTheTinhLai, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;
            }
        }

        public IEnumerator<IResult> DoPrintOldService(PatientRegistration registrationInfo)
        {
            _msgTask = new MessageBoxTask(eHCMSResources.Z0368_G1_BanCoMuonInKg, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
            yield return _msgTask;
            if (_msgTask.Result == AxMessageBoxResult.Ok)
            {
                yield return Loader.Show("Đang in");

                //In Dich vu kham chua benh
                if (registrationInfo != null && registrationInfo.PtRegistrationID > 0 &&
                    registrationInfo.PatientRegistrationDetails != null)
                {
                    foreach (var regDetails in registrationInfo.PatientRegistrationDetails)
                    {
                        if (regDetails.PtRegDetailID > 0 && regDetails.IsChecked
                            && regDetails.PaidTime != null)
                        {
                            yield return new PrintRegisteredServiceSilently(registrationInfo, regDetails);
                        }
                    }
                }

                //In dich vu CLS
                //PrintPclItemsSilently
                if (registrationInfo != null && registrationInfo.PCLRequests != null)
                {
                    foreach (var request in registrationInfo.PCLRequests)
                    {
                        if (request.PatientPCLReqID > 0 && request.IsChecked
                            && request.PaidTime != null)
                        {
                            var ids = (from requestDetails in request.PatientPCLRequestIndicators where requestDetails.PCLReqItemID > 0 && requestDetails.EntityState == EntityState.PERSITED select requestDetails.PCLReqItemID).ToList();
                            if (ids.Count > 0)
                            {
                                yield return new PrintPclItemsSilently(registrationInfo, request, ids);
                            }
                        }
                    }
                }

                yield return Loader.Hide();
            }
        }
        #endregion

        #region EVENT HANDLERS
        public void Handle(RemoveItem<PatientRegistrationDetail> message)
        {
            if (GetView() != null)
            {
                PatientRegistrationDetail details = message.Item;
                Coroutine.BeginExecute(DoRemovePatientRegistrationDetails(details), null, (o, e) =>
                {

                });
            }
        }
        public void Handle(RemoveItem<PatientPCLRequestDetail> message)
        {
            if (GetView() != null)
            {
                PatientPCLRequestDetail details = message.Item;
                Coroutine.BeginExecute(DoRemovePatientPCLRequestDetails(details), null, (o, e) =>
                {

                });
            }
        }
        public void Handle(RemoveItem<PatientPCLRequest> message)
        {
            if (GetView() != null)
            {
                PatientPCLRequest pclRequest = message.Item;
                Coroutine.BeginExecute(DoRemovePatientPCLRequest(pclRequest), null, (o, e) =>
                {
                });
            }
        }
        public void Handle(RemoveItem<InPatientBillingInvoice> message)
        {
            if (GetView() != null && message != null && message.Item != null
                && CurrentRegistration != null
                && CurrentRegistration.InPatientBillingInvoices != null)
            {
                InPatientBillingInvoice aBillingInvoice = message.Item;
                if (CurrentRegistration.InPatientBillingInvoices.Any(x => x.InPatientBillingInvID == aBillingInvoice.InPatientBillingInvID))
                {
                    CurrentRegistration.InPatientBillingInvoices.First(x => x.InPatientBillingInvID == aBillingInvoice.InPatientBillingInvID).RecordState = RecordState.DELETED;
                    OldBillingContent.UpdateItemList(CurrentRegistration.InPatientBillingInvoices == null || !CurrentRegistration.InPatientBillingInvoices.Any(x => x.PaidTime != null && x.RecordState != RecordState.DELETED) ? null : CurrentRegistration.InPatientBillingInvoices.Where(x => x.PaidTime != null && x.RecordState != RecordState.DELETED).ToList());
                }
            }
        }
        public void Handle(ChangeHIStatus<PatientRegistrationDetail> message)
        {
            if (GetView() != null)
            {
                OnDetailsChanged();
                InitViewForServiceItems();
            }
        }
        public void Handle(ChangeDiscountStatus<PatientRegistrationDetail> message)
        {
            if (GetView() != null)
            {
                OnDetailsChanged();
                //▼===== 20200717 TTM:  khi thay đổi thông tin miễn giảm không cần thiết phải gọi hàm này vì lúc này dịch vụ đã nằm ở màn hình rồi.
                //                      Hàm đưa dịch vụ vào content này chỉ sử dụng khi thêm mới dịch vụ hoặc load dịch vụ thôi
                //InitViewForServiceItems();
                //▲===== 20200717 
            }
        }
        public void Handle(ChangeDiscountStatus<PatientPCLRequestDetail> message)
        {
            if (GetView() != null)
            {
                OnDetailsChanged();
                //▼===== 20200717 TTM:  khi thay đổi thông tin miễn giảm không cần thiết phải gọi hàm này vì lúc này dịch vụ đã nằm ở màn hình rồi.
                //                      Hàm đưa dịch vụ vào content này chỉ sử dụng khi thêm mới dịch vụ hoặc load dịch vụ thôi
                //                      P/s: Code chỗ này là copy từ trên xuống nhưng không kiểm tra InitViewForServiceItems là hàm dành cho dịch vụ
                //                      Đáng lẻ có xài phải xài hàm InitViewForPCLRequests mới đúng.
                //InitViewForServiceItems();
                //▲===== 20200717 
            }
        }
        #endregion

        private void OnDetailsChanged()
        {
            RegistrationInfoHasChanged = true;
            if (_currentRegMode == RegistrationFormMode.OLD_REGISTRATION_OPENED)
            {
                CurrentRegMode = RegistrationFormMode.OLD_REGISTRATION_CHANGED;
            }
            else if (_currentRegMode == RegistrationFormMode.NEW_REGISTRATION_OPENED)
            {
                CurrentRegMode = RegistrationFormMode.NEW_REGISTRATION_CHANGED;
            }
        }

        private bool _isInEditMode;
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set
            {
                if (_isInEditMode != value)
                {
                    _isInEditMode = value;
                    NotifyOfPropertyChange(() => IsInEditMode);
                    NotifyOfPropertyChange(() => CanAddService);
                    NotifyOfPropertyChange(() => CanStartAddingNewServicesAndPclCmd);
                    NotifyOfPropertyChange(() => CanStartEditRegistrationCmd);
                }
            }
        }
        /// <summary>
        /// Lưu giữ 1 bản backup của đăng ký mỗi khi bắt đầu edit.
        /// </summary>
        private PatientRegistration _tempRegistration;
        public PatientRegistration tempRegistration
        {
            get { return _tempRegistration; }
            set
            {
                if (_tempRegistration != value)
                {
                    _tempRegistration = value;
                    NotifyOfPropertyChange(() => tempRegistration);
                }
            }
        }
        public void BeginEdit()
        {
            if (CurrentRegistration != null)
            {
                IsInEditMode = true;
                RegistrationInfoHasChanged = false;
                _tempRegistration = CurrentRegistration.DeepCopy();
            }
        }

        public void CancelEdit()
        {
            //if (_tempRegistration != null)
            //{
            //IsInEditMode = false;
            ////CurrentRegistration = _tempRegistration;
            //SetRegistration(_tempRegistration); //20181218 TBL: Khi doi tab thi thong tin mien giam se clear.
            //_tempRegistration = null;
            //RegistrationInfoHasChanged = false;
            //CurrentRegMode = _currentRegistration.PtRegistrationID > 0 ? RegistrationFormMode.OLD_REGISTRATION_OPENED : RegistrationFormMode.NEW_REGISTRATION_OPENED;
            //}
            IsInEditMode = false;
            RemoveAllNewService();
            RegistrationInfoHasChanged = false;
            CurrentRegMode = _currentRegistration.PtRegistrationID > 0 ? RegistrationFormMode.OLD_REGISTRATION_OPENED : RegistrationFormMode.NEW_REGISTRATION_OPENED;
        }
        //▼====== #004
        private void RemoveAllNewService()
        {
            if (CurrentRegistration != null)
            {
                PatientRegistration TmpCurrentRegistration = new PatientRegistration();
                TmpCurrentRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                TmpCurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                if (CurrentRegistration.PatientRegistrationDetails != null)
                {
                    foreach (var oldServiceItem in CurrentRegistration.PatientRegistrationDetails)
                    {
                        if (oldServiceItem.PtRegDetailID > 0)
                        {
                            TmpCurrentRegistration.PatientRegistrationDetails.Add(oldServiceItem);
                        }
                    }
                    if (TmpCurrentRegistration.PatientRegistrationDetails != null)
                    {
                        CurrentRegistration.PatientRegistrationDetails = TmpCurrentRegistration.PatientRegistrationDetails;
                    }
                }
                if (CurrentRegistration.PCLRequests != null)
                {
                    foreach (var oldPCLItem in CurrentRegistration.PCLRequests)
                    {
                        if (oldPCLItem.PatientPCLReqID > 0)
                        {
                            TmpCurrentRegistration.PCLRequests.Add(oldPCLItem);
                        }
                    }
                    if (TmpCurrentRegistration.PCLRequests != null)
                    {
                        CurrentRegistration.PCLRequests = TmpCurrentRegistration.PCLRequests;
                    }
                }
            }
            RegistrationInfoHasChanged = false;
            IsInEditMode = false;
            IsShowCount15HIPercentCmd = CurrentRegistration.IsCrossRegion.GetValueOrDefault(true) ? false : true;
            NewServiceContent.RegistrationObj = CurrentRegistration;
            NewPclContent.RegistrationObj = CurrentRegistration;
            InitRegistration();
        }
        //▲====== #004
        /// <summary>
        /// Khi remove dich vu co bao hiem, refresh lai danh sach
        /// </summary>
        private void RefreshRegistrationDetails()
        {
            foreach (var item in CurrentRegistration.PatientRegistrationDetails)
            {
                //--Chỉ áp dụng cho dịch vụ ko chỉnh sửa
                if (item.RecordState == RecordState.MODIFIED)
                {
                    item.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime());
                    item.GetItemTotalPrice();
                    //cho thang dau tien roi dung lai
                    if (item.TotalHIPayment > 0)
                    {
                        break;
                    }
                }
            }
        }

        private void CorrectRegistrationDetailsForAppt()
        {
            //Có áp dụng luật Bảo hiểm đối với dịch vụ KCB hay không (Đối với tất cả các dịch vụ KCB, bảo hiểm chỉ tính 1 dịch vụ thôi, còn lại là không có BH)
            //if (!Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.SpecialRuleForHIConsultationApplied]))

            // Txd 25/05/2014 Replaced ConfigList
            if (!Globals.ServerConfigSection.HealthInsurances.SpecialRuleForHIConsultationApplied)
            {
                return;
            }
            if (CurrentRegistration.PatientRegistrationDetails == null)
            {
                return;
            }
            //Tinh tong so dich vu KCB duoc bao hiem thanh toan, neu tong so nay > 1 thi phai tinh lai

            //(Chi co 1 dich vu KCB duoc tinh bao hiem)
            //Lay tat ca cac dich vu KCB co bao hiem
            IList<PatientRegistrationDetail> hiRegDetails = CurrentRegistration.PatientRegistrationDetails.Where(registrationDetail =>
                        registrationDetail.RefMedicalServiceItem.RefMedicalServiceType.V_RefMedicalServiceTypes == (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH
                        && registrationDetail.RecordState != RecordState.DELETED
                        && !registrationDetail.MarkedAsDeleted
                        && registrationDetail.ExamRegStatus != AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                        && registrationDetail.HisID.HasValue && registrationDetail.HisID.Value > 0
                        && registrationDetail.HIAllowedPrice.HasValue
                        && registrationDetail.HIAllowedPrice.Value > 0).ToList();

            if (hiRegDetails == null || hiRegDetails.Count < 1)
            {
                return; // Khong co dich vu BH nao thi return
            }

            //Tinh tong so nhung dich vu bao hiem chap nhan tinh
            Func<PatientRegistrationDetail, bool> hiAcceptedRegDetails = registrationDetail => registrationDetail.TotalHIPayment > 0;

            //BH chi tinh cho 1 dich vu KCB thoi
            var total = hiRegDetails.Where(hiAcceptedRegDetails).Count();
            if (total == 0)
            {
                var registrationDetail = hiRegDetails.FirstOrDefault();
                var totalHiPayment = registrationDetail.TotalHIPayment;
                var totalCoPayment = registrationDetail.TotalCoPayment;
                var hisID = registrationDetail.HisID;
                var benefit = registrationDetail.HIBenefit;

                if (registrationDetail.ID > 0 && registrationDetail.HisID.HasValue
                    && registrationDetail.HisID.Value > 0
                    && registrationDetail.HIAllowedPrice.HasValue
                    && registrationDetail.HIAllowedPrice.Value > 0)//Co su dung the bh
                {
                    registrationDetail.HIBenefit = CurrentRegistration.PtInsuranceBenefit;
                }

                registrationDetail.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime());
                registrationDetail.GetItemTotalPrice();

                if (registrationDetail.TotalCoPayment != totalCoPayment
                    || registrationDetail.TotalHIPayment != totalHiPayment
                    || hisID != registrationDetail.HisID
                    || benefit != registrationDetail.HIBenefit)
                {
                    if (registrationDetail.RecordState == RecordState.UNCHANGED)
                    {
                        registrationDetail.RecordState = RecordState.ADDED;
                    }
                }
            }
            else if (total > 1)
            {
                var firstItem = hiRegDetails.First(hiAcceptedRegDetails);
                //Thang dau tien khong tinh.
                foreach (var registrationDetail in hiRegDetails.Where(item => item != firstItem))
                {
                    var totalHiPayment = registrationDetail.TotalHIPayment;
                    var totalCoPayment = registrationDetail.TotalCoPayment;

                    registrationDetail.HIBenefit = 0;
                    registrationDetail.TotalCoPayment = 0;
                    registrationDetail.TotalHIPayment = 0;

                    registrationDetail.TotalPatientPayment = registrationDetail.TotalInvoicePrice;
                    registrationDetail.TotalPriceDifference = registrationDetail.TotalPatientPayment;

                    if (registrationDetail.TotalCoPayment != totalCoPayment
                        || registrationDetail.TotalHIPayment != totalHiPayment)
                    {
                        if (registrationDetail.RecordState == RecordState.UNCHANGED)
                        {
                            registrationDetail.RecordState = RecordState.ADDED;
                        }
                    }
                }
            }
        }

        private void AddNewRegistrationDetails(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, long? ConsultationRoomStaffAllocID = null,
                                                DateTime? ApptStartDate = null, DateTime? ApptEndDate = null, Staff staff = null
                                                , DateTime? dt = null, string diagnosis = null, bool isPriority = false
            , decimal Qty = 1)
        {
            var newRegistrationDetail = new PatientRegistrationDetail { RefMedicalServiceItem = serviceItem };
            newRegistrationDetail.RefMedicalServiceItem.RefMedicalServiceType = serviceType;
            newRegistrationDetail.DeptLocation = deptLoc != null && deptLoc.DeptLocationID > 0 ? deptLoc : null;
            newRegistrationDetail.EntityState = EntityState.DETACHED;
            newRegistrationDetail.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            //newRegistrationDetail.CreatedDate = DateTime.Today.Date;
            newRegistrationDetail.CreatedDate = Globals.GetCurServerDateTime();

            newRegistrationDetail.Qty = Qty;
            newRegistrationDetail.HisID = CurrentRegistration.HisID;
            newRegistrationDetail.RecordState = RecordState.ADDED;
            newRegistrationDetail.CanDelete = true;
            newRegistrationDetail.HIAllowedPrice = serviceItem.HIAllowedPrice;
            newRegistrationDetail.InvoicePrice = CurrentRegistration.HealthInsurance != null ? serviceItem.HIPatientPrice : serviceItem.NormalPrice;
            newRegistrationDetail.ReqDeptID = Globals.DeptLocation.DeptID; //20181217 TBL: Them khoa de khi in report Phieu chi dinh se co noi chi dinh

            newRegistrationDetail.GetItemPrice(CurrentRegistration, Globals.GetCurServerDateTime(), false, Globals.ServerConfigSection.HealthInsurances.FullHIBenefitForConfirm, Globals.ServerConfigSection.HealthInsurances.HiPolicyMinSalary);
            if (CurrentRegistration.Appointment != null &&
                CurrentRegistration.Appointment.ClientContract != null &&
                CurrentRegistration.Appointment.ClientContract.DiscountPercent > 0)
            {
                newRegistrationDetail.DiscountAmt = newRegistrationDetail.PatientPayment * CurrentRegistration.Appointment.ClientContract.DiscountPercent / 100;
                newRegistrationDetail.PatientPayment = newRegistrationDetail.PatientPayment - newRegistrationDetail.DiscountAmt;
            }
            newRegistrationDetail.GetItemTotalPrice();
            newRegistrationDetail.ConsultationRoomStaffAllocID = ConsultationRoomStaffAllocID;
            newRegistrationDetail.ApptStartDate = ApptStartDate;
            newRegistrationDetail.ApptEndDate = ApptEndDate;
            newRegistrationDetail.DoctorStaffID = staff != null ? staff.StaffID : 0;
            newRegistrationDetail.MedicalInstructionDate = dt;
            newRegistrationDetail.Diagnosis = diagnosis;
            newRegistrationDetail.IsPriority = isPriority;
            newRegistrationDetail.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
            CurrentRegistration.PatientRegistrationDetails.Add(newRegistrationDetail);

            //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
            CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);

            InitViewForServiceItems();


            OnDetailsChanged();
        }
        public void AddNewService(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, long? ConsultationRoomStaffAllocID = null,
                                    DateTime? ApptStartDate = null, DateTime? ApptEndDate = null, Staff staff = null, DateTime? dt = null, string diagnosis = null, bool isPriority = false
            , decimal Qty = 1)
        {
            Coroutine.BeginExecute(DoAddRegItem(serviceItem, deptLoc, serviceType, ConsultationRoomStaffAllocID, ApptStartDate, ApptEndDate, staff, dt, diagnosis, isPriority, Qty));
        }
        public void AddNewPclRequestDetailFromPclExamType(PCLExamType examType, DeptLocation deptLoc, Staff staff = null, DateTime? dt = null, string diagnosis = null, bool isPriority = false)
        {
            Coroutine.BeginExecute(DoAddPclExamType(examType, deptLoc, staff, dt, diagnosis, isPriority));
        }

        public void AddNewAllPclRequestDetailFromPclExamType(ObservableCollection<PCLExamType> AllExamType, Staff staff = null, DateTime? dt = null, string diagnosis = null)
        {
            Coroutine.BeginExecute(DoAddAllPclExamType(AllExamType, staff, dt, diagnosis));
        }

        public ValidateRegistrationInfo ValidateRegistration
        {
            get;
            set;
        }

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }
        #region checking account

        private bool _mDichVuDaTT_ChinhSua = true;
        //private bool _mDichVuDaTT_Luu = true
        //&& Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ShowAddRegisButton]);
        // Txd 25/05/2014 Replaced ConfigList

        private bool _mDichVuDaTT_Luu = true
            && Globals.ServerConfigSection.CommonItems.ShowAddRegisButton;

        private bool _mDichVuDaTT_TraTien = true;
        private bool _mDichVuDaTT_In = true;
        private bool _mDichVuDaTT_LuuTraTien = true;

        private bool _mDichVuMoi_ChinhSua = true;

        // Txd 25/05/2014 Replaced ConfigList
        private bool _mDichVuMoi_Luu = true && Globals.ServerConfigSection.CommonItems.ShowAddRegisButton;
        //Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ShowAddRegisButton]);

        private bool _mDichVuMoi_TraTien = true;
        private bool _mDichVuMoi_In = true;
        private bool _mDichVuMoi_LuuTraTien = true;

        public bool mDichVuDaTT_ChinhSua
        {
            get
            {
                return _mDichVuDaTT_ChinhSua;
            }
            set
            {
                if (_mDichVuDaTT_ChinhSua == value)
                    return;
                _mDichVuDaTT_ChinhSua = value;
                NotifyOfPropertyChange(() => mDichVuDaTT_ChinhSua);
            }
        }

        public bool mDichVuDaTT_Luu
        {
            get
            {
                // Txd 25/05/2014 Replaced ConfigList
                return _mDichVuDaTT_Luu && Globals.ServerConfigSection.CommonItems.ShowAddRegisButton;
                //&& Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ShowAddRegisButton]);
            }
            set
            {
                if (_mDichVuDaTT_Luu == value)
                    return;
                _mDichVuDaTT_Luu = value;
                NotifyOfPropertyChange(() => mDichVuDaTT_Luu);
            }
        }

        public bool mDichVuDaTT_TraTien
        {
            get
            {
                return _mDichVuDaTT_TraTien;
            }
            set
            {
                if (_mDichVuDaTT_TraTien == value)
                    return;
                _mDichVuDaTT_TraTien = value;
                NotifyOfPropertyChange(() => mDichVuDaTT_TraTien);
            }
        }

        public bool mDichVuDaTT_In
        {
            get
            {
                return _mDichVuDaTT_In;
            }
            set
            {
                if (_mDichVuDaTT_In == value)
                    return;
                _mDichVuDaTT_In = value;
                NotifyOfPropertyChange(() => mDichVuDaTT_In);
            }
        }

        public bool mDichVuDaTT_LuuTraTien
        {
            get
            {
                return _mDichVuDaTT_LuuTraTien;
            }
            set
            {
                if (_mDichVuDaTT_LuuTraTien == value)
                    return;
                _mDichVuDaTT_LuuTraTien = value;
                NotifyOfPropertyChange(() => mDichVuDaTT_LuuTraTien);
            }
        }


        public bool mDichVuMoi_ChinhSua
        {
            get
            {
                return _mDichVuMoi_ChinhSua;
            }
            set
            {
                if (_mDichVuMoi_ChinhSua == value)
                    return;
                _mDichVuMoi_ChinhSua = value;
                NotifyOfPropertyChange(() => mDichVuMoi_ChinhSua);
            }
        }

        public bool mDichVuMoi_Luu
        {
            get
            {
                //return _mDichVuMoi_Luu && Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ShowAddRegisButton]);
                // Txd 25/05/2014 Replaced ConfigList
                return _mDichVuMoi_Luu && Globals.ServerConfigSection.CommonItems.ShowAddRegisButton;
            }
            set
            {
                if (_mDichVuMoi_Luu == value)
                    return;
                _mDichVuMoi_Luu = value;
                NotifyOfPropertyChange(() => mDichVuMoi_Luu);
            }
        }

        public bool mDichVuMoi_TraTien
        {
            get
            {
                return _mDichVuMoi_TraTien;
            }
            set
            {
                if (_mDichVuMoi_TraTien == value)
                    return;
                _mDichVuMoi_TraTien = value;
                NotifyOfPropertyChange(() => mDichVuMoi_TraTien);
            }
        }

        public bool mDichVuMoi_In
        {
            get
            {
                return _mDichVuMoi_In;
            }
            set
            {
                if (_mDichVuMoi_In == value)
                    return;
                _mDichVuMoi_In = value;
                NotifyOfPropertyChange(() => mDichVuMoi_In);
            }
        }

        public bool mDichVuMoi_LuuTraTien
        {
            get
            {
                return _mDichVuMoi_LuuTraTien;
            }
            set
            {
                if (_mDichVuMoi_LuuTraTien == value)
                    return;
                _mDichVuMoi_LuuTraTien = value;
                NotifyOfPropertyChange(() => mDichVuMoi_LuuTraTien);
            }
        }


        #endregion

        #region binding visibilty

        //public HyperlinkButton lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as HyperlinkButton;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        public void Handle(StateChanged<PatientPCLRequest> message)
        {
            if (GetView() == null)
            {
                return;
            }
            if (message.Item == null || CurrentRegistration == null || CurrentRegistration.PCLRequests == null) return;
            if (CurrentRegistration.PCLRequests.Contains(message.Item))
            {
                RegistrationInfoHasChanged = true;
            }
        }

        public void Handle(StateChanged<PatientRegistrationDetail> message)
        {
            if (GetView() == null)
            {
                return;
            }
            if (message.Item == null || CurrentRegistration == null ||
                CurrentRegistration.PatientRegistrationDetails == null) return;
            if (CurrentRegistration.PatientRegistrationDetails.Contains(message.Item))
            {
                RegistrationInfoHasChanged = true;
            }
        }
        //▼===== #013
        public void Handle(SetEkipForServiceSuccess message)
        {
            if (GetView() == null)
            {
                return;
            }
            if (message == null || message.RegistrationInfo == null || message.RegistrationInfo.PatientRegistrationDetails == null || CurrentRegistration == null || CurrentRegistration.PatientRegistrationDetails == null || CurrentRegistration.PatientRegistrationDetails.Count == 0)
            {
                return;
            }
            foreach (PatientRegistrationDetail item in message.RegistrationInfo.PatientRegistrationDetails)
            {
                foreach (PatientRegistrationDetail detail in CurrentRegistration.PatientRegistrationDetails)
                {
                    if (detail.MyID == item.MyID)
                    {
                        detail.V_Ekip = item.V_Ekip;
                        detail.V_EkipIndex = item.V_EkipIndex;
                        //▼====: #020
                        detail.UserOfficialAccountID = Globals.DoctorAccountBorrowed.StaffID;
                        //▲====: #020
                        if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip)
                        {
                            detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault() * (decimal)Globals.ServerConfigSection.HealthInsurances.PercentForEkip;
                            detail.HIPaymentPercent = Math.Round(Globals.ServerConfigSection.HealthInsurances.PercentForEkip, 2);
                            detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                            detail.PatientCoPayment = (detail.TotalHIPayment / (decimal)detail.HIBenefit.GetValueOrDefault()) * (1 - (decimal)detail.HIBenefit.GetValueOrDefault());
                            detail.TotalCoPayment = (decimal)Math.Ceiling((double)(detail.PatientCoPayment * detail.Qty));
                        }
                        else if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip)
                        {
                            detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault() * (decimal)Globals.ServerConfigSection.HealthInsurances.PercentForOtherEkip;
                            detail.HIPaymentPercent = Math.Round(Globals.ServerConfigSection.HealthInsurances.PercentForOtherEkip, 2);
                            detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                            detail.PatientCoPayment = (detail.TotalHIPayment / (decimal)detail.HIBenefit.GetValueOrDefault()) * (1 - (decimal)detail.HIBenefit.GetValueOrDefault());
                            detail.TotalCoPayment = (decimal)Math.Ceiling((double)(detail.PatientCoPayment * detail.Qty));
                        }
                        else
                        {
                            detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault();
                            detail.HIPaymentPercent = 1;
                            detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                            detail.PatientCoPayment = (detail.TotalHIPayment / (decimal)detail.HIBenefit.GetValueOrDefault()) * (1 - (decimal)detail.HIBenefit.GetValueOrDefault());
                            detail.TotalCoPayment = (decimal)Math.Ceiling((double)(detail.PatientCoPayment * detail.Qty));
                        }
                        if (detail.IsDiscounted)
                        {
                            CurrentRegistration.ApplyDiscount(detail, Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward, false);
                        }
                        CommonGlobals.ChangeItemsRecordState(detail);
                    }
                }
            }
        }
        //▲===== #013
        //20200222 TBL Mod TMV1: Nhận sự kiện từ popup
        public void Handle(RegDetailSelectedForProcess message)
        {
            if (GetView() == null)
            {
                return;
            }
            if (message == null || message.RegDetail == null || CurrentRegistration == null || CurrentRegistration.PatientRegistrationDetails == null)
            {
                return;
            }
            message.RegDetail.RecordState = RecordState.ADDED;
            message.RegDetail.PaidTimeTmp = message.RegDetail.PaidTime.DeepCopy();
            message.RegDetail.CanDelete = true;
            message.RegDetail.PackServDetailID = message.RegDetail.PtRegDetailID;
            CurrentRegistration.PatientRegistrationDetails.Add(message.RegDetail);
            InitViewForServiceItems();
            OnDetailsChanged();
        }
        public TabItem NewRegItemsTab
        {
            get;
            set;
        }
        public void tabitemNewItems_Loaded(object sender, RoutedEventArgs e)
        {
            NewRegItemsTab = (TabItem)sender;
        }

        public void Handle(ItemSelected<Patient> message)
        {
            if (message != null && NewRegItemsTab != null)
            {
                ((TabItem)NewRegItemsTab).IsSelected = true;
            }
        }

        public void Handle(SaveRegisFromSimplePayCompleted savedRegis)
        {
            if (savedRegis != null)
            {
                CurrentRegistration = savedRegis.RegistrationInfo;
                //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
                CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);
                BeginEdit();

                Globals.EventAggregator.Publish(new SetCurRegistrationForPatientSummaryInfoV3 { });
            }
        }

        public void Count15HIPercentCmd()
        {
            if (CurrentRegistration == null || Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.Staff == null)
            {
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecal15PercentHIBenefit(CurrentRegistration.PtRegistrationID, Globals.LoggedUserAccount.Staff.StaffID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    if (contract.EndRecal15PercentHIBenefit(asyncResult))
                                    {
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                finally
                                {
                                    Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurrentRegistration.Patient });
                                    if (NewRegItemsTab != null) ((TabItem)NewRegItemsTab).IsSelected = true;
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

        public void ConfirmHIPercentCmd(bool IsReported = false, bool IsUpdateHisID = false)
        {
            if (CurrentRegistration == null || Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.Staff == null)
            {
                return;
            }
            string mErrorMessage;
            if (Globals.CheckValidRegistrationForPay(_currentRegistration, out mErrorMessage, (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN))
            {
                Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                {
                    vm.Registration = CurrentRegistration;
                    if (!IsReported)
                    {
                        vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN;
                    }
                    else
                    {
                        vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY;
                    }
                    vm.FormMode = PaymentFormMode.PAY;
                    vm.AllowZeroPayment = true;
                    vm.RegistrationDetails = null;
                    vm.PclRequests = null;
                    vm.RegistrationDetails = CurrentRegistration.PatientRegistrationDetails.Where(x => x.PaidTime == null && (x.RecordState == RecordState.UNCHANGED || x.RecordState == RecordState.ADDED)).ToList();
                    //20200311 TBL: Khi bấm nút Hoàn tiền không lưu lại RefundTime xuống database
                    //vm.PclRequests = CurrentRegistration.PCLRequests.Where(x => x.PaidTime == null && x.RecordState == RecordState.UNCHANGED).ToList();
                    var lsPclRequests = new List<PatientPCLRequest>();
                    if (CurrentRegistration.PCLRequests != null)
                    {
                        foreach (var request in CurrentRegistration.PCLRequests)
                        {
                            //Neu co 1 request detail bi delete va chua hoan tien thi add nguyen cai request nay luon.
                            if (request.PatientPCLRequestIndicators != null)
                            {
                                if (request.PaidTime != null && request.RefundTime == null && request.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.CANCEL
                                    && request.RecordState == RecordState.UNCHANGED)
                                {
                                    lsPclRequests.Add(request);
                                    continue;
                                }

                                if (request.PatientPCLRequestIndicators.Any(requestDetail => requestDetail.PaidTime != null && requestDetail.RefundTime == null &&
                                                                                             requestDetail.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
                                                                                             && requestDetail.RecordState == RecordState.UNCHANGED))
                                {
                                    lsPclRequests.Add(request);
                                }
                            }
                        }
                    }
                    vm.PclRequests = lsPclRequests;
                    //20200311 TBL: Khi bấm nút Hoàn tiền không lưu lại RefundTime xuống database
                    vm.PayNewService = true;
                    vm.IsReported = IsReported;
                    vm.IsUpdateHisID = IsUpdateHisID;
                    if (CurrentRegistration.DrugInvoices == null)
                    {
                        CurrentRegistration.DrugInvoices = new ObservableCollection<OutwardDrugInvoice>();
                    }
                    vm.DrugInvoices = CurrentRegistration.DrugInvoices.Where(x => x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE).ToList();
                    vm.StartCalculating();
                };
                GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
            }
            else
            {
                //▼====== #001
                if (mErrorMessage != null)
                {
                    MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
                //▲====== #001
            }
            //this.ShowBusyIndicator();
            //var t = new Thread(() =>
            //{
            //    try
            //    {
            //        using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
            //        {
            //            var contract = serviceFactory.ServiceInstance;
            //            contract.BeginRecalRegistrationHIBenefit(CurrentRegistration.PtRegistrationID, Globals.LoggedUserAccount.Staff.StaffID,
            //                Globals.DispatchCallback((asyncResult) =>
            //                {
            //                    try
            //                    {
            //                        if (contract.EndRecalRegistrationHIBenefit(asyncResult))
            //                        {
            //                        }
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //                    }
            //                    finally
            //                    {
            //                        Globals.EventAggregator.Publish(new ItemSelected<Patient>() { Item = CurrentRegistration.Patient });
            //                        if (NewRegItemsTab != null) ((TabItem)NewRegItemsTab).IsSelected = true;
            //                        this.HideBusyIndicator();
            //                    }

            //                }), null);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //        this.HideBusyIndicator();
            //    }
            //});
            //t.Start();
        }

        //▼==== #023
        public void btnDelTranFinalWithOutBill()
        {
            DeleteTransFinalCmd(true);
        }
        //▲==== #023
        
        //▼==== #022
        public void DeleteTransFinalCmd(bool IsWithOutBill = false)
        {
            if (CurrentRegistration == null || CurrentRegistration.PtRegistrationID == 0 || CurrentRegistration.V_RegistrationStatus != (long)AllLookupValues.RegistrationStatus.COMPLETED)
            {
                MessageBox.Show(eHCMSResources.Z2716_G1_CaKhamChuaDuocQuyetToan, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.StaffID.GetValueOrDefault(0) == 0)
            {
                return;
            }
            var mThread = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteTransactionFinalization(null, Globals.LoggedUserAccount.StaffID.Value, (long)AllLookupValues.RegistrationType.NGOAI_TRU, CurrentRegistration.PtRegistrationID, IsWithOutBill, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            string mOutMessage = contract.EndDeleteTransactionFinalization(asyncResult);
                            if (!string.IsNullOrEmpty(mOutMessage))
                            {
                                MessageBox.Show(mOutMessage, eHCMSResources.G0442_G1_TBao);
                            }
                            CurrentRegistration.V_RegistrationStatus = (long)AllLookupValues.RegistrationStatus.PROCESSING;
                            SetRegistration(CurrentRegistration);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            mThread.Start();
        }
        //▲==== #022

        public void ChangeHIPercentCmd()
        {
            ConfirmHIPercentCmd(false, true);
        }

        public void NewCount15HIPercentCmd()
        {
            //if(CurrentRegistration ==null || CurrentRegistration.PtRegistrationID <=0 || CurrentRegistration.)
            Apply15HIPercent = 1;

            this.ShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            Coroutine.BeginExecute(CountAgainDoSaveAndPayOldServices(false), null, (o, e) =>
            {

                this.HideBusyIndicator();
            });
            Apply15HIPercent = null;
        }
        private bool? _gIsHIUnder15Percent;
        public bool? gIsHIUnder15Percent
        {
            get
            {
                return _gIsHIUnder15Percent;
            }
            set
            {
                if (_gIsHIUnder15Percent == value) return;
                _gIsHIUnder15Percent = value;
                NotifyOfPropertyChange(() => gIsHIUnder15Percent);
            }
        }

        private string gIAPISendHIReportAddress = "http://egw.baohiemxahoi.gov.vn/";
        private string gIAPISendHIReportAddressParams = "api/egw/guiHoSoGiamDinh?token={0}&id_token={1}&username={2}&password={3}&loaiHoSo=3&maTinh={4}&maCSKCB={5}";
        public void btnHIReport()
        {
            if ((GlobalsNAV.gLoggedHIAPIUser == null || GlobalsNAV.gLoggedHIAPIUser.APIKey == null || string.IsNullOrEmpty(GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token)))
            {
                GlobalsNAV.LoginHIAPI();
            }
            Coroutine.BeginExecute(CreateHIReportXml_Routine());
        }
        private void CreateHIReportFile(GenericCoRoutineTask genTask)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        HealthInsuranceReport mHealthInsuranceReport = new HealthInsuranceReport { Title = string.Format("BC 1-{0}", CurrentRegistration.PtRegistrationID), V_HIReportType = new Lookup { LookupID = (long)AllLookupValues.V_HIReportType.REGID } };
                        contract.BeginCreateHIReport_V2(mHealthInsuranceReport,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    long mHIReportID;
                                    var mResultVal = contract.EndCreateHIReport_V2(out mHIReportID, asyncResult);
                                    if (!mResultVal || mHIReportID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                    else
                                    {
                                        mHealthInsuranceReport.HIReportID = mHIReportID;
                                        genTask.AddResultObj(mHealthInsuranceReport);
                                        genTask.ActionComplete(true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private void CreateHIReportXml(GenericCoRoutineTask genTask, object aHealthInsuranceReport)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new TransactionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetHIXmlReport9324_AllTab123_InOneRpt((aHealthInsuranceReport as HealthInsuranceReport).HIReportID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var ReportStream = contract.EndGetHIXmlReport9324_AllTab123_InOneRpt(asyncResult);
                                    string mHIAPICheckHICardAddress = string.Format(gIAPISendHIReportAddressParams, GlobalsNAV.gLoggedHIAPIUser.APIKey.access_token, GlobalsNAV.gLoggedHIAPIUser.APIKey.id_token, Globals.ServerConfigSection.Hospitals.HIAPILoginAccount, GlobalsNAV.gLoggedHIAPIUser.password, Globals.ServerConfigSection.Hospitals.HospitalCode.Length < 2 ? "" : Globals.ServerConfigSection.Hospitals.HospitalCode.Substring(0, 2), Globals.ServerConfigSection.Hospitals.HospitalCode);
                                    string mRestJson = GlobalsNAV.GetRESTServiceJSon(gIAPISendHIReportAddress, mHIAPICheckHICardAddress, ReportStream);
                                    HIAPIUploadHIReportXmlResult mHIAPIUploadHIReportXmlResult = GlobalsNAV.ConvertJsonToObject<HIAPIUploadHIReportXmlResult>(mRestJson);
                                    if (mHIAPIUploadHIReportXmlResult.maKetQua == 200)
                                        genTask.ActionComplete(true);
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                        genTask.ActionComplete(false);
                                        this.HideBusyIndicator();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private IEnumerator<IResult> CreateHIReportXml_Routine()
        {
            this.ShowBusyIndicator();

            var CreateHIReportFileTask = new GenericCoRoutineTask(CreateHIReportFile);
            yield return CreateHIReportFileTask;

            var CreateHIReportXmlTask = new GenericCoRoutineTask(CreateHIReportXml, CreateHIReportFileTask.GetResultObj(0));
            yield return CreateHIReportXmlTask;

            this.HideBusyIndicator();
        }

        public bool CanCreateHIReport
        {
            get { return CurrentRegistration != null && CurrentRegistration.HIReportID == 0 ? true : false; }
        }
        public void ckbDiscount_Click(object source, object sender)
        {
            if (CurrentRegistration == null)
            {
                CurrentRegistration = new PatientRegistration();
            }
            if (CurrentRegistration.PtRegistrationID <= 0)
            {
                MessageBox.Show(eHCMSResources.Z2972_G1_KhongTaoMienGiamKhiChuaCoDangKy);
                return;
            }
            IPromoDiscountProgramCollection DiscountProgramCollectionView = Globals.GetViewModel<IPromoDiscountProgramCollection>();
            DiscountProgramCollectionView.PtRegistrationID = CurrentRegistration.PtRegistrationID;
            DiscountProgramCollectionView.V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU;
            if (CurrentRegistration.DiscountProgramCollection == null)
            {
                CurrentRegistration.DiscountProgramCollection = new List<PromoDiscountProgram>();
            }
            DiscountProgramCollectionView.DiscountProgramCollection = new ObservableCollection<PromoDiscountProgram>(CurrentRegistration.DiscountProgramCollection);
            //DiscountProgramCollectionView.GetAllExamptions();
            GlobalsNAV.ShowDialog_V3<IPromoDiscountProgramCollection>(DiscountProgramCollectionView, null, null, false, true, new Size(1000, 600));
            if (DiscountProgramCollectionView.IsUpdated)
            {
                CurrentRegistration.DiscountProgramCollection = new List<PromoDiscountProgram>(DiscountProgramCollectionView.DiscountProgramCollection);
            }
            if (DiscountProgramCollectionView.IsChoosed && DiscountProgramCollectionView.SelectedPromoDiscountProgram != null && CurrentRegistration.DiscountProgramCollection != null && CurrentRegistration.DiscountProgramCollection.Count > 0)
            {
                CurrentRegistration.PromoDiscountProgramObj = CurrentRegistration.DiscountProgramCollection.FirstOrDefault(x => x.PromoDiscProgID == DiscountProgramCollectionView.SelectedPromoDiscountProgram.PromoDiscProgID);
                NewServiceContent.NotifyOfCanApplyIsOnPriceDiscount();
                NewPclContent.NotifyOfCanApplyIsOnPriceDiscount();
                OldServiceContent.NotifyOfCanApplyIsOnPriceDiscount();
                OldPclContent.NotifyOfCanApplyIsOnPriceDiscount();
                //CommonGlobals.CorrectRegistrationDetails(CurrentRegistration);
                CommonGlobals.CorrectRegistrationDetails_V2(CurrentRegistration);
            }
        }
        public RegistrationViewCase ViewCase
        {
            get
            {
                return _ViewCase;
            }
            set
            {
                if (_ViewCase == value)
                {
                    return;
                }
                _ViewCase = value;
                NotifyOfPropertyChange(() => ViewCase);
                NotifyOfPropertyChange(() => IsRegistrationView);
                NotifyOfPropertyChange(() => IsRegistrationView2);
                NotifyOfPropertyChange(() => IsMedicalServiceGroupView);
                if (NewPclContent != null)
                {
                    NewPclContent.ViewCase = ViewCase;
                }
            }
        }
        public bool IsDiscountVisible
        {
            get
            {
                return _IsDiscountVisible;
            }
            set
            {
                if (_IsDiscountVisible == value)
                {
                    return;
                }
                _IsDiscountVisible = value;
                NotifyOfPropertyChange(() => IsDiscountVisible);
            }
        }
        private RegistrationViewCase _ViewCase = RegistrationViewCase.RegistrationView;
        public bool IsRegistrationView
        {
            get
            {
                return ViewCase == RegistrationViewCase.RegistrationView;
            }
        }
        public bool IsRegistrationView2
        {
            get
            {
                return ViewCase == RegistrationViewCase.RegistrationView && !IsProcess && Globals.ServerConfigSection.OutRegisElements.IsPerformingTMVFunctionsA;
            }
        }
        public bool IsMedicalServiceGroupView
        {
            get
            {
                return ViewCase == RegistrationViewCase.MedicalServiceGroupView;
            }
        }
        private bool _IsDiscountVisible = true;
        protected override void OnActivate()
        {
            base.OnActivate();
            if (ViewCase == RegistrationViewCase.MedicalServiceGroupView || ViewCase == RegistrationViewCase.RegistrationRequestView)
            {
                mDichVuDaTT_ChinhSua = false;
                if (ViewCase == RegistrationViewCase.MedicalServiceGroupView)
                {
                    mDichVuDaTT_Luu = false;
                }
                mDichVuDaTT_LuuTraTien = false;
                mDichVuDaTT_TraTien = false;
                mDichVuDaTT_In = false;
                IsDiscountVisible = false;
                IsInEditMode = true;
                RefMedicalServiceGroupObj = new RefMedicalServiceGroups();
                //▼===== #016
                if (ViewCase == RegistrationViewCase.RegistrationRequestView)
                {
                    base.HasInputBindingCmd = false;
                    IsRequestView = true;
                }
                //▲==== #016
            }
        }
        private RefMedicalServiceGroups _RefMedicalServiceGroupObj;
        public string SaveMedicalServiceGroupButtonTitle
        {
            get
            {
                return RefMedicalServiceGroupObj == null || RefMedicalServiceGroupObj.MedicalServiceGroupID == 0 ? eHCMSResources.T2937_G1_Luu : eHCMSResources.K1599_G1_CNhat;
            }
        }
        public RefMedicalServiceGroups RefMedicalServiceGroupObj
        {
            get => _RefMedicalServiceGroupObj; set
            {
                _RefMedicalServiceGroupObj = value;
                NotifyOfPropertyChange(() => RefMedicalServiceGroupObj);
                NotifyOfPropertyChange(() => SaveMedicalServiceGroupButtonTitle);
            }
        }
        public void Handle(CreateNewMedicalServiceGroup message)
        {
            RefMedicalServiceGroupObj = new RefMedicalServiceGroups();
            ApplyRefMedicalServiceGroup(RefMedicalServiceGroupObj,null);
        }
        public void SaveMedicalServiceGroupCmd()
        {
            if (RefMedicalServiceGroupObj == null)
            {
                RefMedicalServiceGroupObj = new RefMedicalServiceGroups();
            }
            if (string.IsNullOrEmpty(RefMedicalServiceGroupObj.MedicalServiceGroupCode) || string.IsNullOrEmpty(RefMedicalServiceGroupObj.MedicalServiceGroupName))
            {
                MessageBox.Show("Vui lòng nhập các thông tin bắt buộc(*)","", MessageBoxButton.OK);
                return;
            }
            if (RefMedicalServiceGroupObj.AgeFrom != 0 && RefMedicalServiceGroupObj.AgeTo != 0 && RefMedicalServiceGroupObj.AgeFrom > RefMedicalServiceGroupObj.AgeTo)
            {
                MessageBox.Show("Tuổi từ không được lớn hơn tuổi đến", "", MessageBoxButton.OK);
                return;
            }
            if (RefMedicalServiceGroupObj.V_MedicalServiceGroupType == (long)AllLookupValues.V_MedicalServiceGroupType.Kham_Benh)
            {
                RefMedicalServiceGroupObj.Gender = null;
                RefMedicalServiceGroupObj.AgeFrom = null;
                RefMedicalServiceGroupObj.AgeTo = null;
                RefMedicalServiceGroupObj.V_MedicalServiceParentGroup = null;
            }
            if((CurrentRegistration.PatientRegistrationDetails == null  || (CurrentRegistration.PatientRegistrationDetails != null 
                && !CurrentRegistration.PatientRegistrationDetails.Any(x => x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED)))
               && (CurrentRegistration.PCLRequests == null || (CurrentRegistration.PCLRequests != null 
               && !CurrentRegistration.PCLRequests.Any(x => (x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED) 
               && x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Count > 0))))
            {
                if(MessageBox.Show("Gói không có dịch vụ nào? Bạn có muốn lưu?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            List<MedRegItemBase> mMedRegItemBases = new List<MedRegItemBase>();
            //if (CurrentRegistration.PatientRegistrationDetails != null && CurrentRegistration.PatientRegistrationDetails.Any(x => x.DeptLocation == null))
            //{
            //    MessageBox.Show("Có dịch vụ chưa có phòng!", "", MessageBoxButton.OK);
            //    return;
            //}
            //if (CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Any(x => (x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED) && x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Count > 0 && x.PatientPCLRequestIndicators.Any(y => y.DeptLocation == null)))
            //{
            //    MessageBox.Show("Có CLS chưa có phòng!", eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            //    return;
            //}
            if (CurrentRegistration.PatientRegistrationDetails != null && CurrentRegistration.PatientRegistrationDetails.Any(x => x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED))
            {
                
                mMedRegItemBases.AddRange(CurrentRegistration.PatientRegistrationDetails.Where(x => x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED).ToList());
            }
            if (CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Any(x => (x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED) && x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Count > 0))
            {
                mMedRegItemBases.AddRange(CurrentRegistration.PCLRequests.Where(x => (x.RecordState == RecordState.ADDED || x.RecordState == RecordState.DETACHED) && x.PatientPCLRequestIndicators != null && x.PatientPCLRequestIndicators.Count > 0).SelectMany(x => x.PatientPCLRequestIndicators));
            }
            RefMedicalServiceGroupObj.RefMedicalServiceGroupItems = new List<RefMedicalServiceGroupItem>();
            if (mMedRegItemBases != null && mMedRegItemBases.Count() > 0)
            {
                //RefMedicalServiceGroupObj.RefMedicalServiceGroupItems.AddRange(mMedRegItemBases.Select(x => new RefMedicalServiceGroupItem(x)).ToList());
                foreach(var item in mMedRegItemBases)
                {
                    RefMedicalServiceGroupItem temp = new RefMedicalServiceGroupItem();
                    if (item is PatientRegistrationDetail)
                    {
                        var detailsItem = item as PatientRegistrationDetail;
                        temp.MedServiceID = ((RefMedicalServiceItem)detailsItem.ChargeableItem).MedServiceID;
                        temp.Qty = (int)detailsItem.Qty;
                    }
                    else if (item is PatientPCLRequestDetail)
                    {
                        var detailsItem = item as PatientPCLRequestDetail;
                        temp.PCLExamTypeID = ((PCLExamType)detailsItem.ChargeableItem).PCLExamTypeID;
                        temp.Qty = (int)item.Qty;
                    }
                    RefMedicalServiceGroupObj.RefMedicalServiceGroupItems.Add(temp);
                }
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginEditRefMedicalServiceGroup(RefMedicalServiceGroupObj,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try

                                {
                                    var mMedicalServiceGroupID = contract.EndEditRefMedicalServiceGroup(asyncResult);
                                    if (mMedicalServiceGroupID == 0)
                                    {
                                        MessageBox.Show(eHCMSResources.Z0477_G1_LuuKhongThanhCong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    }
                                    else
                                    {
                                        RefMedicalServiceGroupObj.MedicalServiceGroupID = mMedicalServiceGroupID;
                                        NotifyOfPropertyChange(() => RefMedicalServiceGroupObj);
                                        NotifyOfPropertyChange(() => SaveMedicalServiceGroupButtonTitle);
                                        LoadMedicalServiceGroupCollection();
                                        MessageBox.Show(eHCMSResources.A0468_G1_Msg_InfoLuuOK, "", MessageBoxButton.OK);
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
        private void GetAllMedicalServiceItemsByType(RefMedicalServiceType serviceType)
        {
            this.ShowBusyIndicator();
            Thread t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        long? serviceTypeID = null;
                        if (serviceType != null)
                        {
                            serviceTypeID = serviceType.MedicalServiceTypeID;
                        }
                        contract.BeginGetAllMedicalServiceItemsByType(serviceTypeID, null, null,
                            Globals.DispatchCallback(asyncResult =>
                            {
                                try
                                {
                                    IList<RefMedicalServiceItem> allItem = contract.EndGetAllMedicalServiceItemsByType(asyncResult);
                                    if (allItem != null)
                                    {
                                        AllRefMedicalServiceItems = allItem.ToObservableCollection();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                            }), serviceType);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        private ObservableCollection<DeptLocation> GetDefaultLocationsByServiceIDFromCatche(long medServiceID)
        {
            if (AllRefMedicalServiceItems.Any(o => o.MedServiceID == medServiceID) == false)
            {
                return null;
            }
            var theItem = (from medItem in AllRefMedicalServiceItems
                           where medItem.MedServiceID == medServiceID && medItem.allDeptLocation != null && medItem.allDeptLocation.Count > 0
                           select medItem).SingleOrDefault();
            if (theItem == null || theItem.allDeptLocation == null || theItem.allDeptLocation.Count == 0)
            {
                return null;
            }
            var DeptLocations = new ObservableCollection<DeptLocation>(AllRefMedicalServiceItems.Where(o => o.MedServiceID == medServiceID && o.allDeptLocation != null && o.allDeptLocation.Count > 0).ToList()[0].allDeptLocation);
            if (DeptLocations.Count > 1)
            {
                DeptLocation defaultDeptLoc = new DeptLocation()
                {
                    Location = new Location()
                    {
                        LocationName = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0606_G1_TuDongPhBoPg)
                    },
                    DeptLocationID = -1
                };
                DeptLocations.Insert(0, defaultDeptLoc);
            }
            return DeptLocations;
        }
        public void ApplyRefMedicalServiceGroup(RefMedicalServiceGroups aRefMedicalServiceGroupObj, DeptLocation aDeptLocation, bool IsAddNewOnly = false, Staff staff = null)
        {
            RefMedicalServiceGroupObj = aRefMedicalServiceGroupObj;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefMedicalServiceGroupItemsByID(RefMedicalServiceGroupObj.MedicalServiceGroupID,
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var mItemCollection = contract.EndGetRefMedicalServiceGroupItemsByID(asyncResult);
                                    if (!IsAddNewOnly)
                                    {
                                        CurrentRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                                        CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                                    }
                                    else
                                    {
                                        if (CurrentRegistration.PatientRegistrationDetails == null)
                                        {
                                            CurrentRegistration.PatientRegistrationDetails = new ObservableCollection<PatientRegistrationDetail>();
                                        }
                                        if (CurrentRegistration.PCLRequests == null)
                                        {
                                            CurrentRegistration.PCLRequests = new ObservableCollection<PatientPCLRequest>();
                                        }
                                    }
                                    if (mItemCollection != null)
                                    {
                                        RefMedicalServiceGroupObj.RefMedicalServiceGroupItems = mItemCollection.ToList();
                                        foreach (var item in RefMedicalServiceGroupObj.RefMedicalServiceGroupItems.Where(x => x.MedServiceID.HasValue && x.MedServiceID > 0))
                                        {
                                            //▼===== 20200222 TTM:  Không được phép gán đại cho Dịch vụ là khám bệnh vì sẽ sai trong trường hợp Thanh vũ => Dịch vụ khám bệnh chặt nửa giá.
                                            //                      Dẫn đến khi đưa gói mà trong gói có dịch vụ không phân biệt dịch vụ nào hết đều bị chặt giá.
                                            //                      Mà cho dù không phải Thanh Vũ thì cũng không được phép gán đại 1 giá trị cho 1 giá trị đã có dữ liệu từ DB.
                                            //                      Mà phải sửa từ gốc là việc đọc dữ liệu không thành công do sai code.
                                            //item.RefMedicalServiceItemObj.RefMedicalServiceType = new RefMedicalServiceType { V_RefMedicalServiceTypes = (long)AllLookupValues.V_RefMedicalServiceTypes.KHAMBENH };
                                            //▲===== 20200222 TTM:
                                            DeptLocation mDefaultLocation = null;
                                            item.RefMedicalServiceItemObj.allDeptLocation = GetDefaultLocationsByServiceIDFromCatche(item.RefMedicalServiceItemObj.MedServiceID);
                                            if (item.RefMedicalServiceItemObj.allDeptLocation != null && item.RefMedicalServiceItemObj.allDeptLocation.Count == 1)
                                            {
                                                mDefaultLocation = item.RefMedicalServiceItemObj.allDeptLocation.FirstOrDefault();
                                            }
                                            Coroutine.BeginExecute(DoAddRegItem(item.RefMedicalServiceItemObj, mDefaultLocation != null ? mDefaultLocation : aDeptLocation, item.RefMedicalServiceItemObj.RefMedicalServiceType
                                                , null, null, null, null, null, null, false, item.Qty));
                                        }
                                        foreach (var item in RefMedicalServiceGroupObj.RefMedicalServiceGroupItems.Where(x => x.PCLExamTypeID.HasValue && x.PCLExamTypeID > 0))
                                        {
                                            //▼====#025 Thêm Staff khi chỉ định gói
                                            if (Globals.ListPclExamTypesAllPCLFormImages.Any(x => x.PCLExamTypeID == item.PCLExamTypeID))
                                            {
                                                Coroutine.BeginExecute(DoAddPclExamType(Globals.ListPclExamTypesAllPCLFormImages.Where(x => x.PCLExamTypeID == item.PCLExamTypeID).FirstOrDefault(), null, staff));
                                            }
                                            if (Globals.ListPclExamTypesAllPCLForms.Any(x => x.PCLExamTypeID == item.PCLExamTypeID))
                                            {
                                                Coroutine.BeginExecute(DoAddPclExamType(Globals.ListPclExamTypesAllPCLForms.Where(x => x.PCLExamTypeID == item.PCLExamTypeID).FirstOrDefault(), null, staff));
                                            }
                                            else if (Globals.ListPclExamTypesAllCombos.Any(x => x.PCLExamTypeID == item.PCLExamTypeID))
                                            {
                                                Coroutine.BeginExecute(DoAddPclExamType(Globals.ListPclExamTypesAllCombos.Where(x => x.PCLExamTypeID == item.PCLExamTypeID).FirstOrDefault().PCLExamType, null, staff));
                                            }
                                            //▲====#025
                                        }
                                    }
                                    InitViewForServiceItems();
                                    InitViewForPCLRequests();
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
        private List<RefMedicalServiceGroups> _MedicalServiceGroupCollection;
        public List<RefMedicalServiceGroups> MedicalServiceGroupCollection
        {
            get => _MedicalServiceGroupCollection; set
            {
                _MedicalServiceGroupCollection = value;
                NotifyOfPropertyChange(() => MedicalServiceGroupCollection);
            }
        }
        private void LoadMedicalServiceGroupCollection()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginGetRefMedicalServiceGroups("",
                            Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    MedicalServiceGroupCollection = new List<RefMedicalServiceGroups>();
                                    var mItemCollection = contract.EndGetRefMedicalServiceGroups(asyncResult);
                                    if (mItemCollection != null)
                                    {
                                        MedicalServiceGroupCollection = mItemCollection.ToList();
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

        private bool _IsFinalization = false;
        public bool IsFinalization
        {
            get => _IsFinalization; set
            {
                _IsFinalization = value;
                NotifyOfPropertyChange(() => IsFinalization);
            }
        }

        public void btnFinalization()
        {
            if (CurrentRegistration == null || CurrentRegistration.Patient == null || Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.Staff == null)
            {
                return;
            }
            //▼====: #019
            GlobalsNAV.ShowDialog((IEditOutPtTransactionFinalization aView) =>
            {
                aView.Registration = CurrentRegistration;
                aView.TransactionFinalizationObj = new OutPtTransactionFinalization
                {
                    TaxMemberName = CurrentRegistration.Patient.FullName,
                    TaxMemberAddress = CurrentRegistration.Patient.PatientFullStreetAddress,
                    StaffID = Globals.LoggedUserAccount.Staff.StaffID,
                    CreatedStaff = Globals.LoggedUserAccount.Staff,
                    PtRegistrationID = CurrentRegistration.PtRegistrationID,
                    PatientFullName = CurrentRegistration.Patient == null ? null : CurrentRegistration.Patient.FullName,
                    V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT,
                    V_RegistrationType = (long)AllLookupValues.RegistrationType.NGOAI_TRU,
                    Buyer = (CurrentRegistration.Patient != null && !string.IsNullOrEmpty(CurrentRegistration.Patient.PatientEmployer)) ? CurrentRegistration.Patient.PatientEmployer : null
                };
            });
            //▲====: #019
        }
        //▼====== #003
        private string _BasicDiagTreatment = "";
        public string BasicDiagTreatment
        {
            get
            {
                return _BasicDiagTreatment;
            }
            set
            {
                _BasicDiagTreatment = value;
            }
        }
        //▲====== #003
        private ObservableCollection<RefMedicalServiceItem> _AllRefMedicalServiceItems;
        public ObservableCollection<RefMedicalServiceItem> AllRefMedicalServiceItems
        {
            get { return _AllRefMedicalServiceItems; }
            set
            {
                _AllRefMedicalServiceItems = value;
                NotifyOfPropertyChange(() => AllRefMedicalServiceItems);
            }
        }

        public void btnCancelService()
        {
            this.ShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            Coroutine.BeginExecute(DoSaveAndPayOldServices(false), null, (o, e) =>
            {
                this.HideBusyIndicator();
            });
        }

        IScanImageCapture theScanImageCaptureDlg = null;
        private List<ScanImageFileStorageDetail> NewScanImageFilesToBeSave = null;
        private List<ScanImageFileStorageDetail> ScanImageFilesToDeleted = new List<ScanImageFileStorageDetail>();
        private Patient CurrentPatient { get { return CurrentRegistration == null ? null : CurrentRegistration.Patient; } }
        private long GetPtRegistrationID_ForScanningStuff()
        {
            long PtRegistrationID = 0;
            DateTime dtToday = Globals.GetCurServerDateTime();
            bool bReload_Of_Registration_DoneToday = CurrentRegistration == null && CurrentPatient != null && CurrentPatient.LatestRegistration != null && (CurrentPatient.LatestRegistration.ExamDate.Date == dtToday.Date);
            if (bReload_Of_Registration_DoneToday)
            {
                PtRegistrationID = CurrentPatient.LatestRegistration.PtRegistrationID;
            }
            else
            {
                // Registration has just been done
                PtRegistrationID = (CurrentRegistration != null && (CurrentRegistration.PtRegistrationID > 0 && CurrentRegistration.ExamDate.Date == dtToday.Date) ? CurrentRegistration.PtRegistrationID : 0);
            }
            return PtRegistrationID;
        }
        public void DoScanCmd()
        {
            //long PtRegistrationID = GetPtRegistrationID_ForScanningStuff();
            //if (PtRegistrationID == 0)
            //    return;
            if (CurrentRegistration == null || CurrentPatient == null || CurrentRegistration.PtRegistrationID == 0
                || CurrentPatient.PatientID == 0
                || string.IsNullOrEmpty(CurrentPatient.PatientCode))
            {
                return;
            }
            theScanImageCaptureDlg = Globals.GetViewModel<IScanImageCapture>();
            Action<IScanImageCapture> onInitDlg = delegate (IScanImageCapture vm)
            {
                vm.PatientID = (CurrentPatient != null ? CurrentPatient.PatientID : 0);
                vm.PatientCode = (CurrentPatient != null ? CurrentPatient.PatientCode : "");
                vm.PtRegistrationID = CurrentRegistration.PtRegistrationID;
            };
            GlobalsNAV.ShowDialog_V3(theScanImageCaptureDlg, onInitDlg, null);
            NewScanImageFilesToBeSave = theScanImageCaptureDlg.ScanImageFileToBeSaved;
            ScanImageFilesToDeleted = theScanImageCaptureDlg.ScanImageFileToBeDeleted;
        }
        public void SaveScanCmd()
        {
            long PtRegistrationID = GetPtRegistrationID_ForScanningStuff();
            if (PtRegistrationID == 0)
                return;
            SaveScanImageFiletoServer(PtRegistrationID);
        }
        private void SaveScanImageFiletoServer(long PtRegistrationID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PCLsImportClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    try
                    {
                        contract.BeginAddScanFileStorageDetails(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), CurrentPatient.PatientID, PtRegistrationID,
                            CurrentPatient.PatientCode, NewScanImageFilesToBeSave, ScanImageFilesToDeleted, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    bool resOK = contract.EndAddScanFileStorageDetails(asyncResult);
                                    if (resOK)
                                    {
                                        GlobalsNAV.ShowMessagePopup(eHCMSResources.Z1562_G1_DaLuu);
                                        // MessageBox.Show(eHCMSResources.Z1562_G1_DaLuu);
                                        Globals.EventAggregator.Publish(new ReloadOutStandingStaskPCLRequest());
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ClientLoggerHelper.LogError(ex.ToString());
                                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                }
                                finally
                                {
                                    this.HideBusyIndicator();
                                }
                            }), null);
                    }
                    catch (Exception ex)
                    {
                        ClientLoggerHelper.LogError(ex.ToString());
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();

        }
        private List<RefTreatmentRegimen> _TreatmentRegimenCollection = new List<RefTreatmentRegimen>();
        public List<RefTreatmentRegimen> TreatmentRegimenCollection
        {
            get
            {
                return _TreatmentRegimenCollection;
            }
            set
            {
                _TreatmentRegimenCollection = value;
            }
        }
        private bool _IsUserAdmin = !Globals.isAccountCheck;
        public bool IsUserAdmin
        {
            get
            {
                return _IsUserAdmin;
            }
            set
            {
                if (_IsUserAdmin == value)
                {
                    return;
                }
                _IsUserAdmin = value;
                NotifyOfPropertyChange(() => IsUserAdmin);
            }
        }

        private IOutPatientBillingManage _NewBillingContent;
        public IOutPatientBillingManage NewBillingContent
        {
            get
            {
                return _NewBillingContent;
            }
            set
            {
                _NewBillingContent = value;
                NotifyOfPropertyChange(() => NewBillingContent);
            }
        }

        private IOutPatientBillingManage _OldBillingContent;
        public IOutPatientBillingManage OldBillingContent
        {
            get
            {
                return _OldBillingContent;
            }
            set
            {
                _OldBillingContent = value;
                NotifyOfPropertyChange(() => OldBillingContent);
            }
        }
        private bool _RegistrationView = false;
        public bool RegistrationView
        {
            get { return _RegistrationView; }
            set
            {
                _RegistrationView = value;
                NotifyOfPropertyChange(() => RegistrationView);
            }
        }
        //▼===== 20190105 TTM: Thay đổi thông tin tích BH và mở các nút lên để cập nhật.
        public void Handle(ChangePCLHIStatus<PatientPCLRequestDetail> message)
        {
            if (GetView() != null)
            {
                PatientPCLRequestDetail details = message.Item;
                DoChangePCLHIStatusPatientRegistrationDetails(details);
            }
        }
        private void DoChangePCLHIStatusPatientRegistrationDetails(PatientPCLRequestDetail details)
        {
            OnDetailsChanged();
            InitViewForPCLRequests();
        }
        //▲===== 
        //20200222 TBL Mod TMV1: Mở popup danh sách dịch vụ của gói
        public void AddPackageService()
        {
            if (CurrentPatient == null || CurrentPatient.PatientID == 0 || string.IsNullOrEmpty(CurrentPatient.PatientCode))
            {
                return;
            }
            if (SearchRegCriteria == null)
            {
                SearchRegCriteria = new SeachPtRegistrationCriteria();
            }
            SearchRegCriteria.PatientCode = CurrentPatient.PatientCode;
            SearchRegistrationDetails(0, 10, true);
        }
        private void SearchRegistrationDetails(int pageIndex, int pageSize, bool bCountTotal)
        {
            this.ShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0601_G1_DangTimDK));
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchRegistrationsForProcess(SearchRegCriteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<PatientRegistrationDetail> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchRegistrationsForProcess(out totalCount, asyncResult);
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
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                            if (bOK)
                            {
                                SearchRegCriteria.FromDate = null;
                                SearchRegCriteria.ToDate = null;
                                Action<IFindPackageServiceDetail> onInitDlg = delegate (IFindPackageServiceDetail vm)
                                {
                                    vm.SearchCriteria = SearchRegCriteria;
                                    vm.IsPopup = true;
                                    vm.CopyExistingPatientList(allItems, _searchRegCriteria, totalCount);
                                };
                                GlobalsNAV.ShowDialog<IFindPackageServiceDetail>(onInitDlg);
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        WarningWithConfirmMsgBoxTask warnConfDlg = null;
        private bool IsDiffBetweenRegistrationAndTicket = false;
        private TicketIssue _TicketIssue;
        public TicketIssue TicketIssueObj
        {
            get
            {
                return _TicketIssue;
            }
            set
            {
                _TicketIssue = value;
                NotifyOfPropertyChange(() => TicketIssueObj);
            }
        }
        //▼===== 20200430 TTM: Hàm gọi sang QMSService để đi cập nhật lại tình trạng của số thứ tự
        private void UpdateTicketStatusAfterRegister(string TicketNumberText, DateTime TicketGetTime)
        {

            var t = new Thread(() =>
            {
                this.ShowBusyIndicator();
                using (var mFactory = new QMSService.QMSServiceClient())
                {
                    try
                    {
                        var mContract = mFactory.ServiceInstance;
                        mContract.BeginUpdateTicketStatusAfterRegister(TicketNumberText, TicketGetTime, Globals.DeptLocation.DeptLocationID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                bool IsOK = mContract.EndUpdateTicketStatusAfterRegister(asyncResult);
                                if (IsOK)
                                {
                                    TicketIssueObj.V_TicketStatus = (int)V_TicketStatus_Enum.TKT_ALREADY_REGIS;
                                    IsDiffBetweenRegistrationAndTicket = false;
                                    NotifyOfPropertyChange(() => TicketIssueObj);
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
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0074_G1_I);
                        this.HideBusyIndicator();
                    }
                }
            });
            t.Start();
        }
        //▲====== 
        //▼===== #015
        public void ResetEkip(PatientRegistrationDetail RegistrationDetailItem)
        {
            if (RegistrationDetailItem != null && RegistrationDetailItem.V_EkipIndex != null)
            {
                if (RegistrationDetailItem.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.DauTien)
                {
                    long V_Ekip = RegistrationDetailItem.V_Ekip.LookupID;
                    foreach (PatientRegistrationDetail item in CurrentRegistration.AllSaveRegistrationDetails)
                    {
                        if (item.V_Ekip != null && item.V_Ekip.LookupID == V_Ekip)
                        {
                            item.V_Ekip = new Lookup();
                            item.V_EkipIndex = new Lookup();
                            item.HIPaymentPercent = 1;
                            CommonGlobals.ChangeItemsRecordState(item);
                            CommonGlobals.ChangeHIBenefit(CurrentRegistration, item, item);
                        }
                    }
                }
            }
        }
        //▲===== #015

        //▼===== #017
        private bool _IsRequestView = false;
        public bool IsRequestView
        {
            get { return _IsRequestView; }
            set
            {
                _IsRequestView = value;
                NotifyOfPropertyChange(() => IsRequestView);
            }
        }

        public void ViewPrintNewServiceCmd()
        {
            if (CurrentRegistration != null && CurrentRegistration.PatientRegistrationDetails != null && CurrentRegistration.PatientRegistrationDetails.Count > 0)
            {
                bool IsCanShowPrint = false;
                StringBuilder sb = new StringBuilder();
                sb.Append("<Root>");
                sb.Append("<ServiceReqIDList>");
                foreach (PatientRegistrationDetail item in CurrentRegistration.PatientRegistrationDetails)
                {
                    if (item.MedicalInstructionDate != null && item.PaidTime == null && RequestDoctorStaffID > 0 && RequestDoctorStaffID == item.DoctorStaffID)
                    {
                        sb.AppendFormat("<ServiceReqID>{0}</ServiceReqID>", item.PtRegDetailID);
                        if (!IsCanShowPrint)
                        {
                            IsCanShowPrint = true;
                        }
                    }
                }
                sb.Append("</ServiceReqIDList>");
                sb.Append("</Root>");

                if (!IsCanShowPrint)
                {
                    MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn);
                    return;
                }

                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.RegistrationID = (long)CurrentRegistration.PtRegistrationID;
                    proAlloc.Result = sb.ToString();
                    proAlloc.eItem = ReportName.RptPatientServiceRequestDetailsByPatientServiceReqID;
                    proAlloc.V_RegistrationType = (long)CurrentRegistration.V_RegistrationType;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }

        public void ViewPrintNewPCLCmd()
        {
            if (CurrentRegistration != null && CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Count > 0)
            {
                bool IsCanShowPrintPCL = false;
                StringBuilder sb = new StringBuilder();


                sb.Append("<Root>");
                foreach (var PCLRequest in CurrentRegistration.PCLRequests)
                {
                    if (PCLRequest.MedicalInstructionDate != null && PCLRequest.PaidTime == null && RequestDoctorStaffID > 0 && RequestDoctorStaffID == PCLRequest.DoctorStaffID)
                    {
                        sb.Append("<PCLReqIDList>");
                        sb.AppendFormat("<PCLReqID>{0}</PCLReqID>", PCLRequest.PatientPCLReqID);
                        sb.Append("</PCLReqIDList>");
                        if (!IsCanShowPrintPCL)
                        {
                            IsCanShowPrintPCL = true;
                        }
                    }
                }
                sb.Append("</Root>");


                if (!IsCanShowPrintPCL)
                {
                    MessageBox.Show(eHCMSResources.Z2850_G1_KhongCoGiDeXemIn);
                    return;
                }

                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    proAlloc.RegistrationID = (long)CurrentRegistration.PtRegistrationID;
                    proAlloc.Result = sb.ToString();
                    proAlloc.eItem = ReportName.PatientPCLRequestDetailsByPatientPCLReqID_XML;
                    proAlloc.V_RegistrationType = (long)CurrentRegistration.V_RegistrationType;
                    proAlloc.TieuDeRpt = eHCMSResources.P0381_G1_PhYeuCauCLS;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        //▲===== #017

        private long _RequestDoctorStaffID;
        public long RequestDoctorStaffID
        {
            get
            {
                return _RequestDoctorStaffID;
            }
            set
            {
                if (_RequestDoctorStaffID != value)
                {
                    _RequestDoctorStaffID = value;
                    NotifyOfPropertyChange(() => RequestDoctorStaffID);
                }
            }
        }

        //public bool IsPriority { get; set; }
        private bool _IsPriority;
        public bool IsPriority
        {
            get { return _IsPriority; }
            set
            {
                if (_IsPriority != value)
                {
                    _IsPriority = value;
                    NotifyOfPropertyChange(() => IsPriority);
                }
            }
        }

        //▼====#025
        private bool _IsFromRequestDoctor;
        public bool IsFromRequestDoctor
        {
            get { return _IsFromRequestDoctor; }
            set
            {
                if (_IsFromRequestDoctor != value)
                {
                    _IsFromRequestDoctor = value;
                    NotifyOfPropertyChange(() => IsFromRequestDoctor);
                }
            }
        }
        //▲====#025
        public void Handle(BasicDiagTreatmentChanged message)
        {
            if(message != null && message.aICD10Code != null)
            {
                CurrentRegistration.AdmissionICD10 = message.aICD10Code;
                CurrentRegistration.BasicDiagTreatment = message.aICD10Code.DiseaseNameVN;
                BasicDiagTreatment = message.aICD10Code.DiseaseNameVN;
            }

        }
    }
    internal class WaitForSaveRegistrationCompletedTask : IResult, IHandle<ItemLoaded<PatientRegistration, long>>
    {
        public PatientRegistration RegistrationInfo
        {
            get;
            private set;
        }
        private readonly long _registrationID;
        public WaitForSaveRegistrationCompletedTask(long regID, IEventAggregator eventAggregator)
        {
            _registrationID = regID;
            eventAggregator.Subscribe(this);
        }

        public void Execute(ActionExecutionContext context)
        {
            //Khong lam gi het. Cho bat duoc event load dang ky xong roi complete
        }

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public void Handle(ItemLoaded<PatientRegistration, long> message)
        {
            if (message.ID == _registrationID)
            {
                if (Completed != null)
                {
                    RegistrationInfo = message.Item;
                    Completed(this, new ResultCompletionEventArgs
                    {
                        Error = null,
                        WasCancelled = false
                    });
                }
            }

        }
    }
}
