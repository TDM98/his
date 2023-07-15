using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows.Controls;
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
using eHCMSLanguage;
using aEMR.Common.BaseModel;
using aEMR.Common;

namespace aEMR.DrugDept.ViewModels
{
     [Export(typeof(IDrugDeptPurchaseOrderSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptPurchaseOrderSearchViewModel : ViewModelBase, IDrugDeptPurchaseOrderSearch
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


        [ImportingConstructor]
        public DrugDeptPurchaseOrderSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();
             DrugDeptPurchaseOrderList = new PagedSortableCollectionView<DrugDeptPurchaseOrder>();
             DrugDeptPurchaseOrderList.OnRefresh += DrugDeptPurchaseOrderList_OnRefresh;
             DrugDeptPurchaseOrderList.PageSize = Globals.PageSize;
         }

         void DrugDeptPurchaseOrderList_OnRefresh(object sender, RefreshEventArgs e)
         {
             DrugDeptPurchaseOrder_Search(DrugDeptPurchaseOrderList.PageIndex, DrugDeptPurchaseOrderList.PageSize);
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

         private PagedSortableCollectionView<DrugDeptPurchaseOrder> _DrugDeptPurchaseOrderList;
         public PagedSortableCollectionView<DrugDeptPurchaseOrder> DrugDeptPurchaseOrderList
         {
             get
             {
                 return _DrugDeptPurchaseOrderList;
             }
             set
             {
                 if (_DrugDeptPurchaseOrderList != value)
                 {
                     _DrugDeptPurchaseOrderList = value;
                 }
                 NotifyOfPropertyChange(() => DrugDeptPurchaseOrderList);
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

         #endregion

         private void DrugDeptPurchaseOrder_Search(int PageIndex, int PageSize)
         {
             IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
             var t = new Thread(() =>
             {
                 try
                 {
                     using (var serviceFactory = new PharmacyEstimattionServiceClient())
                     {
                         var contract = serviceFactory.ServiceInstance;
                         contract.BeginDrugDeptPurchaseOrder_Search(SearchCriteria, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                          {

                              try
                              {
                                  int Total = 0;
                                  var results = contract.EndDrugDeptPurchaseOrder_Search(out Total, asyncResult);
                                  DrugDeptPurchaseOrderList.Clear();
                                  DrugDeptPurchaseOrderList.TotalItemCount = Total;
                                  if (results != null)
                                  {
                                      foreach (DrugDeptPurchaseOrder p in results)
                                      {
                                          DrugDeptPurchaseOrderList.Add(p);
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
                                  IsLoading = false;
                                  // Globals.IsBusy = false;
                                  this.DlgHideBusyIndicator();
                              }

                          }), null);

                     }
                 }
                 catch (Exception ex)
                 {
                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                     _logger.Error(ex.Message);
                     this.DlgHideBusyIndicator();
                 }
             });

             t.Start();
         }

         public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
         {
             TryClose();
             Globals.EventAggregator.Publish(new DrugDeptCloseSearchPurchaseOrderEvent { SelectedPurchaseOrder=e.Value});
         }

         public void btnSearch()
         {
             DrugDeptPurchaseOrderList.PageIndex = 0;
             DrugDeptPurchaseOrder_Search(0, DrugDeptPurchaseOrderList.PageSize);
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
