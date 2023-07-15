using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;



namespace aEMR.UserAccountManagement.Views
{
    public partial class GroupRoleTabView : UserControl
    {
        public GroupRoleTabView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(GroupRoleTabView_Loaded);
            this.Unloaded += new RoutedEventHandler(GroupRoleTabView_Unloaded);
        }
        
        void GroupRoleTabView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void GroupRoleTabView_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();            
        }

        public void InitData()
        {
            //if (!DesignerProperties.IsInDesignTool)
            //{

            //}
        }

        public void ModulesTreeVM_GetAllUserAccountCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllUserGroupGetByIDCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllGroupByGroupIDCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllRolesCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetModulesTreeViewCompleted(object sender, EventArgs e)
        {
            
        }

        public void FunctionsVM_GetAllFunctionCompleted(object sender, EventArgs e)
        {
            
        }


        public void UserAccountsVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }


    }
}

