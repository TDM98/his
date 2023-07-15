using System;
using System.Windows.Data;

namespace aEMR.Common.Converters
{
    public class StringEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }
            return String.Compare(value.ToString().Trim(), parameter.ToString().Trim(), StringComparison.CurrentCultureIgnoreCase) == 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
