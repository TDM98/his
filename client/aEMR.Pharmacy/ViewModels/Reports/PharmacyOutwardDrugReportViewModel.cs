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
using aEMR.CommonTasks;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Collections.Generic;
using System.Text;
using eHCMSLanguage;
using aEMR.Common.PagedCollectionView;
using aEMR.Controls;
using aEMR.Common.BaseModel;
/*
* 20181114 #001 TTM: BM 0005259: Chuyển đổi CellEditEnded => CellEditEnding.
*/
namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyOutwardDrugReport)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyOutwardDrugReportViewModel : ViewModelBase, IPharmacyOutwardDrugReport, IHandle<PharmacyCloseSearchOutReportDrugEvent>
    {
        public string TitleForm { get; set; }

        #region Indicator Member

        private bool _isLoadingLookup = false;
        public bool isLoadingLookup
        {
            get { return _isLoadingLookup; }
            set
            {
                if (_isLoadingLookup != value)
                {
                    _isLoadingLookup = value;
                    NotifyOfPropertyChange(() => isLoadingLookup);
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
            get { return (isLoadingLookup || isLoadingFullOperator || isLoadingGetID || isLoadingSearch || isLoadingDetail); }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyOutwardDrugReportViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            authorization();
            //Globals.EventAggregator.Subscribe(this);
            CurrentPharmacyOutwardDrugReport = new PharmacyOutwardDrugReport();
            CurrentPharmacyOutwardDrugReport.RepStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentPharmacyOutwardDrugReport.FullName = Globals.LoggedUserAccount.Staff.FullName;

            SearchCriteria = new SearchOutwardReport();

            Coroutine.BeginExecute(GetV_PharmacyOutRepType());
            GetStaffLogin();
            UnCheckPaging();

            PharmacyOutwardDrugReportList = new PagedSortableCollectionView<PharmacyOutwardDrugReport>();
            PharmacyOutwardDrugReportList.OnRefresh += PharmacyOutwardDrugReportList_OnRefresh;
            //KMx: Nhà thuốc muốn hiện tất cả báo cáo nộp tiền trong 1 trang, để cuối tháng tick vào tất cả báo cáo cộng tiền lại 1 lần.
            //Hiện tại gán mặc định là 10000 dòng/ trang.
            //PharmacyOutwardDrugReportList.PageSize = Globals.PageSize;
            PharmacyOutwardDrugReportList.PageSize = 10000;
            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
        }

        void PharmacyOutwardDrugReportList_OnRefresh(object sender, RefreshEventArgs e)
        {
            PharmacyOutwardDrugReport_Search(PharmacyOutwardDrugReportList.PageIndex, PharmacyOutwardDrugReportList.PageSize);
        }


        public void authorization()
        {
            if (!Globals.isAccountCheck)
            {
                return;
            }

            mBaoCaoNopTienHangNgay_PhieuMoi = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoNopTienHangNgay,
                                               (int)oPharmacyEx.mBaoCaoNopTienHangNgay_PhieuMoi, (int)ePermission.mView);
            mBaoCaoNopTienHangNgay_Tim = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoNopTienHangNgay,
                                               (int)oPharmacyEx.mBaoCaoNopTienHangNgay_Tim, (int)ePermission.mView);
            mBaoCaoNopTienHangNgay_Them = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoNopTienHangNgay,
                                               (int)oPharmacyEx.mBaoCaoNopTienHangNgay_Them, (int)ePermission.mView);
            mBaoCaoNopTienHangNgay_In = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoNopTienHangNgay,
                                               (int)oPharmacyEx.mBaoCaoNopTienHangNgay_In, (int)ePermission.mView);
            mBaoCaoNopTienHangNgay_InThongKeChiTiet = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mPharmacy
                                               , (int)ePharmacy.mBaoCaoNopTienHangNgay,
                                               (int)oPharmacyEx.mBaoCaoNopTienHangNgay_InThongKeChiTiet, (int)ePermission.mView);

        }
        #region checking account

        private bool _mBaoCaoNopTienHangNgay_PhieuMoi = true;
        private bool _mBaoCaoNopTienHangNgay_Tim = true;
        private bool _mBaoCaoNopTienHangNgay_Them = true;
        private bool _mBaoCaoNopTienHangNgay_In = true;
        private bool _mBaoCaoNopTienHangNgay_InThongKeChiTiet = true;

        public bool mBaoCaoNopTienHangNgay_PhieuMoi
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_PhieuMoi;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_PhieuMoi == value)
                    return;
                _mBaoCaoNopTienHangNgay_PhieuMoi = value;
                NotifyOfPropertyChange(() => mBaoCaoNopTienHangNgay_PhieuMoi);
            }
        }

        public bool mBaoCaoNopTienHangNgay_Tim
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_Tim;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_Tim == value)
                    return;
                _mBaoCaoNopTienHangNgay_Tim = value;
                NotifyOfPropertyChange(() => mBaoCaoNopTienHangNgay_Tim);
            }
        }

        public bool mBaoCaoNopTienHangNgay_Them
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_Them;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_Them == value)
                    return;
                _mBaoCaoNopTienHangNgay_Them = value;
                NotifyOfPropertyChange(() => mBaoCaoNopTienHangNgay_Them);
            }
        }

        public bool mBaoCaoNopTienHangNgay_In
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_In;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_In == value)
                    return;
                _mBaoCaoNopTienHangNgay_In = value;
                NotifyOfPropertyChange(() => mBaoCaoNopTienHangNgay_In);
            }
        }

        public bool mBaoCaoNopTienHangNgay_InThongKeChiTiet
        {
            get
            {
                return _mBaoCaoNopTienHangNgay_InThongKeChiTiet;
            }
            set
            {
                if (_mBaoCaoNopTienHangNgay_InThongKeChiTiet == value)
                    return;
                _mBaoCaoNopTienHangNgay_InThongKeChiTiet = value;
                NotifyOfPropertyChange(() => mBaoCaoNopTienHangNgay_InThongKeChiTiet);
            }
        }


        #endregion

        #region Propeties Member


        private SearchOutwardReport _searchCriteria;
        public SearchOutwardReport SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
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
        //IsInsurance
        private bool _IsInsurance;
        public bool IsInsurance
        {
            get { return _IsInsurance; }
            set
            {
                _IsInsurance = value;
                NotifyOfPropertyChange(() => IsInsurance);
            }
        }

        private PharmacyOutwardDrugReport _CurrentPharmacyOutwardDrugReport;
        public PharmacyOutwardDrugReport CurrentPharmacyOutwardDrugReport
        {
            get
            {
                return _CurrentPharmacyOutwardDrugReport;
            }
            set
            {
                if (_CurrentPharmacyOutwardDrugReport != value)
                {
                    _CurrentPharmacyOutwardDrugReport = value;
                    NotifyOfPropertyChange(() => CurrentPharmacyOutwardDrugReport);
                }
            }
        }

        private ObservableCollection<Lookup> _LookupList;
        public ObservableCollection<Lookup> LookupList
        {
            get
            {
                return _LookupList;
            }
            set
            {
                if (_LookupList != value)
                {
                    _LookupList = value;
                    NotifyOfPropertyChange(() => LookupList);
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

        private Staff GetStaffLogin()
        {
            if (CurrentPharmacyOutwardDrugReport != null)
            {
                CurrentPharmacyOutwardDrugReport.RepStaffID = Globals.LoggedUserAccount.Staff.StaffID;
                CurrentPharmacyOutwardDrugReport.FullName = Globals.LoggedUserAccount.Staff.FullName;
            }
            return Globals.LoggedUserAccount.Staff;
        }

        #endregion

        #region Function Member

        private IEnumerator<IResult> GetV_PharmacyOutRepType()
        {
            isLoadingLookup = true;
            var paymentTypeTask = new LoadLookupListTask(LookupValues.V_PharmacyOutRepType, false, false);
            yield return paymentTypeTask;
            LookupList = paymentTypeTask.LookupList;
            SetDefaultV_PharmacyOutRepType();
            isLoadingLookup = false;
            yield break;
        }


        private void SetDefaultV_PharmacyOutRepType()
        {
            if (CurrentPharmacyOutwardDrugReport != null && LookupList != null)
            {
                CurrentPharmacyOutwardDrugReport.V_PharmacyOutRepType = (long)AllLookupValues.V_PharmacyOutRepType.TUNG_NGUOI;
                //CurrentPharmacyOutwardDrugReport.V_PharmacyOutRepType = (long)AllLookupValues.V_PharmacyOutRepType.ALL;
            }
        }

        private void PharmacyOutwardDrugReport_Search(int PageIndex, int PageSize)
        {
            //isLoadingSearch = true;
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyOutwardDrugReport_Search(SearchCriteria, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(), PageIndex, PageSize,
                        Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int TotalCount = 0;
                            var results = contract.EndPharmacyOutwardDrugReport_Search(out TotalCount, asyncResult);
                            if (results != null && results.Count > 0)
                            {
                                PharmacyOutwardDrugReportList.Clear();
                                PharmacyOutwardDrugReportList.TotalItemCount = TotalCount;

                                foreach (PharmacyOutwardDrugReport p in results)
                                {
                                    PharmacyOutwardDrugReportList.Add(p);
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
                            //isLoadingSearch = false;
                            this.HideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void UnCheckPaging()
        {
            if (PagingChecked != null && CurrentPharmacyOutwardDrugReport != null && CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails != null)
            {
                PagingChecked.IsChecked = false;
                VisibilityPaging = Visibility.Collapsed;
            }
        }

        //▼====== #001: TBR: Đây là màn hình báo cáo thì không có lý do gì phải sửa chữa trên Grid, comment lại.
        //public void GridStockTakes_CellEditEnded(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    CountMoney();
        //}
        //public void GridStockTakes_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        //{
        //    CountMoney();
        //}
        //▲====== #001
        private void CountMoney()
        {
            if (CurrentPharmacyOutwardDrugReport != null && CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails != null)
            {
                InitValue();
                for (int i = 0; i < CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails.Count; i++)
                {
                    if (CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails[i].IsChecked)
                    {
                        CurrentPharmacyOutwardDrugReport.OutAmount += CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails[i].OutAmount;
                        CurrentPharmacyOutwardDrugReport.OutHIRebate += CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails[i].OutHIRebate;
                        CurrentPharmacyOutwardDrugReport.AmountCoPay += CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails[i].AmountCoPay;
                    }
                }
                CurrentPharmacyOutwardDrugReport.ThucThu = CurrentPharmacyOutwardDrugReport.OutAmount - CurrentPharmacyOutwardDrugReport.OutHIRebate - CurrentPharmacyOutwardDrugReport.AmountCoPay;
                CurrentPharmacyOutwardDrugReport.ThucNop = CurrentPharmacyOutwardDrugReport.OutAmount - CurrentPharmacyOutwardDrugReport.OutHIRebate;
            }
        }

        private void InitValue()
        {
            CurrentPharmacyOutwardDrugReport.OutHIRebate = 0;
            CurrentPharmacyOutwardDrugReport.OutAmount = 0;
            CurrentPharmacyOutwardDrugReport.PatientPayment = 0;
            CurrentPharmacyOutwardDrugReport.AmountCoPay = 0;
            CurrentPharmacyOutwardDrugReport.ThucThu = 0;
            CurrentPharmacyOutwardDrugReport.ThucNop = 0;
        }
        private int _CountTotalDetails = 0;
        public int CountTotalDetails
        {
            get
            {
                return _CountTotalDetails;
            }
            set
            {
                if (_CountTotalDetails != value)
                {
                    _CountTotalDetails = value;
                    NotifyOfPropertyChange(() => CountTotalDetails);
                }
            }
        }
        private void PharmacyOutwardDrugReportDetail_GetReport()
        {
            isLoadingDetail = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyOutwardDrugReportDetail_GetReport(CurrentPharmacyOutwardDrugReport, Globals.LoggedUserAccount.StaffID.GetValueOrDefault(),
                        Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPharmacyOutwardDrugReportDetail_GetReport(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails = results.ToObservableCollection();
                            LoadDataGrid();
                            //goi ham tinh tien o day ne
                            CountMoney();
                            //▼====== 20190113 TTM: Thể hiện tổng số hóa đơn trong ngày.
                            CountTotalDetails = results.Count();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingDetail = false;
                            // Globals.IsBusy = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyOutwardDrugReportDetail_Get(long ID)
        {
            isLoadingGetID = true;
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyOutwardDrugReportDetail_GetID(ID, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndPharmacyOutwardDrugReportDetail_GetID(asyncResult);
                            //load danh sach thuoc theo hoa don 
                            if (CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails == null)
                            {
                                CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails = new ObservableCollection<PharmacyOutwardDrugReportDetail>();
                            }
                            CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails = results.ToObservableCollection();
                            LoadDataGrid();
                            CountMoney();
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingGetID = false;
                            this.DlgHideBusyIndicator();
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void PharmacyOutwardDrugReport_Save()
        {
            isLoadingFullOperator = true;
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyOutwardDrugReport_Save(CurrentPharmacyOutwardDrugReport, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            long ID = 0;
                            var results = contract.EndPharmacyOutwardDrugReport_Save(out ID, asyncResult);
                            //load danh sach thuoc theo hoa don 
                            CurrentPharmacyOutwardDrugReport.PharmacyOutRepID = ID;
                            NotifyOfPropertyChange(() => CurrentPharmacyOutwardDrugReport.CanSave);

                            if (CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails != null)
                            {
                                var items = CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails.Where(x => x.IsChecked == true);
                                CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails = items.ToObservableCollection();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                        }
                        finally
                        {
                            isLoadingFullOperator = false;
                            this.DlgHideBusyIndicator();
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
            if (CurrentPharmacyOutwardDrugReport != null && CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails != null)
            {
                PCVOutwardDrugDetails = null;
                PCVOutwardDrugDetails = new PagedCollectionView(CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails);
                NotifyOfPropertyChange(() => PCVOutwardDrugDetails);
                btnFilter();
                if (PagingChecked != null && PagingChecked.IsChecked.GetValueOrDefault() && pagerStockTakes != null)
                {
                    //pagerStockTakes.Source = GridStockTakes.ItemsSource;
                }
            }
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
            PharmacyOutwardDrugReportDetail emp = o as PharmacyOutwardDrugReportDetail;

            if (emp != null)
            {
                if (string.IsNullOrEmpty(SearchKey))
                {
                    SearchKey = "";
                }
                if (emp.CustomerName.ToLower().IndexOf(SearchKey.Trim().ToLower()) >= 0)
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

        public void btnGetDS()
        {
            if (CurrentPharmacyOutwardDrugReport.RepDateFrom == null || CurrentPharmacyOutwardDrugReport.RepDateTo == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1659_G1_NhapTuNgDenNgLamBC, eHCMSResources.G0442_G1_TBao);
            }
            else if (CurrentPharmacyOutwardDrugReport.RepDateFrom.Date > CurrentPharmacyOutwardDrugReport.RepDateTo.Date)
            {
                Globals.ShowMessage(eHCMSResources.K0229_G1_TuNgKhongLonHonDenNg, eHCMSResources.G0442_G1_TBao);
            }
            else
            {
                PharmacyOutwardDrugReportDetail_GetReport();
                //get ds  cac hoa don can bao cao
            }
        }

        public void btnSave()
        {
            if (CurrentPharmacyOutwardDrugReport.RepDateFrom == null || CurrentPharmacyOutwardDrugReport.RepDateTo == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1659_G1_NhapTuNgDenNgLamBC, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentPharmacyOutwardDrugReport.RepDateFrom.Date > CurrentPharmacyOutwardDrugReport.RepDateTo.Date)
            {
                Globals.ShowMessage(eHCMSResources.K0229_G1_TuNgKhongLonHonDenNg, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentPharmacyOutwardDrugReport.ReportDate == null)
            {
                Globals.ShowMessage(eHCMSResources.Z1660_G1_NhapNgLamBC, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails == null || CurrentPharmacyOutwardDrugReport.PharmacyOutwardDrugReportDetails.Count == 0)
            {
                Globals.ShowMessage(eHCMSResources.Z1661_G1_KgCoDuLieuBC, eHCMSResources.G0442_G1_TBao);
                return;
            }
            if (MessageBox.Show(eHCMSResources.K0456_G1_XemKyTruocKhiLamBC, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //Gọi hàm save o đây nè!
                PharmacyOutwardDrugReport_Save();
            }
        }

        private PagedSortableCollectionView<PharmacyOutwardDrugReport> _PharmacyOutwardDrugReportList;
        public PagedSortableCollectionView<PharmacyOutwardDrugReport> PharmacyOutwardDrugReportList
        {
            get
            {
                return _PharmacyOutwardDrugReportList;
            }
            set
            {
                if (_PharmacyOutwardDrugReportList != value)
                {
                    _PharmacyOutwardDrugReportList = value;
                    NotifyOfPropertyChange(() => PharmacyOutwardDrugReportList);
                }
            }
        }

        public void btnSearch()
        {
            PharmacyOutwardDrugReportList.PageIndex = 0;
            PharmacyOutwardDrugReport_Search(PharmacyOutwardDrugReportList.PageIndex, PharmacyOutwardDrugReportList.PageSize);
        }


        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new PharmacyCloseSearchOutReportDrugEvent { SelectedPharmacyOutwardDrugReport = e.Value });
        }
        public void btnNew()
        {
            CurrentPharmacyOutwardDrugReport = null;
            CurrentPharmacyOutwardDrugReport = new PharmacyOutwardDrugReport();
            CurrentPharmacyOutwardDrugReport.RepStaffID = Globals.LoggedUserAccount.Staff.StaffID;
            CurrentPharmacyOutwardDrugReport.FullName = Globals.LoggedUserAccount.Staff.FullName;
            SetDefaultV_PharmacyOutRepType();
            //them 1 so field nua
            PCVOutwardDrugDetails = null;
            UnCheckPaging();
        }

        #region printing member


        private string GetReportSelected()
        {
            StringBuilder sb = new StringBuilder();
           
          
            if (PharmacyOutwardDrugReportList != null)
            {
                var items = PharmacyOutwardDrugReportList.Where(x=>x.Checked==true);
                if (items != null && items.Count() > 0)
                {
                    sb.Append("<Root>");
                    foreach (var details in items)
                    {
                        sb.Append("<IDList>");
                        sb.AppendFormat("<ID>{0}</ID>", details.PharmacyOutRepID);
                        sb.Append("</IDList>");
                    }
                    sb.Append("</Root>");
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
           
            return sb.ToString();
        }

        private string values = string.Empty;
        private bool CheckPreview()
        {
            values = GetReportSelected();
            if (string.IsNullOrEmpty(values))
            {
                MessageBox.Show(eHCMSResources.K0383_G1_ChonPh);
                return false;
            }
            else
            {
                return true;
            }
        }
        public void btnPreviewNopTien()
        {
            if (CheckPreview())
            {
                IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
                DialogView.ListID = values;
                DialogView.eItem = ReportName.PHARMACY_BCHANGNGAY_NOPTIEN;
                GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }

        public void btnPreviewNopTienChiTiet()
        {
            if (CheckPreview())
            {
                IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
                DialogView.ListID = values;
                DialogView.eItem = ReportName.PHARMACY_BCHANGNGAY_NOPTIENCHITIET;
                GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }

        public void btnPrintBangKe()
        {
            if (CheckPreview())
            {
                IReportDocumentPreview DialogView = Globals.GetViewModel<IReportDocumentPreview>();
                DialogView.ListID = values;
                DialogView.IsInsurance = IsInsurance;
                DialogView.eItem = ReportName.PHARMACY_BCHANGNGAY_PHATTHUOC;
                GlobalsNAV.ShowDialog_V3(DialogView, null, null, false, true, Globals.GetDefaultDialogViewSize());
            }
        }

        #endregion

        private bool _AllChecked;
        public bool AllChecked
        {
            get
            {
                return _AllChecked;
            }
            set
            {
                if (_AllChecked != value)
                {
                    _AllChecked = value;
                    NotifyOfPropertyChange(() => AllChecked);
                    if (_AllChecked)
                    {
                        AllCheckedfc();
                    }
                    else
                    {
                        UnCheckedfc();
                    }
                }
            }
        }

        private void AllCheckedfc()
        {
            if (PharmacyOutwardDrugReportList != null && PharmacyOutwardDrugReportList.Count > 0)
            {
                for (int i = 0; i < PharmacyOutwardDrugReportList.Count; i++)
                {
                    PharmacyOutwardDrugReportList[i].Checked = true;
                }
            }
        }
        private void UnCheckedfc()
        {
            if (PharmacyOutwardDrugReportList != null && PharmacyOutwardDrugReportList.Count > 0)
            {
                for (int i = 0; i < PharmacyOutwardDrugReportList.Count; i++)
                {
                    PharmacyOutwardDrugReportList[i].Checked = false;
                }
            }
        }


        #region IHandle<PharmacyCloseSearchOutReportDrugEvent> Members
        public void Handle(PharmacyCloseSearchOutReportDrugEvent message)
        {
            if (this.IsActive && message != null)
            {
                CurrentPharmacyOutwardDrugReport = message.SelectedPharmacyOutwardDrugReport as PharmacyOutwardDrugReport;
                PharmacyOutwardDrugReportDetail_Get(CurrentPharmacyOutwardDrugReport.PharmacyOutRepID);
            }
        }
        #endregion

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
        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            this.ShowBusyIndicator();
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            if (StoreCbx != null && CurrentPharmacyOutwardDrugReport != null)
            {
                CurrentPharmacyOutwardDrugReport.StoreID = StoreCbx.FirstOrDefault().StoreID;
                SearchCriteria.StoreID = CurrentPharmacyOutwardDrugReport.StoreID;
            }
            this.HideBusyIndicator();
            yield break;
        }
        public void cboStore_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbx = sender as ComboBox;
            if (cbx.SelectedItem != null)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.StoreID = CurrentPharmacyOutwardDrugReport.StoreID;
                }
                btnGetDS();
                if (PharmacyOutwardDrugReportList != null)
                {
                    PharmacyOutwardDrugReportList.Clear();
                }
            }
        }
    }
}