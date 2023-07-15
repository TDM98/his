using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.ViewContracts;


namespace aEMR.Common.Views
{
    public partial class FindAppointmentKSKView : UserControl
    {
        public FindAppointmentKSKView()
        {
            InitializeComponent();

            this.txtName.GotFocus += new RoutedEventHandler(txtName_GotFocus);
        }

        void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txtName.GotFocus -= new RoutedEventHandler(txtName_GotFocus);
            FocusOnGrid();
        }

        private void gridPatient_KeyDown(object sender, KeyEventArgs e)
        {
            IFindPatient vm = Globals.GetViewModel<IFindPatient>();
            var context = (sender as DataGrid).SelectedItem;
            if (e.Key == Key.Enter)
            {
                vm.SelectPatientAndClose(context); 
            }
        }
        public void FocusOnGrid()
        {
            //if (gridPatient.SelectedIndex >= 0)
            //{
            //    gridPatient.SetFocus();
            //}
        }
    }
}