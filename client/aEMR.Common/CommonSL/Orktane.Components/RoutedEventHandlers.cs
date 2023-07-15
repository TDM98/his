using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orktane.Components
{

    public sealed class RoutedHandler : Handler<RoutedEventArgs, RoutedEventHandler>
    {

        public RoutedHandler(Action<Object, RoutedEventArgs> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public RoutedHandler(Action<Object, RoutedEventArgs> action, Action<RoutedEventHandler> removeAction)
            : base((a) => new RoutedEventHandler(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~RoutedHandler()
        {
            Console.WriteLine("Releasing Routed Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakRoutedHandler : WeakHandler<RoutedEventArgs, RoutedEventHandler>
    {

        public WeakRoutedHandler(Action<Object, RoutedEventArgs> action) 
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public WeakRoutedHandler(Action<Object, RoutedEventArgs> action, Action<RoutedEventHandler> removeAction)
            : base((a) => new RoutedEventHandler(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~WeakRoutedHandler()
        {
            Console.WriteLine("Releasing Routed Weak Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class SingleRoutedHandler : SingleHandler<RoutedEventArgs, RoutedEventHandler>
    {

        public SingleRoutedHandler(Action<Object, RoutedEventArgs> action)
            : base((a) => new RoutedEventHandler(a), action, (h) => { if (h != null) h -= h; }) { }

        public SingleRoutedHandler(Action<Object, RoutedEventArgs> action, Action<RoutedEventHandler> removeAction)
            : base((a) => new RoutedEventHandler(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~SingleRoutedHandler()
        {
            Console.WriteLine("Releasing Routed Single Handler " + this.GetHashCode().ToString());
        }
#endif

    }

}
