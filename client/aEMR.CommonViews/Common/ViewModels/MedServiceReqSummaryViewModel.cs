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
using System.Text;
using aEMR.Common.BaseModel;

/*
 * 20181119 #001 TBL:   BM 0005283: Dich vu duoc chi dinh can biet duoc chi dinh tu khoa nao
 * 20181205 #002 TNHX:  [BM0005300] Add function View/Print PhieuChiDinh_DichVu
 * 20191121 #003 TTM:   BM 0019640: Lỗi thêm đc hơn 2 DV khám bệnh đc hưởng QL BHYT
 * 20200208 #004 TBL:   BM 0022891: Thay đổi cách lưu Ekip
 */
namespace aEMR.Common.ViewModels
{
    [Export(typeof(IMedServiceReqSummary)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedServiceReqSummaryViewModel : ViewModelBase, IMedServiceReqSummary
        , IHandle<RemoveItem<PatientRegistrationDetail>>
        , IHandle<RemoveItem<PatientPCLRequestDetail>>
        , IHandle<RemoveItem<PatientPCLRequest>>
        , IHandle<StateChanged<PatientPCLRequest>>
        , IHandle<StateChanged<PatientRegistrationDetail>>
        , IHandle<ItemSelected<Patient>>
        , IHandle<SetEkipForServiceSuccess>
    {
        IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public MedServiceReqSummaryViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _eventAggregator = eventAggregator;

            authorization();
            var newServiceVm = Globals.GetViewModel<IOutPatientServiceManage>();
            NewServiceContent = newServiceVm;
            NewServiceContent.IsOldList = false;
            ActivateItem(newServiceVm);

            var oldServiceVm = Globals.GetViewModel<IOutPatientServiceManage>();
            OldServiceContent = oldServiceVm;
            OldServiceContent.IsOldList = true;
            OldServiceContent.CanDelete = false;
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

            ShowCheckBoxColumn = true;

            //ShowAddRegisButton = Convert.ToBoolean(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.ShowAddRegisButton]);
            //AllowDuplicateMedicalServiceItems = Convert.ToInt16(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowDuplicateMedicalServiceItems]);

            // Txd 25/05/2014 Replaced ConfigList
            ShowAddRegisButton = Globals.ServerConfigSection.CommonItems.ShowAddRegisButton;
            AllowDuplicateMedicalServiceItems = Globals.ServerConfigSection.CommonItems.AllowDuplicateMedicalServiceItems;
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
        public IOutPatientDrugManage OldDrugContent { get; set; }

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
        public IOutPatientPclRequestManage OldPclContent { get; set; }
        public IOutPatientPclRequestManage NewPclContent { get; set; }
        public IOutPatientServiceManage OldServiceContent { get; set; }
        public IOutPatientServiceManage NewServiceContent { get; set; }
        public IPatientPayment OldPaymentContent { get; set; }

        private PatientRegistration _currentRegistration;

        public PatientRegistration CurrentRegistration
        {
            get { return _currentRegistration; }
            set
            {
                if (_currentRegistration != value)
                {
                    _currentRegistration = value;
                    NotifyOfPropertyChange(() => CurrentRegistration);
                    NotifyOfPropertyChange(() => CanNewCount15HIPercentCmd);
                    InitRegistration();
                }
            }
        }

        public bool CanAddService
        {
            get { return _currentTabIndex == 0 && IsInEditMode; }
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

        #region Tính tiền dự đoán
        //private string _TongGiaTien_Text;
        //public string TongGiaTien_Text
        //{
        //    get { return _TongGiaTien_Text; }
        //    set
        //    {
        //        if (_TongGiaTien_Text != value)
        //        {
        //            _TongGiaTien_Text = value;
        //            NotifyOfPropertyChange(() => TongGiaTien_Text);
        //        }
        //    }
        //}

        //private string _TongGiaTienBH_Text;
        //public string TongGiaTienBH_Text
        //{
        //    get { return _TongGiaTienBH_Text; }
        //    set
        //    {
        //        if (_TongGiaTienBH_Text != value)
        //        {
        //            _TongGiaTienBH_Text = value;
        //            NotifyOfPropertyChange(() => TongGiaTienBH_Text);
        //        }
        //    }
        //}

        //private string _TongGiaTienBN_Text;
        //public string TongGiaTienBN_Text
        //{
        //    get { return _TongGiaTienBN_Text; }
        //    set
        //    {
        //        if (_TongGiaTienBN_Text != value)
        //        {
        //            _TongGiaTienBN_Text = value;
        //            NotifyOfPropertyChange(() => TongGiaTienBN_Text);
        //        }
        //    }
        //}

        private void TinhTongGiaTien()
        {
            decimal tonggia = 0;
            decimal tonggiabh = 0;
            decimal tonggiabn = 0;
            if (_currentRegistration.PatientRegistrationDetails != null && _currentRegistration.PatientRegistrationDetails.Count > 0)
            {
                foreach (var serItem in _currentRegistration.PatientRegistrationDetails)
                {
                    if (serItem.RecordState != RecordState.DELETED && serItem.PaidTime == null)
                    {
                        // tonggia += serItem.InvoicePrice;
                        //tonggia += serItem.TotalPatientPayment;
                        tonggia += serItem.TotalInvoicePrice;
                        tonggiabh += serItem.TotalHIPayment;
                        tonggiabn += serItem.TotalPatientPayment;
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
                            //tonggia += pcl.InvoicePrice;
                            //tonggia += pcl.TotalPatientPayment;
                            tonggia += pcl.TotalInvoicePrice;
                            tonggiabh += pcl.TotalHIPayment;
                            tonggiabn += pcl.TotalPatientPayment;
                        }
                    }
                }
            }

            //TongGiaTien_Text = string.Format("{0}: ", eHCMSResources.G1472_G1_TCong) + tonggia.ToString("#,##0.##");
            //TongGiaTienBH_Text = string.Format("{0}: ", eHCMSResources.G0909_G1_TienBHTra) + tonggiabh.ToString("#,##0.##");
            //TongGiaTienBN_Text = string.Format("{0}: ", eHCMSResources.G1466_G1_TgBNTra) + tonggiabn.ToString("#,##0.##");
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

        private void InitRegistration()
        {
            if (CurrentRegistration != null)
            {
                CurrentRegistration.HIApprovedStaffID = Globals.LoggedUserAccount.StaffID;
                InitViewForServiceItems();
                InitViewForPCLRequests();
                InitViewForDrugItems();
                InitViewForPayments();
                //gIsHIUnder15Percent = CurrentRegistration.IsHIUnder15Percent;
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
                        if (item.PaidTime.HasValue) //Da tra tien
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
            TinhTongGiaTien();

        }

        private void InitViewForPayments()
        {
            var patientPayments = new List<PatientTransactionPayment>();

            if (CurrentRegistration != null && CurrentRegistration.PatientTransaction != null)
            {
                if (CurrentRegistration.PatientTransaction.PatientTransactionPayments != null)
                {
                    patientPayments.AddRange(CurrentRegistration.PatientTransaction.PatientTransactionPayments);
                }
            }

            OldPaymentContent.PatientPayments = patientPayments.ToObservableCollection();
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
        }

        public void SetRegistration(PatientRegistration registrationInfo)
        {
            CurrentRegistration = registrationInfo;
            RegistrationInfoHasChanged = false;
            IsInEditMode = false;
            IsShowCount15HIPercentCmd = CurrentRegistration.IsCrossRegion.GetValueOrDefault(true) ? false : true;
            if (NewServiceContent != null)
            {
                NewServiceContent.RegistrationObj = CurrentRegistration;
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
                return;
            }
            this.DlgShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            //▼===== #003
            if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurrentRegistration, null, CurrentRegistration.PatientRegistrationDetails))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                this.DlgHideBusyIndicator();
                return;
            }
            //▲===== #003
            _currentRegistration.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;

            Coroutine.BeginExecute(DoSaveNewServices(), null, (o, e) =>
            {
                this.DlgHideBusyIndicator();
            });
        }

        public bool CanSaveNewServicesAndPclCmd
        {
            get
            {
                return RegistrationInfoHasChanged; // && !IsSavingRegistration;
            }
        }

        /// <summary>
        /// Lưu và tính tiền luôn cho những dịch vụ mới thêm vào và những dịch vụ chưa trả tiền bị xóa.
        /// </summary>
        public void SaveAndPayForNewServiceCmd()
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "lưu và trả tiền cho bill"))
            {
                return;
            }
            this.ShowBusyIndicator("Đang lưu đăng ký");
            _currentRegistration.ReqFromDeptLocID = Globals.DeptLocation.DeptLocationID;

            Coroutine.BeginExecute(DoSaveAndPayNewServices(), null, (o, e) =>
                {
                    this.HideBusyIndicator();
                });
        }

        public bool CanSaveAndPayForNewServiceCmd
        {
            get { return RegistrationInfoHasChanged; } // && !IsSavingRegistration; }
        }


        /// <summary>
        /// Trả tiền cho những dịch vụ chưa được tính tiền (và được chọn)
        /// </summary>
        /// 
        public void PayForNewServiceCmd()
        {
            if (CurrentRegistration != null && Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "trả tiền bill"))
            {
                return;
            }
            Coroutine.BeginExecute(PayForNewService());
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
                        GetItemPrice(regDetail, CurrentRegistration);
                        GetItemTotalPrice(regDetail);
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
                            GetItemPrice(pclDetail, CurrentRegistration);
                            GetItemTotalPrice(pclDetail);
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
        }


        WarningWithConfirmMsgBoxTask warnOfHIExpired = null;

        public IEnumerator<IResult> PayForNewService()
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
            if (bMustRefund)
            {
                MessageBox.Show(eHCMSResources.A0456_G1_Msg_InfoHTienTruocTToan);
                yield break;
            }
            List<PatientRegistrationDetail> lsRegDetails = null;
            List<PatientPCLRequest> lsPclRequests = null;
            if (_currentRegistration.PatientRegistrationDetails != null)
            {
                lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.IsChecked && item.PaidTime == null &&
                                                                            (item.RecordState == RecordState.UNCHANGED || item.RecordState == RecordState.ADDED)).ToList();
            }
            if (_currentRegistration.PCLRequests != null)
            {
                lsPclRequests = _currentRegistration.PCLRequests.Where(item => item.IsChecked && item.PaidTime == null && item.RecordState == RecordState.UNCHANGED).ToList();
            }

            if ((lsRegDetails == null || lsRegDetails.Count == 0)
                && (lsPclRequests == null || lsPclRequests.Count == 0))
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0900_G1_Msg_InfoChonDVDeTinhTien));
                yield break;
            }

            Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
            {
                vm.Registration = _currentRegistration;
                vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.DANG_KY;
                vm.FormMode = PaymentFormMode.PAY;
                vm.PayNewService = true;

                vm.RegistrationDetails = lsRegDetails;
                vm.PclRequests = lsPclRequests;

                vm.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
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
                return;
            }
            //IsSavingRegistration = true;
            this.DlgShowBusyIndicator(eHCMSResources.Z1539_G1_DangLuuDK);
            //▼===== #003
            if (!Globals.CheckMaxNumberOfServicesAllowForOutPatient(CurrentRegistration, null, CurrentRegistration.PatientRegistrationDetails))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z2654_G1_TBSoLuongDichVuDangKyKhamToiDa, Globals.ServerConfigSection.OutRegisElements.MaxNumberOfServicesAllowForOutPatient, eHCMSResources.G0442_G1_TBao));
                this.DlgHideBusyIndicator();
                return;
            }
            //▲===== #003
            Coroutine.BeginExecute(DoSaveOldServices(), null, (o, e) =>
            {
                //IsSavingRegistration = false;
                this.DlgHideBusyIndicator();
            });
        }

        public bool CanUpdateRegistrationCmd
        {
            get { return RegistrationInfoHasChanged; } // && !IsSavingRegistration; }
        }

        /// <summary>
        /// Lưu và tính tiền cho đăng ký cũ (sau khi xóa dịch vụ đã trả tiền)
        /// </summary>
        public void SaveAndPayForOldServiceCmd()
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "lưu và trả tiền bill cũ"))
            {
                return;
            }
            //IsSavingRegistration = true;
            this.ShowBusyIndicator("Đang lưu đăng ký");
            Coroutine.BeginExecute(DoSaveAndPayOldServices(), null, (o, e) =>
            {
                //IsSavingRegistration = false;
                this.HideBusyIndicator();
            });
        }
        public bool CanSaveAndPayForOldServiceCmd
        {
            get { return RegistrationInfoHasChanged; } // && !IsSavingRegistration; }
        }

        /// <summary>
        /// Trả tiền cho những dịch vụ cũ (trả tiền chưa hết, hoàn tiền).
        /// Hiển thị tất cả những chi tiết đăng ký (dịch vụ, CLS, thuốc, y dụng cụ, hóa chất)
        /// </summary>
        public void PayForOldServiceCmd()
        {
            if (Globals.IsLockRegistration(CurrentRegistration.RegLockFlag, "trả tiền bill cũ"))
            {
                return;
            }
            //Neu co dich vu nao o trang thai hoan tien ma chua hoan tien thi hoan tien luon.
            Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
            {
                vm.Registration = _currentRegistration;
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

                    lsRegDetails = _currentRegistration.PatientRegistrationDetails.Where(item => item.PaidTime != null && item.RefundTime == null &&
                                                                                                   item.ExamRegStatus == AllLookupValues.ExamRegStatus.NGUNG_TRA_TIEN_LAI
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

                vm.RegistrationDetails = lsRegDetails;
                vm.PclRequests = lsPclRequests;

                vm.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
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
            }
        }
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
                OnDetailsChanged();
                InitViewForServiceItems();
                //--Đỉnh-- kiểm tra nếu xóa dịch vụ bảo hiểm chi trả, phải tính lại
                if (details.TotalHIPayment > 0 && details.PtRegDetailID < 1)
                {
                    RefreshRegistrationDetails();
                    CorrectRegistrationDetails();
                }
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
                    details.RecordState = RecordState.DELETED;
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
                OnDetailsChanged();
                InitViewForServiceItems();
                //--Đỉnh-- kiểm tra nếu xóa dịch vụ bảo hiểm chi trả, phải tính lại
                if (details.TotalHIPayment > 0 && details.PtRegDetailID < 1)
                {
                    RefreshRegistrationDetails();
                    CorrectRegistrationDetails();
                }
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
        private IEnumerator<IResult> DoAddRegItem(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, Staff staff, DateTime? dt, string diagnosis)
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

                AddNewRegistrationDetails(serviceItem, deptLoc, serviceType, staff, dt, diagnosis);

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
                            GetItemPrice(item, CurrentRegistration);
                            GetItemTotalPrice(item);
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
        private PatientPCLRequestDetail CreatePatientPCLRequestDetail(PCLExamType examType, DeptLocation deptLoc)
        {
            var item = new PatientPCLRequestDetail
            {
                StaffID = Globals.LoggedUserAccount.StaffID,
                MedProductType = AllLookupValues.MedProductType.CAN_LAM_SANG,
                PCLExamType = examType,
                Qty = 1,
                DeptLocation = deptLoc,
                HIAllowedPrice = examType.HIAllowedPrice,
                InvoicePrice = CurrentRegistration.HealthInsurance != null ? examType.HIPatientPrice : examType.NormalPrice
            };

            GetItemPrice(item, CurrentRegistration);
            GetItemTotalPrice(item);
            return item;
        }
        private PatientPCLRequest CreateTempPatientPCLRequest()
        {
            //Tim xem co request moi nao chua. Neu chua co thi tao moi. Neu co roi thi thoi.
            PatientPCLRequest tempRequest = _currentRegistration.PCLRequests.Where(p => p.V_PCLRequestType == AllLookupValues.V_PCLRequestType.NGOAI_TRU
                                                                                                && p.RecordState == RecordState.DETACHED).FirstOrDefault();
            if (tempRequest == null)
            {
                tempRequest = new PatientPCLRequest
                {
                    PatientPCLRequestIndicators = new ObservableCollection<PatientPCLRequestDetail>(),
                    Diagnosis = eHCMSResources.Z1116_G1_ChuaXacDinh,
                    StaffID = Globals.LoggedUserAccount.StaffID,
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

        private IEnumerator<IResult> DoAddPclExamType(PCLExamType examType, DeptLocation deptLoc)
        {
            if (_currentRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1200_G1_DKDaBiHuyKgTheThemCLS, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            var item = CreatePatientPCLRequestDetail(examType, deptLoc);

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

            var tempRequest = CreateTempPatientPCLRequest();
            tempRequest.PatientPCLRequestIndicators.Add(item);

            PermissionManager.ApplyPermissionToPclRequest(tempRequest);

            InitViewForPCLRequests();
            OnDetailsChanged();

            yield break;
        }

        private IEnumerator<IResult> DoAddAllPclExamType(ObservableCollection<PCLExamType> AllExamType)
        {
            if (_currentRegistration.RegistrationStatus == AllLookupValues.RegistrationStatus.REFUND)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1200_G1_DKDaBiHuyKgTheThemCLS, eHCMSResources.G0442_G1_TBao);
                yield return _msgTask;
                yield break;
            }

            //Kiem tra nhu vay moi dung????da thuc hien roi co thong bao hay ko ta?
            var listchuathuchien = (from c in _currentRegistration.PCLRequests
                                    where c.RecordState != RecordState.DELETED //c.V_PCLRequestStatus == AllLookupValues.V_PCLRequestStatus.OPEN &&
                                    select c);
            string duplicateExamtype = "";
            var tempRequest = CreateTempPatientPCLRequest();
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

            PermissionManager.ApplyPermissionToPclRequest(tempRequest);
            InitViewForPCLRequests();
            OnDetailsChanged();
            if (CurrentRegistration.PCLRequests != null && CurrentRegistration.PCLRequests.Count > 0 && CanSaveNewServicesAndPclCmd == false)
            {
                Globals.HIRegistrationForm = "";
            }
            else
            {
                Globals.HIRegistrationForm = string.Format(eHCMSResources.Z1126_G1_ChuaLuuDKBN, CurrentRegistration.Patient.FullName);
            }
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

        private IEnumerator<IResult> DoSaveNewServices()
        {
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateNewServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;

            if (!result.IsValid)
            {
                yield break;
            }
            //Se dang ky ngay o cho nay.
            //validate lại chỗ này- cho trường hợp load đăng ký từ cuộc hẹn
            //CorrectRegistrationDetails();
            var newServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var newPclRequestList = new ObservableCollection<PatientPCLRequest>(); //_currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var deletedServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList();

            var deletedPclRequestList = new List<PatientPCLRequest>(); //_currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED).ToList();


            if (newServiceList.Count == 0
                && newPclRequestList.Count == 0 && deletedServiceList.Count == 0 && deletedPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1202_G1_KgCoThayDoi), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
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



            //list request goi xuong la tat ca cac request chua tra tien, tra roi khong ban lam gi nua
            var listPCLRequest = new List<PatientPCLRequest>();
            //(from c in _currentRegistration.PCLRequests
            //                  where c.PaidTime == null
            //                  select c);

            foreach (PatientRegistrationDetail detail in newServiceList)
            {
                detail.ServiceRecID = ServiceRecID;
            }
            //Lưu xuống ở đây -2
            var regTask = new AddUpdateNewRegItemTask(_currentRegistration, newServiceList, listPCLRequest.ToList(), deletedServiceList, deletedPclRequestList, null);
            yield return regTask;
            if (!string.IsNullOrEmpty(regTask.ErrorMesage))
            {
                var message = new MessageWarningShowDialogTask(regTask.ErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = _currentRegistration });
                yield break;
            }
            if (regTask.Error != null)
            {
                //Thong bao loi.
                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(regTask.Error) });
                yield break;
            }

            CurrentRegistration = regTask.CurRegistration;

            //Thanh cong roi.
            Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = regTask.CurRegistration, RefreshItemFromReturnedObj = true });
            CorrectRegistrationDetails();
            BeginEdit();
            // #002
            CanViewPrintNewServiceCmd = true;
            // #002
        }


        private IEnumerator<IResult> DoSaveAndPayNewServices()
        {
            //kiem tra dang ky nay da dc dang ky bao hiem chua?neu roi thi ko cho dang ky bao hiem nua
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateNewServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;

            if (!result.IsValid)
            {
                yield break;
            }
            //Se dang ky ngay o cho nay.
            var newServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var newPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var deletedServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList();

            var deletedPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED).ToList();

            if (newServiceList.Count == 0
                && newPclRequestList.Count == 0 && deletedServiceList.Count == 0 && deletedPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(eHCMSResources.Z1203_G1_KgTheLuuVaTraTien, eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
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

            //Lưu xuống ở đây -3
            var regTask = new AddUpdateNewRegItemTask(_currentRegistration, newServiceList, listPCLRequest.ToList(), deletedServiceList, deletedPclRequestList, null);
            yield return regTask;
            if (!string.IsNullOrEmpty(regTask.ErrorMesage))
            {
                var message = new MessageWarningShowDialogTask(regTask.ErrorMesage, "", false);
                yield return message;
                Globals.EventAggregator.Publish(new ItemSelected<PatientRegistration> { Item = _currentRegistration });
                yield break;
            }
            if (regTask.Error != null)
            {
                //Thong bao loi.
                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(regTask.Error) });
                yield break;
            }

            CurrentRegistration = regTask.CurRegistration;

            //Thanh cong roi.
            Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = regTask.CurRegistration, RefreshItemFromReturnedObj = true });

            //BeginEdit();
            //_waitForSaveCompletedTask = new WaitForSaveRegistrationCompletedTask(regTask.CurRegistration.PtRegistrationID);
            //yield return _waitForSaveCompletedTask;
            //if (_waitForSaveCompletedTask.RegistrationInfo != null)
            //{

            PayForNewServiceCmd();

            //}

        }


        private IEnumerator<IResult> CountAgainDoSaveAndPayNewServices()
        {
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateNewServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;

            if (!result.IsValid)
            {
                yield break;
            }
            //Se dang ky ngay o cho nay.
            var newServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var newPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.ADDED || item.RecordState == RecordState.DETACHED).ToList();

            var deletedServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.DELETED).ToList();

            var deletedPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.DELETED || item.RecordState == RecordState.MODIFIED).ToList();

            //list request goi xuong la tat ca cac request chua tra tien, tra roi khong tinh toi khong lam gi nua
            var listPCLRequest = (from c in _currentRegistration.PCLRequests
                                  where c.PaidTime == null
                                  select c);


            //Lưu xuống ở đây -1
            var regTask = new AddUpdateNewRegItemTask(_currentRegistration, newServiceList, listPCLRequest.ToList(), deletedServiceList, deletedPclRequestList, Apply15HIPercent);
            yield return regTask;
            if (regTask.Error != null)
            {
                //Thong bao loi.
                Globals.EventAggregator.Publish(new ErrorOccurred { CurrentError = new AxErrorEventArgs(regTask.Error) });
                yield break;
            }

            CurrentRegistration = regTask.CurRegistration;

            //Thanh cong roi.
            Globals.EventAggregator.Publish(new AddCompleted<PatientRegistration> { Item = regTask.CurRegistration, RefreshItemFromReturnedObj = true });
            //BeginEdit();
            //_waitForSaveCompletedTask = new WaitForSaveRegistrationCompletedTask(regTask.CurRegistration.PtRegistrationID);
            //yield return _waitForSaveCompletedTask;
            //if (_waitForSaveCompletedTask.RegistrationInfo != null)
            //{                
            PayForNewServiceCmd();
            //}

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
            //Se dang ky ngay o cho nay.
            var oldServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            var oldPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            if (oldServiceList.Count == 0 && oldPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1202_G1_KgCoThayDoi), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;

            }
            var removeOldServiceTask = new RemoveOldRegItemTask(_currentRegistration, oldServiceList, oldPclRequestList, null);
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
        }

        private IEnumerator<IResult> DoSaveAndPayOldServices()
        {
            var result = new YieldValidationResult();
            IEnumerator e = DoValidateOldServices(result);

            while (e.MoveNext())
                yield return e.Current as IResult;
            if (!result.IsValid)
            {
                yield break;
            }
            //Se dang ky ngay o cho nay.
            var oldServiceList = _currentRegistration.PatientRegistrationDetails.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            var oldPclRequestList = _currentRegistration.PCLRequests.Where(item => item.RecordState == RecordState.MODIFIED).ToList();

            if (oldServiceList.Count == 0 && oldPclRequestList.Count == 0)
            {
                _msgTask = new MessageBoxTask(string.Format("{0}.", eHCMSResources.Z1202_G1_KgCoThayDoi), eHCMSResources.G0442_G1_TBao, aEMR.Infrastructure.MessageBoxOptions.OkCancel);
                yield return _msgTask;
                yield break;

            }
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
            if (_waitForSaveCompletedTask.RegistrationInfo != null)
            {
                PayForOldServiceCmd();
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
            if (_tempRegistration != null)
            {
                IsInEditMode = false;
                CurrentRegistration = _tempRegistration;
                _tempRegistration = null;

                RegistrationInfoHasChanged = false;
                CurrentRegMode = _currentRegistration.PtRegistrationID > 0 ? RegistrationFormMode.OLD_REGISTRATION_OPENED : RegistrationFormMode.NEW_REGISTRATION_OPENED;
            }
        }


        protected void GetItemPriceNotAppliedBH(IInvoiceItem invoiceItem, PatientRegistration registration, double hiBenefit)
        {
            invoiceItem.HisID = null;
            invoiceItem.HIBenefit = null;

            if (invoiceItem.ChargeableItem != null)
            {
                invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
            }

            invoiceItem.PriceDifference = 0;
            invoiceItem.HIPayment = 0;
            invoiceItem.PatientCoPayment = 0;
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice;
        }

        protected void GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration registration, double hiBenefit)
        {
            // BN khong bao hiem
            if (registration.HisID == null || registration.HisID <= 0)
            {
                GetItemPriceNotAppliedBH(invoiceItem, registration, hiBenefit);
                return;
            }


            //KMx: Dịch vụ đã lưu nhưng chưa trả tiền thì cũng phải kiểm tra lại thẻ BH đã hết hạn chưa (11/11/2014 17:10).
            //if (invoiceItem.ID > 0)
            //{
            //    if (invoiceItem.HIBenefit.HasValue && invoiceItem.HIBenefit > 0.0)
            //    {
            //        hiBenefit = invoiceItem.HIBenefit.Value;
            //    }
            //}
            //else
            {
                //Doi voi item moi
                //Neu ngay tao dich vu khong nam trong thoi han cho phep cua the bao hiem thi khong tinh nua.
                //DateTime date = invoiceItem.CreatedDate.Date;
                //DateTime date = Globals.ServerDate.Value.Date;
                DateTime date = Globals.GetCurServerDateTime().Date;
                if (date > registration.HealthInsurance.ValidDateTo.GetValueOrDefault(DateTime.MaxValue).Date || date < registration.HealthInsurance.ValidDateFrom.GetValueOrDefault(DateTime.MinValue).Date)
                {
                    hiBenefit = 0.0;
                }

                if (invoiceItem.ChargeableItem != null)
                {
                    invoiceItem.HIBenefit = hiBenefit;
                    invoiceItem.HIAllowedPrice = invoiceItem.ChargeableItem.HIAllowedPrice;

                    //Kiểm tra nếu có sử dụng bảo hiểm + dịch vụ có bảo hiểm thì lấy giá của dịch vụ bằng với giá bảo hiểm.
                    //Nếu không thì lấy giá bình thường.
                    if (hiBenefit > 0 && invoiceItem.ChargeableItem.HIPatientPrice > 0)
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.HIPatientPrice;
                    }
                    else
                    {
                        invoiceItem.InvoicePrice = invoiceItem.ChargeableItem.NormalPrice;
                    }
                }
            }

            if (hiBenefit <= 0.0 || !invoiceItem.HIAllowedPrice.HasValue)
            {
                invoiceItem.HIAllowedPrice = 0;
            }
            //Truong hop dac biet neu la thuoc noi tru thi de xuong duoi luon.

            if (!(invoiceItem is OutwardDrugClinicDept) && invoiceItem.HIAllowedPrice.Value == 0)
            {
                invoiceItem.HisID = null;
                invoiceItem.HIBenefit = 0;
            }
            else
            {
                if (!invoiceItem.HisID.HasValue)
                {
                    invoiceItem.HisID = registration.HisID;
                }
            }
            invoiceItem.PriceDifference = invoiceItem.InvoicePrice - invoiceItem.HIAllowedPrice.Value;
            invoiceItem.HIPayment = invoiceItem.HIAllowedPrice.Value * (decimal)hiBenefit;
            invoiceItem.PatientCoPayment = invoiceItem.HIAllowedPrice.Value - invoiceItem.HIPayment;
            invoiceItem.PatientPayment = invoiceItem.InvoicePrice - invoiceItem.HIPayment;
        }

        protected void GetItemPrice(IInvoiceItem invoiceItem, PatientRegistration registration)
        {
            GetItemPrice(invoiceItem, registration, registration.PtInsuranceBenefit.GetValueOrDefault(0.0));
        }

        protected void GetItemTotalPrice(IInvoiceItem invoiceItem)
        {
            //Tính tổng tiền cho mỗi InvoiceItem.
            // Chưa tính trường hợp: nhiều dịch vụ KHÁM BỆNH chỉ tính 1 lần

            invoiceItem.TotalCoPayment = (decimal)Math.Ceiling((double)(invoiceItem.PatientCoPayment * invoiceItem.Qty));
            invoiceItem.TotalHIPayment = (decimal)Math.Floor((double)(invoiceItem.HIPayment * invoiceItem.Qty));
            invoiceItem.TotalInvoicePrice = invoiceItem.InvoicePrice * (decimal)invoiceItem.Qty;
            invoiceItem.TotalPatientPayment = invoiceItem.TotalInvoicePrice - invoiceItem.TotalHIPayment; //invoiceItem.PatientPayment * (decimal)invoiceItem.Qty;
            invoiceItem.TotalPriceDifference = invoiceItem.PriceDifference * (decimal)invoiceItem.Qty;
        }
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
                    GetItemPrice(item, CurrentRegistration);
                    GetItemTotalPrice(item);
                    //cho thang dau tien roi dung lai
                    if (item.TotalHIPayment > 0)
                    {
                        break;
                    }
                }
            }
        }

        private void CorrectRegistrationDetails()
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
                //foreach (var registrationDetail in hiRegDetails)
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

                    GetItemPrice(registrationDetail, CurrentRegistration);
                    GetItemTotalPrice(registrationDetail);

                    if (registrationDetail.TotalCoPayment != totalCoPayment
                        || registrationDetail.TotalHIPayment != totalHiPayment
                        || hisID != registrationDetail.HisID
                        || benefit != registrationDetail.HIBenefit)
                    {
                        if (registrationDetail.RecordState == RecordState.UNCHANGED)
                        {
                            registrationDetail.RecordState = RecordState.MODIFIED;
                        }
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
                            registrationDetail.RecordState = RecordState.MODIFIED;
                        }
                    }
                }
            }

        }
        private void AddNewRegistrationDetails(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, Staff staff, DateTime? dt, string diagnosis)
        {
            var newRegistrationDetail = new PatientRegistrationDetail { RefMedicalServiceItem = serviceItem };
            newRegistrationDetail.RefMedicalServiceItem.RefMedicalServiceType = serviceType;
            newRegistrationDetail.DeptLocation = deptLoc != null && deptLoc.DeptLocationID > 0 ? deptLoc : null;
            newRegistrationDetail.EntityState = EntityState.DETACHED;
            newRegistrationDetail.ExamRegStatus = AllLookupValues.ExamRegStatus.DANG_KY_KHAM;
            newRegistrationDetail.CreatedDate = Globals.GetCurServerDateTime();

            newRegistrationDetail.Qty = 1;
            newRegistrationDetail.HisID = CurrentRegistration.HisID;
            newRegistrationDetail.RecordState = RecordState.ADDED;
            newRegistrationDetail.CanDelete = true;
            newRegistrationDetail.HIAllowedPrice = serviceItem.HIAllowedPrice;
            newRegistrationDetail.InvoicePrice = CurrentRegistration.HealthInsurance != null ? serviceItem.HIPatientPrice : serviceItem.NormalPrice;
            newRegistrationDetail.DoctorStaffID = staff.StaffID;
            newRegistrationDetail.MedicalInstructionDate = dt;
            //==== #001 ====
            newRegistrationDetail.ReqDeptID = Globals.DeptLocation.DeptID;
            //==== #001 ====
            newRegistrationDetail.Diagnosis = diagnosis; //20190402 TBL: Them truong chan doan de hien thi khi in phieu chi dinh dich vu
            GetItemPrice(newRegistrationDetail, CurrentRegistration);
            GetItemTotalPrice(newRegistrationDetail);

            CurrentRegistration.PatientRegistrationDetails.Add(newRegistrationDetail);

            CorrectRegistrationDetails();

            InitViewForServiceItems();

            OnDetailsChanged();
        }
        public void AddNewService(RefMedicalServiceItem serviceItem, DeptLocation deptLoc, RefMedicalServiceType serviceType, Staff staff, DateTime? dt, string diagnosis)
        {
            Coroutine.BeginExecute(DoAddRegItem(serviceItem, deptLoc, serviceType, staff, dt, diagnosis));
        }
        public void AddNewPclRequestDetailFromPclExamType(PCLExamType examType, DeptLocation deptLoc)
        {
            Coroutine.BeginExecute(DoAddPclExamType(examType, deptLoc));
        }

        public void AddNewAllPclRequestDetailFromPclExamType(ObservableCollection<PCLExamType> AllExamType)
        {
            Coroutine.BeginExecute(DoAddAllPclExamType(AllExamType));
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
            if (message != null)
            {
                ((TabItem)NewRegItemsTab).IsSelected = true;
            }
        }

        // #002
        public void ViewPrintNewServiceCmd()
        {
            //Generic list ServiceID
            if (CurrentRegistration != null && CurrentRegistration.PatientRegistrationDetails != null && CurrentRegistration.PatientRegistrationDetails.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<Root>");
                sb.Append("<ServiceReqIDList>");
                foreach (PatientRegistrationDetail item in CurrentRegistration.PatientRegistrationDetails)
                {
                    if(item.MedicalInstructionDate != null)
                    {
                        sb.AppendFormat("<ServiceReqID>{0}</ServiceReqID>", item.PtRegDetailID);
                    }
                }
                sb.Append("</ServiceReqIDList>");
                sb.Append("</Root>");

                void onInitDlg(ICommonPreviewView proAlloc)
                {
                    //proAlloc.PatientServiceReqID = CurrentRegistration.PatientRegistrationDetails;
                    proAlloc.RegistrationID = (long) CurrentRegistration.PtRegistrationID;
                    proAlloc.Result = sb.ToString();
                    proAlloc.eItem = ReportName.RptPatientServiceRequestDetailsByPatientServiceReqID;
                    proAlloc.V_RegistrationType = (long)CurrentRegistration.V_RegistrationType;
                }
                GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
            }
        }
        
        private bool _canViewPrintNewServiceCmd = false;
        public bool CanViewPrintNewServiceCmd
        {
            get
            {
                return _canViewPrintNewServiceCmd;
            }
            set
            {
                if (_canViewPrintNewServiceCmd == value)
                    return;
                _canViewPrintNewServiceCmd = value;
                NotifyOfPropertyChange(() => CanViewPrintNewServiceCmd);
            }
        }

        private bool _mPhieuChiDinh_In = false;
        public bool ShowPhieuChiDinh_In
        {
            get
            {
                return _mPhieuChiDinh_In;
            }
            set
            {
                if (_mPhieuChiDinh_In == value)
                    return;
                _mPhieuChiDinh_In = value;
                NotifyOfPropertyChange(() => ShowPhieuChiDinh_In);
            }
        }
        // #002
        //▼===== #004
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
                        if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.CungEkip)
                        {
                            detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault() * (decimal)Globals.ServerConfigSection.HealthInsurances.PercentForEkip;
                            detail.HIPaymentPercent = Math.Round(Globals.ServerConfigSection.HealthInsurances.PercentForEkip, 2);
                            detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                        }
                        else if (detail.V_EkipIndex != null && detail.V_EkipIndex.LookupID == (long)AllLookupValues.V_EkipIndex.KhacEkip)
                        {
                            detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault() * (decimal)Globals.ServerConfigSection.HealthInsurances.PercentForOtherEkip;
                            detail.HIPaymentPercent = Math.Round(Globals.ServerConfigSection.HealthInsurances.PercentForOtherEkip, 2);
                            detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                        }
                        else
                        {
                            detail.TotalHIPayment = detail.HIAllowedPrice.GetValueOrDefault() * detail.Qty * (decimal)detail.HIBenefit.GetValueOrDefault();
                            detail.HIPaymentPercent = 1;
                            detail.TotalPatientPayment = detail.InvoicePrice - detail.TotalHIPayment;
                        }
                    }
                }
            }
        }
        //▲===== #004
    }
}
