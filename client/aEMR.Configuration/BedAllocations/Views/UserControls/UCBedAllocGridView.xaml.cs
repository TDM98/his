using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Configuration.BedAllocations.Views
{
    public partial class UCBedAllocGridView : UserControl
    {
        public UCBedAllocGridView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCBedAllocGridView_Loaded);
            this.Unloaded += new RoutedEventHandler(UCBedAllocGridView_Unloaded);
        }

        void UCBedAllocGridView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void UCBedAllocGridView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        private void DataGridDoubleClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(this, null);
            }
        }
        public event EventHandler DoubleClick;

        private void grdBedAllocations_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter) || e.Key.Equals(Key.Down)) 
            {
                this.grdBedAllocations.BeginEdit();
            }
        }
        
    }
}
