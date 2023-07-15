using System.Windows;
using System.Windows.Media;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{

    public partial class ImmunizationsView: AxUserControl
    {
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        
        public ImmunizationsView()
        {
            InitializeComponent();
            this.Unloaded += new RoutedEventHandler(ImmunizationsView_Unloaded);
            this.Loaded += new RoutedEventHandler(ImmunizationsView_Loaded);
        }

        public void ImmunizationsView_Loaded(object sender, RoutedEventArgs e)
        {
          
        }
        public void ImmunizationsView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdCommonRecord.SetValue(DataGrid.ItemsSourceProperty, null);
        }
       

    }
}
