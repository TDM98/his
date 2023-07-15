using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    /// <summary>
    /// Hien gio khong su dung.
    /// </summary>
    public partial class PclExamTypeListingView : UserControl, IPclExamTypeListingView
    {
        public PclExamTypeListingView()
        {
            InitializeComponent();
        }
        public void PopulateComplete()
        {
            this.auc1.PopulateComplete();
        }
        public AxAutoComplete PclExamTypes
        {
            get { return this.auc1; }
        }
    }
}
