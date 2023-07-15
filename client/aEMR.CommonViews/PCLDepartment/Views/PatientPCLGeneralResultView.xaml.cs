using aEMR.ViewModels;
using System.Windows.Controls;

namespace aEMR.Views
{
    public partial class PatientPCLGeneralResultView : UserControl
    {
        public PatientPCLGeneralResultView()
        {
            InitializeComponent();
        }
        private bool gSkipPreviewKeyDown = false;
        private void WBGeneral_PreviewKeyDown(object sender, System.Windows.Forms.PreviewKeyDownEventArgs e)
        {
            if (this.DataContext == null || !(this.DataContext is PatientPCLGeneralResultViewModel) || e.KeyCode == System.Windows.Forms.Keys.ControlKey)
            {
                return;
            }
            //20190411 TTM: Thay đổi hot key chụp hình từ Ctrl + Q => F2.
            //if (e.KeyCode == System.Windows.Forms.Keys.Q && e.Modifiers == System.Windows.Forms.Keys.Control && !gSkipPreviewKeyDown)
            if (e.KeyCode == System.Windows.Forms.Keys.F2 && e.Modifiers == System.Windows.Forms.Keys.None && !gSkipPreviewKeyDown)
            {
                (this.DataContext as PatientPCLGeneralResultViewModel).CallCatureEvent();
            }
            gSkipPreviewKeyDown = !gSkipPreviewKeyDown;
        }
    }
}