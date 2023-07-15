using System.ComponentModel.Composition;
using System.ServiceModel;
using aEMR.DataContracts;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using aEMR.Common;
using System.Windows.Media;
using aEMR.Pharmacy.Views;
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
using aEMR.Common.Converters;
using Castle.Windsor;
using System.Windows.Data;
using System.Text;
using aEMR.Common.ConfigurationManager.Printer;
/*
* 20140803 #001 CMN:    Add HI Store Service
* 20181018 #002 TTM:    BM0002188 Bỏ việc Focus trong AutoComplete khi người dùng tìm kiếm theo tên thuốc. Điều này không cần thiết vì sẽ gây khó khăn cho người dùng khi tìm kiếm thuốc 
*                       (Do tự động Focus và bôi đen => không gõ ký tự thứ 2 đc).
* 20181103 #003 TNHX:   [BM0005214] Update report PhieuNhanThuoc base RefApplicationConfig.MixedHIPharmacyStores
* 20181105 #004 TTM:    Bổ sung cho fix lỗi quyết toán 2 lần die chương trình
* 20181108 #005 TNHX:   [BM0005225] Khong in toa rong, popup sau khi tra tien hien thi giong khi nhan nut "In phieu nhan thuoc"
* 20181115 #006 TNHX:   [BM0005248] Print PhieuNhanThuoc after LoadPrescriptionDetailByInvoice also base on isConfirmHIView and Refactor code
* 20190422 #007 TNHX:   [BM0006716] Apply thermal report for Cashier at BHYT, change services at Pharmacy
* 20190521 #008 TNHX:   [BM0006874] Print Receipt at ConfirmHIView base on config PrintingReceiptWithDrugBill
* 20190730 #009 TTM:    BM 0013008: Sửa lỗi Focus của AutoComplete không chính xác.
* 20200628 #010 TTM:    Thêm điều kiện chặn nếu như đăng ký đã được xác nhận thì không cho phép huỷ phiếu xuất.
* 20220610 #011 DatTB: Thêm in report hướng dẫn sử dụng theo toa thuốc
*/

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPrescription)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PrescriptionViewModel : Common.ViewModels.PharmacyHandlePayCompletedViewModelBase, IPrescription
        , IHandle<PharmacyCloseSearchPrescriptionEvent>
        , IHandle<PharmacyCloseSearchPrescriptionInvoiceEvent>
        , IHandle<AddCompleted<List<OutwardDrugInvoice>>>
        , IHandle<ChooseBatchNumberVisitorEvent>
        , IHandle<ChooseBatchNumberVisitorResetQtyEvent>
        , IHandle<PayForRegistrationCompleted>
        , IHandle<ReLoadToaOfDuocSiEditEvent<Prescription>>
        , IHandle<PharmacyCloseFormReturnEvent>
        , IHandle<PharmacyCloseEditPayedPrescription>
        , IHandle<PharmacyPayEvent>
        , IHandle<DeletedOutwardDrug>
        , IHandle<ChangedQtyOutwardDrug>
        , IHandle<PayForRegistrationCompletedAtConfirmHIView>
        , IHandle<RemoveItem<InPatientBillingInvoice>>
    {
        private enum DataGridCol
        {
            ColDelete = 0,
            BH = 1,
            //MaThuoc = 2,
            TenThuoc = 2,
            HamLuong = 3,
            DVT = 4,
            SLYC = 5,
            ThucXuat = 6,
            LoSX = 7,
            DonGiaBan = 8,
            DonGiaBH = 9
        }
        #region Indicator Member

        private bool _isLoadingGetStore = false;
        public bool isLoadingGetStore
        {
            get { return _isLoadingGetStore; }
            set
            {
                if (_isLoadingGetStore != value)
                {
                    _isLoadingGetStore = value;
                    NotifyOfPropertyChange(() => isLoadingGetStore);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingInfoPatient = false;
        public bool isLoadingInfoPatient
        {
            get { return _isLoadingInfoPatient; }
            set
            {
                if (_isLoadingInfoPatient != value)
                {
                    _isLoadingInfoPatient = value;
                    NotifyOfPropertyChange(() => isLoadingInfoPatient);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetailPrescript = false;
        public bool isLoadingDetailPrescript
        {
            get { return _isLoadingDetailPrescript; }
            set
            {
                if (_isLoadingDetailPrescript != value)
                {
                    _isLoadingDetailPrescript = value;
                    NotifyOfPropertyChange(() => isLoadingDetailPrescript);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingFullOperator = false;
        public bool isLoadingFullOperator
        {
            get { return _isLoadingFullOperator; }
            set
            {
                if (_isLoadingFullOperator != value)
                {
                    _isLoadingFullOperator = value;
                    NotifyOfPropertyChange(() => isLoadingFullOperator);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingPrescriptID = false;
        public bool isLoadingPrescriptID
        {
            get { return _isLoadingPrescriptID; }
            set
            {
                if (_isLoadingPrescriptID != value)
                {
                    _isLoadingPrescriptID = value;
                    NotifyOfPropertyChange(() => isLoadingPrescriptID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingGetID = false;
        public bool isLoadingGetID
        {
            get { return _isLoadingGetID; }
            set
            {
                if (_isLoadingGetID != value)
                {
                    _isLoadingGetID = value;
                    NotifyOfPropertyChange(() => isLoadingGetID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingDetail = false;
        public bool isLoadingDetail
        {
            get { return _isLoadingDetail; }
            set
            {
                if (_isLoadingDetail != value)
                {
                    _isLoadingDetail = value;
                    NotifyOfPropertyChange(() => isLoadingDetail);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public bool IsLoading
        {
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingInfoPatient || isLoadingPrescriptID || isLoadingDetailPrescript || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        //==== #001
        private bool _IsHIOutPt = false;
        public bool IsHIOutPt
        {
            get { return _IsHIOutPt && Globals.ServerConfigSection.CommonItems.EnableHIStore; }
            set
            {
                if (_IsHIOutPt != value)
                {
                    _IsHIOutPt = value;
                    NotifyOfPropertyChange(() => IsHIOutPt);
                }
            }
        }

        private bool _AllowPayAfter = Globals.ServerConfigSection.CommonItems.EnablePayAfter;
        public bool AllowPayAfter { get { return IsHIOutPt && _AllowPayAfter; } }

        private IOutPatientServiceManage _oldServiceContent;
        public IOutPatientServiceManage OldServiceContent
        {
            get { return _oldServiceContent; }
            set { _oldServiceContent = value; }
        }

        private IOutPatientPclRequestManage _oldPclContent;
        public IOutPatientPclRequestManage OldPclContent
        {
            get { return _oldPclContent; }
            set { _oldPclContent = value; }
        }

        private IPatientPayment _oldPaymentContent;
        public IPatientPayment OldPaymentContent
        {
            get { return _oldPaymentContent; }
            set { _oldPaymentContent = value; }
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
        //==== #001

        //--▼--18/12/2020 DatTB
        private bool _Is_Enabled = false;
        public bool Is_Enabled
        {
            get
            {
                return _Is_Enabled;
            }
            set
            {
                if (_Is_Enabled != value)
                {
                    _Is_Enabled = value;
                    NotifyOfPropertyChange(() => Is_Enabled);
                }
            }
        }
        //--▲--18/12/2020 DatTB

        #endregion

        #region Khoi Tao Member
        private IPrescriptionView _currentView;
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _currentView = view as IPrescriptionView;
        }

        [ImportingConstructor]
        public PrescriptionViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            authorization();
            //==== #001
            //Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            if (Globals.ServerConfigSection.CommonItems.EnableOutPtCashAdvance || IsConfirmHIView)
            {
                var oldServiceVm = Globals.GetViewModel<IOutPatientServiceManage>();
                OldServiceContent = oldServiceVm;
                OldServiceContent.HiServiceBeingUsed = true;
                ActivateItem(oldServiceVm);

                var oldPclVm = Globals.GetViewModel<IOutPatientPclRequestManage>();
                OldPclContent = oldPclVm;
                OldPclContent.HiServiceBeingUsed = true;
                ActivateItem(oldPclVm);

                var oldPaymentVm = Globals.GetViewModel<IPatientPayment>();
                OldPaymentContent = oldPaymentVm;
                OldPaymentContent.ShowPrintColumn = true;
                ActivateItem(oldPaymentVm);

                OldBillingContent = Globals.GetViewModel<IOutPatientBillingManage>();
                ActivateItem(OldBillingContent);
            }
            //==== #001
            GetStaffLogin();

            RefeshData();
            SetDefaultForStore();
            Is_Enabled = false; //--18/12/2020 DatTB

        }

        protected override void OnActivate()
        {
            //==== #001
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            //==== #001
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Globals.EventAggregator.Unsubscribe(this);
        }

        //DateTime DateServer = DateTime.Now;
        DateTime DateServer = Globals.ServerDate.Value;
        private void RefeshData()
        {
            strNgayDung = "";
            SelectedOutInvoice = null;
            SelectedOutInvoice = new OutwardDrugInvoice();
            Seller = "";
            SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
            SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
            SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();

            SearchCriteria = null;
            SearchCriteria = new PrescriptionSearchCriteria();

            // TxD 02/08/2014: Use Globals Server Date instead
            //GetCurrentDate();
            DateTime todayDate = Globals.GetCurServerDateTime();
            SearchCriteria.FromDate = todayDate.AddDays(-1);
            SearchCriteria.ToDate = todayDate;
            DateServer = todayDate;

            SearchInvoiceCriteria = null;
            SearchInvoiceCriteria = new SearchOutwardInfo();
            SearchInvoiceCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;

            PatientInfo = null;
            NotifyOfPropertyChange(() => ValueQuyenLoiBH);
            SelectedPrescription = null;
            SelectedPrescription = new Prescription();
            OutwardDrugsCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }
            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;
            CurrentChoNhanThuoc = null; // #011
        }

        private void ClearData()
        {
            OutwardDrugsCopy = null;

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }
            if (GetDrugForSellVisitorListSum == null)
            {
                GetDrugForSellVisitorListSum = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorListSum.Clear();
            }

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorTemp.Clear();
            }
        }

        //vi non-shared 
        //protected override void OnActivate()
        //{
        //}

        //protected override void OnDeactivate(bool close)
        //{
        //    Globals.EventAggregator.Unsubscribe(this);

        //    SelectedOutInvoice = null;
        //    SearchCriteria = null;
        //    SearchInvoiceCriteria = null;
        //    OutwardDrugsCopy = null;
        //    PatientInfo = null;
        //    SelectedPrescription = null;
        //    GetDrugForSellVisitorList = null;
        //    GetDrugForSellVisitorTemp = null;
        //    GetDrugForSellVisitorListSum = null;
        //    StoreCbx = null;

        //    BatchNumberListTemp = null;
        //    BatchNumberListShow = null;
        //    OutwardDrugListByDrugID = null;
        //    OutwardDrugListByDrugIDFirst = null;
        //    SelectedOutwardDrug = null;
        //    if (au != null)
        //    {
        //        au.SetValue(AutoCompleteBox.ItemsSourceProperty, null);
        //    }

        //}
        #endregion

        #region Properties Member
        public string TitleForm { get; set; }
        public class ListDrugAndQtySell
        {
            public long DrugID;
            public int xban;
            public int QtyOffer;
        }
        private ObservableCollection<ListDrugAndQtySell> ListDrugTemp;

        private string _strNgayDung;
        public string strNgayDung
        {
            get { return _strNgayDung; }
            set
            {
                if (_strNgayDung != value)
                {
                    _strNgayDung = value;
                    NotifyOfPropertyChange(() => strNgayDung);
                }
            }
        }

        private bool _IsNotCountInsurance;
        public bool IsNotCountInsurance
        {
            get { return _IsNotCountInsurance; }
            set
            {
                if (_IsNotCountInsurance != value)
                {
                    _IsNotCountInsurance = value;
                    NotifyOfPropertyChange(() => IsNotCountInsurance);
                }
            }
        }

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbx;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbx
        {
            get
            {
                return _StoreCbx;
            }
            set
            {
                if (_StoreCbx != value)
                {
                    _StoreCbx = value;
                    NotifyOfPropertyChange(() => StoreCbx);
                }
            }
        }

        private long _StoreID;
        public long StoreID
        {
            get { return _StoreID; }
            set
            {
                if (_StoreID != value)
                {
                    _StoreID = value;
                    NotifyOfPropertyChange(() => StoreID);
                }
            }
        }

        private string _StaffName;
        public string StaffName
        {
            get { return _StaffName; }
            set
            {
                _StaffName = value;
                NotifyOfPropertyChange(() => StaffName);
            }
        }

        private string _Seller;
        public string Seller
        {
            get { return _Seller; }
            set
            {
                _Seller = value;
                NotifyOfPropertyChange(() => Seller);
            }
        }

        private Staff GetStaffLogin()
        {
            if (SelectedOutInvoice != null && SelectedOutInvoice.SelectedStaff != null)
            {
                Seller = SelectedOutInvoice.SelectedStaff.FullName;
            }
            else
            {
                StaffName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        private SearchOutwardInfo _SearchInvoiceCriteria;
        public SearchOutwardInfo SearchInvoiceCriteria
        {
            get
            {
                return _SearchInvoiceCriteria;
            }
            set
            {
                if (_SearchInvoiceCriteria != value)
                {
                    _SearchInvoiceCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchInvoiceCriteria);
            }
        }

        private PrescriptionSearchCriteria _SearchCriteria;
        public PrescriptionSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                if (_SearchCriteria != value)
                {
                    _SearchCriteria = value;
                }
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private Prescription _SelectedPrescription;
        public Prescription SelectedPrescription
        {
            get
            {
                return _SelectedPrescription;
            }
            set
            {
                if (_SelectedPrescription != value)
                {
                    _SelectedPrescription = value;
                }
                NotifyOfPropertyChange(() => SelectedPrescription);
            }
        }

        private ObservableCollection<OutwardDrug> OutwardDrugsCopy;

        private OutwardDrugInvoice SelectedOutInvoiceCopy;

        private OutwardDrugInvoice _SelectedOutInvoice;
        public OutwardDrugInvoice SelectedOutInvoice
        {
            get
            {
                return _SelectedOutInvoice;
            }
            set
            {
                if (_SelectedOutInvoice != value)
                {
                    _SelectedOutInvoice = value;
                    if (_SelectedOutInvoice != null)
                    {
                        strOutwardInvoiceStatus = _SelectedOutInvoice.OutDrugInvStatus;
                    }
                    SetPermissionForOutward();
                }
                NotifyOfPropertyChange(() => SelectedOutInvoice);
                NotifyOfPropertyChange(() => strOutwardInvoiceStatus);
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
                NotifyOfPropertyChange(() => IsRefundMoney);
            }
        }

        private Visibility _Visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                _Visibility = value;
                if (_Visibility == Visibility.Collapsed)
                {
                    IsVisibility = Visibility.Visible;
                }
                else
                {
                    IsVisibility = Visibility.Collapsed;
                }
                NotifyOfPropertyChange(() => Visibility);
            }
        }

        private Visibility _IsVisibility;
        public Visibility IsVisibility
        {
            get
            {
                return _IsVisibility;
            }
            set
            {
                if (_IsVisibility != value)
                {
                    _IsVisibility = value;
                }
                NotifyOfPropertyChange(() => IsVisibility);
            }
        }

        private Patient _patientInfo;
        public Patient PatientInfo
        {
            get
            {
                return _patientInfo;
            }
            set
            {
                if (_patientInfo != value)
                {
                    _patientInfo = value;
                    NotifyOfPropertyChange(() => PatientInfo);
                    NotifyOfPropertyChange(() => IsRefundMoney);
                }
            }
        }

        private bool _PayAndComfirmHIIsVisible = Globals.ServerConfigSection.CommonItems.PayOnComfirmHI;
        public bool PayAndComfirmHIIsVisible
        {
            get
            {
                return _PayAndComfirmHIIsVisible && IsServiceTabsVisible;
            }
            set
            {
                if (_PayAndComfirmHIIsVisible = value)
                {
                    return;
                }
                _PayAndComfirmHIIsVisible = value;
                NotifyOfPropertyChange(() => PayAndComfirmHIIsVisible);
            }
        }
        #endregion

        #region checking account
        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mBanThuocTheoToa_TimBN = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_TimBN, (int)ePermission.mView);
            mBanThuocTheoToa_ThongTin = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_ThongTin, (int)ePermission.mView);
            mBanThuocTheoToa_SuaToaThuoc = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_SuaToaThuoc, (int)ePermission.mView);
            mBanThuocTheoToa_Sua_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_Sua_In, (int)ePermission.mView);
            mBanThuocTheoToa_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_Them, (int)ePermission.mView);
            mBanThuocTheoToa_ThuTien = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_ThuTien, (int)ePermission.mView);
            mBanThuocTheoToa_HuyPhieuXuat = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_HuyPhieuXuat, (int)ePermission.mView);
            mBanThuocTheoToa_CapNhatPhieu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_CapNhatPhieu, (int)ePermission.mView);
            mBanThuocTheoToa_ReportIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_ReportIn, (int)ePermission.mView);
            mBanThuocTheoToa_CapNhatSauBaoCao = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBanThuocTheoToa,
                                               (int)oPharmacyEx.mBanThuocTheoToa_CapNhatSauBaoCao, (int)ePermission.mView);
        }

        private bool _mBanThuocTheoToa_TimBN = true;
        private bool _mBanThuocTheoToa_ThongTin = true;
        private bool _mBanThuocTheoToa_SuaToaThuoc = true;
        private bool _mBanThuocTheoToa_Sua_In = true;
        private bool _mBanThuocTheoToa_Them = true;
        private bool _mBanThuocTheoToa_ThuTien = true;
        private bool _mBanThuocTheoToa_HuyPhieuXuat = true;
        private bool _mBanThuocTheoToa_CapNhatPhieu = true;
        private bool _mBanThuocTheoToa_ReportIn = true;
        private bool _mBanThuocTheoToa_CapNhatSauBaoCao = true;

        public bool mBanThuocTheoToa_TimBN
        {
            get
            {
                return _mBanThuocTheoToa_TimBN && !IsConfirmHIView;
            }
            set
            {
                if (_mBanThuocTheoToa_TimBN == value)
                    return;
                NotifyOfPropertyChange(() => mBanThuocTheoToa_TimBN);
            }
        }

        public bool mBanThuocTheoToa_ThongTin
        {
            get
            {
                return _mBanThuocTheoToa_ThongTin;
            }
            set
            {
                if (_mBanThuocTheoToa_ThongTin == value)
                    return;
                _mBanThuocTheoToa_ThongTin = value;
            }
        }

        public bool mBanThuocTheoToa_SuaToaThuoc
        {
            get
            {
                return _mBanThuocTheoToa_SuaToaThuoc;
            }
            set
            {
                if (_mBanThuocTheoToa_SuaToaThuoc == value)
                    return;
                _mBanThuocTheoToa_SuaToaThuoc = value;
            }
        }

        public bool mBanThuocTheoToa_Sua_In
        {
            get
            {
                return _mBanThuocTheoToa_Sua_In;
            }
            set
            {
                if (_mBanThuocTheoToa_Sua_In == value)
                    return;
                _mBanThuocTheoToa_Sua_In = value;
            }
        }

        public bool mBanThuocTheoToa_Them
        {
            get
            {
                return _mBanThuocTheoToa_Them;
            }
            set
            {
                if (_mBanThuocTheoToa_Them == value)
                    return;
                _mBanThuocTheoToa_Them = value;
                NotifyOfPropertyChange(() => RenewScreenVisible);
            }
        }

        public bool RenewScreenVisible
        {
            get
            {
                return mBanThuocTheoToa_Them && !IsConfirmHIView;
            }
        }

        public bool mBanThuocTheoToa_ThuTien
        {
            get
            {
                return _mBanThuocTheoToa_ThuTien && (!IsConfirmPrescriptionOnly || !Globals.ServerConfigSection.CommonItems.EnableHIStore);
            }
            set
            {
                if (_mBanThuocTheoToa_ThuTien == value)
                    return;
                _mBanThuocTheoToa_ThuTien = value;
            }
        }

        public bool mBanThuocTheoToa_HuyPhieuXuat
        {
            get
            {
                return _mBanThuocTheoToa_HuyPhieuXuat;
            }
            set
            {
                if (_mBanThuocTheoToa_HuyPhieuXuat == value)
                    return;
                _mBanThuocTheoToa_HuyPhieuXuat = value;
            }
        }

        public bool mBanThuocTheoToa_CapNhatPhieu
        {
            get
            {
                return _mBanThuocTheoToa_CapNhatPhieu && !IsConfirmHIView;
            }
            set
            {
                if (_mBanThuocTheoToa_CapNhatPhieu == value)
                    return;
                _mBanThuocTheoToa_CapNhatPhieu = value;
            }
        }

        public bool mBanThuocTheoToa_ReportIn
        {
            get
            {
                return _mBanThuocTheoToa_ReportIn;
            }
            set
            {
                if (_mBanThuocTheoToa_ReportIn == value)
                    return;
                _mBanThuocTheoToa_ReportIn = value;
            }
        }

        public bool mBanThuocTheoToa_CapNhatSauBaoCao
        {
            get
            {
                return _mBanThuocTheoToa_CapNhatSauBaoCao;
            }
            set
            {
                if (_mBanThuocTheoToa_CapNhatSauBaoCao == value)
                    return;
                _mBanThuocTheoToa_CapNhatSauBaoCao = value;
            }
        }
        //private bool _bEdit = true;
        //private bool _bAdd = true;
        //private bool _bDelete = true;
        //private bool _bView = true;
        //private bool _bPrint = true;
        //private bool _bReport = true;
        //private bool _bTinhTien = true;
        //private bool _bLuuTinhTien = true;
        //private bool _bSuaToa = true;
        //public bool bSuaToa
        //{
        //    get
        //    {
        //        return _bSuaToa;
        //    }
        //    set
        //    {
        //        if (_bSuaToa == value)
        //            return;
        //        _bSuaToa = value;
        //        NotifyOfPropertyChange(() => bSuaToa);
        //    }
        //}
        //public bool bEdit
        //{
        //    get
        //    {
        //        return _bEdit;
        //    }
        //    set
        //    {
        //        if (_bEdit == value)
        //            return;
        //        _bEdit = value;
        //    }
        //}
        //public bool bAdd
        //{
        //    get
        //    {
        //        return _bAdd;
        //    }
        //    set
        //    {
        //        if (_bAdd == value)
        //            return;
        //        _bAdd = value;
        //    }
        //}
        //public bool bDelete
        //{
        //    get
        //    {
        //        return _bDelete;
        //    }
        //    set
        //    {
        //        if (_bDelete == value)
        //            return;
        //        _bDelete = value;
        //    }
        //}
        //public bool bView
        //{
        //    get
        //    {
        //        return _bView;
        //    }
        //    set
        //    {
        //        if (_bView == value)
        //            return;
        //        _bView = value;
        //    }
        //}
        //public bool bPrint
        //{
        //    get
        //    {
        //        return _bPrint;
        //    }
        //    set
        //    {
        //        if (_bPrint == value)
        //            return;
        //        _bPrint = value;
        //    }
        //}
        //public bool bTinhTien
        //{
        //    get
        //    {
        //        return _bTinhTien;
        //    }
        //    set
        //    {
        //        if (_bTinhTien == value)
        //            return;
        //        _bTinhTien = value;
        //    }
        //}
        //public bool bLuuTinhTien
        //{
        //    get
        //    {
        //        return _bLuuTinhTien;
        //    }
        //    set
        //    {
        //        if (_bLuuTinhTien == value)
        //            return;
        //        _bLuuTinhTien = value;
        //    }
        //}
        //public bool bReport
        //{
        //    get
        //    {
        //        return _bReport;
        //    }
        //    set
        //    {
        //        if (_bReport == value)
        //            return;
        //        _bReport = value;
        //    }
        //}
        #endregion

        #region binding visibilty

        //public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            //lnkDelete = sender as Button;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        #region DS Kho Ngoai Tru Member

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            //==== #001
            long StoreType = (long)AllLookupValues.StoreType.STORAGE_EXTERNAL;
            if (IsHIOutPt)
                StoreType = (long)AllLookupValues.StoreType.STORAGE_HIDRUGs;
            //==== #001
            var paymentTypeTask = new LoadStoreListTask(StoreType, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (!IsHIOutPt)
            {
                StoreCbx.Remove(StoreCbx.FirstOrDefault(x => x.StoreTypeID == (long)AllLookupValues.StoreType.STORAGE_HIDRUGs));
            }
            //▼===== 20191211 TTM: Sửa lại code để có thể làm việc được cho cả bệnh viện có kho lẻ xuất và không có kho lẻ xuất.
            if (StoreCbx != null && StoreCbx.Where(x => x.IsSubStorage).Count() > 0)
            {
                StoreCbx = StoreCbx.OrderByDescending(x => x.IsSubStorage).ToObservableCollection();
            }
            else
            {
                StoreCbx = paymentTypeTask.LookupList;
            }
            //▲=====
            SetDefaultForStore();
            isLoadingGetStore = false;
            yield break;
        }

        private decimal AmountPaided = 0;
        private IEnumerator<IResult> DoGetAmountPaided(long outiID, OutwardDrugInvoice Invoice, bool IsDepent = false, bool aPayOnComfirmHI = true)
        {
            var paymentTypeTask = new CalcMoneyPaidedForDrugInvoiceTask(outiID);
            yield return paymentTypeTask;
            AmountPaided = paymentTypeTask.Amount;
            if (!IsDepent)
            {
                if (IsBenhNhanNoiTru())
                {
                    GetRegistrationInfo_InPt(Invoice.PtRegistrationID.Value);
                }
                else
                {
                    GetRegistrationInfo(Invoice.PtRegistrationID.Value, false, aPayOnComfirmHI);
                }
            }
            else
            {
                ShowFormCountMoney();
            }
            yield break;
        }

        public void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                RefeshData();
            }
        }

        private void SetDefaultForStore()
        {
            if (StoreCbx != null)
            {
                StoreID = StoreCbx.FirstOrDefault().StoreID;
            }
        }
        #endregion

        #region Kiem Tra Du Lieu Member

        private bool CheckQuantity(object outward)
        {
            try
            {
                OutwardDrug p = outward as OutwardDrug;
                if (p.QtyOffer == p.OutQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public Nullable<double> ValueQuyenLoiBH
        {
            get
            {
                if (SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null)
                {
                    if (SelectedOutInvoice.outiID == 0)
                    {
                        if (SelectedOutInvoice.SelectedPrescription.IsSold)
                            return 0;
                        return PatientInfo.LatestRegistration.PtInsuranceBenefit;
                    }
                    else
                    {
                        if (SelectedOutInvoice.IsHICount.GetValueOrDefault())
                            return PatientInfo.LatestRegistration.PtInsuranceBenefit;
                        return 0;
                    }
                }
                return null;
            }
        }

        int DayRpts = 0;
        private void GetClassicationPatientPrescription()
        {
            //neu da ban 1 lan khong tinh BH,thi lan sau mua co dc tinh BH hay ko?
            //if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= DateTime.Now.Date
            //&& SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && !SelectedOutInvoice.SelectedPrescription.IsSold)
            //if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= Globals.ServerDate.Value.Date)
            //{
            //    if (Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "BHYT thanh toán cho toa thuốc này!"))
            //    {
            //        DayRpts = 0;
            //        Visibility = Visibility.Visible;
            //        IsHIPatient = false;
            //    }
            //    else if (SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.SelectedPrescription.Issue_HisID > 0 && !SelectedOutInvoice.SelectedPrescription.IsSold)
            //    {
            //        TimeSpan t = PatientInfo.CurrentHealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.ServerDate.Value.Date;
            //        DayRpts = Convert.ToInt32(t.TotalDays + 1);
            //        Visibility = Visibility.Collapsed;
            //        IsHIPatient = true;
            //    }
            //}
            if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= Globals.ServerDate.Value.Date
            && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.SelectedPrescription.Issue_HisID > 0 && !SelectedOutInvoice.SelectedPrescription.IsSold)
            {
                TimeSpan t = PatientInfo.CurrentHealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.ServerDate.Value.Date;
                DayRpts = Convert.ToInt32(t.TotalDays + 1);
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            NotifyOfPropertyChange(() => ValueQuyenLoiBH);
        }

        private void GetClassicationPatientInvoice()
        {
            //if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= DateTime.Now.Date
            // && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.IsHICount.GetValueOrDefault())
            //Kiên thêm đk SelectedOutInvoice.IsHICount == true, vì khi load 1 phiếu xuất cũ lên, không có IsSold nên không biết là phiếu đó được bán lần 1 hay lần 2.
            //Nên phải dựa vào IsHICount, nếu IsHICount của phiếu đó = true tức là phiếu đó bán lần 1 và được tính bảo hiểm.
            if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= Globals.ServerDate.Value
             && SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null && SelectedOutInvoice.SelectedPrescription.Issue_HisID > 0 && SelectedOutInvoice.IsHICount == true)
            {
                TimeSpan t = PatientInfo.CurrentHealthInsurance.ValidDateTo.GetValueOrDefault() - Globals.ServerDate.Value;
                DayRpts = Convert.ToInt32(t.TotalDays + 1);
                Visibility = Visibility.Collapsed;
                IsHIPatient = true;
            }
            else
            {
                //0:benh nhan thong thuong,1:benh nhan bao hiem
                DayRpts = 0;
                Visibility = Visibility.Visible;
                IsHIPatient = false;
            }
            NotifyOfPropertyChange(() => ValueQuyenLoiBH);
        }

        private bool CheckValid()
        {
            bool result = true;
            string strError = "";
            if (SelectedOutInvoice != null)
            {
                if (eHCMS.Services.Core.AxHelper.CompareDate((DateTime)SelectedPrescription.IssuedDateTime, SelectedOutInvoice.OutDate) == 1)
                {
                    MessageBox.Show(eHCMSResources.A0861_G1_Msg_InfoNgXuatKhHopLe2, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (eHCMS.Services.Core.AxHelper.CompareDate(SelectedOutInvoice.OutDate, Globals.ServerDate.Value) == 1)
                {
                    MessageBox.Show(eHCMSResources.A0861_G1_Msg_InfoNgXuatKhHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if (SelectedOutInvoice.OutwardDrugs == null && (SelectedPrescription == null || !SelectedPrescription.IsEmptyPrescription))
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                if ((SelectedOutInvoice.OutwardDrugs == null || SelectedOutInvoice.OutwardDrugs.Count == 0) && (SelectedPrescription == null || !SelectedPrescription.IsEmptyPrescription))
                {
                    MessageBox.Show(eHCMSResources.A0928_G1_Msg_InfoPhXuatKhDcDeTrong, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
                {
                    if (item.OutPrice <= 0)
                    {
                        MessageBox.Show(eHCMSResources.A0525_G1_Msg_InfoGiaBanLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    if (item.OutQuantity <= 0)
                    {
                        MessageBox.Show(eHCMSResources.Z0837_G1_SLgXuatMoiDongLonHon0, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    if (item.Validate() == false)
                    {
                        MessageBox.Show(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        result = false;
                        break;
                    }
                    //neu ngay het han lon hon ngay hien tai
                    if (eHCMS.Services.Core.AxHelper.CompareDate(Globals.ServerDate.Value, item.InExpiryDate) == 1)
                    {
                        strError += item.GetDrugForSellVisitor.BrandName + string.Format(" {0}!", eHCMSResources.Z0868_G1_DaHetHanDung) + Environment.NewLine;
                    }
                }
                if (!string.IsNullOrEmpty(strError))
                {
                    if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0939_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        //KMx: 2 hàm GetValueFromAnonymousType(), CalScheduleQty() giống với 2 hàm cùng tên ở EditPrescriptionViewModel (08/06/2014 17:41).
        //Ban đầu tính đem 2 hàm này sang Globals để 2 ViewModels sử dụng chung. Nhưng đem qua Globals thì bị lỗi ở dòng (T)type.GetProperty(itemKey).GetValue(dataItem, null);
        public static T GetValueFromAnonymousType<T>(object dataItem, string itemKey)
        {
            System.Type type = dataItem.GetType();

            T itemValue = (T)type.GetProperty(itemKey).GetValue(dataItem, null);

            return itemValue;
        }

        public static float CalScheduleQty(object item, int nNumDay)
        {
            if (item == null)
            {
                return 0;
            }

            float[] WeeklySchedule = new float[7];
            byte? SchedBeginDOW;
            double DispenseVolume;

            SchedBeginDOW = GetValueFromAnonymousType<byte?>(item, "SchedBeginDOW");

            DispenseVolume = GetValueFromAnonymousType<double>(item, "DispenseVolume");

            WeeklySchedule[0] += GetValueFromAnonymousType<float>(item, "QtySchedMon");
            WeeklySchedule[1] += GetValueFromAnonymousType<float>(item, "QtySchedTue");
            WeeklySchedule[2] += GetValueFromAnonymousType<float>(item, "QtySchedWed");
            WeeklySchedule[3] += GetValueFromAnonymousType<float>(item, "QtySchedThu");
            WeeklySchedule[4] += GetValueFromAnonymousType<float>(item, "QtySchedFri");
            WeeklySchedule[5] += GetValueFromAnonymousType<float>(item, "QtySchedSat");
            WeeklySchedule[6] += GetValueFromAnonymousType<float>(item, "QtySchedSun");

            return Globals.CalcWeeklySchedulePrescription(SchedBeginDOW, nNumDay, WeeklySchedule, (float)DispenseVolume);
        }


        //KMx: Hàm này được viết lại từ hàm CheckInsurance. Lý do đổi cách tính thuốc lịch (08/06/2014 15:54)
        private bool CheckInsurance_New(int BHValidDays)
        {
            if (SelectedPrescription == null)
            {
                return true;
            }
            if (SelectedOutInvoice == null)
            {
                return true;
            }
            if (ListDrugTemp == null)
            {
                ListDrugTemp = new ObservableCollection<ListDrugAndQtySell>();
            }
            ListDrugTemp.Clear();

            var hhh = from hd in SelectedOutInvoice.OutwardDrugs
                      where hd.GetDrugForSellVisitor.InsuranceCover == true && hd.ChargeableItem.HIAllowedPrice > 0
                      group hd by new
                      {
                          hd.DrugID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetDrugForSellVisitor.InsuranceCover,
                          hd.GetDrugForSellVisitor.BrandName
                      } into hdgroup

                      select new
                      {
                          QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          DrugID = hdgroup.Key.DrugID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          InsuranceCover = hdgroup.Key.InsuranceCover,
                          BrandName = hdgroup.Key.BrandName
                      };

            string strError = "";
            int xNgayToiDa = 30;
            int xNgayBHToiDa_NgoaiTru = 30;
            int xNgayBHToiDa_NoiTru = 5;

            xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;
            xNgayBHToiDa_NoiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NoiTru;

            if (IsBenhNhanNoiTru() == false)
            {
                xNgayToiDa = xNgayBHToiDa_NgoaiTru;
            }
            else
            {
                xNgayToiDa = xNgayBHToiDa_NoiTru;
            }

            //Cảnh báo cho thuốc Cần.
            string strDrugsNotHaveDayRpts = "";
            //Những thuốc mà hết hạn bảo hiểm. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
            string strInvalidDrugs = "";

            for (int i = 0; i < hhh.Count(); i++)
            {
                //QtyMaxAllowed: Số lượng tối đa được bán (Minimum của SLYC và SL BH cho phép).
                //QtyHIValidateTo: Số lượng thuốc tính đến ngày BH hết hạn. Nếu SL bán > SL tính đến hết ngày BH hết hạn thì cảnh báo.
                //QtyOffer: Số lượng bác sĩ yêu cầu. Bác sĩ yêu cầu bao nhiêu viên thì đưa lên bấy nhiêu.

                int QtyMaxAllowed = 0;
                int QtyHIValidateTo = 0;

                string strReasonCannotSell;

                if (hhh.ToList()[i].QtyOffer <= 0)
                {
                    MessageBox.Show(string.Format("Thuốc {0} không có SLYC nên không được bán. Nếu muốn bán toa này phải xóa thuốc đó đi!", hhh.ToList()[i].BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return false;
                }

                QtyMaxAllowed = Convert.ToInt32(hhh.ToList()[i].QtyMaxAllowed);

                //KMx: Nếu là thuốc Cần thì so sánh "số lượng xuất" và "số lượng được bán tối đa."
                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN)
                {
                    if (hhh.ToList()[i].OutQuantity > QtyMaxAllowed)
                    {
                        strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                        strError += string.Format(eHCMSResources.Z1717_G1_Thuoc0SLgBSiKe, hhh.ToList()[i].BrandName, hhh.ToList()[i].QtyOffer) + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.xban = QtyMaxAllowed;
                        ListDrugTemp.Add(item);
                    }
                    else
                    {
                        strDrugsNotHaveDayRpts += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }
                }

                //KMx: Nếu là thuốc Thường hoặc thuốc Lịch.
                else
                {
                    //KMx: QtyHIValidateTo: Số lượng tính đến ngày BH còn hiệu lực.
                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                    {
                        //KMx: Phải Round trước rồi mới Ceiling sau, nếu không sẽ bị sai trong trường hợp kết quả có nhiều số lẻ. VD: 5.00001
                        double QtyRounded = Math.Round((float)hhh.ToList()[i].QtyForDay * BHValidDays, 2);
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(QtyRounded));
                    }

                    if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                    {
                        QtyHIValidateTo = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], BHValidDays)));
                    }

                    //Nếu số lượng muốn bán > số lượng mà bảo hiểm còn hiệu lực. VD: Bác sĩ kê 10 ngày, nhưng bảo hiểm chỉ còn hiệu lực 5 ngày.
                    if (hhh.ToList()[i].OutQuantity > QtyHIValidateTo)
                    {
                        strInvalidDrugs += hhh.ToList()[i].BrandName + Environment.NewLine;
                    }

                    //Lấy Min của SLYC và SL tối đa để khống chế số lượng thực xuất.
                    int QtyAllowSell = Math.Min(hhh.ToList()[i].QtyOffer, QtyMaxAllowed);

                    if (hhh.ToList()[i].OutQuantity > QtyAllowSell)
                    {
                        if (hhh.ToList()[i].QtyOffer > QtyMaxAllowed)
                        {
                            strReasonCannotSell = string.Format(eHCMSResources.Z0876_G1_BHChiTraToiDa, xNgayToiDa.ToString());
                            strError += string.Format(eHCMSResources.Z1718_G1_Thuoc0SLgDcBan, hhh.ToList()[i].BrandName, QtyMaxAllowed) + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }
                        else
                        {
                            strReasonCannotSell = eHCMSResources.Z0874_G1_SLgBanVuotYC;
                            strError += string.Format(eHCMSResources.Z1717_G1_Thuoc0SLgBSiKe, hhh.ToList()[i].BrandName, hhh.ToList()[i].QtyOffer) + Environment.NewLine + strReasonCannotSell + Environment.NewLine;
                        }

                        ListDrugAndQtySell item = new ListDrugAndQtySell();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.xban = QtyAllowSell;
                        ListDrugTemp.Add(item);
                    }
                }
            }

            if (!string.IsNullOrEmpty(strError))
            {
                if (MessageBox.Show(strError + Environment.NewLine + eHCMSResources.I0940_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
                //sua lai so luong xuat cho hop li
                for (int i = 0; i < ListDrugTemp.Count; i++)
                {
                    var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == ListDrugTemp[i].DrugID).OrderBy(x => x.InExpiryDate);

                    if (values == null || values.Count() <= 0)
                    {
                        continue;
                    }

                    int xban = ListDrugTemp[i].xban;

                    foreach (OutwardDrug p in values)
                    {
                        if (xban <= 0)
                        {
                            DeleteOutwardDrugs(p);
                            continue;
                        }

                        if (p.OutQuantity <= xban)
                        {
                            if (p.QtyOffer <= xban)
                            {
                                p.OutQuantity = p.QtyOffer;
                            }
                            xban = xban - p.OutQuantity;
                        }
                        else //p.OutQuantity > xban
                        {
                            p.OutQuantity = xban;
                            xban = 0;
                        }
                    }
                }

                SumTotalPrice();
                return false;
            }

            if (!string.IsNullOrEmpty(strInvalidDrugs))
            {
                if (MessageBox.Show(string.Format(eHCMSResources.Z1710_G1_BHCuaBNConHieuLuc0Ng, BHValidDays, strInvalidDrugs), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(strDrugsNotHaveDayRpts))
            {
                if (MessageBox.Show(eHCMSResources.Z0882_G1_I + Environment.NewLine + Environment.NewLine + strDrugsNotHaveDayRpts + Environment.NewLine + "Bạn có đồng ý bán không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }


        public void btnNgayDung_New()
        {
            if (SelectedPrescription == null)
            {
                return;
            }

            if (SelectedOutInvoice == null)
            {
                return;
            }

            int NgayDung = 0;

            if (!Int32.TryParse(strNgayDung, out NgayDung))
            {
                MessageBox.Show(eHCMSResources.A0836_G1_Msg_InfoNgDungKhHopLe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (NgayDung <= 0)
            {
                MessageBox.Show(eHCMSResources.A0837_G1_Msg_InfoNgDungLonHon0);
                return;
            }

            if (IsHIPatient && !IsNotCountInsurance) //toa bao hiem
            {
                int xNgayBHToiDa_NgoaiTru = 30;

                xNgayBHToiDa_NgoaiTru = Globals.ServerConfigSection.HealthInsurances.PharmacyMaxDaysHIRebate_NgoaiTru;

                //KMx: Lấy thuốc có ngày dùng nhỏ nhất (không tính thuốc cần). Nếu ngày dùng muốn cập nhật > ngày dùng nhỏ nhất thì không cho cập nhật.
                var DrugMinDayRpts = SelectedOutInvoice.OutwardDrugs.Where(x => x.DayRpts > 0 && x.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN).OrderBy(x => x.DayRpts).Take(1);

                if (NgayDung > xNgayBHToiDa_NgoaiTru)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1711_G1_BHChiTraToiDa0NgThuoc, xNgayBHToiDa_NgoaiTru), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return;
                }

                foreach (OutwardDrug d in DrugMinDayRpts)
                {
                    if (NgayDung > d.DayRpts)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1712_G1_KgDcNhapNgDungLonHonBSiYC, d.GetDrugForSellVisitor.BrandName, d.DayRpts), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        return;
                    }
                }
            }

            if (GetDrugForSellVisitorList == null)
            {
                GetDrugForSellVisitorList = new ObservableCollection<GetDrugForSellVisitor>();
            }
            else
            {
                GetDrugForSellVisitorList.Clear();
            }

            //dung de lay ton hien tai va ton ban dau cua tung thuoc theo lo
            ObservableCollection<GetDrugForSellVisitor> temp = GetDrugForSellVisitorListByPrescriptID.DeepCopy();
            LaySoLuongTheoNgayDung(temp);

            //dung de lay ton hien tai va ton ban dau cua tung thuoc da duoc sum
            var ObjSumRemaining = from hd in GetDrugForSellVisitorList
                                  group hd by new { hd.DrugID, hd.BrandName } into hdgroup
                                  select new
                                  {
                                      Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                                      RemainingFirst = hdgroup.Sum(groupItem => groupItem.RemainingFirst),
                                      DrugID = hdgroup.Key.DrugID,
                                      BrandName = hdgroup.Key.BrandName
                                  };

            //lay tong so luong yc cua cac thuoc co trong toa (dua vao ngay dung > 0) 
            var hhh = from hd in SelectedOutInvoice.OutwardDrugs
                      where hd.DayRpts > 0 && hd.V_DrugType != (long)AllLookupValues.V_DrugType.THUOC_UONGKHICAN
                      group hd by new
                      {
                          hd.DrugID,
                          hd.DayRpts,
                          hd.V_DrugType,
                          hd.QtyForDay,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.GetDrugForSellVisitor.BrandName
                      } into hdgroup
                      select new
                      {
                          QtyOffer = hdgroup.Sum(groupItem => groupItem.QtyOffer),
                          OutQuantity = hdgroup.Sum(groupItem => groupItem.OutQuantity),
                          DrugID = hdgroup.Key.DrugID,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,

                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,

                          BrandName = hdgroup.Key.BrandName
                      };

            string ThongBao = "";
            //Lưu ý: Khi SelectedOutInvoice.OutwardDrugs add thêm 1 đối tượng (OutwardDrug) thì hhh.Count() tự động tăng lên 1.

            for (int i = 0; i < hhh.Count(); i++)
            {
                int QtyWillChange = 0;

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_THONGTHUONG)
                {
                    //KMx: Sau khi lấy QtyForDay * NgayDung thì phải Round lại rồi sau đó mới Ceiling.
                    //Nếu không sẽ bị sai trong trường hợp thuốc có QtyForDay = 0.1666667. Khi nhân lên 30 ngày sẽ ra kết quả 5.00001 và Ceiling liền thì sẽ ra 6.
                    double QtyRounded = Math.Round((double)hhh.ToList()[i].QtyForDay * NgayDung, 2);
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(QtyRounded));
                }

                if (hhh.ToList()[i].V_DrugType == (long)AllLookupValues.V_DrugType.THUOC_UONGLICHTUAN)
                {
                    QtyWillChange = Convert.ToInt32(Math.Ceiling(CalScheduleQty(hhh.ToList()[i], NgayDung)));
                }

                //KMx: Dòng thuốc có số lượng muốn thay đổi bằng 0 thì sẽ xóa dòng thuốc đó đi (14/06/2014 16:54)
                //if (QtyWillChange <= 0)
                //{
                //    continue;
                //}

                var values = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault()).OrderBy(x => x.InExpiryDate);

                if (values == null || values.Count() <= 0)
                {
                    continue;
                }

                //Neu so luong < So luong hien co tren luoi thi chi can ham ben duoi
                foreach (OutwardDrug p in values)
                {
                    if (QtyWillChange <= 0)
                    {
                        DeleteOutwardDrugs(p);
                    }
                    else if (p.OutQuantity > QtyWillChange)
                    {
                        p.OutQuantity = QtyWillChange;
                        QtyWillChange = 0;
                    }
                    else if (p.OutQuantity <= QtyWillChange)
                    {
                        QtyWillChange = QtyWillChange - p.OutQuantity;
                    }
                }

                if (QtyWillChange <= 0)
                {
                    continue;
                }

                //else neu > so luong hien co tren luoi thi them vao

                var Obj = ObjSumRemaining.Where(x => x.DrugID == hhh.ToList()[i].DrugID.GetValueOrDefault());
                if (Obj != null && Obj.Count() > 0)
                {
                    if (Obj.ToArray()[0].Remaining > 0 && Obj.ToArray()[0].Remaining >= QtyWillChange)
                    {
                        GetDrugForSellVisitor item = new GetDrugForSellVisitor();
                        item.DrugID = hhh.ToList()[i].DrugID.GetValueOrDefault();
                        item.BrandName = hhh.ToList()[i].BrandName;
                        //KMx: Phải assign Remaining trước RequiredNumber. Nếu không sẽ bị lỗi, vì khi assign RequiredNumber thì nó sẽ so sánh với Remaining.
                        item.Remaining = Obj.ToArray()[0].Remaining;
                        item.RequiredNumber = QtyWillChange;
                        item.Qty = hhh.ToList()[i].QtyOffer;
                        //item.Remaining = Obj.ToArray()[0].Remaining;

                        ReCountQtyAndAddList(item);
                    }
                    else
                    {
                        if (Obj.ToArray()[0].RemainingFirst > 0)
                        {
                            ThongBao = ThongBao + string.Format("Thuốc {0}: SL cần bán = {1}, SL còn lại = {2}", Obj.ToArray()[0].BrandName, QtyWillChange.ToString(), Obj.ToArray()[0].RemainingFirst.ToString()) + Environment.NewLine;
                        }
                        else
                        {
                            ThongBao = ThongBao + string.Format("Thuốc {0}: SL cần bán = {1}, SL còn lại = {2}", Obj.ToArray()[0].BrandName, QtyWillChange.ToString() + "0") + Environment.NewLine;
                        }
                    }
                }
                else
                {
                    ThongBao = ThongBao + string.Format("Thuốc {0} đã hết!", hhh.ToList()[i].BrandName) + Environment.NewLine;
                }

            }
            //KMx: Nếu để dòng dưới thì sẽ bị lỗi khi thêm cùng 1 loại thuốc có trong toa nhưng khác lô.
            //SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();
            if (!string.IsNullOrEmpty(ThongBao))
            {
                MessageBox.Show(ThongBao);
            }
        }
        #endregion

        #region Luu, Thu Tien Member
        public void btnSaveMoney()
        {
            if (PatientInfo.LatestRegistration != null 
                && Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "bán thuốc") && ValueQuyenLoiBH.Value > 0)
            {
                return;
            }
            if (SelectedOutInvoice == null)
            {
                SelectedOutInvoice = new OutwardDrugInvoice();
                SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
            }
            if (IsConfirmHIView)
            {
                SelectedOutInvoice.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN;
            }
            else
            {
                SelectedOutInvoice.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
            }
            if (!CheckValid())
            {
                return;
            }

            if (SelectedPrescription == null)
            {
                return;
            }

            if (SelectedPrescription.IsEmptyPrescription && IsConfirmPrescriptionOnly)
            {
                return;
            }
            //▼===== 20190105 TTM: Chỗ này set tạm như vầy vì ý ban đầu là thế để hỏi anh Tuấn cách kiểm lại cho chính xác.
            foreach (var x in SelectedOutInvoice.OutwardDrugs)
            {
                foreach (var y in PatientInfo.LatestRegistration.AllSaveRegistrationDetails)
                {
                    foreach(var z in PatientInfo.LatestRegistration.ListOfPrescriptionIssueHistory)
                    {
                        if (z.PtRegDetailID == y.PtRegDetailID && y.HisID == null && y.TotalHIPayment > 0 && x.PrescriptionDetailObj.PrescriptID == z.PrescriptID && x.HI == true)
                        {
                            MessageBox.Show("Trong toa dịch vụ có thuốc bảo hiểm đề nghị hiệu chỉnh lại toa thuốc");
                            return;
                        }
                    }
                }
            }
            //▲=====
            if (SelectedOutInvoice.outiID <= 0)
            {
                SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                SelectedOutInvoice.IssueID = SelectedPrescription.IssueID;
                SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                SelectedOutInvoice.SelectedStorage = new RefStorageWarehouseLocation();
                SelectedOutInvoice.SelectedStorage.StoreID = StoreID;
                SelectedOutInvoice.SelectedStaff = GetStaffLogin();
                SelectedOutInvoice.StaffID = Globals.LoggedUserAccount.StaffID.GetValueOrDefault();
                SelectedOutInvoice.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
                SelectedOutInvoice.StoreID = StoreID;
            }
            SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE;
            SelectedOutInvoice.IsHICount = IsHIPatient && !IsNotCountInsurance;
            if (IsHIPatient && !IsNotCountInsurance)//toa bao hiem
            {
                if (DayRpts > 0)
                {
                    //if (CheckInsurance(DayRpts))
                    if (CheckInsurance_New(DayRpts))
                    {
                        SelectedOutInvoice.ColectDrugSeqNumType = 2;//co BH
                        SaveDrugNew(SelectedOutInvoice);
                    }
                }
                else
                {
                    if (MessageBox.Show(eHCMSResources.Z0955_G1_TheBHBNDaHetHan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        //gan gia BH = 0 het
                        if (SelectedOutInvoice.OutwardDrugs != null)
                        {
                            foreach (OutwardDrug p in SelectedOutInvoice.OutwardDrugs)
                            {
                                p.HIAllowedPrice = 0;
                                p.InwardDrug.HIAllowedPrice = 0;

                                p.OutPrice = p.NormalPrice;
                            }
                            SelectedOutInvoice.ColectDrugSeqNumType = 1;//khong bao hiem
                            SaveDrugNew(SelectedOutInvoice);
                        }

                    }
                }
            }
            else
            {
                SelectedOutInvoice.ColectDrugSeqNumType = 1;//khong bao hiem
                SaveDrugNew(SelectedOutInvoice);
            }
        }

        private void SaveDrugNew(OutwardDrugInvoice Invoice)
        {
            //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)
            // Txd 25/05/2014 Replaced ConfigList
            if (Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1)
            {
                //ham tinh tien thuoc rieng biet
                SaveDrugIndependents(SelectedOutInvoice);
            }
            else
            {
                //SaveDrugs(SelectedOutInvoice, true);
                //b2d: sua lai coroutine cho nay
                Coroutine.BeginExecute(DoSaveDrugs(SelectedOutInvoice, true));
            }
        }

        private void SaveDrugs(OutwardDrugInvoice Invoice, bool value)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    /*==== #001 ====*/
                    Invoice.IsOutHIPt = this.IsHIOutPt;
                    /*==== #001 ====*/
                    contract.BeginSaveDrugs(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID, null, Invoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            OutwardDrugInvoice OutInvoice;
                            contract.EndSaveDrugs(out OutInvoice, asyncResult);
                            if (value)
                            {
                                ClearData();
                                OutwardDrugsCopy = OutInvoice.OutwardDrugs;
                                GetOutWardDrugInvoiceVisitorByID(OutInvoice.outiID);
                                LoadAndPayMoneyForService(OutInvoice);
                            }
                            else
                            {
                                if (OutInvoice != null)
                                {
                                    ClearData();
                                    OutwardDrugsCopy = OutInvoice.OutwardDrugs;
                                    GetOutWardDrugInvoiceVisitorByID(OutInvoice.outiID);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            isLoadingFullOperator = false;
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void btnMoney(object sender, RoutedEventArgs e)
        {
            if (PatientInfo != null && PatientInfo.LatestRegistration != null
                && Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "thu tiền") && ValueQuyenLoiBH.Value > 0)
            {
                return;
            }
            //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)
            // Txd 25/05/2014 Replaced ConfigList
            LoadAndPayMoneyForService(SelectedOutInvoice, Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1, false);
        }
        public void btnConfirmHIAndPay()
        {
            Coroutine.BeginExecute(DoConfirmHIBenefit(SelectedOutInvoice));
        }

        private void LoadAndPayMoneyForService(OutwardDrugInvoice Invoice, bool IsDepent = false, bool aPayOnComfirmHI = true)
        {
            //long PtRegistrationID = Invoice.PtRegistrationID.Value;
            if (Invoice == null)
            {
                Globals.ShowMessage(eHCMSResources.A0664_G1_Msg_InfoKhCoPhXuat, eHCMSResources.G0442_G1_TBao);
                return;
            }
            Coroutine.BeginExecute(DoGetAmountPaided(Invoice.outiID, Invoice, IsDepent, aPayOnComfirmHI));
        }

        private void GetRegistrationInfo(long PtRegistrationID, bool aIsConfirmAlso = false, bool aPayOnComfirmHI = true, bool CallComfirmOnPay = false)
        {
            isLoadingGetID = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRegistrationInfo(PtRegistrationID, 0, false, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var RegistrationInfo = contract.EndGetRegistrationInfo(asyncResult);
                                if (RegistrationInfo == null)
                                {
                                    MessageBox.Show(eHCMSResources.K0299_G1_ChonDK);
                                    return;
                                }
                                if (this.IsConfirmHIView && !aIsConfirmAlso && (aPayOnComfirmHI || (!aPayOnComfirmHI && RegistrationInfo.ConfirmHIStaffID.GetValueOrDefault(0) == 0)))
                                {
                                    ApplySearchInvoiceCriteriaValues(null, PtRegistrationID);
                                    SearchInvoiceOld();
                                    return;
                                }
                                string mErrorMessage;
                                if (Globals.CheckValidRegistrationForPay(RegistrationInfo, out mErrorMessage, this.IsConfirmHIView ? (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN : (long)AllLookupValues.V_TradingPlaces.NHA_THUOC))
                                {
                                    Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                                    {
                                        vm.CallComfirmOnPay = CallComfirmOnPay;
                                        vm.Registration = RegistrationInfo;
                                        vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                        vm.FormMode = PaymentFormMode.PAY;
                                        if (SelectedOutInvoice.ModFromOutiID > 0)
                                        {
                                            vm.AllowZeroPayment = true;
                                        }
                                        vm.RegistrationDetails = null;
                                        vm.PclRequests = null;
                                        if (IsConfirmHIView)
                                        {
                                            vm.RegistrationDetails = RegistrationInfo.PatientRegistrationDetails.Where(x => x.PaidTime == null && (x.RecordState == RecordState.UNCHANGED || x.RecordState == RecordState.ADDED)).ToList();
                                            vm.PclRequests = RegistrationInfo.PCLRequests.Where(x => x.PaidTime == null && x.RecordState == RecordState.UNCHANGED).ToList();
                                            vm.PayNewService = true;
                                            vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN;
                                            //20181113TNHX: [BM0003235] Doesn't print PhieuChiDinh if Doctor already printed it and When call by IsConfirmHIView
                                            vm.IsConfirmHIView = IsConfirmHIView;
                                        }
                                        if (RegistrationInfo.DrugInvoices == null)
                                        {
                                            RegistrationInfo.DrugInvoices = new ObservableCollection<OutwardDrugInvoice>();
                                        }
                                        vm.DrugInvoices = RegistrationInfo.DrugInvoices.Where(x => (x.outiID == SelectedOutInvoice.outiID || this.IsConfirmHIView) && (x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                                        vm.StartCalculating();
                                    };
                                    GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
                                }
                                else
                                {
                                    //▼====== #004
                                    if (mErrorMessage != null)
                                    {
                                        MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    }
                                    //▲====== #004
                                }
                                //if (vm.TotalPayForSelectedItem != vm.TotalPaySuggested)
                                //{
                                //    Globals.ShowDialog(vm as Conductor<object>);
                                //}
                                //else
                                //{
                                //    var vm2 = Globals.GetViewModel<ISimplePay2>();
                                //    vm2.Registration = RegistrationInfo;
                                //    vm2.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                //    vm2.RegistrationDetails = null;
                                //    vm2.PclRequests = null;
                                //    vm2.ObjectState = SelectedOutInvoice.outiID;

                                //    vm2.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                                //    vm2.StartCalculating();
                                //    Globals.ShowDialog(vm2 as Conductor<object>);
                                //}
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogError(ex.ToString());
                                MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                            }
                            finally
                            {
                                isLoadingGetID = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogError(ex.ToString());
                    MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        private void GetRegistrationInfo_InPt(long PtRegistrationID)
        {
            isLoadingGetID = true;

            //KMx: Chỉ lấy những thông tin cần thiết của đăng ký thôi, không load hết (17/09/2014 17:31).
            //KMx: Bộ 5 Properties ở dưới phải bằng true hết thì mới lấy PayableSum được (18/09/2014 11:04).
            LoadRegistrationSwitch LoadRegisSwitch = new LoadRegistrationSwitch();
            LoadRegisSwitch.IsGetRegistrationDetails = true;
            LoadRegisSwitch.IsGetPCLRequests = true;
            LoadRegisSwitch.IsGetDrugInvoices = true;
            LoadRegisSwitch.IsGetPatientTransactions = true;
            LoadRegisSwitch.IsGetDrugClinicDeptInvoices = true;

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PatientRegistrationServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetRegistrationInfo_InPt(PtRegistrationID, 1, LoadRegisSwitch, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var RegistrationInfo = contract.EndGetRegistrationInfo_InPt(asyncResult);
                            var vm = Globals.GetViewModel<ISimplePay>();
                            vm.Registration = RegistrationInfo;
                            vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                            vm.FormMode = PaymentFormMode.PAY;

                            if (RegistrationInfo == null)
                            {
                                MessageBox.Show(eHCMSResources.K0299_G1_ChonDK);
                                return;
                            }

                            vm.RegistrationDetails = null;
                            vm.PclRequests = null;
                            if (RegistrationInfo.DrugInvoices == null)
                            {
                                RegistrationInfo.DrugInvoices = new ObservableCollection<OutwardDrugInvoice>();
                            }

                            vm.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                            vm.StartCalculating();

                            if (vm.TotalPayForSelectedItem != vm.TotalPaySuggested)
                            {
                                Action<ISimplePay> onInitDlg = delegate (ISimplePay _vm)
                                {
                                    _vm = vm;
                                };
                                GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
                            }
                            else
                            {
                                string mErrorMessage;
                                if (Globals.CheckValidRegistrationForPay(RegistrationInfo, out mErrorMessage))
                                {
                                    Action<ISimplePay2> onInitDlg = delegate (ISimplePay2 vm2)
                                    {
                                        vm2.Registration = RegistrationInfo;
                                        vm2.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                                        vm2.RegistrationDetails = null;
                                        vm2.PclRequests = null;
                                        vm2.ObjectState = SelectedOutInvoice.outiID;

                                        vm2.DrugInvoices = RegistrationInfo.DrugInvoices.Where(item => item.outiID == SelectedOutInvoice.outiID && (item.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                                        vm2.StartCalculating();
                                    };
                                    GlobalsNAV.ShowDialog<ISimplePay2>(onInitDlg);
                                }
                                else
                                {
                                    //▼====== #004
                                    if (mErrorMessage != null)
                                    {
                                        MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    }
                                    //▲====== #004
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }
        #endregion

        #region Total Price Member
        private decimal _TotalInvoicePrice;
        public decimal TotalInvoicePrice
        {
            get
            {
                return _TotalInvoicePrice;
            }
            set
            {
                if (_TotalInvoicePrice != value)
                {
                    _TotalInvoicePrice = value;
                    NotifyOfPropertyChange(() => TotalInvoicePrice);
                }
            }
        }

        private decimal _TotalHIPayment;
        public decimal TotalHIPayment
        {
            get
            {
                return _TotalHIPayment;
            }
            set
            {
                if (_TotalHIPayment != value)
                {
                    _TotalHIPayment = value;
                    NotifyOfPropertyChange(() => TotalHIPayment);
                }
            }
        }

        private decimal _TotalPatientPayment;
        public decimal TotalPatientPayment
        {
            get
            {
                return _TotalPatientPayment;
            }
            set
            {
                if (_TotalPatientPayment != value)
                {
                    _TotalPatientPayment = value;
                    NotifyOfPropertyChange(() => TotalPatientPayment);
                }
            }
        }

        private void SumTotalPrice(bool aIsComfirmHICalled = false)
        {
            //KMx: OnlyRoundResultForOutward = true: Tính tổng tiền BH trả rồi mới làm tròn. Ngược lại thì làm tròn tiền BH trả trên từng dòng thuốc rồi mới tính tổng(02/08/2014 18:24).
            bool onlyRoundResultForOutward = Globals.ServerConfigSection.PharmacyElements.OnlyRoundResultForOutward;

            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugs == null)
            {
                return;
            }

            double HIBenefit = 0;
            if (PatientInfo != null && PatientInfo.LatestRegistration != null)
            {
                HIBenefit = PatientInfo.LatestRegistration.PtInsuranceBenefit.GetValueOrDefault();
            }

            TotalInvoicePrice = 0;
            TotalHIPayment = 0;
            TotalPatientPayment = 0;

            SelectedOutInvoice.TotalInvoicePrice = 0;
            SelectedOutInvoice.TotalPatientPayment = 0;
            SelectedOutInvoice.TotalHIPayment = 0;
            SelectedOutInvoice.TotalPriceDifference = 0;
            SelectedOutInvoice.TotalCoPayment = 0;

            if (!onlyRoundResultForOutward)
            {
                if (SelectedOutInvoice.outiID == 0)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() > 0)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].HIBenefit = HIBenefit;
                        }
                        SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice = SelectedOutInvoice.OutwardDrugs[i].OutPrice * SelectedOutInvoice.OutwardDrugs[i].OutQuantity;
                        SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = (decimal)Math.Floor((double)(SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit));
                        SelectedOutInvoice.OutwardDrugs[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugs[i].TotalPrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;
                        if (SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment > 0)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment = (decimal)Math.Ceiling((double)(SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment - (SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity)));
                        }

                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment;
                    }
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                else
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment;
                    }
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                SelectedOutInvoice.TotalInvoicePrice = TotalInvoicePrice;
                SelectedOutInvoice.TotalPatientPayment = TotalPatientPayment;
                SelectedOutInvoice.TotalHIPayment = TotalHIPayment;
            }
            else
            {
                if (SelectedOutInvoice.outiID == 0)
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        if (SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() > 0)
                        {
                            SelectedOutInvoice.OutwardDrugs[i].HIBenefit = HIBenefit;
                        }
                        SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice = SelectedOutInvoice.OutwardDrugs[i].OutPrice * SelectedOutInvoice.OutwardDrugs[i].OutQuantity;

                        //SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = (decimal)Math.Floor((double)(SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit));
                        //KMx: Không được làm tròn giá BH trả, nếu không mẫu 25 sẽ bị lệch với báo cáo nhà thuốc khi tính lại tiền bảo hiểm mà bệnh nhân được hưởng (trên mẫu 25). (31/07/2014 17:43).
                        SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment = SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity * (decimal)HIBenefit;

                        SelectedOutInvoice.OutwardDrugs[i].TotalPatientPayment = SelectedOutInvoice.OutwardDrugs[i].TotalPrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;
                        if (SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment > 0)
                        {
                            //SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment = (decimal)Math.Ceiling((double)(SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment - (SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity)));
                            SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment = SelectedOutInvoice.OutwardDrugs[i].HIAllowedPrice.GetValueOrDefault() * SelectedOutInvoice.OutwardDrugs[i].OutQuantity - SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;
                        }

                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalPrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment;
                    }

                    TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                    TotalHIPayment = MathExt.Round(TotalHIPayment, aEMR.Common.Converters.MidpointRounding.AwayFromZero);

                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                else
                {
                    for (int i = 0; i < SelectedOutInvoice.OutwardDrugs.Count; i++)
                    {
                        TotalInvoicePrice += SelectedOutInvoice.OutwardDrugs[i].TotalInvoicePrice;
                        TotalHIPayment += SelectedOutInvoice.OutwardDrugs[i].TotalHIPayment;

                        SelectedOutInvoice.TotalPriceDifference += SelectedOutInvoice.OutwardDrugs[i].PriceDifference * SelectedOutInvoice.OutwardDrugs[i].OutQuantity;
                        SelectedOutInvoice.TotalCoPayment += SelectedOutInvoice.OutwardDrugs[i].TotalCoPayment;
                    }

                    TotalInvoicePrice = MathExt.Round(TotalInvoicePrice, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                    TotalHIPayment = MathExt.Round(TotalHIPayment, aEMR.Common.Converters.MidpointRounding.AwayFromZero);
                    TotalPatientPayment = TotalInvoicePrice - TotalHIPayment;
                }
                SelectedOutInvoice.TotalInvoicePrice = TotalInvoicePrice;
                SelectedOutInvoice.TotalPatientPayment = TotalPatientPayment;
                SelectedOutInvoice.TotalHIPayment = TotalHIPayment;
                if (aIsComfirmHICalled && PatientInfo != null && PatientInfo.LatestRegistration != null && TotalPatientPayment == 0
                    && (PatientInfo.LatestRegistration.InPatientBillingInvoices == null || PatientInfo.LatestRegistration.InPatientBillingInvoices.Count == 0 || !PatientInfo.LatestRegistration.InPatientBillingInvoices.Any(x => x.TotalPatientPayment != x.TotalPatientPaid)))
                {
                    ISimplePay vm = Globals.GetViewModel<ISimplePay>();
                    vm.Registration = PatientInfo.LatestRegistration;
                    vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                    vm.FormMode = PaymentFormMode.PAY;
                    if (SelectedOutInvoice.ModFromOutiID > 0)
                    {
                        vm.AllowZeroPayment = true;
                    }
                    vm.RegistrationDetails = null;
                    vm.PclRequests = null;
                    vm.RegistrationDetails = PatientInfo.LatestRegistration.PatientRegistrationDetails.Where(x => x.PaidTime == null && (x.RecordState == RecordState.UNCHANGED || x.RecordState == RecordState.ADDED)).ToList();
                    vm.PclRequests = PatientInfo.LatestRegistration.PCLRequests.Where(x => x.PaidTime == null && x.RecordState == RecordState.UNCHANGED).ToList();
                    vm.PayNewService = true;
                    vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN;
                    vm.IsConfirmHIView = IsConfirmHIView;
                    if (PatientInfo.LatestRegistration.DrugInvoices == null)
                    {
                        PatientInfo.LatestRegistration.DrugInvoices = new ObservableCollection<OutwardDrugInvoice>();
                    }
                    vm.DrugInvoices = PatientInfo.LatestRegistration.DrugInvoices.Where(x => (x.outiID == SelectedOutInvoice.outiID || this.IsConfirmHIView) && (x.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.SAVE)).ToList();
                    vm.StartCalculating();
                    if (vm.TotalPaySuggested == 0)
                    {
                        vm.PayCmd();
                    }
                }
            }
        }
        #endregion

        #region Tim Toa Thuoc De Ban Member

        public void Search_KeyUp_Pre(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchPrescriptionToSell();
            }
        }

        public void Search_KeyUp_PreHICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //if (SearchCriteria != null)
                //{
                //    SearchCriteria.HICardCode = (sender as TextBox).Text;
                //}
                SearchPrescriptionToSell();
            }
        }

        //Search_KeyUp_PrePrescriptID
        public void Search_KeyUp_PrePrescriptID(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //if (SearchCriteria != null)
                //{
                //    long value = 0;
                //    long.TryParse((sender as TextBox).Text, out value);
                //    SearchCriteria.PrescriptID = value;
                //}
                SearchPrescriptionToSell();
            }
        }

        private void LoadInfoCommon()
        {
            if (SelectedPrescription != null)
            {
                //Kien: Delete data of previous prescription
                SelectedSellVisitor = null;
                strNgayDung = "";
                ((PrescriptionView)this.GetView()).AutoDrug_Text.Text = "";
                ((PrescriptionView)this.GetView()).chbBH.IsChecked = false;
                //---------------------------------------------------------
                SelectedOutInvoice = null;
                SelectedOutInvoice = new OutwardDrugInvoice();
                SelectedOutInvoice.OutDate = Globals.ServerDate.Value;
                SelectedOutInvoice.SelectedPrescription = SelectedPrescription;
                SelectedOutInvoice.PrescriptID = SelectedPrescription.PrescriptID;
                SelectedOutInvoice.OutInvID = "";
                SelectedOutInvoice.outiID = 0;
                SelectedOutInvoice.PtRegistrationID = SelectedPrescription.PtRegistrationID;
                SelectedOutInvoice.CheckedPoint = false;

                HideShowColumnDelete();

                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);

                //KMx: Hàm SearchGetDrugForSellVisitorByPrescriptID() để trong Coroutine DoGetInfoPatientPrescription() luôn.
                //Vì SearchGetDrugForSellVisitorByPrescriptID() có dùng biến IsHIPatient. Mà biến IsHIPatient do Coroutine quyết định.
                //Nên sau khi Coroutine quyết định IsHIPatient xong thì mới gọi hàm SearchGetDrugForSellVisitorByPrescriptID()
                //SearchGetDrugForSellVisitorByPrescriptID();
                if (SelectedPrescription.PtRegistrationID.HasValue)
                {
                    Coroutine.BeginExecute(DoGetInfoPatientPrescription());
                }

                OutwardDrug_GetMaxDayBuyInsurance(SelectedPrescription.PatientID.GetValueOrDefault(), SelectedOutInvoice.outiID);
            }
        }

        int findPatient = 0;
        private IEnumerator<IResult> DoGetInfoPatientPrescription()
        {
            isLoadingInfoPatient = true;
            if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
            {
                findPatient = 1;
            }
            else
            {
                findPatient = 0;
            }

            var paymentTypeTask = new LoadPatientInfoByRegistrationTask(SelectedPrescription.PtRegistrationID, SelectedPrescription.PatientID, findPatient);
            yield return paymentTypeTask;
            PatientInfo = paymentTypeTask.CurrentPatient;

            try
            {
                if (!PatientInfo.AgeOnly.GetValueOrDefault())
                {
                    PatientInfo.DOBText = PatientInfo.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
                }
                PatientInfo.LatestRegistration = paymentTypeTask.CurrentPatient.LatestRegistration;
                NotifyOfPropertyChange(() => IsRefundMoney);
                NotifyOfPropertyChange(() => IsEnableCancelBillingButton);
                NotifyOfPropertyChange(() => IsHasBillingInvoice);
                PatientInfo.CurrentHealthInsurance = paymentTypeTask.CurrentPatient.CurrentHealthInsurance;
                PatientInfo.CurrentClassification = paymentTypeTask.CurrentPatient.CurrentClassification;
            }
            catch
            { }

            if (IsConfirmHIView)
            {
                var loadRegTask = new LoadRegistrationInfoTask(PatientInfo.LatestRegistration.PtRegistrationID, true);
                yield return loadRegTask;
                if (OldPaymentContent != null && OldServiceContent != null && OldPclContent != null && loadRegTask.Registration != null)
                {
                    PatientInfo.LatestRegistration = loadRegTask.Registration;
                    NotifyOfPropertyChange(() => IsRefundMoney);
                    NotifyOfPropertyChange(() => IsEnableCancelBillingButton);
                    NotifyOfPropertyChange(() => IsHasBillingInvoice);
                    OldPaymentContent.InitViewForPayments(PatientInfo.LatestRegistration);
                    OldServiceContent.UpdateServiceItemList(PatientInfo.LatestRegistration.PatientRegistrationDetails.ToList());
                    OldPclContent.PCLRequests = PatientInfo.LatestRegistration.PCLRequests.Where(x => x.RecordState != RecordState.DELETED && x.PatientPCLRequestIndicators != null).ToObservableCollection();
                    OldBillingContent.UpdateItemList(PatientInfo.LatestRegistration.InPatientBillingInvoices == null || !PatientInfo.LatestRegistration.InPatientBillingInvoices.Any(x => x.PaidTime != null && x.RecordState != RecordState.DELETED) ? null : PatientInfo.LatestRegistration.InPatientBillingInvoices.Where(x => x.PaidTime != null && x.RecordState != RecordState.DELETED).ToList());
                }
                else if (OldPaymentContent != null && OldServiceContent != null && OldPclContent != null)
                {
                    OldPaymentContent.InitViewForPayments(null);
                    OldServiceContent.UpdateServiceItemList(null);
                    OldPclContent.PCLRequests = null;
                    OldBillingContent.UpdateItemList(null);
                }
            }

            GetClassicationPatientPrescription();
            SearchGetDrugForSellVisitorByPrescriptID();
            //KMx: Sau khi load toa thuốc sẽ kiểm tra.
            //Trường hợp toa đã bán rồi, user vẫn đồng ý cập nhật lại toa thuốc.
            if (SelectedPrescription.V_PrescriptionIssuedCase == (long)AllLookupValues.V_PrescriptionIssuedCase.UPDATE_FROM_PRESCRIPT_WAS_SOLD)
            {
                MessageBox.Show(eHCMSResources.A0806_G1_Msg_ToaCNhatTuToaDaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            else
            {
                if (PatientInfo != null && PatientInfo.CurrentHealthInsurance != null && PatientInfo.CurrentHealthInsurance.ValidDateTo >= DateTime.Now.Date
                    && Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "thanh toán BHYT cho toa thuốc này!"))
                {
                    IsHIPatient = false;
                }
            }
            Int16 isobject = 0;
            if (IsHIPatient)
            {
                //if (SubDate(SelectedPrescription.IssuedDateTime.Value.Date, DateServer) >= Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.MaxDaySellPrescriptInsurance]))
                // Txd 25/05/2014 Replaced ConfigList
                if (SubDate(SelectedPrescription.IssuedDateTime.Value.Date, DateServer) >= Globals.ServerConfigSection.HealthInsurances.MaxDaySellPrescriptInsurance)
                {
                    MessageBox.Show(eHCMSResources.Z0895_G1_BHYTKgTraTienCHoToa);
                    IsHIPatient = false;
                }
            }
            if (IsHIPatient && !IsNotCountInsurance)
            {
                isobject = 1;
            }
            else
            {
                isobject = 0;
            }
            if (IsConfirmHIView)
            {
                spGetInBatchNumberAndPrice_ByPresciption(0, StoreID, isobject, true, SelectedPrescription.PtRegistrationID);
            }
            else if ((paymentTypeTask.CurrentPatient.LatestRegistration.OutPtTreatmentProgramID == null  
                || paymentTypeTask.CurrentPatient.LatestRegistration.OutPtTreatmentProgramID <= 0)
                && Globals.ServerConfigSection.CommonItems.EnableHIStore && !IsConfirmHIView
                && !SelectedPrescription.IsSold
                && SelectedPrescription.PrescriptionIssueHistory != null && SelectedPrescription.PrescriptionIssueHistory.HisID > 0)
            {
                MessageBox.Show(eHCMSResources.Z2379_G1_BNCoToaThuocBHChuaDuocXN, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            else if (!IsConfirmHIView && paymentTypeTask.CurrentPatient.LatestRegistration.OutPtTreatmentProgramID > 0)
            {
                spGetInBatchNumberAndPrice_ByPresciption(SelectedPrescription.IssueID, StoreID, isobject);
            }
            else
            {
                spGetInBatchNumberAndPrice_ByPresciption(SelectedPrescription.IssueID, StoreID, isobject);
            }
            isLoadingInfoPatient = false;
            Globals.EventAggregator.Publish(new LoadDataCompleted<PatientRegistration> { Obj = PatientInfo.LatestRegistration });
            yield break;
        }

        // TxD 02/08/2014: The following methid is nolonger required
        //public void GetCurrentDate()
        //{
        //    isLoadingGetID = true;
        //    var t = new Thread(() =>
        //    {
        //        try
        //        {
        //            using (var serviceFactory = new CommonServiceClient())
        //            {
        //                var contract = serviceFactory.ServiceInstance;

        //                contract.BeginGetDate(Globals.DispatchCallback((asyncResult) =>
        //                {
        //                    try
        //                    {
        //                        DateTime date = contract.EndGetDate(asyncResult);
        //                        SearchCriteria.FromDate = date.AddDays(-1);
        //                        SearchCriteria.ToDate = date;
        //                        DateServer = date;


        //                    }
        //                    catch (FaultException<AxException> fault)
        //                    {
        //                        ClientLoggerHelper.LogInfo(fault.ToString());
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ClientLoggerHelper.LogInfo(ex.ToString());
        //                    }

        //                }), null);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //        }
        //        finally
        //        {
        //            isLoadingGetID = false;
        //        }
        //    });
        //    t.Start();
        //}

        private DateTime? _NgayBHGanNhat;
        public DateTime? NgayBHGanNhat
        {
            get { return _NgayBHGanNhat; }
            set
            {
                _NgayBHGanNhat = value;
                NotifyOfPropertyChange(() => NgayBHGanNhat);
            }
        }
        public void OutwardDrug_GetMaxDayBuyInsurance(long PatientID, long outiID)
        {
            isLoadingGetID = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        contract.BeginOutwardDrug_GetMaxDayBuyInsurance(PatientID, outiID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                NgayBHGanNhat = contract.EndOutwardDrug_GetMaxDayBuyInsurance(asyncResult);
                            }
                            catch (FaultException<AxException> fault)
                            {
                                ClientLoggerHelper.LogInfo(fault.ToString());
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
                finally
                {
                    isLoadingGetID = false;
                }
            });
            t.Start();
        }

        public void btnSearchNangCao()
        {
            IPrescriptionSearch DialogView = Globals.GetViewModel<IPrescriptionSearch>();
            DialogView.SearchCriteria = SearchCriteria.DeepCopy();
            DialogView.FormType = 1;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        private void SearchPrescription(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator();
            int Total = 0;
            //isLoadingSearch = true;

            if (SearchCriteria == null)
            {
                MessageBox.Show("Không có giá trị để tìm kiếm.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            //KMx: Nếu tìm kiếm mà không có ngày thì phải gán ngày (11/08/2014 09:49).
            if (SearchCriteria.FromDate == null || SearchCriteria.FromDate == DateTime.MinValue)
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-1);
            }
            if (SearchCriteria.ToDate == null || SearchCriteria.ToDate == DateTime.MinValue)
            {
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }

            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchPrescription(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndSearchPrescription(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //if (this.IsConfirmHIView)
                                    //{
                                    //    PrescriptionCollection = results.ToObservableCollection();
                                    //    LoadPrescriptionInfo();
                                    //    return;
                                    //}
                                    // mo pop up tim
                                    IPrescriptionSearch DialogView = Globals.GetViewModel<IPrescriptionSearch>();
                                    DialogView.SearchCriteria = SearchCriteria.DeepCopy();
                                    DialogView.FormType = 1;
                                    DialogView.PrescriptionList.Clear();
                                    DialogView.PrescriptionList.TotalItemCount = Total;
                                    DialogView.PrescriptionList.PageIndex = 0;
                                    DialogView.PrescriptionList.PageSize = PageSize;
                                    foreach (Prescription p in results)
                                    {
                                        DialogView.PrescriptionList.Add(p);
                                    }
                                    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
                                }
                                else
                                {
                                    SelectedPrescription = results.FirstOrDefault();
                                    if (SelectedPrescription.IsSold)
                                    {
                                        if (MessageBox.Show(eHCMSResources.Z0896_G1_ToaDaBanCoMuonBanTiepKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                        {
                                            return;
                                        }
                                    }
                                    LoadPrescriptionInfo();
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }

                            SearchCriteria = new PrescriptionSearchCriteria();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingSearch = false;
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void LoadPrescriptionInfo()
        {
            //Đọc thông tin đăng ký lên
            if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                Coroutine.BeginExecute(GetRegistrationInfoTask(SelectedPrescription.PtRegistrationID.Value));
            }
            else
            {
                Coroutine.BeginExecute(GetRegistrationInfo_InPtTask(SelectedPrescription.PtRegistrationID.Value));
            }
        }

        //private ObservableCollection<GetDrugForSellVisitor> _lstDrugInPrescription;
        //public ObservableCollection<GetDrugForSellVisitor> lstDrugInPrescription
        //{
        //    get { return _lstDrugInPrescription; }
        //    set
        //    {
        //        _lstDrugInPrescription = value;
        //        NotifyOfPropertyChange(()=>lstDrugInPrescription);
        //    }
        //}
        //private ObservableCollection<GetDrugForSellVisitor> lstDrugInPrescriptionDeleted;

        //private GetDrugForSellVisitor _DrugInPrescriptionItem;
        //public GetDrugForSellVisitor DrugInPrescriptionItem
        //{
        //    get { return _DrugInPrescriptionItem; }
        //    set
        //    {
        //        _DrugInPrescriptionItem = value;
        //        NotifyOfPropertyChange(() => DrugInPrescriptionItem);
        //    }
        //}

        public void spGetInBatchNumberAndPrice_ByPresciption(long aPresciptionID, long aStoreID, Int16 aIsObject, bool IsIssueID = true, long? PtRegistrationID = null)
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            isLoadingDetail = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
                    contract.BeginGetInBatchNumberAndPrice_ByPresciption_V3(aPresciptionID, aStoreID, aIsObject, IsHIOutPt, true, PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInBatchNumberAndPrice_ByPresciption_V3(asyncResult);
                            if (results != null)
                            {
                                if (SelectedOutInvoice != null)
                                {
                                    SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                    OutwardDrugsCopy = null;
                                    SumTotalPrice();
                                    if (SelectedOutInvoice != null && SelectedOutInvoice.OutwardDrugs != null && this.IsConfirmHIView)
                                    {
                                        CV_Prescriptions_Source = new CollectionViewSource { Source = SelectedOutInvoice.OutwardDrugs };
                                        CV_Prescriptions = (CollectionView)CV_Prescriptions_Source.View;
                                        if (SelectedOutInvoice.PaidTime != null)
                                        {
                                            CV_Prescriptions.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("PrescriptionGroupString"));
                                            IsInvoiceSearch = true;
                                        }
                                        NotifyOfPropertyChange(() => CV_Prescriptions);
                                        CV_Prescriptions.Refresh();
                                    }
                                }
                                if (SelectedPrescription != null && SelectedPrescription.IsEmptyPrescription && (PatientInfo != null && PatientInfo.LatestRegistration != null && PatientInfo.LatestRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0))
                                {
                                    IsInvoiceSearch = true;
                                }
                                else
                                {
                                    IsInvoiceSearch = false;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            this.HideBusyIndicator();
                        }
                    }), null);
                }
            });
            t.Start();
        }

        public void btnClick_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //mai coi lai
            if (e.ClickCount == 1)
            {
                SearchPrescriptionToSell();
            }
        }

        public void SearchPrescriptionToSell()
        {
            if (_currentView != null)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardCode = _currentView.GetValuePreHICode();
                    //long value = 0;
                    //long.TryParse(_currentView.GetValuePreID(), out value);
                    //SearchCriteria.PrescriptID = value;
                    SearchCriteria.PrescriptionIssueCode = _currentView.GetValuePreID(); //20181213 BM 0005414 TBL: Ma toa bay gio da doi sang PrescriptionIssueCode
                }
            }
            SearchPrescription(0, 20);
            //GetCurrentDate();
        }

        #region IHandle<PharmacyCloseSearchPrescriptionEvent> Members
        public void LoadPrescriptionDetail(Prescription aPrescription)
        {
            SelectedPrescription = aPrescription;
            CurrentChoNhanThuoc = aPrescription; // #011
            if (SelectedPrescription.IsSold)
            {
                if (IsConfirmHIView)
                {
                    ApplySearchInvoiceCriteriaValues(null, SelectedPrescription.PtRegistrationID);
                    SearchInvoiceOld();
                    return;
                }
                if (MessageBox.Show(eHCMSResources.Z0896_G1_ToaDaBanCoMuonBanTiepKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NGOAI_TRU)
            {
                //Đọc thông tin đăng ký lên
                Coroutine.BeginExecute(GetRegistrationInfoTask(SelectedPrescription.PtRegistrationID.Value));
            }
            else
            {
                Coroutine.BeginExecute(GetRegistrationInfo_InPtTask(SelectedPrescription.PtRegistrationID.Value));
            }
        }
        public void Handle(PharmacyCloseSearchPrescriptionEvent message)
        {
            if (message != null)
            {
                LoadPrescriptionDetail(message.SelectedPrescription as Prescription);
            }
        }
        #endregion

        #endregion

        #region Phieu Xuat Member
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchInvoiceOld();
            }
        }

        public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //if (SearchInvoiceCriteria != null)
                //{
                //    SearchInvoiceCriteria.HICardCode = (sender as TextBox).Text;
                //}
                SearchInvoiceOld();
            }
        }

        //OutInvID
        public void Search_KeyUp_OutInvID(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //if (SearchInvoiceCriteria != null)
                //{
                //    SearchInvoiceCriteria.OutInvID = (sender as TextBox).Text;
                //}
                SearchInvoiceOld();
            }
        }

        public void SearchInvoiceOld()
        {
            if (_currentView != null)
            {
                if (SearchInvoiceCriteria != null)
                {
                    SearchInvoiceCriteria.HICardCode = _currentView.GetValueHICode();
                    SearchInvoiceCriteria.OutInvID = _currentView.GetValueInvoiceID();
                }
            }
            SearchOutwardPrescriptionIndex0(0, Globals.PageSize);
        }

        //▼====: #006
        private void LoadInfoCommonInvoice(bool isCallByPayViewModel = false, bool aIsComfirmHICalled = false)
        {
            //▲====: #006
            if (SelectedOutInvoice != null)
            {
                //Kien: Delete data of previous prescription
                SelectedSellVisitor = null;
                strNgayDung = "";
                ((PrescriptionView)this.GetView()).AutoDrug_Text.Text = "";
                ((PrescriptionView)this.GetView()).chbBH.IsChecked = false;
                //---------------------------------------------------------

                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
                HideShowColumnDelete();
                if (SelectedOutInvoice.SelectedPrescription != null)
                {
                    //KMx: Hàm SearchGetDrugForSellVisitorByPrescriptID() để trong Coroutine DoGetInfoPatientPrescription() luôn.
                    //Vì SearchGetDrugForSellVisitorByPrescriptID() có dùng biến IsHIPatient. Mà biến IsHIPatient do Coroutine quyết định.
                    //Nên sau khi Coroutine quyết định IsHIPatient xong thì mới gọi hàm SearchGetDrugForSellVisitorByPrescriptID()
                    //SearchGetDrugForSellVisitorByPrescriptID();
                    if (SelectedOutInvoice.SelectedPrescription.PtRegistrationID.HasValue)
                    {
                        //▼====: #006
                        Coroutine.BeginExecute(DoGetInfoPatientInvoice(isCallByPayViewModel, aIsComfirmHICalled));
                        //▲====: #006
                    }
                    //OutwardDrug_GetMaxDayBuyInsurance(SelectedOutInvoice.SelectedPrescription.PatientID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                    //KMx: Không được dùng SelectedOutInvoice.SelectedPrescription.PatientID vì sau khi tính tiền thì PatientID = null.
                    OutwardDrug_GetMaxDayBuyInsurance(SelectedPrescription.PatientID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                }
            }
        }

        //▼====: #006
        private IEnumerator<IResult> DoGetInfoPatientInvoice(bool isCallByPayViewModel = false, bool aIsComfirmHICalled = false)
        {
            //▲====: #006
            isLoadingInfoPatient = true;
            long? PtRegistrationID = null;
            long? PatientID = null;
            if (SelectedOutInvoice != null && SelectedOutInvoice.SelectedPrescription != null)
            {
                PtRegistrationID = SelectedOutInvoice.SelectedPrescription.PtRegistrationID;
                PatientID = SelectedOutInvoice.SelectedPrescription.PatientID;
                if (SelectedOutInvoice.SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    findPatient = 1;
                }
                else
                {
                    findPatient = 0;
                }
            }
            var paymentTypeTask = new LoadPatientInfoByRegistrationTask(PtRegistrationID, PatientID, findPatient);
            yield return paymentTypeTask;
            PatientInfo = paymentTypeTask.CurrentPatient;

            try
            {
                if (!PatientInfo.AgeOnly.GetValueOrDefault())
                {
                    PatientInfo.DOBText = PatientInfo.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
                }
                PatientInfo.LatestRegistration = paymentTypeTask.CurrentPatient.LatestRegistration;
                NotifyOfPropertyChange(() => IsRefundMoney);
                NotifyOfPropertyChange(() => IsEnableCancelBillingButton);
                NotifyOfPropertyChange(() => IsHasBillingInvoice);
                PatientInfo.CurrentHealthInsurance = paymentTypeTask.CurrentPatient.CurrentHealthInsurance;
                PatientInfo.CurrentClassification = paymentTypeTask.CurrentPatient.CurrentClassification;
            }
            catch
            {
            }

            if (IsConfirmHIView)
            {
                var loadRegTask = new LoadRegistrationInfoTask(PatientInfo.LatestRegistration.PtRegistrationID, true);
                yield return loadRegTask;
                if (OldPaymentContent != null && OldServiceContent != null && OldPclContent != null && loadRegTask.Registration != null)
                {
                    PatientInfo.LatestRegistration = loadRegTask.Registration;
                    NotifyOfPropertyChange(() => IsRefundMoney);
                    NotifyOfPropertyChange(() => IsEnableCancelBillingButton);
                    NotifyOfPropertyChange(() => IsHasBillingInvoice);
                    OldPaymentContent.InitViewForPayments(PatientInfo.LatestRegistration);
                    OldServiceContent.UpdateServiceItemList(PatientInfo.LatestRegistration.PatientRegistrationDetails.ToList());
                    OldPclContent.PCLRequests = PatientInfo.LatestRegistration.PCLRequests.Where(x => x.RecordState != RecordState.DELETED && x.PatientPCLRequestIndicators != null).ToObservableCollection();
                    OldBillingContent.UpdateItemList(PatientInfo.LatestRegistration.InPatientBillingInvoices == null || !PatientInfo.LatestRegistration.InPatientBillingInvoices.Any(x => x.PaidTime != null && x.RecordState != RecordState.DELETED) ? null : PatientInfo.LatestRegistration.InPatientBillingInvoices.Where(x => x.PaidTime != null && x.RecordState != RecordState.DELETED).ToList());
                }
                else if (OldPaymentContent != null && OldServiceContent != null && OldPclContent != null)
                {
                    OldPaymentContent.InitViewForPayments(null);
                    OldServiceContent.UpdateServiceItemList(null);
                    OldPclContent.PCLRequests = null;
                    OldBillingContent.UpdateItemList(null);
                }
            }

            GetClassicationPatientInvoice();
            SearchGetDrugForSellVisitorByPrescriptID();
            if (IsConfirmHIView && (PatientInfo != null && PatientInfo.LatestRegistration != null && PatientInfo.LatestRegistration.PtInsuranceBenefit.GetValueOrDefault(0) > 0))
            {
                //▼====: #006
                LoadPrescriptionDetailByInvoice(null, SelectedOutInvoice.PtRegistrationID, isCallByPayViewModel, aIsComfirmHICalled);
                //▲====: #006
            }
            else
            {
                LoadPrescriptionDetailByInvoice(SelectedOutInvoice.outiID);
            }
            isLoadingInfoPatient = false;
            Globals.EventAggregator.Publish(new LoadDataCompleted<PatientRegistration> { Obj = PatientInfo.LatestRegistration });
            yield break;
        }

        //▼====: #006
        private void LoadPrescriptionDetailByInvoice(long[] OutiIDArray, long? PtRegistrationID, bool isCallByPayViewModel = false, bool aIsComfirmHICalled = false)
        {
            //▲====: #006
            isLoadingDetailPrescript = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //20181011 TBL: Edit parameter long OutiID
                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice_V2(SelectedOutInvoice.outiID, IsHIOutPt, OutiIDArray, PtRegistrationID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice_V2(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                if (SelectedOutInvoice.OutwardDrugs != null)
                                {
                                    OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                                }
                                CV_Prescriptions_Source = new CollectionViewSource { Source = SelectedOutInvoice.OutwardDrugs };
                                CV_Prescriptions = (CollectionView)CV_Prescriptions_Source.View;
                                CV_Prescriptions.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("PrescriptionGroupString"));
                                NotifyOfPropertyChange(() => CV_Prescriptions);
                                SumTotalPrice(aIsComfirmHICalled);
                                IsInvoiceSearch = true;
                                //▼====: #006
                                if (isCallByPayViewModel)
                                {
                                    btnPreview();
                                }
                                //▲====: #006
                            }
                            SelectedOutInvoiceCopy = SelectedOutInvoice.DeepCopy();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingDetailPrescript = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);

                }
            });
            t.Start();
        }

        //load chi tiet phieu xuat ban hang theo toa dua vao ma phieu xuat
        private void LoadPrescriptionDetailByInvoice(long OutiID)
        {
            isLoadingDetailPrescript = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice(OutiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult);
                            if (results != null)
                            {
                                SelectedOutInvoice.OutwardDrugs = results.ToObservableCollection();
                                if (SelectedOutInvoice.OutwardDrugs != null)
                                {
                                    OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs.DeepCopy();
                                }
                                SumTotalPrice();
                            }
                            SelectedOutInvoiceCopy = SelectedOutInvoice.DeepCopy();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingDetailPrescript = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });
            t.Start();
        }

        //tim phieu xuat ban theo toa
        private void SearchOutwardPrescriptionIndex0(int PageIndex, int PageSize)
        {
            int Total = 0;
            this.ShowBusyIndicator();
            //isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

            //KMx: Nếu tìm kiếm mà không theo tiêu chí nào hết thì phải giới hạn ngày (08/08/2014 09:51).
            if (SearchInvoiceCriteria == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchInvoiceCriteria.PatientCode) && string.IsNullOrEmpty(SearchInvoiceCriteria.CustomerName) && string.IsNullOrEmpty(SearchInvoiceCriteria.HICardCode) && string.IsNullOrEmpty(SearchInvoiceCriteria.OutInvID))
            {
                SearchInvoiceCriteria.fromdate = Globals.GetCurServerDateTime().AddDays(-1);
                SearchInvoiceCriteria.todate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchInvoiceCriteria.fromdate = null;
                SearchInvoiceCriteria.todate = null;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchInvoiceCriteria, PageIndex, PageSize, true, null, false, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutWardDrugInvoiceSearchAllByStatus(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1 && this.IsConfirmHIView && results.Select(x => x.PtRegistrationID).Distinct().Count() == 1)
                                {
                                    OutwardDrugInvoiceCollection = results.ToObservableCollection();
                                    SelectedOutInvoice = results.OrderBy(x => x.V_OutDrugInvStatus).ThenByDescending(x => x.PaidTime).ThenByDescending(x => x.IsHICount).ThenByDescending(x => x.StoreID).FirstOrDefault();
                                    SelectedPrescription = SelectedOutInvoice.SelectedPrescription;
                                    LoadInfoCommonInvoice();
                                    return;
                                }
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    IPrescriptionSearchInvoice DialogView = Globals.GetViewModel<IPrescriptionSearchInvoice>();
                                    DialogView.SearchInvoiceCriteria = SearchInvoiceCriteria.DeepCopy();
                                    DialogView.OutwardDrugInvoices.Clear();
                                    DialogView.OutwardDrugInvoices.TotalItemCount = Total;
                                    DialogView.OutwardDrugInvoices.PageIndex = 0;
                                    DialogView.OutwardDrugInvoices.PageSize = 20;
                                    foreach (OutwardDrugInvoice p in results)
                                    {
                                        DialogView.OutwardDrugInvoices.Add(p);
                                    }
                                    GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
                                }
                                else
                                {
                                    SelectedOutInvoice = results.FirstOrDefault();
                                    //KMx: Phải cập nhật SelectedPrescription, nếu không toa vừa load lên sẽ lấy PrescriptID của toa cũ. 
                                    SelectedPrescription = SelectedOutInvoice.SelectedPrescription;
                                    LoadInfoCommonInvoice();
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                            //KMx: Sau khi set new thì phải set TypID lại, để lần sau tìm phiếu xuất của bán theo toa thôi. Nếu không là bị lỗi tìm phiếu xuất của bán lẻ luôn (25/02/2013 18:03)
                            SearchInvoiceCriteria = new SearchOutwardInfo();
                            SearchInvoiceCriteria.TypID = (long)AllLookupValues.RefOutputType.BANTHEOTOA;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        //▼====: #006
        private void GetOutWardDrugInvoiceVisitorByID(long OutwardID, bool isCallByPayViewModel = false)
        {
            isLoadingGetID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutWardDrugInvoiceVisitorByID(OutwardID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            SelectedOutInvoice = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                            //▼====: #006
                            LoadInfoCommonInvoice(isCallByPayViewModel);
                            //▲====: #006
                        }
                        catch (Exception ex)
                        {
                            isLoadingGetID = false;
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        #region IHandle<PharmacyCloseSearchPrescriptionInvoiceEvent> Members

        public void Handle(PharmacyCloseSearchPrescriptionInvoiceEvent message)
        {
            if (message != null)
            {
                SelectedOutInvoice = message.SelectedInvoice as OutwardDrugInvoice;
                SelectedPrescription = SelectedOutInvoice.SelectedPrescription;
                LoadInfoCommonInvoice();
                GetStaffLogin();
                
            }
        }
        #endregion
        #endregion

        #region DataGrid Event Member

        DataGrid grdPrescription = null;
        DataGrid grdPrescriptionCV = null;
        public void grdPrescription_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescription = sender as DataGrid;
        }
        public void grdPrescriptionCV_Loaded(object sender, RoutedEventArgs e)
        {
            grdPrescriptionCV = sender as DataGrid;
        }

        public void grdPrescription_Unloaded(object sender, RoutedEventArgs e)
        {
            grdPrescription.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        public void grdPrescription_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            if (!CheckQuantity(e.Row.DataContext))
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
            if (SelectedOutInvoice != null && SelectedOutInvoice.CanSaveAndPaidPrescript)
            {
                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = true;
                }
            }
            else
            {
                Button colBatchNumber = grdPrescription.Columns[(int)DataGridCol.LoSX].GetCellContent(e.Row) as Button;
                if (colBatchNumber != null)
                {
                    colBatchNumber.IsEnabled = false;
                }
            }
        }

        public void grdPrescription_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            SumTotalPrice();
            //tam thoi ko can hien chi tiet nen ko can goi ham tinh nay
        }

        #endregion

        #region printing member
        //▼====: #003
        // 20192002 TNHX Update Report for Printer Bisollon (In Nhiệt)
        public void btnPreview()
        {
            //▼====: #005
            //▼====: #006
            if (SelectedOutInvoice == null || SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED || SelectedOutInvoice.OutwardDrugs == null || SelectedOutInvoice.OutwardDrugs.Count == 0)
            {
                return;
            }
            //▼====: #007
            var printerConfigManager = new PrinterConfigurationManager();
            var allAssignedPrinterTypes = printerConfigManager.GetAllAssignedPrinterType();
            if (allAssignedPrinterTypes.ContainsKey(PrinterType.IN_NHIET) && allAssignedPrinterTypes[PrinterType.IN_NHIET] != ""
                && Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
            {
                if (Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<Root>");
                    sb.Append("<IDList>");

                    foreach (var item in SelectedOutInvoice.OutwardDrugs.Select(x => x.outiID).Distinct())
                    {
                        sb.AppendFormat("<ID>{0}</ID>", item);
                    }
                    //▲====: #006
                    sb.Append("</IDList>");
                    sb.Append("</Root>");

                    this.ShowBusyIndicator();
                    var t = new Thread(() =>
                    {
                        using (var serviceFactory = new ReportServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            // 20192002 TNHX Update Report for Printer Bisollon (In Nhiệt)
                            contract.BeginGetCollectionDrugForThermalSummaryInPdfFormat(sb.ToString(), Globals.ServerConfigSection.CommonItems.ReportHospitalName, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    var results = contract.EndGetCollectionDrugForThermalSummaryInPdfFormat(asyncResult);
                                    var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, results, ActiveXPrintType.ByteArray, "A5");
                                    Globals.EventAggregator.Publish(printEvt);
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
                    t.Start();
                }
                else
                {
                    if (SelectedOutInvoice.IsHICount.GetValueOrDefault(false))
                    {
                        this.ShowBusyIndicator();
                        var t = new Thread(() =>
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                // 20192002 TNHX Update Report PhieuNhanThuoc BHYT for Printer Bisollon (In Nhiệt)
                                contract.BeginGetCollectionDrugBHYTForThermalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetCollectionDrugBHYTForThermalInPdfFormat(asyncResult);
                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, results, ActiveXPrintType.ByteArray, "A5");
                                        Globals.EventAggregator.Publish(printEvt);
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
                        t.Start();
                    }
                    else
                    {
                        this.ShowBusyIndicator();
                        var t = new Thread(() =>
                        {
                            using (var serviceFactory = new ReportServiceClient())
                            {
                                var contract = serviceFactory.ServiceInstance;
                                // 20192002 TNHX Update Report for Printer Bisollon (In Nhiệt)
                                contract.BeginGetCollectionDrugForThermalInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                                {
                                    try
                                    {
                                        var results = contract.EndGetCollectionDrugForThermalInPdfFormat(asyncResult);
                                        var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_NHIET, results, ActiveXPrintType.ByteArray, "A5");
                                        Globals.EventAggregator.Publish(printEvt);
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
                        t.Start();
                    }
                }
            }
            else
            {
                //▲====: #007
                Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
                {
                    if (Globals.ServerConfigSection.CommonItems.MixedHIPharmacyStores)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<Root>");
                        sb.Append("<IDList>");

                        foreach (var item in SelectedOutInvoice.OutwardDrugs.Select(x => x.outiID).Distinct())
                        {
                            sb.AppendFormat("<ID>{0}</ID>", item);
                        }
                        //▲====: #006
                        sb.Append("</IDList>");
                        sb.Append("</Root>");

                        proAlloc.PatientID = PatientInfo.PatientID;
                        proAlloc.ID = SelectedOutInvoice.outiID;

                        if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
                        {
                            proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_SUMMARY;
                            proAlloc.ListID = sb.ToString();
                        }
                        else
                        {
                            proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_PRIVATE;
                        }
                    }
                    else
                    {
                        proAlloc.PatientID = PatientInfo.PatientID;
                        proAlloc.ID = SelectedOutInvoice.outiID;
                        if (SelectedOutInvoice.IsHICount.GetValueOrDefault(false))
                        {
                            proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_BH;
                        }
                        else
                        {
                            if (Globals.ServerConfigSection.CommonItems.OrganizationUseSoftware == 0)
                            {
                                proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC;
                            }
                            else
                            {
                                proAlloc.eItem = ReportName.PHARMACY_PHIEUNHANTHUOC_PRIVATE;
                            }
                        }
                    }
                };
                GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, null);
            }
        }
        //▲====: #003
        //▲====: #005

        public void btnPrintReceipt()
        {
            GenericPayment rptGenpayment = new GenericPayment();
            rptGenpayment.PatientCode = PatientInfo.PatientCode;
            rptGenpayment.FileCodeNumber = PatientInfo.FileCodeNumber;
            rptGenpayment.PersonName = PatientInfo.FullName;
            rptGenpayment.PersonAddress = PatientInfo.PatientStreetAddress;
            rptGenpayment.PhoneNumber = !string.IsNullOrWhiteSpace(PatientInfo.PatientCellPhoneNumber) ? PatientInfo.PatientCellPhoneNumber : (!string.IsNullOrWhiteSpace(PatientInfo.PatientPhoneNumber) ? PatientInfo.PatientPhoneNumber : "");
            rptGenpayment.GenericPaymentCode = SelectedOutInvoice.OutInvID;
            rptGenpayment.V_GenericPaymentReason = "Thu tiền bán thuốc";
            rptGenpayment.StaffName = SelectedOutInvoice.SelectedStaff != null ? SelectedOutInvoice.SelectedStaff.FullName : "";
            rptGenpayment.PaymentDate = SelectedOutInvoice.OutDate;
            rptGenpayment.DOB = PatientInfo.DOBText;
            rptGenpayment.PaymentAmount = SelectedOutInvoice.TotalPatientPayment;
            rptGenpayment.VATAmount = SelectedOutInvoice.TotalPatientPayment - (SelectedOutInvoice.TotalPatientPayment / (decimal)1.05);
            rptGenpayment.VATPercent = 1.05;

            Action<ICommonPreviewView> onInitDlg = delegate (ICommonPreviewView reportVm)
            {
                reportVm.CurGenPaymt = rptGenpayment;
                reportVm.eItem = ReportName.PHIEU_THU_KHAC_V4;
            };
            GlobalsNAV.ShowDialog<ICommonPreviewView>(onInitDlg);
        }

        private void PrintSilient()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetCollectionDrugInPdfFormat(SelectedOutInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetCollectionDrugInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray, "A5");
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });
            t.Start();
        }

        public void btnPrint()
        {
            if (SelectedOutInvoice == null) return;
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0);
                proAlloc.eItem = ReportName.TEMP38a;

                if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                {
                    proAlloc.StaffFullName = GetStaffLogin().FullName;
                }
                else
                {
                    proAlloc.StaffFullName = "";
                }
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, new Size(1500, 1000));
        }

        public void btnPrint12()
        {
            if (SelectedOutInvoice == null) return;
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0);
                // Ngoại trú thì áp trực tiếp
                if (Globals.ServerConfigSection.CommonItems.ApplyTemp12Version6556)
                {
                    proAlloc.eItem = ReportName.TEMP12_6556;
                }
                else
                {
                proAlloc.eItem = ReportName.TEMP12;
                }
                if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                {
                    proAlloc.StaffFullName = GetStaffLogin().FullName;
                }
                else
                {
                    proAlloc.StaffFullName = "";
                }
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, new Size(1500, 1000));
        }
        #endregion

        #region auto Drug For Prescription member
        private string BrandName;
        private byte HI;
        private bool IsHIPatient = false;

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListByPrescriptID;
        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorList;

        private ObservableCollection<GetDrugForSellVisitor> _GetDrugForSellVisitorSum;
        public ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorListSum
        {
            get { return _GetDrugForSellVisitorSum; }
            set
            {
                if (_GetDrugForSellVisitorSum != value)
                    _GetDrugForSellVisitorSum = value;
                NotifyOfPropertyChange(() => GetDrugForSellVisitorListSum);
            }
        }

        private ObservableCollection<GetDrugForSellVisitor> GetDrugForSellVisitorTemp;
        private GetDrugForSellVisitor _SelectedSellVisitor;
        public GetDrugForSellVisitor SelectedSellVisitor
        {
            get { return _SelectedSellVisitor; }
            set
            {
                if (_SelectedSellVisitor != value)
                    _SelectedSellVisitor = value;
                NotifyOfPropertyChange(() => SelectedSellVisitor);
            }
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                SearchGetDrugForSellVisitor(e.Parameter, false);
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedSellVisitor = au.SelectedItem as GetDrugForSellVisitor;
            }
        }

        private void ListDisplayAutoComplete()
        {
            var hhh = from hd in GetDrugForSellVisitorList
                      group hd by new
                      {
                          hd.DrugID,
                          hd.DrugCode,
                          hd.BrandName,
                          hd.UnitName,
                          hd.Qty,
                          hd.DayRpts,
                          hd.QtyForDay,
                          hd.V_DrugType,
                          hd.QtyMaxAllowed,
                          hd.QtySchedMon,
                          hd.QtySchedTue,
                          hd.QtySchedWed,
                          hd.QtySchedThu,
                          hd.QtySchedFri,
                          hd.QtySchedSat,
                          hd.QtySchedSun,
                          hd.SchedBeginDOW,
                          hd.DispenseVolume,
                          hd.HIPaymentPercent
                      } into hdgroup
                      select new
                      {
                          Remaining = hdgroup.Sum(groupItem => groupItem.Remaining),
                          DrugID = hdgroup.Key.DrugID,
                          DrugCode = hdgroup.Key.DrugCode,
                          UnitName = hdgroup.Key.UnitName,
                          BrandName = hdgroup.Key.BrandName,
                          Qty = hdgroup.Key.Qty,
                          DayRpts = hdgroup.Key.DayRpts,
                          V_DrugType = hdgroup.Key.V_DrugType,
                          QtyForDay = hdgroup.Key.QtyForDay,
                          QtyMaxAllowed = hdgroup.Key.QtyMaxAllowed,
                          QtySchedMon = hdgroup.Key.QtySchedMon,
                          QtySchedTue = hdgroup.Key.QtySchedTue,
                          QtySchedWed = hdgroup.Key.QtySchedWed,
                          QtySchedThu = hdgroup.Key.QtySchedThu,
                          QtySchedFri = hdgroup.Key.QtySchedFri,
                          QtySchedSat = hdgroup.Key.QtySchedSat,
                          QtySchedSun = hdgroup.Key.QtySchedSun,
                          SchedBeginDOW = hdgroup.Key.SchedBeginDOW,
                          DispenseVolume = hdgroup.Key.DispenseVolume,
                          HIPaymentPercent = hdgroup.Key.HIPaymentPercent
                      };
            for (int i = 0; i < hhh.Count(); i++)
            {
                GetDrugForSellVisitor item = new GetDrugForSellVisitor();
                item.DrugID = hhh.ToList()[i].DrugID;
                item.DrugCode = hhh.ToList()[i].DrugCode;
                item.BrandName = hhh.ToList()[i].BrandName;
                item.UnitName = hhh.ToList()[i].UnitName;
                item.Remaining = hhh.ToList()[i].Remaining;
                item.Qty = hhh.ToList()[i].Qty;
                item.DayRpts = hhh.ToList()[i].DayRpts;
                item.V_DrugType = hhh.ToList()[i].V_DrugType;
                item.QtyForDay = hhh.ToList()[i].QtyForDay;
                item.QtyMaxAllowed = hhh.ToList()[i].QtyMaxAllowed;
                item.QtySchedMon = hhh.ToList()[i].QtySchedMon;
                item.QtySchedTue = hhh.ToList()[i].QtySchedTue;
                item.QtySchedWed = hhh.ToList()[i].QtySchedWed;
                item.QtySchedThu = hhh.ToList()[i].QtySchedThu;
                item.QtySchedFri = hhh.ToList()[i].QtySchedFri;
                item.QtySchedSat = hhh.ToList()[i].QtySchedSat;
                item.QtySchedSun = hhh.ToList()[i].QtySchedSun;
                item.SchedBeginDOW = hhh.ToList()[i].SchedBeginDOW;
                item.DispenseVolume = hhh.ToList()[i].DispenseVolume;
                item.HIPaymentPercent = hhh.ToList()[i].HIPaymentPercent;

                GetDrugForSellVisitorListSum.Add(item);
            }
            if (IsCode.GetValueOrDefault())
            {
                if (GetDrugForSellVisitorListSum != null && GetDrugForSellVisitorListSum.Count > 0)
                {
                    var item = GetDrugForSellVisitorListSum.Where(x => x.DrugCode == txt);
                    if (item != null && item.Count() > 0)
                    {
                        SelectedSellVisitor = item.ToList()[0];
                    }
                    else
                    {
                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                }
            }
            else
            {
                if (au != null)
                {
                    au.ItemsSource = GetDrugForSellVisitorListSum;
                    au.PopulateComplete();
                }
            }
        }

        private int SubDate(DateTime date1, DateTime date2)
        {
            TimeSpan subDate = date2 - date1;
            return subDate.Days;
        }

        private void SearchGetDrugForSellVisitorByPrescriptID()
        {
            long? PrescriptID = null;
            if (SelectedOutInvoice != null)
            {
                PrescriptID = SelectedOutInvoice.PrescriptID;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForPrescriptionByID(HI, IsHIPatient, StoreID, PrescriptID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            //lay tat ca nhung thuoc ton (tung lo) theo ma toa thuoc(co bao nhieu lo lay het ra luon) 
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForPrescriptionByID(asyncResult);
                            GetDrugForSellVisitorListByPrescriptID = results.ToObservableCollection();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private bool? IsCode = false;
        private void SearchGetDrugForSellVisitor(string Name, bool? IsCode)
        {
            if (IsCode.GetValueOrDefault())
            {
                isLoadingDetailPrescript = true;
            }

            long? PrescriptID = null;
            if (SelectedOutInvoice != null)
            {
                PrescriptID = SelectedOutInvoice.PrescriptID;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorAutoComplete_ForPrescription(HI, (IsHIPatient && !IsNotCountInsurance), Name, StoreID, PrescriptID, IsCode, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorAutoComplete_ForPrescription(asyncResult);
                            GetDrugForSellVisitorList.Clear();
                            GetDrugForSellVisitorListSum.Clear();
                            LaySoLuongTheoNgayDung(results);
                            ListDisplayAutoComplete();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            //▼====== #002
                            //if (IsCode.GetValueOrDefault())
                            //{
                            //    isLoadingDetailPrescript = false;
                            //    if (AxQty != null)
                            //    {
                            //        AxQty.Focus();
                            //    }
                            //}
                            //else
                            //{
                            //    if (au != null)
                            //    {
                            //        au.Focus();
                            //    }
                            //}
                            //▲====== #002
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void LaySoLuongTheoNgayDung(IList<GetDrugForSellVisitor> results)
        {
            GetDrugForSellVisitorTemp = results.ToObservableCollection();

            if (GetDrugForSellVisitorTemp == null)
            {
                GetDrugForSellVisitorTemp = new ObservableCollection<GetDrugForSellVisitor>();
            }

            if (OutwardDrugsCopy != null && OutwardDrugsCopy.Count > 0)
            {
                foreach (OutwardDrug d in OutwardDrugsCopy)
                {
                    var value = results.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetDrugForSellVisitor s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        GetDrugForSellVisitorTemp.Add(p);
                        // d = null;
                    }
                }
            }

            foreach (GetDrugForSellVisitor s in GetDrugForSellVisitorTemp)
            {
                if (SelectedOutInvoice.OutwardDrugs != null && SelectedOutInvoice.OutwardDrugs.Count > 0)
                {
                    foreach (OutwardDrug d in SelectedOutInvoice.OutwardDrugs)
                    {
                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
                GetDrugForSellVisitorList.Add(s);
            }
        }

        private bool CheckValidDrugAuto(GetDrugForSellVisitor temp)
        {
            if (temp == null)
            {
                return false;
            }
            return !temp.HasErrors;
        }

        private void CheckBatchNumberExists(OutwardDrug p)
        {
            bool kq = false;
            if (SelectedOutInvoice.OutwardDrugs != null)
            {
                foreach (OutwardDrug p1 in SelectedOutInvoice.OutwardDrugs)
                {
                    if (p.DrugID == p1.DrugID && p.InBatchNumber == p1.InBatchNumber && p.InID == p1.InID)
                    {
                        p1.OutQuantity += p.OutQuantity;
                        p1.QtyOffer += p.QtyOffer;
                        kq = true;
                        break;
                    }
                }
                if (!kq)
                {
                    p.HI = p.GetDrugForSellVisitor.InsuranceCover;

                    if (p.InwardDrug == null)
                    {
                        p.InwardDrug = new InwardDrug();
                        p.InwardDrug.InID = p.InID.GetValueOrDefault();
                        p.InwardDrug.DrugID = p.DrugID;
                    }
                    p.InvoicePrice = p.OutPrice;
                    p.InwardDrug.NormalPrice = p.OutPrice;
                    p.InwardDrug.HIPatientPrice = p.OutPrice;
                    p.InwardDrug.HIAllowedPrice = p.HIAllowedPrice;
                    if (p.HIAllowedPrice.GetValueOrDefault(-1) > 0)
                    {
                        p.PriceDifference = p.OutPrice - p.HIAllowedPrice.GetValueOrDefault(0);
                    }

                    SelectedOutInvoice.OutwardDrugs.Add(p);
                }
                txt = "";
                SelectedSellVisitor = null;
                if (IsCode.GetValueOrDefault())
                {
                    if (tbx != null)
                    {
                        tbx.Text = "";
                        tbx.Focus();
                    }
                }
                else
                {
                    if (au != null)
                    {
                        au.Text = "";
                        au.Focus();
                    }
                }
            }
        }

        private void ChooseBatchNumber(GetDrugForSellVisitor value)
        {
            var items = GetDrugForSellVisitorList.Where(x => x.DrugID == value.DrugID).OrderBy(p => p.STT);
            foreach (GetDrugForSellVisitor item in items)
            {
                OutwardDrug p = new OutwardDrug();

                if (item.Remaining <= 0)
                {
                    continue;
                }

                item.DrugIDChanged = value.DrugIDChanged;
                p.GetDrugForSellVisitor = item;
                p.DrugID = item.DrugID;
                p.InBatchNumber = item.InBatchNumber;
                p.InID = item.InID;
                p.OutPrice = item.OutPrice;
                p.InExpiryDate = item.InExpiryDate;
                p.SdlDescription = item.SdlDescription;
                p.HIAllowedPrice = item.HIAllowedPrice;

                //KMx: Nếu thuốc muốn thêm vào giống với thuốc đã có trong phiếu phải gán lại những thuộc tính bên dưới.
                //Nếu không gán lại thì hàm CheckInsurance_New(), btnNgayDung_New() và ListDisplayAutoComplete() sẽ sai ở dòng 
                //group hd by new
                //    { 
                //        hd.DrugID, hd.DayRpts, hd.V_DrugType, hd.QtyForDay, hd.QtyMaxAllowed, hd.QtySchedMon, hd.QtySchedTue,
                //        hd.QtySchedWed, hd.QtySchedThu, hd.QtySchedFri, hd.QtySchedSat, hd.QtySchedSun, hd.SchedBeginDOW, hd.DispenseVolume,
                //        hd.GetDrugForSellVisitor.InsuranceCover, hd.GetDrugForSellVisitor.BrandName
                //    } into hdgroup

                p.V_DrugType = item.V_DrugType;
                p.QtyForDay = (decimal)item.QtyForDay;
                p.DayRpts = item.DayRpts;
                p.QtyMaxAllowed = item.QtyMaxAllowed;
                p.QtySchedMon = item.QtySchedMon;
                p.QtySchedTue = item.QtySchedTue;
                p.QtySchedWed = item.QtySchedWed;
                p.QtySchedThu = item.QtySchedThu;
                p.QtySchedFri = item.QtySchedFri;
                p.QtySchedSat = item.QtySchedSat;
                p.QtySchedSun = item.QtySchedSun;
                p.SchedBeginDOW = item.SchedBeginDOW;
                p.DispenseVolume = item.DispenseVolume;

                p.HIPaymentPercent = item.HIPaymentPercent;

                if (SelectedPrescription != null && SelectedPrescription.PrescriptID > 0)
                {
                    p.PrescriptionDetailObj = new PrescriptionDetail { PrescriptID = SelectedPrescription.PrescriptID };
                }

                if (item.Remaining - value.RequiredNumber < 0)
                {
                    if (value.Qty > item.Remaining)
                    {
                        p.QtyOffer = item.Remaining;
                        value.Qty = value.Qty - item.Remaining;
                    }
                    else
                    {
                        p.QtyOffer = value.Qty;
                        value.Qty = 0;
                    }
                    value.RequiredNumber = value.RequiredNumber - item.Remaining;

                    p.OutQuantity = item.Remaining;

                    CheckBatchNumberExists(p);
                    item.Remaining = 0;
                }
                else
                {
                    p.QtyOffer = value.Qty;
                    value.Qty = 0;

                    p.OutQuantity = (int)value.RequiredNumber;

                    CheckBatchNumberExists(p);
                    item.Remaining = item.Remaining - (int)value.RequiredNumber;
                    break;
                }
            }
            SumTotalPrice();
        }

        private void AddListOutwardDrug(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (CheckValidDrugAuto(SelectedDrugForSell))
            {
                ChooseBatchNumber(SelectedDrugForSell);
            }
            else
            {
                string ErrorMessage = eHCMSResources.Z0888_G1_ThuocThaoTacBiLoi;
                if (SelectedDrugForSell.BrandName != null)
                {
                    ErrorMessage = string.Format(eHCMSResources.Z1715_G1_Thuoc0DaBiLoi, SelectedDrugForSell.BrandName);
                }
                MessageBox.Show(ErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
            }
        }

        private void ReCountQtyRequest(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (SelectedOutInvoice.OutwardDrugs == null)
            {
                SelectedOutInvoice.OutwardDrugs = new ObservableCollection<OutwardDrug>();
            }
            var results1 = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == SelectedDrugForSell.DrugID);
            if (results1 != null && results1.Count() > 0)
            {
                foreach (OutwardDrug p in results1)
                {
                    if (p.QtyOffer > p.OutQuantity)
                    {
                        p.QtyOffer = p.OutQuantity;
                    }
                    SelectedDrugForSell.Qty = SelectedDrugForSell.Qty - p.QtyOffer;
                }
            }
        }

        public bool CheckValidQty(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (SelectedOutInvoice != null && SelectedDrugForSell != null)
            {
                int intOutput = 0;
                if (SelectedDrugForSell.RequiredNumber <= 0
                    || !Int32.TryParse(SelectedDrugForSell.RequiredNumber.ToString(), out intOutput) //Nếu số lượng không phải là số nguyên.
                    || SelectedDrugForSell.RequiredNumber > SelectedDrugForSell.Remaining) //Nếu số lượng muốn thêm > số lượng còn trong kho.
                {
                    MessageBox.Show(eHCMSResources.Z0890_G1_SLgKgHopLe, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            else
            {
                MessageBox.Show(eHCMSResources.K0400_G1_ChonThuocCanThem, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                return false;
            }
        }

        //Kết hợp 2 hàm ReCountQtyRequest() và AddListOutwardDrug() (09/06/2014 16:40)
        public void ReCountQtyAndAddList(GetDrugForSellVisitor SelectedDrugForSell)
        {
            if (!CheckValidQty(SelectedDrugForSell))
                return;
            else
            {
                ReCountQtyRequest(SelectedDrugForSell);
                AddListOutwardDrug(SelectedDrugForSell);
            }
        }

        //Kiên: Những module có hàm tương tự (Khi module này sai thì phải sửa những module khác).
        //1. Bán thuốc theo toa.
        //2. Xuất nội bộ.
        public void AddItem()
        {
            //if (DrugInPrescriptionItem != null && DrugInPrescriptionItem.DrugID > 0 && SelectedSellVisitor != null)
            //{
            //    SelectedSellVisitor.DrugIDChanged = DrugInPrescriptionItem.DrugID;
            //    SelectedSellVisitor.Qty = DrugInPrescriptionItem.Qty.DeepCopy();
            //    if (lstDrugInPrescriptionDeleted == null)
            //    {
            //        lstDrugInPrescriptionDeleted = new ObservableCollection<GetDrugForSellVisitor>();
            //    }
            //    lstDrugInPrescriptionDeleted.Add(DrugInPrescriptionItem.DeepCopy());
            //    lstDrugInPrescription.Remove(DrugInPrescriptionItem);
            //}
            //ReCountQtyRequest(SelectedSellVisitor);
            //AddListOutwardDrug(SelectedSellVisitor, true);
            // DrugInPrescriptionItem = null;

            //Kiên: Combine 2 functions: ReCountQtyRequest and AddListOutwardDrug
            ReCountQtyAndAddList(SelectedSellVisitor);
        }

        #region Properties member
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListTemp;
        private ObservableCollection<GetDrugForSellVisitor> BatchNumberListShow;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugID;
        private ObservableCollection<OutwardDrug> OutwardDrugListByDrugIDFirst;

        private OutwardDrug _SelectedOutwardDrug;
        public OutwardDrug SelectedOutwardDrug
        {
            get { return _SelectedOutwardDrug; }
            set
            {
                if (_SelectedOutwardDrug != value)
                    _SelectedOutwardDrug = value;
                NotifyOfPropertyChange(() => SelectedOutwardDrug);
            }
        }
        #endregion

        private void GetDrugForSellVisitorBatchNumber(long DrugID, bool? InsuranceCover = null)
        {
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetDrugForSellVisitorBatchNumber_V2(DrugID, StoreID, IsHIPatient, InsuranceCover, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetDrugForSellVisitorBatchNumber_V2(asyncResult);
                            BatchNumberListTemp = results.ToObservableCollection();
                            if (BatchNumberListTemp != null && BatchNumberListTemp.Count > 0)
                            {
                                UpdateListToShow();
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void UpdateListToShow()
        {
            if (OutwardDrugListByDrugIDFirst != null)
            {
                foreach (OutwardDrug d in OutwardDrugListByDrugIDFirst)
                {
                    var value = BatchNumberListTemp.Where(x => x.DrugID == d.DrugID && x.InBatchNumber == d.InBatchNumber && x.InID == d.InID);
                    if (value.Count() > 0)
                    {
                        foreach (GetDrugForSellVisitor s in value.ToList())
                        {
                            s.Remaining = s.Remaining + d.OutQuantityOld;
                            s.RemainingFirst = s.RemainingFirst + d.OutQuantityOld;
                        }
                    }
                    else
                    {
                        GetDrugForSellVisitor p = d.GetDrugForSellVisitor;
                        p.Remaining = d.OutQuantity;
                        p.RemainingFirst = d.OutQuantity;
                        p.InBatchNumber = d.InBatchNumber;
                        p.SellingPrice = d.OutPrice;
                        p.InID = Convert.ToInt64(d.InID);
                        p.STT = d.STT;
                        BatchNumberListTemp.Add(p);
                    }
                }
            }
            foreach (GetDrugForSellVisitor s in BatchNumberListTemp)
            {
                if (OutwardDrugListByDrugID.Count > 0)
                {
                    foreach (OutwardDrug d in OutwardDrugListByDrugID)
                    {
                        //20200422 TBL: Trừ luôn số lượng dòng nhập đang được chọn để đổi lô
                        //if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID && d.InID != SelectedOutwardDrug.InID)
                        if (d.DrugID == s.DrugID && d.InBatchNumber == s.InBatchNumber && d.InID == s.InID)
                        {
                            s.Remaining = s.Remaining - d.OutQuantity;
                        }
                    }
                }
            }

            BatchNumberListShow = BatchNumberListTemp.Where(x => x.Remaining > 0).ToObservableCollection();

            if (BatchNumberListShow != null && BatchNumberListShow.Count > 0)
            {
                Action<IChooseBatchNumberVisitor> onInitDlg = delegate (IChooseBatchNumberVisitor proAlloc)
                {
                    proAlloc.SelectedOutwardDrug = SelectedOutwardDrug.DeepCopy();
                    if (BatchNumberListShow != null)
                    {
                        proAlloc.BatchNumberListShow = BatchNumberListShow.DeepCopy();
                    }
                    if (OutwardDrugListByDrugID != null)
                    {
                        proAlloc.OutwardDrugListByDrugID = OutwardDrugListByDrugID.DeepCopy();
                    }
                };
                GlobalsNAV.ShowDialog<IChooseBatchNumberVisitor>(onInitDlg);
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0891_G1_KgConLoNaoKhac, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        #endregion

        #region Chon Lo Member

        public void lnkChooseBatchNumber_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOutInvoice != null)
            {
                Button lnkBatchNumber = sender as Button;
                long DrugID = (long)lnkBatchNumber.CommandParameter;
                OutwardDrugListByDrugID = SelectedOutInvoice.OutwardDrugs.Where(x => x.DrugID == DrugID).ToObservableCollection();
                if (OutwardDrugsCopy != null)
                {
                    OutwardDrugListByDrugIDFirst = OutwardDrugsCopy.Where(x => x.DrugID == DrugID).ToObservableCollection();
                }
                GetDrugForSellVisitorBatchNumber(DrugID, (lnkBatchNumber.DataContext as OutwardDrug).HI);
            }
        }

        #region IHandle<ChooseBatchNumberVisitorEvent> Members

        public void Handle(ChooseBatchNumberVisitorEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetDrugForSellVisitor.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetDrugForSellVisitor.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetDrugForSellVisitor.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetDrugForSellVisitor.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetDrugForSellVisitor.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetDrugForSellVisitor.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;

                SelectedOutwardDrug.GetDrugForSellVisitor.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SumTotalPrice();
            }
        }

        #endregion

        #region IHandle<ChooseBatchNumberVisitorResetQtyEvent> Members

        public void Handle(ChooseBatchNumberVisitorResetQtyEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedOutwardDrug.GetDrugForSellVisitor.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.GetDrugForSellVisitor.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.GetDrugForSellVisitor.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.HIAllowedPrice = message.BatchNumberVisitorSelected.HIAllowedPrice;
                SelectedOutwardDrug.GetDrugForSellVisitor.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.GetDrugForSellVisitor.Remaining = message.BatchNumberVisitorSelected.Remaining;
                SelectedOutwardDrug.GetDrugForSellVisitor.RemainingFirst = message.BatchNumberVisitorSelected.RemainingFirst;
                SelectedOutwardDrug.GetDrugForSellVisitor.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;

                SelectedOutwardDrug.GetDrugForSellVisitor.SellingPrice = message.BatchNumberVisitorSelected.SellingPrice;

                SelectedOutwardDrug.InBatchNumber = message.BatchNumberVisitorSelected.InBatchNumber;
                SelectedOutwardDrug.InExpiryDate = message.BatchNumberVisitorSelected.InExpiryDate;
                SelectedOutwardDrug.InID = message.BatchNumberVisitorSelected.InID;
                SelectedOutwardDrug.OutPrice = message.BatchNumberVisitorSelected.SellingPrice;
                SelectedOutwardDrug.SdlDescription = message.BatchNumberVisitorSelected.SdlDescription;
                SelectedOutwardDrug.OutQuantity = message.BatchNumberVisitorSelected.Remaining;
                SumTotalPrice();
            }
        }

        #endregion

        #endregion

        #region RadioButton Event Member
        public void RadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            HI = 0;
        }
        public void RadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            HI = 1;
        }
        public void RadioButton3_Checked(object sender, RoutedEventArgs e)
        {
            HI = 2;
        }

        #endregion

        #region Xoa OutwardDrug Details Va refesh Du Lieu Member
        private void DeleteOutwardDrugs(OutwardDrug temp)
        {
            OutwardDrug p = temp.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(temp);
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
        }

        private void DeleteInvoiceDrugInObject()
        {
            OutwardDrug p = SelectedOutwardDrug.DeepCopy();
            SelectedOutInvoice.OutwardDrugs.Remove(SelectedOutwardDrug);
            //if (lstDrugInPrescriptionDeleted != null)
            //{
            //    foreach (var ite in lstDrugInPrescriptionDeleted)
            //    {

            //        if (p.GetDrugForSellVisitor != null && p.GetDrugForSellVisitor.DrugIDChanged == ite.DrugID)
            //        {
            //            if (lstDrugInPrescription.Where(x => x.DrugID == ite.DrugID).Count() > 0)
            //            {
            //                break;
            //            }
            //            else
            //            {
            //                lstDrugInPrescription.Add(ite);
            //            }
            //        }
            //    }
            //}
            foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
            {
                if (item.DrugID == p.DrugID)
                {
                    item.QtyOffer = item.QtyOffer + p.QtyOffer;
                    break;
                }
            }
            SelectedOutInvoice.OutwardDrugs = SelectedOutInvoice.OutwardDrugs.ToObservableCollection();
            SumTotalPrice();

            if (IsConfirmHIView)
            {
                CV_Prescriptions_Source = new CollectionViewSource { Source = SelectedOutInvoice.OutwardDrugs };
                CV_Prescriptions = (CollectionView)CV_Prescriptions_Source.View;
                NotifyOfPropertyChange(() => CV_Prescriptions);
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (IsServiceTabsVisible) return;
            if (SelectedOutwardDrug != null && SelectedOutInvoice.CanSaveAndPaidPrescript)
            {
                //if (MessageBox.Show("Bạn có muốn xóa thuốc này không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                DeleteInvoiceDrugInObject();
                // }
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z0904_G1_PhDaThuTienHoacKC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        public void btnNew()
        {
            // if (MessageBox.Show(eHCMSResources.A0141_G1_Msg_ConfTaoMoiPhBanThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            // {
            RefeshData();
            //  }
        }

        #endregion

        #region IHandle<AddCompleted<List<OutwardDrugInvoice>>> Members

        public void Handle(AddCompleted<List<OutwardDrugInvoice>> message)
        {
            if (message != null && this.IsActive)
            {
                if (message.Item != null)
                {
                    OutwardDrugInvoice item = message.Item.FirstOrDefault();
                    if (item != null)
                    {
                        OutwardDrugsCopy = item.OutwardDrugs;
                        GetOutWardDrugInvoiceVisitorByID(item.outiID);
                    }
                }
            }
        }

        #endregion

        #region IHandle<PayForRegistrationCompleted> Members

        public void Handle(PayForRegistrationCompleted message)
        {
            if (message != null && IsActive)
            {
                //Load lai thong tin luc save roi - Khong can load lai o day
                OutwardDrugsCopy = SelectedOutInvoice == null ? null : SelectedOutInvoice.OutwardDrugs;
                //▼====: #006
                if (SelectedOutInvoice != null)
                {
                    GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID, true);
                }
                if (!IsConfirmHIView)
                {
                    btnPreview();
                }
                //▲====: #006
                if (IsConfirmHIView && SelectedPrescription != null && SelectedPrescription.PtRegistrationID.GetValueOrDefault(0) > 0 && SelectedPrescription.IsEmptyPrescription)
                {
                    if (PatientInfo == null)
                    {
                        PatientInfo = new Patient();
                    }
                    Coroutine.BeginExecute(DoGetInfoPatientPrescription());
                }
            }
        }

        #endregion

        //▼====: #007
        #region IHandle<PayForRegistrationCompletedAtConfirmHIView> Members
        public void Handle(PayForRegistrationCompletedAtConfirmHIView message)
        {
            if (message != null && IsActive)
            {
                OutwardDrugsCopy = SelectedOutInvoice.OutwardDrugs;
                GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID, true);
                if (!IsConfirmHIView)
                {
                    btnPreview();
                }
                else
                {
                    var payment = message.Payment;
                    if (payment != null && payment.PatientTransaction != null && payment.PatientTransaction.PtRegistrationID.HasValue)
                    {
                        //OpenRegistration(message.Registration.PtRegistrationID);
                        ProcessPayCompletedEvent(message);
                    }
                }
                if (IsConfirmHIView && SelectedPrescription != null && SelectedPrescription.PtRegistrationID.GetValueOrDefault(0) > 0 && SelectedPrescription.IsEmptyPrescription)
                {
                    if (PatientInfo == null)
                    {
                        PatientInfo = new Patient();
                    }
                    Coroutine.BeginExecute(DoGetInfoPatientPrescription());
                }
            }
        }
        #endregion
        //▲====: #007

        #region "Dược Sĩ Sửa Lại Toa Thuốc"

        private bool _CanEditPrescription = false;
        public bool CanEditPrescription
        {
            get { return _CanEditPrescription; }
            set
            {
                if (_CanEditPrescription != value)
                {
                    _CanEditPrescription = value;
                    NotifyOfPropertyChange(() => CanEditPrescription);
                }
            }
        }

        public void btEditPrescriptions()
        {
            //Đọc lại thông tin Toa Thuốc Này rồi Popup Sửa
            Registration_DataStorage.CurrentPatientRegistration.Patient = PatientInfo;
            Action<IePrescriptionOld> onInitDlg = delegate (IePrescriptionOld typeInfo)
            {
                typeInfo.DuocSi_IsEditingToaThuoc = true;
                typeInfo.btDuocSiEditVisibility = true;
                typeInfo.btChonChanDoanVisibility = false;
                typeInfo.Registration_DataStorage = Registration_DataStorage;
                Globals.EventAggregator.Publish(new DuocSi_EditingToaThuocEvent { SelectedPrescription = PrecriptionsBeforeEdit });
            };
            GlobalsNAV.ShowDialog<IePrescriptionOld>(onInitDlg);
        }

        private Prescription _PrecriptionsBeforeEdit;
        public Prescription PrecriptionsBeforeEdit
        {
            get
            {
                return _PrecriptionsBeforeEdit;
            }
            set
            {
                if (_PrecriptionsBeforeEdit != value)
                {
                    _PrecriptionsBeforeEdit = value;
                    NotifyOfPropertyChange(() => PrecriptionsBeforeEdit);
                }
            }
        }

        private void Prescription_ByPrescriptIDIssueID(Int64 PrescriptID, Int64 IssueID)
        {
            isLoadingPrescriptID = false;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = "Thông Tin Toa Thuốc..." });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ePrescriptionsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPrescription_ByPrescriptIDIssueID(PrescriptID, IssueID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            PrecriptionsBeforeEdit = contract.EndPrescription_ByPrescriptIDIssueID(asyncResult);
                            if (SelectedPrescription != null)
                            {
                                SelectedPrescription.IssuedDateTime = PrecriptionsBeforeEdit.IssuedDateTime;
                            }
                            CanEditPrescription = PrecriptionsBeforeEdit.CanEdit;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            isLoadingPrescriptID = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        public void Handle(ReLoadToaOfDuocSiEditEvent<Prescription> message)
        {
            if (message != null)
            {
                SelectedPrescription = message.Prescription as Prescription;
                PrecriptionsBeforeEdit = message.Prescription as Prescription;
                LoadInfoCommon();
                // spGetInBatchNumberAndPrice_ByPresciption(SelectedPrescription.IssueID, StoreID, 0);
            }
        }
        #endregion

        #region Huy Phieu Member
        private void CancelCurrentInvoice()
        {
            if (SelectedOutInvoice == null || SelectedOutInvoice.OutwardDrugs == null || !SelectedOutInvoice.OutwardDrugs.Any(x => x.DrugID > 0))
            {
                return;
            }
            if (PatientInfo != null && PatientInfo.LatestRegistration != null
                && Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "hủy phiếu xuất") && ValueQuyenLoiBH.Value > 0)
            {
                return;
            }
            //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)
            // Txd 25/05/2014 Replaced ConfigList
            if (Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1)
            {
                CancalOutwardInvoiceVisitor();
            }
            else
            {
                CancalOutwardInvoice();
            }
        }
        public void btnCancel(object sender, RoutedEventArgs e)
        {
            //▼===== #010
            if (PatientInfo != null && PatientInfo.LatestRegistration != null && PatientInfo.LatestRegistration.ConfirmHIStaffID > 0)
            {
                MessageBox.Show(eHCMSResources.Z3040_G1_DangKyDaXNKhongHuyPX);
                return;
            }
            //▲===== #010
            if (MessageBox.Show(eHCMSResources.Z0906_G1_CoMuonHuyPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CancelCurrentInvoice();
            }
        }

        private void CancalOutwardInvoice()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new CommonServiceClient())
                using (var serviceFactory = new PharmacySaleAndOutwardClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //contract.BeginCancelOutwardDrugInvoice(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID, null, SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    contract.BeginCancelOutwardDrugInvoice_Pst(Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID, null, SelectedOutInvoice, (IsConfirmHIView ? (long?)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN : null), Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            //var results = contract.EndCancelOutwardDrugInvoice(asyncResult);
                            var results = contract.EndCancelOutwardDrugInvoice_Pst(asyncResult);
                            //Globals.ShowMessage("Đã Hủy Phiếu", eHCMSResources.G0442_G1_TBao);
                            var inv = (OutwardDrugInvoice)asyncResult.AsyncState;
                            if (inv.PtRegistrationID.HasValue && inv.PtRegistrationID.Value > 0)
                            {
                                Coroutine.BeginExecute(DoRefund(inv.PtRegistrationID.Value, inv.outiID, null));
                                if (IsConfirmHIView && SelectedPrescription != null && SelectedPrescription.PrescriptID > 0 && !SelectedPrescription.IsEmptyPrescription)
                                {
                                    //DoNothing
                                }
                                else
                                {
                                    GetOutWardDrugInvoiceVisitorByID(inv.outiID);
                                }
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z1793_G1_KgCoDK);
                            }
                            //RefeshData();
                            if (IsConfirmHIView && SelectedPrescription != null && SelectedPrescription.PrescriptID > 0 && !SelectedPrescription.IsEmptyPrescription)
                            {
                                SelectedOutInvoice = null;
                                LoadPrescriptionDetail(SelectedPrescription);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }
                    }), SelectedOutInvoice);
                }
            });

            t.Start();
        }

        #endregion

        #region Tra Hang Member

        public void btnReturn()
        {
            Action<IReturnDrug> onInitDlg = delegate (IReturnDrug proAlloc)
            {
                proAlloc.IsChildWindow = true;
                if (proAlloc.SearchCriteria == null)
                {
                    proAlloc.SearchCriteria = new DataEntities.SearchOutwardInfo();
                }
                proAlloc.SearchCriteria.ID = SelectedOutInvoice.outiID;
                proAlloc.btnSearch();
            };
            GlobalsNAV.ShowDialog<IReturnDrug>(onInitDlg);
        }

        #region IHandle<PharmacyCloseFormReturnEvent> Members

        public void Handle(PharmacyCloseFormReturnEvent message)
        {
            if (IsActive && message != null)
            {
                LoadPrescriptionDetailByInvoice(SelectedOutInvoice.outiID);
            }
        }

        #endregion


        #endregion

        #region Cap Nhat Phieu Da Thu Tien Member
        public void btnEditPayed()
        {
            if (PatientInfo != null && PatientInfo.LatestRegistration != null
                && Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "cập nhật phiếu") && ValueQuyenLoiBH.Value > 0)
            {
                return;
            }
            //KMx: Kiểm tra thời gian tối đa cho phép cập nhật (23/07/2014 14:55).
            int AllowTimeUpdateOutInvoice = Globals.ServerConfigSection.PharmacyElements.AllowTimeUpdateOutInvoice;
            if (AllowTimeUpdateOutInvoice > 0)
            {
                TimeSpan t = Globals.ServerDate.Value - SelectedOutInvoice.OutDate;
                if (t.TotalHours >= AllowTimeUpdateOutInvoice)
                {
                    MessageBox.Show(string.Format(eHCMSResources.Z1291_G1_TGianToiDaChoPhepCNhat, AllowTimeUpdateOutInvoice), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
            }

            //Phần kiểm tra này có 3 chổ sử dụng tương tự (Bán thuốc lẻ, bán thuốc theo toa, xuất nội bộ). Nếu sửa ở đây thì phải sửa cho 2 chổ kia luôn. (24/02/2014 16:25)
            //KMx: Nếu phiếu xuất đã báo cáo rồi.
            if (SelectedOutInvoice.AlreadyReported)
            {
                //Nếu user không có quyền cập nhật phiếu xuất đã báo cáo rồi thì chặn lại.
                if (!mBanThuocTheoToa_CapNhatSauBaoCao)
                {
                    MessageBox.Show(eHCMSResources.Z1653_G1_PhXuatDaBCLHeQLy, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                //Nếu user có quyền cập nhật phiếu xuất đã báo cáo rồi thì confirm lại.
                else
                {
                    if (MessageBox.Show(eHCMSResources.Z1654_G1_PhXuatDaBCCoMuonCNhat, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                    {
                        return;
                    }
                }
            }

            //▼===== 20191211 TTM:  Sửa lại GlobalsNav gọi màn hình cập nhật phiếu xuất.
            //                      Không còn DeepCopy tùm lum (Biến decimal ...) bổ sung thêm hàm SetDefaultForStore để load kho cho màn hình cập nhật theo kho của phiếu xuất muốn cập nhật.
            Action<IEditPrescription> onInitDlg = delegate (IEditPrescription proAlloc)
            {
                OutwardDrugInvoice tmpOutwardDrugInvoiceDeepCopy1st = SelectedOutInvoice.DeepCopy();
                OutwardDrugInvoice tmpOutwardDrugInvoiceDeepCopy2nd = SelectedOutInvoice.DeepCopy();
                proAlloc.SelectedOutInvoice = tmpOutwardDrugInvoiceDeepCopy1st;
                proAlloc.SelectedOutInvoiceCopy = tmpOutwardDrugInvoiceDeepCopy2nd;
                proAlloc.PatientInfo = PatientInfo;
                proAlloc.GetDrugForSellVisitorListByPrescriptID = GetDrugForSellVisitorListByPrescriptID;
                proAlloc.SelectedPrescription = SelectedPrescription;
                proAlloc.OutwardDrugsCopy = proAlloc.SelectedOutInvoiceCopy.OutwardDrugs;
                proAlloc.GetClassicationPatientInvoice();
                proAlloc.TotalInvoicePrice = TotalInvoicePrice;
                proAlloc.TotalHIPayment = TotalHIPayment;
                proAlloc.TotalPatientPayment = TotalPatientPayment;
                proAlloc.SetDefaultForStore();
            };
            GlobalsNAV.ShowDialog<IEditPrescription>(onInitDlg);

        }

        #endregion

        #region IHandle<PharmacyCloseEditPayedPrescription> Members

        public void Handle(PharmacyCloseEditPayedPrescription message)
        {
            if (message != null && this.IsActive)
            {
                if (message.SelectedOutwardInvoice != null)
                {
                    OutwardDrugsCopy = message.SelectedOutwardInvoice.OutwardDrugs;
                    GetOutWardDrugInvoiceVisitorByID(message.SelectedOutwardInvoice.outiID);
                }
            }
        }

        #endregion

        public void CheckBoxBH_Checked(object sender, RoutedEventArgs e)
        {
            FunctionNotCountBHYT();
        }
        TextBox SearchPrePrescriptIDTextBox;
        public void SearchPrePrescriptIDTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            SearchPrePrescriptIDTextBox = (TextBox)sender;
            SearchPrePrescriptIDTextBox.Focus();
        }
        private void FunctionNotCountBHYT()
        {
            IsNotCountInsurance = true;
            //neu la benh nhan BH,khong muon tinh BH thi cap nhat co la da ban va thay doi gia
            if (SelectedOutInvoice != null && IsHIPatient && SelectedOutInvoice.OutwardDrugs != null)
            {
                foreach (OutwardDrug p in SelectedOutInvoice.OutwardDrugs)
                {
                    //vi khi luu xuong a Tuyen lay gia trong p.InwardDrug nen phai gan gia tri lai nhu vay

                    //if (p.GetDrugForSellVisitor != null && (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowedPharmacyChangeHIPrescript]) == 1 || p.QtyOffer > 0))
                    // Txd 25/05/2014 Replaced ConfigList
                    if (p.GetDrugForSellVisitor != null && (Globals.ServerConfigSection.PharmacyElements.AllowedPharmacyChangeHIPrescript == 1 || p.QtyOffer > 0))
                    {
                        p.OutPrice = p.GetDrugForSellVisitor.NormalPrice;
                        p.ChargeableItem.HIAllowedPrice = 0;
                        p.InwardDrug.NormalPrice = p.OutPrice;
                        p.InwardDrug.HIPatientPrice = p.OutPrice;
                        p.HIAllowedPrice = 0;
                        p.PriceDifference = 0;
                        p.TotalPriceDifference = 0;
                        SumTotalPrice();
                    }
                }
            }
        }

        public void CheckBoxBH_Unchecked(object sender, RoutedEventArgs e)
        {
            IsNotCountInsurance = false;
            //neu la benh nhan BH,khong muon tinh BH thi cap nhat co la da ban va thay doi gia
            if (SelectedOutInvoice != null && IsHIPatient && SelectedOutInvoice.OutwardDrugs != null)
            {
                foreach (OutwardDrug p in SelectedOutInvoice.OutwardDrugs)
                {
                    //vi khi luu xuong a Tuyen lay gia trong p.InwardDrug nen phai gan gia tri lai nhu vay

                    //if (p.HI.GetValueOrDefault() && p.GetDrugForSellVisitor != null && (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.AllowedPharmacyChangeHIPrescript]) == 1 || p.QtyOffer > 0))
                    // Txd 25/05/2014 Replaced ConfigList
                    if (p.HI.GetValueOrDefault() && p.GetDrugForSellVisitor != null && (Globals.ServerConfigSection.PharmacyElements.AllowedPharmacyChangeHIPrescript == 1 || p.QtyOffer > 0))
                    {
                        p.OutPrice = p.GetDrugForSellVisitor.PriceForHIPatient;
                        p.InwardDrug.NormalPrice = p.OutPrice;
                        p.InwardDrug.HIPatientPrice = p.OutPrice;

                        p.ChargeableItem.HIAllowedPrice = p.GetDrugForSellVisitor.HIAllowedPriceNoChange;
                        p.HIAllowedPrice = p.ChargeableItem.HIAllowedPrice;
                        if (p.HIAllowedPrice.GetValueOrDefault(-1) > 0)
                        {
                            p.PriceDifference = p.OutPrice - p.ChargeableItem.HIAllowedPrice.GetValueOrDefault(0);
                            p.TotalPriceDifference = p.PriceDifference * p.OutQuantity;
                        }
                    }
                    SumTotalPrice();
                }
            }
        }

        #region COROUTINES
        public IEnumerator<IResult> DoRefund(long registrationID, long outiID, ObservableCollection<InPatientBillingInvoice> updatedbillings)
        {
            var loadRegInfoTask = new LoadRegistrationInfoTask(registrationID);
            yield return loadRegInfoTask;
            if (loadRegInfoTask.Error == null && loadRegInfoTask.Registration != null)
            {
                if (updatedbillings != null)
                {
                    loadRegInfoTask.Registration.InPatientBillingInvoices = updatedbillings;
                }
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    string mErrorMessage;
                    if (Globals.CheckValidRegistrationForPay(loadRegInfoTask.Registration, out mErrorMessage)
                        && ((!IsConfirmHIView || (IsRefundMoney && outiID == 0)) || loadRegInfoTask.Registration.ConfirmHIStaffID.GetValueOrDefault(0) > 0 || (IsEnableCancelBillingButton && updatedbillings != null)))
                    {
                        Action<ISimplePay> onInitDlg = delegate (ISimplePay vm)
                        {
                            vm.Registration = loadRegInfoTask.Registration;
                            if (IsConfirmHIView)
                            {
                                vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.QUAY_XAC_NHAN;
                            }
                            else
                            {
                                vm.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                            }
                            vm.FormMode = PaymentFormMode.PAY;
                            //▼===== 20191118 TTM: Bổ sung biến để phân biệt đây là huỷ hoàn tiền.
                            vm.IsRefundFromPharmacy = true;
                            //▲===== 
                            OutwardDrugInvoice tempInv = null;
                            if (vm.Registration.DrugInvoices != null && updatedbillings == null)
                            {
                                foreach (var inv in vm.Registration.DrugInvoices)
                                {
                                    if ((inv.outiID == outiID || (IsConfirmHIView && IsRefundMoney && outiID == 0))
                                        && inv.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED
                                        && inv.PaidTime != null && inv.RefundTime == null)
                                    {
                                        tempInv = inv;
                                        break;
                                    }
                                }
                            }
                            if (tempInv != null)
                            {
                                vm.DrugInvoices = new List<OutwardDrugInvoice> { tempInv };
                            }
                            vm.StartCalculating();
                            if (updatedbillings != null)
                            {
                                vm.IsRefundBilling = true;
                            }
                        };
                        GlobalsNAV.ShowDialog<ISimplePay>(onInitDlg);
                    }
                    else
                    {
                        //▼====== #004
                        if (mErrorMessage != null)
                        {
                            MessageBox.Show(mErrorMessage, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        //▲====== #004
                    }
                });
            }
            else
            {
                MessageBox.Show(eHCMSResources.Z1708_G1_KgTheLayDKDeTraTien);
            }
            yield break;
        }
        #endregion

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetDrugForSellVisitor(txt, true);
                }
            }
        }

        public void DrugCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchGetDrugForSellVisitor((sender as TextBox).Text, true);
            }
        }

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                if (SelectedOutInvoice != null)
                {
                    return _VisibilityName && SelectedOutInvoice.CanSaveAndPaidPrescript;
                }
                return _VisibilityName;
            }
            set
            {
                if (SelectedOutInvoice != null)
                {
                    _VisibilityName = value && SelectedOutInvoice.CanSaveAndPaidPrescript;
                    _VisibilityCode = !_VisibilityName && SelectedOutInvoice.CanSaveAndPaidPrescript;
                }
                else
                {
                    _VisibilityName = value;
                    _VisibilityCode = !_VisibilityName;
                }
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }

        private bool _VisibilityCode = false;
        public bool VisibilityCode
        {
            get
            {
                if (SelectedOutInvoice != null)
                {
                    return !_VisibilityName && SelectedOutInvoice.CanSaveAndPaidPrescript;
                }
                return _VisibilityCode;
            }
        }

        public void Code_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = true;
            VisibilityName = false;
        }

        public void Name_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            IsCode = false;
            VisibilityName = true;
        }
        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }

        //Tìm phiếu xuất cũ.
        public void btnClick_MouseLeftButtonDown_Invoice(object sender, MouseButtonEventArgs e)
        {
            //mai coi lai
            if (e.ClickCount == 1)
            {
                SearchInvoiceOld();
            }
        }

        public void btnSearchInvoiceAdvance()
        {
            IPrescriptionSearchInvoice DialogView = Globals.GetViewModel<IPrescriptionSearchInvoice>();
            DialogView.SearchInvoiceCriteria = SearchInvoiceCriteria.DeepCopy();
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        private void HideShowColumnDelete()
        {
            if (grdPrescription != null)
            {
                if (SelectedOutInvoice.CanSaveAndPaidPrescript && mBanThuocTheoToa_Them)
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Visible;
                }
                else
                {
                    grdPrescription.Columns[(int)DataGridCol.ColDelete].Visibility = Visibility.Collapsed;
                }
            }
        }

        public void btnHoanTien()
        {
            if (PatientInfo != null && PatientInfo.LatestRegistration != null
                && Globals.IsLockRegistration(PatientInfo.LatestRegistration.RegLockFlag, "hoàn tiền") && ValueQuyenLoiBH.Value > 0)
            {
                return;
            }
            //if (Convert.ToInt32(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyCountMoneyIndependent]) == 1)
            // Txd 25/05/2014 Replaced ConfigList
            if (Globals.ServerConfigSection.PharmacyElements.PharmacyCountMoneyIndependent == 1)
            {
                LoadAndPayMoneyForService(SelectedOutInvoice, true);
                //ShowFormCountMoney();
            }
            else if (SelectedOutInvoice == null || SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0) == 0 || SelectedOutInvoice.outiID == 0 &&
                IsRefundMoney && PatientInfo.LatestRegistration != null &&
                PatientInfo.LatestRegistration.PtRegistrationID > 0)
            {
                Coroutine.BeginExecute(DoRefund(PatientInfo.LatestRegistration.PtRegistrationID, 0, null));
            }
            else
            {
                Coroutine.BeginExecute(DoRefund(SelectedOutInvoice.PtRegistrationID.Value, SelectedOutInvoice.outiID, null));
                GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
            }
        }

        //Kiên comment vì không sử dụng
        //private PatientRegistration _ObjPatientRegistration;
        //public PatientRegistration ObjPatientRegistration
        //{
        //    get { return _ObjPatientRegistration; }
        //    set
        //    {
        //        if (_ObjPatientRegistration != value)
        //        {
        //            _ObjPatientRegistration = value;
        //            NotifyOfPropertyChange(() => ObjPatientRegistration);
        //        }
        //    }
        //}
        //--------------------------------------------------------------------------

        private bool IsBenhNhanNoiTru()
        {
            if (SelectedPrescription != null)
            {
                if (SelectedPrescription.V_RegistrationType == AllLookupValues.RegistrationType.NOI_TRU)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private IEnumerator<IResult> GetRegistrationInfoTask(long PtRegistrationID)
        {
            //Kiên: comment vì không sử dụng.
            //var task = new LoadRegistrationInfoTask(PtRegistrationID, false);
            //yield return task;
            //ObjPatientRegistration = task.Registration;
            //----------------------------------------------------------------------

            //Đọc thông tin coi đc Sửa không
            Prescription_ByPrescriptIDIssueID(SelectedPrescription.PrescriptID, SelectedPrescription.IssueID);
            LoadInfoCommon();

            if (OldPaymentContent != null && PatientInfo != null && PatientInfo.LatestRegistration != null)
                OldPaymentContent.InitViewForPayments(PatientInfo.LatestRegistration);

            yield break;
        }

        private IEnumerator<IResult> GetRegistrationInfo_InPtTask(long PtRegistrationID)
        {
            //Kiên: comment vì không sử dụng.
            //var task = new LoadRegistrationInfo_InPtTask(PtRegistrationID, false);
            //yield return task;
            //ObjPatientRegistration = task.Registration;
            //-----------------------------------------------------------------------

            //Đọc thông tin coi đc Sửa không
            Prescription_ByPrescriptIDIssueID(SelectedPrescription.PrescriptID, SelectedPrescription.IssueID);
            LoadInfoCommon();

            yield break;
        }
        #region Thu Tien Thuoc Doc Lap Member

        private void ShowFormCountMoney()
        {
            Action<ISimplePayPharmacy> onInitDlg = delegate (ISimplePayPharmacy proAlloc)
            {
                proAlloc.V_TradingPlaces = (long)AllLookupValues.V_TradingPlaces.NHA_THUOC;
                if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    proAlloc.TotalPayForSelectedItem = 0;
                    proAlloc.TotalPaySuggested = -AmountPaided;//(SelectedOutInvoice.TotalInvoicePrice - SelectedOutInvoice.TotalHIPayment);
                }
                else
                {
                    proAlloc.TotalPayForSelectedItem = SelectedOutInvoice.TotalInvoicePrice - SelectedOutInvoice.TotalHIPayment;
                    proAlloc.TotalPaySuggested = SelectedOutInvoice.TotalInvoicePrice - SelectedOutInvoice.TotalHIPayment - AmountPaided;
                }
                proAlloc.StartCalculating();
            };
            GlobalsNAV.ShowDialog<ISimplePayPharmacy>(onInitDlg);
        }

        private void SaveDrugIndependents(OutwardDrugInvoice OutwardInvoice)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long OutID = 0;
                            string StrError;
                            bool value = contract.EndOutwardDrugInvoice_SaveByType(out OutID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError) && value)
                            {
                                //goi ham tinh tien
                                //CountMoneyForVisitorPharmacy(OutID, true);
                                ShowFormCountMoney();
                                OutwardDrugsCopy = OutwardInvoice.OutwardDrugs;
                                GetOutWardDrugInvoiceVisitorByID(OutID);
                            }
                            else
                            {
                                isLoadingFullOperator = false;
                                MessageBox.Show(StrError);
                            }
                        }
                        catch (Exception ex)
                        {
                            isLoadingFullOperator = false;
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        private void CancalOutwardInvoiceVisitor()
        {
            SelectedOutInvoice.V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.CANCELED;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutWardDrugInvoiceVisitor_Cancel(SelectedOutInvoice, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            long TransItemID = 0;
                            var results = contract.EndOutWardDrugInvoiceVisitor_Cancel(out TransItemID, asyncResult);
                            if (TransItemID > 0)
                            {
                                ShowFormCountMoney();
                            }
                            GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }

        #region IHandle<PharmacyPayEvent> Members

        private IEnumerator<IResult> AddTransactionVisitor(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionForDrugPayTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), Globals.DeptLocation.DeptLocationID);
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> AddTransactionHoanTien(PatientTransactionPayment payment, OutwardDrugInvoice InvoiceDrug)
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new AddTracsactionForDrugRefundTask(payment, InvoiceDrug, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return paymentTypeTask;
            isLoadingGetStore = false;
            yield break;
        }

        public void Handle(PharmacyPayEvent message)
        {
            //thu tien
            if (IsActive && message != null)
            {
                if (SelectedOutInvoice.V_OutDrugInvStatus == (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
                {
                    Coroutine.BeginExecute(AddTransactionHoanTien(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
                    {
                        GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
                    });
                }
                else
                {
                    Coroutine.BeginExecute(AddTransactionVisitor(message.CurPatientPayment, SelectedOutInvoice), null, (o, e) =>
                    {
                        btnPreview();
                        GetOutWardDrugInvoiceVisitorByID(SelectedOutInvoice.outiID);
                    });
                }
            }
        }

        #endregion

        //==== #001
        public void Handle(DeletedOutwardDrug message)
        {
            if (message != null)
            {
                SelectedOutwardDrug = message.DeleteOutwardDrug;
                if (SelectedOutwardDrug != null && SelectedOutInvoice.CanSaveAndPaidPrescript)
                {
                    DeleteInvoiceDrugInObject();
                }
                else
                {
                    MessageBox.Show(eHCMSResources.Z0904_G1_PhDaThuTienHoacKC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
        }

        public void Handle(ChangedQtyOutwardDrug message)
        {
            if (message != null)
            {
                SelectedOutwardDrug = message.DeleteOutwardDrug;
                SumTotalPrice();
            }
        }
        //==== #001

        #endregion

        public void btnUp()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex > 0)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrug Up = SelectedOutInvoice.OutwardDrugs[i - 1].DeepCopy();
                OutwardDrug Down = SelectedOutInvoice.OutwardDrugs[i].DeepCopy();
                SelectedOutInvoice.OutwardDrugs[i] = Up;
                SelectedOutInvoice.OutwardDrugs[i - 1] = Down;
                grdPrescription.SelectedIndex = i - 1;
            }
        }

        public void btnDown()
        {
            if (grdPrescription.SelectedItem != null && grdPrescription.SelectedIndex < SelectedOutInvoice.OutwardDrugs.Count() - 1)
            {
                int i = grdPrescription.SelectedIndex.DeepCopy();
                OutwardDrug Up = SelectedOutInvoice.OutwardDrugs[i].DeepCopy();
                OutwardDrug Down = SelectedOutInvoice.OutwardDrugs[i + 1].DeepCopy();
                SelectedOutInvoice.OutwardDrugs[i] = Down;
                SelectedOutInvoice.OutwardDrugs[i + 1] = Up;
                grdPrescription.SelectedIndex = i + 1;
            }
        }

        private bool _IsConfirmHIView = false;
        public bool IsConfirmHIView
        {
            get => _IsConfirmHIView; set
            {
                _IsConfirmHIView = value;
                NotifyOfPropertyChange(() => IsConfirmHIView);
                NotifyOfPropertyChange(() => IsServiceTabsVisible);
                NotifyOfPropertyChange(() => PayAndComfirmHIIsVisible);
                NotifyOfPropertyChange(() => mBanThuocTheoToa_CapNhatPhieu);
                NotifyOfPropertyChange(() => RenewScreenVisible);
                NotifyOfPropertyChange(() => mBanThuocTheoToa_TimBN);
                NotifyOfPropertyChange(() => IsAccountingView);
            }
        }

        public void btnConfirmHIBenefit()
        {
            if (IsConfirmHIView && PatientInfo != null && PatientInfo.LatestRegistration != null)
            {
                DoConfirmHIBenefit(PatientInfo.LatestRegistration);
            }
        }
        public void btnCancelConfirmHIBenefit()
        {
            if (IsConfirmHIView && PatientInfo != null && PatientInfo.LatestRegistration != null)
            {
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginCancelConfirmHIBenefit(PatientInfo.LatestRegistration.PtRegistrationID, Globals.LoggedUserAccount.Staff.StaffID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                string msg = string.Empty;
                                if (!contract.EndCancelConfirmHIBenefit(out msg, asyncResult))
                                {
                                    MessageBox.Show(eHCMSResources.T0074_G1_I, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                else
                                {
                                    bool IsCancelInvoice = true;
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2414_G1_HuyXNThanhCong);
                                    if (PatientInfo != null && PatientInfo.LatestRegistration != null)
                                    {
                                        PatientInfo.LatestRegistration.ConfirmHIStaffID = null;
                                        NotifyOfPropertyChange(() => IsEnableCancelBillingButton);
                                        if (PatientInfo.LatestRegistration.OutPtTreatmentProgramID > 0)
                                        {
                                            IsCancelInvoice = false;
                                        }
                                    }
                                    //▼===== 20200612 TTM: Nếu là bệnh nhân điều trị ngoại trú thì khi huỷ xác nhận sẽ không huỷ phiếu xuất nữa.
                                    if (IsCancelInvoice)
                                    {
                                        CancelCurrentInvoice();
                                    }
                                    //▲=====
                                }
                                if (!String.IsNullOrEmpty(msg))
                                {
                                    Globals.ShowMessage(msg, "[CẢNH BÁO]");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });
            t.Start();
        }
        private IEnumerator<IResult> DoConfirmHIBenefit(OutwardDrugInvoice Invoice)
        {
            if (PatientInfo == null || PatientInfo.LatestRegistration == null) yield break;

            //if (this.IsConfirmHIView && PatientInfo.LatestRegistration != null && PatientInfo.LatestRegistration.ConfirmHIStaffID.GetValueOrDefault(0) == 0)
            //{
            //    yield return GenericCoRoutineTask.StartTask(ConfirmHIBenefitTask, PatientInfo.LatestRegistration);
            //}
            //LoadInfoCommonInvoice();

            var paymentTypeTask = new CalcMoneyPaidedForDrugInvoiceTask(Invoice.outiID);
            yield return paymentTypeTask;
            AmountPaided = paymentTypeTask.Amount;
            if (IsBenhNhanNoiTru())
            {
                GetRegistrationInfo_InPt(Invoice.PtRegistrationID.Value);
            }
            else
            {
                GetRegistrationInfo(Invoice.PtRegistrationID.Value, this.IsConfirmHIView, true, true);
            }
            yield break;
        }

        private IEnumerator<IResult> DoSaveDrugs(OutwardDrugInvoice Invoice, bool value)
        {
            if (SelectedPrescription != null && SelectedPrescription.IsEmptyPrescription)
            {
                if (!IsBenhNhanNoiTru())
                {
                    GetRegistrationInfo(Invoice.PtRegistrationID.Value, true);
                }
                yield break;
            }

            var res = new SaveDrugsTask(Invoice, Globals.LoggedUserAccount.StaffID.GetValueOrDefault());
            yield return res;

            SelectedOutInvoice = res.SavedOutwardInvoice;

            if (this.IsConfirmHIView && SelectedOutInvoice != null)
            {
                if (!SavedOutwardDrugInvoice.ContainsKey(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0)))
                {
                    SavedOutwardDrugInvoice.Clear();
                    SavedOutwardDrugInvoice.Add(SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0), new List<OutwardDrug>());
                }
                if (res.SavedOutwardDrugs != null)
                {
                    foreach (OutwardDrug item in res.SavedOutwardDrugs)
                    {
                        SavedOutwardDrugInvoice[SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0)].Add(item);
                    }
                }
                else
                {
                    foreach (OutwardDrug item in SelectedOutInvoice.OutwardDrugs)
                    {
                        SavedOutwardDrugInvoice[SelectedOutInvoice.PtRegistrationID.GetValueOrDefault(0)].Add(item);
                    }
                }
            }

            ClearData();
            OutwardDrugsCopy = res.SavedOutwardInvoice.OutwardDrugs;

            //LoadInfoCommonInvoice();
            NotifyOfPropertyChange(() => VisibilityName);
            NotifyOfPropertyChange(() => VisibilityCode);
            HideShowColumnDelete();

            if (SelectedOutInvoice.SelectedPrescription != null)
            {
                //SearchGetDrugForSellVisitorByPrescriptID();
                //lay tat ca nhung thuoc ton (tung lo) theo ma toa thuoc(co bao nhieu lo lay het ra luon) 
                var OutWardDrugInvoiceVisitor = new SearchGetDrugForSellVisitorByPrescriptIDTask(SelectedOutInvoice.PrescriptID, HI, IsHIPatient, StoreID);
                yield return OutWardDrugInvoiceVisitor;

                GetDrugForSellVisitorListByPrescriptID = OutWardDrugInvoiceVisitor.DrugForSellVisitorList;

                GetClassicationPatientInvoice();

                SumTotalPrice();
                //KMx: Sau khi lưu, không cần lấy ngày nhận thuốc bảo hiểm gần nhất vì lúc đầu load toa thuốc lên thì đã lấy rồi.
                //var GetMaxDayBuyInsurance = new OutwardDrug_GetMaxDayBuyInsuranceTask(SelectedOutInvoice.SelectedPrescription.PatientID.GetValueOrDefault(), SelectedOutInvoice.outiID);
                //yield return GetMaxDayBuyInsurance;
                //NgayBHGanNhat = GetMaxDayBuyInsurance.NgayBHGanNhat;
            }
            else
            {
                ClientLoggerHelper.LogError("SelectedOutInvoice.SelectedPrescription == null");
            }

            //het phan loadInfoCommonInvoice
            //LoadAndPayMoneyForService(res.OutInvoice);

            var paymentTypeTask = new CalcMoneyPaidedForDrugInvoiceTask(Invoice.outiID);
            yield return paymentTypeTask;
            AmountPaided = paymentTypeTask.Amount;
            if (IsBenhNhanNoiTru())
            {
                GetRegistrationInfo_InPt(Invoice.PtRegistrationID.Value);
            }
            else
            {
                GetRegistrationInfo(Invoice.PtRegistrationID.Value);
            }
            yield break;
        }

        private void ConfirmHIBenefitTask(GenericCoRoutineTask genTask, object _aRegistration)
        {
            PatientRegistration aRegistration = _aRegistration as PatientRegistration;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalRegistrationHIBenefit(aRegistration.PtRegistrationID, Globals.LoggedUserAccount.Staff.StaffID
                            , null
                            , aRegistration.PtInsuranceBenefit
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    string OutputBalanceServicesXML;
                                    genTask.ActionComplete(contract.EndRecalRegistrationHIBenefit(out OutputBalanceServicesXML, asyncResult));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                    genTask.ActionComplete(false);
                                }
                            }), null);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                    genTask.ActionComplete(false);
                }
            });
            t.Start();
        }

        public CollectionView CV_Prescriptions { get; set; }

        private CollectionViewSource CV_Prescriptions_Source;

        private ObservableCollection<OutwardDrugInvoice> _OutwardDrugInvoiceCollection;
        public ObservableCollection<OutwardDrugInvoice> OutwardDrugInvoiceCollection
        {
            get => _OutwardDrugInvoiceCollection; set
            {
                _OutwardDrugInvoiceCollection = value;
                NotifyOfPropertyChange(() => OutwardDrugInvoiceCollection);
            }
        }

        private bool _IsInvoiceSearch = false;
        public bool IsInvoiceSearch
        {
            get => _IsInvoiceSearch; set
            {
                _IsInvoiceSearch = value;
                NotifyOfPropertyChange(() => IsInvoiceSearch);
                NotifyOfPropertyChange(() => IsServiceTabsVisible);
                NotifyOfPropertyChange(() => PayAndComfirmHIIsVisible);
                VisibleDoseColumns(!IsInvoiceSearch);
            }
        }

        public bool IsServiceTabsVisible
        {
            get { return this.IsConfirmHIView && IsInvoiceSearch; }
        }

        public Dictionary<long, IList<OutwardDrug>> SavedOutwardDrugInvoice = new Dictionary<long, IList<OutwardDrug>>();

        public void btnPrepareConfirmHIBenefit()
        {
            if (!this.IsConfirmHIView) return;
            if (SelectedPrescription == null || !SavedOutwardDrugInvoice.ContainsKey(SelectedPrescription.PtRegistrationID.GetValueOrDefault(0))) return;
            CV_Prescriptions_Source = new CollectionViewSource { Source = SavedOutwardDrugInvoice.SelectMany(x => x.Value).ToList() };
            CV_Prescriptions = (CollectionView)CV_Prescriptions_Source.View;
            CV_Prescriptions.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("PrescriptionGroupString"));
            NotifyOfPropertyChange(() => CV_Prescriptions);
            SumTotalPrice();
            IsInvoiceSearch = true;
        }

        public void btnFinalization()
        {
            if (PatientInfo == null || PatientInfo.LatestRegistration == null || Globals.LoggedUserAccount == null || Globals.LoggedUserAccount.Staff == null)
            {
                return;
            }
            GlobalsNAV.ShowDialog<IEditOutPtTransactionFinalization>((IEditOutPtTransactionFinalization aView) =>
            {
                aView.TransactionFinalizationObj = new OutPtTransactionFinalization { TaxMemberName = PatientInfo.FullName, TaxMemberAddress = PatientInfo.PatientStreetAddress
                    ,StaffID = Globals.LoggedUserAccount.Staff.StaffID
                    ,PtRegistrationID = PatientInfo.LatestRegistration.PtRegistrationID
                    ,PatientFullName = PatientInfo.LatestRegistration.Patient == null ? null : PatientInfo.LatestRegistration.Patient.FullName
                    ,V_PaymentMode = (long)AllLookupValues.PaymentMode.TIEN_MAT
                };
            });
        }

        public void ApplySearchInvoiceCriteriaValues(string aOutInvID = null, long? PtRegistrationID = null)
        {
            if (SearchInvoiceCriteria != null)
            {
                SearchInvoiceCriteria.OutInvID = aOutInvID;
                SearchInvoiceCriteria.PtRegistrationID = PtRegistrationID;
            }
        }

        private void VisibleDoseColumns(bool IsVisible)
        {
            try
            {
                if (grdPrescriptionCV == null)
                {
                    return;
                }
                grdPrescriptionCV.GetColumnByName("colMDoseStr").Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
                grdPrescriptionCV.GetColumnByName("colADoseStr").Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
                grdPrescriptionCV.GetColumnByName("colEDoseStr").Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
                grdPrescriptionCV.GetColumnByName("colNDoseStr").Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
                grdPrescriptionCV.GetColumnByName("colDayRpts").Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
            catch { }
        }

        private void DoConfirmHIBenefit(PatientRegistration aRegistration)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new BillingPaymentWcfServiceLibClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRecalRegistrationHIBenefit(aRegistration.PtRegistrationID, Globals.LoggedUserAccount.Staff.StaffID
                            , aRegistration.InPatientBillingInvoices
                            , aRegistration.PtInsuranceBenefit
                            , Globals.DispatchCallback((asyncResult) =>
                            {
                                string OutputBalanceServicesXML = null;
                                try
                                {
                                    var retval = contract.EndRecalRegistrationHIBenefit(out OutputBalanceServicesXML, asyncResult);
                                    LoadInfoCommonInvoice(false, true);
                                    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2387_G1_XacNhanThanhCong);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                            }), null);
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
            });
            t.Start();
        }

        private bool _IsConfirmPrescriptionOnly = false;
        public bool IsConfirmPrescriptionOnly
        {
            get => _IsConfirmPrescriptionOnly; set
            {
                _IsConfirmPrescriptionOnly = value;
                NotifyOfPropertyChange(() => IsConfirmPrescriptionOnly);
                NotifyOfPropertyChange(() => mBanThuocTheoToa_ThuTien);
                NotifyOfPropertyChange(() => IsAccountingView);
                NotifyOfPropertyChange(() => IsHasBillingInvoice);
            }
        }

        private bool _IsServicePatient = false;
        public bool IsServicePatient
        {
            get => _IsServicePatient; set
            {
                _IsServicePatient = value;
                NotifyOfPropertyChange(() => IsServicePatient);
            }
        }
        //▼====== #009
        AxGrid RootAxGrid;
        public void AxGrid_Loaded(object sender, RoutedEventArgs e)
        {
            RootAxGrid = sender as AxGrid;
        }
        public void AddItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (RootAxGrid != null)
                {
                    RootAxGrid.DisableFirstNextFocus = true;
                }
            }
        }
        public void AddItem_Click(object sender, object e)
        {
            ReCountQtyAndAddList(SelectedSellVisitor);
        }
        //▲====== #009
        private IRegistration_DataStorage _Registration_DataStorage;
        public IRegistration_DataStorage Registration_DataStorage
        {
            get
            {
                return _Registration_DataStorage;
            }
            set
            {
                if (_Registration_DataStorage == value)
                {
                    return;
                }
                _Registration_DataStorage = value;
                NotifyOfPropertyChange(() => Registration_DataStorage);
            }
        }
        public bool IsRefundMoney
        {
            get
            {
                return (SelectedOutInvoice != null && SelectedOutInvoice.RefundMoney) || (IsConfirmHIView && PatientInfo != null && PatientInfo.LatestRegistration != null && PatientInfo.LatestRegistration.ConfirmHIStaffID.GetValueOrDefault(0) == 0 && PatientInfo.LatestRegistration.PayableSum != null && (PatientInfo.LatestRegistration.PayableSum.TotalCashAdvanceAmount > PatientInfo.LatestRegistration.PayableSum.TotalPaymentForTransaction));
            }
        }
        private string _strOutwardInvoiceStatus = "";
        public string strOutwardInvoiceStatus
        {
            get
            {
                return _strOutwardInvoiceStatus;
            }
            set
            {
                if (_strOutwardInvoiceStatus != value)
                {
                    _strOutwardInvoiceStatus = value;
                    NotifyOfPropertyChange(() => strOutwardInvoiceStatus);
                }
            }
        }
        public void Handle(RemoveItem<InPatientBillingInvoice> message)
        {
            if (PatientInfo == null || PatientInfo.LatestRegistration == null)
            {
                return;
            }
            var CurrentRegistration = PatientInfo.LatestRegistration;
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
                NotifyOfPropertyChange(() => IsEnableCancelBillingButton);
            }
        }
        public bool IsAccountingView
        {
            get
            {
                return IsConfirmHIView && !IsConfirmPrescriptionOnly;
            }
        }
        public bool IsEnableCancelBillingButton
        {
            get
            {
                return PatientInfo != null && PatientInfo.LatestRegistration != null && PatientInfo.LatestRegistration.InPatientBillingInvoices != null &&
                    PatientInfo.LatestRegistration.InPatientBillingInvoices.Any(x => x.RecordState == RecordState.DELETED && x.RefundTime == null) &&
                    PatientInfo.LatestRegistration.ConfirmHIStaffID.GetValueOrDefault(0) == 0;
            }
        }
        public void CancelBillingButton()
        {
            if (PatientInfo == null || PatientInfo.LatestRegistration == null)
            {
                return;
            }
            Coroutine.BeginExecute(DoRefund(PatientInfo.LatestRegistration.PtRegistrationID, 0, PatientInfo.LatestRegistration.InPatientBillingInvoices));
        }
        public bool IsHasBillingInvoice
        {
            get
            {
                if (PatientInfo == null || PatientInfo.LatestRegistration == null || PatientInfo.LatestRegistration.InPatientBillingInvoices == null)
                {
                    return false;
                }
                return PatientInfo.LatestRegistration.InPatientBillingInvoices.Any(x => x.PaidTime != null && x.RecordState != RecordState.DELETED);
            }
        }
        private bool _CanCancel;
        public bool CanCancel
        {
            get { return _CanCancel; }
            set
            {
                if (_CanCancel != value)
                {
                    _CanCancel = value;
                    NotifyOfPropertyChange(() => CanCancel);
                }
            }
        }
        private bool _CanEditPayed;
        public bool CanEditPayed
        {
            get { return _CanEditPayed; }
            set
            {
                if (_CanEditPayed != value)
                {
                    _CanEditPayed = value;
                    NotifyOfPropertyChange(() => CanEditPayed);
                }
            }
        }
        private void SetPermissionForOutward()
        {
            if (SelectedOutInvoice == null)
            {
                return;
            }
            if (SelectedOutInvoice.OutDate.Date != Globals.GetCurServerDateTime().Date|| SelectedOutInvoice.V_OutDrugInvStatus ==15003)
            {
                CanCancel = false;
                CanEditPayed = false;
            }
            else if (SelectedOutInvoice.PaidTime == null)
            {
                CanCancel = SelectedOutInvoice.CanCancel;
                CanEditPayed = SelectedOutInvoice.CanEditPayed;
            }
            else
            {
                CanCancel = mBanThuocTheoToa_HuyPhieuXuat && SelectedOutInvoice.DQGReportID == 0;
                CanEditPayed = mBanThuocTheoToa_CapNhatPhieu && SelectedOutInvoice.DQGReportID == 0;
            }

        }

        //▼==== #011
        private Prescription _CurrentChoNhanThuoc;
        public Prescription CurrentChoNhanThuoc
        {
            get => _CurrentChoNhanThuoc; set
            {
                _CurrentChoNhanThuoc = value;
                NotifyOfPropertyChange(() => CurrentChoNhanThuoc);
            }
        }

        private void InHuongDanSuDungThuoc()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentChoNhanThuoc.PrescriptID;
                proAlloc.IsInsurance = false;
                proAlloc.eItem = ReportName.Huong_Dan_Su_Dung_Thuoc;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg, null, false, true, null);
        }
        public void PreviewHDSDCmd(object sender, object eventArgs)
        {
            if (CurrentChoNhanThuoc == null)
            {
                return;
            }
            InHuongDanSuDungThuoc();
        }
        //▲==== #011
    }
}