using System.ComponentModel.Composition;
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
using aEMR.Common.Printing;
using aEMR.Controls;
using aEMR.CommonTasks;
using eHCMSLanguage;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using System.Text;
using System.ServiceModel;
using aEMR.DataContracts;

/*
 * 07082018 #001 TTM:
 * 20190914 #002 TBL: BM 0014338: Cảnh báo nếu nhập hàng không có giá bán DV
 * 20191113 #003 TTM: BM 0019561: Giới hạn độ dài các trường trong phiếu nhập (Mã hoá đơn, số serial). Vì chương trình cho phép tối đa 16 nhưng FAST chỉ lấy được 12 kí tự.
 * 20191219 #004 TBL: BM 0020743: Khi nhập y cụ kiểm tra Lô SX nếu không nhập sẽ cảnh báo khi nhập hàng
 * 20200321 #005 TBL: BM 0026013: Khi nhập đơn giá lẻ, đơn giá đóng gói, thành tiền thì tự động tính giá bán DV và giá bán BH
 * 20200330 #006 TTM: BM 0029055: Fix lỗi khi nhập chiết khấu cho dòng nhập thì lưu thông tin chiết khấu không chính xác.
 * 20200403 #007 TTM: BM 0029078: [Nhập hàng] Fix lỗi hạn sử dụng nhỏ hơn ngày sản xuất vẫn cho lưu.
 * 20200411 #008 TTM: BM 0029095: Thêm điều kiện kiểm tra để không lưu thông tin giá bán dv, giá BH và giá cho BNBH < 0
 * 20200623 #009 TTM: BM 0039291: Sửa lại thầu theo cấu hình mới.
 * 20200807 #010 TNHX: Thêm loại để nhập hàng cho hóa chất
 * 20220806 #011 QTD: Không cho chọn lại NCC khi đã đánh thuốc
 */

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptInwardDrugSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardDrugSupplierViewModel : ViewModelBase, IDrugDeptInwardDrugSupplier
        , IHandle<DrugDeptCloseSearchSupplierEvent>
        , IHandle<DrugDeptCloseSearchInwardIncoiceEvent>
        , IHandle<DrugDeptCloseEditInwardEvent>
        , IHandle<DrugDeptCloseSearchOutMedDeptInvoiceEvent>
    {
        private long V_SupplierType = 7200;
        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NCC;

        public enum DataGridCol
        {
            Code = 1,
            TenThuoc = 2,
            PackagingQty = 10,
            Qty = 11,
            DonGiaPackage = 13,
            DonGiaLe = 14,
            TotalNotVAT = 15,
            ViTri = 16,
            CKPercent = 17,
            CK = 18
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
		
        [ImportingConstructor]
        public InwardDrugSupplierViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            InitializeInvoiceDrug();

            InnitSearchCriteria();

            GetAllCurrency(true);
            LoadGoodsTypeCbx();

            IsHideAuSupplier = true;
            IsEnabled = true;
            IsVisibility = Visibility.Collapsed;

            SupplierCriteria = new SupplierSearchCriteria
            {
                V_SupplierType = V_SupplierType
            };

            CurrentRefGenMedProductDetails = new PagedSortableCollectionView<RefGenMedProductDetails>();
            CurrentRefGenMedProductDetails.OnRefresh += CurrentRefGenMedProductDetails_OnRefresh;
            CurrentRefGenMedProductDetails.PageSize = Globals.PageSize;

            InwardDrugMedDeptList = new PagedSortableCollectionView<InwardDrugMedDept>();
            InwardDrugMedDeptList.OnRefresh += new EventHandler<RefreshEventArgs>(InwardDrugMedDeptList_OnRefresh);
            InwardDrugMedDeptList.PageSize = Globals.PageSize;

            Suppliers = new PagedSortableCollectionView<DrugDeptSupplier>();
            Suppliers.OnRefresh += new EventHandler<RefreshEventArgs>(Suppliers_OnRefresh);
            Suppliers.PageSize = Globals.PageSize;
        }
        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        private long inviID = 0;
        void InwardDrugMedDeptList_OnRefresh(object sender, RefreshEventArgs e)
        {
            InwardDrugDetails_ByID(inviID, InwardDrugMedDeptList.PageIndex, InwardDrugMedDeptList.PageSize, null);
        }

        void CurrentRefGenMedProductDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(null, BrandName, CurrentRefGenMedProductDetails.PageIndex, CurrentRefGenMedProductDetails.PageSize);
        }

        #region Properties Member
        private double _TotalPercent = 0;
        public double TotalPercent
        {
            get
            {
                return _TotalPercent;
            }
            set
            {
                _TotalPercent = value;
                NotifyOfPropertyChange(() => TotalPercent);
            }
        }

        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

        private InwardInvoiceSearchCriteria _searchCriteria;
        public InwardInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private int _CDC = 0;
        public int CDC
        {
            get
            {
                return _CDC;
            }
            set
            {
                _CDC = value;
                NotifyOfPropertyChange(() => CDC);
            }
        }

        private InwardDrugMedDeptInvoice _CurrentInwardDrugMedDeptInvoice;
        public InwardDrugMedDeptInvoice CurrentInwardDrugMedDeptInvoice
        {
            get
            {
                return _CurrentInwardDrugMedDeptInvoice;
            }
            set
            {
                if (_CurrentInwardDrugMedDeptInvoice != value)
                {
                    _CurrentInwardDrugMedDeptInvoice = value;
                    if (value != null && value.CanEditAndDelete && !IsRetundView)
                    {
                        IsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        IsVisibility = Visibility.Collapsed;
                    }
                    NotifyOfPropertyChange(() => IsVisibility);
                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDeptInvoice);
                }
            }
        }

        IList<Currency> _Currencies;
        public IList<Currency> Currencies
        {
            get
            {
                return _Currencies;
            }
            set
            {
                if (_Currencies != value)
                {
                    _Currencies = value;
                    NotifyOfPropertyChange(() => Currencies);
                }
            }
        }

        private ObservableCollection<Lookup> _CbxGoodsTypes;
        public ObservableCollection<Lookup> CbxGoodsTypes
        {
            get
            {
                return _CbxGoodsTypes;
            }
            set
            {
                _CbxGoodsTypes = value;
                NotifyOfPropertyChange(() => CbxGoodsTypes);
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

        private PagedSortableCollectionView<InwardDrugMedDept> _InwardDrugMedDeptList;
        public PagedSortableCollectionView<InwardDrugMedDept> InwardDrugMedDeptList
        {
            get
            {
                return _InwardDrugMedDeptList;
            }
            set
            {
                if (_InwardDrugMedDeptList != value)
                {
                    _InwardDrugMedDeptList = value;
                    NotifyOfPropertyChange(() => InwardDrugMedDeptList);
                }
            }
        }

        private InwardDrugMedDeptInvoice CurrentInwardInvoiceCopy;

        private ObservableCollection<DrugDeptPurchaseOrderDetail> _DrugDeptPurchaseOrderDetailList;
        public ObservableCollection<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetailList
        {
            get
            {
                return _DrugDeptPurchaseOrderDetailList;
            }
            set
            {
                if (_DrugDeptPurchaseOrderDetailList != value)
                {
                    _DrugDeptPurchaseOrderDetailList = value;
                    NotifyOfPropertyChange(() => DrugDeptPurchaseOrderDetailList);
                }
            }
        }

        private DrugDeptPurchaseOrderDetail _CurrentDrugDeptPurchaseOrderDetail;
        public DrugDeptPurchaseOrderDetail CurrentDrugDeptPurchaseOrderDetail
        {
            get
            {
                return _CurrentDrugDeptPurchaseOrderDetail;
            }
            set
            {
                if (_CurrentDrugDeptPurchaseOrderDetail != value)
                {
                    _CurrentDrugDeptPurchaseOrderDetail = value;
                    NotifyOfPropertyChange(() => CurrentDrugDeptPurchaseOrderDetail);
                }
            }
        }

        private bool _IsPercentInwardDrug;
        public bool IsPercentInwardDrug
        {
            get
            {
                return _IsPercentInwardDrug;
            }
            set
            {
                _IsPercentInwardDrug = value;
                NotifyOfPropertyChange(() => IsPercentInwardDrug);
                CheckIsPercentAll();
            }
        }

        private ObservableCollection<DrugDeptPurchaseOrder> _DrugDeptPurchaseOrderList;
        public ObservableCollection<DrugDeptPurchaseOrder> DrugDeptPurchaseOrderList
        {
            get
            {
                return _DrugDeptPurchaseOrderList;
            }
            set
            {
                if (_DrugDeptPurchaseOrderList != value)
                {
                    _DrugDeptPurchaseOrderList = value;
                    NotifyOfPropertyChange(() => DrugDeptPurchaseOrderList);
                }
            }
        }

        private InwardDrugMedDept _CurrentInwardDrugMedDept;
        public InwardDrugMedDept CurrentInwardDrugMedDept
        {
            get
            {
                return _CurrentInwardDrugMedDept;
            }
            set
            {
                if (_CurrentInwardDrugMedDept != value)
                {
                    _CurrentInwardDrugMedDept = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDept);
                }
            }
        }

        private InwardDrugMedDept _CurrentInwardDrugMedDeptCopy;
        public InwardDrugMedDept CurrentInwardDrugMedDeptCopy
        {
            get
            {
                return _CurrentInwardDrugMedDeptCopy;
            }
            set
            {
                if (_CurrentInwardDrugMedDeptCopy != value)
                {
                    _CurrentInwardDrugMedDeptCopy = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrugMedDeptCopy);
                }
            }
        }

        //private ObservableCollection<InwardDrugMedDept> InwardDrugMedDeptListCopy;

        private bool _IsHideAuSupplier = true;
        public bool IsHideAuSupplier
        {
            get
            {
                return _IsHideAuSupplier;
            }
            set
            {
                if (_IsHideAuSupplier != value)
                {
                    _IsHideAuSupplier = value;
                    NotifyOfPropertyChange(() => IsHideAuSupplier);
                }
            }
        }

        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        private Visibility _IsVisibility = Visibility.Collapsed;
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
                    if (_IsVisibility == Visibility.Visible || IsRetundView)
                    {
                        ElseVisibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ElseVisibility = Visibility.Visible;
                    }
                    NotifyOfPropertyChange(() => ElseVisibility);
                    NotifyOfPropertyChange(() => IsVisibility);
                }
            }
        }

        private Visibility _ElseVisibility = Visibility.Visible;
        public Visibility ElseVisibility
        {
            get
            {
                return _ElseVisibility;
            }
            set
            {
                if (_ElseVisibility != value)
                {
                    _ElseVisibility = value;
                    NotifyOfPropertyChange(() => ElseVisibility);
                }
            }
        }

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }
            }
        }

        private bool _PurchaseOrderAllChecked = false;
        public bool PurchaseOrderAllChecked
        {
            get
            {
                return _PurchaseOrderAllChecked;
            }
            set
            {
                if (_PurchaseOrderAllChecked != value)
                {
                    _PurchaseOrderAllChecked = value;
                    NotifyOfPropertyChange(() => PurchaseOrderAllChecked);
                    if (_PurchaseOrderAllChecked)
                    {
                        Process_AllPurchaseOrderChecked();
                    }
                    else
                    {
                        Process_AllPurchaseOrderUnChecked();
                    }
                }
            }
        }

        private bool _IsRefundView = false;
        public bool IsRetundView
        {
            get => _IsRefundView; set
            {
                _IsRefundView = value;
                NotifyOfPropertyChange(() => IsRetundView);
                NotifyOfPropertyChange(() => mThemMoi);
                NotifyOfPropertyChange(() => mCapNhat);
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

        private List<DrugDeptSellPriceProfitScale> _ObjDrugDeptSellPriceProfitScale;
        public List<DrugDeptSellPriceProfitScale> ObjDrugDeptSellPriceProfitScale
        {
            get { return _ObjDrugDeptSellPriceProfitScale; }
            set
            {
                _ObjDrugDeptSellPriceProfitScale = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellPriceProfitScale);
            }
        }
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
        }

        #region checking account
        private bool _mTim = true;
        private bool _mThemMoi = true;
        private bool _mIn = true;
        private bool _mCapNhat = true;

        public bool mTim
        {
            get
            {
                return _mTim;
            }
            set
            {
                if (_mTim == value)
                    return;
                _mTim = value;
                NotifyOfPropertyChange(() => mTim);
            }
        }

        public bool mThemMoi
        {
            get
            {
                return _mThemMoi & !IsRetundView;
            }
            set
            {
                if (_mThemMoi == value)
                    return;
                _mThemMoi = value;
                NotifyOfPropertyChange(() => mThemMoi);
            }
        }

        public bool mIn
        {
            get
            {
                return _mIn;
            }
            set
            {
                if (_mIn == value)
                    return;
                _mIn = value;
                NotifyOfPropertyChange(() => mIn);
            }
        }

        public bool mCapNhat
        {
            get
            {
                return _mCapNhat && !IsRetundView;
            }
            set
            {
                if (_mCapNhat == value)
                    return;
                _mCapNhat = value;
                NotifyOfPropertyChange(() => mCapNhat);
            }
        }

        private bool _bbtnSearch = true;
        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

        public bool bbtnSearch
        {
            get
            {
                return _bbtnSearch;
            }
            set
            {
                if (_bbtnSearch == value)
                    return;
                _bbtnSearch = value;
            }
        }

        public bool bEdit
        {
            get
            {
                return _bEdit;
            }
            set
            {
                if (_bEdit == value)
                    return;
                _bEdit = value;
            }
        }

        public bool bAdd
        {
            get
            {
                return _bAdd;
            }
            set
            {
                if (_bAdd == value)
                    return;
                _bAdd = value;
            }
        }

        public bool bDelete
        {
            get
            {
                return _bDelete;
            }
            set
            {
                if (_bDelete == value)
                    return;
                _bDelete = value;
            }
        }

        public bool bView
        {
            get
            {
                return _bView;
            }
            set
            {
                if (_bView == value)
                    return;
                _bView = value;
            }
        }

        public bool bPrint
        {
            get
            {
                return _bPrint;
            }
            set
            {
                if (_bPrint == value)
                    return;
                _bPrint = value;
            }
        }

        public bool bReport
        {
            get
            {
                return _bReport;
            }
            set
            {
                if (_bReport == value)
                    return;
                _bReport = value;
            }
        }
        #endregion

        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkDeleteMain { get; set; }
        public Button lnkEdit { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        public void lnkDeleteMain_Loaded(object sender)
        {
            lnkDeleteMain = sender as Button;
            lnkDeleteMain.Visibility = Globals.convertVisibility(bDelete);
        }

        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(bEdit);
        }

        public StackPanel StackTest { get; set; }

        public void StackTest_Loaded(object sender)
        {
            StackTest = sender as StackPanel;
            StackTest.DataContext = this;
        }
        #endregion

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void GetAllCurrency(bool value)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllCurrency(value, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                Currencies = contract.EndGetAllCurrency(asyncResult);
                                CurrentInwardDrugMedDeptInvoice.SelectedCurrency = Currencies.FirstOrDefault();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void LoadGoodsTypeCbx()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new CommonService_V2Client())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetAllLookupValuesByType(LookupValues.V_GoodsType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetAllLookupValuesByType(asyncResult);
                                CbxGoodsTypes = results.ToObservableCollection();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false, true);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
                if (StoreCbx.FirstOrDefault().IsVATCreditStorage)
                {
                    CurrentInwardDrugMedDeptInvoice.IsVATCredit = true;
                }
                else
                {
                    CurrentInwardDrugMedDeptInvoice.IsVATCredit = false;
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            yield break;
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            InitializeInvoiceDrug();
            NotifyOfPropertyChange(() => IsHadDetail);
        }

        private void InitializeInvoiceDrug()
        {
            CurrentInwardDrugMedDeptInvoice = new InwardDrugMedDeptInvoice
            {
                InvDateInvoice = Globals.GetCurServerDateTime(),
                DSPTModifiedDate = Globals.GetCurServerDateTime(),
                SelectedStaff = GetStaffLogin(),
                StaffID = GetStaffLogin().StaffID,
                IsTrongNuoc = true,
                IsForeign = false,

                //if (Globals.ConfigList != null)
                //{
                //    CurrentInwardDrugMedDeptInvoice.VAT = Decimal.Parse(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyDefaultVATInward].ToString(), CultureInfo.InvariantCulture);
                //}

                // Txd 25/05/2014 Replaced ConfigList
                VAT = (decimal)Globals.ServerConfigSection.PharmacyElements.PharmacyDefaultVATInward,

                SelectedCurrency = new Currency()
            };
            CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID = (long)AllLookupValues.CurrencyTable.VND;

            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
                if (StoreCbx.FirstOrDefault().IsVATCreditStorage)
                {
                    CurrentInwardDrugMedDeptInvoice.IsVATCredit = true;
                }
                else
                {
                    CurrentInwardDrugMedDeptInvoice.IsVATCredit = false;
                }
            }
            IsHideAuSupplier = true;
            if (InwardDrugMedDeptList != null)
            {
                InwardDrugMedDeptList.Clear();
            }
            DeepCopyInvoice();
            InitDetailInward();
            TotalPercent = 0;
        }

        private void InitDetailInward()
        {
            if (DrugDeptPurchaseOrderDetailList != null)
            {
                DrugDeptPurchaseOrderDetailList.Clear();
            }
            IsEnabled = true;
            if (ckbDH != null)
            {
                ckbDH.IsChecked = false;
            }
        }

        private void DeepCopyInvoice()
        {
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.inviID > 0
                && CurrentInwardDrugMedDeptInvoice.SupplierID.GetValueOrDefault(0) > 0)
            {
                LoadValidBidFromSupplierID(CurrentInwardDrugMedDeptInvoice.SupplierID.Value, V_MedProductType);
            }
            else
            {
                BidCollection = new ObservableCollection<Bid>();
            }
            CurrentInwardInvoiceCopy = CurrentInwardDrugMedDeptInvoice.DeepCopy();
        }

        private void DeleteInwardInvoiceDrug()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteInwardDrugMedDeptInvoice(CurrentInwardDrugMedDeptInvoice.inviID , V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndDeleteInwardDrugMedDeptInvoice(asyncResult);
                                if (results == 0)
                                {
                                    InitializeInvoiceDrug();
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0575_G1_HDDaXoaThanhCong));
                                }
                                else if (results == 1)
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0576_G1_KgTheXoaViDaXuat), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0577_G1_PhKgTonTai), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void btnDeletePhieu(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0557_G1_CoChacMuonXoa, eHCMSResources.Z0578_G1_PhNhap), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteInwardInvoiceDrug();
            }
        }

        private void CheckIsPercentAll()
        {
            if (DrugDeptPurchaseOrderDetailList != null)
            {
                for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
                {
                    DrugDeptPurchaseOrderDetailList[i].IsPercent = IsPercentInwardDrug;
                }
            }
        }

        public string StringFormat = "Total: {0:C}";
        private void UpdateInwardInvoiceDrug()
        {
            if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency == null || CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.K3709_G1_DViTinh), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
            {
                if (CurrentInwardDrugMedDeptInvoice.ExchangeRates <= 0)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z0581_G1_TyGia), eHCMSResources.G0442_G1_TBao);
                    return;
                }
            }


            CurrentInwardDrugMedDeptInvoice.TypID = TypID;
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginUpdateInwardDrugMedDeptInvoice(CurrentInwardDrugMedDeptInvoice, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                int results = contract.EndUpdateInwardDrugMedDeptInvoice(asyncResult);
                                if (results == 0)
                                {
                                    DeepCopyInvoice();
                                    CheckIsPercentAll();
                                    Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID, null);
                                    InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0279_G1_Msg_InfoCNhatOK), eHCMSResources.G0442_G1_TBao);
                                }
                                else if (results == 1)
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0582_G1_HDDaTonTai), eHCMSResources.T0074_G1_I);
                                }
                                else
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0577_G1_PhKgTonTai), eHCMSResources.T0074_G1_I);
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void btnEdit(object sender, RoutedEventArgs e)
        {
            string ErrorMsg = "";
            if (MessageBox.Show(string.Format(eHCMSResources.Z0583_G1_CoMuonChinhSuaKg, eHCMSResources.Z0578_G1_PhNhap), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CheckValidate())
                {
                    //▼===== #003
                    if (!Globals.CheckLimitCharaterOfInwardInvoice(out ErrorMsg, CurrentInwardDrugMedDeptInvoice.InvInvoiceNumber, CurrentInwardDrugMedDeptInvoice.SerialNumber))
                    {
                        MessageBox.Show(ErrorMsg, eHCMSResources.G0449_G1_TBaoLoi);
                        return;
                    }
                    //▲===== #003
                    if (CurrentInwardDrugMedDeptInvoice.SelectedStorage != null)
                    {
                        CurrentInwardDrugMedDeptInvoice.StoreID = CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID;
                    }
                    if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency != null)
                    {
                        CurrentInwardDrugMedDeptInvoice.CurrencyID = CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID;
                    }
                    UpdateInwardInvoiceDrug();
                }
            }
        }

        private void CountTotalPrice(decimal TongTienSPChuaVAT,
            decimal CKTrenSP,
            decimal TongTienTrenSPDaTruCK,
            decimal TongCKTrenHoaDon,
            decimal TongTienHoaDonCoThueNK,
            decimal TongTienHoaDonCoVAT,
            decimal TotalVATDifferenceAmount)
        {
            CurrentInwardDrugMedDeptInvoice.TotalPriceNotVAT = TongTienSPChuaVAT;
            CurrentInwardDrugMedDeptInvoice.TotalDiscountOnProduct = CKTrenSP;//tong ck tren hoan don
            CurrentInwardDrugMedDeptInvoice.TotalDiscountInvoice = CKTrenSP + TongCKTrenHoaDon;
            CurrentInwardDrugMedDeptInvoice.TotalPrice = TongTienSPChuaVAT - (CKTrenSP + TongCKTrenHoaDon);
            CurrentInwardDrugMedDeptInvoice.TotalHaveCustomTax = TongTienHoaDonCoThueNK;
            CurrentInwardDrugMedDeptInvoice.TotalPriceVAT = TongTienHoaDonCoVAT;
            CurrentInwardDrugMedDeptInvoice.TotalVATDifferenceAmount = TotalVATDifferenceAmount;
            CurrentInwardDrugMedDeptInvoice.TotalVATAmountActual = CurrentInwardDrugMedDeptInvoice.TotalVATAmount + CurrentInwardDrugMedDeptInvoice.TotalVATDifferenceAmount;
            NotifyOfPropertyChange(() => IsHadDetail);
        }

        private void Load_InwardDrugDetails(long ID, OutwardDrugMedDeptInvoice aReturnOutInv)
        {
            inviID = ID;
            InwardDrugMedDeptList.PageIndex = 0;
            InwardDrugDetails_ByID(CurrentInwardDrugMedDeptInvoice.inviID, 0, Globals.PageSize, aReturnOutInv);
        }

        private List<InwardDrugMedDept> InwardDrugMedDeptRemain;
        private void InwardDrugDetails_ByID(long ID, int PageIndex, int PageSize, OutwardDrugMedDeptInvoice aReturnOutInv)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginGetInwardDrugMedDept_ByIDInvoice(ID, PageSize, PageIndex, true, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                int total = 0;
                                decimal TongTienSPChuaVAT = 0;
                                decimal CKTrenSP = 0;
                                decimal TongTienTrenSPDaTruCK = 0;
                                decimal TongCKTrenHoaDon = 0;
                                decimal TongTienHoaDonCoThueNK = 0;
                                decimal TongTienHoaDonCoVAT = 0;
                                decimal TotalVATDifferenceAmount = 0;
                                var results = contract.EndGetInwardDrugMedDept_ByIDInvoice(out total
                                        , out TongTienSPChuaVAT
                                        , out CKTrenSP
                                        , out TongTienTrenSPDaTruCK
                                        , out TongCKTrenHoaDon
                                        , out TongTienHoaDonCoThueNK
                                        , out TongTienHoaDonCoVAT
                                        , out TotalVATDifferenceAmount, asyncResult);

                                //load danh sach thuoc theo hoa don 
                                InwardDrugMedDeptList.Clear();
                                InwardDrugMedDeptList.TotalItemCount = total;
                                if (results != null)
                                {
                                    foreach (InwardDrugMedDept p in results)
                                    {
                                        InwardDrugMedDeptList.Add(p);
                                    }
                                }
                                if (InwardDrugMedDeptList != null && InwardDrugMedDeptList.Count > 0
                                    && InwardDrugMedDeptList.Any(x => x.RefGenMedProductDetails != null && x.RefGenMedProductDetails.BidID > 0))
                                {
                                    IsInvoiceCanDefineBid = false;
                                    //20200408 TBL: BM 0029102: Không lấy mặc định đợt thầu
                                    //SelectedBidID = InwardDrugMedDeptList.First(x => x.RefGenMedProductDetails != null && x.RefGenMedProductDetails.BidID > 0).RefGenMedProductDetails.BidID;
                                }
                                else
                                {
                                    IsInvoiceCanDefineBid = true;
                                }
                                if (aReturnOutInv != null)
                                {
                                    OutwardDrugMedDeptDetails_Load(aReturnOutInv.outiID);
                                }
                                //tinh tong tien 
                                CountTotalPrice(TongTienSPChuaVAT, CKTrenSP, TongTienTrenSPDaTruCK, TongCKTrenHoaDon, TongTienHoaDonCoThueNK, TongTienHoaDonCoVAT, TotalVATDifferenceAmount);

                                //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
                                CheckEnableSupplier();

                                //if (InwardDrugMedDeptList != null && InwardDrugMedDeptList.Count > 0)
                                //{
                                //    IsHideAuSupplier = false;
                                //}
                                //else
                                //{
                                //    IsHideAuSupplier = true;
                                //}
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
            GetInwardDrugMedDept_ByIDDrugDeptInIDOrig(ID);
        }

        private void GetInwardDrugMedDept_ByIDDrugDeptInIDOrig(long inviID)
        {
            InwardDrugMedDeptRemain = new List<InwardDrugMedDept>();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginGetInwardDrugMedDept_ByIDDrugDeptInIDOrig(inviID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                var ItemCollection = contract.EndGetInwardDrugMedDept_ByIDDrugDeptInIDOrig(asyncResult);
                                if (ItemCollection != null)
                                {
                                    InwardDrugMedDeptRemain = ItemCollection.ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void PharmacyPurchaseOrder_BySupplierID()
        {
            if (CurrentInwardDrugMedDeptInvoice.SelectedSupplier != null)
            {
                PharmacyPurchaseOrder_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
            }
            else
            {
                if (DrugDeptPurchaseOrderList != null)
                {
                    DrugDeptPurchaseOrderList.Clear();
                }
            }
        }

        private void PharmacyPurchaseOrder_BySupplierID(long SupplierID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrder_BySupplierID(SupplierID, V_MedProductType, SelectedBidID.GetValueOrDefault(), Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptPurchaseOrder_BySupplierID(asyncResult);
                                DrugDeptPurchaseOrderList = results.ToObservableCollection();
                                //if (IsHideAuSupplier)
                                //{
                                //    if ((CurrentInwardDrugMedDeptInvoice == null || CurrentInwardDrugMedDeptInvoice.inviID == 0) && (DrugDeptPurchaseOrderList == null || DrugDeptPurchaseOrderList.Count == 0))
                                //    {
                                //        Globals.ShowMessage("Không có đơn đặt hàng cho NCC này", eHCMSResources.G0442_G1_TBao);
                                //    }
                                //}
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetInwardInvoiceDrugByID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginGetInwardDrugMedDeptInvoice_ByID(ID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                CurrentInwardDrugMedDeptInvoice = contract.EndGetInwardDrugMedDeptInvoice_ByID(asyncResult);
                                DeepCopyInvoice();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void AddInwardInvoiceDrug()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            CurrentInwardDrugMedDeptInvoice.TypID = TypID;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddInwardDrugMedDeptInvoice(CurrentInwardDrugMedDeptInvoice, IsNotCheckInvalid, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID;
                                int results = contract.EndAddInwardDrugMedDeptInvoice(out ID, asyncResult);
                                if (results == 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK, eHCMSResources.G0442_G1_TBao);
                                    IsNotCheckInvalid = false;
                                    GetInwardInvoiceDrugByID(ID);
                                }
                                else
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0584_G1_SoHDDaTonTai), eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                //MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                if (ex.Message.Contains("19090610") && MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    //20191230 TBL: Nếu đồng ý lưu thì lưu lại và bỏ qua kiểm tra dưới store
                                    IsNotCheckInvalid = true;
                                    AddInwardInvoiceDrug();
                                }
                                else if (!ex.Message.Contains("19090610"))
                                {
                                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void GoodsType_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            cb.ItemsSource = CbxGoodsTypes;
            cb.SelectedIndex = 0;
        }

        private bool CheckValidate()
        {
            if (CurrentInwardDrugMedDeptInvoice == null)
            {
                return false;
            }
            return CurrentInwardDrugMedDeptInvoice.Validate();
        }

        public void btnAdd(object sender, RoutedEventArgs e)
        {
            string ErrorMsg = "";
            //nhap thong tin phieu nhap vao database
            if (CheckValidate())
            {
                if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency == null || CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID == 0)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.K3709_G1_DViTinh), eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
                {
                    if (CurrentInwardDrugMedDeptInvoice.ExchangeRates <= 0)
                    {
                        Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z0581_G1_TyGia), eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                }
                if (CurrentInwardDrugMedDeptInvoice.VAT < 1 && CurrentInwardDrugMedDeptInvoice.VAT > 0)
                {
                    Globals.ShowMessage(eHCMSResources.K0262_G1_VATKhongHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugMedDeptInvoice.DiscountingByPercent < 1 && CurrentInwardDrugMedDeptInvoice.DiscountingByPercent > 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0072_G1_CKKhHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▼===== #003
                if (!Globals.CheckLimitCharaterOfInwardInvoice(out ErrorMsg, CurrentInwardDrugMedDeptInvoice.InvInvoiceNumber, CurrentInwardDrugMedDeptInvoice.SerialNumber))
                {
                    MessageBox.Show(ErrorMsg, eHCMSResources.G0449_G1_TBaoLoi);
                    return;
                }
                //▲===== #003
                if (CurrentInwardDrugMedDeptInvoice.SelectedStorage != null)
                {
                    CurrentInwardDrugMedDeptInvoice.StoreID = CurrentInwardDrugMedDeptInvoice.SelectedStorage.StoreID;
                }
                CurrentInwardDrugMedDeptInvoice.CurrencyID = CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID;
                CurrentInwardDrugMedDeptInvoice.V_MedProductType = V_MedProductType;
                AddInwardInvoiceDrug();
            }
        }

        private void DeleteDrugDeptPurchaseOrderDetail(DrugDeptPurchaseOrderDetail item)
        {
            DrugDeptPurchaseOrderDetailList.Remove(item);

            //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
            CheckEnableSupplier();
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentDrugDeptPurchaseOrderDetail != null)
                {
                    DeleteDrugDeptPurchaseOrderDetail(CurrentDrugDeptPurchaseOrderDetail);
                }
            }
        }

        public void lnkRefGenMedProductDetail_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptPurchaseOrderDetail == null || CurrentDrugDeptPurchaseOrderDetail.GenMedProductID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0645_G1_Msg_InfoKhCoHgDeShow), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            GetRefGenMedProductDetails(CurrentDrugDeptPurchaseOrderDetail.GenMedProductID, null);
        }

        public void lnkRefGenMedProductDetail_Inward_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugMedDept == null || CurrentInwardDrugMedDept.GenMedProductID == null || CurrentInwardDrugMedDept.GenMedProductID <= 0)
            {
                return;
            }

            GetRefGenMedProductDetails(CurrentInwardDrugMedDept.GenMedProductID.GetValueOrDefault(), CurrentInwardDrugMedDept.GenMedVersionID);
        }

        private void GetRefGenMedProductDetails(long genMedProductID, long? genMedVersionID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefGenMedProductDetails(genMedProductID, genMedVersionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RefGenMedProductDetails results = contract.EndGetRefGenMedProductDetails(asyncResult);
                                if (results != null)
                                {
                                    ShowRefGenMedProductDetails(results);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}.", eHCMSResources.A0638_G1_Msg_InfoKhCoData), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void ShowRefGenMedProductDetails(RefGenMedProductDetails product)
        {
            if (product == null)
            {
                return;
            }

            switch (product.V_MedProductType)
            {
                case 11001:
                    {
                        //20180801 TTM neu mo chuc nang cho phep su dung kho BHYT thi su dung view moi
                        if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
                        {
                            void onInitDlg_V3(ICMDrug_AddEdit_V3 typeInfo)
                            {
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                                typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, product.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                                typeInfo.CanEdit = false;
                            }
                            GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V3>(onInitDlg_V3);
                            break;
                        }
                        else
                        {
                            void onInitDlg(ICMDrug_AddEdit_V2 typeInfo)
                            {
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                                typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, product.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                                typeInfo.CanEdit = false;
                            }
                            GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V2>(onInitDlg);
                            break;
                        }
                    }
                case 11002:
                    {
                        void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
                        {
                            typeInfo.V_MedProductType = V_MedProductType;
                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                            typeInfo.IsShowDispenseVolume = true;
                            typeInfo.IsShowMedicalMaterial = true;
                            typeInfo.SetRadioButton();
                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                            typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                            typeInfo.CanEdit = false;
                        }
                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg);
                        break;
                    }
                case 11003:
                    {
                        void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
                        {
                            typeInfo.V_MedProductType = V_MedProductType;
                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                            typeInfo.IsShowDispenseVolume = true;
                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                            typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                            typeInfo.CanEdit = false;
                        }
                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg);
                        break;
                    }
                case 11014:
                    {
                        void onInitDlg(IDrugDept_MedDevAndChem_AddEdit typeInfo)
                        {
                            typeInfo.V_MedProductType = V_MedProductType;
                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                            typeInfo.IsShowDispenseVolume = true;
                            typeInfo.IsShowMedicalMaterial = true;
                            typeInfo.SetRadioButton();
                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                            typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                            typeInfo.CanEdit = false;
                        }
                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg);
                        break;
                    }
            }
        }

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void rdtTrongNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentInwardDrugMedDeptInvoice.IsForeign = false;
        }

        public void rdtNgoaiNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentInwardDrugMedDeptInvoice.IsForeign = true;
        }

        private long? DrugDeptPoID = 0;
        private void DrugDeptPurchaseOrderDetail_ByParentID(object Item)
        {
            DrugDeptPurchaseOrder CurrentOrder = Item as DrugDeptPurchaseOrder;
            if (IsInvoiceCanDefineBid && CurrentOrder.BidID > 0)
            {
                SelectedBidID = CurrentOrder.BidID;
                IsInvoiceCanDefineBid = false;
            }
            else if (!IsInvoiceCanDefineBid && CurrentOrder.BidID > 0 && SelectedBidID > 0 && CurrentOrder.BidID != SelectedBidID)
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2740_G1_PhieuNhapCoChua2DThau);
                return;
            }
            DrugDeptPoID = CurrentOrder.DrugDeptPoID;
            DrugDeptPurchaseOrderDetail_ByParentID(DrugDeptPoID.GetValueOrDefault());
        }

        DataGrid GridInwardDrug = null;
        public void GridInwardDrug_Loaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug = sender as DataGrid;
            InitControls();
        }

        public void GridInwardDrug_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void TinhTienDonDatHang()
        {
            if (DrugDeptPurchaseOrderDetailList != null)
            {
                for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
                {
                    DrugDeptPurchaseOrderDetailList[i].TotalPriceNotVAT = DrugDeptPurchaseOrderDetailList[i].UnitPrice * (decimal)DrugDeptPurchaseOrderDetailList[i].InQuantity;
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail != null)
                    {
                        if (DrugDeptPurchaseOrderDetailList[i].InQuantity > 0 && DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1) != 0)
                        {
                            DrugDeptPurchaseOrderDetailList[i].PackageQuantity = DrugDeptPurchaseOrderDetailList[i].InQuantity / DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                        }
                        else if (DrugDeptPurchaseOrderDetailList[i].PackageQuantity > 0)
                        {
                            if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault() > 0)
                            {
                                DrugDeptPurchaseOrderDetailList[i].InQuantity = DrugDeptPurchaseOrderDetailList[i].PackageQuantity * DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault();
                            }
                        }

                        if (DrugDeptPurchaseOrderDetailList[i].UnitPrice > 0)
                        {
                            DrugDeptPurchaseOrderDetailList[i].PackagePrice = DrugDeptPurchaseOrderDetailList[i].UnitPrice * DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault();
                        }
                        else if (DrugDeptPurchaseOrderDetailList[i].PackagePrice > 0)
                        {
                            if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault() > 0)
                            {
                                DrugDeptPurchaseOrderDetailList[i].UnitPrice = DrugDeptPurchaseOrderDetailList[i].PackagePrice / DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.UnitPackaging.GetValueOrDefault();
                            }
                        }

                        //20200325 TBL: BM 0029040: Khi load phiếu đặt hàng thì sẽ tự động tính giá bán DV và BH theo thang giá bán nếu cấu hình mở
                        if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                        {
                            SetPriceByPriceScale(DrugDeptPurchaseOrderDetailList[i]);
                        }
                    }
                }
            }
        }

        private void DrugDeptPurchaseOrderDetail_ByParentID(long PharmacyPoID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrderDetail_ByParentID(PharmacyPoID, 1, SelectedBidID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptPurchaseOrderDetail_ByParentID(asyncResult);
                                ClearDrugDeptPurchaseOrderDetailList();
                                DrugDeptPurchaseOrderDetailList = results.ToObservableCollection();
                                //so sanh ds load len voi ds ngay ben duoi
                                //GetDrugDeptPurchaseOrderDetailList(PharmacyPoID);
                                CheckIsPercentAll();
                                TinhTienDonDatHang();
                                //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
                                CheckEnableSupplier();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void ClearDrugDeptPurchaseOrderDetailList()
        {
            if (DrugDeptPurchaseOrderDetailList != null)
            {
                DrugDeptPurchaseOrderDetailList.Clear();
            }
        }

        public void cbxChonDH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxChonDH = sender as ComboBox;
            if (cbxChonDH != null)
            {
                if (cbxChonDH.SelectedItem != null)
                {
                    DrugDeptPurchaseOrderDetail_ByParentID(cbxChonDH.SelectedItem);
                }
                else
                {
                    ClearDrugDeptPurchaseOrderDetailList();
                    DrugDeptPoID = 0;
                    //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
                    CheckEnableSupplier();
                }
            }
            else
            {
                DrugDeptPoID = 0;

                //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
                CheckEnableSupplier();
            }
        }

        //private void IsChangedItem()
        //{
        //    if (CurrentInwardDrugMedDeptInvoice.CurrencyID != CurrentInwardInvoiceCopy.CurrencyID || CurrentInwardDrugMedDeptInvoice.ExchangeRates != CurrentInwardInvoiceCopy.ExchangeRates || CurrentInwardDrugMedDeptInvoice.VAT != CurrentInwardInvoiceCopy.VAT || CurrentInwardDrugMedDeptInvoice.IsForeign != CurrentInwardInvoiceCopy.IsForeign || CurrentInwardDrugMedDeptInvoice.Notes != CurrentInwardInvoiceCopy.Notes)
        //    {
        //        if (MessageBox.Show(eHCMSResources.Z0587_G1_CoMuonCNhatHD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
        //        {
        //            if (CheckValidate())
        //            {
        //                UpdateInwardInvoiceDrug();
        //            }
        //            else
        //            {
        //                CurrentInwardDrugMedDeptInvoice = CurrentInwardInvoiceCopy;
        //            }
        //        }
        //        else
        //        {
        //            CurrentInwardDrugMedDeptInvoice = CurrentInwardInvoiceCopy;
        //        }

        //    }
        //}

        //KMx: Copy và chỉnh sửa từ hàm gốc. Lý do: Thêm điều kiện nếu thay đổi NCC mà chưa bấm cập nhật thì ko cho nhập hàng, vì bị lỗi phiếu nhập NCC A, nhưng hàng của NCC B (24/02/2015 09:37).
        private bool IsChangedItem()
        {
            if (CurrentInwardDrugMedDeptInvoice == null || CurrentInwardDrugMedDeptInvoice.SelectedSupplier == null || CurrentInwardInvoiceCopy == null || CurrentInwardInvoiceCopy.SelectedSupplier == null)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0585_G1_PhNhapKgDuTTin), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID != CurrentInwardInvoiceCopy.SelectedSupplier.SupplierID)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0586_G1_NCCDaThayDoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (CurrentInwardDrugMedDeptInvoice.CurrencyID != CurrentInwardInvoiceCopy.CurrencyID || CurrentInwardDrugMedDeptInvoice.ExchangeRates != CurrentInwardInvoiceCopy.ExchangeRates || CurrentInwardDrugMedDeptInvoice.VAT != CurrentInwardInvoiceCopy.VAT || CurrentInwardDrugMedDeptInvoice.IsForeign != CurrentInwardInvoiceCopy.IsForeign || CurrentInwardDrugMedDeptInvoice.Notes != CurrentInwardInvoiceCopy.Notes)
            {
                if (MessageBox.Show(eHCMSResources.Z0587_G1_CoMuonCNhatHD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CheckValidate())
                    {
                        UpdateInwardInvoiceDrug();
                    }
                    else
                    {
                        CurrentInwardDrugMedDeptInvoice = CurrentInwardInvoiceCopy;
                    }
                }
                else
                {
                    CurrentInwardDrugMedDeptInvoice = CurrentInwardInvoiceCopy;
                }
            }

            return true;
        }

        private bool CheckValidationData()
        {
            bool results = true;
            if (DrugDeptPurchaseOrderDetailList == null || DrugDeptPurchaseOrderDetailList.Count == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0588_G1_KgCoDLieuNhap));
                return false;
            }

            if (DrugDeptPurchaseOrderDetailList.Count <= 0 || DrugDeptPurchaseOrderDetailList[0].GenMedProductID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0647_G1_Msg_InfoKhongCoHgDeNhap);
                return false;
            }

            string TB = "";
            //20200408 TBL: BM 0029102: Anh Tuân nói cho phép nhập 2 đợt thầu khác nhau cho 1 phiếu nhập
            //if (DrugDeptPurchaseOrderDetailList != null
            //    && DrugDeptPurchaseOrderDetailList.Count > 0
            //    && DrugDeptPurchaseOrderDetailList.Any(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.BidID > 0)
            //    && DrugDeptPurchaseOrderDetailList.Where(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.BidID > 0).Select(x => x.RefGenMedProductDetail.BidID).Distinct().Count() > 1)
            //{
            //    GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2740_G1_PhieuNhapCoChua2DThau);
            //    return false;
            //}
            StringBuilder sb = new StringBuilder();
            StringBuilder sbForCheckVAT = new StringBuilder();
            for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
            {
                if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail != null && DrugDeptPurchaseOrderDetailList[i].GenMedProductID > 0)
                {
                    if (DrugDeptPurchaseOrderDetailList[i].GenMedProductID == 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0785_G1_Msg_InfoPhaiChonThuoc));
                        return false;
                    }
                    else if (DrugDeptPurchaseOrderDetailList[i].InQuantity <= 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0789_G1_Msg_InfoSLgNhapLonHon0));
                        return false;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        if (string.IsNullOrWhiteSpace(DrugDeptPurchaseOrderDetailList[i].InBatchNumber))
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0784_G1_Msg_InfoChuaNhapSoLo));
                            return false;
                        }
                        else if (DrugDeptPurchaseOrderDetailList[i].InExpiryDate == null)
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0782_G1_Msg_InfoChuaNhapHSD));
                            return false;
                        }
                    }
                    //▼===== #004
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                    {
                        if (string.IsNullOrWhiteSpace(DrugDeptPurchaseOrderDetailList[i].InBatchNumber))
                        {
                            sb.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0784_G1_Msg_InfoChuaNhapSoLo));
                        }
                    }
                    //▲===== #004
                    //▼===== #010
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                    {
                        if (string.IsNullOrWhiteSpace(DrugDeptPurchaseOrderDetailList[i].InBatchNumber))
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0784_G1_Msg_InfoChuaNhapSoLo));
                            return false;
                        }
                        else if (DrugDeptPurchaseOrderDetailList[i].InExpiryDate == null)
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0782_G1_Msg_InfoChuaNhapHSD));
                            return false;
                        }
                    }
                    //▲===== #010
                    //▼===== #007
                    //else if (DrugDeptPurchaseOrderDetailList[i].InExpiryDate <= DateTime.Now)
                    //{
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0787_G1_Msg_InfoHSDLonHonNgHTai));
                    //    return false;
                    //}
                    ////else if (DrugDeptPurchaseOrderDetailList[i].InProductionDate == null)
                    ////{
                    ////    MessageBox.Show(string.Format("{0} ", eHCMSResources.Z0589_G1_LoiDongThu) + (i + 1).ToString() + string.Format(" {0}.", eHCMSResources.A0783_G1_Msg_InfoChuaNhapNgSX));
                    ////    return false;
                    ////}
                    //else if (DrugDeptPurchaseOrderDetailList[i].InProductionDate >= DateTime.Now)
                    //{
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0590_G1_NgSXPhaiNhoHonNgHTai));
                    //    return false;
                    //}
                    if (DrugDeptPurchaseOrderDetailList[i].InExpiryDate < DrugDeptPurchaseOrderDetailList[i].InProductionDate)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3004_G1_HSDNhoHonNSX));
                        return false;
                    }
                    //▲===== #007
                    if (DrugDeptPurchaseOrderDetailList[i].UnitPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0591_G1_DGiaKgDuocNhoHon0));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].UnitPrice == 0 && DrugDeptPurchaseOrderDetailList[i].V_GoodsType != null && DrugDeptPurchaseOrderDetailList[i].V_GoodsType.LookupID == (long)AllLookupValues.V_GoodsType.HANGMUA)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0556_G1_Msg_InfoGiaMuaLonHon0));
                        return false;
                    }
                    if ((DrugDeptPurchaseOrderDetailList[i].DiscountingByPercent < 1 && DrugDeptPurchaseOrderDetailList[i].DiscountingByPercent > 0) || (DrugDeptPurchaseOrderDetailList[i].DiscountingByPercent > 2))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0072_G1_CKKhHopLe));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].V_GoodsType == null || DrugDeptPurchaseOrderDetailList[i].V_GoodsType.LookupID <= 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0592_G1_ChonLoaiHang));
                        return false;
                    }
                    //KMx: Khoa Dược yêu cầu không có giá bán vẫn cho lưu (hàng tặng) (01/07/2016 10:27).
                    //tam thoi dong cai nay de nhap hang cai da
                    //if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.NormalPrice <= 0)
                    //{
                    //    MessageBox.Show(string.Format("{0} ", eHCMSResources.Z0589_G1_LoiDongThu) + (i + 1).ToString() + string.Format(" {0}!", eHCMSResources.A0786_G1_Msg_InfoCNhatBGiaBan));
                    //    return false;
                    //}
                    //▼===== #008
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.NormalPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3013_G1_GiaBanDVKhongDuocNhoHon0));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.HIAllowedPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3012_G1_GiaBHKhongDuocNhoHon0));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.HIPatientPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3011_G1_GiaBNBHKhongDuocNhoHon0));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].TotalPriceNotVAT < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3014_G1_ThanhTienKhongDuocNhoHon0));
                        return false;
                    }
                    //▲===== #008
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.NormalPrice < DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.HIPatientPrice)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.HIAllowedPrice > DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.HIPatientPrice)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0593_G1_GiaBHChoPhep));
                        return false;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].UnitPrice != DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.InBuyingPrice && DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        TB += DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.BrandName + Environment.NewLine;
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].VAT < 0
                            || DrugDeptPurchaseOrderDetailList[i].VAT > 1
                            || (!DrugDeptPurchaseOrderDetailList[i].IsNotVat && DrugDeptPurchaseOrderDetailList[i].VAT == null))
                    {
                        sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2991_G1_VATKhongHopLe));
                    }
                    else if (DrugDeptPurchaseOrderDetailList[i].IsNotVat
                        && !(DrugDeptPurchaseOrderDetailList[i].VAT == null || DrugDeptPurchaseOrderDetailList[i].VAT == 0))
                    {
                        sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2992_G1_CoVatKhongTinhThue));
                    }
                }
            }
            if (!string.IsNullOrEmpty(sbForCheckVAT.ToString()))
            {
                MessageBox.Show(sbForCheckVAT.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }
            if (!string.IsNullOrEmpty(TB))
            {
                if (MessageBox.Show(eHCMSResources.A0554_G1_Msg_InfoGiaMuaKhacTruoc + Environment.NewLine + TB + Environment.NewLine + eHCMSResources.I0920_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //▼===== #004
            if (!string.IsNullOrEmpty(sb.ToString()) && V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            {
                sb.AppendLine(eHCMSResources.I0941_G1_I);
                if (MessageBox.Show(sb.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //▲===== #004
 
            return results;
        }

        /*▼====: #001*/
        private bool WarningValidationDate()
        {
            bool results = true;
            string TB = "";
            for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
            {
                if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail != null && DrugDeptPurchaseOrderDetailList[i].GenMedProductID > 0)
                {
                    if (DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.NormalPrice <= 0)
                    {
                        TB += (string.Format("{0} ", eHCMSResources.Z0589_G1_LoiDongThu) + (i + 1).ToString() + string.Format(" {0}!", eHCMSResources.Z2819_G1_ChuaCoGiaBanDV));
                    }
                    if (DrugDeptPurchaseOrderDetailList[i].UnitPrice != DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.InBuyingPrice && DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        TB += DrugDeptPurchaseOrderDetailList[i].RefGenMedProductDetail.BrandName + Environment.NewLine;
                    }
                }
            }
            if (!string.IsNullOrEmpty(TB))
            {
                if (MessageBox.Show(TB + Environment.NewLine + eHCMSResources.I0920_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return results;
        }
        /*▲====: #001*/

        private void InwardDrug_InsertList()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginInwardDrugMedDept_InsertList(DrugDeptPurchaseOrderDetailList.ToList(), CurrentInwardDrugMedDeptInvoice.inviID, DrugDeptPoID
                            , V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                contract.EndInwardDrugMedDept_InsertList(asyncResult);
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0594_G1_NhapHangThCong), eHCMSResources.G0442_G1_TBao);
                                ClearDrugDeptPurchaseOrderDetailList();
                                PharmacyPurchaseOrder_BySupplierID();
                                if (!IsEnabled)
                                {
                                    AddBlankRow();
                                }
                                if (CurrentInwardDrugMedDeptInvoice != null)
                                {
                                    Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID, null);
                                    InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }

        public void btnNhapHang(object sender, RoutedEventArgs e)
        {
            if (!IsChangedItem())
            {
                return;
            }

            if (CheckValidationData())
            {
                if (WarningValidationDate())
                {
                    InwardDrug_InsertList();
                }
            }
        }

        CheckBox ckbDH = null;
        public void ckbDH_Loaded(object sender, RoutedEventArgs e)
        {
            ckbDH = sender as CheckBox;
        }

        public void ckbDH_Unchecked(object sender, RoutedEventArgs e)
        {
            IsEnabled = true;
            ReadOnlyColumnTrue();

            //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
            CheckEnableSupplier();
        }

        private bool CheckBlankRow()
        {
            if (DrugDeptPurchaseOrderDetailList != null && DrugDeptPurchaseOrderDetailList.Count > 0)
            {
                for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
                {
                    if (DrugDeptPurchaseOrderDetailList[i].GenMedProductID == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void AddBlankRow()
        {
            if (CheckBlankRow())
            {
                if (DrugDeptPurchaseOrderDetailList == null)
                {
                    DrugDeptPurchaseOrderDetailList = new ObservableCollection<DrugDeptPurchaseOrderDetail>();
                }
                DrugDeptPurchaseOrderDetail p = new DrugDeptPurchaseOrderDetail();
                Lookup item = new Lookup
                {
                    LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA,
                    ObjectValue = eHCMSResources.Z0595_G1_HangMua
                };
                p.V_GoodsType = item;
                p.IsPercent = IsPercentInwardDrug;
                //Không mặc định VAT từ Invoice cha mà mặc định từ danh mục
                //if (CurrentInwardDrugMedDeptInvoice != null)
                //{
                //    //20191026 TTM: lý do - 1: vì trong Inward details chỉ lấy giá trị tăng thêm (từ 0 -> 1) mà thôi.
                //    p.VAT = CurrentInwardDrugMedDeptInvoice.VAT - 1;
                //}
                DrugDeptPurchaseOrderDetailList.Add(p);
            }
        }

        ComboBox cbxChonDH = null;
        public void cbxChonDH_Loaded(object sender, RoutedEventArgs e)
        {
            cbxChonDH = sender as ComboBox;
        }

        public void ckbDH_Checked(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            if (cbxChonDH != null)
            {
                cbxChonDH.SelectedItem = null;
            }
            ReadOnlyColumnFalse();
            AddBlankRow();

            //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
            CheckEnableSupplier();
        }

        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
            //if (GridSuppliers != null)
            //{
            //    //GridSuppliers.CurrentCellChanged += GridSuppliers_CurrentCellChanged;
            //}
        }

        private void ReadOnlyColumnFalse()
        {
            if (GridSuppliers != null)
            {
                //KMx: Không lấy cột theo index mà lấy theo tên, để sau này có thêm cột thì chức năng không bị thay đổi (26/06/2015 11:18).
                //GridSuppliers.Columns[(int)DataGridCol.TenThuoc].IsReadOnly = false;
                //GridSuppliers.Columns[(int)DataGridCol.TenThuoc].CellStyle = null;
                //GridSuppliers.Columns[(int)DataGridCol.Code].IsReadOnly = false;
                //GridSuppliers.Columns[(int)DataGridCol.Code].CellStyle = null;
                var colBrandName = GridSuppliers.GetColumnByName("colBrandName");
                var colCode = GridSuppliers.GetColumnByName("colCode");
                if (colBrandName != null)
                {
                    colBrandName.IsReadOnly = false;
                    colBrandName.CellStyle = null;
                }
                if (colCode != null)
                {
                    colCode.IsReadOnly = false;
                    colCode.CellStyle = null;
                }
            }
        }

        private void ReadOnlyColumnTrue()
        {
            if (GridSuppliers != null)
            {
                //KMx: Không lấy cột theo index mà lấy theo tên, để sau này có thêm cột thì chức năng không bị thay đổi (26/06/2015 11:18).
                //GridSuppliers.Columns[(int)DataGridCol.TenThuoc].IsReadOnly = true;
                //GridSuppliers.Columns[(int)DataGridCol.TenThuoc].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];
                //GridSuppliers.Columns[(int)DataGridCol.Code].IsReadOnly = true;
                //GridSuppliers.Columns[(int)DataGridCol.Code].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];

                var colBrandName = GridSuppliers.GetColumnByName("colBrandName");
                var colCode = GridSuppliers.GetColumnByName("colCode");
                if (colBrandName != null)
                {
                    colBrandName.IsReadOnly = true;
                    colBrandName.CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];
                }
                if (colCode != null)
                {
                    colCode.IsReadOnly = true;
                    colCode.CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];
                }
            }
        }

        #region Auto For Location

        private void SetValueSdlDescription(string Name, object item)
        {
            DrugDeptPurchaseOrderDetail P = item as DrugDeptPurchaseOrderDetail;
            P.SdlDescription = Name;
        }

        AutoCompleteBox Au;

        private void SearchRefShelfLocation(string Name)
        {
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefShelfDrugLocationAutoComplete(Name, 0, Globals.PageSize, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetRefShelfDrugLocationAutoComplete(asyncResult);
                                Au.ItemsSource = results.ToObservableCollection();
                                Au.PopulateComplete();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void AutoLocation_Text_Populating(object sender, PopulatingEventArgs e)
        {
            Au = sender as AutoCompleteBox;
            SearchRefShelfLocation(e.Parameter);
            if (GridSuppliers != null && GridSuppliers.SelectedItem != null)
            {
                SetValueSdlDescription(e.Parameter, GridSuppliers.SelectedItem);
            }
        }

        public void AutoLocation_Tex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Au != null && CurrentDrugDeptPurchaseOrderDetail != null)
            {
                if (Au.SelectedItem != null)
                {
                    CurrentDrugDeptPurchaseOrderDetail.SelectedShelfDrugLocation = Au.SelectedItem as RefShelfDrugLocation;
                }
                else
                {
                    CurrentDrugDeptPurchaseOrderDetail.SdlDescription = Au.Text;
                }
            }
        }

        #endregion

        #region Auto for Drug Member

        private string BrandName;

        private PagedSortableCollectionView<RefGenMedProductDetails> _CurrentRefGenMedProductDetails;
        public PagedSortableCollectionView<RefGenMedProductDetails> CurrentRefGenMedProductDetails
        {
            get
            {
                return _CurrentRefGenMedProductDetails;
            }
            set
            {
                if (_CurrentRefGenMedProductDetails != value)
                {
                    _CurrentRefGenMedProductDetails = value;
                }
                NotifyOfPropertyChange(() => CurrentRefGenMedProductDetails);
            }
        }

        //KMx: Trường hợp nhập thuốc A, số lượng quy cách là 50. Sau đó đổi thuốc A thành thuốc B, số lượng quy cách là 28.
        //Nhưng thuốc B vẫn lấy tính theo số lượng quy cách của thuốc A => Tính ra số lượng lẻ và giá vốn bị sai (18/12/2014 15:58).
        private void ResetCurrentDrugDeptPurchaseOrderDetail()
        {
            if (CurrentDrugDeptPurchaseOrderDetail != null)
            {
                CurrentDrugDeptPurchaseOrderDetail.InProductionDate = null;
                CurrentDrugDeptPurchaseOrderDetail.InBatchNumber = null;
                CurrentDrugDeptPurchaseOrderDetail.InExpiryDate = null;
                CurrentDrugDeptPurchaseOrderDetail.PackageQuantity = 0;
                CurrentDrugDeptPurchaseOrderDetail.InQuantity = 0;
                Lookup item = new Lookup
                {
                    LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA,
                    ObjectValue = eHCMSResources.Z0595_G1_HangMua
                };
                CurrentDrugDeptPurchaseOrderDetail.V_GoodsType = item;
                CurrentDrugDeptPurchaseOrderDetail.IsPercent = IsPercentInwardDrug;
                CurrentDrugDeptPurchaseOrderDetail.IsUnitPackage = false;
                CurrentDrugDeptPurchaseOrderDetail.PackagePrice = 0;
                CurrentDrugDeptPurchaseOrderDetail.UnitPrice = 0;
                CurrentDrugDeptPurchaseOrderDetail.TotalPriceNotVAT = 0;
                CurrentDrugDeptPurchaseOrderDetail.SdlDescription = null;
                CurrentDrugDeptPurchaseOrderDetail.DiscountingByPercent = 0;
                CurrentDrugDeptPurchaseOrderDetail.Discounting = 0;
                CurrentDrugDeptPurchaseOrderDetail.NoPrint = false;
                CurrentDrugDeptPurchaseOrderDetail.SdlID = null;
                CurrentDrugDeptPurchaseOrderDetail.SelectedShelfDrugLocation = null;
            }
        }

        /// <summary>
        /// VuTTM
        /// </summary>
        /// <param name="drugDeptPurchaseOrderDetail"></param>
        private void GetDrugDeptSellingItemPriceDetails(DrugDeptPurchaseOrderDetail drugDeptPurchaseOrderDetail)
        {
            if (null == drugDeptPurchaseOrderDetail
                || 0 == drugDeptPurchaseOrderDetail.RefGenMedProductDetail.GenMedProductID)
            {
                return;
            }
            var thread = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugDeptSellingItemPriceDetails(
                            drugDeptPurchaseOrderDetail.RefGenMedProductDetail.GenMedProductID,
                            Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var _Result = contract.EndGetDrugDeptSellingItemPriceDetails(asyncResult);
                                if (null != _Result)
                                {
                                    drugDeptPurchaseOrderDetail.RefGenMedProductDetail.HIAllowedPrice = _Result.HIAllowedPrice;
                                }
                            }
                            catch (Exception _Ex)
                            {
                                Globals.ShowMessage(_Ex.Message, "Không thể lấy thông tin giá.");
                            }
                        }), null);
                    }
                }
                catch (Exception _Ex)
                {
                    Globals.ShowMessage(_Ex.Message, "Không thể lấy thông tin giá.");
                }
            });
            thread.Start();
        }

        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            long? SupplierID = 0;
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.SelectedSupplier != null)
            {
                SupplierID = CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID;
            }
            int totalCount = 0;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefGenMedProductDetails_SearchAutoPaging(IsCode, Name, SupplierID, V_MedProductType, null, PageSize, PageIndex, SelectedBidID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ListUnits = contract.EndRefGenMedProductDetails_SearchAutoPaging(out totalCount, asyncResult);
                                if (IsCode.GetValueOrDefault())
                                {
                                    if (ListUnits != null && ListUnits.Count > 0)
                                    {
                                        if (CurrentDrugDeptPurchaseOrderDetail == null)
                                        {
                                            CurrentDrugDeptPurchaseOrderDetail = new DrugDeptPurchaseOrderDetail();
                                        }
                                        else
                                        {
                                            ResetCurrentDrugDeptPurchaseOrderDetail();
                                        }
                                        CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail = ListUnits.FirstOrDefault();
                                        CurrentDrugDeptPurchaseOrderDetail.UnitPrice = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.InBuyingPrice;
                                        CurrentDrugDeptPurchaseOrderDetail.PackagePrice = CurrentDrugDeptPurchaseOrderDetail.UnitPrice * CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault();
                                        CurrentDrugDeptPurchaseOrderDetail.Code = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail != null ? CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Code : "";
                                        CurrentDrugDeptPurchaseOrderDetail.VAT = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT != null ? CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT : null;
                                        CurrentDrugDeptPurchaseOrderDetail.IsNotVat = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.IsNotVat;
                                        CurrentDrugDeptPurchaseOrderDetail.PriceIncludeVAT = CurrentDrugDeptPurchaseOrderDetail.UnitPrice
                                            * (1 + (decimal)(CurrentDrugDeptPurchaseOrderDetail.VAT == null ? 0 : CurrentDrugDeptPurchaseOrderDetail.VAT));
                                        SetPriceByPriceScale(CurrentDrugDeptPurchaseOrderDetail);
                                    }
                                    else
                                    {
                                        MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                    }

                                    //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
                                    CheckEnableSupplier();
                                }
                                else
                                {
                                    if (ListUnits != null)
                                    {
                                        CurrentRefGenMedProductDetails.Clear();
                                        CurrentRefGenMedProductDetails.TotalItemCount = totalCount;
                                        CurrentRefGenMedProductDetails.ItemCount = totalCount;
                                        foreach (RefGenMedProductDetails p in ListUnits)
                                        {
                                            CurrentRefGenMedProductDetails.Add(p);
                                         }
                                        NotifyOfPropertyChange(() => CurrentRefGenMedProductDetails);
                                    }
                                    AuDrug.ItemsSource = CurrentRefGenMedProductDetails;
                                    AuDrug.PopulateComplete();
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        AutoCompleteBox AuDrug;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            BrandName = e.Parameter;
            CurrentRefGenMedProductDetails.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(null, e.Parameter, 0, CurrentRefGenMedProductDetails.PageSize);
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            //(sender as AutoCompleteBox).ItemsSource = CurrentRefGenMedProductDetails;
            AuDrug = sender as AutoCompleteBox;
        }

        public void acbDrug_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (AuDrug != null)
            {
                if (CurrentDrugDeptPurchaseOrderDetail != null)
                {
                    ResetCurrentDrugDeptPurchaseOrderDetail();
                    CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductDetails;
                    if (EditedDetailItem == null)
                    {
                        EditedDetailItem = new DrugDeptPurchaseOrderDetail();
                    }
                    if (CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail != null)
                    {
                        CurrentDrugDeptPurchaseOrderDetail.UnitPrice = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.InBuyingPrice;
                        CurrentDrugDeptPurchaseOrderDetail.PackagePrice = CurrentDrugDeptPurchaseOrderDetail.UnitPrice * CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault();
                        CurrentDrugDeptPurchaseOrderDetail.Code = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Code;
                        CurrentDrugDeptPurchaseOrderDetail.VAT = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT;
                        CurrentDrugDeptPurchaseOrderDetail.IsNotVat = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.IsNotVat;
                    }
                    else
                    {
                        CurrentDrugDeptPurchaseOrderDetail.Code = "";
                    }
                }
            }
        }

        #endregion

        #region Auto for Supplier
        private SupplierSearchCriteria _SupplierCriteria;
        public SupplierSearchCriteria SupplierCriteria
        {
            get
            {
                return _SupplierCriteria;
            }
            set
            {
                _SupplierCriteria = value;
                NotifyOfPropertyChange(() => SupplierCriteria);
            }
        }

        private PagedSortableCollectionView<DrugDeptSupplier> _Suppliers;
        public PagedSortableCollectionView<DrugDeptSupplier> Suppliers
        {
            get
            {
                return _Suppliers;
            }
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                }
                NotifyOfPropertyChange(() => Suppliers);
            }
        }

        private void SearchSupplierAuto(int PageIndex, int PageSize)
        {
            int totalCount = 0;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptSupplier_SearchAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var ListUnits = contract.EndDrugDeptSupplier_SearchAutoPaging(out totalCount, asyncResult);
                                // Suppliers = ListUnits.ToObservableCollection();
                                if (ListUnits != null)
                                {
                                    Suppliers.Clear();
                                    Suppliers.TotalItemCount = totalCount;
                                    foreach (DrugDeptSupplier p in ListUnits)
                                    {
                                        Suppliers.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => Suppliers);
                                }
                                auSupplier.ItemsSource = Suppliers;
                                auSupplier.PopulateComplete();
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        AxAutoComplete auSupplier;
        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            auSupplier = sender as AxAutoComplete;
            SupplierCriteria.SupplierName = e.Parameter;
            Suppliers.PageIndex = 0;
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (CurrentInwardDrugMedDeptInvoice != null)
            {
                PharmacyPurchaseOrder_BySupplierID();
            }
        }

        #endregion

        private void DeleteInwardDrug(long InID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginDeleteInwardDrugMedDept(InID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                int results = contract.EndDeleteInwardDrugMedDept(asyncResult);
                                if (results == 0)
                                {
                                    Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID, null);
                                    InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
                                }
                                else if (results == 1)
                                {
                                    MessageBox.Show(eHCMSResources.K0055_G1_ThuocKhongTheXoa, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("{0}.", eHCMSResources.K0058_G1_ThuocKhongTonTai), eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void lnkDeleteMain_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugMedDeptInvoice.CanEditAndDelete)
            {
                if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CurrentInwardDrugMedDept != null)
                    {
                        DeleteInwardDrug(CurrentInwardDrugMedDept.InID);
                    }
                }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0596_G1_ThuocDaKChuyenTDK, eHCMSResources.G0442_G1_TBao);
            }
        }

        private void CopyCurrentInwardDrugMedDept()
        {
            CurrentInwardDrugMedDeptCopy = CurrentInwardDrugMedDept.DeepCopy();
        }

        public void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugMedDeptInvoice.DrugDeptSupplierPaymentReqID != null && CurrentInwardDrugMedDeptInvoice.DrugDeptSupplierPaymentReqID > 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0597_G1_PhNhapDaTToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }

            if (!IsChangedItem())
            {
                return;
            }

            CopyCurrentInwardDrugMedDept();
            void onInitDlg(IDrugDeptEditInwardDrug proAlloc)
            {
                if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency != null && CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
                {
                    proAlloc.IsVND = false;
                }
                else
                {
                    proAlloc.IsVND = true;
                }
                proAlloc.CbxGoodsTypes = CbxGoodsTypes.DeepCopy();
                proAlloc.CurrentInwardDrugMedDeptCopy = CurrentInwardDrugMedDept.DeepCopy();
                proAlloc.SetValueForProperty();
                //▼====== #010
                proAlloc.V_MedProductType = V_MedProductType;
                //▲====== #010
                if (CurrentInwardDrugMedDeptInvoice.CheckedPoint)
                {
                    proAlloc.CurrentInwardDrugMedDeptCopy.IsCanEdit = false;
                    MessageBox.Show(eHCMSResources.K0047_G1_ThuocDaKCTonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            }
            GlobalsNAV.ShowDialog<IDrugDeptEditInwardDrug>(onInitDlg);
        }

        private void ReadOnlyColumnValueFalse()
        {
            if (GridSuppliers != null)
            {
                //GridSuppliers.Columns[(int)DataGridCol.CKPercent].IsReadOnly = false;
                //GridSuppliers.Columns[(int)DataGridCol.CKPercent].CellStyle = null;
                //GridSuppliers.Columns[(int)DataGridCol.CK].IsReadOnly = true;
                //GridSuppliers.Columns[(int)DataGridCol.CK].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyRight"];

                var colDiscountPercent = GridSuppliers.GetColumnByName("colDiscountPercent");
                var colDiscount = GridSuppliers.GetColumnByName("colDiscount");
                if (colDiscountPercent != null)
                {
                    colDiscountPercent.IsReadOnly = false;
                    colDiscountPercent.CellStyle = null;
                }
                if (colDiscount != null)
                {
                    colDiscount.IsReadOnly = true;
                    colDiscount.CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyRight"];
                }
            }
        }

        private void ReadOnlyColumnValueTrue()
        {
            if (GridSuppliers != null)
            {
                //GridSuppliers.Columns[(int)DataGridCol.CKPercent].IsReadOnly = true;
                //GridSuppliers.Columns[(int)DataGridCol.CKPercent].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyRight"];
                //GridSuppliers.Columns[(int)DataGridCol.CK].IsReadOnly = false;
                //GridSuppliers.Columns[(int)DataGridCol.CK].CellStyle = null;

                var colDiscountPercent = GridSuppliers.GetColumnByName("colDiscountPercent");
                var colDiscount = GridSuppliers.GetColumnByName("colDiscount");
                if (colDiscountPercent != null)
                {
                    colDiscountPercent.IsReadOnly = true;
                    colDiscountPercent.CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyRight"];
                }
                if (colDiscount != null)
                {
                    colDiscount.IsReadOnly = false;
                    colDiscount.CellStyle = null;
                }
            }
        }

        public void ckbCKHangNhap_Unchecked(object sender, RoutedEventArgs e)
        {
            ReadOnlyColumnValueTrue();
        }

        public void ckbCKHangNhap_Checked(object sender, RoutedEventArgs e)
        {
            ReadOnlyColumnValueFalse();
        }
        string value = "";
        public void grdRequestDetails_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            //AxTextBox tbl = GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) as AxTextBox;
            //if (tbl != null)
            //{
            //    value = tbl.Text;
            //}
            var mAxTextBox = e.EditingElement.GetChildrenByType<AxTextBox>().FirstOrDefault();
            if (mAxTextBox != null)
            {
                value = mAxTextBox.Text;
            }
        }

        //KMx: Hiện tại, không biết chặn AutoComplete khi user typing, nên làm đỡ cách này, cách này có 1 khuyết điểm là phải để nhiều chổ gọi khi thực hiện xong 1 chức năng.
        //Khi nào có thời gian thì sửa lại cách này (13/01/2015 12:01).
        private void CheckEnableSupplier()
        {
            if ((DrugDeptPurchaseOrderDetailList != null && DrugDeptPurchaseOrderDetailList.Count > 0
                && DrugDeptPurchaseOrderDetailList.Any(x => x.RefGenMedProductDetail != null))
                || (InwardDrugMedDeptList != null && InwardDrugMedDeptList.Count > 0))
            {
                IsHideAuSupplier = false;
            }
            else
            {
                IsHideAuSupplier = true;
            }
        }

        //▼====== #001 TTM: 
        //Thay đổi cách thức lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colCode") => dùng e.Column.Equals(GridSuppliers.GetColumnByName("colCode")).
        //public void grdRequestDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    DrugDeptPurchaseOrderDetail selItem = (DrugDeptPurchaseOrderDetail)e.Row.Item;

        //    EditedDetailItem = (e.Row.DataContext as DrugDeptPurchaseOrderDetail);
        //    EditedColumnName = null;
        //    if (e.Column.Equals(GridSuppliers.GetColumnByName("colCode")))
        //    {
        //        DrugDeptPurchaseOrderDetail item = e.Row.DataContext as DrugDeptPurchaseOrderDetail;
        //        if (item != null && !string.IsNullOrEmpty(item.Code) && value != item.Code)
        //        {
        //            string Code = Globals.FormatCode(V_MedProductType, item.Code);
        //            SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, 1);
        //        }
        //    }
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
        //    {
        //        EditedColumnName = "colUnitPrice";
        //    }
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
        //    {
        //        EditedColumnName = "colPackagePrice";
        //    }
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagingQty")))
        //    {
        //        EditedColumnName = "colPackagingQty";
        //    }
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colQty")))
        //    {
        //        EditedColumnName = "colQty";
        //    }
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colTotalNotVAT")))
        //    {
        //        EditedColumnName = "colTotalNotVAT";
        //    }
        //    //▼===== #006
        //    else if (e.Column.Equals(GridSuppliers.GetColumnByName("colDiscount")))
        //    {
        //        EditedColumnName = "colDiscount";
        //    }
        //    //▲===== #006
        //    if (!IsEnabled)
        //    {
        //        if (e.Row.GetIndex() == (DrugDeptPurchaseOrderDetailList.Count - 1) && e.EditAction == DataGridEditAction.Commit)
        //        {
        //            AddBlankRow();
        //        }
        //    }
        //}

        private void UpdatePriceIncludeVAT(DrugDeptPurchaseOrderDetail item)
        {
            item.VAT = (item.VAT == null || item.IsNotVat) ? 0 : item.VAT;
            item.PriceIncludeVAT = item.UnitPrice * (1 + (decimal)item.VAT);
        }

        public void grdRequestDetails_CellEditEndingNew(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = (DrugDeptPurchaseOrderDetail)e.Row.Item;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colCode")))
            {
                DrugDeptPurchaseOrderDetail item = e.Row.DataContext as DrugDeptPurchaseOrderDetail;
                if (item != null && !string.IsNullOrEmpty(item.Code) && value != item.Code)
                {
                    string Code = Globals.FormatCode(V_MedProductType, item.Code);
                    SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, 1);
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colVAT")))
            {
                double.TryParse(value, out double ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.VAT != ite)
                {
                    if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                    {
                        UpdatePriceIncludeVAT(item);
                        SetPriceByPriceScale(item);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colQty")))
            {
                double.TryParse(value, out double ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.InQuantity != ite)
                {
                    item.PackageQuantity = item.InQuantity / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                    if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                    {
                        SetPriceByPriceScale(item);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.UnitPrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    item.PackagePrice = item.UnitPrice * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //▼===== #005
                    if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                    {
                        UpdatePriceIncludeVAT(item);
                        SetPriceByPriceScale(item);
                    }
                    //▲===== #005
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPriceIncludeTax")))
            {
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PriceIncludeVAT != ite)
                {
                    item.VAT = (item.VAT == null || item.IsNotVat) ? 0 : item.VAT;
                    item.UnitPrice = item.PriceIncludeVAT / (1 + (decimal)item.VAT);
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    item.PackagePrice = item.UnitPrice * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                    {
                        SetPriceByPriceScale(item);
                    }
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PackagePrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.PackageQuantity * item.PackagePrice;
                    }
                    item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //▼===== #005
                    if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                    {
                        SetPriceByPriceScale(item);
                    }
                    //▲===== #005
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagingQty")))
            {
                double.TryParse(value, out double ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PackageQuantity != ite)
                {
                    item.InQuantity = item.PackageQuantity * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                    if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                    {
                        SetPriceByPriceScale(item);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colTotalNotVAT")))
            {
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.TotalPriceNotVAT != ite)
                {
                    if (item.InQuantity > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                    }
                    if (item.PackageQuantity > 0)
                    {
                        if (item.PackagePrice == 0)
                        {
                            item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                        }
                        if (item.UnitPrice == 0)
                        {
                            item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                        }
                        //▼===== #005
                        if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
                        {
                            SetPriceByPriceScale(item);
                        }
                        //▲===== #005
                    }
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            //▼===== #006
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colDiscount")))
            {
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null)
                {
                    if (item.Discounting > 0 && item.TotalPriceNotVAT > 0)
                    {
                        item.DiscountingByPercent = (1 + (item.Discounting / item.TotalPriceNotVAT));
                    }
                    else
                    {
                        item.DiscountingByPercent = 1;
                    }
                }
            }
            
            //▲===== #006
            if (!IsEnabled)
            {
                if (e.Row.GetIndex() == (DrugDeptPurchaseOrderDetailList.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                {
                    //▼===== #011
                    CheckEnableSupplier();
                    //▲===== #011
                    AddBlankRow();
                }
            }
        }

        private DrugDeptPurchaseOrderDetail EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        //private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        //{
        //    if (EditedDetailItem == null || string.IsNullOrEmpty(EditedColumnName))
        //    {
        //        return;
        //    }
        //    if (EditedColumnName.Equals("colUnitPrice"))
        //    {
        //        EditedColumnName = null;
        //        decimal.TryParse(value, out decimal ite);
        //        DrugDeptPurchaseOrderDetail item = EditedDetailItem;
        //        if (item != null && item.RefGenMedProductDetail != null && item.UnitPrice != ite)
        //        {
        //            if (item.TotalPriceNotVAT == 0)
        //            {
        //                item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
        //            }
        //            item.PackagePrice = item.UnitPrice * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
        //            //▼===== #005
        //            if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
        //            {
        //                SetPriceByPriceScale(item);
        //            }
        //            //▲===== #005
        //            if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
        //            {
        //                Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
        //            }
        //        }
        //    }
        //    else if (EditedColumnName.Equals("colPackagePrice"))
        //    {
        //        EditedColumnName = null;
        //        decimal.TryParse(value, out decimal ite);
        //        DrugDeptPurchaseOrderDetail item = EditedDetailItem;
        //        if (item != null && item.RefGenMedProductDetail != null && item.PackagePrice != ite)
        //        {
        //            if (item.TotalPriceNotVAT == 0)
        //            {
        //                item.TotalPriceNotVAT = (decimal)item.PackageQuantity * item.PackagePrice;
        //            }
        //            item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
        //            //▼===== #005
        //            if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
        //            {
        //                SetPriceByPriceScale(item);
        //            }
        //            //▲===== #005
        //            if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
        //            {
        //                Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
        //            }
        //        }
        //    }
        //    else if (EditedColumnName.Equals("colPackagingQty"))
        //    {
        //        EditedColumnName = null;
        //        double.TryParse(value, out double ite);
        //        DrugDeptPurchaseOrderDetail item = EditedDetailItem;
        //        if (item != null && item.RefGenMedProductDetail != null && item.PackageQuantity != ite)
        //        {
        //            item.InQuantity = item.PackageQuantity * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
        //            if (item.PackagePrice > 0)
        //            {
        //                item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
        //            }
        //            else if (item.TotalPriceNotVAT > 0)
        //            {
        //                item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
        //                item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
        //            }
        //        }
        //    }
        //    else if (EditedColumnName.Equals("colQty"))
        //    {
        //        EditedColumnName = null;
        //        double.TryParse(value, out double ite);
        //        DrugDeptPurchaseOrderDetail item = EditedDetailItem;
        //        if (item != null && item.RefGenMedProductDetail != null && item.InQuantity != ite)
        //        {
        //            item.PackageQuantity = item.InQuantity / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
        //            if (item.PackagePrice > 0)
        //            {
        //                item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
        //            }
        //            else if (item.TotalPriceNotVAT > 0)
        //            {
        //                item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
        //                item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
        //            }
        //        }
        //    }
        //    else if (EditedColumnName.Equals("colTotalNotVAT"))
        //    {
        //        EditedColumnName = null;
        //        decimal.TryParse(value, out decimal ite);
        //        DrugDeptPurchaseOrderDetail item = EditedDetailItem;
        //        if (item != null && item.RefGenMedProductDetail != null && item.TotalPriceNotVAT != ite)
        //        {
        //            if (item.PackageQuantity > 0)
        //            {
        //                if (item.PackagePrice == 0)
        //                {
        //                    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
        //                }
        //                if (item.UnitPrice == 0)
        //                {
        //                    item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
        //                }
        //                //▼===== #005
        //                if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
        //                {
        //                    SetPriceByPriceScale(item);
        //                }
        //                //▲===== #005
        //            }
        //            if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
        //            {
        //                Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
        //            }
        //        }
        //    }
        //    //▼===== #006
        //    else if (EditedColumnName.Equals("colDiscount"))
        //    {
        //        EditedColumnName = null;
        //        DrugDeptPurchaseOrderDetail item = EditedDetailItem;
        //        if (item != null)
        //        {
        //            if (item.Discounting > 0 && item.TotalPriceNotVAT > 0)
        //            {
        //                item.DiscountingByPercent = (1 + (item.Discounting / item.TotalPriceNotVAT));
        //            }
        //            else
        //            {
        //                item.DiscountingByPercent = 1;
        //            }
        //        }
        //    }
        //    //▲===== #006
        //    //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
        //    CheckEnableSupplier();
        //    EditedColumnName = null;
        //}
        //▲====== #001

        private void InnitSearchCriteria()
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new InwardInvoiceSearchCriteria();
            }
            SearchCriteria.InvoiceNumber = "";
            SearchCriteria.InwardID = "";
            SearchCriteria.FromDate = null;
            SearchCriteria.ToDate = null;
            SearchCriteria.DateInvoice = null;
            SearchCriteria.SupplierID = 0;
        }

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugMedDeptInvoice> results, int Totalcount)
        {
            //mo pop up tim
            void onInitDlg(IDrugDeptInwardDrugSupplierSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.TypID = TypID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (IsRetundView)
                {
                    proAlloc.strHienThi = eHCMSResources.Z2601_G1_XuatTraNCC.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0618_G1_TimHDonNhapThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0619_G1_TimHDonNhapYCu.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0619_G1_TimHDonNhapYCu.ToUpper();
                }
                else
                {
                    proAlloc.strHienThi = eHCMSResources.Z0620_G1_TimHDonNhapHChat.ToUpper();
                }
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
                proAlloc.InwardInvoiceList.PageSize = 20;
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugMedDeptInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            }
            GlobalsNAV.ShowDialog<IDrugDeptInwardDrugSupplierSearch>(onInitDlg);
        }

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize, OutwardDrugMedDeptInvoice aReturnOutInv)
        {
            if (SearchCriteria == null)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);

            if (string.IsNullOrEmpty(SearchCriteria.InwardID) && string.IsNullOrEmpty(SearchCriteria.InvoiceNumber))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardDrugMedDeptInvoice(SearchCriteria, TypID, V_MedProductType, PageIndex, PageSize, true, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Totalcount;
                                var results = contract.EndSearchInwardDrugMedDeptInvoice(out Totalcount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                    }
                                    else
                                    {
                                        CurrentInwardDrugMedDeptInvoice = results.FirstOrDefault();
                                        InitDetailInward();
                                        DeepCopyInvoice();
                                        LoadInfoThenSelectedInvoice(aReturnOutInv);
                                        if (aReturnOutInv != null && CurrentInwardDrugMedDeptInvoice != null)
                                        {
                                            CurrentInwardDrugMedDeptInvoice.ReturnInvInvoiceNumber = aReturnOutInv.InvInvoiceNumber;
                                            CurrentInwardDrugMedDeptInvoice.ReturnSerialNumber = aReturnOutInv.SerialNumber;
                                            CurrentInwardDrugMedDeptInvoice.ReturnInvoiceForm = aReturnOutInv.InvoiceForm;
                                            CurrentInwardDrugMedDeptInvoice.ReturnVAT = aReturnOutInv.VAT;
                                            CurrentInwardDrugMedDeptInvoice.ReturnNote = aReturnOutInv.Notes;
                                            CurrentInwardDrugMedDeptInvoice.ReturnoutiID = aReturnOutInv.outiID;
                                            CurrentInwardDrugMedDeptInvoice.OutInvID = aReturnOutInv.OutInvID;
                                        }
                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                        SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                        OpenPopUpSearchInwardInvoice(null, 0);
                                    }
                                }
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        public void tbx_Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InvoiceNumber = (sender as TextBox).Text;
                }
                SearchInwardInvoiceDrug(0, 20, null);
            }
        }

        public void tbx_Search_KeyUpPN(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InwardID = (sender as TextBox).Text;
                }
                SearchInwardInvoiceDrug(0, 20, null);
            }
        }

        public void btnSearch()
        {
            SearchInwardInvoiceDrug(0, 20, null);
        }

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                PharmacyPurchaseOrder_BySupplierID();
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseEditInwardEvent> Members

        public void Handle(DrugDeptCloseEditInwardEvent message)
        {
            if (IsActive)
            {
                Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID, null);
                InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
            }
        }
        public void Handle(DrugDeptCloseSearchOutMedDeptInvoiceEvent message)
        {
            if (message == null || message.SelectedOutMedDeptInvoice == null || !(message.SelectedOutMedDeptInvoice is OutwardDrugMedDeptInvoice) || (message.SelectedOutMedDeptInvoice as OutwardDrugMedDeptInvoice).ReturninviID == 0)
            {
                return;
            }
            OutwardDrugMedDeptInvoice mOutwardDrugMedDeptInvoice = (message.SelectedOutMedDeptInvoice as OutwardDrugMedDeptInvoice);
            LoadReturnInvoice(mOutwardDrugMedDeptInvoice);
        }
        #endregion

        #region printing member

        public void btnPreview()
        {
            void onInitDlg(IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentInwardInvoiceCopy.inviID;
                proAlloc.V_MedProductType = V_MedProductType;
                //▼====== #010
                if (CurrentInwardInvoiceCopy.IsForeign)
                {
                    if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                    {
                        proAlloc.eItem = ReportName.DRUGDEPT_INWARD_HOA_CHAT_SUPPLIER;
                    }
                    else proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER;
                }
                else
                {
                    if (V_MedProductType == (long)AllLookupValues.MedProductType.HOA_CHAT)
                    {
                        proAlloc.eItem = ReportName.DRUGDEPT_INWARD_HOA_CHAT_SUPPLIER_TRONGNUOC;
                    }
                    else proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER_TRONGNUOC;
                }
                //▲====== #010

                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0569_G1_PhNhapKhoThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0570_G1_PhNhapKhoYCu.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3208_G1_PhNhapKhoDDuong.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z0571_G1_PhNhapKhoHChat.ToUpper();
                }
                if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.SelectedStaff != null)
                {
                    proAlloc.StaffFullName = CurrentInwardDrugMedDeptInvoice.SelectedStaff.FullName;
                }
            }
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInWardDrugSupplierInPdfFormat(CurrentInwardDrugMedDeptInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInWardDrugSupplierInPdfFormat(asyncResult);
                                var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                                Globals.EventAggregator.Publish(results);
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
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchInwardIncoiceEvent> Members

        public void Handle(DrugDeptCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentInwardDrugMedDeptInvoice = message.SelectedInwardInvoice as InwardDrugMedDeptInvoice;
                InitDetailInward();
                DeepCopyInvoice();
                LoadInfoThenSelectedInvoice(null);
            }
        }

        private void LoadInfoThenSelectedInvoice(OutwardDrugMedDeptInvoice aReturnOutInv)
        {
            //IsHideAuSupplier = false;
            InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
            Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID, aReturnOutInv);
            PharmacyPurchaseOrder_BySupplierID();
        }

        #endregion

        #region ListCost Member

        public void GridListCost_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void InwardDrugMedDeptInvoice_GetListCost(long inviID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //▼====== #010
                        contract.BeginInwardDrugMedDeptInvoice_GetListCost(inviID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                        //▲====== #010
                            try
                            {
                                var results = contract.EndInwardDrugMedDeptInvoice_GetListCost(asyncResult);
                                CurrentInwardDrugMedDeptInvoice.CostTableMedDeptLists = results.ToObservableCollection();
                                SumTotalPercent();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //isLoadingCost = false;
                                // Globals.IsBusy = false;
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void SumTotalPercent()
        {
            TotalPercent = 0;
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.CostTableMedDeptLists != null)
            {
                for (int i = 0; i < CurrentInwardDrugMedDeptInvoice.CostTableMedDeptLists.Count; i++)
                {
                    TotalPercent += CurrentInwardDrugMedDeptInvoice.CostTableMedDeptLists[i].Rates;
                }
            }
        }
        #endregion

        public void ActualQD_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.ExchangeRates != 0)
                {
                    try
                    {
                        CurrentInwardDrugMedDeptInvoice.TotalPriceActual_QĐ = Convert.ToDecimal((sender as TextBox).Text);
                    }
                    catch
                    {
                        Globals.ShowMessage(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe, eHCMSResources.T0074_G1_I);
                    }
                    CurrentInwardDrugMedDeptInvoice.TotalPriceActual = CurrentInwardDrugMedDeptInvoice.TotalPriceActual_QĐ / (decimal)CurrentInwardDrugMedDeptInvoice.ExchangeRates;
                }
            }
        }

        public void Actual_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (CurrentInwardDrugMedDeptInvoice != null)
                {
                    try
                    {
                        CurrentInwardDrugMedDeptInvoice.TotalPriceActual = Convert.ToDecimal((sender as TextBox).Text);
                    }
                    catch
                    {
                        Globals.ShowMessage(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe, eHCMSResources.T0074_G1_I);
                    }
                }
            }
        }

        private void Process_AllPurchaseOrderChecked()
        {
            if (DrugDeptPurchaseOrderDetailList != null && DrugDeptPurchaseOrderDetailList.Count > 0)
            {
                for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
                {
                    DrugDeptPurchaseOrderDetailList[i].IsCheckedForDel = true;
                }
            }
        }

        private void Process_AllPurchaseOrderUnChecked()
        {
            if (DrugDeptPurchaseOrderDetailList != null && DrugDeptPurchaseOrderDetailList.Count > 0)
            {
                for (int i = 0; i < DrugDeptPurchaseOrderDetailList.Count; i++)
                {
                    DrugDeptPurchaseOrderDetailList[i].IsCheckedForDel = false;
                }
            }
        }

        public void btnDeleteSelItem()
        {
            if (DrugDeptPurchaseOrderDetailList == null || DrugDeptPurchaseOrderDetailList.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.A0648_G1_Msg_InfoKhCoHgDeXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            var items = DrugDeptPurchaseOrderDetailList.Where(x => x.IsCheckedForDel == true);

            if (items != null && items.Count() > 0)
            {
                if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DrugDeptPurchaseOrderDetailList = DrugDeptPurchaseOrderDetailList.Where(x => x.IsCheckedForDel == false).ToObservableCollection();
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
        }

        #region Events
        protected override void OnActivate()
        {
            base.OnActivate();
            //▼===== #005
            ObjDrugDeptSellPriceProfitScale = new List<DrugDeptSellPriceProfitScale>();
            if (Globals.ServerConfigSection.MedDeptElements.CalForPriceProfitScale_DrugDept)
            {
                DrugDeptSellPriceProfitScale_GetList_Paging(0, 50, true);
            }
            //▲===== #005
            Coroutine.BeginExecute(DoGetStore_MedDept());
            InitControls();
        }

        public void btnSaveReturn()
        {
            string ErrorMsg = "";
            if (CurrentInwardDrugMedDeptInvoice == null || CurrentInwardDrugMedDeptInvoice.inviID == 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(CurrentInwardDrugMedDeptInvoice.ReturnInvInvoiceNumber) || string.IsNullOrEmpty(CurrentInwardDrugMedDeptInvoice.ReturnNote) || !InwardDrugMedDeptList.Any(x => x.ReturnQuantity > 0))
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z2349_G1_VuiLongNhapDayDuTT), eHCMSResources.T0074_G1_I);
                return;
            }

            //▼===== #003
            if (!Globals.CheckLimitCharaterOfInwardInvoice(out ErrorMsg, CurrentInwardDrugMedDeptInvoice.ReturnInvInvoiceNumber, CurrentInwardDrugMedDeptInvoice.ReturnSerialNumber))
            {
                MessageBox.Show(ErrorMsg, eHCMSResources.G0449_G1_TBaoLoi);
                return;
            }
            //▲===== #003
            ObservableCollection<OutwardDrugMedDept> ReturnDrugMedDeptCollection = new ObservableCollection<OutwardDrugMedDept>(InwardDrugMedDeptList.Where(x => x.ReturnQuantity > 0).Select(x => new OutwardDrugMedDept
            {
                GenMedProductID = x.GenMedProductID,
                InID = x.InID,
                OutQuantity = x.ReturnQuantity,
                OutPrice = x.InBuyingPriceActual,
                RefGenericDrugDetail = new RefGenMedProductDetails
                {
                    GenMedProductID = x.GenMedProductID.GetValueOrDefault(0)
                },
                DrugDeptInIDOrig = x.DrugDeptInIDOrig,
                Remaining = x.Remaining
            }).ToList());
            ObservableCollection<OutwardDrugMedDept> OutwardDrugMedDeptCollection = new ObservableCollection<OutwardDrugMedDept>();
            var CopyOfInwardDrugMedDeptRemain = InwardDrugMedDeptRemain.DeepCopy();
            foreach (var item in ReturnDrugMedDeptCollection)
            {
                if (item.OutQuantity > item.Remaining
                    && CopyOfInwardDrugMedDeptRemain.Any(x => (x.DrugDeptInIDOrig == item.InID || x.InID == item.InID) && x.Remaining > 0)
                    && CopyOfInwardDrugMedDeptRemain.Where(x => (x.DrugDeptInIDOrig == item.InID || x.InID == item.InID) && x.Remaining > 0).Sum(x => x.Remaining) >= item.OutQuantity)
                {
                    foreach (var childitem in CopyOfInwardDrugMedDeptRemain.Where(x => (x.DrugDeptInIDOrig == item.InID || x.InID == item.InID) && x.Remaining > 0).ToList())
                    {
                        decimal oOutQuantity = 0;
                        if (item.OutQuantity > childitem.Remaining)
                        {
                            oOutQuantity = childitem.Remaining;
                        }
                        else
                        {
                            oOutQuantity = item.OutQuantity;
                        }
                        var oOutwardDrug = item.DeepCopy();
                        oOutwardDrug.OutQuantity = oOutQuantity;
                        oOutwardDrug.InID = childitem.InID;
                        childitem.Remaining = childitem.Remaining - oOutQuantity;
                        item.OutQuantity = item.OutQuantity - oOutQuantity;
                        OutwardDrugMedDeptCollection.Add(oOutwardDrug);
                    }
                    continue;
                }
                else
                {
                    OutwardDrugMedDeptCollection.Add(item);
                }
            }
            OutwardDrugMedDeptInvoice_SaveByType(new OutwardDrugMedDeptInvoice
            {
                V_MedProductType = V_MedProductType,
                StoreID = CurrentInwardDrugMedDeptInvoice.StoreID,
                StaffID = Globals.LoggedUserAccount?.StaffID,
                TypID = (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC,
                V_OutDrugInvStatus = (long)AllLookupValues.V_OutDrugInvStatus.SAVE,
                OutDate = Globals.GetCurServerDateTime(),
                V_ByOutPriceMedDept = (long)AllLookupValues.V_ByOutPriceMedDept.GIAVON,
                VAT = CurrentInwardDrugMedDeptInvoice.ReturnVAT,
                ReturnID = CurrentInwardDrugMedDeptInvoice.inviID,
                ReturnInvInvoiceNumber = CurrentInwardDrugMedDeptInvoice.ReturnInvInvoiceNumber,
                ReturnSerialNumber = CurrentInwardDrugMedDeptInvoice.ReturnSerialNumber,
                ReturnInvoiceForm = CurrentInwardDrugMedDeptInvoice.ReturnInvoiceForm,
                ReturnNote = CurrentInwardDrugMedDeptInvoice.ReturnNote,
                OutwardDrugMedDepts = OutwardDrugMedDeptCollection
            });
        }

        public void btnNewReturnInv()
        {
            if (CurrentInwardDrugMedDeptInvoice == null || CurrentInwardDrugMedDeptInvoice.inviID == 0 || string.IsNullOrEmpty(CurrentInwardDrugMedDeptInvoice.OutInvID))
            {
                return;
            }
            CurrentInwardDrugMedDeptInvoice.ReturnInvInvoiceNumber = null;
            CurrentInwardDrugMedDeptInvoice.ReturnSerialNumber = null;
            CurrentInwardDrugMedDeptInvoice.ReturnInvoiceForm = null;
            CurrentInwardDrugMedDeptInvoice.ReturnNote = null;
            CurrentInwardDrugMedDeptInvoice.OutInvID = null;
            if (InwardDrugMedDeptList == null || InwardDrugMedDeptList.Count == 0)
            {
                return;
            }
            foreach (var item in InwardDrugMedDeptList)
            {
                item.ReturnQuantity = 0;
            }
        }

        public void btnPrintReturnInv()
        {
            if (CurrentInwardDrugMedDeptInvoice == null || CurrentInwardDrugMedDeptInvoice.ReturnoutiID == 0)
            {
                return;
            }
            void onInitDlg(IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentInwardDrugMedDeptInvoice.ReturnoutiID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z2190_G1_PhXuatTraThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z2190_G1_PhXuatTraYCu.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION)
                {
                    proAlloc.LyDo = eHCMSResources.Z3210_G1_PhXuatTraDDuong.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z2190_G1_PhXuatTraHChat.ToUpper();
                }
                proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;
            }
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        public void btnFindReturnInv()
        {
            if (CurrentInwardDrugMedDeptInvoice == null)
            {
                return;
            }
            MedDeptInvoiceSearchCriteria SearchCriteria = new MedDeptInvoiceSearchCriteria
            {
                StoreID = CurrentInwardDrugMedDeptInvoice.StoreID,
                TypID = (long)AllLookupValues.RefOutputType.XUAT_TRA_NCC,
                V_MedProductType = V_MedProductType
            };
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugMedDeptInvoice_SearchByType(SearchCriteria, 0, Globals.PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int TotalCount = 0;
                                var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out TotalCount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        void onInitDlg(IXuatNoiBoSearch proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.OutwardMedDeptInvoiceList.Clear();
                                            proAlloc.OutwardMedDeptInvoiceList.TotalItemCount = TotalCount;
                                            proAlloc.OutwardMedDeptInvoiceList.PageIndex = 0;
                                            foreach (OutwardDrugMedDeptInvoice p in results)
                                            {
                                                proAlloc.OutwardMedDeptInvoiceList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IXuatNoiBoSearch>(onInitDlg);
                                    }
                                    else
                                    {
                                        if (results != null && results.Count > 0)
                                        {
                                            LoadReturnInvoice(results.FirstOrDefault());
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }
        #endregion

        #region Methods
        private void InitControls()
        {
            if (GridInwardDrug != null && IsRetundView)
            {
                var ButtonsColumn = GridInwardDrug.Columns.FirstOrDefault(x => x.Equals(GridInwardDrug.GetColumnByName("ButtonsColumn")));
                var ReturnQtyColumn = GridInwardDrug.Columns.FirstOrDefault(x => x.Equals(GridInwardDrug.GetColumnByName("ReturnQtyColumn")));
                if (ButtonsColumn != null)
                {
                    ButtonsColumn.Visibility = Visibility.Collapsed;
                }
                if (ReturnQtyColumn != null)
                {
                    ReturnQtyColumn.Visibility = Visibility.Visible;
                }
            }
            if (IsRetundView)
            {
                ElseVisibility = Visibility.Collapsed;
            }
        }

        private void OutwardDrugMedDeptInvoice_SaveByType(OutwardDrugMedDeptInvoice OutwardInvoice)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0172_G1_DangLuuDLieu);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginOutwardDrugMedDeptInvoice_SaveByType(OutwardInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long OutID = 0;
                                string StrError;
                                bool value = contract.EndOutwardDrugMedDeptInvoice_SaveByType(out OutID, out StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError) && value)
                                {
                                    MessageBox.Show(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK));
                                    GetOutwardDrugMedDeptInvoice(OutID);
                                }
                                else
                                {
                                    MessageBox.Show(StrError);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.HideBusyIndicator();
                }
            });

            t.Start();
        }

        private void GetOutwardDrugMedDeptInvoice(long OutwardID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugMedDeptInvoice(OutwardID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var OutInvoice = contract.EndGetOutwardDrugMedDeptInvoice(asyncResult);
                                if (CurrentInwardDrugMedDeptInvoice != null && OutInvoice != null)
                                {
                                    CurrentInwardDrugMedDeptInvoice.ReturnoutiID = OutInvoice.outiID;
                                    CurrentInwardDrugMedDeptInvoice.OutInvID = OutInvoice.OutInvID;
                                }
                                //OutwardDrugMedDeptDetails_Load(SelectedOutInvoice.outiID);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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

        private void OutwardDrugMedDeptDetails_Load(long outiID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetOutwardDrugMedDeptDetailByInvoice(outiID, V_MedProductType, false, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetOutwardDrugMedDeptDetailByInvoice(asyncResult);
                                if (results != null && InwardDrugMedDeptList != null && InwardDrugMedDeptList.Count > 0)
                                {
                                    foreach (var item in results)
                                    {
                                        if (InwardDrugMedDeptList.Any(x => x.InID == item.InID || x.InID == item.DrugDeptInIDOrig))
                                        {
                                            InwardDrugMedDeptList.FirstOrDefault(x => x.InID == item.InID || x.InID == item.DrugDeptInIDOrig).ReturnQuantity += item.OutQuantity;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
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

        private void LoadReturnInvoice(OutwardDrugMedDeptInvoice mOutwardDrugMedDeptInvoice)
        {
            SearchCriteria.InwardID = null;
            SearchCriteria.InvoiceNumber = null;
            SearchCriteria.inviID = mOutwardDrugMedDeptInvoice.ReturninviID;
            SearchInwardInvoiceDrug(0, 20, mOutwardDrugMedDeptInvoice);
        }
        //▼===== #005
        private void DrugDeptSellPriceProfitScale_GetList_Paging(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjDrugDeptSellPriceProfitScale.Clear();
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginDrugDeptSellPriceProfitScale_GetList_Paging(V_MedProductType, true, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<DrugDeptSellPriceProfitScale> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndDrugDeptSellPriceProfitScale_GetList_Paging(out Total, asyncResult);
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

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    ObjDrugDeptSellPriceProfitScale = allItems.ToList();
                                }
                            }
                        }), null)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    this.HideBusyIndicator();
                }
            });
            t.Start();
        }
        //▲===== #005
        #endregion

        private ObservableCollection<Bid> _BidCollection;
        public ObservableCollection<Bid> BidCollection
        {
            get
            {
                return _BidCollection;
            }
            set
            {
                _BidCollection = value;
                NotifyOfPropertyChange(() => BidCollection);
            }
        }

        private void LoadValidBidFromSupplierID(long SupplierID, long V_MedProductType)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                using (var mServiceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var mContract = mServiceFactory.ServiceInstance;
                    try
                    {
                        mContract.BeginGetInUsingBidCollectionFromSupplierID(SupplierID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var mReturnValues = mContract.EndGetInUsingBidCollectionFromSupplierID(asyncResult);
                                if (mReturnValues == null || mReturnValues.Count == 0)
                                {
                                    BidCollection = new ObservableCollection<Bid>();
                                }
                                else
                                {
                                    //▼===== #009:  Không đưa thông tin rỗng vào dòng đầu tiên nữa mà sẽ mặc định chọn cái thầu cuối cùng để đưa vào thuốc/ y cụ.
                                    mReturnValues.Insert(0, new Bid { BidName = " " });
                                    BidCollection = mReturnValues.ToObservableCollection();
                                    SelectedBidID = mReturnValues.OrderByDescending(x => x.BidID).FirstOrDefault().BidID;
                                    //▲===== #009
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
                        MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        this.HideBusyIndicator();
                    }
                }
            });

            t.Start();
        }

        private long? _SelectedBidID;
        public long? SelectedBidID
        {
            get
            {
                return _SelectedBidID;
            }
            set
            {
                _SelectedBidID = value;
                PharmacyPurchaseOrder_BySupplierID();
                NotifyOfPropertyChange(() => SelectedBidID);
            }
        }

        private bool _IsInvoiceCanDefineBid = true;
        public bool IsInvoiceCanDefineBid
        {
            get
            {
                return _IsInvoiceCanDefineBid;
            }
            set
            {
                _IsInvoiceCanDefineBid = value;
                NotifyOfPropertyChange(() => IsInvoiceCanDefineBid);
            }
        }

        public bool IsHadDetail
        {
            get
            {
                return InwardDrugMedDeptList != null && InwardDrugMedDeptList.Count > 0 && Globals.ServerConfigSection.MedDeptElements.UseBidDetailOnInward;
            }
        }
        private bool _IsVatReadOnly = false;
        public bool IsVatReadOnly
        {
            get{ return _IsVatReadOnly; }
            set
            {
                if (_IsVatReadOnly != value)
                {
                    _IsVatReadOnly = value;
                    NotifyOfPropertyChange(() => IsVatReadOnly);
                }
            }
        }
        public void chkIsNotVat_Click(object sender, RoutedEventArgs e)
        {
            var chkIsNotVat = sender as CheckBox;
            if ((chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail) != null)
            {
                if (chkIsNotVat.IsChecked == true)
                {
                    if (MessageBox.Show(eHCMSResources.Z2993_G1_ChonKhongThue, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel,MessageBoxImage.Warning,MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                    {
                        chkIsNotVat.IsChecked = false;
                        (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = false;
                        return;
                    }
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = true;
                }
                else
                {
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = false;
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).VAT = 0;
                }
                UpdatePriceIncludeVAT(chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail);
                SetPriceByPriceScale(chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail);
            }
        }
        KeyEnabledComboBox KeySC = null;
        public void cbxKho_Loaded(object sender, SelectionChangedEventArgs e)
        {
            KeySC = sender as KeyEnabledComboBox;
        }
        public void cbxKho_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (KeySC != null && KeySC.SelectedItem is RefStorageWarehouseLocation)
            {
                RefStorageWarehouseLocation Store = KeySC.SelectedItem as RefStorageWarehouseLocation;
                if((CurrentInwardDrugMedDeptInvoice == null || CurrentInwardDrugMedDeptInvoice.inviID == 0))
                {
                    if (Store.IsVATCreditStorage)
                    {
                        CurrentInwardDrugMedDeptInvoice.IsVATCredit = true;
                    }
                    else
                    {
                        CurrentInwardDrugMedDeptInvoice.IsVATCredit = false;
                    }
                }
                //TypID = Store.IsConsignment ? (long)AllLookupValues.RefOutputType.NHAP_HANGKYGOI : (long)AllLookupValues.RefOutputType.NHAP_TU_NCC;
            }
        }
        public bool ChangeVATCreditOnInwardInvoice
        {
            get { return Globals.ServerConfigSection.CommonItems.ChangeVATCreditOnInwardInvoice; }
        }

        public void SetPriceByPriceScale(DrugDeptPurchaseOrderDetail item)
        {
            decimal PriceProfitScale = 0;
            decimal PriceProfitScaleForHI = 0;
            PriceProfitScale = (decimal) ObjDrugDeptSellPriceProfitScale
                .Where(x => x.BuyingCostFrom <= item.UnitPrice && x.BuyingCostTo >= item.UnitPrice)
                .Select(x => x.NormalProfitPercent).FirstOrDefault();
            if (item.RefGenMedProductDetail.BidID != 0)
            {
                PriceProfitScaleForHI = (decimal) ObjDrugDeptSellPriceProfitScale
                    .Where(x => x.BuyingCostFrom <= item.UnitPrice && x.BuyingCostTo >= item.UnitPrice)
                    .Select(x => x.HIAllowProfitPercent).FirstOrDefault();
            }

            // VuTTM
            if (CurrentInwardDrugMedDeptInvoice.IsVATCredit
                && 0 == item.RefGenMedProductDetail.BidID)
            {
                SetPricesByDeductAndOutsideBid(item, PriceProfitScale);
                return;
            }
            if (CurrentInwardDrugMedDeptInvoice.IsVATCredit
                && 0 != item.RefGenMedProductDetail.BidID)
            {
                SetPricesByDeductAndBid(item, PriceProfitScaleForHI);
                GetDrugDeptSellingItemPriceDetails(item);
                return;
            }
            if (!CurrentInwardDrugMedDeptInvoice.IsVATCredit)
            {
                SetPricesByNoneDeduct(item);
            }
        }

        public void SetPricesByDeductAndOutsideBid(DrugDeptPurchaseOrderDetail item, decimal priceProfitScale)
        {
            item.VAT = (null == item.VAT || item.IsNotVat) ? 0 : item.VAT;
            item.PackageQuantity = item.InQuantity / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
            
            item.TotalPriceNotVAT = Decimal.Round((decimal)item.InQuantity * item.UnitPrice);
            item.RefGenMedProductDetail.NormalPrice = item.UnitPrice * (1 + (decimal)item.VAT) * (1 + (priceProfitScale / 100));
            item.RefGenMedProductDetail.HIPatientPrice = item.UnitPrice * (1 + (decimal)item.VAT) * (1 + (priceProfitScale / 100));
            item.PackagePrice = 0;
            item.RefGenMedProductDetail.HIAllowedPrice = 0;
            item.RefGenMedProductDetail.NormalPrice = Decimal.Round(item.RefGenMedProductDetail.NormalPrice);
            item.RefGenMedProductDetail.HIPatientPrice = V_MedProductType == (long)AllLookupValues.MedProductType.NUTRITION
                                            ? Decimal.Round(item.RefGenMedProductDetail.HIPatientPrice) : item.RefGenMedProductDetail.HIPatientPrice;
        }

        public void SetPricesByDeductAndBid(DrugDeptPurchaseOrderDetail item, decimal PriceProfitScaleForHI)
        {
            item.VAT = (null == item.VAT || item.IsNotVat) ? 0 : item.VAT;
            item.PackageQuantity = item.InQuantity / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);

            item.TotalPriceNotVAT = Decimal.Round((decimal)item.InQuantity * item.UnitPrice);
            item.RefGenMedProductDetail.NormalPrice = item.UnitPrice * (1 + (decimal)item.VAT) * (1 + (PriceProfitScaleForHI / 100));
            item.RefGenMedProductDetail.HIPatientPrice = item.RefGenMedProductDetail.BidInCost;
            item.RefGenMedProductDetail.HIAllowedPrice = item.RefGenMedProductDetail.BidInCost;
            item.RefGenMedProductDetail.NormalPrice = Decimal.Round(item.RefGenMedProductDetail.NormalPrice);
        }

        public void SetPricesByNoneDeduct(DrugDeptPurchaseOrderDetail item)
        {
            item.VAT = (null == item.VAT || item.IsNotVat) ? 0 : item.VAT;
            item.PackageQuantity = item.InQuantity / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);

            item.PriceIncludeVAT = item.UnitPrice;
            item.TotalPriceNotVAT = Decimal.Round((decimal)item.InQuantity * item.UnitPrice);
            item.RefGenMedProductDetail.NormalPrice = 0;
            item.RefGenMedProductDetail.HIPatientPrice = 0;
            item.PackagePrice = 0;
            item.RefGenMedProductDetail.HIAllowedPrice = 0;
        }
    }
}
