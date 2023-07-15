using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Configuration.RoomType.Views
{
    [Export(typeof(RoomTypeInfoView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class RoomTypeInfoView : UserControl
    {
        public RoomTypeInfoView()
        {
            InitializeComponent();
        }
    }
}
