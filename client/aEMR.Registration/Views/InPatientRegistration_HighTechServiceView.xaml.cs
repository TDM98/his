using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.Registration.Views
{
    public partial class InPatientRegistration_HighTechServiceView : UserControl, IInPatientRegistration_HighTechServiceView
    {
        public InPatientRegistration_HighTechServiceView()
        {
            InitializeComponent();
        }
        public void SetActiveTab(InPatientRegistrationViewTab activeTab)
        {
            tabBillingInvoiceInfo.SelectedIndex = (int)activeTab;
        }
    }
}
