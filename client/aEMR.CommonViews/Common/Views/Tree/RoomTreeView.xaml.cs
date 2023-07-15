using System.ComponentModel.Composition;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    [Export(typeof(RoomTreeView))]
    public partial class RoomTreeView : AxUserControl
    {
        
        public RoomTreeView()
        {
            InitializeComponent();
        }
    }
}
