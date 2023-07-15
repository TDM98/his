using System.Windows.Controls;
using aEMR.ViewContracts.GuiContracts;

namespace aEMR.Common.Views
{
    public partial class AddEditAppointmentView : UserControl
    {
        public AddEditAppointmentView()
        {
            InitializeComponent();           
        }

        private void dtgPatientApptPCLRequestsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AddApptPclCmd_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //e.Handled = true;
        }

        private void AddApptPclCmd_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //e.Handled = true;
        }

        
        //void AddEditAppointmentView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    gridAppointmentDetails.SetValue(DataGrid.ItemsSourceProperty, null);
        //    dfmApptDetailsInfo.SetValue(DataForm.ItemsSourceProperty, null);
        //}

        //private void dfmApptDetailsInfo_BeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    gridAppointmentDetails.IsEnabled = false;
        //}

        //private void dfmApptDetailsInfo_EditEnded(object sender, DataFormEditEndedEventArgs e)
        //{
        //    gridAppointmentDetails.IsEnabled = true;
        //}

        //private void dfmApptDetailsInfo_CurrentItemChanged(object sender, System.EventArgs e)
        //{
        //    if (dfmApptDetailsInfo.CurrentItem != null)
        //    {
        //        gridAppointmentDetails.SelectedItem = dfmApptDetailsInfo.CurrentItem;
        //    }
        //}

        //private void gridAppointmentDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (gridAppointmentDetails.SelectedItem != null)
        //    {
        //        dfmApptDetailsInfo.CurrentItem = gridAppointmentDetails.SelectedItem;
        //    }
        //}

        //private void dfmApptDetailsInfo_DeletingItem(object sender, System.ComponentModel.CancelEventArgs e)
        //{
            
        //}

        //private void HyperlinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    gridAppointmentDetails.IsEnabled = false;
        //}

        //private void NewItemButton_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    gridAppointmentDetails.IsEnabled = false;
        //}
        //public void CancelEdit()
        //{
        //    if (dfmApptDetailsInfo.Mode == DataFormMode.AddNew
        //        || dfmApptDetailsInfo.Mode == DataFormMode.Edit)
        //    {
        //        dfmApptDetailsInfo.CancelEdit();
        //    }
        //}
    }
}
