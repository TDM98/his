using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.AdmissionCriteria.Views
{
    [Export(typeof(AdmissionCriteria_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionCriteria_AddEditView : UserControl
    {
        public AdmissionCriteria_AddEditView()
        {
            InitializeComponent();
        }
    }
}
