using aEMR.Controls;
using System.Windows;
namespace aEMR.DrugDept.Views
{
    public partial class PCLExamTypeContactDrugListView : AxUserControl
    {
        public PCLExamTypeContactDrugListView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Visibility V = ((aEMR.DrugDept.ViewModels.PCLExamTypeContactDrugListViewModel)(this.DataContext)).dgColumnExtOfThuoc_Visible;
            //for (int i = 12; i < 17; i++)
            //{
            //    dtGrid.Columns[i].Visibility = V;
            //}
            //if (dtGrid.GetColumnByName("PaymentRateOfHIAddedItem") != null)
            //{
            //    dtGrid.GetColumnByName("PaymentRateOfHIAddedItem").Visibility = ((aEMR.DrugDept.ViewModels.PCLExamTypeContactDrugListViewModel)(this.DataContext)).IsMatView ? Visibility.Visible : Visibility.Collapsed;
            //}
            //if (dtGrid.GetColumnByName("CeilingPrice2ndItem") != null)
            //{
            //    dtGrid.GetColumnByName("CeilingPrice2ndItem").Visibility = ((aEMR.DrugDept.ViewModels.PCLExamTypeContactDrugListViewModel)(this.DataContext)).IsMatView ? Visibility.Visible : Visibility.Collapsed;
            //}
        }
    }
}