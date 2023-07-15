using System.Windows.Controls;

namespace aEMR.Common.Views
{
    public partial class AppointmentListingView : UserControl
    {
        public AppointmentListingView()
        {
            InitializeComponent();
        }
        public int DisplayIndex { get; set; }

        private void gridAppointments_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
