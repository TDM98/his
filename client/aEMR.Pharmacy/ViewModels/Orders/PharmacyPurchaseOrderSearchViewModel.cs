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
using aEMR.Common;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
     [Export(typeof(IPharmacyPurchaseOrderSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacyPurchaseOrderSearchViewModel : ViewModelBase, IPharmacyPurchaseOrderSearch
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
        public PharmacyPurchaseOrderSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();
             PharmacyPurchaseOrderList = new PagedSortableCollectionView<PharmacyPurchaseOrder>();
             PharmacyPurchaseOrderList.OnRefresh += PharmacyPurchaseOrderList_OnRefresh;
             PharmacyPurchaseOrderList.PageSize = Globals.PageSize;
         }

         void PharmacyPurchaseOrderList_OnRefresh(object sender, RefreshEventArgs e)
         {
             PharmacyPurchaseOrder_Search(PharmacyPurchaseOrderList.PageIndex, PharmacyPurchaseOrderList.PageSize);
         }

         #region Properties Member

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

         private PagedSortableCollectionView<PharmacyPurchaseOrder> _PharmacyPurchaseOrderList;
         public PagedSortableCollectionView<PharmacyPurchaseOrder> PharmacyPurchaseOrderList
         {
             get
             {
                 return _PharmacyPurchaseOrderList;
             }
             set
             {
                 if (_PharmacyPurchaseOrderList != value)
                 {
                     _PharmacyPurchaseOrderList = value;
                 }
                 NotifyOfPropertyChange(() => PharmacyPurchaseOrderList);
             }
         }


         #endregion

         private void PharmacyPurchaseOrder_Search(int PageIndex, int PageSize)
         {
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
             //IsLoading = true;
             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyEstimattionServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;
                     contract.BeginPharmacyPurchaseOrder_Search(SearchCriteria, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                     {

                         try
                         {
                             int Total = 0;
                             var results = contract.EndPharmacyPurchaseOrder_Search(out Total, asyncResult);
                             PharmacyPurchaseOrderList.Clear();
                             PharmacyPurchaseOrderList.TotalItemCount = Total;
                             if (results != null)
                             {
                                 foreach (PharmacyPurchaseOrder p in results)
                                 {
                                     PharmacyPurchaseOrderList.Add(p);
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
                             //IsLoading = false;
                             //Globals.IsBusy = false;
                         }

                     }), null);

                 }

             });

             t.Start();
         }

         public void dataGrid1_DblClick(object sender, EventArgs<object> e)
         {
             TryClose();
             Globals.EventAggregator.Publish(new PharmacyCloseSearchPurchaseOrderEvent { SelectedPurchaseOrder=e.Value});
         }

         public void btnSearch()
         {
             PharmacyPurchaseOrderList.PageIndex = 0;
             PharmacyPurchaseOrder_Search(0, PharmacyPurchaseOrderList.PageSize);
         }

         public void Search_KeyUp_MaPhieuDat(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if(SearchCriteria !=null)
                 {
                     SearchCriteria.Code = (sender as TextBox).Text;
                 }
                 btnSearch();
             }
         }
         public void Search_KeyUp_MaPhieuDutru(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchCriteria != null)
                 {
                     SearchCriteria.Code1 = (sender as TextBox).Text;
                 }
                 btnSearch();
             }
         }
     }
}
