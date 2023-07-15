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
using System.Linq;
using aEMR.Common.BaseModel;

namespace aEMR.Pharmacy.ViewModels.InwardDrugs
{
    [Export(typeof(IInwardFromInternalExportSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardFromInternalExportSearchViewModel : ViewModelBase, IInwardFromInternalExportSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InwardFromInternalExportSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new InwardInvoiceSearchCriteria();
            InwardInvoiceList = new PagedSortableCollectionView<InwardDrugInvoice>();
            InwardInvoiceList.OnRefresh += InwardInvoiceList_OnRefresh;
            InwardInvoiceList.PageSize = Globals.PageSize;
        }

        void InwardInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchInwardInvoiceDrug(InwardInvoiceList.PageIndex,InwardInvoiceList.PageSize);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Globals.EventAggregator.Subscribe(this);
        }

        protected override void OnDeactivate(bool close)
        {
            SearchCriteria = null;
            InwardInvoiceList = null;
            Globals.EventAggregator.Unsubscribe(this);
        }

        public long? TypID { get; set; }
        private InwardInvoiceSearchCriteria _searchCriteria;
        public InwardInvoiceSearchCriteria SearchCriteria
        {
            get
            {
                return _searchCriteria;
            }
            set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(()=>SearchCriteria);
                }
            }
        }

        private PagedSortableCollectionView<InwardDrugInvoice> _inwardinvoicelist;
        public PagedSortableCollectionView<InwardDrugInvoice> InwardInvoiceList
        {
            get
            {
                return _inwardinvoicelist;
            }
            set
            {
                if (_inwardinvoicelist != value)
                {
                    _inwardinvoicelist = value;
                    NotifyOfPropertyChange(()=>InwardInvoiceList);
                }
            }
        }   

        private void SearchInwardInvoiceDrug(int PageIndex, int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }

            if (!Globals.isAccountCheck)
            {
                SearchCriteria.InDeptID = 0;
            }
            else
            {
                if (Globals.LoggedUserAccount.DeptIDResponsibilityList == null || Globals.LoggedUserAccount.DeptIDResponsibilityList.Count <= 0)
                {
                    MessageBox.Show(eHCMSResources.A0107_G1_Msg_InfoKhTheTimKiem, eHCMSResources.G0442_G1_TBao, MessageBoxButton.OK);
                    return;
                }
                SearchCriteria.InDeptID = Globals.LoggedUserAccount.DeptIDResponsibilityList.FirstOrDefault();
            }
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardInvoiceDrug(SearchCriteria, TypID, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                        {
                            try
                            {
                                var results = contract.EndSearchInwardInvoiceDrug(out int Totalcount, asyncResult);
                                InwardInvoiceList.Clear();
                                InwardInvoiceList.TotalItemCount = Totalcount;
                                if (results != null)
                                {
                                    foreach (InwardDrugInvoice p in results)
                                    {
                                        InwardInvoiceList.Add(p);
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

        public void BtnSearch(object sender, RoutedEventArgs e)
        {
            InwardInvoiceList.PageIndex = 0;
            SearchInwardInvoiceDrug(0, InwardInvoiceList.PageSize);
        }

        public void DataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {     
            Globals.EventAggregator.Publish(new PharmacyCloseSearchInwardIncoiceEvent { SelectedInwardInvoice=e.Value });
            TryClose();
        }
    }
}
