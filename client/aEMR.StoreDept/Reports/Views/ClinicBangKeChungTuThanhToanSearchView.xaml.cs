using System.Windows;
using System.Windows.Controls;

namespace aEMR.StoreDept.Reports.Views
{
    public partial class ClinicBangKeChungTuThanhToanSearchView : UserControl
    {
        public ClinicBangKeChungTuThanhToanSearchView()
        {
            InitializeComponent();
        }

        private void dataGrid1_Unloaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.SetValue(DataGrid.ItemsSourceProperty, null);
        }

    }
}
