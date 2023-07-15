using eHCMSLanguage;
using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
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

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPurchaseOrderSearchEstimate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PurchaseOrderSearchEstimateViewModel : Conductor<object>, IPurchaseOrderSearchEstimate
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

        public long V_MedProductType = 11001;

        [ImportingConstructor]
        public PurchaseOrderSearchEstimateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();
            SearchCriteria.IsNotOrder = true;

            PharmacyEstimationForPOList = new PagedSortableCollectionView<PharmacyEstimationForPO>();
            PharmacyEstimationForPOList.OnRefresh += PharmacyEstimationForPOList_OnRefresh;
            PharmacyEstimationForPOList.PageSize = Globals.PageSize;

           // PharmacyEstimationForPO_Search(PharmacyEstimationForPOList.PageIndex, PharmacyEstimationForPOList.PageSize);
        }

        void PharmacyEstimationForPOList_OnRefresh(object sender, RefreshEventArgs e)
        {
            PharmacyEstimationForPO_Search(PharmacyEstimationForPOList.PageIndex, PharmacyEstimationForPOList.PageSize);
        }
        #region Properties member
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

        private PagedSortableCollectionView<PharmacyEstimationForPO> _PharmacyEstimationForPOList;
        public PagedSortableCollectionView<PharmacyEstimationForPO> PharmacyEstimationForPOList
        {
            get
            {
                return _PharmacyEstimationForPOList;
            }
            set
            {
                if (_PharmacyEstimationForPOList != value)
                {
                    _PharmacyEstimationForPOList = value;
                    NotifyOfPropertyChange("PharmacyEstimationForPOList");
                }
            }
        }


        #endregion

        public void btnSearch()
        {
            PharmacyEstimationForPOList.PageIndex = 0;
            PharmacyEstimationForPO_Search(PharmacyEstimationForPOList.PageIndex, PharmacyEstimationForPOList.PageSize);
        }
        private void PharmacyEstimationForPO_Search(int PageIndex, int PageSize)
        {
            IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginPharmacyEstimationForPO_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, false, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndPharmacyEstimationForPO_Search(out Total, asyncResult);
                            if (results != null )
                            {
                                PharmacyEstimationForPOList.Clear();
                                PharmacyEstimationForPOList.TotalItemCount = Total;
                                foreach(PharmacyEstimationForPO p in results)
                                {
                                    PharmacyEstimationForPOList.Add(p);
                                }
                               
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

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new PharmacyCloseSearchPurchaseOrderEstimationEvent { SelectedEstimation = e.Value });
        }
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
    }
}
