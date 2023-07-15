using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common.Printing;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;


namespace aEMR.TransactionManager.ViewModels
{
     [Export(typeof(ITemp38NgoaiTru)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Temp38NgoaiTruViewModel : Conductor<object>, ITemp38NgoaiTru
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Temp38NgoaiTruViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
         {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
            authorization();
             SearchCriteria = new SeachPtRegistrationCriteria();
             SearchCriteria.FromDate = DateTime.Now;
             SearchCriteria.ToDate = DateTime.Now;
             PatientTransactionList = new PagedSortableCollectionView<PatientTransaction>();
             PatientTransactionList.OnRefresh += PatientTransactionList_OnRefresh;
             PatientTransactionList.PageSize = Globals.PageSize;
         }

         public void authorization()
         {
             if (!Globals.isAccountCheck)
             {
                 return;
             }
             else
             {
                 mSearch = Globals.CheckAuthorization(Globals.listRefModule, (int)eModules.mTransaction_Management,
                     (int)eTransaction_Management.mTemp01_BV_NgoaiTru, (int)oTransaction_ManagementEx.mSearch, (int)ePermission.mView);
             }
         }

         private bool _mSearch = true;

         public bool mSearch
         {
             get 
             { 
                 return _mSearch; 
             }
             set
             {
                 if (_mSearch == value)
                     return;
                 _mSearch = value;
                 NotifyOfPropertyChange(() => mSearch);
             }
         }



         void PatientTransactionList_OnRefresh(object sender, RefreshEventArgs e)
         {
             GetTransaction_ByPtID(PatientTransactionList.PageIndex, PatientTransactionList.PageSize);
         }

         #region propety member

         private bool _IsNgoaiTru;
         public bool IsNgoaiTru
         {
             get
             {
                 return _IsNgoaiTru;
             }
             set
             {
                 if (_IsNgoaiTru != value)
                 {
                     _IsNgoaiTru = value;
                     NotifyOfPropertyChange(() => IsNgoaiTru);
                 }
             }
         }

         private PagedSortableCollectionView<PatientTransaction> _PatientTransactions;
         public PagedSortableCollectionView<PatientTransaction> PatientTransactionList
         {
             get
             {
                 return _PatientTransactions;
             }
             set
             {
                 if (_PatientTransactions != value)
                 {
                     _PatientTransactions = value;
                     NotifyOfPropertyChange(()=>PatientTransactionList);
                 }
             }
         }

         private SeachPtRegistrationCriteria _SearchCriteria;
         public SeachPtRegistrationCriteria SearchCriteria
         {
             get
             {
                 return _SearchCriteria;
             }
             set
             {
                 if (_SearchCriteria != value)
                 {
                     _SearchCriteria = value;
                     NotifyOfPropertyChange(()=>SearchCriteria);
                 }
             }
         }

         #endregion

         public void btnSearch()
         {
             PatientTransactionList.PageIndex = 0;
             GetTransaction_ByPtID(0,PatientTransactionList.PageSize);
         }

         private void GetTransaction_ByPtID(int PageIndex,int PageSize)
         {
             SearchCriteria.IsNgoaiTru = IsNgoaiTru;
             this.ShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
             var t = new Thread(() =>
             {
                 try
                 {
                     using (var serviceFactory = new TransactionServiceClient())
                     {
                         var contract = serviceFactory.ServiceInstance;
                         contract.BeginGetTransaction_ByPtID(SearchCriteria, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                           {
                               try
                               {
                                   var results = contract.EndGetTransaction_ByPtID(out int Total, asyncResult);
                                   PatientTransactionList.Clear();
                                   PatientTransactionList.TotalItemCount = Total;
                                   if (results != null)
                                   {
                                       foreach (PatientTransaction p in results)
                                       {
                                           PatientTransactionList.Add(p);
                                       }
                                       NotifyOfPropertyChange(() => PatientTransactionList);
                                   }
                               }
                               catch (Exception ex)
                               {
                                   Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                               }
                               finally
                               {
                                   this.HideBusyIndicator();
                               }

                           }), null);
                     }
                 }
                 catch (Exception ex)
                 {
                     this.HideBusyIndicator();
                     Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                 }
             });

             t.Start();
         }

         public void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
         {
             e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
         }

         public void lnkTransactionID_Click(object sender, RoutedEventArgs e)
         {
             Button linkButton = e.OriginalSource as Button;
             if (linkButton.CommandParameter != null && (long)linkButton.CommandParameter > 0)
             {
                 //var proAlloc = Globals.GetViewModel<IDocumentPreview>();
                 //proAlloc.ID = (long)linkButton.CommandParameter;
                 //if (IsNgoaiTru)
                 //{
                 //    proAlloc.eItem = ReportName.TEMP38a;
                 //    if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                 //    {
                 //        proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                 //    }
                 //    else
                 //    {
                 //        proAlloc.StaffFullName = "";
                 //    }
                 //}
                 //else
                 //{
                 //    proAlloc.eItem = ReportName.TEMP38aNoiTru;
                 //}
               
                 //var instance = proAlloc as Conductor<object>;
                 //Globals.ShowDialog(instance, (o) => { });

                Action<IDocumentPreview> onInitDlg = (proAlloc) =>
                {
                    proAlloc.ID = (long)linkButton.CommandParameter;
                    if (IsNgoaiTru)
                    {
                        proAlloc.eItem = ReportName.TEMP38a;
                        if (Globals.ServerConfigSection.CommonItems.ShowLoginNameOnReport38)
                        {
                            proAlloc.StaffFullName = Globals.LoggedUserAccount.Staff.FullName;
                        }
                        else
                        {
                            proAlloc.StaffFullName = "";
                        }
                    }
                    else
                    {
                        proAlloc.eItem = ReportName.TEMP38aNoiTru;
                    }
                };
                GlobalsNAV.ShowDialog<IDocumentPreview>(onInitDlg);
            }
         }
         private void btnPrint(long ID)
         {
             Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
             var t = new Thread(() =>
             {
                 using (var serviceFactory = new ReportServiceClient())
                 {
                     var contract = serviceFactory.ServiceInstance;

                     contract.BeginGetTemplate38aInPdfFormat(ID,0, Globals.DispatchCallback((asyncResult) =>
                     {
                         try
                         {
                             var results = contract.EndGetTemplate38aInPdfFormat(asyncResult);
                             var printEvt = new ActiveXPrintEvt(this, PrinterType.IN_PHIEU, results, ActiveXPrintType.ByteArray);
                             Globals.EventAggregator.Publish(results);
                         }
                         catch (Exception ex)
                         {
                             Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                         }
                         finally
                         {
                             Globals.IsBusy = false;
                         }

                     }), null);

                 }

             });
             t.Start();
         }

         public void lnkPrint_Click(object sender, RoutedEventArgs e)
         {
             Button linkButton = e.OriginalSource as Button;
             if ((long)linkButton.CommandParameter > 0)
             {
                 btnPrint((long)linkButton.CommandParameter);
             }
         }


    }
}
