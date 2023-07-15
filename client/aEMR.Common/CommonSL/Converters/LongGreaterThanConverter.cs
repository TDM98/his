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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace aEMR.Common.Converters
{
    public class LongGreaterThanConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long d;
            if(value != null)
            {
                if(long.TryParse(value.ToString(), out d))
                {
                    return d > long.Parse(parameter.ToString());
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongGreaterThanConverter: Not Implemented");
        }

        #endregion
    }
    public class LongLessThanEqualConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            long d;
            if (value != null)
            {
                if (long.TryParse(value.ToString(), out d))
                {
                    return d <= long.Parse(parameter.ToString());
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongGreaterThanConverter: Not Implemented");
        }
        #endregion
    }
    public class DecimalEqualConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }
            decimal d;
            decimal p;
            if (decimal.TryParse(value.ToString(), out d) && decimal.TryParse(parameter.ToString(), out p))
            {
                return d == p;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongGreaterThanConverter: Not Implemented");
        }
        #endregion
    }
    public class DecimalNotEqualConverter : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }
            decimal d;
            decimal p;
            if (decimal.TryParse(value.ToString(), out d) && decimal.TryParse(parameter.ToString(), out p))
            {
                return d != p;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("LongGreaterThanConverter: Not Implemented");
        }
        #endregion
    }
}