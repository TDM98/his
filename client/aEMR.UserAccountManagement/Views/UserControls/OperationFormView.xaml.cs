using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.UserAccountManagement.Views
{
    public partial class OperationFormView : UserControl
    {
        public OperationFormView()
        {
            InitializeComponent();
        }
        
        
        public void InitData()
        {
            
        }

        
        public void ModulesTreeVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "SelectedModulesTree")
            //{
            //    if (ModulesTreeVM.SelectedModulesTree.Level == 2)
            //    {
            //        ModulesTreeVM.GetAllOperationsByFuncID(ModulesTreeVM.SelectedModulesTree.NodeID);
            //    }
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

        
        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //string stName = txtName.Text;
            //string stDescript = txtDescription.Text;
            //if (ModulesTreeVM.SelectedModulesTree.Level == 2)
            //{
            //    ModulesTreeVM.AddNewOperations(ModulesTreeVM.SelectedModulesTree.NodeID, stName, stDescript);
            //}            
        }

    }
}
