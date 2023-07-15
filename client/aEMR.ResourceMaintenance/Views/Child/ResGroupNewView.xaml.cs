using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResGroupNewView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ResGroupNewView : AxUserControl
    {
        public ResGroupNewView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ResGroupNewView_Loaded);
            this.Unloaded += new RoutedEventHandler(ResGroupNewView_Unloaded);

        }

        void ResGroupNewView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        void ResGroupNewView_Loaded(object sender, RoutedEventArgs e)
        {
        }

    }
}

