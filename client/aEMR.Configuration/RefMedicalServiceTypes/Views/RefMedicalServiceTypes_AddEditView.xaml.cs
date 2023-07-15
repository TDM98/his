using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.RefMedicalServiceTypes.Views
{
    [Export(typeof(RefMedicalServiceTypes_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RefMedicalServiceTypes_AddEditView : UserControl
    {
        public RefMedicalServiceTypes_AddEditView()
        {
            InitializeComponent();
        }
    }
}
