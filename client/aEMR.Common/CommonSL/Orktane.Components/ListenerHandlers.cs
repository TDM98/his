using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orktane.Components
{

    public sealed class ListenerHandler : ListenerHandler<EventArgs, EventHandler>
    {
        public ListenerHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public ListenerHandler(IWeakEventListener listener, Action<EventHandler> removeAction)
            : base((a) => new EventHandler(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~ListenerHandler()
        {
            Console.WriteLine("Releasing Listener Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakListenerHandler : WeakListenerHandler<EventArgs, EventHandler>
    {

        public WeakListenerHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public WeakListenerHandler(IWeakEventListener listener, Action<EventHandler> removeAction)
            : base((a) => new EventHandler(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~WeakListenerHandler()
        {
            Console.WriteLine("Releasing Weak Listener Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class SingleListenerHandler : SingleListenerHandler<EventArgs, EventHandler>
    {
        public SingleListenerHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public SingleListenerHandler(IWeakEventListener listener, Action<EventHandler> removeAction)
            : base((a) => new EventHandler(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~SingleListenerHandler()
        {
            Console.WriteLine("Releasing Single Listener Handler " + this.GetHashCode().ToString());
        }
#endif

    }

}
