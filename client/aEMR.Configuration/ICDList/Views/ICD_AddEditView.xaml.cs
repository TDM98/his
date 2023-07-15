using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.ICDList.Views
{
    [Export(typeof(ICD_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ICD_AddEditView : UserControl
    {
        public ICD_AddEditView()
        {
            InitializeComponent();
        }
    }
}
