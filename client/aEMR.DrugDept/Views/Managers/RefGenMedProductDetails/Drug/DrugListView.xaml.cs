using System.Windows;
using System.Windows.Controls;

namespace aEMR.DrugDept.Views
{
    public partial class DrugListView : UserControl
    {
        public DrugListView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Visibility V= ((aEMR.DrugDept.ViewModels.DrugListViewModel)(this.DataContext)).dgColumnExtOfThuoc_Visible;
            //for(int i=12;i<17;i++)
            //{
            //    dtGrid.Columns[i].Visibility = V;
            //}

        }
    }
}
