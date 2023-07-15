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
using aEMR.Common.BaseModel;
using eHCMSLanguage;

namespace aEMR.Pharmacy.ViewModels
{
     [Export(typeof(IReturnDrugSearchInvoice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReturnDrugSearchInvoiceViewModel : ViewModelBase, IReturnDrugSearchInvoice
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
        public ReturnDrugSearchInvoiceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            SearchCriteriaReturn = new SearchOutwardInfo();

            OutwardDrugInvoices = new PagedSortableCollectionView<OutwardDrugInvoice>();
            OutwardDrugInvoices.OnRefresh += new EventHandler<RefreshEventArgs>(OutwardDrugInvoices_OnRefresh);
            OutwardDrugInvoices.PageSize = 20;
        }

         void OutwardDrugInvoices_OnRefresh(object sender, RefreshEventArgs e)
         {
             GetOutWardDrugInvoiceSearchReturn(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
         }

         #region Properties Member

         private SearchOutwardInfo _searchCriteriaReturn;
         public SearchOutwardInfo SearchCriteriaReturn
         {
             get { return _searchCriteriaReturn; }
             set
             {
                 if (_searchCriteriaReturn != value)
                     _searchCriteriaReturn = value;
                 NotifyOfPropertyChange(() => SearchCriteriaReturn);
             }
         }

         private PagedSortableCollectionView<OutwardDrugInvoice> _OutwardDrugInvoices;
         public PagedSortableCollectionView<OutwardDrugInvoice> OutwardDrugInvoices
         {
             get
             {
                 return _OutwardDrugInvoices;
             }
             set
             {
                 if (_OutwardDrugInvoices != value)
                 {
                     _OutwardDrugInvoices = value;
                     NotifyOfPropertyChange(() => OutwardDrugInvoices);
                 }
                
             }
         }

         #endregion

         public void Search_KeyUp_MaPhieuXuat(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchCriteriaReturn != null)
                 {
                     SearchCriteriaReturn.OutInvID = (sender as TextBox).Text;
                 }
                 btnSearch();
             }
         }
         public void Search_KeyUp_PatientName(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchCriteriaReturn != null)
                 {
                     SearchCriteriaReturn.CustomerName = (sender as TextBox).Text;
                 }
                 btnSearch();
             }
         }
         public void Search_KeyUp_PatientCode(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchCriteriaReturn != null)
                 {
                     SearchCriteriaReturn.PatientCode = (sender as TextBox).Text;
                 }
                 btnSearch();
             }
         }
         public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchCriteriaReturn != null)
                 {
                     SearchCriteriaReturn.HICardCode = (sender as TextBox).Text;
                 }
                 btnSearch();
             }
         }
         private void GetOutWardDrugInvoiceSearchReturn(int PageIndex, int PageSize)
         {
             this.DlgShowBusyIndicator(eHCMSResources.Z1125_G1_DangTimKiem);
             int Total = 0;
             //IsLoading = true;
             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginGetOutWardDrugInvoiceSearchReturn(SearchCriteriaReturn, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             var results = contract.EndGetOutWardDrugInvoiceSearchReturn(out Total, asyncResult);
                             if (results != null)
                             {
                                 OutwardDrugInvoices.Clear();
                                 OutwardDrugInvoices.TotalItemCount = Total;
                                 foreach (OutwardDrugInvoice p in results)
                                 {
                                     OutwardDrugInvoices.Add(p);
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
                            // Globals.IsBusy = false;
                         }

                     }), null);

                 }

             });

             t.Start();
         }

         public void btnSearch()
         {
             OutwardDrugInvoices.PageIndex = 0;
             GetOutWardDrugInvoiceSearchReturn(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);

         }

         public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
         {
             TryClose();
             Globals.EventAggregator.Publish(new PharmacyCloseSearchReturnInvoiceEvent { SelectedInvoice = e.Value });
         }
    }
}
