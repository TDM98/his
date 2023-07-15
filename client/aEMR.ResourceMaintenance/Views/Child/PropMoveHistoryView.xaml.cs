using System.ComponentModel.Composition;
using System.Windows;
using aEMR.Controls;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(PropMoveHistoryView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class PropMoveHistoryView : AxUserControl
    {
        
        public PropMoveHistoryView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(PropMoveHistoryView_Loaded);
            this.Unloaded += new RoutedEventHandler(PropMoveHistoryView_Unloaded);
        }
        void PropMoveHistoryView_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        void PropMoveHistoryView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        
        public void InitData()
        {
            
        }
        
    }
}

