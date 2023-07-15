using System.Windows;
using System.Windows.Controls;

namespace aEMR.DrugDept.Views
{
    public partial class BangKeChungTuThanhToanSearchView : UserControl
    {
        public BangKeChungTuThanhToanSearchView()
        {
            InitializeComponent();
        }

        private void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SetValue(DataGrid.ItemsSourceProperty,null);
        }
    }
}
