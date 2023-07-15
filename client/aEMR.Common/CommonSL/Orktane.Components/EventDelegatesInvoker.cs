using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Linq.Expressions;

namespace Orktane.Components
{

    public sealed class EventDelegateInvoker<E>
        where E : EventArgs
    {

        static readonly Dictionary<Type, Func<MethodInfo, DelegateInvoker<E>>> _activators = 
            new Dictionary<Type, Func<MethodInfo, DelegateInvoker<E>>>();
        static readonly Type _invokerGenericType = typeof(DelegateInvoker<Object, E>).GetGenericTypeDefinition();

        DelegateInvoker<E> _invoker;
        Type _targetType;
        MethodInfo _method;
        
        public EventDelegateInvoker(Type targetType, MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");
            _targetType = targetType;
            _method = method;
        }

        public EventDelegateInvoker(DelegateInvoker<E> invoker)
        {
            if (invoker == null) throw new ArgumentNullException("invoker");
            _invoker = invoker;
        }

        public void RaiseEvent(Object target, Object sender, E args)
        {
            if (_invoker == null) CreateInvoker();
            _invoker.Invoke(target, sender, args);
        }

        void CreateInvoker()
        {
            // lazy load in a way
            if (_targetType != null)
            {

                //var _invokerType = _invokerGenericType.MakeGenericType(typeof(E), _targetType, typeof(E));
                //_invoker = (DelegateInvoker<E>)_invokerType.GetConstructors()[0].Invoke(new object[] { _method });

                // todo - lock check
                if (_activators.ContainsKey(_targetType))
                    _invoker = _activators[_targetType](_method);
                else 
                {
                    var _func = CreateActivator();
                    _activators.Add(_targetType, _func);
                    _invoker = _func(_method);
                }
            }
            else
            {
                _invoker = new DelegateInvoker<E>(_method);
            }
        }

        Func<MethodInfo, DelegateInvoker<E>> CreateActivator()
        {
            
            // adopted from http://beaucrawford.net/post/Constructor-invocation-with-DynamicMethod.aspx
            var type = _invokerGenericType.MakeGenericType(typeof(E), _targetType, typeof(E));
            var _constParms = new Type[] { typeof(MethodInfo) };
#if SILVERLIGHT
            var _dynamicMethod = new DynamicMethod("DelegateInvoker" + type.Name, type, _constParms);
#else
            var _dynamicMethod = new DynamicMethod("DelegateInvoker" + type.Name, type, _constParms, type);
#endif
            var _ilGenerator = _dynamicMethod.GetILGenerator();
            
            var constructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, _constParms, null);
            _ilGenerator.Emit(OpCodes.Nop);
            _ilGenerator.Emit(OpCodes.Ldarg_0);
            _ilGenerator.Emit(OpCodes.Newobj, constructor);
            _ilGenerator.Emit(OpCodes.Ret);

            // note we actually create DelegateInvoker<T, E> but get it back as DelegateInvoker<T>
            var _delegate = _dynamicMethod.CreateDelegate(typeof(DelegateActivator<DelegateInvoker<E>, MethodInfo>)) 
                as DelegateActivator<DelegateInvoker<E>, MethodInfo>;
            Expression<Func<MethodInfo, DelegateInvoker<E>>> _exp = (m) => _delegate(m);
            return _exp.Compile();

        }

#region Internal Classes

        public delegate T DelegateActivator<T, A>(A arg1) 
            where T : class;

        public class DelegateInvoker<U>
            where U : EventArgs
        {

            EventDelegate<U> _delegate;

            protected DelegateInvoker() { }

            public DelegateInvoker(MethodInfo method)
            {
                if (method == null) throw new ArgumentNullException("method");
#if SILVERLIGHT
                _delegate = (EventDelegate<U>)Delegate.CreateDelegate(typeof(EventDelegate<U>), method);
#else
                _delegate = (EventDelegate<U>)Delegate.CreateDelegate(typeof(EventDelegate<U>), method, true);
#endif
            }

            public virtual void Invoke(Object target, Object sender, U args)
            {
                _delegate(sender, args);
            }

        }

        public class DelegateInvoker<T, Q>
            : DelegateInvoker<Q>
            where Q : EventArgs
        {

            EventDelegate<T, Q> _delegate;

            public DelegateInvoker(MethodInfo method)
                : base()
            {
                if (method == null) throw new ArgumentNullException("method");
#if SILVERLIGHT
                _delegate = (EventDelegate<T, Q>)Delegate.CreateDelegate(typeof(EventDelegate<T, Q>), method);
#else
                _delegate = (EventDelegate<T, Q>)Delegate.CreateDelegate(typeof(EventDelegate<T, Q>), method, true);
#endif
            }

            public override void Invoke(object target, object sender, Q args)
            {
                this.Invoke((T)target, sender, args);
            }

            public void Invoke(T target, object sender, Q args)
            {
                _delegate(target, sender, args);
            }

        }

#endregion

    }

}
