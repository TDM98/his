using eHCMSLanguage;
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
using aEMR.Infrastructure.CachingUtils;
using aEMR.Common;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptPurchaseOrderSearchEstimate)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptPurchaseOrderSearchEstimateViewModel : Conductor<object>, IDrugDeptPurchaseOrderSearchEstimate
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC; //11001 : thuoc, 11002 : y cu , 11003 :hoa chat
        public long V_MedProductType
        {
            get
            {
                return _V_MedProductType;
            }
            set
            {
                if (_V_MedProductType != value)
                {
                    _V_MedProductType = value;
                    NotifyOfPropertyChange(() => V_MedProductType);
                }

            }
        }

        [ImportingConstructor]
        public DrugDeptPurchaseOrderSearchEstimateViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            SearchCriteria = new RequestSearchCriteria();
            SearchCriteria.IsNotOrder = true;

            DrugDeptEstimationForPOList = new PagedSortableCollectionView<DrugDeptEstimationForPO>();
            DrugDeptEstimationForPOList.OnRefresh += DrugDeptEstimationForPOList_OnRefresh;
            DrugDeptEstimationForPOList.PageSize = Globals.PageSize;

           // DrugDeptEstimationForPO_Search(DrugDeptEstimationForPOList.PageIndex, DrugDeptEstimationForPOList.PageSize);
        }

        void DrugDeptEstimationForPOList_OnRefresh(object sender, RefreshEventArgs e)
        {
            DrugDeptEstimationForPO_Search(DrugDeptEstimationForPOList.PageIndex, DrugDeptEstimationForPOList.PageSize);
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

        private PagedSortableCollectionView<DrugDeptEstimationForPO> _DrugDeptEstimationForPOList;
        public PagedSortableCollectionView<DrugDeptEstimationForPO> DrugDeptEstimationForPOList
        {
            get
            {
                return _DrugDeptEstimationForPOList;
            }
            set
            {
                if (_DrugDeptEstimationForPOList != value)
                {
                    _DrugDeptEstimationForPOList = value;
                    NotifyOfPropertyChange("DrugDeptEstimationForPOList");
                }
            }
        }


        #endregion

        public void btnSearch()
        {
            DrugDeptEstimationForPOList.PageIndex = 0;
            DrugDeptEstimationForPO_Search(DrugDeptEstimationForPOList.PageIndex, DrugDeptEstimationForPOList.PageSize);
        }
        private void DrugDeptEstimationForPO_Search(int PageIndex, int PageSize)
        {
            IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyEstimattionServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptEstimationForPO_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndDrugDeptEstimationForPO_Search(out Total, asyncResult);
                            if (results != null )
                            {
                                DrugDeptEstimationForPOList.Clear();
                                DrugDeptEstimationForPOList.TotalItemCount = Total;
                                foreach(DrugDeptEstimationForPO p in results)
                                {
                                    DrugDeptEstimationForPOList.Add(p);
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
                           // Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void dataGrid1_DblClick(object sender, EventArgs<object> e)
        {
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchPurchaseOrderEstimationEvent { SelectedEstimation = e.Value });
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
