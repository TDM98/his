using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(TranfHomeView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class TranfHomeView : AxUserControl
    {
        public TranfHomeView()
        {
            InitializeComponent();
        }
    }
}
