using System.ComponentModel.Composition;
using System.Windows.Data;
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
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
//using aEMR.Common.PagedCollectionView;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
/*
 * 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
 *                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
 * 20200807 #002 TNHX: Thêm loại để nhập hàng cho hóa chất
 * 20211102 #003 QTD: Lọc kho theo cấu hình trách nhiệm
 */
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IXapNhapPXHangKyGoi)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class XapNhapPXHangKyGoiViewModel : Conductor<object>, IXapNhapPXHangKyGoi
            , IHandle<DrugDeptCloseSearchSupplierEvent>, IHandle<DrugDeptCloseSearchInwardIncoiceEvent>
            , IHandle<DrugDeptCloseEditInwardEvent>
    {
        private long V_SupplierType = 7200;
        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NCC;

        #region Indicator Member
        private bool _isLoadingCurrency = false;
        public bool isLoadingCurrency
        {
            get { return _isLoadingCurrency; }
            set
            {
                if (_isLoadingCurrency != value)
                {
                    _isLoadingCurrency = value;
                    NotifyOfPropertyChange(() => isLoadingCurrency);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingGoodsType = false;
        public bool isLoadingGoodsType
        {
            get { return _isLoadingGoodsType; }
            set
            {
                if (_isLoadingGoodsType != value)
                {
                    _isLoadingGoodsType = value;
                    NotifyOfPropertyChange(() => isLoadingGoodsType);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

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

        private bool _isLoadingCost = false;
        public bool isLoadingCost
        {
            get { return _isLoadingCost; }
            set
            {
                if (_isLoadingCost != value)
                {
                    _isLoadingCost = value;
                    NotifyOfPropertyChange(() => isLoadingCost);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingInvoiceDetailD = false;
        public bool isLoadingInvoiceDetailD
        {
            get { return _isLoadingInvoiceDetailD; }
            set
            {
                if (_isLoadingInvoiceDetailD != value)
                {
                    _isLoadingInvoiceDetailD = value;
                    NotifyOfPropertyChange(() => isLoadingInvoiceDetailD);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingInvoiceID = false;
        public bool isLoadingInvoiceID
        {
            get { return _isLoadingInvoiceID; }
            set
            {
                if (_isLoadingInvoiceID != value)
                {
                    _isLoadingInvoiceID = value;
                    NotifyOfPropertyChange(() => isLoadingInvoiceID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingOrderID = false;
        public bool isLoadingOrderID
        {
            get { return _isLoadingOrderID; }
            set
            {
                if (_isLoadingOrderID != value)
                {
                    _isLoadingOrderID = value;
                    NotifyOfPropertyChange(() => isLoadingOrderID);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingOrderDetailID = false;
        public bool isLoadingOrderDetailID
        {
            get { return _isLoadingOrderDetailID; }
            set
            {
                if (_isLoadingOrderDetailID != value)
                {
                    _isLoadingOrderDetailID = value;
                    NotifyOfPropertyChange(() => isLoadingOrderDetailID);
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

        public bool IsLoading
        {
            get { return (isLoadingCurrency || isLoadingGoodsType || isLoadingGetStore || isLoadingFullOperator || isLoadingInvoiceDetailD || isLoadingOrderID || isLoadingOrderDetailID || isLoadingSearch || isLoadingCost); }
        }

        #endregion

        public enum DataGridCol
        {
            TenThuoc = 3,
            PackagingQty = 11,
            Qty = 12,
            DonGiaPackage = 14,
            DonGiaLe = 15,
            TotalNotVAT = 16,
            CKPercent = 17,
            CK = 18
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public XapNhapPXHangKyGoiViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
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

            Coroutine.BeginExecute(DoGetStore_MedDept());

            IsHideAuSupplier = true;
            IsEnabled = true;
            IsVisibility = Visibility.Collapsed;

            SupplierCriteria = new SupplierSearchCriteria();
            SupplierCriteria.V_SupplierType = V_SupplierType;
            SupplierCriteria.V_MedProductType = V_MedProductType;

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
        void InwardDrugMedDeptList_OnRefresh(object sender, RefreshEventArgs e)
        {
            InwardDrugDetails_ByID(inviID, InwardDrugMedDeptList.PageIndex, InwardDrugMedDeptList.PageSize);
        }

        void CurrentRefGenMedProductDetails_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRefDrugGenericDetails_AutoPaging(BrandName, CurrentRefGenMedProductDetails.PageIndex, CurrentRefGenMedProductDetails.PageSize);
        }

     
        #region Properties Member
        private Double _TotalPercent = 0;
        public Double TotalPercent
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
                    if (value != null && value.CanEditAndDelete)
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

        private bool _OutwardAllChecked = false;
        public bool OutwardAllChecked
        {
            get
            {
                return _OutwardAllChecked;
            }
            set
            {
                if (_OutwardAllChecked != value)
                {
                    _OutwardAllChecked = value;
                    NotifyOfPropertyChange(() => OutwardAllChecked);
                    if (_OutwardAllChecked)
                    {
                        Process_AllOutwardChecked();
                    }
                    else
                    {
                        Process_AllOutwardUnChecked();
                    }
                }
            }
        }
        public CollectionView CV_ObjPNTemp_BySupplierIDList { get; set; }

        private CollectionViewSource _CVS_ObjPNTemp_BySupplierIDList;
        public CollectionViewSource CVS_ObjPNTemp_BySupplierIDList
        {
            get
            {
                return _CVS_ObjPNTemp_BySupplierIDList;
            }
            set
            {
                if (_CVS_ObjPNTemp_BySupplierIDList != value)
                {
                    _CVS_ObjPNTemp_BySupplierIDList = value;
                    NotifyOfPropertyChange(() => CVS_ObjPNTemp_BySupplierIDList);
                }
            }
        }

        private ObservableCollection<InwardDrugMedDept> _ObjPNTemp_BySupplierIDList_Root;
        public ObservableCollection<InwardDrugMedDept> ObjPNTemp_BySupplierIDList_Root
        {
            get
            {
                return _ObjPNTemp_BySupplierIDList_Root;
            }
            set
            {
                if (_ObjPNTemp_BySupplierIDList_Root != value)
                {
                    _ObjPNTemp_BySupplierIDList_Root = value;
                    NotifyOfPropertyChange(() => ObjPNTemp_BySupplierIDList_Root);
                }
            }
        }

        private InwardDrugMedDept _PNTemp_BySupplierIDSelected;
        public InwardDrugMedDept PNTemp_BySupplierIDSelected
        {
            get
            {
                return _PNTemp_BySupplierIDSelected;
            }
            set
            {
                if (_PNTemp_BySupplierIDSelected != value)
                {
                    _PNTemp_BySupplierIDSelected = value;
                    NotifyOfPropertyChange(() => PNTemp_BySupplierIDSelected);
                }
            }
        }


        private ObservableCollection<InwardDrugMedDept> _ObjPNTemp_BySupplierIDList_Virtual;
        public ObservableCollection<InwardDrugMedDept> ObjPNTemp_BySupplierIDList_Virtual
        {
            get
            {
                return _ObjPNTemp_BySupplierIDList_Virtual;
            }
            set
            {
                if (_ObjPNTemp_BySupplierIDList_Virtual != value)
                {
                    _ObjPNTemp_BySupplierIDList_Virtual = value;
                    NotifyOfPropertyChange(() => ObjPNTemp_BySupplierIDList_Virtual);
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
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            
        }
        #region checking account

        private bool _mSapNhapHangKyGui_Tim = true;
        private bool _mSapNhapHangKyGui_PhieuMoi = true;
        private bool _mSapNhapHangKyGui_CapNhat = true;
        private bool _mSapNhapHangKyGui_Xoa = true;
        private bool _mSapNhapHangKyGui_XemIn = true;
        private bool _mSapNhapHangKyGui_In = true;

        public bool mSapNhapHangKyGui_Tim
        {
            get
            {
                return _mSapNhapHangKyGui_Tim;
            }
            set
            {
                if (_mSapNhapHangKyGui_Tim == value)
                    return;
                _mSapNhapHangKyGui_Tim = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Tim);
            }
        }

        public bool mSapNhapHangKyGui_PhieuMoi
        {
            get
            {
                return _mSapNhapHangKyGui_PhieuMoi;
            }
            set
            {
                if (_mSapNhapHangKyGui_PhieuMoi == value)
                    return;
                _mSapNhapHangKyGui_PhieuMoi = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_PhieuMoi);
            }
        }

        public bool mSapNhapHangKyGui_CapNhat
        {
            get
            {
                return _mSapNhapHangKyGui_CapNhat;
            }
            set
            {
                if (_mSapNhapHangKyGui_CapNhat == value)
                    return;
                _mSapNhapHangKyGui_CapNhat = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_CapNhat);
            }
        }

        public bool mSapNhapHangKyGui_Xoa
        {
            get
            {
                return _mSapNhapHangKyGui_Xoa;
            }
            set
            {
                if (_mSapNhapHangKyGui_Xoa == value)
                    return;
                _mSapNhapHangKyGui_Xoa = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_Xoa);
            }
        }

        public bool mSapNhapHangKyGui_XemIn
        {
            get
            {
                return _mSapNhapHangKyGui_XemIn;
            }
            set
            {
                if (_mSapNhapHangKyGui_XemIn == value)
                    return;
                _mSapNhapHangKyGui_XemIn = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_XemIn);
            }
        }

        public bool mSapNhapHangKyGui_In
        {
            get
            {
                return _mSapNhapHangKyGui_In;
            }
            set
            {
                if (_mSapNhapHangKyGui_In == value)
                    return;
                _mSapNhapHangKyGui_In = value;
                NotifyOfPropertyChange(() => mSapNhapHangKyGui_In);
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
            lnkDelete.Visibility = Globals.convertVisibility(mSapNhapHangKyGui_Xoa);
        }
        public void lnkDeleteMain_Loaded(object sender)
        {
            lnkDeleteMain = sender as Button;
            lnkDeleteMain.Visibility = Globals.convertVisibility(mSapNhapHangKyGui_Xoa);
        }
        public void lnkEdit_Loaded(object sender)
        {
            lnkEdit = sender as Button;
            lnkEdit.Visibility = Globals.convertVisibility(mSapNhapHangKyGui_CapNhat);
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
            isLoadingCurrency = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
                            isLoadingCurrency = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void LoadGoodsTypeCbx()
        {
            isLoadingGoodsType = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
                            isLoadingGoodsType = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false,null, false, false);
            yield return paymentTypeTask;
            //StoreCbx = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            //▼===== #003
            var StoreTemp = paymentTypeTask.LookupList.Where(x => (V_MedProductType != 0 && x.ListV_MedProductType != null && x.ListV_MedProductType.Contains(V_MedProductType.ToString()))).ToObservableCollection();
            StoreCbx = Globals.CheckDrugMedStoreWareHouse(StoreTemp);
            if (StoreCbx != null && StoreCbx.Count > 0)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0110_G1_Msg_InfoChuaCauHinhTNKho.Replace("\\n ", "\n"));
            }
            //▲===== #003
            isLoadingGetStore = false;
            yield break;
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show("Bạn có muốn tạo phiếu nhập khác không?", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
                InitializeInvoiceDrug();
            //}
        }

        private void InitializeInvoiceDrug()
        {
            CurrentInwardDrugMedDeptInvoice = new InwardDrugMedDeptInvoice();
            CurrentInwardDrugMedDeptInvoice.InvDateInvoice = Globals.GetCurServerDateTime();
            CurrentInwardDrugMedDeptInvoice.DSPTModifiedDate = Globals.GetCurServerDateTime();
            CurrentInwardDrugMedDeptInvoice.SelectedStaff = GetStaffLogin();
            CurrentInwardDrugMedDeptInvoice.StaffID = GetStaffLogin().StaffID;
            CurrentInwardDrugMedDeptInvoice.IsForeign = false;
            CurrentInwardDrugMedDeptInvoice.IsCheckBuyingPrice = Globals.ServerConfigSection.MedDeptElements.CheckValueBuyPriceOnImportTempInward;

            //KMx: Khi bấm thêm mới thì phải xóa list Danh sách hàng cần sáp nhập (03/10/2014 09:25).
            CVS_ObjPNTemp_BySupplierIDList = new CollectionViewSource();
            CV_ObjPNTemp_BySupplierIDList = (CollectionView)CVS_ObjPNTemp_BySupplierIDList.View;

            //if (Globals.ConfigList != null)
            //{
            //    CurrentInwardDrugMedDeptInvoice.VAT = Decimal.Parse(Globals.ConfigList[(int)AppConfigKeys.ConfigItemKey.PharmacyDefaultVATInward].ToString(), CultureInfo.InvariantCulture);
            //}

            // Txd 25/05/2014 Replaced ConfigList
            CurrentInwardDrugMedDeptInvoice.VAT = (decimal)Globals.ServerConfigSection.PharmacyElements.PharmacyDefaultVATInward;

            if (Currencies != null)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedCurrency = Currencies.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
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
            CurrentInwardInvoiceCopy = CurrentInwardDrugMedDeptInvoice.DeepCopy();
        }

        private void DeleteInwardInvoiceDrug()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteInwardDrugMedDeptInvoice(CurrentInwardDrugMedDeptInvoice.inviID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
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
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

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

        private void UpdateInwardInvoiceDrug()
        {
            isLoadingFullOperator = true;
            CurrentInwardDrugMedDeptInvoice.TypID = TypID;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //▼====== #002
                    contract.BeginUpdateInwardDrugMedDeptInvoice(CurrentInwardDrugMedDeptInvoice, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                    //▲====== #002
                        try
                        {
                            int results = contract.EndUpdateInwardDrugMedDeptInvoice(asyncResult);
                            if (results == 0)
                            {
                                DeepCopyInvoice();
                                CheckIsPercentAll();
                                Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID);
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
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

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
        }

        private void Load_InwardDrugDetails(long ID)
        {
            inviID = ID;
            InwardDrugMedDeptList.PageIndex = 0;
            InwardDrugDetails_ByID(CurrentInwardDrugMedDeptInvoice.inviID, 0, Globals.PageSize);
        }

        private void InwardDrugDetails_ByID(long ID, int PageIndex, int PageSize)
        {
            isLoadingInvoiceDetailD = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //▼====== #002
                    contract.BeginGetInwardDrugMedDept_ByIDInvoice(ID, PageSize, PageIndex, true, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                    //▲====== #002
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

                           

                            //tinh tong tien 
                            CountTotalPrice(TongTienSPChuaVAT, CKTrenSP, TongTienTrenSPDaTruCK, TongCKTrenHoaDon, TongTienHoaDonCoThueNK, TongTienHoaDonCoVAT, TotalVATDifferenceAmount);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingInvoiceDetailD = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

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
            isLoadingOrderID = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
                            isLoadingOrderID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }



        private void InwardDrugMedDeptIsInputTemp_BySupplierID(long SupplierID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInwardDrugMedDeptIsInputTemp_BySupplierID(SupplierID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInwardDrugMedDeptIsInputTemp_BySupplierID(asyncResult);
                            ObjPNTemp_BySupplierIDList_Virtual = new ObservableCollection<InwardDrugMedDept>(results);


                            //Tính Số Lượng Đóng Gói
                            Calc_SoLuongDongGoi();
                            //Tính Số Lượng Đóng Gói


                            CVS_ObjPNTemp_BySupplierIDList = new CollectionViewSource { Source = ObjPNTemp_BySupplierIDList_Virtual };
                            CV_ObjPNTemp_BySupplierIDList = (CollectionView)CVS_ObjPNTemp_BySupplierIDList.View;
                            CV_ObjPNTemp_BySupplierIDList.Filter = null;
                            
                            //ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("inviID"));

                            CV_ObjPNTemp_BySupplierIDList.GroupDescriptions.Clear();
                            CV_ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("TempOutwardInvNum"));

                            NotifyOfPropertyChange(() => CV_ObjPNTemp_BySupplierIDList);

                            if (ObjPNTemp_BySupplierIDList_Virtual != null && ObjPNTemp_BySupplierIDList_Virtual.Count > 0 && CurrentInwardDrugMedDeptInvoice.CheckedPoint==false)
                            {
                                GridSuppliers.Columns[0].Visibility = Visibility.Visible;
                            }
                            else
                            {
                                GridSuppliers.Columns[0].Visibility = Visibility.Collapsed;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingOrderID = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }


        private void Calc_SoLuongDongGoi()
        {
            if(ObjPNTemp_BySupplierIDList_Virtual!=null && ObjPNTemp_BySupplierIDList_Virtual.Count>0)
            {
                foreach (var item in ObjPNTemp_BySupplierIDList_Virtual)
                {
                    item.PackageQuantity =(double)item.InQuantity / item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);

                    if (item.PackageQuantity > 0)
                    {
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                        item.RefGenMedProductDetails.UnitPrice = item.PackagePrice / item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    }
                }
            }
        }


        private void GetInwardInvoiceDrugByID(long ID)
        {
            isLoadingInvoiceID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //▼====== #002
                    contract.BeginGetInwardDrugMedDeptInvoice_ByID(ID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                    //▲====== #002
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
                            isLoadingInvoiceID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void AddInwardInvoiceDrug()
        {
            isLoadingFullOperator = true;
            CurrentInwardDrugMedDeptInvoice.TypID = TypID;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddInwardDrugMedDeptInvoice(CurrentInwardDrugMedDeptInvoice, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID;
                            int results = contract.EndAddInwardDrugMedDeptInvoice(out ID, asyncResult);
                            if (results == 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z1413_G1_ThemTTinHDonThCong, eHCMSResources.G0442_G1_TBao);
                                GetInwardInvoiceDrugByID(ID);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0584_G1_SoHDDaTonTai, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
            //nhap thong tin phieu nhap vao database
            if (CheckValidate())
            {
                if (CurrentInwardDrugMedDeptInvoice.SelectedCurrency == null || CurrentInwardDrugMedDeptInvoice.SelectedCurrency.CurrencyID == 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z1597_G1_ChonTinhBang, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugMedDeptInvoice.VAT < 1 && CurrentInwardDrugMedDeptInvoice.VAT > 0)
                {
                    Globals.ShowMessage(eHCMSResources.K0262_G1_VATKhongHopLe, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrugMedDeptInvoice.DiscountingByPercent < 1 && CurrentInwardDrugMedDeptInvoice.DiscountingByPercent > 0)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0072_G1_CKKhHopLe), eHCMSResources.G0442_G1_TBao);
                    return;
                }
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
            DrugDeptPoID = CurrentOrder.DrugDeptPoID;
            DrugDeptPurchaseOrderDetail_ByParentID(DrugDeptPoID.GetValueOrDefault());
        }

        DataGrid GridInwardDrug = null;
        public void GridInwardDrug_Loaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug = sender as DataGrid;
        }

        public void GridOutwardDrug_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            InwardDrugMedDept outItem = e.Row.DataContext as InwardDrugMedDept;
            outItem.DisplayGridRowNumber = e.Row.GetIndex() + 1;

            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
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


        private void AssignDrugDeptPoDetailID()
        {
            if (DrugDeptPurchaseOrderDetailList == null || DrugDeptPurchaseOrderDetailList.Count <= 0
                || ObjPNTemp_BySupplierIDList_Virtual == null || ObjPNTemp_BySupplierIDList_Virtual.Count <= 0)
            {
                return;
            }

            //Tính lại số lượng chờ của phiếu đặt hàng. Tránh trường hợp 1 phiếu đặt hàng click 2 lần (31/01/2015 16:02).
            foreach (DrugDeptPurchaseOrderDetail value in DrugDeptPurchaseOrderDetailList)
            {
                int InQty = Convert.ToInt32(ObjPNTemp_BySupplierIDList_Virtual.Where(x => x.GenMedProductID == value.GenMedProductID && x.DrugDeptPoDetailID == value.DrugDeptPoDetailID).Sum(x => x.InQuantity));
                value.WaitingDeliveryQty -= InQty;
            }

            foreach (InwardDrugMedDept item in ObjPNTemp_BySupplierIDList_Virtual)
            {
                if (item.DrugDeptPoDetailID > 0)
                {
                    continue;
                }

                var orderDetail = DrugDeptPurchaseOrderDetailList.Where(x => x.GenMedProductID == item.GenMedProductID && x.WaitingDeliveryQty > 0).FirstOrDefault();

                if (orderDetail == null)
                {
                    continue;
                }
                orderDetail.WaitingDeliveryQty -= Convert.ToInt16(item.InQuantity);

                item.DrugDeptPoID = orderDetail.DrugDeptPoID;
                item.PONumber = orderDetail.PONumber;
                item.DrugDeptPoDetailID = orderDetail.DrugDeptPoDetailID;

                if (item.RefGenMedProductDetails != null)
                {
                    item.RefGenMedProductDetails.UnitPrice = orderDetail.UnitPrice;
                    item.TotalPriceNotVAT = (decimal)item.InQuantity * item.RefGenMedProductDetails.UnitPrice;
                    item.PackagePrice = item.RefGenMedProductDetails.UnitPrice * item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                }

            }
        }

        private void DrugDeptPurchaseOrderDetail_ByParentID(long PharmacyPoID)
        {
            this.ShowBusyIndicator();
            //isLoadingOrderDetailID = true;
            //  Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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

                            //KMx: Gán mã đặt hàng cho những dòng nhập tạm.
                            AssignDrugDeptPoDetailID();

                            //so sanh ds load len voi ds ngay ben duoi
                            //GetDrugDeptPurchaseOrderDetailList(PharmacyPoID);
                            //CheckIsPercentAll();
                            //TinhTienDonDatHang();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            this.HideBusyIndicator();
                            //isLoadingOrderDetailID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

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

        //public void cbxChonDH_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ComboBox cbxChonDH = sender as ComboBox;
        //    if (cbxChonDH != null)
        //    {
        //        if (cbxChonDH.SelectedItem != null)
        //        {
        //            DrugDeptPurchaseOrderDetail_ByParentID(cbxChonDH.SelectedItem);
        //        }
        //        else
        //        {
        //            ClearDrugDeptPurchaseOrderDetailList();
        //            DrugDeptPoID = 0;
        //        }
        //    }
        //    else
        //    {
        //        DrugDeptPoID = 0;
        //    }
        //}

        private void IsChangedItem()
        {
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
        }


        private bool CheckValidationData()
        {
            bool results = true;
          
            string TB = "";
            for (int i = 0; i < ObjPNTemp_BySupplierIDList_Virtual.Count; i++)
            {
                if (ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails != null && ObjPNTemp_BySupplierIDList_Virtual[i].GenMedProductID > 0)
                {
                    if (ObjPNTemp_BySupplierIDList_Virtual[i].GenMedProductID == 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0785_G1_Msg_InfoPhaiChonThuoc));
                        return false;
                    }
                    else if (ObjPNTemp_BySupplierIDList_Virtual[i].InQuantity <= 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0789_G1_Msg_InfoSLgNhapLonHon0));
                        return false;
                    }
                    else if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                    {
                        if (string.IsNullOrWhiteSpace(ObjPNTemp_BySupplierIDList_Virtual[i].InBatchNumber))
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0784_G1_Msg_InfoChuaNhapSoLo));
                            return false;
                        }
                        else if (ObjPNTemp_BySupplierIDList_Virtual[i].InExpiryDate == null)
                        {
                            MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0782_G1_Msg_InfoChuaNhapHSD));
                            return false;
                        }
                    }
                   
                    else if (ObjPNTemp_BySupplierIDList_Virtual[i].InExpiryDate <= DateTime.Now)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0787_G1_Msg_InfoHSDLonHonNgHTai));
                        return false;
                    }
                   
                    else if (ObjPNTemp_BySupplierIDList_Virtual[i].InProductionDate >= DateTime.Now)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.Z0590_G1_NgSXPhaiNhoHonNgHTai));
                        return false;
                    }
                    if (ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails.UnitPrice < 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.Z0591_G1_DGiaKgDuocNhoHon0));
                        return false;
                    }
                    if (ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails.UnitPrice == 0 && ObjPNTemp_BySupplierIDList_Virtual[i].ObjV_GoodsType != null && ObjPNTemp_BySupplierIDList_Virtual[i].ObjV_GoodsType.LookupID == (long)AllLookupValues.V_GoodsType.HANGMUA)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0556_G1_Msg_InfoGiaMuaLonHon0));
                        return false;
                    }
                    if ((ObjPNTemp_BySupplierIDList_Virtual[i].DiscountingByPercent < 1 && ObjPNTemp_BySupplierIDList_Virtual[i].DiscountingByPercent > 0) || (ObjPNTemp_BySupplierIDList_Virtual[i].DiscountingByPercent > 2))
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.A0072_G1_CKKhHopLe));
                        return false;
                    }
                    if (ObjPNTemp_BySupplierIDList_Virtual[i].ObjV_GoodsType == null || ObjPNTemp_BySupplierIDList_Virtual[i].ObjV_GoodsType.LookupID <= 0)
                    {
                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), eHCMSResources.Z0592_G1_ChonLoaiHang));
                        return false;
                    }
                    //tam thoi dong cai nay de nhap hang cai da
                    //if (DrugDeptPurchaseOrderDetailList[i].CurrentRefGenMedProductDetail.NormalPrice <= 0)
                    //{
                    //    MessageBox.Show(string.Format("{0} ", eHCMSResources.Z0589_G1_LoiDongThu) + (i + 1).ToString() + string.Format(" {0}!", eHCMSResources.A0786_G1_Msg_InfoCNhatBGiaBan));
                    //    return false;
                    //}
                    if (ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails.UnitPrice != ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails.InBuyingPrice && ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails.InBuyingPrice > 0)
                    {
                        TB += string.Format(eHCMSResources.Z1598_G1_GiaMua0DotNayKhacDotTruoc, ObjPNTemp_BySupplierIDList_Virtual[i].RefGenMedProductDetails.BrandName);
                    }
                }
                if (!string.IsNullOrEmpty(TB))
                {
                    if (MessageBox.Show(TB + Environment.NewLine + eHCMSResources.I0920_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return results;
        }

        private void CheckValidationData(long SupplierID)
        {
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    //Phải đọc lại 1 lần, vì nhiều khi có 1 người nào đó trên máy khác đã Xáp Nhập bớt 1 Ít rồi. Nên đọc lại để coi Số thực hiện tại còn lại bao nhiêu

                    contract.BeginInwardDrugMedDeptIsInputTemp_BySupplierID(SupplierID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndInwardDrugMedDeptIsInputTemp_BySupplierID(asyncResult);
                            ObjPNTemp_BySupplierIDList_Root = new ObservableCollection<InwardDrugMedDept>(results);

                            bool ResCheck = true;
                            if (ObjPNTemp_BySupplierIDList_Root.Count > 0)
                            {
                                for (int i = 0; i < ObjPNTemp_BySupplierIDList_Virtual.Count; i++)
                                {
                                    decimal slRoot = 0;
                                    if (CheckSLEditValid(ObjPNTemp_BySupplierIDList_Virtual[i].ObjSupplierID.SupplierID, ObjPNTemp_BySupplierIDList_Virtual[i].GenMedProductID.Value, ObjPNTemp_BySupplierIDList_Virtual[i].InQuantity, out slRoot) == false)
                                    {
                                        MessageBox.Show(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01
                                            , ObjPNTemp_BySupplierIDList_Virtual[i].DisplayGridRowNumber.ToString(), string.Format("{0}: {1}", eHCMSResources.A0792_G1_Msg_InfoSLgNhapMax, slRoot.ToString())));
                                        ResCheck = false;
                                    }
                                }

                                if (ResCheck)
                                {
                                    InwardDrugInvoices_XapNhapInputTemp_Save(CurrentInwardDrugMedDeptInvoice.inviID, ObjPNTemp_BySupplierIDList_Virtual);
                                }
                            }
                            else
                            {
                                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0588_G1_KgCoDLieuNhap));
                            }

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private bool CheckSLEditValid(long SupplierID, long GenMedProductID, decimal sledit, out decimal slRoot)
        {
            slRoot = 0;

            foreach (var items in ObjPNTemp_BySupplierIDList_Root)
            {
                if(items.GenMedProductID==GenMedProductID && items.ObjSupplierID.SupplierID==SupplierID)
                {
                    slRoot = items.InQuantity;

                    if(items.InQuantity>=sledit)
                    {
                        return true;
                    }
                }
            }
            return true;    
        }

        private void InwardDrugInvoices_XapNhapInputTemp_Save(long inviIDJoin, IEnumerable<InwardDrugMedDept> ObjInwardDrugMedDeptList)
        {
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.IsCheckBuyingPrice && ObjInwardDrugMedDeptList.Any(x => Math.Abs(x.TotalPriceNotVAT - x.NormalPrice / CurrentInwardDrugMedDeptInvoice.VAT) > 0.1m))
            {
                MessageBox.Show(eHCMSResources.Z2167_G1_GiaNhapKhongKhopGiaBan, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            isLoadingFullOperator = true;

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInwardDrugInvoices_XapNhapInputTemp_Save(inviIDJoin, ObjInwardDrugMedDeptList, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            string Result = "";
                            bool b=contract.EndInwardDrugInvoices_XapNhapInputTemp_Save(out Result, asyncResult);
                            if(b)
                            {
                                MessageBox.Show(eHCMSResources.K0466_G1_XapNhapOk);

                                ClearDrugDeptPurchaseOrderDetailList();
                                PharmacyPurchaseOrder_BySupplierID();

                                InwardDrugMedDeptIsInputTemp_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);

                                Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID);
                                InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
                                IsHideAuSupplier = false;
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.Z1405_G1_LoiSapNhapKgThanhCong);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.T0432_G1_Error, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnNhapHang(object sender, RoutedEventArgs e)
        {
            IsChangedItem();

            if (ObjPNTemp_BySupplierIDList_Virtual == null || ObjPNTemp_BySupplierIDList_Virtual.Count == 0)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.Z0588_G1_KgCoDLieuNhap));
                return;
            }

            if (CheckValidationData())
            {
                CheckValidationData(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
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
                GridSuppliers.Columns[(int)DataGridCol.TenThuoc].IsReadOnly = false;
                GridSuppliers.Columns[(int)DataGridCol.TenThuoc].CellStyle = null;
            }
        }

        private void ReadOnlyColumnTrue()
        {
            if (GridSuppliers != null)
            {
                GridSuppliers.Columns[(int)DataGridCol.TenThuoc].IsReadOnly = true;
                GridSuppliers.Columns[(int)DataGridCol.TenThuoc].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyLeft"];
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

        private void SearchRefDrugGenericDetails_AutoPaging(string Name, int PageIndex, int PageSize)
        {
            long? SupplierID = 0;
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.SelectedSupplier != null)
            {
                SupplierID = CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID;
            }
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyGenMedDeptDetailsServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_SearchAutoPaging(null, Name, SupplierID, V_MedProductType, null, PageSize, PageIndex, null, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndRefGenMedProductDetails_SearchAutoPaging(out totalCount, asyncResult);
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
            SearchRefDrugGenericDetails_AutoPaging(e.Parameter, 0, CurrentRefGenMedProductDetails.PageSize);
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
                    CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail = (sender as AutoCompleteBox).SelectedItem as RefGenMedProductDetails;
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
            int Total = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptSupplierXapNhapPXTemp_SearchPaging(SupplierCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var Res = contract.EndDrugDeptSupplierXapNhapPXTemp_SearchPaging(out Total, asyncResult);

                            if (Res != null)
                            {
                                Suppliers.Clear();
                                Suppliers.TotalItemCount = Total;
                                foreach (DrugDeptSupplier p in Res)
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
            SupplierCriteria.V_MedProductType = V_MedProductType;
            Suppliers.PageIndex = 0;
            SearchSupplierAuto(Suppliers.PageIndex, Suppliers.PageSize);

        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            Action<ISuppliersXapNhapPXHangKyGoi> onInitDlg = delegate (ISuppliersXapNhapPXHangKyGoi proAlloc)
            {
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISuppliersXapNhapPXHangKyGoi>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (CurrentInwardDrugMedDeptInvoice != null && CurrentInwardDrugMedDeptInvoice.SelectedSupplier !=null)
            {
                PharmacyPurchaseOrder_BySupplierID();
                InwardDrugMedDeptIsInputTemp_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
            }
        }


        #endregion

        private void DeleteInwardDrug(long InID)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteInwardDrugMedDeptTemp(InID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool results = contract.EndDeleteInwardDrugMedDeptTemp(asyncResult);
                            if (results)
                            {
                                Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID);
                                InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
                                PharmacyPurchaseOrder_BySupplierID();
                                InwardDrugMedDeptIsInputTemp_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
                                MessageBox.Show(eHCMSResources.K0476_G1_XoaHgOk, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                            else
                            {
                                MessageBox.Show(eHCMSResources.K0477_G1_XoaHgFail, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
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
            if (CurrentInwardDrugMedDeptInvoice.CanEditAndDelete)
            {
                IsChangedItem();
                CopyCurrentInwardDrugMedDept();
                Action<IDrugDeptEditInwardDrug> onInitDlg = delegate (IDrugDeptEditInwardDrug proAlloc)
                {
                    proAlloc.CbxGoodsTypes = CbxGoodsTypes.DeepCopy();
                    proAlloc.CurrentInwardDrugMedDeptCopy = CurrentInwardDrugMedDept.DeepCopy();
                    proAlloc.SetValueForProperty();
                };
                GlobalsNAV.ShowDialog<IDrugDeptEditInwardDrug>(onInitDlg);
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1419_G1_ThuocDaKChTDK, eHCMSResources.G0442_G1_TBao);
            }
        }

        private void ReadOnlyColumnValueFalse()
        {
            if (GridSuppliers != null)
            {
                GridSuppliers.Columns[(int)DataGridCol.CKPercent].IsReadOnly = false;
                GridSuppliers.Columns[(int)DataGridCol.CKPercent].CellStyle = null;
                GridSuppliers.Columns[(int)DataGridCol.CK].IsReadOnly = true;
                GridSuppliers.Columns[(int)DataGridCol.CK].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyRight"];
            }
        }

        private void ReadOnlyColumnValueTrue()
        {
            if (GridSuppliers != null)
            {
                GridSuppliers.Columns[(int)DataGridCol.CKPercent].IsReadOnly = true;
                GridSuppliers.Columns[(int)DataGridCol.CKPercent].CellStyle = (Style)Application.Current.Resources["CellStyleReadOnlyRight"];
                GridSuppliers.Columns[(int)DataGridCol.CK].IsReadOnly = false;
                GridSuppliers.Columns[(int)DataGridCol.CK].CellStyle = null;
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
            AxTextBox tbl = GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) as AxTextBox;
            if (tbl != null)
            {
                value = tbl.Text;
            }
        }
        public void grdRequestDetails_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = (e.Row.DataContext as InwardDrugMedDept);
            EditedColumnName = null;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colCode")))
            {
                DrugDeptPurchaseOrderDetail item = e.Row.DataContext as DrugDeptPurchaseOrderDetail;
                if (item != null && !string.IsNullOrEmpty(item.Code) && value != item.Code)
                {
                    string Code = Globals.FormatCode(V_MedProductType, item.Code);
                    SearchRefDrugGenericDetails_AutoPaging(Code, 0, 1);
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
        private InwardDrugMedDept EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditedDetailItem == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                decimal ite = 0;
                Decimal.TryParse(value, out ite);

                InwardDrugMedDept item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetails != null && item.RefGenMedProductDetails.UnitPrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = item.InQuantity * item.RefGenMedProductDetails.UnitPrice;
                    }
                    item.PackagePrice = item.RefGenMedProductDetails.UnitPrice * item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    if (item.RefGenMedProductDetails.UnitPrice != item.RefGenMedProductDetails.InBuyingPrice && item.RefGenMedProductDetails.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (EditedColumnName.Equals("colPackagePrice"))
            {
                decimal ite = 0;
                Decimal.TryParse(value, out ite);

                InwardDrugMedDept item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetails != null && item.PackagePrice != ite)
                {
                    if (item.TotalPriceNotVAT == 0)
                    {
                        item.TotalPriceNotVAT = (decimal)item.PackageQuantity * item.PackagePrice;
                    }
                    item.RefGenMedProductDetails.UnitPrice = item.PackagePrice / item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    if (item.RefGenMedProductDetails.UnitPrice != item.RefGenMedProductDetails.InBuyingPrice && item.RefGenMedProductDetails.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
            else if (EditedColumnName.Equals("colPackagingQty"))
            {
                double ite = 0;
                Double.TryParse(value, out ite);

                InwardDrugMedDept item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetails != null && item.PackageQuantity != ite)
                {
                    item.InQuantity = (decimal)item.PackageQuantity * item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = item.InQuantity * item.RefGenMedProductDetails.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.RefGenMedProductDetails.UnitPrice = item.TotalPriceNotVAT / (decimal)item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                }
            }
            else if (EditedColumnName.Equals("colQty"))
            {
                decimal ite = 0;
                Decimal.TryParse(value, out ite);

                InwardDrugMedDept item = EditedDetailItem;
                item.PackageQuantity = (double)item.InQuantity / item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                if (item != null && item.RefGenMedProductDetails != null && item.InQuantity != ite)
                {
                    if (item.PackagePrice > 0)
                    {
                        item.TotalPriceNotVAT = item.InQuantity * item.RefGenMedProductDetails.UnitPrice;
                    }
                    else if (item.TotalPriceNotVAT > 0)
                    {
                        item.RefGenMedProductDetails.UnitPrice = item.TotalPriceNotVAT / item.InQuantity;
                        item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                    }
                }
            }
            else if (EditedColumnName.Equals("colTotalNotVAT"))
            {
                decimal ite = 0;
                Decimal.TryParse(value, out ite);

                InwardDrugMedDept item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetails != null && item.TotalPriceNotVAT != ite)
                {
                    if (item.PackageQuantity > 0)
                    {
                        if (item.PackagePrice == 0)
                        {
                            item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PackageQuantity;
                        }
                        if (item.RefGenMedProductDetails.UnitPrice == 0)
                        {
                            item.RefGenMedProductDetails.UnitPrice = item.PackagePrice / item.RefGenMedProductDetails.UnitPackaging.GetValueOrDefault(1);
                        }
                    }
                    if (item.RefGenMedProductDetails.UnitPrice != item.RefGenMedProductDetails.InBuyingPrice && item.RefGenMedProductDetails.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                }
            }
        }

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
            Action<IDrugDeptInwardDrugSupplierSearch> onInitDlg = delegate (IDrugDeptInwardDrugSupplierSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.TypID = TypID;
                proAlloc.V_MedProductType = V_MedProductType;
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.strHienThi = eHCMSResources.Z0618_G1_TimHDonNhapThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
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
                if (results != null && results.Count > 0)
                {
                    foreach (InwardDrugMedDeptInvoice p in results)
                    {
                        proAlloc.InwardInvoiceList.Add(p);
                    }
                }
            };
            GlobalsNAV.ShowDialog<IDrugDeptInwardDrugSupplierSearch>(onInitDlg);
        }

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            isLoadingSearch = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });

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
                                    LoadInfoThenSelectedInvoice();

                                    InwardDrugMedDeptIsInputTemp_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
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
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

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
                SearchInwardInvoiceDrug(0, Globals.PageSize);

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
                SearchInwardInvoiceDrug(0, Globals.PageSize);
            }
        }

        public void btnSearch()
        {
            SearchInwardInvoiceDrug(0, Globals.PageSize);
        }

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentInwardDrugMedDeptInvoice.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                PharmacyPurchaseOrder_BySupplierID();
                InwardDrugMedDeptIsInputTemp_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseEditInwardEvent> Members

        public void Handle(DrugDeptCloseEditInwardEvent message)
        {
            if (this.IsActive)
            {
                Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID);
                InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<IDrugDeptReportDocumentPreview> onInitDlg = delegate (IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentInwardDrugMedDeptInvoice.inviID;
                //20200320 TBL: Xem in của phiếu xáp nhập giống như nhập từ NCC
                //proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER;
                if (CurrentInwardDrugMedDeptInvoice.IsForeign)
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER;
                }
                else
                {
                    proAlloc.eItem = ReportName.DRUGDEPT_INWARD_MEDDEPTSUPPLIER_TRONGNUOC;
                }
                if (V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0569_G1_PhNhapKhoThuoc.ToUpper();
                }
                else if (V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0570_G1_PhNhapKhoYCu.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z0571_G1_PhNhapKhoHChat.ToUpper();
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
                CurrentInwardDrugMedDeptInvoice = message.SelectedInwardInvoice as InwardDrugMedDeptInvoice;
                InitDetailInward();
                DeepCopyInvoice();
                LoadInfoThenSelectedInvoice();
                InwardDrugMedDeptIsInputTemp_BySupplierID(CurrentInwardDrugMedDeptInvoice.SelectedSupplier.SupplierID);
            }
        }

        private void LoadInfoThenSelectedInvoice()
        {
            IsHideAuSupplier = false;
            InwardDrugMedDeptInvoice_GetListCost(CurrentInwardDrugMedDeptInvoice.inviID);
            Load_InwardDrugDetails(CurrentInwardDrugMedDeptInvoice.inviID);
            PharmacyPurchaseOrder_BySupplierID();
        }

        #endregion

        #region ListCost Member

        public void GridListCost_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        //▼====== #002
        private void InwardDrugMedDeptInvoice_GetListCost(long inviID)
        {
            isLoadingCost = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginInwardDrugMedDeptInvoice_GetListCost(inviID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {
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
                            isLoadingCost = false;
                            // Globals.IsBusy = false;
                        }
                    }), null);
                }
            });

            t.Start();
        }
        //▲====== #002

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

        public void hplDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z1406_G1_CoMuonBoDongNay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObjPNTemp_BySupplierIDList_Virtual.Remove(PNTemp_BySupplierIDSelected);
                //ObjPNTemp_BySupplierIDList.Refresh();
                CVS_ObjPNTemp_BySupplierIDList = new CollectionViewSource { Source = ObjPNTemp_BySupplierIDList_Virtual };
                CV_ObjPNTemp_BySupplierIDList = (CollectionView)CVS_ObjPNTemp_BySupplierIDList.View;
                //ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("inviID"));
                CV_ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("TempOutwardInvNum"));
            }
        }

        public void lnkRefGenMedProductDetail_Click(object sender, RoutedEventArgs e)
        {
            if (PNTemp_BySupplierIDSelected == null || PNTemp_BySupplierIDSelected.GenMedProductID == null || PNTemp_BySupplierIDSelected.GenMedProductID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.A0645_G1_Msg_InfoKhCoHgDeShow), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            GetRefGenMedProductDetails(PNTemp_BySupplierIDSelected.GenMedProductID.GetValueOrDefault(), PNTemp_BySupplierIDSelected.GenMedVersionID);
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
            this.ShowBusyIndicator();
            var t = new Thread(() =>
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
                            Action<ICMDrug_AddEdit_V3> onInitDlg_V3 = delegate (ICMDrug_AddEdit_V3 typeInfo)
                            {
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                                typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, product.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                                typeInfo.CanEdit = false;
                            };
                            GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V3>(onInitDlg_V3);
                            break;
                        }
                        else
                        {
                            Action<ICMDrug_AddEdit_V2> onInitDlg = delegate (ICMDrug_AddEdit_V2 typeInfo)
                            {
                                typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                                typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                                typeInfo.GetContraIndicatorDrugsRelToMedCondList(0, product.GenMedProductID);
                                typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                                typeInfo.CanEdit = false;
                            };
                            GlobalsNAV.ShowDialog<ICMDrug_AddEdit_V2>(onInitDlg);
                            break;
                        }
                    }
                case 11002:
                    {
                        Action<IDrugDept_MedDevAndChem_AddEdit> onInitDlg = delegate (IDrugDept_MedDevAndChem_AddEdit typeInfo)
                        {
                            typeInfo.V_MedProductType = V_MedProductType;
                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                            typeInfo.IsShowDispenseVolume = true;
                            typeInfo.IsShowMedicalMaterial = true;
                            typeInfo.SetRadioButton();
                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                            typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                            typeInfo.CanEdit = false;
                        };
                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg);
                        break;
                    }
                case 11003:
                    {
                        Action<IDrugDept_MedDevAndChem_AddEdit> onInitDlg = delegate (IDrugDept_MedDevAndChem_AddEdit typeInfo)
                        {
                            typeInfo.V_MedProductType = V_MedProductType;
                            typeInfo.ObjRefGenMedProductDetails_Current = ObjectCopier.DeepCopy(product);
                            typeInfo.IsShowDispenseVolume = true;
                            typeInfo.SupplierGenMedProduct_LoadDrugIDNotPaging(product.GenMedProductID);
                            typeInfo.TitleForm = string.Format("{0}: ", eHCMSResources.G0613_G1_TTinHg) + product.BrandName.Trim();
                            typeInfo.CanEdit = false;
                        };
                        GlobalsNAV.ShowDialog<IDrugDept_MedDevAndChem_AddEdit>(onInitDlg);
                        break;
                    }
            }
        }

        public void btnFilter(object sender, RoutedEventArgs e)
        {
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
                }
            }
            else
            {
                DrugDeptPoID = 0;
            }
        }

        public void btnDeleteItemNotInOrder(object sender, RoutedEventArgs e)
        {
            if (ObjPNTemp_BySupplierIDList_Virtual == null || ObjPNTemp_BySupplierIDList_Virtual.Count <= 0)
            {
                return;
            }

            if (MessageBox.Show(eHCMSResources.A0132_G1_Msg_ConfXoaDongKhongCoTrongP_DatHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObservableCollection<InwardDrugMedDept> list = ObjPNTemp_BySupplierIDList_Virtual.Where(x => x.DrugDeptPoDetailID != null && x.DrugDeptPoDetailID > 0).ToObservableCollection();
                ObjPNTemp_BySupplierIDList_Virtual = list.DeepCopy();
                CVS_ObjPNTemp_BySupplierIDList = new CollectionViewSource { Source = ObjPNTemp_BySupplierIDList_Virtual };
                CV_ObjPNTemp_BySupplierIDList = (CollectionView)CVS_ObjPNTemp_BySupplierIDList.View;

                //ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("inviID"));
                CV_ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("TempOutwardInvNum"));
            }
        }

        private void Process_AllOutwardChecked()
        {
            if (ObjPNTemp_BySupplierIDList_Virtual != null && ObjPNTemp_BySupplierIDList_Virtual.Count > 0)
            {
                for (int i = 0; i < ObjPNTemp_BySupplierIDList_Virtual.Count; i++)
                {
                    ObjPNTemp_BySupplierIDList_Virtual[i].IsCheckedForDel = true;
                }
            }
        }

        private void Process_AllOutwardUnChecked()
        {
            if (ObjPNTemp_BySupplierIDList_Virtual != null && ObjPNTemp_BySupplierIDList_Virtual.Count > 0)
            {
                for (int i = 0; i < ObjPNTemp_BySupplierIDList_Virtual.Count; i++)
                {
                    ObjPNTemp_BySupplierIDList_Virtual[i].IsCheckedForDel = false;
                }
            }
        }

        private void Process_AllInwardChecked()
        {
            if (ObjPNTemp_BySupplierIDList_Virtual != null && ObjPNTemp_BySupplierIDList_Virtual.Count > 0)
            {
                for (int i = 0; i < ObjPNTemp_BySupplierIDList_Virtual.Count; i++)
                {
                    ObjPNTemp_BySupplierIDList_Virtual[i].IsCheckedForDel = true;
                }
            }
        }

        private void Process_AllInwardUnChecked()
        {
            if (ObjPNTemp_BySupplierIDList_Virtual != null && ObjPNTemp_BySupplierIDList_Virtual.Count > 0)
            {
                for (int i = 0; i < ObjPNTemp_BySupplierIDList_Virtual.Count; i++)
                {
                    ObjPNTemp_BySupplierIDList_Virtual[i].IsCheckedForDel = false;
                }
            }
        }

        public void btnDeleteSelOutItem()
        {
            if (ObjPNTemp_BySupplierIDList_Virtual != null && ObjPNTemp_BySupplierIDList_Virtual.Count > 0)
            {
                var items = ObjPNTemp_BySupplierIDList_Virtual.Where(x => x.IsCheckedForDel == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        ObjPNTemp_BySupplierIDList_Virtual = ObjPNTemp_BySupplierIDList_Virtual.Where(x => x.IsCheckedForDel == false).ToObservableCollection();
                        CVS_ObjPNTemp_BySupplierIDList = new CollectionViewSource { Source = ObjPNTemp_BySupplierIDList_Virtual };
                        CV_ObjPNTemp_BySupplierIDList = (CollectionView)CVS_ObjPNTemp_BySupplierIDList.View;

                        //ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("inviID"));
                        CV_ObjPNTemp_BySupplierIDList.GroupDescriptions.Add(new PropertyGroupDescription("TempOutwardInvNum"));
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
            }
        }

        public void btnDeleteSelInItem()
        {
            if (InwardDrugMedDeptList != null && InwardDrugMedDeptList.Count > 0)
            {
                var items = InwardDrugMedDeptList.Where(x => x.IsCheckedForDel == true);
                if (items != null && items.Count() > 0)
                {
                    if (MessageBox.Show(eHCMSResources.Z0565_G1_CoChacXoaHangDaChon, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        var updList = InwardDrugMedDeptList.Where(x => x.IsCheckedForDel == false).ToList();
                        InwardDrugMedDeptList.Clear();
                        foreach (var item in updList)
                        {
                            InwardDrugMedDeptList.Add(item);
                        }                        
                    }
                }
                else
                {
                    MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
                }
            }
            else
            {
                MessageBox.Show(eHCMSResources.A0090_G1_Msg_InfoChuaChonHgCanXoa);
            }
        }
    }
}
