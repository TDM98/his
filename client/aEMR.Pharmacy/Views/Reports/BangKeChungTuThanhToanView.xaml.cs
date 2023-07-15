using System.Windows;
using System.Windows.Controls;

namespace aEMR.Pharmacy.Views
{
    public partial class BangKeChungTuThanhToanView : UserControl
    {
        public BangKeChungTuThanhToanView()
        {
            InitializeComponent();
        }

        private void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SetValue(DataGrid.ItemsSourceProperty,null);
        }

     
    }
}
