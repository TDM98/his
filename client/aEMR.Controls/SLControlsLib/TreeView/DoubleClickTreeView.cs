using aEMR.Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace aEMR.Controls
{
    public class DoubleClickTreeView : TreeView
    {
        private TreeViewDoubleClickBehavior dblClickBehavior;
        public event EventHandler<EventArgs<object>> DblClick;
        public DoubleClickTreeView()
        {
            dblClickBehavior = new TreeViewDoubleClickBehavior();
            System.Windows.Interactivity.Interaction.GetBehaviors(this).Add(dblClickBehavior);

            this.Unloaded += DoubleClickTreeView_Unloaded;
            this.Loaded += DoubleClickTreeView_Loaded;
        }

        private void DoubleClickTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            dblClickBehavior.DoubleClick += new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }

        private void DoubleClickTreeView_Unloaded(object sender, RoutedEventArgs e)
        {
            dblClickBehavior.DoubleClick -= new EventHandler<MouseButtonEventArgs>(dblClickBehavior_DoubleClick);
        }

        void dblClickBehavior_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DblClick != null)
            {
                DblClick(sender, new EventArgs<object>(((DoubleClickTreeView)sender).SelectedItem));
            }
        }
    }
}