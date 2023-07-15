using aEMR.Controls;
using System.ComponentModel.Composition;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(InfoView))]
    public partial class InfoView : AxUserControl
    {
        public InfoView()
        {
            InitializeComponent();
        }
    }
}
