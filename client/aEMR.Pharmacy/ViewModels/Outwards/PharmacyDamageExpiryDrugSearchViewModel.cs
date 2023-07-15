using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Windows.Controls;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacyDamageExpiryDrugSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyDamageExpiryDrugSearchViewModel : ViewModelBase, IPharmacyDamageExpiryDrugSearch
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

        public enum DataGridCol
        {
            MAPHIEU = 0,
            MAPHIEUMUON=1,
            NGAY = 2,
            TENKHOXUAT = 3,
            NHANVIENXUAT = 4,
            XUATDENKHO = 5,
            NHANVIENNHAN=6,
            BVBAN = 7,
            MAPHIEUYC= 8,
            TRANGTHAI = 9,
            GHICHU = 10
        }

        [ImportingConstructor]
        public PharmacyDamageExpiryDrugSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            OutwardDrugInvoiceList = new PagedSortableCollectionView<OutwardDrugInvoice>();
            OutwardDrugInvoiceList.OnRefresh += OutwardDrugInvoiceList_OnRefresh;
            OutwardDrugInvoiceList.PageSize = 20;
        }

        void OutwardDrugInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchOutwardInfo(OutwardDrugInvoiceList.PageIndex, OutwardDrugInvoiceList.PageSize);
        }

        protected override void OnActivate()
        {
        }
        protected override void OnDeactivate(bool close)
        {
            OutwardDrugInvoiceList.Clear();
        }
      
        #region Properties member

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
          
        private PagedSortableCollectionView<OutwardDrugInvoice> _OutwardDrugInvoiceList;
        public PagedSortableCollectionView<OutwardDrugInvoice> OutwardDrugInvoiceList
        {
            get
            {
                return _OutwardDrugInvoiceList;
            }
            set
            {
                if (_OutwardDrugInvoiceList != value)
                {
                    _OutwardDrugInvoiceList = value;
                    NotifyOfPropertyChange(() => OutwardDrugInvoiceList);
                }
            }
        }
        #endregion

        public void SearchOutwardInfo(int PageIndex, int PageSize)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            int Total = 0;
            IsLoading = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginOutWardDrugInvoice_SearchByType(SearchCriteria, PageIndex, PageSize,true, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndOutWardDrugInvoice_SearchByType(out Total, asyncResult);
                            if (results != null)
                            {
                                OutwardDrugInvoiceList.Clear();
                                OutwardDrugInvoiceList.TotalItemCount = Total;
                                foreach (OutwardDrugInvoice p in results)
                                {
                                    OutwardDrugInvoiceList.Add(p);
                                }
                                NotifyOfPropertyChange(() => OutwardDrugInvoiceList);
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
            OutwardDrugInvoiceList.PageIndex = 0;
            SearchOutwardInfo(OutwardDrugInvoiceList.PageIndex, OutwardDrugInvoiceList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new PharmacyCloseSearchDemageDrugEvent { SelectedOutwardDrugInvoice = e.Value });
            TryClose();
        }
        DataGrid dataGrid1 = null;
        public void dataGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1 = sender as DataGrid;
            if (SearchCriteria.TypID == (long)AllLookupValues.RefOutputType.HUYHANG)
            {
                dataGrid1.Columns[(int)DataGridCol.MAPHIEUMUON].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.TRANGTHAI].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.XUATDENKHO].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.BVBAN].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.MAPHIEUYC].Visibility = Visibility.Collapsed;
                dataGrid1.Columns[(int)DataGridCol.NHANVIENNHAN].Visibility = Visibility.Collapsed;
            }
            else if (SearchCriteria.TypID == (long)AllLookupValues.RefOutputType.XUATNOIBO)
            {
                dataGrid1.Columns[(int)DataGridCol.MAPHIEUMUON].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.TRANGTHAI].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.XUATDENKHO].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.BVBAN].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.MAPHIEUYC].Visibility = Visibility.Visible;
                dataGrid1.Columns[(int)DataGridCol.NHANVIENNHAN].Visibility = Visibility.Visible;
            }
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
