using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Controls;


namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResTypeNewView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ResTypeNewView : AxUserControl
    {
        
        public ResTypeNewView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ResTypeNewView_Loaded);
            this.Unloaded += new RoutedEventHandler(ResTypeNewView_Unloaded);
        }

        void ResTypeNewView_Unloaded(object sender, RoutedEventArgs e)
        {
                      
        }

        void ResTypeNewView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

    }
}

