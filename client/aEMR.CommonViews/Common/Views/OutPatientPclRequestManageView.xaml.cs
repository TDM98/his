using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using aEMR.ViewContracts.GuiContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    public partial class OutPatientPclRequestManageView : UserControl, IOutPatientPclRequestManageView
    {
        public OutPatientPclRequestManageView()
        {
            InitializeComponent();
            // TBLD
            //gridPCLRequests.UnloadingRowGroup += (s, e) => { Resize(); };
            //gridPCLRequests.LoadingRowGroup += (s, e) => { refreshUI = true; };

            this.Unloaded += new RoutedEventHandler(PclRequestItemsView_Unloaded);
            this.Loaded += new RoutedEventHandler(PclRequestItemsView_Loaded);
        }

        void PclRequestItemsView_Loaded(object sender, RoutedEventArgs e)
        {
            Binding binding = new Binding();
            binding.Source = this.DataContext;
            //binding.Path = new PropertyPath("PclServiceDetails");
            binding.Path = new PropertyPath("PclDetailsListView");
            binding.Mode = BindingMode.OneWay;

            gridPCLRequests.SetBinding(DataGrid.ItemsSourceProperty, binding);
            //gridPCLRequests.Width = LayoutRoot.ActualWidth;
        }

        void PclRequestItemsView_Unloaded(object sender, RoutedEventArgs e)
        {
            gridPCLRequests.SetValue(DataGrid.ItemsSourceProperty, null);
        }
        bool refreshUI = true;
        List<StackPanel> headers = null;
        DataGridColumnHeadersPresenter dghc = null;

        // TBLD commented out
        void Resize()
        {
            // TxD 06/07/2018 Commented out Resize to see if it's actually required
            //if (dghc != null)
            //{
            //    headers = gridPCLRequests.GetChildrenByType<StackPanel>().Where(x => x.Name == "ghsp").ToList();
            //    headers.ForEach(x =>
            //    {
            //        //Span cell dau tien tren 2 cot.
            //        double w = 0.0;
            //        if (dghc.Children[1]. == Visibility.Visible)
            //        {
            //            dghc.Items.
            //            w += dghc.Children[1].RenderSize.Width;
            //        }
            //        if (dghc.Children[2].Visibility == Visibility.Visible)
            //        {
            //            w += dghc.Children[2].RenderSize.Width;
            //        }
            //        if (dghc.Children[3].Visibility == Visibility.Visible)
            //        {
            //            w += dghc.Children[3].RenderSize.Width;
            //        }
            //        if (dghc.Children[4].Visibility == Visibility.Visible)
            //        {
            //            w += dghc.Children[4].RenderSize.Width;
            //        }
            //        (x.Children[0] as DataGridCell).Width = w + dghc.Children[5].RenderSize.Width;
            //        //(x.Children[0] as DataGridCell).Width = dghc.Children[1].RenderSize.Width + dghc.Children[2].RenderSize.Width + dghc.Children[3].RenderSize.Width;
            //        if ((x.Children[0] as DataGridCell).Width > 5)
            //        {
            //            (x.Children[0] as DataGridCell).Width -= 5; //Chieu rong cua IndentSpacer. Tam thoi de vay.   
            //        }
            //        (x.Children[1] as DataGridCell).Width = 0;
            //        (x.Children[2] as DataGridCell).Width = 0;
            //        (x.Children[3] as DataGridCell).Width = 0;

            //        try
            //        {
            //            int i = 0;
            //            for (i = 6; i < dghc.Children.Count - 1; i++)
            //            {
            //                (x.Children[i - 1] as DataGridCell).Width = dghc.Children[i].RenderSize.Width;
            //                (x.Children[i - 1] as DataGridCell).Visibility = dghc.Children[i].Visibility;
            //            }
            //        }
            //        catch (Exception)
            //        {
            //        }
            //    });
            //}
        }
        private void gridPCLRequests_LayoutUpdated(object sender, EventArgs e)
        {
            //if (refreshUI && headers == null)
            //{
            //    dghc = gridPCLRequests.GetChildrenByType<DataGridColumnHeadersPresenter>().FirstOrDefault();
            //    if (dghc != null)
            //    {
            //        foreach (DataGridColumnHeader dgch in dghc.Items)
            //        {
            //            dgch.SizeChanged += (s, args) => { Resize(); };
            //        }
            //    }
            //}
            //if (refreshUI)
            //    Resize();

            //refreshUI = false;
        }

        private void gridPCLRequests_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString() + ". ";
        }

        public bool ShowCheckBoxColumn
        {
            get { return (bool)GetValue(ShowCheckBoxColumnProperty); }
            set
            {
                SetValue(ShowCheckBoxColumnProperty, value);
            }
        }

        public static readonly DependencyProperty ShowCheckBoxColumnProperty = DependencyProperty.Register(
            "ShowCheckBoxColumn",
            typeof(bool),
            typeof(OutPatientPclRequestManageView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnShowCheckBoxColumnChanged)));

        private static void OnShowCheckBoxColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                ((OutPatientPclRequestManageView)d).gridPCLRequests.Columns[0].Visibility = Visibility.Visible;
            }
            else
            {
                ((OutPatientPclRequestManageView)d).gridPCLRequests.Columns[0].Visibility = Visibility.Collapsed;
            }
        }

        public bool ShowDeleteColumn
        {
            get { return (bool)GetValue(ShowDeleteColumnProperty); }
            set
            {
                SetValue(ShowDeleteColumnProperty, value);
            }
        }

        public static readonly DependencyProperty ShowDeleteColumnProperty = DependencyProperty.Register(
            "ShowDeleteColumn",
            typeof(bool),
            typeof(OutPatientPclRequestManageView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnShowDeleteColumnPropertyChanged)));

        private static void OnShowDeleteColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                ((OutPatientPclRequestManageView)d).gridPCLRequests.Columns[1].Visibility = Visibility.Visible;
            }
            else
            {
                ((OutPatientPclRequestManageView)d).gridPCLRequests.Columns[1].Visibility = Visibility.Collapsed;
            }
        }
        private void SetVisibilityBindingForColumn(string colName)
        {
            Binding binding = new Binding();
            binding.Source = this.DataContext;
            binding.Path = new PropertyPath("HiServiceBeingUsed");
            binding.Mode = BindingMode.OneWay;
            binding.Converter = Application.Current.Resources["VisibilityConverter"] as IValueConverter;

            AxDataGridTextColumn col = gridPCLRequests.GetColumnByName(colName) as AxDataGridTextColumn;
            if (col != null)
            {
                col.VisibilityBinding = binding;
            }
        }
        public void SetVisibilityBindingForHiColumns()
        {
            SetVisibilityBindingForColumn("colHiAllowedPrice");
            SetVisibilityBindingForColumn("colPriceDifference");
            SetVisibilityBindingForColumn("colHiPay");
            SetVisibilityBindingForColumn("colCoPay");
        }


        private void DatagridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var myDataGrid = (DataGrid)sender;

            if (myDataGrid.RenderSize.Width == 0) 
                return;

            double totalActualWidthOfNonStarColumns = myDataGrid.Columns.Sum(c => c.Width.IsStar ? 0 : c.ActualWidth);

            double totalDesiredWidthOfStarColumns = myDataGrid.Columns.Sum(c => c.Width.IsStar ? c.Width.Value : 0);

            if (totalDesiredWidthOfStarColumns == 0)
                return;  // No star columns

            // Space available to fill ( -18 Standard vScrollbar)
            double spaceAvailable = (myDataGrid.RenderSize.Width - 18) - totalActualWidthOfNonStarColumns;

            double inIncrementsOf = spaceAvailable / totalDesiredWidthOfStarColumns;

            foreach (var column in myDataGrid.Columns)
            {
                if (!column.Width.IsStar) continue;

                var width = inIncrementsOf * column.Width.Value;
                column.Width = new DataGridLength(width, DataGridLengthUnitType.Star);
            }
        }
    }
}
