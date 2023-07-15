using System;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common.Converters;
using DataEntities;

namespace aEMR.DrugDept.Views
{
    public partial class InwardVPPMedDeptSupplierView : UserControl
    {
        public InwardVPPMedDeptSupplierView()
        {
            InitializeComponent();
        }

       private void GridSuppliers_Unloaded(object sender, RoutedEventArgs e)
        {
            aEMR.Common.Converters.DecimalConverter2 aa = new aEMR.Common.Converters.DecimalConverter2();

            GridSuppliers.SetValue(DataGrid.ItemsSourceProperty,null);
        }
        private void GridInwardDrug_Unloaded(object sender, RoutedEventArgs e)
        {
            GridInwardDrug.SetValue(DataGrid.ItemsSourceProperty,null);
        }

        private void cbxCurrency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DecimalConverter value = (DecimalConverter)this.Resources["DecimalConverter"];
            if (Convert.ToInt64(cbxCurrency.SelectedValue) != (long)AllLookupValues.CurrencyTable.VND)
            {
                value.IsVND = false;
            }
            else
            {
                value.IsVND = true;
            }
        }
    }
}
