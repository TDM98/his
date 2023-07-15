using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.AdmissionCriterion_Mgnt.Views
{
    [Export(typeof(SymptomCategory_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SymptomCategory_AddEditView : UserControl
    {
        public SymptomCategory_AddEditView()
        {
            InitializeComponent();
        }
    }
}
