using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    /// <summary>
    /// Hien gio khong su dung.
    /// </summary>
    public partial class Icd10ListingView : UserControl, IAutoCompleteView
    {
        public Icd10ListingView()
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
