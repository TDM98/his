using System.Windows;
using System.Windows.Controls;


namespace aEMR.Configuration.Hospitals.Views
{
    public partial class HospitalAgencyContentView : UserControl
    {
        public HospitalAgencyContentView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(RefMedicalServiceItemsView_Loaded);
        }

        void RefMedicalServiceItemsView_Loaded(object sender, RoutedEventArgs e)
        {
            //AxDataGridTemplateColumn col1 = dtgList.Columns[0] as AxDataGridTemplateColumn;
            //if (col1 != null)
            //{
            //    Binding binding = new Binding();
            //    binding.Source = dtgList.DataContext;
            //    binding.Path = new PropertyPath("bBtnEdit");
            //    binding.Mode = BindingMode.OneWay;
            //    binding.Converter = new BooleanToVisibilityConverter();
            //    col1.VisibilityBinding = binding;
            //    //col1.CellTemplate.
            //}
            
        }

       
    }
}
