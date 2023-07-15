using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.Common.Views
{
    public partial class InPatientSummaryDetailsView : UserControl, IInPatientSummaryDetailsView
    {
        public InPatientSummaryDetailsView()
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
