using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.RefApplicationConfig.Views
{
    [Export(typeof(RefApplicationConfig_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RefApplicationConfig_AddEditView : UserControl
    {
        public RefApplicationConfig_AddEditView()
        {
            InitializeComponent();
        }
    }
}
