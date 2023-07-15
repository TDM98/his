using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using aEMR.Infrastructure;
using aEMR.ViewContracts;

namespace aEMR.Common.Views
{
    public partial class FindPatientView : UserControl
    {
        public FindPatientView()
        {
            InitializeComponent();

            gridPatient.AddHandler(Control.KeyDownEvent, new KeyEventHandler(gridPatient_KeyDown), true);
            this.txtName.GotFocus += new RoutedEventHandler(txtName_GotFocus);
            this.Loaded += new RoutedEventHandler(FindPatientView_Loaded);
            this.Unloaded += new RoutedEventHandler(FindPatientView_Unloaded);
        }

        private object o;
        void FindPatientView_Loaded(object sender, RoutedEventArgs e)
        {
            o = gridPatient.GetValue(DataGrid.ItemsSourceProperty);
        }

        void FindPatientView_Unloaded(object sender, RoutedEventArgs e)
        {
            gridPatient.SetValue(DataGrid.ItemsSourceProperty, null);
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
            if (gridPatient.SelectedIndex >= 0)
            {
                gridPatient.Focus();
            }
        }
    }
}