using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace aEMR.UserAccountManagement.Views
{
    public partial class GridUserGroupView : UserControl
    {
        public GridUserGroupView()
        {
            InitializeComponent();
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
            //ModulesTreeVM.allUserGroup.Remove(ModulesTreeVM.SelectedUserGroup);
        }
        
            
    }
}
