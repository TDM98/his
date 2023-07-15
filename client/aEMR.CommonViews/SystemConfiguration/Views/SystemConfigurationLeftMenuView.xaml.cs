using System.Windows;
namespace aEMR.SystemConfiguration.Views
{
    public partial class SystemConfigurationLeftMenuView
    {
        public SystemConfigurationLeftMenuView()
        {
            InitializeComponent();
        }
        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
        }
    }
}