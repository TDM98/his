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
//using eHCMS.ViewModels.eHCMSConfigurationManagerViewModel;
using System.ComponentModel;

using Orktane.Components;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.Views
{
    [Export(typeof(ConsultRoomTreeView))]
    public partial class ConsultRoomTreeView : AxUserControl
    {
        
        //private VMBedAllcations BedAllcationsVM;
        public ConsultRoomTreeView()
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

        }
    }
}
