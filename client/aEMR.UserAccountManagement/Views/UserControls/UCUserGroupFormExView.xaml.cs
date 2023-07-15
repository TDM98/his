using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace aEMR.UserAccountManagement.Views
{
    public partial class UCUserGroupFormExView : UserControl
    {
        public UCUserGroupFormExView()
        {
            InitializeComponent();
            
        }
        
        public void InitData()
        {
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    //UCUserGroupGridEx.DataContext = ModulesTreeVM;
            //    //ModulesTreeVM.GetAllUserAccount(0);
            //    //ModulesTreeVM.GetAllGroupByGroupID(0);
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

        private void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void grdBedAllocations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //if(ModulesTreeVM.allUserGroup.Count>0)
            //{
            //    for (int i = 0; i < ModulesTreeVM.allUserGroup.Count; i++)
            //    {
            //        ModulesTreeVM.AddNewUserGroup( ModulesTreeVM.allUserGroup[i].UserAccount.AccountID
            //            , ModulesTreeVM.allUserGroup[i].Group.GroupID);
            //    }
            //}
        }

        private void butAdd_Click(object sender, RoutedEventArgs e)
        {
            //UserGroup UG = new DataEntities.UserGroup();
            //UG.Group = new DataEntities.Group();
            //UG.UserAccount = new DataEntities.UserAccount();
            //UG.Group = ModulesTreeVM.SelectedGroup;
            //UG.UserAccount = ModulesTreeVM.SelectedUserAccount;
            //ModulesTreeVM.allUserGroup.Add(UG);
        }

        private void cboUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ModulesTreeVM.GetAllUserGroupGetByID(ModulesTreeVM.SelectedUserAccount.AccountID,0);
        }

        private void cboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ModulesTreeVM.GetAllUserGroupGetByID(0,ModulesTreeVM.SelectedGroup.GroupID);
        }

        
    
    }
}
