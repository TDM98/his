using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    /// <summary>
    /// Hien gio khong su dung.
    /// </summary>
    public partial class MedItemListingView : UserControl, IAutoCompleteView
    {
        public MedItemListingView()
        {
            InitializeComponent();
        }
        public void PopulateComplete()
        {
            this.auc1.PopulateComplete();
        }

        public AxAutoComplete AutoCompleteBox
        {
            get { return this.auc1; }
        }
    }
}
