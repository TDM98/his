using eHCMSLanguage;
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
using System.Collections.Generic;


namespace aEMR.Pharmacy.ViewModels
{
     [Export(typeof(IBangKeChungTuThanhToanSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BangKeChungTuThanhToanSearchViewModel : Conductor<object>, IBangKeChungTuThanhToanSearch
    {
        #region Indicator Member

        private bool _isLoadingSupplier = false;
        public bool isLoadingSupplier
        {
            get { return _isLoadingSupplier; }
            set
            {
                if (_isLoadingSupplier != value)
                {
                    _isLoadingSupplier = value;
                    NotifyOfPropertyChange(() => isLoadingSupplier);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        private bool _isLoadingSearch = false;
        public bool isLoadingSearch
        {
            get { return _isLoadingSearch; }
            set
            {
                if (_isLoadingSearch != value)
                {
                    _isLoadingSearch = value;
                    NotifyOfPropertyChange(() => isLoadingSearch);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }


        private bool _isLoadingStatus = false;
        public bool isLoadingStatus
        {
            get { return _isLoadingStatus; }
            set
            {
                if (_isLoadingStatus != value)
                {
                    _isLoadingStatus = value;
                    NotifyOfPropertyChange(() => isLoadingStatus);
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }

        public bool IsLoading
        {
            get { return (isLoadingSupplier || isLoadingSearch || isLoadingStatus); }
        }

        #endregion
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public BangKeChungTuThanhToanSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();
            GetStatus();
            GetAllSupplierCbx();

            SupplierPharmacyPaymentReqList = new PagedSortableCollectionView<SupplierPharmacyPaymentReqs>();
            SupplierPharmacyPaymentReqList.OnRefresh += SupplierPharmacyPaymentReqList_OnRefresh;
            SupplierPharmacyPaymentReqList.PageSize = Globals.PageSize;
        }

        void SupplierPharmacyPaymentReqList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierPharmacyPaymentReqs(SupplierPharmacyPaymentReqList.PageIndex, SupplierPharmacyPaymentReqList.PageSize);
        }
        protected override void OnDeactivate(bool close)
        {
            //base.OnDeactivate(close);
            SearchCriteria = null;
            SupplierPharmacyPaymentReqList = null;
            SuppliersSearch = null;
            PaymentReqStatus = null;
        }

        #region Properties member

        private PagedSortableCollectionView<SupplierPharmacyPaymentReqs> _SupplierPharmacyPaymentReqList;
        public PagedSortableCollectionView<SupplierPharmacyPaymentReqs> SupplierPharmacyPaymentReqList
        {
            
            get
            {
                return _SupplierPharmacyPaymentReqList;
            }
            set
            {
                if (_SupplierPharmacyPaymentReqList != value)
                {
                    _SupplierPharmacyPaymentReqList = value;
                    NotifyOfPropertyChange(() => SupplierPharmacyPaymentReqList);
                }
            }
        }

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
                    NotifyOfPropertyChange(() => SuppliersSearch);
                }
            }
        }

        int supplierType = (int)AllLookupValues.SupplierType.CUNG_CAP_THIET_BI_Y_TE;

        private const string ALLITEMS = "[ALL]";

        IList<Lookup> _PaymentReqStatus;
        public IList<Lookup> PaymentReqStatus
        {
            get
            {
                return _PaymentReqStatus;
            }
            set
            {
                if (_PaymentReqStatus != value)
                {
                    _PaymentReqStatus = value;
                    NotifyOfPropertyChange(() => PaymentReqStatus);
                }
            }
        }
        #endregion

        private void GetStatus()
        {
            isLoadingStatus = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new CommonService_V2Client())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllLookupValuesByType(LookupValues.PAYMENT_REQ_STATUS, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            PaymentReqStatus = contract.EndGetAllLookupValuesByType(asyncResult);
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingStatus = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        private void GetAllSupplierCbx()
        {
            isLoadingSupplier = true;
           // Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginGetAllSupplierCbx(supplierType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndGetAllSupplierCbx(asyncResult);
                            SuppliersSearch = results.ToObservableCollection();
                            Supplier p = new Supplier();
                            p.SupplierID = 0;
                            p.SupplierName = ALLITEMS;
                            SuppliersSearch.Insert(0, p);

                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSupplier = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }

        public void btnSearch(object sender, RoutedEventArgs e)
        {
            SupplierPharmacyPaymentReqList.PageIndex = 0;
            SearchSupplierPharmacyPaymentReqs(SupplierPharmacyPaymentReqList.PageIndex, SupplierPharmacyPaymentReqList.PageSize);
        }

        public void SearchSupplierPharmacyPaymentReqs(int PageIndex, int PageSize)
        {
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacyInwardDrugServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginSupplierPharmacyPaymentReqs_Search(SearchCriteria,  PageSize,PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            int Total = 0;
                            var results = contract.EndSupplierPharmacyPaymentReqs_Search(out Total, asyncResult);
                            SupplierPharmacyPaymentReqList.Clear();
                            SupplierPharmacyPaymentReqList.TotalItemCount = Total;
                            if (results != null)
                            {
                                foreach (SupplierPharmacyPaymentReqs p in results)
                                {
                                    SupplierPharmacyPaymentReqList.Add(p);
                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            Globals.ShowMessage(ex.Message, eHCMSResources.T0432_G1_Error);
                        }
                        finally
                        {
                            isLoadingSearch = false;
                            //Globals.IsBusy = false;
                        }

                    }), null);

                }

            });

            t.Start();
        }


        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            Globals.EventAggregator.Publish(new PharmacyCloseSearchSupplierPharmacyPaymentReqEvent { SelectedPaymentReq = e.Value });
            TryClose();
            //phat su kien 
        }
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SupplierPharmacyPaymentReqList.PageIndex = 0;
                SearchSupplierPharmacyPaymentReqs(SupplierPharmacyPaymentReqList.PageIndex, SupplierPharmacyPaymentReqList.PageSize);
            }
        }
    }
}
