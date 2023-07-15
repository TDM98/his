using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Orktane.Components
{

    public sealed class PropertyChangedListenerHandler
    {
        string _propertyName;
        IWeakEventListener _listener;
        Action<PropertyChangedEventHandler> _removeAction;
        PropertyChangedEventHandler _eventHandler;

        public PropertyChangedListenerHandler(IWeakEventListener listener)
            : this(null, listener, (h) => { if (h != null) h -= h; }) { }

        public PropertyChangedListenerHandler(IWeakEventListener listener, Action<PropertyChangedEventHandler> removeAction)
            : this(null, listener, removeAction) { }

        public PropertyChangedListenerHandler(Expression<Func<Object>> propertySelector,
            IWeakEventListener listener,
            Action<PropertyChangedEventHandler> removeAction)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            _listener = listener;
            _removeAction = removeAction;
            if (propertySelector != null) _propertyName = PropertyChangedHandler.GetPropertyName(propertySelector);
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _listener = null;
            _removeAction = null;
            _eventHandler = null;
        }

        public static implicit operator PropertyChangedEventHandler(PropertyChangedListenerHandler handler)
        {
            handler._eventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                if (handler != null && handler._listener != null)
                {
                    if (!string.IsNullOrEmpty(handler._propertyName) &&
                        !string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase)) return;
                    handler._listener.ReceiveWeakEvent(typeof(PropertyChangedEventHandler), s, e);
                }
                else
                {
                    // since we don't have a listener, we ensure we unregister
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~PropertyChangedListenerHandler()
        {
            Console.WriteLine("Releasing Property Changed Listener Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class WeakPropertyChangedListenerHandler
    {
        string _propertyName;
        WeakReference _listener;
        Action<PropertyChangedEventHandler> _removeAction;
        PropertyChangedEventHandler _eventHandler;

        public WeakPropertyChangedListenerHandler(IWeakEventListener listener) 
            : this(null, listener, (h) => { if (h != null) h -= h; }) { }

        public WeakPropertyChangedListenerHandler(IWeakEventListener listener, Action<PropertyChangedEventHandler> removeAction) 
            : this(null, listener, removeAction) { }

        public WeakPropertyChangedListenerHandler(Expression<Func<Object>> propertySelector,
            IWeakEventListener listener)
            : this(propertySelector, listener, (h) => { if (h != null) h -= h; }) { }

        public WeakPropertyChangedListenerHandler(Expression<Func<Object>> propertySelector,
            IWeakEventListener listener,
            Action<PropertyChangedEventHandler> removeAction)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            _listener = new WeakReference(listener);
            _removeAction = removeAction;
            if (propertySelector != null) _propertyName = PropertyChangedHandler.GetPropertyName(propertySelector);
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            if (_listener != null) _listener.Target = null;
            _listener = null;
            _removeAction = null;
            _eventHandler = null;
        }

        public static implicit operator PropertyChangedEventHandler(WeakPropertyChangedListenerHandler handler)
        {
            handler._eventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                Object _listenerObj = null;
                if (handler != null && handler._listener != null && handler._listener.IsAlive)
                    _listenerObj = handler._listener.Target;

                // we use a flag coz if the listener returns false we need to remove it
                bool _removeHandlerFlag = false;
                if (_listenerObj != null)
                {
                    // we check if we only raise in specific cases
                    if (!string.IsNullOrEmpty(handler._propertyName) &&
                        !string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase)) return;
                    // if this returns false we unregister
                    if (((IWeakEventListener)_listenerObj).ReceiveWeakEvent(typeof(PropertyChangedEventHandler), s, e) == false)
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
        ~WeakPropertyChangedListenerHandler()
        {
            Console.WriteLine("Releasing Weak Property Changed Listener Handler " + this.GetHashCode().ToString());
        }
#endif

    }

    public sealed class SinglePropertyChangedListenerHandler
    {
        string _propertyName;
        IWeakEventListener _listener;
        Action<PropertyChangedEventHandler> _removeAction;
        PropertyChangedEventHandler _eventHandler;

        public SinglePropertyChangedListenerHandler(IWeakEventListener listener) 
            : this(null, listener, (h) => { if (h != null) h -= h; }) { }

        public SinglePropertyChangedListenerHandler(IWeakEventListener listener, Action<PropertyChangedEventHandler> removeAction)
            : this(null, listener, removeAction) { }

        public SinglePropertyChangedListenerHandler(Expression<Func<Object>> propertySelector,
            IWeakEventListener listener)
            : this(propertySelector, listener, (h) => { if (h != null) h -= h; }) { }

        public SinglePropertyChangedListenerHandler(Expression<Func<Object>> propertySelector,
            IWeakEventListener listener,
            Action<PropertyChangedEventHandler> removeAction)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            _listener = listener;
            _removeAction = removeAction;
            if (propertySelector != null) _propertyName = PropertyChangedHandler.GetPropertyName(propertySelector);
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _listener = null;
            _removeAction = null;
            _eventHandler = null;
        }

        public static implicit operator PropertyChangedEventHandler(SinglePropertyChangedListenerHandler handler)
        {
            handler._eventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                if (handler != null && handler._listener != null)
                {
                    if (!string.IsNullOrEmpty(handler._propertyName) &&
                        !string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase)) return;
                    handler._listener.ReceiveWeakEvent(typeof(PropertyChangedEventHandler), s, e);
                }
               
                // we ensure we unregister after the first call
                if (handler != null) handler.UnregisterHandler();
                handler = null;
                
            });
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~SinglePropertyChangedListenerHandler()
        {
            Console.WriteLine("Releasing Single Property Changed Listener Handler " + this.GetHashCode().ToString());
        }
#endif

    }

}
