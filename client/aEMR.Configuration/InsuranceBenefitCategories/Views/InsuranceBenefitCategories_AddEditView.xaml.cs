using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.InsuranceBenefitCategories.Views
{
    [Export(typeof(InsuranceBenefitCategories_AddEditView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class InsuranceBenefitCategories_AddEditView : UserControl
    {
        public InsuranceBenefitCategories_AddEditView()
        {
            InitializeComponent();
        }
    }
}
