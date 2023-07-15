using eHCMSLanguage;
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
using System.Collections.ObjectModel;
using aEMR.CommonTasks;
using System.Collections.Generic;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IStockTakesSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StockTakesSearchViewModel : ViewModelBase, IStockTakesSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StockTakesSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            Coroutine.BeginExecute(DoGetStore_EXTERNAL());
            PharmacyStockTakeList = new PagedSortableCollectionView<PharmacyStockTakes>();
            PharmacyStockTakeList.OnRefresh += PharmacyStockTakeList_OnRefresh;
            PharmacyStockTakeList.PageSize = Globals.PageSize;
        }

        void PharmacyStockTakeList_OnRefresh(object sender, RefreshEventArgs e)
        {
            PharmacyStockTakesSearchCriteria(PharmacyStockTakeList.PageIndex, PharmacyStockTakeList.PageSize);
        }

        //protected override void OnActivate()
        //{
        //}
        //protected override void OnDeactivate(bool close)
        //{
        //    PharmacyStockTakeList.Clear();
        //}

        #region Properties member

        private PharmacyStockTakesSearchCriteria _searchCriteria;
        public PharmacyStockTakesSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            set
            {
                if (_searchCriteria != value)
                    _searchCriteria = value;
                NotifyOfPropertyChange(() => SearchCriteria);
            }
        }

        private PagedSortableCollectionView<PharmacyStockTakes> _PharmacyStockTakeList;
        public PagedSortableCollectionView<PharmacyStockTakes> PharmacyStockTakeList
        {
            get
            {
                return _PharmacyStockTakeList;
            }
            set
            {
                if (_PharmacyStockTakeList != value)
                {
                    _PharmacyStockTakeList = value;
                    NotifyOfPropertyChange(() => PharmacyStockTakeList);
                }
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
        #endregion

        public void PharmacyStockTakesSearchCriteria(int PageIndex, int PageSize)
        {
            int Total = 0;
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
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
                                var results = contract.EndPharmacyStockTakes_Search(out Total, asyncResult);
                                if (results != null)
                                {
                                    PharmacyStockTakeList.Clear();
                                    PharmacyStockTakeList.TotalItemCount = Total;
                                    foreach (PharmacyStockTakes p in results)
                                    {
                                        PharmacyStockTakeList.Add(p);
                                    }
                                    NotifyOfPropertyChange(() => PharmacyStockTakeList);
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                }
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    this.DlgHideBusyIndicator();
                }
            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            PharmacyStockTakeList.PageIndex = 0;
            PharmacyStockTakesSearchCriteria(PharmacyStockTakeList.PageIndex, PharmacyStockTakeList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            //phat ra su kien
            Globals.EventAggregator.Publish(new PharmacyCloseSearchStockTakesEvent { SelectedPharmacyStockTakes = e.Value });
            TryClose();
        }

        private IEnumerator<IResult> DoGetStore_EXTERNAL()
        {
            var paymentTypeTask = new LoadStoreListTask((long)AllLookupValues.StoreType.STORAGE_EXTERNAL, false, null, false, false);
            yield return paymentTypeTask;
            StoreCbx = paymentTypeTask.LookupList;
            yield break;
        }
    }
}
