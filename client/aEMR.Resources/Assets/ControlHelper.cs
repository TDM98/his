
using System.Windows;
using System.Windows.Controls;

namespace aEMR.Helper.Classes
{
    public static class MyControlHelper
    {
        /// <summary>  
        /// The disable double click property  
        /// </summary>  
        public static readonly DependencyProperty DisableDoubleClickProperty =
            DependencyProperty.RegisterAttached("DisableDoubleClick", typeof(bool), typeof(MyControlHelper), new FrameworkPropertyMetadata(false, OnDisableDoubleClickChanged));

        /// <summary>  
        /// Sets the disable double click.  
        /// </summary>  
        /// <param name="element">The element.</param>  
        /// <param name="value">if set to <c>true</c> [value].</param>  
        public static void SetDisableDoubleClick(UIElement element, bool value)
        {
            element.SetValue(DisableDoubleClickProperty, value);
        }

        /// <summary>  
        /// Gets the disable double click.  
        /// </summary>  
        /// <param name="element">The element.</param>  
        /// <returns></returns>  
        public static bool GetDisableDoubleClick(UIElement element)
        {
            return (bool)element.GetValue(DisableDoubleClickProperty);
        }

        /// <summary>  
        /// Called when [disable double click changed].  
        /// </summary>  
        /// <param name="d">The d.</param>  
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>  
        private static void OnDisableDoubleClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            if ((bool)e.NewValue)
            {
                control.PreviewMouseDown += (sender, args) =>
                {
                    if (args.ClickCount > 1)
                    {
                        args.Handled = true;
                    }
                };
            }
        }
    }
}

