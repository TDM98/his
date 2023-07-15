using System.Windows;
using System.Windows.Controls;

namespace aEMR.Controls
{
    public class AxTreeView : TreeView
    {
        public AxTreeView() : base()
        {
            base.Loaded += new RoutedEventHandler(AxTreeView_Loaded);
        }
        void AxTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            //this.ExpandAll();
        }
    }
}