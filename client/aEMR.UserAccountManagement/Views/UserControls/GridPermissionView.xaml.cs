using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace aEMR.UserAccountManagement.Views
{
    public partial class GridPermissionView : UserControl
    {
        public GridPermissionView()
        {
            InitializeComponent();
            
        }
        
        void UCPermissionGrid_Unloaded(object sender, RoutedEventArgs e)
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
            
        }

        private void grdBedAllocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
            
    }
}
