using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
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
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IVisitorSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class VisitorSearchViewModel : ViewModelBase, IVisitorSearch
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
        public VisitorSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            LoadOutStatus();

            OutwardInfoList = new PagedSortableCollectionView<OutwardDrugInvoice>();
            OutwardInfoList.PageSize = 20;
            OutwardInfoList.OnRefresh += OutwardInfoList_OnRefresh;
        }

        void OutwardInfoList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchOutwardInfo1(OutwardInfoList.PageIndex, OutwardInfoList.PageSize);
        }

      
        #region Properties member
        private const string ALLITEMS = "[All]";

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
        #endregion

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
        public void SearchOutwardInfo1(int PageIndex, int PageSize)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
            int Total = 0;
            //IsLoading = true;
            // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchCriteria, PageIndex, PageSize, true, null, false, Globals.DispatchCallback((asyncResult) =>
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
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch()
        {
            OutwardInfoList.PageIndex = 0;
            SearchOutwardInfo1(OutwardInfoList.PageIndex, OutwardInfoList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            //phat ra su kien
            Globals.EventAggregator.Publish(new PharmacyCloseSearchVisitorEvent { SelectedOutwardInfo =e.Value});
        }

        public void Search_KeyUp_MaPhieu(object sender, KeyEventArgs e)
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
        public void Search_KeyUp_PatientName(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.CustomerName = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
    }
}
