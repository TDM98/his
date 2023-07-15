using System;

namespace aEMR.Common.Converters
{
    public class QtyRemainingConverter : EnumConverter
    {

        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "(SL Tồn:" + new DecimalConverter3().Convert(value, null, null, null) + ")";
            //string.Format("(SL Tồn: {0})", value
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
