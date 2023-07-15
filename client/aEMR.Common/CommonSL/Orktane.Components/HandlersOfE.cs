using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orktane.Components
{

    public sealed class Handler<E> : Handler<E, EventHandler<E>>
        where E : EventArgs
    {

        public Handler(Action<Object, E> action) 
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public Handler(Action<Object, E> action, Action<EventHandler<E>> removeAction)
            : base((a) => new EventHandler<E>(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~Handler()
        {
            Console.WriteLine("Releasing Handler<E> " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakHandler<E> : WeakHandler<E, EventHandler<E>>
        where E : EventArgs
    {

        public WeakHandler(Action<Object, E> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public WeakHandler(Action<Object, E> action, Action<EventHandler<E>> removeAction)
            : base((a) => new EventHandler<E>(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~WeakHandler()
        {
            Console.WriteLine("Releasing Weak Handler<E> " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class SingleHandler<E> : SingleHandler<E, EventHandler<E>>
        where E : EventArgs
    {

        public SingleHandler(Action<Object, E> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public SingleHandler(Action<Object, E> action, Action<EventHandler<E>> removeAction)
            : base((a) => new EventHandler<E>(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~SingleHandler()
        {
            Console.WriteLine("Releasing Single Handler<E> " + this.GetHashCode().ToString());
        }
#endif

    }

}
