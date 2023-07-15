using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Orktane.Components
{

    public class Handler<E, H>
        where E : EventArgs
    {

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static Handler()
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

        Action<Object, E> _action;
        H _eventHandler;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;

        public Handler(Action<Object, E> action)
            : this((a) => _createDelegate(a), action, (h) => _removeDelegate(h)) { }

        public Handler(Func<Action<Object, E>, H> createAction, Action<Object, E> action, Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (action == null) throw new ArgumentNullException("action");
            _createAction = createAction;
            _action = action;
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _action = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(Handler<E, H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                if (handler != null && handler._createAction != null)
                {
                    handler._action(s, e);
                }
                else
                {
                    // if we don't have a action thing
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });

            // we return
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~Handler()
        {
            Console.WriteLine("Releasing Handler<E, H> " + this.GetHashCode().ToString());
        }
#endif

    }

    public class WeakHandler<E, H>
        where E : EventArgs
    {

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static WeakHandler()
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

        WeakReference _target;
        H _eventHandler;
        bool _methodIsStatic;
        EventDelegateInvoker<E> _invoker;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;

        public WeakHandler(Action<Object, E> action)
            : this((a) => _createDelegate(a), action, (h) => _removeDelegate(h)) { }

        public WeakHandler(Func<Action<Object, E>, H> createAction, Action<Object, E> action, Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (action == null) throw new ArgumentNullException("action");
            
            _createAction = createAction;
            _target = new WeakReference(action.Target);
            _removeAction = removeAction;

            var _methodInfo = action.Method;
            _methodIsStatic = _methodInfo.IsStatic;
            _invoker = new EventDelegateInvoker<E>(action.Target != null ? action.Target.GetType() : null, _methodInfo);
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            if (_target != null) _target.Target = null;
            _target = null;
            _invoker = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(WeakHandler<E, H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                Object _targetObj = null;
                if (handler != null && handler._target != null && handler._target.IsAlive)
                    _targetObj = handler._target.Target;

                if (_targetObj != null && handler._invoker != null)
                {
                    handler._invoker.RaiseEvent(_targetObj, s, e);
                }
                else if (handler != null && handler._invoker != null && handler._methodIsStatic)
                {
                    handler._invoker.RaiseEvent(null, s, e);
                }
                else
                {
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });

            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~WeakHandler()
        {
            Console.WriteLine("Releasing Weak Handler<E, H> " + this.GetHashCode().ToString());
        }
#endif

    }

    public class SingleHandler<E, H>
        where E : EventArgs
    {

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static SingleHandler()
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

        Action<Object, E> _action;
        H _eventHandler;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;

        public SingleHandler(Action<Object, E> action)
            : this((a) => _createDelegate(a), action, (h) => _removeDelegate(h)) { }

        public SingleHandler(Func<Action<Object, E>, H> createAction, Action<Object, E> action, Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (action == null) throw new ArgumentNullException("action");
            _createAction = createAction;
            _action = action;
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _action = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(SingleHandler<E, H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                if (handler != null && handler._action != null) handler._action(s, e);

                // we remove what ever the case
                if (handler != null) handler.UnregisterHandler();
                handler = null;
            });

            // we return
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~SingleHandler()
        {
            Console.WriteLine("Releasing Single Handler<E, H> " + this.GetHashCode().ToString());
        }
#endif

    }

#if DEBUG

    public class InvokeWeakHandler<E, H>
        where E : EventArgs
    {

#region Static Per Type

        protected readonly static Func<Action<Object, E>, H> _createDelegate;
        protected readonly static Action<H> _removeDelegate;

        static InvokeWeakHandler()
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

        WeakReference _target;
        MethodInfo _methodInfo;
        H _eventHandler;
        Func<Action<Object, E>, H> _createAction;
        Action<H> _removeAction;

        public InvokeWeakHandler(Action<Object, E> action)
            : this((a) => _createDelegate(a), action, (h) => _removeDelegate(h)) { }

        public InvokeWeakHandler(Func<Action<Object, E>, H> createAction, Action<Object, E> action, Action<H> removeAction)
        {
            if (createAction == null) throw new ArgumentNullException("createAction");
            if (action == null) throw new ArgumentNullException("action");
            _createAction = createAction;
            _target = new WeakReference(action.Target);
            _methodInfo = action.Method;
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            if (_target != null) _target.Target = null;
            _target = null;
            _methodInfo = null;
            _createAction = null;
            _removeAction = null;
            _eventHandler = default(H);
        }

        public static implicit operator H(InvokeWeakHandler<E, H> handler)
        {
            handler._eventHandler = handler._createAction((s, e) =>
            {
                Object _targetObj = null;
                if (handler != null && handler._target != null && handler._target.IsAlive)
                    _targetObj = handler._target.Target;
                
                if (_targetObj != null && handler._methodInfo != null)
                {
                    handler._methodInfo.Invoke(_targetObj, new object[] { s, e });
                }
                else if (handler != null && handler._methodInfo != null && handler._methodInfo.IsStatic)
                {
                    handler._methodInfo.Invoke(null, new object[] { s, e });
                }
                else
                {
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });

            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~InvokeWeakHandler()
        {
            Console.WriteLine("Releasing Invoke Weak Handler<E, H> " + this.GetHashCode().ToString());
        }
#endif

    }

#endif

}
