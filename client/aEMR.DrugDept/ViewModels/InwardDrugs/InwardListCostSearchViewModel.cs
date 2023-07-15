using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using DataEntities;
using System.Linq;
using aEMR.ViewContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.Infrastructure.CachingUtils;
using aEMR.ServiceClient;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Common.Utilities;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IInwardListCostSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardListCostSearchViewModel : Conductor<object>, IInwardListCostSearch
        , IHandle<DrugDeptCloseSearchSupplierEvent_V2>
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
        public InwardListCostSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);

            SearchCriteria = new InwardInvoiceSearchCriteria();
            InwardInvoiceList = new PagedSortableCollectionView<CostTableMedDept>();
            InwardInvoiceList.OnRefresh += InwardInvoiceList_OnRefresh;
            InwardInvoiceList.PageSize = Globals.PageSize;

            SearchSupplierAuto();
            //GetAllSupplierCbx();
        }

        void InwardInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchInwardInvoiceDrug(InwardInvoiceList.PageIndex,InwardInvoiceList.PageSize);
        }

        #region Properties Member


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

        private PagedSortableCollectionView<CostTableMedDept> _inwardinvoicelist;
        public PagedSortableCollectionView<CostTableMedDept> InwardInvoiceList
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

        //private ObservableCollection<Supplier> _suppliersSearch;
        //public ObservableCollection<Supplier> SuppliersSearch
        //{
        //    get
        //    {
        //        return _suppliersSearch;
        //    }
        //    set
        //    {
        //        if (_suppliersSearch != value)
        //        {
        //            _suppliersSearch = value;
        //            NotifyOfPropertyChange(()=>SuppliersSearch);
        //        }
        //    }
        //}

        private const string ALLITEMS = "[All]";

        #endregion

        //private void GetAllSupplierCbx()
        //{
        //   // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    var t = new Thread(() =>
        //    {
        //        using (var serviceFactory = new PharmacySuppliersServiceClient())
        //        {
        //            var contract = serviceFactory.ServiceInstance;
        //            contract.BeginGetAllSupplierCbx(supplierType, Globals.DispatchCallback((asyncResult) =>
        //            {

        //                try
        //                {
        //                    var results = contract.EndGetAllSupplierCbx(asyncResult);
        //                    SuppliersSearch = results.ToObservableCollection();
        //                    Supplier p = new Supplier();
        //                    p.SupplierID = 0;
        //                    p.SupplierName = ALLITEMS;
        //                    SuppliersSearch.Insert(0,p);

        //                }
        //                catch (Exception ex)
        //                {
        //                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
        //                }
        //                finally
        //                {
        //                    //Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        private void SearchInwardInvoiceDrug(int PageIndex,int PageSize)
        {
            if (SearchCriteria == null)
            {
                return;
            }
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            IsLoading = true;

            if (SelectedSupplier != null)
            {
                SearchCriteria.SupplierID = SelectedSupplier.SupplierID;
            }
            else
            {
                SearchCriteria.SupplierID = null;
            }

            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyMedDeptServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginCostTableMedDept_Search(SearchCriteria,V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Totalcount;
                            var results = contract.EndCostTableMedDept_Search(out Totalcount, asyncResult);
                            InwardInvoiceList.Clear();
                            InwardInvoiceList.TotalItemCount = Totalcount;
                            if (results != null)
                            {
                                foreach (CostTableMedDept p in results)
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
                            IsLoading = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            InwardInvoiceList.PageIndex = 0;
            SearchInwardInvoiceDrug(0, InwardInvoiceList.PageSize);
        }

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchInwardCostListEvent { SelectedInwardInvoice = e.Value });
        }

        //KMx: Copy từ trang Đặt hàng (20/12/2014 17:24).
        private ObservableCollection<DrugDeptSupplier> _Suppliers;
        public ObservableCollection<DrugDeptSupplier> Suppliers
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

        private DrugDeptSupplier _SelectedSupplier;
        public DrugDeptSupplier SelectedSupplier
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
                    contract.BeginGetSupplierDrugDept_ByPCOIDNotPaging(null, (long)AllLookupValues.V_SupplierType.CUNGCAP_THIETBI_YTE, Globals.DispatchCallback((asyncResult) =>
                    {
                        try
                        {
                            var results = contract.EndGetSupplierDrugDept_ByPCOIDNotPaging(asyncResult);
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
            //var proAlloc = Globals.GetViewModel<ISupplierProduct>();
            //proAlloc.IsChildWindow = true;
            //proAlloc.LeftModule = LeftModuleActive.KHOADUOC_PHANBOPHI_TIMPHIEUPHANBO;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
                proAlloc.LeftModule = LeftModuleActive.KHOADUOC_PHANBOPHI_TIMPHIEUPHANBO;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());
        }

        public void Handle(DrugDeptCloseSearchSupplierEvent_V2 message)
        {
            if (message != null && this.IsActive)
            {
                SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
            }
        }
    }
}
