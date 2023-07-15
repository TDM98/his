using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.ConsultantEPrescription.Views
{
    [Export(typeof(ShortHandDictionaryAddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ShortHandDictionaryAddEditView : UserControl
    {
        public ShortHandDictionaryAddEditView()
        {
            InitializeComponent();
        }
    }
}
