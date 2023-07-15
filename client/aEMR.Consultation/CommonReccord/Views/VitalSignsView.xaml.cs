using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{

    public partial class VitalSignsView : AxUserControl
    {        
        //private int pPatientID;
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        
        public VitalSignsView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(VitalSignsView_Loaded);
            this.Unloaded += new RoutedEventHandler(VitalSignsView_Unloaded);
        }

        void VitalSignsView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdCommonRecord.SetValue(DataGrid.ItemsSourceProperty, null);
        }

        void VitalSignsView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void grdVitalSigns_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {            
            //e.Row.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 125, 125));          
            e.Row.Background = new SolidColorBrush(Colors.Orange);
            e.Row.BorderBrush = new SolidColorBrush(Colors.Purple);
            e.Row.Foreground = new SolidColorBrush(Colors.Magenta);
        }
        
    }
}
