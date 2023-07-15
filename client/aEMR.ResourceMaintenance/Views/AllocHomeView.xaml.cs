using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(AllocHomeView))]
    public partial class AllocHomeView : AxUserControl
    {
        public AllocHomeView()
        {
            InitializeComponent();
        }
    }
}
