using System.Windows;
using System.Windows.Controls;


namespace aEMR.UserAccountManagement.Views
{
    public partial class GridControlView : UserControl
    {
        public GridControlView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(GridControlView_Loaded);
            this.Unloaded += new RoutedEventHandler(GridControlView_Unloaded);
            
        }

        void GridControlView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void GridControlView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
