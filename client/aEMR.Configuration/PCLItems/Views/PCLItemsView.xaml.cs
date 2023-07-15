using System.ComponentModel.Composition;
using System.Windows.Controls;


namespace aEMR.Configuration.PCLItems.Views
{
    [Export(typeof(PCLItemsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PCLItemsView : UserControl
    {
        public PCLItemsView()
        {
            InitializeComponent();
        }
        
    }
}
