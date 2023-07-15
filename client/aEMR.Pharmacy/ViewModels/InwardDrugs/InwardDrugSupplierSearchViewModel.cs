using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows.Input;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using DataEntities;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Common.Collections;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using aEMR.Common.BaseModel;
using eHCMSLanguage;
using System.Linq;
using aEMR.Common.Utilities;
using System.Windows;
using aEMR.Common;
using aEMR.Controls;

/*
 * 20220516 #001 DatTB: Chuyển box tìm NCC từ KeyEnabledComboBox thành AxAutoComplete giống khoa dược
 * 
 */

namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IInwardDrugSupplierSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardDrugSupplierSearchViewModel : ViewModelBase, IInwardDrugSupplierSearch
        , IHandle<PharmacySupplierToEstimationEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public InwardDrugSupplierSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            SearchCriteria = new InwardInvoiceSearchCriteria();
            InwardInvoiceList = new PagedSortableCollectionView<InwardDrugInvoice>();
            InwardInvoiceList.OnRefresh += InwardInvoiceList_OnRefresh;
            InwardInvoiceList.PageSize = 20;

            SearchSupplierAuto();

            //GetAllSupplierCbx();
        }

        void InwardInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchInwardInvoiceDrug(InwardInvoiceList.PageIndex,InwardInvoiceList.PageSize);
        }

        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            SearchCriteria = null;
            InwardInvoiceList = null;
            SuppliersSearch = null;
        }

        #region Properties Member
        public long? TypID { get; set; }

        int supplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;

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

        private ObservableCollection<Supplier> _suppliersSearch;
        public ObservableCollection<Supplier> SuppliersSearch
        {
            get
            {
                return _suppliersSearch;
            }
            set
            {
                if (_suppliersSearch != value)
                {
                    _suppliersSearch = value;
                    NotifyOfPropertyChange(()=>SuppliersSearch);
                }
            }
        }

        private string _pageTitle;
        public string pageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    NotifyOfPropertyChange(() => pageTitle);
                }
            }
        }

        private const string ALLITEMS = "[All]";

        //▼====: #001
        private ObservableCollection<Supplier> _Suppliers;
        public ObservableCollection<Supplier> Suppliers
        {
            get
            {
                return _Suppliers;
            }
            set
            {
                if (_Suppliers != value)
                {
                    _Suppliers = value;
                }
                NotifyOfPropertyChange(() => Suppliers);
            }
        }

        private Supplier _SelectedSupplier;
        public Supplier SelectedSupplier
        {
            get
            {
                return _SelectedSupplier;
            }
            set
            {
                if (_SelectedSupplier != value)
                {
                    _SelectedSupplier = value;
                }
                NotifyOfPropertyChange(() => SelectedSupplier);
            }
        }

        public void Supplier_Populating(object sender, PopulatingEventArgs e)
        {
            AutoCompleteBox AutoSupplier = sender as AutoCompleteBox;
            if (AutoSupplier != null && Suppliers != null)
            {
                AutoSupplier.ItemsSource = Suppliers.Where(x => StringUtil.RemoveSign4VietnameseString(x.SupplierName).ToUpper().Contains(StringUtil.RemoveSign4VietnameseString(e.Parameter.ToUpper())) || (x.SupplierCode != null && x.SupplierCode.ToUpper().Contains(e.Parameter.ToUpper())));
                AutoSupplier.PopulateComplete();
            }
        }

        private void SearchSupplierAuto()
        {
            this.ShowBusyIndicator();
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetSupplier_ByPCOIDNotPaging(null, (long)AllLookupValues.V_SupplierType.CUNGCAP_THIETBI_YTE, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSupplier_ByPCOIDNotPaging(asyncResult);
                            Suppliers = results.ToObservableCollection();
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

            });

            t.Start();
        }

        public void btnSupplier(object sender, RoutedEventArgs e)
        {
            void onInitDlg(ISuppliers proAlloc)
            {
                proAlloc.IsChildWindow = true;
                proAlloc.ePharmacySupplierEvent = eFirePharmacySupplierEvent.EstimationPharmacy;
            }
            GlobalsNAV.ShowDialog<ISuppliers>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());

        }

        public void Handle(PharmacySupplierToEstimationEvent message)
        {
            if (message != null && this.IsActive)
            {
                SelectedSupplier = message.SelectedSupplier as Supplier;
            }
        }
        //▲==== #001
        #endregion

        private void SearchInwardInvoiceDrug(int PageIndex,int PageSize)
        {
            //▼==== #001
            if (SearchCriteria == null)
            {
                return;
            }
            
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);

            if (SelectedSupplier != null)
            {
                SearchCriteria.SupplierID = SelectedSupplier.SupplierID;
            }
            else
            {
                SearchCriteria.SupplierID = null;
            }
            //▲==== #001

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
                                 int Totalcount;
                                 var results = contract.EndSearchInwardInvoiceDrug(out Totalcount, asyncResult);
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
                                 _logger.Info(ex.Message);
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

        public void btnSearch()
        {
            InwardInvoiceList.PageIndex = 0;
            SearchInwardInvoiceDrug(0, InwardInvoiceList.PageSize);
        }

        public void InwardID_KeyUp(object sender,KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria == null)
                {
                    SearchCriteria = new InwardInvoiceSearchCriteria();
                }
                SearchCriteria.InwardID = (sender as TextBox).Text;
                btnSearch();
            }
        }

        //InvoiceNumber_KeyUp
        public void InvoiceNumber_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria == null)
                {
                    SearchCriteria = new InwardInvoiceSearchCriteria();
                }
                SearchCriteria.InvoiceNumber = (sender as TextBox).Text;
                btnSearch();
            }
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            Globals.EventAggregator.Publish(new PharmacyCloseSearchInwardIncoiceEvent {SelectedInwardInvoice=e.Value });
        }
    }
}
