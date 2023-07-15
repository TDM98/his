using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.Locations.Views
{
    [Export(typeof(Locations_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Locations_AddEditView : UserControl
    {
        public Locations_AddEditView()
        {
            InitializeComponent();
        }
    }
}
