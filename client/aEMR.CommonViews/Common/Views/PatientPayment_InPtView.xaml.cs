using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace aEMR.Common.Views
{
    public partial class PatientPayment_InPtView
    {
        public PatientPayment_InPtView()
        {
            InitializeComponent();
            this.Unloaded += PaymentSummaryView_Unloaded;
            this.Loaded += PaymentSummaryView_Loaded;
        }

        void PaymentSummaryView_Loaded(object sender, RoutedEventArgs e)
        {
            var binding = new Binding { Source = DataContext, Path = new PropertyPath("PatientPayments"), Mode = BindingMode.OneWay };
            gridPayable.SetBinding(DataGrid.ItemsSourceProperty, binding);
        }

        void PaymentSummaryView_Unloaded(object sender, RoutedEventArgs e)
        {
            gridPayable.SetValue(DataGrid.ItemsSourceProperty, null);
        }
    }
}
