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
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using Castle.Windsor;
using System.Text;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPurchaseOrder)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseOrderViewModel : ViewModelBase, IPurchaseOrder
        , IHandle<PharmacyCloseSearchSupplierEvent>, IHandle<PharmacyCloseSearchPurchaseOrderEstimationEvent>
        , IHandle<PharmacyCloseSearchPurchaseOrderEvent>, IHandle<PharmacyCloseSearchPharmaceuticalCompanyEvent>
    {
        public string TitleForm { get; set; }

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

        private bool _isLoadingWarning = false;
        public bool isLoadingWarning
        {
            get { return _isLoadingWarning; }
            set
            {
                if (_isLoadingWarning != value)
                {
                    _isLoadingWarning = value;
                    NotifyOfPropertyChange(() => isLoadingWarning);
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

        private bool _isLoadingPharmaceutialCompany = false;
        public bool isLoadingPharmaceutialCompany
        {
            get { return _isLoadingPharmaceutialCompany; }
            set
            {
                if (_isLoadingPharmaceutialCompany != value)
                {
                    _isLoadingPharmaceutialCompany = value;
                    NotifyOfPropertyChange(() => isLoadingPharmaceutialCompany);
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

        private bool _isLoadingEstimate_Search = false;
        public bool isLoadingEstimate_Search
        {
            get { return _isLoadingEstimate_Search; }
            set
            {
                if (_isLoadingEstimate_Search != value)
                {
                    _isLoadingEstimate_Search = value;
                    NotifyOfPropertyChange(() => isLoadingEstimate_Search);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingOrder_Search = false;
        public bool isLoadingOrder_Search
        {
            get { return _isLoadingOrder_Search; }
            set
            {
                if (_isLoadingOrder_Search != value)
                {
                    _isLoadingOrder_Search = value;
                    NotifyOfPropertyChange(() => isLoadingOrder_Search);
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

        public bool IsLoading
        {
            get { return (isLoadingCurrency || isLoadingFullOperator || isLoadingInvoiceDetailD || isLoadingOrder_Search || isLoadingOrderDetailID || isLoadingOrderID || isLoadingPharmaceutialCompany || isLoadingWarning); }
        }

        #endregion
        public long V_MedProductType = 11001;
        public long V_SupplierType = 7200;
        private enum DataGridCol
        {
            ColDelete = 0,
            MaThuoc = 1,
            TenThuoc = 2,
            DaDat = 7,
            NoWaiting = 15,

            PackageQty = 10,
            QTy = 11,
            PackagePrice = 12,
            UnitPrice = 13,
            VAT = 16,
            IsNotVAt = 17
        }
        [ImportingConstructor]
        public PurchaseOrderViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            Globals.EventAggregator.Subscribe(this);
            GetAllCurrency(true);
            SearchCriteria = new RequestSearchCriteria();
            //SearchCriteria.IsNotOrder = true;
            PharmacyPurchaseOrderDetailDeleted = new ObservableCollection<PharmacyPurchaseOrderDetail>();
            CurrentPharmacyPurchaseOrderDetail = new PharmacyPurchaseOrderDetail();
            RefeshData();

            authorization();

            Coroutine.BeginExecute(DoGetPaymentModes());

            RefGenericDrugDetail = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetail.OnRefresh += RefGenericDrugDetail_OnRefresh;
            RefGenericDrugDetail.PageSize = Globals.PageSize;

            #region Warning Order Member

            RefGenericDrugDetailWarningOrders = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetailWarningOrders.OnRefresh += new EventHandler<RefreshEventArgs>(RefGenericDrugDetailWarningOrders_OnRefresh);
            RefGenericDrugDetailWarningOrders.PageSize = Globals.PageSize;
            RefGenericDrugDetailWarningOrders.PageIndex = 0;

            long? SupplierID = null;
            if (CurrentPharmacyPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenericDrugDetailWarningOrders(RefGenericDrugDetailWarningOrders.PageIndex, RefGenericDrugDetailWarningOrders.PageSize, SupplierID);

            #endregion
            SearchSupplierAuto();
            GetPharmaceuticalCompanyCbx();
        }

        void RefGenericDrugDetailWarningOrders_OnRefresh(object sender, RefreshEventArgs e)
        {
            long? SupplierID = null;
            if (CurrentPharmacyPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenericDrugDetailWarningOrders(RefGenericDrugDetailWarningOrders.PageIndex, RefGenericDrugDetailWarningOrders.PageSize, SupplierID);
        }

        void RefGenericDrugDetail_OnRefresh(object sender, RefreshEventArgs e)
        {
            GetRefGenericDrugDetail_Auto(BrandName, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize, false);
        }

        private void RefeshData()
        {
            CurrentPharmacyPurchaseOrder = null;
            CurrentPharmacyPurchaseOrder = new PharmacyPurchaseOrder();
            SelectedSupplierCopy = null;
            SetDefaultCurrency();
            SetDefaultPaymentMode();
            PharmacyPurchaseOrderDetailDeleted.Clear();
            Visibility = Visibility.Visible;
        }

      
        #region 1. Property Member

        private string NONEITEM = string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z0913_G1_None);

        private ObservableCollection<PharmaceuticalCompany> _pharmaceuticalCompanies;
        public ObservableCollection<PharmaceuticalCompany> PharmaceuticalCompanies
        {
            get { return _pharmaceuticalCompanies; }
            set
            {
                if (_pharmaceuticalCompanies != value)
                    _pharmaceuticalCompanies = value;
                NotifyOfPropertyChange(() => PharmaceuticalCompanies);
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

        private PharmacyPurchaseOrder _CurrentPharmacyPurchaseOrder;
        public PharmacyPurchaseOrder CurrentPharmacyPurchaseOrder
        {
            get
            {
                return _CurrentPharmacyPurchaseOrder;
            }
            set
            {
                if (_CurrentPharmacyPurchaseOrder != value)
                {
                    _CurrentPharmacyPurchaseOrder = value;
                    SumTotalPrice();
                }
                NotifyOfPropertyChange(() => CurrentPharmacyPurchaseOrder);
                NotifyOfPropertyChange(() => VisibilityCode);
                NotifyOfPropertyChange(() => VisibilityName);
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

        private ObservableCollection<PharmacyPurchaseOrderDetail> PharmacyPurchaseOrderDetailDeleted;

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
        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_DSThuocCanDatHang, (int)ePermission.mView);
            mDatHang_Tim = mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_Tim, (int)ePermission.mView);
            mDatHang_ThongTin = mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_ThongTin, (int)ePermission.mView);
            mDatHang_Edit = mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_Edit, (int)ePermission.mView);
            mDatHang_Them = mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_Them, (int)ePermission.mView);

            mDatHang_Xoa = mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_Xoa, (int)ePermission.mView);
            mDatHang_In = mDatHang_DSThuocCanDatHang = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mDatHang,
                                               (int)oPharmacyEx.mDatHang_In, (int)ePermission.mView);
        }

        #region checking account

        private bool _mDatHang_DSThuocCanDatHang = true;
        private bool _mDatHang_Tim = true;
        private bool _mDatHang_ThongTin = true;
        private bool _mDatHang_Edit = true;
        private bool _mDatHang_Them = true;

        private bool _mDatHang_Xoa = true;
        private bool _mDatHang_In = true;

        public bool mDatHang_Them
        {
            get
            {
                return _mDatHang_Them;
            }
            set
            {
                if (_mDatHang_Them == value)
                    return;
                _mDatHang_Them = value;
            }
        }
        public bool mDatHang_DSThuocCanDatHang
        {
            get
            {
                return _mDatHang_DSThuocCanDatHang;
            }
            set
            {
                if (_mDatHang_DSThuocCanDatHang == value)
                    return;
                _mDatHang_DSThuocCanDatHang = value;
            }
        }
        public bool mDatHang_Tim
        {
            get
            {
                return _mDatHang_Tim;
            }
            set
            {
                if (_mDatHang_Tim == value)
                    return;
                _mDatHang_Tim = value;
            }
        }
        public bool mDatHang_ThongTin
        {
            get
            {
                return _mDatHang_ThongTin;
            }
            set
            {
                if (_mDatHang_ThongTin == value)
                    return;
                _mDatHang_ThongTin = value;
            }
        }
        public bool mDatHang_Edit
        {
            get
            {
                return _mDatHang_Edit;
            }
            set
            {
                if (_mDatHang_Edit == value)
                    return;
                _mDatHang_Edit = value;
            }
        }

        public bool mDatHang_Xoa
        {
            get
            {
                return _mDatHang_Xoa;
            }
            set
            {
                if (_mDatHang_Xoa == value)
                    return;
                _mDatHang_Xoa = value;
            }
        }
        public bool mDatHang_In
        {
            get
            {
                return _mDatHang_In;
            }
            set
            {
                if (_mDatHang_In == value)
                    return;
                _mDatHang_In = value;
            }
        }
        //private bool _bEdit = true;
        //private bool _bAdd = true;
        //private bool _bDelete = true;
        //private bool _bView = true;
        //private bool _bPrint = true;
        //private bool _bReport = true;

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
        //        NotifyOfPropertyChange(() => bEdit);
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
        //        NotifyOfPropertyChange(() => bAdd);
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
        //        NotifyOfPropertyChange(() => bDelete);
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
        //        NotifyOfPropertyChange(() => bView);
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
        //        NotifyOfPropertyChange(() => bPrint);
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
        //        NotifyOfPropertyChange(() => bReport);
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
            if (PaymentModes != null && CurrentPharmacyPurchaseOrder != null)
            {
                CurrentPharmacyPurchaseOrder.V_PaymentMode = PaymentModes.FirstOrDefault().LookupID;
            }
        }

        private Staff GetStaffLogin()
        {
            return Globals.LoggedUserAccount.Staff;
        }

        private void GetPharmaceuticalCompanyCbx()
        {
            isLoadingPharmaceutialCompany = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetPharmaceuticalCompanyCbx(Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPharmaceuticalCompanyCbx(asyncResult);
                            PharmaceuticalCompanies = results.ToObservableCollection();
                            PharmaceuticalCompany item = new PharmaceuticalCompany();
                            item.PCOID = 0;
                            item.PCOName = NONEITEM;
                            PharmaceuticalCompanies.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingPharmaceutialCompany = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyPurchaseOrderDetail_ByParentID(long PharmacyPoID)
        {
            isLoadingOrderDetailID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrderDetail_ByParentID(PharmacyPoID, 0, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPharmacyPurchaseOrderDetail_ByParentID(asyncResult);
                            CurrentPharmacyPurchaseOrder.PurchaseOrderDetails = results.ToObservableCollection();
                            PharmacyPurchaseOrderDetailDeleted.Clear();
                            SumTotalPrice();
                            //if (CurrentPharmacyPurchaseOrder.CanSave)
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
                            isLoadingOrderDetailID = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyPurchaseOrder_Search(int PageIndex, int PageSize)
        {

            isLoadingOrder_Search = true;
            //  Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrder_Search(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndPharmacyPurchaseOrder_Search(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IPharmacyPurchaseOrderSearch> onInitDlg = delegate (IPharmacyPurchaseOrderSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.PharmacyPurchaseOrderList.Clear();
                                        proAlloc.PharmacyPurchaseOrderList.TotalItemCount = Total;
                                        proAlloc.PharmacyPurchaseOrderList.PageIndex = 0;
                                        foreach (PharmacyPurchaseOrder p in results)
                                        {
                                            proAlloc.PharmacyPurchaseOrderList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IPharmacyPurchaseOrderSearch>(onInitDlg);
                                }
                                else
                                {
                                    //lay thang gia tri
                                    CurrentPharmacyPurchaseOrder = results.FirstOrDefault();
                                    HideColumn();
                                    PharmacyPurchaseOrderDetail_ByParentID(CurrentPharmacyPurchaseOrder.PharmacyPoID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0863_G1_KgTimThay, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingOrder_Search = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void SetDefaultCurrency()
        {
            if (CurrentPharmacyPurchaseOrder != null && Currencies != null)
            {
                CurrentPharmacyPurchaseOrder.CurrencyID = Currencies.FirstOrDefault().CurrencyID;
            }
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
                            SetDefaultCurrency();
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

        private void RefeshMaPhieu()
        {
            CurrentPharmacyPurchaseOrder.PONumber = "";
            CurrentPharmacyPurchaseOrder.PharmacyPoID = 0;
        }

        private void PharmacyPurchaseOrderDetail_GetFirst(long? PharmacyEstimateID, long? SupplierID, long? PCOID)
        {
            RefeshMaPhieu();
            if (PharmacyEstimateID == null || PharmacyEstimateID <= 0)
            {
                if (CurrentPharmacyPurchaseOrder != null)
                {
                    if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails != null)
                    {
                        CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Clear();
                    }
                    //if (CurrentPharmacyPurchaseOrder.CanSave)
                    //{
                    //    AddNewBlankRow();
                    //}
                }
                return;
            }
            isLoadingOrderDetailID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrderDetail_GetFirst(PharmacyEstimateID, SupplierID, PCOID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPharmacyPurchaseOrderDetail_GetFirst(asyncResult);
                        
                            HideColumn();
                            if (CurrentPharmacyPurchaseOrder.PharmacyEstimationForPO != null && CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID > 0)
                            {
                                if (results == null || results.Count == 0)
                                {
                                    if (CurrentPharmacyPurchaseOrder.SelectedSupplier == null || CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID == 0)
                                    {
                                        Globals.ShowMessage(eHCMSResources.Z0806_G1_ChonNCCDatHg, eHCMSResources.G0442_G1_TBao);
                                    }
                                    else
                                    {
                                        Globals.ShowMessage(string.Format(eHCMSResources.Z0807_G1_NCCKgCoDuTru0, CurrentPharmacyPurchaseOrder.PharmacyEstimationForPO.EstimationCode), eHCMSResources.G0442_G1_TBao);
                                    }
                                }
                            }
                            CurrentPharmacyPurchaseOrder.PurchaseOrderDetails = results.ToObservableCollection();
                            PharmacyPurchaseOrderDetailDeleted.Clear();
                            SumTotalPrice();

                            //if (CurrentPharmacyPurchaseOrder.CanSave)
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
                            isLoadingOrderDetailID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyEstimationForPO_Search(int PageIndex, int PageSize)
        {
            if (CurrentPharmacyPurchaseOrder != null && CurrentPharmacyPurchaseOrder.SelectedSupplier != null)
            {
                SearchCriteria.SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;
            }
            else
            {
                SearchCriteria.SupplierID = 0;
            }
            isLoadingEstimate_Search = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPO_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndPharmacyEstimationForPO_Search(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    Action<IPurchaseOrderSearchEstimate> onInitDlg = delegate (IPurchaseOrderSearchEstimate proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.PharmacyEstimationForPOList.Clear();
                                        proAlloc.PharmacyEstimationForPOList.TotalItemCount = Total;
                                        proAlloc.PharmacyEstimationForPOList.PageIndex = 0;
                                        foreach (PharmacyEstimationForPO p in results)
                                        {
                                            proAlloc.PharmacyEstimationForPOList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IPurchaseOrderSearchEstimate>(onInitDlg);
                                }
                                else
                                {
                                    //lay thang gia tri
                                    CurrentPharmacyPurchaseOrder.PharmacyEstimationForPO = results.FirstOrDefault();
                                    PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, CurrentPharmacyPurchaseOrder.SupplierID, PCOID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z0808_G1_KgCoDuTruChoNCC, eHCMSResources.G0442_G1_TBao);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingEstimate_Search = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyPurchaseOrder_FullOperator()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrder_FullOperator(CurrentPharmacyPurchaseOrder, 0, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PharmacyPurchaseOrder OrderOut;
                            long results = contract.EndPharmacyPurchaseOrder_FullOperator(out OrderOut, asyncResult);
                            Globals.ShowMessage(eHCMSResources.A0468_G1_Msg_InfoLuuOK, eHCMSResources.G0442_G1_TBao);
                            if (results >= 0)
                            {
                                CurrentPharmacyPurchaseOrder = OrderOut;
                                PharmacyPurchaseOrderDetailDeleted.Clear();
                                //if (CurrentPharmacyPurchaseOrder.CanSave)
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

        private void ReLoadOrderWarning()
        {
            long? SupplierID = null;
            RefGenericDrugDetailWarningOrders.PageIndex = 0;
            if (CurrentPharmacyPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenericDrugDetailWarningOrders(RefGenericDrugDetailWarningOrders.PageIndex, RefGenericDrugDetailWarningOrders.PageSize, SupplierID);
        }

        public void KeyUp_Click(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                PharmacyPurchaseOrder_Search(0, Globals.PageSize);
            }
        }

        public void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            PharmacyPurchaseOrder_Search(0, Globals.PageSize);
        }

        public void btnFindEstimate(object sender, RoutedEventArgs e)
        {
            PharmacyEstimationForPO_Search(0, Globals.PageSize);
        }

        public void GridSuppliers_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public void rdtTrongNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentPharmacyPurchaseOrder.IsForeign = false;

        }

        public void rdtNgoaiNuoc_Checked(object sender, RoutedEventArgs e)
        {
            CurrentPharmacyPurchaseOrder.IsForeign = true;
        }

        private bool CheckValidate()
        {
            if (CurrentPharmacyPurchaseOrder == null)
            {
                return false;
            }
            return CurrentPharmacyPurchaseOrder.Validate();
        }

        public void btnSave(object sender, RoutedEventArgs e)
        {
            CurrentPharmacyPurchaseOrder.StaffID = GetStaffLogin().StaffID;
            if (CheckValidate())
            {
                if (CurrentPharmacyPurchaseOrder.PharmacyPoID == 0)
                {
                    CurrentPharmacyPurchaseOrder.V_PurchaseOrderStatus = (long)AllLookupValues.V_PurchaseOrderStatus.NEW;
                    StringBuilder sbForCheckVAT = new StringBuilder();
                    if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails != null && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Count > 0)
                    {
                        for (int i = 0; i < CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Count; i++)
                        {
                            if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail != null && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].PoUnitQty <= 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z0811_G1_SLgDatHgLonHon0, eHCMSResources.G0442_G1_TBao);
                                return;
                            }
                            if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].UnitPrice < 0)
                            {
                                Globals.ShowMessage(eHCMSResources.Z0812_G1_SLgDatHgKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                                return;
                            }
                            if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail != null 
                                && (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail.VAT < 0
                                || CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail.VAT > 1))
                            {
                                sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2991_G1_VATKhongHopLe));
                            }
                            if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail != null
                                && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].IsNotVat
                                && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail.VAT != null)
                            {
                                sbForCheckVAT.AppendLine(string.Format(eHCMSResources.Z1308_G1_LoiDongThu01, (i + 1).ToString(), eHCMSResources.Z2992_G1_CoVatKhongTinhThue));
                            }
                        }
                        if (!string.IsNullOrEmpty(sbForCheckVAT.ToString()))
                        {
                            MessageBox.Show(sbForCheckVAT.ToString(), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            return;
                        }
                        PharmacyPurchaseOrder_FullOperator();
                    }
                    else
                    {
                        Globals.ShowMessage(eHCMSResources.K0430_G1_NhapDLieu, eHCMSResources.G0442_G1_TBao);
                    }
                }
                else
                {
                    for (int i = 0; i < PharmacyPurchaseOrderDetailDeleted.Count; i++)
                    {
                        CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Add(PharmacyPurchaseOrderDetailDeleted[i]);
                    }
                    if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Count == 1)
                    {
                        if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[0].RefGenericDrugDetail == null)
                        {
                            Globals.ShowMessage(eHCMSResources.Z0810_G1_ChonHgCanDat, eHCMSResources.G0442_G1_TBao);
                            return;
                        }
                    }
                    for (int i = 0; i < CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Count; i++)
                    {
                        if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].RefGenericDrugDetail != null && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].PoUnitQty <= 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z0811_G1_SLgDatHgLonHon0, eHCMSResources.G0442_G1_TBao);
                            return;
                        }
                        if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].UnitPrice < 0)
                        {
                            Globals.ShowMessage(eHCMSResources.Z0812_G1_SLgDatHgKgNhoHon0, eHCMSResources.G0442_G1_TBao);
                            return;
                        }
                    }
                    PharmacyPurchaseOrder_FullOperator();
                }
            }
        }

        private bool ischanged(object item)
        {
            PharmacyPurchaseOrderDetail p = (PharmacyPurchaseOrderDetail)item;
            if (p != null)
            {
                if (p.EntityState == EntityState.PERSITED)
                {
                    p.EntityState = EntityState.MODIFIED;
                }

                if (p.RefGenericDrugDetail != null && p.RefGenericDrugDetail.DrugID != 0)
                {
                    var vars = CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Where(x => x.DrugID == p.RefGenericDrugDetail.DrugID && x.EntityState != EntityState.DETACHED);
                    if (vars.Count() > 1)
                    {
                        MessageBox.Show(eHCMSResources.K0044_G1_ThuocDaCoTrongPhDHg);
                        p.RefGenericDrugDetail = null;
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
            AxTextBox tbl = GridSuppliers.CurrentColumn.GetCellContent(GridSuppliers.SelectedItem) as AxTextBox;
            if (tbl != null)
            {
                value = tbl.Text.DeepCopy();
            }
        }

        DataGridColumn columnTMP ;
        DataGridRow rowTMP ;
        public void GridSuppliers_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //if (e.Column.DisplayIndex == (int)DataGridCol.UnitPrice)
            //{
            //    decimal ite = 0;
            //    Decimal.TryParse(value, out ite);

            //    PharmacyPurchaseOrderDetail item = e.Row.DataContext as PharmacyPurchaseOrderDetail;
            //    if (item != null && item.RefGenericDrugDetail != null && item.UnitPrice != ite)
            //    {
            //        item.TotalPriceNotVAT = (decimal)item.PoUnitQty * item.UnitPrice;
            //        item.PackagePrice = item.UnitPrice * item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
            //        if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
            //        {
            //            Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
            //        }
            //        if (item.EntityState == EntityState.PERSITED)
            //        {
            //            item.EntityState = EntityState.MODIFIED;
            //        }
            //        SumTotalPrice();
            //    }
            //}

            //if (e.Column.DisplayIndex == (int)DataGridCol.PackagePrice)
            //{
            //    decimal ite = 0;
            //    Decimal.TryParse(value, out ite);

            //    PharmacyPurchaseOrderDetail item = e.Row.DataContext as PharmacyPurchaseOrderDetail;
            //    if (item != null && item.RefGenericDrugDetail != null && item.PackagePrice != ite)
            //    {
            //        item.TotalPriceNotVAT = (decimal)item.PoPackageQty * item.PackagePrice;
            //        item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
            //        if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
            //        {
            //            Globals.ShowMessage(eHCMSResources.Z0574_G1_GiaMuaKhacGiaDotTruoc, eHCMSResources.G0442_G1_TBao);
            //        }
            //        if (item.EntityState == EntityState.PERSITED)
            //        {
            //            item.EntityState = EntityState.MODIFIED;
            //        }
            //        SumTotalPrice();
            //    }
            //}

            //if (e.Column.DisplayIndex == (int)DataGridCol.PackageQty)
            //{
            //    double ite = 0;
            //    Double.TryParse(value, out ite);

            //    PharmacyPurchaseOrderDetail item = e.Row.DataContext as PharmacyPurchaseOrderDetail;
            //    if (item != null && item.RefGenericDrugDetail != null && item.PoPackageQty != ite)
            //    {
            //        item.PoUnitQty = Convert.ToInt32(item.PoPackageQty * item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1));
            //        item.TotalPriceNotVAT = (decimal)item.PoPackageQty * item.PackagePrice;
            //        //if (item.PoPackageQty > 0)
            //        //{
            //        //    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PoPackageQty;
            //        //    item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
            //        //}
            //        if (item.EntityState == EntityState.PERSITED)
            //        {
            //            item.EntityState = EntityState.MODIFIED;
            //        }
            //        SumTotalPrice();
            //    }
            //}

            //if (e.Column.DisplayIndex == (int)DataGridCol.QTy)
            //{
            //    double ite = 0;
            //    Double.TryParse(value, out ite);

            //    PharmacyPurchaseOrderDetail item = e.Row.DataContext as PharmacyPurchaseOrderDetail;
            //    if (item != null && item.RefGenericDrugDetail != null && item.PoUnitQty != ite)
            //    {
            //        item.PoPackageQty = (double)item.PoUnitQty / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
            //        item.TotalPriceNotVAT = item.PoUnitQty * item.UnitPrice;
            //        //if (item.PoPackageQty > 0)
            //        //{
            //        //    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PoPackageQty;
            //        //   // item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
            //        //}
            //        if (item.EntityState == EntityState.PERSITED)
            //        {
            //            item.EntityState = EntityState.MODIFIED;
            //        }
            //        SumTotalPrice();
            //    }
            //}
            columnTMP = e.Column;
            rowTMP = e.Row;
        }

        public void GridSuppliers_CurrentCellChanged(object sender, EventArgs e)
        {
            if (columnTMP != null)
            {

                if (columnTMP.DisplayIndex == (int)DataGridCol.UnitPrice)
                {
                    decimal ite = 0;
                    Decimal.TryParse(value, out ite);

                    PharmacyPurchaseOrderDetail item = rowTMP.DataContext as PharmacyPurchaseOrderDetail;
                    if (item != null && item.RefGenericDrugDetail != null && item.UnitPrice != ite)
                    {
                        item.TotalPriceNotVAT = (decimal)item.PoUnitQty * item.UnitPrice;
                        item.PackagePrice = item.UnitPrice * item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                        if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
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

                if (columnTMP.DisplayIndex == (int)DataGridCol.PackagePrice)
                {
                    decimal ite = 0;
                    Decimal.TryParse(value, out ite);

                    PharmacyPurchaseOrderDetail item = rowTMP.DataContext as PharmacyPurchaseOrderDetail;
                    if (item != null && item.RefGenericDrugDetail != null && item.PackagePrice != ite)
                    {
                        item.TotalPriceNotVAT = (decimal)item.PoPackageQty * item.PackagePrice;
                        item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                        if (item.UnitPrice != item.RefGenericDrugDetail.InBuyingPrice && item.RefGenericDrugDetail.InBuyingPrice > 0)
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

                if (columnTMP.DisplayIndex == (int)DataGridCol.PackageQty)
                {
                    double ite = 0;
                    Double.TryParse(value, out ite);

                    PharmacyPurchaseOrderDetail item = rowTMP.DataContext as PharmacyPurchaseOrderDetail;
                    if (item != null && item.RefGenericDrugDetail != null && item.PoPackageQty != ite)
                    {
                        item.PoUnitQty = Convert.ToInt32(item.PoPackageQty * item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1));
                        item.TotalPriceNotVAT = (decimal)item.PoPackageQty * item.PackagePrice;
                        //if (item.PoPackageQty > 0)
                        //{
                        //    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PoPackageQty;
                        //    item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                        //}
                        if (item.EntityState == EntityState.PERSITED)
                        {
                            item.EntityState = EntityState.MODIFIED;
                        }
                        SumTotalPrice();
                    }
                }

                if (columnTMP.DisplayIndex == (int)DataGridCol.QTy)
                {
                    double ite = 0;
                    Double.TryParse(value, out ite);

                    PharmacyPurchaseOrderDetail item = rowTMP.DataContext as PharmacyPurchaseOrderDetail;
                    if (item != null && item.RefGenericDrugDetail != null && item.PoUnitQty != ite)
                    {
                        item.PoPackageQty = (double)item.PoUnitQty / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                        item.TotalPriceNotVAT = item.PoUnitQty * item.UnitPrice;
                        //if (item.PoPackageQty > 0)
                        //{
                        //    item.PackagePrice = item.TotalPriceNotVAT / (decimal)item.PoPackageQty;
                        //   // item.UnitPrice = item.PackagePrice / item.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                        //}
                        if (item.EntityState == EntityState.PERSITED)
                        {
                            item.EntityState = EntityState.MODIFIED;
                        }
                        SumTotalPrice();
                    }
                }
                if (columnTMP.Equals(GridSuppliers.GetColumnByName("colVAT")))
                {
                    PharmacyPurchaseOrderDetail item = rowTMP.DataContext as PharmacyPurchaseOrderDetail;
                    if (item != null && item.RefGenericDrugDetail != null)
                    {
                        if (item.EntityState == EntityState.PERSITED)
                        {
                            item.EntityState = EntityState.MODIFIED;
                        }
                    }
                }
            }
        }
        DataGrid GridSuppliers = null;
        public void GridSuppliers_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GridSuppliers = sender as DataGrid;
        }


        public void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CurrentPharmacyPurchaseOrder.VAT < 1 || CurrentPharmacyPurchaseOrder.VAT > 2)
            {
                Globals.ShowMessage(eHCMSResources.K0263_G1_VATKhongHopLe2, eHCMSResources.G0442_G1_TBao);
                CurrentPharmacyPurchaseOrder.VAT = 1;
            }
            else
            {
                TotalVAT = TotalPriceNotVAT * (decimal)(CurrentPharmacyPurchaseOrder.VAT - 1);
                ReadMoneyString();
            }
        }

        #region Auto for Supplier

        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
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

        private ObservableCollection<Supplier> _SupplierAlls;
        public ObservableCollection<Supplier> SupplierAlls
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
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSupplier_ByPCOIDNotPaging(null, V_SupplierType, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSupplier_ByPCOIDNotPaging(asyncResult);
                            SupplierAlls = results.ToObservableCollection();
                            Supplier item = new Supplier();
                            item.SupplierID = 0;
                            item.SupplierName = NONEITEM;
                            SupplierAlls.Insert(0, item);
                            Suppliers = SupplierAlls.DeepCopy();

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

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            Action<ISuppliers> onInitDlg = delegate (ISuppliers proAlloc)
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg);
        }

        //public void AutoDrug_Text_DropDownClosed(object sender, System.Windows.RoutedPropertyChangedEventArgs<bool> e)
        //{
        //    if (CurrentPharmacyPurchaseOrder != null && CurrentPharmacyPurchaseOrder.SelectedSupplier != null)
        //    {
        //        PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID, PCOID);
        //    }
        //    else if (CurrentPharmacyPurchaseOrder != null && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails != null)
        //    {
        //        CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Clear();
        //    }
        //}


        private void SumTotalPrice()
        {
            TotalPriceNotVAT = 0;
            TotalPriceHaveVAT = 0;
            //TotalVAT = 0;
            if (CurrentPharmacyPurchaseOrder != null && CurrentPharmacyPurchaseOrder.PurchaseOrderDetails != null)
            {
                for (int i = 0; i < CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Count; i++)
                {
                    TotalPriceNotVAT += (decimal)CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].PoUnitQty * CurrentPharmacyPurchaseOrder.PurchaseOrderDetails[i].UnitPrice;
                }
                //if (CurrentPharmacyPurchaseOrder.VAT > 1)
                //{
                //    TotalVAT = TotalPriceNotVAT * (decimal)(CurrentPharmacyPurchaseOrder.VAT - 1);
                //}
                //else
                //{
                //    TotalVAT = 0;
                //}
                CalcTotalVat(CurrentPharmacyPurchaseOrder.PurchaseOrderDetails);
                ReadMoneyString();
            }
        }

        private void ReadMoneyString()
        {
            TotalPriceHaveVAT = Math.Round(TotalPriceNotVAT + TotalVAT, 2);
            eHCMS.Services.Core.NumberToLetterConverter converter = new eHCMS.Services.Core.NumberToLetterConverter();
            decimal temp = 0;
            string prefix = "";
            if (TotalPriceHaveVAT < 0)
            {
                temp = 0 - TotalPriceHaveVAT;
                prefix = string.Format(" {0} ", eHCMSResources.Z0873_G1_Am);
            }
            else
            {
                temp = TotalPriceHaveVAT;
                prefix = "";
            }
            ReadMoney = prefix + converter.Convert(temp.ToString(), '.', eHCMSResources.Z0871_G1_Le) + string.Format(" {0}", eHCMSResources.Z0872_G1_Dong);
        }
        #endregion

        #region AutoGenMedProduct Member
        private string BrandName;
        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetail;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetail
        {
            get
            {
                return _RefGenericDrugDetail;
            }
            set
            {
                if (_RefGenericDrugDetail != value)
                {
                    _RefGenericDrugDetail = value;
                    NotifyOfPropertyChange(() => RefGenericDrugDetail);
                }
            }
        }

        private void GetRefGenericDrugDetail_Auto(string BrandName, int PageIndex, int PageSize, bool? IsCode)
        {
            if (IsCode.GetValueOrDefault())
            {
                isLoadingFullOperator = true;
            }
            long? EstimationID = 0;
            long? SupplierID = 0;
            if (CurrentPharmacyPurchaseOrder != null)
            {
                EstimationID = CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID;
                SupplierID = CurrentPharmacyPurchaseOrder.SupplierID;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenericDrugDetail_AutoRequest(BrandName, EstimationID, SupplierID, PageIndex, PageSize, IsCode, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total;
                            var results = contract.EndRefGenericDrugDetail_AutoRequest(out Total, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (IsCode.GetValueOrDefault())
                                {
                                    SetValueCurrentOrder(results.FirstOrDefault());
                                }
                                else
                                {
                                    RefGenericDrugDetail.Clear();
                                    RefGenericDrugDetail.TotalItemCount = Total;
                                    foreach (RefGenericDrugDetail p in results)
                                    {
                                        RefGenericDrugDetail.Add(p);
                                    }
                                    AutoGenMedProduct.ItemsSource = RefGenericDrugDetail;
                                    AutoGenMedProduct.PopulateComplete();
                                }
                            }
                            else
                            {
                                if (IsCode.GetValueOrDefault())
                                {
                                    MessageBox.Show("Không tìm thấy.(NCC này không CC thuốc bạn vừa nhập vào)!");
                                    txt = "";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            if (IsCode.GetValueOrDefault())
                            {
                                isLoadingFullOperator = false;
                                if (AxQty != null)
                                {
                                    AxQty.Focus();
                                }
                            }
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
                if (CurrentPharmacyPurchaseOrder != null && CurrentPharmacyPurchaseOrder.SelectedSupplier != null && CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID > 0)
                {
                    RefGenericDrugDetail.PageIndex = 0;
                    BrandName = e.Parameter;
                    GetRefGenericDrugDetail_Auto(e.Parameter, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize, false);
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.K0347_G1_ChonNCC, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void AutoDrug_Text_DropDownClosed(object sender, System.Windows.RoutedPropertyChangedEventArgs<bool> e)
        {
            if (AutoGenMedProduct != null)
            {
                if (CurrentPharmacyPurchaseOrderDetail != null && AutoGenMedProduct.SelectedItem != null)
                {
                    SetValueCurrentOrder(AutoGenMedProduct.SelectedItem);
                }
            }
        }

        private void SetValueCurrentOrder(object Item)
        {
            if (CurrentPharmacyPurchaseOrderDetail == null)
            {
                CurrentPharmacyPurchaseOrderDetail = new PharmacyPurchaseOrderDetail();
            }
            CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail = Item as RefGenericDrugDetail;
            CurrentPharmacyPurchaseOrderDetail.EstimateQty = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.Qty.GetValueOrDefault();
            CurrentPharmacyPurchaseOrderDetail.OrderedQty = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.Ordered.GetValueOrDefault();
            if (CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.Qty.GetValueOrDefault() - CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.Ordered.GetValueOrDefault() > 0)
            {
                CurrentPharmacyPurchaseOrderDetail.PoUnitQty = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.Qty.GetValueOrDefault() - CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.Ordered.GetValueOrDefault();
            }
            else
            {
                CurrentPharmacyPurchaseOrderDetail.PoUnitQty = 0;
            }
            CurrentPharmacyPurchaseOrderDetail.UnitPrice = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.UnitPrice;
            CurrentPharmacyPurchaseOrderDetail.PackagePrice = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.PackagePrice;
            CurrentPharmacyPurchaseOrderDetail.IsNotVat = CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.IsNotVat;
        }

        #endregion

        #region IHandle<PharmacyCloseSearchSupplierEvent> Members

        public void Handle(PharmacyCloseSearchSupplierEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentPharmacyPurchaseOrder.SelectedSupplier = message.SelectedSupplier as Supplier;
                PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID, PCOID);
            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchPurchaseOrderEstimationEvent> Members

        public void Handle(PharmacyCloseSearchPurchaseOrderEstimationEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentPharmacyPurchaseOrder.PharmacyEstimationForPO = message.SelectedEstimation as PharmacyEstimationForPO;
                long SupplierID = 0;
                try
                {
                    SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;

                }
                catch { SupplierID = 0; }
                //if (SupplierID > 0)
                //{
                PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, SupplierID, PCOID);
                //}

            }
        }

        #endregion

        #region IHandle<PharmacyCloseSearchPurchaseOrderEvent> Members

        public void Handle(PharmacyCloseSearchPurchaseOrderEvent message)
        {
            if (message != null && this.IsActive)
            {
                PharmacyPurchaseOrder item = message.SelectedPurchaseOrder as PharmacyPurchaseOrder;
                SelectedSupplierCopy = item.SelectedSupplier;
                CurrentPharmacyPurchaseOrder = item;
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
                PharmacyPurchaseOrderDetail_ByParentID(CurrentPharmacyPurchaseOrder.PharmacyPoID);
            }
        }

        #endregion

        private void PharmacyPurchaseOrder_Delete(long PharmacyPoID)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrders_Delete(PharmacyPoID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndPharmacyPurchaseOrders_Delete(asyncResult);
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
                            isLoadingFullOperator = false;
                            // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        #region link button member

        public void btnNewPurchase()
        {
            if (MessageBox.Show(eHCMSResources.A0140_G1_Msg_ConfTaoMoiDonHg, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                RefeshData();
            }
        }

        public void btnDeletePurchase()
        {
            if (MessageBox.Show(eHCMSResources.A0120_G1_Msg_ConfXoaPh, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                if (CurrentPharmacyPurchaseOrder != null && CurrentPharmacyPurchaseOrder.PharmacyPoID > 0)
                {
                    PharmacyPurchaseOrder_Delete(CurrentPharmacyPurchaseOrder.PharmacyPoID);
                }
            }
        }

        public void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyPurchaseOrder.CanSave && GridSuppliers != null && GridSuppliers.SelectedItem != null)
            {
                if (MessageBox.Show(eHCMSResources.A0121_G1_Msg_ConfXoaThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {

                    PharmacyPurchaseOrderDetail item = GridSuppliers.SelectedItem as PharmacyPurchaseOrderDetail;
                    if (item.EntityState != EntityState.NEW)
                    {
                        item.EntityState = EntityState.DETACHED;
                        PharmacyPurchaseOrderDetailDeleted.Add(item);
                    }
                    CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Remove(item);
                    CurrentPharmacyPurchaseOrder.PurchaseOrderDetails = CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.ToObservableCollection();
                    SumTotalPrice();
                    //if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails == null || CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Count == 0)
                    //{
                    //    AddNewBlankRow();
                    //}
                }
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
            if (chk != null && chk.IsChecked == true && CurrentPharmacyPurchaseOrderDetail != null && CurrentPharmacyPurchaseOrderDetail.PharmacyPoDetailID > 0)
            {
                if (CurrentPharmacyPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY || CurrentPharmacyPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED)
                {
                    if (MessageBox.Show(eHCMSResources.Z1632_G1_CoChacKgChoThuocNay, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        PharmacyPurchaseOrderDetail_UpdateNoWaiting(CurrentPharmacyPurchaseOrderDetail.PharmacyPoDetailID, true);
                    }
                    else
                    {
                        CurrentPharmacyPurchaseOrderDetail.NoWaiting = false;
                    }
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z1633_G1_ThuocChuaDcGuiDi, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void HyperlinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.Z0823_G1_CoChacGuiDonHgDenNCC + Environment.NewLine + eHCMSResources.Z0824_G1_KgDuocSuaDonHgDaGui, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PharmacyPurchaseOrder_UpdateStatus(CurrentPharmacyPurchaseOrder.PharmacyPoID, (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED);
            }
        }

        public void hblNoWaiting()
        {
            if (MessageBox.Show(eHCMSResources.A0115_G1_Msg_ConfKhongChoNCCGiaoDu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                PharmacyPurchaseOrder_UpdateStatus(CurrentPharmacyPurchaseOrder.PharmacyPoID, (long)AllLookupValues.V_PurchaseOrderStatus.NO_WAITING);
            }
        }

        private void PharmacyPurchaseOrder_UpdateStatus(long ID, long V_PurchaseOrderStatus)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrder_UpdateStatus(ID, V_PurchaseOrderStatus, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndPharmacyPurchaseOrder_UpdateStatus(asyncResult);
                            Globals.ShowMessage(eHCMSResources.A0282_G1_Msg_InfoCNhatStatusOK, eHCMSResources.G0442_G1_TBao);
                            PharmacyPurchaseOrder_ID(ID);
                            PharmacyPurchaseOrderDetail_ByParentID(ID);
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
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyPurchaseOrderDetail_UpdateNoWaiting(long ID, bool? NoWaiting)
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrderDetail_UpdateNoWaiting(ID, NoWaiting, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            contract.EndPharmacyPurchaseOrderDetail_UpdateNoWaiting(asyncResult);
                            ReLoadOrderWarning();
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

        private void PharmacyPurchaseOrder_ID(long ID)
        {
            isLoadingOrderID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyPurchaseOrder_ByID(ID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            CurrentPharmacyPurchaseOrder = contract.EndPharmacyPurchaseOrder_ByID(asyncResult);
                            if (CurrentPharmacyPurchaseOrder != null)
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
                            isLoadingOrderID = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void HideColumn()
        {
            if (GridSuppliers != null)
            {
                if (CurrentPharmacyPurchaseOrder.PharmacyPoID > 0)
                {
                    GridSuppliers.Columns[(int)DataGridCol.DaDat].Visibility = Visibility.Collapsed;
                }
                else
                {
                    GridSuppliers.Columns[(int)DataGridCol.DaDat].Visibility = Visibility.Visible;
                }
                if (CurrentPharmacyPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.ORDERED || CurrentPharmacyPurchaseOrder.V_PurchaseOrderStatus == (long)AllLookupValues.V_PurchaseOrderStatus.PART_DELIVERY)
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
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = CurrentPharmacyPurchaseOrder.PharmacyPoID;
            DialogView.eItem = ReportName.PHARMACY_PHIEUDATHANG;
            GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        #endregion

        #region Warning Order Member

        private PagedSortableCollectionView<RefGenericDrugDetail> _RefGenericDrugDetailWarningOrders;
        public PagedSortableCollectionView<RefGenericDrugDetail> RefGenericDrugDetailWarningOrders
        {
            get
            {
                return _RefGenericDrugDetailWarningOrders;
            }
            set
            {
                if (_RefGenericDrugDetailWarningOrders != value)
                {
                    _RefGenericDrugDetailWarningOrders = value;
                    NotifyOfPropertyChange(() => RefGenericDrugDetailWarningOrders);
                }
            }
        }

        private void Get_RefGenericDrugDetailWarningOrders(int PageIndex, int PageSize, long? SupplierID)
        {
            isLoadingWarning = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginRefGenericDrugDetail_WarningOrder(PageIndex, PageSize, SupplierID, IsAll, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndRefGenericDrugDetail_WarningOrder(out Total, asyncResult);
                            if (results != null)
                            {
                                RefGenericDrugDetailWarningOrders.Clear();
                                RefGenericDrugDetailWarningOrders.TotalItemCount = Total;
                                foreach (RefGenericDrugDetail p in results)
                                {
                                    RefGenericDrugDetailWarningOrders.Add(p);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingWarning = false;
                            //Globals.IsBusy = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void GridRefGenericDrugDetailWarningOrders_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            RefGenericDrugDetail item = e.Row.DataContext as RefGenericDrugDetail;
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
            RefGenericDrugDetailWarningOrders.PageIndex = 0;
            if (CurrentPharmacyPurchaseOrder.SelectedSupplier != null)
            {
                SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;
            }
            Get_RefGenericDrugDetailWarningOrders(RefGenericDrugDetailWarningOrders.PageIndex, RefGenericDrugDetailWarningOrders.PageSize, SupplierID);
        }

        #endregion

        private Supplier SelectedSupplierCopy;
        public void cbxNSX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupplierAlls != null)
            {
                if (CurrentPharmacyPurchaseOrder != null)
                {
                    SelectedSupplierCopy = CurrentPharmacyPurchaseOrder.SelectedSupplier.DeepCopy();
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
                        CurrentPharmacyPurchaseOrder.SelectedSupplier = null;
                        SelectedSupplierCopy = null;
                    }
                }

                long SupplierID = 0;
                try
                {
                    SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;
                }
                catch { SupplierID = 0; }
                //PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, SupplierID, PCOID);
            }
        }

        //public void cbxNCC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (CurrentPharmacyPurchaseOrder != null)
        //    {
        //        if (SelectedSupplierCopy == null || !SelectedSupplierCopy.Equals(CurrentPharmacyPurchaseOrder.SelectedSupplier))
        //        {
        //            long SupplierID = 0;
        //            try
        //            {
        //                SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;

        //            }
        //            catch { SupplierID = 0; }
        //            PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, SupplierID, PCOID);
        //            SelectedSupplierCopy = CurrentPharmacyPurchaseOrder.SelectedSupplier.DeepCopy();
        //        }
        //    }
        //}


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

            if (CurrentPharmacyPurchaseOrder != null)
            {
                if (SelectedSupplierCopy == null || !SelectedSupplierCopy.Equals(CurrentPharmacyPurchaseOrder.SelectedSupplier))
                {
                    long SupplierID = 0;
                    try
                    {
                        SupplierID = CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID;

                    }
                    catch { SupplierID = 0; }
                    PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, SupplierID, PCOID);
                    SelectedSupplierCopy = CurrentPharmacyPurchaseOrder.SelectedSupplier.DeepCopy();
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
            Action<IPharmacieucalCompany> onInitDlg = delegate (IPharmacieucalCompany proAlloc)
            {
                proAlloc.IsChildWindow = true;
            };
            GlobalsNAV.ShowDialog<IPharmacieucalCompany>(onInitDlg);
        }

        #region IHandle<PharmacyCloseSearchPharmaceuticalCompanyEvent> Members

        public void Handle(PharmacyCloseSearchPharmaceuticalCompanyEvent message)
        {
            if (this.IsActive && message.SelectedPharmaceuticalCompany != null)
            {
                PCOID = (message.SelectedPharmaceuticalCompany as PharmaceuticalCompany).PCOID;
            }
        }

        #endregion

        #region search by code member
        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CurrentPharmacyPurchaseOrder == null || CurrentPharmacyPurchaseOrder.SelectedSupplier == null || CurrentPharmacyPurchaseOrder.SelectedSupplier.SupplierID <= 0)
            {
                Globals.ShowMessage(eHCMSResources.K0347_G1_ChonNCC, eHCMSResources.G0442_G1_TBao);
                return;
            }
            txt = (sender as TextBox).Text;
            if (!string.IsNullOrEmpty(txt))
            {
                RefGenericDrugDetail.PageIndex = 0;
                GetRefGenericDrugDetail_Auto(txt, RefGenericDrugDetail.PageIndex, RefGenericDrugDetail.PageSize, true);
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
                if (CurrentPharmacyPurchaseOrder != null)
                {
                    return _VisibilityName && CurrentPharmacyPurchaseOrder.CanSave;
                }
                return _VisibilityName;
            }
            set
            {
                if (CurrentPharmacyPurchaseOrder != null)
                {
                    _VisibilityName = value && CurrentPharmacyPurchaseOrder.CanSave;
                    _VisibilityCode = !_VisibilityName && CurrentPharmacyPurchaseOrder.CanSave;
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
                    if (CurrentPharmacyPurchaseOrderDetail != null)
                    {
                        if (CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail != null && CurrentPharmacyPurchaseOrderDetail.PoPackageQty != ite)
                        {
                            CurrentPharmacyPurchaseOrderDetail.PoPackageQty = CurrentPharmacyPurchaseOrderDetail.PoUnitQty / CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                            CurrentPharmacyPurchaseOrderDetail.TotalPriceNotVAT = CurrentPharmacyPurchaseOrderDetail.PoUnitQty * CurrentPharmacyPurchaseOrderDetail.UnitPrice;
                        }
                    }

                }
            }
        }
        private bool CheckedValidation(PharmacyPurchaseOrderDetail item)
        {
            if (item == null)
            {
                return false;
            }
            return item.Validate();
        }

        public void AddItem()
        {
            if (CheckedValidation(CurrentPharmacyPurchaseOrderDetail))
            {
                if (CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail == null)
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.K0410_G1_ChonThuoc), eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentPharmacyPurchaseOrderDetail.PoUnitQty <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0811_G1_SLgDatHgLonHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentPharmacyPurchaseOrder == null)
                {
                    CurrentPharmacyPurchaseOrder = new PharmacyPurchaseOrder();
                }
                if (CurrentPharmacyPurchaseOrder.PurchaseOrderDetails == null)
                {
                    CurrentPharmacyPurchaseOrder.PurchaseOrderDetails = new ObservableCollection<PharmacyPurchaseOrderDetail>();
                }
                var chk = CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Where(x => x.RefGenericDrugDetail != null && x.RefGenericDrugDetail.DrugID == CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.DrugID);
                if (chk != null && chk.Count() > 0)
                {
                    Globals.ShowMessage(eHCMSResources.K0053_G1_ThuocDaTonTai, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1) > 0)
                {
                    CurrentPharmacyPurchaseOrderDetail.PoPackageQty = (double)CurrentPharmacyPurchaseOrderDetail.PoUnitQty / CurrentPharmacyPurchaseOrderDetail.RefGenericDrugDetail.UnitPackaging.GetValueOrDefault(1);
                }
                else
                {
                    CurrentPharmacyPurchaseOrderDetail.PoPackageQty = CurrentPharmacyPurchaseOrderDetail.PoUnitQty;
                }
                var item = CurrentPharmacyPurchaseOrderDetail.DeepCopy();
                CurrentPharmacyPurchaseOrderDetail.EntityState = EntityState.NEW;
                CurrentPharmacyPurchaseOrder.PurchaseOrderDetails.Insert(0, item);
                SumTotalPrice();
                CurrentPharmacyPurchaseOrderDetail = new PharmacyPurchaseOrderDetail();
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
            else
            {
                Globals.ShowMessage(eHCMSResources.A0540_G1_Msg_InfoDataKhHopLe, eHCMSResources.G0442_G1_TBao);
            }
        }

        #endregion

        public void KiemTraHangDat()
        {
            GlobalsNAV.ShowDialog<IPharmacyCheckOrder>();
        }


        //public void btnTest(object sender, RoutedEventArgs e)
        //{
        //    if (CurrentPharmacyPurchaseOrder == null)
        //    {
        //        return;
        //    }
        //    if (CurrentPharmacyPurchaseOrder.SupplierID <= 0)
        //    {
        //        MessageBox.Show("Hãy chọn nhà cung cấp", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
        //        return;
        //    }
        //    PharmacyPurchaseOrderDetail_GetFirst(CurrentPharmacyPurchaseOrder.PharmacyEstimatePoID, CurrentPharmacyPurchaseOrder.SupplierID, PCOID);
        //}

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
                    (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).EntityState = EntityState.MODIFIED;
                }
                else
                {
                    (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).IsNotVat = false;
                    if ((chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).RefGenericDrugDetail != null)
                    {
                        (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).RefGenericDrugDetail.VAT = 0;
                    }
                    (chkIsNotVat.DataContext as PharmacyPurchaseOrderDetail).EntityState = EntityState.MODIFIED;
                }
            }
        }
        public void CalcTotalVat(ObservableCollection<PharmacyPurchaseOrderDetail> ListPurchaseOrderDetails)
        {
            TotalVAT = 0;
            if (ListPurchaseOrderDetails != null && ListPurchaseOrderDetails.Count > 0)
            {
                foreach (var detail in ListPurchaseOrderDetails)
                {
                    if (detail.RefGenericDrugDetail != null)
                    {
                        if (detail.RefGenericDrugDetail.VAT >= 0)
                        {
                            TotalVAT += detail.PoUnitQty * detail.UnitPrice * (decimal)detail.RefGenericDrugDetail.VAT;
                        }
                    }
                }
                TotalPriceHaveVAT = TotalPriceNotVAT + TotalVAT;
            }
        }
    }

}
