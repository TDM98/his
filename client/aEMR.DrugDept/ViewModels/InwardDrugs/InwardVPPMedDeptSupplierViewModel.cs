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
/*
 * 20191113 #001 TTM: BM 0019561: Giới hạn độ dài các trường trong phiếu nhập (Mã hoá đơn, số serial). Vì chương trình cho phép tối đa 16 nhưng FAST chỉ lấy được 12 kí tự.
 * 20200411 #002 TTM: BM 0029095: Thêm điều kiện kiểm tra để không lưu thông tin giá bán dv, giá BH và giá cho BNBH < 0
 * 20211102 #003: Lọc kho theo cấu hình trách nhiệm
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IVPPMedDeptInwardSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardVPPMedDeptSupplierViewModel : ViewModelBase, IVPPMedDeptInwardSupplier
        , IHandle<DrugDeptCloseSearchSupplierEvent>
        , IHandle<DrugDeptCloseSearchInwardIncoiceEvent>
        , IHandle<DrugDeptCloseEditInwardEvent>
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
        public InwardVPPMedDeptSupplierViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            InitializeInvoice();
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

            InwardVPPMedDeptList = new PagedSortableCollectionView<InwardDrugMedDept>();
            InwardVPPMedDeptList.OnRefresh += new EventHandler<RefreshEventArgs>(InwardVPPMedDeptList_OnRefresh);
            InwardVPPMedDeptList.PageSize = Globals.PageSize;

            Suppliers = new PagedSortableCollectionView<DrugDeptSupplier>();
            Suppliers.OnRefresh += new EventHandler<RefreshEventArgs>(Suppliers_OnRefresh);
            Suppliers.PageSize = Globals.PageSize;
        }
        protected override void OnActivate()
        {
            base.OnActivate();
            Coroutine.BeginExecute(DoGetStore_MedDept());
        }
        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }
        private long inviID = 0;
        void InwardVPPMedDeptList_OnRefresh(object sender, RefreshEventArgs e)
        {
            InwardVPPDetails_ByID(inviID, InwardVPPMedDeptList.PageIndex, InwardVPPMedDeptList.PageSize);
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

        private InwardDrugMedDeptInvoice _CurrentInwardVPPMedDeptInvoice;
        public InwardDrugMedDeptInvoice CurrentInwardVPPMedDeptInvoice
        {
            get
            {
                return _CurrentInwardVPPMedDeptInvoice;
            }
            set
            {
                if (_CurrentInwardVPPMedDeptInvoice != value)
                {
                    _CurrentInwardVPPMedDeptInvoice = value;
                    if (value != null && value.CanEditAndDelete)
                    {
                        IsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        IsVisibility = Visibility.Collapsed;
                    }
                    NotifyOfPropertyChange(() => IsVisibility);
                    NotifyOfPropertyChange(() => CurrentInwardVPPMedDeptInvoice);
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

        private PagedSortableCollectionView<InwardDrugMedDept> _InwardVPPMedDeptList;
        public PagedSortableCollectionView<InwardDrugMedDept> InwardVPPMedDeptList
        {
            get
            {
                return _InwardVPPMedDeptList;
            }
            set
            {
                if (_InwardVPPMedDeptList != value)
                {
                    _InwardVPPMedDeptList = value;
                    NotifyOfPropertyChange(() => InwardVPPMedDeptList);
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

        private InwardDrugMedDept _CurrentInwardVPPMedDept;
        public InwardDrugMedDept CurrentInwardVPPMedDept
        {
            get
            {
                return _CurrentInwardVPPMedDept;
            }
            set
            {
                if (_CurrentInwardVPPMedDept != value)
                {
                    _CurrentInwardVPPMedDept = value;
                    NotifyOfPropertyChange(() => CurrentInwardVPPMedDept);
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
                    if (_IsVisibility == Visibility.Visible)
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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM;
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
        #endregion

        public void Authorization()
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
                return _mThemMoi;
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
                return _mCapNhat;
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                CurrentInwardVPPMedDeptInvoice.SelectedCurrency = Currencies.FirstOrDefault();
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false, null, false, false, true);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #003
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                CurrentInwardVPPMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #003
            this.HideBusyIndicator();
            yield break;
        }

        public void BtnNew(object sender, RoutedEventArgs e)
        {
            InitializeInvoice();
        }

        private void InitializeInvoice()
        {
            CurrentInwardVPPMedDeptInvoice = new InwardDrugMedDeptInvoice
            {
                InvDateInvoice = Globals.GetCurServerDateTime(),
                DSPTModifiedDate = Globals.GetCurServerDateTime(),
                SelectedStaff = GetStaffLogin(),
                StaffID = GetStaffLogin().StaffID,
                IsTrongNuoc = true,
                IsForeign = false,

                // Txd 25/05/2014 Replaced ConfigList
                VAT = (decimal)Globals.ServerConfigSection.PharmacyElements.PharmacyDefaultVATInward,

                SelectedCurrency = new Currency()
            };
            CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID = (long)AllLookupValues.CurrencyTable.VND;

            if (StoreCbx != null)
            {
                CurrentInwardVPPMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
            }
            IsHideAuSupplier = true;
            if (InwardVPPMedDeptList != null)
            {
                InwardVPPMedDeptList.Clear();
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
            CurrentInwardInvoiceCopy = CurrentInwardVPPMedDeptInvoice.DeepCopy();
        }

        private void DeleteInwardInvoice()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteInwardVPPMedDeptInvoice(CurrentInwardVPPMedDeptInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndDeleteInwardVPPMedDeptInvoice(asyncResult);
                                if (results == 0)
                                {
                                    InitializeInvoice();
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

        public void BtnDeletePhieu(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0557_G1_CoChacMuonXoa, eHCMSResources.Z0578_G1_PhNhap), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteInwardInvoice();
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
            if (CurrentInwardVPPMedDeptInvoice.SelectedCurrency == null || CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID == 0)
            {
                Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.K3709_G1_DViTinh), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
            {
                if (CurrentInwardVPPMedDeptInvoice.ExchangeRates <= 0)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z0581_G1_TyGia), eHCMSResources.G0442_G1_TBao);
                    return;
                }
            }

            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            CurrentInwardVPPMedDeptInvoice.TypID = TypID;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateInwardVPPMedDeptInvoice(CurrentInwardVPPMedDeptInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndUpdateInwardVPPMedDeptInvoice(asyncResult);
                                if (results == 0)
                                {
                                    DeepCopyInvoice();
                                    CheckIsPercentAll();
                                    Load_InwardVPPDetails(CurrentInwardVPPMedDeptInvoice.inviID);
                                    InwardVPPMedDeptInvoice_GetListCost(CurrentInwardVPPMedDeptInvoice.inviID);
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

        public void BtnEdit(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(eHCMSResources.Z0583_G1_CoMuonChinhSuaKg, eHCMSResources.Z0578_G1_PhNhap), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CheckValidate())
                {
                    //▼===== #001
                    string ErrorMsg = "";
                    if (!Globals.CheckLimitCharaterOfInwardInvoice(out ErrorMsg, CurrentInwardVPPMedDeptInvoice.InvInvoiceNumber, CurrentInwardVPPMedDeptInvoice.SerialNumber))
                    {
                        MessageBox.Show(ErrorMsg, eHCMSResources.G0449_G1_TBaoLoi);
                        return;
                    }
                    //▲===== #001
                    if (CurrentInwardVPPMedDeptInvoice.SelectedStorage != null)
                    {
                        CurrentInwardVPPMedDeptInvoice.StoreID = CurrentInwardVPPMedDeptInvoice.SelectedStorage.StoreID;
                    }
                    if (CurrentInwardVPPMedDeptInvoice.SelectedCurrency != null)
                    {
                        CurrentInwardVPPMedDeptInvoice.CurrencyID = CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID;
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
            CurrentInwardVPPMedDeptInvoice.TotalPriceNotVAT = TongTienSPChuaVAT;
            CurrentInwardVPPMedDeptInvoice.TotalDiscountOnProduct = CKTrenSP;//tong ck tren hoan don
            CurrentInwardVPPMedDeptInvoice.TotalDiscountInvoice = CKTrenSP + TongCKTrenHoaDon;
            CurrentInwardVPPMedDeptInvoice.TotalPrice = TongTienSPChuaVAT - (CKTrenSP + TongCKTrenHoaDon);
            CurrentInwardVPPMedDeptInvoice.TotalHaveCustomTax = TongTienHoaDonCoThueNK;
            CurrentInwardVPPMedDeptInvoice.TotalPriceVAT = TongTienHoaDonCoVAT;
            CurrentInwardVPPMedDeptInvoice.TotalVATDifferenceAmount = TotalVATDifferenceAmount;
            CurrentInwardVPPMedDeptInvoice.TotalVATAmountActual = CurrentInwardVPPMedDeptInvoice.TotalVATAmount + CurrentInwardVPPMedDeptInvoice.TotalVATDifferenceAmount;
        }

        private void Load_InwardVPPDetails(long ID)
        {
            inviID = ID;
            InwardVPPMedDeptList.PageIndex = 0;
            InwardVPPDetails_ByID(CurrentInwardVPPMedDeptInvoice.inviID, 0, Globals.PageSize);
        }

        private void InwardVPPDetails_ByID(long ID, int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardVPPMedDept_ByIDInvoice(ID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetInwardVPPMedDept_ByIDInvoice(out int total, out decimal TongTienSPChuaVAT, out decimal CKTrenSP
                                    , out decimal TongTienTrenSPDaTruCK, out decimal TongCKTrenHoaDon, out decimal TongTienHoaDonCoThueNK
                                    , out decimal TongTienHoaDonCoVAT, out decimal TotalVATDifferenceAmount, asyncResult
                                );

                                InwardVPPMedDeptList.Clear();
                                InwardVPPMedDeptList.TotalItemCount = total;
                                if (results != null)
                                {
                                    foreach (InwardDrugMedDept p in results)
                                    {
                                        InwardVPPMedDeptList.Add(p);
                                    }
                                }
                                CountTotalPrice(TongTienSPChuaVAT, CKTrenSP, TongTienTrenSPDaTruCK, TongCKTrenHoaDon, TongTienHoaDonCoThueNK, TongTienHoaDonCoVAT, TotalVATDifferenceAmount);

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

        private void PharmacyPurchaseOrder_BySupplierID()
        {
            if (CurrentInwardVPPMedDeptInvoice.SelectedSupplier != null)
            {
                PharmacyPurchaseOrder_BySupplierID(CurrentInwardVPPMedDeptInvoice.SelectedSupplier.SupplierID);
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrder_BySupplierID(SupplierID, V_MedProductType, 0, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptPurchaseOrder_BySupplierID(asyncResult);
                                DrugDeptPurchaseOrderList = results.ToObservableCollection();
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardVPPMedDeptInvoice_ByID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentInwardVPPMedDeptInvoice = contract.EndGetInwardVPPMedDeptInvoice_ByID(asyncResult);
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
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            CurrentInwardVPPMedDeptInvoice.TypID = TypID;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddInwardVPPMedDeptInvoice(CurrentInwardVPPMedDeptInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndAddInwardVPPMedDeptInvoice(out long ID, asyncResult);
                                if (results == 0)
                                {
                                    Globals.ShowMessage(eHCMSResources.A1027_G1_Msg_InfoThemOK, eHCMSResources.G0442_G1_TBao);
                                    GetInwardInvoiceDrugByID(ID);
                                }
                                else
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0584_G1_SoHDDaTonTai), eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            if (CurrentInwardVPPMedDeptInvoice == null)
            {
                return false;
            }
            return CurrentInwardVPPMedDeptInvoice.Validate();
        }

        public void BtnAdd(object sender, RoutedEventArgs e)
        {
            if (CheckValidate())
            {
                if (CurrentInwardVPPMedDeptInvoice.SelectedCurrency == null || CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID == 0)
                {
                    Globals.ShowMessage(string.Format(eHCMSResources.Z0579_G1_VuiLongChon, eHCMSResources.K3709_G1_DViTinh), eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
                {
                    if (CurrentInwardVPPMedDeptInvoice.ExchangeRates <= 0)
                    {
                        Globals.ShowMessage(string.Format(eHCMSResources.Z0580_G1_VuiLongNhap, eHCMSResources.Z0581_G1_TyGia), eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                }
                if (CurrentInwardVPPMedDeptInvoice.VAT < 1 && CurrentInwardVPPMedDeptInvoice.VAT > 0)
                {
                    Globals.ShowMessage(eHCMSResources.K0262_G1_VATKhongHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardVPPMedDeptInvoice.DiscountingByPercent < 1 && CurrentInwardVPPMedDeptInvoice.DiscountingByPercent > 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0072_G1_CKKhHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▼===== #001
                string ErrorMsg = "";
                if (!Globals.CheckLimitCharaterOfInwardInvoice(out ErrorMsg, CurrentInwardVPPMedDeptInvoice.InvInvoiceNumber, CurrentInwardVPPMedDeptInvoice.SerialNumber))
                {
                    MessageBox.Show(ErrorMsg, eHCMSResources.G0449_G1_TBaoLoi);
                    return;
                }
                //▲===== #001
                if (CurrentInwardVPPMedDeptInvoice.SelectedStorage != null)
                {
                    CurrentInwardVPPMedDeptInvoice.StoreID = CurrentInwardVPPMedDeptInvoice.SelectedStorage.StoreID;
                }
                CurrentInwardVPPMedDeptInvoice.CurrencyID = CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID;
                CurrentInwardVPPMedDeptInvoice.V_MedProductType = V_MedProductType;
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
            if (CurrentInwardVPPMedDept == null || CurrentInwardVPPMedDept.GenMedProductID == null || CurrentInwardVPPMedDept.GenMedProductID <= 0)
            {
                return;
            }

            GetRefGenMedProductDetails(CurrentInwardVPPMedDept.GenMedProductID.GetValueOrDefault(), CurrentInwardVPPMedDept.GenMedVersionID);
        }

        private void GetRefGenMedProductDetails(long genMedProductID, long? genMedVersionID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                case 11008:
                case 11009:
                case 11012:
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
            }
        }

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void rdtTrongNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentInwardVPPMedDeptInvoice.IsForeign = false;
        }
        public void rdtNgoaiNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentInwardVPPMedDeptInvoice.IsForeign = true;
        }
        private long? DrugDeptPoID = 0;
        private void DrugDeptPurchaseOrderDetail_ByParentID(object Item)
        {
            DrugDeptPurchaseOrder CurrentOrder = Item as DrugDeptPurchaseOrder;
            DrugDeptPoID = CurrentOrder.DrugDeptPoID;
            DrugDeptPurchaseOrderDetail_ByParentID(DrugDeptPoID.GetValueOrDefault());
        }

        DataGrid GridInwardDrug = null;
        public void GridInwardDrug_Loaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug = sender as DataGrid;
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
                    }

                }
            }
        }
        private void DrugDeptPurchaseOrderDetail_ByParentID(long PharmacyPoID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrderDetail_ByParentID(PharmacyPoID, 1, null, Globals.DispatchCallback((asyncResult) =>
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

        //KMx: Copy và chỉnh sửa từ hàm gốc. Lý do: Thêm điều kiện nếu thay đổi NCC mà chưa bấm cập nhật thì ko cho nhập hàng, vì bị lỗi phiếu nhập NCC A, nhưng hàng của NCC B (24/02/2015 09:37).
        private bool IsChangedItem()
        {
            if (CurrentInwardVPPMedDeptInvoice == null || CurrentInwardVPPMedDeptInvoice.SelectedSupplier == null || CurrentInwardInvoiceCopy == null || CurrentInwardInvoiceCopy.SelectedSupplier == null)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0585_G1_PhNhapKgDuTTin), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (CurrentInwardVPPMedDeptInvoice.SelectedSupplier.SupplierID != CurrentInwardInvoiceCopy.SelectedSupplier.SupplierID)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0586_G1_NCCDaThayDoi), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            if (CurrentInwardVPPMedDeptInvoice.CurrencyID != CurrentInwardInvoiceCopy.CurrencyID || CurrentInwardVPPMedDeptInvoice.ExchangeRates != CurrentInwardInvoiceCopy.ExchangeRates || CurrentInwardVPPMedDeptInvoice.VAT != CurrentInwardInvoiceCopy.VAT || CurrentInwardVPPMedDeptInvoice.IsForeign != CurrentInwardInvoiceCopy.IsForeign || CurrentInwardVPPMedDeptInvoice.Notes != CurrentInwardInvoiceCopy.Notes)
            {
                if (MessageBox.Show(eHCMSResources.Z0587_G1_CoMuonCNhatHD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CheckValidate())
                    {
                        UpdateInwardInvoiceDrug();
                    }
                    else
                    {
                        CurrentInwardVPPMedDeptInvoice = CurrentInwardInvoiceCopy;
                    }
                }
                else
                {
                    CurrentInwardVPPMedDeptInvoice = CurrentInwardInvoiceCopy;
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
                    else if (DrugDeptPurchaseOrderDetailList[i].InExpiryDate <= DateTime.Now)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0787_G1_Msg_InfoHSDLonHonNgHTai));
                        return false;
                    }
                    //else if (DrugDeptPurchaseOrderDetailList[i].InProductionDate == null)
                    //{
                    //    MessageBox.Show(string.Format("{0} ", eHCMSResources.Z0589_G1_LoiDongThu) + (i + 1).ToString() + string.Format(" {0}.", eHCMSResources.A0783_G1_Msg_InfoChuaNhapNgSX));
                    //    return false;
                    //}
                    else if (DrugDeptPurchaseOrderDetailList[i].InProductionDate >= DateTime.Now)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0590_G1_NgSXPhaiNhoHonNgHTai));
                        return false;
                    }
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
                    //▼===== #002
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
                    //▲===== #002
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
                    else if (DrugDeptPurchaseOrderDetailList[i].IsNotVat && DrugDeptPurchaseOrderDetailList[i].VAT != null)
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
            return results;
        }

        private void InwardDrug_InsertList()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardVPPMedDept_InsertList(DrugDeptPurchaseOrderDetailList.ToList(), CurrentInwardVPPMedDeptInvoice.inviID, DrugDeptPoID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndInwardVPPMedDept_InsertList(asyncResult);
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0594_G1_NhapHangThCong), eHCMSResources.G0442_G1_TBao);
                                ClearDrugDeptPurchaseOrderDetailList();
                                PharmacyPurchaseOrder_BySupplierID();
                                if (!IsEnabled)
                                {
                                    AddBlankRow();
                                }
                                if (CurrentInwardVPPMedDeptInvoice != null)
                                {
                                    Load_InwardVPPDetails(CurrentInwardVPPMedDeptInvoice.inviID);
                                    InwardVPPMedDeptInvoice_GetListCost(CurrentInwardVPPMedDeptInvoice.inviID);
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

        public void BtnNhapHang(object sender, RoutedEventArgs e)
        {
            if (!IsChangedItem())
            {
                return;
            }

            if (CheckValidationData())
            {
                InwardDrug_InsertList();
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
                Lookup item = new Lookup();
                item.LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA;
                item.ObjectValue = eHCMSResources.Z0595_G1_HangMua;
                p.V_GoodsType = item;
                p.IsPercent = IsPercentInwardDrug;
                //if (CurrentInwardVPPMedDeptInvoice != null)
                //{
                //    //20191026 TTM: lý do - 1: vì trong Inward details chỉ lấy giá trị tăng thêm (từ 0 -> 1) mà thôi.
                //    p.VAT = CurrentInwardVPPMedDeptInvoice.VAT - 1;
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
            if (GridSuppliers != null)
            {
                GridSuppliers.CurrentCellChanged += GridSuppliers_CurrentCellChanged;
            }
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
                Lookup item = new Lookup();
                item.LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA;
                item.ObjectValue = eHCMSResources.Z0595_G1_HangMua;
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

        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            long? SupplierID = 0;
            if (CurrentInwardVPPMedDeptInvoice != null && CurrentInwardVPPMedDeptInvoice.SelectedSupplier != null)
            {
                SupplierID = CurrentInwardVPPMedDeptInvoice.SelectedSupplier.SupplierID;
            }
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging(IsCode, Name, SupplierID, V_MedProductType, null, PageSize, PageIndex, null, Globals.DispatchCallback((asyncResult) =>
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
                                    CurrentDrugDeptPurchaseOrderDetail.Code = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail != null ? CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Code : "";
                                    CurrentDrugDeptPurchaseOrderDetail.VAT = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT != null ? CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT : null;
                                    CurrentDrugDeptPurchaseOrderDetail.IsNotVat = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.IsNotVat;
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
                    CurrentDrugDeptPurchaseOrderDetail.Code = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail != null ? CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Code : "";
                    if (CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail != null)
                    {
                        CurrentDrugDeptPurchaseOrderDetail.VAT = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT;
                        CurrentDrugDeptPurchaseOrderDetail.IsNotVat = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.IsNotVat;
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
            if (CurrentInwardVPPMedDeptInvoice != null)
            {
                PharmacyPurchaseOrder_BySupplierID();
            }
        }


        #endregion

        private void DeleteInwardDrug(long InID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDeleteInwardVPPMedDept(InID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndDeleteInwardVPPMedDept(asyncResult);
                                if (results == 0)
                                {
                                    Load_InwardVPPDetails(CurrentInwardVPPMedDeptInvoice.inviID);
                                    InwardVPPMedDeptInvoice_GetListCost(CurrentInwardVPPMedDeptInvoice.inviID);
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
            if (CurrentInwardVPPMedDeptInvoice.CanEditAndDelete)
            {
                if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CurrentInwardVPPMedDept != null)
                    {
                        DeleteInwardDrug(CurrentInwardVPPMedDept.InID);
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
            CurrentInwardDrugMedDeptCopy = CurrentInwardVPPMedDept.DeepCopy();
        }

        public void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardVPPMedDeptInvoice.DrugDeptSupplierPaymentReqID != null && CurrentInwardVPPMedDeptInvoice.DrugDeptSupplierPaymentReqID > 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z0597_G1_PhNhapDaTToan), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }

            if (!IsChangedItem())
            {
                return;
            }
            
            CopyCurrentInwardDrugMedDept();
            Action<IDrugDeptEditInwardDrug> onInitDlg = delegate (IDrugDeptEditInwardDrug proAlloc)
            {
                if (CurrentInwardVPPMedDeptInvoice.SelectedCurrency != null && CurrentInwardVPPMedDeptInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
                {
                    proAlloc.IsVND = false;
                }
                else
                {
                    proAlloc.IsVND = true;
                }
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.CbxGoodsTypes = CbxGoodsTypes.DeepCopy();
                proAlloc.CurrentInwardDrugMedDeptCopy = CurrentInwardVPPMedDept.DeepCopy();
                proAlloc.SetValueForProperty();

                if (CurrentInwardVPPMedDeptInvoice.CheckedPoint)
                {
                    proAlloc.CurrentInwardDrugMedDeptCopy.IsCanEdit = false;
                    MessageBox.Show(eHCMSResources.K0047_G1_ThuocDaKCTonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            };
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
                || (InwardVPPMedDeptList != null && InwardVPPMedDeptList.Count > 0))
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
        public void grdRequestDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = (e.Row.DataContext as DrugDeptPurchaseOrderDetail);
            EditedColumnName = null;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colCode")))
            {
                DrugDeptPurchaseOrderDetail item = e.Row.DataContext as DrugDeptPurchaseOrderDetail;
                if (item != null && !string.IsNullOrEmpty(item.Code) && value != item.Code)
                {
                    string Code = Globals.FormatCode(V_MedProductType, item.Code);
                    SearchRefDrugGenericDetails_AutoPaging(true, Code, 0, 1);
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                EditedColumnName = "colUnitPrice";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                EditedColumnName = "colPackagePrice";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagingQty")))
            {
                EditedColumnName = "colPackagingQty";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colQty")))
            {
                EditedColumnName = "colQty";
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colTotalNotVAT")))
            {
                EditedColumnName = "colTotalNotVAT";
            }
            if (!IsEnabled)
            {
                if (e.Row.GetIndex() == (DrugDeptPurchaseOrderDetailList.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                {
                    AddBlankRow();
                }
            }
        }
        private DrugDeptPurchaseOrderDetail EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditedDetailItem == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                EditedColumnName = null;
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.UnitPrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    item.PackagePrice = item.UnitPrice * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (EditedColumnName.Equals("colPackagePrice"))
            {
                EditedColumnName = null;
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PackagePrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.PackageQuantity * item.PackagePrice;
                    }
                    item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (EditedColumnName.Equals("colPackagingQty"))
            {
                EditedColumnName = null;
                double.TryParse(value, out double ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PackageQuantity != ite)
                {
                    item.InQuantity = item.PackageQuantity * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //if (item.PackageQuantity > 0)
                    //{
                    //    if (item.PackagePrice == 0)
                    //    {
                    //        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    //    }
                    //    if (item.UnitPrice == 0)
                    //    {
                    //        item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //    }
                    //}
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                }
            }
            else if (EditedColumnName.Equals("colQty"))
            {
                EditedColumnName = null;
                double.TryParse(value, out double ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.InQuantity != ite)
                {
                    item.PackageQuantity = item.InQuantity / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //if (item.PackageQuantity > 0)
                    //{
                    //    if (item.PackagePrice == 0)
                    //    {
                    //        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    //    }
                    //    if (item.UnitPrice == 0)
                    //    {
                    //        item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //    }
                    //}
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                }
            }
            else if (EditedColumnName.Equals("colTotalNotVAT"))
            {
                EditedColumnName = null;
                decimal.TryParse(value, out decimal ite);
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.TotalPriceNotVAT != ite)
                {
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
                    }
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            //KMx: Kiểm tra xem có cho chỉnh sửa NCC không. Nếu đã chọn hàng rồi thì không cho chỉnh sửa NCC (13/01/2015 10:17).
            CheckEnableSupplier();
            EditedColumnName = null;
        }

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
                 if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM)
                {
                    proAlloc.strHienThi = eHCMSResources.Z2520_G1_VPP.ToUpper();
                }
                else
                {
                    proAlloc.strHienThi = eHCMSResources.Z0620_G1_TimHDonNhapHChat.ToUpper();
                }
                proAlloc.InwardInvoiceList.Clear();
                proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                proAlloc.InwardInvoiceList.PageIndex = 0;
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

        private void SearchInwardInvoiceVPP(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                        contract.BeginSearchInwardVPPMedDeptInvoice(SearchCriteria, TypID, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchInwardVPPMedDeptInvoice(out int Totalcount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                    //mo pop up tim
                                    OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                    }
                                    else
                                    {
                                        CurrentInwardVPPMedDeptInvoice = results.FirstOrDefault();
                                        InitDetailInward();
                                        DeepCopyInvoice();
                                        LoadInfoThenSelectedInvoice();
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
                SearchInwardInvoiceVPP(0, Globals.PageSize);
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
                SearchInwardInvoiceVPP(0, Globals.PageSize);
            }
        }

        public void btnSearch()
        {
            SearchInwardInvoiceVPP(0, Globals.PageSize);
        }

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentInwardVPPMedDeptInvoice.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                PharmacyPurchaseOrder_BySupplierID();
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseEditInwardEvent> Members

        public void Handle(DrugDeptCloseEditInwardEvent message)
        {
            if (this.IsActive)
            {
                Load_InwardVPPDetails(CurrentInwardVPPMedDeptInvoice.inviID);
                InwardVPPMedDeptInvoice_GetListCost(CurrentInwardVPPMedDeptInvoice.inviID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentInwardInvoiceCopy.inviID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (CurrentInwardInvoiceCopy.IsForeign)
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_INWARD_VPP_SUPPLIER;
                }
                else
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_INWARD_VPP_SUPPLIER_TRONGNUOC;
                }

                if (V_MedProductType == (long)AllLookupValues.MedProductType.VAN_PHONG_PHAM )
                {
                    proAlloc.LyDo = eHCMSResources.Z2534_G1_PhNhapVPP.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = "";
                }
                if (CurrentInwardVPPMedDeptInvoice != null && CurrentInwardVPPMedDeptInvoice.SelectedStaff != null)
                {
                    proAlloc.StaffFullName = CurrentInwardVPPMedDeptInvoice.SelectedStaff.FullName;
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }
        public void btnPrint()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new ReportServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetInWardVPPInPdfFormat(CurrentInwardVPPMedDeptInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetInWardVPPInPdfFormat(asyncResult);
                            var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                            Globals.EventAggregator.Publish(results);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });
            t.Start();
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchInwardIncoiceEvent> Members

        public void Handle(DrugDeptCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentInwardVPPMedDeptInvoice = message.SelectedInwardInvoice as InwardDrugMedDeptInvoice;
                InitDetailInward();
                DeepCopyInvoice();
                LoadInfoThenSelectedInvoice();

            }
        }

        private void LoadInfoThenSelectedInvoice()
        {
            InwardVPPMedDeptInvoice_GetListCost(CurrentInwardVPPMedDeptInvoice.inviID);
            Load_InwardVPPDetails(CurrentInwardVPPMedDeptInvoice.inviID);
            PharmacyPurchaseOrder_BySupplierID();
        }

        #endregion

        #region ListCost Member

        public void GridListCost_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void InwardVPPMedDeptInvoice_GetListCost(long inviID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardVPPMedDeptInvoice_GetListCost(inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndInwardVPPMedDeptInvoice_GetListCost(asyncResult);
                                CurrentInwardVPPMedDeptInvoice.CostTableMedDeptLists = results.ToObservableCollection();
                                SumTotalPercent();
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

        private void SumTotalPercent()
        {
            TotalPercent = 0;
            if (CurrentInwardVPPMedDeptInvoice != null && CurrentInwardVPPMedDeptInvoice.CostTableMedDeptLists != null)
            {
                for (int i = 0; i < CurrentInwardVPPMedDeptInvoice.CostTableMedDeptLists.Count; i++)
                {
                    TotalPercent += CurrentInwardVPPMedDeptInvoice.CostTableMedDeptLists[i].Rates;
                }
            }
        }
        #endregion

        public void ActualQD_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (CurrentInwardVPPMedDeptInvoice != null && CurrentInwardVPPMedDeptInvoice.ExchangeRates != 0)
                {
                    try
                    {
                        CurrentInwardVPPMedDeptInvoice.TotalPriceActual_QĐ = Convert.ToDecimal((sender as TextBox).Text);
                    }
                    catch
                    {
                        Globals.ShowMessage(eHCMSResources.A0559_G1_Msg_InfoGTriKhHopLe, eHCMSResources.T0074_G1_I);
                    }
                    CurrentInwardVPPMedDeptInvoice.TotalPriceActual = CurrentInwardVPPMedDeptInvoice.TotalPriceActual_QĐ / (decimal)CurrentInwardVPPMedDeptInvoice.ExchangeRates;
                }
            }
        }

        public void Actual_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (CurrentInwardVPPMedDeptInvoice != null)
                {
                    try
                    {
                        CurrentInwardVPPMedDeptInvoice.TotalPriceActual = Convert.ToDecimal((sender as TextBox).Text);
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

        public void BtnDeleteSelItem()
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
        public void chkIsNotVat_Click(object sender, RoutedEventArgs e)
        {
            var chkIsNotVat = sender as CheckBox;
            if ((chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail) != null)
            {
                if (chkIsNotVat.IsChecked == true)
                {
                    if (MessageBox.Show(eHCMSResources.Z2993_G1_ChonKhongThue, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                    {
                        chkIsNotVat.IsChecked = false;
                        (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = false;
                        return;
                    }
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = true;
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).VAT = null;
                }
                else
                {
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = false;
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).VAT = 0;
                }
            }
        }
    }
}
