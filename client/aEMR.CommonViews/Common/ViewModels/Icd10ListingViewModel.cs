using System;
using System.ComponentModel.Composition;
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

namespace aEMR.Common.ViewModels
{
    [Export(typeof(Iicd10Listing)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class Icd10ListingViewModel : Conductor<object>, Iicd10Listing
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Icd10ListingViewModel(IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;

            AllItems = new PagedSortableCollectionView<DiseasesReference>();
            AllItems.OnRefresh += new EventHandler<RefreshEventArgs>(AllItems_OnRefresh);
            AllItems.PageSize = Globals.PageSize;
        }
        private byte type = 0;
        private string _searchString;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                NotifyOfPropertyChange(() => SearchString);
            }
        }

        void AllItems_OnRefresh(object sender, RefreshEventArgs e)
        {
            LoadIcd10Items(AllItems.PageIndex, AllItems.PageSize, false);
        }

        
        private DiseasesReference _selectedItem;

        public DiseasesReference SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
                Globals.EventAggregator.Publish(new ItemSelected<DiseasesReference>() { Item = _selectedItem,Source = this});
            }
        }

        private PagedSortableCollectionView<DiseasesReference> _allItems;

        public PagedSortableCollectionView<DiseasesReference> AllItems
        {
            get { return _allItems; }
            private set
            {
                _allItems = value;
                NotifyOfPropertyChange(() => AllItems);
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
        private void LoadIcd10Items(int pageIndex, int pageSize, bool bCountTotal)
        {
            var t = new Thread(() =>
            {
                IsLoading = true;
                AxErrorEventArgs error = null;
                try
                {
                    using (var serviceFactory = new CommonUtilsServiceClient())
                    {
                        var contract = serviceFactory.ServiceInstance;

                        //contract.BeginSearchRefDiseases(SearchString, 0, Globals.PageSize, type, Globals.DispatchCallback((asyncResult) =>
                        contract.BeginSearchRefDiseases(SearchString, pageIndex, pageSize, type
                            , 0
                            , Globals.GetCurServerDateTime()
                            , Globals.DispatchCallback((asyncResult) =>
                        {

                            try
                            {
                                int Total = 0;
                                var results = contract.EndSearchRefDiseases(out Total, asyncResult);

                                AllItems.Clear();
                                if (results != null)
                                {
                                    //if (bCountTotal)
                                    //{
                                    //    AllItems.TotalItemCount = totalCount;
                                    //}
                                    //Vi khong phan trang nen.
                                    //AllItems.TotalItemCount = results.Count;

                                    AllItems.TotalItemCount = Total;

                                    foreach (var item in results)
                                    {
                                        AllItems.Add(item);
                                    }
                                    var currentView = this.GetView() as IAutoCompleteView;
                                    if (currentView != null)
                                    {
                                        currentView.PopulateComplete();
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            finally
                            {

                            }

                        }), null);

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
        private string tempSearchString;
        public void StartSearching()
        {
            tempSearchString = _searchString;

            AllItems.PageIndex = 0;
            LoadIcd10Items(AllItems.PageIndex, AllItems.PageSize, true);
        }

        public void ClearItems()
        {
            AllItems.Clear();
            AllItems.TotalItemCount = 0;
        }
        public void SetText(string str)
        {
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                currentView.AutoCompleteBox.Text = str;
            }
        }
        public void PopulatingCmd(object source, PopulatingEventArgs eventArgs)
        {
            eventArgs.Cancel = true;
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                //SearchCriteria.BrandName = currentView.AutoCompleteBox.SearchText;
                SearchString = currentView.AutoCompleteBox.SearchText;
                StartSearching();
            }
        }
        public void Clear()
        {
            AllItems.Clear();
            var currentView = this.GetView() as IAutoCompleteView;
            if (currentView != null)
            {
                currentView.AutoCompleteBox.SelectedItem = null;
                currentView.AutoCompleteBox.Text = string.Empty;
            }
        }
    }
}
