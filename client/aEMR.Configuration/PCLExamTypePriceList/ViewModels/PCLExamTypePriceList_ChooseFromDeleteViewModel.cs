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

namespace aEMR.Configuration.PCLExamTypePriceList.ViewModels
{
    [Export(typeof(IPCLExamTypePriceList_ChooseFromDelete)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PCLExamTypePriceList_ChooseFromDeleteViewModel : Conductor<object>, IPCLExamTypePriceList_ChooseFromDelete
    {
        [ImportingConstructor]
        public PCLExamTypePriceList_ChooseFromDeleteViewModel(IWindsorContainer container, INavigationService navigationService, IEventAggregator eventArg) { }
        protected override void OnActivate()
        {
            base.OnActivate();

            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete = new PagedSortableCollectionView<DataEntities.PCLExamType>();
            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.OnRefresh += new System.EventHandler<RefreshEventArgs>(ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete_OnRefresh);

            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.TotalItemCount = ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.Count;
            PagingLinq(0, ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.PageSize);
        }

        void ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete_OnRefresh(object sender, RefreshEventArgs e)
        {
            PagingLinq(ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.PageSize);
        }

        private PagedSortableCollectionView<DataEntities.PCLExamType> _ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete;
        public PagedSortableCollectionView<DataEntities.PCLExamType> ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete);
            }
        }

        private ObservableCollection<DataEntities.PCLExamType> _ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete;
        public ObservableCollection<DataEntities.PCLExamType> ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete
        {
            get { return _ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete; }
            set
            {
                _ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete = value;
                NotifyOfPropertyChange(() => ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete);
            }
        }

        private void PagingLinq(int pIndex, int pPageSize)
        {
            var ResultAll = from p in ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.ToObservableCollection()
                            select p;
            List<DataEntities.PCLExamType> Items = ResultAll.Skip(pIndex * pPageSize).Take(pPageSize).ToList();
            ShowItemsOnList(Items);
        }

        private void ShowItemsOnList(List<DataEntities.PCLExamType> ObjCollect)
        {
            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.Clear();
            foreach (DataEntities.PCLExamType item in ObjCollect)
            {
                ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.Add(item);
            }
        }

        public void DoubleClick(object args)
        {
            EventArgs<object> eventArgs = args as EventArgs<object>;

            DataEntities.PCLExamType ItemSelect = (eventArgs.Value as DataEntities.PCLExamType);

            Globals.EventAggregator.Publish(new SelectedObjectEvent<DataEntities.PCLExamType>() { Result = ItemSelect });

            RemoveItem(ItemSelect);

        }

        private void RemoveItem(DataEntities.PCLExamType p)
        {
            ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.Remove(p);

            ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.TotalItemCount = ObjPCLExamTypesAndPriceIsActive_ByPCLGroupID_All_Virtual_Delete.Count;
            PagingLinq(ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.PageIndex, ObjPCLExamTypesAndPriceIsActive_Paging_Virtual_Delete.PageSize);

        }

        public void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataEntities.PCLExamType objRows = e.Row.DataContext as DataEntities.PCLExamType;
            if (objRows != null)
            {
                switch (objRows.ObjPCLExamTypePrice.PriceType)
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
