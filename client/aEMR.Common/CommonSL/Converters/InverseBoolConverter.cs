﻿using System;
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
    public class InverseBoolConverter:IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            else
            {
                bool val = (bool)value;
                return !val;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            else
            {
                bool val = (bool)value;
                return !val;
            }
        }

        #endregion
    }

    public class InverseBoolTrueConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return true;
            else
            {
                bool val = (bool)value;
                return !val;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return true;
            else
            {
                bool val = (bool)value;
                return !val;
            }
        }

        #endregion
    }
}
