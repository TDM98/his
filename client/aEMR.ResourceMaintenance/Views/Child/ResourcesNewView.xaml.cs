using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.Controls;


namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesNewView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ResourcesNewView : AxUserControl
    {
        
        public ResourcesNewView()
        {
            InitializeComponent();
            
        }
        void ResourcesNew_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        void ResourcesNew_Loaded(object sender, RoutedEventArgs e)
        {
                    
        }
        void InitData()
        {
            
        }

        private void refResourceType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

