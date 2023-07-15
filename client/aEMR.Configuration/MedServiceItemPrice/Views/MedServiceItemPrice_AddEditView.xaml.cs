using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.Configuration.MedServiceItemPrice.Views
{
    [Export(typeof(MedServiceItemPrice_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class MedServiceItemPrice_AddEditView : AxUserControl
    {
        public MedServiceItemPrice_AddEditView()
        {
            InitializeComponent();
        }
    }
}
