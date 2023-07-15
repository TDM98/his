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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Media;
using aEMR.Common.BaseModel;
using eHCMSLanguage;


namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(ICollectionDrugSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CollectionDrugSearchViewModel : ViewModelBase, ICollectionDrugSearch
    {
        #region Indicator Member

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        #endregion


        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CollectionDrugSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            LoadOutStatus();

            OutwardInfoList = new PagedSortableCollectionView<OutwardDrugInvoice>();
            OutwardInfoList.OnRefresh += OutwardInfoList_OnRefresh;
            OutwardInfoList.PageSize = 20;
        }

        void OutwardInfoList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchOutwardInfo(OutwardInfoList.PageIndex, OutwardInfoList.PageSize);
        }

      
        #region Properties member

        private ObservableCollection<Lookup> _OutStatus;
        public ObservableCollection<Lookup> OutStatus
        {
            get
            {
                return _OutStatus;
            }
            set
            {
                if (_OutStatus != value)
                {
                    _OutStatus = value;
                    NotifyOfPropertyChange(() => OutStatus);
                }
            }
        }

        private SearchOutwardInfo _searchCriteria;
        public SearchOutwardInfo SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<OutwardDrugInvoice> _OutwardInfoList;
        public PagedSortableCollectionView<OutwardDrugInvoice> OutwardInfoList
        {
            get
            {
                return _OutwardInfoList;
            }
            set
            {
                if (_OutwardInfoList != value)
                {
                    _OutwardInfoList = value;
                    NotifyOfPropertyChange(() => OutwardInfoList);
                }
            }
        }

        private string _pageTitle;
        public string pageTitle
        {
            get { return _pageTitle; }
            set
            {
                if (_pageTitle != value)
                    _pageTitle = value;
                NotifyOfPropertyChange(() => pageTitle);
            }
        }

        private bool? _bFlagStoreHI;
        public bool? bFlagStoreHI
        {
            get
            {
                return _bFlagStoreHI;
            }
            set
            {
                if (_bFlagStoreHI != value)
                {
                    _bFlagStoreHI = value;
                    NotifyOfPropertyChange(() => bFlagStoreHI);
                }
            }
        }

        private bool _bFlagPaidTime;
        public bool bFlagPaidTime
        {
            get
            {
                return _bFlagPaidTime;
            }
            set
            {
                if (_bFlagPaidTime != value)
                {
                    _bFlagPaidTime = value;
                    NotifyOfPropertyChange(() => bFlagPaidTime);
                }
            }
        }
        #endregion

        private const string ALLITEMS = "[All]";
        private void LoadOutStatus()
        {
            IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.V_OutDrugInvStatus, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetAllLookupValuesByType(asyncResult);
                            OutStatus = results.ToObservableCollection();
                            Lookup item = new Lookup();
                            item.LookupID = 0;
                            item.ObjectValue = ALLITEMS;
                            OutStatus.Insert(0, item);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        public void SearchOutwardInfo(int PageIndex, int PageSize)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            int Total = 0;
            //IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchCriteria, PageIndex, PageSize, true, bFlagStoreHI, bFlagPaidTime, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetOutWardDrugInvoiceSearchAllByStatus(out Total, asyncResult);
                            if (results != null)
                            {
                                OutwardInfoList.Clear();
                                OutwardInfoList.TotalItemCount = Total;
                                foreach (OutwardDrugInvoice p in results)
                                {
                                    OutwardInfoList.Add(p);
                                }
                                NotifyOfPropertyChange(() => OutwardInfoList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            _logger.Info(ex.Message);
                        }
                        finally
                        {
                            this.DlgHideBusyIndicator();
                            //IsLoading = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch();
            }
        }
        public void Search_KeyUp_MaPhieu(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.OutInvID=(sender as TextBox).Text;
                }
                btnSearch();
            }
        }
        public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.HICardCode = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
        public void btnSearch()
        {
            OutwardInfoList.PageIndex = 0;
            SearchOutwardInfo(OutwardInfoList.PageIndex, OutwardInfoList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            //phat ra su kien
            Globals.EventAggregator.Publish(new PharmacyCloseSearchVisitorEvent { SelectedOutwardInfo =e.Value});
        }
        public void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
            OutwardDrugInvoice p = e.Row.DataContext as OutwardDrugInvoice;
            if (p != null && p.IsHICount.GetValueOrDefault() && p.V_OutDrugInvStatus != (long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
            {
                e.Row.Background = new SolidColorBrush(Colors.Yellow);
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
            else
            {
                e.Row.Background = new SolidColorBrush(Colors.Transparent);
                e.Row.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        DataGrid gr = null;
        public void dataGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            gr = sender as DataGrid;
            gr.ItemsSource = OutwardInfoList;
        }
      
    }
}
