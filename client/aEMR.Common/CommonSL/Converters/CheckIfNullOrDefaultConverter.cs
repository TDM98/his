using System;
using System.Globalization;
using System.Windows.Data;

namespace aEMR.Common.Converters
{
    public class CheckIfNullOrDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value.GetType().IsValueType && value.Equals(Activator.CreateInstance(value.GetType())))) return true;
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("CheckIfNullOrDefaultConverter: Method Not Implemented");
        }
        private T GetDefaultGeneric<T>()
        {
            return default(T);
        }
    }
}