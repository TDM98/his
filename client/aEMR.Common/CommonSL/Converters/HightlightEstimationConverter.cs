using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;

namespace aEMR.Common.Converters
{
    public class HightlightEstimationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = System.Convert.ToInt32(value);
            if (num > 0)
            {
                return Brushes.Yellow;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}