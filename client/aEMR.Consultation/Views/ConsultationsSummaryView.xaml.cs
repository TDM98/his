using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class ConsultationsSummaryView : UserControl, IConsultationSummaryView
    {
        public ConsultationsSummaryView()
        {
            InitializeComponent();
        }

        public Window GetViewWindow()
        {
            return Window.GetWindow(this);
        }

    }
}