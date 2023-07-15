using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.Configuration.PCLExamTypePrices.Views
{
    [Export(typeof(PCLExamTypePrices_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PCLExamTypePrices_AddEditView : AxUserControl
    {
        public PCLExamTypePrices_AddEditView()
        {
            InitializeComponent();
        }
    }
}
