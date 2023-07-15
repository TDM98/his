using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace aEMR.Common.Converters
{
    public class EnumVisibilityConverter : EnumConverter
    {

        #region IValueConverter Members

        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)base.Convert(value,targetType,parameter,culture);
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;

            bool b = (visibility == Visibility.Visible);

            return base.ConvertBack(b, targetType, parameter, culture);
        }

        #endregion
    }
}
