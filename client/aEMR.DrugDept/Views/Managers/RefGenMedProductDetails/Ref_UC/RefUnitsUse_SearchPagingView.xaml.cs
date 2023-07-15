using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.DrugDept.Views
{
    public partial class RefUnitsUse_SearchPagingView : UserControl,IRefUnitsUse_SearchPagingView
    {
        public RefUnitsUse_SearchPagingView()
        {
            InitializeComponent();
        }

        public void PopulateComplete()
        {
            autMedItems.PopulateComplete();
        }
    }
}
