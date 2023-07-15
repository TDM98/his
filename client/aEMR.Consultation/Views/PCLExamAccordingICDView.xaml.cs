using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.ViewContracts.Consultation_ePrescription;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class PCLExamAccordingICDView : UserControl
    {
        public PCLExamAccordingICDView()
        {
            InitializeComponent();
        }
        public Window GetViewWindow()
        {
            return Window.GetWindow(this);
        }

    }
}