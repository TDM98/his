using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.AdmissionCriterion_Mgnt.Views
{
    [Export(typeof(GroupPCLs_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class GroupPCLs_AddEditView : UserControl
    {
        public GroupPCLs_AddEditView()
        {
            InitializeComponent();
        }
    }
}
