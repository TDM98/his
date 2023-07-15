using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace aEMR.Common.Converters
{
    public class NullableValueConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
                return null;
            return value;
        }
        #endregion
    }
    public class NullableValueEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }
            if (value.GetType().Equals(typeof(string)) && string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NullableValueNotEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            if (value.GetType().Equals(typeof(string)) && string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }
            return true;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}