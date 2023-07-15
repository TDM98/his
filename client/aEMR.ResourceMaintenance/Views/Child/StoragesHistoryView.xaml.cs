using System.Windows;
using aEMR.Controls;
using System.ComponentModel.Composition;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(StoragesHistoryView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoragesHistoryView : AxUserControl
    {
        
        public StoragesHistoryView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(StoragesHistoryView_Loaded);
            this.Unloaded += new RoutedEventHandler(StoragesHistoryView_Unloaded);
        }
        void StoragesHistoryView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
        void StoragesHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
    }
}

