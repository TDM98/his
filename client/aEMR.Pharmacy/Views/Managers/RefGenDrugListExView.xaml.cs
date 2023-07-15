using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace aEMR.Pharmacy.Views
{
    [Export(typeof(RefGenDrugListExView))]
    public partial class RefGenDrugListExView : UserControl
    {
        public RefGenDrugListExView()
        {
            InitializeComponent();
        }

       
    }
}
