using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;
using aEMR.Common;
using Castle.Windsor;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IEstimationDrugDeptSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class EstimationDrugDeptSearchViewModel : ViewModelBase, IEstimationDrugDeptSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;
        [ImportingConstructor]
        public EstimationDrugDeptSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();
            DrugDeptEstimationForPOList = new PagedSortableCollectionView<DrugDeptEstimationForPO>();
            DrugDeptEstimationForPOList.OnRefresh += DrugDeptEstimationForPOList_OnRefresh;
            DrugDeptEstimationForPOList.PageSize = Globals.PageSize;
        }

        void DrugDeptEstimationForPOList_OnRefresh(object sender, RefreshEventArgs e)
        {
            DrugDeptEstimationForPO_Search(DrugDeptEstimationForPOList.PageIndex, DrugDeptEstimationForPOList.PageSize);
        }

        #region Properties member
        private string _strHienThi;
        public string strHienThi
        {
            get
            {
                return _strHienThi;
            }
            set
            {
                _strHienThi = value;
                NotifyOfPropertyChange(() => strHienThi);
            }
        }

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

        public bool IsByBid { get; set; } = false;
        #endregion

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            DrugDeptEstimationForPOList.PageIndex = 0;
            DrugDeptEstimationForPO_Search(DrugDeptEstimationForPOList.PageIndex, DrugDeptEstimationForPOList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, EventArgs<object> e)
        {
            TryClose();
            //phat su kien 
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchEstimationEvent { SelectedEstimation = e.Value });
        }

        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DrugDeptEstimationForPOList.PageIndex = 0;
                DrugDeptEstimationForPO_Search(DrugDeptEstimationForPOList.PageIndex, DrugDeptEstimationForPOList.PageSize);
            }
        }

        #region Methods
        private void DrugDeptEstimationForPO_Search(int PageIndex, int PageSize)
        {
            this.DlgShowBusyIndicator(eHCMSResources.K2887_G1_DangXuLy);
            if (IsByBid)
            {
                var t = new Thread(() =>
                {
                    try
                    {
                        using (var serviceFactory = new PharmacyEstimattionServiceClient())
                        {
                            var contract = serviceFactory.ServiceInstance;
                            contract.BeginDrugDeptEstimationForPO_SearchByBid(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                            {
                                try
                                {
                                    int Total = 0;
                                    var results = contract.EndDrugDeptEstimationForPO_SearchByBid(out Total, asyncResult);
                                    if (results != null)
                                    {
                                        DrugDeptEstimationForPOList.Clear();
                                        DrugDeptEstimationForPOList.TotalItemCount = Total;
                                        foreach (DrugDeptEstimationForPO p in results)
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
            else
            {
                var t = new Thread(() =>
                {
                    try
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
                                    if (results != null)
                                    {
                                        DrugDeptEstimationForPOList.Clear();
                                        DrugDeptEstimationForPOList.TotalItemCount = Total;
                                        foreach (DrugDeptEstimationForPO p in results)
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
        }
        #endregion
    }
}
