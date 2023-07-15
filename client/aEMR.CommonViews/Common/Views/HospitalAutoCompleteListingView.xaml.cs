using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    /// <summary>
    /// Hien gio khong su dung.
    /// </summary>
    public partial class HospitalAutoCompleteListingView : UserControl, IAutoCompleteView
    {
        public HospitalAutoCompleteListingView()
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

        //private void auc1_TextChanged(object sender, System.Windows.RoutedEventArgs e)
        //{

        //}

        //private void auc1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //}
    }
}
