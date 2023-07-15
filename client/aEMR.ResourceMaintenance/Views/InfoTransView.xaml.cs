using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(InfoTransView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InfoTransView : AxUserControl
    {
        public InfoTransView()
        {
            InitializeComponent();
        }
    }
}
