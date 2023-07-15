using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesHomeView))]
    public partial class ResourcesHomeView : AxUserControl
    {
        public ResourcesHomeView()
        {
            InitializeComponent();
        }
    }
}
