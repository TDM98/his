using System.Windows;
using System.Windows.Controls;

namespace aEMR.Pharmacy.Views
{
    public partial class ChooseBatchNumberVisitorView : UserControl
    {
        public ChooseBatchNumberVisitorView()
        {
            InitializeComponent();
        }

        void grdRequestDetails_UnLoaded(object sender, RoutedEventArgs e)
        {
            grdRequestDetails.SetValue(DataGrid.ItemsSourceProperty,null);
        }
    }
}
