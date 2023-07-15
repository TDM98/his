using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.ComponentModel;

namespace aEMR.UserAccountManagement.Views
{
    public partial class GridGroupRoleView : UserControl
    {
        public GridGroupRoleView()
        {
            InitializeComponent();
            
        }
        
        void UCGroupRoleGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            //if(wkGroupRoleVM_PropertyChanged!=null)
            {
                //wkGroupRoleVM_PropertyChanged.UnregisterHandler();
            }
            
        }

        public void GroupRoleVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "pageId")
            //{
            //    GroupRoleVM.GetAllUserGroupGetByID(AccountID, GroupID);   
            //}
        }

        
        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(this, null);
            }
        }
        public event EventHandler DoubleClick;

    }
}
