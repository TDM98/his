using System.Windows;
using System.Windows.Controls;



namespace aEMR.Configuration.BedAllocations.Views
{
    public partial class UCRoomEditView : UserControl
    {
        public UCRoomEditView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCRoomEditView_Loaded);
            this.Unloaded += new RoutedEventHandler(UCRoomEditView_Unloaded);
        }

        void UCRoomEditView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void UCRoomEditView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void butSave_Click(object sender, RoutedEventArgs e)
        {
            //if (BedAllcationsVM.SeletedRefDepartmentsTree.Parent == null
            //    || BedAllcationsVM.SeletedRefDepartmentsTree.IsDeptLocation == false)
            //{
            //    MessageBox.Show("Chưa chọn phòng!");
            //    return;
            //}
            //var cw = new eHCMSApp.Views.ConfigurationManager.BedAllocation.ChildWindows.cwdBedAllocView();            
            //cw.DataContext = BedAllcationsVM;
            //cw.Show();
        }

        private void butAddBed_Click(object sender, RoutedEventArgs e)
        {
            //if (BedAllcationsVM.SeletedRefDepartmentsTree.Parent == null 
            //    || BedAllcationsVM.SeletedRefDepartmentsTree.IsDeptLocation== false)
            //{
            //    MessageBox.Show("Chưa chọn phòng!");
            //    return;
            //}
            //var cw = new eHCMSApp.Views.ConfigurationManager.BedAllocation.ChildWindows.cwdBedAllocation();
            //cw.DataContext = BedAllcationsVM;
            //cw.Show();
        }

        private void butOrder_Click(object sender, RoutedEventArgs e)
        {
            //if (BedAllcationsVM.SeletedRefDepartmentsTree.Parent== null
            //    || BedAllcationsVM.SeletedRefDepartmentsTree.IsDeptLocation==false)
            //{
            //    MessageBox.Show("Chưa chọn phòng!");
            //    return;
            //}
            //var cw = new eHCMSApp.Views.ConfigurationManager.BedAllocation.ChildWindows.cwdBedPatientAllocList();
            //cw.DataContext = BedAllcationsVM;
            //cw.Show();
        }

        
    }
}
