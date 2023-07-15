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
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Windows.Media;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels
{
     [Export(typeof(IPrescriptionSearchInvoice)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrescriptionSearchInvoiceViewModel : ViewModelBase, IPrescriptionSearchInvoice
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
        public PrescriptionSearchInvoiceViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            LoadOutStatus();

             OutwardDrugInvoices = new PagedSortableCollectionView<OutwardDrugInvoice>();
             OutwardDrugInvoices.OnRefresh += new EventHandler<RefreshEventArgs>(OutwardDrugInvoices_OnRefresh);
             OutwardDrugInvoices.PageSize = 20;
            
         }

         void OutwardDrugInvoices_OnRefresh(object sender, RefreshEventArgs e)
         {
             SearchOutwardPrescription(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
         }

         #region Properties Member
         private ObservableCollection<Lookup> _OutStatus;
         public ObservableCollection<Lookup> OutStatus
         {
             get
             {
                 return _OutStatus;
             }
             set
             {
                 if (_OutStatus != value)
                 {
                     _OutStatus = value;
                     NotifyOfPropertyChange(() => OutStatus);
                 }
             }
         }

         private SearchOutwardInfo _SearchInvoiceCriteria;
         public SearchOutwardInfo SearchInvoiceCriteria
         {
             get
             {
                 return _SearchInvoiceCriteria;
             }
             set
             {
                 if (_SearchInvoiceCriteria != value)
                 {
                     _SearchInvoiceCriteria = value;
                 }
                 NotifyOfPropertyChange(() => SearchInvoiceCriteria);
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
                 }
                 NotifyOfPropertyChange(()=>OutwardDrugInvoices);
             }
         }


         #endregion

         private const string ALLITEMS = "[All]";
         private void LoadOutStatus()
         {
             IsLoading = true;
             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new CommonService_V2Client())
                 {
                     var contract = serviceFactory.ServiceInstance;
                     contract.BeginGetAllLookupValuesByType(LookupValues.V_OutDrugInvStatus, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             var results = contract.EndGetAllLookupValuesByType(asyncResult);
                             OutStatus = results.ToObservableCollection();
                             Lookup item = new Lookup();
                             item.LookupID = 0;
                             item.ObjectValue = ALLITEMS;
                             OutStatus.Insert(0, item);
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
         public void Search_KeyUp_MaPhieuXuat(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchInvoiceCriteria != null)
                 {
                     SearchInvoiceCriteria.OutInvID = (sender as TextBox).Text;
                 }
                 OutwardDrugInvoices.PageIndex = 0;
                 SearchOutwardPrescription(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
             }
         }
         public void Search_KeyUp_PatientName(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchInvoiceCriteria != null)
                 {
                     SearchInvoiceCriteria.CustomerName = (sender as TextBox).Text;
                 }
                 OutwardDrugInvoices.PageIndex = 0;
                 SearchOutwardPrescription(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
             }
         }
         public void Search_KeyUp_HICardCode(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchInvoiceCriteria != null)
                 {
                     SearchInvoiceCriteria.HICardCode = (sender as TextBox).Text;
                 }
                 OutwardDrugInvoices.PageIndex = 0;
                 SearchOutwardPrescription(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
             }
         }
         public void Search_KeyUp_PatientCode(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 if (SearchInvoiceCriteria != null)
                 {
                     SearchInvoiceCriteria.PatientCode = (sender as TextBox).Text;
                 }
                 OutwardDrugInvoices.PageIndex = 0;
                 SearchOutwardPrescription(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
             }
         }

         public void Search_KeyUp(object sender, KeyEventArgs e)
         {
             if (e.Key == Key.Enter)
             {
                 //if (SearchInvoiceCriteria != null)
                 //{
                 //    SearchInvoiceCriteria.PatientCode = (sender as TextBox).Text;
                 //}
                 OutwardDrugInvoices.PageIndex = 0;
                 SearchOutwardPrescription(OutwardDrugInvoices.PageIndex, OutwardDrugInvoices.PageSize);
             }
         }

         public void SearchOutwardPrescription(int PageIndex, int PageSize)
         {     
             this.DlgShowBusyIndicator(string.Format(eHCMSResources.Z0997_G1_Format2, eHCMSResources.Z1125_G1_DangTimKiem));
             //IsLoading = true;
             //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             int Total = 0;
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginGetOutWardDrugInvoiceSearchAllByStatus(SearchInvoiceCriteria, PageIndex, PageSize, true, null, false, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             var results = contract.EndGetOutWardDrugInvoiceSearchAllByStatus(out Total, asyncResult);
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
                             //this.HideBusyIndicator();
                             //IsLoading = false;
                            // Globals.IsBusy = false;
                         }

                     }), null);

                 }

             });

             t.Start();
         }

         public void btnSearch(object sender, RoutedEventArgs e)
         {
             OutwardDrugInvoices.PageIndex = 0;
             SearchOutwardPrescription(OutwardDrugInvoices.PageIndex,OutwardDrugInvoices.PageSize);
         }

         public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
         {
             TryClose();
             Globals.EventAggregator.Publish(new PharmacyCloseSearchPrescriptionInvoiceEvent { SelectedInvoice=e.Value});
         }

         public void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
         {
             e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
             OutwardDrugInvoice p = e.Row.DataContext as OutwardDrugInvoice;
             if (p !=null && p.IsHICount.GetValueOrDefault() && p.V_OutDrugInvStatus !=(long)AllLookupValues.V_OutDrugInvStatus.CANCELED)
             {
                 e.Row.Background = new SolidColorBrush(Colors.Yellow);
                 e.Row.Foreground = new SolidColorBrush(Colors.Black);
             }
             else
             {
                 e.Row.Background = new SolidColorBrush(Colors.Transparent);
                 e.Row.Foreground = new SolidColorBrush(Colors.Black);
             }
         }

         DataGrid gr = null;
         public void dataGrid1_Loaded(object sender, RoutedEventArgs e)
         {
             gr = sender as DataGrid;
             gr.ItemsSource = OutwardDrugInvoices;
         }
     }

}
