using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.DrugDept.Views
{
    public partial class DrugClasses_SearchPaging_DrugView : UserControl, IDrugClasses_SearchPaging_DrugView
    {
        public DrugClasses_SearchPaging_DrugView()
        {
            InitializeComponent();
        }

        public void PopulateComplete()
        {
            autMedItems.PopulateComplete();
        }
    }
}
