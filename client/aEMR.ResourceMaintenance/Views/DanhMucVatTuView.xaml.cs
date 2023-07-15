using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(DanhMucVatTuView))]
    public partial class DanhMucVatTuView : AxUserControl
    {
        public DanhMucVatTuView()
        {
            InitializeComponent();
        }
    }
}
