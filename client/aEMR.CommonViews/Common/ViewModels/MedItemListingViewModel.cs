using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.Infrastructure.CachingUtils;
using Castle.Windsor;
using Castle.Core.Logging;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using eHCMSLanguage;

namespace aEMR.Common.ViewModels
{
    [Export(typeof (IMedItemListing)),PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedItemListingViewModel : Conductor<object>, IMedItemListing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MedItemListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            MedProductList = new PagedSortableCollectionView<RefGenMedProductDetails>();
            MedProductList.OnRefresh += new EventHandler<RefreshEventArgs>(MedItemList_OnRefresh);

            _searchCriteria = new MedProductSearchCriteria();
            _searchCriteria.MedProductType = AllLookupValues.MedProductType.Y_CU;
        }

        void MedItemList_OnRefresh(object sender, RefreshEventArgs e)
        {
            LoadMedItems(MedProductList.PageIndex, MedProductList.PageSize, false);
        }

        private MedProductSearchCriteria _criteria;
        private MedProductSearchCriteria _searchCriteria;
        public MedProductSearchCriteria SearchCriteria
        {
            get { return _searchCriteria; }
            private set
            {
                if (_searchCriteria != value)
                {
                    _searchCriteria = value;
                    NotifyOfPropertyChange(() => SearchCriteria);
                }
            }
        }


        private RefGenMedProductDetails _selectedMedProduct;

        public RefGenMedProductDetails SelectedMedProduct
        {
            get { return _selectedMedProduct; }
            set
            {
                _selectedMedProduct = value;
                NotifyOfPropertyChange(() => SelectedMedProduct);
                //KMx: Không được tự động chọn (26/07/2014 15:18)
                //Globals.EventAggregator.Publish(new ItemSelected<RefGenMedProductDetails>() { Item = _selectedMedProduct });
            }
        }

        private PagedSortableCollectionView<RefGenMedProductDetails> _medProductList;

        public PagedSortableCollectionView<RefGenMedProductDetails> MedProductList
        {
            get { return _medProductList; }
            private set
            {
                _medProductList = value;
                NotifyOfPropertyChange(() => MedProductList);
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (!_isLoading)
                {
                    _isLoading = value;
                    NotifyOfPropertyChange(() => IsLoading);
                }
            }
        }
        private void LoadMedItems(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                if (_criteria == null)
                {
                    _criteria = _searchCriteria;
                }
                IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new PatientRegistrationServiceClient())
                    {
                        var client = serviceFactory.ServiceInstance;
                        client.BeginSearchMedProducts(_criteria, pageIndex, pageSize, bCountTotal, Globals.DispatchCallback((asyncResult) =>
                        {
                            int totalCount = 0;
                            IList<RefGenMedProductDetails> allItems = null;
                            bool bOK = false;
                            try
                            {
                                allItems = client.EndSearchMedProducts(out totalCount, asyncResult);
                                bOK = true;
                            }
                            catch (FaultException<AxException> fault)
                            {
                                error = new AxErrorEventArgs(fault);
                            }
                            catch (Exception ex)
                            {
                                error = new AxErrorEventArgs(ex);
                            }

                            MedProductList.Clear();
                            if (bOK)
                            {
                                if (bCountTotal)
                                {
                                    MedProductList.TotalItemCount = totalCount;
                                }
                                if (allItems != null)
                                {
                                    var store = asyncResult.AsyncState as RefStorageWarehouseLocation;
                                    foreach (var item in allItems)
                                    {
                                        if (store != null)
                                        {
                                            item.Storage = store;
                                            item.StoreID = store.StoreID;
                                        }

                                        MedProductList.Add(item);
                                    }
                                }
                                var currentView = this.GetView() as IAutoCompleteView;
                                if (currentView != null)
                                {
                                    currentView.PopulateComplete();
                                }
                            }
                        }), _criteria.Storage)
                            ;
                    }
                }
                catch (Exception ex)
                {
                    error = new AxErrorEventArgs(ex);
                }
                finally
                {
                    IsLoading = false;
                    //Globals.IsBusy = false;
                }
                if (error != null)
                {
                    Globals.EventAggregator.Publish(new ErrorOccurred() { CurrentError = error });
                }
            });
            t.Start();
        }

        public void StartSearching()
        {
            _criteria = _searchCriteria.DeepCopy();

            MedProductList.PageIndex = 0;
            LoadMedItems(MedProductList.PageIndex, MedProductList.PageSize, true);
        }

        public void ClearItems()
        {
            MedProductList.Clear();
            MedProductList.TotalItemCount = 0;
        }

        public void PopulatingCmd(object source, PopulatingEventArgs eventArgs)
        {
            if (SearchCriteria.StoreID <= 0)
            {
                MessageBox.Show(string.Format("{0}.", eHCMSResources.Z1031_G1_ChonKhoTruocKhiTimDSYCu));
                return;
            }
            eventArgs.Cancel = true;
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                SearchCriteria.BrandName = currentView.AutoCompleteBox.SearchText;
                StartSearching();
            }
        }
        public void Clear()
        {
            MedProductList.Clear();
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                currentView.AutoCompleteBox.SelectedItem = null;
                currentView.AutoCompleteBox.Text = string.Empty;
            }
        }
    }
}