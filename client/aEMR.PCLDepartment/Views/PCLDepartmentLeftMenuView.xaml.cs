using System.Windows;
using aEMR.ViewContracts;
namespace aEMR.PCLDepartment.Views
{
    public partial class PCLDepartmentLeftMenuView : ILeftMenuView
    {
        public PCLDepartmentLeftMenuView()
        {
            InitializeComponent();
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];
            InputResultClick.Style = defaultStyle;
            PatientPCLRequest_ByPatientIDV_ParamClick.Style = defaultStyle;
            SieuAmResultTemplate_Cmd.Style = defaultStyle;
            UltrasoundStatistics_Cmd.Style = defaultStyle;
            PCLImageCapture_Cmd.Style = defaultStyle;
        }
    }
}