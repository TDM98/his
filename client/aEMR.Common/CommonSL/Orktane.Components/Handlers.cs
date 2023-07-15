using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Orktane.Components
{

    public sealed class Handler : Handler<EventArgs, EventHandler>
    {

        public Handler(Action<Object, EventArgs> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public Handler(Action<Object, EventArgs> action, Action<EventHandler> removeAction)
            : base((a) => new EventHandler(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~Handler()
        {
            Console.WriteLine("Releasing Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakHandler : WeakHandler<EventArgs, EventHandler>
    {

        public WeakHandler(Action<Object, EventArgs> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public WeakHandler(Action<Object, EventArgs> action, Action<EventHandler> removeAction)
            : base((a) => new EventHandler(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE && WRITETOCONSOLE
        ~WeakHandler()
        {
            Console.WriteLine("Releasing Weak Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class SingleHandler : SingleHandler<EventArgs, EventHandler>
    {

        public SingleHandler(Action<Object, EventArgs> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public SingleHandler(Action<Object, EventArgs> action, Action<EventHandler> removeAction)
            : base((a) => new EventHandler(a), action, removeAction) { }

#if DEBUG && WRITETOCONSOLE
        ~SingleHandler()
        {
            Console.WriteLine("Releasing Single Handler " + this.GetHashCode().ToString());
        }
#endif

    }

}
