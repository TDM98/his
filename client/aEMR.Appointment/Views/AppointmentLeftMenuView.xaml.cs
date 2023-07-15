using System.Windows;
using aEMR.ViewContracts;

namespace aEMR.Appointment.Views
{
    public partial class AppointmentLeftMenuView : ILeftMenuView
    {
        public AppointmentLeftMenuView()
        {
            InitializeComponent();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            AppointmentManagementCmd.Style = defaultStyle;
            AppointmentListingCmd.Style = defaultStyle;
            AppointmentTotalCmd.Style = defaultStyle;
            AppointmentFromDiagCmd.Style = defaultStyle;
            AppointmentDetailCmd.Style = defaultStyle;
            PCLExamTargetCmd.Style = defaultStyle;
        }
    }
}
