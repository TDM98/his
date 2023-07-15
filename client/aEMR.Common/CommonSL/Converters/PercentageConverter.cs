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
    public class PercentageConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d;
            if(value != null)
            {
                if(double.TryParse(value.ToString(), out d))
                {
                    return (Math.Round(d * 100)).ToString() + "%";
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d;
            if (value != null)
            {
                if (double.TryParse(value.ToString(), out d))
                {
                    return (Math.Round(d * 100)).ToString() + "%";
                }
            }
            return "";
        }

        #endregion
    }

    /// <summary>
    /// Khong co dau %
    /// </summary>
    public class PercentageConverter2 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //0.7 => 70
            double d;
            if (value != null)
            {
                if (double.TryParse(value.ToString(), out d))
                {
                    return (Math.Round(d * 100)).ToString();
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value == null)
            {
                return null;
            }
            //70 => 0.7
            double d;
            if (double.TryParse(value.ToString(), out d))
            {
                return d/100;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
    public class PercentageConverterHpt : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d;
            if (value != null)
            {
                if (double.TryParse(value.ToString(), out d))
                {
                    return (Math.Round(d * 100,2)).ToString();
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            double d;
            if (double.TryParse(value.ToString(), out d))
            {
                return d / 100;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }

    public class PercentageConverterHpt1 : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double d;
            if (value != null)
            {
                if (double.TryParse(value.ToString(), out d))
                {
                    return (Math.Round(d * 100, 2)).ToString();
                }
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            double d;
            if (double.TryParse(value.ToString(), out d))
            {
                return d / 100;
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}
