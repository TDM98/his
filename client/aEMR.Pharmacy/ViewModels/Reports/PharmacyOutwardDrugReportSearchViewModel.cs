using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Controls;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyOutwardDrugReportSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyOutwardDrugReportSearchViewModel : Conductor<object>, IPharmacyOutwardDrugReportSearch
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

        public enum DataGridCol
        {
            MAPHIEU = 0,
            NGAY = 1,
            TENKHOXUAT = 2,
            NHANVIENXUAT = 3,
            XUATDENKHO = 4,
            NHANVIENNHAN=5,
            BVBAN = 6,
            MAPHIEUYC= 7,
        }
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacyOutwardDrugReportSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            PharmacyOutwardDrugReportList = new PagedSortableCollectionView<PharmacyOutwardDrugReport>();
            PharmacyOutwardDrugReportList.OnRefresh += PharmacyOutwardDrugReportList_OnRefresh;
            PharmacyOutwardDrugReportList.PageSize = Globals.PageSize;
        }

        void PharmacyOutwardDrugReportList_OnRefresh(object sender, RefreshEventArgs e)
        {
            PharmacyOutwardDrugReport_Search(PharmacyOutwardDrugReportList.PageIndex, PharmacyOutwardDrugReportList.PageSize);
        }

        protected override void OnActivate()
        {
        }
        protected override void OnDeactivate(bool close)
        {
            PharmacyOutwardDrugReportList.Clear();
        }
      
        #region Properties member

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
        #endregion

        public void PharmacyOutwardDrugReport_Search(int PageIndex, int PageSize)
        {
            int Total = 0;
            IsLoading = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
                            var results = contract.EndPharmacyOutwardDrugReport_Search(out Total, asyncResult);
                            if (results != null)
                            {
                                PharmacyOutwardDrugReportList.Clear();
                                PharmacyOutwardDrugReportList.TotalItemCount = Total;
                                foreach (PharmacyOutwardDrugReport p in results)
                                {
                                    PharmacyOutwardDrugReportList.Add(p);
                                }
                                NotifyOfPropertyChange(() => PharmacyOutwardDrugReportList);
                            }
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

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            PharmacyOutwardDrugReportList.PageIndex = 0;
            PharmacyOutwardDrugReport_Search(PharmacyOutwardDrugReportList.PageIndex, PharmacyOutwardDrugReportList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new PharmacyCloseSearchOutReportDrugEvent { SelectedPharmacyOutwardDrugReport = e.Value });
            TryClose();
        }
        DataGrid dataGrid1 = null;
        public void dataGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1 = sender as DataGrid;
        }

        public void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            if (dataGrid1 != null)
            {
                dataGrid1.SetValue(DataGrid.ItemsSourceProperty, null);
            }
        }
    }
}
