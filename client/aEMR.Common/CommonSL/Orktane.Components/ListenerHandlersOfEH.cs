using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Linq.Expressions;

namespace Orktane.Components
{

    public class ListenerHandler<E, H>
        where E : EventArgs
    {

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static ListenerHandler()
        {

#if SILVERLIGHT
            _createDelegate = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _removeDelegate = (h) => Delegate.Remove((Delegate)(object)h, (Delegate)(object)h);
#else
            Expression<Func<Action<Object, E>, H>> _createExpr
                = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _createDelegate = _createExpr.Compile();

            Expression<Action<H>> _removeExpr = (h) => Delegate.Remove((Delegate)(object)h, (Delegate)(object)h);
            _removeDelegate = _removeExpr.Compile();
#endif

        }

#endregion

        IWeakEventListener _listener;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;
        H _eventHandler;

        public ListenerHandler(IWeakEventListener listener)
            : this((a) => _createDelegate(a), listener, (h) => _removeDelegate(h)) { }

        public ListenerHandler(Func<Action<Object, E>, H> createAction, IWeakEventListener listener, Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (listener == null) throw new ArgumentNullException("listener");
            _createAction = createAction;
            _listener = listener;
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _listener = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(ListenerHandler<E, H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                // we use a flag coz if the listener returns false we need to remove it
                bool _removeHandlerFlag = false;

                if (handler != null && handler._listener != null)
                {
                    if (handler._listener.ReceiveWeakEvent(typeof(H), s, e) == false)
                        _removeHandlerFlag = true;
                }
                else
                {
                    // since we don't have a listener, we ensure we unregister
                    _removeHandlerFlag = true;
                }

                // we remove
                if (_removeHandlerFlag)
                {
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }

            });
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~ListenerHandler()
        {
            Console.WriteLine("Releasing Listener Handler<E,H> " + this.GetHashCode().ToString());
        }
#endif

    }

    public class WeakListenerHandler<E, H>
        where E : EventArgs
    {

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static WeakListenerHandler()
        {

#if SILVERLIGHT
            _createDelegate = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _removeDelegate = (h) => Delegate.Remove((Delegate)(object)h, (Delegate)(object)h);
#else
            Expression<Func<Action<Object, E>, H>> _createExpr
                = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _createDelegate = _createExpr.Compile();

            Expression<Action<H>> _removeExpr = (h) => Delegate.Remove((Delegate)(object)h, (Delegate)(object)h);
            _removeDelegate = _removeExpr.Compile();
#endif

        }

#endregion

        WeakReference _listener;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;
        H _eventHandler;

        public WeakListenerHandler(IWeakEventListener listener)
            : this((a) => _createDelegate(a), listener, (h) => _removeDelegate(h)) { }

        public WeakListenerHandler(Func<Action<Object, E>, H> createAction, IWeakEventListener listener, 
            Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (listener == null) throw new ArgumentNullException("listener");
            _createAction = createAction;
            _listener = new WeakReference(listener);
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            if (_listener != null) _listener.Target = null;
            _listener = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(WeakListenerHandler<E,H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                Object _listenerObj = null;
                if (handler != null && handler._listener != null && handler._listener.IsAlive)
                    _listenerObj = handler._listener.Target;

                // we use a flag coz if the listener returns false we need to remove it
                bool _removeHandlerFlag = false;
                if (_listenerObj != null)
                {
                    // if we recieve a false signal then
                    if (((IWeakEventListener)_listenerObj).ReceiveWeakEvent(typeof(H), s, e) == false)
                        _removeHandlerFlag = true;
                }
                else
                {
                    // if we don't have a listener then just remove
                    _removeHandlerFlag = true;
                }

                if (_removeHandlerFlag)
                {
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~WeakListenerHandler()
        {
            Console.WriteLine("Releasing Weak Listener Handler<E,H> " + this.GetHashCode().ToString());
        }
#endif

    }

    public class SingleListenerHandler<E, H>
        where E : EventArgs
    {
        IWeakEventListener _listener;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;
        H _eventHandler;

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static SingleListenerHandler()
        {

#if SILVERLIGHT
            _createDelegate = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _removeDelegate = (h) => Delegate.Remove((Delegate)(object)h, (Delegate)(object)h);
#else
            Expression<Func<Action<Object, E>, H>> _createExpr
                = (a) => (H)(Object)Delegate.CreateDelegate(typeof(H), a.Target, a.Method);
            _createDelegate = _createExpr.Compile();

            Expression<Action<H>> _removeExpr = (h) => Delegate.Remove((Delegate)(object)h, (Delegate)(object)h);
            _removeDelegate = _removeExpr.Compile();
#endif

        }

#endregion

        public SingleListenerHandler(IWeakEventListener listener)
            : this((a) => _createDelegate(a), listener, (h) => _removeDelegate(h)) { }

        public SingleListenerHandler(Func<Action<Object, E>, H> createAction, IWeakEventListener listener, 
            Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (listener == null) throw new ArgumentNullException("listener");
            _createAction = createAction;
            _listener = listener;
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _listener = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(SingleListenerHandler<E, H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                if (handler != null && handler._listener != null)
                    handler._listener.ReceiveWeakEvent(typeof(H), s, e);

                // we unregister what ever the case
                if (handler != null) handler.UnregisterHandler();
                handler = null;
            });
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~SingleListenerHandler()
        {
            Console.WriteLine("Releasing Single Listener Handler<E,H> " + this.GetHashCode().ToString());
        }
#endif

    }

}
