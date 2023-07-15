/*
 * 201470803 #001 CMN: Add HI Store Service
*/
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
    public partial class OutPatientDrugManageView : UserControl, IOutPatientDrugManageView
    {
        public OutPatientDrugManageView()
        {
            InitializeComponent();
            //gridDrugs.UnloadingRowGroup += (s, e) => { Resize(); };
            //gridDrugs.LoadingRowGroup += (s, e) => { refreshUI = true; };

            this.Loaded += new RoutedEventHandler(OutPatientDrugManageView_Loaded);
            this.Unloaded += new RoutedEventHandler(OutPatientDrugManageView_Unloaded);
        }

        void OutPatientDrugManageView_Unloaded(object sender, RoutedEventArgs e)
        {
            gridDrugs.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void OutPatientDrugManageView_Loaded(object sender, RoutedEventArgs e)
        {
            Binding binding = new Binding();
            binding.Source = this.DataContext;
            binding.Path = new PropertyPath("DrugItems");
            binding.Mode = BindingMode.OneWay;

            gridDrugs.SetBinding(DataGrid.ItemsSourceProperty, binding);
        }

        bool refreshUI = true;
        List<StackPanel> headers = null;
        DataGridColumnHeadersPresenter dghc = null;

        //void Resize()
        //{
        //    if (dghc != null)
        //    {
        //        headers = gridDrugs.GetChildrenByType<StackPanel>().Where(x => x.Name == "ghsp").ToList();
        //        headers.ForEach(x =>
        //        {
        //            for (int i = 1; i < dghc.Children.Count - 1; i++)
        //            {
        //                try
        //                {
        //                    (x.Children[i - 1] as DataGridCell).Width = dghc.Children[i].RenderSize.Width;
        //                    (x.Children[i - 1] as DataGridCell).Visibility = dghc.Children[i].Visibility;
        //                }
        //                catch (ArgumentOutOfRangeException)
        //                {
        //                }
        //            }

        //            //(x.Children.Last() as DataGridCell).Width = dghc.Children.Last().RenderSize.Width;
        //            //(x.Children.Last() as DataGridCell).Visibility = dghc.Children.Last().Visibility;
        //        });
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object o = gridDrugs.DataContext;
        }

        //private void gridDrugs_LayoutUpdated(object sender, EventArgs e)
        //{
        //    if (refreshUI && headers == null)
        //    {
        //        dghc = gridDrugs.GetChildrenByType<DataGridColumnHeadersPresenter>().FirstOrDefault();
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
        //==== #001
        private void SetVisibilityBindingForColumn(string colName, string PropertyPath = "HiServiceBeingUsed", bool Revert = false)
        {
            Binding binding = new Binding();
            binding.Source = this.DataContext;
            binding.Path = new PropertyPath(PropertyPath);
            binding.Mode = BindingMode.OneWay;
            if (Revert)
                binding.Converter = Application.Current.Resources["VisibilityElseConverter"] as IValueConverter;
            else
                binding.Converter = Application.Current.Resources["VisibilityConverter"] as IValueConverter;
            AxDataGridTextColumn col = gridDrugs.GetColumnByName(colName) as AxDataGridTextColumn;
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
            //SetVisibilityBindingForColumn("colCoPay");
        }
        public void SetVisibilityBindingForHIOutPtColumns()
        {
            SetVisibilityBindingForColumn("colContent", "IsHIOutPt");
            SetVisibilityBindingForColumn("colUnitName", "IsHIOutPt");
            SetVisibilityBindingForColumn("colInvoice", "IsHIOutPt", true);
            SetVisibilityBindingForColumn("colBatchNumber", "IsHIOutPt");
            SetVisibilityBindingForColumn("colHiAllowedPrice", "IsHIOutPt");
            SetVisibilityBindingForColumn("colHiPay", "IsHIOutPt");
            //SetVisibilityBindingForColumn("colCoPay", "IsHIOutPt");
            SetVisibilityBindingForColumn("colQty", "IsHIOutPt", true);
            SetVisibilityBindingForColumn("colQtyOffer", "IsHIOutPt");
            SetVisibilityBindingForColumn("colOutQuantity", "IsHIOutPt");
            SetVisibilityBindingForColumn("colExpiry", "IsHIOutPt");
        }
        //==== #001
    }
}
