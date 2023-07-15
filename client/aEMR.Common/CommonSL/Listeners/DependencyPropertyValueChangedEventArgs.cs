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

namespace aEMR.Common
{
    public class DependencyPropertyValueChangedEventArgs : System.EventArgs
    {
        internal DependencyPropertyValueChangedEventArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        ///  Gets the value of the property before the change.
        /// </summary>
        public object OldValue { get; private set; }

        /// <summary>
        /// Gets the value of the property after the change.
        /// </summary>
        public object NewValue { get; private set; }
    }
}
