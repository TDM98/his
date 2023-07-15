using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.DrugDept.Views
{
    [Export(typeof(UnitsView))]
    public partial class UnitsView : UserControl
    {
        public UnitsView()
        {
            InitializeComponent();
        }
    }
}
