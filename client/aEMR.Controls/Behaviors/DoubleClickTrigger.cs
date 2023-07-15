using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Windows.Threading;
//using System.ServiceModel.Channels;

namespace aEMR.Controls
{
    public class DoubleClickTrigger:TriggerBase<UIElement>
    {
        private readonly DispatcherTimer timer;
        public DoubleClickTrigger() 
        {
            timer=new DispatcherTimer
            {
                Interval=new TimeSpan(0,0,0,0,200)
            };
            timer.Tick+=new EventHandler(timer_Tick);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseLeftButtonDown -= new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }

        void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(!timer.IsEnabled)
            {
                timer.Start();
                return;
            }
            timer.Stop();
            InvokeActions(null);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
        }
    }
}
