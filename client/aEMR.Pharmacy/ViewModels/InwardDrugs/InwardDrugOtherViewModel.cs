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
using System.Globalization;
using aEMR.CommonTasks;
using aEMR.Controls;
using eHCMSLanguage;
using Castle.Windsor;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IInwardDrugOther)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardDrugOtherViewModel : Conductor<object>, IInwardDrugOther
        , IHandle<PharmacyCloseSearchInwardIncoiceEvent>
        , IHandle<PharmacyCloseEditInwardEvent>
        , IHandle<PharmacyCloseSearchDrugEvent>
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

        private bool _isLoadingStore1 = false;
        public bool isLoadingStore1
        {
            get { return _isLoadingStore1; }
            set
            {
                if (_isLoadingStore1 != value)
                {
                    _isLoadingStore1 = value;
                    NotifyOfPropertyChange(() => isLoadingStore1);
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
            get { return (isLoadingCurrency || isLoadingGoodsType || isLoadingGetStore || isLoadingFullOperator || isLoadingInvoiceDetailD || isLoadingStore1 || isLoadingOrderDetailID || isLoadingSearch || isLoadingCost); }
        }

        #endregion

        public long V_SupplierType = 7200;
        private long TypID = (long)AllLookupValues.RefOutputType.NHAP_TU_NGUON_KHAC;

        public enum DataGridCol
        {
            TenThuoc = 2,
            ViTri = 14,
            CKPercent = 14,
            CK = 15
        }
        [ImportingConstructor]
        public InwardDrugOtherViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {
            eventArg.Subscribe(this);
            InitializeInvoiceDrug();

            InnitSearchCriteria();

            InitCurrentInwardDrug();

            GetAllCurrency(true);
            Coroutine.BeginExecute(DoGetLookup_GoodType());

            Coroutine.BeginExecute(DoGetStore_All());
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            IsHideAuSupplier = true;
            IsEnabled = true;
            IsVisibility = Visibility.Collapsed;

            authorization();
            RefGenericDrugDetails = new PagedSortableCollectionView<RefGenericDrugDetail>();
            RefGenericDrugDetails.OnRefresh += RefGenericDrugDetails_OnRefresh;
            RefGenericDrugDetails.PageSize = Globals.PageSize;

            InwardDrugList = new PagedSortableCollectionView<InwardDrug>();
            InwardDrugList.OnRefresh += new EventHandler<RefreshEventArgs>(InwardDrugList_OnRefresh);
            InwardDrugList.PageSize = Globals.PageSize;

            FillListVAT();
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

        private ObservableCollection<RefStorageWarehouseLocation> _StoreCbxAll;
        public ObservableCollection<RefStorageWarehouseLocation> StoreCbxAll
        {
            get
            {
                return _StoreCbxAll;
            }
            set
            {
                if (_StoreCbxAll != value)
                {
                    _StoreCbxAll = value;
                    NotifyOfPropertyChange(() => StoreCbxAll);
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

        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mNhapHangNK_CapNhat = mNhapHangNK_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mNhapHangTuNguonKhac,
                                                   (int)oPharmacyEx.mNhapHangNK_CapNhat, (int)ePermission.mView);
            mNhapHangNK_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mNhapHangTuNguonKhac,
                                               (int)oPharmacyEx.mNhapHangNK_Tim, (int)ePermission.mView);
            mNhapHangNK_ThongTinDonHang = mNhapHangNK_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mNhapHangTuNguonKhac,
                                                   (int)oPharmacyEx.mNhapHangNK_ThongTinDonHang, (int)ePermission.mView)
                                                   || mNhapHangNK_CapNhat;
            mNhapHangNK_PhieuMoi = mNhapHangNK_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mNhapHangTuNguonKhac,
                                                   (int)oPharmacyEx.mNhapHangNK_PhieuMoi, (int)ePermission.mView);

            mNhapHangNK_ReportIn = mNhapHangNK_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                                   , (int)ePharmacy.mNhapHangTuNguonKhac,
                                               (int)oPharmacyEx.mNhapHangNK_ReportIn, (int)ePermission.mView);
        }
        #region checking account

        private bool _mNhapHangNK_Tim = true;
        private bool _mNhapHangNK_ThongTinDonHang = true;
        private bool _mNhapHangNK_PhieuMoi = true;
        private bool _mNhapHangNK_CapNhat = true;
        private bool _mNhapHangNK_ReportIn = true;

        public bool mNhapHangNK_Tim
        {
            get
            {
                return _mNhapHangNK_Tim;
            }
            set
            {
                if (_mNhapHangNK_Tim == value)
                    return;
                _mNhapHangNK_Tim = value;
            }
        }
        public bool mNhapHangNK_ThongTinDonHang
        {
            get
            {
                return _mNhapHangNK_ThongTinDonHang;
            }
            set
            {
                if (_mNhapHangNK_ThongTinDonHang == value)
                    return;
                _mNhapHangNK_ThongTinDonHang = value;
            }
        }
        public bool mNhapHangNK_PhieuMoi
        {
            get
            {
                return _mNhapHangNK_PhieuMoi;
            }
            set
            {
                if (_mNhapHangNK_PhieuMoi == value)
                    return;
                _mNhapHangNK_PhieuMoi = value;
            }
        }
        public bool mNhapHangNK_CapNhat
        {
            get
            {
                return _mNhapHangNK_CapNhat;
            }
            set
            {
                if (_mNhapHangNK_CapNhat == value)
                    return;
                _mNhapHangNK_CapNhat = value;
            }
        }
        public bool mNhapHangNK_ReportIn
        {
            get
            {
                return _mNhapHangNK_ReportIn;
            }
            set
            {
                if (_mNhapHangNK_ReportIn == value)
                    return;
                _mNhapHangNK_ReportIn = value;
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
            isLoadingCurrency = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
                            CurrentInwardDrugInvoice.SelectedCurrency = Currencies.FirstOrDefault();
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

        private IEnumerator<IResult> DoGetLookup_GoodType()
        {
            isLoadingGoodsType = true;
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_GoodsType, false, false);
            yield return paymentTypeTask;
            CbxGoodsTypes = paymentTypeTask.LookupList.Where(x => x.LookupID != (long)AllLookupValues.V_GoodsType.HANG_NHAP_TU_LUAN_CHUYEN_KHO).ToObservableCollection();
            isLoadingGoodsType = false;
            yield break;
        }

        private void SetDefaultStore()
        {
            if (CurrentInwardDrugInvoice != null)
            {
                if (StoreCbx != null)
                {
                    CurrentInwardDrugInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
                }
                if (StoreCbxAll != null)
                {
                    CurrentInwardDrugInvoice.SelectedStorageOut = StoreCbxAll.FirstOrDefault();
                }
            }
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            SetDefaultStore();
            isLoadingGetStore = false;
            yield break;
        }

        private IEnumerator<IResult> DoGetStore_All()
        {
            isLoadingStore1 = true;
            var paymentTypeTask = new LoadStoreListTask(null, false, null, true, false);
            yield return paymentTypeTask;
            StoreCbxAll = paymentTypeTask.LookupList;
            SetDefaultStore();
            isLoadingStore1 = false;
            yield break;
        }

        public void btnNew(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(eHCMSResources.A0147_G1_Msg_ConfTaoMoiPhNhap, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                InitializeInvoiceDrug();
            }
        }

        private void InitializeInvoiceDrug()
        {
            CurrentInwardDrugInvoice = new InwardDrugInvoice();
            CurrentInwardDrugInvoice.InvDateInvoice = Globals.ServerDate.Value;
            CurrentInwardDrugInvoice.DSPTModifiedDate = Globals.ServerDate.Value;
            CurrentInwardDrugInvoice.SelectedStaff = GetStaffLogin();
            CurrentInwardDrugInvoice.StaffID = GetStaffLogin().StaffID;
            CurrentInwardDrugInvoice.IsForeign = false;
            if (Currencies != null)
            {
                CurrentInwardDrugInvoice.SelectedCurrency = Currencies.FirstOrDefault();
            }
            if (StoreCbx != null)
            {
                CurrentInwardDrugInvoice.SelectedStorage = StoreCbx.FirstOrDefault();
            }
            if (StoreCbxAll != null)
            {
                CurrentInwardDrugInvoice.SelectedStorageOut = StoreCbxAll.FirstOrDefault();
            }
            IsHideAuSupplier = true;
            if (InwardDrugList != null)
            {
                InwardDrugList.Clear();
            }
            DeepCopyInvoice();
            InitCurrentInwardDrug();
            _IsCode = false;
        }

        private void DeepCopyInvoice()
        {
            CurrentInwardInvoiceCopy = CurrentInwardDrugInvoice.DeepCopy();
        }

        private void DeleteInwardInvoiceDrug()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteInwardInvoiceDrug(CurrentInwardDrugInvoice.inviID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndDeleteInwardInvoiceDrug(asyncResult);
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
            DeleteInwardInvoiceDrug();
        }

        private void UpdateInwardInvoiceDrug()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
            if (CheckValidInvoice())
            {
                CurrentInwardDrugInvoice.StaffID = GetStaffLogin().StaffID;
                CurrentInwardDrugInvoice.TypID = TypID;
                UpdateInwardInvoiceDrug();
            }
        }
        private void CountTotalPrice(decimal TongTienSPChuaVAT
            ,decimal CKTrenSP
            ,decimal TongTienTrenSPDaTruCK
            ,decimal TongCKTrenHoaDon
            ,decimal TongTienHoaDonCoVAT
            ,decimal TotalVATDifferenceAmount)
        {
            CurrentInwardDrugInvoice.TotalPriceNotVAT = TongTienSPChuaVAT;
            CurrentInwardDrugInvoice.TotalDiscountOnProduct = CKTrenSP + TongCKTrenHoaDon;//tong ck tren hoan don
            CurrentInwardDrugInvoice.TotalPrice = TongTienSPChuaVAT - (CKTrenSP + TongCKTrenHoaDon);
            CurrentInwardDrugInvoice.TotalPriceVAT = TongTienHoaDonCoVAT;
            CurrentInwardDrugInvoice.TotalVATDifferenceAmount = TotalVATDifferenceAmount;
        }

        private void InwardDrugDetails_ByID(long ID, int PageIndex, int PageSize)
        {
            isLoadingInvoiceDetailD = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
                            //tinh tong tien 
                            CountTotalPrice(TongTienSPChuaVAT, CKTrenSP, TongTienTrenSPDaTruCK, TongCKTrenHoaDon, TongTienHoaDonCoVAT, TotalVATDifferenceAmount);
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

        private void GetInwardInvoiceDrugByID(long ID)
        {
            isLoadingInvoiceID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddInwardInvoiceDrug(CurrentInwardDrugInvoice, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID;
                            int results = contract.EndAddInwardInvoiceDrug(out ID, asyncResult);
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


        private bool CheckValidInvoice()
        {
            string StrError = "";
            if (CurrentInwardDrugInvoice.SelectedStorageOut == null)
            {
                StrError += eHCMSResources.Z1082_G1_ChonNguonNhap + Environment.NewLine;
            }
            else
            {
                if (CurrentInwardDrugInvoice.SelectedStorageOut.StoreID < 1)
                {
                    StrError += eHCMSResources.Z1082_G1_ChonNguonNhap + Environment.NewLine;
                }
                if (CurrentInwardDrugInvoice.SelectedStorageOut.StoreID == CurrentInwardDrugInvoice.SelectedStorage.StoreID)
                {
                    StrError += eHCMSResources.Z1414_G1_NguonNhapVaKhoKgDcGiongNhau + Environment.NewLine;
                }
            }
            if (CurrentInwardDrugInvoice.SelectedStorage == null)
            {
                StrError += eHCMSResources.Z1415_G1_VuiLongChonKhoNhap + Environment.NewLine;
            }
            if (string.IsNullOrWhiteSpace(CurrentInwardDrugInvoice.InvInvoiceNumber))
            {
                StrError += eHCMSResources.Z1083_G1_ChuaNhapMaHD + Environment.NewLine;
            }
            if (CurrentInwardDrugInvoice.InvDateInvoice == null || CurrentInwardDrugInvoice.InvDateInvoice.Year < Globals.ServerDate.Value.Year - 50)
            {
                StrError += eHCMSResources.Z1416_G1_ChuaNhapNgHDon + Environment.NewLine;
            }
            if ((CurrentInwardDrugInvoice.VAT < 1 && CurrentInwardDrugInvoice.VAT != 0) || CurrentInwardDrugInvoice.VAT >= 2)
            {
                StrError += eHCMSResources.K0262_G1_VATKhongHopLe + Environment.NewLine;
            }
            if ((CurrentInwardDrugInvoice.DiscountingByPercent < 1 && CurrentInwardDrugInvoice.DiscountingByPercent != 0) || CurrentInwardDrugInvoice.DiscountingByPercent >= 2)
            {
                StrError += eHCMSResources.A0072_G1_CKKhHopLe + Environment.NewLine;
            }
            if (string.IsNullOrEmpty(StrError))
            {
                CurrentInwardDrugInvoice.StoreID = CurrentInwardDrugInvoice.SelectedStorage.StoreID;
                if (CurrentInwardDrugInvoice.SelectedStorageOut != null)
                {
                    CurrentInwardDrugInvoice.StoreIDOut = CurrentInwardDrugInvoice.SelectedStorageOut.StoreID;
                }
                return true;
            }
            else
            {
                Globals.ShowMessage(StrError, eHCMSResources.G0442_G1_TBao);
                return false;
            }
        }

        public void btnAdd(object sender, RoutedEventArgs e)
        {
            if (CheckValidInvoice())
            {
                CurrentInwardDrugInvoice.StaffID = GetStaffLogin().StaffID;
                CurrentInwardDrugInvoice.TypID = TypID;
                AddInwardInvoiceDrug();
            }
        }

        public void GridInwardDrug_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        private void IsChangedItem()
        {
            if (CurrentInwardDrugInvoice.CurrencyID != CurrentInwardInvoiceCopy.CurrencyID || CurrentInwardDrugInvoice.ExchangeRates != CurrentInwardInvoiceCopy.ExchangeRates || CurrentInwardDrugInvoice.VAT != CurrentInwardInvoiceCopy.VAT || CurrentInwardDrugInvoice.IsForeign != CurrentInwardInvoiceCopy.IsForeign || CurrentInwardDrugInvoice.Notes != CurrentInwardInvoiceCopy.Notes
                || CurrentInwardDrugInvoice.DiscountingByPercent != CurrentInwardInvoiceCopy.DiscountingByPercent)
            {
                if (MessageBox.Show(eHCMSResources.A0135_G1_Msg_ConfCNhatBillBeforeEdit, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (CheckValidInvoice())
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

        public bool CheckValid()
        {
            if (CurrentInwardDrug == null)
            {
                return false;
            }
            return CurrentInwardDrug.Validate();
        }

        public void btnNhapHang(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                if (CurrentInwardDrug.InQuantity <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.Z0572_G1_SLgNhapPhaiLonHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrug.InBuyingPrice <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0519_G1_Msg_InfoDGiaLonHon0, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrug.SelectedDrug.NormalPrice <= 0)
                {
                    Globals.ShowMessage(eHCMSResources.A0786_G1_Msg_InfoCNhatBGiaBan, eHCMSResources.G0442_G1_TBao);
                    return;
                }
                if (CurrentInwardDrug.SelectedDrug.NormalPrice < CurrentInwardDrug.SelectedDrug.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.Z0573_G1_GiaBNBHNhoHonDGia);
                    return;
                }
                if (CurrentInwardDrug.SelectedDrug.HIAllowedPrice > CurrentInwardDrug.SelectedDrug.PriceForHIPatient)
                {
                    MessageBox.Show(eHCMSResources.T1982_G1_GiaBHChoPhep2);
                    return;
                }
                if (CurrentInwardDrug.SelectedDrug.UnitPackaging.GetValueOrDefault(0) > 0)
                {
                    CurrentInwardDrug.PackageQuantity = CurrentInwardDrug.InQuantity / CurrentInwardDrug.SelectedDrug.UnitPackaging.GetValueOrDefault(0);
                }
                else
                {
                    CurrentInwardDrug.PackageQuantity = CurrentInwardDrug.InQuantity;
                }
                CurrentInwardDrug.PackagePrice = CurrentInwardDrug.InBuyingPrice * CurrentInwardDrug.SelectedDrug.UnitPackaging.GetValueOrDefault(0);

                IsChangedItem();
                ////////////////
                CurrentInwardDrug.inviID = CurrentInwardDrugInvoice.inviID;
                if (CurrentInwardDrug.SelectedDrug != null)
                {
                    CurrentInwardDrug.DrugID = CurrentInwardDrug.SelectedDrug.DrugID;
                }
                AddInwardDrug();
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1417_G1_CTietNhapKgDung, eHCMSResources.G0442_G1_TBao);
            }
        }

        private void AddInwardDrug()
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginAddInwardDrug(CurrentInwardDrug, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool results = contract.EndAddInwardDrug(asyncResult);
                            if (results)
                            {
                                InitCurrentInwardDrug();
                                Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.Z1418_G1_KgTheThemDuoc, eHCMSResources.T0074_G1_I);
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

        #region Auto For Location

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
            CurrentInwardDrug.SdlDescription = e.Parameter;
            SearchRefShelfLocation(e.Parameter);
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


        private void SearchRefDrugGenericDetails_AutoPaging(bool? IsCode, string Name, int PageIndex, int PageSize)
        {
            int totalCount = 0;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRefDrugGenericDetails_AutoPaging(IsCode, Name, 0, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var ListUnits = contract.EndSearchRefDrugGenericDetails_AutoPaging(out totalCount, asyncResult);
                            if (IsCode.GetValueOrDefault())
                            {
                                if (CurrentInwardDrug == null)
                                {
                                    CurrentInwardDrug = new InwardDrug();
                                }
                                if (ListUnits != null && ListUnits.Count > 0)
                                {
                                    CurrentInwardDrug.SelectedDrug = ListUnits.FirstOrDefault();
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0752_G1_Msg_InfoKhTimThay);
                                    InitCurrentInwardDrug();
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
                            _IsCode = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        AutoCompleteBox AuDrug;
        public void acbDrug_Populating(object sender, PopulatingEventArgs e)
        {
            if (!_IsCode)
            {
                AuDrug = sender as AutoCompleteBox;
                BrandName = e.Parameter;
                RefGenericDrugDetails.PageIndex = 0;
                SearchRefDrugGenericDetails_AutoPaging(null, e.Parameter, 0, RefGenericDrugDetails.PageSize); ;
            }
        }

        public void btnSearchDrug()
        {
            Action<IRefGenDrugList> onInitDlg = delegate (IRefGenDrugList proAlloc)
            {
                proAlloc.IsPopUp = true;
            };

            Action<IRefGenDrugListNew> onInitDlgNew = delegate (IRefGenDrugListNew proAlloc)
            {
                proAlloc.IsPopUp = true;
            };

            if (Globals.ServerConfigSection.CommonItems.EnableHIStore)
            {
                GlobalsNAV.ShowDialog<IRefGenDrugListNew>(onInitDlgNew);
            }
            else
            {
                GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg);
            }
            //GlobalsNAV.ShowDialog<IRefGenDrugList>(onInitDlg);
        }

        #endregion

        private void DeleteInwardDrug(long InID)
        {
            isLoadingFullOperator = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDeleteInwardDrug(InID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int results = contract.EndDeleteInwardDrug(asyncResult);
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
                            isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();

        }

        private void Load_InwardDrugDetails(long ID)
        {
            inviID = ID;
            InwardDrugList.PageIndex = 0;
            InwardDrugDetails_ByID(CurrentInwardDrugInvoice.inviID, 0, Globals.PageSize);
        }
        DataGrid GridInwardDrug = null;
        public void GridInwardDrug_Loaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug = sender as DataGrid;
        }

        public void lnkDeleteMain_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugInvoice.CanEditAndDelete)
            {
                if (GridInwardDrug != null && GridInwardDrug.SelectedItem != null)
                {
                    if (MessageBox.Show(eHCMSResources.A0118_G1_Msg_ConfXoaDong, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        DeleteInwardDrug((GridInwardDrug.SelectedItem as InwardDrug).InID);
                    }
                }
            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z0596_G1_ThuocDaKChuyenTDK, eHCMSResources.G0442_G1_TBao);
            }
        }

        public void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentInwardDrugInvoice.CanEditAndDelete)
            {
                IsChangedItem();
                if (GridInwardDrug != null && GridInwardDrug.SelectedItem != null)
                {
                    Action<IEditInwardDrug> onInitDlg = delegate (IEditInwardDrug proAlloc)
                    {
                        proAlloc.CurrentInwardDrugCopy = (GridInwardDrug.SelectedItem as InwardDrug).DeepCopy();
                        proAlloc.SetValueForProperty();
                    };
                    GlobalsNAV.ShowDialog<IEditInwardDrug>(onInitDlg);
                }

            }
            else
            {
                Globals.ShowMessage(eHCMSResources.Z1419_G1_ThuocDaKChTDK, eHCMSResources.G0442_G1_TBao);
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

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
        {
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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
                                    Action<IInwardDrugSupplierSearch> onInitDlg = delegate (IInwardDrugSupplierSearch proAlloc)
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.TypID = TypID;
                                        proAlloc.InwardInvoiceList.Clear();
                                        proAlloc.InwardInvoiceList.TotalItemCount = Totalcount;
                                        proAlloc.InwardInvoiceList.PageIndex = 0;
                                        proAlloc.pageTitle = eHCMSResources.N0206_G1_NhapHgTuKhac;
                                        foreach (InwardDrugInvoice p in results)
                                        {
                                            proAlloc.InwardInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IInwardDrugSupplierSearch>(onInitDlg);
                                }
                                else
                                {
                                    CurrentInwardDrugInvoice = results.FirstOrDefault();
                                    DeepCopyInvoice();
                                    InitCurrentInwardDrug();
                                    LoadInfoThenSelectedInvoice();
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
                            isLoadingSearch = false;
                            // Globals.IsBusy = false;
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

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            SearchInwardInvoiceDrug(0, Globals.PageSize);
        }

        #region IHandle<PharmacyCloseSearchInwardIncoiceEvent> Members

        public void Handle(PharmacyCloseSearchInwardIncoiceEvent message)
        {
            if (message != null && this.IsActive)
            {
                CurrentInwardDrugInvoice = message.SelectedInwardInvoice as InwardDrugInvoice;
                DeepCopyInvoice();
                InitCurrentInwardDrug();
                LoadInfoThenSelectedInvoice();

            }
        }

        private void InitCurrentInwardDrug()
        {
            CurrentInwardDrug = new InwardDrug();
            if (AuDrug != null)
            {
                AuDrug.Text = "";
            }
        }

        private void LoadInfoThenSelectedInvoice()
        {
            IsHideAuSupplier = false;
            Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
        }

        #endregion

        #region IHandle<PharmacyCloseEditInwardEvent> Members

        public void Handle(PharmacyCloseEditInwardEvent message)
        {
            if (this.IsActive)
            {
                Load_InwardDrugDetails(CurrentInwardDrugInvoice.inviID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            Action<IReportDocumentPreview> onInitDlg = delegate (IReportDocumentPreview proAlloc)
            {
                proAlloc.ID = CurrentInwardDrugInvoice.inviID;
                proAlloc.eItem = ReportName.PHARMACY_NHAPTHUOCTUNCC;
            };
            GlobalsNAV.ShowDialog<IReportDocumentPreview>(onInitDlg);
        }
        public void btnPrint()
        {
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
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

            });
            t.Start();
        }
        #endregion

        private ObservableCollection<string> _ListVAT;
        public ObservableCollection<string> ListVAT
        {
            get
            {
                return _ListVAT;
            }
            set
            {
                _ListVAT = value;
                NotifyOfPropertyChange(() => ListVAT);
            }
        }

        private void FillListVAT()
        {
            CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            ListVAT = new ObservableCollection<string>();
            //cost.ToString("#,###.##", currentCulture)
            ListVAT.Add((Convert.ToDecimal(1.10)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.05)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.20)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.25)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.01)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.02)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.03)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.04)).ToString("#,###.##", currentCulture));
            ListVAT.Add((Convert.ToDecimal(1.50)).ToString("#,###.##", currentCulture));
        }
        public void AutoCompleteBox_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            try
            {
                CurrentInwardDrugInvoice.VAT = Convert.ToDecimal((sender as AutoCompleteBox).Text);
            }
            catch
            {
                CurrentInwardDrugInvoice.VAT = 0;
            }
        }

        public void AutoCompleteBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as AutoCompleteBox).ItemsSource = ListVAT;
        }

        public void AutoCompleteBox_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as AutoCompleteBox).SetValue(AutoCompleteBox.ItemsSourceProperty, null);
        }

        #region IHandle<PharmacyCloseSearchDrugEvent> Members

        public void Handle(PharmacyCloseSearchDrugEvent message)
        {
            if (message != null)
            {
                CurrentInwardDrug.SelectedDrug = message.SupplierDrug as RefGenericDrugDetail;
            }
        }

        #endregion
        private bool _IsCode = false;

        public void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _IsCode = true;
            AxTextBox obj = sender as AxTextBox;
            if (obj != null && !string.IsNullOrEmpty(obj.Text))
            {
                if (CurrentInwardDrug != null && CurrentInwardDrug.SelectedDrug != null)
                {
                    if (CurrentInwardDrug.SelectedDrug.DrugCode != obj.Text)
                    {
                        SearchRefDrugGenericDetails_AutoPaging(true, obj.Text, 0, RefGenericDrugDetails.PageSize);
                    }
                }
                else
                {
                    SearchRefDrugGenericDetails_AutoPaging(true, obj.Text, 0, RefGenericDrugDetails.PageSize);
                }
            }
            _IsCode = false;
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            RefGenericDrugDetail obj = (sender as AutoCompleteBox).SelectedItem as RefGenericDrugDetail;
            if (CurrentInwardDrug != null && CurrentInwardDrug.SelectedDrug != null)
            {
                if (obj != null && CurrentInwardDrug.SelectedDrug.BrandName != obj.BrandName)
                {
                    CurrentInwardDrug.SelectedDrug = obj;
                }
            }
            else
            {
                CurrentInwardDrug.SelectedDrug = obj;
            }
        }
    }
}
