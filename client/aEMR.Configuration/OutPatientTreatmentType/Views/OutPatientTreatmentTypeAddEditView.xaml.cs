using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.OutPatientTreatmentType.Views
{
    [Export(typeof(OutPatientTreatmentTypeAddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OutPatientTreatmentTypeAddEditView : UserControl
    {
        public OutPatientTreatmentTypeAddEditView()
        {
            InitializeComponent();
        }
    }
}
