using System.Windows;
using aEMR.ViewContracts;

namespace aEMR.ClinicManagement.Views
{
    public partial class ClinicMenuView : ILeftMenuView
    {
        public ClinicMenuView()
        {
            InitializeComponent();
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            ClinicCmd.Style = defaultStyle;
            ClinicTargetCmd.Style = defaultStyle;
            ClinicTimeCmd.Style = defaultStyle;
            //ClinicReportCmd.Style = defaultStyle;
            ConsultCmd.Style = defaultStyle;
            //PCLCmd.Style = defaultStyle;
            ShelfManCmd.Style = defaultStyle;
            FileInportCmd.Style = defaultStyle;
            FileCheckInCmd.Style = defaultStyle;
            FileCheckOutCmd.Style = defaultStyle;
            FileCodePrintCmd.Style = defaultStyle;
            FileCheckInFromRegCmd.Style = defaultStyle;
            FileCodeHistoryCmd.Style = defaultStyle;
            FileManCmd.Style = defaultStyle;
        }
    }
}
