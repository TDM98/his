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
    public class IsEnableConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsEnabled = false;
            if (value != null)
            {
                decimal results = System.Convert.ToDecimal(value); 
                if (results > 0)
                {
                    IsEnabled = true;
                }
            }
            return IsEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }

        #endregion
    }
    public class IsEnableFalseConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsEnabled = true;
            if (value != null)
            {
                decimal results = System.Convert.ToDecimal(value); ;
                if (results > 0)
                {
                    IsEnabled = false;
                }
            }
            return IsEnabled;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }

        #endregion
    }
}
