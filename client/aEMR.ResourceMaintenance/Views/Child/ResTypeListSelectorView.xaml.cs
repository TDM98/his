using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Controls;


namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResTypeListSelectorView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ResTypeListSelectorView : AxUserControl
    {
        
        public ResTypeListSelectorView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(ResTypeListSelectorView_Loaded);
            this.Unloaded += new RoutedEventHandler(ResTypeListSelectorView_Unloaded);
        }

        void ResTypeListSelectorView_Unloaded(object sender, RoutedEventArgs e)
        {
                      
        }

        void ResTypeListSelectorView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

    }
}

