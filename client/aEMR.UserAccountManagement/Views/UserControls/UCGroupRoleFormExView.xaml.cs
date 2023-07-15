using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
namespace aEMR.UserAccountManagement.Views
{
    public partial class UCGroupRoleFormExView : UserControl
    {
        public UCGroupRoleFormExView()
        {
            InitializeComponent();
        }

        public void InitData()
        {
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    //ModulesTreeVM.GetAllRoles();
            //    //ModulesTreeVM.GetAllGroupByGroupID(0);
            //}
        }

        public void ModulesTreeVM_GetAllGroupRolesGetByIDCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllGroupByGroupIDCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllRolesCompleted(object sender, EventArgs e)
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

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //if(ModulesTreeVM.allGroupRole.Count>0)
            //{
            //    for (int i = 0; i < ModulesTreeVM.allGroupRole.Count;i++ )
            //    {
            //        ModulesTreeVM.AddNewGroupRoles(ModulesTreeVM.allGroupRole[i].Group.GroupID
            //            , ModulesTreeVM.allGroupRole[i].Role.RoleID);
            //    }
            //}                   
        }

        private void butAdd_Click(object sender, RoutedEventArgs e)
        {
            //GroupRole GR = new DataEntities.GroupRole();
            //GR.Group = new DataEntities.Group();
            //GR.Role = new DataEntities.Role();
            //GR.Group = ModulesTreeVM.SelectedGroup;
            //GR.Role = ModulesTreeVM.SelectedRole;
            //ModulesTreeVM.allGroupRole.Add(GR);
        }

        private void cboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ModulesTreeVM.GetAllGroupRolesGetByID(ModulesTreeVM.SelectedGroup.GroupID,0);
        }

        private void cboUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ModulesTreeVM.GetAllGroupRolesGetByID(0, ModulesTreeVM.SelectedRole.RoleID);
        }

        
    
    }
}
