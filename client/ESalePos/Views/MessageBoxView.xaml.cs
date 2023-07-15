using System.Windows.Controls;
namespace aEMRClient.Views
{
    public partial class MessageBoxView : UserControl
    {
        public MessageBoxView()
        {
            InitializeComponent();
            Ok.Focus();
        }
    }
}