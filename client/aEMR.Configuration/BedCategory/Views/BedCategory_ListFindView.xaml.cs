using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.BedCategory.Views
{
    [Export(typeof(BedCategory_ListFindView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BedCategory_ListFindView : UserControl
    {
        public BedCategory_ListFindView()
        {
            InitializeComponent();
            //this.Loaded += new RoutedEventHandler(PtBedAllocationsView_Loaded);
            //this.Unloaded += new RoutedEventHandler(PtBedAllocationsView_Unloaded);
        }

        public event EventHandler DblClick;

        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null)
            {
                DblClick(this, null);
            }
        }

        //void PtBedAllocationsView_Unloaded(object sender, RoutedEventArgs e)
        //{

        //}

        //void PtBedAllocationsView_Loaded(object sender, RoutedEventArgs e)
        //{

        //}
        private void dtgList_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //DataEntities.MedServiceItemPrice objRows = e.Row.DataContext as DataEntities.MedServiceItemPrice;
            //if (objRows != null)
            //{
            //    switch (objRows.PriceType)
            //    {
            //        case "PriceCurrent":
            //            {
            //                e.Row.Foreground = new SolidColorBrush(Colors.Green);
            //                break;
            //            }
            //        case "PriceFuture-Active-1":
            //            {
            //                e.Row.Foreground = new SolidColorBrush(Colors.Gray);
            //                break;
            //            }
            //            //case "PriceOld":
            //            //    {
            //            //        e.Row.Foreground = new SolidColorBrush(Colors.Orange);
            //            //        break;
            //            //    }
            //    }
            //}
        }
    }
}
