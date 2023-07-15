using System.Windows;
using System.Windows.Media;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{

    public partial class FamilyHistoryView: AxUserControl
    {
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        
        public FamilyHistoryView()
        {
            
        }

        private void LnkSave_OnGotFocus(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
