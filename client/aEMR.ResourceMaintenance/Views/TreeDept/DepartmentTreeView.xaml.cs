using System;
using System.ComponentModel.Composition;

using System.Windows;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(DepartmentTreeView))]
    public partial class DepartmentTreeView : AxUserControl
    {
        
        //private VMBedAllcations BedAllcationsVM;
        public DepartmentTreeView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DepartmentTreeView_Loaded);
            this.Unloaded += new RoutedEventHandler(DepartmentTreeView_Unloaded);
        }

        void DepartmentTreeView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void DepartmentTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }     

        public void BedAllcationsVM_GetDeptLocationTreeViewCompleted(object sender, EventArgs e)
        {
            
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            //BedAllcationsVM.SeletedRefDepartmentsTree = (RefDepartmentsTree)treeView.SelectedItem;
            //BedAllcationsVM.selectedRoomPrices = new RoomPrices();
            //if (BedAllcationsVM.SeletedRefDepartmentsTree.IsDeptLocation==true)
            //{
                
            //}
            
        }
    }
}
