using eHCMSLanguage;
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
using Service.Core.Common;
using System.Windows.Media;
using aEMR.Controls;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using System.Text;
/*
* 20190624 #001 TTM:   BM 0011877: Lỗi thông tin auto complete không clear khi thay đổi nhà cung cấp => hàng của nhà cung cấp a sẽ được lưu cho nhà cung cấp b nếu người dùng không để ý.
*/
namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptPurchaseOrder)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptPurchaseOrderViewModel : ViewModelBase, IDrugDeptPurchaseOrder
        , IHandle<DrugDeptCloseSearchSupplierEvent>
        , IHandle<DrugDeptCloseSearchPurchaseOrderEstimationEvent>
        , IHandle<DrugDeptCloseSearchPurchaseOrderEvent>
        , IHandle<DrugDeptCloseSearchPharmaceuticalCompanyEvent>
    {
        public long V_SupplierType = 7200;
        private enum DataGridCol
        {
            ColDelete = 0,
            MaThuoc = 1,
            TenThuoc = 2,
            DaDat = 7,
            NoWaiting = 14,

            PackageQty = 9,
            QTy = 10,
            PackagePrice = 11,
            UnitPrice = 12
        }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public DrugDeptPurchaseOrderViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            GetAllCurrency(true);
            SearchCriteria = new RequestSearchCriteria();
            //SearchCriteria.IsNotOrder = true;
            DrugDeptPurchaseOrderDetailDeleted = new ObservableCollection<DrugDeptPurchaseOrderDetail>();
            CurrentDrugDeptPurchaseOrderDetail = new DrugDeptPurchaseOrderDetail();

            RefeshData();

            authorization();

            Coroutine.BeginExecute(DoGetPaymentModes());

            RefGenMedProductDetail = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetail.OnRefresh += RefGenMedProductDetail_OnRefresh;
            RefGenMedProductDetail.PageSize = Globals.PageSize;

            #region Warning Order Member

            RefGenMedProductDetailWarningOrders = new PagedSortableCollectionView<RefGenMedProductDetails>();
            RefGenMedProductDetailWarningOrders.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenMedProductDetailWarningOrders_OnRefresh);
            RefGenMedProductDetailWarningOrders.PageSize = Globals.PageSize;

            #endregion
            SearchSupplierAuto();
            GetPharmaceuticalCompanyCbx();
        }

        void RefGenMedProductDetailWarningOrders_OnRefresh(object sender, RefreshEventArgs e)
        {
            long? SupplierID = null;
            if (CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenMedProductDetailWarningOrders(RefGenMedProductDetailWarningOrders.PageIndex, RefGenMedProductDetailWarningOrders.PageSize, SupplierID);
        }

        void RefGenMedProductDetail_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenMedProductDetail_Auto(BrandName, RefGenMedProductDetail.PageIndex, RefGenMedProductDetail.PageSize, false);
        }

        public void LoadOrderWarning()
        {
            long? SupplierID = null;
            RefGenMedProductDetailWarningOrders.PageIndex = 0;
            if (CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenMedProductDetailWarningOrders(RefGenMedProductDetailWarningOrders.PageIndex, RefGenMedProductDetailWarningOrders.PageSize, SupplierID);
        }

        private void RefeshData()
        {
            CurrentDrugDeptPurchaseOrder = null;
            CurrentDrugDeptPurchaseOrder = new DrugDeptPurchaseOrder();
            SelectedSupplierCopy = null;
            SetDefaultCurrency();
            SetDefaultPaymentMode();
            DrugDeptPurchaseOrderDetailDeleted.Clear();
            Visibility = Visibility.Visible;
            ReadMoney = "";
        }

        #region 1. Property Member

        private string NONEITEM = string.Format(eHCMSResources.Z0996_G1_Format1, eHCMSResources.Z0913_G1_None);

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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
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

        private ObservableCollection<DrugDeptPharmaceuticalCompany> _DrugDeptPharmaceuticalCompanies;
        public ObservableCollection<DrugDeptPharmaceuticalCompany> DrugDeptPharmaceuticalCompanies
        {
            get { return _DrugDeptPharmaceuticalCompanies; }
            set
            {
                if (_DrugDeptPharmaceuticalCompanies != value)
                    _DrugDeptPharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => DrugDeptPharmaceuticalCompanies);
            }
        }

        private long? _PCOID;
        public long? PCOID
        {
            get { return _PCOID; }
            set
            {
                if (_PCOID != value)
                    _PCOID = value;
                NotifyOfPropertyChange(() => PCOID);
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
                NotifyOfPropertyChange(() => Visibility);
                if (Visibility == Visibility.Visible)
                {
                    ElseVisibility = Visibility.Collapsed;
                }
                else
                {
                    ElseVisibility = Visibility.Visible;
                }
            }
        }
        private Visibility _ElseVisibility;
        public Visibility ElseVisibility
        {
            get
            {
                return _ElseVisibility;
            }
            set
            {
                _ElseVisibility = value;
                NotifyOfPropertyChange(() => ElseVisibility);
            }
        }
        private RequestSearchCriteria _SearchCriteria;
        public RequestSearchCriteria SearchCriteria
        {
            get
            {
                return _SearchCriteria;
            }
            set
            {
                _SearchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private DrugDeptPurchaseOrder _CurrentDrugDeptPurchaseOrder;
        public DrugDeptPurchaseOrder CurrentDrugDeptPurchaseOrder
        {
            get
            {
                return _CurrentDrugDeptPurchaseOrder;
            }
            set
            {
                if (_CurrentDrugDeptPurchaseOrder != value)
                {
                    _CurrentDrugDeptPurchaseOrder = value;
                    SelectedBidID = _CurrentDrugDeptPurchaseOrder != null ? _CurrentDrugDeptPurchaseOrder.BidID : null;
                    SumTotalPrice();
                }
                NotifyOfPropertyChange(() => CurrentDrugDeptPurchaseOrder);
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

        private ObservableCollection<DrugDeptPurchaseOrderDetail> DrugDeptPurchaseOrderDetailDeleted;

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

        private decimal _TotalPriceNotVAT;
        public decimal TotalPriceNotVAT
        {
            get
            {
                return _TotalPriceNotVAT;
            }
            set
            {
                _TotalPriceNotVAT = value;
                NotifyOfPropertyChange(() => TotalPriceNotVAT);
            }
        }

        private decimal _TotalVAT;
        public decimal TotalVAT
        {
            get
            {
                return _TotalVAT;
            }
            set
            {
                _TotalVAT = value;
                NotifyOfPropertyChange(() => TotalVAT);
            }
        }

        private decimal _TotalPriceHaveVAT;
        public decimal TotalPriceHaveVAT
        {
            get
            {
                return _TotalPriceHaveVAT;
            }
            set
            {
                _TotalPriceHaveVAT = value;
                NotifyOfPropertyChange(() => TotalPriceHaveVAT);
            }
        }

        private string _ReadMoney;
        public string ReadMoney
        {
            get
            {
                return _ReadMoney;
            }
            set
            {
                _ReadMoney = value;
                NotifyOfPropertyChange(() => ReadMoney);
            }
        }

        private bool _IsAll = false;
        public bool IsAll
        {
            get
            {
                return _IsAll;
            }
            set
            {
                _IsAll = value;
                NotifyOfPropertyChange(() => IsAll);
            }
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
                NotifyOfPropertyChange(() => SelectedBidID);
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
        private bool _mDSCanDatHang = true;
        private bool _mTim = true;
        private bool _mChinhSua = true;
        private bool _mThemMoi = true;
        private bool _mIn = true;

        public bool mDSCanDatHang
        {
            get
            {
                return _mDSCanDatHang;
            }
            set
            {
                if (_mDSCanDatHang == value)
                    return;
                _mDSCanDatHang = value;
                NotifyOfPropertyChange(() => mDSCanDatHang);
            }
        }


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


        public bool mChinhSua
        {
            get
            {
                return _mChinhSua;
            }
            set
            {
                if (_mChinhSua == value)
                    return;
                _mChinhSua = value;
                NotifyOfPropertyChange(() => mChinhSua);
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


        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;

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
                NotifyOfPropertyChange(() => bEdit);
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
                NotifyOfPropertyChange(() => bAdd);
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
                NotifyOfPropertyChange(() => bDelete);
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
                NotifyOfPropertyChange(() => bView);
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
                NotifyOfPropertyChange(() => bPrint);
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
                NotifyOfPropertyChange(() => bReport);
            }
        }
        #endregion

        #region binding visibilty

        public Button lnkDelete { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bDelete);
        }

        #endregion

        ObservableCollection<Lookup> _PaymentModes;
        public ObservableCollection<Lookup> PaymentModes
        {
            get
            {
                return _PaymentModes;
            }
            set
            {
                if (_PaymentModes != value)
                {
                    _PaymentModes = value;
                    NotifyOfPropertyChange(() => PaymentModes);
                }
            }
        }

        private IEnumerator<IResult> DoGetPaymentModes()
        {
            var paymentTypeTask = new LoadLookupListTask(LookupValues.PAYMENT_MODE, false, false);
            yield return paymentTypeTask;
            PaymentModes = paymentTypeTask.LookupList;
            SetDefaultPaymentMode();
            yield break;
        }

        private void SetDefaultPaymentMode()
        {
            if (PaymentModes != null)
            {
                CurrentDrugDeptPurchaseOrder.V_PaymentMode = PaymentModes.FirstOrDefault().LookupID;
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void GetPharmaceuticalCompanyCbx()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugDeptPharmaceuticalCompanyCbx(Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetDrugDeptPharmaceuticalCompanyCbx(asyncResult);
                                DrugDeptPharmaceuticalCompanies = results.ToObservableCollection();
                                DrugDeptPharmaceuticalCompany item = new DrugDeptPharmaceuticalCompany();
                                item.PCOID = 0;
                                item.PCOName = NONEITEM;
                                DrugDeptPharmaceuticalCompanies.Insert(0, item);
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

        private void DrugDeptPurchaseOrderDetail_ByParentID(long DrugDeptPoID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrderDetail_ByParentID(DrugDeptPoID, 0, SelectedBidID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptPurchaseOrderDetail_ByParentID(asyncResult);
                                CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails = results.ToObservableCollection();
                                CurrentDrugDeptPurchaseOrder.BidID = SelectedBidID;
                                DrugDeptPurchaseOrderDetailDeleted.Clear();
                                SumTotalPrice();
                                //if (CurrentDrugDeptPurchaseOrder.CanSave)
                                //{
                                //    AddNewBlankRow();
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

        public void OpenPopUpSearchOrderInvoice(IList<DrugDeptPurchaseOrder> results, int Totalcount)
        {
            void onInitDlg(IDrugDeptPurchaseOrderSearch proAlloc)
            {
                proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                proAlloc.V_MedProductType = V_MedProductType;
                proAlloc.DrugDeptPurchaseOrderList.Clear();
                proAlloc.DrugDeptPurchaseOrderList.TotalItemCount = Totalcount;
                proAlloc.DrugDeptPurchaseOrderList.PageIndex = 0;
                if (results != null && results.Count > 0)
                {
                    foreach (DrugDeptPurchaseOrder p in results)
                    {
                        proAlloc.DrugDeptPurchaseOrderList.Add(p);
                    }
                }
            }
            GlobalsNAV.ShowDialog<IDrugDeptPurchaseOrderSearch>(onInitDlg);
        }

        private void DrugDeptPurchaseOrder_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchCriteria.Code))
            {
                SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                SearchCriteria.ToDate = Globals.GetCurServerDateTime();
            }
            else
            {
                SearchCriteria.FromDate = null;
                SearchCriteria.ToDate = null;
            }

            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrder_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndDrugDeptPurchaseOrder_Search(out Total, asyncResult);
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        OpenPopUpSearchOrderInvoice(results.ToList(), Total);
                                    }
                                    else
                                    {
                                        //lay thang gia tri
                                        CurrentDrugDeptPurchaseOrder = results.FirstOrDefault();
                                        HideColumn();
                                        DrugDeptPurchaseOrderDetail_ByParentID(CurrentDrugDeptPurchaseOrder.DrugDeptPoID);
                                        LoadValidBidFromSupplierID(CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID, V_MedProductType);
                                    }
                                }
                                else
                                {
                                    if (MessageBox.Show(eHCMSResources.Z0568_G1_KgTimThay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                                    {
                                        SearchCriteria.FromDate = Globals.GetCurServerDateTime().AddDays(-7);
                                        SearchCriteria.ToDate = Globals.GetCurServerDateTime();
                                        OpenPopUpSearchOrderInvoice(null, 0);
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

        private void SetDefaultCurrency()
        {
            if (CurrentDrugDeptPurchaseOrder != null && Currencies != null)
            {
                CurrentDrugDeptPurchaseOrder.CurrencyID = Currencies.FirstOrDefault().CurrencyID;
            }
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
                                SetDefaultCurrency();
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

        private void RefeshMaPhieu()
        {
            CurrentDrugDeptPurchaseOrder.PONumber = "";
            CurrentDrugDeptPurchaseOrder.DrugDeptPoID = 0;
        }

        private void DrugDeptPurchaseOrderDetail_GetFirst(long? DrugDeptEstimatePoID, long? SupplierID, long? PCOID)
        {
            RefeshMaPhieu();
            if (DrugDeptEstimatePoID == null || DrugDeptEstimatePoID <= 0)
            {
                if (CurrentDrugDeptPurchaseOrder != null)
                {
                    if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails != null)
                    {
                        CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Clear();
                    }
                }
                return;
            }
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrderDetail_GetFirst(DrugDeptEstimatePoID, SupplierID, PCOID, V_MedProductType, CurrentDrugDeptPurchaseOrder?.BidID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndDrugDeptPurchaseOrderDetail_GetFirst(asyncResult);
                                HideColumn();
                                if (CurrentDrugDeptPurchaseOrder.DrugDeptEstimationForPO != null && CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID > 0)
                                {
                                    if (results == null || results.Count == 0)
                                    {
                                        if (CurrentDrugDeptPurchaseOrder.SelectedSupplier == null || CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID == 0)
                                        {
                                            Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0806_G1_ChonNCCDatHg), eHCMSResources.G0442_G1_TBao);
                                        }
                                        else
                                        {
                                            Globals.ShowMessage(string.Format("{0} ", eHCMSResources.Z0861_G1_NCCKgCoTrongDuTru) + CurrentDrugDeptPurchaseOrder.DrugDeptEstimationForPO.EstimationCode + ".", eHCMSResources.G0442_G1_TBao);
                                        }
                                    }
                                }
                                CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails = results.ToObservableCollection();
                                DrugDeptPurchaseOrderDetailDeleted.Clear();
                                SumTotalPrice();
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

        private void DrugDeptEstimationForPO_Search(int PageIndex, int PageSize, long? BidID = null)
        {
            if (CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
            {
                SearchCriteria.SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
            }
            else
            {
                SearchCriteria.SupplierID = 0;
            }
            if (BidID.GetValueOrDefault(0) > 0)
            {
                var t = new Thread(() =>
                {
                    this.ShowBusyIndicator();
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPO_SearchByBid(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndDrugDeptEstimationForPO_SearchByBid(out Total, asyncResult);
                                this.HideBusyIndicator();
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        void onInitDlg(IDrugDeptPurchaseOrderSearchEstimate proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.DrugDeptEstimationForPOList.Clear();
                                            proAlloc.DrugDeptEstimationForPOList.TotalItemCount = Total;
                                            proAlloc.DrugDeptEstimationForPOList.PageIndex = 0;
                                            proAlloc.V_MedProductType = V_MedProductType;
                                            foreach (DrugDeptEstimationForPO p in results)
                                            {
                                                proAlloc.DrugDeptEstimationForPOList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IDrugDeptPurchaseOrderSearchEstimate>(onInitDlg);
                                    }
                                    else
                                    {
                                        //lay thang gia tri
                                        CurrentDrugDeptPurchaseOrder.DrugDeptEstimationForPO = results.FirstOrDefault();
                                        DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, CurrentDrugDeptPurchaseOrder.SupplierID, PCOID);
                                    }
                                }
                                else
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0808_G1_KgCoDuTruChoNCC), eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                });

                t.Start();
            }
            else
            {
                var t = new Thread(() =>
                {
                    this.ShowBusyIndicator();
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptEstimationForPO_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndDrugDeptEstimationForPO_Search(out Total, asyncResult);
                                this.HideBusyIndicator();
                                if (results != null && results.Count > 0)
                                {
                                    if (results.Count > 1)
                                    {
                                        //mo pop up tim
                                        void onInitDlg(IDrugDeptPurchaseOrderSearchEstimate proAlloc)
                                        {
                                            proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                            proAlloc.DrugDeptEstimationForPOList.Clear();
                                            proAlloc.DrugDeptEstimationForPOList.TotalItemCount = Total;
                                            proAlloc.DrugDeptEstimationForPOList.PageIndex = 0;
                                            proAlloc.V_MedProductType = V_MedProductType;
                                            foreach (DrugDeptEstimationForPO p in results)
                                            {
                                                proAlloc.DrugDeptEstimationForPOList.Add(p);
                                            }
                                        }
                                        GlobalsNAV.ShowDialog<IDrugDeptPurchaseOrderSearchEstimate>(onInitDlg);
                                    }
                                    else
                                    {
                                        //lay thang gia tri
                                        CurrentDrugDeptPurchaseOrder.DrugDeptEstimationForPO = results.FirstOrDefault();
                                        DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, CurrentDrugDeptPurchaseOrder.SupplierID, PCOID);
                                    }
                                }
                                else
                                {
                                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0808_G1_KgCoDuTruChoNCC), eHCMSResources.G0442_G1_TBao);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.HideBusyIndicator();
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                        }), null);
                    }
                });

                t.Start();
            }
        }

        private void DrugDeptPurchaseOrder_FullOperator()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            DrugDeptPurchaseOrderDetailDeleted.Clear();
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrder_FullOperator(CurrentDrugDeptPurchaseOrder, 0, SelectedBidID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                DrugDeptPurchaseOrder OrderOut;
                                long results = contract.EndDrugDeptPurchaseOrder_FullOperator(out OrderOut, asyncResult);
                                //Globals.ShowMessage(string.Format("{0}!", eHCMSResources.A0468_G1_Msg_InfoLuuOK), eHCMSResources.G0442_G1_TBao);
                                GlobalsNAV.ShowMessagePopup(eHCMSResources.A0468_G1_Msg_InfoLuuOK);
                                if (results >= 0)
                                {
                                    CurrentDrugDeptPurchaseOrder = OrderOut;
                                    //DrugDeptPurchaseOrderDetailDeleted.Clear();
                                    //if (CurrentDrugDeptPurchaseOrder.CanSave)
                                    //{
                                    //    AddNewBlankRow();
                                    //}
                                    ReLoadOrderWarning();
                                }
                                else
                                {
                                    RefeshData();
                                }
                            }
                            catch (Exception ex)
                            {
                                //KMx: Nếu những dòng delete nào không delete thành công thì phải set về trạng thái chỉnh sửa, để user có thể bấm lưu lần tiếp theo, nếu không thì trạng thái của dòng đó luôn luôn là delete và logic bị sai (27/01/2015 17:22).
                                //Làm cách này hơi tạm bợ, nhưng A. Tuấn kiu làm như vậy.
                                ObservableCollection<DrugDeptPurchaseOrderDetail> ListDeleteNotSuccess = CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Where(x => x.EntityState == EntityState.DETACHED).ToObservableCollection();
                                foreach (DrugDeptPurchaseOrderDetail item in ListDeleteNotSuccess)
                                {
                                    item.EntityState = EntityState.MODIFIED;
                                }
                                //Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        private void ReLoadOrderWarning()
        {
            long? SupplierID = null;
            RefGenMedProductDetailWarningOrders.PageIndex = 0;
            if (CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenMedProductDetailWarningOrders(RefGenMedProductDetailWarningOrders.PageIndex, RefGenMedProductDetailWarningOrders.PageSize, SupplierID);
        }

        public void KeyUp_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                DrugDeptPurchaseOrder_Search(0, Globals.PageSize);
            }
        }

        public void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            DrugDeptPurchaseOrder_Search(0, Globals.PageSize);
        }

        public void btnFindEstimate(object sender, RoutedEventArgs e)
        {
            DrugDeptEstimationForPO_Search(0, Globals.PageSize, CurrentDrugDeptPurchaseOrder == null ? null : CurrentDrugDeptPurchaseOrder.BidID);
        }

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void rdtTrongNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentDrugDeptPurchaseOrder.IsForeign = false;
        }

        public void rdtNgoaiNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentDrugDeptPurchaseOrder.IsForeign = true;
        }

        private bool CheckValidate()
        {
            if (CurrentDrugDeptPurchaseOrder == null)
            {
                return false;
            }
            return CurrentDrugDeptPurchaseOrder.Validate();
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            CurrentDrugDeptPurchaseOrder.StaffID = GetStaffLogin().StaffID;

            if (!CheckValidate())
            {
                return;
            }
            if (CurrentDrugDeptPurchaseOrder.V_PaymentMode == 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0809_G1_ChonHinhThucChiTra), eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails == null || CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count <= 0)
            {
                MessageBox.Show(eHCMSResources.K0431_G1_NhapDLieu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            StringBuilder sbForCheckVAT = new StringBuilder();
            for (int i = 0; i < CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count; i++)
            {
                if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].RefGenMedProductDetail == null)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0810_G1_ChonHgCanDat), eHCMSResources.G0442_G1_TBao);
                    return;
                }

                if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].PoUnitQty <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0811_G1_SLgDatHgLonHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }

                if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].UnitPrice < 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0812_G1_SLgDatHgKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }

                if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i] != null
                    && (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].VAT < 0
                    || CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].VAT > 1))
                {
                    sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2991_G1_VATKhongHopLe));
                }
                if ( CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].IsNotVat
                    && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].VAT != null)
                {
                    sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2992_G1_CoVatKhongTinhThue));
                }
            }
            if (!string.IsNullOrEmpty(sbForCheckVAT.ToString()))
            {
                MessageBox.Show(sbForCheckVAT.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentDrugDeptPurchaseOrder.DrugDeptPoID == 0)
            {
                CurrentDrugDeptPurchaseOrder.V_PurchaseOrderStatus = (long)AllLookupValues.V_PurchaseOrderStatus.NEW;
                CurrentDrugDeptPurchaseOrder.V_MedProductType = V_MedProductType;
            }
            else
            {
                //20200325 TBL: BM 0029042: Khi xóa chi tiết sẽ để vào ObservableCollection khác
                CurrentDrugDeptPurchaseOrder.PurchaseOrderDetailDeleted = new ObservableCollection<DrugDeptPurchaseOrderDetail>();
                for (int i = 0; i < DrugDeptPurchaseOrderDetailDeleted.Count; i++)
                {
                    CurrentDrugDeptPurchaseOrder.PurchaseOrderDetailDeleted.Add(DrugDeptPurchaseOrderDetailDeleted[i]);
                }
            }

            string msg = "";
            if (CurrentDrugDeptPurchaseOrder.DeliveryDayNo <= 0)
            {
                msg += string.Format("{0}.", eHCMSResources.Z0813_G1_ChuaNhapTGianGiaoHg) + Environment.NewLine;
            }
            if (CurrentDrugDeptPurchaseOrder.DeliveryMoneyDayNo <= 0)
            {
                msg += string.Format("{0}.", eHCMSResources.Z0814_G1_ChuaNhapTGianChiTra) + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(msg))
            {
                if (MessageBox.Show(msg + eHCMSResources.I0941_G1_I, eHCMSResources.G2363_G1_XNhan, MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    return;
                }
            }
            if (CurrentDrugDeptPurchaseOrder != null
                && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails != null && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count > 0
                && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Any(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.BidID > 0)
                && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Where(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.BidID > 0).Select(x => x.RefGenMedProductDetail.BidID).Distinct().Count() > 1)
            {
                GlobalsNAV.ShowMessagePopup(eHCMSResources.Z2740_G1_PhieuNhapCoChua2DThau);
                return;
            }
            if (CurrentDrugDeptPurchaseOrder != null
                && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails != null && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count > 0
                && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Any(x => Globals.ServerConfigSection.MedDeptElements.UseBidDetailOnInward && x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.BidID > 0 && x.RefGenMedProductDetail.BidRemainingQty < x.PoUnitQty))
            {
                if (MessageBox.Show(eHCMSResources.Z2736_G1_SoLuongCNTKoHopLe + Environment.NewLine + eHCMSResources.I0940_G1_I, eHCMSResources.G0442_G1_TBao, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (var eItem in CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Where(x => Globals.ServerConfigSection.MedDeptElements.UseBidDetailOnInward && x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.BidID > 0 && x.RefGenMedProductDetail.BidRemainingQty < x.PoUnitQty))
                    {
                        eItem.PoUnitQty = eItem.RefGenMedProductDetail.BidRemainingQty;
                        eItem.PoPackageQty = (double)(eItem.PoUnitQty / eItem.RefGenMedProductDetail.UnitPackaging);
                    }
                }
                return;
            }
            DrugDeptPurchaseOrder_FullOperator();
        }

        private bool ischanged(object item)
        {
            DrugDeptPurchaseOrderDetail p = (DrugDeptPurchaseOrderDetail)item;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }

                if (p.RefGenMedProductDetail != null && p.RefGenMedProductDetail.GenMedProductID != 0)
                {
                    var vars = CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Where(x => x.GenMedProductID == p.RefGenMedProductDetail.GenMedProductID && x.EntityState != EntityState.DETACHED);
                    if (vars.Count() > 1)
                    {
                        MessageBox.Show(eHCMSResources.K0044_G1_ThuocDaCoTrongPhDHg);
                        p.RefGenMedProductDetail = null;
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        string value = "";
        public void GridSuppliers_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            //AxTextBox tbl = GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) as AxTextBox;
            //if (tbl != null)
            //{
            //    value = tbl.Text.DeepCopy();
            //}
            var mAxTextBox = e.EditingElement.GetChildrenByType<AxTextBox>().FirstOrDefault();
            if (mAxTextBox != null)
            {
                value = mAxTextBox.Text;
            }
        }
        private DrugDeptPurchaseOrderDetail EditedDetailItem { get; set; }
        private string EditedColumnName { get; set; }
        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            EditedDetailItem = (e.Row.DataContext as DrugDeptPurchaseOrderDetail);
            EditedColumnName = null;
            if (e.Column.Equals(GridSuppliers.GetColumnByName("colUnitPrice")))
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
        }
        private void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (EditedDetailItem == null || string.IsNullOrEmpty(EditedColumnName))
            {
                return;
            }
            if (EditedColumnName.Equals("colUnitPrice"))
            {
                EditedColumnName = null;
                decimal ite = 0;
                decimal.TryParse(value, out ite);

                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.UnitPrice != ite)
                {
                    item.TotalPriceNotVAT = item.PoUnitQty * item.UnitPrice;
                    item.PackagePrice = item.UnitPrice * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                    if (item.EntityState == EntityState.PERSITED)
                    {
                        item.EntityState = EntityState.MODIFIED;
                    }
                    SumTotalPrice();
                }
            }
            else if (EditedColumnName.Equals("colPackagePrice"))
            {
                EditedColumnName = null;
                decimal ite = 0;
                decimal.TryParse(value, out ite);

                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PackagePrice != ite)
                {
                    item.TotalPriceNotVAT = (decimal)item.PoPackageQty * item.PackagePrice;
                    item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    if (item.UnitPrice != item.RefGenMedProductDetail.InBuyingPrice && item.RefGenMedProductDetail.InBuyingPrice > 0)
                    {
                        Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
                    }
                    if (item.EntityState == EntityState.PERSITED)
                    {
                        item.EntityState = EntityState.MODIFIED;
                    }
                    SumTotalPrice();
                }
            }
            else if (EditedColumnName.Equals("colPackagingQty"))
            {
                EditedColumnName = null;
                double ite = 0;
                double.TryParse(value, out ite);

                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PoPackageQty != ite)
                {
                    item.PoUnitQty = Convert.ToInt32(item.PoPackageQty * item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1));
                    item.TotalPriceNotVAT = (decimal)item.PoPackageQty * item.PackagePrice;
                    //if (item.PoPackageQty > 0)
                    //{
                    //    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PoPackageQty;
                    //    item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //}
                    if (item.EntityState == EntityState.PERSITED)
                    {
                        item.EntityState = EntityState.MODIFIED;
                    }
                    SumTotalPrice();
                }
            }
            else if (EditedColumnName.Equals("colQty"))
            {
                EditedColumnName = null;
                double ite = 0;
                Double.TryParse(value, out ite);

                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null && item.PoUnitQty != ite)
                {
                    item.PoPackageQty = (double)item.PoUnitQty / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    item.TotalPriceNotVAT = item.PoUnitQty * item.UnitPrice;
                    //if (item.PoPackageQty > 0)
                    //{
                    //    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PoPackageQty;
                    //   // item.UnitPrice = item.PackagePrice / item.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                    //}
                    if (item.EntityState == EntityState.PERSITED)
                    {
                        item.EntityState = EntityState.MODIFIED;
                    }
                    SumTotalPrice();
                }
            }
            else if (EditedColumnName.Equals("colVAT"))
            {
                EditedColumnName = null;
                DrugDeptPurchaseOrderDetail item = EditedDetailItem;
                if (item != null && item.RefGenMedProductDetail != null)
                {
                    if (item.EntityState == EntityState.PERSITED)
                    {
                        item.EntityState = EntityState.MODIFIED;
                    }
                }
            }
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

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptPurchaseOrder.VAT < 1 || CurrentDrugDeptPurchaseOrder.VAT > 2)
            {
                Globals.ShowMessage(string.Format("{0} ", eHCMSResources.K0263_G1_VATKhongHopLe2), eHCMSResources.G0442_G1_TBao);
                CurrentDrugDeptPurchaseOrder.VAT = 1;
            }
            else
            {
                TotalVAT = TotalPriceNotVAT * (decimal)(CurrentDrugDeptPurchaseOrder.VAT - 1);
                TotalPriceHaveVAT = TotalPriceNotVAT + TotalVAT;
                eHCMS.Services.Core.NumberToLetterConverter converter = new eHCMS.Services.Core.NumberToLetterConverter();
                ReadMoney = string.Format("{0} : ", eHCMSResources.G1581_G1_TgTienBangChu) + converter.Convert(Math.Round(TotalPriceHaveVAT, 4).ToString(), ',', eHCMSResources.Z0871_G1_Le) + string.Format(" {0}", eHCMSResources.Z0872_G1_Dong);
            }
        }

        #region Auto for Supplier

        private ObservableCollection<DrugDeptSupplier> _Suppliers;
        public ObservableCollection<DrugDeptSupplier> Suppliers
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

        private ObservableCollection<DrugDeptSupplier> _SupplierAlls;
        public ObservableCollection<DrugDeptSupplier> SupplierAlls
        {
            get
            {
                return _SupplierAlls;
            }
            set
            {
                if (_SupplierAlls != value)
                {
                    _SupplierAlls = value;
                }
                NotifyOfPropertyChange(() => SupplierAlls);
            }
        }

        private void SearchSupplierAuto()
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacySuppliersServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetSupplierDrugDept_ByPCOIDNotPaging(null, V_SupplierType, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetSupplierDrugDept_ByPCOIDNotPaging(asyncResult);
                                SupplierAlls = results.ToObservableCollection();
                                DrugDeptSupplier item = new DrugDeptSupplier
                                {
                                    SupplierID = 0,
                                    SupplierName = NONEITEM,
                                    SupplierCode = ""
                                };
                                SupplierAlls.Insert(0, item);
                                Suppliers = SupplierAlls.DeepCopy();

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

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        //public void AutoDrug_Text_DropDownClosed(object sender, System.Windows.RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
        //    {
        //        DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID, PCOID);
        //    }
        //    else if (CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails != null)
        //    {
        //        CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Clear();
        //    }
        //}

        private void SumTotalPrice()
        {
            TotalPriceNotVAT = 0;
            TotalPriceHaveVAT = 0;
            //TotalVAT = 0;
            if (CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails != null)
            {
                for (int i = 0; i < CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count; i++)
                {
                    TotalPriceNotVAT += CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].PoUnitQty * CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails[i].UnitPrice;
                }
                //if (CurrentDrugDeptPurchaseOrder.VAT > 1)
                //{
                //    TotalVAT = TotalPriceNotVAT * (decimal)(CurrentDrugDeptPurchaseOrder.VAT - 1);
                //}
                //else
                //{
                //    TotalVAT = 0;
                //}
                //TotalPriceHaveVAT = TotalPriceNotVAT + TotalVAT;
                CalcTotalVat(CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails);
                eHCMS.Services.Core.NumberToLetterConverter converter = new eHCMS.Services.Core.NumberToLetterConverter();
                decimal temp = 0;
                string prefix = "";
                if (TotalPriceHaveVAT < 0)
                {
                    temp = 0 - Math.Round(TotalPriceHaveVAT, 4);
                    prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am);
                }
                else
                {
                    temp = Math.Round(TotalPriceHaveVAT, 4);
                    prefix = "";
                }
                ReadMoney = string.Format("{0} : ", eHCMSResources.G1581_G1_TgTienBangChu) + prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le) + string.Format(" {0}", eHCMSResources.Z0872_G1_Dong);
            }
            NotifyOfPropertyChange(() => IsHadDetail);
        }
        #endregion

        #region AutoGenMedProduct Member
        private string BrandName;
        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetail;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetail
        {
            get
            {
                return _RefGenMedProductDetail;
            }
            set
            {
                if (_RefGenMedProductDetail != value)
                {
                    _RefGenMedProductDetail = value;
                    NotifyOfPropertyChange(() => RefGenMedProductDetail);
                }
            }
        }

        private void GetRefGenMedProductDetail_Auto(string BrandName, int PageIndex, int PageSize, bool? IsCode)
        {
            long? EstimationID = 0;
            long? SupplierID = 0;
            if (CurrentDrugDeptPurchaseOrder != null)
            {
                EstimationID = CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID;
                SupplierID = CurrentDrugDeptPurchaseOrder.SupplierID;
            }
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenMedProductDetails_AutoPurchaseOrder(BrandName, EstimationID, V_MedProductType, SupplierID, PageIndex, PageSize, IsCode, CurrentDrugDeptPurchaseOrder.BidID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            RefGenMedProductDetail.Clear();
                            RefGenMedProductDetail.TotalItemCount = 0;
                            int Total;
                            var results = contract.EndRefGenMedProductDetails_AutoPurchaseOrder(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (IsCode.GetValueOrDefault())
                                {
                                    SetValueCurrentOrder(results.FirstOrDefault());
                                }
                                else
                                {
                                    RefGenMedProductDetail.TotalItemCount = Total;
                                    foreach (RefGenMedProductDetails p in results)
                                    {
                                        RefGenMedProductDetail.Add(p);
                                    }
                                    AutoGenMedProduct.ItemsSource = RefGenMedProductDetail;
                                    AutoGenMedProduct.PopulateComplete();
                                }
                            }
                            else
                            {
                                if (IsCode.GetValueOrDefault())
                                {
                                    MessageBox.Show(string.Format("{0}.({1})!", eHCMSResources.Z0863_G1_KgTimThay, eHCMSResources.Z0862_G1_NCCKgCungCapHang));
                                    txt = "";
                                }
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

        AutoCompleteBox AutoGenMedProduct;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            AutoGenMedProduct = sender as AutoCompleteBox;
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (AutoGenMedProduct != null && !IsCode.GetValueOrDefault())
            {
                if (CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.SelectedSupplier != null && CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID > 0)
                {
                    RefGenMedProductDetail.PageIndex = 0;
                    BrandName = e.Parameter;
                    GetRefGenMedProductDetail_Auto(e.Parameter, RefGenMedProductDetail.PageIndex, RefGenMedProductDetail.PageSize, false);
                }
                else
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0817_G1_ChonNCC), eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, System.Windows.RoutedPropertyChangedEventArgs<bool> e)
        {
            if (AutoGenMedProduct != null)
            {
                if (CurrentDrugDeptPurchaseOrderDetail != null && AutoGenMedProduct.SelectedItem != null)
                {
                    SetValueCurrentOrder(AutoGenMedProduct.SelectedItem);
                }
            }
        }

        private void SetValueCurrentOrder(object Item)
        {
            if (CurrentDrugDeptPurchaseOrderDetail == null)
            {
                CurrentDrugDeptPurchaseOrderDetail = new DrugDeptPurchaseOrderDetail();
            }
            CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail = Item as RefGenMedProductDetails;
            CurrentDrugDeptPurchaseOrderDetail.EstimateQty = Convert.ToInt32(CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.RequestQty);
            CurrentDrugDeptPurchaseOrderDetail.OrderedQty = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Ordered.GetValueOrDefault();
            if (CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.RequestQty - CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Ordered.GetValueOrDefault() > 0)
            {
                CurrentDrugDeptPurchaseOrderDetail.PoUnitQty = Convert.ToInt32(CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.RequestQty) - CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.Ordered.GetValueOrDefault();
            }
            else
            {
                CurrentDrugDeptPurchaseOrderDetail.PoUnitQty = 0;
            }
            CurrentDrugDeptPurchaseOrderDetail.UnitPrice = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.UnitPrice;
            CurrentDrugDeptPurchaseOrderDetail.PackagePrice = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.PackagePrice;
            CurrentDrugDeptPurchaseOrderDetail.VAT = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.VAT;
            CurrentDrugDeptPurchaseOrderDetail.IsNotVat = CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.IsNotVat;
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchSupplierEvent> Members

        public void Handle(DrugDeptCloseSearchSupplierEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentDrugDeptPurchaseOrder.SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
                DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID, PCOID);
                //20200325 TBL: BM 0029042: Chọn NCC không load thông tin thầu của NCC đó
                LoadValidBidFromSupplierID(CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID, V_MedProductType);
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchPurchaseOrderEstimationEvent> Members

        public void Handle(DrugDeptCloseSearchPurchaseOrderEstimationEvent message)
        {
            if (message != null && IsActive)
            {
                CurrentDrugDeptPurchaseOrder.DrugDeptEstimationForPO = message.SelectedEstimation as DrugDeptEstimationForPO;
                long SupplierID = 0;
                try
                {
                    SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;

                }
                catch { SupplierID = 0; }
                //if (SupplierID > 0)
                //{
                DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, SupplierID, PCOID);
                //}
            }
        }

        #endregion

        #region IHandle<DrugDeptCloseSearchPurchaseOrderEvent> Members

        public void Handle(DrugDeptCloseSearchPurchaseOrderEvent message)
        {
            if (message != null && IsActive)
            {
                DrugDeptPurchaseOrder item = message.SelectedPurchaseOrder as DrugDeptPurchaseOrder;
                SelectedSupplierCopy = item.SelectedSupplier;
                //20200325 TBL: BM 0029042: Khi load lại phiếu đặt hàng không lấy thông tin thầu hiển thị trên cbb
                LoadValidBidFromSupplierID(SelectedSupplierCopy.SupplierID, V_MedProductType);
                CurrentDrugDeptPurchaseOrder = item;
                if (PCOID != null || PCOID > 0)
                {
                    PCOID = 0;
                }
                else
                {
                    Suppliers = SupplierAlls.DeepCopy();
                }
                if (SelectedSupplierCopy != null && SupplierAlls != null)
                {
                    var value = SupplierAlls.Where(x => x.SupplierID == SelectedSupplierCopy.SupplierID);
                    if (value == null || value.Count() == 0)
                    {
                        Visibility = Visibility.Collapsed;
                        //if(MessageBox.Show("NCC "+SelectedSupplierCopy.SupplierName +" đã không còn hoạt động."+Environment.NewLine+"Bạn có muốn in lại phiếu này để xem 1 cách chính xác?",eHCMSResources.G0442_G1_TBao,MessageBoxButton.OKCancel)==MessageBoxResult.OK)
                        //{
                        //    btnPreview();
                        //}
                    }
                    else
                    {
                        Visibility = Visibility.Visible;
                    }
                }
                HideColumn();
                //load chi tiet phieu du tru
                DrugDeptPurchaseOrderDetail_ByParentID(CurrentDrugDeptPurchaseOrder.DrugDeptPoID);
            }
        }

        #endregion

        private void DrugDeptPurchaseOrder_Delete(long DrugDeptPoID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrders_Delete(DrugDeptPoID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDrugDeptPurchaseOrders_Delete(asyncResult);
                                RefeshData();
                                TotalPriceHaveVAT = 0;
                                TotalPriceNotVAT = 0;
                                TotalVAT = 0;
                                ReLoadOrderWarning();
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

        #region link button member

        public void btnNewPurchase()
        {
            if (MessageBox.Show(eHCMSResources.Z0864_G1_CoMuonTaoMoiDonHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshData();
            }
        }

        public void btnDeletePurchase()
        {
            if (CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.DrugDeptPoID > 0)
            {
                if (MessageBox.Show(eHCMSResources.A0157_G1_Msg_ConfXoaDonHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DrugDeptPurchaseOrder_Delete(CurrentDrugDeptPurchaseOrder.DrugDeptPoID);
                }
            }
            else
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0577_G1_PhKgTonTai), "");
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if(IsEstimateFromRequest)
            {
                Globals.ShowMessage("Đơn hàng tổng hợp từ phiếu lĩnh dự trù không thể xoá chi tiết!", eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentDrugDeptPurchaseOrder.CanSave)
            {
                if (GridSuppliers != null && GridSuppliers.SelectedItem != null)
                {
                    DrugDeptPurchaseOrderDetail item = GridSuppliers.SelectedItem as DrugDeptPurchaseOrderDetail;
                    if (item.EntityState != EntityState.NEW)
                    {
                        item.EntityState = EntityState.DETACHED;
                        DrugDeptPurchaseOrderDetailDeleted.Add(item);
                    }
                    CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Remove(item);
                    CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails = CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.ToObservableCollection();
                    SumTotalPrice();
                }
                //if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails == null || CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count == 0)
                //{
                //    AddNewBlankRow();
                //}
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0820_G1_PhDaGuiDenNCCKgDuocSua, eHCMSResources.G0442_G1_TBao);
            }
        }

        #endregion

        public void lnkCanWaiting_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null && chk.IsChecked == true && CurrentDrugDeptPurchaseOrderDetail != null && CurrentDrugDeptPurchaseOrderDetail.DrugDeptPoDetailID > 0)
            {
                if (CurrentDrugDeptPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY || CurrentDrugDeptPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED)
                {
                    if (MessageBox.Show(eHCMSResources.Z0865_G1_CoChacKgChoNuaKg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DrugDeptPurchaseOrderDetail_UpdateNoWaiting(CurrentDrugDeptPurchaseOrderDetail.DrugDeptPoDetailID, true);
                    }
                    else
                    {
                        CurrentDrugDeptPurchaseOrderDetail.NoWaiting = false;
                    }
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z0822_G1_DonHgChuaGuiDi, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z0823_G1_CoChacGuiDonHgDenNCC + Environment.NewLine + string.Format("{0}!", eHCMSResources.Z0824_G1_KgDuocSuaDonHgDaGui), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DrugDeptPurchaseOrder_UpdateStatus(CurrentDrugDeptPurchaseOrder.DrugDeptPoID, (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED);
            }
        }

        public void hblNoWaiting()
        {
            if (MessageBox.Show(eHCMSResources.Z0825_G1_CoChacKgChoNCCGiaoDuHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DrugDeptPurchaseOrder_UpdateStatus(CurrentDrugDeptPurchaseOrder.DrugDeptPoID, (long)AllLookupValues.V_PurchaseOrderStatus.NO_WAITING);
            }
        }

        private void DrugDeptPurchaseOrder_UpdateStatus(long ID, long V_PurchaseOrderStatus)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrders_UpdateStatus(ID, V_PurchaseOrderStatus, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDrugDeptPurchaseOrders_UpdateStatus(asyncResult);
                                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0826_G1_CNhatTThaiThCong), eHCMSResources.G0442_G1_TBao);
                                DrugDeptPurchaseOrder_ID(ID);
                                DrugDeptPurchaseOrderDetail_ByParentID(ID);
                                if (V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.NO_WAITING)
                                {
                                    ReLoadOrderWarning();
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

        private void DrugDeptPurchaseOrderDetail_UpdateNoWaiting(long ID, bool? NoWaiting)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrderDetail_UpdateNoWaiting(ID, NoWaiting, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                contract.EndDrugDeptPurchaseOrderDetail_UpdateNoWaiting(asyncResult);
                                ReLoadOrderWarning();
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

        private void DrugDeptPurchaseOrder_ID(long ID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginDrugDeptPurchaseOrder_ByID(ID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                CurrentDrugDeptPurchaseOrder = contract.EndDrugDeptPurchaseOrder_ByID(asyncResult);
                                if (CurrentDrugDeptPurchaseOrder != null)
                                {
                                    HideColumn();
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

        private void HideColumn()
        {
            if (GridSuppliers != null)
            {
                if (CurrentDrugDeptPurchaseOrder.DrugDeptPoID > 0)
                {
                    GridSuppliers.Columns[(int)DataGridCol.DaDat].Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridSuppliers.Columns[(int)DataGridCol.DaDat].Visibility = Visibility.Visible;
                }
                if (CurrentDrugDeptPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED || CurrentDrugDeptPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY)
                {
                    GridSuppliers.Columns[(int)DataGridCol.NoWaiting].Visibility = Visibility.Visible;
                }
                else
                {
                    GridSuppliers.Columns[(int)DataGridCol.NoWaiting].Visibility = Visibility.Collapsed;
                }
            }
        }

        #region Print member

        public void btnPreview()
        {
            void onInitDlg(IDrugDeptReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentDrugDeptPurchaseOrder.DrugDeptPoID;
                proAlloc.eItem = ReportName.DRUGDEPT_ORDER;
            }
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        #endregion

        #region Warning Order Member

        private PagedSortableCollectionView<RefGenMedProductDetails> _RefGenMedProductDetailWarningOrders;
        public PagedSortableCollectionView<RefGenMedProductDetails> RefGenMedProductDetailWarningOrders
        {
            get
            {
                return _RefGenMedProductDetailWarningOrders;
            }
            set
            {
                _RefGenMedProductDetailWarningOrders = value;
                NotifyOfPropertyChange(() => RefGenMedProductDetailWarningOrders);
            }
        }

        private void Get_RefGenMedProductDetailWarningOrders(int PageIndex, int PageSize, long? SupplierID)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyEstimattionServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginRefGenMedProductDetails_WarningOrder(V_MedProductType, PageIndex, PageSize, SupplierID, IsAll, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndRefGenMedProductDetails_WarningOrder(out Total, asyncResult);
                                RefGenMedProductDetailWarningOrders.Clear();
                                RefGenMedProductDetailWarningOrders.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (RefGenMedProductDetails p in results)
                                    {
                                        RefGenMedProductDetailWarningOrders.Add(p);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        public void GridRefGenMedProductDetailWarningOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            RefGenMedProductDetails item = e.Row.DataContext as RefGenMedProductDetails;
            if (item.FactorSafety - item.Remaining > item.WaitingDeliveryQty)
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        public void RefeshListOrder()
        {
            long? SupplierID = null;
            RefGenMedProductDetailWarningOrders.PageIndex = 0;
            if (CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenMedProductDetailWarningOrders(RefGenMedProductDetailWarningOrders.PageIndex, RefGenMedProductDetailWarningOrders.PageSize, SupplierID);
        }

        #endregion

        private DrugDeptSupplier SelectedSupplierCopy;
        public void cbxNSX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupplierAlls != null)
            {
                if (CurrentDrugDeptPurchaseOrder != null)
                {
                    SelectedSupplierCopy = CurrentDrugDeptPurchaseOrder.SelectedSupplier.DeepCopy();
                }
                if (PCOID != null && PCOID > 0)
                {
                    Suppliers = SupplierAlls.Where(x => ((x.SupplierID == 0) || x.ListPCOID != null && x.ListPCOID.Contains(PCOID.ToString() + ","))).ToObservableCollection();
                }
                else
                {
                    Suppliers = SupplierAlls.DeepCopy();
                }
                if (SelectedSupplierCopy != null)
                {
                    var value = Suppliers.Where(x => x.SupplierID == SelectedSupplierCopy.SupplierID);
                    if (value == null || value.Count() == 0)
                    {
                        CurrentDrugDeptPurchaseOrder.SelectedSupplier = null;
                        SelectedSupplierCopy = null;
                    }
                }

                long SupplierID = 0;
                try
                {
                    SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
                }
                catch { SupplierID = 0; }
                //  DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, SupplierID, PCOID);
            }
        }

        public void Supplier_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;
            if (axApThau != null && Suppliers != null)
            {
                axApThau.ItemsSource = Suppliers.Where(x => aEMR.Common.Utilities.StringUtil.RemoveSign4VietnameseString(x.SupplierName).ToUpper().Contains(aEMR.Common.Utilities.StringUtil.RemoveSign4VietnameseString(e.Parameter.ToUpper())) || (x.SupplierCode != null && x.SupplierCode.ToUpper().Contains(e.Parameter.ToUpper())));
                axApThau.PopulateComplete();
            }
        }

        public void Supplier_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            AutoCompleteBox axApThau = sender as AutoCompleteBox;

            if (CurrentDrugDeptPurchaseOrder != null)
            {
                if (SelectedSupplierCopy == null || !SelectedSupplierCopy.Equals(CurrentDrugDeptPurchaseOrder.SelectedSupplier))
                {
                    long SupplierID = 0;
                    try
                    {
                        if(CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.SelectedSupplier != null)
                        {
                            SupplierID = CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID;
                        }
                    }
                    catch { SupplierID = 0; }
                    DrugDeptPurchaseOrderDetail_GetFirst(CurrentDrugDeptPurchaseOrder.DrugDeptEstimatePoID, SupplierID, PCOID);
                    SelectedSupplierCopy = CurrentDrugDeptPurchaseOrder.SelectedSupplier.DeepCopy();
                    if (SupplierID > 0)
                    {
                        LoadValidBidFromSupplierID(SupplierID, V_MedProductType);
                    }
                    else
                    {
                        BidCollection = new ObservableCollection<Bid>();
                    }
                    //▼===== #001
                    AutoGenMedProduct.ItemsSource = new PagedSortableCollectionView<RefGenMedProductDetails>();
                    //▲===== #001
                }
            }
        }

        //Callback method 
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search 
            Supplier emp = o as Supplier;
            if (emp != null)
            {
                if (emp.SupplierID == 0)
                {
                    return true;
                }
                if (emp.SupplierID > 0 && emp.ListPCOID == null)
                {
                    return false;
                }
                string[] list = emp.ListPCOID.Split(',');
                if (list != null)
                {
                    for (int i = 0; i < list.Count(); i++)
                    {
                        if (list[i].ToLower().Equals(PCOID.ToString().Trim().ToLower()))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void btnSearchNSX()
        {
            void onInitDlg(IDrugDeptPharmacieucalCompany proAlloc)
            {
                proAlloc.IsChildWindow = true;
            }
            GlobalsNAV.ShowDialog<IDrugDeptPharmacieucalCompany>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #region IHandle<DrugDeptCloseSearchPharmaceuticalCompanyEvent> Members

        public void Handle(DrugDeptCloseSearchPharmaceuticalCompanyEvent message)
        {
            if (this.IsActive && message.SelectedPharmaceuticalCompany != null)
            {
                PCOID = (message.SelectedPharmaceuticalCompany as DrugDeptPharmaceuticalCompany).PCOID;
            }
        }

        #endregion

        #region search by code member
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentDrugDeptPurchaseOrder == null || CurrentDrugDeptPurchaseOrder.SelectedSupplier == null || CurrentDrugDeptPurchaseOrder.SelectedSupplier.SupplierID <= 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0817_G1_ChonNCC), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    string Code = Globals.FormatCode(V_MedProductType, txt);

                    RefGenMedProductDetail.PageIndex = 0;
                    GetRefGenMedProductDetail_Auto(Code, RefGenMedProductDetail.PageIndex, RefGenMedProductDetail.PageSize, true);
                }
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
                if (CurrentDrugDeptPurchaseOrder != null)
                {
                    return _VisibilityName && CurrentDrugDeptPurchaseOrder.CanSave;
                }
                return _VisibilityName;
            }
            set
            {
                if (CurrentDrugDeptPurchaseOrder != null)
                {
                    _VisibilityName = value && CurrentDrugDeptPurchaseOrder.CanSave;
                    _VisibilityCode = !_VisibilityName && CurrentDrugDeptPurchaseOrder.CanSave;
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
                return _VisibilityCode;
            }
            set
            {
                if (_VisibilityCode != value)
                {
                    _VisibilityCode = value;
                    NotifyOfPropertyChange(() => VisibilityCode);
                }
            }
        }

        private bool? IsCode = false;
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

        private string txtQuantity = "";
        public void txtQuantity_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (txtQuantity != (sender as AxTextBox).Text)
            {
                txtQuantity = (sender as AxTextBox).Text;
                if (!string.IsNullOrEmpty(txtQuantity))
                {
                    double ite = 0;
                    Double.TryParse(txtQuantity, out ite);
                    if (CurrentDrugDeptPurchaseOrderDetail != null)
                    {
                        if (CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail != null && CurrentDrugDeptPurchaseOrderDetail.PoPackageQty != ite)
                        {
                            CurrentDrugDeptPurchaseOrderDetail.PoPackageQty = (double)CurrentDrugDeptPurchaseOrderDetail.PoUnitQty / (double)CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.UnitPackaging.GetValueOrDefault(1);
                            CurrentDrugDeptPurchaseOrderDetail.TotalPriceNotVAT = CurrentDrugDeptPurchaseOrderDetail.PoUnitQty * CurrentDrugDeptPurchaseOrderDetail.UnitPrice;
                        }
                    }

                }
            }
        }
        public void AddItem()
        {
            if (CurrentDrugDeptPurchaseOrderDetail == null || CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail == null)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0867_G1_VuiLongChonHg), eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentDrugDeptPurchaseOrderDetail.PoUnitQty <= 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0811_G1_SLgDatHgLonHon0), eHCMSResources.G0442_G1_TBao);
                return;
            }

            if (CurrentDrugDeptPurchaseOrder == null)
            {
                CurrentDrugDeptPurchaseOrder = new DrugDeptPurchaseOrder();
            }
            if (CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails == null)
            {
                CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails = new ObservableCollection<DrugDeptPurchaseOrderDetail>();
            }
            var chk = CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Where(x => x.RefGenMedProductDetail != null && x.RefGenMedProductDetail.GenMedProductID == CurrentDrugDeptPurchaseOrderDetail.RefGenMedProductDetail.GenMedProductID);
            if (chk != null && chk.Count() > 0)
            {
                Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0827_G1_MatHgDaTonTai), eHCMSResources.G0442_G1_TBao);
                return;
            }
            CurrentDrugDeptPurchaseOrderDetail.EntityState = EntityState.NEW;
            CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Insert(0, CurrentDrugDeptPurchaseOrderDetail);
            SumTotalPrice();
            CurrentDrugDeptPurchaseOrderDetail = new DrugDeptPurchaseOrderDetail();
            if (IsCode.GetValueOrDefault())
            {
                if (tbx != null)
                {
                    tbx.Focus();
                }
            }
            else
            {
                if (AutoGenMedProduct != null)
                {
                    AutoGenMedProduct.Text = "";
                    AutoGenMedProduct.Focus();
                }
            }
        }
        #endregion

        public void CalcTotalVat(ObservableCollection<DrugDeptPurchaseOrderDetail> ListPurchaseOrderDetails)
        {
            TotalVAT = 0;
            if (ListPurchaseOrderDetails != null && ListPurchaseOrderDetails.Count > 0)
            {
                foreach(var detail in ListPurchaseOrderDetails)
                {
                    if(detail.VAT >= 0)
                    {
                        TotalVAT += detail.PoUnitQty * detail.UnitPrice * (decimal)detail.VAT;
                    }
                }
                TotalPriceHaveVAT = TotalPriceNotVAT + TotalVAT;
            }
        }


        public void btnCheckOrder()
        {
            void onInitDlg(IDrugDeptCheckOrder proAlloc)
            {
                proAlloc.V_MedProductType = V_MedProductType;
            }
            GlobalsNAV.ShowDialog<IDrugDeptCheckOrder>(onInitDlg);
        }
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
            this.ShowBusyIndicator();
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
                                    mReturnValues.Insert(0, new Bid { BidName = " " });
                                    BidCollection = mReturnValues.ToObservableCollection();
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

        public bool IsHadDetail
        {
            get
            {
                return CurrentDrugDeptPurchaseOrder != null && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails != null && CurrentDrugDeptPurchaseOrder.PurchaseOrderDetails.Count > 0
                    && Globals.ServerConfigSection.MedDeptElements.UseBidDetailOnInward;
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
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).EntityState = EntityState.MODIFIED;
                }
                else
                {
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).IsNotVat = false;
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).VAT = 0;
                    (chkIsNotVat.DataContext as DrugDeptPurchaseOrderDetail).EntityState = EntityState.MODIFIED;
                }
            }
        }

        public void cbxBid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbxBid = sender as ComboBox;
            if (cbxBid != null)
            {
                if (cbxBid.SelectedItem != null)
                {
                    SelectedBidID = (cbxBid.SelectedItem as Bid).BidID;
                }
            }
        }

        private bool _IsEstimateFromRequest = false;
        public bool IsEstimateFromRequest
        {
            get
            {
                return _IsEstimateFromRequest;
            }
            set
            {
                _IsEstimateFromRequest = value;
                NotifyOfPropertyChange(() => IsEstimateFromRequest);
            }
        }
    }
}
