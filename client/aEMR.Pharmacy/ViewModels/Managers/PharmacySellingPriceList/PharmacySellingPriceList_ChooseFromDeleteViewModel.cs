using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using aEMR.DataContracts;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ServiceClient;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using DataEntities;
using aEMR.Common.Collections;
using aEMR.Common;
using aEMR.Infrastructure.CachingUtils;
using Castle.Core.Logging;
using Castle.Windsor;


namespace aEMR.Pharmacy.ViewModels
{
    [Export(typeof(IPharmacySellingPriceList_ChooseFromDelete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PharmacySellingPriceList_ChooseFromDeleteViewModel : Conductor<object>, IPharmacySellingPriceList_ChooseFromDelete
    {
        private readonly INavigationService _navigationService;
        private readonly ISalePosCaching _salePosCaching;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PharmacySellingPriceList_ChooseFromDeleteViewModel (IWindsorContainer container, INavigationService navigationService, ISalePosCaching salePosCaching)
        {
            _navigationService = navigationService;
            _logger = container.Resolve<ILogger>();
            _salePosCaching = salePosCaching;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging = new PagedSortableCollectionView<DataEntities.PharmacySellingItemPrices>();
            ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.OnRefresh += new System.EventHandler<RefreshEventArgs>(ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging_OnRefresh);

            ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.TotalItemCount = ObjPharmacySellingItemPrices_All_Virtual_Delete.Count;
            PagingLinq(0, ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.PageSize);
        }

        void ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PagingLinq(ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.PageIndex, ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.PageSize);
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

        private PagedSortableCollectionView<DataEntities.PharmacySellingItemPrices> _ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging;
        public PagedSortableCollectionView<DataEntities.PharmacySellingItemPrices> ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging
        {
            get { return _ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging; }
            set
            {
                _ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging);
            }
        }

        private ObservableCollection<DataEntities.PharmacySellingItemPrices> _ObjPharmacySellingItemPrices_All_Virtual_Delete;
        public ObservableCollection<DataEntities.PharmacySellingItemPrices> ObjPharmacySellingItemPrices_All_Virtual_Delete
        {
            get { return _ObjPharmacySellingItemPrices_All_Virtual_Delete; }
            set
            {
                _ObjPharmacySellingItemPrices_All_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjPharmacySellingItemPrices_All_Virtual_Delete);
            }
        }

        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in ObjPharmacySellingItemPrices_All_Virtual_Delete.ToObservableCollection()
                            select p;
            List<DataEntities.PharmacySellingItemPrices> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<DataEntities.PharmacySellingItemPrices> ObjCollect)
        {
            ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.Clear();
            foreach (DataEntities.PharmacySellingItemPrices item in ObjCollect)
            {
                ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.Add(item);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            DataEntities.PharmacySellingItemPrices ItemSelect = (eventArgs.Value as DataEntities.PharmacySellingItemPrices);

            Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.PharmacySellingItemPrices>() { Result = ItemSelect });

            RemoveItem(ItemSelect);
        }

        private void RemoveItem(DataEntities.PharmacySellingItemPrices p)
        {
            ObjPharmacySellingItemPrices_All_Virtual_Delete.Remove(p);

            ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.TotalItemCount = ObjPharmacySellingItemPrices_All_Virtual_Delete.Count;
            PagingLinq(ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.PageIndex, ObjPharmacySellingItemPrices_All_Virtual_Delete_Paging.PageSize);
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.PharmacySellingItemPrices objRows = e.Row.DataContext as DataEntities.PharmacySellingItemPrices;
            if (objRows != null)
            {
                switch (objRows.PriceType)
                {
                    case "PriceCurrent":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Green);
                            break;
                        }
                    case "PriceFuture-Active-1":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                            break;
                        }
                    case "PriceFuture-Active-0":
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Blue);
                            break;
                        }
                    default:
                        {
                            e.Row.Foreground = new SolidColorBrush(Colors.Black);
                            break;
                        }
                }
            }
        }

        public void btClose()
        {
            TryClose();
        }

    }
}
