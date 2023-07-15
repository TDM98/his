using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.RefDepartments.Views
{
    [Export(typeof(RefDepartments_InfoView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RefDepartments_InfoView : UserControl
    {
        public RefDepartments_InfoView()
        {
            InitializeComponent();
        }
    }
}
