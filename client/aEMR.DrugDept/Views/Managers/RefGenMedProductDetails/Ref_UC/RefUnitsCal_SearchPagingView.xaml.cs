using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.DrugDept.Views
{
    public partial class RefUnitsCal_SearchPagingView : UserControl,IRefUnitsCal_SearchPagingView
    {
        public RefUnitsCal_SearchPagingView()
        {
            InitializeComponent();
        }

        public void PopulateComplete()
        {
            autMedItems.PopulateComplete();
        }
    }
}
