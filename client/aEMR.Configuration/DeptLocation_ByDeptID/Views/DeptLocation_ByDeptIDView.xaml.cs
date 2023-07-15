using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.DeptLocation_ByDeptID.Views
{
 [Export(typeof(DeptLocation_ByDeptIDView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DeptLocation_ByDeptIDView : UserControl
    {
        public DeptLocation_ByDeptIDView()
        {
            InitializeComponent();
        }
    }
}
