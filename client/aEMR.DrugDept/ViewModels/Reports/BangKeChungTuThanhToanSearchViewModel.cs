﻿using System.ComponentModel.Composition;
using aEMR.ViewContracts;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Input;
using System;
using aEMR.Infrastructure;
using System.Collections.ObjectModel;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using System.Threading;
using DataEntities;
using aEMR.Common.Collections;
using System.Linq;
using System.Collections.Generic;
using eHCMSLanguage;
using aEMR.Common.BaseModel;

namespace aEMR.DrugDept.ViewModels
{
     [Export(typeof(IDrugDeptBangKeChungTuThanhToanSearch)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BangKeChungTuThanhToanSearchViewModel : ViewModelBase, IDrugDeptBangKeChungTuThanhToanSearch
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

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

        [ImportingConstructor]
        public BangKeChungTuThanhToanSearchViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            SearchCriteria = new RequestSearchCriteria();
            GetStatus();
            GetAllSupplierCbx();

            SupplierDrugDeptPaymentReqList = new PagedSortableCollectionView<SupplierDrugDeptPaymentReqs>();
            SupplierDrugDeptPaymentReqList.OnRefresh += SupplierDrugDeptPaymentReqList_OnRefresh;
            SupplierDrugDeptPaymentReqList.PageSize = Globals.PageSize;
        }

        void SupplierDrugDeptPaymentReqList_OnRefresh(object sender, RefreshEventArgs e)
        {
            SearchSupplierDrugDeptPaymentReqs(SupplierDrugDeptPaymentReqList.PageIndex, SupplierDrugDeptPaymentReqList.PageSize);
        }

        #region Properties member

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

        private PagedSortableCollectionView<SupplierDrugDeptPaymentReqs> _SupplierDrugDeptPaymentReqList;
        public PagedSortableCollectionView<SupplierDrugDeptPaymentReqs> SupplierDrugDeptPaymentReqList
        {
            
            get
            {
                return _SupplierDrugDeptPaymentReqList;
            }
            set
            {
                if (_SupplierDrugDeptPaymentReqList != value)
                {
                    _SupplierDrugDeptPaymentReqList = value;
                    NotifyOfPropertyChange(() => SupplierDrugDeptPaymentReqList);
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

        private ObservableCollection<DrugDeptSupplier> _suppliersSearch;
        public ObservableCollection<DrugDeptSupplier> SuppliersSearch
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

        private const string ALLITEMS = "[All]";

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
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            var t = new Thread(() =>
            {
                using (var serviceFactory = new PharmacySuppliersServiceClient())
                {
                    var contract = serviceFactory.ServiceInstance;
                    contract.BeginDrugDeptSupplier_GetCbx(supplierType, Globals.DispatchCallback((asyncResult) =>
                    {

                        try
                        {
                            var results = contract.EndDrugDeptSupplier_GetCbx(asyncResult);
                            SuppliersSearch = results.ToObservableCollection();
                            DrugDeptSupplier p = new DrugDeptSupplier();
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
            SupplierDrugDeptPaymentReqList.PageIndex = 0;
            SearchSupplierDrugDeptPaymentReqs(SupplierDrugDeptPaymentReqList.PageIndex, SupplierDrugDeptPaymentReqList.PageSize);
        }

        public void SearchSupplierDrugDeptPaymentReqs(int PageIndex, int PageSize)
        {
            isLoadingSearch = true;
            //Globals.EventAggregator.Publish(new BusyEvent { IsBusy = true, Message = eHCMSResources.Z0125_G1_DangXuLi });
            this.DlgShowBusyIndicator(eHCMSResources.Z0125_G1_DangXuLi);
            var t = new Thread(() =>
            {
                try
                {
                    using (var serviceFactory = new PharmacyMedDeptServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;
                        contract.BeginSupplierDrugDeptPaymentReqs_Search(SearchCriteria, V_MedProductType, PageSize, PageIndex, true, Globals.DispatchCallback((asyncResult) =>
                         {

                             try
                             {
                                 int Total = 0;
                                 var results = contract.EndSupplierDrugDeptPaymentReqs_Search(out Total, asyncResult);
                                 SupplierDrugDeptPaymentReqList.Clear();
                                 SupplierDrugDeptPaymentReqList.TotalItemCount = Total;
                                 if (results != null)
                                 {
                                     foreach (SupplierDrugDeptPaymentReqs p in results)
                                     {
                                         SupplierDrugDeptPaymentReqList.Add(p);
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
                                 isLoadingSearch = false;
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


        public void dataGrid1_DblClick(object sender, Common.EventArgs<object> e)
        {
            Globals.EventAggregator.Publish(new DrugDeptCloseSearchSupplierDrugDeptPaymentReqEvent { SelectedPaymentReq = e.Value });
            TryClose();
            //phat su kien 
        }
        public void Search_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SupplierDrugDeptPaymentReqList.PageIndex = 0;
                SearchSupplierDrugDeptPaymentReqs(SupplierDrugDeptPaymentReqList.PageIndex, SupplierDrugDeptPaymentReqList.PageSize);
            }
        }
    }
}
