using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;


namespace aEMR.Pharmacy.Views
{
    public partial class InwardDrugSupplierSearchView : UserControl
    {
        public InwardDrugSupplierSearchView()
        {
            InitializeComponent();
        }

        private void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        private void dataPager1_Unloaded(object sender, RoutedEventArgs e)
        {
            dataPager1.SetValue(DataPager.SourceProperty,null);
        }
    }
}
