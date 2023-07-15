using System.Windows;
using System.Windows.Controls;

namespace eHCMS.CommonUserControls
{
    public partial class UCInPatientDeptListing : UserControl
    {
        public UCInPatientDeptListing()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBedPatientAlloc_Loaded);
        }

        void UCBedPatientAlloc_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
