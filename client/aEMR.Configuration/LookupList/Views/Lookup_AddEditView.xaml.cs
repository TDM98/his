using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.LookupList.Views
{
    [Export(typeof(Lookup_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Lookup_AddEditView : UserControl
    {
        public Lookup_AddEditView()
        {
            InitializeComponent();
        }
    }
}
