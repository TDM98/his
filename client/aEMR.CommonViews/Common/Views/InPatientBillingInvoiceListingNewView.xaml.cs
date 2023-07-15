using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;

namespace aEMR.Common.Views
{
    public partial class InPatientBillingInvoiceListingNewView : UserControl, IInPatientBillingInvoiceListingNewView
    {
        public InPatientBillingInvoiceListingNewView()
        {
            InitializeComponent();
        }

        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void CheckBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }


        //public void ExpandDetailsInfo()
        //{
        //    if (!ServiceExpander.IsExpanded)
        //    {
        //        ServiceExpander.IsExpanded = true;
        //    }
        //}


        //public void ShowEditColumn(bool bShow)
        //{
        //    var editColumn = gridServices.GetColumnByName("colEdit");
        //    if (editColumn != null)
        //    {
        //        if (bShow)
        //        {
        //            editColumn.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            editColumn.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}

        //public void ShowInfoColumn(bool bShow)
        //{
        //    var col = gridServices.GetColumnByName("colShowDetails");
        //    if (col != null)
        //    {
        //        if (bShow)
        //        {
        //            col.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            col.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}
        //public void ShowRecalcHiColumn(bool bShow)
        //{
        //    var col = gridServices.GetColumnByName("colRecalcHi");
        //    if (col != null)
        //    {
        //        if (bShow)
        //        {
        //            col.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            col.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}

        public void ShowColumn(string columnName, bool bShow)
        {
            var col = gridServices.GetColumnByName(columnName);
            if (col != null)
            {
                if (bShow)
                {
                    col.Visibility = Visibility.Visible;
                }
                else
                {
                    col.Visibility = Visibility.Collapsed;
                }
            }
        }

    }
}
