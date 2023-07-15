using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.BedCategory.Views
{
    [Export(typeof(BedCategory_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class BedCategory_AddEditView : UserControl
    {
        public BedCategory_AddEditView()
        {
            InitializeComponent();
        }
    }
}
