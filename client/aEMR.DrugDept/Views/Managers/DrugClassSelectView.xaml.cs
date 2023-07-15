using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.DrugDept.Views
{
    [Export(typeof(DrugClassSelectView))]
    public partial class DrugClassSelectView : UserControl
    {
        public DrugClassSelectView()
        {
            InitializeComponent();
        }
    }
}
