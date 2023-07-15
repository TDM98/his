using System.Windows;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    public partial class PatientSummaryInfoV3View : AxUserControl, IPatientSummaryInfoV3View
    {
        public PatientSummaryInfoV3View()
        {
            InitializeComponent();
        }

        public void Switch(bool isExpanded)
        {
            if (isExpanded)
            {
                border1.Visibility = Visibility.Visible;
                border2.Visibility = Visibility.Collapsed;
            }
            else
            {
                border1.Visibility = Visibility.Collapsed;
                border2.Visibility = Visibility.Visible;
            }
        }
    }
}