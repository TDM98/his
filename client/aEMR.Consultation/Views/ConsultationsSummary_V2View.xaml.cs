using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.ConsultantEPrescription.Views
{
    public partial class ConsultationsSummary_V2View : UserControl, IConsultationSummaryView
    {
        public ConsultationsSummary_V2View()
        {
            InitializeComponent();
        }
        public Window GetViewWindow()
        {
            return Window.GetWindow(this);
        }

    }
}