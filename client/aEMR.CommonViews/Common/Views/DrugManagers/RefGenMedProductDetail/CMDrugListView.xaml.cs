using aEMR.Controls;
using System.Windows;
namespace aEMR.Common.Views
{
    public partial class CMDrugListView : AxUserControl
    {
        public CMDrugListView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility V = ((aEMR.Common.ViewModels.CMDrugListViewModel)(this.DataContext)).dgColumnExtOfThuoc_Visible;
            for (int i = 12; i < 17; i++)
            {
                dtGrid.Columns[i].Visibility = V;
            }
            if (dtGrid.GetColumnByName("PaymentRateOfHIAddedItem") != null)
            {
                dtGrid.GetColumnByName("PaymentRateOfHIAddedItem").Visibility = ((aEMR.Common.ViewModels.CMDrugListViewModel)(this.DataContext)).IsMatView ? Visibility.Visible : Visibility.Collapsed;
            }
            if (dtGrid.GetColumnByName("CeilingPrice2ndItem") != null)
            {
                dtGrid.GetColumnByName("CeilingPrice2ndItem").Visibility = ((aEMR.Common.ViewModels.CMDrugListViewModel)(this.DataContext)).IsMatView ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}