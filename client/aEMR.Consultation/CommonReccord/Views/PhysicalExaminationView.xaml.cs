using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{
    
    public partial class PhysicalExaminationView : UserControl
    {
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        public PhysicalExaminationView()
        {
            InitializeComponent();
        }


        private void CboLookupSmokeStatus_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

    }
}
