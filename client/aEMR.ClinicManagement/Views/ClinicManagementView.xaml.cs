using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.ClinicManagement.Views
{
    [Export(typeof(ClinicManagementView))]
    public partial class ClinicManagementView : AxUserControl
    {
        public ClinicManagementView()
        {
            InitializeComponent();
        }
    }
}
