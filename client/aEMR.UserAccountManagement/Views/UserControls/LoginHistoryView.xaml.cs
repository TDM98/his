using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;

namespace aEMR.UserAccountManagement.Views
{
    public partial class LoginHistoryView : UserControl
    {
        public LoginHistoryView()
        {
            InitializeComponent();
        }

        public void InitData()
        {
            //if (!DesignerProperties.IsInDesignTool)
            bool designTime = System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (!designTime)
            {
                //ModulesTreeVM.GetAllRoles();
                //ModulesTreeVM.GetAllGroupByGroupID(0);
            }
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
