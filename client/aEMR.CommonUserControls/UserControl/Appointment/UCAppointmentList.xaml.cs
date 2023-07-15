using System;
using System.Windows;
using System.Windows.Controls;
using aEMR.Common;

namespace aEMR.CommonUserControls
{
    public partial class UCAppointmentList : UserControl
    {
        public UCAppointmentList()
        {
            InitializeComponent();

        }
        public event EventHandler<EventArgs<object>> DoubleClick;
     
        private void DataGridDoubleClickBehavior_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(((DataGridRow)sender).DataContext != null)
            {
                if (DoubleClick != null)
                {
                    DoubleClick(sender, new EventArgs<object>(((DataGridRow)sender).DataContext));
                }
            }
            
        }
        public event EventHandler<EventArgs<object>> deleteClick; 
        private void lnkDelete_Click(object sender, RoutedEventArgs e)
        {
            if (deleteClick!=null)
            {
                deleteClick(sender, new EventArgs<object>(gridAppointments.SelectedItem));
            }
        }
    }
}
