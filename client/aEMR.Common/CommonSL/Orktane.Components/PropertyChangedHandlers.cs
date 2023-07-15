using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;

namespace Orktane.Components
{

    public sealed class PropertyChangedHandler
    {
        string _propertyName;
        Action<Object, PropertyChangedEventArgs> _action;
        Action<PropertyChangedEventHandler> _removeAction;
        PropertyChangedEventHandler _eventHandler;

        public PropertyChangedHandler(Action<Object, PropertyChangedEventArgs> action,
            Action<PropertyChangedEventHandler> removeAction) : this(null, action, removeAction) { }

        public PropertyChangedHandler(Expression<Func<Object>> propertySelector,
            Action<Object, PropertyChangedEventArgs> action,
            Action<PropertyChangedEventHandler> removeAction)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (propertySelector != null) _propertyName = GetPropertyName(propertySelector);
            _action = action;
            _removeAction = removeAction;
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _action = null;
            _propertyName = null;
            _removeAction = null;
            _eventHandler = null;
        }

        public static implicit operator PropertyChangedEventHandler(PropertyChangedHandler handler)
        {
            handler._eventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                if (handler != null && handler._action != null && !string.IsNullOrEmpty(handler._propertyName))
                {
                    if (string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase))
                        handler._action(s, e);
                }
                else if (handler != null && handler._action != null)
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
            return handler._eventHandler;
        }

        internal static string GetPropertyName(Expression<Func<Object>> propertySelector)
        {

#if SILVERLIGHT
            var _memberExpression = propertySelector.Body as MemberExpression;
            if (_memberExpression != null)
            {
                if (_memberExpression.Member.MemberType != MemberTypes.Property)
                        throw new ArgumentException("PropertySelector must select a property type.");
                var _sender = ((ConstantExpression)_memberExpression.Expression).Value;
                return _memberExpression.Member.Name;
            }
            return null; 
#else
            var _unaryExpression = propertySelector.Body as UnaryExpression;
            if (_unaryExpression != null)
            {
                var _memberExpression = _unaryExpression.Operand as MemberExpression;
                if (_memberExpression != null)
                {
                    if (_memberExpression.Member.MemberType != MemberTypes.Property)
                        throw new ArgumentException("PropertySelector must select a property type.");
                    return _memberExpression.Member.Name;
                }
            }
            return null;
#endif
        }

#if DEBUG && WRITETOCONSOLE
        ~PropertyChangedHandler()
        {
            Console.WriteLine("Releasing Property Changed Handler " + this.GetHashCode().ToString());
        } 
#endif

    }

    public sealed class WeakPropertyChangedHandler
    {
        string _propertyName;
        PropertyChangedEventHandler _eventHandler;
        WeakReference _target;
        bool _methodIsStatic;
        EventDelegateInvoker<PropertyChangedEventArgs> _invoker;
        Action<PropertyChangedEventHandler> _removeAction;

        public WeakPropertyChangedHandler(Action<Object, PropertyChangedEventArgs> action)
            : this(action, (h) => { if (h != null) h -= h; }) { }

        public WeakPropertyChangedHandler(Action<Object, PropertyChangedEventArgs> action,
            Action<PropertyChangedEventHandler> removeAction)
            : this(null, action, removeAction) { }

        public WeakPropertyChangedHandler(Expression<Func<Object>> propertySelector,
            Action<Object, PropertyChangedEventArgs> action)
            : this(propertySelector, action, (h) => { if (h != null) h -= h; }) { }

        public WeakPropertyChangedHandler(Expression<Func<Object>> propertySelector, 
            Action<Object, PropertyChangedEventArgs> action,
            Action<PropertyChangedEventHandler> removeAction)
        {
            if (action == null) throw new ArgumentNullException("action");

            _target = new WeakReference(action.Target);
            _removeAction = removeAction;
            if (propertySelector != null) _propertyName = PropertyChangedHandler.GetPropertyName(propertySelector);

            var _methodInfo = action.Method;
            _methodIsStatic = _methodInfo.IsStatic;
            _invoker = new EventDelegateInvoker<PropertyChangedEventArgs>(
                action.Target != null ? action.Target.GetType() : null, _methodInfo);
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            if (_target != null) _target.Target = null;
            _propertyName = null;
            _target = null;
            _invoker = null;
            _removeAction = null;
            _eventHandler = null;
        }

        public static implicit operator PropertyChangedEventHandler(WeakPropertyChangedHandler handler)
        {
            handler._eventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                Object _targetObj = null;
                if (handler != null && handler._target != null && handler._target.IsAlive)
                    _targetObj = handler._target.Target;

                if (_targetObj != null && handler._invoker != null)
                { 
                    if (!string.IsNullOrEmpty(handler._propertyName) &&
                        !string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase)) return;
                    handler._invoker.RaiseEvent(_targetObj, s, e);
                }
                else if (handler != null && handler._invoker != null && handler._methodIsStatic)
                {
                    if (!string.IsNullOrEmpty(handler._propertyName) &&
                        !string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase)) return; 
                    // this might be the case with lambda statements
                    handler._invoker.RaiseEvent(null, s, e);
                }
                else
                {
                    // we remove the handler since the target is not alive
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });

            // we return
            return handler._eventHandler;

        }

#if DEBUG && WRITETOCONSOLE
        ~WeakPropertyChangedHandler()
        {
            Console.WriteLine("Releasing Weak Property Changed Handler " + this.GetHashCode().ToString());
        } 
#endif

    }

    public sealed class SinglePropertyChangedHandler
    {

        string _propertyName;
        Action<Object, PropertyChangedEventArgs> _action;
        Action<PropertyChangedEventHandler> _removeAction;
        PropertyChangedEventHandler _eventHandler;

        public SinglePropertyChangedHandler(Action<Object, PropertyChangedEventArgs> action,
            Action<PropertyChangedEventHandler> removeAction)
            : this(null, action, removeAction) { }

        public SinglePropertyChangedHandler(Expression<Func<Object>> propertySelector,
            Action<Object, PropertyChangedEventArgs> action,
            Action<PropertyChangedEventHandler> removeAction)
        {
            if (action == null) throw new ArgumentNullException("action");
            _action = action;
            _removeAction = removeAction;
            if (propertySelector != null) _propertyName = PropertyChangedHandler.GetPropertyName(propertySelector);
        }

        public void UnregisterHandler()
        {
            if (_removeAction != null) _removeAction(_eventHandler);
            _action = null;
            _propertyName = null;
            _removeAction = null;
            _eventHandler = null;
        }

        public static implicit operator PropertyChangedEventHandler(SinglePropertyChangedHandler handler)
        {
            handler._eventHandler = new PropertyChangedEventHandler((s, e) =>
            {
                bool _removeHandlerFlag = true;
                if (handler != null && handler._action != null && !string.IsNullOrEmpty(handler._propertyName))
                {
                    // in a way we are waiting for the first property changed event to match this
                    if (string.Equals(e.PropertyName, handler._propertyName, StringComparison.OrdinalIgnoreCase))
                        handler._action(s, e);
                    else
                        _removeHandlerFlag = false; // wait for the next match
                }
                else if (handler != null && handler._action != null )
                {
                    handler._action(s, e);
                    _removeHandlerFlag = true;
                }
                // else if we don't match anything then true

                // we remove
                if (_removeHandlerFlag)
                {
                    if (handler != null) handler.UnregisterHandler();
                    handler = null;
                }
            });

            // we return
            return handler._eventHandler;
        }

#if DEBUG && WRITETOCONSOLE
        ~SinglePropertyChangedHandler()
        {
            Console.WriteLine("Releasing Single Property Changed Handler " + this.GetHashCode().ToString());
        } 
#endif

    }

}
