using System.Windows;
using System.Windows.Controls;

namespace aEMR.DrugDept.Views
{
    public partial class DrugDeptSellingItemPrices_ListDrugView : UserControl
    {
        public DrugDeptSellingItemPrices_ListDrugView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility V = ((aEMR.DrugDept.ViewModels.DrugDeptSellingItemPrices_ListDrugViewModel)(this.DataContext)).dgColumnExtOfThuoc_Visible;
            for (int i = 15; i < 20; i++)
            {
                dtGrid.Columns[i].Visibility = V;
            }

        }
    }
}
