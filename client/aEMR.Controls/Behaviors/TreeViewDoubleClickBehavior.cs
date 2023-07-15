
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace aEMR.Controls
{
    public class TreeViewDoubleClickBehavior : Behavior<TreeView>
    {
        private MouseClickManager _gridClickManager;
        public event EventHandler<MouseButtonEventArgs> DoubleClick;

        public TreeViewDoubleClickBehavior()
        {
            _gridClickManager = new MouseClickManager(300);
            _gridClickManager.DoubleClick += new MouseButtonEventHandler(_gridClickManager_DoubleClick);
        }

        void _gridClickManager_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(sender, e);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
            this.AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        private void AssociatedObject_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.MouseLeftButtonUp -= _gridClickManager.HandleClick;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.MouseLeftButtonUp += _gridClickManager.HandleClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            this.AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
        }
    }
}