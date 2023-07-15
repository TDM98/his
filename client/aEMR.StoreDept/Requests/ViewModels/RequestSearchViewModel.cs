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
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
/*
 * 20181219 #001 TTM: BM 0005443: Lọc dữ liệu tìm kiếm cho trường hợp phiếu lĩnh nhà thuốc hoặc phiếu lĩnh của kho BHYT - nhà thuốc.                          
 */
namespace aEMR.StoreDept.Requests.ViewModels
{
    [Export(typeof(IStoreDeptRequestSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class RequestSearchViewModel : ViewModelBase, IStoreDeptRequestSearch
    {
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

            RequestDruglist = new PagedSortableCollectionView<RequestDrugInwardClinicDept>();
            RequestDruglist.OnRefresh += RequestDruglist_OnRefresh;
            RequestDruglist.PageSize = 20;
            RequestFoodlist = new PagedSortableCollectionView<RequestFoodClinicDept>();
            RequestFoodlist.OnRefresh += RequestFoodlist_OnRefresh;
            RequestFoodlist.PageSize = 20;
        }

        private long _FilterByPtRegistrationID = 0;
        public long FilterByPtRegistrationID
        {
            get
            {
                return _FilterByPtRegistrationID;
            }
            set
            {
                _FilterByPtRegistrationID = value;
                if (FilterByPtRegistrationID > 0)
                {
                    SearchCriteria.PtRegistrationID = FilterByPtRegistrationID;
                }
            }
        }

        private bool _IsCreateNewListFromSelectExisting = false;
        public bool IsCreateNewListFromSelectExisting
        {
            get
            {
                return _IsCreateNewListFromSelectExisting;
            }
            set
            {
                _IsCreateNewListFromSelectExisting = value;
            }
        }

        void RequestDruglist_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
        }
        void RequestFoodlist_OnRefresh(object sender, RefreshEventArgs e)
        {
            //SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
        }

        #region Properties member

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
        private PagedSortableCollectionView<RequestDrugInwardClinicDept> _RequestDrugList;
        public PagedSortableCollectionView<RequestDrugInwardClinicDept> RequestDruglist
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
        private PagedSortableCollectionView<RequestFoodClinicDept> _RequestFoodlist;
        public PagedSortableCollectionView<RequestFoodClinicDept> RequestFoodlist
        {
            get
            {
                return _RequestFoodlist;
            }
            set
            {
                if (_RequestFoodlist != value)
                {
                    _RequestFoodlist = value;
                    NotifyOfPropertyChange(() => RequestFoodlist);
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
            if (!IsRequestFromHIStore)
            {
                RequestDruglist.PageIndex = 0;
                SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
            }
            else
            {
                RequestDruglistHIStore.PageIndex = 0;
                SearchRequestDrugInwardHIStore(RequestDruglistHIStore.PageIndex, RequestDruglistHIStore.PageSize);
            }
        }

        public void SearchRequestDrugInwardClinicDept(int PageIndex, int PageSize)
        {
            this.DlgShowBusyIndicator();
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardClinicDept(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardClinicDept(out Total, asyncResult);
                                RequestDruglist.Clear();
                                RequestDruglist.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (RequestDrugInwardClinicDept p in results)
                                    {
                                        RequestDruglist.Add(p);
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                _logger.Error(ex.Message);
                            }
                            finally
                            {
                                this.DlgHideBusyIndicator();
                            }
                        }), null);
                    }
                });

                t.Start();
            }
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                _logger.Error(ex.Message);
                this.DlgHideBusyIndicator();
            }
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();

            if (FilterByPtRegistrationID > 0)
            {
                Globals.EventAggregator.Publish(new ClinicDeptInPtSelReqFormForOutward { SelectedReqForm = e.Value });
            }
            else
            {
                Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestEvent { SelectedRequest = e.Value, IsCreateNewFromExisting = IsCreateNewListFromSelectExisting });
            }
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.Code = (sender as TextBox).Text;
                }

                if (!IsRequestFromHIStore)
                {
                    RequestDruglist.PageIndex = 0;
                    SearchRequestDrugInwardClinicDept(RequestDruglist.PageIndex, RequestDruglist.PageSize);
                }
                else
                {
                    RequestDruglistHIStore.PageIndex = 0;
                    SearchRequestDrugInwardHIStore(RequestDruglistHIStore.PageIndex, RequestDruglistHIStore.PageSize);
                }
            }
        }

        //▼====== #001
        public void SetList()
        {
            RequestDruglistHIStore = new PagedSortableCollectionView<RequestDrugInwardForHiStore>();
            RequestDruglistHIStore.OnRefresh += RequestDruglistHIStore_OnRefresh;
            RequestDruglistHIStore.PageSize = Globals.PageSize;
        }

        void RequestDruglistHIStore_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchRequestDrugInwardHIStore(RequestDruglistHIStore.PageIndex, RequestDruglistHIStore.PageSize);
        }

        public void dataGrid2_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchRequestForHIStoreEvent { SelectedRequest = e.Value, IsCreateNewFromExisting = IsCreateNewListFromSelectExisting });
        }

        private PagedSortableCollectionView<RequestDrugInwardForHiStore> _RequestDruglistHIStore;
        public PagedSortableCollectionView<RequestDrugInwardForHiStore> RequestDruglistHIStore
        {
            get
            {
                return _RequestDruglistHIStore;
            }
            set
            {
                if (_RequestDruglistHIStore != value)
                {
                    _RequestDruglistHIStore = value;
                    NotifyOfPropertyChange(() => RequestDruglistHIStore);
                }
            }
        }

        private bool _vGrid = true;
        public bool vGrid
        {
            get
            {
                return _vGrid;
            }
            set
            {
                _vGrid = value;
                NotifyOfPropertyChange(() => vGrid);
            }
        }

        private bool _IsRequestFromHIStore = false;
        public bool IsRequestFromHIStore
        {
            get
            {
                return _IsRequestFromHIStore;
            }
            set
            {
                _IsRequestFromHIStore = value;
                if (_IsRequestFromHIStore)
                {
                    vGrid = false;
                }
                NotifyOfPropertyChange(() => IsRequestFromHIStore);
            }
        }

        private void SearchRequestDrugInwardHIStore(int PageIndex, int PageSize)
        {
            try
            {
                var t = new Thread(() =>
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchRequestDrugInwardHIStore(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRequestDrugInwardHIStore(out Total, asyncResult);
                                RequestDruglistHIStore.Clear();
                                RequestDruglistHIStore.TotalItemCount = Total;
                                if (results != null)
                                {
                                    foreach (RequestDrugInwardForHiStore p in results)
                                    {
                                        RequestDruglistHIStore.Add(p);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                                ClientLoggerHelper.LogError(ex.ToString());
                            }
                            finally
                            {
                                this.HideBusyIndicator();
                            }
                        }), null);
                    }

                });

                t.Start();

            }
            catch (Exception ex)
            {
                Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                ClientLoggerHelper.LogError(ex.ToString());
                this.HideBusyIndicator();
            }
        }
        //▲====== #001
    }
}