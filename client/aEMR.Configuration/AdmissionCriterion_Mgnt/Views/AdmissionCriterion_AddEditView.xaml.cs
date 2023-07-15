using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.AdmissionCriterion_Mgnt.Views
{
    [Export(typeof(AdmissionCriterion_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class AdmissionCriterion_AddEditView : UserControl
    {
        public AdmissionCriterion_AddEditView()
        {
            InitializeComponent();
        }
    }
}
