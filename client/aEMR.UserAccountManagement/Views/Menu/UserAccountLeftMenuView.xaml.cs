using System.Windows;
using aEMR.ViewContracts;

namespace aEMR.UserAccountManagement.Views
{
    public partial class UserAccountLeftMenuView : ILeftMenuView
    {
        public UserAccountLeftMenuView()
        {
            InitializeComponent();
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            UserAccountStaffInfoCmd.Style = defaultStyle;
            UserAccountStaffCmd.Style = defaultStyle;
            UserAccountListCmd.Style = defaultStyle;
            UserConfigCmd.Style = defaultStyle;
            LoginHistoryCmd.Style = defaultStyle;
            StaffDeptResponCmd.Style = defaultStyle;
            DoctorAuthoCmd.Style = defaultStyle;
        }
    }
}
