using System;

using System.Windows.Data;

using System.Linq;


namespace aEMR.Common.Converters
{
    public class ConcatStringWithDelimiterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
        object parameter, System.Globalization.CultureInfo culture)
        {
            return String.Join(", ", values.Where(o => o != null && !string.IsNullOrWhiteSpace(o.ToString())));
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DateTimeFromToConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Length >= 2)
            {
                DateTime? dateFrom = values[0] as DateTime?;
                DateTime? dateTo = values[1] as DateTime?;

                return String.Format("({0} - {1})", dateFrom.HasValue ? dateFrom.Value.ToString("HH:mm") : "?", dateTo.HasValue ? dateTo.Value.ToString("HH:mm") : "?");
            }
            else
            {
                return "";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
