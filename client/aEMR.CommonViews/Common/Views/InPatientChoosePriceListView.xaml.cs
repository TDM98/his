using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;
using aEMR.Controls;
using System;
using DataEntities;
using System.Windows.Forms;

namespace aEMR.Common.Views
{
    public partial class InPatientChoosePriceListView : Window, IInPatientChoosePriceList
    {
        public InPatientChoosePriceListView()
        {
            InitializeComponent();
            SizeToContent = SizeToContent.WidthAndHeight;
        }

        public InPatientBillingInvoice RecalBillingInvoice { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }

        private void CheckBox_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CenterWindowOnScreen();
        }

        private void CenterWindowOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth - 100;
            double screenHeight = SystemParameters.PrimaryScreenHeight - 50;
            double windowWidth = double.IsNaN(Width) ? 0 : Width;
            double windowHeight = double.IsNaN(Height) ? 0 : Height;
            Left = (screenWidth - windowWidth) / 2;
            Top = (screenHeight - windowHeight) / 2;
            //WindowStartupLocation = WindowStartupLocation.CenterScreen;

            //Screen screen = Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            //Left = screen.Bounds.X + ((screen.Bounds.Width - ActualWidth) / 2) + 0;
            //Top = screen.Bounds.Y + ((screen.Bounds.Height - ActualHeight) / 2) + 0;
        }
    }
}
