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
using aEMR.Common.Printing;
using aEMR.Controls;
using aEMR.CommonTasks;
using eHCMSLanguage;
using aEMR.Common;
using Castle.Windsor;
using System.Text;

/*
 * 07082018 #001 TTM: Bổ sung comment
 * 20191110 #002 TTM: BM 0018502: Bổ sung VAT cho nhập xuất của nhà thuốc.
 * 20200330 #003 TTM: BM 0029055: Fix lỗi khi nhập chiết khấu cho dòng nhập thì lưu thông tin chiết khấu không chính xác.
 * 20200403 #004 TTM: BM 0029078: [Nhập hàng] Fix lỗi hạn sử dụng nhỏ hơn ngày sản xuất vẫn cho lưu.
 * 20200411 #005 TTM: BM 0029095: Thêm điều kiện kiểm tra để không lưu thông tin giá bán dv, giá BH và giá cho BNBH < 0
 * 20220806 #006 QTD: Không cho chọn lại NCC khi đã đánh thuốc
 */

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IInwardDrugSupplier)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardDrugSupplierViewModel : Conductor<object>, IInwardDrugSupplier
        , IHandle<PharmacyCloseSearchSupplierEvent>, IHandle<PharmacyCloseSearchInwardIncoiceEvent>
        , IHandle<PharmacyCloseEditInwardEvent>
    {
        public string TitleForm { get; set; }
        public long V_SupplierType = 7200;
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

        [ImportingConstructor]
        public InwardDrugSupplierViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            InitializeInvoiceDrug();
            //FillGroupName();

            InnitSearchCriteria();

            GetAllCurrency(true);
            Coroutine.BeginExecute(GetAllReferenceValues());
            //Coroutine.BeginExecute(DoGetLookup_GoodType());

            IsHideAuSupplier = true;
            IsEnabled = true;
            IsVisibility = Visibility.Collapsed;

            SupplierCriteria = new SupplierSearchCriteria
            {
                V_SupplierType = V_SupplierType
            };

            Authorization();
            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;

            InwardDrugList = new PagedSortableCollectionView<InwardDrug>();
            InwardDrugList.OnRefresh += new EventHandler<RefreshEventArgs>(InwardDrugList_OnRefresh);
            InwardDrugList.PageSize = Globals.PageSize;

            Suppliers = new PagedSortableCollectionView<Supplier>();
            Suppliers.OnRefresh += new EventHandler<RefreshEventArgs>(Suppliers_OnRefresh);
            Suppliers.PageSize = Globals.PageSize;

            ObjPharmacySellPriceProfitScale = new List<PharmacySellPriceProfitScale>();
            if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
            {
                PharmacySellPriceProfitScale_GetList(0, 50, true);
            }
        }

        void Suppliers_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);
        }

        private long inviID = 0;
        void InwardDrugList_OnRefresh(object sender, RefreshEventArgs e)
        {
            InwardDrugDetails_ByID(inviID, InwardDrugList.PageIndex, InwardDrugList.PageSize);
        }

        void RefGenericDrugDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(null, BrandName, RefGenericDrugDetails.PageIndex, RefGenericDrugDetails.PageSize);
        }

        #region Properties Member

        //private ObservableCollection<GroupName> _GroupNames;
        //public ObservableCollection<GroupName> GroupNames
        //{
        //    get
        //    { return _GroupNames; }
        //    set
        //    {
        //        if (_GroupNames != value)
        //        {
        //            _GroupNames = value;
        //            NotifyOfPropertyChange(() => GroupNames);
        //        }
        //    }
        //}

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

        private int _CDC = 5;
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

        private InwardDrugInvoice _CurrentInwardDrugInvoice;
        public InwardDrugInvoice CurrentInwardDrugInvoice
        {
            get
            {
                return _CurrentInwardDrugInvoice;
            }
            set
            {
                if (_CurrentInwardDrugInvoice != value)
                {
                    _CurrentInwardDrugInvoice = value;
                    if (value != null && value.CanEditAndDelete)
                    {
                        IsVisibility = Visibility.Visible;
                    }
                    else
                    {
                        IsVisibility = Visibility.Collapsed;
                    }
                    NotifyOfPropertyChange(() => IsVisibility);
                    NotifyOfPropertyChange(() => CurrentInwardDrugInvoice);
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

        private ObservableCollection<Lookup> _ByUnitSelected;
        public ObservableCollection<Lookup> ByUnitSelected
        {
            get
            {
                return _ByUnitSelected;
            }
            set
            {
                _ByUnitSelected = value;
                NotifyOfPropertyChange(() => ByUnitSelected);
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

        private PagedSortableCollectionView<InwardDrug> _InwardDrugList;
        public PagedSortableCollectionView<InwardDrug> InwardDrugList
        {
            get
            {
                return _InwardDrugList;
            }
            set
            {
                if (_InwardDrugList != value)
                {
                    _InwardDrugList = value;
                    NotifyOfPropertyChange(() => InwardDrugList);
                }
            }
        }

        private InwardDrugInvoice CurrentInwardInvoiceCopy;

        private ObservableCollection<PharmacyPurchaseOrderDetail> _PharmacyPurchaseOrderDetailList;
        public ObservableCollection<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetailList
        {
            get
            {
                return _PharmacyPurchaseOrderDetailList;
            }
            set
            {
                if (_PharmacyPurchaseOrderDetailList != value)
                {
                    _PharmacyPurchaseOrderDetailList = value;
                    NotifyOfPropertyChange(() => PharmacyPurchaseOrderDetailList);
                }
            }
        }

        private PharmacyPurchaseOrderDetail _CurrentPharmacyPurchaseOrderDetail;
        public PharmacyPurchaseOrderDetail CurrentPharmacyPurchaseOrderDetail
        {
            get
            {
                return _CurrentPharmacyPurchaseOrderDetail;
            }
            set
            {
                if (_CurrentPharmacyPurchaseOrderDetail != value)
                {
                    _CurrentPharmacyPurchaseOrderDetail = value;
                    NotifyOfPropertyChange(() => CurrentPharmacyPurchaseOrderDetail);
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

        private ObservableCollection<PharmacyPurchaseOrder> _PharmacyPurchaseOrderList;
        public ObservableCollection<PharmacyPurchaseOrder> PharmacyPurchaseOrderList
        {
            get
            {
                return _PharmacyPurchaseOrderList;
            }
            set
            {
                if (_PharmacyPurchaseOrderList != value)
                {
                    _PharmacyPurchaseOrderList = value;
                    NotifyOfPropertyChange(() => PharmacyPurchaseOrderList);
                }
            }
        }

        //  private ObservableCollection<PharmacyPurchaseOrder> PharmacyPurchaseOrderFirst;

        private InwardDrug _CurrentInwardDrug;
        public InwardDrug CurrentInwardDrug
        {
            get
            {
                return _CurrentInwardDrug;
            }
            set
            {
                if (_CurrentInwardDrug != value)
                {
                    _CurrentInwardDrug = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrug);
                }
            }
        }

        private InwardDrug _CurrentInwardDrugCopy;
        public InwardDrug CurrentInwardDrugCopy
        {
            get
            {
                return _CurrentInwardDrugCopy;
            }
            set
            {
                if (_CurrentInwardDrugCopy != value)
                {
                    _CurrentInwardDrugCopy = value;
                    NotifyOfPropertyChange(() => CurrentInwardDrugCopy);
                }
            }
        }

        //private ObservableCollection<InwardDrug> InwardDrugListCopy;

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

        private ObservableCollection<RefUnit> _units;
        public ObservableCollection<RefUnit> Units
        {
            get
            {
                return _units;
            }
            set
            {
                if (_units != value)
                {
                    _units = value;
                    NotifyOfPropertyChange(() => Units);
                }
            }
        }
        private bool _IsNotCheckInvalid = false;
        public bool IsNotCheckInvalid
        {
            get
            {
                return _IsNotCheckInvalid;
            }
            set
            {
                if (_IsNotCheckInvalid != value)
                {
                    _IsNotCheckInvalid = value;
                    NotifyOfPropertyChange(() => IsNotCheckInvalid);
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
            mNhapHangNCC_CapNhat = mNhapHangNCC_TimHoaDonCu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhapHangTuNhaCungCap,
                                               (int)oPharmacyEx.mNhapHangNCC_CapNhat, (int)ePermission.mView);
            mNhapHangNCC_TimHoaDonCu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhapHangTuNhaCungCap,
                                               (int)oPharmacyEx.mNhapHangNCC_TimHoaDonCu, (int)ePermission.mView);
            mNhapHangNCC_ThongTinDonHang = mNhapHangNCC_TimHoaDonCu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhapHangTuNhaCungCap,
                                               (int)oPharmacyEx.mNhapHangNCC_ThongTinDonHang, (int)ePermission.mView)
                                               || mNhapHangNCC_CapNhat;
            mNhapHangNCC_PhieuMoi = mNhapHangNCC_TimHoaDonCu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhapHangTuNhaCungCap,
                                               (int)oPharmacyEx.mNhapHangNCC_PhieuMoi, (int)ePermission.mView);

            mNhapHangNCC_ReportIn = mNhapHangNCC_TimHoaDonCu = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhapHangTuNhaCungCap,
                                               (int)oPharmacyEx.mNhapHangNCC_ReportIn, (int)ePermission.mView);
        }

        #region checking account

        private bool _mNhapHangNCC_TimHoaDonCu = true;
        private bool _mNhapHangNCC_ThongTinDonHang = true;
        private bool _mNhapHangNCC_PhieuMoi = true;
        private bool _mNhapHangNCC_CapNhat = true;
        private bool _mNhapHangNCC_ReportIn = true;

        public bool mNhapHangNCC_TimHoaDonCu
        {
            get
            {
                return _mNhapHangNCC_TimHoaDonCu;
            }
            set
            {
                if (_mNhapHangNCC_TimHoaDonCu == value)
                    return;
                _mNhapHangNCC_TimHoaDonCu = value;
            }
        }
        public bool mNhapHangNCC_ThongTinDonHang
        {
            get
            {
                return _mNhapHangNCC_ThongTinDonHang;
            }
            set
            {
                if (_mNhapHangNCC_ThongTinDonHang == value)
                    return;
                _mNhapHangNCC_ThongTinDonHang = value;
            }
        }
        public bool mNhapHangNCC_PhieuMoi
        {
            get
            {
                return _mNhapHangNCC_PhieuMoi;
            }
            set
            {
                if (_mNhapHangNCC_PhieuMoi == value)
                    return;
                _mNhapHangNCC_PhieuMoi = value;
            }
        }
        public bool mNhapHangNCC_CapNhat
        {
            get
            {
                return _mNhapHangNCC_CapNhat;
            }
            set
            {
                if (_mNhapHangNCC_CapNhat == value)
                    return;
                _mNhapHangNCC_CapNhat = value;
            }
        }
        public bool mNhapHangNCC_ReportIn
        {
            get
            {
                return _mNhapHangNCC_ReportIn;
            }
            set
            {
                if (_mNhapHangNCC_ReportIn == value)
                    return;
                _mNhapHangNCC_ReportIn = value;
            }
        }

        //private bool _bbtnSearch = true;
        //private bool _bEdit = true;
        //private bool _bAdd = true;
        //private bool _bDelete = true;
        //private bool _bView = true;
        //private bool _bPrint = true;
        //private bool _bReport = true;

        //public bool bbtnSearch
        //{
        //    get
        //    {
        //        return _bbtnSearch;
        //    }
        //    set
        //    {
        //        if (_bbtnSearch == value)
        //            return;
        //        _bbtnSearch = value;
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

        public Button lnkDelete { get; set; }
        public Button lnkDeleteMain { get; set; }
        public Button lnkEdit { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            //lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }
        public void lnkDeleteMain_Loaded(object sender)
        {
            lnkDeleteMain = sender as Button;
            //lnkDeleteMain.Visibility = Globals.convertVisibility(bDelete);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            //lnkEdit.Visibility = Globals.convertVisibility(bEdit);
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
                                CurrentInwardDrugInvoice.SelectedCurrency = Currencies.FirstOrDefault();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        //private IEnumerator<IResult> DoGetLookup_GoodType()
        //{
        //    isLoadingGoodsType = true;
        //    var paymentTypeTask = new LoadLookupListTask(LookupValues.V_GoodsType, false, false);
        //    yield return paymentTypeTask;
        //    CbxGoodsTypes = paymentTypeTask.LookupList.Where(x => x.LookupID != (long)AllLookupValues.V_GoodsType.HANG_NHAP_TU_LUAN_CHUYEN_KHO).ToObservableCollection();
        //    isLoadingGoodsType = false;
        //    yield break;

        //}

        private IEnumerator<IResult> GetAllReferenceValues()
        {
            var goodsType = Globals.AllLookupValueList.Where(x => x.ObjectTypeID == (long)LookupValues.V_GoodsType).ToObservableCollection();
            CbxGoodsTypes = goodsType.Where(x => x.LookupID != (long)AllLookupValues.V_GoodsType.HANG_NHAP_TU_LUAN_CHUYEN_KHO).ToObservableCollection();

            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false, true);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList.Where(x => x.IsMain == true).ToObservableCollection();
            if (StoreCbx != null)
            {
                CurrentInwardDrugInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
                if (StoreCbx.FirstOrDefault().IsVATCreditStorage)
                {
                    CurrentInwardDrugInvoice.IsVATCredit = true;
                }
                else
                {
                    CurrentInwardDrugInvoice.IsVATCredit = false;
                }
            }


            //KMx: Copy bên DM thuốc (22/10/2015 10:40).
            var loadUnit = new LoadUnitListTask(false, false);
            yield return loadUnit;
            Units = loadUnit.RefUnitList;
            yield break;
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show("Bạn có muốn tạo phiếu nhập mới không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            InitializeInvoiceDrug();
            // }
        }

        private void InitializeInvoiceDrug()
        {
            CurrentInwardDrugInvoice = new InwardDrugInvoice
            {
                InvDateInvoice = Globals.ServerDate.Value,
                DSPTModifiedDate = Globals.ServerDate.Value,
                SelectedStaff = GetStaffLogin(),
                StaffID = GetStaffLogin().StaffID,
                IsForeign = false,

                //if (Globals.ConfigList != null)
                //{
                //    CurrentInwardDrugInvoice.VAT = Decimal.Parse(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyDefaultVATInward].ToString(), CultureInfo.InvariantCulture);
                //}
                // Txd 25/05/2014 Replaced ConfigList
                VAT = (decimal)Globals.ServerConfigSection.PharmacyElements.PharmacyDefaultVATInward
            };

            if (Currencies != null)
            {
                CurrentInwardDrugInvoice.SelectedCurrency = Currencies.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                CurrentInwardDrugInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
                if (StoreCbx.FirstOrDefault().IsVATCreditStorage)
                {
                    CurrentInwardDrugInvoice.IsVATCredit = true;
                }
                else
                {
                    CurrentInwardDrugInvoice.IsVATCredit = false;
                }
            }
            IsHideAuSupplier = true;
            if (InwardDrugList != null)
            {
                InwardDrugList.Clear();
            }
            if (auSupplier != null)
            {
                auSupplier.Text = "";
            }
            DeepCopyInvoice();
            InitDetailInward();
        }

        private void InitDetailInward()
        {
            if (PharmacyPurchaseOrderDetailList != null)
            {
                PharmacyPurchaseOrderDetailList.Clear();
                //Kiên: Phải set Collection = null thì khi check vào "Không có trong đơn hàng" mới hiện ra dòng mới.
                PharmacyPurchaseOrderDetailList = null;
            }
            IsEnabled = true;
            if (ckbDH != null)
            {
                ckbDH.IsChecked = false;
            }
        }

        private void DeepCopyInvoice()
        {
            CurrentInwardInvoiceCopy = CurrentInwardDrugInvoice.DeepCopy();
        }

        private void DeleteInwardInvoiceDrug()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginDeleteInwardInvoiceDrug(CurrentInwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginDeleteInwardInvoiceDrug_Pst(CurrentInwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                //int results = contract.EndDeleteInwardInvoiceDrug(asyncResult);
                                int results = contract.EndDeleteInwardInvoiceDrug_Pst(asyncResult);
                                if (results == 0)
                                {
                                    InitializeInvoiceDrug();
                                    MessageBox.Show(eHCMSResources.Z0575_G1_HDDaXoaThanhCong);
                                }
                                else if (results == 1)
                                {
                                    MessageBox.Show(eHCMSResources.Z0576_G1_KgTheXoaViDaXuat, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0577_G1_PhKgTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void btnDeletePhieu(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z1403_G1_CoChacMuonXoaPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DeleteInwardInvoiceDrug();
            }
        }

        public void btnTransferGlobalDrugSystem()
        {
            if (CurrentInwardDrugInvoice == null || CurrentInwardDrugInvoice.SelectedSupplier == null)
            {
                return;
            }
            GlobalsNAV.LoginPharmacyAPI();
            GlobalDrugsSystemAPIResultCode mResult = GlobalsNAV.PharmacyAPIImportInwardDrugInvoice(CurrentInwardDrugInvoice, InwardDrugList.ToList());
            if (mResult != null)
            {
                MessageBox.Show(mResult.mess);
            }
        }

        private void CheckIsPercentAll()
        {
            if (PharmacyPurchaseOrderDetailList != null)
            {
                for (int i = 0; i < PharmacyPurchaseOrderDetailList.Count; i++)
                {
                    PharmacyPurchaseOrderDetailList[i].IsPercent = IsPercentInwardDrug;
                }
            }
        }

        private void UpdateInwardInvoiceDrug()
        {            
            CurrentInwardDrugInvoice.TypID = TypID;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginUpdateInwardInvoiceDrug(CurrentInwardDrugInvoice, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int results = contract.EndUpdateInwardInvoiceDrug(asyncResult);
                                if (results == 0)
                                {
                                    DeepCopyInvoice();
                                    CheckIsPercentAll();
                                    Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
                                    Globals.ShowMessage(eHCMSResources.A0279_G1_Msg_InfoCNhatOK, eHCMSResources.G0442_G1_TBao);
                                }
                                else if (results == 1)
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0582_G1_HDDaTonTai, eHCMSResources.T0074_G1_I);
                                }
                                else
                                {
                                    Globals.ShowMessage(eHCMSResources.Z0577_G1_PhKgTonTai, eHCMSResources.T0074_G1_I);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void btnEdit(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z1404_G1_CoChacMuonSuaPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CheckValidate())
                {
                    if (CurrentInwardDrugInvoice.SelectedCurrency == null)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1420_G1_ChonDViTToan, eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                    if (CurrentInwardDrugInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
                    {
                        if (CurrentInwardDrugInvoice.ExchangeRates <= 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z1421_G1_VuiLongNhapTyGia, eHCMSResources.G0442_G1_TBao);
                            return;
                        }
                    }
                    if (CurrentInwardDrugInvoice.SelectedStorage != null)
                    {
                        CurrentInwardDrugInvoice.StoreID = CurrentInwardDrugInvoice.SelectedStorage.StoreID;
                        UpdateInwardInvoiceDrug();
                    }
                }
            }
        }

        private void CountTotalPrice(decimal TongTienSPChuaVAT
            ,decimal CKTrenSP
            ,decimal TongTienTrenSPDaTruCK
            ,decimal TongCKTrenHoaDon
            ,decimal TongTienHoaDonCoThueNK
            ,decimal TongTienHoaDonCoVAT
            ,decimal TotalVATDifferenceAmount)
        {
            CurrentInwardDrugInvoice.TotalPriceNotVAT = TongTienSPChuaVAT;
            CurrentInwardDrugInvoice.TotalDiscountOnProduct = CKTrenSP;//tong ck tren hoan don
            CurrentInwardDrugInvoice.TotalDiscountInvoice = CKTrenSP + TongCKTrenHoaDon;
            CurrentInwardDrugInvoice.TotalPrice = TongTienSPChuaVAT - (CKTrenSP + TongCKTrenHoaDon);
            CurrentInwardDrugInvoice.TotalHaveCustomTax = TongTienHoaDonCoThueNK;
            CurrentInwardDrugInvoice.TotalPriceVAT = TongTienHoaDonCoVAT;
            CurrentInwardDrugInvoice.TotalVATDifferenceAmount = TotalVATDifferenceAmount;
            CurrentInwardDrugInvoice.TotalVATAmountActual = CurrentInwardDrugInvoice.TotalVATAmount + CurrentInwardDrugInvoice.TotalVATDifferenceAmount;
        }

        private void Load_InwardDrugDetails(long ID)
        {
            inviID = ID;
            InwardDrugList.PageIndex = 0;
            InwardDrugDetails_ByID(CurrentInwardDrugInvoice.inviID, 0, Globals.PageSize);
        }

        private void InwardDrugDetails_ByID(long ID, int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetspInwardDrugDetailsByID(ID, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                        {

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
                                var results = contract.EndGetspInwardDrugDetailsByID(out total
                                        , out TongTienSPChuaVAT
                                        , out CKTrenSP
                                        , out TongTienTrenSPDaTruCK
                                        , out TongCKTrenHoaDon
                                        , out TongTienHoaDonCoThueNK
                                        , out TongTienHoaDonCoVAT
                                        , out TotalVATDifferenceAmount, asyncResult);

                            //load danh sach thuoc theo hoa don 
                            InwardDrugList.Clear();
                                InwardDrugList.TotalItemCount = total;
                                if (results != null)
                                {
                                    foreach (InwardDrug p in results)
                                    {
                                        InwardDrugList.Add(p);
                                    }
                                }
                                if (InwardDrugList != null && InwardDrugList.Count > 0)
                                {
                                    IsHideAuSupplier = false;
                                }
                                else
                                {
                                    IsHideAuSupplier = true;
                                }
                            //tinh tong tien 
                            CountTotalPrice(TongTienSPChuaVAT, CKTrenSP, TongTienTrenSPDaTruCK, TongCKTrenHoaDon, TongTienHoaDonCoThueNK, TongTienHoaDonCoVAT, TotalVATDifferenceAmount);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void PharmacyPurchaseOrder_BySupplierID()
        {
            if (CurrentInwardDrugInvoice.SelectedSupplier != null)
            {
                PharmacyPurchaseOrder_BySupplierID(CurrentInwardDrugInvoice.SelectedSupplier.SupplierID);
            }
            else
            {
                if (PharmacyPurchaseOrderList != null)
                {
                    PharmacyPurchaseOrderList.Clear();
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
                        contract.BeginPharmacyPurchaseOrder_BySupplierID(SupplierID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndPharmacyPurchaseOrder_BySupplierID(asyncResult);
                                PharmacyPurchaseOrderList = results.ToObservableCollection();
                                //if (IsHideAuSupplier)
                                //{
                                //    if (PharmacyPurchaseOrderList == null || PharmacyPurchaseOrderList.Count == 0)
                                //    {
                                //        Globals.ShowMessage("Không đơn đặt hàng cho NCC này", eHCMSResources.G0442_G1_TBao);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInwardInvoiceDrugByID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentInwardDrugInvoice = contract.EndGetInwardInvoiceDrugByID(asyncResult);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void AddInwardInvoiceDrug()
        {
            CurrentInwardDrugInvoice.TypID = TypID;
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginAddInwardInvoiceDrug(CurrentInwardDrugInvoice, IsNotCheckInvalid, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                long ID;
                                int results = contract.EndAddInwardInvoiceDrug(out ID, asyncResult);
                                if (results == 0)
                                {
                                    MessageBox.Show(eHCMSResources.Z1413_G1_ThemTTinHDonThCong, eHCMSResources.G0442_G1_TBao);
                                    IsNotCheckInvalid = false;
                                    GetInwardInvoiceDrugByID(ID);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.Z0584_G1_SoHDDaTonTai, eHCMSResources.G0442_G1_TBao);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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

        public void ByUnitSelected_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            cb.ItemsSource = ByUnitSelected;
            cb.SelectedIndex = 0;
        }

        private bool CheckValidate()
        {
            if (CurrentInwardDrugInvoice == null)
            {
                return false;
            }
            return CurrentInwardDrugInvoice.Validate();
        }

        public void btnAdd(object sender, RoutedEventArgs e)
        {
            //nhap thong tin phieu nhap vao database
            if (CheckValidate())
            {
                if ((CurrentInwardDrugInvoice.VAT < 1 && CurrentInwardDrugInvoice.VAT > 0) || CurrentInwardDrugInvoice.VAT >= 2)
                {
                    Globals.ShowMessage(eHCMSResources.K0262_G1_VATKhongHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▼===== 20200402 TTM: Gỡ kiểm tra = 2 ra vì nếu để vào thì sẽ không bao giờ có chiết khấu 100%
                if ((CurrentInwardDrugInvoice.DiscountingByPercent < 1 && CurrentInwardDrugInvoice.DiscountingByPercent > 0)/* || CurrentInwardDrugInvoice.DiscountingByPercent >= 2*/)
                {
                    Globals.ShowMessage(eHCMSResources.A0072_G1_CKKhHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                //▲=====
                if (CurrentInwardDrugInvoice.SelectedCurrency == null)
                {
                    Globals.ShowMessage(eHCMSResources.Z1420_G1_ChonDViTToan, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugInvoice.SelectedCurrency.CurrencyID != (long)AllLookupValues.CurrencyTable.VND)
                {
                    if (CurrentInwardDrugInvoice.ExchangeRates <= 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z1421_G1_VuiLongNhapTyGia, eHCMSResources.G0442_G1_TBao);
                        return;
                    }
                }
                if (CurrentInwardDrugInvoice.SelectedStorage != null)
                {
                    CurrentInwardDrugInvoice.StoreID = CurrentInwardDrugInvoice.SelectedStorage.StoreID;
                }
                AddInwardInvoiceDrug();
            }
        }

        private void DeletePharmacyPurchaseOrderDetail(PharmacyPurchaseOrderDetail item)
        {
            PharmacyPurchaseOrderDetailList.Remove(item);
            //▼====== #006
            CheckEnableSupplier();
            //▲====== #006
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentPharmacyPurchaseOrderDetail != null)
                {
                    DeletePharmacyPurchaseOrderDetail(CurrentPharmacyPurchaseOrderDetail);
                }
            }
        }

        public void lnkRefGenericDrugDetail_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyPurchaseOrderDetail == null || CurrentPharmacyPurchaseOrderDetail.DrugID <= 0)
            {
                MessageBox.Show(eHCMSResources.A0645_G1_Msg_InfoKhCoHgDeShow, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            GetRefGenericDrugDetail(CurrentPharmacyPurchaseOrderDetail.DrugID, null);
        }

        public void lnkRefGenericDrugDetail_Inward_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrug == null || CurrentInwardDrug.DrugID == null || CurrentInwardDrug.DrugID <= 0)
            {
                return;
            }

            GetRefGenericDrugDetail(CurrentInwardDrug.DrugID.GetValueOrDefault(), CurrentInwardDrug.DrugVersionID);
        }

        private void GetRefGenericDrugDetail(long drugID, long? drugVersionID)
        {
            this.ShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetRefGenericDrugDetail(drugID, drugVersionID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                RefGenericDrugDetail results = contract.EndGetRefGenericDrugDetail(asyncResult);
                                if (results != null)
                                {
                                    ShowRefGenMedProductDetails(results);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0638_G1_Msg_InfoKhCoData, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void ShowRefGenMedProductDetails(RefGenericDrugDetail product)
        {
            if (product == null)
            {
                return;
            }

            void onInitDlg(IRefGenDrug_Add_V2 proAlloc)
            {
                proAlloc.NewDrug = product.DeepCopy();
                proAlloc.IsAdd = false;
                proAlloc.IsEdit = true;
                proAlloc.CanEdit = false;
                proAlloc.TitleForm = eHCMSResources.G0691_G1_TTinThuoc;

                //proAlloc.Countries = Countries;
                //proAlloc.FamilyTherapies = FamilyTherapies;
                //proAlloc.PharmaceuticalCompanies = PharmaceuticalCompanies;
                //proAlloc.RefGenDrugBHYT_Categorys = RefGenDrugBHYT_Categorys;
                //proAlloc.RefGenericDrugCategory_2s = RefGenericDrugCategory_2s;
                //proAlloc.Suppliers = Suppliers;
                proAlloc.Units = Units;
            }
            void onInitDlgNew(IRefGenDrug_Add_V2New proAlloc)
            {
                proAlloc.NewDrug = product.DeepCopy();
                proAlloc.IsAdd = false;
                proAlloc.IsEdit = true;
                proAlloc.CanEdit = false;
                proAlloc.TitleForm = eHCMSResources.G0691_G1_TTinThuoc;

                //proAlloc.Countries = Countries;
                //proAlloc.FamilyTherapies = FamilyTherapies;
                //proAlloc.PharmaceuticalCompanies = PharmaceuticalCompanies;
                //proAlloc.RefGenDrugBHYT_Categorys = RefGenDrugBHYT_Categorys;
                //proAlloc.RefGenericDrugCategory_2s = RefGenericDrugCategory_2s;
                //proAlloc.Suppliers = Suppliers;
                proAlloc.Units = Units;
            }

            if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
            {
                GlobalsNAV.ShowDialog<IRefGenDrug_Add_V2New>(onInitDlgNew);
            }
            else
            {
                GlobalsNAV.ShowDialog<IRefGenDrug_Add_V2>(onInitDlg);
            }
            //GlobalsNAV.ShowDialog<IRefGenDrug_Add_V2>(onInitDlg);
        }

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void rdtTrongNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentInwardDrugInvoice.IsForeign = false;
        }

        public void rdtNgoaiNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentInwardDrugInvoice.IsForeign = true;
        }

        private void PharmacyPurchaseOrderDetail_ByParentID(object Item)
        {
            PharmacyPurchaseOrder CurrentOrder = Item as PharmacyPurchaseOrder;
            PharmacyPurchaseOrderDetail_ByParentID(CurrentOrder.PharmacyPoID);
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
            if (PharmacyPurchaseOrderDetailList != null)
            {
                for (int i = 0; i < PharmacyPurchaseOrderDetailList.Count; i++)
                {
                    PharmacyPurchaseOrderDetailList[i].TotalPriceNotVAT = PharmacyPurchaseOrderDetailList[i].UnitPrice * (decimal)PharmacyPurchaseOrderDetailList[i].InQuantity;
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail != null)
                    {
                        if (PharmacyPurchaseOrderDetailList[i].InQuantity > 0 && PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault() > 0)
                        {
                            PharmacyPurchaseOrderDetailList[i].PackageQuantity = PharmacyPurchaseOrderDetailList[i].InQuantity / PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault();
                        }
                        else if (PharmacyPurchaseOrderDetailList[i].PackageQuantity > 0)
                        {
                            if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault() > 0)
                            {
                                PharmacyPurchaseOrderDetailList[i].InQuantity = PharmacyPurchaseOrderDetailList[i].PackageQuantity * PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault();
                            }
                        }

                        if (PharmacyPurchaseOrderDetailList[i].UnitPrice > 0)
                        {
                            PharmacyPurchaseOrderDetailList[i].PackagePrice = PharmacyPurchaseOrderDetailList[i].UnitPrice * PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault();
                        }
                        else if (PharmacyPurchaseOrderDetailList[i].PackagePrice > 0)
                        {
                            if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault() > 0)
                            {
                                PharmacyPurchaseOrderDetailList[i].UnitPrice = PharmacyPurchaseOrderDetailList[i].PackagePrice / PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.UnitPackaging.GetValueOrDefault();
                            }
                        }
                    }
                }
            }
        }

        private void PharmacyPurchaseOrderDetail_ByParentID(long PharmacyPoID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyPurchaseOrderDetail_ByParentID(PharmacyPoID, 1, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndPharmacyPurchaseOrderDetail_ByParentID(asyncResult);
                                ClearPharmacyPurchaseOrderDetailList();
                                PharmacyPurchaseOrderDetailList = results.ToObservableCollection();
                                //so sanh ds load len voi ds ngay ben duoi
                                //GetPharmacyPurchaseOrderDetailList(PharmacyPoID);
                                CheckIsPercentAll();
                                //tinh tien 
                                TinhTienDonDatHang();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void ClearPharmacyPurchaseOrderDetailList()
        {
            if (PharmacyPurchaseOrderDetailList != null)
            {
                PharmacyPurchaseOrderDetailList.Clear();
            }
        }

        public void cbxChonDH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxChonDH = sender as ComboBox;
            if (cbxChonDH != null)
            {
                if (cbxChonDH.SelectedItem != null)
                {
                    PharmacyPurchaseOrderDetail_ByParentID(cbxChonDH.SelectedItem);
                }
                else
                {
                    ClearPharmacyPurchaseOrderDetailList();
                }
            }
        }

        private void IsChangedItem()
        {
            if (CurrentInwardDrugInvoice.CurrencyID != CurrentInwardInvoiceCopy.CurrencyID || CurrentInwardDrugInvoice.ExchangeRates != CurrentInwardInvoiceCopy.ExchangeRates || CurrentInwardDrugInvoice.VAT != CurrentInwardInvoiceCopy.VAT || CurrentInwardDrugInvoice.IsForeign != CurrentInwardInvoiceCopy.IsForeign || CurrentInwardDrugInvoice.Notes != CurrentInwardInvoiceCopy.Notes)
            {
                if (MessageBox.Show(eHCMSResources.Z0587_G1_CoMuonCNhatHD, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CheckValidate())
                    {
                        UpdateInwardInvoiceDrug();
                    }
                    else
                    {
                        CurrentInwardDrugInvoice = CurrentInwardInvoiceCopy;
                    }
                }
                else
                {
                    CurrentInwardDrugInvoice = CurrentInwardInvoiceCopy;
                }
            }
        }

        private bool CheckValidationData()
        {
            if (PharmacyPurchaseOrderDetailList == null || PharmacyPurchaseOrderDetailList.Count == 0)
            {
                MessageBox.Show(eHCMSResources.Z0588_G1_KgCoDLieuNhap);
                return false;
            }

            if (PharmacyPurchaseOrderDetailList.Count <= 0 || PharmacyPurchaseOrderDetailList[0].DrugID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0452_G1_NhapThuoc);
                return false;
            }

            string TB = "";
            StringBuilder sbForCheckVAT = new StringBuilder();
            for (int i = 0; i < PharmacyPurchaseOrderDetailList.Count; i++)
            {
                if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail != null && PharmacyPurchaseOrderDetailList[i].DrugID > 0)
                {
                    if (PharmacyPurchaseOrderDetailList[i].DrugID == 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0785_G1_Msg_InfoPhaiChonThuoc));
                        return false;
                    }
                    else if (PharmacyPurchaseOrderDetailList[i].InQuantity <= 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0789_G1_Msg_InfoSLgNhapLonHon0));
                        return false;
                    }
                    else if (PharmacyPurchaseOrderDetailList[i].InBatchNumber == null || PharmacyPurchaseOrderDetailList[i].InBatchNumber.Length == 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0784_G1_Msg_InfoChuaNhapSoLo));
                        return false;
                    }
                    else if (PharmacyPurchaseOrderDetailList[i].InExpiryDate == null)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0782_G1_Msg_InfoChuaNhapHSD));
                        return false;
                    }
                    //▼===== #004
                    //else if (PharmacyPurchaseOrderDetailList[i].InExpiryDate <= Globals.ServerDate.Value)
                    //{
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0787_G1_Msg_InfoHSDLonHonNgHTai));
                    //    return false;
                    //}
                    ////else if (PharmacyPurchaseOrderDetailList[i].InProductionDate == null)
                    ////{
                    ////    MessageBox.Show("Lỗi : dòng thứ " + (i + 1).ToString() + " bạn chưa nhập ngày sản xuất.");
                    ////    return false;
                    ////}
                    //else if (PharmacyPurchaseOrderDetailList[i].InProductionDate >= Globals.ServerDate.Value)
                    //{
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0590_G1_NgSXPhaiNhoHonNgHTai));
                    //    return false;
                    //}
                    if (PharmacyPurchaseOrderDetailList[i].InExpiryDate < PharmacyPurchaseOrderDetailList[i].InProductionDate)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3004_G1_HSDNhoHonNSX));
                        return false;
                    }
                    //▲===== #004
                    if (PharmacyPurchaseOrderDetailList[i].UnitPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0591_G1_DGiaKgDuocNhoHon0));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].UnitPrice == 0 && PharmacyPurchaseOrderDetailList[i].V_GoodsType != null && PharmacyPurchaseOrderDetailList[i].V_GoodsType.LookupID == (long)AllLookupValues.V_GoodsType.HANGMUA)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0556_G1_Msg_InfoGiaMuaLonHon0));
                        return false;
                    }
                    if ((PharmacyPurchaseOrderDetailList[i].DiscountingByPercent < 1 && PharmacyPurchaseOrderDetailList[i].DiscountingByPercent > 0) || (PharmacyPurchaseOrderDetailList[i].DiscountingByPercent > 2))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0072_G1_CKKhHopLe));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.NormalPrice <= 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.A0786_G1_Msg_InfoCNhatBGiaBan));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.NormalPrice < PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.PriceForHIPatient)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.HIAllowedPrice > PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.PriceForHIPatient)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z0593_G1_GiaBHChoPhep));
                        return false;
                    }
                    //▼===== #005
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.NormalPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3013_G1_GiaBanDVKhongDuocNhoHon0));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.HIAllowedPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3012_G1_GiaBHKhongDuocNhoHon0));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.PriceForHIPatient < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3011_G1_GiaBNBHKhongDuocNhoHon0));
                        return false;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].TotalPriceNotVAT < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z3014_G1_ThanhTienKhongDuocNhoHon0));
                        return false;
                    }
                    //▲===== #005
                    ////▼===== #002: Do giá trị của VAT lấy ra từ danh mục và có luật là VAT trong danh mục và VAT của phiếu nhập phải bằng nhau.
                    ////             Nếu không thì sẽ xảy ra tình trạng phiếu nhập là 5% mà từng dòng là 10%. Khi nào cần thiếu cho 1 phiếu nhập nhiều
                    ////             VAT thì sửa kiểm tra lại.
                    //if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.VAT != (CurrentInwardDrugInvoice.VAT - 1))
                    //{
                    //    MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2912_G1_VATSaiSoVoiInvoice));
                    //    return false;
                    //}
                    ////▲===== #002
                    if (PharmacyPurchaseOrderDetailList[i].UnitPrice != PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.InBuyingPrice && PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.InBuyingPrice > 0)
                    {
                        TB += PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.BrandName + Environment.NewLine;
                    }
                    if (PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.VAT < 0
                        || PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.VAT > 1
                        || (!PharmacyPurchaseOrderDetailList[i].IsNotVat && PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.VAT == null))
                    {
                        sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2991_G1_VATKhongHopLe));
                    }
                    else if (PharmacyPurchaseOrderDetailList[i].IsNotVat && PharmacyPurchaseOrderDetailList[i].RefGenericDrugDetail.VAT != null)
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

            return true;
        }

        private void InwardDrug_InsertList()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginInwardDrug_InsertList(PharmacyPurchaseOrderDetailList.ToList(), CurrentInwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndInwardDrug_InsertList(asyncResult);
                                Globals.ShowMessage(eHCMSResources.Z0594_G1_NhapHangThCong, eHCMSResources.G0442_G1_TBao);
                                ClearPharmacyPurchaseOrderDetailList();
                                PharmacyPurchaseOrder_BySupplierID();
                                if (!IsEnabled)
                                {
                                    AddBlankRow();
                                }
                                if (CurrentInwardDrugInvoice != null)
                                {
                                    Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void btnNhapHang(object sender, RoutedEventArgs e)
        {
            IsChangedItem();
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
        }

        private bool CheckBlankRow()
        {
            if (PharmacyPurchaseOrderDetailList != null)
            {
                for (int i = 0; i < PharmacyPurchaseOrderDetailList.Count; i++)
                {
                    if (PharmacyPurchaseOrderDetailList[i].DrugID == 0)
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
                if (PharmacyPurchaseOrderDetailList == null)
                {
                    PharmacyPurchaseOrderDetailList = new ObservableCollection<PharmacyPurchaseOrderDetail>();
                }
                PharmacyPurchaseOrderDetail p = new PharmacyPurchaseOrderDetail();
                Lookup item = new Lookup();
                item.LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA;
                item.ObjectValue = eHCMSResources.Z0595_G1_HangMua;
                p.V_GoodsType = item;
                p.IsPercent = IsPercentInwardDrug;
                p.IsUnitPackage = true;
                PharmacyPurchaseOrderDetailList.Add(p);
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
        }

        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
        }

        private void ReadOnlyColumnFalse()
        {
            if (GridSuppliers != null)
            {
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
            PharmacyPurchaseOrderDetail P = item as PharmacyPurchaseOrderDetail;
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
            if (Au != null && CurrentPharmacyPurchaseOrderDetail != null)
            {
                if (Au.SelectedItem != null)
                {
                    CurrentPharmacyPurchaseOrderDetail.SelectedShelfDrugLocation = Au.SelectedItem as RefShelfDrugLocation;
                }
                else
                {
                    CurrentPharmacyPurchaseOrderDetail.SdlDescription = Au.Text;
                }
            }
        }

        #endregion

        #region Auto for Drug Member


        private string BrandName;

        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetails;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetails
        {
            get
            {
                return _RefGenericDrugDetails;
            }
            set
            {
                if (_RefGenericDrugDetails != value)
                {
                    _RefGenericDrugDetails = value;
                }
                NotifyOfPropertyChange(() => RefGenericDrugDetails);
            }
        }

        //KMx: Trường hợp nhập thuốc A, số lượng quy cách là 50. Sau đó đổi thuốc A thành thuốc B, số lượng quy cách là 28.
        //Nhưng thuốc B vẫn lấy tính theo số lượng quy cách của thuốc A => Tính ra số lượng lẻ và giá vốn bị sai (28/04/2014 17:05)
        private void ResetCurrentPharmacyPurchaseOrderDetail()
        {
            if (CurrentPharmacyPurchaseOrderDetail != null)
            {
                CurrentPharmacyPurchaseOrderDetail.InProductionDate = null;
                CurrentPharmacyPurchaseOrderDetail.InBatchNumber = null;
                CurrentPharmacyPurchaseOrderDetail.InExpiryDate = null;
                CurrentPharmacyPurchaseOrderDetail.PackageQuantity = 0;
                CurrentPharmacyPurchaseOrderDetail.InQuantity = 0;
                Lookup item = new Lookup();
                item.LookupID = (long)AllLookupValues.V_GoodsType.HANGMUA;
                item.ObjectValue = "Hàng mua";
                CurrentPharmacyPurchaseOrderDetail.V_GoodsType = item;
                CurrentPharmacyPurchaseOrderDetail.IsPercent = IsPercentInwardDrug;
                CurrentPharmacyPurchaseOrderDetail.IsUnitPackage = false;
                CurrentPharmacyPurchaseOrderDetail.PackagePrice = 0;
                CurrentPharmacyPurchaseOrderDetail.UnitPrice = 0;
                CurrentPharmacyPurchaseOrderDetail.TotalPriceNotVAT = 0;
                CurrentPharmacyPurchaseOrderDetail.SdlDescription = null;
                CurrentPharmacyPurchaseOrderDetail.DiscountingByPercent = 0;
                CurrentPharmacyPurchaseOrderDetail.Discounting = 0;
                CurrentPharmacyPurchaseOrderDetail.NoPrint = false;
                CurrentPharmacyPurchaseOrderDetail.SdlID = null;
                CurrentPharmacyPurchaseOrderDetail.SelectedShelfDrugLocation = null;
            }
        }

        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            long? SupplierID = 0;
            if (CurrentInwardDrugInvoice != null && CurrentInwardDrugInvoice.SelectedSupplier != null)
            {
                SupplierID = CurrentInwardDrugInvoice.SelectedSupplier.SupplierID;
            }
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_AutoPaging(IsCode, Name, SupplierID, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var ListUnits = contract.EndSearchRefDrugGenericDetails_AutoPaging(out totalCount, asyncResult);
                            if (IsCode.GetValueOrDefault())
                            {
                                if (ListUnits != null && ListUnits.Count > 0)
                                {
                                    if (CurrentPharmacyPurchaseOrderDetail == null)
                                    {
                                        CurrentPharmacyPurchaseOrderDetail = new PharmacyPurchaseOrderDetail();
                                    }
                                    else
                                    {
                                        ResetCurrentPharmacyPurchaseOrderDetail();
                                    }
                                    CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail = ListUnits.FirstOrDefault();
                                    CurrentPharmacyPurchaseOrderDetail.UnitPrice = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.InBuyingPrice;
                                    //InBuyingPriceActual = Math.Round(CurrentPharmacyPurchaseOrderDetail.UnitPrice * (CurrentInwardDrugInvoice.VAT == 0 ? 1 : CurrentInwardDrugInvoice.VAT));

 

                                    CurrentPharmacyPurchaseOrderDetail.PackagePrice = CurrentPharmacyPurchaseOrderDetail.UnitPrice * CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault();
                                    CurrentPharmacyPurchaseOrderDetail.DrugCode = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail != null ? CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.DrugCode : "";
                                    SetPriceByPriceScale(CurrentPharmacyPurchaseOrderDetail);
                                    CurrentPharmacyPurchaseOrderDetail.IsNotVat = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.IsNotVat;
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                }
                            }
                            else
                            {
                                if (ListUnits != null)
                                {
                                    RefGenericDrugDetails.Clear();
                                    RefGenericDrugDetails.TotalItemCount = totalCount;
                                    RefGenericDrugDetails.ItemCount = totalCount;
                                    foreach (RefGenericDrugDetail p in ListUnits)
                                    {
                                        RefGenericDrugDetails.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => RefGenericDrugDetails);
                                }
                                AuDrug.ItemsSource = RefGenericDrugDetails;
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
            RefGenericDrugDetails.PageIndex = 0;
            SearchRefDrugGenericDetails_AutoPaging(null, e.Parameter, 0, RefGenericDrugDetails.PageSize);
        }

        public void acbDrug_Loaded(object sender, RoutedEventArgs e)
        {
            //(sender as AutoCompleteBox).ItemsSource = RefGenericDrugDetails;
            AuDrug = sender as AutoCompleteBox;
        }

        public void acbDrug_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (AuDrug != null)
            {
                if (CurrentPharmacyPurchaseOrderDetail != null)
                {
                    ResetCurrentPharmacyPurchaseOrderDetail();
                    CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail = (sender as AutoCompleteBox).SelectedItem as RefGenericDrugDetail;
                    if (CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail != null)
                    {
                        CurrentPharmacyPurchaseOrderDetail.UnitPrice = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.InBuyingPrice;
                        CurrentPharmacyPurchaseOrderDetail.PackagePrice = CurrentPharmacyPurchaseOrderDetail.UnitPrice * CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault();
                        CurrentPharmacyPurchaseOrderDetail.DrugCode = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.DrugCode;
                        //InBuyingPriceActual = Math.Round(CurrentPharmacyPurchaseOrderDetail.UnitPrice * (CurrentInwardDrugInvoice.VAT == 0 ? 1 : CurrentInwardDrugInvoice.VAT));

                        if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                        {
                            SetPriceByPriceScale(CurrentPharmacyPurchaseOrderDetail);
                        }

                        CurrentPharmacyPurchaseOrderDetail.IsNotVat = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.IsNotVat;
                    }
                    else
                    {
                        CurrentPharmacyPurchaseOrderDetail.DrugCode = "";
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

        private PagedSortableCollectionView<Supplier> _Suppliers;
        public PagedSortableCollectionView<Supplier> Suppliers
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

        private Supplier _Supplierstest;
        public Supplier Supplierstest
        {
            get
            {
                return _Supplierstest;
            }
            set
            {
                if (_Supplierstest != value)
                {
                    _Supplierstest = value;
                }
                NotifyOfPropertyChange(() => Supplierstest);
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
                    contract.BeginSearchSupplierAutoPaging(SupplierCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndSearchSupplierAutoPaging(out totalCount, asyncResult);
                            // Suppliers = ListUnits.ToObservableCollection();
                            if (ListUnits != null)
                            {
                                Suppliers.Clear();
                                Suppliers.TotalItemCount = totalCount;
                                foreach (Supplier p in ListUnits)
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
            Action<ISuppliers> onInitDlg = delegate (ISuppliers proAlloc)
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg, null, false, true);
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (CurrentInwardDrugInvoice != null)
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
                //using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                try
                {
                    using (var serviceFactory = new PharmacySaleAndOutwardClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        //contract.BeginDeleteInwardDrug(InID, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginDeleteInwardDrug_Pst(InID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                            //int results = contract.EndDeleteInwardDrug(asyncResult);
                            int results = contract.EndDeleteInwardDrug_Pst(asyncResult);
                                if (results == 0)
                                {
                                    Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
                                }
                                else if (results == 1)
                                {
                                    MessageBox.Show(eHCMSResources.K0055_G1_ThuocKhongTheXoa, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.K0058_G1_ThuocKhongTonTai, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();

        }

        public void lnkDeleteMain_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugInvoice.CanEditAndDelete)
            {
                if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CurrentInwardDrug != null)
                    {
                        DeleteInwardDrug(CurrentInwardDrug.InID);
                    }
                }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0596_G1_ThuocDaKChuyenTDK, eHCMSResources.G0442_G1_TBao);
            }
        }

        private void CopyCurrentInwardDrug()
        {
            CurrentInwardDrugCopy = CurrentInwardDrug.DeepCopy();
        }

        public void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugInvoice.PharmacySupplierPaymentReqID != null && CurrentInwardDrugInvoice.PharmacySupplierPaymentReqID > 0)
            {
                MessageBox.Show(eHCMSResources.Z0597_G1_PhNhapDaTToan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
            }
            else
            {
                IsChangedItem();
                CopyCurrentInwardDrug();
                void onInitDlg(IEditInwardDrug vmEditInwardDrug)
                {
                    vmEditInwardDrug.CurrentInwardDrugCopy = CurrentInwardDrug.DeepCopy();
                    vmEditInwardDrug.SetValueForProperty();

                    if (CurrentInwardDrugInvoice.CheckedPoint)
                    {
                        vmEditInwardDrug.CurrentInwardDrugCopy.IsCanEdit = false;
                        MessageBox.Show(eHCMSResources.K0047_G1_ThuocDaKCTonDK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    }
                }
                GlobalsNAV.ShowDialog<IEditInwardDrug>(onInitDlg);
            }
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
            //if (GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) is AxTextBox tbl)
            //{
            //    value = tbl.Text;
            //}
            var mAxTextBox = e.EditingElement.GetChildrenByType<AxTextBox>().FirstOrDefault();
            if (mAxTextBox != null)
            {
                value = mAxTextBox.Text;
            }
        }
        private PharmacyPurchaseOrderDetail EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        //▼===== #001
        //Thay đổi cách thức lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colCode") => dùng e.Column.Equals(GridSuppliers.GetColumnByName("colCode")).
        public void grdRequestDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = (e.Row.DataContext as PharmacyPurchaseOrderDetail);
            EditedColumnName = null;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colCode")))
            {
                PharmacyPurchaseOrderDetail item = e.Row.DataContext as PharmacyPurchaseOrderDetail;
                if (item != null && !string.IsNullOrEmpty(item.DrugCode) && value != item.DrugCode)
                {
                    SearchRefDrugGenericDetails_AutoPaging(true, item.DrugCode, 0, 1);
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
            {
                EditedColumnName = null;
                Decimal.TryParse(value, out decimal ite);
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenericDrugDetail != null && item.UnitPrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    item.PackagePrice = item.UnitPrice * item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                    if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                    {
                        SetPriceByPriceScale(item);
                    }
                    if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagePrice")))
            {
                decimal.TryParse(value, out decimal ite);
                EditedColumnName = null;
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenericDrugDetail != null && item.PackagePrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.PackageQuantity * item.PackagePrice;
                    }
                    item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                    if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                    {
                        SetPriceByPriceScale(item);
                    }
                    if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colPackagingQty")))
            {
                double.TryParse(value, out double ite);
                EditedColumnName = null;
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenericDrugDetail != null && item.PackageQuantity != ite)
                {
                    item.InQuantity = item.PackageQuantity * item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                    if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                    {
                        SetPriceByPriceScale(item);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colQty")))
            {
                double.TryParse(value, out double ite);
                EditedColumnName = null;
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenericDrugDetail != null && item.InQuantity != ite)
                {
                    item.PackageQuantity = item.InQuantity / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.InQuantity * item.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                    if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                    {
                        SetPriceByPriceScale(item);
                    }
                }
            }
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colTotalNotVAT")))
            {
                decimal.TryParse(value, out decimal ite);
                EditedColumnName = null;
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenericDrugDetail != null && item.TotalPriceNotVAT != ite)
                {
                    if (item.PackageQuantity > 0)
                    {
                        if (item.PackagePrice == 0)
                        {
                            item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                        }
                        if (item.UnitPrice == 0)
                        {
                            item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                        }
                        if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                        {
                            SetPriceByPriceScale(item);
                        }
                    }
                    if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            //▼===== #003
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colDiscount")))
            {
                EditedColumnName = null;
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
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
            //▲===== #003
            else if (e.Column.Equals(GridSuppliers.GetColumnByName("colVAT")))
            {
                EditedColumnName = null;
                PharmacyPurchaseOrderDetail item = EditedDetailItem;
                if (item != null)
                {
                    if (Globals.ServerConfigSection.PharmacyElements.CalForPriceProfitScale)
                    {
                        SetPriceByPriceScale(item);
                    }
                }
            }
            if (!IsEnabled)
            {
                if (e.Row.GetIndex() == (PharmacyPurchaseOrderDetailList.Count - 1) && e.EditAction == DataGridEditAction.Commit)
                {
                    //▼====== #006
                    CheckEnableSupplier();
                    //▲====== #006
                    AddBlankRow();
                }
            }
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

        public void OpenPopUpSearchInwardInvoice(IList<InwardDrugInvoice> results, int Totalcount)
        {
            //mo pop up tim
            IInwardDrugSupplierSearch DialogView = Globals.GetViewModel<IInwardDrugSupplierSearch>();
            DialogView.SearchCriteria = SearchCriteria.DeepCopy();
            DialogView.TypID = TypID;
            DialogView.pageTitle = eHCMSResources.N0207_G1_NhapHgTuNCC;
            DialogView.InwardInvoiceList.Clear();
            DialogView.InwardInvoiceList.TotalItemCount = Totalcount;
            DialogView.InwardInvoiceList.PageIndex = 0;
            DialogView.InwardInvoiceList.PageSize = 20;
            if (results != null && results.Count > 0)
            {
                foreach (InwardDrugInvoice p in results)
                {
                    DialogView.InwardInvoiceList.Add(p);
                }
            }
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
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
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardInvoiceDrug(SearchCriteria, TypID, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Totalcount;
                                var results = contract.EndSearchInwardInvoiceDrug(out Totalcount, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                    //mo pop up tim
                                    OpenPopUpSearchInwardInvoice(results.ToList(), Totalcount);
                                    }
                                    else
                                    {
                                        CurrentInwardDrugInvoice = results.FirstOrDefault();
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                SearchInwardInvoiceDrug(0, 20);
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
                SearchInwardInvoiceDrug(0, 20);
            }
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            SearchInwardInvoiceDrug(0, 20);
        }

        private List<PharmacySellPriceProfitScale> _ObjPharmacySellPriceProfitScale;
        public List<PharmacySellPriceProfitScale> ObjPharmacySellPriceProfitScale
        {
            get { return _ObjPharmacySellPriceProfitScale; }
            set
            {
                _ObjPharmacySellPriceProfitScale = value;
                NotifyOfPropertyChange(() => ObjPharmacySellPriceProfitScale);
            }
        }

        private void PharmacySellPriceProfitScale_GetList(int PageIndex, int PageSize, bool CountTotal)
        {
            ObjPharmacySellPriceProfitScale.Clear();
            Globals.EventAggregator.Publish(new BusyEvent() { IsBusy = true, Message = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0715_DSCgThuc) });
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyDrugServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginPharmacySellPriceProfitScale_GetList_Paging(true, PageIndex, PageSize, "", CountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int Total = 0;
                            IList<PharmacySellPriceProfitScale> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndPharmacySellPriceProfitScale_GetList_Paging(out Total, asyncResult);
                                bOK = true;
                            }
                            catch (Exception ex)
                            {
                                ClientLoggerHelper.LogInfo(ex.ToString());
                            }

                            if (bOK)
                            {
                                if (allItems != null)
                                {
                                    foreach (var item in allItems)
                                    {
                                        ObjPharmacySellPriceProfitScale.Add(item);
                                    }

                                }
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    ClientLoggerHelper.LogInfo(ex.ToString());
                }
                finally
                {
                    Globals.IsBusy = false;
                }
            });

            t.Start();
        }

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members
        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentInwardDrugInvoice.SelectedSupplier = message.SelectedSupplier as Supplier;
                PharmacyPurchaseOrder_BySupplierID();
            }
        }
        #endregion

        #region IHandle<PharmacyCloseSearchInwardIncoiceEvent> Members
        public void Handle(PharmacyCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentInwardDrugInvoice = message.SelectedInwardInvoice as InwardDrugInvoice;
                InitDetailInward();
                DeepCopyInvoice();
                LoadInfoThenSelectedInvoice();
            }
        }

        private void LoadInfoThenSelectedInvoice()
        {
            Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
            PharmacyPurchaseOrder_BySupplierID();
        }
        #endregion

        #region IHandle<PharmacyCloseEditInwardEvent> Members
        public void Handle(PharmacyCloseEditInwardEvent message)
        {
            if (IsActive)
            {
                Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
            }
        }
        #endregion

        #region printing member
        public void btnPreview()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = CurrentInwardDrugInvoice.inviID;
            DialogView.eItem = ReportName.PHARMACY_NHAPTHUOCTUNCC;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void btnPrint()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new ReportServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetInWardDrugSupplierInPdfFormat(CurrentInwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
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
                                Globals.IsBusy = false;
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }
        #endregion


        public void chkIsNotVat_Click(object sender, RoutedEventArgs e)
        {
            var chkIsNotVat = sender as CheckBox;
            if ((chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail) != null)
            {
                if (chkIsNotVat.IsChecked == true)
                {
                    if (MessageBox.Show(eHCMSResources.Z2993_G1_ChonKhongThue, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                    {
                        chkIsNotVat.IsChecked = false;
                        (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).IsNotVat = false;
                        return;
                    }
                    (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).IsNotVat = true;
                    if ((chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).RefGenericDrugDetail != null)
                    {
                        (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).RefGenericDrugDetail.VAT = null;
                    }
                }
                else
                {
                    (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).IsNotVat = false;
                    if ((chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).RefGenericDrugDetail != null)
                    {
                        (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).RefGenericDrugDetail.VAT = 0;
                    }
                }
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
                if (Store.IsVATCreditStorage)
                {
                    CurrentInwardDrugInvoice.IsVATCredit = true;
                }
                else
                {
                    CurrentInwardDrugInvoice.IsVATCredit = false;
                }
            }
        }
        public bool ChangeVATCreditOnInwardInvoice
        {
            get { return Globals.ServerConfigSection.CommonItems.ChangeVATCreditOnInwardInvoice; }
        }
        public void SetPriceByPriceScale(PharmacyPurchaseOrderDetail item)
        {
            decimal PriceProfitScale = 0;
            decimal PriceProfitScaleForHI = 0;
            PriceProfitScale = (decimal)ObjPharmacySellPriceProfitScale.Where(x => x.BuyingCostFrom <= item.UnitPrice && x.BuyingCostTo >= item.UnitPrice).Select(x => x.NormalProfitPercent).FirstOrDefault();
            PriceProfitScaleForHI = (decimal)ObjPharmacySellPriceProfitScale.Where(x => x.BuyingCostFrom <= item.UnitPrice && x.BuyingCostTo >= item.UnitPrice).Select(x => x.HIAllowProfitPercent).FirstOrDefault();

            if (item.RefGenericDrugDetail.VAT == null || item.RefGenericDrugDetail.VAT == 0)
            {
                if (item.RefGenericDrugDetail.RefPharmacyDrugCatID == 5 || item.RefGenericDrugDetail.RefPharmacyDrugCatID == 6)
                {
                    item.RefGenericDrugDetail.NormalPrice = item.RefGenericDrugDetail.NormalPrice / 100 * 120;
                }
                else
                {
                    item.RefGenericDrugDetail.NormalPrice = item.UnitPrice * (PriceProfitScale / 100 + 1);
                }
                item.RefGenericDrugDetail.PriceForHIPatient = item.UnitPrice * (PriceProfitScaleForHI / 100 + 1);
            }
            else
            {
                if (item.RefGenericDrugDetail.RefPharmacyDrugCatID == 5 || item.RefGenericDrugDetail.RefPharmacyDrugCatID == 6)
                {
                    item.RefGenericDrugDetail.NormalPrice = item.UnitPrice * (1 + (decimal)item.RefGenericDrugDetail.VAT) / 100 * 120;
                }
                else
                {
                    item.RefGenericDrugDetail.NormalPrice = item.UnitPrice * (1 + (decimal)item.RefGenericDrugDetail.VAT) * (PriceProfitScale / 100 + 1);
                }
                item.RefGenericDrugDetail.PriceForHIPatient = item.UnitPrice * (1 + (decimal)item.RefGenericDrugDetail.VAT) * (PriceProfitScaleForHI / 100 + 1);
            }
        }
        //▼====== #006
        private void CheckEnableSupplier()
        {
            if ((PharmacyPurchaseOrderDetailList != null && PharmacyPurchaseOrderDetailList.Count > 0
                && PharmacyPurchaseOrderDetailList.Any(x => x.RefGenericDrugDetail != null))
                || (InwardDrugList != null && InwardDrugList.Count > 0))
            {
                IsHideAuSupplier = false;
            }
            else
            {
                IsHideAuSupplier = true;
            }
        }
        //▲====== #006
    }
}
