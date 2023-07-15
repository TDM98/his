using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.UserAccountManagement.Views
{
    public partial class PermissonFormView : UserControl
    {
        public PermissonFormView()
        {
            InitializeComponent();
        }
        
        
        void UCPermissonForm_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void InitData()
        {

        }

        public void ModulesTreeVM_GetAllRolesCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_GetAllPermissionsCompleted(object sender, EventArgs e)
        {
            
        }

        public void ModulesTreeVM_AddNewPermissionsCompleted(object sender, EventArgs e)
        {
            //MessageBox.Show("Add new permission success!");
            //ModulesTreeVM.GetAllPermissions_GetByID(ModulesTreeVM.SelectedRole.RoleID, 0);
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
            //if (ModulesTreeVM.SelectedPermission != null
            //    && ModulesTreeVM.SelectedPermission.OperationID > 0
            //    )
            //{
            //    ModulesTreeVM.AddNewPermissions(
            //             ModulesTreeVM.SelectedRole.RoleID
            //             , ModulesTreeVM.SelectedOperation.OperationID
            //            , ModulesTreeVM.SelectedPermission.pFullControl
            //            , ModulesTreeVM.SelectedPermission.pView
            //            , ModulesTreeVM.SelectedPermission.pAdd
            //            , ModulesTreeVM.SelectedPermission.pUpdate
            //            , ModulesTreeVM.SelectedPermission.pDelete
            //            , ModulesTreeVM.SelectedPermission.pReport
            //            , ModulesTreeVM.SelectedPermission.pPrint  
            //            );
                
            //}            
        }

        private void butExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkAdmin_Checked(object sender, RoutedEventArgs e)
        {
            if (chkAdmin.IsChecked==true)
            {
                chkDelete.IsChecked = true;
                chkPrint.IsChecked = true;
                chkAdd.IsChecked = true;
                chkUpdate.IsChecked = true;
                chkView.IsChecked = true;
                chkReport.IsChecked = true;
            }
            
        }

        private void chkAdmin_Unchecked(object sender, RoutedEventArgs e)
        {
            chkDelete.IsChecked = false;
            chkPrint.IsChecked = false;
            chkAdd.IsChecked = false;
            chkUpdate.IsChecked = false;
            chkView.IsChecked = false;
            chkReport.IsChecked = false;
        }

        private void cboUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ModulesTreeVM.GetAllPermissions_GetByID(ModulesTreeVM.SelectedRole.RoleID,0);
        }
    }
}
