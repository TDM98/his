using System;
using System.Collections.Generic;
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
using aEMR.Controls;
using aEMR.Common;
using Orktane.Components;

namespace aEMR.Configuration.BedAllocations.Views
{
    public partial class cwdBedAllocationView : UserControl
    {
        public cwdBedAllocationView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(cwdBedAllocationView_Loaded);
            this.Unloaded += new RoutedEventHandler(cwdBedAllocationView_Unloaded);
        }

        void cwdBedAllocationView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void cwdBedAllocationView_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public void BedAllcationsVM_GetAllDeptMSItemsByDeptIDSerTypeIDCompleted(object sender, EventArgs e)
        {
            
        }
        public void BedAllcationsVM_GetAllStaffCompleted(object sender, EventArgs e)
        {
            
        }
        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //if(BedAllcationsVM.selectedBedAllocation.VBedLocType==null)
            //{
            //    BedAllcationsVM.selectedBedAllocation.VBedLocType = new Lookup();
            //}
            
            //BedAllcationsVM.selectedBedAllocation.BedNumber= "";
            //BedAllcationsVM.selectedBedAllocation.IsActive = true;
            //BedAllcationsVM.selectedBedAllocation.VRefMedicalServiceItem = BedAllcationsVM.selectedMedServiceItemPrice.ObjMedServiceID;
            //BedAllcationsVM.selectedBedAllocation.DeptLocationID = BedAllcationsVM.SeletedRefDepartmentsTree.NodeID;
            //BedAllcationsVM.selectedBedAllocation.BAGuid = Guid.NewGuid().ToString();
            //for (int i = 0; i < BedAllcationsVM.selectedBedAllocation.BedQuantity; i++ )
            //{
            //    DataEntities.BedAllocation bl = new DataEntities.BedAllocation();
            //    BedAllcationsVM.selectedBedAllocation.Status = 1;
            //    bl = ObjectCopier.DeepCopy(BedAllcationsVM.selectedBedAllocation);
            //    BedAllcationsVM.allBedAllocation.Add(bl);
            //}
            
            
        }

        private void butExit_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void cboVBedLocType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cboMedServiceID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}

