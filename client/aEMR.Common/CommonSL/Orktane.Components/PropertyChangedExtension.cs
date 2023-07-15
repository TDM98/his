using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Orktane.Components
{

    public static class PropertyChangedExtension
    {

        public static void Notify<T>(this INotifyPropertyChanged target,
            Expression<Func<T>> propertySelector, Action<string> notifier)
        {
            if (notifier != null)
                notifier(GetPropertyName(propertySelector));
        }

        public static void Notify<T>(this Action<string> notifier,
            Expression<Func<T>> propertySelector)
        {
            if (notifier != null) 
                notifier(GetPropertyName(propertySelector));
        }

        public static void Notify<T>(this PropertyChangedEventHandler handler, Expression<Func<T>> propertySelector)
        {
            if (handler != null)
            {
                var memberExpression = GetMemberExpression(propertySelector);
                if (memberExpression != null)
                {
                    var _sender = ((ConstantExpression)memberExpression.Expression).Value;
                    handler(_sender, new PropertyChangedEventArgs(memberExpression.Member.Name));
                }
                //else we raise error?
            }
        }

        public static string GetPropertyName<EValue>(Expression<Func<EValue>> propertySelector)
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
#endif
            // all else
            return null;
        }

        public static MemberExpression GetMemberExpression<T>(Expression<Func<T>> propertySelector)
        {

#if SILVERLIGHT

            var _memberExpression = propertySelector.Body as MemberExpression;
            if (_memberExpression != null)
            {
                if (_memberExpression.Member.MemberType != MemberTypes.Property)
                        throw new ArgumentException("PropertySelector must select a property type.");
                return _memberExpression;
            }
#else

            var _unaryExpression = propertySelector.Body as UnaryExpression;
            if (_unaryExpression != null)
            {
                var _memberExpression = _unaryExpression.Operand as MemberExpression;
                if (_memberExpression != null)
                {
                    if (_memberExpression.Member.MemberType != MemberTypes.Property)
                        throw new ArgumentException("PropertySelector must select a property type.");
                    return _memberExpression;
                }
            }
#endif
            // all else
            return null;
        }

    }

}
