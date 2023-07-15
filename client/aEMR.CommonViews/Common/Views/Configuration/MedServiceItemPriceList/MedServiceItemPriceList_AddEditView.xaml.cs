using System.Windows;
using System.Windows.Controls;
namespace aEMR.Configuration.MedServiceItemPriceList.Views
{
    public partial class MedServiceItemPriceList_AddEditView : UserControl
    {
        public MedServiceItemPriceList_AddEditView()
        {
            InitializeComponent();

        }
        private void dtgList_Loaded(object sender, RoutedEventArgs e)
        {
            dtgList.Columns[0].Visibility = ((aEMR.Configuration.MedServiceItemPriceList.ViewModels.MedServiceItemPriceList_AddEditViewModel)(this.DataContext)).dgCellTemplate0_Visible;
        }
    }
}