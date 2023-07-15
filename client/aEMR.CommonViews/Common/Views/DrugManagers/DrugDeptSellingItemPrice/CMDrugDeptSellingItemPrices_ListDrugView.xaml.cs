using System.Windows;
using System.Windows.Controls;
namespace aEMR.Common.Views
{
    public partial class CMDrugDeptSellingItemPrices_ListDrugView : UserControl
    {
        public CMDrugDeptSellingItemPrices_ListDrugView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility V = ((aEMR.Common.ViewModels.CMDrugDeptSellingItemPrices_ListDrugViewModel)(this.DataContext)).dgColumnExtOfThuoc_Visible;
            for (int i = 15; i < 20; i++)
            {
                dtGrid.Columns[i].Visibility = V;
            }
        }
    }
}