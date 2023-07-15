using System.Windows.Data;
using System;

namespace aEMR.Common.Converters
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || System.Convert.ToInt32(value) == 0)
                return 10000;
            else
                return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("WidthConverter: Not Implemented");
        }
    }
}
