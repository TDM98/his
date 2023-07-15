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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using System.Linq;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Controls;
using System.Windows.Data;


namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptDamageExpiryDrug)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptDamageExpiryDrugViewModel : Conductor<object>, IDrugDeptDamageExpiryDrug, IHandle<DrugDeptCloseSearchDemageDrugEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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
            get { return (isLoadingGetStore || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion

        [ImportingConstructor]
        public DrugDeptDamageExpiryDrugViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            authorization();
            //Globals.EventAggregator.Subscribe(this);
            CurrentOutwardDrugMedDeptInvoice = new OutwardDrugMedDeptInvoice();
            CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;

            Coroutine.BeginExecute(DoGetStore_MedDept());

            SearchCriteria = new MedDeptInvoiceSearchCriteria();

            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;

            GetStaffLogin();
            UnCheckPaging();
        }

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
        private bool _mXuatExcel = true;
        private bool _mXemIn = true;

        public bool mTim
        {
            get
            {
                return _mTim;
            }
            set
            {
                if (_mTim== value)
                    return;
                _mTim= value;
                NotifyOfPropertyChange(() => mTim);
            }
        }


        public bool mThemMoi
        {
            get
            {
                return _mThemMoi
                ;
            }
            set
            {
                if (_mThemMoi== value)
                    return;
                _mThemMoi= value;
                NotifyOfPropertyChange(() => mThemMoi);
            }
        }

        public bool mXuatExcel
        {
            get
            {
                return _mXuatExcel;
            }
            set
            {
                if (_mXuatExcel== value)
                    return;
                _mXuatExcel= value;
                NotifyOfPropertyChange(() => mXuatExcel);
            }
        }


        public bool mXemIn
        {
            get
            {
                return _mXemIn;
            }
            set
            {
                if (_mXemIn == value)
                    return;
                _mXemIn = value;
                NotifyOfPropertyChange(() => mXemIn);
            }
        }


        private bool _bEdit = true;
        private bool _bAdd = true;
        private bool _bDelete = true;
        private bool _bView = true;
        private bool _bPrint = true;
        private bool _bReport = true;
        private bool _bXuatExcel = true;

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

        private OutwardDrugMedDeptInvoice _CurrentOutwardDrugMedDeptInvoice;
        public OutwardDrugMedDeptInvoice CurrentOutwardDrugMedDeptInvoice
        {
            get
            {
                return _CurrentOutwardDrugMedDeptInvoice;
            }
            set
            {
                if (_CurrentOutwardDrugMedDeptInvoice != value)
                {
                    _CurrentOutwardDrugMedDeptInvoice = value;
                    NotifyOfPropertyChange(() => CurrentOutwardDrugMedDeptInvoice);
                }
            }
        }

        private CollectionViewSource CVS_PCVOutwardDrugDetails = null;
        public CollectionView CV_PCVOutwardDrugDetails { get; set; }
       

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

        private ObservableCollection<OutwardDrugMedDept> OutwardDrugDetailList;

        private MedDeptInvoiceSearchCriteria _SearchCriteria;
        public MedDeptInvoiceSearchCriteria SearchCriteria
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
            if (CurrentOutwardDrugMedDeptInvoice != null)
            {
                CurrentOutwardDrugMedDeptInvoice.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentOutwardDrugMedDeptInvoice.StaffName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        #endregion

        #region Function Member

        private IEnumerator<IResult> DoGetStore_MedDept()
        {
            isLoadingGetStore = true;
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_DRUGDEPT, false,null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            isLoadingGetStore = false;
            yield break;
        }

        private void OutwardDrugMedDeptInvoice_Search(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                SearchCriteria = new MedDeptInvoiceSearchCriteria();
            }
            SearchCriteria.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            SearchCriteria.V_MedProductType = V_MedProductType;

            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoice_SearchByType(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndOutwardDrugMedDeptInvoice_SearchByType(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                if (results.Count > 1)
                                {
                                    //mo pop up tim
                                    //var proAlloc = Globals.GetViewModel<IDrugDeptDamageExpiryDrugSearch>();
                                    //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                    //proAlloc.OutwardDrugMedDeptInvoiceList.Clear();
                                    //proAlloc.OutwardDrugMedDeptInvoiceList.TotalItemCount = TotalCount;
                                    //proAlloc.OutwardDrugMedDeptInvoiceList.PageIndex = 0;
                                    //foreach (OutwardDrugMedDeptInvoice p in results)
                                    //{
                                    //    proAlloc.OutwardDrugMedDeptInvoiceList.Add(p);
                                    //}
                                    //var instance = proAlloc as Conductor<object>;
                                    //Globals.ShowDialog(instance, (o) => { });

                                    Action<IDrugDeptDamageExpiryDrugSearch> onInitDlg = (proAlloc) =>
                                    {
                                        proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                        proAlloc.OutwardDrugMedDeptInvoiceList.Clear();
                                        proAlloc.OutwardDrugMedDeptInvoiceList.TotalItemCount = TotalCount;
                                        proAlloc.OutwardDrugMedDeptInvoiceList.PageIndex = 0;
                                        foreach (OutwardDrugMedDeptInvoice p in results)
                                        {
                                            proAlloc.OutwardDrugMedDeptInvoiceList.Add(p);
                                        }
                                    };
                                    GlobalsNAV.ShowDialog<IDrugDeptDamageExpiryDrugSearch>(onInitDlg);
                                }
                                else
                                {
                                    long? storeID = CurrentOutwardDrugMedDeptInvoice.StoreID.DeepCopy();
                                    if (storeID != results.FirstOrDefault().StoreID)
                                    {
                                        IsOld = true;
                                    }
                                    CurrentOutwardDrugMedDeptInvoice = results.FirstOrDefault();
                                    //load detail
                                    UnCheckPaging();
                                    OutwardDrugDetails_Load(CurrentOutwardDrugMedDeptInvoice.outiID);
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
                            //Globals.IsBusy = false;
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

        private void OutwardDrugDetails_Load(long DrugDeptStockTakeID)
        {
            isLoadingDetail = true;
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugMedDeptDetailByInvoice(DrugDeptStockTakeID, V_MedProductType, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutwardDrugMedDeptDetailByInvoice(asyncResult);
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
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void OutwardDrugMedDeptInvoice_Get(long ID)
        {
            isLoadingGetID = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetOutwardDrugMedDeptInvoice(ID, V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetOutwardDrugMedDeptInvoice(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            if (results != null)
                            {
                                CurrentOutwardDrugMedDeptInvoice.OutInvID = results.OutInvID;
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

        private void OutwardDrugDetails_Get(long ID)
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetListDrugExpiryDate_DrugDept(ID, Type,V_MedProductType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetListDrugExpiryDate_DrugDept(asyncResult);
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
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginOutwardDrugMedDeptInvoice_SaveByType(CurrentOutwardDrugMedDeptInvoice, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID = 0;
                            string StrError;
                            var results = contract.EndOutwardDrugMedDeptInvoice_SaveByType(out ID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                //load danh sach thuoc theo hoa don 
                                CurrentOutwardDrugMedDeptInvoice.outiID = ID;
                                OutwardDrugMedDeptInvoice_Get(ID);
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
            if (OutwardDrugDetailList == null)
            {
                OutwardDrugDetailList = new ObservableCollection<OutwardDrugMedDept>();
            }
            if (CVS_PCVOutwardDrugDetails == null)
            {
                CVS_PCVOutwardDrugDetails = new CollectionViewSource { Source = OutwardDrugDetailList };
                CV_PCVOutwardDrugDetails = (CollectionView)CVS_PCVOutwardDrugDetails.View;
                NotifyOfPropertyChange(() => CV_PCVOutwardDrugDetails);
            }
            else
            {
                CV_PCVOutwardDrugDetails.Refresh();
            }
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
            if (CV_PCVOutwardDrugDetails != null)
            {
                CV_PCVOutwardDrugDetails.Filter = null;
                CV_PCVOutwardDrugDetails.Filter = new Predicate<object>(DoFilter);
            }
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            OutwardDrugMedDept emp = o as OutwardDrugMedDept;

            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (emp.RefGenericDrugDetail.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
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
            if (CurrentOutwardDrugMedDeptInvoice.StoreID == null || CurrentOutwardDrugMedDeptInvoice.StoreID == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z0828_G1_ChonKhoCanHuy, eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                if (OutwardDrugDetailList != null && OutwardDrugDetailList.Count > 0)
                {
                    if (MessageBox.Show(eHCMSResources.A0084_G1_Msg_ConfHuyDSThuoc, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
                        CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = OutwardDrugDetailList;
                        CurrentOutwardDrugMedDeptInvoice.V_MedProductType = V_MedProductType;
                        OutwardDrugDetails_Save();
                    }
                }
                else
                {
                    Globals.ShowMessage(string.Format("{0}!", eHCMSResources.Z0829_G1_KgCoThuocTrongDSHuy), eHCMSResources.G0442_G1_TBao);
                }
            }
        }

        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                if (!IsOld && CurrentOutwardDrugMedDeptInvoice != null)
                {
                    CurrentOutwardDrugMedDeptInvoice.outiID = 0;
                    CurrentOutwardDrugMedDeptInvoice.OutInvID = "";
                    CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
                    CurrentOutwardDrugMedDeptInvoice.FullName = "";
                    CurrentOutwardDrugMedDeptInvoice.OutDate = DateTime.Now;
                    CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = null;
                    CurrentOutwardDrugMedDeptInvoice.StoreID = (long)(sender as ComboBox).SelectedValue;
                    OutwardDrugDetails_Get(CurrentOutwardDrugMedDeptInvoice.StoreID.GetValueOrDefault());
                }
                IsOld = false;
            }
        }

        public void btnSearch()
        {
            OutwardDrugMedDeptInvoice_Search(0, Globals.PageSize);
        }


        public void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CodeInvoice = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }

        private int Type = 0;

        public void rdtExpiry_Checked(object sender, RoutedEventArgs e)
        {

            ClearData();
            Type = 0;
            CurrentOutwardDrugMedDeptInvoice.outiID = 0;
            CurrentOutwardDrugMedDeptInvoice.OutInvID = "";
            CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugMedDeptInvoice.FullName = "";
            CurrentOutwardDrugMedDeptInvoice.OutDate = DateTime.Now;
            CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = null;
            OutwardDrugDetails_Get(CurrentOutwardDrugMedDeptInvoice.StoreID.GetValueOrDefault());

        }

        public void rdtPreExpiry_Checked(object sender, RoutedEventArgs e)
        {

            ClearData();
            Type = 1;
            CurrentOutwardDrugMedDeptInvoice.outiID = 0;
            CurrentOutwardDrugMedDeptInvoice.OutInvID = "";
            CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugMedDeptInvoice.FullName = "";
            CurrentOutwardDrugMedDeptInvoice.OutDate = DateTime.Now;
            CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = null;
            OutwardDrugDetails_Get(CurrentOutwardDrugMedDeptInvoice.StoreID.GetValueOrDefault());

        }
        public void rdtAll_Checked(object sender, RoutedEventArgs e)
        {

            ClearData();
            Type = 2;
            CurrentOutwardDrugMedDeptInvoice.outiID = 0;
            CurrentOutwardDrugMedDeptInvoice.OutInvID = "";
            CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugMedDeptInvoice.FullName = "";
            CurrentOutwardDrugMedDeptInvoice.OutDate = DateTime.Now;
            CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = null;
            OutwardDrugDetails_Get(CurrentOutwardDrugMedDeptInvoice.StoreID.GetValueOrDefault());

        }


        public void btnNew()
        {
            CurrentOutwardDrugMedDeptInvoice.outiID = 0;
            CurrentOutwardDrugMedDeptInvoice.OutInvID = "";
            CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugMedDeptInvoice.FullName = "";
            CurrentOutwardDrugMedDeptInvoice.OutDate = DateTime.Now;
            CurrentOutwardDrugMedDeptInvoice.StoreID = 0;
            CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = null;
            OutwardDrugDetailList = null;
            CVS_PCVOutwardDrugDetails = null;
            UnCheckPaging();
        }

        public void ClearData()
        {
            CurrentOutwardDrugMedDeptInvoice.outiID = 0;
            CurrentOutwardDrugMedDeptInvoice.TypID = (long)AllLookupValues.RefOutputType.HUYHANG;
            CurrentOutwardDrugMedDeptInvoice.FullName = "";
            CurrentOutwardDrugMedDeptInvoice.OutDate = DateTime.Now;
            CurrentOutwardDrugMedDeptInvoice.OutwardDrugMedDepts = null;
            OutwardDrugDetailList = null;
            CVS_PCVOutwardDrugDetails = null;
            UnCheckPaging();
        }

        #region IHandle<DrugDeptCloseSearchDemageDrugEvent> Members

        public void Handle(DrugDeptCloseSearchDemageDrugEvent message)
        {
            if (message != null && this.IsActive)
            {
                OutwardDrugMedDeptInvoice Item = message.SelectedOutwardDrugMedDeptInvoice as OutwardDrugMedDeptInvoice;
                long? storeID = CurrentOutwardDrugMedDeptInvoice.StoreID.DeepCopy();
                if (storeID != Item.StoreID)
                {
                    IsOld = true;
                }
                CurrentOutwardDrugMedDeptInvoice = Item;
                UnCheckPaging();
                //load detail
                OutwardDrugDetails_Load(CurrentOutwardDrugMedDeptInvoice.outiID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            //var proAlloc = Globals.GetViewModel<IDrugDeptReportDocumentPreview>();
            //proAlloc.ID = CurrentOutwardDrugMedDeptInvoice.outiID;
            //if (CurrentOutwardDrugMedDeptInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0830_G1_PhXuatHuyThuoc.ToUpper();
            //}
            //else if (CurrentOutwardDrugMedDeptInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0831_G1_PhXuatHuyYCu.ToUpper();
            //}
            //else
            //{
            //    proAlloc.LyDo = eHCMSResources.Z0832_G1_PhXuatHuyHChat.ToUpper();
            //}
            
            //proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;

            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            Action<IDrugDeptReportDocumentPreview> onInitDlg = (proAlloc) =>
            {
                proAlloc.ID = CurrentOutwardDrugMedDeptInvoice.outiID;
                if (CurrentOutwardDrugMedDeptInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.THUOC)
                {
                    proAlloc.LyDo = eHCMSResources.Z0830_G1_PhXuatHuyThuoc.ToUpper();
                }
                else if (CurrentOutwardDrugMedDeptInvoice.V_MedProductType == (long)AllLookupValues.MedProductType.Y_CU)
                {
                    proAlloc.LyDo = eHCMSResources.Z0831_G1_PhXuatHuyYCu.ToUpper();
                }
                else
                {
                    proAlloc.LyDo = eHCMSResources.Z0832_G1_PhXuatHuyHChat.ToUpper();
                }

                proAlloc.eItem = ReportName.DRUGDEPT_HUYTHUOC;
            };
            GlobalsNAV.ShowDialog<IDrugDeptReportDocumentPreview>(onInitDlg);
        }

        //public void btnPrint()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new ReportServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;

        //            contract.BeginGetDrugDeptDemageDrugInPdfFormat(CurrentOutwardDrugMedDeptInvoice.outiID, Globals.DispatchCallback((asyncResult) =>
        //            {
        //                try
        //                {
        //                    var results = contract.EndGetDrugDeptDemageDrugInPdfFormat(asyncResult);
        //                    ClientPrintHelper.PrintPdfData(results);
        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });
        //    t.Start();
        //}

        #endregion

    }
}
