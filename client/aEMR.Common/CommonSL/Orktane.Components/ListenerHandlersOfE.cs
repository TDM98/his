using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Orktane.Components
{

    public sealed class ListenerHandler<E> : ListenerHandler<E, EventHandler<E>>
        where E : EventArgs
    {
         public ListenerHandler(IWeakEventListener listener) 
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public ListenerHandler(IWeakEventListener listener, Action<EventHandler<E>> removeAction)
            : base((a) => new EventHandler<E>(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~ListenerHandler()
        {
            Console.WriteLine("Releasing Listener Handler<E> " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakListenerHandler<E> : WeakListenerHandler<E, EventHandler<E>>
        where E : EventArgs
    {

        public WeakListenerHandler(IWeakEventListener listener) 
            : this(listener, (h) => { if (h != null) h -= h; }) { }

         public WeakListenerHandler(IWeakEventListener listener, Action<EventHandler<E>> removeAction) 
             : base((a) => new EventHandler<E>(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
         ~WeakListenerHandler()
         {
             Console.WriteLine("Releasing Weak Listener Handler<E> " + this.GetHashCode().ToString());
         }
#endif

    }

    public sealed class SingleListenerHandler<E> : SingleListenerHandler<E, EventHandler<E>>
        where E : EventArgs
    {

        public SingleListenerHandler(IWeakEventListener listener)
            : this(listener, (h) => { if (h != null) h -= h; }) { }

        public SingleListenerHandler(IWeakEventListener listener, Action<EventHandler<E>> removeAction)
            : base((a) => new EventHandler<E>(a), listener, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~SingleListenerHandler()
         {
             Console.WriteLine("Releasing Single Listener Handler<E> " + this.GetHashCode().ToString());
         }
#endif

    }

}
