
using System.Windows;
using System.Windows.Controls;


namespace aEMR.CommonUserControls
{
    public partial class UCBedPatientAlloc : UserControl
    {
        public UCBedPatientAlloc()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBedPatientAlloc_Loaded);
        }

        void UCBedPatientAlloc_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
