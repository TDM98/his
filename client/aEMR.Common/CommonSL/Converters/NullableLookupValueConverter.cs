using System;
using DataEntities;

namespace aEMR.Common.Converters
{
    public class NullableLookupValueConverter : EnumConverter
    {
        #region IValueConverter Members
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(Lookup) && (value as Lookup).LookupID == 0)
                return null;
            return value;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(Lookup) && (value as Lookup).LookupID == 0)
                return null;
            return value;
        }
        #endregion
    }
}
