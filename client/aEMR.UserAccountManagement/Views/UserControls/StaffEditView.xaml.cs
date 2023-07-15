using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.UserAccountManagement.Views
{
    public partial class StaffEditView : UserControl
    {
        public StaffEditView()
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
