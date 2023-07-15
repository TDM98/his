using System;
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Configuration.BedAllocations.Views
{
    public partial class cwdBedAllocView : UserControl
    {
        public cwdBedAllocView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(cwdBedAllocView_Loaded);
            this.Unloaded += new RoutedEventHandler(cwdBedAllocView_Unloaded);
        }

        void cwdBedAllocView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void cwdBedAllocView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        

        public void BedAllcationsVM_GetCountBedAllocByDeptIDCompleted(object sender, EventArgs e)
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
            //for (int i = 0; i < BedAllcationsVM.lstCountBedAllocation.Count;i++ )
            //{
            //    if (BedAllcationsVM.lstCountBedAllocation[i].VBedLocType==null)
            //    {
            //        BedAllcationsVM.lstCountBedAllocation[i].VBedLocType = new Lookup();
            //    }

            //    BedAllcationsVM.UpdateBedAllocationMedSer(BedAllcationsVM.lstCountBedAllocation[i].BAGuid
            //        , BedAllcationsVM.lstCountBedAllocation[i].VRefMedicalServiceItem.MedServiceID
            //        , BedAllcationsVM.lstCountBedAllocation[i].VBedLocType.LookupID);

            //}            
            //this.DialogResult = true;
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

