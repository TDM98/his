using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;

namespace aEMR.UserAccountManagement.Views
{
    public partial class UserAccountView : UserControl
    {
        public UserAccountView()
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
