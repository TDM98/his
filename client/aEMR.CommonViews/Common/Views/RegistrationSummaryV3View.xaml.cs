using System.Windows.Controls;
using System.Windows;
using aEMR.ViewContracts;

namespace aEMR.Common.Views
{
    public partial class RegistrationSummaryV3View : UserControl, IRegistrationSummaryV3View
    {
        public RegistrationSummaryV3View()
        {
            InitializeComponent();
        }

        public void ResetView()
        {
            tabRegistrationInfo.SelectedIndex = 0;
        }

        public void ShowClickButton(bool bShow)
        {
            if (bShow)
            {
                ConTrolButton1.Visibility = Visibility.Visible;
                ControlButton2.Visibility = Visibility.Visible;
            }
            else
            {
                ConTrolButton1.Visibility = Visibility.Collapsed;
                ControlButton2.Visibility = Visibility.Collapsed;
            }
        }
    }
}