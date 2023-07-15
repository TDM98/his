using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace aEMR.Pharmacy.Views
{
    [Export(typeof(StorageView))]
    public partial class StorageView : UserControl
    {
        public StorageView()
        {
            InitializeComponent();
        }

     
    }
}
