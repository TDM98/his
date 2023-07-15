using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.Common.Views
{
    public partial class InPatientBedPatientListingView : UserControl, IInPatientBedPatientListingView
    {
        public InPatientBedPatientListingView()
        {
            InitializeComponent();
        }

        public void ShowDeleteColumn(bool bShow)
        {
            //if (bShow)
            //{
            //    grid.Columns[0].Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    grid.Columns[0].Visibility = Visibility.Collapsed;
            //}
        }
    }
}
