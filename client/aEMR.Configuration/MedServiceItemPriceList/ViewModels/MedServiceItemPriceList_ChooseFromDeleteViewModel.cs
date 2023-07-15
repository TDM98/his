using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Infrastructure;
using aEMR.Infrastructure.Events;
using aEMR.ViewContracts.Configuration;
using Caliburn.Micro;
using aEMR.Common.Collections;
using aEMR.Common;
using Castle.Windsor;
using aEMR.ViewContracts;

namespace aEMR.Configuration.MedServiceItemPriceList.ViewModels
{
    [Export(typeof(IMedServiceItemPriceList_ChooseFromDelete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MedServiceItemPriceList_ChooseFromDeleteViewModel : Conductor<object>, IMedServiceItemPriceList_ChooseFromDelete
    {
        [ImportingConstructor]
        public MedServiceItemPriceList_ChooseFromDeleteViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg) { }
        protected override void OnActivate()
        {
            base.OnActivate();

            ObjGetDeptMedServiceItems_Paging_Virtual_Delete=new PagedSortableCollectionView<DataEntities.MedServiceItemPrice>();
            ObjGetDeptMedServiceItems_Paging_Virtual_Delete.OnRefresh += new System.EventHandler<RefreshEventArgs>(ObjGetDeptMedServiceItems_Paging_Virtual_Delete_OnRefresh);

            ObjGetDeptMedServiceItems_Paging_Virtual_Delete.TotalItemCount = ObjGetDeptMedServiceItems_All_Virtual_Delete.Count;
            PagingLinq(0, ObjGetDeptMedServiceItems_Paging_Virtual_Delete.PageSize);
        }

        void ObjGetDeptMedServiceItems_Paging_Virtual_Delete_OnRefresh(object sender, RefreshEventArgs e)
        {
            PagingLinq(ObjGetDeptMedServiceItems_Paging_Virtual_Delete.PageIndex,ObjGetDeptMedServiceItems_Paging_Virtual_Delete.PageSize);
        }

        private PagedSortableCollectionView<DataEntities.MedServiceItemPrice> _ObjGetDeptMedServiceItems_Paging_Virtual_Delete;
        public PagedSortableCollectionView<DataEntities.MedServiceItemPrice> ObjGetDeptMedServiceItems_Paging_Virtual_Delete
        {
            get { return _ObjGetDeptMedServiceItems_Paging_Virtual_Delete; }
            set
            {
                _ObjGetDeptMedServiceItems_Paging_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems_Paging_Virtual_Delete);
            }
        }

        private ObservableCollection<DataEntities.MedServiceItemPrice> _ObjGetDeptMedServiceItems_All_Virtual_Delete;
        public ObservableCollection<DataEntities.MedServiceItemPrice> ObjGetDeptMedServiceItems_All_Virtual_Delete
        {
            get { return _ObjGetDeptMedServiceItems_All_Virtual_Delete; }
            set
            {
                _ObjGetDeptMedServiceItems_All_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjGetDeptMedServiceItems_All_Virtual_Delete);
            }
        }

        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in ObjGetDeptMedServiceItems_All_Virtual_Delete.ToObservableCollection()
                            select p;
            List<DataEntities.MedServiceItemPrice> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<DataEntities.MedServiceItemPrice> ObjCollect)
        {
            ObjGetDeptMedServiceItems_Paging_Virtual_Delete.Clear();
            foreach (DataEntities.MedServiceItemPrice item in ObjCollect)
            {
                ObjGetDeptMedServiceItems_Paging_Virtual_Delete.Add(item);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            DataEntities.MedServiceItemPrice ItemSelect = (eventArgs.Value as DataEntities.MedServiceItemPrice);

            Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.MedServiceItemPrice>() { Result = ItemSelect });

            RemoveItem(ItemSelect);

        }

        private void RemoveItem(DataEntities.MedServiceItemPrice p)
        {
            ObjGetDeptMedServiceItems_All_Virtual_Delete.Remove(p);

            ObjGetDeptMedServiceItems_Paging_Virtual_Delete.TotalItemCount = ObjGetDeptMedServiceItems_All_Virtual_Delete.Count;
            PagingLinq(ObjGetDeptMedServiceItems_Paging_Virtual_Delete.PageIndex, ObjGetDeptMedServiceItems_Paging_Virtual_Delete.PageSize);

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.MedServiceItemPrice objRows = e.Row.DataContext as DataEntities.MedServiceItemPrice;
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
                }
            }
        }

        public void btClose()
        {
            TryClose();
        }

    }
}
