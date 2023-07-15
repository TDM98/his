using System.Windows.Controls;

namespace aEMR.Pharmacy.Views
{
    public partial class PurchaseOrderView : UserControl
    {
        public PurchaseOrderView()
        {
            InitializeComponent();
        }

        private void GridSuppliers_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GridSuppliers.SetValue(DataGrid.ItemsSourceProperty,null);
        }

        private void AxTextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
