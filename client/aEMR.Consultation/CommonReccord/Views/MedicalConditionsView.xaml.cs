using System.Windows;
using System.Windows.Media;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{

    public partial class MedicalConditionsView: AxUserControl
    {
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        
        //private bool author=true;
        
        public MedicalConditionsView()
        {
            InitializeComponent();
            
            this.Unloaded += new RoutedEventHandler(MedicalConditionsView_Unloaded);
            this.Loaded += new RoutedEventHandler(MedicalConditionsView_Loaded);
        }

        public void MedicalConditionsView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        public void MedicalConditionsView_Unloaded(object sender, RoutedEventArgs e)
        {
            //grdCommonRecord.SetValue(DataGrid.ItemsSourceProperty, null);
        }
       
        private void lnkEdit_Click(object sender, RoutedEventArgs e)
        {
            
            //row.Height DefaultCellStyle.BackColor = Color.Bisque;

        }

        private void lnkSave_Click(object sender, RoutedEventArgs e)
        {
            
        }

        
    }
}
