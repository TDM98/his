using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.DiseaseProgression.Views
{
    [Export(typeof(DiseaseProgression_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DiseaseProgression_AddEditView : UserControl
    {
        public DiseaseProgression_AddEditView()
        {
            InitializeComponent();
        }
    }
}
