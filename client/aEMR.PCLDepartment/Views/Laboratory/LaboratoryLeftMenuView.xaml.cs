using System.Windows.Controls;
using System.Windows;

namespace aEMR.PCLDepartment.Views
{
    public partial class LaboratoryLeftMenuView : UserControl
    {
        public LaboratoryLeftMenuView()
        {
            InitializeComponent();
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            PCLRequest_Cmd.Style = defaultStyle;
        }
    }
}