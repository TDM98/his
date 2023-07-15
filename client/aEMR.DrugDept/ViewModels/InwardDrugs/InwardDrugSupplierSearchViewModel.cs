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
using System.Linq;
using aEMR.Common;
using aEMR.Common.Utilities;
using aEMR.Controls;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptInwardDrugSupplierSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class InwardDrugSupplierSearchViewModel : ViewModelBase, IDrugDeptInwardDrugSupplierSearch
        , IHandle<DrugDeptCloseSearchSupplierEvent_V1>
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
        public InwardDrugSupplierSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            eventAggregator.Subscribe(this);
            SearchCriteria = new InwardInvoiceSearchCriteria();
            InwardInvoiceList = new PagedSortableCollectionView<InwardDrugMedDeptInvoice>();
            InwardInvoiceList.OnRefresh += InwardInvoiceList_OnRefresh;
            InwardInvoiceList.PageSize = 20;

            SearchSupplierAuto();
            //KMx: Đây là Khoa Dược, không dùng combobox chọn NCC của nhà thuốc (19/12/2014 10:46).
            //GetAllSupplierCbx();
        }

        void InwardInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchInwardInvoiceDrug(InwardInvoiceList.PageIndex,InwardInvoiceList.PageSize);
        }

        #region Properties Member

        private long? _TypID;
        public long? TypID
        {
            get 
            {
                return _TypID;
            }
            set
            { 
                _TypID = value;
                NotifyOfPropertyChange(() => TypID);
                NotifyOfPropertyChange(() => ShowSupplier);
            }
        }

        public bool ShowSupplier
        {
            get
            {
                return TypID.GetValueOrDefault(0) == (long)AllLookupValues.RefOutputType.NHAP_TU_NCC;
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

        private PagedSortableCollectionView<InwardDrugMedDeptInvoice> _inwardinvoicelist;
        public PagedSortableCollectionView<InwardDrugMedDeptInvoice> InwardInvoiceList
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

        private long _V_MedProductType = (long)AllLookupValues.MedProductType.THUOC;
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
        private const string ALLITEMS = "[All]";

        #endregion

        //KMx: Đây là Khoa Dược, không dùng combobox chọn NCC của nhà thuốc (19/12/2014 10:46).
        //private void GetAllSupplierCbx()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
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
        //                    Globals.IsBusy = false;
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
            
            IsLoading = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);

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
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardDrugMedDeptInvoice(SearchCriteria, TypID, V_MedProductType, PageIndex, PageSize, true, IsConsignment, Globals.DispatchCallback((asyncResult) =>
                          {

                              try
                              {
                                  int Totalcount;
                                  var results = contract.EndSearchInwardDrugMedDeptInvoice(out Totalcount, asyncResult);
                                  InwardInvoiceList.Clear();
                                  InwardInvoiceList.TotalItemCount = Totalcount;
                                  if (results != null)
                                  {
                                      foreach (InwardDrugMedDeptInvoice p in results)
                                      {
                                          InwardInvoiceList.Add(p);
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
                                  //Globals.IsBusy = false;
                                  this.DlgHideBusyIndicator();
                              }

                          }), null);

                    }
                }
                catch(Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
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

        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            TryClose();
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchInwardIncoiceEvent {SelectedInwardInvoice=e.Value });
        }

        public void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InwardID = (sender as TextBox).Text;
                }
                btnSearch();
            }

        }

        public void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchCriteria != null)
                {
                    SearchCriteria.InvoiceNumber = (sender as TextBox).Text;
                }
                btnSearch();
            }
        }
        public void KeyEnabledComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null)
            {
                btnSearch();
            }
        }


        //KMx: Copy từ trang Đặt hàng (18/12/2014 17:24).
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
            //proAlloc.LeftModule = LeftModuleActive.KHOADUOC_NHAPHANG_TIMPHIEUNHAP;
            //var instance = proAlloc as Conductor<object>;
            //Globals.ShowDialog(instance, (o) => { });

            void onInitDlg(ISupplierProduct proAlloc)
            {
                proAlloc.IsChildWindow = true;
                proAlloc.LeftModule = LeftModuleActive.KHOADUOC_NHAPHANG_TIMPHIEUNHAP;
            }
            GlobalsNAV.ShowDialog<ISupplierProduct>(onInitDlg, null, false, true, Globals.GetDefaultDialogViewSize());

        }

        public void Handle(DrugDeptCloseSearchSupplierEvent_V1 message)
        {
            if (message != null && this.IsActive)
            {
                SelectedSupplier = message.SelectedSupplier as DrugDeptSupplier;
            }
        }


        public void GridInwardInvoice_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid == null)
            {
                return;
            }

            var colSupplierName = grid.GetColumnByName("colSupplierName");

            if (colSupplierName == null)
            {
                return;
            }

            if (TypID.GetValueOrDefault(0) == (long)AllLookupValues.RefOutputType.NHAP_TU_NCC)
            {
                colSupplierName.Visibility = Visibility.Visible;
            }
            else
            {
                colSupplierName.Visibility = Visibility.Collapsed;
            }
        }

        private bool _IsConsignment;
        public bool IsConsignment
        {
            get
            {
                return _IsConsignment;
            }
            set
            {
                _IsConsignment = value;
                NotifyOfPropertyChange(() => IsConsignment);
            }
        }
    }
}
