using System.Windows;
using System.Windows.Controls;


namespace aEMR.Configuration.BedAllocations.Views
{
    public partial class UCRoomInfoView : UserControl
    {
        public UCRoomInfoView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCRoomInfoView_Loaded);
            this.Unloaded += new RoutedEventHandler(UCRoomInfoView_Unloaded);
        }

        void UCRoomInfoView_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        void UCRoomInfoView_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
