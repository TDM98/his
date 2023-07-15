using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;

namespace aEMR.DrugDept.Views
{
    public partial class InwardDrugSupplierSearchView : UserControl
    {
        public InwardDrugSupplierSearchView()
        {
            InitializeComponent();
        }

        private void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            GridInwardInvoice.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        private void dataPager1_Unloaded(object sender, RoutedEventArgs e)
        {
            dataPager1.SetValue(DataPager.SourceProperty,null);
        }


    }
}
