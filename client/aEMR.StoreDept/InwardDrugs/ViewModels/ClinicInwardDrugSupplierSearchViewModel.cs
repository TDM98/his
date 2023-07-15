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
/*
 * 20190805 #001 TNHX: [BM0013084] Fix can't find InwardClinicDept after import. Get DeptID base on loginview
 */
namespace aEMR.StoreDept.InwardDrugs.ViewModels
{
    [Export(typeof(IStoreDeptClinicInwardDrugSupplierSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreDeptClinicInwardDrugSupplierSearchViewModel : ViewModelBase, IStoreDeptClinicInwardDrugSupplierSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public StoreDeptClinicInwardDrugSupplierSearchViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventAggregator, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new InwardInvoiceSearchCriteria();
            InwardInvoiceList = new PagedSortableCollectionView<InwardDrugClinicDeptInvoice>();
            InwardInvoiceList.OnRefresh += InwardInvoiceList_OnRefresh;
            InwardInvoiceList.PageSize = 20;

            //GetAllSupplierCbx();
        }

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

        void InwardInvoiceList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchInwardInvoiceDrug(InwardInvoiceList.PageIndex,InwardInvoiceList.PageSize);
        }
        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            SearchCriteria = null;
            InwardInvoiceList = null;
            //SuppliersSearch = null;
        }
        #region Properties Member

        private long? _TypID;
        public long? TypID
        {
            get { return _TypID; }
            set { _TypID = value; }
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

        private PagedSortableCollectionView<InwardDrugClinicDeptInvoice> _inwardinvoicelist;
        public PagedSortableCollectionView<InwardDrugClinicDeptInvoice> InwardInvoiceList
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

        //private void GetAllSupplierCbx()
        //{
        //    Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
        //    IsLoading = true;
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
        //                    IsLoading = false;
        //                    Globals.IsBusy = false;
        //                }

        //            }), null);

        //        }

        //    });

        //    t.Start();
        //}

        private void SearchInwardInvoiceDrug(int PageIndex,int PageSize)
        {
            //KMx: Hiện tại không có thời gian làm combobox cho user chọn khoa, nên mặc định lấy khoa trách nhiệm đầu tiên của user để tìm kiếm.
            //Khi nào có thời gian thì làm combobox chọn khoa (21/01/2015 14:18).
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
                //▼===: #001
                SearchCriteria.InDeptID = Globals.DeptLocation != null ? Globals.DeptLocation.DeptID : Globals.LoggedUserAccount.DeptIDResponsibilityList.FirstOrDefault();
                //▲===: #001
            }

            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            IsLoading = true;
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyClinicDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSearchInwardDrugClinicDeptInvoice(SearchCriteria, TypID, V_MedProductType, PageIndex, PageSize, true, Globals.DispatchCallback((asyncResult) =>
                          {

                              try
                              {
                                  int Totalcount;
                                  var results = contract.EndSearchInwardDrugClinicDeptInvoice(out Totalcount, asyncResult);
                                  InwardInvoiceList.Clear();
                                  InwardInvoiceList.TotalItemCount = Totalcount;
                                  if (results != null)
                                  {
                                      foreach (InwardDrugClinicDeptInvoice p in results)
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
                catch (Exception ex)
                {
                    Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                    _logger.Error(ex.Message);
                    this.DlgHideBusyIndicator();
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
            Globals.EventAggregator.Publish(new ClinicDrugDeptCloseSearchInwardIncoiceEvent {SelectedInwardInvoice=e.Value });
            TryClose();
        }
    }
}
