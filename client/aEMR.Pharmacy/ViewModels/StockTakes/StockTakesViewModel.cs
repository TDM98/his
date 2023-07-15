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
using aEMR.Common.Printing;
using aEMR.CommonTasks;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.PagedCollectionView;
using Microsoft.Win32;
using System.Windows.Data;
using System.IO;
using OfficeOpenXml;
/*
* 20181006 #001 TTM:   Thay đổi cách lấy giá trị cột từ e.Column.GetValue(FrameworkElement.NameProperty).ToString()
*                      => e.Column.Equals("Tên Grid".GetColumnByName("Tên cột") vì cách cũ bị sai => Lấy giá trị cột không đc. 
* 20181008 #002 TTM: Chuyển từ PageCollectionView => CollectionViewSource. Do WPF ko sử dụng đc PagedCollectionView (hoặc do mình không biết xài PagedCollectionView).
* 20200714 #003 TTM: BM 0039359: Bổ sung kiểm tra nếu đã có kiểm kê rồi thì không cho phép thay đổi Ngày giờ kiểm kê.
* 20200715 #004 KVT: BM 0039359: Bổ sung thêm điều kiện để vừa lưu sẽ không cho cập nhật ngày giờ kiểm kê, Do không thể Notify được nên dựa vào điều kiện có PharmacyStockTakeID khi lưu sẽ set IsEnableStockTakingDate = false.
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IStockTakes)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockTakesViewModel : Conductor<object>, IStockTakes
        , IHandle<PharmacyCloseSearchStockTakesEvent>
    {
        public string TitleForm { get; set; }

        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StockTakesViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            //Globals.EventAggregator.Subscribe(this);
            CurrentPharmacyStockTakes = new PharmacyStockTakes();

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());

            SearchCriteria = new PharmacyStockTakesSearchCriteria();
            authorization();
            GetStaffLogin();
            UnCheckPaging();
        }
        //protected override void OnActivate()
        //{
        //}
        //protected override void OnDeactivate(bool close)
        //{
        //    CurrentPharmacyStockTakes = null;
        //    PharmacyStockTakeDetailList = null;
        //    PCVPharmacyStockTakeDetails = null;
        //    SearchCriteria=null;

        //}
        #region Propeties Member

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

        private PharmacyStockTakes _CurrentPharmacyStockTakes;
        public PharmacyStockTakes CurrentPharmacyStockTakes
        {
            get
            {
                return _CurrentPharmacyStockTakes;
            }
            set
            {
                if (_CurrentPharmacyStockTakes != value)
                {
                    _CurrentPharmacyStockTakes = value;
                    //▼===== #003
                    if (CurrentPharmacyStockTakes != null && CurrentPharmacyStockTakes.PharmacyStockTakeID > 0)
                    {
                        IsEnableStockTakingDate = false;
                        NotifyOfPropertyChange(() => IsEnableStockTakingDate);
                    }
                    else
                    {
                        IsEnableStockTakingDate = true;
                        NotifyOfPropertyChange(() => IsEnableStockTakingDate);
                    }
                    //▲===== #003
                    NotifyOfPropertyChange(() => CurrentPharmacyStockTakes);
                }
            }
        }
        //▼===== #003
        private bool _IsEnableStockTakingDate = true;
        public bool IsEnableStockTakingDate
        {
            get
            {
                return _IsEnableStockTakingDate;
            }
            set
            {
                if (_IsEnableStockTakingDate != value)
                {
                    _IsEnableStockTakingDate = value;
                    NotifyOfPropertyChange(() => IsEnableStockTakingDate);
                }
            }
        }
        //▲===== #003
        private PagedCollectionView _PCVPharmacyStockTakeDetails;
        public PagedCollectionView PCVPharmacyStockTakeDetails
        {
            get
            {
                return _PCVPharmacyStockTakeDetails;
            }
            set
            {
                if (_PCVPharmacyStockTakeDetails != value)
                {
                    _PCVPharmacyStockTakeDetails = value;
                    NotifyOfPropertyChange(() => PCVPharmacyStockTakeDetails);
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

        private ObservableCollection<PharmacyStockTakeDetails> PharmacyStockTakeDetailList;

        private PharmacyStockTakesSearchCriteria _SearchCriteria;
        public PharmacyStockTakesSearchCriteria SearchCriteria
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
            if (CurrentPharmacyStockTakes != null)
            {
                CurrentPharmacyStockTakes.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentPharmacyStockTakes.FullName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        #endregion

        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }
            bTim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_KiemKeHangThang,
                                               (int)oPharmacyEx.mBCThang_KiemKeHangThang_Tim, (int)ePermission.mView);
            bChinhSua = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_KiemKeHangThang,
                                               (int)oPharmacyEx.mBCThang_KiemKeHangThang_ChinhSua, (int)ePermission.mView);
            bXuatExcel = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_KiemKeHangThang,
                                               (int)oPharmacyEx.mBCThang_KiemKeHangThang_XuatExcel, (int)ePermission.mView);
            bIn = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBCThang_KiemKeHangThang,
                                               (int)oPharmacyEx.mBCThang_KiemKeHangThang_In, (int)ePermission.mView);
        }
        #region checking account

        private bool _bTim = true;
        private bool _bThem = true;
        private bool _bChinhSua = true;
        private bool _bXuatExcel = true;
        private bool _bIn = true;

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

        public bool bChinhSua
        {
            get
            {
                return _bChinhSua;
            }
            set
            {
                if (_bChinhSua == value)
                    return;
                _bChinhSua = value;
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

        #endregion
        #region binding visibilty

        public Button lnkDelete { get; set; }
        public Button lnkView { get; set; }

        public void lnkDelete_Loaded(object sender)
        {
            lnkDelete = sender as Button;
            lnkDelete.Visibility = Globals.convertVisibility(bChinhSua);
        }

        public void lnkView_Loaded(object sender)
        {
            lnkView = sender as Button;
            lnkView.Visibility = Globals.convertVisibility(bChinhSua);
        }
        #endregion

        #region Function Member
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }

        private void PharmacyStockTakes_Search(int PageIndex, int PageSize)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyStockTakes_Search(SearchCriteria, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                         {
                             try
                             {
                                 var results = contract.EndPharmacyStockTakes_Search(out int TotalCount, asyncResult);
                                 if (results != null && results.Count > 0)
                                 {
                                     if (results.Count > 1)
                                     {
                                         //mo pop up tim
                                         //var proAlloc = Globals.GetViewModel<IStockTakesSearch>();
                                         //proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                         //proAlloc.PharmacyStockTakeList.Clear();
                                         //proAlloc.PharmacyStockTakeList.TotalItemCount = TotalCount;
                                         //proAlloc.PharmacyStockTakeList.PageIndex = 0;
                                         //foreach (PharmacyStockTakes p in results)
                                         //{
                                         //    proAlloc.PharmacyStockTakeList.Add(p);
                                         //}
                                         //var instance = proAlloc as Conductor<object>;
                                         //Globals.ShowDialog(instance, (o) => { });

                                         Action<IStockTakesSearch> onInitDlg = (proAlloc) =>
                                              {
                                                  proAlloc.SearchCriteria = SearchCriteria.DeepCopy();
                                                  proAlloc.PharmacyStockTakeList.Clear();
                                                  proAlloc.PharmacyStockTakeList.TotalItemCount = TotalCount;
                                                  proAlloc.PharmacyStockTakeList.PageIndex = 0;
                                                  foreach (PharmacyStockTakes p in results)
                                                  {
                                                      proAlloc.PharmacyStockTakeList.Add(p);
                                                  }
                                              };
                                         GlobalsNAV.ShowDialog<IStockTakesSearch>(onInitDlg);
                                     }
                                     else
                                     {
                                         //ChangeValue(CurrentPharmacyStockTakes.StoreID,results.FirstOrDefault().StoreID);
                                         CurrentPharmacyStockTakes = results.FirstOrDefault();
                                         //load detail
                                         UnCheckPaging();
                                         PharmacyStockTakeDetails_Load(CurrentPharmacyStockTakes.PharmacyStockTakeID);
                                        

                                     }
                                 }
                                 else
                                 {
                                     Globals.ShowMessage(eHCMSResources.A0752_G1_Msg_InfoKhTimThay, eHCMSResources.G0442_G1_TBao);
                                 }
                                 GetLastPharmacyStockTakes(CurrentPharmacyStockTakes.StoreID);
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

        private void UnCheckPaging()
        {
            if (PagingChecked != null && PharmacyStockTakeDetailList != null)
            {
                PagingChecked.IsChecked = false;
                VisibilityPaging = Visibility.Collapsed;
            }
        }

        private void PharmacyStockTakeDetails_Load(long PharmacyStockTakeID)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            SearchKey = "";
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyStockTakeDetails_Load(PharmacyStockTakeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndPharmacyStockTakeDetails_Load(asyncResult);
                                //load danh sach thuoc theo hoa don 
                                PharmacyStockTakeDetailList = results.ToObservableCollection();
                                Count_DifferenceValueInventory();
                                LoadDataGrid();
                                CanGetStockTake = false;
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

        private void PharmacyStockTakeDetails_Get(long ID, DateTime StockTakingDate)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyStockTakeDetails_Get(ID, StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndPharmacyStockTakeDetails_Get(asyncResult);
                                //load danh sach thuoc theo hoa don 
                                PharmacyStockTakeDetailList = results.ToObservableCollection();
                                Count_DifferenceValueInventory();
                                LoadDataGrid();
                                //tinh tong tien 
                                CanGetStockTake = false;
                                GetLastPharmacyStockTakes(ID);
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

        private void PharmacyStockTakeDetails_Save(bool IsConfirmFinished = false)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginPharmacyStockTake_Save(CurrentPharmacyStockTakes, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndPharmacyStockTake_Save(out long ID, out string StrError, asyncResult);
                                if (string.IsNullOrEmpty(StrError))
                                {
                                    MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                    //load danh sach thuoc theo hoa don 
                                    CurrentPharmacyStockTakes.PharmacyStockTakeID = ID;
                                    GetLastPharmacyStockTakes(ID);
                                    //▼===== #004
                                    if (ID > 0)
                                    {
                                        IsEnableStockTakingDate = false;
                                    }
                                    //▲===== #004
                                  
                                    CurrentPharmacyStockTakes.IsFinished = true;
                                    
                                }
                                else
                                {
                                    MessageBox.Show(StrError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
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
                }
                catch (Exception ex)
                {
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                }
            });

            t.Start();
        }

        public void btnConfirmFinished()
        {
            //SavePharmacyStockTake();
        }
        private void SavePharmacyStockTake()
        {
            if (string.IsNullOrEmpty((CurrentPharmacyStockTakes.StockTakePeriodName).Trim()))
            {
                MessageBox.Show(eHCMSResources.Z2803_G1_ChuaNhapTenKK, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (!IsLoaded)
            {
                MessageBox.Show(eHCMSResources.Z2439_G1_DLDaThayDoiLuuLaiTT, eHCMSResources.G0442_G1_TBao);
                return;
            }
            //if (IsConfirmFinished && (CurrentPharmacyStockTakes == null || CurrentPharmacyStockTakes.PharmacyStockTakeID == 0))
            //{
            //    return;
            //}
            if (MessageBox.Show(eHCMSResources.A0197_G1_Msg_InfoYCCheckSLgThucTe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ObservableCollection<PharmacyStockTakeDetails> InvalidProduct = new ObservableCollection<PharmacyStockTakeDetails>();
                InvalidProduct = PharmacyStockTakeDetailList.Where(x => x.CaculatedQty < 0 || x.ActualQty < 0).ToObservableCollection();

                if (InvalidProduct.Count > 0)
                {
                    string strBrandName = "";
                    int limitRow = 10;
                    int count = 0;

                    foreach (PharmacyStockTakeDetails temp in InvalidProduct)
                    {
                        //strBrandName += temp.BrandName + " (" + temp.Code + ") " + Environment.NewLine;
                        if (count < limitRow)
                        {
                            strBrandName += "\t" + (count + 1).ToString() + ". " + temp.DrugCode + " - " + temp.BrandName + Environment.NewLine;
                            count++;
                        }
                        else
                        {
                            strBrandName += "\t..." + Environment.NewLine;
                            break;
                        }
                    }

                    MessageBox.Show(string.Format(eHCMSResources.Z1286_G1_I, strBrandName) + Environment.NewLine, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                else
                {
                    var temp = PharmacyStockTakeDetailList.Where(x => x.CaculatedQty >= 0 && x.ActualQty >= 0);
                    CurrentPharmacyStockTakes.StockTakeDetails = temp.ToObservableCollection();
                    if (IsReInsert)
                    {
                        PharmacyStockTakeDetails_Resave();
                        return;
                    }
                    PharmacyStockTakeDetails_Save();
                }
               
            }
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

        //▼====== #002
        private CollectionViewSource _CVS_PharmacyStockTakeDetails;
        public CollectionViewSource CVS_PharmacyStockTakeDetails
        {
            get
            {
                return _CVS_PharmacyStockTakeDetails;
            }
            set
            {
                _CVS_PharmacyStockTakeDetails = value;
                NotifyOfPropertyChange(() => CVS_PharmacyStockTakeDetails);
            }
        }

        public CollectionView CV_PharmacyStockTakeDetails { get; set; }
        private void LoadDataGrid()
        {
            if (PharmacyStockTakeDetailList == null)
            {
                PharmacyStockTakeDetailList = new ObservableCollection<PharmacyStockTakeDetails>();
            }
            CVS_PharmacyStockTakeDetails = new CollectionViewSource { Source = PharmacyStockTakeDetailList };
            CV_PharmacyStockTakeDetails = (CollectionView)CVS_PharmacyStockTakeDetails.View;
            NotifyOfPropertyChange(() => CV_PharmacyStockTakeDetails);
            btnFilter();
            IsLoaded = true;
        }

        //▲====== #002
        public void cbxPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox) != null && (sender as ComboBox).SelectedItem != null)
            {
                PCVPageSize = Convert.ToInt32(((sender as ComboBox).SelectedItem as ComboBoxItem).Content);
                long storeId = (long)(sender as ComboBox).SelectedValue;
               
            }
        }

        public void btnFilter()
        {
            //20200401 TBL: BM 0029065: Fix lỗi khi lọc theo mã hoặc tên thì không tìm thấy gì sau đó xóa đi để load lại danh sách như ban đầu thì không được
            //if (CV_PharmacyStockTakeDetails == null || CV_PharmacyStockTakeDetails.Count == 0)
            //{
            //    return;
            //}

            //PCVPharmacyStockTakeDetails.Filter = null;
            //PCVPharmacyStockTakeDetails.Filter = new Predicate<object>(DoFilter);
            //▼====== #002
            CV_PharmacyStockTakeDetails.Filter = null;
            CV_PharmacyStockTakeDetails.Filter = new Predicate<object>(DoFilter);
            //▲====== #002
        }

        //Callback method
        private bool DoFilter(object o)
        {
            //it is not a case sensitive search
            PharmacyStockTakeDetails emp = o as PharmacyStockTakeDetails;

            if (emp == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(SearchKey))
            {
                SearchKey = "";
            }

            if (SearchKey.Length == 1)
            {
                if (emp.BrandName.ToLower().StartsWith(SearchKey.Trim().ToLower()) || emp.DrugCode.ToLower() == SearchKey.Trim().ToLower())
                {
                    return true;
                }
            }
            else
            {
                if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0 || emp.DrugCode.ToLower() == SearchKey.Trim().ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        //Callback method
        //private bool DoFilter(object o)
        //{
        //    //it is not a case sensitive search
        //    PharmacyStockTakeDetails emp = o as PharmacyStockTakeDetails;
        //    if (emp != null)
        //    {
        //        if (string.IsNullOrEmpty(SearchKey))
        //        {
        //            SearchKey = "";
        //        }
        //        if (emp.BrandName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    return false;
        //}

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
            SavePharmacyStockTake();
            //SavePharmacyStockTake(true);
            //if (MessageBox.Show(eHCMSResources.A0197_G1_Msg_InfoYCCheckSLgThucTe, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //{
            //    //KMx: Sửa x.ActualQty > 0 thành x.ActualQty >= 0. Vì chấp nhận số lượng thực tế = 0 (25/03/2014 10:11)
            //    //var temp = PharmacyStockTakeDetailList.Where(x => x.CaculatedQty > 0 && x.ActualQty >= 0);
            //    //KMx: Sửa x.CaculatedQty > 0 thành x.CaculatedQty >= 0. Vì chấp nhận số lượng Lý Thuyết = 0 (29/04/2014 11:39)
            //    var temp = PharmacyStockTakeDetailList.Where(x => x.CaculatedQty >= 0 && x.ActualQty >= 0);
            //    CurrentPharmacyStockTakes.StockTakeDetails = temp.ToObservableCollection();
            //    PharmacyStockTakeDetails_Save();
            //}
        }

        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // if (CurrentPharmacyStockTakes != null && CurrentPharmacyStockTakes.PharmacyStockTakeID <= 0)
            //{
                if ((sender as ComboBox).SelectedItem != null)
                {
                    long storeId = (long)(sender as ComboBox).SelectedValue;
                    GetLastPharmacyStockTakes(storeId);

                    //if (CurrentPharmacyStockTakes != null)
                    //{
                    //    CurrentPharmacyStockTakes.PharmacyStockTakeID = 0;
                    //    CurrentPharmacyStockTakes.StockTakeNotes = "";
                    //    CurrentPharmacyStockTakes.StockTakePeriodName = "";
                    //    //CurrentPharmacyStockTakes.StockTakingDate = Globals.ServerDate.Value;
                    //    CurrentPharmacyStockTakes.StockTakeDetails = null;
                    //    PharmacyStockTakeDetailList = null;
                    //    PCVPharmacyStockTakeDetails = null;
                    //    CurrentPharmacyStockTakes.StoreID = (long)(sender as ComboBox).SelectedValue;
                    //    IsLoaded = false;
                //}
            }
           // }
            
        }
        public void StockTakingDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentPharmacyStockTakes != null && CurrentPharmacyStockTakes.PharmacyStockTakeID <= 0)
            {
                if (CurrentPharmacyStockTakes != null)
                {
                    CurrentPharmacyStockTakes.PharmacyStockTakeID = 0;
                    CurrentPharmacyStockTakes.StockTakeNotes = "";
                    CurrentPharmacyStockTakes.StockTakePeriodName = "";
                    CurrentPharmacyStockTakes.StockTakeDetails = null;
                    PharmacyStockTakeDetailList = null;
                    PCVPharmacyStockTakeDetails = null;
                    IsLoaded = false;
                    
                }
            }
        }

        public void btnSearch()
        {
            PharmacyStockTakes_Search(0, Globals.PageSize);
        }

        public void btnNew()
        {
            IsLoaded = false;
            TotalValueInventory = 0;
            DifferenceValueInventory = 0;

            CurrentPharmacyStockTakes = new PharmacyStockTakes();
            CurrentPharmacyStockTakes.PharmacyStockTakeID = 0;
            CurrentPharmacyStockTakes.StockTakeNotes = "";
            CurrentPharmacyStockTakes.StockTakePeriodName = "";
            CurrentPharmacyStockTakes.StockTakingDate = Globals.ServerDate.Value;
            CurrentPharmacyStockTakes.StoreID = 0;
            CurrentPharmacyStockTakes.StockTakeDetails = null;
            CurrentPharmacyStockTakes.StaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentPharmacyStockTakes.FullName = Globals.LoggedUserAccount.Staff.FullName;

            //▼====== #002: Không thể xét null CollectionView/Source => phải new mới rs giá trị đc.
            PharmacyStockTakeDetailList = new ObservableCollection<PharmacyStockTakeDetails>();

            CVS_PharmacyStockTakeDetails = new CollectionViewSource { Source = PharmacyStockTakeDetailList };
            CV_PharmacyStockTakeDetails = (CollectionView)CVS_PharmacyStockTakeDetails.View;
            NotifyOfPropertyChange(() => CV_PharmacyStockTakeDetails);
            //▲====== #002
            PCVPharmacyStockTakeDetails = null;
            UnCheckPaging();

            

            //SelectedProductStockTake = null;
            //ClinicDeptStockTakeDetailList_Hide = null;
            CanGetStockTake = true;
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            IsReInsert = false;
           
        }


        //private void ChangeValue(long value1, long value2)
        //{
        //    if (value1 != value2)
        //    {
        //        flag = false;
        //    }
        //    else
        //    {
        //        flag = true;
        //    }
        //}

        //private bool flag = true;

        public void btnExportExcel()
        {
            if (CurrentPharmacyStockTakes == null || CurrentPharmacyStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0327_G1_ChonKhoHoacPhKKDeXuatExcel, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = "Excel (2003) (*.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx",
                //Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() != true)
            {
                return;
            }

            ReportParameters RptParameters = new ReportParameters();
            RptParameters.ReportType = ReportType.KIEM_KE;
            RptParameters.StoreID = CurrentPharmacyStockTakes.StoreID;
            RptParameters.StockTake = new StockTake()
            {
                StockTakeID = CurrentPharmacyStockTakes.PharmacyStockTakeID,
                StockTakeDate = CurrentPharmacyStockTakes.StockTakingDate,
                StockTakeType = StockTakeType.KIEM_KE_NHA_THUOC
            };
            RptParameters.Show = "KiemKe";

            ExportToExcelGeneric.Action(RptParameters, objSFD, this);
            //Coroutine.BeginExecute(DoSaveExcel(RptParameters, objSFD));
        }

        //Import kiểm kê từ Excel
        public byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, (int)fs.Length);
                }
            }
            catch
            {
                MessageBox.Show("File Excel đang được sử dụng, vui lòng đóng trước khi nhập!");
            }
            return buffer;
        }

        public void btnImportFromExcell()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XLS files (*.xls)|*.xls|XLSX files (*.xlsx)|*.xlsx";
            openFileDialog.FilterIndex = 2;
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                byte[] file = ReadAllBytes(filePath);
                if (file != null)
                {
                    ImportFromExcell(file);
                }
                return;
            }
        }

        private void ImportFromExcell(byte[] file)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            try
            {
                using (MemoryStream ms = new MemoryStream(file))
                {
                    try
                    {
                        List<PharmacyStockTakeDetails> invoicedrug = new List<PharmacyStockTakeDetails>();
                        using (ExcelPackage package = new ExcelPackage(ms))
                        {
                            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                            ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault();
                            int startRow = workSheet.Dimension.Start.Row + 1;
                            int endRow = workSheet.Dimension.End.Row;                           
                            if (workSheet.Dimension.Columns != 14)
                            {
                                MessageBox.Show("File excel không đúng định dạng");
                                return;
                            }
                            else
                            {
                                for (int i = startRow; i <= endRow; i++)
                                {
                                    PharmacyStockTakeDetails drugs = new PharmacyStockTakeDetails();
                                    int j = 1;
                                    try
                                    {
                                        drugs.DrugID = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.DrugCode = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.BrandName = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.Packaging = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.UnitName = workSheet.Cells[i, j++].Value.ToString();
                                        drugs.InBatchNumber = workSheet.Cells[i, j++].Value.ToString();
                                        if(workSheet.Cells[i, j].Value.ToString() != "")
                                        {
                                            drugs.InExpiryDate = Convert.ToDateTime(workSheet.Cells[i, j++].Value);
                                        }
                                        else
                                        {
                                            j++;
                                        }
                                        drugs.NewestInwardPrice = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        drugs.Price = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        drugs.CaculatedQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.ActualQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        drugs.AdjustQty = Convert.ToInt32(workSheet.Cells[i, j++].Value);
                                        //drugs.FinalAmount = Convert.ToDecimal(workSheet.Cells[i, j++].Value);
                                        j++;
                                        drugs.Notes = workSheet.Cells[i, j++].Value.ToString();
                                        invoicedrug.Add(drugs);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Giá trị tại dòng " + i + ", Cột " + j + " trong file Excel không đúng định dạng!");
                                        return;
                                    }
                                }
                                PharmacyStockTakeDetailList = invoicedrug.ToObservableCollection();
                                CheckCaculatedQty();
                                if (null != PharmacyStockTakeDetailList)
                                {
                                    CurrentPharmacyStockTakes.PharmacyStockTakeID = 0;
                                }
                                if (PharmacyStockTakeDetailList == null)
                                {
                                    MessageBox.Show("Chưa có dữ liệu");
                                }
                                GetLastPharmacyStockTakes(CurrentPharmacyStockTakes.PharmacyStockTakeID);
                                bChinhSua = true;
                                IsReInsert = true; //Cho phép tính lại
                                Count_DifferenceValueInventory();
                                LoadDataGrid();
                                CanGetStockTake = false;
                                CanLockStore = false;
                                CanUnLockStore = false;
                                CanReGetStockTake = false;
                            }
                        }                        
                    }
                    catch (Exception ex)
                    {
                        Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        bChinhSua = false;
                    }
                    finally
                    {
                        this.HideBusyIndicator();
                    }
                }
            }
            catch (Exception ex)
            {
                this.HideBusyIndicator();
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
            }
        }
        //▲======

        //private IEnumerator<IResult> DoSaveExcel(ReportParameters rptParameters, SaveFileDialog objSFD)
        //{
        //    var res = new ExportToExcellAllGenericTask(rptParameters, objSFD);
        //    yield return res;
        //    //IsProcessing = false;
        //    yield break;
        //}

        #region IHandle<PharmacyCloseSearchStockTakesEvent> Members

        public void Handle(PharmacyCloseSearchStockTakesEvent message)
        {
            if (message != null && this.IsActive)
            {
                PharmacyStockTakes temp = message.SelectedPharmacyStockTakes as PharmacyStockTakes;
                //  ChangeValue(CurrentPharmacyStockTakes.StoreID,temp.StoreID);
                CurrentPharmacyStockTakes = temp;
                UnCheckPaging();
                //load detail
                PharmacyStockTakeDetails_Load(CurrentPharmacyStockTakes.PharmacyStockTakeID);
            }
        }

        #endregion

        #region printing member

        public void btnPreview()
        {
            IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
            DialogView.ID = CurrentPharmacyStockTakes.PharmacyStockTakeID;
            DialogView.eItem = ReportName.PHARMACY_PHIEUKIEMKE;
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

                        contract.BeginGetPhieuKiemKeInPdfFormat(CurrentPharmacyStockTakes.PharmacyStockTakeID, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetPhieuKiemKeInPdfFormat(asyncResult);
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
                    this.HideBusyIndicator();
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                }
            });

            t.Start();
        }

        #endregion

        #region DifferenceValueInventory Member

        private decimal _DifferenceValueInventory = 0;
        public decimal DifferenceValueInventory
        {
            get { return _DifferenceValueInventory; }
            set
            {
                _DifferenceValueInventory = value;
                NotifyOfPropertyChange(() => DifferenceValueInventory);
            }
        }

        private decimal _TotalValueInventory = 0;
        public decimal TotalValueInventory
        {
            get { return _TotalValueInventory; }
            set
            {
                _TotalValueInventory = value;
                NotifyOfPropertyChange(() => TotalValueInventory);
            }
        }

        private void Count_DifferenceValueInventory()
        {
            DifferenceValueInventory = 0;
            if (CurrentPharmacyStockTakes != null && PharmacyStockTakeDetailList != null)
            {
                DifferenceValueInventory = PharmacyStockTakeDetailList.Sum(x => x.AdjustQty * x.Price);
                TotalValueInventory = PharmacyStockTakeDetailList.Sum(x => x.ActualQty * x.Price);
            }
        }
        //▼====== #001: Ended bị lỗi => Ending
        //public void GridStockTakes_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    //KMx: Không dựa vào index, dựa vào Column Name là luôn đúng (27/02/2015 10:03).
        //    //if (e.Column.DisplayIndex == 6)
        //    if (e.Column.GetValue(FrameworkElement.NameProperty).ToString() == "colActualQty")
        //    {
        //        Count_DifferenceValueInventory();
        //    }
        //}

        public void GridStockTakes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Equals(GridStockTakes.GetColumnByName("colActualQty")))
            {
                Count_DifferenceValueInventory();
            }
        }
        //▲====== #001
        #endregion

        //KMx: Thêm AutoComplete tìm thuốc (03/09/2014 15:35).

        private bool _VisibilityName = true;
        public bool VisibilityName
        {
            get
            {
                return _VisibilityName;
            }
            set
            {
                _VisibilityName = value;
                VisibilityCode = !_VisibilityName;
                NotifyOfPropertyChange(() => VisibilityName);
                NotifyOfPropertyChange(() => VisibilityCode);
            }
        }
        public bool VisibilityCode { get; private set; } = false;

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

        TextBox tbx = null;
        public void AxTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            tbx = sender as TextBox;
        }

        string txt = "";
        public void AxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (CanGetStockTake)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0572_G1_Msg_InfoChonBatDauKK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                SelectedDrugStockTake = null;

                return;
            }
            if (txt != (sender as TextBox).Text)
            {
                txt = (sender as TextBox).Text;

                if (!string.IsNullOrEmpty(txt))
                {
                    SearchGetDrugForStockTake(txt, true);
                }
            }
        }

        AxTextBox AxQty = null;
        public void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            AxQty = sender as AxTextBox;
        }

        AutoCompleteBox au;
        public void AutoDrug_Text_Loaded(object sender, RoutedEventArgs e)
        {
            au = sender as AutoCompleteBox;
        }

        public void AutoDrug_Text_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (au.SelectedItem != null)
            {
                SelectedDrugStockTake = au.SelectedItem as PharmacyStockTakeDetails;
            }
        }

        public void AutoCompleteBox_Populating(object sender, PopulatingEventArgs e)
        {
            if (CanGetStockTake)
            {
                MessageBox.Show(string.Format("{0}!", eHCMSResources.A0572_G1_Msg_InfoChonBatDauKK), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);

                SelectedDrugStockTake = null;

                return;
            }
            if (!IsCode.GetValueOrDefault())
            {
                BrandName = e.Parameter;
                SearchGetDrugForStockTake(e.Parameter, false);
            }
        }

        private void ListDisplayAutoComplete()
        {
            if (IsCode.GetValueOrDefault())
            {
                if (SearchDrugList != null && SearchDrugList.Count > 0)
                {
                    var item = SearchDrugList.Where(x => x.DrugCode == txt);
                    if (item != null && item.Count() > 0)
                    {
                        SelectedDrugStockTake = item.ToList()[0];
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
                    au.ItemsSource = SearchDrugList;
                    au.PopulateComplete();
                }
            }
        }

        private bool? IsCode = false;
        private string BrandName;

        private ObservableCollection<PharmacyStockTakeDetails> _searchDrugList;
        public ObservableCollection<PharmacyStockTakeDetails> SearchDrugList
        {
            get { return _searchDrugList; }
            set
            {
                if (_searchDrugList != value)
                    _searchDrugList = value;
                NotifyOfPropertyChange(() => SearchDrugList);
            }
        }

        private PharmacyStockTakeDetails _selectedDrugStockTake;
        public PharmacyStockTakeDetails SelectedDrugStockTake
        {
            get { return _selectedDrugStockTake; }
            set
            {
                if (_selectedDrugStockTake != value)
                    _selectedDrugStockTake = value;
                NotifyOfPropertyChange(() => SelectedDrugStockTake);
            }
        }

        private void SearchGetDrugForStockTake(string Name, bool IsCode)
        {
            if (CurrentPharmacyStockTakes == null)
            {
                return;
            }

            if (CurrentPharmacyStockTakes.StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K0324_G1_ChonKhoDeTimHgCanKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetDrugForAutoCompleteStockTake(Name, CurrentPharmacyStockTakes.StoreID, IsCode, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetDrugForAutoCompleteStockTake(asyncResult);
                                SearchDrugList = results.ToObservableCollection();
                                ListDisplayAutoComplete();
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                //this.HideBusyIndicator();
                                //if (IsCode)
                                //{
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

        public void AddItem()
        {
            if (CurrentPharmacyStockTakes == null || PharmacyStockTakeDetailList == null || SelectedDrugStockTake == null)
            {
                return;
            }

            if (CurrentPharmacyStockTakes.PharmacyStockTakeID > 0)
            {
                MessageBox.Show(eHCMSResources.Z1289_G1_PhKKeDaLuu, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            if (PharmacyStockTakeDetailList.Where(x => x.DrugID == SelectedDrugStockTake.DrugID).Count() > 0)
            {
                MessageBox.Show(string.Format(eHCMSResources.K0019_G1_DaCoTrongDSKK, SelectedDrugStockTake.BrandName), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PharmacyStockTakeDetailList.Add(SelectedDrugStockTake);

            //KMx: Tính lại tổng tiền (03/09/2014 15:51).
            Count_DifferenceValueInventory();

            txt = "";
            SelectedDrugStockTake = null;
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

        private bool IsLoaded = false;
        public void btnGetData()
        {
            if (CurrentPharmacyStockTakes != null && CurrentPharmacyStockTakes.PharmacyStockTakeID > 0)
            {
                return;
            }
            PharmacyStockTakeDetails_Get(CurrentPharmacyStockTakes.StoreID, CurrentPharmacyStockTakes.StockTakingDate);
        }
        KeyEnabledComboBox cbxStore = new KeyEnabledComboBox();
        public void cbxStore_Loaded(object sender, RoutedEventArgs e)
        {
            cbxStore = sender as KeyEnabledComboBox;
        }
        private PharmacyStockTakes _LastPharmacyStockTakes;
        public PharmacyStockTakes LastPharmacyStockTakes
        {
            get
            {
                return _LastPharmacyStockTakes;
            }
            set
            {
                if (_LastPharmacyStockTakes != value)
                {
                    _LastPharmacyStockTakes = value;
                    NotifyOfPropertyChange(() => LastPharmacyStockTakes);
                }
            }
        }
        private bool _CanGetStockTake = true;
        public bool CanGetStockTake
        {
            get { return _CanGetStockTake; }
            set
            {
                if (_CanGetStockTake != value)
                    _CanGetStockTake = value;
                NotifyOfPropertyChange(() => CanGetStockTake);
            }
        }

        private bool _CanReGetStockTake = false;
        public bool CanReGetStockTake
        {
            get { return _CanReGetStockTake; }
            set
            {
                if (_CanReGetStockTake != value)
                    _CanReGetStockTake = value;
                NotifyOfPropertyChange(() => CanReGetStockTake);
            }
        }

        private bool _CanLockStore = false;
        public bool CanLockStore
        {
            get { return _CanLockStore; }
            set
            {
                if (_CanLockStore != value)
                    _CanLockStore = value;
                NotifyOfPropertyChange(() => CanLockStore);
            }
        }

        private bool _CanUnLockStore = false;
        public bool CanUnLockStore
        {
            get { return _CanUnLockStore; }
            set
            {
                if (_CanUnLockStore != value)
                    _CanUnLockStore = value;
                NotifyOfPropertyChange(() => CanUnLockStore);
            }
        }
        private bool _IsReInsert = false;
        public bool IsReInsert
        {
            get
            {
                return _IsReInsert;
            }
            set
            {
                if (_IsReInsert == value)
                    return;
                _IsReInsert = value;
                NotifyOfPropertyChange(() => IsReInsert);
            }
        }
        public void btnLockStore()
        {
            if (CurrentPharmacyStockTakes == null)
            {
                MessageBox.Show("Không thể khóa kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentPharmacyStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Không thể khóa kho. Hãy chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastPharmacyStockTakes
                && (CurrentPharmacyStockTakes.PharmacyStockTakeID < LastPharmacyStockTakes.PharmacyStockTakeID)
                && !LastPharmacyStockTakes.IsLocked)
            {
                MessageBox.Show("Không thể khóa kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PharmacyLockAndUnlockStore(CurrentPharmacyStockTakes.StoreID, true);
            GetLastPharmacyStockTakes(CurrentPharmacyStockTakes.StoreID);
        }
        public void btnUnlockStore()
        {
            if (CurrentPharmacyStockTakes == null)
            {
                MessageBox.Show("Không thể mở kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentPharmacyStockTakes.StoreID <= 0)
            {
                MessageBox.Show("Không thể mở kho. Hãy chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastPharmacyStockTakes
                && (CurrentPharmacyStockTakes.PharmacyStockTakeID < LastPharmacyStockTakes.PharmacyStockTakeID)
                && LastPharmacyStockTakes.IsLocked)
            {
                MessageBox.Show("Không thể mở kho. Vui lòng chọn thời điểm tính tồn gần nhất.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            PharmacyLockAndUnlockStore(CurrentPharmacyStockTakes.StoreID, false);
            GetLastPharmacyStockTakes(CurrentPharmacyStockTakes.StoreID);
        }
        public void btnGetStockTake()
        {
            //--▼-- 06/02/2021 DatTB
            DateTime _FirstStockTakingDate = Convert.ToDateTime(Globals.ServerConfigSection.CommonItems.NgayNhapLaiTDK.ToString());
            if (null == _FirstStockTakingDate)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Không có thông tin cấu hình ngày đầu tiên thực hiện tính tồn.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (CurrentPharmacyStockTakes.StockTakingDate.Year.CompareTo(Globals.GetCurServerDateTime().Year) != 0)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Ngày thực hiện không hợp lệ.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▲-- 06/02/2021 DatTB
            if (CurrentPharmacyStockTakes == null)
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Vui lòng chọn kho.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (String.IsNullOrEmpty(CurrentPharmacyStockTakes.StockTakePeriodName))
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Vui lòng nhập tên phiếu.", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▼-- 06/02/2021 DatTB
            if ((null != CurrentPharmacyStockTakes.StockTakingDate
                    && (CurrentPharmacyStockTakes.StockTakingDate.DayOfYear >= Globals.GetCurServerDateTime().DayOfYear)
                    && (CurrentPharmacyStockTakes.StockTakingDate.DayOfYear != _FirstStockTakingDate.DayOfYear))
                || (null != LastPharmacyStockTakes && null != LastPharmacyStockTakes.StockTakingDate
                    && (DateTime.Compare(CurrentPharmacyStockTakes.StockTakingDate, LastPharmacyStockTakes.StockTakingDate) <= 0)))
            {
                MessageBox.Show("Không thể tính tồn đầu kỳ. Ngày thực hiện phải trước ngày hiện tại và lớn hơn ngày tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            //--▲-- 06/02/2021 DatTB
            if (CurrentPharmacyStockTakes.PharmacyStockTakeID > 0)
            {
                MessageBox.Show(eHCMSResources.A0600_G1_Msg_InfoLoadKK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (cbxStore == null || cbxStore.SelectedItem == null || ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID <= 0)
            {
                MessageBox.Show(eHCMSResources.K1973_G1_ChonKho, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }

            CurrentPharmacyStockTakes.StoreID = ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID;
            PharmacyStockTakeDetails_Get(CurrentPharmacyStockTakes.StoreID,CurrentPharmacyStockTakes.StockTakingDate);
        }
        public void btnReGetStockTake()
        {
            if (CurrentPharmacyStockTakes == null)
            {
                MessageBox.Show("Không thể tính lại tồn đầu. Vui lòng chọn kho và chọn thời điểm tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            if (null != LastPharmacyStockTakes
                && !(LastPharmacyStockTakes.PharmacyStockTakeID == CurrentPharmacyStockTakes.PharmacyStockTakeID
                    && !LastPharmacyStockTakes.IsLocked))
            {
                MessageBox.Show("Không thể tính lại tồn đầu. Vui lòng mở kho và chọn thời điểm tính tồn gần nhất.",
                    eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return;
            }
            CurrentPharmacyStockTakes.StoreID = ((RefStorageWarehouseLocation)cbxStore.SelectedItem).StoreID;
            ReGetClinicDeptStockTakeDetails(CurrentPharmacyStockTakes.StoreID);
        }
        private void PharmacyLockAndUnlockStore(long StoreID, bool IsLock)
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyLockAndUnlockStore(StoreID, IsLock, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            bool results = contract.EndPharmacyLockAndUnlockStore(asyncResult);

                            if (results)
                            {
                                if (IsLock)
                                {
                                    MessageBox.Show(eHCMSResources.A0620_G1_Msg_InfoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0819_G1_Msg_InfoMoKhoaKhoOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                GetLastPharmacyStockTakes(StoreID);
                            }
                            else
                            {
                                if (IsLock)
                                {
                                    MessageBox.Show("Khóa kho thất bại", eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
                                else
                                {
                                    MessageBox.Show(eHCMSResources.A0820_G1_Msg_InfoMoKhoaKhoFail, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                }
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

            });

            t.Start();
        }
        private void GetLastPharmacyStockTakes(long storeId)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginGetLastPharmacyStockTakes(storeId, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndGetLastPharmacyStockTakes(asyncResult);
                                LastPharmacyStockTakes = null;
                                if (null != results)
                                {
                                    LastPharmacyStockTakes = results;
                                    
                                }
                                UpdateGroupBtn1StatusAfterChangedStore(); 
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
                    this.HideBusyIndicator();
                    MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                }
            });

            t.Start();
        }
        //private void PharmacyStockTakeDetails_Get(long ID)
        //{
        //    //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    //isLoadingDetail = true;
        //    this.ShowBusyIndicator();
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacyInwardDrugServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginPharmacyStockTakeDetails_Get(ID, CurrentPharmacyStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var results = contract.EndPharmacyStockTakeDetails_Get(asyncResult);

                          
        //                    PharmacyStockTakeDetailList = results.ToObservableCollection();
        //                    CheckCaculatedQty();
                            

        //                    LoadDataGrid();
        //                    CanGetStockTake = false;
        //                    GetLastPharmacyStockTakes(ID);
        //                    //tinh tong tien 
        //                }
        //                catch (Exception ex)
        //                {
        //                    //isLoadingDetail = false;
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //isLoadingDetail = false;
        //                    //Globals.IsBusy = false;
        //                    this.HideBusyIndicator();
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}
        private void ReGetClinicDeptStockTakeDetails(long storeId)
        {
            this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginReGetPharmacyStockTakeDetails(storeId,  CurrentPharmacyStockTakes.StockTakingDate, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndReGetPharmacyStockTakeDetails(asyncResult);
                          
                            PharmacyStockTakeDetailList = results.ToObservableCollection();
                            CheckCaculatedQty();
                            if (null != results)
                            {
                                CurrentPharmacyStockTakes.PharmacyStockTakeID = 0;
                            }
                            GetLastPharmacyStockTakes(CurrentPharmacyStockTakes.PharmacyStockTakeID);
                            LoadDataGrid();
                            CanGetStockTake = false;
                            bChinhSua = true;
                            IsReInsert = true;
                            
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
        public void UpdateGroupBtn1StatusAfterChangedStore()
        {
            CanReGetStockTake = false;
            CanLockStore = false;
            CanUnLockStore = false;
            if (null == CurrentPharmacyStockTakes
                || null == LastPharmacyStockTakes
                || CurrentPharmacyStockTakes.PharmacyStockTakeID != LastPharmacyStockTakes.PharmacyStockTakeID)
            {
                return;
            }
            if (CurrentPharmacyStockTakes.PharmacyStockTakeID == LastPharmacyStockTakes.PharmacyStockTakeID)
            {
                CanReGetStockTake = !LastPharmacyStockTakes.IsLocked;
            }
            if (LastPharmacyStockTakes.IsLocked)
            {
                CanUnLockStore = true;
            }
            else
            {
                CanLockStore = true;
            }
        }
        
        public bool CheckCaculatedQty()
        {
            if (PharmacyStockTakeDetailList == null || PharmacyStockTakeDetailList.Count <= 0)
            {
                return false;
            }

            string error = "";

            int limitRow = 10;

            int count = 0;
            foreach (PharmacyStockTakeDetails item in PharmacyStockTakeDetailList)
            {
                if (item.CaculatedQty < 0)
                {
                    if (count < limitRow)
                    {
                        error += "\t" + (count + 1).ToString() + ". " + item.DrugCode + " - " + item.BrandName + Environment.NewLine;
                        count++;
                    }
                    else
                    {
                        error += "\t..." + Environment.NewLine;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(string.Format(eHCMSResources.Z1285_G1_I, error), eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                return false;
            }

            return true;
        }
        private void PharmacyStockTakeDetails_Resave()
        {
            this.ShowBusyIndicator();
           

            CurrentPharmacyStockTakes.StockTakingDate = CurrentPharmacyStockTakes.StockTakingDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            //CheckFilteringOutRowWithAllZeroValues();
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            //isLoadingFullOperator = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    //CurrentClinicDeptStockTakes.ClinicDeptStockTakeID = 0;
                    contract.BeginPharmacyStockTake_Resave(CurrentPharmacyStockTakes, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID = 0;
                            string StrError;
                            var results = contract.EndPharmacyStockTake_Resave(out ID, out StrError, asyncResult);
                            if (string.IsNullOrEmpty(StrError))
                            {
                                MessageBox.Show(eHCMSResources.A0756_G1_Msg_InfoKKOK, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                                //load danh sach thuoc theo hoa don 
                                CurrentPharmacyStockTakes.PharmacyStockTakeID = ID;
                                IsReInsert = false;
                                GetLastPharmacyStockTakes(CurrentPharmacyStockTakes.StoreID);
                            }
                            else
                            {
                                MessageBox.Show(StrError, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                            }
                        }
                        catch (Exception ex)
                        {
                            //isLoadingFullOperator = false;
                            MessageBox.Show(ex.Message, eHCMSResources.T0074_G1_I, MessageBoxButton.OK);
                        }
                        finally
                        {
                            //isLoadingFullOperator = false;
                            //Globals.IsBusy = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }
    }
}