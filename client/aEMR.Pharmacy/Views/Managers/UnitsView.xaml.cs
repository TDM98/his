using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.Pharmacy.Views
{
    [Export(typeof(UnitsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UnitsView : UserControl
    {
        public UnitsView()
        {
            InitializeComponent();
        }
    }
}
