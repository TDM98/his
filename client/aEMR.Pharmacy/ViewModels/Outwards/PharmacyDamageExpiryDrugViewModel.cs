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
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common.Printing;
using aEMR.Common;
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.Controls;
using aEMR.Common.PagedCollectionView;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyDamageExpiryDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyDamageExpiryDrugViewModel : Conductor<object>, IPharmacyDamageExpiryDrug, IHandle<PharmacyCloseSearchDemageDrugEvent>
    {
        public string TitleForm { get; set; }

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
            get { return ( isLoadingGetStore || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyDamageExpiryDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            authorization();
            //Globals.EventAggregator.Subscribe(this);
            CurrentOutwardDrugInvoice = new OutwardDrugInvoice();
            CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            SearchCriteria = new SearchOutwardInfo();

            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;

            GetStaffLogin();
            UnCheckPaging();
        }
        //protected override void OnActivate()
        //{
        //    //khi nao share thi dung cai nay
        //}
        //protected override void OnDeactivate(bool close)
        //{
        //    CurrentOutwardDrugInvoice = null;
        //    OutwardDrugDetailList = null;
        //    PCVOutwardDrugDetails = null;
        //    SearchCriteria = null;
        //    if (GridStockTakes != null)
        //    {
        //        GridStockTakes.SetValue(DataGrid.ItemsSourceProperty, null);
        //    }
        //   // Globals.EventAggregator.Unsubscribe(this);

        //}

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            
            
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuyThuocHetHan,
                                               (int)oPharmacyEx.mXuatHuyThuocHetHan_Tim, (int)ePermission.mView);
            bThem = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuyThuocHetHan,
                                               (int)oPharmacyEx.mXuatHuyThuocHetHan_Them, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuyThuocHetHan,
                                               (int)oPharmacyEx.mXuatHuyThuocHetHan_ThucHien, (int)ePermission.mView);
            bXuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mXuatHuyThuocHetHan,
                                               (int)oPharmacyEx.mXuatHuyThuocHetHan_XuatExcel, (int)ePermission.mView);
            
        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bIn = true;
        private bool _bXuatExcel = true;

        public bool bTim
        {
            get
            {
                return _bTim;
            }
            set
            {
                if (_bTim == value)
                    return;
                _bTim = value;
            }
        }
        public bool bThem
        {
            get
            {
                return _bThem;
            }
            set
            {
                if (_bThem == value)
                    return;
                _bThem = value;
            }
        }
        public bool bIn
        {
            get
            {
                return _bIn;
            }
            set
            {
                if (_bIn == value)
                    return;
                _bIn = value;
            }
        }
        public bool bXuatExcel
        {
            get
            {
                return _bXuatExcel;
            }
            set
            {
                if (_bXuatExcel == value)
                    return;
                _bXuatExcel = value;
            }
        }
        #endregion

        #region Propeties Member
        private bool IsOld = false;

        private string _SearchKey;
        public string SearchKey
        {
            get { return _SearchKey; }
            set
            {
                _SearchKey = value;
                NotifyOfPropertyChange(() => SearchKey);
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

        private OutwardDrugInvoice _CurrentOutwardDrugInvoice;
        public OutwardDrugInvoice CurrentOutwardDrugInvoice
        {
            get
            {
                return _CurrentOutwardDrugInvoice;
            }
            set
            {
                if (_CurrentOutwardDrugInvoice != value)
                {
                    _CurrentOutwardDrugInvoice = value;
                    NotifyOfPropertyChange(() => CurrentOutwardDrugInvoice);
                }
            }
        }

        private PagedCollectionView _PCVOutwardDrugDetails;
        public PagedCollectionView PCVOutwardDrugDetails
        {
            get
            {
                return _PCVOutwardDrugDetails;
            }
            set
            {
                if (_PCVOutwardDrugDetails != value)
                {
                    _PCVOutwardDrugDetails = value;
                    NotifyOfPropertyChange(() => PCVOutwardDrugDetails);
                }
            }
        }

        private int _PCVPageSize = 15;
        public int PCVPageSize
        {
            get
            {
                return _PCVPageSize;
            }
            set
            {
                if (_PCVPageSize != value)
                {
                    _PCVPageSize = value;
                    NotifyOfPropertyChange(() => PCVPageSize);
                }
            }
        }

        private Visibility _VisibilityPaging = Visibility.Collapsed;
        public Visibility VisibilityPaging
        {
            get
            {
                return _VisibilityPaging;
            }
            set
            {
                if (_VisibilityPaging != value)
                {
                    _VisibilityPaging = value;
                    NotifyOfPropertyChange(() => VisibilityPaging);
                }
            }
        }

        private ObservableCollection<OutwardDrug> OutwardDrugDetailList;

        private SearchOutwardInfo _SearchCriteria;
        public SearchOutwardInfo SearchCriteria
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
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }

        private Staff GetStaffLogin()
        {
            if (CurrentOutwardDrugInvoice != null)
            {
                CurrentOutwardDrugInvoice.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentOutwardDrugInvoice.SelectedStaff = new Staff();
                CurrentOutwardDrugInvoice.SelectedStaff.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentOutwardDrugInvoice.SelectedStaff.FullName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        #endregion

        #region Function Member

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            isLoadingGetStore = false;
            yield break;
        }

        private void OutwardDrugInvoice_Search(int PageIndex, int PageSize)
        {
            isLoadingSearch = true;
          //  Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutWardDrugInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndOutWardDrugInvoice_SearchByType(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    //var proAlloc = Globals.GetViewModel<IPharmacyDamageExpiryDrugSearch>();
                                    //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                    //proAlloc.OutwardDrugInvoiceList.Clear();
                                    //proAlloc.OutwardDrugInvoiceList.TotalItemCount = TotalCount;
                                    //proAlloc.OutwardDrugInvoiceList.PageIndex = 0;
                                    //proAlloc.pageTitle = eHCMSResources.G2888_G1_XuatHuyThuocHetHanTimPhCu;
                                    //foreach (OutwardDrugInvoice p in results)
                                    //{
                                    //    proAlloc.OutwardDrugInvoiceList.Add(p);
                                    //}
                                    //var instance = proAlloc as Conductor<object>;
                                    //Globals.ShowDialog(instance, (o) => { });

                                    Action<IPharmacyDamageExpiryDrugSearch> onInitDlg = (proAlloc) =>
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardDrugInvoiceList.Clear();
                                        proAlloc.OutwardDrugInvoiceList.TotalItemCount = TotalCount;
                                        proAlloc.OutwardDrugInvoiceList.PageIndex = 0;
                                        proAlloc.pageTitle = eHCMSResources.G2888_G1_XuatHuyThuocHetHanTimPhCu;
                                        foreach (OutwardDrugInvoice p in results)
                                        {
                                            proAlloc.OutwardDrugInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IPharmacyDamageExpiryDrugSearch>(onInitDlg);
                                }
                                else
                                {
                                    long? storeID = CurrentOutwardDrugInvoice.StoreID.DeepCopy();
                                    if (storeID != results.FirstOrDefault().StoreID)
                                    {
                                        IsOld = true;
                                    }
                                    CurrentOutwardDrugInvoice = results.FirstOrDefault();
                                    //load detail
                                    UnCheckPaging();
                                    OutwardDrugDetails_Load(CurrentOutwardDrugInvoice.outiID);
                                }
                            }
                            else
                            {
                                Globals.ShowMessage(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao);
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

        private void UnCheckPaging()
        {
            if (PagingChecked != null && OutwardDrugDetailList != null)
            {
                PagingChecked.IsChecked = false;
                VisibilityPaging = Visibility.Collapsed;
            }
        }

        private void OutwardDrugDetails_Load(long PharmacyStockTakeID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugDetailsByOutwardInvoice(PharmacyStockTakeID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutwardDrugDetailsByOutwardInvoice(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            OutwardDrugDetailList = results.ToObservableCollection();
                            LoadDataGrid();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void OutwardDrugInvoice_Get(long ID)
        {
            isLoadingGetID = true;
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutWardDrugInvoiceVisitorByID(ID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutWardDrugInvoiceVisitorByID(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            if (results != null)
                            {
                                CurrentOutwardDrugInvoice.OutInvID = results.OutInvID;
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void OutwardDrugDetails_Get(long ID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetListDrugExpiryDate(ID, Type, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetListDrugExpiryDate(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            OutwardDrugDetailList = results.ToObservableCollection();
                            LoadDataGrid();
                            //tinh tong tien 
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void OutwardDrugDetails_Save()
        {
            isLoadingFullOperator = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginListDrugExpiryDate_Save(CurrentOutwardDrugInvoice, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID = 0;
                            string StrError;
                            var results = contract.EndListDrugExpiryDate_Save(out ID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                //load danh sach thuoc theo hoa don 
                                CurrentOutwardDrugInvoice.outiID = ID;
                                OutwardDrugInvoice_Get(ID);
                            }
                            else
                            {
                                Globals.ShowMessage(StrError, eHCMSResources.G0442_G1_TBao);
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

        #endregion
        DataGrid GridStockTakes = null;
        public void GridStockTakes_Loaded(object sender, RoutedEventArgs e)
        {
            GridStockTakes = sender as DataGrid;
        }

        DataPager pagerStockTakes = null;
        public void pagerStockTakes_Loaded(object sender, RoutedEventArgs e)
        {
            pagerStockTakes = sender as DataPager;
        }
        public void GridStockTakes_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        CheckBox PagingChecked;
        public void Paging_Checked(object sender, RoutedEventArgs e)
        {
            //avtivate datapager
            PagingChecked = sender as CheckBox;
            //pagerStockTakes.Source = GridStockTakes.ItemsSource;
            VisibilityPaging = Visibility.Visible;
        }

        public void Paging_Unchecked(object sender, RoutedEventArgs e)
        {
            //deavtivate datapager
            pagerStockTakes.Source = null;
            VisibilityPaging = Visibility.Collapsed;

            LoadDataGrid();
        }

        private void LoadDataGrid()
        {
            PCVOutwardDrugDetails = null;
            PCVOutwardDrugDetails = new PagedCollectionView(OutwardDrugDetailList);
            NotifyOfPropertyChange(() => PCVOutwardDrugDetails);
            btnFilter();
        }

        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
            }
        }

        public void btnFilter()
        {
            if (PCVOutwardDrugDetails != null)
            {
                PCVOutwardDrugDetails.Filter = null;
                PCVOutwardDrugDetails.Filter = new Predicate<object>(DoFilter);
            }
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            OutwardDrug emp = o as OutwardDrug;

            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (emp.GetDrugForSellVisitor.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public void SearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchKey = (sender as TextBox).Text;
                btnFilter();
            }
        }

        public void btnSave()
        {
            if (CurrentOutwardDrugInvoice.StoreID == null || CurrentOutwardDrugInvoice.StoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0828_G1_ChonKhoCanHuy, eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                if (OutwardDrugDetailList != null && OutwardDrugDetailList.Count > 0)
                {
                    if (MessageBox.Show(eHCMSResources.A0084_G1_Msg_ConfHuyDSThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
                        CurrentOutwardDrugInvoice.OutwardDrugs = OutwardDrugDetailList;
                        OutwardDrugDetails_Save();
                    }
                }
                else
                {
                    Globals.ShowMessage(eHCMSResources.Z0829_G1_KgCoThuocTrongDSHuy, eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                if (!IsOld && CurrentOutwardDrugInvoice != null)
                {
                    CurrentOutwardDrugInvoice.outiID = 0;
                    CurrentOutwardDrugInvoice.OutInvID = "";
                    CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
                    CurrentOutwardDrugInvoice.FullName = "";
                    CurrentOutwardDrugInvoice.OutDate = Globals.ServerDate.Value;
                    CurrentOutwardDrugInvoice.OutwardDrugs = null;
                    CurrentOutwardDrugInvoice.StoreID = (long)(sender as ComboBox).SelectedValue;
                    OutwardDrugDetails_Get(CurrentOutwardDrugInvoice.StoreID.GetValueOrDefault());
                }
                IsOld = false;
            }
        }

        public void btnSearch()
        {
            OutwardDrugInvoice_Search(0, Globals.PageSize);
        }


        public void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.OutInvID = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }

        private int Type = 0;

        public void rdtExpiry_Checked(object sender, RoutedEventArgs e)
        {

            ClearData();
            Type = 0;
            CurrentOutwardDrugInvoice.outiID = 0;
            CurrentOutwardDrugInvoice.OutInvID = "";
            CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugInvoice.FullName = "";
            CurrentOutwardDrugInvoice.OutDate = Globals.ServerDate.Value;
            CurrentOutwardDrugInvoice.OutwardDrugs = null;
            OutwardDrugDetails_Get(CurrentOutwardDrugInvoice.StoreID.GetValueOrDefault());

        }

        public void rdtPreExpiry_Checked(object sender, RoutedEventArgs e)
        {

            ClearData();
            Type = 1;
            CurrentOutwardDrugInvoice.outiID = 0;
            CurrentOutwardDrugInvoice.OutInvID = "";
            CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugInvoice.FullName = "";
            CurrentOutwardDrugInvoice.OutDate = Globals.ServerDate.Value;
            CurrentOutwardDrugInvoice.OutwardDrugs = null;
            OutwardDrugDetails_Get(CurrentOutwardDrugInvoice.StoreID.GetValueOrDefault());

        }
        public void rdtAll_Checked(object sender, RoutedEventArgs e)
        {

            ClearData();
            Type = 2;
            CurrentOutwardDrugInvoice.outiID = 0;
            CurrentOutwardDrugInvoice.OutInvID = "";
            CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugInvoice.FullName = "";
            CurrentOutwardDrugInvoice.OutDate = Globals.ServerDate.Value;
            CurrentOutwardDrugInvoice.OutwardDrugs = null;
            OutwardDrugDetails_Get(CurrentOutwardDrugInvoice.StoreID.GetValueOrDefault());

        }


        public void btnNew()
        {
            if (MessageBox.Show(eHCMSResources.A0144_G1_Msg_ConfTaoMoiPhHuyThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                CurrentOutwardDrugInvoice.outiID = 0;
                CurrentOutwardDrugInvoice.OutInvID = "";
                CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
                CurrentOutwardDrugInvoice.FullName = "";
                CurrentOutwardDrugInvoice.OutDate = Globals.ServerDate.Value;
                CurrentOutwardDrugInvoice.StoreID = 0;
                CurrentOutwardDrugInvoice.OutwardDrugs = null;
                OutwardDrugDetailList = null;
                PCVOutwardDrugDetails = null;
                UnCheckPaging();
            }
        }
        public void ClearData()
        {
            CurrentOutwardDrugInvoice.outiID = 0;
            CurrentOutwardDrugInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugInvoice.FullName = "";
            CurrentOutwardDrugInvoice.OutDate = Globals.ServerDate.Value;
            CurrentOutwardDrugInvoice.OutwardDrugs = null;
            OutwardDrugDetailList = null;
            PCVOutwardDrugDetails = null;
            UnCheckPaging();
        }
        #region IHandle<PharmacyCloseSearchDemageDrugEvent> Members

        public void Handle(PharmacyCloseSearchDemageDrugEvent message)
        {
            if (message != null && this.IsActive)
            {
                OutwardDrugInvoice Item = message.SelectedOutwardDrugInvoice as OutwardDrugInvoice;
                long? storeID = CurrentOutwardDrugInvoice.StoreID.DeepCopy();
                if (storeID != Item.StoreID)
                {
                    IsOld = true;
                }
                CurrentOutwardDrugInvoice = Item;
                UnCheckPaging();
                //load detail
                OutwardDrugDetails_Load(CurrentOutwardDrugInvoice.outiID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            //var proAlloc = Globals.GetViewModel<IReportDocumentPreview>();
            //proAlloc.ID = CurrentOutwardDrugInvoice.outiID;
            //proAlloc.eItem = ReportName.PHARMACY_PHIEUHUYHANG;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = CurrentOutwardDrugInvoice.outiID;
                proAlloc.eItem = ReportName.PHARMACY_PHIEUHUYHANG;
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

                    contract.BeginGetPharmacyDemageDrugInPdfFormat(CurrentOutwardDrugInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetPharmacyDemageDrugInPdfFormat(asyncResult);
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

    }
}



