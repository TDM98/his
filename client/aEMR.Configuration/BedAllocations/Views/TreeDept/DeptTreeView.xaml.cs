using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DataEntities;
using System.ComponentModel;
using aEMR.Controls;

namespace aEMR.Configuration.BedAllocations.Views
{
    [Export(typeof(DeptTreeView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DeptTreeView : AxUserControl
    {
        
        //private VMBedAllcations BedAllcationsVM;
        public DeptTreeView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DeptTreeView_Loaded);
            this.Unloaded += new RoutedEventHandler(DeptTreeView_Unloaded);
        }

        void DeptTreeView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        public void DeptTreeView_Loaded(object sender, RoutedEventArgs e)
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
