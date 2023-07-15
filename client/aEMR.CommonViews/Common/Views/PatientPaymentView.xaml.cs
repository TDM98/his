using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using aEMR.ViewContracts;

namespace aEMR.Common.Views
{
    public partial class PatientPaymentView:IPatientPaymentView
    {
        public PatientPaymentView()
        {
            InitializeComponent();
            Unloaded += PaymentSummaryView_Unloaded;
            Loaded += PaymentSummaryView_Loaded;
        }

        void PaymentSummaryView_Loaded(object sender, RoutedEventArgs e)
        {
            var binding = new Binding
                              {
                                  Source = DataContext,
                                  Path = new PropertyPath("PatientPayments"),
                                  Mode = BindingMode.OneWay
                              };

            gridPayable.SetBinding(DataGrid.ItemsSourceProperty, binding);
        }

        void PaymentSummaryView_Unloaded(object sender, RoutedEventArgs e)
        {
            gridPayable.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        public void TogglePrintColumnVisibility(bool visible)
        {
            var colPrint = gridPayable.Columns.SingleOrDefault(p =>
            (string)p.GetValue(NameProperty) == "colPrint");
            if(colPrint != null)
            {
                colPrint.Visibility = visible? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
