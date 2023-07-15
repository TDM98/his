using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.RefDepartments.Views
{
    [Export(typeof(RefDepartmentsView))]
    public partial class RefDepartmentsView : UserControl
    {
        public RefDepartmentsView()
        {
            InitializeComponent();
        }
    }
}
