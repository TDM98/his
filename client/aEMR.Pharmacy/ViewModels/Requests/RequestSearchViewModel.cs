using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
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
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IRequestSearchPharmacy)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestSearchViewModel : ViewModelBase, IRequestSearchPharmacy
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
        public RequestSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();

            RequestDruglist = new PagedSortableCollectionView<RequestDrugInward>();
            RequestDruglist.OnRefresh += RequestDruglist_OnRefresh;
            RequestDruglist.PageSize = Globals.PageSize;
        }

        void RequestDruglist_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRequestDrugInward(RequestDruglist.PageIndex, RequestDruglist.PageSize);
        }

        
        #region Properties member

  

        private PagedSortableCollectionView<RequestDrugInward> _RequestDrugList;
        public PagedSortableCollectionView<RequestDrugInward> RequestDruglist
        {
            get
            {
                return _RequestDrugList;
            }
            set
            {
                if (_RequestDrugList != value)
                {
                    _RequestDrugList = value;
                    NotifyOfPropertyChange(() => RequestDruglist);
                }
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

        #endregion

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            RequestDruglist.PageIndex = 0;
            SearchRequestDrugInward(RequestDruglist.PageIndex, RequestDruglist.PageSize);
        }

        public void SearchRequestDrugInward(int PageIndex, int PageSize)
        {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSearchRequestDrugInward(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndSearchRequestDrugInward(out Total, asyncResult);
                            RequestDruglist.Clear();
                            RequestDruglist.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (RequestDrugInward p in results)
                                {
                                    RequestDruglist.Add(p);
                                }

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


        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new PharmacyCloseSearchRequestEvent { SelectedRequest = e.Value });
        }
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                RequestDruglist.PageIndex = 0;
                SearchRequestDrugInward(RequestDruglist.PageIndex, RequestDruglist.PageSize);
            }
        }
    }
}
