using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace aEMR.Controls
{
    public static class FrameworkElementExtensions
    {
        public static T FindParentOfType<T>(this FrameworkElement element)
        {
            var parent = VisualTreeHelper.GetParent(element) as FrameworkElement;

            while (parent != null)
            {
                if (parent is T)
                    return (T)(object)parent;

                parent = VisualTreeHelper.GetParent(parent) as FrameworkElement;
            }
            return default(T);
        }

        // Methods
        public static List<T> GetChildrenByType<T>(this UIElement element) where T : UIElement
        {
            return element.GetChildrenByType<T>(null);
        }

        public static List<T> GetChildrenByType<T>(this UIElement element, Func<T, bool> condition) where T : UIElement
        {
            List<T> results = new List<T>();
            GetChildrenByType<T>(element, condition, results);
            return results;
        }

        private static void GetChildrenByType<T>(UIElement element, Func<T, bool> condition, List<T> results) where T : UIElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                UIElement child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null)
                {
                    T t = child as T;
                    if (t != null)
                    {
                        if (condition == null)
                        {
                            results.Add(t);
                        }
                        else if (condition(t))
                        {
                            results.Add(t);
                        }
                    }
                    GetChildrenByType<T>(child, condition, results);
                }
            }
        }

        public static bool HasChildrenByType<T>(this UIElement element, Func<T, bool> condition) where T : UIElement
        {
            return (element.GetChildrenByType<T>(condition).Count != 0);
        }
    }
}
