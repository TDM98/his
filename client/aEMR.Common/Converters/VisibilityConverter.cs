using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace aEMR.Common.Converters
{
    public class VisibilityConverter:IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visible = false;
            if (value != null)
            {
                visible = (bool)value;
            }
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibility = (Visibility)value;

            return (visibility == Visibility.Visible);
        }

        #endregion
    }

    public class BoolToXConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? "x" : "";
        }

        /// <summary>
        /// Converts a value <see cref="Visibility"/> value to a boolean value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value) == "x";
        }
    }
}
