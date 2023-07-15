using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using aEMR.ViewContracts;

namespace aEMR.ResourceMaintenance.Views
{
    [Export(typeof(ResourcesLeftMenuView))]
    public partial class ResourcesLeftMenuView : ILeftMenuView
    {
        public ResourcesLeftMenuView()
        {
            InitializeComponent();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mnuLeft.Height = LayoutRoot.ActualHeight - mnuLeft.Margin.Top - mnuLeft.Margin.Bottom;
        }

        private void ListBox_Loaded(object sender, RoutedEventArgs e)
        {
            ListBox curListBox = sender as ListBox;
            if (curListBox != null)
            {
                curListBox.Width = mnuLeft.ActualWidth - 5;
            }
        }
        public void ResetMenuColor()
        {
            var defaultStyle = (Style)Application.Current.Resources["MainFrame.HyperlinkButton.LeftMenu"];

            ResourcesMedListCmd.Style = defaultStyle;
            ResourcesOffListCmd.Style = defaultStyle;
            AllocResourcesMedCmd.Style = defaultStyle;
            AllocResourcesOffCmd.Style = defaultStyle;
            TranfResourcesCmd.Style = defaultStyle;
            ResourceMaintenanceLog_Mgnt.Style = defaultStyle;
            ResourceMaintenanceLog_AddNewMgnt.Style = defaultStyle;
            
        }
    }
}
