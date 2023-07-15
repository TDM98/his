using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.CitiesProvinces.Views
{
    [Export(typeof(CitiesProvinces_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CitiesProvinces_AddEditView : UserControl
    {
        public CitiesProvinces_AddEditView()
        {
            InitializeComponent();
        }
    }
}
