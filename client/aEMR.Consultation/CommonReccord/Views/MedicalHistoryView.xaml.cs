using System.Windows;
using System.Windows.Media;
using aEMR.Controls;

namespace aEMR.ConsultantEPrescription.CommonRecs.Views
{
    public partial class MedicalHistoryView : AxUserControl
    {
        static SolidColorBrush darkColor = new SolidColorBrush(Color.FromArgb(255, 15, 37, 63));
        static SolidColorBrush lightColor = new SolidColorBrush(Color.FromArgb(255, 75, 105, 137));
        
        //private bool author = true;
       
        #region Backup
        /*
        [Import(ViewModelTypes.DepartmentsViewModel)]
        public object ViewModel
        {
            set
            {
                DataContext = value; // Sets this page to the imported ViewModel
            }

        }
        */
        #endregion

        //PagedCollectionView globalPVC;
        public MedicalHistoryView()
        {
            InitializeComponent();
            
            this.Unloaded += new RoutedEventHandler(MedicalHistoryView_Unloaded);
            this.Loaded += new RoutedEventHandler(MedicalHistoryView_Loaded);
        }

        public void MedicalHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
        public void MedicalHistoryView_Unloaded(object sender, RoutedEventArgs e)
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
