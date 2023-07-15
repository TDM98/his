using System.Windows.Controls;

namespace aEMR.DrugDept.Views
{
    public partial class DrugDeptPurchaseOrderView : UserControl
    {
        public DrugDeptPurchaseOrderView()
        {
            InitializeComponent();
        }

        private void GridSuppliers_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GridSuppliers.SetValue(DataGrid.ItemsSourceProperty,null);
        }
    }
}
