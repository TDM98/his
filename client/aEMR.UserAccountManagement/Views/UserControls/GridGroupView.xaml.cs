using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.UserAccountManagement.Views
{
    public partial class GridGroupView : UserControl
    {
        public GridGroupView()
        {
            InitializeComponent();
            
        }
        
        void GridGroupView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        
        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(this, null);
            }
        }
        public event EventHandler DoubleClick;

        private void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            //MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this Permission?", "Delete a OPeration",
            //                             MessageBoxButton.OKCancel);
            //if (result == MessageBoxResult.Cancel)
            //    return;
            //if (ModulesTreeVM.SelectedOperation != null)
            //{
            //    //ModulesTreeVM.DeletePermissions(ModulesTreeVM.SelectedPermission.PermissionItemID);
            //}
        }
        
    }
}
