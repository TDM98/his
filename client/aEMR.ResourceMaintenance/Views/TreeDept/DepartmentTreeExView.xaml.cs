using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.ComponentModel;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(DepartmentTreeExView))]
    public partial class DepartmentTreeExView : AxUserControl
    {
        
        //private VMBedAllcations BedAllcationsVM;
        public DepartmentTreeExView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DepartmentTreeExView_Loaded);
            this.Unloaded += new RoutedEventHandler(DepartmentTreeExView_Unloaded);
        }

        void DepartmentTreeExView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void DepartmentTreeExView_Loaded(object sender, RoutedEventArgs e)
        {

        }
        
        public void initdata() 
        {
            //if (!DesignerProperties.IsInDesignTool)
            //{
            //    //BedAllcationsVM.GetDeptLocTreeViewBang("7000,7001,7002",true);
            //}
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
