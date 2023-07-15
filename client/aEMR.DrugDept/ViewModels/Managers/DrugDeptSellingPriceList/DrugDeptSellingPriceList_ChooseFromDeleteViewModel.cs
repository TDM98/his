using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts;
using Caliburn.Micro;
using aEMR.Common.Collections;
using aEMR.Common;
using Castle.Windsor;

namespace eHCMS.DrugDept.ViewModels
{
    [Export(typeof(IDrugDeptSellingPriceList_ChooseFromDelete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DrugDeptSellingPriceList_ChooseFromDeleteViewModel : Conductor<object>, IDrugDeptSellingPriceList_ChooseFromDelete
    {
        [ImportingConstructor]
        public DrugDeptSellingPriceList_ChooseFromDeleteViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg)
        {

        }
        protected override void OnActivate()
        {
            base.OnActivate();

            ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging = new PagedSortableCollectionView<DataEntities.DrugDeptSellingItemPrices>();
            ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.OnRefresh += new System.EventHandler<RefreshEventArgs>(ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging_OnRefresh);

            ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.TotalItemCount = ObjDrugDeptSellingItemPrices_All_Virtual_Delete.Count;
            PagingLinq(0, ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.PageSize);
        }

        void ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging_OnRefresh(object sender, RefreshEventArgs e)
        {
            PagingLinq(ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.PageIndex, ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.PageSize);
        }

        private PagedSortableCollectionView<DataEntities.DrugDeptSellingItemPrices> _ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging;
        public PagedSortableCollectionView<DataEntities.DrugDeptSellingItemPrices> ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging
        {
            get { return _ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging; }
            set
            {
                _ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging);
            }
        }

        private ObservableCollection<DataEntities.DrugDeptSellingItemPrices> _ObjDrugDeptSellingItemPrices_All_Virtual_Delete;
        public ObservableCollection<DataEntities.DrugDeptSellingItemPrices> ObjDrugDeptSellingItemPrices_All_Virtual_Delete
        {
            get { return _ObjDrugDeptSellingItemPrices_All_Virtual_Delete; }
            set
            {
                _ObjDrugDeptSellingItemPrices_All_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjDrugDeptSellingItemPrices_All_Virtual_Delete);
            }
        }

        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in ObjDrugDeptSellingItemPrices_All_Virtual_Delete.ToObservableCollection()
                            select p;
            List<DataEntities.DrugDeptSellingItemPrices> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<DataEntities.DrugDeptSellingItemPrices> ObjCollect)
        {
            ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.Clear();
            foreach (DataEntities.DrugDeptSellingItemPrices item in ObjCollect)
            {
                ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.Add(item);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            DataEntities.DrugDeptSellingItemPrices ItemSelect = (eventArgs.Value as DataEntities.DrugDeptSellingItemPrices);

            Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.DrugDeptSellingItemPrices>() { Result = ItemSelect });

            RemoveItem(ItemSelect);
        }

        private void RemoveItem(DataEntities.DrugDeptSellingItemPrices p)
        {
            ObjDrugDeptSellingItemPrices_All_Virtual_Delete.Remove(p);

            ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.TotalItemCount = ObjDrugDeptSellingItemPrices_All_Virtual_Delete.Count;
            PagingLinq(ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.PageIndex, ObjDrugDeptSellingItemPrices_All_Virtual_Delete_Paging.PageSize);
        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.DrugDeptSellingItemPrices objRows = e.Row.DataContext as DataEntities.DrugDeptSellingItemPrices;
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
