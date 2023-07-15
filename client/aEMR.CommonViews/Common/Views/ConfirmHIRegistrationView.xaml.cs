using System.Windows.Controls;
using aEMR.Common.ViewModels;
using eHCMSLanguage;
namespace aEMR.Common.Views
{
    public partial class ConfirmHIRegistrationView : UserControl
    {
        public ConfirmHIRegistrationView()
        {
            InitializeComponent();
        }
        private void CloseMainMenuSB_Completed(object sender, System.EventArgs e)
        {
            if (this.DataContext is ConfirmHIRegistrationViewModel)
            {
                (this.DataContext as ConfirmHIRegistrationViewModel).ConfirmHIViewVisible = false;
            }
        }
    }
}