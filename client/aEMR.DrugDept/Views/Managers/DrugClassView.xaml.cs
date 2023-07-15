using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.DrugDept.Views
{
    [Export(typeof(DrugClassView))]
    public partial class DrugClassView : UserControl
    {
        public DrugClassView()
        {
            InitializeComponent();
        }
    }
}
