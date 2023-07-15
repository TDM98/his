using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.RefMedicalServiceItems_IsPCL.Views
{
    [Export(typeof(RefMedicalServiceItems_IsPCL_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RefMedicalServiceItems_IsPCL_AddEditView : UserControl
    {
        public RefMedicalServiceItems_IsPCL_AddEditView()
        {
            InitializeComponent();
        }
    }
}
