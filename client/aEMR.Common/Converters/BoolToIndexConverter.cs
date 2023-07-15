using System;
using System.Globalization;
using System.Windows.Data;
//ca
namespace aEMR.Common.Converters
{
    public class BoolToIndexConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value == true) ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value == 0) ? false : true ;
        }
        #endregion Methods
    }
}