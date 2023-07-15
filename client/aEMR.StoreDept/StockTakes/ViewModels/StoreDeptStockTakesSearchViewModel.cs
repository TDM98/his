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
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;

namespace aEMR.StoreDept.StockTakes.ViewModels
{
    [Export(typeof(IStoreDeptStockTakesSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreDeptStockTakesSearchViewModel : Conductor<object>, IStoreDeptStockTakesSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StoreDeptStockTakesSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            ClinicDeptStockTakeList = new PagedSortableCollectionView<ClinicDeptStockTakes>();
            ClinicDeptStockTakeList.OnRefresh += ClinicDeptStockTakeList_OnRefresh;
            ClinicDeptStockTakeList.PageSize = Globals.PageSize;
        }

        void ClinicDeptStockTakeList_OnRefresh(object sender, RefreshEventArgs e)
        {
            ClinicDeptStockTakesSearchCriteria(ClinicDeptStockTakeList.PageIndex, ClinicDeptStockTakeList.PageSize);
        }

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
        protected override void OnActivate()
        {
        }
        protected override void OnDeactivate(bool close)
        {
            ClinicDeptStockTakeList.Clear();
        }
      
        #region Properties member

        private ClinicDeptStockTakesSearchCriteria _searchCriteria;
        public ClinicDeptStockTakesSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

          
        private PagedSortableCollectionView<ClinicDeptStockTakes> _ClinicDeptStockTakeList;
        public PagedSortableCollectionView<ClinicDeptStockTakes> ClinicDeptStockTakeList
        {
            get
            {
                return _ClinicDeptStockTakeList;
            }
            set
            {
                if (_ClinicDeptStockTakeList != value)
                {
                    _ClinicDeptStockTakeList = value;
                    NotifyOfPropertyChange(() => ClinicDeptStockTakeList);
                }
            }
        }
        #endregion

        public void ClinicDeptStockTakesSearchCriteria(int PageIndex, int PageSize)
        {
            int Total = 0;
            Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;

                    contract.BeginClinicDeptStockTakes_Search(SearchCriteria, PageIndex, PageSize, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndClinicDeptStockTakes_Search(out Total, asyncResult);
                            if (results != null)
                            {
                                ClinicDeptStockTakeList.Clear();
                                ClinicDeptStockTakeList.TotalItemCount = Total;
                                foreach (ClinicDeptStockTakes p in results)
                                {
                                    ClinicDeptStockTakeList.Add(p);
                                }
                                NotifyOfPropertyChange(() => ClinicDeptStockTakeList);
                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            IsLoading = false;
                            Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            ClinicDeptStockTakeList.PageIndex = 0;
            ClinicDeptStockTakesSearchCriteria(ClinicDeptStockTakeList.PageIndex, ClinicDeptStockTakeList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new ClinicDeptCloseSearchStockTakesEvent { SelectedClinicDeptStockTakes = e.Value });
            TryClose();
        }

    }
}
