using System.Windows;
using System.Windows.Controls;
using System;
using eHCMS.Common;

namespace eHCMS.CommonUserControls
{
    public partial class UCBedPatientAllocListing : UserControl
    {
        public UCBedPatientAllocListing()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBedPatientAlloc_Loaded);
        }

        void UCBedPatientAlloc_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public event EventHandler<EventArgs<object>> RemoveItem;
        private void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            var lnkDelete = (HyperlinkButton)sender;
            if(RemoveItem != null)
            {
                var args = new EventArgs<object>(lnkDelete.DataContext);
                RemoveItem(this, args);
            }
        }
    }
}
