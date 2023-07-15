using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orktane.Components
{

    public sealed class ListenerRoutedHandler : ListenerHandler<RoutedEventArgs, RoutedEventHandler>
    {

        public ListenerRoutedHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public ListenerRoutedHandler(IWeakEventListener listener, Action<RoutedEventHandler> removeAction)
            : base((a) => new RoutedEventHandler(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~ListenerRoutedHandler()
        {
            Console.WriteLine("Releasing Listener Routed Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakListenerRoutedHandler : WeakListenerHandler<RoutedEventArgs, RoutedEventHandler>
    {

        public WeakListenerRoutedHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public WeakListenerRoutedHandler(IWeakEventListener listener, Action<RoutedEventHandler> removeAction)
            : base((a) => new RoutedEventHandler(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~WeakListenerRoutedHandler()
        {
            Console.WriteLine("Releasing Weak Listener Routed Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class SingleListenerRoutedHandler : SingleListenerHandler<RoutedEventArgs, RoutedEventHandler>
    {

        public SingleListenerRoutedHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public SingleListenerRoutedHandler(IWeakEventListener listener, Action<RoutedEventHandler> removeAction)
            : base((a) => new RoutedEventHandler(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~SingleListenerRoutedHandler()
        {
            Console.WriteLine("Releasing Single Listener Routed Handler " + this.GetHashCode().ToString());
        }
#endif

    }

}
