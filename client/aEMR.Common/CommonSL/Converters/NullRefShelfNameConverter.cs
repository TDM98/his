using System;
using System.Windows.Data;
using System.Globalization;
using eHCMSLanguage;

namespace aEMR.Common.Converters
{
    public class NullRefShelfNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return eHCMSResources.Z1116_G1_ChuaXacDinh;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
