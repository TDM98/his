using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using aEMR.ViewContracts.GuiContracts;
using aEMR.Controls;
namespace aEMR.Common.Views
{
    public partial class OutPatientServiceManageView : UserControl, IOutPatientServiceManageView
    {
        public OutPatientServiceManageView()
        {
            InitializeComponent();
            //gridServices.UnloadingRowGroup += (s, e) => { Resize(); };
            //gridServices.LoadingRowGroup += (s, e) => { refreshUI = true; };

            this.Loaded += new RoutedEventHandler(RegisteredServicesView_Loaded);
            this.Unloaded += new RoutedEventHandler(RegisteredServicesView_Unloaded);
        }

        void RegisteredServicesView_Unloaded(object sender, RoutedEventArgs e)
        {
            gridServices.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void RegisteredServicesView_Loaded(object sender, RoutedEventArgs e)
        {
            Binding binding = new Binding();
            binding.Source = this.DataContext;
            binding.Path = new PropertyPath("CV_RegDetailItems");
            binding.Mode = BindingMode.OneWay;
            gridServices.SetBinding(DataGrid.ItemsSourceProperty, binding);
        }
        bool refreshUI = true;
        List<StackPanel> headers = null;
        DataGridColumnHeadersPresenter dghc = null;

        //private void gridServices_LayoutUpdated(object sender, EventArgs e)
        //{
        //    if (refreshUI && headers == null)
        //    {
        //        dghc = gridServices.GetChildrenByType<DataGridColumnHeadersPresenter>().FirstOrDefault();
        //        if (dghc != null)
        //        {
        //            foreach (DataGridColumnHeader dgch in dghc.Children)
        //            {
        //                dgch.SizeChanged += (s, args) => { Resize(); };
        //            }
        //        }
        //    }
        //    if (refreshUI)
        //        Resize();

        //    refreshUI = false;
        //}

        //void Resize()
        //{
        //    if (dghc != null)
        //    {
        //        headers = gridServices.GetChildrenByType<StackPanel>().Where(x => x.Name == "ghsp").ToList();
        //        headers.ForEach(x =>
        //        {
        //            //Span cell dau tien tren 3 cot.
        //            double w = 0.0;
        //            if (dghc.Children[1].Visibility == Visibility.Visible)
        //            {
        //                w += dghc.Children[1].RenderSize.Width;
        //            }
        //            if (dghc.Children[2].Visibility == Visibility.Visible)
        //            {
        //                w += dghc.Children[2].RenderSize.Width;
        //            }
        //            if (dghc.Children[3].Visibility == Visibility.Visible)
        //            {
        //                w += dghc.Children[3].RenderSize.Width;
        //            }
        //            (x.Children[0] as DataGridCell).Width = w + dghc.Children[4].RenderSize.Width;
        //            //(x.Children[0] as DataGridCell).Width = dghc.Children[1].RenderSize.Width + dghc.Children[2].RenderSize.Width + dghc.Children[3].RenderSize.Width;
        //            (x.Children[1] as DataGridCell).Width = 0;
        //            (x.Children[2] as DataGridCell).Width = 0;
        //            (x.Children[3] as DataGridCell).Width = 0;

        //            int i = 0;
        //            for (i = 5; i < dghc.Children.Count - 1; i++)
        //            {
        //                (x.Children[i - 1] as DataGridCell).Width = dghc.Children[i].RenderSize.Width;
        //                (x.Children[i - 1] as DataGridCell).Visibility = dghc.Children[i].Visibility;
        //            }
        //        });
        //    }
        //}

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
            typeof(OutPatientServiceManageView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnShowCheckBoxColumnChanged)));

        private static void OnShowCheckBoxColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                ((OutPatientServiceManageView)d).gridServices.Columns[0].Visibility = Visibility.Visible;
            }
            else
            {
                ((OutPatientServiceManageView)d).gridServices.Columns[0].Visibility = Visibility.Collapsed;
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
            typeof(OutPatientServiceManageView),
            new PropertyMetadata(true, new PropertyChangedCallback(OnShowDeleteColumnPropertyChanged)));

        private static void OnShowDeleteColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool visible = (bool)e.NewValue;
            if (visible)
            {
                ((OutPatientServiceManageView)d).gridServices.Columns[1].Visibility = Visibility.Visible;
            }
            else
            {
                ((OutPatientServiceManageView)d).gridServices.Columns[1].Visibility = Visibility.Collapsed;
            }
        }

        private void SetVisibilityBindingForColumn(string colName)
        {
            Binding binding = new Binding();
            binding.Source = this.DataContext;
            binding.Path = new PropertyPath("HiServiceBeingUsed");
            binding.Mode = BindingMode.OneWay;
            binding.Converter = Application.Current.Resources["VisibilityConverter"] as IValueConverter;

            AxDataGridTextColumn col = gridServices.GetColumnByName(colName) as AxDataGridTextColumn;
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
    }
}