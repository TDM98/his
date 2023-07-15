using System;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.UserAccountManagement.Views
{
    public partial class UserGroupTabView : UserControl
    {
        public UserGroupTabView()
        {
            InitializeComponent();
        }

        void UCUserGroupTab_Loaded(object sender, RoutedEventArgs e)
        {
            InitData();            
        }

        public void InitData()
        {
            //ModulesTreeVM.GetModulesTreeView();
            //ModulesTreeVM.GetAllRoles();
            //ModulesTreeVM.GetAllGroupByGroupID(0);
            //ModulesTreeVM.GetAllUserGroupGetByID(0, 0);
            //ModulesTreeVM.GetAllUserAccount(0);
        }


        public void ModulesTreeVM_GetAllUserAccountCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllUserGroupGetByIDCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllGroupByGroupIDCompleted(object sender, EventArgs e)
        {
            //ModulesTreeVM.SelectedRole
            //ModulesTreeVM.allGroup
            //ModulesTreeVM.SelectedUserAccount.AccountID
            
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

