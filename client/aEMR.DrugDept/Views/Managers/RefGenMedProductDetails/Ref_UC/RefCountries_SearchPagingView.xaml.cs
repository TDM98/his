using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.DrugDept.Views
{
    public partial class RefCountries_SearchPagingView : UserControl, IRefCountries_SearchPagingView
    {
        public RefCountries_SearchPagingView()
        {
            InitializeComponent();
        }

        public void PopulateComplete()
        {
            autMedItems.PopulateComplete();
        }
    }
}
