using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Windows.Automation.Peers;
//using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using System.Threading;

namespace aEMR.Controls
{
    public class MouseClickManager
    {
        private event MouseButtonEventHandler _click;
        private event MouseButtonEventHandler _doubleClick;

        /// <summary>
        /// Indicate whether this object is clicked 
        /// </summary>
        private bool Clicked;
        public int DoubleClickTimeOut { get; set; }

        public MouseClickManager(int dblClickTimeout)
        {
            this.Clicked = false;
            this.DoubleClickTimeOut = dblClickTimeout;
        }

        public event MouseButtonEventHandler Click
        {
            add
            {
                _click += value;
            }
            remove
            {
                _click -= value;
            }
        }

        public event MouseButtonEventHandler DoubleClick
        {
            add
            {
                _doubleClick += value;
            }
            remove
            {
                _doubleClick -= value;
            }
        }

        private void OnClick(object sender, MouseButtonEventArgs args)
        {
            if (_click != null)
            {
                (sender as Control).Dispatcher.BeginInvoke(_click, sender, args);
            }
        }
        private void OnDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (_doubleClick != null)
            {
                _doubleClick(sender, args);
            }
        }

        public void HandleClick(object sender, MouseButtonEventArgs args)
        {
            lock(this)
            {
                if(this.Clicked)
                {
                    this.Clicked = false;
                    OnDoubleClick(sender, args);
                }
                else
                {
                    this.Clicked = true;
                    ParameterizedThreadStart threadStart = new ParameterizedThreadStart(Reset);
                    Thread t = new Thread(threadStart);
                    t.Start(args);
                }
            }
        }
        private void Reset(object state)
        {
            Thread.Sleep(this.DoubleClickTimeOut);
            lock(this)
            {
                if(this.Clicked)
                {
                    this.Clicked = false;
                    OnClick(this, (MouseButtonEventArgs)state);
                }
            }
        }
    }
    public class DataGridDoubleClickBehavior : Behavior<DataGrid>
    {
        private MouseClickManager _gridClickManager;
        public event EventHandler<MouseButtonEventArgs> DoubleClick;

        public DataGridDoubleClickBehavior()
        {
            _gridClickManager = new MouseClickManager(300);
            _gridClickManager.DoubleClick += new MouseButtonEventHandler(_gridClickManager_DoubleClick);
        }

        void _gridClickManager_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(DoubleClick != null)
            {
                DoubleClick(sender, e);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.LoadingRow += AssociatedObject_LoadingRow;
            this.AssociatedObject.UnloadingRow += AssociatedObject_UnloadingRow;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.LoadingRow -= AssociatedObject_LoadingRow;
            this.AssociatedObject.UnloadingRow -= AssociatedObject_UnloadingRow;
        }

        void AssociatedObject_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= _gridClickManager.HandleClick;
        }

        void AssociatedObject_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp += _gridClickManager.HandleClick;
        }
    }

}
